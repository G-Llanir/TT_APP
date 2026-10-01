using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TT_Flow.FrameWork.IA
{
    public class IAHttpResultadoProvider
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool SucessoHttp { get; set; }
        public string Json { get; set; }
        public int Tentativas { get; set; }
    }

    public class IAHttpStreamResultadoProvider
    {
        public HttpResponseMessage Response { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public bool SucessoHttp { get; set; }
        public string Json { get; set; }
        public int Tentativas { get; set; }
    }

    public static class cls_IA_ProviderUtils
    {
        private static readonly int[] RetryDelaysMs = { 1000, 2000, 4000 };

        // Texto unico usado por todos os providers quando o modelo nao devolve texto.
        // Centralizado para o orquestrador detectar a resposta vazia e tentar novamente.
        public const string RespostaTextualIndisponivel = "Nao consegui gerar uma resposta textual para esta solicitacao.";

        public static bool RespostaVazia(string texto)
        {
            return string.IsNullOrWhiteSpace(texto) ||
                   string.Equals(texto.Trim(), RespostaTextualIndisponivel, System.StringComparison.OrdinalIgnoreCase);
        }

        public static IAHttpResultadoProvider ExecutarPostComRetry(HttpClient client, string url, string payloadJson)
        {
            int tentativa = 0;

            while (true)
            {
                tentativa++;
                bool ultimaTentativa = tentativa > RetryDelaysMs.Length;

                try
                {
                    // HttpClient descarta o content apos o envio; cada tentativa precisa de um StringContent novo
                    using (StringContent content = new StringContent(payloadJson, Encoding.UTF8, "application/json"))
                    {
                        HttpResponseMessage response = client.PostAsync(url, content)
                            .ConfigureAwait(false)
                            .GetAwaiter()
                            .GetResult();

                        string json = response.Content.ReadAsStringAsync()
                            .ConfigureAwait(false)
                            .GetAwaiter()
                            .GetResult();

                        if (ultimaTentativa || !EhStatusTransitorio(response.StatusCode))
                        {
                            return new IAHttpResultadoProvider
                            {
                                StatusCode = response.StatusCode,
                                SucessoHttp = response.IsSuccessStatusCode,
                                Json = json,
                                Tentativas = tentativa
                            };
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    // timeout do HttpClient: o request Web ja esperou o limite; repetir estouraria o executionTimeout do ASP.NET
                    throw;
                }
                catch (HttpRequestException)
                {
                    if (ultimaTentativa)
                    {
                        throw;
                    }
                }

                Thread.Sleep(RetryDelaysMs[tentativa - 1]);
            }
        }

        // Variante para streaming: devolve a resposta com o corpo ainda aberto para leitura.
        // O retry cobre apenas a fase antes do primeiro byte (status transitorio ou queda de conexao).
        public static IAHttpStreamResultadoProvider ExecutarPostStreamComRetry(HttpClient client, string url, string payloadJson)
        {
            int tentativa = 0;

            while (true)
            {
                tentativa++;
                bool ultimaTentativa = tentativa > RetryDelaysMs.Length;

                try
                {
                    HttpRequestMessage requisicao = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new StringContent(payloadJson, Encoding.UTF8, "application/json")
                    };

                    HttpResponseMessage response = client.SendAsync(requisicao, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        return new IAHttpStreamResultadoProvider
                        {
                            Response = response,
                            StatusCode = response.StatusCode,
                            SucessoHttp = true,
                            Json = string.Empty,
                            Tentativas = tentativa
                        };
                    }

                    HttpStatusCode statusCode = response.StatusCode;
                    string json = response.Content.ReadAsStringAsync()
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                    response.Dispose();

                    if (ultimaTentativa || !EhStatusTransitorio(statusCode))
                    {
                        return new IAHttpStreamResultadoProvider
                        {
                            Response = null,
                            StatusCode = statusCode,
                            SucessoHttp = false,
                            Json = json,
                            Tentativas = tentativa
                        };
                    }
                }
                catch (TaskCanceledException)
                {
                    throw;
                }
                catch (HttpRequestException)
                {
                    if (ultimaTentativa)
                    {
                        throw;
                    }
                }

                Thread.Sleep(RetryDelaysMs[tentativa - 1]);
            }
        }

        public static string DescreverTentativas(int tentativas)
        {
            return tentativas > 1 ? " (apos " + tentativas + " tentativas)" : string.Empty;
        }

        private static bool EhStatusTransitorio(HttpStatusCode statusCode)
        {
            int status = (int)statusCode;
            return status == 429 || status == 502 || status == 503 || status == 504;
        }

        public static JArray ClonarArray(JArray origem)
        {
            return origem == null ? new JArray() : (JArray)origem.DeepClone();
        }

        public static JObject ClonarObjeto(JToken origem)
        {
            return origem == null ? new JObject() : (JObject)origem.DeepClone();
        }

        public static JArray ConverterFerramentasParaGemini(JArray ferramentas)
        {
            JArray tools = new JArray();

            foreach (JToken item in ferramentas ?? new JArray())
            {
                tools.Add(new JObject
                {
                    { "type", "function" },
                    { "name", Texto(item["name"]) },
                    { "description", Texto(item["description"]) },
                    { "parameters", SanitizarSchemaGemini(item["parameters"]) }
                });
            }

            return tools;
        }

        public static JArray ConverterFerramentasParaClaude(JArray ferramentas)
        {
            JArray tools = new JArray();

            foreach (JToken item in ferramentas ?? new JArray())
            {
                tools.Add(new JObject
                {
                    { "name", Texto(item["name"]) },
                    { "description", Texto(item["description"]) },
                    { "input_schema", item["parameters"] != null ? item["parameters"].DeepClone() : new JObject { { "type", "object" } } }
                });
            }

            return tools;
        }

        public static JArray ConverterFerramentasParaOpenAIChat(JArray ferramentas)
        {
            JArray tools = new JArray();

            foreach (JToken item in ferramentas ?? new JArray())
            {
                JObject function = new JObject
                {
                    { "name", Texto(item["name"]) },
                    { "description", Texto(item["description"]) },
                    { "parameters", item["parameters"] != null ? item["parameters"].DeepClone() : new JObject { { "type", "object" } } }
                };

                if (item["strict"] != null)
                {
                    function["strict"] = item["strict"].DeepClone();
                }

                tools.Add(new JObject
                {
                    { "type", "function" },
                    { "function", function }
                });
            }

            return tools;
        }

        public static JArray ConverterFerramentasParaGroq(JArray ferramentas)
        {
            JArray tools = new JArray();

            foreach (JToken item in ferramentas ?? new JArray())
            {
                JObject function = new JObject
                {
                    { "name", Texto(item["name"]) },
                    { "description", Texto(item["description"]) },
                    { "parameters", item["parameters"] != null ? item["parameters"].DeepClone() : new JObject { { "type", "object" } } }
                };

                tools.Add(new JObject
                {
                    { "type", "function" },
                    { "function", function }
                });
            }

            return tools;
        }

        public static string Texto(JToken token)
        {
            return token == null ? string.Empty : token.ToString().Trim();
        }

        public static int ParaInt(JToken token)
        {
            int retorno;
            return token != null && int.TryParse(token.ToString(), out retorno) ? retorno : 0;
        }

        public static JObject ParseObjetoSeguro(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new JObject();
            }

            try
            {
                return JObject.Parse(json);
            }
            catch
            {
                return new JObject();
            }
        }

        public static string JsonCompacto(JToken token)
        {
            return token == null ? string.Empty : token.ToString(Formatting.None);
        }

        public static string ExtrairResumoErroProvider(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return string.Empty;
            }

            try
            {
                JObject obj = JObject.Parse(json);
                JToken erro = obj["error"];
                string mensagem = erro == null ? string.Empty : Texto(erro["message"]);

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

        private static JObject SanitizarSchemaGemini(JToken schema)
        {
            JObject origem = schema as JObject;
            if (origem == null)
            {
                return new JObject { { "type", "object" } };
            }

            JObject destino = new JObject();
            CopiarSeExiste(destino, origem, "type");
            CopiarSeExiste(destino, origem, "description");
            CopiarSeExiste(destino, origem, "enum");
            CopiarSeExiste(destino, origem, "required");
            CopiarSeExiste(destino, origem, "nullable");

            JObject propriedadesOrigem = origem["properties"] as JObject;
            if (propriedadesOrigem != null)
            {
                JObject propriedadesDestino = new JObject();
                foreach (JProperty propriedade in propriedadesOrigem.Properties())
                {
                    propriedadesDestino[propriedade.Name] = SanitizarSchemaGemini(propriedade.Value);
                }

                destino["properties"] = propriedadesDestino;
            }

            if (origem["items"] != null)
            {
                destino["items"] = SanitizarSchemaGemini(origem["items"]);
            }

            if (destino["type"] == null)
            {
                destino["type"] = "object";
            }

            return destino;
        }

        private static void CopiarSeExiste(JObject destino, JObject origem, string nome)
        {
            JToken valor = origem[nome];
            if (valor != null)
            {
                destino[nome] = valor.DeepClone();
            }
        }
    }
}
