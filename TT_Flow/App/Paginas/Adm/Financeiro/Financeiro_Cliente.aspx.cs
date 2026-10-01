using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using TT_Hub.App.Paginas.RRHH;
using TT_Flow.FrameWork;
using TT.FrameWork;
using TT_Flow;
using System.Text;
using System.Globalization;
using System.Data.Common.CommandTrees.ExpressionBuilder;
using System.Data.Common.CommandTrees;
using static TT.FrameWork.BD;
using TT_Flow.App.Controles;
using static iTextSharp.text.pdf.AcroFields;
using TT_Flow.App.Paginas.Manutencao;
using TT_Flow.App.Paginas.WMS.Manutencao;
using Org.BouncyCastle.Crypto;
using static Permissao.Financeiro;
using Identity = TT.FrameWork.Identity;


namespace TT_Flow.App.Paginas.Adm.Financeiro
{
    public partial class Financeiro_Cliente : System.Web.UI.Page
    {
        string sTituloPagina = "Financeiro";
        string sPagina_NovoRegistro = "app/Paginas/Adm/Financeiro/Financeiro_Cliente_Detalhe.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-Financeiro.pdf";

            FUNCOES.ValidaPermissao(Permissao.Financeiro.Financeiro_Cliente.Consultar, true);

            if (!IsPostBack)
            {
                    BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                    pnResultado.Visible = false;
                    Pesquisar("");
            }
            else
            {
                if (Session["MensagemErro"] != null)
                {
                    string mensagemErro = Session["MensagemErro"].ToString();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('" + mensagemErro + "');", true);
                    Session.Remove("MensagemErro");
                }
            }
            List<int> indexesToIgnore = new List<int> {17, 18 };
            Grid.BotoesOcultarColuna(placeholderButtons, dtgvConsulta, this, indexesToIgnore);
        }

        protected void Pesquisar(string DashBoard)
        {
            pnResultado.Visible = false;

            try
            {
                string sFuncao = "CONSULTAR";
                DataTable tb;
                string sSql = "sp_Manipula_tbl_Flow_Financeiro_Cliente";
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", sFuncao);

                if (DashBoard == "")
                {
                    vParametros.Add("@idUsuario", Identity.Variaveis.idUsuario());
                    vParametros.Add("@dtInicio", txtdtInicio.Text);
                    vParametros.Add("@dtFinal", txtdtFinal.Text);
                    vParametros.Add("@sStatus", ddlsStatus.SelectedValue);
                }
                tb = BD.ExecutarDataTable(sSql, vParametros, false);

                if (tb.Rows.Count > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb, 0, "desc"), true);
                    pnResultado.Visible = true;
                    GRID.SomarColunas(dtgvConsulta, true, GRID.Formatação.Moeda, 6, 7, 8);
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Nenhum Registro Localizado");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao Consultar: " + ex.Message);
            }
        }

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

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            if (ValidarDados())
            {
                Pesquisar("");
            }
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }
        }
    }
}