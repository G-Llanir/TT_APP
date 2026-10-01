using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;

namespace TT_Flow.App.Paginas.COMEX.Manutencao
{
    public partial class Resultados : Page
    {
        string sTituloPagina = "Resultado";

        #region | Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Comex.Resultado, true);
            Grid_Pedidos.Visible = FUNCOES.ValidaPermissao(Permissao.Comex.VisualizarPedidosResultado);

            manual.sNomeArquivo = "Manual-Resultados.pdf";

            hddidTipo.Value = "3";
            string DashBoard = "";

            if (Request["sTp"] != null)
                hddidTipo.Value = Request["sTp"].ToString();
            else
                hddidTipo.Value = "3";

            if (hddidTipo.Value == "3")
            {
                sTituloPagina = "Resultados";
                gvPedidos.Columns[0].HeaderText = "ID";
            }
            else
            {
                DIV_Filtro.Visible = false;
                MensagemPagina.MostraMensagem_Erro("Tipo Inválido!");
            }

            if (!IsPostBack)
            {
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;

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

                    if (hddidTipo.Value == "3")
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
            pnResultado.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "PESQUISA_RESULTADO" },
                { "@idTipo", hddidTipo.Value }
            };

            if (DashBoard == "")
            {
                vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());

                if (hddidTipo.Value == "3")
                {
                    vParametros.Add("@Importacao_idFornecedor", ddlCliente.SelectedValue.ToString());
                    vParametros.Add("@Importacao_idDespachante", ddlImportacao_idDespachante.SelectedValue.ToString());
                    vParametros.Add("@Importacao_idImportador", ddlImportador.SelectedValue);
                    vParametros.Add("@StatusBalanco", ddlFinanceiro.SelectedValue);
                    vParametros.Add("@dtInicio", txtdtInicio.Text);
                    vParametros.Add("@dtFinal", txtdtFinal.Text);
                }
                vParametros.Add("@sPesquisa", txtPesquisa.Text);
            }
            else
            {
                vParametros.Add("@sDashBoard", DashBoard);
                vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
            }

            DataTable tb = BD.ExecutarDataTable("sp_Consulta_tbl_Flow_Pedidos", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                pnResultado.Visible = true;

                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", GRID.DataBindComScriptData(gvPedidos, tb, 0, new int[2] { 1, 13 }, "desc", "false", "''"), true);

                GRID.SomarColunas(gvPedidos, false, GRID.Formatação.Numero, 7, 8, 9, 10);

                decimal somar = 0;

                if (gvPedidos.FooterRow.Cells[6].Text != null && gvPedidos.FooterRow.Cells[6].Text != "&nbsp;")
                    somar = decimal.Parse(gvPedidos.FooterRow.Cells[6].Text.Length > 0 ? gvPedidos.FooterRow.Cells[6].Text : "0");

                foreach (GridViewRow row in gvPedidos.Rows)
                {
                    try
                    {
                        somar += decimal.Parse(row.Cells[6].Text.Split(' ')[0]);
                    }
                    catch
                    {

                    }
                }
                gvPedidos.FooterRow.Cells[6].Text = somar.ToString("N2");

            }
            else
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
        }
        #endregion

        #region | Popular Combos
        void PopularCombos()
        {
            if (hddidTipo.Value == "3")
            {
                FUNCOES.Popula_Combo(ddlCliente, "sp_Select 'Flow_COMEX_Fornecedor'", "idParceiro", "sRazaoSocial", false, "Todos os Exportadores", "0");
                FUNCOES.Popula_Combo(ddlImportacao_idDespachante, "sp_Select 'Flow_Parceiro_Importacao_Despachante'", "idParceiro", "sRazaoSocial", false, "Todos os Despachantes", "0");
                FUNCOES.Popula_Combo(ddlImportacao_idDespachante, "sp_Select 'Flow_Parceiro_Importacao_Despachante'", "idParceiro", "sRazaoSocial", false, "Todos os Despachantes", "0");
                FUNCOES.Popula_Combo(ddlImportador, "sp_Select 'Flow_Parceiro_Importacao_Importador'", "idParceiro", "sRazaoSocial", false, "Todos os Importadores", "0");
            }
        }
        #endregion

        #region | gvPedidos
        protected void gvPedidos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCorStatus").ToString();

                var dtPrevisao = e.Row.Cells[13].Text;

                if (DateTime.TryParse(dtPrevisao, out DateTime previsao))
                {

                    if (dtPrevisao == "01/01/1900")
                    {
                        int columnIndex = 13;
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