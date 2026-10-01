using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TT_Flow.FrameWork.IA
{
    [Serializable]
    public class IAContextoTela
    {
        public string Tela { get; set; }
        public int IdRegistro { get; set; }
        public string Titulo { get; set; }
        public string Resumo { get; set; }
    }

    // O contexto de tela vem do cliente e serve apenas para orientar o modelo sobre onde o usuario esta.
    // Ele nunca concede acesso a dados: qualquer informacao real continua saindo das ferramentas,
    // que validam a permissao do usuario no servidor a cada chamada.
    public static class cls_IA_Contexto
    {
        private static readonly string[] TelasPermitidas = { "PEDIDO", "PARCEIRO", "PRODUTO", "MENSAGEM", "COLABORADOR", "GERAL" };

        public static IAContextoTela Parse(string contextoJson)
        {
            if (string.IsNullOrWhiteSpace(contextoJson))
            {
                return null;
            }

            try
            {
                JObject obj = JObject.Parse(contextoJson);
                string tela = Texto(obj["tela"]).ToUpperInvariant();

                if (!TelaPermitida(tela))
                {
                    return null;
                }

                return new IAContextoTela
                {
                    Tela = tela,
                    IdRegistro = ParaInt(obj["id"]),
                    Titulo = cls_IA_Sanitizacao.LimparEntradaUsuario(Texto(obj["titulo"]), 200),
                    Resumo = cls_IA_Sanitizacao.LimparEntradaUsuario(Texto(obj["resumo"]), 600)
                };
            }
            catch
            {
                return null;
            }
        }

        public static string Serializar(string tela, int idRegistro, string titulo, string resumo)
        {
            return new JObject
            {
                { "tela", (tela ?? string.Empty).Trim().ToUpperInvariant() },
                { "id", idRegistro },
                { "titulo", cls_IA_Sanitizacao.Resumir(titulo ?? string.Empty, 200) },
                { "resumo", cls_IA_Sanitizacao.Resumir(resumo ?? string.Empty, 600) }
            }.ToString(Formatting.None);
        }

        public static string NormalizarTela(string tela)
        {
            tela = (tela ?? string.Empty).Trim().ToUpperInvariant();
            return TelaPermitida(tela) ? tela : string.Empty;
        }

        private static bool TelaPermitida(string tela)
        {
            foreach (string permitida in TelasPermitidas)
            {
                if (permitida == tela)
                {
                    return true;
                }
            }

            return false;
        }

        private static string Texto(JToken token)
        {
            return token == null ? string.Empty : token.ToString().Trim();
        }

        private static int ParaInt(JToken token)
        {
            int retorno;
            return token != null && int.TryParse(token.ToString(), out retorno) ? retorno : 0;
        }
    }
}
