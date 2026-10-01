using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TT_Flow.FrameWork.IA
{
    public class cls_IA_GroqProvider : IIAProvider
    {
        private readonly cls_IA_Config _config;
        private const string Endpoint = "https://api.groq.com/openai/v1/chat/completions";

        public cls_IA_GroqProvider(cls_IA_Config config)
        {
            _config = config;
        }

        public IAProviderResponse GerarResposta(IAProviderRequest request)
        {
            if (_config == null || string.IsNullOrWhiteSpace(_config.GroqApiKey))
            {
                return new IAProviderResponse
                {
                    Sucesso = false,
                    Erro = "Chave Groq nao configurada. Informe uma nova chave na tela Configuracao IA ou configure TT_FLOW_IA_GROQ_API_KEY no IIS."
                };
            }

            try
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;

                JObject payload = MontarPayload(request);

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(90);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.GroqApiKey);

                    IAHttpResultadoProvider resultado = cls_IA_ProviderUtils.ExecutarPostComRetry(client, Endpoint, payload.ToString());

                    if (!resultado.SucessoHttp)
                    {
                        return new IAProviderResponse
                        {
                            Sucesso = false,
                            Erro = "Erro no provider Groq: " + resultado.StatusCode + cls_IA_ProviderUtils.ExtrairResumoErroProvider(resultado.Json) + cls_IA_ProviderUtils.DescreverTentativas(resultado.Tentativas),
                            JsonOriginal = resultado.Json
                        };
                    }

                    return ConverterResposta(resultado.Json);
                }
            }
            catch (Exception ex)
            {
                return new IAProviderResponse
                {
                    Sucesso = false,
                    Erro = "Erro ao chamar provider Groq: " + ex.Message
                };
            }
        }

        private JObject MontarPayload(IAProviderRequest request)
        {
            JObject payload = new JObject
            {
                { "model", request.Modelo },
                { "messages", MontarMensagens(request) }
            };

            if (request.MaxTokensSaida > 0)
            {
                payload["max_completion_tokens"] = request.MaxTokensSaida;
            }

            JArray tools = cls_IA_ProviderUtils.ConverterFerramentasParaGroq(request.Ferramentas);
            if (tools.Count > 0)
            {
                payload["tools"] = tools;
                payload["tool_choice"] = request.ForcarSemFerramentas ? "none" : "auto";
            }

            return payload;
        }

        private static JArray MontarMensagens(IAProviderRequest request)
        {
            JArray messages = new JArray();

            if (!string.IsNullOrWhiteSpace(request.Instrucoes))
            {
                messages.Add(new JObject
                {
                    { "role", "system" },
                    { "content", request.Instrucoes }
                });
            }

            if (request.Historico != null)
            {
                foreach (IAMensagemHistorico item in request.Historico)
                {
                    messages.Add(new JObject
                    {
                        { "role", string.Equals(item.Papel, "ASSISTANT", StringComparison.OrdinalIgnoreCase) ? "assistant" : "user" },
                        { "content", item.Conteudo ?? string.Empty }
                    });
                }
            }

            messages.Add(new JObject
            {
                { "role", "user" },
                { "content", request.MensagemUsuario ?? string.Empty }
            });

            if (request.ToolResults == null || request.ToolResults.Count == 0)
            {
                return messages;
            }

            if (request.InputItens != null)
            {
                foreach (JToken item in request.InputItens)
                {
                    messages.Add(item.DeepClone());
                }
            }

            foreach (IAToolResult resultado in request.ToolResults)
            {
                messages.Add(new JObject
                {
                    { "role", "tool" },
                    { "tool_call_id", resultado.CallId },
                    { "content", resultado.ResultadoJson ?? "{}" }
                });
            }

            return messages;
        }

        private static IAProviderResponse ConverterResposta(string json)
        {
            JObject obj = JObject.Parse(json);
            JToken message = obj["choices"] != null && obj["choices"].HasValues ? obj["choices"][0]["message"] : null;
            JArray outputItens = new JArray();
            if (message != null)
            {
                outputItens.Add(message.DeepClone());
            }

            IAProviderResponse retorno = new IAProviderResponse
            {
                Sucesso = true,
                JsonOriginal = json,
                Resposta = ExtrairTexto(message),
                OutputItens = outputItens
            };
            retorno.ToolCalls = ExtrairToolCalls(message);

            JToken usage = obj["usage"];
            if (usage != null)
            {
                retorno.TokensEntrada = cls_IA_ProviderUtils.ParaInt(usage["prompt_tokens"]);
                retorno.TokensSaida = cls_IA_ProviderUtils.ParaInt(usage["completion_tokens"]);
            }

            if (string.IsNullOrWhiteSpace(retorno.Resposta) && retorno.ToolCalls.Count == 0)
            {
                retorno.Resposta = cls_IA_ProviderUtils.RespostaTextualIndisponivel;
            }

            return retorno;
        }

        private static string ExtrairTexto(JToken message)
        {
            if (message == null)
            {
                return string.Empty;
            }

            JToken content = message["content"];
            if (content == null)
            {
                return string.Empty;
            }

            if (content.Type == JTokenType.String)
            {
                return content.ToString().Trim();
            }

            StringBuilder sb = new StringBuilder();
            foreach (JToken parte in content.Children())
            {
                string texto = cls_IA_ProviderUtils.Texto(parte["text"]);
                if (!string.IsNullOrWhiteSpace(texto))
                {
                    sb.AppendLine(texto);
                }
            }

            return sb.ToString().Trim();
        }

        private static System.Collections.Generic.List<IAToolCall> ExtrairToolCalls(JToken message)
        {
            System.Collections.Generic.List<IAToolCall> chamadas = new System.Collections.Generic.List<IAToolCall>();
            JToken toolCalls = message == null ? null : message["tool_calls"];
            if (toolCalls == null)
            {
                return chamadas;
            }

            foreach (JToken toolCall in toolCalls.Children())
            {
                JToken function = toolCall["function"];
                string nome = cls_IA_ProviderUtils.Texto(function == null ? null : function["name"]);
                string argumentosJson = function != null && function["arguments"] != null ? function["arguments"].ToString() : "{}";

                if (string.IsNullOrWhiteSpace(nome))
                {
                    continue;
                }

                chamadas.Add(new IAToolCall
                {
                    CallId = cls_IA_ProviderUtils.Texto(toolCall["id"]),
                    Nome = nome,
                    ArgumentosJson = argumentosJson,
                    Argumentos = cls_IA_ProviderUtils.ParseObjetoSeguro(argumentosJson)
                });
            }

            return chamadas;
        }
    }
}
