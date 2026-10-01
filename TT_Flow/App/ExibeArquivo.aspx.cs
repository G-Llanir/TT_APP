using System;
using System.Web.UI;

namespace TT_Flow.App
{
    public partial class ExibeArquivo : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ExibeArquivo"] is byte[] pdf)
            {
                string tipo = Session["ExibeArquivo_Tipo"] as string ?? "application/pdf";
                string nome = Session["ExibeArquivo_Nome"] as string ?? "arquivo.pdf";

                Response.Clear();
                Response.ContentType = tipo;
                Response.AddHeader("Content-Length", pdf.Length.ToString());
                Response.AddHeader("Content-Disposition", $"inline; filename={nome}");

                Response.BinaryWrite(pdf);

                Session["ExibeArquivo"] = null;
                Session["ExibeArquivo_Tipo"] = null;
                Session["ExibeArquivo_Nome"] = null;

                Response.End();
            }
            else Response.Write("Nenhum Arquivo disponível.");
        }
    }
}