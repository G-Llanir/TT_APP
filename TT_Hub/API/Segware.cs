using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using TT_Hub.FrameWork;
using System.Net;
using System.Web.Services;
using System.Text;
using BD = TT.FrameWork.BD;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Hub.API
{
    public class Segware
    {
        public Segware()
        {
            //Carregar parametros

            DataSet dsParametros = new DataSet("SW");
            string sProcedure = "sp_HUB_Consulta_Tarefas_ConsumirWS";
            try
            {

                dsParametros = BD.ExecutarDataSet(sProcedure);





            }
            catch (Exception ex)
            {
                throw new Exception("Erro: " + ex.Message);
            }


        }

        public void Enviar()
        {

            try
            {

                DataSet dsEnviar;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR ENVIO");
              
                dsEnviar = BD.ExecutarDataSet("sp_HUB_Manipula_SW", vParametros);
                if (BD.ValidarDataSet(dsEnviar))
                {
                    for (int nLinha = 0; nLinha < dsEnviar.Tables[0].Rows.Count; nLinha++)
                    {
                        string idOcorrencia = RETORNO.DATASET(dsEnviar, nLinha, "idOcorrencia");
                        string sTipoOcorrecia = RETORNO.DATASET(dsEnviar, nLinha, "sTipoOcorrecia");
                        string sEndereco = RETORNO.DATASET(dsEnviar, nLinha, "sw_API_Alarm");
                        string sCabecalho = string.Format("Authorization: {0}", RETORNO.DATASET(dsEnviar, nLinha, "sw_API_Token"));
                        string sJsonEnvio = MontajSonEnvio(dsEnviar, nLinha);


                        string sErro = ConsumirWS(sEndereco, sCabecalho, sJsonEnvio, "POST", "Json");
                        


                        DataSet dsRegistro;
                        Dictionary<String, String> vParametros_RegistroEnvio = new Dictionary<string, string>();
                        vParametros_RegistroEnvio.Add("@sFuncao", "REGISTRAR_ENVIO");
                        vParametros_RegistroEnvio.Add("@idOcorrencia", idOcorrencia);
                        vParametros_RegistroEnvio.Add("@sTipoOcorrecia", sTipoOcorrecia);
                        vParametros_RegistroEnvio.Add("@sErro", sErro);
                        dsRegistro = BD.ExecutarDataSet("sp_HUB_Manipula_SW", vParametros_RegistroEnvio);



                        if (sErro == "")
                        {

                        }

                        

                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception (ex.Message);
            }


        }

        string MontajSonEnvio(DataSet dsEnvio, int nLinha)
        {
            System.Text.StringBuilder sbJson = new System.Text.StringBuilder();
            try
            {

                sbJson.AppendLine("{");
                sbJson.AppendLine(string.Format("\"uniqueId\":\"{0}\",", RETORNO.DATASET(dsEnvio, nLinha, "uniqueId").Trim()));
                sbJson.AppendLine("\"events\":[{");
                sbJson.AppendLine(string.Format("   \"account\":\"{0}\",", RETORNO.DATASET(dsEnvio, nLinha, "account").Trim()));
                sbJson.AppendLine(string.Format("   \"auxiliary\":\"{0}\",", RETORNO.DATASET(dsEnvio, nLinha, "auxiliary").Trim()));
                sbJson.AppendLine(string.Format("   \"code\":\"{0}\",", RETORNO.DATASET(dsEnvio, nLinha, "code").Trim()));
                sbJson.AppendLine(string.Format("   \"eventId\":\"{0}\",", RETORNO.DATASET(dsEnvio, nLinha, "eventID").Trim()));
                sbJson.AppendLine(string.Format("   \"protocolType\":\"{0}\"", RETORNO.DATASET(dsEnvio, nLinha, "protocolType").Trim()));
                sbJson.AppendLine("}]}");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar jSon Envio: " + ex.ToString());
            }
            return sbJson.ToString();

        }

        string ConsumirWS(string sURLWS, string sCabecalho, string sCorpoMesagem, string sMetodo, string sAssinatura)
        {
            string sRetorno = "";
            string lStrUsuario = string.Empty;
            string lStrSenha = string.Empty;


            try
            {
            

                WebClient WSClient = new WebClient();
                WSClient.UploadStringCompleted += (send, args) =>
                {
                    sRetorno = sMetodo + ":" + (args.Error == null ? args.Result : args.Error.Message);
                };

              
                    //WSClient.Headers.Add(HttpRequestHeader.Cookie, sCookieEnvio);
                    //WSClient.Headers.Add(HttpRequestHeader.ContentType, "multipart/form­data");
              
                WSClient.Encoding = Encoding.UTF8;



                if (sAssinatura != "")
                {

                    if (sAssinatura == "Json")
                    {
                        WSClient.Headers.Add("accept", "*/*");
                        WSClient.Headers.Add(sCabecalho);
                        WSClient.Headers.Add("Content-Type", "application/json");
                    }
                }
                string s = WSClient.UploadString(new Uri(sURLWS), "POST", sCorpoMesagem);
                sRetorno = s;

            }
            catch (Exception ex)
            {
                sRetorno = "Erro ao consumir API: " + ex.ToString();
            }
            return sRetorno;
        }
    }
}