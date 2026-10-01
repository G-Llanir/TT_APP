using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;
using TT_Flow.FrameWork;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class Colaboradores_Pagamento : Page
    {
        private const string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Pagamento";
        private const string sProcedureContasPagar = "sp_Manipula_tbl_Flow_Adm_Contas_Pagar";
        private const string sTituloPagina = "Pagamentos de Colaboradores";
        private const int ColunaAcoesConsulta = 8;

        List<cls_LancamentoPag> bs_LancamentoModal
        {
            get
            {
                if (ViewState["bs_LancamentoModal"] == null)
                    ViewState["bs_LancamentoModal"] = new List<cls_LancamentoPag>();
                return (List<cls_LancamentoPag>)ViewState["bs_LancamentoModal"];
            }
            set { ViewState["bs_LancamentoModal"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.RRHH.PagamentosColaboradores.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.PagamentosColaboradores.Incluir, false);

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                PopularCombos();
                PopularCombosModal();

                if (Session["SalvoComSucesso"] != null && (bool)Session["SalvoComSucesso"])
                {
                    MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!");
                    Session["SalvoComSucesso"] = false;
                }
                if (Session["GeradoCPComSucesso"] != null && (bool)Session["GeradoCPComSucesso"])
                {
                    MensagemPagina.MostraMensagem_Sucesso(Session["GeradoCPMensagem"]?.ToString() ?? "Contas a Pagar gerados com sucesso!");
                    Session["GeradoCPComSucesso"] = false;
                    Session["GeradoCPMensagem"] = null;
                }

                Pesquisar();
            }
        }

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidTipo, "sp_Select 'tbl_Flow_Colaboradores_Pagamento_Tipo'", "idTipo", "sDscTipo", false, "Todos os Tipos", "0");
            FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Todas as Empresas", "0");
            FUNCOES.Popula_Combo(ddlidSituacao, "sp_Select 'tbl_Flow_Colaboradores_Pagamento_Situacao'", "idSituacao", "sDscSituacao", false, "Todas as Situa\u00e7\u00f5es", "0");
        }

        void PopularCombosModal()
        {
            FUNCOES.Popula_Combo(ddlidCategoriaPagar, "sp_Select 'Flow_Adm_Contas_Pagar_Categoria'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Selecione a Categoria", "0");
            FiltrarCategoriasInternos();
            FUNCOES.Popula_Combo(ddlidFormaPagamento, "sp_Select 'tbl_Flow_CondicaodePagamento_Tipo'", "idTipoCondicaoPagamento", "sDscTipoCondicaoPagamento", false, "Selecione o Pagamento", "0");
            FUNCOES.Popula_Combo(ddlidMeioPagamento, "sp_Select 'tbl_Flow_Adm_MeioPagamento'", "idMeioPagamento", "sDscMeioPagamento", false, "Selecione um Meio de Pagamento", "0");
            FUNCOES.Popula_Combo(ddlidContabil, "sp_Select 'Flow_CodigoContabil'", "idContabil", "sDscCodContabil", false, "Selecione o C\u00f3digo Cont\u00e1bil", "0");
            div_camposModal.Visible = false;
            btnConfirmarGerarCP.Visible = false;
            LimparParcelasModal();
        }

        void LimparParcelasModal()
        {
            bs_LancamentoModal = new List<cls_LancamentoPag>();
            txtnParcelas.Text = "";
            Div_nParcelas.Visible = false;
            DIV_Lancamentos.Visible = false;
        }

        void FiltrarCategoriasInternos()
        {
            var remover = new List<ListItem>();
            foreach (ListItem item in ddlidCategoriaPagar.Items)
            {
                if (item.Value == "0") continue;
                if (!item.Text.StartsWith("Internos -", StringComparison.OrdinalIgnoreCase))
                    remover.Add(item);
            }
            foreach (var item in remover)
                ddlidCategoriaPagar.Items.Remove(item);
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        void Pesquisar()
        {
            pnResultado.Visible = false;
            try
            {
                var vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR" },
                    { "@dtInicio", txtdtInicio.Text },
                    { "@dtFinal", txtdtFinal.Text },
                    { "@idTipo", ddlidTipo.SelectedValue },
                    { "@idEmpresa", ddlidEmpresa.SelectedValue },
                    { "@idSituacao", ddlidSituacao.SelectedValue }
                };

                DataTable tb = BD.ExecutarDataTable(sProcedure, vParametros);
                if (tb.Rows.Count > 0)
                {
                    pnResultado.Visible = true;
                    AplicarVisibilidadeColunaAcoes(tb);
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables",
                        Grid.DataBindComScriptDataPaging(dtgvConsulta, tb, true, new int[] { 2, 3 }, "desc", "false", "''"), true);
                }
                else
                    MensagemPagina.MostraMensagem_Erro("Nenhum pagamento localizado.");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao consultar: " + ex.Message);
            }
        }

        void AplicarVisibilidadeColunaAcoes(DataTable tb)
        {
            bool temPagamentoEmDigitacao = false;
            if (tb != null)
            {
                foreach (DataRow row in tb.Rows)
                {
                    if (row["idSituacao"]?.ToString() == "1")
                    {
                        temPagamentoEmDigitacao = true;
                        break;
                    }
                }
            }

            bool exibirColuna = temPagamentoEmDigitacao
                && FUNCOES.ValidaPermissao(Permissao.RRHH.PagamentosColaboradores.GerarContasPagar, false);

            if (dtgvConsulta.Columns.Count > ColunaAcoesConsulta)
                dtgvConsulta.Columns[ColunaAcoesConsulta].Visible = exibirColuna;
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            string idSituacao = DataBinder.Eval(e.Row.DataItem, "idSituacao")?.ToString();
            var lnkEditar = (HyperLink)e.Row.FindControl("lnkEditar");
            var btnGerar = (LinkButton)e.Row.FindControl("btnGerarCP");

            if (idSituacao != "1")
            {
                if (lnkEditar != null)
                    lnkEditar.NavigateUrl = "Colaboradores_Pagamento_Detalhe.aspx?id=" + DataBinder.Eval(e.Row.DataItem, "idPagamento") + "&modo=visualizar";
                if (btnGerar != null)
                    btnGerar.Visible = false;
            }
            else
            {
                if (btnGerar != null)
                    btnGerar.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.PagamentosColaboradores.GerarContasPagar, false);
            }
        }

        protected string GetGerarCpScript(object idPagamento, object referencia, object total, object idSituacao)
        {
            if (idSituacao?.ToString() != "1") return "return false;";
            decimal nTotal = 0;
            decimal.TryParse(total?.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture, out nTotal);
            string sRef = referencia?.ToString()?.Replace("'", "\\'") ?? "";
            return $"openModalGerarCP('{idPagamento}', '{sRef}', '{nTotal.ToString("N2")}'); return false;";
        }

        protected void ddlidCategoriaPagar_SelectedIndexChanged(object sender, EventArgs e)
        {
            div_camposModal.Visible = false;
            btnConfirmarGerarCP.Visible = false;
            hddTipoCategoria.Value = "0";
            LimparParcelasModal();

            if (ddlidCategoriaPagar.SelectedValue == "0") return;

            try
            {
                var vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "Autoselecao_Contabil" },
                    { "@idCategoriaPagar", ddlidCategoriaPagar.SelectedValue }
                };
                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", vParametros);
                if (BD.ValidarDataSet(ds))
                {
                    ddlidContabil.SelectedValue = RETORNO.DATASET(ds, 0, "idContabil");
                    string idCatTipo = RETORNO.DATASET(ds, 0, "idCategoriaTipo");
                    hddTipoCategoria.Value = idCatTipo;

                    if (idCatTipo != "4")
                    {
                        MensagemPaginaModal.MostraMensagem_Erro("Selecione uma categoria do tipo Internos.");
                        return;
                    }

                    if (!string.IsNullOrEmpty(hddidPagamentoGerar.Value) && hddidPagamentoGerar.Value != "0")
                        ViewState["idPagamentoGerar"] = hddidPagamentoGerar.Value;

                    div_camposModal.Visible = true;
                    btnConfirmarGerarCP.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaModal.MostraMensagem_Erro(ex.Message);
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "js_OpenModalGerar",
                "limparBackdropModal(); $('#modalGerarCP').modal({ backdrop: 'static', keyboard: false, show: true }); setTimeout(inicializarChosenModalGerarCP, 200);", true);
        }

        protected void ddlidMeioPagamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            LimparParcelasModal();
            if (ddlidMeioPagamento.SelectedValue == "2")
                Div_nParcelas.Visible = true;
            ReabrirModalGerarCP();
        }

        protected void txtnParcelas_TextChanged(object sender, EventArgs e)
        {
            MontarParcelasModal();
            ReabrirModalGerarCP();
        }

        void MontarParcelasModal()
        {
            bs_LancamentoModal = new List<cls_LancamentoPag>();
            if (ddlidMeioPagamento.SelectedValue != "2")
            {
                DIV_Lancamentos.Visible = false;
                return;
            }

            if (!int.TryParse(txtnParcelas.Text, out int numeroParcelas) || numeroParcelas <= 0)
            {
                DIV_Lancamentos.Visible = false;
                return;
            }

            for (int i = 1; i <= numeroParcelas; i++)
            {
                bs_LancamentoModal.Add(new cls_LancamentoPag
                {
                    nParcelaLancamentoPag = i,
                    dtLancamentoPag = "",
                    sFuncao = "INSERIR_LANCAMENTO"
                });
            }

            DIV_Lancamentos.Visible = true;
            dtgLancamentoModal_DataBind();
        }

        void dtgLancamentoModal_DataBind()
        {
            dtgLancamentoModal.DataSource = bs_LancamentoModal;
            dtgLancamentoModal.DataBind();
        }

        void SincronizarDatasParcelasModal()
        {
            int idx = 0;
            foreach (GridViewRow row in dtgLancamentoModal.Rows)
            {
                if (idx >= bs_LancamentoModal.Count) break;
                var txt = (TextBox)row.FindControl("txtdtLancamentoPag");
                if (txt != null)
                    bs_LancamentoModal[idx].dtLancamentoPag = txt.Text.Trim();
                idx++;
            }
        }

        bool ValidarParcelasModal(out string mensagem)
        {
            mensagem = "";
            if (ddlidMeioPagamento.SelectedValue != "2")
                return true;

            if (!int.TryParse(txtnParcelas.Text, out int numeroParcelas) || numeroParcelas <= 0)
            {
                mensagem = "Informe a quantidade de parcelas.";
                return false;
            }

            SincronizarDatasParcelasModal();
            if (bs_LancamentoModal.Count != numeroParcelas)
            {
                mensagem = "Quantidade de parcelas inconsistente. Informe novamente o n\u00famero de parcelas.";
                return false;
            }

            foreach (var parcela in bs_LancamentoModal)
            {
                if (string.IsNullOrWhiteSpace(parcela.dtLancamentoPag))
                {
                    mensagem = "Preencha a data de vencimento de todas as parcelas.";
                    return false;
                }
            }

            return true;
        }

        static string NormalizarDataParaSql(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return data;
            return data.Trim().Replace("-", "/");
        }

        Dictionary<string, string> MontarParametrosSalvarContasPagar(
            DataRow row, string idPagamento, string sCodigoLote, string sObservacaoCp,
            decimal valorTitulo, string dtVencimentoTitulo, string sQuantidadeParcela, int nParcelas)
        {
            string sValor = valorTitulo.ToString(CultureInfo.InvariantCulture);
            return new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR" },
                { "@idColaborador", row["idColaborador"].ToString() },
                { "@idEmpresa", row["idEmpresa"].ToString() },
                { "@nSaldo", sValor },
                { "@nValorOriginal", sValor },
                { "@nValorBruto", sValor },
                { "@dtEmissao", NormalizarDataParaSql(row["dtEmissao"].ToString()) },
                { "@dtVencimento", NormalizarDataParaSql(dtVencimentoTitulo) },
                { "@sCodigo", sCodigoLote },
                { "@sDocumento", row["sReferencia"].ToString() },
                { "@sQuantidadeParcela", sQuantidadeParcela },
                { "@nParcelas", nParcelas.ToString() },
                { "@idCategoriaPagar", ddlidCategoriaPagar.SelectedValue },
                { "@idFormaPagamento", ddlidFormaPagamento.SelectedValue },
                { "@idMeioPagamento", ddlidMeioPagamento.SelectedValue },
                { "@idContabil", ddlidContabil.SelectedValue },
                { "@idCentroDeCusto", "0" },
                { "@idRegistroCategoria", "0" },
                { "@dtApuracao", NormalizarDataParaSql(row["dtEmissao"].ToString()) },
                { "@sObservacaoGeral", sObservacaoCp },
                { "@idPagamentoColaboradores", idPagamento },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
            };
        }

        bool SalvarTituloContasPagar(Dictionary<string, string> vCp, string idColaborador, out string idContasPagar, out string sErro)
        {
            idContasPagar = "";
            DataSet dsCp = BD.ExecutarDataSet(sProcedureContasPagar, vCp);
            if (!BD.ValidarDataSet(dsCp, out sErro))
            {
                sErro = "Erro ao gerar Contas a Pagar para o colaborador ID " + idColaborador + ": " + sErro;
                return false;
            }

            idContasPagar = RETORNO.DATASET(dsCp, 0, "idContasPagar");
            if (string.IsNullOrEmpty(idContasPagar) || idContasPagar == "0")
            {
                sErro = "Erro ao gerar Contas a Pagar para o colaborador ID " + idColaborador + ".";
                return false;
            }

            return true;
        }

        void ReabrirModalGerarCP()
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "js_ReopenModalGerar",
                "limparBackdropModal(); $('#modalGerarCP').modal({ backdrop: 'static', keyboard: false, show: true }); setTimeout(inicializarChosenModalGerarCP, 200);", true);
        }

        protected void btnConfirmarGerarCP_Click(object sender, EventArgs e)
        {
            if (!FUNCOES.ValidaPermissao(Permissao.RRHH.PagamentosColaboradores.GerarContasPagar, false))
            {
                MensagemPaginaModal.MostraMensagem_Erro("Sem permiss\u00e3o para gerar Contas a Pagar.");
                ReabrirModalGerarCP();
                return;
            }

            string idPagamento = hddidPagamentoGerar.Value;
            if (string.IsNullOrEmpty(idPagamento) || idPagamento == "0")
                idPagamento = ViewState["idPagamentoGerar"]?.ToString();
            if (string.IsNullOrEmpty(idPagamento) || idPagamento == "0")
            {
                MensagemPaginaModal.MostraMensagem_Erro("Pagamento n\u00e3o selecionado.");
                ReabrirModalGerarCP();
                return;
            }

            if (hddTipoCategoria.Value != "4")
            {
                MensagemPaginaModal.MostraMensagem_Erro("Selecione uma categoria do tipo Internos.");
                ReabrirModalGerarCP();
                return;
            }

            if (ddlidFormaPagamento.SelectedValue == "0" || ddlidMeioPagamento.SelectedValue == "0" || ddlidContabil.SelectedValue == "0")
            {
                MensagemPaginaModal.MostraMensagem_Erro("Preencha forma de pagamento, tipo de pagamento e c\u00f3digo cont\u00e1bil.");
                ReabrirModalGerarCP();
                return;
            }

            if (!ValidarParcelasModal(out string msgParcelas))
            {
                MensagemPaginaModal.MostraMensagem_Erro(msgParcelas);
                ReabrirModalGerarCP();
                return;
            }

            bool isParcelado = ddlidMeioPagamento.SelectedValue == "2";
            int nParcelas = isParcelado ? bs_LancamentoModal.Count : 1;

            try
            {
                var vValidar = new Dictionary<string, string>
                {
                    { "@sFuncao", "VALIDAR_GERACAO_CP" },
                    { "@idPagamento", idPagamento },
                    { "@idCategoriaPagar", ddlidCategoriaPagar.SelectedValue }
                };

                DataSet dsValidar = BD.ExecutarDataSet(sProcedure, vValidar);
                string sErro;
                if (!BD.ValidarDataSet(dsValidar, out sErro))
                {
                    MensagemPaginaModal.MostraMensagem_Erro(sErro);
                    ReabrirModalGerarCP();
                    return;
                }

                if (dsValidar.Tables.Count < 2 || dsValidar.Tables[1].Rows.Count == 0)
                {
                    MensagemPaginaModal.MostraMensagem_Erro("N\u00e3o existem colaboradores com valor para gerar Contas a Pagar.");
                    ReabrirModalGerarCP();
                    return;
                }

                var sbXml = new StringBuilder("<Itens>");
                var idsGerados = new List<string>();

                foreach (DataRow row in dsValidar.Tables[1].Rows)
                {
                    string idRegistro = row["idRegistro"].ToString();
                    string idColaborador = row["idColaborador"].ToString();
                    string sObservacao = row["sObservacao"]?.ToString() ?? "";
                    string sObservacaoCp = MontarObservacaoContasPagar(sObservacao, idPagamento);
                    decimal nValor = Convert.ToDecimal(row["nValorPagamento"]);
                    string sCodigoLote = idPagamento.PadLeft(6, '0');
                    string idContasPagarVinculo = null;

                    if (isParcelado)
                    {
                        decimal valorParcela = Math.Round(nValor / nParcelas, 2);
                        decimal valorUltimaParcela = nValor - (valorParcela * (nParcelas - 1));

                        for (int p = 1; p <= nParcelas; p++)
                        {
                            decimal valorAtual = p == nParcelas ? valorUltimaParcela : valorParcela;
                            string dtVencParcela = bs_LancamentoModal[p - 1].dtLancamentoPag;
                            string sQtdParcela = p + " de " + nParcelas;

                            var vCp = MontarParametrosSalvarContasPagar(
                                row, idPagamento, sCodigoLote, sObservacaoCp,
                                valorAtual, dtVencParcela, sQtdParcela, nParcelas);

                            if (!SalvarTituloContasPagar(vCp, idColaborador, out string idContasPagar, out sErro))
                            {
                                MensagemPaginaModal.MostraMensagem_Erro(sErro);
                                ReabrirModalGerarCP();
                                return;
                            }

                            RegistrarHistoricoContasPagar(idContasPagar, idPagamento);
                            idsGerados.Add(idContasPagar);
                            if (p == 1)
                                idContasPagarVinculo = idContasPagar;
                        }
                    }
                    else
                    {
                        var vCp = MontarParametrosSalvarContasPagar(
                            row, idPagamento, sCodigoLote, sObservacaoCp,
                            nValor, row["dtVencimento"].ToString(), "\u00danica", 1);

                        if (!SalvarTituloContasPagar(vCp, idColaborador, out string idContasPagar, out sErro))
                        {
                            MensagemPaginaModal.MostraMensagem_Erro(sErro);
                            ReabrirModalGerarCP();
                            return;
                        }

                        RegistrarHistoricoContasPagar(idContasPagar, idPagamento);
                        idContasPagarVinculo = idContasPagar;
                        idsGerados.Add(idContasPagar);
                    }

                    sbXml.AppendFormat("<Item idRegistro=\"{0}\" idContasPagar=\"{1}\" />", idRegistro, idContasPagarVinculo);
                }
                sbXml.Append("</Itens>");

                var vFinalizar = new Dictionary<string, string>
                {
                    { "@sFuncao", "FINALIZAR_GERACAO_CP" },
                    { "@idPagamento", idPagamento },
                    { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() },
                    { "@sItensXML", sbXml.ToString() }
                };

                DataSet dsFinal = BD.ExecutarDataSet(sProcedure, vFinalizar);
                if (!BD.ValidarDataSet(dsFinal, out sErro))
                {
                    MensagemPaginaModal.MostraMensagem_Erro("T\u00edtulos gerados, mas falha ao finalizar o lote: " + sErro);
                    ReabrirModalGerarCP();
                    return;
                }

                var sbLinks = new StringBuilder("Contas a Pagar gerados com sucesso!<br/>");
                foreach (string idCp in idsGerados)
                    sbLinks.AppendFormat("<a href='/App/Paginas/Adm/Financeiro/ContasPagar_Detalhe.aspx?id={0}' target='_blank'>T\u00edtulo {0}</a><br/>", idCp);

                Session["GeradoCPComSucesso"] = true;
                Session["GeradoCPMensagem"] = sbLinks.ToString();
                Response.Redirect(Request.RawUrl);
            }
            catch (Exception ex)
            {
                MensagemPaginaModal.MostraMensagem_Erro("Erro ao gerar Contas a Pagar: " + ex.Message);
                ReabrirModalGerarCP();
            }
        }

        static string MontarObservacaoContasPagar(string sObservacaoLote, string idPagamento)
        {
            string prefixo = "via Pagamento de Colaboradores - Lote " + idPagamento;
            if (string.IsNullOrWhiteSpace(sObservacaoLote))
                return prefixo;
            if (sObservacaoLote.StartsWith("via Pagamento de Colaboradores", StringComparison.OrdinalIgnoreCase))
                return sObservacaoLote;
            return prefixo + Environment.NewLine + sObservacaoLote;
        }

        const string DetalheHistoricoInclusaoPagamentoColaboradores = "Título incluído via Pagamento de Colaboradores - Lote ";

        static void RegistrarHistoricoContasPagar(string idContasPagar, string idPagamento)
        {
            string sDetalhe = DetalheHistoricoInclusaoPagamentoColaboradores + idPagamento;
            string idUsuario = IDENTITY.Variaveis.idUsuario();

            var vHistorico = new Dictionary<string, string>
            {
                { "@sFuncao", "REGISTRAR_HISTORICO_INCLUSAO_PAGAMENTO_COLABORADORES" },
                { "@idContasPagar", idContasPagar },
                { "@idPagamentoColaboradores", idPagamento },
                { "@idUsuarioAtualizacao", idUsuario }
            };

            try
            {
                DataSet dsHistorico = BD.ExecutarDataSet(sProcedureContasPagar, vHistorico);
                if (BD.ValidarDataSet(dsHistorico, out _))
                    return;
            }
            catch
            {
                // Patch SQL pode não estar aplicado — tenta fallback direto na tabela de log.
            }

            try
            {
                AtualizarHistoricoInclusaoContasPagarFallback(idContasPagar, idUsuario, sDetalhe);
            }
            catch
            {
                // Histórico complementar; não interrompe a geração dos títulos.
            }
        }

        static void AtualizarHistoricoInclusaoContasPagarFallback(string idContasPagar, string idUsuario, string sDetalhe)
        {
            if (!int.TryParse(idContasPagar, out int idCp) || idCp <= 0)
                return;
            if (!int.TryParse(idUsuario, out int idUsr) || idUsr <= 0)
                return;

            if (sDetalhe.Length > 500)
                sDetalhe = sDetalhe.Substring(0, 500);

            using (var conn = new SqlConnection(BD.StringDeConexao))
            {
                conn.Open();

                int? idLog = null;
                using (var cmdSel = new SqlCommand(@"
                    SELECT TOP 1 idLog
                    FROM tbl_Flow_Adm_Contas_Pagar_Log (NOLOCK)
                    WHERE idContasPagar = @idContasPagar AND sAcao = N'Inserção'
                    ORDER BY dtAcao DESC, idLog DESC", conn))
                {
                    cmdSel.Parameters.Add("@idContasPagar", SqlDbType.Int).Value = idCp;
                    object o = cmdSel.ExecuteScalar();
                    if (o != null && o != DBNull.Value)
                        idLog = Convert.ToInt32(o);
                }

                if (idLog.HasValue)
                {
                    using (var cmdUpd = new SqlCommand(@"
                        UPDATE tbl_Flow_Adm_Contas_Pagar_Log
                        SET sObservacao = @sObservacao
                        WHERE idLog = @idLog", conn))
                    {
                        cmdUpd.Parameters.Add("@sObservacao", SqlDbType.NVarChar, 500).Value = sDetalhe;
                        cmdUpd.Parameters.Add("@idLog", SqlDbType.Int).Value = idLog.Value;
                        cmdUpd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (var cmdIns = new SqlCommand(@"
                        INSERT INTO tbl_Flow_Adm_Contas_Pagar_Log
                            (idContasPagar, dtAcao, idUsuario, sAcao, sObservacao)
                        VALUES
                            (@idContasPagar, GETDATE(), @idUsuario, N'Inserção', @sObservacao)", conn))
                    {
                        cmdIns.Parameters.Add("@idContasPagar", SqlDbType.Int).Value = idCp;
                        cmdIns.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsr;
                        cmdIns.Parameters.Add("@sObservacao", SqlDbType.NVarChar, 500).Value = sDetalhe;
                        cmdIns.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
