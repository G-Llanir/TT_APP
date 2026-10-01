using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using TT_Flow.App.Paginas.RRHH.Solicitacoes;
using TT_Hub.App.Paginas.RRHH;
using static Permissao.RRHH;
using static TT.FrameWork.BD;
using Funcoes = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class RelatorioSeguro : System.Web.UI.Page
    {
        string sTituloPagina = "Relatório Seguro";
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores";

        public List<cls_Relatorio_Seguro> bs_Relatorio_Seguro
        {
            get
            {
                if (ViewState["bs_Relatorio_Seguro"] == null)
                {
                    ViewState["bs_Relatorio_Seguro"] = new List<cls_Relatorio_Seguro>();
                }
                return (List<cls_Relatorio_Seguro>)ViewState["bs_Relatorio_Seguro"];
            }
            set
            {
                ViewState["bs_Relatorio_Seguro"] = value;
            }
        }

        public List<cls_Relatorio_Seguro_Detalhe> bs_Relatorio_Seguro_Detalhe
        {
            get
            {
                if (ViewState["bs_Relatorio_Seguro_Detalhe"] == null)
                {
                    ViewState["bs_Relatorio_Seguro_Detalhe"] = new List<cls_Relatorio_Seguro_Detalhe>();
                }
                return (List<cls_Relatorio_Seguro_Detalhe>)ViewState["bs_Relatorio_Seguro_Detalhe"];
            }
            set
            {
                ViewState["bs_Relatorio_Seguro_Detalhe"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                manual.sNomeArquivo = "Manual_RelatorioSeguro.pdf";

                if (Request.QueryString["action"] == "export")
                {
                    if (Session["Relatorio_Seguro"] != null)
                    {
                        cls_Relatorio_Seguro relatorio = (cls_Relatorio_Seguro)Session["Relatorio_Seguro"];
                        ExportarRelatorioSeguroParaXLS(relatorio, "Relatorio_Seguro_");

                        RelatorioSeguro_Detalhe();
                    }
                    
                    return;
                }

                Funcoes.ValidaPermissao(Permissao.RRHH.RelatorioSeguro.Consultar, true);

                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;

                PopulaCombo();

                div_gvConsulta.Visible = false;
                ModoEdicao(false);
            }
            else
            {
                var requestTarget = Page.Request["__EVENTTARGET"];

                if (requestTarget == "funcao_Excluir")
                    ExcluirRelatorio();
            }

            if (!Funcoes.ValidaPermissao(Permissao.RRHH.RelatorioSeguro.GerarRelatorio))
            {
                btnNovoRelatorio.Visible = false;
                btnGerarContasPagar.Visible = false;
            }

            RegistraScript();
        }

        #region | Metodo Banco de Dados

        private void Pesquisar()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "Consultar_Relatorio_Seguro" },
                { "@dtInicio_Seguro", txtdtInicio.Text },
                { "@dtFinal_Seguro", txtdtFinal.Text },
                { "@idEmpresa", ddlsEmpresa.SelectedValue },
                { "@idSeguro", ddlsSeguradora.SelectedValue },
                { "@nTipo", "0" }
            };
            DataTable tb = BD.ExecutarDataTable(sProcedure, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                div_gvConsulta.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb, 0, "asc"), true);
            }
            else
            {
                div_gvConsulta.Visible = false;
                MensagemPagina.MostraMensagem_Erro("Nenhum registro encontrado");
            }
        }        

        private void RelatorioSeguro_Detalhe()
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "Consultar_Relatorio_Seguro" },
                    { "@idRelatorioSeguro", hddidRelatorioSeguro.Value },
                    { "@nTipo", "2" }
                };
                DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(ds, out string sErro))
                {
                    PopulaClasseRelatorio(ds);
                    PopulaDadosRelatorio();
                    Popular_Documentos();
                    ModoEdicao(true);
                                        
                    PainelAtualizacao.Visible = true;
                    string lbPainelAtualizacao = string.Format("Criado em <b>{0}</b> por <b>{1}</b>", Retorno.DATASET(ds, 0, 0, "dtCriaRelatorio"), Retorno.DATASET(ds, 0, 0, "sDscUsuario_CriaRelatorio"));
                    btnGerarRelatorio.Visible = false;
                    btnGerarContasPagar.Visible = true;
                    btnExcluirRelatorio.Visible = true;

                    hddidTitulo.Value = Retorno.DATASET(ds, "idContasPagar");

                    if (hddidTitulo.Value != "0")
                    {
                        lbPainelAtualizacao = lbPainelAtualizacao + string.Format("<br />Gerado em <b>{0}</b> por <b>{1}</b>", Retorno.DATASET(ds, 0, 0, "dtGeraRelatorio"), Retorno.DATASET(ds, 0, 0, "sDscUsuario_GeraRelatorio"));
                        btnGerarContasPagar.Visible = false;
                        btnExcluirRelatorio.Visible = false;
                        lnkExcelRelatorio.Visible = true;
                    }

                    PainelAtualizacao.Personalizar(lbPainelAtualizacao);

                    Session["Relatorio_Seguro"] = bs_Relatorio_Seguro.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao redirecionar para detalhe: " + ex);
            }
        }

        private void GerarRelatorio()
        {
            try
            {

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "Salvar_Relatorio_Seguro" },
                    { "@idRelatorioSeguro", hddidRelatorioSeguro.Value },
                    { "@idUsuario", Identity.Variaveis.idUsuario() },
                    { "@idContasPagar", hddidTitulo.Value},
                    { "@nTipo", "1"}
                };
                DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

            }
            catch (Exception ex)
            {
                MensagemPaginaDetalhe.MostraMensagem_Erro("Erro ao gerar Relatório: " + ex);
            }
        }

        private void ExcluirRelatorio()
        {
            try
            {

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "Excluir_Relatorio_Seguro" },
                    { "@idRelatorioSeguro", hddidRelatorioSeguro.Value },
                    { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }                   
                };
                DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

                ModoEdicao(false);
                Pesquisar();

                MensagemPagina.MostraMensagem_Sucesso("Relatório Excluído com sucesso");
            }
            catch (Exception ex)
            {
                MensagemPaginaDetalhe.MostraMensagem_Erro("Erro ao Excluir Relatório: " + ex);
            }
        }

        private void SalvarRelatorioSeguroDetalhe()
        {
            foreach (var item in bs_Relatorio_Seguro_Detalhe)
            {                
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "Salvar_Relatorio_Seguro_Detalhe" },
                        { "@idRelatorioSeguro", hddidRelatorioSeguro.Value },
                        { "@idColaborador", item.idColaborador.ToString() },
                        { "@nValorNatural", item.nValorNatural.ToString().Replace(",",".") },
                        { "@nValorAcidental", item.nValorAcidental.ToString().Replace(",",".") },
                        { "@nValorInvalidez", item.nValorInvalidez.ToString().Replace(",",".") },
                        { "@nVG", item.nVG.ToString().Replace(",",".") },
                        { "@nAPC", item.nAPC.ToString().Replace(",",".") }
                    };
                    DataTable dt = BD.ExecutarDataTable(sProcedure, vParametros);                
            }
        }

        #endregion

        #region | Utils e Script

        private void ModoEdicao(bool bHabilitar)
        {
            div_RelatorioSeguroConsulta.Visible = !bHabilitar;
            div_RelatorioSeguroDetalhe.Visible = bHabilitar;
        }

        private bool ValidarCampos()
        {
            bool bRetorno = true;
            string sMensagemErro = "";


            if (!Validacoes.ValidarData(txtdtInicio))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Data Início";
            }

            if (!Validacoes.ValidarData(txtdtFinal))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Data Final";
            }

            if (ddlsSeguradora.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione a Seguradora";
            }

            if (ddlsEmpresa.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione a Empresa";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPagina_Filtro.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        private bool ValidarCamposModal()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlidCategoriaPagar.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Categoria!";
            }

            if (hddTipoCategoria.Value == "1" || hddTipoCategoria.Value == "2") //Compras e Serviços
            {
                if (ddlidFormaPagamento.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Forma de Pagamento!";
                }

                if (ddlidEmpresa.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
                }

                if (ddlidCentroDeCusto.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Centro de Custo!";
                }

            }
            else if (hddTipoCategoria.Value == "3") //Impostos
            {
                if (!Validacoes.ValidarData(txtdtApuracao))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira a Data de Apuração!";
                }

                if (ddlidFormaPagamento.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Forma de Pagamento!";
                }

                if (ddlidEmpresa.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
                }

                if (ddlidParceiro.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Credor!";
                }

            }
            else if (hddTipoCategoria.Value == "4") //Internos
            {
                if (!Validacoes.ValidarData(txtdtApuracao))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira a Data de Apuração!";
                }

                if (ddlidFormaPagamento.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Forma de Pagamento!";
                }

                if (ddlidEmpresa.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
                }

            }
            else if (hddTipoCategoria.Value == "5" || hddTipoCategoria.Value == "6") //Operaçoes Financeiras e Benificios
            {
                if (!Validacoes.ValidarData(txtdtApuracao))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira a Data de Apuração!";
                }

                if (ddlidFormaPagamento.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Forma de Pagamento!";
                }

                if (ddlidEmpresa.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
                }

                if (ddlidCentroDeCusto.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Centro de Custo!";
                }

            }

            if (hddTipoCategoria.Value == "" || hddTipoCategoria.Value == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Categoria";
            }

            if (txtdtVencimento.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma data de vencimento";
            }

            if (txtnValorBruto.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira o Valor Bruto";
            }

            if (txtnValorLiquido.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira o Valor Líquido";
            }

            DateTime dtVencimento = txtdtVencimento.Text == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(txtdtVencimento.Text);
            DateTime dtEmissao = txtdtEmissao.Text == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(txtdtEmissao.Text);
            if (dtVencimento < dtEmissao)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data de Validade não pode ser antes da Data de Emissão";
            }

            decimal valorBruto = txtnValorBruto.Text != "" ? decimal.Parse(txtnValorBruto.Text) : 0;
            decimal valorLiquido = txtnValorLiquido.Text != "" ? decimal.Parse(txtnValorLiquido.Text) : 0;
            if (valorBruto < valorLiquido)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Bruto deve ser maior ou igual ao Valor Líquido";
            }


            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPaginaModalDetalhe.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        private void RegistraScript()
        {
            lblTituloExcluir.Text = "Confirma a Exclusão desse Relatório ?";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<script type='text/javascript'>");
            sb.Append("$v192(function() {");

            sb.Append("$v192(\"#dialog-Excluir\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_Excluir\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=btnExcluirRelatorio]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Excluir').dialog('open');");
            sb.Append("});");
            sb.AppendLine("});");

            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("     $('[id*=txtnValorBruto]').mask('0.000.000.009,99999', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnValorLiquido]').mask('0.000.000.009,99999', { reverse: true });");
            sb.AppendLine("});");


            sb.AppendLine("</script>");


            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptExtrato", sb.ToString(), false);
        }

        #endregion

        #region | Popular Dados

        private void PopulaClasseRelatorio(DataSet ds)
        {
            bs_Relatorio_Seguro.Clear();
            bs_Relatorio_Seguro_Detalhe.Clear();

            cls_Relatorio_Seguro objtItemSeguro = new cls_Relatorio_Seguro();
            objtItemSeguro.idRelatorioSeguro = hddidRelatorioSeguro.Value == "" ? 0 : Convert.ToInt32(hddidRelatorioSeguro.Value);
            objtItemSeguro.dtInicio = Retorno.DATASET(ds, 0, "dtInicio");
            objtItemSeguro.dtFinal = Retorno.DATASET(ds, 0, "dtFinal");
            objtItemSeguro.nApoliceVG = Convert.ToInt32(Retorno.DATASET(ds, 0, "nApoliceVG"));
            objtItemSeguro.nApoliceAPC = Convert.ToInt32(Retorno.DATASET(ds, 0, "nApoliceAPC"));
            objtItemSeguro.sEndereco = Retorno.DATASET(ds, 0, "sEndereco");
            objtItemSeguro.sTelefone = Retorno.DATASET(ds, 0, "sTelefoneApoio");
            objtItemSeguro.idEmpresa = Convert.ToInt32(Retorno.DATASET(ds, 0, "idEmpresa"));
            objtItemSeguro.sEmpresa = Retorno.DATASET(ds, 0, "sEmpresa");
            objtItemSeguro.idSeguro = Convert.ToInt32(Retorno.DATASET(ds, 0, "idSeguro"));
            objtItemSeguro.sSeguradora = Retorno.DATASET(ds, 0, "sSeguradora");
            objtItemSeguro.idParceiro = Convert.ToInt32(Retorno.DATASET(ds, 0, "idParceiro"));

            objtItemSeguro.Qtd_nVG = Convert.ToInt32(Retorno.DATASET(ds, 1, 0, "qtd_nVG"));
            objtItemSeguro.Qtd_nAPC = Convert.ToInt32(Retorno.DATASET(ds, 1, 0, "qtd_nAPC"));
            objtItemSeguro.Qtd_nVG_nAPC = Convert.ToInt32(Retorno.DATASET(ds, 1, 0, "qtd_nVG_nAPC"));
            objtItemSeguro.Total_nVG = Convert.ToDecimal(Retorno.DATASET(ds, 1, 0, "Total_nVG"));
            objtItemSeguro.Total_nAPC = Convert.ToDecimal(Retorno.DATASET(ds, 1, 0, "Total_nAPC"));
            objtItemSeguro.Total_nVG_nAPC = Convert.ToDecimal(Retorno.DATASET(ds, 1, 0, "Total_nVG_nAPC"));

            bs_Relatorio_Seguro.Add(objtItemSeguro);

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                cls_Relatorio_Seguro_Detalhe objItem = new cls_Relatorio_Seguro_Detalhe();
                objItem.idColaborador = Convert.ToInt32(row["idColaborador"]);
                objItem.sDscColaborador = row["sDscColaborador"].ToString();
                objItem.sCPF = row["sCPF"].ToString();
                objItem.sEmpresa = row["sEmpresa"].ToString();
                objItem.sEstadoCivil = row["sEstadoCivil"].ToString();
                objItem.sSexo = row["sSexo"].ToString();
                objItem.sNomeSocial = row["sNomeSocial"].ToString();
                objItem.sFuncaoCarteira = row["sFuncaoCarteira"].ToString();
                objItem.nSalario = Convert.ToDecimal(row["nSalario"]);
                objItem.dtNascimento = row["dtNascimento"].ToString();
                objItem.sDscDepartamento = row["sDscDepartamento"].ToString();
                objItem.sGHE = row["sGHE"].ToString();
                objItem.sDscSetor = row["sDscSetor"].ToString();
                objItem.sDscCargo = row["sDscCargo"].ToString();
                objItem.dtInicioSeguro = row["dtInicioSeguro"].ToString();
                objItem.sTipoPlano = row["sTipoPlano"].ToString();
                objItem.nValorNatural = Convert.ToDecimal(row["nValorNatural"]);
                objItem.nValorAcidental = Convert.ToDecimal(row["nValorAcidental"]);
                objItem.nValorInvalidez = Convert.ToDecimal(row["nValorInvalidez"]);
                objItem.nVG = Convert.ToDecimal(row["nVG"]);
                objItem.nAPC = Convert.ToDecimal(row["nAPC"]);
                objItem.nVG_APC = Convert.ToDecimal(row["nVG_APC"]);

                bs_Relatorio_Seguro_Detalhe.Add(objItem);
            }

            objtItemSeguro.lsSeguroColaborador = bs_Relatorio_Seguro_Detalhe;
        }

        private void PopulaDadosRelatorio()
        {
            var relatorioSeguro = bs_Relatorio_Seguro.FirstOrDefault();

            txtIdRelatorioSeguro.Text = relatorioSeguro.idRelatorioSeguro.ToString();
            txtsPeriodo.Text = string.Format("{0} - {1}", relatorioSeguro.dtInicio, relatorioSeguro.dtFinal);
            txtsEmpresa.Text = relatorioSeguro.sEmpresa;
            txtsSeguradora.Text = relatorioSeguro.sSeguradora;
            txtnApoliceVG.Text = relatorioSeguro.nApoliceVG.ToString();
            txtnApoliceAPC.Text = relatorioSeguro.nApoliceAPC.ToString();
            txtsEndereco.Text = relatorioSeguro.sEndereco;
            txtsContato.Text = relatorioSeguro.sTelefone;
            txtnTotalVG.Text = relatorioSeguro.Total_nVG.ToString("N5");
            txtnTotalAPC.Text = relatorioSeguro.Total_nAPC.ToString("N5");
            txtnTotalVgApc.Text = relatorioSeguro.Total_nVG_nAPC.ToString("N5");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", Grid.DataBindComScript(gvRelatorioSeguro, bs_Relatorio_Seguro_Detalhe, 1, "asc"), true);
            
        }

        private void PopulaCombo()
        {
            Funcoes.Popula_Combo(ddlsEmpresa, "sp_Select 'Flow_Usuario_x_Empresa'", "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
            Funcoes.Popula_Combo(ddlsSeguradora, "sp_Select 'Flow_Seguradora'", "idSeguro", "sSeguradora", false, "Selecione a Seguradora", "0");

            Funcoes.Popula_Combo(ddlidCategoriaPagar, "sp_Select 'Flow_Adm_Contas_Pagar_Categoria'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Selecione a Categoria", "0");
            Funcoes.Popula_Combo(ddlidFormaPagamento, "sp_Select 'tbl_Flow_CondicaodePagamento_Tipo'", "idTipoCondicaoPagamento", "sDscTipoCondicaoPagamento", false, "Selecione o Pagamento", "0");
            Funcoes.Popula_Combo(ddlidCentroDeCusto, "sp_Select 'Flow_Adm_CentroDeCusto'", "idCentroDeCusto", "sDescricao", false, "Selecione um Centro de Custo", "0");
            Funcoes.Popula_Combo(ddlidContabil, "sp_Select 'Flow_CodigoContabil'", "idContabil", "sDscCodContabil", false, "Selecione o Código Contábil", "0");
            Funcoes.Popula_Combo(ddlidMeioPagamento, "sp_Select 'tbl_Flow_Adm_MeioPagamento'", "idMeioPagamento", "sDscMeioPagamento", false, "Selecione um Meio de Pagamento", "0");
            Funcoes.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa'", "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
            Funcoes.Popula_Combo(ddlidParceiro, "sp_Select 'Flow_Clientes'", "idCliente", "sRazaoSocial", false, "Selecione um credor", "0");

        }

        void Popular_Documentos()
        {
            frmArquivos.Attributes.Add("src", string.Format("~/App/Paginas/Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", hddidRelatorioSeguro.Value, "RelatorioSeguro"));
            frmArquivos.Visible = true;
            DIV_Arquivos.Visible = true;
            aba_Arquivo.Visible = true;
        }

        #endregion

        #region | Eventos

        protected void btnNovoRelatorio_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "Consultar_Relatorio_Seguro" },
                        { "@dtInicio_Seguro", txtdtInicio.Text },
                        { "@dtFinal_Seguro", txtdtFinal.Text },
                        { "@idEmpresa", ddlsEmpresa.SelectedValue },
                        { "@idSeguro", ddlsSeguradora.SelectedValue },
                        { "@nTipo", "1" }
                    };
                    DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);
                    if (BD.ValidarDataSet(ds, out string sErro))
                    {
                        if (ds.Tables[1].Rows.Count > 0)
                        {
                            ModoEdicao(true);

                            PopulaClasseRelatorio(ds);

                            PopulaDadosRelatorio();

                            txtIdRelatorioSeguro.Text = "Novo";
                            btnExcluirRelatorio.Visible = false;
                            aba_Arquivo.Visible = false;
                        }
                        else
                        {
                            MensagemPagina.MostraMensagem_Erro("Não foi encontrado nenhum registro para criar o relatório com esses parâmetros");
                        }

                    }
                    else
                    {
                        MensagemPaginaDetalhe.MostraMensagem_Erro(sErro);
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Filtro.MostraMensagem_Erro("Erro ao criar novo Relatório: " + ex);
            }
        }

        protected void lbDetalheRelatorio_Command(object sender, CommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();
            hddidRelatorioSeguro.Value = id;
            RelatorioSeguro_Detalhe();
        }

        protected void btnGerarRelatorio_Click(object sender, EventArgs e)
        {
            try
            {
                var relatorioSeguro = bs_Relatorio_Seguro.FirstOrDefault();

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "Salvar_Relatorio_Seguro" },
                    { "@dtInicio_Seguro", relatorioSeguro.dtInicio },
                    { "@dtFinal_Seguro", relatorioSeguro.dtFinal },
                    { "@idEmpresa", relatorioSeguro.idEmpresa.ToString() },
                    { "@idSeguro", relatorioSeguro.idSeguro.ToString() },
                    { "@idUsuario", Identity.Variaveis.idUsuario() }
                };
                DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(ds, out string sErro))
                {
                    hddidRelatorioSeguro.Value = Retorno.DATASET(ds, 0, "idRelatorioSeguro");
                    relatorioSeguro.idRelatorioSeguro = Convert.ToInt32(hddidRelatorioSeguro.Value);

                    SalvarRelatorioSeguroDetalhe();

                    txtIdRelatorioSeguro.Text = hddidRelatorioSeguro.Value;
                    btnGerarRelatorio.Visible = false;
                    btnGerarContasPagar.Visible = true;
                    btnExcluirRelatorio.Visible = true;
                    Popular_Documentos();

                    PainelAtualizacao.Visible = true;
                    string sCriado = string.Format("Criado em <b>{1}</b> por <b>{0}</b>", Retorno.DATASET(ds, 0, "sDscUsuario_CriaRelatorio"), Retorno.DATASET(ds, 0, "dtCriaRelatorio"));
                    PainelAtualizacao.Personalizar(sCriado);

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables2", Grid.DataBindComScript(gvRelatorioSeguro, bs_Relatorio_Seguro_Detalhe, 1, "asc"), true);

                    MensagemPaginaDetalhe.MostraMensagem_Sucesso("Relatório salvo com sucesso!");
                }

            }
            catch (Exception ex)
            {
                MensagemPaginaDetalhe.MostraMensagem_Erro("Erro ao salvar Relatório: " + ex.ToString());
            }

            ModoEdicao(true);

        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            ModoEdicao(false);
            Response.Redirect("RelatorioSeguro.aspx");
        }

        protected void btnGerarContasPagar_Click(object sender, EventArgs e)
        {
            div_ddlCategoriaPagar.Visible = true;
            div_camposDetalhe.Visible = false;
            btnSalvarModal.Visible = false;

            //MensagemPaginaModalDetalhe.MostraMensagem("", "info", false);
            MensagemPaginaModalDetalhe.MostraMensagem("Selecione uma Categoria", "info", false);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalNovo", "$('#modalContasPagarDetalhe').modal('show');", true);
        }

        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        protected void ddlidCategoriaPagar_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            int idCategoriaTipo_Consulta = 0;
            try
            {
                DataSet dsContabil;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "Autoselecao_Contabil");
                vParametros.Add("@idCategoriaPagar", ddlidCategoriaPagar.SelectedValue);

                dsContabil = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", vParametros);

                if (BD.ValidarDataSet(dsContabil))
                {
                    ddlidContabil.SelectedValue = Retorno.DATASET(dsContabil, 0, "idContabil");
                    idCategoriaTipo_Consulta = Convert.ToInt32(Retorno.DATASET(dsContabil, 0, "idCategoriaTipo"));
                }
            }
            catch
            {
                ddlidContabil.SelectedValue = "";
            }
            switch (idCategoriaTipo_Consulta)
            {

                case 1: //Compras
                case 2: //Serviços 
                    div_ddlEmpresa.Visible = true;
                    div_ddlidParceiro.Visible = true;
                    div_TipoCategoriaPagar.Visible = true;
                    div_txtdtApuracao.Visible = false;
                    div_ddlidCentroCusto.Visible = true;

                    lblsCodigo.Text = "Nota Fiscal";

                    break;
                case 3: //Impostos   
                    div_ddlidParceiro.Visible = true;
                    div_ddlEmpresa.Visible = true;

                    div_TipoCategoriaPagar.Visible = true;
                    div_txtdtApuracao.Visible = true;
                    div_ddlidCentroCusto.Visible = false;

                    lblsCodigo.Text = "Código Receita";

                    break;
                case 4: //Internos
                    div_ddlEmpresa.Visible = true;
                    div_ddlidParceiro.Visible = true;
                    div_TipoCategoriaPagar.Visible = true;
                    div_txtdtApuracao.Visible = true;

                    lblsCodigo.Text = "Nota Fiscal";

                    break;

                case 5: //Benefícios
                case 6: //Operações Financeiras   
                    div_ddlEmpresa.Visible = true;
                    div_ddlidParceiro.Visible = true;
                    div_TipoCategoriaPagar.Visible = true;
                    div_txtdtApuracao.Visible = true;

                    lblsCodigo.Text = "Nota Fiscal";

                    break;
                case 0:
                    div_ddlCategoriaPagar.Visible = true;                    
                    btnSalvarModal.Visible = false;
                    break;

            }

            var relatorio = bs_Relatorio_Seguro.FirstOrDefault();

            ddlidParceiro.SelectedValue = relatorio.idParceiro.ToString();
            ddlidParceiro.Attributes.Add("disabled", "disabled");
            ddlidEmpresa.SelectedValue = relatorio.idEmpresa.ToString();
            ddlidEmpresa.Attributes.Add("disabled", "disabled");
            ddlidMeioPagamento.SelectedValue = "1";
            ddlidMeioPagamento.Attributes.Add("disabled", "disabled");
            div_dtEmissao.Visible = true;
            txtdtEmissao.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtnValorBruto.Text = relatorio.Total_nVG_nAPC.ToString("N5");
            txtnValorLiquido.Text = relatorio.Total_nVG_nAPC.ToString("N5");
            txtnValorBruto.ReadOnly = true;
            txtnValorLiquido.ReadOnly = true;
            ddlidContabil.Attributes.Add("disabled", "disabled");
            btnSalvarModal.Visible = true;
            div_camposDetalhe.Visible = true;
            hddTipoCategoria.Value = idCategoriaTipo_Consulta.ToString();

            RegistraScript();
        }        

        protected void btnSalvarModal_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidarCamposModal())
                {                    
                    DataSet ds;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@dtVencimento", DateTime.Parse(txtdtVencimento.Text).ToString());
                    vParametros.Add("@sQuantidadeParcela", $"Única");
                    vParametros.Add("@sCodigo", txtsCodigo.Text);
                    vParametros.Add("@dtEmissao", DateTime.Parse(txtdtEmissao.Text).ToString());
                    vParametros.Add("@sDocumento", txtsDocumento.Text);
                    vParametros.Add("@idContabil", ddlidContabil.SelectedValue);
                    vParametros.Add("@idCentroDeCusto", ddlidCentroDeCusto.SelectedValue);
                    vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                    vParametros.Add("@nParcelas", "1");
                    vParametros.Add("@idParceiro", ddlidParceiro.SelectedValue);
                    vParametros.Add("@sObservacaoGeral", txtsObservacao.Text);
                    vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());

                    decimal nValor = Math.Round(Convert.ToDecimal(txtnValorBruto.Text), 2);

                    vParametros.Add("@nSaldo", nValor.ToString().Replace(".", "").Replace(",","."));
                    vParametros.Add("@nValorOriginal", nValor.ToString().Replace(".", "").Replace(",", "."));
                    vParametros.Add("@nValorBruto", nValor.ToString().Replace(".", "").Replace(",", "."));
                    vParametros.Add("@idFormaPagamento", ddlidFormaPagamento.SelectedValue);
                    vParametros.Add("@idMeioPagamento", ddlidMeioPagamento.SelectedValue);
                    vParametros.Add("@dtApuracao", txtdtApuracao.Text);
                    vParametros.Add("@idCategoriaPagar", ddlidCategoriaPagar.SelectedValue);
                    vParametros.Add("@idRelatorioSeguro", hddidRelatorioSeguro.Value);

                    ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", vParametros);
                    if (BD.ValidarDataSet(ds, out string sErro))
                    {
                        hddidTitulo.Value = Retorno.DATASET(ds, 0, "idContasPagar");

                        GerarRelatorio();
                        RelatorioSeguro_Detalhe();

                        string url = "/app/Paginas/Adm/Financeiro/ContasPagar_Detalhe.aspx?id=" + hddidTitulo.Value;
                        string mensagemSucesso = string.Format("Relatório gerado com sucesso! <br/>" + "<a href='{0}' target='_blank'>Clique aqui para ver o título do Contas a Pagar</a> ", url);

                        MensagemPaginaDetalhe.MostraMensagem_Sucesso(mensagemSucesso);

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "js_CloseModal", "$('#modalContasPagarDetalhe').modal('hide');", true);
                    }                
                      
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaModalDetalhe.MostraMensagem_Erro("Erro ao gerar contas a Pagar:" + ex);
            }
        }

        protected void btnFecharFooterDetalhe_Click(object sender, EventArgs e)
        {
            RelatorioSeguro_Detalhe();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalNovo", "$('#modalContasPagarDetalhe').modal('hide');", true);
        }
               

        #endregion

        public static void ExportarRelatorioSeguroParaXLS(cls_Relatorio_Seguro relatorio, string sNomeArquivoSemExten)
        {
            string sNomeArquivoComExtensao = sNomeArquivoSemExten + Funcoes.CarimboDataHora() + ".xls";
            var server = HttpContext.Current.Server;
            var response = HttpContext.Current.Response;


            // Carrega logo
            string logoBase64 = "";
            string caminhoLogo = server.MapPath("~/img/LogoTT.png");
            if (File.Exists(caminhoLogo))
            {
                byte[] imageBytes = File.ReadAllBytes(caminhoLogo);
                string base64String = Convert.ToBase64String(imageBytes);
                logoBase64 = "data:image/png;base64," + base64String;
            }

            response.BufferOutput = true;
            response.Clear();
            response.ClearHeaders();
            response.ClearContent();
            response.AddHeader("content-disposition", "attachment; filename=" + sNomeArquivoComExtensao);
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.ContentType = "application/vnd.ms-excel";
            response.ContentEncoding = System.Text.Encoding.UTF8;

            StringBuilder lSbExcel = new StringBuilder();

            // --- Estilos ---
            lSbExcel.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\r\n");
            lSbExcel.Append("<style type=\"text/css\">\r\n");
            lSbExcel.Append("body { font-family: Arial, Helvetica, sans-serif; color: #333; }\r\n");
            lSbExcel.Append(".report-table { border-collapse: collapse; width: 100%; font-size: 12px; }\r\n");
            lSbExcel.Append(".report-table th, .report-table td { font-family: Arial, Helvetica, sans-serif; border: 1px solid #999999; padding: 12px 15px; text-align: left; vertical-align: middle; }\r\n");
            lSbExcel.Append(".report-table th { background-color: #009a22; color: #ffffff; font-size: 13px; font-weight: bold; text-transform: uppercase; }\r\n");
            lSbExcel.Append(".report-table tr.alt-row td { background-color: #f2f2f2; }\r\n");
            lSbExcel.Append(".header-container { text-align: center; margin-bottom: 25px; }\r\n");
            lSbExcel.Append(".logo { max-height: 60px; margin-bottom: 15px; }\r\n");
            lSbExcel.Append(".report-title { font-family: Arial, Helvetica, sans-serif; color: #024e0a; font-size: 24px; font-weight: bold; margin: 0; }\r\n");
            lSbExcel.Append(".report-subtitle { font-family: Arial, Helvetica, sans-serif; font-size: 14px; text-align: center; color: #666; margin-top: 5px; }\r\n");
            lSbExcel.Append(".info-section { margin-bottom: 20px; }\r\n");
            lSbExcel.Append(".info-table { border-collapse: collapse; width: 100%; margin-bottom: 15px; }\r\n");
            lSbExcel.Append(".info-table td { padding: 6px 10px; border: 1px solid #ddd; font-size: 13px; }\r\n");
            lSbExcel.Append(".info-label { font-weight: bold; width: 25%; background-color: #f9f9f9; }\r\n");
            lSbExcel.Append("hr.separator { border: 0; height: 2px; background-color: #009a22; margin-top: 25px; }\r\n");
            lSbExcel.Append("</style>\r\n\r\n");

            // --- Cabeçalho ---
            lSbExcel.Append("<div class='header-container'>");
            if (!string.IsNullOrEmpty(logoBase64))
            {
                lSbExcel.AppendFormat("<img src='{0}' class='logo' />", logoBase64);
            }
            lSbExcel.Append("<div class='report-title'>Relatório Seguradora: " + relatorio.sSeguradora + "</div>");
            lSbExcel.AppendFormat("<div class='report-subtitle'>Gerado em: {0}</div>", DateTime.Now.ToString("dd/MM/yyyy 'às' HH:mm:ss"));
            lSbExcel.Append("</div>");
            lSbExcel.Append("<hr class='separator' />");

            // --- DADOS PRINCIPAIS ---
            lSbExcel.Append("<div class='info-section'>");
            lSbExcel.Append("<table class='report-table'>");
            lSbExcel.Append("<thead><tr><th colspan='2'>Informações do Seguro</th></tr></thead>");
            lSbExcel.Append("<tbody>");
            lSbExcel.Append("<tr><td>Empresa</td><td>" + HttpUtility.HtmlEncode(relatorio.sEmpresa) + "</td></tr>");
            lSbExcel.Append("<tr class='alt-row'><td>Seguradora</td><td>" + HttpUtility.HtmlEncode(relatorio.sSeguradora) + "</td></tr>");
            lSbExcel.Append("<tr><td>Nº Apólice V.G.</td><td>" + HttpUtility.HtmlEncode(relatorio.nApoliceVG.ToString()) + "</td></tr>");
            lSbExcel.Append("<tr class='alt-row'><td>Nº Apólice APC</td><td>" + HttpUtility.HtmlEncode(relatorio.nApoliceAPC.ToString()) + "</td></tr>");
            lSbExcel.Append("<tr><td>Período</td><td>" + HttpUtility.HtmlEncode(relatorio.dtInicio) + " a " + HttpUtility.HtmlEncode(relatorio.dtFinal) + "</td></tr>");
            lSbExcel.Append("<tr class='alt-row'><td>Endereço</td><td>" + HttpUtility.HtmlEncode(relatorio.sEndereco) + "</td></tr>");
            lSbExcel.Append("<tr><td>Telefone</td><td>" + HttpUtility.HtmlEncode(relatorio.sTelefone) + "</td></tr>");
            lSbExcel.Append("</tbody>");
            lSbExcel.Append("</table>");
            lSbExcel.Append("</div>");

            lSbExcel.Append("<table style='width:100%; height:20px;'><tr><td></td></tr></table>");

            // --- TABELA DETALHADA ---
            if (relatorio.lsSeguroColaborador != null && relatorio.lsSeguroColaborador.Any())
            {
                lSbExcel.Append("<table class=\"report-table\">\r\n");
                lSbExcel.Append("<thead>\r\n");
                lSbExcel.Append("<tr>\r\n");

                lSbExcel.Append("\t<th>ID</th>\r\n");
                lSbExcel.Append("\t<th>Colaborador</th>\r\n");
                lSbExcel.Append("\t<th>Nome Social</th>\r\n");
                lSbExcel.Append("\t<th>CPF</th>\r\n");
                lSbExcel.Append("\t<th>Data Nascimento</th>\r\n");
                lSbExcel.Append("\t<th>Função Carteira</th>\r\n");
                lSbExcel.Append("\t<th>Salário</th>\r\n");
                lSbExcel.Append("\t<th>Sexo</th>\r\n");
                lSbExcel.Append("\t<th>Estado Civil</th>\r\n");
                lSbExcel.Append("\t<th>Empresa</th>\r\n");
                lSbExcel.Append("\t<th>Início Contrato</th>\r\n");
                lSbExcel.Append("\t<th>Tipo</th>\r\n");
                lSbExcel.Append("\t<th>Morte Natural</th>\r\n");
                lSbExcel.Append("\t<th>Morte Acidental</th>\r\n");
                lSbExcel.Append("\t<th>Invalidez por Acidente</th>\r\n");
                lSbExcel.Append("\t<th>V.G.</th>\r\n");
                lSbExcel.Append("\t<th>APC</th>\r\n");
                lSbExcel.Append("\t<th>V.G. + APC</th>\r\n");

                lSbExcel.Append("</tr>\r\n");
                lSbExcel.Append("</thead>\r\n");
                lSbExcel.Append("<tbody>\r\n");

                int rowIndex = 0;
                foreach (var item in relatorio.lsSeguroColaborador)
                {
                    string rowClass = (rowIndex % 2 != 0) ? "class='alt-row'" : "";
                    lSbExcel.AppendFormat("<tr {0}>\r\n", rowClass);

                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.idColaborador.ToString() ?? ""));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.sDscColaborador ?? ""));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.sNomeSocial ?? ""));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.sCPF ?? ""));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.dtNascimento ?? ""));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.sFuncaoCarteira ?? ""));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.nSalario.ToString("N2") ?? "0,00"));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.sSexo ?? ""));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.sEstadoCivil ?? ""));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.sEmpresa ?? ""));                  
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.dtInicioSeguro ?? ""));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.sTipoPlano ?? ""));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.nValorNatural.ToString("N2") ?? "0,00"));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.nValorAcidental.ToString("N2") ?? "0,00"));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.nValorInvalidez.ToString("N2") ?? "0,00"));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.nVG.ToString("N5") ?? "0,00"));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.nAPC.ToString("N5") ?? "0,00"));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.nVG_APC.ToString("N5") ?? "0,00"));

                    lSbExcel.Append("</tr>\r\n");
                    rowIndex++;
                }

                lSbExcel.Append("</tbody>\r\n");
                lSbExcel.Append("</table>\r\n");
            }
            else
            {
                lSbExcel.Append("<p><em>Nenhum dado detalhado encontrado.</em></p>");
            }

            lSbExcel.Append("<table style='width:100%; height:20px;'><tr><td></td></tr></table>");

            // --- Dados Totais 
            lSbExcel.Append("<div class='info-section'>");
            lSbExcel.Append("<table class='report-table'>");
            lSbExcel.Append("<tr><td>Vidas VG:</td><td>" + HttpUtility.HtmlEncode(relatorio.Qtd_nVG.ToString()) + "</td></tr>");
            lSbExcel.Append("<tr class='alt-row'><td>Vidas APC:</td><td>" + HttpUtility.HtmlEncode(relatorio.Qtd_nAPC.ToString()) + "</td></tr>");
            lSbExcel.Append("<tr><td>Total Geral:</td><td>" + HttpUtility.HtmlEncode(relatorio.Qtd_nVG_nAPC.ToString()) + "</td></tr>");
            lSbExcel.Append("<tr class='alt-row'><td>Total VG:</td><td>" + HttpUtility.HtmlEncode(relatorio.Total_nVG.ToString("N5")) + "</td></tr>");
            lSbExcel.Append("<tr><td>Total APC:</td><td>" + HttpUtility.HtmlEncode(relatorio.Total_nAPC.ToString("N5")) + "</td></tr>");
            lSbExcel.Append("<tr class='alt-row'><td>Total VG + APC:</td><td>" + HttpUtility.HtmlEncode(relatorio.Total_nVG_nAPC.ToString("N5")) + "</td></tr>");
            lSbExcel.Append("</table>");
            lSbExcel.Append("</div>");

            // --- Finalização (mantida) ---
            response.Write(lSbExcel.ToString());
            response.Flush();
            response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();            
        }
        
    }

    [Serializable]
    public class cls_Relatorio_Seguro
    {
        public int idRelatorioSeguro { get; set; }
        public string dtInicio { get; set; }
        public string dtFinal { get; set; }
        public int idEmpresa { get; set; }
        public string sEmpresa { get; set; }
        public int nApoliceVG { get; set; }
        public int nApoliceAPC { get; set; }
        public int idParceiro { get; set; }
        public int idSeguro { get; set; }
        public string sSeguradora { get; set; }
        public string sEndereco { get; set; }
        public string sTelefone { get; set; }
        public List<cls_Relatorio_Seguro_Detalhe> lsSeguroColaborador { get; set; }
        public int Qtd_nVG { get; set; }
        public int Qtd_nAPC { get; set; }
        public int Qtd_nVG_nAPC { get; set; }
        public decimal Total_nVG { get; set; }
        public decimal Total_nAPC { get; set; }
        public decimal Total_nVG_nAPC { get; set; }

    }

    [Serializable]
    public class cls_Relatorio_Seguro_Detalhe
    {
        public int idColaborador { get; set; }
        public string sDscColaborador { get; set; }
        public string sCPF { get; set; }
        public string sEmpresa { get; set; }
        public string sEstadoCivil { get; set; }
        public string sSexo { get; set; }
        public string sNomeSocial { get; set; }
        public string sFuncaoCarteira { get; set; }
        public decimal nSalario { get; set; }
        public string dtNascimento { get; set; }
        public string sDscDepartamento { get; set; }
        public string sGHE { get; set; }
        public string sDscSetor { get; set; }
        public string sDscCargo { get; set; }
        public string dtInicioSeguro { get; set; }
        public string sTipoPlano { get; set; }
        public decimal nValorNatural { get; set; }
        public decimal nValorAcidental { get; set; }
        public decimal nValorInvalidez { get; set; }
        public decimal nVG { get; set; }
        public decimal nAPC { get; set; }
        public decimal nVG_APC { get; set; }
    }
}