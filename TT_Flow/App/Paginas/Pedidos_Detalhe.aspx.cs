using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using TT_Flow.FrameWork;
using static Permissao;
using static TT.FrameWork.BD;
using static TT.FrameWork.BD.Retorno;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Grid;
using static TT.FrameWork.Identity;
using static TT_Flow.App.Paginas.Comercial.Orcamento_Detalhe;
using Image = System.Web.UI.WebControls.Image;

namespace TT_Flow.App
{
    public partial class Pedidos_Detalhe : Page
    {
        #region | Construtores

        string sProcedure = "sp_Manipula_tbl_Flow_Pedidos";
        string sProcedure_Consulta = "sp_Consulta_tbl_Flow_Pedidos";
        string LM_sProcedure = "sp_Manipula_tbl_Flow_Pedidos_LM";
        string sProcedure_OPI = "sp_Manipula_FLow_WMS_OPI";
        string sProcedure_Faturamento = "sp_Manipula_tbl_Flow_Adm_Faturamento";
        string sProcedure_Moedas = "sp_Manipula_tbl_Flow_Adm_Moedas";

        /* Tipos de Pedidos:
         * 1 - Orçamento
         * 2 - Pedido de Vendas
         * 3 - COMEX - Importação
         * 4 - Pré-Cotação de Vendas
         * 5 - CRM
         * 6 - COMEX - Exportação
         * 7 - Pedido de Compras
         */

        int idTipo_Orcamento = 1;
        int idTipo_Pedido = 2;
        int idTipo_Importacao = 3;
        int idTipo_Cotacao = 4;
        int idTipo_CRM = 5;
        int idTipo_Exportacao = 6;
        int idTipo_PedidoCompra = 7;

        int Col_chkItem_Selecionado = 0;
        int Col_sDscCategoriaVendas = 1;
        int Col_sTipo = 2;
        int Col_idContador = 3;
        int Col_nOrdem_Edit = 4;
        int Col_nOrdem_View = 5;
        int Col_sCodigoProduto = 6;
        int Col_sDscProduto = 7;
        int Col_dtPrevisaoEntrega_View = 8;
        int Col_dtPrevisaoEntrega_Edit = 9;
        int Col_sCodigoNCM = 10;
        int Col_Unidade = 11;
        int Col_nQuantidade_Edit = 12;
        int Col_nQuantidade_View = 13;
        int Col_nICMS_Edit = 14;
        int Col_nICMS_View = 15;
        int Col_nIPI_Edit = 16;
        int Col_nIPI_View = 17;
        int Col_nValorUnitario_Edit = 18;
        int Col_nValorUnitario_View = 19;
        int Col_nDesconto_Edit = 20;
        int Col_nDesconto_View = 21;
        int Col_nValorTotal = 22;
        int Col_sDscDestino_Edit = 23;
        int Col_sDscDestino_View = 24;
        int Col_sCodigoAtoConcessorio_Edit = 25;
        int Col_sCodigoAtoConcessorio_View = 26;
        int Col_BotaoExcluir = 27;

        int Col_ValoresDatas_idContador = 0;
        int Col_ValoresDatas_PO_Edit = 10;
        int Col_ValoresDatas_PO_View = 11;
        int Col_ValoresDatas_ETD_Edit = 12;
        int Col_ValoresDatas_ETD_View = 13;
        int Col_ValoresDatas_ETA_Edit = 14;
        int Col_ValoresDatas_ETA_View = 15;
        int Col_ValoresDatas_nValorUnitario_Edit = 7;
        int Col_ValoresDatas_nValorUnitario_View = 8;
        int Col_ValoresDatas_nQuantidade_Edit = 5;
        int Col_ValoresDatas_nQuantidade_View = 6;
        int Col_ValoresDatas_Chegada = 16;

        int Col_Faturamento_Tipo = 2;
        int Col_Faturamento_Servico = 4;
        int Col_Faturamento_Empresa = 5;
        int Col_Faturamento_Valor = 6;
        int Col_Faturamento_Porcentagem = 7;
        int Col_Faturamento_Faturado = 8;

        int nTabela_Dados = 0;
        int nTabela_Itens = 1;
        int nTabela_Historico = 2;
        int nTabela_Volumes = 3;
        int nTabela_Envios = 4;
        int nTabela_Faturamento = 13;

        int nColuna_idXML = 0;
        int nColuna_idEnvioOPI = 1;
        int nColuna_sTipo = 3;
        int nColuna_sChave = 6;
        int nColuna_Status = 7;
        int nColuna_idFaturamento = 10;
        int nColuna_nNumeroCartaCorrecao = 11;
        int nColuna_Cancelamento = 12;
        int nColuna_sChaveNFe = 13;

        string idStatus_Bloqueado = "27";

        public static string metodo = "";
        public static string LM_sDscOPI = "";
        public static string idLMRecuperado = "";
        public string sMoeda = "";
        public string ValorProduto = "";
        public static int click = 0;
        public static string ValorInicial = "";
        public static string Ordem = "S";
        public string sMoedaCompra = "";
        public static int idRegistro = 0;
        public static int idLink = 0;
        public static string sEmail = "";
        public static string sLink = "";
        public static string DirecionaLink = "";
        public static string sSenha = "";
        public static string sChave = "";

        public string sSimbolo_Moeda { get => hddMoeda_Simbolo.Value; }

        public List<cls_Pedidos_Itens> Base_Pedidos_Itens
        {
            get
            {
                if (ViewState["Base_Pedidos_Itens"] == null)
                    ViewState["Base_Pedidos_Itens"] = new List<cls_Pedidos_Itens>();
                return (List<cls_Pedidos_Itens>)ViewState["Base_Pedidos_Itens"];
            }
            set => ViewState["Base_Pedidos_Itens"] = value;
        }

        public List<cls_Pedidos_Itens> Base_LM_Itens
        {
            get
            {
                if (ViewState["Base_LM_Itens"] == null)
                    ViewState["Base_LM_Itens"] = new List<cls_Pedidos_Itens>();
                return (List<cls_Pedidos_Itens>)ViewState["Base_LM_Itens"];
            }
            set => ViewState["Base_LM_Itens"] = value;
        }

        public List<cls_Pedidos_Envios> Base_Pedidos_Envios
        {
            get
            {
                if (ViewState["Base_Pedidos_Envios"] == null)
                    ViewState["Base_Pedidos_Envios"] = new List<cls_Pedidos_Envios>();
                return (List<cls_Pedidos_Envios>)ViewState["Base_Pedidos_Envios"];
            }
            set => ViewState["Base_Pedidos_Envios"] = value;
        }

        public List<cls_Pedidos_Envios> Base_Pedidos_Volumes
        {
            get
            {
                if (ViewState["Base_Pedidos_Volumes"] == null)
                    ViewState["Base_Pedidos_Volumes"] = new List<cls_Pedidos_Envios>();
                return (List<cls_Pedidos_Envios>)ViewState["Base_Pedidos_Volumes"];
            }
            set => ViewState["Base_Pedidos_Volumes"] = value;
        }

        public List<cls_Pagamento> bs_Pagamento
        {
            get
            {
                if (ViewState["bs_Pagamento"] == null)
                    ViewState["bs_Pagamento"] = new List<cls_Pagamento>();
                return (List<cls_Pagamento>)ViewState["bs_Pagamento"];
            }
            set => ViewState["bs_Pagamento"] = value;
        }

        public List<cls_Pedidos_Garantia> bs_Garantia
        {
            get
            {
                if (ViewState["bs_Garantia"] == null)
                    ViewState["bs_Garantia"] = new List<cls_Pedidos_Garantia>();
                return (List<cls_Pedidos_Garantia>)ViewState["bs_Garantia"];
            }
            set => ViewState["bs_Garantia"] = value;
        }

        public List<cls_Itens_Tabela> bs_Itens_Tabela
        {
            get
            {
                if (ViewState["bs_Itens_Tabela"] == null)
                    ViewState["bs_Itens_Tabela"] = new List<cls_Itens_Tabela>();
                return (List<cls_Itens_Tabela>)ViewState["bs_Itens_Tabela"];
            }
            set => ViewState["bs_Itens_Tabela"] = value;
        }

        public List<string> Base_Pedidos_PDF
        {
            get
            {
                if (ViewState["Base_Pedidos_PDF"] == null)
                    ViewState["Base_Pedidos_PDF"] = new List<string>();
                return (List<string>)ViewState["Base_Pedidos_PDF"];
            }
            set => ViewState["Base_Pedidos_PDF"] = value;
        }

        public List<cls_Fluxo> Base_Fluxo
        {
            get
            {
                if (ViewState["Base_Fluxo"] == null)
                    ViewState["Base_Fluxo"] = new List<cls_Fluxo>();
                return (List<cls_Fluxo>)ViewState["Base_Fluxo"];
            }
            set => ViewState["Base_Fluxo"] = value;
        }

        public List<cls_Arquivos_STSO> Base_Arquivos_STSO
        {
            get
            {
                if (ViewState["Base_Arquivos_STSO"] == null)
                    ViewState["Base_Arquivos_STSO"] = new List<cls_Arquivos_STSO>();
                return (List<cls_Arquivos_STSO>)ViewState["Base_Arquivos_STSO"];
            }
            set => ViewState["Base_Arquivos_STSO"] = value;
        }

        public List<cls_Arquivos_STSO> Base_Arquivos_Link
        {
            get
            {
                if (ViewState["Base_Arquivos_Link"] == null)
                    ViewState["Base_Arquivos_Link"] = new List<cls_Arquivos_STSO>();
                return (List<cls_Arquivos_STSO>)ViewState["Base_Arquivos_Link"];
            }
            set => ViewState["Base_Arquivos_Link"] = value;
        }

        public List<cls_Download_STSO> Base_Download_STSO
        {
            get
            {
                if (ViewState["Base_Download_STSO"] == null)
                    ViewState["Base_Download_STSO"] = new List<cls_Download_STSO>();
                return (List<cls_Download_STSO>)ViewState["Base_Download_STSO"];
            }
            set => ViewState["Base_Download_STSO"] = value;
        }

        public List<cls_Categoria> list_Perguntas_x_Opcoes
        {
            get
            {
                if (ViewState["list_Perguntas_x_Opcoes"] == null)
                    ViewState["list_Perguntas_x_Opcoes"] = new List<cls_Categoria>();
                return (List<cls_Categoria>)ViewState["list_Perguntas_x_Opcoes"];
            }
            set => ViewState["list_Perguntas_x_Opcoes"] = value;
        }

        public List<cls_Arquivos_STSO> Base_Documentos_STSO
        {
            get
            {
                if (ViewState["Base_Documentos_STSO"] == null)
                    ViewState["Base_Documentos_STSO"] = new List<cls_Arquivos_STSO>();
                return (List<cls_Arquivos_STSO>)ViewState["Base_Documentos_STSO"];
            }
            set => ViewState["Base_Documentos_STSO"] = value;
        }

        public List<cls_Pedidos_Faturamento> Base_Faturamento
        {
            get
            {
                if (ViewState["Base_Faturamento"] == null)
                    ViewState["Base_Faturamento"] = new List<cls_Pedidos_Faturamento>();
                return (List<cls_Pedidos_Faturamento>)ViewState["Base_Faturamento"];
            }
            set => ViewState["Base_Faturamento"] = value;
        }

        public DataTable dt_CheckList
        {
            get
            {
                if (ViewState["dt_CheckList"] == null)
                    ViewState["dt_CheckList"] = new DataTable();
                return (DataTable)ViewState["dt_CheckList"];
            }
            set => ViewState["dt_CheckList"] = value;
        }

        private DataSet dsSugestoes = new DataSet();

        #endregion 

        #region | Page Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(LM_cmdExportarExcel);
            ValidaPermissao(Permissao.Pedidos.Consultar, true);

            metodo = "";
            Calendario.TipoEvento = 1;
            LM_ddlLista.Enabled = true;
            chkObras.Visible = true;
            chkEngenharia.Visible = true;
            chkRetornoObras.Visible = true;

            string idCliente = "0";
            string idTipo = "2"; // Pedido
            string idRegistroTarefa = "0";

            if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
            {
                if (ddlEnvio.SelectedValue == "1" || ddlEnvio.SelectedValue == "0")
                {
                    if (Base_Pedidos_Itens.Count != 0 && Base_Pedidos_Itens[Base_Pedidos_Itens.Count - 1].nOrdem == 9999)
                    {
                        cls_Pedidos_Itens Linha = Base_Pedidos_Itens[Base_Pedidos_Itens.Count - 1];
                        Base_Pedidos_Itens.Remove(Linha);
                        gv_resultado.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM").OrderBy(x => x.nOrdem).ToList();
                        gv_resultado.DataBind();
                    }
                }

                if (ddlEnvio.SelectedValue == "2")
                {
                    if (!Base_Pedidos_Itens.Any(item => item.nOrdem == 9999))
                    {
                        cls_Pedidos_Itens Linha = new cls_Pedidos_Itens
                        {
                            sDscProduto = "Valor do Envio",
                            sCodigoProduto = "Envio",
                            nValorUnitario = Convert.ToDouble(hddValorEnvio.Value),
                            nValorTotal = Convert.ToDouble(hddValorEnvio.Value),
                            nValorReal = Convert.ToDouble(hddValorEnvioReal.Value),
                            nValorRealTotal = Convert.ToDecimal(hddValorEnvioReal.Value),
                            nResultado = -Convert.ToDecimal(hddValorEnvio.Value) + Convert.ToDecimal(hddValorEnvioReal.Value),
                            sFuncao = "Inserir_item_envio",
                            nOrdem = 9999,
                            nQuantidade = 1,
                            EnvioResultado = true,
                            Importacao_dtPO = "",
                            Importacao_dtETA = "",
                            Importacao_dtETD = ""
                        };
                        Base_Pedidos_Itens.Add(Linha);

                        gv_resultado.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM").OrderBy(x => x.nOrdem).ToList();
                        gv_resultado.DataBind();
                    }
                }
            }

            if (!IsPostBack)
            {
                txtsRazaoSocial.ReadOnly = true;

                if (Request["idc"] != null)
                {
                    idCliente = Request["idc"].ToString();
                    hddidCliente_Produto.Value = idCliente;
                }

                if (Request["idrt"] != null) idRegistroTarefa = Request["idrt"].ToString();

                if (Request["sTp"] != null) idTipo = Request["sTp"].ToString();

                hddidTipo.Value = idTipo;
                PopulaCombos();

                if (Request["id"] != null) PesquisarPedido(Request["id"].ToString(), idCliente, idRegistroTarefa, "Inicializar", idTipo);
                else PesquisarPedido("0", idCliente, "0", "Novo", idTipo);

                if (Request["sMsg"] != null) MensagemPagina.MostraMensagem_Sucesso(Request["sMsg"].ToString());

                if (Request["sArquivoSTSO"] != null) Scripts.Mantem_AbaAtiva(Page, "STSO-tab");

                if (Request["idLM"] != null) RedirecionaLM(Request["idLM"], Request["sTipoLM"]);
            }
            else
            {
                var requestTarget = Request["__EVENTTARGET"];
                var requestArgs = Request["__EVENTARGUMENT"];

                if (requestTarget == "funcao_SAIR") DirecionaPagina("/app/dashboard.aspx");
                else if (requestTarget == "funcao_SALVAR")
                {
                    metodo = "";
                    GravarPedido();
                }
                else if (requestTarget == "funcao_Editar")
                {
                    metodo = "Editar";
                    EditarPedido();
                }
                else if (requestTarget == "funcao_Duplicar") DuplicarPedido();
                else if (requestTarget == "funcao_LM_Salvar") LM_Gravar();
                else if (requestTarget == "dialog_Composicao")
                {
                    if (click < 3)
                    {
                        ComposicaoServico(hddsModalComposicao.Value, hddsDscComposicao.Value);
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_AbreModal_VincularCRM", "$('#modalComposicao_1').modal('show');", true);
                        click += 1;
                    }

                    if (cmdEditar.Visible) DataBind_dtgItens("");
                    else DataBind_dtgItens("Editar");
                }
                else if (requestTarget == "dialog_ServicoComposicao")
                {
                    if (click < 3)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_FechaModal_VincularCRM", "$('#modalComposicao_1').modal('hide');\r\n$('.modal-backdrop').remove();\r\n$('body').removeClass('modal-open');", true);
                        ComposicaoServico(hddsComposicao.Value, hddsDscComposicaoServico.Value);
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_AbreModal_VincularCRM", "$('#modalComposicao_1').modal('show');", true);
                        click += 1;
                    }

                    if (cmdEditar.Visible) DataBind_dtgItens("");
                    else DataBind_dtgItens("Editar");
                }
                else if (requestTarget == "dialog_Apagar")
                {
                    ExcluirArquivo();
                    Scripts.Mantem_AbaAtiva(Page, "STSO-tab");
                }
                else if (requestTarget == "dialog_Aprovar") AprovarDocumento();
                else if (requestTarget == "dialog_CheckArquivo")
                {
                    if (hddChecked.Value != "")
                    {
                        ExcluirArquivos(hddChecked.Value);
                        PesquisarPedido(hddidPedido.Value, "", "0", "", "");
                        Scripts.Mantem_AbaAtiva(Page, "STSO-tab");
                    }
                    else
                    {
                        MensagemPagina6.MostraMensagem_Erro("Selecione o Arquivo que Deseja Excluir!");
                        PesquisarPedido(hddidPedido.Value, "", "0", "", "");
                        Scripts.Mantem_AbaAtiva(Page, "STSO-tab");
                    }
                }
                else if (requestTarget == "dialog_AprovarArquivo")
                {
                    if (hddAprovarDoc.Value != "") AprovarDocumento();
                    else
                    {
                        MensagemPagina6.MostraMensagem_Erro("Selecione o Arquivo que Deseja Aprovar!");
                        PesquisarPedido(hddidPedido.Value, "", "0", "", "");
                        Scripts.Mantem_AbaAtiva(Page, "STSO-tab");
                    }
                }
                else if (requestTarget == "dialog_DonwloadArquivo") cmdGerarLinkSTSO_Click(hddsidArquivo.Value);
                else if (requestTarget == "dialog_DownloadSTSO")
                {
                    if (hddDownloadSTSO.Value != "") cmdDownloadSTSO_Click(hddDownloadSTSO.Value);
                }
                else if (requestTarget == "dialog_EnviarEmail")
                {
                    if (hddidLink.Value != "") cmdEnviarEmail_Click();

                    Scripts.Mantem_AbaAtiva(Page, "STSO-tab");
                }

                decimal cambio = 0, pago = 0;
                if (txtCambio.Text != "") cambio = decimal.Parse(txtCambio.Text);
                if (txtPago.Text != "") pago = decimal.Parse(txtPago.Text);

                txtPagar.Text = (pago - cambio).ToString();

                if (sClienteFinalswt.Recuperar() == "S") div_idClienteFinal.Attributes["class"] = "col-lg-10";
                else div_idClienteFinal.Attributes["class"] = "col-lg-10 visible";
            }

            if (LM_chkEngenharia.Checked) LM_cmdEditaLM.Visible = false;

            ValorInicial = ddlsTipoCompra.SelectedValue;
            AtualizarSimboloMoeda(hddMoeda_Simbolo.Value);

            Scripts.Aplica_TooltipPersonalizado(Page, "tooltip");
            RegistraScript("");
        }

        protected void PesquisarPedido(string idPedido, string idCliente, string idRegistroTarefa, string sMetodoChamada, string idTipo)
        {
            pnImportarItens.Visible = false;
            LM_cmdSalvaLM.Visible = false;
            DIV_Departamento.Visible = false;
            DIV_STATUS_ATUAL.Visible = false;
            div_PrevisaoEntrega.Visible = false;
            div_SelecaoItens.Visible = true;
            PainelAtualizacao.Visible = false;
            cmdGravarPedido.Visible = false;
            cmdPedido_Vinculado.Visible = false;
            ValoresDatas_cmdGravarPedido.Visible = false;
            cmdEditar.Visible = false;
            ValoresDatas_cmdEditar.Visible = false;
            cmdDuplicar.Visible = false;
            ValoresDatas_cmdDuplicar.Visible = false;
            aba_Arquivos.Visible = false;
            aba_Tarefas.Visible = false;
            aba_Historico.Visible = false;
            aba_ArquivoMorto.Visible = false;
            aba_Acoes.Visible = false;
            aba_LM.Visible = false;
            DIV_nVlrProdutos.Visible = false;
            DIV_nVlrServicos.Visible = false;
            DIV_nVlrTotal.Visible = false;
            DIV_Comissao.Visible = false;
            div_ComissaoValor.Visible = false;
            LM_cmdRecuperarSugestoes.Visible = false;
            ddlsEnderecoEntrega.Visible = true;
            txtsEnderecoEntrega.Visible = false;
            txtsEnderecoEntrega.ReadOnly = true;
            Contato_Exportador.Visible = false;
            Contato_Despachante.Visible = false;
            Contato_Empresa.Visible = false;
            Contato_Importador.Visible = false;
            txtCambio.ReadOnly = true;
            Div_CadastroPagamento.Visible = false;
            DIV_Kanban.Visible = false;
            dtgPagamento.Columns[6].Visible = false;
            PC.Visible = false;
            PCI.Visible = false;
            div_Moeda.Visible = false;
            div_CustoAduaneiro.Visible = false;
            div_CustoDespachante.Visible = false;
            div25.Visible = false;
            aba_STSO.Visible = false;
            aba_ART.Visible = false;
            div_nVlrFrete.Visible = false;
            div_nVlrImpostos.Visible = false;
            div_NavegaPedidos.Visible = false;
            div_btnExcluirSelecionados.Visible = false;

            string currentClass = div_ComissaoPaga.Attributes["class"];
            div_ComissaoPaga.Attributes["class"] += " visible";

            string sErro = "";
            string nNumeroPedido = "";
            string nNumeroPedidoCompra = "";

            try
            {
                ConfigurarCampos();
                LimpaCampos();
                Base_Pedidos_Itens.Clear();
                Base_Pedidos_Envios.Clear();
                Base_Pedidos_Volumes.Clear();
                Base_Arquivos_STSO.Clear();
                Base_Documentos_STSO.Clear();
                Base_Download_STSO.Clear();
                Base_Faturamento.Clear();

                if (idPedido != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTA PEDIDO" },
                        { "@idPedido", idPedido },
                        { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                    };
                    DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

                    if (ValidarDataSet(dsPesquisa, out sErro))
                    {
                        nNumeroPedido = DATASET(dsPesquisa, nTabela_Dados, "nNumeroPedido");
                        hddnNumeroPedido.Value = nNumeroPedido;
                        nNumeroPedidoCompra = DATASET(dsPesquisa, nTabela_Dados, "sPedidoCompras");
                        hddidTipo.Value = DATASET(dsPesquisa, nTabela_Dados, "idTipo");
                        hddididStatus.Value = DATASET(dsPesquisa, nTabela_Dados, "idStatus");
                        PopulaCombos();
                        ConfigurarCampos();

                        hddidPedido.Value = DATASET(dsPesquisa, nTabela_Dados, "idPedido");
                        DIV_Conceitual.Visible = false;

                        hddsPossuiProdutos.Value = DATASET(dsPesquisa, "sPossuiProduto");
                        hddsPossuiServicos.Value = DATASET(dsPesquisa, "sPossuiServico");

                        if (hddidTipo.Value != "2") cmdGerarOPI.Visible = false;
                        else ValidaBotão_OPI(idPedido, DATASET(dsPesquisa, "sPossuiProduto").Equals("S"));

                        if (hddidTipo.Value == "7")
                        {
                            hddFornecedor.Value = DATASET(dsPesquisa, nTabela_Dados, "idCliente");

                            if (int.Parse(idPedido) > 2443 || int.Parse(idPedido) == 0)
                                Popula_Combo(ddlCondPagamento, "sp_Select 'FLOW_CondicaoDePagamento', @idFiltro= 1, @idPesquisa=" + hddFornecedor.Value, "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione a Condição de Pagamento", "0");
                            else
                                Popula_Combo(ddlCondPagamento, "sp_Select 'FLOW_CondicaoDePagamento', @idPesquisa=" + hddFornecedor.Value, "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione a Condição de Pagamento", "0");

                            DIV_Conceitual.Visible = true;
                            ddlFrete.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idFrete");
                        }
                        else if (hddidTipo.Value == "2")
                        {
                            if (Request.GetValue("sSession") == "S" && !string.IsNullOrEmpty(Session.GetValue("Dashboard")))
                            {
                                div_NavegaPedidos.Visible = true;
                                cmdPedidoAnterior.Visible = false;
                                cmdProximoPedido.Visible = false;

                                try
                                {
                                    string dashboard = Session["Dashboard"]?.ToString().Trim();

                                    List<int> ids = dashboard.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).OrderBy(i => i).ToList();
                                    int.TryParse(hddidPedido.Value, out int id);
                                    int index = ids.IndexOf(id);

                                    if (index > 0) { cmdPedidoAnterior.Visible = true; cmdPedidoAnterior.NavigateUrl = $"/App/Paginas/Pedidos_Detalhe.aspx?id={ids[index - 1]}&sTp=2&sSession=S"; }
                                    if (index >= 0 && index < ids.Count - 1) { cmdProximoPedido.Visible = true; cmdProximoPedido.NavigateUrl = $"/App/Paginas/Pedidos_Detalhe.aspx?id={ids[index + 1]}&sTp=2&sSession=S"; }
                                }
                                catch { }
                            }

                            hddidCliente.Value = DATASET(dsPesquisa, nTabela_Dados, "idCliente");

                            div_nVlrFrete.Visible = false;
                            Div_FreteVendas.Visible = true;
                            decimal.TryParse(DATASET(dsPesquisa, "nFretePrevisto"), out decimal nFrete);
                            txtnFreteVendas.Text = Math.Round(nFrete, 2).ToString();
                            txtnFreteVendas.ReadOnly = true;

                            txtFrete.Text = Math.Round(nFrete, 2).ToString();
                            txtFrete.ReadOnly = true;

                            DIV_nVlrProdutos.Attributes["class"] = "col-lg-3";
                            DIV_nVlrServicos.Attributes["class"] = "col-lg-3 form-group";

                            if (int.Parse(idPedido) > 2443 || int.Parse(idPedido) == 0)
                            {
                                Popula_Combo(ddlCondPagamento, $"sp_Select 'FLOW_CondicaoDePagamento', @idFiltro= 2, @idPesquisa={hddidCliente.Value}, @idPais={hddidPedido.Value}", "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione a Condição de Pagamento", "0");
                                ddlCondPagamento.Items.Add(new ListItem("A vista", "1"));
                            }
                            else Popula_Combo(ddlCondPagamento, $"sp_Select 'FLOW_CondicaoDePagamento', @idPais={hddidPedido.Value}", "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione a Condição de Pagamento", "0");

                            MoedaFrete.InnerText = DATASET(dsPesquisa, nTabela_Dados, "sMoedaVenda");
                            MoedaCompra3.InnerText = DATASET(dsPesquisa, nTabela_Dados, "sMoedaVenda");
                            MoedaCompra2.InnerText = DATASET(dsPesquisa, nTabela_Dados, "sMoedaVenda");
                            MoedaCompra1.InnerText = DATASET(dsPesquisa, nTabela_Dados, "sMoedaVenda");
                            MoedaCompra.InnerText = DATASET(dsPesquisa, nTabela_Dados, "sMoedaVenda");
                            MoedaFreteVendas.InnerText = DATASET(dsPesquisa, nTabela_Dados, "sMoedaVenda");
                        }

                        txtsReferencia.Text = DATASET(dsPesquisa, nTabela_Dados, "sReferencia");
                        txtdtPedido.Text = DATASET(dsPesquisa, nTabela_Dados, "dtPedido");
                        txtsRazaoSocial.Text = DATASET(dsPesquisa, nTabela_Dados, "sRazaoSocial");
                        txtsObservacao.Text = DATASET(dsPesquisa, nTabela_Dados, "sObservacao");
                        txtdtEstimativaEntrega.Text = DATASET(dsPesquisa, nTabela_Dados, "dtEstimativaEntrega");
                        lbldtPrevisaoEntrega.Text = DATASET(dsPesquisa, nTabela_Dados, "dtPrevisaoEntrega");
                        ddlFluxoPedido.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idFluxo");
                        ddlidInstalador.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idInstalador");

                        if (ddlVendedor.Items.FindByValue(DATASET(dsPesquisa, nTabela_Dados, "idVendedor")) != null) ddlVendedor.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idVendedor");
                        else ddlVendedor.SelectedValue = "0";

                        ddlCondPagamento.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idCondicaoPagamento");
                        txtsDepartamentoAtual.Text = DATASET(dsPesquisa, nTabela_Dados, "sDepartamentoAtual");
                        txtsDscStatus.Text = DATASET(dsPesquisa, nTabela_Dados, "sDscStatus");
                        txtsEnderecoEntrega.Text = DATASET(dsPesquisa, nTabela_Dados, "sEnderecoEntrega");

                        if (hddidTipo.Value == "2")
                        {
                            Popula_Combo(ddlsEnderecoEntrega, string.Format("sp_Manipula_tbl_Flow_Clientes 'CONSULTAR_ENDERECO', {0}", hddidCliente.Value), "idEndereco", "sEnderecoCompleto", false, "Selecione um Endereço de Entrega", "0");
                            ddlsEnderecoEntrega.SelectedValue = DATASET(dsPesquisa, "idEnderecoEntrega").Equals("0") && txtsEnderecoEntrega.Text.ToLower().Trim().Equals("coleta") ? "-1" : DATASET(dsPesquisa, "idEnderecoEntrega");
                            hddAltera_Endereco.Value = DATASET(dsPesquisa, "sAltera_Endereco");
                            txtnControleTT.Text = DATASET(dsPesquisa, "nControleTT");

                            txtnTempoContrato.ReadOnly = true;
                            ddlTipoPeriodo.Attributes.Add("disabled", "disabled");

                            hddidPedido_Vinculado.Value = DATASET(dsPesquisa, "idPedido_Vinculado");
                            cmdPedido_Vinculado.Visible = !hddidPedido_Vinculado.Value.Equals("0");
                            cmdPedido_Vinculado.NavigateUrl = $"/App/Paginas/Comercial/Orcamento_Detalhe.aspx?id={hddidPedido_Vinculado.Value}";

                            string idTipoMoeda = DATASET(dsPesquisa, nTabela_Dados, "idTipoMoeda");
                            if (ddlsMoeda.Items.FindByValue(idTipoMoeda) != null)
                                ddlsMoeda.SelectedValue = idTipoMoeda;

                            decimal.TryParse(DATASET(dsPesquisa, nTabela_Dados, "nCambio"), out decimal nCambioPedido);
                            txtCambioMoeda.Text = nCambioPedido.ToString("N4");

                            decimal.TryParse(DATASET(dsPesquisa, nTabela_Dados, "nCusto_Aduaneiro"), out decimal nCustoAduaneiro);
                            txtCustoAduaneiro.Text = nCustoAduaneiro.ToString("N2");

                            decimal.TryParse(DATASET(dsPesquisa, nTabela_Dados, "nCusto_Despachante"), out decimal nCustoDespachante);
                            txtCustoDespachante.Text = nCustoDespachante.ToString("N2");

                            AplicarLayoutMoeda(DATASET(dsPesquisa, nTabela_Dados, "sCliente_Nacional") == "N", sMetodoChamada == "Editar");
                        }

                        if (hddidTipo.Value == "7") txtsPedidoCliente.Text = DATASET(dsPesquisa, nTabela_Dados, "sPedidoCompras");
                        else txtsPedidoCliente.Text = DATASET(dsPesquisa, nTabela_Dados, "sPedidoCliente");

                        ddlTipoEnvio.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idTipoEnvio");
                        txtnVlrProdutos.Text = string.Format("{0:N2}", Convert.ToDouble(DATASET(dsPesquisa, nTabela_Dados, "nVlrProdutos")));
                        txtnVlrServicos.Text = string.Format("{0:N2}", Convert.ToDouble(DATASET(dsPesquisa, nTabela_Dados, "nVlrServicos")));
                        CalcularValorTotal();
                        ddlidTipoFaturamento.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idTipoFaturamento");
                        ddlidEmpresa.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idEmpresa");
                        ddlidParceiro_Comissionador.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idParceiro_Comissionador");
                        ComissaoPaga.Definir(DATASET(dsPesquisa, nTabela_Dados, "sComissaoPaga"), "Paga", "");
                        txtnVlrComissao.Text = DATASET(dsPesquisa, nTabela_Dados, "nVlrComissao");
                        txtnInlandF.Text = DATASET(dsPesquisa, nTabela_Dados, "nInlandF");
                        txtnHandling.Text = DATASET(dsPesquisa, nTabela_Dados, "nHandling");
                        txtnConsular.Text = DATASET(dsPesquisa, nTabela_Dados, "nConsular");
                        txtnOcean_Air.Text = DATASET(dsPesquisa, nTabela_Dados, "nOcean_Air");
                        txtnInsurance.Text = DATASET(dsPesquisa, nTabela_Dados, "nInsurance");
                        txtnOther_Charges.Text = DATASET(dsPesquisa, nTabela_Dados, "nOther_Charges");
                        txtPago.Text = DATASET(dsPesquisa, nTabela_Dados, "nValorPago");
                        txtPagar.Text = DATASET(dsPesquisa, nTabela_Dados, "nSaldo");
                        txtCambio.Text = DATASET(dsPesquisa, nTabela_Dados, "nValorOriginal");

                        if (hddidTipo.Value == "2")
                        {
                            if (ValidaPermissao(Permissao.Pedidos.Visualizar_Aba_STSO))
                            {
                                if (DATASET(dsPesquisa, nTabela_Dados, "sSTSO") == "S")
                                {
                                    if (DATASET(dsPesquisa, nTabela_Dados, "statusTarefa") != "5") aba_STSO.Visible = true;

                                    if (metodo != "Editar")
                                    {
                                        Image img = new Image();

                                        for (int i = 0; i < dsPesquisa.Tables[11].Rows.Count; i++)
                                        {
                                            img.ImageUrl = "data:image/jpeg;base64," + Convert.ToBase64String((byte[])dsPesquisa.Tables[11].Rows[i]["vbArquivo"]);
                                            img.Width = 70;
                                            img.Height = 70;
                                            img.Style["position"] = "absolute";
                                            img.Style["right"] = "45px";
                                            img.Style["top"] = "31%";
                                            img.Style["transform"] = "translateY(-50%)";

                                            caixaTitulo.Controls.Add(img);
                                        }
                                    }
                                }
                                else aba_STSO.Visible = false;
                            }

                            sLink = DATASET(dsPesquisa, nTabela_Dados, "sLink");

                            if (ValidaPermissao(Permissao.Pedidos.Visualizar_Aba_ART))
                            {
                                if (DATASET(dsPesquisa, nTabela_Dados, "sART") == "S")
                                {
                                    if (DATASET(dsPesquisa, nTabela_Dados, "statusTarefaART") != "5") aba_ART.Visible = true;

                                    DIV_PedidoClienteFinal.Visible = true;

                                    if (hddidTipo.Value == "2")
                                    {
                                        Div_ContratoVendas.Visible = true;
                                        txtnTempoContrato.Text = DATASET(dsPesquisa, nTabela_Dados, "nDiasContrato");
                                        ddlTipoPeriodo.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "sTipoPeriodoContrato");

                                        txtnTempoContratoART.Text = txtnTempoContrato.Text;
                                        spPeriodo.InnerText = ddlTipoPeriodo.SelectedItem.Text;
                                    }


                                }
                                else
                                {
                                    aba_ART.Visible = false;
                                    DIV_PedidoClienteFinal.Visible = false;
                                }
                            }

                            sClienteFinalswt.Definir(DATASET(dsPesquisa, nTabela_Dados, "sClienteFinal"), "Tem cliente Final ?", "");

                            if (sClienteFinalswt.Recuperar() == "S")
                            {
                                div_idClienteFinal.Attributes["class"] = "col-lg-10";
                                ddlidClienteFinal.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idClienteFinal");
                            }
                            else div_idClienteFinal.Attributes["class"] = "col-lg-10 visible";
                        }

                        // Importação
                        if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
                        {
                            ddlidModal.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idModal");
                            ddlidPais.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "Importacao_idPais");
                            ddlImportacao_idDespachante.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "Importacao_idDespachante");
                            ddlImportacao_idDespacho.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "Importacao_idDespacho");
                            ddlImportacao_idFornecedor.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "Importacao_idFornecedor");
                            ddlImportador.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "Importacao_idImportador");
                            ddlTermosPagamento.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "Importacao_idCondicaoDePagamento");
                            txtnVlrServicos.Text = string.Format("{0:N2}", Convert.ToDouble(DATASET(dsPesquisa, nTabela_Dados, "nTotalEnvio")));
                            CalcularValorTotal();
                            ddlEnvio.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idEnvioPago");
                            ddlsMoeda.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idTipoMoeda");

                            Popula_Combo(ddlContato_Despachante, string.Format("sp_Select 'Flow_Contato_Importacao', @idPesquisa ={0}", ddlImportacao_idDespachante.SelectedValue), "idContato", "sNome", false, "Selecione o Contato do Despachante", "0");
                            Popula_Combo(ddlContato_Exportador, string.Format("sp_Select 'Flow_Contato_Importacao', @idPesquisa={0}", ddlImportacao_idFornecedor.SelectedValue), "idContato", "sNome", false, "Selecione o Contato do Exportador", "0");
                            Popula_Combo(ddlContato_Importador, string.Format("sp_Select 'Flow_Contato_Importacao', @idPesquisa={0}", ddlImportador.SelectedValue), "idContato", "sNome", false, "Selecione o Contato do Importador", "0");
                            Popula_Combo(ddlContato_Empresa, string.Format("sp_Select 'Flow_Contato_Empresa', @idPesquisa={0}", ddlidEmpresa.SelectedValue), "idContato", "sNome", false, "Selecione o Contato da Empresa", "0");

                            try
                            {
                                ddlContato_Exportador.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idContato_Exportador");
                                ddlContato_Empresa.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idContato_Empresa");
                                ddlContato_Despachante.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idContato_Despachante");
                                ddlContato_Importador.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idContato_Importador");
                            }
                            catch { }

                            TextInvoiceNumber.Text = DATASET(dsPesquisa, nTabela_Dados, "sInvoiceNumber");
                            txtnOrderNumber.Text = DATASET(dsPesquisa, nTabela_Dados, "nOrderNumber");
                            txtsZonaEnvio.Text = DATASET(dsPesquisa, nTabela_Dados, "sZonaEnvio");
                            hddValorEnvio.Value = DATASET(dsPesquisa, nTabela_Dados, "nValorEnvio");
                            hddValorEnvioReal.Value = DATASET(dsPesquisa, nTabela_Dados, "nValorEnvioReal");
                            txtnTotalEnvio.Text = DATASET(dsPesquisa, nTabela_Dados, "nTotalEnvio");
                            sMoeda = DATASET(dsPesquisa, nTabela_Dados, "sMoedaOrigem");
                            sMoedaOrigem0.InnerText = sMoeda;
                            sMoedaOrigem1.InnerText = sMoeda;
                            sMoedaOrigem2.InnerText = sMoeda;
                            sMoedaOrigem3.InnerText = sMoeda;
                            sMoedaOrigem14.InnerText = sMoeda;
                            sMoedaOrigem15.InnerText = sMoeda;
                            sMoedaOrigem16.InnerText = sMoeda;
                            sMoedaOrigem24.InnerText = sMoeda;
                            sMoedaOrigem25.InnerText = sMoeda;
                            hddMoeda.Value = sMoeda;

                            string sDscTipoStatus = DATASET(dsPesquisa, 0, "sStatus");
                            lblsDscTipoStatus.Text = sDscTipoStatus;
                            lblsDscTipoStatus.CssClass = string.Format("label label-{0}", DATASET(dsPesquisa, 0, "sCorCambio"));

                            Contato_Exportador.Visible = true;
                            Contato_Despachante.Visible = true;
                            Contato_Empresa.Visible = true;
                            Contato_Importador.Visible = true;
                        }

                        if (hddidTipo.Value == "7")
                        {
                            hddEndereco.Value = DATASET(dsPesquisa, nTabela_Dados, "idEnderecoEntrega");
                            Popula_Combo(ddlsEnderecoEntrega, string.Format("sp_Manipula_tbl_Flow_Clientes 'CONSULTAR_ENDERECO_COMPRAS', {0}", ddlidEmpresa.SelectedValue), "idEndereco", "sEnderecoCompleto", false);
                            ddlsEnderecoEntrega.SelectedValue = hddEndereco.Value;
                            txtsEnderecoEntrega.Text = DATASET(dsPesquisa, nTabela_Dados, "sEnderecoEntregaCompras");
                            ddlsTipoCompra.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "sTipoCompra");
                            txtnDesconto.Text = DATASET(dsPesquisa, nTabela_Dados, "nDesconto");
                            txtnVlrDesconto.Text = DATASET(dsPesquisa, nTabela_Dados, "nVlrDesconto");

                            PopulaCombosConceitos();
                            hddidConceito.Value = DATASET(dsPesquisa, nTabela_Dados, "idConceito");
                            hddidGrupoPatrimonio.Value = DATASET(dsPesquisa, nTabela_Dados, "idGrupoPatrimonio");
                            hddidCentroCusto.Value = DATASET(dsPesquisa, nTabela_Dados, "idCentroCusto");

                            ddlConceito.SelectedValue = string.IsNullOrEmpty(hddidConceito.Value) ? "0" : hddidConceito.Value;
                            ddlCentroCusto.SelectedValue = string.IsNullOrEmpty(hddidCentroCusto.Value) ? "0" : hddidCentroCusto.Value;
                            ddlGrupoPatrimonio.SelectedValue = string.IsNullOrEmpty(hddidGrupoPatrimonio.Value) ? "0" : hddidGrupoPatrimonio.Value;

                            var idGrupoPatrimonio = DATASET(dsPesquisa, "idGrupoPatrimonio");
                            if (idGrupoPatrimonio != "0" && !string.IsNullOrEmpty(idGrupoPatrimonio))
                            {
                                Popula_Combo(ddlGrupoPatrimonio, "sp_Select 'Flow_Patrimonio_Grupo'", "idPatrimonioGrupo", "sDscPatrimonio", false, "Selecione um Grupo de Patrimônio.", "0");
                                ddlGrupoPatrimonio.SelectedValue = idGrupoPatrimonio;
                                divGrupoPatrimonio.Visible = true;
                            }
                            else divGrupoPatrimonio.Visible = false;

                            ValorInicial = ddlsTipoCompra.SelectedValue;
                            Popula_Combo(ddlFornecedor, "sp_Select 'Flow_Parceiros_Fornecedores', @sTipo =" + ddlsTipoCompra.SelectedValue, "idCliente", "Razao_CNPJ", false, "Selecione o Fornecedor", "0");
                            ddlFornecedor.SelectedValue = hddFornecedor.Value;
                            ddlsMoeda.SelectedValue = DATASET(dsPesquisa, nTabela_Dados, "idTipoMoeda");
                            sMoedaCompra = DATASET(dsPesquisa, nTabela_Dados, "sSimboloMoeda");
                            MoedaCompra.InnerText = sMoedaCompra;
                            MoedaCompra1.InnerText = sMoedaCompra;
                            MoedaCompra2.InnerText = sMoedaCompra;
                            MoedaCompra3.InnerText = sMoedaCompra;
                            MoedaCompra4.InnerText = sMoedaCompra;

                            if (sMetodoChamada == "Editar" && ddlFornecedor.SelectedValue != "0" && ddlFornecedor.SelectedValue != "")
                                div_btnItens.Visible = true;

                            if (ddlsTipoCompra.SelectedValue == "N")
                            {
                                PC.Visible = true;
                                div_Moeda.Visible = false;
                                div_txtnVlrDesconto.Attributes["class"] = "";
                            }
                            else if (ddlsTipoCompra.SelectedValue == "I")
                            {
                                PCI.Visible = true;
                                PI.Visible = true;

                                div_Moeda.Visible = false;
                                div_MoedaConsulta.Visible = true;
                                txtMoedaConsulta.Text = ddlsMoeda.SelectedItem.Text;

                                DIV_txtnVlrTotal.Attributes["class"] = "input-group";
                                DIV_Produtos.Attributes["class"] = "input-group";
                                DIV_Servicos.Attributes["class"] = "input-group";
                                MoedaCompra.Visible = true;
                                MoedaCompra1.Visible = true;
                                MoedaCompra2.Visible = true;
                                MoedaCompra4.Visible = true;
                            }

                            if (ddlsTipoCompra.SelectedValue == "N" || ddlsTipoCompra.SelectedValue == "0")
                            {
                                DIV_txtnVlrTotal.Attributes["class"] = "form-group";
                                DIV_Produtos.Attributes["class"] = "form-group";
                                DIV_Servicos.Attributes["class"] = "form-group";
                                MoedaCompra.Visible = false;
                                MoedaCompra1.Visible = false;
                                MoedaCompra2.Visible = false;
                                MoedaCompra4.Visible = false;
                            }
                        }

                        caixaTitulo.Attributes.CssStyle.Add("class", string.Format("bg-{0}", DATASET(dsPesquisa, nTabela_Dados, "sCor")));
                        txtsDscStatus.Attributes.CssStyle.Add("class", string.Format("bg-{0}", DATASET(dsPesquisa, nTabela_Dados, "sCor")));

                        PainelAtualizacao.Atualizar(DATASET(dsPesquisa, 0, "dtUltimaAtualizacao"), DATASET(dsPesquisa, nTabela_Dados, "sdscUsuarioAtualizacao"));

                        if (sMetodoChamada == "Duplicar") hddDupliacado.Value = "Duplicado ID: " + hddidPedido.Value;

                        if (hddidTipo.Value == "2")
                        {
                            lblTituloPagina.Text = string.Format("Pedido N.º {0} - Referência: {1} - Cliente: {2}", nNumeroPedido.ToString().PadLeft(6, '0'), DATASET(dsPesquisa, nTabela_Dados, "sReferencia"), DATASET(dsPesquisa, nTabela_Dados, "sRazaoSocial"));
                            BreadCrumb.TitulodaPagina = string.Format("Pedido n. {0} - Referência: {1} ", nNumeroPedido.ToString().PadLeft(6, '0'), DATASET(dsPesquisa, nTabela_Dados, "sReferencia"));

                            if (sMetodoChamada == "Duplicar") BreadCrumb.TitulodaPagina = string.Format("Duplicar Pedido");

                            aba_depara.Visible = ValidaPermissao(Permissao.Pedidos.Visualizar_Aba_De_Para);
                        }
                        else if (hddidTipo.Value == "3")
                        {
                            lblTituloPagina.Text = string.Format("Importação N.º {0} - Referência: {1}", nNumeroPedido.ToString().PadLeft(6, '0'), DATASET(dsPesquisa, nTabela_Dados, "sReferencia"));
                            BreadCrumb.TitulodaPagina = lblTituloPagina.Text;

                            if (sMetodoChamada == "Duplicar") BreadCrumb.TitulodaPagina = string.Format("Duplicar Importação");

                            aba_Resultado.Visible = ValidaPermissao(Permissao.Pedidos.Visualizar_Aba_Resultado);
                        }
                        else if (hddidTipo.Value == "6")
                        {
                            lblTituloPagina.Text = string.Format("Exportação N.º {0} - Referência: {1}", nNumeroPedido.ToString().PadLeft(6, '0'), DATASET(dsPesquisa, nTabela_Dados, "sReferencia"));
                            BreadCrumb.TitulodaPagina = lblTituloPagina.Text;
                            if (sMetodoChamada == "Duplicar") BreadCrumb.TitulodaPagina = string.Format("Duplicar Exportação");

                            aba_Resultado.Visible = ValidaPermissao(Permissao.Pedidos.Visualizar_Aba_Resultado);
                        }
                        else if (hddidTipo.Value == "7")
                        {
                            lblTituloPagina.Text = string.Format("Pedido de Compra N.º {0} - Referência: {1} - Fornecedor: {2}", nNumeroPedidoCompra.ToString().PadLeft(6, '0'), DATASET(dsPesquisa, nTabela_Dados, "sReferencia"), DATASET(dsPesquisa, nTabela_Dados, "sRazaoSocial"));
                            BreadCrumb.TitulodaPagina = string.Format("Pedido de Compra N.° {0} - Referência: {1} ", nNumeroPedidoCompra.ToString().PadLeft(6, '0'), DATASET(dsPesquisa, nTabela_Dados, "sReferencia"));

                            if (sMetodoChamada == "Duplicar") BreadCrumb.TitulodaPagina = string.Format("Duplicar Pedido de Compras");
                        }

                        lbldtPrevisaoEntrega.CssClass = string.Format("label label-{0}", DATASET(dsPesquisa, nTabela_Dados, "sCor"));

                        ddlsEnderecoEntrega.Visible = false;
                        txtsEnderecoEntrega.Visible = true;
                        txtsEnderecoEntrega.ReadOnly = true;
                        TextInvoiceNumber.ReadOnly = true;
                        TextInvoiceNumber.ReadOnly = true;
                        txtnOrderNumber.ReadOnly = true;
                        txtsZonaEnvio.ReadOnly = true;

                        sMoeda = DATASET(dsPesquisa, nTabela_Dados, div_Moeda.Visible ? "sSimboloMoeda" : "sMoedaVenda");
                        hddMoeda_Simbolo.Value = sMoeda;

                        Popular_GVItens(dsPesquisa, sMetodoChamada);
                        StatusDosCampos(true);
                        gv_resultado.DataBind();
                        Popular_Aba_Historico(dsPesquisa);
                        Popular_Aba_Arquivos(idPedido);
                        AtualizaCalendario();

                        if (new[] { "2" }.Contains(hddidTipo.Value))
                            Popular_Aba_ArquivoMorto(idPedido);

                        if (hddidTipo.Value == "2")
                        {
                            AtualizarSimboloMoeda(sMoeda);

                            Popular_Aba_OPI(idPedido, "");
                            Popular_Aba_LM();
                            PopularSTSO(dsPesquisa);
                            PopularDownloadArquivos(dsPesquisa);
                            Popular_HistoricoSTSO(dsPesquisa);

                            ddlidTipoFaturamento.Visible = true;

                            depara_Pesquisar();
                            PopularArt(DATASET(dsPesquisa, nTabela_Dados, "sCNPJ_Cliente"), DATASET(dsPesquisa, nTabela_Dados, "sRazaoSocial"), DATASET(dsPesquisa, nTabela_Dados, "sRazaoSocialFinal"), DATASET(dsPesquisa, nTabela_Dados, "sCNPJ_ClienteFinal"));
                            Consulta_gvCheckList(DATASET(dsPesquisa, nTabela_Dados, "idPedido_Vinculado"));
                        }
                        else if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
                        {
                            if (ValidaPermissao(Permissao.Comex.Visualizar_Aba_Documentos)) aba_Documentos.Visible = true;
                            aba_ValoresDatas.Visible = true;

                            gvImportacao_Envios_Popular(dsPesquisa, sMetodoChamada);
                            gvImportacao_Volumes_Popular(dsPesquisa, sMetodoChamada);
                        }

                        if (DATASET(dsPesquisa, 0, "sExisteTarefas") == "S") Popular_Aba_Tarefas(idPedido, idRegistroTarefa);

                        if (DATASET(dsPesquisa, 0, "sPermiteEdicaoPedido") == "S")
                        {
                            cmdEditar.Visible = ValidaPermissao(Permissao.Pedidos.Alterar);
                            ValoresDatas_cmdEditar.Visible = ValidaPermissao(Permissao.Pedidos.Alterar);
                            cmdDuplicar.Visible = ValidaPermissao(Permissao.Pedidos.Alterar);
                            ValoresDatas_cmdDuplicar.Visible = ValidaPermissao(Permissao.Pedidos.Alterar);
                            frmacoes.Attributes.Add("src", string.Format("Pedidos_Detalhe_Acoes.aspx?id={0}&sGerarTarefas={1}&idTipo={2}&STSO={3}", idPedido, DATASET(dsPesquisa, 0, "sGerarTarefas"), hddidTipo.Value, DATASET(dsPesquisa, nTabela_Dados, "sSTSO")));
                            aba_Acoes.Visible = true;
                        }

                        if (DATASET(dsPesquisa, "idMotivoCancelamento") == "0")
                            DIV_Departamento.Visible = true;

                        DIV_STATUS_ATUAL.Visible = true;
                        div_EstimativaEntrega.Visible = true;
                        div_PrevisaoEntrega.Visible = true;
                        div_SelecaoItens.Visible = false;
                        PainelAtualizacao.Visible = true;
                        cmdGravarPedido.Visible = false;
                        ValoresDatas_cmdGravarPedido.Visible = false;

                        lblTituloSalvar.Text = "Confirma a Alteração do pedido " + lblTituloPagina.Text + "?";
                        lblTituloEdiar.Text = "Deseja editar o pedido " + lblTituloPagina.Text + "?";
                        lblTituloDuplicar.Text = "Deseja Duplicar o pedido " + lblTituloPagina.Text + "?";

                        if (sMetodoChamada != "Editar" && sMetodoChamada != "Duplicar")
                        {
                            Popular_Kanban(idPedido);
                            Popular_dtgPagamento(dsPesquisa);
                            Popular_gv_Garantia(dsPesquisa);
                        }
                        else
                        {
                            cmdGravarPedido.Text = "Salvar";
                            ValoresDatas_cmdGravarPedido.Text = "Salvar";
                            Resultados_cmdGravarPedido.Text = "Salvar";
                        }

                        DIV_Valores.Visible = true;

                        if (ValidaPermissao(Permissao.Pedidos.EnxergarValores))
                        {
                            DIV_nVlrProdutos.Visible = true;
                            DIV_nVlrServicos.Visible = true;
                            div_nVlrImpostos.Visible = hddidTipo.Value == "2";
                            DIV_DADOS_CondPagamento.Visible = hddidTipo.Value == "2" || hddidTipo.Value == "7";
                            DIV_nVlrTotal.Visible = true;
                        }
                        else if (hddidTipo.Value == "2")
                            DIV_Valores.Visible = false;
                        else if (hddidTipo.Value == "7")
                            DIV_DADOS_CondPagamento.Visible = true;

                        if (hddidTipo.Value == "2")
                        {
                            DIV_Comissao.Visible = ValidaPermissao(Permissao.Pedidos.ExibirComissionador_Comissao);

                            if (DIV_Comissao.Visible && ddlidParceiro_Comissionador.SelectedValue != "0")
                            {
                                div_ComissaoValor.Visible = true;
                                currentClass = div_ComissaoPaga.Attributes["class"];
                                div_ComissaoPaga.Attributes["class"] = currentClass.Replace("visible", "").Trim();
                            }
                        }

                        if (idRegistroTarefa != "0")
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Acao_Tab_Tarefas", "$('#tarefas-tab').tab('show');", true);

                        if (Variaveis.idParceiro() != "0")
                        {
                            aba_Acoes.Visible = false;
                            aba_Arquivos.Visible = false;
                            aba_Documentos.Visible = false;
                            aba_Historico.Visible = false;
                            aba_ArquivoMorto.Visible = false;
                            aba_Tarefas.Visible = false;
                            aba_ValoresDatas.Visible = false;
                            aba_depara.Visible = true;
                        }

                        if (hddidTipo.Value == "7")
                        {
                            div_PrevisaoEntrega.Visible = true;
                            DivTipoCompra.Attributes["class"] = "col-lg-3";
                            cmdEditar.Visible = ValidaPermissao(Permissao.Compras.Pedido_Compras.Alterar);
                        }

                        if (hddidTipo.Value == "2")
                        {
                            txtTotal_Pedido.Text = txtnVlrTotal.Text;
                            txtTotal_Faturado.Text = "0,00";
                            txtSaldo_Faturar.Text = txtnVlrTotal.Text;
                            txtTotalParcial.Text = "0,00";

                            ddlDepartamento.Popula_Combo($"sp_Select 'Flow_Departamento_x_Tarefas',@idPesquisa={hddidPedido.Value}", "idDepartamento", "sDscDepartamento", false, "Selecione o Departamento", "0");
                            PopularFaturamento(dsPesquisa);
                            FaturamentosEfetuados();

                            if (ddlTipoFaturamento.SelectedValue == "0")
                                div_DeptoFaturamento.Visible = false;
                        }
                    }
                    else
                    {
                        aba_Dados.Visible = false;
                        DIV_PEDIDO_GERAL.Visible = false;
                        lblTituloPagina.Visible = false;
                        BreadCrumb.TitulodaPagina = "Erro ao carregar";

                        throw new Exception("Erro ao carregar pedido!");
                    }

                    if (hddidTipo.Value == "7")
                    {
                        Dictionary<string, string> vParametrosStatus = new Dictionary<string, string>
                        {
                            { "@sFuncao", "PESQUISA_STATUS" },
                            { "@idPedido", hddidPedido.Value }
                        };
                        DataSet ds = ExecutarDataSet(sProcedure_Consulta, vParametrosStatus, false);

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            if (DATASET(ds, 0, "sPedidoFinalizado").Contains("S"))
                            {
                                cmdEditar.Visible = false;
                                DADOS_lblEntrega.Visible = false;
                                lbldtPrevisaoEntrega.Visible = false;
                            }
                        }
                    }
                }
                else
                {
                    cmdGerarOPI.Visible = false;

                    if (Variaveis.idParceiro() == "0")
                    {
                        ConfigurarCampos();
                        DIV_nVlrProdutos.Visible = true;
                        DIV_nVlrServicos.Visible = true;
                        DIV_nVlrTotal.Visible = true;
                        aba_Resultado.Visible = false;
                        aba_depara.Visible = false;
                        Div_PagoEnvio.Visible = true;
                        Div_Garantia.Visible = true;
                        aba_STSO.Visible = false;
                        aba_Documentos.Visible = false;
                        aba_OPI.Visible = false;
                        aba_Cronograma.Visible = false;
                        aba_Faturamento.Visible = false;

                        if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
                        {
                            txtnVlrProdutos.ReadOnly = true;
                            div_Ordem.Visible = true;
                            div_btdesc.Visible = true;
                            div_ImportarItens.Visible = true;
                        }

                        if (hddidTipo.Value == "2" || hddidTipo.Value == "7")
                        {
                            PesquisaCliente(idCliente);
                            DIV_Comissao.Visible = true;
                            div_ComissaoValor.Visible = true;
                            currentClass = div_ComissaoPaga.Attributes["class"];
                            div_ComissaoPaga.Attributes["class"] = currentClass.Replace("visible", "").Trim();
                            Div_PagoEnvio.Visible = false;
                            txtnVlrProdutos.ReadOnly = true;
                            txtnVlrServicos.ReadOnly = true;
                            txtFrete.ReadOnly = false;
                            txtnFreteVendas.ReadOnly = false;
                            txtnTempoContrato.ReadOnly = false;
                            ddlTipoPeriodo.Attributes.Remove("disabled");

                            if (hddidTipo.Value == "7")
                            {
                                PopulaCombosConceitos();
                                DIV_Comissao.Visible = false;
                                txtnVlrServicos.ReadOnly = false;
                                div_PrevisaoEntrega.Visible = false;
                                DivTipoCompra.Attributes["class"] = "col-lg-4";
                                aba_Documentos.Visible = false;
                                DivCentroCustos.Visible = true;
                                DivConceito.Visible = true;
                                DIV_Conceitual.Visible = true;
                            }
                            else
                            {
                                div_nVlrFrete.Visible = false;
                                Div_FreteVendas.Visible = true;
                                DivCentroCustos.Visible = false;
                                DivConceito.Visible = false;
                                DIV_nVlrProdutos.Attributes["class"] = "col-lg-3";
                                DIV_nVlrServicos.Attributes["class"] = "col-lg-3 form-group";
                            }

                            if (hddidTipo.Value == "2")
                            {
                                DIV_Instalador.Attributes["class"] = "col-lg-12";
                                DIV_ddlInstalador.Attributes["class"] = "col-lg-6";
                            }
                        }

                        cmdGravarPedido.Text = "Incluir";
                        ValoresDatas_cmdGravarPedido.Text = "Incluir";
                        cmdGravarPedido.Visible = true;
                        ValoresDatas_cmdGravarPedido.Visible = true;

                        if (hddidTipo.Value != "7")
                            Popula_Combo(ddlsEnderecoEntrega, string.Format("sp_Manipula_tbl_Flow_Clientes 'CONSULTAR_ENDERECO', {0}", idCliente), "idEndereco", "sEnderecoCompleto", false);

                        if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
                            dtgItens.Columns[Col_chkItem_Selecionado].Visible = true;
                        else
                            dtgItens.Columns[Col_chkItem_Selecionado].Visible = false;

                        dtgItens.Columns[Col_nQuantidade_Edit].Visible = true;
                        dtgItens.Columns[Col_nQuantidade_View].Visible = false;
                        dtgItens.Columns[Col_nValorUnitario_Edit].Visible = true;
                        dtgItens.Columns[Col_nValorUnitario_View].Visible = false;
                        dtgItens.Columns[Col_nDesconto_Edit].Visible = hddidTipo.Value == "7";
                        dtgItens.Columns[Col_nDesconto_View].Visible = false;
                        dtgItens.Columns[Col_BotaoExcluir].Visible = true;
                        dtgItens.Columns[Col_dtPrevisaoEntrega_Edit].Visible = false;
                        dtgItens.Columns[Col_dtPrevisaoEntrega_View].Visible = false;

                        gvImportacao_Volumes.Columns[0].Visible = false;
                        gvImportacao_Volumes.Columns[1].Visible = false;
                        gvImportacao_Volumes.Columns[2].Visible = true;
                        gvImportacao_Volumes.Columns[3].Visible = false;
                        gvImportacao_Volumes.Columns[4].Visible = false;
                        gvImportacao_Volumes.Columns[5].Visible = false;
                        gvImportacao_Volumes.Columns[6].Visible = false;
                        gvImportacao_Volumes.Columns[7].Visible = true;
                        gvImportacao_Volumes.Columns[8].Visible = true;
                        gvImportacao_Volumes.Columns[9].Visible = false;
                        gvImportacao_Volumes.Columns[10].Visible = true;
                        gvImportacao_Volumes.Columns[11].Visible = false;
                        gvImportacao_Volumes.Columns[12].Visible = true;
                        gvImportacao_Volumes.Columns[13].Visible = false;
                        gvImportacao_Volumes.Columns[14].Visible = true;
                        gvImportacao_Volumes.Columns[15].Visible = false;
                        gvImportacao_Volumes.Columns[16].Visible = true;
                        gvImportacao_Volumes.Columns[17].Visible = true;
                        gvImportacao_Volumes.Columns[18].Visible = false;

                        if (hddidTipo.Value != "3" && hddidTipo.Value != "6")
                            dtgItens.Columns[Col_sCodigoNCM].Visible = false;
                        else
                        {
                            aba_ValoresDatas.Visible = true;

                            if (txtnInlandF.Text.Length < 1) txtnInlandF.Text = "0,00";
                            if (txtnHandling.Text.Length < 1) txtnHandling.Text = "0,00";
                            if (txtnConsular.Text.Length < 1) txtnConsular.Text = "0,00";
                            if (txtnOcean_Air.Text.Length < 1) txtnOcean_Air.Text = "0,00";
                            if (txtnInsurance.Text.Length < 1) txtnInsurance.Text = "0,00";
                            if (txtnOther_Charges.Text.Length < 1) txtnOther_Charges.Text = "0,00";
                            if (txtnTotalEnvio.Text.Length < 1) txtnTotalEnvio.Text = "0,00";
                        }

                        txtdtPedido.Text = DateTime.Today.ToString("dd/MM/yyyy");
                        PainelAtualizacao.Visible = false;
                        div_SelecaoItens.Visible = true;

                        if (hddidTipo.Value == "7")
                        {
                            txtsPedidoCliente.ReadOnly = true;
                            txtdtEstimativaEntrega.ReadOnly = true;
                            dtgItens.Columns[Col_dtPrevisaoEntrega_Edit].Visible = true;
                            dtgItens.Columns[Col_dtPrevisaoEntrega_View].Visible = false;
                        }
                    }
                    else
                    {
                        aba_Dados.Visible = false;
                        DIV_PEDIDO_GERAL.Visible = false;
                        lblTituloPagina.Visible = false;
                        BreadCrumb.TitulodaPagina = "Ação não permitida!";
                        throw new Exception("Ação não permitida!");
                    }

                    if (sClienteFinalswt.Recuperar() == "S")
                        div_idClienteFinal.Attributes["class"] = "col-lg-10";
                    else div_idClienteFinal.Attributes["class"] = "col-lg-10 visible";

                    sClienteFinalswt.Definir("N", "Tem cliente Final ?", "");
                    ComissaoPaga.Definir("N", "Paga", "");

                    currentClass = div_ComissaoPaga.Attributes["class"];
                    div_ComissaoPaga.Attributes["class"] = currentClass + " visible";
                    div_ComissaoValor.Visible = false;

                    if (ddlidParceiro_Comissionador.SelectedValue != "0")
                    {
                        currentClass = div_ComissaoPaga.Attributes["class"];
                        div_ComissaoPaga.Attributes["class"] = currentClass.Replace("visible", "").Trim();
                        div_ComissaoValor.Visible = true;
                    }
                }

                if (hddidTipo.Value == "7")
                    ddlsTipoCompra.Focus();
                else txtsPedidoCliente.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        void PesquisaCliente(string idCliente)
        {
            try
            {
                if (idCliente != "0")
                {
                    string sProcedure = "sp_Manipula_tbl_Flow_Clientes";
                    string sErro = "";

                    DataSet dsCliente;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR" },
                        { "@idCliente", idCliente }
                    };

                    dsCliente = ExecutarDataSet(sProcedure, vParametros);
                    if (ValidarDataSet(dsCliente, out sErro))
                    {
                        txtsRazaoSocial.Text = DATASET(dsCliente, 0, "sRazaoSocial");
                        hddidCliente.Value = DATASET(dsCliente, 0, "idCliente");
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }
                    txtsRazaoSocial.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro Consulta Cliente: " + ex.Message);
            }
        }

        #endregion

        #region | Utils

        #region | ID Pedido

        int idPedido()
        {
            int idPedido = 0;
            string[] vidPedido = hddidPedido.Value.Split(',');
            if (vidPedido[0].ToString() == "")
            {
                idPedido = 0;
            }
            else
            {
                idPedido = Convert.ToInt32(vidPedido[0].ToString());
            }
            return idPedido;
        }

        #endregion

        #region | Popular

        void PopulaCombos()
        {
            if (hddidTipo.Value == "2" || hddidTipo.Value == "7")
            {
                Popula_Combo(ddlVendedor, "sp_Select 'FLOW_Vendedores'", "idVendedor", "sDscUsuario", false, "Selecione o Vendedor", "0");

                if (hddidTipo.Value != "7")
                {
                    string Cliente = "";
                    if (hddidCliente.Value != "") Cliente = hddidCliente.Value;
                    if (hddidCliente_Produto.Value != "") Cliente = hddidCliente_Produto.Value;

                    int idPedido = 0;
                    if (hddidPedido.Value == "") idPedido = 0;
                    else idPedido = int.Parse(hddidPedido.Value);

                    Popula_Combo(ddlCondPagamento, $"sp_Select 'FLOW_CondicaoDePagamento', @idFiltro= 2, @idPesquisa={Cliente}, @idPais={idPedido}", "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione a Condição de Pagamento", "0");
                }

                Popula_Combo(ddlidTipoFaturamento, "sp_Select 'Flow_TipoFaturamento'", "idTipoFaturamento", "sDscTipoFaturamento", false, "Selecione o Tipo de Faturamento ", "0");
                Popula_Combo(ddlidParceiro_Comissionador, "sp_Select 'Flow_Parceiro_RepresentanteComercial'", "idParceiro", "sRazaoSocial", false, "Selecione o Parceiro", "0");
                Popula_Combo(ddlidClienteFinal, "sp_Select 'Flow_Clientes'", "idCliente", "sRazaoSocial", false, "Selecione o Cliente Final", "0");
                Popula_Combo(ddlidColaboradorOS, "sp_Select 'Usuarios'", "idUsuario", "sDscUsuario", false, "Selecione o Colaborador", "0");
                Popula_Combo(ddlidInstalador, "sp_Select 'Flow_Parceiro_Importacao_Instalador'", "idParceiro", "sRazaoSocial", false, "Selecione o Instalador", "0");
                ddlsMoeda.Popula_Combo("sp_Select 'tbl_Flow_Tipo_Moeda_CambioCadastrado'", "idmOeda", "sDscTipoMoeda", false, "Selecione a Moeda", "0");
            }

            if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
            {
                Popula_Combo(ddlidPais, "sp_Select 'tbl_Flow_WMS_Produtos_Origem'", "idPais", "sDscPais", false, "Selecione o País de Origem", "0");
                Popula_Combo(ddlImportacao_idDespachante, "sp_Select 'Flow_Parceiro_Importacao_Despachante'", "idParceiro", "sRazaoSocial", false, "Selecione o Despachante", "0");
                Popula_Combo(ddlidModal, "sp_Select 'tbl_Flow_Pedidos_Modal'", "idModal", "sDscModal", false, "Selecione o Modal", "0");
                Popula_Combo(ddlImportacao_idDespacho, "sp_Select 'tbl_Flow_Pedidos_Despacho'", "idDespacho", "sDscDespacho", false, "Selecione o Despacho", "0");
                Popula_Combo(ddlImportacao_idFornecedor, "sp_Select 'Flow_Parceiro_Importacao_Fornecedor', @idPesquisa=" + ddlidPais.SelectedValue, "idParceiro", "sRazaoSocial", false, "Selecione o Exportador", "0");
                Popula_Combo(ddlImportador, "sp_Select 'Flow_Parceiro_Importacao_Importador'", "idParceiro", "sRazaoSocial", false, "Selecione o Importador", "0");
                Popula_Combo(ddlTermosPagamento, "sp_Select 'FLOW_CondicaoDePagamento'", "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione os Termos de Pagamentos", "0");
                ddlsMoeda.Popula_Combo("sp_Select 'tbl_Flow_Tipo_Moeda_CambioCadastrado'", "idmOeda", "sDscTipoMoeda", false, "Selecione a Moeda", "0");
            }

            Popula_Combo(Item_ddlsUnidade, "sp_Select 'Flow_Produtos_Unidade'", "sUnidade", "sDscUnidade", false, "Selecione ", "0");
            Popula_Combo(LM_ddlsUnidade, "sp_Select 'Flow_Produtos_Unidade'", "sUnidade", "sDscUnidade", false, "Selecione ", "");
            Popula_Combo(ddlTipoEnvio, "sp_Select 'Flow_Pedidos_TipoEnvio'," + hddidTipo.Value, "idTipoEnvio", "sDscTipoEnvio", false, "Selecione o Tipo de Envio ", "0");

            if (hddidTipo.Value != "7") Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
            if (hddidTipo.Value == "2") ddlEmpresa_Faturamento.Popula_Combo("sp_Select 'Flow_Empresa', @idUsuario=" + Variaveis.idUsuario(), "idEmpresa", "sDscCodigoEmpresa", false, "Selecione a Empresa", "0");

            Popula_Combo(ddlidEmpresaSTSO, "sp_Select 'Flow_Empresa', @idUsuario=" + Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");

            if (hddidTipo.Value == "6") Popula_Combo(ddlTipoEnvio, "sp_Select 'Flow_Pedidos_TipoEnvio', 3", "idTipoEnvio", "sDscTipoEnvio", false, "Selecione o Tipo de Envio ", "0");

            if (hddidTipo.Value == "7")
            {
                Popula_Combo(ddlVendedor, "sp_Select 'tbl_Compradores'", "idUsuario", "sDscUsuario", false, "Selecione o Comprador", "0");
                Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @sTipo = S, @idUsuario=" + Variaveis.idUsuario(), "idEmpresa", "sDscEmpresaReduzida", false, "Selecione a Empresa", "0");
                Popula_Combo(Item_ddlsUnidadeEntrega, "sp_Select 'Flow_Produtos_Unidade'", "sUnidade", "sDscUnidade", false, "Selecione ", "0");
                ddlsMoeda.Popula_Combo("sp_Select 'tbl_Flow_Tipo_Moeda_CambioCadastrado'", "idmOeda", "sDscTipoMoeda", false, "Selecione a Moeda", "0");
                Popula_Combo(ddlFrete, "sp_Select 'Flow_Pedidos_Frete'", "idTipoEnvio", "sDscTipoEnvio", false, "Selecione o Frete", "0");
            }

            SqlDataReader dr;
            dr = ExecutarDataReader("sp_Select 'Flow_Fluxo', " + hddidTipo.Value);

            if (dr != null)
            {
                Base_Fluxo.Clear();
                ddlFluxoPedido.Items.Clear();
                ddlFluxoPedido.Items.Add(new ListItem("Selecione o Fluxo", "0"));

                while (dr.Read())
                {
                    cls_Fluxo objItem = new cls_Fluxo
                    {
                        idFluxo = Convert.ToInt32(dr["idFluxo"].ToString()),
                        sDscFluxo = dr["sDscFluxo"].ToString(),
                        nTempoTotalHoras = Convert.ToInt32(dr["nTempoTotalHoras"].ToString()),
                        nTempoDias = Convert.ToInt32(dr["nTempoDias"].ToString())
                    };
                    Base_Fluxo.Add(objItem);

                    ddlFluxoPedido.Items.Add(new ListItem(objItem.sDscFluxo, objItem.idFluxo.ToString()));
                }
            }

            dr.Close();
        }

        void PopulaCombosConceitos()
        {
            Popula_Combo(ddlConceito, "sp_Manipula_tbl_Flow_Pedidos 'FLOW-CONCEITO'", "idConceito", "sDscConceito", false, "Nenhum Conceito", "0");
            Popula_Combo(ddlCentroCusto, "sp_Manipula_tbl_Flow_Pedidos 'FLOW-CENTRO-CUSTOS'", "idCentroDeCusto", "sDescricao", false, "Selecione o Centro de Custo", "0");
        }

        void Popular_Aba_Historico(DataSet ds)
        {
            gv_Historico.DataSource = ds.Tables[nTabela_Historico];
            gv_Historico.DataBind();
            aba_Historico.Visible = true;
        }

        void Popular_Aba_Arquivos(string idPedido)
        {
            string sTipoObjeto = FrameWork.cls_Arquivos.ObterTipoObjetoArquivoPedido(hddidTipo.Value);
            frmArquivos.Attributes.Add("src", string.Format("Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}&sTp={2}", idPedido, sTipoObjeto, hddidTipo.Value));
            aba_Arquivos.Visible = true;
        }

        void Popular_Aba_ArquivoMorto(string idPedido)
        {
            aba_ArquivoMorto.Visible = ValidaPermissao(Permissao.Pedidos.Visualizar_Aba_ArquivoMorto);
            if (!aba_ArquivoMorto.Visible)
                return;

            string sTipoObjeto = cls_Arquivos.ObterTipoObjetoArquivoPedido(hddidTipo.Value);
            cls_Arquivos objArquivos = new cls_Arquivos();
            DataSet dsArquivados = objArquivos.ConsultarArquivados(Convert.ToInt32(idPedido), sTipoObjeto);

            if (ValidarDataSet(dsArquivados, out _))
            {
                gv_ArquivoMorto.DataSource = dsArquivados.Tables[0];
                gv_ArquivoMorto.DataBind();
            }
            else
            {
                gv_ArquivoMorto.DataSource = null;
                gv_ArquivoMorto.DataBind();
            }
        }

        protected void gv_ArquivoMorto_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            LinkButton lnkDownload = e.Row.FindControl("lnkArquivoMorto_Download") as LinkButton;
            if (lnkDownload != null)
                lnkDownload.Visible = ValidaPermissao(Permissao.Pedidos.Download_ArquivoMorto);
        }

        protected void gv_ArquivoMorto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Download_Arquivo")
                return;

            if (!ValidaPermissao(Permissao.Pedidos.Visualizar_Aba_ArquivoMorto))
            {
                MensagemPagina.MostraMensagem_Erro("Você não possui permissão para visualizar o arquivo morto.");
                Scripts.Mantem_AbaAtiva(Page, "arquivoMorto-tab");
                return;
            }

            if (!ValidaPermissao(Permissao.Pedidos.Download_ArquivoMorto))
            {
                MensagemPagina.MostraMensagem_Erro("Você não possui permissão para baixar arquivos do arquivo morto.");
                Scripts.Mantem_AbaAtiva(Page, "arquivoMorto-tab");
                return;
            }

            try
            {
                int idArquivo = Convert.ToInt32(e.CommandArgument);
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idArquivo", idArquivo.ToString() }
                };
                DataTable dtArquivo = ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);
                if (dtArquivo.Rows.Count == 0)
                {
                    MensagemPagina.MostraMensagem_Erro("Arquivo não encontrado.");
                    Scripts.Mantem_AbaAtiva(Page, "arquivoMorto-tab");
                    return;
                }

                string sNomeArquivo = dtArquivo.Rows[0]["sNomeArquivo"].ToString().Replace(",", "");
                byte[] bytes = dtArquivo.Rows[0]["vbArquivo"] as byte[];
                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);
                DownloadArquivo(Page, sNomeArquivo);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }

            Scripts.Mantem_AbaAtiva(Page, "arquivoMorto-tab");
        }

        void Popular_Kanban(string idPedido)
        {
            frmKanban.Attributes.Add("src", string.Format("Kanban.aspx?idPedido={0}", idPedido));
            DIV_Kanban.Visible = true;
        }

        void Popular_Aba_Tarefas(string idPedido, string idRegistroTarefa)
        {
            string sDashBoard = "";
            if (Request["DashBoard"] != null)
            {
                sDashBoard = Request["DashBoard"].ToString();
            }

            frmTarefas.Attributes.Add("src", string.Format("Pedidos_Detalhe_Tarefas.aspx?id={0}&idrt={1}&DashBoard={2}", idPedido, idRegistroTarefa, sDashBoard));
            aba_Tarefas.Visible = true;
        }

        void Popular_GVItens(DataSet ds, string sMetodoChamada)
        {
            string sFuncao = "INCLUIR ITEM";
            int idPedido = 0;
            int idItem = 0;

            ddlServico.Items.Clear();
            ddlServico.Items.Add(new ListItem("Selecione um Serviço", "0"));

            foreach (DataRow row in ds.Tables[nTabela_Itens].Rows)
            {
                if (int.TryParse(row["idProdutoPai"].ToString(), out int idProdutoPai) && idProdutoPai > 0 && row["sTipoItem"].ToString().Equals("Produto"))
                    continue;

                if (sMetodoChamada != "Duplicar")
                {
                    sFuncao = "CONSULTA ITEM";
                    idPedido = Convert.ToInt32(row["idPedido"]);
                    idItem = Convert.ToInt32(row["idItem"]);
                }

                cls_Pedidos_Itens objItem = new cls_Pedidos_Itens
                {
                    idItem = idItem,
                    idPedido = idPedido,
                    idProduto = Convert.ToInt32(row["idProduto"]),
                    sCodigoProduto = row["sCodigo"].ToString(),
                    sDscProduto = row["sDscProduto"].ToString(),
                    sUnidade = row["sUnidade"].ToString(),
                    nQuantidade = Convert.ToDouble(row["nQuantidade"].ToString()),
                    nValorUnitario = Convert.ToDouble(row["nValorUnitario"].ToString()),
                    nValorTotal = Convert.ToDouble(row["nValorTotal"].ToString()),
                    sDscCategoriaVendas = row["sDscCategoriaVendas"].ToString(),
                    dtPrevisaoEntrega = row["sEntregue"].ToString(),
                    sFuncao = sFuncao,
                    idContador = Base_Pedidos_Itens.Count() + 1,
                    nOrdem = Convert.ToInt32(row["nOrdem"]),
                    idCNM = Convert.ToInt32(row["idNCM"].ToString()),
                    sCodigoNCM = row["sCodigoNCM"].ToString(),
                    sCodigoCEST = row["sCodigoCEST"].ToString(),
                    Importacao_idDestino = Convert.ToInt32(row["Importacao_idDestino"].ToString()),
                    Importacao_sDscDestino = row["Importacao_sDscDestinoEN"].ToString(),
                    Importacao_idAtoConcessorio = Convert.ToInt32(row["Importacao_idAtoConcessorio"].ToString()),
                    Importacao_sCodigoAtoConcessorio = row["Importacao_sCodigoAtoConcessorio"].ToString(),
                    Importacao_dtPO = row["Importacao_dtPO"].ToString(),
                    Importacao_dtETA = row["Importacao_dtETA"].ToString(),
                    Importacao_dtETD = row["Importacao_dtETD"].ToString(),
                    nValorReal = Convert.ToDouble(row["nValorReal"].ToString()),
                    nValorRealTotal = Convert.ToDecimal(row["nValorRealTotal"].ToString()),
                    nResultado = Convert.ToDecimal(row["nResultado"].ToString()),
                    sTipoProduto_Servico = row["sTipoProduto_Servico"].ToString(),
                    sDscProdutoIdioma = row["sDscProdutoIdioma"].ToString(),
                    sTipo = row["sTipo"].ToString(),
                    nICMS = Convert.ToDecimal(row["Item_nICMS"].ToString()),
                    nIPI = Convert.ToDecimal(row["Item_nIPI"].ToString()),
                    nValorTblPreco = Convert.ToDecimal(row["nValorTblPreco"].ToString()),
                    sUnidadeEntrega = row["sUnidadeEntrega"].ToString(),
                    dtChegada = row["dtChegada"].ToString(),
                    sMoedaVenda = sMoeda,

                    nVlrIPI = Convert.ToDecimal((row["nVlrIPI"] ?? "0").ToString()),
                    nVlrICMS = Convert.ToDecimal((row["nVlrICMS"] ?? "0").ToString()),
                    nMVA = Convert.ToDecimal((row["nMVA"] ?? "0").ToString()),
                    nICMS_Interno_Destino = Convert.ToDecimal((row["nICMS_Interno_Destino"] ?? "0").ToString()),
                    bProdutoPai = false
                };

                objItem.nVlrDIFAL = objItem.nValorTotal.DoubleToDecimal() * (Convert.ToDecimal((row["nDIFAL"] ?? "0").ToString()) / 100);

                if (hddidTipo.Value == "7") objItem.nDesconto = Convert.ToDecimal((row["nDesconto"] ?? "0").ToString());
                if (hddidTipo.Value == "2")
                {
                    objItem.bProdutoPai = row["sProdutoPai"].ToString().Equals("S");

                    try
                    {
                        objItem.nMVA = Retorna_MVA_LegisWeb(row["sUF_Origem"].ToString(), row["sUF_Destino"].ToString(), objItem.nMVA, row["sDestinoVenda"].ToString().Equals("R"), row["sICMSST"].ToString().Equals("S"), objItem.sCodigoNCM, objItem.sCodigoCEST, objItem.nVlrICMS, row["dtUltimaConsulta"].ToString(), row["sMsgErro"].ToString());

                        if (objItem.nMVA > 0)
                        {
                            var st = CalculaValor_ST(objItem.nValorTotal.DoubleToDecimal(), objItem.nVlrICMS * objItem.nQuantidade.DoubleToDecimal(), objItem.nMVA, objItem.nICMS_Interno_Destino);
                            objItem.nVlrST = st.Item1;
                            objItem.nMVA = st.Item2;
                        }
                    }
                    catch { }

                    if (row["sTipoItem"].ToString().Equals("Servico_Recurso")) ddlServico.Items.Add(new ListItem($"{objItem.sCodigoProduto} - {objItem.sDscProduto}", objItem.idItem.ToString()));
                }

                Base_Pedidos_Itens.Add(objItem);
            }

            hddsAlteracaoItens.Value = "N";
            DataBind_dtgItens(sMetodoChamada);
        }

        #endregion

        #region | Configurações de Campos

        void StatusDosCampos(bool bStatus)
        {
            string sStatusCombo = "disabled";

            if (!bStatus)
                sStatusCombo = "enabled";

            txtsObservacao.ReadOnly = bStatus;
            txtsReferencia.ReadOnly = bStatus;
            txtsDepartamentoAtual.ReadOnly = bStatus;
            txtsDscStatus.ReadOnly = bStatus;
            txtsPedidoCliente.ReadOnly = bStatus;
            txtnVlrProdutos.ReadOnly = bStatus;
            txtnVlrServicos.ReadOnly = bStatus;
            TextInvoiceNumber.ReadOnly = bStatus;
            txtnOrderNumber.ReadOnly = bStatus;
            txtsZonaEnvio.ReadOnly = bStatus;
            txtnOther_Charges.ReadOnly = bStatus;
            txtnInsurance.ReadOnly = bStatus;
            txtnOcean_Air.ReadOnly = bStatus;
            txtnConsular.ReadOnly = bStatus;
            txtnHandling.ReadOnly = bStatus;
            txtnInlandF.ReadOnly = bStatus;
            txtnVlrComissao.ReadOnly = bStatus;
            txtnControleTT.ReadOnly = bStatus;
            txtCustoAduaneiro.ReadOnly = bStatus;
            txtCustoDespachante.ReadOnly = bStatus;

            ddlCondPagamento.Attributes.Remove("disabled");
            ddlCondPagamento.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlFluxoPedido.Attributes.Remove("disabled");
            ddlFluxoPedido.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlVendedor.Attributes.Remove("disabled");
            ddlVendedor.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlTipoEnvio.Attributes.Remove("disabled");
            ddlTipoEnvio.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlidTipoFaturamento.Attributes.Remove("disabled");
            ddlidTipoFaturamento.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlidEmpresa.Attributes.Remove("disabled");
            ddlidEmpresa.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlidParceiro_Comissionador.Attributes.Remove("disabled");
            ddlidParceiro_Comissionador.Attributes.Add(sStatusCombo, sStatusCombo);
            //ddlsComissaoPaga.Attributes.Remove("disabled");
            //ddlsComissaoPaga.Attributes.Add(sStatusCombo, sStatusCombo);
            ComissaoPaga.BloquearEdicao(true);
            ddlImportacao_idDespachante.Attributes.Remove("disabled");
            ddlImportacao_idDespachante.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlImportacao_idFornecedor.Attributes.Remove("disabled");
            ddlImportacao_idFornecedor.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlidModal.Attributes.Remove("disabled");
            ddlidModal.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlidPais.Attributes.Remove("disabled");
            ddlidPais.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlImportacao_idDespacho.Attributes.Remove("disabled");
            ddlImportacao_idDespacho.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlTermosPagamento.Attributes.Remove("disabled");
            ddlTermosPagamento.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlContato_Exportador.Attributes.Remove("disabled");
            ddlContato_Exportador.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlContato_Despachante.Attributes.Remove("disabled");
            ddlContato_Despachante.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlContato_Empresa.Attributes.Remove("disabled");
            ddlContato_Empresa.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlImportador.Attributes.Remove("disabled");
            ddlImportador.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlContato_Importador.Attributes.Remove("disabled");
            ddlContato_Importador.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlEnvio.Attributes.Remove("disabled");
            ddlEnvio.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlFornecedor.Attributes.Remove("disabled");
            ddlFornecedor.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlsTipoCompra.Attributes.Remove("disabled");
            ddlsTipoCompra.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlConceito.Attributes.Remove("disabled");
            ddlConceito.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlCentroCusto.Attributes.Remove("disabled");
            ddlCentroCusto.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlGrupoPatrimonio.Attributes.Remove("disabled");
            ddlGrupoPatrimonio.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlsMoeda.Atributos.Remove("disabled");
            ddlsMoeda.Atributos.Add(sStatusCombo, sStatusCombo);
            sClienteFinalswt.BloquearEdicao(true);
            ddlidClienteFinal.Attributes.Remove("disabled");
            ddlidClienteFinal.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlidInstalador.Attributes.Remove("disabled");
            ddlidInstalador.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlFrete.Attributes.Remove("disabled");
            ddlFrete.Attributes.Add(sStatusCombo, sStatusCombo);

            if (hddidTipo.Value == "2" || hddidTipo.Value == "7")
            {
                //Antonio Lemos - 20/06/2024
                //Solução péssima, mas só os pedidos acima deste número ou novos deverão ser calculados automáticamente
                //Não retirar (GATO)
                if (idPedido() > 1879 || idPedido() == 0)
                {
                    txtnVlrProdutos.ReadOnly = true;
                    if (hddidTipo.Value != "7")
                        txtnVlrServicos.ReadOnly = true;
                }
            }
            if (hddidTipo.Value == "7")
            {
                txtsPedidoCliente.ReadOnly = true;
            }
        }

        void ConfigurarCampos()
        {
            DIV_DADOS_Origem.Visible = false;
            DIV_DADOS_NPedido.Visible = false;
            DIV_DADOS_Cliente.Visible = false;
            DIV_DADOS_Despachante.Visible = false;
            DIV_DADOS_Despacho.Visible = false;
            DIV_DADOS_Modal.Visible = false;
            DIV_DADOS_Importacao_Fornecedor.Visible = false;
            DIV_DADOS_Vendedor.Visible = false;
            DIV_DADOS_CondPagamento.Visible = false;
            DIV_DADOS_TipoFaturamento.Visible = false;
            DIV_Volumes.Visible = false;
            DIV_Envios.Visible = false;
            DIV_Item_ValorUnitario.Visible = false;
            aba_Documentos.Visible = false;
            aba_ValoresDatas.Visible = false;
            aba_LM.Visible = false;
            DIV_TermosPagamento.Visible = false;
            InvoiceNumber.Visible = false;
            Contato_Exportador.Visible = false;
            Contato_Despachante.Visible = false;
            aba_Resultado.Visible = false;
            DIV_OrderNumber.Visible = false;
            DIV_Exportador.Visible = false;
            DIV_Despachante_C.Visible = false;
            Contato_Empresa.Visible = false;
            aba_depara.Visible = false;
            Contato_Importador.Visible = false;
            div_Ordem.Visible = false;
            div_btdesc.Visible = false;
            div_ImportarItens.Visible = false;
            div_btnItens.Visible = false;
            DIV_Fornecedor.Visible = false;
            DivTipoCompra.Visible = false;
            DIV_Item_dtPrevisaoEntrega.Visible = false;
            DIV_UnidadeEntrega.Visible = false;
            aba_OPI.Visible = false;
            lnkAddProduto.Visible = false;
            cblNome.Visible = false;
            Div_FormadeEnvio.Style["z-index"] = "8";
            MoedaCompra.Visible = false;
            MoedaCompra1.Visible = false;
            MoedaCompra2.Visible = false;
            MoedaCompra4.Visible = false;
            cblProdutoCliente.Visible = false;
            DIV_DocumentoEmpresa.Visible = false;
            DIV_DocumentoColaboradores.Visible = false;
            DIV_InserirDocumento.Visible = false;
            DIV_ColaboradorSTSO.Visible = false;
            DIV_txtnVlrTotal.Attributes["class"] = "form-group";
            DIV_Produtos.Attributes["class"] = "form-group";
            DIV_Servicos.Attributes["class"] = "form-group";
            DIV_Frete.Attributes["class"] = "form-group";
            DIV28.Visible = false;
            DIV29.Visible = false;
            DIV33.Visible = false;
            DIV34.Visible = false;
            DIV35.Visible = false;
            DIV44.Visible = false;
            DIV39.Visible = false;
            DIV_nControleTT.Visible = false;
            DIV_PedidoClienteFinal.Visible = false;
            aba_Cronograma.Visible = false;
            aba_Faturamento.Visible = false;
            MoedaFrete.Visible = false;
            MoedaFreteVendas.Visible = false;
            DIV_Instalador.Visible = false;
            divFrete.Visible = false;
            DIV_nDesconto.Visible = false;
            DIV_nVlrDesconto.Visible = false;
            txtnDesconto.ReadOnly = true;
            txtnVlrDesconto.ReadOnly = true;
            Div_FreteVendas.Visible = false;

            if (hddidTipo.Value == "2")
            {
                DIV_Zona.Visible = false;
                caixaTitulo.Visible = true;
                DIV_DADOS_NPedido.Visible = true;
                DIV_DADOS_Cliente.Visible = true;
                DIV_DADOS_Vendedor.Visible = true;
                DIV_DADOS_TipoFaturamento.Visible = true;
                DIV_Item_ValorUnitario.Visible = true;
                DADOS_lblAba01.Text = "Pedido";
                DADOS_lblData.Text = "Data do Pedido";
                BreadCrumb.TitulodaPagina = "Novo Pedido";
                Div_FormadeEnvio.Style["z-index"] = "8";
                lblTituloPagina.Text = string.Format("Novo Pedido Cliente: {0}", txtsRazaoSocial.Text);
                lblTituloSalvar.Text = "Confirma a inclusão do pedido do Cliente " + txtsRazaoSocial.Text + "?";
                lblFormadeEnvio.Text = "Forma de Envio";
                lblnVlrServicos.Text = "Valor Serviços";
                DIV_Referencia.Attributes["class"] = "col-lg-4";
                DIV16.Attributes["class"] = "col-lg-12";
                DIV15.Attributes["class"] = "col-lg-6";
                DIV17.Attributes["class"] = "row";
                DIV18.Attributes["class"] = "row";

                //Alteração - 26/01/2026 - Thiago Rodrigues
                div_EstimativaEntrega.Attributes["class"] = "col-lg-2";
                Div_FormadeEnvio.Attributes["class"] = "col-lg-6";
                DADOS_lblEstimativa.Text = "Est. Entrega";

                Div_FreteVendas.Visible = true;
                Div_FreteVendas.Attributes["class"] = "col-lg-3";
                div_nVlrFrete.Visible = false;

                Contato_Empresa.Visible = false;
                Contato_Importador.Visible = false;
                DIV_Importador.Visible = false;
                Div_PagoEnvio.Visible = false;
                aba_OPI.Visible = true;
                DIV_CodigoInput.Attributes["class"] = "form-group";
                aba_Documentos.Visible = false;
                DIV_Documentos.Visible = false;
                DIV_Documentos_Tipos.Visible = true;
                ExportarPDF.Visible = false;
                ExportarExcel.Visible = false;
                cblsCEST.Visible = false;
                cblsDecimal.Visible = false;
                cblsEnvio.Visible = false;
                cblsDrawback.Visible = false;
                cblsBancario.Visible = false;
                cblCarimbos_Aprovado.Visible = false;
                cblCarimbos_Original.Visible = false;
                cblCarimbos_CNPJ.Visible = false;
                cblAssinatura.Visible = false;
                cblNome.Visible = false;
                cblProdutoCliente.Visible = true;
                DIV_nControleTT.Visible = true;
                DIV_PedidoClienteFinal.Visible = true;
                aba_Cronograma.Visible = true;
                aba_Faturamento.Visible = ValidaPermissao(Permissao.Pedidos.Visualizar_Aba_Faturamento) && hddididStatus.Value != idStatus_Bloqueado;

                div_nVlrImpostos.Attributes["class"] = "col-lg-3";
                DIV_nVlrTotal.Attributes["class"] = "col-lg-3";
                DIV_Produtos.Attributes["class"] = "input-group";
                DIV_Servicos.Attributes["class"] = "input-group";
                DIV_txtnVlrTotal.Attributes["class"] = "input-group";
                DIV_Frete.Attributes["class"] = "input-group";
                MoedaCompra2.Visible = true;
                MoedaCompra1.Visible = true;
                MoedaCompra.Visible = true;
                MoedaFrete.Visible = true;
                MoedaFreteVendas.Visible = true;
                AplicarLayoutMoeda(false, false);
                DIV_Instalador.Visible = true;
                DIV_Instalador.Attributes["class"] = "col-lg-12";
                DIV_ddlInstalador.Attributes["class"] = "col-lg-6";
            }
            else if (hddidTipo.Value == "3")
            {
                caixaTitulo.Visible = false;
                DIV_DADOS_Origem.Visible = true;
                DIV_DADOS_Despachante.Visible = true;
                DIV_DADOS_Despacho.Visible = true;
                DIV_DADOS_Modal.Visible = true;
                DIV_DADOS_Importacao_Fornecedor.Visible = true;
                DIV_Comissao.Visible = false;
                DADOS_lblAba01.Text = "Importação";
                DADOS_lblData.Text = "Previsão Saída Origem";
                DADOS_lblEntrega.Text = "Previsão Chegada Destino";
                DADOS_lblEstimativa.Text = "Estimativa Entrega Importador";
                BreadCrumb.TitulodaPagina = "Nova Importação";
                lblTituloSalvar.Text = "Confirma a inclusão da Importação?";
                lblFormadeEnvio.Text = "Incoterm";
                Div_FormadeEnvio.Style["z-index"] = "8";
                lblnVlrServicos.Text = "Valor Envio";
                InvoiceNumber.Visible = true;
                aba_Resultado.Visible = true;
                DIV_OrderNumber.Visible = true;
                DIV_Referencia.Attributes["class"] = "col-lg-2";
                DivOrigem.Attributes["class"] = "col-lg-2";
                div_PrevisaoEntrega.Attributes["class"] = "col-lg-2 padd-0";
                Div_FormadeEnvio.Attributes["class"] = "col-lg-2";
                DIV_TermosPagamento.Visible = true;
                DIV_EnderecoEntrega.Attributes["class"] = "col-lg-4";
                DIV_Exportador.Visible = true;
                DIV_Despachante_C.Visible = true;
                Contato_Empresa.Visible = true;
                aba_depara.Visible = false;
                DIV_EMPRESA.Visible = false;
                Div_InlandF.Visible = false;
                Div_Handling.Visible = false;
                Div_Consular.Visible = false;
                BtnEdicaoPagamento.Visible = false;
                this.dtgPagamento.Columns[6].Visible = false;
                this.dtgPagamento.Columns[7].Visible = true;
                Div_PagoEnvio.Visible = true;
                Div_Garantia.Visible = false;
                this.gv_Garantia.Columns[0].Visible = true;
                this.gv_Garantia.Columns[6].Visible = false;
                this.gv_Garantia.Columns[7].Visible = true;
                DIV_20.Attributes["class"] = "col-lg-12";
                DIV_21.Attributes["class"] = "row";
                DIV_CodigoInput.Attributes["class"] = "form-group";
                cblCarimbos_Aprovado.Visible = false;
                cblCarimbos_CNPJ.Visible = false;
                div_Moeda.Visible = true;
                DIV_Departamento.Attributes["class"] = "col-lg-4";
            }
            else if (hddidTipo.Value == "6")
            {
                caixaTitulo.Visible = false;
                DIV_DADOS_Origem.Visible = true;
                DIV_DADOS_Despachante.Visible = true;
                DIV_DADOS_Despacho.Visible = true;
                DIV_DADOS_Modal.Visible = true;
                DIV_DADOS_Importacao_Fornecedor.Visible = true;
                DIV_Comissao.Visible = false;
                DADOS_lblAba01.Text = "Exportação";
                DADOS_lblData.Text = "Previsão Saída Origem";
                DADOS_lblEntrega.Text = "Previsão Chegada Destino";
                DADOS_lblEstimativa.Text = "Estimativa Entrega Exportador";
                BreadCrumb.TitulodaPagina = "Nova Exportação";
                lblTituloSalvar.Text = "Confirma a inclusão da Exportação?";
                lblFormadeEnvio.Text = "Incoterm";
                Div_FormadeEnvio.Style["z-index"] = "8";
                lblnVlrServicos.Text = "Valor Envio";
                InvoiceNumber.Visible = true;
                aba_Resultado.Visible = true;
                DIV_OrderNumber.Visible = true;
                DIV_Referencia.Attributes["class"] = "col-lg-2";
                DivOrigem.Attributes["class"] = "col-lg-2";
                div_PrevisaoEntrega.Attributes["class"] = "col-lg-2 padd-0";
                Div_FormadeEnvio.Attributes["class"] = "col-lg-2";
                DIV_TermosPagamento.Visible = true;
                DIV_EnderecoEntrega.Attributes["class"] = "col-lg-4";
                DIV_Exportador.Visible = true;
                DIV_Despachante_C.Visible = true;
                Contato_Empresa.Visible = true;
                aba_depara.Visible = false;
                DIV_EMPRESA.Visible = false;
                Div_InlandF.Visible = false;
                Div_Handling.Visible = false;
                Div_Consular.Visible = false;
                BtnEdicaoPagamento.Visible = false;
                this.dtgPagamento.Columns[6].Visible = false;
                this.dtgPagamento.Columns[7].Visible = true;
                Div_PagoEnvio.Visible = true;
                Div_Garantia.Visible = false;
                this.gv_Garantia.Columns[0].Visible = true;
                this.gv_Garantia.Columns[6].Visible = false;
                this.gv_Garantia.Columns[7].Visible = true;
                DIV_20.Attributes["class"] = "col-lg-12";
                DIV_21.Attributes["class"] = "row";
                DIV_CodigoInput.Attributes["class"] = "form-group";
                cblCarimbos_Aprovado.Visible = false;
                cblCarimbos_CNPJ.Visible = false;
                div_Moeda.Visible = true;
                DIV_Departamento.Attributes["class"] = "col-lg-4";
            }
            else if (hddidTipo.Value == "7")
            {
                DIV_Documentos.Visible = true;
                cblCarimbos_Aprovado.Visible = true;
                cblCarimbos_CNPJ.Visible = true;
                cblAssinatura.Visible = true;
                ExportarPDF.Visible = true;
                ExportarExcel.Visible = true;
                DIV_Zona.Visible = false;
                caixaTitulo.Visible = true;
                DIV_DADOS_NPedido.Visible = true;
                DIV_DADOS_Cliente.Visible = true;
                DIV_DADOS_Vendedor.Visible = true;
                DIV_DADOS_TipoFaturamento.Visible = true;
                DIV_Item_ValorUnitario.Visible = true;
                DADOS_lblAba01.Text = "Pedido de Compras";
                DADOS_lblData.Text = "Data do Pedido";
                BreadCrumb.TitulodaPagina = "Novo Pedido de Compras";
                lblTituloPagina.Text = string.Format("Novo Pedido Compras: {0}", txtsRazaoSocial.Text);
                lblTituloSalvar.Text = "Confirma a inclusão do pedido de Compras " + txtsRazaoSocial.Text + "?";
                lblFormadeEnvio.Text = "Forma de Envio";
                lblnVlrServicos.Text = "Valor Serviços";
                DIV_Referencia.Attributes["class"] = "col-lg-3";
                Div_FormadeEnvio.Style["z-index"] = "4";
                DIV16.Attributes.Remove("class");
                DIV17.Attributes.Remove("class");
                Contato_Empresa.Visible = false;
                Contato_Importador.Visible = false;
                DIV_Importador.Visible = false;
                Div_PagoEnvio.Visible = false;
                lblnPedido.Text = "Nº Pedido";
                DIV_Fornecedor.Visible = true;
                DIV_DADOS_Cliente.Visible = false;
                lblVendedor.Text = "Comprador";
                DIV_Comissao.Visible = false;
                div_produtosEnvios.Visible = true;
                DIV_nVlrTotal.Visible = true;
                DIV_nDesconto.Visible = true;
                DIV_nVlrDesconto.Visible = true;
                aba_LM.Visible = false;
                DIV_Valores.Attributes["class"] = "col-lg-8";
                DIV_nVlrProdutos.Attributes["class"] = "col-lg-3";
                DIV_nVlrServicos.Attributes["class"] = "col-lg-2";
                DIV_nVlrTotal.Attributes["class"] = "col-lg-3";
                DIV_Departamento.Attributes["class"] = "col-lg-4";
                DIV_STATUS_ATUAL.Attributes["class"] = "col-lg-6";
                DIV_DEPARTAMENTO_ATUAL.Attributes["class"] = "col-lg-6";
                DIV_DADOS_TipoFaturamento.Visible = false;
                DIV_EMPRESA.Attributes["class"] = "col-lg-2";
                Div_ddlEmpresa.Attributes["class"] = "col-lg-12";
                DIV_EnderecoEntrega.Attributes["class"] = "col-lg-4";
                lblnVlrServicos.Text = "Valor Frete";
                DivTipoCompra.Visible = true;
                DivCentroCustos.Visible = true;
                DivConceito.Visible = true;
                DIV_Conceitual.Visible = true;
                if (!ValidaPermissao(Compras.Pedido_Compras.IncluirNacional)) ddlsTipoCompra.Items.Remove(ddlsTipoCompra.Items.FindByValue("N"));
                if (!ValidaPermissao(Compras.Pedido_Compras.IncluirInternacional)) ddlsTipoCompra.Items.Remove(ddlsTipoCompra.Items.FindByValue("I"));
                div_Item_txtsDscProduto.Attributes["class"] = "col-lg-3";
                DIV_Item_Quantidade.Attributes["class"] = "col-lg-1";
                lbl_Item_Quantidade.InnerText = "Qtde";
                DIV_Item_ValorUnitario.Attributes["class"] = "col-lg-1";
                DIV_Item_ValorUnitario.Attributes["style"] = "padding: 0;";
                DIV_Unidade.Attributes["class"] = "col-lg-2";
                DIV_Item_dtPrevisaoEntrega.Visible = true;
                aba_Documentos.Visible = ValidaPermissao(Permissao.Compras.Pedido_Compras.Visualizar_Aba_Documentos);
                DIV_Documentos_Tipos.Visible = true;
                cblsCEST.Visible = false;
                cblsDecimal.Visible = false;
                cblsEnvio.Visible = false;
                cblsDrawback.Visible = false;
                cblsBancario.Visible = false;
                cblCarimbos_Original.Visible = false;
                IP.Visible = false;
                CO.Visible = false;
                PL.Visible = false;
                PI.Visible = false;
                CI.Visible = false;
                DIV_20.Attributes["class"] = "col-lg-12";
                DIV_21.Attributes["class"] = "row";

                if (ValidaPermissao(Compras.Pedido_Compras.AdicionarProdutos)) lnkAddProduto.Visible = true;
                else
                {
                    lnkAddProduto.Visible = false;
                    DIV_CodigoInput.Attributes["class"] = "form-group";
                }

                cblNome.Visible = true;
                divFrete.Visible = true;
            }
        }

        void LimpaCampos()
        {
            txtsReferencia.Text = "";
            txtdtPedido.Text = "";
            TextInvoiceNumber.Text = "";
            txtnOrderNumber.Text = "";
            txtsZonaEnvio.Text = "";
            txtdtEstimativaEntrega.Text = "";
            ddlCondPagamento.SelectedValue = "0";
            ddlTermosPagamento.SelectedValue = "0";
            ddlFluxoPedido.SelectedValue = "0";
            txtsObservacao.Text = "";
            txtsRazaoSocial.Text = "";
            txtnVlrProdutos.Text = "";
            txtnVlrServicos.Text = "";
            txtnVlrTotal.Text = "";
            ddlidTipoFaturamento.SelectedValue = "0";
            ddlidEmpresa.SelectedValue = "0";
            ddlImportador.SelectedValue = "0";
            ddlTipoEnvio.SelectedValue = "0";
            ddlOrdem.SelectedValue = "0";
            ddlidParceiro_Comissionador.SelectedValue = "0";
            txtnVlrComissao.Text = "";
            txtnControleTT.Text = "";
            ddlFornecedor.SelectedValue = "0";
            ddlsTipoCompra.SelectedValue = "0";
            ddlConceito.SelectedValue = "0";
            ddlCentroCusto.SelectedValue = "0";
            ddlGrupoPatrimonio.SelectedValue = "0";

            Item_LimpaCampos();
        }

        void AtualizarSimboloMoeda(string simbolo)
        {
            MoedaFrete.InnerText = simbolo;
            MoedaCompra3.InnerText = simbolo;
            MoedaCompra2.InnerText = simbolo;
            MoedaCompra1.InnerText = simbolo;
            MoedaCompra.InnerText = simbolo;
            MoedaFreteVendas.InnerText = simbolo;
            txtCustoAduaneiro.Grupo_Simbolo = simbolo;
            txtCustoDespachante.Grupo_Simbolo = simbolo;
        }

        void AplicarLayoutMoeda(bool exibirMoeda, bool modoEdicao)
        {
            div_Moeda.Visible = exibirMoeda;
            div_MoedaConsulta.Visible = false;
            div_CambioMoeda.Visible = exibirMoeda && modoEdicao;
            div_CustoAduaneiro.Visible = exibirMoeda;
            div_CustoDespachante.Visible = exibirMoeda;
            div_Moeda.Attributes["class"] = modoEdicao ? "col-lg-4" : "col-lg-2";
            div_MoedaSelecao.Attributes["class"] = modoEdicao ? "col-lg-6 padd-0" : "col-lg-12 padd-0";
            DIV_Comissao.Attributes["class"] = exibirMoeda ? "col-lg-4" : "col-lg-6";
        }

        void EditarPedido()
        {
            TextInvoiceNumber.ReadOnly = false;
            txtnOrderNumber.ReadOnly = false;
            txtsZonaEnvio.ReadOnly = false;
            PesquisarPedido(hddidPedido.Value.ToString(), "0", "0", "Editar", hddidTipo.Value);
            StatusDosCampos(false);
            VisibilidadeLinkProduto(true);
            cmdGravarPedido.Visible = true;
            ValoresDatas_cmdGravarPedido.Visible = true;
            cmdEditar.Visible = false;
            ValoresDatas_cmdEditar.Visible = false;
            cmdDuplicar.Visible = false;
            ValoresDatas_cmdDuplicar.Visible = false;
            aba_Arquivos.Visible = false;
            aba_Documentos.Visible = false;
            aba_Historico.Visible = false;
            aba_ArquivoMorto.Visible = false;
            aba_Tarefas.Visible = false;
            aba_Acoes.Visible = false;
            aba_LM.Visible = false;
            aba_depara.Visible = false;
            div_SelecaoItens.Visible = true;
            DIV_STATUS_ATUAL.Visible = false;
            div_PrevisaoEntrega.Visible = false;
            DIV_Departamento.Visible = false;
            div_EstimativaEntrega.Visible = false;
            txtCambio.ReadOnly = true;
            Div_CadastroPagamento.Visible = true;
            this.dtgPagamento.Columns[6].Visible = true;
            this.dtgPagamento.Columns[7].Visible = false;
            sClienteFinalswt.BloquearEdicao(false);
            ComissaoPaga.BloquearEdicao(false);
            txtFrete.ReadOnly = false;
            cmdGerarOPI.Visible = false;
            aba_Faturamento.Visible = false;
            aba_OPI.Visible = false;
            txtnFreteVendas.ReadOnly = false;

            if (hddidTipo.Value == "7")
            {
                ddlsTipoCompra.Attributes.Add("disabled", "disabled");
                div_PrevisaoEntrega.Visible = true;
                txtdtEstimativaEntrega.ReadOnly = true;
                aba_Documentos.Visible = false;
                txtnDesconto.ReadOnly = false;
                txtnVlrDesconto.ReadOnly = false;

                if (ddlFluxoPedido.SelectedValue == "19")
                {
                    ddlsEnderecoEntrega.Visible = false;
                    txtsEnderecoEntrega.Visible = true;
                    txtsEnderecoEntrega.ReadOnly = false;
                }
                else
                {
                    ddlsEnderecoEntrega.Visible = true;
                    txtsEnderecoEntrega.Visible = false;
                }
            }
            else
            {
                ddlsEnderecoEntrega.Visible = hddAltera_Endereco.Value.Equals("S");
                txtsEnderecoEntrega.Visible = !hddAltera_Endereco.Value.Equals("S");
            }

            if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
            {
                Div_PagoEnvio.Visible = true;
                txtnVlrProdutos.ReadOnly = true;
                div_Ordem.Visible = true;
                div_btdesc.Visible = true;
                div_ImportarItens.Visible = true;
            }

            if (hddidTipo.Value == "2")
            {
                DIV_Instalador.Attributes["class"] = "col-lg-12";
                DIV_ddlInstalador.Attributes["class"] = "col-lg-6";

                txtnTempoContrato.ReadOnly = false;
                ddlTipoPeriodo.Attributes.Remove("disabled");

                if (DIV_PedidoClienteFinal.Visible == false)
                {
                    DIV_Instalador.Attributes["class"] = "col-lg-6";
                    DIV_ddlInstalador.Attributes["class"] = "col-lg-6";
                }
            }

            Div_Garantia.Visible = true;
            this.gv_Garantia.Columns[0].Visible = true;
            this.gv_Garantia.Columns[6].Visible = true;
            this.gv_Garantia.Columns[7].Visible = false;
            div25.Visible = true;
            aba_STSO.Visible = false;
            aba_ART.Visible = false;
            aba_Cronograma.Visible = false;
        }

        private bool AplicarValidacoes()
        {
            decimal nValorPagamento_Info_Pag = 0;
            decimal ValorOriginal = 0;
            decimal nValorComDesconto = 0;
            decimal nValorAoLiquidar = 0;
            string sMensagem = "";

            if (decimal.TryParse(txtCambio.Text, out ValorOriginal))
            {
                foreach (var linha in bs_Pagamento)
                {
                    if (linha.sFuncao != "EXCLUIR_Pagamento")
                    {
                        nValorPagamento_Info_Pag += (linha.nValorPagamento_Info_Pag);
                    }
                }
            }

            if (txtsReferencia.Text.Length < 4)
            {
                sMensagem = "Informe uma Referência válida para o Pedido!";
            }

            if (!string.IsNullOrEmpty(txtdtPedido.Text))
            {
                DateTime resultado = DateTime.MinValue;
                if (!DateTime.TryParse(this.txtdtPedido.Text.Trim(), out resultado))
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "Data do Pedido inválida!";
                }
            }

            if (ddlFluxoPedido.SelectedValue == "0")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Fluxo (Tipo) do Pedido!";
            }
            if (hddidTipo.Value != "3" && hddidTipo.Value != "6")
            {
                if (ddlCondPagamento.SelectedValue == "0")
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione uma Condição de Pagamento!";
                }
            }

            if (ddlVendedor.SelectedValue == "0")
            {
                if (hddidTipo.Value == "7")
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Comprador!";
                }
                else
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Vendedor!";
                }
            }
            else if (ddlTermosPagamento.SelectedValue == "0")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione os termos de pagamento!";
            }

            if (!string.IsNullOrEmpty(txtdtEstimativaEntrega.Text))
            {
                DateTime resultado = DateTime.MinValue;
                if (!DateTime.TryParse(this.txtdtEstimativaEntrega.Text.Trim(), out resultado))
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "Estimativa de entrega inválida!";
                }
            }

            if (ddlTipoEnvio.SelectedValue == "0")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione a forma de envio!";
            }

            if (ddlidPais.SelectedValue == "0")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Pais de Origem!";
            }

            if (Base_Pedidos_Itens.Count == 0)
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Adicione itens ao pedido!";
            }

            var funcao = Base_Pedidos_Itens.FirstOrDefault(x => (x.sFuncao.ToString() == "INCLUIR ITEM" || x.sFuncao.ToString() == "ALTERAR_ITEM" || x.sFuncao.ToString() == "CONSULTA ITEM" || x.sFuncao.ToString() == "INCLUIR ITEM ALTERACAO" || x.sFuncao.ToString() == "Inserir_item_envio"));

            if (funcao == null)
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Adicione itens ao pedido!";
            }

            if (hddidTipo.Value == "7")
            {
                if (ddlFornecedor.SelectedValue == "0")
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Fornecedor!";
                }
                if (ddlsTipoCompra.SelectedValue == "0")
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Tipo de Compra!";
                }


                if (ddlConceito.SelectedValue != "0")
                {
                    if (hddidPedido.Value != "0" && !string.IsNullOrEmpty(hddidPedido.Value))
                    {
                        if (ddlGrupoPatrimonio.SelectedValue == "0" && Valida_Conceito())
                            sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione um Grupo de Patrimônio!";
                    }
                    else
                    {
                        if (ddlGrupoPatrimonio.SelectedValue == "0" && Valida_Conceito())
                            sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione um Grupo de Patrimônio!";
                    }
                }

                if (hddidPedido.Value != "0" && !string.IsNullOrEmpty(hddidPedido.Value))
                {

                    if (ddlCentroCusto.SelectedValue != "0")
                    {
                        hddidCentroCusto.Value = ddlCentroCusto.SelectedValue;
                    }

                    if (hddidCentroCusto.Value == "0")
                    {
                        sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Centro de Custo!";
                    }
                }
                else
                {
                    if (ddlCentroCusto.SelectedValue == "0" && DateTime.Parse(txtdtPedido.Text) > new DateTime(2025, 7, 14))
                    {
                        sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Centro de Custo!";
                    }
                }

                if (ddlidEmpresa.SelectedValue == "0")
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione a Empresa!";
                }
            }

            if (sClienteFinalswt.Recuperar() == "S")
            {
                if (ddlidClienteFinal.SelectedValue == "0")
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione se o cliente Final!";
            }

            if (hddidTipo.Value == "2")
            {
                if (txtnControleTT.Text == "")
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "Informe o N° Controle TT!";
                }
            }

            if (sMensagem != "")
            {
                MensagemPagina.MostraMensagem_Erro(sMensagem);
                return false;
            }

            return true;
        }

        bool Valida_Conceito()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR-FLAG-CONCEITO" },
                { "@idConceito", ddlConceito.SelectedValue }
            };
            DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametros, true);
            if (DATASET(ds, "sExibePatrimonio") == "S") return true;
            else return false;
        }

        void Item_LimpaCampos()
        {
            Item_txtsCodigoProduto.Text = "";
            Item_txtsDscProduto.Text = "";
            Item_txtnValorUnitario.Text = "";
            Item_ddlsUnidade.SelectedValue = "0";
            Item_txtnQuantidade.Text = "";
            Item_hddPesquisaPor.Value = "";
            Item_ddlsUnidadeEntrega.SelectedValue = "0";
        }

        #endregion

        #region | Sugestão

        protected void PopularRptSugestao(string idProduto, double qtdProduto)
        {
            DataSet ds;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            string sProcedure = "sp_Manipula_tbl_Flow_Pedidos";
            vParametros.Add("@sFuncao", "CONSULTAR_SUGESTAO");
            vParametros.Add("@idProduto", idProduto);

            ds = ExecutarDataSet(sProcedure, vParametros);

            if (ds.Tables[0].Rows.Count > 0)
            {
                AtualizarQuantidades(ds, qtdProduto);

                rptItensSugestao.DataSource = ds;
                rptItensSugestao.DataBind();

                //Abrir o modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalSugestao", "$('#modalSugestao').modal('show');", true);
            }
        }

        #region | Função de Imagem

        protected void rptItemSugerido_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Image img = (Image)e.Item.FindControl("imgProdutoPrincipal");

                TextBox txtIdProduto = e.Item.FindControl("txtIdProdutoSugestao") as TextBox;
                if (txtIdProduto != null)
                {
                    CarregaImgProduto(txtIdProduto.Text, img);
                }
            }
        }

        protected void CarregaImgProduto(string idProduto, Image img)
        {
            DataTable dsPesquisa;
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_IMAGEM" },
                { "@idTipoArquivo", "201" },
                { "@idObjeto", idProduto }
            };
            dsPesquisa = ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (dsPesquisa.Rows.Count > 0)
            {
                DataRow imgBd = dsPesquisa.Rows[0];
                string imgUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])imgBd["vbArquivo"]);
                img.ImageUrl = imgUrl;
                img.Visible = true;
            }
        }

        #endregion

        private void AtualizarQuantidades(DataSet ds, double qtdProduto)
        {
            //Calcula a Quantidade
            foreach (DataTable tabela in ds.Tables)
            {
                foreach (DataRow row in tabela.Rows)
                {
                    double qtdTotal = Convert.ToDouble(row["nQuantidade"]) * qtdProduto;
                    row["nQuantidade"] = qtdTotal;
                }
            }
        }

        protected void Adicionar_Click(object sender, EventArgs e)
        {
            AdicionarProdutos(ObterDadosRepeater());
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_CloseModal", "$('#modalSugestao').modal('hide'); $('body').removeClass('modal-open'); $('.modal-backdrop').remove(); $('[id$=Item_txtsCodigoProduto]').focus();", true);
        }

        protected void AdicionarProdutos(DataSet dsProdutos)
        {
            dtgItens_SalvarGRID();

            int idPedido = 0;
            string[] vidPedido = hddidPedido.Value.Split(',');
            string sFuncao = "INCLUIR ITEM";

            if (vidPedido[0].ToString() == "") idPedido = 0;
            else
            {
                idPedido = Convert.ToInt32(vidPedido[0].ToString());
                sFuncao = "INCLUIR ITEM ALTERACAO";
            }

            foreach (DataRow row in dsProdutos.Tables["Itens"].Rows)
            {
                double nValorReal = 0;
                double.TryParse(Item_txtnValorUnitario.Text, out double nValorUnitario);

                cls_Pedidos_Itens objItem = new cls_Pedidos_Itens
                {
                    idContador = Base_Pedidos_Itens.Count() + 1,
                    idPedido = idPedido,
                    idItem = 0,
                    sDscCategoriaVendas = Item_hddsDscCategoriaVendas.Value,
                    idProduto = Convert.ToInt32(row["idProdutoSugestao"]),
                    sCodigoProduto = row["sCodigo"].ToString(),
                    sDscProduto = row["sDscProduto"].ToString(),
                    sUnidade = row["sUnidade"].ToString(),
                    nQuantidade = Convert.ToDouble(row["nQuantidade"]),
                    sFuncao = sFuncao,
                    nValorUnitario = nValorUnitario,
                    idCNM = Convert.ToInt32(Item_hddidNCM.Value),
                    sCodigoNCM = Item_hddsCodigoNCM.Value.ToString(),
                    Importacao_dtPO = "",
                    Importacao_dtETA = "",
                    Importacao_dtETD = "",
                    nValorReal = nValorReal,
                    sTipoProduto_Servico = Item_hddsTipoProduto_Servico.Value
                };

                double.TryParse(txtCambioMoeda.Text, out double nCambio);
                if (hddidTipo.Value != "2" || !div_Moeda.Visible || nCambio <= 0)
                    nCambio = 1;

                objItem.nValorTotal = objItem.nQuantidade * objItem.nValorUnitario * nCambio;

                objItem.nValorRealTotal = Convert.ToDecimal(objItem.nQuantidade) * Convert.ToDecimal(objItem.nValorReal);
                objItem.nResultado = objItem.nValorRealTotal - Convert.ToDecimal(objItem.nValorTotal);

                if (hddidTipo.Value == "7" || hddidTipo.Value == "1")
                    objItem.nOrdem = Base_Pedidos_Itens.Count > 0 ? Base_Pedidos_Itens.OrderBy(i => i.nOrdem).Last().nOrdem + 10 : 10;
                else
                    objItem.nOrdem = Base_Pedidos_Itens.Where(c => c.sDscCategoriaVendas.ToString().ToUpper().Equals(Item_hddsDscCategoriaVendas.Value.ToUpper())).Count() + 1;

                if (hddidTipo.Value == "7")
                {
                    DateTime data = Convert.ToDateTime(txtdtPrevisaoEntrega.Text);
                    objItem.dtPrevisaoEntrega = data.ToString("dd/MM/yyyy");
                    objItem.nValorTblPreco = Convert.ToDecimal(Item_hddsValortblPreco.Value);
                    objItem.sUnidadeEntrega = row["sUnidade"].ToString();
                }

                Base_Pedidos_Itens.Add(objItem);
                hddsAlteracaoItens.Value = "S";
            }

            Item_LimpaCampos();
            DataBind_dtgItens("Editar");
            VisibilidadeLinkProduto(true);
        }

        protected DataSet ObterDadosRepeater()
        {
            //Criei para Obter os dados do repeater e adicionar na tabela
            DataSet ds = new DataSet();
            DataTable tabela = new DataTable("Itens");

            tabela.Columns.Add("idProdutoSugestao", typeof(int));
            tabela.Columns.Add("sCodigo", typeof(string));
            tabela.Columns.Add("sDscProduto", typeof(string));
            tabela.Columns.Add("sUnidade", typeof(string));
            tabela.Columns.Add("nQuantidade", typeof(double));

            foreach (RepeaterItem item in rptItensSugestao.Items)
            {
                if (Convert.ToDouble(((TextBox)item.FindControl("txtQuantidade")).Text) > 0)
                {
                    DataRow row = tabela.NewRow();

                    HiddenField hddsCodigo = (HiddenField)item.FindControl("hddsCodigo");
                    HiddenField hddsUnidade = (HiddenField)item.FindControl("hddsUnidade");
                    HiddenField hddsDscProduto = (HiddenField)item.FindControl("hddsDscProduto");
                    row["idProdutoSugestao"] = Convert.ToInt32(Convert.ToInt32(((TextBox)item.FindControl("txtIdProdutoSugestao")).Text));
                    row["sCodigo"] = hddsCodigo.Value;
                    row["sDscProduto"] = hddsDscProduto.Value;
                    row["sUnidade"] = hddsUnidade.Value;
                    row["nQuantidade"] = Convert.ToDouble(((TextBox)item.FindControl("txtQuantidade")).Text);
                    tabela.Rows.Add(row);
                }
            }
            ds.Tables.Add(tabela);

            return ds;
        }

        protected void Cancelar_Click(object sender, EventArgs e) => ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_CloseModal", "$('#modalSugestao').modal('hide'); $('body').removeClass('modal-open'); $('.modal-backdrop').remove(); $('[id$=Item_txtsCodigoProduto]').focus();", true);

        #endregion

        #region | Helper GroupHeader / Summary / ConvertSortDirectionToSql

        private void helper_GroupHeader(string groupName, object[] values, GridViewRow row)
        {
            row.BackColor = Color.FromArgb(102, 165, 200);
            row.Cells[0].Font.Bold = true;
            row.Cells[0].ForeColor = Color.White;
            row.Cells[0].Text = row.Cells[0].Text.ToUpper();
            row.Cells[0].HorizontalAlign = HorizontalAlign.Left;
        }

        private string ConvertSortDirectionToSql(SortDirection sortDirection)
        {
            string m_SortDirection = String.Empty;

            switch (sortDirection)
            {
                case SortDirection.Ascending:
                    m_SortDirection = "ASC";
                    break;

                case SortDirection.Descending:
                    m_SortDirection = "DESC";
                    break;
            }
            return m_SortDirection;
        }

        private void helper_Summary(string groupName, object[] values, GridViewRow row)
        {
            row.Cells[0].Font.Bold = true;
            row.Cells[1].Font.Bold = true;

            row.Cells[0].HorizontalAlign = HorizontalAlign.Left;
            row.Cells[0].Text = "Total";
        }

        public static DataTable ConvertTo<T>(IList<T> list)
        {
            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
            foreach (T item in list)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item);
                }
                table.Rows.Add(row);
            }
            return table;
        }

        public static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            return table;
        }

        #endregion

        #region | Detalhes Produto

        protected void VerDetalheProduto(object sender, CommandEventArgs e)
        {
            // Obtém o LinkButton que disparou o evento
            LinkButton lnkDetalhes = (LinkButton)sender;

            // Obtém o DataItem associado à linha da GridView
            GridViewRow row = (GridViewRow)lnkDetalhes.NamingContainer;
            HiddenField hddidProduto_ref = (HiddenField)row.FindControl("hddidProduto_ref");

            int idProduto = Convert.ToInt32(hddidProduto_ref.Value);

            DetalheModalProduto.PopularRptProduto(idProduto.ToString());

            DataBind_dtgItens(metodo);
            VisibilidadeLinkProduto(true);
        }

        protected void VisibilidadeLinkProduto(bool ativar)
        {
            // Iterar pelas linhas da GridView para encontrar o LinkButton
            foreach (GridViewRow row in dtgItens.Rows)
            {
                LinkButton lnkDetalhePtoduto = row.FindControl("lnkDetalhePtoduto") as LinkButton;
                Label lblDetalhePtoduto = row.FindControl("lblDetalhePtoduto") as Label;
                if (lnkDetalhePtoduto != null)
                {
                    // Definir a visibilidade do LinkButton
                    lnkDetalhePtoduto.Visible = ativar;
                    lblDetalhePtoduto.Visible = !ativar;
                }
            }
        }

        //Agnes Partal * 19/07/2024 --------------------------------------------//
        protected string MostrarLink(object idProduto, object sCodigoProduto)
        {
            if (ValidaPermissao(Produtos.Consultar))
            {
                string url = "/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id=" + idProduto.ToString();
                return $"<a href='{url}' target='_blank'>{sCodigoProduto}</a>";
            }
            else return sCodigoProduto.ToString();
        }
        //----------------------------------------------------------------------//

        #endregion

        #endregion

        #region | Gravar / Duplicar Pedido

        void DuplicarPedido()
        {
            PesquisarPedido(hddidPedido.Value.ToString(), "0", "0", "Duplicar", hddidTipo.Value);
            StatusDosCampos(false);

            BreadCrumb.TitulodaPagina = "Duplicar Pedido";
            lblTituloPagina.Text = string.Format("Duplicar Pedido Cliente: {0}", txtsRazaoSocial.Text);
            lblTituloSalvar.Text = "Confirma a inclusão do pedido do Cliente " + txtsRazaoSocial.Text + "?";
            DIV_nVlrProdutos.Visible = true;
            DIV_nVlrServicos.Visible = true;
            DIV_nVlrTotal.Visible = true;
            hddsDuplicar.Value = "S";

            if (hddidTipo.Value == "2")
            {
                DIV_Comissao.Visible = true;
                div_ComissaoValor.Visible = true;
                div_ComissaoPaga.Attributes["class"] = div_ComissaoPaga.Attributes["class"].Replace("visible", "").Trim();
            }

            cmdGravarPedido.Text = "Incluir";
            ValoresDatas_cmdGravarPedido.Text = "Incluir";
            cmdGravarPedido.Visible = true;
            ValoresDatas_cmdGravarPedido.Visible = true;
            Resultados_cmdGravarPedido.Text = "Incluir";
            Resultados_cmdGravarPedido.Visible = true;
            Popula_Combo(ddlsEnderecoEntrega, string.Format("sp_Manipula_tbl_Flow_Clientes 'CONSULTAR_ENDERECO', {0}", hddidCliente.Value), "idEndereco", "sEnderecoCompleto", false);
            ddlsEnderecoEntrega.Visible = true;
            txtsEnderecoEntrega.Visible = false;

            this.dtgItens.Columns[Col_nQuantidade_Edit].Visible = true;
            this.dtgItens.Columns[Col_nQuantidade_View].Visible = false;
            this.dtgItens.Columns[Col_nValorUnitario_Edit].Visible = true;
            this.dtgItens.Columns[Col_nValorUnitario_View].Visible = false;
            this.dtgItens.Columns[Col_BotaoExcluir].Visible = true;

            if (hddidTipo.Value != "3" || hddidTipo.Value != "6")
                this.dtgItens.Columns[Col_sCodigoNCM].Visible = false;

            txtdtPedido.Text = DateTime.Today.ToString("dd/MM/yyyy");
            PainelAtualizacao.Visible = false;
            div_SelecaoItens.Visible = true;
            hddidPedido.Value = "";
            cmdGravarPedido.Visible = true;
            ValoresDatas_cmdGravarPedido.Visible = true;
            cmdEditar.Visible = false;
            ValoresDatas_cmdEditar.Visible = false;
            cmdDuplicar.Visible = false;
            ValoresDatas_cmdDuplicar.Visible = false;
            aba_Arquivos.Visible = false;
            aba_Historico.Visible = false;
            aba_ArquivoMorto.Visible = false;
            aba_Tarefas.Visible = false;
            aba_Acoes.Visible = false;
            aba_LM.Visible = false;
            aba_ART.Visible = false;
            aba_depara.Visible = false;
            aba_STSO.Visible = false;
            aba_Documentos.Visible = false;
            div_SelecaoItens.Visible = true;
            DIV_STATUS_ATUAL.Visible = false;
            div_PrevisaoEntrega.Visible = false;
            DIV_Departamento.Visible = false;
            cmdGerarOPI.Visible = false;

            if (hddidTipo.Value == "7")
            {
                Popula_Combo(ddlsEnderecoEntrega, string.Format("sp_Manipula_tbl_Flow_Clientes 'CONSULTAR_ENDERECO_COMPRAS', {0}", ddlidEmpresa.SelectedValue), "idEndereco", "sEnderecoCompleto", false);

                if (ddlFluxoPedido.SelectedValue == "19")
                {
                    txtsEnderecoEntrega.Visible = true;
                    txtsEnderecoEntrega.ReadOnly = false;
                    ddlsEnderecoEntrega.Visible = false;
                }
                else
                {
                    txtsEnderecoEntrega.Visible = false;
                    ddlsEnderecoEntrega.Visible = true;
                }

                ddlsEnderecoEntrega.SelectedValue = hddEndereco.Value;
            }

            if (sClienteFinalswt.Recuperar() == "S")
            {
                sClienteFinalswt.Definir("S", "Tem cliente Final ?", "");
                div_idClienteFinal.Visible = true;
            }
            else
            {
                sClienteFinalswt.Definir("N", "Tem cliente Final ?", "");
                div_idClienteFinal.Visible = false;
            }
        }

        void GravarPedido()
        {
            string sFuncao = "INCLUIR PEDIDO";
            string sAlteracaoItens = "N";
            string sErro = "";

            if (AplicarValidacoes())
            {
                try
                {
                    dtgItens_SalvarGRID();

                    if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
                        gvImportacao_Volumes_SalvarGRID();

                    VisibilidadeLinkProduto(true);

                    string[] vidPedido = hddidPedido.Value.Split(',');
                    string idPedido = vidPedido[0].ToString();
                    string idTipo = hddidTipo.Value;

                    sFuncao = idPedido == "" ? "INCLUIR PEDIDO" : "ALTERAR PEDIDO";

                    if (bs_Garantia.Where(c => c.sFuncao.ToString() != "CONSULTAR GARANTIA").Any()) sAlteracaoItens = "S";
                    else if (Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "CONSULTA ITEM").Any()) sAlteracaoItens = "S";
                    else if (Base_Pedidos_Envios.Where(c => c.sFuncao.ToString() != "CONSULTAR ENVIOS").Any()) sAlteracaoItens = "S";
                    else if (Base_Pedidos_Volumes.Where(c => c.sFuncao.ToString() != "CONSULTAR VOLUMES").Any()) sAlteracaoItens = "S";

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", sFuncao },
                        { "@idTipo", idTipo },
                        { "@idPedido", idPedido },
                        { "@idUsuarioInclusao", Variaveis.idUsuario() },
                        { "@sReferencia", txtsReferencia.Text.ToString() },
                        { "@dtPedido", txtdtPedido.Text },
                        { "@dtEstimativaEntrega", txtdtEstimativaEntrega.Text.ToUpper() },
                        { "@idFluxo", ddlFluxoPedido.SelectedValue },
                        { "@idVendedor", ddlVendedor.SelectedValue },
                        { "@idCondicaoDePagamento", ddlCondPagamento.SelectedValue },
                        { "@idEmpresa", ddlidEmpresa.SelectedValue },
                        { "@idTipoEnvio", ddlTipoEnvio.SelectedValue },
                        { "@nVlrProdutos", Conversoes.Numerico(txtnVlrProdutos) },
                        { "@nVlrServicos", Conversoes.Numerico(txtnVlrServicos) },
                        { "@nControleTT", txtnControleTT.Text },
                        { "@sClienteFinal", sClienteFinalswt.Recuperar() },
                        { "@idTipoFaturamento", ddlidTipoFaturamento.SelectedValue },
                        { "@idParceiro_Comissionador", ddlidParceiro_Comissionador.SelectedValue },
                        { "@nVlrComissao", Conversoes.Numerico(txtnVlrComissao) },
                        { "@sComissaoPaga", ComissaoPaga.Recuperar() },
                        { "@sObservacao", txtsObservacao.Text },
                        { "@sAlteracaoItens", sAlteracaoItens },
                        { "@sidDuplicado", hddDupliacado.Value },
                        { "@idInstalador", ddlidInstalador.SelectedValue }
                    };

                    if (hddidTipo.Value == "7")
                    {
                        vParametros.Add("@idCliente", ddlFornecedor.SelectedValue);
                        vParametros.Add("@sTipoCompra", ddlsTipoCompra.SelectedValue);
                        vParametros.Add("@idTipoMoeda", ddlsMoeda.SelectedValue);
                        vParametros.Add("@nDesc", Conversoes.Numerico(txtnDesconto));
                        vParametros.Add("@nVlrDesconto", Conversoes.Numerico(txtnVlrDesconto));

                        if (ddlConceito.SelectedValue != "0" && !string.IsNullOrEmpty(ddlConceito.SelectedValue))
                            vParametros.Add("@idConceito", ddlConceito.SelectedValue);
                        else
                            vParametros.Add("@idConceito", hddidConceito.Value);

                        if (ddlCentroCusto.SelectedValue != "0" && !string.IsNullOrEmpty(ddlCentroCusto.SelectedValue))
                            vParametros.Add("@idCentroCusto", ddlCentroCusto.SelectedValue);
                        else
                            vParametros.Add("@idCentroCusto", hddidCentroCusto.Value);

                        if (ddlGrupoPatrimonio.SelectedValue != "0" && !string.IsNullOrEmpty(ddlGrupoPatrimonio.SelectedValue))
                            vParametros.Add("@idGrupoPatrimonio", ddlConceito.SelectedValue == "4" ? ddlGrupoPatrimonio.SelectedValue : "0");
                        else
                            vParametros.Add("@idGrupoPatrimonio", ddlConceito.SelectedValue == "4" ? hddidGrupoPatrimonio.Value : "0");

                        vParametros.Add("@idFrete", ddlFrete.SelectedValue);
                    }
                    else
                    {
                        vParametros.Add("@idCliente", hddidCliente.Value);

                        if (hddidTipo.Value == "2")
                        {
                            vParametros.Add("@idTipoMoeda", ddlsMoeda.SelectedValue);
                            vParametros.Add("@nCusto_Aduaneiro", txtCustoAduaneiro.Text.StringToDecimalString());
                            vParametros.Add("@nCusto_Despachante", txtCustoDespachante.Text.StringToDecimalString());
                        }
                    }

                    if (hddidTipo.Value == "7") vParametros.Add("@sPedidoCompras", txtsPedidoCliente.Text);
                    else vParametros.Add("@sPedidoCliente", txtsPedidoCliente.Text);

                    if (sClienteFinalswt.Recuperar() == "S") vParametros.Add("@idClienteFinal", ddlidClienteFinal.SelectedValue);
                    else vParametros.Add("@idClienteFinal", "0");

                    if (hddidTipo.Value == "2")
                    {
                        if (int.TryParse(hddidPedido_Vinculado.Value, out int idPedido_Vinculado) && idPedido_Vinculado > 0)
                            vParametros.Add("@idPedido_Vinculado", hddidPedido_Vinculado.Value);

                        try
                        {
                            string[] partes = ddlsEnderecoEntrega.SelectedItem.ToString().Split(',');
                            string[] Rua = partes[0].Split('-');
                            vParametros.Add("@sLogradouro", Rua[1]);

                            vParametros.Add("@nDiasContrato", txtnTempoContrato.Text);
                            vParametros.Add("@sTipoPeriodoContrato", ddlTipoPeriodo.SelectedValue);
                            vParametros.Add("@nVlrDIFAL", txtImposto_DIFAL.Text.Replace(".", "").Replace(",", "."));
                            vParametros.Add("@nVlrST", txtImposto_ST.Text.Replace(".", "").Replace(",", "."));

                            //vParametros.Add("@nFretePrevisto", Math.Round(decimal.Parse(string.IsNullOrEmpty(txtFrete.Text) ? "0,00" : txtFrete.Text), 2).ToString().Replace(",", "."));
                            vParametros.Add("@nFretePrevisto", Math.Round(decimal.Parse(string.IsNullOrEmpty(txtnFreteVendas.Text) ? "0,00" : txtnFreteVendas.Text), 2).ToString().Replace(",", "."));
                        }
                        catch { }
                    }

                    if (ddlsEnderecoEntrega.SelectedValue != "")
                    {
                        if (hddidTipo.Value == "7")
                        {
                            if (ddlFluxoPedido.SelectedValue == "19")
                            {
                                vParametros.Add("@sLogradouro", txtsEnderecoEntrega.Text);
                                vParametros.Add("@idEnderecoEntrega", "-1");
                            }
                            else
                            {
                                vParametros.Add("@sLogradouro", "0");
                                vParametros.Add("@idEnderecoEntrega", ddlsEnderecoEntrega.SelectedValue);
                            }
                        }
                        else vParametros.Add("@idEnderecoEntrega", ddlsEnderecoEntrega.SelectedValue);
                    }
                    else vParametros.Add("@idEnderecoEntrega", hddEndereco.Value);

                    // Importação -- Exportação
                    if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
                    {
                        vParametros.Add("@idModal", ddlidModal.SelectedValue);
                        vParametros.Add("@Importacao_idDespacho", ddlImportacao_idDespacho.SelectedValue);
                        vParametros.Add("@Importacao_idPais", ddlidPais.SelectedValue);
                        vParametros.Add("@Importacao_idFornecedor", ddlImportacao_idFornecedor.SelectedValue);
                        vParametros.Add("@Importacao_idImportador", ddlImportador.SelectedValue);
                        vParametros.Add("@Importacao_idDespachante", ddlImportacao_idDespachante.SelectedValue);
                        vParametros.Add("@idContato_Exportador", ddlContato_Exportador.SelectedValue);
                        vParametros.Add("@idContato_Empresa", ddlContato_Empresa.SelectedValue);
                        vParametros.Add("@idContato_Despachante", ddlContato_Despachante.SelectedValue);
                        vParametros.Add("@idContato_Importador", ddlContato_Importador.SelectedValue);
                        vParametros.Add("@sEditarEnvios", Envio_hddEditarEnvios.Value);
                        vParametros.Add("@nInlandF", Conversoes.Numerico(txtnInlandF));
                        vParametros.Add("@nHandling", Conversoes.Numerico(txtnHandling));
                        vParametros.Add("@nConsular", Conversoes.Numerico(txtnConsular));
                        vParametros.Add("@nOcean_Air", Conversoes.Numerico(txtnOcean_Air));
                        vParametros.Add("@nInsurance", Conversoes.Numerico(txtnInsurance));
                        vParametros.Add("@nOther_Charges", Conversoes.Numerico(txtnOther_Charges));
                        vParametros.Add("@Importacao_idCondicaoDePagamento", ddlTermosPagamento.SelectedValue);
                        vParametros.Add("@sInvoiceNumber", TextInvoiceNumber.Text);
                        vParametros.Add("@nOrderNumber", txtnOrderNumber.Text);
                        vParametros.Add("@sZonaEnvio", txtsZonaEnvio.Text);
                        vParametros.Add("@idEnvioPago", ddlEnvio.SelectedValue);
                        vParametros.Add("@idTipoMoeda", ddlsMoeda.SelectedValue);

                        if (txtPago.Text != "") vParametros.Add("@nValorPago", txtPago.Text.Replace(',', '.'));

                        decimal nOcean = 0;
                        decimal Insurance = 0;
                        decimal Other_Charges = 0;

                        if (txtnOcean_Air.Text != "" || txtnInsurance.Text != "" || txtnOther_Charges.Text != "")
                        {
                            nOcean = decimal.Parse(txtnOcean_Air.Text);
                            Insurance = decimal.Parse(txtnInsurance.Text);
                            Other_Charges = decimal.Parse(txtnOther_Charges.Text);
                        }

                        decimal totalenvio = nOcean + Insurance + Other_Charges;
                        string saldototalenvio = totalenvio.ToString().Replace(',', '.');
                        vParametros.Add("@nTotalEnvio", saldototalenvio);

                        if (ddlEnvio.SelectedValue == "1")
                        {
                            vParametros.Add("@nValorEnvio", "0");
                            vParametros.Add("@nValorEnvioReal", "0");
                            vParametros.Add("@nResultadoEnvio", "0");
                        }

                        if (txtCambio.Text != "") vParametros.Add("@nValorOriginal", Conversoes.Numerico(txtCambio));

                        decimal cambio = 0;
                        decimal pago = 0;
                        if (txtCambio.Text != "") cambio = decimal.Parse(txtCambio.Text);
                        if (txtPago.Text != "") pago = decimal.Parse(txtPago.Text);

                        decimal saldo = pago - cambio;
                        vParametros.Add("@nSaldo", saldo.ToString().Replace(',', '.'));
                    }

                    DataSet dsGravar = ExecutarDataSet(sProcedure, vParametros);

                    if (ValidarDataSet(dsGravar, out sErro))
                    {
                        idPedido = DATASET(dsGravar, "idPedido");

                        if (Salvar_Pagamento(idPedido) && Salvar_Garantia(idPedido))
                            Session["SalvoComSucesso"] = true;

                        if (GravarPedido_Itens(idPedido))
                        {
                            if (sFuncao == "INCLUIR PEDIDO") ExecutarDataSet(sProcedure, new Dictionary<string, string> { { "@sFuncao", "GRAVAR_FATURAMENTOS" }, { "@idPedido", idPedido } });

                            if (GravarPedido_Envios(idPedido) && GravarPedido_Volumes(idPedido))
                            {
                                if (hddsDuplicar.Value == "N")
                                {
                                    PesquisarPedido(idPedido, "0", "0", "Gravar", hddidTipo.Value);
                                    MensagemPagina.MostraMensagem_Sucesso("Pedido gravado com sucesso!");
                                }
                                else
                                    Response.Redirect("Pedidos_Detalhe.aspx?id=" + idPedido + "&sTp=3&sMsg=Pedido gravado com sucesso");
                            }
                        }
                    }
                    else throw new Exception("BD: " + sErro.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                    DataBind_dtgItens("Editar");
                    VisibilidadeLinkProduto(true);
                }

                RegistraScript("");
            }
            else
            {
                DataBind_dtgItens("Editar");
                VisibilidadeLinkProduto(true);
            }
        }

        #endregion

        #region | ReportViewer

        #region | COMEX

        void GerarCertificateOrigin(bool bPDF)
        {
            try
            {
                ReportViewer rv = new ReportViewer();

                rv.ProcessingMode = ProcessingMode.Local;
                rv.LocalReport.EnableExternalImages = true;

                rv.LocalReport.ReportPath = "App\\Reports\\" + "Origin.rdlc";

                Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>
                {
                    { "@idPedido", hddidPedido.Value }
                };
                DataSet dtProduct1 = ExecutarDataSet("sp_Manipula_tbl_Flow_Pedido_PDF_Commercial_Invoice", vParametrosProduct);

                var dtPedido = DateTime.Parse(dtProduct1.Tables[0].Rows[0]["dtPedido"].ToString());
                var sReferencia = dtProduct1.Tables[0].Rows[0]["sReferencia"].ToString();
                var sMoeda = dtProduct1.Tables[0].Rows[0]["sMoeda"].ToString();
                decimal nExWorks = 0;

                foreach (DataRow row in dtProduct1.Tables[1].Rows)
                {
                    nExWorks += decimal.Parse(row["nValorTotal"].ToString());

                    row["nValorTotal"] = Math.Round(decimal.Parse(row["nValorTotal"].ToString()), 2);

                    row["nValorUnitario"] = Math.Round(decimal.Parse(row["nValorUnitario"].ToString()), 2);
                }

                nExWorks = Math.Round(nExWorks, 2);

                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtProduct1.Tables[0]));
                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", dtProduct1.Tables[1]));

                string assinatura = " ";
                string carimbo_original = " ";

                if (cblAssinatura.Checked)
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-USUARIO" },
                        { "@idUsuarioIntegrado", Variaveis.idUsuario().Trim() }
                    };
                    DataTable dt = ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_Area", vParametros);

                    if (dt.Rows.Count > 0)
                        assinatura = Convert.ToBase64String(dt.Rows[0]["vbAssinatura"] as byte[]);
                }

                if (cblCarimbos_Original.Checked)
                {
                    var carimbo = GerarCarimbos("Original");

                    if (carimbo != null)
                        carimbo_original = Convert.ToBase64String(carimbo.ToArray());
                }

                ReportParameter[] rp = new ReportParameter[4];

                rp[0] = new ReportParameter("DataEmissao", DateTime.Today.ToString("dd/MM/yyyy"));
                rp[1] = new ReportParameter("CEST", cblsCEST.Checked.ToString());
                rp[2] = new ReportParameter("Assinatura", string.IsNullOrEmpty(assinatura) ? " " : assinatura);
                rp[3] = new ReportParameter("Carimbo_Original", string.IsNullOrEmpty(carimbo_original) ? " " : carimbo_original);

                rv.LocalReport.SetParameters(rp);

                rv.LocalReport.Refresh();

                byte[] bytesOrigin = bPDF ? rv.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings) : rv.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                string sNomeArquivoOrigin = "CertificateOrigin_" + dtProduct1.Tables[0].Rows[0]["nNumeroPedido"].ToString().PadLeft(6, '0') + "_" + CarimboDataHora() + ".pdf";

                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivoOrigin, bytesOrigin);

                Base_Pedidos_PDF.Add(sNomeArquivoOrigin);
            }
            catch (Exception ex)
            {
                MensagemPagina_Documentos.MostraMensagem_Erro("Erro ao gerar o Certificate of Origin! </br>" + ex.Message + (ex.InnerException == null ? "" : "<br />" + ex.InnerException.Message));
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Documentos", "$('#documentos-tab').tab('show');", true);
        }

        void GerarPackinglist(bool bPDF)
        {
            try
            {
                ReportViewer rv = new ReportViewer();

                rv.ProcessingMode = ProcessingMode.Local;
                rv.LocalReport.EnableExternalImages = true;

                rv.LocalReport.ReportPath = "App\\Reports\\" + "Packing.rdlc";

                Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>
                {
                    { "@idPedido", hddidPedido.Value }
                };

                DataSet dtProduct1;
                dtProduct1 = ExecutarDataSet("sp_Manipula_tbl_Flow_Pedido_PDF_Commercial_Invoice", vParametrosProduct);

                var dtPedido = DateTime.Parse(dtProduct1.Tables[0].Rows[0]["dtPedido"].ToString());
                var dtEstimativaEntrega = DateTime.Parse(dtProduct1.Tables[0].Rows[0]["dtEstimativaEntrega"].ToString());
                var sReferencia = dtProduct1.Tables[0].Rows[0]["sReferencia"].ToString();
                var sMoeda = dtProduct1.Tables[0].Rows[0]["sMoeda"].ToString();
                var Importacao_idFornecedor = dtProduct1.Tables[0].Rows[0]["Importacao_idFornecedor"].ToString();
                decimal nExWorks = 0;
                decimal SomaNetoTotal = 0;
                decimal nVolume = 0;
                decimal nPesoEnvio = 0;
                string totalSkids = dtProduct1.Tables[2].Rows.Count > 0
                    ? dtProduct1.Tables[2].Rows[dtProduct1.Tables[2].Rows.Count - 1]["nOrdemEnvio"].ToString()
                    : "0";

                foreach (DataRow row in dtProduct1.Tables[1].Rows)
                {
                    nExWorks += decimal.Parse(row["nValorTotal"].ToString());

                    SomaNetoTotal += decimal.Parse(row["nPesoNetoTotal"].ToString());

                    row["nValorTotal"] = Math.Round(decimal.Parse(row["nValorTotal"].ToString()), 2);

                    row["nValorUnitario"] = Math.Round(decimal.Parse(row["nValorUnitario"].ToString()), 2);

                    row["nPesoNetoTotal"] = Math.Round(decimal.Parse(row["nPesoNetoTotal"].ToString()), 4);
                }

                foreach (DataRow row in dtProduct1.Tables[2].Rows)
                {
                    nVolume += decimal.Parse(row["nVolume"].ToString());
                    nPesoEnvio += decimal.Parse(row["nPesoEnvio"].ToString());
                }

                nExWorks = Math.Round(nExWorks, 2);
                SomaNetoTotal = Math.Round(SomaNetoTotal, 2);
                nVolume = Math.Round(nVolume, 2);
                nPesoEnvio = Math.Round(nPesoEnvio, 2);

                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtProduct1.Tables[0]));
                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", dtProduct1.Tables[1]));
                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet3", dtProduct1.Tables[2]));

                string assinatura = " ";
                string carimbo_original = " ";

                if (cblAssinatura.Checked)
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-USUARIO" },
                        { "@idUsuarioIntegrado", Variaveis.idUsuario().Trim() }
                    };
                    DataTable dt = ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_Area", vParametros);

                    if (dt.Rows.Count > 0)
                        assinatura = Convert.ToBase64String(dt.Rows[0]["vbAssinatura"] as byte[]);
                }

                if (cblCarimbos_Original.Checked)
                {
                    var carimbo = GerarCarimbos("Original");

                    if (carimbo != null)
                        carimbo_original = Convert.ToBase64String(carimbo.ToArray());
                }

                ReportParameter[] rp = new ReportParameter[10];

                rp[0] = new ReportParameter("dtPedido", dtPedido.ToString("dd/MM/yyyy"));
                rp[1] = new ReportParameter("dtEstimativaEntrega", dtEstimativaEntrega.ToString("dd/MM/yyyy"));
                rp[2] = new ReportParameter("SomaNetoTotal", SomaNetoTotal.ToString());
                rp[3] = new ReportParameter("Importacao_idFornecedor", Importacao_idFornecedor.ToString() == null || Importacao_idFornecedor.ToString() == string.Empty ? " " : Importacao_idFornecedor.ToString());
                rp[4] = new ReportParameter("sReferencia", sReferencia.ToString());
                rp[5] = new ReportParameter("nVolume", nVolume.ToString());
                rp[6] = new ReportParameter("nPesoEnvio", nPesoEnvio.ToString());
                rp[7] = new ReportParameter("TotalSkids", totalSkids.ToString());

                rp[8] = new ReportParameter("Assinatura", string.IsNullOrEmpty(assinatura) ? " " : assinatura);
                rp[9] = new ReportParameter("Carimbo_Original", string.IsNullOrEmpty(carimbo_original) ? " " : carimbo_original);

                rv.LocalReport.SetParameters(rp);

                rv.LocalReport.Refresh();

                byte[] bytesPackingList = bPDF ? rv.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings) : rv.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                string sNomeArquivoPackingList = "PackingList_" + dtProduct1.Tables[0].Rows[0]["nNumeroPedido"].ToString().PadLeft(6, '0') + "_" + CarimboDataHora() + ".pdf";
                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivoPackingList, bytesPackingList);

                Base_Pedidos_PDF.Add(sNomeArquivoPackingList);
            }
            catch (Exception ex)
            {
                MensagemPagina_Documentos.MostraMensagem_Erro("Erro ao gerar o Packing List! </br>" + ex.Message + (ex.InnerException == null ? "" : "<br />" + ex.InnerException.Message));
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Documentos", "$('#documentos-tab').tab('show');", true);
        }

        void GerarProformaInvoice(bool bPDF)
        {
            try
            {
                ReportViewer rv = new ReportViewer();

                rv.ProcessingMode = ProcessingMode.Local;
                rv.LocalReport.EnableExternalImages = true;

                rv.LocalReport.ReportPath = "App\\Reports\\" + "ProformaInvoice.rdlc";

                Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>
                {
                    { "@idPedido", hddidPedido.Value }
                };

                DataSet dtProduct1;
                dtProduct1 = ExecutarDataSet("sp_Manipula_tbl_Flow_Pedido_PDF_Commercial_Invoice", vParametrosProduct);

                var dtPedido = DateTime.Parse(dtProduct1.Tables[0].Rows[0]["dtPedido"].ToString());
                var sReferencia = dtProduct1.Tables[0].Rows[0]["sReferencia"].ToString();
                var sMoeda = dtProduct1.Tables[0].Rows[0]["sMoeda"].ToString();
                decimal nExWorks = 0;
                decimal Total = 0;

                foreach (DataRow row in dtProduct1.Tables[1].Rows)
                {
                    nExWorks += decimal.Parse(row["nValorTotal"].ToString());

                    row["nValorTotal"] = Math.Round(decimal.Parse(row["nValorTotal"].ToString()), 2);

                    row["nValorUnitario"] = Math.Round(decimal.Parse(row["nValorUnitario"].ToString()), 2);
                }

                nExWorks = Math.Round(nExWorks, 2);

                string assinatura = " ";
                string carimbo_original = " ";

                if (cblAssinatura.Checked)
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-USUARIO" },
                        { "@idUsuarioIntegrado", Variaveis.idUsuario().Trim() }
                    };
                    DataTable dt = ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_Area", vParametros);

                    if (dt.Rows.Count > 0)
                        assinatura = Convert.ToBase64String(dt.Rows[0]["vbAssinatura"] as byte[]);
                }

                if (cblCarimbos_Original.Checked)
                    carimbo_original = Convert.ToBase64String(GerarCarimbos("Original").ToArray());

                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtProduct1.Tables[0]));
                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", dtProduct1.Tables[1]));

                ReportParameter[] rp = new ReportParameter[12];

                Total += nExWorks + decimal.Parse(dtProduct1.Tables[0].Rows[0]["nInlandF"].ToString()) + decimal.Parse(dtProduct1.Tables[0].Rows[0]["nHandling"].ToString()) + decimal.Parse(dtProduct1.Tables[0].Rows[0]["nConsular"].ToString()) + decimal.Parse(dtProduct1.Tables[0].Rows[0]["nOcean_Air"].ToString()) + decimal.Parse(dtProduct1.Tables[0].Rows[0]["nInsurance"].ToString()) + decimal.Parse(dtProduct1.Tables[0].Rows[0]["nOther_Charges"].ToString());
                rp[0] = new ReportParameter("Total", Total.ToString());
                rp[1] = new ReportParameter("ExWorks", nExWorks.ToString());
                rp[2] = new ReportParameter("dtPedido", dtPedido.ToString("dd/MM/yyyy"));
                rp[3] = new ReportParameter("sReferencia", string.IsNullOrEmpty(sReferencia) ? " " : sReferencia);
                rp[4] = new ReportParameter("sMoeda", string.IsNullOrEmpty(sMoeda) ? " " : sMoeda);
                rp[5] = new ReportParameter("DataEmissao", DateTime.Today.ToString("dd/MM/yyyy"));
                rp[6] = new ReportParameter("CheckBoxParameter", cblsDecimal.Checked.ToString());
                rp[7] = new ReportParameter("Envio", cblsEnvio.Checked.ToString());
                rp[8] = new ReportParameter("Drawback", cblsDrawback.Checked.ToString());
                rp[9] = new ReportParameter("Bancario", cblsBancario.Checked.ToString());
                rp[10] = new ReportParameter("Carimbo_Original", string.IsNullOrEmpty(carimbo_original) ? " " : carimbo_original);
                rp[11] = new ReportParameter("Assinatura", string.IsNullOrEmpty(assinatura) ? " " : assinatura);

                rv.LocalReport.SetParameters(rp);

                rv.LocalReport.Refresh();

                byte[] bytesProforma = bPDF ? rv.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings) : rv.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                string sNomeArquivoProforma = "ProformaInvoice_" + dtProduct1.Tables[0].Rows[0]["nNumeroPedido"].ToString().PadLeft(6, '0') + "_" + CarimboDataHora() + ".pdf";
                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivoProforma, bytesProforma);

                Base_Pedidos_PDF.Add(sNomeArquivoProforma);
            }
            catch (Exception ex)
            {
                MensagemPagina_Documentos.MostraMensagem_Erro("Erro ao gerar o Proforma Invoice! </br>" + ex.Message + (ex.InnerException == null ? "" : "<br />" + ex.InnerException.Message));
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Documentos", "$('#documentos-tab').tab('show');", true);
        }

        void GerarCommercialInvoice(bool bPDF)
        {
            try
            {
                ReportViewer rv = new ReportViewer();

                rv.ProcessingMode = ProcessingMode.Local;
                rv.LocalReport.EnableExternalImages = true;

                rv.LocalReport.ReportPath = "App\\Reports\\" + "Invoice.rdlc";

                Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>
                {
                    { "@idPedido", hddidPedido.Value }
                };
                DataSet dtProduct1 = ExecutarDataSet("sp_Manipula_tbl_Flow_Pedido_PDF_Commercial_Invoice", vParametrosProduct);

                var dtPedido = DateTime.Parse(dtProduct1.Tables[0].Rows[0]["dtPedido"].ToString());
                var sReferencia = dtProduct1.Tables[0].Rows[0]["sReferencia"].ToString();
                var sMoeda = dtProduct1.Tables[0].Rows[0]["sMoeda"].ToString();
                decimal nExWorks = 0;
                decimal Total = 0;

                foreach (DataRow row in dtProduct1.Tables[1].Rows)
                {
                    nExWorks += decimal.Parse(row["nValorTotal"].ToString());

                    row["nValorTotal"] = Math.Round(decimal.Parse(row["nValorTotal"].ToString()), 2);

                    row["nValorUnitario"] = Math.Round(decimal.Parse(row["nValorUnitario"].ToString()), 2);
                }

                nExWorks = Math.Round(nExWorks, 2);

                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtProduct1.Tables[0]));
                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", dtProduct1.Tables[1]));

                string assinatura = " ";
                string carimbo_original = " ";

                if (cblAssinatura.Checked)
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-USUARIO" },
                        { "@idUsuarioIntegrado", Variaveis.idUsuario().Trim() }
                    };
                    DataTable dt = ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_Area", vParametros);

                    if (dt.Rows.Count > 0)
                        assinatura = Convert.ToBase64String(dt.Rows[0]["vbAssinatura"] as byte[]);
                }

                if (cblCarimbos_Original.Checked)
                {
                    var carimbo = GerarCarimbos("Original");

                    if (carimbo != null)
                        carimbo_original = Convert.ToBase64String(carimbo.ToArray());
                }

                ReportParameter[] rp = new ReportParameter[12];

                Total += nExWorks + decimal.Parse(dtProduct1.Tables[0].Rows[0]["nOcean_Air"].ToString()) + decimal.Parse(dtProduct1.Tables[0].Rows[0]["nInsurance"].ToString()) + decimal.Parse(dtProduct1.Tables[0].Rows[0]["nOther_Charges"].ToString());
                rp[0] = new ReportParameter("Total", Total.ToString());
                rp[1] = new ReportParameter("ExWorks", nExWorks.ToString());
                rp[2] = new ReportParameter("dtPedido", dtPedido.ToString("dd/MM/yyyy"));
                rp[3] = new ReportParameter("sReferencia", sReferencia.ToString());
                rp[4] = new ReportParameter("sMoeda", sMoeda.ToString());
                rp[5] = new ReportParameter("DataEmissao", DateTime.Today.ToString("dd/MM/yyyy"));
                rp[6] = new ReportParameter("CheckBoxParameter", cblsDecimal.Checked.ToString());
                rp[7] = new ReportParameter("Envio", cblsEnvio.Checked.ToString());
                rp[8] = new ReportParameter("Drawback", cblsDrawback.Checked.ToString());
                rp[9] = new ReportParameter("Bancario", cblsBancario.Checked.ToString());

                rp[10] = new ReportParameter("Assinatura", string.IsNullOrEmpty(assinatura) ? " " : assinatura);
                rp[11] = new ReportParameter("Carimbo_Original", string.IsNullOrEmpty(carimbo_original) ? " " : carimbo_original);

                rv.LocalReport.SetParameters(rp);

                rv.LocalReport.Refresh();

                byte[] bytesCommercial = bPDF ? rv.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings) : rv.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                string sNomeArquivoCommercial = "CommercialInvoice_" + dtProduct1.Tables[0].Rows[0]["nNumeroPedido"].ToString().PadLeft(6, '0') + "_" + CarimboDataHora() + ".pdf";
                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivoCommercial, bytesCommercial);

                Base_Pedidos_PDF.Add(sNomeArquivoCommercial);
            }
            catch (Exception ex)
            {
                MensagemPagina_Documentos.MostraMensagem_Erro("Erro ao gerar o Commercial Invoice! </br>" + ex.Message + (ex.InnerException == null ? "" : "<br />" + ex.InnerException.Message));
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Documentos", "$('#documentos-tab').tab('show');", true);
        }

        void GerarInvoiceDespachante(bool bPDF)
        {
            try
            {
                ReportViewer rv = new ReportViewer();

                rv.ProcessingMode = ProcessingMode.Local;
                rv.LocalReport.EnableExternalImages = true;

                rv.LocalReport.ReportPath = "App\\Reports\\" + "InvoiceDespachante.rdlc";

                Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>
                {
                    { "@idPedido", hddidPedido.Value }
                };

                DataSet dtProduct1;
                dtProduct1 = ExecutarDataSet("sp_Manipula_tbl_Flow_Pedido_PDF_Commercial_Invoice", vParametrosProduct);

                var dtPedido = DateTime.Parse(dtProduct1.Tables[0].Rows[0]["dtPedido"].ToString());
                var sReferencia = dtProduct1.Tables[0].Rows[0]["sReferencia"].ToString();
                var sMoeda = dtProduct1.Tables[0].Rows[0]["sMoeda"].ToString();
                decimal nExWorks = (decimal)0;
                decimal Total = 0;

                foreach (DataRow row in dtProduct1.Tables[1].Rows)
                {
                    nExWorks += decimal.Parse(row["nValorTotal"].ToString());

                    row["nValorTotal"] = Math.Round(decimal.Parse(row["nValorTotal"].ToString()), 2);

                    row["nValorUnitario"] = Math.Round(decimal.Parse(row["nValorUnitario"].ToString()), 2);
                }

                nExWorks = Math.Round(nExWorks, 2);

                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtProduct1.Tables[0]));
                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", dtProduct1.Tables[1]));

                string assinatura = " ";
                string carimbo_original = " ";

                if (cblAssinatura.Checked)
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-USUARIO" },
                        { "@idUsuarioIntegrado", Variaveis.idUsuario().Trim() }
                    };
                    DataTable dt = ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_Area", vParametros);

                    if (dt.Rows.Count > 0)
                        assinatura = Convert.ToBase64String(dt.Rows[0]["vbAssinatura"] as byte[]);
                }

                if (cblCarimbos_Original.Checked)
                {
                    var carimbo = GerarCarimbos("Original");

                    if (carimbo != null)
                        carimbo_original = Convert.ToBase64String(carimbo.ToArray());
                }

                ReportParameter[] rp = new ReportParameter[11];

                Total += nExWorks + decimal.Parse(dtProduct1.Tables[0].Rows[0]["nOcean_Air"].ToString()) + decimal.Parse(dtProduct1.Tables[0].Rows[0]["nInsurance"].ToString()) + decimal.Parse(dtProduct1.Tables[0].Rows[0]["nOther_Charges"].ToString());
                rp[0] = new ReportParameter("Total", Total.ToString());
                rp[1] = new ReportParameter("ExWorks", nExWorks.ToString());
                rp[2] = new ReportParameter("dtPedido", dtPedido.ToString("dd/MM/yyyy"));
                rp[3] = new ReportParameter("sReferencia", string.IsNullOrEmpty(sReferencia) ? " " : sReferencia);
                rp[4] = new ReportParameter("sMoeda", string.IsNullOrEmpty(sMoeda) ? " " : sMoeda);
                rp[5] = new ReportParameter("DataEmissao", DateTime.Today.ToString("dd/MM/yyyy"));
                rp[6] = new ReportParameter("CheckBoxParameter", cblsDecimal.Checked.ToString());
                rp[7] = new ReportParameter("Envio", cblsEnvio.Checked.ToString());
                rp[8] = new ReportParameter("Drawback", cblsDrawback.Checked.ToString());
                rp[9] = new ReportParameter("Assinatura", string.IsNullOrEmpty(assinatura) ? " " : assinatura);
                rp[10] = new ReportParameter("Carimbo_Original", string.IsNullOrEmpty(carimbo_original) ? " " : carimbo_original);

                rv.LocalReport.SetParameters(rp);

                rv.LocalReport.Refresh();

                byte[] bytesCommercial = bPDF ? rv.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings) : rv.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                string sNomeArquivoCommercial = "InvoiceDespachante_" + dtProduct1.Tables[0].Rows[0]["nNumeroPedido"].ToString().PadLeft(6, '0') + "_" + CarimboDataHora() + ".pdf";
                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivoCommercial, bytesCommercial);

                Base_Pedidos_PDF.Add(sNomeArquivoCommercial);
            }
            catch (Exception ex)
            {
                MensagemPagina_Documentos.MostraMensagem_Erro("Erro ao gerar o Despachante Invoice! </br>" + ex.Message + (ex.InnerException == null ? "" : "<br />" + ex.InnerException.Message));
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Documentos", "$('#documentos-tab').tab('show');", true);
        }

        #endregion

        #region | Pedido de Compras

        void GerarPedidoCompra(bool bPDF)
        {
            try
            {
                ReportViewer rv = new ReportViewer { ProcessingMode = ProcessingMode.Local };
                rv.LocalReport.EnableExternalImages = true;
                rv.LocalReport.ReportPath = "App\\Reports\\" + "Pedido_Compras.rdlc";

                Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>
                {
                    { "@idPedido", hddidPedido.Value }
                };
                DataSet dtProduct1 = ExecutarDataSet("sp_Manipula_tbl_Flow_Pedido_Compra_PDF", vParametrosProduct);

                var dtPedido = DateTime.Parse(dtProduct1.Tables[0].Rows[0]["dtPedido"].ToString());
                var sReferencia = dtProduct1.Tables[0].Rows[0]["sReferencia"].ToString();
                var dtPrevisaoEntrega = DateTime.Parse(dtProduct1.Tables[1].Rows[0]["dtPrevisaoEntrega"].ToString());
                var FormaPagamento = dtProduct1.Tables[0].Rows[0]["FormaPagamento"].ToString();
                decimal vlrProduto = Convert.ToDecimal(dtProduct1.Tables[0].Rows[0]["nVlrProdutos"].ToString());
                decimal vlrLiquido = 0;
                decimal vlrFrete = Convert.ToDecimal(dtProduct1.Tables[0].Rows[0]["nVlrFrete"].ToString());
                decimal nICMS = Convert.ToDecimal(dtProduct1.Tables[1].Rows[0]["nICMS"].ToString());
                decimal nIPI = Convert.ToDecimal(dtProduct1.Tables[1].Rows[0]["nIPI"].ToString());
                decimal vlrICMS = Convert.ToDecimal(dtProduct1.Tables[1].Rows[0]["vlrICMS"].ToString());
                decimal vlrIPI = 0;
                decimal vlrTotalIPI = 0;

                foreach (DataRow row in dtProduct1.Tables[1].Rows)
                {
                    vlrIPI = Convert.ToDecimal(row["vlrIPI"].ToString());
                    vlrTotalIPI += Convert.ToDecimal(row["vlrIPI"].ToString());
                    vlrLiquido += Convert.ToDecimal(row["vlrLiquido"].ToString());

                    try
                    {
                        string sTipoArquivo = "image/jpeg";
                        object vbArquivo = row["vbArquivo"];

                        if (vbArquivo != null) sTipoArquivo = Arquivo.RetornaContentTypeArquivo(string.Empty, vbArquivo);

                        if (!sTipoArquivo.StartsWith("image/"))
                            sTipoArquivo = "image/jpeg";
                        else if (sTipoArquivo.Contains("webp"))
                        {
                            sTipoArquivo = "image/png";
                            vbArquivo = Arquivo.ConverterImagem_PNG(vbArquivo);
                            row["vbArquivo"] = vbArquivo;
                        }
                    }
                    catch { row["vbArquivo"] = null; }
                }

                decimal Total_Material = vlrLiquido;
                decimal total = vlrProduto + vlrFrete;
                decimal valor_item = Convert.ToDecimal(dtProduct1.Tables[1].Rows[0]["nValorTotal"].ToString());

                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtProduct1.Tables[0]));
                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", dtProduct1.Tables[1]));

                string assinatura = " ";
                string carimbo_cnpj = " ";
                string carimbo_aprovado = " ";
                string nome = " ";
                string data = " ";
                string Endereco = " ";

                if (cblAssinatura.Checked)
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-USUARIO" },
                        { "@idUsuarioIntegrado", Variaveis.idUsuario().Trim() }
                    };
                    DataTable dt = ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_Area", vParametros);

                    if (dt.Rows.Count > 0)
                        assinatura = Convert.ToBase64String(dt.Rows[0]["vbAssinatura"] as byte[]);
                }

                if (cblCarimbos_CNPJ.Checked)
                {
                    var carimbo = GerarCarimbo_CNPJ(ddlidEmpresa.SelectedValue);

                    if (carimbo != null)
                        carimbo_cnpj = Convert.ToBase64String(carimbo.ToArray());
                }

                if (cblCarimbos_Aprovado.Checked)
                {
                    var carimbo = GerarCarimbos("Aprovado");

                    if (carimbo != null)
                        carimbo_aprovado = Convert.ToBase64String(carimbo.ToArray());
                }

                if (cblNome.Checked)
                {
                    nome = Variaveis.sUsuarioLogado();
                    data = DateTime.Today.ToString("dd/MM/yyyy");
                    Endereco = txtsEnderecoEntrega.Text;
                }

                ReportParameter[] rp = new ReportParameter[15];

                rp[0] = new ReportParameter("Espaco", " ");
                rp[1] = new ReportParameter("dtPedido", dtPedido.ToString("dd/MM/yyyy"));
                rp[2] = new ReportParameter("dtPrevisaoEntrega", dtPrevisaoEntrega.ToString("dd/MM/yyyy"));
                rp[3] = new ReportParameter("Total_Geral", total.ToString("N2"));
                rp[4] = new ReportParameter("Total_Material", Total_Material.ToString("N2"));
                rp[5] = new ReportParameter("vlrICMS", vlrICMS.ToString("N2"));
                rp[6] = new ReportParameter("vlrIPI", vlrTotalIPI.ToString("N2"));
                rp[7] = new ReportParameter("Assinatura", string.IsNullOrEmpty(assinatura) ? " " : assinatura);
                rp[8] = new ReportParameter("Carimbo_Aprovado", string.IsNullOrEmpty(carimbo_aprovado) ? " " : carimbo_aprovado);
                rp[9] = new ReportParameter("Carimbo_CNPJ", string.IsNullOrEmpty(carimbo_cnpj) ? " " : carimbo_cnpj);
                rp[10] = new ReportParameter("DataEmissao", data);
                rp[11] = new ReportParameter("Nome", nome);
                rp[12] = new ReportParameter("Endereco", Endereco);
                rp[13] = new ReportParameter("FormaPagamento", FormaPagamento);
                rp[14] = new ReportParameter("sObservacao", string.IsNullOrEmpty(txtsObservacao.Text) ? " " : txtsObservacao.Text);

                rv.LocalReport.SetParameters(rp);

                rv.LocalReport.Refresh();

                byte[] bytesPedidoCompra = bPDF ? rv.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings) : rv.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                string sNomeArquivoPedidoCompra = "PedidoCompra_" + dtProduct1.Tables[0].Rows[0]["nNumeroPedido"].ToString().PadLeft(6, '0') + "_" + CarimboDataHora() + ".pdf";
                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivoPedidoCompra, bytesPedidoCompra);

                Base_Pedidos_PDF.Add(sNomeArquivoPedidoCompra);
            }
            catch (Exception ex)
            {
                MensagemPagina_Documentos.MostraMensagem_Erro("Erro ao gerar o Pedido de Compra! </br>" + ex.Message + (ex.InnerException == null ? "" : "<br />" + ex.InnerException.Message));
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Documentos", "$('#documentos-tab').tab('show');", true);
        }

        void GerarPedidoCompraInternacional(bool bPDF)
        {
            try
            {
                ReportViewer rv = new ReportViewer { ProcessingMode = ProcessingMode.Local };
                rv.LocalReport.EnableExternalImages = true;
                rv.LocalReport.EnableHyperlinks = true;
                rv.LocalReport.ReportPath = "App\\Reports\\" + "PedidoCompra_Internacional.rdlc";

                Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>
                {
                    { "@idPedido", hddidPedido.Value }
                };
                DataSet dtProduct1 = ExecutarDataSet("sp_Manipula_tbl_Flow_Pedido_Compra_PDF", vParametrosProduct);

                var dtPedido = DateTime.Parse(dtProduct1.Tables[0].Rows[0]["dtPedido"].ToString());
                var sReferencia = dtProduct1.Tables[0].Rows[0]["sReferencia"].ToString();
                var dtPrevisaoEntrega = DateTime.Parse(dtProduct1.Tables[1].Rows[0]["dtPrevisaoEntrega"].ToString());
                decimal vlrProduto = Convert.ToDecimal(dtProduct1.Tables[0].Rows[0]["nVlrProdutos"].ToString());
                decimal vlrFrete = Convert.ToDecimal(dtProduct1.Tables[0].Rows[0]["nVlrFrete"].ToString());
                decimal nICMS = Convert.ToDecimal(dtProduct1.Tables[1].Rows[0]["nICMS"].ToString());
                decimal nIPI = Convert.ToDecimal(dtProduct1.Tables[1].Rows[0]["nIPI"].ToString());
                decimal vlrICMS = Convert.ToDecimal(dtProduct1.Tables[1].Rows[0]["vlrICMS"].ToString());
                decimal vlrIPI = 0;
                decimal quantidade = 0;
                decimal vlrTotalIPI = 0;
                string Moeda = dtProduct1.Tables[1].Rows[0]["sMoeda"].ToString();
                string TipoCliente = dtProduct1.Tables[0].Rows[0]["sTipoCliente"].ToString();
                string TipoFornecedor = dtProduct1.Tables[0].Rows[0]["sTipoFornecedor"].ToString();

                foreach (DataRow row in dtProduct1.Tables[1].Rows)
                {
                    vlrIPI = Convert.ToDecimal(row["vlrIPI"].ToString());
                    quantidade = Convert.ToDecimal(row["nQuantidade"].ToString());
                    vlrTotalIPI += vlrIPI * quantidade;

                    try
                    {
                        string sTipoArquivo = "image/jpeg";
                        object vbArquivo = row["vbArquivo"];

                        if (vbArquivo != null) sTipoArquivo = Arquivo.RetornaContentTypeArquivo(string.Empty, vbArquivo);

                        if (!sTipoArquivo.StartsWith("image/"))
                            sTipoArquivo = "image/jpeg";
                        else if (sTipoArquivo.Contains("webp"))
                        {
                            sTipoArquivo = "image/png";
                            vbArquivo = Arquivo.ConverterImagem_PNG(vbArquivo);
                            row["vbArquivo"] = vbArquivo;
                        }
                    }
                    catch { row["vbArquivo"] = null; }
                }

                decimal Total_Material = vlrProduto;
                decimal total = vlrProduto + vlrFrete;
                decimal valor_item = Convert.ToDecimal(dtProduct1.Tables[1].Rows[0]["nValorTotal"].ToString());

                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtProduct1.Tables[0]));
                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", dtProduct1.Tables[1]));

                string assinatura = " ";
                string carimbo_cnpj = " ";
                string carimbo_aprovado = " ";
                string nome = " ";
                string data = " ";
                string Endereco = " ";

                if (cblAssinatura.Checked)
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-USUARIO" },
                        { "@idUsuarioIntegrado", Variaveis.idUsuario().Trim() }
                    };
                    DataTable dt = ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_Area", vParametros);

                    if (dt.Rows.Count > 0)
                        assinatura = Convert.ToBase64String(dt.Rows[0]["vbAssinatura"] as byte[]);
                }

                if (cblCarimbos_CNPJ.Checked)
                {
                    var carimbo = GerarCarimbo_CNPJ(ddlidEmpresa.SelectedValue);

                    if (carimbo != null)
                        carimbo_cnpj = Convert.ToBase64String(carimbo.ToArray());
                }

                if (cblCarimbos_Aprovado.Checked)
                {
                    var carimbo = GerarCarimbos("Approved");

                    if (carimbo != null)
                        carimbo_aprovado = Convert.ToBase64String(carimbo.ToArray());
                }

                if (cblNome.Checked)
                {
                    nome = Variaveis.sUsuarioLogado();
                    data = DateTime.Today.ToString("dd/MM/yyyy");
                    Endereco = txtsEnderecoEntrega.Text;
                }

                ReportParameter[] rp = new ReportParameter[16];

                rp[0] = new ReportParameter("Espaco", " ");
                rp[1] = new ReportParameter("dtPedido", dtPedido.ToString("yyyy/MM/dd"));
                rp[2] = new ReportParameter("dtPrevisaoEntrega", dtPrevisaoEntrega.ToString("yyyy/MM/dd"));
                rp[3] = new ReportParameter("Total_Geral", total.ToString("N2"));
                rp[4] = new ReportParameter("Total_Material", Total_Material.ToString("N2"));
                rp[5] = new ReportParameter("vlrICMS", vlrICMS.ToString("N2"));
                rp[6] = new ReportParameter("vlrIPI", vlrTotalIPI.ToString("N2"));
                rp[7] = new ReportParameter("Moeda", string.IsNullOrEmpty(Moeda) ? " " : Moeda);

                if (TipoFornecedor == "E") rp[8] = new ReportParameter("VAT_FEIN", "VAT:");
                else rp[8] = new ReportParameter("VAT_FEIN", "FEIN:");

                if (TipoCliente == "2") rp[9] = new ReportParameter("CNPJ_FEIN", "FEIN:");
                else rp[9] = new ReportParameter("CNPJ_FEIN", "CNPJ:");

                rp[10] = new ReportParameter("Assinatura", string.IsNullOrEmpty(assinatura) ? " " : assinatura);
                rp[11] = new ReportParameter("Carimbo_Aprovado", string.IsNullOrEmpty(carimbo_aprovado) ? " " : carimbo_aprovado);
                rp[12] = new ReportParameter("Carimbo_CNPJ", string.IsNullOrEmpty(carimbo_cnpj) ? " " : carimbo_cnpj);
                rp[13] = new ReportParameter("DataEmissao", data);
                rp[14] = new ReportParameter("Nome", nome);
                rp[15] = new ReportParameter("Endereco", Endereco);

                rv.LocalReport.SetParameters(rp);

                rv.LocalReport.Refresh();

                byte[] bytesPedidoCompra = bPDF ? rv.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings) : rv.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                string sNomeArquivoPedidoCompra = "PedidoCompra_" + dtProduct1.Tables[0].Rows[0]["nNumeroPedido"].ToString().PadLeft(6, '0') + "_" + CarimboDataHora() + ".pdf";
                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivoPedidoCompra, bytesPedidoCompra);

                Base_Pedidos_PDF.Add(sNomeArquivoPedidoCompra);
            }
            catch (Exception ex)
            {
                MensagemPagina_Documentos.MostraMensagem_Erro("Erro ao gerar o Purchase Order! </br>" + ex.Message + (ex.InnerException == null ? "" : "<br />" + ex.InnerException.Message));
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Documentos", "$('#documentos-tab').tab('show');", true);
        }

        void GerarPedidoCompraInternacionalInvoice(bool bPDF)
        {
            try
            {
                ReportViewer rv = new ReportViewer();

                rv.ProcessingMode = ProcessingMode.Local;
                rv.LocalReport.EnableExternalImages = true;

                rv.LocalReport.ReportPath = "App\\Reports\\" + "Invoice_PedidoCompra.rdlc";

                Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>
                {
                    { "@idPedido", hddidPedido.Value }
                };
                DataSet dtProduct1 = ExecutarDataSet("sp_Manipula_tbl_Flow_Pedido_Compra_PDF", vParametrosProduct);

                var dtPedido = DateTime.Parse(dtProduct1.Tables[0].Rows[0]["dtPedido"].ToString());
                var sReferencia = dtProduct1.Tables[0].Rows[0]["sReferencia"].ToString();
                var dtPrevisaoEntrega = DateTime.Parse(dtProduct1.Tables[1].Rows[0]["dtPrevisaoEntrega"].ToString());
                decimal vlrProduto = Convert.ToDecimal(dtProduct1.Tables[0].Rows[0]["nVlrProdutos"].ToString());
                decimal vlrFrete = Convert.ToDecimal(dtProduct1.Tables[0].Rows[0]["nVlrFrete"].ToString());
                decimal nICMS = Convert.ToDecimal(dtProduct1.Tables[1].Rows[0]["nICMS"].ToString());
                decimal nIPI = Convert.ToDecimal(dtProduct1.Tables[1].Rows[0]["nIPI"].ToString());
                decimal vlrICMS = Convert.ToDecimal(dtProduct1.Tables[1].Rows[0]["vlrICMS"].ToString());
                decimal vlrIPI = 0;
                decimal quantidade = 0;
                decimal vlrTotalIPI = 0;
                string Moeda = dtProduct1.Tables[1].Rows[0]["sMoeda"].ToString();
                string TipoCliente = dtProduct1.Tables[0].Rows[0]["sTipoCliente"].ToString();
                string TipoFornecedor = dtProduct1.Tables[0].Rows[0]["sTipoFornecedor"].ToString();

                foreach (DataRow row in dtProduct1.Tables[1].Rows)
                {
                    vlrIPI = Convert.ToDecimal(row["vlrIPI"].ToString());
                    quantidade = Convert.ToDecimal(row["nQuantidade"].ToString());
                    vlrTotalIPI += vlrIPI * quantidade;

                    try
                    {
                        string sTipoArquivo = "image/jpeg";
                        object vbArquivo = row["vbArquivo"];

                        if (vbArquivo != null) sTipoArquivo = Arquivo.RetornaContentTypeArquivo(string.Empty, vbArquivo);

                        if (!sTipoArquivo.StartsWith("image/"))
                            sTipoArquivo = "image/jpeg";
                        else if (sTipoArquivo.Contains("webp"))
                        {
                            sTipoArquivo = "image/png";
                            vbArquivo = Arquivo.ConverterImagem_PNG(vbArquivo);
                            row["vbArquivo"] = vbArquivo;
                        }
                    }
                    catch { row["vbArquivo"] = null; }
                }

                decimal Total_Material = vlrProduto;
                decimal total = vlrProduto + vlrFrete;
                decimal valor_item = Convert.ToDecimal(dtProduct1.Tables[1].Rows[0]["nValorTotal"].ToString());

                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtProduct1.Tables[0]));
                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", dtProduct1.Tables[1]));

                string assinatura = " ";
                string carimbo_original = " ";

                if (cblAssinatura.Checked)
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-USUARIO" },
                        { "@idUsuarioIntegrado", Variaveis.idUsuario().Trim() }
                    };
                    DataTable dt = ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_Area", vParametros);

                    if (dt.Rows.Count > 0)
                        assinatura = Convert.ToBase64String(dt.Rows[0]["vbAssinatura"] as byte[]);
                }

                if (cblCarimbos_Aprovado.Checked)
                {
                    var carimbo = GerarCarimbos("Original");

                    if (carimbo != null)
                        carimbo_original = Convert.ToBase64String(carimbo.ToArray());
                }

                ReportParameter[] rp = new ReportParameter[12];

                rp[0] = new ReportParameter("Espaco", " ");
                rp[1] = new ReportParameter("dtPedido", dtPedido.ToString("yyyy/MM/dd"));
                rp[2] = new ReportParameter("dtPrevisaoEntrega", dtPrevisaoEntrega.ToString("yyyy/MM/dd"));
                rp[3] = new ReportParameter("Total_Geral", total.ToString("N2"));
                rp[4] = new ReportParameter("Total_Material", Total_Material.ToString("N2"));
                rp[5] = new ReportParameter("vlrICMS", vlrICMS.ToString("N2"));
                rp[6] = new ReportParameter("vlrIPI", vlrTotalIPI.ToString("N2"));
                rp[7] = new ReportParameter("Moeda", string.IsNullOrEmpty(Moeda) ? " " : Moeda);

                if (TipoFornecedor == "E") rp[8] = new ReportParameter("VAT_FEIN", "VAT:");
                else rp[8] = new ReportParameter("VAT_FEIN", "FEIN:");

                if (TipoCliente == "2") rp[9] = new ReportParameter("CNPJ_FEIN", "FEIN:");
                else rp[9] = new ReportParameter("CNPJ_FEIN", "CNPJ:");

                rp[10] = new ReportParameter("Assinatura", string.IsNullOrEmpty(assinatura) ? " " : assinatura);
                rp[11] = new ReportParameter("Carimbo_Original", string.IsNullOrEmpty(carimbo_original) ? " " : carimbo_original);

                rv.LocalReport.SetParameters(rp);

                rv.LocalReport.Refresh();

                byte[] bytesPedidoCompra = bPDF ? rv.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings) : rv.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                string sNomeArquivoPedidoCompra = "PedidoCompra_" + dtProduct1.Tables[0].Rows[0]["nNumeroPedido"].ToString().PadLeft(6, '0') + "_" + CarimboDataHora() + ".pdf";
                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivoPedidoCompra, bytesPedidoCompra);

                Base_Pedidos_PDF.Add(sNomeArquivoPedidoCompra);
            }
            catch (Exception ex)
            {
                MensagemPagina_Documentos.MostraMensagem_Erro("Erro ao gerar o Proforma Invoice do Pedido de Compra! </br>" + ex.Message + (ex.InnerException == null ? "" : "<br />" + ex.InnerException.Message));
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Documentos", "$('#documentos-tab').tab('show');", true);
        }

        #endregion

        void GerarArquivosCombinados(bool bPDF)
        {
            try
            {
                if (bPDF)
                {
                    string[] arquivosParaCombinar = new string[Base_Pedidos_PDF.Count];

                    for (int i = 0; i < Base_Pedidos_PDF.Count; i++)
                    {
                        arquivosParaCombinar[i] = Server.MapPath("~/Download/") + Base_Pedidos_PDF[i];
                    }

                    string nomefinal = "Document_" + CarimboDataHora() + ".pdf";
                    string arquivoFinal = Server.MapPath("~/Download/") + nomefinal;

                    CombinePDFs(arquivosParaCombinar, arquivoFinal);

                    DownloadArquivo(Page, nomefinal);
                }
                else
                    DownloadArquivo(Page, CombinarExcel(Base_Pedidos_PDF, Server.MapPath("~/Download/")));

                MensagemPagina_Documentos.MostraMensagem_Sucesso("Documentos gerados com sucesso!");
            }
            catch (Exception ex)
            {
                MensagemPagina_Documentos.MostraMensagem_Erro("Erro ao Combinar Arquivos! </br>" + ex.Message);
            }

            PesquisarPedido(hddidPedido.Value.ToString(), "0", "0", "", hddidTipo.Value);
        }

        protected void ExportarPDF_Click(object sender, EventArgs e)
        {
            if (PI.Checked || CI.Checked || PL.Checked || CO.Checked || IP.Checked || PC.Checked || PCI.Checked)
            {
                if (hddidTipo.Value != "7")
                {
                    if (PI.Checked)
                        GerarProformaInvoice(true);
                    if (CI.Checked)
                        GerarCommercialInvoice(true);
                    if (PL.Checked)
                        GerarPackinglist(true);
                    if (CO.Checked)
                        GerarCertificateOrigin(true);
                    if (IP.Checked)
                        GerarInvoiceDespachante(true);
                }
                else
                {
                    if (PC.Checked)
                        GerarPedidoCompra(true);
                    if (PCI.Checked)
                        GerarPedidoCompraInternacional(true);
                    if (PI.Checked)
                        GerarPedidoCompraInternacionalInvoice(true);
                }

                GerarArquivosCombinados(true);
                Base_Pedidos_PDF.Clear();
            }
            else
            {
                MensagemPagina_Documentos.MostraMensagem_Erro("Selecione ao menos um Documento para ser gerado!");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Documentos", "$('#documentos-tab').tab('show');", true);
                PesquisarPedido(hddidPedido.Value.ToString(), "0", "0", "", hddidTipo.Value);
            }
        }

        protected void ExportarExcel_Click(object sender, EventArgs e)
        {
            if (PI.Checked || CI.Checked || PL.Checked || CO.Checked || IP.Checked || PC.Checked || PCI.Checked)
            {
                if (hddidTipo.Value != "7")
                {
                    if (PI.Checked)
                        GerarProformaInvoice(false);

                    if (CI.Checked)
                        GerarCommercialInvoice(false);

                    if (PL.Checked)
                        GerarPackinglist(false);

                    if (CO.Checked)
                        GerarCertificateOrigin(false);

                    if (IP.Checked)
                        GerarInvoiceDespachante(false);
                }
                else
                {
                    if (PC.Checked)
                        GerarPedidoCompra(false);

                    if (PCI.Checked)
                        GerarPedidoCompraInternacional(false);

                    if (PI.Checked)
                        GerarPedidoCompraInternacionalInvoice(false);
                }

                GerarArquivosCombinados(false);
                Base_Pedidos_PDF.Clear();
            }
            else
            {
                MensagemPagina_Documentos.MostraMensagem_Erro("Selecione o Documento desejado !");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Documentos", "$('#documentos-tab').tab('show');", true);
                PesquisarPedido(hddidPedido.Value.ToString(), "0", "0", "", hddidTipo.Value);
            }
        }

        #endregion

        #region | Itens

        void CalcularValorTotal()
        {
            try
            {
                if (hddidTipo.Value.Equals("2"))
                {
                    decimal nVlrST = Base_Pedidos_Itens.Sum(i => i.nVlrST);
                    decimal nVlrDIFAL = Base_Pedidos_Itens.Sum(i => i.nVlrDIFAL);
                    txtImposto_ST.Text = nVlrST.ToString("N2");
                    txtImposto_DIFAL.Text = nVlrDIFAL.ToString("N2");
                    txtImpostos.Text = (nVlrST + nVlrDIFAL).ToString("N2");
                }

                decimal.TryParse(hddidTipo.Value.Equals("2") ? txtnFreteVendas.Text : txtFrete.Text, out decimal nVlrFrete);
                decimal.TryParse(txtImpostos.Text, out decimal nVlrImpostos);
                decimal.TryParse(txtnVlrDesconto.Text, out decimal nVlrDesconto);
                decimal.TryParse(txtnVlrProdutos.Text.Replace("R$", ""), out decimal nVlrProdutos);
                decimal.TryParse(txtnVlrServicos.Text.Replace("R$", ""), out decimal nVlrServicos);
                decimal.TryParse(txtCustoAduaneiro.Text.Replace("R$", ""), out decimal nCustoAduaneiro);
                decimal.TryParse(txtCustoDespachante.Text.Replace("R$", ""), out decimal nCustoDespachante);

                decimal total = nVlrProdutos + nVlrServicos;

                if (hddidTipo.Value.Equals("2"))
                    total += nVlrFrete + nVlrImpostos + (div_CustoAduaneiro.Visible ? nCustoAduaneiro + nCustoDespachante : 0);

                if (hddidTipo.Value.Equals("7"))
                {
                    if (total != 0) hddnVlrTotal.Value = total.ToString();

                    // Correção Divisão por zero - Thiago Rodrigues 10/02/2026
                    if (!string.IsNullOrEmpty(hddidPedido.Value) || hddidPedido.Value != "0")
                    {
                        if (total == 0) decimal.TryParse(hddnVlrTotal.Value.Replace("R$", ""), out total);
                        txtnDesconto.Text = nVlrDesconto == 0 || total == 0 ? "0,00" : (nVlrDesconto * 100 / total).ToString("N2");
                    }
                    else txtnDesconto.Text = (nVlrDesconto * 100 / total).ToString("N2");

                    if (nVlrDesconto != 0)
                    {
                        total -= nVlrDesconto;

                        if (total != 0)
                            hddnVlrTotal.Value = total.ToString("N2");
                    }
                    else if (total == 0)
                        decimal.TryParse(hddnVlrTotal.Value.Replace("R$", ""), out total);
                }

                txtnVlrTotal.Text = string.Format("{0:N2}", total.ToString("N2"));
            }
            catch
            {
                txtnVlrTotal.Text = "Erro";
            }
        }

        bool GravarPedido_Itens(string idPedido)
        {
            bool bRetorno = false;
            string sCodigoProduto = "";

            try
            {
                foreach (GridViewRow item in gv_resultado.Rows)
                {
                    try
                    {
                        if (Base_Pedidos_Itens.Where(i => i.sCodigoProduto == item.Cells[2].Text).FirstOrDefault() is cls_Pedidos_Itens resultado)
                        {
                            if (resultado.sFuncao != "Inserir_item_envio")
                            {
                                resultado.sFuncao = "INCLUIR ITEM ALTERACAO";
                                resultado.nValorReal = double.Parse((item.FindControl("txtnValorReal") as TextBox).Text);
                                resultado.nValorRealTotal = Convert.ToDecimal(resultado.nValorReal * resultado.nQuantidade);
                                resultado.nResultado = resultado.nValorRealTotal - Convert.ToDecimal(resultado.nValorTotal);
                            }
                            else
                            {
                                resultado.sFuncao = "Inserir_item_envio";
                                resultado.nValorReal = double.Parse((item.FindControl("txtnValorReal") as TextBox).Text);
                                resultado.nValorRealTotal = Convert.ToDecimal(resultado.nValorReal * resultado.nQuantidade);
                                resultado.nResultado = resultado.nValorRealTotal - Convert.ToDecimal(resultado.nValorTotal);
                            }
                        }
                    }
                    catch
                    {
                        if (Base_Pedidos_Itens.Where(i => i.EnvioResultado).FirstOrDefault() is cls_Pedidos_Itens resultado)
                        {
                            resultado.sFuncao = "Inserir_item_envio";
                            resultado.nValorReal = double.Parse((item.FindControl("txtnValorReal") as TextBox).Text);
                            resultado.nValorRealTotal = Convert.ToDecimal(resultado.nValorReal * resultado.nQuantidade);
                            resultado.nResultado = resultado.nValorRealTotal - Convert.ToDecimal(resultado.nValorTotal);
                        }
                    }
                }

                foreach (var linha in Base_Pedidos_Itens)
                {
                    sCodigoProduto = "";

                    if (idPedido != "0")
                    {
                        if (linha.sFuncao != "CONSULTA ITEM")
                        {
                            Dictionary<string, string> vParametrosItem = new Dictionary<string, string>();

                            if (linha.sFuncao != "Inserir_item_envio")
                            {
                                string nQuantidade = "0";
                                string Importacao_dtPO = "";
                                string Importacao_dtETD = "";
                                string Importacao_dtETA = "";

                                try
                                {
                                    nQuantidade = Convert.ToDouble(linha.nQuantidade).ToString().Replace(',', '.');
                                }
                                catch (Exception)
                                {
                                    throw new Exception("Erro na quantdade do item " + linha.sDscProduto);
                                }

                                sCodigoProduto = linha.sCodigoProduto + " - " + linha.sDscProduto;

                                vParametrosItem.Add("@sFuncao", linha.sFuncao);
                                vParametrosItem.Add("@idPedido", idPedido);
                                vParametrosItem.Add("@idItem", linha.idItem.ToString());
                                vParametrosItem.Add("@idProduto", linha.idProduto.ToString());
                                vParametrosItem.Add("@sCodigo", linha.sCodigoProduto);
                                vParametrosItem.Add("@sDscProduto", linha.sDscProduto);
                                vParametrosItem.Add("@sUnidade", linha.sUnidade);
                                vParametrosItem.Add("@nValorUnitario", Conversoes.Numerico(linha.nValorUnitario));
                                vParametrosItem.Add("@nValorTotal", Conversoes.Numerico(linha.nValorTotal));
                                vParametrosItem.Add("@nQuantidade", nQuantidade);
                                vParametrosItem.Add("@idNCM", linha.idCNM.ToString());
                                vParametrosItem.Add("@nOrdem", linha.nOrdem.ToString());
                                vParametrosItem.Add("@Importacao_idTipoDestino", linha.Importacao_idDestino.ToString());
                                vParametrosItem.Add("@Importacao_idAtoConcessorio", linha.Importacao_idAtoConcessorio.ToString());

                                if (linha.Importacao_dtPO != null) Importacao_dtPO = linha.Importacao_dtPO.ToString();
                                vParametrosItem.Add("@Importacao_dtPO", Importacao_dtPO);

                                if (linha.Importacao_dtETD != null) Importacao_dtETD = linha.Importacao_dtETD.ToString();
                                vParametrosItem.Add("@Importacao_dtETD", Importacao_dtETD);

                                if (linha.Importacao_dtETA != null) Importacao_dtETA = linha.Importacao_dtETA.ToString();
                                vParametrosItem.Add("@Importacao_dtETA", Importacao_dtETA);

                                vParametrosItem.Add("@nValorReal", Conversoes.Numerico(linha.nValorReal));
                                vParametrosItem.Add("@nValorRealTotal", Conversoes.Numerico(linha.nValorRealTotal));
                                vParametrosItem.Add("@nResultado", Conversoes.Numerico(linha.nResultado));
                                vParametrosItem.Add("@idUsuarioInclusao", Variaveis.idUsuario());

                                if (hddidTipo.Value == "7")
                                {
                                    vParametrosItem.Add("@nICMS", Conversoes.Numerico(linha.nICMS));
                                    vParametrosItem.Add("@nIPI", Conversoes.Numerico(linha.nIPI));
                                    vParametrosItem.Add("@nDesconto", Conversoes.Numerico(linha.nDesconto));
                                    vParametrosItem.Add("@nValorTblPreco", Conversoes.Numerico(linha.nValorTblPreco));
                                    vParametrosItem.Add("@dtPrevisaoEntrega", linha.dtPrevisaoEntrega.ToString());
                                    vParametrosItem.Add("@idTipo", hddidTipo.Value);
                                }
                            }
                            else
                            {
                                vParametrosItem.Add("@sFuncao", "Inserir_item_envio");
                                vParametrosItem.Add("@idPedido", idPedido);
                                vParametrosItem.Add("@nValorEnvio", Conversoes.Numerico(linha.nValorUnitario));
                                vParametrosItem.Add("@nValorEnvioReal", Conversoes.Numerico(linha.nValorReal));
                                vParametrosItem.Add("@nResultadoEnvio", Conversoes.Numerico(linha.nResultado));
                            }

                            DataSet dsItem = ExecutarDataSet(sProcedure, vParametrosItem);
                        }
                    }

                    bRetorno = true;
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao Incluir/Alterar Item: " + sCodigoProduto + "</br>" + ex.Message);
            }

            return bRetorno;
        }

        #region | dtgItens

        void DataBind_dtgItens(string sMetodoChamada)
        {
            dtgItens.Columns[Col_chkItem_Selecionado].Visible = false;
            dtgItens.Columns[Col_sTipo].Visible = false;
            dtgItens.Columns[Col_idContador].Visible = true;
            dtgItens.Columns[Col_nOrdem_View].Visible = true;
            dtgItens.Columns[Col_nQuantidade_View].Visible = true;
            gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_nQuantidade_View].Visible = true;
            dtgItens.Columns[Col_dtPrevisaoEntrega_Edit].Visible = false;
            dtgItens.Columns[Col_dtPrevisaoEntrega_View].Visible = false;
            dtgItens.Columns[Col_nICMS_Edit].Visible = false;
            dtgItens.Columns[Col_nIPI_Edit].Visible = false;
            dtgItens.Columns[Col_nICMS_View].Visible = false;
            dtgItens.Columns[Col_nIPI_View].Visible = false;
            dtgItens.Columns[Col_nDesconto_Edit].Visible = false;
            dtgItens.Columns[Col_nDesconto_View].Visible = false;

            if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
            {
                dtgItens.Columns[Col_sCodigoAtoConcessorio_View].Visible = true;
                dtgItens.Columns[Col_sDscDestino_View].Visible = true;
                dtgItens.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM" && !c.EnvioResultado).OrderBy(x => x.nOrdem).ToList();
                dtgItens.DataBind();
            }
            if (hddidTipo.Value == "2" || hddidTipo.Value == "7")
            {
                dtgItens.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM").OrderBy(x => x.nOrdem).ToList();
                dtgItens.DataBind();

                if (hddidTipo.Value == "2") CalcularValorTotal();

                // Antonio Lemos - 20/06/2024
                // Solução péssima, mas só os pedidos acima deste número ou novos deverão ser calculados automáticamente
                // Não retirar (GATO)
                if (idPedido() > 1879 || idPedido() == 0)
                {
                    decimal nVlrFrete;

                    if (hddidTipo.Value.Equals("2"))
                        decimal.TryParse(txtnFreteVendas.Text, out nVlrFrete);
                    else
                        decimal.TryParse(txtFrete.Text, out nVlrFrete);

                    decimal nVlrProdutos = 0;
                    decimal nVlrServicos = 0;
                    DateTime maiorData = DateTime.MinValue;

                    foreach (GridViewRow row in dtgItens.Rows)
                    {
                        var nValorTotal = row.FindControl("nValorTotal") as Label;
                        string valor = Regex.Replace(nValorTotal.Text.Trim(), @"[^\d,\.]", "");

                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            try
                            {
                                if (nValorTotal.Text != null && nValorTotal.Text != "&nbsp;")
                                {
                                    if (hddidTipo.Value != "7")
                                    {
                                        if (dtgItens.DataKeys[row.DataItemIndex]["sTipoProduto_Servico"].ToString() == "S") // Serviço
                                            nVlrServicos += decimal.Parse(valor);
                                        else
                                            nVlrProdutos += decimal.Parse(valor);
                                    }
                                    else
                                    {
                                        nVlrProdutos += decimal.Parse(valor);
                                        TextBox dtRevisao = (TextBox)row.FindControl("txtdtEntrega");

                                        if (DateTime.TryParse(dtRevisao.Text, out DateTime dataConvertida))
                                        {
                                            if (dataConvertida > maiorData)
                                            {
                                                maiorData = dataConvertida;
                                                txtdtEstimativaEntrega.Text = maiorData.ToString("dd/MM/yyyy");
                                            }
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }

                    txtnVlrProdutos.ReadOnly = true;
                    txtnVlrProdutos.Text = nVlrProdutos.ToString("N2");
                    decimal total = nVlrProdutos + nVlrServicos;

                    if (hddidTipo.Value != "7")
                    {
                        txtnVlrServicos.ReadOnly = true;
                        txtnVlrServicos.Text = nVlrServicos.ToString("N2");

                        decimal.TryParse(txtImpostos.Text, out decimal nVlrImpostos);
                        decimal.TryParse(txtCustoAduaneiro.Text, out decimal nVlrAduaneiro);
                        decimal.TryParse(txtCustoDespachante.Text, out decimal nVlrDespachante);

                        if (hddidTipo.Value.Equals("2"))
                            total += nVlrFrete + nVlrImpostos + (div_CustoAduaneiro.Visible ? nVlrAduaneiro + nVlrDespachante : 0);

                        txtnVlrTotal.Text = total.ToString("N2");
                    }
                    else
                    {
                        decimal.TryParse(txtnVlrServicos.Text, out nVlrServicos);
                        decimal.TryParse(txtnVlrDesconto.Text, out decimal nVlrDesconto);

                        total = nVlrProdutos + nVlrServicos;

                        // Correção Divisão por zero - Thiago Rodrigues 10/02/2026
                        txtnDesconto.Text = nVlrDesconto == 0 || total == 0 ? "0,00" : (nVlrDesconto * 100 / total).ToString("N2");
                        total -= nVlrDesconto;

                        txtnVlrTotal.Text = total.ToString("N2");
                    }

                    upd_Pedido.Update();
                }
            }

            if (hddidTipo.Value == "7")
            {
                dtgItens.Columns[Col_sTipo].Visible = false;
                dtgItens.Columns[Col_sDscCategoriaVendas].Visible = false;

                foreach (GridViewRow row in dtgItens.Rows)
                {
                    decimal Unitario = Convert.ToDecimal((row.FindControl("nValorUnitario") as Label).Text);
                    decimal Quantidade = Convert.ToDecimal((row.FindControl("Item_GV_txtnQuantidade") as TextBox).Text);
                    decimal Desconto = Convert.ToDecimal((row.FindControl("Item_GV_txtnDesconto") as TextBox).Text);
                    decimal totalCalculado = Unitario * Quantidade - (Unitario * Quantidade * (Desconto / 100));
                    string Total = Regex.Replace(totalCalculado.ToString("N2"), @"[^\d,\.]", "");

                    if (sMoedaCompra != "")
                    {
                        (row.FindControl("nValorUnitario") as Label).Text = $"{Unitario} {sMoedaCompra}";
                        (row.FindControl("nValorTotal") as Label).Text = $"{Total} {sMoedaCompra}";
                    }
                }
            }

            dtgItens.Columns[Col_idContador].Visible = false;
            dtgItens.Visible = true;

            if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
            {
                this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_idContador].Visible = true;
                this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_nQuantidade_View].Visible = true;
                this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_nValorUnitario_View].Visible = true;
                this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_PO_View].Visible = true;
                this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_ETA_View].Visible = true;
                this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_ETD_View].Visible = true;
                this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_Chegada].Visible = false;

                if (hddidTipo.Value == "3")
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_Chegada].Visible = true;

                gv_Itens_ValoresDatas.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM" && !c.EnvioResultado).OrderBy(x => x.nOrdem).ToList();
                gv_Itens_ValoresDatas.DataBind();
                foreach (GridViewRow row in gv_Itens_ValoresDatas.Rows)
                {
                    sMoeda = hddMoeda.Value;
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        if (sMoeda != "")
                        {
                            (row.FindControl("sMoedaOrigem") as HtmlGenericControl).InnerText = sMoeda;
                            (row.FindControl("sMoedaOrigem5") as HtmlGenericControl).InnerText = sMoeda;
                            (row.FindControl("sMoedaOrigem6") as HtmlGenericControl).InnerText = sMoeda;

                            try
                            {
                                gv_Itens_ValoresDatas.FooterRow.Font.Bold = true;
                                gv_Itens_ValoresDatas.FooterRow.Visible = true;
                                decimal total = 0;
                                if (gv_Itens_ValoresDatas.FooterRow.Cells[9].Text != null && gv_Itens_ValoresDatas.FooterRow.Cells[9].Text != "&nbsp;")
                                {
                                    total = decimal.Parse(gv_Itens_ValoresDatas.FooterRow.Cells[9].Text.Replace(sMoeda, "").Trim());
                                }
                                total += decimal.Parse((row.FindControl("nValorTotal") as Label).Text);
                                gv_Itens_ValoresDatas.FooterRow.Cells[9].Text = string.Format("{0:N4} {1}", total, sMoeda);
                                ValorProduto = total.ToString("N2");
                            }
                            catch { }
                        }
                        else
                        {
                            try
                            {
                                gv_Itens_ValoresDatas.FooterRow.Font.Bold = true;
                                gv_Itens_ValoresDatas.FooterRow.Visible = true;
                                decimal total = 0;
                                if (gv_Itens_ValoresDatas.FooterRow.Cells[9].Text != null && gv_Itens_ValoresDatas.FooterRow.Cells[9].Text != "&nbsp;")
                                {
                                    total = decimal.Parse(gv_Itens_ValoresDatas.FooterRow.Cells[9].Text);
                                }
                                total += decimal.Parse((row.FindControl("nValorTotal") as Label).Text);
                                gv_Itens_ValoresDatas.FooterRow.Cells[9].Text = string.Format("{0:N4}", total);
                                ValorProduto = total.ToString("N2");
                            }
                            catch { }
                        }
                    }
                }
                txtnVlrProdutos.Text = string.Format("{0:N2}", ValorProduto);
                upd_Pedido.Update();

                this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_idContador].Visible = false;
                updValoresDatas.Update();
            }

            GridViewHelper helper = new GridViewHelper(dtgItens);
            helper.GroupHeader += new GroupEvent(helper_GroupHeader);
            helper.GroupSummary += new GroupEvent(helper_Summary);

            if (hddidTipo.Value == "7") helper.RegisterGroup("sTipo", true, true);
            else helper.RegisterGroup("sDscCategoriaVendas", true, true);

            this.dtgItens.Columns[Col_sCodigoAtoConcessorio_Edit].Visible = false;
            this.dtgItens.Columns[Col_sCodigoAtoConcessorio_View].Visible = false;
            this.dtgItens.Columns[Col_sDscDestino_Edit].Visible = false;
            this.dtgItens.Columns[Col_sDscDestino_View].Visible = false;
            this.dtgItens.Columns[Col_nOrdem_Edit].Visible = false;


            if (sMetodoChamada != "Editar" && sMetodoChamada != "Duplicar")
            {
                this.dtgItens.Columns[Col_nQuantidade_Edit].Visible = false;
                this.dtgItens.Columns[Col_nQuantidade_View].Visible = true;
                this.dtgItens.Columns[Col_nValorUnitario_Edit].Visible = false;
                this.dtgItens.Columns[Col_nValorUnitario_View].Visible = true;
                this.dtgItens.Columns[Col_BotaoExcluir].Visible = false;
                this.dtgItens.Columns[Col_nOrdem_View].Visible = false;
                this.dtgItens.Columns[Col_nOrdem_Edit].Visible = false;

                if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
                {
                    this.dtgItens.Columns[Col_nOrdem_Edit].Visible = false;
                    this.dtgItens.Columns[Col_nOrdem_View].Visible = true;
                    this.dtgItens.Columns[Col_nValorUnitario_View].Visible = false;
                    this.dtgItens.Columns[Col_nValorTotal].Visible = false;
                    this.dtgItens.Columns[Col_sCodigoAtoConcessorio_View].Visible = true;
                    this.dtgItens.Columns[Col_sDscDestino_View].Visible = true;
                    this.dtgItens.Columns[Col_chkItem_Selecionado].Visible = false;

                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_nValorUnitario_View].Visible = true;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_nValorUnitario_Edit].Visible = false;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_nQuantidade_View].Visible = true;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_nQuantidade_Edit].Visible = false;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_PO_View].Visible = true;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_PO_Edit].Visible = false;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_ETA_View].Visible = true;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_ETA_Edit].Visible = false;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_ETD_View].Visible = true;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_ETD_Edit].Visible = false;
                }
                else if (hddidTipo.Value == "7")
                {
                    this.dtgItens.Columns[Col_nDesconto_Edit].Visible = false;
                    this.dtgItens.Columns[Col_nDesconto_View].Visible = true;
                    this.dtgItens.Columns[Col_nOrdem_View].Visible = true;
                    this.dtgItens.Columns[Col_dtPrevisaoEntrega_Edit].Visible = false;
                    this.dtgItens.Columns[Col_dtPrevisaoEntrega_View].Visible = true;
                    this.dtgItens.Columns[Col_nICMS_View].Visible = true;
                    this.dtgItens.Columns[Col_nIPI_View].Visible = true;

                    if (ddlsTipoCompra.SelectedValue == "I")
                    {
                        this.dtgItens.Columns[Col_nICMS_Edit].Visible = false;
                        this.dtgItens.Columns[Col_nIPI_Edit].Visible = false;
                        this.dtgItens.Columns[Col_nICMS_View].Visible = false;
                        this.dtgItens.Columns[Col_nIPI_View].Visible = false;
                    }
                }
                else if (hddidTipo.Value == "2")
                {
                    this.dtgItens.Columns[Col_nOrdem_Edit].Visible = false;
                    this.dtgItens.Columns[Col_nOrdem_View].Visible = true;
                }
            }
            else // Edição
            {
                this.dtgItens.Columns[Col_nQuantidade_Edit].Visible = true;
                this.dtgItens.Columns[Col_nQuantidade_View].Visible = false;
                this.dtgItens.Columns[Col_nValorUnitario_Edit].Visible = true;
                this.dtgItens.Columns[Col_nValorUnitario_View].Visible = false;
                this.dtgItens.Columns[Col_BotaoExcluir].Visible = true;
                this.dtgItens.Columns[Col_nOrdem_View].Visible = false;
                this.dtgItens.Columns[Col_nOrdem_Edit].Visible = false;

                if (hddidTipo.Value == "7")
                {
                    this.dtgItens.Columns[Col_nOrdem_Edit].Visible = true;
                    this.dtgItens.Columns[Col_nDesconto_Edit].Visible = true;
                    this.dtgItens.Columns[Col_nDesconto_View].Visible = false;
                    this.dtgItens.Columns[Col_dtPrevisaoEntrega_Edit].Visible = true;
                    this.dtgItens.Columns[Col_nICMS_Edit].Visible = true;
                    this.dtgItens.Columns[Col_nIPI_Edit].Visible = true;

                    if (ddlsTipoCompra.SelectedValue == "I")
                    {
                        this.dtgItens.Columns[Col_nICMS_Edit].Visible = false;
                        this.dtgItens.Columns[Col_nIPI_Edit].Visible = false;
                        this.dtgItens.Columns[Col_nICMS_View].Visible = false;
                        this.dtgItens.Columns[Col_nIPI_View].Visible = false;
                    }
                }

                this.dtgItens.Columns[Col_dtPrevisaoEntrega_View].Visible = false;

                if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
                {
                    this.dtgItens.Columns[Col_nOrdem_Edit].Visible = true;
                    this.dtgItens.Columns[Col_nOrdem_View].Visible = false;
                    this.dtgItens.Columns[Col_nValorUnitario_Edit].Visible = false;
                    this.dtgItens.Columns[Col_nValorTotal].Visible = false;
                    this.dtgItens.Columns[Col_sCodigoAtoConcessorio_Edit].Visible = true;
                    this.dtgItens.Columns[Col_sDscDestino_Edit].Visible = true;
                    this.dtgItens.Columns[Col_nQuantidade_Edit].Visible = false;
                    this.dtgItens.Columns[Col_nQuantidade_View].Visible = false;
                    this.dtgItens.Columns[Col_chkItem_Selecionado].Visible = true;

                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_nValorUnitario_View].Visible = false;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_nValorUnitario_Edit].Visible = true;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_nQuantidade_View].Visible = false;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_nQuantidade_Edit].Visible = true;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_ETA_View].Visible = false;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_ETA_Edit].Visible = true;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_PO_View].Visible = false;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_PO_Edit].Visible = true;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_ETD_View].Visible = false;
                    this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_ETD_Edit].Visible = true;

                    if (hddidTipo.Value == "3")
                        this.gv_Itens_ValoresDatas.Columns[Col_ValoresDatas_Chegada].Visible = true;

                    if (dtgItens.Rows.Count > 0)
                        div_btnExcluirSelecionados.Visible = true;
                    else
                        div_btnExcluirSelecionados.Visible = false;
                }
                else if (hddidTipo.Value == "2")
                {
                    this.dtgItens.Columns[Col_nOrdem_Edit].Visible = true;
                    this.dtgItens.Columns[Col_nOrdem_View].Visible = false;
                }

                foreach (GridViewRow row in dtgItens.Rows)
                {
                    var nValorTotal = row.FindControl("nValorTotal") as Label;
                    string Total = Regex.Replace(nValorTotal.Text.Trim(), @"[^\d,\.]", "");

                    nValorTotal.Text = string.Format(Total);
                }
            }

            if (hddidTipo.Value != "3" && hddidTipo.Value != "6")
                this.dtgItens.Columns[Col_sCodigoNCM].Visible = false;

            DIV_Valores.Visible = true;
            if (ValidaPermissao(Permissao.Pedidos.EnxergarValores))
            {
                DIV_nVlrProdutos.Visible = true;
                DIV_nVlrServicos.Visible = true;
                div_nVlrImpostos.Visible = hddidTipo.Value == "2";
                DIV_DADOS_CondPagamento.Visible = hddidTipo.Value == "2" || hddidTipo.Value == "7";

                if (hddidTipo.Value != "7") DIV_nVlrTotal.Visible = true;
                if (hddidTipo.Value == "2") helper.RegisterSummary("nValorTotal", SummaryOperation.Sum, "sDscCategoriaVendas");
                if (hddidTipo.Value == "7") helper.RegisterSummary("nValorTotal", SummaryOperation.Sum, "sTipo");
            }
            else
            {
                if (hddidTipo.Value == "7")
                {
                    DIV_DADOS_CondPagamento.Visible = true;
                }

                DIV_Valores.Visible = hddidTipo.Value != "2";
                this.dtgItens.Columns[Col_nValorUnitario_View].Visible = false;
                this.dtgItens.Columns[Col_nValorTotal].Visible = false;
            }

            // Resultado
            if (sMetodoChamada != "Editar" && sMetodoChamada != "Duplicar")
            {
                this.gv_resultado.Columns[0].Visible = false;
                this.gv_resultado.Columns[5].Visible = false;
                this.gv_resultado.Columns[6].Visible = true;
                this.gv_resultado.Columns[7].Visible = false;
                this.gv_resultado.Columns[8].Visible = true;
                this.gv_resultado.Columns[10].Visible = false;
                this.gv_resultado.Columns[11].Visible = true;
                this.gv_resultado.Columns[12].Visible = true;
                Resultados_cmdGravarPedido.Visible = false;
                Resultados_cmdEditar.Visible = true;
                Resultados_cmdDuplicar.Visible = true;
            }
            else // Edição
            {
                this.gv_resultado.Columns[0].Visible = false;
                this.gv_resultado.Columns[5].Visible = true;
                this.gv_resultado.Columns[6].Visible = false;
                this.gv_resultado.Columns[7].Visible = true;
                this.gv_resultado.Columns[8].Visible = false;
                this.gv_resultado.Columns[10].Visible = true;
                this.gv_resultado.Columns[11].Visible = false;
                this.gv_resultado.Columns[12].Visible = true;
                Resultados_cmdEditar.Visible = false;
                Resultados_cmdDuplicar.Visible = false;
                Resultados_cmdGravarPedido.Visible = true;
            }

            if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
            {
                if (Base_Pedidos_Itens.Where(c => c.EnvioResultado).FirstOrDefault() == null)
                {
                    if (ddlEnvio.SelectedValue == "2")
                    {
                        cls_Pedidos_Itens Linha = new cls_Pedidos_Itens();
                        Linha.sCodigoProduto = "Envio";
                        Linha.sDscProduto = "Valor do Envio";
                        Linha.nValorUnitario = Convert.ToDouble(hddValorEnvio.Value);
                        Linha.nValorTotal = Convert.ToDouble(hddValorEnvio.Value);
                        Linha.nValorReal = Convert.ToDouble(hddValorEnvioReal.Value);
                        Linha.nValorRealTotal = Convert.ToDecimal(hddValorEnvioReal.Value);
                        Linha.nResultado = -Convert.ToDecimal(hddValorEnvio.Value) + Convert.ToDecimal(hddValorEnvioReal.Value);
                        Linha.sFuncao = "Inserir_item_envio";
                        Linha.nOrdem = 9999;
                        Linha.nQuantidade = 1;
                        Linha.EnvioResultado = true;
                        Base_Pedidos_Itens.Add(Linha);
                    }
                }

                gv_resultado.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM").OrderBy(x => x.nOrdem).ToList();
                gv_resultado.DataBind();
            }
        }

        void dtgItens_SalvarGRID()
        {
            foreach (GridViewRow item in dtgItens.Rows)
            {
                if (item.RowType == DataControlRowType.DataRow)
                {
                    try
                    {
                        int idContador = 0;
                        bool bAlterar = false;

                        try
                        {
                            idContador = Convert.ToInt32(item.Cells[Col_idContador].Text);
                        }
                        catch
                        {
                            idContador = Convert.ToInt32(dtgItens.DataKeys[item.RowIndex]["idContador"]);
                        }

                        int index = Base_Pedidos_Itens.FindIndex(x => x.idContador.Equals(idContador));
                        string data = "";
                        double nQuantidade = 0, nValorUnitario = 0, nDesconto = 0, IPIsoma = 0;
                        int nOrdem = 0, Importacao_idDestino = 0, Importacao_idAtoConcessorio = 0;
                        decimal ICMS = 0, IPI = 0, Desconto = 0;

                        if (hddidTipo.Value == "2" || hddidTipo.Value == "7")
                        {
                            TextBox txtnQuantidade = (TextBox)item.FindControl("Item_GV_txtnQuantidade");
                            Double.TryParse(txtnQuantidade.Text, out nQuantidade);

                            TextBox txtnValorUnitario = (TextBox)item.FindControl("Item_GV_txtnValorUnitario");
                            Double.TryParse(txtnValorUnitario.Text, out nValorUnitario);

                            TextBox txtnOrdem_pedidoItem = (TextBox)item.FindControl("txtnOrdem_pedidoItem");
                            nOrdem = Convert.ToInt32(txtnOrdem_pedidoItem.Text);

                            if (hddidTipo.Value == "7")
                            {
                                TextBox txtdtEntrega = (TextBox)item.FindControl("txtdtEntrega");
                                data = txtdtEntrega.Text;

                                TextBox Item_GV_txtnICMS = (TextBox)item.FindControl("Item_GV_txtnICMS");
                                if (Item_GV_txtnICMS.Text == "") ICMS = 0;
                                else ICMS = Convert.ToDecimal(Item_GV_txtnICMS.Text);

                                TextBox Item_GV_txtnIPI = (TextBox)item.FindControl("Item_GV_txtnIPI");
                                if (Item_GV_txtnIPI.Text == "") IPI = 0;
                                else
                                {
                                    IPI = Convert.ToDecimal(Item_GV_txtnIPI.Text);
                                    IPIsoma = Convert.ToDouble(Item_GV_txtnIPI.Text);
                                }

                                TextBox Item_GV_txtnDesconto = (TextBox)item.FindControl("Item_GV_txtnDesconto");
                                decimal.TryParse(Item_GV_txtnDesconto.Text, out Desconto);
                                double.TryParse(Item_GV_txtnDesconto.Text, out nDesconto);
                            }

                            if (Base_Pedidos_Itens[index].nQuantidade != nQuantidade || Base_Pedidos_Itens[index].nValorUnitario != nValorUnitario || Base_Pedidos_Itens[index].nDesconto != Desconto || Base_Pedidos_Itens[index].nOrdem != nOrdem || Base_Pedidos_Itens[index].dtPrevisaoEntrega != data || Base_Pedidos_Itens[index].nICMS != ICMS || Base_Pedidos_Itens[index].nIPI != IPI)
                            {
                                Base_Pedidos_Itens[index].nQuantidade = nQuantidade;
                                Base_Pedidos_Itens[index].nOrdem = nOrdem;

                                if (hddidTipo.Value != "7")
                                {
                                    Base_Pedidos_Itens[index].nValorUnitario = nValorUnitario;

                                    double.TryParse(txtCambioMoeda.Text, out double nCambio);
                                    if (hddidTipo.Value != "2" || !div_Moeda.Visible || nCambio <= 0)
                                        nCambio = 1;

                                    Base_Pedidos_Itens[index].nValorTotal = nQuantidade * nValorUnitario * nCambio;
                                }
                                else
                                {
                                    double vlrComIPI = (nValorUnitario + (nValorUnitario * (IPIsoma / 100)));
                                    Base_Pedidos_Itens[index].nValorUnitario = nValorUnitario;
                                    Base_Pedidos_Itens[index].nDesconto = Desconto;
                                    Base_Pedidos_Itens[index].nValorTotal = vlrComIPI * nQuantidade - (vlrComIPI * nQuantidade * (nDesconto / 100));
                                    Base_Pedidos_Itens[index].dtPrevisaoEntrega = data;
                                    Base_Pedidos_Itens[index].nICMS = ICMS;
                                    Base_Pedidos_Itens[index].nIPI = IPI;
                                }

                                bAlterar = true;
                            }
                        }
                        else if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
                        {
                            TextBox txtnOrdem_pedidoItem = (TextBox)item.FindControl("txtnOrdem_pedidoItem");
                            nOrdem = Convert.ToInt32(txtnOrdem_pedidoItem.Text);

                            DropDownList ddlImportacao_idDestino = (item.FindControl("ddlItem_Importacao_idDestino") as DropDownList);
                            DropDownList ddlImportacao_idAtoConcessorio = (item.FindControl("ddlItem_Importacao_idAtoConcessorio") as DropDownList);

                            int.TryParse(ddlImportacao_idDestino.SelectedValue, out Importacao_idDestino);
                            int.TryParse(ddlImportacao_idAtoConcessorio.SelectedValue, out Importacao_idAtoConcessorio);

                            if (Base_Pedidos_Itens[index].Importacao_idDestino != Importacao_idDestino || Base_Pedidos_Itens[index].Importacao_idAtoConcessorio != Importacao_idAtoConcessorio || Base_Pedidos_Itens[index].nOrdem != nOrdem)
                            {
                                Base_Pedidos_Itens[index].Importacao_idDestino = Importacao_idDestino;
                                Base_Pedidos_Itens[index].Importacao_idAtoConcessorio = Importacao_idAtoConcessorio;
                                Base_Pedidos_Itens[index].nOrdem = nOrdem;
                                bAlterar = true;
                            }
                        }

                        if (bAlterar && Base_Pedidos_Itens[index].sFuncao.Substring(0, 12) != "INCLUIR ITEM")
                            Base_Pedidos_Itens[index].sFuncao = "ALTERAR_ITEM";
                    }
                    catch
                    {
                        Item_MensagemPagina.MostraMensagem_Erro("Erro nos valores dos itens!");
                    }
                }
            }

            if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
            {
                foreach (GridViewRow item in gv_Itens_ValoresDatas.Rows)
                {
                    if (item.RowType == DataControlRowType.DataRow)
                    {
                        try
                        {
                            bool bAlterar = false;
                            int idContador = Convert.ToInt32(item.Cells[Col_ValoresDatas_idContador].Text);
                            int index = Base_Pedidos_Itens.FindIndex(x => x.idContador.Equals(idContador));
                            double nValorUnitario = 0;
                            double nQuantidade = 0;
                            string Importacao_dtPO = "";
                            string Importacao_dtETD = "";
                            string Importacao_dtETA = "";

                            TextBox txtnValorUnitario = (TextBox)item.FindControl("Itens_ValoresDatas_txtnValorUnitario");
                            Double.TryParse(txtnValorUnitario.Text, out nValorUnitario);

                            TextBox txtnQuantidade = (TextBox)item.FindControl("Itens_ValoresDatas_txtnQuantidade");
                            Double.TryParse(txtnQuantidade.Text, out nQuantidade);

                            TextBox txtImportacao_dtPO = (TextBox)item.FindControl("Itens_ValoresDatas_txtdtPO");
                            TextBox txtImportacao_dtETA = (TextBox)item.FindControl("Itens_ValoresDatas_txtdtETA");
                            TextBox txtImportacao_dtETD = (TextBox)item.FindControl("Itens_ValoresDatas_txtdtETD");

                            if (Base_Pedidos_Itens[index].nQuantidade != nQuantidade || Base_Pedidos_Itens[index].nValorUnitario != nValorUnitario)
                            {
                                Base_Pedidos_Itens[index].nQuantidade = nQuantidade;
                                Base_Pedidos_Itens[index].nValorUnitario = nValorUnitario;
                                Base_Pedidos_Itens[index].nValorTotal = nValorUnitario * Base_Pedidos_Itens[index].nQuantidade;
                                bAlterar = true;
                            }

                            if (Base_Pedidos_Itens[index].Importacao_dtPO != txtImportacao_dtPO.Text)
                            {
                                Base_Pedidos_Itens[index].Importacao_dtPO = txtImportacao_dtPO.Text;
                                bAlterar = true;
                            }

                            if (Base_Pedidos_Itens[index].Importacao_dtETA != txtImportacao_dtETA.Text)
                            {
                                Base_Pedidos_Itens[index].Importacao_dtETA = txtImportacao_dtETA.Text;
                                bAlterar = true;
                            }

                            if (Base_Pedidos_Itens[index].Importacao_dtETD != txtImportacao_dtETD.Text)
                            {
                                Base_Pedidos_Itens[index].Importacao_dtETD = txtImportacao_dtETD.Text;
                                bAlterar = true;
                            }

                            if (bAlterar)
                            {
                                if (Base_Pedidos_Itens[index].sFuncao.Substring(0, 12) != "INCLUIR ITEM")
                                {
                                    Base_Pedidos_Itens[index].sFuncao = "ALTERAR_ITEM";
                                }
                            }
                        }
                        catch
                        {
                            Item_MensagemPagina.MostraMensagem_Erro("Erro nas Datas e Valores");
                        }
                    }
                }

            }
        }

        protected void dtgItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlImportacao_idDestino = e.Row.FindControl("ddlItem_Importacao_idDestino") as DropDownList;
                Popula_Combo(ddlImportacao_idDestino, "sp_Select 'tbl_Flow_Comex_Destino'", "idDestino", "sDscDestinoEN", false, "Selecione o Destino", "0");
                ddlImportacao_idDestino.SelectedValue = DataBinder.Eval(e.Row.DataItem, "Importacao_idDestino").ToString();

                DropDownList ddlImportacao_idAtoConcessorio = e.Row.FindControl("ddlItem_Importacao_idAtoConcessorio") as DropDownList;
                Popula_Combo(ddlImportacao_idAtoConcessorio, "sp_Select 'tbl_Flow_Comex_AtoConcessorio'", "idAtoConcessorio", "sCodigoAtoConcessorio", false, "Selecione o Ato Concessório", "0");
                ddlImportacao_idAtoConcessorio.SelectedValue = DataBinder.Eval(e.Row.DataItem, "Importacao_idAtoConcessorio").ToString();

                int idProduto = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "idProduto"));

                LinkButton btnToggle = e.Row.FindControl("btnToggle") as LinkButton;
                LinkButton lnkDetalhes = e.Row.FindControl("lnkDetalhes") as LinkButton;
                LinkButton lnkServico = e.Row.FindControl("lnkServico") as LinkButton;

                btnToggle.Visible = false;
                lnkDetalhes.Visible = false;
                lnkServico.Visible = false;

                if (hddidTipo.Value == "2" || hddidTipo.Value == "7")
                {
                    TextBox txtValor = (TextBox)e.Row.FindControl("Item_GV_txtnValorUnitario");
                    decimal valor = Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "nValorUnitario"));
                    Label nValorUnitario = (Label)e.Row.FindControl("nValorUnitario");
                    Label nValorTotal = (Label)e.Row.FindControl("nValorTotal");

                    if (hddidTipo.Value == "2")
                    {
                        string sMoedaVenda = dtgItens.DataKeys[e.Row.RowIndex]["sMoedaVenda"].ToString();
                        string nValor = valor.ToString("N2") + " " + sMoedaVenda;
                        nValorUnitario.Text = nValor;

                        string total = nValorTotal.Text;
                        nValorTotal.Text = total + " " + sMoedaVenda;

                        txtValor.Text = valor.ToString("N2");
                    }
                    else
                    {
                        txtValor.Text = valor.ToString("N4");
                        nValorUnitario.Text = valor.ToString("N4");
                    }

                    if (dtgItens.DataKeys[e.Row.RowIndex]["sTipoProduto_Servico"].ToString() == "S") // Serviço
                    {
                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONSULTA_COMPOSICAO" },
                            { "@iditem", idProduto.ToString() }
                        };
                        DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

                        lnkServico.Visible = ValidarDataSet(dsPesquisa) && click < 2;
                    }
                    else
                    {
                        lnkDetalhes.Visible = true;

                        AdicionarLinhasComposicao(idProduto, e.Row, Col_sDscProduto);

                        if (hddidTipo.Value == "2" && e.Row.FindControl("gvComposicaoSistema") is GridView gvComposicaoSistema)
                        {
                            var item = Base_Pedidos_Itens.FirstOrDefault(x => x.idContador.ToString() == DataBinder.Eval(e.Row.DataItem, "idContador").ToString());
                            if (item != null && Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "bProdutoPai")))
                            {
                                Dictionary<string, string> vParam = new Dictionary<string, string>
                                {
                                   { "@sFuncao", "CONSULTA_COMPOSICAO_SISTEMA" },
                                   { "@idPedido", item.idPedido.ToString() },
                                   { "@idItem", item.idItem.ToString() }
                                };
                                DataTable dt = ExecutarDataTable(sProcedure, vParam);

                                gvComposicaoSistema.DataSource = dt;
                                gvComposicaoSistema.DataBind();

                                btnToggle.Visible = gvComposicaoSistema.Rows.Count > 0;
                            }
                            else btnToggle.Visible = false;
                        }
                    }
                }
                else if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
                {
                    lnkServico.Visible = false;
                    AdicionarLinhasComposicao(idProduto, e.Row, Col_sDscProduto);
                }
            }
        }

        protected void dtgItens_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            int idContador = Convert.ToInt32(e.Keys[0].ToString());
            Base_Pedidos_Itens[Base_Pedidos_Itens.FindIndex(x => x.idContador.Equals(idContador))].sFuncao = "EXCLUIR ITEM";
            hddsAlteracaoItens.Value = "S";
            DataBind_dtgItens("Editar");
            VisibilidadeLinkProduto(true);
        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable m_DataTable = ConvertTo<FrameWork.cls_Pedidos_Itens>(Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM").ToList());

            if (m_DataTable != null)
            {
                DataView m_DataView = new DataView(m_DataTable);
                m_DataView.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(e.SortDirection);
                dtgItens.DataSource = m_DataView;
                dtgItens.DataBind();
            }
        }

        #region | Ordenar

        //------------------- Higor Maestrello 16/07/2024 ----------------------
        protected void ddlOrdem_SelectedIndexChanged(object sender, EventArgs e)
        {
            btDesc_Click(sender, e);
        }

        protected void btDesc_Click(object sender, EventArgs e)
        {
            Item_MensagemPagina.MostraMensagem_Aviso("Aviso: Ao alterar a ordenação dos itens, a numeração da ordem será modificada. Após salvar, não será possível retornar ao valor anterior!");

            bool ordenacaoDescendente = ViewState["OrdenacaoDescendente"] as bool? ?? true;

            if (ddlOrdem.SelectedValue == "1")
            {
                if (ordenacaoDescendente)
                {
                    dtgItens.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM" && c.sFuncao != "Inserir_item_envio").OrderByDescending(x => x.nOrdem).ToList();
                    ViewState["OrdenacaoDescendente"] = false;
                }
                else
                {
                    dtgItens.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM" && c.sFuncao != "Inserir_item_envio").OrderBy(x => x.nOrdem).ToList();
                    ViewState["OrdenacaoDescendente"] = true;
                }
                dtgItens.DataBind();
                RenumeranOrdem();
            }
            if (ddlOrdem.SelectedValue == "2")
            {
                if (ordenacaoDescendente)
                {
                    dtgItens.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM" && c.sFuncao != "Inserir_item_envio").OrderByDescending(x => x.sCodigoProduto).ToList();
                    ViewState["OrdenacaoDescendente"] = false;
                }
                else
                {
                    dtgItens.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM" && c.sFuncao != "Inserir_item_envio").OrderBy(x => x.sCodigoProduto).ToList();
                    ViewState["OrdenacaoDescendente"] = true;
                }
                dtgItens.DataBind();
                RenumeranOrdem();
            }
            if (ddlOrdem.SelectedValue == "3")
            {
                if (ordenacaoDescendente)
                {
                    dtgItens.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM" && c.sFuncao != "Inserir_item_envio").OrderByDescending(x => x.sDscProduto).ToList();
                    ViewState["OrdenacaoDescendente"] = false;
                }
                else
                {
                    dtgItens.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM" && c.sFuncao != "Inserir_item_envio").OrderBy(x => x.sDscProduto).ToList();
                    ViewState["OrdenacaoDescendente"] = true;
                }
                dtgItens.DataBind();
                RenumeranOrdem();
            }
            if (ddlOrdem.SelectedValue == "4")
            {
                if (ordenacaoDescendente)
                {
                    dtgItens.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM" && c.sFuncao != "Inserir_item_envio").OrderByDescending(x => x.sCodigoNCM).ToList();
                    ViewState["OrdenacaoDescendente"] = false;
                }
                else
                {
                    dtgItens.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM" && c.sFuncao != "Inserir_item_envio").OrderBy(x => x.sCodigoNCM).ToList();
                    ViewState["OrdenacaoDescendente"] = true;
                }
                dtgItens.DataBind();
                RenumeranOrdem();
            }
            if (ddlOrdem.SelectedValue == "5")
            {
                if (ordenacaoDescendente)
                {
                    dtgItens.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM" && c.sFuncao != "Inserir_item_envio").OrderByDescending(x => x.nQuantidade).ToList();
                    ViewState["OrdenacaoDescendente"] = false;
                }
                else
                {
                    dtgItens.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM" && c.sFuncao != "Inserir_item_envio").OrderBy(x => x.nQuantidade).ToList();
                    ViewState["OrdenacaoDescendente"] = true;
                }
                dtgItens.DataBind();
                RenumeranOrdem();
            }
            if (ddlOrdem.SelectedValue == "6")
            {
                if (ordenacaoDescendente)
                {
                    dtgItens.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM" && c.sFuncao != "Inserir_item_envio").OrderByDescending(x => x.Importacao_idDestino).ToList();
                    ViewState["OrdenacaoDescendente"] = false;
                }
                else
                {
                    dtgItens.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM" && c.sFuncao != "Inserir_item_envio").OrderBy(x => x.Importacao_idDestino).ToList();
                    ViewState["OrdenacaoDescendente"] = true;
                }
                dtgItens.DataBind();
                RenumeranOrdem();
            }
            if (ddlOrdem.SelectedValue == "7")
            {
                if (ordenacaoDescendente)
                {
                    dtgItens.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM" && c.sFuncao != "Inserir_item_envio").OrderByDescending(x => x.Importacao_idAtoConcessorio).ToList();
                    ViewState["OrdenacaoDescendente"] = false;
                }
                else
                {
                    dtgItens.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM" && c.sFuncao != "Inserir_item_envio").OrderBy(x => x.Importacao_idAtoConcessorio).ToList();
                    ViewState["OrdenacaoDescendente"] = true;
                }
                dtgItens.DataBind();
                RenumeranOrdem();
            }
            Item_MensagemPagina.Focus();
        }

        private void RenumeranOrdem()
        {
            for (int i = 0; i < dtgItens.Rows.Count; i++)
            {
                int nOrdem = i + 1;

                TextBox txtnOrdem = dtgItens.Rows[i].FindControl("txtnOrdem_pedidoItem") as TextBox;

                if (txtnOrdem != null)
                {
                    txtnOrdem.Text = nOrdem.ToString();
                }
            }
        }

        protected void ddlidPais_SelectedIndexChanged(object sender, EventArgs e)
        {
            RegistraScript("");
            Popula_Combo(ddlImportacao_idFornecedor, "sp_Select 'Flow_Parceiro_Importacao_Fornecedor', @idPesquisa=" + ddlidPais.SelectedValue, "idParceiro", "sRazaoSocial", false, "Selecione o Exportador", "0");
        }
        //---------------------------------------------------------------------

        #endregion

        #endregion

        #region | Importar Itens

        protected void rbTipo_ImportarItens_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnImportarItens.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sfuncao", "PESQUISA_ITENS" },
                { "@sPesquisa", rbTipo_ImportarItens.SelectedValue }
            };
            DataSet dsPesquisa = ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_EmissaoNFE", vParametros);

            if (ValidarDataSet(dsPesquisa, out _))
            {
                pnImportarItens.Visible = true;
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", DataBindComScript_Geral(gvImportarItens, dsPesquisa.Tables[0], true, true, true, "false", "''", "10", true, true, true, 0, "desc", null), true);
            }
        }

        protected void gvImportarItens_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            dtgItens_SalvarGRID();

            string id = gvImportarItens.DataKeys[Convert.ToInt32(e.CommandArgument.ToString())]["id"].ToString();

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sfuncao", "CONSULTAR_ITENS" },
                { "@sPesquisa", rbTipo_ImportarItens.SelectedValue },
                { "@idPedido", id }
            };
            DataSet dsPesquisa = ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_EmissaoNFE", vParametros);

            if (ValidarDataSet(dsPesquisa, out _))
            {
                try
                {
                    int.TryParse(hddidPedido.Value, out int idPedido);

                    foreach (DataRow linha in dsPesquisa.Tables[0].Rows)
                    {
                        cls_Pedidos_Itens item = new cls_Pedidos_Itens
                        {
                            sFuncao = "INCLUIR ITEM",
                            idContador = Base_Pedidos_Itens.Count + 1,
                            nOrdem = Base_Pedidos_Itens.Count + 1,
                            idPedido = idPedido,
                            idItem = 0,
                            idProduto = int.Parse(linha["idProduto"].ToString()),

                            sCodigoProduto = linha["sCodigo"].ToString(),
                            sDscProduto = linha["sDscProduto"].ToString(),
                            sUnidade = linha["sUnidade"].ToString(),
                            nQuantidade = Math.Round(Convert.ToDouble(linha["nQuantidade"].ToString()), 2),
                            nValorUnitario = Convert.ToDouble(linha["nValorUnitario"].ToString()),
                            nValorTotal = Math.Round(Convert.ToDouble(linha["nValorUnitario"].ToString()) * Math.Round(Convert.ToDouble(linha["nQuantidade"].ToString()), 2), 2),

                            sDscCategoriaVendas = linha["sDscCategoriaVendas"].ToString(),
                            idCNM = Convert.ToInt32(linha["idNCM"].ToString()),
                            sCodigoNCM = linha["sCodigoNCM"].ToString(),
                            sTipoProduto_Servico = linha["sTipo"].ToString(),
                            sDscProdutoIdioma = linha["sDscImportacao"].ToString(),

                            Importacao_dtPO = "",
                            Importacao_dtETA = "",
                            Importacao_dtETD = "",
                            sMoedaVenda = ""
                        };
                        Base_Pedidos_Itens.Add(item);
                    }

                    DataBind_dtgItens("Editar");
                    VisibilidadeLinkProduto(true);
                }
                catch (Exception ex)
                {
                    MensagemPagina_Modal_ImportarItens.MostraMensagem_Erro(ex.Message);
                    Scripts.AbrirModal(Page, "Modal_ImportarItens");
                    return;
                }
            }

            pnImportarItens.Visible = false;
            rbTipo_ImportarItens.ClearSelection();

            Scripts.FocusScript(Page, "Item_txtsCodigoProduto");
            Scripts.FecharModal(Page, "Modal_ImportarItens");
        }

        #endregion

        #endregion

        #region | Eventos

        #region | Click

        protected void cmdIncluirItem_Click(object sender, EventArgs e)
        {
            dtgItens_SalvarGRID();

            string sMensagem = "";
            if ((hddidTipo.Value == "3" || hddidTipo.Value == "6") && ddlidPais.SelectedValue == "0") sMensagem = "Selecione o Pais de Origem!";

            if (string.IsNullOrEmpty(Item_txtsCodigoProduto.Text)) sMensagem = "Informe um código de produto válido!";
            else if (string.IsNullOrEmpty(Item_txtsDscProduto.Text)) sMensagem = "Informe um produto válido!";
            else if (Item_ddlsUnidade.SelectedValue == "0") sMensagem = "Selecione a unidade!";
            else if (string.IsNullOrEmpty(Item_txtnQuantidade.Text)) sMensagem = "Informe a quantidade!";

            if (hddidTipo.Value == "7" && string.IsNullOrEmpty(txtdtPrevisaoEntrega.Text)) sMensagem = "Informe a Previsão de Entrega!";

            if ((hddidTipo.Value == "2" || hddidTipo.Value == "7" && sMensagem == "") && !Validacoes.ValidarMoeda(Item_txtnValorUnitario)) sMensagem = "Informe o valor unitário!";

            if (sMensagem == "")
            {
                int idPedido = 0;
                string[] vidPedido = hddidPedido.Value.Split(',');
                string sFuncao = "INCLUIR ITEM";

                if (vidPedido[0].ToString() == "") idPedido = 0;
                else
                {
                    idPedido = Convert.ToInt32(vidPedido[0].ToString());
                    sFuncao = "INCLUIR ITEM ALTERACAO";
                }

                cls_Pedidos_Itens objItem = new cls_Pedidos_Itens();

                double nValorUnitario = 0, nValorReal = 0;
                double.TryParse(Item_txtnValorUnitario.Text, out nValorUnitario);

                objItem.idContador = Base_Pedidos_Itens.Count + 1;

                if (hddidTipo.Value == "2") objItem.nOrdem = Base_Pedidos_Itens.Where(c => c.sDscCategoriaVendas.ToString().ToUpper().Equals(Item_hddsDscCategoriaVendas.Value.ToUpper())).Count() + 1;
                if (hddidTipo.Value == "7") objItem.nOrdem = Base_Pedidos_Itens.Count > 0 ? Base_Pedidos_Itens.OrderBy(i => i.nOrdem).Last().nOrdem + 10 : 10;
                if (hddidTipo.Value == "3" || hddidTipo.Value == "6") objItem.nOrdem = Base_Pedidos_Itens.Count + 1;

                double.TryParse(txtCambioMoeda.Text, out double nCambio);
                if (hddidTipo.Value != "2" || !div_Moeda.Visible || nCambio <= 0)
                    nCambio = 1;

                objItem.idPedido = idPedido;
                objItem.idItem = 0;
                objItem.idProduto = Convert.ToInt32(Item_hddidProduto.Value);
                objItem.sDscCategoriaVendas = Item_hddsDscCategoriaVendas.Value;
                objItem.sCodigoProduto = Item_txtsCodigoProduto.Text.ToString();
                objItem.sDscProduto = Item_txtsDscProduto.Text;
                objItem.sUnidade = Item_ddlsUnidade.SelectedValue;
                objItem.nQuantidade = Convert.ToDouble(Item_txtnQuantidade.Text.ToString());
                objItem.sFuncao = sFuncao;
                objItem.nValorUnitario = nValorUnitario;
                objItem.nValorTotal = objItem.nQuantidade * objItem.nValorUnitario * nCambio;
                objItem.idCNM = Convert.ToInt32(Item_hddidNCM.Value);
                objItem.sCodigoNCM = Item_hddsCodigoNCM.Value.ToString();
                objItem.Importacao_dtPO = "";
                objItem.Importacao_dtETA = "";
                objItem.Importacao_dtETD = "";
                objItem.nValorReal = nValorReal;
                objItem.nValorRealTotal = Convert.ToDecimal(objItem.nQuantidade) * Convert.ToDecimal(objItem.nValorReal);
                objItem.nResultado = Convert.ToDecimal(objItem.nValorRealTotal) - Convert.ToDecimal(objItem.nValorTotal);
                objItem.sTipoProduto_Servico = Item_hddsTipoProduto_Servico.Value;
                objItem.sDscProdutoIdioma = Item_hddssDscDescricaoIdioma.Value;
                objItem.sMoedaVenda = "";

                if (hddidTipo.Value == "7")
                {
                    DateTime data = Convert.ToDateTime(txtdtPrevisaoEntrega.Text);
                    objItem.dtPrevisaoEntrega = data.ToString("dd/MM/yyyy");
                    objItem.nValorTblPreco = Convert.ToDecimal(Item_hddsValortblPreco.Value);
                    objItem.sUnidadeEntrega = Item_ddlsUnidadeEntrega.SelectedValue;
                }
                else if (hddidTipo.Value == "2")
                {
                    try
                    {
                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONSULTA_REGRAS__PEDIDOS" },
                            { "@idPedido", idPedido.ToString() },
                            { "@idItem", Item_hddidProduto.Value },
                            { "@nValorUnitario", nValorUnitario.ToString() },
                            { "@nFretePrevisto", Conversoes.Numerico(txtFrete) }
                        };
                        DataTable dt = ExecutarDataTable(sProcedure, vParametros);

                        objItem.sCodigoCEST = dt.Rows[0]["nVlrICMS"].ToString();
                        objItem.nVlrIPI = decimal.Parse(dt.Rows[0]["nVlrIPI"].ToString());
                        objItem.nVlrICMS = decimal.Parse(dt.Rows[0]["nVlrICMS"].ToString());

                        objItem.nVlrDIFAL = objItem.nValorTotal.DoubleToDecimal() * (decimal.Parse(dt.Rows[0]["nDIFAL"].ToString()) / 100);
                        objItem.nICMS_Interno_Destino = decimal.Parse(dt.Rows[0]["nICMS_Interno_Destino"].ToString());
                        objItem.nMVA = Retorna_MVA_LegisWeb(dt.Rows[0]["sUF_Origem"].ToString(), dt.Rows[0]["sUF_Destino"].ToString(), decimal.Parse(dt.Rows[0]["nMVA"].ToString()), dt.Rows[0]["sDestinoVenda"].ToString().Equals("R"), dt.Rows[0]["sICMSST"].ToString().Equals("S"), dt.Rows[0]["sCodigoNCM"].ToString(), objItem.sCodigoCEST, objItem.nVlrICMS, dt.Rows[0]["dtUltimaConsulta"].ToString(), dt.Rows[0]["sMsgErro"].ToString());

                        if (objItem.nMVA > 0)
                        {
                            var st = CalculaValor_ST(objItem.nValorTotal.DoubleToDecimal(), objItem.nVlrICMS * objItem.nQuantidade.DoubleToDecimal(), objItem.nMVA, objItem.nICMS_Interno_Destino);
                            objItem.nVlrST = st.Item1;
                            objItem.nMVA = st.Item2;
                        }
                    }
                    catch { }
                }

                Base_Pedidos_Itens.Add(objItem);
                Item_LimpaCampos();
                hddsAlteracaoItens.Value = "S";
                PopularRptSugestao(objItem.idProduto.ToString(), Convert.ToDouble(objItem.nQuantidade));
            }
            else
            {
                Item_MensagemPagina.MostraMensagem_Erro(sMensagem, false);

                if (ddlidPais.SelectedValue == "0")
                {
                    Item_txtsCodigoProduto.Text = "";
                    Item_txtsDscProduto.Text = "";
                    Item_txtnQuantidade.Text = "";
                    Item_ddlsUnidade.SelectedValue = "0";
                    Item_ddlsUnidadeEntrega.SelectedValue = "0";
                }
            }

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Focus", "$('[id$=Item_txtsCodigoProduto]').focus();", true);
            DataBind_dtgItens("Editar");
            VisibilidadeLinkProduto(true);
        }

        protected void cmd_ExcluirItensSelecionados_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in dtgItens.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chk = (CheckBox)row.FindControl("cb_Excluir");

                    if (chk != null && chk.Checked)
                    {
                        int idProduto = Convert.ToInt32(dtgItens.DataKeys[row.RowIndex].Values["idProduto"]);

                        var item = Base_Pedidos_Itens.FirstOrDefault(x => x.idProduto == idProduto);

                        item.sFuncao = "EXCLUIR ITEM";
                    }
                }
            }

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Focus2", "$('[id$=Item_txtsCodigoProduto]').focus();", true);
            DataBind_dtgItens("Editar");
            VisibilidadeLinkProduto(true);
        }

        #endregion

        #region | Text Changed

        protected void txtsDscProduto_TextChanged(object sender, EventArgs e)
        {

        }

        protected void txtsCodigoProduto_TextChanged(object sender, EventArgs e)
        {
            dtgItens_SalvarGRID();

            if (Item_hddPesquisaPor.Value != "DESCRICAO")
            {
                if (Item_txtsCodigoProduto.Text != "")
                {
                    string idPesquisa = "1";
                    string sPesquisa = Item_txtsCodigoProduto.Text.Trim();
                    string idPais = "";
                    string idClienteProduto = "";

                    if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
                    {
                        idPesquisa = "0";
                        idPais = ddlidPais.SelectedValue;
                    }
                    else if (hddidTipo.Value == "2" || hddidTipo.Value == "7")
                        idPais = "0";

                    if (hddidTipo.Value == "2")
                    {
                        if (hddidCliente_Produto.Value != "") idClienteProduto = hddidCliente_Produto.Value;
                        else idClienteProduto = hddidCliente.Value;
                    }
                    else idClienteProduto = "0";

                    if (hddidTipo.Value != "7")
                    {
                        //21/06/2024 - Antonio Lemos - Inclusão de @idFiltro=99
                        SqlDataReader sdr = ExecutarDataReader("sp_Select 'FLOW_Produtos_Codigo'," + idPesquisa + ", 'S', '" + sPesquisa + "', @idFiltro=99, @idPais =" + idPais + ", @idUsuario= " + idClienteProduto);

                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                Item_txtsDscProduto.Text = sdr["sDscProduto"].ToString();
                                Item_ddlsUnidade.SelectedValue = sdr["sUnidade"].ToString();
                                Item_hddidProduto.Value = sdr["idItem"].ToString();
                                Item_hddsDscCategoriaVendas.Value = sdr["sDscCategoriaVendas"].ToString();
                                Item_hddsTipoProduto_Servico.Value = sdr["sTipoProduto_Servico"].ToString();
                                Item_hddidNCM.Value = sdr["idNCM"].ToString();
                                Item_hddsCodigoNCM.Value = sdr["sCodigoNCM"].ToString();
                                Item_hddPesquisaPor.Value = "";
                                Item_hddssDscDescricaoIdioma.Value = sdr["sDscProdutoIdioma"].ToString();

                                if (hddidTipo.Value == "7")
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtdtPrevisaoEntrega]').focus();", true);
                                else
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=Item_txtnQuantidade]').focus();", true);
                            }
                        }
                        else
                        {
                            Item_LimpaCampos();
                            Item_MensagemPagina.MostraMensagem_Erro("Produto " + sPesquisa + ", Não localizado!", false);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=Item_txtsCodigoProduto]').focus();", true);
                        }

                        sdr.Close();
                    }
                    else
                    {
                        string sidParceiro = "";

                        if (ddlFornecedor.SelectedValue == "") sidParceiro = "0";
                        else sidParceiro = ddlFornecedor.SelectedValue;

                        SqlDataReader sdr = ExecutarDataReader("sp_Select 'FLOW_Produtos_Codigo'," + idPesquisa + ", 'S', '" + sPesquisa + "', @idFiltro=7, @idPais =" + idPais + ", @sidParceiro=" + sidParceiro);

                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                Item_txtsDscProduto.Text = sdr["sDscProduto"].ToString();
                                Item_ddlsUnidade.SelectedValue = sdr["sUnidade"].ToString();
                                Item_hddidProduto.Value = sdr["idItem"].ToString();
                                Item_hddsDscCategoriaVendas.Value = sdr["sDscCategoriaVendas"].ToString();
                                Item_hddsTipoProduto_Servico.Value = sdr["sTipoProduto_Servico"].ToString();
                                Item_hddidNCM.Value = sdr["idNCM"].ToString();
                                Item_hddsCodigoNCM.Value = sdr["sCodigoNCM"].ToString();
                                Item_hddPesquisaPor.Value = "";
                                Item_hddssDscDescricaoIdioma.Value = sdr["sDscProdutoIdioma"].ToString();
                                Item_hddsValortblPreco.Value = sdr["nTotal"].ToString();

                                if (hddidTipo.Value == "7")
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtdtPrevisaoEntrega]').focus();", true);
                                else
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=Item_txtnQuantidade]').focus();", true);
                            }
                        }
                        else
                        {
                            Item_LimpaCampos();
                            Item_MensagemPagina.MostraMensagem_Erro("Produto " + sPesquisa + ", Não localizado!", false);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=Item_txtsCodigoProduto]').focus();", true);
                        }

                        sdr.Close();
                    }
                }
                else
                {
                    Item_LimpaCampos();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=Item_txtsCodigoProduto]').focus();", true);
                }

                DataBind_dtgItens("Editar");
                VisibilidadeLinkProduto(true);
                RegistraScript("");
            }
            else Item_hddPesquisaPor.Value = "";
        }

        protected void txtFrete_TextChanged(object sender, EventArgs e) => CalcularValorTotal();

        protected void txtnVlrProdutos_TextChanged(object sender, EventArgs e) => CalcularValorTotal();

        protected void txtnVlrServicos_TextChanged(object sender, EventArgs e) => CalcularValorTotal();

        protected void txtCustoAdicional_TextChanged(object sender, EventArgs e) => CalcularValorTotal();

        protected void txtnDesconto_TextChanged(object sender, EventArgs e)
        {
            decimal.TryParse(txtnVlrProdutos.Text.Replace("R$", ""), out decimal nVlrProdutos);
            decimal.TryParse(txtnVlrServicos.Text.Replace("R$", ""), out decimal nVlrServicos);
            decimal.TryParse(txtnDesconto.Text, out decimal nDesconto);
            decimal.TryParse(txtnVlrDesconto.Text, out decimal nVlrDesconto);
            decimal total = nVlrProdutos + nVlrServicos;

            if (total == 0 && !string.IsNullOrEmpty(hddnVlrTotal.Value) && hddnVlrTotal.Value != "0")
                decimal.TryParse(hddnVlrTotal.Value, out total);

            if (nVlrDesconto != 0)
            {
                if ((sender as TextBox).ID.Contains("nVlr"))
                    txtnDesconto.Text = (total == 0 ? 0 : nVlrDesconto * 100 / total).ToString("N2");
                else
                    txtnVlrDesconto.Text = (total * (nDesconto / 100)).ToString("N2");

                total -= nVlrDesconto;
            }

            CalcularValorTotal();
        }

        protected void Item_GV_txtnValorUnitario_TextChanged(object sender, EventArgs e)
        {
            if (hddidTipo.Value == "2" || hddidTipo.Value == "7")
            {
                double nVlrProdutos = 0, nVlrServicos = 0, nVlrST = 0, nVlrDIFAL = 0, nVlrIPI = 0;
                DateTime maiorData = DateTime.MinValue;

                foreach (GridViewRow row in dtgItens.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        try
                        {
                            var nValorTotal = row.FindControl("nValorTotal") as Label;

                            if (nValorTotal.Text != null && nValorTotal.Text != "&nbsp;")
                            {
                                if (hddidTipo.Value != "7")
                                {
                                    double.TryParse((row.FindControl("Item_GV_txtnQuantidade") as TextBox).Text, out double qtd);
                                    double.TryParse((row.FindControl("Item_GV_txtnValorUnitario") as TextBox).Text, out double valor);
                                    double.TryParse(txtCambioMoeda.Text, out double nCambio);

                                    if (hddidTipo.Value != "2" || !div_Moeda.Visible || nCambio <= 0)
                                        nCambio = 1;

                                    double total = qtd * valor * nCambio;

                                    if (dtgItens.DataKeys[row.DataItemIndex]["sTipoProduto_Servico"].ToString() == "S") // Serviço
                                    {
                                        nVlrServicos += total;
                                        nValorTotal.Text = total.ToString("N2");

                                        var itemServico = Base_Pedidos_Itens.FirstOrDefault(i => i.idContador.ToString() == row.Cells[Col_idContador].Text);
                                        if (itemServico != null)
                                        {
                                            itemServico.nQuantidade = qtd;
                                            itemServico.nValorUnitario = valor;
                                            itemServico.nValorTotal = total;

                                            if (itemServico.sFuncao != "Inserir_item_envio")
                                                itemServico.sFuncao = "ALTERAR_ITEM";
                                        }
                                    }
                                    else
                                    {
                                        nVlrProdutos += total;
                                        nValorTotal.Text = total.ToString("N2");

                                        var item = Base_Pedidos_Itens.FirstOrDefault(i => i.idContador.ToString() == row.Cells[Col_idContador].Text);
                                        if (item != null)
                                        {
                                            item.nQuantidade = qtd;
                                            item.nValorUnitario = valor;
                                            item.nValorTotal = total;

                                            Dictionary<string, string> vParametros = new Dictionary<string, string>
                                            {
                                                { "@sFuncao", "CONSULTA_REGRAS__PEDIDOS" },
                                                { "@idPedido", item.idPedido.ToString() },
                                                { "@idItem", item.idProduto.ToString() },
                                                { "@nValorUnitario", valor.ToString().Replace(".", "").Replace(",", ".") },
                                                { "@nFretePrevisto", Conversoes.Numerico(txtFrete) }
                                            };
                                            DataTable dt = ExecutarDataTable(sProcedure, vParametros);

                                            item.nVlrDIFAL = total.DoubleToDecimal() * (decimal.Parse(dt.Rows[0]["nDIFAL"].ToString()) / 100);
                                            nVlrDIFAL += item.nVlrDIFAL.DecimalToDouble();

                                            if (item.nMVA > 0)
                                            {
                                                var st = CalculaValor_ST(total.DoubleToDecimal(), item.nVlrICMS * item.nQuantidade.DoubleToDecimal(), item.nMVA, item.nICMS_Interno_Destino);
                                                nVlrST += st.Item1.DecimalToDouble();
                                                item.nVlrST = st.Item1;
                                                item.nMVA = st.Item2;
                                            }

                                            if (item.sFuncao != "Inserir_item_envio")
                                                item.sFuncao = "ALTERAR_ITEM";
                                        }
                                    }
                                }
                                else
                                {
                                    double nValorUnitario = double.Parse((row.FindControl("Item_GV_txtnValorUnitario") as TextBox).Text);
                                    nVlrIPI = nValorUnitario * (double.Parse((row.FindControl("Item_GV_txtnIPI") as TextBox).Text) / 100);
                                    double qtd = double.Parse((row.FindControl("Item_GV_txtnQuantidade") as TextBox).Text);
                                    double desc = double.Parse((row.FindControl("Item_GV_txtnDesconto") as TextBox).Text);
                                    double total = qtd * (nValorUnitario + nVlrIPI);
                                    total -= total * (desc / 100);
                                    nVlrProdutos += total;
                                    nValorTotal.Text = total.ToString("N2");

                                    TextBox drevisao = (TextBox)row.FindControl("txtdtEntrega");
                                    if (DateTime.TryParse(drevisao.Text, out DateTime dataConvertida))
                                    {
                                        if (dataConvertida > maiorData)
                                        {
                                            maiorData = dataConvertida;
                                            txtdtEstimativaEntrega.Text = maiorData.ToString("dd/MM/yyyy");
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }

                if (idPedido() > 1879 || idPedido() == 0)
                {
                    txtnVlrProdutos.ReadOnly = true;
                    txtnVlrProdutos.Text = nVlrProdutos.ToString("N2");
                    txtImposto_ST.Text = nVlrST.ToString("N2");
                    txtImposto_DIFAL.Text = nVlrDIFAL.ToString("N2");
                    txtImpostos.Text = (nVlrST + nVlrDIFAL).ToString("N2");

                    double.TryParse(hddidTipo.Value.Equals("2") ? txtnFreteVendas.Text : txtFrete.Text, out double nVlrFrete);
                    double.TryParse(txtnVlrDesconto.Text, out double nVlrDesconto);
                    double.TryParse(txtCustoAduaneiro.Text.Replace("R$", ""), out double nCustoAduaneiro);
                    double.TryParse(txtCustoDespachante.Text.Replace("R$", ""), out double nCustoDespachante);

                    double total = nVlrProdutos + nVlrServicos;

                    if (hddidTipo.Value != "7")
                    {
                        txtnVlrServicos.ReadOnly = true;
                        txtnVlrServicos.Text = nVlrServicos.ToString("N2");

                        if (hddidTipo.Value.Equals("2"))
                            total += nVlrFrete + nVlrST + (div_CustoAduaneiro.Visible ? nCustoAduaneiro + nCustoDespachante : 0);

                        txtnVlrTotal.Text = total.ToString("N2");
                    }
                    else
                    {
                        if (txtnVlrServicos.Text != "") nVlrServicos = double.Parse(txtnVlrServicos.Text);

                        total = nVlrProdutos + nVlrServicos;

                        // Correção Divisão por zero - Thiago Rodrigues 10/02/2026
                        txtnDesconto.Text = nVlrDesconto == 0 || total == 0 ? "0,00" : (nVlrDesconto * 100 / total).ToString("N2");
                        total -= nVlrDesconto;

                        txtnVlrTotal.Text = total.ToString("N2");
                    }
                }

                upd_Pedido.Update();
            }
        }

        #endregion

        #region | Selected Index Changed

        protected void ddlidParceiro_Comissionador_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currentClass = ((HtmlGenericControl)div_ComissaoPaga).Attributes["class"];
            ((HtmlGenericControl)div_ComissaoPaga).Attributes["class"] = currentClass + " visible";
            div_ComissaoValor.Visible = false;

            if (ddlidParceiro_Comissionador.SelectedValue != "0")
            {
                currentClass = ((HtmlGenericControl)div_ComissaoPaga).Attributes["class"];
                ((HtmlGenericControl)div_ComissaoPaga).Attributes["class"] = currentClass.Replace("visible", "").Trim();
                div_ComissaoValor.Visible = true;
            }
            else
            {
                ComissaoPaga.Definir("N", "Paga", "");
            }
        }

        protected void ddlFluxoPedido_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as DropDownList).ID != "ddlFluxoPedido")
            {
                Contato_Despachante.Visible = true;
                Popula_Combo(ddlContato_Despachante, string.Format("sp_Select 'Flow_Contato_Importacao', @idPesquisa ={0}", ddlImportacao_idDespachante.SelectedValue), "idContato", "sNome", false, "Selecione o Contato do Despachante", "0");
            }

            if (hddidTipo.Value != "7")
            {
                try
                {
                    var Localiza = Base_Fluxo.Where(c => c.idFluxo.ToString().Equals(ddlFluxoPedido.SelectedValue)).Select(x => new { x.nTempoDias, x.nTempoTotalHoras }).ToList();
                    DateTime dtPrev = Convert.ToDateTime(txtdtPedido.Text);
                    dtPrev = dtPrev.AddDays(Localiza[0].nTempoDias);
                    txtdtEstimativaEntrega.Text = string.Format("{0:d}", dtPrev);

                    if (hddidTipo.Value == "2")
                    {
                        ValidaFluxoART(ddlFluxoPedido.SelectedValue);
                    }

                }
                catch
                {

                }
            }
            else
            {
                if (ddlFluxoPedido.SelectedValue == "19")
                {

                    ddlsEnderecoEntrega.Visible = false;
                    txtsEnderecoEntrega.Visible = true;
                    txtsEnderecoEntrega.ReadOnly = false;
                }
                else
                {

                    ddlsEnderecoEntrega.Visible = true;
                    txtsEnderecoEntrega.Visible = false;
                }
            }
        }

        protected void ddlExportador_SelectedIndexChanged(object sender, EventArgs e)
        {
            Contato_Exportador.Visible = true;
            Popula_Combo(ddlContato_Exportador, string.Format("sp_Select 'Flow_Contato_Importacao', @idPesquisa={0}", ddlImportacao_idFornecedor.SelectedValue), "idContato", "sNome", false, "Selecione o Contato do Exportador", "0");
        }

        protected void ddlEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
            {
                Contato_Empresa.Visible = true;
                Popula_Combo(ddlContato_Empresa, string.Format("sp_Select 'Flow_Contato_Empresa', @idPesquisa={0}", ddlidEmpresa.SelectedValue), "idContato", "sNome", false, "Selecione o Contato da Empresa", "0");
            }
            if (hddidTipo.Value == "7")
            {
                Popula_Combo(ddlsEnderecoEntrega, string.Format("sp_Manipula_tbl_Flow_Clientes 'CONSULTAR_ENDERECO_COMPRAS', {0}", ddlidEmpresa.SelectedValue), "idEndereco", "sEnderecoCompleto", false);
                if (ddlFluxoPedido.SelectedValue == "19")
                {
                    if (hddidPedido.Value == "")
                    {
                        ddlsEnderecoEntrega.Visible = false;
                        txtsEnderecoEntrega.Visible = true;
                        txtsEnderecoEntrega.ReadOnly = false;
                    }
                }
                else
                {
                    txtsEnderecoEntrega.Visible = false;
                    ddlsEnderecoEntrega.Visible = true;
                }

                if (hddidPedido.Value == "0" || hddidPedido.Value == "")
                {
                    if (ddlsTipoCompra.SelectedValue != "0" && ddlidEmpresa.SelectedValue != "0")
                    {
                        string Pedido = "";
                        string sDscEmpresa = "";
                        string nPedido = "";
                        int anoAtual = DateTime.Now.Year;
                        string anoFormatado = (anoAtual % 100).ToString("D2");

                        if (ddlsTipoCompra.SelectedValue == "N")
                            Pedido = "PC";
                        if (ddlsTipoCompra.SelectedValue == "I")
                            Pedido = "PO";

                        DataSet dsPesquisa;
                        Dictionary<String, String> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONSULTA_NUMERO_PEDIDO_COMPRA" },
                            { "@idEmpresa", ddlidEmpresa.SelectedValue }
                        };

                        dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

                        sDscEmpresa = DATASET(dsPesquisa, nTabela_Dados, "sDscEmpresaReduzida");
                        if (ddlsTipoCompra.SelectedValue == "N")
                            nPedido = DATASET(dsPesquisa, nTabela_Dados, "nPedidoNacional");
                        if (ddlsTipoCompra.SelectedValue == "I")
                            nPedido = DATASET(dsPesquisa, nTabela_Dados, "nPedidoInternacional");

                        txtsPedidoCliente.Text = sDscEmpresa + "." + Pedido + "." + anoFormatado;
                    }
                }
            }
        }

        protected void ddlImportador_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
            {
                Contato_Importador.Visible = true;
                Popula_Combo(ddlContato_Importador, string.Format("sp_Select 'Flow_Contato_Importacao', @idPesquisa={0}", ddlImportador.SelectedValue), "idContato", "sNome", false, "Selecione o Contato do Importador", "0");
            }
        }

        protected void ddlVendedor_SelectedIndexChanged(object sender, EventArgs e) { }

        protected void ddlTipoEnvio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTipoEnvio.SelectedValue == "1")
            {
                ddlsEnderecoEntrega.SelectedValue = "-1";
            }
        }

        protected void ddlFornecedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hddidTipo.Value == "7")
            {
                Popula_Combo(ddlCondPagamento, "sp_Select 'FLOW_CondicaoDePagamento', @idFiltro= 1, @idPesquisa=" + ddlFornecedor.SelectedValue, "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione a Condição de Pagamento", "0");

                if (ddlFornecedor.SelectedValue != "0" && ddlFornecedor.SelectedValue != "")
                    div_btnItens.Visible = true;
                else
                    div_btnItens.Visible = false;
            }
        }

        protected void ddlsTipoCompra_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hddidTipo.Value == "7")
            {
                Popula_Combo(ddlFornecedor, "sp_Select 'Flow_Parceiros_Fornecedores', @sTipo =" + ddlsTipoCompra.SelectedValue, "idCliente", "Razao_CNPJ", false, "Selecione o Fornecedor", "0");

                if (ddlsTipoCompra.SelectedValue == "I")
                {
                    this.dtgItens.Columns[Col_nICMS_Edit].Visible = false;
                    this.dtgItens.Columns[Col_nIPI_Edit].Visible = false;
                    updPanel_Itens.Update();
                    div_Moeda.Visible = true;
                }
                else
                {
                    this.dtgItens.Columns[Col_nICMS_Edit].Visible = true;
                    this.dtgItens.Columns[Col_nIPI_Edit].Visible = true;
                    updPanel_Itens.Update();
                    div_Moeda.Visible = false;
                }

                if (hddidPedido.Value == "0" || hddidPedido.Value == "")
                {
                    div_btnItens.Visible = false;
                    if (ddlsTipoCompra.SelectedValue != "0" && ddlidEmpresa.SelectedValue != "0")
                    {
                        string Pedido = "";
                        string sDscEmpresa = "";
                        string nPedido = "";
                        int anoAtual = DateTime.Now.Year;
                        string anoFormatado = (anoAtual % 100).ToString("D2");

                        if (ddlsTipoCompra.SelectedValue == "N")
                            Pedido = "PC";
                        if (ddlsTipoCompra.SelectedValue == "I")
                            Pedido = "PO";

                        DataSet dsPesquisa;
                        Dictionary<String, String> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONSULTA_NUMERO_PEDIDO_COMPRA" },
                            { "@idEmpresa", ddlidEmpresa.SelectedValue }
                        };

                        dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

                        sDscEmpresa = DATASET(dsPesquisa, nTabela_Dados, "sDscEmpresaReduzida");
                        if (ddlsTipoCompra.SelectedValue == "N")
                            nPedido = DATASET(dsPesquisa, nTabela_Dados, "nPedidoNacional");
                        if (ddlsTipoCompra.SelectedValue == "I")
                            nPedido = DATASET(dsPesquisa, nTabela_Dados, "nPedidoInternacional");

                        txtsPedidoCliente.Text = sDscEmpresa + "." + Pedido + "." + anoFormatado;
                    }
                }
            }
        }

        protected void ddlConceito_SelectedIndexChanged(object sender, EventArgs e)
        {
            Dictionary<String, String> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONSULTAR-FLAG-CONCEITO" },
                            { "@idConceito", ddlConceito.SelectedValue }
                        };
            DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametros, true);
            if (DATASET(ds, "sExibePatrimonio") == "S")
            {
                divGrupoPatrimonio.Visible = true;
                Popula_Combo(ddlGrupoPatrimonio, "sp_Select 'Flow_Patrimonio_Grupo'", "idPatrimonioGrupo", "sDscPatrimonio", false, "Selecione um Grupo de Patrimônio.", "0");
            }
            else
            {
                divGrupoPatrimonio.Visible = false;
                ddlGrupoPatrimonio.SelectedValue = "0";
            }

        }

        protected void ddlsMoeda_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hddidTipo.Value != "2")
                return;

            txtCambioMoeda.Text = "0,0000";
            sMoeda = "R$";

            if (ddlsMoeda.SelectedValue != "0")
            {
                DataSet ds = ExecutarDataSet(sProcedure_Moedas, new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_DETALHE" }, { "@idMoeda", ddlsMoeda.SelectedValue } });

                if (ValidarDataSet(ds, out _))
                {
                    sMoeda = DATASET(ds, "sSimbolo");

                    foreach (var item in Base_Pedidos_Itens)
                        item.sMoedaVenda = sMoeda;

                    decimal.TryParse(DATASET(ds, "nValorCambio"), out decimal nCambio);
                    txtCambioMoeda.Text = nCambio.ToString("N4");
                }
            }

            AtualizarSimboloMoeda(sMoeda);
            hddMoeda_Simbolo.Value = sMoeda;

            if (dtgItens.Rows.Count > 0)
                Item_GV_txtnValorUnitario_TextChanged(sender, e);
        }

        #endregion

        #endregion

        #region | Volumes

        bool GravarPedido_Volumes(string idPedido)
        {
            bool bRetorno = false;

            if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
            {
                try
                {
                    Importacao_Volumes_Insert("AtualizarClasse", 0, 0, 0, "", 0, 0, 0, 0, 0, 0, "", 0);

                    foreach (var linha in Base_Pedidos_Volumes)
                    {
                        int idArquivo = linha.idArquivo;

                        if (linha.sNomeArquivo != "" && linha.idArquivo == 0)
                        {
                            TT_Flow.FrameWork.cls_Arquivos Arquivo = new FrameWork.cls_Arquivos();
                            Arquivo.idTipoArquivo = 9999;
                            Arquivo.idObjeto = linha.idRegistroOcorrencia;
                            Arquivo.sNomeArquivo = linha.sNomeArquivo;
                            Arquivo.sDscArquivo = linha.sObservacaoArquivo;
                            Arquivo.sObservacao = "";
                            Arquivo.idUsuario = Convert.ToInt32(Variaveis.idUsuario());
                            Arquivo.vbArquivo = linha.objArquivo;
                            Arquivo.dtExpiracaoDoc = "";
                            linha.idArquivo = Convert.ToInt32(DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));
                        }

                        DataSet dsEnvios;
                        Dictionary<String, String> vParametrosSalvarEnvios = new Dictionary<string, string>
                            {
                                {"@sFuncao",                        linha.sFuncao.ToString()},
                                {"@idRegistro",                     linha.idRegistro.ToString()},
                                {"@idPedido",                       idPedido },
                                {"@sNomeArquivo",                   linha.sNomeArquivo == null ? "" : linha.sNomeArquivo},
                                {"@Importacao_idMeioEnvio",         linha.idMeioEnvio.ToString()},
                                {"@idUsuarioInclusao",              Variaveis.idUsuario()},
                                {"@nPesoEnvio",                     linha.nPesoEnvio.ToString().Replace (",", ".")},
                                {"@nLarguraEnvio",                  linha.nLarguraEnvio.ToString().Replace (",", ".")},
                                {"@nComprimentoEnvio",              linha.nComprimentoEnvio.ToString().Replace (",", ".")},
                                {"@nAlturaEnvio",                   linha.nAlturaEnvio.ToString().Replace (",", ".")},
                                {"@nVolume",                        linha.nVolume.ToString().Replace (",", ".")},
                                {"@nOrdemEnvio",                    linha.idContador.ToString()},
                                {"@idArquivo",                      linha.sNomeArquivo == null ? "0" : linha.idArquivo.ToString()}
                    };

                        if (linha.sFuncao != "SALVAR VOLUMES" && linha.sFuncao != "ALTERAR VOLUMES" && linha.sFuncao != "EXCLUIR VOLUMES")
                            continue;

                        dsEnvios = ExecutarDataSet(sProcedure, vParametrosSalvarEnvios);
                    }
                    bRetorno = true;
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao Salvar Volumes: " + ex.Message);
                }
            }
            else
            {
                bRetorno = true;
            }

            return bRetorno;
        }

        void gvImportacao_Volumes_SalvarGRID()
        {
            foreach (GridViewRow item in gvImportacao_Volumes.Rows)
            {
                if (item.RowType == DataControlRowType.DataRow)
                {
                    try
                    {
                        int idContador = 0;
                        cls_Pedidos_Envios objEnvios = Base_Pedidos_Volumes.FirstOrDefault(e => e.idRegistro == Int32.Parse(gvImportacao_Volumes.DataKeys[item.RowIndex]["idRegistro"].ToString()));
                        if (objEnvios.idContador != Convert.ToInt32((item.FindControl("txtidContador") as TextBox).Text))
                        {
                            if (objEnvios.sFuncao == "SALVAR VOLUMES")
                                objEnvios.idContador = Convert.ToInt32((item.FindControl("txtidContador") as TextBox).Text);
                            else
                                objEnvios.sFuncao = "ALTERAR VOLUMES";
                        }
                        if (objEnvios.nPesoEnvio != decimal.Parse((item.FindControl("txtnPesoEnvio") as TextBox).Text))
                        {
                            if (objEnvios.sFuncao == "SALVAR VOLUMES")
                                objEnvios.nPesoEnvio = decimal.Parse((item.FindControl("txtnPesoEnvio") as TextBox).Text);
                            else
                                objEnvios.sFuncao = "ALTERAR VOLUMES";
                        }
                        if (objEnvios.nLarguraEnvio != decimal.Parse((item.FindControl("txtnLarguraEnvio") as TextBox).Text))
                        {
                            if (objEnvios.sFuncao == "SALVAR VOLUMES")
                                objEnvios.nLarguraEnvio = decimal.Parse((item.FindControl("txtnLarguraEnvio") as TextBox).Text);
                            else
                                objEnvios.sFuncao = "ALTERAR VOLUMES";
                        }
                        if (objEnvios.nComprimentoEnvio != decimal.Parse((item.FindControl("txtnComprimentoEnvio") as TextBox).Text))
                        {
                            if (objEnvios.sFuncao == "SALVAR VOLUMES")
                                objEnvios.nComprimentoEnvio = decimal.Parse((item.FindControl("txtnComprimentoEnvio") as TextBox).Text);
                            else
                                objEnvios.sFuncao = "ALTERAR VOLUMES";
                        }
                        if (objEnvios.nAlturaEnvio != decimal.Parse((item.FindControl("txtnAlturaEnvio") as TextBox).Text))
                        {
                            if (objEnvios.sFuncao == "SALVAR VOLUMES")
                                objEnvios.nAlturaEnvio = decimal.Parse((item.FindControl("txtnAlturaEnvio") as TextBox).Text);
                            else
                                objEnvios.sFuncao = "ALTERAR VOLUMES";
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        void Importacao_Volumes_LimpaCampos()
        {
            ddlImportacao_MeioVolume.SelectedValue = "0";
            txtLargura.Text = "";
            txtComprimento.Text = "";
            txtAltura.Text = "";
            txtPeso.Text = "";
            txtQuantidade.Text = "";
        }

        void Importacao_Volumes_Insert(string sFuncao, int idRegistro, int idPedido, int idMeioEnvio, string sDscMeioEnvio, decimal nPesoEnvio, decimal nLarguraEnvio, decimal nComprimentoEnvio, decimal nAlturaEnvio, decimal nVolume, int idArquivo, string sNomeArquivo, int nOrdem)
        {
            try
            {
                if (sFuncao == "AtualizarClasse")
                {
                    foreach (GridViewRow envio in gvImportacao_Volumes.Rows)
                    {
                        try
                        {
                            cls_Pedidos_Envios objEnvios = Base_Pedidos_Volumes.FirstOrDefault(e => e.idRegistro == Int32.Parse(gvImportacao_Volumes.DataKeys[envio.RowIndex]["idRegistro"].ToString()));

                            if (objEnvios.nPesoEnvio != decimal.Parse((envio.FindControl("txtnPesoEnvio") as TextBox).Text) && objEnvios.sFuncao != "SALVAR VOLUMES" && objEnvios.sFuncao != "EXCLUIR VOLUMES" && objEnvios.sFuncao != "INEXISTENTE")
                                objEnvios.sFuncao = "ALTERAR VOLUMES";

                            if (objEnvios.nLarguraEnvio != decimal.Parse((envio.FindControl("txtnLarguraEnvio") as TextBox).Text) && objEnvios.sFuncao != "SALVAR VOLUMES" && objEnvios.sFuncao != "EXCLUIR VOLUMES" && objEnvios.sFuncao != "INEXISTENTE")
                                objEnvios.sFuncao = "ALTERAR VOLUMES";

                            if (objEnvios.nComprimentoEnvio != decimal.Parse((envio.FindControl("txtnComprimentoEnvio") as TextBox).Text) && objEnvios.sFuncao != "SALVAR VOLUMES" && objEnvios.sFuncao != "EXCLUIR VOLUMES" && objEnvios.sFuncao != "INEXISTENTE")
                                objEnvios.sFuncao = "ALTERAR VOLUMES";

                            if (objEnvios.nAlturaEnvio != decimal.Parse((envio.FindControl("txtnAlturaEnvio") as TextBox).Text) && objEnvios.sFuncao != "SALVAR VOLUMES" && objEnvios.sFuncao != "EXCLUIR VOLUMES" && objEnvios.sFuncao != "INEXISTENTE")
                                objEnvios.sFuncao = "ALTERAR VOLUMES";

                            if ((objEnvios.sNomeArquivo != "" && objEnvios.sNomeArquivo != null) && objEnvios.sFuncao != "SALVAR VOLUMES" && objEnvios.sFuncao != "EXCLUIR VOLUMES" && objEnvios.sFuncao != "INEXISTENTE")
                                objEnvios.sFuncao = "ALTERAR VOLUMES";

                            if (objEnvios.idContador != Convert.ToInt32((envio.FindControl("txtidContador") as TextBox).Text) && objEnvios.sFuncao != "SALVAR VOLUMES" && objEnvios.sFuncao != "EXCLUIR VOLUMES" && objEnvios.sFuncao != "INEXISTENTE")
                                objEnvios.sFuncao = "ALTERAR VOLUMES";


                            objEnvios.idContador = Convert.ToInt32((envio.FindControl("txtidContador") as TextBox).Text);
                            objEnvios.nPesoEnvio = decimal.Parse((envio.FindControl("txtnPesoEnvio") as TextBox).Text);
                            objEnvios.nLarguraEnvio = decimal.Parse((envio.FindControl("txtnLarguraEnvio") as TextBox).Text);
                            objEnvios.nComprimentoEnvio = decimal.Parse((envio.FindControl("txtnComprimentoEnvio") as TextBox).Text);
                            objEnvios.nAlturaEnvio = decimal.Parse((envio.FindControl("txtnAlturaEnvio") as TextBox).Text);
                            var total = (objEnvios.nLarguraEnvio * objEnvios.nComprimentoEnvio * objEnvios.nAlturaEnvio);
                            objEnvios.nVolume = total;
                            objEnvios.idMeioEnvio = int.Parse((envio.FindControl("sDscMeioEnvio") as Label).Text.Split('-')[0].Trim());

                            int _idArquivo = 0;
                            int.TryParse((envio.FindControl("idArquivo") as LinkButton).ToString(), out _idArquivo);
                            objEnvios.idArquivo = _idArquivo;
                        }
                        catch
                        {

                        }
                    }
                }
                else
                {
                    FrameWork.cls_Pedidos_Envios objEnvios = new FrameWork.cls_Pedidos_Envios();
                    objEnvios.idPedido = idPedido;

                    try
                    {
                        objEnvios.idRegistro = sFuncao == "SALVAR VOLUMES" ? Base_Pedidos_Volumes.OrderBy(x => x.idRegistro).Last().idRegistro + 1 : idRegistro;
                    }
                    catch
                    {
                        objEnvios.idRegistro = 1;
                    }

                    objEnvios.sFuncao = sFuncao;
                    objEnvios.idPedido = idPedido;
                    objEnvios.idMeioEnvio = idMeioEnvio;
                    objEnvios.sDscMeioEnvio = sDscMeioEnvio;
                    objEnvios.nPesoEnvio = nPesoEnvio;
                    objEnvios.nLarguraEnvio = nLarguraEnvio;
                    objEnvios.nComprimentoEnvio = nComprimentoEnvio;
                    objEnvios.nAlturaEnvio = nAlturaEnvio;
                    objEnvios.nVolume = nVolume;
                    objEnvios.idArquivo = idArquivo;

                    if (sNomeArquivo != "")
                    {
                        objEnvios.sNomeArquivo = sNomeArquivo.ToString();
                    }

                    objEnvios.idContador = nOrdem;
                    Base_Pedidos_Volumes.Add(objEnvios);
                }
            }
            catch (Exception ex)
            {
                msgImportacao_Envio.MostraMensagem_Erro("Erro ao Incluir VOLUMES: " + ex.Message);
            }

            gvImportacao_Volumes.Columns[0].Visible = false;
        }

        protected void cmdIncluirVolumes_Click(object sender, EventArgs e)
        {
            int quantidade = 0;
            decimal Peso = 0;
            decimal Largura = 0;
            decimal Comprimento = 0;
            decimal Altura = 0;

            if (txtQuantidade.Text != "")
                quantidade = int.Parse(txtQuantidade.Text);

            if (txtPeso.Text != "")
                Peso = decimal.Parse(txtPeso.Text);

            if (txtLargura.Text != "")
                Largura = decimal.Parse(txtLargura.Text);

            if (txtComprimento.Text != "")
                Comprimento = decimal.Parse(txtComprimento.Text);

            if (txtAltura.Text != "")
                Altura = decimal.Parse(txtAltura.Text);

            if (Importacao_Volumes_Validar())
            {
                int count = 0;
                while (count < quantidade)
                {
                    Importacao_Volumes_Insert("SALVAR VOLUMES"
                                        , 0
                                        , idPedido()
                                        , Convert.ToInt32(ddlImportacao_MeioVolume.SelectedValue)
                                        , ddlImportacao_MeioVolume.SelectedItem.ToString()
                                        , Peso
                                        , Largura
                                        , Comprimento
                                        , Altura
                                        , Largura * Comprimento * Altura
                                        , 0
                                        , ""
                                        , Base_Pedidos_Volumes.Where(x => x.sFuncao != "EXCLUIR VOLUMES" && x.sFuncao != "INEXISTENTE").ToList().Count + 1
                                        );

                    Importacao_Volumes_Insert("AtualizarClasse", 0, 0, 0, "", 0, 0, 0, 0, 0, 0, "", 0);
                    gvImportacao_Volumes_Databound();
                    Envio_hddEditarEnvios.Value = "S";

                    count++;
                }
                if (quantidade == 0)
                {
                    Importacao_Volumes_Insert("SALVAR VOLUMES"
                                         , 0
                                         , idPedido()
                                         , Convert.ToInt32(ddlImportacao_MeioVolume.SelectedValue)
                                         , ddlImportacao_MeioVolume.SelectedItem.ToString()
                                         , Peso
                                         , Largura
                                         , Comprimento
                                         , Altura
                                         , Largura * Comprimento * Altura
                                         , 0
                                         , ""
                                         , Base_Pedidos_Volumes.Where(x => x.sFuncao != "EXCLUIR VOLUMES" && x.sFuncao != "INEXISTENTE").ToList().Count + 1
                                         );

                    Importacao_Volumes_Insert("AtualizarClasse", 0, 0, 0, "", 0, 0, 0, 0, 0, 0, "", 0);
                    gvImportacao_Volumes_Databound();
                    Envio_hddEditarEnvios.Value = "S";
                }
                Importacao_Volumes_LimpaCampos();
            }
        }

        bool Importacao_Volumes_Validar()
        {
            string sMensagemErro = "";

            if (ddlImportacao_MeioVolume.SelectedValue == "0")
            {
                sMensagemErro += "Selecione o meio de Volume <br/>";
            }

            if (sMensagemErro != "")
            {
                msgImportacao_Envio.MostraMensagem_Erro(sMensagemErro);
            }

            return sMensagemErro != "" ? false : true;
        }

        void gvImportacao_Volumes_Databound()
        {
            gvImportacao_Volumes.DataSource = Base_Pedidos_Volumes.Where(c => c.sFuncao.ToString() != "EXCLUIR VOLUMES" && c.sFuncao.ToString() != "INEXISTENTE").OrderBy(x => x.idContador).ToList();
            gvImportacao_Volumes.DataBind();
        }

        void Importacao_Volumes_PopularCombos()
        {
            DIV_Volumes.Visible = false;
            if (ddlidModal.SelectedValue != "0")
            {
                Popula_Combo(ddlImportacao_MeioVolume, "sp_Select 'tbl_Flow_Pedidos_Modal_MeioEnvio', '0'", "idMeioEnvio", "sDscMeioEnvio", false, "Selecione o Meio de Volume", "0");
                ddlImportacao_MeioVolume.Items.Remove(ddlImportacao_MeioVolume.Items.FindByValue("2"));
                ddlImportacao_MeioVolume.Items.Remove(ddlImportacao_MeioVolume.Items.FindByValue("3"));
                DIV_Volumes.Visible = true;
            }
            updVolumes.Update();
        }

        void gvImportacao_Volumes_Popular(DataSet ds, string sMetodoChamada)
        {
            DIV_Volumes_Selecao.Visible = false;
            this.gvImportacao_Volumes.Columns[0].Visible = false;
            this.gvImportacao_Volumes.Columns[1].Visible = true;
            this.gvImportacao_Volumes.Columns[2].Visible = false;
            this.gvImportacao_Volumes.Columns[3].Visible = false;
            this.gvImportacao_Volumes.Columns[4].Visible = false;
            this.gvImportacao_Volumes.Columns[5].Visible = false;
            this.gvImportacao_Volumes.Columns[6].Visible = false;
            this.gvImportacao_Volumes.Columns[7].Visible = true;
            this.gvImportacao_Volumes.Columns[8].Visible = false;
            this.gvImportacao_Volumes.Columns[9].Visible = true;
            this.gvImportacao_Volumes.Columns[10].Visible = false;
            this.gvImportacao_Volumes.Columns[11].Visible = true;
            this.gvImportacao_Volumes.Columns[12].Visible = false;
            this.gvImportacao_Volumes.Columns[13].Visible = true;
            this.gvImportacao_Volumes.Columns[14].Visible = false;
            this.gvImportacao_Volumes.Columns[15].Visible = true;
            this.gvImportacao_Volumes.Columns[16].Visible = true;
            this.gvImportacao_Volumes.Columns[17].Visible = false;
            this.gvImportacao_Volumes.Columns[18].Visible = true;

            foreach (DataRow row in ds.Tables[nTabela_Volumes].Rows)
            {

                Importacao_Volumes_Insert("CONSULTAR VOLUMES"
                                            , Convert.ToInt32(row["idRegistro"].ToString())
                                            , idPedido()
                                            , Convert.ToInt32(row["idMeioEnvio"])
                                            , row["sDscMeioEnvio"].ToString()
                                            , decimal.Parse(row["nPesoEnvio"].ToString())
                                            , decimal.Parse(row["nLarguraEnvio"].ToString())
                                            , decimal.Parse(row["nComprimentoEnvio"].ToString())
                                            , decimal.Parse(row["nAlturaEnvio"].ToString())
                                            , decimal.Parse(row["nVolume"].ToString())
                                            , int.Parse(row["idArquivo"].ToString())
                                            , row["sNomeArquivo"].ToString()
                                            , int.Parse(row["nOrdem"].ToString())
                   );
            }

            gvImportacao_Volumes_Databound();
            DIV_Volumes.Visible = true;

            if (sMetodoChamada == "Editar")
            {
                TextInvoiceNumber.ReadOnly = false;
                txtnOrderNumber.ReadOnly = false;
                txtsZonaEnvio.ReadOnly = false;
                Importacao_Volumes_PopularCombos();
                DIV_Volumes_Selecao.Visible = true;
                this.gvImportacao_Volumes.Columns[0].Visible = false;
                this.gvImportacao_Volumes.Columns[1].Visible = false;
                this.gvImportacao_Volumes.Columns[2].Visible = true;
                this.gvImportacao_Volumes.Columns[3].Visible = false;
                this.gvImportacao_Volumes.Columns[4].Visible = false;
                this.gvImportacao_Volumes.Columns[5].Visible = false;
                this.gvImportacao_Volumes.Columns[6].Visible = false;
                this.gvImportacao_Volumes.Columns[7].Visible = true;
                this.gvImportacao_Volumes.Columns[8].Visible = true;
                this.gvImportacao_Volumes.Columns[9].Visible = false;
                this.gvImportacao_Volumes.Columns[10].Visible = true;
                this.gvImportacao_Volumes.Columns[11].Visible = false;
                this.gvImportacao_Volumes.Columns[12].Visible = true;
                this.gvImportacao_Volumes.Columns[13].Visible = false;
                this.gvImportacao_Volumes.Columns[14].Visible = true;
                this.gvImportacao_Volumes.Columns[15].Visible = false;
                this.gvImportacao_Volumes.Columns[16].Visible = true;
                this.gvImportacao_Volumes.Columns[17].Visible = true;
                this.gvImportacao_Volumes.Columns[18].Visible = false;
            }
        }

        protected void gvImportacao_Volumes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColunaBotoes = e.Row.Cells.Count - 2;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = (e.Row.FindControl("lnkEnvio_Download") as LinkButton).ToolTip;
                //int idArquivo = Base_Pedidos_Volumes.Where(x => x.idContador.Equals(e.Row.Cells[0])).FirstOrDefault().idArquivo;

                foreach (LinkButton lnk in e.Row.Cells[nColunaBotoes].Controls.OfType<LinkButton>())
                {
                    if ((lnk.ID == "lnkEnvio_Download" && sNomeArquivo == "") || (lnk.ID == "lnkEnvio_UpLoad" && sNomeArquivo != "")/* || (lnk.ID == "lnkEnvio_UpLoad" && idArquivo != 0)*/)
                    {
                        lnk.Visible = false;
                    }
                }
            }

            int nColunaBotao = e.Row.Cells.Count - 1;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = (e.Row.FindControl("lnkEnvio_Download") as LinkButton).ToolTip;

                foreach (LinkButton lnk in e.Row.Cells[nColunaBotao].Controls.OfType<LinkButton>())
                {
                    if (sNomeArquivo == "")
                    {
                        lnk.Visible = false;
                    }
                }
            }
        }

        protected void gvImportacao_Volumes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            int idRegistro = Convert.ToInt32(gvImportacao_Volumes.DataKeys[e.RowIndex]["idRegistro"].ToString());
            string sFuncao = Base_Pedidos_Volumes[Base_Pedidos_Volumes.FindIndex(x => x.idRegistro.Equals(idRegistro))].sFuncao;

            if (sFuncao == "SALVAR VOLUMES")
                Base_Pedidos_Volumes[Base_Pedidos_Volumes.FindIndex(x => x.idRegistro.Equals(idRegistro))].sFuncao = "INEXISTENTE";
            else
                Base_Pedidos_Volumes[Base_Pedidos_Volumes.FindIndex(x => x.idRegistro.Equals(idRegistro))].sFuncao = "EXCLUIR VOLUMES";

            foreach (var envio in Base_Pedidos_Volumes.Where(x => x.idContador > Convert.ToInt32(e.Keys[0].ToString())))
            {
                envio.idContador -= 1;

                if (envio.sFuncao != "SALVAR VOLUMES" && envio.sFuncao != "EXCLUIR VOLUMES" && envio.sFuncao != "INEXISTENTE")
                    envio.sFuncao = "ALTERAR VOLUMES";
            }

            Importacao_Volumes_Insert("AtualizarClasse", 0, 0, 0, "", 0, 0, 0, 0, 0, 0, "", 0);
            hddsAlteracaoItens.Value = "S";
            gvImportacao_Volumes_Databound();
        }

        protected void gvImportacao_Volumes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idRegistro = int.Parse(e.CommandArgument.ToString());
            hddsBloco.Value = "envio";
            hddidLinha_Envio.Value = idRegistro.ToString();

            if (e.CommandName == "Upload_Arquivo")
            {
                AbrirModal_EnvioArquivo(eBloco.envio, idRegistro, Base_Pedidos_Volumes[Base_Pedidos_Volumes.FindIndex(x => x.idRegistro.Equals(idRegistro))].sDscTipoEnvio);
            }
            else if (e.CommandName == "Download_Arquivo")
            {
                Efetuar_Download_Arquivo(eBloco.envio, idRegistro);
            }

            RegistraScript("");
        }

        protected void ddlImportacao_TipoVolumes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlidModal_SelectedIndexChanged(object sender, EventArgs e)
        {
            Importacao_Volumes_PopularCombos();
            Importacao_Envios_PopularCombos();
        }

        void AbrirModal_EnvioArquivo(eBloco bloco, int idRegistro, string sTituloModal)
        {
            div8.Visible = false;
            div4.Visible = true;
            div14.Visible = false;
            lblEnviarArquivos_Titulo.Text = sTituloModal;
            txtEnviarArquivo_sDscArquivo.Text = "";
            hddIdLinha.Value = idRegistro.ToString();
            hddsBloco.Value = bloco.ToString();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_AbrirModalUploadArquivos", "$('#UploadArquivos_Modal').modal('show')", true);
        }

        protected void cmdEnviarArquivos_Click(object sender, EventArgs e)
        {
            string sJSExecutar = "";

            if (fu_EnviarArquivo.HasFile)
            {
                int index;
                int idRegistro = Convert.ToInt32(hddIdLinha.Value);
                eBloco bloco = (eBloco)Enum.Parse(typeof(eBloco), hddsBloco.Value);
                Byte[] lObjArquivo = null;

                try
                {
                    TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                    lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(fu_EnviarArquivo.FileName, fu_EnviarArquivo.PostedFile.InputStream);

                    switch (bloco)
                    {
                        case eBloco.envio:
                            index = Base_Pedidos_Volumes.FindIndex(x => x.idRegistro.Equals(idRegistro));
                            Base_Pedidos_Volumes[index].idArquivo = 0;
                            Base_Pedidos_Volumes[index].sNomeArquivo = fu_EnviarArquivo.FileName;
                            Base_Pedidos_Volumes[index].objArquivo = lObjArquivo;
                            Base_Pedidos_Volumes[index].sObservacaoArquivo = txtEnviarArquivo_sDscArquivo.Text;
                            break;
                    }
                }
                catch
                {
                    MensagemPagina_EnviarArquivo.MostraMensagem_Erro("Selecione o Arquivo!");
                }
            }

            if (fu_EnviarArquivoPagamento.HasFile)
            {
                int index;
                int idLinha = Convert.ToInt32(hddIdLinha.Value);
                eBloco bloco = (eBloco)Enum.Parse(typeof(eBloco), hddsBloco.Value);
                Byte[] lObjArquivo = null;

                try
                {
                    TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                    lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(fu_EnviarArquivoPagamento.FileName, fu_EnviarArquivoPagamento.PostedFile.InputStream);

                    switch (bloco)
                    {
                        case eBloco.Pagamento:
                            index = bs_Pagamento.FindIndex(x => x.idLinha.Equals(idLinha));
                            bs_Pagamento[index].idArquivo = 0;
                            bs_Pagamento[index].sNomeArquivo = fu_EnviarArquivoPagamento.FileName;
                            bs_Pagamento[index].objArquivo = lObjArquivo;
                            bs_Pagamento[index].sObservacaoArquivo = txtEnviarArquivo_sDscArquivo.Text;
                            dtgPagamento_DataBind();
                            sJSExecutar = "$('#Resultado_tab').tab('show');";
                            break;
                    }
                }
                catch
                {
                    return;
                }
            }

            if (fu_EnviarArquivoGarantia.HasFile)
            {
                int index;
                int idLinha = Convert.ToInt32(hddIdLinha.Value);
                eBloco bloco = (eBloco)Enum.Parse(typeof(eBloco), hddsBloco.Value);
                Byte[] lObjArquivo = null;

                try
                {
                    TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                    lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(fu_EnviarArquivoGarantia.FileName, fu_EnviarArquivoGarantia.PostedFile.InputStream);

                    switch (bloco)
                    {
                        case eBloco.Garantia:
                            index = bs_Garantia.FindIndex(x => x.idLinha.Equals(idLinha));
                            bs_Garantia[index].idArquivo = 0;
                            bs_Garantia[index].sNomeArquivo = fu_EnviarArquivoGarantia.FileName;
                            bs_Garantia[index].objArquivo = lObjArquivo;
                            bs_Garantia[index].sObservacaoArquivo = txtEnviarArquivo_sDscArquivo.Text;
                            gv_Garantia_DataBind();
                            sJSExecutar = "$('#Resultado_tab').tab('show');";
                            break;
                    }
                }
                catch
                {
                    return;
                }
            }

            Importacao_Volumes_Insert("AtualizarClasse", 0, 0, 0, "", 0, 0, 0, 0, 0, 0, "", 0);
            gvImportacao_Volumes_Databound();
            updVolumes.Update();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ModalUpload_Fecha", "$('body').removeClass('modal-open').find('.modal-backdrop').remove();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_aba_resultado", sJSExecutar, true);
        }

        void Efetuar_Download_Arquivo(eBloco bloco, int idRegistro)
        {
            int index;
            int idArquivo = 0;
            string sNomeArquivo = "";
            byte[] bObjArquivo = null;
            string urlAtualPagina = Request.UrlReferrer.ToString().Replace(Request.RawUrl, "/Download/");

            switch (bloco)
            {
                case eBloco.envio:
                    index = Base_Pedidos_Volumes.FindIndex(x => x.idRegistro.Equals(idRegistro));
                    sNomeArquivo = Base_Pedidos_Volumes[index].sNomeArquivo;
                    bObjArquivo = Base_Pedidos_Volumes[index].objArquivo;
                    idArquivo = Base_Pedidos_Volumes[index].idArquivo;
                    break;
            }

            if (bObjArquivo == null)
            {
                Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
                DataTable dtArquivo;
                vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "CONSULTAR_DETALHE" },
                        {"@idArquivo",                  idArquivo.ToString()}
                    };

                dtArquivo = ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

                foreach (DataRow item in dtArquivo.Rows)
                {
                    try
                    {
                        sNomeArquivo = item["sNomeArquivo"].ToString();
                        bObjArquivo = (byte[])item["vbArquivo"];
                    }
                    catch
                    {

                    }
                }
            }

            //Gera o Arquivo
            TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
            FileStream lObjFile = objArquivo.TransformarArrayBytesEmArquivo(bObjArquivo, Server.MapPath("~/Download/" + sNomeArquivo));
            lObjFile.Close();
            lObjFile.Dispose();

            //Efetua o Download
            StringBuilder strDownload = new StringBuilder();
            strDownload.AppendLine("var link = document.createElement('a');");
            strDownload.AppendLine("link.download = '" + sNomeArquivo + "';");
            strDownload.AppendLine("link.href = '" + string.Concat(urlAtualPagina, sNomeArquivo) + "';");
            strDownload.AppendLine("link.click();");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Download_dArquivos", strDownload.ToString(), true);
        }

        private enum eBloco
        {
            envio = 1,
            Pagamento = 2,
            Garantia = 3
        }

        #endregion

        #region | Aba LM

        void Popular_Aba_LM()
        {
            string sErro = "";
            string sTitulo_SelecioneLista = "Selecione a Lista de Material";
            int idTipoLM = 0;

            LM_cmdNova.Visible = false;
            LM_cmdImportar.Visible = false;
            LM_divLista.Visible = false;
            LM_ddlLista.Items.Clear();
            LM_ddlLista.Visible = false;
            LM_divImportar.Visible = false;

            if (Variaveis.idParceiro() != "0")
            {
                LM_chkEngenharia.Checked = true;
                idTipoLM = 1;
                chkObras.Visible = false;
                chkRetornoObras.Visible = false;
                chkCompraEmObras.Visible = false;
                LM_cmdNova.Visible = false;
                LM_cmdImportar.Visible = false;
            }

            if (LM_chkEngenharia.Checked || LM_chkObras.Checked || LM_chkRetornoObras.Checked || LM_chkCompraEmObras.Checked)
            {
                LM_ddlLista.Visible = true;
                if (((LM_chkEngenharia.Checked && !LM_chkObras.Checked && !LM_chkRetornoObras.Checked && !LM_chkCompraEmObras.Checked)
                    || (!LM_chkEngenharia.Checked && LM_chkObras.Checked && !LM_chkRetornoObras.Checked && !LM_chkCompraEmObras.Checked)
                    || (!LM_chkEngenharia.Checked && !LM_chkObras.Checked && LM_chkRetornoObras.Checked && !LM_chkCompraEmObras.Checked)
                    || (!LM_chkEngenharia.Checked && !LM_chkObras.Checked && !LM_chkRetornoObras.Checked && LM_chkCompraEmObras.Checked)
                   ) && Variaveis.idParceiro() == "0")
                {
                    LM_cmdNova.Visible = true;
                    if (LM_chkEngenharia.Checked)
                    {
                        idTipoLM = 1;
                        LM_cmdNova.Text = "Nova Lista de " + LM_chkEngenharia.Text;
                    }

                    if (LM_chkObras.Checked)
                    {
                        idTipoLM = 2;
                        LM_cmdNova.Text = "Nova Lista de " + LM_chkObras.Text;
                    }

                    if (LM_chkRetornoObras.Checked)
                    {
                        idTipoLM = 3;
                        LM_cmdNova.Text = "Nova Lista de " + LM_chkRetornoObras.Text;
                    }

                    if (LM_chkCompraEmObras.Checked)
                    {
                        idTipoLM = 4;
                        LM_cmdNova.Text = "Nova Lista de " + LM_chkCompraEmObras.Text;
                    }
                }
                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    ["@sFuncao"] = "CONSULTAR_LM_RESUMO",
                    ["@idPedido"] = hddidPedido.Value,
                    ["@idTipoLM"] = idTipoLM.ToString()
                };
                DataSet LM_ds = ExecutarDataSet(LM_sProcedure, vParametros);

                if (ValidarDataSet(LM_ds, out sErro))
                {
                    if (Variaveis.idParceiro() == "0")
                    {
                        LM_ddlLista.Items.Add(new ListItem("Selecione a Lista de Material", "0"));
                        foreach (DataRow row in LM_ds.Tables[0].Rows)
                        {
                            LM_ddlLista.Items.Add(new ListItem(row["sDscLM_Completa"].ToString(), row["idLM"].ToString()));
                        }

                        if (DATASET(LM_ds, 1, 0, "sLiberadoNovaLM") == "N")
                        {
                            LM_cmdNova.Visible = false;
                        }
                    }
                    else
                    {
                        LM_ddlLista.Items.Add(new ListItem(
                            DATASET(LM_ds, LM_ds.Tables[0].Rows.Count - 1, "sDscLM_Completa"),
                            DATASET(LM_ds, LM_ds.Tables[0].Rows.Count - 1, "idLM")
                            ));

                        LM_Pesquisar(LM_ddlLista.SelectedValue, "CONSULTAR", false);
                        LM_cmdVoltar.Visible = false;
                        LM_cmdGerarOPI.Visible = false;
                        LM_cmdEditaLM.Visible = false;
                    }
                }
                else
                {
                    LM_ddlLista.Items.Add(new ListItem("Nenhuma Lista de Material Encontrada", "0"));

                    if (!ValidaPermissao(Permissao.WMS.OPI.GerarOPI))
                    {
                        cmdGerarOPI.Visible = false;
                    }

                    if (idTipoLM == 1)
                    {
                        if (Variaveis.idParceiro() == "0")
                        {

                            LM_cmdImportar.Visible = true;
                        }
                    }
                }

            }
            aba_LM.Visible = true;
        }

        protected void LME_chk_CheckedChanged(object sender, EventArgs e) => Popular_Aba_LM();

        protected void LM_cmdNova_Click(object sender, EventArgs e)
        {
            string idUltimaLM = "0";
            string sTipoPesquisa = "NOVA";

            if (LM_chkEngenharia.Checked && LM_ddlLista.Items.Count > 1)
            {
                idUltimaLM = LM_ddlLista.Items[1].Value.ToString();
                sTipoPesquisa = "NOVA_VERSAO";
            }

            LM_cmdRecuperarSugestoes.Visible = true;
            LM_Pesquisar(idUltimaLM, sTipoPesquisa, false);
            hddidLM.Value = "";
            LM_cmdSalvaLM.Visible = false;
            RegistraScript("");
            Scripts.Mantem_AbaAtiva(Page, "lm-tab");
        }

        void LM_LimpaCampo()
        {
            LM_txtsDscLM.Text = "";
            LM_txtsObservacao.Text = "";
            LM_cmdGerarOPI.Visible = false;
            LM_cmdEditaLM.Visible = false;
            LM_cmdExportarExcel.Visible = false;
            LM_LimpaCampos_Itens();
            Base_LM_Itens.Clear();
            LM_gvItens_DataBind("Editar");
            LM_DIV_Arquivos.Visible = false;
            LM_frmArquivos.Attributes.Clear();
        }

        void LM_LimpaCampos_Itens()
        {
            LM_txtsCodigoProduto.Text = "";
            LM_txtsDscProduto.Text = "";
            LM_txtnQuantidade.Text = "";
            LM_hddidProduto.Value = "";
            LM_txtsAgrupamento.Text = "";
            LM_hddPesquisaPor.Value = "";
            LM_txtItens_sObservacao.Text = "";
        }

        protected void LM_ddlLista_SelectedIndexChanged(object sender, EventArgs e)
        {
            LM_divLista.Visible = false;
            LM_cmdSalvaLM.Visible = false;
            if (LM_chkEngenharia.Checked)
                LM_cmdEditaLM.Visible = false;

            if (LM_ddlLista.SelectedValue != "0")
                LM_Pesquisar(LM_ddlLista.SelectedValue, "CONSULTAR", false);
        }

        void LM_Pesquisar(string idObjetoPesquisa, string sTipoPesquisa, bool Popular_LM)
        {
            if (Popular_LM)
                Popular_Aba_LM();

            LM_LimpaCampo();

            if (Variaveis.idParceiro() == "0")
            {
                DataBind_dtgItens("CONSULTAR");
                VisibilidadeLinkProduto(false);
            }

            if (idObjetoPesquisa != "0")
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>();

                if (sTipoPesquisa == "IMPORTAR_LM")
                {
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE_IMPORTAR");
                    vParametros.Add("@idPedido", idObjetoPesquisa);
                }
                else
                {
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idLM", idObjetoPesquisa);

                }
                vParametros.Add("@idUsuario", Variaveis.idUsuario());
                DataSet dsPesquisa = ExecutarDataSet(LM_sProcedure, vParametros);

                if (ValidarDataSet(dsPesquisa, out string sErro))
                {
                    LM_txtsDscLM.Text = DATASET(dsPesquisa, 0, "sDscLM");
                    LM_txtsObservacao.Text = DATASET(dsPesquisa, 0, "sObservacao");

                    foreach (DataRow row in dsPesquisa.Tables[1].Rows)
                    {
                        LM_Incluir_Item(Convert.ToInt32(row["idRegistro"])
                                        , Convert.ToInt32(row["nOrdem"])
                                        , row["sAgrupamento"].ToString()
                                        , Convert.ToInt32(row["idProduto"])
                                        , row["sCodigo"].ToString()
                                        , row["sDscProduto"].ToString()
                                        , row["sUnidade"].ToString()
                                        , Convert.ToDouble(row["nQuantidade"].ToString())
                                        , "SALVAR_ITENS"
                                        , row["sObservacao"].ToString()
                                        , row["sEscopo"].ToString()
                                        , "N"
                                        , Convert.ToDouble(row["nValorUnitario"].ToString())
                                        , Convert.ToDecimal(row["nIPI"].ToString())
                                        , Convert.ToDouble(row["nTotal"].ToString())
                                        );
                    }

                    if (sTipoPesquisa.ToUpper() == "CONSULTAR")
                    {
                        hdd_dtLM.Value = DATASET(dsPesquisa, 0, "dtLM");
                        LM_lblTitulo.Text = DATASET(dsPesquisa, 0, "sDscLM_Completa");
                        LM_txtsDscLM.Text = DATASET(dsPesquisa, 0, "sDscLM");
                        LM_txtsObservacao.Text = DATASET(dsPesquisa, 0, "sObservacao");
                        LM_sDscOPI = LM_lblTitulo.Text;
                        LM_hddidLM.Value = DATASET(dsPesquisa, 0, "idLM");
                        idLMRecuperado = DATASET(dsPesquisa, 0, "idLM");
                        LM_PainelAtualizacao.Atualizar(DATASET(dsPesquisa, 0, "dtLM"), DATASET(dsPesquisa, 0, "sDscUsuario"));
                        LM_gvItens_DataBind("CONSULTAR");
                        LM_AjustaControles(false);
                        LM_cmdSalvarLM.Visible = false;
                        LM_cmdExportarExcel.Visible = true;
                        LM_divLista.Visible = true;
                        LM_frmArquivos.Attributes.Add("src", string.Format("Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}&sPermiteNovoArquivo={2}", idObjetoPesquisa, "LM", DATASET(dsPesquisa, 0, "sPermiteArquivo")));
                        LM_DIV_Arquivos.Visible = true;
                        LM_cmdRecuperarSugestoes.Visible = false;
                    }
                    else if (sTipoPesquisa == "NOVA_VERSAO")
                    {
                        LM_lblTitulo.Text = LM_cmdNova.Text + " - REV: " + (Convert.ToInt32(DATASET(dsPesquisa, 0, "idVersao")) + 1).ToString().PadLeft(2, '0');
                        LM_gvItens_DataBind("Editar");
                        LM_AjustaControles(true);
                        LM_cmdSalvarLM.Visible = true;
                        LM_divLista.Visible = true;
                        LM_cmdRecuperarSugestoes.Visible = true;
                        LM_cmdNova.Visible = false;
                        LM_chkObras.Enabled = false;
                        LM_chkEngenharia.Enabled = false;
                        LM_chkRetornoObras.Enabled = false;
                        LM_ddlLista.Visible = false;
                        LM_hddidLM.Value = "NOVA_VERSAO";
                    }
                    else if (sTipoPesquisa == "IMPORTAR_LM")
                    {
                        LM_lblTitulo.Text = LM_cmdNova.Text + " - Importada do Pedido N.º: " + (Convert.ToInt32(DATASET(dsPesquisa, 0, "idPedido"))).ToString().PadLeft(6, '0') + " - " + DATASET(dsPesquisa, 0, "sDscLM_Completa");
                        LM_gvItens_DataBind("Editar");
                        LM_AjustaControles(true);
                        LM_cmdSalvarLM.Visible = true;
                        LM_cmdRecuperarSugestoes.Visible = true;
                        LM_divLista.Visible = true;
                        LM_cmdNova.Visible = false;
                        LM_chkObras.Enabled = false;
                        LM_chkEngenharia.Enabled = false;
                        LM_chkRetornoObras.Enabled = false;
                        LM_ddlLista.Visible = false;
                        LM_divImportar.Visible = false;
                    }
                }
                else
                {
                    if (sTipoPesquisa == "IMPORTAR_LM")
                        LM_Importar_Mensagem.MostraMensagem_Erro("LM não localizada no Pedido!");
                }

                if (DATASET(dsPesquisa, 0, "idTipoLM") == "1")
                {
                    if (DATASET(dsPesquisa, 0, "idOPI") == "0" && LM_hddidLM.Value != "NOVA_VERSAO")
                    {
                        LM_cmdGerarOPI.Visible = true;
                        LM_cmdEditaLM.Visible = true;
                    }
                    else
                    {
                        LM_cmdGerarOPI.Visible = false;
                        LM_cmdEditaLM.Visible = false;
                    }
                }
                else if (DATASET(dsPesquisa, 0, "idTipoLM") == "4")
                {
                    LM_cmdGerarOPI.Visible = false;
                    LM_cmdEditaLM.Visible = true;
                }
                else
                {
                    LM_cmdGerarOPI.Visible = true;
                    LM_cmdEditaLM.Visible = true;
                }

                if (DATASET(dsPesquisa, 0, "idOPI") != "0")
                {
                    LM_cmdEditaLM.Visible = false;
                    LM_cmdGerarOPI.Visible = false;
                }

                if (LM_chkEngenharia.Checked)
                    LM_cmdEditaLM.Visible = false;

                if (!ValidaPermissao(Permissao.WMS.OPI.GerarOPI))
                {
                    cmdGerarOPI.Visible = false;
                    LM_cmdGerarOPI.Visible = false;
                }
            }
            else
            {
                LM_divLista.Visible = true;
                LM_cmdExportarExcel.Visible = false;
                LM_lblTitulo.Text = LM_cmdNova.Text;
                LM_cmdNova.Visible = false;
                LM_cmdImportar.Visible = false;
                LM_divImportar.Visible = false;
                LM_chkObras.Enabled = false;
                LM_chkEngenharia.Enabled = false;
                LM_chkRetornoObras.Enabled = false;
                LM_chkCompraEmObras.Enabled = false;
                LM_ddlLista.Visible = false;

                if (LM_chkEngenharia.Checked && LM_ddlLista.Items.Count == 1)
                {
                    LM_cmdRecuperarSugestoes.Visible = true;
                    //Gera a Primeira LM de Engenharia com os materiais do Pedido
                    Base_LM_Itens.Clear();
                    foreach (var linha in Base_Pedidos_Itens)
                    {
                        LM_Incluir_Item(0, linha.sDscCategoriaVendas, linha.idProduto, linha.sCodigoProduto, linha.sDscProduto, linha.sUnidade, linha.nQuantidade, "SALVAR_ITENS", "", "TTL", false, "S", 0, 0, 0);
                    }
                    LM_gvItens_DataBind("Editar");
                }

                if (LM_chkRetornoObras.Checked && LM_ddlLista.Items.Count != 0)
                {
                    LM_cmdRecuperarSugestoes.Visible = true;
                    //Gera a Primeira LM de Retorno Obras com os materiais de engenharia e obras
                    Base_LM_Itens.Clear();

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "RETORNO_OBRAS" },
                        { "@idPedido", hddidPedido.Value },
                        { "@sRetornoObras", "S" }
                    };
                    DataSet dsPesquisa = ExecutarDataSet(LM_sProcedure, vParametros);
                    
                    foreach (DataRow row in dsPesquisa.Tables[0].Rows)
                    {
                        LM_Incluir_Item(Convert.ToInt32(row["idRegistro"])
                                        , Convert.ToInt32(row["nOrdem"])
                                        , row["sAgrupamento"].ToString()
                                        , Convert.ToInt32(row["idProduto"])
                                        , row["sCodigo"].ToString()
                                        , row["sDscProduto"].ToString()
                                        , row["sUnidade"].ToString()
                                        , Convert.ToDouble(row["nQuantidade"].ToString()) == 0 ? Convert.ToDouble(row["nQuantidadeO"].ToString()) : Convert.ToDouble(row["nQuantidade"].ToString())
                                        , "SALVAR_ITENS"
                                        , row["sObservacao"].ToString()
                                        , row["sEscopo"].ToString()
                                        , row["sMsg"].ToString()
                                        , 0, 0, 0);
                    }
                    LM_gvItens_DataBind("Editar");
                }
                LM_AjustaControles(true);
            }

            if (!ValidaPermissao(Permissao.WMS.OPI.GerarOPI))
                LM_cmdGerarOPI.Visible = false;

            LM_DIV_Geral.Focus();
            LM_txtsDscLM.Focus();
        }

        void LM_AjustaControles(bool sAtivo)
        {
            LM_txtsDscLM.ReadOnly = !sAtivo;
            LM_txtsObservacao.ReadOnly = !sAtivo;
            LM_DIV_SelecaoItens.Visible = sAtivo;
            LM_PainelAtualizacao.Visible = !sAtivo;
            LM_divLista.Visible = sAtivo;
            LM_cmdSalvarLM.Visible = sAtivo;
        }

        void RedirecionaLM(string idLM, string sTipoLM)
        {
            switch (sTipoLM)
            {
                case "LME":
                    LM_chkEngenharia.Checked = true;
                    LM_chkObras.Enabled = false;
                    LM_chkEngenharia.Enabled = false;
                    LM_chkRetornoObras.Enabled = false;
                    LM_chkCompraEmObras.Enabled = false;
                    break;
                case "LMO":
                    LM_chkObras.Checked = true;
                    LM_chkObras.Enabled = false;
                    LM_chkEngenharia.Enabled = false;
                    LM_chkRetornoObras.Enabled = false;
                    LM_chkCompraEmObras.Enabled = false;
                    break;
                case "LMRO":
                    LM_chkRetornoObras.Checked = true;
                    LM_chkObras.Enabled = false;
                    LM_chkEngenharia.Enabled = false;
                    LM_chkRetornoObras.Enabled = false;
                    LM_chkCompraEmObras.Enabled = false;
                    break;
                case "LMCO":
                    LM_chkCompraEmObras.Checked = true;
                    LM_chkObras.Enabled = false;
                    LM_chkEngenharia.Enabled = false;
                    LM_chkRetornoObras.Enabled = false;
                    LM_chkCompraEmObras.Enabled = false;
                    break;
            }

            LM_Pesquisar(idLM, "CONSULTAR", false);

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_AbaAtiva_LM", " $('#lm-tab').tab('show');", true);
        }

        #region | Sugestão LM Funções

        #region | Função de Imagem

        protected void rptItemSugeridoLM_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Image img = (Image)e.Item.FindControl("imgProdutoPrincipal");

                TextBox txtIdProduto = e.Item.FindControl("txtIdProdutoSugestao") as TextBox;
                if (txtIdProduto != null)
                {
                    CarregaImgProdutoLM(txtIdProduto.Text, img);
                }
            }
        }

        protected void CarregaImgProdutoLM(string idProduto, Image img)
        {
            DataTable dsPesquisa;
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_IMAGEM" },
                { "@idTipoArquivo", "201" },
                { "@idObjeto", idProduto }
            };
            dsPesquisa = ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (dsPesquisa.Rows.Count > 0)
            {
                DataRow imgBd = dsPesquisa.Rows[0];
                string imgUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])imgBd["vbArquivo"]);
                img.ImageUrl = imgUrl;
                img.Visible = true;
            }
        }

        #endregion

        #region | Carregar Repeater

        protected void PopularRptSugestaoLM(string idProduto, double qtdProduto)
        {
            DataSet ds;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            string sProcedure = "sp_Manipula_tbl_Flow_Pedidos";
            vParametros.Add("@sFuncao", "CONSULTAR_SUGESTAO");
            vParametros.Add("@idProduto", idProduto);

            ds = ExecutarDataSet(sProcedure, vParametros);

            if (ds.Tables[0].Rows.Count > 0)
            {
                AtualizarQuantidades(ds, qtdProduto);
                rptItensSugestaoLM.DataSource = ds;
                rptItensSugestaoLM.DataBind();
                //Abrir o modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalSugestao", "$('#modalSugestaoLM').modal('show');", true);
            }
        }


        protected void PopularRptSugestaoLM(string idProduto, double qtdProduto, bool isRecuperar)
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                string sProcedure = "sp_Manipula_tbl_Flow_Pedidos";
                vParametros.Add("@sFuncao", "CONSULTAR_SUGESTAO");
                vParametros.Add("@idProduto", idProduto);

                DataSet ds = ExecutarDataSet(sProcedure, vParametros);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    // Importar as novas linhas do DataSet original para o DataSet dsSugestoes, caso necessário
                    if (dsSugestoes.Tables.Count == 0)
                    {
                        dsSugestoes.Tables.Add(ds.Tables[0].Clone());
                    }

                    // Iterar sobre as linhas do DataSet e somar as quantidades de sugestões para produtos com sugestões iguais
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        string sugestao = row["sDscProduto"].ToString();
                        double quantidade = Convert.ToDouble(row["nQuantidade"]);
                        // Verifica se a sugestão já existe no DataSet dsSugestoes
                        DataRow[] existingRows = dsSugestoes.Tables[0].Select($"sDscProduto = '{sugestao}'");

                        if (existingRows.Length > 0)
                        {
                            // Se a sugestão já existe, soma a quantidade atual com a quantidade existente
                            existingRows[0]["nQuantidade"] = Convert.ToDouble(existingRows[0]["nQuantidade"]) + quantidade;
                        }
                        else
                        {
                            dsSugestoes.Tables[0].ImportRow(row);
                        }
                    }
                    // Preencher o repeater com o DataSet completo
                    rptItensSugestaoLM.DataSource = dsSugestoes;
                    rptItensSugestaoLM.DataBind();
                    //Abrir o modal
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalSugestao", "$('#modalSugestaoLM').modal('show');", true);
                }
            }
            catch (Exception ex)
            {
                LM_Mensagem_Itens.MostraMensagem_Erro("Erro: " + ex);
            }
        }

        #endregion

        protected void AdicionarLM_Click(object sender, EventArgs e)
        {
            AdicionarProdutosLM(ObterDadosRepeaterLM());
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_CloseModal", "$('#modalSugestaoLM').modal('hide'); $('body').removeClass('modal-open'); $('.modal-backdrop').remove(); $('[id$=Item_txtsCodigoProduto]').focus();", true);
        }

        protected void AdicionarProdutosLM(DataSet dsProdutos)
        {
            foreach (DataRow row in dsProdutos.Tables["ItensLM"].Rows)
            {
                LM_Incluir_Item(0, 0, row["sAgrupamento"].ToString(), Convert.ToInt32(row["idProdutoSugestao"]), row["sCodigo"].ToString(), row["sDscProduto"].ToString(), row["sUnidade"].ToString(), Convert.ToDouble(row["nQuantidade"]), "SALVAR_ITENS", "", "TTL", "S", 0, 0, 0);

                LM_gvItens_DataBind("Editar");
            }
        }

        protected DataSet ObterDadosRepeaterLM()
        {
            //Criei para Obter os dados do repeater e adicionar na tabela
            DataSet ds = new DataSet();
            DataTable tabela = new DataTable("ItensLM");

            tabela.Columns.Add("sAgrupamento", typeof(string));
            tabela.Columns.Add("idProdutoSugestao", typeof(int));
            tabela.Columns.Add("sCodigo", typeof(string));
            tabela.Columns.Add("sDscProduto", typeof(string));
            tabela.Columns.Add("sUnidade", typeof(string));
            tabela.Columns.Add("nQuantidade", typeof(double));

            foreach (RepeaterItem item in rptItensSugestaoLM.Items)
            {
                if (Convert.ToDouble(((TextBox)item.FindControl("txtQuantidade")).Text) > 0)
                {
                    DataRow row = tabela.NewRow();
                    HiddenField hddsCodigo = (HiddenField)item.FindControl("hddsCodigo");
                    HiddenField hddsUnidade = (HiddenField)item.FindControl("hddsUnidade");
                    HiddenField hddsDscProduto = (HiddenField)item.FindControl("hddsDscProduto");
                    HiddenField hddsCategoriaVendas = (HiddenField)item.FindControl("hddsCategoriaVendas");
                    row["sAgrupamento"] = hddsCategoriaVendas.Value;
                    row["idProdutoSugestao"] = Convert.ToInt32(Convert.ToInt32(((TextBox)item.FindControl("txtIdProdutoSugestao")).Text));
                    row["sCodigo"] = hddsCodigo.Value;
                    row["sDscProduto"] = hddsDscProduto.Value;
                    row["sUnidade"] = hddsUnidade.Value;
                    row["nQuantidade"] = Convert.ToDouble(((TextBox)item.FindControl("txtQuantidade")).Text);
                    tabela.Rows.Add(row);
                }
            }

            ds.Tables.Add(tabela);
            return ds;
        }

        protected void CancelarLM_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_CloseModal", "$('#modalSugestaoLM').modal('hide'); $('body').removeClass('modal-open'); $('.modal-backdrop').remove(); $('[id$=Item_txtsCodigoProduto]').focus();", true);
        }

        #endregion

        protected void LM_cmdItens_Incluir_Click(object sender, EventArgs e)
        {
            string sMensagem = "";

            LM_gvItens_SalvarGRID();

            if (LM_ddlsUnidade.SelectedValue == "")
            {
                sMensagem = "Selecione a unidade!";
            }
            else if (string.IsNullOrEmpty(LM_txtsCodigoProduto.Text))
            {
                sMensagem = "Informe um código válido!";
                LM_txtsCodigoProduto.Focus();
            }
            else if (string.IsNullOrEmpty(LM_txtsDscProduto.Text))
            {
                sMensagem = "Informe um produto válido!";
                LM_txtsDscProduto.Focus();
            }
            else if (string.IsNullOrEmpty(LM_txtnQuantidade.Text))
            {
                sMensagem = "Informe a quantidade!";
                LM_txtnQuantidade.Focus();
            }

            if (sMensagem == "")
            {
                int idPedido = 0;
                string[] vidPedido = hddidPedido.Value.Split(',');
                string sFuncao = "INCLUIR ITEM";

                if (vidPedido[0].ToString() == "")
                {
                    idPedido = 0;
                }
                else
                {
                    idPedido = Convert.ToInt32(vidPedido[0].ToString());
                    sFuncao = "INCLUIR ITEM ALTERACAO";
                }

                LM_gvItens_SalvarGRID();
                LM_Incluir_Item(0, LM_txtsAgrupamento.Text, Convert.ToInt32(LM_hddidProduto.Value), LM_txtsCodigoProduto.Text, LM_txtsDscProduto.Text, LM_ddlsUnidade.SelectedValue, Convert.ToDouble(LM_txtnQuantidade.Text), "SALVAR_ITENS", LM_txtItens_sObservacao.Text, "TTL", true, "S", 0, 0, 0);
            }
            else
            {
                LM_Mensagem_Itens.MostraMensagem_Erro(sMensagem, false);
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=LM_txtsCodigoProduto]').focus();", true);
            LM_gvItens_DataBind("Editar");
        }

        void LM_Incluir_Item(int idRegistro, string sAgrupamento, int idProduto, string sCodigoProduto, string sDscProduto, string sUnidade, double nQuantidade, string sFuncao, string sObservacao, string sEscopo, bool exibeSugestao, string sMsg, double nValorUnitario, decimal nIPI, double nTotal)
        {
            LM_Incluir_Item(idRegistro, 0, sAgrupamento, idProduto, sCodigoProduto, sDscProduto, sUnidade, nQuantidade, sFuncao, sObservacao, sEscopo, sMsg, nValorUnitario, nIPI, nTotal);
            if (exibeSugestao)
                PopularRptSugestaoLM(idProduto.ToString(), nQuantidade);
        }

        void LM_Incluir_Item(int idRegistro, int nOrdem, string sAgrupamento, int idProduto, string sCodigoProduto, string sDscProduto, string sUnidade, double nQuantidade, string sFuncao, string sObservacao, string sEscopo, string sMsg, double nValorUnitario, decimal nIPI, double nTotal)
        {
            // Verifica se já existe um item com as mesmas características na lista
            var itemExistente = Base_LM_Itens.FirstOrDefault(c => c.idProduto == idProduto &&
                                                                   c.sObservacao.Equals(sObservacao, StringComparison.OrdinalIgnoreCase) &&
                                                                   c.sDscCategoriaVendas.Equals(sAgrupamento, StringComparison.OrdinalIgnoreCase));

            if (itemExistente != null)
            {
                // Se existe um item com as mesmas características, adiciona a quantidade ao item existente
                itemExistente.nQuantidade += nQuantidade;
            }
            else
            {
                // Se não existe, cria um novo item e adiciona à lista
                FrameWork.cls_Pedidos_Itens objItem = new FrameWork.cls_Pedidos_Itens();
                objItem.idContador = Base_LM_Itens.Count() + 1;
                objItem.nOrdem = Base_LM_Itens.Where(c => c.sDscCategoriaVendas.Equals(sAgrupamento, StringComparison.OrdinalIgnoreCase)).Count() + 1;
                objItem.sDscCategoriaVendas = sAgrupamento;
                objItem.idProduto = idProduto;
                objItem.sCodigoProduto = sCodigoProduto;
                objItem.sDscProduto = sDscProduto;
                objItem.sUnidade = sUnidade;
                objItem.nQuantidade = nQuantidade;
                objItem.sFuncao = sFuncao;
                objItem.sObservacao = sObservacao;
                objItem.sEscopo = sEscopo;
                objItem.idItem = idRegistro;

                if (LM_chkCompraEmObras.Checked)
                {
                    objItem.nValorUnitario = nValorUnitario;
                    objItem.nIPI = nIPI;
                    objItem.nTotal = nTotal;
                }
                Base_LM_Itens.Add(objItem);
            }

            LM_LimpaCampos_Itens();
        }

        protected void LM_txtsCodigoProduto_TextChanged(object sender, EventArgs e)
        {
            if (LM_hddPesquisaPor.Value != "DESCRICAO")
            {
                SqlDataReader sdr = ExecutarDataReader("sp_Select 'FLOW_Produtos_Codigo', 2, 'S', '" + LM_txtsCodigoProduto.Text + "'");
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        LM_txtsDscProduto.Text = sdr["sDscProduto"].ToString();
                        LM_ddlsUnidade.SelectedValue = sdr["sUnidade"].ToString();
                        LM_hddidProduto.Value = sdr["idItem"].ToString();
                        LM_txtsAgrupamento.Text = sdr["sDscCategoriaVendas"].ToString();
                        LM_hddPesquisaPor.Value = "";

                        ScriptManager.RegisterStartupScript(this, GetType(), "js_Focus", "$('[id$=LM_txtnQuantidade]').focus();", true);
                    }
                }
                else
                {
                    LM_LimpaCampos_Itens();
                    LM_Mensagem_Itens.MostraMensagem_Erro("Produto Não Localizado!", false);
                }

                sdr.Close();
            }
        }

        protected void LM_txtsDscProduto_TextChanged(object sender, EventArgs e)
        {

        }

        protected void LM_cmdVoltar_Click(object sender, EventArgs e)
        {
            LM_LimpaCampo();
            LM_divLista.Visible = false;
            LM_chkObras.Enabled = true;
            LM_chkEngenharia.Enabled = true;
            LM_chkRetornoObras.Enabled = true;
            LM_chkCompraEmObras.Enabled = true;
            LM_ddlLista.Visible = true;
            Popular_Aba_LM();
            LM_ddlLista.Attributes.Remove("disabled");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=aba_Historico], [id$=aba_Tarefas], [id$=aba_Arquivos], [id$=aba_Acoes], [id$=aba_Dados], [id$=aba_depara]').show();", true);
        }

        bool LM_AplicarValidacoes()
        {
            if (Validacoes.ValidarTexto(LM_txtsDscLM))
            {
                LM_Mensagem.MostraMensagem_Erro("Informe uma Descrição Válida!");
                LM_txtsDscLM.Focus();
                return false;
            }

            if (Base_LM_Itens.Count == 0)
            {
                PesquisarPedido(hddidPedido.Value.ToString(), "0", "0", "", hddidTipo.Value);
                //MensagemPagina2.MostraMensagem_Erro("Impossivel gravar uma LM sem Itens!");
                LM_chkEngenharia.Enabled = true;
                LM_chkObras.Enabled = true;
                LM_chkRetornoObras.Enabled = true;
                LM_chkCompraEmObras.Enabled = true;
                LM_Mensagem.MostraMensagem_Erro("Impossivel gravar uma LM sem Itens!");
                LM_cmdSalvaLM.Visible = false;
                return false;
            }
            return true;
        }

        void LM_Gravar()
        {
            string sFuncao = "SALVAR";
            string sErro = "";
            string idLM = "0";
            string idTipoLM = "";

            LM_gvItens_SalvarGRID();

            if (LM_AplicarValidacoes())
            {
                try
                {
                    if (LM_chkEngenharia.Checked)
                    {
                        idTipoLM = "1";
                    }
                    else if (LM_chkObras.Checked)
                    {
                        idTipoLM = "2";
                    }
                    else if (LM_chkRetornoObras.Checked)
                    {
                        idTipoLM = "3";
                    }
                    else if (LM_chkCompraEmObras.Checked)
                    {
                        idTipoLM = "4";
                    }

                    if (hddidLM.Value != "0")
                    {
                        idLM = hddidLM.Value;
                    }

                    string[] vidPedido = hddidPedido.Value.Split(',');
                    string idPedido = vidPedido[0].ToString();

                    DataSet dsGravar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", sFuncao },
                        { "@idPedido", idPedido },
                        { "@idTipoLM", idTipoLM },
                        { "@idLM", idLM },
                        { "@sDscLM", LM_txtsDscLM.Text },
                        { "@sObservacao", LM_txtsObservacao.Text },
                        { "@idUsuario", Variaveis.idUsuario() }
                    };
                    dsGravar = ExecutarDataSet(LM_sProcedure, vParametros);

                    if (ValidarDataSet(dsGravar, out sErro))
                    {
                        idLM = DATASET(dsGravar, 0, "idLM");

                        if (LM_Gravar_Itens(idLM, idTipoLM))
                        {
                            LM_Pesquisar(idLM, "Consultar", true);
                            depara_Pesquisar();
                            LM_Mensagem.MostraMensagem_Sucesso("LM Gravada com sucesso!");
                            hddidLM.Value = idLM;
                            LM_ddlLista.Visible = true;
                            LM_ddlLista.SelectedValue = idLM;
                            Popular_Aba_OPI(idPedido, "LM");
                        }
                    }
                    else
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }

                }
                catch (Exception ex)
                {
                    LM_Mensagem.MostraMensagem_Erro(ex.Message);
                    LM_gvItens_DataBind("Editar");
                }
                LM_cmdSalvaLM.Visible = false;
            }
            else
            {
                LM_gvItens_DataBind("Editar");
            }
            DataBind_DocumentosSTSO();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_MantemAbs_Ativa_LM", " $('#lm-tab').tab('show');", true);
        }

        bool LM_Gravar_Itens(string idLM, string idTipoLM)
        {
            bool bRetorno = false;

            try
            {
                foreach (var linha in Base_LM_Itens)
                {
                    if (idLM != "0")
                    {
                        DataSet dsItem;
                        Dictionary<String, String> vParametrosItem = new Dictionary<string, string>
                        {

                            {"@sFuncao",        linha.sFuncao },
                            {"@idLM",           idLM },
                            {"@idProduto",      linha.idProduto.ToString() },
                            {"@sCodigoProduto", linha.sCodigoProduto.ToString() },
                            {"@sUnidade",       linha.sUnidade},
                            {"@nOrdem",         linha.nOrdem.ToString()},
                            {"@nQuantidade",    Conversoes.Numerico(linha.nQuantidade)},
                            {"@sAgrupamento",   linha.sDscCategoriaVendas},
                            {"@sObservacao",    linha.sObservacao},
                            {"@sEscopo",        linha.sEscopo},
                            {"@idRegistro",     linha.idItem.ToString()}

                        };

                        if (idTipoLM == "4")
                        {
                            vParametrosItem.Add("@nValorUnitario", Conversoes.Numerico(linha.nValorUnitario));
                            vParametrosItem.Add("@nIPI", Conversoes.Numerico(linha.nIPI));
                            vParametrosItem.Add("@nTotal", Conversoes.Numerico(linha.nTotal));
                        }

                        dsItem = ExecutarDataSet(LM_sProcedure, vParametrosItem);
                    }
                }
                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao salvar Item: " + ex.Message);
            }

            return bRetorno;
        }

        void LM_gvItens_ConfigurarColunas(string sMetodoChamada)
        {
            int LM_nCol_Agrupamento_View = 0;
            int LM_nCol_Agrupamento_Edit = 1;
            int LM_nCol_Ordem_Edit = 2;
            int LM_nCol_Escopo_View = 5;
            int LM_nCol_Escopo_Edit = 6;
            int LM_nCol_Quantidade_View = 8;
            int LM_nCol_Quantidade_Edit = 9;
            int LM_nCol_ValorUnitario_View = 10;
            int LM_nCol_ValorUnitario_Edit = 11;
            int LM_nCol_IPI_View = 12;
            int LM_nCol_IPI_Edit = 13;
            int LM_nCol_Total_View = 14;
            int LM_nCol_Observacao_View = 15;
            int LM_nCol_Observacao_Edit = 16;
            int LM_nCol_Excluir = 17;
            bool bExibe = false;

            if (sMetodoChamada != "Editar")
            {
                bExibe = true;
            }

            this.LM_gvItens.Columns[LM_nCol_Agrupamento_View].Visible = bExibe;
            this.LM_gvItens.Columns[LM_nCol_Agrupamento_Edit].Visible = !bExibe;
            this.LM_gvItens.Columns[LM_nCol_Ordem_Edit].Visible = !bExibe;
            this.LM_gvItens.Columns[LM_nCol_Escopo_View].Visible = bExibe;
            this.LM_gvItens.Columns[LM_nCol_Escopo_Edit].Visible = !bExibe;
            this.LM_gvItens.Columns[LM_nCol_Quantidade_View].Visible = bExibe;
            this.LM_gvItens.Columns[LM_nCol_Quantidade_Edit].Visible = !bExibe;
            this.LM_gvItens.Columns[LM_nCol_Observacao_View].Visible = bExibe;
            this.LM_gvItens.Columns[LM_nCol_Observacao_Edit].Visible = !bExibe;
            this.LM_gvItens.Columns[LM_nCol_Excluir].Visible = !bExibe;

            if (!LM_chkCompraEmObras.Checked)
            {
                this.LM_gvItens.Columns[LM_nCol_ValorUnitario_Edit].Visible = false;
                this.LM_gvItens.Columns[LM_nCol_ValorUnitario_View].Visible = false;
                this.LM_gvItens.Columns[LM_nCol_IPI_Edit].Visible = false;
                this.LM_gvItens.Columns[LM_nCol_IPI_View].Visible = false;
                this.LM_gvItens.Columns[LM_nCol_Total_View].Visible = false;
            }
            else
            {
                this.LM_gvItens.Columns[LM_nCol_ValorUnitario_View].Visible = bExibe;
                this.LM_gvItens.Columns[LM_nCol_ValorUnitario_Edit].Visible = !bExibe;
                this.LM_gvItens.Columns[LM_nCol_IPI_View].Visible = bExibe;
                this.LM_gvItens.Columns[LM_nCol_IPI_Edit].Visible = !bExibe;
                this.LM_gvItens.Columns[LM_nCol_Total_View].Visible = true;
            }

            this.LM_gvItens.Columns[18].Visible = false;
        }

        void LM_gvItens_DataBind(string sMetodoChamada)
        {
            LM_gvItens.DataSource = Base_LM_Itens.Where(x => x.sFuncao != "EXCLUIR_ITENS" && x.sFuncao != "INEXISTENTE").OrderBy(x => x.sDscCategoriaVendas).ThenBy(y => y.nOrdem).ToList();

            try
            {
                LM_gvItens.DataBind();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.ToString());
            }

            LM_gvItens.Visible = true;

            LM_gvItens_ConfigurarColunas(sMetodoChamada);
            if (sMetodoChamada != "Editar")
            {
                GridViewHelper helper = new GridViewHelper(LM_gvItens);
                helper.GroupHeader += new GroupEvent(helper_GroupHeader);
                helper.RegisterGroup("sDscCategoriaVendas", true, true);
                helper.ApplyGroupSort();
            }
        }

        #region | Recuperar Sugestões

        protected void LM_cmdRecuperar_Click(object sender, EventArgs e)
        {
            try
            {
                bool contemSugestoes = false;

                foreach (GridViewRow row in LM_gvItens.Rows)
                {
                    // Encontrar os controles dentro da linha
                    Label lblQuantidade = row.FindControl("lblnQuantidade") as Label;
                    Label lblIdProduto = row.FindControl("lblidProduto") as Label;

                    // Verificar se os controles foram encontrados
                    if (!string.IsNullOrEmpty(lblQuantidade.ToString()) && !string.IsNullOrEmpty(lblIdProduto.ToString()))
                    {
                        // Acessar os valores dos controles
                        string idProduto = lblIdProduto.Text;
                        string quantidade = lblQuantidade.Text;

                        if (idProduto != "" && quantidade != "")
                        {
                            PopularRptSugestaoLM(idProduto, Convert.ToDouble(quantidade), true);

                            if (dsSugestoes.Tables.Count > 0 && dsSugestoes.Tables[0].Rows.Count > 0)
                            {
                                contemSugestoes = true;
                            }
                        }
                    }
                }
                if (!contemSugestoes)
                {
                    LM_Mensagem_Itens.MostraMensagem_Erro("Não há itens com sugestões nesta lista!");
                }
                LM_gvItens_DataBind("Editar");
            }
            catch (Exception ex)
            {
                LM_Mensagem.MostraMensagem_Erro("Erro:" + ex);
            }
        }

        #endregion

        void LM_gvItens_SalvarGRID()
        {
            foreach (GridViewRow item in LM_gvItens.Rows)
            {
                if (item.RowType == DataControlRowType.DataRow)
                {
                    try
                    {
                        int idContador = Convert.ToInt32(LM_gvItens.DataKeys[item.RowIndex]["idContador"].ToString());
                        int index = Base_LM_Itens.FindIndex(x => x.idContador.Equals(idContador));

                        TextBox txtnOrdem = (TextBox)item.FindControl("LM_gvItens_txtnOrdem");
                        Base_LM_Itens[index].nOrdem = Convert.ToInt32(txtnOrdem.Text);

                        TextBox txtnQuantidade = (TextBox)item.FindControl("LM_gvItens_txtnQuantidade");
                        Base_LM_Itens[index].nQuantidade = Convert.ToDouble(txtnQuantidade.Text);

                        TextBox txtsAgrupamento = (TextBox)item.FindControl("LM_gvItens_txtsAgrupamento");
                        Base_LM_Itens[index].sDscCategoriaVendas = txtsAgrupamento.Text;

                        TextBox txtsObservacao = (TextBox)item.FindControl("LM_gvItens_txtsObservacao");
                        Base_LM_Itens[index].sObservacao = txtsObservacao.Text;

                        DropDownList ddlddlsEscopo = (DropDownList)item.FindControl("LM_gvItens_ddlsEscopo");
                        Base_LM_Itens[index].sEscopo = ddlddlsEscopo.SelectedValue;

                        if (LM_chkCompraEmObras.Checked)
                        {
                            TextBox txtnValorUnitario = (TextBox)item.FindControl("LM_gvItens_txtnValorUnitario");
                            Base_LM_Itens[index].nValorUnitario = Convert.ToDouble(txtnValorUnitario.Text);

                            TextBox txtnIPI = (TextBox)item.FindControl("LM_gvItens_txtnIPI");
                            Base_LM_Itens[index].nIPI = Convert.ToDecimal(txtnIPI.Text);

                            double nValor = Convert.ToDouble(txtnValorUnitario.Text) * Convert.ToDouble(txtnQuantidade.Text);
                            Base_LM_Itens[index].nTotal = nValor + (nValor * (Convert.ToDouble(txtnIPI.Text) / 100));
                        }
                    }
                    catch
                    {
                        LM_Mensagem_Itens.MostraMensagem_Erro("Erro na Coluna de Ordenação dos Itens");
                    }
                }
            }
        }


        #region | Popular Linha Composição

        protected void LM_gvItens_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Obtém o idProduto para a linha atual
                int idProduto = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "idProduto"));

                // Realiza as operações desejadas com base no idProduto
                AdicionarLinhasComposicao(idProduto, e.Row, 4);
            }
        }

        private void AdicionarLinhasComposicao(int idProduto, GridViewRow row, int nColuna)
        {

            decimal nQuantidade = 0;
            decimal.TryParse(DataBinder.Eval(row.DataItem, "nQuantidade").ToString(), out nQuantidade);

            // Encontra o PlaceHolder dentro da célula de detalhes
            PlaceHolder phDetalhesComposicao = (PlaceHolder)row.FindControl("phDetalhesComposicao");

            // Cria um novo GridView para Composição
            GridView gvDetalhesComposicao = new GridView();
            gvDetalhesComposicao.ID = "gvDetalhesComposicao_" + idProduto; // ID exclusivo Para cada Grid
            gvDetalhesComposicao.CssClass = "table table-striped table-bordered table-hover table-condensed";
            // Adiciona as colunas e outras configurações necessárias para o gvDetalhesComposicao

            //Adiciona o Evento RowDataBound (delegate)
            gvDetalhesComposicao.RowDataBound += GvDetalhesComposicao_RowDataBound;

            // Adiciona o GridView de detalhes ao PlaceHolder
            phDetalhesComposicao.Controls.Add(gvDetalhesComposicao);

            DataTable dtComposicao = ObterProdutosComposicao(idProduto, nQuantidade);
            if (dtComposicao.Rows.Count > 0)
            {
                gvDetalhesComposicao.DataSource = dtComposicao;
                gvDetalhesComposicao.DataBind();
            }
            else
            {
                // Se não houver detalhes oculta a GridView
                gvDetalhesComposicao.Visible = false;

                foreach (LinkButton lnk in row.Cells[nColuna].Controls.OfType<LinkButton>())
                {
                    if (lnk.ID == "lnkDetalhes")
                    {
                        lnk.Visible = false;
                    }
                }
            }
        }

        #endregion

        #region | Composição detalhes

        protected DataTable ObterProdutosComposicao(int idItem, decimal nQuantidade)
        {
            //Método para popular a Grid
            string sFuncao = "CONSULTAR_DETALHE";

            DataTable dtComposicao = new DataTable();
            string sSql = "sp_Manipula_tbl_Flow_WMS_Composicao";
            Dictionary<String, String> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", sFuncao },
                { "@idProduto", idItem.ToString() },
                { "@nQuantidade", Conversoes.Numerico(nQuantidade) }
            };

            dtComposicao = ExecutarDataTable(sSql, vParametros, false);
            return dtComposicao;
        }

        #endregion

        private void GvDetalhesComposicao_RowDataBound(object sender, GridViewRowEventArgs e) => EsconderColunas(e, 0);

        protected void LM_gvItens_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            LM_gvItens_SalvarGRID();
            int index = e.RowIndex;
            int idContador = Convert.ToInt32(e.Keys[0].ToString());
            Base_LM_Itens.Where(x => x.idContador == idContador).First().sFuncao = Base_LM_Itens.Where(x => x.idContador == idContador).First().sFuncao == "SALVAR_ITENS" ? "INEXISTENTE" : "EXCLUIR_ITENS";
            LM_gvItens_DataBind("Editar");
        }

        protected void LM_cmdExportarExcel_Click(object sender, EventArgs e)
        {
            //Panel pnExportar = new Panel();
            //pnExportar.Controls.Add(Div3);
            ////pnExportar.Controls.Add(LM_pnExportacaoExcel);
            //Exportacao.ExportarPainel_Excel(this, pnExportar, "LM");
            ////Exportacao.ExportarGRID_Excel(LM_gvItens, "LM");

            // --------------------------------------------
            // Gabriel Llanir - 22/08/2024
            try
            {
                ReportViewer rv = new ReportViewer();

                //rv.LocalReport.EnableExternalImages = true;
                rv.LocalReport.ReportPath = "App\\Reports\\Excel_Pedidos_LM.rdlc";

                DateTime dtEntrega = DateTime.Now;

                List<cls_Comercial_Tabelas> list_LM = new List<cls_Comercial_Tabelas>();

                Base_LM_Itens.Where(x => x.sFuncao != "EXCLUIR_ITENS" && x.sFuncao != "INEXISTENTE").OrderBy(x => x.sDscCategoriaVendas).ThenBy(y => y.nOrdem).ToList().ForEach(x =>
                {
                    Dictionary<string, string> vParametrosProduto = new Dictionary<string, string>()
                    {
                        { "@sFuncao", "CONSULTA_INFO_PRODUTOS_PARCEIRO" },
                        { "@idItem", x.idProduto.ToString() },
                        { "@idParceiro_Cliente", hddidCliente.Value },
                        { "@idUsuarioAtualizacao", Variaveis.idUsuario() },
                        { "@idUsuarioLogado", Variaveis.idUsuario() }
                    };

                    DataSet dsProdutos = ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParametrosProduto);

                    cls_Comercial_Tabelas item = new cls_Comercial_Tabelas();

                    item.SCodigo = x.sCodigoProduto;
                    item.SDscProduto = x.sDscProduto;
                    item.sDIFAL = x.sEscopo;
                    item.SUnidade = x.sUnidade;
                    item.NQuantidade = decimal.Parse(x.nQuantidade.ToString());
                    item.sOrdem = x.sObservacao;

                    item.sCodigoPai = DATASET(dsProdutos, "sCodigoCliente");
                    item.sDscProdutoPai = DATASET(dsProdutos, "sDscProdutoCliente");

                    list_LM.Add(item);
                });

                rv.LocalReport.DataSources.Add(new ReportDataSource("ds_LM", list_LM));
                rv.LocalReport.DataSources.Add(new ReportDataSource("ds_Vazio", new List<string>() { "vazio" }));

                Dictionary<string, string> vParametros = new Dictionary<string, string>()
                {
                    { "@sFuncao", "CONSULTAR_CLIENTE_x_PEDIDO_LM" },
                    { "@idParceiro", hddidCliente.Value },
                    { "@idPedido", hddidPedido.Value },
                };

                DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Clientes", vParametros);

                string razaoSocial = string.IsNullOrEmpty(DATASET(ds, "sRazaoSocial")) || string.IsNullOrWhiteSpace(DATASET(ds, "sRazaoSocial")) ? "N/A" : DATASET(ds, "sRazaoSocial");
                string cnpj = string.IsNullOrEmpty(DATASET(ds, "sCPF_CNPJ")) || string.IsNullOrWhiteSpace(DATASET(ds, "sCPF_CNPJ")) ? "N/A" : DATASET(ds, "sCPF_CNPJ");

                ReportParameter[] rp = new ReportParameter[7];

                rp[0] = new ReportParameter("Cliente_RazaoSocial", razaoSocial);
                rp[1] = new ReportParameter("Cliente_CNPJ_CPF", cnpj.Length > 0 ? cnpj.Replace(".", "").Replace("-", "").Replace("/", "").Length == 14 ? string.Format("CNPJ: {0}", Convert.ToInt64(cnpj.Replace(".", "").Replace("-", "").Replace("/", "")).ToString(@"00\.000\.000\/0000\-00")) : cnpj.Replace(".", "").Replace("-", "").Replace("/", "").Length == 11 ? string.Format("CPF: {0}", Convert.ToInt64(cnpj.Replace(".", "").Replace("-", "").Replace("/", "")).ToString(@"000\.000\.000\-00")) : cnpj : "N/A");
                rp[2] = new ReportParameter("Aparece_Dados_Cliente", cbDadosCliente.Checked ? "true" : "false");
                rp[3] = new ReportParameter("Titulo_LM", string.IsNullOrEmpty(LM_lblTitulo.Text) || string.IsNullOrWhiteSpace(LM_lblTitulo.Text) ? "Lista de Materiais" : LM_lblTitulo.Text);
                rp[4] = new ReportParameter("DataLM", string.IsNullOrEmpty(hdd_dtLM.Value) || string.IsNullOrWhiteSpace(hdd_dtLM.Value) ? txtdtPedido.Text : DateTime.Parse(hdd_dtLM.Value).ToString("dd/MM/yyyy"));
                rp[5] = new ReportParameter("Obs", string.IsNullOrEmpty(LM_txtsObservacao.Text) || string.IsNullOrWhiteSpace(LM_txtsObservacao.Text) ? "N/A" : LM_txtsObservacao.Text);
                rp[6] = new ReportParameter("nNumeroPedido", string.IsNullOrEmpty(DATASET(ds, "nNumeroPedido")) || string.IsNullOrWhiteSpace(DATASET(ds, "nNumeroPedido")) ? "000000" : DATASET(ds, "nNumeroPedido").PadLeft(6, '0'));

                rv.LocalReport.SetParameters(rp);
                rv.LocalReport.Refresh();

                byte[] bytes = rv.LocalReport.Render("Excel", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);
                string sNomeArquivo = "LM_" + CarimboDataHora() + ".xlsx";
                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);

                DownloadArquivo(Page, sNomeArquivo);

                LM_Mensagem.MostraMensagem_Sucesso("Relatório em Excel gerado com sucesso!", true);
            }
            catch (Exception ex)
            {
                LM_Mensagem.MostraMensagem_Erro("Houve um erro ao Gerar o Relatório em Excel dos Itens da LM!<br />" + ex.InnerException + "<br />" + ex.Message, true);
            }

            LM_Pesquisar(LM_ddlLista.SelectedValue, "CONSULTAR", false);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Mantem_Aba_LM", "$('#lm-tab').tab('show');", true);
        }

        public override void VerifyRenderingInServerForm(Control control) { }

        protected void LM_gvItens_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable m_DataTable = ConvertTo<cls_Pedidos_Itens>(Base_LM_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM").ToList());

            if (m_DataTable != null)
            {
                DataView m_DataView = new DataView(m_DataTable);
                m_DataView.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(e.SortDirection);
                LM_gvItens.DataSource = m_DataView;
                LM_gvItens.DataBind();
            }
        }

        protected void LM_cmdImportar_Click(object sender, EventArgs e)
        {
            LM_LimpaCampo();
            LM_divImportar.Visible = true;
            LM_divLista.Visible = false;
            LM_chkObras.Enabled = false;
            LM_chkEngenharia.Enabled = false;
            LM_chkRetornoObras.Enabled = false;
            LM_chkCompraEmObras.Enabled = false;
            LM_ddlLista.Visible = false;
            LM_cmdImportar.Visible = false;
            LM_cmdNova.Visible = false;
            LM_Importar_txtNumeroPedido.Text = "";
            LM_Importar_txtNumeroPedido.Focus();
        }

        protected void LM_cmdImportarLista_Click(object sender, EventArgs e) => LM_Pesquisar(LM_Importar_txtNumeroPedido.Text, "IMPORTAR_LM", true);

        protected void LM_cmdImportarVoltar_Click(object sender, EventArgs e) => LM_cmdVoltar_Click(sender, e);

        protected void LM_cmdGerarOPI_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "GERAR-OPI" },
                { "@idUsuarioInclusao", Variaveis.idUsuario() },
                { "@idLM", idLMRecuperado },
                { "@sDscOPI", $"OPI{DateTime.Now} - {LM_sDscOPI} " },
                { "@idPedido", hddidPedido.Value }
            };
            DataSet dsSalvar = ExecutarDataSet(sProcedure_OPI, vParametros);

            if (ValidarDataSet(dsSalvar, out string sErro) && DATASET(dsSalvar, "ret") == "0")
                LM_Mensagem.MostraMensagem_Sucesso(string.Format($"Gerado com sucesso!  </br><a href='WMS/OPI_Detalhe.aspx?id={DATASET(dsSalvar, "idOPI")}'>Clique aqui Visualizar a OPI Gerada!</a>"));
            else if (DATASET(dsSalvar, 0, "ret") == "1")
                LM_Mensagem.MostraMensagem_Aviso(DATASET(dsSalvar, "sMensagem"));
            else
                LM_Mensagem.MostraMensagem_Erro("BD: " + sErro.ToString());

            if (DATASET(dsSalvar, "idOPI") != "0")
                LM_cmdEditaLM.Visible = false;

            LM_Pesquisar(LM_ddlLista.SelectedValue, "CONSULTAR", false);
        }

        protected void LM_cmdEditarLM_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "js_Focus", "$('[id$=aba_Historico], [id$=aba_Tarefas], [id$=aba_Arquivos], [id$=aba_Acoes], [id$=aba_Dados], [id$=aba_depara]').hide();", true);

            if (LM_chkRetornoObras.Checked)
            {
                chkObras.Visible = false;
                chkEngenharia.Visible = false;
                chkCompraEmObras.Visible = false;
                LM_chkRetornoObras.Enabled = false;
            }

            if (LM_chkObras.Checked)
            {
                chkEngenharia.Visible = false;
                chkRetornoObras.Visible = false;
                chkCompraEmObras.Visible = false;
                LM_chkObras.Enabled = false;
            }

            if (LM_chkEngenharia.Checked)
            {
                chkObras.Visible = false;
                chkRetornoObras.Visible = false;
                chkCompraEmObras.Visible = false;
                LM_chkEngenharia.Enabled = false;
            }

            if (LM_chkCompraEmObras.Checked)
            {
                chkObras.Visible = false;
                chkRetornoObras.Visible = false;
                chkEngenharia.Visible = false;
                LM_chkCompraEmObras.Enabled = false;
            }

            LM_ddlLista.Attributes.Add("disabled", "disabled");
            LM_cmdExportarExcel.Visible = false;
            LM_cmdGerarOPI.Visible = false;
            LM_cmdNova.Visible = false;
            LM_cmdSalvaLM.Visible = true;
            LM_DIV_Arquivos.Visible = false;
            LM_DIV_SelecaoItens.Visible = true;
            LM_txtsObservacao.ReadOnly = false;
            LM_txtsDscLM.ReadOnly = false;

            hddidLM.Value = LM_ddlLista.SelectedValue;
            Base_LM_Itens.Clear();
            LM_Pesquisar(hddidLM.Value, "", false);

            LM_gvItens_ConfigurarColunas("Editar");
            LM_gvItens_DataBind("Editar");



            foreach (var item in Base_LM_Itens)
            {
                item.sFuncao = "ALTERAR_ITENS";
            }
            LM_cmdEditaLM.Visible = false;
            RegistraScript("");
        }

        protected void LM_cmdSalvaLM_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "js_Focus", "$('[id$=aba_Historico], [id$=aba_Tarefas], [id$=aba_Arquivos], [id$=aba_Acoes], [id$=aba_Dados], [id$=aba_depara]').show();", true);
            LM_Gravar();
            LM_ddlLista.Attributes.Remove("disabled");
            LM_frmArquivos.Visible = true;
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_MantemAbaAtiva_LM", " $('#lm-tab').tab('show');", true);
        }

        #endregion

        #region | Aba Envios

        bool GravarPedido_Envios(string idPedido)
        {
            bool bRetorno = false;

            if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
            {
                try
                {
                    Importacao_Envios_Insert("AtualizarClasse", 0, 0, 0, "", 0, "", "", 0, "", 0, "");

                    foreach (var linha in Base_Pedidos_Envios)
                    {
                        DataSet dsEnvios;
                        Dictionary<String, String> vParametrosSalvarEnvios = new Dictionary<string, string>
                            {
                                {"@idRegistro",                     linha.idRegistro.ToString()},
                                {"@sFuncao",                        linha.sFuncao.ToString()},
                                {"@idPedido",                       idPedido },
                                {"@idTipoEnvio",                    linha.idTipoEnvio.ToString()},
                                {"@Importacao_idParceiroEnvio",     linha.idParceiro.ToString() },
                                {"@Importacao_sDscParceiro",        linha.sDscParceiro.ToString() },
                                {"@sCodigo",                        linha.sCodigo },
                                {"@sNomeArquivo",                   linha.sNomeArquivo == null ? "" : linha.sNomeArquivo},
                                {"@Importacao_idMetodoEnvio",       linha.idMetodoEnvio.ToString()},
                                {"@Importacao_idMeioEnvio",         linha.idMeioEnvio.ToString()},
                                {"@idUsuarioInclusao",              Variaveis.idUsuario()},
                            };
                        if (linha.sFuncao != "SALVAR ENVIOS" && linha.sFuncao != "ALTERAR ENVIOS" && linha.sFuncao != "EXCLUIR ENVIOS")
                            continue;

                        dsEnvios = ExecutarDataSet(sProcedure, vParametrosSalvarEnvios);
                    }
                    bRetorno = true;
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao Salvar Envios: " + ex.Message);
                }
            }
            else
            {
                bRetorno = true;
            }

            return bRetorno;
        }

        void Importacao_Envios_LimpaCampos()
        {
            ddlImportacao_TipoEnvio.SelectedValue = "0";
            ddlImportacao_TipoEnvioParceiro.SelectedValue = "0";
            DIV_Importacao_Envio_Parceiro.Visible = false;
            DIV_Importacao_Envio_Transportadora.Visible = false;
            txtImportacao_TipoEnvioCodigo.Text = "";
            ddlImportacao_MeioEnvio.SelectedValue = "0";
            ddlImportacao_MetodoEnvio.SelectedValue = "0";
        }

        void Importacao_Envios_Insert(string sFuncao, int idRegistro, int idPedido, int idTipoEnvio, string sDscTipoEnvio, int idParceiro, string sDscParceiro, string sCodigo, int idMetodoEnvio, string sDscMetodoEnvio, int idMeioEnvio, string sDscMeioEnvio)
        {
            try
            {
                if (sFuncao == "AtualizarClasse")
                {
                    foreach (GridViewRow envio in gv_Envios.Rows)
                    {
                        try
                        {
                            cls_Pedidos_Envios objEnvios = Base_Pedidos_Envios.FirstOrDefault(e => e.idRegistro == Int32.Parse(gv_Envios.DataKeys[envio.RowIndex]["idRegistro"].ToString()));
                        }
                        catch
                        {

                        }
                    }
                }
                else
                {
                    FrameWork.cls_Pedidos_Envios objEnvios = new FrameWork.cls_Pedidos_Envios();
                    objEnvios.idPedido = idPedido;

                    try
                    {
                        objEnvios.idRegistro = sFuncao == "SALVAR ENVIOS" ? Base_Pedidos_Envios.OrderBy(x => x.idRegistro).Last().idRegistro + 1 : idRegistro;
                    }
                    catch
                    {
                        objEnvios.idRegistro = 1;
                    }

                    objEnvios.sFuncao = sFuncao;
                    objEnvios.idTipoEnvio = idTipoEnvio;
                    objEnvios.sDscTipoEnvio = sDscTipoEnvio;
                    objEnvios.idParceiro = idParceiro;
                    objEnvios.sDscParceiro = sDscParceiro;
                    objEnvios.sCodigo = sCodigo;
                    objEnvios.idMetodoEnvio = idMetodoEnvio;
                    objEnvios.sDscMetodoEnvio = sDscMetodoEnvio;
                    objEnvios.idMeioEnvio = idMeioEnvio;
                    objEnvios.sDscMeioEnvio = sDscMeioEnvio;
                    objEnvios.idContador = Base_Pedidos_Envios.Where(x => x.sFuncao != "EXCLUIR ENVIOS" && x.sFuncao != "INEXISTENTE").ToList().Count + 1;
                    Base_Pedidos_Envios.Add(objEnvios);
                }
                gv_Envios.Columns[0].Visible = false;
            }
            catch (Exception ex)
            {
                msgImportacao_Envio.MostraMensagem_Erro("Erro ao Incluir Envios: " + ex.Message);
            }
        }

        protected void cmdIncluirEnvios_Click(object sender, EventArgs e)
        {
            if (Importacao_Envios_Validar())
            {
                if (idRegistro == 0)
                {
                    int TipoEnvio_idParceiro = 0;
                    string TipoEnvio_sParceiro = "";
                    int.TryParse(ddlImportacao_TipoEnvioParceiro.SelectedValue, out TipoEnvio_idParceiro);

                    if (TipoEnvio_idParceiro != 0)
                    {
                        TipoEnvio_sParceiro = ddlImportacao_TipoEnvioParceiro.SelectedItem.ToString();
                    }
                    else
                    {
                        TipoEnvio_sParceiro = txtImportacao_Envio_Transportadora.Text;
                    }

                    Importacao_Envios_Insert("SALVAR ENVIOS"
                        , 0
                        , idPedido()
                        , Convert.ToInt32(ddlImportacao_TipoEnvio.SelectedValue)
                        , ddlImportacao_TipoEnvio.SelectedItem.ToString()
                        , TipoEnvio_idParceiro
                        , TipoEnvio_sParceiro
                        , txtImportacao_TipoEnvioCodigo.Text
                        , Convert.ToInt32(ddlImportacao_MetodoEnvio.SelectedValue)
                        , ddlImportacao_MetodoEnvio.SelectedItem.ToString()
                        , Convert.ToInt32(ddlImportacao_MeioEnvio.SelectedValue)
                        , ddlImportacao_MeioEnvio.SelectedItem.ToString()
                        );

                    Importacao_Envios_Insert("AtualizarClasse", 0, 0, 0, "", 0, "", "", 0, "", 0, "");
                }
                else
                {
                    var Envio = Base_Pedidos_Envios[Base_Pedidos_Envios.FindIndex(x => x.idContador.Equals(idRegistro))];

                    Envio.idTipoEnvio = Convert.ToInt32(ddlImportacao_TipoEnvio.SelectedValue);
                    Envio.sDscTipoEnvio = ddlImportacao_TipoEnvio.SelectedItem.ToString();
                    if (Envio.idTipoEnvio == 1)
                    {
                        Envio.idParceiro = Convert.ToInt32(ddlImportacao_TipoEnvioParceiro.SelectedValue);
                        Envio.sDscParceiro = ddlImportacao_TipoEnvioParceiro.SelectedItem.ToString();
                    }
                    else
                    {
                        Envio.sDscParceiro = txtImportacao_Envio_Transportadora.Text;
                    }
                    Envio.sCodigo = txtImportacao_TipoEnvioCodigo.Text;
                    Envio.idMetodoEnvio = Convert.ToInt32(ddlImportacao_MetodoEnvio.SelectedValue);
                    Envio.sDscMetodoEnvio = ddlImportacao_MetodoEnvio.SelectedItem.ToString();
                    Envio.idMeioEnvio = Convert.ToInt32(ddlImportacao_MeioEnvio.SelectedValue);
                    Envio.sDscMeioEnvio = ddlImportacao_MeioEnvio.SelectedItem.ToString();
                    Envio.sFuncao = "ALTERAR ENVIOS";
                    idRegistro = 0;
                }

                gvImportacao_Envios_Databound();
                Importacao_Envios_LimpaCampos();
            }
        }

        bool Importacao_Envios_Validar()
        {
            string sMensagemErro = "";
            if (ddlImportacao_TipoEnvio.SelectedValue == "0")
            {
                sMensagemErro += "Selecione um tipo de Envio <br/>";
            }
            else if (ddlImportacao_TipoEnvio.SelectedValue == "1")
            {
                if (ddlImportacao_TipoEnvioParceiro.SelectedValue == "0")
                {
                    sMensagemErro += "Selecione um Courier <br/>";
                }
            }

            if (ddlImportacao_TipoEnvio.SelectedValue != "1")
            {
                if (txtImportacao_Envio_Transportadora.Text == "")
                {
                    sMensagemErro += "Informe uma Transportadora válida <br/>";
                }
            }


            if (Validacoes.ValidarTexto(txtImportacao_TipoEnvioCodigo))
            {
                sMensagemErro += "Informe um Rastreamento válido <br/>";
            }

            if (ddlImportacao_MetodoEnvio.SelectedValue == "0")
            {
                sMensagemErro += "Selecione o Método de Envio <br/>";
            }

            if (ddlImportacao_MeioEnvio.SelectedValue == "0")
            {
                sMensagemErro += "Selecione o Meio de Envio <br/>";
            }

            if (sMensagemErro != "")
            {
                MensagemPagina1.MostraMensagem_Erro(sMensagemErro);
            }

            return sMensagemErro != "" ? false : true;
        }

        void gvImportacao_Envios_Databound()
        {
            gv_Envios.DataSource = Base_Pedidos_Envios.Where(c => c.sFuncao.ToString() != "EXCLUIR ENVIOS" && c.sFuncao.ToString() != "INEXISTENTE").OrderBy(x => x.idContador).ToList();
            gv_Envios.DataBind();
        }

        void Importacao_Envios_PopularCombos()
        {
            DIV_Envios.Visible = false;
            DIV_Importacao_Envio_Parceiro.Visible = false;
            DIV_Importacao_Envio_Transportadora.Visible = false;
            if (ddlidModal.SelectedValue != "0")
            {
                Popula_Combo(ddlImportacao_TipoEnvio, "sp_Select 'Flow_COMEX_TipoEnvio', '" + ddlidModal.SelectedValue + "'", "idTipoEnvio", "sDscTipoEnvio", false, "Selecione o Tipo de Envio", "0");
                Popula_Combo(ddlImportacao_MetodoEnvio, "sp_Select 'tbl_Flow_Pedidos_Modal_MetodoEnvio', '" + ddlidModal.SelectedValue + "'", "idMetodoEnvio", "sDscMetodoEnvio", false, "Selecione o Método de Envio", "0");
                Popula_Combo(ddlImportacao_MeioEnvio, "sp_Select 'tbl_Flow_Pedidos_Modal_MeioEnvio', '" + ddlidModal.SelectedValue + "'", "idMeioEnvio", "sDscMeioEnvio", false, "Selecione o Meio de Envio", "0");
                DIV_Envios.Visible = true;
            }
            UpdEnvios.Update();
        }

        void gvImportacao_Envios_Popular(DataSet ds, string sMetodoChamada)
        {
            DIV_Envios_Selecao.Visible = false;
            this.gv_Envios.Columns[7].Visible = false;

            foreach (DataRow row in ds.Tables[nTabela_Envios].Rows)
            {
                Importacao_Envios_Insert("CONSULTAR ENVIOS"
                                            , Convert.ToInt32(row["idRegistro"].ToString())
                                            , idPedido()
                                            , Convert.ToInt32(row["idTipoEnvio"])
                                            , row["sDscTipoEnvio"].ToString()
                                            , Convert.ToInt32(row["idParceiro"].ToString())
                                            , row["sDscParceiro"].ToString()
                                            , row["sCodigo"].ToString()
                                            , Convert.ToInt32(row["idMetodoEnvio"].ToString())
                                            , row["sDscMetodoEnvio"].ToString()
                                            , Convert.ToInt32(row["idMeioEnvio"].ToString())
                                            , row["sDscMeioEnvio"].ToString()
                   );
            }

            gvImportacao_Envios_Databound();
            DIV_Envios.Visible = true;

            if (sMetodoChamada == "Editar")
            {
                TextInvoiceNumber.ReadOnly = false;
                txtnOrderNumber.ReadOnly = false;
                txtsZonaEnvio.ReadOnly = false;
                Importacao_Envios_PopularCombos();
                DIV_Envios_Selecao.Visible = true;
                this.gv_Envios.Columns[7].Visible = true;
            }
        }

        protected void gvImportacao_Envios_RowDataBound(object sender, GridViewRowEventArgs e)
        {
        }

        protected void gvImportacao_Envios_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            int idRegistro = Convert.ToInt32(gv_Envios.DataKeys[e.RowIndex]["idRegistro"].ToString());
            string sFuncao = Base_Pedidos_Envios[Base_Pedidos_Envios.FindIndex(x => x.idRegistro.Equals(idRegistro))].sFuncao;

            if (sFuncao == "SALVAR ENVIOS")
                Base_Pedidos_Envios[Base_Pedidos_Envios.FindIndex(x => x.idRegistro.Equals(idRegistro))].sFuncao = "INEXISTENTE";
            else
                Base_Pedidos_Envios[Base_Pedidos_Envios.FindIndex(x => x.idRegistro.Equals(idRegistro))].sFuncao = "EXCLUIR ENVIOS";

            foreach (var envio in Base_Pedidos_Envios.Where(x => x.idContador > Convert.ToInt32(e.Keys[0].ToString())))
            {
                envio.idContador -= 1;

                if (envio.sFuncao != "SALVAR ENVIOS" && envio.sFuncao != "EXCLUIR ENVIOS" && envio.sFuncao != "INEXISTENTE")
                    envio.sFuncao = "ALTERAR ENVIOS";
            }

            hddsAlteracaoItens.Value = "S";
            gvImportacao_Envios_Databound();
        }

        protected void gvImportacao_Envios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = 0;
            if (e.CommandName == "Alterar")
            {
                idLinha = int.Parse(e.CommandArgument.ToString());
                var Envio = Base_Pedidos_Envios[Base_Pedidos_Envios.FindIndex(x => x.idContador.Equals(idLinha))];

                ddlImportacao_TipoEnvio.SelectedValue = Envio.idTipoEnvio.ToString();
                if (Envio.idTipoEnvio == 1)
                {
                    DIV_Importacao_Envio_Parceiro.Visible = true;
                    Popula_Combo(ddlImportacao_TipoEnvioParceiro, "sp_Select 'Flow_COMEX_Courier'", "idParceiro", "sRazaoSocial", false, "Selecione o Courier", "0");
                    ddlImportacao_TipoEnvioParceiro.SelectedValue = Envio.idParceiro.ToString();
                    DIV_Importacao_Envio_Transportadora.Visible = false;
                }
                else
                {
                    DIV_Importacao_Envio_Transportadora.Visible = true;
                    txtImportacao_Envio_Transportadora.Text = Envio.sDscParceiro;
                    DIV_Importacao_Envio_Parceiro.Visible = false;
                }
                txtImportacao_TipoEnvioCodigo.Text = Envio.sCodigo;
                ddlImportacao_MetodoEnvio.SelectedValue = Envio.idMetodoEnvio.ToString();
                ddlImportacao_MeioEnvio.SelectedValue = Envio.idMeioEnvio.ToString();
                idRegistro = idLinha;
            }
            UpdEnvios.Update();
        }

        protected void ddlImportacao_TipoEnvios_SelectedIndexChanged(object sender, EventArgs e)
        {
            DIV_Importacao_Envio_Parceiro.Visible = false;
            DIV_Importacao_Envio_Transportadora.Visible = false;
            if (ddlImportacao_TipoEnvio.SelectedValue == "1")
            {
                DIV_Importacao_Envio_Parceiro.Visible = true;
                Popula_Combo(ddlImportacao_TipoEnvioParceiro, "sp_Select 'Flow_COMEX_Courier'", "idParceiro", "sRazaoSocial", false, "Selecione o Courier", "0");
            }
            else
            {
                DIV_Importacao_Envio_Transportadora.Visible = true;
                txtImportacao_Envio_Transportadora.Text = "";
            }
        }

        #endregion

        #region | Aba Resultado

        protected void gv_resultado_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                var grid = (GridView)sender;
                if (grid.FooterRow != null)
                {
                    decimal EnvioTotal = 0;
                    decimal EnvioReal = 0;
                    decimal resultado = 0;

                    foreach (GridViewRow row in grid.Rows)
                    {
                        try
                        {
                            EnvioTotal += decimal.Parse((row.FindControl("nValorTotal") as Label).Text);
                            EnvioReal += decimal.Parse((row.FindControl("nValorRealTotal") as Label).Text);
                            resultado += decimal.Parse((row.FindControl("nResultado") as Label).Text);
                        }
                        catch
                        {

                        }
                    }

                    e.Row.Cells[9].Text = HttpUtility.HtmlDecode(string.Format("<b>{0:N4} {1}</b>", EnvioTotal, sMoeda));
                    e.Row.Cells[12].Text = HttpUtility.HtmlDecode(string.Format("<b>{0:N4} {1}</b>", EnvioReal, sMoeda));
                    e.Row.Cells[13].Text = HttpUtility.HtmlDecode(string.Format("<b>{0:N4} {1}</b>", resultado, sMoeda));
                    txtCambio.Text = EnvioTotal.ToString("F2");
                    //Vittorio - 06/06/2024
                    decimal Pago = txtPago.Text != "" ? decimal.Parse(txtPago.Text) : 0;
                    decimal Pagar = 0;
                    Pagar = Pago - EnvioTotal;
                    txtPagar.Text = Pagar.ToString("F2");
                }
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                (e.Row.FindControl("sMoedaOrigem17") as HtmlGenericControl).InnerText = hddMoeda.Value;
                (e.Row.FindControl("sMoedaOrigem18") as HtmlGenericControl).InnerText = hddMoeda.Value;
                (e.Row.FindControl("sMoedaOrigem19") as HtmlGenericControl).InnerText = hddMoeda.Value;
                (e.Row.FindControl("sMoedaOrigem20") as HtmlGenericControl).InnerText = hddMoeda.Value;
                (e.Row.FindControl("sMoedaOrigem21") as HtmlGenericControl).InnerText = hddMoeda.Value;
                (e.Row.FindControl("sMoedaOrigem22") as HtmlGenericControl).InnerText = hddMoeda.Value;
                (e.Row.FindControl("sMoedaOrigem23") as HtmlGenericControl).InnerText = hddMoeda.Value;
            }
        }

        protected void Itens_Resultado_txtnValorUnitario_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            GridViewRow row = txt.NamingContainer as GridViewRow;
            string codigo = row.Cells[2].Text;
            var item = Base_Pedidos_Itens.Where(i => i.sCodigoProduto == codigo || i.EnvioResultado).FirstOrDefault();

            if (item != null)
            {
                if (txt.ID == "Itens_Resultado_txtnValorUnitario" || txt.ID == "Itens_ValoresDatas_txtnValorUnitario")
                {
                    if (txt.Text == "")
                    {
                        txt.Text = "0,0000";
                    }
                    item.nValorUnitario = double.Parse(txt.Text);
                }

                if (txt.ID == "txtnValorReal")
                {
                    if (txt.Text == "")
                    {
                        txt.Text = "0,0000";
                    }
                    item.nValorReal = double.Parse(txt.Text);
                }

                item.nValorTotal = item.nQuantidade * item.nValorUnitario;
                item.nResultado = Convert.ToDecimal(item.nValorTotal) - item.nValorRealTotal;
                if (item.sFuncao != "Inserir_item_envio")
                    item.sFuncao = "ALTERAR_ITEM";

                foreach (GridViewRow itens in gv_resultado.Rows)
                {
                    try
                    {
                        if (Base_Pedidos_Itens.Where(i => i.sCodigoProduto == itens.Cells[2].Text).First() is cls_Pedidos_Itens resultado)
                        {
                            if (resultado.sFuncao != "Inserir_item_envio")
                            {
                                resultado.sFuncao = "INCLUIR ITEM ALTERACAO";
                                resultado.nValorReal = double.Parse((itens.FindControl("txtnValorReal") as TextBox).Text);
                                resultado.nValorRealTotal = Convert.ToDecimal(resultado.nValorReal) * Convert.ToDecimal(resultado.nQuantidade);
                                resultado.nResultado = resultado.nValorRealTotal - Convert.ToDecimal(resultado.nValorTotal);
                            }
                            else
                            {
                                resultado.sFuncao = "Inserir_item_envio";
                                resultado.nValorReal = double.Parse((itens.FindControl("txtnValorReal") as TextBox).Text);
                                resultado.nValorRealTotal = Convert.ToDecimal(resultado.nValorReal) * Convert.ToDecimal(resultado.nQuantidade);
                                resultado.nResultado = resultado.nValorRealTotal - Convert.ToDecimal(resultado.nValorTotal);
                            }
                        }
                    }
                    catch
                    {
                        if (Base_Pedidos_Itens.Where(i => i.EnvioResultado).First() is cls_Pedidos_Itens resultado)
                        {
                            resultado.sFuncao = "Inserir_item_envio";
                            resultado.nValorReal = double.Parse((itens.FindControl("txtnValorReal") as TextBox).Text);
                            resultado.nValorRealTotal = Convert.ToDecimal(resultado.nValorReal) * Convert.ToDecimal(resultado.nQuantidade);
                            resultado.nResultado = resultado.nValorRealTotal - Convert.ToDecimal(resultado.nValorTotal);
                        }
                    }
                }

                Importacao_ValoresDatas_Insert();
                DataBind_dtgItens("Editar");
            }
            decimal ocean = decimal.Parse(txtnOcean_Air.Text);
            decimal Insurance = decimal.Parse(txtnInsurance.Text);
            decimal Other = decimal.Parse(txtnOther_Charges.Text);
            decimal valor = ocean + Insurance + Other;
            txtnTotalEnvio.Text = valor.ToString();
            txtnVlrServicos.Text = valor.ToString();
        }

        protected void lnkAplicaValorReal_Click(object sender, EventArgs e)
        {
            var celula = (sender as LinkButton).Parent;
            var linha = (sender as LinkButton).NamingContainer;

            if (celula != null)
            {
                decimal precoUni = decimal.Parse((celula.FindControl("Itens_Resultado_txtnValorUnitario") as TextBox).Text);
                decimal Quantidade = decimal.Parse((celula.FindControl("txtnQuantidade") as TextBox).Text);

                decimal totalunitario = decimal.Parse(((linha as GridViewRow).FindControl("nValorTotal") as Label).Text);

                if (linha != null)
                {
                    ((linha as GridViewRow).FindControl("txtnValorReal") as TextBox).Text = precoUni.ToString("N4");
                    decimal TotalReal = precoUni * Quantidade;
                    ((linha as GridViewRow).FindControl("nValorRealTotal") as Label).Text = TotalReal.ToString("N4");

                    decimal resultado = -totalunitario + TotalReal;

                    ((linha as GridViewRow).FindControl("nResultado") as Label).Text = resultado.ToString("N4");

                    if (Base_Pedidos_Itens.Where(i => i.sCodigoProduto == (linha as GridViewRow).Cells[2].Text).First() is cls_Pedidos_Itens item)
                    {
                        item.nValorReal = Convert.ToDouble(precoUni.ToString("N4"));
                        item.nValorRealTotal = Convert.ToDecimal(TotalReal.ToString("N4"));
                        item.nResultado = Convert.ToDecimal(resultado.ToString("N4"));
                        if (item.sFuncao != "Inserir_item_envio")
                            item.sFuncao = "ALTERAR_ITEM";
                    }
                    gv_resultado.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM").OrderBy(x => x.nOrdem).ToList();
                    gv_resultado.DataBind();
                }
            }
        }

        protected void TodoslnkAplicaValorReal_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gv_resultado.Rows)
            {
                decimal valor = decimal.Parse((row.FindControl("Itens_Resultado_txtnValorUnitario") as TextBox).Text);
                (row.FindControl("txtnValorReal") as TextBox).Text = valor.ToString("N4");
                decimal Quantidade = decimal.Parse((row.FindControl("txtnQuantidade") as TextBox).Text);
                decimal TotalReal = valor * Quantidade;
                (row.FindControl("nValorRealTotal") as Label).Text = TotalReal.ToString("N4");
                decimal ValorTotal = decimal.Parse((row.FindControl("nValorTotal") as Label).Text);
                decimal resultado = -ValorTotal + TotalReal;

                if (Base_Pedidos_Itens.Where(i => i.sCodigoProduto == row.Cells[2].Text).First() is cls_Pedidos_Itens item)
                {
                    item.nValorReal = Convert.ToDouble(valor.ToString("N4"));
                    item.nValorRealTotal = Convert.ToDecimal(TotalReal.ToString("N4"));
                    item.nResultado = Convert.ToDecimal(resultado.ToString("N4"));
                    if (item.sFuncao != "Inserir_item_envio")
                        item.sFuncao = "ALTERAR_ITEM";
                }
                gv_resultado.DataSource = Base_Pedidos_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM").OrderBy(x => x.nOrdem).ToList();
                gv_resultado.DataBind();
            }
        }

        void Popular_dtgPagamento(DataSet dsPesquisa)
        {
            bs_Pagamento.Clear();
            foreach (DataRow row in dsPesquisa.Tables[5].Rows)
            {
                FrameWork.cls_Pagamento objItem = new FrameWork.cls_Pagamento();

                objItem.idRegistroPagamento = Convert.ToInt32(row["idRegistroPagamento"].ToString());
                objItem.idLinha = bs_Pagamento.Count() + 1;
                objItem.sFuncao = "SEM ALTERAÇÃO";
                objItem.nValorPagamento_Info_Pag = ConverterStringDecimal(row["nValorPagamento_Info_Pag"].ToString());
                objItem.dtPagamento_Info_Pag = row["dtPagamento_Info_Pag"].ToString();
                objItem.sCorretora = row["sCorretora"].ToString();
                objItem.dtAtualizacao = row["dtAtualizacao"].ToString();
                objItem.sContratoCambio = row["sContratoCambio"].ToString();
                objItem.sDscConta_Info_Pag = row["sDscConta_Info_Pag"].ToString();
                objItem.sDscFormaPagamento_Info_Pag = row["sDscFormaPagamento_Info_Pag"].ToString();
                objItem.nNumeroParcela_Info_Pag = Convert.ToInt32(row["nNumeroParcela_Info_Pag"].ToString());
                objItem.idArquivo = Convert.ToInt32(row["idArquivo"].ToString());
                objItem.sNomeArquivo = row["sNomeArquivo"].ToString();
                bs_Pagamento.Add(objItem);
            }
            dtgPagamento_DataBind();
            LimpaCampos_Pagamento();
        }

        protected void cmdPagamento_Incluir_Click(object sender, EventArgs e)
        {
            string sMensagem = "";

            if (ValidarDados_Pagamento(ref sMensagem))
            {
                FrameWork.cls_Pagamento objItem = new FrameWork.cls_Pagamento();

                objItem.idLinha = bs_Pagamento.Count() + 1;
                objItem.sFuncao = "INSERIR_Cambio";
                objItem.nValorPagamento_Info_Pag = ConverterStringDecimal(txtnValorPagamento_Info_Pag.Text);
                objItem.sContratoCambio = txtsContratoCambio.Text;
                objItem.sCorretora = txtsCorretora.Text;
                objItem.dtPagamento_Info_Pag = DateTime.Parse(txtdtPagamento_Info_Pag.Text).ToString("dd/MM/yyyy");

                var Parcela = bs_Pagamento.Where(p => p.nNumeroParcela_Info_Pag >= 0).LastOrDefault();
                int proximaParcela = Parcela == null ? 1 : Parcela.nNumeroParcela_Info_Pag + 1;
                objItem.nNumeroParcela_Info_Pag = proximaParcela;
                objItem.sNomeArquivo = "";
                bs_Pagamento.Add(objItem);
            }
            dtgPagamento_DataBind();
            LimpaCampos_Pagamento();
            RegistraScript("");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtdtPagamento_Info_Pag]').focus();", true);
        }

        private bool ValidarDados_Pagamento(ref string sMensagem)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (txtnValorPagamento_Info_Pag.Text.Length < 3)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Inválido";
            }

            if (txtsContratoCambio.Text == "")
            {
                if (sMensagemErro != "")
                {
                    sMensagemErro = sMensagemErro + "</br>";
                }
                sMensagemErro += "Contrato de Cambio Inválido!";
            }

            if (txtsCorretora.Text == "")
            {
                if (sMensagemErro != "")
                {
                    sMensagemErro = sMensagemErro + "</br>";
                }
                sMensagemErro += "Corretora Inválida!";
            }

            if (Validacoes.ValidarTexto(txtdtPagamento_Info_Pag))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe a data do Pagamento" + "</br>";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemLancamento.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        protected void BtnEdicaoPagamento_Click(object sender, EventArgs e)
        {
            int idLinha = Convert.ToInt32(hddPagamento_idLinha.Value);
            int index = bs_Pagamento.FindIndex(x => x.idLinha.Equals(idLinha));
            string sMensagem = "";
            decimal nValor = 0;
            decimal.TryParse(txtnValorPagamento_Info_Pag.Text.Replace(".", ""), out nValor);

            bs_Pagamento[index].nValorPagamento_Info_Pag = nValor;
            bs_Pagamento[index].sContratoCambio = txtsContratoCambio.Text;
            bs_Pagamento[index].sCorretora = txtsCorretora.Text;
            bs_Pagamento[index].dtPagamento_Info_Pag = DateTime.Parse(txtdtPagamento_Info_Pag.Text).ToString("dd/MM/yyyy");
            bs_Pagamento[index].sFuncao = "INSERIR_Cambio";
            BtnEdicaoPagamento.Visible = false;

            dtgPagamento_DataBind();
            LimpaCampos_Pagamento();
            RegistraScript("");
        }

        bool Salvar_Pagamento(string idPedido)
        {
            bool bRetorno = true;
            try
            {
                foreach (var Pagamento_Linha in bs_Pagamento)
                {
                    int idArquivo = Pagamento_Linha.idArquivo;
                    if (Pagamento_Linha.sNomeArquivo != "" && Pagamento_Linha.idArquivo == 0)
                    {
                        TT_Flow.FrameWork.cls_Arquivos Arquivo = new FrameWork.cls_Arquivos();
                        Arquivo.idTipoArquivo = 9999;
                        Arquivo.idObjeto = Pagamento_Linha.idRegistroPagamento;
                        Arquivo.sNomeArquivo = Pagamento_Linha.sNomeArquivo;
                        Arquivo.sDscArquivo = Pagamento_Linha.sObservacaoArquivo;
                        Arquivo.sObservacao = "";
                        Arquivo.idUsuario = Convert.ToInt32(Variaveis.idUsuario());
                        Arquivo.vbArquivo = Pagamento_Linha.objArquivo;
                        Arquivo.dtExpiracaoDoc = "";
                        Pagamento_Linha.idArquivo = Convert.ToInt32(DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));

                        if (Pagamento_Linha.sFuncao == "SEM ALTERAÇÃO")
                        {
                            Pagamento_Linha.sFuncao = "INSERIR_Cambio";
                        }
                    }

                    if (Pagamento_Linha.sFuncao != "SEM ALTERAÇÃO")
                    {
                        Dictionary<String, String> vParametroItensPagamento = new Dictionary<string, string>
                        {
                            ["@idRegistro"] = Pagamento_Linha.idRegistroPagamento.ToString(),
                            ["@sFuncao"] = Pagamento_Linha.sFuncao,
                            ["@idPedido"] = idPedido,
                            ["@nValorPagamento_Info_Pag"] = Pagamento_Linha.nValorPagamento_Info_Pag.ToString().Replace(",", "."),
                            ["@sContratoCambio"] = Pagamento_Linha.sContratoCambio.ToString(),
                            ["@dtPagamento_Info_Pag"] = Pagamento_Linha.dtPagamento_Info_Pag.ToString(),
                            ["@sCorretora"] = Pagamento_Linha.sCorretora.ToString(),
                            ["@nNumeroParcela_Info_Pag"] = Pagamento_Linha.nNumeroParcela_Info_Pag.ToString(),
                            ["@idArquivo"] = Pagamento_Linha.idArquivo.ToString(),
                            ["@idUsuarioAtualizacao"] = Variaveis.idUsuario()
                        };
                        ExecutarDataSet(sProcedure, vParametroItensPagamento);
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
                bRetorno = false;
            }
            return bRetorno;
        }

        void dtgPagamento_DataBind()
        {
            dtgPagamento.DataSource = bs_Pagamento.Where(c => c.sFuncao.ToString() != "EXCLUIR_Pagamento").OrderBy(x => x.nNumeroParcela_Info_Pag);
            dtgPagamento.DataBind();
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

        void LimpaCampos_Pagamento()
        {
            txtnValorPagamento_Info_Pag.Text = "";
            txtsContratoCambio.Text = "";
            txtdtPagamento_Info_Pag.Text = "";
            txtsCorretora.Text = "";
            hddPagamento_idLinha.Value = "";
            cmdPagamento_Incluir.Visible = true;
        }

        protected void dtgPagamento_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(dtgPagamento.Rows[e.RowIndex].Cells[0].Text);
            var nParcela = bs_Pagamento[bs_Pagamento.FindIndex(x => x.idLinha.Equals(idLinha))].nNumeroParcela_Info_Pag;
            bs_Pagamento[bs_Pagamento.FindIndex(x => x.idLinha.Equals(idLinha))].sFuncao = "EXCLUIR_Pagamento";
            bs_Pagamento[bs_Pagamento.FindIndex(x => x.idLinha.Equals(idLinha))].nNumeroParcela_Info_Pag = -1;
            bs_Pagamento.Where(p => p.nNumeroParcela_Info_Pag > nParcela).ToList().ForEach(p => p.nNumeroParcela_Info_Pag -= 1);
            dtgPagamento_DataBind();
        }

        protected void dtgPagamento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColunaBotoes = e.Row.Cells.Count - 2;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = DataBinder.Eval(e.Row.DataItem, "sNomeArquivo").ToString();

                foreach (LinkButton lnk in e.Row.Cells[nColunaBotoes].Controls.OfType<LinkButton>())
                {
                    if ((lnk.ID == "lnkPagamento_Download" && sNomeArquivo == "") || (lnk.ID == "lnkPagamento_UpLoad" && sNomeArquivo != ""))
                    {
                        lnk.Visible = false;
                    }
                }
            }
            int nColunaBotao = e.Row.Cells.Count - 1;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = DataBinder.Eval(e.Row.DataItem, "sNomeArquivo").ToString();

                foreach (LinkButton lnk in e.Row.Cells[nColunaBotao].Controls.OfType<LinkButton>())
                {
                    if (lnk.ID == "Pagamento_Download" && sNomeArquivo == "")
                    {
                        lnk.Visible = false;
                    }
                }
            }

            if (e.Row.RowType == DataControlRowType.Footer)
            {
                var grid = (GridView)sender;
                decimal total = 0;
                decimal pago = 0;

                foreach (GridViewRow row in grid.Rows)
                {
                    (row.FindControl("sMoedaOrigem26") as HtmlGenericControl).InnerText = hddMoeda.Value;
                    pago += decimal.Parse((row.FindControl("nValorPagamento_Info_Pag") as Label).Text);
                }
                txtPago.Text = Convert.ToString(pago);
            }
            EsconderColunas(e, 0);
        }

        protected void dtgPagamento_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = 0;

            if (e.CommandArgument != null && !string.IsNullOrEmpty(e.CommandArgument.ToString()))
            {
                idLinha = int.Parse(e.CommandArgument.ToString());
                hddsBloco.Value = "Pagamento";
                hddPagamento_idLinha.Value = idLinha.ToString();
            }

            if (e.CommandArgument.ToString() != "")
            {
                int.Parse(e.CommandArgument.ToString());
                hddsBloco.Value = "Pagamento";
                hddPagamento_idLinha.Value = idLinha.ToString();
            }

            if (e.CommandName == "Upload_Arquivo")
            {
                AbrirModal_EnvioArquivoPagamento(eBloco.Pagamento, idLinha, bs_Pagamento[bs_Pagamento.FindIndex(x => x.idLinha.Equals(idLinha))].nNumeroParcela_Info_Pag.ToString());
            }
            else if (e.CommandName == "Download_Arquivo")
            {
                Efetuar_Download_ArquivoPagamento(eBloco.Pagamento, idLinha);
            }
            else if (e.CommandName == "Editar")
            {
                cmdPagamento_Incluir.Visible = false;
                BtnEdicaoPagamento.Visible = true;
                var Pagamento = bs_Pagamento[bs_Pagamento.FindIndex(x => x.idLinha.Equals(idLinha))];
                txtnValorPagamento_Info_Pag.Text = Pagamento.nValorPagamento_Info_Pag.ToString();
                txtsContratoCambio.Text = Pagamento.sContratoCambio.ToString();
                var dt3 = Convert.ToDateTime(Pagamento.dtPagamento_Info_Pag);
                txtdtPagamento_Info_Pag.Text = dt3.ToString(@"yyyy/MM/dd").Replace('/', '-');
                txtsCorretora.Text = Pagamento.sCorretora.ToString();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtdtPagamento_Info_Pag]').focus();", true);
            }
            RegistraScript("");
        }

        void AbrirModal_EnvioArquivoPagamento(eBloco bloco, int idRegistro, string sTituloModal)
        {
            div4.Visible = false;
            div8.Visible = true;
            div14.Visible = false;
            lblEnviarArquivos_Titulo.Text = sTituloModal;
            txtEnviarArquivo_sDscArquivo.Text = "";
            hddIdLinha.Value = idRegistro.ToString();
            hddsBloco.Value = bloco.ToString();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_AbrirModalUploadArquivos", "$('#UploadArquivos_Modal').modal('show')", true);
        }

        void Efetuar_Download_ArquivoPagamento(eBloco bloco, int idLinha)
        {
            int index;
            int idArquivo = 0;
            string sNomeArquivo = "";
            byte[] bObjArquivo = null;
            string urlAtualPagina = Request.UrlReferrer.ToString().Replace(Request.RawUrl, "/Download/");

            switch (bloco)
            {
                case eBloco.Pagamento:
                    index = bs_Pagamento.FindIndex(x => x.idLinha.Equals(idLinha));
                    sNomeArquivo = bs_Pagamento[index].sNomeArquivo;
                    bObjArquivo = bs_Pagamento[index].objArquivo;
                    idArquivo = bs_Pagamento[index].idArquivo;
                    break;

                case eBloco.Garantia:
                    index = bs_Garantia.FindIndex(x => x.idLinha.Equals(idLinha));
                    sNomeArquivo = bs_Garantia[index].sNomeArquivo;
                    bObjArquivo = bs_Garantia[index].objArquivo;
                    idArquivo = bs_Garantia[index].idArquivo;
                    break;
            }

            if (bObjArquivo == null)
            {
                Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
                DataTable dtArquivo;
                vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "CONSULTAR_DETALHE" },
                        {"@idArquivo",                  idArquivo.ToString()}
                    };

                dtArquivo = ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);
                foreach (DataRow item in dtArquivo.Rows)
                {
                    sNomeArquivo = item["sNomeArquivo"].ToString();
                    bObjArquivo = (byte[])item["vbArquivo"];
                }
            }

            //Gera o Arquivo
            TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
            FileStream lObjFile = objArquivo.TransformarArrayBytesEmArquivo(bObjArquivo, Server.MapPath("~/Download/" + sNomeArquivo));
            lObjFile.Close();
            lObjFile.Dispose();

            //Efetua o Download
            StringBuilder strDownload = new StringBuilder();
            strDownload.AppendLine("var link = document.createElement('a');");
            strDownload.AppendLine("link.download = '" + sNomeArquivo + "';");
            strDownload.AppendLine("link.href = '" + string.Concat(urlAtualPagina, sNomeArquivo) + "';");
            strDownload.AppendLine("link.click();");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Download_dArquivos", strDownload.ToString(), true);
        }

        void Popular_gv_Garantia(DataSet dsPesquisa)
        {
            bs_Garantia.Clear();
            foreach (DataRow row in dsPesquisa.Tables[6].Rows)
            {
                FrameWork.cls_Pedidos_Garantia objItem = new FrameWork.cls_Pedidos_Garantia();

                objItem.idRegistro = Convert.ToInt32(row["idRegistro"].ToString());
                objItem.idLinha = bs_Garantia.Count() + 1;
                objItem.sFuncao = "SEM ALTERAÇÃO";
                objItem.sDscGarantia = (row["sDscGarantia"].ToString());
                objItem.sEnvioRemesa = (row["sEnvioRemesa"].ToString());
                objItem.dtGarantia = DateTime.Parse(row["dtGarantia"].ToString()).ToString("dd/MM/yyyy");
                objItem.sGarantia = (row["sGarantia"].ToString());
                objItem.nValorGarantia = ConverterStringDecimal(row["nValorGarantia"].ToString());
                objItem.idArquivo = Convert.ToInt32(row["idArquivo"].ToString());
                objItem.sNomeArquivo = row["sNomeArquivo"].ToString();
                bs_Garantia.Add(objItem);
            }
            gv_Garantia_DataBind();
            LimpaCampos_Garantia();
        }

        protected void cmdGarantia_Click(object sender, EventArgs e)
        {
            string sMensagem = "";
            if (ValidarDados_Garantia(ref sMensagem))
            {
                FrameWork.cls_Pedidos_Garantia objItem = new FrameWork.cls_Pedidos_Garantia();

                objItem.idLinha = bs_Garantia.Count() + 1;
                objItem.sFuncao = "Inserir_Garantia";
                objItem.sDscGarantia = txtsDscGarantia.Text;
                objItem.sEnvioRemesa = txtsEnvioRemesa.Text;
                objItem.dtGarantia = txtdtGarantia.Text;
                objItem.sGarantia = ddlsGarantia.SelectedItem.ToString();
                objItem.nValorGarantia = ConverterStringDecimal(txtnValorGarantia.Text);
                objItem.sNomeArquivo = "";
                bs_Garantia.Add(objItem);
            }
            gv_Garantia_DataBind();
            LimpaCampos_Garantia();
            RetornarScripts();
        }

        bool Salvar_Garantia(string idPedido)
        {
            bool bRetorno = true;
            try
            {
                foreach (var Linha in bs_Garantia)
                {
                    int idArquivo = Linha.idArquivo;
                    if (Linha.sNomeArquivo != "" && Linha.idArquivo == 0)
                    {
                        TT_Flow.FrameWork.cls_Arquivos Arquivo = new FrameWork.cls_Arquivos();
                        Arquivo.idTipoArquivo = 9999;
                        Arquivo.sNomeArquivo = Linha.sNomeArquivo;
                        Arquivo.sDscArquivo = Linha.sObservacaoArquivo;
                        Arquivo.sObservacao = "";
                        Arquivo.idUsuario = Convert.ToInt32(Variaveis.idUsuario());
                        Arquivo.vbArquivo = Linha.objArquivo;
                        Arquivo.dtExpiracaoDoc = "";
                        Linha.idArquivo = Convert.ToInt32(DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));

                        if (Linha.sFuncao == "SEM ALTERAÇÃO")
                        {
                            Linha.sFuncao = "Inserir_Garantia";
                        }
                    }
                    if (Linha.sFuncao != "SEM ALTERAÇÃO")
                    {
                        Dictionary<String, String> vParametro = new Dictionary<string, string>
                        {
                            ["@idRegistro"] = Linha.idRegistro.ToString(),
                            ["@sFuncao"] = Linha.sFuncao,
                            ["@idPedido"] = idPedido,
                            ["@nValorGarantia"] = Linha.nValorGarantia.ToString().Replace(",", "."),
                            ["@dtGarantia"] = Linha.dtGarantia.ToString(),
                            ["@sDscGarantia"] = Linha.sDscGarantia.ToString(),
                            ["@sEnvioRemesa"] = Linha.sEnvioRemesa.ToString(),
                            ["@sGarantia"] = Linha.sGarantia.ToString(),
                            ["@idArquivo"] = Linha.idArquivo.ToString()
                        };

                        ExecutarDataSet(sProcedure, vParametro);
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
                bRetorno = false;
            }
            return bRetorno;
        }

        void gv_Garantia_DataBind()
        {
            gv_Garantia.DataSource = bs_Garantia.Where(c => c.sFuncao.ToString() != "Excluir_Garantia").OrderBy(x => x.idLinha);
            gv_Garantia.DataBind();
            foreach (GridViewRow row in gv_Garantia.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    (row.FindControl("sMoedaOrigem7") as HtmlGenericControl).InnerText = sMoeda;
                }
            }
        }

        void LimpaCampos_Garantia()
        {
            ddlsGarantia.SelectedValue = "0";
            txtsDscGarantia.Text = "";
            txtdtGarantia.Text = "";
            txtnValorGarantia.Text = "";
            txtsEnvioRemesa.Text = "";
        }

        private bool ValidarDados_Garantia(ref string sMensagem)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlsGarantia.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione a Garantia";
            }

            if (txtsDscGarantia.Text == "")
            {
                if (sMensagemErro != "")
                {
                    sMensagemErro = sMensagemErro + "</br>";
                }
                sMensagemErro += "Descrição Incorreta";
            }

            if (txtdtGarantia.Text == "")
            {
                if (sMensagemErro != "")
                {
                    sMensagemErro = sMensagemErro + "</br>";
                }
                sMensagemErro += "Data Invalida";
            }

            if (txtnValorGarantia.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Invalido" + "</br>";
            }

            if (txtsEnvioRemesa.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Envio Remessa Incorreto" + "</br>";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPagina4.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        protected void gv_Garantia_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(gv_Garantia.DataKeys[e.RowIndex]["idLinha"].ToString());
            bs_Garantia[bs_Garantia.FindIndex(x => x.idLinha.Equals(idLinha))].sFuncao = "Excluir_Garantia";
            gv_Garantia_DataBind();
        }

        //---------- Higor Maestrello 16/07/2024 -----------
        protected void gv_Garantia_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColunaBotoes = e.Row.Cells.Count - 2;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = DataBinder.Eval(e.Row.DataItem, "sNomeArquivo").ToString();

                foreach (LinkButton lnk in e.Row.Cells[nColunaBotoes].Controls.OfType<LinkButton>())
                {
                    if ((lnk.ID == "lnkGarantia_Download" && sNomeArquivo == "") || (lnk.ID == "lnkGarantia_UpLoad" && sNomeArquivo != ""))
                    {
                        lnk.Visible = false;
                    }
                }
            }

            int nColunaBotao = e.Row.Cells.Count - 1;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = DataBinder.Eval(e.Row.DataItem, "sNomeArquivo").ToString();

                foreach (LinkButton lnk in e.Row.Cells[nColunaBotao].Controls.OfType<LinkButton>())
                {
                    if (lnk.ID == "Garantia_Download" && sNomeArquivo == "")
                    {
                        lnk.Visible = false;
                    }
                }
            }
        }

        protected void gv_Garantia_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = 0;

            if (e.CommandArgument != null && !string.IsNullOrEmpty(e.CommandArgument.ToString()))
            {
                idLinha = int.Parse(e.CommandArgument.ToString());
                hddsBloco.Value = "Pagamento";
                hddGarantia_idLinhaArquivo.Value = idLinha.ToString();
            }

            if (e.CommandArgument.ToString() != "")
            {
                int.Parse(e.CommandArgument.ToString());
                hddsBloco.Value = "Pagamento";
                hddGarantia_idLinhaArquivo.Value = idLinha.ToString();
            }

            if (e.CommandName == "Upload_Arquivo")
            {
                AbrirModal_EnvioArquivoGarantia(eBloco.Garantia, idLinha, bs_Garantia[bs_Garantia.FindIndex(x => x.idLinha.Equals(idLinha))].sDscGarantia.ToString());
            }
            else if (e.CommandName == "Download_Arquivo")
            {
                Efetuar_Download_ArquivoPagamento(eBloco.Garantia, idLinha);
            }
        }

        void AbrirModal_EnvioArquivoGarantia(eBloco bloco, int idRegistro, string sTituloModal)
        {
            div4.Visible = false;
            div8.Visible = false;
            div14.Visible = true;
            lblEnviarArquivos_Titulo.Text = sTituloModal;
            txtEnviarArquivo_sDscArquivo.Text = "";
            hddIdLinha.Value = idRegistro.ToString();
            hddsBloco.Value = bloco.ToString();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_AbrirModalUploadArquivos", "$('#UploadArquivos_Modal').modal('show')", true);
        }
        //--------------------------------------------------

        #endregion

        #region | Aba De / Para

        void depara_Pesquisar()
        {
            DataSet dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "RETORNO_OBRAS" },
                { "@idPedido", hddidPedido.Value }
            };
            dsPesquisa = ExecutarDataSet(LM_sProcedure, vParametros);
            Base_LM_Itens.Clear();

            foreach (var linha in Base_Pedidos_Itens)
            {
                DePara_Incluir_Item(linha.idProduto
                                , linha.nOrdem
                                , ""
                                , linha.idProduto
                                , linha.sCodigoProduto
                                , linha.sDscProduto
                                , linha.sUnidade
                                , linha.nQuantidade
                                , "SALVAR_ITENS"
                                , linha.sObservacao
                                , "TTL"
                                , "S"
                                , 0
                                , 0
                                , 0
                                , 0
                                , 0
                                , 0);
            }

            foreach (DataRow row in dsPesquisa.Tables[0].Rows)
            {
                DePara_Incluir_Item(Convert.ToInt32(row["idRegistro"])
                                , Convert.ToInt32(row["nOrdem"])
                                , row["sAgrupamento"].ToString()
                                , Convert.ToInt32(row["idProduto"])
                                , row["sCodigo"].ToString()
                                , row["sDscProduto"].ToString()
                                , row["sUnidade"].ToString()
                                , 0
                                , "SALVAR_ITENS"
                                , row["sObservacao"].ToString()
                                , row["sEscopo"].ToString()
                                , row["sMsg"].ToString()
                                , Convert.ToInt32(row["idTipoLM"].ToString())
                                , Convert.ToDouble(row["nQuantidade"].ToString())
                                , Convert.ToDouble(row["nQuantidadeO"].ToString())
                                , Convert.ToDouble(row["nQuantidadeRO"].ToString())
                                , Convert.ToDouble(row["nQuantidadeCO"].ToString())
                                , 0
                                );
            }

            foreach (var item in Base_LM_Itens)
            {
                item.nTotal = item.nQuantidade - item.nQuantidadeE - item.nQuantidadeO + item.nQuantidadeRO; //+ item.nQuantidadeCO;
            }

            gv_depara.DataSource = Base_LM_Itens.Where(x => x.sFuncao != "EXCLUIR_ITENS").OrderBy(x => x.sDscCategoriaVendas).ThenBy(y => y.nOrdem).ToList();
            gv_depara.DataBind();
            Upddepara.Update();
        }

        void DePara_Incluir_Item(int idRegistro, int nOrdem, string sAgrupamento, int idProduto, string sCodigoProduto, string sDscProduto, string sUnidade, double nQuantidade, string sFuncao, string sObservacao, string sEscopo, string sMsg, int idTipo, double nQuantidadeE, double nQuantidadeO, double nQuantidadeRO, double nQuantidadeCO, double sTotal)
        {
            // Verifica se já existe um item com as mesmas características na lista
            var itemExistente = new cls_Pedidos_Itens();

            if (idTipo != 0)
            {
                itemExistente = Base_LM_Itens.FirstOrDefault(c => c.idProduto == idProduto);
            }
            else
            {
                itemExistente = Base_LM_Itens.FirstOrDefault(c => c.idProduto == idProduto);
            }

            if (itemExistente != null)
            {
                // Se existe um item com as mesmas características, adiciona a quantidade ao item existente
                if (idTipo == 0)
                {
                    itemExistente.nQuantidade = nQuantidade;
                }
                if (idTipo == 1)
                {
                    itemExistente.nQuantidadeE = nQuantidadeE;
                }
                if (idTipo == 2)
                {
                    itemExistente.nQuantidadeO += nQuantidadeO;
                }
                if (idTipo == 3)
                {
                    itemExistente.nQuantidadeRO += nQuantidadeRO;
                }
                if (idTipo == 4)
                {
                    itemExistente.nQuantidadeCO += nQuantidadeCO;
                }
            }
            else
            {
                // Se não existe, cria um novo item e adiciona à lista
                FrameWork.cls_Pedidos_Itens objItem = new FrameWork.cls_Pedidos_Itens();
                objItem.idContador = Base_LM_Itens.Count() + 1;
                objItem.nOrdem = Base_LM_Itens.Where(c => c.sDscCategoriaVendas.Equals(sAgrupamento, StringComparison.OrdinalIgnoreCase)).Count() + 1;
                objItem.sDscCategoriaVendas = sAgrupamento;
                objItem.idProduto = idProduto;
                objItem.sCodigoProduto = sCodigoProduto;
                objItem.sDscProduto = sDscProduto;
                objItem.sUnidade = sUnidade;
                objItem.nQuantidade = nQuantidade;
                objItem.nQuantidadeE = nQuantidadeE;
                objItem.nQuantidadeO = nQuantidadeO;
                objItem.nQuantidadeRO = nQuantidadeRO;
                objItem.nQuantidadeCO = nQuantidadeCO;
                objItem.sFuncao = sFuncao;
                objItem.sObservacao = sObservacao;
                objItem.sEscopo = sEscopo;
                objItem.idItem = idRegistro;
                Base_LM_Itens.Add(objItem);
            }

            LM_LimpaCampos_Itens();
        }

        protected void gv_depara_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Obtém o idProduto para a linha atual
                int idProduto = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "idProduto"));

                // Realiza as operações desejadas com base no idProduto
                AdicionarLinhasComposicao(idProduto, e.Row, 1);
            }
        }

        #endregion

        #region | Aba Valores e Datas

        void Importacao_ValoresDatas_Insert()
        {
            try
            {
                foreach (GridViewRow envio in gv_Itens_ValoresDatas.Rows)
                {
                    try
                    {
                        cls_Pedidos_Itens objEnvios = Base_Pedidos_Itens.FirstOrDefault(e => e.idContador == Int32.Parse(gv_Itens_ValoresDatas.DataKeys[envio.RowIndex]["idContador"].ToString()));

                        if (objEnvios.Importacao_dtPO != ((envio.FindControl("Itens_ValoresDatas_txtdtPO") as TextBox).Text) && objEnvios.sFuncao != "INCLUIR ITEM" && objEnvios.sFuncao != "INEXISTENTE")
                            objEnvios.sFuncao = "ALTERAR_ITEM";

                        if (objEnvios.Importacao_dtETD != ((envio.FindControl("Itens_ValoresDatas_txtdtETD") as TextBox).Text) && objEnvios.sFuncao != "INCLUIR ITEM" && objEnvios.sFuncao != "INEXISTENTE")
                            objEnvios.sFuncao = "ALTERAR_ITEM";

                        if (objEnvios.Importacao_dtETA != ((envio.FindControl("Itens_ValoresDatas_txtdtETA") as TextBox).Text) && objEnvios.sFuncao != "INCLUIR ITEM" && objEnvios.sFuncao != "INEXISTENTE")
                            objEnvios.sFuncao = "ALTERAR_ITEM";

                        objEnvios.Importacao_dtPO = ((envio.FindControl("Itens_ValoresDatas_txtdtPO") as TextBox).Text);
                        objEnvios.Importacao_dtETD = ((envio.FindControl("Itens_ValoresDatas_txtdtETD") as TextBox).Text);
                        objEnvios.Importacao_dtETA = ((envio.FindControl("Itens_ValoresDatas_txtdtETA") as TextBox).Text);
                    }
                    catch
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                msgImportacao_Envio.MostraMensagem_Erro("Erro ao Incluir VOLUMES: " + ex.Message);
            }
        }

        protected void btAtualizarVlr_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gv_Itens_ValoresDatas.Rows)
            {
                if (Base_Pedidos_Itens.Where(i => i.idContador == decimal.Parse(row.Cells[0].Text)).First() is cls_Pedidos_Itens Valores)
                {
                    Valores.nQuantidade = double.Parse((row.FindControl("Itens_ValoresDatas_txtnQuantidade") as TextBox).Text);
                    Valores.nValorUnitario = double.Parse((row.FindControl("Itens_ValoresDatas_txtnValorUnitario") as TextBox).Text);
                    Valores.nValorTotal = double.Parse((row.FindControl("Itens_ValoresDatas_txtnQuantidade") as TextBox).Text) * double.Parse((row.FindControl("Itens_ValoresDatas_txtnValorUnitario") as TextBox).Text);
                    Valores.Importacao_dtPO = (row.FindControl("Itens_ValoresDatas_txtdtPO") as TextBox).Text;
                    Valores.Importacao_dtETD = (row.FindControl("Itens_ValoresDatas_txtdtETD") as TextBox).Text;
                    Valores.Importacao_dtETA = (row.FindControl("Itens_ValoresDatas_txtdtETA") as TextBox).Text;
                    if (Valores.sFuncao != "Inserir_item_envio")
                        Valores.sFuncao = "INCLUIR ITEM ALTERACAO";
                }
            }
            DataBind_dtgItens("Editar");
            decimal ocean = decimal.Parse(txtnOcean_Air.Text);
            decimal Insurance = decimal.Parse(txtnInsurance.Text);
            decimal Other = decimal.Parse(txtnOther_Charges.Text);
            decimal valor = ocean + Insurance + Other;
            txtnTotalEnvio.Text = valor.ToString();
            txtnVlrServicos.Text = valor.ToString();
        }

        #endregion

        #region | Aba OPI / Envios

        void Popular_Aba_OPI(string idPedido, string tp)
        {
            // CORREÇÃO: Usar PROC_OPI em vez de sProcedure
            Dictionary<string, string> vParametros = new Dictionary<string, string>
    {
        { "@sFuncao", "CONSULTAR-OPIS" }, // Certifique-se que essa função existe na proc de OPI e filtra por Pedido
        { "@idPedido", idPedido }
    };

            // Usando a constante da procedure correta
            DataSet dsOPIs = ExecutarDataSet(sProcedure, vParametros);

            if (dsOPIs.Tables.Count > 0 && dsOPIs.Tables[0].Rows.Count > 0)
            {
                rptOPIs.DataSource = dsOPIs.Tables[0];
                rptOPIs.DataBind();
                aba_OPI.Visible = true;
            }
            else
            {
                aba_OPI.Visible = false;
            }
        }

        protected void rptOPIs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string idOPI = DataBinder.Eval(e.Item.DataItem, "idOPI").ToString();

                Repeater rptEnvios = (Repeater)e.Item.FindControl("rptEnvios");
                Label lblSemEnvios = (Label)e.Item.FindControl("lblSemEnvios");

                Dictionary<string, string> vParametros = new Dictionary<string, string>
        {
            { "@sFuncao", "CONSULTAR-ENVIOS" },
            { "@idOPI", idOPI }
        };

                // CORREÇÃO: Usar PROC_OPI
                DataSet dsEnvios = ExecutarDataSet(sProcedure, vParametros);

                if (dsEnvios.Tables.Count > 0 && dsEnvios.Tables[0].Rows.Count > 0)
                {
                    rptEnvios.DataSource = dsEnvios.Tables[0];
                    rptEnvios.DataBind();
                    lblSemEnvios.Visible = false;
                }
                else
                {
                    rptEnvios.Visible = false;
                    lblSemEnvios.Visible = true;
                }
            }
        }

        protected void rptEnvios_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HiddenField hdnIdEnvioOPI = (HiddenField)e.Item.FindControl("hdnIdEnvioOPI");
                string idEnvioOPI = hdnIdEnvioOPI.Value;

                GridView dtgProdutosEnvio = (GridView)e.Item.FindControl("dtgProdutosEnvio");

                Dictionary<string, string> vParametros = new Dictionary<string, string>
        {
            { "@sFuncao", "CONSULTAR-ITENS-POR-ENVIO" },
            { "@idEnvioOPI", idEnvioOPI }
        };

                // CORREÇÃO: Você tinha deixado sProcedure aqui, alterei para PROC_OPI
                DataSet dsProdutos = ExecutarDataSet(sProcedure, vParametros);

                if (dsProdutos.Tables.Count > 0 && dsProdutos.Tables[0].Rows.Count > 0)
                {
                    dtgProdutosEnvio.DataSource = dsProdutos.Tables[0];
                    dtgProdutosEnvio.DataBind();
                }
                else
                {
                    dtgProdutosEnvio.Visible = false;
                }
            }
        }

        protected void AbrirModalArquivo_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                string idEnvioOPI = btn.CommandArgument;

                rptGaleriaFotos.DataSource = null;
                rptGaleriaFotos.DataBind();
                pnlSemFotos.Visible = false;

                Dictionary<string, string> vParametros = new Dictionary<string, string>
        {
            { "@sFuncao", "CONSULTAR-GALERIA-FOTOS-ENVIO" },
            { "@idEnvioOPI", idEnvioOPI }
        };

                // Garanta que está usando a string correta da procedure de OPI
                DataSet dsFotos = ExecutarDataSet(sProcedure, vParametros);

                if (dsFotos.Tables.Count > 0 && dsFotos.Tables[0].Rows.Count > 0)
                {
                    rptGaleriaFotos.DataSource = dsFotos.Tables[0];
                    rptGaleriaFotos.DataBind();
                }
                else
                {
                    pnlSemFotos.Visible = true;
                }

                updGaleria.Update();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "AbriModalGaleria", "$('#modalArquivosEnvio').modal('show');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "FecharModalGaleriaErro", "$('#modalArquivosEnvio').modal('hide');", true);
                MensagemPagina.MostraMensagem_Erro("Erro ao carregar fotos: " + ex.Message);
            }
        }

        protected string ConverterImagemBase64(object vbArquivo)
        {
            if (vbArquivo != DBNull.Value && vbArquivo != null)
            {
                byte[] bytes = (byte[])vbArquivo;
                if (bytes.Length > 0)
                {
                    // Assume-se que é imagem. O prefixo data:image/png funciona para jpg também na maioria dos browsers modernos
                    return "data:image/png;base64," + Convert.ToBase64String(bytes);
                }
            }
            // Retorna uma imagem de "sem foto" ou vazio caso não tenha arquivo
            return "/App/img/sem_imagem.png";
        }

        //protected void dtgOPI_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    DataTable m_DataTable = dtgOPI.DataSource as DataTable;

        //    if (m_DataTable != null)
        //    {
        //        DataView m_DataView = new DataView(m_DataTable);
        //        m_DataView.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(e.SortDirection);

        //        dtgOPI.DataSource = m_DataView;
        //        dtgOPI.DataBind();
        //    }
        //}

        //protected void dtgOPI_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        if (e.Row.Cells.Count > 6)
        //        {
        //            for (int i = 6; i < e.Row.Cells.Count; i++)
        //            {
        //                if (string.IsNullOrEmpty(e.Row.Cells[i].Text) || e.Row.Cells[i].Text == "&nbsp;")
        //                {
        //                    e.Row.Cells[i].Text = "0,0000";
        //                }
        //            }
        //        }
        //    }
        //}

        //void Popular_Aba_OPI(string idPedido, string LM)
        //{
        //    Dictionary<string, string> vParametros = new Dictionary<string, string>
        //    {
        //        { "@sFuncao", "CONSULTAR-ITENS" },
        //        { "@idPedido", idPedido }
        //    };
        //    DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

        //    if (dsPesquisa.Tables.Count > 0)
        //    {
        //        int i = 1;
        //        if (LM == "")
        //        {
        //            foreach (DataColumn coluna in dsPesquisa.Tables[0].Columns)
        //            {
        //                if (coluna.ColumnName != "idProduto" && coluna.ColumnName != "idOPI" && coluna.ColumnName != "sCodigo" && coluna.ColumnName != "nOrdem" && coluna.ColumnName != "sDscProduto" && coluna.ColumnName != "sUnidade" && coluna.ColumnName != "nQuantidade" && coluna.ColumnName != "nEnviar" && coluna.ColumnName != "OPI")
        //                {
        //                    BoundField bf = new BoundField();
        //                    bf.DataField = coluna.ColumnName;
        //                    bf.HeaderText = "Envio " + i;
        //                    bf.ItemStyle.Width = Unit.Percentage(7);
        //                    dtgOPI.Columns.Add(bf);
        //                    i++;
        //                }
        //            }

        //            BoundField st = new BoundField();
        //            st.DataField = "nEnviar";
        //            st.HeaderText = "Saldo";
        //            st.ItemStyle.Width = Unit.Percentage(7);
        //            dtgOPI.Columns.Add(st);
        //        }

        //        dtgOPI.DataSource = dsPesquisa.Tables[0];
        //        dtgOPI.DataBind();

        //        this.dtgOPI.Columns[3].Visible = false;
        //        this.dtgOPI.Columns[1].Visible = false;

        //        GridViewHelper helper = new GridViewHelper(dtgOPI);
        //        helper.GroupHeader += new GroupEvent(helper_GroupHeader);
        //        helper.GroupSummary += new GroupEvent(helper_Summary);
        //        helper.RegisterGroup("idOPI", true, true);
        //        helper.ApplyGroupSort();
        //    }
        //    else aba_OPI.Visible = false;
        //}

        #endregion

        #region | Aba STSO

        protected void ddlidEmpresaSTSO_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidEmpresaSTSO.SelectedValue != "0")
            {
                DIV_DocumentoEmpresa.Visible = true;
                DIV_ColaboradorSTSO.Visible = true;
                DIV_InserirDocumento.Visible = true;
                Popula_Combo(ddlDocumentoEmpresa, "sp_Select 'ARQUIVOS_EMPRESAS', @idPesquisa=" + ddlidEmpresaSTSO.SelectedValue, "idArquivo", "sDscDocumentos", false);
                ddlDocumentoEmpresa.Items.Add(new ListItem("Enviar Arquivo", "9999999"));
                Popula_Combo(ddlidColaborador, $"sp_Select 'Flow_Credor_Colaboradores_Filtrado', @idPesquisa={ddlidEmpresaSTSO.SelectedValue}, @idUsuario={Variaveis.idUsuario()}", "idColaborador", "sDscColaborador", false, "Selecione o Colaborador", "0");
            }
            else
            {
                DIV_DocumentoEmpresa.Visible = false;
                DIV_ColaboradorSTSO.Visible = false;
                DIV_InserirDocumento.Visible = false;
            }
            DataBind_DocumentosSTSO();

            var idsEmpresasRemove = Base_Arquivos_STSO.Where(x => x.sFuncao != "EXCLUIR_DOCUMENTO_STSO").Select(x => x.idArquivo).ToList();
            foreach (ListItem item in ddlDocumentoEmpresa.Items.Cast<ListItem>().ToList())
            {
                if (!string.IsNullOrEmpty(item.Value) && idsEmpresasRemove.Contains(Convert.ToInt32(item.Value)))
                {
                    ddlDocumentoEmpresa.Items.Remove(item);
                }
            }
        }

        protected void btnInserirDocumento_Click(object sender, EventArgs e)
        {
            string Inserir = "S";
            int quantidade = 0;
            int qtdColaborador = 0;
            List<string> ids = new List<string>();
            List<string> idsColaborador = new List<string>();
            foreach (ListItem item in ddlDocumentoEmpresa.Items)
            {
                if (item.Selected == true)
                {
                    quantidade += 1;
                    ids.Add(item.Value);
                }
            }
            if (quantidade > 1 && ids.Contains("9999999"))
            {
                Inserir = "N";
            }

            foreach (ListItem itemColaborador in ddlDocumentoColaboradores.Items)
            {
                if (itemColaborador.Selected == true)
                {
                    qtdColaborador += 1;
                    idsColaborador.Add(itemColaborador.Value);
                }
            }
            if (qtdColaborador > 1 && idsColaborador.Contains("9999999"))
            {
                Inserir = "N";
            }

            if (ddlDocumentoEmpresa.SelectedValue == "9999999" && ddlDocumentoColaboradores.SelectedValue == "9999999")
            {
                Inserir = "N";
            }
            else if (ddlDocumentoEmpresa.SelectedValue == "9999999" && ddlDocumentoColaboradores.SelectedValue != "9999999" && ddlDocumentoColaboradores.SelectedValue != "")
            {
                Inserir = "N";
            }
            else if (ddlDocumentoEmpresa.SelectedValue != "9999999" && ddlDocumentoEmpresa.SelectedValue != "" && ddlDocumentoColaboradores.SelectedValue == "9999999")
            {
                Inserir = "N";
            }

            if (Inserir == "S")
            {
                if (ddlDocumentoEmpresa.SelectedValue != "9999999")
                {
                    foreach (ListItem item in ddlDocumentoEmpresa.Items)
                    {
                        if (item.Selected == true)
                        {
                            FrameWork.cls_Arquivos_STSO objItem = new FrameWork.cls_Arquivos_STSO();

                            objItem.idContador = Base_Arquivos_STSO.Count() + 1;
                            objItem.idArquivo = int.Parse(item.Value);
                            string[] partes = item.ToString().Split('-');
                            if (partes.Length > 0)
                            {
                                objItem.sNome = ddlidEmpresaSTSO.SelectedItem.ToString().Replace("-", "");
                                objItem.sTipo = partes[0] + " - " + partes[1];
                                objItem.sDscsNomeArquivo = partes[2];
                                objItem.sDescricao = partes[3];
                            }
                            objItem.sDscsArquivo = objItem.sNome + " - " + item.ToString();
                            objItem.sFuncao = "INSERIR_DOCUMENTO_STSO";
                            objItem.sAprovacao = "N";
                            objItem.sEmpresaxColaborador = "Empresa";
                            objItem.idObjeto = int.Parse(ddlidEmpresaSTSO.SelectedValue);
                            Base_Arquivos_STSO.Add(objItem);

                            if (Base_Documentos_STSO.Where(c => c.sNome.Trim().ToLower() == ddlidEmpresaSTSO.SelectedItem.ToString().Replace("-", "").Trim().ToLower()).Count() <= 0)
                            {
                                FrameWork.cls_Arquivos_STSO obj = new FrameWork.cls_Arquivos_STSO();
                                obj.sNome = ddlidEmpresaSTSO.SelectedItem.ToString().Replace("-", "");
                                obj.sEmpresaxColaborador = "Empresa";
                                obj.sTipo = "Empresa";
                                obj.idObjeto = int.Parse(ddlidEmpresaSTSO.SelectedValue);
                                Base_Documentos_STSO.Add(obj);
                            }
                        }
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalArquivosSTSO", "$('#Modal_Arquivo_STSO').modal('show');", true);
                    frmArquivosSTSO.Attributes.Add("src", string.Format("Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}&sEmpresaxColaborador={2}&idPedido={3}", ddlidEmpresaSTSO.SelectedValue, "EmpresaSTSO", ddlidEmpresaSTSO.SelectedItem, hddidPedido.Value));
                }

                if (ddlDocumentoColaboradores.SelectedValue != "9999999")
                {
                    foreach (ListItem item in ddlDocumentoColaboradores.Items)
                    {
                        if (item.Selected == true)
                        {
                            FrameWork.cls_Arquivos_STSO objItem = new FrameWork.cls_Arquivos_STSO();

                            objItem.idContador = Base_Arquivos_STSO.Count() + 1;
                            objItem.idArquivo = int.Parse(item.Value);
                            string[] partes = item.ToString().Split('-');
                            string[] Nome = ddlidColaborador.SelectedItem.ToString().Split('-');
                            if (partes.Length > 0)
                            {
                                objItem.sNome = Nome[0];
                                objItem.sTipo = partes[0] + " - " + partes[1];
                                objItem.sDscsNomeArquivo = partes[2];
                                objItem.sDescricao = partes[3];
                            }
                            objItem.sDscsArquivo = objItem.sNome + " - " + item.ToString();
                            objItem.sFuncao = "INSERIR_DOCUMENTO_STSO";
                            objItem.sAprovacao = "N";
                            objItem.sEmpresaxColaborador = "Colaborador";
                            objItem.idObjeto = int.Parse(ddlidColaborador.SelectedValue);
                            Base_Arquivos_STSO.Add(objItem);

                            if (Base_Documentos_STSO.Where(c => c.sNome.Trim().ToLower() == Nome[0].Trim().ToLower()).Count() <= 0)
                            {
                                FrameWork.cls_Arquivos_STSO obj = new FrameWork.cls_Arquivos_STSO();
                                obj.sNome = Nome[0];
                                obj.sEmpresaxColaborador = "Colaborador";
                                obj.sTipo = "Colaborador";
                                obj.idObjeto = int.Parse(ddlidColaborador.SelectedValue);
                                Base_Documentos_STSO.Add(obj);
                            }
                        }
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalArquivosSTSO", "$('#Modal_Arquivo_STSO').modal('show');", true);
                    string[] Nome = ddlidColaborador.SelectedItem.ToString().Split('-');
                    frmArquivosSTSO.Attributes.Add("src", string.Format("Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}&sEmpresaxColaborador={2}&idPedido={3}", ddlidColaborador.SelectedValue, "ColaboradorSTSO", Nome[0], hddidPedido.Value));
                }

                DataBind_DocumentosSTSO();
                LimparSTSO();
            }
            else
            {
                MensagemPagina6.MostraMensagem_Erro("Uso da função para enviar arquivos de empresa ou de colaboradores devem ser inseridos individualmente, um por vez!");
            }
        }

        void DataBind_DocumentosSTSO()
        {
            gvDocumentosSTSO.DataSource = Base_Documentos_STSO;
            gvDocumentosSTSO.DataBind();

            if (Base_Arquivos_STSO.Count() > 0)
            {
                DIV28.Visible = true;
            }
        }

        protected void gvDocumentosSTSO_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var gv = e.Row.FindControl("gvDocumentosSTSO_1") as GridView;
                if (gv != null)
                {
                    gv.DataSource = Base_Arquivos_STSO.Where(c => c.sFuncao.ToString() != "EXCLUIR_DOCUMENTO_STSO" && c.sEmpresaxColaborador == gvDocumentosSTSO.DataKeys[e.Row.RowIndex]["sTipo"].ToString() && c.idObjeto.ToString() == gvDocumentosSTSO.DataKeys[e.Row.RowIndex]["idObjeto"].ToString());
                    gv.DataBind();
                }
            }
        }

        void LimparSTSO()
        {
            Popula_Combo(ddlDocumentoEmpresa, "sp_Select 'ARQUIVOS_EMPRESAS', @idPesquisa=" + ddlidEmpresaSTSO.SelectedValue, "idArquivo", "sDscDocumentos", false);
            ddlDocumentoEmpresa.Items.Add(new ListItem("Enviar Arquivo", "9999999"));
            Popula_Combo(ddlDocumentoColaboradores, "sp_Select 'ARQUIVOS_COLABORADORES', @idFiltro=" + ddlidColaborador.SelectedValue + ", @idPesquisa=" + ddlidEmpresaSTSO.SelectedValue, "idArquivo", "sDscDocumentos", false);
            ddlDocumentoColaboradores.Items.Add(new ListItem("Enviar Arquivo", "9999999"));

            var idsEmpresasRemove = Base_Arquivos_STSO.Where(x => x.sFuncao != "EXCLUIR_DOCUMENTO_STSO").Select(x => x.idArquivo).ToList();
            foreach (ListItem item in ddlDocumentoEmpresa.Items.Cast<ListItem>().ToList())
            {
                if (!string.IsNullOrEmpty(item.Value) && idsEmpresasRemove.Contains(Convert.ToInt32(item.Value)))
                {
                    ddlDocumentoEmpresa.Items.Remove(item);
                }
            }

            var idsColaboradoresRemove = Base_Arquivos_STSO.Where(x => x.sFuncao != "EXCLUIR_DOCUMENTO_STSO").Select(x => x.idArquivo).ToList();
            foreach (ListItem item in ddlDocumentoColaboradores.Items.Cast<ListItem>().ToList())
            {
                if (!string.IsNullOrEmpty(item.Value) && idsColaboradoresRemove.Contains(Convert.ToInt32(item.Value)))
                {
                    ddlDocumentoColaboradores.Items.Remove(item);
                }
            }
        }

        void ExcluirArquivo()
        {
            if (hddExcluirArquivo.Value != null)
            {
                int.TryParse(hddExcluirArquivo.Value, out int idLinha);
                var nParcela = Base_Arquivos_STSO[Base_Arquivos_STSO.FindIndex(x => x.idContador.Equals(idLinha))].idArquivo;
                Base_Arquivos_STSO[Base_Arquivos_STSO.FindIndex(x => x.idContador.Equals(idLinha))].sFuncao = "EXCLUIR_DOCUMENTO_STSO";
                DataBind_DocumentosSTSO();
            }

            if (ddlidEmpresaSTSO.SelectedValue != "0")
            {
                Popula_Combo(ddlDocumentoEmpresa, "sp_Select 'ARQUIVOS_EMPRESAS', @idPesquisa=" + ddlidEmpresaSTSO.SelectedValue, "idArquivo", "sDscDocumentos", false);
                ddlDocumentoEmpresa.Items.Add(new ListItem("Enviar Arquivo", "9999999"));
                Popula_Combo(ddlDocumentoColaboradores, "sp_Select 'ARQUIVOS_COLABORADORES', @idFiltro=" + ddlidColaborador.SelectedValue + ", @idPesquisa=" + ddlidEmpresaSTSO.SelectedValue, "idArquivo", "sDscDocumentos", false);
                ddlDocumentoColaboradores.Items.Add(new ListItem("Enviar Arquivo", "9999999"));

                var idsEmpresasRemove = Base_Arquivos_STSO.Where(x => x.sFuncao != "EXCLUIR_DOCUMENTO_STSO").Select(x => x.idArquivo).ToList();
                foreach (ListItem item in ddlDocumentoEmpresa.Items.Cast<ListItem>().ToList())
                {
                    if (!string.IsNullOrEmpty(item.Value) && idsEmpresasRemove.Contains(Convert.ToInt32(item.Value)))
                    {
                        ddlDocumentoEmpresa.Items.Remove(item);
                    }
                }

                var idsColaboradoresRemove = Base_Arquivos_STSO.Where(x => x.sFuncao != "EXCLUIR_DOCUMENTO_STSO").Select(x => x.idArquivo).ToList();
                foreach (ListItem item in ddlDocumentoColaboradores.Items.Cast<ListItem>().ToList())
                {
                    if (!string.IsNullOrEmpty(item.Value) && idsColaboradoresRemove.Contains(Convert.ToInt32(item.Value)))
                    {
                        ddlDocumentoColaboradores.Items.Remove(item);
                    }
                }
            }
        }

        void PopularSTSO(DataSet dsPesquisa)
        {
            bool bRetorno = true;

            try
            {
                foreach (DataRow row in dsPesquisa.Tables[8].Rows)
                {
                    if (row["idArquvoSTSO"].ToString() != "")
                    {
                        FrameWork.cls_Arquivos_STSO objItem = new FrameWork.cls_Arquivos_STSO();

                        objItem.idArquvoSTSO = Convert.ToInt32(row["idArquvoSTSO"].ToString());
                        objItem.idArquivo = Convert.ToInt32(row["idArquivo"].ToString());
                        objItem.idPedido = Convert.ToInt32(row["idPedido"].ToString());
                        string[] partes = row["sDscsArquivo"].ToString().Split('-');
                        if (partes.Length > 0)
                        {
                            objItem.sNome = partes[0];
                            objItem.sTipo = partes[1] + " - " + partes[2].Replace("_", "-");
                            objItem.sDscsNomeArquivo = partes[3];
                            objItem.sDescricao = partes[4];
                        }
                        objItem.sDscsArquivo = row["sDscsArquivo"].ToString();
                        objItem.sFuncao = "CONSULTAR";
                        objItem.idContador = Base_Arquivos_STSO.Count() + 1;
                        objItem.dtAtualizacao = row["dtAtualizacao"].ToString();
                        objItem.sAprovacao = row["sAprovacao"].ToString();
                        objItem.sEmpresaxColaborador = row["sTipo"].ToString();
                        objItem.idObjeto = int.Parse(row["idObjeto"].ToString());
                        Base_Arquivos_STSO.Add(objItem);
                        sEmail = row["sEmail"].ToString();

                        if (Base_Documentos_STSO.Where(c => c.sNome == partes[0]).Count() <= 0)
                        {
                            FrameWork.cls_Arquivos_STSO obj = new FrameWork.cls_Arquivos_STSO();
                            obj.sNome = partes[0];
                            obj.sEmpresaxColaborador = row["sTipo"].ToString();
                            obj.sTipo = row["sTipo"].ToString();
                            obj.idObjeto = int.Parse(row["idObjeto"].ToString());
                            Base_Documentos_STSO.Add(obj);
                        }
                    }
                }
                if (dsPesquisa.Tables[8].Rows.Count > 0)
                {
                    DIV29.Visible = true;
                    DIV39.Visible = true;
                    DIV37.Visible = true;
                }
                else
                {
                    sEmail = "";
                    DirecionaLink = "";
                }

                DataBind_DocumentosSTSO();
            }
            catch
            {

            }
        }

        protected void ddlidColaborador_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidColaborador.SelectedValue != "0")
            {
                DIV_DocumentoColaboradores.Visible = true;
                Popula_Combo(ddlDocumentoColaboradores, "sp_Select 'ARQUIVOS_COLABORADORES', @idFiltro=" + ddlidColaborador.SelectedValue + ", @idPesquisa=" + ddlidEmpresaSTSO.SelectedValue, "idArquivo", "sDscDocumentos", false);
                ddlDocumentoColaboradores.Items.Add(new ListItem("Enviar Arquivo", "9999999"));
            }
            else
            {
                DIV_DocumentoColaboradores.Visible = false;
            }
            DataBind_DocumentosSTSO();

            var idsColaboradoresRemove = Base_Arquivos_STSO.Where(x => x.sFuncao != "EXCLUIR_DOCUMENTO_STSO").Select(x => x.idArquivo).ToList();
            foreach (ListItem item in ddlDocumentoColaboradores.Items.Cast<ListItem>().ToList())
            {
                if (!string.IsNullOrEmpty(item.Value) && idsColaboradoresRemove.Contains(Convert.ToInt32(item.Value)))
                {
                    ddlDocumentoColaboradores.Items.Remove(item);
                }
            }
        }

        protected void cmdSalvarSTSO_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var Linha in Base_Arquivos_STSO)
                {
                    if (Linha.sFuncao != "CONSULTAR")
                    {
                        Dictionary<String, String> vParametro = new Dictionary<string, string>
                        {
                            ["@sFuncao"] = Linha.sFuncao,
                            ["@idArquvoSTSO"] = Linha.idArquvoSTSO.ToString(),
                            ["@idArquivo"] = Linha.idArquivo.ToString(),
                            ["@idPedido"] = hddidPedido.Value,
                            ["@sDscsArquivo"] = Linha.sDscsArquivo,
                            ["@idUsuarioAtualizacao"] = Variaveis.idUsuario(),
                            ["@sAprovacao"] = Linha.sAprovacao,
                            ["@sTipo"] = Linha.sEmpresaxColaborador
                        };

                        ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametro);
                    }
                }
                MensagemPagina6.MostraMensagem_Sucesso("Arquivos STSO gravado com sucesso!");
            }
            catch
            {
                MensagemPagina6.MostraMensagem_Erro("Erro ao Salvar os Arquivos STSO!");
            }
            PesquisarPedido(hddidPedido.Value, "", "0", "", "");
            Scripts.Mantem_AbaAtiva(Page, "STSO-tab");
            ddlidEmpresaSTSO.SelectedValue = "0";
        }

        protected void gvDocumentosSTSO_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable m_DataTable = ConvertTo<FrameWork.cls_Arquivos_STSO>(Base_Arquivos_STSO.Where(c => c.sFuncao.ToString() != "EXCLUIR_DOCUMENTO_STSO").ToList().OrderBy(x => x.sTipo).ToList());

            if (m_DataTable != null)
            {
                DataView m_DataView = new DataView(m_DataTable);
                m_DataView.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(e.SortDirection);
                gvDocumentosSTSO.DataSource = m_DataView;
                gvDocumentosSTSO.DataBind();
            }
        }

        void cmdGerarLinkSTSO_Click(string sidArquivo)
        {
            Random random = new Random();
            string senha = random.Next(1000, 10000).ToString();
            try
            {
                Dictionary<String, String> vParametro = new Dictionary<string, string>
                {
                    ["@sFuncao"] = "GERAR_LINK",
                    ["@sTipo"] = "Pedidos",
                    ["@sSenha"] = senha,
                    ["@idUsuarioInclusao"] = Variaveis.idUsuario(),
                    ["@sidArquivo"] = sidArquivo,
                    ["@idPedido"] = hddidPedido.Value
                };
                ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametro);

                MensagemPagina6.MostraMensagem_Sucesso("Link Gerado com Sucesso!");
            }
            catch
            {
                MensagemPagina6.MostraMensagem_Erro("Erro ao gerar Link!");
            }
            PesquisarPedido(hddidPedido.Value, "", "0", "", "");
            Scripts.Mantem_AbaAtiva(Page, "STSO-tab");
        }

        void cmdDownloadSTSO_Click(string Chave)
        {
            string[] partes = Chave.Split('=');
            DirecionaPagina_NovaAba(Page, "/App/Download.aspx?id=" + partes[1] + "&sTp=2" + "&idP=" + hddidCliente.Value);
            PesquisarPedido(hddidPedido.Value, "", "0", "", "");
            Scripts.Mantem_AbaAtiva(Page, "STSO-tab");
        }

        void cmdEnviarEmail_Click()
        {
            DIV33.Visible = true;
            DIV34.Visible = true;
            DIV35.Visible = true;
            DIV44.Visible = true;
            MensagemEnviarEmail.MostraMensagem_Aviso("Para inserir e-mails em cópia, separe-os com um ponto e virgula " + "';'" + " entre cada endereço.");

            DataBind_DocumentosSTSO();
        }

        protected void cmdEnviarEmailSTSO_Click(object sender, EventArgs e)
        {
            string mensagem = "";
            if (txtsNomeEmail.Text == "")
            {
                mensagem = "Informe o Nome do Destinatário ";
            }
            if (txtsEmail.Text == "")
            {
                mensagem += (mensagem != "" ? "</br>" : "") + "Informe o Email do Destinatário!";
            }

            if (mensagem == "")
            {
                try
                {
                    string razaoSocial = txtsRazaoSocial.Text;
                    var palavra = razaoSocial.Split(' ');
                    string usuario = string.Join("", palavra);
                    if (usuario.Length > 10)
                    {
                        usuario = usuario.Substring(0, Math.Min(usuario.Length, 10));
                    }

                    Dictionary<String, String> vParametro = new Dictionary<string, string>
                    {
                        ["@sFuncao"] = "GERAR_EMAIL_STSO",
                        ["@sNome"] = txtsNomeEmail.Text,
                        ["@sEmail"] = txtsEmail.Text,
                        ["@idLink"] = hddidLink.Value,
                        ["@idPedido"] = hddidPedido.Value,
                        ["@sLink"] = hddsChave.Value,
                        ["@sEmailCopia"] = txtsEmailCopia.Text,
                        ["@idUsuarioInclusao"] = Variaveis.idUsuario(),

                        ["@idEmpresa"] = hddidCliente.Value,
                        ["@sEmailSTSO"] = "",
                        ["@sUsuario"] = "P" + hddidCliente.Value + "." + usuario.Replace("ã", "a").Replace("ç", "c").Replace("é", "e").Replace("á", "a").Replace("â", "a")
                        .Replace("ú", "u").Replace("í", "i").Replace("ó", "o").Replace("õ", "o").Replace("ô", "o"),
                        ["@sSenhaSTSO"] = new Random().Next(10000000, 99999999).ToString(),
                        ["@sNomeSTSO"] = txtsRazaoSocial.Text
                    };
                    ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametro);
                    PesquisarPedido(hddidPedido.Value, "", "0", "", "");
                    Scripts.Mantem_AbaAtiva(Page, "STSO-tab");
                    MensagemPagina6.MostraMensagem_Sucesso("Email Enviado com Sucesso!");
                }
                catch
                {
                    MensagemPagina6.MostraMensagem_Erro("Erro ao Enviar o Email!");
                }
            }
            else
            {
                DataBind_DocumentosSTSO();
                MensagemPagina6.MostraMensagem_Erro(mensagem);
            }
        }

        void AprovarDocumento()
        {
            string idDocumento = hddAprovarDoc.Value;

            try
            {
                Dictionary<String, String> vParametro = new Dictionary<string, string>
                {
                    ["@sFuncao"] = "AprovarDoc",
                    ["@sidArquivo"] = idDocumento,
                    ["@sAprovacao"] = "S",
                    ["@idUsuarioAtualizacao"] = Variaveis.idUsuario(),
                    ["@idPedido"] = hddidPedido.Value
                };

                ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametro);

                MensagemPagina6.MostraMensagem_Sucesso("Arquivos Aprovado com sucesso!");
            }
            catch
            {
                MensagemPagina6.MostraMensagem_Sucesso("Erro ao Aprovado os Arquivos STSO!");
            }

            PesquisarPedido(hddidPedido.Value, "", "0", "", "");
            Scripts.Mantem_AbaAtiva(Page, "STSO-tab");
        }

        void ExcluirArquivos(string idsArquivos)
        {
            try
            {
                Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
                DataTable dtArquivo;
                vParametrosItem = new Dictionary<string, string>
                {
                {"@sFuncao", "EXCLUIR_DOCUMENTO_STSO" },
                {"@sidArquivo", idsArquivos},
                {"@idUsuarioAtualizacao", Variaveis.idUsuario()},
                {"@idPedido", hddidPedido.Value}
                };

                dtArquivo = ExecutarDataTable(sProcedure, vParametrosItem);

                MensagemPagina6.MostraMensagem_Sucesso("Arquivos Excluido com sucesso!");
            }
            catch
            {
                MensagemPagina6.MostraMensagem_Erro("Erro ao Excluir os Arquivos STSO!");
            }
        }

        void PopularDownloadArquivos(DataSet dsPesquisa)
        {
            bool bRetorno = true;
            try
            {
                foreach (DataRow row in dsPesquisa.Tables[9].Rows)
                {
                    if (row["idLink"].ToString() != "")
                    {
                        FrameWork.cls_Download_STSO objItem = new FrameWork.cls_Download_STSO();

                        objItem.idLink = Convert.ToInt16(row["idLink"].ToString());
                        objItem.sChave = row["sChave"].ToString();
                        objItem.sSenha = row["sSenha"].ToString();
                        objItem.dtCriacao = row["dtCriacao"].ToString();
                        objItem.sFuncao = "CONSULTAR";
                        Base_Download_STSO.Add(objItem);
                    }
                }
                DataBind_DownloadSTSO();
            }
            catch
            {

            }
        }

        void DataBind_DownloadSTSO()
        {
            gvDownloadSTSO.DataSource = Base_Download_STSO.OrderByDescending(x => x.idLink).ToList();
            gvDownloadSTSO.DataBind();

            if (Base_Download_STSO.Count > 0)
            {
                DIV29.Visible = false;
            }
        }

        protected void gvDownloadSTSO_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            EsconderColunas(e, 1);
            if (e.Row.Cells[2].Text != "Chave" && e.Row.Cells[2].Text != "&nbsp;")
            {
                sChave = e.Row.Cells[2].Text;
                e.Row.Cells[2].Text = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/App/Download.aspx?id=" + sChave + "&idP=" + hddidCliente.Value;
            }
        }

        protected void lnkVisualizar_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalItens", "$('#Modal_ArquivosSTSO').modal('show');", true);

            DataTable dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_ARQUIVOS_LINK" },
                { "@idLink", (sender as LinkButton).CommandArgument }
            };

            dsPesquisa = ExecutarDataTable(sProcedure, vParametros);

            List<int> ids = new List<int>();
            foreach (DataRow row in dsPesquisa.Rows)
            {
                int id = Convert.ToInt32(row["idArquivo"]);
                ids.Add(id);
            }

            Base_Arquivos_Link = Base_Arquivos_STSO.Where(a => ids.Contains(a.idArquivo)).ToList();

            gvArquivosSTSO.DataSource = Base_Arquivos_Link;
            gvArquivosSTSO.DataBind();

            GridViewHelper helper = new GridViewHelper(gvArquivosSTSO);
            helper.GroupHeader += new GroupEvent(helper_GroupHeader);
            helper.GroupSummary += new GroupEvent(helper_Summary);
            helper.RegisterGroup("sEmpresaxColaborador", true, true);
            helper.RegisterGroup("sNome", true, true);
            helper.ApplyGroupSort();

            DataBind_DocumentosSTSO();
        }

        protected void gvArquivosSTSO_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable m_DataTable = ConvertTo<FrameWork.cls_Arquivos_STSO>(Base_Arquivos_Link.Where(c => c.sFuncao.ToString() != "EXCLUIR_DOCUMENTO_STSO").ToList().OrderBy(x => x.sTipo).ToList());

            if (m_DataTable != null)
            {
                DataView m_DataView = new DataView(m_DataTable);
                m_DataView.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(e.SortDirection);
                gvArquivosSTSO.DataSource = m_DataView;
                gvArquivosSTSO.DataBind();
            }
        }

        void Popular_HistoricoSTSO(DataSet ds)
        {
            if (ds.Tables[10].Rows.Count > 0)
            {
                div30.Visible = true;
                gvHistoricoSTSO.DataSource = ds.Tables[10];
                gvHistoricoSTSO.DataBind();
            }
            else
            {
                div30.Visible = false;
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
                return string.Format(@"</td></tr><tr id='tr{0}{1}' class='collapsed-row'>
                               <td></td><td colspan='100' style='padding:0px; margin:0px;'>", gridNome, id);
            }
            else
            {
                // Se não houver ID, retorna uma string vazia para que nada seja renderizado e o botão de colapso desapareça
                return string.Empty;
            }
        }

        protected void gvDocumentosSTSO_1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string aceito = (e.Row.FindControl("Aprovado") as Label).Text;

                if (aceito.Equals("S"))
                {
                    (e.Row.FindControl("Aprovado") as Label).Visible = true;
                    (e.Row.FindControl("Aprovado") as Label).Text = "✔";
                }
                else
                {
                    (e.Row.FindControl("Aprovado") as Label).Visible = false;
                    (e.Row.FindControl("Aprovado") as Label).Text = "";
                }
            }
        }

        protected void gvHistoricoSTSO_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[2].Text == "Inclusão Documento" || e.Row.Cells[2].Text == "Excluiu o Documento" || e.Row.Cells[2].Text == "Inclus&#227;o Documento" || e.Row.Cells[2].Text == "Aprovou o Documento")
                {
                    if (e.Row.Cells[3].Text != "&nbsp;")
                    {
                        string[] partes = e.Row.Cells[3].Text.ToString().Split('-');
                        e.Row.Cells[3].Text = "<ul> <li> Nome: " + partes[0] + "</li><li> Tipo: " + partes[1] + " - " + partes[2] + "</li><li> Nome Documento: " + partes[3] + "</li><li> Descrição: " + partes[4] + "</li></ul>";
                        e.Row.Cells[3].Text = HttpUtility.HtmlDecode(e.Row.Cells[3].Text);
                    }
                }
                else if (e.Row.Cells[2].Text == "Envio Email")
                {
                    if (e.Row.Cells[3].Text != "&nbsp;")
                    {
                        string[] partes = e.Row.Cells[3].Text.ToString().Split('-');
                        e.Row.Cells[3].Text = "<ul> <li>" + partes[0] + "</li><li>" + partes[1] + "</li><li>" + partes[2] + "</li></ul>";
                        e.Row.Cells[3].Text = HttpUtility.HtmlDecode(e.Row.Cells[3].Text);
                    }
                }
                else if (e.Row.Cells[2].Text == "Gerou o Link")
                {
                    if (e.Row.Cells[3].Text != "&nbsp;")
                    {
                        e.Row.Cells[3].Text = "<ul> <li>" + HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/App/Download.aspx?id=" + sChave + "&idP=" + hddidCliente.Value + "</li></ul>";
                        e.Row.Cells[3].Text = HttpUtility.HtmlDecode(e.Row.Cells[3].Text);
                    }
                }
                else if (e.Row.Cells[2].Text == "Download")
                {
                    if (e.Row.Cells[3].Text != "&nbsp;")
                    {
                        e.Row.Cells[3].Text = "<ul> <li>" + e.Row.Cells[3].Text + "</li></ul>";
                        e.Row.Cells[3].Text = HttpUtility.HtmlDecode(e.Row.Cells[3].Text);
                    }
                }
            }
        }

        protected void gvDocumentosSTSO_1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idArquivo = int.Parse(e.CommandArgument.ToString());
            string sNomeArquivo = "";
            byte[] bObjArquivo = null;
            if (e.CommandName == "Download")
            {
                Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
                DataTable dtArquivo;
                vParametrosItem = new Dictionary<string, string>
                     {
                         {"@sFuncao",                    "CONSULTAR_DETALHE" },
                         {"@idArquivo",                  idArquivo.ToString()}
                     };

                dtArquivo = ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

                foreach (DataRow item in dtArquivo.Rows)
                {
                    sNomeArquivo = item["sNomeArquivo"].ToString();
                    bObjArquivo = (byte[])item["vbArquivo"];
                }
                TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                FileStream lObjFile = objArquivo.TransformarArrayBytesEmArquivo(bObjArquivo, Server.MapPath("~/Download/" + sNomeArquivo));
                lObjFile.Close();
                lObjFile.Dispose();

                DownloadArquivo(Page, sNomeArquivo);
            }
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalArquivosSTSO", "$('#Modal_Arquivo_STSO').modal('hide');", true);
            //PesquisarPedido(hddidPedido.Value, "", "0", "", "2");
            Scripts.Mantem_AbaAtiva(Page, "STSO-tab");
            DirecionaPagina("App/Paginas/Pedidos_Detalhe.aspx?id=" + hddidPedido.Value + "&sTp=2&sArquivoSTSO=S");
        }

        #endregion

        #region | Aba ART

        void PopularArt(string sCNPJ_Cliente, string sRazaoSocial, string sRazaoSocialFinal, string sCNPJ_ClienteFinal)
        {
            txtsEndereçoContratante.Text = txtsEnderecoEntrega.Text;
            txtnControle.Text = txtnControleTT.Text;
            txtnValorContrato.Text = txtnVlrTotal.Text;
            txtnContrato.Text = txtsPedidoCliente.Text;
            txtdtContrato.Text = txtdtPedido.Text;
            txtsRazaoContratante.Text = sRazaoSocial;
            if (sCNPJ_Cliente.Length == 14)
                txtsCNPJContratante.Text = Convert.ToUInt64(sCNPJ_Cliente).ToString(@"00\.000\.000\/0000\-00");
            else
                txtsCNPJContratante.Text = sCNPJ_Cliente;

            if (sClienteFinalswt.Recuperar() == "S")
            {
                DIV_ClienteFinal.Visible = true;
                if (sCNPJ_ClienteFinal != "")
                {
                    if (sCNPJ_ClienteFinal.Length == 14)
                        txtsCNPJClienteFinal.Text = Convert.ToUInt64(sCNPJ_ClienteFinal).ToString(@"00\.000\.000\/0000\-00");
                    else
                        txtsCNPJClienteFinal.Text = sCNPJ_ClienteFinal;
                }

                txtsRazaoClienteFinal.Text = sRazaoSocialFinal;
                txtsEnderecoClienteFinal.Text = txtsEnderecoEntrega.Text;
            }
            else
                DIV_ClienteFinal.Visible = false;

            frmArquivosART.Attributes.Add("src", string.Format("Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}&sTp={2}", hddidPedido.Value, "PedidoART", hddidTipo.Value));
        }

        protected void Consulta_gvCheckList(string idOrcamento)
        {
            list_Perguntas_x_Opcoes.Clear();

            Dictionary<string, string> vParametros = new Dictionary<string, string>()
            {
                { "@sFuncao", "CONSULTA_ESCOPOS" },
                { "@idOrcamento", idOrcamento }
            };

            DataSet ds = ExecutarDataSet(sProcedure, vParametros);

            if (ds.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    cls_Categoria item = new cls_Categoria();

                    item.idEscopo = Convert.ToInt32(row["idEscopo"].ToString());
                    item.idCategoria = Convert.ToInt32(row["idCategoriaEscopo"].ToString());
                    item.sPerguntas = row["idPergunta"].ToString().Length > 0 ? row["idPergunta"].ToString() + "|" + row["sPergunta"].ToString() : row["sPergunta"].ToString();
                    item.sOpcoes = row["idOpcao"].ToString() + "|" + row["sOpcao"].ToString();

                    list_Perguntas_x_Opcoes.Add(item);
                }

                Popula_gvCheckList(DATASET(ds, 1, 0, "idTipoOrcamento"), false, DATASET(ds, 1, 0, "sidEscopos"));
            }
            else
            {
                dt_CheckList = null;
            }
        }

        protected void Popula_gvCheckList(string idTipoOrcamento, bool bNovo, string sidEscopos)
        {
            dt_CheckList.Clear();

            Dictionary<string, string> vParametros = new Dictionary<string, string>()
            {
                { "@sFuncao", "CONSULTA_ESCOPOS" },
                { "@idTipoOrcamento", idTipoOrcamento },
                { "@sidEscopos", sidEscopos }
            };

            DataSet ds = ExecutarDataSet(sProcedure, vParametros);

            if (ValidarDataSet(ds))
            {
                dt_CheckList = ds.Tables[0];

                if (bNovo)
                    list_Perguntas_x_Opcoes.Clear();

                rptCategoriasEscopos_dataBind();
                Div50.Visible = true;
            }
            else
            {
                dt_CheckList.Clear();
                list_Perguntas_x_Opcoes.Clear();

                div_CheckList_View.Visible = false;
                Div50.Visible = false;
            }
        }

        protected void rptCategoriasEscopos_dataBind()
        {
            rptCheckList_View.DataSource = dt_CheckList;
            rptCheckList_View.DataBind();

        }

        protected void rptCheckList_View_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRow dr = (rptCheckList_View.DataSource as DataTable).Rows[e.Item.ItemIndex];

                GridView gv = e.Item.FindControl("gvCheckList_View") as GridView;

                string[] opcoes = dr.Field<string>("sOpcoes").Split('|');

                int i = 0;

                foreach (string opcao in opcoes.Where(o => o.Length > 0))
                {
                    TemplateField tf = new TemplateField();
                    tf.HeaderText = opcao;

                    tf.ItemTemplate = new CheckBoxItem(i, opcao, gv, true);
                    tf.HeaderStyle.Width = Unit.Percentage(opcao.Length < 10 ? 5 : opcao.Length < 20 ? 10 : 15);
                    tf.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                    tf.ItemStyle.VerticalAlign = VerticalAlign.Middle;

                    gv.Columns.Add(tf);

                    i++;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("sPergunta");
                dt.Columns.Add("idCategoria");
                dt.Columns.Add("idEscopo");
                dt.Columns.Add("sDscCategoria");

                foreach (string pergunta in dr.Field<string>("sPerguntas").Split('|').Where(p => p.Length > 0))
                {
                    dt.Rows.Add(pergunta, dr.Field<int>("idCategoria"), dr.Field<int>("idEscopo"), dr.Field<string>("sDscCategoria"));
                }

                gv.DataSource = dt;
                gv.DataBind();
            }
        }

        protected void gvCheckList_View_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                GridView gv = sender as GridView;

                if (gv != null)
                {
                    GridViewRow titleRow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

                    TableCell titleCell = new TableCell
                    {
                        ColumnSpan = gv.Columns.Count,
                        HorizontalAlign = HorizontalAlign.Center,
                        CssClass = "checkList_Title"
                    };
                    titleRow.Cells.Add(titleCell);

                    gv.Controls[0].Controls.AddAt(0, titleRow);
                }
            }
        }

        protected void gvCheckList_View_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridView gv = sender as GridView;

                string pergunta = HttpUtility.HtmlDecode(e.Row.Cells[2].Text);

                e.Row.ID = e.Row.RowIndex.ToString();
                e.Row.ClientIDMode = ClientIDMode.Static;

                if (list_Perguntas_x_Opcoes != null && list_Perguntas_x_Opcoes.Count > 0)
                {
                    var pergunta_x_opcao = list_Perguntas_x_Opcoes.Where(p => p.idEscopo.ToString() == e.Row.Cells[0].Text && p.idCategoria.ToString() == e.Row.Cells[1].Text && p.sPerguntas.Equals(pergunta)).FirstOrDefault();

                    if (pergunta_x_opcao != null)
                        (e.Row.Cells[Convert.ToInt32(pergunta_x_opcao.sOpcoes.Split('|')[0]) + 3].Controls[0] as CheckBox).Checked = true;
                    else
                    {
                        try
                        {
                            var list = list_Perguntas_x_Opcoes.Where(p => p.idEscopo.ToString() == e.Row.Cells[0].Text && p.idCategoria.ToString() == e.Row.Cells[1].Text && p.sPerguntas.Split('|').Length > 1).ToList();
                            pergunta_x_opcao = list.Where(p => p.sPerguntas.Split('|')[1].Equals(pergunta) || p.sPerguntas.Split('|')[0].Equals(e.Row.ID.ToString())).FirstOrDefault();

                            if (pergunta_x_opcao != null)
                                (e.Row.Cells[Convert.ToInt32(pergunta_x_opcao.sOpcoes.Split('|')[0]) + 3].Controls[0] as CheckBox).Checked = true;
                            else
                                (e.Row.Cells[3].Controls[0] as CheckBox).Checked = true;
                        }
                        catch
                        {
                            (e.Row.Cells[3].Controls[0] as CheckBox).Checked = true;
                        }
                    }
                }
                else
                    (e.Row.Cells[3].Controls[0] as CheckBox).Checked = true;

                (gv.Controls[0].Controls[0] as GridViewRow).Cells[0].Text = gv.DataKeys[0]["sDscCategoria"].ToString();

                e.Row.Cells[2].Text = HttpUtility.HtmlDecode(pergunta);
            }
        }

        public void ValidaFluxoART(string idFluxo)
        {
            DataSet dsValido = new DataSet();
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "VALIDAR-FLUXO-ART" },
                { "@idFluxo", idFluxo}
            };

            dsValido = ExecutarDataSet(sProcedure, vParametros);

            if (ValidarDataSet(dsValido))
            {
                if (hddidTipo.Value == "2")
                    Div_ContratoVendas.Visible = true;
                else
                    Div_ContratoVendas.Visible = false;
            }
            else
            {
                Div_ContratoVendas.Visible = false;
            }
        }

        #endregion

        #region | Aba Cronograma

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            if (ValidarCronograma())
            {
                DateTime dtFinal = DateTime.Parse(txtdtFinal.Text);
                DateTime dtInicio = DateTime.Parse(txtdtInicio.Text);
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "SALVAR_EVENTO" },
                    { "@idUsuario", ddlidColaboradorOS.SelectedValue },
                    { "@idTipo", "1" },
                    { "@dtFinal", dtFinal.ToString("dd/MM/yyyy HH:mm") },
                    { "@dtInicial", dtInicio.ToString("dd/MM/yyyy HH:mm") },
                    { "@sDscEvento", txtsObservacaoOS.Text },
                    { "@sAtivo", "S" },
                    { "@sUrl", HttpContext.Current.Request.RawUrl },
                    { "@sDiaInteiro", "N" },
                    { "@sDscTitulo", "Cliente: "+ txtsRazaoSocial.Text +" - Pedido: "+ txtnControleTT.Text },
                    { "@idUsuarioAtualizacao", Variaveis.idUsuario() },
                    { "@idFiltro", hddidPedido.Value }
                };
                DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Calendario_Eventos_x_Usuario", vParametros);
                LimparCronograma();
            }
            PesquisarPedido(hddidPedido.Value, "", "0", "", "");
        }

        void LimparCronograma()
        {
            txtdtInicio.Text = "";
            txtdtFinal.Text = "";
            txtsObservacaoOS.Text = "";
            ddlidColaboradorOS.SelectedValue = "0";
        }

        private bool ValidarCronograma()
        {
            string sMensagem = "";
            if (txtdtInicio.Text == "")
            {
                sMensagem = "Descreva a data de Início!";
            }
            if (txtdtFinal.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva a data Final!";
            }
            if (ddlidColaboradorOS.SelectedValue == "0")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Colaborador!";
            }
            if (sMensagem != "")
            {
                MensagemPagina8.MostraMensagem_Erro(sMensagem);
                return false;
            }
            return true;
        }

        protected void AtualizaCalendario()
        {
            DIV_DataCronograma.Visible = false;
            DIV51.Visible = false;
            DIV52.Visible = false;
            DIV_btnInserir.Visible = false;
            DIV56.Visible = false;

            Dictionary<string, string> vParametrosINCLUIR = new Dictionary<string, string>
            {
                { "@sFuncao", "INCLUIR_CRONOGRAMA" },
                { "@idPedido", hddidPedido.Value },
                { "@idUsuarioInclusao", Variaveis.idUsuario() }
            };
            DataSet dsINCLUIR = ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametrosINCLUIR);

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@idUsuario", Variaveis.idUsuario() },
                { "@idTipo", "1" },
                { "@idFiltro", hddidPedido.Value }
            };
            DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Calendario_Eventos_x_Usuario", vParametros);

            gvCronograma.DataSource = ds;
            gvCronograma.DataBind();

            UpdatePanel5.Update();
            Calendario.RegistraScriptViwer(ds);
        }

        #endregion

        #region | Aba Faturamento

        #region | Faturamentos Efetuados

        void FaturamentosEfetuados()
        {
            div_FaturamentosEfetuados.Visible = false;

            try
            {
                DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Receber", new Dictionary<string, string> { { "@sFuncao", "FATURAMENTO" }, { "@idPedido", hddidPedido.Value } });

                if (ds.Tables[0].Rows.Count > 0)
                {
                    div_FaturamentosEfetuados.Visible = true;

                    gvFaturamentosEfetuados.DataSource = ds.Tables[0];
                    gvFaturamentosEfetuados.DataBind();
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Faturamento.MostraMensagem_Erro(ex.Message);
            }
        }

        protected void gvFaturamentosEfetuados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton cmdDownloadDANFE = e.Row.FindControl("cmdDownloadDANFE") as LinkButton;
                LinkButton cmdDownloadXML = e.Row.FindControl("cmdDownloadXML") as LinkButton;
                LinkButton cmdGerarPDFEspelho = e.Row.FindControl("cmdGerarPDFEspelho") as LinkButton;
                LinkButton cmdGerarTXT = e.Row.FindControl("cmdGerarTXT") as LinkButton;

                cmdDownloadDANFE.Visible = false;
                cmdDownloadXML.Visible = false;
                cmdGerarPDFEspelho.Visible = false;
                cmdGerarTXT.Visible = false;

                if (e.Row.Cells[nColuna_sTipo].Text == "NFS-e")
                {
                    cmdGerarPDFEspelho.CommandArgument = DataBinder.Eval(e.Row.DataItem, "idArquivoPDF").ToString();
                    cmdGerarTXT.CommandArgument = DataBinder.Eval(e.Row.DataItem, "idArquivo").ToString();
                }

                switch (e.Row.Cells[nColuna_Status].Text)
                {
                    case "Espelho":
                        if (e.Row.Cells[nColuna_sTipo].Text == "NFS-e")
                        {
                            cmdGerarPDFEspelho.Visible = true;
                            cmdGerarTXT.Visible = true;
                        }
                        else
                        {
                            cmdDownloadXML.Visible = true;
                            cmdDownloadDANFE.Visible = true;
                            cmdDownloadDANFE.Text = "Espelho";
                        }
                        break;

                    case "Autorizada":
                        if (e.Row.Cells[nColuna_sTipo].Text == "NF-e")
                        {
                            cmdDownloadXML.Visible = true;
                            cmdDownloadDANFE.Visible = true;
                        }
                        else if (e.Row.Cells[nColuna_sTipo].Text == "NFD-e")
                        {
                            cmdDownloadXML.Visible = true;
                            cmdDownloadDANFE.Visible = true;
                        }
                        else if (e.Row.Cells[nColuna_sTipo].Text == "NFS-e")
                            cmdGerarTXT.Visible = true;
                        break;

                    case "Em processamento":
                        if (e.Row.Cells[nColuna_sTipo].Text == "NF-e") cmdDownloadXML.Visible = true;
                        else if (e.Row.Cells[nColuna_sTipo].Text == "NFS-e") cmdGerarTXT.Visible = true;
                        break;

                    case "Rejeitada":
                        if (e.Row.Cells[nColuna_sTipo].Text == "NFS-e") cmdGerarTXT.Visible = true;
                        break;

                    case "Aguardando DANFE":
                        if (e.Row.Cells[nColuna_sTipo].Text == "NFS-e") cmdGerarTXT.Visible = true;
                        else cmdDownloadXML.Visible = true;
                        break;

                    case "Cancelada":
                        if (e.Row.Cells[nColuna_sTipo].Text == "NFS-e") cmdGerarTXT.Visible = true;
                        else
                        {
                            cmdDownloadXML.Visible = true;
                            cmdDownloadDANFE.Visible = true;
                        }
                        break;
                }

                var btnToggle = e.Row.FindControl("btnToggle") as LinkButton;
                if (e.Row.FindControl("gv_ContasReceber") is GridView gv_ContasReceber)
                {
                    Dictionary<string, string> vParam = new Dictionary<string, string>
                    {
                        { "@sFuncao", "FATURAMENTO_DETALHE" },
                        { "@idPedido", hddidPedido.Value },
                        { "@idRegistro", (e.Row.FindControl("lblidXML") as Label).Text }
                    };
                    DataTable tb = ExecutarDataTable("sp_Manipula_tbl_Flow_Adm_Contas_Receber", vParam);

                    gv_ContasReceber.DataSource = tb;
                    gv_ContasReceber.DataBind();

                    btnToggle.Visible = tb.Rows.Count > 0;
                }
                else btnToggle.Visible = false;
            }
        }

        protected void gv_ContasReceber_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();

            EsconderColunas(e, 3, 4, 9, 10, 11, 15);
        }

        protected void cmdDownloadXML_Click(object sender, EventArgs e)
        {
            Funcoes_NFe.Download.XML(Page, (((sender as LinkButton).NamingContainer as GridViewRow).FindControl("lblidXML") as Label).Text);
            Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
        }

        protected void cmdDownloadDANFE_Click(object sender, EventArgs e)
        {
            Funcoes_NFe.Download.PDF(Page, (((sender as LinkButton).NamingContainer as GridViewRow).FindControl("lblidXML") as Label).Text);
            Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
        }

        protected void cmdGerarPDFEspelho_Click(object sender, EventArgs e)
        {
            string sNomeArquivo = "";
            byte[] bObjArquivo = null;

            Dictionary<string, string> vParametrosItem = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idArquivo", (sender as LinkButton).CommandArgument }
            };
            DataTable dtArquivo = ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

            foreach (DataRow item in dtArquivo.Rows)
            {
                try
                {
                    sNomeArquivo = item["sNomeArquivo"].ToString();
                    bObjArquivo = (byte[])item["vbArquivo"];
                }
                catch { }

                FileStream lObjFile = new Arquivo().TransformarArrayBytesEmArquivo(bObjArquivo, Server.MapPath("~/Download/" + sNomeArquivo));
                lObjFile.Close();
                lObjFile.Dispose();

                DownloadArquivo(Page, sNomeArquivo);
            }

            Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
        }

        protected void cmdGerarTXT_Click(object sender, EventArgs e)
        {
            string sNomeArquivo = "";
            byte[] bObjArquivo = null;

            Dictionary<string, string> vParametrosItem = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idArquivo", (sender as LinkButton).CommandArgument }
            };
            DataTable dtArquivo = ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

            foreach (DataRow item in dtArquivo.Rows)
            {
                try
                {
                    sNomeArquivo = item["sNomeArquivo"].ToString();
                    bObjArquivo = (byte[])item["vbArquivo"];
                }
                catch { }

                FileStream lObjFile = new Arquivo().TransformarArrayBytesEmArquivo(bObjArquivo, Server.MapPath("~/Download/" + sNomeArquivo));
                lObjFile.Close();
                lObjFile.Dispose();

                DownloadArquivo(Page, sNomeArquivo);
            }

            Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
        }

        #endregion

        protected void PopularFaturamento(DataSet ds)
        {
            Base_Faturamento.Clear();

            lblPanel_gvFaturamento.SetAttribute("class", "col-lg-9");
            div_txtSaldoParcial.Visible = false;
            div_cmdFaturamento.Visible = false;
            div_cmdSalvarFaturamento.Visible = false;

            cmdFaturamento.NavigateUrl = $"/App/Paginas/Adm/Faturamento/Faturamento.aspx?idPedido={hddidPedido.Value}";

            if (ds.Tables[nTabela_Faturamento].Rows.Count > 0)
            {
                decimal nTotal = decimal.Parse(txtTotal_Pedido.Text.Trim());
                decimal nTotal_Faturado = 0, nTotal_Parcial = 0; //, nPorcentagemTotal = 0;

                foreach (DataRow linha in ds.Tables[nTabela_Faturamento].Rows)
                {
                    decimal nPorcentagem = decimal.Parse(linha["nPorcentagem"].ToString());
                    decimal nVlrFaturamento = decimal.Parse(linha["vlrFaturamento"].ToString());
                    string sFaturado = linha["sFaturado"].ToString();

                    //nPorcentagemTotal += nPorcentagem;
                    nTotal_Parcial += nVlrFaturamento;
                    nTotal_Faturado += sFaturado == "S" ? nVlrFaturamento : 0;

                    cls_Pedidos_Faturamento objItem = new cls_Pedidos_Faturamento
                    {
                        sFuncao = "FATURAMENTO",
                        idContador = Base_Faturamento.Count + 1,
                        idFaturamento = int.Parse(linha["idFaturamento"].ToString()),
                        idServico = int.Parse(linha["idItem_Servico"].ToString()),
                        idEmpresa = int.Parse(linha["idEmpresa"].ToString()),
                        sDscFaturamento = linha["sDscFaturamento"].ToString(),
                        sDscServico = linha["sDscServico"].ToString(),
                        sDscEmpresa = linha["sDscEmpresa"].ToString(),
                        sBase_Porcentagem = linha["sBase_Porcentagem"].ToString(),
                        nPorcentagem = nPorcentagem,
                        nValor = nVlrFaturamento,
                        dtFaturamento = Convert.ToDateTime(linha["dtFaturamento"]).ToString("dd/MM/yyyy"),
                        sFaturado = sFaturado,
                        sCor = linha["sCor"].ToString(),
                        idTipo = int.Parse(linha["idTipo"].ToString()),
                        sDscTipo = linha["sTipoFaturamento"].ToString(),
                        idDepartamento = int.Parse(linha["idDepartamento"].ToString()),
                        sDscDepartamento = linha["sDscDepartamento"].ToString()
                    };
                    Base_Faturamento.Add(objItem);
                }

                txtTotal_Faturado.Text = nTotal_Faturado.ToString("N2");
                txtSaldo_Faturar.Text = (nTotal - nTotal_Faturado).ToString("N2");

                //txtPorcentagemParcial.Text = nPorcentagemTotal.ToString("N4");
                txtTotalParcial.Text = nTotal_Parcial.ToString("N2");

                div_IncluirFaturamento.Visible = nTotal_Parcial < nTotal;
                div_cmdFaturamento.Visible = !div_IncluirFaturamento.Visible;
            }

            div_FaturaServicos.Visible = DATASET(ds, "sPossuiServico").Equals("S");

            LimpaCampos_IncluirFaturamento();
            gvFaturamento_DataBind();
        }

        protected void gvFaturamento_DataBind()
        {
            if (hddsPossuiServicos.Value.Equals("N"))
            {
                gvFaturamento.Columns[Col_Faturamento_Servico].Visible = false;
                gvFaturamento.Columns[Col_Faturamento_Empresa].Visible = false;
            }

            txtVlrFaturamento.Text = txtnVlrTotal.Text;

            var edita = Base_Faturamento.FirstOrDefault(f => f.sFuncao == "EDITAR_FATURAMENTO");
            if (edita != null) edita.sFuncao = "EXCLUIR_FATURAMENTO";

            gvFaturamento.DataSource = Base_Faturamento.Where(f => f.sFuncao != "EXCLUIR_FATURAMENTO");
            gvFaturamento.DataBind();

            if (edita != null && edita is cls_Pedidos_Faturamento faturamento)
            {
                ddlServico.SelectedValue = faturamento.idServico.ToString();
                ddlEmpresa_Faturamento.SelectedValue = faturamento.idEmpresa.ToString();
                txtdtFaturamento.Text = Convert.ToDateTime(faturamento.dtFaturamento).ToString("yyyy-MM-dd");
                ddlTipoFaturamento.SelectedValue = faturamento.idTipo.ToString();
                ddlDepartamento.SelectedValue = faturamento.idDepartamento.ToString();
                txtsDescricao.Text = faturamento.sDscFaturamento;
                txtVlrFaturamento.Text = faturamento.nValor.ToString("N2");

                faturamento.sCor = "info";

                div_DeptoFaturamento.Visible = faturamento.idDepartamento > 0;
            }
            else LimpaCampos_IncluirFaturamento(false);

            //var faturamentos = Base_Faturamento.Where(f => f.sFuncao != "EXCLUIR_FATURAMENTO");
            //decimal totalV = faturamentos.Sum(f => f.nValor);
            //decimal totalP = faturamentos.Sum(f => f.nPorcentagem);
            //if (totalV.ToString("N2") == txtTotal_Pedido.Text && totalP != 100m)
            //{
            //    Base_Faturamento.OrderBy(f => f.idContador).Last().nPorcentagem += 100m - totalP;
            //    txtPorcentagemParcial.Text = "100,0000";
            //    gvFaturamento_DataBind();
            //}

            txtSaldoParcial.Text = (decimal.Parse(txtTotal_Pedido.Text) - decimal.Parse(txtTotalParcial.Text)).ToString("N2");

            txtVlrFaturamento_TextChanged(null, null);

            div_cmdSalvarFaturamento.Visible = gvFaturamento.Rows.Count > 0 && !cmdFaturamento.Visible;
        }

        protected void LimpaCampos_IncluirFaturamento(bool bLimpaValores = true)
        {
            div_DeptoFaturamento.Visible = false;

            txtdtFaturamento.Text = "";
            ddlServico.SelectedValue = "0";
            ddlEmpresa_Faturamento.SelectedValue = "0";
            ddlTipoFaturamento.SelectedValue = "0";
            txtsDescricao.Text = "";
            ddlDepartamento.SelectedValue = "0";
            txtParcelas_Faturamento.Text = "1";

            if (bLimpaValores)
            {
                txtPorcentagem_Faturamento.Text = "";
                txtVlrFaturamento.Text = "";
            }

            div_ddlTipoFaturamento.Attributes["class"] = "col-lg-3 form-group";
        }

        protected bool ValidarFaturamento()
        {
            string Mensagem = "";

            if (hddsPossuiProdutos.Value.Equals("N"))
            {
                if (ddlServico.SelectedValue == "0") Mensagem += $"{(Mensagem != "" ? "</br>" : "")}É obrigatório selecionar um Serviço!";
                if (ddlEmpresa_Faturamento.SelectedValue == "0") Mensagem += $"{(Mensagem != "" ? "</br>" : "")}É obrigatório selecionar uma Empresa!";
            }

            if (txtsDescricao.Text == "") Mensagem += $"{(Mensagem != "" ? "</br>" : "")}É obrigatório preencher a Descrição!";
            if (txtdtFaturamento.Text == "") Mensagem += $"{(Mensagem != "" ? "</br>" : "")}É obrigatório preencher a Data!";
            if (ddlTipoFaturamento.SelectedValue == "0") Mensagem += $"{(Mensagem != "" ? "</br>" : "")}É obrigatório preencher o Tipo de Faturamento!";
            if (txtPorcentagem_Faturamento.Text == "") Mensagem += $"{(Mensagem != "" ? "</br>" : "")}É obrigatório preencher a Porcentagem!";
            if (txtVlrFaturamento.Text == "") Mensagem += $"{(Mensagem != "" ? "</br>" : "")}É obrigatório preencher o Valor!";

            if (Mensagem != "")
            {
                MensagemPagina_Faturamento.MostraMensagem_Erro(Mensagem);
                return false;
            }

            return true;
        }

        protected string RetornaBaseCalculo_Faturamento()
        {
            string sBase = txtnVlrTotal.Text.Trim();
            switch (ddlBase_Porcentagem.SelectedValue)
            {
                case "S": sBase = txtnVlrServicos.Text.Trim(); break;
                case "SS":
                    if (ddlServico.SelectedValue == "0")
                    {
                        ddlBase_Porcentagem.SelectedValue = "T";
                        throw new Exception("É necessário selecionar um Serviço para utilizar o 'Serviço selecionado' como Base!");
                    }
                    else
                        sBase = Base_Pedidos_Itens.First(i => i.idItem.ToString() == ddlServico.SelectedValue).nValorTotal.ToString("N2");
                    break;
                case "P": sBase = txtnVlrProdutos.Text.Trim(); break;
            }

            return sBase;
        }

        protected void gvFaturamento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                bool bFaturado = DataBinder.Eval(e.Row.DataItem, "sFaturado").ToString() == "S";
                if (e.Row.FindControl("cmdEdita_Faturamento") is LinkButton editar) editar.Visible = !bFaturado;
                if (e.Row.FindControl("cmdExclui_Faturamento") is LinkButton excluir) excluir.Visible = !bFaturado;
                if (e.Row.FindControl("cmdImportarNota_Faturamento") is LinkButton importar) importar.Visible = !bFaturado;
                if (e.Row.FindControl("chkExcluir") is CheckBox chkExclui) chkExclui.Visible = !bFaturado;

                string sBase = "Total do Pedido";
                switch (DataBinder.Eval(e.Row.DataItem, "sBase_Porcentagem"))
                {
                    case "S": sBase = "Total dos Serviços"; break;
                    case "SS": sBase = "Total do Serviço selecionado"; break;
                    case "P": sBase = "Total dos Produtos"; break;
                }
                e.Row.Cells[Col_Faturamento_Porcentagem].Text = $"{decimal.Parse(DataBinder.Eval(e.Row.DataItem, "nPorcentagem").ToString()):N4} %<br />do {sBase}";

                e.Row.Cells[Col_Faturamento_Tipo].Text += e.Row.Cells[Col_Faturamento_Tipo].Text.Contains("Tarefas") ? $" - {DataBinder.Eval(e.Row.DataItem, "sDscDepartamento")}" : "";
                e.Row.Cells[Col_Faturamento_Faturado].Text = e.Row.Cells[Col_Faturamento_Faturado].Text == "S" ? "✔" : "";
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();

                txtVlrFaturamento.Text = (decimal.Parse(txtVlrFaturamento.Text) - decimal.Parse(DataBinder.Eval(e.Row.DataItem, "nValor").ToString())).ToString("N2");
            }
        }

        protected void gvFaturamento_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToString() != "Importar")
            {
                var faturamento = Base_Faturamento.FirstOrDefault(f => f.idContador.ToString() == e.CommandArgument.ToString());
                faturamento.sFuncao = "EDITAR_FATURAMENTO";

                foreach (var item in Base_Faturamento.Where(f => f.idContador != faturamento.idContador && f.sCor == "info"))
                {
                    item.sFuncao = "INCLUIR_FATURAMENTO";
                    item.sCor = "";
                }

                //txtPorcentagemParcial.Text = (decimal.Parse(txtPorcentagemParcial.Text) - faturamento.nPorcentagem).ToString("N4");
                txtTotalParcial.Text = (decimal.Parse(txtTotalParcial.Text) - faturamento.nValor).ToString("N2");

                gvFaturamento_DataBind();

                lblPanel_gvFaturamento.SetAttribute("class", "col-lg-8");
                div_txtSaldoParcial.Visible = true;
                div_IncluirFaturamento.Visible = true;
                div_cmdSalvarFaturamento.Visible = true;
                div_cmdFaturamento.Visible = false;
            }
            else
            {
                Session["idContador"] = e.CommandArgument.ToString();
                Scripts.AbrirModal(Page, "modalImportarNota");
            }
        }

        protected void btnIncluirFaturamento_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidarFaturamento())
                {
                    decimal valorAtual = 0, vlrfaturamento = 0;

                    foreach (var Linha in Base_Faturamento)
                    {
                        if (Linha.sFuncao != "EXCLUIR_FATURAMENTO")
                            valorAtual += Linha.nValor;
                    }

                    decimal.TryParse(txtPorcentagem_Faturamento.Text.Trim(), out decimal porcentagem);
                    decimal.TryParse(txtVlrFaturamento.Text.Trim(), out decimal valor);
                    decimal.TryParse(txtTotal_Pedido.Text.Trim(), out decimal totalPedido);

                    if (valorAtual < totalPedido)
                    {
                        if (hddsPossuiServicos.Value.Equals("N"))
                        {
                            ddlServico.SelectedValue = "0";
                            ddlEmpresa_Faturamento.SelectedValue = "0";
                        }

                        string servico = "", empresa = "", departamento = "";
                        if (ddlServico.SelectedValue != "0") servico = ddlServico.SelectedItem.Text;
                        if (ddlEmpresa_Faturamento.SelectedValue != "0") empresa = ddlEmpresa_Faturamento.SelectedItem.Text;
                        if (ddlDepartamento.SelectedValue != "0") departamento = ddlDepartamento.SelectedItem.Text;

                        int.TryParse(txtParcelas_Faturamento.Text, out int nParcelas);
                        if (nParcelas > 1)
                        {
                            for (int i = 1; i <= nParcelas; i++)
                            {
                                decimal parcela = Math.Round(valor / nParcelas, 2);
                                if (i == nParcelas && (parcela * nParcelas) != valor)
                                    parcela = valor - (parcela * (nParcelas - 1));

                                cls_Pedidos_Faturamento objItem = new cls_Pedidos_Faturamento
                                {
                                    sFuncao = "INCLUIR_FATURAMENTO",
                                    idContador = Base_Faturamento.Count + 1,
                                    idFaturamento = 0,
                                    sDscFaturamento = txtsDescricao.Text.Trim(),
                                    idServico = int.Parse(ddlServico.SelectedValue),
                                    sDscServico = servico,
                                    idEmpresa = int.Parse(ddlEmpresa_Faturamento.SelectedValue),
                                    sDscEmpresa = empresa,
                                    sBase_Porcentagem = ddlBase_Porcentagem.SelectedValue,
                                    nPorcentagem = porcentagem / nParcelas,
                                    nValor = parcela,
                                    dtFaturamento = Convert.ToDateTime(txtdtFaturamento.Text).ToString("dd/MM/yyyy"),
                                    sFaturado = "N",
                                    sCor = "",
                                    idTipo = int.Parse(ddlTipoFaturamento.SelectedValue),
                                    sDscTipo = ddlTipoFaturamento.SelectedItem.Text,
                                    idDepartamento = int.Parse(ddlDepartamento.SelectedValue),
                                    sDscDepartamento = departamento
                                };
                                Base_Faturamento.Add(objItem);
                            }
                        }
                        else
                        {
                            cls_Pedidos_Faturamento objItem = new cls_Pedidos_Faturamento
                            {
                                sFuncao = "INCLUIR_FATURAMENTO",
                                idContador = Base_Faturamento.Count + 1,
                                idFaturamento = 0,
                                sDscFaturamento = txtsDescricao.Text.Trim(),
                                idServico = int.Parse(ddlServico.SelectedValue),
                                sDscServico = servico,
                                idEmpresa = int.Parse(ddlEmpresa_Faturamento.SelectedValue),
                                sDscEmpresa = empresa,
                                sBase_Porcentagem = ddlBase_Porcentagem.SelectedValue,
                                nPorcentagem = porcentagem,
                                nValor = valor,
                                dtFaturamento = Convert.ToDateTime(txtdtFaturamento.Text).ToString("dd/MM/yyyy"),
                                sFaturado = "N",
                                sCor = "",
                                idTipo = int.Parse(ddlTipoFaturamento.SelectedValue),
                                sDscTipo = ddlTipoFaturamento.SelectedItem.Text,
                                idDepartamento = int.Parse(ddlDepartamento.SelectedValue),
                                sDscDepartamento = departamento
                            };
                            Base_Faturamento.Add(objItem);
                        }

                        foreach (var linha in Base_Faturamento)
                        {
                            if (linha.sFuncao != "EXCLUIR_FATURAMENTO")
                                vlrfaturamento += linha.nValor;
                        }

                        vlrfaturamento = 0;
                        decimal vlrParcial = 0;
                        foreach (var linha in Base_Faturamento)
                        {
                            if (linha.sFuncao != "EXCLUIR_FATURAMENTO")
                            {
                                vlrParcial += linha.nValor;

                                if (linha.sFaturado == "S")
                                    vlrfaturamento += linha.nValor;
                            }
                        }

                        txtTotal_Faturado.Text = vlrfaturamento.ToString("N2");
                        txtSaldo_Faturar.Text = (totalPedido - vlrfaturamento).ToString("N2");
                        txtTotalParcial.Text = vlrParcial.ToString("N2");

                        LimpaCampos_IncluirFaturamento();
                        gvFaturamento_DataBind();

                        cmdSalvarFaturamento.Visible = true;
                        div_cmdSalvarFaturamento.Visible = true;
                        div_cmdFaturamento.Visible = true;

                        Scripts.FocusScript(Page, txtdtFaturamento.ClientID);
                    }
                    else MensagemPagina_Faturamento.MostraMensagem_Erro("Não é possível adicionar Faturamentos de forma que ultrapassem o Valor Total do Pedido!");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Faturamento.MostraMensagem_Erro(ex.Message);
            }
        }

        protected void cmdSalvarFaturamento_Click(object sender, EventArgs e)
        {
            try
            {
                if (decimal.Parse(txtTotalParcial.Text) != decimal.Parse(txtTotal_Pedido.Text))
                {
                    MensagemPagina_Faturamento.MostraMensagem_Erro("É necessário que o <b>Valor Configurado</b> seja equivalente ao <b>Total do Pedido</b>!");
                    return;
                }

                foreach (var Linha in Base_Faturamento)
                {
                    if (Linha.sFuncao != "FATURAMENTO")
                    {
                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", Linha.sFuncao },
                            { "@idPedido", hddidPedido.Value },
                            { "@idFaturamento", Linha.idFaturamento.ToString() },
                            { "@sDscFaturamento", Linha.sDscFaturamento },
                            { "@idItem_Servico", Linha.idServico.ToString() },
                            { "@idEmpresa", Linha.idEmpresa.ToString() },
                            { "@nPorcentagem", Linha.nPorcentagem.ToString().StringToDecimalString() },
                            { "@vlrFaturamento", Linha.nValor.ToString().StringToDecimalString() },
                            { "@sBase_Porcentagem", Linha.sBase_Porcentagem },
                            { "@idUsuario", Variaveis.idUsuario() },
                            { "@dtFaturamento", Linha.dtFaturamento },
                            { "@idTipo", Linha.idTipo.ToString() },
                            { "@idDepartamento", Linha.idDepartamento.ToString() }
                        };
                        ExecutarDataSet(sProcedure, vParametros);
                    }
                }

                lblPanel_gvFaturamento.SetAttribute("class", "col-lg-9");
                div_txtSaldoParcial.Visible = false;

                PesquisarPedido(hddidPedido.Value, "", "0", "", "");
                MensagemPagina_Faturamento.MostraMensagem_Sucesso("Faturamento incluído com sucesso!");
            }
            catch (Exception ex)
            {
                MensagemPagina_Faturamento.MostraMensagem_Erro(ex.Message);
            }
        }

        protected void txtPorcentagem_Faturamento_TextChanged(object sender, EventArgs e)
        {
            string sBase;
            try { sBase = RetornaBaseCalculo_Faturamento(); }
            catch (Exception ex)
            {
                MensagemPagina_Faturamento.MostraMensagem_Erro(ex.Message);
                return;
            }

            decimal valorBase = decimal.Parse(sBase);
            if (decimal.TryParse(txtPorcentagem_Faturamento.Text.Trim(), out decimal porcentagem) && porcentagem > 0)
                txtVlrFaturamento.Text = (valorBase * porcentagem / 100).ToString("N2");
            else
                txtVlrFaturamento.Text = "";
        }

        protected void txtVlrFaturamento_TextChanged(object sender, EventArgs e)
        {
            string sBase;
            try { sBase = RetornaBaseCalculo_Faturamento(); }
            catch (Exception ex)
            {
                MensagemPagina_Faturamento.MostraMensagem_Erro(ex.Message);
                return;
            }

            decimal valorBase = decimal.Parse(sBase);
            if (decimal.TryParse(txtVlrFaturamento.Text.Trim(), out decimal valor) && valor > 0)
                txtPorcentagem_Faturamento.Text = (valor / valorBase * 100).ToString("N4");
            else
                txtPorcentagem_Faturamento.Text = "";
        }

        protected void ddlTipoFaturamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTipoFaturamento.SelectedValue == "2")
            {
                div_DeptoFaturamento.Visible = true;
                div_ddlTipoFaturamento.Attributes["class"] = "col-lg-4 form-group";
            }
            else
            {
                div_DeptoFaturamento.Visible = false;
                ddlDepartamento.SelectedValue = "0";
                div_ddlTipoFaturamento.Attributes["class"] = "col-lg-3 form-group";
            }
        }

        protected void ddlServico_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = Base_Pedidos_Itens.FirstOrDefault(i => i.idItem.ToString() == ddlServico.SelectedValue);

            if (item != null)
            {
                txtVlrFaturamento.Text = (item.nValorTotal.DoubleToDecimal() - Base_Faturamento.Where(f => f.sFuncao != "EXCLUIR_FATURAMENTO" && f.idServico.ToString() == ddlServico.SelectedValue).Sum(f => f.nValor)).ToString("N2");
                txtVlrFaturamento_TextChanged(null, null);
            }
        }

        protected void ddlBase_Porcentagem_SelectedIndexChanged(object sender, EventArgs e) => txtPorcentagem_Faturamento_TextChanged(null, null);

        protected void cmdExcluir_Faturamento_Click(object sender, EventArgs e)
        {
            decimal nTotal = 0; //, nPorcentagem = 0;

            foreach (GridViewRow row in gvFaturamento.Rows)
            {
                if ((row.FindControl("chkExcluir") as CheckBox).Checked)
                {
                    var faturamento = Base_Faturamento.FirstOrDefault(f => f.idContador.ToString() == row.Cells[0].Text);
                    faturamento.sFuncao = "EXCLUIR_FATURAMENTO";

                    //nPorcentagem += faturamento.nPorcentagem;
                    nTotal += faturamento.nValor;
                }
            }

            //txtPorcentagemParcial.Text = (decimal.Parse(txtPorcentagemParcial.Text) - nPorcentagem).ToString("N4");
            txtTotalParcial.Text = (decimal.Parse(txtTotalParcial.Text) - nTotal).ToString("N2");

            gvFaturamento_DataBind();

            lblPanel_gvFaturamento.SetAttribute("class", "col-lg-8");
            div_txtSaldoParcial.Visible = true;
            div_IncluirFaturamento.Visible = true;
            div_cmdSalvarFaturamento.Visible = true;
            div_cmdFaturamento.Visible = false;
        }

        protected void btnImportarNota_Click(object sender, EventArgs e)
        {
            if (fu_ImportarNota.HasFile)
            {
                string fileName = fu_ImportarNota.FileName;
                string idContador = Session["idContador"].ToString();

                if (Path.GetExtension(fileName).ToLower() != ".pdf")
                {
                    MensagemPaginaModalImportar.MostraMensagem_Erro("Formato de arquivo inválido. Por favor, envie um arquivo .pdf");
                    Scripts.AbrirModal(Page, "modalImportarNota");
                    return;
                }

                try
                {
                    byte[] fileBytes = fu_ImportarNota.FileBytes;
                    var faturamento = Base_Faturamento.FirstOrDefault(f => f.idContador.ToString() == idContador);

                    cls_Arquivos Arquivo = new cls_Arquivos
                    {
                        idTipoArquivo = 10013,
                        idObjeto = faturamento.idFaturamento,
                        sNomeArquivo = fileName,
                        sDscArquivo = fileName,
                        sObservacao = "",
                        idUsuario = Convert.ToInt32(Variaveis.idUsuario()),
                        vbArquivo = fileBytes,
                        dtExpiracaoDoc = "",
                        dtRegistroDoc = DateTime.Now.ToString()
                    };

                    Arquivo.EnviarArquivo(Arquivo);

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "Importar_Nota" },
                        { "@idFaturamento", faturamento.idFaturamento.ToString() },
                        { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                    };
                    DataSet ds = ExecutarDataSet(sProcedure, vParametros);
                    if (ValidarDataSet(ds, out _))
                        PesquisarPedido(hddidPedido.Value, "", "0", "", "");
                }
                catch
                {
                    MensagemPaginaModalImportar.MostraMensagem_Erro("Houve um erro na importação do arquivo! Não foi possível ler o arquivo", false);
                    Scripts.AbrirModal(Page, "modalImportarNota");
                }
            }
            else
            {
                MensagemPaginaModalImportar.MostraMensagem_Erro("Insira um arquivo");
                Scripts.AbrirModal(Page, "modalImportarNota");
            }

            ScriptManager.RegisterStartupScript(Page, GetType(), "js_MantemAtiva_faturamento", " $('#Faturamento-tab').tab('show');", true);
        }

        #endregion

        #region | Modal

        #region | Modal - Composição de Serviços

        //------------------- Higor Maestrello 26/07/2024 ----------------------
        protected void gvServicos_Recursos_Composicao_1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            EsconderColunas(gvServicos_Recursos_Composicao_1, "nOrdem");
            EsconderColunas(gvServicos_Recursos_Composicao_1, "Item");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string id = (e.Row.FindControl("lblidItem") as Label).Text;
                DataSet dsPesquisa;
                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTA_COMPOSICAO" },
                    { "@iditem", id }
                };

                dsPesquisa = ExecutarDataSet(sProcedure, vParametros);
                LinkButton lnkComposicaoServico = e.Row.FindControl("lnkComposicaoServico") as LinkButton;
                lnkComposicaoServico.Visible = false;
                if (ValidarDataSet(dsPesquisa))
                {
                    if (click < 2)
                        lnkComposicaoServico.Visible = true;
                }
            }
        }

        void ComposicaoServico(string id, string sDscProduto)
        {
            DataSet dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA_COMPOSICAO" },
                { "@iditem", id }
            };

            dsPesquisa = ExecutarDataSet(sProcedure, vParametros);
            h4_titleModalComposicao_1.InnerText = "Composição do Serviço:" + sDscProduto;
            gvServicos_Recursos_Composicao_1.DataSource = dsPesquisa;
            gvServicos_Recursos_Composicao_1.DataBind();
        }

        protected void cmdFecharModal_Click(object sender, EventArgs e)
        {
            click = 0;
            if (cmdEditar.Visible)
            {
                DataBind_dtgItens("");
            }
            else
            {
                DataBind_dtgItens("Editar");
            }
        }
        //---------------------------------------------------------------------

        #endregion

        #region | Modal - Itens Fornecedores Tabela de Preço

        protected void btn_ItensFornecedor_Click(object sender, EventArgs e)
        {

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_OpenModalItens", "$('#Modal_Itens_Tabela').modal('show');", true);

            string list = ",";

            foreach (GridViewRow row in dtgItens.Rows)
            {
                list += dtgItens.DataKeys[row.DataItemIndex]["idProduto"].ToString() + ",";
            }

            Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTA_TABELA_PRECO" },
                    { "@sidParceiro", ddlFornecedor.SelectedValue },
                    { "@sidExcluirItem", list }
                };
            DataTable dsPesquisa = ExecutarDataTable(sProcedure, vParametros);

            btnInserirItem.Visible = true;

            if (dsPesquisa.Rows.Count > 0)
            {
                gvItens.Visible = true;

                bs_Itens_Tabela.Clear();
                foreach (DataRow row in dsPesquisa.Rows)
                {
                    cls_Itens_Tabela objItem = new cls_Itens_Tabela();

                    objItem.idItem = Convert.ToInt32(row["idItem"].ToString());
                    objItem.idNCM = Convert.ToInt32(row["idNCM"].ToString());
                    objItem.sCodigo = (row["sCodigo"].ToString());
                    objItem.sDscProduto = (row["sDscProduto"].ToString());
                    objItem.sUnidade = (row["sUnidade"].ToString());
                    objItem.nTotal = decimal.Parse((row["nTotal"].ToString()));
                    objItem.sUnidadeEntrega = (row["sUnidadeEntrega"].ToString());
                    bs_Itens_Tabela.Add(objItem);
                }
                int.TryParse(ddl_nItens.SelectedValue, out int nItens);
                gvItens.PageSize = nItens;
                gvItens.VirtualItemCount = bs_Itens_Tabela.Count;

                ddlOrdemTabela.SelectedValue = "0";
                txtsPesquisar.Text = "";

                gvItens.DataSource = bs_Itens_Tabela.OrderBy(x => x.idItem).Skip(gvItens.PageIndex * nItens).Take(nItens).ToList();
                gvItens.DataBind();
                this.gvItens.Columns[6].Visible = false;

                if (ddlsTipoCompra.SelectedValue == "I")
                {
                    this.gvItens.Columns[9].Visible = false;
                    this.gvItens.Columns[10].Visible = false;
                }
                else
                {
                    this.gvItens.Columns[9].Visible = true;
                    this.gvItens.Columns[10].Visible = true;
                }
            }
            else
            {
                btnInserirItem.Visible = false;
                gvItens.Visible = false;
                MensagemPaginaModal.MostraMensagem_Erro("Nenhum Registro Localizado !");
            }


        }

        protected void btnInserirItem_Click(object sender, EventArgs e)
        {
            string sFuncao = "INCLUIR ITEM";
            string sMensagem = "";

            dtgItens_SalvarGRID();

            foreach (GridViewRow row in gvItens.Rows)
            {
                if ((row.FindControl("Tabela_txtnQuantidade") as TextBox).Text != "" && (row.FindControl("Tabela_txtnQuantidade") as TextBox).Text != "0,00")
                {
                    if ((row.FindControl("Tabela_txtnValorUnitario") as TextBox).Text == "0,00")
                    {
                        sMensagem = "Informe o Valor Unitario !";
                    }

                    if ((row.FindControl("Tabela_txtdtPrevisao") as TextBox).Text == "")
                    {
                        sMensagem += (sMensagem != "" ? "</br>" : "") + "Informe o Previsão Entrega !";
                    }

                    if (sMensagem != "")
                    {
                        MensagemPaginaModal.MostraMensagem_Erro(sMensagem);
                    }
                }
            }

            if (sMensagem == "")
            {
                foreach (GridViewRow row in gvItens.Rows)
                {
                    cls_Pedidos_Itens objItem = new cls_Pedidos_Itens();

                    if ((row.FindControl("Tabela_txtnQuantidade") as TextBox).Text != "" && (row.FindControl("Tabela_txtnQuantidade") as TextBox).Text != "0,00")
                    {
                        objItem.sFuncao = sFuncao;
                        objItem.nOrdem = Base_Pedidos_Itens.Count > 0 ? Base_Pedidos_Itens.OrderBy(i => i.nOrdem).Last().nOrdem + 10 : 10;
                        objItem.idContador = Base_Pedidos_Itens.Count() + 1;
                        objItem.idProduto = Convert.ToInt32(row.Cells[0].Text);
                        objItem.sCodigoProduto = row.Cells[2].Text;
                        objItem.sDscProduto = row.Cells[3].Text;
                        objItem.sUnidade = row.Cells[4].Text;
                        objItem.nQuantidade = Convert.ToDouble((row.FindControl("Tabela_txtnQuantidade") as TextBox).Text);

                        if ((row.FindControl("Tabela_txtnValorUnitario") as TextBox).Text != "")
                            objItem.nValorUnitario = Convert.ToDouble((row.FindControl("Tabela_txtnValorUnitario") as TextBox).Text);
                        else
                            objItem.nValorUnitario = 0;

                        if (hddidPedido.Value != "")
                            objItem.idPedido = Convert.ToInt32(hddidPedido.Value);
                        else
                            objItem.idPedido = 0;

                        objItem.sTipoProduto_Servico = "P";

                        double.TryParse(txtCambioMoeda.Text, out double nCambio);
                        if (hddidTipo.Value != "2" || !div_Moeda.Visible || nCambio <= 0)
                            nCambio = 1;

                        objItem.nValorTotal = Convert.ToDouble((row.FindControl("Tabela_txtnValorUnitario") as TextBox).Text) * Convert.ToDouble((row.FindControl("Tabela_txtnQuantidade") as TextBox).Text) * nCambio;

                        if ((row.FindControl("Tabela_txtdtPrevisao") as TextBox).Text != "")
                        {
                            DateTime data = Convert.ToDateTime((row.FindControl("Tabela_txtdtPrevisao") as TextBox).Text);
                            objItem.dtPrevisaoEntrega = data.ToString("dd/MM/yyyy");
                        }
                        else
                            objItem.dtPrevisaoEntrega = "";

                        if ((row.FindControl("Tabela_txtnICMS") as TextBox).Text != "")
                            objItem.nICMS = Convert.ToDecimal((row.FindControl("Tabela_txtnICMS") as TextBox).Text);
                        else
                            objItem.nICMS = 0;

                        if ((row.FindControl("Tabela_txtnIPI") as TextBox).Text != "")
                            objItem.nIPI = Convert.ToDecimal((row.FindControl("Tabela_txtnIPI") as TextBox).Text);
                        else
                            objItem.nIPI = 0;

                        objItem.idCNM = Convert.ToInt32(row.Cells[1].Text);
                        objItem.Importacao_dtPO = "";
                        objItem.Importacao_dtETA = "";
                        objItem.Importacao_dtETD = "";
                        objItem.nValorTblPreco = Convert.ToDecimal(row.Cells[5].Text);

                        Base_Pedidos_Itens.Add(objItem);
                    }
                }

                DataBind_dtgItens("Editar");
                ScriptManager.RegisterStartupScript(Page, GetType(), "js_CloseModalItens", "$('#Modal_Itens_Tabela').modal('hide');", true);
                updPanel_Itens.Update();
            }
        }

        protected void gvItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList Tabela_ddlsUnidadeEntrega = (e.Row.FindControl("Tabela_ddlsUnidadeEntrega") as DropDownList);
                Popula_Combo(Tabela_ddlsUnidadeEntrega, "sp_Select 'Flow_Produtos_Unidade'", "sUnidade", "sDscUnidade", false, "Selecione ", "0");
                Tabela_ddlsUnidadeEntrega.SelectedValue = DataBinder.Eval(e.Row.DataItem, "sUnidadeEntrega").ToString();

                if ((e.Row.FindControl("Tabela_txtnValorUnitario") as TextBox).Text == "0,00" && e.Row.Cells[5].Text != "0,00")
                {
                    decimal valortabela = decimal.Parse(e.Row.Cells[5].Text);
                    (e.Row.FindControl("Tabela_txtnValorUnitario") as TextBox).Text = valortabela.ToString("N2");
                }

            }
        }

        protected void Tabela_txtdtPrevisao_TextChanged(object sender, EventArgs e)
        {
            var dataitem = bs_Itens_Tabela.FirstOrDefault(i => i.idItem.Equals(int.Parse(((sender as TextBox).NamingContainer as GridViewRow).Cells[0].Text)));
            if (dataitem != null)
                dataitem.dtPrevisao = (sender as TextBox).Text;

            foreach (cls_Itens_Tabela item in bs_Itens_Tabela)
            {
                if (string.IsNullOrEmpty(item.dtPrevisao))
                {
                    item.dtPrevisao = (sender as TextBox).Text;
                }
            }

            AtualizaClasseTabela();
            Ordem = "N";
            hddPageIndex.Value = "N";
            btOrdemTabela_Click(sender, e);
        }

        protected void ddlOrdemTabela_SelectedIndexChanged(object sender, EventArgs e)
        {
            Ordem = "N";
            hddPageIndex.Value = "S";
            btOrdemTabela_Click(sender, e);
        }

        protected void btOrdemTabela_Click(object sender, EventArgs e)
        {
            if (sender is LinkButton linkButton && linkButton.ID == "btOrdemTabela")
            {
                Ordem = "S";
            }

            int quantidade = 0;
            List<cls_Itens_Tabela> lista = null;
            bool ordenacaoDescendente = ViewState["OrdenacaoDescendente"] as bool? ?? true;
            if (Ordem == "N")
            {
                ordenacaoDescendente = false;
                Ordem = "S";
            }
            int.TryParse(ddl_nItens.SelectedValue, out int nItens);
            gvItens.PageSize = nItens;

            if (hddPageIndex.Value == "S")
            {
                gvItens.PageIndex = 0;
            }

            if (ddlOrdemTabela.SelectedValue == "0")
            {
                if (ordenacaoDescendente)
                {
                    if (string.IsNullOrEmpty(txtsPesquisar.Text))
                    {
                        lista = bs_Itens_Tabela.OrderByDescending(x => x.idItem).ToList();
                        ViewState["OrdenacaoDescendente"] = false;
                    }
                    else
                    {
                        lista = bs_Itens_Tabela.Where(x => x.sCodigo.ToUpper().Contains(txtsPesquisar.Text.ToUpper()) || x.sDscProduto.ToUpper().Contains(txtsPesquisar.Text.ToUpper())).OrderByDescending(x => x.idItem).ToList();
                        ViewState["OrdenacaoDescendente"] = false;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(txtsPesquisar.Text))
                    {
                        lista = bs_Itens_Tabela.OrderBy(x => x.idItem).ToList();
                        ViewState["OrdenacaoDescendente"] = true;
                    }
                    else
                    {
                        lista = bs_Itens_Tabela.Where(x => x.sCodigo.ToUpper().Contains(txtsPesquisar.Text.ToUpper()) || x.sDscProduto.ToUpper().Contains(txtsPesquisar.Text.ToUpper())).OrderBy(x => x.idItem).ToList();
                        ViewState["OrdenacaoDescendente"] = true;
                    }
                }
            }

            else if (ddlOrdemTabela.SelectedValue == "1")
            {
                if (ordenacaoDescendente)
                {
                    if (string.IsNullOrEmpty(txtsPesquisar.Text))
                    {
                        lista = bs_Itens_Tabela.OrderByDescending(x => x.sCodigo).ToList();
                        ViewState["OrdenacaoDescendente"] = false;
                    }
                    else
                    {
                        lista = bs_Itens_Tabela.Where(x => x.sCodigo.ToUpper().Contains(txtsPesquisar.Text.ToUpper()) || x.sDscProduto.ToUpper().Contains(txtsPesquisar.Text.ToUpper())).OrderByDescending(x => x.sCodigo).ToList();
                        ViewState["OrdenacaoDescendente"] = false;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(txtsPesquisar.Text))
                    {
                        lista = bs_Itens_Tabela.OrderBy(x => x.sCodigo).ToList();
                        ViewState["OrdenacaoDescendente"] = true;
                    }
                    else
                    {
                        lista = bs_Itens_Tabela.Where(x => x.sCodigo.ToUpper().Contains(txtsPesquisar.Text.ToUpper()) || x.sDscProduto.ToUpper().Contains(txtsPesquisar.Text.ToUpper())).OrderBy(x => x.sCodigo).ToList();
                        ViewState["OrdenacaoDescendente"] = true;
                    }
                }
            }

            else if (ddlOrdemTabela.SelectedValue == "2")
            {
                if (ordenacaoDescendente)
                {
                    if (string.IsNullOrEmpty(txtsPesquisar.Text))
                    {
                        lista = bs_Itens_Tabela.OrderByDescending(x => x.sDscProduto).ToList();
                        ViewState["OrdenacaoDescendente"] = false;
                    }
                    else
                    {
                        lista = bs_Itens_Tabela.Where(x => x.sCodigo.ToUpper().Contains(txtsPesquisar.Text.ToUpper()) || x.sDscProduto.ToUpper().Contains(txtsPesquisar.Text.ToUpper())).OrderByDescending(x => x.sDscProduto).ToList();
                        ViewState["OrdenacaoDescendente"] = false;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(txtsPesquisar.Text))
                    {
                        lista = bs_Itens_Tabela.OrderBy(x => x.sDscProduto).ToList();
                        ViewState["OrdenacaoDescendente"] = true;
                    }
                    else
                    {
                        lista = bs_Itens_Tabela.Where(x => x.sCodigo.ToUpper().Contains(txtsPesquisar.Text.ToUpper()) || x.sDscProduto.ToUpper().Contains(txtsPesquisar.Text.ToUpper())).OrderBy(x => x.sDscProduto).ToList();
                        ViewState["OrdenacaoDescendente"] = true;
                    }
                }
            }

            else if (ddlOrdemTabela.SelectedValue == "3")
            {
                if (ordenacaoDescendente)
                {
                    if (string.IsNullOrEmpty(txtsPesquisar.Text))
                    {
                        lista = bs_Itens_Tabela.OrderByDescending(x => x.sUnidade).ToList();
                        ViewState["OrdenacaoDescendente"] = false;
                    }
                    else
                    {
                        lista = bs_Itens_Tabela.Where(x => x.sCodigo.ToUpper().Contains(txtsPesquisar.Text.ToUpper()) || x.sDscProduto.ToUpper().Contains(txtsPesquisar.Text.ToUpper())).OrderByDescending(x => x.sUnidade).ToList();
                        ViewState["OrdenacaoDescendente"] = false;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(txtsPesquisar.Text))
                    {
                        lista = bs_Itens_Tabela.OrderBy(x => x.sUnidade).ToList();
                        ViewState["OrdenacaoDescendente"] = true;
                    }
                    else
                    {
                        lista = bs_Itens_Tabela.Where(x => x.sCodigo.ToUpper().Contains(txtsPesquisar.Text.ToUpper()) || x.sDscProduto.ToUpper().Contains(txtsPesquisar.Text.ToUpper())).OrderBy(x => x.sUnidade).ToList();
                        ViewState["OrdenacaoDescendente"] = true;
                    }
                }
            }

            else if (ddlOrdemTabela.SelectedValue == "4")
            {
                if (ordenacaoDescendente)
                {
                    if (string.IsNullOrEmpty(txtsPesquisar.Text))
                    {
                        lista = bs_Itens_Tabela.OrderByDescending(x => x.nTotal).Take(nItens).ToList();
                        ViewState["OrdenacaoDescendente"] = false;
                    }
                    else
                    {
                        lista = bs_Itens_Tabela.Where(x => x.sCodigo.ToUpper().Contains(txtsPesquisar.Text.ToUpper()) || x.sDscProduto.ToUpper().Contains(txtsPesquisar.Text.ToUpper())).OrderByDescending(x => x.nTotal).ToList();
                        ViewState["OrdenacaoDescendente"] = false;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(txtsPesquisar.Text))
                    {
                        lista = bs_Itens_Tabela.OrderBy(x => x.nTotal).ToList();
                        ViewState["OrdenacaoDescendente"] = true;
                    }
                    else
                    {
                        lista = bs_Itens_Tabela.Where(x => x.sCodigo.ToUpper().Contains(txtsPesquisar.Text.ToUpper()) || x.sDscProduto.ToUpper().Contains(txtsPesquisar.Text.ToUpper())).OrderBy(x => x.nTotal).ToList();
                        ViewState["OrdenacaoDescendente"] = true;
                    }
                }
            }
            if (!string.IsNullOrEmpty(txtsPesquisar.Text))
                quantidade = bs_Itens_Tabela.Where(x => x.sCodigo.ToUpper().Contains(txtsPesquisar.Text.ToUpper()) || x.sDscProduto.ToUpper().Contains(txtsPesquisar.Text.ToUpper())).Count();

            gvItens.VirtualItemCount = string.IsNullOrEmpty(txtsPesquisar.Text) ? bs_Itens_Tabela.Count : quantidade;
            gvItens.DataSource = lista.Skip(gvItens.PageIndex * nItens).Take(nItens).ToList();
            gvItens.DataBind();
        }

        protected void gvItens_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvItens.PageIndex = e.NewPageIndex;
            hddPageIndex.Value = "N";
            AtualizaClasseTabela();
            Ordem = "N";
            btOrdemTabela_Click(sender, e);
        }

        protected void ddl_nItens_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizaClasseTabela();
            Ordem = "N";
            hddPageIndex.Value = "S";
            btOrdemTabela_Click(sender, e);
        }

        void AtualizaClasseTabela()
        {
            foreach (GridViewRow row in gvItens.Rows)
            {
                if (bs_Itens_Tabela.Where(i => i.idItem == Int32.Parse(row.Cells[0].Text)).First() is cls_Itens_Tabela objItem)
                {
                    if ((row.FindControl("Tabela_txtnQuantidade") as TextBox).Text != "")
                        objItem.nQuantidade = decimal.Parse((row.FindControl("Tabela_txtnQuantidade") as TextBox).Text);
                    if ((row.FindControl("Tabela_txtnICMS") as TextBox).Text != "")
                        objItem.nICMS = decimal.Parse((row.FindControl("Tabela_txtnICMS") as TextBox).Text);
                    if ((row.FindControl("Tabela_txtnIPI") as TextBox).Text != "")
                        objItem.nIPI = decimal.Parse((row.FindControl("Tabela_txtnIPI") as TextBox).Text);
                    if ((row.FindControl("Tabela_txtnValorUnitario") as TextBox).Text != "")
                        objItem.nValorUnitario = decimal.Parse((row.FindControl("Tabela_txtnValorUnitario") as TextBox).Text);
                    if ((row.FindControl("nPorcentagem") as Label).Text != "")
                        objItem.nPorcentagem = decimal.Parse((row.FindControl("nPorcentagem") as Label).Text);
                }
            }
        }

        protected void sPesquisar_TextChanged(object sender, EventArgs e)
        {
            AtualizaClasseTabela();
            Ordem = "N";
            hddPageIndex.Value = "S";
            btOrdemTabela_Click(sender, e);
        }

        #endregion

        #region | Modal - Adicionar Produtos

        protected void lnkAddProduto_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalItens", "$('#Modal_Add_Produtos').modal('show');", true);
            Popula_Combo(ddlAddUnidade, "sp_Select 'Flow_Produtos_Unidade'", "sUnidade", "sDscUnidade", false, "Selecione ", "0");
            txtAddCodigo.Text = "";
            txtAddDescricao.Text = "";
            ddlAddUnidade.SelectedValue = "0";
        }

        protected void btnAdicionar_Click(object sender, EventArgs e)
        {
            string Mensagem = "";
            if (txtAddCodigo.Text == "")
                Mensagem = "Informe um Código de Produto Válido!";
            if (txtAddDescricao.Text == "")
                Mensagem += (Mensagem != "" ? "</br>" : "") + "Informe uma Descrição Válida!";
            if (ddlAddUnidade.SelectedValue == "0")
                Mensagem += (Mensagem != "" ? "</br>" : "") + "Selecione uma Unidade Válida!";

            if (Mensagem == "")
            {
                try
                {
                    DataSet dsGravar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@sCodigo", txtAddCodigo.Text },
                        { "@sDscProduto", txtAddDescricao.Text },
                        { "@sUnidade", ddlAddUnidade.SelectedValue },
                        { "@sSituacao", "V" },
                        { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                    };
                    dsGravar = ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParametros);

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_CloseModalItens", "$('#Modal_Add_Produtos').modal('hide');", true);
                    MensagemPagina5.MostraMensagem_Sucesso("Produto gravado com sucesso!");
                    Item_txtsCodigoProduto.Text = txtAddCodigo.Text;
                    txtsCodigoProduto_TextChanged(sender, e);
                    updPanel_Itens.Update();
                }
                catch
                {
                    msgProduto.MostraMensagem_Erro("Erro ao Adicionar o Produto !");
                }
            }
            else
            {
                msgProduto.MostraMensagem_Erro(Mensagem);
            }
        }

        #endregion

        #endregion

        #region | OPI

        protected void cmdGerarOPI_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "GERAR-OPI" },
                { "@idUsuarioInclusao", Variaveis.idUsuario() },
                { "@idLM", "0" },
                { "@sDscOPI", $"OPI{DateTime.Now} - Via Pedido: {int.Parse(hddidPedido.Value):D4}" },
                { "@idPedido", hddidPedido.Value }
            };
            DataSet dsSalvar = ExecutarDataSet(sProcedure_OPI, vParametros);

            if (ValidarDataSet(dsSalvar, out string sErro) && DATASET(dsSalvar, 0, "ret") == "0")
            {
                cmdGerarOPI.Visible = false;
                MensagemPagina.MostraMensagem_Sucesso(string.Format($"Gerado com sucesso!</br><a href='WMS/OPI_Detalhe.aspx?id={DATASET(dsSalvar, "idOPI")}'>Visualizar OPI Gerada!</a>"));
            }
            else if (DATASET(dsSalvar, 0, "ret") == "1") MensagemPagina.MostraMensagem_Aviso(DATASET(dsSalvar, 0, "sMensagem"));
            else MensagemPagina.MostraMensagem_Erro("BD: " + sErro.ToString());
        }

        void ValidaBotão_OPI(string idPedido, bool bPossuiProduto)
        {
            cmdGerarOPI.Visible = false;

            if (!bPossuiProduto) return;

            if (ValidaBotão_LM(idPedido))
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR-OPI-PEDIDO" },
                    { "@idPedido", idPedido }
                };
                DataSet ds = ExecutarDataSet(sProcedure_OPI, vParametros);

                if (!ValidarDataSet(ds, out _) && hddididStatus.Value != idStatus_Bloqueado) cmdGerarOPI.Visible = ValidaPermissao(Permissao.WMS.OPI.GerarOPI);
            }
        }

        bool ValidaBotão_LM(string idPedido)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_LM_RESUMO" },
                { "@idPedido", idPedido }
            };
            DataSet LM_ds = ExecutarDataSet(LM_sProcedure, vParametros);

            if (ValidarDataSet(LM_ds, out _))
            {
                cmdGerarOPI.Visible = false;
                return false;
            }
            //else if (hddididStatus.Value != idStatus_Bloqueado) cmdGerarOPI.Visible = ValidaPermissao(Permissao.WMS.OPI.GerarOPI);

            return true;
        }

        #endregion

        #region | Script / WebMethod

        protected string RetornarScripts()
        {
            string idPesquisa = "1";
            string idCliente = "";
            string idParceiro = "";

            if (hddidCliente_Produto.Value != "") idCliente = hddidCliente_Produto.Value;
            else idCliente = hddidCliente.Value;

            if (hddidTipo.Value == "3" || hddidTipo.Value == "6") idPesquisa = "0";

            if (hddidTipo.Value == "7")
            {
                if (ddlFornecedor.SelectedValue != "") idParceiro = ddlFornecedor.SelectedValue;
                else idParceiro = "0";
            }

            StringBuilder sb = new StringBuilder();

            if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
            {
                sb.Append("$v192(function() {");
                sb.Append("$v192(\"[id$=Item_txtsDscProduto]\").autocomplete({");
                sb.Append("source: function(request, response) {");
                sb.Append("$v192.ajax({");
                sb.Append("url: '/app/Paginas/Pedidos_Detalhe.aspx/GetProdutos',");
                sb.Append("data: JSON.stringify({ sDscProduto: request.term, idPesquisa: '" + idPesquisa + "', idPais: '" + 0 + "', sidParceiro: '" + 0 + "', idUsuario: '" + 0 + "', tipo: '" + hddidTipo.Value + "'}),");
                sb.Append("dataType: \"json\",");
                sb.Append("type: \"POST\",");
                sb.Append("contentType: \"application/json; charset=utf-8\",");
                sb.Append("success: function(data) {");
                sb.Append("response($v192.map(data.d, function(item) {");
                sb.Append("return {");
                sb.Append("label: item.split('|')[0],");
                sb.Append("val: item.split('|')[2],");
                sb.Append("idProduto: item.split('|')[1],");
                sb.Append("un: item.split('|')[3],");
                sb.Append("sDscCategoriaVendas: item.split('|')[4]");
                sb.Append(", idNCM: item.split('|')[5]");
                sb.Append(", sCodigoNCM: item.split('|')[6]");
                sb.Append(", sTipoProduto_Servico: item.split('|')[7]");
                sb.Append(", sDscProdutoIdioma: item.split('|')[8]");
                sb.Append("}");
                sb.Append("}))");
                sb.Append("},");
                sb.Append("error: function(response) {");
                sb.Append("alert(response.responseText);");
                sb.Append("},");
                sb.Append("failure: function(response) {");
                sb.Append("alert(response.responseText);");
                sb.Append("}");
                sb.Append("});");
                sb.Append("},");
                sb.Append("select: function(e, i) {");
                sb.Append("$('[id$=Item_txtsCodigoProduto]').val(i.item.val);");
                sb.Append("$('[id$=Item_ddlsUnidade]').val(i.item.un);");
                sb.Append("$('[id$=Item_hddsDscCategoriaVendas]').val(i.item.sDscCategoriaVendas);");
                sb.Append("$('[id$=Item_hddidProduto]').val(i.item.idProduto);");
                sb.Append("$('[id$=Item_hddidPesquisa]').val('1');");
                sb.Append("$('[id$=Item_hddPesquisaPor]').val('DESCRICAO');");
                sb.Append("$('[id$=Item_hddidNCM]').val(i.item.idNCM);");
                sb.Append("$('[id$=Item_hddsCodigoNCM]').val(i.item.sCodigoNCM);");
                sb.Append("$('[id$=Item_hddsTipoProduto_Servico]').val(i.item.sTipoProduto_Servico);");
                sb.Append("$('[id$=Item_hddssDscDescricaoIdioma]').val(i.item.sDscProdutoIdioma);");
                sb.Append("$('[id$=Item_txtnQuantidade]').focus();");
                sb.Append("},");
                sb.Append("minLength: 3");
                sb.Append("});});");
                sb.AppendLine("     $('.excluirTodos input').on('change', function() {");
                sb.AppendLine("         var excluir = $(this).prop('checked');\r\n");
                sb.AppendLine("         $('.excluir input').each(function () {\r\n");
                sb.AppendLine("             $(this).prop('checked', excluir);\r\n");
                sb.AppendLine("         });\r\n");
                sb.AppendLine("     });");

                sb.AppendLine("    var gridId = '" + dtgItens.ClientID + "';");
                sb.AppendLine("");
                sb.AppendLine("    function configurarReplicacao(btnId, ddlPattern) {");
                sb.AppendLine("        var $btn = $('#' + btnId);");
                sb.AppendLine("        $btn.off('click').on('click', function(e) {");
                sb.AppendLine("            e.preventDefault();");
                sb.AppendLine("            var $grid = $('#' + gridId);");
                sb.AppendLine("            var $linhas = $grid.find('tr').has('select[id*=\"' + ddlPattern + '\"]');");
                sb.AppendLine("            if ($linhas.length === 0) {");
                sb.AppendLine("                console.warn('Nenhuma linha com o dropdown encontrado.');");
                sb.AppendLine("                return;");
                sb.AppendLine("            }");
                sb.AppendLine("            var $primeiroDdl = $linhas.first().find('select[id*=\"' + ddlPattern + '\"]');");
                sb.AppendLine("            var valor = $primeiroDdl.val();");
                sb.AppendLine("            console.log('Valor capturado para replicar:', valor, '| Dropdown:', $primeiroDdl.attr('id'));");
                sb.AppendLine("            if (!valor) {");
                sb.AppendLine("                alert('Selecione um valor na primeira linha antes de replicar.');");
                sb.AppendLine("                return;");
                sb.AppendLine("            }");
                sb.AppendLine("            $linhas.each(function() {");
                sb.AppendLine("                var $ddl = $(this).find('select[id*=\"' + ddlPattern + '\"]');");
                sb.AppendLine("                if ($ddl.length > 0) {");
                sb.AppendLine("                    var $opcaoValida = $ddl.find('option').filter(function() { return this.value === valor; });");
                sb.AppendLine("                    if ($opcaoValida.length > 0) {");
                sb.AppendLine("                        $ddl.val(valor).trigger('change'); // .trigger('change') atualiza UI/plugins");
                sb.AppendLine("                    } else {");
                sb.AppendLine("                        console.warn('Valor \"' + valor + '\" não existe no dropdown:', $ddl.attr('id'));");
                sb.AppendLine("                    }");
                sb.AppendLine("                }");
                sb.AppendLine("            });");
                sb.AppendLine("        });");
                sb.AppendLine("    }");
                sb.AppendLine("    configurarReplicacao('cmdReplicaSelecao_Destino', 'ddlItem_Importacao_idDestino');");
                sb.AppendLine("    configurarReplicacao('cmdReplicarSelecao_DrawBack', 'ddlItem_Importacao_idAtoConcessorio');");
            }

            if (hddidTipo.Value == "2")
            {
                sb.Append("$v192(function() {");
                sb.Append("$v192(\"[id$=Item_txtsDscProduto]\").autocomplete({");
                sb.Append("source: function(request, response) {");
                sb.Append("$v192.ajax({");
                sb.Append("url: '/app/Paginas/Pedidos_Detalhe.aspx/GetProdutos',");
                sb.Append("data: JSON.stringify({ sDscProduto: request.term, idPesquisa: '" + idPesquisa + "', idPais: '" + 0 + "', sidParceiro: '" + 0 + "', idUsuario: '" + idCliente + "', tipo: '" + hddidTipo.Value + "' }),");  // Use JSON.stringify para converter dados em uma string JSON
                sb.Append("dataType: \"json\",");
                sb.Append("type: \"POST\",");
                sb.Append("contentType: \"application/json; charset=utf-8\",");
                sb.Append("success: function(data) {");
                sb.Append("response($v192.map(data.d, function(item) {");
                sb.Append("return {");
                sb.Append("label: item.split('|')[0],");
                sb.Append("val: item.split('|')[2],");
                sb.Append("idProduto: item.split('|')[1],");
                sb.Append("un: item.split('|')[3],");
                sb.Append("sDscCategoriaVendas: item.split('|')[4]");
                sb.Append(", idNCM: item.split('|')[5]");
                sb.Append(", sCodigoNCM: item.split('|')[6]");
                sb.Append(", sTipoProduto_Servico: item.split('|')[7]");
                sb.Append(", sDscProdutoIdioma: item.split('|')[8]");
                sb.Append("}");
                sb.Append("}))");
                sb.Append("},");
                sb.Append("error: function(response) {");
                sb.Append("alert(response.responseText);");
                sb.Append("},");
                sb.Append("failure: function(response) {");
                sb.Append("alert(response.responseText);");
                sb.Append("}");
                sb.Append("});");
                sb.Append("},");
                sb.Append("select: function(e, i) {");
                sb.Append("$('[id$=Item_txtsCodigoProduto]').val(i.item.val);");
                sb.Append("$('[id$=Item_ddlsUnidade]').val(i.item.un);");
                sb.Append("$('[id$=Item_hddsDscCategoriaVendas]').val(i.item.sDscCategoriaVendas);");
                sb.Append("$('[id$=Item_hddidProduto]').val(i.item.idProduto);");
                sb.Append("$('[id$=Item_hddidPesquisa]').val('1');");
                sb.Append("$('[id$=Item_hddPesquisaPor]').val('DESCRICAO');");
                sb.Append("$('[id$=Item_hddidNCM]').val(i.item.idNCM);");
                sb.Append("$('[id$=Item_hddsCodigoNCM]').val(i.item.sCodigoNCM);");
                sb.Append("$('[id$=Item_hddsTipoProduto_Servico]').val(i.item.sTipoProduto_Servico);");
                sb.Append("$('[id$=Item_hddssDscDescricaoIdioma]').val(i.item.sDscProdutoIdioma);");
                sb.Append("$('[id$=Item_txtnQuantidade]').focus();");
                sb.Append("},");
                sb.Append("minLength: 3");
                sb.Append("});});");

                sb.AppendLine("var inputCalculo = '[id*=LM_gvItens_txtnQuantidade], [id*=LM_gvItens_txtnValorUnitario], [id*=LM_gvItens_txtnIPI]';");
                sb.AppendLine("if ($(inputCalculo).length > 0) {");
                sb.AppendLine("     $(inputCalculo).on('blur', function () {");
                sb.AppendLine("         recalcularTotalLinha(this);");
                sb.AppendLine("     });");
                sb.AppendLine("}");

                sb.AppendLine("function recalcularTotalLinha(inputAtual) {");
                sb.AppendLine("     var $row = $(inputAtual).closest('tr');");
                sb.AppendLine("     var gridView = $(inputAtual).closest('[data-grid-type]');");
                sb.AppendLine("     var gridType = gridView.data('grid-type');");
                sb.AppendLine("     var valorText = $row.find('[id*=LM_gvItens_txtnValorUnitario]').val() || '0';");
                sb.AppendLine("     var valorUnitario = parseFloat(valorText.replace(/\\./g, '').replace(',', '.')) || 0;");
                sb.AppendLine("     var qtdText = $row.find('[id*=LM_gvItens_txtnQuantidade]').val() || '0';");
                sb.AppendLine("     var qtd = parseFloat(qtdText.replace(',', '.')) || 0;");
                sb.AppendLine("     var ipiText = $row.find('[id*=LM_gvItens_txtnIPI]').val() || '0';");
                sb.AppendLine("     var ipi = parseFloat(ipiText.replace(',', '.')) || 0;");

                sb.AppendLine("         var subtotal = valorUnitario * qtd;");
                sb.AppendLine("         var total = subtotal + (subtotal * (ipi / 100));");
                sb.AppendLine("         $row.find('.lblTotal').text(total.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));\r\n");
                sb.AppendLine("}");
            }

            if (hddidTipo.Value == "7")
            {
                sb.Append("$v192(function() {");
                sb.Append("$v192(\"[id$=Item_txtsDscProduto]\").autocomplete({");
                sb.Append("source: function(request, response) {");
                sb.Append("$v192.ajax({");
                sb.Append("url: '/app/Paginas/Pedidos_Detalhe.aspx/GetProdutos',");
                sb.Append("data: JSON.stringify({ sDscProduto: request.term, idPesquisa: '" + idPesquisa + "', idPais: '" + 0 + "', sidParceiro: '" + idParceiro + "', idUsuario: '" + 0 + "', tipo: '" + hddidTipo.Value + "'}),");  // Use JSON.stringify para converter dados em uma string JSON
                sb.Append("dataType: \"json\",");
                sb.Append("type: \"POST\",");
                sb.Append("contentType: \"application/json; charset=utf-8\",");
                sb.Append("success: function(data) {");
                sb.Append("response($v192.map(data.d, function(item) {");
                sb.Append("return {");
                sb.Append("label: item.split('|')[0],");
                sb.Append("val: item.split('|')[2],");
                sb.Append("idProduto: item.split('|')[1],");
                sb.Append("un: item.split('|')[3],");
                sb.Append("sDscCategoriaVendas: item.split('|')[4]");
                sb.Append(", idNCM: item.split('|')[5]");
                sb.Append(", sCodigoNCM: item.split('|')[6]");
                sb.Append(", sTipoProduto_Servico: item.split('|')[7]");
                sb.Append(", sDscProdutoIdioma: item.split('|')[8]");
                sb.Append(", nTotal: item.split('|')[9]");
                sb.Append(", sUnidadeEntrega: item.split('|')[10]");
                sb.Append("}");
                sb.Append("}))");
                sb.Append("},");
                sb.Append("error: function(response) {");
                sb.Append("alert(response.responseText);");
                sb.Append("},");
                sb.Append("failure: function(response) {");
                sb.Append("alert(response.responseText);");
                sb.Append("}");
                sb.Append("});");
                sb.Append("},");
                sb.Append("select: function(e, i) {");
                sb.Append("$('[id$=Item_txtsCodigoProduto]').val(i.item.val);");
                sb.Append("$('[id$=Item_ddlsUnidade]').val(i.item.un);");
                sb.Append("$('[id$=Item_hddsDscCategoriaVendas]').val(i.item.sDscCategoriaVendas);");
                sb.Append("$('[id$=Item_hddidProduto]').val(i.item.idProduto);");
                sb.Append("$('[id$=Item_hddidPesquisa]').val('1');");
                sb.Append("$('[id$=Item_hddPesquisaPor]').val('DESCRICAO');");
                sb.Append("$('[id$=Item_hddidNCM]').val(i.item.idNCM);");
                sb.Append("$('[id$=Item_hddsCodigoNCM]').val(i.item.sCodigoNCM);");
                sb.Append("$('[id$=Item_hddsTipoProduto_Servico]').val(i.item.sTipoProduto_Servico);");
                sb.Append("$('[id$=Item_hddssDscDescricaoIdioma]').val(i.item.sDscProdutoIdioma);");
                sb.Append("$('[id$=Item_hddsValortblPreco]').val(i.item.nTotal);");
                sb.Append("$('[id$=Item_txtnQuantidade]').focus();");
                sb.Append("$('[id$=Item_ddlsUnidadeEntrega]').val(i.item.sUnidadeEntrega);");
                sb.Append("},");
                sb.Append("minLength: 3");
                sb.Append("});});");
            }

            sb.Append("$v192(function() {");
            sb.Append("$v192(\"[id$=LM_txtsDscProduto]\").autocomplete({");
            sb.Append("source: function(request, response) {");
            sb.Append("$v192.ajax({");
            sb.Append("url: '/app/Paginas/Pedidos_Detalhe.aspx/GetProdutos',");
            sb.Append("data: JSON.stringify({ sDscProduto: request.term, idPesquisa: '2', idPais: '0', sidParceiro: '0', idUsuario: '0', tipo: '" + hddidTipo.Value + "'}),");  // Use JSON.stringify para converter dados em uma string JSON
            sb.Append("dataType: \"json\",");
            sb.Append("type: \"POST\",");
            sb.Append("contentType: \"application/json; charset=utf-8\",");
            sb.Append("success: function(data) {");
            sb.Append("response($v192.map(data.d, function(item) {");
            sb.Append("return {");
            sb.Append("label: item.split('|')[0],");
            sb.Append("val: item.split('|')[2],");
            sb.Append("idProduto: item.split('|')[1],");
            sb.Append("un: item.split('|')[3],");
            sb.Append("sDscCategoriaVendas: item.split('|')[4]");
            sb.Append("}");
            sb.Append("}))");
            sb.Append("},");
            sb.Append("error: function(response) {");
            sb.Append("alert(response.responseText);");
            sb.Append("},");
            sb.Append("failure: function(response) {");
            sb.Append("alert(response.responseText);");
            sb.Append("}");
            sb.Append("});");
            sb.Append("},");
            sb.Append("select: function(e, i) {");
            sb.Append("$('[id$=LM_txtsCodigoProduto]').val(i.item.val);");
            sb.Append("$('[id$=LM_ddlsUnidade]').val(i.item.un);");
            sb.Append("$('[id$=LM_txtsAgrupamento]').val(i.item.sDscCategoriaVendas);");
            sb.Append("$('[id$=LM_hddidProduto]').val(i.item.idProduto);");
            sb.Append("$('[id$=LM_hddidPesquisa]').val('2');");
            sb.Append("$('[id$=LM_hddPesquisaPor]').val('DESCRICAO');");
            sb.Append("$('[id$=LM_txtnQuantidade]').focus();");
            sb.Append("},");
            sb.Append("minLength: 3");
            sb.Append("});});");

            // ToggleDetails - para controlar exibição da GridComposição
            sb.Append("function toggleDetails(linkButton) {");
            sb.Append("    var row = $(linkButton).closest('tr');");
            sb.Append("    var detailsColumn = row.find('.details-column');");
            sb.Append("    if (detailsColumn.is(':hidden')) {");
            sb.Append("        detailsColumn.show();");
            sb.Append("    } else {");
            sb.Append("        detailsColumn.hide();");
            sb.Append("    }");
            sb.Append("}");

            sb.Append("function Servico(linkButton, idProduto, sDscProduto) {");
            sb.Append("    $('[id*=hddsModalComposicao]').val(idProduto);");
            sb.Append("    $('[id*=hddsDscComposicao]').val(sDscProduto);");
            sb.Append("    __doPostBack('dialog_Composicao', '');");
            sb.Append("}");

            sb.Append("function ComposicaoServico(linkButton) {");
            sb.Append("    var row = $(linkButton).closest('tr');"); // Encontra a linha da tabela onde o botão foi clicado
            sb.Append("    var idItem = row.find('.idItem').text();"); // Encontra o texto da Label com a classe 'idItem'
            sb.Append("    var sDscItem = row.find('.sDscProduto').text();"); // Encontra o texto da Label com a classe 'idItem'
            sb.Append("    $('[id*=hddsComposicao]').val(idItem);"); // Define o valor do campo hddsComposicao")
            sb.Append("    $('[id*=hddsDscComposicaoServico]').val(sDscItem);"); // Define o valor do campo hddsComposicao")
            sb.Append("    __doPostBack('dialog_ServicoComposicao', '');"); // Aciona o postback
            sb.Append("}");

            sb.Append("function mascaraData(campo, e )");
            sb.Append("{");
            sb.Append("var kC = (document.all) ? event.keyCode : e.keyCode;");
            sb.Append("var data = campo.value;");
            sb.Append("if (kC!=8 && kC!=46 )");
            sb.Append("{");
            sb.Append("if (data.length==2)");
            sb.Append("{campo.value = data += '/';}");
            sb.Append("else if(data.length==5){campo.value = data += '/';}elsecampo.value = data;}};");

            // Mensagens de Confirmação
            sb.Append("$v192(function() {");
            sb.Append("$v192(\"#dialog-Salvar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_SALVAR\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");

            sb.Append("$v192('[id*=cmdGravarPedido]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("$v192(\"#dialog-Editar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_Editar\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");

            sb.Append("$v192('[id*=cmdEditar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Editar').dialog('open');");
            sb.Append("});");

            sb.Append("$v192(\"#dialog-Duplicar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_Duplicar\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");

            sb.Append("$v192('[id*=cmdDuplicar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Duplicar').dialog('open');");
            sb.Append("});");

            sb.Append("$v192(\"#LM_dialog_Salvar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_LM_Salvar\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");

            sb.Append("$v192('[id*=LM_cmdSalvarLM]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#LM_dialog_Salvar').dialog('open');");
            sb.Append("});");

            //Caixa de seleção de datas
            sb.Append("});");

            if (!txtdtPedido.ReadOnly)
            {
                sb.Append("$(function() {$('[id*=txtdtPedido]').datepicker({");
                sb.Append("autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");
            }

            if (!txtdtEstimativaEntrega.ReadOnly)
            {
                sb.Append("$(function() {$('[id*=txtdtEstimativaEntrega]').datepicker({");
                sb.Append("autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");
            }

            if (hddidTipo.Value == "2" || hddidTipo.Value == "7")
            {
                sb.Append("$('[id*=txtnVlrServicos]').mask('0.000.000.009,99', { reverse: true });");
                sb.Append("$('[id*=txtnVlrComissao]').mask('0.000.000.009,99', { reverse: true });");
                //----------------------------------------------------------
                sb.Append("$('[id*=txtFrete]').mask('0.000.000.009,99', { reverse: true });");
                sb.Append("$('[id*=txtnFreteVendas]').mask('0.000.000.009,99', { reverse: true });");

                if (hddidTipo.Value == "2")
                {
                    sb.Append("$('[id*=Item_txtnValorUnitario]').mask('0.000.000.009,99', { reverse: true });");
                    sb.Append("$('[id*=Item_GV_txtnValorUnitario]').mask('0.000.000.009,99', { reverse: true });");
                    sb.Append("$('[id*=LM_gvItens_txtnValorUnitario]').mask('0.000.000.009,99', { reverse: true });");
                    sb.Append("$('[id*=LM_gvItens_txtnIPI]').mask('009,99', { reverse: true });");
                }

                sb.Append("$('[id*=txtnQuantidade]').mask('0.000.009,99', { reverse: true });");

                if (hddidTipo.Value == "7")
                {
                    sb.Append("$('[id*=Item_txtnValorUnitario]').mask('0.000.000.009,9999', { reverse: true });");
                    sb.Append("$('[id*=Item_GV_txtnValorUnitario]').mask('0.000.000.009,9999', { reverse: true });");
                    sb.Append("$('[id*=Item_GV_txtnDesconto]').mask('0.000.000.009,9999', { reverse: true });");
                }

                sb.Append("if ($('[id*=GV_txtnQuantidade]').length > 0) {");
                sb.Append("     $('[id*=GV_txtnQuantidade]').on('input', function () {");
                sb.Append("         var $row = $(this).closest('tr');");
                sb.Append("         var nQuantidade = parseFloat($row.find('[id*=GV_txtnQuantidade]').val().replace('.','').replace(',', '.'));");
                sb.Append("         var nValorUnitario = parseFloat($row.find('[id*=GV_txtnValorUnitario]').val().replace('.','').replace(',', '.'));");

                if (hddidTipo.Value == "7")
                {
                    sb.Append("         var nDesc = parseFloat($row.find('[id*=GV_txtnDesconto]').val().replace('.','').replace(',', '.'));");
                    sb.Append("         var nValorTotal = isNaN(nValorUnitario) || isNaN(nQuantidade) ? 0 : (nValorUnitario * nQuantidade) - (nValorUnitario * nQuantidade * (nDesc / 100));");
                }
                else if (hddidTipo.Value == "2" && div_Moeda.Visible)
                {
                    sb.Append("         var nCambio = parseFloat($('.nCambio').val().replace('.','').replace(',', '.') || '0') || 0;");
                    sb.Append("         var nValorTotal = isNaN(nValorUnitario) || isNaN(nQuantidade) ? 0 : nValorUnitario * nQuantidade * (nCambio > 0 ? nCambio : 1);");
                }
                else
                    sb.Append("         var nValorTotal = isNaN(nValorUnitario) || isNaN(nQuantidade) ? 0 : nValorUnitario * nQuantidade;");

                sb.Append("         $row.find('._nValorTotal').text(FormatarValor(nValorTotal, 2));");
                sb.Append("     });");
                sb.Append("}");
                sb.Append("if ($('[id*=GV_txtnValorUnitario]').length > 0) {");
                sb.Append("     $('[id*=GV_txtnValorUnitario]').on('input', function () {");
                sb.Append("         var $row = $(this).closest('tr');");
                sb.Append("         var nQuantidade = parseFloat($row.find('[id*=GV_txtnQuantidade]').val().replace('.','').replace(',', '.'));");
                sb.Append("         var nValorUnitario = parseFloat($row.find('[id*=GV_txtnValorUnitario]').val().replace('.','').replace(',', '.'));");

                if (hddidTipo.Value == "7")
                {
                    sb.Append("         var nDesc = parseFloat($row.find('[id*=GV_txtnDesconto]').val().replace('.','').replace(',', '.'));");
                    sb.Append("         var nValorTotal = isNaN(nValorUnitario) || isNaN(nQuantidade) ? 0 : (nValorUnitario * nQuantidade) - (nValorUnitario * nQuantidade * (nDesc / 100));");
                }
                else if (hddidTipo.Value == "2" && div_Moeda.Visible)
                {
                    sb.Append("         var nCambio = parseFloat($('.nCambio').val().replace('.','').replace(',', '.') || '0') || 0;");
                    sb.Append("         var nValorTotal = isNaN(nValorUnitario) || isNaN(nQuantidade) ? 0 : nValorUnitario * nQuantidade * (nCambio > 0 ? nCambio : 1);");
                }
                else
                    sb.Append("         var nValorTotal = isNaN(nValorUnitario) || isNaN(nQuantidade) ? 0 : nValorUnitario * nQuantidade;");

                sb.Append("         $row.find('._nValorTotal').text(FormatarValor(nValorTotal, 2));");
                sb.Append("     });");
                sb.Append("}");

                if (hddidTipo.Value == "7")
                {
                    sb.Append("if ($('[id*=GV_txtnDesconto]').length > 0) {");
                    sb.Append("     $('[id*=GV_txtnDesconto]').on('input', function () {");
                    sb.Append("         var $row = $(this).closest('tr');");
                    sb.Append("         var nQuantidade = parseFloat($row.find('[id*=GV_txtnQuantidade]').val().replace('.','').replace(',', '.'));");
                    sb.Append("         var nValorUnitario = parseFloat($row.find('[id*=GV_txtnValorUnitario]').val().replace('.','').replace(',', '.'));");
                    sb.Append("         var nDesc = parseFloat($row.find('[id*=GV_txtnDesconto]').val().replace('.','').replace(',', '.'));");
                    sb.Append("         var nValorTotal = isNaN(nValorUnitario) || isNaN(nQuantidade) ? 0 : (nValorUnitario * nQuantidade) - (nValorUnitario * nQuantidade * (nDesc / 100));");
                    sb.Append("         $row.find('._nValorTotal').text(nValorTotal.toFixed(2).replace('.', ','));");
                    sb.Append("     });");
                    sb.Append("}");
                }
            }
            else if (hddidTipo.Value == "3" || hddidTipo.Value == "6")
            {
                sb.Append("$('[id*=txtnValorUnitario]').mask('0.000.000.009,9999', { reverse: true });");
                sb.Append("$('[id*=txtnQuantidade]').mask('0.000.009,9999', { reverse: true });");
                sb.Append("$('[id*=txtnValorUnitario]').mask('0.000.000.009,9999', { reverse: true });");
                sb.Append("if ($('[id*=Itens_ValoresDatas_txtnQuantidade]').length > 0)    {\r\n$('[id*=Itens_ValoresDatas_txtnQuantidade]'   ).on('input', function () {\r\nvar $row = $(this).closest('tr');\r\nvar nQuantidade = parseFloat($row.find('[id*=Itens_ValoresDatas_txtnQuantidade]').val().replace('.','').replace(',', '.'));\r\nvar nValorUnitario = parseFloat($row.find('[id*=Itens_ValoresDatas_txtnValorUnitario]').val().replace('.','').replace(',', '.'));\r\nvar nValorTotal = isNaN(nValorUnitario) || isNaN(nQuantidade) ? 0 : nValorUnitario * nQuantidade;\r\n$row.find('._nValorTotal').text(nValorTotal.toFixed(4).replace('.', ',')); \r\n}); \r\n}");
                sb.Append("if ($('[id*=Itens_ValoresDatas_txtnValorUnitario]').length > 0) {\r\n$('[id*=Itens_ValoresDatas_txtnValorUnitario]').on('input', function () {\r\nvar $row = $(this).closest('tr');\r\nvar nQuantidade = parseFloat($row.find('[id*=Itens_ValoresDatas_txtnQuantidade]').val().replace('.','').replace(',', '.'));\r\nvar nValorUnitario = parseFloat($row.find('[id*=Itens_ValoresDatas_txtnValorUnitario]').val().replace('.','').replace(',', '.'));\r\nvar nValorTotal = isNaN(nValorUnitario) || isNaN(nQuantidade) ? 0 : nValorUnitario * nQuantidade;\r\n$row.find('._nValorTotal').text(nValorTotal.toFixed(4).replace('.', ',')); \r\n}); \r\n}");
                //Resultado
                sb.Append("$('[id*=txtnValorReal]').mask('0.000.000.009,9999', { reverse: true });");
                sb.Append("if ($('[id*=txtnQuantidade]').length > 0)    {\r\n$('[id*=txtnQuantidade]'   ).on('input', function () {\r\nvar $row = $(this).closest('tr');\r\nvar nQuantidade = parseFloat($row.find('[id*=txtnQuantidade]').val().replace('.','').replace(',', '.'));\r\nvar nValorUnitario = parseFloat($row.find('[id*=Itens_Resultado_txtnValorUnitario]').val().replace('.','').replace(',', '.'));\r\nvar nValorTotal = isNaN(nValorUnitario) || isNaN(nQuantidade) ? 0 : nValorUnitario * nQuantidade;\r\n$row.find('._nValorTotal').text(nValorTotal.toFixed(4).replace('.', ',')); \r\n}); \r\n}");
                sb.Append("if ($('[id*=Itens_Resultado_txtnValorUnitario]').length > 0) {" +
                    "\r\n$('[id*=Itens_Resultado_txtnValorUnitario]').on('input', function () {" +
                    "\r\nvar $row = $(this).closest('tr');" +
                    "\r\nvar nQuantidade = parseFloat($row.find('._nQuantidade').text().replace('.','').replace(',', '.'));" +
                    "\r\nvar nValorUnitario = parseFloat($row.find('[id*=Itens_Resultado_txtnValorUnitario]').val().replace('.','').replace(',', '.'));" +
                    "\r\nvar nValorTotal = isNaN(nValorUnitario) || isNaN(nQuantidade) ? 0 : nValorUnitario * nQuantidade;" +
                    "\r\n$row.find('._nValorTotal').text(nValorTotal.toFixed(2).replace('.', ','));" +
                    "\r\nvar nValorRealTotal = parseFloat($row.find('._nValorRealTotal').text().replace('.','').replace(',', '.'));" +
                    "\r\nvar nValorTotal = parseFloat($row.find('._nValorTotal').text().replace('.','').replace(',', '.'));" +
                    "\r\nvar nResultado = isNaN(nValorTotal) || isNaN(nValorRealTotal) ? 0 : (nValorRealTotal - nValorTotal);" +
                    "\r\n$row.find('._nResultado').text(nResultado.toFixed(4).replace('.', ','));  \r\n}); \r\n}");

                sb.Append("if ($('[id*=txtnValorReal]').length > 0) {" +
                    "\r\n$('[id*=txtnValorReal]').on('input', function () {" +
                    "\r\nvar $row = $(this).closest('tr');" +
                    "\r\nvar nQuantidade = parseFloat($row.find('._nQuantidade').text().replace('.','').replace(',', '.'));" +
                    "\r\nvar nValorUnitario = parseFloat($row.find('[id*=txtnValorReal]').val().replace('.','').replace(',', '.'));" +
                    "\r\nvar nValorRealTotal = isNaN(nValorUnitario) || isNaN(nQuantidade) ? 0 : nValorUnitario * nQuantidade;" +
                    "\r\n$row.find('._nValorRealTotal').text(nValorRealTotal.toFixed(2).replace('.', ','));" +
                    "\r\nvar nValorRealTotal = parseFloat($row.find('._nValorRealTotal').text().replace('.','').replace(',', '.'));" +
                    "\r\nvar nValorTotal = parseFloat($row.find('._nValorTotal').text().replace('.','').replace(',', '.'));" +
                    "\r\nvar nResultado = isNaN(nValorTotal) || isNaN(nValorRealTotal) ? 0 : (nValorRealTotal - nValorTotal);" +
                    "\r\n$row.find('._nResultado').text(nResultado.toFixed(4).replace('.', ',')); \r\n}); \r\n}");

                sb.Append("$(function() {$('[id*=gv_Itens_ValoresDatas_Itens_ValoresDatas_txtdtPO]').datepicker({autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");
                sb.Append("$(function() {$('[id*=gv_Itens_ValoresDatas_Itens_ValoresDatas_txtdtETD]').datepicker({autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");
                sb.Append("$(function() {$('[id*=gv_Itens_ValoresDatas_Itens_ValoresDatas_txtdtETA]').datepicker({autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");

                sb.Append("$('[id*=Itens_ValoresDatas_txtnValorUnitario]').mask('0.000.000.009,9999', { reverse: true });");
                sb.Append("$('[id*=Itens_Resultado_txtnValorUnitario]').mask('0.000.000.009,9999', { reverse: true });");

                //Cambio
                sb.Append("$('[id*=txtCambio]').mask('0.000.000.009,99', { reverse: true });");
                //sb.Append("$('[id*=txtPagar]').mask('0.000.000.009,99', { reverse: true });");
                sb.Append("$('[id*=txtPago]').mask('0.000.000.009,99', { reverse: true });");
                sb.Append("$('[id*=txtnValorPagamento_Info_Pag]').mask('0.000.000.009,99', { reverse: true });");

                sb.Append("$('[id*=txtnValorEnvio]').mask('0.000.000.009,99', { reverse: true });");
                sb.Append("$('[id*=txtnValorEnvioReal]').mask('0.000.000.009,99', { reverse: true });");

                sb.Append("if ($('[id*=txtCambio]').length > 0) {" +
                    "\r\n$('[id*=txtCambio]').on('input', function () {" +
                    "\r\nvar Cambio = parseFloat($('[id*=txtCambio]').val().replace('.','').replace(',', '.'));" +
                    "\r\nvar Pago = parseFloat($('[id*=txtPago]').val().replace('.','').replace(',', '.'));" +
                    "\r\nvar Pagar = isNaN(Cambio) || isNaN(Pago) ? 0 : (Cambio - Pago);" +
                    "\r\nvar formattedPagar = Pagar.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });" +
                    "\r\n$('[id*=txtPagar]').val(formattedPagar); \r\n}); \r\n}");

                sb.Append("if ($('[id*=txtnValorEnvio]').length > 0) {" +
                    "\r\n$('[id*=txtnValorEnvio]').on('input', function () {" +
                    "\r\nvar Envio = parseFloat($('[id*=txtnValorEnvio]').val().replace('.','').replace(',', '.'));" +
                    "\r\nvar EnvioReal = parseFloat($('[id*=txtnValorEnvioReal]').val().replace('.','').replace(',', '.'));" +
                    "\r\nvar Resultado = isNaN(Envio) || isNaN(EnvioReal) ? 0 : (-Envio + EnvioReal);" +
                    "\r\nvar formattedResultado = Resultado.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });" +
                    "\r\n$('[id*=txtnResultadoEnvio]').val(formattedResultado); \r\n}); \r\n}");

                sb.Append("$('[id*=txtnResultadoEnvio]').mask('0.000.000.009,99', { reverse: true });");
                sb.Append("$('[id*=txtnOcean_Air]').mask('0.000.000.009,99', { reverse: true });");
                sb.Append("$('[id*=txtnInsurance]').mask('0.000.000.009,99', { reverse: true });");
                sb.Append("$('[id*=txtnOther_Charges]').mask('0.000.000.009,99', { reverse: true });");


                sb.Append("if ($('[id*=txtnOcean_Air]').length > 0 && $('[id*=txtnInsurance]').length > 0 && $('[id*=txtnOther_Charges]').length > 0) {" +
                      "\r\n$('[id*=txtnOcean_Air], [id*=txtnInsurance], [id*=txtnOther_Charges]').on('input', function () {" +
                      "\r\nvar Ocean = parseFloat($('[id*=txtnOcean_Air]').val().replace('.','').replace(',', '.'));" +
                      "\r\nvar Insurance = parseFloat($('[id*=txtnInsurance]').val().replace('.','').replace(',', '.'));" +
                      "\r\nvar Other = parseFloat($('[id*=txtnOther_Charges]').val().replace('.','').replace(',', '.'));" +
                      "\r\nvar TotalEnvio = isNaN(Ocean) || isNaN(Insurance) || isNaN(Other) ? 0 : Ocean + Insurance + Other;" +
                      "\r\nvar formattedTotalEnvio = TotalEnvio.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });" +
                      "\r\n$('[id*=txtnTotalEnvio]').val(formattedTotalEnvio);" +
                      "\r\n$('[id*=txtnTotalEnvio]').mask('0.000.000.009,99', { reverse: true }); \r\n}); \r\n}");

                sb.Append("$('[id*=txtnTotalEnvio]').mask('0.000.000.009,99', { reverse: true });");

                sb.Append("$('[id*=txtnValorGarantia]').mask('0.000.000.009,99', { reverse: true });");
                Scripts.MascaraDatas_ComDatePicker(Page, txtdtGarantia.ClientID);

                sb.Append("if ($('[id*=txtnOcean_Air]').length > 0 && $('[id*=txtnInsurance]').length > 0 && $('[id*=txtnOther_Charges]').length > 0) {" +
                      "\r\n$('[id*=txtnOcean_Air], [id*=txtnInsurance], [id*=txtnOther_Charges]').on('input', function () {" +
                      "\r\nvar Ocean = parseFloat($('[id*=txtnOcean_Air]').val().replace('.','').replace(',', '.'));" +
                      "\r\nvar Insurance = parseFloat($('[id*=txtnInsurance]').val().replace('.','').replace(',', '.'));" +
                      "\r\nvar Other = parseFloat($('[id*=txtnOther_Charges]').val().replace('.','').replace(',', '.'));" +
                      "\r\nvar TotalEnvio = isNaN(Ocean) || isNaN(Insurance) || isNaN(Other) ? 0 : Ocean + Insurance + Other;" +
                      "\r\nvar formattedTotalEnvio = TotalEnvio.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });" +
                      "\r\n$('[id*=txtnVlrServicos]').val(formattedTotalEnvio); \r\n}); \r\n}");

                sb.Append("$('[id*=txtnPesoEnvio]').mask('0000000000009,9999', { reverse: true });");
                sb.Append("$('[id*=txtnComprimentoEnvio]').mask('0009,9999', { reverse: true });");
                sb.Append("$('[id*=txtnAlturaEnvio]').mask('0009,9999', { reverse: true });");
                sb.Append("$('[id*=txtnLarguraEnvio]').mask('0009,9999', { reverse: true });");

                sb.Append("$('[id*=txtPeso]').mask('0009,9999', { reverse: true });");
                sb.Append("$('[id*=txtLargura]').mask('0009,9999', { reverse: true });");
                sb.Append("$('[id*=txtComprimento]').mask('0009,9999', { reverse: true });");
                sb.Append("$('[id*=txtAltura]').mask('0009,9999', { reverse: true });");
                sb.Append("$('[id*=txtQuantidade]').mask('00099999', { reverse: true });");

                sb.Append("if ($('[id*=txtnLarguraEnvio]').length > 0 && $('[id*=txtnComprimentoEnvio]').length > 0 && $('[id*=txtnAlturaEnvio]').length > 0) {" +
                      "\r\n$('[id*=txtnLarguraEnvio], [id*=txtnComprimentoEnvio], [id*=txtnAlturaEnvio]').on('input', function () {" +
                      "\r\nvar $row = $(this).closest('tr');" +
                      "\r\nvar Largura = parseFloat($row.find('[id*=txtnLarguraEnvio]').val().replace(',', '.'));" +
                      "\r\nvar Comprimento = parseFloat($row.find('[id*=txtnComprimentoEnvio]').val().replace(',', '.'));" +
                      "\r\nvar Altura = parseFloat($row.find('[id*=txtnAlturaEnvio]').val().replace(',', '.'));" +
                      "\r\nvar Total = isNaN(Largura) || isNaN(Comprimento) || isNaN(Altura) ? 0 : Largura * Comprimento * Altura;" +
                      "\r\n$row.find('[id*=nVolume]').text(Total.toFixed(4).replace('.', ',')); \r\n}); \r\n}");

                sb.Append("$('[id*=nValorPagamento_Info_Pag]').mask('0.000.000.009,99', { reverse: true });");
                sb.Append("$('[id*=nValorGarantia]').mask('0.000.000.009,99', { reverse: true });");
            }

            sb.Append("$('[id*=txtnVlrProdutos]').mask('0.000.000.009,99', { reverse: true });");  //Agnes Partal - 20/06/2024 - email do dia 20/06/2024 15:30

            if (hddidTipo.Value == "7")
            {
                sb.Append("$(document).ready(function () {\r\n");
                sb.Append("     var $dtInicial = $('[id*=txtdtEntrega]');\r\n");
                sb.Append("     $dtInicial.datepicker({\r\n");
                sb.Append("         autoclose: true,\r\n");
                sb.Append("         format: 'dd/mm/yyyy',\r\n");
                sb.Append("         language: 'pt-BR'\r\n");
                sb.Append("     }).on('changeDate', function (e) {\r\n");
                sb.Append("         if ($(this).data('previous') !== $(this).val()) {\r\n");
                sb.Append("                 $(this).data('previous', $(this).val());\r\n");
                sb.Append("                 __doPostBack($dtInicial.attr('name'), '');\r\n");
                sb.Append("         }\r\n");
                sb.Append("     }).mask('99/99/9999');\r\n");
                sb.Append("     $dtInicial.data('previous', $dtInicial.val());\r\n");
                sb.Append("});\r\n\r\n");

                sb.Append("$('[id*=Item_GV_txtnICMS]').mask('009,99', { reverse: true });");
                sb.Append("$('[id*=Item_GV_txtnIPI]').mask('009,99', { reverse: true });");

                sb.Append("$('[id*=Tabela_txtnQuantidade]').mask('0.000.000.009,99', { reverse: true });");
                sb.Append("$('[id*=Tabela_txtnValorUnitario]').mask('0.000.000.009,99', { reverse: true });");
                sb.Append("$('[id*=Tabela_txtnICMS]').mask('009,99', { reverse: true });");
                sb.Append("$('[id*=Tabela_txtnIPI]').mask('009,99', { reverse: true });");
                sb.Append("$(function() {$('[id*=Tabela_txtdtPrevisao]').datepicker({autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");

                sb.Append("if ($('[id*=Tabela_txtnValorUnitario]').length > 0) {" +
                      "\r\n$('[id*=Tabela_txtnValorUnitario]').on('input', function () {" +
                      "\r\nvar $row = $(this).closest('tr');" +
                      "\r\nvar ValorTabela = parseFloat($row.find('.Tabela_Valor').text().replace('.', '').replace(',', '.'));" +
                      "\r\nvar ValorAtual = parseFloat($row.find('[id*=Tabela_txtnValorUnitario]').val().replace('.', '').replace(',', '.'));" +
                      "\r\nvar diferenca = isNaN(ValorTabela) || isNaN(ValorAtual) ? 0 : ValorAtual - ValorTabela;" +
                      "\r\nvar resultado = isNaN(diferenca) || isNaN(ValorTabela) ? 0 : diferenca / ValorTabela;" +
                      "\r\nvar porcentagem = isNaN(resultado) ? 0 : resultado * 100;" +
                      "\r\n$row.find('[id*=nPorcentagem]').text(porcentagem.toFixed(2).replace('.', ',')); \r\n}); \r\n}");
            }

            Scripts.AplicaMultiSelect(Page, ddlDocumentoEmpresa.ID, false, "Selecione o Documento da Empresa", "Documentos", "Todos Documentos", true);
            Scripts.AplicaMultiSelect(Page, ddlDocumentoColaboradores.ID, false, "Selecione o Documento do Colaborador", "Documentos", "Todos Documentos", true);

            sb.Append("$v192(function() {");
            sb.Append("$v192(\"#dialog_Apagar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"dialog_Apagar\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");

            sb.Append("$v192('.ExcluirArquivo').click(function(e) {");
            //sb.Append("console.log('aqui');");
            sb.Append("e.preventDefault();");
            sb.Append("$('[id*=hddExcluirArquivo]').val($(this).closest('tr').find('.idContador').text());");
            sb.Append("$v192('#dialog_Apagar').dialog('open');");
            sb.Append("});");
            sb.Append("});");

            sb.Append("$v192(function() {");
            sb.Append("$v192(\"#dialog_Aprovar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"dialog_Aprovar\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");

            sb.Append("$v192('.AprovarDoc').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$('[id*=hddAprovarDoc]').val($(this).closest('tr').find('.idArquvoSTSO').text());");
            sb.Append("$v192('#dialog_Aprovar').dialog('open');");
            sb.Append("});");
            sb.Append("});");

            sb.Append("$('.Todos').change(function(){ var isChecked = $(this).find('input').is(':checked'); $('.Individual').find('input').prop('checked', isChecked); $('.ArquivoIndividual').find('input').prop('checked', isChecked);});");
            sb.Append("$('.Individual').change(function(){ var isChecked = $(this).find('input').is(':checked'); var arquivo = $(this).closest('tr').find('.composicaoLinha').data('div-id'); $('[id*='+arquivo+']').find('.ArquivoIndividual').find('input').prop('checked', isChecked); });");

            sb.Append("$v192('.btExcluir').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("var hddChecked = [];");
            sb.Append("$v192('.ArquivoIndividual').each(function() {");
            sb.Append("var checkboxId = $v192(this).closest('tr').find('.idArquvoSTSO').text();");
            sb.Append("if ($v192(this).find('input').is(':checked')) {");
            sb.Append("hddChecked.push(checkboxId);");
            sb.Append("}");
            sb.Append("});");
            sb.Append("if (hddChecked.length > 0) {");
            sb.Append("$('[id*=hddChecked]').val(hddChecked.join(','));");
            sb.Append("__doPostBack('dialog_CheckArquivo', '');");
            sb.Append("} else {");
            sb.Append("$('[id*=hddChecked]').val('');");
            sb.Append("__doPostBack('dialog_CheckArquivo', '');");
            sb.Append("}");
            sb.Append("});");

            sb.Append("$v192('.btAprovar').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("var hddAprovarDoc = [];");
            sb.Append("$v192('.ArquivoIndividual').each(function() {");
            sb.Append("var checkboxId = $v192(this).closest('tr').find('.idArquvoSTSO').text();");
            sb.Append("if ($v192(this).find('input').is(':checked')) {");
            sb.Append("hddAprovarDoc.push(checkboxId);");
            sb.Append("}");
            sb.Append("});");
            sb.Append("if (hddAprovarDoc.length > 0) {");
            sb.Append("$('[id*=hddAprovarDoc]').val(hddAprovarDoc.join(','));");
            sb.Append("__doPostBack('dialog_AprovarArquivo', '');");
            sb.Append("} else {");
            sb.Append("$('[id*=hddAprovarDoc]').val('');");
            sb.Append("__doPostBack('dialog_AprovarArquivo', '');");
            sb.Append("}");
            sb.Append("});");

            sb.Append("$v192('.cmdGerarLinkSTSO').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("var hddsidArquivo = [];");
            sb.Append("$v192('.ArquivoIndividual').each(function() {");
            sb.Append("var checkboxId = $v192(this).closest('tr').find('.idArquvoSTSO').text();");
            sb.Append("if ($v192(this).find('input').is(':checked')) {");
            sb.Append("hddsidArquivo.push(checkboxId);");
            sb.Append("}");
            sb.Append("});");
            sb.Append("if (hddsidArquivo.length > 0) {");
            sb.Append("$('[id*=hddsidArquivo]').val(hddsidArquivo.join(','));");
            sb.Append("__doPostBack('dialog_DonwloadArquivo', '');");
            sb.Append("} else {");
            sb.Append("$('[id*=hddsidArquivo]').val('');");
            sb.Append("__doPostBack('dialog_DonwloadArquivo', '');");
            sb.Append("}");
            sb.Append("});");

            sb.Append("$v192('.DownloadSTSO').click(function(e) {");
            sb.Append("    e.preventDefault();");
            sb.Append("    $('[id*=hddDownloadSTSO]').val($(this).closest('tr').find('.sChave').text());");
            sb.Append("    __doPostBack('dialog_DownloadSTSO', '');");
            sb.Append("});");

            sb.Append("$v192('.EnviarEmail').click(function(e) {");
            sb.Append("    e.preventDefault();");
            sb.Append("    $('[id*=hddidLink]').val($(this).closest('tr').find('.idLink').text());");
            sb.Append("    $('[id*=hddsChave]').val($(this).closest('tr').find('.sChave').text());");
            sb.Append("    __doPostBack('dialog_EnviarEmail', '');");
            sb.Append("});");

            sb.Append("$v192('[id*=sClienteFinalswt_idSwitch]').click(function(e) {");
            sb.Append("     var div = '#' + $(this).attr('id').replace('sClienteFinalswt_idSwitch', 'sClienteFinalswt_hddSwitch');");
            sb.Append("     var valor = $(div).val() == 'S' ? 'N' : 'S';");
            sb.Append("     $(div).val(valor);");
            sb.Append("     if (valor === 'N') {");
            sb.Append("         $('#" + div_idClienteFinal.ClientID + "').addClass('visible');");
            sb.Append("     } else {");
            sb.Append("         $('#" + div_idClienteFinal.ClientID + "').removeClass('visible');");
            sb.Append("     }");
            sb.Append("});");

            sb.Append("$v192('[id*=ComissaoPaga_idSwitch]').click(function(e) {");
            sb.Append("     var div = '#' + $(this).attr('id').replace('ComissaoPaga_idSwitch', 'ComissaoPaga_hddSwitch');");
            sb.Append("     $(div).val($(div).val() == 'N' ? 'S' : 'N');");
            sb.Append("});");

            sb.Append("$('.composicaoLinha').addClass('fa fa-plus');\r\n");
            sb.Append("$('.composicaoLinha').click(function() {\r\n");
            sb.Append("     var icon = $(this);\r\n");
            sb.Append("     var divId = $(this).data('div-id');\r\n");
            sb.Append("     var current = $('#' + divId).css('display');\r\n");
            sb.Append("     if (current == 'none') {\r\n");
            sb.Append("         $('#' + divId).show('slow');\r\n");
            sb.Append("         icon.removeClass('fa fa-plus').addClass('fa fa-minus');\r\n");
            sb.Append("     } else {\r\n");
            sb.Append("         $('#' + divId).hide('slow');\r\n");
            sb.Append("         icon.removeClass('fa fa-minus').addClass('fa fa-plus');\r\n");
            sb.Append("     }\r\n");
            sb.Append("     return false;\r\n");
            sb.Append("});\r\n\r\n");

            return sb.ToString();
        }

        void RegistraScript(string sScript)
        {
            sScript = RetornarScripts() + sScript;
            ScriptManager.RegisterStartupScript(Page, GetType(), "js_ScriptPagina", sScript, true);
        }

        [WebMethod]
        public static string[] GetProdutos(string sDscProduto, string idPesquisa, string idPais, string sidParceiro, string idUsuario, string tipo)
        {
            List<string> lstProdutos = new List<string>();
            if (sDscProduto.Length > 3)
            {
                SqlDataReader sdr = ExecutarDataReader("sp_Select 'FLOW_Produtos', '" + idPesquisa + "', 'S', '" + sDscProduto + "', @idPais = " + idPais + ", @sidParceiro = '" + sidParceiro + "', @idUsuario = " + idUsuario + ", @sTipo = '" + tipo + "'");
                while (sdr.Read())
                {
                    lstProdutos.Add(string.Format("{2}|{0}|{1}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}", sdr["idItem"], sdr["sCodigo"], sdr["sDscProduto"], sdr["sUnidade"], sdr["sDscCategoriaVendas"], sdr["idNCM"], sdr["sCodigoNCM"], sdr["sTipoProduto_Servico"], sdr["sDscProdutoIdioma"], sdr["nTotal"], sdr["sUnidadeEntrega"]));// Modificado no Merge
                }
                sdr.Close();
            }

            return lstProdutos.ToArray();
        }

        #endregion
    }
}