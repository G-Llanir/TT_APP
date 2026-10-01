using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Newtonsoft.Json.Linq;
using TT.FrameWork;

namespace TT_Windows
{
    public class IAArquivoMarkdownWorker
    {
        private const string Procedure = "sp_Manipula_tbl_Flow_IA_Chat";
        private static readonly XNamespace W = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        private static readonly XNamespace S = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

        public int ProcessarFila(int limite, Action<string> log)
        {
            IAConfigWorker config = CarregarConfig();
            if (!config.IAHabilitada || !config.ArquivosHabilitado)
            {
                log?.Invoke("Fila IA arquivos desabilitada por configuracao.");
                return 0;
            }

            LimparTemporariosExpirados(log);

            DataTable tbFila = BD.ExecutarDataTable(Procedure, new Dictionary<string, string>
            {
                { "@sFuncao", "RESERVAR_ARQUIVO_PROCESSAMENTO" },
                { "@nLimite", Math.Max(1, limite).ToString() }
            }, false);

            int processados = 0;
            foreach (DataRow row in tbFila.Rows)
            {
                ProcessarArquivo(row, config, log);
                processados++;
            }

            return processados;
        }

        private void ProcessarArquivo(DataRow row, IAConfigWorker config, Action<string> log)
        {
            int idArquivoIA = ParaInt(Valor(row, "idArquivoIA"));
            int idConversaIA = ParaInt(Valor(row, "idConversaIA"));
            int idUsuario = ParaInt(Valor(row, "idUsuario"));
            string nomeOriginal = Valor(row, "sNomeOriginal");
            string extensao = Valor(row, "sExtensao").ToLowerInvariant();
            string mimeType = Valor(row, "sMimeType");
            string caminho = Valor(row, "sArquivoTemporario");

            try
            {
                log?.Invoke("Convertendo arquivo IA " + idArquivoIA + " - " + nomeOriginal);

                if (string.IsNullOrWhiteSpace(caminho) || !File.Exists(caminho))
                {
                    AtualizarErro(idArquivoIA, idConversaIA, idUsuario, "Arquivo temporario nao localizado para conversao.");
                    return;
                }

                IAArquivoConvertido convertido = Converter(File.ReadAllBytes(caminho), nomeOriginal, extensao, mimeType, config.MaxCharsTrecho);
                convertido.IdArquivoIA = idArquivoIA;
                convertido.IdConversaIA = idConversaIA;
                PersistirConversao(convertido, idUsuario);
                RemoverArquivo(caminho);

                RegistrarAuditoria(idConversaIA, idUsuario, convertido.Status == "PRONTO" ? "ARQUIVO_CONVERTIDO" : "ARQUIVO_CONVERSAO_ERRO", convertido.Status == "PRONTO" ? "INFO" : "ERRO", new JObject
                {
                    { "idArquivoIA", idArquivoIA },
                    { "status", convertido.Status },
                    { "totalCaracteres", convertido.TotalCaracteres },
                    { "totalTrechos", convertido.Trechos.Count },
                    { "erro", convertido.Erro },
                    { "origem", "TT_WINDOWS" }
                });

                log?.Invoke("Arquivo IA " + idArquivoIA + " finalizado com status " + convertido.Status);
            }
            catch (Exception ex)
            {
                AtualizarErro(idArquivoIA, idConversaIA, idUsuario, "Erro no worker TT_Windows: " + ex.Message);
                log?.Invoke("Erro ao converter arquivo IA " + idArquivoIA + ": " + ex.Message);
            }
        }

        // internal: reaproveitado pelo IAConhecimentoPastaWorker (pasta de entrada da base de conhecimento),
        // para um documento que entra pela pasta ser gravado exatamente como um que entra pela fila.
        internal static void PersistirConversao(IAArquivoConvertido convertido, int idUsuario)
        {
            BD.ExecutarDataTable(Procedure, new Dictionary<string, string>
            {
                { "@sFuncao", "LIMPAR_TRECHOS_ARQUIVO" },
                { "@idArquivoIA", convertido.IdArquivoIA.ToString() },
                { "@idUsuario", idUsuario.ToString() }
            }, false);

            if (convertido.Status == "PRONTO")
            {
                foreach (IAArquivoTrechoWorker trecho in convertido.Trechos)
                {
                    BD.ExecutarDataSet(Procedure, new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR_TRECHO_ARQUIVO" },
                        { "@idArquivoIA", convertido.IdArquivoIA.ToString() },
                        { "@idUsuario", idUsuario.ToString() },
                        { "@nOrdem", trecho.Ordem.ToString() },
                        { "@sTituloTrecho", trecho.Titulo },
                        { "@sConteudoTrecho", trecho.ConteudoMarkdown },
                        { "@nInicioChar", trecho.InicioChar.ToString() },
                        { "@nFimChar", trecho.FimChar.ToString() },
                        { "@nTotalCaracteres", trecho.TotalCaracteres.ToString() }
                    });
                }
            }

            BD.ExecutarDataTable(Procedure, new Dictionary<string, string>
            {
                { "@sFuncao", "ATUALIZAR_ARQUIVO_PROCESSAMENTO" },
                { "@idArquivoIA", convertido.IdArquivoIA.ToString() },
                { "@idUsuario", idUsuario.ToString() },
                { "@sStatusArquivo", convertido.Status },
                { "@sMarkdown", convertido.Markdown },
                { "@sResumo", Resumir(convertido.Markdown, 1000) },
                { "@sErro", convertido.Erro },
                { "@sArquivoTemporario", string.Empty },
                { "@nTotalCaracteres", convertido.TotalCaracteres.ToString() },
                { "@nTotalTrechos", convertido.Trechos.Count.ToString() }
            }, false);
        }

        private void AtualizarErro(int idArquivoIA, int idConversaIA, int idUsuario, string erro)
        {
            BD.ExecutarDataTable(Procedure, new Dictionary<string, string>
            {
                { "@sFuncao", "ATUALIZAR_ARQUIVO_PROCESSAMENTO" },
                { "@idArquivoIA", idArquivoIA.ToString() },
                { "@idUsuario", idUsuario.ToString() },
                { "@sStatusArquivo", "ERRO_CONVERSAO" },
                { "@sErro", erro },
                { "@sArquivoTemporario", string.Empty },
                { "@nTotalCaracteres", "0" },
                { "@nTotalTrechos", "0" }
            }, false);

            RegistrarAuditoria(idConversaIA, idUsuario, "ARQUIVO_CONVERSAO_ERRO", "ERRO", new JObject
            {
                { "idArquivoIA", idArquivoIA },
                { "erro", erro },
                { "origem", "TT_WINDOWS" }
            });
        }

        private void LimparTemporariosExpirados(Action<string> log)
        {
            DataTable tbExpirados = BD.ExecutarDataTable(Procedure, new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_ARQUIVOS_TEMPORARIOS_EXPIRADOS" },
                { "@nLimite", "20" }
            }, false);

            foreach (DataRow row in tbExpirados.Rows)
            {
                int idArquivoIA = ParaInt(Valor(row, "idArquivoIA"));
                int idConversaIA = ParaInt(Valor(row, "idConversaIA"));
                int idUsuario = ParaInt(Valor(row, "idUsuario"));
                string status = Valor(row, "sStatus");
                string caminho = Valor(row, "sArquivoTemporario");

                RemoverArquivo(caminho);

                if (status == "CONVERTENDO" || status == "ENVIADO" || status == "PROCESSANDO")
                {
                    AtualizarErro(idArquivoIA, idConversaIA, idUsuario, "Arquivo temporario expirado antes da conversao. Envie o arquivo novamente.");
                    log?.Invoke("Arquivo IA temporario expirado: " + idArquivoIA);
                }
            }
        }

        internal static IAArquivoConvertido Converter(byte[] conteudo, string nomeOriginal, string extensao, string mimeType, int maxCharsTrecho)
        {
            IAArquivoConvertido convertido = new IAArquivoConvertido();

            try
            {
                string markdown;
                switch ((extensao ?? string.Empty).ToLowerInvariant())
                {
                    case ".pdf":
                        markdown = ExtrairPdf(conteudo, nomeOriginal);
                        break;
                    case ".docx":
                        markdown = ExtrairDocx(conteudo, nomeOriginal);
                        break;
                    case ".xlsx":
                        markdown = ExtrairXlsx(conteudo, nomeOriginal);
                        break;
                    case ".csv":
                        markdown = ExtrairCsv(conteudo, nomeOriginal);
                        break;
                    case ".md":
                    case ".txt":
                        markdown = "# " + nomeOriginal + "\n\n" + LerTexto(conteudo);
                        break;
                    default:
                        convertido.Status = "ERRO_CONVERSAO";
                        convertido.Erro = "Extensao nao permitida para conversao.";
                        return convertido;
                }

                markdown = SanitizarMarkdown(markdown);

                // O cabecalho "# nome" entra sempre, entao IsNullOrWhiteSpace nunca disparava sozinho: um PDF
                // escaneado ficava "PRONTO" so com o titulo e entrava na base sem nada pesquisavel. Mesma
                // correcao que o conversor do TT_Flow ja tinha (cls_IA_ArquivoMarkdownService.ConteudoUtil).
                if (string.IsNullOrWhiteSpace(markdown) || ConteudoUtil(markdown) < 40)
                {
                    convertido.Status = "ERRO_CONVERSAO";
                    convertido.Erro = "Nao foi localizado texto extraivel no arquivo. OCR nao esta habilitado nesta fase.";
                    return convertido;
                }

                convertido.Status = "PRONTO";
                convertido.Markdown = markdown;
                convertido.TotalCaracteres = markdown.Length;
                convertido.Trechos = CriarTrechos(markdown, maxCharsTrecho);
                return convertido;
            }
            catch (Exception ex)
            {
                convertido.Status = "ERRO_CONVERSAO";
                convertido.Erro = "Nao foi possivel converter o arquivo para markdown: " + ex.Message;
                return convertido;
            }
        }

        private static string ExtrairPdf(byte[] conteudo, string nomeOriginal)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# " + nomeOriginal);

            using (PdfReader reader = new PdfReader(conteudo))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string texto = PdfTextExtractor.GetTextFromPage(reader, i);
                    if (!string.IsNullOrWhiteSpace(texto))
                    {
                        sb.AppendLine();
                        sb.AppendLine("## Pagina " + i);
                        sb.AppendLine(texto.Trim());
                    }
                }
            }

            return sb.ToString();
        }

        private static string ExtrairDocx(byte[] conteudo, string nomeOriginal)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# " + nomeOriginal);

            using (ZipArchive zip = new ZipArchive(new MemoryStream(conteudo), ZipArchiveMode.Read))
            {
                ZipArchiveEntry entry = zip.GetEntry("word/document.xml");
                if (entry == null)
                {
                    return sb.ToString();
                }

                using (Stream stream = entry.Open())
                {
                    XDocument doc = XDocument.Load(stream);
                    foreach (XElement paragraph in doc.Descendants(W + "p"))
                    {
                        string texto = string.Join("", TextosWord(paragraph));
                        if (!string.IsNullOrWhiteSpace(texto))
                        {
                            sb.AppendLine();
                            sb.AppendLine(texto.Trim());
                        }
                    }
                }
            }

            return sb.ToString();
        }

        private static IEnumerable<string> TextosWord(XElement element)
        {
            foreach (XElement text in element.Descendants(W + "t"))
            {
                yield return text.Value;
            }
        }

        private static string ExtrairXlsx(byte[] conteudo, string nomeOriginal)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# " + nomeOriginal);

            using (ZipArchive zip = new ZipArchive(new MemoryStream(conteudo), ZipArchiveMode.Read))
            {
                List<string> sharedStrings = LerSharedStrings(zip);
                foreach (ZipArchiveEntry sheet in zip.Entries)
                {
                    if (!sheet.FullName.StartsWith("xl/worksheets/sheet", StringComparison.OrdinalIgnoreCase) || !sheet.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    sb.AppendLine();
                    sb.AppendLine("## " + System.IO.Path.GetFileNameWithoutExtension(sheet.FullName));
                    using (Stream stream = sheet.Open())
                    {
                        XDocument doc = XDocument.Load(stream);
                        List<List<string>> linhas = new List<List<string>>();

                        foreach (XElement row in doc.Descendants(S + "row"))
                        {
                            List<string> valores = new List<string>();
                            foreach (XElement cell in row.Elements(S + "c"))
                            {
                                int indiceColuna = IndiceColunaExcel((string)cell.Attribute("r"));
                                while (valores.Count < indiceColuna)
                                {
                                    valores.Add(string.Empty);
                                }

                                valores.Add(EscaparCelulaMarkdown(ValorCelulaExcel(cell, sharedStrings)));
                            }

                            if (LinhaTemConteudo(valores))
                            {
                                linhas.Add(valores);
                            }
                        }

                        AdicionarTabelaMarkdown(sb, linhas);
                    }
                }
            }

            return sb.ToString();
        }

        private static List<string> LerSharedStrings(ZipArchive zip)
        {
            List<string> retorno = new List<string>();
            ZipArchiveEntry entry = zip.GetEntry("xl/sharedStrings.xml");
            if (entry == null)
            {
                return retorno;
            }

            using (Stream stream = entry.Open())
            {
                XDocument doc = XDocument.Load(stream);
                foreach (XElement si in doc.Descendants(S + "si"))
                {
                    retorno.Add(string.Join("", TextosSpreadsheet(si)));
                }
            }

            return retorno;
        }

        private static IEnumerable<string> TextosSpreadsheet(XElement element)
        {
            foreach (XElement text in element.Descendants(S + "t"))
            {
                yield return text.Value;
            }
        }

        private static string ValorCelulaExcel(XElement cell, List<string> sharedStrings)
        {
            string tipo = ((string)cell.Attribute("t") ?? string.Empty).Trim();
            string valor = cell.Element(S + "v") != null ? cell.Element(S + "v").Value : string.Empty;

            if (tipo == "s")
            {
                int indice = ParaInt(valor);
                return indice >= 0 && indice < sharedStrings.Count ? sharedStrings[indice] : string.Empty;
            }

            if (tipo == "inlineStr")
            {
                return string.Join("", TextosSpreadsheet(cell));
            }

            return valor;
        }

        private static int IndiceColunaExcel(string referencia)
        {
            if (string.IsNullOrWhiteSpace(referencia))
            {
                return 0;
            }

            int indice = 0;
            foreach (char ch in referencia.ToUpperInvariant())
            {
                if (ch < 'A' || ch > 'Z')
                {
                    break;
                }

                indice = indice * 26 + (ch - 'A' + 1);
            }

            return Math.Max(0, indice - 1);
        }

        private static string ExtrairCsv(byte[] conteudo, string nomeOriginal)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# " + nomeOriginal);
            sb.AppendLine();

            List<List<string>> linhas = new List<List<string>>();
            string texto = LerTexto(conteudo).Replace("\r\n", "\n").Replace("\r", "\n");
            foreach (string linha in texto.Split('\n'))
            {
                if (!string.IsNullOrWhiteSpace(linha))
                {
                    List<string> colunas = ParseCsvLinha(linha);
                    for (int i = 0; i < colunas.Count; i++)
                    {
                        colunas[i] = EscaparCelulaMarkdown(colunas[i]);
                    }
                    linhas.Add(colunas);
                }
            }

            AdicionarTabelaMarkdown(sb, linhas);
            return sb.ToString();
        }

        private static string LerTexto(byte[] conteudo)
        {
            try
            {
                return new UTF8Encoding(false, true).GetString(conteudo ?? new byte[0]);
            }
            catch
            {
                return Encoding.Default.GetString(conteudo ?? new byte[0]);
            }
        }

        private static string SanitizarMarkdown(string markdown)
        {
            string decodificado = HttpUtility.HtmlDecode(markdown ?? string.Empty);
            decodificado = Regex.Replace(decodificado, "<.*?>", string.Empty);
            decodificado = Regex.Replace(decodificado, @"[\x00-\x08\x0B\x0C\x0E-\x1F]", string.Empty);
            decodificado = decodificado.Replace("\r\n", "\n").Replace("\r", "\n");
            decodificado = Regex.Replace(decodificado, @"[ \t]+\n", "\n");
            decodificado = Regex.Replace(decodificado, @"\n{4,}", "\n\n\n");
            return decodificado.Trim();
        }

        private static List<IAArquivoTrechoWorker> CriarTrechos(string markdown, int tamanhoTrecho)
        {
            List<IAArquivoTrechoWorker> trechos = new List<IAArquivoTrechoWorker>();
            tamanhoTrecho = tamanhoTrecho <= 0 ? 4000 : tamanhoTrecho;

            int inicio = 0;
            int ordem = 1;
            while (inicio < markdown.Length)
            {
                int fim = Math.Min(markdown.Length, inicio + tamanhoTrecho);
                if (fim < markdown.Length)
                {
                    int quebra = markdown.LastIndexOf('\n', fim - 1, fim - inicio);
                    if (quebra > inicio + 500)
                    {
                        fim = quebra + 1;
                    }
                }

                string conteudo = markdown.Substring(inicio, fim - inicio).Trim();
                if (!string.IsNullOrWhiteSpace(conteudo))
                {
                    trechos.Add(new IAArquivoTrechoWorker
                    {
                        Ordem = ordem,
                        InicioChar = inicio,
                        FimChar = fim,
                        TotalCaracteres = conteudo.Length,
                        Titulo = "Trecho " + ordem,
                        ConteudoMarkdown = conteudo
                    });
                    ordem++;
                }

                if (fim >= markdown.Length)
                {
                    break;
                }

                // Sobreposicao com o trecho anterior, igual ao conversor do TT_Flow: sem ela, uma
                // especificacao cortada na fronteira fica metade em cada trecho e nenhum bate na busca.
                // O Math.Max garante progresso mesmo com trecho pequeno.
                inicio = Math.Max(inicio + 1, fim - OverlapTrechoChars);
            }

            return trechos;
        }

        private const int OverlapTrechoChars = 300;

        // Conteudo alem do cabecalho "# nome" (primeira linha): mede se sobrou texto de verdade.
        private static int ConteudoUtil(string markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
            {
                return 0;
            }

            int quebra = markdown.IndexOf('\n');
            string corpo = quebra >= 0 ? markdown.Substring(quebra + 1) : (markdown.StartsWith("#") ? string.Empty : markdown);
            return corpo.Trim().Length;
        }

        // Impede que um binario disfarcado (ex.: .exe renomeado para .pdf) passe so pela extensao.
        // Copia fiel de cls_IA_ArquivoMarkdownService.ConteudoCoerenteComExtensao, do TT_Flow.
        internal static bool ConteudoCoerenteComExtensao(byte[] conteudo, string extensao)
        {
            if (conteudo == null || conteudo.Length == 0)
            {
                return false;
            }

            switch ((extensao ?? string.Empty).Trim().ToLowerInvariant())
            {
                case ".pdf":
                    return ComecaCom(conteudo, AssinaturaPdf);
                case ".docx":
                case ".xlsx":
                    // OOXML e um pacote ZIP; .doc/.xls antigos (OLE) nao passam aqui de proposito
                    return EhZip(conteudo);
                case ".txt":
                case ".md":
                case ".csv":
                    return PareceTexto(conteudo);
                default:
                    return false;
            }
        }

        private static readonly byte[] AssinaturaPdf = { 0x25, 0x50, 0x44, 0x46 }; // %PDF

        private static bool ComecaCom(byte[] conteudo, byte[] assinatura)
        {
            if (conteudo == null || assinatura == null || conteudo.Length < assinatura.Length)
            {
                return false;
            }

            for (int i = 0; i < assinatura.Length; i++)
            {
                if (conteudo[i] != assinatura[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool EhZip(byte[] c)
        {
            // PK\x03\x04 (normal), PK\x05\x06 (vazio), PK\x07\x08 (dividido)
            if (c.Length < 4 || c[0] != 0x50 || c[1] != 0x4B)
            {
                return false;
            }

            return (c[2] == 0x03 && c[3] == 0x04) || (c[2] == 0x05 && c[3] == 0x06) || (c[2] == 0x07 && c[3] == 0x08);
        }

        private static bool PareceTexto(byte[] c)
        {
            if (ComecaCom(c, AssinaturaPdf)) return false;                                   // PDF
            if (c.Length >= 2 && c[0] == 0x50 && c[1] == 0x4B) return false;                 // ZIP/OOXML
            if (c.Length >= 2 && c[0] == 0x4D && c[1] == 0x5A) return false;                 // MZ (exe/dll)
            if (c.Length >= 4 && c[0] == 0xD0 && c[1] == 0xCF && c[2] == 0x11 && c[3] == 0xE0) return false; // OLE

            int inicio = 0;
            if (c.Length >= 3 && c[0] == 0xEF && c[1] == 0xBB && c[2] == 0xBF)
            {
                inicio = 3; // pula BOM UTF-8
            }
            else if (c.Length >= 2 && ((c[0] == 0xFF && c[1] == 0xFE) || (c[0] == 0xFE && c[1] == 0xFF)))
            {
                return true; // UTF-16 com BOM: texto valido (contem bytes NUL por natureza)
            }

            // Texto real (UTF-8/ANSI) nao tem byte NUL; amostra os primeiros 8 KB
            int limite = Math.Min(c.Length, inicio + 8192);
            for (int i = inicio; i < limite; i++)
            {
                if (c[i] == 0x00)
                {
                    return false;
                }
            }

            return true;
        }

        private static void AdicionarTabelaMarkdown(StringBuilder sb, List<List<string>> linhas)
        {
            if (linhas == null || linhas.Count == 0)
            {
                return;
            }

            int colunas = 0;
            foreach (List<string> linha in linhas)
            {
                if (linha != null && linha.Count > colunas)
                {
                    colunas = linha.Count;
                }
            }

            EscreverLinhaTabela(sb, linhas[0], colunas);
            List<string> separador = new List<string>();
            for (int i = 0; i < colunas; i++)
            {
                separador.Add("---");
            }
            EscreverLinhaTabela(sb, separador, colunas);

            for (int i = 1; i < linhas.Count; i++)
            {
                EscreverLinhaTabela(sb, linhas[i], colunas);
            }
        }

        private static void EscreverLinhaTabela(StringBuilder sb, List<string> linha, int colunas)
        {
            sb.Append("|");
            for (int i = 0; i < colunas; i++)
            {
                string valor = linha != null && i < linha.Count ? linha[i] : string.Empty;
                sb.Append(" ").Append(valor).Append(" |");
            }
            sb.AppendLine();
        }

        private static List<string> ParseCsvLinha(string linha)
        {
            List<string> valores = new List<string>();
            StringBuilder atual = new StringBuilder();
            bool entreAspas = false;

            for (int i = 0; i < linha.Length; i++)
            {
                char ch = linha[i];
                if (ch == '"')
                {
                    if (entreAspas && i + 1 < linha.Length && linha[i + 1] == '"')
                    {
                        atual.Append('"');
                        i++;
                    }
                    else
                    {
                        entreAspas = !entreAspas;
                    }
                }
                else if ((ch == ',' || ch == ';') && !entreAspas)
                {
                    valores.Add(atual.ToString());
                    atual.Length = 0;
                }
                else
                {
                    atual.Append(ch);
                }
            }

            valores.Add(atual.ToString());
            return valores;
        }

        private static bool LinhaTemConteudo(List<string> celulas)
        {
            foreach (string celula in celulas ?? new List<string>())
            {
                if (!string.IsNullOrWhiteSpace(celula))
                {
                    return true;
                }
            }

            return false;
        }

        private static string EscaparCelulaMarkdown(string valor)
        {
            valor = (valor ?? string.Empty).Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
            valor = Regex.Replace(valor, @"\s+", " ").Trim();
            return valor.Replace("|", "\\|");
        }

        internal static IAConfigWorker CarregarConfig()
        {
            DataTable tb = BD.ExecutarDataTable(Procedure, new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_CONFIG" }
            }, false);

            IAConfigWorker config = new IAConfigWorker();
            foreach (DataRow row in tb.Rows)
            {
                string chave = Valor(row, "sChave").ToUpperInvariant();
                string valor = Valor(row, "sValor");

                if (chave == "IA.HABILITADO") config.IAHabilitada = ParaBool(valor);
                else if (chave == "IA.ARQUIVOS.HABILITADO") config.ArquivosHabilitado = ParaBool(valor);
                else if (chave == "IA.ARQUIVOS.MAXCHARSTRECHO") config.MaxCharsTrecho = Math.Max(1000, ParaInt(valor, config.MaxCharsTrecho));
                else if (chave == "IA.ARQUIVOS.MAXMBARQUIVO") config.MaxMBArquivo = Math.Max(1, ParaInt(valor, config.MaxMBArquivo));
                // Pasta de entrada da base de conhecimento (job tmrConhecimentoPasta). Mesmas chaves e
                // mesmos limites que o TT_Flow usa na tela IA - Configuracao.
                else if (chave == "IA.CONHECIMENTO.PASTAENTRADA") config.PastaEntrada = (valor ?? string.Empty).Trim();
                else if (chave == "IA.CONHECIMENTO.PASTAMAXPORVARREDURA") config.PastaMaxPorVarredura = Math.Min(200, Math.Max(1, ParaInt(valor, config.PastaMaxPorVarredura)));
                else if (chave == "IA.CONHECIMENTO.PASTAIDUSUARIO") config.PastaIdUsuario = Math.Max(0, ParaInt(valor, 0));
            }

            return config;
        }

        private static string Resumir(string texto, int limite)
        {
            texto = SanitizarMarkdown(texto ?? string.Empty);
            if (limite > 0 && texto.Length > limite)
            {
                texto = texto.Substring(0, limite) + "...";
            }

            return texto;
        }

        internal static void RegistrarAuditoria(int idConversaIA, int idUsuario, string evento, string severidade, JObject payload)
        {
            try
            {
                BD.ExecutarDataTable(Procedure, new Dictionary<string, string>
                {
                    { "@sFuncao", "SALVAR_AUDITORIA" },
                    { "@idConversaIA", idConversaIA.ToString() },
                    { "@idUsuario", idUsuario.ToString() },
                    { "@sEvento", evento },
                    { "@sSeveridade", severidade },
                    { "@sPayloadJson", payload == null ? string.Empty : payload.ToString(Newtonsoft.Json.Formatting.None) }
                }, false);
            }
            catch
            {
            }
        }

        private static void RemoverArquivo(string caminho)
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

        private static string Valor(DataRow row, string coluna)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(coluna) || row[coluna] == DBNull.Value)
            {
                return string.Empty;
            }

            return row[coluna].ToString().Trim();
        }

        private static int ParaInt(string valor)
        {
            return ParaInt(valor, 0);
        }

        private static int ParaInt(string valor, int padrao)
        {
            int retorno;
            return int.TryParse((valor ?? string.Empty).Trim(), out retorno) ? retorno : padrao;
        }

        private static bool ParaBool(string valor)
        {
            valor = (valor ?? string.Empty).Trim().ToUpperInvariant();
            return valor == "S" || valor == "SIM" || valor == "TRUE" || valor == "1";
        }

        internal class IAConfigWorker
        {
            public bool IAHabilitada { get; set; }
            public bool ArquivosHabilitado { get; set; }
            public int MaxCharsTrecho { get; set; }
            public int MaxMBArquivo { get; set; }
            public string PastaEntrada { get; set; }
            public int PastaMaxPorVarredura { get; set; }
            public int PastaIdUsuario { get; set; }

            public IAConfigWorker()
            {
                ArquivosHabilitado = true;
                MaxCharsTrecho = 4000;
                MaxMBArquivo = 10;
                PastaEntrada = string.Empty;
                PastaMaxPorVarredura = 20;
            }
        }

        internal class IAArquivoConvertido
        {
            public int IdArquivoIA { get; set; }
            public int IdConversaIA { get; set; }
            public string Status { get; set; }
            public string Markdown { get; set; }
            public string Erro { get; set; }
            public int TotalCaracteres { get; set; }
            public List<IAArquivoTrechoWorker> Trechos { get; set; }

            public IAArquivoConvertido()
            {
                Status = "ERRO_CONVERSAO";
                Markdown = string.Empty;
                Erro = string.Empty;
                Trechos = new List<IAArquivoTrechoWorker>();
            }
        }

        internal class IAArquivoTrechoWorker
        {
            public int Ordem { get; set; }
            public int InicioChar { get; set; }
            public int FimChar { get; set; }
            public int TotalCaracteres { get; set; }
            public string Titulo { get; set; }
            public string ConteudoMarkdown { get; set; }
        }
    }
}
