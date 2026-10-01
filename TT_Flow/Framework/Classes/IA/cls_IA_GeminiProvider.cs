using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TT_Flow.FrameWork.IA
{
    public class cls_IA_GeminiProvider : IIAProvider
    {
        private readonly cls_IA_Config _config;
        private const string Endpoint = "https://generativelanguage.googleapis.com/v1beta/interactions";

        public cls_IA_GeminiProvider(cls_IA_Config config)
        {
            _config = config;
        }

        public IAProviderResponse GerarResposta(IAProviderRequest request)
        {
            if (_config == null || string.IsNullOrWhiteSpace(_config.GeminiApiKey))
            {
                return new IAProviderResponse
                {
                    Sucesso = false,
                    Erro = "Chave Gemini nao configurada. Informe uma nova chave na tela Configuracao IA ou configure TT_FLOW_IA_GEMINI_API_KEY no IIS."
                };
            }

            try
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;

                JArray inputEnviado;
                JObject payload = MontarPayload(request, out inputEnviado);

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(90);
                    client.DefaultRequestHeaders.Add("x-goog-api-key", _config.GeminiApiKey);

                    IAHttpResultadoProvider resultado = cls_IA_ProviderUtils.ExecutarPostComRetry(client, Endpoint, payload.ToString());

                    if (!resultado.SucessoHttp)
                    {
                        return new IAProviderResponse
                        {
                            Sucesso = false,
                            Erro = "Erro no provider Gemini: " + resultado.StatusCode + ExtrairResumoErro(resultado.Json) + cls_IA_ProviderUtils.DescreverTentativas(resultado.Tentativas),
                            JsonOriginal = resultado.Json
                        };
                    }

                    return ConverterResposta(resultado.Json, inputEnviado);
                }
            }
            catch (Exception ex)
            {
                return new IAProviderResponse
                {
                    Sucesso = false,
                    Erro = "Erro ao chamar provider Gemini: " + ex.Message
                };
            }
        }

        private JObject MontarPayload(IAProviderRequest request, out JArray inputEnviado)
        {
            inputEnviado = MontarInput(request);

            JObject payload = new JObject
            {
                { "model", request.Modelo },
                { "store", request.StoreExterno },
                { "input", inputEnviado }
            };

            JArray tools = cls_IA_ProviderUtils.ConverterFerramentasParaGemini(request.Ferramentas);
            if (tools.Count > 0)
            {
                payload["tools"] = tools;
            }

            if (request.ForcarSemFerramentas)
            {
                payload["generation_config"] = new JObject
                {
                    { "tool_choice", "none" }
                };
            }

            return payload;
        }

        private JArray MontarInput(IAProviderRequest request)
        {
            if (request.ToolResults != null && request.ToolResults.Count > 0)
            {
                JArray history = cls_IA_ProviderUtils.ClonarArray(request.InputItens);

                foreach (IAToolResult resultado in request.ToolResults)
                {
                    history.Add(new JObject
                    {
                        { "type", "function_result" },
                        { "name", resultado.Nome },
                        { "call_id", resultado.CallId },
                        { "result", new JArray
                            {
                                new JObject
                                {
                                    { "type", "text" },
                                    { "text", resultado.ResultadoJson ?? "{}" }
                                }
                            }
                        }
                    });
                }

                return history;
            }

            if (request.InputItens != null && request.InputItens.Count > 0)
            {
                return cls_IA_ProviderUtils.ClonarArray(request.InputItens);
            }

            return new JArray
            {
                new JObject
                {
                    { "type", "user_input" },
                    { "content", new JArray
                        {
                            new JObject
                            {
                                { "type", "text" },
                                { "text", MontarTextoUsuario(request) }
                            }
                        }
                    }
                }
            };
        }

        private static string MontarTextoUsuario(IAProviderRequest request)
        {
            bool temInstrucoes = !string.IsNullOrWhiteSpace(request.Instrucoes);
            bool temHistorico = request.Historico != null && request.Historico.Count > 0;

            if (!temInstrucoes && !temHistorico)
            {
                return request.MensagemUsuario ?? string.Empty;
            }

            // A Interactions API nao tem papel proprio para historico neste fluxo;
            // o historico vai dobrado no texto, mesmo padrao ja usado para as instrucoes
            StringBuilder texto = new StringBuilder();

            if (temInstrucoes)
            {
                texto.AppendLine("Instrucoes do sistema:");
                texto.AppendLine(request.Instrucoes);
                texto.AppendLine();
            }

            if (temHistorico)
            {
                texto.AppendLine("Historico recente da conversa:");
                foreach (IAMensagemHistorico item in request.Historico)
                {
                    string prefixo = string.Equals(item.Papel, "ASSISTANT", StringComparison.OrdinalIgnoreCase) ? "Assistente: " : "Usuario: ";
                    texto.AppendLine(prefixo + (item.Conteudo ?? string.Empty));
                }

                texto.AppendLine();
            }

            texto.AppendLine("Mensagem do usuario:");
            texto.Append(request.MensagemUsuario ?? string.Empty);
            return texto.ToString();
        }

        private static IAProviderResponse ConverterResposta(string json, JArray inputEnviado)
        {
            JObject obj = JObject.Parse(json);
            JArray steps = obj["steps"] as JArray ?? new JArray();
            JArray history = cls_IA_ProviderUtils.ClonarArray(inputEnviado);

            foreach (JToken step in steps)
            {
                history.Add(step.DeepClone());
            }

            IAProviderResponse retorno = new IAProviderResponse
            {
                Sucesso = true,
                JsonOriginal = json,
                Resposta = ExtrairTexto(obj),
                OutputItens = history
            };
            retorno.ToolCalls = ExtrairToolCalls(steps);

            JToken usage = obj["usage"] ?? obj["usage_metadata"];
            if (usage != null)
            {
                retorno.TokensEntrada = PrimeiroInt(usage["total_input_tokens"], usage["input_tokens"], usage["prompt_token_count"]);
                retorno.TokensSaida = PrimeiroInt(usage["total_output_tokens"], usage["output_tokens"], usage["candidates_token_count"]) +
                                      PrimeiroInt(usage["total_thought_tokens"], usage["thought_tokens"], usage["thoughts_token_count"]);
            }

            if (string.IsNullOrWhiteSpace(retorno.Resposta) && retorno.ToolCalls.Count == 0)
            {
                retorno.Resposta = cls_IA_ProviderUtils.RespostaTextualIndisponivel;
            }

            return retorno;
        }

        private static string ExtrairTexto(JObject obj)
        {
            string outputText = cls_IA_ProviderUtils.Texto(obj["output_text"]);
            if (!string.IsNullOrWhiteSpace(outputText))
            {
                return outputText;
            }

            StringBuilder sb = new StringBuilder();
            JArray steps = obj["steps"] as JArray ?? new JArray();

            foreach (JToken step in steps)
            {
                string tipo = cls_IA_ProviderUtils.Texto(step["type"]);
                if (tipo == "function_call" || tipo == "function_result")
                {
                    continue;
                }

                AdicionarTextoConteudo(sb, step["content"]);
                AdicionarTextoConteudo(sb, step["text"]);
            }

            return sb.ToString().Trim();
        }

        private static void AdicionarTextoConteudo(StringBuilder sb, JToken content)
        {
            if (sb == null || content == null)
            {
                return;
            }

            if (content.Type == JTokenType.String)
            {
                string textoDireto = content.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(textoDireto))
                {
                    sb.AppendLine(textoDireto);
                }

                return;
            }

            if (content.Type == JTokenType.Array)
            {
                foreach (JToken item in content.Children())
                {
                    AdicionarTextoConteudo(sb, item);
                }

                return;
            }

            JObject obj = content as JObject;
            if (obj == null)
            {
                return;
            }

            string texto = cls_IA_ProviderUtils.Texto(obj["text"]);
            string recusa = cls_IA_ProviderUtils.Texto(obj["refusal"]);

            if (!string.IsNullOrWhiteSpace(texto))
            {
                sb.AppendLine(texto);
            }
            else if (!string.IsNullOrWhiteSpace(recusa))
            {
                sb.AppendLine(recusa);
            }

            AdicionarTextoConteudo(sb, obj["content"]);
            AdicionarTextoConteudo(sb, obj["parts"]);
        }

        private static System.Collections.Generic.List<IAToolCall> ExtrairToolCalls(JArray steps)
        {
            System.Collections.Generic.List<IAToolCall> chamadas = new System.Collections.Generic.List<IAToolCall>();

            foreach (JToken step in steps)
            {
                if (cls_IA_ProviderUtils.Texto(step["type"]) != "function_call")
                {
                    continue;
                }

                string nome = cls_IA_ProviderUtils.Texto(step["name"]);
                if (string.IsNullOrWhiteSpace(nome))
                {
                    continue;
                }

                JToken argumentosToken = step["arguments"];
                string argumentosJson = argumentosToken == null ? "{}" : argumentosToken.ToString(Newtonsoft.Json.Formatting.None);

                chamadas.Add(new IAToolCall
                {
                    CallId = PrimeiroTexto(step["id"], step["call_id"]),
                    Nome = nome,
                    ArgumentosJson = argumentosJson,
                    Argumentos = argumentosToken as JObject ?? cls_IA_ProviderUtils.ParseObjetoSeguro(argumentosJson)
                });
            }

            return chamadas;
        }

        private static string PrimeiroTexto(params JToken[] tokens)
        {
            foreach (JToken token in tokens)
            {
                string valor = cls_IA_ProviderUtils.Texto(token);
                if (!string.IsNullOrWhiteSpace(valor))
                {
                    return valor;
                }
            }

            return string.Empty;
        }

        private static string ExtrairResumoErro(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return string.Empty;
            }

            try
            {
                JObject obj = JObject.Parse(json);
                JToken erro = obj["error"];
                string mensagem = erro == null ? string.Empty : cls_IA_ProviderUtils.Texto(erro["message"]);

                if (!string.IsNullOrWhiteSpace(mensagem))
                {
                    return " - " + cls_IA_Sanitizacao.Resumir(mensagem, 300);
                }
            }
            catch
            {
            }

            return " - " + cls_IA_Sanitizacao.Resumir(json, 300);
        }

        private static int PrimeiroInt(params JToken[] tokens)
        {
            foreach (JToken token in tokens)
            {
                int valor = cls_IA_ProviderUtils.ParaInt(token);
                if (valor > 0)
                {
                    return valor;
                }
            }

            return 0;
        }
    }
}
