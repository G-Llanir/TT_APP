using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using static TT.FrameWork.Funcoes;

namespace Framework.API
{
    public class Funcoes_API
    {
        public static string ConsumirAPI(string sURLWS) => ConsumirAPI(sURLWS, "", "", "POST", "Json");

        //public static string ConsumirAPI(string sURLWS, string sCabecalho, string sCorpoMesagem, string sMetodo, string sAssinatura)
        //{
        //    string sRetorno = "";
        //    string lStrUsuario = string.Empty;
        //    string lStrSenha = string.Empty;

        //    try
        //    {
        //        WebClient WSClient = new WebClient();
        //        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        //        WSClient.UploadStringCompleted += (send, args) => { sRetorno = sMetodo + ":" + (args.Error == null ? args.Result : args.Error.Message); };
        //        WSClient.Encoding = Encoding.UTF8;

        //        if (sAssinatura != "")
        //        {
        //            if (sAssinatura == "Json")
        //            {
        //                WSClient.Headers.Add("accept", "*/*");
                        
        //                if (!string.IsNullOrEmpty(sCabecalho)) WSClient.Headers.Add(sCabecalho);

        //                WSClient.Headers.Add("Content-Type", "application/json");
        //            }
        //        }

        //        sRetorno = WSClient.UploadString(new Uri(sURLWS), "POST", sCorpoMesagem);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Erro ao consumir API: " + ex.ToString());
        //    }

        //    return sRetorno;
        //}

        public static string ConsumirAPI(string sURLWS, string sCabecalho, string sCorpoMesagem, string sMetodo, string sAssinatura)
        {
            string sRetorno = "";

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                using (WebClient WSClient = new WebClient())
                {
                    WSClient.Encoding = Encoding.UTF8;

                    if (!string.IsNullOrEmpty(sAssinatura) && sAssinatura.Equals("Json", StringComparison.OrdinalIgnoreCase))
                    {
                        WSClient.Headers.Add("accept", "*/*");

                        if (!string.IsNullOrEmpty(sCabecalho)) WSClient.Headers.Add(sCabecalho);

                        WSClient.Headers.Add("Content-Type", "application/json");
                    }

                    sRetorno = WSClient.UploadString(new Uri(sURLWS), sMetodo ?? "POST", sCorpoMesagem ?? "");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao consumir API: " + RetornaMensagem_Erro(ex));
            }

            return sRetorno;
        }

        public class Post
        {
            public string codigo_xml { get; set; }

        }

        //public static string Consumir_API_REST_FULL(string sURL, string sXML)
        //{
        //    WebRequest wrGETURL;
        //    wrGETURL = WebRequest.Create(sURL);
        //    wrGETURL.Method = "POST";

        //    HttpClient client = new HttpClient();

        //    var content = new StringContent("codigo_xml=" + sXML);

        //    HttpResponseMessage response =  client.PostAsync(sURL, (HttpContent)content).Result;


        //    var post = new Post
        //    {
        //        codigo_xml = sXML
        //    };

        //    HttpResponseMessage response1 = client.PostAsync("https://jsonplaceholder.typicode.com/posts", post);



        //    wrGETURL.ContentType = @"application/json; charset=utf-8";
        //    using (var stream = new StreamWriter(wrGETURL.GetRequestStream()))
        //    {
        //        var bodyContent = new
        //        {
        //            codigo_xml = sXML,
        //        }; // This will need to be changed to an actual class after finding what the specification sheet requires.

        //        //var json = JsonConvert.SerializeObject(bodyContent);

        //        stream.Write(bodyContent);
        //    }
        //    HttpWebResponse webresponse = wrGETURL.GetResponse() as HttpWebResponse;

        //    Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
        //    // read response stream from response object
        //    StreamReader loResponseStream = new StreamReader(webresponse.GetResponseStream(), enc);
        //    // read string from stream data
        //    string strResult = loResponseStream.ReadToEnd();
        //    // close the stream object
        //    loResponseStream.Close();
        //    // close the response object
        //    webresponse.Close();

        //    return strResult;

        //}

        //public static async Task<string> ConsumirAPI(string sURLWS, string sCabecalho, string sCorpoMesagem, string sMetodo, string sAssinatura)
        //{
        //    try
        //    {
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        //        using (HttpClient client = new HttpClient())
        //        {
        //            client.Timeout = TimeSpan.FromSeconds(90);

        //            if (!string.IsNullOrEmpty(sAssinatura) && sAssinatura.Equals("Json", StringComparison.OrdinalIgnoreCase))
        //            {
        //                client.DefaultRequestHeaders.Accept.Clear();
        //                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        //            }

        //            if (!string.IsNullOrEmpty(sCabecalho))
        //            {
        //                foreach (var cab in sCabecalho.Split(';'))
        //                {
        //                    if (!string.IsNullOrWhiteSpace(cab))
        //                    {
        //                        var partes = cab.Split(':');
        //                        if (partes.Length == 2)
        //                            client.DefaultRequestHeaders.Add(partes[0].Trim(), partes[1].Trim());
        //                    }
        //                }
        //            }

        //            var conteudo = new StringContent(sCorpoMesagem ?? "", Encoding.UTF8, "application/json");

        //            HttpResponseMessage resposta;

        //            sMetodo = (sMetodo ?? "POST").ToUpper();

        //            switch (sMetodo)
        //            {
        //                case "GET":
        //                    resposta = await client.GetAsync(sURLWS);
        //                    break;

        //                case "PUT":
        //                    resposta = await client.PutAsync(sURLWS, conteudo);
        //                    break;

        //                case "DELETE":
        //                    resposta = await client.DeleteAsync(sURLWS);
        //                    break;

        //                default:
        //                case "POST":
        //                    resposta = await client.PostAsync(sURLWS, conteudo);
        //                    break;
        //            }

        //            string retorno = await resposta.Content.ReadAsStringAsync();

        //            if (!resposta.IsSuccessStatusCode) throw new Exception($"Erro ao consumir API ({(int)resposta.StatusCode}): {resposta.ReasonPhrase}\nResposta: {retorno}");

        //            return retorno;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Erro ao consumir API: " + ex.Message, ex);
        //    }
        //}
    }
}