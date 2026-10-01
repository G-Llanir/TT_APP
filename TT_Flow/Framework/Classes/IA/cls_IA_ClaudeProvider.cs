using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TT_Flow.FrameWork.IA
{
    public class cls_IA_ClaudeProvider : IIAProvider, IIAProviderStreaming
    {
        private readonly cls_IA_Config _config;
        private const string Endpoint = "https://api.anthropic.com/v1/messages";

        public cls_IA_ClaudeProvider(cls_IA_Config config)
        {
            _config = config;
        }

        public IAProviderResponse GerarResposta(IAProviderRequest request)
        {
            if (_config == null || string.IsNullOrWhiteSpace(_config.ClaudeApiKey))
            {
                return new IAProviderResponse
                {
                    Sucesso = false,
                    Erro = "Chave Claude nao configurada. Informe uma nova chave na tela Configuracao IA ou configure TT_FLOW_IA_CLAUDE_API_KEY no IIS."
                };
            }

            try
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;

                JObject payload = MontarPayload(request);

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(90);
                    client.DefaultRequestHeaders.Add("x-api-key", _config.ClaudeApiKey);
                    client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

                    IAHttpResultadoProvider resultado = cls_IA_ProviderUtils.ExecutarPostComRetry(client, Endpoint, payload.ToString());

                    if (!resultado.SucessoHttp)
                    {
                        return new IAProviderResponse
                        {
                            Sucesso = false,
                            Erro = "Erro no provider Claude: " + resultado.StatusCode + cls_IA_ProviderUtils.ExtrairResumoErroProvider(resultado.Json) + cls_IA_ProviderUtils.DescreverTentativas(resultado.Tentativas),
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
                    Erro = "Erro ao chamar provider Claude: " + ex.Message
                };
            }
        }

        public IAProviderResponse GerarRespostaStream(IAProviderRequest request, Action<string> onDeltaTexto)
        {
            if (_config == null || string.IsNullOrWhiteSpace(_config.ClaudeApiKey))
            {
                return new IAProviderResponse
                {
                    Sucesso = false,
                    Erro = "Chave Claude nao configurada. Informe uma nova chave na tela Configuracao IA ou configure TT_FLOW_IA_CLAUDE_API_KEY no IIS."
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
                    client.DefaultRequestHeaders.Add("x-api-key", _config.ClaudeApiKey);
                    client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

                    IAHttpStreamResultadoProvider resultado = cls_IA_ProviderUtils.ExecutarPostStreamComRetry(client, Endpoint, payload.ToString());

                    if (!resultado.SucessoHttp)
                    {
                        return new IAProviderResponse
                        {
                            Sucesso = false,
                            Erro = "Erro no provider Claude: " + resultado.StatusCode + cls_IA_ProviderUtils.ExtrairResumoErroProvider(resultado.Json) + cls_IA_ProviderUtils.DescreverTentativas(resultado.Tentativas),
                            JsonOriginal = resultado.Json
                        };
                    }

                    JArray content = new JArray();
                    Dictionary<int, StringBuilder> jsonParciais = new Dictionary<int, StringBuilder>();
                    int tokensEntrada = 0;
                    int tokensSaida = 0;
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
                                if (string.IsNullOrWhiteSpace(dados))
                                {
                                    continue;
                                }

                                JObject evento = cls_IA_ProviderUtils.ParseObjetoSeguro(dados);
                                string tipo = cls_IA_ProviderUtils.Texto(evento["type"]);

                                if (tipo == "message_start")
                                {
                                    JToken usageInicio = evento["message"] != null ? evento["message"]["usage"] : null;
                                    if (usageInicio != null)
                                    {
                                        tokensEntrada = cls_IA_ProviderUtils.ParaInt(usageInicio["input_tokens"]);
                                    }
                                }
                                else if (tipo == "content_block_start")
                                {
                                    int indice = cls_IA_ProviderUtils.ParaInt(evento["index"]);
                                    JObject blocoInicial = evento["content_block"] as JObject;
                                    JObject bloco = blocoInicial != null
                                        ? (JObject)blocoInicial.DeepClone()
                                        : new JObject { { "type", "text" }, { "text", string.Empty } };

                                    while (content.Count <= indice)
                                    {
                                        content.Add(new JObject());
                                    }

                                    content[indice] = bloco;
                                }
                                else if (tipo == "content_block_delta")
                                {
                                    int indice = cls_IA_ProviderUtils.ParaInt(evento["index"]);
                                    JToken delta = evento["delta"];
                                    string tipoDelta = cls_IA_ProviderUtils.Texto(delta == null ? null : delta["type"]);
                                    JObject bloco = indice < content.Count ? content[indice] as JObject : null;

                                    if (tipoDelta == "text_delta")
                                    {
                                        string textoDelta = delta["text"] != null ? delta["text"].ToString() : string.Empty;
                                        if (textoDelta.Length > 0)
                                        {
                                            if (bloco != null)
                                            {
                                                string textoAtual = bloco["text"] != null ? bloco["text"].ToString() : string.Empty;
                                                bloco["text"] = textoAtual + textoDelta;
                                            }

                                            if (onDeltaTexto != null)
                                            {
                                                try { onDeltaTexto(textoDelta); } catch { }
                                            }
                                        }
                                    }
                                    else if (tipoDelta == "input_json_delta")
                                    {
                                        string parcial = delta["partial_json"] != null ? delta["partial_json"].ToString() : string.Empty;
                                        if (!jsonParciais.ContainsKey(indice))
                                        {
                                            jsonParciais[indice] = new StringBuilder();
                                        }

                                        jsonParciais[indice].Append(parcial);
                                    }
                                    // thinking_delta e signature_delta sao ignorados: nao entram na resposta final
                                }
                                else if (tipo == "content_block_stop")
                                {
                                    int indice = cls_IA_ProviderUtils.ParaInt(evento["index"]);
                                    if (jsonParciais.ContainsKey(indice))
                                    {
                                        JObject bloco = indice < content.Count ? content[indice] as JObject : null;
                                        if (bloco != null)
                                        {
                                            bloco["input"] = cls_IA_ProviderUtils.ParseObjetoSeguro(jsonParciais[indice].ToString());
                                        }
                                    }
                                }
                                else if (tipo == "message_delta")
                                {
                                    JToken usageFim = evento["usage"];
                                    if (usageFim != null)
                                    {
                                        tokensSaida = cls_IA_ProviderUtils.ParaInt(usageFim["output_tokens"]);
                                    }
                                }
                                else if (tipo == "error")
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

                    if (!string.IsNullOrWhiteSpace(erroStream))
                    {
                        return new IAProviderResponse
                        {
                            Sucesso = false,
                            Erro = "Erro no provider Claude (stream)" + erroStream
                        };
                    }

                    JObject reconstruido = new JObject
                    {
                        { "content", content },
                        { "usage", new JObject { { "input_tokens", tokensEntrada }, { "output_tokens", tokensSaida } } }
                    };

                    return ConverterResposta(reconstruido.ToString(Newtonsoft.Json.Formatting.None));
                }
            }
            catch (Exception ex)
            {
                return new IAProviderResponse
                {
                    Sucesso = false,
                    Erro = "Erro ao chamar provider Claude (stream): " + ex.Message
                };
            }
        }

        private JObject MontarPayload(IAProviderRequest request)
        {
            JObject payload = new JObject
            {
                { "model", request.Modelo },
                { "max_tokens", request.MaxTokensSaida > 0 ? request.MaxTokensSaida : 1200 },
                { "system", request.Instrucoes ?? string.Empty },
                { "messages", MontarMensagens(request) }
            };

            JArray tools = cls_IA_ProviderUtils.ConverterFerramentasParaClaude(request.Ferramentas);
            if (tools.Count > 0)
            {
                payload["tools"] = tools;
                payload["tool_choice"] = new JObject
                {
                    { "type", request.ForcarSemFerramentas ? "none" : "auto" }
                };
            }

            AplicarRaciocinio(payload, request);
            return payload;
        }

        private static JArray MontarMensagens(IAProviderRequest request)
        {
            JArray messages = new JArray();
            AdicionarHistorico(messages, request);

            messages.Add(new JObject
            {
                { "role", "user" },
                { "content", request.MensagemUsuario ?? string.Empty }
            });

            if (request.ToolResults == null || request.ToolResults.Count == 0)
            {
                return messages;
            }

            messages.Add(new JObject
            {
                { "role", "assistant" },
                { "content", cls_IA_ProviderUtils.ClonarArray(request.InputItens) }
            });

            JArray toolResults = new JArray();
            foreach (IAToolResult resultado in request.ToolResults)
            {
                toolResults.Add(new JObject
                {
                    { "type", "tool_result" },
                    { "tool_use_id", resultado.CallId },
                    { "content", resultado.ResultadoJson ?? "{}" },
                    { "is_error", resultado.Erro }
                });
            }

            messages.Add(new JObject
            {
                { "role", "user" },
                { "content", toolResults }
            });

            return messages;
        }

        private static void AdicionarHistorico(JArray messages, IAProviderRequest request)
        {
            if (request.Historico == null || request.Historico.Count == 0)
            {
                return;
            }

            // A Messages API exige turnos alternados comecando por user; mensagens fora de ordem sao puladas
            string ultimoRole = string.Empty;
            foreach (IAMensagemHistorico item in request.Historico)
            {
                string role = string.Equals(item.Papel, "ASSISTANT", StringComparison.OrdinalIgnoreCase) ? "assistant" : "user";

                if (messages.Count == 0 && role != "user")
                {
                    continue;
                }

                if (role == ultimoRole)
                {
                    continue;
                }

                messages.Add(new JObject
                {
                    { "role", role },
                    { "content", item.Conteudo ?? string.Empty }
                });
                ultimoRole = role;
            }

            // A mensagem atual do usuario vem em seguida: o historico precisa terminar em assistant
            if (ultimoRole == "user" && messages.Count > 0)
            {
                messages.RemoveAt(messages.Count - 1);
            }
        }

        private void AplicarRaciocinio(JObject payload, IAProviderRequest request)
        {
            string raciocinio = (_config.RaciocinioNivel ?? string.Empty).Trim().ToLowerInvariant();
            if (raciocinio == "none")
            {
                return;
            }

            // Extended thinking da Messages API: formato valido e apenas thinking { type: enabled, budget_tokens }.
            // Regras da API: budget_tokens >= 1024 e max_tokens > budget_tokens. So habilitamos quando o admin
            // define um budget manual e essas condicoes sao atendidas; caso contrario nao enviamos thinking
            // (o modelo responde normalmente). Campos 'adaptive', 'display' e 'output_config' nao existem na API.
            int maxTokens = request.MaxTokensSaida > 0 ? request.MaxTokensSaida : 1200;
            if (_config.ThinkingBudgetTokens >= 1024 && maxTokens > 1024 && ModeloAceitaBudgetManual(request.Modelo))
            {
                int budget = _config.ThinkingBudgetTokens;
                if (budget >= maxTokens)
                {
                    budget = maxTokens - 1;
                }

                if (budget >= 1024)
                {
                    payload["thinking"] = new JObject
                    {
                        { "type", "enabled" },
                        { "budget_tokens", budget }
                    };
                }
            }
        }

        private static IAProviderResponse ConverterResposta(string json)
        {
            JObject obj = JObject.Parse(json);
            JArray content = obj["content"] as JArray ?? new JArray();

            IAProviderResponse retorno = new IAProviderResponse
            {
                Sucesso = true,
                JsonOriginal = json,
                Resposta = ExtrairTexto(content),
                OutputItens = content
            };
            retorno.ToolCalls = ExtrairToolCalls(content);

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

        private static string ExtrairTexto(JArray content)
        {
            StringBuilder sb = new StringBuilder();

            foreach (JToken bloco in content)
            {
                if (cls_IA_ProviderUtils.Texto(bloco["type"]) != "text")
                {
                    continue;
                }

                string texto = cls_IA_ProviderUtils.Texto(bloco["text"]);
                if (!string.IsNullOrWhiteSpace(texto))
                {
                    sb.AppendLine(texto);
                }
            }

            return sb.ToString().Trim();
        }

        private static System.Collections.Generic.List<IAToolCall> ExtrairToolCalls(JArray content)
        {
            System.Collections.Generic.List<IAToolCall> chamadas = new System.Collections.Generic.List<IAToolCall>();

            foreach (JToken bloco in content)
            {
                if (cls_IA_ProviderUtils.Texto(bloco["type"]) != "tool_use")
                {
                    continue;
                }

                string nome = cls_IA_ProviderUtils.Texto(bloco["name"]);
                JToken input = bloco["input"];
                string argumentosJson = input == null ? "{}" : input.ToString(Newtonsoft.Json.Formatting.None);

                chamadas.Add(new IAToolCall
                {
                    CallId = cls_IA_ProviderUtils.Texto(bloco["id"]),
                    Nome = nome,
                    ArgumentosJson = argumentosJson,
                    Argumentos = input as JObject ?? cls_IA_ProviderUtils.ParseObjetoSeguro(argumentosJson)
                });
            }

            return chamadas;
        }

        private static bool ModeloAceitaBudgetManual(string modelo)
        {
            modelo = (modelo ?? string.Empty).ToLowerInvariant();
            return !(modelo.Contains("claude-opus-4-8") ||
                     modelo.Contains("claude-opus-4-7") ||
                     modelo.Contains("claude-fable-5") ||
                     modelo.Contains("claude-mythos-5"));
        }

    }
}
