using NPOI.SS.Formula.Functions;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using TT.FrameWork;
using TT_Flow.App.Controles;
using TT_Flow.FrameWork;
using static Permissao;
using static TT_Flow.FrameWork.cls_Comercial_Tabelas;
using static TT_Flow.FrameWork.cls_WMS_MovimentacaoRel;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Paginas.WMS.Movimentacao
{
    public partial class Movimentacao_Detalhe : Page
    {
        private bool isDeleting = false;

        string urlPagina = "/App/Paginas/WMS/Movimentacao_Detalhe.aspx";
        string sProcedureLocal = "sp_Manipula_tbl_Flow_WMS_LocalArmazenamento";
        string sProcedureMov = "sp_Manipula_tbl_Flow_Produtos_Movimentacao";
        int indiceLocal = 4;
        int indicePosicao = 5;

        #region | Classes

        public string sRequestAcao
        {
            get
            {
                if (ViewState["sRequestAcao"] == null)
                {
                    if (Request["Acao"] != null) ViewState["sRequestAcao"] = Request["Acao"].ToString();
                    else return "";
                }
                return ViewState["sRequestAcao"].ToString();
            }
            set { ViewState["sRequestAcao"] = value; }
        }

        public EntidadeFuncoes<cls_WMS_MovimentacaoRel> conversorMovimentacao = new EntidadeFuncoes<cls_WMS_MovimentacaoRel>();

        public List<cls_WMS_MovimentacaoRel> bs_Movimentacao
        {
            get
            {
                if (ViewState["bs_Movimentacao"] == null) ViewState["bs_Movimentacao"] = new List<cls_WMS_MovimentacaoRel>();
                return (List<cls_WMS_MovimentacaoRel>)ViewState["bs_Movimentacao"];
            }
            set { ViewState["bs_Movimentacao"] = value; }
        }

        public List<cls_WMS_MovimentacaoRel> bs_ItemEntrada
        {
            get
            {
                if (ViewState["bs_ItemEntrada"] == null) ViewState["bs_ItemEntrada"] = new List<cls_WMS_MovimentacaoRel>();
                return (List<cls_WMS_MovimentacaoRel>)ViewState["bs_ItemEntrada"];
            }
            set { ViewState["bs_ItemEntrada"] = value; }
        }

        public List<cls_WMS_MovimentacaoRel> bs_ItemSaida
        {
            get
            {
                if (ViewState["bs_ItemSaida"] == null) ViewState["bs_ItemSaida"] = new List<cls_WMS_MovimentacaoRel>();
                return (List<cls_WMS_MovimentacaoRel>)ViewState["bs_ItemSaida"];
            }
            set { ViewState["bs_ItemSaida"] = value; }
        }

        public List<cls_WMS_MovimentacaoRel> bs_ItemIncluirSerie
        {
            get
            {
                if (ViewState["bs_ItemIncluirSerie"] == null) ViewState["bs_ItemIncluirSerie"] = new List<cls_WMS_MovimentacaoRel>();
                return (List<cls_WMS_MovimentacaoRel>)ViewState["bs_ItemIncluirSerie"];
            }
            set { ViewState["bs_ItemIncluirSerie"] = value; }
        }

        public List<cls_WMS_MovimentacaoRel> bs_ItemMudanca
        {
            get
            {
                if (ViewState["bs_ItemMudanca"] == null) ViewState["bs_ItemMudanca"] = new List<cls_WMS_MovimentacaoRel>();
                return (List<cls_WMS_MovimentacaoRel>)ViewState["bs_ItemMudanca"];
            }
            set { ViewState["bs_ItemMudanca"] = value; }
        }

        //public List<cls_WMS_Movimentacao_Motivo> BS_MOTIVO
        //{
        //    get
        //    {
        //        if (ViewState["BS_MOTIVO"] == null) ViewState["BS_MOTIVO"] = new List<cls_WMS_Movimentacao_Motivo>();
        //        return (List<cls_WMS_Movimentacao_Motivo>)ViewState["BS_MOTIVO"];
        //    }
        //    set { ViewState["BS_TIPO_MOVIMENTACAO"] = value; }
        //}

        //public List<cls_WMS_Movimentacao_Motivo> BS_TIPO_MOVIMENTACAO
        //{
        //    get
        //    {
        //        if (ViewState["BS_TIPO_MOVIMENTACAO"] == null) ViewState["BS_TIPO_MOVIMENTACAO"] = new List<cls_WMS_Movimentacao_Motivo>();
        //        return (List<cls_WMS_Movimentacao_Motivo>)ViewState["BS_TIPO_MOVIMENTACAO"];
        //    }
        //    set { ViewState["BS_TIPO_MOVIMENTACAO"] = value; }
        //}

        //public List<cls_WMS_MovimentacaoRel> bs_ItemNSerie
        //{
        //    get
        //    {
        //        if (ViewState["bs_ItemNSerie"] == null)
        //        {
        //            ViewState["bs_ItemNSerie"] = new List<cls_WMS_MovimentacaoRel>();
        //        }
        //        return (List<cls_WMS_MovimentacaoRel>)ViewState["bs_ItemNSerie"];
        //    }
        //    set
        //    {
        //        ViewState["bs_ItemNSerie"] = value;
        //    }
        //}

        //public List<cls_WMS_Produtos> bs_Itens_Pedido_Compra
        //{
        //    get
        //    {
        //        if (ViewState["bs_Itens_Pedido_Compra"] == null) ViewState["bs_Itens_Pedido_Compra"] = new List<cls_WMS_Produtos>();
        //        return (List<cls_WMS_Produtos>)ViewState["bs_Itens_Pedido_Compra"];
        //    }
        //    set { ViewState["bs_Itens_Pedido_Compra"] = value; }
        //}

        #endregion

        #region | Page Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            string eventTarget = Request.Params["__EVENTTARGET"];
            if (!string.IsNullOrEmpty(eventTarget) && eventTarget.Contains("ddlArmazenamento"))
                AbrirModal();

            ScriptManager.GetCurrent(this).RegisterPostBackControl(btnProcessarXML);

            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();

            if (!IsPostBack)
            {
                hfModalAberta.Value = "";
                PopularCombos();

                if (Request["Acao"] == "ImportarXML")
                    PrepararTelaParaImportacao();
                else
                {
                    if (Request["id"] != null)
                        Pesquisar(Request["id"].ToString());
                    else
                    {
                        Pesquisar("0");
                        divItens.Visible = true;
                    }
                }

                if (Session["MensagemSucesso"] != null)
                {
                    MensagemPagina.MostraMensagem_Sucesso(Session["MensagemSucesso"].ToString());
                    Session.Remove("MensagemSucesso");
                }

                // Exibir mensagem de erro
                if (Session["MensagemErro"] != null)
                {
                    MensagemPagina.MostraMensagem_Erro(Session["MensagemErro"].ToString());
                    Session.Remove("MensagemErro");
                }

                if (Session["MensagemSucessoSeries"] != null)
                {
                    MensagemPaginaItens.MostraMensagem_Sucesso(Session["MensagemSucessoSeries"].ToString());
                    Session.Remove("MensagemSucessoSeries");
                }

                if (Session["MensagemAlertaSeries"] != null)
                {
                    MensagemPaginaItens.MostraMensagem_Aviso("<b>Aviso: </b>" + Session["MensagemAlertaSeries"].ToString(), false);
                    Session.Remove("MensagemAlertaSeries");
                }

                if (Session["MensagemErrorSeries"] != null)
                {
                    MensagemPaginaItens.MostraMensagem_Erro("<b>Erro: </b>" + Session["MensagemErrorSeries"].ToString(), false);
                    Session.Remove("MensagemErrorSeries");
                }

                if (Session["MensagemAlertaSeriesSalvar"] != null)
                {
                    MensagemPaginaItens.MostraMensagem_Aviso(Session["MensagemAlertaSeriesSalvar"].ToString(), false);
                    Session.Remove("MensagemAlertaSeriesSalvar");
                }

                divItensDestino.Visible = false;

                Session["CameraAbertaValidacao"] = false;
                DivBipadorValidacao.Visible = false;
            }
            else
            {
                // Recuperando estados de visibilidade do ViewState
                if (ViewState["divLoteVisible"] != null) div_Lote.Visible = (bool)ViewState["divLoteVisible"];
                if (ViewState["divItensVisible"] != null) divItens.Visible = (bool)ViewState["divItensVisible"];
                if (ViewState["divMotivoVisible"] != null) div_Motivo.Visible = (bool)ViewState["divMotivoVisible"];
                if (ViewState["divItensDestinoVisible"] != null) divItensDestino.Visible = (bool)ViewState["divItensDestinoVisible"];
            }

            var requestTarget = Request["__EVENTTARGET"];
            if (requestTarget == "funcao_SAIR")
                FUNCOES.DirecionaPagina("/app/dashboard.aspx");
            else if (requestTarget == "funcao_SALVAR")
            {
                if (!string.IsNullOrEmpty(Request["id"]) && Request["id"] != "0")
                    SalvarDados("S");
                else if (string.Equals(sRequestAcao, "ImportarXML", StringComparison.OrdinalIgnoreCase))
                    SalvarImportacaoXML();
                else
                    SalvarDados("N");
            }
            else if (requestTarget == "funcao_Editar")
                Pesquisar(hddidMovimentacao.Value);

            if (string.IsNullOrEmpty(Request["id"]) || Request["id"] == "0")
            {
                if (chkAlternar.Checked)
                    ddlFornecedor.Attributes.Add("Disabled", "Disabled");
                else if (!chkAlternar.Checked && (hddidMovimentacao.Value == "0" || string.IsNullOrEmpty(hddidMovimentacao.Value)) && ddlMotivo.SelectedValue != "4")
                    ddlFornecedor.Attributes.Remove("Disabled");
            }

            RegistrarScriptPagina();

            txtdtMovimentacao.Text = DateTime.Now.ToString(@"yyyy/MM/dd").Replace('/', '-');
            Pesquisa_Parceiros.RegistrarScriptPesquisarItens();
            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa.ModificaTamanhoCampos(2, 6, 2, 2, 0);

            if (ddlsTipoMovimentacao.SelectedValue == "5")
            {
                FiltroPesquisaMudanca.SAtivaPostBack = "S";
                FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
                FiltroPesquisaMudanca.ModificaTamanhoCampos(2, 6, 2, 2, 0);
            }

            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.ModificaTamanhoCampos(2, 6, 2, 2, 0);

            if (ddlsTipoMovimentacao.SelectedValue == "2" || ddlsTipoMovimentacao.SelectedValue == "3")
            {
                if (ddlMotivo.SelectedValue == "1" || ddlMotivo.SelectedValue == "2" || ddlMotivo.SelectedValue == "3")
                    FiltroPesquisa.ModificaTamanhoCampos(2, 4, 2, 2, 2);
            }

            var chaves = new[] { "Valor Unitário" };
            var dicDesativarValor = chaves.ToDictionary(k => k, v => false);
            FiltroPesquisa1.Controle_ExibicaoCampos(dicDesativarValor, "1");

            GuiarUsuario();
            dtgItens_DataBind();

            if (IsPostBack && !string.IsNullOrEmpty(FiltroPesquisa1.IdItem))
            {
                if (ViewState["UltimoIdProduto"].ToString() != FiltroPesquisa1.IdItem)
                {
                    PopularLocal(FiltroPesquisa1.IdItem);
                    ViewState["UltimoIdProduto"] = FiltroPesquisa1.IdItem;
                }
            }

            RegistrarScriptAutocompleteGrid();
            ImportadorItensModal.OnItensImportados += ImportadorItensModal_ItensImportados;

            // Lincagem do UserControl LeitorQuagga com os inputs da sua tela
            LeitorQuaggaMovimentacao.txtClient = txtCodigoBarrasValidacao.ClientID;
            LeitorQuaggaMovimentacao.click = cmdProcessarLeituraValidacao.ClientID;

            LeitorQuaggaTodos.txtClient = txtCodigoBarrasTodos.ClientID;
            LeitorQuaggaTodos.click = cmdProcessarLeituraTodos.ClientID;

            if (LeitorQuaggaMudanca != null)
            {
                LeitorQuaggaMudanca.txtClient = txtCodigoBarrasMudanca.ClientID;
                LeitorQuaggaMudanca.click = cmdProcessarLeituraMudanca.ClientID;
            }
        }

        protected void Pesquisar(string idPesquisa)
        {
            var chaves = new[] { "Valor Unitário" };
            var dicAtivarValor = chaves.ToDictionary(k => k, v => true);
            var dicDesativarValor = chaves.ToDictionary(k => k, v => false);

            try
            {
                PopularClasseMotivo();

                if (idPesquisa != "0")
                {
                    DataSet dsMovimentacao = cls_WMS_MovimentacaoRel.Movimentacao_ConsultarDetalhe(idPesquisa); //obtem a pesquisa pelo ID
                    if (BD.ValidarDataSet(dsMovimentacao, out _))
                    {
                        Movimentacao_ConverterDS_BD(dsMovimentacao, bs_Movimentacao);// atualiza a classe
                        hddidMovimentacao.Value = RETORNO.DATASET(dsMovimentacao, 0, "idMovimentacao");
                        txtidMovimentacao.Text = RETORNO.DATASET(dsMovimentacao, 0, "idMovimentacao");
                       
                        var dt = Convert.ToDateTime(RETORNO.DATASET(dsMovimentacao, 0, "dtMovimentacao").ToString());
                        txtdtMovimentacao.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');
                        hdddtEfetivado.Value = RETORNO.DATASET(dsMovimentacao, 0, "dtEfetivado");

                        FUNCOES.Popula_Combo(ddlsTipoMovimentacao, "sp_Select 'Flow_WMS_Produtos_Movimentacao_Tipo', @idFiltro = 0", "idTipoMovimentacao", "sDscTipoMovimentacao", false, "Selecione o tipo de Mov.", "0");

                        ddlsTipoMovimentacao.SelectedValue = RETORNO.DATASET(dsMovimentacao, 0, "idTipoMovimentacao");
                        ddlMotivo.SelectedValue = RETORNO.DATASET(dsMovimentacao, 0, "idMotivo");
                        hddidStatus.Value = RETORNO.DATASET(dsMovimentacao, 0, "idStatus");
                        hddsLiberaSerie.Value = RETORNO.DATASET(dsMovimentacao, 0, "sLiberaNSerie");
                        hddsLiberaImpressao.Value = RETORNO.DATASET(dsMovimentacao, 0, "sExibeBotaoImprimir");

                        spanStatus.InnerText = RETORNO.DATASET(dsMovimentacao, 0, "sDscStatus");
                        spanStatus.Attributes["class"] = $"label label-{RETORNO.DATASET(dsMovimentacao, "sCorStatus")}";

                        cmdEfetivar.Visible = RETORNO.DATASET(dsMovimentacao, "sLiberaMovimentacao") == "S" && string.IsNullOrEmpty(RETORNO.DATASET(dsMovimentacao, "dtEfetivado"));

                        if (ddlsTipoMovimentacao.SelectedValue == "2")
                            cmdImprimirEtiquetas.Visible = RETORNO.DATASET(dsMovimentacao, 0, "sExibeBotaoImprimir") == "S" && string.IsNullOrEmpty(RETORNO.DATASET(dsMovimentacao, 0, "dtEfetivado"));

                        // Exibir botão de Validar Todos se estiver pendente de validação e se libera série
                        cmdValidarTodos.Visible = hddsLiberaSerie.Value == "S" && (hddidStatus.Value == "2" || hddidStatus.Value == "6");

                        txtsNotaFiscal.Text = RETORNO.DATASET(dsMovimentacao, 0, "sNotaFiscal");
                        txtsDscObservacao.Text = RETORNO.DATASET(dsMovimentacao, 0, "sObservacao");
                        Pesquisa_Parceiros.SCnpj_CPF = RETORNO.DATASET(dsMovimentacao, 0, "sCNPJ_CPF");
                        Pesquisa_Parceiros.SRazaoSocial = RETORNO.DATASET(dsMovimentacao, 0, "sRazaoSocial");
                        txtsPedidoCompra.Text = RETORNO.DATASET(dsMovimentacao, 0, "sPO");
                        ddlFornecedor.SelectedValue = RETORNO.DATASET(dsMovimentacao, 0, "idParceiro");
                        hddidFornecedor.Value = ddlFornecedor.SelectedValue;
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsMovimentacao, 0, "dtInclusao"), RETORNO.DATASET(dsMovimentacao, 0, "sDscUsuarioAtualizacao"));

                        dtgItens_DataBind();

                        FiltroPesquisa.Visible = false;
                        Pesquisa_Parceiros.Visible = false;
                        cmdIncluir.Visible = false;
                        cmdSalvar.Visible = false;
                        cmdExcluir.Visible = hddidStatus.Value != "7" && string.IsNullOrEmpty(hdddtEfetivado.Value);
                        ddlFornecedor.Attributes.Add("disabled", "disabled");

                        if (ddlsTipoMovimentacao.SelectedValue == "2")
                        {
                            lblFornecedor.InnerText = "Fornecedor";

                            if (ddlMotivo.SelectedValue == "1" || ddlMotivo.SelectedValue == "9")
                            {
                                div_Impressora.Visible = true;

                                PopularImpressora();
                                ddlImpressora.SelectedValue = RETORNO.DATASET(dsMovimentacao, "idImpressora");

                                if (hddidStatus.Value == "6")
                                    ddlImpressora.Attributes.Remove("disabled");
                                else ddlImpressora.Attributes.Add("disabled", "disabled");
                            }
                            else div_Impressora.Visible = false;
                        }
                        else if (ddlsTipoMovimentacao.SelectedValue == "3")
                        {
                            lblFornecedor.InnerText = "Cliente";
                            div_Lote.Visible = false;
                        }
                        else
                        {
                            lblFornecedor.InnerText = "Empresa";
                            div_Impressora.Visible = false;
                            div_Lote.Visible = false;
                            div_Impressora.Visible = false;
                            div_Form.Visible = false;
                        }
                    }

                    if (ddlsTipoMovimentacao.SelectedValue == "5")
                    {
                        divFpMudanca.Visible = false;
                        divItens.Visible = false;
                        divItensDestino.Visible = false;
                        divMudancaPosicao.Visible = true;
                        div_Motivo.Visible = true;
                        div_Descricao.Visible = true;
                        cmdValidarTodos.Visible = false;

                        bs_ItemMudanca = bs_Movimentacao.Where(x => x.STipoMov == "Saída").ToList();

                        dtgMudancaPosicao.DataSource = bs_ItemMudanca;
                        dtgMudancaPosicao.DataBind();
                    }

                    ConfigurarControles(false);
                    FiltroPesquisa.ConfigurarControles(false);
                    Pesquisa_Parceiros.ConfigurarControles(false);
                    FiltroPesquisaMudanca.ConfigurarControles(false);
                    chkAlternar.Visible = false;

                    if (ddlsTipoMovimentacao.SelectedValue == "2" && ddlMotivo.SelectedValue == "1")
                        div_Lote.Visible = true;
                    else
                    {
                        div_Lote.Visible = false;
                        if (!string.IsNullOrEmpty(txtsPedidoCompra.Text) && ddlsTipoMovimentacao.SelectedValue == "4" && ddlMotivo.SelectedValue == "1")
                        {
                            div_Lote.Visible = true;
                            txtsPedidoCompra.Visible = true;
                        }
                    }

                    if (string.IsNullOrEmpty(txtsNotaFiscal.Text))
                        txtsNotaFiscal.Attributes["placeholder"] = "Sem Nota Fiscal";

                    div_Motivo.Visible = true;
                    ViewState["isNovo"] = "false";
                }
                else
                {
                    LimparTela();
                    ViewState["isNovo"] = "true";
                    txtdtMovimentacao.Text = DateTime.Now.ToString(@"yyyy/MM/dd").Replace('/', '-');
                    BreadCrumb_Pagina.TitulodaPagina = "Nova";

                    //chkAlternar.Visible = true;
                    //chkAlternar.Checked = false;
                    //cmdEfetivar.Visible = false;
                    //FiltroPesquisa.HabilitarItemQtde(true);
                    //ddlFornecedor.Attributes.Remove("disabled");
                    //cmdSalvar.Text = "Salvar";
                    //

                    //div_Form.Visible = false;
                    //div_Descricao.Visible = false;
                    //divItens.Visible = false;
                    //FiltroPesquisa.ModificaTamanhoCampos(2, 6, 2, 2, 0);
                    //FiltroPesquisa.Controle_ExibicaoCampos(dicDesativarValor, "1");

                    //FiltroPesquisaMudanca.ModificaTamanhoCampos(2, 6, 2, 2, 0);
                    //FiltroPesquisaMudanca.Controle_ExibicaoCampos(dicDesativarValor, "1");

                    //hddidStatus.Value = "6";
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.ToString(), false);
            }
        }

        #endregion

        #region | Métodos Iniciais

        void LimpaCampos()
        {
            ddlsTipoMovimentacao.SelectedValue = "0";
            ddlMotivo.SelectedValue = "0";
            txtsNotaFiscal.Text = "";
            FiltroPesquisa.LimparCampos();
            FiltroPesquisaMudanca.LimparCampos(); // Adicionar esta linha

        }

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlsTipoMovimentacao, "sp_Select 'Flow_WMS_Produtos_Movimentacao_Tipo', @idFiltro = 1", "idTipoMovimentacao", "sDscTipoMovimentacao", false,
            "Selecione o tipo de Movimentação", "0");
            FUNCOES.Popula_Combo(ddlMotivo, "sp_Select 'Flow_WMS_Produtos_Movimentacao_Motivo'", "idMotivo", "sDscMotivo", false,
            "Selecione o Motivo.", "0");

            PopularComboFornecedor("0");

            FUNCOES.Popula_Combo(ddlsPO, "sp_Manipula_tbl_Flow_Produtos_Movimentacao 'FLOW-PEDIDO-COMPRA'", "idPedido", "sPedidoCompras", false, "Selecione o Pedido", "0");

            if (DateTime.TryParse(txtdtMovimentacao.Text, out DateTime dataMovimentacao) && dataMovimentacao < DateTime.Today)
            {
                FUNCOES.Popula_Combo(ddlsTipoMovimentacao, "sp_Select 'Flow_WMS_Produtos_Movimentacao_Tipo', @idFiltro = 0", "idTipoMovimentacao", "sDscTipoMovimentacao", false,
                "Selecione o tipo de Mov.", "0");
            }

        }

        void PopularComboFornecedor(string sMotivo)
        {
            if (sMotivo == "4")
                FUNCOES.Popula_Combo(ddlFornecedor, "sp_Manipula_tbl_Flow_Produtos_Movimentacao 'FLOW_PESQUISA_PARCEIROS-MOV'", "idCliente", "Razao_CNPJ", false, "Selecione a Empresa", "0");
            else
                FUNCOES.Popula_Combo(ddlFornecedor, "sp_Select 'Flow_Parceiros_Fornecedores', @sTipo='N'", "idCliente", "Razao_CNPJ", false, "Selecione a Empresa", "0");
        }

        void PopularImpressora() => FUNCOES.Popula_Combo(ddlImpressora, "sp_Manipula_tbl_Flow_WMS_OPI_Etiqueta 'FLOW-IMPRESSORA'", "idImpressora", "sDscImpressora", false, "Selecione a Impressora", "0");

        void PopularClasseMotivo()
        {
            //string sErro = "";
            //DataSet dsMotivo = Movimentacao_Consultar_Motivo_x_Tipo();
            //if (BD.ValidarDataSet(dsMotivo, out sErro))
            //{
            //    Movimentacao_ConverterDS_BD(dsMotivo, BS_MOTIVO);
            //}
        }

        void PopularCombo_Motivo(string idTipoMovimentacao)
        {
            if (idTipoMovimentacao == "T")
                FUNCOES.Popula_Combo(ddlMotivo, $"{sProcedureMov} 'FLOW_MOTIVO'", "idMotivo", "sDscMotivo", false, "Selecione um Motivo", "0");
            else
                FUNCOES.Popula_Combo(ddlMotivo, $"{sProcedureMov} 'FLOW_MOTIVO', @idTipoMovimentacao ={idTipoMovimentacao}", "idMotivo", "sDscMotivo", false, "Selecione um Motivo", "0");
            div_Motivo.Visible = true;
        }

        protected void ddlLocalMov_SelectedIndexChanged(object sender, EventArgs e)
        {
            FUNCOES.Popula_Combo(ddlPosicaoPai, $"{sProcedureLocal} 'FLOW-POSICOES', @idLocalArmazenamento={ddlLocal.SelectedValue}", "idPosicao", "sCodigoLocal", false, "Selecione a Posicao", "0");
        }

        #endregion

        #region | XML Importação 

        private void PrepararTelaParaImportacao()
        {
            // 1. Configura Layout
            lblTituloPagina.Text = "Entrada de Mercadoria Via XML NFe";
            divImportacaoXML.Visible = true;
            divItens.Visible = false;
            divItensDestino.Visible = false;
            div_Motivo.Visible = true;
            chkAlternar.Visible = false;
            div_Descricao.Visible = false;
            cmdEfetivar.Visible = false;
            PainelAtualizacao.Visible = false;
            txtsPedidoCompra.Text = "Importação XML";
            txtsNotaFiscal.Attributes.Add("disabled", "disabled");

            lblFornecedor.InnerText = "Fornecedor";

            ddlsTipoMovimentacao.SelectedValue = "2";
            ddlsTipoMovimentacao.Attributes.Add("disabled", "disabled");

            PopularCombo_Motivo(ddlsTipoMovimentacao.SelectedValue);
            //ddlMotivo.SelectedValue = "1";
            //ddlMotivo.Attributes.Add("disabled", "disabled");

            ddlFornecedor.Enabled = true;
        }

        private string BuscarIdFornecedorPorCNPJ(string cnpj)
        {
            // Validação básica
            if (string.IsNullOrEmpty(cnpj)) return "0";

            string idEncontrado = "0";

            try
            {
                // Limpa formatação do CNPJ recebido para garantir o match no banco
                string cnpjLimpo = cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Trim();

                // Query
                string sql = $"SELECT TOP 1 idCliente FROM tbl_Flow_Clientes WHERE replace(replace(replace(sCPF_CNPJ, '.', ''), '/', ''), '-', '') = '{cnpjLimpo}'";

                // Executa e recebe o Leitor (Reader)
                SqlDataReader dr = BD.ExecutarDataReader(sql);

                // Verifica se o leitor não veio nulo
                if (dr != null)
                {
                    // O comando .Read() tenta ler a primeira linha retornada pelo banco
                    if (dr.Read())
                    {
                        // Agora sim pegamos o valor da coluna "idCliente"
                        idEncontrado = dr["idCliente"].ToString();
                    }

                    // IMPORTANTE: Fechar o reader libera a conexão com o banco
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                // Opcional: logar o erro para debug
                System.Diagnostics.Debug.WriteLine("Erro ao buscar CNPJ: " + ex.Message);
            }

            return idEncontrado;
        }

        protected void btnProcessarXML_Click(object sender, EventArgs e)
        {
            // Verifica se o arquivo foi enviado (Necessário PostBackTrigger no UpdatePanel)
            if (fupArquivoXML.HasFile)
            {
                try
                {
                    using (Stream stream = fupArquivoXML.PostedFile.InputStream)
                    {
                        // Carrega o XML
                        XDocument xml = XDocument.Load(stream);
                        XNamespace ns = "http://www.portalfiscal.inf.br/nfe";

                        // --- 1. IDENTIFICAR DADOS NO XML ---
                        var ide = xml.Descendants(ns + "ide").FirstOrDefault();
                        var emit = xml.Descendants(ns + "emit").FirstOrDefault();
                        var dest = xml.Descendants(ns + "dest").FirstOrDefault();

                        string tpNF = ide?.Element(ns + "tpNF")?.Value; // 0=Entrada, 1=Saída
                        string nNFe = ide?.Element(ns + "nNF")?.Value;
                        string cNFe = ide?.Element(ns + "cNF")?.Value;
                        string cnpjEmitenteXml = emit?.Element(ns + "CNPJ")?.Value;

                        // Variáveis para definir quem é o Parceiro Comercial dessa operação
                        string cnpjParceiro = "";
                        string nomeParceiro = "";

                        // LÓGICA DE IMPORTAÇÃO / EMISSÃO PRÓPRIA
                        bool isImportacaoOuEmissaoPropria = (tpNF == "0" && cnpjEmitenteXml == "19132916000123");

                        if (isImportacaoOuEmissaoPropria)
                        {
                            // É Importação: O Parceiro é o Destinatário
                            nomeParceiro = dest?.Element(ns + "xNome")?.Value;

                            // Tenta pegar CNPJ se tiver (parceiro nacional), senão define vazio (estrangeiro)
                            if (dest?.Element(ns + "CNPJ") != null)
                                cnpjParceiro = dest.Element(ns + "CNPJ").Value;
                            else if (dest?.Element(ns + "CPF") != null)
                                cnpjParceiro = dest.Element(ns + "CPF").Value;
                            else
                                cnpjParceiro = ""; // Estrangeiro
                        }
                        else
                        {
                            // É Compra Normal (Nacional): O Parceiro é o Emitente
                            cnpjParceiro = cnpjEmitenteXml;
                            nomeParceiro = emit?.Element(ns + "xNome")?.Value;
                        }

                        // --- 2. PREENCHER O PAINEL VISUAL (CABEÇALHO) ---
                        lblNomeEmit.Text = nomeParceiro;
                        lblNumNFe.Text = nNFe;
                        txtsNotaFiscal.Text = nNFe;

                        // Formatação simples de CNPJ para exibição
                        if (!string.IsNullOrEmpty(cnpjParceiro) && cnpjParceiro.Length == 14 && long.TryParse(cnpjParceiro, out long lCnpj))
                            lblCNPJEmit.Text = lCnpj.ToString(@"00\.000\.000\/0000\-00");
                        else if (!string.IsNullOrEmpty(cnpjParceiro))
                            lblCNPJEmit.Text = cnpjParceiro;
                        else
                            lblCNPJEmit.Text = "Estrangeiro / Sem Documento";


                        // --- 3. TENTAR SELECIONAR O FORNECEDOR NO DROPDOWN ---
                        bool fornecedorEncontrado = false;
                        string idParceiroEncontrado = "0";

                        // Limpa seleção anterior
                        ddlFornecedor.SelectedIndex = -1;

                        if (!string.IsNullOrEmpty(cnpjParceiro))
                        {
                            // Tenta buscar o ID pelo CNPJ no banco
                            idParceiroEncontrado = BuscarIdFornecedorPorCNPJ(cnpjParceiro);
                        }

                        // Se achou um ID válido, seleciona no combo
                        if (idParceiroEncontrado != "0" && ddlFornecedor.Items.FindByValue(idParceiroEncontrado) != null)
                        {
                            ddlFornecedor.SelectedValue = idParceiroEncontrado;
                            fornecedorEncontrado = true;
                        }

                        // Feedback visual se não encontrou
                        if (!fornecedorEncontrado)
                        {
                            string msg = "Parceiro não vinculado automaticamente pelo CNPJ.";
                            if (isImportacaoOuEmissaoPropria)
                                msg += " Nota de Importação detectada: Selecione o fornecedor estrangeiro manualmente no campo 'Empresa'.";

                            MensagemPagina.MostraMensagem_Aviso(msg);
                        }


                        // --- 4. LER OS ITENS E FAZER O AUTO-MATCH ---
                        List<ItemImportacaoXML> listaItensXML = new List<ItemImportacaoXML>();
                        var itens = xml.Descendants(ns + "det");

                        foreach (var item in itens)
                        {
                            var prod = item.Element(ns + "prod");

                            ItemImportacaoXML obj = new ItemImportacaoXML();
                            obj.cProd = prod.Element(ns + "cProd")?.Value;
                            obj.xProd = prod.Element(ns + "xProd")?.Value;
                            obj.NCM = prod.Element(ns + "NCM")?.Value;
                            obj.uCom = prod.Element(ns + "uCom")?.Value;

                            // Tratamento seguro para números (Substituindo ponto por virgula conforme seu padrão, ou usando CultureInfo)
                            string qComStr = prod.Element(ns + "qCom")?.Value ?? "0";
                            string vUnComStr = prod.Element(ns + "vUnCom")?.Value ?? "0";
                            string vProdStr = prod.Element(ns + "vProd")?.Value ?? "0";

                            // Usa replace se seu servidor estiver em PT-BR, ou InvariantCulture se vier com ponto do XML
                            obj.qCom = decimal.Parse(qComStr.Replace(".", ","));
                            obj.vUnCom = decimal.Parse(vUnComStr.Replace(".", ","));
                            obj.vProd = decimal.Parse(vProdStr.Replace(".", ","));


                            if (fornecedorEncontrado)
                            {
                                var dadosVinculo = VerificarVinculoExistenteCompleto(ddlFornecedor.SelectedValue, obj.cProd);

                                obj.idProdutoSistema = dadosVinculo.Id;
                                obj.xProdSistema = dadosVinculo.Nome;
                            }
                            else
                            {
                                obj.idProdutoSistema = 0;
                                obj.xProdSistema = "";
                            }

                            listaItensXML.Add(obj);
                        }

                        // 5. BIND NA GRID
                        dtgVinculoXML.DataSource = listaItensXML;
                        dtgVinculoXML.DataBind();

                        // 6. ATIVAR AUTOCOMPLETE E MANTER ARQUIVO
                        // Chama o script de autocomplete após o bind para ativar nos textboxes
                        RegistrarScriptAutocompleteGrid();

                        // Guarda o XML na sessão para salvar no final do processo
                        Session["ArquivoXML_Bytes"] = fupArquivoXML.FileBytes;
                        Session["ArquivoXML_Nome"] = fupArquivoXML.FileName;

                        Div_painel_parceiro_xml.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao processar XML: " + ex.Message);
                    Div_painel_parceiro_xml.Visible = false;
                }
            }
        }

        protected void dtgVinculoXML_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Panel pnlBusca = (Panel)e.Row.FindControl("pnlBuscaProduto");
                Panel pnlVinculado = (Panel)e.Row.FindControl("pnlProdutoVinculado");
                Label lblVinculado = (Label)e.Row.FindControl("lblProdutoVinculado");
                HiddenField hddId = (HiddenField)e.Row.FindControl("hddIdProdutoVinculado");
                HiddenField hddDesc = (HiddenField)e.Row.FindControl("hddDescProdutoVinculado");
                TextBox txtBusca = (TextBox)e.Row.FindControl("txtProdutoNfe");

                // Lógica de Exibição
                if (hddId.Value != "0" && !string.IsNullOrEmpty(hddId.Value))
                {
                    // TEM VÍNCULO: Mostra Label, Esconde Busca
                    pnlBusca.Visible = false;
                    pnlVinculado.Visible = true;
                    lblVinculado.Text = hddDesc.Value; // "Cód - Descrição"
                    e.Row.CssClass = "success"; // Linha Verde
                }
                else
                {
                    // NÃO TEM VÍNCULO: Mostra Busca, Esconde Label
                    pnlBusca.Visible = true;
                    pnlVinculado.Visible = false;
                    // O Autocomplete (JS) vai atuar sobre o txtProdutoNfe aqui
                }
            }
        }

        private void SalvarImportacaoXML()
        {
            // 1. VALIDAÇÃO PRÉVIA
            // Verifica se todos os itens têm um produto vinculado
            foreach (GridViewRow row in dtgVinculoXML.Rows)
            {
                HiddenField hddId = (HiddenField)row.FindControl("hddIdProdutoVinculado");

                // Se o ID for 0 ou vazio, o usuário não vinculou
                if (string.IsNullOrEmpty(hddId.Value) || hddId.Value == "0")
                {
                    // Pinta a linha de erro e avisa
                    row.CssClass = "danger";
                    MensagemPagina.MostraMensagem_Erro("Existem itens sem vínculo (linhas vermelhas). Vincule-os a um produto interno antes de salvar.");
                    return;
                }
            }

            // 2. CRIAR O CABEÇALHO DA MOVIMENTAÇÃO
            string idMovimentacaoGerada = "0";
            string sErro = "";

            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("sFuncao", "MOVIMENTAR_ESTOQUE");
                vParametros.Add("@idMovimentacao", "0");
                vParametros.Add("@idTipoMovimentacao", "2");
                vParametros.Add("@idMotivo", ddlMotivo.SelectedValue);
                vParametros.Add("@sNotaFiscal", lblNumNFe.Text); // Pega do label do XML

                vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario().ToString());
                vParametros.Add("@sObservacao", "Importação via XML - " + DateTime.Now.ToString("dd/MM/yyyy"));
                vParametros.Add("@idParceiro", ddlFornecedor.SelectedValue);
                vParametros.Add("@sPO", "Importação XML");
                vParametros.Add("@sEfetivaEstoque", "N"); // Salva como Pendente primeiro
                //vParametros.Add("@idImpressora", "0");
                vParametros.Add("@idStatus", "6");
                vParametros.Add("@sDscTipoTU", "E");

                DataSet dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);

                if (BD.ValidarDataSet(dsSalvar, out sErro))
                {
                    idMovimentacaoGerada = RETORNO.DATASET(dsSalvar, 0, "idMovimentacao");
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro(sErro);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao criar cabeçalho da movimentação: " + ex.Message);
                return;
            }

            // 3. PROCESSAR ITENS (VINCULAR + SALVAR ITEM)
            try
            {
                foreach (GridViewRow row in dtgVinculoXML.Rows)
                {
                    // Dados da Grid
                    HiddenField hddId = (HiddenField)row.FindControl("hddIdProdutoVinculado");
                    string idProdutoInterno = hddId.Value;
                    string codigoFornecedor = dtgVinculoXML.DataKeys[row.RowIndex]["cProd"].ToString();

                    // Dados Visuais (Labels) - Convertendo de volta para decimal
                    string sQtd = ((Label)row.FindControl("lblQtdXML")).Text;
                    string sVlrUnit = ((Label)row.FindControl("lblVlrXML")).Text;
                    string sDescricaoNFe = row.Cells[1].Text;

                    string descricaoFornecedor = row.Cells[2].Text;

                    // --- PASSO A (IMPORTANTE): APRENDER O VÍNCULO ---
                    // Grava na tabela tbl_Flow_WMS_Produtos_Fornecedores
                    Dictionary<string, string> vParamVinculo = new Dictionary<string, string>();
                    vParamVinculo.Add("@sFuncao", "VINCULAR-SALVAR-MOVIMENTACAO");
                    vParamVinculo.Add("@idParceiro", ddlFornecedor.SelectedValue);
                    vParamVinculo.Add("@sCodigoFornecedor", codigoFornecedor);
                    vParamVinculo.Add("@idProduto", idProdutoInterno);
                    vParamVinculo.Add("@sDscFornecedorProduto", descricaoFornecedor); // Opcional, bom para histórico

                    BD.ExecutarDataSet("sp_Manipula_tbl_Flow_WMS_Produtos_Fornecedores", vParamVinculo);

                    // --- PASSO B: INSERIR O ITEM NA MOVIMENTAÇÃO ---

                    Dictionary<string, string> vParamItem = new Dictionary<string, string>();
                    vParamItem.Add("@sFuncao", "MOVIMENTAR_ESTOQUE_ITENS");
                    vParamItem.Add("@idMovimentacao", idMovimentacaoGerada);
                    vParamItem.Add("@idProduto", idProdutoInterno);

                    // Valores precisam ir no formato SQL (Ponto) ou passados como parametros limpos
                    vParamItem.Add("@nQtdMovimentacao", sQtd.Replace(".", "").Replace(",", "."));
                    vParamItem.Add("@nValorUnitario", sVlrUnit.Replace(".", "").Replace(",", "."));

                    vParamItem.Add("@idUsuario", IDENTITY.Variaveis.idUsuario().ToString());
                    vParamItem.Add("@sEfetivaEstoque", "N");
                    vParamItem.Add("@idLocalArmazenamento", "0");
                    vParamItem.Add("@idPosicao", "0");
                    vParamItem.Add("@sLote", "");
                    vParamItem.Add("@nSerie", "");
                    //vParamItem.Add("@sDscTipo", "E"); 

                    BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParamItem);
                }

                // -----------------------------------------------------------------------
                // 4. SALVAR O ARQUIVO XML NA TABELA DE NFe (METADADOS + ARQUIVO)
                // -----------------------------------------------------------------------
                if (Session["ArquivoXML_Bytes"] != null)
                {
                    try
                    {
                        byte[] arquivoBytes = (byte[])Session["ArquivoXML_Bytes"];

                        // Recarrega o XML da memória para extrair metadados para a procedure complexa
                        using (MemoryStream ms = new MemoryStream(arquivoBytes))
                        {
                            XDocument xml = XDocument.Load(ms);
                            XNamespace ns = "http://www.portalfiscal.inf.br/nfe";

                            var ide = xml.Descendants(ns + "ide").FirstOrDefault();
                            var emit = xml.Descendants(ns + "emit").FirstOrDefault();
                            var dest = xml.Descendants(ns + "dest").FirstOrDefault();
                            var infNFe = xml.Descendants(ns + "infNFe").FirstOrDefault();

                            // Extração dos dados para a tabela tbl_Flow_XML_NFe
                            string sChave = infNFe?.Attribute("Id")?.Value.Replace("NFe", "") ?? "";
                            string sCNPJ_Emit = emit?.Element(ns + "CNPJ")?.Value ?? "";
                            string sNome_Emit = emit?.Element(ns + "xNome")?.Value ?? "";
                            string sNome_Dest = dest?.Element(ns + "xNome")?.Value ?? "";

                            // Tratamento numérico seguro
                            string sNNF = ide?.Element(ns + "nNF")?.Value ?? "0";
                            string sSerie = ide?.Element(ns + "serie")?.Value ?? "0";

                            decimal nNF = 0;
                            decimal nSerie = 0;
                            decimal.TryParse(sNNF, out nNF);
                            decimal.TryParse(sSerie, out nSerie);

                            // Configura o SqlDataAdapter para chamar a proc correta
                            SqlDataAdapter da = new SqlDataAdapter("sp_Manipula_tbl_Flow_XML_NFe", TT.FrameWork.BD.StringDeConexao);
                            DataSet tabela = new DataSet();
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;

                            da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "INCLUIR_XML";

                            // Parâmetros extraídos do XML
                            da.SelectCommand.Parameters.Add("@sChaveNFe", SqlDbType.VarChar).Value = sChave;
                            da.SelectCommand.Parameters.Add("@sCNPJ_Emitente", SqlDbType.VarChar).Value = sCNPJ_Emit;
                            da.SelectCommand.Parameters.Add("@nNumeroNF", SqlDbType.Decimal).Value = nNF;
                            da.SelectCommand.Parameters.Add("@nSerieNF", SqlDbType.Decimal).Value = nSerie;
                            da.SelectCommand.Parameters.Add("@sNome_Emitente", SqlDbType.VarChar).Value = sNome_Emit;
                            da.SelectCommand.Parameters.Add("@sNome_Destinatario", SqlDbType.VarChar).Value = sNome_Dest;
                            da.SelectCommand.Parameters.Add("@sXML", SqlDbType.NVarChar).Value = xml.ToString(); // Conteúdo Texto

                            // Dados de Controle do Sistema
                            da.SelectCommand.Parameters.Add("@idOrigem", SqlDbType.Int).Value = 51;
                            da.SelectCommand.Parameters.Add("@idTipoObjeto", SqlDbType.Int).Value = 1; // Ajuste conforme sua regra
                            da.SelectCommand.Parameters.Add("@idObjeto", SqlDbType.Int).Value = Convert.ToInt32(idMovimentacaoGerada); // Vínculo com a Movimentação
                            da.SelectCommand.Parameters.Add("@idEmpresa", SqlDbType.Int).Value = 0; // Deixa a proc resolver

                            da.Fill(tabela);
                        }
                    }
                    catch (Exception exXML)
                    {
                        // Logamos o erro mas não paramos o fluxo, pois a movimentação principal já foi criada
                        System.Diagnostics.Debug.WriteLine("Erro ao salvar log XML: " + exXML.Message);
                    }
                }

                // SUCESSO
                MensagemPagina.MostraMensagem_Sucesso("Importação realizada com sucesso!");

                // Redireciona para a tela de detalhes normal, onde o usuário verá os itens carregados e poderá efetivar
                Response.Redirect("Movimentacao_Detalhe.aspx?id=" + idMovimentacaoGerada);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao salvar itens: " + ex.Message);
            }
        }

        private (int Id, string Nome) VerificarVinculoExistenteCompleto(string idFornecedor, string cProdXML)
        {
            if (string.IsNullOrEmpty(idFornecedor) || idFornecedor == "0" || string.IsNullOrEmpty(cProdXML))
                return (0, "");

            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR_VINCULO_INCLUIR"); // A procedure deve fazer um JOIN para trazer o nome do produto
                vParametros.Add("@idParceiro", idFornecedor);
                vParametros.Add("@sCodigoFornecedor", cProdXML);

                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_WMS_Produtos_Fornecedores", vParametros);

                string sErro = "";
                if (BD.ValidarDataSet(ds, out sErro))
                {
                    int id = Convert.ToInt32(RETORNO.DATASET(ds, 0, "idProduto"));
                    string nome = RETORNO.DATASET(ds, 0, "sDscProduto"); // Assumindo que a proc retorna essa coluna
                    return (id, nome);
                }
            }
            catch { }

            return (0, "");
        }

        protected void lnkRemoverVinculo_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;

            Panel pnlBusca = (Panel)row.FindControl("pnlBuscaProduto");
            Panel pnlVinculado = (Panel)row.FindControl("pnlProdutoVinculado");
            HiddenField hddId = (HiddenField)row.FindControl("hddIdProdutoVinculado");
            TextBox txtBusca = (TextBox)row.FindControl("txtProdutoNfe");

            // Reseta para o estado de busca
            hddId.Value = "0";
            pnlBusca.Visible = true;
            pnlVinculado.Visible = false;
            txtBusca.Text = "";
            txtBusca.Focus();

            // Re-registra o script para o autocomplete funcionar no campo que acabou de aparecer
            RegistrarScriptAutocompleteGrid();
        }

        [WebMethod]
        public static List<string> GetProdutosAutocomplete(string term)
        {
            List<string> resultado = new List<string>();

            if (string.IsNullOrEmpty(term) || term.Length < 3)
                return resultado;

            try
            {
                // Sua query original adaptada para SQL Parametrizado ou string segura
                // Retorno esperado: DESCRIÇÃO | ID | CODIGO | UNIDADE
                string sql = "sp_Select 'FLOW_Produtos', 0, 'S', '" + term.Replace("'", "") + "'";

                using (SqlDataReader sdr = BD.ExecutarDataReader(sql))
                {
                    while (sdr.Read())
                    {
                        // Formato: NOME - CODIGO | ID
                        string itemFormatado = string.Format("{0} - {1}|{2}",
                            sdr["sCodigo"] + " - " + sdr["sDscProduto"], // Nome
                            sdr["sCodigo"],     // Codigo Visual
                            sdr["idItem"]       // ID Real (Value)
                        );
                        resultado.Add(itemFormatado);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log de erro se necessário
            }

            return resultado;
        }

        [WebMethod]
        public static string[] GetProdutos(string term, string idTipoProduto, string idFamilia, string idGrupo, string idPaisOrigem, string FTidParceiro)
        {
            List<string> resultado = new List<string>();

            if (string.IsNullOrEmpty(term) || term.Length < 3)
                return resultado.ToArray();

            try
            {
                string sql = "sp_Select 'FLOW_Produtos', 0, 'S', '" + term.Replace("'", "") + "'";

                using (SqlDataReader sdr = BD.ExecutarDataReader(sql))
                {
                    while (sdr.Read())
                    {
                        // Formato idêntico ao que funcionava: CODIGO - DESCRIÇÃO | ID
                        string itemFormatado = string.Format("{0}|{1}|{2}|{3}|{4}",
      sdr["sDscProduto"].ToString(),   // parts[0] -> Vai pro sDsc
      sdr["sCodigo"].ToString(),       // parts[1] -> Vai pro sCodigo
      sdr["idItem"].ToString(),        // parts[2] -> Vai pro hddComposicao_ID
      sdr["sUnidade"].ToString(),      // parts[3] -> Vai pro hddsUnidade
      sdr["sDscTipoProduto"].ToString()// parts[4] -> Vai pro hddsTipo
  );

                        resultado.Add(itemFormatado);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log de erro
            }

            return resultado.ToArray();
        }

        private void RegistrarScriptAutocompleteGrid()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("var $v192 = window.jQuery;");
            sb.AppendLine("$v192(document).ready(function() {");
            sb.AppendLine("  $v192('.css-autocomplete').autocomplete({");
            sb.AppendLine("    minLength: 3,");
            sb.AppendLine("    source: function(request, response) {");
            sb.AppendLine("      $v192.ajax({");
            sb.AppendLine("        url: 'Movimentacao_Detalhe.aspx/GetProdutosAutocomplete',");
            sb.AppendLine("        data: JSON.stringify({ 'term': request.term }),");
            sb.AppendLine("        dataType: 'json',");
            sb.AppendLine("        type: 'POST',");
            sb.AppendLine("        contentType: 'application/json; charset=utf-8',");
            sb.AppendLine("        success: function(data) {");
            sb.AppendLine("          if(!data.d) { console.log('Nenhum dado retornado'); return; }");
            sb.AppendLine("          response($v192.map(data.d, function(item) {");
            sb.AppendLine("            var parts = item.split('|');");
            sb.AppendLine("            return {");
            sb.AppendLine("              label: parts[0],");
            sb.AppendLine("              value: parts[0],");
            sb.AppendLine("              id: parts[1]");
            sb.AppendLine("            };");
            sb.AppendLine("          }));");
            sb.AppendLine("        },");
            sb.AppendLine("        error: function(xhr, status, error) {");
            sb.AppendLine("          console.log('Erro Ajax:', xhr.responseText);");
            sb.AppendLine("        }");
            sb.AppendLine("      });");
            sb.AppendLine("    },");
            sb.AppendLine("    select: function(event, ui) {");
            sb.AppendLine("      console.log('Item selecionado:', ui.item);");
            sb.AppendLine("      var td = $v192(this).closest('td');");
            sb.AppendLine("      var hidden = td.find('.css-hdd-id');");
            sb.AppendLine("      if (hidden.length === 0) { hidden = td.find('input[type=hidden]'); }");
            sb.AppendLine("      if (hidden.length > 0) {");
            sb.AppendLine("         hidden.val(ui.item.id);");
            sb.AppendLine("         console.log('ID setado no hidden:', ui.item.id);");
            sb.AppendLine("      } else {");
            sb.AppendLine("         console.log('Hidden field não encontrado na linha');");
            sb.AppendLine("      }");
            sb.AppendLine("    }");
            sb.AppendLine("  });");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(Page, GetType(), "js_Autocomplete", sb.ToString(), true);
        }

        #endregion

        #region | Eventos

        protected void ddlFornecedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "COMPLETAR_PARCEIROS" },
                { "@idParceiro", ddlFornecedor.SelectedValue }
            };
            DataSet dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_CotacaoCompras", vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out string sErro))
            {
                Pesquisa_Parceiros.SCnpj_CPF = RETORNO.DATASET(dsPesquisa, "sCPF_CNPJ");
                Pesquisa_Parceiros.idParceiro = Convert.ToInt32(RETORNO.DATASET(dsPesquisa, "idCliente"));
                Pesquisa_Parceiros.SRazaoSocial = RETORNO.DATASET(dsPesquisa, "sRazaoSocial");
            }

            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();

            if (string.Equals(sRequestAcao, "ImportarXML", StringComparison.OrdinalIgnoreCase))
            {
                if (ddlFornecedor.SelectedValue == "0" && dtgVinculoXML.Rows.Count > 0)
                    Div_painel_parceiro_xml.Visible = true;
                else Div_painel_parceiro_xml.Visible = false;
            }
        }

        void CompletarCamposDadosPO(string idPedido)
        {
            ddlFornecedor.SelectedValue = "0";

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "COMPLETAR_DADOS_PEDIDOS" },
                { "@idPedido", idPedido},
                {"@idTipoMovimentacao", ddlsTipoMovimentacao.SelectedValue }
            };
            DataSet dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out string sErro))
            {
                //txtsNotaFiscal.Text = RETORNO.DATASET(dsPesquisa, "sReferencia");
                txtsDscObservacao.Text = RETORNO.DATASET(dsPesquisa, "sObservacao");

                if (RETORNO.DATASET(dsPesquisa, "idParceiro") != "0" && !string.IsNullOrEmpty(RETORNO.DATASET(dsPesquisa, "idParceiro")))
                {
                    if (ddlFornecedor.Items.FindByValue(RETORNO.DATASET(dsPesquisa, "idParceiro")) == null)
                        MensagemPagina.MostraMensagem_Erro("<b>Erro</b> O Pedido de Compras Consta com fornecedor incorreto, não será possível Seguir a Movimentação.", false);
                    else ddlFornecedor.SelectedValue = RETORNO.DATASET(dsPesquisa, "idParceiro");
                }
                else MensagemPagina.MostraMensagem_Erro("<b>Erro</b> O Pedido de Compras não tem um fornecedor, não será possível Seguir a Movimentação.", false);

                if (chkAlternar.Checked)
                {
                    bs_Movimentacao = conversorMovimentacao.ConverterDataSet(dsPesquisa, "table1");
                    dtgItens_DataBind();
                }
                else
                {
                    bs_ItemSaida = conversorMovimentacao.ConverterDataSet(dsPesquisa, "table1");
                    dtgItensSaida_DataBind();
                }
            }
            else MensagemPagina.MostraMensagem_Erro("<b>Erro: </b>Nenhum Pedido Encontrado ou Liberado com esse código para importar!");

            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
        }

        protected void ddlsPO_SelectedIndexChanged(object sender, EventArgs e)
        {
            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
            txtsPedidoCompra.Text = ddlsPO.SelectedItem.Text;
            CompletarCamposDadosPO(ddlsPO.SelectedValue);
        }

        #endregion

        #region | Scripts

        void RegistrarScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("$('[id*=FT_txtnValor]').mask('0.000.000.009,9999', { reverse: true });");

            sb.Append("if (typeof window.$v192 === 'undefined') { window.$v192 = window.jQuery || window.$; } ");
            // Função para inicializar as modais
            sb.Append("$v192(function() {");

            // Configuração da modal para Salvar Principal
            sb.Append("$v192(\"#dialog-SalvarPrincipal\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons: {");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_SALVAR\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("}");
            sb.Append("}");
            sb.Append("});");

            // Configuração da modal para Editar Principal (Efetivar)
            sb.Append("$v192(\"#dialog-EditarPrincipal\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons: {");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_Editar\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("}");
            sb.Append("}");
            sb.Append("});");

            // Eventos de clique para abrir as modais com textos personalizados
            sb.Append("$v192('[id*=cmdSalvar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-SalvarPrincipal').dialog('option', 'title', 'Confirmação de Salvamento');"); // Personaliza o título
            sb.Append("$v192('#lblTituloSalvar').text('Tem certeza que deseja salvar?');"); // Personaliza o texto
            sb.Append("$v192('#dialog-SalvarPrincipal').dialog('open');");
            sb.Append("});");

            sb.Append("$v192('[id*=cmdEFetivar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-EditarPrincipal').dialog('option', 'title', 'Confirmação de Efetivação');"); // Personaliza o título
            sb.Append("$v192('#lblTituloSalvar').text('Tem certeza que deseja efetivar esta ação?');"); // Personaliza o texto
            sb.Append("$v192('#dialog-EditarPrincipal').dialog('open');");
            sb.Append("});");

            sb.Append("});");

            ScriptManager.RegisterStartupScript(Page, GetType(), "js_ScriptPagina", sb.ToString(), true);
        }

        void RegistrarScriptPagina()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"$('#FiltroPesquisacphCorpo_FiltroPesquisa_FT_txtnValor').mask('000.000.000.000.000,00', {{ reverse: true }});");

            // INÍCIO DA CORREÇÃO: Usa window.jQuery para não "matar" o $ do FiltroPesquisa
            sb.AppendLine("var $j = window.jQuery;");
            sb.AppendLine("(function ($j) {");
            sb.AppendLine("$(document).ready(function () {");

            sb.AppendLine("function AtualizaTotalGeral() {");
            sb.AppendLine("    var nTotalGeral = 0;");
            sb.AppendLine("    $j('._nValorTotal').each(function () {");
            sb.AppendLine("        var valor = parseFloat($j(this).text().replace(/\\./g, '').replace(',', '.'));");
            sb.AppendLine("        if (!isNaN(valor))");
            sb.AppendLine("            nTotalGeral += valor;");
            sb.AppendLine("    });");

            sb.AppendLine("    $j('._nTotalGeral').text(nTotalGeral.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));");
            sb.AppendLine("}");

            sb.AppendLine("function AtualizaTotalQuantidade() {");
            sb.AppendLine("    var nTotalQuantidade = 0;");
            sb.AppendLine("    $j('[id*=Itens_txtnQuantidade]').each(function () {");
            sb.AppendLine("        var valor = $j(this).val();");
            sb.AppendLine("        if (valor != null && valor.trim() != '') {");
            sb.AppendLine("            valor = parseFloat(valor.replace(/\\./g, '').replace(',', '.'));");
            sb.AppendLine("            if (!isNaN(valor))");
            sb.AppendLine("                nTotalQuantidade += valor;");
            sb.AppendLine("        }");
            sb.AppendLine("    });");
            sb.AppendLine("    $j('._nTotalQuantidade').text(nTotalQuantidade.toLocaleString('pt-BR', { minimumFractionDigits: 4, maximumFractionDigits: 4 }));");
            sb.AppendLine("}");

            sb.AppendLine("if ($j('[id*=Itens_txtnQuantidade]').length > 0) {");
            sb.AppendLine("$j('[id*=Itens_txtnQuantidade], [id*=Itens_txtnValorUnitario], [id*=Itens_txtnPercIPI]' ).on('input', function () {");
            sb.AppendLine("var $row = $j(this).closest('tr');");
            sb.AppendLine("var nValorUnitario = parseFloat($row.find('[id*=Itens_txtnValorUnitario]').val().replace(/\\./g, '').replace(',', '.'));");
            sb.AppendLine("var nQuantidade = parseFloat($row.find('[id*=Itens_txtnQuantidade]').val().replace(',', '.'));");
            sb.AppendLine("var nPercIPI = parseFloat($row.find('[id*=Itens_txtnPercIPI]').val().replace(',', '.'));");
            sb.AppendLine("var nValorTotal = isNaN(nValorUnitario) || isNaN(nQuantidade) || isNaN(nPercIPI)  ? 0 : (nValorUnitario * (1+(nPercIPI/100))) * nQuantidade;");
            sb.AppendLine("$row.find('._nValorTotal').text(nValorTotal.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));");

            sb.AppendLine("        AtualizaTotalGeral();");
            sb.AppendLine("        AtualizaTotalQuantidade();");
            sb.AppendLine("});");

            sb.AppendLine("    AtualizaTotalGeral();");
            sb.AppendLine("     AtualizaTotalQuantidade();");
            sb.AppendLine("}");
            sb.AppendLine("});");
            sb.AppendLine("})($j);");

            string script = sb.ToString();
            RegistrarScript();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Calculo", script, true);
        }

        #endregion

        #region | DtgItens e Status

        protected void AtualizarItens()
        {
            foreach (GridViewRow row in dtgItens.Rows)
            {
                decimal.TryParse((row.FindControl("Itens_txtnQuantidade") as TextBox)?.Text, out decimal nQuantidade);
                decimal.TryParse((row.FindControl("Itens_txtnValorUnitario") as TextBox)?.Text, out decimal nValorUnitario);
                decimal.TryParse((row.FindControl("Itens_txtnPercIPI") as TextBox)?.Text, out decimal nPercIPI);

                var item = bs_Movimentacao.FirstOrDefault(m => m.IdProduto.ToString() == row.Cells[0].Text);
                if (item != null)
                {
                    item.NQuantidade = nQuantidade;
                    item.nValorUnitario = nValorUnitario;
                    item.nPercIPI = nPercIPI;
                    item.nValorTotal = Math.Round(nQuantidade * (nValorUnitario * (1 + (nPercIPI / 100))), 2);
                }
            }
        }

        protected void dtgItens_DataBind()
        {
            try
            {
                AtualizarItens();
                foreach (cls_WMS_MovimentacaoRel tb in bs_Movimentacao)
                {
                    tb.SCodigoComDescricao = tb.SCodigo + " - " + tb.SDscProduto;
                }

                dtgItens.DataSource = bs_Movimentacao;
                dtgItens.DataBind();

               // Grid.SomarColunas(dtgItens, true, Grid.Formatação.Moeda, 8);
            }
            catch (Exception ex)
            {
                MensagemPaginaItens.MostraMensagem_Erro("ERRO: " + ex.ToString());
            }
        }

        protected void dtgItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (ddlMotivo.SelectedValue == "1")
            {
                FUNCOES.ReexibirColunas(dtgItens, "Valor");
                FUNCOES.ReexibirColunas(dtgItens, "Total");
            }
            else if (ddlMotivo.SelectedValue == "3")
            {
                FUNCOES.ReexibirColunas(dtgItens, "Valor");
                FUNCOES.ReexibirColunas(dtgItens, "Total");
            }
            else
            {
                FUNCOES.EsconderColunas(dtgItens, "Valor");
                FUNCOES.EsconderColunas(dtgItens, "Total");
            }

            FUNCOES.EsconderColunas(dtgItens, "Descrição do Produto");

            string sStatus = "disabled";
            if (Request["id"] != "0")
            {
                sStatus = "disabled";
                FUNCOES.EsconderColunas(dtgItens, "Excluir");
            }
            else sStatus = "enabled";

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton lnkExcluir = e.Row.FindControl("lnkExcluir") as LinkButton;
                TextBox Itens_txtnQuantidade = e.Row.FindControl("Itens_txtnQuantidade") as TextBox;
                TextBox Itens_txtnValorUnitario = e.Row.FindControl("Itens_txtnValorUnitario") as TextBox;
                TextBox Itens_txtnPercIPI = e.Row.FindControl("Itens_txtnPercIPI") as TextBox;
                TextBox txtGarantia = e.Row.FindControl("txtsGarantia") as TextBox;

                if (hddidStatus.Value != "7")
                {
                    var itemAtual = (cls_WMS_MovimentacaoRel)e.Row.DataItem;
                    string serializa = itemAtual != null && !string.IsNullOrEmpty(itemAtual.SSerializavel) ? itemAtual.SSerializavel : "S";



                    if (chkAlternar.Checked && (hddidMovimentacao.Value == "0" || string.IsNullOrEmpty(hddidMovimentacao.Value)))
                    {
                        Itens_txtnValorUnitario.Attributes.Remove("disabled");
                        Itens_txtnPercIPI.Attributes.Remove("disabled");
                        Itens_txtnQuantidade.Attributes.Remove("disabled");
                    }
                    else
                    {
                        Itens_txtnValorUnitario.Attributes.Remove("disabled");
                        Itens_txtnValorUnitario.Attributes.Add(sStatus, sStatus);
                        Itens_txtnPercIPI.Attributes.Remove("disabled");
                        Itens_txtnPercIPI.Attributes.Add(sStatus, sStatus);
                        Itens_txtnQuantidade.Attributes.Remove("disabled");
                        Itens_txtnQuantidade.Attributes.Add(sStatus, sStatus);
                    }

                    TextBox txtidProduto = e.Row.FindControl("txtIdProduto") as TextBox;
                    LinkButton cmdAdicionarSerie = e.Row.FindControl("cmdAdicionarSerie") as LinkButton;
                    LinkButton cmdVisualizarSerie = e.Row.FindControl("cmdVisualizarSerie") as LinkButton;
                    
                    TextBox txtsTipoMov = e.Row.FindControl("txtsTipoMov") as TextBox;

                    bool isConsultaDetalhe = Request["id"] != null && Request["id"] != "0";
                    bool liberaSerie = hddsLiberaSerie.Value == "S";

                    if (cmdAdicionarSerie != null)
                    {
                        cmdAdicionarSerie.Visible = isConsultaDetalhe && liberaSerie && serializa == "S";
                        lnkExcluir.Visible = !isConsultaDetalhe;
                    }

                    if (lnkExcluir != null)
                    {
                        if (!string.IsNullOrEmpty(hddidMovimentacao.Value) && hddidMovimentacao.Value != "0")
                        {
                            lnkExcluir.Attributes.Add("disabled", "disabled");
                            lnkExcluir.Visible = false;
                        }
                        else
                        {
                            if (chkAlternar.Checked) lnkExcluir.Visible = false;
                            if (!cmdAdicionarSerie.Visible && !chkAlternar.Checked) lnkExcluir.Visible = true;

                            lnkExcluir.Attributes.Remove("disabled");
                        }
                    }

                    if (txtidProduto != null && !string.IsNullOrEmpty(txtidProduto.Text) && txtidProduto.Text != "0")
                    {
                        if (serializa == "N")
                        {
                            if (cmdAdicionarSerie != null) cmdAdicionarSerie.Visible = false;
                            if (cmdVisualizarSerie != null) cmdVisualizarSerie.Visible = false;
                        }
                        else
                        {
                            Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONSULTAR-ETIQUETAS-VALIDACAO" },
                            { "@idMovimentacao", txtidMovimentacao.Text },
                            { "@idProduto", txtidProduto.Text }
                        };
                            DataSet dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);

                            int qtdValidadas = 0, totalEtiquetasGeradas = 0;

                            if (BD.ValidarDataSet(dsPesquisa))
                            {
                                totalEtiquetasGeradas = dsPesquisa.Tables[0].Rows.Count;

                                foreach (DataRow row in dsPesquisa.Tables[0].Rows)
                                {
                                    if (row["sConfirmado"].ToString() == "S")
                                        qtdValidadas++;
                                }
                            }

                            if (Itens_txtnQuantidade != null)
                            {
                                decimal.TryParse(Itens_txtnQuantidade.Text, out decimal quantidadeGrid);

                                if (cmdAdicionarSerie != null)
                                {
                                    if (isConsultaDetalhe)
                                    {
                                        if (totalEtiquetasGeradas > 0 && qtdValidadas == totalEtiquetasGeradas)
                                        {
                                            cmdAdicionarSerie.Visible = false;
                                            cmdVisualizarSerie.Visible = true;

                                            if (string.IsNullOrEmpty(hdddtEfetivado.Value))
                                                AtualizarStatus("5");
                                            else AtualizarStatus("4");
                                        }
                                        else
                                        {
                                            cmdAdicionarSerie.Visible = liberaSerie;
                                            cmdVisualizarSerie.Visible = false;

                                            if (string.IsNullOrEmpty(hdddtEfetivado.Value))
                                            {
                                                if (hddsLiberaImpressao.Value == "S")
                                                    AtualizarStatus("6");
                                                else AtualizarStatus("2");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (ddlMotivo.SelectedValue == "9")
                    {
                        lnkExcluir.Visible = false;

                        Itens_txtnValorUnitario.Attributes.Add("disabled", "disabled");
                        Itens_txtnPercIPI.Attributes.Add("disabled", "disabled");
                        Itens_txtnQuantidade.Attributes.Add("disabled", "disabled");
                    }
                }
                else 
                {
                    lnkExcluir.Visible = false;
                    Itens_txtnValorUnitario.Attributes.Add("disabled", "disabled");
                    Itens_txtnPercIPI.Attributes.Add("disabled", "disabled");
                    Itens_txtnQuantidade.Attributes.Add("disabled", "disabled");
                }


            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Font.Bold = true;
                e.Row.Font.Size = 13;
                e.Row.Cells[0].Text = "Total Geral";
                e.Row.Cells[0].ColumnSpan = 5;
                e.Row.Cells[0].Font.Bold = true;
                e.Row.Cells.RemoveAt(1);
                e.Row.Cells.RemoveAt(2);
                e.Row.Cells.RemoveAt(3);
                e.Row.Cells.RemoveAt(4);

                e.Row.Cells[4].Font.Bold = true;
                e.Row.Cells[4].Text = "0,00";
                e.Row.Cells[4].CssClass = "_nTotalGeral";
                e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Right;

                e.Row.Cells[1].Font.Bold = true;
                e.Row.Cells[1].Text = "0,0000";
                e.Row.Cells[1].CssClass = "_nTotalQuantidade";
                e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Right;  

            }
        }

        protected void dtgItens_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            bs_Movimentacao.RemoveAt(e.RowIndex);
            dtgItens_DataBind();
            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
        }

        protected void AtualizarStatus(string idStatus)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "ATUALIZAR-STATUS" },
                { "@idMovimentacao", txtidMovimentacao.Text },
                { "@idStatus", idStatus }
            };
            DataSet dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);
        }

        protected void ValidarStatus(string idStatus, bool isCmdSalvarSerie)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "VALIDAR-STATUS" },
                { "@idMovimentacao", txtidMovimentacao.Text },
                { "@idStatus", idStatus }
            };

            if (string.IsNullOrEmpty(hdddtEfetivado.Value))
            {
                DataSet dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);

                if (BD.ValidarDataSet(dsPesquisa))
                {
                    if (RETORNO.DATASET(dsPesquisa, "idStatusNovo") == "5")
                    {
                        if (Session["CameraAbertaValidacao"] != null && (bool)Session["CameraAbertaValidacao"])
                        {
                            LeitorQuaggaMovimentacao.DesligarCamVariante();
                            Session["CameraAbertaValidacao"] = false;
                        }

                        Session["MensagemSucesso"] = "Todas as etiquetas validadas! Movimentação pronta para Efetivação.";
                        Response.Redirect(urlPagina + $"?id={hddidMovimentacao.Value}");
                        return;
                    }
                }
            }

            // Seu fluxo antigo mantido 100% intacto para as demais validações
            if (hddidStatus.Value == "0" || string.IsNullOrEmpty(hddidStatus.Value) || isCmdSalvarSerie)
                Response.Redirect(urlPagina + $"?id={hddidMovimentacao.Value}");
        }

        #endregion

        #region | Salvamento e Configuração

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            if (divImportacaoXML.Visible)
                SalvarImportacaoXML();
            else SalvarDados("N");
        }

        protected void cmdExcluir_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hddidMovimentacao.Value) || hddidMovimentacao.Value == "0")
            {
                MensagemPaginaErro.MostraMensagem_Erro("Salve a movimentação antes de excluí-la.", true);
                return;
            }

            if (!string.IsNullOrEmpty(hdddtEfetivado.Value))
            {
                MensagemPaginaErro.MostraMensagem_Erro("Não é possível excluir uma movimentação já efetivada no estoque.", true);
                return;
            }

            try
            {
                AtualizarStatus("7");
                Session["MensagemSucesso"] = "Movimentação excluída com sucesso.";
                Response.Redirect(urlPagina + "?id=" + hddidMovimentacao.Value);
            }
            catch (Exception ex)
            {
                MensagemPaginaErro.MostraMensagem_Erro("Erro ao excluir a movimentação: " + ex.Message, true);
            }
        }
        void SalvarDados(string sEfetiva)
        {
            if (ValidarDados())
            {
                try
                {
                    string[] vidItem = hddidMovimentacao.Value.Split(',');
                    string idMovimentacao = vidItem[0].ToString();
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "sFuncao", "MOVIMENTAR_ESTOQUE" },
                        { "@idMovimentacao", hddidMovimentacao.Value },
                        { "@idTipoMovimentacao", ddlsTipoMovimentacao.SelectedValue },
                        { "@idMotivo", ddlMotivo.SelectedValue },
                        { "@sNotaFiscal", txtsNotaFiscal.Text },
                        { "@idUsuario", IDENTITY.Variaveis.idUsuario() },
                        { "@sObservacao", txtsDscObservacao.Text },
                        { "@idParceiro", ddlFornecedor.SelectedValue },
                        { "@sPO", txtsPedidoCompra.Text },
                        { "@sEfetivaEstoque", sEfetiva },
                        { "@idImpressora", ddlImpressora.SelectedValue }
                    };

                    if (idMovimentacao == "0" || string.IsNullOrEmpty(idMovimentacao))
                        vParametros.Add("@idStatus", "6");

                    if (bs_Movimentacao.Count > 0 && ddlsTipoMovimentacao.SelectedValue != "4")
                    {
                        DataSet dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);
                        if (BD.ValidarDataSet(dsSalvar, out string sErro))
                        {
                            idMovimentacao = RETORNO.DATASET(dsSalvar, "idMovimentacao");
                            if (sEfetiva == "S")
                                hddidStatus.Value = RETORNO.DATASET(dsSalvar, "idStatus");
                            if (Movimentacao_Itens_Salvar(idMovimentacao, sEfetiva))
                            {
                                string urlReabertuta = string.Format("/App/Paginas/WMS/Movimentacao_Detalhe.aspx?id={0}", idMovimentacao);
                                txtidMovimentacao.Text = idMovimentacao;
                                Session["MensagemSucesso"] = "Registro gravado com sucesso";
                                ConfigurarControles(false);
                                divItens.Visible = true;
                                divItensDestino.Visible = false;

                                if (sEfetiva == "S")
                                    AtualizarComex();

                                Response.Redirect(urlReabertuta);
                            }
                        }
                    }
                    else if (ddlsTipoMovimentacao.SelectedValue == "4" && bs_ItemEntrada.Count > 0 && bs_ItemSaida.Count > 0 && sEfetiva == "N")
                    {
                        DataSet dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);
                        if (BD.ValidarDataSet(dsSalvar, out string sErro))
                        {
                            idMovimentacao = RETORNO.DATASET(dsSalvar, "idMovimentacao");
                            if (sEfetiva == "S")
                                hddidStatus.Value = RETORNO.DATASET(dsSalvar, "idMovimentacao");
                            if (Movimentacao_Itens_Salvar(idMovimentacao, sEfetiva))
                            {
                                string urlReabertuta = string.Format("/App/Paginas/WMS/Movimentacao_Detalhe.aspx?id={0}", idMovimentacao);
                                txtidMovimentacao.Text = idMovimentacao;
                                Session["MensagemSucesso"] = "Registro gravado com sucesso";
                                ConfigurarControles(false);
                                divItens.Visible = true;
                                divItensDestino.Visible = false;
                                Response.Redirect(urlReabertuta);
                            }
                        }
                    }
                    else if (bs_Movimentacao.Count > 0 && sEfetiva == "S")
                    {
                        DataSet dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);
                        if (BD.ValidarDataSet(dsSalvar, out string sErro))
                        {
                            idMovimentacao = RETORNO.DATASET(dsSalvar, "idMovimentacao");
                            if (sEfetiva == "S")
                                hddidStatus.Value = RETORNO.DATASET(dsSalvar, "idMovimentacao");
                            if (Movimentacao_Itens_Salvar(idMovimentacao, sEfetiva))
                            {
                                string urlReabertuta = string.Format("/App/Paginas/WMS/Movimentacao_Detalhe.aspx?id={0}", idMovimentacao);
                                txtidMovimentacao.Text = idMovimentacao;
                                Session["MensagemSucesso"] = "Registro gravado com sucesso";
                                ConfigurarControles(false);
                                divItens.Visible = true;
                                divItensDestino.Visible = false;
                                Response.Redirect(urlReabertuta);
                            }
                        }
                    }
                    else if (ddlsTipoMovimentacao.SelectedValue == "5" && bs_ItemMudanca.Count > 0)
                    {
                        DataSet dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);
                        if (BD.ValidarDataSet(dsSalvar, out string sErro))
                        {
                            idMovimentacao = RETORNO.DATASET(dsSalvar, "idMovimentacao");
                            if (sEfetiva == "S") hddidStatus.Value = RETORNO.DATASET(dsSalvar, "idStatus");

                            if (Movimentacao_Itens_Salvar(idMovimentacao, sEfetiva))
                            {
                                string urlReabertuta = string.Format("/App/Paginas/WMS/Movimentacao_Detalhe.aspx?id={0}", idMovimentacao);
                                txtidMovimentacao.Text = idMovimentacao;
                                Session["MensagemSucesso"] = "Registro gravado com sucesso";
                                ConfigurarControles(false);
                                divItens.Visible = false;
                                divItensDestino.Visible = false;
                                divMudancaPosicao.Visible = true;
                                Response.Redirect(urlReabertuta);
                            }
                        }
                    }
                    else MensagemPaginaItens.MostraMensagem_Erro("É Necessário adicionar Itens");
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro($"Erro: {ex.ToString()}");
                }
            }
        }

        protected bool Movimentacao_Itens_Salvar(string idMovimentacao, string sEfetivaEstoque)
        {
            var sProcedureTransform = "sp_Manipula_tbl_Flow_Produtos_Movimentacao";
            try
            {
                if (ddlsTipoMovimentacao.SelectedValue != "4" && ddlsTipoMovimentacao.SelectedValue != "5")
                {
                    Movimentacao_SalvarItens(idMovimentacao, bs_Movimentacao, sEfetivaEstoque, ddlImpressora.SelectedValue);
                    return true;
                }
                else if (ddlsTipoMovimentacao.SelectedValue == "5")
                {
                    try
                    {
                        AtualizaMemoriaGridMudanca(); // <-- Garante a versão final

                        StringBuilder sbXmlMudanca = new StringBuilder();
                        sbXmlMudanca.Append("<root>");

                        foreach (var itemMemoria in bs_ItemMudanca)
                        {
                            string qtd = BD.Conversoes.Numerico(itemMemoria.NQuantidade);
                            string vlrUnitario = BD.Conversoes.Numerico(itemMemoria.nValorUnitario);

                            sbXmlMudanca.AppendFormat("<item idProduto=\"{0}\" qtd=\"{1}\" vlrUnit=\"{2}\" locOrigem=\"{3}\" posOrigem=\"{4}\" locDestino=\"{5}\" posDestino=\"{6}\" />",
                                itemMemoria.IdProduto,
                                qtd,
                                vlrUnitario,
                                itemMemoria.IdLocalArmazenamento,
                                itemMemoria.IdPosicao,
                                itemMemoria.IdLocalDestino,
                                itemMemoria.IdPosicaoDestino);
                        }
                        sbXmlMudanca.Append("</root>");

                        Dictionary<String, String> vParametros = new Dictionary<string, string>();
                        vParametros.Add("@sFuncao", "SALVAR_MUDANCA_POSICAO_LOTE");
                        vParametros.Add("@idMovimentacao", idMovimentacao);
                        vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario().ToString());
                        vParametros.Add("@sEfetivaEstoque", sEfetivaEstoque);
                        vParametros.Add("@sXMLItens", sbXmlMudanca.ToString());

                        BD.ExecutarDataSet(sProcedureTransform, vParametros);

                        return true;
                    }
                    catch (Exception ex)
                    {
                        MensagemPagina.MostraMensagem_Erro("Erro ao salvar mudança de posição: " + ex.Message);
                        return false;
                    }
                }
                else
                {
                    try
                    {

                        Dictionary<String, String> vParametros = new Dictionary<string, string>();
                        vParametros.Add("@sFuncao", "TRANSFORMAR_ESTOQUE_ITENS");
                        vParametros.Add("@idMovimentacao", idMovimentacao);
                        vParametros.Add("@idProduto", "");
                        vParametros.Add("@idLocalArmazenamento", "");
                        vParametros.Add("@idPosicao", "");
                        vParametros.Add("@nQtdMovimentacao", "");
                        vParametros.Add("@nValorUnitario", "");
                        vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());

                        if (sEfetivaEstoque == "S")
                        {
                            if (sEfetivaEstoque == "S")
                            {
                                bs_Movimentacao.ForEach(item =>
                                {

                                    DataSet dsItens;

                                    vParametros["@idProduto"] = item.IdProduto.ToString();
                                    vParametros["@nValorUnitario"] = BD.Conversoes.Numerico(item.nValorUnitario);
                                    vParametros["@nQtdMovimentacao"] = BD.Conversoes.Numerico(item.NQuantidade);
                                    vParametros["@tipoMov"] = item.STipoMov;
                                    vParametros["@sEfetivaEstoque"] = sEfetivaEstoque;
                                    vParametros["@idLocalArmazenamento"] = item.IdLocalArmazenamento.ToString();
                                    vParametros["@idPosicao"] = item.IdPosicao.ToString();
                                    vParametros["@sCodigoBarras"] = $"PR{item.IdProduto}";

                                    dsItens = BD.ExecutarDataSet(sProcedureTransform, vParametros);

                                });
                            }
                            return true;
                        }
                        else
                        {
                            bs_ItemEntrada.ForEach(item =>
                            {

                                DataSet dsItens;

                                vParametros["@idProduto"] = item.IdProduto.ToString();
                                vParametros["@nValorUnitario"] = removeCaracteres(item.nValorUnitario.ToString());
                                vParametros["@nQtdMovimentacao"] = removeCaracteres(item.NQuantidade.ToString());
                                vParametros["@sDscTipoTU"] = "E";
                                vParametros["@sEfetivaEstoque"] = sEfetivaEstoque;
                                vParametros["@idLocalArmazenamento"] = item.IdLocalArmazenamento.ToString();
                                vParametros["@idPosicao"] = item.IdPosicao.ToString();

                                dsItens = BD.ExecutarDataSet(sProcedureTransform, vParametros);

                            });

                            bs_ItemSaida.ForEach(item =>
                            {

                                DataSet dsItens;

                                vParametros["@idProduto"] = item.IdProduto.ToString();
                                vParametros["@nValorUnitario"] = removeCaracteres(item.nValorUnitario.ToString());
                                vParametros["@nQtdMovimentacao"] = removeCaracteres(item.NQuantidade.ToString());
                                vParametros["@sDscTipoTU"] = "S";
                                vParametros["@sEfetivaEstoque"] = sEfetivaEstoque;
                                vParametros["@idLocalArmazenamento"] = item.IdLocalArmazenamento.ToString();
                                vParametros["@idPosicao"] = item.IdPosicao.ToString();

                                dsItens = BD.ExecutarDataSet(sProcedureTransform, vParametros);

                            });

                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

            return false;
        }

    
        void ConfigurarControles(bool bAtivo)
        {

            string sStatus = "disabled";
            if (bAtivo)
            {
                sStatus = "enabled";
            }
            ddlMotivo.Attributes.Remove("disabled");
            ddlMotivo.Attributes.Add(sStatus, sStatus);
            ddlsTipoMovimentacao.Attributes.Remove("disabled");
            ddlsTipoMovimentacao.Attributes.Add(sStatus, sStatus);
            txtdtMovimentacao.ReadOnly = !bAtivo;
            txtsDscObservacao.ReadOnly = !bAtivo;
            txtsNotaFiscal.ReadOnly = !bAtivo;
            txtsPedidoCompra.ReadOnly = !bAtivo;
            divPedidoCompra.Visible = false;
            divPedidoCompra2.Visible = false;
        }

        #endregion

        #region | Validações e Eventos

        private bool ValidarDados()
        {
            string mensagensErro = string.Empty;

            if (ddlMotivo.SelectedValue == "1")
            {
                if (!chkAlternar.Checked)
                {
                    bool isImportacao = string.Equals(sRequestAcao, "ImportarXML", StringComparison.OrdinalIgnoreCase);

                    if (string.IsNullOrEmpty(txtsPedidoCompra.Text) && !isImportacao)
                    {
                        mensagensErro += "Informe um Pedido de Compra!<br>";
                    }
                }
                else
                {
                    if (ddlsPO.SelectedValue == "0")
                    {
                        mensagensErro += "Selecione um Pedido de Compra!<br>";
                        ddlsPO.Focus();
                    }
                }

                if (chkAlternar.Checked)
                {
                    if (ddlFornecedor.SelectedValue == "0")
                    {
                        mensagensErro += "Informe um fornecedor!<br>";
                        ddlFornecedor.Focus();
                    }
                }
            }

            if (string.IsNullOrEmpty(txtsDscObservacao.Text))
            {
                mensagensErro += "Informe uma descrição!<br>";
                txtsDscObservacao.Focus();
            }


            if (ddlsTipoMovimentacao.SelectedValue == "0")
            {
                mensagensErro += "Selecione o Tipo de Movimentação!<br>";
            }

            if (div_Motivo.Visible)
            {
                if (ddlMotivo.SelectedValue == "0")
                {
                    mensagensErro += "Selecione o Motivo!<br>";
                    ddlMotivo.Focus();
                }
            }

            //Validação dos Itens
            AtualizarItens();
            foreach (var Linha in bs_Movimentacao.Where(c => c.nValorTotal.Equals(0)).Select(x => new { x.SCodigoComDescricao }).ToList())
            {
                mensagensErro += string.Format("O Item <b>{0}</b> está com o valor total Incorreto!<br>", Linha.SCodigoComDescricao, Linha);
                //dtgItens.Rows[]
            }
  
            if (!string.IsNullOrEmpty(mensagensErro))
            {
                MensagemPagina.MostraMensagem_Erro(mensagensErro.Trim());
                return false;
            }
            if (ddlsTipoMovimentacao.SelectedValue == "5")
            {
                if (bs_ItemMudanca.Count == 0)
                    mensagensErro += "É necessário adicionar Itens na grid de Mudança de Posicionamento.<br>";

                AtualizaMemoriaGridMudanca(); // <-- Puxa tudo pro objeto de memória

                foreach (var item in bs_ItemMudanca)
                {
                    if (item.IdLocalDestino <= 0)
                    {
                        mensagensErro += "Atenção: É obrigatório informar um Local de Destino para todos os itens.<br>";
                        break; // Já achou erro, sai do loop.
                    }
                }
            }


            return true;
        }

        private bool ValidarCamposFiltro()
        {
            if (string.IsNullOrEmpty(FiltroPesquisa.SCodigo))
            {
                MensagemPaginaItens.MostraMensagem_Erro("Informe um código do produto");
                txtdtMovimentacao.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(FiltroPesquisa.SDscProduto))
            {
                MensagemPaginaItens.MostraMensagem_Erro("Informe a Descrição do Produto");
                txtsDscObservacao.Focus();
                return false;
            }

            if (FiltroPesquisa.NQuantidade <= 0)
            {
                MensagemPaginaItens.MostraMensagem_Erro("Informe uma Quantidade Maior que 0");
                txtsDscObservacao.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(FiltroPesquisa.SUnidade))
            {
                MensagemPaginaItens.MostraMensagem_Erro("Informe uma unidade");
                return false;
            }

            if (ddlMotivo.SelectedValue == "2" || ddlMotivo.SelectedValue == "1")
            {
                // ACESSANDO O TEXTO BRUTO DIRETAMENTE
                TextBox txtValorInterno = (TextBox)FiltroPesquisa.FindControl("FT_txtnValor");

                decimal valorReal = 0;
                if (txtValorInterno != null)
                {
                    // Usamos o SEU método de limpeza, não o da DLL cls_Comercial_Tabelas
                    valorReal = ConverterMoeda(txtValorInterno.Text);
                }

                if (valorReal <= 0)
                {
                    MensagemPaginaItens.MostraMensagem_Erro("Informe um Valor Maior que 0");
                    // Se o valor estiver sumindo visualmente da tela, comente a linha abaixo para testar:
                    // FiltroPesquisa.Focus_sCodigo(); 
                    return false;
                }
            }

            return true;
        }

        private bool ValidarCamposFiltro2()
        {
            StringBuilder mensagensErro = new StringBuilder();

            if (string.IsNullOrEmpty(FiltroPesquisa1.SCodigo))
            {
                mensagensErro.AppendLine("• Informe um código do produto");
                txtdtMovimentacao.Focus();
            }

            if (string.IsNullOrEmpty(FiltroPesquisa1.SDscProduto))
            {
                mensagensErro.AppendLine("</br• Informe a Descrição do Produto");
            }

            if (FiltroPesquisa1.NQuantidade <= 0)
            {
                mensagensErro.AppendLine("</br• Informe uma Quantidade Maior que 0");
            }

            if (string.IsNullOrEmpty(FiltroPesquisa1.SUnidade))
            {
                mensagensErro.AppendLine("</br• Informe uma unidade");
            }

            if (ddlMotivo.SelectedValue == "2" || ddlMotivo.SelectedValue == "1")
            {
                // PEGANDO O TEXTO BRUTO DIRETO DO TEXTBOX DENTRO DO COMPONENTE
                TextBox txtValorInterno = (TextBox)FiltroPesquisa1.FindControl("FT_txtnValor");

                decimal valorReal = 0;
                if (txtValorInterno != null)
                {
                    // Usamos o seu método que limpa pontos e vírgulas corretamente
                    valorReal = ConverterMoeda(txtValorInterno.Text);
                }

                if (valorReal <= 0)
                {
                    MensagemPaginaItens.MostraMensagem_Erro("Informe um Valor Maior que 0");
                    return false;
                }
            }

            if (ddlsTipoMovimentacao.SelectedValue == "5")
            {
                if (bs_ItemMudanca.Count == 0)
                {
                    mensagensErro.AppendLine("É necessário adicionar Itens na grid de Mudança de Posicionamento.<br>");
                }

                // Valida se ele setou um destino válido para cada linha
                foreach (GridViewRow row in dtgMudancaPosicao.Rows)
                {
                    DropDownList ddlLocDestino = (DropDownList)row.FindControl("ddlLocalDestino");
                    if (ddlLocDestino == null || string.IsNullOrEmpty(ddlLocDestino.SelectedValue) || ddlLocDestino.SelectedValue == "0")
                    {
                        mensagensErro.AppendLine("Atenção: É obrigatório informar um Local de Destino para todos os itens.<br>");
                        break; // Para o loop pra não repetir a mensagem
                    }
                }
            }
            if (mensagensErro.Length > 0)
            {
                MensagemPaginaDePara.MostraMensagem_Erro(mensagensErro.ToString(), false);
                return false;
            }

            return true;
        }

        protected void cmdIncluir_Click(object sender, EventArgs e)
        {
            if (ValidarCamposFiltro())
            {
                cls_WMS_MovimentacaoRel mv = new cls_WMS_MovimentacaoRel();
                string[] vidItem = FiltroPesquisa.IdItem.ToString().Split(',');
                string idItem = vidItem[0].ToString();
                try
                {
                    mv.IdItem = Convert.ToInt32(idItem);
                    mv.IdTipoMovimentacao = Convert.ToInt32(FiltroPesquisa.IdItem.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Item não possui código cadastrado </br>" + ex.Message);
                    return;
                }
                mv.IdProduto = Convert.ToInt32(FiltroPesquisa.IdItem);
                mv.SCodigo = FiltroPesquisa.SCodigo;
                mv.SDscProduto = FiltroPesquisa.SDscProduto;
                mv.SUnidade = FiltroPesquisa.SUnidade;
                mv.TipoProduto = GetCampoTipo(mv.IdProduto.ToString());
                TextBox txtValorIncluir = (TextBox)FiltroPesquisa.FindControl("FT_txtnValor");
                mv.nValorUnitario = ConverterMoeda(txtValorIncluir != null ? txtValorIncluir.Text : "0");
                mv.NQuantidade = FiltroPesquisa.NQuantidade;
                mv.nValorTotal = mv.nValorUnitario * mv.NQuantidade;

                mv.NSerie = string.IsNullOrEmpty(txtnSerie.Text) ? "0" : txtnSerie.Text;
                mv.SLote = txtsLote.Text;
                mv.STipoMov = "N/A";


                var itemExistente = bs_Movimentacao.FirstOrDefault(Item => Item.IdProduto == mv.IdProduto);

                if (itemExistente == null)
                {
                    bs_Movimentacao.Add(mv);
                }
                else
                {
                    MensagemPaginaItens.MostraMensagem_Aviso("<b>Aviso</b> Já Existe um Produto Cadastrado com esse código!");
                }

                dtgItens_DataBind();
                FiltroPesquisa.LimparCampos();
            }
            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
        }

        private decimal ConverterMoeda(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return 0;
            try
            {
                // Limpeza: remove pontos de milhar, espaços e R$
                string limpo = valor.Replace("R$", "").Trim().Replace(".", "").Replace(",", ".");

                // Se após a limpeza o valor for apenas um ponto ou vazio, retorna 0
                if (string.IsNullOrEmpty(limpo) || limpo == ".") return 0;

                return decimal.Parse(limpo, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

    
        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/App/Paginas/WMS/Movimentacao_Consulta.aspx");
        }

        string GetCampoTipo(string idProduto)
        {
            DataSet dsPesquisa;
            string sErro;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_TIPO-PRODUTO");
            vParametros.Add("@idProduto", idProduto);
            dsPesquisa = BD.ExecutarDataSet(sProcedureMov, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                return RETORNO.DATASET(dsPesquisa, "sDscTipoProduto");
            }
            else
            {
                return "Indisponível";
            }
        }

        #endregion

        #region | Numero de Série

        // Quando clicar em "Validar" na Grid principal
        protected void cmdAdicionarSerie_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string[] args = btn.CommandArgument.Split('|');
            string idProduto = args[0];

            // Guarda qual produto estamos validando
            hddIdProduto.Value = idProduto;

            // Carrega a Grid de Etiquetas
            PopularRptItensSalvosValidacao(idProduto);

            // Garante que a câmera e o campo iniciem zerados ao abrir
            txtCodigoBarrasValidacao.Text = "";
            Session["CameraAbertaValidacao"] = false;
            DivBipadorValidacao.Visible = false;
            cmdAbrirCameraValidacao.Text = "Abrir Leitor / Câmera";
            LeitorQuaggaMovimentacao.DesligarCamVariante();
            divCodigoManualValidacao.Style["Display"] = "block";

            updModalItens.Update();

            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModal",
                "$('#modalItens').modal('show'); setTimeout(function() { document.getElementById('" + txtCodigoBarrasValidacao.ClientID + "').focus(); }, 500);", true);
        }

        private void PopularRptItensSalvosValidacao(string idProduto)
        {
            DataSet dsPesquisa;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR-ETIQUETAS-VALIDACAO");
            vParametros.Add("@idMovimentacao", hddidMovimentacao.Value);
            vParametros.Add("@idProduto", idProduto);

            dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);

            bs_ItemIncluirSerie.Clear(); // Aproveitando sua classe global

            if (BD.ValidarDataSet(dsPesquisa))
            {
                foreach (DataRow row in dsPesquisa.Tables[0].Rows)
                {
                    bs_ItemIncluirSerie.Add(new FrameWork.cls_WMS_MovimentacaoRel
                    {
                        IdProduto = Convert.ToInt32(row["idProduto"]),
                        SCodigoBarras = row["sCodigoBarras"].ToString(),
                        SLote = row["sLote"].ToString(),
                        NSerie = row["nSerie"].ToString(),
                        SConfirmado = row["sConfirmado"].ToString()
                    });
                }
            }

            // Calcula os totais para o Header
            int totalValidados = bs_ItemIncluirSerie.Count(x => x.SConfirmado == "S");
            spanQtdPreparados.Text = totalValidados.ToString();
            spanQtdtotal.Text = bs_ItemIncluirSerie.Count.ToString();

            rptItensSalvosValidacao.DataSource = bs_ItemIncluirSerie;
            rptItensSalvosValidacao.DataBind();
        }

        protected void ProcessarLeituraValidacao_Click(object sender, EventArgs e)
        {
            string barcodeLido = "";

            // Se o Quagga jogou pelo controle dele, usamos ele. Se não, pegamos do TextBox direto
            if (!string.IsNullOrEmpty(LeitorQuaggaMovimentacao.GetCodigoBarras(txtCodigoBarrasValidacao)))
            {
                barcodeLido = LeitorQuaggaMovimentacao.GetCodigoBarras(txtCodigoBarrasValidacao).Trim();
            }
            else
            {
                barcodeLido = txtCodigoBarrasValidacao.Text.Trim();
            }

            txtCodigoBarrasValidacao.Text = ""; // Limpa para a próxima leitura

            if (string.IsNullOrEmpty(barcodeLido))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModal", "$('#modalItens').modal('show');", true);
                return;
            }

            // Precisamos recarregar a lista da memória atualizada
            PopularRptItensSalvosValidacao(hddIdProduto.Value);

            // Procura na lista da modal a etiqueta lida
            var etiquetaEncontrada = bs_ItemIncluirSerie.FirstOrDefault(x => x.SCodigoBarras.Equals(barcodeLido, StringComparison.OrdinalIgnoreCase));

            if (etiquetaEncontrada != null)
            {
                if (etiquetaEncontrada.SConfirmado == "S")
                {
                    MensagemPaginaItensModal.MostraMensagem_Aviso("Esta etiqueta já foi validada anteriormente!");
                }
                else
                {
                    // Valida a etiqueta no Banco de Dados
                    Dictionary<string, string> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "VALIDAR-ETIQUETA");
                    vParametros.Add("@idMovimentacao", hddidMovimentacao.Value);
                    vParametros.Add("@sCodigoBarras", barcodeLido);
                    vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario().ToString());

                    string sErro;
                    DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);

                    if (BD.ValidarDataSet(ds, out sErro))
                    {
                        MensagemPaginaItensModal.MostraMensagem_Sucesso($"Etiqueta {barcodeLido} validada com sucesso!");

                        // Recarrega a lista para mostrar a linha verde e atualizar os contadores
                        PopularRptItensSalvosValidacao(hddIdProduto.Value);

                        // CHAMA O SEU MÉTODO ISOLADO: Vai no banco perguntar se a Movimentação inteira acabou
                        ValidarStatus(hddidStatus.Value, false);

                        // Se a função acima NÃO redirecionou (ou seja, não fechou a OPI inteira), 
                        // verificamos se pelo menos ESSE produto específico terminou para fechar a modal:
                        if (bs_ItemIncluirSerie.All(x => x.SConfirmado == "S") && bs_ItemIncluirSerie.Count > 0)
                        {
                            if (Session["CameraAbertaValidacao"] != null && (bool)Session["CameraAbertaValidacao"])
                            {
                                LeitorQuaggaMovimentacao.DesligarCamVariante();
                                Session["CameraAbertaValidacao"] = false;
                                DivBipadorValidacao.Visible = false;
                                cmdAbrirCameraValidacao.Text = "Abrir Leitor / Câmera";
                            }

                            dtgItens_DataBind();
                            updDtgItens.Update();

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "fecharModalSucesso",
                                "setTimeout(function() { $('#modalItens').modal('hide'); $('.modal-backdrop').remove(); $('body').removeClass('modal-open'); $('body').css('overflow', ''); }, 1000);", true);

                            return;
                        }
                    }
                    else
                    {
                        MensagemPaginaItensModal.MostraMensagem_Erro("Erro no Banco: " + sErro);
                    }
                }
            }
            else
            {
                MensagemPaginaItensModal.MostraMensagem_Erro("Atenção: A etiqueta não pertence a este produto na movimentação.");
            }

            // --- SOLUÇÃO DO RESET DA CÂMERA ---
            bool isCameraAberta = Session["CameraAbertaValidacao"] != null && (bool)Session["CameraAbertaValidacao"];

            //string scriptFinal = "$('#modalItens').modal('show'); ";
            string scriptFinal = "";

            if (isCameraAberta)
            {
                LeitorQuaggaMovimentacao.AbrirCameraVariante();
                divCodigoManualValidacao.Style["Display"] = "none";
            }
            else
            {
                scriptFinal += "setTimeout(function() { document.getElementById('" + txtCodigoBarrasValidacao.ClientID + "').focus(); }, 200);";
                divCodigoManualValidacao.Style["Display"] = "block";
            }


            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "keepModal", scriptFinal, true);
        }

        protected void AbrirCameraValidacao_Click(object sender, EventArgs e)
        {
            bool abrir = Session["CameraAbertaValidacao"] != null ? (bool)Session["CameraAbertaValidacao"] : false;
            abrir = !abrir;
            Session["CameraAbertaValidacao"] = abrir;

            DivBipadorValidacao.Visible = abrir;
            cmdAbrirCameraValidacao.Text = abrir ? "Fechar Leitor" : "Abrir Leitor";

            if (abrir)
            {
                LeitorQuaggaMovimentacao.AbrirCameraVariante();
                divCodigoManualValidacao.Style["Display"] = "none"; // Oculta o campo manual se a câmera estiver ligada
            }
            else
            {
                LeitorQuaggaMovimentacao.DesligarCamVariante();
                divCodigoManualValidacao.Style["Display"] = "block"; // Mostra o campo manual se desligar
            }

            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
        }

        #region | Função de Imagem

        protected void rptItemSugerido_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Image img = (Image)e.Item.FindControl("imgProdutoPrincipal");

                TextBox txtIdProduto = e.Item.FindControl("txtIdProduto") as TextBox;
                DropDownList ddlArmazenamento = e.Item.FindControl("ddlArmazenamento") as DropDownList;

                FUNCOES.Popula_Combo(ddlArmazenamento, "sp_Manipula_tbl_Flow_Produtos_Movimentacao 'FLOW_LOCAL_ARMAZENAMENTO'", "idLocalArmazenamento", "sDscLocalArmazenamento", false, "Selecione o Local", "0");

                if (txtIdProduto != null)
                {
                    CarregaImgProduto(txtIdProduto.Text, img);
                }

                Button cmdIncluirSeries = (Button)e.Item.FindControl("cmdIncluirSeries");

                if (cmdIncluirSeries != null)
                {
                    if (bs_ItemIncluirSerie.Count < Convert.ToInt32(hddQtdTotal.Value))
                    {
                        cmdIncluirSeries.Visible = true;
                    }
                    else
                    {
                        cmdIncluirSeries.Visible = false;
                        MensagemPaginaItensModal.MostraMensagem("<b>Lembrete:</b> Limite de inclusão atingido, salve os dados para registrar alterações", "info", false);
                    }
                }


                var item = (FrameWork.cls_WMS_MovimentacaoRel)e.Item.DataItem;

                DropDownList ddlPosicao = e.Item.FindControl("ddlPosicaoPai") as DropDownList;

                if (ddlArmazenamento != null)
                {
                    // Preenche a DropDownList
                    FUNCOES.Popula_Combo(
                        ddlArmazenamento,
                        "sp_Manipula_tbl_Flow_Produtos_Movimentacao 'FLOW_LOCAL_ARMAZENAMENTO'",
                        "idLocalArmazenamento",
                        "sDscLocalArmazenamento",
                        false,
                        "Selecione o Local",
                        "0"
                    );

                    // Configura o valor selecionado baseado no IdLocalArmazenamento do item
                    if (item.IdLocalArmazenamento > 0)
                    {
                        ddlArmazenamento.SelectedValue = item.IdLocalArmazenamento.ToString();
                        FUNCOES.Popula_Combo(ddlPosicao, $"{sProcedureLocal} 'FLOW-POSICOES', @idLocalArmazenamento={ddlArmazenamento.SelectedValue}", "idPosicao", "sCodigoLocal", false, "Selecione a Posição", "0");
                        ddlPosicao.SelectedValue = item.IdPosicao.ToString();
                    }

                    if (ddlsTipoMovimentacao.SelectedValue == "5")
                    {
                        ddlArmazenamento.Attributes.Add("disabled", "disabled");
                        ddlPosicao.Attributes.Add("disabled", "disabled");
                    }
                    else
                    {
                        ddlArmazenamento.Attributes.Remove("disabled");
                        ddlPosicao.Attributes.Remove("disabled");
                    }
                }

            }
        }

        protected void CarregaImgProduto(string idProduto, Image img)
        {
            DataTable dsPesquisa;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_IMAGEM");
            vParametros.Add("@idTipoArquivo", "201");
            vParametros.Add("@idObjeto", idProduto);
            dsPesquisa = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (dsPesquisa.Rows.Count > 0)
            {
                DataRow imgBd = dsPesquisa.Rows[0];
                string imgUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])imgBd["vbArquivo"]);

                img.ImageUrl = imgUrl;
                img.Visible = true;
            }
            else
            {
                img.ImageUrl = "/App/img/wms_dimensoes.svg";
                img.Visible = true;
            }
        }

        #endregion

        void AbrirModal()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ResetAndOpenModalItens", "resetAndOpenModalItens();", true);
        }

        #endregion

        #region | Modal nSerie

        private void FecharModal()
        {
            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_CloseModal", "$('#modalItens').modal('hide'); $('.modal-backdrop').remove(); $('body').removeClass('modal-open'); $('body').css('overflow', '');", true);
        }

        protected void cmdFecharSerie_Click(object sender, EventArgs e)
        {
            FecharModal();
        }

        protected void ddlLocal_SelectedIndexChanged(object sender, EventArgs e)
        {
            hfModalAberta.Value = "True";  // Mantém o estado do modal
            DropDownList ddlLocal = (DropDownList)sender;
            RepeaterItem item = (RepeaterItem)ddlLocal.NamingContainer;
            DropDownList ddlPosicaoPai = (DropDownList)item.FindControl("ddlPosicaoPai");

            if (ddlPosicaoPai != null)
            {
                PopularComboGrid(ddlLocal.SelectedValue, ddlPosicaoPai);
            }

            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
        }

        void PopularComboGrid(string idLocal, DropDownList ddlPosicao)
        {
            FUNCOES.Popula_Combo(ddlPosicao, $"{sProcedureLocal} 'FLOW-POSICOES', @idLocalArmazenamento={idLocal}", "idPosicao", "sCodigoLocal", false, "Selecione a Posição", "0");
        }

        protected void ddlLocal_Salvos_SelectedIndexChanged(object sender, EventArgs e)
        {
            hfModalAberta.Value = "True";
            DropDownList ddlLocal = (DropDownList)sender;
            RepeaterItem item = (RepeaterItem)ddlLocal.NamingContainer;
            DropDownList ddlPosicaoPai = (DropDownList)item.FindControl("ddlPosicaoPai");

            if (ddlPosicaoPai != null)
            {
                PopularComboGrid(ddlLocal.SelectedValue, ddlPosicaoPai);
            }
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalSugestao", "$('#modalItens').modal('show');", true);
        }

        #endregion

        #region | De/Para

        protected void AtualizaConteudoClasse(List<cls_WMS_MovimentacaoRel> lista, string str)
        {
            int nContador = 0;
            try
            {
                foreach (GridViewRow item in dtgItens.Rows)
                {
                    TextBox txtnFator_Linha = (TextBox)item.FindControl(str);
                    if (lista[nContador] is cls_WMS_MovimentacaoRel movimentacao)
                    {
                        if (str.ToString() == "NQuantidade".ToString() && txtnFator_Linha.Text != "")
                            movimentacao.NQuantidade = decimal.Parse(txtnFator_Linha.Text);
                        else if (str == "nValorUnitario" && txtnFator_Linha.Text != "")
                            movimentacao.nValorUnitario = ConverterMoeda(txtnFator_Linha.Text);
                        else if (str == "nValorTotal" && txtnFator_Linha.Text != "")
                            movimentacao.nValorTotal = decimal.Parse(txtnFator_Linha.Text);
                    }
                    nContador++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // De
        protected void cmdIncluirRetirada_Click(object sender, EventArgs e)
        {
            if (ValidarCamposFiltro2() || ddlsTipoMovimentacao.SelectedValue == "5")
            {
                cls_WMS_MovimentacaoRel mv = new cls_WMS_MovimentacaoRel();
                string[] vidItem = FiltroPesquisa1.IdItem.ToString().Split(',');
                string idItem = vidItem[0].ToString();
                try
                {
                    mv.IdItem = Convert.ToInt32(idItem);
                    mv.IdTipoMovimentacao = Convert.ToInt32(FiltroPesquisa1.IdItem.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Item não possui código cadastrado </br>" + ex.Message);
                    return;
                }
                mv.IdProduto = Convert.ToInt32(FiltroPesquisa1.IdItem);
                mv.SCodigo = FiltroPesquisa1.SCodigo;
                mv.SDscProduto = FiltroPesquisa1.SDscProduto;
                mv.SUnidade = FiltroPesquisa1.SUnidade;
                mv.TipoProduto = FiltroPesquisa1.STipoProduto;
                mv.nValorUnitario = FiltroPesquisa1.NValorProduto;
                mv.NQuantidade = FiltroPesquisa1.NQuantidade;
                mv.nValorTotal = mv.nValorUnitario * mv.NQuantidade;

                if (ddlsTipoMovimentacao.SelectedValue == "5")
                {
                    mv.IdLocalArmazenamento = Convert.ToInt32(ddlLocal.SelectedValue);
                    mv.IdPosicao = Convert.ToInt32(ddlPosicaoPai.SelectedValue);
                }


                var itemExistente = bs_ItemSaida.FirstOrDefault(Item => Item.IdProduto == mv.IdProduto);

                // Verifica se o item existe na outra lista
                var itemEmEntrada = bs_ItemEntrada.FirstOrDefault(Item => Item.IdProduto == mv.IdProduto);

                if (itemExistente == null && itemEmEntrada == null || itemExistente == null && ddlsTipoMovimentacao.SelectedValue == "5")
                {
                    bs_ItemSaida.Add(mv);
                }
                else if (itemEmEntrada != null)
                {
                    MensagemPaginaDePara.MostraMensagem_Erro("Este item já está na lista de entrada e não pode ser adicionado à saída.");
                }

                dtgItensSaida_DataBind();
                if (ddlsTipoMovimentacao.SelectedValue != "5")
                {
                    FiltroPesquisa1.LimparCampos();
                    LimpaIdFiltroPesquisa();
                }
            }
            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
        }

        // Para
        protected void cmdIncluirEntrada_Click(object sender, EventArgs e)
        {
            if (ValidarCamposFiltro2() || ddlsTipoMovimentacao.SelectedValue == "5")
            {
                cls_WMS_MovimentacaoRel mv = new cls_WMS_MovimentacaoRel();
                string[] vidItem = FiltroPesquisa1.IdItem.ToString().Split(',');
                string idItem = vidItem[0].ToString();
                try
                {
                    mv.IdItem = Convert.ToInt32(idItem);
                    mv.IdTipoMovimentacao = Convert.ToInt32(FiltroPesquisa1.IdItem.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Item não possui código cadastrado </br>" + ex.Message);
                    return;
                }
                mv.IdProduto = Convert.ToInt32(FiltroPesquisa1.IdItem);
                mv.SCodigo = FiltroPesquisa1.SCodigo;
                mv.SDscProduto = FiltroPesquisa1.SDscProduto;
                mv.SUnidade = FiltroPesquisa1.SUnidade;
                mv.TipoProduto = FiltroPesquisa1.STipoProduto;
                mv.nValorUnitario = FiltroPesquisa1.NValorProduto;
                mv.NQuantidade = FiltroPesquisa1.NQuantidade;
                mv.nValorTotal = mv.nValorUnitario * mv.NQuantidade;

                if (ddlsTipoMovimentacao.SelectedValue == "5")
                {
                    mv.IdLocalArmazenamento = Convert.ToInt32(ddlLocal.SelectedValue);
                    mv.IdPosicao = Convert.ToInt32(ddlPosicaoPai.SelectedValue);
                }

                var itemExistente = bs_ItemEntrada.FirstOrDefault(Item => Item.IdProduto == mv.IdProduto);

                var itemEmSaida = bs_ItemSaida.FirstOrDefault(Item => Item.IdProduto == mv.IdProduto);

                if (itemExistente == null && itemEmSaida == null || itemExistente == null && ddlsTipoMovimentacao.SelectedValue == "5")
                {
                    bs_ItemEntrada.Add(mv);
                }
                else if (itemEmSaida != null && ddlsTipoMovimentacao.SelectedValue != "5")
                {
                    MensagemPaginaDePara.MostraMensagem_Erro("Este item já está na lista de saída e não pode ser adicionado à entrada.");
                }


                dtgItensEntrada_DataBind();
                if (ddlsTipoMovimentacao.SelectedValue != "5")
                {
                    FiltroPesquisa1.LimparCampos();
                    LimpaIdFiltroPesquisa();
                }
            }
            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
        }

        protected void cmdIncluirMov_Click(object sender, EventArgs e)
        {
            if (ValidarCamposFiltro2())
            {
                cmdIncluirEntrada_Click(sender, e);
                cmdIncluirRetirada_Click(sender, e);

                MensagemPaginaDePara.MostraMensagem("<b>Info:</b> As Posições não podem ser iguais se o <b>Local(Atual)</b> for o igual ao <b>Local(final)</b> e o <b>Local</b> não pode ser igual se não houver <b>Posição</b>.", "info", false);

                FiltroPesquisa1.LimparCampos();
                LimpaIdFiltroPesquisa();

                ddlPosicaoPai.Items.Clear();
                ddlLocal.Items.Clear();
                ViewState["UltimoIdProduto"] = "";
                FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
                FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
                FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
            }
        }

        void dtgItensEntrada_DataBind()
        {
            try
            {
                dtgItensEntrada.DataSource = bs_ItemEntrada;
                dtgItensEntrada.DataBind();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao vincular dados: " + ex.Message);
            }
            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
        }

        void dtgItensSaida_DataBind()
        {
            try
            {
                dtgItensSaida.DataSource = bs_ItemSaida;
                dtgItensSaida.DataBind();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao vincular dados: " + ex.Message);
            }
            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
        }

        void LimpaIdFiltroPesquisa()
        {
            FiltroPesquisa1.IdItem = "";
        }

        protected void dtgItensEntrada_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (isDeleting) return;

            isDeleting = true;

            int index = e.RowIndex;
            int idProdutoEntrada = (int)dtgItensEntrada.DataKeys[index].Value;
            bs_ItemEntrada.RemoveAt(index);
            dtgItensEntrada_DataBind();

            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();

            if (ddlsTipoMovimentacao.SelectedValue == "5")
            {
                DeletarItemSaidaPorIdProduto(idProdutoEntrada);
            }

            isDeleting = false;
        }

        protected void dtgItensSaida_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (isDeleting) return;

            isDeleting = true;

            int index = e.RowIndex;
            int idProdutoSaida = (int)dtgItensSaida.DataKeys[index].Value;
            bs_ItemSaida.RemoveAt(index);
            dtgItensSaida_DataBind();

            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();

            if (ddlsTipoMovimentacao.SelectedValue == "5")
            {
                DeletarItemEntradaPorIdProduto(idProdutoSaida);
            }

            isDeleting = false;
        }

        private void DeletarItemSaidaPorIdProduto(int idProduto)
        {
            for (int i = 0; i < bs_ItemSaida.Count; i++)
            {
                if (((FrameWork.cls_WMS_MovimentacaoRel)bs_ItemSaida[i]).IdProduto == idProduto)
                {
                    bs_ItemSaida.RemoveAt(i);
                    dtgItensSaida_DataBind();
                    break;
                }
            }
        }

        private void DeletarItemEntradaPorIdProduto(int idProduto)
        {
            for (int i = 0; i < bs_ItemEntrada.Count; i++)
            {
                if (((FrameWork.cls_WMS_MovimentacaoRel)bs_ItemEntrada[i]).IdProduto == idProduto)
                {
                    bs_ItemEntrada.RemoveAt(i);
                    dtgItensEntrada_DataBind();
                    break;
                }
            }
        }

        protected void dtgItensSaida_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            bool exibirColunas = ddlsTipoMovimentacao.SelectedValue == "5";
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlArmazenamento = e.Row.FindControl("ddlLocal") as DropDownList;
                DropDownList ddlPosicaoPai = e.Row.FindControl("ddlPosicaoPai") as DropDownList;
                HiddenField hddidProdutoGvSaida = e.Row.FindControl("hddidProdutoGvSaida") as HiddenField;

                var item = (cls_WMS_MovimentacaoRel)e.Row.DataItem;

                if (item != null)
                {
                    e.Row.Cells[indiceLocal].Visible = exibirColunas;
                    e.Row.Cells[indicePosicao].Visible = exibirColunas;

                    if (exibirColunas)
                    {
                        string idLocal = ddlArmazenamento.SelectedValue;

                        FUNCOES.Popula_Combo(ddlArmazenamento, $"sp_Manipula_tbl_Flow_Produtos_Movimentacao 'FLOW_LOCAL_FILTRADO', @idProduto={hddidProdutoGvSaida.Value}", "idLocalArmazenamento", "sDscLocalArmazenamento", false, "Selecione o Local", "0");


                        if (ddlArmazenamento.Items.FindByValue(item.IdLocalArmazenamento.ToString()) != null)
                        {
                            ddlArmazenamento.SelectedValue = item.IdLocalArmazenamento.ToString();
                            FUNCOES.Popula_Combo(ddlPosicaoPai, $"{sProcedureLocal} 'FLOW-POSICOES', @idLocalArmazenamento={ddlArmazenamento.SelectedValue}", "idPosicao", "sCodigoLocal", false, "Selecione a Posição", "0");
                            ddlPosicaoPai.SelectedValue = item.IdPosicao.ToString();
                        }
                    }
                }
            }

            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[indiceLocal].Visible = exibirColunas;
                e.Row.Cells[indicePosicao].Visible = exibirColunas;
            }

            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
        }

        protected void dtgItensEntrada_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            bool exibirColunas = ddlsTipoMovimentacao.SelectedValue == "5";

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlArmazenamento = e.Row.FindControl("ddlLocal") as DropDownList;
                DropDownList ddlPosicaoPai = e.Row.FindControl("ddlPosicaoPai") as DropDownList;

                var item = (cls_WMS_MovimentacaoRel)e.Row.DataItem;

                if (item != null)
                {
                    e.Row.Cells[indiceLocal].Visible = exibirColunas;
                    e.Row.Cells[indicePosicao].Visible = exibirColunas;

                    if (exibirColunas)
                    {
                        FUNCOES.Popula_Combo(ddlArmazenamento, "sp_Manipula_tbl_Flow_Produtos_Movimentacao 'FLOW_LOCAL_ARMAZENAMENTO'", "idLocalArmazenamento", "sDscLocalArmazenamento", false, "Selecione o Local", "0");

                        if (ddlArmazenamento.Items.FindByValue(item.IdLocalArmazenamento.ToString()) != null)
                        {
                            ddlArmazenamento.SelectedValue = item.IdLocalArmazenamento.ToString();
                            FUNCOES.Popula_Combo(ddlPosicaoPai, $"{sProcedureLocal} 'FLOW-POSICOES', @idLocalArmazenamento={ddlArmazenamento.SelectedValue}", "idPosicao", "sCodigoLocal", false, "Selecione a Posição", "0");
                            ddlPosicaoPai.SelectedValue = item.IdPosicao.ToString();
                        }
                    }
                }
            }

            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[indiceLocal].Visible = exibirColunas;
                e.Row.Cells[indicePosicao].Visible = exibirColunas;
            }
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
        }

        protected void ddlLocal_Mov_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlLocal = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddlLocal.NamingContainer;
            DropDownList ddlPosicaoPai = (DropDownList)row.FindControl("ddlPosicaoPai");

            if (ddlPosicaoPai != null)
            {
                PopularComboGrid(ddlLocal.SelectedValue, ddlPosicaoPai);
            }

            FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
            AtualizarItensDaGrid();
            dtgItensEntrada_DataBind();
            dtgItensSaida_DataBind();
        }

        private void AtualizarItensDaGrid()
        {
            foreach (GridViewRow row in dtgItensSaida.Rows)
            {
                int idProduto = Convert.ToInt32(dtgItensSaida.DataKeys[row.RowIndex].Value);

                var itemSaida = bs_ItemSaida.FirstOrDefault(i => i.IdProduto == idProduto);
                if (itemSaida != null)
                {
                    itemSaida.IdLocalArmazenamento = Convert.ToInt32(((DropDownList)row.FindControl("ddlLocal")).SelectedValue);
                    itemSaida.IdPosicao = Convert.ToInt32(((DropDownList)row.FindControl("ddlPosicaoPai")).SelectedValue);
                }
            }

            foreach (GridViewRow row in dtgItensEntrada.Rows)
            {
                int idProduto = Convert.ToInt32(dtgItensEntrada.DataKeys[row.RowIndex].Value);

                var itemEntrada = bs_ItemEntrada.FirstOrDefault(i => i.IdProduto == idProduto);
                if (itemEntrada != null)
                {
                    itemEntrada.IdLocalArmazenamento = Convert.ToInt32(((DropDownList)row.FindControl("ddlLocal")).SelectedValue);
                    itemEntrada.IdPosicao = Convert.ToInt32(((DropDownList)row.FindControl("ddlPosicaoPai")).SelectedValue);
                }
            }
        }

        public void PopularLocal(string idProduto)
        {
            FUNCOES.Popula_Combo(ddlLocal, $"sp_Manipula_tbl_Flow_Produtos_Movimentacao 'FLOW_LOCAL_FILTRADO', @idProduto={idProduto}", "idLocalArmazenamento", "sDscLocalArmazenamento", false, "Selecione o Local", "0");

            if (ddlLocal.Items.Count < 2 && ddlsTipoMovimentacao.SelectedValue == "5")
            {
                MensagemPaginaDePara.MostraMensagem_Erro("<b>Erro:</b> Não será possível inserir esse produto pois nenhum Local Atual foi atribuído.");
            }
        }

        #endregion

        #region | PO Visibilidade

        protected void chkAlternar_CheckedChanged(object sender, EventArgs e)
        {
            // Alterna a visibilidade dos controles com base no estado do checkbox
            txtsPedidoCompra.Visible = !chkAlternar.Checked;
            ddlsPO.Visible = chkAlternar.Checked;
            divVisibleItens.Visible = !chkAlternar.Checked;
            var idMotivo = ddlMotivo.SelectedValue;
            var idTipo = ddlsTipoMovimentacao.SelectedValue;


            if (chkAlternar.Checked)
            {
                MensagemPaginaItens.MostraMensagem("<b>Lembrete: </b>Ao importar dados, não é possível adicionar novos itens, apenas itens importados selecionando um Pedido de Compra.", "info", false);
                ddlFornecedor.Attributes.Add("Disabled", "Disabled");
            }
            else
            {
                ddlFornecedor.Attributes.Remove("Disabled");
                ddlFornecedor.SelectedValue = "0";
                ddlsPO.SelectedValue = "0";
                LimpaCampos();
                bs_Movimentacao.Clear();
                txtsDscObservacao.Text = "";
                txtsPedidoCompra.Text = "";
                ddlMotivo.SelectedValue = idMotivo;
                ddlsTipoMovimentacao.SelectedValue = idTipo;
                dtgItens.DataSource = bs_Movimentacao;
                dtgItens.DataBind();
            }
        }

        #endregion

        #region | Efetivação

        public void AtualizarComex()
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "ATUALIZAR-COMEX");
                vParametros.Add("@idMovimentacao", hddidMovimentacao.Value);

                BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);
            }
            catch (Exception ex)
            {
                // Se der erro ao atualizar a dtChegada, você pode tratar ou logar aqui
                System.Diagnostics.Debug.WriteLine("Erro ao atualizar dtChegada: " + ex.Message);
            }
        }

        private bool ExisteEstoquePadrao()
        {
            bool existe = false;
            try
            {
                // Busca 1 registro que seja padrão e esteja ativo
                string sql = "SELECT TOP 1 idLocalArmazenamento FROM tbl_Flow_WMS_LocalArmazenamento (NOLOCK) WHERE sEstoquePadrao = 'S' AND ISNULL(sSituacao, 'S') = 'S'";

                SqlDataReader dr = BD.ExecutarDataReader(sql);

                if (dr != null)
                {
                    if (dr.Read())
                    {
                        existe = true; // Achou pelo menos 1
                    }
                    dr.Close(); // Libera a conexão
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao verificar Estoque Padrão: " + ex.Message);
            }

            return existe;
        }

        protected void cmdEfetivar_Click(object sender, EventArgs e)
        {
            if (ddlsTipoMovimentacao.SelectedValue == "2")
            {
                // Checa se tem algum local marcado como Padrão no banco
                if (!ExisteEstoquePadrao())
                {
                    string linkLocal = "/App/Paginas/WMS/Manutencao/LocalArmazenamento.aspx";

                    // Mostra a mensagem de erro já com o HTML do Link para a tela de Locais
                    MensagemPaginaErro.MostraMensagem_Erro($"<b>Ação Bloqueada:</b> Não há nenhum <b>Estoque Padrão</b> configurado no sistema para receber a entrada de mercadorias.<br/><br/><i class='fa fa-external-link'></i> <a href='{linkLocal}' target='_blank' style='text-decoration: underline;'>Clique aqui para acessar a tela de Locais e configurar um Estoque Padrão</a>.", true);

                    return; // Para a execução e impede a efetivação!
                }
            }

            SalvarDados("S");
            AtualizarStatus("4");
            AtualizarComex();
        }

        #endregion

        #region | Produção Eletrônica

        void LimparCamposMotivo()
        {
            txtsPedidoCompra.Text = "";
            ddlFornecedor.SelectedValue = "0";
        }

        void LimparTela()
        {
            divItens.Visible = false;
            divMudancaPosicao.Visible = false;
            DIV_Fornecedor.Visible = false;
            divItensDestino.Visible = false;
            div_Lote.Visible = false;
            div_Impressora.Visible = false;
            div_Form.Visible = false;
            div_Descricao.Visible = false;
            PainelAtualizacao.Visible = false;
            lblFornecedor.InnerText = "Empresa";
            cmdSalvar.Visible = false;


        }
        protected void ddlsTipoMovimentacao_SelectedIndexChanged(object sender, EventArgs e)
        {

            LimparTela();
            DropDownList ddl = (DropDownList)sender;
            string ID_ddl = ddl.SelectedValue;
    
            if (ID_ddl == "0")
            {
                div_Motivo.Visible = false;
            }
            else
            {
                PopularCombo_Motivo(ID_ddl);
            }


            //var chaves = new[] { "Valor Unitário" };
            //var dicAtivarValor = chaves.ToDictionary(k => k, v => true);
            //var dicDesativarValor = chaves.ToDictionary(k => k, v => false);

            ////else if (ID_ddl == "3")
            ////    lblFornecedor.InnerText = "Cliente";
            ////else if (ID_ddl == "2")
            //{
            //    ////divItens.Visible = true;
            //    //divItensDestino.Visible = false;
            //    //div_Motivo.Visible = true;
            //    //lblFornecedor.InnerText = "Fornecedor";

            //    //if (ddlMotivo.SelectedValue == "1" || ddlMotivo.SelectedValue == "3")
            //    //    div_Lote.Visible = true;

            //    //div_Form.Visible = true;
            //    //div_Descricao.Visible = true;

            //    //if (ddlMotivo.SelectedValue == "1")
            //    //{
            //    //    div_Impressora.Visible = true;
            //    //    FUNCOES.ReexibirColunas(dtgItens, "Valor");
            //    //    FUNCOES.ReexibirColunas(dtgItens, "Total");
            //    //    FiltroPesquisa.ModificaTamanhoCampos(2, 4, 2, 2, 2);
            //    //    FiltroPesquisa.Controle_ExibicaoCampos(dicAtivarValor, "1");
            //    //}
            //    //else
            //    //{
            //    //    div_Impressora.Visible = false;
            //    //    FUNCOES.EsconderColunas(dtgItens, "Valor");
            //    //    FUNCOES.EsconderColunas(dtgItens, "Total");
            //    //    FiltroPesquisa.ModificaTamanhoCampos(2, 6, 2, 2, 0);
            //    //    FiltroPesquisa.Controle_ExibicaoCampos(dicDesativarValor, "1");
            //    //}
            //}
            //else if (ID_ddl == "4")
            //{
            //    //lblDePara.InnerText = "Transformação de Unidade";
            //    //lblFornecedor.InnerText = "Empresa";
            //    //divItens.Visible = false;
            //    //divItensDestino.Visible = true;
            //    //div_Motivo.Visible = true;
            //    //div_Lote.Visible = false;
            //    //div_movDePara.Visible = false;
            //    //div_botoaoRetirada.Visible = true;
            //    //div_botoaoEntrada.Visible = true;
            //    //div_botoaoMov.Visible = false;
            //    //div_Impressora.Visible = false;
            //    //div_Form.Visible = false;
            //    //div_Descricao.Visible = true;
            //    //FUNCOES.EsconderColunas(dtgItens, "Valor");
            //    //FUNCOES.EsconderColunas(dtgItens, "Total");
            //    //FiltroPesquisa.ModificaTamanhoCampos(2, 6, 2, 2, 0);
            //    //FiltroPesquisa.Controle_ExibicaoCampos(dicDesativarValor, "1");
            //}
            //else if (ID_ddl == "5")
            //{
            //    //    lblDePara.InnerText = "Mudança de Posicionamento";
            //    //    lblFornecedor.InnerText = "Empresa";

            //    //    divItens.Visible = false;
            //    //    divItensDestino.Visible = false; // Matamos o De/Para antigo!
            //    //    divMudancaPosicao.Visible = true; // Ligamos a nova tela!

            //    //    div_Motivo.Visible = true;
            //    //    div_Lote.Visible = false;
            //    //    div_Impressora.Visible = false;
            //    //    div_Form.Visible = false;
            //    //    div_Descricao.Visible = true;

            //    //    // Configura o novo filtro
            //    //    FiltroPesquisaMudanca.ModificaTamanhoCampos(2, 6, 2, 2, 0);
            //    //    FiltroPesquisaMudanca.Controle_ExibicaoCampos(dicDesativarValor, "1");
            //}
            ////else
            ////{
            ////    lblFornecedor.InnerText = "Empresa";
            ////    divItensDestino.Visible = false;
            ////    div_Motivo.Visible = true;
            ////    div_Lote.Visible = false;
            ////    divItens.Visible = true;
            ////    div_Impressora.Visible = false;
            ////    div_Form.Visible = true;
            ////    div_Descricao.Visible = true;
            ////}

            //// Armazenando estados de visibilidade no ViewState
            ////ViewState["divItensDestinoVisible"] = divItensDestino.Visible;
            ////ViewState["divItensVisible"] = divItens.Visible;
            ////ViewState["divMotivoVisible"] = div_Motivo.Visible;
            ////ViewState["divLoteVisible"] = div_Lote.Visible;
            ////
            ////FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
            ////FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
            ////FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
        }

        protected void ddlMotivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.Equals(sRequestAcao, "ImportarXML", StringComparison.OrdinalIgnoreCase))
            {

                LimparTela();

                DropDownList ddl = (DropDownList)sender;
                string ID_ddl = ddl.SelectedValue;


                var chaves = new[] { "Valor Unitário" };
                var dicAtivarValor = chaves.ToDictionary(k => k, v => true);
                var dicDesativarValor = chaves.ToDictionary(k => k, v => false);
                div_ImportComex.Visible = false;


                switch (ID_ddl)
                {
                    case "1": //Compras
                        PopularImpressora();
                        PopularComboFornecedor(ID_ddl);
                        lblFornecedor.InnerText = "Fornecedor";
                        div_Referencia.Visible = true;
                        div_Lote.Visible = true;
                        div_Impressora.Visible = true;
                        div_Descricao.Visible = true;
                        divItens.Visible = true;
                        div_Form.Visible = true;
                        DIV_Fornecedor.Visible = true;
                        FiltroPesquisa.Controle_ExibicaoCampos(dicAtivarValor, "1");
                        FUNCOES.ReexibirColunas(dtgItens, "Valor");
                        FUNCOES.ReexibirColunas(dtgItens, "Total");
                        FiltroPesquisa.ModificaTamanhoCampos(2, 4, 2, 2, 2);
                        LimparCamposMotivo();
                        cmdSalvar.Visible = true;
                        break;
                    case "2":
                        break;

                }

                //ddlFornecedor.Attributes.Remove("disabled");
                //txtsNotaFiscal.Attributes.Remove("disabled");

                //if (ID_ddl == "4")
                //{
                //    PopularComboFornecedor(ID_ddl);
                //    div_Lote.Visible = false;

                //    var item = ddlFornecedor.Items.FindByValue("779");
                //    if (item != null)
                //        ddlFornecedor.SelectedValue = "779";
                //    else
                //        ddlFornecedor.SelectedValue = "0";

                //    if (chkAlternar.Checked)
                //    {
                //        divVisibleItens.Visible = true;
                //        chkAlternar.Checked = false;
                //    }

                //    div_Referencia.Visible = false;
                //    bs_Movimentacao.Clear();
                //    dtgItens.DataSource = bs_Movimentacao;
                //    dtgItens.DataBind();
                //    txtsPedidoCompra.Text = "Interno";
                //    ddlFornecedor.Attributes.Add("disabled", "disabled");
                //    FUNCOES.EsconderColunas(dtgItens, "Valor");
                //    FUNCOES.EsconderColunas(dtgItens, "Total");
                //    FiltroPesquisa.ModificaTamanhoCampos(2, 6, 2, 2, 0);
                //    FiltroPesquisa.Controle_ExibicaoCampos(dicDesativarValor, "1");
                //}
                //else if (ID_ddl == "1")
                //{
                //    if (ddlsTipoMovimentacao.SelectedValue == "2")
                //    {
                //    }

                //}
                //else if (ID_ddl == "3")
                //{
                //    div_Referencia.Visible = true;
                //    div_Lote.Visible = false;

                //    if (ddlsTipoMovimentacao.SelectedValue == "2")
                //    {
                //        div_Impressora.Visible = true;
                //        PopularImpressora();
                //        ddlImpressora.SelectedValue = "2";
                //    }

                //    FiltroPesquisa.Controle_ExibicaoCampos(dicAtivarValor, "1");
                //    FiltroPesquisa.ModificaTamanhoCampos(2, 4, 2, 2, 2);
                //    FUNCOES.EsconderColunas(dtgItens, "Valor");
                //    FUNCOES.EsconderColunas(dtgItens, "Total");
                //    LimparCamposMotivo();
                //}
                //else if (ID_ddl == "2")
                //{
                //    div_Referencia.Visible = true;
                //    FiltroPesquisa.ModificaTamanhoCampos(2, 4, 2, 2, 2);
                //    FiltroPesquisa.Controle_ExibicaoCampos(dicAtivarValor, "1");
                //    FUNCOES.ReexibirColunas(dtgItens, "Valor");
                //    FUNCOES.ReexibirColunas(dtgItens, "Total");
                //    LimparCamposMotivo();
                //}
                //else if (ID_ddl == "9")
                //{
                //    if (ddlsTipoMovimentacao.SelectedValue == "2")
                //    {
                //        div_Impressora.Visible = true;
                //        PopularImpressora();
                //        // ddlImpressora.SelectedValue = "2";
                //    }

                //    bs_Movimentacao.Clear();
                //    ddlFornecedor.Attributes.Add("disabled", "disabled");
                //    txtsNotaFiscal.Attributes.Add("disabled", "disabled");
                //    div_Referencia.Visible = true;
                //    div_Lote.Visible = false;
                //    divVisibleItens.Visible = false;
                //    div_ImportComex.Visible = true;
                //    LimparCamposMotivo();
                //}
                //else
                //{
                //    txtsPedidoCompra.Text = "";
                //    PopularComboFornecedor(ID_ddl);
                //    div_Lote.Visible = false;
                //    div_Impressora.Visible = false;
                //    div_Referencia.Visible = false;
                //    ddlFornecedor.SelectedValue = "0";
                //    ddlFornecedor.Attributes.Remove("disabled");
                //    FiltroPesquisa.ModificaTamanhoCampos(2, 6, 2, 2, 0);
                //    FiltroPesquisa.Controle_ExibicaoCampos(dicDesativarValor, "1");
                //    FUNCOES.EsconderColunas(dtgItens, "Valor");
                //    FUNCOES.EsconderColunas(dtgItens, "Total");
                //    LimparCamposMotivo();
                //}
                // Armazenando estados de visibilidade no ViewState
                ViewState["divLoteVisible"] = div_Lote.Visible;

                if (chkAlternar.Checked)
                {
                    ddlFornecedor.Attributes.Add("Disabled", "Disabled");
                }
                else if (!chkAlternar.Checked && ID_ddl != "4" && ID_ddl != "9")
                {
                    ddlFornecedor.Attributes.Remove("Disabled");
                }

                FiltroPesquisa.RegistrarScriptPesquisarMovimentacao();
                FiltroPesquisa1.RegistrarScriptPesquisarMovimentacao();
                FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
            }
        }

        #endregion

        #region | Guia Usuário

        void GuiarUsuario()
        {
            if (!Convert.ToBoolean(ViewState["isNovo"]) && hddidStatus.Value == "2")
            {
                MensagemPaginaInfos.MostraMensagem("<b>Lembrete: </b> Valide a Etiquetas clicando em <b>Validar</b> na grid para Liberar Efetivação", "info", false);
            }
            else if (hddidStatus.Value == "5")
            {
                MensagemPaginaInfos.MostraMensagem("<b>Lembrete: </b> Produtos Validados, clique em <b>Efetivar o Estoque</b> para que o Saldo seja atualizado!", "info", false);
            }
        }

        #endregion

        #region | Etiquetas

        //protected void cmdImprimirEtiquetas_Click(object sender, EventArgs e)
        //{
        //    GerarEtiquetas();
        //}

        private void GerarEtiquetas()
        {
            try
            {
                // Validação básica
                if (string.IsNullOrEmpty(txtidMovimentacao.Text) || txtidMovimentacao.Text == "0")
                {
                    MensagemPaginaErro.MostraMensagem_Erro("Salve a movimentação antes de gerar etiquetas.", true);
                    return;
                }

                // Verifica se tem impressora selecionada
                if (ddlImpressora.SelectedValue == "0")
                {
                    MensagemPaginaErro.MostraMensagem_Erro("Selecione uma impressora para gerar as etiquetas.", true);
                    ddlImpressora.Focus();
                    return;
                }

                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "GERAR-ETIQUETAS");
                vParametros.Add("@idMovimentacao", txtidMovimentacao.Text);
                vParametros.Add("@idProduto", "0"); // 0 = Gera para TODOS os itens da nota
                vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario().ToString());
                vParametros.Add("@idImpressora", ddlImpressora.SelectedValue);
                vParametros.Add("@idTipoEtiqueta", "2"); // 2 = Etiqueta de Produto

                // Executa
                BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);

                // --- AQUI ESTÁ A MUDANÇA PARA GARANTIR A MENSAGEM ---
                Session["MensagemSucesso"] = "Etiquetas enviadas para a fila de impressão (Local Padrão definido)!";

                // Redireciona para a mesma página (recarrega) para limpar o estado e mostrar a mensagem
                Response.Redirect(urlPagina + "?id=" + txtidMovimentacao.Text);
            }
            catch (Exception ex)
            {
                // Tratamento de erro com Redirect também, para garantir visualização
                //Session["MensagemErro"] = "Erro ao gerar etiquetas: " + ex.Message;
                //Response.Redirect(urlPagina + "?id=" + txtidMovimentacao.Text);
            }
        }

        #region | Imprimir Etiquetas / Fracionamento

        protected void cmdImprimirEtiquetas_Click(object sender, EventArgs e)
        {
            // Validação básica
            if (string.IsNullOrEmpty(txtidMovimentacao.Text) || txtidMovimentacao.Text == "0")
            {
                MensagemPaginaErro.MostraMensagem_Erro("Salve a movimentação antes de gerar etiquetas.", true);
                return;
            }

            if (ddlImpressora.SelectedValue == "0")
            {
                MensagemPaginaErro.MostraMensagem_Erro("Selecione uma impressora para gerar as etiquetas.", true);
                ddlImpressora.Focus();
                return;
            }

            // REGRA 1: Removemos itens vazios e os que possuem sSerializavel = 'N' (Não Imprime)
            var itensValidosParaImpressao = bs_Movimentacao
                .Where(x => x.IdProduto > 0 && (string.IsNullOrEmpty(x.SSerializavel) || x.SSerializavel.ToUpper() != "N"))
                .ToList();

            if (itensValidosParaImpressao.Count == 0)
            {
                MensagemPaginaErro.MostraMensagem_Aviso("Não há produtos configurados para impressão de etiquetas nesta movimentação.", true);
                return;
            }

            // Alimenta a Grid da Modal
            dtgFracionamento.DataSource = itensValidosParaImpressao;
            dtgFracionamento.DataBind();
            updModalFracionamento.Update();

            // Abre a modal e ativa os scripts matemáticos do front-end
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_AbrirFracionamento",
                "$('#modalFracionamento').modal('show'); bindFracionamentoEvents();", true);
        }

        protected void dtgFracionamento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = (FrameWork.cls_WMS_MovimentacaoRel)e.Row.DataItem;

                TextBox txtQtdEtiquetas = (TextBox)e.Row.FindControl("txtQtdEtiquetas");
                TextBox txtQtdFracionada = (TextBox)e.Row.FindControl("txtQtdFracionada");

                decimal qtdTotal = item.NQuantidade;

                // Limpa espaços extras e joga pra maiúsculo para evitar furos de cadastro
                string unidade = (item.SUnidade ?? "").ToUpper().Trim()
                    .Replace(" ", "")
                    .Replace("Í", "I")
                    .Replace("Ú", "U");

                // Lista de siglas/nomes que SÃO tradicionalmente fracionáveis
                string[] unidadesFracionaveis = {
            "KG", "QUILOGRAMA", "QUILOGRAMAS",
            "L", "LT", "LTS", "LITRO", "LITROS",
            "M", "MT", "MTS", "METRO", "METROS",
            "M2", "METROQUADRADO",
            "M3", "METROCUBICO",
            "CM", "CENTIMETRO", "CENTIMETROS",
            "LB", "LIBRA", "LIBRAS"
        };

                // REGRA DE OURO: Verifica se tem casas decimais diferentes de ,0000
                // (qtdTotal % 1) pega o resto da divisão por 1. Se sobrar algo, é porque tem decimal (ex: 1.5 % 1 = 0.5)
                bool possuiDecimal = (qtdTotal % 1) != 0;

                // É FRACIONADO SE: A unidade estiver na lista acima OU se a quantidade possuir decimais reais
                bool isFracionavel = unidadesFracionaveis.Contains(unidade) || possuiDecimal;

                // Consequentemente, é unitário se NÃO for fracionável
                bool isUnitario = !isFracionavel;

                // Marca a linha no HTML para o Javascript saber
                e.Row.Attributes["data-unitario"] = isUnitario ? "S" : "N";

                if (isUnitario)
                {
                    // REGRA UNITÁRIA (BLOQUEADO): Ex: Qtd = 2 Caixas inteiras -> 2 Etiquetas de 1
                    int qtdInteira = (int)Math.Floor(qtdTotal);
                    txtQtdEtiquetas.Text = qtdInteira > 0 ? qtdInteira.ToString() : "1";
                    txtQtdFracionada.Text = "1,0000";

                    // TRAVA OS CAMPOS PARA NÃO PERMITIR EDIÇÃO
                    txtQtdEtiquetas.Attributes.Add("readonly", "readonly");
                    txtQtdFracionada.Attributes.Add("readonly", "readonly");

                    // Adiciona uma cor/estilo de bloqueado pro usuário sacar que não pode mexer
                    txtQtdEtiquetas.Style.Add("background-color", "#e9ecef");
                    txtQtdFracionada.Style.Add("background-color", "#e9ecef");
                    txtQtdEtiquetas.Style.Add("cursor", "not-allowed");
                    txtQtdFracionada.Style.Add("cursor", "not-allowed");
                }
                else
                {
                    // REGRA FRACIONÁVEL (LIVRE): Ex: Qtd = 2.50 Quilogramas -> 1 Etiqueta de 2.5000
                    txtQtdEtiquetas.Text = "1";
                    txtQtdFracionada.Text = qtdTotal.ToString("N4");

                    // Garante que campos fracionáveis estejam abertos para edição
                    txtQtdEtiquetas.Attributes.Remove("readonly");
                    txtQtdFracionada.Attributes.Remove("readonly");
                    txtQtdEtiquetas.Style.Remove("background-color");
                    txtQtdFracionada.Style.Remove("background-color");
                    txtQtdEtiquetas.Style.Remove("cursor");
                    txtQtdFracionada.Style.Remove("cursor");
                }
            }
        }

        protected void cmdConfirmarImpressao_Click(object sender, EventArgs e)
        {
            // 1. Vamos montar um XML poderoso para o banco ler tudo de uma vez
            StringBuilder sbXml = new StringBuilder();
            sbXml.Append("<root>");

            foreach (GridViewRow row in dtgFracionamento.Rows)
            {
                int idProduto = Convert.ToInt32(dtgFracionamento.DataKeys[row.RowIndex].Value);
                TextBox txtEtiquetas = (TextBox)row.FindControl("txtQtdEtiquetas");
                TextBox txtFracao = (TextBox)row.FindControl("txtQtdFracionada");

                int qtdEtiquetas = 1;
                int.TryParse(txtEtiquetas.Text, out qtdEtiquetas);

                decimal qtdFracionada = 0;
                // Pega o valor digitado, respeitando a vírgula brasileira no C#
                decimal.TryParse(txtFracao.Text, out qtdFracionada);

                if (qtdFracionada <= 0 || qtdEtiquetas <= 0)
                {
                    // Exibe um alerta na tela avisando o usuário
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertaQtdZero", "alert('Atenção: Existem itens com quantidade zero. Verifique os valores antes de imprimir!');", true);

                    // Opcional:
                    // MensagemPaginaErro.MostraMensagem_Erro("Atenção: Existem itens com quantidade zero. Verifique os valores antes de imprimir!");
                    // updModalFracionamento.Update();

                    return;
                }

                // O segredo de ouro: Mandar o decimal pro SQL Server com PONTO (Invariant Culture)
                string fracaoSql = qtdFracionada.ToString(System.Globalization.CultureInfo.InvariantCulture);

                sbXml.AppendFormat("<item idProduto=\"{0}\" qtdEtiquetas=\"{1}\" qtdFracionada=\"{2}\" />",
                    idProduto,
                    Math.Max(1, qtdEtiquetas), // Garante pelo menos 1 etiqueta
                    fracaoSql);
            }
            sbXml.Append("</root>");

            // 2. Manda pro Banco
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "GERAR-ETIQUETAS-LOTE");
            vParametros.Add("@idMovimentacao", txtidMovimentacao.Text);
            vParametros.Add("@sXMLItens", sbXml.ToString()); // Parâmetro novo!
            vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario().ToString());
            vParametros.Add("@idImpressora", ddlImpressora.SelectedValue);
            vParametros.Add("@idTipoEtiqueta", "2");

            try
            {
                BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);

                Session["MensagemSucesso"] = "Etiquetas fracionadas geradas com sucesso na fila de impressão!";
                Response.Redirect(urlPagina + "?id=" + txtidMovimentacao.Text);
            }
            catch (Exception ex)
            {
                MensagemPaginaErro.MostraMensagem_Erro("Erro ao gerar: " + ex.Message);
                updModalFracionamento.Update();
            }
        }

        #endregion

        #endregion

        #region | Importação de Itens

        protected void cmdImportar_Click(object sender, EventArgs e)
        {
            // Chama o método que criamos dentro do UserControl
            ImportadorItensModal.AbrirModal();
        }

        // O evento que o UserControl vai disparar e enviar a tabela genérica
        protected void ImportadorItensModal_ItensImportados(object sender, ItensImportadosEventArgs e)
        {
            bs_Movimentacao.Clear();
            if (e.DadosImportados != null && e.DadosImportados.Rows.Count > 0)
            {
                int ordemInicial = bs_Movimentacao.Count;

                foreach (DataRow row in e.DadosImportados.Rows)
                {
                    ordemInicial++;
                    FrameWork.cls_WMS_MovimentacaoRel novoItem = new FrameWork.cls_WMS_MovimentacaoRel();

                    // TRATAMENTO EXCLUSIVO PARA ITENS VINDOS DO COMEX (XML NFE)
                    if (e.TipoOrigem == "COMEX")
                    {
                        novoItem.IdProduto = Convert.ToInt32(row["idProduto"]);
                        novoItem.SCodigo = row["sCodigo"].ToString();
                        novoItem.SDscProduto = row["xProd"].ToString(); // Na proc do comex vem como xProd
                        novoItem.SUnidade = row["uCom"].ToString();

                        // Tratamento seguro para conversão de valores decimais
                        decimal quantidade = 0;
                        decimal nValorUnitario = 0;
                        decimal nPercIPI = 0;
                        decimal.TryParse(row["qCom"].ToString(), out quantidade);
                        decimal.TryParse(row["vUnCom"].ToString(), out nValorUnitario);
                        decimal.TryParse(row["pIPI"].ToString(), out nPercIPI);

                        novoItem.NQuantidade = quantidade;
                        novoItem.nValorUnitario = nValorUnitario;
                        novoItem.nPercIPI = nPercIPI;
                        novoItem.nValorTotal = Math.Round(quantidade * (nValorUnitario * (1+ (nPercIPI/100))), 2);

                        // Define o ID do Pedido/Importador para manter rastreabilidade se necessário
                        novoItem.idPedido = Convert.ToInt32(row["idPedido"]);

                        // Buscar o tipo de produto
                        novoItem.TipoProduto = GetCampoTipo(novoItem.IdProduto.ToString());

                        ddlFornecedor.SelectedValue = row["idParceiro_Dest"].ToString();
                        txtsNotaFiscal.Text = string.IsNullOrEmpty(row["sNotaFiscal"].ToString()) ? "Importação Comex" : row["sNotaFiscal"].ToString();
                        ddlFornecedor.Attributes.Add("disabled", "disabled");
                    }
                    // TRATAMENTO PARA AS DEMAIS ORIGENS (PEDIDO, LME, OPI)
                    else
                    {
                        novoItem.IdProduto = Convert.ToInt32(row["idProduto"]);
                        novoItem.SCodigo = row["sCodigo"].ToString();
                        novoItem.SDscProduto = row["sDscProduto"].ToString();
                        novoItem.SUnidade = row["sUnidade"].ToString();

                        decimal quantidade = 0;
                        decimal valorUnitario = 0;
                        decimal.TryParse(row["nQuantidade"].ToString(), out quantidade);
                        decimal.TryParse(row["nValorUnitario"].ToString(), out valorUnitario);

                        novoItem.NQuantidade = Math.Round(quantidade, 2);
                        novoItem.nValorUnitario = valorUnitario;
                        novoItem.nValorTotal = Math.Round(quantidade * valorUnitario, 2);
                        novoItem.TipoProduto = GetCampoTipo(novoItem.IdProduto.ToString());
                    }

                    // Propriedades comuns a todos os itens independentes da origem
                    novoItem.IdItem = 0;
                    novoItem.NSerie = "0";
                    novoItem.SLote = "";
                    novoItem.STipoMov = "N/A"; // Ajuste conforme a lógica de tipo de movimentação

                    // Impede duplicidade de produtos na mesma movimentação
                    var itemExistente = bs_Movimentacao.FirstOrDefault(Item => Item.IdProduto == novoItem.IdProduto);
                    if (itemExistente == null)
                    {
                        bs_Movimentacao.Add(novoItem);
                    }
                }

                // Exibe as colunas de Valores caso não estejam visíveis
                var dicAtivarValor = new[] { "Valor Unitário" }.ToDictionary(k => k, v => true);
                FiltroPesquisa.Controle_ExibicaoCampos(dicAtivarValor, "1");

                // Atualiza a Grid da tela
                dtgItens_DataBind();

                MensagemPagina.MostraMensagem_Sucesso("Itens importados e adicionados à lista com sucesso!");
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Atenção: A importação não retornou nenhum item.");
            }
        }

        #endregion

        #region | Validar Todos (Nova Função)

        public List<string> CodigosValidadosTodos
        {
            get
            {
                if (ViewState["CodigosValidadosTodos"] == null)
                {
                    ViewState["CodigosValidadosTodos"] = new List<string>();
                }
                return (List<string>)ViewState["CodigosValidadosTodos"];
            }
            set
            {
                ViewState["CodigosValidadosTodos"] = value;
            }
        }

        protected void cmdValidarTodos_Click(object sender, EventArgs e)
        {
            // Reseta os controles
            txtCodigoBarrasTodos.Text = "";
            Session["CameraAbertaTodos"] = false;
            DivBipadorTodos.Visible = false;
            cmdAbrirCameraTodos.Text = "Abrir Leitor / Câmera";
            LeitorQuaggaTodos.DesligarCamVariante();
            divCodigoManualTodos.Style["Display"] = "block";

            // Carrega a grid de validados e os contadores
            PopularGridValidadosTodos();

            updModalValidarTodos.Update();

            // Abre a modal
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalTodos",
                "$('#modalValidarTodos').modal('show'); setTimeout(function() { document.getElementById('" + txtCodigoBarrasTodos.ClientID + "').focus(); }, 500);", true);
        }

        private void PopularGridValidadosTodos()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR-ETIQUETAS-TODAS-VALIDADAS");
            vParametros.Add("@idMovimentacao", hddidMovimentacao.Value);

            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);

            if (BD.ValidarDataSet(ds))
            {
                // Pega os contadores (1º ResultSet)
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string total = ds.Tables[0].Rows[0]["Total"].ToString();
                    string validados = ds.Tables[0].Rows[0]["Validados"].ToString();
                    spanContadorTodos.Text = $"<b>{validados}</b> Validados de <b>{total}</b> ";
                }

                // Popula a Grid com a lista de quem JÁ FOI validado (2º ResultSet)
                if (ds.Tables.Count > 1)
                {
                    dtgValidadosTodos.DataSource = ds.Tables[1];
                    dtgValidadosTodos.DataBind();

                    // ADICIONE ESTE BLOCO: Guarda as etiquetas na memória
                    List<string> listValidados = new List<string>();
                    foreach (DataRow r in ds.Tables[1].Rows)
                    {
                        listValidados.Add(r["sCodigoBarras"].ToString().ToUpper());
                    }
                    CodigosValidadosTodos = listValidados;
                }
            }
        }

        protected void dtgValidadosTodos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            dtgValidadosTodos.PageIndex = e.NewPageIndex;
            PopularGridValidadosTodos();
            updModalValidarTodos.Update();
        }

        protected void cmdAbrirCameraTodos_Click(object sender, EventArgs e)
        {
            bool abrir = Session["CameraAbertaTodos"] != null ? (bool)Session["CameraAbertaTodos"] : false;
            abrir = !abrir;
            Session["CameraAbertaTodos"] = abrir;

            DivBipadorTodos.Visible = abrir;
            cmdAbrirCameraTodos.Text = abrir ? "Fechar Leitor" : "Abrir Leitor";

            if (abrir)
            {
                LeitorQuaggaTodos.AbrirCameraVariante();
                divCodigoManualTodos.Style["Display"] = "none";
            }
            else
            {
                LeitorQuaggaTodos.DesligarCamVariante();
                divCodigoManualTodos.Style["Display"] = "block";
            }

            updModalValidarTodos.Update();
        }

        protected void cmdProcessarLeituraTodos_Click(object sender, EventArgs e)
        {
            string barcodeLido = "";

            if (!string.IsNullOrEmpty(LeitorQuaggaTodos.GetCodigoBarras(txtCodigoBarrasTodos)))
            {
                barcodeLido = LeitorQuaggaTodos.GetCodigoBarras(txtCodigoBarrasTodos).Trim();
            }
            else
            {
                barcodeLido = txtCodigoBarrasTodos.Text.Trim();
            }

            txtCodigoBarrasTodos.Text = ""; // Limpa para a próxima

            // ====== NOVA TRAVA DE BIPE DUPLO ======
            if (CodigosValidadosTodos.Contains(barcodeLido.ToUpper()))
            {
                MensagemPaginaTodosModal.MostraMensagem_Aviso($"A etiqueta {barcodeLido} já foi validada!");

                // Mantém a câmera no estado correto e refoca no campo para ele continuar trabalhando
                bool isCamAberta = Session["CameraAbertaTodos"] != null && (bool)Session["CameraAbertaTodos"];
                string scriptF = "";

                if (isCamAberta)
                {
                    LeitorQuaggaTodos.AbrirCameraVariante();
                    divCodigoManualTodos.Style["Display"] = "none";
                }
                else
                {
                    scriptF += "setTimeout(function() { document.getElementById('" + txtCodigoBarrasTodos.ClientID + "').focus(); }, 200);";
                    divCodigoManualTodos.Style["Display"] = "block";
                }

                updModalValidarTodos.Update();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "keepModalTodos", scriptF, true);
                return; // Para a execução aqui! Nem chega perto do banco.
            }

            // Valida etiqueta globalmente!
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "VALIDAR-ETIQUETA");
            vParametros.Add("@idMovimentacao", hddidMovimentacao.Value);
            vParametros.Add("@sCodigoBarras", barcodeLido);
            vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario().ToString());

            string sErro;
            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);

            if (BD.ValidarDataSet(ds, out sErro))
            {
                MensagemPaginaTodosModal.MostraMensagem_Sucesso($"Etiqueta {barcodeLido} validada com sucesso!");

                // Atualiza a Grid (O item bipado agora vai aparecer no topo!)
                PopularGridValidadosTodos();

                // Checa se finalizou tudo para fechar a modal e atualizar a capa
                ValidarStatus(hddidStatus.Value, false);
            }
            else
            {
                MensagemPaginaTodosModal.MostraMensagem_Erro("Atenção: Etiqueta inválida, não pertence a esta movimentação ou erro no BD.");
            }

            // Mantém a câmera no estado correto e refoca no campo
            bool isCameraAberta = Session["CameraAbertaTodos"] != null && (bool)Session["CameraAbertaTodos"];
            string scriptFinal = "";

            if (isCameraAberta)
            {
                LeitorQuaggaTodos.AbrirCameraVariante();
                divCodigoManualTodos.Style["Display"] = "none";
            }
            else
            {
                scriptFinal += "setTimeout(function() { document.getElementById('" + txtCodigoBarrasTodos.ClientID + "').focus(); }, 200);";
                divCodigoManualTodos.Style["Display"] = "block";
            }

            updModalValidarTodos.Update();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "keepModalTodos", scriptFinal, true);
        }

        #endregion

        #region | Reimpressão de Etiquetas

        protected void cmdReimprimirTodas_Click(object sender, EventArgs e)
        {
            // Passa vazio no código de barras para afetar todas as etiquetas deste produto
            ReimprimirEtiquetas(hddIdProduto.Value, "");
        }

        protected void lnkReimprimirIndividual_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string codigoBarras = btn.CommandArgument; // Pega o código da linha específica
            ReimprimirEtiquetas(hddIdProduto.Value, codigoBarras);
            MensagemPaginaInfos.MostraMensagem_Sucesso("A Etiqueta Foi Imprimida com Sucesso!");
        }

        private void ReimprimirEtiquetas(string idProduto, string codigoBarras)
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "REIMPRIMIR_ETIQUETAS");
                vParametros.Add("@idMovimentacao", hddidMovimentacao.Value);
                vParametros.Add("@idProduto", idProduto);
                vParametros.Add("@sCodigoBarras", codigoBarras);

                string sErro;
                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);

                if (BD.ValidarDataSet(ds, out sErro))
                {
                    MensagemPaginaItensModal.MostraMensagem_Sucesso("Etiqueta(s) enviada(s) para a fila de impressão!");
                }
                else
                {
                    MensagemPaginaItensModal.MostraMensagem_Erro("Erro: " + sErro);
                }

                // Dá um refresh no painel da modal e garante que ela continue aberta
                updModalItens.Update();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "keepModalAberta", "setTimeout(function() { $('#modalItens').modal('show'); }, 200);", true);
            }
            catch (Exception ex)
            {
                MensagemPaginaItensModal.MostraMensagem_Erro("Erro ao reimprimir: " + ex.Message);
            }
        }

        #endregion

        #region | Grid Tipo 5 - Mudança de Posicionamento

        protected void cmdIncluirMudanca_Click(object sender, EventArgs e)
        {
            AtualizaMemoriaGridMudanca();
            if (ValidarCamposFiltroMudanca())
            {
                cls_WMS_MovimentacaoRel mv = new cls_WMS_MovimentacaoRel();
                string[] vidItem = FiltroPesquisaMudanca.IdItem.ToString().Split(',');
                string idItem = vidItem[0].ToString();

                try
                {
                    mv.IdItem = Convert.ToInt32(idItem);
                    mv.IdTipoMovimentacao = Convert.ToInt32(FiltroPesquisaMudanca.IdItem.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPaginaMudanca.MostraMensagem_Erro("Item não possui código cadastrado <br/>" + ex.Message);
                    return;
                }

                mv.IdProduto = Convert.ToInt32(FiltroPesquisaMudanca.IdItem);
                mv.SCodigo = FiltroPesquisaMudanca.SCodigo;
                mv.SDscProduto = FiltroPesquisaMudanca.SDscProduto;
                mv.SUnidade = FiltroPesquisaMudanca.SUnidade;
                mv.TipoProduto = GetCampoTipo(mv.IdProduto.ToString());
                mv.nValorUnitario = FiltroPesquisaMudanca.NValorProduto;
                mv.NQuantidade = FiltroPesquisaMudanca.NQuantidade;
                mv.nValorTotal = mv.nValorUnitario * mv.NQuantidade;

                // Adiciona direto na lista (Permite repetição intencionalmente para o Tipo 5)
                bs_ItemMudanca.Add(mv);

                dtgMudancaPosicao.DataSource = bs_ItemMudanca;
                dtgMudancaPosicao.DataBind();
                updDtgMudanca.Update();

                FiltroPesquisaMudanca.LimparCampos();
            }

            FiltroPesquisaMudanca.RegistrarScriptPesquisarMovimentacao();
        }

        protected void dtgMudancaPosicao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlLocalOrigem = e.Row.FindControl("ddlLocalOrigem") as DropDownList;
                DropDownList ddlPosicaoOrigem = e.Row.FindControl("ddlPosicaoOrigem") as DropDownList;
                DropDownList ddlLocalDestino = e.Row.FindControl("ddlLocalDestino") as DropDownList;
                DropDownList ddlPosicaoDestino = e.Row.FindControl("ddlPosicaoDestino") as DropDownList;

                TextBox txtQtdMudanca = e.Row.FindControl("txtQtdMudanca") as TextBox;
                LinkButton lnkExcluirMudanca = e.Row.FindControl("lnkExcluirMudanca") as LinkButton;
                LinkButton cmdValidarMudanca = e.Row.FindControl("cmdValidarMudanca") as LinkButton;
                LinkButton cmdVisualizarMudanca = e.Row.FindControl("cmdVisualizarMudanca") as LinkButton;

                var item = (cls_WMS_MovimentacaoRel)e.Row.DataItem;

                if (item != null)
                {
                    bool isConsultaDetalhe = Request["id"] != null && Request["id"] != "0";
                    bool liberaSerie = hddsLiberaSerie.Value == "S";
                    string serializa = !string.IsNullOrEmpty(item.SSerializavel) ? item.SSerializavel : "S";

                    if (ddlLocalOrigem != null)
                    {
                        FUNCOES.Popula_Combo(ddlLocalOrigem, $"sp_Manipula_tbl_Flow_Produtos_Movimentacao 'FLOW_LOCAL_FILTRADO', @idProduto={item.IdProduto}", "idLocalArmazenamento", "sDscLocalArmazenamento", false, "Sem Local de Origem", "0");

                        if (item.IdLocalArmazenamento > 0 && ddlLocalOrigem.Items.FindByValue(item.IdLocalArmazenamento.ToString()) != null)
                        {
                            ddlLocalOrigem.SelectedValue = item.IdLocalArmazenamento.ToString();
                            FUNCOES.Popula_Combo(ddlPosicaoOrigem, $"{sProcedureLocal} 'FLOW-POSICOES', @idLocalArmazenamento={ddlLocalOrigem.SelectedValue}", "idPosicao", "sCodigoLocal", false, "Sem Posição", "0");
                            ddlPosicaoOrigem.SelectedValue = item.IdPosicao.ToString();
                        }
                    }

                    // DESTINO
                    if (ddlLocalDestino != null)
                    {
                        FUNCOES.Popula_Combo(ddlLocalDestino, "sp_Manipula_tbl_Flow_Produtos_Movimentacao 'FLOW_LOCAL_ARMAZENAMENTO'", "idLocalArmazenamento", "sDscLocalArmazenamento", false, "Selecione o Destino", "0");

                        if (item.IdLocalDestino > 0 && ddlLocalDestino.Items.FindByValue(item.IdLocalDestino.ToString()) != null)
                        {
                            ddlLocalDestino.SelectedValue = item.IdLocalDestino.ToString();
                            FUNCOES.Popula_Combo(ddlPosicaoDestino, $"{sProcedureLocal} 'FLOW-POSICOES', @idLocalArmazenamento={ddlLocalDestino.SelectedValue}", "idPosicao", "sCodigoLocal", false, "Sem Posição", "0");
                            ddlPosicaoDestino.SelectedValue = item.IdPosicaoDestino.ToString();
                        }
                    }

                    // TRAVAS PÓS-SALVAMENTO & BOTÕES DE VALIDAÇÃO
                    if (isConsultaDetalhe)
                    {
                        if (txtQtdMudanca != null) txtQtdMudanca.Attributes.Add("disabled", "disabled");
                        if (ddlLocalOrigem != null) ddlLocalOrigem.Attributes.Add("disabled", "disabled");
                        if (ddlPosicaoOrigem != null) ddlPosicaoOrigem.Attributes.Add("disabled", "disabled");
                        if (ddlLocalDestino != null) ddlLocalDestino.Attributes.Add("disabled", "disabled");
                        if (ddlPosicaoDestino != null) ddlPosicaoDestino.Attributes.Add("disabled", "disabled");
                        if (lnkExcluirMudanca != null) lnkExcluirMudanca.Visible = false;

                        // Lógica de exibir o botão de Bipar
                        if (serializa == "S" && liberaSerie)
                        {
                            DataSet dsPesquisa;
                            Dictionary<string, string> vParametros = new Dictionary<string, string>();
                            vParametros.Add("@sFuncao", "CONSULTAR-ETIQUETAS-VALIDACAO");
                            vParametros.Add("@idMovimentacao", txtidMovimentacao.Text);
                            vParametros.Add("@idProduto", item.IdProduto.ToString());

                            dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);

                            int qtdValidadas = 0;
                            int totalEtiquetasGeradas = 0;

                            if (BD.ValidarDataSet(dsPesquisa))
                            {
                                totalEtiquetasGeradas = dsPesquisa.Tables[0].Rows.Count;
                                foreach (DataRow rowEtq in dsPesquisa.Tables[0].Rows)
                                    if (rowEtq["sConfirmado"].ToString() == "S") qtdValidadas++;
                            }

                            if (totalEtiquetasGeradas > 0 && qtdValidadas == totalEtiquetasGeradas)
                            {
                                if (cmdValidarMudanca != null) cmdValidarMudanca.Visible = false;
                                if (cmdVisualizarMudanca != null) cmdVisualizarMudanca.Visible = true;
                            }
                            else
                            {
                                if (cmdValidarMudanca != null) cmdValidarMudanca.Visible = true;
                                if (cmdVisualizarMudanca != null) cmdVisualizarMudanca.Visible = false;
                            }
                        }
                    }
                }
            }
        }

        protected void ddlLocalOrigem_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizaMemoriaGridMudanca();
            DropDownList ddlLocal = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddlLocal.NamingContainer;
            DropDownList ddlPosicao = (DropDownList)row.FindControl("ddlPosicaoOrigem");

            if (ddlPosicao != null)
            {
                FUNCOES.Popula_Combo(ddlPosicao, $"{sProcedureLocal} 'FLOW-POSICOES', @idLocalArmazenamento={ddlLocal.SelectedValue}", "idPosicao", "sCodigoLocal", false, "Sem Posição", "0");
            }
        }

        protected void ddlLocalDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizaMemoriaGridMudanca();
            DropDownList ddlLocal = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddlLocal.NamingContainer;
            DropDownList ddlPosicao = (DropDownList)row.FindControl("ddlPosicaoDestino");

            if (ddlPosicao != null)
            {
                FUNCOES.Popula_Combo(ddlPosicao, $"{sProcedureLocal} 'FLOW-POSICOES', @idLocalArmazenamento={ddlLocal.SelectedValue}", "idPosicao", "sCodigoLocal", false, "Sem Posição", "0");
            }
        }

        protected void dtgMudancaPosicao_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            bs_ItemMudanca.RemoveAt(e.RowIndex);
            dtgMudancaPosicao.DataSource = bs_ItemMudanca;
            dtgMudancaPosicao.DataBind();
            updDtgMudanca.Update();
        }

        private bool ValidarCamposFiltroMudanca()
        {
            StringBuilder mensagensErro = new StringBuilder();

            if (string.IsNullOrEmpty(FiltroPesquisaMudanca.SCodigo))
            {
                mensagensErro.AppendLine("• Informe um código do produto");
                txtdtMovimentacao.Focus();
            }

            if (string.IsNullOrEmpty(FiltroPesquisaMudanca.SDscProduto))
            {
                mensagensErro.AppendLine("<br/>• Informe a Descrição do Produto");
            }

            if (FiltroPesquisaMudanca.NQuantidade <= 0)
            {
                mensagensErro.AppendLine("<br/>• Informe uma Quantidade Maior que 0");
            }

            if (string.IsNullOrEmpty(FiltroPesquisaMudanca.SUnidade))
            {
                mensagensErro.AppendLine("<br/>• Informe uma unidade");
            }

            if (mensagensErro.Length > 0)
            {
                MensagemPaginaMudanca.MostraMensagem_Erro(mensagensErro.ToString(), false);
                return false;
            }

            return true;
        }

        #region | Modal de Mudança

        // Evento do botão "Validar Séries" da Grid dtgMudancaPosicao (Agora ele não aponta mais pro cmdAdicionarSerie_Click, você precisa trocar o OnClick lá no HTML pra apontar pra esse aqui!)
        protected void cmdValidarMudanca_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string[] args = btn.CommandArgument.Split('|');
            string idProduto = args[0];

            hddIdProduto.Value = idProduto;

            // --- NOVO: Puxar o Local de Origem para o Autocomplete pesquisar apenas nele ---
            var itemMemoria = bs_ItemMudanca.FirstOrDefault(x => x.IdProduto.ToString() == idProduto);
            hddIdLocalOrigemMudanca.Value = itemMemoria != null ? itemMemoria.IdLocalArmazenamento.ToString() : "0";
            // -----------------------------------------------------------------------------

            // Limpa a sujeira
            txtCodigoBarrasMudanca.Text = "";
            Session["CameraAbertaMudanca"] = false;
            DivBipadorMudanca.Visible = false;
            cmdAbrirCameraMudanca.Text = "Abrir Leitor / Câmera";
            if (LeitorQuaggaMudanca != null) LeitorQuaggaMudanca.DesligarCamVariante();
            divCodigoManualMudanca.Style["Display"] = "block";

            // Consulta o que já foi lido do chão de fábrica
            PopularGridMudancaValidadas(idProduto);

            updModalMudanca.Update();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalMud",
                    "$('#modalMudanca').modal('show'); RegistrarAutocompleteMudanca(); setTimeout(function() { document.getElementById('" + txtCodigoBarrasMudanca.ClientID + "').focus(); }, 500);", true);
        }

        private void PopularGridMudancaValidadas(string idProduto)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_ETIQUETAS_MUDANCA");
            vParametros.Add("@idMovimentacao", hddidMovimentacao.Value);
            vParametros.Add("@idProduto", idProduto);

            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);

            // Pega a quantidade limite configurada lá na Grid de trás
            var itemMemoria = bs_ItemMudanca.FirstOrDefault(x => x.IdProduto.ToString() == idProduto);
            int totalEsperado = itemMemoria != null ? (int)Math.Floor(itemMemoria.NQuantidade) : 0;
            spanTotalMudanca.Text = totalEsperado.ToString();

            if (BD.ValidarDataSet(ds))
            {
                dtgMudancaValidadas.DataSource = ds.Tables[0];
                dtgMudancaValidadas.DataBind();

                // Pega quantas etiquetas já bipamos para Mover
                int validadas = ds.Tables[0].Rows.Count;
                spanValidadasMudanca.Text = validadas.ToString();
            }
            else
            {
                // O SEGREDO AQUI: Força o bind vazio pra ASP.NET renderizar o EmptyDataTemplate!
                dtgMudancaValidadas.DataSource = new DataTable();
                dtgMudancaValidadas.DataBind();

                spanValidadasMudanca.Text = "0";
            }
        }

        protected void cmdAbrirCameraMudanca_Click(object sender, EventArgs e)
        {
            bool abrir = Session["CameraAbertaMudanca"] != null ? (bool)Session["CameraAbertaMudanca"] : false;
            abrir = !abrir;
            Session["CameraAbertaMudanca"] = abrir;

            DivBipadorMudanca.Visible = abrir;
            cmdAbrirCameraMudanca.Text = abrir ? "Fechar Leitor" : "Abrir Leitor";

            if (abrir)
            {
                LeitorQuaggaMudanca.AbrirCameraVariante();
                divCodigoManualMudanca.Style["Display"] = "none";
            }
            else
            {
                LeitorQuaggaMudanca.DesligarCamVariante();
                divCodigoManualMudanca.Style["Display"] = "block";
            }
            updModalMudanca.Update();
        }

        protected void cmdProcessarLeituraMudanca_Click(object sender, EventArgs e)
        {
            string barcodeLido = !string.IsNullOrEmpty(LeitorQuaggaMudanca.GetCodigoBarras(txtCodigoBarrasMudanca))
                ? LeitorQuaggaMudanca.GetCodigoBarras(txtCodigoBarrasMudanca).Trim()
                : txtCodigoBarrasMudanca.Text.Trim();

            txtCodigoBarrasMudanca.Text = "";

            if (string.IsNullOrEmpty(barcodeLido)) return;

            // Ver trava de limite de bipagem
            int atual = Convert.ToInt32(spanValidadasMudanca.Text);
            int limite = Convert.ToInt32(spanTotalMudanca.Text);

            if (atual >= limite)
            {
                MensagemPaginaMudancaModal.MostraMensagem_Aviso("Você já bipou a quantidade total necessária para transferir este produto!");
                updModalMudanca.Update();
                return;
            }

            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "VALIDAR_ETIQUETA_MUDANCA");
            vParametros.Add("@idMovimentacao", hddidMovimentacao.Value);
            vParametros.Add("@sCodigoBarras", barcodeLido);
            vParametros.Add("@idProduto", hddIdProduto.Value);
            vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario().ToString());

            string sErro;
            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);

            if (BD.ValidarDataSet(ds, out sErro))
            {
                MensagemPaginaMudancaModal.MostraMensagem_Sucesso($"Etiqueta {barcodeLido} bipada e preparada para mudança!");
                PopularGridMudancaValidadas(hddIdProduto.Value);

                // Verifica se terminou AGORA
                if (spanValidadasMudanca.Text == spanTotalMudanca.Text)
                {
                    ValidarStatus(hddidStatus.Value, false);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "fecharModalSucessoMudanca",
                        "setTimeout(function() { $('#modalMudanca').modal('hide'); $('.modal-backdrop').remove(); $('body').removeClass('modal-open'); $('body').css('overflow', ''); }, 1500);", true);
                    return;
                }
            }
            else
            {
                MensagemPaginaMudancaModal.MostraMensagem_Erro(sErro);
            }

            bool isCameraAberta = Session["CameraAbertaMudanca"] != null && (bool)Session["CameraAbertaMudanca"];
            string scriptFinal = isCameraAberta ? "" : "setTimeout(function() { document.getElementById('" + txtCodigoBarrasMudanca.ClientID + "').focus(); }, 200);";

            updModalMudanca.Update();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "keepModalMud", scriptFinal, true);
        }

        private void AtualizaMemoriaGridMudanca()
        {
            for (int i = 0; i < dtgMudancaPosicao.Rows.Count; i++)
            {
                if (i < bs_ItemMudanca.Count)
                {
                    GridViewRow row = dtgMudancaPosicao.Rows[i];
                    var item = bs_ItemMudanca[i];

                    TextBox txtQtdMudanca = (TextBox)row.FindControl("txtQtdMudanca");
                    DropDownList ddlLocOrig = (DropDownList)row.FindControl("ddlLocalOrigem");
                    DropDownList ddlPosOrig = (DropDownList)row.FindControl("ddlPosicaoOrigem");
                    DropDownList ddlLocDest = (DropDownList)row.FindControl("ddlLocalDestino");
                    DropDownList ddlPosDest = (DropDownList)row.FindControl("ddlPosicaoDestino");

                    if (txtQtdMudanca != null && !string.IsNullOrEmpty(txtQtdMudanca.Text))
                    {
                        // A MÁGICA AQUI: Limpa os pontos BR, troca a vírgula por ponto e força o Parse Invariante!
                        string valorLimpo = txtQtdMudanca.Text.Replace(".", "").Replace(",", ".");
                        if (decimal.TryParse(valorLimpo, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal qtdTratada))
                        {
                            item.NQuantidade = qtdTratada;
                        }
                    }

                    if (ddlLocOrig != null && !string.IsNullOrEmpty(ddlLocOrig.SelectedValue))
                        item.IdLocalArmazenamento = Convert.ToInt32(ddlLocOrig.SelectedValue);

                    if (ddlPosOrig != null && !string.IsNullOrEmpty(ddlPosOrig.SelectedValue))
                        item.IdPosicao = Convert.ToInt32(ddlPosOrig.SelectedValue);

                    if (ddlLocDest != null && !string.IsNullOrEmpty(ddlLocDest.SelectedValue))
                        item.IdLocalDestino = Convert.ToInt32(ddlLocDest.SelectedValue);

                    if (ddlPosDest != null && !string.IsNullOrEmpty(ddlPosDest.SelectedValue))
                        item.IdPosicaoDestino = Convert.ToInt32(ddlPosDest.SelectedValue);
                }
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<string> GetEtiquetasMudancaAutocomplete(string term, string idProduto, string idLocal)
        {
            List<string> resultado = new List<string>();

            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "AUTOCOMPLETE_ETIQUETAS_MUDANCA");
                vParametros.Add("@idProduto", idProduto);
                vParametros.Add("@idLocalArmazenamento", idLocal);

                vParametros.Add("@sObservacao", string.IsNullOrEmpty(term) ? "" : term.Replace("'", ""));

                using (DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros))
                {
                    if (BD.ValidarDataSet(ds))
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            string itemFormatado = string.Format("{0}|{1}",
                                row["sDescricaoVisual"].ToString(),
                                row["sCodigoBarras"].ToString()
                            );
                            resultado.Add(itemFormatado);
                        }
                    }
                }
            }
            catch { /* Log opcional */ }

            return resultado;
        }

        #endregion

        #endregion
    }

    [Serializable]
    public class ItemImportacaoXML
    {
        public string cProd { get; set; } // Código Fornecedor
        public string xProd { get; set; } // Descrição
        public string nCNPJ { get; set; }
        public string NCM { get; set; }
        public string uCom { get; set; }
        public decimal qCom { get; set; }
        public decimal vUnCom { get; set; }
        public decimal vProd { get; set; }
        public int idProdutoSistema { get; set; } // 0 se não encontrar
        public string xProdSistema { get; set; }
    }
}