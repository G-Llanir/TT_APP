using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TT_Login.App.Controles
{
    public partial class MensagemPagina : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void MostraMensagem_Sucesso(String Mensagem)
        {
            MostraMensagem(Mensagem, "SUCESSO", true);
        }
        public void MostraMensagem_Sucesso(String Mensagem, bool bFocus)
        {
            MostraMensagem(Mensagem, "SUCESSO", bFocus);
        }

        public void MostraMensagem_Erro(String Mensagem)
        {
            MostraMensagem(Mensagem, "ERRO", true);
        }
        public void MostraMensagem_Erro(String Mensagem, bool bFocus)
        {
            MostraMensagem(Mensagem, "ERRO", bFocus);
        }

        public void MostraMensagem(string Mensagem, string TipoMensagem, bool bFocus)
        {

            Literal ltrMensagem = new Literal();
            string sClasse = "";

            if (Mensagem != "")
            {
                if (TipoMensagem == "ERRO")
                {
                    sClasse = "danger";
                }
                else if (TipoMensagem == "SUCESSO")
                {
                    sClasse = "success";
                }
                else
                {
                    sClasse = "info";
                }


                ltrMensagem.Text = "<div class='alert alert-" + sClasse + " '>";
                ltrMensagem.Text += "   <button type='button' class='close' data-dismiss='alert'>&times;</button>";
                ltrMensagem.Text += Mensagem;
                ltrMensagem.Text += "</div>";

                DIV_MENSAGEM.Controls.Add(ltrMensagem);
                DIV_MENSAGEM.Visible = true;

                if (bFocus)
                {
                    DIV_MENSAGEM.Focus();
                }
            }


        }
    }
}