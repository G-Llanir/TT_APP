using System;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.FrameWork.IA;
using FUNCOES = TT.FrameWork.Funcoes;
using GRID = TT.FrameWork.Grid;

namespace TT_Flow.App.Paginas.IA
{
    public partial class Arquivos : Page
    {
        private const int MaxMarkdownModalChars = 200000;
        private readonly cls_IA_Repositorio _repositorio = new cls_IA_Repositorio();

        public class ArquivoMarkdownResponse
        {
            public bool Sucesso { get; set; }
            public string Mensagem { get; set; }
            public string Titulo { get; set; }
            public string Conteudo { get; set; }
        }

        public class ArquivoAcaoResponse
        {
            public bool Sucesso { get; set; }
            public string Mensagem { get; set; }
            public string Status { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.IA.VisualizarArquivos, true);

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = "Arquivos IA";
                PopularFiltros();
                txtdtInicial.Text = DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy");
                txtdtFinal.Text = DateTime.Today.ToString("dd/MM/yyyy");
                Pesquisar();
            }
        }

        // Dropdowns com busca (Chosen): Usuario via sp_Select 'Usuarios'; Extensao via distinct dos arquivos.
        private void PopularFiltros()
        {
            FUNCOES.Popula_Combo(ddlUsuario, "sp_Select 'Usuarios'", "idUsuario", "sDscUsuario", false, "Todos os usuários", "0");

            ddlExtensao.Items.Clear();
            ddlExtensao.Items.Add(new ListItem("Todas as extensões", string.Empty));
            DataTable tbExt = _repositorio.ListarExtensoesArquivos();
            if (tbExt != null)
            {
                foreach (DataRow row in tbExt.Rows)
                {
                    string ext = Convert.ToString(row["sExtensao"]);
                    if (!string.IsNullOrWhiteSpace(ext)) ddlExtensao.Items.Add(new ListItem(ext, ext));
                }
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        protected void cmdLimpar_Click(object sender, EventArgs e)
        {
            txtTermo.Text = string.Empty;
            if (ddlUsuario.Items.Count > 0) ddlUsuario.SelectedIndex = 0;
            if (ddlExtensao.Items.Count > 0) ddlExtensao.SelectedIndex = 0;
            ddlStatusArquivo.SelectedValue = string.Empty;
            txtdtInicial.Text = DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy");
            txtdtFinal.Text = DateTime.Today.ToString("dd/MM/yyyy");
            Pesquisar();
        }

        private void Pesquisar()
        {
            string dtInicial;
            string dtFinal;
            string mensagemErro;
            if (!NormalizarPeriodo(out dtInicial, out dtFinal, out mensagemErro))
            {
                MensagemPagina.MostraMensagem_Erro(mensagemErro);
                return;
            }

            DataTable tbArquivos = _repositorio.ConsultarArquivosAdmin(
                0,
                0,
                ParaInt(ddlUsuario.SelectedValue),
                ddlStatusArquivo.SelectedValue,
                txtTermo.Text.Trim(),
                dtInicial,
                dtFinal,
                ddlExtensao.SelectedValue);

            if (tbArquivos == null || tbArquivos.Rows.Count == 0)
            {
                dtgvArquivos.DataSource = null;
                dtgvArquivos.DataBind();
                MensagemPagina.MostraMensagem_Erro("Nenhum arquivo IA localizado para os filtros informados.");
                return;
            }

            // Padrao das listagens da casa (Pedidos.aspx): o helper binda e devolve o script do
            // DataTables (paginacao, busca e ordenacao). Colunas 0 e 1 sao datas: declaradas para
            // ordenarem como data (dd/MM/yyyy) e nao como texto.
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables_" + dtgvArquivos.ID,
                GRID.DataBindComScriptData(dtgvArquivos, tbArquivos, 0, new int[] { 0, 1 }, "desc", "false", "''"), true);
        }

        protected void dtgvArquivos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            AplicarClasse(e, 9, "ia-arquivos-nowrap");  // status
            AplicarClasse(e, 13, "ia-arquivos-hash");
            AplicarClasse(e, 15, "ia-arquivos-texto");  // erro
            AplicarClasse(e, 16, "ia-arquivos-nowrap"); // acoes
        }

        private static void AplicarClasse(GridViewRowEventArgs e, int indiceCelula, string classe)
        {
            if (e.Row.Cells.Count > indiceCelula)
            {
                e.Row.Cells[indiceCelula].CssClass = classe;
            }
        }

        // Conversa: link para o chat quando > 0; senao "-" sem link. id e inteiro -> sem injecao.
        protected static string LinkConversa(object idConversa)
        {
            int id = ParaInt(Convert.ToString(idConversa));
            return id > 0 ? "<a href=\"Chat.aspx?id=" + id + "\">" + id + "</a>" : "-";
        }

        // Mensagem (e afins): mostra o numero quando > 0; senao "-".
        protected static string TracoSeZero(object valor)
        {
            int n = ParaInt(Convert.ToString(valor));
            return n > 0 ? n.ToString() : "-";
        }

        protected static string TituloModal(object nomeOriginal)
        {
            string nome = Convert.ToString(nomeOriginal) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(nome))
            {
                nome = "Arquivo IA";
            }

            return HttpUtility.HtmlEncode("Resumo - " + nome);
        }

        protected static string ConteudoModal(object valor)
        {
            string conteudo = Convert.ToString(valor) ?? string.Empty;
            return HttpUtility.HtmlEncode(conteudo);
        }

        protected static string AtributoHtml(object valor)
        {
            return HttpUtility.HtmlAttributeEncode(Convert.ToString(valor) ?? string.Empty);
        }

        protected static string StatusBadgeCss(object status)
        {
            string valor = (Convert.ToString(status) ?? string.Empty).ToUpperInvariant();
            switch (valor)
            {
                case "PRONTO":
                    return "label label-success";
                case "ENVIADO":
                case "CONVERTENDO":
                    return "label label-info";
                case "PROCESSANDO":
                    return "label label-warning";
                case "ERRO_CONVERSAO":
                    return "label label-danger";
                case "REMOVIDO":
                    return "label label-default";
                default:
                    return "label label-primary";
            }
        }

        protected static string StatusTexto(object status)
        {
            string valor = (Convert.ToString(status) ?? string.Empty).ToUpperInvariant();
            switch (valor)
            {
                case "ERRO_CONVERSAO":
                    return "Erro";
                case "PROCESSANDO":
                    return "Processando";
                case "CONVERTENDO":
                    return "Convertendo";
                case "ENVIADO":
                    return "Enviado";
                case "PRONTO":
                    return "Pronto";
                case "REMOVIDO":
                    return "Removido";
                default:
                    return string.IsNullOrWhiteSpace(valor) ? "Sem status" : HttpUtility.HtmlEncode(valor);
            }
        }

        protected static string AtributoReprocessar(object status, object possuiTemporario)
        {
            return PodeReprocessar(status, possuiTemporario)
                ? string.Empty
                : "disabled=\"disabled\" title=\"Sem temporario disponivel ou status nao permitido\"";
        }

        private static bool PodeReprocessar(object status, object possuiTemporario)
        {
            string statusArquivo = (Convert.ToString(status) ?? string.Empty).ToUpperInvariant();
            string temp = (Convert.ToString(possuiTemporario) ?? string.Empty).ToUpperInvariant();
            bool temTemporario = temp == "SIM";

            return temTemporario
                && (statusArquivo == "ENVIADO"
                    || statusArquivo == "CONVERTENDO"
                    || statusArquivo == "PROCESSANDO"
                    || statusArquivo == "ERRO_CONVERSAO");
        }

        [WebMethod]
        public static ArquivoMarkdownResponse ConsultarMarkdownArquivo(string idArquivoIA)
        {
            if (!FUNCOES.ValidaPermissao(Permissao.IA.VisualizarArquivos, false))
            {
                return RespostaMarkdown(false, "Voce nao possui permissao para visualizar arquivos IA.", string.Empty, string.Empty);
            }

            int id = ParaInt(idArquivoIA);
            if (id <= 0)
            {
                return RespostaMarkdown(false, "Arquivo IA invalido.", string.Empty, string.Empty);
            }

            IAArquivoChat arquivo = new cls_IA_Repositorio().ConsultarArquivoAdmin(id);
            if (arquivo == null)
            {
                return RespostaMarkdown(false, "Arquivo IA nao localizado.", string.Empty, string.Empty);
            }

            if (!string.Equals(arquivo.Status, "PRONTO", StringComparison.OrdinalIgnoreCase))
            {
                string mensagem = string.Equals(arquivo.Status, "ERRO_CONVERSAO", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(arquivo.Erro)
                    ? "Arquivo com erro de conversao: " + arquivo.Erro
                    : "Arquivo ainda nao esta pronto para visualizacao. Status atual: " + arquivo.Status;

                return RespostaMarkdown(false, mensagem, "Markdown - " + arquivo.NomeOriginal, string.Empty);
            }

            string markdown = arquivo.Markdown ?? string.Empty;
            if (string.IsNullOrWhiteSpace(markdown))
            {
                return RespostaMarkdown(false, "Sem conteudo markdown disponivel para este arquivo.", "Markdown - " + arquivo.NomeOriginal, string.Empty);
            }

            bool truncado = markdown.Length > MaxMarkdownModalChars;
            if (truncado)
            {
                markdown = markdown.Substring(0, MaxMarkdownModalChars)
                    + "\r\n\r\n[Conteudo truncado para visualizacao. Use os trechos/fontes ou filtros para investigar o restante.]";
            }

            cls_IA_Auditoria.RegistrarComUsuario(
                arquivo.IdConversaIA,
                IdUsuarioAtualSeguro(),
                "ARQUIVO_MARKDOWN_VISUALIZADO",
                "INFO",
                new
                {
                    idArquivoIA = arquivo.IdArquivoIA,
                    arquivo.NomeOriginal,
                    arquivo.HashSHA256,
                    truncado
                });

            return RespostaMarkdown(true, string.Empty, "Markdown - " + arquivo.NomeOriginal, markdown);
        }

        [WebMethod]
        public static ArquivoAcaoResponse ReprocessarArquivo(string idArquivoIA)
        {
            if (!FUNCOES.ValidaPermissao(Permissao.IA.VisualizarArquivos, false))
            {
                return RespostaAcao(false, "Voce nao possui permissao para reprocessar arquivos IA.", string.Empty);
            }

            int id = ParaInt(idArquivoIA);
            if (id <= 0)
            {
                return RespostaAcao(false, "Arquivo IA invalido.", string.Empty);
            }

            cls_IA_Repositorio repositorio = new cls_IA_Repositorio();
            IAArquivoChat arquivo = repositorio.ConsultarArquivoAdmin(id);
            if (arquivo == null)
            {
                return RespostaAcao(false, "Arquivo IA nao localizado.", string.Empty);
            }

            int retencaoHoras = RetencaoOriginalHoras();
            DataTable tbResultado = repositorio.ReenfileirarArquivoAdmin(id, retencaoHoras);
            bool sucesso = ResultadoSucesso(tbResultado);
            string mensagem = ResultadoMensagem(tbResultado, sucesso ? "Arquivo reenfileirado para conversao." : "Nao foi possivel reenfileirar o arquivo.");
            string status = ResultadoValor(tbResultado, "sStatus");

            cls_IA_Auditoria.RegistrarComUsuario(
                arquivo.IdConversaIA,
                IdUsuarioAtualSeguro(),
                sucesso ? "ARQUIVO_REENFILEIRADO" : "ARQUIVO_REENFILEIRAR_NEGADO",
                sucesso ? "INFO" : "WARN",
                new
                {
                    idArquivoIA = arquivo.IdArquivoIA,
                    arquivo.NomeOriginal,
                    arquivo.HashSHA256,
                    statusAnterior = arquivo.Status,
                    mensagem
                });

            return RespostaAcao(sucesso, mensagem, status);
        }

        private static ArquivoMarkdownResponse RespostaMarkdown(bool sucesso, string mensagem, string titulo, string conteudo)
        {
            return new ArquivoMarkdownResponse
            {
                Sucesso = sucesso,
                Mensagem = mensagem ?? string.Empty,
                Titulo = string.IsNullOrWhiteSpace(titulo) ? "Markdown do arquivo" : titulo,
                Conteudo = conteudo ?? string.Empty
            };
        }

        private static ArquivoAcaoResponse RespostaAcao(bool sucesso, string mensagem, string status)
        {
            return new ArquivoAcaoResponse
            {
                Sucesso = sucesso,
                Mensagem = mensagem ?? string.Empty,
                Status = status ?? string.Empty
            };
        }

        private static int RetencaoOriginalHoras()
        {
            try
            {
                cls_IA_Config config = cls_IA_Config.Carregar();
                return config != null && config.ArquivosRetencaoOriginalHoras > 0 ? config.ArquivosRetencaoOriginalHoras : 24;
            }
            catch
            {
                return 24;
            }
        }

        private static bool ResultadoSucesso(DataTable tb)
        {
            string nRet = ResultadoValor(tb, "nRet");
            return nRet == "0";
        }

        private static string ResultadoMensagem(DataTable tb, string padrao)
        {
            string mensagem = ResultadoValor(tb, "sMsg");
            return string.IsNullOrWhiteSpace(mensagem) ? padrao : mensagem;
        }

        private static string ResultadoValor(DataTable tb, string coluna)
        {
            if (tb == null || tb.Rows.Count == 0 || !tb.Columns.Contains(coluna))
            {
                return string.Empty;
            }

            return Convert.ToString(tb.Rows[0][coluna]) ?? string.Empty;
        }

        private static string IdUsuarioAtualSeguro()
        {
            try
            {
                return TT.FrameWork.Identity.Variaveis.idUsuario();
            }
            catch
            {
                return "0";
            }
        }

        private bool NormalizarPeriodo(out string dtInicial, out string dtFinal, out string mensagemErro)
        {
            dtInicial = string.Empty;
            dtFinal = string.Empty;
            mensagemErro = string.Empty;

            if (!NormalizarData(txtdtInicial.Text, out dtInicial))
            {
                mensagemErro = "Data inicial invalida.";
                return false;
            }

            if (!NormalizarData(txtdtFinal.Text, out dtFinal))
            {
                mensagemErro = "Data final invalida.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(dtInicial) && !string.IsNullOrWhiteSpace(dtFinal))
            {
                DateTime inicial = DateTime.ParseExact(dtInicial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime final = DateTime.ParseExact(dtFinal, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                if (final < inicial)
                {
                    mensagemErro = "Data final nao pode ser menor que a data inicial.";
                    return false;
                }
            }

            return true;
        }

        private static bool NormalizarData(string valor, out string dataSql)
        {
            dataSql = string.Empty;
            valor = (valor ?? string.Empty).Trim().Replace("_", string.Empty);
            if (string.IsNullOrWhiteSpace(valor))
            {
                return true;
            }

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
