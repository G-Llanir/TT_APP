using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.FrameWork;
using static TT.FrameWork.BD;
using Identity = TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.Comercial
{

    //http://localhost:6997/App/Paginas/Comercial/DashboardCRM.aspx

    public partial class DashboardCRM : System.Web.UI.Page
    {
        string sTituloPagina = "DashBoard CRM";
        string sProcedure = "sp_Flow_DashBoard_CRM";
        int tab_graficoTaxaFechamento = 0;
        int tab_graficoPerdidos = 1;
        int tab_graficoEstadoOportunidade = 2;
        int tab_graficoCotacao = 3;
        int tab_graficoTempoMedioFechamento = 4;
        int tab_graficoRankingCliente = 5;
        int tab_graficoVendaFamilia = 6;
        int tab_graficoQtdOrcamento = 7;
        int tab_graficoVendaEstado = 8;
        int tab_graficoMetaVendedor = 9;

        List<int> lsGraficos = new List<int>();


        #region | Construtor
        public List<cls_Comercial_Dashboard_CRM> bs_GraficoTaxaFechamento
        {
            get
            {
                if (ViewState["bs_GraficoTaxaFechamento"] == null)
                {
                    ViewState["bs_GraficoTaxaFechamento"] = new List<cls_Comercial_Dashboard_CRM>();
                }
                return (List<cls_Comercial_Dashboard_CRM>)ViewState["bs_GraficoTaxaFechamento"];
            }
            set
            {
                ViewState["bs_GraficoTaxaFechamento"] = value;
            }
        }

        public List<cls_Comercial_Dashboard_CRM> bs_GraficoPerdidos
        {
            get
            {
                if (ViewState["bs_GraficoPerdidos"] == null)
                {
                    ViewState["bs_GraficoPerdidos"] = new List<cls_Comercial_Dashboard_CRM>();
                }
                return (List<cls_Comercial_Dashboard_CRM>)ViewState["bs_GraficoPerdidos"];
            }
            set
            {
                ViewState["bs_GraficoPerdidos"] = value;
            }
        }

        public List<cls_Comercial_Dashboard_CRM> bs_GraficoEstadoOportunidade
        {
            get
            {
                if (ViewState["bs_GraficoEstadoOportunidade"] == null)
                {
                    ViewState["bs_GraficoEstadoOportunidade"] = new List<cls_Comercial_Dashboard_CRM>();
                }
                return (List<cls_Comercial_Dashboard_CRM>)ViewState["bs_GraficoEstadoOportunidade"];
            }
            set
            {
                ViewState["bs_GraficoEstadoOportunidade"] = value;
            }
        }

        public List<cls_Comercial_Dashboard_CRM> bs_GraficoCotacao
        {
            get
            {
                if (ViewState["bs_GraficoCotacao"] == null)
                {
                    ViewState["bs_GraficoCotacao"] = new List<cls_Comercial_Dashboard_CRM>();
                }
                return (List<cls_Comercial_Dashboard_CRM>)ViewState["bs_GraficoCotacao"];
            }
            set
            {
                ViewState["bs_GraficoCotacao"] = value;
            }
        }

        public List<cls_Comercial_Dashboard_CRM> bs_GraficoTempoMedioFechamento
        {
            get
            {
                if (ViewState["bs_GraficoTempoMedioFechamento"] == null)
                {
                    ViewState["bs_GraficoTempoMedioFechamento"] = new List<cls_Comercial_Dashboard_CRM>();
                }
                return (List<cls_Comercial_Dashboard_CRM>)ViewState["bs_GraficoTempoMedioFechamento"];
            }
            set
            {
                ViewState["bs_GraficoTempoMedioFechamento"] = value;
            }
        }

        public List<cls_Comercial_Dashboard_CRM> bs_GraficoRankingCliente
        {
            get
            {
                if (ViewState["bs_GraficoRankingCliente"] == null)
                {
                    ViewState["bs_GraficoRankingCliente"] = new List<cls_Comercial_Dashboard_CRM>();
                }
                return (List<cls_Comercial_Dashboard_CRM>)ViewState["bs_GraficoRankingCliente"];
            }
            set
            {
                ViewState["bs_GraficoRankingCliente"] = value;
            }
        }

        public List<cls_Comercial_Dashboard_CRM> bs_GraficoVendaFamilia
        {
            get
            {
                if (ViewState["bs_GraficoVendaFamilia"] == null)
                {
                    ViewState["bs_GraficoVendaFamilia"] = new List<cls_Comercial_Dashboard_CRM>();
                }
                return (List<cls_Comercial_Dashboard_CRM>)ViewState["bs_GraficoVendaFamilia"];
            }
            set
            {
                ViewState["bs_GraficoVendaFamilia"] = value;
            }
        }

        public List<cls_Comercial_Dashboard_CRM> bs_GraficoQtdOrcamento
        {
            get
            {
                if (ViewState["bs_GraficoQtdOrcamento"] == null)
                {
                    ViewState["bs_GraficoQtdOrcamento"] = new List<cls_Comercial_Dashboard_CRM>();
                }
                return (List<cls_Comercial_Dashboard_CRM>)ViewState["bs_GraficoQtdOrcamento"];
            }
            set
            {
                ViewState["bs_GraficoQtdOrcamento"] = value;
            }
        }

        public List<cls_Comercial_Dashboard_CRM> bs_GraficoVendaEstado
        {
            get
            {
                if (ViewState["bs_GraficoVendaEstado"] == null)
                {
                    ViewState["bs_GraficoVendaEstado"] = new List<cls_Comercial_Dashboard_CRM>();
                }
                return (List<cls_Comercial_Dashboard_CRM>)ViewState["bs_GraficoVendaEstado"];
            }
            set
            {
                ViewState["bs_GraficoVendaEstado"] = value;
            }
        }

        public List<cls_Comercial_Dashboard_CRM> bs_GraficoMetaVendedor
        {
            get
            {
                if (ViewState["bs_GraficoMetaVendedor"] == null)
                {
                    ViewState["bs_GraficoMetaVendedor"] = new List<cls_Comercial_Dashboard_CRM>();
                }
                return (List<cls_Comercial_Dashboard_CRM>)ViewState["bs_GraficoMetaVendedor"];
            }
            set
            {
                ViewState["bs_GraficoMetaVendedor"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Funcoes.ValidaPermissao(Permissao.Comercial.DashboardCRM.Consultar, true);

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnGraficos.Visible = true;

                //Manual Usuario
                manual.sNomeArquivo = "Manual_DashboardCRM.pdf";

                Funcoes.Popula_Combo(ddlidVendedor, "sp_Select 'FLOW_Vendedores_Ativos'", "idVendedor", "sDscUsuario", false, "Selecione um Vendedor", "0");

                VisibleGraficos();

                if (Permissoes())
                {
                    ConsultarDados();
                    Preferencias();
                }                

            }

        }

        private bool Permissoes()
        {
            bool retorno = false;
            if (Funcoes.ValidaPermissao(Permissao.Comercial.DashboardCRM.Visualizar_Vendedor))
            {
                try
                {
                    ddlidVendedor.SelectedValue = ConsultaVendedor();
                    ddlidVendedor.Attributes.Add("disabled", "disabled");
                    hddidVendedor.Value = ddlidVendedor.SelectedValue;

                    if (ddlidVendedor.SelectedValue == "0")
                        MensagemPagina.MostraMensagem_Erro("Seu Usuário não é um vendedor, necessário atualizar as informações ou permissões no perfil");
                    else                
                        retorno = true;

                }
                catch
                {
                    MensagemPagina.MostraMensagem_Erro("Seu Usuário não é um vendedor, necessário atualizar as informações ou permissões no perfil");
                }

            }
            else if (Funcoes.ValidaPermissao(Permissao.Comercial.DashboardCRM.Visualizar_Supervisor))
            {
                hddidUsuario.Value = Identity.Variaveis.idUsuario();
                hddidVendedor.Value = ddlidVendedor.SelectedValue;
                retorno = true;
            }
            else if (Funcoes.ValidaPermissao(Permissao.Comercial.DashboardCRM.Visualizar_Diretor))
            {
                hddidUsuario.Value = "0";
                hddidVendedor.Value = "0";
                retorno = true;
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Permissão inválida, necessário atualizar as permissões no perfil");               
            }
            return retorno;
        }

        private void ConsultarDados()
        {
            DataSet dsDashBoard;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "POPULA_GRAFICO");
            vParametros.Add("@idVendedor", hddidVendedor.Value);
            vParametros.Add("@idUsuario", hddidUsuario.Value);
            dsDashBoard = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsDashBoard))
            {
                if (dsDashBoard.Tables[tab_graficoTaxaFechamento].Rows.Count > 0)
                {
                    HashSet<string> sAnosSet = new HashSet<string>();
                    HashSet<string> sTrimestreSet = new HashSet<string>();
                    HashSet<string> sMesSet = new HashSet<string>();

                    foreach (DataRow row in dsDashBoard.Tables[tab_graficoTaxaFechamento].Rows)
                    {
                        cls_Comercial_Dashboard_CRM item = new cls_Comercial_Dashboard_CRM();
                        item.sIndicador = row["sDscStatus"].ToString();
                        item.sPeriodo = row["sPeriodo"].ToString();
                        item.nQuantidade = Convert.ToInt32(row["quantidade"]);
                        item.sFiltro = Convert.ToInt32(row["sFiltro"]);
                        item.idVendedor = Convert.ToInt32(row["idVendedor"]);

                        switch (Convert.ToInt32(row["sFiltro"]))
                        {
                            case 0:
                                sMesSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 1:
                                sTrimestreSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 2:
                                sAnosSet.Add(row["sPeriodo"].ToString());
                                break;
                            default:
                                break;
                        }

                        bs_GraficoTaxaFechamento.Add(item);

                    }

                    foreach (var row in bs_GraficoTaxaFechamento)
                    {
                        switch (row.sFiltro)
                        {
                            case 0:
                                row.sLabels = sMesSet.ToArray();
                                row.sNomeGrafico = "Taxa de Fechamento - Mensal";
                                break;
                            case 1:
                                row.sLabels = sTrimestreSet.ToArray();
                                row.sNomeGrafico = "Taxa de Fechamento - Trimestral";
                                break;
                            case 2:
                                row.sLabels = sAnosSet.ToArray();
                                row.sNomeGrafico = "Taxa de Fechamento - Anual";
                                break;
                            default:
                                break;
                        }
                    }

                }

                if (dsDashBoard.Tables[tab_graficoPerdidos].Rows.Count > 0)
                {
                    HashSet<string> sAnosSet = new HashSet<string>();
                    HashSet<string> sTrimestreSet = new HashSet<string>();
                    HashSet<string> sMesSet = new HashSet<string>();

                    foreach (DataRow row in dsDashBoard.Tables[tab_graficoPerdidos].Rows)
                    {
                        cls_Comercial_Dashboard_CRM item = new cls_Comercial_Dashboard_CRM();
                        item.sIndicador = row["sDscMotivo"].ToString();
                        item.sPeriodo = row["sPeriodo"].ToString();
                        item.nQuantidade = Convert.ToInt32(row["quantidade"]);
                        item.sFiltro = Convert.ToInt32(row["sFiltro"]);
                        item.idVendedor = Convert.ToInt32(row["idVendedor"]);

                        switch (Convert.ToInt32(row["sFiltro"]))
                        {
                            case 0:
                                sMesSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 1:
                                sTrimestreSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 2:
                                sAnosSet.Add(row["sPeriodo"].ToString());
                                break;
                            default:
                                break;
                        }

                        bs_GraficoPerdidos.Add(item);

                    }

                    foreach (var row in bs_GraficoPerdidos)
                    {
                        switch (row.sFiltro)
                        {
                            case 0:
                                row.sLabels = sMesSet.ToArray();
                                row.sNomeGrafico = "Taxa de Perdidos por Motivos - Mensal";
                                break;
                            case 1:
                                row.sLabels = sTrimestreSet.ToArray();
                                row.sNomeGrafico = "Taxa de Perdidos por Motivos - Trimestral";
                                break;
                            case 2:
                                row.sLabels = sAnosSet.ToArray();
                                row.sNomeGrafico = "Taxa de Perdidos por Motivos - Anual";
                                break;
                            default:
                                break;
                        }
                    }

                }

                if (dsDashBoard.Tables[tab_graficoEstadoOportunidade].Rows.Count > 0)
                {
                    HashSet<string> sAnosSet = new HashSet<string>();
                    HashSet<string> sTrimestreSet = new HashSet<string>();
                    HashSet<string> sMesSet = new HashSet<string>();

                    foreach (DataRow row in dsDashBoard.Tables[tab_graficoEstadoOportunidade].Rows)
                    {
                        cls_Comercial_Dashboard_CRM item = new cls_Comercial_Dashboard_CRM();
                        item.sIndicador = row["Chance"].ToString();
                        item.sPeriodo = row["sPeriodo"].ToString();
                        item.nQuantidade = Convert.ToInt32(row["quantidade"]);
                        item.sFiltro = Convert.ToInt32(row["sFiltro"]);
                        item.idVendedor = Convert.ToInt32(row["idVendedor"]);

                        switch (Convert.ToInt32(row["sFiltro"]))
                        {
                            case 0:
                                sMesSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 1:
                                sTrimestreSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 2:
                                sAnosSet.Add(row["sPeriodo"].ToString());
                                break;
                            default:
                                break;
                        }

                        bs_GraficoEstadoOportunidade.Add(item);

                    }

                    foreach (var row in bs_GraficoEstadoOportunidade)
                    {
                        switch (row.sFiltro)
                        {
                            case 0:
                                row.sLabels = sMesSet.ToArray();
                                row.sNomeGrafico = "Chance de Fechamento - Mensal";
                                break;
                            case 1:
                                row.sLabels = sTrimestreSet.ToArray();
                                row.sNomeGrafico = "Chance de Fechamento - Trimestral";
                                break;
                            case 2:
                                row.sLabels = sAnosSet.ToArray();
                                row.sNomeGrafico = "Chance de Fechamento - Anual";
                                break;
                            default:
                                break;
                        }
                    }

                }

                if (dsDashBoard.Tables[tab_graficoCotacao].Rows.Count > 0)
                {
                    HashSet<string> sAnosSet = new HashSet<string>();
                    HashSet<string> sTrimestreSet = new HashSet<string>();
                    HashSet<string> sMesSet = new HashSet<string>();

                    foreach (DataRow row in dsDashBoard.Tables[tab_graficoCotacao].Rows)
                    {
                        cls_Comercial_Dashboard_CRM item = new cls_Comercial_Dashboard_CRM();
                        item.sIndicador = row["sDscStatus"].ToString();
                        item.sPeriodo = row["sPeriodo"].ToString();
                        item.nQuantidade = Convert.ToInt32(row["quantidade"]);
                        item.sFiltro = Convert.ToInt32(row["sFiltro"]);
                        item.idVendedor = Convert.ToInt32(row["idVendedor"]);

                        switch (Convert.ToInt32(row["sFiltro"]))
                        {
                            case 0:
                                sMesSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 1:
                                sTrimestreSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 2:
                                sAnosSet.Add(row["sPeriodo"].ToString());
                                break;
                            default:
                                break;
                        }

                        bs_GraficoCotacao.Add(item);

                    }

                    foreach (var row in bs_GraficoCotacao)
                    {
                        switch (row.sFiltro)
                        {
                            case 0:
                                row.sLabels = sMesSet.ToArray();
                                row.sNomeGrafico = "Estado de Cotações - Mensal";
                                break;
                            case 1:
                                row.sLabels = sTrimestreSet.ToArray();
                                row.sNomeGrafico = "Estado de Cotações - Trimestral";
                                break;
                            case 2:
                                row.sLabels = sAnosSet.ToArray();
                                row.sNomeGrafico = "Estado de Cotações - Anual";
                                break;
                            default:
                                break;
                        }
                    }

                }

                if (dsDashBoard.Tables[tab_graficoTempoMedioFechamento].Rows.Count > 0)
                {
                    HashSet<string> sAnosSet = new HashSet<string>();
                    HashSet<string> sTrimestreSet = new HashSet<string>();
                    HashSet<string> sMesSet = new HashSet<string>();

                    foreach (DataRow row in dsDashBoard.Tables[tab_graficoTempoMedioFechamento].Rows)
                    {
                        cls_Comercial_Dashboard_CRM item = new cls_Comercial_Dashboard_CRM();
                        item.sIndicador = row["TempoMedioDias"].ToString();
                        item.sPeriodo = row["sPeriodo"].ToString();
                        item.nQuantidade = Convert.ToInt32(row["quantidade"]);
                        item.sFiltro = Convert.ToInt32(row["sFiltro"]);
                        item.idVendedor = Convert.ToInt32(row["idVendedor"]);

                        switch (Convert.ToInt32(row["sFiltro"]))
                        {
                            case 0:
                                sMesSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 1:
                                sTrimestreSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 2:
                                sAnosSet.Add(row["sPeriodo"].ToString());
                                break;
                            default:
                                break;
                        }

                        bs_GraficoTempoMedioFechamento.Add(item);

                    }

                    foreach (var row in bs_GraficoTempoMedioFechamento)
                    {
                        switch (row.sFiltro)
                        {
                            case 0:
                                row.sLabels = sMesSet.ToArray();
                                row.sNomeGrafico = "Tempo Médio de Fechamento - Mensal";
                                break;
                            case 1:
                                row.sLabels = sTrimestreSet.ToArray();
                                row.sNomeGrafico = "Tempo Médio de Fechamento - Trimestral";
                                break;
                            case 2:
                                row.sLabels = sAnosSet.ToArray();
                                row.sNomeGrafico = "Tempo Médio de Fechamento - Anual";
                                break;
                            default:
                                break;
                        }
                    }

                }

                if (dsDashBoard.Tables[tab_graficoRankingCliente].Rows.Count > 0)
                {
                    HashSet<string> sAnosSet = new HashSet<string>();
                    HashSet<string> sTrimestreSet = new HashSet<string>();
                    HashSet<string> sMesSet = new HashSet<string>();

                    foreach (DataRow row in dsDashBoard.Tables[tab_graficoRankingCliente].Rows)
                    {
                        cls_Comercial_Dashboard_CRM item = new cls_Comercial_Dashboard_CRM();
                        item.sIndicador = row["sRazaoSocial"].ToString();
                        item.sPeriodo = row["sPeriodo"].ToString();
                        item.nQuantidade = Convert.ToInt32(row["quantidade"]);
                        item.sFiltro = Convert.ToInt32(row["sFiltro"]);
                        item.idVendedor = Convert.ToInt32(row["idVendedor"]);

                        switch (Convert.ToInt32(row["sFiltro"]))
                        {
                            case 0:
                                sMesSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 1:
                                sTrimestreSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 2:
                                sAnosSet.Add(row["sPeriodo"].ToString());
                                break;
                            default:
                                break;
                        }

                        bs_GraficoRankingCliente.Add(item);

                    }

                    foreach (var row in bs_GraficoRankingCliente)
                    {
                        switch (row.sFiltro)
                        {
                            case 0:
                                row.sLabels = sMesSet.ToArray();
                                row.sNomeGrafico = "Ranking Clientes - Mensal";
                                break;
                            case 1:
                                row.sLabels = sTrimestreSet.ToArray();
                                row.sNomeGrafico = "Ranking Clientes - Trimestral";
                                break;
                            case 2:
                                row.sLabels = sAnosSet.ToArray();
                                row.sNomeGrafico = "Ranking Clientes - Anual";
                                break;
                            default:
                                break;
                        }
                    }

                }

                if (dsDashBoard.Tables[tab_graficoVendaFamilia].Rows.Count > 0)
                {
                    HashSet<string> sAnosSet = new HashSet<string>();
                    HashSet<string> sTrimestreSet = new HashSet<string>();
                    HashSet<string> sMesSet = new HashSet<string>();

                    foreach (DataRow row in dsDashBoard.Tables[tab_graficoVendaFamilia].Rows)
                    {
                        cls_Comercial_Dashboard_CRM item = new cls_Comercial_Dashboard_CRM();
                        item.sIndicador = row["sDscFamilia"].ToString();
                        item.sPeriodo = row["sPeriodo"].ToString();
                        item.nQuantidade = Convert.ToInt32(row["quantidade"]);
                        item.sFiltro = Convert.ToInt32(row["sFiltro"]);
                        item.idVendedor = Convert.ToInt32(row["idVendedor"]);

                        switch (Convert.ToInt32(row["sFiltro"]))
                        {
                            case 0:
                                sMesSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 1:
                                sTrimestreSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 2:
                                sAnosSet.Add(row["sPeriodo"].ToString());
                                break;
                            default:
                                break;
                        }

                        bs_GraficoVendaFamilia.Add(item);

                    }

                    foreach (var row in bs_GraficoVendaFamilia)
                    {
                        switch (row.sFiltro)
                        {
                            case 0:
                                row.sLabels = sMesSet.ToArray();
                                row.sNomeGrafico = "Vendas por Família - Mensal";
                                break;
                            case 1:
                                row.sLabels = sTrimestreSet.ToArray();
                                row.sNomeGrafico = "Vendas por Família - Trimestral";
                                break;
                            case 2:
                                row.sLabels = sAnosSet.ToArray();
                                row.sNomeGrafico = "Vendas por Família - Anual";
                                break;
                            default:
                                break;
                        }
                    }

                }

                if (dsDashBoard.Tables[tab_graficoQtdOrcamento].Rows.Count > 0)
                {
                    HashSet<string> sAnosSet = new HashSet<string>();
                    HashSet<string> sTrimestreSet = new HashSet<string>();
                    HashSet<string> sMesSet = new HashSet<string>();

                    foreach (DataRow row in dsDashBoard.Tables[tab_graficoQtdOrcamento].Rows)
                    {
                        cls_Comercial_Dashboard_CRM item = new cls_Comercial_Dashboard_CRM();
                        item.sIndicador = row["sDscIndicador"].ToString();
                        item.sPeriodo = row["sPeriodo"].ToString();
                        item.nQuantidade = Convert.ToInt32(row["quantidade"]);
                        item.sFiltro = Convert.ToInt32(row["sFiltro"]);
                        item.idVendedor = Convert.ToInt32(row["idVendedor"]);

                        switch (Convert.ToInt32(row["sFiltro"]))
                        {
                            case 0:
                                sMesSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 1:
                                sTrimestreSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 2:
                                sAnosSet.Add(row["sPeriodo"].ToString());
                                break;
                            default:
                                break;
                        }

                        bs_GraficoQtdOrcamento.Add(item);

                    }

                    foreach (var row in bs_GraficoQtdOrcamento)
                    {
                        switch (row.sFiltro)
                        {
                            case 0:
                                row.sLabels = sMesSet.ToArray();
                                row.sNomeGrafico = "Quantidade de Orçamentos - Mensal";
                                break;
                            case 1:
                                row.sLabels = sTrimestreSet.ToArray();
                                row.sNomeGrafico = "Quantidade de Orçamentos - Trimestral";
                                break;
                            case 2:
                                row.sLabels = sAnosSet.ToArray();
                                row.sNomeGrafico = "Quantidade de Orçamentos - Anual";
                                break;
                            default:
                                break;
                        }
                    }

                }

                if (dsDashBoard.Tables[tab_graficoVendaEstado].Rows.Count > 0)
                {
                    HashSet<string> sAnosSet = new HashSet<string>();
                    HashSet<string> sTrimestreSet = new HashSet<string>();
                    HashSet<string> sMesSet = new HashSet<string>();

                    foreach (DataRow row in dsDashBoard.Tables[tab_graficoVendaEstado].Rows)
                    {
                        cls_Comercial_Dashboard_CRM item = new cls_Comercial_Dashboard_CRM();
                        item.sIndicador = row["sEstado"].ToString();
                        item.sPeriodo = row["sPeriodo"].ToString();
                        item.nQuantidade = Convert.ToInt32(row["quantidade"]);
                        item.sFiltro = Convert.ToInt32(row["sFiltro"]);
                        item.idVendedor = Convert.ToInt32(row["idVendedor"]);

                        switch (Convert.ToInt32(row["sFiltro"]))
                        {
                            case 0:
                                sMesSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 1:
                                sTrimestreSet.Add(row["sPeriodo"].ToString());
                                break;
                            case 2:
                                sAnosSet.Add(row["sPeriodo"].ToString());
                                break;
                            default:
                                break;
                        }

                        bs_GraficoVendaEstado.Add(item);

                    }

                    foreach (var row in bs_GraficoVendaEstado)
                    {
                        switch (row.sFiltro)
                        {
                            case 0:
                                row.sLabels = sMesSet.ToArray();
                                row.sNomeGrafico = "Vendas por Estado - Mensal";
                                break;
                            case 1:
                                row.sLabels = sTrimestreSet.ToArray();
                                row.sNomeGrafico = "Vendas por Estado - Trimestral";
                                break;
                            case 2:
                                row.sLabels = sAnosSet.ToArray();
                                row.sNomeGrafico = "Vendas por Estado - Anual";
                                break;
                            default:
                                break;
                        }
                    }

                }

                //if (dsDashBoard.Tables[tab_graficoMetaVendedor].Rows.Count > 0)
                //{
                //    cls_Comercial_Dashboard_CRM item = new cls_Comercial_Dashboard_CRM();
                //    HashSet<string> sAnosSet = new HashSet<string>();
                //    HashSet<string> sTrimestreSet = new HashSet<string>();
                //    HashSet<string> sMesSet = new HashSet<string>();

                //    foreach (DataRow row in dsDashBoard.Tables[tab_graficoMetaVendedor].Rows)
                //    {
                //        item.sIndicador = row["sDscStatus"].ToString();
                //        item.sPeriodo = row["sPeriodo"].ToString();
                //        item.nQuantidade = Convert.ToInt32(row["quantidade"]);
                //        item.sFiltro = Convert.ToInt32(row["sFiltro"]);

                //        switch (Convert.ToInt32(row["sFiltro"]))
                //        {
                //            case 0:
                //                sAnosSet.Add(row["sPeriodo"].ToString());
                //                break;
                //            case 1:
                //                sTrimestreSet.Add(row["sPeriodo"].ToString());
                //                break;
                //            case 2:
                //                sMesSet.Add(row["sPeriodo"].ToString());
                //                break;
                //            default:
                //                break;
                //        }

                //        bs_GraficoTaxaFechamento.Add(item);

                //    }

                //    foreach (var row in bs_GraficoTaxaFechamento)
                //    {
                //        switch (row.sFiltro)
                //        {
                //            case 0:
                //                row.sLabels = sAnosSet.ToArray();
                //                break;
                //            case 1:
                //                row.sLabels = sTrimestreSet.ToArray();
                //                break;
                //            case 2:
                //                row.sLabels = sMesSet.ToArray();
                //                break;
                //            default:
                //                break;
                //        }
                //    }

                //}
            }
        }

        private void Preferencias()
        {
            DataSet ds;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR SESSAO");
            vParametros.Add("@idUsuario", Identity.Variaveis.idUsuario());
            ds = BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios_Preferencia", vParametros);
            if (BD.ValidarDataSet(ds))
            {
                if (Retorno.DATASET(ds, 0, "sDashboardCRM") != "")
                {
                    hddsPreferencia.Value = Retorno.DATASET(ds, 0, "sDashboardCRM");

                    lsGraficos.Clear();
                    string[] arrayPreferencia = hddsPreferencia.Value.Split('|');
                    lsGraficos = new List<int>(arrayPreferencia.Select(int.Parse).ToList());

                    foreach (ListItem item in cblsExibicao.Items)
                    {
                        item.Selected = false;
                    }

                    foreach (int item in lsGraficos)
                    {
                        ListItem grafico = cblsPreferencia.Items.FindByValue(item.ToString());
                        ListItem filtro = cblsExibicao.Items.FindByValue(item.ToString());
                        grafico.Selected = true;
                        filtro.Selected = true;
                    }

                    if (Permissoes())
                        ExibirGraficos();

                }
                else
                {
                    MensagemPagina.MostraMensagem_Aviso("Não há preferências salvas, utilize o campo de Preferências abaixo para salvá-las");
                }

            }
        }

        private void VisibleGraficos()
        {
            div_graficoTaxaFechamento.Visible = false;
            div_graficoPerdidos.Visible = false;
            div_graficoChanceFechamento.Visible = false;
            div_graficoEstadoCotacao.Visible = false;
            div_graficoTempoFechamento.Visible = false;
            div_graficoRankingClientes.Visible = false;
            div_graficoVendaFamilia.Visible = false;
            div_graficoQtdOrcamento.Visible = false;
            div_graficoVendaEstado.Visible = false;
        }

        private void ExibirGraficos()
        {
            StringBuilder sb = new StringBuilder();
            VisibleGraficos();
            foreach (int item in lsGraficos)
            {
                switch (item)
                {

                    case 0:

                        var sFiltroGraficoTaxaFechamento = new List<cls_Comercial_Dashboard_CRM>();
                        if (hddidVendedor.Value == "0")
                        {
                            sFiltroGraficoTaxaFechamento = bs_GraficoTaxaFechamento.Where(x => x.sFiltro.ToString() == hddsTipoPeriodo.Value).ToList();
                        }
                        else
                        {
                            sFiltroGraficoTaxaFechamento = bs_GraficoTaxaFechamento.Where(x => (x.sFiltro.ToString() == hddsTipoPeriodo.Value) && (x.idVendedor.ToString() == hddidVendedor.Value)).ToList();
                        }

                        if (sFiltroGraficoTaxaFechamento.Count > 0)
                        {

                            div_graficoTaxaFechamento.Visible = true;
                            string[] labelsGraficoTaxaFechamento = sFiltroGraficoTaxaFechamento.FirstOrDefault()?.sLabels;
                            var sIndicadorGraficoTaxaFechamento = sFiltroGraficoTaxaFechamento.Select(x => x.sIndicador).Distinct().ToList();

                            sb.AppendLine("<script type='text/javascript'>");
                            sb.AppendLine("$(document).ready(function() {");
                            sb.AppendLine(" var labels = [");
                            sb.AppendLine(string.Join(",", labelsGraficoTaxaFechamento.Select(l => $"'{l}'")));
                            sb.AppendLine("];");
                            sb.AppendLine("var datasets = [];");
                            Dictionary<string, string> corIndicadoresGraficoTaxaFechamento = new Dictionary<string, string>();

                            foreach (var indicador in sIndicadorGraficoTaxaFechamento)
                            {
                                var dataValuesGraficoTaxaFechamento = sFiltroGraficoTaxaFechamento.Where(x => x.sIndicador == indicador).OrderBy(x => Array.IndexOf(labelsGraficoTaxaFechamento, x.sPeriodo)).Select(x => x.nQuantidade).ToList();

                                //// Preencher zeros para períodos sem dados
                                //var dataValuesComplete = labelsGraficoTaxaFechamento.Select(label =>
                                //{
                                //    var i = sFiltroGraficoTaxaFechamento.FirstOrDefault(f => f.sIndicador == indicador && f.sPeriodo == label);
                                //    return i != null ? i.nQuantidade : 0;
                                //}).ToList();

                                string backgroundColorGraficoTaxaFechamento = GetCorPorIndicador(indicador);
                                corIndicadoresGraficoTaxaFechamento[indicador] = backgroundColorGraficoTaxaFechamento;

                                sb.AppendLine("datasets.push({");
                                sb.AppendLine(" label: '" + indicador + "',");
                                sb.AppendLine(" data: [" + string.Join(",", dataValuesGraficoTaxaFechamento) + "],");
                                sb.AppendLine(" backgroundColor: '" + backgroundColorGraficoTaxaFechamento + "', ");
                                //sb.AppendLine(" stack: 'Stack 0'");
                                sb.AppendLine("});");
                            }


                            sb.AppendLine("var ctx = document.getElementById('grafico_taxa_fechamento').getContext('2d');");
                            sb.AppendLine("var graficoTaxaFechamento = new Chart(ctx, {");
                            sb.AppendLine("    type: 'bar',");
                            sb.AppendLine("    data: {");
                            sb.AppendLine("        labels: labels,");
                            sb.AppendLine("        datasets: datasets.map(dataset => ({...dataset, maxBarThickness: 50}))");
                            sb.AppendLine("    },");
                            sb.AppendLine("    options: {");
                            sb.AppendLine("         plugins: {");
                            sb.AppendLine("             title: {");
                            sb.AppendLine("                 display: true,");
                            sb.AppendLine("                 text: '" + sFiltroGraficoTaxaFechamento.FirstOrDefault()?.sNomeGrafico + "'");
                            sb.AppendLine("             },");
                            sb.AppendLine("         },");
                            sb.AppendLine("         responsive: true,");
                            sb.AppendLine("         scale: {");
                            sb.AppendLine("             x: {");
                            sb.AppendLine("                 stacked: true,");
                            sb.AppendLine("             },");
                            sb.AppendLine("             y: {");
                            sb.AppendLine("                 stacked: true,");
                            sb.AppendLine("             }");
                            sb.AppendLine("         }");
                            sb.AppendLine("     }");
                            sb.AppendLine(" });");
                            sb.AppendLine("});");
                            sb.AppendLine("</script>");
                        }
                        else
                        {
                            MensagemPagina.MostraMensagem("Sem dados para o Gráfico Taxa de Fechamento", "info", false);
                        }

                        break; //  Grafico taxa fechamento

                    case 1:

                        var sFiltroGraficoPerdidos = new List<cls_Comercial_Dashboard_CRM>();
                        if (hddidVendedor.Value == "0")
                        {
                            sFiltroGraficoPerdidos = bs_GraficoPerdidos.Where(x => x.sFiltro.ToString() == hddsTipoPeriodo.Value).ToList();
                        }
                        else
                        {
                            sFiltroGraficoPerdidos = bs_GraficoPerdidos.Where(x => (x.sFiltro.ToString() == hddsTipoPeriodo.Value) && (x.idVendedor.ToString() == hddidVendedor.Value)).ToList();
                        }

                        if (sFiltroGraficoPerdidos.Count > 0)
                        {

                            div_graficoPerdidos.Visible = true;
                            string[] labelsGraficoPerdidos = sFiltroGraficoPerdidos.FirstOrDefault()?.sLabels;
                            var sIndicadorGraficoPerdidos = sFiltroGraficoPerdidos.Select(x => x.sIndicador).Distinct().ToList();

                            sb.AppendLine("<script type='text/javascript'>");
                            sb.AppendLine("$(document).ready(function() {");
                            sb.AppendLine(" var labels = [");
                            sb.AppendLine(string.Join(",", labelsGraficoPerdidos.Select(l => $"'{l}'")));
                            sb.AppendLine("];");
                            sb.AppendLine("var datasets = [];");
                            Dictionary<string, string> corIndicadoresGraficoPerdidos = new Dictionary<string, string>();

                            foreach (var indicador in sIndicadorGraficoPerdidos)
                            {
                                var dataValuesGraficoPerdidos = sFiltroGraficoPerdidos.Where(x => x.sIndicador == indicador).OrderBy(x => Array.IndexOf(labelsGraficoPerdidos, x.sPeriodo)).Select(x => x.nQuantidade).ToList();

                                //// Preencher zeros para períodos sem dados
                                //var dataValuesComplete = labelsGraficoPerdidos.Select(label =>
                                //{
                                //    var i = sFiltroGraficoPerdidos.FirstOrDefault(f => f.sIndicador == indicador && f.sPeriodo == label);
                                //    return i != null ? i.nQuantidade : 0;
                                //}).ToList();

                                string backgroundColorGraficoPerdidos = GetCorPorIndicador(indicador);
                                corIndicadoresGraficoPerdidos[indicador] = backgroundColorGraficoPerdidos;

                                sb.AppendLine("datasets.push({");
                                sb.AppendLine(" label: '" + indicador + "',");
                                sb.AppendLine(" data: [" + string.Join(",", dataValuesGraficoPerdidos) + "],");
                                sb.AppendLine(" backgroundColor: '" + backgroundColorGraficoPerdidos + "', ");
                                sb.AppendLine("});");
                            }


                            sb.AppendLine("var ctx2 = document.getElementById('grafico_perdidos').getContext('2d');");
                            sb.AppendLine("var graficoPerdidos = new Chart(ctx2, {");
                            sb.AppendLine("    type: 'bar',");
                            sb.AppendLine("    data: {");
                            sb.AppendLine("        labels: labels,");
                            sb.AppendLine("        datasets: datasets.map(dataset => ({...dataset, maxBarThickness: 50}))");
                            sb.AppendLine("    },");
                            sb.AppendLine("    options: {");
                            sb.AppendLine("         plugins: {");
                            sb.AppendLine("             legend: {");
                            sb.AppendLine("                 position: 'right',");
                            sb.AppendLine("             },");
                            sb.AppendLine("             title: {");
                            sb.AppendLine("                 display: true,");
                            sb.AppendLine("                 text: '" + sFiltroGraficoPerdidos.FirstOrDefault()?.sNomeGrafico + "'");
                            sb.AppendLine("             }");
                            sb.AppendLine("         },");
                            sb.AppendLine("         responsive: true,");
                            sb.AppendLine("         indexAxis: 'y',");
                            sb.AppendLine("         elements: {");
                            sb.AppendLine("             bar: {");
                            sb.AppendLine("                 borderWidth: 2,");
                            sb.AppendLine("             }");
                            sb.AppendLine("         }");
                            sb.AppendLine("     }");
                            sb.AppendLine(" });");
                            sb.AppendLine("});");
                            sb.AppendLine("</script>");
                        }
                        else
                        {
                            MensagemPagina.MostraMensagem("Sem dados para o Gráfico Taxa de Perdidos por Motivos", "info", false);
                        }

                        break; //  Grafico Taxa de Perdidos por Motivos

                    case 2:

                        var sFiltroGraficoChanceFechamento = new List<cls_Comercial_Dashboard_CRM>();
                        if (hddidVendedor.Value == "0")
                        {
                            sFiltroGraficoChanceFechamento = bs_GraficoEstadoOportunidade.Where(x => x.sFiltro.ToString() == hddsTipoPeriodo.Value).ToList();
                        }
                        else
                        {
                            sFiltroGraficoChanceFechamento = bs_GraficoEstadoOportunidade.Where(x => (x.sFiltro.ToString() == hddsTipoPeriodo.Value) && (x.idVendedor.ToString() == hddidVendedor.Value)).ToList();
                        }

                        if (sFiltroGraficoChanceFechamento.Count > 0)
                        {

                            div_graficoChanceFechamento.Visible = true;
                            string[] labelsGraficoChanceFechamento = sFiltroGraficoChanceFechamento.FirstOrDefault()?.sLabels;
                            var sIndicadorGraficoChanceFechamento = sFiltroGraficoChanceFechamento.Select(x => x.sIndicador).Distinct().ToList();

                            sb.AppendLine("<script type='text/javascript'>");
                            sb.AppendLine("$(document).ready(function() {");
                            sb.AppendLine(" var labels = [");
                            sb.AppendLine(string.Join(",", labelsGraficoChanceFechamento.Select(l => $"'{l}'")));
                            sb.AppendLine("];");
                            sb.AppendLine("var datasets = [];");
                            Dictionary<string, string> corIndicadoresGraficoChanceFechamento = new Dictionary<string, string>();

                            foreach (var indicador in sIndicadorGraficoChanceFechamento)
                            {
                                var dataValuesGraficoChanceFechamento = sFiltroGraficoChanceFechamento.Where(x => x.sIndicador == indicador).OrderBy(x => Array.IndexOf(labelsGraficoChanceFechamento, x.sPeriodo)).Select(x => x.nQuantidade).ToList();

                                //// Preencher zeros para períodos sem dados
                                //var dataValuesComplete = labelsGraficoPerdidos.Select(label =>
                                //{
                                //    var i = sFiltroGraficoPerdidos.FirstOrDefault(f => f.sIndicador == indicador && f.sPeriodo == label);
                                //    return i != null ? i.nQuantidade : 0;
                                //}).ToList();

                                string backgroundColorGraficoChanceFechamento = GetCorPorIndicador(indicador);
                                corIndicadoresGraficoChanceFechamento[indicador] = backgroundColorGraficoChanceFechamento;

                                sb.AppendLine("datasets.push({");
                                sb.AppendLine(" label: '" + indicador + "',");
                                sb.AppendLine(" data: [" + string.Join(",", dataValuesGraficoChanceFechamento) + "],");
                                sb.AppendLine(" backgroundColor: '" + backgroundColorGraficoChanceFechamento + "', ");
                                //sb.AppendLine(" stack: 'Stack 0'");
                                sb.AppendLine("});");
                            }


                            sb.AppendLine("var ctx3 = document.getElementById('grafico_chance_fechamento').getContext('2d');");
                            sb.AppendLine("var graficoChanceFechamento = new Chart(ctx3, {");
                            sb.AppendLine("    type: 'bar',");
                            sb.AppendLine("    data: {");
                            sb.AppendLine("        labels: labels,");
                            sb.AppendLine("        datasets: datasets.map(dataset => ({...dataset, maxBarThickness: 50}))");
                            sb.AppendLine("    },");
                            sb.AppendLine("    options: {");
                            sb.AppendLine("         plugins: {");
                            sb.AppendLine("             legend: {");
                            sb.AppendLine("                 position: 'right',");
                            sb.AppendLine("             },");
                            sb.AppendLine("             title: {");
                            sb.AppendLine("                 display: true,");
                            sb.AppendLine("                 text: '" + sFiltroGraficoChanceFechamento.FirstOrDefault()?.sNomeGrafico + "'");
                            sb.AppendLine("             }");
                            sb.AppendLine("         },");
                            sb.AppendLine("         responsive: true,");
                            sb.AppendLine("         scale: {");
                            sb.AppendLine("             x: {");
                            sb.AppendLine("                 stacked: true");
                            sb.AppendLine("             },");
                            sb.AppendLine("             y: {");
                            sb.AppendLine("                 stacked: true");
                            sb.AppendLine("             }");
                            sb.AppendLine("         }");
                            sb.AppendLine("     }");
                            sb.AppendLine(" });");
                            sb.AppendLine("});");
                            sb.AppendLine("</script>");
                        }
                        else
                        {
                            MensagemPagina.MostraMensagem("Sem dados para o Gráfico Estado de Oportunidade", "info", false);
                        }

                        break; //  Grafico Estado de Oportunidade

                    case 3:

                        var sFiltroGraficoEstadoCotacao = new List<cls_Comercial_Dashboard_CRM>();
                        if (hddidVendedor.Value == "0")
                        {
                            sFiltroGraficoEstadoCotacao = bs_GraficoCotacao.Where(x => x.sFiltro.ToString() == hddsTipoPeriodo.Value).ToList();
                        }
                        else
                        {
                            sFiltroGraficoEstadoCotacao = bs_GraficoCotacao.Where(x => (x.sFiltro.ToString() == hddsTipoPeriodo.Value) && (x.idVendedor.ToString() == hddidVendedor.Value)).ToList();
                        }

                        if (sFiltroGraficoEstadoCotacao.Count > 0)
                        {

                            div_graficoEstadoCotacao.Visible = true;
                            string[] labelsGraficoEstadoCotacao = sFiltroGraficoEstadoCotacao.FirstOrDefault()?.sLabels;
                            var sIndicadorGraficoEstadoCotacao = sFiltroGraficoEstadoCotacao.Select(x => x.sIndicador).Distinct().ToList();

                            sb.AppendLine("<script type='text/javascript'>");
                            sb.AppendLine("$(document).ready(function() {");
                            sb.AppendLine(" var labels = [");
                            sb.AppendLine(string.Join(",", labelsGraficoEstadoCotacao.Select(l => $"'{l}'")));
                            sb.AppendLine("];");
                            sb.AppendLine("var datasets = [];");
                            Dictionary<string, string> corIndicadoresGraficoEstadoCotacao = new Dictionary<string, string>();

                            foreach (var indicador in sIndicadorGraficoEstadoCotacao)
                            {
                                var dataValuesGraficoEstadoCotacao = sFiltroGraficoEstadoCotacao.Where(x => x.sIndicador == indicador).OrderBy(x => Array.IndexOf(labelsGraficoEstadoCotacao, x.sPeriodo)).Select(x => x.nQuantidade).ToList();

                                //// Preencher zeros para períodos sem dados
                                //var dataValuesComplete = labelsGraficoPerdidos.Select(label =>
                                //{
                                //    var i = sFiltroGraficoPerdidos.FirstOrDefault(f => f.sIndicador == indicador && f.sPeriodo == label);
                                //    return i != null ? i.nQuantidade : 0;
                                //}).ToList();

                                string backgroundColorGraficoEstadoCotacao = GetCorPorIndicador(indicador);
                                corIndicadoresGraficoEstadoCotacao[indicador] = backgroundColorGraficoEstadoCotacao;

                                sb.AppendLine("datasets.push({");
                                sb.AppendLine(" label: '" + indicador + "',");
                                sb.AppendLine(" data: [" + string.Join(",", dataValuesGraficoEstadoCotacao) + "],");
                                sb.AppendLine(" backgroundColor: '" + backgroundColorGraficoEstadoCotacao + "', ");
                                //sb.AppendLine(" stack: 'Stack 0'");
                                sb.AppendLine("});");
                            }


                            sb.AppendLine("var ctx3 = document.getElementById('grafico_estado_cotacao').getContext('2d');");
                            sb.AppendLine("var graficoEstadoCotacao = new Chart(ctx3, {");
                            sb.AppendLine("    type: 'bar',");
                            sb.AppendLine("    data: {");
                            sb.AppendLine("        labels: labels,");
                            sb.AppendLine("        datasets: datasets.map(dataset => ({...dataset, maxBarThickness: 50}))");
                            sb.AppendLine("    },");
                            sb.AppendLine("    options: {");
                            sb.AppendLine("         plugins: {");
                            sb.AppendLine("             title: {");
                            sb.AppendLine("                 display: true,");
                            sb.AppendLine("                 text: '" + sFiltroGraficoEstadoCotacao.FirstOrDefault()?.sNomeGrafico + "'");
                            sb.AppendLine("             }");
                            sb.AppendLine("         },");
                            sb.AppendLine("         responsive: true,");
                            sb.AppendLine("         scale: {");
                            sb.AppendLine("             x: {");
                            sb.AppendLine("                 stacked: true");
                            sb.AppendLine("             },");
                            sb.AppendLine("             y: {");
                            sb.AppendLine("                 stacked: true");
                            sb.AppendLine("             }");
                            sb.AppendLine("         }");
                            sb.AppendLine("     }");
                            sb.AppendLine(" });");
                            sb.AppendLine("});");
                            sb.AppendLine("</script>");
                        }
                        else
                        {
                            MensagemPagina.MostraMensagem("Sem dados para o Gráfico Estado de Cotações", "info", false);
                        }

                        break; //  Grafico Estado de Cotações

                    case 4:

                        var sFiltroGraficoTempoMedioFechamento = new List<cls_Comercial_Dashboard_CRM>();
                        if (hddidVendedor.Value == "0")
                        {
                            sFiltroGraficoTempoMedioFechamento = bs_GraficoTempoMedioFechamento.Where(x => x.sFiltro.ToString() == hddsTipoPeriodo.Value).ToList();
                        }
                        else
                        {
                            sFiltroGraficoTempoMedioFechamento = bs_GraficoTempoMedioFechamento.Where(x => (x.sFiltro.ToString() == hddsTipoPeriodo.Value) && (x.idVendedor.ToString() == hddidVendedor.Value)).ToList();
                        }

                        if (sFiltroGraficoTempoMedioFechamento.Count > 0)
                        {

                            div_graficoTempoFechamento.Visible = true;
                            string[] labelsGraficoTempoMedioFechamento = sFiltroGraficoTempoMedioFechamento.FirstOrDefault()?.sLabels;
                            var sIndicadorGraficoTempoMedioFechamento = sFiltroGraficoTempoMedioFechamento.Select(x => x.sIndicador).Distinct().ToList();

                            sb.AppendLine("<script type='text/javascript'>");
                            sb.AppendLine("$(document).ready(function() {");
                            sb.AppendLine(" var labels = [");
                            sb.AppendLine(string.Join(",", labelsGraficoTempoMedioFechamento.Select(l => $"'{l}'")));
                            sb.AppendLine("];");
                            sb.AppendLine("var datasets = [];");
                            Dictionary<string, string> corIndicadoresGraficoTempoMedioFechamento = new Dictionary<string, string>();

                            foreach (var indicador in sIndicadorGraficoTempoMedioFechamento)
                            {
                                var dataValuesGraficoTempoMedioFechamento = sFiltroGraficoTempoMedioFechamento.Where(x => x.sIndicador == indicador).OrderBy(x => Array.IndexOf(labelsGraficoTempoMedioFechamento, x.sPeriodo)).Select(x => x.nQuantidade).ToList();

                                //// Preencher zeros para períodos sem dados
                                //var dataValuesComplete = labelsGraficoPerdidos.Select(label =>
                                //{
                                //    var i = sFiltroGraficoPerdidos.FirstOrDefault(f => f.sIndicador == indicador && f.sPeriodo == label);
                                //    return i != null ? i.nQuantidade : 0;
                                //}).ToList();

                                string backgroundColorGraficoTempoMedioFechamento = GetCorPorIndicador(indicador);
                                corIndicadoresGraficoTempoMedioFechamento[indicador] = backgroundColorGraficoTempoMedioFechamento;

                                sb.AppendLine("datasets.push({");
                                sb.AppendLine(" label: '" + indicador + "',");
                                sb.AppendLine(" data: [" + string.Join(",", dataValuesGraficoTempoMedioFechamento) + "],");
                                sb.AppendLine(" backgroundColor: '" + backgroundColorGraficoTempoMedioFechamento + "', ");
                                sb.AppendLine("});");
                            }


                            sb.AppendLine("var ctx4 = document.getElementById('grafico_tempo_fechamento').getContext('2d');");
                            sb.AppendLine("var graficoTempoFechamento = new Chart(ctx4, {");
                            sb.AppendLine("    type: 'bar',");
                            sb.AppendLine("    data: {");
                            sb.AppendLine("        labels: labels,");
                            sb.AppendLine("        datasets: datasets.map(dataset => ({...dataset, maxBarThickness: 50}))");
                            sb.AppendLine("    },");
                            sb.AppendLine("    options: {");
                            sb.AppendLine("         plugins: {");
                            sb.AppendLine("             legend: {");
                            sb.AppendLine("                 position: 'top',");
                            sb.AppendLine("             },");
                            sb.AppendLine("             title: {");
                            sb.AppendLine("                 display: true,");
                            sb.AppendLine("                 text: '" + sFiltroGraficoTempoMedioFechamento.FirstOrDefault()?.sNomeGrafico + "'");
                            sb.AppendLine("             }");
                            sb.AppendLine("         },");
                            sb.AppendLine("         responsive: true");
                            sb.AppendLine("     }");
                            sb.AppendLine(" });");
                            sb.AppendLine("});");
                            sb.AppendLine("</script>");
                        }
                        else
                        {
                            MensagemPagina.MostraMensagem("Sem dados para o Gráfico Tempo Médio de Fechamento", "info", false);
                        }

                        break; //  Grafico Tempo Medio de Fechamento

                    case 5:

                        var sFiltroGraficoRankingCliente = new List<cls_Comercial_Dashboard_CRM>();
                        if (hddidVendedor.Value == "0")
                        {
                            sFiltroGraficoRankingCliente = bs_GraficoRankingCliente.Where(x => x.sFiltro.ToString() == hddsTipoPeriodo.Value).ToList();
                        }
                        else
                        {
                            sFiltroGraficoRankingCliente = bs_GraficoRankingCliente.Where(x => (x.sFiltro.ToString() == hddsTipoPeriodo.Value) && (x.idVendedor.ToString() == hddidVendedor.Value)).ToList();
                        }

                        if (sFiltroGraficoRankingCliente.Count > 0)
                        {

                            div_graficoRankingClientes.Visible = true;
                            string[] labelsGraficoRankingCliente = sFiltroGraficoRankingCliente.FirstOrDefault()?.sLabels;
                            var sIndicadorGraficoRankingCliente = sFiltroGraficoRankingCliente.Select(x => x.sIndicador).Distinct().ToList();

                            sb.AppendLine("<script type='text/javascript'>");
                            sb.AppendLine("$(document).ready(function() {");
                            sb.AppendLine(" var labels = [");
                            sb.AppendLine(string.Join(",", labelsGraficoRankingCliente.Select(l => $"'{l}'")));
                            sb.AppendLine("];");
                            sb.AppendLine("var datasets = [];");
                            Dictionary<string, string> corIndicadoresGraficoRankingCliente = new Dictionary<string, string>();

                            foreach (var indicador in sIndicadorGraficoRankingCliente)
                            {
                                var dataValuesGraficoRankingCliente = sFiltroGraficoRankingCliente.Where(x => x.sIndicador == indicador).OrderBy(x => Array.IndexOf(labelsGraficoRankingCliente, x.sPeriodo)).Select(x => x.nQuantidade).ToList();

                                //// Preencher zeros para períodos sem dados
                                //var dataValuesComplete = labelsGraficoPerdidos.Select(label =>
                                //{
                                //    var i = sFiltroGraficoPerdidos.FirstOrDefault(f => f.sIndicador == indicador && f.sPeriodo == label);
                                //    return i != null ? i.nQuantidade : 0;
                                //}).ToList();

                                string backgroundColorGraficoRankingCliente = GetCorPorIndicador(indicador);
                                corIndicadoresGraficoRankingCliente[indicador] = backgroundColorGraficoRankingCliente;

                                sb.AppendLine("datasets.push({");
                                sb.AppendLine(" label: '" + indicador + "',");
                                sb.AppendLine(" data: [" + string.Join(",", dataValuesGraficoRankingCliente) + "],");
                                sb.AppendLine(" backgroundColor: '" + backgroundColorGraficoRankingCliente + "', ");
                                sb.AppendLine("});");
                            }


                            sb.AppendLine("var ctx5 = document.getElementById('grafico_ranking_clientes').getContext('2d');");
                            sb.AppendLine("var graficoRankingClientes = new Chart(ctx5, {");
                            sb.AppendLine("    type: 'bar',");
                            sb.AppendLine("    data: {");
                            sb.AppendLine("        labels: labels,");
                            sb.AppendLine("        datasets: datasets");
                            sb.AppendLine("    },");
                            sb.AppendLine("    options: {");
                            sb.AppendLine("         plugins: {");
                            sb.AppendLine("             legend: {");
                            sb.AppendLine("                 position: 'right',");
                            sb.AppendLine("             },");
                            sb.AppendLine("             title: {");
                            sb.AppendLine("                 display: true,");
                            sb.AppendLine("                 text: '" + sFiltroGraficoRankingCliente.FirstOrDefault()?.sNomeGrafico + "'");
                            sb.AppendLine("             }");
                            sb.AppendLine("         },");
                            sb.AppendLine("         responsive: true,");
                            sb.AppendLine("         scale: {");
                            sb.AppendLine("             y: {");
                            sb.AppendLine("                 beginAtZero: true,");
                            sb.AppendLine("                 categoryPercentage: 0.8,");
                            sb.AppendLine("                 barPercentage: 0.9,");
                            sb.AppendLine("             }");
                            sb.AppendLine("         },");
                            sb.AppendLine("         elements: {");
                            sb.AppendLine("             bar: {");
                            sb.AppendLine("                 barThickness: 20,");
                            sb.AppendLine("                 maxBarThickness: 30");
                            sb.AppendLine("             }");
                            sb.AppendLine("         },");
                            sb.AppendLine("         maintainAspectRatio: false,");
                            sb.AppendLine("         indexAxis: 'y'");
                            sb.AppendLine("     }");
                            sb.AppendLine(" });");
                            sb.AppendLine("});");
                            sb.AppendLine("</script>");
                        }
                        else
                        {
                            MensagemPagina.MostraMensagem("Sem dados para o Gráfico Ranking de Clientes", "info", false);
                        }

                        break; //  Grafico Clientes Ranking

                    case 6:

                        var sFiltroGraficoVendaFamilia = new List<cls_Comercial_Dashboard_CRM>();
                        if (hddidVendedor.Value == "0")
                        {
                            sFiltroGraficoVendaFamilia = bs_GraficoVendaFamilia.Where(x => x.sFiltro.ToString() == hddsTipoPeriodo.Value).ToList();
                        }
                        else
                        {
                            sFiltroGraficoVendaFamilia = bs_GraficoVendaFamilia.Where(x => (x.sFiltro.ToString() == hddsTipoPeriodo.Value) && (x.idVendedor.ToString() == hddidVendedor.Value)).ToList();
                        }

                        if (sFiltroGraficoVendaFamilia.Count > 0)
                        {

                            div_graficoVendaFamilia.Visible = true;
                            string[] labelsGraficoVendaFamilia = sFiltroGraficoVendaFamilia.FirstOrDefault()?.sLabels;
                            var sIndicadorGraficoVendaFamilia = sFiltroGraficoVendaFamilia.Select(x => x.sIndicador).Distinct().ToList();

                            sb.AppendLine("<script type='text/javascript'>");
                            sb.AppendLine("$(document).ready(function() {");
                            sb.AppendLine(" var labels = [");
                            sb.AppendLine(string.Join(",", labelsGraficoVendaFamilia.Select(l => $"'{l}'")));
                            sb.AppendLine("];");
                            sb.AppendLine("var datasets = [];");
                            Dictionary<string, string> corIndicadoresGraficoVendaFamilia = new Dictionary<string, string>();

                            foreach (var indicador in sIndicadorGraficoVendaFamilia)
                            {
                                var dataValuesGraficoVendaFamilia = sFiltroGraficoVendaFamilia.Where(x => x.sIndicador == indicador).OrderBy(x => Array.IndexOf(labelsGraficoVendaFamilia, x.sPeriodo)).Select(x => x.nQuantidade).ToList();

                                //// Preencher zeros para períodos sem dados
                                //var dataValuesComplete = labelsGraficoPerdidos.Select(label =>
                                //{
                                //    var i = sFiltroGraficoPerdidos.FirstOrDefault(f => f.sIndicador == indicador && f.sPeriodo == label);
                                //    return i != null ? i.nQuantidade : 0;
                                //}).ToList();

                                string backgroundColorGraficoVendaFamilia = GetCorPorIndicador(indicador);
                                corIndicadoresGraficoVendaFamilia[indicador] = backgroundColorGraficoVendaFamilia;

                                sb.AppendLine("datasets.push({");
                                sb.AppendLine(" label: '" + indicador + "',");
                                sb.AppendLine(" data: [" + string.Join(",", dataValuesGraficoVendaFamilia) + "],");
                                sb.AppendLine(" backgroundColor: '" + backgroundColorGraficoVendaFamilia + "', ");
                                sb.AppendLine("});");
                            }


                            sb.AppendLine("var ctx6 = document.getElementById('grafico_vendas_familia').getContext('2d');");
                            sb.AppendLine("var graficoVendasFamilia = new Chart(ctx6, {");
                            sb.AppendLine("    type: 'bar',");
                            sb.AppendLine("    data: {");
                            sb.AppendLine("        labels: labels,");
                            sb.AppendLine("        datasets: datasets");
                            sb.AppendLine("    },");
                            sb.AppendLine("    options: {");
                            sb.AppendLine("         plugins: {");
                            sb.AppendLine("             legend: {");
                            sb.AppendLine("                 position: 'right',");
                            sb.AppendLine("             },");
                            sb.AppendLine("             title: {");
                            sb.AppendLine("                 display: true,");
                            sb.AppendLine("                 text: '" + sFiltroGraficoVendaFamilia.FirstOrDefault()?.sNomeGrafico + "'");
                            sb.AppendLine("             }");
                            sb.AppendLine("         },");
                            sb.AppendLine("         responsive: true,");
                            sb.AppendLine("         scale: {");
                            sb.AppendLine("             y: {");
                            sb.AppendLine("                 beginAtZero: true,");
                            sb.AppendLine("                 categoryPercentage: 0.8,");
                            sb.AppendLine("                 barPercentage: 0.9,");
                            sb.AppendLine("             }");
                            sb.AppendLine("         },");
                            sb.AppendLine("         elements: {");
                            sb.AppendLine("             bar: {");
                            sb.AppendLine("                 barThickness: 20,");
                            sb.AppendLine("                 maxBarThickness: 30");
                            sb.AppendLine("             }");
                            sb.AppendLine("         },");
                            sb.AppendLine("         maintainAspectRatio: false,");
                            sb.AppendLine("         indexAxis: 'y'");
                            sb.AppendLine("     }");
                            sb.AppendLine(" });");
                            sb.AppendLine("});");
                            sb.AppendLine("</script>");

                        }
                        else
                        {
                            MensagemPagina.MostraMensagem("Sem dados para o Gráfico Vendas por Família", "info", false);
                        }

                        break; //  Grafico Vendas por Familia

                    case 7:

                        var sFiltroGraficoQtdOrcamento = new List<cls_Comercial_Dashboard_CRM>();
                        if (hddidVendedor.Value == "0")
                        {
                            sFiltroGraficoQtdOrcamento = bs_GraficoQtdOrcamento.Where(x => x.sFiltro.ToString() == hddsTipoPeriodo.Value).ToList();
                        }
                        else
                        {
                            sFiltroGraficoQtdOrcamento = bs_GraficoQtdOrcamento.Where(x => (x.sFiltro.ToString() == hddsTipoPeriodo.Value) && (x.idVendedor.ToString() == hddidVendedor.Value)).ToList();
                        }

                        if(sFiltroGraficoQtdOrcamento.Count > 0)
                        {

                            div_graficoQtdOrcamento.Visible = true;
                            string[] labelsGraficoQtdOrcamento = sFiltroGraficoQtdOrcamento.FirstOrDefault()?.sLabels;
                            var sIndicadorGraficoQtdOrcamento = sFiltroGraficoQtdOrcamento.Select(x => x.sIndicador).Distinct().ToList();

                            sb.AppendLine("<script type='text/javascript'>");
                            sb.AppendLine("$(document).ready(function() {");
                            sb.AppendLine(" var labels = [");
                            sb.AppendLine(string.Join(",", labelsGraficoQtdOrcamento.Select(l => $"'{l}'")));
                            sb.AppendLine("];");
                            sb.AppendLine("var datasets = [];");
                            Dictionary<string, string> corIndicadoresGraficoQtdOrcamento = new Dictionary<string, string>();

                            foreach (var indicador in sIndicadorGraficoQtdOrcamento)
                            {
                                var dataValuesGraficoQtdOrcamento = sFiltroGraficoQtdOrcamento.Where(x => x.sIndicador == indicador).OrderBy(x => Array.IndexOf(labelsGraficoQtdOrcamento, x.sPeriodo)).Select(x => x.nQuantidade).ToList();

                                //// Preencher zeros para períodos sem dados
                                //var dataValuesComplete = labelsGraficoPerdidos.Select(label =>
                                //{
                                //    var i = sFiltroGraficoPerdidos.FirstOrDefault(f => f.sIndicador == indicador && f.sPeriodo == label);
                                //    return i != null ? i.nQuantidade : 0;
                                //}).ToList();

                                string backgroundColorGraficoQtdOrcamento = GetCorPorIndicador(indicador);
                                corIndicadoresGraficoQtdOrcamento[indicador] = backgroundColorGraficoQtdOrcamento;

                                sb.AppendLine("datasets.push({");
                                sb.AppendLine(" label: '" + indicador + "',");
                                sb.AppendLine(" data: [" + string.Join(",", dataValuesGraficoQtdOrcamento) + "],");
                                sb.AppendLine(" backgroundColor: '" + backgroundColorGraficoQtdOrcamento + "', ");
                                sb.AppendLine("});");
                            }


                            sb.AppendLine("var ctx7 = document.getElementById('grafico_quantidade_cotacoes').getContext('2d');");
                            sb.AppendLine("var graficoQuantidadeCotacoes = new Chart(ctx7, {");
                            sb.AppendLine("    type: 'bar',");
                            sb.AppendLine("    data: {");
                            sb.AppendLine("        labels: labels,");
                            sb.AppendLine("        datasets: datasets.map(dataset => ({...dataset, maxBarThickness: 50}))");
                            sb.AppendLine("    },");
                            sb.AppendLine("    options: {");
                            sb.AppendLine("         plugins: {");
                            sb.AppendLine("             legend: {");
                            sb.AppendLine("                 position: 'top',");
                            sb.AppendLine("             },");
                            sb.AppendLine("             title: {");
                            sb.AppendLine("                 display: true,");
                            sb.AppendLine("                 text: '" + sFiltroGraficoQtdOrcamento.FirstOrDefault()?.sNomeGrafico + "'");
                            sb.AppendLine("             }");
                            sb.AppendLine("         },");
                            sb.AppendLine("         responsive: true");
                            sb.AppendLine("     }");
                            sb.AppendLine(" });");
                            sb.AppendLine("});");
                            sb.AppendLine("</script>");
                        }
                        else
                        {
                            MensagemPagina.MostraMensagem("Sem dados para o Gráfico Quantidade de Orçamentos", "info", false);
                        }

                        break; //  Grafico de Orçamentos

                    case 8:


                        var sFiltroGraficoVendaEstado = new List<cls_Comercial_Dashboard_CRM>();
                        if (hddidVendedor.Value == "0")
                        {
                            sFiltroGraficoVendaEstado = bs_GraficoVendaEstado.Where(x => x.sFiltro.ToString() == hddsTipoPeriodo.Value).ToList();
                        }
                        else
                        {
                            sFiltroGraficoVendaEstado = bs_GraficoVendaEstado.Where(x => (x.sFiltro.ToString() == hddsTipoPeriodo.Value) && (x.idVendedor.ToString() == hddidVendedor.Value)).ToList();
                        }

                        if (sFiltroGraficoVendaEstado.Count > 0)
                        {

                            div_graficoVendaEstado.Visible = true;
                            string[] labelsGraficoVendaEstado = sFiltroGraficoVendaEstado.FirstOrDefault()?.sLabels;
                            var sIndicadorGraficoVendaEstado = sFiltroGraficoVendaEstado.Select(x => x.sIndicador).Distinct().ToList();

                            sb.AppendLine("<script type='text/javascript'>");
                            sb.AppendLine("$(document).ready(function() {");
                            sb.AppendLine(" var labels = [");
                            sb.AppendLine(string.Join(",", labelsGraficoVendaEstado.Select(l => $"'{l}'")));
                            sb.AppendLine("];");
                            sb.AppendLine("var datasets = [];");
                            Dictionary<string, string> corIndicadoresGraficoVendaEstado = new Dictionary<string, string>();

                            foreach (var indicador in sIndicadorGraficoVendaEstado)
                            {
                                var dataValuesGraficoVendaEstado = sFiltroGraficoVendaEstado.Where(x => x.sIndicador == indicador).OrderBy(x => Array.IndexOf(labelsGraficoVendaEstado, x.sPeriodo)).Select(x => x.nQuantidade).ToList();

                                //// Preencher zeros para períodos sem dados
                                //var dataValuesComplete = labelsGraficoPerdidos.Select(label =>
                                //{
                                //    var i = sFiltroGraficoPerdidos.FirstOrDefault(f => f.sIndicador == indicador && f.sPeriodo == label);
                                //    return i != null ? i.nQuantidade : 0;
                                //}).ToList();

                                string backgroundColorGraficoVendaEstado = GetCorPorIndicador(indicador);
                                corIndicadoresGraficoVendaEstado[indicador] = backgroundColorGraficoVendaEstado;

                                sb.AppendLine("datasets.push({");
                                sb.AppendLine(" label: '" + indicador + "',");
                                sb.AppendLine(" data: [" + string.Join(",", dataValuesGraficoVendaEstado) + "],");
                                sb.AppendLine(" backgroundColor: '" + backgroundColorGraficoVendaEstado + "', ");
                                sb.AppendLine("});");
                            }


                            sb.AppendLine("var ctx8 = document.getElementById('grafico_vendas_estado').getContext('2d');");
                            sb.AppendLine("var graficoVendasEstado = new Chart(ctx8, {");
                            sb.AppendLine("    type: 'bar',");
                            sb.AppendLine("    data: {");
                            sb.AppendLine("        labels: labels,");
                            sb.AppendLine("        datasets: datasets.map(dataset => ({...dataset, maxBarThickness: 50}))");
                            sb.AppendLine("    },");
                            sb.AppendLine("    options: {");
                            sb.AppendLine("         plugins: {");
                            sb.AppendLine("             legend: {");
                            sb.AppendLine("                 position: 'top',");
                            sb.AppendLine("             },");
                            sb.AppendLine("             title: {");
                            sb.AppendLine("                 display: true,");
                            sb.AppendLine("                 text: '" + sFiltroGraficoVendaEstado.FirstOrDefault()?.sNomeGrafico + "'");
                            sb.AppendLine("             }");
                            sb.AppendLine("         },");
                            sb.AppendLine("         responsive: true");
                            sb.AppendLine("     }");
                            sb.AppendLine(" });");
                            sb.AppendLine("});");
                            sb.AppendLine("</script>");
                        }
                        else
                        {
                            MensagemPagina.MostraMensagem("Sem dados para o Gráfico Vendas por Estado", "info", false);
                        }

                        break; //  Grafico Vendas por Estado

                    case 9:    //  Grafico Meta Vendedor

                        break;
                    default:

                        break;

                }
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "ChartScript", sb.ToString(), false);


        } //FALTA COISA

        protected void cmdFiltro_Click(object sender, EventArgs e)
        {
            lsGraficos.Clear();
            foreach (ListItem item in cblsExibicao.Items)
            {
                if (item.Selected)
                {
                    lsGraficos.Add(Convert.ToInt32(item.Value));
                }
            }

            hddsTipoPeriodo.Value = ddlPeriodo.SelectedValue;

            if (Funcoes.ValidaPermissao(Permissao.Comercial.DashboardCRM.Visualizar_Vendedor))
            {
                try
                {
                    ddlidVendedor.SelectedValue = ConsultaVendedor();
                    ddlidVendedor.Attributes.Add("disabled", "disabled");
                    hddidVendedor.Value = ddlidVendedor.SelectedValue;
                    ExibirGraficos();
                }
                catch
                {
                    VisibleGraficos();
                    MensagemPagina.MostraMensagem_Erro("Seu Usuário não é um vendedor, necessário atualizar as informações ou permissões no perfil");
                }
            }
            else
            {
                hddidVendedor.Value = ddlidVendedor.SelectedValue;
                ExibirGraficos();
            }

        }

        protected void cmdSalvarPreferencia_Click(object sender, EventArgs e)
        {

            List<string> lsPreferencias = new List<string>();
            foreach (ListItem item in cblsPreferencia.Items)
            {
                if (item.Selected)
                {
                    lsPreferencias.Add(item.Value);
                }
            }

            hddsPreferencia.Value = string.Join("|", lsPreferencias);

            if (lsPreferencias.Count > 0)
            {
                DataTable dt;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "SALVAR_PREFERENCIA");
                vParametros.Add("@sPreferencia", hddsPreferencia.Value);
                vParametros.Add("@idUsuario", Identity.Variaveis.idUsuario());
                dt = BD.ExecutarDataTable(sProcedure, vParametros);

                Preferencias();
                MensagemPagina.MostraMensagem_Sucesso("Preferências salvas e aplicadas com sucesso!", false);
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("É necessário selecionar pelo menos 1 gráfico");
            }

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

        public string GetCorPorIndicador(string indicador)
        {
            int hash = indicador.GetHashCode();

            byte red = (byte)((hash & 0xFF0000) >> 16);
            byte green = (byte)((hash & 0x00FF00) >> 8);
            byte blue = (byte)((hash & 0x0000FF));

            red = (byte)(red / 2 + 64);
            green = (byte)(green / 2 + 64);
            blue = (byte)(blue / 2 + 64);

            return $"#{red:X2}{green:X2}{blue:X2}";
        }

        protected void lnkAtualizar_Click(object sender, EventArgs e)
        {
            if (Permissoes())
            {
                ConsultarDados();
                Preferencias();
            }
        }
    }
}