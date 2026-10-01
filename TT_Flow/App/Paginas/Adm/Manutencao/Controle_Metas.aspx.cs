using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using TT.FrameWork;
using TT_Flow.FrameWork;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Globalization;
using static TT_Flow.App.Controles.MultiSelecao;

namespace TT_Flow.App.Paginas.Adm.Manutencao
{
    public partial class Controle_Metas : System.Web.UI.Page
    {

        string sTituloPagina = "Controle de Metas";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_Metas";

        #region | Classes

        public List<FrameWork.cls_Meta> bs_Meta
        {
            get
            {
                if (ViewState["bs_Meta"] == null)
                {
                    ViewState["bs_Meta"] = new List<FrameWork.cls_Meta>();
                }
                return (List<FrameWork.cls_Meta>)ViewState["bs_Meta"];
            }
            set
            {
                ViewState["bs_Meta"] = value;
            }
        }

        public List<FrameWork.cls_Meta_Vendedor> bs_Meta_Vendedor_Mensal
        {
            get
            {
                if (ViewState["bs_Meta_Vendedor_Mensal"] == null)
                {
                    ViewState["bs_Meta_Vendedor_Mensal"] = new List<FrameWork.cls_Meta_Vendedor>();
                }
                return (List<FrameWork.cls_Meta_Vendedor>)ViewState["bs_Meta_Vendedor_Mensal"];
            }
            set
            {
                ViewState["bs_Meta_Vendedor_Mensal"] = value;
            }
        }

        public List<FrameWork.cls_Meta_Vendedor> bs_Meta_Vendedor_Modal
        {
            get
            {
                if (ViewState["bs_Meta_Vendedor_Modal"] == null)
                {
                    ViewState["bs_Meta_Vendedor_Modal"] = new List<FrameWork.cls_Meta_Vendedor>();
                }
                return (List<FrameWork.cls_Meta_Vendedor>)ViewState["bs_Meta_Vendedor_Modal"];
            }
            set
            {
                ViewState["bs_Meta_Vendedor_Modal"] = value;
            }
        }

        #endregion

        #region | Funções Incialização do Forms
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();


            if (!IsPostBack)
            {
                FUNCOES.ValidaPermissao(Permissao.Administracao.Meta.Consultar, true);
                PopularCombos();

                string aba = Request.QueryString["aba"];
                if (aba == "comercial")
                {
                    string script = @"
                                    <script type='text/javascript'>
                                        $(document).ready(function() {
                                            $('#comercial-tab').tab('show');
                                        });
                                    </script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "AbaComercial", script);
                }
                else if (aba == "financeiro")
                {
                    string script = @"
                                    <script type='text/javascript'>
                                        $(document).ready(function() {
                                            $('#Meta-tab').tab('show'); 
                                        });
                                    </script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "AbaFinanceiro", script);
                }

                Pesquisar();
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
                    Salvar_Meta();
                }                
            }

            RegistraScript("");

        }       

        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar()
        {           

            string sErro = "";

            try
            {
                LimpaCampos();

                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR");
                vParametros.Add("@idEmpresaFiltro", ddlidEmpresaFiltro.SelectedValue);
                vParametros.Add("@nAnoFiltro", txtnAnoComercialFiltro.Text);
                vParametros.Add("@idVendedorFiltro", ddlidVendedorFiltro.SelectedValue);
                DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out sErro))
                {
                    foreach (DataRow row in dsPesquisa.Tables[0].Rows)
                    {
                        FrameWork.cls_Meta objItem = new FrameWork.cls_Meta();

                        objItem.IdMeta = Convert.ToInt32(row["idMeta"].ToString());
                        objItem.NValor = Convert.ToDecimal(row["nValor"].ToString());
                        objItem.SDscUsuarioAtualizacao = row["sDscUsuarioAtualizacao"].ToString();
                        objItem.DtAtualizacao = row["dtAtualizacao"].ToString();
                        objItem.NAno = Convert.ToInt32(row["nAno"].ToString());
                        objItem.IdMes = Convert.ToInt32(row["idMes"].ToString());
                        objItem.SDscMes = row["sDscMes"].ToString();
                        objItem.SFuncao = "ATUAL";
                        bs_Meta.Add(objItem);

                    }

                    Popula_Aba_Comercial(dsPesquisa);

                    dtgMeta_DataBind();
                    LimpaCampos_Meta();
                    PainelAtualizacao.Visible = true;
                    PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 1, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 1, 0, "sDscUsuarioAtualizacao"));

                    int anoAtual = DateTime.Now.Year;
                    txtnAno.Text = anoAtual.ToString();                    

                    lblTituloSalvar.Text = "Confirma salvar as metas inseridas/alteradas?";

                    cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Administracao.Meta.Alterar);

                    Popular_Aba_Historico(dsPesquisa);
                }

                else
                {
                    throw new Exception(sErro);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }

        #endregion

        #region | Limpar Campos

        void LimpaCampos()
        {
            hddidMeta.Value = "0";
            txtnValor.Text = "";

            int anoAtual = DateTime.Now.Year;
            txtnAno.Text = anoAtual.ToString();


            bs_Meta.Clear();

            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
        }

        #endregion

        #region | Validação 

        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";


            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        #endregion

        #region | Script 

        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            //Mensagens de Confirmação
            sb.Append("$v192(function() {");

            sb.Append("$v192(\"#dialog-Salvar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_SALVAR\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('#cphCorpo_cmdSalvar').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("$('[id*=txtnValor]').mask('000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnValorComercial]').mask('000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnAno]').mask('0000', { reverse: true });");
            sb.Append("$('[id*=txtnAnoComercial]').mask('0000', { reverse: true });");

            sb.Append("});");

            // Script para exibir a Composição dos Itens no método por Linhas, sem a utilização de modal
            sb.Append("$('.composicaoLinha').addClass('fa fa-plus');\r\n");
            sb.Append("$('.composicaoLinha').click(function() {\r\n");
            sb.Append("     var icon = $(this);\r\n");
            sb.Append("     var divId = $(this).data('div-id');\r\n");
            sb.Append("     var current = $('#' + divId).css('display');\r\n");
            sb.Append("     if (current == 'none') {\r\n");
            sb.Append("         $('#' + divId).show('slow');\r\n");
            sb.Append("         icon.removeClass('fa fa-plus').addClass('fa fa-minus');\r\n");
            sb.Append("     } else {\r\n");
            sb.Append("         $('#' + divId).hide('slow');\r\n");
            sb.Append("         icon.removeClass('fa fa-minus').addClass('fa fa-plus');\r\n");
            sb.Append("     }\r\n");
            sb.Append("     return false;\r\n");
            sb.Append("});\r\n\r\n");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }

        #endregion

        #region | Combos/DDL

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidMes, "sp_Select 'Flow_Mes'", "idMes", "sDscMes", false, "Selecione um Mês", "0");
            Funcoes.Popula_Combo(ddlidVendedor, "sp_Select 'FLOW_Vendedores_Ativos'", "idVendedor", "sDscUsuario", false, "Selecione um Vendedor", "0");
            Funcoes.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa'", "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");

            Funcoes.Popula_Combo(ddlidVendedorFiltro, "sp_Select 'FLOW_Vendedores_Ativos'", "idVendedor", "sDscUsuario", false, "Selecione um Vendedor", "0");
            Funcoes.Popula_Combo(ddlidEmpresaFiltro, "sp_Select 'Flow_Empresa'", "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");

        }
          
        #endregion

        #region | Meta

        void Salvar_Meta()
        {

            try
            {
                SalvarGrid();
                Dictionary<String, String> vParametroMeta = new Dictionary<string, string>();
                foreach (cls_Meta Meta_Linha in bs_Meta)
                {

                    vParametroMeta["@sFuncao"] = Meta_Linha.SFuncao;
                    vParametroMeta["@idMeta"] = Meta_Linha.IdMeta.ToString();
                    vParametroMeta["@idMes"] = Meta_Linha.IdMes.ToString();
                    vParametroMeta["@nValor"] = BD.Conversoes.Numerico(Meta_Linha.NValor);
                    vParametroMeta["@nAno"] = Meta_Linha.NAno.ToString();
                    vParametroMeta["@idUsuarioAtualizacao"] = IDENTITY.Variaveis.idUsuario();
                    BD.ExecutarDataSet(sProcedure, vParametroMeta);
                }
                MensagemPagina.MostraMensagem_Sucesso("Registros salvos/alterados com sucesso!");
                Pesquisar();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
                        
        }

        void LimpaCampos_Meta()
        {
            ddlidMes.SelectedValue = "0";
            txtnValor.Text = "";
            ddlidVendedor.SelectedValue = "0";
            txtnAnoComercial.Text = DateTime.Now.Year.ToString();
            ddlidEmpresa.SelectedValue = "0";
        }

        private bool ValidarDados_Meta(ref string sMensagemErro)
        {
           
            if (ddlidMes.SelectedValue == "0")
            {
                sMensagemErro += "Selecione um mês <br/>";
            }

            if (!Validacoes.ValidarMoeda(txtnValor))
            {
                sMensagemErro += "Inserir um Valor válido <br/>";
            }

            if (txtnAno.Text.Length < 4)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Inserir um Ano válido ";
            }                        

            int anoSelecionado = Convert.ToInt32(txtnAno.Text);
            int mesSelecionado = Convert.ToInt32(ddlidMes.SelectedValue);            

            bool jaExiste = bs_Meta.Any(meta => meta.NAno == anoSelecionado && meta.IdMes == mesSelecionado && meta.SFuncao != "EXCLUIR_META");
            if (jaExiste && hddAbaComercial.Value == "N")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Já existe uma meta cadastrada para o mês e ano selecionados.";
                return false;
            }

            return sMensagemErro != "" ? false : true;

        }

        void SalvarGrid()
        {
            int nContador = 0;
            decimal nValor_NOVO = 0;

            foreach (GridViewRow item in dtgMeta.Rows)
            {
                TextBox txtnValor_Linha = (TextBox)item.FindControl("txtnValor");
                nValor_NOVO = Convert.ToDecimal(txtnValor_Linha.Text);

                if (nValor_NOVO != bs_Meta[nContador].NValor)
                {
                    bs_Meta[nContador].NValor = nValor_NOVO;
                    bs_Meta[nContador].SFuncao = "SALVAR";
                }

                nContador++;
            }

        }

        protected void cmdMeta_Incluir_Click(object sender, EventArgs e)
        {
            string sMensagem = "";
            if (ValidarDados_Meta(ref sMensagem))
            {
                FrameWork.cls_Meta objItem = new FrameWork.cls_Meta();
                objItem.IdMeta = 0;
                objItem.IdMes = Convert.ToInt32(ddlidMes.SelectedValue);
                objItem.SDscMes = ddlidMes.SelectedItem.ToString();
                objItem.NAno = Convert.ToInt32(txtnAno.Text.ToString());
                objItem.NValor = Convert.ToDecimal(txtnValor.Text);
                objItem.SFuncao = "SALVAR";

                bs_Meta.Add(objItem);
                dtgMeta_DataBind();
                LimpaCampos_Meta();

                MensagemPagina.MostraMensagem("Lembre-se de clicar em \"Salvar\" para salvar os registros incluídos!", "info", false);
            }
            else
            {
                MensagemAcoes.MostraMensagem_Erro(sMensagem, false);
            }

            RegistraScript("$('[id$=txtnValor]').focus();");
        }

        void dtgMeta_DataBind()
        {
            dtgMeta.DataSource = bs_Meta.Where(c => c.SFuncao.ToString() != "EXCLUIR_META");
            dtgMeta.DataBind();
        }

        protected void dtgMeta_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            bs_Meta[index].SFuncao = "EXCLUIR_META";
            dtgMeta_DataBind();
        }

        protected void dtgMeta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0, 1);

            if (e.Row.RowType == DataControlRowType.DataRow)
                e.Row.CssClass = "gvMainTd";

            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "gvMainTh";
        }
        

        void Popular_dtgMeta(DataSet dsPesquisa)
        {
            PopularCombos();

        }      

        #endregion

        #region | Meta Comercial

        private void Popula_Aba_Comercial(DataSet dsPesquisa)
        {
            List<cls_Meta_Vendedor> metasVendedores = GetMetasVendedores(dsPesquisa.Tables[2]);
            List<cls_EmpresaMeta> metas = GetMetasFiltradas(metasVendedores);

            bs_Meta_Vendedor_Mensal = metasVendedores;

            gv_MetaComercial.DataSource = metas;
            gv_MetaComercial.DataBind();
        }        

        private List<cls_EmpresaMeta> GetMetasFiltradas(List<cls_Meta_Vendedor> metasVendedores)
        {           
            var empresas = metasVendedores
                .GroupBy(x => new { x.idEmpresa, x.sDscEmpresa })
                .Select(gEmp =>
                {
                    decimal totalEmpresa = gEmp.Sum(x => x.nValor);                    
                    var metasMensais = gEmp
                        .GroupBy(x => new { x.idEmpresa , x.nAno, x.idMes, x.sDscMes })
                        .Select(gMes =>
                        {
                            decimal somaPeriodo = gMes.Sum(x => x.nValor);
                            
                            return new cls_Meta_Mensal
                            {
                                idEmpresa = gMes.Key.idEmpresa,
                                nAno = gMes.Key.nAno,
                                idMes = gMes.Key.idMes,
                                sDscMes = gMes.Key.sDscMes,
                                nTotalMetaMensal = somaPeriodo,
                                lsMetaVendedores = gMes.ToList() 
                            };
                        })
                        .ToList();

                    return new cls_EmpresaMeta
                    {
                        idEmpresa = gEmp.Key.idEmpresa,
                        sEmpresa = gEmp.Key.sDscEmpresa,
                        nTotalEmpresa = totalEmpresa,
                        lsMetasMensais = metasMensais
                    };
                })
                .ToList();

            return empresas;
        }

        private List<cls_Meta_Vendedor> GetMetasVendedores(DataTable dt)
        {
            List<cls_Meta_Vendedor> metasVendedores = new List<cls_Meta_Vendedor>();

            foreach (DataRow row in dt.Rows)
            {
                cls_Meta_Vendedor metaVendedor = new cls_Meta_Vendedor
                {
                    idMeta = Convert.ToInt32(row["idMeta"]),
                    nValor = Convert.ToDecimal(row["nValor"]),
                    dtAtualizacao = row["dtAtualizacao"].ToString(),
                    nAno = Convert.ToInt32(row["nAno"]),
                    idMes = Convert.ToInt32(row["idMes"]),
                    sDscMes = row["sDscMes"].ToString(),
                    idVendedor = Convert.ToInt32(row["idVendedor"]),
                    sDscVendedor = row["sDscVendedor"].ToString(),
                    idEmpresa = Convert.ToInt32(row["idEmpresa"]),
                    sDscEmpresa = row["sDscEmpresa"].ToString()
                };

                metasVendedores.Add(metaVendedor);
            }

            return metasVendedores;
        }

        private void ConsultaMetaVendedor()
        {
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR");
            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            Popula_Aba_Comercial(dsPesquisa);
        }

        private void ExcluirMetaVendedor(int idMeta)
        {
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "EXCLUIR_META");
            vParametros.Add("@idMeta", idMeta.ToString());
            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
        }       

        private void PopulaModalMetaVendedor(string sAno, string sidVendedor, string idEmpresa)
        {
            div_gvMetaVendedor.Visible = true;
            btnSalvar.Visible = true;

            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTA_META_VENDEDOR");
            vParametros.Add("@nAno", sAno);
            vParametros.Add("@idVendedor", sidVendedor);
            vParametros.Add("@idEmpresa", idEmpresa);
            DataTable dsPesquisa = BD.ExecutarDataTable(sProcedure, vParametros);

            List<cls_Meta_Vendedor> metaVendedor = new List<cls_Meta_Vendedor>();

            foreach (DataRow row in dsPesquisa.Rows)
            {
                var item = new cls_Meta_Vendedor
                {
                    idMeta = Convert.ToInt32(row["idMeta"]),
                    nValor = Convert.ToDecimal(row["nValor"]),                    
                    nAno = Convert.ToInt32(row["nAno"]),
                    idMes = Convert.ToInt32(row["idMes"]),
                    sDscMes = row["sDscMes"].ToString(),
                    idVendedor = Convert.ToInt32(row["idVendedor"]),
                    sDscVendedor = row["sDscVendedor"].ToString(),
                    idEmpresa = Convert.ToInt32(row["idEmpresa"]),
                    sDscEmpresa = row["sDscEmpresa"].ToString()
                };

                metaVendedor.Add(item);
            }

            bs_Meta_Vendedor_Modal = metaVendedor;

            gvMetaVendedor.DataSource = metaVendedor;
            gvMetaVendedor.DataBind();

            //PopularCombos(); //REVER
            RegistraScript("");
        }

        private bool ValidaCampos()
        {
            bool bRetorno = true;
            string sMensagemErro = "";                

            if (ddlidEmpresa.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
            }

            if (txtnAnoComercial.Text.Length < 4)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira um Ano valído!";
            }

            if (ddlidVendedor.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Vendedor!";
            }           

            if (sMensagemErro != "")
            {
                bRetorno = false;

                ResetaModal();

                MensagemPaginaModal.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        private void ResetaModal()
        {
            div_gvMetaVendedor.Visible = false;
            btnSalvar.Visible = false;
            gvMetaVendedor.DataSource = null;
            gvMetaVendedor.DataBind();
        }

        protected void btnIncluir_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidaCampos())
                {
                    PopulaModalMetaVendedor(txtnAnoComercial.Text, ddlidVendedor.SelectedValue, ddlidEmpresa.SelectedValue);
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaModal.MostraMensagem_Erro(ex.Message);
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            
            foreach (GridViewRow row in gvMetaVendedor.Rows)
            {                
                try
                {                   
                    decimal valor = decimal.Parse(((TextBox)row.FindControl("txtnValorComercial")).Text.Replace(".", "").Replace(",", "."), CultureInfo.InvariantCulture);
                    if (valor == 0)
                        continue;

                    int idMeta = Convert.ToInt32(gvMetaVendedor.DataKeys[row.RowIndex]["idMeta"]);
                    int idEmpresa = Convert.ToInt32(gvMetaVendedor.DataKeys[row.RowIndex]["idEmpresa"]);
                    int idVendedor = Convert.ToInt32(gvMetaVendedor.DataKeys[row.RowIndex]["idVendedor"]);
                    int idMes = Convert.ToInt32(gvMetaVendedor.DataKeys[row.RowIndex]["idMes"]);

                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR_META_COMERCIAL");
                    vParametros.Add("@idMeta", idMeta.ToString());
                    vParametros.Add("@idEmpresa", idEmpresa.ToString());
                    vParametros.Add("@idVendedor", idVendedor.ToString());
                    vParametros.Add("@nAno", row.Cells[5].Text);
                    vParametros.Add("@idMes",idMes.ToString());
                    vParametros.Add("@nValor", valor.ToString().Replace(".", "").Replace(",", "."));
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);                    
                                        
                }    
                catch (Exception ex)
                {
                    MensagemPaginaModal.MostraMensagem_Erro(ex.Message);
                }
            }
            
            LimpaCampos_Meta();
            ResetaModal();
            MensagemPaginaModal.MostraMensagem_Sucesso("Metas incluídas com sucesso!");
            
        }

        protected void gvMetaVendedor_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {                
                gvMetaVendedor.Columns[0].Visible = false;
                gvMetaVendedor.Columns[1].Visible = false;
                gvMetaVendedor.Columns[3].Visible = false;
                gvMetaVendedor.Columns[7].Visible = false;
                
            }   

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                cls_Meta_Vendedor meta = (cls_Meta_Vendedor)e.Row.DataItem;
                e.Row.Cells[3].Text = meta.sDscMes;

                TextBox txtValor = (TextBox)e.Row.FindControl("txtnValorComercial");
                txtValor.Text = meta.nValor.ToString();
            }
        }

        protected void gv_MetaComercial_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                cls_EmpresaMeta empresa = (cls_EmpresaMeta)e.Row.DataItem;

                GridView gvVendedorMensal = (GridView)e.Row.FindControl("gv_VendedorMensal");
                if (gvVendedorMensal != null)
                {
                    gvVendedorMensal.DataSource = empresa.lsMetasMensais;
                    gvVendedorMensal.DataBind();
                }
                e.Row.CssClass = "gvMainTd";
            }
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "gvMainTh";
        }

        protected void gv_VendedorMensal_RowDataBound(object sender, GridViewRowEventArgs e)
        {           

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                cls_Meta_Mensal metaMensal = (cls_Meta_Mensal)e.Row.DataItem;

                GridView gvVendedorDetalhe = (GridView)e.Row.FindControl("gv_VendedorDetalhe");
                if (gvVendedorDetalhe != null)
                {
                    gvVendedorDetalhe.DataSource = metaMensal.lsMetaVendedores;
                    gvVendedorDetalhe.DataBind();
                }
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = "gvMainTd";
            }
            else if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "gvChildHeader";
        }

        protected void gv_VendedorDetalhe_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = "gvMainTd";
            }
            else if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "gvChildHeader2";
        }

        protected void gv_VendedorDetalhe_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ExcluirVendedor")
            {
                int idMeta = Convert.ToInt32(e.CommandArgument);

                ExcluirMetaVendedor(idMeta);

                ConsultaMetaVendedor();
            }
            else if (e.CommandName == "EditarVendedor")
            {
                int idMeta = Convert.ToInt32(e.CommandArgument);

                var item = bs_Meta_Vendedor_Mensal.FirstOrDefault(x => x.idMeta == idMeta);
                string sAno = item.nAno.ToString();
                string sidVendedor = item.idVendedor.ToString();
                string idEmpresa = item.idEmpresa.ToString();

                PopulaModalMetaVendedor(sAno, sidVendedor, idEmpresa);                

                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalMetaVendedor", "$('#modalMetaVendedor').modal('show');", true);
            }
        }

        protected void btnNovaMetaVendedor_Click(object sender, EventArgs e)
        {
            ResetaModal();
            txtnAnoComercial.Text = DateTime.Now.Year.ToString();            
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalMetaVendedor", "$('#modalMetaVendedor').modal('show');", true);
        }

        public string NovaLinha(object id, string gridNome)
        {
            /* 
            * Passo a passo:
            * 1. Fecha a célula atual
            * 2. Fecha a linha Atual
            * 3. Cria uma nova linha com o ID e a classe <TR id='...' style='...'>
            * 4. Cria uma célula em branco: <TD></TD>
            * 5. Cria uma nova célula para conter o gridview
            ************************************************************/
            if (id != null && !string.IsNullOrEmpty(id.ToString()))
            {
                // Se houver um ID, retorna a nova linha com o ID e a classe
                return string.Format(@"</td></tr><tr id='tr{0}{1}' class='collapsed-row'>
                               <td></td><td colspan='100' style='padding:0px; margin:0px;'>", gridNome, id);
            }
            else
            {
                // Se não houver ID, retorna uma string vazia para que nada seja renderizado e o botão de colapso desapareça
                return string.Empty;
            }
        }

        protected void btnMetaComercialPesquisa_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        #endregion

        decimal ConverterStringDecimal(string dado)
        {
            decimal converter = 0;
            if (decimal.TryParse(dado, out converter))
            {
                converter = Convert.ToDecimal(dado);
                return converter;
            }
            return decimal.Zero;
        }

        void Popular_Aba_Historico(DataSet ds)
        {

        }

    }
}