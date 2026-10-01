using System;
using System.Web.UI;
using static TT.FrameWork.Identity;

namespace TT_Colaborador.Aplicativo
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
               if (Request["sChave"] != null)
                {
                    if (!Usuario.Login_Sessao(Request["sChave"].ToString(), "Aplicativo/Paginas/Calendario"))
                        Response.Redirect("https://login.tecandtec.com.br");
                }
                else
                {
                    Usuario.Logoff();
                    Response.Redirect("https://login.tecandtec.com.br");
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }
}