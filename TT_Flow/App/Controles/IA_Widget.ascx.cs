using System;
using System.Web;
using System.Web.UI;
using TT_Flow.FrameWork.IA;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Controles
{
    // Botao "Perguntar à IA" reutilizavel: a tela host define Tela/IdRegistro/Titulo/Resumo
    // e o controle abre o chat com o contexto serializado na query string.
    // O contexto e apenas orientacao; as ferramentas continuam validando permissao no servidor.
    public partial class IA_Widget : UserControl
    {
        public string Tela { get; set; }
        public int IdRegistro { get; set; }
        public string Titulo { get; set; }
        public string Resumo { get; set; }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                lnkPerguntarIA.Visible = false;

                if (string.IsNullOrWhiteSpace(Tela) || IdRegistro <= 0)
                {
                    return;
                }

                if (!FUNCOES.ValidaPermissao(Permissao.IA.Consultar))
                {
                    return;
                }

                cls_IA_Config config = cls_IA_Config.Carregar();
                if (!config.Habilitado)
                {
                    return;
                }

                string contextoJson = cls_IA_Contexto.Serializar(Tela, IdRegistro, Titulo, Resumo);
                lnkPerguntarIA.NavigateUrl = ResolveUrl("~/App/Paginas/IA/Chat.aspx") + "?contexto=" + HttpUtility.UrlEncode(contextoJson);
                lnkPerguntarIA.Visible = true;
            }
            catch
            {
                // O widget e opcional e nunca pode derrubar a tela host
                lnkPerguntarIA.Visible = false;
            }
        }
    }
}
