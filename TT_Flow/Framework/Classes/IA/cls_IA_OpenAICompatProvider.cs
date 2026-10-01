using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TT_Flow.FrameWork.IA
{
    public class cls_IA_OpenAICompatProvider : IIAProvider
    {
        private readonly cls_IA_Config _config;

        public cls_IA_OpenAICompatProvider(cls_IA_Config config)
        {
            _config = config;
        }

        public IAProviderResponse GerarResposta(IAProviderRequest request)
        {
            if (_config == null || string.IsNullOrWhiteSpace(_config.CompatApiKey))
            {
                return new IAProviderResponse
                {
                    Sucesso = false,
                    Erro = "Chave do provider OpenAI compativel nao configurada. Informe uma nova chave na tela Configuracao IA."
                };
            }

            if (string.IsNullOrWhiteSpace(_config.CompatBaseUrl))
            {
                return new IAProviderResponse
                {
                    Sucesso = false,
                    Erro = "Base URL do provider OpenAI compativel nao configurada."
                };
            }

            string erroBaseUrl;
            if (!BaseUrlPermitida(_config.CompatBaseUrl, out erroBaseUrl))
            {
                return new IAProviderResponse
                {
                    Sucesso = false,
                    Erro = erroBaseUrl
                };
            }

            try
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;

                JObject payload = MontarPayload(request);

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(90);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.CompatApiKey);

                    IAHttpResultadoProvider resultado = cls_IA_ProviderUtils.ExecutarPostComRetry(client, MontarEndpoint(_config.CompatBaseUrl), payload.ToString());

                    if (!resultado.SucessoHttp)
                    {
                        return new IAProviderResponse
                        {
                            Sucesso = false,
                            Erro = "Erro no provider OpenAI compativel: " + resultado.StatusCode + cls_IA_ProviderUtils.ExtrairResumoErroProvider(resultado.Json) + cls_IA_ProviderUtils.DescreverTentativas(resultado.Tentativas),
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
                    Erro = "Erro ao chamar provider OpenAI compativel: " + ex.Message
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
                payload["max_tokens"] = request.MaxTokensSaida;
            }

            JArray tools = cls_IA_ProviderUtils.ConverterFerramentasParaOpenAIChat(request.Ferramentas);
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

        private static string MontarEndpoint(string baseUrl)
        {
            string url = (baseUrl ?? string.Empty).Trim().TrimEnd('/');
            if (url.EndsWith("/chat/completions", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }

            return url + "/chat/completions";
        }

        // Compartilhada com a tela de Configuracao IA; a chave e as conversas sao enviadas
        // para esta URL, entao host local/rede interna e http sem TLS sao bloqueados.
        public static bool BaseUrlPermitida(string baseUrl, out string erro)
        {
            erro = string.Empty;

            Uri uri;
            if (!Uri.TryCreate((baseUrl ?? string.Empty).Trim(), UriKind.Absolute, out uri))
            {
                erro = "Base URL do provider OpenAI compativel invalida.";
                return false;
            }

            if (uri.Scheme != Uri.UriSchemeHttps)
            {
                erro = "Base URL do provider OpenAI compativel deve usar https.";
                return false;
            }

            if (EhHostPrivado(uri))
            {
                erro = "Base URL do provider OpenAI compativel nao pode apontar para host local ou de rede interna.";
                return false;
            }

            return true;
        }

        private static bool EhHostPrivado(Uri uri)
        {
            string host = (uri.Host ?? string.Empty).Trim().ToLowerInvariant();

            if (host == "localhost" || host == "0.0.0.0" || host.EndsWith(".local") || host.EndsWith(".internal"))
            {
                return true;
            }

            IPAddress ip;
            if (IPAddress.TryParse(host.Trim('[', ']'), out ip))
            {
                return EhIpPrivado(ip);
            }

            return false;
        }

        private static bool EhIpPrivado(IPAddress ip)
        {
            if (IPAddress.IsLoopback(ip))
            {
                return true;
            }

            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                byte[] bytes = ip.GetAddressBytes();
                return bytes[0] == 0 ||
                       bytes[0] == 10 ||
                       (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) ||
                       (bytes[0] == 192 && bytes[1] == 168) ||
                       (bytes[0] == 169 && bytes[1] == 254);
            }

            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                byte[] bytes = ip.GetAddressBytes();
                return ip.IsIPv6LinkLocal ||
                       ip.IsIPv6SiteLocal ||
                       (bytes[0] & 0xFE) == 0xFC;
            }

            return false;
        }
    }
}
