using System.Web.Script.Services;
using System.Web.Services;
using static TT.FrameWork.Funcoes;
using static TT_Colaborador.SiteMaster;

namespace TT_Colaborador.API
{
    [ScriptService]
    public class WebMethods : WebService
    {
        [WebMethod(EnableSession = true)]
        public void Timer_Atualizar()
        {
            ValidarSessao();
            AtualizarParametrosSistema();
        }
    }
}
