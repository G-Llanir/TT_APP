using System;
using TT.FrameWork;

namespace TT_Colaborador.Aplicativo
{
    public partial class PermissaoNegada : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            divVoltar.Visible = true;
            divUsuarioVinculado_Voltar.Visible = false;

            if (!string.IsNullOrEmpty(Request["UsuarioVinculado"]))
            {
                lblTituloPagina.Text = "Acesso negado! ";

                if (Request["Recurso"] != null)
                    lblTituloPagina.Text += "<br/>" + Request["Recurso"];
            }
            else
            {
                lblTituloPagina.Text = "Seu Usuário não está devidamente vinculado à um Colaborador, por favor solicite ao RRHH que faça o vínculo! ";
                divVoltar.Visible = false;
                divUsuarioVinculado_Voltar.Visible = true;
            }
        }

        protected void cmdVoltar_Click(object sender, EventArgs e) => Funcoes.DirecionaPagina("Aplicativo/login.aspx");
    }
}