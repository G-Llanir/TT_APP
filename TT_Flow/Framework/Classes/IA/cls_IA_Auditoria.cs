using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;
using TT.FrameWork;

namespace TT_Flow.FrameWork.IA
{
    public static class cls_IA_Auditoria
    {
        private const string Procedure = "sp_Manipula_tbl_Flow_IA_Chat";

        public static void Registrar(int idConversaIA, string evento, string severidade, object payload)
        {
            string idUsuario = "0";
            try
            {
                idUsuario = TT.FrameWork.Identity.Variaveis.idUsuario();
            }
            catch
            {
                idUsuario = "0";
            }

            RegistrarComUsuario(idConversaIA, idUsuario, evento, severidade, payload);
        }

        public static void RegistrarComUsuario(int idConversaIA, string idUsuario, string evento, string severidade, object payload)
        {
            try
            {
                HttpContext contexto = HttpContext.Current;
                HttpRequest request = contexto != null ? contexto.Request : null;

                Dictionary<string, string> parametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "SALVAR_AUDITORIA" },
                    { "@idConversaIA", idConversaIA.ToString() },
                    { "@idUsuario", string.IsNullOrWhiteSpace(idUsuario) ? "0" : idUsuario },
                    { "@sEvento", evento ?? string.Empty },
                    { "@sSeveridade", string.IsNullOrWhiteSpace(severidade) ? "INFO" : severidade },
                    { "@sIP", request != null ? request.UserHostAddress : string.Empty },
                    { "@sUserAgent", request != null ? request.UserAgent : string.Empty },
                    { "@sURLOrigem", request != null ? request.RawUrl : string.Empty },
                    { "@sPayloadJson", payload == null ? string.Empty : JsonConvert.SerializeObject(payload) }
                };

                BD.ExecutarDataTable(Procedure, parametros, false);
            }
            catch
            {
                // Auditoria é best-effort aqui para não mascarar o erro principal já tratado no fluxo do chat.
            }
        }
    }
}
