using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;

namespace TT_Login
{
    public partial class Sistemas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidarSessao();

            string sErro = "";
            try
            {
                DataSet dsSistemas;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();

                vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
                dsSistemas = BD.ExecutarDataSet("sp_Login_ValidaSistemas", vParametros);

                if (BD.ValidarDataSet(dsSistemas, out sErro))
                {
                    StringBuilder strBotoes = new StringBuilder();
                    for (int i = 0; i < dsSistemas.Tables[0].Rows.Count; i++)
                    {
                        strBotoes.AppendLine(string.Format("<a href=\"{0}\">", BD.Retorno.DATASET(dsSistemas, i, "sURL")));
                        strBotoes.AppendLine(string.Format("<div class=\"col-lg-4\"><div class=\"panel panel-{0} caixaBotao\"><div class=\"panel-heading\"><div class=\"row\"><div class=\"col-xs-6\">", BD.Retorno.DATASET(dsSistemas, i, "sCor")));
                        strBotoes.AppendLine(string.Format("<i class=\"{0}\"></i></div><div class=\"col-xs-6 text-right\"><p class=\"announcement-menu\">{1}</p><p class=\"announcement-text\">{2}</p>", BD.Retorno.DATASET(dsSistemas, i, "sLogotipo"), BD.Retorno.DATASET(dsSistemas,i, "sNomeSistema"), ""));
                        strBotoes.AppendLine(string.Format("</div></div></div><div class=\"panel-footer announcement-bottom\">", ""));
                        strBotoes.AppendLine(string.Format("<div class=\"row\"><div class=\"col-xs-10 text-left\">{0}</div><div class=\"col-xs-2 text-right\"><i class=\"fa fa-arrow-circle-right\"></i></div></div></div></div></div></a>", BD.Retorno.DATASET(dsSistemas, i, "sDscSistema")));
                    }

                    ltrBotoes.Text = strBotoes.ToString();
                    }
                else
                {
                    throw new Exception("Nenhum sistema liberado para seu usuário!");
                }

            }
            catch (Exception ex)
            {
                ltrBotoes.Text = string.Format("<h2><b>{0}</b></h2>", ex.Message);

            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/app/login.aspx");
        }
    }
}