using System;
using System.Web.UI;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.IA
{
    public partial class Chat : Page
    {
        // Controla a exibicao do botao "Usar base de conhecimento" (permissao 695).
        protected bool PodeUsarConhecimento;

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.IA.Consultar, true);
            PodeUsarConhecimento = FUNCOES.ValidaPermissao(Permissao.IA.UsarConhecimento, false);

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = "Tina";
            }
        }
    }
}
