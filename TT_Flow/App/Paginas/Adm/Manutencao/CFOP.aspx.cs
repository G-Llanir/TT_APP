using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using System.Text;
using GRID = TT.FrameWork.Grid;
using RETORNO = TT.FrameWork.BD.Retorno;
using TT_Flow.FrameWork;
using System.Data.SqlClient;
using AjaxControlToolkit;
using System.Linq;

namespace TT_Flow.App.Paginas.Adm.Manutencao
{
    public partial class CFOP : System.Web.UI.Page
    {
        #region | Construtores

        public List<cls_CST> Base_CST
        {
            get
            {
                if (ViewState["Base_CST"] == null)
                {
                    ViewState["Base_CST"] = new List<FrameWork.cls_CST>();
                }
                return (List<FrameWork.cls_CST>)ViewState["Base_CST"];
            }

            set
            {
                ViewState["Base_CST"] = value;
            }
        }

        #endregion
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_CFOP";

        #region | Funções Incialização do Form

        protected void Page_Load(object sender, EventArgs e)
        {
            Pesquisar();

            if (IsPostBack)
            {
                var requestTarget = this.Request["__EVENTTARGET"];
            }            

            RegistraScript();
        }

        #endregion

        #region | Metodos Banco de Dados

        protected void Pesquisar()
        {
            FUNCOES.ValidaPermissao(Permissao.Administracao.CFOP.Consultar, true);
            lblTituloPagina.Text = "CFOP";

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sPesquisa", txtsPesquisa.Text},
                { "@tpNF", ddlTipo.SelectedValue},
                { "@sAtivo", ddlsAtivo.SelectedValue}
            };
            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa))
            {
                div_gvRelatorio.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", GRID.DataBindComScript(gvRelatorio, dsPesquisa), true);
            }
            else
            {
                div_gvRelatorio.Visible = false;
            }
        }
        #endregion

        #region | Script
        void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("$('[id*=txtsCFOP]').mask('0000', { reverse: true });");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }

        #endregion

        #region | Eventos

        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            PopulaCombo();
            txtidCFOP.Text = "NOVO";
            txtsCFOP.Text = "";
            txtsDescricao.Text = "";
            ddltpNF.SelectedValue = "";
            Verifica_CodigoBeneficio();
            sTotal.Definir("N", "Total", "N");
            sMovimentacao.Definir("N", "Movimenta Estoque", "N");
            sPagamento.Definir("N", "Pagamento", "N");
            sRecebimento.Definir("N", "Recebimento", "N");
            sDevolucao.Definir("N", "Devolução", "N");
            sFaturamento.Definir("N", "Exibir Faturamento", "N");
            sAtivo.Definir("S", "Ativo", "N");
            PainelAtualizacao.Visible = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModal", "$('#modal_Tipos').modal('show');", true);
        }

        protected void gvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                
                if (e.Row.Cells[9].Text == "S")
                    e.Row.Cells[9].Text = "Sim";
                else if (e.Row.Cells[9].Text == "N")
                    e.Row.Cells[9].Text = "Não";

                if (e.Row.Cells[10].Text == "S")
                    e.Row.Cells[10].Text = "Sim";
                else if (e.Row.Cells[10].Text == "N")
                    e.Row.Cells[10].Text = "Não";

                if (e.Row.Cells[11].Text == "S")
                    e.Row.Cells[11].Text = "Sim";
                else if (e.Row.Cells[11].Text == "N")
                    e.Row.Cells[11].Text = "Não";

                if (e.Row.Cells[12].Text == "S")
                    e.Row.Cells[12].Text = "Sim";
                else if (e.Row.Cells[12].Text == "N")
                    e.Row.Cells[12].Text = "Não";

                if (e.Row.Cells[13].Text == "S")
                    e.Row.Cells[13].Text = "Sim";
                else if (e.Row.Cells[13].Text == "N")
                    e.Row.Cells[13].Text = "Não";

                if (e.Row.Cells[14].Text == "S")
                    e.Row.Cells[14].Text = "Sim";
                else if (e.Row.Cells[14].Text == "N")
                    e.Row.Cells[14].Text = "Não";

            }
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidarTipo())
                {
                    string sErro = "";
                    string idCFOP = "";
                    string idcBenef = "0";

                    if (txtidCFOP.Text != "NOVO")
                        idCFOP = txtidCFOP.Text;

                    if (div_cBenef.Visible)
                        idcBenef = ddlidcBenef.SelectedValue;

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idCFOP", idCFOP},
                        { "@sDescricao", txtsDescricao.Text},
                        { "@sCFOP", txtsCFOP.Text},
                        { "@tpNF", ddltpNF.SelectedValue},
                        { "@idCSTICMS", ddlidCSTICMS.SelectedValue},
                        { "@idcBenef", idcBenef},
                        { "@idCSTIPI", ddlidCSTIPI.SelectedValue},
                        { "@idCSTPIS", ddlidCSTPIS.SelectedValue},
                        { "@idCSTCOFINS", ddlidCSTCOFINS.SelectedValue},
                        { "@sTotal", sTotal.Recuperar()},
                        { "@sMovimentacao", sMovimentacao.Recuperar()},
                        { "@sPagamento", sPagamento.Recuperar()},
                        { "@sRecebimento", sRecebimento.Recuperar()},
                        { "@sDevolucao", sDevolucao.Recuperar()},
                        { "@idUsuarioInclusao", IDENTITY.Variaveis.idUsuario()},
                        { "@sFaturamento", sFaturamento.Recuperar()},
                        { "@sAtivo", sAtivo.Recuperar()},

                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidCFOP.Value = RETORNO.DATASET(dsPesquisa, 0, "idCFOP");
                        PesquisarTipos(hddidCFOP.Value);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "$('#modal_Tipos').modal('show');", true);
                        MensagemPagina1.MostraMensagem_Sucesso("CFOP salvo com sucesso!");
                        Pesquisar();
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModal", "$('#modal_Tipos').modal('show');", true);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        void PesquisarTipos(string idCFOP)
        {
            string sErro = "";
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idCFOP", idCFOP}
            };
            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                PopulaCombo();

                txtsCFOP.Text = RETORNO.DATASET(dsPesquisa, 0, "sCFOP");
                txtsDescricao.Text = RETORNO.DATASET(dsPesquisa, 0, "sDescricao");
                txtidCFOP.Text = RETORNO.DATASET(dsPesquisa, 0, "idCFOP");
                ddltpNF.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "tpNF");
                ddlidCSTICMS.SelectedValue = ddlidCSTICMS.Items.FindByValue(RETORNO.DATASET(dsPesquisa, 0, "idCSTICMS")) != null ? RETORNO.DATASET(dsPesquisa, 0, "idCSTICMS") : "0";
                Verifica_CodigoBeneficio();
                if (div_cBenef.Visible)
                    ddlidcBenef.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idcBenef");
                ddlidCSTIPI.SelectedValue = ddlidCSTIPI.Items.FindByValue(RETORNO.DATASET(dsPesquisa, 0, "idCSTIPI")) != null ? RETORNO.DATASET(dsPesquisa, 0, "idCSTIPI") : "0";
                ddlidCSTPIS.SelectedValue = ddlidCSTPIS.Items.FindByValue(RETORNO.DATASET(dsPesquisa, 0, "idCSTPIS")) != null ? RETORNO.DATASET(dsPesquisa, 0, "idCSTPIS") : "0";
                ddlidCSTCOFINS.SelectedValue = ddlidCSTCOFINS.Items.FindByValue(RETORNO.DATASET(dsPesquisa, 0, "idCSTCOFINS")) != null ? RETORNO.DATASET(dsPesquisa, 0, "idCSTCOFINS") : "0";
                sTotal.Definir(RETORNO.DATASET(dsPesquisa, 0, "sTotal"), "Total", "N");
                sMovimentacao.Definir(RETORNO.DATASET(dsPesquisa, 0, "sMovimentacao"), "Movimentação", "N");
                sPagamento.Definir(RETORNO.DATASET(dsPesquisa, 0, "sPagamento"), "Pagamento", "N");
                sRecebimento.Definir(RETORNO.DATASET(dsPesquisa, 0, "sRecebimento"), "Recebimento", "N");
                sDevolucao.Definir(RETORNO.DATASET(dsPesquisa, 0, "sDevolucao"), "Devolução", "N");
                sFaturamento.Definir(RETORNO.DATASET(dsPesquisa, 0, "sFaturamento"), "Exibir Faturamento", "N");
                sAtivo.Definir(RETORNO.DATASET(dsPesquisa, 0, "sAtivo"), "Ativo", "N");
                PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                PainelAtualizacao.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModal", "$('#modal_Tipos').modal('show');", true);
            }
        }

        private bool ValidarTipo()
        {
            string sMensagem = "";

            if (ddltpNF.SelectedValue == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Informe se o CFOP é de ENTRADA ou Saída!";
            }
            if (txtsCFOP.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Informe o Código do CFOP!";
            }
            if (txtsDescricao.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Informe a Descrição!";
            }
            
            if (ddlidCSTICMS.SelectedValue == "0")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o CST para ICMS!";
            }

            if (div_cBenef.Visible && ddlidcBenef.SelectedValue == "0")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Código de Benefício!";
            }

            if (ddlidCSTIPI.SelectedValue == "0")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o CST para IPI!";
            }

            if (ddlidCSTPIS.SelectedValue == "0")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o CST para PIS!";
            }

            if (ddlidCSTCOFINS.SelectedValue == "0")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o CST para COFINS!";
            }



            if (sMensagem != "")
            {
                MensagemPagina1.MostraMensagem_Erro(sMensagem);
                return false;
            }
            else
            {
                return true;

            }
        }

        void Verifica_CodigoBeneficio()
        {
            div_cBenef.Visible = false;

            try
            {


                var Localiza = Base_CST.Where(c => c.idCST.ToString().Equals(ddlidCSTICMS.SelectedValue)).Select(x => new { x.sExige_cBenef }).ToList();
                if (Localiza[0].sExige_cBenef.ToString() == "S")
                {
                    div_cBenef.Visible = true;
                }
            }
            catch (Exception)
            {

                
            }
        }

        void PopulaCombo()
        {

            Base_CST = cls_CST.Popular();

            ddlidCSTICMS.DataSource = Base_CST.Where(c => c.sICMS.ToString().Equals("S")).Select(x => new { x.idCST, x.sDescricao }).ToList();
            ddlidCSTICMS.DataValueField = "idCST";
            ddlidCSTICMS.DataTextField = "sDescricao";
            ddlidCSTICMS.DataBind();

            ddlidCSTIPI.DataSource = Base_CST.Where(c => c.sIPI.ToString().Equals("S")).Select(x => new { x.idCST, x.sDescricao }).ToList();
            ddlidCSTIPI.DataValueField = "idCST";
            ddlidCSTIPI.DataTextField = "sDescricao";
            ddlidCSTIPI.DataBind();


            ddlidCSTPIS.DataSource = Base_CST.Where(c => c.sPIS.ToString().Equals("S")).Select(x => new { x.idCST, x.sDescricao }).ToList();
            ddlidCSTPIS.DataValueField = "idCST";
            ddlidCSTPIS.DataTextField = "sDescricao";
            ddlidCSTPIS.DataBind();

            ddlidCSTCOFINS.DataSource = Base_CST.Where(c => c.sCOFINS.ToString().Equals("S")).Select(x => new { x.idCST, x.sDescricao }).ToList();
            ddlidCSTCOFINS.DataValueField = "idCST";
            ddlidCSTCOFINS.DataTextField = "sDescricao";
            ddlidCSTCOFINS.DataBind();


            ddlidcBenef.DataSource = Base_CST.Where(c => c.sBENEF.ToString().Equals("S")).Select(x => new { x.idCST, x.sDescricao }).ToList();
            ddlidcBenef.DataValueField = "idCST";
            ddlidcBenef.DataTextField = "sDescricao";
            ddlidcBenef.DataBind();

            //.

            //FUNCOES.Popula_Combo(ddlidCSTICMS, "sp_Manipula_tbl_Flow_Adm_CFOP 'Consulta_CFOP_CST', @sPesquisa=ICMS", "idCST", "sCST", false, "Selecione CST ICMS", "0");
            //FUNCOES.Popula_Combo(ddlidCSTIPI, "sp_Manipula_tbl_Flow_Adm_CFOP 'Consulta_CFOP_CST', @sPesquisa=IPI", "idCST", "sCST", false, "Selecione CST IPI", "0");
            //FUNCOES.Popula_Combo(ddlidCSTPIS, "sp_Manipula_tbl_Flow_Adm_CFOP 'Consulta_CFOP_CST', @sPesquisa=PIS", "idCST", "sCST", false, "Selecione CST PIS", "0");
            //FUNCOES.Popula_Combo(ddlidCSTCOFINS, "sp_Manipula_tbl_Flow_Adm_CFOP 'Consulta_CFOP_CST', @sPesquisa=COFINS", "idCST", "sCST", false, "Selecione CST COFINS", "0");
        }

        protected void gvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Pesquisar")
            {
                string idCFOP = e.CommandArgument.ToString();
                PesquisarTipos(idCFOP);
            }
        }

        #endregion

        protected void ddlidCSTICMS_SelectedIndexChanged(object sender, EventArgs e)
        {
            Verifica_CodigoBeneficio();
        }
    }
}