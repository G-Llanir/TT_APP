using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using System.Text.RegularExpressions;
using System.Linq;
using static TT.FrameWork.Identity;
using static TT.FrameWork.Funcoes;

namespace TT_Flow.App
{
    public partial class Pedidos : Page
    {
        string sTituloPagina = "Pedidos";

        int nCol_nNumeroPedido = 0;
        int nCol_sPedidoCompras = 1;
        int nCol_sPedidoCliente = 2;
        int nCol_nControleTT = 3;
        int nCol_sReferencia = 4;
        int nCol_Cliente = 5;
        int nCol_Exportador = 6;
        int nCol_Despachante = 7;
        int nCol_Importador = 8;
        int nCol_Fluxo = 10;
        int nCol_Financeiro = 11;
        int nCol_PaisOrigem = 12;
        int nCol_Modal = 13;
        int nCol_TipoEnvio = 16;
        int nCol_EstimativaEntrega = 17;
        int nCol_PrevisaoEntrega = 18;
        int nCol_ValorTotal = 19;
        int nCol_ValorPrevistoFaturar = 20;
        int nCol_ValorFaturado = 21;
        int nCol_SaldoFaturar = 22;
        int nCol_idPedido = 23;
        int nCol_UF = 24;

        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-PedidoCompras.pdf";

            string DashBoard = "";
            string Ano = "";
            string Faturamento = "";
            string Fluxo = "N";
            string sDscFluxo = "";

            if (Request["sTp"] != null) hddidTipo.Value = Request["sTp"].ToString();
            else hddidTipo.Value = "2";

            manual.Visible = false;
            gvPedidos.Columns[nCol_sPedidoCompras].Visible = false;
            gvPedidos.Columns[nCol_sPedidoCliente].Visible = false;
            div_ddlPedidoCliente.Visible = false;

            // UF só existe em Pedido de Cliente. Em Importação, Compras e nos demais tipos
            // nenhum registro tem o dado, então o filtro só devolveria vazio.
            DIV_UF.Visible = false;
            gvPedidos.Columns[nCol_UF].Visible = false;

            if (hddidTipo.Value == "2")
            {
                sTituloPagina = "Pedidos";
                ValidaPermissao(Permissao.Pedidos.Consultar, true);
                cmdNovoPedido.Text = "Novo Pedido";
                gvPedidos.Columns[nCol_Exportador].Visible = false;
                gvPedidos.Columns[nCol_Despachante].Visible = false;
                gvPedidos.Columns[nCol_PaisOrigem].Visible = false;
                gvPedidos.Columns[nCol_Importador].Visible = false;
                gvPedidos.Columns[nCol_Financeiro].Visible = false;
                gvPedidos.Columns[nCol_Modal].Visible = false;
                gvPedidos.Columns[nCol_nControleTT].Visible = true;
                gvPedidos.Columns[nCol_sPedidoCliente].Visible = true;
                gvPedidos.Columns[nCol_nNumeroPedido].HeaderText = "Pedido";
                DIV_Importador.Visible = false;
                DIV_Financeiro.Visible = false;
                DIV_PaisOrigem.Visible = false;
                DIV_Despachante.Visible = false;
                DIV_idModal.Visible = false;
                div_ddlPedidoCliente.Visible = true;
                DIV_UF.Visible = true;
                gvPedidos.Columns[nCol_UF].Visible = true;
            }
            else if (hddidTipo.Value == "3")
            {
                sTituloPagina = "COMEX - Importação";
                cmdNovoPedido.Text = "Nova Importação";
                gvPedidos.Columns[nCol_Cliente].Visible = false;
                gvPedidos.Columns[nCol_nNumeroPedido].HeaderText = "ID";
                DIV_Empresa.Visible = false;
                gvPedidos.Columns[nCol_Financeiro].Visible = true;
                gvPedidos.Columns[nCol_Fluxo].Visible = false;
                gvPedidos.Columns[nCol_nControleTT].Visible = false;
            }
            else if (hddidTipo.Value == "6")
            {
                DIV_Filtro.Visible = true;
                DIV_Empresa.Visible = false;
                sTituloPagina = "COMEX - Exportação";
                cmdNovoPedido.Text = "Nova Exportação";
                gvPedidos.Columns[nCol_Cliente].Visible = false;
                gvPedidos.Columns[nCol_nNumeroPedido].HeaderText = "ID";
                DIV_Empresa.Visible = false;
                gvPedidos.Columns[nCol_Financeiro].Visible = true;
                gvPedidos.Columns[nCol_Fluxo].Visible = false;
                gvPedidos.Columns[nCol_nControleTT].Visible = false;
            }
            else if (hddidTipo.Value == "7")
            {
                sTituloPagina = "Pedido de Compras";
                ValidaPermissao(Permissao.Compras.Pedido_Compras.Consultar, true);
                cmdNovoPedido.Visible = ValidaPermissao(Permissao.Compras.Pedido_Compras.Incluir);
                cmdNovoPedido.Text = "Novo Pedido de Compras";
                gvPedidos.Columns[nCol_Exportador].Visible = false;
                gvPedidos.Columns[nCol_Despachante].Visible = false;
                gvPedidos.Columns[nCol_PaisOrigem].Visible = false;
                gvPedidos.Columns[nCol_Importador].Visible = false;
                gvPedidos.Columns[nCol_Financeiro].Visible = false;
                gvPedidos.Columns[nCol_Modal].Visible = false;
                gvPedidos.Columns[nCol_nNumeroPedido].Visible = false;
                gvPedidos.Columns[nCol_sPedidoCompras].Visible = true;
                gvPedidos.Columns[nCol_Cliente].HeaderText = "Fornecedor";
                DIV_Importador.Visible = false;
                DIV_Financeiro.Visible = false;
                DIV_PaisOrigem.Visible = false;
                DIV_Despachante.Visible = false;
                DIV_idModal.Visible = false;
                DIV_Departamento.Visible = false;
                DIV_Fluxo.Visible = false;
                DIV_Button.Attributes["class"] = "col-lg-3";
                gvPedidos.Columns[nCol_nControleTT].Visible = false;
                manual.Visible = true;
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

                if (Request["DashBoard"] != null) DashBoard = Request["DashBoard"];
                if (Request["Ano"] != null) Ano = Request["Ano"];
                if (Request["Faturamento"] != null) Faturamento = Request["Faturamento"];
                if (Request["Fluxo"] != null) { sDscFluxo = Request["Fluxo"]; Fluxo = "S"; }
                if (Request["idFluxo"] != null) hddidFluxo.Value = Request["idFluxo"].Contains("|") ? "0" : Request["idFluxo"];
                if (Request["idEmpresa"] != null) hddidEmpresa.Value = Request["idEmpresa"].Contains("|") ? "0" : Request["idEmpresa"];

                if (DashBoard.StartsWith("Total Pedidos")) DashBoard = "Total Pedidos";
                else if (DashBoard.StartsWith("Liberado Para Faturar")) DashBoard = "Liberado Para Faturar";
                else if (DashBoard.StartsWith("Faturado")) DashBoard = "Faturado";
                else if (DashBoard.StartsWith("A faturar")) DashBoard = "A faturar";

                if (!string.IsNullOrEmpty(DashBoard) || !string.IsNullOrEmpty(sDscFluxo))
                {
                    DIV_Filtro.Visible = false;

                    if (Fluxo == "N" && Faturamento == "")
                    {
                        if (char.IsDigit(DashBoard[0]) && DashBoard.Length > 7 && DashBoard.Contains("|"))
                        {
                            string[] partes = DashBoard.Split('|');
                            lblTituloPagina.Text = sTituloPagina + " - " + partes[1];
                            BreadCrumb_Pagina.TitulodaPagina = sTituloPagina + " - " + partes[1];
                        }
                        else
                        {
                            if (DashBoard == "Total Pedidos")
                            {
                                lblTituloPagina.Text = sTituloPagina + " - " + DashBoard + " " + Ano.Replace(",", " / ");
                                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina + " - " + DashBoard + " " + Ano.Replace(",", " / ");
                            }
                            else if (DashBoard == "Liberado Para Faturar")
                            {
                                lblTituloPagina.Text = sTituloPagina + " - " + DashBoard + " " + Ano.Replace(",", " / ");
                                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina + " - " + DashBoard + " " + Ano.Replace(",", " / ");
                            }
                            else if (DashBoard == "Faturado")
                            {
                                lblTituloPagina.Text = sTituloPagina + " - " + DashBoard + " " + Ano.Replace(",", " / ");
                                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina + " - " + DashBoard + " " + Ano.Replace(",", " / ");
                            }
                            else if (DashBoard == "A faturar")
                            {
                                lblTituloPagina.Text = sTituloPagina + " - " + DashBoard + " " + Ano.Replace(",", " / ");
                                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina + " - " + DashBoard + " " + Ano.Replace(",", " / ");
                            }
                            else if (DashBoard == "dashboardComercial")
                            {
                                string sPeriodo = Request["dtInicio"].ToString() == "" || Request["dtFinal"].ToString() == "" ? "" : " - Período " + DateTime.Parse(Request["dtInicio"]).ToString("dd/MM/yyyy") + " a " + DateTime.Parse(Request["dtFinal"]).ToString("dd/MM/yyyy");
                                lblTituloPagina.Text = sTituloPagina + sPeriodo;
                            }
                            else
                            {
                                lblTituloPagina.Text = sTituloPagina + " - " + DashBoard.Replace("7D", "7 D").Replace("_eC", " E-C").Replace("_TTL", " - TTL").Replace("_TTS", " - TTS").Replace("_", " ");
                                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina + " - " + DashBoard.Replace("7D", "7 D").Replace("_eC", " E-C").Replace("_TTL", " - TTL").Replace("_TTS", " - TTS").Replace("_", " ");
                            }
                        }
                    }
                    else if (Faturamento != "")
                    {
                        lblTituloPagina.Text = sTituloPagina + " - " + DashBoard + " " + Ano.Replace(",", " / ");
                        BreadCrumb_Pagina.TitulodaPagina = sTituloPagina + " - " + DashBoard + " " + Ano.Replace(",", " / ");
                    }
                    else
                    {
                        lblTituloPagina.Text = sTituloPagina + " - " + Regex.Replace(sDscFluxo, @"\d+$", "") + " " + Ano.Replace(",", " / ");
                        BreadCrumb_Pagina.TitulodaPagina = sTituloPagina + " - " + sDscFluxo + " " + Ano.Replace(",", " / ");
                    }

                    PesquisarPedido(DashBoard, Ano, Fluxo, sDscFluxo, Faturamento, (Request["sSeparado"] ?? "N").ToString());
                }
                else
                {
                    PopularCombos();

                    if (hddidTipo.Value == "3" || hddidTipo.Value == "6" || hddidTipo.Value == "7") PesquisarPedido("", "", "N", "", "", (Request["sSeparado"] ?? "N").ToString());
                }
            }
        }

        void PesquisarPedido(string DashBoard, string Ano, string Fluxo, string sDscFluxo, string Faturamento, string sSeparado = "N")
        {
            pnResultado.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>();

            if (Fluxo == "N")
            {
                if (DashBoard == "Total Pedidos" || DashBoard == "Liberado Para Faturar" || DashBoard == "Faturado" || DashBoard == "A faturar" || Faturamento != "")
                    vParametros.Add("@sFuncao", "FATURAMENTO");
                else
                {
                    vParametros.Add("@sFuncao", "PESQUISA");
                    vParametros.Add("@idTipo", hddidTipo.Value);
                }

                if (string.IsNullOrEmpty(DashBoard))
                {
                    vParametros.Add("@dtInicio", txtdtInicio.Text);
                    vParametros.Add("@dtFinal", txtdtFinal.Text);
                    vParametros.Add("@idDepartamento", ddlDepartamento.SelectedValue.ToString());
                    vParametros.Add("@idStatus", ddlStatus.SelectedValue.ToString());
                    vParametros.Add("@sidFluxos", string.Join("|", ddlFluxo.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value)));
                    vParametros.Add("@idUsuario", Variaveis.idUsuario());

                    if (hddidTipo.Value == "2")
                    {
                        vParametros.Add("@sPedidoCliente", ddlPedidoCliente.SelectedValue);
                        vParametros.Add("@sUF", ddlUF.SelectedValue);
                    }

                    if (hddidTipo.Value == "7")
                    {
                        if (ValidaPermissao(Permissao.Compras.Pedido_Compras.IncluirInternacional) && ValidaPermissao(Permissao.Compras.Pedido_Compras.IncluirNacional)) vParametros.Add("@sTipoCompra", ddlTipos.SelectedValue);
                        else if (ValidaPermissao(Permissao.Compras.Pedido_Compras.IncluirInternacional))
                        {
                            vParametros.Add("@sTipoCompra", "I");
                            ddlTipos.SelectedValue = "I";
                            ddlTipos.Attributes.Add("disabled", "disabled");
                        }
                        else if (ValidaPermissao(Permissao.Compras.Pedido_Compras.IncluirNacional))
                        {
                            vParametros.Add("@sTipoCompra", "N");
                            ddlTipos.SelectedValue = "N";
                            ddlTipos.Attributes.Add("disabled", "disabled");
                        }

                        if (!ValidaPermissao(Permissao.Compras.Pedido_Compras.IncluirInternacional) && !ValidaPermissao(Permissao.Compras.Pedido_Compras.IncluirNacional))
                        {
                            vParametros.Add("@sTipoCompra", "0");
                        }
                    }

                    if (hddidTipo.Value == "2" || hddidTipo.Value == "7") vParametros.Add("@idCliente", ddlCliente.SelectedValue.ToString());
                    else if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
                    {
                        vParametros.Add("@Importacao_idFornecedor", ddlCliente.SelectedValue.ToString());
                        vParametros.Add("@Importacao_idDespachante", ddlImportacao_idDespachante.SelectedValue.ToString());
                        vParametros.Add("@Importacao_idPais", ddlImportacao_idPais.SelectedValue.ToString());
                        vParametros.Add("@idModal", ddlidModal.SelectedValue.ToString());
                        vParametros.Add("@Importacao_idImportador", ddlImportador.SelectedValue);
                        vParametros.Add("@nStatus", ddlFinanceiro.SelectedValue);
                    }

                    vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                    vParametros.Add("@sPesquisa", txtPesquisa.Text);
                }
                else
                {
                    if (DashBoard != "dashboardComercial")
                    {
                        vParametros.Add("@sDashBoard", DashBoard);
                        vParametros.Add("@sFiltraFluxosSeparados", ValidaPermissao(Permissao.Pedidos.Visualizar_FluxosSeparados__Dashboard) ? sSeparado : "N");
                        vParametros.Add("@Ano", Ano);
                        vParametros.Add("@sFaturamento", Faturamento);
                        vParametros.Add("@idFluxo", hddidFluxo.Value);
                        vParametros.Add("@idEmpresa", hddidEmpresa.Value);
                        vParametros.Add("@sidVendedores", Request["idVendedor"] != null ? Request["idVendedor"] : "0");
                        vParametros.Add("@sidFluxos", Request["idFluxo"] != null ? Request["idFluxo"] : "0");
                        vParametros.Add("@sidEmpresas", Request["idEmpresa"] != null ? Request["idEmpresa"] : "0");
                        vParametros.Add("@idUsuario", Variaveis.idUsuario());
                    }
                    else
                    {
                        vParametros.Add("@sDashBoard", DashBoard);
                        vParametros.Add("@dtInicio", Request["dtInicio"]);
                        vParametros.Add("@dtFInal", Request["dtFinal"]);
                        vParametros.Add("@idStatus", Request.QueryString["idStatus"] != null ? Request["idStatus"] : "0");
                        vParametros.Add("@sidFluxos", Request["idFluxo"] != null ? Request["idFluxo"] : "0");
                        vParametros.Add("@sidEmpresas", Request["idEmpresa"] != null ? Request["idEmpresa"] : "0");
                        vParametros.Add("@sidVendedores", Request["idVendedor"] != null ? Request["idVendedor"] : "0");
                        vParametros.Add("@idCliente", Request["idCliente"] != null ? Request["idCliente"] : "0");
                        vParametros.Add("@nMes", Request["nMes"] != null ? Request["nMes"] : "0");
                        vParametros.Add("@nAno", Request["nAno"] != null ? Request["nAno"] : "0");
                        vParametros.Add("@idUsuario", Variaveis.idUsuario());
                    }
                }
            }
            else
            {
                vParametros.Add("@sFuncao", "FATURAMENTO");
                vParametros.Add("@Fluxo", Fluxo);
                vParametros.Add("@sDscFluxo", Regex.Replace(sDscFluxo, @"\d+$", ""));
                vParametros.Add("@Ano", Ano);
                vParametros.Add("@idFluxo", hddidFluxo.Value);
                vParametros.Add("@idEmpresa", hddidEmpresa.Value);
            }

            DataTable tb = BD.ExecutarDataTable("sp_Consulta_tbl_Flow_Pedidos", vParametros);

            Session["Dashboard"] = "";

            if (tb.Rows.Count > 0)
            {
                if (hddidTipo.Value == "2") ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", GRID.DataBindComScriptData(gvPedidos, tb, 0, new int[3] { 5, 10, 11 }, "desc", "false", "''"), true); // Pedido de Vendas
                else if (hddidTipo.Value == "3") ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", GRID.DataBindComScriptData(gvPedidos, tb, 0, new int[3] { 5, 11, 12 }, "desc", "false", "''"), true); // COMEX - Importação
                else if (hddidTipo.Value == "6") ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", GRID.DataBindComScriptData(gvPedidos, tb, 0, new int[3] { 5, 11, 12 }, "desc", "false", "''"), true); // COMEX - Exportação
                else if (hddidTipo.Value == "7") ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", GRID.DataBindComScriptData(gvPedidos, tb, 3, new int[3] { 3, 7, 8 }, "desc", "false", "''"), true); // Pedido de Compras
                else ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", GRID.DataBindComScript(gvPedidos, tb, 0, "desc"), true);

                pnResultado.Visible = true;
            }
            else MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
        }

        void PopularCombos()
        {
            Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa'", "idEmpresa", "sDscEmpresa", false, "Todas as Empresas", "0");

            if (hddidTipo.Value == "6") Popula_Combo(ddlStatus, "sp_Select 'Flow_Status', 3", "idStatus", "sDscStatus", false, "Todos os Status", "0");
            else Popula_Combo(ddlStatus, "sp_Select 'Flow_Status', " + hddidTipo.Value, "idStatus", "sDscStatus", false, "Todos os Status", "0");

            ddlStatus.SelectedIndex = 1;

            if (hddidTipo.Value == "2")
            {
                Popula_Combo(ddlCliente, "sp_Select 'Flow_Clientes_Pedido', " + Variaveis.idParceiro() + ", @idFiltro=1", "idCliente", "sRazaoSocial", false, "Todos os Clientes", "0");
                Popula_Combo(ddlPedidoCliente, "sp_Consulta_tbl_Flow_Pedidos 'Flow_Pedidos_sPedidoCliente'", "id", "sPedidoCliente", false, "Pedidos Clientes", "");
                Popula_Combo(ddlDepartamento, "sp_Select 'Flow_Departamentos_Comercial'", "idDepartamento", "sDscDepartamento", false, "Todos os Departamentos", "0");
                Popula_Combo(ddlFluxo, "sp_Select 'Flow_Fluxo'", "idFluxo", "sDscFluxo", false, "Todos os Fluxos", "0");

                // Mesma fonte de UF que o resto do sistema usa (endereços, empresas).
                Popula_Combo(ddlUF, "sp_Select 'Flow_Estado'", "sEstado", "sEstado", false, "Todos os Estados", "");

                // Um terço dos pedidos históricos não tem UF em fonte nenhuma. Sem esta
                // opção eles sumiriam de qualquer filtro por estado, em silêncio.
                ddlUF.Items.Insert(1, new ListItem("Sem UF informada", "SEM"));

                if (Variaveis.idParceiro() != "0")
                {
                    DIV_Empresa.Visible = false;
                    DIV_Departamento.Visible = false;
                    DIV_Fluxo.Visible = false;
                    cmdNovoPedido.Visible = false;
                    ddlCliente.SelectedValue = Variaveis.idParceiro();
                    ddlCliente.Attributes.Add("enabled", "enabled");
                }
            }
            else if (hddidTipo.Value == "7")
            {
                Popula_Combo(ddlCliente, "sp_Select 'Flow_Parceiros_Fornecedores'", "idCliente", "Razao_CNPJ", false, "Todos os Fornecedores", "0");
                Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Todas as Empresas", "0");
                Popula_Combo(ddlStatus, "sp_Select 'Flow_Status', " + hddidTipo.Value, "idStatus", "sDscStatus", false, "Todos os Status", "0");

                ddlStatus.SelectedIndex = 1;
            }
            else
            {
                Popula_Combo(ddlImportacao_idPais, "sp_Select 'tbl_Flow_WMS_Produtos_Origem'", "idPais", "sDscPais", false, "País de Origem", "0");
                Popula_Combo(ddlCliente, "sp_Select 'Flow_COMEX_Fornecedor'", "idParceiro", "sRazaoSocial", false, "Todos os Exportadores", "0");
                Popula_Combo(ddlDepartamento, "sp_Select 'Flow_Departamentos_COMEX'", "idDepartamento", "sDscDepartamento", false, "Todos os Departamentos", "0");
                Popula_Combo(ddlImportacao_idDespachante, "sp_Select 'Flow_Parceiro_Importacao_Despachante'", "idParceiro", "sRazaoSocial", false, "Todos os Despachantes", "0");
                Popula_Combo(ddlidModal, "sp_Select 'tbl_Flow_Pedidos_Modal'", "idModal", "sDscModal", false, "Todos os Modais", "0");
                Popula_Combo(ddlImportador, "sp_Select 'Flow_Parceiro_Importacao_Importador'", "idParceiro", "sRazaoSocial", false, "Todos os Importadores", "0");

                DIV_Fluxo.Visible = false;
            }
        }

        protected void gvPedidos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow) e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();

            string Faturamento = "";
            string DashBoard = "";
            string Fluxo = "";

            if (Request["Faturamento"] != null) Faturamento = Request["Faturamento"];
            if (Request["DashBoard"] != null) DashBoard = Request["DashBoard"];
            if (Request["Fluxo"] != null) Fluxo = "S";

            if (string.IsNullOrEmpty(Request["sTp"]) || Request["sTp"] == "2")
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    try
                    {
                        (e.Row.Cells[nCol_nNumeroPedido].Controls[0] as HyperLink).NavigateUrl += "&sSession=S";
                        (e.Row.Cells[nCol_sPedidoCliente].Controls[0] as HyperLink).NavigateUrl += "&sSession=S";
                        (e.Row.Cells[nCol_nControleTT].Controls[0] as HyperLink).NavigateUrl += "&sSession=S";
                        (e.Row.Cells[nCol_sReferencia].Controls[0] as HyperLink).NavigateUrl += "&sSession=S";

                        Session["Dashboard"] += e.Row.Cells[nCol_idPedido].Text + "|";
                    }
                    catch { }
                }
            }

            if (DashBoard.StartsWith("Total Pedidos")) DashBoard = "Total Pedidos";
            else if (DashBoard.StartsWith("Liberado Para Faturar")) DashBoard = "Liberado Para Faturar";
            else if (DashBoard.StartsWith("Faturado")) DashBoard = "Faturado";
            else if (DashBoard.StartsWith("A faturar")) DashBoard = "A faturar";

            if (hddidTipo.Value != "2") GRID.EsconderColunas(e, nCol_sPedidoCliente);

            if (DashBoard == "Total Pedidos" || DashBoard == "Liberado Para Faturar" || DashBoard == "Faturado" || DashBoard == "A faturar" || Faturamento != "" || Fluxo == "S") GRID.EsconderColunas(e, nCol_TipoEnvio, nCol_EstimativaEntrega, nCol_PrevisaoEntrega);
            else
            {
                if (hddidTipo.Value != "7") GRID.EsconderColunas(e, nCol_ValorTotal);

                GRID.EsconderColunas(e, nCol_ValorPrevistoFaturar, nCol_ValorFaturado, nCol_SaldoFaturar);
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) => PesquisarPedido("", "", "N", "", "", (Request["sSeparado"] ?? "N").ToString());

        protected void cmdNovoPedido_Click(object sender, EventArgs e)
        {
            if (hddidTipo.Value == "2") DirecionaPagina("app/Paginas/Clientes.aspx?tp=np");
            else if (hddidTipo.Value == "3") DirecionaPagina("app/Paginas/Pedidos_Detalhe.aspx?sTp=3&id=0");
            else if (hddidTipo.Value == "6") DirecionaPagina("app/Paginas/Pedidos_Detalhe.aspx?sTp=6&id=0");
            else if (hddidTipo.Value == "7") DirecionaPagina("app/Paginas/Pedidos_Detalhe.aspx?sTp=7&id=0");
        }
    }
}