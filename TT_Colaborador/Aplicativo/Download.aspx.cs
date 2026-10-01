using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using System.IO;
using System.Text;
using System.Threading;
using System.Security.Policy;

namespace TT_Colaborador
{
    public partial class Download : System.Web.UI.Page
    {
        public static string MaximoidRegistro = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            string idArquivo = "";
            string idItens = "";
            string sPage = "";
;
            if (Request["idArquivo"] != null)
            {
                idArquivo = Request["idArquivo"].ToString();
            }
            if (Request["idItens"] != null)
            {
                idItens = Request["idItens"].ToString();
            }
            if (Request["sPage"] != null)
            {
                sPage = Request["sPage"].ToString();
            }

            string sNomeArquivo = "";
            byte[] bObjArquivo = null;
            if (idItens != "")
            {
                Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
                DataTable dtArquivo;
                vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                  "CONSULTAR_ARQUIVOS" },
                        {"@idItens",                  idItens.ToString()}

                    };

                dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_Relatorio_Despesas", vParametrosItem);

                foreach (DataRow item in dtArquivo.Rows)
                {
                    try
                    {
                        sNomeArquivo = item["sNomeArquivo"].ToString();
                        bObjArquivo = (byte[])item["vbArquivo"];
                    }
                    catch
                    {

                    }

                    TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                    FileStream lObjFile = objArquivo.TransformarArrayBytesEmArquivo(bObjArquivo, Server.MapPath("~/Download/" + sNomeArquivo));
                    lObjFile.Close();
                    lObjFile.Dispose();

                    FUNCOES.DirecionaPagina_NovaAba(Page, "/Download/" + sNomeArquivo);
                }
                FUNCOES.Scripts.DirecionarPagina(Page, "/Download/" + sPage);
            }
            else
            {

            }
        }
    }
}