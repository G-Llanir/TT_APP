using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;							   
using System.IO;
using iTextSharp.text.pdf;

namespace TT.FrameWork
{
   
    /// <summary>
    /// Geração centralizada de códigos de barras (Code128) para relatórios.
    /// Reutiliza o mesmo padrão de geração da NFe (iTextSharp Barcode128 ->
    /// imagem PNG), retornando um <c>byte[]</c> compatível com os componentes de
    /// relatório (RDLC) já utilizados pelo sistema, via Image Source=Database +
    /// System.Convert.FromBase64String.
    /// </summary>
    public static class GeradorCodigoBarras
    {
        private const string PREFIXO = "REL";
        private const int QTD_DIGITOS_ID = 7;
		private const float TAMANHO_FONTE_TEXTO = 6f;
        private const float ESCALA_RENDER = 2f;

        /// <summary>
        /// Tipos de relatório suportados na geração do identificador de código de barras.
        /// Centraliza os sufixos para evitar strings espalhadas pelos relatórios.
        /// </summary>
        public enum TipoRelatorioCodigoBarras
        {
            /// <summary>Ficha de EPI -> sufixo "EPI".</summary>
            EPI,
            /// <summary>Ficha de roupas -> sufixo "RO".</summary>
            RO
        }


        /// <summary>
        /// Monta o identificador no padrão "REL" + ID com zeros à esquerda + sufixo do tipo.
        /// Ex.: tipo EPI e ID 1 -> "REL0000001EPI"; tipo RO e ID 123 -> "REL0000123RO".
        /// </summary>
        public static string MontarIdentificador(TipoRelatorioCodigoBarras tipo, int idEntrega)
        {
            if (idEntrega <= 0)
                throw new ArgumentException("O ID da entrega deve ser maior que zero.", nameof(idEntrega));

            return string.Concat(PREFIXO, idEntrega.ToString().PadLeft(QTD_DIGITOS_ID, '0'), ObterSufixo(tipo));
        }

        /// <summary>
        /// Gera a imagem (PNG) de um código de barras Code128 para o relatório informado.
        /// </summary>
        /// <param name="tipoRelatorio">Tipo do relatório ("EPI" ou "RO").</param>
        /// <param name="idEntrega">ID da entrega (deve ser maior que zero).</param>
        /// <returns>Bytes da imagem PNG do código de barras.</returns>
        public static byte[] GerarCodigoBarras(string tipoRelatorio, int idEntrega)
        {
            if (string.IsNullOrWhiteSpace(tipoRelatorio))
                throw new ArgumentException("O tipo do relatório é obrigatório.", nameof(tipoRelatorio));

            if (!Enum.TryParse(tipoRelatorio.Trim(), true, out TipoRelatorioCodigoBarras tipo) || !Enum.IsDefined(typeof(TipoRelatorioCodigoBarras), tipo))
                throw new ArgumentException($"Tipo de relatório inválido: '{tipoRelatorio}'. Tipos suportados: EPI, RO.", nameof(tipoRelatorio));

            return GerarCodigoBarras(tipo, idEntrega);
        }

        /// <summary>
        /// Sobrecarga tipada para chamadas internas que já conhecem o tipo do relatório.
        /// </summary>
        public static byte[] GerarCodigoBarras(TipoRelatorioCodigoBarras tipo, int idEntrega)
        {
            string identificador = MontarIdentificador(tipo, idEntrega);

            BaseFont fonte = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            Barcode128 codigoBarras = new Barcode128
            {
                Code = identificador,
                CodeType = Barcode.CODE128,
                StartStopText = false,
                GenerateChecksum = true,
                ChecksumText = false,
                BarHeight = 40f,
                X = 0.8f,
                Font = fonte,
                Size = TAMANHO_FONTE_TEXTO,
                Baseline = TAMANHO_FONTE_TEXTO
            };

            // CreateDrawingImage do iTextSharp renderiza apenas as barras; o texto legível
            // é composto abaixo usando as mesmas dimensões e fonte configuradas no Barcode128.
            using (System.Drawing.Image imagemBarras = codigoBarras.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White))
            using (MemoryStream ms = new MemoryStream())
            using (System.Drawing.Font fonteTexto = new System.Drawing.Font("Helvetica", codigoBarras.Size, FontStyle.Regular, GraphicsUnit.Point))
            {
                iTextSharp.text.Rectangle tamanho = codigoBarras.BarcodeSize;
                string texto = string.IsNullOrEmpty(codigoBarras.AltText) ? codigoBarras.Code : codigoBarras.AltText;
                float areaTexto = codigoBarras.Baseline - fonte.GetFontDescriptor(BaseFont.DESCENT, codigoBarras.Size);

                float larguraLogica = Math.Max(tamanho.Width, imagemBarras.Width);
                float alturaLogica = tamanho.Height;
                int largura = (int)Math.Ceiling(larguraLogica * ESCALA_RENDER);
                int altura = (int)Math.Ceiling(alturaLogica * ESCALA_RENDER);

                using (Bitmap composta = new Bitmap(largura, altura))
                using (Graphics graphics = Graphics.FromImage(composta))
                {
                    graphics.ScaleTransform(ESCALA_RENDER, ESCALA_RENDER);
                    graphics.Clear(System.Drawing.Color.White);
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    float barraX = (larguraLogica - imagemBarras.Width) / 2f;
                    graphics.DrawImage(imagemBarras, barraX, 0, imagemBarras.Width, imagemBarras.Height);

                    SizeF medidaTexto = graphics.MeasureString(texto, fonteTexto);
                    float textoX = (larguraLogica - medidaTexto.Width) / 2f;
                    float textoY = codigoBarras.BarHeight + Math.Max(0f, (areaTexto - medidaTexto.Height) / 2f);
                    graphics.DrawString(texto, fonteTexto, Brushes.Black, textoX, textoY);

                    composta.Save(ms, ImageFormat.Png);
                }

                return ms.ToArray();
            }
        }

        private static string ObterSufixo(TipoRelatorioCodigoBarras tipo)
        {
            switch (tipo)
            {
                case TipoRelatorioCodigoBarras.EPI:
                    return "EPI";
                case TipoRelatorioCodigoBarras.RO:
                    return "RO";
                default:
                    throw new ArgumentException($"Tipo de relatório não mapeado: {tipo}.", nameof(tipo));
            }
        }
    }
}
