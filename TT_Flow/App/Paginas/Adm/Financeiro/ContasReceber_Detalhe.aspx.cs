using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using TT_Flow.FrameWork;
using TT.FrameWork;
using static TT.FrameWork.BD;
using TT_Hub.App.Paginas.Adm.Manutencao;
using System.Text;
using System.IO;
using TT_Hub.App.Paginas.Adm.Financeiro;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using static Permissao.Financeiro;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using MathNet.Numerics;
using Control = System.Web.UI.Control;
using Org.BouncyCastle.Utilities;

namespace TT_Flow.App.Paginas.Adm.Financeiro
{
    public partial class ContasReceber_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Conta a Receber";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_Contas_Receber";

        private enum EnumTiposPagamentos
        {
            Compensacao = 1,
            Pix = 2,
            BoletoBancario = 3,
            CartaoDeCredito = 4,
            Transferencia = 5,
            MercadoPago = 6,
            CreditoEmConta = 7,
            Outros = 8
        }

        #region | Classes
        public List<FrameWork.cls_Recebimento> bs_Recebimento
        {

            get
            {
                if (ViewState["bs_Recebimento"] == null)
                {
                    ViewState["bs_Recebimento"] = new List<FrameWork.cls_Recebimento>();
                }
                return (List<cls_Recebimento>)ViewState["bs_Recebimento"];
            }

            set
            {
                ViewState["bs_Recebimento"] = value;
            }



        }

        public List<FrameWork.cls_LancamentoRec> bs_LancamentoRec
        {

            get
            {
                if (ViewState["bs_LancamentoRec"] == null)
                {
                    ViewState["bs_LancamentoRec"] = new List<FrameWork.cls_LancamentoRec>();
                }
                return (List<cls_LancamentoRec>)ViewState["bs_LancamentoRec"];
            }

            set
            {
                ViewState["bs_LancamentoRec"] = value;
            }

        }
        #endregion

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "ManualdoUsuarioContasReceber.pdf";
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            txtidContasReceber.ReadOnly = true;
            ddlidContabil.Attributes.Add("disabled", "disabled");
            txtdtVencimentoOriginal.ReadOnly = true;

            if (!IsPostBack)
            {

                PopularCombos();
                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasReceber.Consultar, true);
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasReceber.Incluir, true);
                    Pesquisar("0", true);

                }
                ddlidMeioRecebimento_SelectedIndexChanged(objSender, objEventArgs);
                ddlidFormaRecebimento_SelectedIndexChanged(objSender, objEventArgs);
                txtnValorRecebimento_Info_Rec_TextChanged(objSender, objEventArgs);
                ddlidCategoriaReceber_TextChanged(objSender, objEventArgs);
                Atualizar_ContaBancaria();

                if (Session["SalvoComSucesso"] != null && (bool)Session["SalvoComSucesso"])
                {
                    MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                    Session["SalvoComSucesso"] = false;
                }
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                var requestArgs = this.Request["__EVENTARGUMENT"];

                if (requestTarget == "funcao_SAIR")
                {
                    FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                }
                else if (requestTarget == "funcao_SALVAR")
                {
                    Salvar_ContasReceber();
                }
                else if (requestTarget == "funcao_Editar")
                {
                    Pesquisar(hddidContasReceber.Value, true);
                }
                else if (requestTarget == "funcao_EXCLUIR")
                {
                    BtnExcluirParcela_Click(hddidContasReceber.Value, EventArgs.Empty);
                }
                else if (requestTarget == "funcao_Reabrir")
                {
                    BtnReabrirTitulo_Click(hddidContasReceber.Value, EventArgs.Empty);
                }
            }

            RegistraScript("");
        }
        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar(string idContasReceber, bool bEdicao)
        {
            if (!FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasReceber.Excluir))
            {
                BtnExcluirParcela.Visible = false;
            }

            Div_Desconto.Visible = false;

            if (txtnSaldo.Text == "0")
            {
                Div_CadastroRecebimento.Visible = false;
            }

            aba_Historico.Visible = false;
            Aba_Log.Visible = false;
            aba_Arquivo.Visible = false;
            Div_dtVencimento.Visible = false;
            Div_nValorOriginal.Visible = true;
            Div_nParcelas.Visible = false;
            txtnValorTotalRec.ReadOnly = true;
            Div_VencimentoOriginal.Visible = false;
            Div_dtVencimento.Visible = false;
            txtnTotal.ReadOnly = true;
            Div_Saldo.Visible = false;
            DivTotal.Visible = false;

            BtnAdicionarObservacao.Visible = false;
            string sErro = "";

            try
            {
                LimpaCampos();

                if (idContasReceber != "0")
                {

                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idContasReceber", idContasReceber);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    Div_nParcelas.Visible = false;
                    cmdFaturamento.Visible = false;
                    BtnAdicionarObservacao.Visible = true;
                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        StatusDosCampos(true);
                        if (RETORNO.DATASET(dsPesquisa, 0, "idEnvioOPI") != "0")
                        {
                            btnAlterarVencimento.Visible = true;
                            BtnExcluirParcela.Visible = false;
                            cmdFaturamento.Visible = true;
                            hddidOPI.Value = RETORNO.DATASET(dsPesquisa, 0, "idOPI");
                        }
                        
                        string sDscTipoStatus = RETORNO.DATASET(dsPesquisa, 0, "sStatus");
                        lblsDscTipoStatus.Text = sDscTipoStatus;
                        lblsDscTipoStatus.CssClass = string.Format("label label-{0}", RETORNO.DATASET(dsPesquisa, 0, "sCor"));

                        string sDscStatusAdiantado = RETORNO.DATASET(dsPesquisa, 0, "sStatusAdiantado");
                        lblsStatusAdiantado.Text = sDscStatusAdiantado;
                        lblsStatusAdiantado.CssClass = string.Format("label label-{0}", RETORNO.DATASET(dsPesquisa, 0, "sCor"));

                        hddidContasReceber.Value = RETORNO.DATASET(dsPesquisa, 0, "idContasReceber");
                        txtidContasReceber.Text = RETORNO.DATASET(dsPesquisa, 0, "idContasReceber");
                        txtsCodigo.Text = RETORNO.DATASET(dsPesquisa, 0, "sCodigo");

                        var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtEmissao").ToString());
                        txtdtEmissao.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');

                        var dt2 = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtVencimento").ToString());
                        txtdtVencimento.Text = dt2.ToString(@"yyyy/MM/dd").Replace('/', '-');

                        var dt3 = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtVencimentoOriginal").ToString());
                        txtdtVencimentoOriginal.Text = dt3.ToString(@"yyyy/MM/dd").Replace('/', '-');

                        txtsDocumento.Text = RETORNO.DATASET(dsPesquisa, 0, "sDocumento");

                        ddlidContabil.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idContabil");
                        ddlidEmpresa.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idEmpresa");
                        hddidEmpresa.Value = ddlidEmpresa.SelectedValue;

                        txtnTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "nTotal");
                        txtnValorOriginal.Text = RETORNO.DATASET(dsPesquisa, 0, "nValorOriginal");
                        txtnValorBruto.Text = RETORNO.DATASET(dsPesquisa, 0, "nValorBruto");
                        txtnSaldo.Text = RETORNO.DATASET(dsPesquisa, 0, "nSaldo");
                        txtsObservacaoGeral.Text = RETORNO.DATASET(dsPesquisa, 0, "sObservacaoGeral");
                        ddlidFormaRecebimento.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idFormaRecebimento");
                        ddlidCentroDeCusto.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCentroDeCusto");

                        string idContaBancaria = RETORNO.DATASET(dsPesquisa, 0, "idConta");

                        if (!string.IsNullOrEmpty(idContaBancaria) &&
                            ddlContaBancaria.Items.FindByValue(idContaBancaria) != null)
                        {
                            ddlContaBancaria.SelectedValue = idContaBancaria;
                        }
                        else
                        {
                            ddlContaBancaria.SelectedValue = "0";
                        }

                        hddidParceiro.Value = RETORNO.DATASET(dsPesquisa, 0, "idParceiro");
                        Pesquisa_Parceiros.idParceiro = Convert.ToInt32(RETORNO.DATASET(dsPesquisa, 0, "idParceiro"));

                        txtsQuantidadeParcela.Text = RETORNO.DATASET(dsPesquisa, 0, "sQuantidadeParcela");
                        txtnParcelas.Text = RETORNO.DATASET(dsPesquisa, 0, "nParcelas");
                        ddlidMeioRecebimento.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idMeioRecebimento");

                        ddlidCategoriaReceber.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCategoriaReceber");

                        hddsTituloAdiantado.Value = RETORNO.DATASET(dsPesquisa, 0, "sTituloAdiantado");                        

                        if (RETORNO.DATASET(dsPesquisa, 0, "sAlteraStatus") == "S")
                        {
                            div_modalAlteraStatus.Visible = true;
                        }

                        //Popular_Aba_Historico(dsPesquisa);
                        Popular_dtgRecebimento(dsPesquisa);

                        Popular_Aba_Arquivos(idContasReceber);
                        Popular_Aba_Log(dsPesquisa);
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));

                        lblTituloPagina.Text = string.Format("Conta a Receber - {0} - Vencimento: {1}", RETORNO.DATASET(dsPesquisa, 0, "sRazaoSocial"), RETORNO.DATASET(dsPesquisa, 0, "dtVencimento"));
                        BreadCrumb.TitulodaPagina = string.Format("Conta a Receber Detalhe - {0}", RETORNO.DATASET(dsPesquisa, 0, "idContasReceber"));

                        lblTituloSalvar.Text = "Confirma a Alteração da " + lblTituloPagina.Text + "?";
                        lblTituloExcluir.Text = "Confirma a Exclusão do registro " + lblTituloPagina.Text + "?";
                        lblTituloReabrir.Text = "Confirma a reabertura de " + lblTituloPagina.Text + "? Todos os Pagamentos serão deletados!";

                        Pesquisa_Parceiros.ConfigurarControles(false);
                        Div_VencimentoOriginal.Visible = true;
                        Div_dtVencimento.Visible = true;
                        Div_nValorOriginal.Visible = true;
                        Div_Recebimento.Visible = false;
                        Div_sQuantidadeParcela.Visible = true;
                        DIV_Lancamentos.Visible = false;
                        Div_dtVencimento.Visible = true;
                        div_linhaFormaRecebimento.Attributes["class"] = "form-group";
                        div_linhaFormaRecebimento2.Attributes["class"] = "col-lg-12";
                        div_linhaFormaRecebimento3.Attributes["class"] = "row";
                        div_linhaTipoRecebimento.Attributes["class"] = "form-group";
                        div_linhaTipoRecebimento2.Attributes["class"] = "col-lg-12";
                        div_linhaTipoRecebimento3.Attributes["class"] = "row";

                        Div_Saldo.Visible = true;
                        DivTotal.Visible = true;

                        //DateTime dataVencimento;
                        //bool conversaoSucesso = DateTime.TryParse(txtdtVencimento.Text, out dataVencimento);

                        //if (conversaoSucesso)
                        //{
                        //    if (dataVencimento < DateTime.Now)
                        //    {
                        //        Div_Multa.Visible = true;
                        //        Div_Juros.Visible = true;
                        //    }
                        //    else
                        //    {
                        //        Div_Multa.Visible = false;
                        //        Div_Juros.Visible = false;
                        //    }
                        //}

                        Div_Multa.Visible = true;
                        Div_Juros.Visible = true;
                        Div_Desconto.Visible = true;

                        SwitchAtivo_EmailPagoAtraso.Definir(RETORNO.DATASET(dsPesquisa, 0, "sEmailPagoAtraso"), "Enviar informação de atraso para o cliente?", "N");

                        bool podeAlterarVencimento = FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasReceber.AlterarVencimento);
                        bool podeEfetuarBaixa = FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasReceber.EfetuarBaixa);
                        bool tituloLiquidado = txtnSaldo.Text == "0,00";

                        ConfigurarCadastroRecebimento(false);
                        cmdSalvar.Visible = false;
                        btnAlterarVencimento.Visible = false;

                        if (!podeAlterarVencimento && !podeEfetuarBaixa)
                        {
                            ConfigurarCadastroRecebimento(false);
                        }

                        if (txtnSaldo.Text != "0,00")
                        {
                            BtnReabrirTitulo.Visible = false;
                        }

                        if (tituloLiquidado)
                        {

                            if (!FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasReceber.Reabrir))
                            {
                                BtnReabrirTitulo.Visible = false;
                            }

                            BtnAdicionarObservacao.Visible = true;
                            txtdtRecebimento_Info_Rec.ReadOnly = true;
                            ddlidCategoriaReceber.Attributes.Add("disabled", "disabled");
                            txtsObservacaoGeral.ReadOnly = true;
                            Div_Recebimento.Visible = true;
                            ConfigurarCadastroRecebimento(false);
                        }
                        else
                        {
                            Div_Recebimento.Visible = true;

                            if (bEdicao)
                            {
                                btnAlterarVencimento.Visible = false;
                                cmdSalvar.Visible = podeAlterarVencimento || podeEfetuarBaixa;
                                ConfigurarCadastroRecebimento(podeEfetuarBaixa);
                            }
                            else
                            {
                                btnAlterarVencimento.Visible = podeAlterarVencimento;
                                cmdSalvar.Visible = podeEfetuarBaixa;
                                ConfigurarCadastroRecebimento(podeEfetuarBaixa);
                            }
                        }

                        if (!podeAlterarVencimento)
                        {
                            btnAlterarVencimento.Visible = false;

                            if (!bEdicao && !podeEfetuarBaixa)
                                cmdSalvar.Visible = false;
                        }

                        //-------------------------------------------------------------
                        //Agnes Partal - 01/07/2024 
                        foreach (DataRow id in dsPesquisa.Tables[1].Rows)
                        {
                            int idConciliacao = id["idConciliacao"] == null ? 0 : Convert.ToInt32(id["idConciliacao"]);

                            if (idConciliacao != 0 && idConciliacao != -1)
                            {
                                BtnReabrirTitulo.Visible = false;

                                div_sStatusConciliado.Visible = true;
                                lblsStatusConciliado.Text = "Conciliado";
                                lblsStatusConciliado.CssClass = string.Format("label label-{0}", "info");
                            }
                        }
                        //-------------------------------------------------------------

                        if (!tituloLiquidado && podeEfetuarBaixa)
                            FUNCOES.Scripts.FocusScript(Page, txtdtRecebimento_Info_Rec.ClientID);
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }
                }
                else
                {
                    BreadCrumb.TitulodaPagina = string.Format("Nova {0}", sTituloPagina);
                    lblTituloPagina.Text = string.Format("Nova {0}", sTituloPagina);
                    txtidContasReceber.Text = "Novo";
                    lblTituloSalvar.Text = "Confirma a Inclusão da Conta a Receber?";
                    cmdSalvar.Text = "Incluir";
                    Div_Recebimento.Visible = false;
                    btnAlterarVencimento.Visible = false;
                    BtnExcluirParcela.Visible = false;
                    BtnReabrirTitulo.Visible = false;
                    BtnExcluirParcela.Visible = false;
                    DivTotal.Visible = false;
                    Div_Saldo.Visible = false;
                    Div_dtVencimento.Visible = false;
                    cmdFaturamento.Visible = false;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtdtEmissao]').focus();", true);

                }

                RegistraScript("");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }

        void ConfigurarCadastroRecebimento(bool habilitar)
        {
            Div_CadastroRecebimento.Visible = habilitar;
            Div_BotaoIncluirRec.Visible = habilitar;

            if (dtgRecebimento.Columns.Count > 0)
                dtgRecebimento.Columns[dtgRecebimento.Columns.Count - 1].Visible = habilitar;
        }

        void StatusDosCampos(bool bStatus)
        {
            txtsCodigo.ReadOnly = bStatus;
            txtdtEmissao.ReadOnly = bStatus;
            txtdtVencimento.ReadOnly = bStatus;
            txtsDocumento.ReadOnly = bStatus;
            txtnValorOriginal.ReadOnly = bStatus;
            txtnValorBruto.ReadOnly = bStatus;
            txtnSaldo.ReadOnly = bStatus;
            txtsObservacaoGeral.ReadOnly = bStatus;
            txtsQuantidadeParcela.ReadOnly = bStatus;


            string sStatusCombo = "disabled";
            if (!bStatus)
            {
                sStatusCombo = "enabled";
            }
            ddlidContabil.Attributes.Remove("disabled");
            ddlidContabil.Attributes.Add(sStatusCombo, sStatusCombo);
            ddlidEmpresa.Attributes.Remove("disabled");

            ddlidEmpresa.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlidFormaRecebimento.Attributes.Remove("disabled");
            ddlidFormaRecebimento.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlidCentroDeCusto.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlidMeioRecebimento.Attributes.Remove("disabled");
            ddlidMeioRecebimento.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlidCategoriaReceber.Attributes.Remove("disabled");
            ddlidCategoriaReceber.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlContaBancaria.Attributes.Remove("disabled");
            ddlContaBancaria.Attributes.Add(sStatusCombo, sStatusCombo);

            txtnParcelas.ReadOnly = false;
        }

        void Salvar_ContasReceber()
        {
            string sErro = "";
            string sMensagem = "";

            if (ValidarDados())
            {
                if (ValidarDados_Lancamento(ref sMensagem))
                {
                    int numeroParcelas = 0;

                    if (txtnParcelas.Text != "")
                    {
                        numeroParcelas = Convert.ToInt32(txtnParcelas.Text);
                    }

                    if (txtidContasReceber.Text != "Novo")
                    {
                        if (ValidarDadosVencimento())
                        {
                            numeroParcelas = 1;

                            try
                            {
                                string[] vidContasReceber = hddidContasReceber.Value.Split(',');
                                string idContasReceber = vidContasReceber[0].ToString();

                                DataSet dsSalvar;
                                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                                vParametros.Add("@sFuncao", "SALVAR");
                                vParametros.Add("@idContasReceber", idContasReceber);
                                vParametros.Add("@nSaldo", BD.Conversoes.Numerico(txtnSaldo));
                                vParametros.Add("@dtVencimento", txtdtVencimento.Text);
                                vParametros.Add("@sQuantidadeParcela", txtsQuantidadeParcela.Text);

                                vParametros.Add("@sCodigo", txtsCodigo.Text);
                                vParametros.Add("@dtEmissao", txtdtEmissao.Text.Replace("-", "/"));
                                vParametros.Add("@sDocumento", txtsDocumento.Text);
                                vParametros.Add("@nValorOriginal", BD.Conversoes.Numerico(txtnValorOriginal));
                                vParametros.Add("@nValorBruto", BD.Conversoes.Numerico(txtnValorBruto));

                                vParametros.Add("@idContabil", ddlidContabil.SelectedValue);
                                vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                                vParametros.Add("@idCentroDeCusto", ddlidCentroDeCusto.SelectedValue);
                                vParametros.Add("@idFormaRecebimento", ddlidFormaRecebimento.SelectedValue);
                                vParametros.Add("@idConta", ddlContaBancaria.SelectedValue);
                                vParametros.Add("@idMeioRecebimento", ddlidMeioRecebimento.SelectedValue);
                                vParametros.Add("@nParcelas", txtnParcelas.Text);
                                vParametros.Add("@idCategoriaReceber", ddlidCategoriaReceber.SelectedValue);
                                vParametros.Add("@dtVencimentoOriginal", txtdtVencimentoOriginal.Text.Replace("-", "/"));
                                vParametros.Add("@sObservacaoGeral", txtsObservacaoGeral.Text);
                                vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                                dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                                if (BD.ValidarDataSet(dsSalvar, out sErro))
                                {
                                    idContasReceber = RETORNO.DATASET(dsSalvar, 0, "idContasReceber");
                                    if (Salvar_Recebimento(idContasReceber))
                                    {
                                        Session["SalvoComSucesso"] = true;
                                        Response.Redirect(Request.RawUrl);
                                    }
                                    else
                                    {
                                        throw new Exception("Erro ao Salvar Recebimento:");
                                    }
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
                            txtdtVencimento.ReadOnly = true;
                            cmdSalvar.Visible = true;
                            RegistraScript("");
                        }
                        else
                        {
                            MensagemLancamento.MostraMensagem_Erro(sMensagem, false);
                        }
                    }
                    else
                    {
                        try
                        {
                            string idContasReceber_Primeira = "";
                            string[] vidContasReceber = hddidContasReceber.Value.Split(',');
                            string idContasReceber = vidContasReceber[0].ToString();

                            int totalPagamentos = numeroParcelas;

                            if (numeroParcelas == 0)
                            {
                                numeroParcelas = 1;
                            }

                            if (ValidarGridLancamento(numeroParcelas))
                            {
                                for (int numeroParcelaAtual = 1; numeroParcelaAtual <= numeroParcelas; numeroParcelaAtual++)
                                {
                                    DataSet dsSalvar;
                                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                                    TextBox txtdtLancamentoRec = (TextBox)dtgLancamento.Rows[numeroParcelaAtual - 1].FindControl("txtdtLancamentoRec");
                                    var dtLancamentoRec = Convert.ToDateTime(txtdtLancamentoRec.Text);

                                    TextBox txtnValorLancamentoRec = (TextBox)dtgLancamento.Rows[numeroParcelaAtual - 1].FindControl("txtnValorLancamentoRec");
                                    decimal valorParcela = Convert.ToDecimal(txtnValorLancamentoRec.Text);

                                    TextBox txtnValorBrutoLancamentoRec = (TextBox)dtgLancamento.Rows[numeroParcelaAtual - 1].FindControl("txtnValorBrutoLancamentoRec");
                                    decimal valorParcelaBruto = Convert.ToDecimal(txtnValorBrutoLancamentoRec.Text);


                                    vParametros.Add("@sFuncao", "SALVAR");
                                    vParametros.Add("@nSaldo", BD.Conversoes.Numerico(valorParcela));
                                    vParametros.Add("@nValorOriginal", BD.Conversoes.Numerico(valorParcela));
                                    vParametros.Add("@dtVencimento", dtLancamentoRec.ToString());
                                    vParametros.Add("@nValorBruto", BD.Conversoes.Numerico(valorParcelaBruto));



                                    if (totalPagamentos == 0)
                                    {
                                        totalPagamentos = 1;
                                        vParametros.Add("@sQuantidadeParcela", $"Única");
                                    }
                                    else
                                    {
                                        vParametros.Add("@sQuantidadeParcela", $"{numeroParcelaAtual} de {totalPagamentos}");
                                    }


                                    vParametros.Add("@sCodigo", txtsCodigo.Text);
                                    vParametros.Add("@dtEmissao", txtdtEmissao.Text.Replace("-", "/"));
                                    vParametros.Add("@sDocumento", txtsDocumento.Text);
                                    vParametros.Add("@idContabil", ddlidContabil.SelectedValue);
                                    vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                                    vParametros.Add("@idCentroDeCusto", ddlidCentroDeCusto.SelectedValue);
                                    vParametros.Add("@idMeioRecebimento", ddlidMeioRecebimento.SelectedValue);
                                    vParametros.Add("@nParcelas", txtnParcelas.Text);
                                    vParametros.Add("@idFormaRecebimento", ddlidFormaRecebimento.SelectedValue);
                                    vParametros.Add("@idCategoriaReceber", ddlidCategoriaReceber.SelectedValue);
                                    vParametros.Add("@dtVencimentoOriginal", dtLancamentoRec.ToString());
                                    vParametros.Add("@sObservacaoGeral", txtsObservacaoGeral.Text);
                                    vParametros.Add("@idParceiro", Pesquisa_Parceiros.idParceiro.ToString());
                                    vParametros.Add("@idConta", ddlContaBancaria.SelectedValue);

                                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                                    {
                                        if (numeroParcelaAtual == 1)
                                        {
                                            idContasReceber_Primeira = RETORNO.DATASET(dsSalvar, 0, "idContasReceber");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("BD: " + sErro.ToString());
                                    }
                                }

                                if (sErro == "")
                                {

                                    Pesquisa_Parceiros.ConfigurarControles(false);
                                    txtsCodigo.ReadOnly = true;
                                    txtdtEmissao.ReadOnly = true;
                                    txtdtVencimento.ReadOnly = true;
                                    txtsDocumento.ReadOnly = true;
                                    txtnValorOriginal.ReadOnly = true;
                                    txtnValorBruto.ReadOnly = true;
                                    txtnParcelas.ReadOnly = true;
                                    Div_dtVencimento.Visible = true;
                                    Div_nValorOriginal.Visible = true;
                                    Div_Saldo.Visible = true;
                                    txtsQuantidadeParcela.ReadOnly = true;
                                    txtsObservacaoGeral.ReadOnly = true;

                                    ddlidContabil.Attributes.Add("disabled", "disabled");
                                    ddlidEmpresa.Attributes.Add("disabled", "disabled");
                                    ddlidFormaRecebimento.Attributes.Add("disabled", "disabled");
                                    ddlidCentroDeCusto.Attributes.Add("disabled", "disabled");
                                    ddlidMeioRecebimento.Attributes.Add("disabled", "disabled");
                                    ddlidCategoriaReceber.Attributes.Add("disabled", "disabled");


                                    string urlNovo = "/app/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id=0";
                                    string urlDetalhes = string.Format("/app/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}", idContasReceber_Primeira);

                                    string mensagemSucesso = string.Format("Lançamentos registrados com sucesso! <br/>" +
                                        "<a href='{1}'>Clique aqui para incluir uma nova {0}</a> " +
                                        "ou <a href='{2}'>Clique aqui para ver os detalhes.</a>", sTituloPagina, urlNovo, urlDetalhes);

                                    MensagemPagina.MostraMensagem_Sucesso(mensagemSucesso);

                                    DIV_Lancamentos.Visible = false;
                                    cmdSalvar.Visible = false;
                                }
                            }
                            else
                            {
                                MensagemPagina_Recebimento.MostraMensagem_Erro("Soma dos valores das Parcelas deve ser igual ao valor do título");
                            }
                        }
                        catch (Exception ex)
                        {
                            MensagemPagina.MostraMensagem_Erro(ex.Message);
                        }
                        txtdtVencimento.ReadOnly = true;
                        RegistraScript("");

                    }

                }
                else
                {
                    MensagemLancamento.MostraMensagem_Erro(sMensagem, false);
                }

            }

        }

        private bool ValidarGridLancamento(int numeroParcelas)
        {
            decimal nSomaValorBruto = 0;
            decimal nSomaValorLiquido = 0;
            for (int numeroParcelaAtual = 1; numeroParcelaAtual <= numeroParcelas; numeroParcelaAtual++)
            {
                TextBox txtnValorLancamentoRec = (TextBox)dtgLancamento.Rows[numeroParcelaAtual - 1].FindControl("txtnValorLancamentoRec");
                decimal valorParcela = txtnValorLancamentoRec.Text == "" ? 0 : Convert.ToDecimal(txtnValorLancamentoRec.Text);
                nSomaValorLiquido += valorParcela;

                TextBox txtnValorBrutoLancamentoRec = (TextBox)dtgLancamento.Rows[numeroParcelaAtual - 1].FindControl("txtnValorBrutoLancamentoRec");
                decimal valorParcelaBruto = txtnValorBrutoLancamentoRec.Text == "" ? 0 : Convert.ToDecimal(txtnValorBrutoLancamentoRec.Text);
                nSomaValorBruto += valorParcelaBruto;

            }

            decimal nValorBruto = txtnValorBruto.Text == "" ? 0 : Convert.ToDecimal(txtnValorBruto.Text);
            decimal nValorLiquido = Convert.ToDecimal(txtnValorOriginal.Text);

            if (nSomaValorBruto != nValorBruto || nSomaValorLiquido != nValorLiquido)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region | Limpar Campos
        void LimpaCampos()
        {
            hddidContasReceber.Value = "0";
            txtidContasReceber.Text = "Novo";
            txtsCodigo.Text = "";
            txtdtEmissao.Text = DateTime.Today.ToString("u").Substring(0, 10);
            txtdtVencimento.Text = "";
            txtsDocumento.Text = "";
            txtnValorOriginal.Text = "";
            txtnValorBruto.Text = "";
            txtnSaldo.Text = "";
            ddlidContabil.SelectedValue = "0";
            ddlidEmpresa.SelectedValue = "0";
            hddidEmpresa.Value = "0";
            ddlidFormaRecebimento.SelectedValue = "0";
            ddlidCentroDeCusto.SelectedValue = "0";
            ddlidMeioRecebimento.SelectedValue = "0";
            txtnParcelas.Text = "";
            ddlidCategoriaReceber.SelectedValue = "0";
            txtsObservacaoGeral.Text = "";

            bs_Recebimento.Clear();

            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;

        }
        #endregion

        #region | Validação 
        private bool ValidarDadosVencimento()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (!Validacoes.ValidarData(txtdtVencimento))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data Previsão de Recebimento inválida!";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            decimal nValorRecebimento_Info_Rec = 0;
            decimal ValorOriginal = 0;
            decimal nValorComDesconto = 0;
            decimal nValorAoLiquidar = 0;


            if (decimal.TryParse(txtnValorOriginal.Text, out ValorOriginal))
            {
                foreach (var linha in bs_Recebimento)
                {
                    if (linha.idFormaRecebimento_Info_Rec == 1) continue;

                    nValorRecebimento_Info_Rec += (linha.nValorRecebimento_Info_Rec);
                }

                if (nValorRecebimento_Info_Rec > ValorOriginal)
                {
                    decimal diferenca = nValorRecebimento_Info_Rec - ValorOriginal;
                    sMensagemErro += string.Format("O valor inserido excede o valor Liquido. A diferença é de {0:C}.", diferenca) + "</br>";
                }
            }
            nValorRecebimento_Info_Rec = 0;
            if (decimal.TryParse(txtnValorOriginal.Text, out ValorOriginal))
            {

                foreach (var linha in bs_Recebimento)
                {
                    if (linha.idFormaRecebimento_Info_Rec == 1) continue;

                    nValorComDesconto += (linha.nValorRecebimento_Info_Rec);
                }

                if (nValorComDesconto > ValorOriginal)
                {
                    decimal diferenca = nValorComDesconto - ValorOriginal;
                    sMensagemErro += string.Format("A soma dos valores Recebidos e Descontos não batem com o Valor Liquido. A diferença é de {0:C}.", diferenca) + "</br>";
                }
            }
            //Data de Emissão
            if (!Validacoes.ValidarData(txtdtEmissao))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data Emissão inválida!";
            }
            //Cliente
            if (Pesquisa_Parceiros.idParceiro == 0)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Cliente!";
            }
            //Valor Bruto
            if (txtnValorBruto.Text.Length < 3)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Bruto Inválido!";
            }
            //Valor Liquido
            if (txtnValorOriginal.Text.Length < 3)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Liquido Inválido!";
            }
            decimal ValorLiquido = txtnValorOriginal.Text == "" ? 0 : Convert.ToDecimal(txtnValorOriginal.Text);
            decimal ValorBruto = txtnValorBruto.Text == "" ? 0 : Convert.ToDecimal(txtnValorBruto.Text);
            if (ValorBruto < ValorLiquido)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Bruto não pode se menor que o valor Líquido";

            }
            //Nota Fiscal
            if (txtsCodigo.Text.Length < 1)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Nota Fiscal inválida!";
            }
            //Numero pedido
            if (txtsDocumento.Text.Length < 2)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Documento/ Número do pedido Inválido!";
            }
            //Empresa
            if (ddlidEmpresa.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
            }
            //Categoria
            if (ddlidCategoriaReceber.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Categoria!";
            }
            //Forma de Recebimento
            if (ddlidFormaRecebimento.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma forma de recebimento!";
            }
            //VERIFICAR SE É OBRIGATORIO!!!!!!!!!!!!!!!!!!!!!
            ////Centro de Custo
            //if (ddlidCentroDeCusto.SelectedValue == "0")
            //{
            //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Centro de custo invalido!";
            //}
            //Meio de Recebimento

            //if (FormaRecebimento_ExigeContaBancaria() && ddlContaBancaria.SelectedValue == "0")
            //{
            //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Conta Bancária!";
            //}

            if (ddlidMeioRecebimento.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo de Recebimento!";
            }


            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }

            if (txtnSaldo.Text != "0,00")
            {
                cmdSalvar.Visible = true;
            }

            return bRetorno;
        }
        
        private bool ValidaDadosModal()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if(hddsTituloAdiantado.Value == "S")
            {
                if (txtsObservacaoStatus.Text.Length < 3)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite um Motivo para a mudança de Status";
                }
            }
            else
            {
                if (ddlsAlteraStatus.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um status";
                }
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemModalAlteraStatus.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        #endregion

        #region | Script 
        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            //Mensagens de Confirmação
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
            sb.Append("$v192('[id*=cmdSalvar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("$v192(\"#dialog-Excluir\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_EXCLUIR\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=BtnExcluirParcela]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Excluir').dialog('open');");
            sb.Append("});");

            sb.Append("$v192(\"#dialog-Reabrir\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_Reabrir\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=BtnReabrirTitulo]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Reabrir').dialog('open');");
            sb.Append("});");


            sb.Append("$('[id*=txtnParcela]').mask('00#');");

            sb.Append("$('[id*=txtnValorOriginal]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnSaldo]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnValorPago]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=nValorRecebimento]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=nValorLancamentoRec]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnValorBruto]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnMulta]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnJuros]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnDesconto]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnTarifa]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnValorTotalRec]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnTotal]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnValorRecebimento_Info_Rec]').mask('000.000.000.000.000,00', { reverse: true });");

            if (sFuncao != "")
            {
                sb.Append(sFuncao);
            }
            sb.Append("});");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }
        #endregion

        #region | Combos/DDL
        void PopularCombos()
        {
            //Principal
            FUNCOES.Popula_Combo(ddlidContabil, "sp_Select 'Flow_CodigoContabil'", "idContabil", "sDscCodContabil", false, "Selecione o Código Contábil", "0");
            FUNCOES.Popula_Combo(ddlidFormaRecebimento, "sp_Select 'tbl_Flow_CondicaodePagamento_Tipo'", "idTipoCondicaoPagamento", "sDscTipoCondicaoPagamento", false, "Selecione o Recebimento", "0");
            FUNCOES.Popula_Combo(ddlidCentroDeCusto, "sp_Select 'Flow_Adm_CentroDeCusto'", "idCentroDeCusto", "sDescricao", false, "Selecione um Centro de Custo", "0");
            FUNCOES.Popula_Combo(ddlidMeioRecebimento, "sp_Select 'tbl_Flow_Adm_MeioPagamento'", "idMeioPagamento", "sDscMeioPagamento", false, "Selecione um Meio", "0");
            FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_empresa'", "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
            FUNCOES.Popula_Combo(ddlidCategoriaReceber, "sp_Select 'Flow_Adm_Contas_Receber_Categoria'", "idCategoriaReceber", "sDscCategoriaReceber", false, "Selecione a Categoria", "0");

            //Recebimento
            FUNCOES.Popula_Combo(ddlidFormaRecebimento_Info_Rec, "sp_Select 'tbl_Flow_CondicaodePagamento_Tipo'", "idTipoCondicaoPagamento", "sDscTipoCondicaoPagamento", false, "Selecione o Recebimento", "0");
            FUNCOES.Popula_Combo(ddlidConta_Info_Rec, "sp_Select 'tbl_Flow_Adm_ContasBancarias'", "idConta", "sDscConta", false, "Selecione uma Conta", "0");
            FUNCOES.Popula_Combo(ddlContaBancaria, "sp_Select 'tbl_Flow_Adm_ContasBancarias'", "idConta", "sDscConta", false, "Selecione uma Conta Bancária", "0");

            //Altera o text no ddl Forma de Recebimento
            ListItem itemFormaRecebimento = ddlidFormaRecebimento.Items.FindByValue("1");
            ListItem itemFormaRecebimento_Info_Rec = ddlidFormaRecebimento_Info_Rec.Items.FindByValue("1");
            itemFormaRecebimento.Text = "Devolução";
            itemFormaRecebimento_Info_Rec.Text = "Devolução";

        }
        #endregion

        #region | Recebimento
        bool FormaRecebimentoExibeTarifa(string idFormaRecebimento)
        {
            int idForma;
            if (!int.TryParse(idFormaRecebimento, out idForma))
                return false;

            return idForma == (int)EnumTiposPagamentos.CartaoDeCredito
                || idForma == (int)EnumTiposPagamentos.MercadoPago;
        }

        void AtualizarVisibilidadeTarifa()
        {
            bool exibir = FormaRecebimentoExibeTarifa(ddlidFormaRecebimento_Info_Rec.SelectedValue);
            Div_Tarifa.Visible = exibir;

            if (!exibir)
                txtnTarifa.Text = string.Empty;
        }

        void Popular_dtgRecebimento(DataSet dsPesquisa)
        {
            foreach (DataRow row in dsPesquisa.Tables[1].Rows)
            {
                FrameWork.cls_Recebimento objItem = new FrameWork.cls_Recebimento();

                objItem.idContasReceber = Convert.ToInt32(row["idContasReceber"].ToString());
                objItem.idRegistroRecebimento = Convert.ToInt32(row["idRegistroRecebimento"].ToString());

                objItem.idLinha = bs_Recebimento.Count() + 1;
                objItem.sFuncao = "SEM ALTERAÇÃO";

                objItem.nValorRecebimento_Info_Rec = ConverterStringDecimal(row["nValorRecebimento_Info_Rec"].ToString());
                objItem.dtRecebimento_Info_Rec = row["dtRecebimento_Info_Rec"].ToString();
                objItem.nNumeroParcela_Info_Rec = Convert.ToInt32(row["nNumeroParcela_Info_Rec"].ToString());
                objItem.idConta_Info_Rec = Convert.ToInt32(row["idConta_Info_Rec"].ToString());
                objItem.sDscConta_Info_Rec = row["sDscConta_Info_Rec"].ToString();
                objItem.dtAtualizacao = row["dtAtualizacao"].ToString();
                objItem.idFormaRecebimento_Info_Rec = Convert.ToInt32(row["idFormaRecebimento_Info_Rec"].ToString());
                objItem.sDscFormaRecebimento_Info_Rec = row["sDscFormaRecebimento_Info_Rec"].ToString();
                objItem.idArquivo = Convert.ToInt32(row["idArquivo"].ToString());
                objItem.sNomeArquivo = row["sNomeArquivo"].ToString();

                objItem.nDesconto = ConverterStringDecimal(row["nDesconto"].ToString());
                objItem.nJuros = ConverterStringDecimal(row["nJuros"].ToString());
                objItem.nMulta = ConverterStringDecimal(row["nMulta"].ToString());
                if (row.Table.Columns.Contains("nTarifa"))
                    objItem.nTarifa = ConverterStringDecimal(row["nTarifa"].ToString());
                objItem.nValorTotalRec = ConverterStringDecimal(row["nValorTotalRec"].ToString());
                objItem.sCor = row["sCor"].ToString();
                objItem.idCredito = Convert.ToInt32(row["idCredito"].ToString());

                bs_Recebimento.Add(objItem);
            }

            BtnEdicaoRecebimento.Visible = false;

            txtnMulta.Visible = true;
            txtnJuros.Visible = true;
            txtnDesconto.Visible = true;

            dtgRecebimento_DataBind();
            LimpaCampos_Recebimento();
        }

        protected void cmdRecebimento_Incluir_Click(object sender, EventArgs e)
        {
            string sMensagem = "";

            if (ValidarDados_Recebimento(ref sMensagem))
            {
                FrameWork.cls_Recebimento objItem = new FrameWork.cls_Recebimento();
                string[] vidContasReceber = hddidContasReceber.Value.Split(',');
                string idContasReceber = vidContasReceber[0].ToString();

                objItem.idLinha = bs_Recebimento.Count() + 1;
                objItem.sFuncao = "INSERIR_Recebimento";
                objItem.idContasReceber = Convert.ToInt32(idContasReceber);
                objItem.idConta_Info_Rec = Convert.ToInt32(ddlidConta_Info_Rec.SelectedValue);
                objItem.sDscConta_Info_Rec = ddlidConta_Info_Rec.SelectedItem.ToString();
                objItem.dtRecebimento_Info_Rec = txtdtRecebimento_Info_Rec.Text;
                objItem.idFormaRecebimento_Info_Rec = Convert.ToInt32(ddlidFormaRecebimento_Info_Rec.SelectedValue);
                objItem.sDscFormaRecebimento_Info_Rec = ddlidFormaRecebimento_Info_Rec.SelectedItem.ToString();
                int proximaParcela = bs_Recebimento.Count + 1;
                objItem.nNumeroParcela_Info_Rec = proximaParcela;

                objItem.sNomeArquivo = "";

                if (ddlidFormaRecebimento_Info_Rec.SelectedValue != "1")
                {
                    objItem.nValorRecebimento_Info_Rec = ConverterStringDecimal(txtnValorRecebimento_Info_Rec.Text);
                    objItem.nMulta = ConverterStringDecimal(txtnMulta.Text);
                    objItem.nJuros = ConverterStringDecimal(txtnJuros.Text);
                    objItem.nDesconto = ConverterStringDecimal(txtnDesconto.Text);
                    objItem.nTarifa = FormaRecebimentoExibeTarifa(ddlidFormaRecebimento_Info_Rec.SelectedValue)
                        ? ConverterStringDecimal(txtnTarifa.Text)
                        : 0;
                    objItem.nValorTotalRec = ConverterStringDecimal(txtnValorTotalRec.Text);
                }
                else
                {
                    objItem.nValorRecebimento_Info_Rec = ConverterStringDecimal(ddlCreditoDevolucao.SelectedItem.ToString());
                    objItem.nMulta = 0;
                    objItem.nJuros = 0;
                    objItem.nDesconto = 0;
                    objItem.nTarifa = 0;
                    objItem.nValorTotalRec = ConverterStringDecimal(ddlCreditoDevolucao.SelectedItem.ToString());
                    objItem.idCredito = Convert.ToInt32(ddlCreditoDevolucao.SelectedValue);
                }

                bs_Recebimento.Add(objItem);

                dtgRecebimento_DataBind();
                LimpaCampos_Recebimento();
                div_valor_recebimento.Visible = true;
                div_devolucao.Visible = false;

                FUNCOES.Scripts.FocusScript(Page, txtdtRecebimento_Info_Rec.ClientID);
            }
            else
            {
                MensagemLancamento.MostraMensagem_Erro(sMensagem, false);
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", " document.getElementById('cphCorpo_Div_Recebimento').scrollIntoView();", true);
        }

        bool Salvar_Recebimento(string idContasReceber)
        {
            bool bRetorno = true;
            Lancamentos_Salvar();
            try
            {

                foreach (var Recebimento_Linha in bs_Recebimento)
                {
                    int idArquivo = Recebimento_Linha.idArquivo;
                    if (Recebimento_Linha.sNomeArquivo != "" && Recebimento_Linha.idArquivo == 0)
                    {
                        TT_Flow.FrameWork.cls_Arquivos Arquivo = new FrameWork.cls_Arquivos();
                        Arquivo.idTipoArquivo = 9999;
                        Arquivo.idObjeto = Recebimento_Linha.idRegistroRecebimento;
                        Arquivo.sNomeArquivo = Recebimento_Linha.sNomeArquivo;
                        Arquivo.sDscArquivo = Recebimento_Linha.sObservacaoArquivo;
                        Arquivo.sObservacao = "";
                        Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                        Arquivo.vbArquivo = Recebimento_Linha.objArquivo;
                        Arquivo.dtExpiracaoDoc = "";

                        Recebimento_Linha.idArquivo = Convert.ToInt32(BD.Retorno.DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));

                        if (Recebimento_Linha.sFuncao == "SEM ALTERAÇÃO")
                        {
                            Recebimento_Linha.sFuncao = "INSERIR_Recebimento";
                        }
                    }

                    if (Recebimento_Linha.sFuncao != "SEM ALTERAÇÃO")
                    {
                        Dictionary<String, String> vParametroItensRecebimento = new Dictionary<string, string>();

                        vParametroItensRecebimento["@idRegistro"] = Recebimento_Linha.idRegistroRecebimento.ToString();
                        vParametroItensRecebimento["@sFuncao"] = Recebimento_Linha.sFuncao;
                        vParametroItensRecebimento["@idContasReceber"] = idContasReceber;
                        vParametroItensRecebimento["@nValorRecebimento_Info_Rec"] = Recebimento_Linha.nValorRecebimento_Info_Rec.ToString().Replace(",", ".");
                        vParametroItensRecebimento["@idConta_Info_Rec"] = Recebimento_Linha.idConta_Info_Rec.ToString();
                        vParametroItensRecebimento["@dtRecebimento_Info_Rec"] = Recebimento_Linha.dtRecebimento_Info_Rec.ToString();
                        vParametroItensRecebimento["@nNumeroParcela_Info_Rec"] = Recebimento_Linha.nNumeroParcela_Info_Rec.ToString();
                        vParametroItensRecebimento["@idFormaRecebimento_Info_Rec"] = Recebimento_Linha.idFormaRecebimento_Info_Rec.ToString();
                        vParametroItensRecebimento["@sDscFormaRecebimento_Info_Rec"] = Recebimento_Linha.sDscFormaRecebimento_Info_Rec.ToString();
                        vParametroItensRecebimento["@sDscConta_Info_Rec"] = Recebimento_Linha.sDscConta_Info_Rec.ToString();

                        vParametroItensRecebimento["@nMulta"] = Recebimento_Linha.nMulta.ToString().Replace(",", ".");
                        vParametroItensRecebimento["@nJuros"] = Recebimento_Linha.nJuros.ToString().Replace(",", ".");
                        vParametroItensRecebimento["@nDesconto"] = Recebimento_Linha.nDesconto.ToString().Replace(",", ".");
                        vParametroItensRecebimento["@nTarifa"] = Recebimento_Linha.nTarifa.ToString().Replace(",", ".");
                        vParametroItensRecebimento["@nValorTotalRec"] = Recebimento_Linha.nValorTotalRec.ToString().Replace(",", ".");

                        vParametroItensRecebimento["@idArquivo"] = Recebimento_Linha.idArquivo.ToString();
                        vParametroItensRecebimento["@idUsuarioAtualizacao"] = IDENTITY.Variaveis.idUsuario();
                        vParametroItensRecebimento["@idRegistroCredito"] = Recebimento_Linha.idCredito.ToString();
                        vParametroItensRecebimento.Add("@sEmailPagoAtraso", SwitchAtivo_EmailPagoAtraso.Recuperar());
                        BD.ExecutarDataSet(sProcedure, vParametroItensRecebimento);
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
                bRetorno = false;
            }

            //btnSalvarRecebimento.Visible =false;
            cmdSalvar.Visible = true;
            btnAlterarVencimento.Visible = false;
            return bRetorno;

        }

        protected void BtnEdicaoRecebimento_Click(object sender, EventArgs e)
        {
            int idLinha = Convert.ToInt32(hddRecebimento_idLinha.Value);
            int index = bs_Recebimento.FindIndex(x => x.idLinha.Equals(idLinha));
            string sMensagem = "";
            if (ValidarDados_Recebimento(ref sMensagem))
            {
                decimal nValor = 0;
                decimal nValorMulta = 0;
                decimal nValorJuros = 0;
                decimal nValorDesconto = 0;
                decimal nValorTarifa = 0;
                decimal nValorTotalRec = 0;

                nValor = Convert.ToDecimal(txtnValorRecebimento_Info_Rec.Text);
                bs_Recebimento[index].nValorRecebimento_Info_Rec = (nValor);
                bs_Recebimento[index].idConta_Info_Rec = Convert.ToInt32(ddlidConta_Info_Rec.SelectedValue);
                bs_Recebimento[index].dtRecebimento_Info_Rec = txtdtRecebimento_Info_Rec.Text;
                bs_Recebimento[index].idFormaRecebimento_Info_Rec = Convert.ToInt32(ddlidFormaRecebimento_Info_Rec.SelectedValue);
                decimal.TryParse(txtnMulta.Text.Replace(".", ","), out nValorMulta);
                bs_Recebimento[index].nMulta = nValorMulta;
                decimal.TryParse(txtnJuros.Text.Replace(".", ","), out nValorJuros);
                bs_Recebimento[index].nJuros = nValorJuros;
                decimal.TryParse(txtnDesconto.Text.Replace(".", ","), out nValorDesconto);
                bs_Recebimento[index].nDesconto = nValorDesconto;
                decimal.TryParse(txtnTarifa.Text.Replace(".", ","), out nValorTarifa);
                bs_Recebimento[index].nTarifa = FormaRecebimentoExibeTarifa(ddlidFormaRecebimento_Info_Rec.SelectedValue)
                    ? nValorTarifa
                    : 0;
                decimal.TryParse(txtnValorTotalRec.Text.Replace(".", ","), out nValorTotalRec);
                bs_Recebimento[index].nValorTotalRec = nValorTotalRec;

                bs_Recebimento[index].sFuncao = "INSERIR_Recebimento";

                dtgRecebimento_DataBind();
                LimpaCampos_Recebimento();
                RegistraScript("");//ok


                Salvar_ContasReceber();
                Response.Redirect(Request.RawUrl);
            }
        }

        protected void dtgRecebimento_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(dtgRecebimento.Rows[e.RowIndex].Cells[0].Text);
            bs_Recebimento[bs_Recebimento.FindIndex(x => x.idLinha.Equals(idLinha))].sFuncao = "EXCLUIR_Recebimento";
            dtgRecebimento_DataBind();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtnValorRecebimento_Info_Rec]').focus();", true);

            Salvar_ContasReceber();
            Response.Redirect(Request.RawUrl);
        }

        protected void dtgRecebimento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColunaBotoes = e.Row.Cells.Count - 1;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                cls_Recebimento recebimento = (cls_Recebimento)e.Row.DataItem;

                LinkButton lnkEditar = (LinkButton)e.Row.Cells[11].FindControl("lnkRecebimento_Editar");
                if (recebimento.idFormaRecebimento_Info_Rec == 1)
                {
                    lnkEditar.Visible = false;
                }

                string sNomeArquivo = DataBinder.Eval(e.Row.DataItem, "sNomeArquivo").ToString();
                string sCor = DataBinder.Eval(e.Row.DataItem, "sCor")?.ToString() ?? string.Empty;
                e.Row.CssClass = sCor;
                foreach (LinkButton lnk in e.Row.Cells[nColunaBotoes].Controls.OfType<LinkButton>())
                {
                    if ((lnk.ID == "lnkRecebimento_Download" && sNomeArquivo == "") || (lnk.ID == "lnkRecebimento_UpLoad" && sNomeArquivo != ""))
                    {
                        lnk.Visible = false;
                    }
                }
            }
            if (txtnSaldo.Text == "0,00")
            {
                GRID.EsconderColunas(e, 11);
            }
            GRID.EsconderColunas(e, 0);
        }

        protected void dtgRecebimento_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = 0;

            if (e.CommandArgument != null && !string.IsNullOrEmpty(e.CommandArgument.ToString()))
            {
                idLinha = int.Parse(e.CommandArgument.ToString());
                hddsBloco.Value = "Recebimento";
                hddRecebimento_idLinha.Value = idLinha.ToString();
            }

            if (e.CommandArgument.ToString() != "")
            {
                int.Parse(e.CommandArgument.ToString());
                hddsBloco.Value = "Recebimento";
                hddRecebimento_idLinha.Value = idLinha.ToString();
            }

            if (e.CommandName == "Upload_Arquivo")
            {
                AbrirModal_EnvioArquivo(eBloco.Recebimento, idLinha, bs_Recebimento[bs_Recebimento.FindIndex(x => x.idLinha.Equals(idLinha))].nNumeroParcela_Info_Rec.ToString());
            }
            else if (e.CommandName == "Download_Arquivo")
            {
                Efetuar_Download_Arquivo(eBloco.Recebimento, idLinha);
            }
            else if (e.CommandName == "Editar")
            {
                cmdRecebimento_Incluir.Visible = false;
                BtnEdicaoRecebimento.Visible = true;
                Div_Desconto.Visible = true;
                var Recebimento = bs_Recebimento[bs_Recebimento.FindIndex(x => x.idLinha.Equals(idLinha))];

                txtnValorRecebimento_Info_Rec.Text = Recebimento.nValorRecebimento_Info_Rec.ToString();
                txtnMulta.Text = Recebimento.nMulta.ToString();
                txtnJuros.Text = Recebimento.nJuros.ToString();
                txtnDesconto.Text = Recebimento.nDesconto.ToString();
                txtnTarifa.Text = Recebimento.nTarifa.ToString();
                txtnValorTotalRec.Text = Recebimento.nValorTotalRec.ToString();


                ddlidConta_Info_Rec.SelectedValue = Recebimento.idConta_Info_Rec.ToString();

                var dt3 = Convert.ToDateTime(Recebimento.dtRecebimento_Info_Rec);
                txtdtRecebimento_Info_Rec.Text = dt3.ToString(@"yyyy/MM/dd").Replace('/', '-');
                ddlidFormaRecebimento_Info_Rec.SelectedValue = Recebimento.idFormaRecebimento_Info_Rec.ToString();
                AtualizarVisibilidadeTarifa();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtdtRecebimento_Info_Rec]').focus();", true);

            }

            RegistraScript("");//ok
        }

        private bool ValidarDados_Recebimento(ref string sMensagem)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (!Validacoes.ValidarData(txtdtRecebimento_Info_Rec))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data de Recebimento Inválida";
            }

            if (ddlidFormaRecebimento_Info_Rec.SelectedValue != "1")
            {
                if (ddlidFormaRecebimento_Info_Rec.SelectedValue == "0")
                {
                    if (sMensagemErro != "")
                    {
                        sMensagemErro = sMensagemErro + "</br>";
                    }
                    sMensagemErro += "Selecione uma forma de Recebimento!";
                }

                if (ddlidConta_Info_Rec.SelectedValue == "0")
                {
                    if (sMensagemErro != "")
                    {
                        sMensagemErro = sMensagemErro + "</br>";
                    }
                    sMensagemErro += "Selecione uma conta";
                }

                if (txtnValorRecebimento_Info_Rec.Text.Length < 3)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Inválido";
                }
            }
            else
            {
                if (ddlCreditoDevolucao.SelectedValue == "0")
                {
                    if (sMensagemErro != "")
                    {
                        sMensagemErro = sMensagemErro + "</br>";
                    }
                    sMensagemErro += "Selecione um Crédito Disponível!";
                }
                else if (bs_Recebimento.Any(item => item.idCredito.ToString() == ddlCreditoDevolucao.SelectedValue))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Crédito já foi selecionado" + "</br>";
                }
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
            }

            sMensagem = sMensagemErro;

            return bRetorno;

        }

        void LimpaCampos_Recebimento()
        {

            txtnValorRecebimento_Info_Rec.Text = "";
            ddlidConta_Info_Rec.SelectedValue = "0";
            txtdtRecebimento_Info_Rec.Text = "";
            ddlidFormaRecebimento_Info_Rec.SelectedValue = "0";
            txtnMulta.Text = "";
            txtnJuros.Text = "";
            txtnDesconto.Text = "";
            txtnTarifa.Text = "";
            txtnValorTotalRec.Text = "";
            ddlCreditoDevolucao.SelectedValue = "0";

            AtualizarVisibilidadeTarifa();

            hddRecebimento_idLinha.Value = "";

            BtnEdicaoRecebimento.Visible = false;
            cmdRecebimento_Incluir.Visible = true;
        }

        void dtgRecebimento_DataBind()
        {
            try
            {
                dtgRecebimento.DataSource = bs_Recebimento.Where(c => c.sFuncao.ToString() != "EXCLUIR_Recebimento");
                dtgRecebimento.DataBind();
                RegistraScript("");
                GRID.SomarColunas(dtgRecebimento, true, GRID.Formatação.Moeda, 8);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar Grid de Recebimento: " + ex.Message);
            }
        }
        #endregion

        #region | Eventos 
        void Popular_Aba_Historico(DataSet dsPesquisa)
        {
            gv_Historico.DataSource = dsPesquisa.Tables[1];
            gv_Historico.DataBind();
            aba_Historico.Visible = false;
        }

        void Popular_Aba_Log(DataSet dsPesquisa)
        {
            gv_Log.DataSource = dsPesquisa.Tables[2];
            gv_Log.DataBind();
            Aba_Log.Visible = true;
        }

        protected void gv_Log_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[4].Text = HttpUtility.HtmlDecode(e.Row.Cells[4].Text);
            }
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

        protected void btnInformarRecebimento_Click(object sender, EventArgs e)
        {
            //txtnValorPago.ReadOnly = false;
        }

        protected void ddlidMeioRecebimento_SelectedIndexChanged(object sender, EventArgs e)
        {
            int MeioRecebimento = Convert.ToInt32(ddlidMeioRecebimento.SelectedValue);

            if (MeioRecebimento != 0)
            {
                DIV_Lancamentos.Visible = true;

                var idContasReceber = hddidContasReceber.Value;
                Div_nParcelas.Visible = false;

                if (idContasReceber == "0")
                {
                    Div_sQuantidadeParcela.Visible = false;
                    if (ddlidMeioRecebimento.SelectedValue != "2")
                    {
                        bs_LancamentoRec.Clear();
                        IncluirLancamento();
                        FUNCOES.Scripts.FocusScript(Page, dtgLancamento.Rows[0].Cells[4].Controls[1].ClientID);
                    }

                    if (ddlidMeioRecebimento.SelectedValue == "2")
                    {
                        Div_nParcelas.Visible = true;
                        FUNCOES.Scripts.FocusScript(Page, txtnParcelas.ClientID);

                    }


                }
                else
                {
                    DIV_Lancamentos.Visible = false;
                    Div_nParcelas.Visible = false;
                }
            }
            else
            {
                Div_sQuantidadeParcela.Visible = false;
                DIV_Lancamentos.Visible = false;
            }

        }

        protected void txtnValorRecebimento_Info_Rec_TextChanged(object sender, EventArgs e)
        {
            txtnTotal.ReadOnly = true;
            bs_LancamentoRec.Clear();
            DateTime dataVencimento;
            bool conversaoSucessoData = DateTime.TryParse(txtdtVencimento.Text, out dataVencimento);

            decimal valorRecebimento;
            decimal ValorOriginal = 0;
            decimal.TryParse(txtnValorOriginal.Text, out ValorOriginal);
            bool conversaoSucesso = Decimal.TryParse(txtnValorRecebimento_Info_Rec.Text, out valorRecebimento);


            decimal ValornTotal = 0;
            decimal.TryParse(txtnTotal.Text, out ValornTotal);

            if (conversaoSucesso)
            {
                if (conversaoSucessoData)
                {
                    if (dataVencimento < DateTime.Now)
                    {
                        Div_Desconto.Visible = true;
                        txtnDesconto.Text = "0";
                    }
                    else
                    {

                        //if (valorRecebimento < ValorOriginal)
                        //{

                        //    Div_Desconto.Visible = true;
                        //    decimal desconto = ValornTotal - valorRecebimento;
                        //    txtnDesconto.Text = Math.Max(desconto, 0).ToString();
                        //}
                        //else
                        //{
                        //    Div_Desconto.Visible = false;
                        //    txtnDesconto.Text = "0";
                        //}

                    }

                }

            }

            txtnValorTotalRec_TextChanged(sender, e);
        }

        //private cls_Adiantamento_Detalhe CalcularAdiantamento_Detalhe(int idContasReceber, decimal valorOriginal, DateTime dataVencimento)
        //{
        //    txtdtEmissaoAdiantamento.Text = DateTime.Now.ToString("yyyy.MM.dd");

        //    string txtnTaxaJurosSemSimbolo = txtnTaxaJuros.Text.Replace("%", "");
        //    string txtnIOFSemSimbolo = txtnIOF.Text.Replace("%", "");
        //    string txtnTarifasSemSimbolo = txtnTarifas.Text.Replace("R$", "");
        //    string txtnIOFAdicionalSemSimbolo = txtnIOFAdicional.Text.Replace("%", "");

        //    //Valores dos juros
        //    decimal taxaJuros = Convert.ToDecimal(txtnTaxaJurosSemSimbolo, CultureInfo.GetCultureInfo("pt-BR"));
        //    decimal iof = Convert.ToDecimal(txtnIOFSemSimbolo, CultureInfo.GetCultureInfo("pt-BR"));
        //    decimal tarifas = Convert.ToDecimal(txtnTarifasSemSimbolo, CultureInfo.GetCultureInfo("pt-BR"));
        //    decimal iofAdicional = Convert.ToDecimal(txtnIOFAdicionalSemSimbolo, CultureInfo.GetCultureInfo("pt-BR"));
        //    DateTime.TryParseExact(txtdtEmissaoAdiantamento.Text, "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime EmissaoAdiantamento);

        //    //Calculo de dias
        //    int nDias = (dataVencimento - EmissaoAdiantamento).Days;

        //    //Clculo dos Juros/IOF
        //    decimal nValorJurosTotal = ((valorOriginal * taxaJuros) / 30) * nDias;

        //    decimal nValoriofTotal = ((valorOriginal * iof) / 30) * nDias;
        //    decimal nValorIofAdicionalTotal = (valorOriginal * iofAdicional);
        //    decimal nSomaIOF_Detalhe = nValoriofTotal + nValorIofAdicionalTotal;

        //    decimal valorTotalComJuros = valorOriginal + nValorJurosTotal + nValoriofTotal + tarifas;
        //    decimal valorEmprestimo = valorOriginal + tarifas;

        //    decimal nDespesas = nValorJurosTotal + nValorIofAdicionalTotal + nValoriofTotal + tarifas;

        //    DateTime dtVencimento = dataVencimento;

        //    int idContasPagar = 0;

        //    return new cls_Adiantamento_Detalhe(
        //        idContasReceber_Detalhe: idContasReceber,
        //        idContasPagar_Detalhe: idContasPagar,
        //        nValorTotalComJuros_Detalhe: valorTotalComJuros,
        //        nSomaValorOriginal_Detalhe: valorOriginal,
        //        nTotalJuros_Detalhe: nValorJurosTotal,
        //        nTaxaJuros_Detalhe: taxaJuros,
        //        nIOF_Detalhe: iof,
        //        nTarifas_Detalhe: tarifas,
        //        dtVencimento_Detalhe: dtVencimento.ToString("yyyy-MM-dd"),
        //        nIOFAdicional_Detalhe: iofAdicional,
        //        nTaxaJurosNominal_Detalhe: 0,
        //        nDespesas_Detalhe: nDespesas,
        //        nValorEmprestimo_Detalhe: valorEmprestimo,
        //        nCET_Detalhe: 0,
        //        nDias_Detalhe: nDias,
        //        nIOFTotal_Detalhe: nValoriofTotal,
        //        nIOFAdicionalTotal_Detalhe: nValorIofAdicionalTotal,
        //        nJurosTotal_Detalhe: nValorJurosTotal,
        //        nSomaIOF_Detalhe: nSomaIOF_Detalhe,
        //        dtEmissaoAdiantamento_Detalhe: EmissaoAdiantamento.ToString("yyyy-MM-dd"),
        //        idEmpresa_Detalhe: 0

        //    );

        //}
        protected void ddlidCategoriaReceber_TextChanged(object sender, EventArgs e)
        {
            int idCategoriaTipo_Consulta = 0;
            try
            {
                DataSet dsContabil;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "Autoselecao_Contabil");
                vParametros.Add("@idCategoriaReceber", ddlidCategoriaReceber.SelectedValue);

                dsContabil = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Receber", vParametros);

                if (BD.ValidarDataSet(dsContabil))
                {
                    ddlidContabil.SelectedValue = RETORNO.DATASET(dsContabil, 0, "idContabil");
                    idCategoriaTipo_Consulta = Convert.ToInt32(RETORNO.DATASET(dsContabil, 0, "idCategoriaTipo"));
                }

            }
            catch
            {
                ddlidContabil.SelectedValue = "";
            }

            string sCategoriaTipo = ddlidCategoriaReceber.SelectedItem.Text;

            switch (idCategoriaTipo_Consulta)
            {
                case 1: //Material

                    break;
                case 2: //Serviço


                    break;

            }
            FUNCOES.Scripts.FocusScript(Page, ddlidFormaRecebimento.ClientID);
        }

        bool TryParseDecimalMoeda(string texto, out decimal valor, string mensagemErro)
        {
            valor = 0;
            if (string.IsNullOrWhiteSpace(texto))
                return true;

            texto = texto.Trim();
            if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.GetCultureInfo("pt-BR"), out valor)
                || decimal.TryParse(texto.Replace(".", "").Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out valor))
            {
                if (valor < 0)
                {
                    MensagemPagina_Recebimento.MostraMensagem_Erro(mensagemErro);
                    return false;
                }

                return true;
            }

            MensagemPagina_Recebimento.MostraMensagem_Erro(mensagemErro);
            return false;
        }

        protected void txtnValorTotalRec_TextChanged(object sender, EventArgs e)
        {
            decimal ValorRecebido = 0;
            decimal ValorMulta = 0;
            decimal ValorJuros = 0;
            decimal ValorDesconto = 0;
            decimal ValorTarifa = 0;
            decimal ValorTotal = 0;

            if (!TryParseDecimalMoeda(txtnValorRecebimento_Info_Rec.Text, out ValorRecebido, "Valor Recebido inválido!"))
                return;
            if (!TryParseDecimalMoeda(txtnMulta.Text, out ValorMulta, "Valor da Multa inválido!"))
                return;
            if (!TryParseDecimalMoeda(txtnJuros.Text, out ValorJuros, "Valor de Juros inválido!"))
                return;
            if (!TryParseDecimalMoeda(txtnDesconto.Text, out ValorDesconto, "Valor de Desconto inválido!"))
                return;
            if (Div_Tarifa.Visible && !TryParseDecimalMoeda(txtnTarifa.Text, out ValorTarifa, "Valor da Tarifa inválido!"))
                return;

            var control = sender as Control;
            if (control != null)
            {
                if (control.ID == "txtnValorRecebimento_Info_Rec")
                {
                    FUNCOES.Scripts.FocusScript(Page, txtnMulta.ClientID);
                }
                else if (control.ID == "txtnMulta")
                {
                    FUNCOES.Scripts.FocusScript(Page, txtnJuros.ClientID);
                }
                else if (control.ID == "txtnJuros")
                {
                    if (Div_Tarifa.Visible)
                        FUNCOES.Scripts.FocusScript(Page, txtnTarifa.ClientID);
                    else
                        FUNCOES.Scripts.FocusScript(Page, txtnDesconto.ClientID);
                }
                else if (control.ID == "txtnTarifa")
                {
                    FUNCOES.Scripts.FocusScript(Page, txtnDesconto.ClientID);
                }
                else if (control.ID == "txtnDesconto")
                {
                    FUNCOES.Scripts.FocusScript(Page, txtnValorTotalRec.ClientID);
                }
            }

            ValorTotal = ValorRecebido + ValorMulta + ValorJuros + ValorTarifa - ValorDesconto;

            txtnValorTotalRec.Text = ValorTotal.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
        }

        protected void ddlidConta_Info_Rec_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtnValorTotalRec_TextChanged(sender, e);
            FUNCOES.Scripts.FocusScript(Page, txtnValorRecebimento_Info_Rec.ClientID);
        }

        protected void ddlidFormaRecebimento_Info_Rec_SelectedIndexChanged(object sender, EventArgs e)
        {
            var btnIncluir = FindControl("cmdRecebimento_Incluir") as Button;
            if (btnIncluir != null)
            {
                FUNCOES.Scripts.FocusScript(Page, btnIncluir.ClientID);
            }
            FUNCOES.Scripts.FocusScript(Page, txtnValorRecebimento_Info_Rec.ClientID);

            if (ddlidFormaRecebimento_Info_Rec.SelectedValue == "1")
            {
                div_valor_recebimento.Visible = false;
                div_devolucao.Visible = true;
                FUNCOES.Popula_Combo(ddlCreditoDevolucao, "sp_Select 'Flow_Contas_Receber_Credito', @sidParceiro=" + hddidParceiro.Value, "idRegistro", "nValorCredito", false, "Selecione o Crédito disponível ", "0");

            }
            else
            {
                div_devolucao.Visible = false;
                div_valor_recebimento.Visible = true;
                txtnValorRecebimento_Info_Rec.Text = "";
                txtnValorTotalRec.Text = "";

                txtnValorRecebimento_Info_Rec.ReadOnly = false;
                //txtnValorTotalRec.ReadOnly = false;
            }

            AtualizarVisibilidadeTarifa();
            txtnValorTotalRec_TextChanged(sender, e);
            txtnValorTotalRec.ReadOnly = true;

        }
        #endregion

        #region | Envio Arquivos
        void AbrirModal_EnvioArquivo(eBloco bloco, int idLinha, string sTituloModal)
        {
            lblEnviarArquivos_Titulo.Text = sTituloModal;
            txtEnviarArquivo_sDscArquivo.Text = "";
            hddIdLinha.Value = idLinha.ToString();
            hddsBloco.Value = bloco.ToString();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_AbrirModalUploadArquivos", "$('#UploadArquivos_Modal').modal('show');", true);

        }

        protected void cmdEnviarArquivos_Click(object sender, EventArgs e)
        {
            int index;
            int idLinha = Convert.ToInt32(hddIdLinha.Value);
            eBloco bloco = (eBloco)Enum.Parse(typeof(eBloco), hddsBloco.Value);
            string sJSExecutar = "";

            if (fu_EnviarArquivo.HasFile)
            {
                Byte[] lObjArquivo = null;
                try
                {

                    TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                    lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(fu_EnviarArquivo.FileName, fu_EnviarArquivo.PostedFile.InputStream);

                    switch (bloco)
                    {
                        case eBloco.Recebimento:
                            index = bs_Recebimento.FindIndex(x => x.idLinha.Equals(idLinha));
                            bs_Recebimento[index].idArquivo = 0;
                            bs_Recebimento[index].sNomeArquivo = fu_EnviarArquivo.FileName;
                            bs_Recebimento[index].objArquivo = lObjArquivo;
                            bs_Recebimento[index].sObservacaoArquivo = txtEnviarArquivo_sDscArquivo.Text;
                            dtgRecebimento_DataBind();
                            sJSExecutar = " $('#aba_ContasReceber').tab('show'); window.location.hash = '#ctl00_cphCorpo_Div_Recebimento';";
                            break;
                    }
                }
                catch
                {
                    return;
                }
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ModalUpload_Fecha", "$('body').removeClass('modal-open').find('.modal-backdrop').remove();" + sJSExecutar, true);
        }

        void Efetuar_Download_Arquivo(eBloco bloco, int idLinha)
        {
            int index;
            int idArquivo = 0;
            string sNomeArquivo = "";
            byte[] bObjArquivo = null;
            string urlAtualPagina = Request.UrlReferrer.ToString().Replace(Request.RawUrl, "/Download/");

            switch (bloco)
            {
                case eBloco.Recebimento:
                    index = bs_Recebimento.FindIndex(x => x.idLinha.Equals(idLinha));
                    sNomeArquivo = bs_Recebimento[index].sNomeArquivo;
                    bObjArquivo = bs_Recebimento[index].objArquivo;
                    idArquivo = bs_Recebimento[index].idArquivo;
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

                dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);
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

        private enum eBloco
        {
            Recebimento = 1,
        }
        #endregion

        #region | Aba Documentos 
        void Popular_Aba_Arquivos(string idContasReceber)
        {
            string sPermissao = "S";
            if (!FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasReceber.IncluiArquivo))
            {
                sPermissao = "N";
            }
            frmArquivos.Attributes.Add("src", string.Format("~/app/Paginas/Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}&sPermiteNovoArquivo={2}", idContasReceber, "ContasReceber", sPermissao));
            aba_Arquivo.Visible = true;
        }
        #endregion

        #region | Lançamentos 
        bool Salvar_LancamentoRec(string idContasReceber)
        {
            bool bRetorno = false;

            try
            {
                foreach (var LancamentoRec_Linha in bs_LancamentoRec)
                {
                    if (LancamentoRec_Linha.sFuncao != "SEM ALTERAÇÃO")
                    {
                        Dictionary<String, String> vParametroItensLancamentoRec = new Dictionary<string, string>();

                        vParametroItensLancamentoRec["@idRegistro"] = LancamentoRec_Linha.idRegistroLancamentoRec.ToString();
                        vParametroItensLancamentoRec["@sFuncao"] = LancamentoRec_Linha.sFuncao;
                        vParametroItensLancamentoRec["@idContasReceber"] = idContasReceber;

                        vParametroItensLancamentoRec["dtLancamentoRec"] = LancamentoRec_Linha.dtLancamentoRec.ToString();
                        vParametroItensLancamentoRec["@nParcelaLancamentoRec"] = LancamentoRec_Linha.nParcelaLancamentoRec.ToString();
                        vParametroItensLancamentoRec["@nValorLancamentoRec"] = LancamentoRec_Linha.nValorLancamentoRec.ToString().Replace(",", ".");
                        vParametroItensLancamentoRec["@idFormaPagamentoLancamentoRec"] = LancamentoRec_Linha.idFormaPagamentoLancamentoRec.ToString();
                        vParametroItensLancamentoRec["@sDscFormaPagamentoLancamentoRec"] = LancamentoRec_Linha.idFormaPagamentoLancamentoRec.ToString();
                        BD.ExecutarDataSet(sProcedure, vParametroItensLancamentoRec);
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

            DIV_Lancamentos.Visible = false;
            Div_Recebimento.Visible = true;

            return bRetorno;

        }

        protected void txtnParcelas_TextChanged(object sender, EventArgs e)
        {
            IncluirLancamento();
            FUNCOES.Scripts.FocusScript(Page, dtgLancamento.Rows[0].Cells[4].Controls[1].ClientID);
        }

        protected void IncluirLancamento()
        {
            if (ddlidMeioRecebimento.SelectedValue != "0")
            {
                bs_LancamentoRec.Clear();
                DIV_Lancamentos.Visible = true;
                string sMensagem = "";
                int numeroParcelas = 1;


                if (txtnParcelas.Text != "")
                {
                    numeroParcelas = Convert.ToInt32(txtnParcelas.Text);
                }

                decimal valorTotal = ConverterStringDecimal(txtnValorOriginal.Text);
                decimal valorParcela = Math.Round(valorTotal / numeroParcelas, 2);
                decimal valorParcelaUltima = valorTotal - (valorParcela * (numeroParcelas - 1));

                decimal ValorBruto = ConverterStringDecimal(txtnValorBruto.Text);
                decimal valorParcelaBruto = Math.Round(ValorBruto / numeroParcelas, 2);
                decimal valorParcelaUltimaBruto = ValorBruto - (valorParcelaBruto * (numeroParcelas - 1));

                for (int i = 0; i < numeroParcelas; i++)
                {
                    FrameWork.cls_LancamentoRec objItem = new FrameWork.cls_LancamentoRec();

                    string[] vidContasReceber = hddidContasReceber.Value.Split(',');
                    string idContasReceber = vidContasReceber[0].ToString();

                    objItem.idLinha = bs_LancamentoRec.Count() + 1;
                    objItem.sFuncao = "INSERIR_LANCAMENTO";

                    objItem.idFormaPagamentoLancamentoRec = Convert.ToInt32(ddlidFormaRecebimento.SelectedValue);

                    objItem.idContasReceber = Convert.ToInt32(idContasReceber);

                    if (i == numeroParcelas - 1)
                    {
                        objItem.nValorLancamentoRec = valorParcelaUltima;
                        objItem.nValorBrutoLancamentoRec = valorParcelaUltimaBruto;
                    }
                    else
                    {
                        objItem.nValorLancamentoRec = valorParcela;
                        objItem.nValorBrutoLancamentoRec = valorParcelaBruto;
                    }

                    int proximaParcela = bs_LancamentoRec.Count + 1;
                    objItem.nParcelaLancamentoRec = proximaParcela;

                    bs_LancamentoRec.Add(objItem);

                }


                dtgLancamento_DataBind();
                LimpaCampos_Lancamento();

                RegistraScript("$('[id$=txtdtLancamentoRec]').focus();");
            }
        }

        void Lancamentos_Salvar()
        {
            int nContador = 0;
            foreach (GridViewRow item in dtgLancamento.Rows)
            {
                TextBox txtnValorLancamentoRec_Linha = (TextBox)item.FindControl("txtnValorLancamentoRec");
                if (string.IsNullOrEmpty(txtnValorLancamentoRec_Linha.Text) || txtnValorLancamentoRec_Linha.Text == "0,00")
                {
                    bs_LancamentoRec[nContador].nValorLancamentoRec = 0.00m;
                }
                else
                {
                    bs_LancamentoRec[nContador].nValorLancamentoRec = Convert.ToDecimal(txtnValorLancamentoRec_Linha.Text);
                }

                TextBox txtdtLancamentoRec_Linha = (TextBox)item.FindControl("txtdtLancamentoRec");
                bs_LancamentoRec[nContador].dtLancamentoRec = txtdtLancamentoRec_Linha.Text;

                DropDownList ddlidFormaPagamentoLancamentoRec_Linha = (DropDownList)item.FindControl("ddlidFormaPagamentoLancamentoRec");
                bs_LancamentoRec[nContador].sDscFormaPagamentoLancamentoRec = ddlidFormaPagamentoLancamentoRec_Linha.SelectedItem.ToString();


                nContador++;
            }

        }

        protected void dtgLancamento_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            bs_LancamentoRec.RemoveAt(index);
            dtgLancamento_DataBind();
        }

        private bool ValidarDados_Lancamento(ref string sMensagem)
        {
            bool bRetorno = true;

            string sMensagemErro = "";

            foreach (GridViewRow item in dtgLancamento.Rows)
            {
                TextBox txtdtLancamentoRec_Linha = (TextBox)item.FindControl("txtdtLancamentoRec");

                if (txtdtLancamentoRec_Linha.Text.Length < 3)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data de Vencimento Inválida";
                }


            }


            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina_Recebimento.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        void LimpaCampos_Lancamento()
        {

        }

        void Popular_dtgLancamento(DataSet dsPesquisa)
        {
            foreach (DataRow row in dsPesquisa.Tables[3].Rows)
            {
                FrameWork.cls_LancamentoRec objItem = new FrameWork.cls_LancamentoRec();
                objItem.idContasReceber = Convert.ToInt32(row["idContasReceber"].ToString());

                objItem.dtLancamentoRec = row["dtLancamentoRec"].ToString();
                objItem.nParcelaLancamentoRec = Convert.ToInt32(row["nParcelaLancamentoRec"].ToString());

                bs_LancamentoRec.Add(objItem);
            }
            dtgLancamento_DataBind();
            LimpaCampos_Lancamento();
        }

        protected void dtgLancamento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlidFormaPagamentoLancamentoRec = (e.Row.FindControl("ddlidFormaPagamentoLancamentoRec") as DropDownList);
                FUNCOES.Popula_Combo(ddlidFormaPagamentoLancamentoRec, "sp_Select 'tbl_Flow_CondicaodePagamento_Tipo'", "idTipoCondicaoPagamento", "sDscTipoCondicaoPagamento", false, "Forma de Recebimento", "0");
                ddlidFormaPagamentoLancamentoRec.SelectedValue = bs_LancamentoRec[e.Row.RowIndex].idFormaPagamentoLancamentoRec.ToString();
            }
            GRID.EsconderColunas(e, 0);
        }

        void dtgLancamento_DataBind()
        {
            dtgLancamento.DataSource = bs_LancamentoRec;
            dtgLancamento.DataBind();
        }
        #endregion

        protected void btnAlterarVencimento_Click(object sender, EventArgs e)
        {
            Pesquisar(hddidContasReceber.Value, true);
            // Pesquisa_Parceiros.ConfigurarControles(true);
            if (hddidOPI.Value == "")
            {
                txtsCodigo.ReadOnly = false;
                txtdtEmissao.ReadOnly = false;
                txtsDocumento.ReadOnly = false;
                txtnValorOriginal.ReadOnly = false;
                txtnValorBruto.ReadOnly = false;
                txtnParcelas.ReadOnly = false;
                //ddlidMeioRecebimento.Attributes.Remove("disabled");
                //ddlidContabil.Attributes.Remove("disabled");
                ddlidEmpresa.Attributes.Remove("disabled");

            }

            txtdtVencimento.ReadOnly = false;
            txtsObservacaoGeral.ReadOnly = false;
            ddlidCategoriaReceber.Attributes.Remove("disabled");
            ddlidFormaRecebimento.Attributes.Remove("disabled");
            ddlidCentroDeCusto.Attributes.Remove("disabled");
            ddlContaBancaria.Attributes.Remove("disabled");

            btnAlterarVencimento.Visible = false;

            Atualizar_ContaBancaria();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtdtEmissao]').focus();", true);
        }

        protected void BtnExcluirParcela_Click(object sender, EventArgs e)
        {

            try
            {
                if (int.TryParse(hddidContasReceber.Value, out int idContasReceber))
                {
                    DataSet dsExcluir;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    dsExcluir = BD.ExecutarDataSet(sProcedure, vParametros);

                    vParametros.Add("@sFuncao", "EXCLUIR");
                    vParametros.Add("@idContasReceber", idContasReceber.ToString());


                    dsExcluir = BD.ExecutarDataSet(sProcedure, vParametros);

                    MensagemPagina.MostraMensagem_Sucesso("Registro excluído com sucesso");

                }
                else
                {
                    throw new Exception("ID inválido");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
            Response.Redirect("ContasReceber.aspx");
            RegistraScript("");


        }

        protected void ddlidCentroDeCusto_SelectedIndexChanged(object sender, EventArgs e)
        {
            Atualizar_ContaBancaria();

            RegistraScript("");
        }

        private bool FormaRecebimento_ExigeContaBancaria()
        {
            int idFormaRecebimento = 0;

            if (!int.TryParse(ddlidFormaRecebimento.SelectedValue, out idFormaRecebimento))
            {
                return false;
            }

            if (!Enum.IsDefined(typeof(EnumTiposPagamentos), idFormaRecebimento))
            {
                return false;
            }

            EnumTiposPagamentos tipoPagamento = (EnumTiposPagamentos)idFormaRecebimento;

            bool bExigeContaBancaria =
                tipoPagamento == EnumTiposPagamentos.BoletoBancario
                || tipoPagamento == EnumTiposPagamentos.CreditoEmConta
                || tipoPagamento == EnumTiposPagamentos.Pix
                || tipoPagamento == EnumTiposPagamentos.Transferencia;

            return bExigeContaBancaria;
        }

        private void Atualizar_ContaBancaria()
        {
            bool bExibirContaBancaria = FormaRecebimento_ExigeContaBancaria();

            Div_idContaBancaria.Visible = bExibirContaBancaria;

            if (!bExibirContaBancaria)
            {
                Limpar_ContaBancaria();
            }
        }

        private void Limpar_ContaBancaria()
        {
            if (ddlContaBancaria.Items.FindByValue("0") != null)
            {
                ddlContaBancaria.SelectedValue = "0";
            }
            else
            {
                ddlContaBancaria.ClearSelection();
            }
        }

        protected void ddlidFormaRecebimento_SelectedIndexChanged(object sender, EventArgs e)
        {
            Atualizar_ContaBancaria();

            FUNCOES.Scripts.FocusScript(Page, ddlidFormaRecebimento.ClientID);

            RegistraScript("");
        }

        protected void BtnReabrirTitulo_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(hddidContasReceber.Value, out int idContasReceber))
                {

                    DataSet dsReabrir;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    dsReabrir = BD.ExecutarDataSet(sProcedure, vParametros);

                    vParametros.Add("@sFuncao", "REABRIR");
                    vParametros.Add("@idContasReceber", idContasReceber.ToString());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());

                    dsReabrir = BD.ExecutarDataSet(sProcedure, vParametros);
                    MensagemPagina.MostraMensagem_Sucesso("Registro Reaberto com sucesso");
                }
                else
                {
                    throw new Exception("ID inválido");
                }
                string urlReabertuta = string.Format("/app/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}", hddidContasReceber.Value);
                Response.Redirect(urlReabertuta);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
            RegistraScript("");
        }

        protected void BtnSalvarObservacao_Click(object sender, EventArgs e)
        {

            try
            {
                if (int.TryParse(hddidContasReceber.Value, out int idContasReceber))
                {

                    DataSet dsOBS;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    dsOBS = BD.ExecutarDataSet(sProcedure, vParametros);

                    vParametros.Add("@sFuncao", "SALVAR_OBSERVACAO");
                    vParametros.Add("@idContasReceber", idContasReceber.ToString());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    vParametros.Add("@sAnotacao", txtsAnotacao.Text);

                    dsOBS = BD.ExecutarDataSet(sProcedure, vParametros);
                    MensagemPagina.MostraMensagem_Sucesso("Observação salva com sucesso!");
                }
                else
                {
                    throw new Exception("ID inválido");
                }
                string urlOBS = string.Format("/app/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}", hddidContasReceber.Value);
                Response.Redirect(urlOBS);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "$('#observacaoModal').modal('hide');", true);
        }

        protected void BtnAdicionarObservacao_Click(object sender, EventArgs e)
        {

            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "$('#observacaoModal').modal('show');", true);

        }

        protected void txtnValorBruto_TextChanged(object sender, EventArgs e)
        {
            if (txtnValorOriginal.Text == "")
            {
                txtnValorOriginal.Text = txtnValorBruto.Text;
            }
            FUNCOES.Scripts.FocusScript(Page, txtsCodigo.ClientID);

        }

        protected void cmdFaturamento_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina("App/Paginas/Adm/Faturamento/Faturamento_Detalhe.aspx?id=" + hddidOPI.Value);
        }

        protected void btnSalvarStatus_Click(object sender, EventArgs e)
        {
            if (ValidaDadosModal())
            {                
                Dictionary<String, String> vParametros = new Dictionary<string, string>();             
                vParametros.Add("@idContasReceber", hddidContasReceber.Value);
                vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                vParametros.Add("@sAnotacao", txtsObservacaoStatus.Text);

                if (hddsTituloAdiantado.Value == "S")
                {
                    vParametros.Add("@sFuncao", "SALVAR_STATUS_ADIANTAMENTO");
                    //vParametros.Add("@sStatus", txtsNovoStatus.Text);
                }
                else
                {
                    vParametros.Add("@sFuncao", "SALVAR_STATUS");                
                    vParametros.Add("@sStatus", ddlsAlteraStatus.SelectedValue);
                }
                
                DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);
                string url = "/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id=" + hddidContasReceber.Value;
                Response.Redirect(url);
            }
            else
            {                
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalStatus", "$('#modalAlterarStatus').modal('show');", true);
            }
        }

        protected void btnAbrirModalStatus_Click(object sender, EventArgs e)
        {
            if (hddsTituloAdiantado.Value == "S")
            {
                div_ddlNovoStatus.Visible = false;
                div_txtNovoStatus.Visible = true;
                lblsObservacao.Text = "Motivo";
            }
            else
            {
                div_ddlNovoStatus.Visible = true;
                div_txtNovoStatus.Visible = false;
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalAlterarStatus", "$('#modalAlterarStatus').modal('show');", true);
        }

        protected void btnFecharModalStatus_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalAlterarStatus", "$('#modalAlterarStatus').modal('hide');", true);
        }
               
    }
}
