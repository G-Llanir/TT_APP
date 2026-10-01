using iTextSharp.text.pdf;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.SessionState;
using System.Xml.Linq;
using BaseColor = iTextSharp.text.BaseColor;
using Element = iTextSharp.text.Element;
using Font = System.Drawing.Font;
using Image = iTextSharp.text.Image;
using Rectangle = iTextSharp.text.Rectangle;
using static TT.FrameWork.BD;
using static TT.FrameWork.BD.Retorno;
using static TT.FrameWork.Identity;

namespace TT.FrameWork
{
    public class Funcoes
    {
        public static string CarimboDataHora()
        {
            string sCarimbo = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');
            return sCarimbo;
        }

        public static string RetornoPathTemp()
        {

            string sTemp = System.Environment.GetEnvironmentVariable("temp");
            if (sTemp == "") sTemp = System.Environment.GetEnvironmentVariable("tmp");
            string sTempDiretorio = sTemp;
            sTemp = sTemp + "\\STD\\";
            if (!Directory.Exists(sTemp))
            {
                Directory.CreateDirectory(sTempDiretorio + "\\STD");
            }
            return @sTemp;
        }

        public static void DirecionaPagina(string sPagina) => HttpContext.Current.Response.Redirect($"/{sPagina.TrimStart('/')}");

        /// <summary>
        /// Método com propósito de Direcionar o Usuário para outra Página em uma Nova Aba do navegador. 
        /// </summary>
        /// <param name="page">Recebe a Página de onde <b>vêm</b> o direcionamento.</param>
        /// <param name="sUrl">Recebe a URL da Página para onde <b>vai</b> o direcionamento.</param>
        public static void DirecionaPagina_NovaAba(Page page, string sUrl)
        {
            if (!sUrl.Trim().StartsWith("/")) sUrl = "/" + sUrl.Trim();

            string url = "";
            try { url = HttpContext.Current.Request.UrlReferrer.Scheme + "://" + HttpContext.Current.Request.UrlReferrer.Authority + sUrl; }
            catch { url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + sUrl; }

            ScriptManager.RegisterStartupScript(page, page.GetType(), "RedirectNewTab_" + Guid.NewGuid(), $"window.open('{url}', '_blank');", true);
        }

        public static void RecarregarPagina(string sMsg = "") => HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl.Contains("?") ? HttpContext.Current.Request.RawUrl.Split('?')[0] + sMsg : HttpContext.Current.Request.RawUrl + sMsg);

        /// <summary>
        /// Método com propósito de Baixar, efetuar o Download, de um Arquivo.<br /><br />Vale destacar que caso o navegador do Usuário seja capaz de ler e exibir o Arquivo, como por exemplo arquivos em PDF, este será exibido em uma nova aba, para os demais devem ser baixados diretamente. 
        /// </summary>
        /// <param name="page">Recebe a Página de onde vêm o Arquivo para Download.</param>
        /// <param name="sArquivo">Recebe o nome do Arquivo para Download.</param>
        public static void DownloadArquivo(Page page, string sArquivo)
        {
            string url = "";
            try
            {
                url = HttpContext.Current.Request.UrlReferrer.Scheme + "://" + HttpContext.Current.Request.UrlReferrer.Authority + "/Download/" + sArquivo;
            }
            catch
            {
                url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/Download/" + sArquivo;
            }

            string script = $"window.open('{url}', '_blank');";
            ScriptManager.RegisterStartupScript(page, page.GetType(), "openNewTab" + Guid.NewGuid(), script, true);
        }

        public static void DownloadArquivo_NFe(Page page, string sArquivo)
        {
            string url = "";
            try
            {
                url = HttpContext.Current.Request.UrlReferrer.Scheme + "://" + HttpContext.Current.Request.UrlReferrer.Authority + "/NFe/" + sArquivo;
            }
            catch
            {
                url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/NFe/" + sArquivo;
            }

            string script = $"window.open('{url}', '_blank');";
            ScriptManager.RegisterStartupScript(page, page.GetType(), "openNewTab" + Guid.NewGuid(), script, true);
        }

        /// <summary>
        /// Função com propósito de Combinar Arquivos Excel.
        /// </summary>
        /// <param name="bListaArquivos">Recebe a Lista dos Arquivos a serem Combinados.</param>
        /// <param name="sDiretorio">Recebe o caminho da pasta, onde será gerado um novo Arquivo, contendo os Arquivos Combinados.</param>
        /// <returns>Retorna o nome do Arquivo Combinado.</returns>
        public static string CombinarExcel(List<string> bListaArquivos, string sDiretorio)
        {
            string combinedFilePath = "Document_" + Funcoes.CarimboDataHora() + ".xlsx";

            IWorkbook combinedWorkbook = new XSSFWorkbook(); // ou HSSFWorkbook para formato XLS
            for (int i = 0; i < bListaArquivos.Count; i++)
            {
                Excel.CombineWorksheets(sDiretorio + bListaArquivos[i], combinedWorkbook, bListaArquivos[i].Split('_')[0]);
            }
            using (FileStream fs = new FileStream(sDiretorio + combinedFilePath, FileMode.Create, FileAccess.Write))
            {
                combinedWorkbook.Write(fs);
            }

            return combinedFilePath;
        }

        /// <summary>
        /// Método com propósito de Combinar Arquivos PDF.
        /// </summary>
        /// <param name="inputPDFs">Recebe um Array com os nomes dos Arquivos a serem Combinados.</param>
        /// <param name="outputPDF">Recebe um nome, utilizado para gerar o Arquivo Combinado.</param>
        /// <exception cref="Exception">Em caso de erros, envia uma exceção com a Mensagem do Erro.</exception>
        public static void CombinePDFs(string[] inputPDFs, string outputPDF)
        {
            try
            {
                using (FileStream stream = new FileStream(outputPDF, FileMode.Create))
                {
                    iTextSharp.text.Document document = new iTextSharp.text.Document();
                    PdfCopy pdf = new PdfCopy(document, stream);
                    document.Open();

                    foreach (string inputPDF in inputPDFs)
                    {
                        PdfReader reader = new PdfReader(inputPDF);
                        for (int pageNum = 1; pageNum <= reader.NumberOfPages; pageNum++)
                        {
                            PdfImportedPage page = pdf.GetImportedPage(reader, pageNum);
                            pdf.AddPage(page);
                        }
                        reader.Close();
                    }

                    document.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Função com propósito de Escrever valores por extenso.
        /// </summary>
        /// <param name="valor">Recebe o valor que será escrito por extenso.</param>
        /// <returns>Retorna o valor escrito por extenso.</returns>
        public static string EscreverExtenso(decimal valor)
        {
            if (valor <= 0 | valor >= 1000000000000000)
                return "Valor não suportado pelo sistema.";
            else
            {
                string strValor = valor.ToString("000000000000000.00");
                string valor_por_extenso = string.Empty;

                for (int i = 0; i <= 15; i += 3)
                {
                    valor_por_extenso += Escrever_Valor_Extenso(Convert.ToDecimal(strValor.Substring(i, 3)));

                    if (i == 0 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(0, 3)) == 1)
                            valor_por_extenso += " TRILHÃO" + ((Convert.ToDecimal(strValor.Substring(3, 12)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValor.Substring(0, 3)) > 1)
                            valor_por_extenso += " TRILHÕES" + ((Convert.ToDecimal(strValor.Substring(3, 12)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 3 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(3, 3)) == 1)
                            valor_por_extenso += " BILHÃO" + ((Convert.ToDecimal(strValor.Substring(6, 9)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValor.Substring(3, 3)) > 1)
                            valor_por_extenso += " BILHÕES" + ((Convert.ToDecimal(strValor.Substring(6, 9)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 6 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(6, 3)) == 1)
                            valor_por_extenso += " MILHÃO" + ((Convert.ToDecimal(strValor.Substring(9, 6)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValor.Substring(6, 3)) > 1)
                            valor_por_extenso += " MILHÕES" + ((Convert.ToDecimal(strValor.Substring(9, 6)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 9 & valor_por_extenso != string.Empty)
                        if (Convert.ToInt32(strValor.Substring(9, 3)) > 0)
                            valor_por_extenso += " MIL" + ((Convert.ToDecimal(strValor.Substring(12, 3)) > 0) ? " E " : string.Empty);

                    if (i == 12)
                    {
                        if (valor_por_extenso.Length > 8)
                            if (valor_por_extenso.Substring(valor_por_extenso.Length - 6, 6) == "BILHÃO" | valor_por_extenso.Substring(valor_por_extenso.Length - 6, 6) == "MILHÃO")
                                valor_por_extenso += " DE";
                            else
                                if (valor_por_extenso.Substring(valor_por_extenso.Length - 7, 7) == "BILHÕES" | valor_por_extenso.Substring(valor_por_extenso.Length - 7, 7) == "MILHÕES" | valor_por_extenso.Substring(valor_por_extenso.Length - 8, 7) == "TRILHÕES")
                                valor_por_extenso += " DE";
                            else
                                    if (valor_por_extenso.Substring(valor_por_extenso.Length - 8, 8) == "TRILHÕES")
                                valor_por_extenso += " DE";

                        if (Convert.ToInt64(strValor.Substring(0, 15)) == 1)
                            valor_por_extenso += " REAL";
                        else if (Convert.ToInt64(strValor.Substring(0, 15)) > 1)
                            valor_por_extenso += " REAIS";

                        if (Convert.ToInt32(strValor.Substring(16, 2)) > 0 && valor_por_extenso != string.Empty)
                            valor_por_extenso += " E ";
                    }

                    if (i == 15)
                        if (Convert.ToInt32(strValor.Substring(16, 2)) == 1)
                            valor_por_extenso += " CENTAVO";
                        else if (Convert.ToInt32(strValor.Substring(16, 2)) > 1)
                            valor_por_extenso += " CENTAVOS";
                }
                return valor_por_extenso;
            }
        }

        /// <summary>
        /// Função com propósito de Escrever valores por extenso.
        /// </summary>
        /// <param name="valor">Recebe o valor que será escrito por extenso.</param>
        /// <returns>Retorna o valor escrito por extenso.</returns>
        private static string Escrever_Valor_Extenso(decimal valor)
        {
            if (valor <= 0)
                return string.Empty;
            else
            {
                string montagem = string.Empty;
                if (valor > 0 & valor < 1)
                {
                    valor *= 100;
                }
                string strValor = valor.ToString("000");
                int a = Convert.ToInt32(strValor.Substring(0, 1));
                int b = Convert.ToInt32(strValor.Substring(1, 1));
                int c = Convert.ToInt32(strValor.Substring(2, 1));

                if (a == 1) montagem += (b + c == 0) ? "CEM" : "CENTO";
                else if (a == 2) montagem += "DUZENTOS";
                else if (a == 3) montagem += "TREZENTOS";
                else if (a == 4) montagem += "QUATROCENTOS";
                else if (a == 5) montagem += "QUINHENTOS";
                else if (a == 6) montagem += "SEISCENTOS";
                else if (a == 7) montagem += "SETECENTOS";
                else if (a == 8) montagem += "OITOCENTOS";
                else if (a == 9) montagem += "NOVECENTOS";

                if (b == 1)
                {
                    if (c == 0) montagem += ((a > 0) ? " E " : string.Empty) + "DEZ";
                    else if (c == 1) montagem += ((a > 0) ? " E " : string.Empty) + "ONZE";
                    else if (c == 2) montagem += ((a > 0) ? " E " : string.Empty) + "DOZE";
                    else if (c == 3) montagem += ((a > 0) ? " E " : string.Empty) + "TREZE";
                    else if (c == 4) montagem += ((a > 0) ? " E " : string.Empty) + "QUATORZE";
                    else if (c == 5) montagem += ((a > 0) ? " E " : string.Empty) + "QUINZE";
                    else if (c == 6) montagem += ((a > 0) ? " E " : string.Empty) + "DEZESSEIS";
                    else if (c == 7) montagem += ((a > 0) ? " E " : string.Empty) + "DEZESSETE";
                    else if (c == 8) montagem += ((a > 0) ? " E " : string.Empty) + "DEZOITO";
                    else if (c == 9) montagem += ((a > 0) ? " E " : string.Empty) + "DEZENOVE";
                }
                else if (b == 2) montagem += ((a > 0) ? " E " : string.Empty) + "VINTE";
                else if (b == 3) montagem += ((a > 0) ? " E " : string.Empty) + "TRINTA";
                else if (b == 4) montagem += ((a > 0) ? " E " : string.Empty) + "QUARENTA";
                else if (b == 5) montagem += ((a > 0) ? " E " : string.Empty) + "CINQUENTA";
                else if (b == 6) montagem += ((a > 0) ? " E " : string.Empty) + "SESSENTA";
                else if (b == 7) montagem += ((a > 0) ? " E " : string.Empty) + "SETENTA";
                else if (b == 8) montagem += ((a > 0) ? " E " : string.Empty) + "OITENTA";
                else if (b == 9) montagem += ((a > 0) ? " E " : string.Empty) + "NOVENTA";

                if (strValor.Substring(1, 1) != "1" & c != 0 & montagem != string.Empty) montagem += " E ";

                if (strValor.Substring(1, 1) != "1")
                    if (c == 1) montagem += "UM";
                    else if (c == 2) montagem += "DOIS";
                    else if (c == 3) montagem += "TRÊS";
                    else if (c == 4) montagem += "QUATRO";
                    else if (c == 5) montagem += "CINCO";
                    else if (c == 6) montagem += "SEIS";
                    else if (c == 7) montagem += "SETE";
                    else if (c == 8) montagem += "OITO";
                    else if (c == 9) montagem += "NOVE";

                return montagem;
            }
        }

        /// <summary>
        /// Função para Formatar CNPJ ou CPF.
        /// </summary>
        /// <param name="valor">Recebe o CNPJ ou CPF.</param>
        /// <returns>Retorna o CNPJ ou CPF já formatado.</returns>
        public static string Formatar_CNPJ_CPF(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return "N/A";

            valor = valor.Replace(".", "").Replace("-", "").Replace("/", "").Trim();
            if (!ulong.TryParse(valor, out ulong numero)) return valor;

            return valor.Length == 14 ? numero.ToString(@"00\.000\.000\/0000\-00")
                    : valor.Length == 11 ? numero.ToString(@"000\.000\.000\-00")
                    : valor;
        }

        /// <summary>
        /// Função com propósito de adicionar à Documentos PDF um Carimbo padrão.
        /// </summary>
        /// <param name="sNomeArquivo">Recebe o nome do Arquivo PDF onde será adicionado o Carimbo.<br /><br />Não é necessário o caminho completo para o Arquivo, apenas seu nome.</param>
        /// <param name="bAprovado">Recebe um valor para definir se o Carimbo aplicado será o de 'Aprovado' ou o de 'Original'.</param>
        /// <param name="bTodasPaginas">Recebe um valor para definir se o Carimbo aplicado estará em todas as Páginas do Documento ou apenas na última.</param>
        /// <param name="posX">Recebe a posição onde ficará o Carimbo, em relação ao eixo X, ou seja, na horizontal.</param>
        /// <param name="posY">Recebe a posição onde ficará o Carimbo, em relação ao eixo Y, ou seja, na vertical.</param>
        /// <param name="width">Recebe o tamanho do Carimbo, na horizontal.</param>
        /// <param name="height">Recebe o tamanho do Carimbo, na vertical.</param>
        /// <returns></returns>
        public static string GerarPDFComCarimbo(string sNomeArquivo, bool bAprovado, bool bTodasPaginas, float posX, float posY, float width, float height)
        {
            PdfReader pdfReader = new PdfReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Download", sNomeArquivo));

            sNomeArquivo = sNomeArquivo.Replace(".pdf", "_Carimbado.pdf");

            string sCarimbo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App", "img");

            if (bAprovado)
                sCarimbo += "\\Carimbo_Approved.png";
            else
                sCarimbo += "\\Carimbo_Original.png";

            using (FileStream fs = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Download", sNomeArquivo), FileMode.Create, FileAccess.Write))
            {
                PdfStamper pdfStamper = new PdfStamper(pdfReader, fs);

                Image img = Image.GetInstance(sCarimbo);

                if (bTodasPaginas)
                {
                    for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                    {
                        Rectangle pageSize = pdfReader.GetPageSizeWithRotation(i);

                        img.SetAbsolutePosition(pageSize.Left + posX, pageSize.Bottom + posY);
                        img.ScaleToFit(width, height);

                        PdfContentByte cb = pdfStamper.GetOverContent(i);
                        cb.AddImage(img);
                    }
                }
                else
                {
                    Rectangle pageSize = pdfReader.GetPageSizeWithRotation(pdfReader.NumberOfPages);

                    img.SetAbsolutePosition(pageSize.Left + posX, pageSize.Bottom + posY);
                    img.ScaleToFit(width, height);

                    PdfContentByte cb = pdfStamper.GetOverContent(pdfReader.NumberOfPages);
                    cb.AddImage(img);
                }

                pdfStamper.Close();
            }

            pdfReader.Close();

            return sNomeArquivo;
        }

        /// <summary>
        /// Função com propósito de Aplicar um Carimbo, com Formato padrão de CNPJ, em Arquivos PDF.
        /// </summary>
        /// <param name="sNomeArquivo">Recebe o Nome do Arquivo onde será aplicado o Carimbo.</param>
        /// <param name="idEmpresa">Recebe o ID da Empresa, para preencher as informações do Carimbo.</param>
        /// <param name="bTodasPaginas">Recebe um booleano que define se o Carimbo deve aparecer em Todas as Páginas ou apenas na última.</param>
        /// <param name="posX">Recebe a posição do Carimbo, em relação ao eixo X, na horizontal.</param>
        /// <param name="posY">Recebe a posição do Carimbo, em relação ao eixo Y, na vertical.</param>
        /// <param name="width">Recebe a largura do Carimbo.</param>
        /// <param name="height">Recebe a altura do Carimbo.</param>
        /// <returns>Retorna o nome do Arquivo com o Carimbo aplicado.</returns>
        public static string GerarPDFComCarimbo_CNPJ(string sNomeArquivo, int idEmpresa, bool bTodasPaginas, float posX, float posY, float width, float height)
        {
            Dictionary<string, string> vParam = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_EMPRESA__CARIMBO_CNPJ" },
                { "@idEmpresa", idEmpresa.ToString().Trim() }
            };
            DataTable dt = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Empresas", vParam);

            return GerarPDFComCarimbo_CNPJ(sNomeArquivo, dt.Rows[0]["sCPF_CNPJ"].ToString(), dt.Rows[0]["sRazaoSocial"].ToString(), dt.Rows[0]["sEnderecoEmpresa_1"].ToString(), dt.Rows[0]["sEnderecoEmpresa_2"].ToString(), dt.Rows[0]["sEnderecoEmpresa_3"].ToString(), bTodasPaginas, posX, posY, width, height);
        }

        /// <summary>
        /// Função com propósito de Aplicar um Carimbo, com Formato padrão de CNPJ, em Arquivos PDF.
        /// </summary>
        /// <param name="sNomeArquivo">Recebe o Nome do Arquivo onde será aplicado o Carimbo.</param>
        /// <param name="sCNPJ">Recebe o CNPJ da Empresa.</param>
        /// <param name="sRazaoSocial">Recebe a Razão Social da Empresa.</param>
        /// <param name="sEndereco_1">Recebe a primeira parte do Endereço da Empresa.<br />No modelo padrão inclui: Logradouro, Número e Complemento.</param>
        /// <param name="sEndereco_2">Recebe a segunda parte do Endereço da Empresa.<br />No modelo padrão inclui: Bairro e CEP</param>
        /// <param name="sEndereco_3">Recebe a terceira parte do Endereço da Empresa.<br />No modelo padrão inclui: Cidade e Estado.</param>
        /// <param name="bTodasPaginas">Recebe um booleano que define se o Carimbo deve aparecer em Todas as Páginas ou apenas na última.</param>
        /// <param name="posX">Recebe a posição do Carimbo, em relação ao eixo X, na horizontal.</param>
        /// <param name="posY">Recebe a posição do Carimbo, em relação ao eixo Y, na vertical.</param>
        /// <param name="width">Recebe a largura do Carimbo.</param>
        /// <param name="height">Recebe a altura do Carimbo.</param>
        /// <returns>Retorna o nome do Arquivo com o Carimbo aplicado.</returns>
        public static string GerarPDFComCarimbo_CNPJ(string sNomeArquivo, string sCNPJ, string sRazaoSocial, string sEndereco_1, string sEndereco_2, string sEndereco_3, bool bTodasPaginas, float posX, float posY, float width, float height)
        {
            PdfReader pdfReader = new PdfReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Download", sNomeArquivo));

            sNomeArquivo = sNomeArquivo.Replace(".pdf", "_Carimbado.pdf");

            using (FileStream fs = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Download", sNomeArquivo), FileMode.Create, FileAccess.Write))
            {
                PdfStamper pdfStamper = new PdfStamper(pdfReader, fs);

                Image img = Image.GetInstance(GerarCarimbo_CNPJ(sRazaoSocial, sCNPJ, sEndereco_1, sEndereco_2, sEndereco_3).ToArray());

                if (bTodasPaginas)
                {
                    for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                    {
                        Rectangle pageSize = pdfReader.GetPageSizeWithRotation(i);

                        img.SetAbsolutePosition(pageSize.Left + posX, pageSize.Bottom + posY);
                        img.ScaleToFit(width, height);

                        PdfContentByte cb = pdfStamper.GetOverContent(i);
                        cb.AddImage(img);
                    }
                }
                else
                {
                    Rectangle pageSize = pdfReader.GetPageSizeWithRotation(pdfReader.NumberOfPages);

                    img.SetAbsolutePosition(pageSize.Left + posX, pageSize.Bottom + posY);
                    img.ScaleToFit(width, height);

                    PdfContentByte cb = pdfStamper.GetOverContent(pdfReader.NumberOfPages);
                    cb.AddImage(img);
                }

                pdfStamper.Close();
            }

            pdfReader.Close();

            return sNomeArquivo;
        }

        /// <summary>
        /// Função com propósito de Gerar uma Imagem com um Carimbo, em padrão de CNPJ, recebendo as informações da Empresa para preencher o Carimbo.
        /// </summary>
        /// <param name="idEmpresa">Recebe o ID da Empresa, para que suas informações sejam Consultadas, para gerar o Carimbo.</param>
        /// <returns>Retorna um objeto MemoryStream do Carimbo gerado.</returns>
        public static MemoryStream GerarCarimbo_CNPJ(string idEmpresa)
        {
            Dictionary<string, string> vParam = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_EMPRESA__CARIMBO_CNPJ" },
                { "@idEmpresa", idEmpresa.Trim() }
            };
            DataTable dt = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Empresas", vParam);

            if (dt.Rows.Count > 0)
                return GerarCarimbo_CNPJ(dt.Rows[0]["sRazaoSocial"].ToString(), dt.Rows[0]["sCPF_CNPJ"].ToString(), dt.Rows[0]["sEnderecoEmpresa_1"].ToString(), dt.Rows[0]["sEnderecoEmpresa_2"].ToString(), dt.Rows[0]["sEnderecoEmpresa_3"].ToString());
            else
                return null;
        }

        /// <summary>
        /// Função com propósito de Gerar uma Imagem com um Carimbo, em padrão de CNPJ, recebendo as informações da Empresa para preencher o Carimbo.
        /// </summary>
        /// <param name="sRazaoSocial">Recebe a Razão Social da Empresa.</param>
        /// <param name="sCNPJ">Recebe o CNPJ da Empresa.</param>
        /// <param name="sEndereco_1">Recebe a primeira parte do Endereço da Empresa.<br />No modelo padrão inclui: Logradouro, Número e Complemento.</param>
        /// <param name="sEndereco_2">Recebe a segunda parte do Endereço da Empresa.<br />No modelo padrão inclui: Bairro e CEP</param>
        /// <param name="sEndereco_3">Recebe a terceira parte do Endereço da Empresa.<br />No modelo padrão inclui: Cidade e Estado.</param>
        /// <returns>Retorna um objeto MemoryStream do Carimbo gerado.</returns>
        public static MemoryStream GerarCarimbo_CNPJ(string sRazaoSocial, string sCNPJ, string sEndereco_1, string sEndereco_2, string sEndereco_3)
        {
            try
            {
                using (Bitmap tempBmp = new Bitmap(1, 1))
                using (Graphics gTemp = Graphics.FromImage(tempBmp))
                {
                    Font fontMaior = new Font("Arial", 10, FontStyle.Bold);
                    Font fontMenor = new Font("Arial", 8, FontStyle.Regular);

                    SizeF sizeCNPJ = gTemp.MeasureString(Formatar_CNPJ_CPF(sCNPJ), fontMaior);
                    SizeF sizeRazaoSocial = gTemp.MeasureString(sRazaoSocial, fontMaior);
                    SizeF sizeEndereco1 = gTemp.MeasureString(sEndereco_1, fontMenor);
                    SizeF sizeEndereco2 = gTemp.MeasureString(sEndereco_2, fontMenor);
                    SizeF sizeEndereco3 = gTemp.MeasureString(sEndereco_3, fontMenor);

                    float maxTextWidth = Math.Max(150, Math.Max(sizeRazaoSocial.Width, Math.Max(sizeCNPJ.Width,
                                                                Math.Max(sizeEndereco1.Width, Math.Max(sizeEndereco2.Width, sizeEndereco3.Width)))));

                    int padding = 5;
                    int width = (int)maxTextWidth + 2 * padding;
                    int height = (int)(sizeRazaoSocial.Height + sizeCNPJ.Height + sizeEndereco1.Height + sizeEndereco2.Height + sizeEndereco3.Height) + padding * 2 + 20;

                    Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                    Graphics g = Graphics.FromImage(bmp);

                    bmp.SetResolution(300, 300);

                    g.Clear(Color.Transparent);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                    Pen pen = new Pen(Color.Blue, 2);
                    g.DrawLine(pen, 0, 0, 20, 0);
                    g.DrawLine(pen, 0, 0, 0, 20);
                    g.DrawLine(pen, 0, height, 20, height);
                    g.DrawLine(pen, 0, height - 20, 0, height);
                    g.DrawLine(pen, width - 20, 0, width, 0);
                    g.DrawLine(pen, width, 0, width, 20);
                    g.DrawLine(pen, width - 20, height, width, height);
                    g.DrawLine(pen, width, height - 20, width, height);

                    float posY = padding;

                    float cnpjX = (width - sizeCNPJ.Width) / 2 + padding;
                    g.DrawString(Formatar_CNPJ_CPF(sCNPJ), fontMaior, Brushes.Blue, new PointF(cnpjX, posY));
                    posY += sizeCNPJ.Height + padding;

                    float empresaX = (width - sizeRazaoSocial.Width) / 2 + padding;
                    g.DrawString(sRazaoSocial, fontMaior, Brushes.Blue, new PointF(empresaX, posY));
                    posY += sizeRazaoSocial.Height + padding;

                    float enderecoX1 = (width - sizeEndereco1.Width) / 2 + padding;
                    g.DrawString(sEndereco_1, fontMenor, Brushes.Blue, new PointF(enderecoX1, posY));
                    posY += sizeEndereco1.Height + padding;

                    float enderecoX2 = (width - sizeEndereco2.Width) / 2 + padding;
                    g.DrawString(sEndereco_2, fontMenor, Brushes.Blue, new PointF(enderecoX2, posY));
                    posY += sizeEndereco2.Height + padding;

                    float enderecoX3 = (width - sizeEndereco3.Width) / 2 + padding;
                    g.DrawString(sEndereco_3, fontMenor, Brushes.Blue, new PointF(enderecoX3, posY));

                    using (MemoryStream stream = new MemoryStream())
                    {
                        bmp.Save(stream, ImageFormat.Png);

                        g.Dispose();
                        bmp.Dispose();

                        return stream;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Função com propósito de Gerar uma Imagem com um Carimbo, de forma padronizada, possiundo apenas uma borda e uma escrita dentro do espaço.
        /// </summary>
        /// <param name="sCarimbo">Recebe uma string com o texto que será impresso no centro do Carimbo.</param>
        /// <returns>Retorna um objeto MemoryStream do Carimbo gerado.</returns>
        public static MemoryStream GerarCarimbos(string sCarimbo)
        {
            try
            {
                sCarimbo = sCarimbo.ToUpper().Trim();

                using (Bitmap tempBmp = new Bitmap(1, 1))
                using (Graphics gTemp = Graphics.FromImage(tempBmp))
                {
                    Font font = new Font("Calibri", 50, FontStyle.Bold);

                    SizeF size = gTemp.MeasureString(sCarimbo, font);

                    int padding = 10;
                    int borderRadius = 50;
                    int width = (int)size.Width + 2 * padding;
                    int height = (int)size.Height + 2 * padding;

                    Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                    Graphics g = Graphics.FromImage(bmp);

                    bmp.SetResolution(300, 300);

                    g.Clear(Color.Transparent);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                    GraphicsPath borda = new GraphicsPath();
                    borda.AddArc(0, 0, borderRadius, borderRadius, 180, 90);
                    borda.AddArc(width - borderRadius, 0, borderRadius, borderRadius, 270, 90);
                    borda.AddArc(width - borderRadius, height - borderRadius, borderRadius, borderRadius, 0, 90);
                    borda.AddArc(0, height - borderRadius, borderRadius, borderRadius, 90, 90);
                    borda.CloseFigure();

                    Pen pen = new Pen(Color.Blue, 7);
                    pen.Alignment = PenAlignment.Inset;

                    g.DrawPath(pen, borda);

                    float x = (width - size.Width) / 2;
                    float y = padding;
                    g.DrawString(sCarimbo, font, Brushes.Blue, new PointF(x, y));

                    g.Dispose();

                    int anguloInclinacao = -15;
                    double radianos = anguloInclinacao * (Math.PI / 180);
                    int widthInclinado = (int)(Math.Abs(width * Math.Cos(radianos)) + Math.Abs(height * Math.Sin(radianos)));
                    int heightInclinado = (int)(Math.Abs(width * Math.Sin(radianos)) + Math.Abs(height * Math.Cos(radianos)));

                    Bitmap bmpInclinado = new Bitmap(widthInclinado, heightInclinado, PixelFormat.Format32bppArgb);
                    Graphics gInclinado = Graphics.FromImage(bmpInclinado);

                    bmpInclinado.SetResolution(300, 300);

                    gInclinado.Clear(Color.Transparent);
                    gInclinado.SmoothingMode = SmoothingMode.HighQuality;
                    gInclinado.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gInclinado.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gInclinado.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                    gInclinado.TranslateTransform(widthInclinado / 2, heightInclinado / 2);
                    gInclinado.RotateTransform(anguloInclinacao);

                    gInclinado.DrawImage(bmp, -width / 2, -height / 2, width, height);

                    gInclinado.ResetTransform();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        bmpInclinado.Save(stream, ImageFormat.Png);

                        gInclinado.Dispose();
                        bmp.Dispose();
                        bmpInclinado.Dispose();

                        return stream;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static bool ValidarPermissaoAcesso()
        {
            ValidarSessao();

            bool bRetorno = true;




            return bRetorno;

        }

        public static bool ValidaPermissao(int idRecurso, bool bGeraMensagemAcesso = false, bool bAreaColaborador = false)
        {
            try { if (HttpContext.Current.Session["sLogin"] == null) HttpContext.Current.Session["sLogin"] = ""; } catch { HttpContext.Current.Session["sLogin"] = ""; }

            if (HttpContext.Current.Session["sLogin"].ToString() == "") return true;
            else if (Identity.Variaveis.sTipo() == "A") return true;
            else
            {
                if (Identity.Variaveis.sPermissao().IndexOf("|" + idRecurso.ToString() + "|") > -1) return true;
                else if (bGeraMensagemAcesso)
                    HttpContext.Current.Response.Redirect(string.Format("~/{0}/PermissaoNegada.aspx?{1}Recurso={2}", bAreaColaborador ? "Aplicativo" : "App", bAreaColaborador ? $"UsuarioVinculado={Identity.Variaveis.idColaborador()}&" : "", idRecurso.ToString().PadLeft(4, '0')));
            }

            return false;
        }

        public static void ValidarSessao(bool bColaborador = false)
        {
            string sURL_Login = bColaborador ? "~/Aplicativo/Login.aspx" : "~/App/Login.aspx";
            try
            {
                try { if (HttpContext.Current.Session["sLogin"] == null) HttpContext.Current.Session["sLogin"] = ""; } catch { HttpContext.Current.Session["sLogin"] = ""; }

                if (HttpContext.Current.Session["sLogin"].ToString() == "")
                {
                    HttpContext.Current.Session["sURLChamada"] = HttpContext.Current.Request.RawUrl.ToString();
                    HttpContext.Current.Session["sServidor"] = HttpContext.Current.Request.Url.Authority.ToString();
                    HttpContext.Current.Response.Redirect(sURL_Login);
                }
            }
            catch
            {
                HttpContext.Current.Response.Redirect(sURL_Login);
            }
        }

        /// <summary>
        /// Função para Popular Combo <br /><br />
        /// dd = Combo qual desejar popular <br />
        /// sSql = Condição de Consulta no BD <br />
        /// sCampoCodigo = Nome do Campo que contem o Código <br />
        /// sCampoDescricao = Nome do Campo que contem a descrição que será preenchida <br />
        /// sConcatenarCodigo_Descricao = Concatena sCampoCodigo e sCampoDescricao <br />
        /// sMensagemPrimeiraLinha = Insere mensagem de primeira Linha <br />
        /// sValorPrimeiraLinha = Código da primeira linha <br />
        /// </summary>
        public static void Popula_Combo(DropDownList dd, string sSql, string sCampoCodigo, string sCampoDescricao) => Popula_Combo(dd, sSql, sCampoCodigo, sCampoDescricao, false);

        public static void Popula_Combo(object dd, string sSql, string sCampoCodigo, string sCampoDescricao, bool sConcatenarCodigo_Descricao) => Popula_Combo(dd, sSql, sCampoCodigo, sCampoDescricao, sConcatenarCodigo_Descricao, "", "");

        public static void Popula_Combo(DropDownList dd, DataSet ds, string sCampoCodigo, string sCampoDescricao, string sMensagemPrimeiraLinha, string sValorPrimeiraLinha)
        {
            try
            {
                if (sMensagemPrimeiraLinha != "")
                    dd.Items.Add(new ListItem(sMensagemPrimeiraLinha, sValorPrimeiraLinha, true));

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        dd.Items.Add(new ListItem(row[sCampoDescricao].ToString(), row[sCampoCodigo].ToString()));
                    }
                }
            }
            catch
            {
                dd.Items.Clear();
                dd.Items.Add(new ListItem("--- ERRO AO CARREGAR ---", ""));
                dd.SelectedIndex = 0;
            }
        }

        public static void Popula_Combo(object ddl, string sSql, string sCampoCodigo, string sCampoDescricao, bool sConcatenarCodigo_Descricao, string sMensagemPrimeiraLinha, string sValorPrimeiraLinha)
        {
            if (ddl is ListControl dd)
            {
                try
                {
                    dd.Items.Clear();

                    if (sMensagemPrimeiraLinha != "") dd.Items.Add(new ListItem(sMensagemPrimeiraLinha, sValorPrimeiraLinha, true));
                    if (sSql.Contains("sp_Select")) sSql += string.Format(",@sCampoCodigo='{0}', @sCampoDescricao='{1}'", sCampoCodigo, sCampoDescricao);

                    SqlDataReader dr = BD.ExecutarDataReader(sSql);

                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            string sDescricao;
                            if (sConcatenarCodigo_Descricao) sDescricao = dr[sCampoCodigo].ToString().Trim() + " - " + dr[sCampoDescricao].ToString();
                            else sDescricao = dr[sCampoDescricao].ToString();

                            dd.Items.Add(new ListItem(sDescricao.Trim(), dr[sCampoCodigo].ToString()));
                        }

                        dr.Close();
                    }
                }
                catch
                {
                    dd.Items.Clear();
                    dd.Items.Add(new ListItem("--- ERRO AO CARREGAR ---", ""));
                    dd.SelectedIndex = 0;
                }
            }
        }

        public static void Popula_Combo(DropDownList dd, string sSql, int idFiltro, string sCampoCodigo, string sCampoDescricao, bool sConcatenarCodigo_Descricao, string sMensagemPrimeiraLinha, string sValorPrimeiraLinha)
        {
            try
            {
                dd.Items.Clear();
                string sDescricao;

                if (sMensagemPrimeiraLinha != "")
                    dd.Items.Add(new ListItem(sMensagemPrimeiraLinha, sValorPrimeiraLinha, true));

                if (sSql.Contains("sp_Select"))
                    sSql += string.Format(",@sCampoCodigo='{0}', @sCampoDescricao='{1}', @idFiltro={2}", sCampoCodigo, sCampoDescricao, idFiltro);

                SqlDataReader dr = BD.ExecutarDataReader(sSql);

                if (dr != null)
                {
                    while (dr.Read())
                    {
                        if (sConcatenarCodigo_Descricao)
                            sDescricao = dr[sCampoCodigo].ToString().Trim() + " - " + dr[sCampoDescricao].ToString();
                        else
                            sDescricao = dr[sCampoDescricao].ToString();

                        dd.Items.Add(new ListItem(sDescricao.Trim(), dr[sCampoCodigo].ToString()));
                    }

                    dr.Close();
                }
            }
            catch
            {
                dd.Items.Clear();
                dd.Items.Add(new ListItem("--- ERRO AO CARREGAR ---", ""));
                dd.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Função para Popular Múltiplos Combos simultaneamente.<br /><br />
        /// Para seu uso ideal, esta função deve receber, da string 'sSql', uma função no banco de dados que retorne todos os dados necessários para popular todos os Combos em ordem.<br /><br />
        /// Ex: sp_Select_Multiple @sFuncao='SelecionaParceiro__Orcamentos', @idPesquisa=1403, @idFiltro=2 <br /><br />
        /// </summary>
        /// <param name="ddls">Recebe a lista com os DropDownList e as configurações individuais para cada DDL.</param>
        /// <param name="vParametros">Recebe os Parâmetros para a consulta na procedure 'sp_Select_Multiple'.</param>
        public static DataSet Popula_Multiplos_Combos(List<cls_Multiplos_Combos> ddls, Dictionary<string, string> vParametros)
        {
            if (ddls.Count > 0 && vParametros.Count > 0)
            {
                DataSet ds = BD.ExecutarDataSet("sp_Select_Multiple", vParametros);
                if (ds != null)
                {
                    for (int i = 0; i < ddls.Count; i++)
                    {
                        cls_Multiplos_Combos ddl = ddls[i];

                        if (ddl.ddl != null)
                        {

                            ddl.ddl.Items.Clear();

                            try
                            {
                                if (ds.Tables.Count - 1 >= ddls.IndexOf(ddl))
                                {
                                    if (!string.IsNullOrEmpty(ddl.sValorPrimeiraLinha) && !string.IsNullOrEmpty(ddl.sMensagemPrimeiraLinha))
                                        ddl.ddl.Items.Add(new ListItem(ddl.sMensagemPrimeiraLinha, ddl.sValorPrimeiraLinha));

                                    foreach (DataRow dr in ds.Tables[ddls.IndexOf(ddl)].Rows)
                                    {
                                        string sDescricao = dr[ddl.sCampoDescricao].ToString();
                                        if (ddl.bConcatenarCodigo_Descricao)
                                            sDescricao = dr[ddl.sCampoCodigo].ToString().Trim() + " - " + dr[ddl.sCampoDescricao].ToString();

                                        ddl.ddl.Items.Add(new ListItem(sDescricao.Trim(), dr[ddl.sCampoCodigo].ToString()));
                                    }
                                }
                            }
                            catch { }
                        }
                        else if (ddl.lst != null)
                        {

                            ddl.lst.Items.Clear();

                            try
                            {
                                if (ds.Tables.Count - 1 >= ddls.IndexOf(ddl))
                                {
                                    if (!string.IsNullOrEmpty(ddl.sValorPrimeiraLinha) && !string.IsNullOrEmpty(ddl.sMensagemPrimeiraLinha))
                                        ddl.lst.Items.Add(new ListItem(ddl.sMensagemPrimeiraLinha, ddl.sValorPrimeiraLinha));

                                    foreach (DataRow dr in ds.Tables[ddls.IndexOf(ddl)].Rows)
                                    {
                                        string sDescricao = dr[ddl.sCampoDescricao].ToString();
                                        if (ddl.bConcatenarCodigo_Descricao)
                                            sDescricao = dr[ddl.sCampoCodigo].ToString().Trim() + " - " + dr[ddl.sCampoDescricao].ToString();

                                        ddl.lst.Items.Add(new ListItem(sDescricao.Trim(), dr[ddl.sCampoCodigo].ToString()));
                                    }
                                }
                            }
                            catch { }
                        }
                    }

                    return ds;
                }
            }

            return null;
        }

        public static void Clona_DropDownList(DropDownList ddlOrigem, DropDownList ddlDestino)
        {
            for (int n = 0; n < ddlOrigem.Items.Count; n++)
            {
                ddlDestino.Items.Add(new ListItem(ddlOrigem.Items[n].Text.ToString(), ddlOrigem.Items[n].Value.ToString()));
            }
        }

        public static void Posiciona_DropDownList(DropDownList dd, string sValor, string sPesquisa)
        {
            int npos = dd.SelectedIndex;
            bool lencontrou = false;

            if (sPesquisa.ToUpper() == "VALOR")
            {
                for (int n = 0; ((n < dd.Items.Count) && (!lencontrou)); n++)
                {
                    dd.SelectedIndex = n;
                    if (dd.SelectedValue == sValor)
                    {
                        lencontrou = true;
                    }

                }
            }
            else
            {
                sValor = sValor.ToUpper().Trim();
                for (int n = 0; ((n < dd.Items.Count) && (!lencontrou)); n++)
                {
                    if (dd.Items[n].ToString().ToUpper().Trim() == sValor)
                    {
                        lencontrou = true;
                        dd.SelectedIndex = n;
                    }

                }
            }
            if (!lencontrou)
            {
                dd.SelectedIndex = npos;
            }




        }

        /// <summary>
        /// 
        ///Welligton - 06/06/2023
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="colunas"></param>
        public static void EsconderColunas(GridView gv, params string[] colunas)
        {
            foreach (string str in colunas)
            {
                foreach (DataControlField coluna in gv.Columns)
                {
                    if (coluna.HeaderText.Equals(str))
                    {
                        coluna.Visible = false;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// Wellington - 03/07/2023
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="colunas"></param>
        public static void ReexibirColunas(GridView gv, params string[] colunas)
        {
            foreach (string str in colunas)
            {
                foreach (DataControlField coluna in gv.Columns)
                {
                    if (coluna.HeaderText.Equals(str))
                    {
                        coluna.Visible = true;
                    }
                }
            }
        }

        /// <summary>
        /// Função para envio de e-mail utilizando Dados de remetente do Banco
        /// </summary>
        /// <param name="sEmailPara">Endereço de e-mail do Destinatário </param>
        /// <param name="sEmailCC"></param>
        /// <param name="sEmailCCo"></param>
        /// <param name="sAssunto"></param>
        /// <param name="sCorpo"></param>
        /// <param name="sAnexo"></param>
        /// <returns></returns>
        public static string EnviarEmail(string sEmailPara, string sEmailCC, string sEmailCCo, string sAssunto, string sCorpo, string sAnexo)
        {
            return EnviarEmail(0, sEmailPara, sEmailCC, sEmailCCo, sAssunto, sCorpo, sAnexo);

        }

        public static string EnviarEmail(int idSMTP, string sEmailPara, string sEmailCC, string sEmailCCo, string sAssunto, string sCorpo, string sAnexo)
        {
            //Instancia a criação de um novo email
            MailMessage mMensagem = new MailMessage();
            string sErro = "";
            try
            {
                //Carregar Variaveis consultando o Banco de Dados atraves do idSMTP
                string sNomeConta = "";
                string sEmailConta = "";
                string sSenhaConta = "";
                string sEnderocoSMTP = "";
                int nPortaSMTP = 587;
                string sSSL = "N";

                SqlDataReader dr;
                dr = BD.ExecutarDataReader("sp_Manipula_tbl_Parametros_SMTP @sFuncao='CONSULTAR', @idSMTP=" + idSMTP.ToString());


                if (dr != null)
                {
                    while (dr.Read())
                    {
                        sNomeConta = dr["sNomeConta"].ToString().Trim();
                        sEmailConta = dr["sEmailConta"].ToString().Trim();
                        sSenhaConta = dr["sSenhaConta"].ToString().Trim();
                        sEnderocoSMTP = dr["sEnderecoSMTP"].ToString().Trim();
                        sSSL = dr["sSSL"].ToString().Trim();
                        nPortaSMTP = Convert.ToInt32(dr["nPortaSMTP"]);
                    }
                }
                else
                {
                    throw new Exception("Nenhuma conta SMTP cadastrada!");
                }

                //Adiciona um anexo ao email
                if (!string.IsNullOrEmpty(sAnexo))
                {
                    string[] anexos = sAnexo.Split(';'); // Separar os caminhos dos anexos
                    foreach (string anexo in anexos)
                    {
                        if (!string.IsNullOrEmpty(anexo))
                        {
                            Attachment item = new Attachment(anexo);
                            mMensagem.Attachments.Add(item);
                        }
                    }
                }

                //Adiciona um Assunto ao email
                mMensagem.Subject = sAssunto;
                mMensagem.From = new MailAddress(string.Format("{0} <{1}>", sNomeConta, sEmailConta));

                //Adiciona um ou mais destinatarios ao email e cria uma lista
                string[] vDestinatarios = sEmailPara.Split(';');
                for (int nRegistro = 0; nRegistro <= (vDestinatarios.Length - 1); nRegistro++)
                {
                    if (!string.IsNullOrEmpty(vDestinatarios[nRegistro].ToString()))
                        mMensagem.To.Add(vDestinatarios[nRegistro].ToString());
                }

                //Adiciona um endereço para mandar copia do email e cria uma lista
                if (!string.IsNullOrEmpty(sEmailCC))
                {
                    string[] vCC = sEmailCC.Split(';');
                    for (int nRegistro = 0; nRegistro <= (vCC.Length - 1); nRegistro++)
                    {
                        if (!string.IsNullOrEmpty(vCC[nRegistro].ToString()))
                            mMensagem.CC.Add(vCC[nRegistro].ToString());
                    }
                }

                //Adiciona um endereço para mandar copia oculta do email e cria uma lista
                if (!string.IsNullOrEmpty(sEmailCCo))
                {
                    string[] vCCo = sEmailCCo.Split(';');
                    for (int nRegistro = 0; nRegistro <= (vCCo.Length - 1); nRegistro++)
                    {
                        if (!string.IsNullOrEmpty(vCCo[nRegistro].ToString()))
                            mMensagem.Bcc.Add(vCCo[nRegistro].ToString());
                    }
                }

                //Cria o corpo do E-mail
                mMensagem.Priority = MailPriority.Normal;
                mMensagem.IsBodyHtml = true;
                mMensagem.Body = sCorpo;
                SmtpClient mSmtp;
                mSmtp = new SmtpClient(sEnderocoSMTP);

                mSmtp.Port = nPortaSMTP;
                if (sSSL == "S")
                {
                    mSmtp.EnableSsl = true;
                }

                mSmtp.Timeout = 100000;
                //Email e senha de quem enviara o email
                mSmtp.Credentials = new System.Net.NetworkCredential(sEmailConta, sSenhaConta);

                mSmtp.Send(mMensagem);

                mMensagem.Dispose();
                mSmtp = null;


            }
            catch (Exception ex)
            {
                sErro = "Erro ao enviar email: " + ex.Message;
            }

            return sErro;
        }

        public static string SeparaNomeArquivo_Banco(string sNomeArquivo) => sNomeArquivo.Split('_')[sNomeArquivo.Split('_').Count() - 1].Split('.')[0];

        public static void CarregarInfo_BD(int idBD = 0)
        {
            Identity.Bancos.Clear();
            foreach (var banco in Directory.EnumerateFiles(Identity.PathAplicacao, "*.xml").Where(x => x.Contains("TT_CONFIG")))
            {
                XDocument XML_Banco = XDocument.Load(banco);

                Identity.Banco_de_Dados novoBanco = new Identity.Banco_de_Dados
                {
                    sIP = BD.Retorno.DadosXML(XML_Banco, "IP"),
                    sUsuario = BD.Retorno.DadosXML(XML_Banco, "USUARIO"),
                    sSenha = BD.Retorno.DadosXML(XML_Banco, "SENHA"),
                    sBanco = BD.Retorno.DadosXML(XML_Banco, "BANCO"),
                    sNome = SeparaNomeArquivo_Banco(Path.GetFileName(banco))
                };
                Identity.Bancos.Add(novoBanco);
            }

            if (string.IsNullOrEmpty(Identity.BancoAtual.sIP)) throw new Exception("Erro ao carregar os dados para Conexão com Banco de Dados!\r\nAusência da TAG XML <IP>");
            else if (string.IsNullOrEmpty(Identity.BancoAtual.sUsuario)) throw new Exception("Erro ao carregar dados para Conexão com Banco de Dados!\r\nAusência da TAG XML <USUARIO>");
            else if (string.IsNullOrEmpty(Identity.BancoAtual.sSenha)) throw new Exception("Erro ao carregar dados para Conexão com Banco de Dados!\r\nAusência da TAG XML <SENHA>");
            else if (string.IsNullOrEmpty(Identity.BancoAtual.sBanco)) throw new Exception("Erro ao carregar dados para Conexão com Banco de Dados!\r\nAusência da TAG XML <BANCO>");
        }

        public static void PopulaCombo_Bancos(object combo)
        {
            if (combo is ListControl ddl)
            {
                ddl.Items.Clear();
                foreach (Identity.Banco_de_Dados banco in Identity.Bancos) ddl.Items.Add(new ListItem(banco.sNome, Identity.Bancos.IndexOf(banco).ToString()));
                ddl.SelectedValue = Identity.Variaveis.idBanco();
            }
        }

        public static string NormalizarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

            string textoSemAcentos = RemoverAcentos(texto);

            textoSemAcentos = textoSemAcentos.Replace(" ", "_").Replace("-", "_");
            textoSemAcentos = Regex.Replace(textoSemAcentos, @"[\\/:*?""<>|]", string.Empty); // Remove caracteres inválidos para nome de arquivo (inclusive \ / : * ? " < > |)
            textoSemAcentos = Regex.Replace(textoSemAcentos, @"[^a-zA-Z0-9_]", string.Empty); // Remove qualquer caractere que não seja letra, número ou underline
            textoSemAcentos = Regex.Replace(textoSemAcentos, @"_+", "_"); // Substitui múltiplos underlines por apenas um

            return textoSemAcentos.Trim('_');
        }

        public static string RemoverAcentos(string texto)
        {
            texto = texto.Trim();
            if (string.IsNullOrEmpty(texto)) return texto;

            var normalizado = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalizado)
            {
                var unicode = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicode != UnicodeCategory.NonSpacingMark) sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string RetornaMensagem_Erro(Exception ex, string separador = "<br />", string seta = "->")
        {
            string erro = string.Empty;

            if (ex != null)
            {
                erro += $"{separador}Exceção {seta} {ex.Message}";
                if (ex.InnerException != null)
                {
                    erro += $"{separador}Exceção Interna {seta} {ex.InnerException.Message}";
                    if (ex.InnerException.InnerException != null) erro += $"{separador}Exceção Interna - Interna {seta} {ex.InnerException.InnerException.Message}";
                }
            }

            return erro;
        }

        /// <summary>
        /// Função utilizada para Consultar as inforamções de ST na API LegisWeb.
        /// </summary>
        /// <param name="UF_Origem">Recebe o UF de Origem.</param>
        /// <param name="UF_Destino">Recebe o UF de Destino.</param>
        /// <param name="NCM">Recebe o código NCM do Produto.</param>
        /// <param name="CEST">Recebe o código CEST do Produto.</param>
        /// <param name="ICMS">Recebe a porcentagem de ICMS do Produto.</param>
        /// <returns>Retorna o valor de MVA do Produto.</returns>
        public static decimal Consultar_ST_LegisWeb(string UF_Origem, string UF_Destino, string NCM, string CEST, decimal ICMS)
        {
            decimal MVA = 0;
            JObject json = null;

            try
            {
                json = JObject.Parse(API.LegisWeb.Consultar_ST_Interestadual_x_CEST(UF_Origem, UF_Destino, CEST.Replace(".", ""), "2"));
                JObject resposta = null;

                if (json == null || (json.ContainsKey("registros") && json["registros"].ToString() == "0")) goto salvaResposta;

                JArray registros = (JArray)json["resposta"];

                foreach (JObject registro in registros)
                {
                    if (registro["cest"].ToString().Replace(".", "").Trim() == CEST.Replace(".", "").Trim() &&
                         (
                            registro["ncm"].ToString().Replace(".", "").Trim() == NCM.Replace(".", "").Trim()
                            || (registro["ncm"].ToString().Replace(".", "").Trim().Length >= 6 && NCM.Replace(".", "").Trim().Length >= 6 && registro["ncm"].ToString().Replace(".", "").Trim().Substring(0, 6) == NCM.Replace(".", "").Trim().Substring(0, 6))
                            || (registro["ncm"].ToString().Replace(".", "").Trim().Length >= 4 && NCM.Replace(".", "").Trim().Length >= 4 && registro["ncm"].ToString().Replace(".", "").Trim().Substring(0, 4) == NCM.Replace(".", "").Trim().Substring(0, 4))
                         )
                        )
                    {
                        resposta = registro;
                        break;
                    }
                }

            salvaResposta:
                if (resposta != null)
                {
                    decimal.TryParse(ICMS == 4 ? resposta["mva_ajustada_4"].ToString().Replace(".", ",") : resposta["mva_ajustada"].ToString().Replace(".", ","), out MVA);

                    if (MVA == 0)
                        decimal.TryParse(resposta["mva"].ToString().Replace(".", ","), out MVA);

                    Dictionary<string, string> vParam = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR_LEGISWEB" },
                        { "@sCodigo", Validar_Dados_ST_LegisWeb(resposta["codigo"].ToString(), 0) },
                        { "@sSigla_Estado_Origem", Validar_Dados_ST_LegisWeb(resposta["sigla_estado_origem"].ToString(), 0) },
                        { "@sSigla_Estado_Destino", Validar_Dados_ST_LegisWeb(resposta["sigla_estado_destino"].ToString(), 0) },
                        { "@sRegime_Origem", Validar_Dados_ST_LegisWeb(resposta["regime_origem"].ToString(), 0) },
                        { "@sRegime_Destino", Validar_Dados_ST_LegisWeb(resposta["regime_destino"].ToString(), 0) },
                        { "@sDestino_Produto", Validar_Dados_ST_LegisWeb(resposta["destino_produto"].ToString(), 0) },
                        { "@sNCM", Validar_Dados_ST_LegisWeb(resposta["ncm"].ToString(), 0) },
                        { "@sCEST", Validar_Dados_ST_LegisWeb(resposta["cest"].ToString(), 0) },
                        { "@sDescricao", Validar_Dados_ST_LegisWeb(resposta["descricao"].ToString(), 0) },
                        { "@sObservacao", Validar_Dados_ST_LegisWeb(resposta["observacao"].ToString(), 0) },
                        { "@sSegmento", Validar_Dados_ST_LegisWeb(resposta["segmento"].ToString(), 0) },
                        { "@sCodigo_Segmento", Validar_Dados_ST_LegisWeb(resposta["codigo_segmento"].ToString(), 0) },
                        { "@nAliquota_Interna", Validar_Dados_ST_LegisWeb(resposta["aliquota_interna"].ToString(), 1) },
                        { "@nAliquota_Interestadual", Validar_Dados_ST_LegisWeb(resposta["aliquota_interestadual"].ToString(), 1) },
                        { "@nFundoPobreza", Validar_Dados_ST_LegisWeb(resposta["fundo_pobreza"].ToString(), 1) },
                        { "@nMVA", Validar_Dados_ST_LegisWeb(resposta["mva"].ToString(), 1) },
                        { "@nMVA_Ajustado", Validar_Dados_ST_LegisWeb(resposta["mva_ajustada"].ToString(), 1) },
                        { "@nMVA_Ajustado_4", Validar_Dados_ST_LegisWeb(resposta["mva_ajustada_4"].ToString(), 1) },
                        { "@nMVA_Positiva", Validar_Dados_ST_LegisWeb(resposta["mva_positiva"].ToString(), 1) },
                        { "@nMVA_Negativa", Validar_Dados_ST_LegisWeb(resposta["mva_negativa"].ToString(), 1) },
                        { "@nMVA_Neutra", Validar_Dados_ST_LegisWeb(resposta["mva_neutra"].ToString(), 1) },
                        { "@dtVigencia_Inicial", Validar_Dados_ST_LegisWeb(resposta["vigencia_inicial"].ToString(), 0) },
                        { "@dtVigencia_Final", resposta["vigencia_final"].ToString().Replace("\\/", "/") },
                        { "@sBase_Legal_ST", Validar_Dados_ST_LegisWeb(resposta["base_legal_st"].ToString(), 0) },
                        { "@dtEfeito_ST", Validar_Dados_ST_LegisWeb(resposta["data_efeito_st"].ToString(), 0) },
                        { "@sNorma_Obs_ST", Validar_Dados_ST_LegisWeb(resposta["norma_observacao_st"].ToString(), 0) },
                        { "@sNorma_Base_Calculo", Validar_Dados_ST_LegisWeb(resposta["norma_base_calculo"].ToString(), 0) },
                        { "@sNorma_Prazo_Recolhimento", Validar_Dados_ST_LegisWeb(resposta["norma_prazo_recolhimento"].ToString(), 0) },
                        { "@sBase_Legal_int", Validar_Dados_ST_LegisWeb(resposta["base_legal_int"].ToString(), 0) },
                        { "@sObservacao_int", Validar_Dados_ST_LegisWeb(resposta["observacao_int"].ToString(), 0) },
                        { "@sBase_Calculo_int", Validar_Dados_ST_LegisWeb(resposta["base_calculo_int"].ToString(), 0) },
                        { "@sPrazo_Recolhimento_int", Validar_Dados_ST_LegisWeb(resposta["prazo_recolhimento_int"].ToString(), 0) },
                        { "@sAplicabilidade", Validar_Dados_ST_LegisWeb(resposta["aplicabilidade"].ToString(), 0) },
                        { "@sNaoAplicabilidade", Validar_Dados_ST_LegisWeb(resposta["nao_aplicabilidade"].ToString(), 0) },
                        { "@sVariacao_MVA", Validar_Dados_ST_LegisWeb(resposta["variacao_mva"].ToString(), 0) },
                        { "@sReducao_MVA", Validar_Dados_ST_LegisWeb(resposta["reducao_mva"].ToString(), 0) },
                        { "@sJSON", json.ToString() }
                    };
                    BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Fiscal_Regras", vParam);
                }
                else
                {
                    Dictionary<string, string> vParam = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR_LEGISWEB" },
                        { "@sCodigo", "" },
                        { "@sSigla_Estado_Origem", UF_Origem },
                        { "@sSigla_Estado_Destino", UF_Destino },
                        { "@sRegime_Origem", "" },
                        { "@sRegime_Destino", "" },
                        { "@sDestino_Produto", "" },
                        { "@sNCM", NCM },
                        { "@sCEST", CEST },
                        { "@sDescricao", "" },
                        { "@sObservacao", "" },
                        { "@sSegmento", "" },
                        { "@sCodigo_Segmento", "" },
                        { "@nAliquota_Interna", "0" },
                        { "@nAliquota_Interestadual", "0" },
                        { "@nFundoPobreza", "0" },
                        { "@nMVA", "0" },
                        { "@nMVA_Ajustado", "0" },
                        { "@nMVA_Ajustado_4", "0" },
                        { "@nMVA_Positiva", "0" },
                        { "@nMVA_Negativa", "0" },
                        { "@nMVA_Neutra", "0" },
                        { "@dtVigencia_Inicial", "" },
                        { "@dtVigencia_Final", "" },
                        { "@sBase_Legal_ST", "" },
                        { "@dtEfeito_ST", "" },
                        { "@sNorma_Obs_ST", "" },
                        { "@sNorma_Base_Calculo", "" },
                        { "@sNorma_Prazo_Recolhimento", "" },
                        { "@sBase_Legal_int", "" },
                        { "@sObservacao_int", "" },
                        { "@sBase_Calculo_int", "" },
                        { "@sPrazo_Recolhimento_int", "" },
                        { "@sAplicabilidade", "" },
                        { "@sNaoAplicabilidade", "" },
                        { "@sVariacao_MVA", "" },
                        { "@sReducao_MVA", "" },
                        { "@sJSON", json.ToString() }
                    };
                    BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Fiscal_Regras", vParam);

                    return 0;
                }
            }
            catch (Exception ex)
            {
                MVA = 0;

                if (ex.Message.Contains("The underlying connection was closed")) goto final;

                string mensagem = "";

                try
                {
                    mensagem = $"Favor encaminhar um email, exibindo toda esta mensagem de erro junto com o número do Orçamento, para o t-flow@tecandtec.com.br <br /><b>Obs:</b> Ainda é possível Salvar as informações do Orçamento, no entanto é provável que alguns produtos do Orçamento não possuam valor de ST.<br /><br /><b>Erro:</b> Houve um erro na tentativa de Consultar e Salvar as informações do ICMS ST do Produto de NCM: {NCM} e CEST: {CEST}<br />Erro ao Consultar o ICMS ST: {ex.Message}";

                    Dictionary<string, string> vParam = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR_LEGISWEB_ERRO" },
                        { "@sMsg_Erro", mensagem },
                        { "@sJSON", json == null ? string.Empty : json.ToString() }
                    };
                    BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Fiscal_Regras", vParam);

                    throw ex;
                }
                catch (Exception ex1)
                {
                    mensagem = $"Favor encaminhar um email, exibindo toda esta mensagem de erro junto com o número do Orçamento, para o t-flow@tecandtec.com.br <br /><b>Obs:</b> Ainda é possível Salvar as informações do Orçamento, no entanto é provável que alguns produtos do Orçamento não possuam valor de ST.<br /><br /><b>Erro:</b> Houve um erro na tentativa de Salvar o registro do erro, ocorrido após Consultar as informações do ICMS ST do Produto de NCM: {NCM} e CEST: {CEST}<br />Segue mensagem de erro gerada ao Salvar o erro: {ex1.Message}<br /><br />Segue mensagem de erro gerada ao Consultar a API do Legis Web: {ex.Message}";

                    throw new Exception(mensagem);
                }
            }

        final:
            return MVA;
        }

        /// <summary>
        /// Função utilizada para Validar os dados da requisição para a API 'LegisWeb'.
        /// </summary>
        /// <returns>Retorna o dado validado e transformado.</returns>
        public static string Validar_Dados_ST_LegisWeb(string dado, int tipo)
        {
            string retorno = dado;

            switch (tipo) // tipos: 0 = string, 1 = decimal
            {
                case 0:
                    retorno = string.IsNullOrEmpty(dado) ? string.Empty : dado.Replace("\\/", "/").Trim();
                    break;

                case 1:
                    decimal.TryParse(dado.Replace(".", ",").Trim(), out decimal valor);
                    retorno = Math.Round(valor, 2).ToString().Replace(",", ".");
                    break;
            }

            return retorno;
        }

        /// <summary>
        /// Função utilizada para Validar e Consultar o MVA de um Item, utilizando uma requisição à API do LegisWeb.
        /// </summary>
        /// <param name="UF_Origem">Recebe o UF de Origem.</param>
        /// <param name="UF_Destino">Recebe o UF de Destino.</param>
        /// <param name="mva">Recebe a porcentagem do MVA.</param>
        /// <param name="bRevenda">Define se é uma 'Revenda'.</param>
        /// <param name="bST">Define se a Regra Fiscal do Item em questão permite o ICMS ST.</param>
        /// <param name="ncm">Recebe o código NCM do Item.</param>
        /// <param name="icms">Recebe a porcentagem de ICMS do Item.</param>
        /// <param name="sMsg_Erro_LegisWeb">Recebe a mensagem de Erro salva na tabela de Parâmetros.</param>
        public static decimal Retorna_MVA_LegisWeb(string UF_Origem, string UF_Destino, decimal mva, bool bRevenda, bool bST, string ncm, string cest, decimal icms, string dtUltimaConsulta, string sMsg_Erro_LegisWeb)
        {
            if (!bRevenda || !bST) return 0;
            if (mva > decimal.Zero) return mva;
            if (!string.IsNullOrEmpty(sMsg_Erro_LegisWeb)) throw new Exception(sMsg_Erro_LegisWeb);

            if (!string.IsNullOrEmpty(cest) && !cest.StartsWith("N") && !string.IsNullOrEmpty(ncm) && !ncm.StartsWith("N"))
            {
                if (DateTime.TryParse(dtUltimaConsulta, out DateTime dtConsulta) ? dtConsulta <= DateTime.Today.AddMonths(-1) : true)
                    mva = Consultar_ST_LegisWeb(UF_Origem, UF_Destino, ncm, cest, icms);
            }
            else mva = 0;

            return mva;
        }

        /// <summary>
        /// Função utilizada para Calcular o Valor Total de ST.
        /// </summary>
        /// <param name="total">Recebe o Valor Total do item.</param>
        /// <param name="ipi">Recebe o valor do IPI do item.</param>
        /// <param name="qtd">Recebe a quantidade do item.</param>
        /// <param name="MVA">Recebe o valor do MVA do Item.</param>
        /// <param name="ICMS_interno_destino">Recebe a porcentagem de ICMS interno do Estado de Destino do Item.</param>
        /// <returns>Retorna uma tupla com o valor do ST e o valor do MVA utilizado no cálculo.</returns>
        public static (decimal, decimal) CalculaValor_ST(decimal total, decimal icms, decimal MVA, decimal ICMS_interno_destino)
        {
            decimal BC_ST = total * (MVA / 100 + 1);
            return (Math.Round(BC_ST * (ICMS_interno_destino / 100) - icms, 2), Math.Round(MVA, 2));
        }

        /// <summary>
        /// Função para aplicar uma marca d'água à um arquivo PDF.
        /// </summary>
        /// <param name="arquivo">Recebe o nome, caminho completo ou o próprio arquivo PDF.</param>
        /// <param name="imagem">Recebe o nome, caminho completo ou a própria Imagem.</param>
        /// <param name="nAjusteEsquerda">Recebe o valor de ajuste da posição da Imagem, a partir da borda à esquerda do PDF.</param>
        /// <param name="nAjusteBaixo">Recebe o valor de ajuste da posição da Imagem, a partir da borda em baixo do PDF.</param>
        /// <param name="nAjusteAltura">Recebe o valor de ajuste da altura da Imagem.</param>
        /// <param name="nRotacao">Recebe o valor da Rotação da Imagem.</param>
        /// <param name="nOpacidade">Recebe o valor da Opacidade da Imagem.</param>
        /// <returns>Retorna o array de byte do Arquivo, já com a marca d'água.</returns>
        public static byte[] Adiciona_MarcaDagua(object arquivo, object imagem, string sTexto, BaseFont fonte = null, BaseColor cor = null, int nTamanhoFonte = 70, int nAjusteEsquerda = 100, int nAjusteBaixo = 50, int nAjusteAltura = -100,
            int nRotacao = 30, float nOpacidade = 0.15f, int texto_X = 0, int texto_Y = 0)
        {
            byte[] bPDF = Arquivo.Transforma_Arquivo_x_Bytes(arquivo);

            using (var msInput = new MemoryStream(bPDF))
            using (var msOutput = new MemoryStream())
            {
                PdfReader pdfReader = new PdfReader(msInput);
                PdfStamper pdfStamper = new PdfStamper(pdfReader, msOutput);

                Image img = null;
                if (imagem != null)
                {
                    img = Image.GetInstance(Arquivo.Transforma_Arquivo_x_Bytes(imagem));
                    img.RotationDegrees = nRotacao;
                }
                else if (!string.IsNullOrEmpty(sTexto))
                {
                    fonte = fonte is null ? BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED) : fonte;
                    cor = cor is null ? BaseColor.RED : cor;
                }

                for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                {
                    Rectangle pageSize = pdfReader.GetPageSizeWithRotation(i);
                    PdfContentByte cb = pdfStamper.GetOverContent(i);

                    cb.SetGState(new PdfGState { FillOpacity = nOpacidade, StrokeOpacity = nOpacidade });

                    if (img != null)
                    {
                        img.SetAbsolutePosition(pageSize.Left + nAjusteEsquerda, pageSize.Bottom + nAjusteBaixo);
                        img.ScaleToFit(pageSize.Width, pageSize.Height + nAjusteAltura);

                        cb.AddImage(img);
                    }
                    else if (!string.IsNullOrEmpty(sTexto))
                    {
                        cb.BeginText();
                        cb.SetColorFill(cor);
                        cb.SetFontAndSize(fonte, nTamanhoFonte);

                        float ajuste = 25;
                        float x = pageSize.Width / 2;
                        float y = pageSize.Height / 2;

                        cb.ShowTextAligned(Element.ALIGN_CENTER, sTexto, texto_X > 0 ? texto_X : x + ajuste, texto_Y > 0 ? texto_Y : y - ajuste, nRotacao);
                        cb.EndText();
                    }
                }

                pdfStamper.Close();
                pdfReader.Close();

                return msOutput.ToArray();
            }
        }

        public class Scripts
        {
            public static void DirecionarPagina(Page page, string sUrl)
            {
                string url = "";
                try
                {
                    url = HttpContext.Current.Request.UrlReferrer.Scheme + "://" + HttpContext.Current.Request.UrlReferrer.Authority + sUrl;
                }
                catch
                {
                    url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + sUrl;
                }

                string script = $"window.location.replace('{url}');";
                ScriptManager.RegisterStartupScript(page, page.GetType(), "js_FuncoesScripts_Redirect", script, true);
            }

            /// <summary>
            /// Método para aplicar Foco à um campo da página.
            /// </summary>
            /// <param name="page">Recebe a página onde está o campo.</param>
            /// <param name="clientID">Recebe o ClientID do campo à ser focado.</param>
            public static void FocusScript(Page page, string clientID) => RegistraScript(page, "js_FuncoesScripts_Focus", $"setTimeout(function() {{ $('#{clientID}').focus(); }}, 150);\r\n\r\n");

            /// <summary>
            /// Método para Esconder um campo da página.
            /// </summary>
            /// <param name="page">Recebe a página onde está o campo.</param>
            /// <param name="clientID">Recebe o ClientID do campo à ser escondido.</param>
            /// <param name="bClasse">Recebe booleano que define se o parâmetro 'clientID' deve ser usado como ID ou classe CSS.</param>
            public static void EsconderCampo(Page page, string clientID, bool bClasse) => RegistraScript(page, "js_FuncoesScripts_Hide", bClasse ? string.Format("$('.{0}').hide();\r\n\r\n", clientID) : string.Format("$('[id*={0}]').hide();\r\n\r\n", clientID));

            /// <summary>
            /// Método para Exibir um campo da página.
            /// </summary>
            /// <param name="page">Recebe a página onde está o campo.</param>
            /// <param name="clientID">Recebe o ClientID do campo à ser exibido.</param>
            /// <param name="bClasse">Recebe booleano que define se o parâmetro 'clientID' deve ser usado como ID ou classe CSS.</param>
            public static void AparecerCampo(Page page, string clientID, bool bClasse) => RegistraScript(page, "js_FuncoesScripts_Show", bClasse ? string.Format("$('.{0}').show();\r\n\r\n", clientID) : string.Format("$('[id*={0}]').show();\r\n\r\n", clientID));

            /// <summary>
            /// Método para aplicar Máscara à um campo, adicionando o padrão de um CPF.
            /// </summary>
            /// <param name="page">Recebe a página onde está o campo.</param>
            /// <param name="clientID">Recebe o ClientID do campo que receberá a Máscara.</param>
            public static void MascaraCPF(Page page, string clientID) => RegistraScript(page, "js_FuncoesScripts_MaskCPF", "$('[id*=" + clientID + "]').mask('000.000.000-00', { reverse: true });\r\n\r\n");

            /// <summary>
            /// Método para aplicar Máscara à um campo, adicionando o padrão de um CNPJ.
            /// </summary>
            /// <param name="page">Recebe a página onde está o campo.</param>
            /// <param name="clientID">Recebe o ClientID do campo que receberá a Máscara.</param>
            public static void MascaraCNPJ(Page page, string clientID) => RegistraScript(page, "js_FuncoesScripts_MaskCNPJ", "$('[id*=" + clientID + "]').mask('00.000.000/0000-00', { reverse: true });\r\n\r\n");

            /// <summary>
            /// Método para aplicar Máscara à um campo, adicionando o padrão de um Valor Inteiro.
            /// </summary>
            /// <param name="page">Recebe a página onde está o campo.</param>
            /// <param name="clientID">Recebe o ClientID do campo que receberá a Máscara.</param>
            public static void MascaraValorInteiro(Page page, string clientID) => RegistraScript(page, "js_FuncoesScripts_MaskInt", "$('[id*=" + clientID + "]').mask('00000000', { reverse: true });\r\n\r\n");

            /// <summary>
            /// Método para aplicar Máscara à um campo, adicionando o padrão de um Valor Decimal com 2 casas decimais.
            /// </summary>
            /// <param name="page">Recebe a página onde está o campo.</param>
            /// <param name="clientID">Recebe o ClientID do campo que receberá a Máscara.</param>
            public static void MascaraValorDecimal(Page page, string clientID) => RegistraScript(page, "js_FuncoesScripts_MaskDecimal2", "$('[id*=" + clientID + "]').mask('00000009,99', { reverse: true });\r\n\r\n");

            /// <summary>
            /// Método para aplicar Máscara à um campo, adicionando o padrão de um Valor Decimal com 4 casas Decimais.
            /// </summary>
            /// <param name="page">Recebe a página onde está o campo.</param>
            /// <param name="clientID">Recebe o ClientID do campo que receberá a Máscara.</param>
            public static void MascaraValorDecimalPreciso(Page page, string clientID) => RegistraScript(page, "js_FuncoesScripts_MaskDecimal4", "$('[id*=" + clientID + "]').mask('00000009,9999', { reverse: true });\r\n\r\n");

            /// <summary>
            /// Método para aplicar Máscara à um campo, adicionando o padrão de Data dd/mm/yyyy.
            /// </summary>
            /// <param name="page">Recebe a página onde está o campo.</param>
            /// <param name="clientID">Recebe o ClientID do campo que receberá a Máscara.</param>
            public static void MascaraDatas(Page page, string clientID) => RegistraScript(page, "js_FuncoesScripts_MaskDate", string.Format("$('[id*={0}]').mask('99/99/9999');\r\n\r\n", clientID));

            /// <summary>
            /// Método para aplicar Máscara à um campo, adicionando o padrão de um Data dd/mm/yyyy, e também com o DatePicker que incrementa um pequeno modal para que o Usuário selecione a Data desejada.
            /// </summary>
            /// <param name="page">Recebe a página onde está o campo.</param>
            /// <param name="clientID">Recebe o ClientID do campo que receberá a Máscara e o DatePicker.</param>
            public static void MascaraDatas_ComDatePicker(Page page, string clientID) => RegistraScript(page, "js_FuncoesScripts_MaskDatePicker", "$('[id*=" + clientID + "]').datepicker({ autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'}); $('[id*=" + clientID + "]').mask('99/99/9999');\r\n\r\n");

            /// <summary>
            /// Método para aplicar MultiSelect à um asp:ListBox, para permitir selecionar várias opções, além de adicionar um botão de selecionar todos os Itens. <br />
            /// <b>Obs:</b> Este método <u>não</u> é responsável por popular os Itens do asp:ListBox, apenas aplica o MutliSelect via Script.
            /// </summary>
            /// <param name="page">Recebe a página onde está o campo.</param>
            /// <param name="sClientID">Recebe o ClientID do campo que receberá o MultiSelect.</param>
            /// <param name="bAlinhaDireita">Recebe um valor que define se o campo de seleção do MultiSelect estará alinhado à direita.</param>
            /// <param name="sNCampos">Recebe o texto que aparecerá quando um ou mais Itens forem selecionados.</param>
            /// <param name="sTodosCampos">Recebe o texto que aparecerá quando todos os Itens estiverem selecionados.</param>
            /// <param name="bAdicionaFiltro">Recebe um valor que define se a opção de filtro aparecerá ou não.</param>
            public static void AplicaMultiSelect(Page page, string sClientID, bool bAlinhaDireita, string sNenhumCampo, string sNCampos, string sTodosCampos, bool bAdicionaFiltro)
            {
                StringBuilder script = new StringBuilder();

                script.Append("$('[id*=\"" + sClientID + "\"]').multiselect({");
                script.Append(" buttonWidth: '100%',");
                script.Append(" includeSelectAllOption: true,");
                script.Append(" maxHeight: 300,");
                script.Append(" dropRight: " + bAlinhaDireita.ToString().ToLower() + ", ");
                script.Append(" nSelectedText: ' - " + sNCampos + " selecionados',");
                script.Append(" allSelectedText: '" + sTodosCampos + "',");
                script.Append(" nonSelectedText: '" + sNenhumCampo + "',");
                script.Append(" enableFiltering: " + bAdicionaFiltro.ToString().ToLower());

                if (bAdicionaFiltro)
                {
                    script.Append(", filterBehavior: 'text',");
                    script.Append(" enableCaseInsensitiveFiltering: true,");
                    script.Append(" filterPlaceholder: 'Buscar " + sNCampos + "...',");
                    script.Append(@" filter: function() {
                                        var input = $(this).val().toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, '');
                                        $('.multiselect-container .multiselect-option').each(function() {
                                            var text = $(this).text().toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, '');
                                            $(this).toggle(text.indexOf(input) > -1);
                                        });
                                    }");
                }

                script.Append("});\r\n\r\n");
                script.Append("$('.multiselect-item').find('.input-group').find('.input-group-addon').find('i').removeClass().addClass('fa fa-search');\r\n\r\n");
                script.Append("$('.multiselect-item').find('.input-group').find('.input-group-btn').find('i').removeClass().addClass('fa fa-times');\r\n\r\n");

                RegistraScript(page, "js_FuncoesScripts_MultiSelect", script.ToString());
            }

            /// <summary>
            /// Método para abrir Modal 'manualmente' via Script. <br /> <br />
            /// <b>Obs:</b> Este método não é recomendado para 'Modais padrão', como por exemplo um modal para validar se o Usuário deseja Salvar as Alterações da Página. <br/> 
            /// Este método é mais recomendado para casos de 'Modais personalizados', como por exemplo um modal que exibe uma Grid de dados ou uma 'sub-Tela' para Cadastro.
            /// </summary>
            /// <param name="page">Recebe a página onde está o campo.</param>
            /// <param name="clientID">Recebe o ClientID do modal.</param>
            public static void AbrirModal(Page page, string clientID) => RegistraScript(page, "js_FuncoesScripts_AbrirModal", "$('#" + clientID + "').modal('show');\r\n\r\n");

            /// <summary>
            /// Método para fechar Modal 'manualmente' via Script. <br /> <br />
            /// <b>Obs:</b> Este método não é recomendado para 'Modais padrão', como por exemplo um modal para validar se o Usuário deseja Salvar as Alterações da Página. <br/> 
            /// Este método é mais recomendado para casos de 'Modais personalizados', como por exemplo um modal que exibe uma Grid de dados ou uma 'sub-Tela' para Cadastro.
            /// </summary>
            /// <param name="page">Recebe a página onde está o campo.</param>
            /// <param name="clientID">Recebe o ClientID do modal.</param>
            public static void FecharModal(Page page, string clientID) => RegistraScript(page, "js_FuncoesScripts_FecharModal", "$('#" + clientID + "').modal('hide');\r\n$('.modal-backdrop').remove();\r\n$('body').removeClass('modal-open');\r\n\r\n");

            /// <summary>
            /// Método para remover o Backdrop do modal.
            /// </summary>
            /// <param name="page">Recebe a página onde está o campo.</param>
            public static void RemoverBackdrop_Modal(Page page) => RegistraScript(page, "js_FuncoesScripts_RemoverBackdrop_Modal", "$('.modal-backdrop').remove();\r\n$('body').removeClass('modal-open');\r\n\r\n");

            /// <summary>
            /// Método para aplicar um 'Tooltip' personalizado à Página.<br /><br />
            /// <b>Modo de uso: </b><br />
            /// Basta chamar este método uma vez, passando um valor padrão como o segundo parâmetro 'tooltip', este parâmetro pode possuir qualquer valor dentro de uma string, mas preferencialmente use uma palavra única,<br />
            /// sem acentuação ou caracteres especiais.<br />
            /// Para aplicar o Tooltip personalizado nos campos desejados da sua Página, será necessário adicionar o atributo 'data-toggle' com o valor que foi passado como o segundo parâmetro deste método, por exemplo: data-toggle="tooltip".<br />
            /// Vale citar que é importante que o segundo parâmetro, 'tooltip', passado para este método deve ser igual ao valor colocado no atributo 'data-toggle', além disto, para definir o texto que aparecerá no Tooltip, basta adicionar o atributo 'title' ao campo,<br />
            /// e neste 'title' colocar o texto que deve aparecer no campo. <br /><br />
            /// 
            /// <b>Exemplo Prático:</b><br />
            /// *No código C#:<br />
            /// Aplica_TooltipPersonalizado(Page, "tooltip");<br /><br />
            /// 
            /// *No HTML:<br />
            /// input type="submit" name="cancel" class="btn btn-lg btn-warning" id="field_cancel" <b>data-toggle="tooltip" title="Voltar"</b> onclick="history.go(-1)" />
            /// </summary>
            /// <param name="page">Recebe a página onde está o campo.</param>
            /// <param name="tooltip">Recebe o valor que será aplicado o 'Tooltip personalizado.'</param>
            public static void Aplica_TooltipPersonalizado(Page page, string tooltip = "tooltip", string posicao = "top", bool bAreaColaborador = false) => RegistraScript(page, "js_FuncoesScripts_TooltipPersonalizado_" + Guid.NewGuid(), !bAreaColaborador ? "$('[data-toggle=" + tooltip + "]').tooltip({ placement: '" + posicao + "', html: true, boundary: 'window' });\r\n\r\n" : "$('[data-bs-toggle=\"" + tooltip + "\"]').each(function () { new bootstrap.Tooltip(this, {placement: '" + posicao + "', html: true, boundary: 'window' }); });\r\n\r\n");

            /// <summary>
            /// Método para registrar um Log, com uma Mensagem específica, no Console do Navegador.
            /// </summary>
            /// <param name="page">Recebe a página onde será registrado o Log.</param>
            /// <param name="mensagem">Recebe a mensagem que será exibida no Console.'</param>
            /// <param name="tipo">Recebe o tipo de mensagem que será exibido no Console.'</param>
            public static void Registra_Log_noConsole_doNavegador(Page page, string mensagem, string tipo = "log") => RegistraScript(page, "js_FuncoesScripts_ConsoleLog", $"console.{tipo}('{mensagem}');\r\n\r\n");

            /// <summary>
            /// Método para manter uma aba como Ativa.
            /// </summary>
            /// <param name="page">Recebe a página onde está a aba.</param>
            /// <param name="sClientID">Recebe o ClientID da aba.</param>
            public static void Mantem_AbaAtiva(Page page, string sClientID) => RegistraScript(page, "js_FuncoesScripts_AbaAtiva", "$('#" + sClientID + "').tab('show');\r\n\r\n");

            /// <summary>
            /// Método para Deativar o evento de Click de um Botão, via script.
            /// </summary>
            /// <param name="page">Recebe a página onde está o botão.</param>
            /// <param name="sClientID">Recebe o ClientID do botão.'</param>
            public static void DesativaClick(Page page, string sClientID) => RegistraScript(page, "js_FuncoesScripts_DesativaClick", "$('#" + sClientID + "').click(function(e) { e.preventDefault(); });\r\n\r\n");

            /// <summary>
            /// Método para forçar o Recarregamento da Página atual.
            /// </summary>
            /// <param name="page">Recebe a Página.</param>
            public static void RecarregaPagina(Page page) => RegistraScript(page, "js_FuncoesScripts_RecarregaPagina", "window.location.reload(true);\r\n\r\n");

            /// <summary>
            /// Método para abrir um elemento colapsado.
            /// </summary>
            /// <param name="page">Recebe a Página.</param>
            /// <param name="sClientID">Recebe o ClientID do elemento.</param>
            public static void AbrirCollapse(Page page, string sClientID) => RegistraScript(page, "js_FuncoesScripts_AbrirCollapse", "$('#" + sClientID + "').collapse('show');\r\n\r\n");

            /// <summary>
            /// Método para fechar um elemento colapsado.
            /// </summary>
            /// <param name="page">Recebe a Página.</param>
            /// <param name="sClientID">Recebe o ClientID do elemento.</param>
            public static void FecharCollapse(Page page, string sClientID) => RegistraScript(page, "js_FuncoesScripts_FecharCollapse", "$('#" + sClientID + "').collapse('hide');\r\n\r\n");

            public static void RegistraScript(Page page, string key, string script) => ScriptManager.RegisterStartupScript(page, page.GetType(), key, script, true);
        }

        public class cls_Multiplos_Combos
        {
            public DropDownList ddl { get; set; }
            public ListBox lst { get; set; }
            public string sCampoCodigo { get; set; }
            public string sCampoDescricao { get; set; }
            public bool bConcatenarCodigo_Descricao { get; set; }
            public string sMensagemPrimeiraLinha { get; set; }
            public string sValorPrimeiraLinha { get; set; }
        }
    }

    public class Validacoes
    {
        public static string ValidaDatas(string dtInicial, string dtFinal)
        {
            return ValidaDatas(dtInicial, dtFinal, true);
        }

        public static string ValidaDatas(string dtInicial, string dtFinal, bool bPermiteHoje)
        {
            string sErro = "";

            DateTime dtAvaliacao = DateTime.MinValue;
            DateTime dtCompara1 = Convert.ToDateTime("01/01/1900");
            DateTime dtCompara2 = Convert.ToDateTime("01/01/1900");
            DateTime dtHoje = Convert.ToDateTime(DateTime.Now.ToShortDateString());


            if (DateTime.TryParse(dtInicial.Trim(), out dtAvaliacao))
            {
                dtCompara1 = Convert.ToDateTime(dtInicial.Trim());
            }
            else
            {
                sErro = "Data Inicial Inválida!";

            }

            if (sErro == "" && (DateTime.TryParse(dtFinal.Trim(), out dtAvaliacao)))
            {
                dtCompara2 = Convert.ToDateTime(dtFinal.Trim());
            }
            else
            {
                sErro = "Data Final Inválida!";
            }


            if (sErro == "")
            {
                int nResultado = DateTime.Compare(dtCompara1, dtCompara2);
                if (nResultado > 0)
                {
                    sErro = "Data inicial não pode ser maior que a data final!";
                }
            }

            if (sErro == "")
            {
                if (!bPermiteHoje)
                {
                    int nResultado = DateTime.Compare(dtCompara2, dtHoje);
                    if (nResultado >= 0)
                    {
                        sErro = "Data final não pode ser maior ou igual a hoje!";
                    }
                }
            }


            return sErro;
        }

        public static string ValidaDatas_Hora(string dtInicial, string dtFinal)
        {
            string sErro = "";

            DateTime dtAvaliacao = DateTime.MinValue;
            DateTime dtCompara1 = Convert.ToDateTime("01/01/1900");
            DateTime dtCompara2 = Convert.ToDateTime("01/01/1900");


            if (DateTime.TryParse(dtInicial.Trim(), out dtAvaliacao))
            {
                dtCompara1 = Convert.ToDateTime(dtInicial.Trim());
            }
            else
            {
                sErro = "Data Inicial Inválida!";

            }

            if (sErro == "" && (DateTime.TryParse(dtFinal.Trim(), out dtAvaliacao)))
            {
                dtCompara2 = Convert.ToDateTime(dtFinal.Trim());
            }
            else
            {
                sErro = "Data Final Inválida!";
            }


            if (sErro == "")
            {
                int nResultado = DateTime.Compare(dtCompara1, dtCompara2);
                if (nResultado > 0)
                {
                    sErro = "Data inicial não pode ser maior que a data final!";
                }
            }


            return sErro;
        }

        public static bool ValidaCPF(string sCPF)
        {
            string valor = sCPF.Replace(".", "");
            valor = valor.Replace("-", "");
            valor = valor.Trim();
            valor = valor.PadLeft(11, '0');



            bool igual = true;
            for (int i = 1; i < 11 && igual; i++)
                if (valor[i] != valor[0])
                    igual = false;
            if (igual || valor == "12345678909")
                return false;
            int[] numeros = new int[11];
            for (int i = 0; i < 11; i++)
                numeros[i] = int.Parse(
                valor[i].ToString());
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (10 - i) * numeros[i];
            int resultado = soma % 11;
            if (resultado == 1 || resultado == 0)
            {
                if (numeros[9] != 0)
                    return false;
            }

            else if (numeros[9] != 11 - resultado)

                return false;



            soma = 0;

            for (int i = 0; i < 10; i++)

                soma += (11 - i) * numeros[i];



            resultado = soma % 11;



            if (resultado == 1 || resultado == 0)
            {

                if (numeros[10] != 0)

                    return false;
            }

            else
                if (numeros[10] != 11 - resultado)

                return false;

            return true;

        }

        public static bool ValidaCNPJ(string sCnpj)
        {

            string CNPJ = sCnpj.Replace(".", "");

            CNPJ = CNPJ.Replace("/", "");
            CNPJ = CNPJ.Trim();
            CNPJ = CNPJ.Replace("-", "");
            CNPJ = CNPJ.PadLeft(14, '0');


            int[] digitos, soma, resultado;

            int nrDig;

            string ftmt;

            bool[] CNPJOk;



            ftmt = "6543298765432";

            digitos = new int[14];

            soma = new int[2];

            soma[0] = 0;

            soma[1] = 0;

            resultado = new int[2];

            resultado[0] = 0;

            resultado[1] = 0;

            CNPJOk = new bool[2];

            CNPJOk[0] = false;

            CNPJOk[1] = false;



            try
            {

                for (nrDig = 0; nrDig < 14; nrDig++)
                {

                    digitos[nrDig] = int.Parse(

                        CNPJ.Substring(nrDig, 1));

                    if (nrDig <= 11)

                        soma[0] += (digitos[nrDig] *

                          int.Parse(ftmt.Substring(

                          nrDig + 1, 1)));

                    if (nrDig <= 12)

                        soma[1] += (digitos[nrDig] *

                          int.Parse(ftmt.Substring(

                          nrDig, 1)));

                }



                for (nrDig = 0; nrDig < 2; nrDig++)
                {

                    resultado[nrDig] = (soma[nrDig] % 11);

                    if ((resultado[nrDig] == 0) || (

                         resultado[nrDig] == 1))

                        CNPJOk[nrDig] = (

                        digitos[12 + nrDig] == 0);

                    else

                        CNPJOk[nrDig] = (

                        digitos[12 + nrDig] == (

                        11 - resultado[nrDig]));

                }

                return (CNPJOk[0] && CNPJOk[1]);

            }

            catch
            {

                return false;

            }

        }

        public static bool ValidarEmail(string sEmail)
        {
            return ValidarEmail(sEmail, true);
        }

        public static bool ValidarEmail(string sEmail, bool Obrigatorio)
        {
            bool lValidEmail = false;

            if (Obrigatorio || !string.IsNullOrEmpty(sEmail))
            {
                Regex rg = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");
                if (rg.IsMatch(sEmail))
                {
                    lValidEmail = true;
                }
            }
            else if (!Obrigatorio && string.IsNullOrEmpty(sEmail))
            {
                lValidEmail = true;
            }

            return lValidEmail;
        }

        public static bool ValidarTelefone(string sTelefone)
        {
            bool lValidTelefone = false;

            Regex rg = new Regex(@"^\(\d{2}\) \d{4,5}-\d{4}$");
            if (rg.IsMatch(sTelefone))
            {
                lValidTelefone = true;
            }

            return lValidTelefone;
        }

        public static bool ValidarNumerico(TextBox txt)
        {
            bool bRetorno = false;
            int resultado;
            if (int.TryParse(txt.Text, out resultado))
            {
                bRetorno = true;
            }
            return bRetorno;

        }

        public static bool ValidarMoeda(TextBox txt)
        {
            bool bRetorno = false;
            double resultado;
            if (double.TryParse(txt.Text, out resultado))
            {
                bRetorno = true;
            }
            return bRetorno;

        }

        public static bool ValidarData(TextBox txt)
        {
            bool bRetorno = false;

            if (DateTime.TryParse(txt.Text, out DateTime resultado))
            {
                //Agnes Partal * 10/12/2024 ----------

                DateTime minDate = new DateTime(1900, 1, 1);
                DateTime maxDate = new DateTime(2100, 12, 31);

                if (resultado < minDate || resultado > maxDate)
                    return bRetorno;
                //------------------------------------

                bRetorno = true;
            }

            return bRetorno;
        }

        public static bool ValidarTexto(TextBox txt)
        {
            bool bRetorno = false;

            if (string.IsNullOrEmpty(txt.Text)) { bRetorno = true; }
            ;

            return bRetorno;
        }
    }

    public class Exportacao
    {
        public static void ExportarGRID_Excel(GridView gv, string sNomeArquivo)
        {
            ExportarGRID_Excel(gv, "", sNomeArquivo);
        }

        public static void ExportarGRID_Excel(GridView gv, string sTitulo, string sNomeArquivo)
        {

            try
            {

                System.IO.StringWriter objSW = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter objHW = new HtmlTextWriter(objSW);


                //Cabeçalho para Download do Arquivo em Excel.
                string attachment = "attachment; filename=" + sNomeArquivo + "_" + CarimboDataHora() + ".xls";
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.AddHeader("content-disposition", attachment);
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Current.Response.ContentType = "application/ms-excel";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                HttpContext.Current.Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());

                if (!string.IsNullOrEmpty(sTitulo))
                {
                    GridViewRow gv_RowTitulo = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

                    TableCell cel_Titulo = new TableCell();
                    cel_Titulo.ColumnSpan = gv.Columns.Count;
                    cel_Titulo.Text = sTitulo;
                    cel_Titulo.Font.Bold = true;
                    cel_Titulo.Font.Size = 13; //Validar
                    cel_Titulo.HorizontalAlign = HorizontalAlign.Center;
                    cel_Titulo.VerticalAlign = VerticalAlign.Middle;

                    gv_RowTitulo.Cells.Add(cel_Titulo);

                    gv.Controls[0].Controls.AddAt(0, gv_RowTitulo);

                }

                //Imprime a GV no HTML
                gv.RenderControl(objHW);

                string sRetorno = objSW.ToString();
                //Case tenha botão de excluir, precisa validar os outros tipos 
                sRetorno = sRetorno.Replace("Button", " ");

                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                HttpContext.Current.Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
                HttpContext.Current.Response.Write(sRetorno);
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar Excel: " + ex.Message);
            }
        }

        public static void ExportarPainel_Excel(Page page, Panel pn, string sNomeArquivo)
        {
            string attachment = "attachment; filename=" + sNomeArquivo + "_" + CarimboDataHora() + ".xls";
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.AddHeader("content-disposition", attachment);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htw = new HtmlTextWriter(sw);

            Page pg = new Page();
            HtmlForm frm = new HtmlForm();
            pg.Controls.Add(frm);
            frm.Controls.Add(pn);
            pg.RenderControl(htw);

            HttpContext.Current.Session["sTipo"] = "GRID";
            string sRetorno = sw.ToString();
            sRetorno = sRetorno.Replace("Button", " ");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            HttpContext.Current.Response.Write(sRetorno);

            ScriptManager.RegisterStartupScript(page, page.GetType(), "js_ScriptPagina" + Guid.NewGuid(), "$('[id*=div_Loading]').hide();$('[id*=div_img]').hide();", true);
            HttpContext.Current.Response.End();

        }

        public static string CarimboDataHora()
        {


            //string sCarimbo = DateTime.Now.Year.ToString("yyyy") + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            //return sCarimbo;
            return DateTime.Now.ToString("yyyyMMdd_HHmmss");
        }
        public static string ExportarConsultaSQLparaCSV(string pStrProcedureExecutar, Dictionary<string, string> pStrParametros, string pStrNomeArquivoSemExtensao)
        {

            string lStrNomeArquivoComExtensao = pStrNomeArquivoSemExtensao + CarimboDataHora() + ".csv";

            DataSet dsConsulta = new DataSet();

            if (pStrParametros != null)
            {
                dsConsulta = BD.ExecutarDataSet(pStrProcedureExecutar, pStrParametros);
            }

            lStrNomeArquivoComExtensao = ExportarConsultaSQLparaCSV(dsConsulta, pStrNomeArquivoSemExtensao);

            return lStrNomeArquivoComExtensao;
        }
        public static string ExportarConsultaSQLparaCSV(DataSet dsExportar, string pStrNomeArquivoSemExtensao)
        {
            string lStrNomeArquivoComExtensao = pStrNomeArquivoSemExtensao + CarimboDataHora() + ".csv";

            StreamWriter lSwArquivo = new StreamWriter(HttpContext.Current.Server.MapPath("~/Download/" + lStrNomeArquivoComExtensao), false, Encoding.Default);

            using (lSwArquivo)
            {
                StringBuilder lSbCabecalho = new StringBuilder();
                StringBuilder lSbLinha = new StringBuilder();

                //Grava Cabeçalho do Arquivo
                for (int ColunaAtual = 0; ColunaAtual < dsExportar.Tables[0].Columns.Count; ColunaAtual++)
                {
                    lSbCabecalho.Append(dsExportar.Tables[0].Columns[ColunaAtual].ToString().Trim() + ";");
                }
                lSwArquivo.WriteLine(lSbCabecalho.ToString());
                lSbCabecalho.Clear();

                for (int LinhaAtual = 0; LinhaAtual < dsExportar.Tables[0].Rows.Count; LinhaAtual++)
                {
                    for (int ColunaAtual = 0; ColunaAtual < dsExportar.Tables[0].Columns.Count; ColunaAtual++)
                    {
                        lSbLinha.Append(dsExportar.Tables[0].Rows[LinhaAtual][ColunaAtual].ToString().Trim() + ";");
                    }

                    lSwArquivo.WriteLine(lSbLinha.ToString());
                    lSbLinha.Clear();
                }

            }

            lSwArquivo.Close();
            lSwArquivo.Dispose();
            return lStrNomeArquivoComExtensao;
        }

        public static string ExportarConsultaSQLparaCSV(DataTable dsExportar, string pStrNomeArquivoSemExtensao)
        {
            string lStrNomeArquivoComExtensao = pStrNomeArquivoSemExtensao + CarimboDataHora() + ".csv";

            StreamWriter lSwArquivo = new StreamWriter(HttpContext.Current.Server.MapPath("~/Download/" + lStrNomeArquivoComExtensao), false, Encoding.Default);

            using (lSwArquivo)
            {
                StringBuilder lSbCabecalho = new StringBuilder();
                StringBuilder lSbLinha = new StringBuilder();

                //Grava Cabeçalho do Arquivo
                for (int ColunaAtual = 0; ColunaAtual < dsExportar.Columns.Count; ColunaAtual++)
                {
                    lSbCabecalho.Append(dsExportar.Columns[ColunaAtual].ToString().Trim() + ";");
                }
                lSwArquivo.WriteLine(lSbCabecalho.ToString());
                lSbCabecalho.Clear();

                for (int LinhaAtual = 0; LinhaAtual < dsExportar.Rows.Count; LinhaAtual++)
                {
                    for (int ColunaAtual = 0; ColunaAtual < dsExportar.Columns.Count; ColunaAtual++)
                    {
                        lSbLinha.Append(dsExportar.Rows[LinhaAtual][ColunaAtual].ToString().Trim() + ";");
                    }

                    lSwArquivo.WriteLine(lSbLinha.ToString());
                    lSbLinha.Clear();
                }

            }

            lSwArquivo.Close();
            lSwArquivo.Dispose();
            return lStrNomeArquivoComExtensao;
        }

        public static string ExportarConsultaSQLparaXLS(string pStrProcedureExecutar, Dictionary<string, string> pStrParametros, string pStrNomeArquivoSemExtensao)
        {

            DataSet dsConsulta = new DataSet();

            if (pStrParametros != null)
            {
                dsConsulta = BD.ExecutarDataSet(pStrProcedureExecutar, pStrParametros);
            }

            return ExportarConsultaSQLparaXLS(dsConsulta, pStrNomeArquivoSemExtensao);

        }
        public static string ExportarConsultaSQLparaXLS(DataSet dsExportar, string pStrNomeArquivoSemExtensao)
        {
            string lStrNomeArquivoComExtensao = pStrNomeArquivoSemExtensao + CarimboDataHora() + ".xls";

            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + lStrNomeArquivoComExtensao);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.ContentEncoding = Encoding.Default;

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            StringBuilder lSbExcel = new StringBuilder();
            StringBuilder lSbLinha = new StringBuilder();
            lSbExcel.Append("<style type=\"text/css\">\r\n");
            lSbExcel.Append(".tabHead\r\n");
            lSbExcel.Append("{\r\n");
            lSbExcel.Append("   background-color: #cccccc;\r\n");
            lSbExcel.Append("   border: solid 1px black;\r\n");
            lSbExcel.Append("}\r\n");
            lSbExcel.Append(".tabRow\r\n");
            lSbExcel.Append("{\r\n");
            lSbExcel.Append("   border: solid 1px black;\r\n");
            lSbExcel.Append("}\r\n");
            lSbExcel.Append("</style>\r\n\r\n");

            lSbExcel.AppendFormat("<table>\r\n");
            lSbExcel.AppendFormat("<thead>\r\n");
            lSbExcel.AppendFormat("<tr>\r\n");

            // Grava Cabeçalho do Arquivo
            for (int ColunaAtual = 0; ColunaAtual < dsExportar.Tables[0].Columns.Count; ColunaAtual++)
            {
                lSbLinha.Append("\t<td class=\"tabHead\">" + dsExportar.Tables[0].Columns[ColunaAtual].ToString().Trim() + "</td>\r\n");
            }
            lSbLinha.AppendFormat("</tr>\r\n");
            lSbLinha.AppendFormat("</thead>\r\n");
            lSbLinha.AppendFormat("<tbody>\r\n");

            lSbExcel.AppendFormat(lSbLinha.ToString());

            for (int LinhaAtual = 0; LinhaAtual < dsExportar.Tables[0].Rows.Count; LinhaAtual++)
            {
                lSbLinha.Clear();
                lSbLinha.AppendFormat("<tr>\r\n");
                for (int ColunaAtual = 0; ColunaAtual < dsExportar.Tables[0].Columns.Count; ColunaAtual++)
                {
                    lSbLinha.Append("\t<td class=\"tabRow\">" + dsExportar.Tables[0].Rows[LinhaAtual][ColunaAtual].ToString().Trim() + "</td>\r\n");
                }
                lSbLinha.AppendFormat("</tr>\r\n");
                lSbExcel.AppendFormat(lSbLinha.ToString());
            }

            lSbExcel.AppendFormat("</tbody>\r\n");
            lSbExcel.AppendFormat("</table>\r\n");

            HttpContext.Current.Response.Write(lSbExcel.ToString());
            HttpContext.Current.Response.End();

            return lStrNomeArquivoComExtensao;
        }
    }

    public class Excel
    {
        public static void CombineExcel(ISheet source, ISheet destination)
        {
            int rowCount = source.LastRowNum;
            for (int i = 0; i <= rowCount; i++)
            {
                IRow newRow = destination.CreateRow(i);
                IRow row = source.GetRow(i);
                if (row != null)
                {
                    foreach (ICell sourceCell in row.Cells)
                    {
                        ICell newCell = newRow.CreateCell(sourceCell.ColumnIndex);

                        // Copiar o conteúdo e o estilo da célula
                        switch (sourceCell.CellType)
                        {
                            case CellType.Boolean:
                                newCell.SetCellValue(sourceCell.BooleanCellValue);
                                break;
                            case CellType.Numeric:
                                newCell.SetCellValue(sourceCell.NumericCellValue);
                                break;
                            case CellType.String:
                                newCell.SetCellValue(sourceCell.StringCellValue);
                                break;
                            case CellType.Formula:
                                newCell.SetCellFormula(sourceCell.CellFormula);
                                break;
                            case CellType.Error:
                                newCell.SetCellErrorValue(sourceCell.ErrorCellValue);
                                break;
                            default:
                                break;
                        }
                        CopyCellStyles(sourceCell, newCell);

                        // Copiar largura da coluna
                        int sourceColumnIndex = sourceCell.ColumnIndex;
                        int targetColumnIndex = newCell.ColumnIndex;
                        destination.SetColumnWidth(targetColumnIndex, source.GetColumnWidth(sourceColumnIndex));

                        // Copiar altura da linha
                        newRow.Height = row.Height;

                        // Verificar e copiar mesclagem de célula
                        CellRangeAddress mergedRegion = GetMergedRegion(source, i, sourceCell.ColumnIndex);
                        if (mergedRegion != null && !IsMergedRegion(destination, mergedRegion))
                        {
                            destination.AddMergedRegion(new CellRangeAddress(mergedRegion.FirstRow, mergedRegion.LastRow, mergedRegion.FirstColumn, mergedRegion.LastColumn));
                        }

                    }
                }
            }
        }

        private static void CopyCellStyles(ICell sourceCell, ICell newCell)
        {
            ICellStyle sourceCellStyle = sourceCell.CellStyle;
            ICellStyle newCellStyle = newCell.Sheet.Workbook.CreateCellStyle();

            // Copiar propriedades básicas do estilo
            newCellStyle.Alignment = sourceCellStyle.Alignment;
            newCellStyle.BorderBottom = sourceCellStyle.BorderBottom;
            newCellStyle.BorderLeft = sourceCellStyle.BorderLeft;
            newCellStyle.BorderRight = sourceCellStyle.BorderRight;
            newCellStyle.BorderTop = sourceCellStyle.BorderTop;
            newCellStyle.BottomBorderColor = sourceCellStyle.BottomBorderColor;
            newCellStyle.DataFormat = sourceCellStyle.DataFormat;
            newCellStyle.FillBackgroundColor = IndexedColors.White.Index;
            newCellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            newCellStyle.FillPattern = sourceCellStyle.FillPattern;
            newCellStyle.Indention = sourceCellStyle.Indention;
            newCellStyle.LeftBorderColor = sourceCellStyle.LeftBorderColor;
            newCellStyle.RightBorderColor = sourceCellStyle.RightBorderColor;
            newCellStyle.Rotation = sourceCellStyle.Rotation;
            newCellStyle.ShrinkToFit = sourceCellStyle.ShrinkToFit;
            newCellStyle.TopBorderColor = sourceCellStyle.TopBorderColor;
            newCellStyle.VerticalAlignment = sourceCellStyle.VerticalAlignment;
            newCellStyle.WrapText = sourceCellStyle.WrapText;


            // Definir fonte
            IFont sourceFont = sourceCellStyle.GetFont(sourceCell.Sheet.Workbook);
            IFont newFont = newCell.Sheet.Workbook.CreateFont();
            newFont.IsBold = sourceFont.IsBold;
            newFont.Color = IndexedColors.Black.Index;
            newFont.FontHeight = sourceFont.FontHeight;
            newFont.FontName = sourceFont.FontName;
            newFont.IsItalic = sourceFont.IsItalic;
            newFont.IsStrikeout = sourceFont.IsStrikeout;
            newFont.Underline = sourceFont.Underline;
            newCellStyle.SetFont(newFont);

            // Aplicar o novo estilo à célula
            newCell.CellStyle = newCellStyle;
        }

        private static CellRangeAddress GetMergedRegion(ISheet sheet, int rowNum, int colNum)
        {
            for (int i = 0; i < sheet.NumMergedRegions; i++)
            {
                CellRangeAddress merged = sheet.GetMergedRegion(i);
                if (merged.IsInRange(rowNum, colNum))
                {
                    return merged;
                }
            }
            return null;
        }

        private static bool IsMergedRegion(ISheet sheet, CellRangeAddress mergedRegion)
        {
            for (int i = 0; i < sheet.NumMergedRegions; i++)
            {
                CellRangeAddress existingRegion = sheet.GetMergedRegion(i);
                if (existingRegion.FirstRow == mergedRegion.FirstRow &&
                    existingRegion.LastRow == mergedRegion.LastRow &&
                    existingRegion.FirstColumn == mergedRegion.FirstColumn &&
                    existingRegion.LastColumn == mergedRegion.LastColumn)
                {
                    return true;
                }
            }
            return false;
        }

        public static void CombineWorksheets(string filePath, IWorkbook combinedWorkbook, string sheetName)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = WorkbookFactory.Create(fs);
                ISheet sheet = workbook.GetSheetAt(0); // assume que a primeira aba será combinada

                ISheet newSheet = combinedWorkbook.CreateSheet(sheetName);
                CombineExcel(sheet, newSheet);
            }
        }
    }

    public class Arquivo
    {
        public static string SalvarArquivo(string sNomeArquivo, int idTipoArquivo, object arquivo, int idObjeto = 0, string sDscArquivo = "", string sObservacao = "", string dtExpiracaoDoc = "", string dtRegistroDoc = "")
        {
            string idArquivo = "0";
            byte[] bytes = null;

            bytes = Transforma_Arquivo_x_Bytes(arquivo);

            SqlDataAdapter da = new SqlDataAdapter("sp_Manipula_tbl_Flow_Arquivos", StringDeConexao);
            da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "INCLUIR";
            da.SelectCommand.Parameters.Add("@idTipoArquivo", SqlDbType.Int).Value = idTipoArquivo;
            da.SelectCommand.Parameters.Add("@idObjeto", SqlDbType.Int).Value = idObjeto;
            da.SelectCommand.Parameters.Add("@sNomeArquivo", SqlDbType.VarChar).Value = sNomeArquivo;
            da.SelectCommand.Parameters.Add("@sDscArquivo", SqlDbType.VarChar).Value = sDscArquivo;
            da.SelectCommand.Parameters.Add("@sObservacao", SqlDbType.VarChar).Value = sObservacao;
            da.SelectCommand.Parameters.Add("@vbArquivo", SqlDbType.VarBinary).Value = bytes;
            da.SelectCommand.Parameters.Add("@idUsuario", SqlDbType.Int).Value = Variaveis.idUsuario();
            da.SelectCommand.Parameters.Add("@dtExpiracaoDoc", SqlDbType.VarChar).Value = dtExpiracaoDoc;
            da.SelectCommand.Parameters.Add("@dtRegistroDoc", SqlDbType.VarChar).Value = dtRegistroDoc;

            try
            {
                DataSet tabela = new DataSet();
                da.Fill(tabela);
                idArquivo = DATASET(tabela, "idArquivo");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao salvar o arquivo: {ex.Message}");
            }

            return idArquivo;
        }

        public static byte[] Transforma_Arquivo_x_Bytes(object arquivo)
        {
            switch (arquivo)
            {
                case byte[] b: return b;

                case string s:
                    try
                    {
                        string caminhoCompleto = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Download", s);
                        if (File.Exists(s))
                            return File.ReadAllBytes(s);
                        else if (File.Exists(caminhoCompleto))
                            return File.ReadAllBytes(caminhoCompleto);
                        else
                            return Encoding.UTF8.GetBytes(s);
                    }
                    catch { return Encoding.UTF8.GetBytes(s); }

                case StringBuilder sb: return Encoding.UTF8.GetBytes(sb.ToString());
                case TextReader tr: return Encoding.UTF8.GetBytes(tr.ReadToEnd());

                case Stream st:
                    using (var ms = new MemoryStream())
                    {
                        st.CopyTo(ms);
                        return ms.ToArray();
                    }

                default: throw new ArgumentException("Tipo de arquivo não suportado.");
            }
        }

        /// <summary>
        /// Método para mover um arquivo para uma pasta específica.
        /// </summary>
        /// <param name="arquivo">Recebe o próprio arquivo como um FileStream para movê-lo.</param>
        /// <param name="caminhoPasta">Recebe o caminho da pasta para qual o arquivo será movido.</param>
        /// <param name="nomeArquivo">Recebe o nome do arquivo para que seja gerado na pasta especificada.</param>	
        public static string MoveArquivo(FileStream arquivo, string caminhoPasta, string nomeArquivo)
        {
            try
            {
                using (FileStream arquivoDestino = File.Create(string.Format("{0}{1}", HttpContext.Current.Request.UrlReferrer.Scheme + "://" + HttpContext.Current.Request.UrlReferrer.Authority + caminhoPasta, nomeArquivo)))
                {
                    arquivo.CopyTo(arquivoDestino);
                }

                return "Arquivo movido com sucesso!";
            }
            catch (Exception ex)
            {
                return $"Erro ao mover o arquivo: {ex.Message}";
            }
        }

        /// <summary>
        /// Método para mover um arquivo para uma pasta específica.
        /// </summary>
        /// <param name="caminhoArquivo">Recebe o caminho para o arquivo para movê-lo.</param>
        /// <param name="caminhoPasta">Recebe o caminho da pasta para qual o arquivo será movido.</param>
        /// <param name="nomeArquivo">Recebe o nome do arquivo para que seja gerado na pasta especificada.</param>
        public static string MoveArquivo(string caminhoArquivo, string caminhoPasta, string nomeArquivo)
        {
            try
            {
                File.Move(caminhoArquivo, string.Format("{0}{1}", HttpContext.Current.Request.UrlReferrer.Scheme + "://" + HttpContext.Current.Request.UrlReferrer.Authority + caminhoPasta, nomeArquivo));
                return "Arquivo movido com sucesso.";
            }
            catch (Exception ex)
            {
                return $"Erro ao mover o arquivo: {ex.Message}";
            }
        }

        public static string MoveArquivo_Windows(string caminhoArquivo, string caminhoPasta, string nomeArquivo)
        {
            try
            {

                if (!Directory.Exists(caminhoPasta))
                {
                    Directory.CreateDirectory(caminhoPasta);
                }

                File.Move(caminhoArquivo, string.Format("{0}{1}", caminhoPasta, nomeArquivo));
                return "Arquivo movido com sucesso.";
            }
            catch (Exception ex)
            {
                return $"Erro ao mover o arquivo: {ex.Message}";
            }
        }

        public static string CopiarArquivo_Windows(string caminhoArquivo, string caminhoPasta, string nomeArquivo)
        {
            try
            {

                if (!Directory.Exists(caminhoPasta))
                {
                    Directory.CreateDirectory(caminhoPasta);
                }

                File.Copy(caminhoArquivo, string.Format("{0}{1}", caminhoPasta, nomeArquivo));
                return "Arquivo movido com sucesso.";
            }
            catch (Exception ex)
            {
                return $"Erro ao mover o arquivo: {ex.Message}";
            }
        }

        public byte[] TransformaArquivoEmArrayBytes(string pStrNomeArquivo, Stream pObjConteudoArquivo) => TransformaArquivoEmArrayBytes(pStrNomeArquivo, "", pObjConteudoArquivo);

        public byte[] TransformaArquivoEmArrayBytes(string pStrNomeArquivo, string pStrCaminhoArquivo, Stream pObjConteudoArquivo)
        {
            try { return new BinaryReader(pObjConteudoArquivo).ReadBytes((int)pObjConteudoArquivo.Length); } catch (Exception ex) { throw ex; }
        }

        public FileStream TransformarArrayBytesEmArquivo(byte[] pStrConteudoArquivo, string pStrNomeArquivo)
        {
            try
            {
                FileStream _FileStream = new FileStream(pStrNomeArquivo, FileMode.Create, FileAccess.Write);
                _FileStream.Write(pStrConteudoArquivo, 0, pStrConteudoArquivo.Length);
                return _FileStream;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Função utilizada para Retornar o 'ContentType' de um arquivo, baseado na sua extensão ou no próprio arquivo em bytes.
        /// </summary>
        /// <param name="sExtencaoArquivo">Recebe a string da extensão do arquivo, se existir.</param>
        /// <param name="arquivo">Recebe o objeto do Arquivo em bytes, se existir.</param>
        /// <returns>Retorna a string do 'ContentType' do Arquivo.</returns>
        public static string RetornaContentTypeArquivo(string sExtencaoArquivo = null, object arquivo = null)
        {
            string sContentType = string.Empty;

            if (string.IsNullOrEmpty(sExtencaoArquivo))
            {
                if (arquivo == null)
                    throw new Exception("A função 'RetornaContentTypeArquivo' necessita que ao menos um dos parâmetros (string sExtencaoArquivo, object arquivo) sejam recebidos.");
                else
                    sExtencaoArquivo = RetornaTipo_Arquivo(arquivo).ToString();
            }

            switch (sExtencaoArquivo)
            {
                case ".jpg":
                case ".jpeg":
                case "1":
                    sContentType = "image/jpeg";
                    break;

                case ".png":
                case "2":
                    sContentType = "image/png";
                    break;

                case ".gif":
                case "3":
                    sContentType = "image/gif";
                    break;

                case ".pdf":
                case "4":
                    sContentType = "application/pdf";
                    break;

                case ".xls":
                case "5":
                    sContentType = "application/vnd.ms-excel";
                    break;

                case ".doc":
                    sContentType = "application/msword";
                    break;

                case ".docx":
                case "6":
                    sContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;

                case ".xlsx":
                case "7":
                    sContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;

                case ".pptx":
                case "8":
                    sContentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    break;

                case ".zip":
                case "9":
                    sContentType = "application/zip";
                    break;

                case ".xml":
                case "10":
                    sContentType = "text/xml";
                    break;

                case ".webp":
                case "11":
                    sContentType = "image/webp";
                    break;
            }

            if (string.IsNullOrEmpty(sContentType))
                throw new Exception("Extensão não suportada! As extensões suportadas são: jpg / jpeg / png / gif / pdf / xls / xlsx / doc / docx / pptx / zip / xml");

            return sContentType;
        }

        /// <summary>
        /// Função que retorna o tipo de arquivo baseado nos primeiros bytes do arquivo.
        /// </summary>
        /// <param name="objeto">Recebe o objeto do Arquivo.</param>
        /// <returns>
        /// Retorna um int que representa o tipo do Arquivo.<br /><br />
        /// Sendo os seguintes Tipos:<br />
        /// - 0: Tipo desconhecido <br />
        /// - 1: JPEG <br />
        /// - 2: PNG <br />
        /// - 3: GIF <br />
        /// - 4: PDF <br />
        /// - 5: DOC ou XLS <br />
        /// - 6: DOCX <br />
        /// - 7: XLSX <br />
        /// - 8: PPTX <br />
        /// - 9: ZIP <br />
        /// - 10: XML <br />
        /// - 11: WEBP <br />
        /// </returns>
        public static int RetornaTipo_Arquivo(object objeto)
        {
            if (objeto is byte[] arquivo)
            {
                if (arquivo == null || arquivo.Length < 4)
                    return 0;

                if (arquivo[0] == 0xFF && arquivo[1] == 0xD8 && arquivo[2] == 0xFF) // JPEG
                    return 1;

                if (arquivo[0] == 0x89 && arquivo[1] == 0x50 && arquivo[2] == 0x4E && arquivo[3] == 0x47) // PNG
                    return 2;

                if (arquivo[0] == 0x47 && arquivo[1] == 0x49 && arquivo[2] == 0x46 && arquivo[3] == 0x38) // GIF
                    return 3;

                if (arquivo[0] == 0x25 && arquivo[1] == 0x50 && arquivo[2] == 0x44 && arquivo[3] == 0x46) // PDF
                    return 4;

                if (arquivo[0] == 0xD0 && arquivo[1] == 0xCF && arquivo[2] == 0x11 && arquivo[3] == 0xE0) // DOC ou XLS
                    return 5;

                if (arquivo[0] == 0x50 && arquivo[1] == 0x4B && arquivo[2] == 0x03 && arquivo[3] == 0x04) // ZIP, DOX, XLSX ou PPTX
                {
                    try
                    {
                        using (var ms = new MemoryStream(arquivo))
                        using (var zip = new ZipArchive(ms, ZipArchiveMode.Read))
                        {
                            if (zip.Entries.Any(e => e.FullName.StartsWith("word/")))
                                return 6; // DOCX

                            if (zip.Entries.Any(e => e.FullName.StartsWith("xl/")))
                                return 7; // XLSX

                            if (zip.Entries.Any(e => e.FullName.StartsWith("ppt/")))
                                return 8; // PPTX

                            return 9; // ZIP genérico
                        }
                    }
                    catch
                    {
                        return 9; // ZIP genérico ou corrompido
                    }
                }

                if (arquivo[0] == 0x3C && arquivo[1] == 0x3F && arquivo[2] == 0x78 && arquivo[3] == 0x6D) // XML
                    return 10;

                if (arquivo.Length >= 12 && arquivo[0] == 0x52 && arquivo[1] == 0x49 && arquivo[2] == 0x46 && arquivo[3] == 0x46 && arquivo[8] == 0x57 && arquivo[9] == 0x45 && arquivo[10] == 0x42 && arquivo[11] == 0x50) // WEBP
                    return 11;
            }

            return 0;
        }

        /// <summary>
        /// Função utilizada para Converter imagens para PNG.
        /// <br /><br />
        /// Tipos de Arquivos suportados:<br />
        /// - JPG / JPEG <br />
        /// - WEBP <br />
        /// - BMP <br />
        /// - ICO (Ícones do Windows) <br />
        /// - GIF (apenas o primeiro quadro não animado) <br />
        /// </summary>
        /// <param name="arquivo">Recebe os bytes do Arquivo da imagem.</param>
        /// <returns>Retorna o Array de bytes da imagem em formato PNG.</returns>
        public static byte[] ConverterImagem_PNG(object arquivo)
        {
            try
            {
                byte[] bytes = null;

                if (arquivo is MemoryStream msArquivo) bytes = msArquivo.ToArray();
                else if (arquivo is Stream stArquivo)
                {
                    MemoryStream ms = new MemoryStream();
                    stArquivo.Position = 0;
                    stArquivo.CopyTo(ms);
                    bytes = ms.ToArray();
                }
                else bytes = arquivo as byte[];

                var imagem = SKImage.FromBitmap(SKBitmap.Decode(SKCodec.Create(new SKMemoryStream(bytes))));
                var png = imagem.Encode(SKEncodedImageFormat.Png, 100);
                return png.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao converter imagem para PNG: " + ex.Message);
            }
        }
    }

    public static class Extensoes
    {
        #region | Comparações

        public static bool IsNumeric(this string value) => decimal.TryParse(value, out _);

        public static bool Between<T>(this T valor, T minimo, T maximo) where T : IComparable<T> => valor.CompareTo(minimo) >= 0 && valor.CompareTo(maximo) <= 0;

        #endregion

        #region | Conversões

        public static decimal DoubleToDecimal(this double value) => (decimal)value;

        public static double DecimalToDouble(this decimal value) => (double)value;

        public static string StringToDecimalString(this string valor) => valor.Replace(".", "").Replace(",", ".");

        #endregion

        #region | Manipulação de Controles

        public static void SetAttribute(this Control control, string atributo, string valor = "")
        {
            var attrs = control.GetAttributes();
            if (attrs == null) return;

            atributo = atributo.Trim();
            valor = string.IsNullOrWhiteSpace(valor) ? atributo : valor.Trim();

            attrs[atributo] = valor;
        }
        public static void RemoveAttribute(this Control control, string atributo) => control.GetAttributes()?.Remove(atributo.Trim());
        public static bool HasAttribute(this Control control, string atributo)
        {
            var attrs = control.GetAttributes();
            return attrs != null && attrs[atributo.Trim()] != null;
        }
        public static string GetAttribute(this Control control, string atributo) => control.GetAttributes()?[atributo.Trim()];

        public static void AddClass(this Control control, string classe)
        {
            var attrs = control.GetAttributes();
            if (attrs == null) return;

            var classes = attrs.GetClasses();
            if (!classes.Contains(classe, StringComparer.OrdinalIgnoreCase))
                attrs["class"] = string.Join(" ", classes.Concat(new[] { classe }));
        }
        public static void RemoveClass(this Control control, string classe)
        {
            var attrs = control.GetAttributes();
            if (attrs == null) return;

            attrs["class"] = string.Join(" ", attrs.GetClasses().Where(x => !string.Equals(x, classe, StringComparison.OrdinalIgnoreCase)));
        }
        public static bool HasClass(this Control control, string classe)
        {
            var attrs = control.GetAttributes();
            if (attrs == null) return false;

            return attrs.GetClasses().Contains(classe, StringComparer.OrdinalIgnoreCase);
        }
        public static void ToggleClass(this Control control, string classe)
        {
            if (control.HasClass(classe))
                control.RemoveClass(classe);
            else
                control.AddClass(classe);
        }

        public static AttributeCollection GetAttributes(this Control control)
        {
            if (control is WebControl web)
                return web.Attributes;

            if (control is HtmlGenericControl html)
                return html.Attributes;

            return null;
        }
        public static IEnumerable<string> GetClasses(this AttributeCollection attrs)
        {
            return (attrs["class"] ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion

        #region | Outros

        public static string GetValue(this HttpRequest request, string key) => request[key]?.ToString() ?? string.Empty;
        public static string GetValue(this HttpSessionState session, string key) => session[key]?.ToString() ?? string.Empty;

        #endregion
    }
}