using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using TT_Flow.App.Paginas.Adm.Financeiro;
using TT_Flow.FrameWork;
using static TT.FrameWork.BD;
using Identity = TT_Flow.FrameWork.Identity;

namespace TT_Flow.App.Paginas.Comercial
{
    public partial class DashboardComercial : System.Web.UI.Page
    {
        string sTituloPagina = "Dashboard Comercial";
        string sProcedure = "sp_Flow_DashBoard_Comercial";
        int tab_orcamentos_geral = 0;
        int tab_pedidos_geral = 1;
        int grafico_pedidos_meta_vendedor = 2;
        int tab_grafico_faturamento = 3;
        int tab_faturamento_fluxo = 4;
        int tab_pedidos_clientes = 5;
        int tab_pedidos_fluxo = 6;
        int tab_pedidos_vendedor = 7;

        public List<cls_FiltroInicial> bs_FiltroInicial
        {
            get
            {
                if (ViewState["bs_FiltroInicial"] == null)
                {
                    ViewState["bs_FiltroInicial"] = new List<cls_FiltroInicial>();
                }
                return (List<cls_FiltroInicial>)ViewState["bs_FiltroInicial"];
            }
            set
            {
                ViewState["bs_FiltroInicial"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Funcoes.ValidaPermissao(Permissao.Comercial.Dashboard_Comercial.Consultar, true);

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;

                //Manual Usuario
                //manual.sNomeArquivo = "Manual_Dashboard.pdf";

                Funcoes.Popula_Combo(lstVendedor, "sp_Select 'FLOW_Vendedores_Ativos'", "idVendedor", "sDscUsuario", false);
                Funcoes.Popula_Combo(lstFluxo, "sp_Select 'tbl_Flow_Fluxo'", "idFluxo", "sDscFluxo", false);
                Funcoes.Popula_Combo(lstEmpresa, "sp_Select 'Flow_Empresa'", "idEmpresa", "sDscCodigoEmpresa", false);

                DateTime dtInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                DateTime dtFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));

                txtdtInicio.Text = dtInicio.ToString("yyyy-MM-dd");
                txtdtFinal.Text = dtFinal.ToString("yyyy-MM-dd");

                if (!Funcoes.ValidaPermissao(Permissao.Comercial.Dashboard_Comercial.Todos_Vendedores))
                {
                    try
                    {
                        lstVendedor.SelectedValue = ConsultaVendedor();
                        lstVendedor.Attributes.Add("disabled", "disabled");                        

                    }
                    catch (Exception ex)
                    {
                        MensagemPagina.MostraMensagem_Erro("Erro Consultar Vendedor:" + ex.ToString());
                    }

                }

                ConsultarDados();

            }
        }

        private void ConsultarDados()
        {
            bs_FiltroInicial.Clear();
            div_graficoOrcamentosGeral.Visible = false;            
            //div_graficoPedidosGeral.Visible = false;
            div_graficoPedidosClientes.Visible = false;
            div_graficoPedidosFluxo.Visible = false;
            div_graficoPedidosMetaVendedor.Visible = false;
            div_graficoFaturamentoGeral.Visible = false;
            div_graficoFaturamentoFluxo.Visible = false;
            div_graficoPedidosVendedor.Visible = false;

            try
            {
                DataSet dsDashBoard;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "POPULA_GRAFICO");
                vParametros.Add("@dtInicio", txtdtInicio.Text);
                vParametros.Add("@dtFinal", txtdtFinal.Text);
                vParametros.Add("@idVendedor", string.Join("|", lstVendedor.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value)));
                vParametros.Add("@idFluxo", string.Join("|", lstFluxo.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value)));
                vParametros.Add("@idEmpresa", string.Join("|", lstEmpresa.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value)));

                dsDashBoard = BD.ExecutarDataSet(sProcedure, vParametros);

                cls_FiltroInicial filtro = new cls_FiltroInicial();
                filtro.dtInicio = txtdtInicio.Text;
                filtro.dtFinal = txtdtFinal.Text;
                filtro.sIdVendedor = string.Join("|", lstVendedor.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value));
                filtro.sIdFluxo = string.Join("|", lstFluxo.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value));
                filtro.sIdEmpresa = string.Join("|", lstEmpresa.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value));

                bs_FiltroInicial.Add(filtro);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<script>");                

                if (dsDashBoard.Tables[tab_orcamentos_geral].Rows.Count > 0)
                {
                    div_graficoOrcamentosGeral.Visible = true;                    
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables_Orcamento", TT.FrameWork.Grid.DataBindComScriptPaging(gvOrcamentoGeral, dsDashBoard.Tables[tab_orcamentos_geral], false, false, false, -1, ""), true);
                    Grid.SomarColunas(gvOrcamentoGeral, false, Grid.Formatação.Moeda, 2);
                }

                //if (dsDashBoard.Tables[tab_pedidos_geral].Rows.Count > 0)
                //{
                //    div_graficoPedidosGeral.Visible = true;
                //    gvPedidosGeral.DataSource = dsDashBoard.Tables[tab_pedidos_geral];
                //    gvPedidosGeral.DataBind();
                //}

                if (dsDashBoard.Tables[tab_pedidos_clientes].Rows.Count > 0)
                {
                    div_graficoPedidosClientes.Visible = true;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables_PedidoCliente", TT.FrameWork.Grid.DataBindComScriptPaging(gvPedidosClientes, dsDashBoard.Tables[tab_pedidos_clientes], false, false, false, 1, "desc"), true);
                    Grid.SomarColunas(gvPedidosClientes, false, Grid.Formatação.Moeda, 2, 3, 4);
                }

                if (dsDashBoard.Tables.Count > tab_pedidos_fluxo && dsDashBoard.Tables[tab_pedidos_fluxo].Rows.Count > 0)
                {
                    div_graficoPedidosFluxo.Visible = true;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables_PedidoFluxo", TT.FrameWork.Grid.DataBindComScriptPaging(gvPedidosFluxo, dsDashBoard.Tables[tab_pedidos_fluxo], false, false, false, 0, "asc"), true);
                    Grid.SomarColunas(gvPedidosFluxo, false, Grid.Formatação.Moeda, 2, 3, 4);
                }

                if (dsDashBoard.Tables[grafico_pedidos_meta_vendedor].Rows.Count > 0)
                {
                    div_graficoPedidosMetaVendedor.Visible = true;
                    var dadosPorMes = new Dictionary<string, List<cls_VendedorData>>();

                    foreach (DataRow row in dsDashBoard.Tables[grafico_pedidos_meta_vendedor].Rows)
                    {
                        string mes = row["sMes"].ToString();
                        string vendedor = row["sDscUsuario"].ToString();
                        decimal meta = Convert.ToDecimal(row["nValor"]);
                        decimal realizado = Convert.ToDecimal(row["nValorTotalPedido"]);

                        if (!dadosPorMes.ContainsKey(mes))
                            dadosPorMes[mes] = new List<cls_VendedorData>();

                        dadosPorMes[mes].Add(new cls_VendedorData
                        {
                            Vendedor = vendedor,
                            Meta = meta,
                            Realizado = realizado
                        });
                    }

                    string[] ordemMeses = { "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };
                    var mesesOrdenados = dadosPorMes.Keys.OrderBy(mes => Array.IndexOf(ordemMeses, mes)).ToList();

                    var labels = new List<string>();
                    var metas = new List<decimal>();
                    var realizados = new List<decimal>();

                    foreach (string mes in mesesOrdenados)
                    {
                        foreach (var vendedorData in dadosPorMes[mes])
                        {
                            labels.Add($"{mes} - {vendedorData.Vendedor}");
                            metas.Add(vendedorData.Meta);
                            realizados.Add(vendedorData.Realizado);
                        }
                    }

                    string jsonLabels = JsonConvert.SerializeObject(labels);
                    string jsonMetas = JsonConvert.SerializeObject(metas);
                    string jsonRealizados = JsonConvert.SerializeObject(realizados);
                
                    sb.AppendLine("document.addEventListener('DOMContentLoaded', function () {");
                    sb.AppendLine("     var ctx = document.getElementById('grafico_pedidos_meta_vendedor').getContext('2d');");
                    sb.AppendLine("     new Chart(ctx, {");
                    sb.AppendLine("         type: 'bar', ");
                    sb.AppendLine("         data: { ");
                    sb.AppendLine($"             labels: {jsonLabels},");
                    sb.AppendLine("              datasets: [ ");
                    sb.AppendLine("                 {");
                    sb.AppendLine("                     label: 'Meta',");
                    sb.AppendLine($"                    data: {jsonMetas},");
                    sb.AppendLine("                     backgroundColor: 'rgba(54, 162, 235, 0.7)',");
                    sb.AppendLine("                     borderColor: 'rgba(54, 162, 235, 1)',");
                    sb.AppendLine("                     borderWidth: 1");
                    sb.AppendLine("                 },");
                    sb.AppendLine("                 {");
                    sb.AppendLine("                     label: 'Realizado',");
                    sb.AppendLine($"                     data: {jsonRealizados},");
                    sb.AppendLine("                     backgroundColor: 'rgba(255, 99, 132, 0.7)',");
                    sb.AppendLine("                     borderColor:'rgba(255, 99, 132, 1)',");
                    sb.AppendLine("                     borderWidth: 1");
                    sb.AppendLine("                 }");
                    sb.AppendLine("             ]");
                    sb.AppendLine("         },");
                    sb.AppendLine("         options: {");
                    sb.AppendLine("             responsive: true,");
                    sb.AppendLine("             maintainAspectRatio: false,");
                    sb.AppendLine("             plugins: {");
                    sb.AppendLine("                 title: {");
                    sb.AppendLine("                     display: true,");
                    sb.AppendLine("                     text: 'Metas Vendedores x Pedidos'");
                    sb.AppendLine("                 },");
                    sb.AppendLine("                 legend: {");
                    sb.AppendLine("                     position: 'top'");
                    sb.AppendLine("                 },");
                    sb.AppendLine("             },");
                    sb.AppendLine("             scales: {");
                    sb.AppendLine("                 x: {");
                    sb.AppendLine("                     stacked: false,");
                    sb.AppendLine("                     ticks: {");
                    sb.AppendLine("                         autoSkip: false,");
                    sb.AppendLine("                         maxRotation: 45,");
                    sb.AppendLine("                         minRotation: 45");
                    sb.AppendLine("                     }");
                    sb.AppendLine("                },");
                    sb.AppendLine("                y: {");
                    sb.AppendLine("                     beginAtZero: true,");
                    sb.AppendLine("                     ticks: {");
                    sb.AppendLine("                         callback: function (value) {");
                    sb.AppendLine("                             return 'R$ ' + value.toLocaleString('pt-BR', { minimunFractionDigits: 2 });");
                    sb.AppendLine("                         }");
                    sb.AppendLine("                     }");
                    sb.AppendLine("                 }");
                    sb.AppendLine("             }");
                    sb.AppendLine("         }");
                    sb.AppendLine("     });");
                    sb.AppendLine(" });");

                }

                if (dsDashBoard.Tables[tab_grafico_faturamento].Rows.Count > 0)
                {
                    div_graficoFaturamentoGeral.Visible = true;

                    ViewState["DadosFaturamento"] = dsDashBoard.Tables[tab_grafico_faturamento];

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables_Faturamento", TT.FrameWork.Grid.DataBindComScriptPaging(gvFaturamento, dsDashBoard.Tables[tab_grafico_faturamento], false, false, false, 0, "asc"), true);

                    var meses = new List<string>();
                    var valoresPrevisao = new List<decimal>();
                    var valoresFaturado = new List<decimal>();

                    foreach(DataRow row in dsDashBoard.Tables[tab_grafico_faturamento].Rows)
                    {
                        meses.Add(row["sMes"].ToString());
                        valoresPrevisao.Add(Convert.ToDecimal(row["nValor"]));
                        valoresFaturado.Add(Convert.ToDecimal(row["nValorFaturado"]));
                    }

                    string jsonMeses = JsonConvert.SerializeObject(meses);
                    string jsonPrevisao = JsonConvert.SerializeObject(valoresPrevisao);
                    string jsonFaturado = JsonConvert.SerializeObject(valoresFaturado);

                    sb.AppendLine("document.addEventListener('DOMContentLoaded', function () {");
                    sb.AppendLine("     var ctx = document.getElementById('grafico_faturamento_geral').getContext('2d');");
                    sb.AppendLine("     new Chart (ctx, {");
                    sb.AppendLine("         type: 'bar',");
                    sb.AppendLine("         data: {");
                    sb.AppendLine($"            labels: {jsonMeses},");
                    sb.AppendLine("             datasets: [{");
                    sb.AppendLine("                 label: 'Previsto',");
                    sb.AppendLine($"                data: {jsonPrevisao},");
                    sb.AppendLine("                 backgroundColor: 'rgba(54, 162, 235, 0.7)',");
                    sb.AppendLine("                 borderColor: 'rgba(54, 162, 235, 1)',");
                    sb.AppendLine("                 borderWidth: 1");
                    sb.AppendLine("             },");
                    sb.AppendLine("             {");
                    sb.AppendLine("                 label: 'Faturado',");
                    sb.AppendLine($"                data: {jsonFaturado},");
                    sb.AppendLine("                 backgroundColor: 'rgba(75, 192, 192, 0.7)',");
                    sb.AppendLine("                 borderColor: 'rgba(75, 192, 192, 1)',");
                    sb.AppendLine("                 borderWidth: 1");
                    sb.AppendLine("             }]");
                    sb.AppendLine("         },");
                    sb.AppendLine("         options: {");
                    sb.AppendLine("             responsive: true,");
                    sb.AppendLine("             maintainAspectRatio: false,");
                    sb.AppendLine("             plugins: {");
                    sb.AppendLine("                 title: {");
                    sb.AppendLine("                     display: true,");
                    sb.AppendLine("                     text: 'Previsão x Faturamento por Mês'");
                    sb.AppendLine("                 },");
                    sb.AppendLine("                 legend: {");
                    sb.AppendLine("                     position: 'top'");
                    sb.AppendLine("                 }");
                    sb.AppendLine("             },");
                    sb.AppendLine("             scales: {");
                    sb.AppendLine("                 x: {");
                    sb.AppendLine("                     stacked: false,");
                    sb.AppendLine("                     ticks: {");
                    sb.AppendLine("                         autoSkip: false,");
                    sb.AppendLine("                         maxRotation: 45,");
                    sb.AppendLine("                         minRotation: 45");
                    sb.AppendLine("                     }");
                    sb.AppendLine("                 },");
                    sb.AppendLine("                 y: {");
                    sb.AppendLine("                     beginAtZero: true,");
                    sb.AppendLine("                     ticks: {");
                    sb.AppendLine("                         callback: function (value) {");
                    sb.AppendLine("                             return 'R$ ' + value.toLocaleString('pt-BR', { minimumFractionDigits: 2 });");
                    sb.AppendLine("                         }");
                    sb.AppendLine("                     }");
                    sb.AppendLine("                 }");
                    sb.AppendLine("             }");
                    sb.AppendLine("         }");
                    sb.AppendLine("     });");
                    sb.AppendLine(" });");

                }

                if (dsDashBoard.Tables.Count > tab_faturamento_fluxo && dsDashBoard.Tables[tab_faturamento_fluxo].Rows.Count > 0)
                {
                    div_graficoFaturamentoFluxo.Visible = true;

                    gvFaturamentoFluxo.Columns.Clear();

                    BoundField colMes = new BoundField
                    {
                        DataField = "sMes",
                        HeaderText = "Mês",
                        ItemStyle = { Width = Unit.Percentage(10) }
                    };
                    gvFaturamentoFluxo.Columns.Add(colMes);

                    foreach (DataColumn col in dsDashBoard.Tables[tab_faturamento_fluxo].Columns)
                    {
                        if ( col.ColumnName != "nMes" && col.ColumnName != "sMes")
                        {
                            string sColunaAlterada = col.ColumnName.Replace("_", " ");
                            BoundField colFluxo = new BoundField
                            {
                                DataField = col.ColumnName, 
                                HeaderText = sColunaAlterada,
                                DataFormatString = "{0:N2}",
                                ItemStyle = { HorizontalAlign = HorizontalAlign.Left }
                            };
                            gvFaturamentoFluxo.Columns.Add(colFluxo);
                        }
                    }

                    ViewState["DadosFaturamentoFluxo"] = dsDashBoard.Tables[tab_faturamento_fluxo];
                    gvFaturamentoFluxo.DataSource = dsDashBoard.Tables[tab_faturamento_fluxo];
                    gvFaturamentoFluxo.DataBind();
                
                }

                if (dsDashBoard.Tables.Count > tab_pedidos_vendedor && dsDashBoard.Tables[tab_pedidos_vendedor].Rows.Count > 0)
                {
                    div_graficoPedidosVendedor.Visible = true;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables_PedidosVendedor", TT.FrameWork.Grid.DataBindComScriptPaging(gvPedidosVendedor, dsDashBoard.Tables[tab_pedidos_vendedor], false, false, false, 2, "desc"), true);
                    Grid.SomarColunas(gvPedidosVendedor, false, Grid.Formatação.Moeda, 2, 3, 4);
                }

                
                
                sb.AppendLine("</script>");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "dashboardComercial" + Guid.NewGuid(), sb.ToString(), false);              
                                                
            }
            catch { }

        }

        private string ConsultaVendedor()
        {
            string sErro = "";
            DataSet dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_VENDEDOR");
            vParametros.Add("@idUsuario", Identity.Variaveis.idUsuario());
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                string sIdVendedor = Retorno.DATASET(dsPesquisa, "idVendedor");
                return sIdVendedor;
            }
            return "0";
        }

        protected void cmdFiltro_Click(object sender, EventArgs e)
        {
            ConsultarDados();
        }

        protected void gvOrcamentoGeral_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int idStatus = Convert.ToInt32(gvOrcamentoGeral.DataKeys[e.Row.RowIndex].Value);

                HyperLink hlsStatusOrcamento = (HyperLink)e.Row.FindControl("hlsStatusOrcamento");
                if (hlsStatusOrcamento != null)
                {
                    var filtro = bs_FiltroInicial.FirstOrDefault();

                    string url;

                    url = string.Format("/App/Paginas/Comercial/Orcamento.aspx?dashboard={0}&idStatus={1}&dtInicio={2}&dtFinal={3}&idFluxo={4}&idEmpresa={5}&idVendedor={6}", "dashboardComercial", idStatus, filtro.dtInicio, filtro.dtFinal, filtro.sIdFluxo, filtro.sIdEmpresa, filtro.sIdVendedor);

                    hlsStatusOrcamento.NavigateUrl = ResolveUrl(url);
                }
            }
        }

        //protected void gvPedidosGeral_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        int idStatus = Convert.ToInt32(gvPedidosGeral.DataKeys[e.Row.RowIndex].Value);

        //        HyperLink hlsStatusPedidos = (HyperLink)e.Row.FindControl("hlsStatusPedidos");
        //        if (hlsStatusPedidos != null)
        //        {
        //            var filtro = bs_FiltroInicial.FirstOrDefault();

        //            string url;

        //            url = string.Format("/App/Paginas/Pedidos.aspx?dashboard={0}&idStatus={1}&dtInicio={2}&dtFinal={3}&idFluxo={4}&idEmpresa={5}&idVendedor={6}", "dashboardComercial", idStatus, filtro.dtInicio, filtro.dtFinal, filtro.sIdFluxo, filtro.sIdEmpresa, filtro.sIdVendedor);

        //            hlsStatusPedidos.NavigateUrl = ResolveUrl(url);
        //        }
        //    }
        //}

        protected void gvFaturamentoFluxo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                DataTable dt = (DataTable)ViewState["DadosFaturamentoFluxo"];

                if (dt == null)
                    return;

                e.Row.Cells[0].Text = "<b>Total:</b>";
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Left;
                e.Row.Font.Bold = true;

                int colIndex = 1;
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName != "nMes" && col.ColumnName != "sMes")
                    {
                        decimal total = dt.AsEnumerable().Where(row => row[col] != DBNull.Value).Sum(row => Convert.ToDecimal(row[col]));
                        e.Row.Cells[colIndex].Text = total.ToString("N2");
                        e.Row.Cells[colIndex].HorizontalAlign = HorizontalAlign.Left;
                        e.Row.Cells[colIndex].Font.Bold = true;
                        colIndex++;
                    }
                }
            }            
        }

        protected void gvFaturamento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string[] parts = e.Row.Cells[1].Text.Split('/');
                string ano = parts[1].Trim();
                string mes = parts[0].Trim();                
                decimal valor = 0;
                decimal vlrFaturado = 0;
                HyperLink link = e.Row.FindControl("nValor") as HyperLink;
                HyperLink nValorFaturado = e.Row.FindControl("nValorFaturado") as HyperLink;
                var filtro = bs_FiltroInicial.FirstOrDefault();
                string url = ResolveUrl($"~/App/Paginas/Pedidos.aspx?dashboard={mes}&ano={ano}&Faturamento=S&idFluxo={filtro.sIdFluxo}&idEmpresa={filtro.sIdEmpresa}&idVendedor={filtro.sIdVendedor}");
                string urls = ResolveUrl($"~/App/Paginas/Pedidos.aspx?dashboard={mes}&ano={ano}&Faturamento=N&idFluxo={filtro.sIdFluxo}&idEmpresa={filtro.sIdEmpresa}&idVendedor={filtro.sIdVendedor}");
                if (link != null)
                {
                    valor = decimal.Parse(link.Text);
                    link.NavigateUrl = url;
                }
                if (nValorFaturado != null)
                {
                    vlrFaturado = decimal.Parse(nValorFaturado.Text);
                    nValorFaturado.NavigateUrl = urls;
                }
                link.Text = valor.ToString("N2");
                nValorFaturado.Text = vlrFaturado.ToString("N2");
            }
            
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                DataTable dt = (DataTable)ViewState["DadosFaturamento"];

                if (dt == null)
                    return;

                e.Row.Cells[0].Text = "<b>Total:</b>";
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Left;
                e.Row.Font.Bold = true;

                int colIndex = 2;
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName != "nMes" && col.ColumnName != "sMes")
                    {
                        decimal total = dt.AsEnumerable().Where(row => row[col] != DBNull.Value).Sum(row => Convert.ToDecimal(row[col]));
                        e.Row.Cells[colIndex].Text = total.ToString("N2");
                        e.Row.Cells[colIndex].HorizontalAlign = HorizontalAlign.Left;
                        e.Row.Cells[colIndex].Font.Bold = true;
                        colIndex++;
                    }
                }
            }
        }

        protected void gvPedidosClientes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int idCliente = Convert.ToInt32(gvPedidosClientes.DataKeys[e.Row.RowIndex].Value);

                HyperLink hlsClientePedido = (HyperLink)e.Row.FindControl("hlsClientePedido");
                if (hlsClientePedido != null)
                {
                    var filtro = bs_FiltroInicial.FirstOrDefault();

                    string url;

                    url = string.Format("/App/Paginas/Pedidos.aspx?dashboard={0}&idCliente={1}&dtInicio={2}&dtFinal={3}&idFluxo={4}&idEmpresa={5}&idVendedor={6}&sTipo=faturamento", "dashboardComercial", idCliente, filtro.dtInicio, filtro.dtFinal, filtro.sIdFluxo, filtro.sIdEmpresa, filtro.sIdVendedor);

                    hlsClientePedido.NavigateUrl = ResolveUrl(url);
                }
            }
        }

        protected void gvPedidosFluxo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int idFluxo = Convert.ToInt32(gvPedidosFluxo.DataKeys[e.Row.RowIndex]["idFluxo"]);

                HyperLink hlsFluxoPedido = (HyperLink)e.Row.FindControl("hlsFluxoPedido");
                if (hlsFluxoPedido != null)
                {
                    var filtro = bs_FiltroInicial.FirstOrDefault();

                    string url;

                    url = string.Format("/App/Paginas/Pedidos.aspx?dashboard={0}&dtInicio={1}&dtFinal={2}&idFluxo={3}&idEmpresa={4}&idVendedor={5}&sTipo=faturamento", "dashboardComercial", filtro.dtInicio, filtro.dtFinal, idFluxo, filtro.sIdEmpresa, filtro.sIdVendedor);

                    hlsFluxoPedido.NavigateUrl = ResolveUrl(url);
                }
            }
        }

        protected void gvPedidosVendedor_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int idVendedor = Convert.ToInt32(gvPedidosVendedor.DataKeys[e.Row.RowIndex].Value);

                HyperLink hlsDscVendedor = (HyperLink)e.Row.FindControl("hlsDscVendedor");
                if (hlsDscVendedor != null)
                {
                    var filtro = bs_FiltroInicial.FirstOrDefault();

                    string url;

                    url = string.Format("/App/Paginas/Pedidos.aspx?dashboard={0}&idVendedor={1}&dtInicio={2}&dtFinal={3}&idFluxo={4}&idEmpresa={5}&sTipo=faturamento", "dashboardComercial", idVendedor, filtro.dtInicio, filtro.dtFinal, filtro.sIdFluxo, filtro.sIdEmpresa);

                    hlsDscVendedor.NavigateUrl = ResolveUrl(url);
                }
            }
        }
    }

    public class cls_VendedorData
    {
        public string Vendedor { get; set; }
        public decimal Meta { get; set; }
        public decimal Realizado { get; set; }
    }

    [Serializable]
    public class cls_FiltroInicial
    {
        public string dtInicio { get; set; }
        public string dtFinal { get; set; }
        public string sIdVendedor { get; set; }
        public string sIdFluxo { get; set; }
        public string sIdEmpresa { get; set; }
    }
}