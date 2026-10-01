using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using TT_Flow.FrameWork;
using static TT.FrameWork.BD;
using static TT.FrameWork.BD.Retorno;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Grid;
using static TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class Produtos_Detalhe : Page
    {
        #region | Construtores

        #region | Propriedades

        string sTituloPagina = "Produto";
        string sTituloPaginaServico = "Serviço";
        string sTituloPaginaSubServico = "Sub-Serviço";
        string sTituloPaginaRecurso = "Recurso";
        string sProcedure = "sp_Manipula_tbl_Flow_Produtos";
        static int idLinhaConstante = 0;
        public static string sCodigoServicoPai = "";
        public static string sCodigoServicoFilho = "";
        public string sMsgErro = "";

        #endregion

        #region | Listas

        public List<cls_WMS_Produtos> bs_Produto_Composicao
        {
            get
            {
                if (ViewState["bs_Produto_Composicao"] == null)
                {
                    ViewState["bs_Produto_Composicao"] = new List<cls_WMS_Produtos>();
                }
                return (List<cls_WMS_Produtos>)ViewState["bs_Produto_Composicao"];
            }
            set
            {
                ViewState["bs_Produto_Composicao"] = value;
            }
        }

        /// <summary>
        /// Itens da aba Instalação/Obra. DTO próprio e enxuto: a lista vive em ViewState e
        /// cls_WMS_Produtos traria dezenas de propriedades sem uso aqui.
        /// </summary>
        public List<cls_ProdutoInstalacaoItem> bs_Produto_Instalacao
        {
            get
            {
                if (ViewState["bs_Produto_Instalacao"] == null)
                {
                    ViewState["bs_Produto_Instalacao"] = new List<cls_ProdutoInstalacaoItem>();
                }
                return (List<cls_ProdutoInstalacaoItem>)ViewState["bs_Produto_Instalacao"];
            }
            set
            {
                ViewState["bs_Produto_Instalacao"] = value;
            }
        }
        public List<cls_WMS_Produtos_Sugestao> BS_SUGESTAO
        {
            get
            {
                if (ViewState["BS_SUGESTAO"] == null)
                {
                    ViewState["BS_SUGESTAO"] = new List<cls_WMS_Produtos_Sugestao>();
                }
                return (List<cls_WMS_Produtos_Sugestao>)ViewState["BS_SUGESTAO"];
            }
            set
            {
                ViewState["BS_SUGESTAO"] = value;
            }
        }

        public List<cls_WMS_CEST> BS_CEST
        {
            get
            {
                if (ViewState["BS_CEST"] == null)
                {
                    ViewState["BS_CEST"] = new List<cls_WMS_CEST>();
                }
                return (List<cls_WMS_CEST>)ViewState["BS_CEST"];
            }
            set
            {
                ViewState["BS_CEST"] = value;
            }
        }

        public List<cls_WMS_NCM> BS_NCM
        {
            get
            {
                if (ViewState["BS_NCM"] == null)
                {
                    ViewState["BS_NCM"] = new List<cls_WMS_NCM>();
                }
                return (List<cls_WMS_NCM>)ViewState["BS_NCM"];
            }
            set
            {
                ViewState["BS_NCM"] = value;
            }
        }

        public List<cls_WMS_Movimentacao_Motivo> BS_MOTIVO
        {
            get
            {
                if (ViewState["BS_MOTIVO"] == null)
                {
                    ViewState["BS_MOTIVO"] = new List<cls_WMS_Movimentacao_Motivo>();
                }
                return (List<cls_WMS_Movimentacao_Motivo>)ViewState["BS_MOTIVO"];
            }
            set
            {
                ViewState["BS_MOTIVO"] = value;
            }
        }

        public List<cls_Produtos_Descricao> bs_Produtos_Descricao
        {
            get
            {
                if (ViewState["bs_Produtos_Descricao"] == null)
                {
                    ViewState["bs_Produtos_Descricao"] = new List<cls_Produtos_Descricao>();
                }
                return (List<cls_Produtos_Descricao>)ViewState["bs_Produtos_Descricao"];
            }
            set
            {
                ViewState["bs_Produtos_Descricao"] = value;
            }
        }

        public List<cls_WMS_Produtos_Fornecedores> BS_FORNECEDORES
        {
            get
            {
                if (ViewState["BS_FORNECEDORES"] == null)
                {
                    ViewState["BS_FORNECEDORES"] = new List<cls_WMS_Produtos_Fornecedores>();
                }
                return (List<cls_WMS_Produtos_Fornecedores>)ViewState["BS_FORNECEDORES"];
            }
            set
            {
                ViewState["BS_FORNECEDORES"] = value;
            }
        }

        public List<cls_WMS_Produtos_Fornecedores> bs_Parceiros
        {
            get
            {
                if (ViewState["bs_Parceiros"] == null)
                {
                    ViewState["bs_Parceiros"] = new List<cls_WMS_Produtos_Fornecedores>();
                }
                return (List<cls_WMS_Produtos_Fornecedores>)ViewState["bs_Parceiros"];
            }
            set
            {
                ViewState["bs_Parceiros"] = value;
            }
        }

        //Agnes Partal * 07/08/2024
        public List<cls_WMS_Produtos_Documentacao> bs_Produtos_Documentacao
        {
            get
            {
                if (ViewState["bs_Produtos_Documentacao"] == null)
                {
                    ViewState["bs_Produtos_Documentacao"] = new List<cls_WMS_Produtos_Documentacao>();
                }
                return (List<cls_WMS_Produtos_Documentacao>)ViewState["bs_Produtos_Documentacao"];
            }
            set
            {
                ViewState["bs_Produtos_Documentacao"] = value;
            }
        }

        public List<cls_WMS_Fabricacao_Produto> bs_Fabricacao_Produto
        {
            get
            {
                if (ViewState["bs_Fabricacao_Produto"] == null)
                {
                    ViewState["bs_Fabricacao_Produto"] = new List<cls_WMS_Fabricacao_Produto>();
                }
                return (List<cls_WMS_Fabricacao_Produto>)ViewState["bs_Fabricacao_Produto"];
            }
            set
            {
                ViewState["bs_Fabricacao_Produto"] = value;
            }
        }

        public List<cls_WMS_Fabricacao_InsumoFerramenta> bs_Fabricacao_InsumoFerramenta
        {
            get
            {
                if (ViewState["bs_Fabricacao_InsumoFerramenta"] == null)
                {
                    ViewState["bs_Fabricacao_InsumoFerramenta"] = new List<cls_WMS_Fabricacao_InsumoFerramenta>();
                }
                return (List<cls_WMS_Fabricacao_InsumoFerramenta>)ViewState["bs_Fabricacao_InsumoFerramenta"];
            }
            set
            {
                ViewState["bs_Fabricacao_InsumoFerramenta"] = value;
            }
        }

        public List<cls_WMS_Fabricacao_Recurso> bs_Fabricacao_Recurso
        {
            get
            {
                if (ViewState["bs_Fabricacao_Recurso"] == null)
                {
                    ViewState["bs_Fabricacao_Recurso"] = new List<cls_WMS_Fabricacao_Recurso>();
                }
                return (List<cls_WMS_Fabricacao_Recurso>)ViewState["bs_Fabricacao_Recurso"];
            }
            set
            {
                ViewState["bs_Fabricacao_Recurso"] = value;
            }
        }

        public List<cls_WMS_Fabricacao_Processo> bs_Fabricacao_Processo
        {
            get
            {
                if (ViewState["bs_Fabricacao_Processo"] == null)
                {
                    ViewState["bs_Fabricacao_Processo"] = new List<cls_WMS_Fabricacao_Processo>();
                }
                return (List<cls_WMS_Fabricacao_Processo>)ViewState["bs_Fabricacao_Processo"];
            }
            set
            {
                ViewState["bs_Fabricacao_Processo"] = value;
            }
        }

        public List<cls_Produtos_Aba_Tabelas> bs_Tabelas
        {
            get
            {
                if (ViewState["bs_Tabelas"] == null)
                {
                    ViewState["bs_Tabelas"] = new List<cls_Produtos_Aba_Tabelas>();
                }
                return (List<cls_Produtos_Aba_Tabelas>)ViewState["bs_Tabelas"];
            }
            set
            {
                ViewState["bs_Tabelas"] = value;
            }
        }

        #endregion

        #region | Classes

        [Serializable]
        public class cls_WMS_Produto_Consumivel
        {
            public int idLinha { get; set; }
            public int idConsumivel { get; set; }
            public int idProduto { get; set; } // Recurso
            public int idProdutoConsumivel { get; set; } // Registro
            public string sCodigo { get; set; }
            public string sDescricao { get; set; }
            public string sUnidade { get; set; }
            public decimal nQtd { get; set; }
            public string sFuncao { get; set; }
        }

        [Serializable]
        public class cls_Flow_Recursos_EPI
        {
            public int idRegistro { get; set; }
            public int idEPI { get; set; }
            public int idItem { get; set; }
            public int idRecurso { get; set; }
            public int nQuantidade { get; set; }

            public int idLinha { get; set; }
            public string sCodigoEPI { get; set; }
            public string sDscEPI { get; set; }
            public string sFuncao { get; set; }
        }

        #endregion

        #endregion

        #region | Page_Load + Pesquisar 

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                manual.Visible = false;
                div_Unidade.Visible = false;
                aba_Clientes.Visible = false;
                div_ExibeComercial_LM.Visible = false;
                div_LMO.Visible = false;
                aba_Fabricacao.Visible = false;
                aba_Lotes.Visible = false;
                aba_Composicao.Visible = false;
                DIV_Gastos.Visible = false;

                DIV_CategoriaPagar.Visible = false;
                div_ServicoMunicipal_NBS_IndOp.Visible = false;

                PopularClasseMotivo();
                EsconderColunas(dtgItens, "Composicao", "ID do Produto");

                FiltroPesquisa.HabilitarItensPesquisa(true);
                FiltroPesquisa1.HabilitarItensPesquisa(true);
                fpFabricacaoRecurso.HabilitarItensPesquisa(true);
                fpFabricacaoInsumoFerramenta.HabilitarItensPesquisa(true);

                fpInstalacao.HabilitarItensPesquisa(true);

                if (Request["id"] != null)
                {
                    if (Request["stp"] == "1")
                    {
                        ValidaPermissao(Permissao.Servicos.Consultar, true);
                        PesquisarServico(Request["id"].ToString(), false);
                        FiltroPesquisa.TipoFiltroPesquisa = hddidTipoFiltroPesquisa.Value;
                        div_ServicoMunicipal_NBS_IndOp.Visible = true;
                        DIV_Sistemas.Visible = false;
                    }
                    else if (Request["stp"] == "2")
                    {
                        ValidaPermissao(Permissao.Produtos_Recursos.Consultar, true);
                        FiltroPesquisa.TipoFiltroPesquisa = "3";
                        PesquisarRecurso(Request["id"].ToString());
                        DIV_Sistemas.Visible = false;
                    }
                    else if (Request["stp"] == "3")
                    {
                        ValidaPermissao(Permissao.Sub_Servicos.Consultar, true);
                        FiltroPesquisa.TipoFiltroPesquisa = "3";
                        PesquisarServico(Request["id"].ToString(), true);
                        DIV_Sistemas.Visible = false;
                    }
                    else
                    {
                        ValidaPermissao(Permissao.Produtos.Consultar, true);
                        FiltroPesquisa.TipoFiltroPesquisa = "1";
                        FiltroPesquisa1.TipoFiltroPesquisa = "1";
                        fpFabricacaoProduto.TipoFiltroPesquisa = "1";
                        fpFabricacaoRecurso.TipoFiltroPesquisa = "3";
                        fpFabricacaoInsumoFerramenta.TipoFiltroPesquisa = "1";
                        fpInstalacao.TipoFiltroPesquisa =  "3";
                        Pesquisar(Request["id"].ToString(), false);
                        DIV_Sistemas.Visible = true;
                    }
                }
                else
                {
                    if (Request["stp"] == "1")
                    {
                        ValidaPermissao(Permissao.Servicos.Incluir, true);
                        PesquisarServico("0", false);
                        DIV_Sistemas.Visible = false;
                    }
                    else if (Request["stp"] == "2")
                    {
                        ValidaPermissao(Permissao.Produtos_Recursos.Incluir, true);
                        PesquisarRecurso("0");
                        DIV_Sistemas.Visible = false;
                    }
                    else if (Request["stp"] == "3")
                    {
                        ValidaPermissao(Permissao.Sub_Servicos.Incluir, true);
                        PesquisarServico("0", true);
                        DIV_Sistemas.Visible = false;
                    }
                    else
                    {
                        ValidaPermissao(Permissao.Produtos.Incluir, true);
                        Pesquisar("0", true);
                        DIV_Sistemas.Visible = true;
                    }
                }

                if (ValidaPermissao(Permissao.Produtos.VisualizarExibeComercial_LMELMO))
                {
                    div_ExibeComercial_LM.Visible = true;
                    div_LMO.Visible = true;
                }
                FiltroPesquisa.HabilitarItemQtde(true);
                FiltroPesquisa1.HabilitarItemQtde(true);
                fpFabricacaoProduto.HabilitarItemQtde(true);
                fpFabricacaoRecurso.HabilitarItemQtde(true);
                fpFabricacaoInsumoFerramenta.HabilitarItemQtde(true);
                Pesquisa_Parceiros.RegistrarScriptPesquisarItens();

                if (ValidaPermissao(Permissao.Produtos.BloquearEdicao))
                {
                    hddsPermissaoCadeado.Value = "1";
                }

                //bool isLocked = GetEstadoCadeado();
                //lockButton.Attributes["data-locked"] = isLocked.ToString().ToLower();lblcmdComposicao_IncluirItem

                // -----------------------------------
                // Gabriel Llanir 01/08/2024
                // ---------
                // GAMBIARRA - favor manter distância
                // ---------
                // O seguinte Script é responsável por executar um Click em um botão invisível da página para que execute um PostBack, desta forma forçando os Scripts dos Controles FiltroPesquisa a funcionarem corretamente

                ScriptManager.RegisterStartupScript(Page, GetType(), "GAMBIARRA_" + Guid.NewGuid(), string.Format("\r\n\r\n$(document).ready(function(){0});\r\n\r\n", "{ \r\nvar btn = $('#" + cmdGAMBIARRA.ClientID + "'); \r\nif (btn.length > 0) { \r\nconsole.log('existe'); \r\nbtn.click(); \r\n} \r\n}"), true);

                // -----------------------------------
            }

            Pesquisa_Parceiros.SsTipoParceiro = "0";

            manual.sNomeArquivo = hddsManual.Value;
            txtidProduto.Text = hddidProduto.Value != "0" ? Request["duplicar"] != "true" ? hddidProduto.Value : "Duplicado" : "Novo";

            FiltroPesquisa2.ModificaTamanhoCampos(2, 4, 2, 2, 0);
            FiltroPesquisa2.Controle_ExibicaoCampos(new Dictionary<string, bool> { { "Valor Unitário", false } }, "1");
            FiltroPesquisa2.desligaColapso(false);
            FiltroPesquisa2.RegistrarScriptPesquisarMovimentacao();

            AtualizarGrid(); //Agnes Partal * 19/08/2024
            RegistraScript();
            RegistraScript_Personalizado(txtCodigo_EPI.ClientID, true);
            RegistraScript_Personalizado(txtDesc_EPI.ClientID, false);
        }

        protected void Pesquisar(string idProduto, bool bEdicao)
        {
            aba_Clientes.Visible = true;
            aba_Arquivos.Visible = false;
            aba_caracteristicasProduto.Visible = false;
            aba_Composicao.Visible = false;
            imgProdutoPrincipal.Visible = false;
            divComboAtivo2.Visible = false;
            aba_Fabricacao.Visible = false;
            div_ServicoMunicipal_NBS_IndOp.Visible = false;

            string sComposicao = "";

            PopularCombos(0);

            string sErro = "";
            try
            {
                LimpaCampos();

                if (idProduto != "0")
                {
                    aba_Tabelas.Visible = ValidaPermissao(Permissao.Produtos.ConsultarAbaTabelas) || ValidaPermissao(Permissao.Produtos.EditarAbaTabelas);
                    div_lstAdicionarTabela.Visible = ValidaPermissao(Permissao.Produtos.EditarAbaTabelas);
                    pnCustoFornecedor.Visible = true;
                    pnCustoTT.Visible = true;
                    pnIndustrializacaoTT.Visible = true;
                    pnVendasPVP.Visible = true;
                    pnCustoEmpreitada.Visible = true;
                    pnVendasLPU.Visible = true;
                    pnCustoRecursos.Visible = false;
                    aba_Lotes.Visible = true;

                    Dictionary<String, String> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idProduto", idProduto }
                    };

                    DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

                    if (ValidarDataSet(dsPesquisa, out sErro))
                    {
                        aba_Idiomas.Visible = true;
                        string sTipo = DATASET(dsPesquisa, 0, 0, "sValidaTipo");

                        if (sTipo == "0")
                        {
                            if (aba_Tabelas.Visible)
                                Popular_Aba_Tabelas(idProduto, false);

                            if (dsPesquisa.Tables[12].Rows.Count > 0)
                            {
                                foreach (DataRow linha in dsPesquisa.Tables[12].Rows)
                                {
                                    ListItem item = ddlsSistemas.Items.FindByValue(linha["idSistema"].ToString());
                                    if (item != null)
                                        item.Selected = true;
                                }
                            }

                            aba_Arquivos.Visible = true;
                            cmdSalvar.Visible = true;
                            divEtiquetas.Visible = true;
                            divEAN.Visible = true;
                            PopularImpressora();

                            hddidProduto.Value = DATASET(dsPesquisa, 0, "idProduto");
                            txtidProduto.Text = DATASET(dsPesquisa, 0, "idProduto");
                            ddlTipoProduto.SelectedValue = DATASET(dsPesquisa, 0, "idTipoProduto");
                            ddlGrupo.SelectedValue = DATASET(dsPesquisa, 0, "idGrupo");
                            ddlFamilia.SelectedValue = DATASET(dsPesquisa, 0, "idFamilia");
                            ddlArmazenagem.SelectedValue = DATASET(dsPesquisa, 0, "idLocalArmazenamento");
                            txtsCodigo.Text = DATASET(dsPesquisa, 0, "sCodigo");
                            txtsDscProduto.Text = DATASET(dsPesquisa, 0, "sDscProduto");
                            ddlUnidade.SelectedValue = DATASET(dsPesquisa, 0, "sUnidade");

                            //Thiago Rodrigues - 03/02/2026
                            ddlLocalOPI.SelectedValue = DATASET(dsPesquisa, "idLocalOPI");
                            ddlTipoEtiqueta.SelectedValue = DATASET(dsPesquisa, "sTipoEtiqueta");

                            txtnComprimento.Text = ArredondarValor(DATASET(dsPesquisa, 0, "nComprimento"));
                            txtnLargura.Text = ArredondarValor(DATASET(dsPesquisa, 0, "nLargura"));
                            txtnAltura.Text = ArredondarValor(DATASET(dsPesquisa, 0, "nAltura"));
                            txtnVolume.Text = DATASET(dsPesquisa, 0, "nVolume");

                            txtnCubagem.Text = DATASET(dsPesquisa, 0, "nCubagem");
                            txtnPesoNeto.Text = DATASET(dsPesquisa, 0, "nPesoNeto");
                            txtnPesoBruto.Text = DATASET(dsPesquisa, 0, "nPesoBruto");
                            txtnEstoqueMinimo.Text = DATASET(dsPesquisa, 0, "nEstoqueMinimo");
                            txtnEstoqueAtual.Text = DATASET(dsPesquisa, 0, "nEstoqueAtual");
                            hddnQtdSeries.Value = txtnEstoqueAtual.Text;
                            txtsFabricante.Text = DATASET(dsPesquisa, 0, "sFabricante");
                            txtnEstoqueReservado.Text = DATASET(dsPesquisa, 0, "nEstoqueReservado");
                            txtnUMZ_Ratio.Text = DATASET(dsPesquisa, 0, "nUMZ_Ratio");
                            ddlPaisOrigem.SelectedValue = DATASET(dsPesquisa, 0, "idPais");
                            ddlUnidadeEntrega.SelectedValue = DATASET(dsPesquisa, 0, "sUnidadeEntrega");
                            txtsCodigoEAN.Text = DATASET(dsPesquisa, 0, "sCodigoEAN");

                            Popula_Combo(lstParceiros, "sp_Select 'Flow_Parceiros_Fornecedores', @idFiltro=" + (ddlPaisOrigem.SelectedValue == "1" ? "1" : ddlPaisOrigem.SelectedValue == "6" ? "3" : "2"), "idCliente", "sCPNJ_RazaoSocial", false);

                            ddlIbama.SelectedValue = DATASET(dsPesquisa, 0, "sIbama");
                            ddlidCategoriaVendas.SelectedValue = DATASET(dsPesquisa, 0, "idCategoriaVendas");

                            //Controle de Exibição GetProdutos
                            ddlExibeComercial.SelectedValue = DATASET(dsPesquisa, 0, "sExibeComercial");
                            ddlExibeLM.SelectedValue = DATASET(dsPesquisa, 0, "sExibeLM");



                            txtsUnMinimaCompra.Text = DATASET(dsPesquisa, 0, "sUnMinimaCompra");
                            object objSender = new object();
                            EventArgs objEventArgs = new EventArgs();
                            ddlCEST.SelectedValue = DATASET(dsPesquisa, 0, "idCEST");
                            ddlCEST_SelectedIndexChanged(objSender, objEventArgs);


                            ddlNCM.SelectedValue = DATASET(dsPesquisa, 0, "idNCM");
                            ddlNCM_SelectedIndexChanged(objSender, objEventArgs);

                            //acessoDados("CEST", ddlCEST.SelectedValue, false);
                            //acessoDados("NCM", ddlNCM.SelectedValue, false);

                            string sSugestao = DATASET(dsPesquisa, 0, "sSugestao");
                            string sFornecedores = DATASET(dsPesquisa, 0, "sFornecedores");
                            sComposicao = DATASET(dsPesquisa, 0, "sComposicao");

                            hddsCadeado.Value = DATASET(dsPesquisa, 0, "sCadeado");               //Agnes Partal * 22/10/2024


                            PainelAtualizacao.Visible = true;
                            PainelAtualizacao.Atualizar(DATASET(dsPesquisa, 0, "dtAtualizacao"), DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                            ComboAtivo.Situacao_Definir(DATASET(dsPesquisa, 0, "sSituacao"));
                            lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscProduto.Text);
                            aba_caracteristicasProduto.Visible = true;

                            cmdSalvar.Visible = ValidaPermissao(Permissao.Produtos.Alterar);

                            fpFabricacaoRecurso.AlteraCampos_x_Tipo(0, false, false);
                            fpInstalacao.AlteraCampos_x_Tipo(1, false, false);

                            Dictionary<string, bool> vParametrosFiltroPesquisaVisualizacao = new Dictionary<string, bool>
                            {
                                { "Valor Unitário", false }
                            };

                            FiltroPesquisa.Controle_ExibicaoCampos(vParametrosFiltroPesquisaVisualizacao, "1");
                            FiltroPesquisa1.Controle_ExibicaoCampos(vParametrosFiltroPesquisaVisualizacao, "1");
                            fpFabricacaoProduto.Controle_ExibicaoCampos(vParametrosFiltroPesquisaVisualizacao, "1");
                            fpFabricacaoRecurso.Controle_ExibicaoCampos(vParametrosFiltroPesquisaVisualizacao, "3");
                            fpFabricacaoInsumoFerramenta.Controle_ExibicaoCampos(vParametrosFiltroPesquisaVisualizacao, "1");
                            fpFabricacaoProduto.desligaColapso(false);
                            fpFabricacaoInsumoFerramenta.desligaColapso(false);

                            fpInstalacao.Controle_ExibicaoCampos(vParametrosFiltroPesquisaVisualizacao, "3");

                            //Thiago Rodrigues 22/11/2024
                            SwitchAtivoGarantia.Definir(DATASET(dsPesquisa, 0, "sControlaGarantiaLote"), "Garantia/Lote", "");

                            SwitchAtivoSerivalizavel.Definir(DATASET(dsPesquisa, 0, "sSerializavel"), "É Serializavel?", "");

                            nTempoGarantia.Text = DATASET(dsPesquisa, 0, "nTempoGarantia");

                            carregaImgPrincipal(idProduto);
                            Popular_Produtos_Descricao(dsPesquisa);

                            Popular_dtgItens(dsPesquisa);
                            cmdComposicao_Alterar.Visible = false;
                            
							// Aba Instalação/Obra: só aqui, no ramo sValidaTipo = 0 (Produto).
                            // Serviço, Sub-Serviço e Recurso passam por PesquisarServico /
                            // PesquisarRecurso e nunca chegam neste ponto.
                            Popular_Aba_Instalacao(idProduto);
							
                            if (Convert.ToBoolean(Request["duplicar"]))
                            {
                                txtidProduto.Text = "";
                                aba_Movimentacao.Visible = false;
                                aba_Sugestao.Visible = false;
                                LimparCamposDuplicando();
                            }
                            if (sSugestao == "S")
                            {
                                Popular_dtgSugestao(dsPesquisa);
                            }
                            if (sFornecedores == "S")
                            {
                                Popular_dtgFornecedores(dsPesquisa);
                            }

                            if (!bEdicao)
                            {
                                Popular_Aba_Arquivos(idProduto);
                            }

                            string sSistema = DATASET(dsPesquisa, "sSistema");

                            if (string.IsNullOrEmpty(sSistema) || sSistema.Equals("N"))
                            {
                                //aba_Idiomas.Visible = true;
                                aba_Fornecedores.Visible = true;
                                aba_Movimentacao.Visible = true;
                                aba_Sugestao.Visible = true;
                                aba_Composicao.Visible = true;
                                aba_caracteristicasProduto.Visible = true;
                            }
                            else
                            {
                                //aba_Idiomas.Visible = false;
                                aba_Fornecedores.Visible = false;
                                aba_Movimentacao.Visible = false;
                                aba_Sugestao.Visible = false;
                                aba_Composicao.Visible = false;
                                aba_caracteristicasProduto.Visible = false;
                            }

                            string sIndustrializado = DATASET(dsPesquisa, "sIndustrializado");

                            if (string.IsNullOrEmpty(sIndustrializado) || sIndustrializado.Equals("S"))
                            {
                                Popular_Aba_Fabricacao(dsPesquisa);
                            }

                            Popular_Aba_Documentacao(dsPesquisa, 6);
                            Popular_Aba_Parceiros(dsPesquisa.Tables[7]);
                            Popular_Aba_Lotes(idProduto);
                            Popular_Aba_Historico(dsPesquisa.Tables[8]);
                        }
                        else
                        {
                            divEAN.Visible = false;
                            divEtiquetas.Visible = false;
                            DirecionaPagina(string.Format("App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}&stp={1}", idProduto, sTipo));
                        }
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }
                }
                else
                {
                    aba_Clientes.Visible = false;
                    aba_Movimentacao.Visible = false;
                    aba_Idiomas.Visible = false;
                    aba_Composicao.Visible = false;
                    aba_Sugestao.Visible = false;
                    aba_Fornecedores.Visible = false;
                    aba_Documentacao.Visible = false;
                    aba_caracteristicasProduto.Visible = true;
                    aba_Historico.Visible = false;
                    aba_Fabricacao.Visible = false;
                    aba_Tabelas.Visible = false;

                    divEtiquetas.Visible = false;

                    if (Request["stp"] == "0" || string.IsNullOrEmpty(Request["stp"]))
                    {
                        divEAN.Visible = true;
                    }

                    BreadCrumb.TitulodaPagina = "Incluir";
                    lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
                    cmdSalvar.Text = "Incluir";
                    //lblDescricaoFornecedor.ToolTip = "Insira a descrição que aparece na nota fiscal do fornecedor!";
                    ddlExibeComercial.SelectedValue = "N";
                    ddlExibeLM.SelectedValue = "N";
                }

                txtsCodigo.Focus();
                RegistraScript();

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message +
                 " <a href='#' onclick='retornar();'>Retornar</a> | <a href='#' onclick='avancar();'>Avançar</a>");
                if (ex.Message == "Nenhum Registro Encontrado")
                {
                    div_Cabecalho.Visible = false;
                    aba_Arquivos.Visible = false;
                    aba_caracteristicasProduto.Visible = false;
                    aba_Composicao.Visible = false;
                    aba_Idiomas.Visible = false;
                    aba_Fornecedores.Visible = false;
                    aba_Movimentacao.Visible = false;
                    aba_Sugestao.Visible = false;
                    cmdSalvar.Visible = false;
                    aba_Historico.Visible = false;
                    aba_Fabricacao.Visible = false;
                }
            }

        }

        #endregion

        #region | Popula

        void Popular_Aba_Arquivos(string idPedido)
        {
            aba_Arquivos.Visible = false;

            if (ValidaPermissao(Permissao.Produtos.ConsultarAbaArquivos))
            {
                frmArquivos.Attributes.Add("src", string.Format("../Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", idPedido, "Produtos"));
                aba_Arquivos.Visible = true;
            }
        }

        void Popular_Aba_Tabelas(string idItem, bool bRecurso)
        {
            // <-----------------------------------------------> \\

            bool bEditar = ValidaPermissao(Permissao.Produtos.EditarAbaTabelas);

            gvTabelas_CustoFornecedor_Nacional.Columns[gvTabelas_CustoFornecedor_Nacional.Columns.Count - 1].Visible = bEditar;
            gvTabelas_CustoFornecedor_Internacional.Columns[gvTabelas_CustoFornecedor_Internacional.Columns.Count - 1].Visible = bEditar;
            gvTabelas_CustoTT_Nacional.Columns[gvTabelas_CustoTT_Nacional.Columns.Count - 1].Visible = bEditar;
            gvTabelas_CustoTT_Internacional.Columns[gvTabelas_CustoTT_Internacional.Columns.Count - 1].Visible = bEditar;
            gvTabelas_IndustrializaçãoTT_Nacional.Columns[gvTabelas_IndustrializaçãoTT_Nacional.Columns.Count - 1].Visible = bEditar;
            gvTabelas_IndustrializaçãoTT_Internacional.Columns[gvTabelas_IndustrializaçãoTT_Internacional.Columns.Count - 1].Visible = bEditar;
            gvTabelas_VendasPVP.Columns[gvTabelas_VendasPVP.Columns.Count - 1].Visible = bEditar;
            gvTabelas_VendasCustomizadas.Columns[gvTabelas_VendasCustomizadas.Columns.Count - 1].Visible = bEditar;
            gvTabelas_CustoEmpreitada.Columns[gvTabelas_CustoEmpreitada.Columns.Count - 1].Visible = bEditar;
            gvTabelas_VendasLPU.Columns[gvTabelas_VendasLPU.Columns.Count - 1].Visible = bEditar;
            gvTabelas_CustoRecursos.Columns[gvTabelas_CustoRecursos.Columns.Count - 1].Visible = bEditar;

            // <-----------------------------------------------> \\

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "ABA_TABELAS" },
                { "idItem", idItem }
            };
            DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParametros);

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                int.TryParse(row["idTabela"].ToString(), out int idTabela);
                int.TryParse(row["idTipoTabela"].ToString(), out int idTipoTabela);
                int.TryParse(row["idMoedaOrigem"].ToString(), out int idMoedaOrigem);
                decimal.TryParse(row["nTaxaCambio"].ToString(), out decimal nCambio);
                decimal.TryParse(row["nPreco"].ToString(), out decimal nPreco);
                decimal.TryParse(row["nPreco_Zona_SD"].ToString(), out decimal nPreco_Zona_SD);
                decimal.TryParse(row["nPreco_Zona_ND"].ToString(), out decimal nPreco_Zona_ND);
                decimal.TryParse(row["nPreco_Zona_N"].ToString(), out decimal nPreco_Zona_N);
                decimal.TryParse(row["nPreco_Zona_CO"].ToString(), out decimal nPreco_Zona_CO);
                decimal.TryParse(row["nPreco_Zona_S"].ToString(), out decimal nPreco_Zona_S);
                decimal.TryParse(row["nFator"].ToString(), out decimal nFator);
                decimal.TryParse(row["nMargem"].ToString(), out decimal nMargem);
                decimal.TryParse(row["nTaxaEnvio"].ToString(), out decimal nTaxaEnvio);
                decimal.TryParse(row["nTaxaLocal"].ToString(), out decimal nTaxaLocal);
                decimal.TryParse(row["nTaxaImpostos"].ToString(), out decimal nTaxaImpostos);
                decimal.TryParse(row["nIPI"].ToString(), out decimal nIPI);
                decimal.TryParse(row["nTotal"].ToString(), out decimal nTotal);

                string sDscTabela = row["sDscTabela"].ToString();
                string sObservacao = row["sObservacao"].ToString();
                string sDscTipoTabela = row["sDscTipoTabela"].ToString();
                string sSimbolo_MoedaOrigem = row["sSimbolo_MoedaOrigem"].ToString();
                string sSimbolo_MoedaDestino = row["sSimbolo_MoedaDestino"].ToString();

                bool bProdutoIncluso = row["sProdutoIncluso"].ToString().Equals("S");
                bool bIndustrializado = DATASET(ds, "sIndustrializado").Equals("S");

                cls_Produtos_Aba_Tabelas atividade = new cls_Produtos_Aba_Tabelas
                {
                    idTabela = idTabela,
                    sDscTabela = sDscTabela,
                    sObservacao = sObservacao,
                    idTipoTabela = idTipoTabela,
                    sDscTipoTabela = sDscTipoTabela,
                    idMoedaOrigem = idMoedaOrigem,
                    sSimbolo_MoedaOrigem = sSimbolo_MoedaOrigem,
                    sSimbolo_MoedaDestino = sSimbolo_MoedaDestino,
                    nCambio = Math.Round(nCambio, 4),
                    nPreco = Math.Round(nPreco, 2),
                    nPreco_Zona_SD = Math.Round(nPreco_Zona_SD, 2),
                    nPreco_Zona_ND = Math.Round(nPreco_Zona_ND, 2),
                    nPreco_Zona_N = Math.Round(nPreco_Zona_N, 2),
                    nPreco_Zona_CO = Math.Round(nPreco_Zona_CO, 2),
                    nPreco_Zona_S = Math.Round(nPreco_Zona_S, 2),
                    nFator = Math.Round(nFator, 2),
                    nMargem = Math.Round(nMargem, 2),
                    nTaxaEnvio = Math.Round(nTaxaEnvio, 2),
                    nTaxaLocal = Math.Round(nTaxaLocal, 2),
                    nTaxaImpostos = Math.Round(idMoedaOrigem != 2 ? nTaxaImpostos : nIPI, 2),
                    nTotal = Math.Round(nTotal, 2),

                    bLiberado = row["sLiberado"].ToString().Equals("S"),
                    bProdutoIncluso = bProdutoIncluso,
                    bIndustrializado = bIndustrializado,
                    bRecurso = bRecurso
                };

                AbaTabelas_ListBox(idTabela, sDscTabela, idTipoTabela, sDscTipoTabela, bProdutoIncluso, bRecurso, bIndustrializado);

                bs_Tabelas.Add(atividade);
            }

            AbaTabelas_DataBind();
        }

        void PopularCombos(int tipo)
        {
            if (tipo == 0)
            {
                Popula_Combo(ddlUnidade, "sp_Select 'Flow_Produtos_Unidade', @sPesquisa='N'", "sUnidade", "sDscUnidade", false, "Selecione", "0");
                Popula_Combo(ddlUnidadeEntrega, "sp_Select 'Flow_Produtos_Unidade', @sPesquisa='N'", "sUnidade", "sDscUnidade", false, "Selecione", "0");
                Popula_Combo(ddlsUnidade, "sp_Select 'Flow_Produtos_Unidade', @sPesquisa='N'", "sUnidade", "sDscUnidade", false, "Selecione", "0");
                Popula_Combo(ddlLocalOPI, $"sp_Manipula_tbl_Flow_WMS_LocalArmazenamento 'FLOW-ARMAZENAMENTO-PRODUTO'", "idLocalArmazenamento", "sDscLocalArmazenamento", false, "Sem Local.", "0");
            }
            if (tipo > 0)
            {
                Popula_Combo(ddlUnidade, "sp_Select 'Flow_Produtos_Unidade', @sPesquisa='S'", "sUnidade", "sDscUnidade", false, "Selecione", "0");
                Popula_Combo(ddlsUnidade, "sp_Select 'Flow_Produtos_Unidade', @sPesquisa='S'", "sUnidade", "sDscUnidade", false, "Selecione", "0");
                Popula_Combo(ddlCategoriapagar, "sp_Select 'Flow_Adm_Contas_Pagar_Categoria_Pai'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Selecione Categoria Contas Pagar", "0");
                Popula_Combo(ddlsCodigoServicoPai, "sp_Select 'Flow_Codigo_Servico_Pai'", "idCodigoServicoPai", "sDscServicoPai", false, "Selecione o Código do Serviço Pai", "0");

                ddlidServicoMunicipal.Popula_Combo("sp_Select 'tbl_Flow_Servico_Municipal'", "idServico", "sDscServico", false, "Selecione Serviço Municipal", "0");
                ddlNBS.Popula_Combo("sp_Select2 'Flow_Adm_NBS'", "idNBS", "sDescricao", false, "Selecione um NBS", "0");
                ddlIndOp.Popula_Combo("sp_Select2 'Flow_Adm_IndicadorOperacao'", "idIndOp", "sDescricao", false, "Selecione um Indicador de Operação", "0");
            }

            Popula_Combo(ddlTipoProduto, "sp_Select 'Flow_Produtos_Tipo', @idPesquisa=" + tipo, "idTipoProduto", "sDscTipoProduto", false, "Selecione um tipo", "0");
            Popula_Combo(ddlGrupo, "sp_Select 'Flow_WMS_Produtos_Grupos_PAI', @idPesquisa=" + tipo, "idGrupo", "sDscGrupo", false, "Selecione um grupo", "0");
            Popula_Combo(ddlFamilia, "sp_Select 'Flow_WMS_Produtos_Familia'", "idFamilia", "sDscFamilia", false, "Selecione a Família", "0");
            Popula_Combo(ddlArmazenagem, "sp_Select 'Flow_WMS_Produtos_Local_Armazenamento'", "idLocalArmazenamento", "sDscLocalArmazenamento", false, "Selecione um local", "0");
            Popula_Combo(ddlPaisOrigem, "sp_Select 'tbl_Flow_WMS_Produtos_Origem'", "idPais", "sDscPais", false, "Selecione o País de Origem", "0");
            Popula_Combo(ddlidCategoriaVendas, "sp_Select 'tbl_Flow_Comercial_CategoriaVendas'", "idCategoriaVendas", "sDscCategoriaVendas", false, "Selecione uma categoria", "0");
            Popula_Combo(ddliTipoAlteracao, "sp_Select 'Flow_WMS_Produtos_Movimentacao_Tipo'", "idTipoMovimentacao", "sDscTipoMovimentacao", false, "Todos os Tipos de Movimentação", "0");
            Popula_Combo(ddlMotivo, "sp_Select 'tbl_Flow_WMS_Produtos_Movimentacao_Motivo'", "idMotivo", "sDscMotivo", false, "Todos os  tipo de Mov.", "0");

            Popula_Combo(ddlidPais, "sp_Select 'tbl_Flow_WMS_Produtos_Origem_Sigla'", "idPais", "sDscPais", false, "Selecione um Pais", "0");
            Popula_Combo(ddlDescricao_IdTipo, "sp_Select 'tbl_Flow_WMS_Produtos_Descricao_Tipo'", "idTipo", "sDscTipo", false, "Selecione um Tipo", "0");

            Popula_Combo(ddlsUnidadeRecurso, "sp_Select 'Flow_Produtos_Unidade', @sPesquisa='S'", "sUnidade", "sDscUnidade", false, "Selecione", "0");

            Popula_Combo(ddlsSistemas, "sp_Select 'tbl_Flow_Sistemas'", "idSistema", "sDscSistema", false, "Selecione um Sistema", "0");

            SqlDataReader dr = ExecutarDataReader("sp_Select 'Flow_WMS_Produtos_CEST'");
            if (dr != null)
            {
                BS_CEST.Clear();
                while (dr.Read())
                {
                    cls_WMS_CEST objItem = new cls_WMS_CEST
                    {
                        idCest = Convert.ToInt32(dr["idCest"].ToString()),
                        sCodigoCEST = dr["sCodigoCEST"].ToString(),
                        sDscCEST = dr["sDscCEST"].ToString()
                    };
                    BS_CEST.Add(objItem);
                }
                dr.Close();

                PopularCombo_CEST();
            }

            dr = ExecutarDataReader("sp_Select 'Flow_WMS_Produtos_NCM'");
            if (dr != null)
            {
                BS_NCM.Clear();

                while (dr.Read())
                {
                    cls_WMS_NCM objItem = new cls_WMS_NCM
                    {
                        idNCM = Convert.ToInt32(dr["idNCM"].ToString()),
                        idCEST = Convert.ToInt32(dr["idCEST"].ToString()),
                        sCodigoNCM = dr["sCodigoNCM"].ToString(),
                        sDscNCM = dr["sDscNCM"].ToString()
                    };
                    BS_NCM.Add(objItem);
                }
                dr.Close();

                PopularCombo_NCM("0");
            }
        }

        void PopularCombo_CEST()
        {
            ddlCEST.Items.Clear();
            ddlCEST.Items.Add(new System.Web.UI.WebControls.ListItem("Selecione o CEST", "0"));

            for (int i = 0; i < BS_CEST.Count; i++)
            {
                ddlCEST.Items.Add(new System.Web.UI.WebControls.ListItem(BS_CEST[i].sDscCEST, BS_CEST[i].idCest.ToString()));
            }
            txtCodigoCEST.Text = "";
        }

        void PopularCombo_NCM(string idCEST)
        {
            List<cls_WMS_NCM> BS_NCM_FILTRO;
            if (idCEST == "0")
            {
                BS_NCM_FILTRO = BS_NCM.ToList();
            }
            else
            {
                BS_NCM_FILTRO = BS_NCM.Where(c => c.idCEST.ToString().Equals(idCEST)).ToList();
            }

            ddlNCM.Items.Clear();
            ddlNCM.Items.Add(new System.Web.UI.WebControls.ListItem("Selecione o NCM", "0"));

            for (int i = 0; i < BS_NCM_FILTRO.Count; i++)
            {
                ddlNCM.Items.Add(new System.Web.UI.WebControls.ListItem(BS_NCM_FILTRO[i].sDscNCM, BS_NCM_FILTRO[i].idNCM.ToString()));
            }
            txtCodigoNCM.Text = "";
        }

        void Popular_dtgItens(DataSet dsPesquisa)

        {
            //dtgItens
            bs_Produto_Composicao.Clear();
            if (ValidaPermissao(Permissao.Produtos.ConsultarAbaComopsicao))
            {
                foreach (DataRow row in dsPesquisa.Tables[1].Rows)
                {
                    cls_WMS_Produtos objItem = new cls_WMS_Produtos();
                    objItem.IdItem = Convert.ToInt32(row["idProduto"].ToString());
                    objItem.IdItemComposicao = Convert.ToInt32(row["idItemComposicao"].ToString());
                    objItem.SCodigo = row["sCodigo"].ToString();
                    objItem.SDscProduto = row["sDscProduto"].ToString();
                    objItem.TipoProduto = row["sDscTipoProduto"].ToString();
                    objItem.NQuantidade = ConverterStringDecimal(row["nQuantidade"].ToString());
                    objItem.SUnidade = row["sUnidade"].ToString();
                    objItem.sExibePedido = row["sExibePedido"].ToString();
                    objItem.nOrdem = Convert.ToInt32(row["nOrdem"]);

                    bs_Produto_Composicao.Add(objItem);
                }

                aba_Composicao.Visible = true;
            }
            dtgItens_DataBind();
        }

        void Popular_dtgSugestao(DataSet dsPesquisa)
        {
            BS_SUGESTAO.Clear();

            aba_Sugestao.Visible = false;

            if (ValidaPermissao(Permissao.Produtos.ConsultarAbaSugestao))
            {
                foreach (DataRow row in dsPesquisa.Tables[2].Rows)
                {
                    cls_WMS_Produtos_Sugestao objItem = new cls_WMS_Produtos_Sugestao();
                    objItem.IdProdutoSugestao = Convert.ToInt32(row["idProdutoSugestao"].ToString());
                    objItem.SCodigo = row["sCodigo"].ToString();
                    objItem.SDscProduto = row["sDscProduto"].ToString();
                    objItem.TipoProduto = row["sDscTipoProduto"].ToString();
                    objItem.SUnidade = row["sUnidade"].ToString();
                    objItem.NQuantidade = ConverterStringDecimal(row["nQuantidade"].ToString());

                    BS_SUGESTAO.Add(objItem);

                }

                aba_Sugestao.Visible = true;
            }
            dtgSugestao_Databind();
        }

        void Popular_dtgFornecedores(DataSet dsPesquisa)
        {

            aba_Fornecedores.Visible = false;

            if (ValidaPermissao(Permissao.Produtos.ConsultarAbaFornecedores))
            {
                foreach (DataRow row in dsPesquisa.Tables[3].Rows)
                {
                    cls_WMS_Produtos_Fornecedores objItem = new cls_WMS_Produtos_Fornecedores();
                    objItem.IdProduto = Convert.ToInt32(row["idProduto"].ToString());
                    objItem.sCNPJ_CPF = row["sCnpj_CPF"].ToString();
                    objItem.SDscFornecedorProduto = row["sDscFornecedorProduto"].ToString();
                    objItem.IdParceiro = Convert.ToInt32(row["idParceiro"].ToString());
                    objItem.IdTipoFornecedor = Convert.ToInt32(row["idTipoFornecedor"].ToString());
                    objItem.SCodigoFornecedor = row["sCodigoFornecedor"].ToString();
                    objItem.NvalorUnitario = ConverterStringDecimal(row["nValorUnitario"].ToString());
                    objItem.SRazaoSocial = row["sRazaoSocial"].ToString();
                    objItem.STipoFornecedor = row["sTipoFornecedor"].ToString();

                    BS_FORNECEDORES.Add(objItem);
                }

                aba_Fornecedores.Visible = true;
            }
            dtgFornecedores_Databind();

        }

        void PopularHistoricoMovimentacao(string idProduto)
        {
            try
            {


                string sErro = "";
                DataSet ds = new DataSet();
                string sProcedure_Movimentacao = "sp_Manipula_tbl_Flow_Produtos_Movimentacao";
                Dictionary<string, string> vParametrosItens = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_MOVIMENTACAO_PRODUTO_SALDO" },
                    { "@idProduto", idProduto },
   //                 { "@nPeriodo", ddlPeriodo.SelectedValue.ToString() },
                    { "@idTipoMovimentacao", ddliTipoAlteracao.SelectedValue.ToString() },
                    { "@idMotivo", ddlMotivo.SelectedValue.ToString() },
                    { "@idStatus", "0" }
                };
                ds = ExecutarDataSet(sProcedure_Movimentacao, vParametrosItens);

                if (ValidarDataSet(ds, out sErro))
                {
                    aba_Movimentacao.Visible = false;

                    if (ValidaPermissao(Permissao.Produtos.ConsultarAbaMovimentacao))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", Grid.DataBindComScript(dtgMovimentacaoItens, ds, 0, "desc"), true);
                        aba_Movimentacao.Visible = true;
                        div_Movimentacao.Visible = true;
                    }
                }
                else if (idProduto == "0")
                    aba_Movimentacao.Visible = false;
                else if (sErro != "0")
                {
                    MensagemPaginaMovimentacao.MostraMensagem_Erro(sErro);
                    div_Movimentacao.Visible = false;
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        void PopularClasseMotivo()
        {
            string sErro = "";
            DataSet dsMotivo = cls_WMS_MovimentacaoRel.Movimentacao_Consultar_Motivo_x_Tipo();
            if (ValidarDataSet(dsMotivo, out sErro))
            {
                cls_WMS_MovimentacaoRel.Movimentacao_ConverterDS_BD(dsMotivo, BS_MOTIVO);
            }
        }

        void PopularCombo_Motivo(string idTipoMovimentacao)
        {
            List<cls_WMS_Movimentacao_Motivo> BS_MOTIVO_FILTRO;
            if (idTipoMovimentacao == "0")
            {
                BS_MOTIVO_FILTRO = BS_MOTIVO.ToList();
            }
            else
            {
                BS_MOTIVO_FILTRO = BS_MOTIVO.Where(c => c.IdTipoMovimentacao.ToString().Equals(idTipoMovimentacao)).ToList();
            }

            ddlMotivo.Items.Clear();
            ddlMotivo.Items.Add(new System.Web.UI.WebControls.ListItem("Selecione o tipo Movimentacao", "0"));

            for (int i = 0; i < BS_MOTIVO_FILTRO.Count; i++)
            {
                ddlMotivo.Items.Add(new System.Web.UI.WebControls.ListItem(BS_MOTIVO_FILTRO[i].SDscMotivo, BS_MOTIVO_FILTRO[i].IdMotivo.ToString()));
            }
        }

        void carregaImgPrincipal(string idObjeto)
        {
            DataTable dsPesquisa;
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_IMAGEM" },
                { "@idTipoArquivo", "201" },
                { "@idObjeto", idObjeto }
            };
            dsPesquisa = ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (dsPesquisa.Rows.Count > 0)
            {
                DataRow imgBd = dsPesquisa.Rows[0];
                byte[] valorImgBd = (byte[])imgBd["vbArquivo"];
                string imgUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])imgBd["vbArquivo"]);
                imgProdutoPrincipal.ImageUrl = imgUrl;
                imgProdutoPrincipal.Visible = true;
            }

        }

        void Popular_Aba_Historico(DataTable dt)
        {
            aba_Historico.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", Grid.DataBindComScriptData(gv_Historico, dt, 0, "desc", "false", "''"), true);
        }

        #endregion

        #region | Limpar Campos

        void LimparCamposDuplicando()
        {
            // Limpa os campos na aba composição
            foreach (GridViewRow row in dtgItens.Rows)
            {
                // Encontra os TextBoxes relevantes na linha atual
                TextBox txtnQuantidade = row.FindControl("txtnQuantidade") as TextBox;

                // Verifica se o TextBox foi encontrado e define o valor -1
                if (txtnQuantidade != null)
                {
                    txtnQuantidade.Text = "-1";
                }
            }
            txtsDscDescricao.Text = txtsDscDescricao.Text + " - Duplicado";
            txtsCodigo.Text = txtsCodigo.Text + " - Duplicado";
            //SwitchAtivoGarantia.Definir("N", "Controla Garantia/Lote", "");

            BS_SUGESTAO.Clear();
        }

        void LimpaCampos()
        {
            txtidProduto.Text = "Novo";

            txtCodigoNCM.Text = string.Empty;
            txtCodigoCEST.Text = string.Empty; ;
            txtnComprimento.Text = string.Empty;
            txtnVolume.Text = string.Empty;
            txtnLargura.Text = string.Empty;
            txtnAltura.Text = string.Empty;
            txtnPesoNeto.Text = string.Empty;
            txtnPesoBruto.Text = string.Empty;
            txtnUMZ_Ratio.Text = string.Empty;
            txtnCubagem.Text = string.Empty;
            txtnEstoqueReservado.Text = string.Empty;
            txtsCodigo.Text = "";
            txtsDscProduto.Text = "";
            ddlUnidade.SelectedValue = "0";

            //Thiago Rodrigues - 03/02/2026
            ddlLocalOPI.SelectedValue = "0";
            ddlTipoEtiqueta.SelectedValue = "U";

            ddlIbama.SelectedValue = "0";
            ddlNCM.SelectedValue = "0";
            ddlCEST.SelectedValue = "0";
            ddlPaisOrigem.SelectedValue = "0";
            ddlidCategoriaVendas.SelectedValue = "0";
            ddlExibeComercial.SelectedValue = "S";
            ddlExibeLM.SelectedValue = "S";
            hddidProduto.Value = "0";
            SwitchAtivoGarantia.Definir("N", "Garantia/Lote", "");
            SwitchAtivoSerivalizavel.Definir("S", "É Serializavel?", "");
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
            txtsUnMinimaCompra.Text = "";
            bs_Produtos_Descricao.Clear();
        }

        #endregion

        #region | Validar

        private bool ValidarDados(int sTipo)
        {
            if (sTipo == 0) // Produto
            {
                if (txtsDscProduto.Text.Length < 12)
                {
                    MensagemPagina.MostraMensagem_Erro("Descrição precisa ter mais de 12 Caracteres!");
                    return false;
                }
                if (ddlUnidade.SelectedValue == "0")
                {
                    MensagemPagina.MostraMensagem_Erro("Selecione uma Unidade na Aba <b><a href='#' onclick='ativarAbaCaracteristicas(); return false;'>[Características do Produto]</a></b>");
                    return false;
                }

                if (txtsCodigo.Text.Length < 4)
                {
                    MensagemPagina.MostraMensagem_Erro("Código do produto precisa ter mais de 4 caracteres");
                    return false;
                }
                else if (Regex.IsMatch(txtsCodigo.Text, @"[""'\\<> &%áàâãéèêíìîóòôõúùûÁÀÂÃÉÈÊÍÌÎÓÒÔÕÚÙÛçÇ$€£¥₡¢]")) //@"[""\\']|[^\x00-\x7F]"))
                {
                    MensagemPagina.MostraMensagem_Erro("O Código não pode conter aspas, barras, acentos, ç, símbolos (< > & % $) ou moedas.");
                    return false;
                }

                if (SwitchAtivoGarantia.Recuperar() == "S")
                {
                    if (string.IsNullOrEmpty(nTempoGarantia.Text) || !int.TryParse(nTempoGarantia.Text, out int tempoGarantia) || tempoGarantia <= 0)
                    {
                        MensagemPagina.MostraMensagem_Erro("O Campo <b>Tempo de Garantia</b> precisa ser maior que <b>0</b>, Corrija Clicando Aqui: <b><a href='#' onclick='ativarAbaCaracteristicas(); return false;'>[Características do Produto]</a></b> ");
                        return false;
                    }
                }
            }
            else if (sTipo == 1) // Serviço
            {
                string mensagem = "";
                if (txtsDscProduto.Text.Length < 12) mensagem += (mensagem != "" ? "</br>" : "") + "Descrição precisa ter mais de 12 Caracteres!";
                if (txtsCodigo.Text.Length < 4) mensagem += (mensagem != "" ? "</br>" : "") + "Código do serviço precisa ter mais de 4 caracteres!";
                if (ddlsUnidade.SelectedValue == "0") mensagem += (mensagem != "" ? "</br>" : "") + "Selecione uma Unidade!";
                if (ddlTipoProduto.SelectedValue == "0") mensagem += (mensagem != "" ? "</br>" : "") + "É necessário selecionar um Tipo de Serviço!";
                if (ddlidCategoriaVendas.SelectedValue == "0") mensagem += (mensagem != "" ? "</br>" : "") + "É necessário selecionar uma Categoria de Vendas!";
                if (ddlidServicoMunicipal.SelectedValue == "0") mensagem += (mensagem != "" ? "</br>" : "") + "É necessário selecionar um Serviço Municipal!";
                if (ddlNBS.SelectedValue == "0") mensagem += (mensagem != "" ? "</br>" : "") + "É necessário selecionar um Código NBS!";
                if (ddlIndOp.SelectedValue == "0") mensagem += (mensagem != "" ? "</br>" : "") + "É necessário selecionar um Indicador de Operação!";

                ddlsCodigoServicoPai.SelectedValue = sCodigoServicoPai;
                if (ddlsCodigoServicoPai.SelectedValue == "0") mensagem += (mensagem != "" ? "</br>" : "") + "É necessário selecionar um Código de Serviço Pai!";

                ddlsCodigoServicoFilho.SelectedValue = sCodigoServicoFilho;
                if (ddlsCodigoServicoFilho.SelectedValue == "0") mensagem += (mensagem != "" ? "</br>" : "") + "É necessário selecionar um Código de Serviço Filho!";

                if (mensagem != "")
                {
                    MensagemPagina.MostraMensagem_Erro(mensagem);
                    return false;
                }
            }
            else if (sTipo == 2) // Recurso
            {
                if (txtsDscProduto.Text.Length < 12)
                {
                    MensagemPagina.MostraMensagem_Erro("Descrição precisa ter mais de 12 Caracteres!");
                    return false;
                }
                if (txtsCodigo.Text.Length < 4)
                {
                    MensagemPagina.MostraMensagem_Erro("Código do Recurso precisa ter mais de 4 caracteres!");
                    return false;
                }
                if (ddlsUnidade.SelectedValue == "0")
                {
                    MensagemPagina.MostraMensagem_Erro("Selecione uma Unidade!");
                    return false;
                }
                if (ddlTipoProduto.SelectedValue == "0")
                {
                    MensagemPagina.MostraMensagem_Erro("É necessário selecionar um Tipo de Recurso!");
                    return false;
                }
                if (ExibirGastos.Recuperar() == "S")
                {
                    if (ddlCategoriapagar.SelectedValue == "0")
                    {
                        MensagemPagina.MostraMensagem_Erro("Selecione a Categoria Contas Pagar!");
                        return false;
                    }
                    if (txtvlrMaximo.Text == "")
                    {
                        MensagemPagina.MostraMensagem_Erro("Descreva o Valor Máximo!");
                        return false;
                    }
                }
            }
            else if (sTipo == 3) // Sub-Serviço
            {
                if (txtsDscProduto.Text.Length < 12)
                {
                    MensagemPagina.MostraMensagem_Erro("Descrição precisa ter mais de 12 Caracteres!");
                    return false;
                }
                if (txtsCodigo.Text.Length < 4)
                {
                    MensagemPagina.MostraMensagem_Erro("Código do Sub-Serviço precisa ter mais de 4 caracteres!");
                    return false;
                }
                if (ddlsUnidade.SelectedValue == "0")
                {
                    MensagemPagina.MostraMensagem_Erro("Selecione uma Unidade!");
                    return false;
                }
                if (ddlTipoProduto.SelectedValue == "0")
                {
                    MensagemPagina.MostraMensagem_Erro("É necessário selecionar um Tipo de Sub-Serviço!");
                    return false;
                }
                if (ddlidCategoriaVendas.SelectedValue == "0")
                {
                    MensagemPagina.MostraMensagem_Erro("É necessário selecionar uma Categoria de Vendas!");
                    return false;
                }
            }

            return true;
        }

        private bool Composicao_ValidarDados(bool bCliente)
        {
            if (bCliente)
            {
                bool bIncluso = false;
                string sidParceiros = string.Empty;
                string cnpjRaiz = "0";

                foreach (ListItem item in lstParceiros.Items)
                {
                    if (item.Selected)
                    {
                        if (!bs_Parceiros.Exists(p => p.IdParceiro.Equals(int.Parse(item.Value))))
                        {
                            if (item.Text.Split('-')[0].Trim().Length > 0)
                            {
                                string cnpj = item.Text.Split('-')[0].Trim();
                                string cnpjFormatado = cnpj.Replace(".", "").Replace("-", "").Replace("/", "").Length == 14 ? Convert.ToInt64(cnpj.Replace(".", "").Replace("-", "").Replace("/", "")).ToString(@"00\.000\.000\/0000\-00") : cnpj.Replace(".", "").Replace("-", "").Replace("/", "").Length == 11 ? Convert.ToInt64(cnpj.Replace(".", "").Replace("-", "").Replace("/", "")).ToString(@"000\.000\.000\-00") : cnpj;

                                if (cnpjRaiz == "0")
                                {
                                    cnpjRaiz = cnpj.PadRight(14, '0').Remove(8);
                                    sidParceiros += string.Format("{0};{1};{2}|", item.Value, cnpjFormatado, item.Text.Split('-')[1].Trim());
                                }
                                else if (cnpjRaiz == cnpj.PadRight(14, '0').Remove(8))
                                    sidParceiros += string.Format("{0};{1};{2}|", item.Value, cnpjFormatado, item.Text.Split('-')[1].Trim());
                                else
                                    throw new Exception("Só é possível selecionar mais de um Parceiro caso todos possuam o mesmo CNPJ raiz!");
                            }
                            else
                                throw new Exception("Um, ou mais, dos Parceiros selecionados não possuem CNPJ cadastrado!");
                        }
                        else
                            bIncluso = true;
                    }
                }

                if (sidParceiros == string.Empty && !bIncluso)
                    throw new Exception("É necessário selecionar ao menos 1 Parceiro para Incluir!");
                else if (bIncluso)
                    throw new Exception("Não é possível Incluir Parceiros já inclusos!");
                else
                    hdd_IncluirParceiros.Value += sidParceiros;

                if (Clientes_txtsDscProduto.Text.Length < 3)
                {
                    MensagemPagina_Clientes.MostraMensagem_Erro("É necessário preencher a Descrição do Produto no Parceiro, com ao menos 3 caracteres!");
                    return false;
                }
            }
            else
            {
                if (Pesquisa_Parceiros.SCnpj_CPF.Length < 3)
                {
                    MensagemPagina_Fornecedor.MostraMensagem_Erro("É necessário preencher o CNPJ / CPF do Parceiro Fornecedor, com ao menos 3 caracteres!");
                    return false;
                }
                if (Pesquisa_Parceiros.SRazaoSocial.Length < 3)
                {
                    MensagemPagina_Fornecedor.MostraMensagem_Erro("Razão Social inválida ou não cadastrado no sistema!");
                    return false;
                }
                if (FN_ddlidTipo.SelectedValue == "0")
                {
                    MensagemPagina_Fornecedor.MostraMensagem_Erro("Selecione um Tipo do Fornecedor para cadastrar!");
                    return false;
                }
            }

            return true;
        }

        private bool Composicao_ValidarDados(string cmd, FiltroPesquisa filtroPesquisa)
        {
            if (cmd == "0")
            {
                if (filtroPesquisa.SCodigo.Length < 3)
                {
                    Composicao_MensagemPagina.MostraMensagem_Erro("Código do Produto Inválido");
                    MensagemPaginaSugestao.MostraMensagem_Erro("Código do Produto Inválido");
                    return false;
                }
                if (filtroPesquisa.SDscProduto.Length < 6)
                {
                    Composicao_MensagemPagina.MostraMensagem_Erro("Produto inválido!");
                    return false;

                }
                if (filtroPesquisa.NQuantidade <= decimal.Zero)
                {
                    Composicao_MensagemPagina.MostraMensagem_Erro("Quantidade inválida!");
                    return false;
                }
                if (filtroPesquisa.SUnidade == "0" || filtroPesquisa.SUnidade == "")
                {
                    Composicao_MensagemPagina.MostraMensagem_Erro("Unidade Inválida!");
                    return false;
                }
            }
            else if (cmd == "1")
            {
                if (filtroPesquisa.SCodigoServico_Recurso.Length < 3)
                {
                    Composicao_MensagemPagina.MostraMensagem_Erro("Código do Serviço Inválido");
                    return false;
                }
                if (filtroPesquisa.SDscServico_Recurso.Length < 3)
                {
                    Composicao_MensagemPagina.MostraMensagem_Erro("Descrição do Serviço inválido!");
                    return false;
                }
                if (filtroPesquisa.NQuantidade <= decimal.Zero)
                {
                    Composicao_MensagemPagina.MostraMensagem_Erro("Quantidade inválida!");
                    return false;
                }
                if (filtroPesquisa.SUnidadeServico_Recurso == "")
                {
                    Composicao_MensagemPagina.MostraMensagem_Erro("Unidade Inválida!");
                    return false;
                }
            }
            else if (cmd == "2")
            {
                if (filtroPesquisa.SCodigoServico_Recurso.Length < 3)
                {
                    Composicao_MensagemPagina.MostraMensagem_Erro("Código do Recurso Inválido");
                    return false;
                }
                if (filtroPesquisa.SDscServico_Recurso.Length < 3)
                {
                    Composicao_MensagemPagina.MostraMensagem_Erro("Descrição do Recurso inválido!");
                    return false;
                }
                if (filtroPesquisa.SUnidadeServico_Recurso == "")
                {
                    Composicao_MensagemPagina.MostraMensagem_Erro("Unidade Inválida!");
                    return false;
                }
            }
            else if (cmd == "3")
            {
                if (filtroPesquisa.SCodigoServico_Recurso.Length < 3)
                {
                    Composicao_MensagemPagina.MostraMensagem_Erro("Código do Sub-Serviço Inválido!");
                    return false;
                }
                if (filtroPesquisa.SDscServico_Recurso.Length < 3)
                {
                    Composicao_MensagemPagina.MostraMensagem_Erro("Descrição do Sub-Serviço inválido!");
                    return false;
                }
                if (filtroPesquisa.NQuantidade <= decimal.Zero)
                {
                    Composicao_MensagemPagina.MostraMensagem_Erro("Quantidade inválida!");
                    return false;
                }
                if (filtroPesquisa.SUnidadeServico_Recurso == "")
                {
                    Composicao_MensagemPagina.MostraMensagem_Erro("Unidade inválida!");
                    return false;
                }
            }
            else
                return false;

            return true;
        }

        #endregion

        #region | Outros

        string ArredondarValor(string str)
        {
            int valor = 0;
            if (!string.IsNullOrEmpty(str))
            {
                valor = (int)double.Parse(str);
                return valor.ToString();
            }
            return "0";
        }

        protected string removeCaracteres(string sObjeto)
        {
            string sObjetoSemCaracter = new string(sObjeto
                .Where(c => Char.IsDigit(c) || c == ',' || c == '.')
                .Select(c => c == ',' ? '.' : c)
                .ToArray());

            if (sObjetoSemCaracter == "")
            {
                sObjetoSemCaracter = "0";
            }

            return sObjetoSemCaracter;
        }

        decimal ConverterStringDecimal(string dado)
        {
            decimal converter = 0;
            if (decimal.TryParse(dado, out converter))
            {
                converter = Convert.ToDecimal(dado);
                return converter;
            }
            return decimal.Zero;
        }

        #endregion

        #region | GridView

        void dtgItens_DataBind()
        {
            dtgItens.DataSource = bs_Produto_Composicao;
            //cmdComposicao_Alterar.Visible = false;
            dtgItens.DataBind();
            RegistraScript();
        }

        void dtgSugestao_Databind()
        {
            dtgSugestao.DataSource = BS_SUGESTAO;
            dtgSugestao.DataBind();
        }

        void dtgFornecedores_Databind()
        {
            dtgFornecedores.DataSource = BS_FORNECEDORES;
            dtgFornecedores.DataBind();
        }

        protected void dtgItens_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (hddsCadeado.Value == "N")
            {
                int index = e.RowIndex;
                bs_Produto_Composicao.RemoveAt(index);
            }
            dtgItens_DataBind();
            Pesquisa_Parceiros.RegistrarScriptPesquisarItens();
        }

        protected void dtgItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlExibePedidoGV = (DropDownList)e.Row.FindControl("ddlExibePedidoGV");

                if (ddlExibePedidoGV != null)
                {
                    string ddlExibePedidoGV_Value = Session["ddlExibePedidoGV_Value"] as string;

                    if (!string.IsNullOrEmpty(ddlExibePedidoGV_Value))
                    {
                        ddlExibePedidoGV.SelectedValue = ddlExibePedidoGV_Value;
                    }
                }
            }
        }

        bool Atualizar_ItensGvClasse()
        {
            for (int i = 0; i < dtgItens.Rows.Count; i++)
            {
                GridViewRow item = dtgItens.Rows[i];

                if (item.RowType == DataControlRowType.DataRow)
                {
                    try
                    {
                        TextBox txtnQuantidade = (TextBox)item.FindControl("txtnQuantidade");
                        if (txtnQuantidade.Text == "-1")
                        {
                            MensagemPagina.MostraMensagem_Erro("Atualize a Quantidade na Aba Composição");
                            return false;
                        }
                        else
                        {
                            bs_Produto_Composicao[i].NQuantidade = Convert.ToDecimal(txtnQuantidade.Text);
                        }

                        TextBox txtnOrdem = (TextBox)item.FindControl("nOrdem");
                        bs_Produto_Composicao[i].nOrdem = Convert.ToInt32(txtnOrdem.Text);

                        DropDownList ddlExibePedidoGV = (DropDownList)item.FindControl("ddlExibePedidoGV");
                        bs_Produto_Composicao[i].sExibePedido = ddlExibePedidoGV.SelectedValue;
                    }
                    catch
                    {
                        Composicao_MensagemPagina.MostraMensagem_Erro("Dados Incorretos na Grid");
                        return false;
                    }
                }
            }
            return true;
        }

     #region | Instalação/Obra

        static readonly System.Globalization.CultureInfo culturaBR_Instalacao =
            System.Globalization.CultureInfo.CreateSpecificCulture("pt-BR");

        /// <summary>
        /// Carrega a configuração de instalação gravada e prepara o filtro da aba.
        /// Só é chamado no ramo de Produto (sValidaTipo = 0) do Pesquisar.
        /// </summary>
        void Popular_Aba_Instalacao(string idProduto)
        {
            int.TryParse(idProduto, out int nIdProduto);

            bs_Produto_Instalacao = nIdProduto > 0
                                  ? cls_ProdutoInstalacao.Consultar(nIdProduto)
                                  : new List<cls_ProdutoInstalacaoItem>();

            // O HH é digitado na grid, não no filtro: o campo do filtro se chama
            // "Quantidade" e usá-lo como HH induziria o usuário a erro.
            //fpInstalacao.Controle_ExibicaoCampos(new Dictionary<string, bool>
            //{
            //    { "Quantidade", false },
            //    { "Valor Unitário", false }
            //}, "3");

            aba_Instalacao.Visible = true;

            dtgInstalacao_DataBind();
        }

        void dtgInstalacao_DataBind()
        {
            // Binda a lista direto, sem OrderBy: o índice da linha da grid precisa casar
            // com o índice em bs_Produto_Instalacao para excluir e reler valores.
            dtgInstalacao.DataSource = bs_Produto_Instalacao;
            dtgInstalacao.DataBind();

            txtInstalacao_TotalHH.Text = bs_Produto_Instalacao.Sum(i => i.HH).ToString("N2", culturaBR_Instalacao);
        }

        protected void cmdInstalacao_IncluirItem_Click(object sender, EventArgs e)
        {
            int.TryParse(fpInstalacao.IdServico_Recurso, out int idItemInstalacao);
            int.TryParse(hddidProduto.Value, out int idProduto);

            if (idItemInstalacao <= 0)
            {
                Instalacao_MensagemPagina.MostraMensagem_Erro("Selecione um Serviço, Sub-Serviço ou Recurso válido!");
                return;
            }

            if (idItemInstalacao == idProduto)
            {
                Instalacao_MensagemPagina.MostraMensagem_Erro("O próprio produto não pode ser item da sua instalação!");
                return;
            }

            if (bs_Produto_Instalacao.Any(i => i.IdItemInstalacao == idItemInstalacao))
            {
                Instalacao_MensagemPagina.MostraMensagem_Aviso("Este item já está na configuração de instalação!", false);
                return;
            }

            // Preserva o que já foi digitado nas outras linhas antes de rebindar.
            Atualizar_InstalacaoGvClasse(false);

            bs_Produto_Instalacao.Add(new cls_ProdutoInstalacaoItem
            {
                IdProduto        = idProduto,
                IdItemInstalacao = idItemInstalacao,
                Codigo           = fpInstalacao.SCodigoServico_Recurso,
                Descricao        = fpInstalacao.SDscServico_Recurso,
                Tipo             = fpInstalacao.STipoServico_Recurso,
                Unidade          = fpInstalacao.SUnidadeServico_Recurso,
                HH               = fpInstalacao.NQuantidade,
                Ordem            = bs_Produto_Instalacao.Any() ? bs_Produto_Instalacao.Max(i => i.Ordem) + 1 : 1
            });

            dtgInstalacao_DataBind();
            fpInstalacao.LimparCampos();

            // LimparCampos limpa os textos mas deixa o id escondido preenchido - é assim para
            // todo mundo que usa o controle, e não vou mexer nele por causa desta tela. Sem
            // zerar aqui, clicar em Incluir de novo sem selecionar nada acusaria "item já está
            // na configuração", quando o certo é pedir que selecione um item.
            fpInstalacao.IdServico_Recurso = "";

            Instalacao_MensagemPagina.MostraMensagem_Aviso("Item incluído. Informe o HH antes de salvar.", false);
        }

        protected void dtgInstalacao_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (hddsCadeado.Value == "N")
            {
                // Sem validar: o usuário pode estar justamente removendo a linha inválida.
                Atualizar_InstalacaoGvClasse(false);
                bs_Produto_Instalacao.RemoveAt(e.RowIndex);
            }

            dtgInstalacao_DataBind();
        }

        /// <summary>
        /// Lê a grid de volta para a lista.
        /// </summary>
        /// <param name="bValidar">
        /// true  - recusa HH ausente, não numérico ou menor/igual a zero e devolve false.
        /// false - grava o que conseguir converter e ignora o resto, preservando o valor
        ///         anterior. Usado ao incluir e ao excluir, onde barrar não faz sentido.
        /// </param>
        bool Atualizar_InstalacaoGvClasse(bool bValidar)
        {
            for (int i = 0; i < dtgInstalacao.Rows.Count && i < bs_Produto_Instalacao.Count; i++)
            {
                GridViewRow row = dtgInstalacao.Rows[i];

                if (row.RowType != DataControlRowType.DataRow)
                    continue;

                TextBox txtOrdem = (TextBox)row.FindControl("txtInstalacao_nOrdem");
                TextBox txtHH    = (TextBox)row.FindControl("txtInstalacao_nHH");

                if (txtOrdem != null && int.TryParse(txtOrdem.Text, out int nOrdem))
                    bs_Produto_Instalacao[i].Ordem = nOrdem;

                if (txtHH == null)
                    continue;

                string sHH = (txtHH.Text ?? "").Trim();

                // Mesmo motivo da Categoria de Vendas: a aplicação roda em pt-BR, onde o
                // ponto é separador de milhar, e "1.5" viraria 15.
                if (sHH.Contains("."))
                {
                    if (!bValidar) continue;

                    Instalacao_MensagemPagina.MostraMensagem_Erro(string.Format(
                        "Use vírgula como separador decimal no HH de <b>{0}</b> (exemplo: 1,5).",
                        bs_Produto_Instalacao[i].Descricao));
                    return false;
                }

                if (!decimal.TryParse(sHH, System.Globalization.NumberStyles.Number, culturaBR_Instalacao, out decimal nHH))
                {
                    if (!bValidar) continue;

                    Instalacao_MensagemPagina.MostraMensagem_Erro(string.Format(
                        "O HH de <b>{0}</b> deve ser um valor numérico.",
                        bs_Produto_Instalacao[i].Descricao));
                    return false;
                }

                if (nHH <= decimal.Zero)
                {
                    if (!bValidar) continue;

                    Instalacao_MensagemPagina.MostraMensagem_Erro(string.Format(
                        "Informe um HH maior que zero para <b>{0}</b>, ou remova o item da lista.",
                        bs_Produto_Instalacao[i].Descricao));
                    return false;
                }

                bs_Produto_Instalacao[i].HH = nHH;
            }

            return true;
        }

        /// <summary>
        /// Grava a configuração. A procedure substitui a lista inteira numa transação.
        /// </summary>
        bool Instalacao_Salvar(string idProduto)
        {
            int.TryParse(idProduto, out int nIdProduto);

            Atualizar_InstalacaoGvClasse(false);
            if (nIdProduto <= 0)
                return true;

            // Trava de segurança: se a aba não chegou a carregar (procedure ausente, erro na
            // consulta), a lista está vazia por falta de dado e não por escolha do usuário -
            // gravar aqui apagaria a configuração existente. Só grava o que foi de fato lido.
            if (!aba_Instalacao.Visible)
                return true;

            int.TryParse(Variaveis.idUsuario(), out int nIdUsuario);

            string sErro = cls_ProdutoInstalacao.Substituir(nIdProduto, bs_Produto_Instalacao, nIdUsuario);

            if (sErro != "")
            {
                Instalacao_MensagemPagina.MostraMensagem_Erro(sErro);
                return false;
            }

            return true;
        }

        #endregion
        protected void dtgFornecedores_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            EsconderColunas(e, 0);
        }

        protected void dtgFornecedores_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (hddsCadeado.Value == "N")
            {
                int index = e.RowIndex;
                BS_FORNECEDORES.RemoveAt(index);
            }
            dtgFornecedores_Databind();
            Pesquisa_Parceiros.RegistrarScriptPesquisarItens();
        }

        protected void dtgSugestao_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void dtgSugestao_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (hddsCadeado.Value == "N")
            {
                int index = e.RowIndex;
                BS_SUGESTAO.RemoveAt(index);
            }
            dtgSugestao_Databind();
            Pesquisa_Parceiros.RegistrarScriptPesquisarItens();
        }

        protected void dtgProdutos_Descricao_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (hddsCadeado.Value == "N")
            {
                int index = e.RowIndex;
                bs_Produtos_Descricao.RemoveAt(index);
            }
            dtgProdutos_Descricao_DataBind();
        }

        protected void dtgProdutos_Descricao_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName.ToLower().Trim().Equals("editar"))
                {
                    int index = int.Parse(e.CommandArgument.ToString());
                    var item = bs_Produtos_Descricao[index];

                    if (item != null)
                    {
                        hddidProduto.Value = item.idProduto.ToString();

                        txtsDscDescricao.Text = item.sDscDescricao;
                        ddlidPais.SelectedValue = item.idPais.ToString();
                        ddlDescricao_IdTipo.SelectedValue = item.idTipo.ToString();

                        bs_Produtos_Descricao.RemoveAt(index);
                    }
                    else
                        throw new Exception("Item não encontrado!");
                }
            }
            catch (Exception ex)
            {
                MensagemProdutos_Descricao.MostraMensagem_Erro("Houve um erro na tentativa de Editar o Idioma!<br />Erro ao Editar: " + ex.Message, true);
            }
            dtgProdutos_Descricao_DataBind();
        }

        #endregion

        #region | Salvar

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            string sErro = "";
            string sLocalErro = "Produto";

            if (Request["stp"] == "1")
            {
                if (ValidarDados(1)) // 1 - Serviços
                {
                    try
                    {
                        string[] vidProduto = hddidProduto.Value.Split(',');
                        string idProduto = vidProduto[0].ToString();

                        if (Convert.ToBoolean(Request["duplicar"]))
                            txtidProduto.Text = "";
                        else
                            txtidProduto.Text = idProduto;

                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "SALVAR_SERVICO" },
                            { "@idProduto", txtidProduto.Text },
                            { "@idTipoProduto", ddlTipoProduto.SelectedValue },
                            { "@sDscProduto", txtsDscProduto.Text.Replace('|', '-') },
                            { "@idCategoriaVendas", ddlidCategoriaVendas.SelectedValue },
                            { "@idGrupo", ddlGrupo.SelectedValue },
                            { "@sCodigo", txtsCodigo.Text },
                            { "@sExibeComercial", ddlExibeComercial.SelectedValue },
                            { "@sExibeLM", ddlExibeLM.SelectedValue },
                            { "@sUnidade", ddlsUnidade.SelectedValue },
                            { "@sSituacao", comboAtivoServico.Situacao_Recuperar() },
                            { "@idUsuarioAtualizacao", Variaveis.idUsuario() },
                            { "@sCadeado", hddsCadeado.Value },
                            { "@idCodigoServico", ddlsCodigoServicoFilho.SelectedValue },
                            { "@idServicoMunicipal", ddlidServicoMunicipal.SelectedValue },
                            { "@idNBS", ddlNBS.SelectedValue },
                            { "@idIndOp", ddlIndOp.SelectedValue }
                        };
                        DataSet dsSalvar = ExecutarDataSet(sProcedure, vParametros);

                        if (ValidarDataSet(dsSalvar, out sErro))
                        {
                            idProduto = DATASET(dsSalvar, 0, "idProduto");
                            sErro = DATASET(dsSalvar, 0, "sErro");
                            string msg = DATASET(dsSalvar, 0, "msg");

                            if (sErro == null || sErro == string.Empty)
                            {
                                if (msg != string.Empty)
                                {
                                    if (Composicao_Salvar(idProduto, 1) && Salvar_Produtos_Descricao(idProduto))
                                        DirecionaPagina(string.Format("App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}&stp=1&msg=1", idProduto));
                                }
                                else DirecionaPagina(string.Format("App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}&stp=1&msg=1", idProduto));
                            }
                            else MensagemPagina.MostraMensagem_Erro(sErro);
                        }
                        else throw new Exception("BD: " + sErro.ToString());
                    }
                    catch (Exception ex)
                    {
                        MensagemPagina.MostraMensagem_Erro(ex.Message);
                    }

                    RegistraScript();
                }
            }// 1 - Serviços
            else if (Request["stp"] == "2")
            {
                if (ValidarDados(2)) // 2 - Recursos
                {
                    try
                    {
                        string[] vidProduto = hddidProduto.Value.Split(',');
                        string idProduto = vidProduto[0].ToString();

                        if (bs_Tabelas.Count > 0)
                            AbaTabelas_Salvar(idProduto);

                        if (Convert.ToBoolean(Request["duplicar"]))
                            txtidProduto.Text = "";
                        else
                            txtidProduto.Text = idProduto;

                        if (txtvlrMaximo.Text == "")
                            txtvlrMaximo.Text = "0,00";

                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "SALVAR_RECURSO" },
                            { "@idProduto", txtidProduto.Text },
                            { "@idTipoProduto", ddlTipoProduto.SelectedValue },
                            { "@sDscProduto", txtsDscProduto.Text },
                            { "@idCategoriaVendas", ddlidCategoriaVendas.SelectedValue },
                            { "@idGrupo", ddlGrupo.SelectedValue },
                            { "@sCodigo", txtsCodigo.Text },
                            { "@sExibeComercial", ddlExibeComercial.SelectedValue },
                            { "@sExibeLM", ddlExibeLM.SelectedValue },
                            { "@sUnidade", ddlsUnidade.SelectedValue },
                            { "@sSituacao", comboAtivoServico.Situacao_Recuperar() },
                            { "@idUsuarioAtualizacao", Variaveis.idUsuario() },
                            { "@sCadeado", hddsCadeado.Value },
                            { "@sExibirGastos", ExibirGastos.Recuperar() },
                            { "@idCategoriaPagar", ddlCategoriapagar.SelectedValue },
                            { "@vlrMaximo", txtvlrMaximo.Text.Replace(".", "").Replace(",", ".") }
                        };
                        DataSet dsSalvar = ExecutarDataSet(sProcedure, vParametros);

                        if (ValidarDataSet(dsSalvar, out sErro))
                        {
                            idProduto = DATASET(dsSalvar, 0, "idProduto");
                            sErro = DATASET(dsSalvar, 0, "sErro");
                            string msg = DATASET(dsSalvar, 0, "msg");

                            if (sErro == null || sErro == string.Empty)
                            {
                                if (msg != string.Empty)
                                {
                                    if (Composicao_Salvar(idProduto, 2) && Salvar_Produtos_Descricao(idProduto) && Salvar_EPI() && ConsumivelSalvar())
                                        DirecionaPagina(string.Format("App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}&stp=2&msg=1", idProduto));
                                }
                                else
                                    DirecionaPagina(string.Format("App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}&stp=2&msg=1", idProduto));
                            }
                            else
                                MensagemPagina.MostraMensagem_Erro(sErro);
                        }
                        else
                        {
                            throw new Exception("BD: " + sErro.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        MensagemPagina.MostraMensagem_Erro(ex.Message);
                    }
                    RegistraScript();
                }
                else
                {
                    ExibirGastos.Definir(ExibirGastos.Recuperar(), "Exibir Relatório de Gastos", "N");
                    if (ExibirGastos.Recuperar() == "N")
                    {
                        DIV_CategoriaPagar.Attributes["class"] = "col-lg-10 invisivel";
                    }
                    else
                    {
                        DIV_CategoriaPagar.Attributes["class"] = "col-lg-10";
                    }
                }
            } // 2 - Recursos
            else if (Request["stp"] == "3")
            {
                if (ValidarDados(3)) // 3 - Sub-Serviços
                {
                    try
                    {
                        string[] vidProduto = hddidProduto.Value.Split(',');
                        string idProduto = vidProduto[0].ToString();



                        if (Convert.ToBoolean(Request["duplicar"]))
                            txtidProduto.Text = "";
                        else
                            txtidProduto.Text = idProduto;

                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "SALVAR_SERVICO" },
                            { "@idProduto", txtidProduto.Text },
                            { "@idTipoProduto", ddlTipoProduto.SelectedValue },
                            { "@sDscProduto", txtsDscProduto.Text },
                            { "@idCategoriaVendas", ddlidCategoriaVendas.SelectedValue },
                            { "@idGrupo", ddlGrupo.SelectedValue },
                            { "@sCodigo", txtsCodigo.Text },
                            { "@sExibeComercial", ddlExibeComercial.SelectedValue },
                            { "@sExibeLM", ddlExibeLM.SelectedValue },
                            { "@sUnidade", ddlsUnidade.SelectedValue },
                            { "@sSituacao", comboAtivoServico.Situacao_Recuperar() },
                            { "@idUsuarioAtualizacao", Variaveis.idUsuario() },
                            { "@sCadeado", hddsCadeado.Value }
                        };
                        DataSet dsSalvar = ExecutarDataSet(sProcedure, vParametros);

                        if (ValidarDataSet(dsSalvar, out sErro))
                        {
                            idProduto = DATASET(dsSalvar, 0, "idProduto");
                            sErro = DATASET(dsSalvar, 0, "sErro");
                            string msg = DATASET(dsSalvar, 0, "msg");

                            if (sErro == null || sErro == string.Empty)
                            {
                                if (msg != string.Empty)
                                {
                                    if (Composicao_Salvar(idProduto, 1) && Salvar_Produtos_Descricao(idProduto))
                                        DirecionaPagina(string.Format("App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}&stp=3&msg=1", idProduto));
                                }
                                else
                                    DirecionaPagina(string.Format("App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}&stp=3&msg=1", idProduto));
                            }
                            else
                                MensagemPagina.MostraMensagem_Erro(sErro.Contains("Código") ? "Código de Sub-Serviço já cadastrado!" : sErro);
                        }
                        else
                        {
                            throw new Exception("BD: " + sErro.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        MensagemPagina.MostraMensagem_Erro(ex.Message);
                    }
                    RegistraScript();
                }
            } // 3 - Sub-Serviços
            else
            {
                if (ValidarDados(0)) // 0 - Produtos
                {
                    try
                    {
                        if (bs_Produtos_Documentacao.Count > 0)
                        {
                            try
                            {
                                string idProdutoDocumentacao = "";
                                foreach (var item in bs_Produtos_Documentacao)
                                {
                                    if (item.idDocumentacao == "0")
                                    {

                                        Dictionary<string, string> vParametrosDocumentacao = new Dictionary<string, string>
                                        {
                                            { "@sFuncao", "SALVAR_DOCUMENTACAO" },
                                            { "@idProduto", item.idProduto.ToString() },
                                            { "@sDscTipo", item.sDscTipo },
                                            { "@sDscIdioma", item.sDscIdioma },
                                            { "@sDscEndereco", item.sDscEndereco },
                                            { "@sListarOrcamento", item.sListarOrcamento },
                                            { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                                        };
                                        DataSet ds = ExecutarDataSet(sProcedure, vParametrosDocumentacao);

                                        if (ValidarDataSet(ds, out sErro))

                                            idProdutoDocumentacao = item.idProduto.ToString();


                                    }
                                    else
                                    {

                                        Dictionary<string, string> vParametrosDocumentacao = new Dictionary<string, string>
                                        {
                                            { "@sFuncao", "ATUALIZAR_DOCUMENTACAO" },
                                            { "@idDocumentacao", item.idDocumentacao },
                                            { "@idProduto", item.idProduto.ToString() },
                                            { "@sDscTipo", item.sDscTipo },
                                            { "@sDscIdioma", item.sDscIdioma },
                                            { "@sDscEndereco", item.sDscEndereco },
                                            { "@sListarOrcamento", item.sListarOrcamento },
                                            { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                                        };
                                        DataSet ds = ExecutarDataSet(sProcedure, vParametrosDocumentacao);

                                        if (ValidarDataSet(ds, out sErro))

                                            idProdutoDocumentacao = item.idProduto.ToString();


                                    }
                                }
                                RegistraScript();
                            }
                            catch (Exception ex)
                            {
                                MensagemPagina.MostraMensagem_Erro("Erro ao salvar os dados da aba documentação: " + ex);
                            }
                        }

                        string Participantes = "";
                        foreach (ListItem item in ddlsSistemas.Items)
                        {
                            if (item.Selected)
                            {
                                Participantes += item.Value + ",";
                            }
                        }

                        Dictionary<string, string> vParametrosSistemas = new Dictionary<string, string>
                                        {
                        { "@sFuncao", "INCLUIR_SISTEMAS" },
                           { "@idProduto", hddidProduto.Value },
                           { "@sSistema", Participantes },
                        };
                        DataSet dsSistemas = ExecutarDataSet(sProcedure, vParametrosSistemas);

                        string[] vidProduto = hddidProduto.Value.Split(',');
                        string idProduto = vidProduto[0].ToString();

                        if (bs_Fabricacao_Produto.Count > 0 || bs_Fabricacao_Recurso.Count > 0 || bs_Fabricacao_Processo.Count > 0)
                        {
                            sMsgErro = FabricacaoSalvar();

                            if (sMsgErro != "")
                                sLocalErro = "Fabricacao";
                        }

                        if (bs_Tabelas.Count > 0)
                            AbaTabelas_Salvar(idProduto);

                        double comprimento = 0, largura = 0, altura = 0;

                        if (Convert.ToBoolean(Request["duplicar"]))
                            txtidProduto.Text = "";
                        else
                            txtidProduto.Text = idProduto;
                        try
                        {
                            comprimento = Convert.ToDouble(txtnComprimento.Text);
                            largura = Convert.ToDouble(txtnLargura.Text);
                            altura = Convert.ToDouble(txtnAltura.Text);
                        }
                        catch { }

                        double volume = comprimento == 0 || largura == 0 || altura == 0 ? 0 : comprimento * largura * altura / 1000000000;
                        txtnVolume.Text = volume.ToString("0.0000");

                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "SALVAR" },
                            { "@idProduto", txtidProduto.Text },
                            { "@idTipoProduto", ddlTipoProduto.SelectedValue },
                            { "@sDscProduto", txtsDscProduto.Text },
                            { "@idGrupo", ddlGrupo.SelectedValue },
                            { "@idFamilia", ddlFamilia.SelectedValue },
                            { "@idCategoriaVendas", ddlidCategoriaVendas.SelectedValue },
                            { "@idNCM", ddlNCM.SelectedValue },
                            { "@idCEST", ddlCEST.SelectedValue },
                            { "@idLocalArmazenamento", ddlArmazenagem.SelectedValue },
                            { "@sIbama", ddlIbama.SelectedItem.Value },
                            { "@sUnidade", ddlUnidade.SelectedValue },
                            { "@sUnidadeEntrega", ddlUnidadeEntrega.SelectedValue },

                            //Thiago Rodrigues - 03/02/2026
                            { "@idLocalOPI", ddlLocalOPI.SelectedValue },
                            { "@sTipoEtiqueta", ddlTipoEtiqueta.SelectedValue },


                            { "@sCodigo", txtsCodigo.Text.Trim() },
                            { "@sExibeComercial", ddlExibeComercial.SelectedValue },
                            { "@sExibeLM", ddlExibeLM.SelectedValue },
                            { "@nComprimento", removeCaracteres(txtnComprimento.Text) },
                            { "@nLargura", removeCaracteres(txtnLargura.Text) },
                            { "@nAltura", removeCaracteres(txtnAltura.Text) },
                            { "@nVolume", removeCaracteres(txtnVolume.Text) },
                            { "@nCubagem", removeCaracteres(txtnCubagem.Text) },
                            { "@nPesoNeto", removeCaracteres(txtnPesoNeto.Text) },
                            { "@nPesoBruto", removeCaracteres(txtnPesoBruto.Text) },
                            { "@nEstoqueMinimo", removeCaracteres(txtnEstoqueMinimo.Text) },
                            { "@nEstoqueAtual", removeCaracteres(txtnEstoqueAtual.Text) },
                            { "@sFabricante", txtsFabricante.Text },
                            { "@nEstoqueReservado", removeCaracteres(txtnEstoqueReservado.Text) },
                            { "@nUMZ_Ratio", removeCaracteres(txtnUMZ_Ratio.Text) },
                            { "@sUnMinimaCompra", txtsUnMinimaCompra.Text },
                            { "@sSituacao", ComboAtivo.Situacao_Recuperar() },
                            { "@idUsuarioAtualizacao", Variaveis.idUsuario() },
                            { "@idPais", ddlPaisOrigem.SelectedValue },
                            { "@sCadeado", hddsCadeado.Value },
                            { "@sControlaGarantiaLote", SwitchAtivoGarantia.Recuperar()},
                            { "@sSerializavel",  SwitchAtivoSerivalizavel.Recuperar()},
                            { "@nTempoGarantia", nTempoGarantia.Text},
                            { "@sCodigoEan" , txtsCodigoEAN.Text }
                        };
                        DataSet dsSalvar = ExecutarDataSet(sProcedure, vParametros);


                        if (ValidarDataSet(dsSalvar, out sErro))
                        {
                            idProduto = DATASET(dsSalvar, 0, "idProduto");
                            sErro = DATASET(dsSalvar, 0, "sErro");

                            if (string.IsNullOrEmpty(sErro) && sMsgErro == "")
                            {
                                if (Composicao_Salvar(idProduto, 0) && Salvar_Produtos_Descricao(idProduto) && Instalacao_Salvar(idProduto))
                                    DirecionaPagina(string.Format("App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}&msg=1", idProduto));
                            }
                            else
                            {
                                switch (sLocalErro)
                                {
                                    case "Produto":
                                        MensagemPagina.MostraMensagem_Erro(sErro);
                                        Scripts.Mantem_AbaAtiva(Page, "produto-tab");
                                        break;
                                    case "Fabricacao":
                                        Fabricacao_MensagemPagina.MostraMensagem_Erro(sMsgErro);
                                        Scripts.Mantem_AbaAtiva(Page, "fabricacao-tab");
                                        break;
                                }
                            }
                        }
                        else

                            throw new Exception("BD: " + sErro.ToString());



                    }
                    catch (Exception ex)
                    {
                        MensagemPagina.MostraMensagem_Erro(ex.Message);
                    }
                    RegistraScript();
                }
            }// 0 - Produtos
        }

        bool Composicao_Salvar(string idProduto, int sTipo)
        {
            bool bRetorno = false;

            try
            {
                if (sTipo == 1)
                {
                    if (!Atualizar_ItensGvClasse() && Request["duplicar"] == "true")
                        return false;

                    DataSet dsItens_Excluir;
                    Dictionary<String, String> vParametroItens_Excluir = new Dictionary<string, string>
                    {
                        { "@sFuncao", "EXCLUIR_ITEM" },
                        { "@idProduto", idProduto }
                    };
                    dsItens_Excluir = ExecutarDataSet("sp_Manipula_tbl_Flow_WMS_Composicao", vParametroItens_Excluir);

                    DataSet ds = new DataSet();
                    Dictionary<string, string> vParametrosItens = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idProduto", idProduto },
                        { "@idItemComposicao", "" },
                        { "@nQuantidade", "" },
                        { "@sUnidade", "" },
                        { "@sExibePedido", ddlExibePedido.SelectedValue }
                    };

                    bs_Produto_Composicao.ForEach(a =>
                    {
                        vParametrosItens["@idItemComposicao"] = a.IdItemComposicao.ToString();
                        vParametrosItens["@nQuantidade"] = a.NQuantidade.ToString().Replace(",", ".");
                        vParametrosItens["@sUnidade"] = a.SUnidade.ToString();
                        vParametrosItens["@sExibePedido"] = a.sExibePedido.ToString();
                        vParametrosItens["@nOrdem"] = a.nOrdem.ToString();

                        ds = ExecutarDataSet("sp_Manipula_tbl_Flow_WMS_Composicao", vParametrosItens);
                    });

                    FiltroPesquisa.LimparCampos();
                }
                else if (sTipo == 2)
                {
                    if (!Atualizar_ItensGvClasse() && Request["duplicar"] == "true")
                        return false;

                    DataSet dsItens_Excluir;
                    Dictionary<String, String> vParametroItens_Excluir = new Dictionary<string, string>
                    {
                        { "@sFuncao", "EXCLUIR_ITEM" },
                        { "@idProduto", idProduto }
                    };
                    dsItens_Excluir = ExecutarDataSet("sp_Manipula_tbl_Flow_WMS_Composicao", vParametroItens_Excluir);

                    DataSet ds = new DataSet();
                    Dictionary<string, string> vParametrosItens = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idProduto", idProduto },
                        { "@idItemComposicao", "" },
                        { "@nQuantidade", "" },
                        { "@sUnidade", "" },
                        { "@sExibePedido", ddlExibePedido.SelectedValue }
                    };

                    bs_Produto_Composicao.ForEach(a =>
                    {
                        vParametrosItens["@idItemComposicao"] = a.IdItemComposicao.ToString();
                        vParametrosItens["@nQuantidade"] = a.NQuantidade.ToString().Replace(",", ".");
                        vParametrosItens["@sUnidade"] = a.SUnidade.ToString();
                        vParametrosItens["@sExibePedido"] = a.sExibePedido.ToString();
                        vParametrosItens["@nOrdem"] = a.nOrdem.ToString();

                        ds = ExecutarDataSet("sp_Manipula_tbl_Flow_WMS_Composicao", vParametrosItens);
                    });

                    FiltroPesquisa.LimparCampos();
                }
                else if (sTipo == 0)
                {
                    if (!Atualizar_ItensGvClasse() && Request["duplicar"] == "true")
                        return false;

                    DataSet dsItens_Excluir;
                    Dictionary<String, String> vParametroItens_Excluir = new Dictionary<string, string>
                    {
                        { "@sFuncao", "EXCLUIR_ITEM" },
                        { "@idProduto", idProduto }
                    };
                    dsItens_Excluir = ExecutarDataSet("sp_Manipula_tbl_Flow_WMS_Composicao", vParametroItens_Excluir);

                    DataSet ds = new DataSet();
                    Dictionary<string, string> vParametrosItens = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idProduto", idProduto },
                        { "@idItemComposicao", "" },
                        { "@nQuantidade", "" },
                        { "@sUnidade", "" },
                        { "@sExibePedido", ddlExibePedido.SelectedValue }
                    };

                    bs_Produto_Composicao.ForEach(a =>
                    {
                        vParametrosItens["@idItemComposicao"] = a.IdItemComposicao.ToString();
                        vParametrosItens["@nQuantidade"] = a.NQuantidade.ToString().Replace(",", ".");
                        vParametrosItens["@sUnidade"] = a.SUnidade.ToString();
                        vParametrosItens["@sExibePedido"] = a.sExibePedido.ToString();
                        vParametrosItens["@nOrdem"] = a.nOrdem.ToString();

                        ds = ExecutarDataSet("sp_Manipula_tbl_Flow_WMS_Composicao", vParametrosItens);
                    });
                    FiltroPesquisa.LimparCampos();

                    DataSet dsItensSugestao_Excluir;
                    Dictionary<String, String> vParametrosSugestaoItens = new Dictionary<string, string>();
                    vParametrosSugestaoItens.Add("@sFuncao", "SUGESTAO_DELETAR");
                    vParametrosSugestaoItens.Add("@idProduto", idProduto);
                    vParametrosSugestaoItens.Add("@idUsuarioAtualizacao", Variaveis.idUsuario());
                    dsItensSugestao_Excluir = ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParametrosSugestaoItens);

                    vParametrosSugestaoItens["@sFuncao"] = "SUGESTAO_INCLUIR_ITENS";
                    vParametrosSugestaoItens.Add("@idProdutoSugestao", "");
                    vParametrosSugestaoItens.Add("@sUnidade", "");
                    vParametrosSugestaoItens.Add("@nQuantidade", "");

                    BS_SUGESTAO.ForEach(a =>
                    {
                        vParametrosSugestaoItens["@idProdutoSugestao"] = a.IdProdutoSugestao.ToString();
                        vParametrosSugestaoItens["@sUnidade"] = a.SUnidade.ToString();
                        vParametrosSugestaoItens["@nQuantidade"] = a.NQuantidade.ToString().Replace(",", ".");
                        ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParametrosSugestaoItens);
                    });

                    Dictionary<string, string> vParametrosFornecedores = new Dictionary<string, string>();
                    vParametrosFornecedores.Add("@sFuncao", "FORNECEDOR_DELETAR");
                    vParametrosFornecedores.Add("@idProduto", idProduto);
                    vParametrosFornecedores.Add("@idUsuarioAtualizacao", Variaveis.idUsuario());
                    DataSet dsFornecedores_Excluir = ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParametrosFornecedores);

                    vParametrosFornecedores["@sFuncao"] = "INCLUIR_FORNECEDOR";
                    vParametrosFornecedores.Add("@sDscFornecedorProduto", "");
                    vParametrosFornecedores.Add("@idParceiro", "");
                    vParametrosFornecedores.Add("@sidParceiro", "");
                    vParametrosFornecedores.Add("@nValorUnitario", "0");
                    vParametrosFornecedores.Add("@sCodigoFornecedor", "");
                    vParametrosFornecedores.Add("@idTipoFornecedor", "");

                    BS_FORNECEDORES.ForEach(a =>
                    {
                        Dictionary<string, string> FornecedoresNaoCadastrados = new Dictionary<string, string>();
                        vParametrosFornecedores["@sCnpj_CPF"] = a.sCNPJ_CPF;
                        vParametrosFornecedores["@sDscFornecedorProduto"] = a.SDscFornecedorProduto;
                        vParametrosFornecedores["@idParceiro"] = a.IdParceiro.ToString();
                        vParametrosFornecedores["@nValorUnitario"] = a.NvalorUnitario.ToString().Replace(",", ".");
                        vParametrosFornecedores["@sCodigoFornecedor"] = a.SCodigoFornecedor.ToString();
                        vParametrosFornecedores["@idTipoFornecedor"] = a.IdTipoFornecedor.ToString();
                        ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParametrosFornecedores);

                        if (ValidarDataSet(ds, out string sErroFornecedor))
                        {
                            if (sErroFornecedor == "")
                                FornecedoresNaoCadastrados.Add(a.SCodigoFornecedor, "Itens não cadastrados devido a possível inexistência de Fornecedor");
                            else
                                MensagemPagina_Fornecedor.MostraMensagem_Erro(sErroFornecedor);
                        }
                    });

                    Atualiza_Aba_Parceiro();
                    bs_Parceiros.ForEach(p =>
                    {
                        Dictionary<string, string> vParametrosParceiros = new Dictionary<string, string>()
                        {
                            { "@sFuncao", "INCLUIR_FORNECEDOR" },
                            { "@idProduto", idProduto },
                            { "@idParceiro", p.IdParceiro.ToString() },
                            { "@idTipoFornecedor", p.IdTipoFornecedor.ToString() },
                            { "@sCodigoFornecedor", p.SCodigoFornecedor },
                            { "@sDscFornecedorProduto", p.SDscFornecedorProduto },
                            { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                        };
                        ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParametrosParceiros);
                    });
                }

                bRetorno = true;

                BS_FORNECEDORES.Clear();
                bs_Parceiros.Clear();
                BS_SUGESTAO.Clear();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

            return bRetorno;
        }

        #endregion

        #region | Produtos Descricao

        void Popular_Produtos_Descricao(DataSet dsPesquisa)
        {
            bs_Produtos_Descricao.Clear();

            aba_Idiomas.Visible = false;
            if (ValidaPermissao(Permissao.Produtos.ConsultarAbaIdiomas))
            {

                foreach (DataRow row in dsPesquisa.Tables[4].Rows)
                {
                    cls_Produtos_Descricao objItem = new cls_Produtos_Descricao();

                    objItem.idRegistroDescricao = Convert.ToInt32(row["idRegistroDescricao"].ToString());
                    objItem.idProduto = Convert.ToInt32(row["idProduto"].ToString());
                    objItem.idPais = Convert.ToInt32(row["idPais"].ToString());
                    objItem.sDscPais = row["sDscPais"].ToString();
                    objItem.idTipo = Convert.ToInt32(row["idTipo"].ToString());
                    objItem.sDscTipo = row["sDscTipo"].ToString();
                    objItem.sDscDescricao = row["sDscDescricao"].ToString();
                    // objItem.dtAtualizacao = row["dtAtualizacao"].ToString();
                    bs_Produtos_Descricao.Add(objItem);
                }
                aba_Idiomas.Visible = true;
            }
            dtgProdutos_Descricao_DataBind();
            LimpaCampos_Produtos_Descricao();
        }

        protected void cmdProdutos_Descricao_Incluir_Click(object sender, EventArgs e)
        {
            string sMensagem = "";
            if (ValidarDados_Produtos_Descricao(ref sMensagem))
            {
                cls_Produtos_Descricao objItem = new cls_Produtos_Descricao();
                string idProduto = hddidProduto.Value.Split(',')[0].ToString();

                objItem.idProduto = Convert.ToInt32(idProduto);
                objItem.idPais = Convert.ToInt32(ddlidPais.SelectedValue);
                objItem.sDscPais = ddlidPais.SelectedItem.ToString();
                objItem.idTipo = Convert.ToInt32(ddlDescricao_IdTipo.SelectedValue);
                objItem.sDscTipo = ddlDescricao_IdTipo.SelectedItem.ToString();
                objItem.sDscDescricao = txtsDscDescricao.Text;
                bs_Produtos_Descricao.Add(objItem);
                dtgProdutos_Descricao_DataBind();
                LimpaCampos_Produtos_Descricao();
            }
            else
            {
                MensagemProdutos_Descricao.MostraMensagem_Erro(sMensagem, false);
            }
        }

        bool Salvar_Produtos_Descricao(string idProduto)
        {
            bool bRetorno = false;

            try
            {
                if (idProduto != "0")
                {
                    Dictionary<String, String> vParametroItensProdutosDescricao = new Dictionary<string, string>()
                    {
                        { "@sFuncao", "EXCLUIR_Produtos_Descricao" },
                        { "@idProduto", idProduto },
                        { "@idUsuarioAtualizacao", Variaveis.idUsuario()}
                    };
                    ExecutarDataSet(sProcedure, vParametroItensProdutosDescricao);

                    //Incluir
                    vParametroItensProdutosDescricao["@sFuncao"] = "INSERIR_Produtos_Descricao";
                    foreach (cls_Produtos_Descricao ProdutosDescricao_Linha in bs_Produtos_Descricao)
                    {
                        vParametroItensProdutosDescricao["idRegistroDescricao"] = ProdutosDescricao_Linha.idRegistroDescricao.ToString();
                        vParametroItensProdutosDescricao["idPais"] = ProdutosDescricao_Linha.idPais.ToString();
                        vParametroItensProdutosDescricao["idTipo"] = ProdutosDescricao_Linha.idTipo.ToString();
                        vParametroItensProdutosDescricao["sDscDescricao"] = ProdutosDescricao_Linha.sDscDescricao;
                        vParametroItensProdutosDescricao["@idUsuarioAtualizacao"] = Variaveis.idUsuario();
                        ExecutarDataSet(sProcedure, vParametroItensProdutosDescricao);
                    }

                    bRetorno = true;
                }


            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }


            return bRetorno;

        }

        private bool ValidarDados_Produtos_Descricao(ref string sMensagemErro)
        {
            bool bRetorno = true;

            if (ddlDescricao_IdTipo.SelectedValue == "0")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo da Descrição!";
            if (ddlidPais.SelectedValue == "0")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um País!";
            if (Validacoes.ValidarTexto(txtsDscDescricao))
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Descrição Inválida!";
            if (sMensagemErro != "")
                bRetorno = false;

            return bRetorno;
        }

        void LimpaCampos_Produtos_Descricao()
        {
            ddlidPais.SelectedValue = "0";
            ddlDescricao_IdTipo.SelectedValue = "0";
            txtsDscDescricao.Text = "";
        }

        void dtgProdutos_Descricao_DataBind()
        {
            dtgProdutos_Descricao.DataSource = bs_Produtos_Descricao;
            dtgProdutos_Descricao.DataBind();
        }

        [WebMethod]
        public static void SalvaEstadoCadeado(bool isLocked, string idProduto)
        {
            try
            {
                DataTable dt;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "Salvar_Cadeado");
                vParametros.Add("@sCadeado", isLocked == true ? "N" : "S");
                vParametros.Add("@idItem", idProduto);
                vParametros.Add("@idUsuarioAtualizacao", Variaveis.idUsuario());
                dt = ExecutarDataTable("sp_Manipula_tbl_Flow_Produtos", vParametros);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region | Avançar e Retornar

        protected void cmdAvancar_click(object sender, EventArgs e)
        {
            int id = 0;
            if (txtidProduto.Text != "Novo")
                id = Convert.ToInt32(txtidProduto.Text) + 1;
            else
                id = Convert.ToInt32(Request["id"]) + 1;

            Response.Redirect($"Produtos_Detalhe.aspx?id={id}");
        }

        protected void cmdRetornar_click(object sender, EventArgs e)
        {
            int id = 0;

            if (txtidProduto.Text != "Novo")
                id = Convert.ToInt32(txtidProduto.Text) - 1;
            else
                id = Convert.ToInt32(Request["id"]) - 1;

            Response.Redirect($"Produtos_Detalhe.aspx?id={id}");
        }

        #endregion

        #region | Serviços / Sub-Serviços

        protected void PesquisarServico(string idServico, bool bSub)
        {
            div_Id_Codigo_Desc_Img1.Attributes.Remove("class");
            div_Id_Codigo_Desc_Img1.Attributes.Add("class", "col-lg-11");

            div_bloquearEdicao.Attributes.Remove("class");
            div_bloquearEdicao.Attributes.Add("class", "col-lg-1");

            div_ID.Attributes.Remove("class");
            div_ID.Attributes.Add("class", "col-lg-12 row");
            div_ID1.Attributes.Remove("class");
            div_ID1.Attributes.Add("class", "form-group col-lg-2");

            div_Codigo.Attributes.Remove("class");
            div_Codigo.Attributes.Add("class", "col-lg-12 row");
            div_Codigo1.Attributes.Remove("class");
            div_Codigo1.Attributes.Add("class", "form-group col-lg-3");

            div_Desc.Attributes.Remove("class");
            div_Desc.Attributes.Add("class", "col-lg-12 row");
            div_Desc1.Attributes.Remove("class");
            div_Desc1.Attributes.Add("class", "form-group col-lg-6");

            div_Tipo.Attributes.Remove("class");
            div_Tipo.Attributes.Add("class", "col-lg-4");
                
            div_CategoriaVendas.Attributes.Remove("class");
            div_CategoriaVendas.Attributes.Add("class", "col-lg-4");


            DIV_Sistemas.Attributes.Remove("class");
            DIV_Sistemas.Attributes.Add("class", "col-lg-2");

            div_ExibeComercial_LM.Attributes.Remove("class");
            div_ExibeComercial_LM.Attributes.Add("class", "col-lg-4");

            div_ExibeComercial.Attributes.Remove("class");
            div_ExibeComercial.Attributes.Add("class", "col-lg-12");

            div_FiltroPesquisa.Attributes.Remove("class");
            div_FiltroPesquisa.Attributes.Add("class", "col-lg-9");

            div_ExibeComrcial_Composicao.Attributes.Remove("class");
            div_ExibeComrcial_Composicao.Attributes.Add("class", "col-lg-2");

            div_cmdComposicao_IncluirItem.Attributes.Remove("class");
            div_cmdComposicao_IncluirItem.Attributes.Add("class", "col-lg-1");

            lblcmdComposicao_IncluirItem.Visible = true;
            divbr2_Composicao_IncluirItem.Visible = false;
            aba_caracteristicasProduto.Visible = false;
            aba_Fornecedores.Visible = false;
            aba_Movimentacao.Visible = false;
            aba_Sugestao.Visible = false;
            aba_Documentacao.Visible = false;
            cmdSalvar.Visible = true;
            aba_Tabelas.Visible = false;


            if (!bSub)
            {
                div_codigoServicoPai.Visible = true;
                div_codigoServicoFilho.Visible = true;
                ddlsCodigoServicoPai.Attributes.Add("disabled", "disabled");
                ddlsCodigoServicoFilho.Attributes.Add("disabled", "disabled");
            }

            manual.Visible = true;

            hddsManual.Value = bSub ? "Manual-Sub_Servicos.pdf" : "Manual-Servicos.pdf";

            bAabaProduto.InnerText = bSub ? "Sub-Serviço" : "Serviço";

            string sComposicao = "";
            string sErro = "";

            LimpaCampos();
            PopularCombos(bSub ? 3 : 1);
            FiltroPesquisa.AlteraCampos_x_Tipo(0, false, false);

            try
            {
                if (idServico != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idProduto", idServico }
                    };
                    DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

                    if (ValidarDataSet(dsPesquisa, out sErro))
                    {
                        string sTipo = DATASET(dsPesquisa, 0, 0, "sValidaTipo");
                        string sSub = bSub ? "3" : "1";

                        if (sTipo == sSub)
                        {
                            Popular_Produtos_Descricao(dsPesquisa);

                            if (bSub)
                                hddidTipoFiltroPesquisa.Value = "3";

                            lblCodigo.InnerText = bSub ? "Código do Sub-Serviço" : "Código do Serviço";
                            lblDescricao.InnerText = "Descrição";
                            lblTipo.InnerText = bSub ? "Tipo de Sub-Serviço" : "Tipo de Serviço";
                            lblExibeComrcial_Composicao.InnerText = "Exibe em Comercial?";
                            panelTitle_Descricao.InnerText = bSub ? "Descrição Sub-Serviço" : "Descrição Serviço";
                            div_ImagemProduto.Visible = false;
                            div2.Visible = false;
                            div_localArmazenamento.Visible = false;
                            div_Familia.Visible = false;
                            div_paisOrigem.Visible = false;
                            div_LMO.Visible = false;
                            updPanel_CEST.Visible = false;
                            divComboAtivo2.Visible = true;
                            div_Unidade.Visible = true;

                            hddidProduto.Value = DATASET(dsPesquisa, 0, "idProduto");
                            txtidProduto.Text = DATASET(dsPesquisa, 0, "idProduto");
                            ddlTipoProduto.SelectedValue = DATASET(dsPesquisa, 0, "idTipoProduto");
                            txtsCodigo.Text = DATASET(dsPesquisa, 0, "sCodigo");
                            txtsDscProduto.Text = DATASET(dsPesquisa, 0, "sDscProduto");
                            ddlidCategoriaVendas.SelectedValue = DATASET(dsPesquisa, 0, "idCategoriaVendas");
                            ddlGrupo.SelectedValue = DATASET(dsPesquisa, 0, "idGrupo");
                            ddlsUnidade.SelectedValue = DATASET(dsPesquisa, 0, "sUnidade");
                            ddlsCodigoServicoPai.SelectedValue = DATASET(dsPesquisa, 0, "idServicoPai");
                            sCodigoServicoPai = DATASET(dsPesquisa, 0, "idServicoPai");
                            Popula_Combo(ddlsCodigoServicoFilho, "sp_Select 'Flow_CodigoServico_Filho', @idPesquisa=" + ddlsCodigoServicoPai.SelectedValue + "", "idCodigoServico", "sDscServico", false, "Selecione o Código do Serviço Filho", "0");
                            ddlsCodigoServicoFilho.SelectedValue = DATASET(dsPesquisa, 0, "idCodigoServico");
                            sCodigoServicoFilho = DATASET(dsPesquisa, "idCodigoServico");
                            ddlidServicoMunicipal.SelectedValue = DATASET(dsPesquisa, "idServicoMunicipal");
                            ddlNBS.SelectedValue = DATASET(dsPesquisa, "idNBS");
                            ddlIndOp.SelectedValue = DATASET(dsPesquisa, "idIndOp");

                            try
                            {
                                if (DATASET(dsPesquisa, 0, "sUnidade").Equals("H") && ddlsUnidade.SelectedValue == "0")
                                    ddlsUnidade.SelectedIndex = ddlsUnidade.Items.IndexOf(ddlsUnidade.Items.FindByText("Hora"));
                            }
                            catch { }

                            ddlExibeComercial.SelectedValue = DATASET(dsPesquisa, 0, "sExibeComercial");
                            ddlExibeLM.SelectedValue = DATASET(dsPesquisa, 0, "sExibeLM");
                            comboAtivoServico.Situacao_Definir(DATASET(dsPesquisa, 0, "sSituacao"));
                            txtsUnMinimaCompra.Text = DATASET(dsPesquisa, 0, "sUnMinimaCompra");

                            ddlCEST.SelectedValue = DATASET(dsPesquisa, 0, "idCEST");
                            ddlCEST_SelectedIndexChanged(null, null);

                            ddlNCM.SelectedValue = DATASET(dsPesquisa, 0, "idNCM");
                            ddlNCM_SelectedIndexChanged(null, null);

                            sComposicao = DATASET(dsPesquisa, 0, "sComposicao");

                            hddsCadeado.Value = DATASET(dsPesquisa, 0, "sCadeado");

                            PainelAtualizacao.Visible = true;
                            PainelAtualizacao.Atualizar(DATASET(dsPesquisa, 0, "dtAtualizacao"), DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                            lblTituloPagina.Text = string.Format("Editar {0} {1}", bSub ? sTituloPaginaSubServico : sTituloPaginaServico, txtsDscProduto.Text);

                            cmdSalvar.Visible = bSub ? ValidaPermissao(Permissao.Sub_Servicos.Alterar) : ValidaPermissao(Permissao.Servicos.Alterar);

                            Popular_Produtos_Descricao(dsPesquisa);

                            Popular_dtgItens(dsPesquisa);

                            cmdComposicao_Alterar.Visible = false;

                            Popular_Aba_Arquivos(idServico);
                            Popular_Aba_Historico(dsPesquisa.Tables[8]);

                            EsconderColunas(dtgItens, "Ordem");

                            if (Convert.ToBoolean(Request["duplicar"]))
                            {
                                txtidProduto.Text = "Duplicado";
                                txtsDscDescricao.Text += " - Duplicado";
                                txtsCodigo.Text += " - Duplicado";

                                lblTituloPagina.Text = string.Format("Duplicar {0} {1}", bSub ? sTituloPaginaSubServico : sTituloPaginaServico, txtsDscProduto.Text);
                            }
                        }
                        else if (sTipo == "1" && bSub)
                            DirecionaPagina(string.Format("App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}&stp={1}", idServico, sTipo));
                        else if (sTipo == "2")
                            DirecionaPagina(string.Format("App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}&stp={1}", idServico, sTipo));
                        else if (sTipo == "3" && !bSub)
                            DirecionaPagina(string.Format("App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}&stp={1}", idServico, sTipo));
                        else if (sTipo == "0")
                            DirecionaPagina(string.Format("App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}", idServico));
                    }
                    else throw new Exception(sErro);
                }
                else
                {
                    aba_Arquivos.Visible = false;
                    aba_caracteristicasProduto.Visible = false;
                    aba_Composicao.Visible = false;
                    aba_Idiomas.Visible = false;
                    aba_Fornecedores.Visible = false;
                    aba_Movimentacao.Visible = false;
                    aba_Sugestao.Visible = false;
                    aba_Documentacao.Visible = false;
                    aba_Fabricacao.Visible = false;
                    aba_Lotes.Visible = false;
                    aba_Tabelas.Visible = false;
                    aba_Historico.Visible = false;

                    lblCodigo.InnerText = bSub ? "Código do Sub-Serviço" : "Código do Serviço";
                    lblDescricao.InnerText = "Descrição";
                    lblTipo.InnerText = bSub ? "Tipo de Sub-Serviço" : "Tipo de Serviço";
                    panelTitle_Descricao.InnerText = bSub ? "Descrição Sub-Serviço" : "Descrição Serviço";
                    div_ImagemProduto.Visible = false;
                    div2.Visible = false;
                    div_localArmazenamento.Visible = false;
                    div_Familia.Visible = false;

                    div_paisOrigem.Visible = false;
                    div_LMO.Visible = false;
                    div_Unidade.Visible = true;
                    updPanel_CEST.Visible = false;
                    divComboAtivo2.Visible = true;

                    BreadCrumb.TitulodaPagina = "Incluir";
                    lblTituloPagina.Text = string.Format("Novo {0}", bSub ? sTituloPaginaSubServico : sTituloPaginaServico);
                    cmdSalvar.Text = "Incluir";
                }

                txtsCodigo.Focus();
                RegistraScript();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message + " <a href='#' onclick='retornar();'>Retornar</a> | <a href='#' onclick='avancar();'>Avançar</a>");
                if (ex.Message == "Nenhum Registro Encontrado")
                {
                    div_Cabecalho.Visible = false;
                    aba_Arquivos.Visible = false;
                    aba_caracteristicasProduto.Visible = false;
                    aba_Composicao.Visible = false;
                    aba_Idiomas.Visible = false;
                    aba_Fornecedores.Visible = false;
                    aba_Movimentacao.Visible = false;
                    aba_Sugestao.Visible = false;
                    cmdSalvar.Visible = false;
                    aba_Documentacao.Visible = false;
                }
            }
        }

        protected void ddlidServicoMunicipal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidServicoMunicipal.SelectedValue != "0")
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                   { "@sFuncao", "CONSULTA_SERVICO_FEDERAL" },
                   { "@idServico", ddlidServicoMunicipal.SelectedValue }
                };
                DataSet dsPesquisa = ExecutarDataSet("sp_Manipula_tbl_Flow_Servico_Municipal", vParametros);

                ddlsCodigoServicoPai.SelectedValue = DATASET(dsPesquisa, 0, "idServicoPai");
                sCodigoServicoPai = DATASET(dsPesquisa, 0, "idServicoPai");
                Popula_Combo(ddlsCodigoServicoFilho, "sp_Select 'Flow_CodigoServico_Filho', @idPesquisa=" + ddlsCodigoServicoPai.SelectedValue + "", "idCodigoServico", "sDscServico", false, "Selecione o Código do Serviço Filho", "0");
                ddlsCodigoServicoFilho.SelectedValue = DATASET(dsPesquisa, 0, "idCodigoServico");
                sCodigoServicoFilho = DATASET(dsPesquisa, 0, "idCodigoServico");
            }
            else
            {
                ddlsCodigoServicoPai.SelectedValue = "0";
                ddlsCodigoServicoFilho.SelectedValue = "0";
                sCodigoServicoPai = "0";
                sCodigoServicoFilho = "0";
            }
        }

        #endregion

        #region | Recursos

        protected void PesquisarRecurso(string idRecurso)
        {
            div_Id_Codigo_Desc_Img1.Attributes.Remove("class");
            div_Id_Codigo_Desc_Img1.Attributes.Add("class", "col-lg-11");

            div_bloquearEdicao.Attributes.Remove("class");
            div_bloquearEdicao.Attributes.Add("class", "col-lg-1");

            div_ID.Attributes.Remove("class");
            div_ID.Attributes.Add("class", "col-lg-12 row");
            div_ID1.Attributes.Remove("class");
            div_ID1.Attributes.Add("class", "form-group col-lg-2");

            div_Codigo.Attributes.Remove("class");
            div_Codigo.Attributes.Add("class", "col-lg-12 row");
            div_Codigo1.Attributes.Remove("class");
            div_Codigo1.Attributes.Add("class", "form-group col-lg-3");

            div_Desc.Attributes.Remove("class");
            div_Desc.Attributes.Add("class", "col-lg-12 row");
            div_Desc1.Attributes.Remove("class");
            div_Desc1.Attributes.Add("class", "form-group col-lg-6");

            div_Grupos.Attributes.Remove("class");
            div_Grupos.Attributes.Add("class", "form-group col-lg-5");

            div_Tipo.Attributes.Remove("class");
            div_Tipo.Attributes.Add("class", "col-lg-4");

            div_CategoriaVendas.Attributes.Remove("class");
            div_CategoriaVendas.Attributes.Add("class", "col-lg-4");

            div_ExibeComercial_LM.Attributes.Remove("class");
            div_ExibeComercial_LM.Attributes.Add("class", "col-lg-4");

            div_ExibeComercial.Attributes.Remove("class");
            div_ExibeComercial.Attributes.Add("class", "col-lg-12");

            div_FiltroPesquisa.Attributes.Remove("class");
            div_FiltroPesquisa.Attributes.Add("class", "col-lg-9");

            div_ExibeComrcial_Composicao.Attributes.Remove("class");
            div_ExibeComrcial_Composicao.Attributes.Add("class", "col-lg-2");

            div_cmdComposicao_IncluirItem.Attributes.Remove("class");
            div_cmdComposicao_IncluirItem.Attributes.Add("class", "col-lg-1");



            ExibirGastos.Definir("N", "Exibir Relatório de Gastos", "N");
            if (ExibirGastos.Recuperar() == "N")
            {
                DIV_CategoriaPagar.Attributes["class"] = "col-lg-10 invisivel";
            }
            else
            {
                DIV_CategoriaPagar.Attributes["class"] = "col-lg-10";
            }

            lblcmdComposicao_IncluirItem.Visible = true;
            divbr2_Composicao_IncluirItem.Visible = false;
            aba_caracteristicasProduto.Visible = false;
            aba_Fornecedores.Visible = false;
            aba_Movimentacao.Visible = false;
            aba_Sugestao.Visible = false;
            aba_Documentacao.Visible = false;
            cmdSalvar.Visible = true;
            DIV_CategoriaPagar.Visible = true;
            manual.Visible = true;



            DIV_Gastos.Visible = false;

            bAabaProduto.InnerText = "Recurso";

            LimpaCampos();
            PopularCombos(2);
            FiltroPesquisa.AlteraCampos_x_Tipo(1, false, false);

            hddsManual.Value = "Manual-Recursos.pdf";

            try
            {
                if (idRecurso != "0")
                {

                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idProduto", idRecurso }
                    };
                    dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

                    if (ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        string sTipo = DATASET(dsPesquisa, 0, 0, "sValidaTipo");

                        if (sTipo == "2")
                        {
                            if (aba_Tabelas.Visible)
                                Popular_Aba_Tabelas(idRecurso, true);

                            Popular_Produtos_Descricao(dsPesquisa);

                            lblCodigo.InnerText = "Código do Recurso";
                            lblDescricao.InnerText = "Descrição";
                            lblTipo.InnerText = "Tipo de Recurso";
                            lblExibeComrcial_Composicao.InnerText = "Exibe em Comercial?";
                            panelTitle_Descricao.InnerText = "Descrição Recurso";
                            div_ImagemProduto.Visible = false;
                            div2.Visible = false;
                            div_localArmazenamento.Visible = false;
                            div_Familia.Visible = false;
                            div_paisOrigem.Visible = false;
                            div_LMO.Visible = false;
                            updPanel_CEST.Visible = false;
                            DIV_Sistemas.Visible = false;
                            div_CategoriaVendas.Visible = false;
                            divComboAtivo2.Visible = true;
                            div_Unidade.Visible = true;

                            ExibirGastos.Definir(DATASET(dsPesquisa, 0, "sExibirGastos"), "Exibir Relatório de Gastos", "N");
                            if (ExibirGastos.Recuperar() == "N")
                            {
                                DIV_CategoriaPagar.Attributes["class"] = "col-lg-10 invisivel";
                            }
                            else
                            {
                                DIV_CategoriaPagar.Attributes["class"] = "col-lg-10";
                                ddlCategoriapagar.SelectedValue = DATASET(dsPesquisa, 0, "idCategoriaPagar");
                                txtvlrMaximo.Text = DATASET(dsPesquisa, 0, "vlrMaximo");
                            }

                            hddidProduto.Value = DATASET(dsPesquisa, 0, "idProduto");
                            txtidProduto.Text = DATASET(dsPesquisa, 0, "idProduto");
                            ddlTipoProduto.SelectedValue = DATASET(dsPesquisa, 0, "idTipoProduto");

                            ValidaTipoProduto(ddlTipoProduto.SelectedValue);

                            txtsCodigo.Text = DATASET(dsPesquisa, 0, "sCodigo");
                            txtsDscProduto.Text = DATASET(dsPesquisa, 0, "sDscProduto");
                            ddlidCategoriaVendas.SelectedValue = DATASET(dsPesquisa, 0, "idCategoriaVendas");
                            ddlGrupo.SelectedValue = DATASET(dsPesquisa, 0, "idGrupo");
                            ddlsUnidade.SelectedValue = DATASET(dsPesquisa, 0, "sUnidade");

                            try
                            {
                                if (DATASET(dsPesquisa, 0, "sUnidade").Equals("H") && ddlsUnidade.SelectedValue == "0")
                                    ddlsUnidade.SelectedIndex = ddlsUnidade.Items.IndexOf(ddlsUnidade.Items.FindByText("Hora"));
                            }
                            catch { }

                            ddlExibeComercial.SelectedValue = DATASET(dsPesquisa, 0, "sExibeComercial");
                            ddlExibeLM.SelectedValue = DATASET(dsPesquisa, 0, "sExibeLM");
                            comboAtivoServico.Situacao_Definir(DATASET(dsPesquisa, 0, "sSituacao"));

                            txtsUnMinimaCompra.Text = DATASET(dsPesquisa, 0, "sUnMinimaCompra");
                            object objSender = new object();
                            EventArgs objEventArgs = new EventArgs();
                            ddlCEST.SelectedValue = DATASET(dsPesquisa, 0, "idCEST");
                            ddlCEST_SelectedIndexChanged(objSender, objEventArgs);

                            ddlNCM.SelectedValue = DATASET(dsPesquisa, 0, "idNCM");
                            ddlNCM_SelectedIndexChanged(objSender, objEventArgs);

                            hddsCadeado.Value = DATASET(dsPesquisa, 0, "sCadeado");

                            PainelAtualizacao.Visible = true;
                            PainelAtualizacao.Atualizar(DATASET(dsPesquisa, 0, "dtAtualizacao"), DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                            lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPaginaRecurso, txtsDscProduto.Text);

                            cmdSalvar.Visible = ValidaPermissao(Permissao.Produtos_Recursos.Alterar);

                            Popular_Produtos_Descricao(dsPesquisa);

                            Popular_dtgItens(dsPesquisa);

                            cmdComposicao_Alterar.Visible = false;

                            Popular_Aba_Arquivos(idRecurso);
                            Popular_Aba_Historico(dsPesquisa.Tables[8]);

                            EsconderColunas(dtgItens, "Ordem");

                            if (Convert.ToBoolean(Request["duplicar"]))
                            {
                                txtidProduto.Text = "Duplicado";
                                txtsDscDescricao.Text += " - Duplicado";
                                txtsCodigo.Text += " - Duplicado";

                                lblTituloPagina.Text = string.Format("Duplicar {0} {1}", sTituloPaginaRecurso, txtsDscProduto.Text);
                            }
                        }
                        else if (sTipo == "1")
                        {
                            DIV_Sistemas.Visible = false;
                            DirecionaPagina(string.Format("App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}&stp={1}", idRecurso, sTipo));
                        }

                        else if (sTipo == "3")
                        {
                            DIV_Sistemas.Visible = false;
                            DirecionaPagina(string.Format("App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}&stp={1}", idRecurso, sTipo));
                        }

                        else
                            DirecionaPagina(string.Format("App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}", idRecurso));
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }
                }
                else
                {
                    aba_Arquivos.Visible = false;
                    aba_caracteristicasProduto.Visible = false;
                    aba_Composicao.Visible = false;
                    aba_Idiomas.Visible = false;
                    aba_Fornecedores.Visible = false;
                    aba_Movimentacao.Visible = false;
                    aba_Sugestao.Visible = false;
                    aba_Documentacao.Visible = false;
                    aba_Fabricacao.Visible = false;
                    aba_Lotes.Visible = false;
                    aba_Tabelas.Visible = false;
                    aba_Historico.Visible = false;
                    DIV_Sistemas.Visible = false;
                    div_CategoriaVendas.Visible = false;

                    lblCodigo.InnerText = "Código do Recurso";
                    lblDescricao.InnerText = "Descrição";
                    lblTipo.InnerText = "Tipo de Recurso";
                    panelTitle_Descricao.InnerText = "Descrição Recurso";
                    div_ImagemProduto.Visible = false;
                    div2.Visible = false;
                    div_localArmazenamento.Visible = false;
                    div_Familia.Visible = false;
                    div_paisOrigem.Visible = false;
                    div_LMO.Visible = false;
                    updPanel_CEST.Visible = false;
                    divComboAtivo2.Visible = true;
                    div_Unidade.Visible = true;

                    BreadCrumb.TitulodaPagina = "Incluir";
                    lblTituloPagina.Text = string.Format("Novo {0}", sTituloPaginaRecurso);
                    cmdSalvar.Text = "Incluir";
                }

                txtsCodigo.Focus();
                RegistraScript();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message +
                 " <a href='#' onclick='retornar();'>Retornar</a> | <a href='#' onclick='avancar();'>Avançar</a>");
                if (ex.Message == "Nenhum Registro Encontrado")
                {
                    div_Cabecalho.Visible = false;
                    aba_Arquivos.Visible = false;
                    aba_caracteristicasProduto.Visible = false;
                    aba_Composicao.Visible = false;
                    aba_Idiomas.Visible = false;
                    aba_Fornecedores.Visible = false;
                    aba_Movimentacao.Visible = false;
                    aba_Sugestao.Visible = false;
                    cmdSalvar.Visible = false;
                    aba_Documentacao.Visible = false;
                }
            }
            div_ExibeComercial_LM.Visible = false;
            div_ExibeComercial.Visible = false;
            div_LMO.Visible = false;
            Popula_EPI();
            Popular_Aba_Consumivel();
            RegistraScript_Personalizado(txtCodigo_EPI.ClientID, true);
            RegistraScript_Personalizado(txtDesc_EPI.ClientID, false);

            FiltroPesquisa2.ModificaTamanhoCampos(2, 4, 2, 2, 0);
            FiltroPesquisa2.Controle_ExibicaoCampos(new Dictionary<string, bool> { { "Valor Unitário", false } }, "1");
            FiltroPesquisa2.desligaColapso(false);
            FiltroPesquisa2.RegistrarScriptPesquisarMovimentacao();
        }

        protected void ddlTipoProduto_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidaTipoProduto(ddlTipoProduto.SelectedValue);
        }

        void ValidaTipoProduto(string idTipo)
        {
            string stp = Request["stp"];
            if (stp == "2")
            {
                var sFuncao = "VERIFICAR-TIPO";


                DataSet dsPesquisa;
                Dictionary<String, String> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", sFuncao },
                        { "@idTipoProduto", idTipo }
                    };
                dsPesquisa = ExecutarDataSet("sp_Manipula_tbl_Flow_WMS_Produtos_Tipo", vParametros);

                if (ValidarDataSet(dsPesquisa, out string sErro))
                {
                    if (DATASET(dsPesquisa, 0, 0, "sFabricacao") != "S")
                    {
                        div_ExibeComercial_LM.Visible = true;
                        DIV_Gastos.Visible = true;

                        aba_EpiConsumiveis.Visible = false;
                    }
                    else
                    {
                        div_ExibeComercial_LM.Visible = false;
                        DIV_Gastos.Visible = false;
                        if (hddidProduto.Value != "0")
                        {
                            aba_EpiConsumiveis.Visible = true;
                        }
                        else
                        {
                            aba_EpiConsumiveis.Visible = false;
                        }

                    }
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Tipo de Produto não encontrado.");
                }
            }
            else
            {

            }
        }

        #endregion

        #region | Eventos

        protected void txtsCodigoNCM_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string idNCM = "0";
                string sCodigoNCM = "";
                var localiza = BS_NCM.Where(x => x.sCodigoNCM.Replace(".", "").Equals(txtCodigoNCM.Text.Replace(".", ""))).Select(t => new { t.sCodigoNCM, t.idNCM }).ToList();
                if (localiza.Count() > 0)
                {
                    idNCM = localiza[0].idNCM.ToString();
                    sCodigoNCM = localiza[0].sCodigoNCM.ToString();
                }

                ddlNCM.SelectedValue = idNCM;
                ddlNCM_SelectedIndexChanged(sender, e);
                txtCodigoNCM.Text = sCodigoNCM;

                RegistraScript();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
            RegistraScript();
        }

        protected void ddlNCM_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sCodigoNCM = "";
            string idCEST = "0";
            var localiza = BS_NCM.Where(x => x.idNCM.ToString().Equals(ddlNCM.SelectedValue)).Select(t => new { t.idCEST, t.sCodigoNCM }).ToList();

            if (localiza.Count() > 0)
            {
                sCodigoNCM = localiza[0].sCodigoNCM.ToString();
                idCEST = localiza[0].idCEST.ToString();
            }
            txtCodigoNCM.Text = sCodigoNCM;

            if (idCEST != "0")
            {
                ddlCEST.SelectedValue = idCEST;
                ddlCEST_SelectedIndexChanged(sender, e, "NCM");
            }
            RegistraScript();

        }

        protected void ddlCEST_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlCEST_SelectedIndexChanged(sender, e, "");
        }

        protected void ddlCEST_SelectedIndexChanged(object sender, EventArgs e, string sObjeto)
        {
            string sCodigoCEST = "";
            var localiza = BS_CEST.Where(x => x.idCest.ToString().Equals(ddlCEST.SelectedValue)).Select(t => new { t.sCodigoCEST }).ToList();

            if (localiza.Count() > 0)
            {
                sCodigoCEST = localiza[0].sCodigoCEST.ToString();
            }
            txtCodigoCEST.Text = sCodigoCEST;

            if (string.IsNullOrEmpty(sObjeto))
            {
                PopularCombo_NCM(ddlCEST.SelectedValue);
            }
            RegistraScript();
        }

        protected void txtsCodigoCEST_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string idCEST = "0";
                string sCodigoCEST = "";
                var localiza = BS_CEST.Where(x => x.sCodigoCEST.Replace(".", "").Equals(txtCodigoCEST.Text.Replace(".", ""))).Select(t => new { t.idCest, t.sCodigoCEST }).ToList();

                if (localiza.Count() > 0)
                {
                    idCEST = localiza[0].idCest.ToString();
                    sCodigoCEST = localiza[0].sCodigoCEST.ToString();
                }

                ddlCEST.SelectedValue = idCEST;
                txtCodigoCEST.Text = sCodigoCEST;

                PopularCombo_NCM(idCEST);
                RegistraScript();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
            RegistraScript();
        }

        protected void ddlUnidadeProduto_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlUnidade.SelectedIndex = ddlUnidade.SelectedIndex;

        }

        protected void ddlUnidade_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlUnidade.SelectedIndex = ddlUnidade.SelectedIndex;
            UpdatePanel1.Update();

        }

        protected void txtsCodigoProduto_TextChanged(object sender, EventArgs e)
        {
            //SqlDataReader sdr = ExecutarDataReader("sp_Select 'FLOW_Produtos_Codigo', 0, 'S', '" + txtComposicao_sCodigoProduto.Text + "'");

            //while (sdr.Read())
            //{
            //    txtComposicao_sDscProduto.Text = sdr["sDscProduto"].ToString();
            //    ddlComposicao_sUnidade.SelectedValue = sdr["sUnidade"].ToString();
            //    hddComposicao_idItem.Value = sdr["idItem"].ToString();
            //    hddComposicao_sDscTipoProduto.Value = sdr["sDscTipoProduto"].ToString();
            //    txtComposicao_nQuantidade.Focus();
            //}
            //sdr.Close();
        }

        protected void cmdComposicao_IncluirItem_Click(object sender, EventArgs e)
        {
            if (Composicao_ValidarDados(Request["stp"] == null || Request["stp"] == string.Empty ? "0" : Request["stp"], FiltroPesquisa))
            {
                cls_WMS_Produtos objItem = new cls_WMS_Produtos();
                string[] vidItem = hddidProduto.Value.Split(',');
                string idItem = vidItem[0].ToString();

                if (bs_Produto_Composicao.Where(x => x.SCodigo == FiltroPesquisa.SCodigo).FirstOrDefault() != null)
                    Composicao_MensagemPagina.MostraMensagem_Aviso(Request["stp"] != null ? Request["stp"] == "1" ? "Serviço já incluso atualizado!" : "Recurso já incluso atualizado!" : "Produto já incluso atualizado!");

                if (Request["stp"] != null && Request["stp"] != string.Empty)
                {
                    objItem.IdItem = Convert.ToInt32(hddidProduto.Value);
                    objItem.IdItemComposicao = Convert.ToInt32(FiltroPesquisa.IdServico_Recurso);
                    objItem.TipoProduto = FiltroPesquisa.IDTipoServico_Recurso;
                    objItem.SCodigo = FiltroPesquisa.SCodigoServico_Recurso;
                    objItem.SDscProduto = FiltroPesquisa.SDscServico_Recurso;

                    try
                    {
                        objItem.SUnidade = FiltroPesquisa.SUnidadeServico_Recurso;
                        objItem.NQuantidade = Convert.ToDecimal(FiltroPesquisa.NQuantidade);
                    }
                    catch { }
                }
                else
                {
                    objItem.IdItem = Convert.ToInt32(hddidProduto.Value);
                    objItem.IdItemComposicao = Convert.ToInt32(FiltroPesquisa.IdItem);
                    objItem.TipoProduto = FiltroPesquisa.ddlTipoProdutoValue;
                    objItem.SCodigo = FiltroPesquisa.SCodigo;
                    objItem.SDscProduto = FiltroPesquisa.SDscProduto;

                    try
                    {
                        objItem.SUnidade = FiltroPesquisa.SUnidade;
                        objItem.NQuantidade = Convert.ToDecimal(FiltroPesquisa.NQuantidade);
                    }
                    catch { }
                }

                // Procura pelo maior valor de nOrdem na lista
                int maiorOrdem = bs_Produto_Composicao.Any() ? bs_Produto_Composicao.Max(item => item.nOrdem) : 0;

                // Incrementa o valor em 1 para obter o próximo valor de nOrdem
                objItem.nOrdem = maiorOrdem + 1;


                // Novo código para atribuir o valor do DropDownList ao novo item
                objItem.sExibePedido = ddlExibePedido.SelectedValue;

                // Procura pelo item na lista
                int index = -1;
                for (int i = 0; i < bs_Produto_Composicao.Count; i++)
                {
                    if (bs_Produto_Composicao[i].IdItemComposicao == objItem.IdItem)
                    {
                        index = i;
                        break;
                    }
                }

                // Se o item for encontrado, substitui; caso contrário, adiciona
                if (index != -1)
                {
                    // Atualiza a quantidade do item existente na lista
                    bs_Produto_Composicao[index].NQuantidade += objItem.NQuantidade;

                    // Chama o método ConverterCamposClasse apenas para o item alterado
                    bs_Produto_Composicao[index].ConverterCamposClasse(objItem);

                    cmdComposicao_Alterar.Visible = false;
                    cmdComposicao_IncluirItem.Visible = true;
                }
                else
                {
                    bs_Produto_Composicao.Add(objItem);
                    // Obtém a referência para o item recém-adicionado na lista
                    var novoItem = bs_Produto_Composicao.Last();

                    // Chama o método ConverterCamposClasse apenas para o novo item
                    novoItem.ConverterCamposClasse(novoItem);
                }

                dtgItens_DataBind();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtCompisicao_sCodigoProduto]').focus();", true);

                FiltroPesquisa.LimparCampos();
            }

            Pesquisa_Parceiros.RegistrarScriptPesquisarItens();
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddliTipoAlteracao_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            string ID_ddl = ddl.SelectedValue;

            if (ID_ddl != "0" && ID_ddl != "1")
            {
                div_Motivo.Visible = true;
                PopularCombo_Motivo(ID_ddl);
            }
            else
            {
                div_Motivo.Visible = false;
            }

        }

        protected void ddlMotivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //PopularHistoricoMovimentacao(hddidItem.Value);
        }

        protected void ddlPeriodo_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularHistoricoMovimentacao(hddidProduto.Value);
            EventArgs objEventArgs = new EventArgs();
            ddliTipoAlteracao_SelectedIndexChanged(ddliTipoAlteracao, objEventArgs);
        }

        protected void cmdIncluirSugestao_Click(object sender, EventArgs e)
        {
            if (Composicao_ValidarDados("0", FiltroPesquisa1))
            {
                cls_WMS_Produtos_Sugestao objItem = new cls_WMS_Produtos_Sugestao();
                string[] vidItem = hddidProduto.Value.Split(',');
                string idItem = vidItem[0].ToString();

                objItem.IdProduto = Convert.ToInt32(idItem);
                objItem.IdProdutoSugestao = Convert.ToInt32(FiltroPesquisa1.IdItem);
                objItem.TipoProduto = hddComposicao_sDscTipoProduto.Value.ToString();
                objItem.SCodigo = FiltroPesquisa1.SCodigo;
                objItem.SDscProduto = FiltroPesquisa1.SDscProduto;
                objItem.SUnidade = FiltroPesquisa1.SUnidade;
                objItem.NQuantidade = FiltroPesquisa1.NQuantidade;

                BS_SUGESTAO.Add(objItem);
                dtgSugestao_Databind();
                FiltroPesquisa1.LimparCampos();
                Pesquisa_Parceiros.RegistrarScriptPesquisarItens();
                FiltroPesquisa1.Focus_sCodigo();
            }
        }

        protected void cmdIncluirFornecedor_Click(object sender, EventArgs e)
        {
            if (Composicao_ValidarDados(false))
            {
                cls_WMS_Produtos_Fornecedores objItem = new cls_WMS_Produtos_Fornecedores();
                string[] vidItem = hddidProduto.Value.Split(',');
                string idItem = vidItem[0].ToString();

                objItem.IdParceiro = Convert.ToInt32(Pesquisa_Parceiros.idParceiro);
                objItem.SDscFornecedorProduto = FN_txtbsDscProdutoFornecedor.Text;
                objItem.sCNPJ_CPF = Pesquisa_Parceiros.SCnpj_CPF;
                objItem.SRazaoSocial = Pesquisa_Parceiros.SRazaoSocial;
                objItem.NvalorUnitario = cls_WMS_Produtos.ConverterStringDecimal(FN_ValorUnitario.Text.Replace(",", "."));
                objItem.IdTipoFornecedor = Convert.ToInt32(FN_ddlidTipo.SelectedValue);
                objItem.SCodigoFornecedor = FN_txtCodigoFornecedor.Text;
                objItem.STipoFornecedor = objItem.IdTipoFornecedor == 1 ? "Principal" : "Secundário";

                if (BS_FORNECEDORES.Exists(item => item.IdTipoFornecedor == 1) && (objItem.IdTipoFornecedor == 1))
                    MensagemPagina_Fornecedor.MostraMensagem_Erro("Não é possível cadastrar mais de 1 Fornecedor Principal!");
                else
                {
                    BS_FORNECEDORES.Add(objItem);
                    dtgFornecedores_Databind();
                    Pesquisa_Parceiros.LimparCampos();
                    FN_txtCodigoFornecedor.Text = "";
                    FN_txtbsDscProdutoFornecedor.Text = "";
                    FN_ValorUnitario.Text = "";
                    FN_ddlidTipo.SelectedValue = "0";
                }
            }

            Pesquisa_Parceiros.RegistrarScriptPesquisarItens();
        }

        protected void gv_Historico_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[3].Text = HttpUtility.HtmlDecode(e.Row.Cells[3].Text);
            }
        }

        #endregion

        #region | Script

        void RegistraScript()
        {
            string script = @"
                <script type='text/javascript'>
                    function retornar() {
                        // Redireciona para o evento cmdRetornar_click
                        __doPostBack('" + cmRetornar.UniqueID + @"', '');
                    }

                    function avancar() {
                        // Redireciona para o evento cmdAvancar_click
                        __doPostBack('" + cmAvancar.UniqueID + @"', '');
                    }
                </script>
            ";

            // Registra o script no final da página
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CustomScript", script, false);
            string script1 = File.ReadAllText(Server.MapPath("~/App/JS/Mascaras.js"));
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Mascaras_Envio_js", script1, true);

            StringBuilder sb = new StringBuilder();

            sb.Append("$('[id*=cbExcluir_Todos]').change(function () {\r\n");
            sb.Append("     var excluir = $(this).is(\":checked\");\r\n");
            sb.Append("     var table = $(this).closest('table');\r\n");
            sb.Append("     $(table).find('tr').each(function () {\r\n");
            sb.Append("         var $row = $(this);\r\n");
            sb.Append("         if ($row.find('[id*=cbExcluir]').length > 0) {\r\n");
            sb.Append("             if (excluir) {\r\n");
            sb.Append("                 $row.find('[id*=cbExcluir]').prop('checked', true);\r\n");
            sb.Append("             }\r\n");
            sb.Append("             else {\r\n");
            sb.Append("                 $row.find('[id*=cbExcluir]').prop('checked', false);\r\n");
            sb.Append("             }\r\n");
            sb.Append("         }\r\n");
            sb.Append("     });\r\n");
            sb.Append("});\r\n\r\n");

            // Função para que todo link na página abra seu Destino em uma nova aba
            sb.Append("$('.link').find('a').click(function() { window.open(window.location.protocol + '//' + window.location.host + '/App/Paginas/' + $(this).attr('href'), '_blank'); return false; });\r\n\r\n");

            sb.Append("$v192('[id*=ExibirGastos_idSwitch]').click(function(e) {");
            sb.Append("     var div = '#' + $(this).attr('id').replace('ExibirGastos_idSwitch', 'ExibirGastos_hddSwitch');");
            sb.Append("     $(div).val($(div).val() == 'N' ? 'S' : 'N');");
            sb.Append("     $('#" + DIV_CategoriaPagar.ClientID + "').toggleClass('invisivel');");
            sb.Append("});");

            sb.Append("$('[id*=txtvlrMaximo]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnValor]').mask('0.000.000.009,9999', { reverse: true });");
            sb.Append("$('[id*=txtnQtd]').mask('0.000.000.009,99', { reverse: true });");


            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Seleciona_Todos_Excluir", sb.ToString(), true);


            if (aba_Tabelas.Visible)
            {
                sb.Clear();

                sb.Append("$(document).ready(function () {\r\n");

                // Links para as Tabelas
                sb.Append("     $('a[href*=\"Tabelas_Detalhe.aspx\"]').attr('target', '_blank');\r\n\r\n");

                // Campos que não podem ser alterados
                sb.Append("     $('.naoAltera').on('keydown', function (e) {\r\n");
                sb.Append("         e.preventDefault();\r\n");
                sb.Append("     });\r\n");

                // CheckBox para Liberar o Item nas Tabelas de Vendas Customizadas
                sb.Append("     $('[id*=cbLiberado]').on('change', function () {\r\n");
                sb.Append("         var $row = $(this).closest('tr');\r\n");
                sb.Append("         $row.toggleClass('success danger');\r\n");
                sb.Append("     });\r\n");

                // Botões para Encolher e Expandir
                sb.Append("     $('.toggle-panel').on('click', function () {\r\n");
                sb.Append("         var $btn = $(this);\r\n");
                sb.Append("         var $panelBody = $btn.closest('.panel').find('.panel-body')[0];\r\n");
                sb.Append("         var $icon = $btn.find('span');\r\n");
                sb.Append("         $panelBody = $($panelBody);\r\n");
                sb.Append("         $panelBody.slideToggle(200, function () {\r\n");
                sb.Append("             if ($panelBody.is(':visible')) {\r\n");
                sb.Append("                 $icon.removeClass('fa-chevron-down').addClass('fa-chevron-up');\r\n");
                sb.Append("                 $('[id*=hddsTabelas_Abertas]').val($('[id*=hddsTabelas_Abertas]').val() + $btn.attr('id') + '|');\r\n");
                sb.Append("             } else {\r\n");
                sb.Append("                 $icon.removeClass('fa-chevron-up').addClass('fa-chevron-down');\r\n");
                sb.Append("                 $('[id*=hddsTabelas_Abertas]').val($('[id*=hddsTabelas_Abertas]').val().replace($btn.attr('id') + '|', ''));\r\n");
                sb.Append("             }\r\n");
                sb.Append("         });\r\n");
                sb.Append("     });\r\n");

                // Trigger para manter todos fechados quando carregar a página
                sb.Append("     $('.toggle-panel').trigger('click');\r\n");

                // Toasts para exibir mais informações
                sb.Append("     Swal.bindClickHandler();\r\n");
                sb.Append("     Swal.mixin({\r\n");
                sb.Append("         toast: true,\r\n");
                sb.Append("         position: 'center-start',\r\n");
                sb.Append("         showConfirmButton: false,\r\n");
                sb.Append("         timer: 3000,\r\n");
                sb.Append("         timerProgressBar: true,\r\n");
                sb.Append("         customClass: {\r\n");
                sb.Append("             popup: 'toastPersonalizado',\r\n");
                sb.Append("             timerProgressBar: 'barraProgresso_toastPersonalizado'\r\n");
                sb.Append("         },\r\n");
                sb.Append("         didOpen: (toast) => {\r\n");
                sb.Append("             toast.onmouseenter = Swal.stopTimer;\r\n");
                sb.Append("             toast.onmouseleave = Swal.resumeTimer;\r\n");
                sb.Append("             setTimeout(() => {\r\n");
                sb.Append("                 Swal.update({ title: 'Nesta Aba, você pode adicionar o ' + $('.tituloPagina').text().replace('Editar ', '').trim() + ' às Tabelas de Preços e alterar seus valores.' });\r\n");
                sb.Append("             }, 50);\r\n");
                sb.Append("         }\r\n");
                sb.Append("     }).bindClickHandler('data-swal-toast-explicacaoAba');\r\n");
                sb.Append("     Swal.mixin({\r\n");
                sb.Append("         toast: true,\r\n");
                sb.Append("         position: 'center-start',\r\n");
                sb.Append("         showConfirmButton: false,\r\n");
                sb.Append("         timer: 3000,\r\n");
                sb.Append("         timerProgressBar: true,\r\n");
                sb.Append("         customClass: {\r\n");
                sb.Append("             popup: 'toastPersonalizado',\r\n");
                sb.Append("             timerProgressBar: 'barraProgresso_toastPersonalizado'\r\n");
                sb.Append("         },\r\n");
                sb.Append("         didOpen: (toast) => {\r\n");
                sb.Append("             toast.onmouseenter = Swal.stopTimer;\r\n");
                sb.Append("             toast.onmouseleave = Swal.resumeTimer;\r\n");
                sb.Append("         }\r\n");
                sb.Append("     }).bindClickHandler('data-swal-toast-avisoAba');\r\n");

                // Função para Calcular o Total do Item
                sb.Append("     function calculaTotal_Item(idTipoTabela, idMoedaOrigem, Preco, nCambio, NEnvio, NLocal, NImpostos, NMargem, NFator) {");
                sb.Append("         return idTipoTabela == 5 ?");
                sb.Append("                     idMoedaOrigem == 2 ?");
                sb.Append("                         Preco * nCambio * (NImpostos / 100 + 1)");
                sb.Append("                     :");
                sb.Append("                         Preco * nCambio * ((NEnvio + NLocal) / 100 + 1)");
                sb.Append("                 :");
                sb.Append("                     idTipoTabela == 2 ?");
                sb.Append("                         idMoedaOrigem == 2 ?");
                sb.Append("                             Preco * nCambio * (NMargem / 100 + 1) * (NImpostos / 100 + 1)");
                sb.Append("                         :");
                sb.Append("                             Preco * nCambio * ((NMargem + NLocal) / 100 + 1) + (Preco * nCambio * (NEnvio / 100 + 1) * (NImpostos / 100))");
                sb.Append("                 :");
                sb.Append("                     idTipoTabela == 4 ?");
                sb.Append("                         idMoedaOrigem == 2 ?");
                sb.Append("                             Preco * nCambio * NFator * (NImpostos / 100 + 1)");
                sb.Append("                         :");
                sb.Append("                             Preco * nCambio * NFator * (NLocal / 100 + 1) + (Preco * nCambio * (NEnvio / 100 + 1) * (NImpostos / 100))");
                sb.Append("                 :");
                sb.Append("                     idTipoTabela == 1 || idTipoTabela == 11 ?");
                sb.Append("                         NFator == 0 ? Preco * nCambio");
                sb.Append("                     :");
                sb.Append("                         Preco * nCambio - (Preco * nCambio * (NFator / 100))");
                sb.Append("                 :");
                sb.Append("                     NFator == 0 ?");
                sb.Append("                         Preco * nCambio");
                sb.Append("                     :");
                sb.Append("                         Preco * nCambio * NFator;");
                sb.Append("     }");

                // Máscaras
                sb.Append("     $('.nCambio').mask('999.999,9999', { reverse: true });\r\n\r\n");
                sb.Append("     $('.nPreco, .nFator, .nDesconto, .nMargem, .nLocal, .nEnvio, .nImpostos, .nTotal, .nPreco_Zona_SD, .nPreco_Zona_ND, .nPreco_Zona_N, .nPreco_Zona_CO, .nPreco_Zona_S').mask('999.999.999,99', { reverse: true });\r\n\r\n");

                // Eventos para ativar o cálculo dos valores do Item
                sb.Append("     $('.nPreco, .nFator, .nDesconto, .nMargem, .nLocal, .nEnvio, .nPreco_Zona_SD, .nPreco_Zona_ND, .nPreco_Zona_N, .nPreco_Zona_CO, .nPreco_Zona_S').on('input', function () {\r\n");
                sb.Append("         var $this = $(this);\r\n");
                sb.Append("         var idTipoTabela = parseInt($this.data('tipo')) || 0;\r\n");
                sb.Append("         var idMoedaOrigem = parseInt($this.data('moeda')) || 0;\r\n");

                sb.Append("         var $row = $this.closest('tr');\r\n");
                sb.Append("         var nPreco = parseFloat($row.find('.nPreco').val().replace('.', '').replace(',', '.')) || 0;\r\n");
                sb.Append("         var nPreco_Zona_SD = $row.find('.nPreco_Zona_SD') && $row.find('.nPreco_Zona_SD').length ? parseFloat($row.find('.nPreco_Zona_SD').val().replace('.', '').replace(',', '.')) || 0 : 0;\r\n");
                sb.Append("         var nPreco_Zona_ND = $row.find('.nPreco_Zona_ND') && $row.find('.nPreco_Zona_ND').length ? parseFloat($row.find('.nPreco_Zona_ND').val().replace('.', '').replace(',', '.')) || 0 : 0;\r\n");
                sb.Append("         var nPreco_Zona_N = $row.find('.nPreco_Zona_N') && $row.find('.nPreco_Zona_N').length ? parseFloat($row.find('.nPreco_Zona_N').val().replace('.', '').replace(',', '.')) || 0 : 0;\r\n");
                sb.Append("         var nPreco_Zona_CO = $row.find('.nPreco_Zona_CO') && $row.find('.nPreco_Zona_CO').length ? parseFloat($row.find('.nPreco_Zona_CO').val().replace('.', '').replace(',', '.')) || 0 : 0;\r\n");
                sb.Append("         var nPreco_Zona_S = $row.find('.nPreco_Zona_S') && $row.find('.nPreco_Zona_S').length ? parseFloat($row.find('.nPreco_Zona_S').val().replace('.', '').replace(',', '.')) || 0 : 0;\r\n");
                sb.Append("         var nFator = $row.find('.nFator') && $row.find('.nFator').length ? parseFloat($row.find('.nFator').val().replace('.', '').replace(',', '.')) || 0 : 0;\r\n");
                sb.Append("         var nDesconto = $row.find('.nDesconto') && $row.find('.nDesconto').length ? parseFloat($row.find('.nDesconto').val().replace('.', '').replace(',', '.')) || 0 : 0;\r\n");
                sb.Append("         var nMargem = $row.find('.nMargem') && $row.find('.nMargem').length ? parseFloat($row.find('.nMargem').val().replace('.', '').replace(',', '.')) || 0 : 0;\r\n");
                sb.Append("         var nLocal = $row.find('.nLocal') && $row.find('.nLocal').length ? parseFloat($row.find('.nLocal').val().replace('.', '').replace(',', '.')) || 0 : 0;\r\n");
                sb.Append("         var nEnvio = $row.find('.nEnvio') && $row.find('.nEnvio').length ? parseFloat($row.find('.nEnvio').val().replace('.', '').replace(',', '.')) || 0 : 0;\r\n");
                sb.Append("         var nCambio = $row.find('.nCambio') && $row.find('.nCambio').length ? parseFloat($row.find('.nCambio').val().replace('.', '').replace(',', '.')) || 0 : 0;\r\n");
                sb.Append("         var nImpostos = $row.find('.nImpostos') && $row.find('.nImpostos').length ? parseFloat($row.find('.nImpostos').val().replace('.', '').replace(',', '.')) || 0 : 0;\r\n");
                sb.Append("         var total = calculaTotal_Item(idTipoTabela, idMoedaOrigem, nPreco, nCambio, nEnvio, nLocal, nImpostos, nMargem, idTipoTabela == 1 || idTipoTabela == 11 ? nDesconto : nFator) || 0;\r\n");
                sb.Append("         $row.find('.nTotal').val(total.toFixed(2).replace('.', ',')).trigger('input');\r\n");
                sb.Append("     });\r\n");

                // Botão para Aplicar o Valor à todas as Zonas
                sb.Append("     $('.aplicaValor_Zonas').on('click', function (e) {\r\n");
                sb.Append("         e.preventDefault();\r\n");
                sb.Append("         const $row = $(this).closest('tr');\r\n");
                sb.Append("         const $this = $(this).siblings('.nPreco_Zona_SD');\r\n");
                sb.Append("         $row.find('[id*=Preco_Zona_]').not($this).val($this.val());\r\n");
                sb.Append("     });\r\n");

                sb.Append("});\r\n\r\n");

                //Calcula total e parcial Aba Fabricação
                sb.AppendLine("var inputCalculo = '[id*=txtnValor], [id*=txtnQtd]';");
                sb.AppendLine("if ($(inputCalculo).length > 0) {");
                sb.AppendLine("     $(inputCalculo).on('blur', function () {");
                sb.AppendLine("         recalcularTotalLinha(this);");
                sb.AppendLine("     });");
                sb.AppendLine("}");

                sb.AppendLine("function recalcularTotalLinha(inputAtual) {");
                sb.AppendLine("     var $row = $(inputAtual).closest('tr');");
                sb.AppendLine("     var gridView = $(inputAtual).closest('[data-grid-type]');");
                sb.AppendLine("     var gridType = gridView.data('grid-type');");
                sb.AppendLine("     var valorText = $row.find('[id*=txtnValor]').val() || '0';");
                sb.AppendLine("     var valorUnitario = parseFloat(valorText.replace(/\\./g, '').replace(',', '.')) || 0;");
                sb.AppendLine("     var qtdText = $row.find('[id*=txtnQtd]').val() || '0';");
                sb.AppendLine("     var qtd = parseFloat(qtdText.replace(',', '.')) || 0;");

                sb.AppendLine("     var total = valorUnitario * qtd;");
                sb.AppendLine("     $row.find('.lblTotal').text('R$ ' + formatarValor(total, 4));");
                sb.AppendLine("     recalcularTotalGrid(gridType);");
                sb.AppendLine("}");

                sb.AppendLine("function recalcularTotalGrid(gridType) {");
                sb.AppendLine("     var totalGeral = 0;");
                sb.AppendLine("     var gridView = $('[data-grid-type=\"' + gridType +'\"]');");
                sb.AppendLine("     gridView.find('.lblTotal').each(function () {");
                sb.AppendLine("         var valorTexto = $(this).text().trim();");
                sb.AppendLine("         var valor = parseFloat(valorTexto.replace(/[^0-9,.-]/g, '').replace(',', '.')) || 0;");
                sb.AppendLine("         totalGeral += valor;");
                sb.AppendLine("     });");
                sb.AppendLine("     if (gridType == 'Produto')");
                sb.AppendLine("         $('#" + txtnTotalProduto.ClientID + "').val(formatarValor(totalGeral, 4));");
                sb.AppendLine("     else if (gridType == 'InsumoFerramenta')");
                sb.AppendLine("         $('#" + txtnTotalInsumoFerramenta.ClientID + "').val(formatarValor(totalGeral, 4));");
                sb.AppendLine("     else if (gridType == 'Recursos')");
                sb.AppendLine("         $('#" + txtnTotalRecursos.ClientID + "').val(formatarValor(totalGeral, 4));");
                sb.AppendLine("     ");
                sb.AppendLine("     var totalProduto = parseFloat($('#" + txtnTotalProduto.ClientID + "').val().replace('.','').replace(',', '.')) || 0;");
                sb.AppendLine("     var totalInsumo = parseFloat($('#" + txtnTotalInsumoFerramenta.ClientID + "').val().replace('.','').replace(',', '.')) || 0;");
                sb.AppendLine("     var totalRecursos = parseFloat($('#" + txtnTotalRecursos.ClientID + "').val().replace('.','').replace(',', '.')) || 0;");
                sb.AppendLine("     var totalFabricacao = totalProduto + totalInsumo + totalRecursos;");
                sb.AppendLine("     $('#" + txtnTotalFabricacao.ClientID + "').val(formatarValor(totalFabricacao, 4));");
                sb.AppendLine("}");
                sb.AppendLine("function recalcularTodasGrids() {");
                sb.AppendLine("     var grids = ['Produto', 'InsumoFerramenta', 'Recursos'];");
                sb.AppendLine("     grids.forEach(function(gridType) {");
                sb.AppendLine("         if ($('[data-grid-type=\"' + gridType + '\"]').length > 0) {");
                sb.AppendLine("             recalcularTotalGrid(gridType);");
                sb.AppendLine("         }");
                sb.AppendLine("     });");
                sb.AppendLine("}");
                sb.AppendLine("function formatarValor(numero, casaDecimal) {");
                sb.AppendLine("     if (isNaN(numero)) return '0,00';");
                sb.AppendLine("     let valor = parseFloat(numero).toFixed(casaDecimal);");
                sb.AppendLine("     let [inteira, decimal] = valor.split('.');");
                sb.AppendLine("     inteira = inteira.replace(/\\B(?=(\\d{3})+(?!\\d))/g, '.');");
                sb.AppendLine("     return inteira + ',' + decimal;");
                sb.AppendLine("}");
                //sb.AppendLine("$(document).ready(function() { recalcularTodasGrids(); });");
                sb.AppendLine("function pageLoad() { recalcularTodasGrids(); }");

                sb.Append("    $('.composicaoLinha').each(function() {\r\n");
                sb.Append("        var divId = $(this).data('div-id');\r\n");
                sb.Append("        $('#' + divId).show();\r\n");
                sb.Append("        $(this).removeClass('fa fa-plus').addClass('fa fa-minus');\r\n");
                sb.Append("    });\r\n");
                sb.Append("    $('.composicaoLinha').click(function() {\r\n");
                sb.Append("        var icon = $(this);\r\n");
                sb.Append("        var divId = $(this).data('div-id');\r\n");
                sb.Append("        var current = $('#' + divId).css('display');\r\n");
                sb.Append("        if (current == 'none') {\r\n");
                sb.Append("            $('#' + divId).show('slow');\r\n");
                sb.Append("            icon.removeClass('fa fa-plus').addClass('fa fa-minus');\r\n");
                sb.Append("        } else {\r\n");
                sb.Append("            $('#' + divId).hide('slow');\r\n");
                sb.Append("            icon.removeClass('fa fa-minus').addClass('fa fa-plus');\r\n");
                sb.Append("        }\r\n");
                sb.Append("        return false;\r\n");
                sb.Append("    });\r\n");


                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Aba_Tabelas", sb.ToString(), true);
            }

            Scripts.AplicaMultiSelect(Page, lstParceiros.ID, false, "Selecione um Parceiro", "Parceiros", "Todos os Parceiros", true);
        }

        #endregion

        #region | Gambiarra de leve

        protected void cmdGAMBIARRA_Click(object sender, EventArgs e)
        {
            Scripts.EsconderCampo(Page, cmdGAMBIARRA.ID, false);

            Composicao_ValidarDados("999", FiltroPesquisa);

            if (Request["msg"] == "1")
            {
                MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!");
                if (SwitchAtivoGarantia.Recuperar() != "S")
                {
                    if (int.TryParse(nTempoGarantia.Text, out int nTempo) && nTempo > 0)
                        MensagemPagina.MostraMensagem("<b>Lembrete:</b> O Tempo de Garantia foi armazenado caso a garantia seja reativada", "info", false);
                }
            }



        }

        #endregion

        #region | Aba Movimentação

        protected void dtgMovimentacaoItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (DataBinder.Eval(e.Row.DataItem, "idStatus").ToString() != "4")
                {
                    e.Row.Cells[7].Font.Bold = false;
                    e.Row.Cells[7].Text = "-";

                }

                if (DataBinder.Eval(e.Row.DataItem, "sDscTipo").ToString().Trim() == "S")
                {
                    e.Row.Cells[4].ForeColor = Color.Red;

                }

                if (Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "nSaldo").ToString()) < 0)
                {
                    e.Row.Cells[7].ForeColor = Color.Red;

                }


                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }
        }

        public void Movimentacao_cmdConsultar_Click(object sender, EventArgs e)
        {
            PopularHistoricoMovimentacao(hddidProduto.Value);
        }

        #endregion

        #region | Aba Documentação

        private void Popular_Aba_Documentacao(DataSet dsPesquisa, int tabela)
        {
            if (dsPesquisa.Tables[tabela].Rows.Count > 0)
            {
                foreach (DataRow row in dsPesquisa.Tables[tabela].Rows)
                {
                    cls_WMS_Produtos_Documentacao objItem = new cls_WMS_Produtos_Documentacao();
                    objItem.idLinha = idLinhaConstante + 1;
                    objItem.idDocumentacao = row["idDocumentacao"].ToString();
                    objItem.idProduto = Convert.ToInt32(row["idProduto"]);
                    objItem.sDscTipo = row["sDscTipo"].ToString();
                    objItem.sDscIdioma = row["sDscIdioma"].ToString();
                    objItem.sDscEndereco = row["sDscEndereco"].ToString();
                    objItem.sListarOrcamento = row["sListarOrcamento"].ToString();

                    bs_Produtos_Documentacao.Add(objItem);
                    idLinhaConstante++;

                }
                //AtualizarGrid();

            }
        }

        void AtualizarGrid()
        {

            dtgv_Documentacao.DataSource = bs_Produtos_Documentacao;
            dtgv_Documentacao.DataBind();

            GridViewHelper helper = new GridViewHelper(dtgv_Documentacao);
            helper.GroupHeader += new GroupEvent(helper_GroupHeader_Documentacao);
            helper.RegisterGroup("sDscIdioma", true, true);

            helper.ApplyGroupSort();
        }

        private void helper_GroupHeader_Documentacao(string groupName, object[] values, GridViewRow row)
        {
            if (groupName == "sDscIdioma")
            {

                foreach (TableCell cell in row.Cells)
                {
                    cell.Controls.Clear();
                    cell.Text = string.Empty;
                }

                row.Cells[0].Text = values[0].ToString().ToUpper();
                row.BackColor = System.Drawing.Color.Green;
                row.Cells[0].Font.Bold = true;
                row.Cells[0].ForeColor = Color.White;
                row.Cells[0].HorizontalAlign = HorizontalAlign.Left;



                if (row.Cells.Count >= dtgv_Documentacao.Columns.Count)
                {
                    row.Cells[0].ColumnSpan = dtgv_Documentacao.Columns.Count - 1;

                    for (int i = 1; i < dtgv_Documentacao.Columns.Count - 1; i++)
                    {
                        if (i < row.Cells.Count)
                        {
                            row.Cells[i].Visible = false;
                        }
                    }
                }
            }

        }

        protected void btnDocumentacaoIncluir_Click(object sender, EventArgs e)
        {
            if (hddGato.Value == "1")
            {
                MensagemPagina_Documentacao.MostraMensagem_Erro("");
            }
            else
            {
                if (ValidarDadosDocumentacao())
                {

                    cls_WMS_Produtos_Documentacao objItem = new cls_WMS_Produtos_Documentacao();
                    objItem.idLinha = idLinhaConstante + 1;
                    objItem.idDocumentacao = hddidDocumentacao.Value != "" ? hddidDocumentacao.Value : "0";
                    objItem.idProduto = Convert.ToInt32(hddidProduto.Value);
                    objItem.sDscTipo = ddlDocumentacaoTipo.SelectedItem.Text;
                    objItem.sDscIdioma = ddlDocumentacaoIdioma.SelectedItem.Text;
                    objItem.sDscEndereco = txtEnderecoLink.Text;
                    objItem.sListarOrcamento = cbsListarOrcamento.Checked ? "S" : "N";

                    bs_Produtos_Documentacao.Add(objItem);
                    idLinhaConstante++;

                    LimpaCamposDocumentacao();
                }

            }
            hddGato.Value = "0";
            hddidDocumentacao.Value = "";
            dtgv_Documentacao.DataSource = bs_Produtos_Documentacao.OrderBy(doc => doc.sDscIdioma).ToList();
            dtgv_Documentacao.DataBind();

        }

        private void LimpaCamposDocumentacao()
        {
            ddlDocumentacaoIdioma.SelectedValue = "0";
            ddlDocumentacaoTipo.SelectedValue = "0";
            txtEnderecoLink.Text = "";
            cbsListarOrcamento.Checked = false;
        }

        private bool ValidarDadosDocumentacao()
        {
            bool bRetorno = true;
            string sMensagemErro = "";
            if (bs_Produtos_Documentacao.Any(item => item.sDscTipo == ddlDocumentacaoTipo.SelectedItem.Text && item.sDscIdioma == ddlDocumentacaoIdioma.SelectedItem.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Esse Tipo já foi adicionado nesse idioma!";
            }
            if (ddlDocumentacaoTipo.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Tipo!";
            }
            if (ddlDocumentacaoIdioma.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Idioma!";
            }
            if (txtEnderecoLink.Text.Length < 3)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Endereço deve ter mais que 3 caracteres";
            }
            if (cbsListarOrcamento.Checked == true)
            {
                string idioma = ddlDocumentacaoIdioma.SelectedItem.Text;
                foreach (var item in bs_Produtos_Documentacao)
                {
                    if (item.sDscIdioma == idioma && item.sListarOrcamento == "S")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Cada idioma pode ter somente um Tipo selecionado para Listar em Orçamento";
                    }
                }
            }
            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPagina_Documentacao.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;

        }

        protected void dtgv_Documentacao_Sorting(object sender, GridViewSortEventArgs e)
        {

            DataTable dataTable = ConvertToDataTable(bs_Produtos_Documentacao);

            if (dataTable != null)
            {
                DataView m_DataView = new DataView(dataTable);
                m_DataView.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(e.SortDirection);

                dtgv_Documentacao.DataSource = m_DataView;
                dtgv_Documentacao.DataBind();
            }

        }

        private string ConvertSortDirectionToSql(SortDirection sortDirection)
        {

            return sortDirection == SortDirection.Ascending ? "ASC" : "DESC";
        }

        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }

            return table;
        }

        protected void lnkDescricao_Excluir_Click(object sender, EventArgs e)
        {
            if (hddsCadeado.Value == "N")
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                HiddenField hddidLinha = (HiddenField)row.FindControl("hddidLinha");
                string idLinha = hddidLinha.Value;

                var item = bs_Produtos_Documentacao.FirstOrDefault(x => x.idLinha.ToString() == idLinha);

                if (item.idDocumentacao != "0")
                    ExcluirDocumentacao(item.idDocumentacao);

                bs_Produtos_Documentacao.Remove(item);
            }
            dtgv_Documentacao.DataSource = bs_Produtos_Documentacao.OrderBy(doc => doc.sDscIdioma).ToList();
            dtgv_Documentacao.DataBind();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_detalhe", "$('#documentacao-tab').tab('show');", true);
        }

        void ExcluirDocumentacao(string idDocumentacao)
        {
            DataTable tb;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "EXCLUIR_DOCUMENTACAO");
            vParametros.Add("@idDocumentacao", idDocumentacao);
            vParametros.Add("@idUsuarioAtualizacao", Variaveis.idUsuario());
            tb = ExecutarDataTable(sProcedure, vParametros);

        }

        protected void lnkDescricao_Editar_Click(object sender, EventArgs e)
        {
            if (hddsCadeado.Value == "N")
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                HiddenField hddidLinha = (HiddenField)row.FindControl("hddidLinha");
                string idLinha = hddidLinha.Value;

                var itemDocumentacao = bs_Produtos_Documentacao.FirstOrDefault(x => x.idLinha.ToString() == idLinha);
                if (itemDocumentacao != null)
                {
                    foreach (ListItem item in ddlDocumentacaoTipo.Items)
                    {
                        if (item.Text.Equals(itemDocumentacao.sDscTipo, StringComparison.OrdinalIgnoreCase))
                        {
                            ddlDocumentacaoTipo.ClearSelection();
                            item.Selected = true;
                            break;
                        }
                    }

                    foreach (ListItem item in ddlDocumentacaoIdioma.Items)
                    {
                        if (item.Text.Equals(itemDocumentacao.sDscIdioma, StringComparison.OrdinalIgnoreCase))
                        {
                            ddlDocumentacaoIdioma.ClearSelection();
                            item.Selected = true;
                            break;
                        }
                    }

                    txtEnderecoLink.Text = itemDocumentacao.sDscEndereco.ToString();
                    cbsListarOrcamento.Checked = itemDocumentacao.sListarOrcamento == "S" ? true : false;

                    hddidDocumentacao.Value = itemDocumentacao.idDocumentacao;

                    bs_Produtos_Documentacao.Remove(itemDocumentacao);
                }
            }
            dtgv_Documentacao.DataSource = bs_Produtos_Documentacao.OrderBy(doc => doc.sDscIdioma).ToList();
            dtgv_Documentacao.DataBind();
        }

        protected void dtgv_Documentacao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                Literal litListarOrcamento = e.Row.FindControl("litListarOrcamento") as Literal;
                if (litListarOrcamento != null)
                {
                    GridViewRow row = (GridViewRow)litListarOrcamento.NamingContainer;
                    HiddenField hddidLinha = (HiddenField)row.FindControl("hddIdLinha");
                    string idLinha = hddidLinha.Value;
                    var item = bs_Produtos_Documentacao.FirstOrDefault(x => x.idLinha.ToString() == idLinha);

                    if (item.sListarOrcamento == "S")
                    {
                        litListarOrcamento.Text = "<i class='fa fa-check'></i>";
                    }
                    else
                    {
                        litListarOrcamento.Text = string.Empty;
                    }
                }

            }
        }

        #endregion

        #region | Aba Parceiros

        protected void Atualiza_Aba_Parceiro()
        {
            try
            {
                if (Request["stp"] != "1" && Request["stp"] != "2" && Request["stp"] != "3" && bs_Parceiros.Count > 0)
                {
                    foreach (GridViewRow row in gvClientes.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            var item = bs_Parceiros.Where(p => p.IdParceiro.Equals(int.Parse(row.Cells[0].Text))).FirstOrDefault();

                            if (item != null)
                            {
                                item.SCodigoFornecedor = (row.FindControl("txtCodigo") as TextBox).Text;
                                item.SDscFornecedorProduto = (row.FindControl("txtsDsc") as TextBox).Text;
                            }
                        }
                    }

                    gvClientes_DataBind();
                }
            }
            catch { }
        }

        protected void Popular_Aba_Parceiros(DataTable dt)
        {
            bs_Parceiros.Clear();

            foreach (DataRow item in dt.Rows)
            {
                if (item.Field<string>("sCnpj_CPF").Trim().Length > 0)
                {
                    string cnpj = item.Field<string>("sCnpj_CPF").Trim();
                    string cnpjFormatado = cnpj.Replace(".", "").Replace("-", "").Replace("/", "").Length == 14 ? Convert.ToInt64(cnpj.Replace(".", "").Replace("-", "").Replace("/", "")).ToString(@"00\.000\.000\/0000\-00") : cnpj.Replace(".", "").Replace("-", "").Replace("/", "").Length == 11 ? Convert.ToInt64(cnpj.Replace(".", "").Replace("-", "").Replace("/", "")).ToString(@"000\.000\.000\-00") : cnpj;
                    int idParceiro = 0;

                    try
                    {
                        idParceiro = int.Parse(item.Field<string>("idParceiro"));
                    }
                    catch
                    {
                        idParceiro = item.Field<int>("idParceiro");
                    }

                    if (idParceiro > 0)
                    {
                        cls_WMS_Produtos_Fornecedores produto = new cls_WMS_Produtos_Fornecedores()
                        {
                            IdParceiro = idParceiro,
                            IdTipoFornecedor = 99,
                            sCNPJ_CPF = cnpjFormatado,
                            SRazaoSocial = item.Field<string>("sRazaoSocial"),
                            SDscFornecedorProduto = item.Field<string>("sDscFornecedorProduto"),
                            SCodigoFornecedor = item.Field<string>("sCodigoFornecedor")
                        };

                        bs_Parceiros.Add(produto);
                    }
                }
            }

            gvClientes_DataBind();
        }

        protected void gvClientes_DataBind()
        {
            gvClientes.DataSource = bs_Parceiros;
            gvClientes.DataBind();
        }

        protected void cmdIncluir_DadosCliente_Click(object sender, EventArgs e)
        {
            try
            {
                Atualiza_Aba_Parceiro();

                if (Composicao_ValidarDados(true))
                {
                    foreach (string idParceiro in hdd_IncluirParceiros.Value.Split('|'))
                    {
                        int id = idParceiro.Length > 0 ? int.Parse(idParceiro.Split(';')[0].Trim()) : bs_Parceiros.First().IdParceiro;

                        if (bs_Parceiros.Where(p => p.IdParceiro.Equals(id)).FirstOrDefault() == null)
                        {
                            cls_WMS_Produtos_Fornecedores objItem = new cls_WMS_Produtos_Fornecedores();

                            objItem.IdParceiro = id;
                            objItem.IdTipoFornecedor = 99;
                            objItem.sCNPJ_CPF = idParceiro.Split(';')[1].Trim();
                            objItem.SRazaoSocial = idParceiro.Split(';')[2].Trim();
                            objItem.SDscFornecedorProduto = Clientes_txtsDscProduto.Text;
                            objItem.SCodigoFornecedor = Clientes_txtCodigoProduto.Text;

                            bs_Parceiros.Add(objItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Clientes.MostraMensagem_Erro("Erro ao Incluir: " + ex.Message, false);
            }

            gvClientes_DataBind();

            Clientes_txtCodigoProduto.Text = string.Empty;
            Clientes_txtsDscProduto.Text = string.Empty;

            foreach (ListItem item in lstParceiros.Items)
                item.Selected = false;
        }

        protected void cmdExcluir_Selecionados_Click(object sender, EventArgs e)
        {
            if (hddsCadeado.Value == "N")
            {
                try
                {
                    int nParceiros = bs_Parceiros.Count;
                    bool bSelecionados = false;

                    foreach (GridViewRow row in gvClientes.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            CheckBox cb = row.FindControl("cbExcluir") as CheckBox;

                            if (cb != null && cb.Checked)
                            {
                                int idParceiro = int.Parse(row.Cells[0].Text);

                                bs_Parceiros.Remove(bs_Parceiros.Where(p => p.IdParceiro.Equals(idParceiro)).First());
                            }
                            else
                                bSelecionados = true;
                        }
                    }

                    if (nParceiros.Equals(bs_Parceiros.Count) && bSelecionados)
                        MensagemPagina_Clientes.MostraMensagem_Erro("É necessário selecionar ao menos 1 Parceiro para Excluir!", true);
                }
                catch (Exception ex)
                {
                    MensagemPagina_Clientes.MostraMensagem_Erro("Houve um erro na tentativa de Excluir os Parceiros Selecionados!<br />Erro ao Excluir: " + ex.Message, true);
                }
            }

            Atualiza_Aba_Parceiro();
            gvClientes_DataBind();
        }

        protected void gvClientes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (hddsCadeado.Value == "N")
            {
                int idParceiro = int.Parse(gvClientes.Rows[e.RowIndex].Cells[0].Text);

                bs_Parceiros.Remove(bs_Parceiros.Where(p => p.IdParceiro.Equals(idParceiro)).First());
            }
            Atualiza_Aba_Parceiro();
            gvClientes_DataBind();
        }

        #endregion

        #region | Aba Lotes

        void Popular_Aba_Lotes(string idProduto)
        {
            DataSet dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>
            {
               { "@sFuncao", "CONSULTAR-LOTES" },
               { "@idProduto", idProduto }
            };

            dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

            if (ValidarDataSet(dsPesquisa))
            {
                lblInfo.Text = $"<b>Atenção!</b> Um Total de <b>{DATASET(dsPesquisa, "nSeriesCadastradas")}</b> lotes estão cadastrados de <b>{Convert.ToInt32(decimal.Parse(hddnQtdSeries.Value.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture))}</b> em Estoque.";
                dtgLotes.DataSource = dsPesquisa;
                dtgLotes.DataBind();
            }
            else
            {
                if (SwitchAtivoSerivalizavel.Recuperar() == "S")
                {
                    lblInfo.Text = $"<b>Atenção!</b> Nenhum Lote Cadastrado para esse Produto! de <b>{Convert.ToInt32(decimal.Parse(hddnQtdSeries.Value.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture))}</b> em estoque";
                }
            }
        }

        #endregion

        #region | Aba Fabricação

        void Popular_Aba_Fabricacao(DataSet dsPesquisa)
        {
            bs_Fabricacao_Produto.Clear();
            bs_Fabricacao_InsumoFerramenta.Clear();
            bs_Fabricacao_Recurso.Clear();
            bs_Fabricacao_Processo.Clear();

            if (ValidaPermissao(Permissao.Produtos.ConsultarAbaFabricacao))
            {
                aba_Fabricacao.Visible = true;

                if (dsPesquisa.Tables[9].Rows.Count > 0) //Produto
                {
                    foreach (DataRow row in dsPesquisa.Tables[9].Rows)
                    {
                        cls_WMS_Fabricacao_Produto objItem = new cls_WMS_Fabricacao_Produto();
                        objItem.idFabricacaoProduto = Convert.ToInt32(row["idFabricacaoProduto"]);
                        objItem.idProduto = Convert.ToInt32(row["idProduto"]);
                        objItem.idTipo = Convert.ToInt32(row["idTipo"]);
                        objItem.sCodigo = row["sCodigoProduto"].ToString();
                        objItem.sDescricao = row["sDscProduto"].ToString();
                        objItem.nOrdem = Convert.ToInt32(row["nOrdem"]);
                        objItem.sTipo = row["sTipoProduto"].ToString();
                        objItem.sUnidade = row["sUnidade"].ToString();
                        objItem.nQtd = Convert.ToDecimal(row["nQuantidadeProduto"]);
                        objItem.nValorUnitario = Convert.ToDecimal(row["nValorUnitario"]);
                        objItem.nTotal = Convert.ToDecimal(row["nValorUnitario"]) * Convert.ToDecimal(row["nQuantidadeProduto"]);
                        objItem.idTabela = Convert.ToInt32(row["idTabela"]);

                        bs_Fabricacao_Produto.Add(objItem);
                    }

                    dtgv_fabricacao_produto_tipo.DataSource = GetFabricacaoPorTipoProduto(bs_Fabricacao_Produto);
                    dtgv_fabricacao_produto_tipo.DataBind();
                }

                if (dsPesquisa.Tables[10].Rows.Count > 0) //Recurso
                {
                    foreach (DataRow row in dsPesquisa.Tables[10].Rows)
                    {
                        cls_WMS_Fabricacao_Recurso objItem = new cls_WMS_Fabricacao_Recurso();
                        objItem.idFabricacaoRecurso = Convert.ToInt32(row["idFabricacaoRecurso"]);
                        objItem.idProduto = Convert.ToInt32(row["idProduto"]);
                        objItem.sCodigo = row["sCodigoRecurso"].ToString();
                        objItem.sDescricao = row["sDscRecurso"].ToString();
                        objItem.sUnidade = row["sUnidade"].ToString();
                        objItem.nQtd = Convert.ToDecimal(row["nQuantidade"]);
                        objItem.nOrdem = Convert.ToInt32(row["nOrdem"]);
                        objItem.sTipo = row["sTipoRecurso"].ToString();
                        objItem.nValorUnitario = Convert.ToDecimal(row["nValorUnitario"]);
                        objItem.nTotal = Convert.ToDecimal(row["nValorUnitario"]) * Convert.ToDecimal(row["nQuantidade"]);
                        objItem.idTabela = Convert.ToInt32(row["idTabela"]);

                        bs_Fabricacao_Recurso.Add(objItem);
                    }

                    dtg_fabricacao_recurso_DataBind();
                }

                if (dsPesquisa.Tables[11].Rows.Count > 0) //Processo
                {
                    idLinhaConstante = 0;
                    foreach (DataRow row in dsPesquisa.Tables[11].Rows)
                    {
                        cls_WMS_Fabricacao_Processo objItem = new cls_WMS_Fabricacao_Processo();
                        objItem.idLinha = idLinhaConstante + 1;
                        objItem.idFabricacaoProcesso = Convert.ToInt32(row["idFabricacaoProcesso"]);
                        objItem.idProduto = Convert.ToInt32(row["idProduto"]);
                        objItem.nOrdem = Convert.ToInt32(row["nOrdem"]);
                        objItem.sDescricao = row["sDscProcesso"].ToString();
                        objItem.idRecurso = Convert.ToInt32(row["idRecurso"]);


                        bs_Fabricacao_Processo.Add(objItem);
                        idLinhaConstante++;
                    }

                    dtg_fabricacao_processo_DataBind();
                }

                if (dsPesquisa.Tables[13].Rows.Count > 0) //Insumos e Ferramentas
                {
                    foreach (DataRow row in dsPesquisa.Tables[13].Rows)
                    {
                        cls_WMS_Fabricacao_InsumoFerramenta objItem = new cls_WMS_Fabricacao_InsumoFerramenta();
                        objItem.idFabricacaoInsumoFerramenta = Convert.ToInt32(row["idFabricacaoInsumoFerramenta"]);
                        objItem.idProduto = Convert.ToInt32(row["idProduto"]);
                        objItem.nOrdem = Convert.ToInt32(row["nOrdem"]);
                        objItem.sCodigo = row["sCodigoInsumoFerramenta"].ToString();
                        objItem.sDescricao = row["sDscInsumoFerramenta"].ToString();
                        objItem.sTipo = row["sTipoInsumoFerramenta"].ToString();
                        objItem.sUnidade = row["sUnidade"].ToString();
                        objItem.nQtd = Convert.ToDecimal(row["nQuantidadeInsumoFerramenta"]);
                        objItem.nValorUnitario = Convert.ToDecimal(row["nValorUnitario"]);
                        objItem.nTotal = Convert.ToDecimal(row["nValorUnitario"]) * Convert.ToDecimal(row["nQuantidadeInsumoFerramenta"]);
                        objItem.idTabela = Convert.ToInt32(row["idTabela"]);

                        bs_Fabricacao_InsumoFerramenta.Add(objItem);
                    }

                    gv_fabricacao_insumo_ferramenta_DataBind();
                }

                ddlsUnidadeRecurso.SelectedValue = "HH";
                ddlsUnidadeRecurso.Attributes.Add("disabled", "disabled");

                AtualizarTotalGeral();
            }
        }

        private List<cls_WMS_Fabricacao_Produto_Tipo> GetFabricacaoPorTipoProduto(List<cls_WMS_Fabricacao_Produto> detalhes)
        {
            var detalhesPorTipo = detalhes
                .GroupBy(d => d.idTipo)
                .ToDictionary(g => g.Key, g => g.ToList());

            var tipoUnicos = detalhes
                .GroupBy(d => d.idTipo)
                .Select(g => new cls_WMS_Fabricacao_Produto_Tipo
                {
                    idTipo = g.Key,
                    sTipo = g.First().sTipo,
                    nTotalTipo = g.Sum(d => d.nTotal),
                    ls_FabricacaoProduto = detalhesPorTipo[g.Key]
                })
                .ToList();

            return tipoUnicos;
        }

        private void AtualizarTotalGeral()
        {
            decimal nTotalProduto = Convert.ToDecimal(bs_Fabricacao_Produto.Sum(item => item.nTotal));
            decimal nTotalInsumoFerramenta = Convert.ToDecimal(bs_Fabricacao_InsumoFerramenta.Sum(item => item.nTotal));
            decimal nTotalRecursos = Convert.ToDecimal(bs_Fabricacao_Recurso.Sum(item => item.nTotal));
            decimal nTotalFabricacao = nTotalProduto + nTotalInsumoFerramenta + nTotalRecursos;

            txtnTotalProduto.Text = nTotalProduto.ToString("N4");
            txtnTotalInsumoFerramenta.Text = nTotalInsumoFerramenta.ToString("N4");
            txtnTotalRecursos.Text = nTotalRecursos.ToString("N4");
            txtnTotalFabricacao.Text = nTotalFabricacao.ToString("N4");
        }

        private string FabricacaoSalvar()
        {
            try
            {
                int sErro = 0;
                if (AtualizarTodasSecoesFabricacaoProduto("1"))
                {
                    foreach (var item in bs_Fabricacao_Produto)
                    {
                        Dictionary<string, string> vParametrosFabricacao = new Dictionary<string, string>
                        {
                            { "@sFuncao", "Salvar_Fabricacao" },
                            { "@sTipo", "1" },
                            { "@idFabricacaoProduto", item.idFabricacaoProduto.ToString()},
                            { "@idProduto", item.idProduto.ToString() },
                            { "@sCodigo", item.sCodigo },
                            { "@sDscProduto", item.sDescricao },
                            { "@sUnidade", item.sUnidade },
                            { "@nQuantidade", item.nQtd.ToString().Replace(",",".") },
                            { "@nOrdem", item.nOrdem.ToString() },
                            { "@idTipoProduto", item.idTipo.ToString() },
                            { "@nValorUnitario", item.nValorUnitario.ToString().Replace(".","").Replace(",",".")  },
                            { "@idTabela", item.idTabela.ToString() },
                            { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                        };
                        DataSet ds = ExecutarDataSet(sProcedure, vParametrosFabricacao);
                    }
                }
                else
                {
                    sErro = 1;
                }

                if (AtualizarOrdemFabricacao(gv_fabricacao_insumo_ferramenta, "idFabricacaoInsumoFerramenta", "2"))
                {
                    foreach (var item in bs_Fabricacao_InsumoFerramenta)
                    {
                        Dictionary<string, string> vParametrosFabricacao = new Dictionary<string, string>
                        {
                            { "@sFuncao", "Salvar_Fabricacao" },
                            { "@sTipo", "4" },
                            { "@idFabricacaoInsumoFerramenta", item.idFabricacaoInsumoFerramenta.ToString()},
                            { "@idProduto", item.idProduto.ToString() },
                            { "@sCodigo", item.sCodigo },
                            { "@sDscProduto", item.sDescricao },
                            { "@sUnidade", item.sUnidade },
                            { "@nQuantidade", item.nQtd.ToString().Replace(",",".") },
                            { "@nOrdem", item.nOrdem.ToString() },
                            { "@idTipoProduto", item.idTipo.ToString() },
                            { "@nValorUnitario", item.nValorUnitario.ToString().Replace(".","").Replace(",",".")  },
                            { "@idTabela", item.idTabela.ToString() },
                            { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                        };
                        DataSet ds = ExecutarDataSet(sProcedure, vParametrosFabricacao);
                    }
                }
                else
                {
                    sErro = 1;
                }

                if (AtualizarOrdemFabricacao(dtg_fabricacao_recurso, "idFabricacaoRecurso", "3"))
                {
                    foreach (var item in bs_Fabricacao_Recurso)
                    {
                        Dictionary<string, string> vParametrosFabricacao = new Dictionary<string, string>
                        {
                            { "@sFuncao", "Salvar_Fabricacao" },
                            { "@sTipo", "2" },
                            { "@idFabricacaoRecurso", item.idFabricacaoRecurso.ToString()},
                            { "@idProduto", item.idProduto.ToString() },
                            { "@sCodigo", item.sCodigo },
                            { "@sDscRecurso", item.sDescricao },
                            { "@sUnidade", item.sUnidade },
                            { "@nQuantidade", item.nQtd.ToString().Replace(",",".") },
                            { "@nOrdem", item.nOrdem.ToString() },
                            { "@idTipoProduto", item.idTipo.ToString() },
                            { "@nValorUnitario", item.nValorUnitario.ToString().Replace(".","").Replace(",",".")  },
                            { "@idTabela", item.idTabela.ToString() },
                            { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                        };
                        DataSet ds = ExecutarDataSet(sProcedure, vParametrosFabricacao);
                    }

                    var itemTotais = bs_Fabricacao_Produto.FirstOrDefault();
                    Dictionary<string, string> vParametrosFabricacaoTotais = new Dictionary<string, string>
                    {
                        { "@sFuncao", "Salvar_Fabricacao" },
                        { "@sTipo", "5" },
                        { "@idProduto", itemTotais.idProduto.ToString() },
                        { "@nTotalFabricacaoProduto", txtnTotalProduto.Text.Replace(".","").Replace(",",".") },
                        { "@nTotalFabricacaoInsumo", txtnTotalInsumoFerramenta.Text.Replace(".","").Replace(",",".") },
                        { "@nTotalFabricacaoRecurso", txtnTotalRecursos.Text.Replace(".","").Replace(",",".") }
                    };
                    DataSet dsTotal = ExecutarDataSet(sProcedure, vParametrosFabricacaoTotais);

                }
                else
                {
                    sErro = 1;
                }

                if (AtualizarGridProcesso())
                {
                    foreach (var item in bs_Fabricacao_Processo)
                    {
                        Dictionary<string, string> vParametrosFabricacao = new Dictionary<string, string>
                        {
                            { "@sFuncao", "Salvar_Fabricacao" },
                            { "@sTipo", "3" },
                            { "@idFabricacaoProcesso", item.idFabricacaoProcesso.ToString()},
                            { "@idProduto", item.idProduto.ToString() },
                            { "@nOrdem", item.nOrdem.ToString() },
                            { "@sDscProcesso", item.sDescricao },
                            { "@idRecurso", item.idRecurso.ToString() },
                            { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                        };
                        DataSet ds = ExecutarDataSet(sProcedure, vParametrosFabricacao);
                    }
                }
                else
                {
                    sErro = 1;
                }

                if (sErro == 1)
                    return sMsgErro;

                return "";
            }
            catch (Exception e)
            {
                return "Erro ao salvar: " + e.ToString();
            }

        }

        private void AtualizarTodasAsGrids()
        {
            if (bs_Fabricacao_Produto.Count > 0)
            {
                AtualizarTodasSecoesFabricacaoProduto("1");
            }

            if (bs_Fabricacao_InsumoFerramenta.Count > 0)
            {
                AtualizarOrdemFabricacao(gv_fabricacao_insumo_ferramenta, "idFabricacaoInsumoFerramenta", "2");
            }

            if (bs_Fabricacao_Recurso.Count > 0)
            {
                AtualizarOrdemFabricacao(dtg_fabricacao_recurso, "idFabricacaoRecurso", "3");
            }
        }

        private bool AtualizarTodasSecoesFabricacaoProduto(string sTipo)
        {
            bool tudoValido = true;

            foreach (GridViewRow rowExterna in dtgv_fabricacao_produto_tipo.Rows)
            {
                if (rowExterna.RowType == DataControlRowType.DataRow)
                {
                    GridView gridViewInterna = (GridView)rowExterna.FindControl("dtgv_fabricacao_produto");

                    if (gridViewInterna != null)
                    {
                        bool resultado = AtualizarOrdemFabricacao(gridViewInterna, "idFabricacaoProduto", sTipo);

                        if (!resultado)
                            tudoValido = false;
                    }
                }
            }

            return tudoValido;

        }

        private bool AtualizarOrdemFabricacao(GridView grid, string dataKeyName, string sTipo)
        {
            bool todasLinhasValidas = true;

            foreach (GridViewRow row in grid.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int idFabricacao = Convert.ToInt32(grid.DataKeys[row.RowIndex][dataKeyName]);
                    string sDscItem = "";

                    object item = null;

                    switch (sTipo)
                    {
                        case "1":
                            item = bs_Fabricacao_Produto.FirstOrDefault(x => x.idFabricacaoProduto == idFabricacao);
                            sDscItem = item != null ? ((cls_WMS_Fabricacao_Produto)item).sCodigo : "N/A";
                            break;
                        case "2":
                            item = bs_Fabricacao_InsumoFerramenta.FirstOrDefault(x => x.idFabricacaoInsumoFerramenta == idFabricacao);
                            sDscItem = item != null ? ((cls_WMS_Fabricacao_InsumoFerramenta)item).sCodigo : "N/A";
                            break;
                        case "3":
                            item = bs_Fabricacao_Recurso.FirstOrDefault(x => x.idFabricacaoRecurso == idFabricacao);
                            sDscItem = item != null ? ((cls_WMS_Fabricacao_Recurso)item).sCodigo : "N/A";
                            break;
                        default:
                            sDscItem = "N/A";
                            break;
                    }

                    TextBox txtOrdem = (TextBox)row.FindControl("nOrdem");
                    TextBox txtnValorUnitario = (TextBox)row.FindControl("txtnValor");
                    TextBox txtnQtd = (TextBox)row.FindControl("txtnQtd");
                    int ordem = 0;
                    decimal nValor = 0;
                    decimal nQtd = 0;

                    if (string.IsNullOrEmpty(txtOrdem.Text) || !int.TryParse(txtOrdem.Text, out ordem))
                    {
                        sMsgErro += (sMsgErro != "" ? "</br>" : "") + $"Item: {sDscItem} - A Ordem deve ser um número válido";
                        todasLinhasValidas = false;
                    }

                    if (string.IsNullOrEmpty(txtnValorUnitario.Text) || !decimal.TryParse(txtnValorUnitario.Text, out nValor) || nValor == 0)
                    {
                        sMsgErro += (sMsgErro != "" ? "</br>" : "") + $"Item: {sDscItem} - O Valor unitário não pode ser 0 (zero)";
                        todasLinhasValidas = false;
                    }

                    if (string.IsNullOrEmpty(txtnQtd.Text) || !decimal.TryParse(txtnQtd.Text, out nQtd) || nQtd == 0)
                    {
                        sMsgErro += (sMsgErro != "" ? "</br>" : "") + $"Item: {sDscItem} - A Quantidade não pode ser 0 (zero)";
                        todasLinhasValidas = false;
                    }

                    if (todasLinhasValidas)
                    {
                        switch (sTipo)
                        {
                            case "1":
                                var produto = (cls_WMS_Fabricacao_Produto)item;
                                produto.nOrdem = ordem;
                                produto.nValorUnitario = nValor;
                                produto.nQtd = nQtd;
                                produto.nTotal = nQtd * nValor;
                                break;
                            case "2":
                                var insumo = (cls_WMS_Fabricacao_InsumoFerramenta)item;
                                insumo.nOrdem = ordem;
                                insumo.nValorUnitario = nValor;
                                insumo.nQtd = nQtd;
                                insumo.nTotal = nQtd * nValor;
                                break;
                            case "3":
                                var recurso = (cls_WMS_Fabricacao_Recurso)item;
                                recurso.nOrdem = ordem;
                                recurso.nValorUnitario = nValor;
                                recurso.nQtd = nQtd;
                                recurso.nTotal = nQtd * nValor;
                                break;
                        }
                    }
                }
            }

            return todasLinhasValidas;
        }

        private bool AtualizarGridProcesso()
        {
            bool todasLinhasValidas = true;

            foreach (GridViewRow row in dtg_fabricacao_processo.Rows)
            {
                int idLinha = Convert.ToInt32(dtg_fabricacao_processo.DataKeys[row.RowIndex].Value);
                var item = bs_Fabricacao_Processo.FirstOrDefault(x => x.idLinha == idLinha);

                if (item != null)
                {
                    TextBox txtOrdem = (TextBox)row.FindControl("nOrdem");
                    if (txtOrdem != null)
                    {
                        if (int.TryParse(txtOrdem.Text, out int ordem))
                        {
                            item.nOrdem = ordem;
                        }
                        else
                        {
                            Fabricacao_MensagemPagina.MostraMensagem_Erro("A Ordem deve ser um número válido na tabela de Procedimentos.");
                            todasLinhasValidas = false;
                        }
                    }


                    DropDownList ddlRecurso = (DropDownList)row.FindControl("ddlFabricacaoRecurso");
                    if (ddlRecurso != null)
                    {
                        if (ddlRecurso.SelectedValue != "0" && !string.IsNullOrEmpty(ddlRecurso.SelectedValue))
                        {
                            item.idRecurso = Convert.ToInt32(ddlRecurso.SelectedValue);
                        }
                        else
                        {
                            Fabricacao_MensagemPagina.MostraMensagem_Erro("Selecione um Recurso na tabela de Procedimentos.");
                            todasLinhasValidas = false;
                        }
                    }

                    TextBox txtsDescricao = (TextBox)row.FindControl("txtsDescricao");
                    if (txtsDescricao != null)
                    {
                        if (txtsDescricao.Text.Trim() != "")
                        {
                            item.sDescricao = txtsDescricao.Text;
                        }
                        else
                        {
                            Fabricacao_MensagemPagina.MostraMensagem_Erro("Descrição do Processo não pode ser vazio.");
                            todasLinhasValidas = false;
                        }
                    }
                }
            }

            return todasLinhasValidas;
        }

        private string ConsultarTipoProduto(string idItem, string sRecurso)
        {
            DataSet ds;
            Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "Consultar_Tipo_Produto" },
                    { "@idItem", idItem },
                    { "@sRecursoFabricacao",  sRecurso}
                };
            ds = ExecutarDataSet(sProcedure, vParametros);
            if (ValidarDataSet(ds, out string sErro))
            {
                hddsTipoFabricacaoProduto.Value = DATASET(ds, 0, "sDscTipoProduto");
                hddnValorUnitario.Value = string.IsNullOrEmpty(DATASET(ds, 1, 0, "nValorUnitario")) ? "0" : DATASET(ds, 1, 0, "nValorUnitario");
                hddidTabela.Value = string.IsNullOrEmpty(DATASET(ds, 1, 0, "idTabela")) ? "0" : DATASET(ds, 1, 0, "idTabela");
                return DATASET(ds, 0, "idTipoProduto");
            }
            else
                return "";
        }

        protected void cmdFabricacaoProduto_Click(object sender, EventArgs e)
        {
            AtualizarTodasAsGrids();

            if (Fabricacao_ValidarDados("1", fpFabricacaoProduto))
            {
                cls_WMS_Fabricacao_Produto objItem = new cls_WMS_Fabricacao_Produto();
                string[] vidItem = hddidProduto.Value.Split(',');
                string idItem = vidItem[0].ToString();

                if (bs_Fabricacao_Produto.Where(x => x.sCodigo == fpFabricacaoProduto.SCodigo).FirstOrDefault() != null)
                    Fabricacao_MensagemPagina.MostraMensagem_Aviso("Produto já incluso atualizado!");

                objItem.idProduto = Convert.ToInt32(hddidProduto.Value);
                objItem.idFabricacaoProduto = Convert.ToInt32(fpFabricacaoProduto.IdItem);
                objItem.sCodigo = fpFabricacaoProduto.SCodigo;
                objItem.sDescricao = fpFabricacaoProduto.SDscProduto;
                objItem.sUnidade = fpFabricacaoProduto.SUnidade;
                objItem.nOrdem = 0;
                objItem.idTipo = Convert.ToInt32(ConsultarTipoProduto(fpFabricacaoProduto.IdItem, "N"));
                objItem.sTipo = hddsTipoFabricacaoProduto.Value;
                objItem.nValorUnitario = Convert.ToDecimal(hddnValorUnitario.Value);
                objItem.idTabela = Convert.ToInt32(hddidTabela.Value);
                objItem.nQtd = Convert.ToDecimal(fpFabricacaoProduto.NQuantidade);
                objItem.nTotal = objItem.nValorUnitario * objItem.nQtd;

                int index = -1;
                for (int i = 0; i < bs_Fabricacao_Produto.Count; i++)
                {
                    if (bs_Fabricacao_Produto[i].idFabricacaoProduto == objItem.idFabricacaoProduto)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    bs_Fabricacao_Produto[index].nQtd = objItem.nQtd + bs_Fabricacao_Produto[index].nQtd;
                    bs_Fabricacao_Produto[index].nTotal = bs_Fabricacao_Produto[index].nQtd * objItem.nValorUnitario;
                }
                else
                {
                    bs_Fabricacao_Produto.Add(objItem);
                }

                fpFabricacaoProduto.LimparCampos();

            }

            //dtgv_fabricacao_produto_DataBind();
            DataBind_Geral();
            AtualizarTotalGeral();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RecalcularTotais", "recalcularTodasGrids();", true);
            Scripts.Mantem_AbaAtiva(Page, "fabricacao-tab");
        }

        protected void cmdIncluirInsumoFerramenta_Click(object sender, EventArgs e)
        {
            AtualizarTodasAsGrids();

            if (Fabricacao_ValidarDados("1", fpFabricacaoInsumoFerramenta))
            {
                cls_WMS_Fabricacao_InsumoFerramenta objItem = new cls_WMS_Fabricacao_InsumoFerramenta();
                string[] vidItem = hddidProduto.Value.Split(',');
                string idItem = vidItem[0].ToString();

                if (bs_Fabricacao_InsumoFerramenta.Where(x => x.sCodigo == fpFabricacaoInsumoFerramenta.SCodigo).FirstOrDefault() != null)
                    Fabricacao_MensagemPagina.MostraMensagem_Aviso("Insumo/Ferramenta já incluso atualizado!");

                objItem.idProduto = Convert.ToInt32(hddidProduto.Value);
                objItem.idFabricacaoInsumoFerramenta = Convert.ToInt32(fpFabricacaoInsumoFerramenta.IdItem);
                objItem.sCodigo = fpFabricacaoInsumoFerramenta.SCodigo;
                objItem.sDescricao = fpFabricacaoInsumoFerramenta.SDscProduto;
                objItem.sUnidade = fpFabricacaoInsumoFerramenta.SUnidade;
                objItem.nOrdem = 0;
                objItem.idTipo = Convert.ToInt32(ConsultarTipoProduto(fpFabricacaoInsumoFerramenta.IdItem, "N"));
                objItem.sTipo = hddsTipoFabricacaoProduto.Value;
                objItem.nValorUnitario = Convert.ToDecimal(hddnValorUnitario.Value);
                objItem.idTabela = Convert.ToInt32(hddidTabela.Value);
                objItem.nQtd = Convert.ToDecimal(fpFabricacaoInsumoFerramenta.NQuantidade);
                objItem.nTotal = objItem.nValorUnitario * objItem.nQtd;

                int index = -1;
                for (int i = 0; i < bs_Fabricacao_InsumoFerramenta.Count; i++)
                {
                    if (bs_Fabricacao_InsumoFerramenta[i].idFabricacaoInsumoFerramenta == objItem.idFabricacaoInsumoFerramenta)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    bs_Fabricacao_InsumoFerramenta[index].nQtd = objItem.nQtd + bs_Fabricacao_InsumoFerramenta[index].nQtd;
                    bs_Fabricacao_InsumoFerramenta[index].nTotal = bs_Fabricacao_InsumoFerramenta[index].nQtd * objItem.nValorUnitario;
                }
                else
                {
                    bs_Fabricacao_InsumoFerramenta.Add(objItem);
                }

                fpFabricacaoInsumoFerramenta.LimparCampos();

            }

            //gv_fabricacao_insumo_ferramenta_DataBind();
            DataBind_Geral();
            AtualizarTotalGeral();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RecalcularTotais", "recalcularTodasGrids();", true);
            Scripts.Mantem_AbaAtiva(Page, "fabricacao-tab");
        }

        protected void cmdFabricacaoRecurso_Click(object sender, EventArgs e)
        {
            AtualizarTodasAsGrids();

            if (Fabricacao_ValidarDados("2", fpFabricacaoRecurso))
            {
                cls_WMS_Fabricacao_Recurso objItem = new cls_WMS_Fabricacao_Recurso();
                string[] vidItem = hddidProduto.Value.Split(',');
                string idItem = vidItem[0].ToString();

                if (bs_Fabricacao_Recurso.Where(x => x.sCodigo == fpFabricacaoRecurso.SCodigo).FirstOrDefault() != null)
                    Fabricacao_MensagemPagina.MostraMensagem_Aviso("Recurso já incluso atualizado!");

                objItem.idProduto = Convert.ToInt32(hddidProduto.Value);
                objItem.idFabricacaoRecurso = Convert.ToInt32(fpFabricacaoRecurso.IdServico_Recurso);
                objItem.sCodigo = fpFabricacaoRecurso.SCodigoServico_Recurso;
                objItem.sDescricao = fpFabricacaoRecurso.SDscServico_Recurso;
                objItem.sUnidade = ddlsUnidadeRecurso.SelectedValue;
                objItem.idTipo = Convert.ToInt32(ConsultarTipoProduto(fpFabricacaoRecurso.IdServico_Recurso, "S"));
                objItem.sTipo = hddsTipoFabricacaoProduto.Value;
                objItem.nOrdem = 0;
                objItem.nValorUnitario = Convert.ToDecimal(hddnValorUnitario.Value);
                objItem.idTabela = Convert.ToInt32(hddidTabela.Value);
                objItem.nQtd = Convert.ToDecimal(fpFabricacaoRecurso.NQuantidade);
                objItem.nTotal = objItem.nValorUnitario * objItem.nQtd;

                int index = -1;
                for (int i = 0; i < bs_Fabricacao_Recurso.Count; i++)
                {
                    if (bs_Fabricacao_Recurso[i].idFabricacaoRecurso == objItem.idFabricacaoRecurso)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    bs_Fabricacao_Recurso[index].nQtd = objItem.nQtd + bs_Fabricacao_Recurso[index].nQtd;
                    bs_Fabricacao_Recurso[index].nTotal = bs_Fabricacao_Recurso[index].nQtd * objItem.nValorUnitario;
                }
                else
                {
                    bs_Fabricacao_Recurso.Add(objItem);
                }

                fpFabricacaoRecurso.LimparCampos();

            }

            //dtg_fabricacao_recurso_DataBind();
            //dtg_fabricacao_processo_DataBind();
            DataBind_Geral();
            AtualizarTotalGeral();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RecalcularTotais", "recalcularTodasGrids();", true);
            Scripts.Mantem_AbaAtiva(Page, "fabricacao-tab");
        }

        protected void cmdFabricacaoProcesso_Click(object sender, EventArgs e)
        {
            SincronizarGridViewParaLista();

            if (Fabricacao_ValidarDados("3", null))
            {
                var listaProcessos = bs_Fabricacao_Processo;

                cls_WMS_Fabricacao_Processo objItem = new cls_WMS_Fabricacao_Processo();

                if (string.IsNullOrWhiteSpace(txtnOrdem.Text))
                {
                    objItem.nOrdem = (listaProcessos.Any()) ? listaProcessos.Max(p => p.nOrdem) + 1 : 1;
                }
                else
                {
                    objItem.nOrdem = Convert.ToInt32(txtnOrdem.Text);
                }

                int novoIdLinha = (listaProcessos.Any()) ? listaProcessos.Max(p => p.idLinha) + 1 : 1;
                objItem.idLinha = novoIdLinha;

                objItem.idProduto = Convert.ToInt32(hddidProduto.Value);
                objItem.sDescricao = txtsDescricao.Text;

                listaProcessos.Add(objItem);
                bs_Fabricacao_Processo = listaProcessos;

                txtnOrdem.Text = "";
                txtsDescricao.Text = "";

            }

            //dtg_fabricacao_processo_DataBind();
            DataBind_Geral();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RecalcularTotais", "recalcularTodasGrids();", true);
            Scripts.Mantem_AbaAtiva(Page, "fabricacao-tab");
        }

        private bool Fabricacao_ValidarDados(string sTipo, FiltroPesquisa filtroPesquisa)
        {
            bool bRetorno = true;
            string sMensagemErro = "";
            if (sTipo == "1") //Produto
            {
                if (filtroPesquisa.SCodigo.Length < 3)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Código do Produto Inválido";
                }
                if (filtroPesquisa.SDscProduto.Length < 6)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Produto inválido!";

                }
                if (filtroPesquisa.NQuantidade <= decimal.Zero)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Quantidade inválida!";
                }
                if (filtroPesquisa.SUnidade == "0" || filtroPesquisa.SUnidade == "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Unidade Inválida!";
                }
            }
            else if (sTipo == "2") //Recurso
            {
                if (filtroPesquisa.SCodigoServico_Recurso.Length < 3)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Código do Recurso Inválido";
                }
                if (filtroPesquisa.SDscServico_Recurso.Length < 6)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Recurso inválido!";

                }
                if (filtroPesquisa.NQuantidade <= decimal.Zero)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Quantidade inválida!";
                }
                if (ddlsUnidadeRecurso.SelectedValue == "0" || ddlsUnidadeRecurso.SelectedValue == "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Unidade Inválida!";
                }
            }
            else if (sTipo == "3") //Processo
            {
                //if (txtnOrdem.Text == "")
                //{
                //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Inserir um valor para a ordem";
                //}
                if (txtsDescricao.Text == "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Inserir uma descrição para o processo";

                }
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                Fabricacao_MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;


        }

        private void SincronizarGridViewParaLista()
        {
            foreach (GridViewRow row in dtg_fabricacao_processo.Rows)
            {
                int idLinha = (int)dtg_fabricacao_processo.DataKeys[row.RowIndex].Value;
                var itemDaLista = bs_Fabricacao_Processo.FirstOrDefault(p => p.idLinha == idLinha);

                if (itemDaLista != null)
                {
                    TextBox txtOrdemNaGrid = (TextBox)row.FindControl("nOrdem");
                    DropDownList ddlRecursoNaGrid = (DropDownList)row.FindControl("ddlFabricacaoRecurso");

                    if (int.TryParse(txtOrdemNaGrid.Text, out int novaOrdem))
                    {
                        itemDaLista.nOrdem = novaOrdem;
                    }

                    if (ddlRecursoNaGrid != null && ddlRecursoNaGrid.SelectedValue != "")
                    {
                        itemDaLista.idRecurso = Convert.ToInt32(ddlRecursoNaGrid.SelectedValue);
                    }
                }
            }
        }

        private void DataBind_Geral()
        {
            dtgv_fabricacao_produto_DataBind();
            dtg_fabricacao_recurso_DataBind();
            dtg_fabricacao_processo_DataBind();
            gv_fabricacao_insumo_ferramenta_DataBind();
        }

        private void dtgv_fabricacao_produto_DataBind()
        {
            dtgv_fabricacao_produto_tipo.DataSource = GetFabricacaoPorTipoProduto(bs_Fabricacao_Produto);
            dtgv_fabricacao_produto_tipo.DataBind();
        }

        private void dtg_fabricacao_recurso_DataBind()
        {
            dtg_fabricacao_recurso.DataSource = bs_Fabricacao_Recurso;
            dtg_fabricacao_recurso.DataBind();
        }

        private void dtg_fabricacao_processo_DataBind()
        {
            var listaOrdenada = bs_Fabricacao_Processo.OrderBy(p => p.nOrdem).ThenBy(p => p.idLinha).ToList();

            dtg_fabricacao_processo.DataSource = listaOrdenada;
            dtg_fabricacao_processo.DataBind();
        }

        private void gv_fabricacao_insumo_ferramenta_DataBind()
        {
            gv_fabricacao_insumo_ferramenta.DataSource = bs_Fabricacao_InsumoFerramenta;
            gv_fabricacao_insumo_ferramenta.DataBind();
        }

        protected void dtgv_fabricacao_produto_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            GridView gv_fabricacao_produto = (GridView)sender;

            int idFabricacaoProduto = Convert.ToInt32(gv_fabricacao_produto.DataKeys[e.RowIndex].Value);

            var item = bs_Fabricacao_Produto.FirstOrDefault(x => x.idFabricacaoProduto == idFabricacaoProduto);

            if (item != null)
            {
                ExcluirFabricacao("1", idFabricacaoProduto, item.idProduto);
                bs_Fabricacao_Produto.Remove(item);
            }

            dtgv_fabricacao_produto_DataBind();
            AtualizarTotalGeral();
            Scripts.Mantem_AbaAtiva(Page, "fabricacao-tab");
        }

        protected void dtg_fabricacao_recurso_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idFabricacaoRecurso = Convert.ToInt32(dtg_fabricacao_recurso.DataKeys[e.RowIndex].Value);

            var item = bs_Fabricacao_Recurso.FirstOrDefault(x => x.idFabricacaoRecurso == idFabricacaoRecurso);

            if (item != null)
            {
                ExcluirFabricacao("2", idFabricacaoRecurso, item.idProduto);
                bs_Fabricacao_Recurso.Remove(item);
            }

            dtg_fabricacao_recurso_DataBind();
            dtg_fabricacao_processo_DataBind();
            AtualizarTotalGeral();
            Scripts.Mantem_AbaAtiva(Page, "fabricacao-tab");
        }

        protected void dtg_fabricacao_processo_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            SincronizarGridViewParaLista();

            int idLinha = Convert.ToInt32(dtg_fabricacao_processo.DataKeys[e.RowIndex].Value);

            var listaProcessos = bs_Fabricacao_Processo;
            var item = listaProcessos.FirstOrDefault(x => x.idLinha == idLinha);

            if (item != null)
            {
                if (item.idFabricacaoProcesso != 0)
                {
                    ExcluirFabricacao("3", item.idFabricacaoProcesso, item.idProduto);
                }
                listaProcessos.Remove(item);
            }

            bs_Fabricacao_Processo = listaProcessos;
            dtg_fabricacao_processo_DataBind();
            Scripts.Mantem_AbaAtiva(Page, "fabricacao-tab");
        }

        protected void gv_fabricacao_insumo_ferramenta_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idFabricacaoInsumoFerramenta = Convert.ToInt32(gv_fabricacao_insumo_ferramenta.DataKeys[e.RowIndex].Value);

            var item = bs_Fabricacao_InsumoFerramenta.FirstOrDefault(x => x.idFabricacaoInsumoFerramenta == idFabricacaoInsumoFerramenta);

            if (item != null)
            {
                ExcluirFabricacao("4", idFabricacaoInsumoFerramenta, item.idProduto);
                bs_Fabricacao_InsumoFerramenta.Remove(item);
            }

            gv_fabricacao_insumo_ferramenta_DataBind();
            AtualizarTotalGeral();
            Scripts.Mantem_AbaAtiva(Page, "fabricacao-tab");
        }

        private List<ListItem> ObterListaDeRecursos()
        {
            var listaItens = new List<ListItem>();
            listaItens.Add(new ListItem("Selecione...", "0"));

            foreach (var recurso in bs_Fabricacao_Recurso)
            {
                listaItens.Add(new ListItem(recurso.sDescricao, recurso.idFabricacaoRecurso.ToString()));
            }
            return listaItens;
        }

        protected void dtg_fabricacao_processo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlFabricacaoRecurso = (DropDownList)e.Row.FindControl("ddlFabricacaoRecurso");

                if (ddlFabricacaoRecurso != null)
                {
                    ddlFabricacaoRecurso.DataSource = ObterListaDeRecursos();
                    ddlFabricacaoRecurso.DataTextField = "Text";
                    ddlFabricacaoRecurso.DataValueField = "Value";
                    ddlFabricacaoRecurso.DataBind();

                    int idLinha = Convert.ToInt32(dtg_fabricacao_processo.DataKeys[e.Row.RowIndex].Value);
                    var item = bs_Fabricacao_Processo.FirstOrDefault(x => x.idLinha == idLinha);

                    if (item != null && item.idRecurso > 0)
                    {
                        if (ddlFabricacaoRecurso.Items.FindByValue(item.idRecurso.ToString()) != null)
                        {
                            ddlFabricacaoRecurso.SelectedValue = item.idRecurso.ToString();
                        }
                    }
                }
            }
        }

        private void ExcluirFabricacao(string sTipo, int idFabricacao, int idProduto)
        {
            Dictionary<string, string> vParametrosFabricacao = new Dictionary<string, string>
                                        {
                                            { "@sFuncao", "Excluir_Fabricacao" },
                                            { "@sTipo", sTipo },
                                            { "@idFabricacao", idFabricacao.ToString()},
                                            { "@idProduto", idProduto.ToString() },
                                            { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                                        };
            DataSet ds = ExecutarDataSet(sProcedure, vParametrosFabricacao);
        }

        protected void dtgv_fabricacao_produto_tipo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var fabricacaoProdutoTipo = (cls_WMS_Fabricacao_Produto_Tipo)e.Row.DataItem;
                var gvDetalhe = (GridView)e.Row.FindControl("dtgv_fabricacao_produto");

                if (gvDetalhe != null)
                {
                    gvDetalhe.DataSource = fabricacaoProdutoTipo.ls_FabricacaoProduto;
                    gvDetalhe.DataBind();
                }
            }
        }

        public string NovaLinha(object id, string gridNome)
        {
            /* 
            * Passo a passo:
            * 1. Fecha a célula atual
            * 2. Fecha a linha Atual
            * 3. Cria uma nova linha com o ID e a classe <TR id='...' style='...'>
            * 4. Cria uma célula em branco: <TD></TD>
            * 5. Cria uma nova célula para conter o gridview
            ************************************************************/
            if (id != null && !string.IsNullOrEmpty(id.ToString()))
            {
                // Se houver um ID, retorna a nova linha com o ID e a classe
                return string.Format(@"</td></tr><tr id='tr{0}{1}' style='display: table-row;'>
                               <td></td><td colspan='100' style='padding:0px; margin:0px;'>", gridNome, id);
            }
            else
            {
                // Se não houver ID, retorna uma string vazia para que nada seja renderizado e o botão de colapso desapareça
                return string.Empty;
            }
        }

        #endregion

        #region | Etiquetas

        void PopularImpressora()
        {
            Popula_Combo(ddlImpressora, "sp_Manipula_tbl_Flow_WMS_OPI_Etiqueta 'FLOW-IMPRESSORA'", "idImpressora", "sDscImpressora", false, "Selecione a Impressora", "0");
        }

        void Popular_Historico(string idProduto)
        {
            DataSet dsPesquisa;

            Dictionary<String, String> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idProduto", idProduto }
                    };

            dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

            gv_Historico.DataSource = dsPesquisa.Tables[8];
            gv_Historico.DataBind();
        }

        public void cmdGerarEtiquetas_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("sFuncao", "GERAR-ETIQUETAS");
                vParametros.Add("@idProduto", hddidProduto.Value);
                vParametros.Add("@nQuantidade", txtnQuantidade.Text);
                vParametros.Add("@idUsuarioAtualizacao", Variaveis.idUsuario());

                if (ddlImpressora.SelectedValue != "0")
                    vParametros.Add("@idImpressora", ddlImpressora.SelectedValue);

                DataSet dsSalvar = ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);

                if (ValidarDataSet(dsSalvar))
                {
                    Popular_Historico(hddidProduto.Value);
                    MensagemPaginaEtiquetas.MostraMensagem_Sucesso("Etiquetas Geradas!");

                    if (ddlImpressora.SelectedValue == "0")
                        MensagemPaginaEtiquetas.MostraMensagem("<b>Info: </b> A Impressora não foi selecionada então a Impressora padrão foi adicionada", "info", true);
                }
                else
                {
                    var msg = "";
                    if (txtnQuantidade.Text == "")
                        msg = "O Campo Quantidade está sem valor ou igual a 0.";

                    MensagemPaginaEtiquetas.MostraMensagem_Erro("Erro ao gerar etiquetas." + msg);
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaEtiquetas.MostraMensagem_Erro("Erro ao gerar etiquetas: " + ex.Message);
            }
        }

        #endregion

        #region | Aba Tabela de Preços

        protected void AbaTabelas_DataBind(bool bPopula_ListBox = true)
        {
            var list = bs_Tabelas.Where(t => t.bProdutoIncluso);

            gvTabelas_CustoFornecedor_Nacional.DataSource = list.Where(t => t.idTipoTabela.Equals(5) && t.idMoedaOrigem.Equals(2));
            gvTabelas_CustoFornecedor_Nacional.DataBind();

            gvTabelas_CustoFornecedor_Internacional.DataSource = list.Where(t => t.idTipoTabela.Equals(5) && !t.idMoedaOrigem.Equals(2));
            gvTabelas_CustoFornecedor_Internacional.DataBind();

            gvTabelas_CustoTT_Nacional.DataSource = list.Where(t => t.idTipoTabela.Equals(2) && t.idMoedaOrigem.Equals(2));
            gvTabelas_CustoTT_Nacional.DataBind();

            gvTabelas_CustoTT_Internacional.DataSource = list.Where(t => t.idTipoTabela.Equals(2) && !t.idMoedaOrigem.Equals(2));
            gvTabelas_CustoTT_Internacional.DataBind();

            gvTabelas_IndustrializaçãoTT_Nacional.DataSource = list.Where(t => t.idTipoTabela.Equals(4) && t.idMoedaOrigem.Equals(2));
            gvTabelas_IndustrializaçãoTT_Nacional.DataBind();

            gvTabelas_IndustrializaçãoTT_Internacional.DataSource = list.Where(t => t.idTipoTabela.Equals(4) && !t.idMoedaOrigem.Equals(2));
            gvTabelas_IndustrializaçãoTT_Internacional.DataBind();

            gvTabelas_VendasPVP.DataSource = list.Where(t => t.idTipoTabela.Equals(3));
            gvTabelas_VendasPVP.DataBind();

            gvTabelas_VendasCustomizadas.DataSource = list.Where(t => t.idTipoTabela.Equals(1));
            gvTabelas_VendasCustomizadas.DataBind();

            gvTabelas_CustoEmpreitada.DataSource = list.Where(t => t.idTipoTabela.Equals(10));
            gvTabelas_CustoEmpreitada.DataBind();

            gvTabelas_VendasLPU.DataSource = list.Where(t => t.idTipoTabela.Equals(11));
            gvTabelas_VendasLPU.DataBind();

            gvTabelas_CustoRecursos.DataSource = list.Where(t => t.idTipoTabela.Equals(8));
            gvTabelas_CustoRecursos.DataBind();

            if (bPopula_ListBox)
            {
                lstAdicionarTabelas.Items.Clear();

                foreach (var tabela in bs_Tabelas.Where(t => !t.bProdutoIncluso))
                {
                    AbaTabelas_ListBox(tabela.idTabela, tabela.sDscTabela, tabela.idTipoTabela, tabela.sDscTipoTabela, false, tabela.bRecurso, tabela.bIndustrializado);
                }
            }

            string sIDsTabelasAbertas = string.Empty;
            foreach (string sid in hddsTabelas_Abertas.Value.Split('|'))
            {
                if (decimal.TryParse(sid.Replace("toggle_", "").Replace("_", ","), out decimal id))
                    sIDsTabelasAbertas += $"#{sid}, ";
            }

            sIDsTabelasAbertas = sIDsTabelasAbertas.Trim();

            if (sIDsTabelasAbertas.EndsWith(",")) sIDsTabelasAbertas = sIDsTabelasAbertas.Remove(sIDsTabelasAbertas.Length - 1);

            if (!string.IsNullOrEmpty(sIDsTabelasAbertas))
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Manter_panels_Abertos", "setTimeout(function() { $('" + sIDsTabelasAbertas + "').trigger('click'); }, 50);", true);

            if (gvTabelas_CustoFornecedor_Nacional.Rows.Count <= 0 && gvTabelas_CustoFornecedor_Internacional.Rows.Count <= 0) pnCustoFornecedor.Visible = false; else pnCustoFornecedor.Visible = true;
            if (gvTabelas_CustoTT_Nacional.Rows.Count <= 0 && gvTabelas_CustoTT_Internacional.Rows.Count <= 0) pnCustoTT.Visible = false; else pnCustoTT.Visible = true;
            if (gvTabelas_IndustrializaçãoTT_Nacional.Rows.Count <= 0 && gvTabelas_IndustrializaçãoTT_Internacional.Rows.Count <= 0) pnIndustrializacaoTT.Visible = false; else pnIndustrializacaoTT.Visible = true;

            if (gvTabelas_CustoFornecedor_Nacional.Rows.Count <= 0) pnCustoFornecedor_Nacional.Visible = false; else pnCustoFornecedor_Nacional.Visible = true;
            if (gvTabelas_CustoFornecedor_Internacional.Rows.Count <= 0) pnCustoFornecedor_Internacional.Visible = false; else pnCustoFornecedor_Internacional.Visible = true;
            if (gvTabelas_CustoTT_Nacional.Rows.Count <= 0) pnCustoTT_Nacional.Visible = false; else pnCustoTT_Nacional.Visible = true;
            if (gvTabelas_CustoTT_Internacional.Rows.Count <= 0) pnCustoTT_Internacional.Visible = false; else pnCustoTT_Internacional.Visible = true;
            if (gvTabelas_IndustrializaçãoTT_Nacional.Rows.Count <= 0) pnIndustrializacaoTT_Nacional.Visible = false; else pnIndustrializacaoTT_Nacional.Visible = true;
            if (gvTabelas_IndustrializaçãoTT_Internacional.Rows.Count <= 0) pnIndustrializacaoTT_Internacional.Visible = false; else pnIndustrializacaoTT_Internacional.Visible = true;
            if (gvTabelas_VendasPVP.Rows.Count <= 0) pnVendasPVP.Visible = false; else pnVendasPVP.Visible = true;
            if (gvTabelas_VendasCustomizadas.Rows.Count <= 0) pnVendasCustom.Visible = false; else pnVendasCustom.Visible = true;
            if (gvTabelas_CustoEmpreitada.Rows.Count <= 0) pnCustoEmpreitada.Visible = false; else pnCustoEmpreitada.Visible = true;
            if (gvTabelas_VendasLPU.Rows.Count <= 0) pnVendasLPU.Visible = false; else pnVendasLPU.Visible = true;
            if (gvTabelas_CustoRecursos.Rows.Count <= 0) pnCustoRecursos.Visible = false; else pnCustoRecursos.Visible = true;
        }

        protected void AbaTabelas_ListBox(int idTabela, string sDscTabela, int idTipoTabela, string sDscTipoTabela, bool bProdutoIncluso, bool bRecurso, bool bIndustrializado)
        {
            if (!bProdutoIncluso && lstAdicionarTabelas.Items.FindByValue(idTabela.ToString()) == null)
            {
                string text = sDscTabela + " - Tipo: " + sDscTipoTabela;

                if (bRecurso && idTipoTabela == 8)
                    lstAdicionarTabelas.Items.Add(new ListItem(text, idTabela.ToString()));
                else
                {
                    if (bIndustrializado && idTipoTabela == 4)
                        lstAdicionarTabelas.Items.Add(new ListItem(text, idTabela.ToString()));
                    else if (!bIndustrializado && idTipoTabela != 4)
                        lstAdicionarTabelas.Items.Add(new ListItem(text, idTabela.ToString()));
                }
            }
        }

        protected void AtualizaTabelas()
        {
            try
            {
                var dictTabelas = bs_Tabelas.ToDictionary(t => t.idTabela);

                foreach (GridViewRow row in gvTabelas_CustoFornecedor_Nacional.Rows)
                {
                    int.TryParse(gvTabelas_CustoFornecedor_Nacional.DataKeys[row.RowIndex]["idTabela"].ToString(), out int idTabela);

                    if (dictTabelas.TryGetValue(idTabela, out var tabela))
                    {
                        decimal.TryParse((row.FindControl("txtPreco") as TextBox).Text, out decimal nPreco);
                        decimal.TryParse((row.FindControl("txtTotal") as TextBox).Text, out decimal nTotal);

                        tabela.nPreco = Math.Round(nPreco, 2);
                        tabela.nTotal = Math.Round(nTotal, 2);
                    }
                }

                foreach (GridViewRow row in gvTabelas_CustoFornecedor_Internacional.Rows)
                {
                    int.TryParse(gvTabelas_CustoFornecedor_Internacional.DataKeys[row.RowIndex]["idTabela"].ToString(), out int idTabela);

                    if (dictTabelas.TryGetValue(idTabela, out var tabela))
                    {
                        decimal.TryParse((row.FindControl("txtPreco") as TextBox).Text, out decimal nPreco);
                        decimal.TryParse((row.FindControl("txtEnvio") as TextBox).Text, out decimal nEnvio);
                        decimal.TryParse((row.FindControl("txtLocal") as TextBox).Text, out decimal nLocal);
                        decimal.TryParse((row.FindControl("txtTotal") as TextBox).Text, out decimal nTotal);

                        tabela.nPreco = Math.Round(nPreco, 2);
                        tabela.nTaxaEnvio = Math.Round(nEnvio, 2);
                        tabela.nTaxaLocal = Math.Round(nLocal, 2);
                        tabela.nTotal = Math.Round(nTotal, 2);
                    }
                }

                foreach (GridViewRow row in gvTabelas_CustoTT_Nacional.Rows)
                {
                    int.TryParse(gvTabelas_CustoTT_Nacional.DataKeys[row.RowIndex]["idTabela"].ToString(), out int idTabela);

                    if (dictTabelas.TryGetValue(idTabela, out var tabela))
                    {
                        decimal.TryParse((row.FindControl("txtPreco") as TextBox).Text, out decimal nPreco);
                        decimal.TryParse((row.FindControl("txtMargem") as TextBox).Text, out decimal nMargem);
                        decimal.TryParse((row.FindControl("txtTotal") as TextBox).Text, out decimal nTotal);

                        tabela.nPreco = Math.Round(nPreco, 2);
                        tabela.nMargem = Math.Round(nMargem, 2);
                        tabela.nTotal = Math.Round(nTotal, 2);
                    }
                }

                foreach (GridViewRow row in gvTabelas_CustoTT_Internacional.Rows)
                {
                    int.TryParse(gvTabelas_CustoTT_Internacional.DataKeys[row.RowIndex]["idTabela"].ToString(), out int idTabela);

                    if (dictTabelas.TryGetValue(idTabela, out var tabela))
                    {
                        decimal.TryParse((row.FindControl("txtPreco") as TextBox).Text, out decimal nPreco);
                        decimal.TryParse((row.FindControl("txtMargem") as TextBox).Text, out decimal nMargem);
                        decimal.TryParse((row.FindControl("txtEnvio") as TextBox).Text, out decimal nEnvio);
                        decimal.TryParse((row.FindControl("txtLocal") as TextBox).Text, out decimal nLocal);
                        decimal.TryParse((row.FindControl("txtTotal") as TextBox).Text, out decimal nTotal);

                        tabela.nPreco = Math.Round(nPreco, 2);
                        tabela.nMargem = Math.Round(nMargem, 2);
                        tabela.nTaxaEnvio = Math.Round(nEnvio, 2);
                        tabela.nTaxaLocal = Math.Round(nLocal, 2);
                        tabela.nTotal = Math.Round(nTotal, 2);
                    }
                }

                foreach (GridViewRow row in gvTabelas_IndustrializaçãoTT_Nacional.Rows)
                {
                    int.TryParse(gvTabelas_IndustrializaçãoTT_Nacional.DataKeys[row.RowIndex]["idTabela"].ToString(), out int idTabela);

                    if (dictTabelas.TryGetValue(idTabela, out var tabela))
                    {
                        decimal.TryParse((row.FindControl("txtPreco") as TextBox).Text, out decimal nPreco);
                        decimal.TryParse((row.FindControl("txtFator") as TextBox).Text, out decimal nFator);
                        decimal.TryParse((row.FindControl("txtTotal") as TextBox).Text, out decimal nTotal);

                        tabela.nPreco = Math.Round(nPreco, 2);
                        tabela.nFator = Math.Round(nFator, 2);
                        tabela.nTotal = Math.Round(nTotal, 2);
                    }
                }

                foreach (GridViewRow row in gvTabelas_IndustrializaçãoTT_Internacional.Rows)
                {
                    int.TryParse(gvTabelas_IndustrializaçãoTT_Internacional.DataKeys[row.RowIndex]["idTabela"].ToString(), out int idTabela);

                    if (dictTabelas.TryGetValue(idTabela, out var tabela))
                    {
                        decimal.TryParse((row.FindControl("txtPreco") as TextBox).Text, out decimal nPreco);
                        decimal.TryParse((row.FindControl("txtEnvio") as TextBox).Text, out decimal nEnvio);
                        decimal.TryParse((row.FindControl("txtLocal") as TextBox).Text, out decimal nLocal);
                        decimal.TryParse((row.FindControl("txtFator") as TextBox).Text, out decimal nFator);
                        decimal.TryParse((row.FindControl("txtTotal") as TextBox).Text, out decimal nTotal);

                        tabela.nPreco = Math.Round(nPreco, 2);
                        tabela.nTaxaEnvio = Math.Round(nEnvio, 2);
                        tabela.nTaxaLocal = Math.Round(nLocal, 2);
                        tabela.nFator = Math.Round(nFator, 2);
                        tabela.nTotal = Math.Round(nTotal, 2);
                    }
                }

                foreach (GridViewRow row in gvTabelas_VendasPVP.Rows)
                {
                    int.TryParse(gvTabelas_VendasPVP.DataKeys[row.RowIndex]["idTabela"].ToString(), out int idTabela);

                    if (dictTabelas.TryGetValue(idTabela, out var tabela))
                    {
                        decimal.TryParse((row.FindControl("txtPreco") as TextBox).Text, out decimal nPreco);
                        decimal.TryParse((row.FindControl("txtFator") as TextBox).Text, out decimal nFator);
                        decimal.TryParse((row.FindControl("txtTotal") as TextBox).Text, out decimal nTotal);

                        tabela.nPreco = Math.Round(nPreco, 2);
                        tabela.nFator = Math.Round(nFator, 2);
                        tabela.nTotal = Math.Round(nTotal, 2);
                    }
                }

                foreach (GridViewRow row in gvTabelas_VendasCustomizadas.Rows)
                {
                    int.TryParse(gvTabelas_VendasCustomizadas.DataKeys[row.RowIndex]["idTabela"].ToString(), out int idTabela);

                    if (dictTabelas.TryGetValue(idTabela, out var tabela))
                    {
                        decimal.TryParse((row.FindControl("txtPreco") as TextBox).Text, out decimal nPreco);
                        decimal.TryParse((row.FindControl("txtDesconto") as TextBox).Text, out decimal nDesconto);
                        decimal.TryParse((row.FindControl("txtTotal") as TextBox).Text, out decimal nTotal);
                        bool bLiberado = (row.FindControl("cbLiberado") as CheckBox).Checked;

                        tabela.nPreco = Math.Round(nPreco, 2);
                        tabela.nFator = Math.Round(nDesconto, 2);
                        tabela.nTotal = Math.Round(nTotal, 2);
                        tabela.bLiberado = bLiberado;
                    }
                }

                foreach (GridViewRow row in gvTabelas_CustoEmpreitada.Rows)
                {
                    int.TryParse(gvTabelas_CustoEmpreitada.DataKeys[row.RowIndex]["idTabela"].ToString(), out int idTabela);

                    if (dictTabelas.TryGetValue(idTabela, out var tabela))
                    {
                        decimal.TryParse((row.FindControl("txtPreco") as TextBox).Text, out decimal nPreco);
                        decimal.TryParse((row.FindControl("txtFator") as TextBox).Text, out decimal nFator);
                        decimal.TryParse((row.FindControl("txtTotal") as TextBox).Text, out decimal nTotal);

                        tabela.nPreco = Math.Round(nPreco, 2);
                        tabela.nFator = Math.Round(nFator, 2);
                        tabela.nTotal = Math.Round(nTotal, 2);
                    }
                }

                foreach (GridViewRow row in gvTabelas_VendasLPU.Rows)
                {
                    int.TryParse(gvTabelas_VendasLPU.DataKeys[row.RowIndex]["idTabela"].ToString(), out int idTabela);

                    if (dictTabelas.TryGetValue(idTabela, out var tabela))
                    {
                        decimal.TryParse((row.FindControl("txtPreco") as TextBox).Text, out decimal nPreco);
                        decimal.TryParse((row.FindControl("txtDesconto") as TextBox).Text, out decimal nDesconto);
                        decimal.TryParse((row.FindControl("txtTotal") as TextBox).Text, out decimal nTotal);

                        tabela.nPreco = Math.Round(nPreco, 2);
                        tabela.nFator = Math.Round(nDesconto, 2);
                        tabela.nTotal = Math.Round(nTotal, 2);
                    }
                }

                foreach (GridViewRow row in gvTabelas_CustoRecursos.Rows)
                {
                    int.TryParse(gvTabelas_CustoRecursos.DataKeys[row.RowIndex]["idTabela"].ToString(), out int idTabela);

                    if (dictTabelas.TryGetValue(idTabela, out var tabela))
                    {
                        decimal.TryParse((row.FindControl("txtPreco_Zona_SD") as TextBox).Text, out decimal nPreco_Zona_SD);
                        decimal.TryParse((row.FindControl("txtPreco_Zona_ND") as TextBox).Text, out decimal nPreco_Zona_ND);
                        decimal.TryParse((row.FindControl("txtPreco_Zona_N") as TextBox).Text, out decimal nPreco_Zona_N);
                        decimal.TryParse((row.FindControl("txtPreco_Zona_CO") as TextBox).Text, out decimal nPreco_Zona_CO);
                        decimal.TryParse((row.FindControl("txtPreco_Zona_S") as TextBox).Text, out decimal nPreco_Zona_S);
                        decimal.TryParse((row.FindControl("txtFator") as TextBox).Text, out decimal nFator);

                        tabela.nPreco_Zona_SD = Math.Round(nPreco_Zona_SD, 2);
                        tabela.nPreco_Zona_ND = Math.Round(nPreco_Zona_ND, 2);
                        tabela.nPreco_Zona_N = Math.Round(nPreco_Zona_N, 2);
                        tabela.nPreco_Zona_CO = Math.Round(nPreco_Zona_CO, 2);
                        tabela.nPreco_Zona_S = Math.Round(nPreco_Zona_S, 2);
                        tabela.nFator = Math.Round(nFator, 2);
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Tabelas.MostraMensagem_Erro("<b>Erro: </b>Houve um erro ao Atualizar os valores nas Tabelas!<br />Erro para Atualizar valores: " + ex.Message, true);
            }
        }

        private void AbaTabelas_Salvar(string idItem)
        {
            AtualizaTabelas();

            try
            {
                foreach (var item in bs_Tabelas)
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", item.bProdutoIncluso ? "SALVAR_ITENS_ABA_TABELAS" : "EXCLUIR_ITENS_ABA_TABELAS" },
                        { "@idItem", idItem },
                        { "@idTabela", item.idTabela.ToString() },
                        { "@nPreco", item.nPreco.ToString().Replace(",", ".") },
                        { "@sUnidade", ddlUnidade.SelectedValue },
                        { "@nFator", item.nFator.ToString().Replace(",", ".") },
                        { "@nTaxaEnvio", item.nTaxaEnvio.ToString().Replace(",", ".") },
                        { "@nTaxaLocal", item.nTaxaLocal.ToString().Replace(",", ".") },
                        { "@nMargem", item.nMargem.ToString().Replace(",", ".") },
                        { "@nTaxaImpostos", item.nTaxaImpostos.ToString().Replace(",", ".") },
                        { "@nTotal", item.nTotal.ToString().Replace(",", ".") },
                        { "@nPreco_Zona_SD", item.nPreco_Zona_SD.ToString().Replace(",", ".") },
                        { "@nPreco_Zona_ND", item.nPreco_Zona_ND.ToString().Replace(",", ".") },
                        { "@nPreco_Zona_N", item.nPreco_Zona_N.ToString().Replace(",", ".") },
                        { "@nPreco_Zona_CO", item.nPreco_Zona_CO.ToString().Replace(",", ".") },
                        { "@nPreco_Zona_S", item.nPreco_Zona_S.ToString().Replace(",", ".") },
                        { "@sLiberado", item.bLiberado ? "S" : "N" },
                        { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                    };
                    DataSet ds = ExecutarDataSet(sProcedure, vParametros);
                }

                MensagemPagina_Tabelas.MostraMensagem_Sucesso("Registros Salvos com sucesso!");
            }
            catch (Exception e)
            {
                MensagemPagina_Tabelas.MostraMensagem_Erro("Erro ao salvar o Item nas Tabelas de Preços: " + e.ToString());
            }
        }

        protected void cmdAdicionarTabelas_Click(object sender, EventArgs e)
        {
            try
            {
                AtualizaTabelas();

                if (!string.IsNullOrEmpty(lstAdicionarTabelas.SelectedValue))
                {
                    foreach (ListItem item in lstAdicionarTabelas.Items)
                    {
                        if (item.Selected)
                        {
                            int.TryParse(item.Value, out int idTabela);
                            var tabela = bs_Tabelas.FirstOrDefault(t => t.idTabela.Equals(idTabela));

                            if (tabela != null)
                                tabela.bProdutoIncluso = true;
                        }
                    }
                }
                else
                    throw new Exception("É necessário selecionar ao menos uma Tabela de Preços para Adicionar!");
            }
            catch (Exception ex)
            {
                MensagemPagina_Tabelas.MostraMensagem_Erro("<b>Erro: </b>Houve um erro ao Adicionar uma Tabela!<br />Erro para Adicionar Tabela: " + ex.Message, true);
            }

            AbaTabelas_DataBind();

            Scripts.FocusScript(Page, lstAdicionarTabelas.ClientID);
        }

        protected void gvTabelas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                AtualizaTabelas();

                int.TryParse(e.CommandArgument.ToString(), out int idTabela);
                var tabela = bs_Tabelas.FirstOrDefault(t => t.idTabela.Equals(idTabela));

                if (tabela != null)
                    tabela.bProdutoIncluso = false;
            }
            catch (Exception ex)
            {
                MensagemPagina_Tabelas.MostraMensagem_Erro("<b>Erro: </b>Houve um erro ao Excluir uma Tabela!<br />Erro para Excluir Tabela: " + ex.Message, true);
            }

            AbaTabelas_DataBind();
        }

        protected void gvTabelas_VendasCustomizadas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = e.Row.DataItem;

                if (item != null && item is cls_Produtos_Aba_Tabelas tabela)
                {
                    (e.Row.FindControl("cbLiberado") as CheckBox).Checked = tabela.bLiberado;
                    e.Row.CssClass = tabela.bLiberado ? "success" : "danger";
                }
            }
        }

        #endregion

        #region | EPI - Consumiveis

        #region | EPI

        public List<cls_EPI_x_Funcao> bs_EPI_x_Entrega
        {
            get
            {
                if (ViewState["bs_EPI_x_Entrega"] == null)
                {
                    ViewState["bs_EPI_x_Entrega"] = new List<cls_EPI_x_Funcao>();
                }
                return (List<cls_EPI_x_Funcao>)ViewState["bs_EPI_x_Entrega"];
            }
            set
            {
                ViewState["bs_EPI_x_Entrega"] = value;
            }
        }

        public List<cls_Flow_Recursos_EPI> bs_Recursos_EPIs
        {
            get
            {
                if (ViewState["bs_Recursos_EPIs"] == null)
                {
                    ViewState["bs_Recursos_EPIs"] = new List<cls_Flow_Recursos_EPI>();
                }
                return (List<cls_Flow_Recursos_EPI>)ViewState["bs_Recursos_EPIs"];
            }
            set
            {
                ViewState["bs_Recursos_EPIs"] = value;
            }
        }

        protected void cmdEPI_Incluir_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hddIncluir_idEPI.Value) || hddIncluir_idEPI.Value == "0")
            {
                AbrirTab();
                MensagemPagina_Incluir_EPI.MostraMensagem_Aviso("Por favor, selecione um EPI da lista antes de incluir.", true);
                return;
            }

            int idEPI_a_Incluir = Convert.ToInt32(hddIncluir_idEPI.Value);
            //int nCA_a_Incluir = Convert.ToInt32(txtIncluirEPI_CA.Text);

            var epiExistente = bs_Recursos_EPIs.FirstOrDefault(epi => epi.idEPI == idEPI_a_Incluir);

            if (epiExistente != null)
            {
                epiExistente.nQuantidade = Convert.ToInt32(txtIncluirEPI_Qtd.Text);
                //epiExistente.nQuantidadeTempo = Convert.ToInt32(txtIncluirEPI_nTempo.Text);
                //epiExistente.sTipoPeriodo = ddlIncluirEPI_sTipoPeriodo.SelectedValue;

                if (epiExistente.sFuncao == "EXCLUIR")
                {
                    epiExistente.sFuncao = "CONSULTA_EPI";
                }

                //string descPeriodo = ddlIncluirEPI_sTipoPeriodo.SelectedItem.Text;
                //if (epiExistente.nQuantidadeTempo > 1)
                //{
                //    descPeriodo = descPeriodo.EndsWith("s") ? descPeriodo : descPeriodo + "s";
                //}
                //epiExistente.sPeriodicidade = $"{epiExistente.nQuantidadeTempo} {descPeriodo}";
            }
            else
            {
                int proximoIdLinha = (bs_Recursos_EPIs.Any() ? bs_Recursos_EPIs.Max(x => x.idLinha) : 0) + 1;

                var novoEpi = new cls_Flow_Recursos_EPI
                {
                    idLinha = proximoIdLinha,
                    idEPI = idEPI_a_Incluir,
                    idRecurso = 0,
                    sFuncao = "INCLUIR",
                    sCodigoEPI = txtCodigo_EPI.Text,
                    sDscEPI = txtDesc_EPI.Text,
                    nQuantidade = Convert.ToInt32(txtIncluirEPI_Qtd.Text)

                    //nCA = nCA_a_Incluir,
                    //nQuantidadeTempo = Convert.ToInt32(txtIncluirEPI_nTempo.Text),
                    //sTipoPeriodo = ddlIncluirEPI_sTipoPeriodo.SelectedValue
                };

                //string descPeriodo = ddlIncluirEPI_sTipoPeriodo.SelectedItem.Text;
                //if (novoEpi.nQuantidadeTempo > 1)
                //{
                //    descPeriodo = descPeriodo.EndsWith("s") ? descPeriodo : descPeriodo + "s";
                //}
                //novoEpi.sPeriodicidade = $"{novoEpi.nQuantidadeTempo} {descPeriodo}";

                bs_Recursos_EPIs.Add(novoEpi);
            }

            dtgEPI_DataBind();
            LimpaCampos_EPI();

            RegistraScript_Personalizado(txtCodigo_EPI.ClientID, true);
            RegistraScript_Personalizado(txtDesc_EPI.ClientID, false);
            AbrirTab();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtDesc_EPI]').focus();", true);
        }

        protected void RegistraScript_Personalizado(string clientID, bool bCodigo)
        {
            // A variável 'list' não é mais necessária aqui.
            StringBuilder sb = new StringBuilder();

            sb.Append("$v192(function() {\r\n");
            sb.Append("  $v192(\"[id*=" + clientID + "]\").autocomplete({\r\n");
            sb.Append("    source: function(request, response) {\r\n");
            sb.Append("      $v192.ajax({\r\n");
            sb.Append("        url: '/app/Paginas/Manutencao/Produtos_Detalhe.aspx/GetEPI',\r\n");
            // ENVIE APENAS O TERMO DA BUSCA
            sb.Append("        data: JSON.stringify({ 'sDsc': request.term }),\r\n");
            sb.Append("        dataType: \"json\",\r\n");
            sb.Append("        type: \"POST\",\r\n");
            sb.Append("        contentType: \"application/json; charset=utf-8\",\r\n");
            sb.Append("        success: function(data) {\r\n");
            // O 'data.d' agora é uma lista de objetos, o mapeamento fica mais simples
            sb.Append("          response($v192.map(data.d, function(item) {\r\n");
            sb.Append("            return {\r\n");
            sb.Append("              label: " + (bCodigo ? "item.codigo" : "item.descricao") + ", \r\n");
            sb.Append("              id: item.id,\r\n");
            sb.Append("              codigo: item.codigo,\r\n");
            sb.Append("              descricao: item.descricao\r\n");
            sb.Append("            };\r\n");
            sb.Append("          }));\r\n");
            sb.Append("        },\r\n");
            sb.Append("        error: function(response) { console.log('Error: ' + response.responseText); }\r\n");
            sb.Append("      });\r\n");
            sb.Append("    },\r\n");
            // O resto do seu script (select, minLength, etc.) continua igual
            sb.Append("    select: function(e, ui) {\r\n");
            sb.Append("      $v192('#" + hddIncluir_idEPI.ClientID + "').val(ui.item.id);\r\n");
            sb.Append("      $v192('#" + txtCodigo_EPI.ClientID + "').val(ui.item.codigo);\r\n");
            sb.Append("      $v192('#" + txtDesc_EPI.ClientID + "').val(ui.item.descricao);\r\n");
            sb.Append("      return false;\r\n");
            sb.Append("    },\r\n");
            sb.Append("    minLength: 3\r\n");
            sb.Append("  });\r\n");
            sb.Append("});\r\n");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina_Personalizado_" + clientID, sb.ToString(), true);
        }

        [WebMethod]
        public static List<object> GetEPI(string sDsc)
        {
            string sProcedure = "sp_Manipula_tbl_Flow_Produtos";
            List<object> resultado = new List<object>();

            Dictionary<string, string> vParametros = new Dictionary<string, string>()
    {
        { "@sFuncao", "BUSCAR-EPI-POR-TEXTO" },
        { "@TermoBusca", sDsc }
    };

            DataSet dsEpis = ExecutarDataSet(sProcedure, vParametros);

            if (ValidarDataSet(dsEpis))
            {
                foreach (DataRow row in dsEpis.Tables[0].Rows)
                {
                    resultado.Add(new
                    {
                        id = row["idItem"].ToString(),
                        codigo = row["sCodigo"].ToString(),
                        descricao = row["sDscProduto"].ToString()
                    });
                }
            }

            return resultado;
        }

        protected void Popula_IncluirEPIs(DataTable dt)
        {
            hddEPIs.Value = string.Empty;
            foreach (DataRow row in dt.Rows)
            {
                hddEPIs.Value += string.Format("[{0}|{1}|{2}]", row["idItem"].ToString(), row["sCodigo"].ToString(), row["sDscProduto"].ToString());
            }
        }

        void Popula_EPI()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>()
                {
                    { "@sFuncao", "CONSULTAR-EPI" }
                };
            DataSet dsPesquisa = ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Entrega_EPI", vParametros);

            if (ValidarDataSet(dsPesquisa))
            {
                Popular_dtgEPI(dsPesquisa);
                Popula_IncluirEPIs(dsPesquisa.Tables[0]);
                RegistraScript_Personalizado(txtCodigo_EPI.ClientID, true);
                RegistraScript_Personalizado(txtDesc_EPI.ClientID, false);
            }
            else
            {
                AbrirTab();
                MensagemPagina_Incluir_EPI.MostraMensagem_Erro("Erro ao consultar EPIs: " + dsPesquisa.Tables[0].Rows[0]["sMensagem"].ToString(), true);
            }
        }

        void Popular_dtgEPI(DataSet dsPesquisa)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>()
             {
                 { "@sFuncao", "CONSULTAR-EPI" },
                 { "@idProduto", hddidProduto.Value }
             };

            DataSet dsGrid = ExecutarDataSet(sProcedure, vParametros);

            var listaEpis = new List<cls_Flow_Recursos_EPI>();
            int linhaCounter = 0;

            if (ValidarDataSet(dsGrid))
            {
                foreach (DataRow row in dsGrid.Tables[0].Rows)
                {
                    var epi = new cls_Flow_Recursos_EPI
                    {
                        idRegistro = Convert.ToInt32(row["idRegistro"]),
                        idEPI = Convert.ToInt32(row["idEPI"]),
                        idRecurso = Convert.ToInt32(row["idRecurso"]),
                        nQuantidade = Convert.ToInt32(row["nQuantidade"]),
                        //nCA = Convert.ToInt32(row["nCA"]),

                        sCodigoEPI = row["sCodigo"].ToString(),
                        sDscEPI = row["sDscProduto"].ToString(),

                        //nQuantidadeTempo = Convert.ToInt32(row["nPeriodicidade"]),
                        //sTipoPeriodo = row["sTipoPeriodo"].ToString(),

                        idLinha = ++linhaCounter,

                        sFuncao = "CONSULTA_EPI"
                    };

                    //string descPeriodo = epi.sTipoPeriodo == "H" ? "Hora" : epi.sTipoPeriodo == "D" ? "Dia" : epi.sTipoPeriodo == "S" ? "Semana" : epi.sTipoPeriodo == "M" ? "Mês" : "Ano";
                    //if (epi.nQuantidadeTempo > 1)
                    //{
                    //    descPeriodo += "s";
                    //}
                    //epi.sPeriodicidade = $"{epi.nQuantidadeTempo} {descPeriodo}";


                    listaEpis.Add(epi);
                }
            }

            this.bs_Recursos_EPIs = listaEpis;

            Popula_IncluirEPIs(dsPesquisa.Tables[0]);
            dtgEPI_DataBind();
            LimpaCampos_EPI();
        }

        void LimpaCampos_EPI()
        {
            hddIncluir_idEPI.Value = "0";
            txtCodigo_EPI.Text = string.Empty;
            txtDesc_EPI.Text = string.Empty;
            //txtIncluirEPI_CA.Text = "0";
            txtIncluirEPI_Qtd.Text = "1";
            //txtIncluirEPI_nTempo.Text = "1";
            //ddlIncluirEPI_sTipoPeriodo.SelectedValue = "H";

            //cmdEPI_Incluir.Visible = true;
        }

        void dtgEPI_DataBind()
        {
            try
            {
                // Use a nova lista bs_Recursos_EPIs
                dtgRecursosEPI.DataSource = bs_Recursos_EPIs.Where(c => c.sFuncao != "EXCLUIR_EPI");
                dtgRecursosEPI.DataBind();
                div_dtgEPI.Visible = dtgRecursosEPI.Rows.Count > 0;

                if (!string.IsNullOrEmpty(hddEPIs.Value))
                {
                    RegistraScript_Personalizado(txtCodigo_EPI.ClientID, true);
                    RegistraScript_Personalizado(txtDesc_EPI.ClientID, false);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Incluir_EPI.MostraMensagem_Erro("Erro ao Carregar os EPIs: " + ex.Message);
            }

            //string scriptText = "$('.mascara-quantidade').mask('000.000.000.000.000,00', { reverse: true });";
            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_MascaraGridGeral", scriptText, true);
        }

        void AbrirTab()
        {
            string scriptAtivaTab = "$('#EpiConsumiveis-tab').tab('show');";
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowEpiTab", scriptAtivaTab, true);
        }

        protected void dtgRecursosEPI_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                cls_Flow_Recursos_EPI dataItem = (cls_Flow_Recursos_EPI)e.Row.DataItem;

                if (dataItem != null && dataItem.sFuncao == "EXCLUIR")
                {
                    e.Row.CssClass = " danger";

                    e.Row.ToolTip = "Este item será excluído ao salvar.";
                }
                else
                {
                    e.Row.CssClass = "";

                    e.Row.ToolTip = "O item será mantido.";
                }

                //DropDownList ddlsTipoPeriodo = (DropDownList)e.Row.FindControl("ddlsTipoPeriodo");

                //if (ddlsTipoPeriodo != null && dataItem != null)
                //{
                //    ddlsTipoPeriodo.ClearSelection();

                //    ListItem item = ddlsTipoPeriodo.Items.FindByValue(dataItem.sTipoPeriodo);

                //    if (item != null)
                //    {
                //        item.Selected = true;
                //    }
                //}

                //TextBox txtnQuantidade = (TextBox)e.Row.FindControl("txtnQuantidade");
                //string scriptText = "$('.mascara-quantidade').mask('000.000.000.000.000,00', { reverse: true });";
                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_MascaraGridGeral", scriptText, true);
            }
        }

        protected void dtgRecursosEPI_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(e.Keys["idLinha"]);

            cls_Flow_Recursos_EPI itemParaExcluir = bs_Recursos_EPIs.FirstOrDefault(item => item.idLinha == idLinha);

            if (itemParaExcluir != null)
            {
                if (itemParaExcluir.idRegistro > 0)
                {
                    if (itemParaExcluir.sFuncao == "EXCLUIR")
                    {
                        itemParaExcluir.sFuncao = "INCLUIR";
                    }
                    else
                    {
                        itemParaExcluir.sFuncao = "EXCLUIR";
                    }
                }
                else
                {
                    if (itemParaExcluir.sFuncao == "EXCLUIR")
                    {
                        itemParaExcluir.sFuncao = "INCLUIR";
                    }
                    else
                    {
                        itemParaExcluir.sFuncao = "EXCLUIR";
                    }
                    //bs_Recursos_EPIs.Remove(itemParaExcluir);
                }
            }

            MensagemPagina_Incluir_EPI.MostraMensagem_Aviso("<b>Aviso</b> Para Efetivar a Exclusão, Salve os dados, e/ou para cancelar uma exclusão, clique novamente no botão de excluir.");
            AbrirTab();
            dtgEPI_DataBind();
        }

        bool Salvar_EPI()
        {

            if (!AtualizaClasses_EPI(true))
            {
                AbrirTab();
                return false;
            }
            else
            {
                try
                {
                    Dictionary<string, string> vParametroExcluir = new Dictionary<string, string>()
        {
            { "@sFuncao", "EXCLUIR_EPI" },
            { "@idProduto", hddidProduto.Value }
        };
                    ExecutarDataSet(sProcedure, vParametroExcluir);


                    foreach (var epiLinha in bs_Recursos_EPIs.Where(e => e.sFuncao != "EXCLUIR"))
                    {
                        Dictionary<string, string> vParametroInserir = new Dictionary<string, string>()
            {
                { "@sFuncao", "SALVAR_EPI" },
                { "@idRecurso", hddidProduto.Value },
                { "@idEPI", epiLinha.idEPI.ToString() },
                { "@nQuantidade", epiLinha.nQuantidade.ToString() }
                //{ "@nCA", epiLinha.nCA.ToString() },
                //{ "@nPeriodicidade", epiLinha.nQuantidadeTempo.ToString() },
                //{ "@sTipoPeriodo", epiLinha.sTipoPeriodo }
            };
                        ExecutarDataSet(sProcedure, vParametroInserir);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    MensagemPagina_Incluir_EPI.MostraMensagem_Erro("Erro ao Salvar os EPIs: " + ex.Message);
                    AbrirTab();
                    return false;
                }
            }

        }

        bool AtualizaClasses_EPI(bool bValidaDuplicado)
        {
            List<string> erros = new List<string>();

            foreach (GridViewRow row in dtgRecursosEPI.Rows)
            {
                int idLinha = Convert.ToInt32(dtgRecursosEPI.DataKeys[row.RowIndex].Value);
                var epi = bs_Recursos_EPIs.FirstOrDefault(e => e.idLinha == idLinha);

                if (epi != null)
                {
                    bool linhaValida = true;
                    int nQuantidade = 0; /*nQuantidadeTempo = 0, nCA = 0*/

                    //TextBox txtCA = (TextBox)row.FindControl("txtCA");
                    //if (!int.TryParse(txtCA.Text, out nCA))
                    //{
                    //    linhaValida = false;
                    //    erros.Add($"Linha {row.RowIndex + 1}: O valor no campo 'CA' não é um número válido.");
                    //}

                    TextBox txtQuantidade = (TextBox)row.FindControl("txtnQuantidade");
                    if (!int.TryParse(txtQuantidade.Text, out nQuantidade))
                    {
                        linhaValida = false;
                        erros.Add($"Linha {row.RowIndex + 1}: O valor no campo 'Quantidade' não é um número válido.");
                    }
                    else
                    {
                        if (Convert.ToInt32(txtQuantidade.Text) == 0)
                        {
                            linhaValida = false;
                            erros.Add($"Linha {row.RowIndex + 1}: O valor no campo 'Quantidade' deve ser maior que 0.");
                        }
                    }


                    //TextBox txtQuantidadeTempo = (TextBox)row.FindControl("txtnQuantidadeTempo");
                    //if (!int.TryParse(txtQuantidadeTempo.Text, out nQuantidadeTempo))
                    //{
                    //    linhaValida = false;
                    //    erros.Add($"Linha {row.RowIndex + 1}: O valor no campo 'Tempo de Troca' não é um número válido.");
                    //}

                    //DropDownList ddlPeriodo = (DropDownList)row.FindControl("ddlsTipoPeriodo");
                    //if (string.IsNullOrEmpty(ddlPeriodo.SelectedValue))
                    //{
                    //    linhaValida = false;
                    //    erros.Add($"Linha {row.RowIndex + 1}: O campo 'Período' precisa ser selecionado.");
                    //}

                    if (linhaValida)
                    {
                        //epi.nCA = nCA;
                        epi.nQuantidade = nQuantidade;
                        //epi.nQuantidadeTempo = nQuantidadeTempo;
                        //epi.sTipoPeriodo = ddlPeriodo.SelectedValue;
                    }
                }
            }

            if (erros.Any())
            {
                string mensagemFinal = string.Join("<br />", erros);
                MensagemPagina_Incluir_EPI.MostraMensagem_Erro(mensagemFinal, true);
                AbrirTab();
                return false;
            }

            if (bValidaDuplicado)
            {
                var gruposDuplicados = bs_Recursos_EPIs
                    .Where(e => e.sFuncao != "EXCLUIR")
                    .GroupBy(e => new { e.idEPI })
                    .Where(g => g.Count() > 1);

                if (gruposDuplicados.Any())
                {
                    MensagemPagina_Incluir_EPI.MostraMensagem_Erro("Não é possível salvar, pois existem EPIs duplicados. Verifique a lista.", true);
                    AbrirTab();
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region | Consumivel

        public List<cls_WMS_Produto_Consumivel> bs_Consumivel_Produto
        {
            get
            {
                if (ViewState["bs_Consumivel_Produto"] == null)
                {
                    ViewState["bs_Consumivel_Produto"] = new List<cls_WMS_Produto_Consumivel>();
                }
                return (List<cls_WMS_Produto_Consumivel>)ViewState["bs_Consumivel_Produto"];
            }
            set
            {
                ViewState["bs_Consumivel_Produto"] = value;
            }
        }

        void Popular_Aba_Consumivel()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>()
    {
        { "@sFuncao", "CONSULTAR-CONSUMIVEIS" },
        { "@idProduto", hddidProduto.Value }
    };

            DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

            var listaConsumiveis = new List<cls_WMS_Produto_Consumivel>();
            int linhaCounter = 0;

            if (dsPesquisa != null && dsPesquisa.Tables.Count > 0 && dsPesquisa.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dsPesquisa.Tables[0].Rows)
                {
                    var consumivel = new cls_WMS_Produto_Consumivel
                    {
                        idProdutoConsumivel = Convert.ToInt32(row["idRegistro"]),
                        idConsumivel = Convert.ToInt32(row["idConsumiveis"]),
                        idProduto = Convert.ToInt32(row["idRecurso"]),
                        sDescricao = row["sDscProduto"].ToString(),
                        sUnidade = row["sUnidade"].ToString(),
                        nQtd = Convert.ToDecimal(row["nQuantidade"]),
                        sCodigo = row["sCodigo"].ToString(),
                        idLinha = ++linhaCounter,
                        sFuncao = "CONSULTA"
                    };
                    listaConsumiveis.Add(consumivel);
                }
            }

            this.bs_Consumivel_Produto = listaConsumiveis;

            FiltroPesquisa2.sRegistraUnidade = "S";
            FiltroPesquisa2.ModificaTamanhoCampos(2, 4, 2, 2, 0);
            FiltroPesquisa2.desligaColapso(false);
            FiltroPesquisa2.Controle_ExibicaoCampos(new Dictionary<string, bool> { { "Valor Unitário", false } }, "1");
            FiltroPesquisa2.RegistrarScriptPesquisarMovimentacao();

            dtgv_consumivel_produto_DataBind();
        }

        private void AtualizarClasse_Consumivel()
        {
            foreach (GridViewRow row in GridView1.Rows)
            {
                int idLinha = Convert.ToInt32(GridView1.DataKeys[row.RowIndex].Value);
                var consumivel = bs_Consumivel_Produto.FirstOrDefault(c => c.idLinha == idLinha);

                if (consumivel != null)
                {
                    var txtGridQuantidade = (TextBox)row.FindControl("txtGridQuantidade");
                    if (txtGridQuantidade != null)
                    {
                        int novaQtd = 0;
                        int.TryParse(txtGridQuantidade.Text, out novaQtd);

                        if (novaQtd <= 0)
                        {
                            AbrirTab();
                            string mensagemAviso = $"A quantidade do consumível '{consumivel.sDescricao}' foi zerada ou é inválida. O item será removido ao salvar.";
                            MensagemPagina1.MostraMensagem_Aviso(mensagemAviso);

                            consumivel.sFuncao = "EXCLUIR";

                            continue;
                        }

                        if (consumivel.nQtd != novaQtd)
                        {
                            consumivel.nQtd = novaQtd;
                            if (consumivel.idProdutoConsumivel > 0)
                            {
                                consumivel.sFuncao = "ATUALIZAR";
                            }
                        }
                    }
                }
            }
        }

        private bool ConsumivelSalvar()
        {
            AtualizarClasse_Consumivel();

            try
            {
                ExcluirConsumivel(hddidProduto.Value);

                foreach (var item in bs_Consumivel_Produto.Where(c => c.sFuncao != "EXCLUIR"))
                {
                    Dictionary<string, string> vParametrosConsumivel = new Dictionary<string, string>
            {
                { "@sFuncao", "Salvar_Consumivel" },
                { "@idConsumivel", item.idConsumivel.ToString()},
                { "@idProduto", item.idProduto.ToString() },
                { "@nQuantidade", item.nQtd.ToString().Replace(",",".") },
            };
                    ExecutarDataSet(sProcedure, vParametrosConsumivel);
                }
                return true;
            }
            catch (Exception e)
            {
                AbrirTab();
                MensagemPagina1.MostraMensagem_Erro("Erro ao salvar consumíveis: " + e.Message);
                return false;
            }
        }

        protected void cmdConsumivelProduto_Click(object sender, EventArgs e)
        {
            if (Consumivel_ValidarDados(FiltroPesquisa2))
            {
                int idConsumivelAdicionar = Convert.ToInt32(FiltroPesquisa2.IdItem);
                var itemExistente = bs_Consumivel_Produto.FirstOrDefault(p => p.idConsumivel == idConsumivelAdicionar);

                if (itemExistente != null)
                {
                    itemExistente.nQtd = Convert.ToDecimal(FiltroPesquisa2.NQuantidade);
                    if (itemExistente.idProdutoConsumivel > 0)
                    {
                        itemExistente.sFuncao = "ATUALIZAR";
                    }
                    MensagemPagina1.MostraMensagem_Aviso("Consumível já incluso. A quantidade foi atualizada.");
                }
                else
                {
                    int proximoIdLinha = (bs_Consumivel_Produto.Any() ? bs_Consumivel_Produto.Max(x => x.idLinha) : 0) + 1;
                    var objItem = new cls_WMS_Produto_Consumivel
                    {
                        idLinha = proximoIdLinha,
                        sFuncao = "INCLUIR",
                        idProduto = Convert.ToInt32(hddidProduto.Value),
                        idConsumivel = idConsumivelAdicionar,
                        sCodigo = FiltroPesquisa2.SCodigo,
                        sDescricao = FiltroPesquisa2.SDscProduto,
                        sUnidade = FiltroPesquisa2.SUnidade,
                        nQtd = Convert.ToDecimal(FiltroPesquisa2.NQuantidade)
                    };
                    bs_Consumivel_Produto.Add(objItem);
                }

                dtgv_consumivel_produto_DataBind();
                FiltroPesquisa2.LimparCampos();
                AbrirTab();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus_Consumivel", "$('[id$=FiltroPesquisa2_txtCodigo]').focus();", true);
                FiltroPesquisa2.ModificaTamanhoCampos(2, 4, 2, 2, 0);
                FiltroPesquisa2.Controle_ExibicaoCampos(new Dictionary<string, bool> { { "Valor Unitário", false } }, "1");
                FiltroPesquisa2.desligaColapso(false);
                FiltroPesquisa2.RegistrarScriptPesquisarMovimentacao();

            }
        }

        private void dtgv_consumivel_produto_DataBind()
        {
            GridView1.DataSource = bs_Consumivel_Produto.OrderBy(p => p.sDescricao).ToList();
            GridView1.DataBind();
        }

        private bool Consumivel_ValidarDados(FiltroPesquisa filtro)
        {
            string sMensagemErro = "";

            if (string.IsNullOrWhiteSpace(filtro.SCodigo) || filtro.SCodigo.Length < 3)
            {
                sMensagemErro += (sMensagemErro != "" ? "<br/>" : "") + "Código do Consumível Inválido.";
            }
            if (string.IsNullOrWhiteSpace(filtro.SDscProduto) || filtro.SDscProduto.Length < 6)
            {
                sMensagemErro += (sMensagemErro != "" ? "<br/>" : "") + "Descrição do Consumível inválida.";
            }
            if (filtro.NQuantidade <= decimal.Zero)
            {
                sMensagemErro += (sMensagemErro != "" ? "<br/>" : "") + "A Quantidade deve ser maior que zero.";
            }

            if (sMensagemErro != "")
            {
                AbrirTab();
                MensagemPagina1.MostraMensagem_Erro(sMensagemErro);
                return false;
            }

            return true;
        }

        private void ExcluirConsumivel(string idProduto)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
    {
        { "@sFuncao", "Excluir_Consumivel" },
        { "@idProduto", idProduto }
    };

            try
            {
                ExecutarDataSet(sProcedure, vParametros);
            }
            catch (Exception ex)
            {
                AbrirTab();
                MensagemPagina1.MostraMensagem_Erro("Erro ao excluir o consumível: " + ex.Message);
            }
        }

        protected void dtgv_consumivel_produto_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int rowIndex = e.RowIndex;

            int idLinha = Convert.ToInt32(GridView1.DataKeys[rowIndex].Values["idLinha"]);

            var itemParaExcluir = bs_Consumivel_Produto.FirstOrDefault(item => item.idLinha == idLinha);

            if (itemParaExcluir != null)
            {
                if (itemParaExcluir.idProdutoConsumivel > 0)
                {
                    if (itemParaExcluir.sFuncao == "EXCLUIR")
                    {
                        itemParaExcluir.sFuncao = "INCLUIR";
                    }
                    else
                    {
                        itemParaExcluir.sFuncao = "EXCLUIR";
                    }

                }
                else
                {
                    if (itemParaExcluir.sFuncao == "EXCLUIR")
                    {
                        itemParaExcluir.sFuncao = "INCLUIR";
                    }
                    else
                    {
                        itemParaExcluir.sFuncao = "EXCLUIR";
                    }
                    //bs_Consumivel_Produto.Remove(itemParaExcluir);
                }
            }

            MensagemPagina1.MostraMensagem_Aviso("<b>Aviso</b> Para Efetivar a Exclusão, Salve os dados, e/ou para cancelar uma exclusão, clique novamente no botão de excluir.");

            AbrirTab();
            dtgv_consumivel_produto_DataBind();
        }

        protected void dtgv_consumivel_produto_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                cls_WMS_Produto_Consumivel dataItem = (cls_WMS_Produto_Consumivel)e.Row.DataItem;

                if (dataItem != null && dataItem.sFuncao == "EXCLUIR")
                {
                    e.Row.CssClass = "danger";
                    e.Row.ToolTip = "Este item será excluído ao salvar.";
                }
                else
                {
                    e.Row.CssClass = ""; // Remove a classe 'danger'
                    e.Row.ToolTip = "O item será mantido";
                }
            }
        }

        #endregion

        #endregion
    }
}
