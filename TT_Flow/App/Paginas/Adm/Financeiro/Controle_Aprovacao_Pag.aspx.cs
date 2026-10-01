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
using System.Globalization;
using TT_Flow.App.Controles;

using TT_Hub.App.Paginas.RRHH;
using TT_Flow;
using Identity = TT.FrameWork.Identity;


namespace TT_Flow.App.Paginas.Adm.Financeiro
{
    public partial class Controle_Aprovacao_Pag : System.Web.UI.Page
    {

        string sTituloPagina = "Controle Aprovacao Pagamentos";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_Contas_Pagar";        

        #region | Funções Incialização do Forms

        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual_de_Uso.pdf.pdf";
            if (!FUNCOES.ValidaPermissao(Permissao.Administracao.AprovacaoPagamento.Incluir))
            {
                BtnAprovar.Visible = false;
            }
            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                PopularCombos();
                ddlidAprovado.SelectedValue = "1";
                Pesquisar();
                if (Session["SalvoComSucesso"] != null && (bool)Session["SalvoComSucesso"])
                {
                    MensagemPagina.MostraMensagem_Sucesso("Titulos Aprovados com sucesso!");
                    Session["SalvoComSucesso"] = false;
                }
                if (Session["RejeitadoComSucesso"] != null && (bool)Session["RejeitadoComSucesso"])
                {
                    MensagemPagina.MostraMensagem_Sucesso("Titulos Rejeitados com sucesso!");
                    Session["RejeitadoComSucesso"] = false;
                }
                PesquisarValores();
                txtTotal.Text = "R$ 0,00";
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                var requestArgs = this.Request["__EVENTARGUMENT"];
                string eventArgument = Request["__EVENTARGUMENT"];
                if (requestTarget == "funcao_SAIR")
                {
                    FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                }
                else if (Request["__EVENTTARGET"] == "BtnAprovar")
                {
                    Aprovar(eventArgument); 
                }
                else if (Request["__EVENTTARGET"] == "BtnRejeitar")
                {
                    Rejeitar(eventArgument); 
                }

            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtPesquisa]').focus();", true);

            List<int> indexesToIgnore = new List<int> { 0, 14 };
            Grid.BotoesOcultarColuna(placeholderButtons, dtgAprovacao, this, indexesToIgnore);
            RegistraScript();
        }

        #endregion

        #region | Combos/DDL

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidAprovado, "sp_Select 'Flow_Adm_Aprovacao_Status'", "idAprovado", "sDscAprovacao", false, "Todos Status de Aprovação", "0");
            FUNCOES.Popula_Combo(ddlidFormaPagamento, "sp_Select 'tbl_Flow_CondicaodePagamento_Tipo'", "idTipoCondicaoPagamento", "sDscTipoCondicaoPagamento", false, "Selecione o Pagamento", "0");
            FUNCOES.Popula_Combo(ddlidCentroDeCusto, "sp_Select 'Flow_Adm_CentroDeCusto'", "idCentroDeCusto", "sDescricao", false, "Selecione um Centro de Custo", "0");
            FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'tbl_Flow_Empresa'", "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
            FUNCOES.Popula_Combo(ddlidCategoriaPagar, "sp_Select 'Flow_Adm_Contas_Pagar_Categoria'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Selecione a Categoria", "0");
            FUNCOES.Popula_Combo(ddlidCredor, "sp_Select 'Flow_Adm_Contas_Pagar_Clientes'", "idParceiro", "sRazaoSocial", false, "Selecione o Credor", "0");


        }

        #endregion

        #region |Metodos Banco de Dados

        protected void Pesquisar()
        {
            pnResultado.Visible = false;
            string sDscPesquisa = "";

            string sPermissao = "N";
            if (FUNCOES.ValidaPermissao(Permissao.Administracao.AprovacaoPagamento.VisualizarTudo))
            {
                sPermissao = "S";
            }
            else
            {
                sPermissao = "N";
            }

            string sUsuarioLogado = "";
            sUsuarioLogado = HttpContext.Current.Session["idUsuario"].ToString();

            try
            {
                sDscPesquisa = txtPesquisa.Text.Trim();
                DataTable dsPesquisa;
                string sSql = "sp_Manipula_tbl_Flow_Adm_Contas_Pagar";
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR");

                //vParametros.Add("@sUsuarioLogado", Identity.Variaveis.idUsuario());
                vParametros.Add("@sUsuarioLogado", sUsuarioLogado);
                vParametros.Add("@sPermissao", sPermissao);

                vParametros.Add("@idFormaPagamento", ddlidFormaPagamento.SelectedValue);
                vParametros.Add("@idCentroDeCusto", ddlidCentroDeCusto.SelectedValue);

                vParametros.Add("@sStatus", ddlsStatus.SelectedValue);
                vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                vParametros.Add("@sPesquisa", txtPesquisa.Text);
                vParametros.Add("@idParceiro", ddlidCredor.SelectedValue);
                vParametros.Add("@idCategoriaPagar", ddlidCategoriaPagar.SelectedValue);
                vParametros.Add("@idAprovado", ddlidAprovado.SelectedValue);
                vParametros.Add("@dtInicio", txtdtInicio.Text);
                vParametros.Add("@dtFinal", txtdtFinal.Text);
                vParametros.Add("@idDataPesquisa", ddlidDataPesquisa.SelectedValue);


                dsPesquisa = BD.ExecutarDataTable(sSql, vParametros, false);


                if (dsPesquisa.Rows.Count > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptDataPaging(dtgAprovacao, dsPesquisa, false, new int[2] { 3, 4 }, "desc", "true", "800"), true);
                    pnResultado.Visible = true;
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Nenhuma Pagamento Localizado");
                }
            }
            catch (Exception ex)
            {

                MensagemPagina.MostraMensagem_Erro("Erro ao Consultar: " + ex.Message);
            }

        }

        #endregion

        #region | Limpar Campos
        void LimpaCampos()
        {
            BtnAprovar.Text = "Aprovar";
            lblTituloPagina.Text = sTituloPagina;
        }
        #endregion

        #region | Validação 
        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (txtdtInicio.Text != "" || txtdtFinal.Text != "")
            {
                sMensagemErro += Validacoes.ValidaDatas(txtdtInicio.Text, txtdtFinal.Text);
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        #endregion

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            if (ValidarDados())
            {
                Pesquisar();
            }
        }

        protected void dtgAprovacao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                
                var idParceiro = DataBinder.Eval(e.Row.DataItem, "idParceiro")?.ToString();
                var idColaborador = DataBinder.Eval(e.Row.DataItem, "idColaborador")?.ToString();

                var isColaborador = !string.IsNullOrEmpty(idColaborador) && idColaborador != "0";

                HyperLink hlRazaoSocial = (HyperLink)e.Row.Cells[2].Controls[0];
                HyperLink hlDscColaborador = (HyperLink)e.Row.Cells[14].Controls[0];

                if (isColaborador && string.IsNullOrEmpty(hlRazaoSocial.Text))
                {
                    hlRazaoSocial.Text = hlDscColaborador.Text;
                }



                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
                string idAprovado = DataBinder.Eval(e.Row.DataItem, "idAprovado").ToString();

                if (idAprovado == "2")
                {
                    e.Row.Cells[0].Controls.Clear();
                }
                if (idAprovado == "3")
                {
                    e.Row.Cells[0].Controls.Clear();
                }

                CheckBox chk = (CheckBox)e.Row.FindControl("chkTitulo_Selecionado");
                if (chk != null)
                {
                    chk.Attributes["onclick"] = "calcularTotal();";
                }
                
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                CheckBox chkHeader = (CheckBox)e.Row.FindControl("chkTitulo_SelecionarTudo");
                if (chkHeader != null)
                {
                    chkHeader.Attributes["onclick"] = "selectAllCheckboxes(this);";
                }
            }
            GRID.EsconderColunas(e, 14);
        }

        private void Aprovar(string observacao)
        {
            try
            {
                foreach (GridViewRow item in dtgAprovacao.Rows)
                {
                    CheckBox chkTitulo_Selecionado_Linha = (CheckBox)item.FindControl("chkTitulo_Selecionado");
                    bool bSelecionado = chkTitulo_Selecionado_Linha.Checked;

                    if (bSelecionado)
                    {
                        int rowIndex = item.RowIndex;
                        string idContasPagar = dtgAprovacao.DataKeys[rowIndex]["idContasPagar"].ToString();

                        DataSet dsAprovar;
                        Dictionary<String, String> vParametros = new Dictionary<string, string>();

                        vParametros.Add("@sFuncao", "APROVAR");
                        vParametros.Add("@idContasPagar", idContasPagar);
                        vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                        vParametros.Add("@sObsAprovacao", txtsObsAprovacao.Text);

                        dsAprovar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", vParametros);

                    }
                }
                Session["SalvoComSucesso"] = true;
                Response.Redirect(Request.RawUrl);
                
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }

            Pesquisar();
        }

        private void Rejeitar(string observacao)
        {
            try
            {
                foreach (GridViewRow item in dtgAprovacao.Rows)
                {
                    CheckBox chkTitulo_Selecionado_Linha = (CheckBox)item.FindControl("chkTitulo_Selecionado");
                    bool bSelecionado = chkTitulo_Selecionado_Linha.Checked;

                    if (bSelecionado)
                    {
                        int rowIndex = item.RowIndex;
                        string idContasPagar = dtgAprovacao.DataKeys[rowIndex]["idContasPagar"].ToString();

                        DataSet dsAprovar;
                        Dictionary<String, String> vParametros = new Dictionary<string, string>();

                        vParametros.Add("@sFuncao", "REJEITAR");
                        vParametros.Add("@idContasPagar", idContasPagar);
                        vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                        vParametros.Add("@sObsAprovacao", txtsObsRejeicao.Text);

                        dsAprovar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", vParametros);

                    }

                }
                Session["RejeitadoComSucesso"] = true;
                Response.Redirect(Request.RawUrl);
                
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }

            Pesquisar();
        }

        //protected void btnSelecionarTodos_Click(object sender, EventArgs e)
        //{
        //    foreach (GridViewRow item in dtgAprovacao.Rows)
        //    {
        //        CheckBox chkTitulo_Selecionado_Linha = (CheckBox)item.FindControl("chkTitulo_Selecionado");
        //        chkTitulo_Selecionado_Linha.Checked = true;
        //    }
        //}
        
        protected void PesquisarValores()
        {
            txtnTotalAprovado.ReadOnly = true;
            txtnTotalRejeitado.ReadOnly = true;
            txtnTotalPendente.ReadOnly = true;
            txtnTotalGeral.ReadOnly = true;

            string sErro = "";
            try
            {
                string sPermissao = "N";
                if (FUNCOES.ValidaPermissao(Permissao.Administracao.AprovacaoPagamento.VisualizarTudo))
                {
                    sPermissao = "S";
                }
                else
                {
                    sPermissao = "N";
                }

                string sUsuarioLogado = "";
                sUsuarioLogado = HttpContext.Current.Session["idUsuario"].ToString();

                DataSet dsPesquisa;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR_VALORES");
                vParametros.Add("@sUsuarioLogado", sUsuarioLogado);
                vParametros.Add("@sPermissao", sPermissao);
                dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out sErro))
                {
                    txtnTotalAprovado.Text = RETORNO.DATASET(dsPesquisa, 0, "TotalAprovado");
                    txtnTotalRejeitado.Text = RETORNO.DATASET(dsPesquisa, 0, "TotalRejeitado");
                    txtnTotalPendente.Text = RETORNO.DATASET(dsPesquisa, 0, "TotalPendente");
                    txtnTotalGeral.Text = RETORNO.DATASET(dsPesquisa, 0, "TotalGeral");

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

        void RegistraScript()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            string gridClientID = dtgAprovacao.ClientID;
            string txtTotalClientID = txtTotal.ClientID;

            sb.AppendLine("function calcularTotal() {");
            sb.AppendLine("     var total = 0;");
            sb.AppendLine("     var grid = document.getElementById('" + gridClientID +"');");
            sb.AppendLine("     var checkboxes = grid.getElementsByTagName('input');");
            sb.AppendLine("     for (var i = 0; i < checkboxes.length; i++) {");
            sb.AppendLine("         if (checkboxes[i].type === 'checkbox' && checkboxes[i].id.indexOf('chkTitulo_Selecionado') !== -1) {");
            sb.AppendLine("             if (checkboxes[i].checked) {");
            sb.AppendLine("                 var row = checkboxes[i].closest('tr');");
            sb.AppendLine("                 var valorTexto = row.cells[9].getElementsByTagName('a')[0].innerText;");
            sb.AppendLine("                 var valorNumerico = parseFloat(valorTexto.replace('R$', '').replace(/\\./g,'').replace(',','.'));");
            sb.AppendLine("                 if (!isNaN(valorNumerico)) {");
            sb.AppendLine("                     total += valorNumerico;");
            sb.AppendLine("                 }");
            sb.AppendLine("             }");
            sb.AppendLine("         }");
            sb.AppendLine("     }");
            sb.AppendLine("     document.getElementById('" + txtTotalClientID + "').value = total.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });");
            sb.AppendLine("}");
            sb.AppendLine("function selectAllCheckboxes(headerCheckbox) {");
            sb.AppendLine("     var grid = document.getElementById('" + gridClientID + "');");
            sb.AppendLine("     var checkboxes = grid.getElementsByTagName('input');");
            sb.AppendLine("     for (var i = 0; i < checkboxes.length; i++) { ");
            sb.AppendLine("         if (checkboxes[i].type === 'checkbox' && checkboxes[i].id.indexOf('chkTitulo_Selecionado') !== -1) {");
            sb.AppendLine("             checkboxes[i].checked = headerCheckbox.checked;");
            sb.AppendLine("         }");
            sb.AppendLine("     }");
            sb.AppendLine("     calcularTotal();");
            sb.AppendLine("}");
            sb.AppendLine("function openModalAprovar() {");
            sb.AppendLine("     var total = document.getElementById('" + txtTotalClientID + "').value;");
            sb.AppendLine("     if (total === 'R$ 0,00') { ");
            sb.AppendLine("         alert('Selecione pelo menos um título para aprovar!');");
            sb.AppendLine("         return false;");
            sb.AppendLine("     }");
            sb.AppendLine("     var mensagem = 'Confirma a seleção dos títulos no valor de ' + total + ' aprovados para pagamento?';");
            sb.AppendLine("     document.querySelector('#modalAprovar .modal-message').innerText = mensagem;");
            sb.AppendLine("     $('#modalAprovar').modal('show');");
            sb.AppendLine("}");
            sb.AppendLine("function openModalRejeitar() {");
            sb.AppendLine("     var total = document.getElementById('" + txtTotalClientID + "').value;");
            sb.AppendLine("     if (total === 'R$ 0,00') {");
            sb.AppendLine("         alert('Selecione pelo menos um título para rejeitar!');");
            sb.AppendLine("         return false;");
            sb.AppendLine("     }");
            sb.AppendLine("     var mensagem = 'Confirme a seleção dos títulos no valor de ' + total + ' rejeitados para pagamento?';");
            sb.AppendLine("     document.querySelector('#modalRejeitar .modal-message').innerText = mensagem;");
            sb.AppendLine("     $('#modalRejeitar').modal('show');");
            sb.AppendLine("}");
         
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }

    }

}