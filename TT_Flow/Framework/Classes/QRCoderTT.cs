using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using ZXing;
using ZXing.QrCode;

namespace TT_Flow.FrameWork
{
    public class QRCoderTT
    {
        public byte[] GerarQrCodeParaAtividade(int idAtividade, string uuidAtividade)
        {
            string qrCodeIdentifier = string.Empty;

            if (!string.IsNullOrEmpty(uuidAtividade))
            {
                qrCodeIdentifier = uuidAtividade;
            }
            else
            {
                qrCodeIdentifier = idAtividade.ToString();
            }

            string qrCodeContent = $"https://t-flow.tecandtec.com.br/App/Paginas/LeitorAtividade.aspx?id={qrCodeIdentifier}";

            try
            {
                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new QrCodeEncodingOptions
                    {
                        Width = 200,
                        Height = 200,
                        Margin = 0
                    }
                };

                using (Bitmap qrCodeBitmap = writer.Write(qrCodeContent))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        qrCodeBitmap.Save(memoryStream, ImageFormat.Png);
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public byte[] GerarQrCodeUsuario(int idUsuario, string uuidUsuario)
        {
            string qrCodeIdentifier = string.Empty;

            if (!string.IsNullOrEmpty(uuidUsuario))
            {
                qrCodeIdentifier = uuidUsuario;
            }
            else
            {
                qrCodeIdentifier = idUsuario.ToString();
            }

            string qrCodeContent = qrCodeIdentifier;

            try
            {
                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new QrCodeEncodingOptions
                    {
                        Width = 200,
                        Height = 200,
                        Margin = 0
                    }
                };

                using (Bitmap qrCodeBitmap = writer.Write(qrCodeContent))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        qrCodeBitmap.Save(memoryStream, ImageFormat.Png);
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            byte[] qrCodeBytes = context.Session["QRCodeImage"] as byte[];

            if (qrCodeBytes != null)
            {
                if (context.Request.QueryString["download"] == "true")
                {
                    context.Response.ContentType = "application/octet-stream";
                    context.Response.AddHeader("Content-Disposition", "attachment; filename=QRCode.png");
                }
                else
                {
                    context.Response.ContentType = "image/png";
                }

                context.Response.BinaryWrite(qrCodeBytes);
            }
            else
            {
                context.Response.StatusCode = 404;
            }
        }

        public bool IsReusable => false;
    }
}


