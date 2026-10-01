using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;

namespace TT_Flow.App.Paginas.Comercial
{
    public partial class Tarefas_STSO : Page
    {
        string sTituloPagina = "STSO";

        #region | Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Comercial.Tarefas_STSO.STSO, true);
            Grid_Pedidos.Visible = FUNCOES.ValidaPermissao(Permissao.Comercial.Tarefas_STSO.Visualizar_Pedidos_STSO);

            manual.sNomeArquivo = "Manual-ArquivosSTSO.pdf";

            string DashBoard = "";

            if (!IsPostBack)
            {
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnSTSO.Visible = false;

                if (Request["DashBoard"] != null)
                    DashBoard = Request["DashBoard"];

                if (DashBoard != "")
                {
                    DIV_Filtro.Visible = false;
                    if (char.IsDigit(DashBoard[0]) && DashBoard.Length > 7)
                    {
                        string[] partes = DashBoard.Split('|');
                        lblTituloPagina.Text = sTituloPagina + " - " + partes[1];
                        BreadCrumb_Pagina.TitulodaPagina = sTituloPagina + " - " + partes[1];
                    }
                    else
                    {
                        lblTituloPagina.Text = sTituloPagina + " - " + DashBoard;
                        BreadCrumb_Pagina.TitulodaPagina = sTituloPagina + " - " + DashBoard;
                    }

                    PesquisarPedido(DashBoard);
                }
                else
                {
                    PopularCombos();
                    PesquisarPedido("");
                }
            }
            List<int> indexesToIgnore = new List<int> { };
            GRID.BotoesOcultarColuna(placeholderButtons, gvPedidos, this, indexesToIgnore);
        }
        #endregion

        #region | Pesquisar
        void PesquisarPedido(string DashBoard)
        {
            pnSTSO.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "PESQUISA_STSO" }
            };

            if (DashBoard == "")
            {
                vParametros.Add("@sPesquisa", txtPesquisa.Text);
                vParametros.Add("@StatusBalanco", ddlStatus.SelectedValue);
            }
            else
            {
                vParametros.Add("@sDashBoard", DashBoard);
                vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
            }

            DataTable tb = BD.ExecutarDataTable("sp_Consulta_tbl_Flow_Pedidos", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                pnSTSO.Visible = true;

                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", GRID.DataBindComScriptData(gvPedidos, tb, 0, new int[4] { 3, 5, 6, 7 }, "desc", "false", "''"), true);
            }
            else
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
        }
        #endregion

        #region | Popular Combos
        void PopularCombos()
        {

        }
        #endregion

        #region | gvPedidos
        protected void gvPedidos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCorSTSO").ToString();

                var dtPrevisao = e.Row.Cells[3].Text;
                var dtHistorico = e.Row.Cells[6].Text;
                var dtConclusao = e.Row.Cells[7].Text;
                var dtCriacao = e.Row.Cells[8].Text;
                var dtEnvio = e.Row.Cells[9].Text;
                var dtDownload = e.Row.Cells[10].Text;

                if (DateTime.TryParse(dtPrevisao, out DateTime previsao))
                {
                    if (dtPrevisao == "01/01/1900")
                    {
                        int columnIndex = 3;
                        e.Row.Cells[columnIndex].Text = "";
                    }
                }

                if (DateTime.TryParse(dtHistorico, out DateTime Inclusao))
                {
                    if (dtHistorico == "01/01/1900 00:00:00")
                    {
                        int columnIndex = 6;
                        e.Row.Cells[columnIndex].Text = "";
                    }
                }

                if (DateTime.TryParse(dtConclusao, out DateTime Conclusao))
                {
                    if (dtConclusao == "01/01/1900 00:00:00")
                    {
                        int columnIndex = 7;
                        e.Row.Cells[columnIndex].Text = "";
                    }
                }

                if (DateTime.TryParse(dtCriacao, out DateTime Criacao))
                {
                    if (dtCriacao == "01/01/1900 00:00:00")
                    {
                        int columnIndex = 8;
                        e.Row.Cells[columnIndex].Text = "";
                    }
                }

                if (DateTime.TryParse(dtEnvio, out DateTime Envio))
                {
                    if (dtEnvio == "01/01/1900 00:00:00")
                    {
                        int columnIndex = 9;
                        e.Row.Cells[columnIndex].Text = "";
                    }
                }

                if (DateTime.TryParse(dtDownload, out DateTime Download))
                {
                    if (dtDownload == "01/01/1900 00:00:00")
                    {
                        int columnIndex = 10;
                        e.Row.Cells[columnIndex].Text = "";
                    }
                }
            }
        }
        #endregion

        #region | cmdPesquisar
        protected void cmdPesquisar_Click(object sender, EventArgs e)
        =>
            PesquisarPedido("");
        #endregion
    }
}