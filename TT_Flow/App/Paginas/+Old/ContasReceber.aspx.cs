using MathNet.Numerics.Providers.SparseSolver;
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
using TT_Flow.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using GRID = TT.FrameWork.Grid;
using Identity = TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.Adm.Financeiro
{
    public partial class ContasReceber : Page
    {
        string sTituloPagina = "Contas a Receber";
        string sPagina_NovoRegistro = "app/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx";

        #region | Classes

        public List<cls_Adiantamento> bs_Adiantamento
        {
            get
            {
                if (ViewState["bs_Adiantamento"] == null) ViewState["bs_Adiantamento"] = new List<cls_Adiantamento>();
                return (List<cls_Adiantamento>)ViewState["bs_Adiantamento"];
            }
            set { ViewState["bs_Adiantamento"] = value; }
        }

        public List<cls_Adiantamento_Detalhe> bs_Adiantamento_Detalhe
        {
            get
            {
                if (ViewState["bs_Adiantamento_Detalhe"] == null) ViewState["bs_Adiantamento_Detalhe"] = new List<cls_Adiantamento_Detalhe>();
                return (List<cls_Adiantamento_Detalhe>)ViewState["bs_Adiantamento_Detalhe"];
            }
            set { ViewState["bs_Adiantamento_Detalhe"] = value; }
        }

        #endregion
        
        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "ManualdoUsuarioContasReceber.pdf";

            string DashBoard = "";
            string AnoSelecionado = Request["sAno"] != null ? Request["sAno"].ToString() : "";
            string Empresa = Request["sEmpresa"] != null ? Request["sEmpresa"].ToString() : "";
            string Data = Request["sData"] != null ? Request["sData"].ToString() : "";
            string Semana = Request["sSemana"] != null ? Request["sSemana"].ToString() : "";
            string Mes = Request["sMes"] != null ? Request["sMes"].ToString() : "";

            FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasReceber.Consultar, true) ;
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasReceber.Incluir);

            if (!FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasReceber.Adiantar))
            {
                cmdAdiantar.Visible = false;
                cmdAdiantamento.Visible = false;
            }
            else
            {
                cmdAdiantar.Visible = true;
                cmdAdiantamento.Visible = true;
            }

            if (!IsPostBack)
            {
                if (Session["SalvoComSucesso"] != null && (bool)Session["SalvoComSucesso"])
                {
                    MensagemPagina.MostraMensagem_Sucesso("Títulos Adiantados com sucesso");
                    Session["SalvoComSucesso"] = false;
                }

                if (Session["AdiantarTitulos"] != null && (bool)Session["AdiantarTitulos"])
                {
                    DashBoard = "Adiantamento";
                    Session["AdiantarTitulos"] = false;
                    DIV_Adiantamento.Visible = true;
                    ViewState["PaginaAdiantamento"] = true; 
                }
                else DIV_Adiantamento.Visible = false;

                if (Request["DashBoard"] != null) DashBoard = Request["DashBoard"];

                if (DashBoard != "")
                {
                    DIV_Filtro.Visible = false;
                    string tituloAdicional = "";

                    if (Request["sEmpresa"] != null)
                    {
                        if (Request["sEmpresa"] == "0") Empresa = " - Todas as Empresas";
                        else
                        {
                            Empresa = EmpresaDescricao(Request["sEmpresa"].ToString());
                            Empresa = " - " + Empresa;
                        }
                    }
                                        
                    string sPeriodo = "";
                    string sStatus = "";
                    if (Request["dtInicio"] != null && Request["dtFinal"] != null)
                        sPeriodo = Request["dtInicio"].ToString() == "" || Request["dtFinal"].ToString() == "" ? "" : " - Período " + DateTime.Parse(Request["dtInicio"]).ToString("dd/MM/yyyy") + " a " + DateTime.Parse(Request["dtFinal"]).ToString("dd/MM/yyyy");

                    if (Request["sStatus"] != null) sStatus = Request["sStatus"].ToString() == "" ? "Todos os Status" : Request["sStatus"].ToString();

                    switch (DashBoard)
                    {
                        case "VenceHoje": tituloAdicional = "Vencendo Hoje" + AnoSelecionado + Empresa; break;
                        case "VenceEm7": tituloAdicional = "Vencendo em até 7 Dias" + AnoSelecionado + Empresa; break;
                        case "VenceEm30": tituloAdicional = "Vencendo em até 30 Dias" + AnoSelecionado + Empresa; break;
                        case "VencerMaisdeTrinta": tituloAdicional = "Vencendo +30 Dias" + AnoSelecionado + Empresa; break;
                        case "TotalAberto": tituloAdicional = "Total Geral" + AnoSelecionado + Empresa; break;
                        case "EmAtraso": tituloAdicional = "Em Atraso em " + AnoSelecionado + Empresa; break;
                        case "Total Liquidado": tituloAdicional = "Recebimentos Liquidados em " + AnoSelecionado + Empresa; break;
                        case "RecebimentosNoAno": tituloAdicional = "Recebidos em " + AnoSelecionado + Empresa; break;
                        case "CaixaRecebidosHoje": tituloAdicional = "Recebidos em " + Data + Empresa; break;
                        case "CaixaRecebidosSemana": tituloAdicional = "Recebidos Semana " + Semana + Empresa; break;
                        case "CaixaRecebidosMes": tituloAdicional = "Recebidos Mês " + Mes + Empresa; break;
                        case "AdiantadoHoje": tituloAdicional = "Adiantado em " + Data + Empresa; break;
                        case "AdiantadoSemana": tituloAdicional = "Adiantados  Semana " + Semana + Empresa; break;
                        case "AdiantadoMes": tituloAdicional = "Adiantados  Ano " + Mes + Empresa; break;
                        case "Adiantamento": tituloAdicional = "Adiantamento de Títulos"; break;
                        case "EmprestimoRecebidosHoje": tituloAdicional = "Pago em " + Data + Empresa; break;
                        case "EmprestimoRecebidosSemana": tituloAdicional = "Pagos Semana " + Semana + Empresa; break;
                        case "EmprestimoRecebidosMes": tituloAdicional = "Pagos Ano " + Mes + Empresa; break;
                        case "ComparativoRecebidosHoje": tituloAdicional = "Receita em " + Data + Empresa; break;
                        case "ComparativoRecebidosSemana": tituloAdicional = "Receita Semana " + Semana + Empresa; break;
                        case "ComparativoRecebidosMes": tituloAdicional = "Receita Ano " + Mes + Empresa; break;
                        case "ContabilRecebidosHoje": tituloAdicional = "Receita em " + Data + Empresa; break;
                        case "ContabilRecebidosSemana": tituloAdicional = "Receita Semana " + Semana + Empresa; break;
                        case "ContabilRecebidosMes": tituloAdicional = "Receita Ano " + Mes + Empresa; break;
                        case "EmissaoNF": tituloAdicional = "Emissão NF " + Mes + Empresa; break;
                        case "Inadimplente": tituloAdicional = "Inadimplentes " + Empresa; break;
                        case "Protestado": tituloAdicional = "Prostestados " + Empresa; break;
                        case "CobrancaJudicial": tituloAdicional = "Cobranças Judiciais"; break;
                        case "relatorioFinanceiro": tituloAdicional = sStatus + sPeriodo; break;
                    }

                    lblTituloPagina.Text = sTituloPagina + " - " + tituloAdicional;
                    BreadCrumb_Pagina.TitulodaPagina = sTituloPagina + " - " + tituloAdicional;

                    Pesquisar(DashBoard);
                }
                else
                {
                    FUNCOES.Popula_Combo(ddlidContabil, "sp_Select 'Flow_Contabil_Financeiro'", "idContabil", "sDscCodContabil", false, "Selecione um Código Contábil", "0");
                    FUNCOES.Popula_Combo(ddlidFormaRecebimento, "sp_Select 'tbl_Flow_CondicaodePagamento_Tipo'", "idTipoCondicaoPagamento", "sDscTipoCondicaoPagamento", false, "Selecione a forma de Recebimento", "0");
                    FUNCOES.Popula_Combo(ddlidCentroDeCusto, "sp_Select 'Flow_Adm_CentroDeCusto'", "idCentroDeCusto", "sDescricao", false, "Selecione um Centro de Custo", "0");
                    FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + Identity.Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
                    FUNCOES.Popula_Combo(ddlidCategoriaReceber, "sp_Select 'Flow_Adm_Contas_Receber_Categoria'", "idCategoriaReceber", "sDscCategoriaReceber", false, "Selecione a Categoria", "0");
                    FUNCOES.Popula_Combo(ddlidConta_Info_Rec, "sp_Select 'tbl_Flow_Adm_ContasBancarias'", "idConta", "sDscConta", false, "Selecione uma Conta", "0");
                    FUNCOES.Popula_Combo(ddlsCLiente, "sp_Manipula_tbl_Flow_Adm_Contas_Pagar 'Consulta_Parceiro_Relatorio', @sLocal=Receber, @sAgrupar=S", "idParceiro", "sRazaoSocial", false, "Todos os Clientes", "0");

                    BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                    pnResultado.Visible = false;
                    ddlsStatus.SelectedValue = "Em Aberto";
                }

                if (Request["sFiltro"] != null && Request["idParceiro"] != null)
                {
                    DIV_Filtro.Visible = false;
                    string sFiltro = Request["sFiltro"], sTituloPagina = "";

                    switch (sFiltro)
                    {
                        case "mediaCompraMensal": sTituloPagina = "Média Compras Mensais (últimos 12 meses)"; break;
                        case "Pontual": sTituloPagina = "Média de Pagamentos - Pontual"; break;
                        case "Atraso_1_5": sTituloPagina = "Média de Pagamentos - Atraso de 1 a 5 dias"; break;
                        case "Atraso_6_10": sTituloPagina = "Média de Pagamentos - Atraso de 6 a 10 dias"; break;
                        case "Atraso_10_30": sTituloPagina = "Média de Pagamentos - Atraso de 10 a 30 dias"; break;
                        case "Atraso_Acima_30": sTituloPagina = "Média de Pagamentos - Atraso Acima de 30 dias"; break;
                        case "Vencidos": sTituloPagina = "Média de Títulos em Aberto - Vencidos"; break;
                        case "Vencendo_Hoje": sTituloPagina = "Média de Títulos em Aberto - Vencendo Hoje"; break;
                        case "Ate_7_Dias": sTituloPagina = "Média de Títulos em Aberto - Até 7 dias"; break;
                        case "Ate_15_Dias": sTituloPagina = "Média de Títulos em Aberto - Até 15 dias"; break;
                        case "Ate_30_Dias": sTituloPagina = "Média de Títulos em Aberto - Até 30 dias"; break;
                        case "Mais_30_Dias": sTituloPagina = "Média de Títulos em Aberto - Mais de 30 dias"; break;
                    }

                    lblTituloPagina.Text = sTituloPagina;
                    PesquisarComFiltro(sFiltro, Request["idParceiro"]);
                }
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

            RegistraScriptModalAdiantamento();
            txtPesquisa.Focus();           

            List<int> indexesToIgnore = new List<int> { 2, 20, 21 };
            List<int> indexesParaIniciarOcultado = new List<int> { };

            if ((ddlsStatus.SelectedValue != "Liquidado" && ddlidDataPesquisa.SelectedValue != "2") || ddlsStatus.SelectedValue == "")
            {
                indexesParaIniciarOcultado.Add(9);
                indexesParaIniciarOcultado.Add(11);
                indexesParaIniciarOcultado.Add(12);
                indexesParaIniciarOcultado.Add(13);
                indexesParaIniciarOcultado.Add(15);
                indexesParaIniciarOcultado.Add(18);
                indexesParaIniciarOcultado.Add(19);
            }            
            else if (ddlidDataPesquisa.SelectedValue == "2" && ddlsStatus.SelectedValue != "Liquidado")
            {
                indexesParaIniciarOcultado.Add(9);                  
                indexesParaIniciarOcultado.Add(15);
            }
            else
            {
                indexesParaIniciarOcultado.Add(9);
                indexesParaIniciarOcultado.Add(14);
                indexesParaIniciarOcultado.Add(15);
            }

            Grid.BotoesOcultarColunaComFiltro(placeholderButtons, dtgvConsulta, this, indexesToIgnore, indexesParaIniciarOcultado);
        }

        private void PesquisarComFiltro(string sFiltro, string idParceiro)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_TITULOS_GRAFICO" },
                { "@sFiltro",  sFiltro },
                { "@idCliente", idParceiro }
            };
            DataSet dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Clientes", vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out string sErro))
            {
                if (dsPesquisa.Tables[0].Rows.Count > 0)
                {
                    pnResultado.Visible = true;
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, dsPesquisa.Tables[0], new int[3] { 2, 3, 16 }, "desc", "true", "800"), true);
                    GRID.SomarColunas(dtgvConsulta, true, GRID.Formatação.Moeda, 10, 11, 12, 13);
                }
                else MensagemPagina.MostraMensagem_Erro("Nenhum Recebimento Localizado");
            }
            else MensagemPagina.MostraMensagem_Erro(sErro);
        }

        private string EmpresaDescricao(string sEmpresa)
        {
            DropDownList ddlTemp = new DropDownList();
            FUNCOES.Popula_Combo(ddlTemp, "sp_Select 'tbl_Flow_Empresa'", "idEmpresa", "sDscEmpresa");

            foreach (ListItem item in ddlTemp.Items) { if (item.Value == sEmpresa) return item.Text; }

            return "";
        }

        protected void Pesquisar(string DashBoard)
        {
            PopularCombos();

            btnConfirmarAdiantamento.Visible = false;
            tituloResumo.Visible = false;
            pnResultado.Visible = false;

            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string> { { "@sFuncao", "CONSULTAR" } };

                if (DashBoard != "Adiantamento")
                { 
                    if (DashBoard == "")
                    {
                        vParametros.Add("@sUsuarioLogado", HttpContext.Current.Session["idUsuario"].ToString());
                        vParametros.Add("@sPermissao", FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasReceber.VisualizarTudo) ? "S" : "N");
                        vParametros.Add("@idContabil", ddlidContabil.SelectedValue);
                        vParametros.Add("@idFormaRecebimento", ddlidFormaRecebimento.SelectedValue);
                        vParametros.Add("@idCentroDeCusto", ddlidCentroDeCusto.SelectedValue);
                        vParametros.Add("@dtInicio", txtdtInicio.Text);
                        vParametros.Add("@dtFinal", txtdtFinal.Text);
                        vParametros.Add("@idDataPesquisa", ddlidDataPesquisa.SelectedValue);
                        vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                        vParametros.Add("@sPesquisa", txtPesquisa.Text);
                        vParametros.Add("@idCategoriaReceber", ddlidCategoriaReceber.SelectedValue);
                        vParametros.Add("@sTituloAdiantado", ddlsTituloAdiantado.SelectedValue);
                        vParametros.Add("@idConta_Info_Rec", ddlidConta_Info_Rec.SelectedValue);
                        vParametros.Add("@sConciliado", ddlsConciliado.SelectedValue);
                        vParametros.Add("@idParceiro", ddlsCLiente.SelectedValue);

                        //if (ddlidDataPesquisa.SelectedValue == "2" && txtdtInicio.Text != "")
                        //{
                        //    vParametros.Add("@sStatus", "");
                        //    ddlsStatus.SelectedValue = "";
                        //}
                        //else 
                        vParametros.Add("@sStatus", ddlsStatus.SelectedValue);
                    }
                    else
                    {
                        vParametros.Add("@sDashBoard", DashBoard);

                        //------------------------------------------------------------------------------------
                        vParametros.Add("@sUsuarioLogado", HttpContext.Current.Session["idUsuario"].ToString());
                        vParametros.Add("@sPermissaoReceber", FUNCOES.ValidaPermissao(Permissao.Financeiro.DashboardAdm.VisualizarTudoReceber) ? "S" : "s");
                        //------------------------------------------------------------------------------------

                        if (Request["sAno"] != null) vParametros.Add("@AnoSelecionado", Request["sAno"].ToString());
                        if (Request["sEmpresa"] != null) vParametros.Add("@idEmpresaSelecionada", Request["sEmpresa"].ToString());
                        if (Request["sData"] != null) vParametros.Add("@DataSelecionada", Request["sData"].ToString());
                        if (Request["sSemana"] != null) vParametros.Add("@SemanaSelecionada", Request["sSemana"].ToString());
                        if (Request["sMes"] != null) vParametros.Add("@MesSelecionada", Request["sMes"].ToString());                        
                        if (!string.IsNullOrEmpty(Request["sTipo"])) vParametros.Add("@sTipo", Request["sTipo"].ToString());                        
                        if (!string.IsNullOrEmpty(Request["dtInicio"])) vParametros.Add("@dtInicio", Request["dtInicio"].ToString());                        
                        if (!string.IsNullOrEmpty(Request["dtFinal"])) vParametros.Add("@dtFinal", Request["dtFinal"].ToString());                        
                        if (!string.IsNullOrEmpty(Request["sStatus"])) vParametros.Add("@sStatus", Request["sStatus"].ToString());                        
                        if (!string.IsNullOrEmpty(Request["idParceiro"])) vParametros.Add("@idParceiro", Request["idParceiro"].ToString());                        
                        if (!string.IsNullOrEmpty(Request["sAgrupar"])) vParametros.Add("@sAgrupar", Request["sAgrupar"].ToString());
                        if (!string.IsNullOrEmpty(Request["sFiltroFinanceiro"])) vParametros.Add("@sFiltro", Request["sFiltroFinanceiro"].ToString());
                        if (!string.IsNullOrEmpty(Request["sEmpresaRelatorio"])) vParametros.Add("@sEmpresaRelatorio", Request["sEmpresaRelatorio"].ToString());
                    }
                }
                else
                {
                    vParametros.Add("@sAdiantamento", "Adiantamento");
                    vParametros.Add("@sUsuarioLogado", HttpContext.Current.Session["idUsuario"].ToString());
                    vParametros.Add("@sPermissao", FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasReceber.VisualizarTudo) ? "S" : "s");
                }

                DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Adm_Contas_Receber", vParametros);

                if (tb.Rows.Count > 0)
                {
                    pnResultado.Visible = true;
                    if (ViewState["PaginaAdiantamento"] != null && (bool)ViewState["PaginaAdiantamento"])
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", Grid.DataBindComScriptDataPaging(dtgvConsulta, tb, false, new int[3] { 3, 4, 17 }, "desc", "false", "''"), true);
                        GRID.SomarColunas(dtgvConsulta, true, GRID.Formatação.Moeda, 12, 14);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, tb, new int[3] { 2, 3, 17 }, "desc", "true", "800"), true);
                        GRID.SomarColunas(dtgvConsulta, true, GRID.Formatação.Moeda, 11, 12, 13, 14);
                    }                    
                }
                else MensagemPagina.MostraMensagem_Erro("Nenhum Recebimento Localizado");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao Consultar: " + ex.Message);
            }
        }

        private bool ValidarDados()
        {
            string sMensagemErro = "";
            if (txtdtInicio.Text != "" || txtdtFinal.Text != "") sMensagemErro += Validacoes.ValidaDatas(txtdtInicio.Text, txtdtFinal.Text);
            if (sMensagemErro != "") { MensagemPagina.MostraMensagem_Erro(sMensagemErro); return false; }

            return true;
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (ViewState["PaginaAdiantamento"] == null || !(bool)ViewState["PaginaAdiantamento"])
            {
                GRID.EsconderColunas(e, 0, 2);
                GRID.EsconderColunas(e, 20, 21);
            }
            else
            {
                e.Row.Cells[2].Style["display"] = "none";
                e.Row.Cells[11].Style["display"] = "none";
                e.Row.Cells[13].Style["display"] = "none";
                e.Row.Cells[18].Style["display"] = "none";
                e.Row.Cells[19].Style["display"] = "none";
                e.Row.Cells[20].Style["display"] = "none";
                e.Row.Cells[21].Style["display"] = "none";
            }

            if (e.Row.RowType == DataControlRowType.DataRow) e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
        }

        protected void ddlsTipoCalculo_SelectedIndexChanged(object sender, EventArgs e)
        {
            div_valoresAdiantamento.Visible = true;

            if (ddlsTipoCalculo.SelectedValue == "M")
            {
                div_totalIOF.Visible = true;
                div_totalJuros.Visible = true;
                div_valorLiberado.Visible = true;
            }
            else if (ddlsTipoCalculo.SelectedValue == "P")
            {
                div_totalIOF.Visible = false;
                div_totalJuros.Visible = false;
                div_valorLiberado.Visible = false;
            }
            else
            {
                div_totalIOF.Visible = false;
                div_totalJuros.Visible = false;
                div_valorLiberado.Visible = false;
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) { if (ValidarDados()) Pesquisar(""); }

        protected void cmdNovo_Click(object sender, EventArgs e) => FUNCOES.DirecionaPagina(sPagina_NovoRegistro);       

        #region | Adiantamentos

        protected void btnSimularAdiantamento_Click(object sender, EventArgs e)
        {
            string sMensagemErro = "";
            bs_Adiantamento.Clear();
            bs_Adiantamento_Detalhe.Clear();

            gvSimulacaoAdiantamento.DataSource = null;
            gvSimulacaoAdiantamento.DataBind();

            gvSimulacaoAdiantamento_Detalhe.DataSource = null;
            gvSimulacaoAdiantamento_Detalhe.DataBind();

            if (ValidarDadosModal())
            { 
                DateTime dtEmissaoInserida;
                dtEmissaoInserida = DateTime.Parse(txtdtEmissaoAdiantamento.Text);          
            
                var adiantamentoCalculado = CalcularAdiantamento();

                if (adiantamentoCalculado != null)
                {
                    bs_Adiantamento.Clear();
                    bs_Adiantamento.Add(adiantamentoCalculado);
                    
                    gvSimulacaoAdiantamento.DataSource = bs_Adiantamento;
                    gvSimulacaoAdiantamento.DataBind();

                    simulacaoGrid.Visible = true;
                    btnConfirmarAdiantamento.Visible = true;
                    tituloResumo.Visible = true;

                    int qtdTitulosCalculados = 0;
                    foreach (GridViewRow item in dtgvConsulta.Rows)
                    {
                        CheckBox chk = (CheckBox)item.FindControl("chkTitulo_Selecionado");
                        if (chk != null)
                        { 
                            if (chk.Checked)
                            {
                                try
                                {
                                    int idContasReceber = Convert.ToInt32(dtgvConsulta.DataKeys[item.RowIndex]["idContasReceber"]);

                                    decimal valorOriginal;
                                    if (!Decimal.TryParse(item.Cells[11].Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("pt-BR"), out valorOriginal))
                                    {
                                        continue;
                                    }

                                    DateTime dataVencimento;
                                    if (!DateTime.TryParseExact(item.Cells[5].Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataVencimento))
                                    {
                                        continue;
                                    }

                                    string sDscCliente = item.Cells[3].Text;

                                    if (string.IsNullOrEmpty(sDscCliente))
                                    {
                                        continue;
                                    }

                                    qtdTitulosCalculados += 1;

                                    var adiantamentoDetalhe = CalcularAdiantamento_Detalhe(idContasReceber, valorOriginal, dataVencimento, sDscCliente, adiantamentoCalculado.nContadorAdiantamento, qtdTitulosCalculados);                                   
                                    bs_Adiantamento_Detalhe.Add(adiantamentoDetalhe);
                                    
                                }
                                catch (Exception ex)
                                {
                                    MensagemPagina.MostraMensagem_Erro("Erro ao Calcular Detalhes: " + ex.Message);
                                }
                                simulacaoGrid_Detalhe.Visible = true;
                            }
                        }
                    }

                    txtdtEmissaoAdiantamento.Text = dtEmissaoInserida.ToString("yyyy-MM-dd");

                    if (ddlsTipoCalculo.SelectedValue == "M")
                    {
                        gvSimulacaoAdiantamento_Detalhe.Columns[5].Visible = false;
                        gvSimulacaoAdiantamento_Detalhe.Columns[6].Visible = false;
                        gvSimulacaoAdiantamento_Detalhe.Columns[7].Visible = false;
                        gvSimulacaoAdiantamento_Detalhe.Columns[8].Visible = false;
                        gvSimulacaoAdiantamento_Detalhe.Columns[9].Visible = false;
                    }

                    gvSimulacaoAdiantamento_Detalhe.DataSource = bs_Adiantamento_Detalhe;
                    gvSimulacaoAdiantamento_Detalhe.DataBind();

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showModal", "$('#Modal_Adiantamento').modal('show'); $('#simulacaoGrid').show();", true);
                }
                else
                {
                    simulacaoGrid.Visible = false;
                    btnConfirmarAdiantamento.Visible = false;
                    Adiantamento_MensagemPagina.MostraMensagem_Erro("Valor Liberado digitado está divergente do Valor Liberado calculado, seguindo o cálculo (Valor Liberado = Soma dos Títulos - Total Juros - Total IOF - Tarifas). " +
                                                                    "<br>Verifique os valores digitados das Despesas e do Valor Liberado</br>", false);
                }

            }
            else
            {
                Adiantamento_MensagemPagina.MostraMensagem_Erro(sMensagemErro, false);
            }
        }

        private cls_Adiantamento CalcularAdiantamento()
        {            
            decimal nValorLiberado = 0m;
            decimal somaValorOriginal = 0m;
            decimal valorTotalComJuros = 0m;
            decimal nDespesasTotal = 0m;
            decimal nValorEmprestimo = 0m;
            decimal nJurosTotal = 0m;
            decimal nIOFTotal = 0m;
            decimal nIOFAdicionalTotal = 0m;
            decimal CET = 0m;
            DateTime dtEmissaoAdiantamento;
            DateTime ultimaDataVencimento = DateTime.MinValue;
            string idsConcatenados = "";
            string idEmpresa = "0";
            int nMaiorPeriodoDias = 0;
            decimal nDespesasTotalSoma = 0m;
            int nContadorAdiantamento = 0;

            try
            {
                if (!DateTime.TryParseExact(txtdtEmissaoAdiantamento.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtEmissaoAdiantamento))
                {
                    Adiantamento_MensagemPagina.MostraMensagem_Erro("Data de Emissão do Adiantamento Inválida.");
                    return null;
                }

                decimal nTaxaJuros = decimal.Parse(txtnTaxaJuros.Text.Replace("%", "")) / 100;
                decimal nIOF = decimal.Parse(txtnIOF.Text.Replace("%", "")) / 100;

                string nTarifasFormatado = txtnTarifas.Text.Replace(".", "").Replace(",", ".").Trim();
                decimal nTarifas = decimal.Parse(nTarifasFormatado, CultureInfo.InvariantCulture);

                decimal nIOFAdicional = decimal.Parse(txtnIOFAdicional.Text.Replace("%", "")) / 100;

                foreach (GridViewRow item in dtgvConsulta.Rows)
                {
                    CheckBox chkTitulo_Selecionado_Linha = item.FindControl("chkTitulo_Selecionado") as CheckBox;
                    if (chkTitulo_Selecionado_Linha != null && chkTitulo_Selecionado_Linha.Checked)
                    {
                        string idContasReceber = item.Cells[2].Text;
                        decimal valorOriginalLinha = decimal.Parse(item.Cells[12].Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("pt-BR"));

                        DateTime dataVencimentoLinha = DateTime.ParseExact(item.Cells[5].Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        idsConcatenados += string.IsNullOrEmpty(idsConcatenados) ? "" : ",";
                        idsConcatenados += idContasReceber;

                        if (dataVencimentoLinha > ultimaDataVencimento)
                        {
                            ultimaDataVencimento = dataVencimentoLinha;
                        }

                        int nDias = (dataVencimentoLinha - dtEmissaoAdiantamento).Days;
                        if (nDias > nMaiorPeriodoDias) nMaiorPeriodoDias = nDias;

                        decimal nIOFIndividual;
                        decimal nJurosIndividual;
                        if (ddlsTipoCalculo.SelectedValue == "P")
                        { 
                            nIOFIndividual = valorOriginalLinha * nIOF * nDias;
                            nJurosIndividual = (valorOriginalLinha * nTaxaJuros / 30) * nDias;                        
                            decimal nIOFAdicionalIndividual = valorOriginalLinha * nIOFAdicional;

                            decimal nDespesasIndividual = nIOFIndividual + nIOFAdicionalIndividual;

                            nIOFTotal += nIOFIndividual;
                            nIOFAdicionalTotal += nIOFAdicionalIndividual;
                            nJurosTotal += nJurosIndividual;
                            nDespesasTotalSoma += nDespesasIndividual;
                        }
                        else if (ddlsTipoCalculo.SelectedValue == "M")
                        {
                            nIOFTotal = Convert.ToDecimal(txtnTotalIOFValor.Text.Replace(".", ""));
                            nJurosTotal = Convert.ToDecimal(txtnTotalJurosValor.Text.Replace(".", ""));
                            nDespesasTotalSoma = Convert.ToDecimal(txtnTotalIOFValor.Text.Replace(".", ""));
                        }

                        somaValorOriginal += valorOriginalLinha;

                        nContadorAdiantamento += 1;
                    }
                }
                nDespesasTotal = nJurosTotal + nDespesasTotalSoma + nTarifas;
                nValorEmprestimo = somaValorOriginal + nDespesasTotal + nJurosTotal;

                nValorLiberado = somaValorOriginal - nDespesasTotal;
                if (ddlsTipoCalculo.SelectedValue == "M")
                {
                    if (nValorLiberado != Convert.ToDecimal(txtnValorLiberado.Text.Replace(".", "")))
                    {
                        return null;
                    }
                }                

                valorTotalComJuros = somaValorOriginal + nDespesasTotal;

                return new cls_Adiantamento(
                    sFuncao: "ADIANTAR",
                    idConta_Info_Pag: Convert.ToInt32(ddlidConta_Info_Pag.SelectedValue),
                    idCategoriaPagar: Convert.ToInt32(ddlidCategoriaPagar.SelectedValue),
                    sidsContasReceber: idsConcatenados,
                    nValorTotalComJuros: Math.Round(valorTotalComJuros, 2),
                    nSomaValorOriginal: Math.Round(somaValorOriginal, 2),
                    nTaxaJuros: Math.Round(nTaxaJuros, 3),
                    nIOF: Math.Round(nIOF, 7),
                    nTarifas: Math.Round(nTarifas, 2),
                    dtVencimento: dtEmissaoAdiantamento.ToString("yyyy-MM-dd"),
                    dtAtualizacao: dtEmissaoAdiantamento.ToString("yyyy-MM-dd"),
                    idUsuarioAtualizacao: Identity.Variaveis.idUsuario(),
                    nIOFAdicional: Math.Round(nIOFAdicional, 7),
                    nTaxaJurosNominal: Math.Round(nTaxaJuros, 3),
                    nDespesas: Math.Round(nDespesasTotal, 2),
                    nValorEmprestimo: Math.Round(nValorEmprestimo, 2),
                    nCET: Math.Round(CET, 2),
                    nDias: nMaiorPeriodoDias,
                    nIOFTotal: Math.Round(nIOFTotal, 2),
                    nIOFAdicionalTotal: Math.Round(nIOFAdicionalTotal, 2),
                    nJurosTotal: Math.Round(nJurosTotal, 2),
                    nSomaIOF: Math.Round(nIOFTotal + nIOFAdicionalTotal, 2),
                    dtEmissaoAdiantamento: dtEmissaoAdiantamento.ToString("yyyy-MM-dd"),
                    idEmpresa: 1,
                    idContasPagar_Adiantado: 0,
                    nValorLiberado: nValorLiberado,
                    nContadorAdiantamento: nContadorAdiantamento                    
                );
            }
            catch (Exception ex)
            {
                Adiantamento_MensagemPagina.MostraMensagem_Erro(ex.Message);
                return null;
            }
        }

        private cls_Adiantamento_Detalhe CalcularAdiantamento_Detalhe(int idContasReceber, decimal valorOriginal, DateTime dataVencimento, string sDscCliente, int nContadorAdiantamento, int qtdTitulosCalulados)
        {
            decimal nValorLiberado_Detalhe = 0m;
            decimal nTaxaJuros_Detalhe = decimal.Parse(txtnTaxaJuros.Text.Replace("%", "")) / 100;
            decimal nIOF_Detalhe = decimal.Parse(txtnIOF.Text.Replace("%", "")) / 100;
            decimal nIOFAdicional_Detalhe = decimal.Parse(txtnIOFAdicional.Text.Replace("%", "")) / 100;

            string nTarifasFormatado_Detalhe = txtnTarifas.Text.Replace(".", "").Replace(",", ".").Trim();
            decimal nTarifas_Detalhe = decimal.Parse(nTarifasFormatado_Detalhe, CultureInfo.InvariantCulture);
            
            decimal nDivisaoTarifa = nTarifas_Detalhe / nContadorAdiantamento;

            if (qtdTitulosCalulados == nContadorAdiantamento)
            {                
                nDivisaoTarifa = nTarifas_Detalhe - (Math.Round(nDivisaoTarifa, 2) * (nContadorAdiantamento - 1));
            }            

            DateTime.TryParseExact(txtdtEmissaoAdiantamento.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime EmissaoAdiantamento_Detalhe);

            //Calculo de dias
            int nDias_Detalhe = (dataVencimento - EmissaoAdiantamento_Detalhe).Days;

            //Cálculo de Juros e IOF
            decimal nValoriofTotal;
            decimal nJurosTotal_Detalhe;
            decimal nValorIofAdicionalTotal = 0m;
            decimal nSomaIOF_Detalhe;
            decimal nDespesas_Detalhe;
            ;

            if (ddlsTipoCalculo.SelectedValue == "P")
            {
                nValoriofTotal = valorOriginal * nIOF_Detalhe * nDias_Detalhe;
                nJurosTotal_Detalhe = (valorOriginal * nTaxaJuros_Detalhe / 30) * nDias_Detalhe;
                nValorIofAdicionalTotal = valorOriginal * nIOFAdicional_Detalhe;
                nSomaIOF_Detalhe = nValoriofTotal + nValorIofAdicionalTotal;
                nDespesas_Detalhe = nSomaIOF_Detalhe;
                nValorLiberado_Detalhe = valorOriginal - nDespesas_Detalhe - nJurosTotal_Detalhe - nDivisaoTarifa;
            }
            else
            {
                nValoriofTotal = Convert.ToDecimal(txtnTotalIOFValor.Text.Replace(".", ""));
                nJurosTotal_Detalhe = Convert.ToDecimal(txtnTotalJurosValor.Text.Replace(".", ""));
                nSomaIOF_Detalhe = Convert.ToDecimal(txtnTotalIOFValor.Text.Replace(".", ""));
                nDespesas_Detalhe = nSomaIOF_Detalhe;
                nValorLiberado_Detalhe = Convert.ToDecimal(txtnValorLiberado.Text.Replace(".", ""));               
            }

            decimal valorEmprestimo = valorOriginal + nDespesas_Detalhe + nJurosTotal_Detalhe;

            //Valor Total com Juros
            decimal valorTotalComJuros_Detalhe = valorOriginal + nDespesas_Detalhe + nJurosTotal_Detalhe;

            decimal nTotalOperacao = valorOriginal + nJurosTotal_Detalhe + nSomaIOF_Detalhe + nDivisaoTarifa;

            decimal nTotalDespesa = nJurosTotal_Detalhe + nSomaIOF_Detalhe + nDivisaoTarifa;    
            
            DateTime dtVencimento = dataVencimento;

            int idContasPagar = 0;

            return new cls_Adiantamento_Detalhe(
                idContasReceber_Detalhe: idContasReceber,
                idContasPagar_Detalhe: idContasPagar,
                nValorTotalComJuros_Detalhe: valorTotalComJuros_Detalhe,
                nSomaValorOriginal_Detalhe: valorOriginal,
                nTaxaJuros_Detalhe: nTaxaJuros_Detalhe,
                nIOF_Detalhe: nIOF_Detalhe,
                nTarifas_Detalhe: nTarifas_Detalhe,
                dtVencimento_Detalhe: EmissaoAdiantamento_Detalhe.ToString("dd/MM/yyyy"),
                nIOFAdicional_Detalhe: nIOFAdicional_Detalhe,
                nTaxaJurosNominal_Detalhe: 0,
                nDespesas_Detalhe: nDespesas_Detalhe,
                nValorEmprestimo_Detalhe: valorEmprestimo,
                nCET_Detalhe: 0,
                nDias_Detalhe: nDias_Detalhe,
                nIOFTotal_Detalhe: nValoriofTotal,
                nIOFAdicionalTotal_Detalhe: nValorIofAdicionalTotal,
                nJurosTotal_Detalhe: nJurosTotal_Detalhe,
                nSomaIOF_Detalhe: nSomaIOF_Detalhe,
                dtEmissaoAdiantamento_Detalhe: EmissaoAdiantamento_Detalhe.ToString("yyyy-MM-dd"),
                idEmpresa_Detalhe: 0,
                sDscCliente_Detalhe: sDscCliente,
                nValorLiberado_Detalhe: nValorLiberado_Detalhe,
                nValorTitulo: valorOriginal,
                nTotalOperacao: nTotalOperacao,
                nTotalDespesas: nTotalDespesa,
                nDivisaoTarifa: nDivisaoTarifa,
                sTipoCalculo: ddlsTipoCalculo.SelectedValue
            );

        }        

        protected void btnConfirmarAdiantamento_Click(object sender, EventArgs e)
        {
            var adiantamento = bs_Adiantamento.FirstOrDefault();

            if (adiantamento != null && Salvar_Adiantamento(adiantamento))
            {
                bool todosDetalhesSalvos = true;

                foreach (var detalhe in bs_Adiantamento_Detalhe)
                {
                    if (!Salvar_Adiantamento_Detalhe(detalhe, adiantamento.idContasPagar_Adiantado))
                    {
                        todosDetalhesSalvos = false;
                        break;
                    }
                }

                if (todosDetalhesSalvos)
                {
                    MensagemPagina.MostraMensagem_Sucesso("Adiantamento e seus detalhes salvos com sucesso.");
                    Session["SalvoComSucesso"] = true;
                    Response.Redirect(Request.RawUrl);
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao salvar os detalhes do adiantamento.");
                }
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao salvar adiantamento.");
            }



        }

        bool Salvar_Adiantamento(cls_Adiantamento adiantamento)
        {
            bool bRetorno = false;

            try
            {
                DataSet dsAdiantamento;
                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "ADIANTAR" },
                    { "@idConta_Info_Pag", adiantamento.idConta_Info_Pag.ToString() },
                    { "@idCategoriaPagar", adiantamento.idCategoriaPagar.ToString() },
                    { "@sidsContasReceber", adiantamento.sidsContasReceber },
                    { "@nValorTotalComJuros", adiantamento.nValorTotalComJuros.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nSomaValorOriginal", adiantamento.nSomaValorOriginal.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nTaxaJuros", adiantamento.nTaxaJuros.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nIOF", adiantamento.nIOF.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nTarifas", adiantamento.nTarifas.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@dtVencimento", adiantamento.dtVencimento },
                    { "@dtAtualizacao", adiantamento.dtAtualizacao },
                    { "@idUsuarioAtualizacao", adiantamento.idUsuarioAtualizacao },
                    { "@nIOFAdicional", adiantamento.nIOFAdicional.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nTaxaJurosNominal", adiantamento.nTaxaJurosNominal.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nDespesas", adiantamento.nDespesas.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nValorEmprestimo", adiantamento.nValorEmprestimo.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nCET", adiantamento.nCET.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nDias", adiantamento.nDias.ToString() },
                    { "@nIOFTotal",  adiantamento.nIOFTotal.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nIOFAdicionalTotal",  adiantamento.nIOFAdicionalTotal.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nJurosTotal", adiantamento.nJurosTotal.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nSomaIOF" ,adiantamento.nSomaIOF.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@dtEmissaoAdiantamento", adiantamento.dtEmissaoAdiantamento },
                    { "@idEmpresa", adiantamento.idEmpresa.ToString() },
                    { "@nValorLiberado", adiantamento.nValorLiberado.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },


                };

                dsAdiantamento = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Receber", vParametros);

                if (BD.ValidarDataSet(dsAdiantamento))
                {
                    adiantamento.idContasPagar_Adiantado = Convert.ToInt32(dsAdiantamento.Tables[0].Rows[0]["idContasPagar_Adiantado"]);
                }

                bRetorno = true;


            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao salvar adiantamento: " + ex.Message);
                bRetorno = false;
            }

            return bRetorno;
        }

        bool Salvar_Adiantamento_Detalhe(cls_Adiantamento_Detalhe adiantamento_Detalhe, int idContasPagar_Adiantado)
        {
            bool bRetorno = false;

            try
            {
                decimal nTaxaJuros = adiantamento_Detalhe.nTaxaJuros_Detalhe * 100;
                decimal nIOF = adiantamento_Detalhe.nIOF_Detalhe * 100;
                decimal nIOF_Adicional = adiantamento_Detalhe.nIOFAdicional_Detalhe * 100; 

                DataSet dsAdiantamento_Detalhe;
                Dictionary<String, String> vParametros_Detalhe = new Dictionary<string, string>
                {
                    { "@sFuncao", "ADIANTAR_DETALHE" },
                    { "@idContasPagar", idContasPagar_Adiantado.ToString() },
                    { "@idContasReceber_Detalhe", adiantamento_Detalhe.idContasReceber_Detalhe.ToString() },
                    { "@nValorTotalComJuros_Detalhe", adiantamento_Detalhe.nValorTotalComJuros_Detalhe.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nSomaValorOriginal_Detalhe", adiantamento_Detalhe.nSomaValorOriginal_Detalhe.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nTaxaJuros_Detalhe", nTaxaJuros.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nIOF_Detalhe", nIOF.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nTarifas_Detalhe", adiantamento_Detalhe.nTarifas_Detalhe.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@dtVencimento_Detalhe", adiantamento_Detalhe.dtVencimento_Detalhe },
                    { "@nIOFAdicional_Detalhe", nIOF_Adicional.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nTaxaJurosNominal_Detalhe", adiantamento_Detalhe.nTaxaJurosNominal_Detalhe.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nDespesas_Detalhe", adiantamento_Detalhe.nDespesas_Detalhe.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nValorEmprestimo_Detalhe", adiantamento_Detalhe.nValorEmprestimo_Detalhe.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nIOFTotal_Detalhe",  adiantamento_Detalhe.nIOFTotal_Detalhe.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nIOFAdicionalTotal_Detalhe",  adiantamento_Detalhe.nIOFAdicionalTotal_Detalhe.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nJurosTotal_Detalhe", adiantamento_Detalhe.nJurosTotal_Detalhe.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nSomaIOF_Detalhe" ,adiantamento_Detalhe.nSomaIOF_Detalhe.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@nDias_Detalhe", adiantamento_Detalhe.nDias_Detalhe.ToString() },
                    { "@nValorLiberado_Detalhe", adiantamento_Detalhe.nValorLiberado_Detalhe.ToString(CultureInfo.InvariantCulture).Replace(",", ".") },
                    { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() },
                    { "@sTipoCalculo", adiantamento_Detalhe.sTipoCalculo },
                };

                dsAdiantamento_Detalhe = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Receber", vParametros_Detalhe);

                bRetorno = true;


            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao salvar adiantamento: " + ex.Message);
                bRetorno = false;
            }

            return bRetorno;
        }

        private bool ValidarDadosModal()
        {
            bool bRetorno = true;
            string sMensagemErro = "";
            
            if (!Validacoes.ValidarData(txtdtEmissaoAdiantamento))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data de vencimento inválida!";
            }
            
            if (txtnTaxaJuros.Text.Length < 3)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Taxa de Juros Inválida! Mínimo 3 caracteres!";
            }
            if (txtnIOF.Text.Length < 3)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor do IOF Inválido! Mínimo 3 caracteres!";
            }
            if(txtnIOFAdicional.Text.Length < 3)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor do IOF Adicional Inválido! Mínimo 3 caracteres!";
            }

            if (txtnTarifas.Text.Length < 3)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor da Tarifa Inválido! Mínimo 3 caracteres!";
            }

            if (ddlsTipoCalculo.SelectedValue == "M")
            {
                if (txtnValorLiberado.Text ==  "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite o Valor Liberado";
                }
                
                if (txtnTotalIOFValor.Text ==  "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite o Valor Total do IOF";
                }
                
                if (txtnTotalJurosValor.Text ==  "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite o Valor Total do Juros";
                }

            }

            if (ddlidConta_Info_Pag.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Conta!";
            }

            if (ddlidCategoriaPagar.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Categoria!";
            }

            if (ddlsTipoCalculo.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Metódo de Cálculo!";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                Adiantamento_MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        void RegistraScriptModalAdiantamento()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$(document).ready(function () {");

            //Mascaras
            sb.AppendLine("    $('[id*=txtnTaxaJuros]').mask('09,999%', {reverse: true});");
            sb.AppendLine("    $('[id*=txtnIOF]').mask('09,9999999%', {reverse: true});");
            sb.AppendLine("    $('[id*=txtnTarifas]').mask('000.000,00', {reverse: true});");
            sb.AppendLine("    $('[id*=txtnIOFAdicional]').mask('09,9999999%', {reverse: true});");
            sb.AppendLine("    $('[id*=txtnValorLiberado]').mask('0.000.000.009,99', { reverse: true });");
            sb.AppendLine("    $('[id*=txtnTotalJurosValor]').mask('0.000.000.009,99', { reverse: true });");
            sb.AppendLine("    $('[id*=txtnTotalIOFValor]').mask('0.000.000.009,99', { reverse: true });");


            //Tamanho do modal
            sb.AppendLine("    $('#Modal_Adiantamento').on('shown.bs.modal', function () {");
            sb.AppendLine("        $('.modal-dialog').css({'max-width': '70%', 'width': '70%'});");
            sb.AppendLine("    });");

            //Limpa campos ao fechar o modal
            sb.AppendLine("    $('#Modal_Adiantamento').on('hidden.bs.modal', function () {");
            sb.AppendLine("        $(this).find('form').trigger('reset');");
            sb.AppendLine("    });");

            //Esconde botão de confirmar ao mudar os campos
            sb.AppendLine("    $(document).on('change', '.trigger-hide-button', function() {");
            sb.AppendLine($"        $('#{btnConfirmarAdiantamento.ClientID}').hide();");
            sb.AppendLine("    });");

            sb.AppendLine("});");

            //Limpa campo de busca antes de abrir modal
            sb.AppendLine("    function prepararEabrirModalAdiantamento() {");
            sb.AppendLine("        $('#cphCorpo_dtgvConsulta_filter input[type=\"search\"]').val('').trigger('keyup');");
            sb.AppendLine("        abrirModalAdiantamento();");
            sb.AppendLine("    }");

            //Abre o modal
            sb.AppendLine("function abrirModalAdiantamento() {");
            sb.AppendLine("    $('#Modal_Adiantamento').modal('show');");
            sb.AppendLine("}");

            //Dispara a simulação do adiantamento
            sb.AppendLine("function simularAdiantamento() {");
            sb.AppendLine("    $('#simulacaoGrid').show();");
            sb.AppendLine("    $('#btnConfirmarAdiantamento').show();");
            sb.AppendLine("}");

            //Limpa campos ao fechar o modal
            sb.AppendLine("$('#Modal_Adiantamento').on('hidden.bs.modal', function () {");
            sb.AppendLine($"    $('[id*={btnLimpaCampos.ClientID}]').click();");
            sb.AppendLine("});");


            ScriptManager.RegisterStartupScript(this, this.GetType(), "ModalScript", sb.ToString(), true);

        }

        protected void btnCancelarAdiantamento_Click(object sender, EventArgs e)
        {
            Pesquisar("");
        }

        protected void gvSimulacaoAdiantamento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

            }
        }

        protected void LimpaCampos(object sender, EventArgs e)
        {
            ddlidConta_Info_Pag.SelectedValue = "0";
            ddlidCategoriaPagar.SelectedValue = "0";
            txtnTaxaJuros.Text = "";
            txtnIOF.Text = "";
            txtnIOFAdicional.Text = "";
            txtnTarifas.Text = "";
            txtdtEmissaoAdiantamento.Text = "";

            btnConfirmarAdiantamento.Visible = false;

            tituloResumo.Visible = false;
            gvSimulacaoAdiantamento.DataSource = null;
            gvSimulacaoAdiantamento.DataBind();

            gvSimulacaoAdiantamento_Detalhe.DataSource = null;
            gvSimulacaoAdiantamento_Detalhe.DataBind();
        }

        protected void btnLimpaCampos_Click(object sender, EventArgs e)
        {
            LimpaCampos(sender, e);
        }

        protected void gvSimulacaoAdiantamento_Detalhe_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

            }
        }

        protected void cmdAdiantar_Click(object sender, EventArgs e)
        {
            Session["AdiantarTitulos"] = true;
            Response.Redirect(Request.RawUrl);

        }

        protected void cmdVoltarAdiantamento_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidConta_Info_Pag, "sp_Select 'tbl_Flow_Adm_ContasBancarias'", "idConta", "sDscConta", false, "Selecione uma Conta", "0");
            FUNCOES.Popula_Combo(ddlidCategoriaPagar, "sp_Select 'Flow_Adm_Contas_Pagar_Categoria'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Selecione a Categoria", "0");
        }

        #endregion
    }
}