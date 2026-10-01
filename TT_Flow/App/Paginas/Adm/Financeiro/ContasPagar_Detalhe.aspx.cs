using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
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
using System.Text;
using System.IO;
using System.Globalization;
using TT_Flow.App.Controles;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using System.Web;

namespace TT_Flow.App.Paginas.Adm.Financeiro
{
    public partial class ContasPagar_Detalhe : System.Web.UI.Page
    {

        string sTituloPagina = "Conta a Pagar";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_Contas_Pagar";

        #region | Classes

        public List<FrameWork.cls_Pagamento> bs_Pagamento
        {
            get
            {
                if (ViewState["bs_Pagamento"] == null)
                {
                    ViewState["bs_Pagamento"] = new List<FrameWork.cls_Pagamento>();
                }
                return (List<cls_Pagamento>)ViewState["bs_Pagamento"];
            }

            set
            {
                ViewState["bs_Pagamento"] = value;
            }

        }


        public List<FrameWork.cls_LancamentoPag> bs_LancamentoPag
        {

            get
            {
                if (ViewState["bs_LancamentoPag"] == null)
                {
                    ViewState["bs_LancamentoPag"] = new List<FrameWork.cls_LancamentoPag>();
                }
                return (List<cls_LancamentoPag>)ViewState["bs_LancamentoPag"];
            }

            set
            {
                ViewState["bs_LancamentoPag"] = value;
            }

        }

        #endregion

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "ManualdoUsuarioContasPagar.pdf";
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            Controle_CategoriasCC.CentroDeCusto.TextChanged += new EventHandler(ddlidCentroDeCusto_TextChanged);

            ControleCategoriasModal.CentroDeCusto.TextChanged += new EventHandler(ControleCategoriasModal_CentroDeCusto_TextChanged);

            txtidContasPagar.ReadOnly = true;
            ddlidContabil.Attributes.Add("disabled", "disabled");


            if (!IsPostBack)
            {
                Div_Parceiro_Imposto.Visible = false;
                Div_Parceiro_Interno.Visible = false;
                Div_Parceiro_Cliente.Visible = false;
                Div_idBancoCredo.Visible = false;
                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasPagar.Consultar, true);
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasPagar.Incluir, true);
                    Pesquisar("0", true);

                }
                ddlidMeioPagamento_SelectedIndexChanged(objSender, objEventArgs);
                ddlidCentroDeCusto_TextChanged(objSender, objEventArgs);
                ddlidCategoriaPagar_TextChanged(objSender, objEventArgs);
                txtnValorPagamento_Info_Pag_TextChanged(objSender, objEventArgs);
                ddlidFormaPagamento_SelectedIndexChanged(objSender, objEventArgs);
                ddlidCompensacao_SelectedIndexChanged(objSender, objEventArgs);
                ddlidFormaPagamento_Info_Pag_SelectedIndexChanged(objSender, objEventArgs);

                ddlidParceiroImposto_SelectedIndexChanged(objSender, objEventArgs);
                ddlidParceiroInterno_SelectedIndexChanged(objSender, objEventArgs);
                ddlidParceiroCliente_SelectedIndexChanged(objSender, objEventArgs);
                ddlidBancoCredor_SelectedIndexChanged(objSender, objEventArgs);

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
                    Salvar_ContasPagar();
                }
                else if (requestTarget == "funcao_Editar")
                {
                    Pesquisar(hddidContasPagar.Value, true);
                }
                else if (requestTarget == "funcao_EXCLUIR")
                {
                    BtnExcluirParcela_Click(hddidContasPagar.Value, EventArgs.Empty);
                }
                else if (requestTarget == "funcao_Reabrir")
                {
                    BtnReabrirTitulo_Click(hddidContasPagar.Value, EventArgs.Empty);
                }


                AlterarVisibilidade();


            }

            ControleCategoriasModal.AlteraTamanhoCampos(4, 4);
            RegistraScript("");
            Div_sAdiantado.Visible = false;

        }

        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar(string idContasPagar, bool bEdicao)
        {


            if (!FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasPagar.Excluir))
            {
                BtnExcluirParcela.Visible = false;
            }
            PopularCombos();
            aba_Historico.Visible = false;
            Aba_Log.Visible = false;
            aba_Arquivo.Visible = false;
            Div_dtApuracao.Visible = false;
            Div_dtVencimento.Visible = false;
            Div_nValorOriginal.Visible = true;
            Div_nParcelas.Visible = false;
            txtnSaldo.ReadOnly = true;
            txtnValorTotalPag.ReadOnly = true;
            txtnTotal.ReadOnly = true;
            DIV_DADOS_COLABORADOR.Visible = false;
            DIV_Dados_Dependentes.Visible = false;
            DIV_Adiantamento.Visible = false;
            Div_linkcompensacao.Visible = false;
            string sErro = "";
            txtsDiaSemana.ReadOnly = true;
            Div_dtPrevisaoPagamento.Visible = false;
            Div_sDiaSemana.Visible = false;
            Div_ComplementoMeioPag.Visible = false;

            Div_Saldo.Visible = false;
            DivTotal.Visible = false;
            txtnTotal.ReadOnly = true;

            Div_Compensacao.Visible = false;
            BtnAdicionarObservacao.Visible = false;


            try
            {
                LimpaCampos();

                if (idContasPagar != "0")
                {
                    object objSender = new object();
                    EventArgs objEventArgs = new EventArgs();

                    ddlidCategoriaPagar_TextChanged(objSender, objEventArgs);

                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idContasPagar", idContasPagar);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    BtnAdicionarObservacao.Visible = true;
                    Div_nParcelas.Visible = false;

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        StatusDosCampos(true);
                        string sDscTipoStatus = RETORNO.DATASET(dsPesquisa, 0, "sStatus");
                        lblsDscTipoStatus.Text = sDscTipoStatus;
                        lblsDscTipoStatus.CssClass = string.Format("label label-{0}", RETORNO.DATASET(dsPesquisa, 0, "sCor"));

                        string sDscStatusAdiantado = RETORNO.DATASET(dsPesquisa, 0, "sStatusAdiantado");
                        lblsStatusAdiantado.Text = sDscStatusAdiantado;
                        lblsStatusAdiantado.CssClass = string.Format("label label-{0}", RETORNO.DATASET(dsPesquisa, 0, "sCor"));

                        hddidContasPagar.Value = RETORNO.DATASET(dsPesquisa, 0, "idContasPagar");
                        txtidContasPagar.Text = RETORNO.DATASET(dsPesquisa, 0, "idContasPagar");

                        txtsCodigo.Text = RETORNO.DATASET(dsPesquisa, 0, "sCodigo");

                        var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtEmissao").ToString());
                        txtdtEmissao.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');

                        var dt2 = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtVencimento").ToString());
                        txtdtVencimento.Text = dt2.ToString(@"yyyy/MM/dd").Replace('/', '-');

                        var dt3 = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtApuracao").ToString());
                        txtdtApuracao.Text = dt3.ToString(@"yyyy/MM/dd").Replace('/', '-');

                        var dt4 = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtPrevisaoPagamento").ToString());
                        txtdtPrevisaoPagamento.Text = dt4.ToString(@"yyyy/MM/dd").Replace('/', '-');

                        txtsDiaSemana.Text = RETORNO.DATASET(dsPesquisa, 0, "sDiaSemana");

                        txtsDocumento.Text = RETORNO.DATASET(dsPesquisa, 0, "sDocumento");
                        txtnValorOriginal.Text = RETORNO.DATASET(dsPesquisa, 0, "nValorOriginal");
                        txtnSaldo.Text = RETORNO.DATASET(dsPesquisa, 0, "nSaldo");
                        ddlidContabil.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idContabil");
                        Controle_CategoriasCC.CentroDeCusto.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCentroDeCusto");

                        if (Controle_CategoriasCC.CentroDeCusto.SelectedValue != "0")
                            Controle_CategoriasCC.CarregarCategorias(Controle_CategoriasCC.CentroDeCusto.SelectedValue);

                        Controle_CategoriasCC.CategoriaCC.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idRegistroCategoria");

                        ddlidCompensacao.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCompensacao");
                        ddlidCompensacao.Items.Remove(ddlidCompensacao.Items.FindByValue(RETORNO.DATASET(dsPesquisa, 0, "idContasPagar")));



                        string Compensacao = RETORNO.DATASET(dsPesquisa, 0, "idCompensacao");

                        hddlidCompensaca.Value = Compensacao;
                        if (Compensacao != "0")
                        {
                            Popular_linkcompensacao(dsPesquisa);

                        }

                        ddlidFormaPagamento.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idFormaPagamento");
                        string FormaPagamento = ddlidFormaPagamento.SelectedValue;
                        if (FormaPagamento == "1")
                        {
                            Div_ValorPago.Visible = false;
                            Div_Multa.Visible = false;
                            Div_Juros.Visible = false;
                            lblValorDesconto.Text = "Valor Compensado";
                            txtnDesconto.Text = txtnValorOriginal.Text;
                            Div_Compensacao.Visible = true;

                        }
                        else
                        {
                            lblValorDesconto.Text = "Valor Desconto";
                            Div_Compensacao.Visible = false;
                        }

                        ddlidEmpresa.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idEmpresa");
                        hddidEmpresa.Value = ddlidEmpresa.SelectedValue;
                        txtnValorBruto.Text = RETORNO.DATASET(dsPesquisa, 0, "nValorBruto");
                        txtnTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "nTotal");

                        txtsAdiantado.Text = RETORNO.DATASET(dsPesquisa, 0, "sAdiantado");
                        ViewState["sAdiantado"] = txtsAdiantado.Text;
                        if (txtsAdiantado.Text != "S")
                        {

                        }
                        else
                        {
                            AlterarVisibilidade();
                        }


                        ViewState["idParceiro"] = Convert.ToInt32(RETORNO.DATASET(dsPesquisa, 0, "idParceiro"));
                        ViewState["idColaborador"] = Convert.ToInt32(RETORNO.DATASET(dsPesquisa, 0, "idColaborador"));

                        txtsQuantidadeParcela.Text = RETORNO.DATASET(dsPesquisa, 0, "sQuantidadeParcela");
                        txtnParcelas.Text = RETORNO.DATASET(dsPesquisa, 0, "nParcelas");
                        ddlidMeioPagamento.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idMeioPagamento");

                        string idCategoriaTipoDesejado = RETORNO.DATASET(dsPesquisa, 0, "idCategoriaTipo");

                        // Popular o DropDownList com todas as categorias
                        FUNCOES.Popula_Combo(ddlidCategoriaPagar, "sp_Select 'Flow_Adm_Contas_Pagar_Categoria'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Selecione a Categoria", "0");

                        // Remover itens que não correspondem ao tipo de categoria desejado
                        for (int i = ddlidCategoriaPagar.Items.Count - 1; i >= 0; i--)
                        {
                            string idCategoriaAtual = ddlidCategoriaPagar.Items[i].Value;
                            if (!CategoriaPertenceAoTipo(idCategoriaAtual, idCategoriaTipoDesejado))
                            {
                                ddlidCategoriaPagar.Items.Remove(ddlidCategoriaPagar.Items[i]);
                            }
                        }

                        // Definir o valor selecionado
                        ddlidCategoriaPagar.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCategoriaPagar");
                        string sCategoriaTipo = ddlidCategoriaPagar.SelectedItem.Text;

                        if (txtsAdiantado.Text == "S")
                        {
                            ddlidBancoCredor.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idContaCredora");
                        }
                        else
                        {
                            if (sCategoriaTipo.StartsWith("Impostos -"))
                            {
                                ddlidParceiroImposto.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idParceiro");
                                Div_Parceiro_Imposto.Visible = true;
                            }
                            else if (sCategoriaTipo.StartsWith("Internos -"))
                            {
                                ddlidParceiroInterno.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idColaborador");
                                Div_Parceiro_Interno.Visible = true;
                            }
                            else
                            {
                                ddlidParceiroCliente.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idParceiro");
                                Div_Parceiro_Cliente.Visible = true;
                            }
                        }
                        txtsObservacaoGeral.Text = RETORNO.DATASET(dsPesquisa, 0, "sObservacaoGeral");
                        txtsComplementoFormaPag.Text = RETORNO.DATASET(dsPesquisa, 0, "sComplementoFormaPag");

                        hddidAprovacao.Value = RETORNO.DATASET(dsPesquisa, 0, "idAprovado");

                        txtidColaborador.Text = RETORNO.DATASET(dsPesquisa, 0, "idColaborador");

                        if (RETORNO.DATASET(dsPesquisa, 0, "idColaborador") != "0")
                        {
                            DIV_DADOS_COLABORADOR.Visible = true;
                            Div_idColaborador.Visible = false;

                            txtsBanco.ReadOnly = true;
                            txtsAgenciaBancaria.ReadOnly = true;
                            txtsContaBancaria.ReadOnly = true;
                            ddlsTipoConta.Attributes.Add("disabled", "disabled");
                            ddlsDependentesConvenio.Attributes.Add("disabled", "disabled");

                            ddlsTipoConta.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sTipoConta");
                            txtsBanco.Text = RETORNO.DATASET(dsPesquisa, 0, "sBanco");
                            txtsAgenciaBancaria.Text = RETORNO.DATASET(dsPesquisa, 0, "sAgenciaBancaria");
                            txtsContaBancaria.Text = RETORNO.DATASET(dsPesquisa, 0, "sContaBancaria");
                            ddlsDependentesConvenio.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sDependentesConvenio");

                            if (ddlsDependentesConvenio.SelectedValue == "S")
                            {
                                DIV_Dados_Dependentes.Visible = false;
                                Popular_dtgDependentes(dsPesquisa);
                            }
                        }

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));

                        CarregarLancamentosCC(idContasPagar);

                        string dtVencimento = RETORNO.DATASET(dsPesquisa, 0, "dtVencimento");
                        string sCredor = RETORNO.DATASET(dsPesquisa, 0, "sDscCredor");

                        lblTituloPagina.Text = string.Format("Conta a Pagar - {0} - Vencimento: {1}", sCredor, dtVencimento);

                        BreadCrumb.TitulodaPagina = string.Format("Conta a Pagar Detalhe - {0}", RETORNO.DATASET(dsPesquisa, 0, "idContasPagar"));

                        lblTituloSalvar.Text = "Confirma a Alteração da " + lblTituloPagina.Text + "?";
                        lblTituloExcluir.Text = "Confirma a Exclusão do registro " + lblTituloPagina.Text + "?";
                        lblTituloReabrir.Text = "Confirma a reabertura de " + lblTituloPagina.Text + "? Todos os pagamentos serão deletados!";

                        //Popular_Aba_Historico(dsPesquisa);
                        Popular_dtgPagamento(dsPesquisa);
                        Popular_Aba_Arquivos(idContasPagar);
                        Popular_Aba_Log(dsPesquisa);
                        Popular_dtgAdiantamento(dsPesquisa);


                        Div_dtVencimento.Visible = true;
                        Div_nValorOriginal.Visible = true;
                        Div_dtPrevisaoPagamento.Visible = true;
                        Div_sDiaSemana.Visible = true;
                        Div_Pagamento.Visible = false;
                        Div_sQuantidadeParcela.Visible = true;
                        DIV_Lancamentos.Visible = false;
                        Div_ComplementoMeioPag.Visible = true;
                        Div_Saldo.Visible = true;
                        DivTotal.Visible = true;

                        if (!FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasPagar.EfetuarPagamento))
                        {
                            btnAlterarVencimento.Visible = false;
                            cmdSalvar.Visible = false;
                            Div_CadastroPagamento.Visible = false;
                            dtgPagamento.Columns[10].Visible = false;
                        }

                        if (txtnSaldo.Text != "0,00")
                        {
                            BtnReabrirTitulo.Visible = false;
                        }
                        if (txtnSaldo.Text == "0,00")
                        {
                            if (!FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasPagar.Reabrir))
                            {
                                BtnReabrirTitulo.Visible = false;
                            }

                            btnAlterarVencimento.Visible = false;
                            cmdSalvar.Visible = false;
                            BtnAdicionarObservacao.Visible = true;
                            Div_CadastroPagamento.Visible = false;
                            txtdtPagamento_Info_Pag.ReadOnly = true;
                            txtsObservacaoGeral.ReadOnly = true;
                            txtsComplementoFormaPag.ReadOnly = false;
                            ddlidCategoriaPagar.Attributes.Add("disabled", "disabled");
                            Div_CadastroPagamento.Visible = false;
                            txtsComplementoFormaPag.ReadOnly = true;

                            //Div Pagamentos
                            Div_Pagamento.Visible = true;
                            Div_CadastroPagamento.Visible = false;
                            Div_BotaoIncluirPag.Visible = false;

                        }
                        else
                        {
                            Div_Pagamento.Visible = true;

                        }

                        if (RETORNO.DATASET(dsPesquisa, 0, "idRelatorioSeguro") != "0")
                        {
                            BtnReabrirTitulo.Visible = false;
                            btnAlterarVencimento.Visible = false;
                        }

                        //-------------------------------------------------------------
                        //Agnes Partal - 01/07/2024                         

                        foreach (DataRow id in dsPesquisa.Tables[1].Rows)
                        {
                            int idConciliacao = id["idConciliacao"] == null ? 0 : Convert.ToInt32(id["idConciliacao"]);

                            if (idConciliacao != 0)
                            {
                                BtnReabrirTitulo.Visible = false;
                                div_sStatusConciliado.Visible = true;
                                lblsStatusConciliado.Text = "Conciliado";
                                lblsStatusConciliado.CssClass = string.Format("label label-{0}", "info");
                            }
                        }
                        //-------------------------------------------------------------

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtdtPagamento_Info_Pag]').focus();", true);
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }

                }
                else
                {

                    BtnReabrirTitulo.Visible = false;
                    BtnExcluirParcela.Visible = false;
                    BreadCrumb.TitulodaPagina = string.Format("Nova {0}", sTituloPagina);
                    lblTituloPagina.Text = string.Format("Nova {0}", sTituloPagina);
                    txtidContasPagar.Text = "Novo";
                    lblTituloSalvar.Text = "Confirma a Inclusão da Conta a Pagar?";
                    cmdSalvar.Text = "Incluir";
                    Div_Pagamento.Visible = false;
                    btnAlterarVencimento.Visible = false;
                    BtnExcluirParcela.Visible = false;

                    btnAbrirModalCC.Visible = true;

                    DivTotal.Visible = false;
                    Div_Saldo.Visible = false;


                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtdtEmissao]').focus();", true);

                }

                PopularCombos_Compensacao();
                RegistraScript("");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }

        private bool CategoriaPertenceAoTipo(string idCategoria, string idCategoriaTipoDesejado)
        {
            // Consulta para obter o tipo da categoria atual
            string sql = $"SELECT idCategoriaTipo FROM tbl_Flow_Adm_Contas_Pagar_Categoria WHERE idCategoriaPagar = {idCategoria}";
            using (SqlDataReader dr = BD.ExecutarDataReader(sql))
            {
                if (dr.Read())
                {
                    return dr["idCategoriaTipo"].ToString() == idCategoriaTipoDesejado;
                }
            }
            return false;
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
            txtsQuantidadeParcela.ReadOnly = bStatus;

            txtdtApuracao.ReadOnly = bStatus;
            txtsObservacaoGeral.ReadOnly = bStatus;
            txtsComplementoFormaPag.ReadOnly = bStatus;
            txtsComplementoFormaPag.ReadOnly = bStatus;

            txtdtPrevisaoPagamento.ReadOnly = bStatus;



            string sStatusCombo = "disabled";
            if (!bStatus)
            {
                sStatusCombo = "enabled";
            }
            ddlidContabil.Attributes.Remove("disabled");
            ddlidContabil.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlidEmpresa.Attributes.Remove("disabled");
            ddlidEmpresa.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlidFormaPagamento.Attributes.Remove("disabled");
            ddlidFormaPagamento.Attributes.Add(sStatusCombo, sStatusCombo);

            Controle_CategoriasCC.CentroDeCusto.Attributes.Add(sStatusCombo, sStatusCombo);
            Controle_CategoriasCC.CategoriaCC.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlidMeioPagamento.Attributes.Remove("disabled");
            ddlidMeioPagamento.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlidCategoriaPagar.Attributes.Remove("disabled");
            ddlidCategoriaPagar.Attributes.Add(sStatusCombo, sStatusCombo);



            txtnParcelas.ReadOnly = false;

        }

        void Salvar_ContasPagar()
        {
            string sErro = "";
            string sMensagem = "";

            if (ValidarDados())
            {
                if (ValidarDados_Lancamento(ref sMensagem))
                {

                    int numeroParcelas = 0;

                    bool usandoRateio = bs_LancamentosCC.Any(l => l.sFuncao != "EXCLUIR");

                    if (txtnParcelas.Text != "")
                    {
                        numeroParcelas = Convert.ToInt32(txtnParcelas.Text);
                    }

                    if (txtidContasPagar.Text != "Novo")
                    {
                        if (ValidarDadosVencimento())
                        {
                            numeroParcelas = 1;

                            try
                            {
                                string[] vidContasPagar = hddidContasPagar.Value.Split(',');
                                string idContasPagar = vidContasPagar[0].ToString();

                                DataSet dsSalvar;
                                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                                vParametros.Add("@sFuncao", "SALVAR");
                                vParametros.Add("@idContasPagar", idContasPagar);
                                vParametros.Add("@nSaldo", BD.Conversoes.Numerico(txtnSaldo));
                                vParametros.Add("@dtVencimento", txtdtVencimento.Text);
                                vParametros.Add("@sQuantidadeParcela", txtsQuantidadeParcela.Text);
                                vParametros.Add("@sCodigo", txtsCodigo.Text);
                                vParametros.Add("@dtEmissao", txtdtEmissao.Text.Replace("-", "/"));
                                vParametros.Add("@sDocumento", txtsDocumento.Text);
                                vParametros.Add("@nValorOriginal", BD.Conversoes.Numerico(txtnValorOriginal));
                                vParametros.Add("@idContabil", ddlidContabil.SelectedValue);


                                // --- LÓGICA APLICADA AQUI (EDIÇÃO) ---
                                if (usandoRateio)
                                {
                                    vParametros.Add("@idCentroDeCusto", "0");
                                    vParametros.Add("@idRegistroCategoria", "0");
                                }
                                else
                                {
                                    vParametros.Add("@idCentroDeCusto", Controle_CategoriasCC.CentroDeCusto.SelectedValue);
                                    vParametros.Add("@idRegistroCategoria", Controle_CategoriasCC.CategoriaCC.SelectedValue);
                                }


                                vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                                vParametros.Add("@idFormaPagamento", ddlidFormaPagamento.SelectedValue);
                                //vParametros.Add("@idCompensacao", ddlidCompensacao.SelectedValue);

                                vParametros.Add("@idMeioPagamento", ddlidMeioPagamento.SelectedValue);
                                vParametros.Add("@nParcelas", txtnParcelas.Text);
                                vParametros.Add("@nValorBruto", BD.Conversoes.Numerico(txtnValorBruto));
                                vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                                vParametros.Add("@dtPrevisaoPagamento", txtdtPrevisaoPagamento.Text);
                                vParametros.Add("@sDiaSemana", txtsDiaSemana.Text);

                                vParametros.Add("@idCategoriaPagar", ddlidCategoriaPagar.SelectedValue);
                                vParametros.Add("@dtApuracao", txtdtApuracao.Text);
                                vParametros.Add("@sObservacaoGeral", txtsObservacaoGeral.Text);
                                vParametros.Add("@sComplementoFormaPag", txtsComplementoFormaPag.Text);


                                string sCategoriaTipo = ddlidCategoriaPagar.SelectedItem.Text;
                                if (sCategoriaTipo.StartsWith("Impostos -"))
                                {
                                    AtualizarCredorImposto();
                                }

                                dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);



                                if (BD.ValidarDataSet(dsSalvar, out sErro))
                                {
                                    idContasPagar = RETORNO.DATASET(dsSalvar, 0, "idContasPagar");


                                    //if (!usandoRateio)
                                    //{
                                    //    Controle_CategoriasCC.AtualizarSaldoCategoria(txtnValorOriginal);
                                    //}
                                    //else
                                    {
                                        if (!SalvarLancamentosCC(idContasPagar))
                                        {
                                            MensagemPagina.MostraMensagem_Erro("Erro ao Salvar Lançamentos do Centro de Custo.");
                                        }
                                    }


                                    // O restante da sua lógica de sucesso...
                                    if (Salvar_Pagamento(idContasPagar))
                                    {
                                        Session["SalvoComSucesso"] = true;
                                        Response.Redirect(Request.RawUrl);
                                    }

                                    else
                                    {
                                        throw new Exception("Erro ao Salvar Pagamento:");
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

                            string idContasPagar_Primeira = "";
                            string[] vidContasPagar = hddidContasPagar.Value.Split(',');
                            string idContasPagar = vidContasPagar[0].ToString();

                            int totalPagamentos = numeroParcelas;

                            if (numeroParcelas == 0)
                            {
                                numeroParcelas = 1;
                            }
                            if (ValidarGridLancamento(numeroParcelas))
                            {
                                var idContasPagar_ParcelaAtual = "";

                                for (int numeroParcelaAtual = 1; numeroParcelaAtual <= numeroParcelas; numeroParcelaAtual++)
                                {
                                    DataSet dsSalvar;
                                    Dictionary<String, String> vParametros = new Dictionary<string, string>();


                                    TextBox txtdtLancamentoPag = (TextBox)dtgLancamento.Rows[numeroParcelaAtual - 1].FindControl("txtdtLancamentoPag");
                                    var dtLancamentoPag = Convert.ToDateTime(txtdtLancamentoPag.Text);

                                    TextBox txtnValorLancamentoPag = (TextBox)dtgLancamento.Rows[numeroParcelaAtual - 1].FindControl("txtnValorLancamentoPag");
                                    decimal valorParcela = Convert.ToDecimal(txtnValorLancamentoPag.Text);

                                    TextBox txtnValorBrutoLancamentoPag = (TextBox)dtgLancamento.Rows[numeroParcelaAtual - 1].FindControl("txtnValorBrutoLancamentoPag");
                                    decimal valorParcelaBruto = txtnValorBrutoLancamentoPag.Text == "" ? 0 : Convert.ToDecimal(txtnValorBrutoLancamentoPag.Text);

                                    decimal valorTotalTitulo = Convert.ToDecimal(txtnValorOriginal.Text);

                                    vParametros.Add("@sFuncao", "SALVAR");

                                    vParametros.Add("@nSaldo", BD.Conversoes.Numerico(valorParcela));
                                    vParametros.Add("@nValorOriginal", BD.Conversoes.Numerico(valorParcela));
                                    vParametros.Add("@dtVencimento", dtLancamentoPag.ToString());
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


                                    if (usandoRateio)
                                    {
                                        vParametros.Add("@idCentroDeCusto", "0");
                                        vParametros.Add("@idRegistroCategoria", "0");
                                    }
                                    else
                                    {
                                        vParametros.Add("@idCentroDeCusto", Controle_CategoriasCC.CentroDeCusto.SelectedValue);
                                        vParametros.Add("@idRegistroCategoria", Controle_CategoriasCC.CategoriaCC.SelectedValue);
                                    }

                                    vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                                    vParametros.Add("@idMeioPagamento", ddlidMeioPagamento.SelectedValue);
                                    vParametros.Add("@nParcelas", txtnParcelas.Text);
                                    vParametros.Add("@idFormaPagamento", ddlidFormaPagamento.SelectedValue);

                                    string sCategoriaTipo = ddlidCategoriaPagar.SelectedItem.Text;
                                    if (sCategoriaTipo.StartsWith("Impostos -"))
                                    {
                                        vParametros.Add("@idParceiro", ddlidParceiroImposto.SelectedValue);
                                    }
                                    else if (sCategoriaTipo.StartsWith("Internos -"))
                                    {
                                        vParametros.Add("@idColaborador", ddlidParceiroInterno.SelectedValue);
                                    }
                                    else
                                    {
                                        vParametros.Add("@idParceiro", ddlidParceiroCliente.SelectedValue);
                                    }


                                    vParametros.Add("@idCategoriaPagar", ddlidCategoriaPagar.SelectedValue);
                                    vParametros.Add("@dtApuracao", txtdtApuracao.Text.Replace("-", "/"));
                                    vParametros.Add("@sObservacaoGeral", txtsObservacaoGeral.Text);
                                    vParametros.Add("@sComplementoFormaPag", txtsComplementoFormaPag.Text);

                                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);
                                   
                                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                                    {
                                        idContasPagar_ParcelaAtual = RETORNO.DATASET(dsSalvar, 0, "idContasPagar");

                                        txtnSaldo.Text = RETORNO.DATASET(dsSalvar, 0, "nSaldo");                    //Agnes Partal * 05/08/2024
                                        txtdtVencimento.Text = RETORNO.DATASET(dsSalvar, 0, "dtVencimento");        //Agnes Partal * 05/08/2024

                                        if (numeroParcelaAtual == 1)
                                        {
                                            idContasPagar_Primeira = RETORNO.DATASET(dsSalvar, 0, "idContasPagar");
                                        }

                                        if (usandoRateio)
                                        {
                                            if (!SalvarLancamentosCC(idContasPagar_ParcelaAtual, totalPagamentos, valorTotalTitulo, valorParcela))
                                            {
                                                MensagemPagina.MostraMensagem_Erro("Erro ao Salvar Lançamentos do Rateio para a parcela " + numeroParcelaAtual);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            TextBox txt = new TextBox();
                                            txt.Text = valorParcela.ToString();
                                            Controle_CategoriasCC.AtualizarSaldoCategoria(txt);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("BD: " + sErro.ToString());
                                    }


                                }
                                if (sErro == "")
                                {

                                    txtsCodigo.ReadOnly = true;
                                    txtdtEmissao.ReadOnly = true;
                                    txtdtVencimento.ReadOnly = true;
                                    txtsDocumento.ReadOnly = true;
                                    txtnValorOriginal.ReadOnly = true;
                                    txtnValorBruto.ReadOnly = true;
                                    txtnParcelas.ReadOnly = true;
                                    Div_dtVencimento.Visible = true;
                                    Div_nValorOriginal.Visible = true;
                                    Div_Saldo.Visible = false;
                                    txtsQuantidadeParcela.ReadOnly = true;
                                    txtdtPrevisaoPagamento.ReadOnly = true;

                                    txtdtApuracao.ReadOnly = true;
                                    txtsObservacaoGeral.ReadOnly = true;
                                    txtsComplementoFormaPag.ReadOnly = true;
                                    txtsComplementoFormaPag.ReadOnly = true;
                                    ddlidCategoriaPagar.Attributes.Add("disabled", "disabled");

                                    ddlidContabil.Attributes.Add("disabled", "disabled");
                                    ddlidEmpresa.Attributes.Add("disabled", "disabled");
                                    ddlidFormaPagamento.Attributes.Add("disabled", "disabled");

                                    //Thiago Rodrigues - 03/09/2025
                                    Controle_CategoriasCC.CentroDeCusto.Attributes.Add("disabled", "disabled");
                                    Controle_CategoriasCC.CategoriaCC.Attributes.Add("disabled", "disabled");
                                    btnAbrirModalCC.Visible = false;

                                    ddlidMeioPagamento.Attributes.Add("disabled", "disabled");


                                    ddlidParceiroImposto.Attributes.Add("disabled", "disabled");
                                    ddlidParceiroInterno.Attributes.Add("disabled", "disabled");
                                    ddlidParceiroCliente.Attributes.Add("disabled", "disabled");

                                    string urlNovo = "/app/Paginas/Adm/Financeiro/ContasPagar_Detalhe.aspx?id=0";
                                    string urlDetalhes = string.Format("/app/Paginas/Adm/Financeiro/ContasPagar_Detalhe.aspx?id={0}", idContasPagar_Primeira);

                                    string mensagemSucesso = string.Format("Lançamentos registrados com sucesso! <br/>" +
                                        "<a href='{1}'>Clique aqui para incluir uma nova {0}</a> " +
                                        "ou <a href='{2}'>Clique aqui para ver os detalhes.</a>", sTituloPagina, urlNovo, urlDetalhes);


                                    MensagemPagina.MostraMensagem_Sucesso(mensagemSucesso);

                                    DIV_Lancamentos.Visible = false;

                                }
                            }
                            else
                            {
                                MensagemPagina_Pagamento.MostraMensagem_Erro("Soma dos valores das Parcelas deve ser igual ao valor do título");
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
                TextBox txtnValorLancamentoPag = (TextBox)dtgLancamento.Rows[numeroParcelaAtual - 1].FindControl("txtnValorLancamentoPag");
                decimal valorParcela = txtnValorLancamentoPag.Text == "" ? 0 : Convert.ToDecimal(txtnValorLancamentoPag.Text);
                nSomaValorLiquido += valorParcela;

                TextBox txtnValorBrutoLancamentoPag = (TextBox)dtgLancamento.Rows[numeroParcelaAtual - 1].FindControl("txtnValorBrutoLancamentoPag");
                decimal valorParcelaBruto = txtnValorBrutoLancamentoPag.Text == "" ? 0 : Convert.ToDecimal(txtnValorBrutoLancamentoPag.Text);
                nSomaValorBruto += valorParcelaBruto;

            }

            decimal nValorBruto = txtnValorBruto.Text == "" ? 0 : Convert.ToDecimal(txtnValorBruto.Text);
            decimal nValorLiquido = Convert.ToDecimal(txtnValorOriginal.Text);

            if (Math.Round(nSomaValorBruto, 2) != Math.Round(nValorBruto, 2) || Math.Round(nSomaValorLiquido, 2) != Math.Round(nValorLiquido, 2))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        void AtualizarCredorImposto()
        {
            string sErro = "";
            try
            {
                string sidContasPagar = hddidContasPagar.Value;
                DataSet dsCredor;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CredorImposto");
                vParametros.Add("@idParceiro", ddlidParceiroImposto.SelectedValue);
                vParametros.Add("@idContasPagar", sidContasPagar);
                dsCredor = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", vParametros);

                if (BD.ValidarDataSet(dsCredor, out sErro))
                {

                }
                else
                {
                    throw new Exception(sErro);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);

            }

        }


        #endregion

        #region | Limpar Campos
        void LimpaCampos()
        {
            hddidContasPagar.Value = "0";
            txtidContasPagar.Text = "Novo";

            txtsCodigo.Text = "";
            txtdtEmissao.Text = DateTime.Today.ToString("u").Substring(0, 10);
            txtdtVencimento.Text = "";
            txtsDocumento.Text = "";
            txtnValorOriginal.Text = "";
            txtnValorBruto.Text = "";
            txtnSaldo.Text = "";
            ddlidContabil.SelectedValue = "0";
            Controle_CategoriasCC.CentroDeCusto.SelectedValue = "0";
            Controle_CategoriasCC.CategoriaCC.SelectedValue = "0";

            ddlidEmpresa.SelectedValue = "0";
            hddidEmpresa.Value = "0";
            ddlidMeioPagamento.SelectedValue = "0";
            txtnParcelas.Text = "";
            ddlidFormaPagamento.SelectedValue = "0";
            //ddlidCompensacao.SelectedValue = "0";
            hddlidCompensaca.Value = "0";

            txtdtPrevisaoPagamento.Text = "";
            txtsDiaSemana.Text = "";

            txtdtApuracao.Text = "";
            txtsObservacaoGeral.Text = "";
            ddlidCategoriaPagar.SelectedValue = "0";
            txtsComplementoFormaPag.Text = "";

            bs_Pagamento.Clear();

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
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data de vencimento inválida!";
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

            if (bs_LancamentosCC.Any(l => l.sFuncao != "EXCLUIR"))
            {
                decimal valorTotalTitulo = 0;
                decimal.TryParse(txtnValorOriginal.Text, out valorTotalTitulo);

                decimal valorTotalRateado = bs_LancamentosCC.Where(l => l.sFuncao != "EXCLUIR").Sum(l => l.nValor);

                //if (Math.Abs(valorTotalTitulo - valorTotalRateado) > 0.01m)
                //{
                //    sMensagemErro += $"A soma dos Centros de Custos Divididos (R$ {valorTotalRateado:N2}) não corresponde ao Valor Líquido do título (R$ {valorTotalTitulo:N2}). Ajuste os valores nos Centros de Custos Divididos antes de salvar.</br>";
                //}

                if (valorTotalTitulo != valorTotalRateado)
                {
                    sMensagemErro += $"A soma dos Centros de Custos Divididos (R$ {valorTotalRateado:N2}) não corresponde ao Valor Líquido do título (R$ {valorTotalTitulo:N2}). Ajuste os valores antes de salvar.</br>";
                }
            }

            if (string.IsNullOrEmpty(ViewState["sAdiantado"] as string) || ViewState["sAdiantado"].ToString() == "N")
            {

                decimal nValorPagamento_Info_Pag = 0;
                decimal ValorOriginal = 0;
                decimal nValorComDesconto = 0;
                decimal nValorAoLiquidar = 0;


                if (decimal.TryParse(txtnValorOriginal.Text, out ValorOriginal))
                {
                    foreach (var linha in bs_Pagamento)
                    {
                        nValorPagamento_Info_Pag += (linha.nValorPagamento_Info_Pag);
                    }

                    if (nValorPagamento_Info_Pag > ValorOriginal)
                    {
                        decimal diferenca = nValorPagamento_Info_Pag - ValorOriginal;
                        sMensagemErro += string.Format("O valor inserido excede o valor Liquido. A diferença é de {0:C}.", diferenca) + "</br>";
                    }
                }
                nValorPagamento_Info_Pag = 0;
                if (decimal.TryParse(txtnValorOriginal.Text, out ValorOriginal))
                {

                    foreach (var linha in bs_Pagamento)
                    {
                        nValorComDesconto += (linha.nValorPagamento_Info_Pag + linha.nDesconto);
                    }

                    if (nValorComDesconto > ValorOriginal)
                    {
                        decimal diferenca = nValorComDesconto - ValorOriginal;
                        sMensagemErro += string.Format("A soma dos valores Pagos e Descontos não batem com o Valor Liquido. A diferença é de {0:C}.", diferenca) + "</br>";
                    }
                }


                if (ddlidCategoriaPagar.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Categoria!";
                }
                //Data emissão
                if (!Validacoes.ValidarData(txtdtEmissao))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data Emissão inválida!";
                }

                //if(txtidContasPagar.Text != "Novo")
                //{ 
                //    if (!Validacoes.ValidarData(txtdtPrevisaoPagamento))
                //    {
                //        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data da Previsão inválida!";
                //    }
                //    //string diaSemana = txtsDiaSemana.Text.Trim().ToLower();
                //    //if (!(diaSemana.StartsWith("terça") || diaSemana.StartsWith("quinta")))
                //    //{
                //    //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O dia da semana deve ser terça ou quinta! Altere a Previsão";
                //    //}
                //}

                string sCategoriaTipo = ddlidCategoriaPagar.SelectedItem.Text;
                if (sCategoriaTipo.StartsWith("Compras -"))
                {

                    //Empresa 
                    if (ddlidEmpresa.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
                    }
                    //Cliente
                    if (ddlidParceiroCliente.SelectedValue == "0")
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
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Bruto não pode se menor que o valor Liquido";

                    }
                    //Nota Fiscal
                    if (txtsCodigo.Text.Length < 1)
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Nota Fiscal inválida!";
                    }
                    //Numero do Pedido
                    //if (txtsDocumento.Text.Length < 2)
                    //{
                    //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Número do pedido ínvalido!";
                    //}
                    //Forma de Pagamento
                    if (ddlidFormaPagamento.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma forma de Pagamento!";
                    }
                    //Tipo de Pagamento
                    if (ddlidMeioPagamento.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo de Pagamento!";
                    }
                }
                else if (sCategoriaTipo.StartsWith("Serviços -"))
                {

                    //Empresa
                    if (ddlidEmpresa.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
                    }
                    //Cliente
                    if (ddlidParceiroCliente.SelectedValue == "0")
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
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Bruto não pode se menor que o valor Liquido";
                    }
                    //Nota Fiscal
                    if (txtsCodigo.Text.Length < 1)
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Nota Fiscal inválida!";
                    }
                    //Forma de Pagamento
                    if (ddlidFormaPagamento.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma forma de Pagamento!";
                    }
                    //Meio de Pagamento
                    if (ddlidMeioPagamento.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo de Pagamento!";
                    }
                }
                else if (sCategoriaTipo.StartsWith("Impostos -"))
                {

                    if (ddlidParceiroImposto.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um credor!";
                    }
                    //Empresa
                    if (ddlidEmpresa.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
                    }
                    //Data de Apuração
                    if (!Validacoes.ValidarData(txtdtApuracao))
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data de Apuração inválida!";
                    }
                    //Valor Liquido
                    if (txtnValorOriginal.Text.Length < 3)
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Liquido Inválido!";
                    }
                    //Codigo Receita
                    if (txtsCodigo.Text.Length < 5)
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Código Receita ínvalido mínimo de 5 caracteres!";
                    }
                    //Forma de Pagamento
                    if (ddlidFormaPagamento.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma forma de Pagamento!";
                    }
                    //Meio de Pagamento
                    if (ddlidMeioPagamento.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo de Pagamento!";
                    }
                }
                else if (sCategoriaTipo.StartsWith("Internos -"))
                {

                    //Empresa
                    if (ddlidEmpresa.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
                    }

                    //Colaborador
                    if (ddlidParceiroInterno.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Colaborador!";
                    }

                    //Data de Apuração
                    if (!Validacoes.ValidarData(txtdtApuracao))
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data de Apuração inválida!";
                    }
                    //Valor Bruto
                    if (txtnValorBruto.Text.Length < 3)
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Bruto Inválido!";
                    }
                    //Valor Liquido
                    if (txtnValorOriginal.Text.Length < 3)
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Liquido ínvalido!";
                    }
                    decimal ValorLiquido = txtnValorOriginal.Text == "" ? 0 : Convert.ToDecimal(txtnValorOriginal.Text);
                    decimal ValorBruto = txtnValorBruto.Text == "" ? 0 : Convert.ToDecimal(txtnValorBruto.Text);
                    if (ValorBruto < ValorLiquido)
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Bruto não pode se menor que o valor Líquido";
                    }
                    //Referencia
                    if (txtsDocumento.Text.Length < 5) //Agnes Partal * 05/08/2024 - Correção solicitada via email 
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Referência ínvalida minimo de 5 caracteres!";
                    }
                    //Forma de Pagamento
                    if (ddlidFormaPagamento.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma forma de Pagamento!";
                    }
                    //Meio de Pagamento
                    if (ddlidMeioPagamento.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo de Pagamento!";
                    }
                }
                else if (sCategoriaTipo.StartsWith("Serviços /"))
                {

                    //Empresa
                    if (ddlidEmpresa.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
                    }
                    //Cliente
                    if (ddlidParceiroCliente.SelectedValue == "0")
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
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Bruto não pode se menor que o valor Liquido";
                    }
                    //Nota Fiscal
                    if (txtsCodigo.Text.Length < 1)
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Nota Fiscal inválida!";
                    }
                    //Forma de Pagamento
                    if (ddlidFormaPagamento.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma forma de Pagamento!";
                    }
                    //Meio de Pagamento
                    if (ddlidMeioPagamento.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo de Pagamento!";
                    }


                }
                else
                {

                    //Empresa
                    if (ddlidEmpresa.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
                    }
                    //Cliente
                    if (ddlidParceiroCliente.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Cliente!";
                    }
                    //Data de Apuração
                    if (!Validacoes.ValidarData(txtdtApuracao))
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data de Apuração inválida!";
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
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Bruto não pode se menor que o valor Liquido";

                    }
                    //Nota Fiscal
                    if (txtsCodigo.Text.Length < 1)
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Nota Fiscal inválida!";
                    }
                    //Numero do Pedido
                    //if (txtsDocumento.Text.Length < 2)
                    //{
                    //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Número do pedido ínvalido!";
                    //}
                    //Forma de Pagamento
                    if (ddlidFormaPagamento.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma forma de Pagamento!";
                    }
                    ////Centro de Custo
                    //if (ddlidCentroDeCusto.SelectedValue == "0")
                    //{
                    //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Centro de custo invalido!";
                    //}
                    //Tipo de Pagamento
                    if (ddlidMeioPagamento.SelectedValue == "0")
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo de Pagamento!";
                    }
                }

                if (sMensagemErro != "")
                {
                    bRetorno = false;

                    MensagemPagina.MostraMensagem_Erro(sMensagemErro);
                }
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

            sb.Append("$('[id*=txtnValorOriginal]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnSaldo]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnValorPago]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=nValorPagamento]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=nValorLancamentoPag]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnValorBruto]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnMulta]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnJuros]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnDesconto]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnTotal]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtModalValor]').mask('000.000.000.000.000,00', { reverse: true });");

            sb.Append("$('[id*=txtnTaxaJuros]').mask('00,00 %', { reverse: true });");
            sb.Append("$('[id*=txtnIOF]').mask('00,9999999 %', { reverse: true });");
            sb.Append("$('[id*=txtnIOF_Adicional]').mask('00,0099999 %', { reverse: true });");

            sb.Append("$('[id*=txtnValorOperacao]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnTotalTitulos]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnTotalIOF]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnTarifas]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnTotalLiberado]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnTotalDespesas]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnTitulosAberto]').mask('000.000.000.000.000,00', { reverse: true });");

            //sb.Append("$('[id*=txtsDscDocumentoCredor]').mask('000.000.000.000.000,00', { reverse: true });");



            sb.Append("$('[id*=txtnParcela]').mask('00#');");

            //if (ddlidFormaPagamento.SelectedItem.ToString() == "Boleto Bancário")
            //{
            //    sb.Append("$('[id*=txtsComplementoFormaPag]').mask('99999.99999 99999.999999 99999.999999 9 99999999999999');");
            //}


            sb.Append("});");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }
        #endregion

        #region | Combos/DDL
        void PopularCombos()
        {
            //Principal
            FUNCOES.Popula_Combo(ddlidContabil, "sp_Select 'Flow_CodigoContabil'", "idContabil", "sDscCodContabil", false, "Selecione o Código Contábil", "0");
            FUNCOES.Popula_Combo(Controle_CategoriasCC.CentroDeCusto, "sp_Select 'Flow_Adm_CentroDeCusto'", "idCentroDeCusto", "sDescricao", false, "Selecione um Centro de Custo", "0");
            FUNCOES.Popula_Combo(ddlidMeioPagamento, "sp_Select 'tbl_Flow_Adm_MeioPagamento'", "idMeioPagamento", "sDscMeioPagamento", false, "Selecione um Meio", "0");
            FUNCOES.Popula_Combo(ddlidFormaPagamento, "sp_Select 'tbl_Flow_CondicaodePagamento_Tipo'", "idTipoCondicaoPagamento", "sDscTipoCondicaoPagamento", false, "Selecione o Pagamento", "0");
            FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa'", "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
            FUNCOES.Popula_Combo(ddlidCategoriaPagar, "sp_Select 'Flow_Adm_Contas_Pagar_Categoria'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Selecione a Categoria", "0");


            FUNCOES.Popula_Combo(ddlidParceiroInterno, "sp_Select 'Flow_Credor_Colaboradores'", "idColaborador", "sDscColaborador", false, "Selecione o Colaborador", "0");
            FUNCOES.Popula_Combo(ddlidParceiroImposto, "sp_Select 'Flow_Parceiro_Credor_Impostos'", "idParceiro", "sRazaoSocial", false, "Selecione um credor", "0");
            FUNCOES.Popula_Combo(ddlidParceiroCliente, "sp_Select 'Flow_Parceiro_Credor_Cliente'", "idParceiro", "sRazaoSocial", false, "Selecione um credor", "0");
            FUNCOES.Popula_Combo(ddlidBancoCredor, "sp_Select 'tbl_Flow_Adm_ContasBancarias'", "idConta", "sDscConta", false, "Selecione uma Conta", "0");


            string compensacao = hddidContasPagar.Value;

            //if (compensacao != null && compensacao != "Novo")
            //{
            //    FUNCOES.Popula_Combo(ddlidCompensacao, "sp_Select 'flow_Compensacao_ja_Selecionadas'", "idContasPagar", "sDscCredor", false, "Selecione um Título", "0");
            //}
            //else
            //{
            //    FUNCOES.Popula_Combo(ddlidCompensacao, "sp_Select 'flow_Compensacao'", "idContasPagar", "sDscCredor", false, "Selecione um Título", "0");
            //}

            //Pagamento
            FUNCOES.Popula_Combo(ddlidFormaPagamento_Info_Pag, "sp_Select 'tbl_Flow_CondicaodePagamento_Tipo'", "idTipoCondicaoPagamento", "sDscTipoCondicaoPagamento", false, "Selecione o Pagamento", "0");
            FUNCOES.Popula_Combo(ddlidConta_Info_Pag, "sp_Select 'tbl_Flow_Adm_ContasBancarias'", "idConta", "sDscConta", false, "Selecione uma Conta", "0");
        }
        void PopularCombos_Compensacao()
        {

            string compensacao = hddidContasPagar.Value;

            if (compensacao != "0")
            {
                FUNCOES.Popula_Combo(ddlidCompensacao, "sp_Select 'flow_Compensacao_ja_Selecionadas'", "idContasPagar", "sDscCredor", false, "Selecione um Título", "0");
            }
            else
            {
                FUNCOES.Popula_Combo(ddlidCompensacao, "sp_Select 'flow_Compensacao'", "idContasPagar", "sDscCredor", false, "Selecione um Título", "0");
            }
            FUNCOES.Popula_Combo(ddlidCompensacao, "sp_Select 'flow_Compensacao'", "idContasPagar", "sDscCredor", false, "Selecione um Título", "0");
        }
        #endregion

        #region | Pagamento

        void Popular_dtgPagamento(DataSet dsPesquisa)
        {
            foreach (DataRow row in dsPesquisa.Tables[1].Rows)
            {
                FrameWork.cls_Pagamento objItem = new FrameWork.cls_Pagamento();

                objItem.idContasPagar = Convert.ToInt32(row["idContasPagar"].ToString());
                objItem.idRegistroPagamento = Convert.ToInt32(row["idRegistroPagamento"].ToString());

                objItem.idLinha = bs_Pagamento.Count() + 1;
                objItem.sFuncao = "SEM ALTERAÇÃO";

                objItem.nValorPagamento_Info_Pag = ConverterStringDecimal(row["nValorPagamento_Info_Pag"].ToString());
                objItem.dtPagamento_Info_Pag = row["dtPagamento_Info_Pag"].ToString();

                objItem.nNumeroParcela_Info_Pag = Convert.ToInt32(row["nNumeroParcela_Info_Pag"].ToString());
                objItem.idConta_Info_Pag = Convert.ToInt32(row["idConta_Info_Pag"].ToString());
                objItem.sDscConta_Info_Pag = row["sDscConta_Info_Pag"].ToString();
                objItem.dtAtualizacao = row["dtAtualizacao"].ToString();
                objItem.idFormaPagamento_Info_Pag = Convert.ToInt32(row["idFormaPagamento_Info_Pag"].ToString());
                objItem.sDscFormaPagamento_Info_Pag = row["sDscFormaPagamento_Info_Pag"].ToString();
                objItem.idCompensacao = Convert.ToInt32(row["idCompensacao"].ToString());


                objItem.idArquivo = Convert.ToInt32(row["idArquivo"].ToString());
                objItem.sNomeArquivo = row["sNomeArquivo"].ToString();

                objItem.nDesconto = ConverterStringDecimal(row["nDesconto"].ToString());
                objItem.nJuros = ConverterStringDecimal(row["nJuros"].ToString());
                objItem.nMulta = ConverterStringDecimal(row["nMulta"].ToString());
                objItem.nValorTotalPag = ConverterStringDecimal(row["nValorTotalPag"].ToString());
                objItem.sCor = row["sCor"].ToString();
                bs_Pagamento.Add(objItem);
            }

            BtnEdicaoPagamento.Visible = false;

            dtgPagamento_DataBind();
            LimpaCampos_Pagamento();
        }

        protected void cmdPagamento_Incluir_Click(object sender, EventArgs e)
        {
            string sMensagem = "";

            if (ValidarDados_Pagamento(ref sMensagem))
            {
                FrameWork.cls_Pagamento objItem = new FrameWork.cls_Pagamento();

                string[] vidContasPagar = hddidContasPagar.Value.Split(',');
                string idContasPagar = vidContasPagar[0].ToString();

                objItem.idLinha = bs_Pagamento.Count() + 1;
                objItem.sFuncao = "INSERIR_Pagamento";

                objItem.idContasPagar = Convert.ToInt32(idContasPagar);
                objItem.nValorPagamento_Info_Pag = ConverterStringDecimal(txtnValorPagamento_Info_Pag.Text);
                objItem.idConta_Info_Pag = Convert.ToInt32(ddlidConta_Info_Pag.SelectedValue);
                objItem.sDscConta_Info_Pag = ddlidConta_Info_Pag.SelectedItem.ToString();
                objItem.dtPagamento_Info_Pag = txtdtPagamento_Info_Pag.Text;

                objItem.idFormaPagamento_Info_Pag = Convert.ToInt32(ddlidFormaPagamento_Info_Pag.SelectedValue);
                objItem.sDscFormaPagamento_Info_Pag = ddlidFormaPagamento_Info_Pag.SelectedItem.ToString();

                objItem.idCompensacao = Convert.ToInt32(ddlidCompensacao.SelectedValue);



                objItem.nMulta = ConverterStringDecimal(txtnMulta.Text);
                objItem.nJuros = ConverterStringDecimal(txtnJuros.Text);
                objItem.nDesconto = ConverterStringDecimal(txtnDesconto.Text);
                objItem.nValorTotalPag = ConverterStringDecimal(txtnValorTotalPag.Text);

                int proximaParcela = bs_Pagamento.Count + 1;
                objItem.nNumeroParcela_Info_Pag = proximaParcela;

                objItem.sNomeArquivo = "";

                bs_Pagamento.Add(objItem);

                string compensacao = ddlidFormaPagamento_Info_Pag.SelectedValue;
                if (compensacao == "1")
                {
                    Div_CadastroPagamento.Visible = false;

                }

                dtgPagamento_DataBind();
                LimpaCampos_Pagamento();



            }
            else
            {
                MensagemPagina.MostraMensagem_Erro(sMensagem, false);
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtdtPagamento_Info_Pag]').focus();", true);

        }

        bool Salvar_Pagamento(string idContasPagar)
        {
            bool bRetorno = true;
            Lancamentos_Salvar();
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
                        Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                        Arquivo.vbArquivo = Pagamento_Linha.objArquivo;
                        Arquivo.dtExpiracaoDoc = "";

                        Pagamento_Linha.idArquivo = Convert.ToInt32(BD.Retorno.DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));

                        if (Pagamento_Linha.sFuncao == "SEM ALTERAÇÃO")
                        {
                            Pagamento_Linha.sFuncao = "INSERIR_Pagamento";
                        }
                    }

                    if (Pagamento_Linha.sFuncao != "SEM ALTERAÇÃO")
                    {
                        Dictionary<String, String> vParametroItensPagamento = new Dictionary<string, string>();

                        vParametroItensPagamento["@idRegistro"] = Pagamento_Linha.idRegistroPagamento.ToString();
                        vParametroItensPagamento["@sFuncao"] = Pagamento_Linha.sFuncao;
                        vParametroItensPagamento["@idContasPagar"] = idContasPagar;

                        vParametroItensPagamento["@nValorPagamento_Info_Pag"] = Pagamento_Linha.nValorPagamento_Info_Pag.ToString().Replace(",", ".");
                        vParametroItensPagamento["@idConta_Info_Pag"] = Pagamento_Linha.idConta_Info_Pag.ToString();
                        vParametroItensPagamento["@dtPagamento_Info_Pag"] = Pagamento_Linha.dtPagamento_Info_Pag.ToString();
                        vParametroItensPagamento["@nNumeroParcela_Info_Pag"] = Pagamento_Linha.nNumeroParcela_Info_Pag.ToString();
                        vParametroItensPagamento["@idFormaPagamento_Info_Pag"] = Pagamento_Linha.idFormaPagamento_Info_Pag.ToString();
                        vParametroItensPagamento["@sDscFormaPagamento_Info_Pag"] = Pagamento_Linha.sDscFormaPagamento_Info_Pag.ToString();
                        vParametroItensPagamento["@sDscConta_Info_Pag"] = Pagamento_Linha.sDscConta_Info_Pag.ToString();

                        vParametroItensPagamento["@idCompensacao"] = Pagamento_Linha.idCompensacao.ToString();


                        vParametroItensPagamento["@nMulta"] = Pagamento_Linha.nMulta.ToString().Replace(",", ".");
                        vParametroItensPagamento["@nJuros"] = Pagamento_Linha.nJuros.ToString().Replace(",", ".");
                        vParametroItensPagamento["@nDesconto"] = Pagamento_Linha.nDesconto.ToString().Replace(",", ".");
                        vParametroItensPagamento["@nValorTotalPag"] = Pagamento_Linha.nValorTotalPag.ToString().Replace(",", ".");

                        vParametroItensPagamento["@idArquivo"] = Pagamento_Linha.idArquivo.ToString();
                        vParametroItensPagamento["@idUsuarioAtualizacao"] = IDENTITY.Variaveis.idUsuario();
                        BD.ExecutarDataSet(sProcedure, vParametroItensPagamento);


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

        protected void BtnEdicaoPagamento_Click(object sender, EventArgs e)
        {
            int idLinha = Convert.ToInt32(hddPagamento_idLinha.Value);
            int index = bs_Pagamento.FindIndex(x => x.idLinha.Equals(idLinha));
            string sMensagem = "";
            if (ValidarDados_Pagamento(ref sMensagem))
            {
                decimal nValor = 0;
                decimal nValorMulta = 0;
                decimal nValorJuros = 0;
                decimal nValorDesconto = 0;
                decimal nValorTotalPag = 0;

                decimal.TryParse(txtnValorPagamento_Info_Pag.Text.Replace(".", ","), out nValor);
                bs_Pagamento[index].nValorPagamento_Info_Pag = nValor;
                bs_Pagamento[index].idConta_Info_Pag = Convert.ToInt32(ddlidConta_Info_Pag.SelectedValue);
                bs_Pagamento[index].dtPagamento_Info_Pag = txtdtPagamento_Info_Pag.Text;
                bs_Pagamento[index].idFormaPagamento_Info_Pag = Convert.ToInt32(ddlidFormaPagamento_Info_Pag.SelectedValue);
                bs_Pagamento[index].idCompensacao = Convert.ToInt32(ddlidCompensacao.SelectedValue);


                decimal.TryParse(txtnMulta.Text.Replace(".", ","), out nValorMulta);
                bs_Pagamento[index].nMulta = nValorMulta;
                decimal.TryParse(txtnJuros.Text.Replace(".", ","), out nValorJuros);
                bs_Pagamento[index].nJuros = nValorJuros;
                decimal.TryParse(txtnDesconto.Text.Replace(".", ","), out nValorDesconto);
                bs_Pagamento[index].nDesconto = nValorDesconto;
                decimal.TryParse(txtnValorTotalPag.Text.Replace(".", ","), out nValorTotalPag);
                bs_Pagamento[index].nValorTotalPag = nValorTotalPag;


                bs_Pagamento[index].sFuncao = "INSERIR_Pagamento";

                dtgPagamento_DataBind();
                LimpaCampos_Pagamento();
                RegistraScript("");

                Div_CadastroPagamento.Visible = false;



                Salvar_ContasPagar();
                Response.Redirect(Request.RawUrl);

            }
        }

        protected void dtgPagamento_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            int idLinha = Convert.ToInt32(dtgPagamento.Rows[e.RowIndex].Cells[0].Text);
            bs_Pagamento[bs_Pagamento.FindIndex(x => x.idLinha.Equals(idLinha))].sFuncao = "EXCLUIR_Pagamento";
            dtgPagamento_DataBind();


            Div_CadastroPagamento.Visible = true;
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();
            ddlidFormaPagamento_Info_Pag_SelectedIndexChanged(objSender, objEventArgs);
            Salvar_ContasPagar();
            Response.Redirect(Request.RawUrl);
        }

        protected void dtgPagamento_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            int nColunaBotoes = e.Row.Cells.Count - 1;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = DataBinder.Eval(e.Row.DataItem, "sNomeArquivo").ToString();
                string sCor = DataBinder.Eval(e.Row.DataItem, "sCor")?.ToString() ?? string.Empty;
                e.Row.CssClass = sCor;
                foreach (LinkButton lnk in e.Row.Cells[nColunaBotoes].Controls.OfType<LinkButton>())
                {
                    if ((lnk.ID == "lnkPagamento_Download" && sNomeArquivo == "") || (lnk.ID == "lnkPagamento_UpLoad" && sNomeArquivo != ""))
                    {
                        lnk.Visible = false;
                    }
                }
            }
            if (txtnSaldo.Text == "0,00")
            {
                GRID.EsconderColunas(e, 10);
            }
            string Pagamento = ddlidFormaPagamento_Info_Pag.SelectedValue;
            if (Pagamento == "1")
            {

                GRID.EsconderColunas(e, 3, 4, 5, 7);
            }



            GRID.EsconderColunas(e, 0);
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
                AbrirModal_EnvioArquivo(eBloco.Pagamento, idLinha, bs_Pagamento[bs_Pagamento.FindIndex(x => x.idLinha.Equals(idLinha))].nNumeroParcela_Info_Pag.ToString());
            }
            else if (e.CommandName == "Download_Arquivo")
            {
                Efetuar_Download_Arquivo(eBloco.Pagamento, idLinha);
            }
            else if (e.CommandName == "Editar")
            {
                Div_CadastroPagamento.Visible = true;

                cmdPagamento_Incluir.Visible = false;
                BtnEdicaoPagamento.Visible = true;

                var Pagamento = bs_Pagamento[bs_Pagamento.FindIndex(x => x.idLinha.Equals(idLinha))];

                txtnValorPagamento_Info_Pag.Text = Pagamento.nValorPagamento_Info_Pag.ToString();
                txtnMulta.Text = Pagamento.nMulta.ToString();
                txtnJuros.Text = Pagamento.nJuros.ToString();
                txtnDesconto.Text = Pagamento.nDesconto.ToString();
                txtnValorTotalPag.Text = Pagamento.nValorTotalPag.ToString();

                ddlidConta_Info_Pag.SelectedValue = Pagamento.idConta_Info_Pag.ToString();
                ddlidCompensacao.SelectedValue = Pagamento.idCompensacao.ToString();


                var dt3 = Convert.ToDateTime(Pagamento.dtPagamento_Info_Pag);
                txtdtPagamento_Info_Pag.Text = dt3.ToString(@"yyyy/MM/dd").Replace('/', '-');
                ddlidFormaPagamento_Info_Pag.SelectedValue = Pagamento.idFormaPagamento_Info_Pag.ToString();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtdtPagamento_Info_Pag]').focus();", true);


            }
            RegistraScript("");
        }

        private bool ValidarDados_Pagamento(ref string sMensagem)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (!Validacoes.ValidarData(txtdtPagamento_Info_Pag))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data de pagamento Inválida";
            }

            string FormaPagamento = ddlidFormaPagamento_Info_Pag.SelectedValue;
            if (FormaPagamento == "1")
            {
                if (txtnDesconto.Text.Length < 3)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Inválido";
                }

                if (ddlidCompensacao.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Título!";
                }
            }
            else
            {
                if (txtnValorPagamento_Info_Pag.Text.Length < 3)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Inválido";
                }
            }


            if (ddlidConta_Info_Pag.SelectedValue == "0")
            {
                if (sMensagemErro != "")
                {
                    sMensagemErro = sMensagemErro + "</br>";
                }
                sMensagemErro += "Selecione uma conta";
            }

            if (ddlidFormaPagamento_Info_Pag.SelectedValue == "0")
            {
                if (sMensagemErro != "")
                {
                    sMensagemErro = sMensagemErro + "</br>";
                }
                sMensagemErro += "Selecione uma forma de Pagamento!" + "</br>";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina_Pagamento.MostraMensagem_Erro(sMensagemErro);
            }


            return bRetorno;

        }

        void LimpaCampos_Pagamento()
        {

            txtnValorPagamento_Info_Pag.Text = "";
            ddlidConta_Info_Pag.SelectedValue = "0";
            txtdtPagamento_Info_Pag.Text = "";
            ddlidFormaPagamento_Info_Pag.SelectedValue = "0";
            txtnMulta.Text = "";
            txtnJuros.Text = "";
            txtnDesconto.Text = "";
            txtnValorTotalPag.Text = "";
            hddPagamento_idLinha.Value = "";
            ddlidCompensacao.SelectedValue = "0";
            BtnEdicaoPagamento.Visible = false;
            cmdPagamento_Incluir.Visible = true;
        }

        void dtgPagamento_DataBind()
        {
            try
            {
                dtgPagamento.DataSource = bs_Pagamento.Where(c => c.sFuncao.ToString() != "EXCLUIR_Pagamento");
                dtgPagamento.DataBind();
                RegistraScript("");
                GRID.SomarColunas(dtgPagamento, true, GRID.Formatação.Moeda, 3);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar Grid de Pagamento: " + ex.Message);
            }

        }

        //--------------------------------------------------------------------------------------------------------
        #endregion

        #region | ConsultasGrid
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

        void Popular_dtgDependentes(DataSet dsPesquisa)
        {
            gv_Dependentes.DataSource = dsPesquisa.Tables[3];
            gv_Dependentes.DataBind();
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

        void Popular_dtgAdiantamento(DataSet dsPesquisa)
        {
            txtnTaxaJuros.Text = RETORNO.DATASET(dsPesquisa, 5, 0, "nTaxaJuros");
            txtnIOF.Text = RETORNO.DATASET(dsPesquisa, 5, 0, "nIOF");
            txtnIOF_Adicional.Text = RETORNO.DATASET(dsPesquisa, 5, 0, "nIOFAdicional");

            txtnValorOperacao.Text = RETORNO.DATASET(dsPesquisa, 6, 0, "nValorOperacao");
            txtnTotalTitulos.Text = RETORNO.DATASET(dsPesquisa, 6, 0, "nTotalTitulos");
            txtnTotalJuros.Text = RETORNO.DATASET(dsPesquisa, 6, 0, "nTotalJuros");
            txtnTotalIOF.Text = RETORNO.DATASET(dsPesquisa, 6, 0, "nTotalIOF");
            txtnTarifas.Text = RETORNO.DATASET(dsPesquisa, 6, 0, "nTarifas");
            txtnTotalLiberado.Text = RETORNO.DATASET(dsPesquisa, 6, 0, "nTotalLiberado");
            txtnTotalDespesas.Text = RETORNO.DATASET(dsPesquisa, 6, 0, "nTotalDespesas");
            txtnTitulosAberto.Text = RETORNO.DATASET(dsPesquisa, 6, 0, "nTotalAberto");

            gv_Adiantamento.DataSource = dsPesquisa.Tables[4];
            gv_Adiantamento.DataBind();
            DIV_Adiantamento.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(gv_Adiantamento, dsPesquisa.Tables[4], 2, "desc", "false", "''"), true);    //Agnes Partal * 12/08/2024
            //GRID.SomarColunas(gv_Adiantamento, true, GRID.Formatação.Moeda, 3, 4, 5, 6, 7, 8, 9);
        }

        protected void gv_Adiantamento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }
        }

        void Popular_linkcompensacao(DataSet dsPesquisa)
        {
            gv_linkcompensacao.DataSource = dsPesquisa.Tables[0];
            gv_linkcompensacao.DataBind();
            Div_linkcompensacao.Visible = true;
        }

        protected void btnInformarPagamento_Click(object sender, EventArgs e)
        {
            //txtnValorPago.ReadOnly = false;
        }

        #endregion

        #region | Eventos 

        protected void ddlidMeioPagamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            int MeioPagamento = Convert.ToInt32(ddlidMeioPagamento.SelectedValue);

            if (MeioPagamento != 0)
            {
                var idContasPagar = hddidContasPagar.Value;
                Div_nParcelas.Visible = false;

                if (idContasPagar == "0")
                {
                    Div_sQuantidadeParcela.Visible = false;
                    if (ddlidMeioPagamento.SelectedValue != "2")
                    {
                        bs_LancamentoPag.Clear();
                        IncluirLancamento();
                        FUNCOES.Scripts.FocusScript(Page, dtgLancamento.Rows[0].Cells[4].Controls[1].ClientID);
                    }

                    if (ddlidMeioPagamento.SelectedValue == "2")
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

        protected void btnAlterarVencimento_Click(object sender, EventArgs e)
        {

            string sCategoriaTipo = ddlidCategoriaPagar.SelectedItem.Text;

            ViewState["sAdiantado"] = txtsAdiantado.Text;
            if (txtsAdiantado.Text != "S")
            {
                if (sCategoriaTipo.StartsWith("Internos -"))
                {
                    Div_Parceiro_Interno.Visible = true;
                    Div_Parceiro_Imposto.Visible = false;
                    Div_Parceiro_Cliente.Visible = false;

                    ddlidParceiroInterno.Attributes.Add("disabled", "disabled");
                }
                else if
                (sCategoriaTipo.StartsWith("Impostos -"))
                {
                    Div_Parceiro_Interno.Visible = false;
                    Div_Parceiro_Imposto.Visible = true;
                    Div_Parceiro_Cliente.Visible = false;

                    ddlidParceiroImposto.Attributes.Remove("disabled");

                }
                else
                {
                    Div_Parceiro_Interno.Visible = false;
                    Div_Parceiro_Imposto.Visible = false;
                    Div_Parceiro_Cliente.Visible = true;

                    ddlidParceiroCliente.Attributes.Add("disabled", "disabled");
                }

                Pesquisar(hddidContasPagar.Value, true);
                txtsCodigo.ReadOnly = false;
                txtdtEmissao.ReadOnly = false;
                txtdtVencimento.ReadOnly = false;
                txtsDocumento.ReadOnly = false;
                txtnValorOriginal.ReadOnly = false;
                txtnValorBruto.ReadOnly = false;
                txtnSaldo.ReadOnly = true;
                txtnParcelas.ReadOnly = false;
                ddlidFormaPagamento.Attributes.Remove("disabled");
                Controle_CategoriasCC.CentroDeCusto.Attributes.Remove("disabled");
                Controle_CategoriasCC.CategoriaCC.Attributes.Remove("disabled");
                btnAbrirModalCC.Visible = true;

                ddlidEmpresa.Attributes.Remove("disabled");
                ddlidCategoriaPagar.Attributes.Remove("disabled");
                txtsObservacaoGeral.ReadOnly = false;
                txtsComplementoFormaPag.ReadOnly = false;
                txtdtApuracao.ReadOnly = false;
                txtsComplementoFormaPag.ReadOnly = false;
                ddlidCategoriaPagar.Attributes.Remove("disabled");
                txtdtPrevisaoPagamento.ReadOnly = false;


                DIV_Adiantamento.Visible = false;
                Div_linkcompensacao.Visible = false;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtdtEmissao]').focus();", true);

            }
            else
            {
                txtsObservacaoGeral.ReadOnly = false;
                Controle_CategoriasCC.CentroDeCusto.Attributes.Remove("disabled");
                Controle_CategoriasCC.CategoriaCC.Attributes.Remove("disabled");
                btnAbrirModalCC.Visible = true;
                ddlidFormaPagamento.Attributes.Remove("disabled");
            }


        }

        protected void BtnExcluirParcela_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(hddidContasPagar.Value, out int idContasPagar))
                {
                    DataSet dsExcluir;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    
                    dsExcluir = BD.ExecutarDataSet(sProcedure, vParametros);
                    vParametros.Add("@sFuncao", "EXCLUIR");
                    vParametros.Add("@idContasPagar", idContasPagar.ToString());
                    vParametros.Add("@sAdiantado", txtsAdiantado.Text);

                    Controle_CategoriasCC.SAcao = "SOMAR";
                    Controle_CategoriasCC.AtualizarSaldoCategoria(txtnValorOriginal);

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
            Response.Redirect("ContasPagar.aspx");
            RegistraScript("");
        }

        protected void ddlidFormaPagamento_SelectedIndexChanged(object sender, EventArgs e)
        {


            lblsComplementoFormaPag.CssClass = "bold-label";

            switch (Convert.ToInt32(ddlidFormaPagamento.SelectedValue))
            {
                case 0: //Selecione

                    Div_ComplementoMeioPag.Visible = false;
                    break;

                case 1:

                    lblsComplementoFormaPag.Text = "Detalhe Compensação";
                    Div_ComplementoMeioPag.Visible = true;
                    break;

                case 2: //Pix
                    lblsComplementoFormaPag.Text = "Chave Pix";
                    Div_ComplementoMeioPag.Visible = true;
                    break;
                case 3: //Boleto Bancário

                    lblsComplementoFormaPag.Text = "N° Boleto";
                    Div_ComplementoMeioPag.Visible = true;
                    break;
                case 4: //Cartão de Crédito

                    lblsComplementoFormaPag.Text = "Detalhe Cartão de Crédito";
                    Div_ComplementoMeioPag.Visible = true;
                    break;

                case 5: //Transferência (TED/DOC)

                    lblsComplementoFormaPag.Text = "Detalhe Transferência";
                    Div_ComplementoMeioPag.Visible = true;
                    break;

                case 6: //Mercado Pago/Livre

                    lblsComplementoFormaPag.Text = "N° Mercado Pago";
                    Div_ComplementoMeioPag.Visible = true;
                    break;

                case 7: //Crédito em Conta

                    lblsComplementoFormaPag.Text = "Detalhe Crédito em Conta";
                    Div_ComplementoMeioPag.Visible = true;
                    break;

                case 8: //Outros

                    lblsComplementoFormaPag.Text = "Observação pagamento";
                    Div_ComplementoMeioPag.Visible = true;
                    break;


            }

            FUNCOES.Scripts.FocusScript(Page, txtsComplementoFormaPag.ClientID);

            ViewState["sAdiantado"] = txtsAdiantado.Text;
            if (txtsAdiantado.Text == "S")
            {
                Div_ComplementoMeioPag.Visible = false;
            }
        }

        protected void ddlidCentroDeCusto_TextChanged(object sender, EventArgs e)
        {
            FUNCOES.Scripts.FocusScript(Page, ddlidMeioPagamento.ClientID);
        }

        protected void ddlidCategoriaPagar_TextChanged(object sender, EventArgs e)
        {
            int idCategoriaTipo_Consulta = 0;
            try
            {
                DataSet dsContabil;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "Autoselecao_Contabil");
                vParametros.Add("@idCategoriaPagar", ddlidCategoriaPagar.SelectedValue);

                dsContabil = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", vParametros);

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

            string sCategoriaTipo = ddlidCategoriaPagar.SelectedItem.Text;

            lblNotaFiscal.CssClass = "bold-label";
            lblsDocumento.CssClass = "bold-label";

            switch (idCategoriaTipo_Consulta)
            {

                case 1: //Compras

                    Div_dtApuracao.Visible = false;
                    txtdtApuracao.Text = "";

                    Div_nValorBruto.Visible = true;
                    Div_CentroCusto.Visible = true;
                    Div_sDocumento.Visible = true;

                    lblNotaFiscal.Text = "Nota Fiscal";
                    lblsDocumento.Text = "Referência";

                    Div_Parceiro_Imposto.Visible = false;
                    ddlidParceiroImposto.SelectedValue = "0";

                    Div_Parceiro_Interno.Visible = false;
                    ddlidParceiroInterno.SelectedValue = "0";

                    Div_Parceiro_Cliente.Visible = true;


                    break;
                case 2: //Serviços

                    Div_dtApuracao.Visible = false;
                    txtdtApuracao.Text = "";

                    Div_nValorBruto.Visible = true;
                    Div_CentroCusto.Visible = true;
                    Div_sDocumento.Visible = true;

                    lblNotaFiscal.Text = "Nota Fiscal";
                    lblsDocumento.Text = "Referência";

                    Div_Parceiro_Imposto.Visible = false;
                    ddlidParceiroImposto.SelectedValue = "0";

                    Div_Parceiro_Interno.Visible = false;
                    ddlidParceiroInterno.SelectedValue = "0";

                    Div_Parceiro_Cliente.Visible = true;


                    break;
                case 3: //Impostos

                    txtnValorBruto.Text = "";
                    Controle_CategoriasCC.CentroDeCusto.SelectedValue = "0";
                    Controle_CategoriasCC.CategoriaCC.SelectedValue = "0";

                    Div_nValorBruto.Visible = false;
                    Div_CentroCusto.Visible = false;


                    Div_dtApuracao.Visible = true;
                    Div_sDocumento.Visible = true;

                    lblNotaFiscal.Text = "Código Receita";
                    lblsDocumento.Text = "Referência ";

                    Div_Parceiro_Imposto.Visible = true;

                    Div_Parceiro_Interno.Visible = false;
                    ddlidParceiroInterno.SelectedValue = "0";

                    Div_Parceiro_Cliente.Visible = false;
                    ddlidParceiroCliente.SelectedValue = "0";

                    break;
                case 4: //Internos


                    Div_sDocumento.Visible = true;

                    Div_dtApuracao.Visible = true;
                    Div_nValorBruto.Visible = true;
                    Div_CentroCusto.Visible = true;

                    lblNotaFiscal.Text = "Nota Fiscal";
                    lblsDocumento.Text = "Referência";

                    Div_Parceiro_Interno.Visible = true;

                    Div_Parceiro_Imposto.Visible = false;
                    ddlidParceiroImposto.SelectedValue = "0";

                    Div_Parceiro_Cliente.Visible = false;
                    ddlidParceiroCliente.SelectedValue = "0";

                    break;

                case 5: //Benefícios

                    Div_dtApuracao.Visible = true;
                    Div_nValorBruto.Visible = true;
                    Div_CentroCusto.Visible = true;
                    Div_sDocumento.Visible = true;


                    lblsDocumento.Text = "Referência ";

                    Div_Parceiro_Imposto.Visible = false;
                    ddlidParceiroImposto.SelectedValue = "0";

                    Div_Parceiro_Interno.Visible = false;
                    ddlidParceiroInterno.SelectedValue = "0";

                    Div_Parceiro_Cliente.Visible = true;
                    break;

                case 6: //Operações Financeiras

                    Div_dtApuracao.Visible = true;
                    Div_nValorBruto.Visible = true;
                    Div_CentroCusto.Visible = true;
                    Div_sDocumento.Visible = true;

                    lblsDocumento.Text = "Referência ";

                    Div_Parceiro_Imposto.Visible = false;
                    ddlidParceiroImposto.SelectedValue = "0";

                    Div_Parceiro_Interno.Visible = false;
                    ddlidParceiroInterno.SelectedValue = "0";

                    Div_Parceiro_Cliente.Visible = true;
                    break;

                case 7: //Serviços / Compensação

                    Div_dtApuracao.Visible = false;
                    txtdtApuracao.Text = "";

                    Div_nValorBruto.Visible = true;
                    Div_CentroCusto.Visible = true;
                    Div_sDocumento.Visible = true;

                    lblNotaFiscal.Text = "Nota Fiscal";
                    lblsDocumento.Text = "Referência";

                    Div_Parceiro_Imposto.Visible = false;
                    ddlidParceiroImposto.SelectedValue = "0";

                    Div_Parceiro_Interno.Visible = false;
                    ddlidParceiroInterno.SelectedValue = "0";

                    Div_Parceiro_Cliente.Visible = true;
                    break;

            }
            string sidContasPagar = hddidContasPagar.Value;
            if (sidContasPagar != "0")
            {
                if (ddlidCategoriaPagar.Attributes["disabled"] == "disabled")
                {
                    ddlidParceiroImposto.Attributes.Add("disabled", "disabled");
                    ddlidParceiroInterno.Attributes.Add("disabled", "disabled");
                    ddlidParceiroCliente.Attributes.Add("disabled", "disabled");
                }
                else
                {
                    ddlidParceiroImposto.Attributes.Remove("disabled");
                    ddlidParceiroInterno.Attributes.Add("disabled", "disabled");
                    ddlidParceiroCliente.Attributes.Add("disabled", "disabled");
                }
            }

            AlterarVisibilidade();

        }

        protected void txtnValorPagamento_Info_Pag_TextChanged(object sender, EventArgs e)
        {
            bs_LancamentoPag.Clear();
            DateTime dataVencimento;
            bool conversaoSucessoData = DateTime.TryParse(txtdtVencimento.Text, out dataVencimento);

            decimal valorPagamento;
            decimal ValorOriginal = 0;
            decimal ValornTotal = 0;
            decimal.TryParse(txtnValorOriginal.Text, out ValorOriginal);
            decimal.TryParse(txtnTotal.Text, out ValornTotal);

            bool conversaoSucesso = Decimal.TryParse(txtnValorPagamento_Info_Pag.Text, out valorPagamento);

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

                        if (valorPagamento < ValorOriginal)
                        {
                            Div_Desconto.Visible = true;
                            decimal desconto = ValornTotal - valorPagamento;
                            txtnDesconto.Text = Math.Max(desconto, 0).ToString();
                        }
                        else
                        {
                            Div_Desconto.Visible = false;
                            txtnDesconto.Text = "0";
                        }

                    }

                }

            }

            txtnValorTotalPag_TextChanged(sender, e);

            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();
            txtnValorTotalPag_TextChanged(objSender, objEventArgs);
        }

        protected void txtnValorTotalPag_TextChanged(object sender, EventArgs e)
        {
            decimal ValorPago = 0;
            decimal ValorMulta = 0;
            decimal ValorJuros = 0;
            decimal ValorDesconto = 0;
            decimal ValorTotal = 0;



            if (!string.IsNullOrEmpty(txtnValorPagamento_Info_Pag.Text))
            {
                if (!decimal.TryParse(txtnValorPagamento_Info_Pag.Text, out ValorPago) || ValorPago < 0)
                {
                    MensagemPagina_Pagamento.MostraMensagem_Erro("Valor Pago inválido!");
                    return;
                }
            }
            if (!string.IsNullOrEmpty(txtnMulta.Text))
            {
                if (!decimal.TryParse(txtnMulta.Text, out ValorMulta) || ValorMulta < 0)
                {
                    MensagemPagina_Pagamento.MostraMensagem_Erro("Valor da Multa inválido!");
                    return;
                }
            }
            if (!string.IsNullOrEmpty(txtnJuros.Text))
            {
                if (!decimal.TryParse(txtnJuros.Text, out ValorJuros) || ValorJuros < 0)
                {
                    MensagemPagina_Pagamento.MostraMensagem_Erro("Valor de Juros inválido!");
                    return;
                }
            }
            if (!string.IsNullOrEmpty(txtnDesconto.Text))
            {
                if (!decimal.TryParse(txtnDesconto.Text, out ValorDesconto) || ValorDesconto < 0)
                {
                    MensagemPagina_Pagamento.MostraMensagem_Erro("Valor de Desconto inválido!");
                    return;
                }
            }
            var control = sender as Control;
            if (control != null)
            {
                if (control.ID == "txtnValorPagamento_Info_Pag")
                {
                    FUNCOES.Scripts.FocusScript(Page, txtnMulta.ClientID);
                }
                else if (control.ID == "txtnMulta")
                {
                    FUNCOES.Scripts.FocusScript(Page, txtnJuros.ClientID);
                }
                else if (control.ID == "txtnJuros")
                {
                    FUNCOES.Scripts.FocusScript(Page, txtnDesconto.ClientID);
                }
                else if (control.ID == "txtnDesconto")
                {
                    FUNCOES.Scripts.FocusScript(Page, ddlidConta_Info_Pag.ClientID);
                }
            }

            ValorTotal = ValorPago + ValorMulta + ValorJuros;

            txtnValorTotalPag.Text = ValorTotal.ToString();
        }

        protected void BtnReabrirTitulo_Click(object sender, EventArgs e)
        {
            string Compensacao = hddlidCompensaca.Value;

            try
            {
                if (int.TryParse(hddidContasPagar.Value, out int idContasPagar))
                {

                    DataSet dsReabrir;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    dsReabrir = BD.ExecutarDataSet(sProcedure, vParametros);

                    vParametros.Add("@sFuncao", "REABRIR");
                    vParametros.Add("@idContasPagar", idContasPagar.ToString());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    vParametros.Add("@idCompensacao", Compensacao.ToString());

                    dsReabrir = BD.ExecutarDataSet(sProcedure, vParametros);
                    MensagemPagina.MostraMensagem_Sucesso("Registro Reaberto com sucesso");
                }
                else
                {
                    throw new Exception("ID inválido");
                }
                string urlReabertuta = string.Format("/app/Paginas/Adm/Financeiro/ContasPagar_Detalhe.aspx?id={0}", hddidContasPagar.Value);
                Response.Redirect(urlReabertuta);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
            RegistraScript("");
        }

        private void AlterarVisibilidade()
        {

            ViewState["sAdiantado"] = txtsAdiantado.Text;
            if (txtsAdiantado.Text != "S")
            {
                DIV_Adiantamento.Visible = false;
                Div_sStatusAdiantado.Visible = false;

            }
            else
            {

                Div_sCodigo.Visible = false;
                Div_sDocumento.Visible = false;

                Div_sStatusAdiantado.Visible = true;
                Div_sQuantidadeParcela.Visible = false;

                lblValorLiquido.CssClass = "bold-label";
                lblValorBruto.CssClass = "bold-label";
                lbldtEmissao.CssClass = "bold-label";

                lblValorLiquido.Text = "Valor Liquido";
                lblValorBruto.Text = "Valor Bruto";
                lbldtEmissao.Text = "Data do Adiantamento";
                Div_ComplementoMeioPag.Visible = false;
                ddlidCategoriaPagar.Attributes.Add("disabled", "disabled");

                DIV_Adiantamento.Visible = true;

                lblValorLiquido.Text = "Valor Despesas";
                Div_nValorBruto.Visible = false;
                Div_Parceiro_Imposto.Visible = false;
                Div_Parceiro_Interno.Visible = false;
                Div_Parceiro_Cliente.Visible = false;
                Div_idBancoCredo.Visible = true;
                ddlidBancoCredor.Attributes.Add("disabled", "disabled");
            }


        }

        protected void txtdtPrevisaoPagamento_TextChanged(object sender, EventArgs e)
        {

            DateTime dataSelecionada;
            if (DateTime.TryParse(txtdtPrevisaoPagamento.Text, out dataSelecionada))
            {
                CultureInfo cultura = new CultureInfo("pt-BR");
                string diaDaSemana = cultura.TextInfo.ToTitleCase(dataSelecionada.ToString("dddd", cultura));

                txtsDiaSemana.Text = diaDaSemana;
            }
            else
            {
                txtsDiaSemana.Text = "Data inválida";
            }

        }

        protected void ddlidCompensacao_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlidFormaPagamento_Info_Pag_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblsComplementoFormaPag.CssClass = "bold-label";

            switch (Convert.ToInt32(ddlidFormaPagamento_Info_Pag.SelectedValue))
            {
                case 0: //Selecione


                    Div_ValorPago.Visible = true;
                    Div_Multa.Visible = true;
                    Div_Juros.Visible = true;
                    Div_idFormaPagamento_Info_Pag.Visible = true;

                    lblValorDesconto.Text = "Valor Desconto";
                    txtnDesconto.Text = "";

                    Div_Compensacao.Visible = false;
                    ddlidCompensacao.SelectedValue = "0";

                    break;
                case 1://Compensação

                    Div_ValorPago.Visible = false;
                    Div_Multa.Visible = false;
                    Div_Juros.Visible = false;
                    lblValorDesconto.Text = "Valor Compensado";
                    txtnDesconto.Text = txtnValorOriginal.Text;
                    txtnDesconto.ReadOnly = true;
                    Div_Compensacao.Visible = true;

                    txtnValorPagamento_Info_Pag.Text = "";
                    txtnMulta.Text = "";
                    txtnJuros.Text = "";
                    txtnValorTotalPag.Text = "";

                    break;

                case 2: //Pix

                    Div_ValorPago.Visible = true;
                    Div_Multa.Visible = true;
                    Div_Juros.Visible = true;
                    Div_idFormaPagamento_Info_Pag.Visible = true;

                    lblValorDesconto.Text = "Valor Desconto";
                    txtnDesconto.ReadOnly = false;
                    txtnDesconto.Text = "";
                    Div_Compensacao.Visible = false;
                    ddlidCompensacao.SelectedValue = "0";
                    break;
                case 3: //Boleto Bancário


                    Div_ValorPago.Visible = true;
                    Div_Multa.Visible = true;
                    Div_Juros.Visible = true;
                    Div_idFormaPagamento_Info_Pag.Visible = true;
                    lblValorDesconto.Text = "Valor Desconto";
                    txtnDesconto.ReadOnly = false;
                    txtnDesconto.Text = "";
                    Div_Compensacao.Visible = false;
                    ddlidCompensacao.SelectedValue = "0";
                    break;
                case 4: //Cartão de Crédito


                    Div_ValorPago.Visible = true;
                    Div_Multa.Visible = true;
                    Div_Juros.Visible = true;
                    Div_idFormaPagamento_Info_Pag.Visible = true;

                    lblValorDesconto.Text = "Valor Desconto";
                    txtnDesconto.ReadOnly = false;
                    txtnDesconto.Text = "";
                    Div_Compensacao.Visible = false;
                    ddlidCompensacao.SelectedValue = "0";
                    break;

                case 5: //Transferência (TED/DOC)


                    Div_ValorPago.Visible = true;
                    Div_Multa.Visible = true;
                    Div_Juros.Visible = true;
                    Div_idFormaPagamento_Info_Pag.Visible = true;

                    lblValorDesconto.Text = "Valor Desconto";
                    txtnDesconto.ReadOnly = false;
                    txtnDesconto.Text = "";
                    Div_Compensacao.Visible = false;
                    ddlidCompensacao.SelectedValue = "0";
                    break;

                case 6: //Mercado Pago/Livre


                    Div_ValorPago.Visible = true;
                    Div_Multa.Visible = true;
                    Div_Juros.Visible = true;
                    Div_idFormaPagamento_Info_Pag.Visible = true;

                    lblValorDesconto.Text = "Valor Desconto";
                    txtnDesconto.ReadOnly = false;
                    txtnDesconto.Text = "";
                    Div_Compensacao.Visible = false;
                    ddlidCompensacao.SelectedValue = "0";
                    break;

                case 7: //Crédito em Conta


                    Div_ValorPago.Visible = true;
                    Div_Multa.Visible = true;
                    Div_Juros.Visible = true;
                    Div_idFormaPagamento_Info_Pag.Visible = true;

                    lblValorDesconto.Text = "Valor Desconto";
                    txtnDesconto.ReadOnly = false;
                    txtnDesconto.Text = "";
                    Div_Compensacao.Visible = false;
                    ddlidCompensacao.SelectedValue = "0";
                    break;

                case 8: //Outros


                    Div_ValorPago.Visible = true;
                    Div_Multa.Visible = true;
                    Div_Juros.Visible = true;
                    Div_idFormaPagamento_Info_Pag.Visible = true;

                    lblValorDesconto.Text = "Valor Desconto";
                    txtnDesconto.ReadOnly = false;
                    txtnDesconto.Text = "";
                    Div_Compensacao.Visible = false;
                    ddlidCompensacao.SelectedValue = "0";
                    break;


            }
            ViewState["sAdiantado"] = txtsAdiantado.Text;
            if (txtsAdiantado.Text == "S")
            {
                Div_ComplementoMeioPag.Visible = false;
            }

            if (txtnSaldo.Text == "0,00")
            {
                btnAlterarVencimento.Visible = false;
            }
            if (txtidContasPagar.Text == "Novo")
            {
                btnAlterarVencimento.Visible = false;
            }
            FUNCOES.Scripts.FocusScript(Page, txtnValorPagamento_Info_Pag.ClientID);

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
                        case eBloco.Pagamento:
                            index = bs_Pagamento.FindIndex(x => x.idLinha.Equals(idLinha));
                            bs_Pagamento[index].idArquivo = 0;
                            bs_Pagamento[index].sNomeArquivo = fu_EnviarArquivo.FileName;
                            bs_Pagamento[index].objArquivo = lObjArquivo;
                            bs_Pagamento[index].sObservacaoArquivo = txtEnviarArquivo_sDscArquivo.Text;
                            dtgPagamento_DataBind();
                            sJSExecutar = " $('#aba_ContasPagar').tab('show'); window.location.hash = '#ctl00_cphCorpo_Div_Pagamento';";
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
                case eBloco.Pagamento:
                    index = bs_Pagamento.FindIndex(x => x.idLinha.Equals(idLinha));
                    sNomeArquivo = bs_Pagamento[index].sNomeArquivo;
                    bObjArquivo = bs_Pagamento[index].objArquivo;
                    idArquivo = bs_Pagamento[index].idArquivo;
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
            Pagamento = 1,
        }

        #endregion

        #region | Aba Documentos 

        void Popular_Aba_Arquivos(string idContasPagar)
        {
            string sPermissao = "S";

            if (!FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasPagar.IncluiArquivo))
            {
                sPermissao = "N";
            }
            frmArquivos.Attributes.Add("src", string.Format("~/App/Paginas/Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}&sPermiteNovoArquivo={2}", idContasPagar, "ContasPagar", sPermissao));
            aba_Arquivo.Visible = true;
            frmArquivos.Visible = true;
        }


        #endregion

        #region | Lançamentos 

        bool Salvar_LancamentoPag(string idContasPagar)
        {
            bool bRetorno = false;

            try
            {
                foreach (var LancamentoPag_Linha in bs_LancamentoPag)
                {
                    if (LancamentoPag_Linha.sFuncao != "SEM ALTERAÇÃO")
                    {
                        Dictionary<String, String> vParametroItensLancamentoPag = new Dictionary<string, string>();

                        vParametroItensLancamentoPag["@idRegistro"] = LancamentoPag_Linha.idRegistroLancamentoPag.ToString();
                        vParametroItensLancamentoPag["@sFuncao"] = LancamentoPag_Linha.sFuncao;
                        vParametroItensLancamentoPag["@idContasPagar"] = idContasPagar;

                        vParametroItensLancamentoPag["dtLancamentoPag"] = LancamentoPag_Linha.dtLancamentoPag.ToString();
                        vParametroItensLancamentoPag["@nParcelaLancamentoPag"] = LancamentoPag_Linha.nParcelaLancamentoPag.ToString();
                        vParametroItensLancamentoPag["@nValorLancamentoPag"] = LancamentoPag_Linha.nValorLancamentoPag.ToString().Replace(",", ".");
                        vParametroItensLancamentoPag["@idFormaPagamentoLancamentoPag"] = LancamentoPag_Linha.idFormaPagamentoLancamentoPag.ToString();
                        vParametroItensLancamentoPag["@sDscFormaPagamentoLancamentoPag"] = LancamentoPag_Linha.idFormaPagamentoLancamentoPag.ToString();
                        BD.ExecutarDataSet(sProcedure, vParametroItensLancamentoPag);
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

            DIV_Lancamentos.Visible = false;
            Div_Pagamento.Visible = true;

            return bRetorno;

        }

        protected void txtnParcelas_TextChanged(object sender, EventArgs e)
        {
            IncluirLancamento();
            FUNCOES.Scripts.FocusScript(Page, dtgLancamento.Rows[0].Cells[4].Controls[1].ClientID);
        }

        protected void IncluirLancamento()
        {
            if (ddlidMeioPagamento.SelectedValue != "0")
            {
                bs_LancamentoPag.Clear();
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
                    FrameWork.cls_LancamentoPag objItem = new FrameWork.cls_LancamentoPag();

                    string[] vidContasPagar = hddidContasPagar.Value.Split(',');
                    string idContasPagar = vidContasPagar[0].ToString();

                    objItem.idLinha = bs_LancamentoPag.Count() + 1;
                    objItem.sFuncao = "INSERIR_LANCAMENTO";
                    objItem.idFormaPagamentoLancamentoPag = Convert.ToInt32(ddlidFormaPagamento.SelectedValue);
                    objItem.idContasPagar = Convert.ToInt32(idContasPagar);

                    if (i == numeroParcelas - 1)
                    {
                        objItem.nValorLancamentoPag = valorParcelaUltima;
                        objItem.nValorBrutoLancamentoPag = valorParcelaUltimaBruto;
                    }
                    else
                    {
                        objItem.nValorLancamentoPag = valorParcela;
                        objItem.nValorBrutoLancamentoPag = valorParcelaBruto;
                    }

                    int proximaParcela = bs_LancamentoPag.Count + 1;
                    objItem.nParcelaLancamentoPag = proximaParcela;

                    bs_LancamentoPag.Add(objItem);

                }



                dtgLancamento_DataBind();
                LimpaCampos_Lancamento();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtdtLancamentoPag]').focus();", true);
            }
        }


        void Lancamentos_Salvar()
        {
            int nContador = 0;
            foreach (GridViewRow item in dtgLancamento.Rows)
            {
                TextBox txtnValorLancamentoPag_Linha = (TextBox)item.FindControl("txtnValorLancamentoPag");
                if (string.IsNullOrEmpty(txtnValorLancamentoPag_Linha.Text) || txtnValorLancamentoPag_Linha.Text == "0,00")
                {
                    TextBox txtdtLancamentoPag_Linha = (TextBox)item.FindControl("txtdtLancamentoPag");
                    bs_LancamentoPag[nContador].dtLancamentoPag = txtdtLancamentoPag_Linha.Text;

                    DropDownList ddlidFormaPagamentoLancamentoPag_Linha = (DropDownList)item.FindControl("ddlidFormaPagamentoLancamentoPag");
                    bs_LancamentoPag[nContador].sDscFormaPagamentoLancamentoPag = ddlidFormaPagamentoLancamentoPag_Linha.SelectedItem.ToString();


                    nContador++;
                }

            }
        }

        protected void dtgLancamento_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            bs_LancamentoPag.RemoveAt(index);
            dtgLancamento_DataBind();
        }


        private bool ValidarDados_Lancamento(ref string sMensagem)
        {
            bool bRetorno = true;

            string sMensagemErro = "";

            foreach (GridViewRow item in dtgLancamento.Rows)
            {
                TextBox txtdtLancamentoPag_Linha = (TextBox)item.FindControl("txtdtLancamentoPag");

                if (!Validacoes.ValidarData(txtdtLancamentoPag_Linha))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data Invalida";
                }


            }

            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina_Pagamento.MostraMensagem_Erro(sMensagemErro);
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
                FrameWork.cls_LancamentoPag objItem = new FrameWork.cls_LancamentoPag();
                objItem.idContasPagar = Convert.ToInt32(row["idContasPagar"].ToString());

                objItem.dtLancamentoPag = row["dtLancamentoPag"].ToString();
                objItem.nParcelaLancamentoPag = Convert.ToInt32(row["nParcelaLancamentoPag"].ToString());

                bs_LancamentoPag.Add(objItem);
            }
            dtgLancamento_DataBind();
            LimpaCampos_Lancamento();
        }
        protected void dtgLancamento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlidFormaPagamentoLancamentoPag = (e.Row.FindControl("ddlidFormaPagamentoLancamentoPag") as DropDownList);
                FUNCOES.Popula_Combo(ddlidFormaPagamentoLancamentoPag, "sp_Select 'tbl_Flow_CondicaodePagamento_Tipo'", "idTipoCondicaoPagamento", "sDscTipoCondicaoPagamento", false, "Forma de Pagamento", "0");
                ddlidFormaPagamentoLancamentoPag.SelectedValue = bs_LancamentoPag[e.Row.RowIndex].idFormaPagamentoLancamentoPag.ToString();
            }
            GRID.EsconderColunas(e, 0);
        }

        void dtgLancamento_DataBind()
        {
            dtgLancamento.DataSource = bs_LancamentoPag;
            dtgLancamento.DataBind();
        }

        void Repopular()
        {
            var idContasPagar = hddidContasPagar.Value;
            if (idContasPagar != "0")
            {
                Pesquisar(hddidContasPagar.Value, true);
                txtsCodigo.ReadOnly = true;
                txtdtEmissao.ReadOnly = true;
                txtdtVencimento.ReadOnly = true;
                txtsDocumento.ReadOnly = true;
                txtnValorOriginal.ReadOnly = true;
                txtnValorBruto.ReadOnly = true;
                txtnSaldo.ReadOnly = true;
                ddlidMeioPagamento.Attributes.Add("disabled", "disabled");
                txtnParcelas.ReadOnly = true;
                txtdtPrevisaoPagamento.ReadOnly = true;

                ddlidFormaPagamento.Attributes.Add("disabled", "disabled");
                ddlidContabil.Attributes.Add("disabled", "disabled");
                Controle_CategoriasCC.CentroDeCusto.Attributes.Add("disabled", "disabled");
                Controle_CategoriasCC.CategoriaCC.Attributes.Add("disabled", "disabled");
                ddlidEmpresa.Attributes.Add("disabled", "disabled");


                txtsObservacaoGeral.ReadOnly = true;
                txtsComplementoFormaPag.ReadOnly = true;
                txtdtApuracao.ReadOnly = true;
                ddlidCategoriaPagar.Attributes.Add("disabled", "disabled");
            }
        }


        #endregion

        protected void BtnSalvarObservacao_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(hddidContasPagar.Value, out int idContasPagar))
                {

                    DataSet dsOBS;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    dsOBS = BD.ExecutarDataSet(sProcedure, vParametros);

                    vParametros.Add("@sFuncao", "SALVAR_OBSERVACAO");
                    vParametros.Add("@idContasPagar", idContasPagar.ToString());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    vParametros.Add("@sAnotacao", txtsAnotacao.Text);

                    dsOBS = BD.ExecutarDataSet(sProcedure, vParametros);
                    MensagemPagina.MostraMensagem_Sucesso("Observação salva com sucesso!");
                }
                else
                {
                    throw new Exception("ID inválido");
                }
                string urlOBS = string.Format("/app/Paginas/Adm/Financeiro/ContasPagar_Detalhe.aspx?id={0}", hddidContasPagar.Value);
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

        protected void ddlidParceiroImposto_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlidParceiroInterno_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlidParceiroCliente_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlidBancoCredor_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void txtnValorBruto_TextChanged(object sender, EventArgs e)
        {
            if (txtnValorOriginal.Text == "")
            {
                txtnValorOriginal.Text = txtnValorBruto.Text;
            }
            FUNCOES.Scripts.FocusScript(Page, txtsCodigo.ClientID);
        }

        #region | Rateio Centro de Custos

        [Serializable] // ESSENCIAL para evitar o erro de serialização no ViewState
        public class LancamentoCentroCusto
        {
            public int idLancamento { get; set; }
            public Guid GridKey { get; set; }
            public int idCentroCusto { get; set; }
            public string sDscCentroCusto { get; set; }
            public int idRegistroCategoria { get; set; }
            public string sDscCategoria { get; set; }
            public decimal nValor { get; set; }
            public string sObservacao { get; set; }
            public string sFuncao { get; set; }
        }

        // Propriedade para armazenar a lista de rateios na ViewState
        public List<LancamentoCentroCusto> bs_LancamentosCC
        {
            get
            {
                if (ViewState["bs_LancamentosCC"] == null)
                {
                    ViewState["bs_LancamentosCC"] = new List<LancamentoCentroCusto>();
                }
                return (List<LancamentoCentroCusto>)ViewState["bs_LancamentosCC"];
            }
            set { ViewState["bs_LancamentosCC"] = value; }
        }

        private void CarregarLancamentosCC(string idContasPagar)
        {
            bs_LancamentosCC.Clear();
            Dictionary<String, String> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@idObjeto", idContasPagar },
                { "@sTipoObjeto", "P" } // 'P' para Contas a Pagar
            };

            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Lancamentos_CentroDeCustos", vParametros);

            if (BD.ValidarDataSet(ds))
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    bs_LancamentosCC.Add(new LancamentoCentroCusto
                    {
                        idLancamento = Convert.ToInt32(row["idLancamento"]),
                        GridKey = Guid.NewGuid(),
                        idCentroCusto = Convert.ToInt32(row["idCentroCusto"]),
                        sDscCentroCusto = row["sDscCentroCusto"].ToString(),
                        idRegistroCategoria = Convert.ToInt32(row["idRegistroCategoria"]),
                        sDscCategoria = row["sDscCategoria"].ToString(),
                        nValor = Convert.ToDecimal(row["nValor"]),
                        sObservacao = row["sObservacao"].ToString(),
                        sFuncao = "SEM ALTERACAO"
                    });
                }
            }
            gvLancamentosCC_DataBind();
            AlternarVisibilidadeRateio();
        }

        private bool SalvarLancamentosCC(string idContasPagar)
        {
            string sErro = "";

            decimal valorTotalTitulo = 0;
            decimal.TryParse(txtnValorOriginal.Text, out valorTotalTitulo);
            decimal valorTotalRateado = bs_LancamentosCC.Where(l => l.sFuncao != "EXCLUIR").Sum(l => l.nValor);

            if (bs_LancamentosCC.Any(l => l.sFuncao != "EXCLUIR") && Math.Abs(valorTotalTitulo - valorTotalRateado) > 0.01m)
            {
                MensagemPagina.MostraMensagem($"A soma dos Centros de Custos Divididos (R$ {valorTotalRateado:N2}) não corresponde ao Valor Líquido do título (R$ {valorTotalTitulo:N2}). Ajuste os valores antes de salvar.", "error", true);
                return false;
            }

            foreach (var lancamento in bs_LancamentosCC)
            {
                if (lancamento.sFuncao != "SEM ALTERACAO")
                {
                    Dictionary<String, String> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", lancamento.sFuncao == "EXCLUIR" ? "EXCLUIR" : "SALVAR" },
                        { "@idLancamento", lancamento.idLancamento.ToString() },
                        { "@idObjeto", idContasPagar },
                        { "@sTipoObjeto", "P" },
                        { "@idCentroCusto", lancamento.idCentroCusto.ToString() },
                        { "@idRegistroCategoria", lancamento.idRegistroCategoria.ToString() },
                        { "@nValor", lancamento.nValor.ToString().Replace(",",".") },
                        { "@sObservacao", lancamento.sObservacao },
                        { "@idUsuario", IDENTITY.Variaveis.idUsuario() }
                    };



                    DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Lancamentos_CentroDeCustos", vParametros);
                    if (!BD.ValidarDataSet(ds, out sErro))
                    {
                        MensagemPagina.MostraMensagem_Erro($"Erro ao salvar Centros de Custos Divididos do centro de custo: {sErro}");
                        return false;
                    }
                    //else
                    //{
                    //    var infoSaldo = new AtualizacaoSaldoInfo
                    //    {
                    //        IdCentroCusto = lancamento.idCentroCusto,
                    //        Valor = lancamento.nValor,
                    //        IdCategoria = lancamento.idRegistroCategoria,
                    //        IdCategoriaAntiga = 0
                    //    };


                    //    switch (lancamento.sFuncao)
                    //    {
                    //        case "INSERIR":
                    //            infoSaldo.Acao = "SUBTRAIR";
                    //            break;
                    //        case "EXCLUIR":
                    //            infoSaldo.Acao = "SOMAR";
                    //            break;
                    //    }

                    //    if (!string.IsNullOrEmpty(infoSaldo.Acao))
                    //    {
                    //        ControleCategoriasModal.AtualizarSaldoCategoria(infoSaldo);
                    //    }
                    //}
                }
            }
            return true;
        }

        private bool SalvarLancamentosCC(string idContaPagarParcela, int totalParcelas, decimal valorTotalTitulo, decimal valorParcela)
        {
            string sErro = "";

            if (totalParcelas == 0)
            {
                totalParcelas = 1;
            }

            foreach (var lancamento in bs_LancamentosCC)
            {
                string funcaoSalvar = (lancamento.sFuncao == "SEM ALTERACAO") ? "SALVAR" : lancamento.sFuncao;

                if (funcaoSalvar != "EXCLUIR")
                {
                    decimal porcentagemEquivalente = lancamento.nValor / valorTotalTitulo;

                    decimal valorRateioParcelado = valorParcela * porcentagemEquivalente;

                    valorRateioParcelado = Math.Round(valorRateioParcelado, 2);

                    Dictionary<String, String> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" }, 
                        { "@idLancamento", "0" }, 
                        { "@idObjeto", idContaPagarParcela },
                        { "@sTipoObjeto", "P" },
                        { "@idCentroCusto", lancamento.idCentroCusto.ToString() },
                        { "@idRegistroCategoria", lancamento.idRegistroCategoria.ToString() },
                        { "@nValor", valorRateioParcelado.ToString().Replace(",",".") }, 
                        { "@sObservacao", lancamento.sObservacao },
                        { "@idUsuario", IDENTITY.Variaveis.idUsuario() }
                    };

                    DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Lancamentos_CentroDeCustos", vParametros);

                    if (!BD.ValidarDataSet(ds, out sErro))
                    {
                        MensagemPagina.MostraMensagem_Erro($"Erro ao salvar rateio do centro de custo: {sErro}");
                        return false;
                    }
                }
            }
            return true;
        }
        private void AlternarVisibilidadeRateio()
        {
            bool temRateios = bs_LancamentosCC.Any(l => l.sFuncao != "EXCLUIR");

            pnlCentroCustoPrincipal.Visible = !temRateios;
            pnlRateiosLista.Visible = temRateios;

            if (temRateios)
            {
                var rateiosAtivos = bs_LancamentosCC.Where(l => l.sFuncao != "EXCLUIR").ToList();

                rptRateios.DataSource = rateiosAtivos;

                rptRateios.DataBind();
            }

            //if (temRateios)
            //{
            //    lbRateios.Items.Clear();
            //    foreach (var item in bs_LancamentosCC.Where(l => l.sFuncao != "EXCLUIR"))
            //    {
            //        string texto = $"{item.sDscCentroCusto} / {item.sDscCategoria} - R$ {item.nValor:N2}";
            //        lbRateios.Items.Add(new ListItem(texto));
            //    }
            //}
        }

        protected void btnAbrirModalCC_Click(object sender, EventArgs e)
        {
            divValorModal.Visible = false;
            ControleCategoriasModal.EsconderDivCategoria();

            if (string.IsNullOrEmpty(txtnValorOriginal.Text) || Convert.ToDecimal(txtnValorOriginal.Text) == 0)
            {
                MensagemPagina.MostraMensagem("É necessário informar o 'Valor Líquido' do título antes de fazer o rateio.", "warning", false);
                return;
            }

            if (!bs_LancamentosCC.Any(l => l.sFuncao != "EXCLUIR"))
            {
                if (Controle_CategoriasCC.CentroDeCusto.SelectedValue != "0")
                {
                    bs_LancamentosCC.Add(new LancamentoCentroCusto
                    {
                        idLancamento = 0,
                        GridKey = Guid.NewGuid(),
                        idCentroCusto = Convert.ToInt32(Controle_CategoriasCC.CentroDeCusto.SelectedValue),
                        sDscCentroCusto = Controle_CategoriasCC.CentroDeCusto.SelectedItem.Text,
                        idRegistroCategoria = Convert.ToInt32(Controle_CategoriasCC.CategoriaCC.SelectedValue),
                        sDscCategoria = Controle_CategoriasCC.CategoriaCC.SelectedItem.Text,
                        nValor = Convert.ToDecimal(txtnValorOriginal.Text),
                        sObservacao = "Lançamento principal importado",
                        sFuncao = "INSERIR"
                    });
                    gvLancamentosCC_DataBind();
                }
            }

            FUNCOES.Popula_Combo(ControleCategoriasModal.CentroDeCusto, "sp_Select 'Flow_Adm_CentroDeCusto'", "idCentroDeCusto", "sDescricao", false, "Selecione...", "0");
            LimparCamposModalCC();

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AbrirModalCC", "$('#modalCentroDeCustos').modal('show');", true);
        }

        protected void ControleCategoriasModal_CentroDeCusto_TextChanged(object sender, EventArgs e)
        {
            if (ControleCategoriasModal.CentroDeCusto.SelectedValue != "0")
            {
                divValorModal.Visible = true;
                ControleCategoriasModal.CarregarCategorias(ControleCategoriasModal.CentroDeCusto.SelectedValue);
            }
            else
            {
                divValorModal.Visible = false;
                ControleCategoriasModal.EsconderDivCategoria();
                ControleCategoriasModal.CategoriaCC.Items.Clear();
                ControleCategoriasModal.CategoriaCC.Items.Add(new ListItem("Selecione um Centro de Custo", "0"));
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ManterModalAberta", "$('#modalCentroDeCustos').modal('show');", true);
        }

        protected void btnModalIncluir_Click(object sender, EventArgs e)
        {
            ControleCategoriasModal.AlteraTamanhoCampos(4, 4);
            string sErro = ValidarInclusaoModal(true);
            if (string.IsNullOrEmpty(sErro))
            {
                bs_LancamentosCC.Add(new LancamentoCentroCusto
                {
                    idLancamento = 0,
                    GridKey = Guid.NewGuid(),
                    idCentroCusto = Convert.ToInt32(ControleCategoriasModal.CentroDeCusto.SelectedValue),
                    sDscCentroCusto = ControleCategoriasModal.CentroDeCusto.SelectedItem.Text,
                    idRegistroCategoria = Convert.ToInt32(ControleCategoriasModal.CategoriaCC.SelectedValue),
                    sDscCategoria = ControleCategoriasModal.CategoriaCC.SelectedItem.Text,
                    nValor = Convert.ToDecimal(txtModalValor.Text),
                    sObservacao = txtModalObservacao.Text,
                    sFuncao = "INSERIR"
                });

                gvLancamentosCC_DataBind();
                LimparCamposModalCC();
                AlternarVisibilidadeRateio();
            }
            else
            {
                MensagemModalCC.MostraMensagem_Erro(sErro);
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ManterModalAberta", "$('#modalCentroDeCustos').modal('show');", true);
        }

        private string ValidarInclusaoModal(bool ehNovaInclusao, decimal valorSendoEditado = 0)
        {
            if (ehNovaInclusao)
            {
                if (ControleCategoriasModal.CentroDeCusto.SelectedValue == "0") return "Selecione um Centro de Custo.";
                //if (ControleCategoriasModal.CategoriaCC.SelectedValue == "0") return "Selecione uma Categoria.";
                if (string.IsNullOrWhiteSpace(txtModalValor.Text)) return "O campo Valor é obrigatório.";
            }

            decimal valorInclusao;
            if (!decimal.TryParse(ehNovaInclusao ? txtModalValor.Text : valorSendoEditado.ToString(), out valorInclusao) || valorInclusao <= 0)
            {
                return "O Valor informado é inválido.";
            }

            decimal valorTotalTitulo = Convert.ToDecimal(txtnValorOriginal.Text);
            decimal valorJaRateado = bs_LancamentosCC.Where(l => l.sFuncao != "EXCLUIR").Sum(l => l.nValor);

            if (!ehNovaInclusao)
            {
                valorJaRateado -= valorSendoEditado;
            }

            if ((valorJaRateado + valorInclusao) > (valorTotalTitulo + 0.01m))
            {
                return $"O valor total do rateio (R$ {(valorJaRateado + valorInclusao):N2}) não pode ultrapassar o valor líquido do título (R$ {valorTotalTitulo:N2}).";
            }
            return string.Empty;
        }

        private void LimparCamposModalCC()
        {
            ControleCategoriasModal.CentroDeCusto.SelectedValue = "0";
            ControleCategoriasModal.CategoriaCC.Items.Clear();
            ControleCategoriasModal.CategoriaCC.Items.Add(new ListItem("Selecione um Centro de Custo", "0"));
            txtModalValor.Text = "";
            txtModalObservacao.Text = "";
        }

        private void gvLancamentosCC_DataBind()
        {
            gvLancamentosCC.DataSource = bs_LancamentosCC.Where(l => l.sFuncao != "EXCLUIR").ToList();
            gvLancamentosCC.DataBind();
        }

        protected void gvLancamentosCC_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Guid gridKey = (Guid)gvLancamentosCC.DataKeys[e.RowIndex]["GridKey"];
            var lancamentoParaExcluir = bs_LancamentosCC.FirstOrDefault(l => l.GridKey == gridKey);

            if (lancamentoParaExcluir != null)
            {
                if (lancamentoParaExcluir.idLancamento == 0)
                {
                    bs_LancamentosCC.Remove(lancamentoParaExcluir);
                }
                else
                {
                    lancamentoParaExcluir.sFuncao = "EXCLUIR";
                }
            }

            gvLancamentosCC_DataBind();
            AlternarVisibilidadeRateio();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ManterModalAberta", "$('#modalCentroDeCustos').modal('show');", true);
        }

        protected void gvLancamentosCC_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Aplica a máscara de moeda na TextBox dentro da Grid
                TextBox txtGridValor = (TextBox)e.Row.FindControl("txtGridValor");
                if (txtGridValor != null)
                {
                    // O ScriptManager já deve ter o jQuery Mask Plugin registrado na página principal
                    string script = string.Format("$('[id$={0}]').mask('000.000.000.000.000,00', {{ reverse: true }});", txtGridValor.ID);
                    ScriptManager.RegisterStartupScript(e.Row, e.Row.GetType(), "MaskValorGrid_" + e.Row.RowIndex, script, true);
                }
            }

            if (e.Row.RowType == DataControlRowType.Footer)
            {
                decimal valorTotalRateado = bs_LancamentosCC.Where(l => l.sFuncao != "EXCLUIR").Sum(l => l.nValor);
                decimal valorTotalTitulo = 0;
                if (!string.IsNullOrEmpty(txtnValorOriginal.Text))
                {
                    decimal.TryParse(txtnValorOriginal.Text, out valorTotalTitulo);
                }
                decimal valorRestante = valorTotalTitulo - valorTotalRateado;

                e.Row.Cells[1].Text = "TOTAIS";
                e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[2].Text = valorTotalRateado.ToString("N2");
                e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[3].Text = $"Restante: {valorRestante:N2}";
                e.Row.Cells[3].Font.Bold = true;

                if (Math.Round(valorRestante, 2) < 0)
                    e.Row.Cells[3].ForeColor = System.Drawing.Color.Red;
                else if (Math.Round(valorRestante, 2) > 0)
                    e.Row.Cells[3].ForeColor = System.Drawing.Color.Orange;
                else
                    e.Row.Cells[3].ForeColor = System.Drawing.Color.Green;
            }
        }

        protected void txtGridValor_TextChanged(object sender, EventArgs e)
        {
            TextBox txtValor = (TextBox)sender;
            GridViewRow row = (GridViewRow)txtValor.NamingContainer;
            Guid gridKey = (Guid)gvLancamentosCC.DataKeys[row.RowIndex]["GridKey"];

            var lancamento = bs_LancamentosCC.FirstOrDefault(l => l.GridKey == gridKey);
            if (lancamento == null) return;

            decimal novoValor;
            if (decimal.TryParse(txtValor.Text, out novoValor) && novoValor >= 0)
            {

                decimal valorTotalTitulo = 0;
                decimal.TryParse(txtnValorOriginal.Text, out valorTotalTitulo);

                decimal somaOutrosRateios = bs_LancamentosCC
                                            .Where(l => l.GridKey != gridKey && l.sFuncao != "EXCLUIR")
                                            .Sum(l => l.nValor);

                if (Math.Round(somaOutrosRateios + novoValor, 2) > Math.Round(valorTotalTitulo, 2))
                {
                    string erro = $"O valor total do Centros de Custos Divididos (R$ {(somaOutrosRateios + novoValor):N2}) excede o valor do título (R$ {valorTotalTitulo:N2}).";

                    txtValor.Text = lancamento.nValor.ToString("N2");
                    MensagemModalCC.MostraMensagem_Erro(erro);
                }
                else
                {
                    lancamento.nValor = novoValor;
                    if (lancamento.sFuncao == "SEM ALTERACAO")
                    {
                        lancamento.sFuncao = "INSERIR";
                    }
                }
            }
            else
            {
                if (lancamento != null) txtValor.Text = lancamento.nValor.ToString("N2");
            }

            gvLancamentosCC_DataBind();
            AlternarVisibilidadeRateio();

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ManterModalAberta", "$('#modalCentroDeCustos').modal('show');", true);
        }

        #endregion

        protected void btnSalvarValoresAdiantamento_Click(object sender, EventArgs e)
        {
            DataSet ds;
            Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "Atualizar_Taxa_Adiantamento" },
                    { "@idContaspagar", hddidContasPagar.Value },
                    { "@nTaxaJuros", txtnTotalJuros.Text.Replace(".","").Replace(",",".") },
                    { "@nIOF", txtnTotalIOF.Text.Replace(".","").Replace(",",".") },
                    { "@nTarifas", txtnTarifas.Text.Replace(".","").Replace(",",".") },                  
                    { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                };
            ds = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(ds, out string sErro))
            {
                Session["SalvoComSucesso"] = true;
                Response.Redirect(Request.RawUrl);
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao alterar valores das taxas: " + sErro);
            }

            RegistraScript("");
        }

        protected void gv_Log_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[4].Text = HttpUtility.HtmlDecode(e.Row.Cells[4].Text);
            }
        }
    }
}