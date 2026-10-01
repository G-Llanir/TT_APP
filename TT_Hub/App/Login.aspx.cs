using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IDENTITY = TT.FrameWork.Identity;

namespace TT_Hub.App
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            

            if (Request["sChave"] != null)
            {
                string sChaveSessao = Request["sChave"].ToString();
                IDENTITY.Usuario.Login_Sessao(sChaveSessao);
            }
            else
            {
                IDENTITY.Usuario.Logoff();
            }


            if (!IsPostBack)
            {
                LimparCampos(true);
            }

        }
        protected void cmdLogin_Click(object sender, EventArgs e)
        {
            string sMensagem = "";
            string sURLChamada = "";


            try
            {
                sURLChamada = HttpContext.Current.Session["sURLChamada"].ToString(); 

                if (sURLChamada == "")
                {
                    sURLChamada = "app/dashboard.aspx";
                }
            }
            catch
            {
                sURLChamada = "app/dashboard.aspx";
            }
            
            if (sURLChamada.Substring(0,1) == "/")
            {
                sURLChamada = sURLChamada.Remove(0, 1);
            }


            if (!IDENTITY.Usuario.Login_Usuario(txtUsuario.Text, txtSenha.Text, out sMensagem, sURLChamada))
            {
                lblresultado.Text = sMensagem;
                lblresultado.Visible = true;
                LimparCampos(false);
            }
        }
        void LimparCampos(bool sFuncao)
        {
            
            txtUsuario.Text = "";
            txtSenha.Text = "";
            if (sFuncao)
            {
                lblresultado.Visible = false;
            }
            else
            {
                lblresultado.Visible = true;
            }
            txtUsuario.Focus();

        }

   
    }
}