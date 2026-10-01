using System;
using System.Web.UI;
using TT.FrameWork;
using static TT.FrameWork.Identity;
using IDENTITY = TT_Flow.FrameWork.Identity;

namespace TT_Flow.App
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                FrameWork.Identity.CarregarVariaveis_Sistema();


                //string sURL_Login = "https://login.tecandtec.com.br";
                //string sURL_Login = "http://localhost:7006/";

                if (Request["sChave"] != null || bDev)
                {
                    if (!Usuario.Login_Sessao((Request["sChave"] ?? "").ToString(), (Session?["sURLChamada"] ?? "").ToString()))
                        Response.Redirect(IDENTITY.sSistema_EnderecoLogin);
                }
                else if (Request["LogOut"] != null)
                {
                    Usuario.Logoff();
                    Response.Redirect(IDENTITY.sSistema_EnderecoLogin);
                }
                else if (Session?["sLogin"] != null && Session?["sLogin"].ToString() != "" && Session?["sChaveSessao"] != null && Session?["sChaveSessao"].ToString() != "")
                {
                    if (!Usuario.Login_Sessao(Session?["sChaveSessao"].ToString(), (Session?["sURLChamada"] ?? "").ToString()))
                        Response.Redirect(IDENTITY.sSistema_EnderecoLogin);
                }
                else
                {
                    Usuario.Logoff();
                    Response.Redirect(IDENTITY.sSistema_EnderecoLogin);
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }
}