using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using IDENTITY =  TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;

namespace TT_Login
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sChaveSessao = "";
            if (Request["sChave"] != null)
            {
                sChaveSessao = Request["sChave"].ToString();
            }
 
            
            IDENTITY.Usuario.Logoff(sChaveSessao);


            if (!IsPostBack)
            {
                LimparCampos(true);
            }

        }
    
        protected void cmdLogin_Click(object sender, EventArgs e)
        {
            string sURLChamada = "";
            string idSistema = "";
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

            if (sURLChamada.Substring(0, 1) == "/")
            {
                sURLChamada = sURLChamada.Remove(0, 1);
            }




            string sMensagem = "";
            if (!IDENTITY.Usuario.Login_Usuario(txtUsuario.Text, txtSenha.Text, out sMensagem, "app/Sistemas.aspx"))
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