using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static TT.FrameWork.BD;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Identity;
using static TT.FrameWork.Grid;

namespace TT_Flow.App.Paginas.Adm.Faturamento
{
    public partial class Faturamento : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-Faturamento.pdf";
            ValidaPermissao(Permissao.Financeiro.Faturamento.Consultar, true);

            if (int.TryParse(Request["idPedido"], out int idPedido) && idPedido > 0)
            {
                ddlFinalizado.SelectedValue = "";
                hddPedido_Filtro.Value = idPedido.ToString();
                pnFiltros.Visible = false;
            }

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = "Faturamento";

                PopularCombos();
                Pesquisar();
            }

            txtPesquisa.Focus();
        }

        protected void Pesquisar()
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "PESQUISA_FATURAMENTO" },
                    { "@sPesquisa", txtPesquisa.Text.Trim() },
                    { "@idPedido", hddPedido_Filtro.Value },
                    { "@idEmpresa", ddlidEmpresa.SelectedValue },
                    { "@sFinalizada", ddlFinalizado.SelectedValue },
                    { "@idFluxo", ddlFluxo.SelectedValue },
                    { "@dtInicio", !int.TryParse(Request["idPedido"], out int idPedido) || idPedido <= 0 ? RetornaAnosSelecionados() : "" },
                    { "@idCliente", ddlCliente.SelectedValue }
                };
                DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Faturamento", vParametros);

                ExibirGraficos(ds.Tables[1]);
                ExibirGraficosFluxo(ds.Tables[3]);

                if (ds.Tables[0].Rows.Count > 0) ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", DataBindComScript(gvOPI, ds.Tables[0], 0, "DESC"), true);
                else MensagemPagina_OPI.MostraMensagem_Erro("Nenhuma OPI localizada!");

                if (ds.Tables[2].Rows.Count > 0)
                {
                    gvMes.DataSource = ds.Tables[2];
                    gvMes.DataBind();

                    SomarColunas(gvMes, false, Formatação.Numero, typeof(HyperLink), 1, 2);
                }

                if (ds.Tables[4].Rows.Count > 0) ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Pedidos", DataBindComScript(gvPedidos, ds.Tables[4], 0, "DESC"), true);
                else MensagemPagina_Servicos.MostraMensagem_Erro("Nenhum Serviço para Faturamento localizado!");
            }
            catch (Exception ex)
            {
                MensagemPagina_OPI.MostraMensagem_Erro(ex.Message);
            }
        }

        protected void PopularCombos()
        {
            Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa'", "idEmpresa", "sDscEmpresaReduzida", false, "Todas as Empresas", "0");
            Popula_Combo(ddlFluxo, "sp_Select 'FLOW_Fluxo'", "idFluxo", "sDscFluxo", false, "Todos os Fluxos", "0");
            Popula_Combo(ddlCliente, "sp_Select 'Flow_Clientes_Pedido', " + Variaveis.idParceiro(), "idCliente", "sRazaoSocial", false, "Todos os Clientes", "0");

            DataSet ds = ExecutarDataSet("sp_Select", new Dictionary<string, string> { { "@sTabela", "Flow_Anos_Pedidos" } });

            lstAno.Items.Clear();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                lstAno.Items.Add(new ListItem(Retorno.DATASET(ds, 0, i, "Ano"), Retorno.DATASET(ds, 0, i, "Ano")));
                if (Retorno.DATASET(ds, 0, i, "Ano") == DateTime.Now.Year.ToString())
                    lstAno.Items[i].Selected = true;
            }
        }

        protected void ExibirGraficos(DataTable tb)
        {
            StringBuilder sb = new StringBuilder();

            if (tb.Rows.Count > 0)
            {
                StringBuilder labels = new StringBuilder();
                StringBuilder dataset = new StringBuilder();
                //                                       Azul                    Verde                 Vermelho                Amarelo
                string[] cores = new string[] { "rgba(0, 102, 204, 1)", "rgba(0, 153, 0, 1)", "rgba(204, 51, 51, 1)", "rgba(255, 255, 0, 1)" };

                foreach (DataColumn column in tb.Columns)
                {
                    decimal totalColumn = 0;

                    foreach (DataRow row in tb.Rows)
                    {
                        totalColumn += Convert.ToDecimal(row[column]);
                    }

                    if (column.ColumnName == "TotalPedidos") column.ColumnName = "Total Pedidos";
                    else if (column.ColumnName == "LiberadoFaturar") column.ColumnName = "Liberado Para Faturar";
                    else if (column.ColumnName == "Faturado") column.ColumnName = "Faturado";
                    else if (column.ColumnName == "afaturar") column.ColumnName = "A faturar";

                    labels.AppendFormat("'{0} {1}',", column.ColumnName, totalColumn.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR")));
                }

                if (labels.Length > 0) labels.Length--;

                dataset.AppendLine(" {");
                dataset.AppendLine("    data: [");

                foreach (DataColumn column in tb.Columns)
                {
                    decimal totalColumn = 0;
                    foreach (DataRow row in tb.Rows)
                    {
                        totalColumn += Convert.ToDecimal(row[column]);
                    }

                    string formattedValue = totalColumn.ToString("F2", CultureInfo.InvariantCulture);
                    dataset.AppendFormat("{0},", formattedValue);
                }

                if (dataset.ToString() != " {\r\n    data: [\r\n0.00,0.00,0.00,")
                {
                    DIV_GRAFICO.Visible = true;

                    if (dataset.Length > 0) dataset.Length--;

                    dataset.AppendLine("],");
                    dataset.AppendLine("    backgroundColor: [");

                    for (int i = 0; i < tb.Columns.Count; i++)
                    {
                        dataset.AppendFormat("'{0}',", cores[i % cores.Length]);
                    }

                    if (dataset.Length > 0) dataset.Length--;

                    dataset.AppendLine("],");
                    dataset.AppendLine("    borderColor: [");

                    for (int i = 0; i < tb.Columns.Count; i++)
                    {
                        dataset.AppendFormat("'{0}',", cores[i % cores.Length].Replace("0.2", "1"));
                    }

                    if (dataset.Length > 0) dataset.Length--;

                    string urlCompleta = HttpContext.Current.Request.Url.ToString();
                    string urlBase = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                    dataset.AppendLine("],");
                    dataset.AppendLine("    borderWidth: 1");
                    dataset.AppendLine("},");

                    sb.AppendLine("<script>");
                    sb.AppendLine("var ctx = document.getElementById('grafico_Faturamento').getContext('2d');");
                    sb.AppendLine("var graficoFaturamento = new Chart(ctx, {");
                    sb.AppendLine("    type: 'doughnut',");
                    sb.AppendLine("    data: {");
                    sb.AppendLine("        labels: [" + labels.ToString() + "],");
                    sb.AppendLine("        datasets: [" + dataset.ToString() + "]");
                    sb.AppendLine("    },");
                    sb.AppendLine("    options: {");
                    sb.AppendLine("        responsive: false,");
                    sb.AppendLine("        plugins: {");
                    sb.AppendLine("            title: {");
                    sb.AppendLine("                display: true,");
                    sb.AppendLine("            },");
                    sb.AppendLine("            legend: {");
                    sb.AppendLine("                position: 'right',");
                    sb.AppendLine("                align: 'center',");
                    sb.AppendLine("                labels: {");
                    sb.AppendLine("                    boxWidth: 20,");
                    sb.AppendLine("                    padding: 10");
                    sb.AppendLine("                }");
                    sb.AppendLine("            }");
                    sb.AppendLine("        },");
                    sb.AppendLine("        onClick: function(event, elements) {");
                    sb.AppendLine("            if (elements.length > 0) {");
                    sb.AppendLine("                var clickedElementIndex = elements[0].index;");
                    sb.AppendLine("                var clickedLabel = graficoFaturamento.data.labels[clickedElementIndex];");
                    sb.AppendLine("                var urlParams = new URLSearchParams(window.location.search);");
                    sb.AppendLine("                var dashboardParam = urlParams.get('dashboard');");
                    sb.AppendLine("                // Criar a nova URL com o valor do label, o parâmetro dashboard, e o Ano");
                    sb.AppendLine("                var newUrl = '" + urlBase + "/app/paginas/Pedidos.aspx?dashboard=' + encodeURIComponent(clickedLabel)  + '&Ano=' + encodeURIComponent('" + RetornaAnosSelecionados() + "') + '&idFluxo=' + '" + ddlFluxo.SelectedValue + "' + '&idEmpresa=' + '" + ddlidEmpresa.SelectedValue + "';");
                    sb.AppendLine("                // Redirecionar para a nova URL");
                    sb.AppendLine("                window.location.href = newUrl;");
                    sb.AppendLine("            }");
                    sb.AppendLine("        }");
                    sb.AppendLine("    }");
                    sb.AppendLine("});");
                    sb.AppendLine("</script>");

                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ChartScript", sb.ToString(), false);
                }
                else DIV_GRAFICO.Visible = false;
            }
            else DIV_GRAFICO.Visible = false;
        }

        protected void ExibirGraficosFluxo(DataTable tb)
        {
            StringBuilder sb = new StringBuilder();

            if (tb.Rows.Count > 0)
            {
                StringBuilder labels = new StringBuilder();
                StringBuilder dataset = new StringBuilder();

                string[] cores = new string[]
                {
                    "rgba(0, 102, 204, 1)",    // Azul
                    "rgba(0, 153, 0, 1)",      // Verde
                    "rgba(204, 51, 51, 1)",    // Vermelho
                    "rgba(255, 255, 0, 1)",    // Amarelo
                    "rgba(255, 165, 0, 1)",    // Laranja
                    "rgba(128, 0, 128, 1)",    // Roxo
                    "rgba(0, 255, 255, 1)",    // Ciano
                    "rgba(255, 0, 255, 1)",    // Magenta
                    "rgba(255, 0, 0, 1)",      // Vermelho puro
                    "rgba(0, 255, 0, 1)",      // Verde puro
                    "rgba(0, 0, 255, 1)",      // Azul puro
                    "rgba(255, 105, 180, 1)",  // Rosa
                    "rgba(75, 0, 130, 1)",     // Índigo
                    "rgba(255, 20, 147, 1)",   // DeepPink
                    "rgba(255, 255, 255, 1)",  // Branco
                    "rgba(0, 0, 0, 1)",        // Preto
                    "rgba(169, 169, 169, 1)",  // Cinza
                    "rgba(255, 222, 173, 1)",  // NavajoWhite
                    "rgba(255, 239, 0, 1)",    // Lemon
                };

                foreach (DataRow row in tb.Rows)
                {
                    labels.AppendFormat("'{0}', ", row[0].ToString() + " " + row[1].ToString());
                }

                if (labels.Length > 0) labels.Length--;

                dataset.AppendLine(" {");
                dataset.AppendLine("    data: [");

                foreach (DataRow row in tb.Rows)
                {
                    dataset.AppendFormat("{0},", row[1].ToString());
                }

                if (dataset.ToString() != " {\r\n    data: [\r\n0.00,0.00,0.00,")
                {
                    DIV3.Visible = true;

                    if (dataset.Length > 0) dataset.Length--;

                    dataset.AppendLine("],");
                    dataset.AppendLine("    backgroundColor: [");

                    for (int i = 0; i < tb.Rows.Count; i++)
                    {
                        dataset.AppendFormat("'{0}',", cores[i % cores.Length]);
                    }

                    if (dataset.Length > 0) dataset.Length--;

                    dataset.AppendLine("],");
                    dataset.AppendLine("    borderColor: [");

                    for (int i = 0; i < tb.Rows.Count; i++)
                    {
                        dataset.AppendFormat("'{0}',", cores[i % cores.Length].Replace("0.2", "1"));
                    }

                    if (dataset.Length > 0) dataset.Length--;

                    string urlBase = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                    dataset.AppendLine("],");
                    dataset.AppendLine("    borderWidth: 1");
                    dataset.AppendLine("},");

                    sb.AppendLine("<script>");
                    sb.AppendLine("var ctx = document.getElementById('Faturamento_Tipos').getContext('2d');");
                    sb.AppendLine("var Faturamento_Tipos = new Chart(ctx, {");
                    sb.AppendLine("    type: 'doughnut',");
                    sb.AppendLine("    data: {");
                    sb.AppendLine("        labels: [" + labels.ToString() + "],");
                    sb.AppendLine("        datasets: [" + dataset.ToString() + "]");
                    sb.AppendLine("    },");
                    sb.AppendLine("    options: {");
                    sb.AppendLine("        responsive: false,");
                    sb.AppendLine("        plugins: {");
                    sb.AppendLine("            title: {");
                    sb.AppendLine("                display: true,");
                    sb.AppendLine("            },");
                    sb.AppendLine("            legend: {");
                    sb.AppendLine("                position: 'right',");
                    sb.AppendLine("                align: 'center',");
                    sb.AppendLine("                labels: {");
                    sb.AppendLine("                    boxWidth: 20,");
                    sb.AppendLine("                    padding: 10");
                    sb.AppendLine("                }");
                    sb.AppendLine("            }");
                    sb.AppendLine("        },");
                    sb.AppendLine("        onClick: function(event, elements) {");
                    sb.AppendLine("            if (elements.length > 0) {");
                    sb.AppendLine("                var clickedElementIndex = elements[0].index;");
                    sb.AppendLine("                var clickedLabel = Faturamento_Tipos.data.labels[clickedElementIndex];");
                    sb.AppendLine("                var urlParams = new URLSearchParams(window.location.search);");
                    sb.AppendLine("                var dashboardParam = urlParams.get('dashboard');");
                    sb.AppendLine("                // Criar a nova URL com o valor do label, o parâmetro dashboard, e o Ano");
                    sb.AppendLine("                var newUrl = '" + urlBase + "/app/paginas/Pedidos.aspx?Fluxo=' + encodeURIComponent(clickedLabel)  + '&Ano=' + encodeURIComponent('" + RetornaAnosSelecionados() + "')+ '&idFluxo=' + '" + ddlFluxo.SelectedValue + "' + '&idEmpresa=' + '" + ddlidEmpresa.SelectedValue + "';");
                    sb.AppendLine("                // Redirecionar para a nova URL");
                    sb.AppendLine("                window.location.href = newUrl;");
                    sb.AppendLine("            }");
                    sb.AppendLine("        }");
                    sb.AppendLine("    }");
                    sb.AppendLine("});");
                    sb.AppendLine("</script>");

                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "FluxoChartScript", sb.ToString(), false);
                }
                else DIV3.Visible = false;
            }
            else DIV3.Visible = false;
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void gvMes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string selectedYear = RetornaAnosSelecionados();
                string url = $"&ano={selectedYear}&Faturamento";

                (e.Row.Cells[1].Controls[0] as HyperLink).NavigateUrl += $"{url}=S";
                (e.Row.Cells[2].Controls[0] as HyperLink).NavigateUrl += $"{url}=N";

                e.Row.Cells[0].Text += $" {selectedYear.Replace(",", " / ")}";
            }
        }

        protected void gvOPI_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[13].Text = e.Row.Cells[13].Text.Equals("S") ? "✔" : "";
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }
        }

        protected void gvPedidos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[12].Text = e.Row.Cells[12].Text.Equals("S") ? "✔" : "";
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }
        }

        protected string RetornaAnosSelecionados()
        {
            var anosSelecionados = lstAno.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value);

            if (!anosSelecionados.Any())
            {
                string anoAtual = DateTime.Now.Year.ToString();

                lstAno.Items.FindByValue(anoAtual).Selected = true;
                anosSelecionados = new[] { anoAtual };
            }

            return string.Join(",", anosSelecionados);
        }

        public string GetCorPorIndicador(string indicador)
        {
            int hash = indicador.GetHashCode();

            byte red = (byte)((hash & 0xFF0000) >> 16);
            byte green = (byte)((hash & 0x00FF00) >> 8);
            byte blue = (byte)(hash & 0x0000FF);

            red = (byte)(red / 2 + 64);
            green = (byte)(green / 2 + 64);
            blue = (byte)(blue / 2 + 64);

            return $"#{red:X2}{green:X2}{blue:X2}";
        }
    }
}