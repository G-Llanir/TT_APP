using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;

namespace TT_Flow.App.Controles
{
    public partial class ContadorMensagem : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void tmrAtualiza_Tick(object sender, EventArgs e)
        {
            Funcoes.ValidarSessao();
            SqlDataReader dr;
            dr = TT.FrameWork.BD.ExecutarDataReader("sp_Flow_ConsultaDadosSistema '" + TT_Flow.FrameWork.Identity.Variaveis.idUsuario() + "'");


            if (dr != null)
            {
                while (dr.Read())
                {
                    TT_Flow.FrameWork.Identity.sNomeSistema = dr["sNomeSistema"].ToString();
                    TT_Flow.FrameWork.Identity.sVersao = dr["sVersao"].ToString();
                    TT_Flow.FrameWork.Identity.nQtdMensagens = dr["nQtdMensagens"].ToString();
                    TT_Flow.FrameWork.Identity.nQtdMensagens_NaoLidas = dr["nQtdMensagens_NaoLidas"].ToString();
                    TT_Flow.FrameWork.Identity.nQtdMensagens_Lidas = dr["nQtdMensagens_Lidas"].ToString();
                    HttpContext.Current.Session["sPermissao"] = dr["sPermissao"].ToString();
                }

                dr.Close();
                    

            }

            Label2.Text = TT_Flow.FrameWork.Identity.nQtdMensagens;

        }
    }
}