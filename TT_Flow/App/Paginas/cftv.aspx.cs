using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Text;
using static System.Net.WebRequestMethods;

namespace TT_Flow.App.Paginas
{
    public partial class cftv : System.Web.UI.Page
    {
        private DateTime ultimaExecucao = DateTime.MinValue;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack && FileUpload01.PostedFile != null)

            {

                if (FileUpload01.PostedFile.FileName.Length > 0)
                {
                    FileUpload01.SaveAs(Server.MapPath("~/Download/") + FileUpload01.PostedFile.FileName);
                }

            }



        }


        private int CalcularIntervaloAteProximaExecucao()
        {
            DateTime agora = DateTime.Now;
            DateTime proximaExecucao = agora.Date.AddDays(1).AddHours(7);
            return (int)(proximaExecucao - agora).TotalMilliseconds;
        }

        protected void cmdCapturar_Click(object sender, EventArgs e)
        {
            WebClient cftv = new WebClient();
            cftv.UseDefaultCredentials = true;
            cftv.Credentials = new NetworkCredential("admin", "tt2995@@a");

            //string s = cftv.DownloadString("http://192.168.50.161/cgi-bin/snapshot.cgi");


            //byte[] valorImgBd = Encoding.ASCII.GetBytes(s);

            cftv.DownloadFile("http://192.168.50.161/cgi-bin/snapshot.cgi", Server.MapPath("~/Download/a.jpg"));
            //string imgUrl = Convert.ToBase64String(valorImgBd);

            Image1.ImageUrl = "http://localhost:6997/Download/a.jpg";
            Image1.Style["display"] = "block";

        }
    }
}