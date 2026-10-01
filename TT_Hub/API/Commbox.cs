using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Linq;
using BD = TT.FrameWork.BD;
using RETORNO = TT.FrameWork.BD.Retorno;
using System.Data;
using System.Net.NetworkInformation;


namespace TT_Hub.API
{
    public class Commbox
    {

        public void ConsultarStatusEquipamentos(string idEquipamento)
        {
            string txt = "";
            ConsultarStatusEquipamentos(ref txt, idEquipamento);
        }

        public void ConsultarStatusEquipamentos(ref string txt, string idEquipamento)
        {

            try
            {

                DataSet dsConsultar;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR EQUIPAMENTOS");
                vParametros.Add("@idEquipamento", idEquipamento);
                dsConsultar = BD.ExecutarDataSet("sp_HUB_Consulta_Tarefas_Commbox", vParametros);
                if (BD.ValidarDataSet(dsConsultar))
                {
                    for (int nLinha = 0; nLinha < dsConsultar.Tables[0].Rows.Count; nLinha++)
                    {
                        try
                        {
                            idEquipamento           = RETORNO.DATASET(dsConsultar, nLinha, "idEquipamento");
                            string sEnderecoIP      = RETORNO.DATASET(dsConsultar, nLinha, "sEnderecoIP");
                            string sPorta           = RETORNO.DATASET(dsConsultar, nLinha, "sPorta");
                            string sUsuario         = RETORNO.DATASET(dsConsultar, nLinha, "sUsuario");
                            string sSenha           = RETORNO.DATASET(dsConsultar, nLinha, "sSenha");
                            string sDescricao       = RETORNO.DATASET(dsConsultar, nLinha, "sDescricao");

                            if (sPorta == "")
                            {
                                sPorta = "80";
                            }

                            Ping objPing = new Ping();
                            PingReply objRespostaPing = objPing.Send(sEnderecoIP, 4000);

                            if (objRespostaPing.Status == IPStatus.Success)
                            {
                                txt = DateTime.Now.ToString() + string.Format(" - Consultando: {0} - IP:{1} - {2} ", sDescricao, sEnderecoIP, "Input") + Environment.NewLine + txt;
                                string sJsonInputs = ConsumirEquipamento(sEnderecoIP, sPorta, "get_input_status", "", "POST", "");
                                txt = DateTime.Now.ToString() + string.Format(" - Retorno: {0}  ", sJsonInputs) + Environment.NewLine + txt;

                                txt = DateTime.Now.ToString() + string.Format(" - Consultando: {0} - IP:{1} - {2} ", sDescricao, sEnderecoIP, "OutPut") + Environment.NewLine + txt;
                                string sJsonOutputs = ConsumirEquipamento(sEnderecoIP, sPorta, "get_output_status", "", "POST", "");
                                txt = DateTime.Now.ToString() + string.Format(" - Retorno: {0}  ", sJsonOutputs) + Environment.NewLine + txt;


                                if (sJsonInputs != "" && sJsonOutputs != "")
                                {

                                    DataSet dsPesquisa;
                                    Dictionary<String, String> vParametros_Incluir = new Dictionary<string, string>();
                                    string sErro = "";
                                    string idEvento = "";
                                    vParametros_Incluir.Add("@sFuncao", "INCLUIR");
                                    vParametros_Incluir.Add("@idEquipamento", idEquipamento);
                                    vParametros_Incluir.Add("@sIPSolicitacao", sEnderecoIP);
                                    vParametros_Incluir.Add("@sNomeArquivo", "");
                                    vParametros_Incluir.Add("@sTamanhoArquivo", "0");
                                    vParametros_Incluir.Add("@sTamanhoPost", "0");
                                    dsPesquisa = BD.ExecutarDataSet("sp_HUB_Manipula_tbl_Eventos", vParametros_Incluir);

                                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                                    {
                                        idEvento = BD.Retorno.DATASET(dsPesquisa, 0, "idEvento");
                                        idEquipamento = BD.Retorno.DATASET(dsPesquisa, 0, "idEquipamento");

                                    }


                                    GravarStatusEquipamento(idEvento, sJsonOutputs, "O");
                                    GravarStatusEquipamento(idEvento, sJsonInputs, "I");


                                    //Finalizar inclusão do Registro
                                    txt = DateTime.Now.ToString() + string.Format(" - Gerando Eventos para " + sDescricao) + Environment.NewLine + txt;

                                    vParametros.Clear();
                                    DataSet dsGerarEentos;
                                    Dictionary<String, String> vParametros_Gerar = new Dictionary<string, string>();
                                    vParametros.Add("@idEvento", idEvento);

                                    dsGerarEentos = BD.ExecutarDataSet("sp_HUB_Gerar_Eventos_Ocorrencia", vParametros);

                                }
                            }
                            else
                            {
                                txt = DateTime.Now.ToString() + string.Format(" - Equipamento Indisponivel: {0} - IP:{1}", objRespostaPing.Status, sEnderecoIP) + Environment.NewLine + txt;

                            }
                            objPing.Dispose();
                        }
                        catch (Exception ex)
                        {
                            txt = DateTime.Now.ToString() + string.Format(" - Erro: {0} ", ex.Message ) + Environment.NewLine + txt;
                        }

                    }

                }
            }
            catch
            {

            }

        }
        public void EnviarAcaoEquipamento(string idRegistroAcao)
        {
            string txt = "";
            EnviarAcaoEquipamento(ref txt, idRegistroAcao);
        }
        public void EnviarAcaoEquipamento(ref string txt, string idRegistroAcao)
        {
            string sTpEnvio = "Processo";
            string sErro = "";
            try
            {

                if (idRegistroAcao != "")
                {
                    sTpEnvio = "Página";
                }

                DataSet dsConsultar;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao",         "ENVIAR ACAO");
                vParametros.Add("@idRegistroAcao",  idRegistroAcao);
                dsConsultar = BD.ExecutarDataSet("sp_HUB_Consulta_Tarefas_Commbox", vParametros);
                if (BD.ValidarDataSet(dsConsultar, out sErro))
                {
                    for (int nLinha = 0; nLinha < dsConsultar.Tables[0].Rows.Count; nLinha++)
                    {
                        try
                        {
                            idRegistroAcao = RETORNO.DATASET(dsConsultar, nLinha, "idRegistroAcao");
                            string idEquipamento = RETORNO.DATASET(dsConsultar, nLinha, "idEquipamento");
                            string sEnderecoIP = RETORNO.DATASET(dsConsultar, nLinha, "sEnderecoIP");
                            string sPorta = RETORNO.DATASET(dsConsultar, nLinha, "sPorta");
                            string sUsuario = RETORNO.DATASET(dsConsultar, nLinha, "sUsuario");
                            string sSenha = RETORNO.DATASET(dsConsultar, nLinha, "sSenha");
                            string sComando = RETORNO.DATASET(dsConsultar, nLinha, "sComando");
                            string sDescricao = RETORNO.DATASET(dsConsultar, nLinha, "sDescricao");
                            string sObservacao = RETORNO.DATASET(dsConsultar, nLinha, "sObservacao");
                            string sJsonInputs = "";
                            string[] objComando = sComando.Split('|');


                            Ping objPing = new Ping();
                            PingReply objRespostaPing = objPing.Send(sEnderecoIP, 4000);

                            if (objRespostaPing.Status == IPStatus.Success)
                            {
                                for (int i = 0; i < objComando.Count(); i++)
                                {
                                    if (sPorta == "")
                                    {
                                        sPorta = "80";
                                    }

                                    RegistraLog(ref txt, string.Format("Enviando: {0} - IP:{1} - {2} ", sDescricao, sEnderecoIP, sObservacao));
                                    sJsonInputs = ConsumirEquipamento(sEnderecoIP, sPorta, objComando[i], "", "POST", "");
                                    RegistraLog(ref txt, string.Format("Retorno: {0} ", sJsonInputs));
                                }
                                if (sJsonInputs.Substring(0, 1) != "#" && sJsonInputs == "{\"result\":\"sucess\",\"data\":null}")
                                {
                                    DataSet dsRegistrarEnvio;
                                    Dictionary<String, String> vParametros_RegistraEnvio = new Dictionary<string, string>();
                                    vParametros_RegistraEnvio.Add("@sFuncao", "REGISTRAR ENVIO");
                                    vParametros_RegistraEnvio.Add("@idRegistroAcao", idRegistroAcao);
                                    vParametros_RegistraEnvio.Add("@sTpEnvio", sTpEnvio);
                                    dsRegistrarEnvio = BD.ExecutarDataSet("sp_HUB_Consulta_Tarefas_Commbox", vParametros_RegistraEnvio);
                                    dsRegistrarEnvio.Dispose();

                                    //ConsultarStatusEquipamentos(idEquipamento);


                                }
                                else
                                {
                                    throw new Exception(sJsonInputs);
                                }
                            }
                            else
                            {
                                throw new Exception( string.Format("Erro acesso PING: {0} - IP:{1} - {2} ", objRespostaPing.Status, sEnderecoIP, sObservacao));

                            }
                        }
                        catch (Exception ex)
                        {
                            RegistraLog(ref txt, ex.Message);
                        }
                    }
                }
                else
                {
                    RegistraLog(ref txt, string.Format("Erro ao Consultar BD: {0} ", sErro));
                }
                dsConsultar.Dispose();

            }
            catch (Exception ex)
            {
                RegistraLog(ref txt, string.Format(" - Erro Geral: {0} ", ex.Message));
                throw new Exception(ex.Message);
            }

        }
        void GravarStatusEquipamento(string idEvento, string sJSon, string sTipo_Bloco)
        {

            string[] sRetorno = sJSon.Split('[');
            string[] sValorFiltrado = sRetorno[1].Split(']');
            string[] sValorInputs = sValorFiltrado[0].Split(',');
            Dictionary<String, String> vParametros = new Dictionary<string, string>();


            DataSet ds_DI;
            for (int i = 0; i < sValorInputs.Count(); i++)
            {
                vParametros.Clear();
                string sBloco = sTipo_Bloco + (sValorInputs.Count() - i).ToString().PadLeft(2, '0');
                string sValor = sValorInputs[i].ToString();

                vParametros.Add("@idEvento", idEvento);
                vParametros.Add("@sParametro", sBloco);
                vParametros.Add("@sValor", sValor);
                vParametros.Add("@sRegistroCompleto", "");
                ds_DI = BD.ExecutarDataSet("sp_HUB_Manipula_tbl_Eventos_Itens", vParametros);
                ds_DI.Dispose();

            }



        }

        string ConsumirEquipamento(string sEnderecoEquipamento, string sPorta, string sFuncao, string sCorpoMesagem, string sMetodo, string sAssinatura)
        {
            string sRetorno = "";
            string lStrUsuario = string.Empty;
            string lStrSenha = string.Empty;
            string sURLWS = string.Format("http://{0}:{1}/{2}", sEnderecoEquipamento, sPorta, sFuncao);

            try
            {


                WebClient WSClient = new WebClient();
                //WSClient.UploadStringCompleted += (send, args) =>
                //{
                //    sRetorno = sMetodo + ":" + (args.Error == null ? args.Result : args.Error.Message);
                //};


                //WSClient.Headers.Add(HttpRequestHeader.Cookie, sCookieEnvio);
                //WSClient.Headers.Add(HttpRequestHeader.ContentType, "multipart/form­data");

                WSClient.Encoding = Encoding.UTF8;



                if (sAssinatura != "")
                {

                    if (sAssinatura == "Json")
                    {
                        WSClient.Headers.Add("accept", "*/*");
                        // WSClient.Headers.Add(sCabecalho);
                        WSClient.Headers.Add("Content-Type", "application/json");
                    }
                }
                string s = WSClient.DownloadString(new Uri(sURLWS));
                sRetorno = s;

            }
            catch (Exception ex)
            {
                sRetorno = "#Erro no Envio: " + ex.Message;
            }
            return sRetorno;
        }

        void RegistraLog(ref string txt, string Mensagem)
        {
            string sme = txt;
            txt = DateTime.Now.ToString() + " - " + Mensagem + Environment.NewLine +  txt;
        }
    }

}