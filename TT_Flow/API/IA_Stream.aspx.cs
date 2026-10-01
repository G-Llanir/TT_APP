using System;
using System.IO;
using System.Text;
using System.Web.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TT_Flow.FrameWork.IA;

namespace TT_Flow.App
{
    // Endpoint de streaming do chat IA: devolve eventos NDJSON (um JSON por linha) com flush imediato.
    // Sessao ReadOnly para nao segurar o lock de sessao durante a resposta longa.
    public partial class IA_Stream : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/x-ndjson";
            Response.Charset = "utf-8";
            Response.BufferOutput = false;
            Response.CacheControl = "no-cache";
            Server.ScriptTimeout = 330;

            if (!string.Equals(Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
            {
                EscreverEvento("erro", "Metodo nao suportado.");
                FinalizarResposta();
                return;
            }

            // Mesma barreira dos WebMethods: exige corpo JSON, o que bloqueia post de formulario cross-site
            string contentType = Request.ContentType ?? string.Empty;
            if (contentType.IndexOf("json", StringComparison.OrdinalIgnoreCase) < 0)
            {
                EscreverEvento("erro", "Content-Type invalido.");
                FinalizarResposta();
                return;
            }

            string corpo;
            using (StreamReader reader = new StreamReader(Request.InputStream, Encoding.UTF8))
            {
                corpo = reader.ReadToEnd();
            }

            JObject json;
            try
            {
                json = JObject.Parse(corpo);
            }
            catch
            {
                json = new JObject();
            }

            string idConversaIA = json["idConversaIA"] != null ? json["idConversaIA"].ToString() : "0";
            string mensagem = json["sMensagem"] != null ? json["sMensagem"].ToString() : string.Empty;
            string arquivosJson = json["sArquivosIAJson"] != null ? json["sArquivosIAJson"].ToString() : "[]";
            string contextoJson = json["sContextoJson"] != null ? json["sContextoJson"].ToString() : string.Empty;
            string usarConhecimento = json["bUsarConhecimento"] != null ? json["bUsarConhecimento"].ToString() : "N";

            cls_IA_Chat chat = new cls_IA_Chat();
            chat.OnEventoStream = EscreverEvento;
            // A permissao 695 e revalidada no servidor (MontarInstrucoesSistema); aqui so repassamos a flag.
            chat.UsarConhecimento = string.Equals(usarConhecimento, "S", StringComparison.OrdinalIgnoreCase);

            IAChatResponse resposta = chat.EnviarMensagemComArquivos(idConversaIA, mensagem, arquivosJson, contextoJson);

            EscreverLinha("{\"t\":\"fim\",\"v\":" + JsonConvert.SerializeObject(resposta) + "}");
            FinalizarResposta();
        }

        private void EscreverEvento(string tipo, string valor)
        {
            EscreverLinha(JsonConvert.SerializeObject(new { t = tipo, v = valor ?? string.Empty }));
        }

        private void EscreverLinha(string linha)
        {
            try
            {
                if (!Response.IsClientConnected)
                {
                    return;
                }

                Response.Write(linha + "\n");
                Response.Flush();
            }
            catch
            {
                // Cliente desconectou durante o stream; o processamento continua e persiste normalmente
            }
        }

        private void FinalizarResposta()
        {
            // CompleteRequest evita a ThreadAbortException do Response.End
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
