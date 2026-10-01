using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TT_Flow.App.Controles
{
    public partial class MensagemPagina : UserControl
    {
        protected void Page_Load(object sender, EventArgs e) { }

        public void MostraMensagem_Sucesso(string Mensagem) => MostraMensagem(Mensagem, "SUCESSO", true);
        public void MostraMensagem_Sucesso(string Mensagem, bool bFocus) => MostraMensagem(Mensagem, "SUCESSO", bFocus);
        public void MostraMensagem_Aviso(string Mensagem) => MostraMensagem(Mensagem, "AVISO", true);
        public void MostraMensagem_Aviso(string Mensagem, bool bFocus) => MostraMensagem(Mensagem, "AVISO", bFocus);
        public void MostraMensagem_Erro(string Mensagem) => MostraMensagem(Mensagem, "ERRO", true);
        public void MostraMensagem_Erro(string Mensagem, bool bFocus) => MostraMensagem(Mensagem, "ERRO", bFocus);

        public void MostraMensagem(string Mensagem, string TipoMensagem, bool bFocus)
        {
            DIV_MENSAGEM.Controls.Clear();
            DIV_MENSAGEM.Visible = false;

            if (string.IsNullOrWhiteSpace(Mensagem))
            {
                return;
            }

            Literal ltrMensagem = new Literal();
            string sClasse = "";
                if (TipoMensagem == "ERRO")
                    sClasse = "danger";
                else if (TipoMensagem == "SUCESSO")
                    sClasse = "success";
                else if (TipoMensagem == "AVISO")
                    sClasse = "warning";
                else
                    sClasse = "info";

            ltrMensagem.Text = "<div class='alert alert-" + sClasse + " '>";
            ltrMensagem.Text += "   <button type='button' class='close' data-dismiss='alert'>&times;</button>";
            ltrMensagem.Text += Mensagem;
            ltrMensagem.Text += "</div>";

            DIV_MENSAGEM.Controls.Add(ltrMensagem);
            DIV_MENSAGEM.Visible = true;

            if (bFocus)
                DIV_MENSAGEM.Focus();
        }

        public bool MensagemAparecendo(string TipoMensagem)
        {
            string sClasse = string.Empty;

            if (TipoMensagem == "ERRO")
                sClasse = "danger";
            else if (TipoMensagem == "SUCESSO")
                sClasse = "success";
            else if (TipoMensagem == "AVISO")
                sClasse = "warning";
            else
                sClasse = "info";

            if (DIV_MENSAGEM.Controls.Count > 0)
            {
                foreach (var controle in DIV_MENSAGEM.Controls)
                {
                    if (controle is Literal ltr && ltr.Text.Contains($"alert-{sClasse}"))
                        return true;
                }
            }

            return false;
        }
    }
}