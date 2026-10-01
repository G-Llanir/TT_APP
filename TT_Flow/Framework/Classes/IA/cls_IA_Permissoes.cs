using System.Web;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Flow.FrameWork.IA
{
    public static class cls_IA_Permissoes
    {
        public static bool PodeUsarChat(out string mensagem)
        {
            if (!ValidaPermissaoEstrita(Permissao.IA.Consultar))
            {
                mensagem = "Você não possui permissão para usar o assistente de IA.";
                return false;
            }

            mensagem = string.Empty;
            return true;
        }

        public static bool ValidaPermissaoEstrita(int idRecurso)
        {
            if (idRecurso <= 0 || !SessaoLogada())
            {
                return false;
            }

            return FUNCOES.ValidaPermissao(idRecurso, false);
        }

        public static bool SessaoLogada()
        {
            try
            {
                HttpContext ctx = HttpContext.Current;
                if (ctx == null || ctx.Session == null || ctx.Session["sLogin"] == null)
                {
                    return false;
                }

                return !string.IsNullOrWhiteSpace(ctx.Session["sLogin"].ToString());
            }
            catch
            {
                return false;
            }
        }
    }
}
