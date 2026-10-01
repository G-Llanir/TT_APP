using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using TT.FrameWork;

namespace TT_Flow.App.Paginas.WMS.Manutencao
{
    public partial class NCM_Importar : System.Web.UI.Page
    {
        string sTituloPagina = "Importador de NCM";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["id"] != null)
                {
                    PesquisarArquivo("1", Request["id"].ToString(), "Saldo");
                }
                else
                {
                    PesquisarArquivo("1", "", "Saldo");
                }
            }
        }

        protected void PesquisarArquivo(string idObjeto, string idArquivo, string sTipoObjeto)
        {
            lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
        }

        protected void cmdAtualizar_Click(object sender, EventArgs e)
        {

        }

        protected void gv_Arquivo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.CommandArgument.ToString()))
            {
                if (e.CommandName == "Download")
                {
                    Dictionary<string, string> vParametrosItem = new Dictionary<string, string>()
                    {
                        {"@sFuncao",                    "CONSULTAR_DETALHE" },
                        {"@idArquivo",                  gv_Arquivo.Rows[int.Parse(e.CommandArgument.ToString())].Cells[0].Text }
                    };

                    DataTable dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

                    foreach (DataRow item in dtArquivo.Rows)
                    {
                        string sNomeArquivo = item["sNomeArquivo"].ToString();
                        Arquivo objArquivo = new Arquivo();
                        FileStream lObjFile = objArquivo.TransformarArrayBytesEmArquivo((byte[])item["vbArquivo"], Server.MapPath("~/Download/" + sNomeArquivo));
                        lObjFile.Close();

                        Funcoes.DownloadArquivo(Page, sNomeArquivo);

                        //Response.ContentType = "application/octet-stream";
                        //Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}", sNomeArquivo));
                        //Response.TransmitFile(Server.MapPath("~/Download/" + sNomeArquivo));
                        //Response.End();
                    }
                }
            }
        }
    }
}
