using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using TT_Flow.FrameWork.IA;
using FUNCOES = TT.FrameWork.Funcoes;
using GRID = TT.FrameWork.Grid;

namespace TT_Flow.App.Paginas.IA
{
    public partial class Conhecimento : Page
    {
        private const int MaxMarkdownModalChars = 200000;
        private readonly cls_IA_Repositorio _repositorio = new cls_IA_Repositorio();

        // Caminho real da pasta de entrada, exibido na tela: sem isso quem gerencia a base nao tem como
        // saber onde copiar o arquivo.
        protected string PastaEntrada = string.Empty;
        protected bool PastaConfigurada { get { return !string.IsNullOrWhiteSpace(PastaEntrada); } }

        // Estado do job tmrConhecimentoPasta no TT_Windows, que e quem le a pasta (desde 16/09/2026 a
        // leitura nao roda mais no TT_Flow). JobSituacao: LIGADO | DESLIGADO | NAO_CADASTRADO.
        protected string JobSituacao = "NAO_CADASTRADO";
        protected int JobIntervaloMinutos;

        public class ConhecimentoMarkdownResponse
        {
            public bool Sucesso { get; set; }
            public string Mensagem { get; set; }
            public string Titulo { get; set; }
            public string Conteudo { get; set; }
            // O documento existia quando a grade foi montada e nao existe mais. Ver ListaDesatualizada().
            public bool ListaDesatualizada { get; set; }
        }

        public class ConhecimentoAcaoResponse
        {
            public bool Sucesso { get; set; }
            public string Mensagem { get; set; }
            // O documento existia quando a grade foi montada e não existe mais. Ver ListaDesatualizada().
            public bool ListaDesatualizada { get; set; }
        }

        // O documento clicado não está mais na base. Dizer só "documento não localizado" faz parecer
        // defeito ou dado perdido; o que houve foi a lista da tela ter envelhecido — outra pessoa removeu
        // o documento, ou a varredura da pasta mexeu na base enquanto esta tela estava aberta. A tela
        // avisa e recarrega sozinha, em vez de deixar a pessoa clicando num item que não existe mais.
        private const string MsgListaDesatualizada =
            "Este documento não está mais na base — a lista da tela estava desatualizada. Atualizando a lista...";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.IA.BaseConhecimento, true);

            // Fora do !IsPostBack de proposito: o caminho precisa ser re-renderizado a cada postback,
            // senao some do painel depois de filtrar por categoria.
            PastaEntrada = (cls_IA_Config.Carregar().ConhecimentoPastaEntrada ?? string.Empty).Trim();
            if (PastaConfigurada)
            {
                CarregarSituacaoJob();
            }

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = "IA - Conhecimento";
                CarregarDocumentos(string.Empty);
            }
        }

        private void CarregarSituacaoJob()
        {
            DataRow job = _repositorio.ConsultarJobConhecimentoPasta();
            if (job == null)
            {
                JobSituacao = "NAO_CADASTRADO";
                return;
            }

            bool habilitado = false;
            try { habilitado = Convert.ToBoolean(job["bHabilitado"]); } catch { }

            int segundos = 0;
            try { segundos = Convert.ToInt32(job["nIntervaloSegundos"]); } catch { }

            JobSituacao = habilitado ? "LIGADO" : "DESLIGADO";
            JobIntervaloMinutos = Math.Max(1, (int)Math.Round(segundos / 60.0));
        }

        protected void cmdAtualizar_Click(object sender, EventArgs e)
        {
            string categoria = (txtFiltroCategoria.Text ?? string.Empty).Trim();
            CarregarDocumentos(categoria);
        }

        private void CarregarDocumentos(string categoria)
        {
            DataTable tbDocs = _repositorio.ListarArquivosConhecimento(categoria);

            if (tbDocs == null || tbDocs.Rows.Count == 0)
            {
                dtgvDocs.DataSource = null;
                dtgvDocs.DataBind();
                return;
            }

            // Coluna 0 = dtUpload (data dd/MM/yyyy HH:mm:ss); ordena como data, desc (mais novo primeiro).
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables_" + dtgvDocs.ID,
                GRID.DataBindComScriptData(dtgvDocs, tbDocs, 0, new int[] { 0 }, "desc", "false", "''"), true);
        }

        [WebMethod]
        public static ConhecimentoMarkdownResponse ConsultarMarkdown(string idArquivoIA)
        {
            if (!FUNCOES.ValidaPermissao(Permissao.IA.BaseConhecimento, false))
            {
                return RespostaMarkdown(false, "Sem permissao para gerenciar a base de conhecimento.", string.Empty, string.Empty);
            }

            int id = ParaInt(idArquivoIA);
            if (id <= 0)
            {
                return RespostaMarkdown(false, "Documento invalido.", string.Empty, string.Empty);
            }

            IAArquivoChat arquivo = new cls_IA_Repositorio().ConsultarArquivoAdmin(id);
            if (arquivo == null || arquivo.Tipo != "B")
            {
                ConhecimentoMarkdownResponse desatualizada = RespostaMarkdown(false, MsgListaDesatualizada, string.Empty, string.Empty);
                desatualizada.ListaDesatualizada = true;
                return desatualizada;
            }

            string titulo = string.IsNullOrWhiteSpace(arquivo.TituloConhecimento) ? arquivo.NomeOriginal : arquivo.TituloConhecimento;

            if (!string.Equals(arquivo.Status, "PRONTO", StringComparison.OrdinalIgnoreCase))
            {
                string mensagem = string.Equals(arquivo.Status, "ERRO_CONVERSAO", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(arquivo.Erro)
                    ? "Documento com erro de conversao: " + arquivo.Erro
                    : "Documento ainda nao esta pronto. Status atual: " + arquivo.Status;

                return RespostaMarkdown(false, mensagem, titulo, string.Empty);
            }

            string markdown = arquivo.Markdown ?? string.Empty;
            if (string.IsNullOrWhiteSpace(markdown))
            {
                return RespostaMarkdown(false, "Sem conteudo convertido para este documento.", titulo, string.Empty);
            }

            bool truncado = markdown.Length > MaxMarkdownModalChars;
            if (truncado)
            {
                markdown = markdown.Substring(0, MaxMarkdownModalChars)
                    + "\r\n\r\n[Conteudo truncado para visualizacao.]";
            }

            cls_IA_Auditoria.RegistrarComUsuario(0, IdUsuarioAtualSeguro(), "CONHECIMENTO_MARKDOWN_VISUALIZADO", "INFO", new
            {
                idArquivoIA = arquivo.IdArquivoIA,
                arquivo.NomeOriginal,
                arquivo.HashSHA256,
                truncado
            });

            return RespostaMarkdown(true, string.Empty, titulo, markdown);
        }

        [WebMethod]
        public static ConhecimentoAcaoResponse RemoverDocumento(string idArquivoIA)
        {
            if (!FUNCOES.ValidaPermissao(Permissao.IA.BaseConhecimento, false))
            {
                return RespostaAcao(false, "Sem permissao para gerenciar a base de conhecimento.");
            }

            int id = ParaInt(idArquivoIA);
            if (id <= 0)
            {
                return RespostaAcao(false, "Documento invalido.");
            }

            cls_IA_Repositorio repositorio = new cls_IA_Repositorio();
            IAArquivoChat arquivo = repositorio.ConsultarArquivoAdmin(id);
            if (arquivo == null || arquivo.Tipo != "B")
            {
                ConhecimentoAcaoResponse desatualizada = RespostaAcao(false, MsgListaDesatualizada);
                desatualizada.ListaDesatualizada = true;
                return desatualizada;
            }

            DataTable tbResultado = repositorio.RemoverArquivoConhecimento(id);
            bool sucesso = ResultadoSucesso(tbResultado);
            string mensagem = ResultadoMensagem(tbResultado, sucesso ? "Documento removido da base." : "Nao foi possivel remover o documento.");

            cls_IA_Auditoria.RegistrarComUsuario(0, IdUsuarioAtualSeguro(),
                sucesso ? "CONHECIMENTO_DOC_REMOVIDO" : "CONHECIMENTO_DOC_REMOVER_NEGADO",
                sucesso ? "INFO" : "WARN",
                new { idArquivoIA = arquivo.IdArquivoIA, arquivo.NomeOriginal, arquivo.HashSHA256, mensagem });

            return RespostaAcao(sucesso, mensagem);
        }

        // Recria os trechos de busca a partir do markdown ja convertido (sem re-upload). Usado para
        // docs antigos ganharem melhorias de trechamento (ex.: overlap) sem subir o arquivo de novo.
        [WebMethod]
        public static ConhecimentoAcaoResponse ReprocessarTrechos(string idArquivoIA)
        {
            if (!FUNCOES.ValidaPermissao(Permissao.IA.BaseConhecimento, false))
            {
                return RespostaAcao(false, "Sem permissao para gerenciar a base de conhecimento.");
            }

            int id = ParaInt(idArquivoIA);
            if (id <= 0)
            {
                return RespostaAcao(false, "Documento invalido.");
            }

            cls_IA_Repositorio repositorio = new cls_IA_Repositorio();
            IAArquivoChat arquivo = repositorio.ConsultarArquivoAdmin(id);
            if (arquivo == null || arquivo.Tipo != "B")
            {
                ConhecimentoAcaoResponse desatualizada = RespostaAcao(false, MsgListaDesatualizada);
                desatualizada.ListaDesatualizada = true;
                return desatualizada;
            }

            if (!string.Equals(arquivo.Status, "PRONTO", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(arquivo.Markdown))
            {
                return RespostaAcao(false, "Documento sem conteudo convertido; nao ha o que reprocessar.");
            }

            cls_IA_Config config = cls_IA_Config.Carregar();
            List<IAArquivoTrecho> trechos = cls_IA_ArquivoMarkdownService.RecriarTrechos(arquivo.Markdown, config);
            if (trechos.Count == 0)
            {
                return RespostaAcao(false, "Nenhum trecho gerado a partir do conteudo do documento.");
            }

            // Mesmo padrao do PersistirConversao do upload: limpa, regrava e atualiza os totais.
            repositorio.LimparTrechosArquivo(id);
            foreach (IAArquivoTrecho trecho in trechos)
            {
                trecho.IdArquivoIA = id;
                repositorio.SalvarTrechoArquivo(trecho);
            }

            arquivo.TotalTrechos = trechos.Count;
            arquivo.ArquivoTemporario = string.Empty;
            repositorio.AtualizarArquivoProcessamento(arquivo);

            cls_IA_Auditoria.RegistrarComUsuario(0, IdUsuarioAtualSeguro(), "CONHECIMENTO_DOC_RETRECHADO", "INFO", new
            {
                idArquivoIA = id,
                arquivo.NomeOriginal,
                totalTrechos = trechos.Count
            });

            return RespostaAcao(true, "Trechos recriados: " + trechos.Count + ".");
        }


        private static ConhecimentoMarkdownResponse RespostaMarkdown(bool sucesso, string mensagem, string titulo, string conteudo)
        {
            return new ConhecimentoMarkdownResponse
            {
                Sucesso = sucesso,
                Mensagem = mensagem ?? string.Empty,
                Titulo = string.IsNullOrWhiteSpace(titulo) ? "Conteudo do documento" : titulo,
                Conteudo = conteudo ?? string.Empty
            };
        }

        private static ConhecimentoAcaoResponse RespostaAcao(bool sucesso, string mensagem)
        {
            return new ConhecimentoAcaoResponse { Sucesso = sucesso, Mensagem = mensagem ?? string.Empty };
        }

        private static bool ResultadoSucesso(DataTable tb)
        {
            return ResultadoValor(tb, "nRet") == "0";
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

        private static int ParaInt(string valor)
        {
            int retorno;
            return int.TryParse((valor ?? string.Empty).Trim(), out retorno) ? retorno : 0;
        }
    }
}
