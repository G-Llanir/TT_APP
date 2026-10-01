using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using Newtonsoft.Json;
using TT_Flow.FrameWork.IA;

namespace TT_Flow.App
{
    public partial class IA_ArquivoUpload : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.Equals(Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
            {
                Responder(new IAChatResponse { Sucesso = false, Mensagem = "Metodo invalido para upload." }, 405);
                return;
            }

            string sTipoArquivo = (Request.Form["sTipo"] ?? "C").Trim().ToUpperInvariant();
            bool ehConhecimento = sTipoArquivo == "B";

            string mensagemPermissao;
            if (ehConhecimento)
            {
                if (!TT.FrameWork.Funcoes.ValidaPermissao(Permissao.IA.BaseConhecimento, false))
                {
                    Responder(new IAChatResponse { Sucesso = false, Mensagem = "Sem permissao para gerenciar a base de conhecimento." }, 403);
                    return;
                }
            }
            else if (!cls_IA_Permissoes.PodeUsarChat(out mensagemPermissao))
            {
                Responder(new IAChatResponse { Sucesso = false, Mensagem = mensagemPermissao }, 403);
                return;
            }

            cls_IA_Config config = cls_IA_Config.Carregar();
            if (!config.SqlDisponivel)
            {
                Responder(new IAChatResponse { Sucesso = false, IAConfigurada = false, Mensagem = config.ErroConfiguracao }, 500);
                return;
            }

            if (!config.Habilitado || (!ehConhecimento && !config.ArquivosHabilitado))
            {
                Responder(new IAChatResponse { Sucesso = false, IAConfigurada = true, IAHabilitada = config.Habilitado, Mensagem = "Upload de arquivos da IA esta desabilitado." }, 403);
                return;
            }

            if (Request.Files == null || Request.Files.Count == 0)
            {
                Responder(new IAChatResponse { Sucesso = false, Mensagem = "Nenhum arquivo recebido." }, 400);
                return;
            }

            HttpPostedFile arquivoPostado = Request.Files[0];
            if (arquivoPostado == null || arquivoPostado.ContentLength <= 0)
            {
                Responder(new IAChatResponse { Sucesso = false, Mensagem = "Arquivo vazio." }, 400);
                return;
            }

            cls_IA_ArquivoMarkdownService markdownService = new cls_IA_ArquivoMarkdownService();
            string nomeOriginal = cls_IA_ArquivoMarkdownService.NomeSeguro(arquivoPostado.FileName);
            string extensao = Path.GetExtension(nomeOriginal);

            if (!markdownService.ExtensaoPermitida(extensao))
            {
                Responder(new IAChatResponse { Sucesso = false, Mensagem = "Formato nao permitido. Use PDF, DOCX, TXT, MD, CSV ou XLSX." }, 400);
                return;
            }

            long maxBytes = (long)config.ArquivosMaxMBArquivo * 1024L * 1024L;
            if (arquivoPostado.ContentLength > maxBytes)
            {
                Responder(new IAChatResponse { Sucesso = false, Mensagem = "Arquivo acima do limite de " + config.ArquivosMaxMBArquivo + " MB." }, 400);
                return;
            }

            byte[] conteudo = LerBytes(arquivoPostado);
            int idConversaIA = ehConhecimento ? 0 : ParaInt(Request.Form["idConversaIA"]);
            string sCategoria = (Request.Form["sCategoria"] ?? string.Empty).Trim();
            string sTituloConh = (Request.Form["sTituloConhecimento"] ?? string.Empty).Trim();
            string idUsuario = TT.FrameWork.Identity.Variaveis.idUsuario();

            // Valida o conteudo real contra a extensao (magic number), nao so o nome do arquivo
            if (!markdownService.ConteudoCoerenteComExtensao(conteudo, extensao))
            {
                cls_IA_Auditoria.RegistrarComUsuario(idConversaIA, idUsuario, "ARQUIVO_UPLOAD_CONTEUDO_INVALIDO", "ALERTA", new
                {
                    nome = nomeOriginal,
                    extensao = extensao,
                    mimeDeclarado = arquivoPostado.ContentType ?? string.Empty,
                    tamanhoBytes = arquivoPostado.ContentLength
                });
                Responder(new IAChatResponse { Sucesso = false, Mensagem = "O conteudo do arquivo nao corresponde a extensao informada. Envie um arquivo valido." }, 400);
                return;
            }

            bool converterSincrono = arquivoPostado.ContentLength <= ((long)config.ArquivosSyncMaxMBArquivo * 1024L * 1024L);
            string caminhoTemporario = string.Empty;

            try
            {
                if (!converterSincrono)
                {
                    caminhoTemporario = SalvarTemporario(conteudo, extensao);
                }

                IAArquivoChat arquivo = new IAArquivoChat
                {
                    IdConversaIA = idConversaIA,
                    Tipo = sTipoArquivo,
                    Categoria = sCategoria,
                    TituloConhecimento = sTituloConh,
                    NomeOriginal = nomeOriginal,
                    Extensao = extensao,
                    MimeType = arquivoPostado.ContentType ?? string.Empty,
                    TamanhoBytes = arquivoPostado.ContentLength,
                    TamanhoFormatado = FormatarBytes(arquivoPostado.ContentLength),
                    HashSHA256 = cls_IA_ArquivoMarkdownService.CalcularHashSHA256(conteudo),
                    Status = "CONVERTENDO",
                    ArquivoTemporario = caminhoTemporario
                };

                cls_IA_Repositorio repositorio = new cls_IA_Repositorio(idUsuario);
                arquivo.IdArquivoIA = repositorio.SalvarArquivo(arquivo, config.ArquivosRetencaoOriginalHoras);

                cls_IA_Auditoria.RegistrarComUsuario(idConversaIA, idUsuario, "ARQUIVO_UPLOAD", "INFO", new
                {
                    idArquivoIA = arquivo.IdArquivoIA,
                    nome = arquivo.NomeOriginal,
                    extensao = arquivo.Extensao,
                    tamanhoBytes = arquivo.TamanhoBytes,
                    hash = arquivo.HashSHA256,
                    conversaoSincrona = converterSincrono
                });

                if (converterSincrono)
                {
                    IAArquivoChat convertido = markdownService.Converter(conteudo, nomeOriginal, arquivo.MimeType, config);
                    convertido.IdArquivoIA = arquivo.IdArquivoIA;
                    convertido.IdConversaIA = idConversaIA;
                    PersistirConversao(convertido, new cls_IA_Repositorio(idUsuario), idConversaIA, idUsuario, string.Empty);
                    Responder(new IAChatResponse
                    {
                        Sucesso = convertido.Status == "PRONTO",
                        Mensagem = convertido.Status == "PRONTO" ? "Arquivo convertido para markdown." : convertido.Erro,
                        IAConfigurada = true,
                        IAHabilitada = true,
                        Arquivo = LimparArquivoParaResposta(convertido)
                    });
                    return;
                }

                cls_IA_Auditoria.RegistrarComUsuario(idConversaIA, idUsuario, "ARQUIVO_CONVERSAO_AGUARDANDO_STATUS", "INFO", new
                {
                    idArquivoIA = arquivo.IdArquivoIA,
                    nome = arquivo.NomeOriginal,
                    tamanhoBytes = arquivo.TamanhoBytes
                });

                Responder(new IAChatResponse
                {
                    Sucesso = true,
                    Mensagem = "Arquivo recebido e aguardando conversao.",
                    IAConfigurada = true,
                    IAHabilitada = true,
                    Arquivo = LimparArquivoParaResposta(arquivo)
                });
            }
            catch (Exception ex)
            {
                RemoverTemporario(caminhoTemporario);
                cls_IA_Auditoria.RegistrarComUsuario(idConversaIA, idUsuario, "ARQUIVO_UPLOAD_ERRO", "ERRO", new { erro = ex.Message });
                Responder(new IAChatResponse { Sucesso = false, Mensagem = "Erro ao receber arquivo: " + ex.Message }, 500);
            }
        }

        private static void PersistirConversao(IAArquivoChat convertido, cls_IA_Repositorio repositorio, int idConversaIA, string idUsuario, string caminhoTemporario)
        {
            if (convertido == null)
            {
                return;
            }

            repositorio.LimparTrechosArquivo(convertido.IdArquivoIA);

            if (convertido.Status == "PRONTO")
            {
                foreach (IAArquivoTrecho trecho in convertido.Trechos)
                {
                    trecho.IdArquivoIA = convertido.IdArquivoIA;
                    repositorio.SalvarTrechoArquivo(trecho);
                }
            }

            convertido.ArquivoTemporario = string.Empty;
            repositorio.AtualizarArquivoProcessamento(convertido);

            cls_IA_Auditoria.RegistrarComUsuario(idConversaIA, idUsuario, convertido.Status == "PRONTO" ? "ARQUIVO_CONVERTIDO" : "ARQUIVO_CONVERSAO_ERRO", convertido.Status == "PRONTO" ? "INFO" : "ERRO", new
            {
                idArquivoIA = convertido.IdArquivoIA,
                status = convertido.Status,
                totalCaracteres = convertido.TotalCaracteres,
                totalTrechos = convertido.TotalTrechos,
                erro = convertido.Erro,
                temporarioRemovido = !string.IsNullOrWhiteSpace(caminhoTemporario)
            });
        }

        private static byte[] LerBytes(HttpPostedFile arquivo)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                arquivo.InputStream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private static string SalvarTemporario(byte[] conteudo, string extensao)
        {
            string pasta = HostingEnvironment.MapPath("~/App_Data/IA_Temp");
            if (string.IsNullOrWhiteSpace(pasta))
            {
                pasta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "IA_Temp");
            }

            Directory.CreateDirectory(pasta);
            string caminho = Path.Combine(pasta, Guid.NewGuid().ToString("N") + (string.IsNullOrWhiteSpace(extensao) ? ".bin" : extensao));
            File.WriteAllBytes(caminho, conteudo ?? new byte[0]);
            return caminho;
        }

        private static void RemoverTemporario(string caminho)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(caminho) && File.Exists(caminho))
                {
                    File.Delete(caminho);
                }
            }
            catch
            {
            }
        }

        private void Responder(object payload, int statusCode = 200)
        {
            Response.Clear();
            Response.StatusCode = statusCode;
            Response.ContentType = "application/json";
            Response.TrySkipIisCustomErrors = true;
            Response.Write(JsonConvert.SerializeObject(payload));
            Context.ApplicationInstance.CompleteRequest();
        }

        private static IAArquivoChat LimparArquivoParaResposta(IAArquivoChat arquivo)
        {
            if (arquivo == null)
            {
                return null;
            }

            arquivo.Markdown = string.Empty;
            arquivo.ArquivoTemporario = string.Empty;
            arquivo.Trechos = new System.Collections.Generic.List<IAArquivoTrecho>();
            arquivo.TamanhoFormatado = string.IsNullOrWhiteSpace(arquivo.TamanhoFormatado) ? FormatarBytes(arquivo.TamanhoBytes) : arquivo.TamanhoFormatado;
            return arquivo;
        }

        private static string FormatarBytes(long bytes)
        {
            if (bytes <= 0)
            {
                return "0 KB";
            }

            if (bytes < 1024 * 1024)
            {
                return Math.Max(1, bytes / 1024) + " KB";
            }

            decimal mb = (decimal)bytes / (1024 * 1024);
            return mb.ToString("0.##") + " MB";
        }

        private static int ParaInt(string valor)
        {
            int retorno;
            return int.TryParse((valor ?? string.Empty).Trim(), out retorno) ? retorno : 0;
        }
    }
}
