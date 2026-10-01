using Microsoft.Reporting.Map.WebForms.BingMaps;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.WebSockets;
using TT_Flow.App.Controles;
using TT_Flow.FrameWork;
using static iTextSharp.text.pdf.AcroFields;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using GRID = TT.FrameWork.Grid;
using IDENTITY = TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Paginas.Adm.Manutencao
{
    public partial class CentroCusto_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Centro de Custo";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_CentroDeCusto";
        string sProcTipoCC = "sp_Manipula_tbl_Flow_Adm_TipoCentroCusto";
        decimal nTeto = 0;
        private int DeletingRowIndex
        {
            get { return (int)(ViewState["DeletingRowIndex"] ?? -1); }
            set { ViewState["DeletingRowIndex"] = value; }
        }
        public List<cls_CentroDeCusto_Lancamentos> bs_CentroDeCusto_Lancamento
        {
            get
            {
                if (ViewState["bs_CentroDeCusto_Lancamento"] == null)
                {
                    ViewState["bs_CentroDeCusto_Lancamento"] = new List<FrameWork.cls_CentroDeCusto_Lancamentos>();
                }
                return (List<FrameWork.cls_CentroDeCusto_Lancamentos>)ViewState["bs_CentroDeCusto_Lancamento"];
            }
            set
            {
                ViewState["bs_CentroDeCusto_Lancamento"] = value;
            }
        }

        public List<cls_CentroDeCusto_Categoria> bs_Categoria_CC
        {
            get
            {
                if (ViewState["bs_Categoria_CC"] == null)
                {
                    ViewState["bs_Categoria_CC"] = new List<cls_CentroDeCusto_Categoria>();
                }
                return (List<cls_CentroDeCusto_Categoria>)ViewState["bs_Categoria_CC"];
            }
            set
            {
                ViewState["bs_Categoria_CC"] = value;
            }
        }
        private DataSet dsAtual
        {
            get
            {
                if (ViewState["dsAtual"] == null)
                {
                    ViewState["dsAtual"] = new DataSet();
                }
                return (DataSet)ViewState["dsAtual"];
            }
            set
            {
                ViewState["dsAtual"] = value;
            }
        }

        private Dictionary<string, DataRow> AlteracoesPendentes
        {
            get
            {
                if (Session["AlteracoesPendentesGrid"] == null)
                {
                    Session["AlteracoesPendentesGrid"] = new Dictionary<string, DataRow>();
                }
                return (Dictionary<string, DataRow>)Session["AlteracoesPendentesGrid"];
            }
            set
            {
                Session["AlteracoesPendentesGrid"] = value;
            }
        }

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();


            if (!IsPostBack)
            {
                PopularCombos();

                string centroCustoId = Request["id"] ?? "0";
                hddidCentroDeCusto.Value = centroCustoId;

                //if (Request["id"] != null)
                //{
                //    FUNCOES.ValidaPermissao(Permissao.Administracao.CentrodeCustos.Consultar, true);
                //    Pesquisar(Request["id"].ToString(), false);
                //}
                //else
                //{
                //    FUNCOES.ValidaPermissao(Permissao.Administracao.CentrodeCustos.Incluir, true);
                //    Pesquisar("0", true);

                //}

                if (centroCustoId != "0")
                {
                    FUNCOES.ValidaPermissao(Permissao.Administracao.CentrodeCustos.Consultar, true);
                    Pesquisar(centroCustoId, false); // Carrega dados gerais do Centro de Custo
                    // Popula os DropDowns de filtro e do modal INICIALMENTE
                    PopulaFiltroCategoriaInicial();
                    PopulaCategoriaModalInicial();
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Administracao.CentrodeCustos.Incluir, true);
                    Pesquisar("0", true);
                    // Desabilitar/ocultar a aba de lançamentos se for novo?
                    aba_Lancamento.Visible = false;
                }
                CarregarGridCategorias();
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                var requestArgs = this.Request["__EVENTARGUMENT"];

                if (requestTarget == "funcao_SAIR")
                {
                    FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                }
                else if (requestTarget == "funcao_SALVAR")
                {
                    Salvar_CentroCusto();
                }
                else if (requestTarget == "funcao_Editar")
                {
                    Pesquisar(hddidCentroDeCusto.Value, true);
                }

                //if(hddidCentroDeCusto.Value != "0" || !string.IsNullOrEmpty(hddidCentroDeCusto.Value))
                //  AtualizarBinds();
            }

            RegistraScript("");
            CarregarGraficoInteligente(bs_CentroDeCusto_Categorias);
            CarregarGraficoDonuts(bs_CentroDeCusto_Categorias);
        }

        private void PopulaFiltroCategoriaInicial()
        {
            // Popula ddlFiltroCategoria para o estado inicial da página
            FUNCOES.Popula_Combo(ddlFiltroCategoria, $"sp_Manipula_tbl_Flow_Adm_CentroCusto_Categorias 'FLOW-CATEGORIAS', @idCentroCusto={hddidCentroDeCusto.Value}", "idRegistro", "sDscCategoria", false, "Todas Categorias", "0");
            ddlFiltroCategoria.Items.Add(new System.Web.UI.WebControls.ListItem("Sem Categoria", "-1"));
        }
        private void PopulaCategoriaModalInicial()
        {
            // Popula ddlCategoriaModal para o estado inicial da página
            FUNCOES.Popula_Combo(ddlCategoriaModal, $"sp_Manipula_tbl_Flow_Adm_CentroCusto_Categorias 'FLOW-CATEGORIAS', @idCentroCusto={hddidCentroDeCusto.Value}", "idRegistro", "sDscCategoria", false, "Sem Categoria", "0");
        }

        void Popular_Aba_Historico(DataSet ds)
        {
            aba_Historico.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTablesHistorico", TT.FrameWork.Grid.DataBindComScriptData(gv_Historico, ds.Tables[1], 0, "desc", "false", "''"), true);
        }

        protected void btnPedido_Click(object sender, EventArgs e)
        {
            string url = string.Format("/App/Paginas/Pedidos_Detalhe.aspx?id={0}&sTp=2", hddidPedido.Value);
            string script = $"window.open('{url}', '_blank');";
            ClientScript.RegisterStartupScript(this.GetType(), "vinculoPedido", script, true);
            BindGridCategorias();
        }

        private void PopularTabelaResumo(DataTable dt)
        {
            gvResumoFinanceiros.DataSource = dt;
            gvResumoFinanceiros.DataBind();
        }

        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar(string idCentroDeCusto, bool bEdicao)
        {
            PopularCombos();

            cmdEditar.Visible = false;
            aba_Historico.Visible = false;
            aba_Lancamento.Visible = false;

            string sErro = "";

            try
            {
                LimpaCampos();

                if (idCentroDeCusto != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idCentroDeCusto", idCentroDeCusto);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidCentroDeCusto.Value = RETORNO.DATASET(dsPesquisa, 0, "idCentroDeCusto");
                        txtidCentroDeCusto.Text = RETORNO.DATASET(dsPesquisa, 0, "idCentroDeCusto");
                        ddlsTipo.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idTipo");
                        txtsDescricao.Text = RETORNO.DATASET(dsPesquisa, 0, "sDescricao");
                        txtsObservacao.Text = RETORNO.DATASET(dsPesquisa, 0, "sObservacao");
                        txtnTetoGasto.Text = RETORNO.DATASET(dsPesquisa, 0, "nTetoGasto");

                        txtnSaldoGasto.Text = RETORNO.DATASET(dsPesquisa, 0, "nSaldoGasto");
                        ddlsSituacao.Text = RETORNO.DATASET(dsPesquisa, 0, "sSituacao");
                        txtsCodCC.Text = RETORNO.DATASET(dsPesquisa, 0, "sCodCC");

                        string idPedido = RETORNO.DATASET(dsPesquisa, 0, "idPedido");

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));

                        lblTituloPagina.Text = string.Format("Centro de Custo - {0}", RETORNO.DATASET(dsPesquisa, 0, "sDescricao"));
                        BreadCrumb.TitulodaPagina = string.Format("Centro de Custo - {0}", RETORNO.DATASET(dsPesquisa, 0, "sDescricao"));

                        lblTituloSalvar.Text = "Confirma a Alteração da " + lblTituloPagina.Text + "?";
                        lblTituloEdiar.Text = "Deseja editar o Lançamento " + lblTituloPagina.Text + "?";

                        Popular_Aba_Historico(dsPesquisa);
                        Popular_Aba_Historico_Lancamentos(idCentroDeCusto);
                        //Popular_Aba_Lancamento("0");
                        PopularTabelaResumo(dsPesquisa.Tables[2]);

                        aba_Lancamento.Visible = true;

                        if (idPedido != "0")
                        {
                            btnPedido.Visible = true;
                            hddidPedido.Value = idPedido;
                        }

                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Administracao.CentrodeCustos.Alterar);
                        //MensagemPagina.MostraMensagem("Lembrete: O Saldo não é atualizado automaticamente, caso seja necessário sua atualização clique no botão (<i class=\"fa fa-refresh\"></i>)", "AVISO", false);

                    }
                    else
                    {
                        throw new Exception(sErro);
                    }

                }
                else
                {
                    BreadCrumb.TitulodaPagina = string.Format("Novo {0}", sTituloPagina);
                    lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
                    txtidCentroDeCusto.Text = "Novo";
                    lblTituloSalvar.Text = "Confirma a Inclusão do Centro de Custo?";
                    cmdSalvar.Text = "Incluir";

                }
                RegistraScript("");

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }       

        void Salvar_CentroCusto()
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idCentroDeCusto", hddidCentroDeCusto.Value);

                    //vParametros.Add("@idParceiro", ddlidParceiro.SelectedValue);

                    vParametros.Add("@idTipoCentroDeCusto", ddlsTipo.SelectedValue);
                    vParametros.Add("@sDescricao", txtsDescricao.Text);
                    vParametros.Add("@sCodCC", txtsCodCC.Text);
                    vParametros.Add("@nTetoGasto", BD.Conversoes.Numerico(txtnTetoGasto));
                    vParametros.Add("@nSaldoGasto", txtnSaldoGasto.Text == "" ? "0" : Convert.ToDecimal(txtnSaldoGasto.Text).ToString().Replace(".", "").Replace(",", "."));

                    vParametros.Add("@sObservacao", txtsObservacao.Text);
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    vParametros.Add("@sSituacao", ddlsSituacao.SelectedValue);
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        hddidCentroDeCusto.Value = RETORNO.DATASET(dsSalvar, "idCentroDeCusto");

                        if (SalvarAlteracoesDaGrid(AlteracoesPendentes))
                        {
                            // Se o salvamento foi bem-sucedido, limpamos o rascunho.
                            AlteracoesPendentes.Clear();
                        }


                        SalvarCategorias(hddidCentroDeCusto.Value);



                        //Popular_Aba_Lancamento(hddidCategoriaFiltro.Value);

                        if (Request["id"] != "0" || string.IsNullOrEmpty(Request["id"]))
                            CarregarGridCategorias();

                        MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                        aba_Lancamento.Visible = true;
                        Popular_Aba_Historico_Lancamentos(hddidCentroDeCusto.Value);
                        Pesquisar(hddidCentroDeCusto.Value, false);
                        PopulaFiltroCategoriaInicial();
                        PopulaCategoriaModalInicial();
                        CarregarGridCategorias();

                    }
                    else
                    {
                        aba_Lancamento.Visible = false;
                        throw new Exception("BD: " + sErro.ToString());
                    }


                }
                catch (Exception ex)
                {
                    aba_Lancamento.Visible = false;
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }

            }
            else
            {
                aba_Lancamento.Visible = false;
                Pesquisar(hddidCentroDeCusto.Value, false);
            }
            RegistraScript("");
        }

        private bool SalvarAlteracoesDaGrid(Dictionary<string, DataRow> alteracoesParaSalvar)
        {
            if (alteracoesParaSalvar == null || alteracoesParaSalvar.Count == 0)
            {
                return true; 
            }

            try
            {
                Dictionary<String, String> vParamsOriginais = new Dictionary<string, string>();
                vParamsOriginais.Add("@sFuncao", "CONSULTA_LANCAMENTO");
                vParamsOriginais.Add("@idCentroDeCusto", hddidCentroDeCusto.Value);
                DataSet dsOriginalCompleto = BD.ExecutarDataSet(sProcedure, vParamsOriginais);

                if (dsOriginalCompleto == null || dsOriginalCompleto.Tables.Count == 0)
                {
                    MensagemPagina_AbaLancamento.MostraMensagem_Erro("Não foi possível obter os dados originais para comparação.");
                    return false;
                }

                // 3. Itera sobre as alterações que recebemos para salvar.
                foreach (DataRow rowAtualizada in alteracoesParaSalvar.Values)
                {
                    int idLancamento = Convert.ToInt32(rowAtualizada["id"]);
                    string tipoLancamento = rowAtualizada["sTipo"].ToString();
                    string idNovaCategoria = rowAtualizada["idRegistroCategoria"].ToString();

                    DataRow[] rowsOriginais = dsOriginalCompleto.Tables[0].Select($"id = {idLancamento} AND sTipo = '{tipoLancamento}'");

                    if (rowsOriginais.Length > 0)
                    {
                        DataRow linhaOriginal = rowsOriginais[0];
                        string idCategoriaAntiga = linhaOriginal["idRegistroCategoria"].ToString();

                        if (idNovaCategoria != idCategoriaAntiga)
                        {
                            // Sua lógica de negócio original para atualizar saldos e o BD
                            TextBox txtnValor = new TextBox { Text = rowAtualizada["nValor"].ToString() };
                            HiddenField CategoriaCcAntiga = new HiddenField { Value = idCategoriaAntiga };
                            cc.HddidRegistroAntigo = CategoriaCcAntiga;
                            cc.DefinirCategoriaNova(hddidCentroDeCusto.Value, idNovaCategoria);
                            cc.SAcao = (tipoLancamento == "R") ? "SOMAR" : "SUBTRAIR";

                            Dictionary<string, string> vParametrosUpdate = new Dictionary<string, string>();
                            vParametrosUpdate.Add("@sFuncao", "ATUALIZAR-LANCAMENTOS");
                            vParametrosUpdate.Add("@idLancamento", idLancamento.ToString());
                            vParametrosUpdate.Add("@sTipo", tipoLancamento);
                            vParametrosUpdate.Add("@idNovaCategoria", idNovaCategoria);
                            vParametrosUpdate.Add("@idCentroDeCusto", hddidCentroDeCusto.Value);
                            vParametrosUpdate.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());

                            BD.ExecutarDataSet(sProcedure, vParametrosUpdate);
                            cc.AtualizarSaldoCategoria(txtnValor);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MensagemPagina_AbaLancamento.MostraMensagem_Erro($"Erro ao salvar lançamentos: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region | Aba Documentos 

        //void Popular_Aba_Documentos(string idColaborador)
        //{
        //    frmDocumentos.Attributes.Add("src", string.Format("../RRHH/Documentos_RRHH.aspx?idObjeto={0}&sTipoObjeto={1}", idContabil, "Adm"));
        //    aba_Documentos.Visible = true;

        //}

        #endregion

        #region | Limpar Campos
        void LimpaCampos()
        {
            txtsDescricao.Text = "";
            txtsObservacao.Text = "";
            txtnTetoGasto.Text = "";
            txtnSaldoGasto.Text = "";
            ddlsTipo.Text = "0";
            txtsCodCC.Text = "";
            //hddidCategoriaFiltro.Value = "";

            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;

            LimparCamposInclusaoCategoria();
            LimpaClasses();
        }

        #endregion

        #region | Validação 
        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlsTipo.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo!";
            }

            if (txtsCodCC.Text.Length < 3)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Código deve ter mais que 3 caracteres!";
            }

            if (txtsDescricao.Text.Length < 5)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Descrição deve ter mais que 5 caracteres!";
            }


            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }

            BindGridCategorias();

            return bRetorno;
        }
        #endregion

        #region | Script 

        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine("$v192(function() {");

            sb.AppendLine("$v192(\"#dialog-Salvar\").dialog({");
            sb.AppendLine("resizable: false,");
            sb.AppendLine("height: \"auto\",");
            sb.AppendLine("width: 400,");
            sb.AppendLine("modal: true,");
            sb.AppendLine("autoOpen: false,");
            sb.AppendLine("buttons:");
            sb.AppendLine("{");
            sb.AppendLine("\"Sim\": function() {");
            sb.AppendLine("__doPostBack(\"funcao_SALVAR\", \"\");");
            sb.AppendLine("$v192(this).dialog(\"close\");");
            sb.AppendLine("},");
            sb.AppendLine("\"Não\": function() {");
            sb.AppendLine("$v192(this).dialog(\"close\");");
            sb.AppendLine("},");
            sb.AppendLine("}");
            sb.AppendLine("});");
            sb.AppendLine("$v192('[id*=cmdSalvar]').click(function(e) {");
            sb.AppendLine("e.preventDefault();");
            sb.AppendLine("$v192('#dialog-Salvar').dialog('open');");
            sb.AppendLine("});");

            sb.AppendLine("$v192(\"#dialog-Editar\").dialog({");
            sb.AppendLine("resizable: false,");
            sb.AppendLine("height: \"auto\",");
            sb.AppendLine("width: 400,");
            sb.AppendLine("modal: true,");
            sb.AppendLine("autoOpen: false,");
            sb.AppendLine("buttons:");
            sb.AppendLine("{");
            sb.AppendLine("\"Sim\": function() {");
            sb.AppendLine("__doPostBack(\"funcao_Editar\", \"\");");
            sb.AppendLine("$v192(this).dialog(\"close\");");
            sb.AppendLine("},");
            sb.AppendLine("\"Não\": function() {");
            sb.AppendLine("$v192(this).dialog(\"close\");");
            sb.AppendLine("},");
            sb.AppendLine("}");
            sb.AppendLine("});");
            sb.AppendLine("$v192('[id*=cmdEditar]').click(function(e) {");
            sb.AppendLine("e.preventDefault();");
            sb.AppendLine("$v192('#dialog-Editar').dialog('open');");
            sb.AppendLine("});");

            sb.AppendLine("$('[id*=txtnTetoGasto]').mask('0.000.000.009,99', { reverse: true });");

            // Use o seletor `end with` ($=) para os campos no UpdatePanel
            sb.AppendLine("$('[id$=txtnTetoCategoria]').mask('0.000.000.009,90', { reverse: true });");
            sb.AppendLine("$('[id$=txtnSaldoCategoria]').mask('0.000.000.009,90', { reverse: true });");

            // Para os campos no GridView (em modo de edição)
            sb.AppendLine("$('[id*=txtGridTeto]').mask('0.000.000.000,99', { reverse: true });");
            sb.AppendLine("$('[id*=txtGridSaldo]').mask('0.000.000.000,99', { reverse: true });");

            //sb.AppendLine("function atualizarDados()");
            //sb.AppendLine(" {");
            //sb.AppendLine($"    var idCentroDeCusto = $('#{hddidCentroDeCusto.ClientID}').val();");
            //sb.AppendLine("    $.ajax({ ");
            //sb.AppendLine("    url: \"/App/Paginas/Adm/Manutencao/CentroCusto_Detalhe.aspx/AtualizarDados\", ");
            //sb.AppendLine("        data: JSON.stringify({ id: idCentroDeCusto }),");
            //sb.AppendLine("        dataType: \"json\","); 
            //sb.AppendLine("        type: \"POST\",");
            //sb.AppendLine("        contentType: \"application/json; charset=utf-8\",");
            //sb.AppendLine("        success: function(response) {");
            //sb.AppendLine("            var dados = response.d;");
            //sb.AppendLine($"            var tabela = $(\"#{dtgv_centroCustoLancamento.ClientID} tbody\");");
            //sb.AppendLine("            tabela.empty(); ");
            //sb.AppendLine("            $.each(dados.lancamentos, function(index, item) {");
            //sb.AppendLine("                var linha = \"<tr> \" +");
            //sb.AppendLine("                    \"<td style='text-align:center;'>\" + item.id + \"</td> \" +");
            //sb.AppendLine("                    \"<td style='text-align:center;'>\" + item.dtLancamento + \"</td> \" +");
            //sb.AppendLine("                    \"<td style='text-align:center;'>\" + item.sTipo + \"</td> \" +");
            //sb.AppendLine("                    \"<td><a href='#'>\" + item.sReferencia + \"</a></td> \" +");
            //sb.AppendLine("                \"<td><a href='#'>\" + item.sDscGeral + \"</a></td> \" +");
            //sb.AppendLine("                    \"<td>\" + item.sDscCategoria + \"</td> \" +");
            //sb.AppendLine("                    \"<td>\" + item.nValor.toFixed(2).replace('.', ',') + \"</td> \" +");
            //sb.AppendLine("                    \"<td>\" + item.nSaldo.toFixed(2).replace('.', ',') + \"</td> \" +");
            //sb.AppendLine("                    \"</tr> \";");
            //sb.AppendLine("                tabela.append(linha);");
            //sb.AppendLine("            });");
            //sb.AppendLine($"            $(\"#{txtnSaldoGasto.ClientID}\").val(dados.saldo);");
            //sb.AppendLine($"            $(\"#{txtdtUltimaAtualizacao.ClientID}\").val(dados.dtAtualizacao);");
            //sb.AppendLine("        },");
            //sb.AppendLine("        error: function(error) {");
            //sb.AppendLine("            console.log(\"Erro ao atualizar dados: \", error);");
            //sb.AppendLine("        }");
            //sb.AppendLine("    });");
            //sb.AppendLine("}");
            //sb.AppendLine("");
            //sb.AppendLine("setInterval(atualizarDados, 60000);");

            sb.AppendLine("});");


            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }


        #endregion

        #region | Combos/DDL

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlsTipo, "sp_Manipula_tbl_Flow_Adm_TipoCentroCusto 'Flow_TiposCC' ", "idTipoCentroCusto", "sDscTipo", false, "Selecione o Tipo", "0");

            // FUNCOES.Popula_Combo(ddlidCentroCustoPai, "sp_Select 'Flow_CodigoContabil'", "idContabil", "sDscCodContabil", false, "Selecione o Código Contábil", "0");
        }
        #endregion

        #region | LANÇAMENTOS - WEB METHODS

        // Classe auxiliar para retornar dados + paginação
        public class LancamentosResult
        {
            public List<Dictionary<string, object>> Lancamentos { get; set; }
            public int TotalRegistros { get; set; }
            public int PaginaAtual { get; set; }
            public int TotalPaginas { get; set; }
            public string SaldoTotalFormatado { get; set; }
            public decimal SaldoTotalDecimal { get; set; } // Pode ser útil
        }

        [WebMethod]
        public static LancamentosResult GetLancamentos(int idCentroDeCusto, int idRegistroCategoria, string sTipo, int pageNumber, int pageSize)
        {
            LancamentosResult result = new LancamentosResult();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            int totalRegistros = 0;
            string saldoTotalFormatado = "R$ 0,00";
            decimal saldoTotalDecimal = 0;

            try
            {
                // Garante pageSize válido (tratar -1 como "todos")
                string effectivePageSize = (pageSize == -1) ? "999999" : pageSize.ToString(); // Ajuste conforme necessário

                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTA_LANCAMENTO");
                vParametros.Add("@idCentroDeCusto", idCentroDeCusto.ToString());
                vParametros.Add("@idRegistroCategoria", idRegistroCategoria.ToString());
                vParametros.Add("@sTipo", sTipo ?? ""); // Garante que não é nulo
                vParametros.Add("@PageNumber", pageNumber.ToString());
                vParametros.Add("@PageSize", effectivePageSize);

                // IMPORTANTE: A Stored Procedure PRECISA retornar 3 tabelas agora:
                // 1: Dados da página atual
                // 2: Contagem total de registros (com os filtros aplicados)
                // 3: Saldo total atualizado e teto
                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_CentroDeCusto", vParametros);

                if (BD.ValidarDataSet(ds))
                {
                    // Tabela 0: Lançamentos da página
                    DataTable dtLancamentos = ds.Tables[0];
                    foreach (DataRow dr in dtLancamentos.Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in dtLancamentos.Columns)
                        {
                            // Formata datas e decimais conforme necessário para exibição
                            if (dr[col] is DateTime dtValue)
                                dict.Add(col.ColumnName, dtValue.ToString("dd/MM/yyyy")); // Ou o formato que o JS espera
                            else if (dr[col] is decimal decValue && (col.ColumnName == "nValor" || col.ColumnName == "nSaldo"))
                                dict.Add(col.ColumnName, decValue.ToString("N2", new CultureInfo("pt-BR")));
                            else
                                dict.Add(col.ColumnName, dr[col] == DBNull.Value ? null : dr[col]);
                        }
                        rows.Add(dict);
                    }

                    // Tabela 1: Total de Registros
                    if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                    {
                        totalRegistros = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRegistros"]);
                    }

                    // Tabela 2: Saldo Total
                    if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                    {
                        saldoTotalFormatado = RETORNO.DATASET(ds, 2, 0, "nSaldoGasto"); // Assume que a SP retorna formatado
                                                                                        // Tentar obter o decimal também, se a SP retornar
                        if (ds.Tables[2].Columns.Contains("nSaldoGastoDecimal")) // Exemplo, ajuste nome na SP
                        {
                            decimal.TryParse(ds.Tables[2].Rows[0]["nSaldoGastoDecimal"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out saldoTotalDecimal);
                        }
                        else // Tenta converter do formatado (menos ideal)
                        {
                            decimal.TryParse(saldoTotalFormatado.Replace("R$", "").Trim(), NumberStyles.Currency, new CultureInfo("pt-BR"), out saldoTotalDecimal);
                        }
                    }
                }

                result.Lancamentos = rows;
                result.TotalRegistros = totalRegistros;
                result.PaginaAtual = pageNumber;
                int effectivePageSizeCalc = (pageSize == -1) ? totalRegistros : pageSize; // Para cálculo de páginas
                result.TotalPaginas = (totalRegistros > 0 && effectivePageSizeCalc > 0) ? (int)Math.Ceiling((double)totalRegistros / effectivePageSizeCalc) : 1;
                result.SaldoTotalFormatado = saldoTotalFormatado;
                result.SaldoTotalDecimal = saldoTotalDecimal;

            }
            catch (Exception ex)
            {
                // Logar o erro - importante para diagnóstico
                System.Diagnostics.Trace.TraceError("Erro em GetLancamentos: " + ex.ToString());
                // Poderia retornar um objeto de erro para o JS, mas por simplicidade, retornamos vazio
                result.Lancamentos = new List<Dictionary<string, object>>();
                result.TotalRegistros = 0;
                result.TotalPaginas = 1;
                result.SaldoTotalFormatado = "Erro";
            }

            return result;
        }


        [WebMethod]
        public static object SalvarCategoriaLancamento(int idCentroDeCusto, int idLancamento, string sTipo, int idNovaCategoria)
        {
            try
            {
                // 1. Chamar a Stored Procedure para ATUALIZAR a categoria
                Dictionary<string, string> vParametrosUpdate = new Dictionary<string, string>();
                vParametrosUpdate.Add("@sFuncao", "ATUALIZAR-LANCAMENTOS"); // Função da SP que atualiza a categoria
                vParametrosUpdate.Add("@idLancamento", idLancamento.ToString());
                vParametrosUpdate.Add("@sTipo", sTipo);
                vParametrosUpdate.Add("@idNovaCategoria", idNovaCategoria.ToString());
                // Adicione @idCentroDeCusto se a SP precisar para a lógica de saldo
                vParametrosUpdate.Add("@idCentroDeCusto", idCentroDeCusto.ToString());
                vParametrosUpdate.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());

                // Execute a SP que ATUALIZA a categoria E recalcula os saldos das categorias envolvidas
                // Idealmente, a SP "ATUALIZAR-LANCAMENTOS" deve fazer o trabalho que SalvarAlteracoesDaGrid fazia.
                DataSet dsResultadoSave = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_CentroDeCusto", vParametrosUpdate); // Ajuste o nome da SP se necessário

                string msgErro = "";
                if (!BD.ValidarDataSet(dsResultadoSave, out msgErro) || RETORNO.DATASET(dsResultadoSave, "ret") != "0")
                {
                    return new { success = false, message = "Erro retornado pela procedure: " + (msgErro == "" ? RETORNO.DATASET(dsResultadoSave, "msg") : msgErro) };
                }


                // 2. Chamar a Stored Procedure para RECALCULAR o saldo total do Centro de Custo
                //    (A função 'CONSULTA_LANCAMENTO' já faz isso ao ser chamada,
                //     então podemos apenas buscar o novo saldo)
                Dictionary<string, string> vParametrosSaldo = new Dictionary<string, string>();
                vParametrosSaldo.Add("@sFuncao", "CONSULTA_LANCAMENTO");
                vParametrosSaldo.Add("@idCentroDeCusto", idCentroDeCusto.ToString());
                vParametrosSaldo.Add("@PageNumber", "1"); // Não importa a página aqui
                vParametrosSaldo.Add("@PageSize", "1"); // Só precisamos do saldo
                DataSet dsSaldo = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_CentroDeCusto", vParametrosSaldo);

                string novoSaldoFormatado = "N/A";
                if (BD.ValidarDataSet(dsSaldo) && dsSaldo.Tables.Count > 2 && dsSaldo.Tables[2].Rows.Count > 0)
                {
                    novoSaldoFormatado = RETORNO.DATASET(dsSaldo, 2, 0, "nSaldoGasto");
                }


                // Retorna sucesso e o novo saldo total para atualizar a UI
                return new { success = true, message = "Categoria atualizada com sucesso!", novoSaldo = novoSaldoFormatado };
            }
            catch (Exception ex)
            {
                // Logar o erro
                System.Diagnostics.Trace.TraceError("Erro em SalvarCategoriaLancamento: " + ex.ToString());
                return new { success = false, message = "Erro no servidor ao salvar: " + ex.Message };
            }
        }

        #endregion // Fim da Region Lançamentos Web Methods

        #region | Categorias CC
        protected void btnRefreshCategorias_Click(object sender, EventArgs e)
        {
            CarregarGridCategorias();
        }
        protected void btnRefreshHistorico_Click(object sender, EventArgs e)
        {
            Popular_Aba_Historico_Lancamentos(hddidCentroDeCusto.Value);
        }
        public List<cls_CentroDeCusto_Categoria> bs_CentroDeCusto_Categorias
        {
            get
            {
                if (ViewState["bs_CentroDeCusto_Categorias"] == null)
                {
                    ViewState["bs_CentroDeCusto_Categorias"] = new List<cls_CentroDeCusto_Categoria>();
                }
                return (List<cls_CentroDeCusto_Categoria>)ViewState["bs_CentroDeCusto_Categorias"];
            }
            set
            {
                ViewState["bs_CentroDeCusto_Categorias"] = value;
            }
        }

        private void CarregarGraficoInteligente(List<cls_CentroDeCusto_Categoria> ls_Categorias)
        {
            if (ls_Categorias == null || !ls_Categorias.Any())
            {
                div_grfTetoCategoria.Visible = false;
                return;
            }

            div_grfTetoCategoria.Visible = true;

            var chartConfig = new GenericChartData();

            var datasetTeto = new ChartDataset
            {
                label = "Teto de Gastos",
                backgroundColor = new List<string> { "rgba(54, 162, 235, 0.7)" },
                borderColor = new List<string> { "rgba(54, 162, 235, 1)" }
            };

            var datasetGastos = new ChartDataset
            {
                label = "Total Gasto",
                backgroundColor = new List<string> { "rgba(255, 99, 132, 0.7)" },
                borderColor = new List<string> { "rgba(255, 99, 132, 1)" }
            };

            var percentuaisGastos = new List<decimal>();

            foreach (var categoria in ls_Categorias.Where(c => !c.bExcluido))
            {
                chartConfig.labels.Add(categoria.sDscCategoria);

                decimal teto = categoria.nTeto ?? 0;
                decimal totalGastosPositivo = Math.Abs(categoria.nTotalGastos);

                datasetTeto.data.Add(100);
                datasetTeto.originalValues.Add(teto);

                if (teto > 0)
                {
                    decimal percentualGasto = Math.Round((totalGastosPositivo / teto) * 100, 2);
                    datasetGastos.data.Add(percentualGasto);
                    datasetGastos.originalValues.Add(totalGastosPositivo);
                    percentuaisGastos.Add(percentualGasto);
                }
                else
                {
                    datasetGastos.data.Add(0);
                    datasetGastos.originalValues.Add(0);
                    percentuaisGastos.Add(0);
                }
            }

            decimal maxPercentual = percentuaisGastos.Any() ? percentuaisGastos.Max() : 0;

            decimal maxYValue;
            if (maxPercentual > 100)
            {
                maxYValue = Math.Ceiling(maxPercentual / 10) * 10;
            }
            else
            {
                maxYValue = 100;
            }

            chartConfig.datasets.Add(datasetTeto);
            chartConfig.datasets.Add(datasetGastos);

            ChartControl.Type = ChartType.Bar;
            ChartControl.BarMode = ChartBarMode.Grouped;
            ChartControl.MaxYValue = maxYValue;
            ChartControl.YAxisShowPercentage = true;
            ChartControl.PercentageCulture = TooltipNumberCulture.Brazilian;

            ChartControl.GenericData = chartConfig;
            ChartControl.DataBindChart();
            CarregarGraficoDonuts(ls_Categorias);
        }

        private List<cls_CentroDeCusto_Categoria> CarregarGraficoDonuts(List<cls_CentroDeCusto_Categoria> ls_Categorias)
        {
            if (ls_Categorias == null)
            {
                div_grfPizzaCategorias.Visible = false;
                return new List<cls_CentroDeCusto_Categoria>(); 
            }

            var dadosParaGrafico = ls_Categorias
                .Where(c => !c.bExcluido && c.nTotalGastos != 0)
                .ToList();

            if (!dadosParaGrafico.Any())
            {
                div_grfPizzaCategorias.Visible = false;
                return dadosParaGrafico; 
            }

            div_grfPizzaCategorias.Visible = true;
            ChartControlPizza.DataSource = dadosParaGrafico;
            ChartControlPizza.DataBindChart();

            return dadosParaGrafico;
        }
        protected void ddlsTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlsTipo.SelectedValue))
            {
                CarregarCategoriasPorTipo(ddlsTipo.SelectedValue);
            }
            else
            {
                BindGridCategorias();
                MensagemPaginaCC.MostraMensagem("Selecione um Tipo para importar as categorias.", "warning", false);
            }
            updCategorias.Update();
            RegistraScript("");
        }

        private void CarregarCategoriasPorTipo(string idTipoCentroCusto)
        {
            try
            {
                var itensParaRemover = bs_CentroDeCusto_Categorias
                    .Where(c => c.idRegistro == 0 && c.idTipoCentroCusto != null)
                    .ToList();

                foreach (var item in itensParaRemover)
                {
                    bs_CentroDeCusto_Categorias.Remove(item);
                }

                var parametros = new Dictionary<string, string>
        {
            { "@sFuncao", "IMPORTAR-CATEGORIAS" },
            { "@idTipoCentroCusto", idTipoCentroCusto }
        };

                List<cls_CentroDeCusto_Categoria> categoriasDoTipo = BD.ExecutarLista<cls_CentroDeCusto_Categoria>(sProcTipoCC, parametros, true);

                foreach (var categoria in categoriasDoTipo)
                {
                    if (!bs_CentroDeCusto_Categorias.Any(c => c.idCategoria == categoria.idCategoria && c.idTipoCentroCusto == categoria.idTipoCentroCusto && c.sExclusao != "S"))
                    {
                        categoria.idRegistro = 0;
                        bs_CentroDeCusto_Categorias.Add(categoria);
                    }
                }

                BindGridCategorias();
                MensagemPaginaCC.MostraMensagem($"As categorias do tipo selecionado foram adicionadas. As categorias importadas anteriormente e não salvas foram removidas.", "info", false);
            }
            catch (Exception ex)
            {
                BindGridCategorias();
                MensagemPaginaCC.MostraMensagem_Erro("Erro ao importar categorias: " + ex.Message);
            }
        }

        private void CarregarGridCategorias()
        {
            btnSalvarCategorias.Visible = false;

            if (FUNCOES.ValidaPermissao(Permissao.Administracao.CentrodeCustos.Alterar, false))
            {
                btnSalvarCategorias.Visible = true;// !string.IsNullOrEmpty(hddidCentroDeCusto.Value) && hddidCentroDeCusto.Value != "0";
            }

            try
            {
                int idCentroDeCusto = Convert.ToInt32(hddidCentroDeCusto.Value);

                var parametros = new Dictionary<string, string>
        {
            { "@sFuncao", "CONSULTAR-CATEGORIA-TelaCC" },
            { "@idCentroCusto", idCentroDeCusto.ToString() }
        };

                bs_CentroDeCusto_Categorias = BD.ExecutarLista<cls_CentroDeCusto_Categoria>(sProcTipoCC, parametros, false);

                BindGridCategorias();
            }
            catch (Exception ex)
            {
                BindGridCategorias();
                MensagemPaginaCategoria.MostraMensagem_Erro("Erro ao carregar categorias: " + ex.Message);
            }
            updCategorias.Update();
        }


        private void BindGridCategorias()
        {
            if (bs_CentroDeCusto_Categorias != null)
            {
                gridCategorias.DataSource = bs_CentroDeCusto_Categorias.Where(c => !c.bExcluido).ToList();
                gridCategorias.DataBind();
                //GRID.SomarColunas(gridCategorias, true, GRID.Formatação.Moeda, 1);
            }

            RegistraScript("");

            //togglePanelExibeGrafico.Visible = bs_CentroDeCusto_Categorias.Count > 0;       

            bool temCategorias = bs_CentroDeCusto_Categorias != null && bs_CentroDeCusto_Categorias.Any(c => !c.bExcluido);
            togglePanelExibeGrafico.Visible = temCategorias;

            if (temCategorias)
            {
                if (hddGraficoState.Value == "open")
                {
                    graficoPanelBody.Style["display"] = "block";
                    iconDownExibe.Attributes["class"] = "fa fa-chevron-up";
                }
                else
                {
                    graficoPanelBody.Style["display"] = "none";
                    iconDownExibe.Attributes["class"] = "fa fa-chevron-down";
                }
            }
            CarregarGraficoInteligente(bs_CentroDeCusto_Categorias);
            CarregarGraficoDonuts(bs_CentroDeCusto_Categorias);
            updCategorias.Update();
        }

        protected void cmdIncluirCategoriaCC_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtsDscCategoria.Text))
            {
                MensagemPaginaCategoria.MostraMensagem_Erro("A descrição da categoria é obrigatória.");
                CarregarGraficoInteligente(bs_CentroDeCusto_Categorias);
                updCategorias.Update();
                return;
            }

            AtualizarListaComDadosDaGrid();

            string nomeCategoria = txtsDscCategoria.Text.Trim();

            if (bs_CentroDeCusto_Categorias.Any(c => c.sDscCategoria.Equals(nomeCategoria, StringComparison.OrdinalIgnoreCase)))
            {
                MensagemPaginaCategoria.MostraMensagem_Erro("Já existe uma categoria com este nome na lista.");
                BindGridCategorias();
                updCategorias.Update();
                return;
            }

            var novaCategoria = new cls_CentroDeCusto_Categoria
            {
                idCategoria = 0,
                idRegistro = 0,
                sDscCategoria = nomeCategoria,
                nTeto = BD.Conversoes.Numerico_Decimal(txtnTetoCategoria.Text),
                nSaldo = BD.Conversoes.Numerico_Decimal(txtnSaldoCategoria.Text),
                idCentroCusto = Convert.ToInt32(hddidCentroDeCusto.Value),
                sEditar="S",
                sExclusao = "N"
            };

            bs_CentroDeCusto_Categorias.Add(novaCategoria);

            BindGridCategorias();
            LimparCamposInclusaoCategoria();

            MensagemPaginaCategoria.MostraMensagem("Categoria adicionada à lista. Clique em 'Salvar' para persistir.", "info", false);
            updCategorias.Update();
            txtsDscCategoria.Focus();
        }
        protected void btnSalvarCategorias_Click(object sender, EventArgs e)
        {
            string idTipo = ddlsTipo.SelectedValue;
            SalvarCategorias(hddidCentroDeCusto.Value);
            ddlsTipo.SelectedValue = idTipo;
            AtualizarTipo(idTipo);
            RegistraScript("");
        }
        bool SalvarCategorias(string idCC)
        {
            AtualizarListaComDadosDaGrid();

            if (!ValidarCategorias())
            {
                updCategorias.Update();
                return false;
            }

            try
            {
                
                foreach (var categoria in bs_CentroDeCusto_Categorias)
                {
                    var parametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR-CATEGORIA-TelaCC" },
                        { "@idRegistro", categoria.idRegistro.ToString() },
                        { "@idCategoria", categoria.idCategoria.ToString() },
                        { "@idCentroCusto", hddidCentroDeCusto.Value },
                        { "@idTipoCentroCusto", categoria.idTipoCentroCusto?.ToString() },
                        { "@sDscCategoria", categoria.sDscCategoria },
                        { "@pTeto", categoria.nTeto?.ToString(CultureInfo.InvariantCulture) },
                        { "@pSaldo", categoria.nSaldo?.ToString(CultureInfo.InvariantCulture)  },
                        { "@sExclusao", categoria.sExclusao },
                        { "@idUsuarioAtualizacao",  IDENTITY.Variaveis.idUsuario()}

                    };

                    if (categoria.sEditar == "S")
                    BD.ExecutarComandoLista(sProcTipoCC, parametros, true);
                }

                MensagemPaginaCategoria.MostraMensagem_Sucesso("Categorias salvas com sucesso!");
                BindGridCategorias();
                updCategorias.Update();
                return true;
            }
            catch (Exception ex)
            {
                MensagemPaginaCategoria.MostraMensagem_Erro($"Erro ao salvar categorias: {ex.Message}");
                BindGridCategorias();
                updCategorias.Update();
                return false;
            }
        }

        void AtualizarTipo(string idTipo)
        {
            string sErro = "";
            if (!string.IsNullOrEmpty(idTipo) && idTipo != "0")
            {
                try
                {
                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro
                    vParametros.Add("@sFuncao", "ATUALIZAR-TIPO");
                    vParametros.Add("@idCentroDeCusto", hddidCentroDeCusto.Value);
                    vParametros.Add("@idTipoCentroDeCusto", idTipo);

                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        MensagemPagina.MostraMensagem_Sucesso("Tipo atualizado com sucesso!");
                    }
                    else
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }

                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro($"Erro ao salvar categorias: {ex.Message}");
                }
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro($"Nenhum Tipo foi selecionado.");
            }

        }

        protected void gridCategorias_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //Timer1.Enabled = false;
            gridCategorias.EditIndex = e.NewEditIndex;
            this.DeletingRowIndex = -1;
            BindGridCategorias();
        }

        protected void gridCategorias_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //Timer1.Enabled = false;
            e.Cancel = true;
            gridCategorias.EditIndex = e.RowIndex;
            this.DeletingRowIndex = e.RowIndex;
            BindGridCategorias();
        }

        protected void gridCategorias_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            //Timer1.Enabled = false;
            gridCategorias.EditIndex = -1;
            this.DeletingRowIndex = -1;
            BindGridCategorias();
        }

        protected void gridCategorias_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            //Timer1.Enabled = false;

            GridViewRow row = gridCategorias.Rows[e.RowIndex];

            object key = gridCategorias.DataKeys[e.RowIndex].Value;
            var itemParaAtualizar = bs_CentroDeCusto_Categorias.FirstOrDefault(c => c.GridKey.Equals(key));

            if (itemParaAtualizar != null)
            {
                string novoNomeCategoria = ((TextBox)row.FindControl("txtGridDscCategoria")).Text.Trim();
                string tetoString = ((TextBox)row.FindControl("txtGridTeto")).Text;

                if (bs_CentroDeCusto_Categorias.Any(c => c.sDscCategoria.Equals(novoNomeCategoria, StringComparison.OrdinalIgnoreCase) && !c.GridKey.Equals(key)))
                {
                    MensagemPaginaCategoria.MostraMensagem_Erro("Já existe uma categoria com este nome. Por favor, use um nome diferente.");
                    e.Cancel = true; 
                    return;
                }

                string tetoLimpo = tetoString.Replace("R$", "").Replace(".", "").Trim();

                itemParaAtualizar.sDscCategoria = novoNomeCategoria;
                itemParaAtualizar.nTeto = BD.Conversoes.Numerico_Decimal(tetoLimpo);
            }

            gridCategorias.EditIndex = -1;
            this.DeletingRowIndex = -1;

            BindGridCategorias();
        }

        protected void gridCategorias_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //Timer1.Enabled = false;
            if (e.CommandName == "ConfirmDelete")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                object key = gridCategorias.DataKeys[rowIndex].Value;

                var itemParaExcluir = bs_CentroDeCusto_Categorias.FirstOrDefault(c => c.GridKey.Equals(key));

                if (itemParaExcluir != null)
                {
                    if (itemParaExcluir.idRegistro > 0)
                    {
                        itemParaExcluir.bExcluido = true;
                        MensagemPaginaCategoria.MostraMensagem("Item marcado para exclusão. Clique em 'Salvar' para confirmar.", "warning", false);
                    }
                    else
                    {
                        bs_CentroDeCusto_Categorias.Remove(itemParaExcluir);
                        MensagemPaginaCategoria.MostraMensagem("Categoria removida da lista.", "info", false);
                    }

                    gridCategorias.EditIndex = -1;
                    this.DeletingRowIndex = -1;
                }
                BindGridCategorias();
            }
            if (e.CommandName == "Update")
            {

            }
       
        }


        protected void gridCategorias_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.DataItem != null)
            {
                try
                {

                
                var lnkEditar = e.Row.FindControl("lnkEditar") as LinkButton;
                var lnkExcluir = e.Row.FindControl("lnkExcluir") as LinkButton;


                //if (Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "nSaldo")) > 0)
                //{
                //    e.Row.CssClass = "success";
                //}
                if (Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "nSaldo")) < 0)
                {
                    e.Row.CssClass = "danger";
                }



                object objnTotalGastos = DataBinder.Eval(e.Row.DataItem, "nTotalGastos");

                decimal nTotalGastos = 0;
                if (objnTotalGastos != null && objnTotalGastos != DBNull.Value)
                {
                    nTotalGastos = Convert.ToDecimal(objnTotalGastos);
                }

                // *** CORREÇÃO APLICADA AQUI ***
                // O botão Editar (lnkEditar) agora fica sempre visível.
                // Apenas o botão Excluir (lnkExcluir) é ocultado se houver saldo.
                if (nTotalGastos != 0)
                {
                    // if (lnkEditar != null) lnkEditar.Visible = false; // LINHA REMOVIDA
                    if (lnkExcluir != null) lnkExcluir.Visible = false; // LINHA MANTIDA
                }
                string sEditar = DataBinder.Eval(e.Row.DataItem, "sEditar").ToString();

                if (sEditar == "N")
                    lnkEditar.Visible = false;


                bool isDeleting = (e.Row.RowIndex == this.DeletingRowIndex);
                var lnkSalvar = e.Row.FindControl("lnkSalvar") as LinkButton;
                var lnkCancelar = e.Row.FindControl("lnkCancelar") as LinkButton;
                var lnkConfirmarExclusao = e.Row.FindControl("lnkConfirmarExclusao") as LinkButton;
                var lnkCancelarExclusao = e.Row.FindControl("lnkCancelarExclusao") as LinkButton;

                if (e.Row.RowState.HasFlag(DataControlRowState.Edit))
                {
                    if (lnkSalvar != null) lnkSalvar.Visible = !isDeleting;
                    if (lnkCancelar != null) lnkCancelar.Visible = !isDeleting;
                    if (lnkConfirmarExclusao != null) lnkConfirmarExclusao.Visible = isDeleting;
                    if (lnkCancelarExclusao != null) lnkCancelarExclusao.Visible = isDeleting;

                    if (isDeleting)
                    {
                        var txtDesc = e.Row.FindControl("txtGridDscCategoria") as TextBox;
                        var txtTeto = e.Row.FindControl("txtGridTeto") as TextBox;
                        var txtSaldo = e.Row.FindControl("txtGridSaldo") as TextBox;

                        if (txtDesc != null) txtDesc.ReadOnly = true;
                        if (txtTeto != null) txtTeto.ReadOnly = true;
                        if (txtSaldo != null) txtSaldo.ReadOnly = true;
                    }
                }
                }
                catch (Exception)
                {
                }
            }
            RegistraScript("");
        }

        private void LimparCamposInclusaoCategoria()
        {
            txtsDscCategoria.Text = string.Empty;
            txtnTetoCategoria.Text = string.Empty;
            txtnSaldoCategoria.Text = string.Empty;
        }
        void LimpaClasses()
        {
            bs_CentroDeCusto_Categorias.Clear();
            BindGridCategorias();
        }
        private void AtualizarListaComDadosDaGrid(int rowIndex = -1)
        {
            int index = (rowIndex != -1) ? rowIndex : gridCategorias.EditIndex;

            if (index >= 0 && index < gridCategorias.Rows.Count)
            {
                GridViewRow row = gridCategorias.Rows[index];
                object key = gridCategorias.DataKeys[index].Value;
                var itemParaAtualizar = bs_CentroDeCusto_Categorias.FirstOrDefault(c => c.GridKey.Equals(key));

                if (itemParaAtualizar != null)
                {
                    string novoNomeCategoria = ((TextBox)row.FindControl("txtGridDscCategoria")).Text.Trim();

                    if (bs_CentroDeCusto_Categorias.Any(c => c.sDscCategoria.Equals(novoNomeCategoria, StringComparison.OrdinalIgnoreCase) && !c.GridKey.Equals(key)))
                    {
                        MensagemPaginaCategoria.MostraMensagem_Erro("Já existe uma categoria com este nome na lista. Por favor, use um nome diferente.");
                        updCategorias.Update();
                        return;
                    }

                    string tetoString = ((TextBox)row.FindControl("txtGridTeto")).Text;
                    string saldoString = ((TextBox)row.FindControl("txtGridSaldo")).Text;

                    string tetoLimpo = tetoString.Replace("R$", "").Trim();
                    string saldoLimpo = saldoString.Replace("R$", "").Trim();

                    itemParaAtualizar.sDscCategoria = novoNomeCategoria;
                    itemParaAtualizar.nTeto = BD.Conversoes.Numerico_Decimal(tetoLimpo);
                    itemParaAtualizar.nSaldo = BD.Conversoes.Numerico_Decimal(saldoLimpo);
                }
            }
        }

        private bool ValidarCategorias()
        {
            for (int i = 0; i < bs_CentroDeCusto_Categorias.Count; i++)
            {
                var item = bs_CentroDeCusto_Categorias[i];
                if (!item.bExcluido && string.IsNullOrWhiteSpace(item.sDscCategoria))
                {
                    MensagemPaginaCategoria.MostraMensagem_Erro($"A descrição da categoria na linha {i + 1} não pode ser vazia.");
                    BindGridCategorias();
                    return false;
                }
            }
            return true;
        }

        [Serializable]
        public class cls_CentroDeCusto_Categoria
        {
            private static int _tempIdCounter = -1;
            public int TempId { get; private set; }

            public int idCategoria { get; set; }
            public string sDscCategoria { get; set; }
            public decimal? nTeto { get; set; }
            public decimal? nSaldo { get; set; }
            public decimal nTotalGastos { get; set; }
            public DateTime? dtAtualizacao { get; set; }
            public int? idUsuario { get; set; }
            public int? idCentroCusto { get; set; }
            public int? idTipoCentroCusto { get; set; }
            public int idRegistro { get; set; } // Representa o idRegistro da tabela X
            public string sExclusao { get; set; }
            public string sEditar { get; set; }

            public bool bExcluido
            {
                get { return sExclusao == "S"; }
                set { sExclusao = value ? "S" : "N"; }
            }

            public cls_CentroDeCusto_Categoria()
            {
                this.TempId = _tempIdCounter--;
            }

            public object GridKey
            {
                get
                {
                    if (this.idRegistro > 0)
                        return (object)this.idRegistro;
                    if (this.idCategoria > 0)
                        return (object)this.idCategoria;
                    return (object)this.TempId;
                }
            }
        }
        #endregion

        #region | Historico Categoria
        protected void btnGlobalRefresh_Click(object sender, EventArgs e)
        {
            string idCentroDeCusto = hddidCentroDeCusto.Value;
            if (string.IsNullOrEmpty(idCentroDeCusto) || idCentroDeCusto == "0")
                return; // Não faz nada se não houver CC

            try
            {
                // 1. ATUALIZA GRIDS DE HISTÓRICO
                // Busca os dados do histórico de CC (que vem da SP CONSULTAR_DETALHE)
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                vParametros.Add("@idCentroDeCusto", idCentroDeCusto);
                DataSet dsRefresh = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsRefresh))
                {
                    // 1a. Atualiza APENAS os campos de saldo (NÃO TOCA EM txtsDescricao)
                    txtnSaldoGasto.Text = RETORNO.DATASET(dsRefresh, 0, "nSaldoGasto");
                    txtdtUltimaAtualizacao.Text = RETORNO.DATASET(dsRefresh, 0, "dtUltimaAtualizacao");

                    // 1b. Repopula o Histórico de CC (Tabela 1 do DataSet)
                    Popular_Aba_Historico(dsRefresh);
                }

                // 1c. Repopula o Histórico de Lançamentos
                Popular_Aba_Historico_Lancamentos(idCentroDeCusto);

                // 2. ATUALIZA CATEGORIAS E GRÁFICOS
                CarregarGridCategorias(); // Isso já recarrega os dados da grid/gráficos

                // 3. ATUALIZA OS PAINÉIS
                updDetalhe.Update(); // Atualiza SÓ o txtnSaldoGasto
                updHistorico.Update(); // Atualiza as duas grids de histórico
                                       // updCategorias.Update() já é chamado por CarregarGridCategorias()
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro no refresh automático: " + ex.Message);
            }
        }
        private void Popular_Aba_Historico_Lancamentos(string idCentroDeCusto)
        {
            try
            {
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR_HISTORICO_LANCAMENTO");
                vParametros.Add("@idCentroDeCusto", idCentroDeCusto);

                DataSet dsHistoricoLancamentos = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsHistoricoLancamentos))
                {
                    // Vincula os dados à nova GridView
                    gv_Historico_Lancamentos.DataSource = dsHistoricoLancamentos.Tables[0];
                    gv_Historico_Lancamentos.DataBind();
                }
                else
                {
                    gv_Historico_Lancamentos.DataSource = null;
                    gv_Historico_Lancamentos.DataBind();
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao carregar histórico de lançamentos: " + ex.Message);
                gv_Historico_Lancamentos.DataSource = null;
                gv_Historico_Lancamentos.DataBind();
            }
        }

        protected void btnToggleHistorico_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            // 1. Mostra/Oculta os painéis (Isto está correto)
            divHistoricoCC.Visible = (btn.ID == "btnToggleHistoricoCC");
            divHistoricoLancamentos.Visible = (btn.ID == "btnToggleHistoricoLancamentos");

            // 2. *** CORREÇÃO AQUI ***
            //    Aplicamos a classe "active" ao <li> (liHistoricoCC) 
            //    e não ao <asp:LinkButton> (btnToggleHistoricoCC)
            liHistoricoCC.Attributes["class"] = (btn.ID == "btnToggleHistoricoCC") ? "active" : "";
            liHistoricoLancamentos.Attributes["class"] = (btn.ID == "btnToggleHistoricoLancamentos") ? "active" : "";

            // 3. Força a atualização do UpdatePanel (Isto está correto)
            updHistorico.Update();
        }
        #endregion

        protected void gvResumoFinanceiros_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (DataBinder.Eval(e.Row.DataItem, "sDscTitulo").ToString() == "Total Receita")
                {
                    e.Row.CssClass = "info";
                    e.Row.Cells[0].ColumnSpan = 2;
                    e.Row.Cells[1].Visible = false;
                    
                }

                if (DataBinder.Eval(e.Row.DataItem, "sDscTitulo").ToString() == "Total Despesas")
                {
                    e.Row.CssClass = "warning";
                    e.Row.Cells[0].ColumnSpan = 2;
                    e.Row.Cells[1].Visible = false;
                }

                if (DataBinder.Eval(e.Row.DataItem, "sDscTitulo").ToString().Contains("Despesas:"))
                {
                    e.Row.CssClass = "warning";
                    e.Row.Font.Italic = true;
                    e.Row.Cells[0].Style.Add("padding-left", "40px;");
                    //e.Row.Cells[1].ColumnSpan = 2;
                    //e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Left;
                    //e.Row.Cells[2].Visible = false;
                }

                if (DataBinder.Eval(e.Row.DataItem, "sDscTitulo").ToString().Contains("Liquido"))
                {
                    if (Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "nTotal")) > 0)
                    {
                        e.Row.CssClass = "success";
                    }
                    else if (Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "nTotal")) < 0)
                    {
                        e.Row.CssClass = "danger";
                    }



                   // e.Row.Cells[0].ColumnSpan = 2;
                   // e.Row.Cells[1].Visible = false;
                }

            }
        }
    }

    [Serializable]
    public class LancamentosResult
    {
        public List<Dictionary<string, object>> Lancamentos { get; set; }
        public int TotalRegistros { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public string SaldoTotalFormatado { get; set; }
        public decimal SaldoTotalDecimal { get; set; }
    }
}