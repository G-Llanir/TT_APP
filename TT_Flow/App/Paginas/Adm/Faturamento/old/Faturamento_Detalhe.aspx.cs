using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using TT.FrameWork;
using TT_Flow.FrameWork;
using TT_Flow.App.Controles;
using static System.IO.Path;
using static TT.FrameWork.Arquivo;
using static TT.FrameWork.BD;
using static TT.FrameWork.BD.Retorno;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Grid;
using static TT.FrameWork.Identity;
using static TT.FrameWork.Funcoes_NFe.NFS;

namespace TT_Flow.App.Paginas.Adm.Faturamento
{
    public partial class Faturamento_Detalhe : Page
    {
        #region | Classes

        string sTituloPagina = "Faturamento";

        string sProcedure = "sp_Manipula_tbl_Flow_Adm_Faturamento";
        string sProcedure_OPI = "sp_Manipula_FLow_WMS_OPI";
        string sProcedure_XML = "sp_Manipula_tbl_Flow_XML_NFe";
        string sProcedure_Pedidos = "sp_Manipula_tbl_Flow_Pedidos";
        string sProcedure_Arquivos = "sp_Manipula_tbl_Flow_Arquivos";
        string sProcedure_Contas_Receber = "sp_Manipula_tbl_Flow_Adm_Contas_Receber";

        int nColuna_idXML = 0;
        int nColuna_idEnvioOPI = 1;
        int nColuna_sTipo = 3;
        int nColuna_sChave = 6;
        int nColuna_Status = 7;
        int nColuna_idFaturamento = 10;
        int nColuna_nNumeroCartaCorrecao = 11;
        int nColuna_Cancelamento = 12;
        int nColuna_sChaveNFe = 13;

        public string idContabil { get { if (ViewState["idContabil"] == null) ViewState["idContabil"] = "0"; return (string)ViewState["idContabil"]; } set => ViewState["idContabil"] = value; }
        public string sRecebimento { get { if (ViewState["sRecebimento"] == null) ViewState["sRecebimento"] = ""; return (string)ViewState["sRecebimento"]; } set => ViewState["sRecebimento"] = value; }
        public string sReceber { get { if (ViewState["sReceber"] == null) ViewState["sReceber"] = "S"; return (string)ViewState["sReceber"]; } set => ViewState["sReceber"] = value; }

        public DataTable dtEnvios { get => (DataTable)ViewState["dtEnvios"]; set => ViewState["dtEnvios"] = value; }

        public List<cls_LancamentoRec> bs_LancamentoRec
        {
            get { if (ViewState["bs_LancamentoRec"] == null) ViewState["bs_LancamentoRec"] = new List<cls_LancamentoRec>(); return (List<cls_LancamentoRec>)ViewState["bs_LancamentoRec"]; }
            set => ViewState["bs_LancamentoRec"] = value;
        }

        public List<cls_Impostos> bs_Impostos
        {
            get { if (ViewState["bs_Impostos"] == null) ViewState["bs_Impostos"] = new List<cls_Impostos>(); return (List<cls_Impostos>)ViewState["bs_Impostos"]; }
            set => ViewState["bs_Impostos"] = value;
        }

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["idPedido"] != null)
                {
                    ValidaPermissao(Permissao.Financeiro.Faturamento.Consultar, true);
                    Pesquisar("0", Request["idPedido"].ToString());
                }

                if (Request["id"] != null)
                {
                    ValidaPermissao(Permissao.Financeiro.Faturamento.Consultar, true);
                    Pesquisar(Request["id"].ToString(), "0");
                }

                if (Request["msg"] != null)
                {
                    if (Request["sFaturamento"] != "S") MensagemPagina.MostraMensagem_Sucesso("Faturamento efetuado com sucesso!", false);
                    else MensagemPagina.MostraMensagem_Sucesso("Faturamento efetuado com sucesso em ambiente de teste!", false);

                    if (Request["id"] != null) Pesquisar(Request["id"].ToString(), "0");
                }
            }
            else
            {
                var requestTarget = Request["__EVENTTARGET"];
                if (requestTarget == "funcao_SAIR") DirecionaPagina("/app/dashboard.aspx");
                else if (requestTarget == "funcao_Editar") Pesquisar(hddidFaturamento.Value, "0");
            }

            RegistraScript("");
        }

        protected void Pesquisar(string idOPI, string idPedido)
        {
            div_Email.Visible = false;
            cmdOPI.Visible = false;
            cmdPedido.Visible = false;

            try
            {
                if (idPedido != "0" || idOPI != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string> { { "@sFuncao", "DETALHES_FATURAMENTO" } };
                    if (idOPI != "0") vParametros.Add("@idOPI", idOPI);
                    if (idPedido != "0") vParametros.Add("@idPedido", idPedido);

                    DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros, out string sql);

                    if (ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        hddidFaturamento.Value = DATASET(dsPesquisa, "idPedido");
                        hddidEmpresa.Value = DATASET(dsPesquisa, "idEmpresa");
                        hddsCaminho_UniNFe.Value = DATASET(dsPesquisa, "sCaminho_UniNFe");
                        hddidCondicaoPagamento.Value = DATASET(dsPesquisa, "idCondicaoPagamento");
                        hddidCliente.Value = DATASET(dsPesquisa, "idCliente");
                        txtdtPedido.Text = DATASET(dsPesquisa, "dtPedido");
                        txtnNumeroPedidoCliente.Text = DATASET(dsPesquisa, "sPedidoCliente");
                        hplPedido.Text = DATASET(dsPesquisa, "nNumeroPedido").PadLeft(6, '0');
                        hplPedido.NavigateUrl = string.Format("/App/Paginas/Pedidos_Detalhe.aspx?id={0}&sTp=2", DATASET(dsPesquisa, "idPedido"));
                        hddnNumeroPedido.Value = DATASET(dsPesquisa, "nNumeroPedido");
                        txtnControle.Text = DATASET(dsPesquisa, "nControleTT");
                        txtsReferencia.Text = DATASET(dsPesquisa, "sReferencia");
                        hddsEmpreitada.Value = DATASET(dsPesquisa, "sEmpreitada");
                        hddidOPI.Value = idOPI;
                        hddidPedido.Value = idPedido;
                        cmdSalvar.Visible = idOPI != "0";

                        if (idOPI != "0")
                        {
                            foreach (DataRow row in dsPesquisa.Tables[1].Rows)
                            {
                                if (row["sDscTipoObjeto"].ToString() != "Cancelada") hddsGerarNFe.Value = row["sGerarNFe"].ToString();
                            }
                        }

                        if (idPedido != "0") sRecebimento = DATASET(dsPesquisa, "sRecebimento");
                        else sRecebimento = DATASET(dsPesquisa, 2, 0, "sRecebimento");

                        string sFinalizada = "N";
                        if (idOPI != "0")
                        {
                            bool Finalizada = false;
                            if (dsPesquisa.Tables.Count > 1)
                            {
                                foreach (DataRow row in dsPesquisa.Tables[1].Rows)
                                {
                                    if (row["sChave"].ToString() != "") sFinalizada = "S";
                                    if (row["sFaturamento"].ToString() == "S") Finalizada = true;
                                }

                                cmdFaturar.Visible = !Finalizada;
                            }
                        }
                        else if (idPedido != "0" && !string.IsNullOrEmpty(DATASET(dsPesquisa, 2, 0, "sChaveNFE"))) sFinalizada = "S";

                        if (sFinalizada == "N")
                        {
                            aba_ContasReceber.Visible = false;
                            aba_PDF.Visible = false;
                        }
                        else
                        {
                            ContasReceber();

                            aba_PDF.Visible = true;

                            gvDocumentosFiscais.DataSource = dsPesquisa.Tables[idPedido != "0" ? 2 : 3];
                            gvDocumentosFiscais.DataBind();
                        }

                        if (idOPI != "0") txtsDscStatus.Text = DATASET(dsPesquisa, "sDscStatus");

                        ddlidCliente.AdicionarItens(DATASET(dsPesquisa, "idCliente"), (DATASET(dsPesquisa, "idCliente"), DATASET(dsPesquisa, "sRazaoSocial")));
                        ddlEmpresa.AdicionarItens(DATASET(dsPesquisa, "idEmpresa"), (DATASET(dsPesquisa, "idEmpresa"), DATASET(dsPesquisa, "sDscEmpresaFaturamento")));
                        ddlidFluxo.AdicionarItens(DATASET(dsPesquisa, "idFluxo"), (DATASET(dsPesquisa, "idFluxo"), DATASET(dsPesquisa, "sDscFluxo")));
                        ddlidCondicaoPagamento.AdicionarItens(DATASET(dsPesquisa, "idCondicaoPagamento"), (DATASET(dsPesquisa, "idCondicaoPagamento"), DATASET(dsPesquisa, "sDscCondicaoPagamento")));
                        txtsEnderecoEntrega.Text = DATASET(dsPesquisa, "sEnderecoEntrega");
                        txtsDscTipoFaturamento.Text = DATASET(dsPesquisa, "sDscTipoFaturamento");

                        decimal.TryParse(DATASET(dsPesquisa, "nVlrProdutos"), out decimal Produtos);
                        if (dsPesquisa.Tables[0].Rows.Count > 1) decimal.TryParse(dsPesquisa.Tables[0].AsEnumerable().Sum(row => row.Field<decimal>("nVlrProdutos")).ToString(), out Produtos);

                        decimal.TryParse(DATASET(dsPesquisa, "vlrFaturado"), out decimal vlrFaturado);
                        decimal.TryParse(DATASET(dsPesquisa, "nVlrServicos"), out decimal Servico);
                        decimal total = Produtos + Servico;
                        decimal vlrAFaturar = total - vlrFaturado;

                        txtvlrFaturado.Text = vlrFaturado.ToString("N2");
                        txtnVlrProdutos.Text = Produtos.ToString("N2");
                        txtnVlrServico.Text = Servico.ToString("N2");
                        txtnVlrTotal.Text = total.ToString("N2");
                        txtvlrAFaturar.Text = vlrAFaturar.ToString("N2");

                        if (vlrAFaturar <= 0)
                        {
                            txtvlrFaturado.Classe = "form-control form-success";

                            if (idPedido != "0") cmdFaturar.Visible = false;
                            if (idOPI != "0" && DATASET(dsPesquisa, "sFinalizada") == "S") cmdFaturar.Visible = false;
                        }
                        else
                        {
                            txtvlrFaturado.Classe = "form-control form-warning";
                            cmdFaturar.Visible = true;
                        }
						
                        lblTituloPagina.Text = string.Format("Faturamento Pedido - {0}", DATASET(dsPesquisa, "sReferencia"));
                        BreadCrumb.TitulodaPagina = string.Format("Faturamento Pedido - {0}", DATASET(dsPesquisa, "sReferencia"));
                        lblTitulo_SalvarFaturamento.Text = "Confirmar o " + lblTituloPagina.Text + "?";

                        if (idOPI != "0")
                        {
                            cmdOPI.Visible = true;
                            cmdOPI.NavigateUrl = $"/App/Paginas/WMS/OPI_Detalhe.aspx?id={idOPI}";

                            Popular_Itens(dsPesquisa);
                            DIV_vlrFaturamento.Visible = false;
                        }
                        else if (idPedido != "0")
                        {
                            cmdPedido.Visible = true;
                            cmdPedido.NavigateUrl = $"/App/Paginas/Pedidos_Detalhe.aspx?id={idPedido}&sTp=2";

                            SwitchAtivo_Retencao_IR.Definir("N", "IR Retido?", "N");
                            SwitchAtivo_Retencao_ISS.Definir("N", "ISS Retido?", "N");
                            SwitchAtivo_Retencao_INSS.Definir("N", "INSS Retido?", "N");
                            SwitchAtivo_Retencao_CSLL.Definir("N", "CSLL Retido?", "N");
                            SwitchAtivo_Retencao_PIS.Definir("N", "PIS Retido?", "N");
                            SwitchAtivo_Retencao_COFINS.Definir("N", "COFINS Retido?", "N");

                            DIV_Status.Visible = false;
                            DIV_Envios.Visible = false;
                            DIV_SalvarCFOP.Visible = false;
                            DIV_vlrFaturamento.Visible = true;

                            gv_Faturamento.ShowFooter = true;
                            gv_Faturamento.DataSource = dsPesquisa.Tables[1];
                            gv_Faturamento.DataBind();

                            SomarColunas(gv_Faturamento, false, Formatação.Numero, 4);

                            if (gv_Faturamento.Rows.Count <= 0) MensagemPagina.MostraMensagem_Erro("Nenhum Faturamento encontrado!<br />Por favor valide a Aba Faturamento deste Pedido, é necessário que ao menos um Faturamento esteja configurado e vinculado à um Serviço e uma Empresa!");

                            ContasReceber();
                        }
                    }
                    else throw new Exception(sErro);
                }
                else
                {
                    BreadCrumb.TitulodaPagina = string.Format("Nova {0}", sTituloPagina);
                    lblTituloPagina.Text = string.Format("Nova {0}", sTituloPagina);
                    lblTitulo_SalvarFaturamento.Text = "Confirma a Inclusão da Provisão?";
                    cmdGerarFaturamento.Text = "Faturar";
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        #endregion

        #region | Salvar

        protected void Salvar_Faturamento()
        {
            Lancamentos_Salvar();

            if (Validar())
            {
                try
                {
                    bool algumSelecionado = false;
                    string idEnvioOPI = "", nNF = "", nSerie = "0";

                    decimal valor = 0;
                    decimal.TryParse(hddvlrFaturamento.Value, out decimal valorAFaturar);
                    foreach (var Linha in bs_LancamentoRec)
                    {
                        valor += Linha.nValorLancamentoRec;
                    }

                    if (valor != valorAFaturar)
                        throw new Exception($"O valor Total dos Lançamentos deve ser igual ao Valor dos Produtos (R$ {valorAFaturar:N2})!");

                    if (hddidOPI.Value != "0")
                    {
                        foreach (GridViewRow row in gv_Envios.Rows)
                        {
                            RadioButton rbEnvio = (RadioButton)row.FindControl("rbEnvio");

                            if (rbEnvio != null && rbEnvio.Checked)
                            {
                                algumSelecionado = true;
                                idEnvioOPI = (row.FindControl("idEnvioOPI") as Label).Text;

                                string sUsarProdutosCliente = "N";
                                if (chksUsarProdutosCliente.Checked) sUsarProdutosCliente = "S";

                                Dictionary<string, string> vParametros = new Dictionary<string, string>
                                {
                                    { "@sFuncao", "SALVAR_TRANS_FATURAMENTO" },
                                    { "@idTransportadora", ddlidTranportadora.SelectedValue },
                                    { "@idModFrete", ddltranporte.SelectedValue },
                                    { "@sEsp", txtEspecie.Text },
                                    { "@sInfCpl", txtInfoComplementares.Text },
                                    { "@sInfAdFisco", txtInfoFisco.Text },
                                    { "@idEnvioOPI", idEnvioOPI },
                                    { "@sGerarNFe", "N" },
                                    { "@xPed", txtxPed.Text },
                                    { "@sUsarProdutosCliente", sUsarProdutosCliente }
                                };
                                DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

                                string filePath = Funcoes_NFe.XML_GerarNFE.GerarXML(hddidFaturamento.Value, hddnNumeroPedido.Value, Page, hddidEmpresa.Value, false, idEnvioOPI, chkExibirFabricante.Checked ? "S" : "N", sUsarProdutosCliente, false, "", "", "N", true);
                                string Arquivo = Server.MapPath("~/Download/" + filePath);

                                XDocument xdoc = XDocument.Load(Arquivo);
                                string chave = "",
                                        CNPJ = "",
                                        CNPJdest = "",
                                        xNome = "",
                                        xNomedest = "",
                                        xFant = "",
                                        xLgr = "",
                                        nro = "",
                                        UF = "",
                                        CEP = "",
                                        xPais = "",
                                        fone = "",
                                        Corpo = "";
                                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}nfeProc").FirstOrDefault() != null) Corpo = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}nfeProc").FirstOrDefault().ToString();
                                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}NFe").FirstOrDefault() != null) Corpo = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}NFe").FirstOrDefault().ToString();

                                var infNFeElement = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}infNFe").FirstOrDefault();
                                if (infNFeElement != null) chave = infNFeElement.Attribute("Id")?.Value?.Substring(3);

                                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}nNF").FirstOrDefault() != null)
                                    nNF = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}nNF").FirstOrDefault()?.Value;

                                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}serie").FirstOrDefault() != null)
                                    nSerie = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}serie").FirstOrDefault()?.Value;

                                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                                    CNPJ = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                                    CNPJdest = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                                    xNomedest = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;

                                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                                    xNome = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;

                                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xFant").FirstOrDefault() != null)
                                    xFant = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xFant").FirstOrDefault()?.Value;

                                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault() != null)
                                    xLgr = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault()?.Value;

                                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault() != null)
                                    nro = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault()?.Value;

                                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault() != null)
                                    UF = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value;

                                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault() != null)
                                    CEP = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault()?.Value;

                                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xPais").FirstOrDefault() != null)
                                    xPais = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xPais").FirstOrDefault()?.Value;

                                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}fone").FirstOrDefault() != null)
                                    fone = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}fone").FirstOrDefault()?.Value;

                                Dictionary<string, string> vParametrosGerenciador = new Dictionary<string, string>
                                {
                                    { "@sfuncao", "INCLUIR_XML" },
                                    { "@idTipoObjeto", "12" },
                                    { "@idObjeto", idEnvioOPI },
                                    { "@idOrigem", "1" },
                                    { "@sChaveNFe", chave },
                                    { "@sCNPJ_Emitente", CNPJ },
                                    { "@nSerieNF", nSerie },
                                    { "@nNumeroNF", nNF },
                                    { "@sXML", Corpo.ToString() },
                                    { "@sNome_Emitente", xNome },
                                    { "@sNomeFantasia_Emitente", xFant },
                                    { "@sLogradouro_Emitente", xLgr },
                                    { "@sNumero_Emitente", nro },
                                    { "@sUF_Emitente", UF },
                                    { "@sCEP_Emitente", CEP },
                                    { "@sPais_Emitente", xPais },
                                    { "@sTelefone_Emitente", fone },
                                    { "@sNome_Destinatario", xNomedest },
                                    { "@sTipoXML", "2" },
                                    { "@idArquivo", hddidArquivo.Value },
                                    { "@sEspelho", "S" }
                                };
                                ExecutarDataSet(sProcedure_XML, vParametrosGerenciador);
                            }
                        }
                    }

                    if (hddsFaturamento.Value != "S")
                    {
                        if (!algumSelecionado && hddidOPI.Value != "0") MensagemPagina.MostraMensagem_Erro("Selecione o envio que deseja faturar!");
                        else
                        {
                            Dictionary<string, string> vParametros = new Dictionary<string, string>
                            {
                                { "@sFuncao", "EFETUAR_FATURAMENTO" },
                                { "@nValorFaturamento", valor.ToString().Replace(",", ".") },
                                { "@idArquivo", hddidArquivo.Value },
                                { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                            };

                            if (hddidOPI.Value != "0") vParametros.Add("@idEnvioOPI", idEnvioOPI);
                            if (hddidPedido.Value != "0") vParametros.Add("@idPedido", hddidPedido.Value);
                            if (sReceber == "N") vParametros.Add("@sReceber", sReceber);

                            DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

                            string sFaturamentoTeste = DATASET(ExecutarDataSet(sProcedure_Contas_Receber, new Dictionary<string, string> { { "@sFuncao", "Parametros" } }), "sFaturamentoTeste");

                            if (sRecebimento == "S")
                            {
                                if (sReceber == "S")
                                {
                                    if (bs_LancamentoRec.Count > 0)
                                    {
                                        int numeroParcelaAtual = 1;
                                        foreach (var Linha in bs_LancamentoRec)
                                        {
                                            string sFuncao = sFaturamentoTeste == "S" ? "SALVAR_TESTE" : "SALVAR";

                                            Dictionary<string, string> vP = new Dictionary<string, string>
                                                {
                                                    { "@sFuncao", sFuncao },
                                                    { "@idContasReceber", "0" },
                                                    { "@nSaldo", Conversoes.Numerico(Linha.nValorLancamentoRec) },
                                                    { "@nValorOriginal", Conversoes.Numerico(Linha.nValorLancamentoRec) },
                                                    { "@dtVencimento", Linha.dtLancamentoRec.ToString() },
                                                    { "@nValorBruto", Conversoes.Numerico(Linha.nValorBrutoLancamentoRec) },
                                                    { "@sCodigo", nNF },
                                                    { "@dtEmissao", txtdtEmissao.Text.Replace("-", "/") },
                                                    { "@sDocumento", txtnControle.Text },
                                                    { "@idContabil", idContabil },
                                                    { "@idEmpresa", hddidEmpresa.Value },
                                                    { "@idCentroDeCusto", ddlidCentroDeCusto.SelectedValue },
                                                    { "@idMeioRecebimento", ddlidMeioRecebimento.SelectedValue },
                                                    { "@nParcelas", txtnParcelas.Text },
                                                    { "@sSituacao", "N" },
                                                    { "@idFormaRecebimento", Linha.idFormaPagamentoLancamentoRec.ToString() },
                                                    { "@idConta", Linha.idContaBancaria.ToString() },
                                                    { "@idCategoriaReceber", ddlidCategoriaReceber.SelectedValue },
                                                    { "@dtVencimentoOriginal", Linha.dtLancamentoRec.ToString() },
                                                    { "@sObservacaoGeral", txtsObservacaoGeral.Text },
                                                    { "@idParceiro", hddidCliente.Value },
                                                    { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                                                };

                                            if (hddidPedido.Value != "0") vP.Add("@idEnvioOPI", hddidEnvioOPI_idFaturamento.Value);
                                            else vP.Add("@idEnvioOPI", idEnvioOPI);

                                            if (bs_LancamentoRec.Count == 1) vP.Add("@sQuantidadeParcela", $"Única");
                                            else vP.Add("@sQuantidadeParcela", $"{numeroParcelaAtual} de {bs_LancamentoRec.Count}");
                                            numeroParcelaAtual += 1;

                                            ExecutarDataSet(sProcedure_Contas_Receber, vP);
                                        }
                                    }

                                    if (hddidPedido.Value != "0")
                                    {
                                        Dictionary<string, string> vParametrosFatu = new Dictionary<string, string>
                                            {
                                                { "@sFuncao", "ALTERAR_FATURAMENTO" },
                                                { "@sFaturado", "S" },
                                                { "@idFaturamento", hddidEnvioOPI_idFaturamento.Value }
                                            };
                                        ExecutarDataSet(sProcedure_Pedidos, vParametrosFatu);
                                    }
                                }

                                //if (hddidOPI.Value != "0") DirecionaPagina("App/Paginas/Adm/Faturamento/Faturamento_Detalhe.aspx?id=" + hddidOPI.Value + "&msg=S&sFaturamento=" + sFaturamentoTeste);
                                //if (hddidPedido.Value != "0") DirecionaPagina("App/Paginas/Adm/Faturamento/Faturamento_Detalhe.aspx?idPedido=" + hddidPedido.Value + "&msg=S");

                                Pesquisar(hddidOPI.Value, hddidPedido.Value);
                                Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
                                MensagemPagina.MostraMensagem_Sucesso("Espelho emitido com sucesso!");
                            }
                            else
                            {
                                DirecionaPagina("App/Paginas/Adm/Faturamento/Faturamento_Detalhe.aspx?id=" + hddidOPI.Value + "&msg=S&sFaturamento=" + sFaturamentoTeste);
                                MensagemPagina.MostraMensagem_Sucesso("XML emitido com sucesso!");
                            }
                        }
                    }
                    else MensagemPagina.MostraMensagem_Sucesso("XML emitido com sucesso!");
                }
                catch (Exception ex)
                {
                    if (ex.Message != "O thread estava sendo anulado.")
                    {
                        MensagemPagina_Modal_Receber.MostraMensagem_Erro(ex.Message);
                        Scripts.AbrirModal(Page, "Modal_Receber");
                    }
                    else MensagemPagina.MostraMensagem_Sucesso("XML emitido com sucesso!");
                }
            }
            else
            {
                Scripts.Mantem_AbaAtiva(Page, "NF-tab");
                Scripts.AbrirModal(Page, "Modal_Receber");
            }
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (GridViewRow row in gv_Envios.Rows)
                {
                    if (row.FindControl("gvItensEnvio") is GridView gvItensEnvio)
                    {
                        foreach (GridViewRow itemRow in gvItensEnvio.Rows)
                        {
                            Dictionary<string, string> vParametros = new Dictionary<string, string>
                            {
                                { "@sFuncao", "SALVAR_NUMEROPEDIDO" },
                                { "@idOPI", hddidOPI.Value },
                                { "@idProduto", itemRow.Cells[0].Text },
                                { "@sCodigoBarras", itemRow.Cells[6].Text },
                                { "@nPedidoCliente", (itemRow.FindControl("txtnPedidoCliente") as TextBox).Text },
                                { "@idCST", (itemRow.FindControl("ddlidCSTICMS") as DropDownList).SelectedValue },
                                { "@pRedBC", Request.Form[itemRow.FindControl("txtpReducao").UniqueID].Replace(",", ".") }
                            };
                            DataSet dsPesquisa = ExecutarDataSet(sProcedure_OPI, vParametros);
                        }
                    }
                }

                Pesquisar(hddidOPI.Value, "0");
                MensagemPagina.MostraMensagem_Sucesso("Salvo com sucesso!");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        #endregion

        #region | Envios

        public string NovaLinha(object id, string gridNome)
        {
            if (id != null && !string.IsNullOrEmpty(id.ToString())) return string.Format(@"</td></tr><tr id='tr{0}{1}' class='collapsed-row'><td></td><td colspan='100' style='padding:0px; margin:0px;'>", gridNome, id);
            else return string.Empty;
        }

        void Popular_Itens(DataSet ds)
        {
            dtEnvios = ds.Tables[2].Copy();
            gv_Envios.DataSource = ds.Tables[1].AsEnumerable().GroupBy(r => r.Field<int>("idEnvioOPI")).Select(g => g.First()).CopyToDataTable();
            gv_Envios.DataBind();
        }

        protected void gv_Envios_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.FindControl("gvItensEnvio") is GridView gv)
                {
                    DataTable dt = dtEnvios.Copy();
                    int contador = 0;
                    while (true)
                    {
                        if (dt.Rows[contador].Field<int>("idEnvioOPI") != int.Parse((e.Row.FindControl("idEnvioOPI") as Label).Text))
                            dt.Rows.RemoveAt(contador);
                        else contador++;

                        if (contador >= dt.Rows.Count)
                            break;
                    }

                    gv.DataSource = dt;
                    gv.DataBind();

                    if (e.Row.Cells[10].Text == "S")
                    {
                        foreach (GridViewRow itemRow in gv.Rows)
                        {
                            if (itemRow.FindControl("txtnPedidoCliente") is TextBox txtnPedidoCliente) txtnPedidoCliente.ReadOnly = true;
                            if (itemRow.FindControl("txtpReducao") is TextBox txtpReducao) txtpReducao.ReadOnly = true;
                            if (itemRow.FindControl("ddlidCSTICMS") is DropDownList ddlidCSTICMS) ddlidCSTICMS.Attributes.Add("disabled", "disabled");
                        }
                    }
                }

                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
                e.Row.FindControl("rbEnvio").Visible = e.Row.Cells[10].Text != "S";
            }

            EsconderColunas(e, 10, 11, 13);
        }

        protected void gvItensEnvio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlidCSTICMS = e.Row.FindControl("ddlidCSTICMS") as DropDownList;
                Popula_Combo(ddlidCSTICMS, $"sp_Select 'tbl_Flow_Adm_CST', @sPesquisa=ICMS", "idCST", "sCST", false, "Selecione o CST", "0");

                TextBox txtpReducao = e.Row.FindControl("txtpReducao") as TextBox;

                string valorAtual = DataBinder.Eval(e.Row.DataItem, "idCST")?.ToString();
                if (!string.IsNullOrEmpty(valorAtual))
                    ddlidCSTICMS.SelectedValue = valorAtual;

                if (e.Row.Cells[13].Text != "S")
                {
                    if (ddlidCSTICMS.SelectedValue != "3")
                        txtpReducao.ReadOnly = true;
                    else txtpReducao.ReadOnly = false;
                }
                else txtpReducao.ReadOnly = true;

                if (e.Row.Cells[7].Text == "&nbsp;")
                {
                    e.Row.Cells[7].Text = "N/D";
                    e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[7].CssClass = "warning";
                }
            }
        }

        #endregion

        #region | Modal Contas Receber

        private bool Validar()
        {
            if (sReceber == "S")
            {
                if (hddsFaturamento.Value != "S")
                {
                    string sMensagem = "";

                    foreach (GridViewRow item in dtgLancamento.Rows)
                    {
                        TextBox txtdtLancamentoRec_Linha = (TextBox)item.FindControl("txtdtLancamentoRec");
                        DropDownList ddlidFormaPagamentoLancamentoRec_Linha = (DropDownList)item.FindControl("ddlidFormaPagamentoLancamentoRec");

                        if (!decimal.TryParse((item.FindControl("txtnValorLancamentoRec") as TextBox).Text, out decimal valor) || valor <= 0) sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva o Valor Líquido de todos Lançamentos!";
                        if (!decimal.TryParse((item.FindControl("txtnValorBrutoLancamentoRec") as TextBox).Text, out decimal bruto) || bruto <= 0) sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva o Valor Bruto de todos Lançamentos!";
                        if (txtdtLancamentoRec_Linha.Text == "") sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione a Data Vencimento de todos Lançamentos!";
                        if (ddlidFormaPagamentoLancamentoRec_Linha.SelectedValue == "0") sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione a Forma de Recebimento de todos Lançamentos!";

                        if (!string.IsNullOrEmpty(sMensagem)) break;
                    }

                    if (ddlidCategoriaReceber.SelectedValue == "0") sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione a Categoria!";
                    if (ddlidMeioRecebimento.SelectedValue == "0") sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Tipo Recebimento!";
                    if (ddlidMeioRecebimento.SelectedValue != "1" && txtnParcelas.Text == "") sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva o N° de parcelas!";
                    if (hddidPedido.Value == "0" && ddltranporte.SelectedValue == "") sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o modo do frete!";

                    if (sMensagem != "")
                    {
                        MensagemPagina_Modal_Receber.MostraMensagem_Erro(sMensagem);
                        return false;
                    }

                    return true;
                }
                else return true;
            }
            else return true;
        }

        void PopulaCombos()
        {
            ddlidContabil.Popula_Combo("sp_Select 'Flow_CodigoContabil'", "idContabil", "sDscCodContabil", false, "Selecione o Código Contábil", "0");
            ddlidCentroDeCusto.Popula_Combo("sp_Select 'Flow_Adm_CentroDeCusto'", "idCentroDeCusto", "sDescricao", false, "Selecione um Centro de Custo", "0");
            ddlidMeioRecebimento.Popula_Combo("sp_Select 'tbl_Flow_Adm_MeioPagamento'", "idMeioPagamento", "sDscMeioPagamento", false, "Selecione um Tipo de Recebimento", "0");
            ddlidEmpresa.Popula_Combo("sp_Select 'Flow_empresa'", "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
            ddlidCategoriaReceber.Popula_Combo("sp_Select 'Flow_Adm_Contas_Receber_Categoria'", "idCategoriaReceber", "sDscCategoriaReceber", false, "Selecione a Categoria", "0");
            ddlidParceiro.Popula_Combo("sp_Select 'Flow_Clientes'", "idCliente", "sRazaoSocial", false, "Selecione o Cliente", "0");
            ddlidTranportadora.Popula_Combo("sp_Select 'Flow_Parceiro_Transportadora'", "idParceiro", "sRazaoSocial", false, "Selecione a Transportadora", "0");
        }

        void StatusDosCampos(bool bStatus)
        {
            string sStatusCombo = "disabled";
            if (!bStatus) sStatusCombo = "enabled";

            ddlidContabil.Attributes.Remove("disabled");
            ddlidContabil.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlidEmpresa.Attributes.Remove("disabled");
            ddlidEmpresa.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlidParceiro.Attributes.Remove("disabled");
            ddlidParceiro.Attributes.Add(sStatusCombo, sStatusCombo);
        }

        #region | Eventos

        protected void cmdFaturar_Click(object sender, EventArgs e)
        {
            try
            {
                div_Servicos.Visible = false;
                cmdEmitirNFS.Visible = false;
                aba_ItensDevolucao.Visible = false;

                PopulaCombos();
                ddlidContabil.SelectedValue = "0";
                ddlidCategoriaReceber.SelectedValue = "0";

                decimal.TryParse(txtvlrAFaturar.Text, out decimal vlrAFaturar);
                hddvlrFaturamento.Value = vlrAFaturar.ToString("N2");

                if (hddidOPI.Value != "0")
                {
                    string idEnvioOPI = "";

                    decimal.TryParse(txtnVlrProdutos.Text, out decimal vlrProdutos);
                    hddvlrFaturamento.Value = vlrProdutos.ToString("N2");

                    aba_NF.Visible = true;
                    div_Fatumento_NF.Visible = true;
                    aba_Transportadora.Visible = true;
                    aba_Informacoes.Visible = true;
                    aba_Impostos.Visible = true;
                    cmdSalvarEdicao.Visible = false;
                    DIV_ItensDevolucao.Visible = false;
                    btnSalvarDevolucao.Visible = false;

                    lblTitulo_Modal_Receber.InnerText = "NF-e - Envio " + hddidEnvioOPI.Value;

                    foreach (GridViewRow row in gv_Envios.Rows)
                    {
                        RadioButton rbEnvio = (RadioButton)row.FindControl("rbEnvio");

                        if (rbEnvio != null && (rbEnvio.Checked || gv_Envios.Rows.Count == 1))
                        {
                            idEnvioOPI = (row.FindControl("idEnvioOPI") as Label).Text;
                            hddidEnvioOPI.Value = (row.FindControl("idEnvioOPI") as Label).Text;
                            hddsFaturamento.Value = row.Cells[10].Text;
                        }

                        if ((row.FindControl("idEnvioOPI") as Label).Text == hddidEnvioOPI.Value)
                        {
                            rbEnvio.Checked = true;
                            hddsFaturamento.Value = row.Cells[10].Text;
                        }
                    }

                    if (hddidEnvioOPI.Value != "")
                    {
                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "VALOR_FATURAMENTO" },
                            { "@idEnvioOPI", hddidEnvioOPI.Value },
                            { "@idEmpresa", hddidEmpresa.Value },
                            { "@idCondicaoPagamento", hddidCondicaoPagamento.Value }
                        };
                        DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

                        if (dsPesquisa.Tables[2].Rows.Count <= 0)
                            throw new Exception($"A Condição de Pagamento não possui as Parcelas configuradas!<br /><a href='/App/Paginas/Manutencao/CondicaodePagamento_Detalhe.aspx?id={hddidCondicaoPagamento.Value}' target='_blank'><b>Configurar Parcelas |</b> {ddlidCondicaoPagamento.SelectedItem.Text}</a>");

                        if (DATASET(dsPesquisa, 1, 0, "nQtdParcelas") == "1")
                        {
                            Div_nParcelas.Visible = false;
                            ddlidMeioRecebimento.SelectedValue = "1";
                            hddidMeioRecebimento.Value = "1";
                        }
                        else
                        {
                            Div_nParcelas.Visible = true;
                            ddlidMeioRecebimento.SelectedValue = "2";
                            hddidMeioRecebimento.Value = "2";
                            txtnParcelas.Text = DATASET(dsPesquisa, 1, 0, "nQtdParcelas");
                        }

                        IncluirLancamento_Envio(dsPesquisa);

                        ddlidEmpresa.SelectedValue = hddidEmpresa.Value;
                        txtsDocumento.Text = hddnNumeroPedido.Value;
                        txtsCodigo.Text = hddnNumeroPedido.Value;
                        txtvlrFaturadoOriginal.Text = txtvlrFaturado.Text;

                        decimal.TryParse(DATASET(dsPesquisa, "nValorFaturamento"), out decimal vlrBruto);
                        txtnValorBruto.Text = vlrBruto.ToString("N2");
                        txtnValorOriginal.Text = vlrBruto.ToString("N2");
                        txtdtEmissao.Text = DateTime.Now.ToString("yyyy-MM-dd");

                        txtVolumes.Text = DATASET(dsPesquisa, 3, 0, "nQtdVolumes");
                        txtPesoLiquido.Text = DATASET(dsPesquisa, 3, 0, "nPesoLiquido");
                        txtPesoBruto.Text = DATASET(dsPesquisa, 3, 0, "nPesoBruto");
                        txtEspecie.Text = DATASET(dsPesquisa, 3, 0, "sEsp");
                        txtInfoFisco.Text = DATASET(dsPesquisa, 3, 0, "sInfAdFisco");
                        txtInfoComplementares.Text = DATASET(dsPesquisa, 3, 0, "sInfCpl");
                        txtxPed.Text = DATASET(dsPesquisa, 3, 0, "xPed");
                        ddltranporte.SelectedValue = DATASET(dsPesquisa, 3, 0, "idModFrete");
                        ddlidTranportadora.SelectedValue = DATASET(dsPesquisa, 3, 0, "idTransportadora");

                        if (ddlidTranportadora.SelectedValue != "0")
                        {
                            Dictionary<string, string> vParametrosTranportadora = new Dictionary<string, string>
                            {
                                { "@sFuncao", "CONSULTAR_Transportadora" },
                                { "@idCliente", ddlidTranportadora.SelectedValue }
                            };
                            DataSet dsPesquisaTranportadora = ExecutarDataSet(sProcedure_Pedidos, vParametrosTranportadora);

                            if (ValidarDataSet(dsPesquisaTranportadora, out string sErro))
                            {
                                div_InfoTransportadora.Visible = true;
                                DIV_IETrans.Visible = txtIETrans.Text != "";

                                txtCNPJTrans.Text = DATASET(dsPesquisaTranportadora, "sCPF_CNPJ");
                                txtNomeTrans.Text = DATASET(dsPesquisaTranportadora, "sRazaoSocial");
                                txtIETrans.Text = DATASET(dsPesquisaTranportadora, "sRG_IE");
                                txtEnderecoTrans.Text = DATASET(dsPesquisaTranportadora, "sLogradouro");
                                txtMunicipiotrans.Text = DATASET(dsPesquisaTranportadora, "sCidade");
                                txtUFTrans.Text = DATASET(dsPesquisaTranportadora, "sEstado");
                            }
                        }
                        else div_InfoTransportadora.Visible = false;

                        ddlidParceiro.SelectedValue = hddidCliente.Value;
                        txtvlrAFaturarOriginal.Text = (decimal.Parse(txtnVlrTotal.Text) - decimal.Parse(txtvlrFaturadoOriginal.Text)).ToString("N2");
                        txtvlrFaturadoOriginal.Classe = decimal.Parse(txtnVlrTotal.Text) == decimal.Parse(txtvlrFaturado.Text) ? "form-control form-success" : "form-control form-warning";

                        StatusDosCampos(true);
                        PopulaImpostos();

                        if (hddsFaturamento.Value != "S")
                        {
                            aba_NF.Visible = true;
                            chkExibirFabricante.Visible = true;
                            cmdGerarFaturamento.Visible = true;
                            txtEspecie.ReadOnly = false;
                            txtInfoFisco.ReadOnly = false;
                            txtInfoComplementares.ReadOnly = false;
                            ddltranporte.Attributes.Remove("disabled");
                            ddlidTranportadora.Attributes.Remove("disabled");

                            if (decimal.Parse(txtvlrAFaturar.Text) != 0) Scripts.AbrirModal(Page, "Modal_Receber");
                            else
                            {
                                sReceber = "N";
                                Salvar_Faturamento();
                            }
                        }
                        else
                        {
                            aba_NF.Visible = false;
                            aba_Transportadora.Visible = true;
                            chkExibirFabricante.Visible = false;
                            cmdGerarFaturamento.Visible = false;
                            txtEspecie.ReadOnly = true;
                            txtInfoFisco.ReadOnly = true;
                            txtInfoComplementares.ReadOnly = true;
                            ddltranporte.Attributes.Add("disabled", "disabled");
                            ddlidTranportadora.Attributes.Add("disabled", "disabled");

                            Scripts.AbrirModal(Page, "Modal_Receber");
                        }
                    }
                    else MensagemPagina.MostraMensagem_Erro("É necessário selecionar um Envio para Faturar!");
                }
                else if (hddidPedido.Value != "0")
                {
                    decimal vlrFaturamento = 0;
                    string idFaturamento = "0", sFaturado = "", sDscFaturamento = "";

                    decimal.TryParse(txtnVlrServico.Text, out decimal vlrServicos);
                    hddvlrFaturamento.Value = vlrServicos.ToString("N2");

                    foreach (GridViewRow row in gv_Faturamento.Rows)
                    {
                        RadioButton rbFaturamento = (RadioButton)row.FindControl("rbFaturamento");

                        if (rbFaturamento != null && (rbFaturamento.Checked || gv_Faturamento.Rows.Count == 1))
                        {
                            hddidEnvioOPI_idFaturamento.Value = row.Cells[1].Text;
                            idFaturamento = row.Cells[1].Text;
                            sDscFaturamento = row.Cells[2].Text;
                            sFaturado = row.Cells[6].Text;
                            vlrFaturamento = decimal.Parse(row.Cells[4].Text);
                            hddvlrFaturamento.Value = vlrFaturamento.ToString("N2");
                            txtsDscServico.Text = (row.Cells[3].Controls[0] as HyperLink).Text;
                            break;
                        }
                    }

                    if (vlrAFaturar == 0)
                    {
                        MensagemPagina.MostraMensagem_Aviso("Este pedido já foi totalmente faturado!");
                        txtvlrFaturadoOriginal.Classe = "form-control form-success";
                    }
                    else if (vlrAFaturar < 0)
                    {
                        MensagemPagina.MostraMensagem_Aviso("<b>Erro:</b> Este pedido foi faturado acima do valor para Faturar!");
                        txtvlrFaturadoOriginal.Classe = "form-control form-danger";
                    }

                    if (idFaturamento != "0")
                    {
                        aba_NF.Visible = true;
                        aba_Transportadora.Visible = false;
                        aba_Informacoes.Visible = false;
                        aba_Impostos.Visible = false;
                        cmdSalvarEdicao.Visible = false;
                        btnSalvarDevolucao.Visible = false;
                        cmdGerarFaturamento.Visible = false;
                        div_Servicos.Visible = true;
                        cmdEmitirNFS.Visible = true;

                        SwitchAtivo_Retencao_IR.Definir("N", "IR Retido?", "N");
                        SwitchAtivo_Retencao_ISS.Definir("N", "ISS Retido?", "N");
                        SwitchAtivo_Retencao_INSS.Definir("N", "INSS Retido?", "N");
                        SwitchAtivo_Retencao_CSLL.Definir("N", "CSLL Retido?", "N");
                        SwitchAtivo_Retencao_PIS.Definir("N", "PIS Retido?", "N");
                        SwitchAtivo_Retencao_COFINS.Definir("N", "COFINS Retido?", "N");

                        lblTitulo_Modal_Receber.InnerText = $"NFS-e - {sDscFaturamento}";

                        ddlidEmpresa.SelectedValue = hddidEmpresa.Value;
                        txtsDocumento.Text = hddnNumeroPedido.Value;
                        txtsCodigo.Text = hddnNumeroPedido.Value;
                        txtvlrFaturadoOriginal.Text = txtvlrFaturado.Text;

                        decimal vlrBruto = Convert.ToDecimal(txtnVlrTotal.Text);
                        txtnValorBruto.Text = vlrBruto.ToString("N2");
                        txtnValorOriginal.Text = vlrBruto.ToString("N2");
                        txtdtEmissao.Text = DateTime.Now.ToString("yyyy-MM-dd");
                        ddlidParceiro.SelectedValue = hddidCliente.Value;
                        txtvlrAFaturarOriginal.Text = (vlrBruto - decimal.Parse(txtvlrFaturadoOriginal.Text)).ToString("N2");
                        Div_nParcelas.Visible = false;
                        txtvlrFaturadoOriginal.Classe = "form-control form-warning";

                        try { ddlidCentroDeCusto.SelectedValue = ExecutarDataSet(sProcedure, new Dictionary<string, string> { { "@sFuncao", "CONSULTA_CENTRO_CUSTO" }, { "@idPedido", hddidPedido.Value } }).Tables[0].Rows[0]["idCentroCusto"].ToString(); }
                        catch { ddlidCentroDeCusto.SelectedValue = ddlidCentroDeCusto.SelectedValue == "0" || ddlidCentroDeCusto.SelectedValue == "" ? "1" : ddlidCentroDeCusto.SelectedValue; }

                        txtsDscContrib.Text = "";
                        txtnISS.Text = "0,00";
                        div_txtnISS.Visible = false;

                        DataSet ds = ExecutarDataSet(sProcedure, new Dictionary<string, string> { { "@sFuncao", "VALOR_IMPOSTOS_FATURAMENTO" }, { "@idFaturamento", idFaturamento } });
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            ddlidEmpresa.SelectedValue = DATASET(ds, "idEmpresa");

                            div_txtnISS.Visible = DATASET(ds, "sEdita_ISS") == "S";
                            txtnISS.Text = DATASET(ds, "sISS");

                            hddValor_Faturar.Value = DATASET(ds, "vlrFaturamento");
                            hddFator_IR.Value = DATASET(ds, "fator_IR");
                            hddFator_ISS.Value = DATASET(ds, "fator_ISS");
                            hddFator_INSS.Value = DATASET(ds, "fator_INSS");
                            hddFator_CSLL.Value = DATASET(ds, "fator_CSLL");
                            hddFator_PIS.Value = DATASET(ds, "fator_PIS");
                            hddFator_COFINS.Value = DATASET(ds, "fator_COFINS");

                            txtImposto_IR.Text = ((decimal.Parse(DATASET(ds, "fator_IR")) * 100 - 100) * -1).ToString("N2") + " %";
                            txtImposto_ISS.Text = ((decimal.Parse(DATASET(ds, "fator_ISS")) * 100 - 100) * -1).ToString("N2") + " %";
                            txtImposto_INSS.Text = ((decimal.Parse(DATASET(ds, "fator_INSS")) * 100 - 100) * -1).ToString("N2") + " %";
                            txtImposto_CSLL.Text = ((decimal.Parse(DATASET(ds, "fator_CSLL")) * 100 - 100) * -1).ToString("N2") + " %";
                            txtImposto_PIS.Text = ((decimal.Parse(DATASET(ds, "fator_PIS")) * 100 - 100) * -1).ToString("N2") + " %";
                            txtImposto_COFINS.Text = ((decimal.Parse(DATASET(ds, "fator_COFINS")) * 100 - 100) * -1).ToString("N2") + " %";
                        }

                        StatusDosCampos(true);

                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "PARCELAS_COND_PGTO" },
                            { "@idCondicaoPagamento", hddidCondicaoPagamento.Value }
                        };
                        DataSet dsParcelas = ExecutarDataSet(sProcedure, vParametros);

                        if (ValidarDataSet(dsParcelas))
                        {
                            int.TryParse(DATASET(dsParcelas, "nQtdParcelas"), out int nParcelas);
                            ddlidMeioRecebimento.SelectedValue = nParcelas > 1 ? "2" : "1";
                            txtnParcelas.Text = nParcelas.ToString();
                            Div_nParcelas.Visible = nParcelas > 1;

                            IncluirLancamento();
                        }
                        else throw new Exception($"A Condição de Pagamento não possui as Parcelas configuradas!<br /><a href='/App/Paginas/Manutencao/CondicaodePagamento_Detalhe.aspx?id={hddidCondicaoPagamento.Value}' target='_blank'><b>Configurar Parcelas |</b> {ddlidCondicaoPagamento.SelectedItem.Text}</a>");

                        Scripts.AbrirModal(Page, "Modal_Receber");
                    }
                    else MensagemPagina.MostraMensagem_Erro("Selecione o Faturamento!");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        protected void cmdSalvarFaturamento_Click(object sender, EventArgs e) => Salvar_Faturamento();

        protected void ddlidCategoriaReceber_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "Autoselecao_Contabil" },
                    { "@idCategoriaReceber", ddlidCategoriaReceber.SelectedValue }
                };
                DataSet dsContabil = ExecutarDataSet(sProcedure_Contas_Receber, vParametros);

                if (ValidarDataSet(dsContabil))
                {
                    idContabil = DATASET(dsContabil, "idContabil");
                    ddlidContabil.SelectedValue = idContabil;
                }
            }
            catch
            {
                ddlidContabil.SelectedValue = "";
            }
        }

        protected void ddlidMeioRecebimento_SelectedIndexChanged(object sender, EventArgs e)
        {
            Div_nParcelas.Visible = ddlidMeioRecebimento.SelectedValue == "2";
            txtnParcelas.Text = "1";
            IncluirLancamento();
        }

        protected void ddlidTranportadora_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidTranportadora.SelectedValue != "0")
            {
                try
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_Transportadora" },
                        { "@idCliente", ddlidTranportadora.SelectedValue }
                    };
                    DataSet dsPesquisa = ExecutarDataSet(sProcedure_Pedidos, vParametros);

                    if (ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        div_InfoTransportadora.Visible = true;
                        txtCNPJTrans.Text = DATASET(dsPesquisa, 0, "sCPF_CNPJ");
                        txtNomeTrans.Text = DATASET(dsPesquisa, 0, "sRazaoSocial");
                        txtIETrans.Text = DATASET(dsPesquisa, 0, "sRG_IE");
                        txtEnderecoTrans.Text = DATASET(dsPesquisa, 0, "sLogradouro");
                        txtMunicipiotrans.Text = DATASET(dsPesquisa, 0, "sCidade");
                        txtUFTrans.Text = DATASET(dsPesquisa, 0, "sEstado");

                        if (txtIETrans.Text == "") DIV_IETrans.Visible = false;
                        else DIV_IETrans.Visible = true;
                    }
                    else
                    {
                        MensagemPagina_Modal_Receber.MostraMensagem_Erro("A transportadora selecionada não possui um endereço principal cadastrado.");
                        ddlidTranportadora.SelectedValue = "0";
                        txtCNPJTrans.Text = "";
                        txtNomeTrans.Text = "";
                        txtIETrans.Text = "";
                        txtEnderecoTrans.Text = "";
                        txtMunicipiotrans.Text = "";
                        txtUFTrans.Text = "";
                        div_InfoTransportadora.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina_Modal_Receber.MostraMensagem_Erro("Erro: " + ex.Message);
                }
            }
            else
            {
                div_InfoTransportadora.Visible = false;
                txtCNPJTrans.Text = "";
                txtNomeTrans.Text = "";
                txtIETrans.Text = "";
                txtEnderecoTrans.Text = "";
                txtMunicipiotrans.Text = "";
                txtUFTrans.Text = "";
            }

            Scripts.Mantem_AbaAtiva(Page, "Transportadora-tab");
        }

        protected void txtnParcelas_TextChanged(object sender, EventArgs e) { IncluirLancamento(); ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Calcula_ValorLiquido", "Calcula_ValorLiquido();", true); }

        #endregion

        #region | Impostos

        void PopulaImpostos()
        {
            bs_Impostos.Clear();

            string sUsarProdutosCliente = chksUsarProdutosCliente.Checked ? "S" : "N";

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA_XML" },
                { "@idPedido", hddidFaturamento.Value },
                { "@idEmpresa", hddidEmpresa.Value },
                { "@idEnvioOPI", hddidEnvioOPI.Value },
                { "@sFabricante", "N" },
                { "@Itens", "" },
                { "@sUsarProdutosCliente", sUsarProdutosCliente }
            };
            DataSet dsPesquisa = ExecutarDataSet(sProcedure_Pedidos, vParametros, out string sql);

            if (ValidarDataSet(dsPesquisa, out string sErro))
            {
                string idCFOP = "";

                foreach (DataRow row in dsPesquisa.Tables[0].Rows)
                {
                    string idItem = row["ID"].ToString();
                    string cProd = row["cProd"].ToString();
                    string xProd = row["xProd"].ToString();
                    string NCM = row["NCM"].ToString();
                    string CFOP = row["CFOP"].ToString();
                    string uCom = row["uCom"].ToString();
                    string cEAN = row["cEAN"].ToString();
                    decimal qCom = decimal.Parse(row["qCom"].ToString(), CultureInfo.InvariantCulture);
                    decimal vProditem = decimal.Parse(row["vProd"].ToString(), CultureInfo.InvariantCulture);
                    decimal vUnCom = decimal.Parse(row["vUnCom"].ToString(), CultureInfo.InvariantCulture);
                    decimal vDescitem = decimal.Parse(row["vDesc"].ToString(), CultureInfo.InvariantCulture);

                    string orig = dsPesquisa.Tables[13].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["orig"]?.ToString();
                    string CST = dsPesquisa.Tables[13].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["CST"]?.ToString();
                    string modBC = dsPesquisa.Tables[13].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["modBC"]?.ToString();
                    string modBCST = dsPesquisa.Tables[13].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["modBCST"]?.ToString();

                    decimal vTotTrib = decimal.Parse(dsPesquisa.Tables[12].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["vTotTrib"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal pRedBC = decimal.Parse(dsPesquisa.Tables[13].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["pRedBC"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal vBC = decimal.Parse(dsPesquisa.Tables[13].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["vBC"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal pICMS = decimal.Parse(dsPesquisa.Tables[13].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["pICMS"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal vICMS = decimal.Parse(dsPesquisa.Tables[13].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["vICMS"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal pMVAST = decimal.Parse(dsPesquisa.Tables[13].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["pMVAST"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal vBCSTICMS = decimal.Parse(dsPesquisa.Tables[13].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["vBCST"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal pICMSST = decimal.Parse(dsPesquisa.Tables[13].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["pICMSST"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal vICMSST = decimal.Parse(dsPesquisa.Tables[13].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["vICMSST"]?.ToString(), CultureInfo.InvariantCulture);

                    // IPI
                    string cEnq = dsPesquisa.Tables[14].Rows.Count > 0 ? dsPesquisa.Tables[14].Rows[0]["cEnq"]?.ToString() : null;
                    string CSTIPI = dsPesquisa.Tables[15].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["CST"]?.ToString();
                    decimal vBCIPI = decimal.Parse(dsPesquisa.Tables[15].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["vBC"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal pIPI = decimal.Parse(dsPesquisa.Tables[15].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["pIPI"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal vIPI = decimal.Parse(dsPesquisa.Tables[15].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["vIPI"]?.ToString(), CultureInfo.InvariantCulture);

                    // PIS
                    string CSTPIS = dsPesquisa.Tables[17].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["CST"]?.ToString();
                    decimal vBCPIS = decimal.Parse(dsPesquisa.Tables[17].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["vBC"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal pPIS = decimal.Parse(dsPesquisa.Tables[17].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["pPIS"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal vPIS = decimal.Parse(dsPesquisa.Tables[17].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["vPIS"]?.ToString(), CultureInfo.InvariantCulture);

                    // COFINS
                    string CSTCOFINS = dsPesquisa.Tables[18].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["CST"]?.ToString();
                    decimal vBCCOFINS = decimal.Parse(dsPesquisa.Tables[18].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["vBC"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal pCOFINS = decimal.Parse(dsPesquisa.Tables[18].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["pCOFINS"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal vCOFINS = decimal.Parse(dsPesquisa.Tables[18].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["vCOFINS"]?.ToString(), CultureInfo.InvariantCulture);

                    //ICMSUFDest
                    decimal vBCUFDest = decimal.Parse(dsPesquisa.Tables[31].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["vBCUFDest"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal pFCPUFDest = decimal.Parse(dsPesquisa.Tables[31].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["pFCPUFDest"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal pICMSUFDest = decimal.Parse(dsPesquisa.Tables[31].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["pICMSUFDest"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal pICMSInter = decimal.Parse(dsPesquisa.Tables[31].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["pICMSInter"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal pICMSInterPart = decimal.Parse(dsPesquisa.Tables[31].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["pICMSInterPart"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal vFCPUFDest = decimal.Parse(dsPesquisa.Tables[31].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["vFCPUFDest"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal vICMSUFDest = decimal.Parse(dsPesquisa.Tables[31].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["vICMSUFDest"]?.ToString(), CultureInfo.InvariantCulture);
                    decimal vICMSUFRemet = decimal.Parse(dsPesquisa.Tables[31].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["vICMSUFRemet"]?.ToString(), CultureInfo.InvariantCulture);
                    string sCalculoDIFAL = dsPesquisa.Tables[31].AsEnumerable().FirstOrDefault(t => t["ID"].ToString() == idItem)?["sCalculoDIFAL"]?.ToString();

                    Dictionary<string, string> vParametrosCFOP = new Dictionary<string, string>
                    {
                        { "@sFuncao", "COLSULTAR_CFOP" },
                        { "@idItem", hddidFaturamento.Value }
                    };
                    DataSet dsPesquisaCFOP = ExecutarDataSet(sProcedure_Pedidos, vParametrosCFOP);

                    idCFOP = DATASET(dsPesquisaCFOP, "idCFOP");

                    cls_Impostos objItem = new cls_Impostos
                    {
                        ID = idItem,
                        cProd = cProd,
                        xProd = xProd,
                        NCM = NCM,
                        CFOP = CFOP,
                        uCom = uCom,
                        cEAN = cEAN,
                        qCom = qCom.ToString("N2"),
                        vUnCom = vUnCom.ToString("N4"),
                        vDesc = vDescitem.ToString("N2"),
                        vProd = vProditem.ToString("N2"),
                        vTotTrib = vTotTrib.ToString("N2"),

                        // ICMS
                        orig = orig,
                        CST = CST,
                        modBC = modBC,
                        vBC = vBC.ToString("N2"),
                        pRedBC = pRedBC.ToString("N2"),
                        pICMS = pICMS.ToString("N2"),
                        vICMS = vICMS.ToString("N2"),
                        modBCST = modBCST,
                        pMVAST = pMVAST.ToString("N2"),
                        vBCSTICMS = vBCSTICMS.ToString("N2"),
                        pICMSST = pICMSST.ToString("N2"),
                        vICMSST = vICMSST.ToString("N2"),

                        // IPI
                        cEnq = cEnq,
                        CSTIPI = CSTIPI,
                        vBCIPI = vBCIPI.ToString("N2"),
                        pIPI = pIPI.ToString("N2"),
                        vIPI = vIPI.ToString("N2"),

                        // PIS
                        CSTPIS = CSTPIS,
                        vBCPIS = vBCPIS.ToString("N2"),
                        pPIS = pPIS.ToString("N2"),
                        vPIS = vPIS.ToString("N2"),

                        // COFINS
                        CSTCOFINS = CSTCOFINS,
                        vBCCOFINS = vBCCOFINS.ToString("N2"),
                        pCOFINS = pCOFINS.ToString("N2"),
                        vCOFINS = vCOFINS.ToString("N2"),

                        //ICMSUFDest
                        vBCUFDest = vBCUFDest.ToString("N2"),
                        pFCPUFDest = pFCPUFDest.ToString("N2"),
                        pICMSUFDest = pICMSUFDest.ToString("N2"),
                        pICMSInter = pICMSInter.ToString("N2"),
                        pICMSInterPart = pICMSInterPart.ToString("N2"),
                        vFCPUFDest = vFCPUFDest.ToString("N2"),
                        vICMSUFDest = vICMSUFDest.ToString("N2"),
                        vICMSUFRemet = vICMSUFRemet.ToString("N2"),
                        sCalculoDIFAL = sCalculoDIFAL,

                        idCFOP = idCFOP
                    };
                    bs_Impostos.Add(objItem);
                }

                gvItensImpostos.DataSource = bs_Impostos;
                gvItensImpostos.DataBind();

                txtvBCtotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vBC"), CultureInfo.InvariantCulture).ToString("N2");
                txtvICMStotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vICMS"), CultureInfo.InvariantCulture).ToString("N2");
                txtvICMSDesontotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vICMSDeson"), CultureInfo.InvariantCulture).ToString("N2");
                txtvFCPtotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vFCP"), CultureInfo.InvariantCulture).ToString("N2");
                txtvBCSTtotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vBCST"), CultureInfo.InvariantCulture).ToString("N2");
                txtvSTtotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vST"), CultureInfo.InvariantCulture).ToString("N2");
                txtvFCPSTtotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vFCPST"), CultureInfo.InvariantCulture).ToString("N2");
                txtvFCPSTRettotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vFCPSTRet"), CultureInfo.InvariantCulture).ToString("N2");
                txtvProdtotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vProd"), CultureInfo.InvariantCulture).ToString("N2");
                txtvFretetotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vFrete"), CultureInfo.InvariantCulture).ToString("N2");
                txtvSegtotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vSeg"), CultureInfo.InvariantCulture).ToString("N2");
                txtvDesctotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vDesc"), CultureInfo.InvariantCulture).ToString("N2");
                txtvIItotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vII"), CultureInfo.InvariantCulture).ToString("N2");
                txtvIPItotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vIPI"), CultureInfo.InvariantCulture).ToString("N2");
                txtvIPIDevoltotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vIPIDevol"), CultureInfo.InvariantCulture).ToString("N2");
                txtvPIStotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vPIS"), CultureInfo.InvariantCulture).ToString("N2");
                txtvCOFINStotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vCOFINS"), CultureInfo.InvariantCulture).ToString("N2");
                txtvOutrototal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vOutro"), CultureInfo.InvariantCulture).ToString("N2");
                txtvNFtotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vNF"), CultureInfo.InvariantCulture).ToString("N2");
                txtvTotTribtotal.Text = decimal.Parse(DATASET(dsPesquisa, 20, 0, "vTotTrib"), CultureInfo.InvariantCulture).ToString("N2");

                if (lblTitulo_Modal_Receber.InnerText != "Devolução") ddlidCFOP.Popula_Combo("sp_Select 'tbl_Flow_Adm_CFOP'", "idCFOP", "sCFOP", false, "Selecione o CFOP", "0");
                else ddlidCFOP.Popula_Combo("sp_Select 'tbl_Flow_Adm_CFOP_DEVOLUCAO'", "idCFOP", "sCFOP", false, "Selecione o CFOP", "0");

                if (hddsEmpreitada.Value == "S")
                {
                    DIV_SalvarCFOP.Visible = true;
                    if (hddsFaturamento.Value == "S")
                    {
                        ddlidCFOP.Attributes.Add("disabled", "disabled");
                        DIV_btnSalvarCFOP.Visible = false;
                    }
                    else
                    {
                        ddlidCFOP.Attributes.Remove("disabled");
                        DIV_btnSalvarCFOP.Visible = true;
                    }
                }
                else DIV_SalvarCFOP.Visible = false;
            }
            else throw new Exception(sErro);
        }

        protected void gvItensImpostos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string txtpRedBC = DataBinder.Eval(e.Row.DataItem, "pRedBC")?.ToString();
                string sCalculoDIFAL = DataBinder.Eval(e.Row.DataItem, "sCalculoDIFAL")?.ToString();
                string txtmodBCST = DataBinder.Eval(e.Row.DataItem, "modBCST")?.ToString();
                string txtpMVAST = DataBinder.Eval(e.Row.DataItem, "pMVAST")?.ToString();
                string txtvBCSTICMS = DataBinder.Eval(e.Row.DataItem, "vBCSTICMS")?.ToString();
                string txtpICMSST = DataBinder.Eval(e.Row.DataItem, "pICMSST")?.ToString();
                string txtvICMSST = DataBinder.Eval(e.Row.DataItem, "vICMSST")?.ToString();

                if (txtpRedBC == "0,00") (e.Row.FindControl("pRedBC") as HtmlTableCell).Visible = false;
                if (txtmodBCST == "0") (e.Row.FindControl("modBCST") as HtmlTableCell).Visible = false;
                if (txtpMVAST == "0,00") (e.Row.FindControl("pMVAST") as HtmlTableCell).Visible = false;
                if (txtvBCSTICMS == "0,00") (e.Row.FindControl("vBCSTICMS") as HtmlTableCell).Visible = false;
                if (txtpICMSST == "0,00") (e.Row.FindControl("pICMSST") as HtmlTableCell).Visible = false;
                if (txtvICMSST == "0,00") (e.Row.FindControl("vICMSST") as HtmlTableCell).Visible = false;

                if (sCalculoDIFAL == "N")
                {
                    if (e.Row.FindControl("trDIFAL") is HtmlTableRow trDIFAL) trDIFAL.Visible = false;
                }
                else (e.Row.FindControl("sCalculoDIFAL") as HtmlTableCell).Visible = false;
            }

            EsconderColunas(e, 0);
        }

        #endregion

        #region | Lançamentos

        void dtgLancamento_DataBind()
        {
            dtgLancamento.DataSource = bs_LancamentoRec;
            dtgLancamento.DataBind();

            SwitchAtivo_Retencao_IR.Definir("N", "IR Retido?", "N");
            SwitchAtivo_Retencao_ISS.Definir("N", "ISS Retido?", "N");
            SwitchAtivo_Retencao_INSS.Definir("N", "INSS Retido?", "N");
            SwitchAtivo_Retencao_CSLL.Definir("N", "CSLL Retido?", "N");
            SwitchAtivo_Retencao_PIS.Definir("N", "PIS Retido?", "N");
            SwitchAtivo_Retencao_COFINS.Definir("N", "COFINS Retido?", "N");

            bool bVariasLinhas = dtgLancamento.Rows.Count > 1;
            foreach (GridViewRow row in dtgLancamento.Rows)
            {
                if (row.FindControl("cmdAplicaTodos_data") is LinkButton cmdAplicaTodos_data)
                    cmdAplicaTodos_data.Visible = bVariasLinhas;
                if (row.FindControl("cmdAplicaTodos_idForma") is LinkButton cmdAplicaTodos_idForma)
                    cmdAplicaTodos_idForma.Visible = bVariasLinhas;
                if (row.FindControl("cmdAplicaTodos_idConta") is LinkButton cmdAplicaTodos_idConta)
                    cmdAplicaTodos_idConta.Visible = bVariasLinhas;
            }

            SomarColunas(dtgLancamento, false, Formatação.Numero, typeof(TextBox), "R$ ", "", 2, 3);
        }

        protected void IncluirLancamento_Envio(DataSet ds)
        {
            DataTable tb = ds.Tables[2];

            if (ddlidMeioRecebimento.SelectedValue != "0")
            {
                DIV_Lancamentos.Visible = true;
                bs_LancamentoRec.Clear();

                decimal.TryParse(hddvlrFaturamento.Value, out decimal totalFaturamento);

                foreach (DataRow row in tb.Rows)
                {
                    decimal porcentagem = Convert.ToDecimal(row["nPorcentagemValor"]);
                    decimal valorParcela = totalFaturamento * porcentagem / 100;
                    if (porcentagem == 0) valorParcela = totalFaturamento / tb.Rows.Count;
                    valorParcela = Math.Round(valorParcela, 2);

                    cls_LancamentoRec objItem = new cls_LancamentoRec
                    {
                        idLinha = bs_LancamentoRec.Count + 1,
                        sFuncao = "INSERIR_LANCAMENTO",
                        dtLancamentoRec = DateTime.Now.AddDays(Convert.ToInt32(row["nDDL"].ToString())).ToString("yyyy-MM-dd"),
                        idFormaPagamentoLancamentoRec = Convert.ToInt32(row["idTipoCondicaoPagamento"].ToString()),
                        idContaBancaria = Convert.ToInt32(ds.Tables[4].Rows[0]["idContaBancaria"].ToString()),
                        idContasReceber = 0,
                        nValorLancamentoRec = valorParcela,
                        nValorBrutoLancamentoRec = valorParcela,
                        nParcelaLancamentoRec = bs_LancamentoRec.Count + 1
                    };
                    bs_LancamentoRec.Add(objItem);
                }

                decimal txtnValorBrutoLancamentoRec = bs_LancamentoRec.Sum(i => i.nValorBrutoLancamentoRec);
                if (txtnValorBrutoLancamentoRec != totalFaturamento && bs_LancamentoRec.Any())
                {
                    var ultimo = bs_LancamentoRec.Last();

                    decimal novoValor = ultimo.nValorBrutoLancamentoRec + Math.Round(totalFaturamento - txtnValorBrutoLancamentoRec, 2);
                    if (novoValor < 0) novoValor = 0;

                    ultimo.nValorBrutoLancamentoRec = novoValor;
                    ultimo.nValorLancamentoRec = novoValor;
                }

                dtgLancamento_DataBind();

                Scripts.FocusScript(Page, "txtdtLancamentoRec");
            }
        }

        protected void IncluirLancamento()
        {
            if (ddlidMeioRecebimento.SelectedValue != "0")
            {
                bs_LancamentoRec.Clear();
                DIV_Lancamentos.Visible = true;

                int numeroParcelas = 1;
                if (txtnParcelas.Text != "") numeroParcelas = Convert.ToInt32(txtnParcelas.Text);

                decimal.TryParse(hddvlrFaturamento.Value, out decimal totalFaturamento);
                decimal valor = Math.Round(totalFaturamento / numeroParcelas, 2);

                for (int i = bs_LancamentoRec.Count; i < numeroParcelas; i++)
                {
                    cls_LancamentoRec objItem = new cls_LancamentoRec
                    {
                        idLinha = bs_LancamentoRec.Count + 1,
                        sFuncao = "INSERIR_LANCAMENTO",
                        idFormaPagamentoLancamentoRec = 0,
                        idContaBancaria = 0,
                        idContasReceber = 0,
                        nValorLancamentoRec = valor,
                        nValorBrutoLancamentoRec = valor,
                        nParcelaLancamentoRec = bs_LancamentoRec.Count + 1
                    };
                    bs_LancamentoRec.Add(objItem);
                }

                decimal diferenca = totalFaturamento - (valor * numeroParcelas);
                var ultimoLancamento = bs_LancamentoRec.Last();
                if (diferenca != 0 && ultimoLancamento != null)
                {
                    ultimoLancamento.nValorLancamentoRec += diferenca;
                    ultimoLancamento.nValorBrutoLancamentoRec += diferenca;
                }

                dtgLancamento_DataBind();
            }
        }

        protected void Lancamentos_Salvar(bool bServico = false)
        {
            int nContador = 0;
            foreach (GridViewRow item in dtgLancamento.Rows)
            {
                TextBox txtnValorLancamentoRec_Linha = (TextBox)item.FindControl("txtnValorLancamentoRec");
                TextBox txtnValorBrutoLancamentoRec_Linha = (TextBox)item.FindControl("txtnValorBrutoLancamentoRec");
                TextBox txtdtLancamentoRec_Linha = (TextBox)item.FindControl("txtdtLancamentoRec");
                DropDownList ddlidFormaPagamentoLancamentoRec_Linha = (DropDownList)item.FindControl("ddlidFormaPagamentoLancamentoRec");
                DropDownList ddlidContaBancaria_Linha = (DropDownList)item.FindControl("ddlidContaBancaria");

                decimal.TryParse(txtnValorLancamentoRec_Linha.Text, out decimal nVlrLancamentoRec_Linha);
                decimal.TryParse(txtnValorBrutoLancamentoRec_Linha.Text, out decimal nVlrBrutoLancamentoRec_Linha);

                if (bServico) decimal.TryParse((item.FindControl("hddValorLiquido") as HiddenField).Value, out nVlrLancamentoRec_Linha);

                bs_LancamentoRec[nContador].nValorLancamentoRec = nVlrLancamentoRec_Linha;
                bs_LancamentoRec[nContador].nValorBrutoLancamentoRec = nVlrBrutoLancamentoRec_Linha;

                if (string.IsNullOrEmpty(txtdtLancamentoRec_Linha.Text) || txtdtLancamentoRec_Linha.Text == "0,00")
                    bs_LancamentoRec[nContador].dtLancamentoRec = "";
                else bs_LancamentoRec[nContador].dtLancamentoRec = txtdtLancamentoRec_Linha.Text;

                if (string.IsNullOrEmpty(ddlidFormaPagamentoLancamentoRec_Linha.SelectedValue) || ddlidFormaPagamentoLancamentoRec_Linha.SelectedValue == "0")
                    bs_LancamentoRec[nContador].idFormaPagamentoLancamentoRec = 0;
                else bs_LancamentoRec[nContador].idFormaPagamentoLancamentoRec = Convert.ToInt32(ddlidFormaPagamentoLancamentoRec_Linha.SelectedValue);

                if (string.IsNullOrEmpty(ddlidContaBancaria_Linha.SelectedValue) || ddlidContaBancaria_Linha.SelectedValue == "0")
                    bs_LancamentoRec[nContador].idContaBancaria = 0;
                else bs_LancamentoRec[nContador].idContaBancaria = Convert.ToInt32(ddlidContaBancaria_Linha.SelectedValue);

                nContador++;
            }
        }

        protected void dtgLancamento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer) { e.Row.CssClass = "linhaTotal"; e.Row.Cells[2].CssClass = "brutoTotal"; e.Row.Cells[3].CssClass = "liquidoTotal"; }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddl = e.Row.FindControl("ddlidFormaPagamentoLancamentoRec") as DropDownList;
                Popula_Combo(ddl, "sp_Select 'tbl_Flow_CondicaodePagamento_Tipo'", "idTipoCondicaoPagamento", "sDscTipoCondicaoPagamento", false, "Forma de Recebimento", "0");
                ddl.SelectedValue = bs_LancamentoRec[e.Row.RowIndex].idFormaPagamentoLancamentoRec > 0 ? bs_LancamentoRec[e.Row.RowIndex].idFormaPagamentoLancamentoRec.ToString() : "2";
                ddlidFormaPagamentoLancamentoRec_SelectedIndexChanged(ddl, null);

                e.Row.Attributes["data-nLinha"] = e.Row.RowIndex.ToString();

                try
                {
                    if (e.Row.FindControl("txtnValorLancamentoRec") is TextBox txtLiquido)
                    {
                        bool bPedido = int.TryParse(hddidPedido.Value, out int idPedido) && idPedido > 0;
                        txtLiquido.ReadOnly = bPedido;

                        if (bPedido && e.Row.FindControl("hddValorLiquido") is HiddenField hddValorLiquido)
                            hddValorLiquido.Value = txtLiquido.Text;
                    }
                }
                catch { }
            }

            EsconderColunas(e, 0);
        }

        protected void dtgLancamento_RowDeleting(object sender, GridViewDeleteEventArgs e) { bs_LancamentoRec.RemoveAt(e.RowIndex); dtgLancamento_DataBind(); }

        protected void ddlidFormaPagamentoLancamentoRec_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is DropDownList ddl)
            {
                DropDownList ddlConta = (ddl.NamingContainer as GridViewRow).FindControl("ddlidContaBancaria") as DropDownList;

                if (ddlConta.Items.Count <= 0)
                {
                    Popula_Combo(ddlConta, "sp_Select 'tbl_Flow_Adm_ContasBancarias_x_AgenciaConta'", "idConta", "sConta", false, "Conta Bancária", "0");
                    ddlConta.SelectedValue = "4";
                }

                if (ddl.SelectedValue == "2" || ddl.SelectedValue == "3" || ddl.SelectedValue == "5" || ddl.SelectedValue == "7") // 2 - PIX | 3 - Boleto Bancário | 5 - Transferência (TED/DOC) | 7 - Crédito em Conta
                {
                    ddlConta.Visible = true;
                    ddlConta.SelectedValue = ddlConta.SelectedValue == "0" ? "4" : ddlConta.SelectedValue;
                }
                else
                {
                    ddlConta.Visible = false;
                    ddlConta.SelectedValue = "0";
                }
            }
        }

        #endregion

        #region | CFOP

        protected void ddlidCFOP_SelectedIndexChanged(object sender, EventArgs e)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_CFOP" },
                { "@idCFOP", ddlidCFOP.SelectedValue }
            };
            DataSet dsPesquisa = ExecutarDataSet(sProcedure_Pedidos, vParametros);

            string sCSTICMS = DATASET(dsPesquisa, "sCSTICMS");
            string sCSTIPI = DATASET(dsPesquisa, "sCSTIPI");
            string sCSTPIS = DATASET(dsPesquisa, "sCSTPIS");
            string sCSTCOFINS = DATASET(dsPesquisa, "sCSTCOFINS");

            foreach (GridViewRow linha in gvItensImpostos.Rows)
            {
                (linha.FindControl("lblCST") as Label).Text = sCSTICMS;
                (linha.FindControl("lblCSTIPI") as Label).Text = sCSTIPI;
                (linha.FindControl("lblCSTPIS") as Label).Text = sCSTPIS;
                (linha.FindControl("lblCSTCOFINS") as Label).Text = sCSTCOFINS;

                foreach (var bs in bs_Impostos)
                {
                    bs.CST = sCSTICMS;
                    bs.CSTIPI = sCSTIPI;
                    bs.CSTPIS = sCSTPIS;
                    bs.CSTCOFINS = sCSTCOFINS;
                    bs.idCFOP = ddlidCFOP.SelectedValue;
                }
            }
        }

        protected void btnSalvarCFOP_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "ALTERAR_CFOP" },
                    { "@idItem", hddidFaturamento.Value },
                    { "@idCFOP", ddlidCFOP.SelectedValue }
                };
                DataSet dsPesquisa = ExecutarDataSet(sProcedure_Pedidos, vParametros);

                PopulaImpostos();

                if (lblTitulo_Modal_Receber.InnerText == "Devolução")
                {
                    DIV_SalvarCFOP.Visible = true;

                    ddlidCFOP.Popula_Combo("sp_Select 'tbl_Flow_Adm_CFOP_DEVOLUCAO'", "idCFOP", "sCFOP", false, "Selecione o CFOP", "0");

                    Dictionary<string, string> vParametrosCFOP = new Dictionary<string, string>
                    {
                        { "@sFuncao", "COLSULTAR_CFOP" },
                        { "@idItem", hddidFaturamento.Value }
                    };
                    DataSet dsPesquisaCFOP = ExecutarDataSet(sProcedure_Pedidos, vParametrosCFOP);

                    ddlidCFOP.SelectedValue = DATASET(dsPesquisaCFOP, "idCFOP");
                }

                MensagemPagina_Modal_Receber.MostraMensagem_Sucesso("CFOP salvo com sucesso!");
                Scripts.Mantem_AbaAtiva(Page, "Transportadora-tab");
            }
            catch (Exception ex)
            {
                MensagemPagina_Modal_Receber.MostraMensagem_Erro(ex.Message);
            }
        }

        #endregion

        #endregion

        #region | Faturamento - Serviços

        protected void gv_Faturamento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
                (e.Row.FindControl("rbFaturamento") as RadioButton).Visible = e.Row.Cells[6].Text != "S";
            }

            EsconderColunas(e, 6);
        }

        protected void cmdEmitirNFS_Click(object sender, EventArgs e)
        {
            Lancamentos_Salvar(true);

            try
            {
                string idFaturamento = hddidEnvioOPI_idFaturamento.Value;

                decimal vlrLancamentos = bs_LancamentoRec.Sum(l => l.nValorBrutoLancamentoRec);
                decimal.TryParse(hddvlrFaturamento.Value, out decimal vlrFaturamento);

                if (txtsDscServico.Text.Length <= 14)
					throw new Exception("É necessário preencher o Corpo da NFS-e com ao menos 15 caracteres!");
                if (Math.Round(vlrLancamentos, 2) != Math.Round(vlrFaturamento, 2))
                    throw new Exception($"É obrigatório que o valor Total dos Lançamentos (R$ {vlrLancamentos:N2}) seja igual ao valor deste Faturamento (R$ {vlrFaturamento:N2})!");
                if (bs_LancamentoRec.Any(l => l.nValorLancamentoRec <= 0 || l.nValorBrutoLancamentoRec <= 0))
					throw new Exception("Não é possível realizar o Faturamento incluindo Lançamentos com o valor Líquido ou Bruto zerados!");

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "EFETUAR_FATURAMENTO" },
                    { "@nValorFaturamento", vlrLancamentos.ToString().Replace(",", ".") },
                    { "@idPedido", hddidPedido.Value },
                    { "@idFaturamento", idFaturamento },
                    { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                };

                if (div_txtnISS.Visible)
                    vParametros.Add("@nISS", txtnISS.Text.Replace(".", "").Replace(",", "."));

                ExecutarDataSet(sProcedure, vParametros);

                if (bs_LancamentoRec.Count > 0)
                {
                    string sFaturamentoTeste = ExecutarDataSet(sProcedure_Contas_Receber, new Dictionary<string, string> { { "@sFuncao", "Parametros" } }).Tables[0].Rows[0]["sFaturamentoTeste"].ToString();
                    int numeroParcelaAtual = 1;
                    foreach (var Linha in bs_LancamentoRec)
                    {
                        Dictionary<string, string> vP = new Dictionary<string, string>();

                        if (sFaturamentoTeste == "S") vP.Add("@sFuncao", "SALVAR_TESTE");
                        else vP.Add("@sFuncao", "SALVAR");

                        vP.Add("@idContasReceber", "0");
                        vP.Add("@nSaldo", Conversoes.Numerico(Linha.nValorLancamentoRec));
                        vP.Add("@nValorOriginal", Conversoes.Numerico(Linha.nValorLancamentoRec));
                        vP.Add("@dtVencimento", Linha.dtLancamentoRec.ToString());
                        vP.Add("@nValorBruto", Conversoes.Numerico(Linha.nValorBrutoLancamentoRec));

                        if (bs_LancamentoRec.Count == 1) vP.Add("@sQuantidadeParcela", $"Única");
                        else vP.Add("@sQuantidadeParcela", $"{numeroParcelaAtual} de {bs_LancamentoRec.Count}");
                        numeroParcelaAtual++;

                        vP.Add("@sCodigo", "Serviço");
                        vP.Add("@dtEmissao", txtdtEmissao.Text.Replace("-", "/"));
                        vP.Add("@sDocumento", txtnControle.Text);
                        vP.Add("@idContabil", idContabil);
                        vP.Add("@idEmpresa", hddidEmpresa.Value);
                        vP.Add("@idCentroDeCusto", ddlidCentroDeCusto.SelectedValue);
                        vP.Add("@idMeioRecebimento", ddlidMeioRecebimento.SelectedValue);
                        vP.Add("@nParcelas", txtnParcelas.Text);
                        vP.Add("@idFormaRecebimento", Linha.idFormaPagamentoLancamentoRec.ToString());
                        vP.Add("@idConta", Linha.idContaBancaria.ToString());
                        vP.Add("@idCategoriaReceber", ddlidCategoriaReceber.SelectedValue);
                        vP.Add("@dtVencimentoOriginal", Linha.dtLancamentoRec.ToString());
                        vP.Add("@sObservacaoGeral", txtsObservacaoGeral.Text);
                        vP.Add("@idParceiro", hddidCliente.Value);
                        vP.Add("@idUsuarioAtualizacao", Variaveis.idUsuario());
                        vP.Add("@sSituacao", "N");
                        vP.Add("@idEnvioOPI", hddidPedido.Value);
                        vP.Add("@idFaturamento", idFaturamento);

                        ExecutarDataSet(sProcedure_Contas_Receber, vP);
                    }
                }

                (string sNomeArquivo, string conteudoTXT, string numeroRPS, string serie) = EstruturaNFS_TXT(idFaturamento, hddidEmpresa.Value, txtsDscServico.Text.Replace(Environment.NewLine, "|").Replace("\n", "|"),
                    SwitchAtivo_Retencao_IR.Recuperar(), SwitchAtivo_Retencao_ISS.Recuperar(), SwitchAtivo_Retencao_INSS.Recuperar(), SwitchAtivo_Retencao_CSLL.Recuperar(), SwitchAtivo_Retencao_PIS.Recuperar(), SwitchAtivo_Retencao_COFINS.Recuperar(),
                    ddlRPS.SelectedValue);

                byte[] bytesPDF = PDF_Servicos(numeroRPS, idFaturamento, txtsDscServico.Text, txtsDscContrib.Text, serie, SwitchAtivo_Retencao_IR.Recuperar(), SwitchAtivo_Retencao_INSS.Recuperar(),
                    SwitchAtivo_Retencao_CSLL.Recuperar(), SwitchAtivo_Retencao_PIS.Recuperar(), SwitchAtivo_Retencao_COFINS.Recuperar());

                string idArquivo = SalvarArquivo(sNomeArquivo, 10009, conteudoTXT);
                string idArquivoPDF = SalvarArquivo($"RPS{numeroRPS}_{hddidPedido.Value}.pdf", 10009, bytesPDF);

                string nNF = numeroRPS.TrimStart('0');
                int tamanho_RPS = numeroRPS.Length;
                numeroRPS = $"RPS: {(hddidPedido.Value + nNF).PadLeft(tamanho_RPS, '0')}";

                ddlEmpresa.SelectedValue = hddidEmpresa.Value;
                ddlidCliente.SelectedValue = hddidCliente.Value;

                Dictionary<string, string> vParametrosGerenciador = new Dictionary<string, string>
                {
                    { "@sfuncao", "INCLUIR_XML" },
                    { "@idTipoObjeto", "15" },
                    { "@idObjeto", idFaturamento },
                    { "@idOrigem", "2" },
                    { "@sChaveNFe", numeroRPS },
                    { "@sCNPJ_Emitente", "" },
                    { "@nNumeroNF", nNF },
                    { "@sXML", conteudoTXT },
                    { "@idEmpresa", ddlidEmpresa.SelectedValue },
                    { "@sNome_Emitente", ddlEmpresa.SelectedItem.Text },
                    { "@sNomeFantasia_Emitente", "" },
                    { "@sLogradouro_Emitente", "" },
                    { "@sNumero_Emitente", "" },
                    { "@sUF_Emitente", "" },
                    { "@sCEP_Emitente", "" },
                    { "@sPais_Emitente", "" },
                    { "@sTelefone_Emitente", "" },
                    { "@sNome_Destinatario", ddlidCliente.SelectedItem.Text },
                    { "@sTipoXML", "2" },
                    { "@idArquivo", idArquivo },
                    { "@idArquivoPDF", idArquivoPDF },
                    { "@scStat", "0" }
                };
                DataSet ds = ExecutarDataSet(sProcedure_XML, vParametrosGerenciador);

                Dictionary<string, string> vParametrosITEM = new Dictionary<string, string>
                {
                    { "@sFuncao", "NFS_ITEM" },
                    { "@idXML", DATASET(ds, "idXML") },
                    { "@idFaturamento", idFaturamento },
                    { "@idArquivoPDF", idArquivoPDF }
                };
                ExecutarDataSet(sProcedure_XML, vParametrosITEM);

                hddidArquivo.Value = idArquivo;

                Pesquisar("0", hddidPedido.Value);

                MensagemPagina.MostraMensagem_Sucesso("NFS gerada com sucesso!");

                // talvez enviar um email direto ao T-Flow com os avisos, ao invés de exibir ao Usuário - função no BD, para que possa ser desativada direto por lá
                //if (!string.IsNullOrEmpty(avisos) && bDev) MensagemPagina.MostraMensagem_Aviso($"A NFS foi gerada <b>corretamente</b>, mas foram detectadas possíveis inconsistências: {avisos}");
            }
            catch (Exception ex)
            {
                dtgLancamento_DataBind();
                MensagemPagina_Modal_Receber.MostraMensagem_Erro("<b>Erro:</b> " + ex.Message);
                Scripts.RemoverBackdrop_Modal(Page);
                Scripts.AbrirModal(Page, "Modal_Receber");
            }
        }

        protected void btnImportarRetorno_Click(object sender, EventArgs e)
        {
            if (fu_ImportarRetorno_NFS.HasFile && GetExtension(fu_ImportarRetorno_NFS.FileName).ToLower() == ".txt")
            {
                (string sErro, List<(string idXML, string idFaturamento, string nNFS)> lstNFS) = Valida_ArquivoRetorno_NFS(fu_ImportarRetorno_NFS.FileContent, hddidPedido.Value);

                if (string.IsNullOrEmpty(sErro))
                {
                    SqlDataAdapter da = new SqlDataAdapter(sProcedure_Arquivos, StringDeConexao);
                    da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "INCLUIR";
                    da.SelectCommand.Parameters.Add("@idTipoArquivo", SqlDbType.Int).Value = 10009;
                    da.SelectCommand.Parameters.Add("@idObjeto", SqlDbType.Int).Value = 0;
                    da.SelectCommand.Parameters.Add("@sNomeArquivo", SqlDbType.VarChar).Value = $"NFS-{CarimboDataHora()}";
                    da.SelectCommand.Parameters.Add("@sDscArquivo", SqlDbType.VarChar).Value = "";
                    da.SelectCommand.Parameters.Add("@sObservacao", SqlDbType.VarChar).Value = "";
                    da.SelectCommand.Parameters.Add("@vbArquivo", SqlDbType.VarBinary).Value = fu_ImportarRetorno_NFS.FileBytes;
                    da.SelectCommand.Parameters.Add("@idUsuario", SqlDbType.Int).Value = Variaveis.idUsuario();
                    da.SelectCommand.Parameters.Add("@dtExpiracaoDoc", SqlDbType.VarChar).Value = "";
                    da.SelectCommand.Parameters.Add("@dtRegistroDoc", SqlDbType.VarChar).Value = "";

                    string idArquivo_Retorno = "0";
                    try
                    {
                        DataSet tabela = new DataSet();
                        da.Fill(tabela);
                        idArquivo_Retorno = DATASET(tabela, "idArquivo");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Erro ao salvar o arquivo de retorno: {ex.Message}");
                    }

                    foreach (var item in lstNFS)
                    {
                        Dictionary<string, string> vParam = new Dictionary<string, string>
                        {
                            { "@sFuncao", "RETORNO_NFS" },
                            { "@idArquivo_Retorno", idArquivo_Retorno },
                            { "@idFaturamento", item.idFaturamento },
                            { "@idXML", item.idXML },
                            { "@nNumeroNF", item.nNFS }
                        };
                        ExecutarDataSet(sProcedure_XML, vParam);
                    }

                    Pesquisar("0", hddidPedido.Value);
                    MensagemPagina_AbaPDF.MostraMensagem_Sucesso("Retorno importado com sucesso!");
                }
                else MensagemPagina_AbaPDF.MostraMensagem_Erro(sErro);

                Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
            }
            else
            {
                MensagemPagina_ModalImportarRetorno_NFS.MostraMensagem_Erro("É necessário selecionar um arquivo válido em formato de Texto (.txt)!");
                Scripts.AbrirModal(Page, "modalImportarRetorno_NFS");
            }
        }

        protected void btnGerarPDFEspelho_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton lnk = sender as LinkButton;
                GridViewRow row = lnk.NamingContainer as GridViewRow;
                DataTable tb = ExecutarDataTable(sProcedure_Arquivos, new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_DETALHE" }, { "@idArquivo", lnk.CommandArgument } });
                DataTable tb_XML = ExecutarDataTable(sProcedure_XML, new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_DETALHE" }, { "@idXML", row.Cells[nColuna_idXML].Text } });
                string sXML = tb_XML.Rows[0]["sXML"].ToString();

                var imagem = Combine(AppDomain.CurrentDomain.BaseDirectory, "App", "img", lnk.CommandName == "N" ? "Espelho.png" : "LogoTT_Horizontal.png");
                byte[] PDF = Adiciona_MarcaDagua((byte[])tb.Rows[0]["vbArquivo"], imagem, "", null, null, 0, 10, 200, -100, 35, 0.35f);

                if (row.Cells[nColuna_Status].Text == "Autorizada")
                {
                    PDF = Adiciona_MarcaDagua(PDF, null, sXML.Substring(9, 8).Trim(), null, BaseColor.BLACK, 10, 10, 200, -100, 0, 0.85f, 487, 812);
                    PDF = Adiciona_MarcaDagua(PDF, null, sXML.Substring(31, 8).Trim(), null, BaseColor.BLACK, 10, 10, 200, -100, 0, 0.85f, 482, 760);
                }

                Session["ExibeArquivo"] = PDF;
                Session["ExibeArquivo_Nome"] = tb.Rows[0]["sNomeArquivo"];

                DirecionaPagina_NovaAba(Page, "/App/ExibeArquivo.aspx");
            }
            catch (Exception ex)
            {
                MensagemPagina_AbaPDF.MostraMensagem_Erro($"Não foi possível exibir o PDF da NFS.<br />Erro: {ex.Message}");
            }

            Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
        }

        protected void btnGerarTXT_Click(object sender, EventArgs e)
        {
            string sIDs = (sender as LinkButton).CommandArgument;
            ExecutarDataSet(sProcedure_XML, new Dictionary<string, string> { { "@sFuncao", "ATUALIZA_ESPELHO" }, { "@idXML", sIDs.Split('|')[0] }, { "@sEspelho", "N" } });
            DataTable tb = ExecutarDataTable(sProcedure_Arquivos, new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_DETALHE" }, { "@idArquivo", sIDs.Split('|')[1] } });

            string sNomeArquivo = tb.Rows[0]["sNomeArquivo"].ToString();
            string caminhoArquivo = Server.MapPath("~/Download/") + sNomeArquivo;
            if (!File.Exists(caminhoArquivo))
                File.WriteAllBytes(caminhoArquivo, (byte[])tb.Rows[0]["vbArquivo"]);

            DownloadArquivo(Page, sNomeArquivo);

            Pesquisar("0", hddidPedido.Value);
            Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
        }

        #endregion

        #region | Aba NFe - Documentos Fiscais

        protected void DocumentosFiscais_cmdGerarNFe_Click(object sender, EventArgs e)
        {
            var row = (sender as LinkButton).NamingContainer as GridViewRow;
            string idXML = row.Cells[nColuna_idXML].Text;
            string idEnvioOPI = row.Cells[nColuna_idEnvioOPI].Text;
            lblTitulo_EnviarSefaz.Text = "Confirma o envio para o SEFAZ?";

            hddidXML.Value = idXML;
            hddidEnvioOPI.Value = idEnvioOPI;

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "OpenEnvioSefaz", "$('#modal_EnviarSefaz').modal('show');", true);
            Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
        }

        protected void DocumentosFiscais_cmdEnviarSefaz_Click(object sender, EventArgs e)
        {
            try
            {
                string idXML = hddidXML.Value;
                string idEnvioOPI = hddidEnvioOPI.Value;
                string nNF = "";
                string sRetorno = "";
                sRetorno = Funcoes_NFe.XML.EnviarXML_SEFAZ(idXML, out nNF);
                if (sRetorno == "")
                {
                    Dictionary<string, string> vParametrosGerarNFe = new Dictionary<string, string>
                        {
                            { "@sFuncao", "GerarNFe" },
                            { "@idEnvioOPI", idEnvioOPI},
                            { "@sGerarNFe", "S" },
                            { "@sNF", nNF }
                        };
                    ExecutarDataSet(sProcedure, vParametrosGerarNFe);

                }
                else
                {
                    throw new Exception("Erro ao Gerar NFe: " + sRetorno);
                }
                Pesquisar(hddidOPI.Value, "0");
                Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
                MensagemPagina.MostraMensagem_Sucesso(string.Format("Nota Fiscal n.º {0}, Gerada com sucesso!", nNF.PadLeft(6, '0')));
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        protected void DocumentosFiscais_cmdDownloadXML_Click(object sender, EventArgs e)
        {
            Funcoes_NFe.Download.XML(Page, ((sender as LinkButton).NamingContainer as GridViewRow).Cells[nColuna_idXML].Text);
            Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
        }

        protected void DocumentosFiscais_cmdDownloadDANFE_Click(object sender, EventArgs e)
        {
            Funcoes_NFe.Download.PDF(Page, ((sender as LinkButton).NamingContainer as GridViewRow).Cells[nColuna_idXML].Text);
            Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
        }

        protected void DocumentosFiscais_cmdEnviarEmail_Click(object sender, EventArgs e)
        {
            ddlsEmailCliente.Popula_Combo("sp_Select 'Flow_Clientes_Contato', @idPesquisa=" + hddidCliente.Value, "idContato", "sEmail", false, "Selecione o email do cliente", "0");

            div_Email.Visible = true;

            var row = (sender as LinkButton).NamingContainer as GridViewRow;
            hddidEnvioEmail.Value = row.Cells[nColuna_idEnvioOPI].Text;

            string idXML = row.Cells[nColuna_idXML].Text, chaveNFe = row.Cells[nColuna_sChave].Text, baseDir = hddsCaminho_UniNFe.Value;
            string cnpjPattern = @"\d{14}", sCaminho = "", nNF = "", serie = "", mod = "";
            var cnpjDirectories = Directory.GetDirectories(baseDir);

            foreach (var cnpjDir in cnpjDirectories)
            {
                string dirName = Path.GetFileName(cnpjDir);
                if (Regex.IsMatch(dirName, cnpjPattern))
                {
                    foreach (var pasta in new string[] { Path.Combine(cnpjDir, "Enviado\\Autorizados") })
                    {
                        foreach (var arquivoPastas in Directory.GetDirectories(pasta))
                        {
                            string[] arquivos = Directory.GetFiles(arquivoPastas, "*-procNFe.xml");

                            foreach (var arquivo in arquivos)
                            {
                                string NomeArquivo = Path.GetFileName(arquivo);
                                if (NomeArquivo.Contains("-procNFe.xml"))
                                {
                                    if (NomeArquivo == chaveNFe + "-procNFe.xml")
                                    {
                                        sCaminho += arquivo;

                                        XDocument xdoc = XDocument.Load(arquivo);

                                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}mod").FirstOrDefault() != null)
                                            mod = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}mod").FirstOrDefault()?.Value;
                                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}serie").FirstOrDefault() != null)
                                            serie = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}serie").FirstOrDefault()?.Value;
                                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}nNF").FirstOrDefault() != null)
                                            nNF = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}nNF").FirstOrDefault()?.Value;
                                    }
                                }
                            }
                        }
                    }

                    foreach (var pasta in new string[] { Path.Combine(cnpjDir, "DownloadNFe") })
                    {
                        if (Directory.Exists(pasta))
                        {
                            var arquivosFiltrados = Directory.GetFiles(pasta, "*.pdf").Where(arquivo =>
                            {
                                if (arquivo.EndsWith("-procNFe.xml") && !File.Exists(Path.ChangeExtension(arquivo, ".pdf"))) return false;
                                return Path.GetFileName(arquivo).Contains(chaveNFe);
                            }).ToArray();

                            foreach (var arquivo in arquivosFiltrados)
                            {
                                string NomeArquivo = Path.GetFileName(arquivo);
                                if (NomeArquivo.Contains(".pdf") && NomeArquivo == chaveNFe + ".pdf") sCaminho += ";" + arquivo;
                            }
                        }
                    }
                }
            }

            hddchaveNFe.Value = chaveNFe;
            hddmod.Value = mod;
            hddserie.Value = serie;
            hddnNF.Value = nNF;
            hddsCaminho.Value = sCaminho;

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Mantem_aba_PDF", "$('#PDF-tab').tab('show');", true);
        }

        protected void cmdEnviarEmailSTSO_Click(object sender, EventArgs e)
        {
            if (ddlsEmailCliente.SelectedValue != "0")
            {
                try
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "EMAIL_PDF" },
                        { "@idEnvioOPI", hddidEnvioEmail.Value },
                        { "@sEmail", ddlsEmailCliente.SelectedItem.Text },
                        { "@sEmailCopia", txtsEmailCopia.Text },
                        { "@sCaminho", hddsCaminho.Value },
                        { "@sChave", hddchaveNFe.Value },
                        { "@serie", hddserie.Value },
                        { "@nNF", hddnNF.Value },
                        { "@mod", hddmod.Value },
                        { "@idUsuarioInclusao", Variaveis.idUsuario() }
                    };
                    ExecutarDataSet(sProcedure_OPI, vParametros);

                    ddlsEmailCliente.SelectedValue = "0";
                    div_Email.Visible = false;

                    MensagemPagina.MostraMensagem_Sucesso("Email enviado com sucesso!");
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
                }
            }
            else
            {
                MensagemPagina_AbaPDF.MostraMensagem_Erro("Descreva o email do destinatário");
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Mantem_aba_PDF", "$('#PDF-tab').tab('show');", true);
            }
        }

        protected void DocumentosFiscais_cmdEditarXML_Click(object sender, EventArgs e)
        {
            aba_NF.Visible = false;
            aba_Transportadora.Visible = true;
            aba_Informacoes.Visible = true;
            aba_Impostos.Visible = true;
            Scripts.Mantem_AbaAtiva(Page, "Impostos-tab");

            cmdGerarFaturamento.Visible = false;
            cmdSalvarEdicao.Visible = true;

            PopulaCombos();

            var row = (sender as LinkButton).NamingContainer as GridViewRow;
            string idEnvioOPI = row.Cells[nColuna_idEnvioOPI].Text;
            hddidEnvioOPI.Value = idEnvioOPI;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "VALOR_FATURAMENTO" },
                { "@idEnvioOPI", idEnvioOPI },
                { "@idCondicaoPagamento", hddidCondicaoPagamento.Value }
            };
            DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

            decimal.TryParse(DATASET(dsPesquisa, "nValorFaturamento"), out decimal vlrBruto);
            txtnValorBruto.Text = vlrBruto.ToString("N2");
            txtnValorOriginal.Text = vlrBruto.ToString("N2");
            ddlidEmpresa.SelectedValue = hddidEmpresa.Value;
            txtsDocumento.Text = hddnNumeroPedido.Value;
            txtsCodigo.Text = hddnNumeroPedido.Value;
            txtvlrFaturadoOriginal.Text = txtvlrFaturado.Text;
            txtdtEmissao.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtVolumes.Text = DATASET(dsPesquisa, 3, 0, "nQtdVolumes");
            txtPesoLiquido.Text = DATASET(dsPesquisa, 3, 0, "nPesoLiquido");
            txtPesoBruto.Text = DATASET(dsPesquisa, 3, 0, "nPesoBruto");
            txtEspecie.Text = DATASET(dsPesquisa, 3, 0, "sEsp");
            txtInfoFisco.Text = DATASET(dsPesquisa, 3, 0, "sInfAdFisco");
            txtInfoComplementares.Text = DATASET(dsPesquisa, 3, 0, "sInfCpl");
            txtxPed.Text = DATASET(dsPesquisa, 3, 0, "xPed");
            ddltranporte.SelectedValue = DATASET(dsPesquisa, 3, 0, "idModFrete");
            ddlidTranportadora.SelectedValue = DATASET(dsPesquisa, 3, 0, "idTransportadora");

            if (ddlidTranportadora.SelectedValue != "0")
            {
                Dictionary<string, string> vParametrosTranportadora = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_Transportadora" },
                    { "@idCliente", ddlidTranportadora.SelectedValue }
                };
                DataSet dsPesquisaTranportadora = ExecutarDataSet(sProcedure_Pedidos, vParametrosTranportadora);

                if (ValidarDataSet(dsPesquisaTranportadora, out _))
                {
                    div_InfoTransportadora.Visible = true;
                    txtCNPJTrans.Text = DATASET(dsPesquisaTranportadora, "sCPF_CNPJ");
                    txtNomeTrans.Text = DATASET(dsPesquisaTranportadora, "sRazaoSocial");
                    txtIETrans.Text = DATASET(dsPesquisaTranportadora, "sRG_IE");

                    if (txtIETrans.Text == "") DIV_IETrans.Visible = false;
                    else DIV_IETrans.Visible = true;

                    txtEnderecoTrans.Text = DATASET(dsPesquisaTranportadora, "sLogradouro");
                    txtMunicipiotrans.Text = DATASET(dsPesquisaTranportadora, "sCidade");
                    txtUFTrans.Text = DATASET(dsPesquisaTranportadora, "sEstado");

                    lblTitulo_Modal_Receber.InnerText = "Editar XML - Envio " + idEnvioOPI;
                }
            }
            else
            {
                div_InfoTransportadora.Visible = false;
                txtCNPJTrans.Text = "";
                txtNomeTrans.Text = "";
                txtIETrans.Text = "";
                txtEnderecoTrans.Text = "";
                txtMunicipiotrans.Text = "";
                txtUFTrans.Text = "";
            }

            Scripts.AbrirModal(Page, "Modal_Receber");
        }

        protected void cmdSalvarEdicao_Click(object sender, EventArgs e)
        {
            if (ddltranporte.SelectedValue != "")
            {
                try
                {
                    string sNomeArquivo = "";

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR_TRANS_FATURAMENTO" },
                        { "@idTransportadora", ddlidTranportadora.SelectedValue },
                        { "@idModFrete", ddltranporte.SelectedValue },
                        { "@sEsp", txtEspecie.Text },
                        { "@sInfCpl", txtInfoComplementares.Text },
                        { "@sInfAdFisco", txtInfoFisco.Text },
                        { "@idEnvioOPI", hddidEnvioOPI.Value },
                        { "@sGerarNFe", "S" },
                        { "@xPed", txtxPed.Text }
                    };
                    DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

                    foreach (GridViewRow row in gvDocumentosFiscais.Rows)
                    {
                        hddidArquivo.Value = row.Cells[14].Text;

                        if (hddidEnvioOPI.Value == (row.FindControl("idEnvioOPI") as Label).Text)
                        {
                            if (hddidArquivo.Value != "0" && hddidArquivo.Value != "&nbsp;" && hddidArquivo.Value != "")
                            {
                                byte[] bObjArquivo = null;

                                Dictionary<string, string> vParametrosItem = new Dictionary<string, string>
                                {
                                    { "@sFuncao", "CONSULTAR_DETALHE" },
                                    { "@idArquivo", hddidArquivo.Value }
                                };
                                DataTable dtArquivo = ExecutarDataTable(sProcedure_Arquivos, vParametrosItem);

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
                                }
                            }
                        }
                    }

                    string chave = "", CNPJ = "", Arquivo = Server.MapPath("~/Download/" + sNomeArquivo);
                    XDocument xdoc = XDocument.Load(Arquivo);
                    var infNFeElement = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}infNFe").FirstOrDefault();

                    if (infNFeElement != null) chave = infNFeElement.Attribute("Id")?.Value?.Substring(3);

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                        CNPJ = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}modFrete").FirstOrDefault() != null)
                        if (ddltranporte.SelectedValue != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}modFrete").FirstOrDefault()?.Value)
                            xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}modFrete").FirstOrDefault().Value = ddltranporte.SelectedValue;

                    var transpElement = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").FirstOrDefault();
                    if (transpElement == null)
                    {
                        transpElement = new XElement("{http://www.portalfiscal.inf.br/nfe}transp");
                        xdoc.Root.Add(transpElement);
                    }

                    var transportaElement = transpElement.Descendants("{http://www.portalfiscal.inf.br/nfe}transporta").FirstOrDefault();
                    if (transportaElement == null)
                    {
                        transportaElement = new XElement("{http://www.portalfiscal.inf.br/nfe}transporta");

                        var volElement = transpElement.Descendants("{http://www.portalfiscal.inf.br/nfe}vol").FirstOrDefault();
                        if (volElement != null) volElement.AddBeforeSelf(transportaElement); // Adiciona o <transporta> acima do <vol>
                        else transpElement.Add(transportaElement); // Se <vol> não existir, adiciona <transporta> no final de <transp>
                    }

                    var cnpjElement = transportaElement.Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault();
                    if (!string.IsNullOrEmpty(txtCNPJTrans.Text))
                    {
                        if (cnpjElement == null)
                        {
                            cnpjElement = new XElement("{http://www.portalfiscal.inf.br/nfe}CNPJ", txtCNPJTrans.Text);
                            transportaElement.Add(cnpjElement);
                        }
                        else cnpjElement.Value = txtCNPJTrans.Text;
                    }
                    else cnpjElement?.Remove();

                    var nomeElement = transportaElement.Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault();
                    if (!string.IsNullOrEmpty(txtNomeTrans.Text))
                    {
                        if (nomeElement == null)
                        {
                            nomeElement = new XElement("{http://www.portalfiscal.inf.br/nfe}xNome", txtNomeTrans.Text);
                            transportaElement.Add(nomeElement);
                        }
                        else nomeElement.Value = txtNomeTrans.Text;
                    }
                    else nomeElement?.Remove();

                    var enderecoElement = transportaElement.Descendants("{http://www.portalfiscal.inf.br/nfe}xEnder").FirstOrDefault();
                    if (!string.IsNullOrEmpty(txtEnderecoTrans.Text))
                    {
                        if (enderecoElement == null)
                        {
                            enderecoElement = new XElement("{http://www.portalfiscal.inf.br/nfe}xEnder", txtEnderecoTrans.Text.ToString());
                            transportaElement.Add(enderecoElement);
                        }
                        else enderecoElement.Value = txtEnderecoTrans.Text;
                    }
                    else enderecoElement?.Remove();

                    var municipioElement = transportaElement.Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault();
                    if (!string.IsNullOrEmpty(txtMunicipiotrans.Text))
                    {
                        if (municipioElement == null)
                        {
                            municipioElement = new XElement("{http://www.portalfiscal.inf.br/nfe}xMun", txtMunicipiotrans.Text);
                            transportaElement.Add(municipioElement);
                        }
                        else municipioElement.Value = txtMunicipiotrans.Text;
                    }
                    else municipioElement?.Remove();

                    var ufElement = transportaElement.Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault();
                    if (!string.IsNullOrEmpty(txtUFTrans.Text))
                    {
                        if (ufElement == null)
                        {
                            ufElement = new XElement("{http://www.portalfiscal.inf.br/nfe}UF", txtUFTrans.Text);
                            transportaElement.Add(ufElement);
                        }
                        else ufElement.Value = txtUFTrans.Text;
                    }
                    else ufElement?.Remove();

                    var ieElement = transportaElement.Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault();
                    if (!string.IsNullOrEmpty(txtIETrans.Text))
                    {
                        if (ieElement == null)
                        {
                            ieElement = new XElement("{http://www.portalfiscal.inf.br/nfe}IE", txtIETrans.Text);
                            transportaElement.Add(ieElement);
                        }
                        else ieElement.Value = txtIETrans.Text;
                    }
                    else ieElement?.Remove();

                    bool shouldRemoveTransporta = !transportaElement.Descendants().Any(elem => !string.IsNullOrEmpty(elem.Value));
                    if (shouldRemoveTransporta) transportaElement.Remove();

                    var volElemento = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}vol").FirstOrDefault();
                    var especieElement = volElemento?.Element("{http://www.portalfiscal.inf.br/nfe}esp");

                    if (!string.IsNullOrEmpty(txtEspecie.Text))
                    {
                        if (especieElement == null)
                        {
                            especieElement = new XElement("{http://www.portalfiscal.inf.br/nfe}esp", txtEspecie.Text);

                            var qVolElement = volElemento?.Element("{http://www.portalfiscal.inf.br/nfe}qVol");

                            if (qVolElement != null) qVolElement.AddAfterSelf(especieElement); // Adiciona <esp> logo após <qVol>
                            else volElemento?.AddFirst(especieElement); // Fallback: adiciona no início de <vol> se <qVol> não existir
                        }
                        else especieElement.Value = txtEspecie.Text;
                    }
                    else especieElement?.Remove();

                    if (!string.IsNullOrEmpty(txtInfoFisco.Text) || !string.IsNullOrEmpty(txtInfoComplementares.Text))
                    {
                        if (infNFeElement != null)
                        {
                            var infAdicElement = infNFeElement.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").FirstOrDefault();
                            if (infAdicElement == null)
                            {
                                infAdicElement = new XElement("{http://www.portalfiscal.inf.br/nfe}infAdic");
                                infNFeElement.Add(infAdicElement);
                            }

                            if (!string.IsNullOrEmpty(txtInfoFisco.Text))
                            {
                                var infAdFiscoElement = infAdicElement.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdFisco").FirstOrDefault();
                                if (infAdFiscoElement == null)
                                {
                                    infAdFiscoElement = new XElement("{http://www.portalfiscal.inf.br/nfe}infAdFisco", txtInfoFisco.Text);
                                    infAdicElement.Add(infAdFiscoElement);
                                }
                                else infAdFiscoElement.Value = txtInfoFisco.Text;
                            }
                            else
                            {
                                var infAdFiscoElement = infAdicElement.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdFisco").FirstOrDefault();
                                infAdFiscoElement?.Remove();
                            }

                            if (!string.IsNullOrEmpty(txtInfoComplementares.Text))
                            {
                                var infCplElement = infAdicElement.Descendants("{http://www.portalfiscal.inf.br/nfe}infCpl").FirstOrDefault();
                                if (infCplElement == null)
                                {
                                    infCplElement = new XElement("{http://www.portalfiscal.inf.br/nfe}infCpl", txtInfoComplementares.Text);
                                    infAdicElement.Add(infCplElement);
                                }
                                else infCplElement.Value = txtInfoComplementares.Text;
                            }
                            else
                            {
                                var infCplElement = infAdicElement.Descendants("{http://www.portalfiscal.inf.br/nfe}infCpl").FirstOrDefault();
                                infCplElement?.Remove();
                            }
                        }
                    }
                    else xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").FirstOrDefault()?.Remove();

                    xdoc.Save(Arquivo);

                    string Corpo = "";
                    XDocument novo = XDocument.Load(Arquivo);
                    if (novo.Descendants("{http://www.portalfiscal.inf.br/nfe}nfeProc").FirstOrDefault() != null)
                        Corpo = novo.Descendants("{http://www.portalfiscal.inf.br/nfe}nfeProc").FirstOrDefault().ToString();
                    if (novo.Descendants("{http://www.portalfiscal.inf.br/nfe}NFe").FirstOrDefault() != null)
                        Corpo = novo.Descendants("{http://www.portalfiscal.inf.br/nfe}NFe").FirstOrDefault().ToString();

                    Byte[] lObjArquivoAlterar = null;
                    Arquivo objArquivoAlterar = new Arquivo();
                    try
                    {
                        using (FileStream fileStream = new FileStream(Arquivo, FileMode.Open, FileAccess.Read))
                        {
                            lObjArquivoAlterar = objArquivoAlterar.TransformaArquivoEmArrayBytes(Arquivo, fileStream);
                        }

                        if (lObjArquivoAlterar != null && lObjArquivoAlterar.Length > 0)
                        {
                            SqlDataAdapter da = new SqlDataAdapter(sProcedure_Arquivos, StringDeConexao);

                            DataSet tabela = new DataSet();
                            da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "ALTERARXML";
                            da.SelectCommand.Parameters.Add("@vbArquivo", SqlDbType.VarBinary).Value = lObjArquivoAlterar;
                            da.SelectCommand.Parameters.Add("@idArquivo", SqlDbType.Int).Value = Convert.ToInt32(hddidArquivo.Value);

                            try
                            {
                                da.Fill(tabela);
                            }
                            catch { }
                        }
                        else MensagemPagina.MostraMensagem_Erro("Erro: O arquivo não foi convertido corretamente em bytes.");
                    }
                    catch (Exception ex)
                    {
                        MensagemPagina.MostraMensagem_Erro(string.Format("Erro ao ler o arquivo: {0}", ex.Message));
                    }

                    if (File.Exists(Arquivo))
                    {
                        try
                        {
                            string cnpjPattern = @"\d{14}";
                            var cnpjDirectories = Directory.GetDirectories(hddsCaminho_UniNFe.Value);

                            foreach (var cnpjDir in cnpjDirectories)
                            {
                                string dirName = Path.GetFileName(cnpjDir);
                                if (Regex.IsMatch(dirName, cnpjPattern))
                                {
                                    if (dirName == CNPJ)
                                    {
                                        string[] pastas = new string[] { Path.Combine(cnpjDir, "Envio") };

                                        foreach (var pasta in pastas)
                                        {
                                            string nomeArquivoDestino = Path.Combine(pasta, Path.GetFileName(chave + "-nfe" + ".xml"));
                                            File.Copy(Arquivo, nomeArquivoDestino);
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }

                    Dictionary<string, string> vParametrosArquivo = new Dictionary<string, string>
                    {
                        { "@sFuncao", "VerificaArquivo" },
                        { "@sChaveNFe", chave },
                        { "@sPastaAlterar", "Pendente de Envio" },
                        { "@sObservacao", "" }
                    };
                    DataSet dsPesquisaArquivo = ExecutarDataSet(sProcedure_XML, vParametrosArquivo);

                    Dictionary<string, string> vParametrosAlterar = new Dictionary<string, string>
                    {
                        { "@sFuncao", "ALTERARXML" },
                        { "@sChaveNFe", chave },
                        { "@sXML", Corpo }
                    };
                    DataSet dsPesquisaAlterar = ExecutarDataSet(sProcedure_XML, vParametrosAlterar);

                    Pesquisar(hddidOPI.Value, "0");
                    MensagemPagina.MostraMensagem_Sucesso("Editado com sucesso!");
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
                }
            }
            else
            {
                MensagemPagina_Modal_Receber.MostraMensagem_Erro("Selecione o Modo Frete!");
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_openModal", "window.onload = function() { $('#Modal_Receber').modal('show'); };", true);
            }
        }

        protected void DocumentosFiscais_cmdCartaCorrecao_Click(object sender, EventArgs e)
        {

            var row = (sender as LinkButton).NamingContainer as GridViewRow;
            string idEnvioOPI = row.Cells[nColuna_idEnvioOPI].Text;
            string sChaveNFe = row.Cells[nColuna_sChave].Text;

            hddidEnvioOPI.Value = idEnvioOPI;
            hddsChave.Value = sChaveNFe;
            hddnCartaCorrecao.Value = row.Cells[nColuna_nNumeroCartaCorrecao].Text;

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "OpenCartacorrecao", "$('#Modal_Cartacorrecao').modal('show');", true);
            Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
        }

        #region | Carta de Correção

        protected void btnSalvaCorrecao_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCorrecao.Text != "" && txtCorrecao.Text.Length > 15)
                {
                    string CNPJ = "", cOrgao = "", tpAmb = "", chave = "", cnpjPattern = @"\d{14}", chaveNFe = hddsChave.Value, baseDir = hddsCaminho_UniNFe.Value;
                    string sCaminhoArquivo_XML = Funcoes_NFe.XML.GerarArquivo("", chaveNFe);
                    string sCaminhoArquivo_Correcao = HttpContext.Current.Server.MapPath("~/Download/" + string.Format("{0}_procEventoNFe.xml", chaveNFe));

                    int nNumeroCartaCorrecao = int.Parse(hddnCartaCorrecao.Value) + 1;
                    var cnpjDirectories = Directory.GetDirectories(baseDir);

                    XDocument xdoc = XDocument.Load(sCaminhoArquivo_XML);

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                        CNPJ = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}cUF").FirstOrDefault() != null)
                        cOrgao = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}cUF").FirstOrDefault()?.Value;

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpAmb").FirstOrDefault() != null)
                        tpAmb = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpAmb").FirstOrDefault()?.Value;

                    var infNFeElement = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}infNFe").FirstOrDefault();
                    if (infNFeElement != null) chave = infNFeElement.Attribute("Id")?.Value?.Substring(3);

                    using (XmlWriter writer = XmlWriter.Create(sCaminhoArquivo_Correcao, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("envEvento", "http://www.portalfiscal.inf.br/nfe");
                        writer.WriteAttributeString("versao", "1.00");
                        writer.WriteElementString("idLote", "000000000000001");

                        writer.WriteStartElement("evento", "http://www.portalfiscal.inf.br/nfe");
                        writer.WriteAttributeString("versao", "1.00");
                        writer.WriteStartElement("infEvento");
                        writer.WriteAttributeString("Id", "ID110110" + chave + nNumeroCartaCorrecao.ToString("D2"));

                        writer.WriteElementString("cOrgao", cOrgao);
                        writer.WriteElementString("tpAmb", tpAmb);
                        writer.WriteElementString("CNPJ", CNPJ);
                        writer.WriteElementString("chNFe", chave);
                        writer.WriteElementString("dhEvento", DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        writer.WriteElementString("tpEvento", "110110");
                        writer.WriteElementString("nSeqEvento", nNumeroCartaCorrecao.ToString());
                        writer.WriteElementString("verEvento", "1.00");

                        writer.WriteStartElement("detEvento");
                        writer.WriteAttributeString("versao", "1.00");

                        writer.WriteElementString("descEvento", "Carta de Correção");
                        writer.WriteElementString("xCorrecao", txtCorrecao.Text);
                        writer.WriteElementString("xCondUso", "A Carta de Correção é disciplinada pelo § 1º-A do art. 7º do Convênio S/N, de 15 de dezembro de 1970 e pode ser utilizada " +
                            "para regularização de erro ocorrido na emissão de documento fiscal, desde que o erro não esteja relacionado com: I - as variáveis que determinam o valor do imposto " +
                            "tais como: base de cálculo, alíquota, diferença de preço, quantidade, valor da operação ou da prestação; II - a correção de dados cadastrais que implique mudança do " +
                            "remetente ou do destinatário; III - a data de emissão ou de saída.");
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }

                    string nomeArquivoDestino = "";
                    if (File.Exists(sCaminhoArquivo_Correcao))
                    {
                        try
                        {
                            foreach (var cnpjDir in cnpjDirectories)
                            {
                                string dirName = Path.GetFileName(cnpjDir);
                                if (Regex.IsMatch(dirName, cnpjPattern))
                                {
                                    if (dirName == CNPJ)
                                    {
                                        foreach (var pasta in new string[] { Path.Combine(cnpjDir, "Envio") })
                                        {
                                            nomeArquivoDestino = Path.Combine(pasta, hddsChave.Value + -110110 + "-" + nNumeroCartaCorrecao.ToString("D2") + "-ped-eve.xml");
                                            File.Copy(sCaminhoArquivo_Correcao, nomeArquivoDestino);
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }

                    Dictionary<string, string> vParametros_CCe = new Dictionary<string, string>
                    {
                        { "@sFuncao", "REGISTRA_CCE" },
                        { "@sObservacao", txtCorrecao.Text },
                        { "@sChaveNFe", chave },
                        { "@nNumeroCartaCorrecao", nNumeroCartaCorrecao.ToString() }
                    };
                    ExecutarDataSet(sProcedure_XML, vParametros_CCe);

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CARTA_CORRECAO" },
                        { "@sCartaCorrecao", "S" },
                        { "@nCartaCorrecao", nNumeroCartaCorrecao.ToString() },
                        { "@idEnvioOPI", hddidEnvioOPI.Value }
                    };
                    ExecutarDataSet(sProcedure_OPI, vParametros);

                    MensagemPagina.MostraMensagem_Sucesso("Carta de correção enviado com sucesso.");
                    Pesquisar(hddidOPI.Value, "0");
                }
                else
                {
                    if (txtCorrecao.Text == "") MensagemPagina3.MostraMensagem_Erro("Descreva a Correção");
                    else if (txtCorrecao.Text.Length < 15) MensagemPagina3.MostraMensagem_Erro("O campo deve conter no mínimo 15 caracteres.");

                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "OpenCartacorrecao", "$('#Modal_Cartacorrecao').modal('show');", true);
                }

                Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
            }
            catch (Exception ex)
            {
                MensagemPagina3.MostraMensagem_Erro(ex.Message);
            }
        }

        protected void DocumentosFiscais_cmdDownloadCCeXML_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (sender as LinkButton).NamingContainer as GridViewRow;
                Funcoes_CCe.Download_XML(Page, row.Cells[nColuna_sChaveNFe].Text, row.Cells[nColuna_nNumeroCartaCorrecao].Text);
                Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        protected void DocumentosFiscais_cmdDownloadCCePDF_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (sender as LinkButton).NamingContainer as GridViewRow;
                Funcoes_CCe.Download_PDF(Page, row.Cells[nColuna_sChaveNFe].Text, row.Cells[nColuna_nNumeroCartaCorrecao].Text);
                Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        #endregion

        #region | Cancelamento

        protected void cmdCancelarNFE_Click(object sender, EventArgs e)
        {
            var row = (sender as LinkButton).NamingContainer as GridViewRow;

            hddsChave.Value = row.Cells[nColuna_sChave].Text;
            hddidXML.Value = row.Cells[nColuna_idXML].Text;

            if (row.Cells[nColuna_sTipo].Text == "NFS-e")
            {
                hddidPedido.Value = row.Cells[nColuna_idEnvioOPI].Text;
                hddidFaturamento_Cancela.Value = row.Cells[nColuna_idFaturamento].Text;
            }
            else hddidEnvioOPI.Value = row.Cells[nColuna_idEnvioOPI].Text;

            Scripts.AbrirModal(Page, "Modal_Cancelamento");
            Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
        }

        protected void btnSalvarCancelamento_Click(object sender, EventArgs e)
        {
            try
            {
                string sEspelho = "S";

                int.TryParse(hddidEnvioOPI.Value, out int idEnvioOPI);
                int.TryParse(hddidPedido.Value, out int idPedido);

                if (txtsJustificativa.Text.Length >= 15)
                {
                    string sNomeArquivoCancelamento = "", CNPJ = "", cOrgao = "", tpAmb = "", chave = "", nProt = "", cnpjPattern = @"\d{14}", chaveNFe = hddsChave.Value, baseDir = hddsCaminho_UniNFe.Value;
                    string idTipoObjeto = "";
                    Dictionary<string, string> vParametrosXML = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idXML", hddidXML.Value }
                    };
                    DataSet ds = ExecutarDataSet(sProcedure_XML, vParametrosXML);

                    sEspelho = DATASET(ds, "sEspelho");
                    idTipoObjeto = DATASET(ds, "idTipoObjeto");

                    if (sEspelho == "N" && idPedido <= 0)
                    {
                        var cnpjDirectories = Directory.GetDirectories(baseDir);
                        chaveNFe = DATASET(ds, "chNFe");
                        sNomeArquivoCancelamento = HttpContext.Current.Server.MapPath("~/Download/" + string.Format("{0}_Cancelamento.xml", chaveNFe));
                        string sNomeArquivo = string.Format("{0}.xml", DATASET(ds, "chNFe"));
                        File.WriteAllText(HttpContext.Current.Server.MapPath("~/Download/" + sNomeArquivo), DATASET(ds, "sXML"));

                        XDocument xdoc = XDocument.Load(HttpContext.Current.Server.MapPath("~/Download/" + sNomeArquivo));

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                            CNPJ = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}cUF").FirstOrDefault() != null)
                            cOrgao = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}cUF").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpAmb").FirstOrDefault() != null)
                            tpAmb = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpAmb").FirstOrDefault()?.Value;

                        var infNFeElement = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}infNFe").FirstOrDefault();
                        if (infNFeElement != null) chave = infNFeElement.Attribute("Id")?.Value?.Substring(3);

                        nProt = DATASET(ds, "snProt");

                        using (XmlWriter writer = XmlWriter.Create(sNomeArquivoCancelamento, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
                        {
                            writer.WriteStartDocument();
                            writer.WriteStartElement("envEvento", "http://www.portalfiscal.inf.br/nfe");
                            writer.WriteAttributeString("versao", "1.00");
                            writer.WriteElementString("idLote", "000000000000001");

                            writer.WriteStartElement("evento");
                            writer.WriteAttributeString("versao", "1.00");
                            writer.WriteStartElement("infEvento");
                            writer.WriteAttributeString("Id", "ID110111" + chave + "01");

                            writer.WriteElementString("cOrgao", cOrgao);
                            writer.WriteElementString("tpAmb", tpAmb);
                            writer.WriteElementString("CNPJ", CNPJ);
                            writer.WriteElementString("chNFe", chave);
                            writer.WriteElementString("dhEvento", DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                            writer.WriteElementString("tpEvento", "110111");
                            writer.WriteElementString("nSeqEvento", "1");
                            writer.WriteElementString("verEvento", "1.00");

                            writer.WriteStartElement("detEvento");
                            writer.WriteAttributeString("versao", "1.00");

                            writer.WriteElementString("descEvento", "Cancelamento");
                            writer.WriteElementString("nProt", nProt);
                            writer.WriteElementString("xJust", txtsJustificativa.Text.Trim());
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }

                        string nomeArquivoDestino = "";
                        if (File.Exists(sNomeArquivoCancelamento))
                        {
                            try
                            {
                                foreach (var cnpjDir in cnpjDirectories)
                                {
                                    string dirName = Path.GetFileName(cnpjDir);
                                    if (Regex.IsMatch(dirName, cnpjPattern))
                                    {
                                        if (dirName == CNPJ)
                                        {
                                            string[] pastas = new string[] { Path.Combine(cnpjDir, "Envio") };

                                            foreach (var pasta in pastas)
                                            {
                                                nomeArquivoDestino = Path.Combine(pasta, chaveNFe + -110111 + "-" + "01" + "-ped-eve.xml");
                                                File.Copy(sNomeArquivoCancelamento, nomeArquivoDestino);
                                            }
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }

                    if (idEnvioOPI > 0)
                    {
                        if (idTipoObjeto != "14") //14=Devolução
                        {
                            Dictionary<string, string> vParametros = new Dictionary<string, string>
                            {
                                { "@sFuncao", "Inativar_ContasReceber" },
                                { "@idEnvioOPI", hddidEnvioOPI.Value }
                            };
                            ExecutarDataSet(sProcedure, vParametros);
                        }
                        Dictionary<string, string> vParametros_CCe = new Dictionary<string, string>
                        {
                            { "@sFuncao", "REGISTRA_CANCELAMENTO" },
                            { "@sObservacao", txtsJustificativa.Text },
                            { "@sChaveNFe", chaveNFe }
                        };
                        ExecutarDataSet(sProcedure_XML, vParametros_CCe);
                    }
                    else if (idPedido > 0)
                    {
                        Dictionary<string, string> vParam = new Dictionary<string, string>
                        {
                            { "@sFuncao", "CANCELA_NFS" },
                            { "@idFaturamento", hddidFaturamento_Cancela.Value },
                            { "@idPedido", hddidPedido.Value },
                            { "@sObservacao", txtsJustificativa.Text },
                            { "@sChaveNFe", chaveNFe }
                        };
                        ExecutarDataSet(sProcedure_XML, vParam);
                    }

                    Pesquisar(hddidOPI.Value, hddidPedido.Value);
                    MensagemPagina.MostraMensagem_Sucesso("Cancelamento/Exclusão enviado com sucesso!");
                }
                else throw new Exception("A justificativa do cancelamento/exclusão deve conter ao menos 15 caracteres!");
            }
            catch (Exception ex)
            {
                MensagemPagina_Modal_Cancelamento.MostraMensagem_Erro(ex.Message);
                Scripts.RemoverBackdrop_Modal(Page);
                Scripts.AbrirModal(Page, "Modal_Cancelamento");
            }

            Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
        }

        #endregion

        protected void DocumentosFiscais_cmdDevolucao_Click(object sender, EventArgs e)
        {
            try
            {
                Scripts.Mantem_AbaAtiva(Page, "PDF-tab");
                div_Fatumento_NF.Visible = false;
                aba_ItensDevolucao.Visible = true;
                aba_Transportadora.Visible = true;
                aba_Informacoes.Visible = true;
                aba_NF.Visible = false;

                cmdEmitirNFS.Visible = false;
                txtEspecie.ReadOnly = false;
                txtInfoFisco.ReadOnly = false;
                txtInfoComplementares.ReadOnly = false;
                ddltranporte.Attributes.Remove("disabled");
                ddlidTranportadora.Attributes.Remove("disabled");
                hddidArquivo.Value = "";

                var row = (sender as LinkButton).NamingContainer as GridViewRow;
                hddsChave.Value = row.Cells[nColuna_sChave].Text;
                hddnCartaCorrecao.Value = row.Cells[nColuna_nNumeroCartaCorrecao].Text;
                hddidEnvioOPI.Value = row.Cells[nColuna_idEnvioOPI].Text;

                ddlidCFOP.Popula_Combo("sp_Select 'tbl_Flow_Adm_CFOP_DEVOLUCAO'", "idCFOP", "sCFOP", false, "Selecione o CFOP", "0");

                Dictionary<string, string> vParametrosCFOP = new Dictionary<string, string>
                {
                    { "@sFuncao", "COLSULTAR_CFOP" },
                    { "@idItem", hddidFaturamento.Value }
                };
                DataSet dsPesquisaCFOP = ExecutarDataSet(sProcedure_Pedidos, vParametrosCFOP);

                ddlidCFOP.SelectedValue = DATASET(dsPesquisaCFOP, 0, "idCFOP");

                chkExibirFabricante.Visible = false;
                cmdGerarFaturamento.Visible = false;
                cmdSalvarEdicao.Visible = false;
                btnSalvarDevolucao.Visible = true;
                lblTitulo_Modal_Receber.InnerText = "Devolução";

                Scripts.Mantem_AbaAtiva(Page, "PDF-tab");

                PopulaCombos();

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "VALOR_FATURAMENTO" },
                    { "@idEnvioOPI", hddidEnvioOPI.Value },
                    { "@idCondicaoPagamento", hddidCondicaoPagamento.Value }
                };
                DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

                ddlidEmpresa.SelectedValue = hddidEmpresa.Value;
                txtsDocumento.Text = hddnNumeroPedido.Value;
                txtsCodigo.Text = hddnNumeroPedido.Value;
                txtvlrFaturadoOriginal.Text = txtvlrFaturado.Text;
                decimal vlrBruto = decimal.Parse(DATASET(dsPesquisa, 0, "nValorFaturamento"));
                txtnValorBruto.Text = vlrBruto.ToString("N2");
                txtnValorOriginal.Text = vlrBruto.ToString("N2");
                txtdtEmissao.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtVolumes.Text = DATASET(dsPesquisa, 3, 0, "nQtdVolumes");
                txtPesoLiquido.Text = DATASET(dsPesquisa, 3, 0, "nPesoLiquido");
                txtPesoBruto.Text = DATASET(dsPesquisa, 3, 0, "nPesoBruto");
                txtEspecie.Text = DATASET(dsPesquisa, 3, 0, "sEsp");
                txtInfoFisco.Text = DATASET(dsPesquisa, 3, 0, "sInfAdFisco");
                txtInfoComplementares.Text = DATASET(dsPesquisa, 3, 0, "sInfCpl");
                txtxPed.Text = DATASET(dsPesquisa, 3, 0, "xPed");
                ddltranporte.SelectedValue = DATASET(dsPesquisa, 3, 0, "idModFrete");
                ddlidTranportadora.SelectedValue = DATASET(dsPesquisa, 3, 0, "idTransportadora");

                if (ddlidTranportadora.SelectedValue != "0")
                {
                    Dictionary<string, string> vParametrosTranportadora = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_Transportadora" },
                        { "@idCliente", ddlidTranportadora.SelectedValue }
                    };
                    DataSet dsPesquisaTranportadora = ExecutarDataSet(sProcedure_Pedidos, vParametrosTranportadora);

                    if (ValidarDataSet(dsPesquisaTranportadora, out _))
                    {
                        div_InfoTransportadora.Visible = true;
                        txtCNPJTrans.Text = DATASET(dsPesquisaTranportadora, "sCPF_CNPJ");
                        txtNomeTrans.Text = DATASET(dsPesquisaTranportadora, "sRazaoSocial");
                        txtIETrans.Text = DATASET(dsPesquisaTranportadora, "sRG_IE");
                        txtEnderecoTrans.Text = DATASET(dsPesquisaTranportadora, "sLogradouro");
                        txtMunicipiotrans.Text = DATASET(dsPesquisaTranportadora, "sCidade");
                        txtUFTrans.Text = DATASET(dsPesquisaTranportadora, "sEstado");

                        if (txtIETrans.Text == "") DIV_IETrans.Visible = false;
                        else DIV_IETrans.Visible = true;
                    }
                }

                DIV_ItensDevolucao.Visible = true;

                if (row.Cells[12].Text != "Devolução" && row.Cells[12].Text != "Devolu&#231;&#227;o")
                {
                    string chaveNFe = row.Cells[nColuna_sChave].Text, sItensDevolucao = "";
                    if (sItensDevolucao != "&nbsp;" && sItensDevolucao != "")
                    {
                        var itensDict = sItensDevolucao.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Split('|')).GroupBy(x => x[0]).ToDictionary(g => g.Key, g => g.Sum(x => int.Parse(x[1])));

                        foreach (DataRow rows in dtEnvios.Rows.Cast<DataRow>().ToList())
                        {
                            string codigo = rows["sCodigo"].ToString();

                            if (itensDict.ContainsKey(codigo))
                            {
                                decimal quantidadeInformada = itensDict[codigo];
                                decimal quantidadeTabela = decimal.Parse(rows["nQuantidade"].ToString());

                                if (quantidadeInformada == quantidadeTabela)
                                    dtEnvios.Rows.Remove(rows);
                                else
                                {
                                    rows["nQuantidade"] = quantidadeTabela - quantidadeInformada;
                                    decimal quantidade = quantidadeTabela - quantidadeInformada;
                                    decimal nValorUnitario = decimal.Parse(rows["nValorUnitario"].ToString());
                                    decimal unitario = nValorUnitario * quantidade;
                                    rows["nVlrTotal"] = unitario.ToString("N2");
                                }
                            }
                        }

                        gvItensDevolucao.DataSource = dtEnvios;
                        gvItensDevolucao.DataBind();
                    }
                    else
                    {
                        DataTable dt = dtEnvios.Copy();

                        int contador = 0;
                        while (true)
                        {
                            if (dt.Rows[contador].Field<int>("idEnvioOPI") != int.Parse(hddidEnvioOPI.Value))
                                dt.Rows.RemoveAt(contador);
                            else contador++;

                            if (contador >= dt.Rows.Count)
                                break;
                        }

                        gvItensDevolucao.DataSource = dt;
                        gvItensDevolucao.DataBind();
                    }
                }

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "OpenDevolucao", "$('#Modal_Receber').modal('show');", true);
                Scripts.Mantem_AbaAtiva(Page, "Transportadora-tab");
            }
            catch { }
        }

        private bool ValidarDevolucao()
        {
            string sMensagem = "";
            bool AlgumSelecionado = false;

            foreach (GridViewRow row in gvItensDevolucao.Rows)
            {
                TextBox txtnQuantidadeEnvio = (TextBox)row.FindControl("txtnQuantidadeEnvio");
                if (!string.IsNullOrEmpty(txtnQuantidadeEnvio.Text))
                {
                    AlgumSelecionado = true;


                    if (txtnQuantidadeEnvio == null || string.IsNullOrWhiteSpace(txtnQuantidadeEnvio.Text))
                        sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva o quantidade de devolução do item " + row.Cells[3].Text + " !";

                    if (decimal.TryParse(txtnQuantidadeEnvio.Text, NumberStyles.Number, new CultureInfo("pt-BR"), out decimal valorDigitado) && decimal.TryParse(row.Cells[6].Text, NumberStyles.Number, new CultureInfo("pt-BR"), out decimal valorMaximo))
                    {
                        if (valorDigitado > valorMaximo)
                            sMensagem += (sMensagem != "" ? "</br>" : "") + "Quantidade de devolução maior que permitido do item" + row.Cells[3].Text + " !";
                    }
                }
            }

            if (!AlgumSelecionado) sMensagem += (sMensagem != "" ? "</br>" : "") + "Informe a quantidade dos itens que será efetuada a devolução!";
            if (ddltranporte.SelectedValue == "") sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o modo do frete!";
            if (ddlidCFOP.SelectedValue == "0") sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o CFOP!";


            if (sMensagem != "")
            {
                MensagemPagina_Modal_Receber.MostraMensagem_Erro(sMensagem);
                return false;
            }

            return true;
        }

        protected void btnSalvarDevolucao_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidarDevolucao())
                {
                    string vItensDevolver = "", status = "C";

                    foreach (GridViewRow row in gvItensDevolucao.Rows)
                    {
                        //CheckBox chkDevolucao_Seleciona = (CheckBox)row.FindControl("chkDevolucao_Seleciona");
                        TextBox txtnQuantidadeDevolucao = row.FindControl("txtnQuantidadeEnvio") as TextBox;

                        if (!string.IsNullOrEmpty(txtnQuantidadeDevolucao.Text))
                        {
                            vItensDevolver += row.Cells[2].Text + "|" + txtnQuantidadeDevolucao.Text + ",";

                            if (decimal.TryParse(row.Cells[6].Text.Trim(), NumberStyles.Any, new CultureInfo("pt-BR"), out decimal quantidadeDecimal))
                            {
                                if (txtnQuantidadeDevolucao.Text != ((int)quantidadeDecimal).ToString())
                                    status = "P";
                            }
                        }
                        else status = "P";
                    }

                    string sUsarProdutosCliente = chksUsarProdutosCliente.Checked ? "S" : "N";

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR_TRANS_FATURAMENTO" },
                        { "@idTransportadora", ddlidTranportadora.SelectedValue },
                        { "@idModFrete", ddltranporte.SelectedValue },
                        { "@sEsp", txtEspecie.Text },
                        { "@sInfCpl", txtInfoComplementares.Text },
                        { "@sInfAdFisco", txtInfoFisco.Text },
                        { "@idEnvioOPI", hddidEnvioOPI.Value },
                        { "@sGerarNFe", "N" },
                        { "@xPed", txtxPed.Text },
                        { "@sUsarProdutosCliente", sUsarProdutosCliente }
                    };
                    DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

                    string chaveNFe = hddsChave.Value;
                    string baseDir = hddsCaminho_UniNFe.Value;
                    string cnpjPattern = @"\d{14}";
                    string Remessa = "N";
                    string filePath = Funcoes_NFe.XML_GerarNFE.GerarXML(hddidFaturamento.Value, hddnNumeroPedido.Value, Page, hddidEmpresa.Value, false, hddidEnvioOPI.Value, "N", sUsarProdutosCliente, true, chaveNFe, vItensDevolver, Remessa, true);
                    string Arquivo = Server.MapPath("~/Download/" + filePath);
                    var cnpjDirectories = Directory.GetDirectories(baseDir);

                    XDocument xdoc = XDocument.Load(Arquivo);
                    string chave = "", CNPJ = "", CNPJdest = "", xNome = "", xNomedest = "", xFant = "", xLgr = "", nro = "", UF = "", CEP = "", xPais = "", fone = "", nNF = "", nSerieNF = "", Corpo = "";

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}nfeProc").FirstOrDefault() != null)
                        Corpo = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}nfeProc").FirstOrDefault().ToString();
                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}NFe").FirstOrDefault() != null)
                        Corpo = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}NFe").FirstOrDefault().ToString();

                    var infNFeElement = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}infNFe").FirstOrDefault();
                    if (infNFeElement != null)
                        chave = infNFeElement.Attribute("Id")?.Value?.Substring(3);

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}serie").FirstOrDefault() != null)
                        nSerieNF = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}serie").FirstOrDefault()?.Value;

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}nNF").FirstOrDefault() != null)
                        nNF = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}nNF").FirstOrDefault()?.Value;

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                        CNPJ = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                        CNPJdest = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                        xNomedest = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                        xNome = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xFant").FirstOrDefault() != null)
                        xFant = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xFant").FirstOrDefault()?.Value;

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault() != null)
                        xLgr = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault()?.Value;

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault() != null)
                        nro = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault()?.Value;

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault() != null)
                        UF = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value;

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault() != null)
                        CEP = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault()?.Value;

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xPais").FirstOrDefault() != null)
                        xPais = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xPais").FirstOrDefault()?.Value;

                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}fone").FirstOrDefault() != null)
                        fone = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}fone").FirstOrDefault()?.Value;

                    //if (File.Exists(Arquivo))
                    //{
                    //    try
                    //    {
                    //        foreach (var cnpjDir in cnpjDirectories)
                    //        {
                    //            string dirName = Path.GetFileName(cnpjDir);
                    //            if (Regex.IsMatch(dirName, cnpjPattern))
                    //            {
                    //                if (dirName == CNPJ)
                    //                {
                    //                    foreach (var pasta in new string[] { Path.Combine(cnpjDir, "Piloto") })
                    //                    {
                    //                        File.Copy(Arquivo, Path.Combine(pasta, Path.GetFileName(chave + ".xml")));
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //    catch { }
                    //}

                    //GerarNFe(true, hddsCaminho_UniNFe.Value, Arquivo, "N");

                    //Arquivo objArquivo = new Arquivo();
                    //byte[] lObjArquivo = null;
                    //using (FileStream fileStream = new FileStream(Server.MapPath("~/Download/" + filePath), FileMode.Open, FileAccess.Read))
                    //{
                    //    lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(Server.MapPath("~/Download/" + filePath), fileStream);
                    //}

                    //SqlDataAdapter da = new SqlDataAdapter(sProcedure_Arquivos, StringDeConexao);

                    //DataSet tabela = new DataSet();
                    //da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
                    //da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    //SqlCommand lObjCommand = new SqlCommand();

                    //da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "INCLUIR";
                    //da.SelectCommand.Parameters.Add("@idTipoArquivo", SqlDbType.Int).Value = 10004;
                    //da.SelectCommand.Parameters.Add("@idObjeto", SqlDbType.Int).Value = 0;
                    //da.SelectCommand.Parameters.Add("@sNomeArquivo", SqlDbType.VarChar).Value = filePath;
                    //da.SelectCommand.Parameters.Add("@sDscArquivo", SqlDbType.VarChar).Value = "";
                    //da.SelectCommand.Parameters.Add("@sObservacao", SqlDbType.VarChar).Value = "";
                    //da.SelectCommand.Parameters.Add("@vbArquivo", SqlDbType.VarBinary).Value = lObjArquivo;
                    //da.SelectCommand.Parameters.Add("@idUsuario", SqlDbType.Int).Value = Variaveis.idUsuario();
                    //da.SelectCommand.Parameters.Add("@dtExpiracaoDoc", SqlDbType.VarChar).Value = "";
                    //da.SelectCommand.Parameters.Add("@dtRegistroDoc", SqlDbType.VarChar).Value = "";

                    //try
                    //{
                    //    da.Fill(tabela);
                    //    hddidArquivo.Value = DATASET(tabela, 0, "idArquivo");
                    //}
                    //catch (Exception ex)
                    //{
                    //    throw new Exception(string.Format("Erro BD-DS: {0}", ex.Message));
                    //}

                    Dictionary<string, string> vParametrosGerenciador = new Dictionary<string, string>
                    {
                        { "@sfuncao", "INCLUIR_XML" },
                        { "@idTipoObjeto", "14" },
                        { "@idObjeto", hddidEnvioOPI.Value },
                        { "@sChaveNFe", chave },
                        { "@sCNPJ_Emitente", CNPJ },
                        { "@nSerieNF", nSerieNF },
                        { "@nNumeroNF", nNF },
                        { "@sXML", Corpo.ToString() },
                        { "@sNome_Emitente", xNome },
                        { "@sNomeFantasia_Emitente", xFant },
                        { "@sLogradouro_Emitente", xLgr },
                        { "@sNumero_Emitente", nro },
                        { "@sUF_Emitente", UF },
                        { "@sCEP_Emitente", CEP },
                        { "@sPais_Emitente", xPais },
                        { "@sTelefone_Emitente", fone },
                        { "@sNome_Destinatario", xNomedest },
                        { "@sTipoXML", "2" },
                        { "@sEspelho", "S" },
                        { "@idOrigem", "4" },
                        { "@idArquivo", hddidArquivo.Value },
                        { "@chNFe_Devolucao", hddsChave.Value }
                    };
                    DataSet dsPesquisaGerenciador = ExecutarDataSet(sProcedure_XML, vParametrosGerenciador);

                    Dictionary<string, string> vALTERAR_DEVOLUCAO = new Dictionary<string, string>
                    {
                        { "@sFuncao", "ALTERAR_DEVOLUCAO" },
                        { "@sDevolucao", status },
                        { "@sItensDevolucao", vItensDevolver },
                        { "@sChaveNFe", hddsChave.Value }
                    };
                    DataSet dsALTERAR_DEVOLUCAO = ExecutarDataSet(sProcedure_XML, vALTERAR_DEVOLUCAO);

                    Pesquisar(hddidOPI.Value, "0");
                    MensagemPagina.MostraMensagem_Sucesso("Devolução efetuado com sucesso!", false);
                }
                else ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DevolucaoOpen", "$('#Modal_Receber').modal('show');", true);
            }
            catch (Exception ex)
            {
                MensagemPagina_Modal_Receber.MostraMensagem_Erro(ex.Message, false);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DevolucaoOpen", "$('#Modal_Receber').modal('show');", true);
            }
        }

        protected void gvItensDevolucao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TextBox txtQuantidade = (TextBox)e.Row.FindControl("txtnQuantidadeEnvio");

                if (txtQuantidade != null)
                {
                    if (decimal.TryParse(e.Row.Cells[5].Text.Trim(), NumberStyles.Number, new CultureInfo("pt-BR"), out decimal quantidadeDecimal))
                        txtQuantidade.Attributes["max"] = Convert.ToInt32(quantidadeDecimal).ToString();
                    else txtQuantidade.Attributes["max"] = "0";
                }
            }

            EsconderColunas(e, 0, 1, 7, 10);
        }

        protected void gvDocumentosFiscais_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton DocumentosFiscais_cmdGerarNFe = e.Row.FindControl("DocumentosFiscais_cmdGerarNFe") as LinkButton;
                LinkButton DocumentosFiscais_cmdDownloadDANFE = e.Row.FindControl("DocumentosFiscais_cmdDownloadDANFE") as LinkButton;
                LinkButton DocumentosFiscais_cmdDownloadXML = e.Row.FindControl("DocumentosFiscais_cmdDownloadXML") as LinkButton;
                LinkButton DocumentosFiscais_cmdEnviarEmail = e.Row.FindControl("DocumentosFiscais_cmdEnviarEmail") as LinkButton;
                LinkButton DocumentosFiscais_cmdEditarXML = e.Row.FindControl("DocumentosFiscais_cmdEditarXML") as LinkButton;
                LinkButton DocumentosFiscais_cmdCartaCorrecao = e.Row.FindControl("DocumentosFiscais_cmdCartaCorrecao") as LinkButton;
                LinkButton DocumentosFiscais_cmdDownloadCCePDF = e.Row.FindControl("DocumentosFiscais_cmdDownloadCCePDF") as LinkButton;
                LinkButton DocumentosFiscais_cmdDownloadCCeXML = e.Row.FindControl("DocumentosFiscais_cmdDownloadCCeXML") as LinkButton;
                LinkButton DocumentosFiscais_cmdCancelarNFE = e.Row.FindControl("DocumentosFiscais_cmdCancelarNFE") as LinkButton;
                LinkButton DocumentosFiscais_cmdDevolucao = e.Row.FindControl("DocumentosFiscais_cmdDevolucao") as LinkButton;
                LinkButton NFS_btnImportarRetorno = e.Row.FindControl("NFS_btnImportarRetorno") as LinkButton;
                LinkButton NFS_btnGerarPDFEspelho = e.Row.FindControl("NFS_btnGerarPDFEspelho") as LinkButton;
                LinkButton NFS_btnGerarTXT = e.Row.FindControl("NFS_btnGerarTXT") as LinkButton;

                DocumentosFiscais_cmdGerarNFe.Visible = false;
                DocumentosFiscais_cmdDownloadDANFE.Visible = false;
                DocumentosFiscais_cmdDownloadXML.Visible = false;
                DocumentosFiscais_cmdEnviarEmail.Visible = false;
                DocumentosFiscais_cmdEditarXML.Visible = false;
                DocumentosFiscais_cmdCartaCorrecao.Visible = false;
                DocumentosFiscais_cmdDownloadCCePDF.Visible = false;
                DocumentosFiscais_cmdDownloadCCeXML.Visible = false;
                DocumentosFiscais_cmdCancelarNFE.Visible = false;
                DocumentosFiscais_cmdDevolucao.Visible = false;
                NFS_btnImportarRetorno.Visible = false;
                NFS_btnGerarPDFEspelho.Visible = false;
                NFS_btnGerarTXT.Visible = false;

                if (e.Row.Cells[nColuna_sTipo].Text == "NFS-e")
                {
                    NFS_btnGerarPDFEspelho.CommandArgument = DataBinder.Eval(e.Row.DataItem, "idArquivoPDF").ToString();
                    NFS_btnGerarPDFEspelho.CommandName = e.Row.Cells[nColuna_Status].Text == "Autorizada" ? "S" : "N";
                    NFS_btnGerarTXT.CommandArgument = $"{e.Row.Cells[nColuna_idXML].Text}|{DataBinder.Eval(e.Row.DataItem, e.Row.Cells[nColuna_Status].Text == "Autorizada" ? "idArquivo_Retorno" : "idArquivo")}";
                }

                switch (e.Row.Cells[nColuna_Status].Text)
                {
                    case "Espelho":
                        DocumentosFiscais_cmdCancelarNFE.Text = "Excluir";
                        DocumentosFiscais_cmdCancelarNFE.Visible = true;
                        if (e.Row.Cells[nColuna_sTipo].Text == "NFS-e")
                        {
                            DocumentosFiscais_cmdCancelarNFE.Text = "Cancelar";
                            NFS_btnGerarPDFEspelho.Visible = true;
                            NFS_btnGerarTXT.Visible = true;
                        }
                        else
                        {
                            DocumentosFiscais_cmdGerarNFe.Visible = true;
                            DocumentosFiscais_cmdDownloadXML.Visible = true;
                            DocumentosFiscais_cmdDownloadDANFE.Visible = true;
                            DocumentosFiscais_cmdDownloadDANFE.Text = "Espelho";
                        }
                        break;

                    case "Autorizada":
                        if (e.Row.Cells[nColuna_sTipo].Text == "NF-e")
                        {
                            DocumentosFiscais_cmdDownloadXML.Visible = true;
                            DocumentosFiscais_cmdDownloadDANFE.Visible = true;
                            if (e.Row.Cells[nColuna_Cancelamento].Text == "N")
                            {
                                DocumentosFiscais_cmdEnviarEmail.Visible = true;
                                DocumentosFiscais_cmdCartaCorrecao.Visible = true;
                                DocumentosFiscais_cmdDevolucao.Visible = true;
                                DocumentosFiscais_cmdCancelarNFE.Visible = true;
                            }
                        }
                        else if (e.Row.Cells[nColuna_sTipo].Text == "CC-e")
                        {
                            DocumentosFiscais_cmdDownloadCCePDF.Visible = true;
                            DocumentosFiscais_cmdDownloadCCeXML.Visible = true;
                        }
                        else if (e.Row.Cells[nColuna_sTipo].Text == "NFD-e")
                        {
                            DocumentosFiscais_cmdDownloadXML.Visible = true;
                            DocumentosFiscais_cmdDownloadDANFE.Visible = true;
                            if (e.Row.Cells[nColuna_Cancelamento].Text == "N")
                            {
                                DocumentosFiscais_cmdCancelarNFE.Visible = true;
                                DocumentosFiscais_cmdCartaCorrecao.Visible = true;
                            }
                        }
                        else if (e.Row.Cells[nColuna_sTipo].Text == "NFS-e")
                        {
                            NFS_btnGerarPDFEspelho.Text = "PDF";
                            NFS_btnGerarPDFEspelho.Visible = true;
                            NFS_btnGerarTXT.Visible = true;
                            DocumentosFiscais_cmdCancelarNFE.Visible = true;
                        }
                        break;

                    case "Em processamento":
                        if (e.Row.Cells[nColuna_sTipo].Text == "NF-e") DocumentosFiscais_cmdDownloadXML.Visible = true;
                        else if (e.Row.Cells[nColuna_sTipo].Text == "NFS-e")
                        {
                            NFS_btnImportarRetorno.Visible = true;
                            NFS_btnGerarPDFEspelho.Visible = true;
                            NFS_btnGerarTXT.Visible = true;
                            DocumentosFiscais_cmdCancelarNFE.Visible = true;
                        }
                        break;

                    case "Rejeitada":
                        DocumentosFiscais_cmdCancelarNFE.Visible = true;
                        DocumentosFiscais_cmdEditarXML.Visible = true;
                        break;

                    case "Aguardando DANFE":
                        DocumentosFiscais_cmdCancelarNFE.Visible = true;
                        DocumentosFiscais_cmdDownloadXML.Visible = true;
                        break;

                    case "Cancelada":
                        if (e.Row.Cells[nColuna_sTipo].Text == "NFS-e") NFS_btnGerarTXT.Visible = true;
                        else
                        {
                            DocumentosFiscais_cmdDownloadXML.Visible = true;
                            DocumentosFiscais_cmdDownloadDANFE.Visible = true;
                        }
                        break;
                }
            }

            EsconderColunas(e, nColuna_idEnvioOPI, nColuna_nNumeroCartaCorrecao, nColuna_Cancelamento, nColuna_sChaveNFe, nColuna_idFaturamento);
        }

        protected void chkExibirProdutosCliente_CheckedChanged(object sender, EventArgs e) { PopulaImpostos(); Scripts.Mantem_AbaAtiva(Page, "Informacoes-tab"); }

        #endregion

        #region | Aba Contas Receber

        void ContasReceber()
        {
            aba_ContasReceber.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string> { { "@sFuncao", "FATURAMENTO" } };

            if (int.TryParse(hddidOPI.Value, out int idOPI) && idOPI > 0)
                vParametros.Add("@idOPI", hddidOPI.Value);
            else vParametros.Add("@idPedido", hddidFaturamento.Value);

            DataSet ds = ExecutarDataSet(sProcedure_Contas_Receber, vParametros);

            if (ds.Tables[1].Rows.Count > 0)
            {
                aba_ContasReceber.Visible = true;

                gv_ContasReceber.DataSource = ds.Tables[1];
                gv_ContasReceber.DataBind();

                SomarColunas(gv_ContasReceber, false, Formatação.Moeda, typeof(HyperLink), 6, 7);
            }
        }

        #endregion

        #region | Script 

        void RegistraScript(string sFuncao)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("function Calcula_ValorLiquido(nLinha) {");
            sb.AppendLine($"    const IR = parseFloat($('#{hddFator_IR.ClientID}').val().replace(/\\./g, '').replace(',', '.'));");
            sb.AppendLine($"    const ISS = parseFloat($('#{hddFator_ISS.ClientID}').val().replace(/\\./g, '').replace(',', '.'));");
            sb.AppendLine($"    const INSS = parseFloat($('#{hddFator_INSS.ClientID}').val().replace(/\\./g, '').replace(',', '.'));");
            sb.AppendLine($"    const CSLL = parseFloat($('#{hddFator_CSLL.ClientID}').val().replace(/\\./g, '').replace(',', '.'));");
            sb.AppendLine($"    const PIS = parseFloat($('#{hddFator_PIS.ClientID}').val().replace(/\\./g, '').replace(',', '.'));");
            sb.AppendLine($"    const COFINS = parseFloat($('#{hddFator_COFINS.ClientID}').val().replace(/\\./g, '').replace(',', '.'));");
            sb.AppendLine("     const bIR = $('#cphCorpo_SwitchAtivo_Retencao_IR_hddSwitch').val() == 'S';");
            sb.AppendLine("     const bISS = $('#cphCorpo_SwitchAtivo_Retencao_ISS_hddSwitch').val() == 'S';");
            sb.AppendLine("     const bINSS = $('#cphCorpo_SwitchAtivo_Retencao_INSS_hddSwitch').val() == 'S';");
            sb.AppendLine("     const bCSLL = $('#cphCorpo_SwitchAtivo_Retencao_CSLL_hddSwitch').val() == 'S';");
            sb.AppendLine("     const bPIS = $('#cphCorpo_SwitchAtivo_Retencao_PIS_hddSwitch').val() == 'S';");
            sb.AppendLine("     const bCOFINS = $('#cphCorpo_SwitchAtivo_Retencao_COFINS_hddSwitch').val() == 'S';");
            sb.AppendLine("     $('.td_IR').removeClass('danger').addClass(bIR ? 'success' : 'danger');");
            sb.AppendLine("     $('.td_ISS').removeClass('danger').addClass(bISS ? 'success' : 'danger');");
            sb.AppendLine("     $('.td_INSS').removeClass('danger').addClass(bINSS ? 'success' : 'danger');");
            sb.AppendLine("     $('.td_CSLL').removeClass('danger').addClass(bCSLL ? 'success' : 'danger');");
            sb.AppendLine("     $('.td_PIS').removeClass('danger').addClass(bPIS ? 'success' : 'danger');");
            sb.AppendLine("     $('.td_COFINS').removeClass('success danger').addClass(bCOFINS ? 'success' : 'danger');");
            sb.AppendLine("");
            sb.AppendLine("     if (nLinha && nLinha >= 0) {");
            sb.AppendLine($"        nLinha = parseInt(nLinha) + 1;");
            sb.AppendLine($"        const row = $($('#{dtgLancamento.ClientID} tr')[nLinha]);");
            sb.AppendLine("         let bruto = parseFloat(row.find('.valorBruto_Lancamento').val().replace(/\\./g, '').replace(',', '.'));");
            sb.AppendLine("         let impostosRetidos = 0;");
            sb.AppendLine("         let liquido = 0;");
            sb.AppendLine("         if (bruto && bruto > 0) {");
            sb.AppendLine("             if (bIR) impostosRetidos += bruto - (bruto * IR);");
            sb.AppendLine("             if (bISS) impostosRetidos += bruto - (bruto * ISS);");
            sb.AppendLine("             if (bINSS) impostosRetidos += bruto - (bruto * INSS);");
            sb.AppendLine("             if (bCSLL) impostosRetidos += bruto - (bruto * CSLL);");
            sb.AppendLine("             if (bPIS) impostosRetidos += bruto - (bruto * PIS);");
            sb.AppendLine("             if (bCOFINS) impostosRetidos += bruto - (bruto * COFINS);");
            sb.AppendLine("             liquido = bruto - impostosRetidos;");
            sb.AppendLine("         }");
            sb.AppendLine("         const sLiquido = FormatarValor(liquido, 2);");
            sb.AppendLine("         row.find('.valorLiquido_Lancamento').val(sLiquido);");
            sb.AppendLine("         row.find('[id*=hddValorLiquido]').val(sLiquido);");
            sb.AppendLine("         let brutoTotal = 0;");
            sb.AppendLine("         let liquidoTotal = 0;");
            sb.AppendLine($"        $('#{dtgLancamento.ClientID} tr').each(function() {{");
            sb.AppendLine("             const linha = $(this);");
            sb.AppendLine("             if (linha.find('.valorBruto_Lancamento').length > 0) {");
            sb.AppendLine("                 brutoTotal += parseFloat(linha.find('.valorBruto_Lancamento').val().replace(/\\./g, '').replace(',', '.'));");
            sb.AppendLine("                 liquidoTotal += parseFloat(linha.find('[id*=hddValorLiquido]').val().replace(/\\./g, '').replace(',', '.'));");
            sb.AppendLine("             }");
            sb.AppendLine("         });");
            sb.AppendLine($"        $('#{dtgLancamento.ClientID} tr.linhaTotal td.brutoTotal').text('R$ ' + FormatarValor(brutoTotal, 2));");
            sb.AppendLine($"        $('#{dtgLancamento.ClientID} tr.linhaTotal td.liquidoTotal').text('R$ ' + FormatarValor(liquidoTotal, 2));");
            sb.AppendLine("     } else {");
            sb.AppendLine("         let brutoTotal = 0;");
            sb.AppendLine("         let liquidoTotal = 0;");
            sb.AppendLine($"        $('#{dtgLancamento.ClientID} tr').each(function() {{");
            sb.AppendLine("             const row = $(this);");
            sb.AppendLine("             if (row.find('.valorBruto_Lancamento').length > 0) {");
            sb.AppendLine("                 let bruto = parseFloat(row.find('.valorBruto_Lancamento').val().replace(/\\./g, '').replace(',', '.'));");
            sb.AppendLine("                 let impostosRetidos = 0;");
            sb.AppendLine("                 let liquido = 0;");
            sb.AppendLine("                 if (bruto && bruto > 0) {");
            sb.AppendLine("                     if (bIR) impostosRetidos += bruto - (bruto * IR);");
            sb.AppendLine("                     if (bISS) impostosRetidos += bruto - (bruto * ISS);");
            sb.AppendLine("                     if (bINSS) impostosRetidos += bruto - (bruto * INSS);");
            sb.AppendLine("                     if (bCSLL) impostosRetidos += bruto - (bruto * CSLL);");
            sb.AppendLine("                     if (bPIS) impostosRetidos += bruto - (bruto * PIS);");
            sb.AppendLine("                     if (bCOFINS) impostosRetidos += bruto - (bruto * COFINS);");
            sb.AppendLine("                     liquido = bruto - impostosRetidos;");
            sb.AppendLine("                 }");
            sb.AppendLine("                 const sLiquido = FormatarValor(liquido, 2);");
            sb.AppendLine("                 row.find('.valorLiquido_Lancamento').val(sLiquido);");
            sb.AppendLine("                 row.find('[id*=hddValorLiquido]').val(sLiquido);");
            sb.AppendLine("                 brutoTotal += bruto;");
            sb.AppendLine("                 liquidoTotal += liquido;");
            sb.AppendLine("             }");
            sb.AppendLine("         });");
            sb.AppendLine($"        $('#{dtgLancamento.ClientID} tr.linhaTotal td.brutoTotal').text('R$ ' + FormatarValor(brutoTotal, 2));");
            sb.AppendLine($"        $('#{dtgLancamento.ClientID} tr.linhaTotal td.liquidoTotal').text('R$ ' + FormatarValor(liquidoTotal, 2));");
            sb.AppendLine("     }");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("");
            sb.AppendLine("     var $uploadContainer = $('[id*=uploadContainer]');");
            sb.AppendLine("     var $fileInput = $('[id*=fu_ImportarRetorno_NFS]');");
            sb.AppendLine("     var $fileNameDisplay = $('[id*=fileName]');");
            sb.AppendLine("");
            sb.AppendLine("     $uploadContainer.on('click', function () {");
            sb.AppendLine("         $fileInput.click();");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $fileInput.on('click', function (e) {");
            sb.AppendLine("         e.stopPropagation();");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $uploadContainer.on('dragover', function (e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         e.stopPropagation();");
            sb.AppendLine("         $uploadContainer.addClass('dragover');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $uploadContainer.on('dragleave', function (e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         e.stopPropagation();");
            sb.AppendLine("         $uploadContainer.removeClass('dragover');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $uploadContainer.on('drop', function (e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         e.stopPropagation();");
            sb.AppendLine("         $uploadContainer.removeClass('dragover');");
            sb.AppendLine("         var files = e.originalEvent.dataTransfer.files;");
            sb.AppendLine("         $fileInput[0].files = files;");
            sb.AppendLine("         displayFileName(files[0].name);");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $fileInput.on('change', function () {");
            sb.AppendLine("         if (this.files.length > 0) {");
            sb.AppendLine("             displayFileName(this.files[0].name);");
            sb.AppendLine("         }");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     function displayFileName(name) {");
            sb.AppendLine("         $fileNameDisplay.text(name);");
            sb.AppendLine("     }");
            sb.AppendLine("");
            sb.AppendLine("     var calcula = false;");
            sb.AppendLine("     $('.valorBruto_Lancamento').on('input', function () {");
            sb.AppendLine("         calcula = $(this).val() ? true : false;");
            sb.AppendLine("     });");
            sb.AppendLine("     $('.valorBruto_Lancamento').on('blur', function () {");
            sb.AppendLine($"        const idPedido = $('#{hddidPedido.ClientID}').val() ? parseInt($('#{hddidPedido.ClientID}').val()) : 0;");
            sb.AppendLine("         if (calcula && idPedido > 0) {");
            sb.AppendLine($"            Calcula_ValorLiquido($(this).closest('tr').attr('data-nLinha'));");
            sb.AppendLine("             calcula = false;");
            sb.AppendLine("         }");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('[id*=cmdAplicaTodos_]').on('click', function (e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         const valor = $(this).siblings('.aplicaTodos').val();");
            sb.AppendLine("         const campo = $(this).siblings('.aplicaTodos').data('aplicatodos');");
            sb.AppendLine("         try {");
            sb.AppendLine("             $(`[data-aplicatodos=${campo}]`).val(valor).trigger('chosen:updated');");
            sb.AppendLine("         } catch {}");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('input[type=\"radio\"]').click(function () {");
            sb.AppendLine("         $('input[type=\"radio\"]').prop('checked', false);");
            sb.AppendLine("         $(this).prop('checked', true);");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('.Todos').change(function(){ var isChecked = $(this).find('input').is(':checked'); $('.Individual').find('input').prop('checked', isChecked); $('.ArquivoIndividual').find('input').prop('checked', isChecked);});");
            sb.AppendLine("     $('.Individual').change(function(){ var isChecked = $(this).find('input').is(':checked'); var arquivo = $(this).closest('tr').find('.composicaoLinha').data('div-id'); $('[id*='+arquivo+']').find('.ArquivoIndividual').find('input').prop('checked', isChecked); });");
            sb.AppendLine("");
            sb.AppendLine("     $('.composicaoLinha').addClass('fa fa-plus');");
            sb.AppendLine("     $('.composicaoLinha').click(function() {");
            sb.AppendLine("          var icon = $(this);");
            sb.AppendLine("          var divId = $(this).data('div-id');");
            sb.AppendLine("          var current = $('#' + divId).css('display');");
            sb.AppendLine("          if (current == 'none') {");
            sb.AppendLine("              $('#' + divId).show('slow');");
            sb.AppendLine("              icon.removeClass('fa fa-plus').addClass('fa fa-minus');");
            sb.AppendLine("          } else {");
            sb.AppendLine("              $('#' + divId).hide('slow');");
            sb.AppendLine("              icon.removeClass('fa fa-minus').addClass('fa fa-plus');");
            sb.AppendLine("          }");
            sb.AppendLine("          return false;");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('idCSTICMS').change(function () {");
            sb.AppendLine("         var ddl = $(this);");
            sb.AppendLine("         var selectedValue = ddl.val();");
            sb.AppendLine("         var txtReducao = ddl.closest('tr').find('.pReducao');");
            sb.AppendLine("         if (ddl.closest('tr').find('.sFaturamento').text() === 'S') {");
            sb.AppendLine("             txtReducao.prop('readonly', true);");
            sb.AppendLine("         } else {");
            sb.AppendLine("             if (selectedValue !== '3') {");
            sb.AppendLine("                 txtReducao.prop('readonly', true);");
            sb.AppendLine("                 txtReducao.val('0,0000');");
            sb.AppendLine("             } else {");
            sb.AppendLine("                 txtReducao.prop('readonly', false);");
            sb.AppendLine("             }");
            sb.AppendLine("         }");
            sb.AppendLine("     });");
            sb.AppendLine("     $('idCSTICMS').trigger('change');");
            sb.AppendLine("");
            sb.AppendLine("     $('[id*=nValorLancamentoRec], [id*=nValorBrutoLancamentoRec]').mask('000.000.000,00', { reverse: true });");
            sb.AppendLine("     $('[id*=txtpReducao]').mask('000,0000', { reverse: true });");
            sb.AppendLine("");
            sb.AppendLine($"    $('#{txtnISS.ClientID}').on('blur', function (e) {{");
            sb.AppendLine("         let valor = 0;");
            sb.AppendLine("         try {");
            sb.AppendLine("             valor = parseFloat($(this).val().replace(/\\./g, '').replace(',', '.'));");
            sb.AppendLine("         } catch { valor = 0; }");
            sb.AppendLine("         const sValor = FormatarValor(valor, 2);");
            sb.AppendLine("         $(this).val(sValor);");
            sb.AppendLine($"        $('#{txtImposto_ISS.ClientID}').text(sValor + ' %');");
            sb.AppendLine($"        $('#{hddFator_ISS.ClientID}').val(FormatarValor(((valor - 100) * (-1)) / 100, 4));");
            sb.AppendLine("         Calcula_ValorLiquido();");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }

        #endregion
    }
}