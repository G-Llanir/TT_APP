using System;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.FrameWork.IA;
using FUNCOES = TT.FrameWork.Funcoes;
using GRID = TT.FrameWork.Grid;

namespace TT_Flow.App.Paginas.IA
{
    public partial class Auditoria : Page
    {
        private readonly cls_IA_Repositorio _repositorio = new cls_IA_Repositorio();

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.IA.VisualizarAuditoria, true);

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = "Auditoria IA";

                string hoje = DateTime.Today.ToString("dd/MM/yyyy");
                string seteDias = DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy");
                txtEvtDtIni.Text = seteDias; txtEvtDtFim.Text = hoje;
                txtChmDtIni.Text = seteDias; txtChmDtFim.Text = hoje;
                txtAvlDtIni.Text = seteDias; txtAvlDtFim.Text = hoje;

                PopularFiltros();
                PesquisarTudo();
            }
        }

        // ---- Botoes por aba (todos re-consultam as 3 grids, para manter o DataTables das outras abas
        // vivo apos o postback parcial; cada grid le os proprios filtros, que persistem no ViewState). ----
        protected void cmdEvtBuscar_Click(object sender, EventArgs e) { PesquisarTudo(); }
        protected void cmdChmBuscar_Click(object sender, EventArgs e) { PesquisarTudo(); }
        protected void cmdAvlBuscar_Click(object sender, EventArgs e) { PesquisarTudo(); }

        protected void cmdEvtLimpar_Click(object sender, EventArgs e)
        {
            LimparDatas(txtEvtDtIni, txtEvtDtFim);
            SelecionarPrimeiro(ddlEvtUsuario); SelecionarPrimeiro(ddlEvtEvento);
            ddlEvtSeveridade.SelectedValue = string.Empty;
            txtEvtBusca.Text = string.Empty;
            PesquisarTudo();
        }

        protected void cmdChmLimpar_Click(object sender, EventArgs e)
        {
            LimparDatas(txtChmDtIni, txtChmDtFim);
            SelecionarPrimeiro(ddlChmUsuario); SelecionarPrimeiro(ddlChmFerramenta);
            ddlChmStatus.SelectedValue = string.Empty;
            txtChmBusca.Text = string.Empty;
            PesquisarTudo();
        }

        protected void cmdAvlLimpar_Click(object sender, EventArgs e)
        {
            LimparDatas(txtAvlDtIni, txtAvlDtFim);
            SelecionarPrimeiro(ddlAvlUsuario);
            ddlAvlAvaliacao.SelectedValue = string.Empty;
            txtAvlBusca.Text = string.Empty;
            PesquisarTudo();
        }

        private void PesquisarTudo()
        {
            string di, df, erro;

            // Eventos
            if (NormalizarPeriodo(txtEvtDtIni.Text, txtEvtDtFim.Text, out di, out df, out erro))
            {
                BindGrid(dtgvAuditoria, _repositorio.ConsultarAuditoria(
                    0, ParaInt(ddlEvtUsuario.SelectedValue), di, df,
                    ddlEvtEvento.SelectedValue, ddlEvtSeveridade.SelectedValue, txtEvtBusca.Text.Trim()));
            }
            else { MensagemPagina.MostraMensagem_Erro("Eventos: " + erro); }

            // Chamadas
            if (NormalizarPeriodo(txtChmDtIni.Text, txtChmDtFim.Text, out di, out df, out erro))
            {
                BindGrid(dtgvChamadas, _repositorio.ConsultarChamadasFerramenta(
                    0, ParaInt(ddlChmUsuario.SelectedValue), di, df,
                    ddlChmFerramenta.SelectedValue, ddlChmStatus.SelectedValue, txtChmBusca.Text.Trim()));
            }
            else { MensagemPagina.MostraMensagem_Erro("Chamadas: " + erro); }

            // Avaliacoes
            if (NormalizarPeriodo(txtAvlDtIni.Text, txtAvlDtFim.Text, out di, out df, out erro))
            {
                BindGrid(dtgvAvaliacoes, _repositorio.ConsultarAvaliacoes(
                    0, ParaInt(ddlAvlUsuario.SelectedValue), di, df,
                    ddlAvlAvaliacao.SelectedValue, txtAvlBusca.Text.Trim()));
            }
            else { MensagemPagina.MostraMensagem_Erro("Avaliacoes: " + erro); }
        }

        // Popula os dropdowns de filtro (Usuario x3 via sp_Select, Evento distinto, Ferramenta do catalogo).
        private void PopularFiltros()
        {
            FUNCOES.Popula_Combo(ddlEvtUsuario, "sp_Select 'Usuarios'", "idUsuario", "sDscUsuario", false, "Todos os usuários", "0");
            FUNCOES.Popula_Combo(ddlChmUsuario, "sp_Select 'Usuarios'", "idUsuario", "sDscUsuario", false, "Todos os usuários", "0");
            FUNCOES.Popula_Combo(ddlAvlUsuario, "sp_Select 'Usuarios'", "idUsuario", "sDscUsuario", false, "Todos os usuários", "0");

            ddlEvtEvento.Items.Clear();
            ddlEvtEvento.Items.Add(new ListItem("Todos os eventos", string.Empty));
            DataTable tbEventos = _repositorio.ListarEventosDistintos();
            if (tbEventos != null)
            {
                foreach (DataRow row in tbEventos.Rows)
                {
                    string evt = Convert.ToString(row["sEvento"]);
                    if (!string.IsNullOrWhiteSpace(evt)) ddlEvtEvento.Items.Add(new ListItem(evt, evt));
                }
            }

            ddlChmFerramenta.Items.Clear();
            ddlChmFerramenta.Items.Add(new ListItem("Todas as ferramentas", string.Empty));
            foreach (IAFerramentaDefinicao f in cls_IA_ToolRegistry.ListarTodas())
            {
                if (!string.IsNullOrWhiteSpace(f.Nome)) ddlChmFerramenta.Items.Add(new ListItem(f.Nome, f.Nome));
            }
        }

        // Sempre binda (mesmo vazio, para o EmptyDataText aparecer). DataTables so quando ha linhas.
        // Coluna 0 e data em todas as grids (ordenacao dd/MM/yyyy pelo type date-uk).
        private void BindGrid(GridView grid, DataTable tabela)
        {
            grid.DataSource = tabela ?? new DataTable();
            grid.DataBind();

            if (tabela != null && tabela.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables_" + grid.ID,
                    GRID.DataBindComScriptData(grid, tabela, 0, new int[] { 0 }, "desc", "false", "''"), true);
            }
        }

        // ---- Cores de linha (severidade nos eventos, status nas chamadas) ----
        protected void dtgvAuditoria_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            string cor = CorSeveridade(Campo(e.Row.DataItem, "sSeveridade"));
            if (cor.Length > 0) e.Row.CssClass = (e.Row.CssClass + " " + cor).Trim();
        }

        protected void dtgvChamadas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            string cor = CorStatus(Campo(e.Row.DataItem, "sStatus"));
            if (cor.Length > 0) e.Row.CssClass = (e.Row.CssClass + " " + cor).Trim();
            AplicarClasseJson(e, 8); // Erro
        }

        protected void dtgvAvaliacoes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            AplicarClasseJson(e, 5); // Resposta
            AplicarClasseJson(e, 6); // Comentario
        }

        private static string CorSeveridade(string severidade)
        {
            switch ((severidade ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "ERRO": return "danger";
                case "ALERTA":
                case "WARN": return "warning";
                default: return string.Empty; // INFO fica sem cor (nao "pinta" a tabela inteira)
            }
        }

        private static string CorStatus(string status)
        {
            switch ((status ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "SUCESSO": return "success";
                case "ERRO": return "danger";
                case "NEGADO": return "warning";
                case "PENDENTE_CONFIRMACAO":
                case "PENDENTE":
                case "EXECUTANDO": return "info";
                default: return string.Empty;
            }
        }

        private static string Campo(object dataItem, string coluna)
        {
            DataRowView drv = dataItem as DataRowView;
            if (drv == null || !drv.Row.Table.Columns.Contains(coluna) || drv[coluna] == DBNull.Value)
            {
                return string.Empty;
            }
            return Convert.ToString(drv[coluna]);
        }

        private static void AplicarClasseJson(GridViewRowEventArgs e, int indiceCelula)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.Cells.Count > indiceCelula)
            {
                e.Row.Cells[indiceCelula].CssClass = "ia-auditoria-json";
            }
        }

        // ---- Utilitarios ----
        private static void LimparDatas(TextBox dtIni, TextBox dtFim)
        {
            dtIni.Text = DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy");
            dtFim.Text = DateTime.Today.ToString("dd/MM/yyyy");
        }

        private static void SelecionarPrimeiro(DropDownList ddl)
        {
            if (ddl.Items.Count > 0) ddl.SelectedIndex = 0;
        }

        private static bool NormalizarPeriodo(string inicialTexto, string finalTexto, out string dtInicial, out string dtFinal, out string mensagemErro)
        {
            dtInicial = string.Empty;
            dtFinal = string.Empty;
            mensagemErro = string.Empty;

            if (!NormalizarData(inicialTexto, out dtInicial)) { mensagemErro = "data inicial invalida."; return false; }
            if (!NormalizarData(finalTexto, out dtFinal)) { mensagemErro = "data final invalida."; return false; }

            if (!string.IsNullOrWhiteSpace(dtInicial) && !string.IsNullOrWhiteSpace(dtFinal))
            {
                DateTime inicial = DateTime.ParseExact(dtInicial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime final = DateTime.ParseExact(dtFinal, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (final < inicial) { mensagemErro = "data final nao pode ser menor que a inicial."; return false; }
            }

            return true;
        }

        private static bool NormalizarData(string valor, out string dataSql)
        {
            dataSql = string.Empty;
            valor = (valor ?? string.Empty).Trim().Replace("_", string.Empty);
            if (string.IsNullOrWhiteSpace(valor)) return true;

            string[] formatos = { "dd/MM/yyyy", "d/M/yyyy", "dd/MM/yy", "yyyy-MM-dd" };
            DateTime data;
            CultureInfo cultura = CultureInfo.GetCultureInfo("pt-BR");
            if (DateTime.TryParseExact(valor, formatos, cultura, DateTimeStyles.None, out data)
                || DateTime.TryParse(valor, cultura, DateTimeStyles.None, out data))
            {
                dataSql = data.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                return true;
            }

            return false;
        }

        private static int ParaInt(string valor)
        {
            int retorno;
            return int.TryParse((valor ?? string.Empty).Trim(), out retorno) ? retorno : 0;
        }
    }
}
