using System;
using System.Text;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using RETORNO = TT.FrameWork.BD.Retorno;
using GRID = TT.FrameWork.Grid;
using static TT.FrameWork.Identity;

namespace TT_Flow.App
{
    public partial class Dashboard : Page
    {
        #region | Construtores

        string sLinkPedidos { get { return $"/App/Paginas/Pedidos.aspx?sSeparado={Switch_DashboardSeparado.Recuperar()}&dashboard="; } }
        string sLinkTarefas = "/App/Paginas/Tarefas.aspx?dashboard=";
         
        int TAB_Quadrados = 0;
        int Tabela_Pedidos = 1;
        int TAB_GraficoDepartamento = 2;
        int TAB_GraficoStatus = 3;
        int Tabela_Tarefas = 4;
        int Tabela_Fluxos = 5;
        int Tabela_Parametros = 6;
        int Tabela_GraficoFluxos = 7;

        #endregion

        #region | Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                timer_Atualizar.Enabled = false;

                div_FluxosSeparados.Visible = FUNCOES.ValidaPermissao(Permissao.Pedidos.Visualizar_FluxosSeparados__Dashboard);
                ddlsClienteEspecial.Visible = FUNCOES.ValidaPermissao(Permissao.Parceiros.EnxergarClientesVisualizacaoDiferenciada);

                div_DashboardSeparado.Visible = FUNCOES.ValidaPermissao(Permissao.Pedidos.Visualizar_TTL_TTS__Dashboard);

                Switch_DashboardSeparado.Definir("N", null, "N");
                div_FluxosSeparados.Visible = false;
                div_Empresas.Visible = false;

                AtualizarDashBoard(sEmpresa: "");

                if (Switch_DashboardSeparado.Recuperar().Equals("S"))
                {
                    AtualizarDashBoard(sEmpresa: null);
                    div_FluxosSeparados.Visible = true;
                }
            }

            var requestTarget = Request["__EVENTTARGET"];
            if (requestTarget == "SEPARAR_DASH")
            {
                bool bDashCompleto = Switch_DashboardSeparado.Recuperar().Equals("S");
                div_FluxosSeparados.Visible = bDashCompleto;
                div_Empresas.Visible = bDashCompleto;
                SalvarPreferenciaUsuario();
                AtualizarDashBoard(sEmpresa: bDashCompleto ? null : "");
            }
            else if (requestTarget == "POSTBACK_GRAFICOS") SalvarPreferenciaUsuario();

            Switch_DashboardSeparado.sPosicao_Nao = "27.5%";
            Switch_DashboardSeparado.sPosicao_Sim = "375%";
            Switch_DashboardSeparado.sTamanho_Switch = "7.25em";
            Switch_DashboardSeparado.sNao = "Simples";
            Switch_DashboardSeparado.sSim = "Detalhado";
            Switch_DashboardSeparado.sCorFundo_Sim = "#5cb85c";
            Switch_DashboardSeparado.sPostBack_Switch = "SEPARAR_DASH";

            Calendario_View.EstilosPersonalizados_Titulo = "P";
            Calendario_View.TipoVisualizacao = 1;
            
            Atualiza_AbaCalendario();
            RegistraScript();
            Calendario_View.RegistraScript_Dashboard_View();
        }

        #endregion

        #region | Utils

        void Atualiza_AbaCalendario()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscTitulo", txtPesquisa.Text },
                { "@idStatus", ddlidStatus.SelectedValue },
                { "@idTipoEvento", ddlTipoEvento.SelectedValue },
                { "@idUsuario", Variaveis.idUsuario() }
            };
            Calendario.RegistraScript(BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Calendario_Eventos_x_Usuario", vParametros));
        }

        void SalvarPreferenciaUsuario()
        {
            string sFLow_idFluxo = "";
            string idUsuario = Variaveis.idUsuario();

            foreach (ListItem item in lstidFluxo.Items) { if (item.Selected) sFLow_idFluxo += item.Value + "|"; }

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "Salvar_Usuarios_x_Preferecias" },
                { "@idUsuario", idUsuario },
                { "@sFLow_idFluxo", sFLow_idFluxo },
                { "@sDashboard_Separado", Switch_DashboardSeparado.Recuperar() }
            };

            if (ddlsClienteEspecial.Visible) vParametros.Add("@sClienteEspecial", ddlsClienteEspecial.SelectedValue);
            if (div_Empresas.Visible) vParametros.Add("@sEmpresa", rbTTL.Checked ? "TTL" : rbTTS.Checked ? "TTS" : null);

            BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios", vParametros);

            AtualizarDashBoard(idUsuario);
        }

        void AtualizarDashBoard(string idUsuario = "", string sEmpresa = null)
        {
            if (string.IsNullOrEmpty(idUsuario)) idUsuario = Variaveis.idUsuario();
            if (sEmpresa != "") sEmpresa = rbTTL.Checked ? "TTL" : rbTTS.Checked ? "TTS" : null;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@idUsuario", idUsuario.ToString() },
                { "@idPais", Variaveis.idEmpresa() == "Brasil" ? "1" : Variaveis.idEmpresa() == "EUA" ? "2" : "0" },
                { "@sFiltraFluxosSeparados", div_FluxosSeparados.Visible ? "S" : "N" },
                { "@sEmpresa", sEmpresa },
                { "@idUsuarioxEmpresa", Variaveis.idUsuario() }
            };
            DataSet dsDashBoard = BD.ExecutarDataSet("sp_Flow_DashBoard", vParametros, out string sSQL);

            if (BD.ValidarDataSet(dsDashBoard))
            {
                lblTarefas_TotalUsuario.Text = string.Format("{0:n0}", dsDashBoard.Tables[Tabela_Tarefas].Rows[0]["SuasTarefas"]);
                lblTarefas_TotalDepartamento.Text = string.Format("{0:n0}", dsDashBoard.Tables[Tabela_Tarefas].Rows[0]["SeuDepartamento"]);

                if (lblTarefas_TotalUsuario.Text != "0") lnkTarefas_TotalUsuario.NavigateUrl = sLinkTarefas + "SuasTarefas";
                if (lblTarefas_TotalDepartamento.Text != "0") lnkTarefas_TotalDepartamento.NavigateUrl = sLinkTarefas + "SeuDepartamento";

                lstidFluxo.Items.Clear();
                for (int i = 0; i < dsDashBoard.Tables[Tabela_Fluxos].Rows.Count; i++)
                {
                    lstidFluxo.Items.Add(new ListItem(RETORNO.DATASET(dsDashBoard, Tabela_Fluxos, i, "sDscFluxo"), RETORNO.DATASET(dsDashBoard, Tabela_Fluxos, i, "idFluxo")));

                    if (RETORNO.DATASET(dsDashBoard, Tabela_Fluxos, i, "sCheck") == "S") lstidFluxo.Items[i].Selected = true;
                }

                ddlsClienteEspecial.SelectedValue = RETORNO.DATASET(dsDashBoard, Tabela_Parametros, 0, "sClienteEspecial");

                lblTotalPedidos.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblTotalPedidos"]);
                lblTarefas_TotalDepto.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lbltotalSeuDepartamento"]);
                lblTotalVencerSeteDias.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblTotalVencerSeteDias"]);
                lblTotalemAtraso.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblTotalemAtraso"]);

                lblEntregas_EmDia.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblEntregas_EmDia"]);
                lblEntregas_EmAtraso.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblEntregas_EmAtraso"]);
                lblColetas_EmDia.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblColetas_EmDia"]);
                lblColetas_EmAtraso.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblColetas_EmAtraso"]);

                lnkTotalPedidos.NavigateUrl = lblTotalPedidos.Text != "0" ? $"{sLinkPedidos}Total" : "";
                lnkTarefas_TotalDepto.NavigateUrl = lblTarefas_TotalDepto.Text != "0" ? $"{sLinkPedidos}Seu_Departamento" : "";
                lnkTotalVencerSeteDias.NavigateUrl = lblTotalVencerSeteDias.Text != "0" ? $"{sLinkPedidos}7Dias" : "";
                lnkTotalemAtraso.NavigateUrl = lblTotalemAtraso.Text != "0" ? $"{sLinkPedidos}Atraso" : "";

                lnkEntregas_EmDia.NavigateUrl = lblEntregas_EmDia.Text != "0" ? $"{sLinkPedidos}Entregas_Em_Dia" : "";
                lnkEntregas_EmAtraso.NavigateUrl = lblEntregas_EmAtraso.Text != "0" ? $"{sLinkPedidos}Entregas_Em_Atraso" : "";
                lnkColetas_EmDia.NavigateUrl = lblColetas_EmDia.Text != "0" ? $"{sLinkPedidos}Coletas_Em_Dia" : "";
                lnkColetas_EmAtraso.NavigateUrl = lblColetas_EmAtraso.Text != "0" ? $"{sLinkPedidos}Coletas_Em_Atraso" : "";

                Switch_DashboardSeparado.Definir(RETORNO.DATASET(dsDashBoard, Tabela_Parametros, 0, "sDashboard_Separado"), null, "N");

                if (Switch_DashboardSeparado.Recuperar().Equals("S") && FUNCOES.ValidaPermissao(Permissao.Pedidos.Visualizar_FluxosSeparados__Dashboard))
                {
                    lblTotalPedidos_B2B.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblTotalPedidos_B2B"]);
                    lblEntrega_B2B.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblEntrega_B2B"]);
                    lblDepto_B2B.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblDepto_B2B"]);
                    lbl7Dias_B2B.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lbl7Dias_B2B"]);
                    lblAtraso_B2B.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblAtraso_B2B"]);
                    lblConfirmacao_B2B.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblConfirmacao_B2B"]);

                    lblTotalPedidos_eCommerce.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblTotalPedidos_eCommerce"]);
                    lblEntrega_eCommerce.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblEntrega_eCommerce"]);
                    lblDepto_eCommerce.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblDepto_eCommerce"]);
                    lbl7Dias_eCommerce.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lbl7Dias_eCommerce"]);
                    lblAtraso_eCommerce.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblAtraso_eCommerce"]);
                    lblConfirmacao_eCommerce.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblConfirmacao_eCommerce"]);

                    if (lblTotalPedidos_B2B.Text != "0") lnkTotalPedidos_B2B.NavigateUrl = sLinkPedidos + "Total_B2B";
                    if (lblEntrega_B2B.Text != "0") lnkEntrega_B2B.NavigateUrl = sLinkPedidos + "Entregas_B2B";
                    if (lblDepto_B2B.Text != "0") lnkDepto_B2B.NavigateUrl = sLinkPedidos + "Seu_Departamento_B2B";
                    if (lbl7Dias_B2B.Text != "0") lnk7Dias_B2B.NavigateUrl = sLinkPedidos + "7Dias_B2B";
                    if (lblAtraso_B2B.Text != "0") lnkAtraso_B2B.NavigateUrl = sLinkPedidos + "Atraso_B2B";
                    if (lblConfirmacao_B2B.Text != "0") lnkConfirmacao_B2B.NavigateUrl = sLinkPedidos + "Confirmacao_B2B";

                    if (lblTotalPedidos_eCommerce.Text != "0") lnkTotalPedidos_eCommerce.NavigateUrl = sLinkPedidos + "Total_eCommerce";
                    if (lblEntrega_eCommerce.Text != "0") lnkEntrega_eCommerce.NavigateUrl = sLinkPedidos + "Entregas_eCommerce";
                    if (lblDepto_eCommerce.Text != "0") lnkDepto_eCommerce.NavigateUrl = sLinkPedidos + "Seu_Departamento_eCommerce";
                    if (lbl7Dias_eCommerce.Text != "0") lnk7Dias_eCommerce.NavigateUrl = sLinkPedidos + "7Dias_eCommerce";
                    if (lblAtraso_eCommerce.Text != "0") lnkAtraso_eCommerce.NavigateUrl = sLinkPedidos + "Atraso_eCommerce";
                    if (lblConfirmacao_eCommerce.Text != "0") lnkConfirmacao_eCommerce.NavigateUrl = sLinkPedidos + "Confirmacao_eCommerce";
                }

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_gvUltimosPedidos_DataBind", "if ($.fn.DataTable.isDataTable($('table[id$=gvUltimosPedidos]'))) {\r\n$('table[id$=gvUltimosPedidos]').DataTable().destroy();\r\n}\r\n" + GRID.DataBindComScriptData(gvUltimosPedidos, dsDashBoard.Tables[Tabela_Pedidos], 0, new int[3] { 3, 7, 8 }, "desc", "false", "''"), true);

                if (Variaveis.idParceiro() != "0")
                {
                    DIV_TAREFAS.Visible = false;
                    ddlsClienteEspecial.Visible = false;
                    lstidFluxo.Visible = false;
                    DIV_GRID_GRAFICOS.Visible = false;
                    DIV_GRID_PEDIDOS.Visible = false;
                }

                if (dsDashBoard.Tables[Tabela_Pedidos].Rows.Count == 0)
                {
                    DIV_GRID_GRAFICOS.Visible = false;
                    DIV_GRID_PEDIDOS.Visible = false;
                }

                if (Switch_DashboardSeparado.Recuperar().Equals("S") && FUNCOES.ValidaPermissao(Permissao.Pedidos.Visualizar_TTL_TTS__Dashboard))
                {
                    div_Empresas.Visible = true;

                    rbTTL.Checked = false;
                    rbTodos.Checked = false;
                    rbTTS.Checked = false;
                    lnkTotalPedidos.Visible = false;
                    div_TotalPedidos_Todos.Visible = false;
                    div_Depto_7Dias.Visible = false;
                    div_Depto_7Dias_Todos.Visible = false;
                    lnkTotalemAtraso.Visible = false;
                    div_TotalemAtraso_Todos.Visible = false;

                    sEmpresa = RETORNO.DATASET(dsDashBoard, Tabela_Parametros, 0, "sEmpresa");
                    if (string.IsNullOrEmpty(sEmpresa))
                    {
                        rbTodos.Checked = true;
                        div_TotalPedidos_Todos.Visible = true;
                        div_Depto_7Dias_Todos.Visible = true;
                        div_TotalemAtraso_Todos.Visible = true;

                        lblTotalPedidos_TTL.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblTotalPedidos_TTL"]);
                        lblTarefas_TotalDepto_TTL.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblTarefas_TotalDepto_TTL"]);
                        lblTotalVencerSeteDias_TTL.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblTotalVencerSeteDias_TTL"]);
                        lblTotalemAtraso_TTL.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblTotalemAtraso_TTL"]);

                        lblTotalPedidos_TTS.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblTotalPedidos_TTS"]);
                        lblTarefas_TotalDepto_TTS.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblTarefas_TotalDepto_TTS"]);
                        lblTotalVencerSeteDias_TTS.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblTotalVencerSeteDias_TTS"]);
                        lblTotalemAtraso_TTS.Text = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["lblTotalemAtraso_TTS"]);

                        if (lblTotalPedidos_TTL.Text != "0") lnkTotalPedidos_TTL.NavigateUrl = sLinkPedidos + "Total_TTL";
                        if (lblTarefas_TotalDepto_TTL.Text != "0") lnkTarefas_TotalDepto_TTL.NavigateUrl = sLinkPedidos + "Seu_Departamento_TTL";
                        if (lblTotalVencerSeteDias_TTL.Text != "0") lnkTotalVencerSeteDias_TTL.NavigateUrl = sLinkPedidos + "7Dias_TTL";
                        if (lblTotalemAtraso_TTL.Text != "0") lnkTotalemAtraso_TTL.NavigateUrl = sLinkPedidos + "Atraso_TTL";

                        if (lblTotalPedidos_TTS.Text != "0") lnkTotalPedidos_TTS.NavigateUrl = sLinkPedidos + "Total_TTS";
                        if (lblTarefas_TotalDepto_TTS.Text != "0") lnkTarefas_TotalDepto_TTS.NavigateUrl = sLinkPedidos + "Seu_Departamento_TTS";
                        if (lblTotalVencerSeteDias_TTS.Text != "0") lnkTotalVencerSeteDias_TTS.NavigateUrl = sLinkPedidos + "7Dias_TTS";
                        if (lblTotalemAtraso_TTS.Text != "0") lnkTotalemAtraso_TTS.NavigateUrl = sLinkPedidos + "Atraso_TTS";
                    }
                    else
                    {
                        lnkTotalPedidos.Visible = true;
                        div_Depto_7Dias.Visible = true;
                        lnkTotalemAtraso.Visible = true;
                        rbTTL.Checked = sEmpresa.Equals("TTL");
                        rbTTS.Checked = sEmpresa.Equals("TTS");

                        lnkTotalPedidos.NavigateUrl = lblTotalPedidos.Text != "0" ? $"{sLinkPedidos}Total_{sEmpresa}" : "";
                        lnkTarefas_TotalDepto.NavigateUrl = lblTarefas_TotalDepto.Text != "0" ? $"{sLinkPedidos}Seu_Departamento_{sEmpresa}" : "";
                        lnkTotalVencerSeteDias.NavigateUrl = lblTotalVencerSeteDias.Text != "0" ? $"{sLinkPedidos}7Dias_{sEmpresa}" : "";
                        lnkTotalemAtraso.NavigateUrl = lblTotalemAtraso.Text != "0" ? $"{sLinkPedidos}Atraso_{sEmpresa}" : "";

                        lnkEntregas_EmDia.NavigateUrl = lblEntregas_EmDia.Text != "0" ? $"{sLinkPedidos}Entregas_Em_Dia_{sEmpresa}" : "";
                        lnkEntregas_EmAtraso.NavigateUrl = lblEntregas_EmAtraso.Text != "0" ? $"{sLinkPedidos}Entregas_Em_Atraso_{sEmpresa}" : "";
                        lnkColetas_EmDia.NavigateUrl = lblColetas_EmDia.Text != "0" ? $"{sLinkPedidos}Coletas_Em_Dia_{sEmpresa}" : "";
                        lnkColetas_EmAtraso.NavigateUrl = lblColetas_EmAtraso.Text != "0" ? $"{sLinkPedidos}Coletas_Em_Atraso_{sEmpresa}" : "";
                    }
                }
                else
                {
                    div_Empresas.Visible = false;

                    rbTTL.Checked = false;
                    rbTodos.Checked = false;
                    rbTTS.Checked = false;
                    div_TotalPedidos_Todos.Visible = false;
                    div_Depto_7Dias_Todos.Visible = false;
                    div_TotalemAtraso_Todos.Visible = false;

                    lnkTotalPedidos.Visible = true;
                    div_Depto_7Dias.Visible = true;
                    lnkTotalemAtraso.Visible = true;
                }

                div_Outros.Visible = false;

                if (DIV_GRID_GRAFICOS.Visible)
                {
                    StringBuilder sb = new StringBuilder();

                    // Gráfico 1 - Departamento
                    sb.Append("$(function() {");

                    sb.Append("Morris.Donut({");
                    sb.Append("element: 'GraficoDepartamento',");
                    sb.Append("data: [");

                    for (int nLinhasOrigem = 0; nLinhasOrigem < dsDashBoard.Tables[TAB_GraficoDepartamento].Rows.Count; nLinhasOrigem++)
                    {
                        sb.Append("{label: '" + dsDashBoard.Tables[TAB_GraficoDepartamento].Rows[nLinhasOrigem]["sDscDepartamento"].ToString() + "',");
                        sb.Append("	value: '" + dsDashBoard.Tables[TAB_GraficoDepartamento].Rows[nLinhasOrigem]["nTotal"].ToString() + "',");
                        sb.Append("	id: '" + dsDashBoard.Tables[TAB_GraficoDepartamento].Rows[nLinhasOrigem]["idDepartamentoAtual"].ToString() + "'}");

                        if (nLinhasOrigem < dsDashBoard.Tables[TAB_GraficoDepartamento].Rows.Count - 1)
                            sb.Append(", ");
                    }

                    sb.Append("],");
                    sb.Append("resize: true");
                    sb.AppendLine("}).on('click', function (i, row) {");
                    sb.AppendLine("display(i, row);");
                    sb.AppendLine("});");

                    sb.AppendLine("function display(i, row){");
                    sb.AppendLine("console.log(row.label + \": \" + row.value);");
                    sb.AppendLine("var url = \"/app/paginas/Pedidos.aspx?dashboard=\" + row.id +\"|\" + row.label  + \"|Departamento\"");
                    sb.AppendLine(" window.location.href = url;}");

                    // Gráfico 2 - Status
                    sb.Append("Morris.Bar({");
                    sb.Append("element: 'GraficosStatus',");
                    sb.Append("data: [");

                    for (int nLinhasGraficoDistribuicao = 0; nLinhasGraficoDistribuicao < dsDashBoard.Tables[TAB_GraficoStatus].Rows.Count; nLinhasGraficoDistribuicao++)
                    {
                        sb.Append("{Status: '" + BD.Retorno.DATASET(dsDashBoard, TAB_GraficoStatus, nLinhasGraficoDistribuicao, "idStatus") + "| " + BD.Retorno.DATASET(dsDashBoard, TAB_GraficoStatus, nLinhasGraficoDistribuicao, "sDscStatus") + "',");
                        sb.Append("	Total: " + BD.Retorno.DATASET(dsDashBoard, TAB_GraficoStatus, nLinhasGraficoDistribuicao, "nTotal") + "}");

                        if (nLinhasGraficoDistribuicao < dsDashBoard.Tables[TAB_GraficoStatus].Rows.Count - 1)
                            sb.Append(", ");
                    }

                    sb.Append("],");
                    sb.Append("xkey: 'Status',");
                    sb.Append("ykeys: ['Total'],");
                    sb.Append("labels: ['Total'],");
                    sb.Append("resize: true,");
                    sb.Append("barRatio: 0.4,");
                    sb.Append("xLabelAngle: 45,");
                    sb.Append("hideHover: 'auto'");
                    sb.AppendLine("});");
                    sb.AppendLine("$(document).ready(function() {");
                    sb.AppendLine("    $('#GraficosStatus').on('mouseenter', 'svg rect', function() {");
                    sb.AppendLine("        $(this).addClass('hover-effect hover-cursor'); ");
                    sb.AppendLine("    }).on('mouseleave', 'svg rect', function() {");
                    sb.AppendLine("        $(this).removeClass('hover-effect hover-cursor');");
                    sb.AppendLine("    });");
                    sb.AppendLine("    $('#GraficosStatus').on('click', 'svg rect', function(i, row){");
                    sb.AppendLine("        var status = $(\".morris-hover-row-label\").html();");
                    sb.AppendLine("        var url =\"/app/paginas/Pedidos.aspx?dashboard=\" + status + \"|Status\"; ");
                    sb.AppendLine("        window.location.href = url;");
                    sb.AppendLine("    });");
                    sb.AppendLine("});");

                    sb.Append(" });");

                    // Gráfico 3 - Fluxo
                    sb.Append("Morris.Donut({");
                    sb.Append("element: 'Grafico_Fluxo',");
                    sb.Append("data: [");

                    for (int nLinhasOrigem = 0; nLinhasOrigem < dsDashBoard.Tables[Tabela_GraficoFluxos].Rows.Count; nLinhasOrigem++)
                    {
                        sb.Append("{label: '" + dsDashBoard.Tables[Tabela_GraficoFluxos].Rows[nLinhasOrigem]["sDscFluxo"].ToString() + "',");
                        sb.Append("	value: '" + dsDashBoard.Tables[Tabela_GraficoFluxos].Rows[nLinhasOrigem]["total"].ToString() + "',");
                        sb.Append("	id: '" + dsDashBoard.Tables[Tabela_GraficoFluxos].Rows[nLinhasOrigem]["idFluxo"].ToString() + "'}");

                        if (nLinhasOrigem < dsDashBoard.Tables[Tabela_GraficoFluxos].Rows.Count - 1)
                            sb.Append(", ");
                    }
                    sb.Append("],");
                    sb.Append("resize: true");
                    sb.AppendLine("}).on('click', function (i, row) {");
                    sb.AppendLine("display(i, row);");
                    sb.AppendLine("});");

                    sb.AppendLine("function display(i, row){");
                    sb.AppendLine("console.log(row.label + \": \" + row.value);");
                    sb.AppendLine("var url = \"/app/paginas/Pedidos.aspx?dashboard=\" + row.id +\"|\" + row.label  + \"|Fluxo\"");
                    sb.AppendLine(" window.location.href = url;}");

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DashBoard_" + Guid.NewGuid().ToString(), sb.ToString(), true);
                }
            }
        }

        #endregion

        #region | Eventos

        protected void timer_Atualizar_Tick(object sender, EventArgs e)
        {
            AtualizarDashBoard();
        }

        protected void cmdAtualizar_Click(object sender, EventArgs e) => SalvarPreferenciaUsuario();

        protected void cmdPesquisar_Calendario_Click(object sender, EventArgs e) => AtualizarDashBoard();

        protected void gvUltimasOcorrencias_RowDataBound(object sender, GridViewRowEventArgs e) { if (e.Row.RowType == DataControlRowType.DataRow) e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString(); }

        #endregion

        #region | Script

        protected void RegistraScript()
        {
            FUNCOES.Scripts.AplicaMultiSelect(Page, lstidFluxo.ID, true, "Selecione um Fluxo", "Fluxos", "Todos os Fluxos", true);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$(document).ready(function() {");
            sb.AppendLine("");
            sb.AppendLine("     $('.escondeCaixa .panel-heading').off('click').on('click', function() {");
            sb.AppendLine("         var body = $(this).siblings('.panel-body');");
            sb.AppendLine("         var icon = $(this).find('h3').siblings('i');");
            sb.AppendLine("         console.log(body, icon);");
            sb.AppendLine("         body.toggle('fast');");
            sb.AppendLine("         icon.toggleClass('fa-chevron-up fa-chevron-down');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine($"    $('#{rbTTL.ClientID}, #{rbTodos.ClientID}, #{rbTTS.ClientID}').off('click').on('click', function() {{");
            sb.AppendLine("         __doPostBack('POSTBACK_GRAFICOS', '_blank');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine($"    $('#{ddlsClienteEspecial.ClientID}').off('change').on('change', function() {{");
            sb.AppendLine("         __doPostBack('POSTBACK_GRAFICOS', '_blank');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_RegistraScipt", sb.ToString(), true);
        }

        #endregion
    }
}
