using System;
using System.Collections.Generic;
using System.Data;
				 
using System.Web.UI;
using System.Web.UI.WebControls;
				  
									 
						   
							   
							  
						
using TT.FrameWork;
using TT_Flow.App.Controles;
using static TT.FrameWork.BD;
using static TT.FrameWork.Excel;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Grid;
using static TT.FrameWork.Identity;

namespace TT_Hub.App.Paginas.Adm.Financeiro
{
    public partial class ContasPagar : Page
    {
        #region | Propriedades

        string sTituloPagina = "Contas a Pagar";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_Contas_Pagar";
        string sPagina_NovoRegistro = "App/Paginas/Adm/Financeiro/ContasPagar_Detalhe.aspx";

        int col_ID = 0;
        int col_Credor = 1;
        int col_Data_Emissao = 2;
        int col_Data_Vencimento = 3;
        int col_Dias_Ate_Vencimento = 4;
        int col_Referencia = 5;
        int col_Parcela = 6;
        int col_NF_Referencia = 7;
        int col_Valor_Multa = 8;
        int col_Valor_Juros = 9;
        int col_Valor_Desconto = 10;
        int col_Valor_Bruto = 11;
        int col_Valor_Liquido = 12;
        int col_Valor_Pago = 13;
        int col_Saldo_em_Aberto = 14;
        int col_Categoria = 15;
        int col_Status = 16;
        int col_Empresa = 17;
        int col_Data_Liquidacao = 18;
        int col_Data_Conciliacao = 19;

        string sNomeArquivo_Excel { get; set; }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "ManualdoUsuarioContasPagar.pdf";
            string DashBoard = "",
                    AnoSelecionado = Request.GetValue("sAno"),
                    Empresa = Request.GetValue("sEmpresa"),
                    Data = Request.GetValue("sData"),
                    Semana = Request.GetValue("sSemana"),
                    Mes = Request.GetValue("sMes");
			


            ValidaPermissao(Permissao.Financeiro.ContasPagar.Consultar, true);
            if (!ValidaPermissao(Permissao.Financeiro.ContasPagar.Incluir))
			 
                cmdNovo.Visible = false;
			 

            if (!IsPostBack)
            {

                if (Request["DashBoard"] != null)
				 
                    DashBoard = Request["DashBoard"];
				 

                if (DashBoard != "")
                {
                    pnFiltro.Visible = false;
												

                    if (!string.IsNullOrWhiteSpace(Request.GetValue("sEmpresa")))
                    {
                        if (Request.GetValue("sEmpresa") == "0")
						 
                            Empresa = " - Todas as Empresas";
						 
                        else
                        {
                            Dictionary<string, string> vParam = new Dictionary<string, string>
                            {
                                { "@sTabela", "tbl_Flow_Empresa" },
                                { "@sCampoCodigo", "idEmpresa" },
                                { "@sCampoDescricao", "sDscEmpresa" },
                                { "@idPesquisa", Request.GetValue("sEmpresa") }
                            };
                            DataSet ds = ExecutarDataSet("sp_Select", vParam);
                            Empresa = $" - {ds.Tables[0].Rows[0]["sDscEmpresa"]}";
                        }

                    }
														
					   
															  
					   

                    string sPeriodo = "";
										
                    if (Request["dtInicio"] != null && Request["dtFinal"] != null)
					 
                        sPeriodo = Request["dtInicio"].ToString() == "" || Request["dtFinal"].ToString() == "" ? "" : " - Período " + DateTime.Parse(Request["dtInicio"]).ToString("dd/MM/yyyy") + " a " + DateTime.Parse(Request["dtFinal"]).ToString("dd/MM/yyyy");
					 

                    string sStatus = "";
                    if (Request["sStatus"] != null)
					 
                        sStatus = Request["sStatus"].ToString() == "" ? "Todos os Status" : Request["sStatus"].ToString();
					 

                    string tituloAdicional = "";
                    switch (DashBoard)
                    {
										 
                        case "VenceHoje": tituloAdicional = "Vencendo Hoje em " + AnoSelecionado + Empresa; break;
								  
										
                        case "VenceEm7": tituloAdicional = "Vencendo em até 7 " + AnoSelecionado + Empresa; break;
								  
										 
                        case "VenceEm30": tituloAdicional = "Vencendo em até 30 Dias em " + AnoSelecionado + Empresa; break;
								  
												  
                        case "VencerMaisdeTrinta": tituloAdicional = "Vencendo +30 Dias em " + AnoSelecionado + Empresa; break;
								  
										   
                        case "TotalAberto": tituloAdicional = "Total Geral em " + AnoSelecionado + Empresa; break;
								  
										
                        case "EmAtraso": tituloAdicional = "Em Atraso" + AnoSelecionado + Empresa; break;
								  
											   
                        case "Total Liquidado": tituloAdicional = "Liquidados em " + AnoSelecionado + Empresa; break;
								  
										  
                        case "PagosNoAno": tituloAdicional = "Pagos em " + AnoSelecionado + Empresa; break;
								  
											  
                        case "CaixaPagosHoje": tituloAdicional = "Pago em " + Data + Empresa; break;
								  
												
                        case "CaixaPagosSemana": tituloAdicional = "Pagos Semana " + Semana + Empresa; break;
								  
											 
                        case "CaixaPagosMes": tituloAdicional = "Pagos Ano " + Mes + Empresa; break;
								  
												   
                        case "EmprestimoPagosHoje": tituloAdicional = "Pago em " + Data + Empresa; break;
								  
													 
                        case "EmprestimoPagosSemana": tituloAdicional = "Pagos Semana " + Semana + Empresa; break;
								  
												  
                        case "EmprestimoPagosMes": tituloAdicional = "Pagos Ano " + Mes + Empresa; break;
								  
													
                        case "ComparativoPagosHoje": tituloAdicional = "Despesas em " + Data + Empresa; break;
								  
													  
                        case "ComparativoPagosSemana": tituloAdicional = "Despesas Semana " + Semana + Empresa; break;
								  
												   
                        case "ComparativoPagosMes": tituloAdicional = "Despesas Ano " + Mes + Empresa; break;
								  
												 
                        case "ContabilPagosHoje": tituloAdicional = "Despesas em " + Data + Empresa; break;
								  
												   
                        case "ContabilPagosSemana": tituloAdicional = "Despesas Semana " + Semana + Empresa; break;
								  
												
                        case "ContabilPagosMes": tituloAdicional = "Despesas Ano " + Mes + Empresa; break;
								  
									   
                        case "Compras": tituloAdicional = "Compras Em " + Mes + Empresa; break;
								  
                        case "Comparativo":
                            ddlidEmpresa.Popula_Combo("sp_Select 'tbl_Flow_Empresa'", "idEmpresa", "sDscEmpresa", false, "Todas as Empresas", "0");
                            ddlidCategoriaPagar.Popula_Combo("sp_Select 'Flow_Adm_Contas_Pagar_Categoria'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Todas as Categorias", "0");

                            //Antonio Lemos - 05/06/2024
                            string dtInicial = Request.GetValue("dtInicio");
                            string dtFinal = Request.GetValue("dtFinal");
                            string idEmpresa = Request.GetValue("idEmpresa");
																														 
																																  

                            txtdtInicio.Text = dtInicial;
                            txtdtFinal.Text = dtFinal;
                            ddlidEmpresa.SelectedValue = idEmpresa;
                            ddlidCategoriaPagar.SelectedValue = Request.GetValue("idCategoria");
                            ddlidDataPesquisa.SelectedValue = Request.GetValue("idDataPesquisa");
                            ddlsStatus.SelectedValue = "";

                            lblSubTituloPagina.Text = string.Format("Comparativo: {0} - {1} {2} á {3}", ddlidCategoriaPagar.SelectedItem.Text, ddlidDataPesquisa.SelectedItem.Text, dtInicial, dtFinal);

                            if (idEmpresa != "0")
							 
                                lblSubTituloPagina.Text += " - " + ddlidEmpresa.SelectedItem.Text;

                            BreadCrumb_Pagina.NivelPagina = 3;
                            BreadCrumb_Pagina.TitulodaPagina = lblSubTituloPagina.Text;
                            break;
												   
                        case "relatorioFinanceiro": tituloAdicional = sStatus + sPeriodo; break;
                        case "fluxoCaixa":
                            tituloAdicional = "Pool Bancário" + sPeriodo;
                            break;
                    }

                    if (tituloAdicional != "")
                    {
                        lblTituloPagina.Text = sTituloPagina + " - " + tituloAdicional;
                        BreadCrumb_Pagina.TitulodaPagina = sTituloPagina + " - " + tituloAdicional;
                    }
                    Pesquisar(DashBoard);
                }
                else
                {
                    ddlidFormaPagamento.Popula_Combo("sp_Select 'tbl_Flow_CondicaodePagamento_Tipo'", "idTipoCondicaoPagamento", "sDscTipoCondicaoPagamento", false, "Todos os Pagamentos", "0");
                    ddlidCentroDeCusto.Popula_Combo("sp_Select 'Flow_Adm_CentroDeCusto'", "idCentroDeCusto", "sDescricao", false, "Todos os Centros de Custo", "0");
																																						 
                    ddlidEmpresa.Popula_Combo("sp_Select 'Flow_Empresa', @idUsuario=" + Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Todas as Empresas", "0");
                    ddlidCategoriaPagar.Popula_Combo("sp_Select 'Flow_Adm_Contas_Pagar_Categoria'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Todas as Categorias", "0");
                    ddlidCredor.Popula_Combo("sp_Select 'Flow_Adm_Contas_Pagar_Clientes'", "idParceiro", "sRazaoSocial", false, "Todos os Credores", "0");
                    ddlidAprovado.Popula_Combo("sp_Select 'tbl_Flow_Adm_Aprovacao_Status'", "idAprovado", "sDscAprovacao", false, "Todos os status de Aprovação", "0");
                    ddlidConta_Info_Pag.Popula_Combo("sp_Select 'tbl_Flow_Adm_ContasBancarias'", "idConta", "sDscConta", false, "Todas as Contas", "0");

                    BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                    pnResultado.Visible = false;
                    ddlsStatus.SelectedValue = "Em Aberto";
									
                }
				
            }
		   
								
														  
																	 

            //txtPesquisa.Focus();
            Scripts.FocusScript(Page, txtPesquisa.ClientID);
																																   
        }

        protected (DataTable, List<int>) Pesquisar(string DashBoard = "", bool bExcel = false)
        {
            DataTable dt = null;
            List<int> list = null;

            pnResultado.Visible = false;

            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string> { { "@sFuncao", "CONSULTAR" } };
                if (string.IsNullOrWhiteSpace(DashBoard))
									
                {
                    vParametros.Add("@sUsuarioLogado", Session.GetValue("idUsuario"));
                    vParametros.Add("@sPermissao", ValidaPermissao(Permissao.Financeiro.ContasPagar.VisualizarTudo) ? "S" : "N");
                    vParametros.Add("@idFormaPagamento", ddlidFormaPagamento.SelectedValue);
                    vParametros.Add("@idCentroDeCusto", ddlidCentroDeCusto.SelectedValue);
                    vParametros.Add("@dtInicio", txtdtInicio.Text);
                    vParametros.Add("@dtFinal", txtdtFinal.Text);
                    vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                    vParametros.Add("@sPesquisa", txtPesquisa.Text);
                    vParametros.Add("@idParceiro", ddlidCredor.SelectedValue);
                    vParametros.Add("@idCategoriaPagar", ddlidCategoriaPagar.SelectedValue);
                    vParametros.Add("@idAprovado", ddlidAprovado.SelectedValue);
                    vParametros.Add("@sAdiantado", ddlsAdiantado.SelectedValue);
                    vParametros.Add("@idDataPesquisa", ddlidDataPesquisa.SelectedValue);
                    vParametros.Add("@idConta_Info_Pag", ddlidConta_Info_Pag.SelectedValue);
                    vParametros.Add("@sConciliado", ddlsConciliado.SelectedValue);

                    if (ddlidDataPesquisa.SelectedValue == "2" && txtdtInicio.Text != "")
                    {
                        vParametros.Add("@sStatus", "");
                        ddlsStatus.SelectedValue = "";
                    }
                    else
                        vParametros.Add("@sStatus", ddlsStatus.SelectedValue);

                    if (bExcel)
                    {
                        vParametros["@sFuncao"] = "CONSULTAR_EXCEL";
                        dt = ExecutarDataTable("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", vParametros);
                        vParametros["@sFuncao"] = "CONSULTAR";
                    }

                }
                else
                {
                    vParametros.Add("@sDashBoard", DashBoard);
                    vParametros.Add("@sUsuarioLogado", Session.GetValue("idUsuario"));
                    vParametros.Add("@sPermissaoPagar", ValidaPermissao(Permissao.Financeiro.DashboardAdm.VisualizarTudoPagar) ? "S" : "N");

                    if (!string.IsNullOrEmpty(Request.GetValue("sAno"))) vParametros.Add("@AnoSelecionado", Request.GetValue("sAno"));
                    if (!string.IsNullOrEmpty(Request.GetValue("sEmpresa"))) vParametros.Add("@idEmpresaSelecionada", Request.GetValue("sEmpresa"));
                    if (!string.IsNullOrEmpty(Request.GetValue("sData"))) vParametros.Add("@DataSelecionada", Request.GetValue("sData"));
                    if (!string.IsNullOrEmpty(Request.GetValue("sSemana"))) vParametros.Add("@SemanaSelecionada", Request.GetValue("sSemana"));
                    if (!string.IsNullOrEmpty(Request.GetValue("sMes"))) vParametros.Add("@MesSelecionada", Request.GetValue("sMes"));
                    if (!string.IsNullOrEmpty(Request.GetValue("sTipo"))) vParametros.Add("@sTipo", Request.GetValue("sTipo"));
                    if (!string.IsNullOrEmpty(Request.GetValue("dtInicio"))) vParametros.Add("@dtInicio", Request.GetValue("dtInicio"));
                    if (!string.IsNullOrEmpty(Request.GetValue("dtFinal"))) vParametros.Add("@dtFinal", Request.GetValue("dtFinal"));
                    if (!string.IsNullOrEmpty(Request.GetValue("sStatus"))) vParametros.Add("@sStatus", Request.GetValue("sStatus"));
                    if (!string.IsNullOrEmpty(Request.GetValue("idParceiro"))) vParametros.Add("@idParceiro", Request.GetValue("idParceiro"));
                    if (!string.IsNullOrEmpty(Request.GetValue("sAgrupar"))) vParametros.Add("@sAgrupar", Request.GetValue("sAgrupar"));
                    if (!string.IsNullOrEmpty(Request.GetValue("sFiltroFinanceiro"))) vParametros.Add("@sFiltro", Request.GetValue("sFiltroFinanceiro"));
                    if (!string.IsNullOrEmpty(Request.GetValue("sEmpresaRelatorio"))) vParametros.Add("@sEmpresaRelatorio", Request.GetValue("sEmpresaRelatorio"));
                    if (!string.IsNullOrEmpty(Request["idConta"])) vParametros.Add("@idConta", Request["idConta"].ToString());
                    if (!string.IsNullOrEmpty(Request["idCategoria"])) vParametros.Add("@idCategoriaPagar",  Request["idCategoria"].ToString());
					if (!string.IsNullOrEmpty(Request.GetValue("idCategoriaTipo"))) vParametros.Add("@idCategoriaPagarTipo", Request.GetValue("idCategoriaTipo"));
	 				if (!string.IsNullOrEmpty(Request["idDataPesquisa"])) vParametros.Add("@idDataPesquisa", Request["idDataPesquisa"].ToString());
                    if (!string.IsNullOrEmpty(Request["idEmpresa"])) vParametros.Add("@idEmpresa", Request["idEmpresa"].ToString());
                    if (!string.IsNullOrEmpty(Request["sidConta"])){ vParametros.Add("@sidConta", Request["sidConta"].ToString()); }
					if (!string.IsNullOrEmpty(Request.GetValue("idCategoriaPai"))) vParametros.Add("@idCategoriaPagar", Request.GetValue("idCategoriaPai"));
					if (!string.IsNullOrEmpty(Request["sidEmpresa"])){ vParametros.Add("@sidEmpresa", Request["sidEmpresa"].ToString()); }
					if (!string.IsNullOrEmpty(Request.GetValue("adiantamento"))) 
                        vParametros.Add("@sAdiantado", Request.GetValue("adiantamento"));
                    else if (DashBoard == "fluxoCaixa" && !string.IsNullOrEmpty(Request.GetValue("idCategoriaPai")))
                        vParametros.Add("@sAdiantado", "N");
                }

                DataTable tb = ExecutarDataTable(sProcedure, vParametros);

                if (tb.Rows.Count > 0)
                {
                    pnResultado.Visible = true;

                    sNomeArquivo_Excel = $"Consulta_Contas_Pagar-{CarimboDataHora()}";

                    List<int> colunasOcultasInicial = new List<int>();
                    if (ddlsStatus.SelectedValue == "Liquidado")
                    {
                        colunasOcultasInicial.Add(col_Dias_Ate_Vencimento);
                        colunasOcultasInicial.Add(col_Valor_Bruto);
                        colunasOcultasInicial.Add(col_Status);

                        sNomeArquivo_Excel += "-Status_Liquidado";
					 
																					 
                    }
                    else if (ddlsStatus.SelectedValue == "")
																  
                    {
                        colunasOcultasInicial.Add(col_Valor_Multa);
                        colunasOcultasInicial.Add(col_Valor_Juros);
                        colunasOcultasInicial.Add(col_Valor_Desconto);
                        colunasOcultasInicial.Add(col_Valor_Bruto);
                        colunasOcultasInicial.Add(col_Data_Liquidacao);

                        sNomeArquivo_Excel += "-Status_Todos";
																				   
                    }
                    else
																	 
                    {
                        colunasOcultasInicial.Add(col_Valor_Multa);
                        colunasOcultasInicial.Add(col_Valor_Juros);
                        colunasOcultasInicial.Add(col_Valor_Desconto);
                        colunasOcultasInicial.Add(col_Valor_Bruto);
                        colunasOcultasInicial.Add(col_Valor_Liquido);
                        colunasOcultasInicial.Add(col_Valor_Pago);
                        colunasOcultasInicial.Add(col_Data_Liquidacao);
                        colunasOcultasInicial.Add(col_Data_Conciliacao);

                        sNomeArquivo_Excel += "-Status_EmAberto";
					 
																					 
                    }

                    if (Request.GetValue("DashBoard") == "PagosNoAno")
                        colunasOcultasInicial.Add(col_NF_Referencia);
                    else if (Request.GetValue("DashBoard") == "Comparativo")
                    {
                        colunasOcultasInicial.Add(col_Valor_Pago);
                        colunasOcultasInicial.Add(col_Data_Conciliacao);
                        colunasOcultasInicial.Add(col_Parcela);
                    }
                    else
                        colunasOcultasInicial.Add(col_Parcela);

                    if (bExcel)
                    {
                        list = new List<int>(colunasOcultasInicial);
                        sNomeArquivo_Excel += ".xlsx";
                    }

                    var colunasData = new int[4] { col_Data_Emissao, col_Data_Vencimento, col_Data_Liquidacao, col_Data_Conciliacao };
                    ScriptManager.RegisterStartupScript(Page, GetType(), "DataTables", DataBindComScriptData(dtgvConsulta, tb, col_Data_Emissao, colunasData, "desc", ltOcultarColunas, new List<int> { -1 }, colunasOcultasInicial), true);
                    SomarColunas(dtgvConsulta, false, Formatação.Moeda, col_Valor_Multa, col_Valor_Juros, col_Valor_Desconto, col_Valor_Bruto, col_Valor_Liquido, col_Valor_Pago, col_Saldo_em_Aberto);
					 
                }
				 
                else MensagemPagina.MostraMensagem_Erro("Nenhum Pagamento encontrado!");
				 
            }
            catch (Exception ex)
            {

                MensagemPagina.MostraMensagem_Erro("Erro ao consultar: " + ex.Message);
            }

            hddsFiltro.Value = "N";
		 

            return (dt, list);
		 
     }

        protected bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (txtdtInicio.Text != "" || txtdtFinal.Text != "")
			 
                sMensagemErro += Validacoes.ValidaDatas(txtdtInicio.Text, txtdtFinal.Text);
			 

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            if (ValidarDados())
            {
                hddsFiltro.Value = "S";
                Pesquisar("");
            }
        }

        protected void cmdNovo_Click(object sender, EventArgs e) => DirecionaPagina(sPagina_NovoRegistro);
		 

        protected void cmdExportar_Click(object sender, EventArgs e)
        {
            (DataTable tb, List<int> list) = Pesquisar("", true);
            ExportarExcel(Page, Server.MapPath("~/Download/") + sNomeArquivo_Excel, sNomeArquivo_Excel, sTituloPagina, tb, list);
        }

    }
}
