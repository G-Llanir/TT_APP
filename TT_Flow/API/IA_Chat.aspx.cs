using System;
using System.Web.Services;
using System.Web.UI;
using TT_Flow.FrameWork.IA;

namespace TT_Flow.App
{
    public partial class IA_Chat : Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse EnviarMensagem(string idConversaIA, string sMensagem, string sContextoJson, string bUsarConhecimento)
        {
            cls_IA_Chat chat = new cls_IA_Chat();
            chat.UsarConhecimento = string.Equals(bUsarConhecimento, "S", StringComparison.OrdinalIgnoreCase);
            return chat.EnviarMensagem(idConversaIA, sMensagem, sContextoJson);
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse EnviarMensagemComArquivos(string idConversaIA, string sMensagem, string sArquivosIAJson, string sContextoJson, string bUsarConhecimento)
        {
            cls_IA_Chat chat = new cls_IA_Chat();
            chat.UsarConhecimento = string.Equals(bUsarConhecimento, "S", StringComparison.OrdinalIgnoreCase);
            return chat.EnviarMensagemComArquivos(idConversaIA, sMensagem, sArquivosIAJson, sContextoJson);
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse ListarConversas()
        {
            return new cls_IA_Chat().ListarConversas();
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse ListarConversasPaginado(string nPagina, string sTermo)
        {
            return new cls_IA_Chat().ListarConversasPaginado(nPagina, sTermo);
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse FavoritarConversa(string idConversaIA, string sFavorita)
        {
            return new cls_IA_Chat().FavoritarConversa(idConversaIA, sFavorita);
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse DeletarConversa(string idConversaIA)
        {
            return new cls_IA_Chat().DeletarConversa(idConversaIA);
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse ConfirmarAcao(string idAprovacaoIA)
        {
            return new cls_IA_Chat().ConfirmarAcao(idAprovacaoIA);
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse CancelarAcao(string idAprovacaoIA)
        {
            return new cls_IA_Chat().CancelarAcao(idAprovacaoIA);
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse ConfirmarWorkflow(string idExecucaoIA, string idConversaIA)
        {
            return new cls_IA_Chat().ConfirmarWorkflow(idExecucaoIA, idConversaIA);
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse InformarEntradasWorkflow(string idExecucaoIA, string idConversaIA, string entradasJson)
        {
            return new cls_IA_Chat().InformarEntradasWorkflow(idExecucaoIA, idConversaIA, entradasJson);
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse SelecionarOpcaoWorkflow(string idExecucaoIA, string idConversaIA, string valor)
        {
            return new cls_IA_Chat().SelecionarOpcaoWorkflow(idExecucaoIA, idConversaIA, valor);
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse CancelarWorkflow(string idExecucaoIA, string idConversaIA)
        {
            return new cls_IA_Chat().CancelarWorkflow(idExecucaoIA, idConversaIA);
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse RejeitarWorkflow(string idExecucaoIA, string idConversaIA)
        {
            return new cls_IA_Chat().RejeitarWorkflow(idExecucaoIA, idConversaIA);
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse ListarTemplates()
        {
            return new cls_IA_Chat().ListarTemplates();
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse SalvarTemplate(string sTitulo, string sConteudo, string sContextoTela)
        {
            return new cls_IA_Chat().SalvarTemplate(sTitulo, sConteudo, sContextoTela);
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse DeletarTemplate(string idTemplatePromptIA)
        {
            return new cls_IA_Chat().DeletarTemplate(idTemplatePromptIA);
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse CarregarConversa(string idConversaIA)
        {
            return new cls_IA_Chat().CarregarConversa(idConversaIA);
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse ConsultarArquivo(string idArquivoIA, string idConversaIA)
        {
            return new cls_IA_Chat().ConsultarArquivo(idArquivoIA, idConversaIA);
        }

        [WebMethod(EnableSession = true)]
        public static IAChatResponse AvaliarResposta(string idMensagemIA, string sAvaliacao, string sComentario)
        {
            return new cls_IA_Chat().AvaliarResposta(idMensagemIA, sAvaliacao, sComentario);
        }
    }
}
