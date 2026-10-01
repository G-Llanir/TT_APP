using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
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
    [Serializable]
    public class ItemPagamentoColaborador
    {
        public int idColaborador { get; set; }
        public string sDscColaborador { get; set; }
        public string sDscEmpresa { get; set; }
        public string sDscDepartamento { get; set; }
        public string sDscCargo { get; set; }
        public string sDscTipoContrato { get; set; }
        public decimal nValor { get; set; }

        public static ItemPagamentoColaborador FromDataRow(DataRow row)
        {
            return new ItemPagamentoColaborador
            {
                idColaborador = Convert.ToInt32(row["idColaborador"]),
                sDscColaborador = row.Table.Columns.Contains("sDscColaborador") ? row["sDscColaborador"]?.ToString() ?? "" : "",
                sDscEmpresa = row.Table.Columns.Contains("sDscEmpresa") ? row["sDscEmpresa"]?.ToString() ?? "" : "",
                sDscDepartamento = row.Table.Columns.Contains("sDscDepartamento") ? row["sDscDepartamento"]?.ToString() ?? "" : "",
                sDscCargo = row.Table.Columns.Contains("sDscCargo") ? row["sDscCargo"]?.ToString() ?? "" : "",
                sDscTipoContrato = row.Table.Columns.Contains("sDscTipoContrato") ? row["sDscTipoContrato"]?.ToString() ?? "" : "",
                nValor = row.Table.Columns.Contains("nValorPagamento")
                    ? Convert.ToDecimal(row["nValorPagamento"])
                    : (row.Table.Columns.Contains("nValor") ? Convert.ToDecimal(row["nValor"]) : 0)
            };
        }
    }

    public partial class Colaboradores_Pagamento_Detalhe : Page
    {
        private const string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Pagamento";
        private const string sProcedureColab = "sp_Manipula_tbl_Flow_Colaboradores";
        private const string sTituloPagina = "Pagamentos de Colaboradores";
        private const int ColunaSelecaoGrid = 0;
        private const int ColunaOrdenarColaboradorBusca = 2;

        private static readonly JavaScriptSerializer JsonSerializer = new JavaScriptSerializer();

        private List<ItemPagamentoColaborador> ItensPagamento
        {
            get
            {
                if (ViewState["ItensPagamento"] == null)
                    ViewState["ItensPagamento"] = new List<ItemPagamentoColaborador>();
                return (List<ItemPagamentoColaborador>)ViewState["ItensPagamento"];
            }
            set { ViewState["ItensPagamento"] = value; }
        }

        private string ChaveSessionBuscaColaboradores => "ColabPag_Busca_" + (hddidPagamento?.Value ?? "0");

        private DataTable UltimaBuscaColaboradores
        {
            get { return Session[ChaveSessionBuscaColaboradores] as DataTable; }
            set { Session[ChaveSessionBuscaColaboradores] = value; }
        }

        void PersistirItensPagamento()
        {
            var copia = ItensPagamento.Select(x => new ItemPagamentoColaborador
            {
                idColaborador = x.idColaborador,
                sDscColaborador = x.sDscColaborador ?? "",
                sDscEmpresa = x.sDscEmpresa ?? "",
                sDscDepartamento = x.sDscDepartamento ?? "",
                sDscCargo = x.sDscCargo ?? "",
                sDscTipoContrato = x.sDscTipoContrato ?? "",
                nValor = x.nValor
            }).ToList();

            ItensPagamento = copia;
            hddItensPagamento.Value = JsonSerializer.Serialize(copia);
        }

        void RestaurarItensPagamentoNoPostBack()
        {
            if (!IsPostBack) return;

            List<ItemPagamentoColaborador> doHidden = null;
            if (!string.IsNullOrWhiteSpace(hddItensPagamento.Value))
            {
                try
                {
                    doHidden = JsonSerializer.Deserialize<List<ItemPagamentoColaborador>>(hddItensPagamento.Value);
                }
                catch { }
            }

            var doViewState = ViewState["ItensPagamento"] as List<ItemPagamentoColaborador>;
            int qtdHidden = doHidden?.Count ?? 0;
            int qtdViewState = doViewState?.Count ?? 0;

            if (qtdHidden >= qtdViewState)
                ItensPagamento = doHidden ?? new List<ItemPagamentoColaborador>();
            else
                ItensPagamento = doViewState ?? new List<ItemPagamentoColaborador>();
        }

        static decimal ParseValorMonetario(string sValor)
        {
            if (string.IsNullOrWhiteSpace(sValor)) return 0;

            sValor = sValor.Trim();
            if (decimal.TryParse(sValor, NumberStyles.Number, CultureInfo.GetCultureInfo("pt-BR"), out decimal nValor))
                return nValor;

            sValor = sValor.Replace(".", "");
            if (decimal.TryParse(sValor, NumberStyles.Number, CultureInfo.GetCultureInfo("pt-BR"), out nValor))
                return nValor;

            if (decimal.TryParse(sValor, NumberStyles.Any, CultureInfo.InvariantCulture, out nValor))
                return nValor;

            return 0;
        }

        void RegistrarScriptsScroll()
        {
            if (Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ColabPagScrollJs"))
                return;

            string js = @"
window.ColabPag_salvarScroll = function() {
    var h = document.getElementById('" + hddScrollY.ClientID + @"');
    if (!h) return;
    h.value = String(window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0);
};
window.ColabPag_restaurarScroll = function() {
    var h = document.getElementById('" + hddScrollY.ClientID + @"');
    if (!h || h.value === '') return;
    var y = parseInt(h.value, 10);
    if (isNaN(y)) return;
    var f = function() { window.scrollTo(0, y); };
    f();
    setTimeout(f, 0);
    setTimeout(f, 80);
    setTimeout(f, 200);
    setTimeout(f, 450);
};
(function() {
    function hookPrm() {
        if (!window.Sys || !Sys.WebForms || !Sys.WebForms.PageRequestManager) return;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        if (prm._colabPagScrollHook) return;
        prm._colabPagScrollHook = true;
        prm.add_beginRequest(function() {
            if (window.ColabPag_salvarScroll) window.ColabPag_salvarScroll();
        });
        prm.add_endRequest(function() {
            if (window.ColabPag_restaurarScroll) window.ColabPag_restaurarScroll();
        });
    }
    if (window.Sys && Sys.Application) Sys.Application.add_load(hookPrm);
    else hookPrm();
})();";

            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "ColabPagScrollJs", js, true);
        }

        void RegistrarRestaurarScroll()
        {
            ScriptManager.RegisterStartupScript(UpdGeral, UpdGeral.GetType(), "ColabPagRestScroll",
                "if (window.ColabPag_restaurarScroll) window.ColabPag_restaurarScroll();", true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RegistrarScriptsScroll();

            bool somenteLeitura = hddSomenteLeitura.Value == "S";

            if (hddidPagamento.Value == "0")
                FUNCOES.ValidaPermissao(Permissao.RRHH.PagamentosColaboradores.Incluir, true);
            else if (somenteLeitura)
                FUNCOES.ValidaPermissao(Permissao.RRHH.PagamentosColaboradores.Consultar, true);
            else
                FUNCOES.ValidaPermissao(Permissao.RRHH.PagamentosColaboradores.Alterar, true);

            if (!IsPostBack)
            {
                string id = Request["id"] ?? "0";
                bool visualizar = Request["modo"] == "visualizar";
                hddidPagamento.Value = id;

                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                PopularCombos();

                if (id == "0")
                {
                    FUNCOES.ValidaPermissao(Permissao.RRHH.PagamentosColaboradores.Incluir, true);
                    lblSubTituloPagina.Text = " Novo";
                    txtdtEmissao.Text = DateTime.Today.ToString("yyyy-MM-dd");
                    txtdtVencimento.Text = DateTime.Today.ToString("yyyy-MM-dd");
                    txtsSituacao.Text = "Em digita\u00e7\u00e3o";
                    ItensPagamento = new List<ItemPagamentoColaborador>();
                    PersistirItensPagamento();
                    BindGridItensPagamento();
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.RRHH.PagamentosColaboradores.Consultar, true);
                    CarregarPagamento(id, visualizar);
                }

                if (Session["SalvoComSucesso"] != null && (bool)Session["SalvoComSucesso"])
                {
                    MensagemPagina.MostraMensagem_Sucesso("Pagamento salvo com sucesso!");
                    Session["SalvoComSucesso"] = false;
                }

                if (hddSomenteLeitura.Value != "S")
                    RegistrarChosenAtualizado();
            }
            else
            {
                RestaurarItensPagamentoNoPostBack();
                PreRender += GarantirGridItensPagamentoVisivel;
                // Não fazer DataBind aqui: no postback isso recria os TextBoxes depois do
                // ProcessPostData e os valores digitados em gvItensPagamento são perdidos.
                if (!somenteLeitura)
                    RegistrarChosenAtualizado();
            }

            cmdSalvar.Visible = !somenteLeitura && hddSomenteLeitura.Value != "S"
                && (hddidPagamento.Value == "0"
                    ? FUNCOES.ValidaPermissao(Permissao.RRHH.PagamentosColaboradores.Incluir, false)
                    : FUNCOES.ValidaPermissao(Permissao.RRHH.PagamentosColaboradores.Alterar, false));

            DIV_Filtro.Visible = hddSomenteLeitura.Value != "S";
            pnColaboradores.Visible = hddSomenteLeitura.Value != "S" && pnColaboradores.Visible;
            AplicarModoSomenteLeitura(hddSomenteLeitura.Value == "S");
        }

        static string ObterReferenciaDaEmissao(string dtEmissao)
        {
            if (!DateTime.TryParse(dtEmissao, out DateTime dt))
                dt = DateTime.Today;
            return dt.ToString("MM/yyyy", CultureInfo.InvariantCulture);
        }

        void CarregarPagamento(string id, bool forcarVisualizar)
        {
            var vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idPagamento", id }
            };
            DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);
            if (!BD.ValidarDataSet(ds) || ds.Tables[0].Rows.Count == 0)
            {
                MensagemPagina.MostraMensagem_Erro("Pagamento não encontrado.");
                return;
            }

            DataRow cab = ds.Tables[0].Rows[0];
            string idSituacao = cab["idSituacao"].ToString();
            bool somenteLeitura = forcarVisualizar || idSituacao != "1";

            if (idSituacao != "1" && !forcarVisualizar)
                FUNCOES.ValidaPermissao(Permissao.RRHH.PagamentosColaboradores.Consultar, true);
            else if (!somenteLeitura)
                FUNCOES.ValidaPermissao(Permissao.RRHH.PagamentosColaboradores.Alterar, true);

            hddSomenteLeitura.Value = somenteLeitura ? "S" : "N";
            lblSubTituloPagina.Text = somenteLeitura ? " Visualiza\u00e7\u00e3o" : " Edi\u00e7\u00e3o";

            string sDscTipo = cab["sDscTipo"]?.ToString() ?? "";
            string sDscEmpresa = cab["sDscEmpresa"]?.ToString() ?? "";
            string idTipo = cab["idTipo"].ToString();
            string idEmpresa = cab["idEmpresa"] != DBNull.Value ? cab["idEmpresa"].ToString() : "0";

            ddlidTipo.SelectedValue = idTipo;
            txtsDscTipo.Text = sDscTipo;
            txtdtEmissao.Text = cab["dtEmissao"].ToString();
            txtdtVencimento.Text = cab["dtVencimento"].ToString();
            if (idEmpresa != "0")
            {
                GarantirItemComboEmpresa(idEmpresa, sDscEmpresa);
                ddlidEmpresa.SelectedValue = idEmpresa;
            }
            txtsDscEmpresa.Text = sDscEmpresa;
            txtsSituacao.Text = cab["sDscSituacao"].ToString();
            txtsObservacao.Text = cab["sObservacao"]?.ToString() ?? "";

            PopularCombo_GHE(idEmpresa != "0" ? idEmpresa : "0");

            CarregarItensPagamentoDoBanco(ds);

            string dtAtualizacao = cab.Table.Columns.Contains("dtAtualizacao") ? cab["dtAtualizacao"]?.ToString() : null;
            string sUsuarioAtualizacao = cab.Table.Columns.Contains("sDscUsuarioAtualizacao") ? cab["sDscUsuarioAtualizacao"]?.ToString() : null;
            if (string.IsNullOrWhiteSpace(dtAtualizacao))
                dtAtualizacao = cab["dtInclusao"]?.ToString();
            if (string.IsNullOrWhiteSpace(sUsuarioAtualizacao))
                sUsuarioAtualizacao = cab["sDscUsuarioInclusao"]?.ToString();

            if (!string.IsNullOrWhiteSpace(dtAtualizacao) || !string.IsNullOrWhiteSpace(sUsuarioAtualizacao))
            {
                PainelAtualizacao.Visible = true;
                PainelAtualizacao.Atualizar(dtAtualizacao ?? "", sUsuarioAtualizacao ?? "");
            }

            AplicarModoSomenteLeitura(somenteLeitura);
            BindGridItensPagamento();

            if (!somenteLeitura)
                BuscarColaboradores();
        }

        void CarregarItensPagamentoDoBanco(DataSet ds = null)
        {
            var lista = new List<ItemPagamentoColaborador>();

            if (ds == null && hddidPagamento.Value != "0")
            {
                var vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idPagamento", hddidPagamento.Value }
                };
                ds = BD.ExecutarDataSet(sProcedure, vParametros);
            }

            if (ds != null && BD.ValidarDataSet(ds) && ds.Tables.Count > 1)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                    lista.Add(ItemPagamentoColaborador.FromDataRow(row));
            }

            ItensPagamento = lista;
            PersistirItensPagamento();
        }

        void GarantirItemComboEmpresa(string idEmpresa, string sDscEmpresa)
        {
            if (ddlidEmpresa.Items.FindByValue(idEmpresa) != null) return;
            ddlidEmpresa.Items.Add(new ListItem(sDscEmpresa, idEmpresa));
        }

        void AplicarModoSomenteLeitura(bool somenteLeitura)
        {
            if (!somenteLeitura)
            {
                if (gvItensPagamento.Columns.Count > ColunaSelecaoGrid)
                    gvItensPagamento.Columns[ColunaSelecaoGrid].Visible = true;
                pnRemoverSelecionados.Visible = true;
                return;
            }

            ddlidTipo.Visible = false;
            txtsDscTipo.Visible = true;
            ddlidEmpresa.Visible = false;
            txtsDscEmpresa.Visible = true;
            txtdtEmissao.ReadOnly = true;
            txtdtVencimento.ReadOnly = true;
            txtsObservacao.ReadOnly = true;

            if (gvItensPagamento.Columns.Count > ColunaSelecaoGrid)
                gvItensPagamento.Columns[ColunaSelecaoGrid].Visible = false;
            pnRemoverSelecionados.Visible = false;
        }

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidTipo, "sp_Select 'tbl_Flow_Colaboradores_Pagamento_Tipo'", "idTipo", "sDscTipo", false, "Selecione o Tipo", "0");
            FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
            FUNCOES.Popula_Combo(ddlidDepartamento, "sp_Select 'Flow_Colaboradores_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Todos os Departamentos", "0");
            FUNCOES.Popula_Combo(ddlidCargo, "sp_Select 'Flow_Colaboradores_Cargos'", "idCargo", "sDscCargo", false, "Todos os Cargos", "0");
            FUNCOES.Popula_Combo(ddlidTipoContrato, "sp_Select 'tbl_Flow_Colaboradores_TipoContrato'", "idTipoContrato", "sDscTipoContrato", false, "Todos os Tipos de Contratos", "0");
            FUNCOES.Popula_Combo(ddlsCBO, "sp_Select 'Flow_Colaboradores_FuncaoCarteira_sCBO'", "sCBO", "sCBO", false, "Todos os CBO", "0");
            FUNCOES.Popula_Combo(ddlidSupervisorDireto, "sp_Select 'RRHH_SUPERVISORES'", "idColaborador", "sDscColaborador", false, "Todos os Supervisores", "0");
            FUNCOES.Popula_Combo(ddlidFuncao, "sp_Manipula_tbl_Flow_Colaboradores_FuncaoCarteira @sFuncao='Flow-Funcoes'", "idFuncao", "sDscFuncao", false, "Todas Funções", "0");
            PopularCombo_GHE(ddlidEmpresa.SelectedValue);
        }

        protected void PopularCombo_GHE(string idEmpresa)
        {
            FUNCOES.Popula_Combo(ddlidGHE, "sp_Manipula_tbl_Flow_Colaboradores_GHE 'SELECT_GHE', @sidEmpresa=" + idEmpresa, "idGHE", "sDscGHE", false, "Todos os GHEs", "0");
        }

        protected void ddlidEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            SincronizarValoresDaGridItens();
            PopularCombo_GHE(ddlidEmpresa.SelectedValue);
            BindGridItensPagamento();
        }

        protected void cmdBuscarColaboradores_Click(object sender, EventArgs e)
        {
            if (ddlidEmpresa.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione a empresa no cabeçalho antes de buscar colaboradores.");
                return;
            }

            SincronizarValoresDaGridItens();
            BuscarColaboradores();
            BindGridItensPagamento();
        }

        void BuscarColaboradores()
        {
            try
            {
                if (ddlidEmpresa.SelectedValue == "0")
                {
                    pnColaboradores.Visible = false;
                    UltimaBuscaColaboradores = null;
                    return;
                }

                DataTable tb = ExecutarConsultaColaboradores();
                NormalizarColunasColaboradores(tb);

                if (tb.Rows.Count == 0)
                {
                    pnColaboradores.Visible = false;
                    UltimaBuscaColaboradores = null;
                    MensagemPagina.MostraMensagem_Erro("Nenhum colaborador localizado. Tente situação \"Todos\" ou confira os filtros.");
                    return;
                }

                if (tb.Columns.Contains("ret") && tb.Rows[0]["ret"].ToString() == "1")
                {
                    pnColaboradores.Visible = false;
                    UltimaBuscaColaboradores = null;
                    string msg = tb.Columns.Contains("msg") ? tb.Rows[0]["msg"].ToString() : "Erro ao consultar colaboradores.";
                    MensagemPagina.MostraMensagem_Erro(msg);
                    return;
                }

                UltimaBuscaColaboradores = tb.Copy();
                BindGridColaboradores(tb);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao buscar colaboradores: " + ex.Message);
            }
        }

        void BindGridColaboradores(DataTable tb)
        {
            pnColaboradores.Visible = true;
            string script = Grid.DataBindComScriptData(gvColaboradores, tb, ColunaOrdenarColaboradorBusca, new int[0], "asc", "false", "''");
            script = script.Replace(
                "$('#" + gvColaboradores.ClientID + "').DataTable({",
                "$('#" + gvColaboradores.ClientID + "').DataTable({ columnDefs: [{ orderable: false, targets: 0 }],");
            script += ";if(window.ColabPag_restaurarScroll){window.ColabPag_restaurarScroll();}";
            ScriptManager.RegisterStartupScript(UpdGeral, UpdGeral.GetType(), "DataTables_Colaboradores_" + gvColaboradores.ClientID, script, true);
        }

        void BindGridItensPagamento(bool aplicarMascara = true)
        {
            gvItensPagamento.DataSource = ItensPagamento.OrderBy(x => x.sDscColaborador).ToList();
            gvItensPagamento.DataBind();

            AplicarModoSomenteLeitura(hddSomenteLeitura.Value == "S");
            AtualizarResumoItens();

            if (hddSomenteLeitura.Value == "S" || !aplicarMascara)
                return;

            string script = InjetarMascaraValorGrid(gvItensPagamento.ClientID, lblResumoItens.ClientID);
            ScriptManager.RegisterStartupScript(UpdGeral, UpdGeral.GetType(), "MascaraItensPag_" + gvItensPagamento.ClientID, script, true);
        }

        void AtualizarResumoItens()
        {
            int qtd = ItensPagamento?.Count ?? 0;
            decimal total = CalcularTotalValoresInformados();
            lblResumoItens.Text = string.Format(CultureInfo.CurrentCulture,
                "{0} colaborador(es) adicionado(s) | Total dos valores informados: {1:N2}", qtd, total);
        }

        decimal CalcularTotalValoresInformados()
        {
            if (ItensPagamento == null || ItensPagamento.Count == 0)
                return 0;

            var idsOrdenados = ItensPagamento.OrderBy(x => x.sDscColaborador).Select(x => x.idColaborador).ToList();

            if (IsPostBack)
            {
                var valoresPostados = ExtrairValoresMonetariosPostadosGridItens();
                if (valoresPostados.Count > 0)
                {
                    decimal totalPost = 0;
                    for (int i = 0; i < idsOrdenados.Count && i < valoresPostados.Count; i++)
                        totalPost += ParseValorMonetario(valoresPostados[i]);

                    if (totalPost > 0 || valoresPostados.Any(v => !string.IsNullOrWhiteSpace(v)))
                        return totalPost;
                }
            }

            if (gvItensPagamento.Rows.Count > 0)
            {
                decimal totalGrid = 0;
                foreach (GridViewRow row in gvItensPagamento.Rows)
                {
                    if (row.RowType != DataControlRowType.DataRow) continue;
                    var txt = (TextBox)row.FindControl("txtnValor");
                    if (txt == null) continue;
                    string raw = IsPostBack ? ObterValorPostado(txt) : txt.Text;
                    totalGrid += ParseValorMonetario(raw);
                }
                if (totalGrid > 0)
                    return totalGrid;
            }

            return ItensPagamento.Sum(x => x.nValor);
        }

        List<string> ExtrairValoresMonetariosPostadosGridItens()
        {
            var itens = new List<(int seq, string valor)>();

            foreach (string key in Request.Form.AllKeys)
            {
                if (string.IsNullOrEmpty(key)) continue;
                if (key.IndexOf("txtnValor", StringComparison.OrdinalIgnoreCase) < 0) continue;
                if (key.IndexOf(gvItensPagamento.ID, StringComparison.OrdinalIgnoreCase) < 0) continue;

                var match = Regex.Match(key, @"ctl(\d+)", RegexOptions.IgnoreCase);
                int seq = match.Success ? int.Parse(match.Groups[1].Value) : itens.Count + 1000;
                itens.Add((seq, Request.Form[key]));
            }

            return itens.OrderBy(x => x.seq).Select(x => x.valor).ToList();
        }

        static string InjetarMascaraValorGrid(string gridClientId, string lblResumoClientId)
        {
            return @"
$(function() {
    function _parseValorBr(v) {
        if (!v) return 0;
        var n = parseFloat(String(v).replace(/\./g, '').replace(',', '.'));
        return isNaN(n) ? 0 : n;
    }
    function _atualizarResumoValoresItens() {
        var total = 0, qtd = 0;
        $('#" + gridClientId + @"').find('input.txtnValorPagamento, input[id*=txtnValor]').each(function () {
            qtd++;
            total += _parseValorBr(this.value);
        });
        var lbl = document.getElementById('" + lblResumoClientId + @"');
        if (lbl) {
            lbl.innerText = qtd + ' colaborador(es) adicionado(s) | Total dos valores informados: '
                + total.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        }
    }
    function _maskValorPagItens() {
        var $inputs = $('#" + gridClientId + @"').find('input.txtnValorPagamento, input[id*=txtnValor]');
        $inputs.off('input.valorPag keypress.valorPag change.valorPag').on('input.valorPag change.valorPag', function () {
            var v = this.value, clean = v.replace(/[^\d,]/g, '');
            if (v !== clean) this.value = clean;
            _atualizarResumoValoresItens();
        }).on('keypress.valorPag', function (e) {
            var c = e.which || e.keyCode;
            if (c <= 32 || e.ctrlKey || e.metaKey) return;
            if (!/[\d,]/.test(String.fromCharCode(c))) e.preventDefault();
        });
        if (typeof $.fn.mask === 'function') {
            $inputs.each(function () {
                var $i = $(this);
                if ($i.data('mask')) { try { $i.unmask(); } catch (e) { } }
                $i.mask('000.000.000.000.000,00', { reverse: true });
            });
        }
        _atualizarResumoValoresItens();
    }
    _maskValorPagItens();
});";
        }

        void RegistrarChosenAtualizado()
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ChosenAtualizado",
                "$('#ddlidTipo,#ddlidEmpresa').trigger('chosen:updated');", true);
        }

        Dictionary<string, string> MontarParametrosFiltroColaboradores(string sFuncao)
        {
            string sCbo = ddlsCBO.Text;
            if (string.IsNullOrEmpty(sCbo) || sCbo == "0" || sCbo == "Todos os CBO")
                sCbo = string.Empty;

            return new Dictionary<string, string>
            {
                { "@sFuncao", sFuncao },
                { "@idDepartamento", ddlidDepartamento.SelectedValue },
                { "@sSituacao", ddlsSituacao.SelectedValue },
                { "@idEmpresa", ddlidEmpresa.SelectedValue },
                { "@idCargo", ddlidCargo.SelectedValue },
                { "@idTipoContrato", ddlidTipoContrato.SelectedValue },
                { "@sPesquisa", txtPesquisa.Text.Trim() },
                { "@sCBO", sCbo },
                { "@idGHE", ddlidGHE.SelectedValue },
                { "@idSupervisorDireto", ddlidSupervisorDireto.SelectedValue },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() },
                { "@idFuncao", ddlidFuncao.SelectedValue }
            };
        }

        static bool IsRetornoErro(DataTable tb)
        {
            return tb != null && tb.Columns.Contains("ret") && tb.Rows.Count > 0 && tb.Rows[0]["ret"].ToString() == "1";
        }

        DataTable ExecutarConsultaColaboradores()
        {
            try
            {
                DataTable tbPag = BD.ExecutarDataTable(sProcedure, MontarParametrosFiltroColaboradores("CONSULTAR_COLABORADORES"));
                if (tbPag != null && tbPag.Rows.Count > 0 && !IsRetornoErro(tbPag))
                    return tbPag;
            }
            catch { }

            DataTable tbColab = BD.ExecutarDataTable(sProcedureColab, MontarParametrosFiltroColaboradores("CONSULTAR"));
            if (tbColab != null && tbColab.Rows.Count > 0 && !IsRetornoErro(tbColab))
                return tbColab;

            return tbColab ?? new DataTable();
        }

        static string ObterValorPostado(TextBox txt)
        {
            if (txt == null) return "";

            string valor = HttpContext.Current?.Request?.Form[txt.UniqueID];
            if (!string.IsNullOrWhiteSpace(valor))
                return valor;

            return txt.Text ?? "";
        }

        void GarantirGridItensPagamentoVisivel(object sender, EventArgs e)
        {
            if (ItensPagamento.Count > 0 && gvItensPagamento.Rows.Count == 0)
                BindGridItensPagamento();
        }

        void SincronizarValoresDoRequestForm(Dictionary<int, ItemPagamentoColaborador> mapa)
        {
            var idsOrdenados = ItensPagamento.OrderBy(x => x.sDscColaborador).Select(x => x.idColaborador).ToList();
            var valoresPostados = ExtrairValoresMonetariosPostadosGridItens();

            for (int i = 0; i < idsOrdenados.Count && i < valoresPostados.Count; i++)
            {
                int id = idsOrdenados[i];
                if (mapa.ContainsKey(id))
                    mapa[id].nValor = ParseValorMonetario(valoresPostados[i]);
            }
        }

        void SincronizarValoresDaGridItens()
        {
            if (ItensPagamento == null || ItensPagamento.Count == 0) return;

            var mapa = ItensPagamento.ToDictionary(x => x.idColaborador);

            if (IsPostBack)
                SincronizarValoresDoRequestForm(mapa);

            foreach (GridViewRow row in gvItensPagamento.Rows)
            {
                if (row.RowType != DataControlRowType.DataRow) continue;

                int idColaborador = Convert.ToInt32(gvItensPagamento.DataKeys[row.RowIndex].Value);
                var txtValor = (TextBox)row.FindControl("txtnValor");
                if (txtValor == null || !mapa.ContainsKey(idColaborador)) continue;

                mapa[idColaborador].nValor = ParseValorMonetario(ObterValorPostado(txtValor));
            }

            PersistirItensPagamento();
        }

        bool ColaboradorJaAdicionado(int idColaborador)
        {
            return ItensPagamento.Any(x => x.idColaborador == idColaborador);
        }

        void AdicionarColaboradorAoPagamento(int idColaborador, bool exibirMensagemUnitaria = true)
        {
            if (ColaboradorJaAdicionado(idColaborador))
            {
                if (exibirMensagemUnitaria)
                    MensagemPagina.MostraMensagem_Erro("Colaborador já está na lista do pagamento.", false);
                return;
            }

            DataRow fonte = ObterLinhaDaUltimaBusca(idColaborador);
            if (fonte == null)
            {
                if (exibirMensagemUnitaria)
                    MensagemPagina.MostraMensagem_Erro("Não foi possível localizar os dados do colaborador. Execute a busca novamente.", false);
                return;
            }

            var item = ItemPagamentoColaborador.FromDataRow(fonte);
            item.nValor = 0;
            ItensPagamento.Add(item);
            PersistirItensPagamento();
            if (exibirMensagemUnitaria)
                MensagemPagina.MostraMensagem_Sucesso("Colaborador adicionado. Informe o valor na lista acima.", false);
        }

        void RemoverColaboradorDoPagamento(int idColaborador)
        {
            ItensPagamento.RemoveAll(x => x.idColaborador == idColaborador);
            PersistirItensPagamento();
        }

        DataRow ObterLinhaDaUltimaBusca(int idColaborador)
        {
            if (UltimaBuscaColaboradores == null) return null;
            foreach (DataRow row in UltimaBuscaColaboradores.Rows)
            {
                if (Convert.ToInt32(row["idColaborador"]) == idColaborador)
                    return row;
            }
            return null;
        }

        static bool CheckBoxMarcadoNoPost(CheckBox chk)
        {
            if (chk == null) return false;
            string valor = HttpContext.Current?.Request?.Form[chk.UniqueID];
            return !string.IsNullOrEmpty(valor);
        }

        void GarantirGridBuscaDataBound()
        {
            if (UltimaBuscaColaboradores == null || UltimaBuscaColaboradores.Rows.Count == 0) return;
            gvColaboradores.DataSource = UltimaBuscaColaboradores;
            gvColaboradores.DataBind();
        }

        protected void cmdAdicionarSelecionados_Click(object sender, EventArgs e)
        {
            SincronizarValoresDaGridItens();
            GarantirGridBuscaDataBound();

            int adicionados = 0;
            foreach (GridViewRow row in gvColaboradores.Rows)
            {
                if (row.RowType != DataControlRowType.DataRow) continue;

                var chk = (CheckBox)row.FindControl("chkColab_Selecionado");
                if (chk == null || !chk.Enabled || !CheckBoxMarcadoNoPost(chk)) continue;

                int idColaborador = Convert.ToInt32(gvColaboradores.DataKeys[row.RowIndex].Value);
                if (ColaboradorJaAdicionado(idColaborador)) continue;

                AdicionarColaboradorAoPagamento(idColaborador, false);
                adicionados++;
            }

            if (adicionados == 0)
                MensagemPagina.MostraMensagem_Erro("Selecione ao menos um colaborador para adicionar.", false);
            else
                MensagemPagina.MostraMensagem_Sucesso(adicionados + " colaborador(es) adicionado(s). Informe os valores na lista acima.", false);

            BindGridItensPagamento();
            if (UltimaBuscaColaboradores != null)
                BindGridColaboradores(UltimaBuscaColaboradores);
            RegistrarRestaurarScroll();
        }

        protected void cmdRemoverSelecionados_Click(object sender, EventArgs e)
        {
            SincronizarValoresDaGridItens();
            BindGridItensPagamento();

            var idsRemover = new List<int>();
            foreach (GridViewRow row in gvItensPagamento.Rows)
            {
                if (row.RowType != DataControlRowType.DataRow) continue;

                var chk = (CheckBox)row.FindControl("chkItem_Selecionado");
                if (!CheckBoxMarcadoNoPost(chk)) continue;

                idsRemover.Add(Convert.ToInt32(gvItensPagamento.DataKeys[row.RowIndex].Value));
            }

            if (idsRemover.Count == 0)
            {
                MensagemPagina.MostraMensagem_Erro("Selecione ao menos um colaborador para remover.", false);
                return;
            }

            foreach (int id in idsRemover)
                RemoverColaboradorDoPagamento(id);

            MensagemPagina.MostraMensagem_Sucesso(idsRemover.Count + " colaborador(es) removido(s) do pagamento.", false);
            BindGridItensPagamento();

            if (UltimaBuscaColaboradores != null)
                BindGridColaboradores(UltimaBuscaColaboradores);
            RegistrarRestaurarScroll();
        }

        protected void gvColaboradores_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            int idColaborador = Convert.ToInt32(gvColaboradores.DataKeys[e.Row.RowIndex].Value);
            var chk = (CheckBox)e.Row.FindControl("chkColab_Selecionado");
            if (chk == null) return;

            if (ColaboradorJaAdicionado(idColaborador))
            {
                chk.Checked = false;
                chk.Enabled = false;
            }
        }

        protected void gvItensPagamento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            bool somenteLeitura = hddSomenteLeitura.Value == "S";

            if (e.Row.RowType == DataControlRowType.Header)
            {
                var chkHdr = (CheckBox)e.Row.FindControl("chkItens_SelecionarTudo");
                if (chkHdr != null)
                    chkHdr.Visible = !somenteLeitura;
                return;
            }

            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var txtValor = (TextBox)e.Row.FindControl("txtnValor");
            var chk = (CheckBox)e.Row.FindControl("chkItem_Selecionado");

            if (txtValor != null)
            {
                txtValor.ReadOnly = somenteLeitura;
                if (!IsPostBack || string.IsNullOrWhiteSpace(txtValor.Text))
                    txtValor.Text = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:N2}",
                        DataBinder.Eval(e.Row.DataItem, "nValor"));
            }

            if (chk != null)
                chk.Visible = !somenteLeitura;
        }

        static void NormalizarColunasColaboradores(DataTable tb)
        {
            if (tb == null) return;

            AdicionarColunaVaziaSeFaltar(tb, "sDscEmpresa");
            AdicionarColunaVaziaSeFaltar(tb, "sDscDepartamento");
            AdicionarColunaVaziaSeFaltar(tb, "sDscCargo");
            AdicionarColunaVaziaSeFaltar(tb, "sDscTipoContrato");
        }

        static void AdicionarColunaVaziaSeFaltar(DataTable tb, string nomeColuna)
        {
            if (tb.Columns.Contains(nomeColuna)) return;

            tb.Columns.Add(nomeColuna, typeof(string));
            foreach (DataRow row in tb.Rows)
                row[nomeColuna] = string.Empty;
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            if (hddSomenteLeitura.Value == "S")
            {
                MensagemPagina.MostraMensagem_Erro("Pagamento não pode ser alterado.");
                return;
            }

            if (hddidPagamento.Value == "0")
            {
                if (!FUNCOES.ValidaPermissao(Permissao.RRHH.PagamentosColaboradores.Incluir, false))
                {
                    MensagemPagina.MostraMensagem_Erro("Sem permissão para incluir.");
                    return;
                }
            }
            else if (!FUNCOES.ValidaPermissao(Permissao.RRHH.PagamentosColaboradores.Alterar, false))
            {
                MensagemPagina.MostraMensagem_Erro("Sem permissão para alterar.");
                return;
            }

            string sErro = ValidarDados();
            if (sErro != "")
            {
                MensagemPagina.MostraMensagem_Erro(sErro);
                return;
            }

            SincronizarValoresDaGridItens();

            string sXml = MontarXmlItens();
            if (sXml == null)
            {
                int qtdLista = ItensPagamento?.Count ?? 0;
                int qtdComValor = ItensPagamento?.Count(x => x.nValor > 0) ?? 0;
                MensagemPagina.MostraMensagem_Erro(
                    "Adicione pelo menos um colaborador com valor maior que zero na lista do pagamento (grid superior). " +
                    $"Itens na lista: {qtdLista}; com valor &gt; 0: {qtdComValor}.");
                BindGridItensPagamento();
                return;
            }

            try
            {
                var vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "SALVAR" },
                    { "@idPagamento", hddidPagamento.Value },
                    { "@idTipo", ddlidTipo.SelectedValue },
                    { "@sReferencia", ObterReferenciaDaEmissao(txtdtEmissao.Text) },
                    { "@dtEmissao", txtdtEmissao.Text },
                    { "@dtVencimento", txtdtVencimento.Text },
                    { "@idEmpresa", ddlidEmpresa.SelectedValue },
                    { "@sObservacao", txtsObservacao.Text.Trim() },
                    { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() },
                    { "@sItensXML", sXml }
                };

                DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (!BD.ValidarDataSet(ds, out sErro))
                {
                    MensagemPagina.MostraMensagem_Erro(sErro);
                    return;
                }

                string idPagamento = RETORNO.DATASET(ds, 0, "idPagamento");
                Session["SalvoComSucesso"] = true;
                Response.Redirect("Colaboradores_Pagamento_Detalhe.aspx?id=" + idPagamento);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao salvar: " + ex.Message);
            }
        }

        string ValidarDados()
        {
            var sb = new StringBuilder();

            if (ddlidTipo.SelectedValue == "0")
                sb.Append("Selecione o tipo de pagamento.<br/>");
            if (string.IsNullOrWhiteSpace(txtdtEmissao.Text))
                sb.Append("Informe a data de emissão.<br/>");
            if (string.IsNullOrWhiteSpace(txtdtVencimento.Text))
                sb.Append("Informe a data de vencimento.<br/>");
            if (ddlidEmpresa.SelectedValue == "0")
                sb.Append("Selecione a empresa.<br/>");

            return sb.ToString();
        }

        string MontarXmlItens()
        {
            if (ItensPagamento == null || ItensPagamento.Count == 0)
                return null;

            var sb = new StringBuilder("<Itens>");
            int count = 0;

            foreach (var item in ItensPagamento.OrderBy(x => x.idColaborador))
            {
                if (item.nValor <= 0) continue;

                sb.AppendFormat(CultureInfo.InvariantCulture,
                    "<Item idColaborador=\"{0}\" nValorPagamento=\"{1:0.####}\" />",
                    item.idColaborador, item.nValor);
                count++;
            }

            sb.Append("</Itens>");
            return count > 0 ? sb.ToString() : null;
        }
    }
}
