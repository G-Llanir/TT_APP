using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TT_Flow.FrameWork.IA
{
    public class cls_IA_OpenAIProvider : IIAProvider, IIAProviderStreaming
    {
        private readonly cls_IA_Config _config;

        public cls_IA_OpenAIProvider(cls_IA_Config config)
        {
            _config = config;
        }

        public IAProviderResponse GerarResposta(IAProviderRequest request)
        {
            if (_config == null || string.IsNullOrWhiteSpace(_config.OpenAIApiKey))
            {
                return new IAProviderResponse
                {
                    Sucesso = false,
                    Erro = "Chave OpenAI nao configurada. Informe uma nova chave na tela Configuracao IA ou configure TT_FLOW_IA_OPENAI_API_KEY no IIS."
                };
            }

            try
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;

                JObject payload = MontarPayload(request);

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(90);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.OpenAIApiKey);

                    IAHttpResultadoProvider resultado = cls_IA_ProviderUtils.ExecutarPostComRetry(client, "https://api.openai.com/v1/responses", payload.ToString());

                    if (!resultado.SucessoHttp)
                    {
                        return new IAProviderResponse
                        {
                            Sucesso = false,
                            Erro = "Erro no provider OpenAI: " + resultado.StatusCode + cls_IA_ProviderUtils.DescreverTentativas(resultado.Tentativas),
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
                    Erro = "Erro ao chamar provider OpenAI: " + ex.Message
                };
            }
        }

        public IAProviderResponse GerarRespostaStream(IAProviderRequest request, Action<string> onDeltaTexto)
        {
            if (_config == null || string.IsNullOrWhiteSpace(_config.OpenAIApiKey))
            {
                return new IAProviderResponse
                {
                    Sucesso = false,
                    Erro = "Chave OpenAI nao configurada. Informe uma nova chave na tela Configuracao IA ou configure TT_FLOW_IA_OPENAI_API_KEY no IIS."
                };
            }

            try
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;

                JObject payload = MontarPayload(request);
                payload["stream"] = true;

                using (HttpClient client = new HttpClient())
                {
                    // Stream pode durar mais que a chamada bufferizada; o timeout cobre a leitura completa
                    client.Timeout = TimeSpan.FromSeconds(300);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.OpenAIApiKey);

                    IAHttpStreamResultadoProvider resultado = cls_IA_ProviderUtils.ExecutarPostStreamComRetry(client, "https://api.openai.com/v1/responses", payload.ToString());

                    if (!resultado.SucessoHttp)
                    {
                        return new IAProviderResponse
                        {
                            Sucesso = false,
                            Erro = "Erro no provider OpenAI: " + resultado.StatusCode + cls_IA_ProviderUtils.DescreverTentativas(resultado.Tentativas),
                            JsonOriginal = resultado.Json
                        };
                    }

                    StringBuilder textoAcumulado = new StringBuilder();
                    string jsonCompleto = string.Empty;
                    string erroStream = string.Empty;

                    try
                    {
                        using (Stream stream = resultado.Response.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult())
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            string linha;
                            while ((linha = reader.ReadLine()) != null)
                            {
                                if (!linha.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                                {
                                    continue;
                                }

                                string dados = linha.Substring(5).Trim();
                                if (string.IsNullOrWhiteSpace(dados) || dados == "[DONE]")
                                {
                                    continue;
                                }

                                JObject evento = cls_IA_ProviderUtils.ParseObjetoSeguro(dados);
                                string tipo = cls_IA_ProviderUtils.Texto(evento["type"]);

                                if (tipo == "response.output_text.delta")
                                {
                                    string delta = evento["delta"] != null ? evento["delta"].ToString() : string.Empty;
                                    if (delta.Length > 0)
                                    {
                                        textoAcumulado.Append(delta);
                                        if (onDeltaTexto != null)
                                        {
                                            try { onDeltaTexto(delta); } catch { }
                                        }
                                    }
                                }
                                else if (tipo == "response.completed" && evento["response"] != null)
                                {
                                    jsonCompleto = evento["response"].ToString(Newtonsoft.Json.Formatting.None);
                                }
                                else if (tipo == "response.failed" || tipo == "error")
                                {
                                    erroStream = cls_IA_ProviderUtils.ExtrairResumoErroProvider(dados);
                                }
                            }
                        }
                    }
                    finally
                    {
                        resultado.Response.Dispose();
                    }

                    if (!string.IsNullOrWhiteSpace(jsonCompleto))
                    {
                        return ConverterResposta(jsonCompleto);
                    }

                    if (!string.IsNullOrWhiteSpace(erroStream))
                    {
                        return new IAProviderResponse
                        {
                            Sucesso = false,
                            Erro = "Erro no provider OpenAI (stream)" + erroStream
                        };
                    }

                    // Stream terminou sem o evento final; usa o texto acumulado para nao perder a resposta
                    return new IAProviderResponse
                    {
                        Sucesso = textoAcumulado.Length > 0,
                        Resposta = textoAcumulado.ToString().Trim(),
                        Erro = textoAcumulado.Length > 0 ? string.Empty : "Stream do provider OpenAI terminou sem resposta.",
                        JsonOriginal = string.Empty,
                        OutputItens = new JArray()
                    };
                }
            }
            catch (Exception ex)
            {
                return new IAProviderResponse
                {
                    Sucesso = false,
                    Erro = "Erro ao chamar provider OpenAI (stream): " + ex.Message
                };
            }
        }

        private JObject MontarPayload(IAProviderRequest request)
        {
            JObject payload = new JObject
            {
                { "model", request.Modelo },
                { "instructions", request.Instrucoes },
                { "store", request.StoreExterno },
                { "input", MontarInput(request) }
            };

            if (!string.IsNullOrWhiteSpace(_config.RaciocinioNivel))
            {
                payload["reasoning"] = new JObject
                {
                    { "effort", _config.RaciocinioNivel }
                };
            }

            if (!string.IsNullOrWhiteSpace(_config.VerbosidadeTexto))
            {
                payload["text"] = new JObject
                {
                    { "verbosity", _config.VerbosidadeTexto }
                };
            }

            if (request.Ferramentas != null && request.Ferramentas.Count > 0)
            {
                payload["tools"] = request.Ferramentas;
                payload["tool_choice"] = request.ForcarSemFerramentas ? "none" : "auto";
            }

            if (request.MaxTokensSaida > 0)
            {
                payload["max_output_tokens"] = request.MaxTokensSaida;
            }

            return payload;
        }

        private static JToken MontarInput(IAProviderRequest request)
        {
            JArray historico = MontarHistoricoInput(request);

            if (request.ToolResults != null && request.ToolResults.Count > 0)
            {
                JArray input = new JArray();

                foreach (JToken item in historico)
                {
                    input.Add(item.DeepClone());
                }

                if (!string.IsNullOrWhiteSpace(request.MensagemUsuario))
                {
                    input.Add(new JObject
                    {
                        { "role", "user" },
                        { "content", request.MensagemUsuario }
                    });
                }

                if (request.InputItens != null)
                {
                    foreach (JToken item in request.InputItens)
                    {
                        input.Add(item.DeepClone());
                    }
                }

                foreach (IAToolResult resultado in request.ToolResults)
                {
                    input.Add(new JObject
                    {
                        { "type", "function_call_output" },
                        { "call_id", resultado.CallId },
                        { "output", resultado.ResultadoJson ?? "{}" }
                    });
                }

                return input;
            }

            if (request.InputItens != null && request.InputItens.Count > 0)
            {
                return request.InputItens;
            }

            if (historico.Count > 0)
            {
                JArray input = new JArray();

                foreach (JToken item in historico)
                {
                    input.Add(item.DeepClone());
                }

                input.Add(new JObject
                {
                    { "role", "user" },
                    { "content", request.MensagemUsuario ?? string.Empty }
                });

                return input;
            }

            return request.MensagemUsuario ?? string.Empty;
        }

        private static JArray MontarHistoricoInput(IAProviderRequest request)
        {
            JArray itens = new JArray();

            if (request.Historico == null)
            {
                return itens;
            }

            foreach (IAMensagemHistorico item in request.Historico)
            {
                itens.Add(new JObject
                {
                    { "role", string.Equals(item.Papel, "ASSISTANT", StringComparison.OrdinalIgnoreCase) ? "assistant" : "user" },
                    { "content", item.Conteudo ?? string.Empty }
                });
            }

            return itens;
        }

        private static IAProviderResponse ConverterResposta(string json)
        {
            JObject obj = JObject.Parse(json);
            IAProviderResponse retorno = new IAProviderResponse
            {
                Sucesso = true,
                JsonOriginal = json,
                Resposta = ExtrairTexto(obj),
                OutputItens = obj["output"] as JArray ?? new JArray()
            };
            retorno.ToolCalls = ExtrairToolCalls(obj);

            JToken usage = obj["usage"];
            if (usage != null)
            {
                retorno.TokensEntrada = cls_IA_ProviderUtils.ParaInt(usage["input_tokens"]);
                retorno.TokensSaida = cls_IA_ProviderUtils.ParaInt(usage["output_tokens"]);
            }

            if (string.IsNullOrWhiteSpace(retorno.Resposta) && retorno.ToolCalls.Count == 0)
            {
                retorno.Resposta = cls_IA_ProviderUtils.RespostaTextualIndisponivel;
            }

            return retorno;
        }

        private static string ExtrairTexto(JObject obj)
        {
            string outputText = obj["output_text"] != null ? obj["output_text"].ToString() : string.Empty;
            if (!string.IsNullOrWhiteSpace(outputText))
            {
                return outputText.Trim();
            }

            JToken output = obj["output"];
            if (output == null)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();

            foreach (JToken item in output.Children())
            {
                JToken content = item["content"];
                if (content == null)
                {
                    continue;
                }

                foreach (JToken parte in content.Children())
                {
                    string texto = parte["text"] != null ? parte["text"].ToString() : string.Empty;
                    string recusa = parte["refusal"] != null ? parte["refusal"].ToString() : string.Empty;

                    if (!string.IsNullOrWhiteSpace(texto))
                    {
                        sb.AppendLine(texto.Trim());
                    }
                    else if (!string.IsNullOrWhiteSpace(recusa))
                    {
                        sb.AppendLine(recusa.Trim());
                    }
                }
            }

            return sb.ToString().Trim();
        }

        private static System.Collections.Generic.List<IAToolCall> ExtrairToolCalls(JObject obj)
        {
            System.Collections.Generic.List<IAToolCall> chamadas = new System.Collections.Generic.List<IAToolCall>();
            JToken output = obj["output"];
            if (output == null)
            {
                return chamadas;
            }

            foreach (JToken item in output.Children())
            {
                string tipo = cls_IA_ProviderUtils.Texto(item["type"]);
                string nome = cls_IA_ProviderUtils.Texto(item["name"]);
                string argumentosJson = item["arguments"] != null ? item["arguments"].ToString() : string.Empty;

                if (tipo != "function_call" || string.IsNullOrWhiteSpace(nome))
                {
                    continue;
                }

                chamadas.Add(new IAToolCall
                {
                    CallId = cls_IA_ProviderUtils.Texto(item["call_id"]),
                    Nome = nome,
                    ArgumentosJson = argumentosJson,
                    Argumentos = cls_IA_ProviderUtils.ParseObjetoSeguro(argumentosJson)
                });
            }

            return chamadas;
        }
    }
}
