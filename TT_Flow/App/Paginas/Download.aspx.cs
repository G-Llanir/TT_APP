using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using TT.FrameWork;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;

namespace TT_Flow.App.Paginas
{
    public partial class Download : Page
    {
        public static string MaximoidRegistro = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            string page = "";
            string filePath = "";
            string Video = "";
            string sNomeArquivo = "";
            string idItens = "";
            string sPage = "";
            string idArquivo = "";
            string caminhoCompleto = "";

            if (Request["page"] != null) page = Request["page"].ToString().Replace("|", "&");

            if (Request["filePath"] != null) filePath = Request["filePath"].ToString();

            if (Request["Video"] != null) { Video = Request["Video"].ToString(); sNomeArquivo = Request["Nome"].ToString(); }

            if (Request["idItens"] != null) idItens = Request["idItens"].ToString();

            if (Request["sPage"] != null) sPage = Request["sPage"].ToString();

            if (Request["idArquivo"] != null) idArquivo = Request["idArquivo"].ToString();

            if (idItens != "") DownloadDespesas(idItens, sPage);

            if (filePath != "") caminhoCompleto = HttpContext.Current.Server.MapPath("~/Download/" + filePath);

            if (Video != "") caminhoCompleto = HttpContext.Current.Server.MapPath(Video);

            if (idItens == "") ScriptManager.RegisterStartupScript(Page, Page.GetType(), "closeTab_Download", $"window.close();", true);

            if (File.Exists(caminhoCompleto))
            {
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                if (filePath != "") HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + filePath);
                if (Video != "") HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + sNomeArquivo);
                HttpContext.Current.Response.TransmitFile(caminhoCompleto);
                HttpContext.Current.Response.End();
            }
        }

        void DownloadDespesas(string idItens, string sPage)
        {
            string sNomeArquivo = "";
            byte[] bObjArquivo = null;

            if (idItens != "")
            {
                Dictionary<string, string> vParametrosItem = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_ARQUIVOS" },
                    { "@idItens", idItens.ToString() }
                };
                DataTable dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_Relatorio_Despesas", vParametrosItem);

                foreach (DataRow item in dtArquivo.Rows)
                {
                    try
                    {
                        sNomeArquivo = item["sNomeArquivo"].ToString();
                        bObjArquivo = (byte[])item["vbArquivo"];
                    }
                    catch { }

                    FileStream lObjFile = new Arquivo().TransformarArrayBytesEmArquivo(bObjArquivo, Server.MapPath("~/Download/" + sNomeArquivo));
                    lObjFile.Close();
                    lObjFile.Dispose();

                    FUNCOES.DirecionaPagina_NovaAba(Page, "/Download/" + sNomeArquivo);
                }

                FUNCOES.Scripts.DirecionarPagina(Page, "/Download/" + sPage);
            }
        }
    }
}