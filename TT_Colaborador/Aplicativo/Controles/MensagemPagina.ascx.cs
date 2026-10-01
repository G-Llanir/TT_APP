using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TT_Colaborador.Aplicativo.Controles
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
            Literal ltrMensagem = new Literal();
            string sClasse = "";

            if (Mensagem != "")
            {
                if (TipoMensagem == "ERRO") sClasse = "danger";
                else if (TipoMensagem == "SUCESSO") sClasse = "success";
                else if (TipoMensagem == "AVISO") sClasse = "warning";
                else sClasse = "info";

                ltrMensagem.Text = $"<div class='alert alert-{sClasse} alert-dismissible fade show' role='alert'><button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button>{Mensagem}</div>";

                DIV_MENSAGEM.Controls.Add(ltrMensagem);
                DIV_MENSAGEM.Visible = true;

                if (bFocus) DIV_MENSAGEM.Focus();
            }
        }
    }
}