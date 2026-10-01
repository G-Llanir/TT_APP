using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using System.Text;
using System.Globalization;
using System.Xml.Linq;
using System.Web.DynamicData;
using System.Web.Script.Serialization;
using NPOI.SS.Formula.Functions;
using System.Runtime.InteropServices.ComTypes;
using static iTextSharp.text.pdf.AcroFields;

namespace TT_Flow.Dashboards.Adm
{
    public partial class Dashboard_Adm : System.Web.UI.Page
    {

        //========================== Graficos Donut ============================
        int Grafico_Pagamento = 0;
        int TAB_Pagamentos = 1;
        int Grafico_Recebimento = 2;
        int TAB_Recebimentos = 3;
        int TAB_PagarAnual = 30;
        int TAB_ReceberAnual = 31;
        ////====================================================================


        //================ Contabil : Receitas / Despesas ======================
        int TAB_Graficos_Contabil_Receber_Ano = 4;
        int TAB_Graficos_Contabil_Pagar_Ano = 7;

        int TAB_Graficos_Contabil_Receber_Trimestre = 26;
        int TAB_Graficos_Contabil_Pagar_Trimestre = 27;

        int TAB_Graficos_Contabil_Receber_Semana = 28;
        int TAB_Graficos_Contabil_Pagar_Semana = 29;

        //======================================================================


        //======================= GRID ADIANTAMENTO ============================
        int TAB_Adiantamentos = 9;
        //======================================================================



        //================= Caixa: Recebimentos / Pagamentos ===================
        int TAB_Graficos_Pagamento_Semana = 10;
        int TAB_Graficos_Recebimento_Semana = 11;

        int TAB_Graficos_Pagamento_Ano = 12;
        int TAB_Graficos_Recebimento_Ano = 13;

        int TAB_Graficos_Recebimento_Trimestre = 14;
        int TAB_Graficos_Pagamento_Trimestre = 15;
        //======================================================================



        //==================== Emprestimo: Pagar / Receber =====================
        int TAB_Grafico_Emprestimo_Receber_Ano = 16;
        int TAB_Grafico_Emprestimo_Pagar_Ano = 17;

        int TAB_Grafico_Emprestimo_Receber_Trimestre = 18;
        int TAB_Grafico_Emprestimo_Pagar_Trimestre = 19;

        int TAB_Grafico_Emprestimo_Receber_Semana = 20;
        int TAB_Grafico_Emprestimo_Pagar_Semana = 21;
        //======================================================================


        //==================== Comparativo: Pagar / Receber =====================
        int TAB_Grafico_Comparativo_Receber_Semana = 22;
        int TAB_Grafico_Comparativo_Pagar_Semana = 23;

        int TAB_Grafico_Comparativo_Receber_Trimestre = 24;
        int TAB_Grafico_Comparativo_Pagar_Trimestre = 25;

        int TAB_Grafico_Comparativo_Pagar_Ano = 6;
        int TAB_Grafico_Comparativo_Receber_Ano = 5;
        int TAB_Grafico_Comparativo_Meta_Ano = 8;
        //======================================================================

        string sLinkContasPagar = "/app/Paginas/Adm/Financeiro/ContasPagar.aspx?dashboard=";
        string sLinkContasReceber = "/app/Paginas/Adm/Financeiro/ContasReceber.aspx?dashboard=";
        List<string> itensSelecionados = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            if (!IsPostBack)
            {
                PopularCombos();
            }
            else
            {

            }

            itensSelecionados.Clear();
            foreach (ListItem item in lstidFluxo.Items)
            {
                if (item.Selected)
                {
                    itensSelecionados.Add(item.Value);
                }
            }

            if (itensSelecionados.Count < 1)
            {
                ListItem item = lstidFluxo.Items.FindByValue(DateTime.Now.Year.ToString());
                item.Selected = true;
                itensSelecionados.Add(DateTime.Now.Year.ToString());
            }

            string selectedYear = string.Join(",", itensSelecionados);
            string EmpresaSelecionada = ddlidEmpresa.SelectedValue;
            string AdiantamentoSelecionado = ddlidAdiantamentoSelecionado.SelectedValue;
        
            AtualizarDashBoard(IDENTITY.Variaveis.idUsuario(), selectedYear, EmpresaSelecionada, AdiantamentoSelecionado);

            ddlidEmpresa_SelectedIndexChanged(objSender, objEventArgs);
           //ddlAnoFinanceiro_SelectedIndexChanged(objSender, objEventArgs);
            ddlExibicao_SelectedIndexChanged(objSender, objEventArgs);

            if (itensSelecionados.Count > 1 || itensSelecionados[0].ToString() != "2024")
            {
                ddlExibicao.SelectedValue = "5";
                ddlExibicao.Visible = false;
            }
            else if (itensSelecionados[0].ToString() == "2024")
            {
                ddlExibicao.Visible = true;
            }

            Div_Detalhe_Pag.Visible = false;
            Div_Grafico_Pag.Visible = true;
            Div_Switch_Pag.Visible = true;
            Div_Detalhe_Rec.Visible = false;
            Div_Grafico_Rec.Visible = true;
            Div_Switch_Rec.Visible = true;
            DIV_Ano_Total.Visible = false;
            Div_Grafico_Ano.Visible = true;
            Div_Switch_Ano.Visible = true;


        }

        void AtualizarDashBoard(string idContasPagar, string selectedYear, string EmpresaSelecionada, string AdiantamentoSelecionado)
        {

            DataSet dsDashBoard;
            string sSql = "sp_Flow_DashBoard_Adm";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@idContasPagar", idContasPagar.ToString());
            vParametros.Add("@AnoSelecionado", selectedYear);
            vParametros.Add("@idEmpresaSelecionada", EmpresaSelecionada);

            vParametros.Add("@idAdiantamentoSelecionado", AdiantamentoSelecionado);

            string sUsuarioLogado = "";
            string sPermissaoPagar = "N";
            if (FUNCOES.ValidaPermissao(Permissao.Financeiro.DashboardAdm.VisualizarTudoPagar))
            {
                sPermissaoPagar = "S";
            }
            else
            {
                sPermissaoPagar = "N";
            }
            string sPermissaoReceber = "N";
            if (FUNCOES.ValidaPermissao(Permissao.Financeiro.DashboardAdm.VisualizarTudoReceber))
            {
                sPermissaoReceber = "S";
            }
            else
            {
                sPermissaoReceber = "N";
            }

            sUsuarioLogado = HttpContext.Current.Session["idUsuario"].ToString();
            vParametros.Add("@sUsuarioLogado", sUsuarioLogado);
            vParametros.Add("@sPermissaoReceber", sPermissaoReceber);
            vParametros.Add("@sPermissaoPagar", sPermissaoPagar);


            dsDashBoard = BD.ExecutarDataSet(sSql, vParametros);

            if (BD.ValidarDataSet(dsDashBoard))
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("$(function() {");

                #region | Grid Adiantamento

                //=====================================================================
                //===========================Grid Adiantamento=========================
                //=====================================================================

                if (dsDashBoard.Tables[TAB_Adiantamentos] != null && dsDashBoard.Tables[TAB_Adiantamentos].Rows.Count > 0)
                {
                    Div_Adiantamento.Visible = true;

                    GRID.DataBind(gvAdiantamento, dsDashBoard, TAB_Adiantamentos);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTablesAdiantado", TT.FrameWork.Grid.DataBindComScript(gvAdiantamento, dsDashBoard.Tables[TAB_Adiantamentos], 1, "Desc"), true);
                    string scriptAdiantado = $@"
                    <script type='text/javascript'>
                    $(document).ready(function() {{
                        setTimeout(function() {{
                            var table = $('#{gvAdiantamento.ClientID}').DataTable();
                            table.page.len(10).draw();
                        }}, 0);
                    }});
                    </script>
                    ";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "AdjustDataTablesPageSize", scriptAdiantado, false);

                    GRID.SomarColunas(gvAdiantamento, true, GRID.Formatação.Moeda, 3, 4, 5, 6, 7, 8, 9, 10);
                }
                else
                {
                    Div_Adiantamento.Visible = false;
                }
                sb.AppendLine("});");

                #endregion

                //-===================================================================


                #region | Donut Contas Pagar
                //=====================================================================
                //========================Contas Pagar=================================
                //=====================================================================

                sb.AppendLine("$(document).ready(function() {");
                sb.AppendLine("    atualizaGraficoePag();");
                sb.AppendLine("    $('" + selectedYear + ", #" + ddlidEmpresa.ClientID + "').change(atualizaGraficoePag);");
                sb.AppendLine("    $('#switchPag').change(atualizaGraficoePag);");
                sb.AppendLine("});");

                sb.AppendLine("function atualizaGraficoePag() {");
                sb.AppendLine("    var isValue = $('#switchPag').is(':checked');");
                sb.AppendLine("    $('#GraficoPagamentos').empty();");
                sb.AppendLine("    var data = isValue ? [");

                for (int nLinhasOrigem = 0; nLinhasOrigem < dsDashBoard.Tables[Grafico_Pagamento].Rows.Count; nLinhasOrigem++)
                {
                    sb.Append("{label: '" + dsDashBoard.Tables[Grafico_Pagamento].Rows[nLinhasOrigem]["titulo"] + "',");
                    sb.Append("	value: getValidNumber(" + Convert.ToDecimal(dsDashBoard.Tables[Grafico_Pagamento].Rows[nLinhasOrigem]["valor"]).ToString(CultureInfo.InvariantCulture) + ")}");
                    if (nLinhasOrigem < dsDashBoard.Tables[Grafico_Pagamento].Rows.Count - 1)
                    {
                        sb.AppendLine(", ");
                    }
                }

                sb.AppendLine("    ] : [");

                for (int nLinhasOrigem = 0; nLinhasOrigem < dsDashBoard.Tables[Grafico_Pagamento].Rows.Count; nLinhasOrigem++)
                {
                    sb.Append("{label: '" + dsDashBoard.Tables[Grafico_Pagamento].Rows[nLinhasOrigem]["titulo"] + "',");
                    sb.Append("	value: '" + dsDashBoard.Tables[Grafico_Pagamento].Rows[nLinhasOrigem]["quantidade"] + "'}");
                    if (nLinhasOrigem < dsDashBoard.Tables[Grafico_Pagamento].Rows.Count - 1)
                    {
                        sb.AppendLine(", ");
                    }
                }

                sb.AppendLine("    ];");

                sb.AppendLine("    Morris.Donut({");
                sb.AppendLine("        element: 'GraficoPagamentos',");
                sb.AppendLine("        data: data,");
                sb.AppendLine("        resize: true,");
                sb.AppendLine("        colors: ['#FB5B68', '#F88A30', '#FEB172', '#BADCEE', '#D0E6F0', '#E0EEF5'],");
                sb.AppendLine("        formatter: function(y, data) { return isValue ? 'R$ ' + y.toFixed(2).replace('.', ',').replace(/\\B(?=(\\d{3})+(?!\\d))/g, '.') : y.toString().replace(/\\B(?=(\\d{3})+(?!\\d))/g, '.'); },");
                sb.AppendLine("    }).on('click', function(i, row) {");
                //  sb.AppendLine("        window.location.href = row.url;");
                sb.AppendLine("    });");
                sb.AppendLine("}");

                sb.AppendLine("function generateUrl(dashboardType) {");
                sb.AppendLine("    var ano = '" + selectedYear + "';");
                sb.AppendLine("    var empresa = $('#" + ddlidEmpresa.ClientID + "').val();");
                sb.AppendLine("    return '/app/Paginas/Adm/Financeiro/' + (dashboardType === 'PagosNoAno' ? 'ContasPagar.aspx' : 'ContasReceber.aspx') + '?dashboard=' + dashboardType + '&sAno=' + ano + '&sEmpresa=' + empresa");
                sb.AppendLine("}");

                sb.AppendLine("function getValidNumber(value) {");
                sb.AppendLine("    return isNaN(value) || value === null ? 0 : parseFloat(value);");
                sb.AppendLine("}");


                litTabPagamento.Text = dsDashBoard.Tables[TAB_Pagamentos].Rows[0]["sTabelaPagamento"].ToString();

                #endregion

                #region | Donut Contas Receber
                //=====================================================================
                //========================Contas Receber=================================
                //====================================================================

                sb.AppendLine("$(document).ready(function() {");
                sb.AppendLine("    atualizaGraficoeRec();");
                sb.AppendLine("    $('" + selectedYear + ", #" + ddlidEmpresa.ClientID + "').change(atualizaGraficoeRec);");
                sb.AppendLine("    $('#switchRec').change(atualizaGraficoeRec);");
                sb.AppendLine("});");

                sb.AppendLine("function atualizaGraficoeRec() {");
                sb.AppendLine("    var isValue = $('#switchRec').is(':checked');");
                sb.AppendLine("    $('#GraficoRecebimentos').empty();");

                sb.AppendLine("    var data = isValue ? [");

                for (int nLinhasOrigem = 0; nLinhasOrigem < dsDashBoard.Tables[Grafico_Recebimento].Rows.Count; nLinhasOrigem++)
                {
                    sb.Append("{label: '" + dsDashBoard.Tables[Grafico_Recebimento].Rows[nLinhasOrigem]["titulo"] + "',");
                    sb.Append("	value: getValidNumber(" + Convert.ToDecimal(dsDashBoard.Tables[Grafico_Recebimento].Rows[nLinhasOrigem]["valor"]).ToString(CultureInfo.InvariantCulture) + ")}");
                    if (nLinhasOrigem < dsDashBoard.Tables[Grafico_Recebimento].Rows.Count - 1)
                    {
                        sb.AppendLine(", ");
                    }
                }               

                sb.AppendLine("    ] : [");

                for (int nLinhasOrigem = 0; nLinhasOrigem < dsDashBoard.Tables[Grafico_Recebimento].Rows.Count; nLinhasOrigem++)
                {
                    sb.Append("{label: '" + dsDashBoard.Tables[Grafico_Recebimento].Rows[nLinhasOrigem]["titulo"] + "',");
                    sb.Append("	value: '" + dsDashBoard.Tables[Grafico_Recebimento].Rows[nLinhasOrigem]["quantidade"] + "'}");
                    if (nLinhasOrigem < dsDashBoard.Tables[Grafico_Recebimento].Rows.Count - 1)
                    {
                        sb.AppendLine(", ");
                    }
                }

                sb.AppendLine("    ];");

                sb.AppendLine("    Morris.Donut({");
                sb.AppendLine("        element: 'GraficoRecebimentos',");
                sb.AppendLine("        data: data,");
                sb.AppendLine("        resize: true,");
                sb.AppendLine("        colors: ['#FB5B68', '#F88A30', '#FEB172', '#BADCEE', '#D0E6F0', '#E0EEF5'],");
                sb.AppendLine("        formatter: function(y, data) { return isValue ? 'R$ ' + y.toFixed(2).replace('.', ',').replace(/\\B(?=(\\d{3})+(?!\\d))/g, '.') : y.toString().replace(/\\B(?=(\\d{3})+(?!\\d))/g, '.'); },");
                sb.AppendLine("    }).on('click', function(i, row) {");
                // sb.AppendLine("        window.location.href = row.url;");
                sb.AppendLine("    });");
                sb.AppendLine("}");

                sb.AppendLine("function generateUrl(dashboardType) {");
                sb.AppendLine("    var ano = '" + selectedYear + "';");
                sb.AppendLine("    var empresa = $('#" + ddlidEmpresa.ClientID + "').val();");
                sb.AppendLine("    return '/app/Paginas/Adm/Financeiro/' + (dashboardType === 'PagosNoAno' ? 'ContasPagar.aspx' : 'ContasReceber.aspx') + '?dashboard=' + dashboardType + '&sAno=' + ano + '&sEmpresa=' + empresa;");
                sb.AppendLine("}");

                sb.AppendLine("function getValidNumber(value) {");
                sb.AppendLine("    return isNaN(value) || value === null ? 0 : parseFloat(value);");
                sb.AppendLine("}");


                litTabRecebimento.Text = dsDashBoard.Tables[TAB_Recebimentos].Rows[0]["sTabelaRecebimento"].ToString();

                #endregion

                #region | Donut Ano Pagar/Receber
                //=====================================================================
                //=====================Contas Pagar/Receber ANO========================
                //=====================================================================


                sb.AppendLine("$(document).ready(function() {");
                sb.AppendLine("    atualizaGraficoeAno();");

                sb.AppendLine("    $('" + selectedYear + ", #" + ddlidEmpresa.ClientID + "').on('change', function() {");
                sb.AppendLine("         atualizaGraficoeAno();");
                sb.AppendLine("     });");
                sb.AppendLine("    $('#switchAno').change(atualizaGraficoeAno);");
                sb.AppendLine("});");

                sb.AppendLine("function atualizaGraficoeAno() {");
                sb.AppendLine("    var isValue = $('#switchAno').is(':checked');");
                sb.AppendLine("    $('#GraficoAnual').empty();");

                sb.AppendLine("    var data = isValue ? [");
                sb.AppendLine("        {label: 'Valor Pagamentos', value: getValidNumber(" + Convert.ToDecimal(dsDashBoard.Tables[TAB_PagarAnual].Rows[0]["lblValorPagamentosAnual"]).ToString(CultureInfo.InvariantCulture) + "), url: generateUrl('PagosNoAno')},");
                sb.AppendLine("        {label: 'Valor Recebimentos', value: getValidNumber(" + Convert.ToDecimal(dsDashBoard.Tables[TAB_ReceberAnual].Rows[0]["lblValorRecebimentosAnual"]).ToString(CultureInfo.InvariantCulture) + "), url: generateUrl('RecebimentosNoAno')}");
                sb.AppendLine("    ] : [");
                sb.AppendLine("        {label: 'Qnt Pagamentos', value: getValidNumber(" + Convert.ToInt32(dsDashBoard.Tables[TAB_PagarAnual].Rows[0]["lblQtdPagamentosAnual"]) + "), url: generateUrl('PagosNoAno')},");
                sb.AppendLine("        {label: 'Qnt Recebimentos', value: getValidNumber(" + Convert.ToInt32(dsDashBoard.Tables[TAB_ReceberAnual].Rows[0]["lblQtdRecebimentosAnual"]) + "), url: generateUrl('RecebimentosNoAno')}");
                sb.AppendLine("    ];");

                sb.AppendLine("    Morris.Donut({");
                sb.AppendLine("        element: 'GraficoAnual',");
                sb.AppendLine("        data: data,");
                sb.AppendLine("        resize: true,");
                sb.AppendLine("        colors: ['#FB5B68', '#0a8a0f'],");
                sb.AppendLine("        formatter: function(y, data) {");
                sb.AppendLine("            if (data.label.includes('Valor')) {");
                sb.AppendLine("                return 'R$ ' + y.toFixed(2).replace('.', ',').replace(/\\B(?=(\\d{3})+(?!\\d))/g, '.');");
                sb.AppendLine("            } else {");
                sb.AppendLine("                return y.toString().replace(/\\B(?=(\\d{3})+(?!\\d))/g, '.');");
                sb.AppendLine("            }");
                sb.AppendLine("        },");


                sb.AppendLine("    }).on('click', function(i, row) {");
                // sb.AppendLine("        if(row.url) window.location.href = row.url;");
                sb.AppendLine("    });");

                //sb.AppendLine("    document.querySelectorAll('#tbAnual tr[data-dashboard]').forEach(row => {");
                //sb.AppendLine("    var dashboardType = row.getAttribute('data-dashboard');");
                //sb.AppendLine("    row.setAttribute('data-url', generateUrl(dashboardType));");
                //sb.AppendLine("});");

                sb.AppendLine("}");

                sb.AppendLine("function generateUrl(dashboardType) {");
                sb.AppendLine("    var ano = '" + selectedYear + "';");
                sb.AppendLine("    var empresa = $('#" + ddlidEmpresa.ClientID + "').val();");             
                sb.AppendLine("    return '/app/Paginas/Adm/Financeiro/' + (dashboardType === 'PagosNoAno' ? 'ContasPagar.aspx' : 'ContasReceber.aspx') + '?dashboard=' + dashboardType + '&sAno=' + ano + '&sEmpresa=' + empresa;");
                sb.AppendLine("}");

                sb.AppendLine("function getValidNumber(value) {");
                sb.AppendLine("    return isNaN(value) || value === null ? 0 : parseFloat(value);");
                sb.AppendLine("}");

                if (dsDashBoard != null && dsDashBoard.Tables.Count > 0)
                {
                    if (dsDashBoard.Tables[TAB_ReceberAnual].Rows.Count > 0)
                    {
                        DataRow drReceb = dsDashBoard.Tables[TAB_ReceberAnual].Rows[0];

                        lblQtdRecebimentosAnual_Grafico.Text = drReceb["lblQtdRecebimentosAnual"].ToString();
                        lblValorRecebimentosAnual_Grafico.Text = String.Format(new CultureInfo("pt-BR"), "{0:C}", drReceb["lblValorRecebimentosAnual"]);
                    }

                    if (dsDashBoard.Tables[TAB_PagarAnual].Rows.Count > 0)
                    {
                        DataRow drPag = dsDashBoard.Tables[TAB_PagarAnual].Rows[0];

                        lblQtdPagamentosAnual_Grafico.Text = drPag["lblQtdPagamentosAnual"].ToString();
                        lblValorPagamentosAnual_Grafico.Text = String.Format(new CultureInfo("pt-BR"), "{0:C}", drPag["lblValorPagamentosAnual"]);
                    }
                }

                #endregion


                //-===================================================================


                ConfigurarGraficoCaixaAno(dsDashBoard);
                ConfigurarGraficoCaixaTrimestre(dsDashBoard);
                ConfigurarGraficoCaixaSemanal(dsDashBoard);

                ConfigurarGraficoEmprestimoAno(dsDashBoard);
                ConfigurarGraficoEmprestimoTrimestre(dsDashBoard);
                ConfigurarGraficoEmprestimoSemana(dsDashBoard);

                ConfigurarGraficoComparativoAno(dsDashBoard);
                //ConfigurarGraficoComparativoTrimestre(dsDashBoard);
                //ConfigurarGraficoComparativoSemana(dsDashBoard);


                ConfigurarGraficoContabilAno(dsDashBoard);
                ConfigurarGraficoContabilTrimestre(dsDashBoard);
                ConfigurarGraficoContabilSemanal(dsDashBoard);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "DashBoard_" + Guid.NewGuid().ToString(), sb.ToString(), true);

            }
        }

        #region Graficos novos
        private void ConfigurarGraficoCaixaAno(DataSet dsDashBoard)
        {
            string selectedYear = string.Join(",", itensSelecionados);

            StringBuilder sb = new StringBuilder();
            sb.Append("<script>");
            sb.Append("$(document).ready(function() {");
            sb.Append("const data = {");
            sb.Append("labels: [");

            int maxCount = Math.Max(dsDashBoard.Tables[TAB_Graficos_Recebimento_Ano].Rows.Count, dsDashBoard.Tables[TAB_Graficos_Pagamento_Ano].Rows.Count);

            for (int i = 0; i < maxCount; i++)
            {
                int referencia = dsDashBoard.Tables[TAB_Graficos_Recebimento_Ano].Rows.Count > dsDashBoard.Tables[TAB_Graficos_Pagamento_Ano].Rows.Count ? TAB_Graficos_Recebimento_Ano : TAB_Graficos_Pagamento_Ano;
                string sMes = i < dsDashBoard.Tables[referencia].Rows.Count ? dsDashBoard.Tables[referencia].Rows[i]["sMes"].ToString() : "";
                sb.Append("\"" + sMes + "\"");

                if (i < maxCount - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("datasets: [{");
            sb.Append("label: 'Pagamentos Mês',");
            sb.Append("data: [");

            for (int i = 0; i < maxCount; i++)
            {
                double valorPagar = i < dsDashBoard.Tables[TAB_Graficos_Pagamento_Ano].Rows.Count ? Convert.ToDouble(dsDashBoard.Tables[TAB_Graficos_Pagamento_Ano].Rows[i]["TotalMensal"]) : 0;
                //sb.Append(valorPagar.ToString(CultureInfo.InvariantCulture));
                string valorFormatadoPagar = valorPagar.ToString("F2", CultureInfo.InvariantCulture);
                sb.Append(valorFormatadoPagar);

                if (i < maxCount - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("borderColor: 'rgba(255, 99, 132, 1)',");
            sb.Append("backgroundColor: 'rgba(245, 71, 71 )'");
            sb.Append("}, {");
            sb.Append("label: 'Recebimentos Mês',");
            sb.Append("data: [");

            for (int i = 0; i < maxCount; i++)
            {
                double valorReceber = i < dsDashBoard.Tables[TAB_Graficos_Recebimento_Ano].Rows.Count ? Convert.ToDouble(dsDashBoard.Tables[TAB_Graficos_Recebimento_Ano].Rows[i]["TotalMensal"]) : 0;
                //sb.Append(valorReceber.ToString(CultureInfo.InvariantCulture));
                string valorFormatadoReceber = valorReceber.ToString("F2", CultureInfo.InvariantCulture);
                sb.Append(valorFormatadoReceber);

                if (i < maxCount - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("borderColor: 'rgba(75, 192, 192, 1)',");
            sb.Append("backgroundColor: 'rgba(34, 130, 239 )'");
            sb.Append("}]");
            sb.Append("};");

            sb.Append("const config = {");
            sb.Append("type: 'bar',");
            sb.Append("data: data,");
            sb.Append("options: {");
            sb.Append("responsive: true,");
            sb.Append("maintainAspectRatio: false,");
            sb.Append("plugins: {");
            sb.Append("legend: { position: 'top' },");
            sb.Append("title: { display: true, text: 'Caixa: Pagamento e Recebimento Mensal' },");            
            sb.Append("tooltip: {");
            sb.Append(" callbacks: {");
            sb.Append("label: function(context) {");
            sb.Append(" let label = context.dataset.label || '';");
            sb.Append("let rawValue = context.parsed.y;");
            sb.Append(" let formattedValue = Number(rawValue).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });");
            sb.Append("return label + ': ' + formattedValue;");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("},");
            sb.Append("scales: {");
            sb.Append("y: {");
            sb.Append("beginAtZero: true,");
            sb.Append("ticks: {");
            sb.Append("callback: function(value) {");           
            sb.Append("return value.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("};");

            sb.Append("var ctx1 = document.getElementById('Grafico_Caixa_Ano_v2').getContext('2d');");
            sb.Append("var graficoCaixaAno = new Chart(ctx1, config);");

            sb.Append("const currentMonthLabel = '" + GetMonthOfYear(DateTime.Now) + "';");
            sb.Append("graficoCaixaAno.data.labels.forEach((label, index) => {");
            sb.Append("if (label === currentMonthLabel) {");
            sb.Append("graficoCaixaAno.getDatasetMeta(0).data[index].custom = { backgroundColor: '#000000', fontWeight: 'bold', fontSize: 16 };");
            sb.Append("graficoCaixaAno.getDatasetMeta(1).data[index].custom = { backgroundColor: '#000000', fontWeight: 'bold', fontSize: 16 };");
            sb.Append("}");
            sb.Append("});");

            sb.Append("document.getElementById('Grafico_Caixa_Ano_v2').addEventListener('click', function(evt) {");
            sb.Append("var activePoints = graficoCaixaAno.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);");
            sb.Append("if (activePoints.length) {");
            sb.Append("var firstPoint = activePoints[0];");
            sb.Append("var label = graficoCaixaAno.data.labels[firstPoint.index];");
            sb.Append("var datasetLabel = graficoCaixaAno.data.datasets[firstPoint.datasetIndex].label;");
            sb.Append("var ano = '" + selectedYear + "';");
            sb.Append("var empresa = '" + ddlidEmpresa.SelectedValue + "';");
            sb.Append("var baseUrlPagar = '" + sLinkContasPagar + "';");
            sb.Append("var baseUrlReceber = '" + sLinkContasReceber + "';");
            sb.Append("var mesCompleto = label.substring(0, 7);");
            sb.Append("if (datasetLabel === 'Recebimentos Mês') { window.open(baseUrlReceber + 'CaixaRecebidosMes&sMes=' + mesCompleto + '&sAno=' + ano + '&sEmpresa=' + empresa, '_blank'); }");
            sb.Append("else if (datasetLabel === 'Pagamentos Mês') { window.open(baseUrlPagar + 'CaixaPagosMes&sMes=' + mesCompleto + '&sAno=' + ano + '&sEmpresa=' + empresa, '_blank'); }");
            sb.Append("}");
            sb.Append("});");

            sb.Append("});");
            sb.Append("</script>");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "GraficoCaixaAno_" + Guid.NewGuid(), sb.ToString(), false);
        }
        private void ConfigurarGraficoCaixaTrimestre(DataSet dsDashBoard)
        {
            string selectedYear = string.Join(",", itensSelecionados);

            StringBuilder sb = new StringBuilder();
            sb.Append("<script>");

            sb.Append("$(document).ready(function() {");

            sb.Append("var canvas = document.getElementById('Grafico_Caixa_Trimestral_v2');");
            sb.Append("if(!canvas) {");
            //sb.Append(" console.log(\"Canvas #Grafico_Caixa_Trimestral_v2 não existe no DOM!\");");
            sb.Append("return;}");

            sb.Append("const labels = [");

            int trimestreSelecionado;
            int[] semanasDoTrimestre;

            if (int.TryParse(ddlExibicao.SelectedValue, out trimestreSelecionado) && trimestreSelecionado >= 1 && trimestreSelecionado <= 4)
            {
                semanasDoTrimestre = GetSemanasPorTrimestre(trimestreSelecionado);
            }
            else
            {
                semanasDoTrimestre = GetSemanasTrimestreAtual();
            }

            for (int i = 0; i < dsDashBoard.Tables[TAB_Graficos_Recebimento_Trimestre].Rows.Count; i++)
            {
                string sAno = dsDashBoard.Tables[TAB_Graficos_Recebimento_Trimestre].Rows[i]["Ano"].ToString();
                string sSemana = dsDashBoard.Tables[TAB_Graficos_Recebimento_Trimestre].Rows[i]["Semana"].ToString();
                if (semanasDoTrimestre.Contains(Convert.ToInt32(sSemana)))
                {
                    DateTime semanaInicio = CalcularPrimeiraDataDaSemanaISO(Convert.ToInt32(sAno), Convert.ToInt32(sSemana));
                    DateTime semanaFim = semanaInicio.AddDays(6);

                    string periodo = $"S{sSemana} {semanaInicio:dd/MM} - {semanaFim:dd/MM}";
                    sb.Append($"\"{periodo}\"");

                    if (i < dsDashBoard.Tables[TAB_Graficos_Recebimento_Trimestre].Rows.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
            }
            sb.Append("];");

            sb.Append("const data = {");
            sb.Append("labels: labels,");
            sb.Append("datasets: [");

            sb.Append("{");
            sb.Append("label: 'Pagamentos Semana',");
            sb.Append("data: [");
            for (int i = 0; i < dsDashBoard.Tables[TAB_Graficos_Pagamento_Trimestre].Rows.Count; i++)
            {
                string sSemana = dsDashBoard.Tables[TAB_Graficos_Pagamento_Trimestre].Rows[i]["Semana"].ToString();
                if (semanasDoTrimestre.Contains(Convert.ToInt32(sSemana)))
                {
                    double valorPagar = (i < dsDashBoard.Tables[TAB_Graficos_Pagamento_Trimestre].Rows.Count) ? Convert.ToDouble(dsDashBoard.Tables[TAB_Graficos_Pagamento_Trimestre].Rows[i]["TotalSemanal"]) : 0;
                    sb.Append(valorPagar.ToString(CultureInfo.InvariantCulture));

                    if (i < dsDashBoard.Tables[TAB_Graficos_Pagamento_Trimestre].Rows.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
            }
            sb.Append("],");
            sb.Append("borderColor: 'rgba(255, 99, 132, 1)',");
            sb.Append("backgroundColor: 'rgba(245, 71, 71)'");
            sb.Append("},");

            sb.Append("{");
            sb.Append("label: 'Recebimentos Semana',");
            sb.Append("data: [");
            for (int i = 0; i < dsDashBoard.Tables[TAB_Graficos_Recebimento_Trimestre].Rows.Count; i++)
            {
                string sSemana = dsDashBoard.Tables[TAB_Graficos_Recebimento_Trimestre].Rows[i]["Semana"].ToString();
                if (semanasDoTrimestre.Contains(Convert.ToInt32(sSemana)))
                {
                    double valorReceber = Convert.ToDouble(dsDashBoard.Tables[TAB_Graficos_Recebimento_Trimestre].Rows[i]["TotalSemanal"]);
                    sb.Append(valorReceber.ToString(CultureInfo.InvariantCulture));

                    if (i < dsDashBoard.Tables[TAB_Graficos_Recebimento_Trimestre].Rows.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
            }
            sb.Append("],");
            sb.Append("borderColor: 'rgba(75, 192, 192, 1)',");
            sb.Append("backgroundColor: 'rgba(34, 130, 239)'");
            sb.Append("}");

            sb.Append("]");
            sb.Append("};");

            sb.Append("const config = {");
            sb.Append("type: 'bar',");
            sb.Append("data: data,");
            sb.Append("options: {");
            sb.Append("responsive: true,");
            sb.Append("maintainAspectRatio: false,");
            sb.Append("plugins: {");
            sb.Append("legend: { position: 'top' },");
            sb.Append("title: { display: true, text: 'Caixa: Pagamentos e Recebimentos Semanal' },");
            sb.Append("tooltip: {");
            sb.Append(" callbacks: {");
            sb.Append("label: function(context) {");
            sb.Append(" let label = context.dataset.label || '';");
            sb.Append("let rawValue = context.parsed.y;");
            sb.Append(" let formattedValue = Number(rawValue).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });");
            sb.Append("return label + ': ' + formattedValue;");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("},");
            sb.Append("scales: {");
            sb.Append("y: {");
            sb.Append("beginAtZero: true,");
            sb.Append("ticks: {");
            sb.Append("callback: function(value) {");
            sb.Append("return value.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");

            sb.Append("};");

            sb.Append("var ctx2 = document.getElementById('Grafico_Caixa_Trimestral_v2').getContext('2d');");
            sb.Append("var graficoCaixaTrimestral = new Chart(ctx2, config);");

            sb.Append("document.getElementById('Grafico_Caixa_Trimestral_v2').addEventListener('click', function(evt) {");
            sb.Append("var activePoints = graficoCaixaTrimestral.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);");
            sb.Append("if (activePoints.length) {");
            sb.Append("var firstPoint = activePoints[0];");
            sb.Append("var label = graficoCaixaTrimestral.data.labels[firstPoint.index];");
            sb.Append("var datasetLabel = graficoCaixaTrimestral.data.datasets[firstPoint.datasetIndex].label;");
            sb.Append("var semana = label.split(' ')[0].replace('S', '').trim();");
            sb.Append("var ano = '" + selectedYear + "';");
            sb.Append("var empresa = '" + ddlidEmpresa.SelectedValue + "';");
            sb.Append("var baseUrlPagar = '" + sLinkContasPagar + "';");
            sb.Append("var baseUrlReceber = '" + sLinkContasReceber + "';");
            sb.Append("var url = (datasetLabel === 'Recebimentos Semana') ? baseUrlReceber + 'CaixaRecebidosSemana&sSemana=' + semana + '&sAno=' + ano + '&sEmpresa=' + empresa : baseUrlPagar + 'CaixaPagosSemana&sSemana=' + semana + '&sAno=' + ano + '&sEmpresa=' + empresa;");
            sb.Append("window.open(url,'_blank');");
            sb.Append("}");
            sb.Append("});");

            sb.Append("});");

            sb.Append("</script>");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "GraficoCaixaTrimestre_" + Guid.NewGuid(), sb.ToString(), false);
        }
        private void ConfigurarGraficoCaixaSemanal(DataSet dsDashBoard)
        {
            string selectedYear = string.Join(",", itensSelecionados);

            StringBuilder sb = new StringBuilder();
            sb.Append("<script>");
            sb.Append("$(document).ready(function() {");

            sb.Append("var canvas = document.getElementById('Grafico_Caixa_Semanal_v2');");
            sb.Append("if(!canvas) {");
            //sb.Append(" console.log(\"Canvas #Grafico_Caixa_Semanal_v2 não existe no DOM!\");");
            sb.Append("return;}");

            sb.Append("const data = {");
            sb.Append("labels: [");

            int selectedExibicao = Convert.ToInt32(ddlExibicao.SelectedValue);
            DateTime startDate, endDate;
            if (selectedExibicao == 6)
            {
                startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }
            else
            {
                startDate = DateTime.Today;
                if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                {
                    startDate = DateTime.Today.AddDays(-6);
                }
                else
                {
                    startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
                }
                endDate = startDate.AddDays(6);
            }

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                string dateString = date.ToString("dd/MM/yyyy");
                sb.Append("\"" + dateString + "\"");

                if (date < endDate)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("datasets: [{");
            sb.Append("label: 'Pagamentos',");
            sb.Append("data: [");

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                DataRow[] rowPagar = dsDashBoard.Tables[TAB_Graficos_Pagamento_Semana].Select($"DataCompleta = '{date.ToString("yyyy-MM-dd")}'");
                double valorPagar = (rowPagar.Length > 0) ? Convert.ToDouble(rowPagar[0]["TotalDiario"]) : 0;
                sb.Append(valorPagar.ToString("F2", CultureInfo.InvariantCulture));

                if (date < endDate)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("borderColor: 'rgba(255, 99, 132, 1)',");
            sb.Append("backgroundColor: 'rgba(245, 71, 71)'");
            sb.Append("}, {");
            sb.Append("label: 'Recebimentos',");
            sb.Append("data: [");

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                DataRow[] rowReceber = dsDashBoard.Tables[TAB_Graficos_Recebimento_Semana].Select($"DataCompleta = '{date.ToString("yyyy-MM-dd")}'");
                double valorReceber = (rowReceber.Length > 0) ? Convert.ToDouble(rowReceber[0]["TotalDiario"]) : 0;
                sb.Append(valorReceber.ToString("F2", CultureInfo.InvariantCulture));

                if (date < endDate)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("borderColor: 'rgba(75, 192, 192, 1)',");
            sb.Append("backgroundColor: 'rgba(34, 130, 239)'");
            sb.Append("}]");
            sb.Append("};");

            sb.Append("const config = {");
            sb.Append("type: 'bar',");
            sb.Append("data: data,");
            sb.Append("options: {");
            sb.Append("responsive: true,");
            sb.Append("maintainAspectRatio: false,");
            sb.Append("plugins: {");
            sb.Append("legend: { position: 'top' },");
            sb.Append("title: { display: true, text: 'Caixa: Pagamento e Recebimento Diário' },");
            sb.Append("tooltip: {");
            sb.Append(" callbacks: {");
            sb.Append("label: function(context) {");
            sb.Append(" let label = context.dataset.label || '';");
            sb.Append("let rawValue = context.parsed.y;");
            sb.Append(" let formattedValue = Number(rawValue).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });");
            sb.Append("return label + ': ' + formattedValue;");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("},");
            sb.Append("scales: {");
            sb.Append("x: {");
            sb.Append("ticks: { callback: function(value) { return data.labels[value]; } },");
            sb.Append("},");
            sb.Append("y: {");
            sb.Append("beginAtZero: true,");
            sb.Append("ticks: { callback: function(value) { return value.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }); } }");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("};");

            sb.Append("var ctx3 = document.getElementById('Grafico_Caixa_Semanal_v2').getContext('2d');");
            sb.Append("var graficoCaixaSemanal = new Chart(ctx3, config);");

            sb.Append("document.getElementById('Grafico_Caixa_Semanal_v2').addEventListener('click', function(evt) {");
            sb.Append("var activePoints = graficoCaixaSemanal.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);");
            sb.Append("if (activePoints.length) {");
            sb.Append("var firstPoint = activePoints[0];");
            sb.Append("var label = graficoCaixaSemanal.data.labels[firstPoint.index];");
            sb.Append("var datasetLabel = graficoCaixaSemanal.data.datasets[firstPoint.datasetIndex].label;");
            sb.Append("var ano = '" + selectedYear + "';");
            sb.Append("var empresa = '" + ddlidEmpresa.SelectedValue + "';");
            sb.Append("var baseUrlPagarSemana = '" + sLinkContasPagar + "';");
            sb.Append("var baseUrlReceberSemana = '" + sLinkContasReceber + "';");
            sb.Append("var dataCompleta = graficoCaixaSemanal.data.labels[firstPoint.index];");
            sb.Append("if (datasetLabel === 'Recebimentos') { window.open(baseUrlReceberSemana + 'CaixaRecebidosHoje&sData=' + dataCompleta + '&sAno=' + ano + '&sEmpresa=' + empresa, '_blank'); }");
            sb.Append("else if (datasetLabel === 'Pagamentos') { window.open(baseUrlPagarSemana + 'CaixaPagosHoje&sData=' + dataCompleta + '&sAno=' + ano + '&sEmpresa=' + empresa, '_blank'); }");
            sb.Append("}");
            sb.Append("});");

            string currentDateString = DateTime.Today.ToString("dd/MM/yyyy");
            sb.Append($"graficoCaixaSemanal.data.labels.forEach((label, index) => {{ if (label === '{currentDateString}') {{ graficoCaixaSemanal.getDatasetMeta(0).data[index].custom = {{ backgroundColor: '#000000', fontWeight: 'bold', fontSize: 13 }}; graficoCaixaSemanal.getDatasetMeta(1).data[index].custom = {{ backgroundColor: '#000000', fontWeight: 'bold', fontSize: 13 }}; }} }});");

            sb.Append("});");
            sb.Append("</script>");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "GraficoCaixaSemanal_" + Guid.NewGuid(), sb.ToString(), false);
        }


        private void ConfigurarGraficoEmprestimoAno(DataSet dsDashBoard)
        {
            string selectedYear = string.Join(",", itensSelecionados);

            StringBuilder sb = new StringBuilder();
            sb.Append("<script>");
            sb.Append("$(document).ready(function() {");
            sb.Append("const data = {");
            sb.Append("labels: [");

            int maxCount = Math.Max(dsDashBoard.Tables[TAB_Grafico_Emprestimo_Receber_Ano].Rows.Count, dsDashBoard.Tables[TAB_Grafico_Emprestimo_Pagar_Ano].Rows.Count);

            for (int i = 0; i < maxCount; i++)
            {
                int referencia = dsDashBoard.Tables[TAB_Grafico_Emprestimo_Receber_Ano].Rows.Count > dsDashBoard.Tables[TAB_Grafico_Emprestimo_Pagar_Ano].Rows.Count ? TAB_Grafico_Emprestimo_Receber_Ano : TAB_Grafico_Emprestimo_Pagar_Ano;
                string sMes = i < dsDashBoard.Tables[referencia].Rows.Count ? dsDashBoard.Tables[referencia].Rows[i]["sMes"].ToString() : "";
                sb.Append("\"" + sMes + "\"");

                if (i < maxCount - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("datasets: [{");
            sb.Append("label: 'Pagamentos Empréstimos',");
            sb.Append("data: [");

            for (int i = 0; i < maxCount; i++)
            {
                double valorPagar = i < dsDashBoard.Tables[TAB_Grafico_Emprestimo_Pagar_Ano].Rows.Count ? Convert.ToDouble(dsDashBoard.Tables[TAB_Grafico_Emprestimo_Pagar_Ano].Rows[i]["TotalMensal"]) : 0;
                sb.Append(valorPagar.ToString(CultureInfo.InvariantCulture));

                if (i < maxCount - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("borderColor: 'rgba(255, 0, 0, 1)',");
            sb.Append("backgroundColor: 'rgba(245, 71, 71 )'");
            sb.Append("}, {");
            sb.Append("label: 'Recebimentos Empréstimos',");
            sb.Append("data: [");

            for (int i = 0; i < maxCount; i++)
            {
                double valorReceber = i < dsDashBoard.Tables[TAB_Grafico_Emprestimo_Receber_Ano].Rows.Count ? Convert.ToDouble(dsDashBoard.Tables[TAB_Grafico_Emprestimo_Receber_Ano].Rows[i]["TotalMensal"]) : 0;
                sb.Append(valorReceber.ToString(CultureInfo.InvariantCulture));

                if (i < maxCount - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("borderColor: 'rgba(11, 98, 164, 1)',");
            sb.Append("backgroundColor: 'rgba(34, 130, 239)'");
            sb.Append("}]");
            sb.Append("};");

            sb.Append("const config = {");
            sb.Append("type: 'bar',");
            sb.Append("data: data,");
            sb.Append("options: {");
            sb.Append("responsive: true,");
            sb.Append("maintainAspectRatio: false,");
            sb.Append("plugins: {");
            sb.Append("legend: { position: 'top' },");
            sb.Append("title: { display: true, text: 'Empréstimo: Pagar / Receber Mensal' },");
            sb.Append("tooltip: {");
            sb.Append(" callbacks: {");
            sb.Append("label: function(context) {");
            sb.Append(" let label = context.dataset.label || '';");
            sb.Append("let rawValue = context.parsed.y;");
            sb.Append(" let formattedValue = Number(rawValue).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });");
            sb.Append("return label + ': ' + formattedValue;");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("},");
            sb.Append("scales: {");
            sb.Append("y: { beginAtZero: true, ticks: { callback: function(value) { return value.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }); } } }");
            sb.Append("}");
            sb.Append("}");
            sb.Append("};");

            sb.Append("var ctx4 = document.getElementById('Grafico_Emprestimo_Anual_v2').getContext('2d');");
            sb.Append("var graficoEmprestimoAno = new Chart(ctx4, config);");

            sb.Append("const currentMonthLabel = '" + GetMonthOfYear(DateTime.Now) + "';");
            sb.Append("graficoEmprestimoAno.data.labels.forEach((label, index) => {");
            sb.Append("if (label === currentMonthLabel) {");
            sb.Append("graficoEmprestimoAno.getDatasetMeta(0).data[index].custom = { backgroundColor: '#000000', fontWeight: 'bold', fontSize: 16 };");
            sb.Append("graficoEmprestimoAno.getDatasetMeta(1).data[index].custom = { backgroundColor: '#000000', fontWeight: 'bold', fontSize: 16 };");
            sb.Append("}");
            sb.Append("});");

            sb.Append("document.getElementById('Grafico_Emprestimo_Anual_v2').addEventListener('click', function(evt) {");
            sb.Append("var activePoints = graficoEmprestimoAno.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);");
            sb.Append("if (activePoints.length) {");
            sb.Append("var firstPoint = activePoints[0];");
            sb.Append("var label = graficoEmprestimoAno.data.labels[firstPoint.index];");
            sb.Append("var datasetLabel = graficoEmprestimoAno.data.datasets[firstPoint.datasetIndex].label;");
            sb.Append("var ano = '" + selectedYear + "';");
            sb.Append("var empresa = '" + ddlidEmpresa.SelectedValue + "';");
            sb.Append("var baseUrlPagar = '" + sLinkContasPagar + "';");
            sb.Append("var baseUrlReceber = '" + sLinkContasReceber + "';");
            sb.Append("var mesCompleto = label.substring(0, 7);");
            sb.Append("if (datasetLabel === 'Recebimentos Empréstimos') { window.open(baseUrlReceber + 'EmprestimoRecebidosMes&sMes=' + mesCompleto + '&sAno=' + ano + '&sEmpresa=' + empresa, '_blank'); }");
            sb.Append("else if (datasetLabel === 'Pagamentos Empréstimos') { window.open(baseUrlPagar + 'EmprestimoPagosMes&sMes=' + mesCompleto + '&sAno=' + ano + '&sEmpresa=' + empresa, '_blank'); }");
            sb.Append("}");
            sb.Append("});");

            sb.Append("});");
            sb.Append("</script>");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "GraficoEmprestimoAno_" + Guid.NewGuid(), sb.ToString(), false);
        }
        private void ConfigurarGraficoEmprestimoTrimestre(DataSet dsDashBoard)
        {
            string selectedYear = string.Join(",", itensSelecionados);

            StringBuilder sb = new StringBuilder();
            sb.Append("<script>");

            sb.Append("$(document).ready(function() {");

            sb.Append("var canvas = document.getElementById('Grafico_Emprestimo_Trimestre_v2');");
            sb.Append("if(!canvas) {");
            //sb.Append(" console.log(\"Canvas #Grafico_Emprestimo_Trimestre_v2 não existe no DOM!\");");
            sb.Append("return;}");

            sb.Append("const labels = [");

            int trimestreSelecionado;
            int[] semanasDoTrimestre;

            if (int.TryParse(ddlExibicao.SelectedValue, out trimestreSelecionado) && trimestreSelecionado >= 1 && trimestreSelecionado <= 4)
            {
                semanasDoTrimestre = GetSemanasPorTrimestre(trimestreSelecionado);
            }
            else
            {
                semanasDoTrimestre = GetSemanasTrimestreAtual();
            }

            for (int i = 0; i < dsDashBoard.Tables[TAB_Grafico_Emprestimo_Receber_Trimestre].Rows.Count; i++)
            {
                string sAno = dsDashBoard.Tables[TAB_Grafico_Emprestimo_Receber_Trimestre].Rows[i]["Ano"].ToString();
                string sSemana = dsDashBoard.Tables[TAB_Grafico_Emprestimo_Receber_Trimestre].Rows[i]["Semana"].ToString();
                if (semanasDoTrimestre.Contains(Convert.ToInt32(sSemana)))
                {
                    DateTime semanaInicio = CalcularPrimeiraDataDaSemanaISO(Convert.ToInt32(sAno), Convert.ToInt32(sSemana));
                    DateTime semanaFim = semanaInicio.AddDays(6);
                    string periodo = $"S{sSemana} {semanaInicio:dd/MM} - {semanaFim:dd/MM}";

                    sb.Append($"\"{periodo}\"");

                    if (i < dsDashBoard.Tables[TAB_Grafico_Emprestimo_Receber_Trimestre].Rows.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
            }
            sb.Append("];");

            sb.Append("const data = {");
            sb.Append("labels: labels,");
            sb.Append("datasets: [");

            sb.Append("{");
            sb.Append("label: 'Pagamentos Empréstimos',");
            sb.Append("data: [");
            for (int i = 0; i < dsDashBoard.Tables[TAB_Grafico_Emprestimo_Receber_Trimestre].Rows.Count; i++)
            {
                string sSemana = dsDashBoard.Tables[TAB_Grafico_Emprestimo_Receber_Trimestre].Rows[i]["Semana"].ToString();
                if (semanasDoTrimestre.Contains(Convert.ToInt32(sSemana)))
                {
                    double valorEmprestimoPagar = (i < dsDashBoard.Tables[TAB_Grafico_Emprestimo_Pagar_Trimestre].Rows.Count) ? Convert.ToDouble(dsDashBoard.Tables[TAB_Grafico_Emprestimo_Pagar_Trimestre].Rows[i]["TotalSemanal"]) : 0;
                    sb.Append(valorEmprestimoPagar.ToString(CultureInfo.InvariantCulture));

                    if (i < dsDashBoard.Tables[TAB_Grafico_Emprestimo_Receber_Trimestre].Rows.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
            }
            sb.Append("],");
            sb.Append("borderColor: 'rgba(255, 99, 132, 1)',");
            sb.Append("backgroundColor: 'rgba(245, 71, 71)'");
            sb.Append("},");

            sb.Append("{");
            sb.Append("label: 'Recebimentos Empréstimos',");
            sb.Append("data: [");
            for (int i = 0; i < dsDashBoard.Tables[TAB_Grafico_Emprestimo_Receber_Trimestre].Rows.Count; i++)
            {
                string sSemana = dsDashBoard.Tables[TAB_Grafico_Emprestimo_Receber_Trimestre].Rows[i]["Semana"].ToString();
                if (semanasDoTrimestre.Contains(Convert.ToInt32(sSemana)))
                {
                    double valorEmprestimoReceber = Convert.ToDouble(dsDashBoard.Tables[TAB_Grafico_Emprestimo_Receber_Trimestre].Rows[i]["TotalSemanal"]);
                    sb.Append(valorEmprestimoReceber.ToString(CultureInfo.InvariantCulture));

                    if (i < dsDashBoard.Tables[TAB_Grafico_Emprestimo_Receber_Trimestre].Rows.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
            }
            sb.Append("],");
            sb.Append("borderColor: 'rgba(75, 192, 192, 1)',");
            sb.Append("backgroundColor: 'rgba(34, 130, 239)'");
            sb.Append("}");

            sb.Append("]");
            sb.Append("};");

            sb.Append("const config = {");
            sb.Append("type: 'bar',");
            sb.Append("data: data,");
            sb.Append("options: {");
            sb.Append("responsive: true,");
            sb.Append("maintainAspectRatio: false,");
            sb.Append("plugins: {");
            sb.Append("legend: { position: 'top' },");
            sb.Append("title: { display: true, text: 'Empréstimo: Pagar / Receber Semanal' },");
            sb.Append("tooltip: {");
            sb.Append(" callbacks: {");
            sb.Append("label: function(context) {");
            sb.Append(" let label = context.dataset.label || '';");
            sb.Append("let rawValue = context.parsed.y;");
            sb.Append(" let formattedValue = Number(rawValue).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });");
            sb.Append("return label + ': ' + formattedValue;");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("},");
            sb.Append("scales: {");
            sb.Append("y: {");
            sb.Append("beginAtZero: true,");
            sb.Append("ticks: {");
            sb.Append("callback: function(value) {");
            sb.Append("return value.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("};");

            sb.Append("var ctx5 = document.getElementById('Grafico_Emprestimo_Trimestre_v2').getContext('2d');");
            sb.Append("var graficoEmprestimoTrimeste = new Chart(ctx5, config);");

            sb.Append("document.getElementById('Grafico_Emprestimo_Trimestre_v2').addEventListener('click', function(evt) {");
            sb.Append("var activePoints = graficoEmprestimoTrimeste.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);");
            sb.Append("if (activePoints.length) {");
            sb.Append("var firstPoint = activePoints[0];");
            sb.Append("var label = graficoEmprestimoTrimeste.data.labels[firstPoint.index];");
            sb.Append("var datasetLabel = graficoEmprestimoTrimeste.data.datasets[firstPoint.datasetIndex].label;");
            sb.Append("var semana = label.split(' ')[0].replace('S', '').trim();");
            sb.Append("var ano = '" + selectedYear + "';");
            sb.Append("var empresa = '" + ddlidEmpresa.SelectedValue + "';");
            sb.Append("var baseUrlPagar = '" + sLinkContasPagar + "';");
            sb.Append("var baseUrlReceber = '" + sLinkContasReceber + "';");
            sb.Append("var url = (datasetLabel === 'Recebimentos Empréstimos') ? baseUrlReceber + 'EmprestimoRecebidosSemana&sSemana=' + semana + '&sAno=' + ano + '&sEmpresa=' + empresa : baseUrlPagar + 'EmprestimoPagosSemana&sSemana=' + semana + '&sAno=' + ano + '&sEmpresa=' + empresa;");
            sb.Append("window.open(url, '_blank');");
            sb.Append("}");
            sb.Append("});");

            sb.Append("});");

            sb.Append("</script>");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "GraficoEmprestimoTrimestral_" + Guid.NewGuid(), sb.ToString(), false);
        }
        private void ConfigurarGraficoEmprestimoSemana(DataSet dsDashBoard)
        {
            string selectedYear = string.Join(",", itensSelecionados);

            StringBuilder sb = new StringBuilder();
            sb.Append("<script>");
            sb.Append("$(document).ready(function() {");

            sb.Append("var canvas = document.getElementById('Grafico_Emprestimo_Semana_v2');");
            sb.Append("if(!canvas) {");
            //sb.Append(" console.log(\"Canvas #Grafico_Emprestimo_Semana_v2 não existe no DOM!\");");
            sb.Append("return;}");

            sb.Append("const data = {");
            sb.Append("labels: [");

            int selectedExibicao = Convert.ToInt32(ddlExibicao.SelectedValue);
            DateTime startDate, endDate;
            if (selectedExibicao == 6)
            {
                startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }
            else
            {
                startDate = DateTime.Today;
                if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                {
                    startDate = DateTime.Today.AddDays(-6);
                }
                else
                {
                    startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
                }
                endDate = startDate.AddDays(6);
            }

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                string dateString = date.ToString("dd/MM/yyyy");
                sb.Append("\"" + dateString + "\"");

                if (date < endDate)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("datasets: [{");
            sb.Append("label: 'Pagamentos Empréstimos',");
            sb.Append("data: [");

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                DataRow[] rowPagar = dsDashBoard.Tables[TAB_Grafico_Emprestimo_Pagar_Semana].Select($"DataCompleta = '{date.ToString("yyyy-MM-dd")}'");
                double valorPagar = (rowPagar.Length > 0) ? Convert.ToDouble(rowPagar[0]["TotalDiario"]) : 0;
                sb.Append(valorPagar.ToString("F2", CultureInfo.InvariantCulture));

                if (date < endDate)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("borderColor: 'rgba(255, 99, 132, 1)',");
            sb.Append("backgroundColor: 'rgba(245, 71, 71)'");
            sb.Append("}, {");
            sb.Append("label: 'Recebimentos Empréstimos',");
            sb.Append("data: [");

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                DataRow[] rowReceber = dsDashBoard.Tables[TAB_Grafico_Emprestimo_Receber_Semana].Select($"DataCompleta = '{date.ToString("yyyy-MM-dd")}'");
                double valorReceber = (rowReceber.Length > 0) ? Convert.ToDouble(rowReceber[0]["TotalDiario"]) : 0;
                sb.Append(valorReceber.ToString("F2", CultureInfo.InvariantCulture));

                if (date < endDate)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("borderColor: 'rgba(75, 192, 192, 1)',");
            sb.Append("backgroundColor: 'rgba(34, 130, 239)'");
            sb.Append("}]");
            sb.Append("};");

            sb.Append("const config = {");
            sb.Append("type: 'bar',");
            sb.Append("data: data,");
            sb.Append("options: {");
            sb.Append("responsive: true,");
            sb.Append("maintainAspectRatio: false,");
            sb.Append("plugins: {");
            sb.Append("legend: { position: 'top' },");
            sb.Append("title: { display: true, text: 'Empréstimo: Pagar / Receber Diário' },");
            sb.Append("tooltip: {");
            sb.Append(" callbacks: {");
            sb.Append("label: function(context) {");
            sb.Append(" let label = context.dataset.label || '';");
            sb.Append("let rawValue = context.parsed.y;");
            sb.Append(" let formattedValue = Number(rawValue).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });");
            sb.Append("return label + ': ' + formattedValue;");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("},");
            sb.Append("scales: {");
            sb.Append("x: {");
            sb.Append("ticks: { callback: function(value) { return data.labels[value]; } },");
            sb.Append("},");
            sb.Append("y: {");
            sb.Append("beginAtZero: true,");
            sb.Append("ticks: { callback: function(value) { return value.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }); } }");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("};");

            sb.Append("var ctx6 = document.getElementById('Grafico_Emprestimo_Semana_v2').getContext('2d');");
            sb.Append("var graficoEmprestimoSemana = new Chart(ctx6, config);");

            sb.Append("document.getElementById('Grafico_Emprestimo_Semana_v2').addEventListener('click', function(evt) {");
            sb.Append("var activePoints = graficoEmprestimoSemana.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);");
            sb.Append("if (activePoints.length) {");
            sb.Append("var firstPoint = activePoints[0];");
            sb.Append("var label = graficoEmprestimoSemana.data.labels[firstPoint.index];");
            sb.Append("var datasetLabel = graficoEmprestimoSemana.data.datasets[firstPoint.datasetIndex].label;");
            sb.Append("var ano = '" + selectedYear + "';");
            sb.Append("var empresa = '" + ddlidEmpresa.SelectedValue + "';");
            sb.Append("var baseUrlPagarEmprestimo = '" + sLinkContasPagar + "';");
            sb.Append("var baseUrlReceberEmprestimo = '" + sLinkContasReceber + "';");
            sb.Append("if (datasetLabel === 'Recebimentos Empréstimos') { window.open(baseUrlReceberEmprestimo + 'EmprestimoRecebidosHoje&sData=' + label + '&sAno=' + ano + '&sEmpresa=' + empresa, '_blank'); }");
            sb.Append("else if (datasetLabel === 'Pagamentos Empréstimos') { window.open(baseUrlPagarEmprestimo + 'EmprestimoPagosHoje&sData=' + label + '&sAno=' + ano + '&sEmpresa=' + empresa, '_blank'); }");
            sb.Append("}");
            sb.Append("});");

            string currentDateString = DateTime.Today.ToString("dd/MM/yyyy");
            sb.Append($"graficoEmprestimoSemana.data.labels.forEach((label, index) => {{ if (label === '{currentDateString}') {{ graficoEmprestimoSemana.getDatasetMeta(0).data[index].custom = {{ backgroundColor: '#000000', fontWeight: 'bold', fontSize: 13 }}; graficoEmprestimoSemana.getDatasetMeta(1).data[index].custom = {{ backgroundColor: '#000000', fontWeight: 'bold', fontSize: 13 }}; }} }});");

            sb.Append("});");
            sb.Append("</script>");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "GraficoEmprestimoSemanal_" + Guid.NewGuid(), sb.ToString(), false);
        }


        private void ConfigurarGraficoComparativoAno(DataSet dsDashBoard)
        {
            string selectedYear = string.Join(",", itensSelecionados);

            StringBuilder sb = new StringBuilder();
            sb.Append("<script>");
            sb.Append("$(document).ready(function() {");
            sb.Append("const data = {");
            sb.Append("labels: [");

            int maxCount = Math.Max(dsDashBoard.Tables[TAB_Grafico_Comparativo_Receber_Ano].Rows.Count, dsDashBoard.Tables[TAB_Grafico_Comparativo_Pagar_Ano].Rows.Count);
            maxCount = Math.Max(maxCount, dsDashBoard.Tables[TAB_Grafico_Comparativo_Meta_Ano].Rows.Count);
            int referencia = (dsDashBoard.Tables[TAB_Grafico_Comparativo_Receber_Ano].Rows.Count > dsDashBoard.Tables[TAB_Grafico_Comparativo_Pagar_Ano].Rows.Count) ? TAB_Grafico_Comparativo_Receber_Ano : TAB_Grafico_Comparativo_Pagar_Ano;
            referencia = referencia > dsDashBoard.Tables[TAB_Grafico_Comparativo_Meta_Ano].Rows.Count ? referencia : TAB_Grafico_Comparativo_Meta_Ano;

            for (int i = 0; i < maxCount; i++)
            {
                string sMes = "";

                if (i < dsDashBoard.Tables[referencia].Rows.Count)
                {
                    sMes = dsDashBoard.Tables[referencia].Rows[i]["sMes"].ToString();
                }

                sb.Append("\"" + sMes + "\"");

                if (i < maxCount - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("datasets: [{");
            sb.Append("label: 'Total Despesa',");
            sb.Append("data: [");

            for (int i = 0; i < maxCount; i++)
            {
                double valorPagar = 0;
                if (i < dsDashBoard.Tables[TAB_Grafico_Comparativo_Pagar_Ano].Rows.Count)
                {
                    valorPagar = Convert.ToDouble(dsDashBoard.Tables[TAB_Grafico_Comparativo_Pagar_Ano].Rows[i]["TotalAcumulado"]);
                }

                sb.Append(valorPagar.ToString(CultureInfo.InvariantCulture));

                if (i < maxCount - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("borderColor: 'rgba(255, 99, 132, 1)',");
            sb.Append("backgroundColor: 'rgba(245, 71, 71)'");
            sb.Append("}, {");
            sb.Append("label: 'Total Receita',");
            sb.Append("data: [");

            for (int i = 0; i < maxCount; i++)
            {
                double valorReceber = 0;
                if (i < dsDashBoard.Tables[TAB_Grafico_Comparativo_Receber_Ano].Rows.Count)
                {
                    valorReceber = Convert.ToDouble(dsDashBoard.Tables[TAB_Grafico_Comparativo_Receber_Ano].Rows[i]["TotalAcumulado"]);
                }

                sb.Append(valorReceber.ToString(CultureInfo.InvariantCulture));

                if (i < maxCount - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("borderColor: 'rgba(75, 192, 192, 1)',");
            sb.Append("backgroundColor: 'rgba(34, 130, 239 )'");
            sb.Append("}, {");
            sb.Append("label: 'Meta Anual',");
            sb.Append("data: [");

            for (int i = 0; i < maxCount; i++)
            {
                double valorMeta = 0;
                if (i < dsDashBoard.Tables[TAB_Grafico_Comparativo_Meta_Ano].Rows.Count)
                {
                    valorMeta = Convert.ToDouble(dsDashBoard.Tables[TAB_Grafico_Comparativo_Meta_Ano].Rows[i]["MetaAcumulada"]);
                }

                sb.Append(valorMeta.ToString(CultureInfo.InvariantCulture));

                if (i < maxCount - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("borderColor: 'rgba(54, 162, 235, 1)',");
            sb.Append("backgroundColor: 'rgba(204, 217, 213)'");
            sb.Append("}]");
            sb.Append("};");

            sb.Append("const config = {");
            sb.Append("type: 'bar',");
            sb.Append("data: data,");
            sb.Append("options: {");
            sb.Append("responsive: true,");
            sb.Append("maintainAspectRatio: false,");
            sb.Append("plugins: {");
            sb.Append("legend: {");
            sb.Append("position: 'top',");
            sb.Append("},");
            sb.Append("title: {");
            sb.Append("display: true,");
            sb.Append("text: 'Comparativo: Recebimento e Pagamento Mensal'");
            sb.Append("},");
            sb.Append("tooltip: {");
            sb.Append(" callbacks: {");
            sb.Append("label: function(context) {");
            sb.Append(" let label = context.dataset.label || '';");
            sb.Append("let rawValue = context.parsed.y;");
            sb.Append(" let formattedValue = Number(rawValue).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });");
            sb.Append("return label + ': ' + formattedValue;");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("},");
            sb.Append("layout: { padding: { left: 10, right: 10, top: 10, bottom: 10 } }");
            sb.Append("}");
            sb.Append("};");

            sb.Append("var ctx7 = document.getElementById('Grafico_Comparativo_Ano_V2').getContext('2d');");
            sb.Append("var graficoComparativoAno = new Chart(ctx7, config);");

            sb.Append("document.getElementById('Grafico_Comparativo_Ano_V2').addEventListener('click', function(evt) {");
            sb.Append("var activePoints = graficoComparativoAno.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);");
            sb.Append("if (activePoints.length) {");
            sb.Append("var firstPoint = activePoints[0];");
            sb.Append("var label = graficoComparativoAno.data.labels[firstPoint.index];");
            sb.Append("var datasetLabel = graficoComparativoAno.data.datasets[firstPoint.datasetIndex].label;");
            sb.Append("var ano = '" + selectedYear + "';");
            sb.Append("var empresa = '" + ddlidEmpresa.SelectedValue + "';");
            sb.Append("var baseUrlPagar = '" + sLinkContasPagar + "';");
            sb.Append("var baseUrlReceber = '" + sLinkContasReceber + "';");
           
            sb.Append("var mesCompleto = label.substring(0, 7);");
            sb.Append("if (datasetLabel === 'Total Receita') { window.open(baseUrlReceber + 'ComparativoRecebidosMes&sMes=' + mesCompleto + '&sAno=' + ano + '&sEmpresa=' + empresa, '_blank'); }");
            sb.Append("else if (datasetLabel === 'Total Despesa') { window.open(baseUrlPagar + 'ComparativoPagosMes&sMes=' + mesCompleto + '&sAno=' + ano + '&sEmpresa=' + empresa, '_blank'); }");
            sb.Append("}");
            sb.Append("});");

            sb.Append("});");
            sb.Append("</script>");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "GraficoAnoCombinado_" + Guid.NewGuid(), sb.ToString(), false);
        }
        private void ConfigurarGraficoComparativoTrimestre(DataSet dsDashBoard)
        {
            //StringBuilder sb = new StringBuilder();
            //sb.Append("<script>");

            //sb.Append("document.addEventListener('DOMContentLoaded', function() {");

            //// Labels
            //sb.Append("const labels = [");
            //int trimestreSelecionado;
            //int[] semanasDoTrimestre;

            //if (int.TryParse(ddlExibicao.SelectedValue, out trimestreSelecionado) && trimestreSelecionado >= 1 && trimestreSelecionado <= 4)
            //{
            //    semanasDoTrimestre = GetSemanasPorTrimestre(trimestreSelecionado);
            //}
            //else
            //{
            //    semanasDoTrimestre = GetSemanasTrimestreAtual();
            //}

            //for (int i = 0; i < dsDashBoard.Tables[TAB_Grafico_Comparativo_Receber_Trimestre].Rows.Count; i++)
            //{
            //    string sAno = dsDashBoard.Tables[TAB_Grafico_Comparativo_Receber_Trimestre].Rows[i]["Ano"].ToString();
            //    string sSemana = dsDashBoard.Tables[TAB_Grafico_Comparativo_Receber_Trimestre].Rows[i]["Semana"].ToString();
            //    if (semanasDoTrimestre.Contains(Convert.ToInt32(sSemana)))
            //    {
            //        DateTime semanaInicio = CalcularPrimeiraDataDaSemanaISO(Convert.ToInt32(sAno), Convert.ToInt32(sSemana));
            //        DateTime semanaFim = semanaInicio.AddDays(6);
            //        string periodo = $"S{sSemana} {semanaInicio:dd/MM} - {semanaFim:dd/MM}";

            //        sb.Append($"\"{periodo}\"");
            //        if (i < dsDashBoard.Tables[TAB_Grafico_Comparativo_Receber_Trimestre].Rows.Count - 1)
            //        {
            //            sb.Append(", ");
            //        }
            //    }
            //}
            //sb.Append("];");

            //// Data
            //sb.Append("const data = {");
            //sb.Append("labels: labels,");
            //sb.Append("datasets: [");

            //sb.Append("{");
            //sb.Append("label: 'Despesa',");
            //sb.Append("data: [");
            //for (int i = 0; i < dsDashBoard.Tables[TAB_Grafico_Comparativo_Receber_Trimestre].Rows.Count; i++)
            //{
            //    string sSemana = dsDashBoard.Tables[TAB_Grafico_Comparativo_Receber_Trimestre].Rows[i]["Semana"].ToString();
            //    if (semanasDoTrimestre.Contains(Convert.ToInt32(sSemana)))
            //    {
            //        double valorComparativoPagar = (i < dsDashBoard.Tables[TAB_Grafico_Comparativo_Pagar_Trimestre].Rows.Count) ? Convert.ToDouble(dsDashBoard.Tables[TAB_Grafico_Comparativo_Pagar_Trimestre].Rows[i]["TotalSemanal"]) : 0;
            //        sb.Append(valorComparativoPagar.ToString(CultureInfo.InvariantCulture));
            //        if (i < dsDashBoard.Tables[TAB_Grafico_Comparativo_Receber_Trimestre].Rows.Count - 1)
            //        {
            //            sb.Append(", ");
            //        }
            //    }
            //}
            //sb.Append("],");
            //sb.Append("borderColor: 'rgba(255, 99, 132, 1)',");
            //sb.Append("backgroundColor: 'rgba(245, 71, 71)'");
            //sb.Append("},");

            //sb.Append("{");
            //sb.Append("label: 'Receita',");
            //sb.Append("data: [");
            //for (int i = 0; i < dsDashBoard.Tables[TAB_Grafico_Comparativo_Receber_Trimestre].Rows.Count; i++)
            //{
            //    string sSemana = dsDashBoard.Tables[TAB_Grafico_Comparativo_Receber_Trimestre].Rows[i]["Semana"].ToString();
            //    if (semanasDoTrimestre.Contains(Convert.ToInt32(sSemana)))
            //    {
            //        double valorComparativoReceber = Convert.ToDouble(dsDashBoard.Tables[TAB_Grafico_Comparativo_Receber_Trimestre].Rows[i]["TotalSemanal"]);
            //        sb.Append(valorComparativoReceber.ToString(CultureInfo.InvariantCulture));
            //        if (i < dsDashBoard.Tables[TAB_Grafico_Comparativo_Receber_Trimestre].Rows.Count - 1)
            //        {
            //            sb.Append(", ");
            //        }
            //    }
            //}
            //sb.Append("],");
            //sb.Append("borderColor: 'rgba(75, 192, 192, 1)',");
            //sb.Append("backgroundColor: 'rgba(34, 130, 239)'");
            //sb.Append("}");

            //sb.Append("]");
            //sb.Append("};");

            //sb.Append("const config = {");
            //sb.Append("type: 'bar',");
            //sb.Append("data: data,");
            //sb.Append("options: {");
            //sb.Append("responsive: true,");
            //sb.Append("maintainAspectRatio: false,");
            //sb.Append("plugins: {");
            //sb.Append("legend: { position: 'top' },");
            //sb.Append("title: { display: true, text: 'Comparativo: Recebimento e Pagamento Semanal' }");
            //sb.Append("},");
            //sb.Append("scales: {");
            //sb.Append("y: {");
            //sb.Append("beginAtZero: true,");
            //sb.Append("ticks: {");
            //sb.Append("callback: function(value) {");
            //sb.Append("return value.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });");
            //sb.Append("}");
            //sb.Append("}");
            //sb.Append("}");
            //sb.Append("}");
            //sb.Append("}");
            //sb.Append("};");

            //sb.Append("const ctx = document.getElementById('Grafico_Comparativo_Trimestre_V2').getContext('2d');");
            //sb.Append("const myChart = new Chart(ctx, config);");

            //sb.Append("document.getElementById('Grafico_Comparativo_Trimestre_V2').addEventListener('click', function(evt) {");
            //sb.Append("var activePoints = myChart.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);");
            //sb.Append("if (activePoints.length) {");
            //sb.Append("var firstPoint = activePoints[0];");
            //sb.Append("var label = myChart.data.labels[firstPoint.index];");
            //sb.Append("var datasetLabel = myChart.data.datasets[firstPoint.datasetIndex].label;");
            //sb.Append("var semana = label.split(' ')[0].replace('S', '').trim();");
            //sb.Append("var ano = '" + ddlAnoFinanceiro.SelectedValue + "';");
            //sb.Append("var empresa = '" + ddlidEmpresa.SelectedValue + "';");
            //sb.Append("var baseUrlPagar = '" + sLinkContasPagar + "';");
            //sb.Append("var baseUrlReceber = '" + sLinkContasReceber + "';");
            //sb.Append("var url = (datasetLabel === 'Receita') ? baseUrlReceber + 'ComparativoRecebidosSemana&sSemana=' + semana + '&sAno=' + ano + '&sEmpresa=' + empresa : baseUrlPagar + 'ComparativoPagosSemana&sSemana=' + semana + '&sAno=' + ano + '&sEmpresa=' + empresa;");
            //sb.Append("window.location.href = url;");
            //sb.Append("}");
            //sb.Append("});");

            //sb.Append("});");

            //sb.Append("</script>");

            //ScriptManager.RegisterStartupScript(this, this.GetType(), "GraficoComparativoTrimestral_" + Guid.NewGuid(), sb.ToString(), false);
        }
        private void ConfigurarGraficoComparativoSemana(DataSet dsDashBoard)
        {
            //StringBuilder sb = new StringBuilder();
            //sb.Append("<script>");
            //sb.Append("document.addEventListener('DOMContentLoaded', function() {");
            //sb.Append("const data = {");
            //sb.Append("labels: [");

            //int selectedExibicao = Convert.ToInt32(ddlExibicao.SelectedValue);
            //DateTime startDate, endDate;
            //if (selectedExibicao == 6)
            //{
            //    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            //    endDate = startDate.AddMonths(1).AddDays(-1);
            //}
            //else
            //{
            //    startDate = DateTime.Today;
            //    if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
            //    {
            //        startDate = DateTime.Today.AddDays(-6);
            //    }
            //    else
            //    {
            //        startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            //    }
            //    endDate = startDate.AddDays(6);
            //}

            //for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            //{
            //    string dateString = date.ToString("dd/MM/yyyy");
            //    sb.Append("\"" + dateString + "\"");

            //    if (date < endDate)
            //    {
            //        sb.Append(", ");
            //    }
            //}

            //sb.Append("],");
            //sb.Append("datasets: [{");
            //sb.Append("label: 'Despesa',");
            //sb.Append("data: [");

            //for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            //{
            //    DataRow[] rowPagar = dsDashBoard.Tables[TAB_Grafico_Comparativo_Pagar_Semana].Select($"DataCompleta = '{date.ToString("yyyy-MM-dd")}'");
            //    double valorPagar = (rowPagar.Length > 0) ? Convert.ToDouble(rowPagar[0]["TotalDiario"]) : 0;
            //    sb.Append(valorPagar.ToString("F2", CultureInfo.InvariantCulture));

            //    if (date < endDate)
            //    {
            //        sb.Append(", ");
            //    }
            //}

            //sb.Append("],");
            //sb.Append("borderColor: 'rgba(255, 99, 132, 1)',");
            //sb.Append("backgroundColor: 'rgba(245, 71, 71)'");
            //sb.Append("}, {");
            //sb.Append("label: 'Receita',");
            //sb.Append("data: [");

            //for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            //{
            //    DataRow[] rowReceber = dsDashBoard.Tables[TAB_Grafico_Comparativo_Receber_Semana].Select($"DataCompleta = '{date.ToString("yyyy-MM-dd")}'");
            //    double valorReceber = (rowReceber.Length > 0) ? Convert.ToDouble(rowReceber[0]["TotalDiario"]) : 0;
            //    sb.Append(valorReceber.ToString("F2", CultureInfo.InvariantCulture));

            //    if (date < endDate)
            //    {
            //        sb.Append(", ");
            //    }
            //}

            //sb.Append("],");
            //sb.Append("borderColor: 'rgba(75, 192, 192, 1)',");
            //sb.Append("backgroundColor: 'rgba(34, 130, 239)'");
            //sb.Append("}]");
            //sb.Append("};");

            //sb.Append("const config = {");
            //sb.Append("type: 'bar',");
            //sb.Append("data: data,");
            //sb.Append("options: {");
            //sb.Append("responsive: true,");
            //sb.Append("maintainAspectRatio: false,");
            //sb.Append("plugins: {");
            //sb.Append("legend: { position: 'top' },");
            //sb.Append("title: { display: true, text: 'Comparativo: Recebimento e Pagamento Diário' }");
            //sb.Append("},");
            //sb.Append("scales: {");
            //sb.Append("x: {");
            //sb.Append("ticks: { callback: function(value) { return data.labels[value]; } },");
            //sb.Append("},");
            //sb.Append("y: {");
            //sb.Append("beginAtZero: true,");
            //sb.Append("ticks: { callback: function(value) { return value.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }); } }");
            //sb.Append("}");
            //sb.Append("}");
            //sb.Append("}");
            //sb.Append("};");

            //sb.Append("const ctx = document.getElementById('Grafico_Comparativo_Semana_V2').getContext('2d');");
            //sb.Append("const myChart = new Chart(ctx, config);");

            //sb.Append("document.getElementById('Grafico_Comparativo_Semana_V2').addEventListener('click', function(evt) {");
            //sb.Append("var activePoints = myChart.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);");
            //sb.Append("if (activePoints.length) {");
            //sb.Append("var firstPoint = activePoints[0];");
            //sb.Append("var label = myChart.data.labels[firstPoint.index];");
            //sb.Append("var datasetLabel = myChart.data.datasets[firstPoint.datasetIndex].label;");
            //sb.Append("var ano = '" + ddlAnoFinanceiro.SelectedValue + "';");
            //sb.Append("var empresa = '" + ddlidEmpresa.SelectedValue + "';");
            //sb.Append("var baseUrlPagar = '" + sLinkContasPagar + "';");
            //sb.Append("var baseUrlReceber = '" + sLinkContasReceber + "';");
            //sb.Append("var dataCompleta = myChart.data.labels[firstPoint.index];");
            //sb.Append("if (datasetLabel === 'Receita') { window.location.href = baseUrlReceber + 'ComparativoRecebidosHoje&sData=' + dataCompleta + '&sAno=' + ano + '&sEmpresa=' + empresa; }");
            //sb.Append("else if (datasetLabel === 'Despesa') { window.location.href = baseUrlPagar + 'ComparativoPagosHoje&sData=' + dataCompleta + '&sAno=' + ano + '&sEmpresa=' + empresa; }");
            //sb.Append("}");
            //sb.Append("});");

            //string currentDateString = DateTime.Today.ToString("dd/MM/yyyy");
            //sb.Append($"myChart.data.labels.forEach((label, index) => {{ if (label === '{currentDateString}') {{ myChart.getDatasetMeta(0).data[index].custom = {{ backgroundColor: '#000000', fontWeight: 'bold', fontSize: 13 }}; myChart.getDatasetMeta(1).data[index].custom = {{ backgroundColor: '#000000', fontWeight: 'bold', fontSize: 13 }}; }} }});");

            //sb.Append("});");
            //sb.Append("</script>");

            //ScriptManager.RegisterStartupScript(this, this.GetType(), "GraficoComparativoSemanal_" + Guid.NewGuid(), sb.ToString(), false);
        }


        private void ConfigurarGraficoContabilAno(DataSet dsDashBoard)
        {
            string selectedYear = string.Join(",", itensSelecionados);

            StringBuilder sb = new StringBuilder();
            sb.Append("<script>");
            sb.Append("$(document).ready(function() {");
            sb.Append("const data = {");
            sb.Append("labels: [");

            int maxCount = Math.Max(dsDashBoard.Tables[TAB_Graficos_Contabil_Receber_Ano].Rows.Count, dsDashBoard.Tables[TAB_Graficos_Contabil_Pagar_Ano].Rows.Count);
            int referencia = dsDashBoard.Tables[TAB_Graficos_Contabil_Receber_Ano].Rows.Count > dsDashBoard.Tables[TAB_Graficos_Contabil_Pagar_Ano].Rows.Count ? TAB_Graficos_Contabil_Receber_Ano : TAB_Graficos_Contabil_Pagar_Ano;

            for (int i = 0; i < maxCount; i++)
            {
                string sMes = i < dsDashBoard.Tables[referencia].Rows.Count
                    ? DateTime.Parse(dsDashBoard.Tables[referencia].Rows[i]["sMes"].ToString()).ToString("yyyy-MM")
                    : "";
                sb.Append("\"" + sMes + "\"");

                if (i < maxCount - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("datasets: [{");
            sb.Append("label: 'Despesa Mês',");
            sb.Append("data: [");

            for (int i = 0; i < maxCount; i++)
            {
                double valorPagar = i < dsDashBoard.Tables[TAB_Graficos_Contabil_Pagar_Ano].Rows.Count ? Convert.ToDouble(dsDashBoard.Tables[TAB_Graficos_Contabil_Pagar_Ano].Rows[i]["TotalMensal"]) : 0;
                sb.Append(valorPagar.ToString(CultureInfo.InvariantCulture));

                if (i < maxCount - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("borderColor: 'rgba(255, 99, 132, 1)',");
            sb.Append("backgroundColor: 'rgba(245, 71, 71, 1)'");
            sb.Append("}, {");
            sb.Append("label: 'Receita Mês',");
            sb.Append("data: [");

            for (int i = 0; i < maxCount; i++)
            {
                double valorReceber = i < dsDashBoard.Tables[TAB_Graficos_Contabil_Receber_Ano].Rows.Count ? Convert.ToDouble(dsDashBoard.Tables[TAB_Graficos_Contabil_Receber_Ano].Rows[i]["TotalMensal"]) : 0;
                sb.Append(valorReceber.ToString(CultureInfo.InvariantCulture));

                if (i < maxCount - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("borderColor: 'rgba(75, 192, 192, 1)',");
            sb.Append("backgroundColor: 'rgba(34, 130, 239, 1)'");
            sb.Append("}]");
            sb.Append("};");

            sb.Append("const config = {");
            sb.Append("type: 'bar',");
            sb.Append("data: data,");
            sb.Append("options: {");
            sb.Append("responsive: true,");
            sb.Append("maintainAspectRatio: false,");
            sb.Append("plugins: {");
            sb.Append("legend: { position: 'top' },");
            sb.Append("title: { display: true, text: 'Contábil: Receita/Despesas Mensal' },");
            sb.Append("tooltip: {");
            sb.Append(" callbacks: {");
            sb.Append("label: function(context) {");
            sb.Append(" let label = context.dataset.label || '';");
            sb.Append("let rawValue = context.parsed.y;");
            sb.Append(" let formattedValue = Number(rawValue).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });");
            sb.Append("return label + ': ' + formattedValue;");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("},");
            sb.Append("scales: {");
            sb.Append("y: { beginAtZero: true, ticks: { callback: function(value) { return value.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }); } } }");
            sb.Append("}");
            sb.Append("}");
            sb.Append("};");

            sb.Append("var ctx8 = document.getElementById('Grafico_Contabil_Ano_v2').getContext('2d');");
            sb.Append("var graficoContabilAno = new Chart(ctx8, config);");

            sb.Append("const currentMonthLabel = '" + DateTime.Now.ToString("yyyy-MM") + "';");
            sb.Append("graficoContabilAno.data.labels.forEach((label, index) => {");
            sb.Append("if (label === currentMonthLabel) {");
            sb.Append("graficoContabilAno.getDatasetMeta(0).data[index].custom = { backgroundColor: '#000000', fontWeight: 'bold', fontSize: 16 };");
            sb.Append("graficoContabilAno.getDatasetMeta(1).data[index].custom = { backgroundColor: '#000000', fontWeight: 'bold', fontSize: 16 };");
            sb.Append("}");
            sb.Append("});");

            sb.Append("document.getElementById('Grafico_Contabil_Ano_v2').addEventListener('click', function(evt) {");
            sb.Append("var activePoints = graficoContabilAno.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);");
            sb.Append("if (activePoints.length) {");
            sb.Append("var firstPoint = activePoints[0];");
            sb.Append("var label = graficoContabilAno.data.labels[firstPoint.index];");
            sb.Append("var datasetLabel = graficoContabilAno.data.datasets[firstPoint.datasetIndex].label;");
            sb.Append("var ano = '" + selectedYear + "';");
            sb.Append("var empresa = '" + ddlidEmpresa.SelectedValue + "';");
            sb.Append("var baseUrlPagar = '" + sLinkContasPagar + "';");
            sb.Append("var baseUrlReceber = '" + sLinkContasReceber + "';");
            sb.Append("var mesCompleto = label.substring(0, 7);");
            sb.Append("if (datasetLabel === 'Receita Mês') { window.open(baseUrlReceber + 'ContabilRecebidosMes&sMes=' + mesCompleto + '&sAno=' + ano + '&sEmpresa=' + empresa, '_blank'); }");
            sb.Append("else if (datasetLabel === 'Despesa Mês') { window.open(baseUrlPagar + 'ContabilPagosMes&sMes=' + mesCompleto + '&sAno=' + ano + '&sEmpresa=' + empresa, '_blank'); }");
            sb.Append("}");
            sb.Append("});");

            sb.Append("});");
            sb.Append("</script>");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "GraficoContabil_" + Guid.NewGuid(), sb.ToString(), false);
        }
        private void ConfigurarGraficoContabilTrimestre(DataSet dsDashBoard)
        {
            string selectedYear = string.Join(",", itensSelecionados);

            StringBuilder sb = new StringBuilder();
            sb.Append("<script>");

            sb.Append("$(document).ready(function() {");

            sb.Append("var canvas = document.getElementById('Grafico_Contabil_Trimestre_V2');");
            sb.Append("if(!canvas) {");
            //sb.Append(" console.log(\"Canvas #Grafico_Contabil_Trimestre_V2 não existe no DOM!\");");
            sb.Append("return;}");

            sb.Append("const labels = [");

            int trimestreSelecionado;
            int[] semanasDoTrimestre;

            if (int.TryParse(ddlExibicao.SelectedValue, out trimestreSelecionado) && trimestreSelecionado >= 1 && trimestreSelecionado <= 4)
            {
                semanasDoTrimestre = GetSemanasPorTrimestre(trimestreSelecionado);
            }
            else
            {
                semanasDoTrimestre = GetSemanasTrimestreAtual();
            }

            for (int i = 0; i < dsDashBoard.Tables[TAB_Graficos_Contabil_Receber_Trimestre].Rows.Count; i++)
            {
                string sAno = dsDashBoard.Tables[TAB_Graficos_Contabil_Receber_Trimestre].Rows[i]["Ano"].ToString();
                string sSemana = dsDashBoard.Tables[TAB_Graficos_Contabil_Receber_Trimestre].Rows[i]["Semana"].ToString();
                if (semanasDoTrimestre.Contains(Convert.ToInt32(sSemana)))
                {
                    DateTime semanaInicio = CalcularPrimeiraDataDaSemanaISO(Convert.ToInt32(sAno), Convert.ToInt32(sSemana));
                    DateTime semanaFim = semanaInicio.AddDays(6);

                    string periodo = $"S{sSemana} {semanaInicio:dd/MM} - {semanaFim:dd/MM}";
                    sb.Append($"\"{periodo}\"");

                    if (i < dsDashBoard.Tables[TAB_Graficos_Contabil_Receber_Trimestre].Rows.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
            }
            sb.Append("];");

            sb.Append("const data = {");
            sb.Append("labels: labels,");
            sb.Append("datasets: [");

            sb.Append("{");
            sb.Append("label: 'Despesa',");
            sb.Append("data: [");
            for (int i = 0; i < dsDashBoard.Tables[TAB_Graficos_Contabil_Receber_Trimestre].Rows.Count; i++)
            {
                string sSemana = dsDashBoard.Tables[TAB_Graficos_Contabil_Receber_Trimestre].Rows[i]["Semana"].ToString();
                if (semanasDoTrimestre.Contains(Convert.ToInt32(sSemana)))
                {
                    double valorPagar = (i < dsDashBoard.Tables[TAB_Graficos_Contabil_Pagar_Trimestre].Rows.Count) ? Convert.ToDouble(dsDashBoard.Tables[TAB_Graficos_Contabil_Pagar_Trimestre].Rows[i]["TotalSemanal"]) : 0;
                    sb.Append(valorPagar.ToString(CultureInfo.InvariantCulture));

                    if (i < dsDashBoard.Tables[TAB_Graficos_Contabil_Receber_Trimestre].Rows.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
            }
            sb.Append("],");
            sb.Append("borderColor: 'rgba(255, 99, 132, 1)',");
            sb.Append("backgroundColor: 'rgba(245, 71, 71)'");
            sb.Append("},");

            sb.Append("{");
            sb.Append("label: 'Receita',");
            sb.Append("data: [");
            for (int i = 0; i < dsDashBoard.Tables[TAB_Graficos_Contabil_Receber_Trimestre].Rows.Count; i++)
            {
                string sSemana = dsDashBoard.Tables[TAB_Graficos_Contabil_Receber_Trimestre].Rows[i]["Semana"].ToString();
                if (semanasDoTrimestre.Contains(Convert.ToInt32(sSemana)))
                {
                    double valorReceber = Convert.ToDouble(dsDashBoard.Tables[TAB_Graficos_Contabil_Receber_Trimestre].Rows[i]["TotalSemanal"]);
                    sb.Append(valorReceber.ToString(CultureInfo.InvariantCulture));

                    if (i < dsDashBoard.Tables[TAB_Graficos_Contabil_Receber_Trimestre].Rows.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
            }
            sb.Append("],");
            sb.Append("borderColor: 'rgba(75, 192, 192, 1)',");
            sb.Append("backgroundColor: 'rgba(34, 130, 239)'");
            sb.Append("}");

            sb.Append("]");
            sb.Append("};");

            sb.Append("const config = {");
            sb.Append("type: 'bar',");
            sb.Append("data: data,");
            sb.Append("options: {");
            sb.Append("responsive: true,");
            sb.Append("maintainAspectRatio: false,");
            sb.Append("plugins: {");
            sb.Append("legend: { position: 'top' },");
            sb.Append("title: { display: true, text: 'Contábil: Receita/Despesas Semanal' },");
            sb.Append("tooltip: {");
            sb.Append(" callbacks: {");
            sb.Append("label: function(context) {");
            sb.Append(" let label = context.dataset.label || '';");
            sb.Append("let rawValue = context.parsed.y;");
            sb.Append(" let formattedValue = Number(rawValue).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });");
            sb.Append("return label + ': ' + formattedValue;");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("},");
            sb.Append("scales: {");
            sb.Append("y: {");
            sb.Append("beginAtZero: true,");
            sb.Append("ticks: {");
            sb.Append("callback: function(value) {");
            sb.Append("return value.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");

            sb.Append("};");

            sb.Append("var ctx9 = document.getElementById('Grafico_Contabil_Trimestre_V2').getContext('2d');");
            sb.Append("var graficoContabilTrimestre = new Chart(ctx9, config);");

            sb.Append("document.getElementById('Grafico_Contabil_Trimestre_V2').addEventListener('click', function(evt) {");
            sb.Append("var activePoints = graficoContabilTrimestre.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);");
            sb.Append("if (activePoints.length) {");
            sb.Append("var firstPoint = activePoints[0];");
            sb.Append("var label = graficoContabilTrimestre.data.labels[firstPoint.index];");
            sb.Append("var datasetLabel = graficoContabilTrimestre.data.datasets[firstPoint.datasetIndex].label;");
            sb.Append("var semana = label.split(' ')[0].replace('S', '').trim();");
            sb.Append("var ano = '" + selectedYear + "';");
            sb.Append("var empresa = '" + ddlidEmpresa.SelectedValue + "';");
            sb.Append("var baseUrlPagar = '" + sLinkContasPagar + "';");
            sb.Append("var baseUrlReceber = '" + sLinkContasReceber + "';");
            sb.Append("var url = (datasetLabel === 'Receita') ? baseUrlReceber + 'ContabilRecebidosSemana&sSemana=' + semana + '&sAno=' + ano + '&sEmpresa=' + empresa : baseUrlPagar + 'ContabilPagosSemana&sSemana=' + semana + '&sAno=' + ano + '&sEmpresa=' + empresa;");
            sb.Append("window.open(url, '_blank');");
            sb.Append("}");
            sb.Append("});");

            sb.Append("});");

            sb.Append("</script>");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "GraficoContabilTrimestre_" + Guid.NewGuid(), sb.ToString(), false);
        }
        private void ConfigurarGraficoContabilSemanal(DataSet dsDashBoard)
        {
            string selectedYear = string.Join(",", itensSelecionados);

            StringBuilder sb = new StringBuilder();
            sb.Append("<script>");
            sb.Append("$(document).ready(function() {");

            sb.Append("var canvas = document.getElementById('Grafico_Contabil_Semana_V2');");
            sb.Append("if(!canvas) {");
            //sb.Append(" console.log(\"Canvas #Grafico_Contabil_Semana_V2 não existe no DOM!\");");
            sb.Append("return;}");

            sb.Append("const data = {");
            sb.Append("labels: [");

            int selectedExibicao = Convert.ToInt32(ddlExibicao.SelectedValue);
            DateTime startDate, endDate;
            if (selectedExibicao == 6)
            {
                startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }
            else
            {
                startDate = DateTime.Today;
                if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                {
                    startDate = DateTime.Today.AddDays(-6);
                }
                else
                {
                    startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
                }
                endDate = startDate.AddDays(6);
            }

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                string dateString = date.ToString("dd/MM/yyyy");
                sb.Append("\"" + dateString + "\"");

                if (date < endDate)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("datasets: [{");
            sb.Append("label: 'Despesa',");
            sb.Append("data: [");

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                DataRow[] rowPagar = dsDashBoard.Tables[TAB_Graficos_Contabil_Pagar_Semana].Select($"DataCompleta = '{date.ToString("yyyy-MM-dd")}'");
                double valorPagar = (rowPagar.Length > 0) ? Convert.ToDouble(rowPagar[0]["TotalDiario"]) : 0;
                sb.Append(valorPagar.ToString("F2", CultureInfo.InvariantCulture));

                if (date < endDate)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("borderColor: 'rgba(255, 99, 132, 1)',");
            sb.Append("backgroundColor: 'rgba(245, 71, 71)'");
            sb.Append("}, {");
            sb.Append("label: 'Receita',");
            sb.Append("data: [");

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                DataRow[] rowReceber = dsDashBoard.Tables[TAB_Graficos_Contabil_Receber_Semana].Select($"DataCompleta = '{date.ToString("yyyy-MM-dd")}'");
                double valorReceber = (rowReceber.Length > 0) ? Convert.ToDouble(rowReceber[0]["TotalDiario"]) : 0;
                sb.Append(valorReceber.ToString("F2", CultureInfo.InvariantCulture));

                if (date < endDate)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("],");
            sb.Append("borderColor: 'rgba(75, 192, 192, 1)',");
            sb.Append("backgroundColor: 'rgba(34, 130, 239)'");
            sb.Append("}]");
            sb.Append("};");

            sb.Append("const config = {");
            sb.Append("type: 'bar',");
            sb.Append("data: data,");
            sb.Append("options: {");
            sb.Append("responsive: true,");
            sb.Append("maintainAspectRatio: false,");
            sb.Append("plugins: {");
            sb.Append("legend: { position: 'top' },");
            sb.Append("title: { display: true, text: 'Contábil: Receita/Despesas Diário' },");
            sb.Append("tooltip: {");
            sb.Append(" callbacks: {");
            sb.Append("label: function(context) {");
            sb.Append(" let label = context.dataset.label || '';");
            sb.Append("let rawValue = context.parsed.y;");
            sb.Append(" let formattedValue = Number(rawValue).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });");
            sb.Append("return label + ': ' + formattedValue;");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("},");
            sb.Append("scales: {");
            sb.Append("x: {");
            sb.Append("ticks: { callback: function(value) { return data.labels[value]; } },");
            sb.Append("},");
            sb.Append("y: {");
            sb.Append("beginAtZero: true,");
            sb.Append("ticks: { callback: function(value) { return value.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }); } }");
            sb.Append("}");
            sb.Append("}");
            sb.Append("}");
            sb.Append("};");

            sb.Append("var ctx10 = document.getElementById('Grafico_Contabil_Semana_V2').getContext('2d');");
            sb.Append("var graficoContabilSemana = new Chart(ctx10, config);");

            sb.Append("document.getElementById('Grafico_Contabil_Semana_V2').addEventListener('click', function(evt) {");
            sb.Append("var activePoints = graficoContabilSemana.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);");
            sb.Append("if (activePoints.length) {");
            sb.Append("var firstPoint = activePoints[0];");
            sb.Append("var label = graficoContabilSemana.data.labels[firstPoint.index];");
            sb.Append("var datasetLabel = graficoContabilSemana.data.datasets[firstPoint.datasetIndex].label;");
            sb.Append("var ano = '" + selectedYear + "';");
            sb.Append("var empresa = '" + ddlidEmpresa.SelectedValue + "';");
            sb.Append("var baseUrlPagarSemana = '" + sLinkContasPagar + "';");
            sb.Append("var baseUrlReceberSemana = '" + sLinkContasReceber + "';");
            sb.Append("var dataCompleta = graficoContabilSemana.data.labels[firstPoint.index];");
            sb.Append("if (datasetLabel === 'Receita') { window.open(baseUrlReceberSemana + 'ContabilRecebidosHoje&sData=' + dataCompleta + '&sAno=' + ano + '&sEmpresa=' + empresa, '_blank'); }");
            sb.Append("else if (datasetLabel === 'Despesa') { window.open(baseUrlPagarSemana + 'ContabilPagosHoje&sData=' + dataCompleta + '&sAno=' + ano + '&sEmpresa=' + empresa, '_blank'); }");
            sb.Append("}");
            sb.Append("});");

            string currentDateString = DateTime.Today.ToString("dd/MM/yyyy");
            sb.Append($"graficoContabilSemana.data.labels.forEach((label, index) => {{ if (label === '{currentDateString}') {{ graficoContabilSemana.getDatasetMeta(0).data[index].custom = {{ backgroundColor: '#000000', fontWeight: 'bold', fontSize: 13 }}; graficoContabilSemana.getDatasetMeta(1).data[index].custom = {{ backgroundColor: '#000000', fontWeight: 'bold', fontSize: 13 }}; }} }});");

            sb.Append("});");
            sb.Append("</script>");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "GraficoContabilSemanal_" + Guid.NewGuid(), sb.ToString(), false);
        }


        #endregion


        protected void timer_Atualizar_Tick(object sender, EventArgs e)
        {            
            string selectedYear = string.Join(",", itensSelecionados); ;
        }

       
        private void LimparGraficos()
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "limparGraficosScript",
            "$('#GraficoAno').empty(); $('#GraficoMensal').empty();", true);


        }
        protected void gvRank_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
            }
        }

        protected void gvRank_Adiantado_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
            }
        }

        void PopularCombos()
        {
            //FUNCOES.Popula_Combo(ddlAnoFinanceiro, "sp_Select 'Flow_Anos_Dash_Adm'", "Ano", "Ano", false, "Todos os Anos", "0");
            //ddlAnoFinanceiro.SelectedValue = DateTime.Now.Year.ToString();
            
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sTabela", "Flow_Anos_Dash_Adm" }                
            };
            DataSet dsDashBoard = BD.ExecutarDataSet("sp_Select", vParametros);
            lstidFluxo.Items.Clear();
            for (int i = 0; i < dsDashBoard.Tables[0].Rows.Count; i++)
            {
                lstidFluxo.Items.Add(new ListItem(BD.Retorno.DATASET(dsDashBoard, 0, i, "Ano"), BD.Retorno.DATASET(dsDashBoard, 0, i, "Ano")));
                if (BD.Retorno.DATASET(dsDashBoard, 0, i, "Ano") == DateTime.Now.Year.ToString())
                {
                    lstidFluxo.Items[i].Selected = true;
                }
            }

            FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa'", "idEmpresa", "sDscEmpresa", false, "Todas as Empresas", "0");
        }

        protected void gvAdiantamento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();

                Literal litLinkAdiantados = e.Row.FindControl("litLinkAdiantados") as Literal;
                if (litLinkAdiantados != null)
                {
                    DataRowView rowView = (DataRowView)e.Row.DataItem;
                    string sidsContasReceber = rowView["sidsContasReceber"].ToString();
                    if ((!string.IsNullOrEmpty(sidsContasReceber) && sidsContasReceber != DBNull.Value.ToString()))
                    {
                        litLinkAdiantados.Text = sidsContasReceber;
                    }
                    else
                    {
                        litLinkAdiantados.Text = string.Empty;
                    }
                }
            }
        }

        protected string LinksGridAdiantamento(object sidsContasReceber)
        {
            string links = string.Empty;

            if (sidsContasReceber != null)
            {
                string sids = sidsContasReceber.ToString();
                string[] ids = sids.Split(',');

                foreach (string id in ids)
                {
                    links += $"<a href='/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={id.Trim()}'>{id.Trim()}</a>, ";
                }

                links = links.TrimEnd(',', ' ');
            }

            return links;
        }

        #region | Metodos de datas

        static string ConvertMonthYear(string mesAno)
        {
            var monthMap = new Dictionary<string, string>
            {
                {"Jan", "01"}, {"Fev", "02"}, {"Mar", "03"},
                {"Abr", "04"}, {"Mai", "05"}, {"Jun", "06"},
                {"Jul", "07"}, {"Ago", "08"}, {"Set", "09"},
                {"Out", "10"}, {"Nov", "11"}, {"Dez", "12"}
            };

            var parts = mesAno.Split('-');
            if (parts.Length == 2 && monthMap.ContainsKey(parts[0]))
            {
                return $"{parts[1]}-{monthMap[parts[0]]}";
            }
            return mesAno;
        }
        public static string GetMonthOfYear(DateTime date)
        {
            return date.ToString("yyyy-MM");
        }
        public static DateTime CalcularPrimeiraDataDaSemanaISO(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;
            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }

            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }
        public static int[] GetSemanasTrimestreAtual()
        {
            DateTime dataAtual = DateTime.Now;
            int trimestreAtual = (dataAtual.Month - 1) / 3 + 1;
            DateTime primeiroDiaTrimestre = new DateTime(dataAtual.Year, (trimestreAtual - 1) * 3 + 1, 1);
            DateTime ultimoDiaTrimestre = primeiroDiaTrimestre.AddMonths(3).AddDays(-1);

            CultureInfo ci = CultureInfo.CurrentCulture;
            int semanaInicio = ci.Calendar.GetWeekOfYear(primeiroDiaTrimestre, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            int semanaFim = ci.Calendar.GetWeekOfYear(ultimoDiaTrimestre, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            int[] semanas = new int[semanaFim - semanaInicio + 1];
            for (int i = 0; i < semanas.Length; i++)
            {
                semanas[i] = semanaInicio + i;
            }

            return semanas;
        }

        public static int[] GetSemanasPorTrimestre(int trimestre)
        {
            if (trimestre < 1 || trimestre > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(trimestre), "O valor de trimestre deve estar entre 1 e 4.");
            }

            DateTime dataAtual = DateTime.Now;
            DateTime primeiroDiaTrimestre = new DateTime(dataAtual.Year, (trimestre - 1) * 3 + 1, 1);
            DateTime ultimoDiaTrimestre = primeiroDiaTrimestre.AddMonths(3).AddDays(-1);

            CultureInfo ci = CultureInfo.CurrentCulture;
            int semanaInicio = ci.Calendar.GetWeekOfYear(primeiroDiaTrimestre, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            int semanaFim = ci.Calendar.GetWeekOfYear(ultimoDiaTrimestre, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            int[] semanas = new int[semanaFim - semanaInicio + 1];
            for (int i = 0; i < semanas.Length; i++)
            {
                semanas[i] = semanaInicio + i;
            }

            return semanas;
        }

        public static string GetWeekOfYear(DateTime date)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return $"{weekNum:D2}";
        }

        #endregion

        #region Eventos ddl
        protected void ddlExibicao_SelectedIndexChanged(object sender, EventArgs e)
        {

            switch (Convert.ToInt32(ddlExibicao.SelectedValue))
            {
                case 0:
                    Div_Grafico_Caixa_Semanal.Visible = true;
                    Div_Grafico_Caixa_Trimestre.Visible = false;
                    Div_Grafico_Caixa_Ano.Visible = false;
                    lblGrafico_Caixa_Semanal_v2.Text = "Caixa: Pagamentos / Recebimentos - Semana Atual";
                    //-----------------------------------------------------------

                    Div_Grafico_Emprestimo_Semana_v2.Visible = true;
                    Div_Grafico_Emprestimo_Trimestre_v2.Visible = false;
                    Div_Grafico_Emprestimo_Anual_v2.Visible = false;
                    lblGrafico_Emprestimo_Semana_v2.Text = "Empréstimo: Pagar / Receber - Semana Atual";
                    //-----------------------------------------------------------

                    //Div_Grafico_Comparativo_Semana_V2.Visible = true;
                    //Div_Grafico_Comparativo_Trimestre_V2.Visible = false;
                    //Div_Grafico_Comparativo_Ano_V2.Visible = false;
                    //lblGrafico_Comparativo_Semana_V2.Text = "Acumulado: Despesas / Receita - Semana Atual";
                    Div_Grafico_Comparativo_Ano_V2.Visible = true;
                    lblGrafico_Comparativo_Semana_V2.Text = "Acumulado: Despesas / Receitas / Metas - Ano";
                    //-----------------------------------------------------------

                    Div_Grafico_Contabil_Semana_V2.Visible = true;
                    Div_Grafico_Contabil_Trimestre_V2.Visible = false;
                    Div_Grafico_Contabil_Ano_v2.Visible = false;
                    lblGrafico_Contabil_Ano_v2.Text = "Contábil: Despesas / Receita - Semana Atual";

                    break;

                case 1:
                    Div_Grafico_Caixa_Semanal.Visible = false;
                    Div_Grafico_Caixa_Trimestre.Visible = true;
                    Div_Grafico_Caixa_Ano.Visible = false;
                    lblGrafico_Caixa_Semanal_v2.Text = "Caixa: Pagamentos / Recebimentos - Q1";
                    //-----------------------------------------------------------

                    Div_Grafico_Emprestimo_Semana_v2.Visible = false;
                    Div_Grafico_Emprestimo_Trimestre_v2.Visible = true;
                    Div_Grafico_Emprestimo_Anual_v2.Visible = false;
                    lblGrafico_Emprestimo_Semana_v2.Text = "Empréstimo: Pagar / Receber - Q1";
                    //-----------------------------------------------------------

                    //Div_Grafico_Comparativo_Semana_V2.Visible = false;
                    //Div_Grafico_Comparativo_Trimestre_V2.Visible = true;
                    //Div_Grafico_Comparativo_Ano_V2.Visible = false;
                    //lblGrafico_Comparativo_Semana_V2.Text = "Acumulado: Despesas / Receita - Q1";
                    Div_Grafico_Comparativo_Ano_V2.Visible = true;
                    lblGrafico_Comparativo_Semana_V2.Text = "Acumulado: Despesas / Receitas / Metas - Ano";
                    //-----------------------------------------------------------

                    Div_Grafico_Contabil_Semana_V2.Visible = false;
                    Div_Grafico_Contabil_Trimestre_V2.Visible = true;
                    Div_Grafico_Contabil_Ano_v2.Visible = false;
                    lblGrafico_Contabil_Ano_v2.Text = "Contábil: Despesas / Receita - Q1";

                    break;

                case 2:
                    Div_Grafico_Caixa_Semanal.Visible = false;
                    Div_Grafico_Caixa_Trimestre.Visible = true;
                    Div_Grafico_Caixa_Ano.Visible = false;
                    lblGrafico_Caixa_Semanal_v2.Text = "Caixa: Pagamentos / Recebimentos - Q2";
                    //-----------------------------------------------------------

                    Div_Grafico_Emprestimo_Semana_v2.Visible = false;
                    Div_Grafico_Emprestimo_Trimestre_v2.Visible = true;
                    Div_Grafico_Emprestimo_Anual_v2.Visible = false;
                    lblGrafico_Emprestimo_Semana_v2.Text = "Empréstimo: Pagar / Receber - Q2";
                    //-----------------------------------------------------------

                    //Div_Grafico_Comparativo_Semana_V2.Visible = false;
                    //Div_Grafico_Comparativo_Trimestre_V2.Visible = true;
                    //Div_Grafico_Comparativo_Ano_V2.Visible = false;
                    //lblGrafico_Comparativo_Semana_V2.Text = "Acumulado: Despesas / Receita - Q2";
                    Div_Grafico_Comparativo_Ano_V2.Visible = true;
                    lblGrafico_Comparativo_Semana_V2.Text = "Acumulado: Despesas / Receitas / Metas - Ano";
                    //-----------------------------------------------------------

                    Div_Grafico_Contabil_Semana_V2.Visible = false;
                    Div_Grafico_Contabil_Trimestre_V2.Visible = true;
                    Div_Grafico_Contabil_Ano_v2.Visible = false;
                    lblGrafico_Contabil_Ano_v2.Text = "Contábil: Despesas / Receita - Q2";

                    break;

                case 3:
                    Div_Grafico_Caixa_Semanal.Visible = false;
                    Div_Grafico_Caixa_Trimestre.Visible = true;
                    Div_Grafico_Caixa_Ano.Visible = false;
                    lblGrafico_Caixa_Semanal_v2.Text = "Caixa: Pagamentos / Recebimentos - Q3";
                    //-----------------------------------------------------------

                    Div_Grafico_Emprestimo_Semana_v2.Visible = false;
                    Div_Grafico_Emprestimo_Trimestre_v2.Visible = true;
                    Div_Grafico_Emprestimo_Anual_v2.Visible = false;
                    lblGrafico_Emprestimo_Semana_v2.Text = "Empréstimo: Pagar / Receber - Q3";
                    //-----------------------------------------------------------

                    //Div_Grafico_Comparativo_Semana_V2.Visible = false;
                    //Div_Grafico_Comparativo_Trimestre_V2.Visible = true;
                    //Div_Grafico_Comparativo_Ano_V2.Visible = false;
                    //lblGrafico_Comparativo_Semana_V2.Text = "Acumulado: Despesas / Receita - Q3";
                    Div_Grafico_Comparativo_Ano_V2.Visible = true;
                    lblGrafico_Comparativo_Semana_V2.Text = "Acumulado: Despesas / Receitas / Metas - Ano";
                    //-----------------------------------------------------------

                    Div_Grafico_Contabil_Semana_V2.Visible = false;
                    Div_Grafico_Contabil_Trimestre_V2.Visible = true;
                    Div_Grafico_Contabil_Ano_v2.Visible = false;
                    lblGrafico_Contabil_Ano_v2.Text = "Contábil: Despesas / Receita - Q3";

                    break;

                case 4:
                    Div_Grafico_Caixa_Semanal.Visible = false;
                    Div_Grafico_Caixa_Trimestre.Visible = true;
                    Div_Grafico_Caixa_Ano.Visible = false;
                    lblGrafico_Caixa_Semanal_v2.Text = "Caixa: Pagamentos / Recebimentos - Q4";
                    //-----------------------------------------------------------

                    Div_Grafico_Emprestimo_Semana_v2.Visible = false;
                    Div_Grafico_Emprestimo_Trimestre_v2.Visible = true;
                    Div_Grafico_Emprestimo_Anual_v2.Visible = false;
                    lblGrafico_Emprestimo_Semana_v2.Text = "Empréstimo: Pagar / Receber - Q4";
                    //-----------------------------------------------------------

                    //Div_Grafico_Comparativo_Semana_V2.Visible = false;
                    //Div_Grafico_Comparativo_Trimestre_V2.Visible = true;
                    //Div_Grafico_Comparativo_Ano_V2.Visible = false;
                    //lblGrafico_Comparativo_Semana_V2.Text = "Acumulado: Despesas / Receita - Q4";
                    Div_Grafico_Comparativo_Ano_V2.Visible = true;
                    lblGrafico_Comparativo_Semana_V2.Text = "Acumulado: Despesas / Receitas / Metas - Ano";
                    //-----------------------------------------------------------

                    Div_Grafico_Contabil_Semana_V2.Visible = false;
                    Div_Grafico_Contabil_Trimestre_V2.Visible = true;
                    Div_Grafico_Contabil_Ano_v2.Visible = false;
                    lblGrafico_Contabil_Ano_v2.Text = "Contábil: Despesas / Receita - Q4";

                    break;

                case 5:
                    Div_Grafico_Caixa_Semanal.Visible = false;
                    Div_Grafico_Caixa_Trimestre.Visible = false;
                    Div_Grafico_Caixa_Ano.Visible = true;
                    lblGrafico_Caixa_Semanal_v2.Text = "Caixa: Pagamentos / Recebimentos - Ano";
                    //-----------------------------------------------------------

                    Div_Grafico_Emprestimo_Semana_v2.Visible = false;
                    Div_Grafico_Emprestimo_Trimestre_v2.Visible = false;
                    Div_Grafico_Emprestimo_Anual_v2.Visible = true;
                    lblGrafico_Emprestimo_Semana_v2.Text = "Empréstimo: Pagar / Receber - Ano";
                    //-----------------------------------------------------------

                    //Div_Grafico_Comparativo_Semana_V2.Visible = false;
                    //Div_Grafico_Comparativo_Trimestre_V2.Visible = false;
                    Div_Grafico_Comparativo_Ano_V2.Visible = true;
                    lblGrafico_Comparativo_Semana_V2.Text = "Acumulado: Despesas / Receitas / Metas - Ano";
                    //-----------------------------------------------------------

                    Div_Grafico_Contabil_Semana_V2.Visible = false;
                    Div_Grafico_Contabil_Trimestre_V2.Visible = false;
                    Div_Grafico_Contabil_Ano_v2.Visible = true;
                    lblGrafico_Contabil_Ano_v2.Text = "Contábil: Despesas / Receita - Ano";

                    break;

                case 6:
                    Div_Grafico_Caixa_Semanal.Visible = true;
                    Div_Grafico_Caixa_Trimestre.Visible = false;
                    Div_Grafico_Caixa_Ano.Visible = false;
                    lblGrafico_Caixa_Semanal_v2.Text = "Caixa: Pagamentos / Recebimentos - Mes Atual";
                    //-----------------------------------------------------------

                    Div_Grafico_Emprestimo_Semana_v2.Visible = true;
                    Div_Grafico_Emprestimo_Trimestre_v2.Visible = false;
                    Div_Grafico_Emprestimo_Anual_v2.Visible = false;
                    lblGrafico_Emprestimo_Semana_v2.Text = "Empréstimo: Pagar / Receber - Mes Atual";
                    //-----------------------------------------------------------

                    //Div_Grafico_Comparativo_Semana_V2.Visible = true;
                    //Div_Grafico_Comparativo_Trimestre_V2.Visible = false;
                    //Div_Grafico_Comparativo_Ano_V2.Visible = false;
                    //lblGrafico_Comparativo_Semana_V2.Text = "Acumulado: Despesas / Receita - Mes Atual";
                    Div_Grafico_Comparativo_Ano_V2.Visible = true;
                    lblGrafico_Comparativo_Semana_V2.Text = "Acumulado: Despesas / Receitas / Metas - Ano";
                    //-----------------------------------------------------------

                    Div_Grafico_Contabil_Semana_V2.Visible = true;
                    Div_Grafico_Contabil_Trimestre_V2.Visible = false;
                    Div_Grafico_Contabil_Ano_v2.Visible = false;
                    lblGrafico_Contabil_Ano_v2.Text = "Contábil: Despesas / Receita - Mes Atual";


                    break;

            }
            // int trimestreSelecionado = int.Parse(ddlExibicao.SelectedValue);

            //AtualizarDashBoard(IDENTITY.Variaveis.idUsuario(), ddlAnoFinanceiro.SelectedValue, ddlidEmpresa.SelectedValue, ddlidAdiantamentoSelecionado.SelectedValue);
           

        }
        protected void ddlidAdiantamentoSelecionado_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePanelGridAdiantamento();
        }
        private void UpdatePanelGridAdiantamento()
        {

            string selectedYear = string.Join(",", itensSelecionados);
            string EmpresaSelecionadaTexto = ddlidEmpresa.SelectedItem.Text;
            string AdiantamentoSelecionado = ddlidAdiantamentoSelecionado.SelectedValue;

            if (itensSelecionados.Count == lstidFluxo.Items.Count)
            {

                lblTituloContasPagar.Text = "Contas a Pagar" + " - " + EmpresaSelecionadaTexto;
                lblContasReceber.Text = "Contas a Receber" + " - " + EmpresaSelecionadaTexto;
                lblAnoFinanceiro.Text = "Ano Financeiro/Todos os Anos" + " - " + EmpresaSelecionadaTexto;

                if (AdiantamentoSelecionado == "1") //Pendentes
                {
                    lblAdiantamento.Text = "Adiantamentos com Pagamento Pendente" + " - " + EmpresaSelecionadaTexto;
                }
                else if (AdiantamentoSelecionado == "2") //Liquidados
                {
                    lblAdiantamento.Text = "Adiantamentos Liquidados" + " - " + EmpresaSelecionadaTexto;
                }
                else
                {
                    lblAdiantamento.Text = "Adiantamentos" + " - " + EmpresaSelecionadaTexto;
                }
            }
            else
            {


                lblTituloContasPagar.Text = "Contas a Pagar" + " - " + EmpresaSelecionadaTexto;
                lblContasReceber.Text = "Contas a Receber" + " - " + EmpresaSelecionadaTexto;
                lblAnoFinanceiro.Text = "Ano Financeiro/" + selectedYear + " - " + EmpresaSelecionadaTexto;

                if (AdiantamentoSelecionado == "1") //Pendentes
                {
                    lblAdiantamento.Text = "Adiantamentos com Pagamento Pendente" + " - " + EmpresaSelecionadaTexto;
                }
                else if (AdiantamentoSelecionado == "2") //Liquidados
                {
                    lblAdiantamento.Text = "Adiantamentos Liquidados" + " - " + EmpresaSelecionadaTexto;
                }
                else
                {
                    lblAdiantamento.Text = "Adiantamentos" + " - " + EmpresaSelecionadaTexto;
                }
            }
        }
        protected void ddlAnoFinanceiro_SelectedIndexChanged(object sender, EventArgs e)
        {
            //foreach (ListItem item in lstidFluxo.Items)
            //{
            //    if (item.Selected)
            //    {
            //        itensSelecionados.Add(item.Value);
            //    }
            //}

            //if (itensSelecionados.Count > 1 || itensSelecionados[0].ToString() != "2024")
            //{
            //    ddlExibicao.SelectedValue = "5";
            //    ddlExibicao.Visible = false;
            //}

            //string selectedYear = string.Join(",", itensSelecionados);
            //LimparGraficos();       
          
            //UpdatePanelGridAdiantamento();
        }
        protected void ddlidEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            string EmpresaSelecionadaValor = ddlidEmpresa.SelectedValue;
            string EmpresaSelecionada = ddlidEmpresa.SelectedValue;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "updateGraph", "atualizaGraficoeAno();", true);
            LimparGraficos();


            string selectedValue = string.Join(",", itensSelecionados);
            string selectedYear = string.Join(",", itensSelecionados);

            if (selectedValue.Contains(",") || selectedValue != "2024")
            {
                ddlExibicao.SelectedValue = "5";
                ddlExibicao.Visible = false;

            }
            else
            {
                ddlExibicao.Visible = true;
            }

            UpdatePanelGridAdiantamento();
        }

        #endregion

        protected void lnkAtualizar_Click(object sender, EventArgs e)
        {
            itensSelecionados.Clear();

            foreach (ListItem item in lstidFluxo.Items)
            {
                if (item.Selected)
                {
                    itensSelecionados.Add(item.Value);
                }
            }

            if(itensSelecionados.Count < 1)
            {
                ListItem item = lstidFluxo.Items.FindByValue(DateTime.Now.Year.ToString());
                item.Selected = true;
                itensSelecionados.Add(DateTime.Now.Year.ToString());
            }

            if (itensSelecionados.Count > 1 || itensSelecionados[0].ToString() != "2024")
            {
                ddlExibicao.SelectedValue = "5";
                ddlExibicao.Visible = false;
            }

            
            LimparGraficos();

            UpdatePanelGridAdiantamento();
        }
    }
}