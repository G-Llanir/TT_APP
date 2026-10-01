using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;

namespace TT_Flow.FrameWork.IA
{
    public class cls_IA_ArquivoMarkdownService
    {
        private static readonly HashSet<string> ExtensoesPermitidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf",
            ".docx",
            ".txt",
            ".md",
            ".csv",
            ".xlsx"
        };

        public bool ExtensaoPermitida(string extensao)
        {
            return ExtensoesPermitidas.Contains(NormalizarExtensao(extensao));
        }

        // Confere se o conteudo real (assinatura/magic number) bate com a extensao declarada.
        // Impede que um binario disfarcado (ex.: .exe renomeado para .pdf) passe so pela extensao.
        public bool ConteudoCoerenteComExtensao(byte[] conteudo, string extensao)
        {
            if (conteudo == null || conteudo.Length == 0)
            {
                return false;
            }

            switch (NormalizarExtensao(extensao))
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
            // Rejeita binarios comuns disfarcados de texto
            if (ComecaCom(c, AssinaturaPdf)) return false;                                   // PDF
            if (c.Length >= 2 && c[0] == 0x50 && c[1] == 0x4B) return false;                 // ZIP/OOXML
            if (c.Length >= 2 && c[0] == 0x4D && c[1] == 0x5A) return false;                 // MZ (exe/dll)
            if (c.Length >= 4 && c[0] == 0xD0 && c[1] == 0xCF && c[2] == 0x11 && c[3] == 0xE0) return false; // OLE (doc/xls antigo)

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

        public IAArquivoChat Converter(byte[] conteudo, string nomeOriginal, string mimeType, cls_IA_Config config)
        {
            IAArquivoChat arquivo = new IAArquivoChat
            {
                NomeOriginal = NomeSeguro(nomeOriginal),
                Extensao = NormalizarExtensao(Path.GetExtension(nomeOriginal)),
                MimeType = mimeType ?? string.Empty,
                TamanhoBytes = conteudo != null ? conteudo.LongLength : 0,
                HashSHA256 = CalcularHashSHA256(conteudo ?? new byte[0])
            };

            if (conteudo == null || conteudo.Length == 0)
            {
                arquivo.Status = "ERRO_CONVERSAO";
                arquivo.Erro = "Arquivo vazio.";
                return arquivo;
            }

            if (!ExtensaoPermitida(arquivo.Extensao))
            {
                arquivo.Status = "ERRO_CONVERSAO";
                arquivo.Erro = "Extensao nao permitida para o chat IA.";
                return arquivo;
            }

            try
            {
                string avisoOcr;
                string markdown = ExtrairMarkdown(conteudo, arquivo.Extensao, arquivo.NomeOriginal, config, out avisoOcr);
                markdown = SanitizarMarkdown(markdown);

                // O cabecalho "# nome" entra sempre, entao IsNullOrWhiteSpace nunca dispara sozinho:
                // um PDF escaneado sem OCR ficava "PRONTO" praticamente vazio (so o titulo) e entrava
                // na base sem conteudo pesquisavel. O check agora olha o conteudo ALEM do cabecalho.
                if (string.IsNullOrWhiteSpace(markdown) || ConteudoUtil(markdown) < 40)
                {
                    arquivo.Status = "ERRO_CONVERSAO";
                    arquivo.Erro = !string.IsNullOrWhiteSpace(avisoOcr)
                        ? "Nao foi localizado texto extraivel no arquivo. " + avisoOcr
                        : "Nao foi localizado texto extraivel no arquivo (provavel PDF escaneado). Habilite o OCR (IA.Arquivos.OcrHabilitado) ou envie um arquivo com texto.";
                    return arquivo;
                }

                arquivo.Markdown = markdown;
                arquivo.TotalCaracteres = markdown.Length;
                arquivo.Resumo = cls_IA_Sanitizacao.Resumir(markdown, 1000);
                arquivo.Trechos = CriarTrechos(markdown, config != null ? config.ArquivosMaxCharsTrecho : 4000);
                arquivo.TotalTrechos = arquivo.Trechos.Count;
                arquivo.Status = "PRONTO";
                return arquivo;
            }
            catch (Exception ex)
            {
                arquivo.Status = "ERRO_CONVERSAO";
                arquivo.Erro = "Nao foi possivel converter o arquivo para markdown: " + ex.Message;
                return arquivo;
            }
        }

        public static string CalcularHashSHA256(byte[] conteudo)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(conteudo ?? new byte[0]);
                StringBuilder sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public static string NomeSeguro(string nomeOriginal)
        {
            string nome = Path.GetFileName(nomeOriginal ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nome))
            {
                return "arquivo";
            }

            nome = Regex.Replace(nome, @"[\x00-\x1F<>:""/\\|?*]", "_");
            return cls_IA_Sanitizacao.Resumir(nome, 240);
        }

        private static string ExtrairMarkdown(byte[] conteudo, string extensao, string nomeOriginal, cls_IA_Config config, out string avisoOcr)
        {
            avisoOcr = string.Empty;
            switch (extensao.ToLowerInvariant())
            {
                case ".pdf":
                    return ExtrairPdf(conteudo, nomeOriginal, config, out avisoOcr);
                case ".docx":
                    return ExtrairDocx(conteudo, nomeOriginal);
                case ".xlsx":
                    return ExtrairXlsx(conteudo, nomeOriginal);
                case ".csv":
                    return ExtrairCsv(conteudo, nomeOriginal);
                case ".md":
                    return "# " + nomeOriginal + "\n\n" + LerTexto(conteudo);
                case ".txt":
                    return "# " + nomeOriginal + "\n\n" + LerTexto(conteudo);
                default:
                    return string.Empty;
            }
        }

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

        private static string ExtrairPdf(byte[] conteudo, string nomeOriginal, cls_IA_Config config, out string avisoOcr)
        {
            avisoOcr = string.Empty;

            int totalPaginas;
            string[] textoPorPagina;
            List<int> paginasSemTexto = new List<int>();

            using (MemoryStream stream = new MemoryStream(conteudo))
            using (PdfReader reader = new PdfReader(stream))
            using (PdfDocument pdf = new PdfDocument(reader))
            {
                totalPaginas = pdf.GetNumberOfPages();
                textoPorPagina = new string[totalPaginas + 1];
                for (int i = 1; i <= totalPaginas; i++)
                {
                    string texto = PdfTextExtractor.GetTextFromPage(pdf.GetPage(i));
                    // Pagina "sem texto" = escaneada (ou quase): candidata a OCR.
                    if (!string.IsNullOrWhiteSpace(texto) && texto.Trim().Length >= 30)
                    {
                        textoPorPagina[i] = texto.Trim();
                    }
                    else
                    {
                        paginasSemTexto.Add(i);
                    }
                }
            }

            if (paginasSemTexto.Count > 0)
            {
                if (config != null && config.ArquivosOcrHabilitado)
                {
                    avisoOcr = OcrPaginasSemTexto(conteudo, textoPorPagina, paginasSemTexto, config);
                }
                else
                {
                    avisoOcr = "O PDF tem " + paginasSemTexto.Count + " pagina(s) sem texto (escaneadas) e o OCR esta desabilitado (IA.Arquivos.OcrHabilitado).";
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# " + nomeOriginal);
            for (int i = 1; i <= totalPaginas; i++)
            {
                if (!string.IsNullOrWhiteSpace(textoPorPagina[i]))
                {
                    sb.AppendLine();
                    sb.AppendLine("## Pagina " + i + (paginasSemTexto.Contains(i) ? " (OCR)" : string.Empty));
                    sb.AppendLine(textoPorPagina[i]);
                }
            }

            return sb.ToString();
        }

        // OCR (Tesseract on-prem) das paginas escaneadas: rasteriza via PDFium (Docnet) e reconhece
        // por+eng. Preenche textoPorPagina[i] das paginas que deram certo. Retorna um aviso quando algo
        // impediu/limitou o OCR (vira mensagem de erro clara se o documento inteiro ficar vazio).
        // Falha de OCR NUNCA derruba a conversao: as paginas com texto normal seguem valendo.
        private static string OcrPaginasSemTexto(byte[] conteudo, string[] textoPorPagina, List<int> paginasSemTexto, cls_IA_Config config)
        {
            string tessdata = ResolverTessdataPath();
            if (!Directory.Exists(tessdata) || Directory.GetFiles(tessdata, "*.traineddata").Length == 0)
            {
                return "OCR habilitado, mas a pasta de idiomas nao foi encontrada (" + tessdata + "). Instale por.traineddata/eng.traineddata.";
            }

            int teto = config.ArquivosOcrMaxPaginas > 0 ? config.ArquivosOcrMaxPaginas : 50;
            int processadas = 0;

            try
            {
                // A4 a ~300 DPI. O DocLib e singleton (nao descartar aqui); readers sao por documento.
                using (Docnet.Core.Readers.IDocReader docReader = Docnet.Core.DocLib.Instance.GetDocReader(
                    conteudo, new Docnet.Core.Models.PageDimensions(2480, 3508)))
                using (Tesseract.TesseractEngine engine = new Tesseract.TesseractEngine(tessdata, "por+eng", Tesseract.EngineMode.Default))
                {
                    foreach (int pagina in paginasSemTexto)
                    {
                        if (processadas >= teto)
                        {
                            return "OCR aplicado em " + processadas + " pagina(s); as demais excederam o teto IA.Arquivos.OcrMaxPaginas (" + teto + ").";
                        }

                        byte[] png = RenderizarPaginaPng(docReader, pagina - 1);
                        if (png == null)
                        {
                            continue;
                        }

                        using (Tesseract.Pix pix = Tesseract.Pix.LoadFromMemory(png))
                        using (Tesseract.Page page = engine.Process(pix))
                        {
                            string texto = (page.GetText() ?? string.Empty).Trim();
                            if (texto.Length > 0)
                            {
                                textoPorPagina[pagina] = texto;
                            }
                        }

                        processadas++;
                    }
                }
            }
            catch (Exception ex)
            {
                // Tipico: binario nativo ausente (tesseract50/leptonica/pdfium fora do bin).
                return "OCR habilitado, mas falhou: " + ex.Message + " Verifique bin\\x64 (tesseract50/leptonica), pdfium.dll e App_Data\\tessdata.";
            }

            return string.Empty;
        }

        // Rasteriza uma pagina do PDF (BGRA do PDFium) e devolve PNG em memoria para o Tesseract.
        private static byte[] RenderizarPaginaPng(Docnet.Core.Readers.IDocReader docReader, int indicePagina)
        {
            try
            {
                using (Docnet.Core.Readers.IPageReader pageReader = docReader.GetPageReader(indicePagina))
                {
                    int largura = pageReader.GetPageWidth();
                    int altura = pageReader.GetPageHeight();
                    byte[] bgra = pageReader.GetImage();
                    if (bgra == null || bgra.Length == 0 || largura <= 0 || altura <= 0)
                    {
                        return null;
                    }

                    using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(largura, altura, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                    {
                        System.Drawing.Imaging.BitmapData dados = bitmap.LockBits(
                            new System.Drawing.Rectangle(0, 0, largura, altura),
                            System.Drawing.Imaging.ImageLockMode.WriteOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        try
                        {
                            System.Runtime.InteropServices.Marshal.Copy(bgra, 0, dados.Scan0, Math.Min(bgra.Length, Math.Abs(dados.Stride) * altura));
                        }
                        finally
                        {
                            bitmap.UnlockBits(dados);
                        }

                        // Fundo branco: paginas PDF costumam vir com alfa 0 e o PNG ficaria "preto" para o OCR.
                        using (System.Drawing.Bitmap opaco = new System.Drawing.Bitmap(largura, altura, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                        {
                            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(opaco))
                            {
                                g.Clear(System.Drawing.Color.White);
                                g.DrawImageUnscaled(bitmap, 0, 0);
                            }

                            using (MemoryStream ms = new MemoryStream())
                            {
                                opaco.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                return ms.ToArray();
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        // Site: ~/App_Data/tessdata. Worker (TT_Windows, fora do IIS): App_Data\tessdata ao lado do exe.
        private static string ResolverTessdataPath()
        {
            string caminho = null;
            try
            {
                caminho = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/tessdata");
            }
            catch
            {
            }

            return string.IsNullOrEmpty(caminho)
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "tessdata")
                : caminho;
        }

        private static string ExtrairDocx(byte[] conteudo, string nomeOriginal)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# " + nomeOriginal);

            using (MemoryStream stream = new MemoryStream(conteudo))
            using (XWPFDocument doc = new XWPFDocument(stream))
            {
                foreach (XWPFParagraph paragraph in doc.Paragraphs)
                {
                    string texto = (paragraph.ParagraphText ?? string.Empty).Trim();
                    if (!string.IsNullOrWhiteSpace(texto))
                    {
                        sb.AppendLine();
                        sb.AppendLine(texto);
                    }
                }

                int indiceTabela = 1;
                foreach (XWPFTable tabela in doc.Tables)
                {
                    sb.AppendLine();
                    sb.AppendLine("## Tabela " + indiceTabela);
                    AdicionarTabelaDocx(sb, tabela);
                    indiceTabela++;
                }
            }

            return sb.ToString();
        }

        private static void AdicionarTabelaDocx(StringBuilder sb, XWPFTable tabela)
        {
            if (tabela == null || tabela.Rows == null || tabela.Rows.Count == 0)
            {
                return;
            }

            List<List<string>> linhas = new List<List<string>>();
            foreach (XWPFTableRow row in tabela.Rows)
            {
                List<string> celulas = new List<string>();
                foreach (XWPFTableCell cell in row.GetTableCells())
                {
                    celulas.Add(EscaparCelulaMarkdown(cell.GetText()));
                }
                linhas.Add(celulas);
            }

            AdicionarTabelaMarkdown(sb, linhas);
        }

        private static string ExtrairXlsx(byte[] conteudo, string nomeOriginal)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# " + nomeOriginal);

            using (MemoryStream stream = new MemoryStream(conteudo))
            using (XSSFWorkbook workbook = new XSSFWorkbook(stream))
            {
                DataFormatter formatter = new DataFormatter();
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    ISheet sheet = workbook.GetSheetAt(i);
                    if (sheet == null)
                    {
                        continue;
                    }

                    sb.AppendLine();
                    sb.AppendLine("## Planilha " + EscaparTitulo(sheet.SheetName));

                    List<List<string>> linhas = new List<List<string>>();
                    for (int r = sheet.FirstRowNum; r <= sheet.LastRowNum; r++)
                    {
                        IRow row = sheet.GetRow(r);
                        if (row == null)
                        {
                            continue;
                        }

                        List<string> celulas = new List<string>();
                        for (int c = row.FirstCellNum; c < row.LastCellNum; c++)
                        {
                            NPOI.SS.UserModel.ICell cell = row.GetCell(c);
                            celulas.Add(EscaparCelulaMarkdown(cell == null ? string.Empty : formatter.FormatCellValue(cell)));
                        }

                        if (LinhaTemConteudo(celulas))
                        {
                            linhas.Add(celulas);
                        }
                    }

                    AdicionarTabelaMarkdown(sb, linhas);
                }
            }

            return sb.ToString();
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
                if (string.IsNullOrWhiteSpace(linha))
                {
                    continue;
                }

                List<string> colunas = ParseCsvLinha(linha);
                for (int i = 0; i < colunas.Count; i++)
                {
                    colunas[i] = EscaparCelulaMarkdown(colunas[i]);
                }
                linhas.Add(colunas);
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
            markdown = cls_IA_Sanitizacao.RemoverHtml(markdown ?? string.Empty);
            markdown = Regex.Replace(markdown, @"[\x00-\x08\x0B\x0C\x0E-\x1F]", string.Empty);
            markdown = markdown.Replace("\r\n", "\n").Replace("\r", "\n");
            markdown = Regex.Replace(markdown, @"[ \t]+\n", "\n");
            markdown = Regex.Replace(markdown, @"\n{4,}", "\n\n\n");
            return markdown.Trim();
        }

        // Sobreposicao entre trechos consecutivos: evita que uma especificacao cortada exatamente na
        // fronteira do trecho fique "invisivel" para a busca (metade em cada trecho, nenhuma bate inteira).
        private const int OverlapTrechoChars = 300;

        // Re-trechamento a partir do markdown ja convertido (usado pelo botao "Reprocessar trechos" da
        // tela Conhecimento, para docs antigos ganharem o overlap sem re-upload).
        public static List<IAArquivoTrecho> RecriarTrechos(string markdown, cls_IA_Config config)
        {
            return CriarTrechos(markdown ?? string.Empty, config != null ? config.ArquivosMaxCharsTrecho : 4000);
        }

        private static List<IAArquivoTrecho> CriarTrechos(string markdown, int tamanhoTrecho)
        {
            List<IAArquivoTrecho> trechos = new List<IAArquivoTrecho>();
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
                    trechos.Add(new IAArquivoTrecho
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

                // Proximo trecho comeca um pouco ANTES do fim do atual (overlap). O Math.Max garante
                // progresso mesmo com trechos pequenos (nunca volta para tras do inicio atual + 1).
                inicio = Math.Max(inicio + 1, fim - OverlapTrechoChars);
            }

            return trechos;
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

            if (colunas == 0)
            {
                return;
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

        private static string EscaparTitulo(string valor)
        {
            return Regex.Replace(valor ?? string.Empty, @"[\r\n#]", " ").Trim();
        }

        private static string NormalizarExtensao(string extensao)
        {
            extensao = (extensao ?? string.Empty).Trim().ToLowerInvariant();
            if (!extensao.StartsWith(".") && extensao.Length > 0)
            {
                extensao = "." + extensao;
            }

            return extensao;
        }
    }
}
