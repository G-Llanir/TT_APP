using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using TT_Hub.FrameWork;
using System.Xml;
using System.Xml.Linq;
using System.Data;
using BD = TT.FrameWork.BD;
using RETORNO = TT.FrameWork.BD.Retorno;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;



namespace TT_Hub
{
    public partial class Monitor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sxMLEntrada = "";
            string sErro = "";
            string idEvento = "";
            string idEquipamento = "";
            string sTamanhoArquivo = "";
            string sTamanhoPost = "";
            try
            {
                

                string sIPSolicitacao   = Request.ServerVariables["REMOTE_ADDR"].ToString();
                //Verificando envio via form-data;
                sxMLEntrada             = System.Web.HttpUtility.UrlDecode(Request.Form.ToString());
                //Verificando envio via raw
                sxMLEntrada             = new StreamReader(this.Request.InputStream).ReadToEnd();
                sTamanhoArquivo         = this.Request.InputStream.Length.ToString();
                sTamanhoPost            = this.Request.ServerVariables["CONTENT_LENGTH"].ToString();


                //Inclusão de Função de pegar os dados via RAW (padrão de envio PHP).
                if (sxMLEntrada != "")
                {
                    string sDeviceName;
                    string sHostName;
                    string sID;
                    string sSysName;
                    string sSysLocation;


                    //Lendo XML de Entrada
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(sxMLEntrada);

                    //Fatiando o XmL
                    XmlNodeList xml_DeviceInfo = xmlDoc.GetElementsByTagName("DeviceInfo");

                    XmlNodeList xml_HTTPPush = xmlDoc.GetElementsByTagName("HTTPPush");
                    XmlNodeList xml_Time = xmlDoc.GetElementsByTagName("Time");

                    //Varrendo a Info do XML
                    sDeviceName     = xml_DeviceInfo[0]["DeviceName"].InnerText;
                    sHostName       = xml_DeviceInfo[0]["HostName"].InnerText;
                    sID             = xml_DeviceInfo[0]["ID"].InnerText;
                    sSysName        = xml_DeviceInfo[0]["SysName"].InnerText;
                    sSysLocation    = xml_DeviceInfo[0]["SysLocation"].InnerText;

                    //Nome do Arquivo
                    string sNomeArquivo =  sDeviceName + "_" + sID.Replace(":", "") + "_" + FUNCOES.CarimboDataHora() + ".xml";

                    //Inserindo no Banco o Evento recebeido
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "INCLUIR");
                    vParametros.Add("@sID", sID);
                    vParametros.Add("@sDeviceName", sDeviceName);
                    vParametros.Add("@sHostName", sHostName);
                    vParametros.Add("@sSysName", sSysName);
                    vParametros.Add("@sIPSolicitacao", sIPSolicitacao);
                    vParametros.Add("@sNomeArquivo", sNomeArquivo);
                    vParametros.Add("@sTamanhoArquivo", sTamanhoArquivo);
                    vParametros.Add("@sTamanhoPost", sTamanhoPost);
                    dsPesquisa = BD.ExecutarDataSet("sp_HUB_Manipula_tbl_Eventos", vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        idEvento        = BD.Retorno.DATASET(dsPesquisa, 0, "idEvento");
                        idEquipamento   = BD.Retorno.DATASET(dsPesquisa, 0, "idEquipamento");

                    }

                    if (idEvento != "0")
                    {
                        #region | Digital Input
                        //Varrendo    Digital Imput      
                        if (idEvento != "")
                        {
                            DataSet ds_DI;
                            for (int i = 1; i < 10; i++)
                            {
                                vParametros.Clear();
                                string sBloco = "DI" + i.ToString();
                                XmlNodeList xml_DI_Itens = xmlDoc.GetElementsByTagName(sBloco);
                                if (xml_DI_Itens.Count == 0)
                                {
                                    break;
                                }
                                string sdescription = xml_DI_Itens[0]["description"].InnerText;
                                string svalue = xml_DI_Itens[0]["value"].InnerText;
                                string sAlarmState = xml_DI_Itens[0]["alarmState"].InnerText;
                                string salarm = xml_DI_Itens[0]["alarm"].InnerText;
                                string sRegistro = string.Format("BLOCO:{0},description:{1},value:{2},alarmState:{3},alarm:{4}|", sBloco, sdescription, svalue, sAlarmState, salarm);

                                vParametros.Add("@idEvento", idEvento);
                                vParametros.Add("@sParametro", sBloco);
                                vParametros.Add("@sValor", salarm);
                                vParametros.Add("@sRegistroCompleto", sRegistro);
                                ds_DI = BD.ExecutarDataSet("sp_HUB_Manipula_tbl_Eventos_Itens", vParametros);

                            }

                        }
                        #endregion

                        #region | S1
                        //Varrendo    Relê     
                        if (idEvento != "")
                        {
                            for (int i = 1; i < 10; i++)
                            {
                                vParametros.Clear();
                                string sBloco = "S" + i.ToString();
                                XmlNodeList xml_S_Itens = xmlDoc.GetElementsByTagName(sBloco);
                                if (xml_S_Itens.Count == 0)
                                {
                                    break;
                                }



                                string sdescription = xml_S_Itens[0]["description"].InnerText;
                                string svalue = xml_S_Itens[0]["item1"]["value"].InnerText;
                                string unit = xml_S_Itens[0]["item1"]["unit"].InnerText;
                                string alarm = xml_S_Itens[0]["item1"]["alarm"].InnerText;
                                string min = xml_S_Itens[0]["item1"]["min"].InnerText;
                                string max = xml_S_Itens[0]["item1"]["max"].InnerText;
                                string sRegistro = string.Format("BLOCO:{0},description:{1},value:{2},unit{3},alarm{4},min{5},max{6} ", sBloco, sdescription, svalue, unit, alarm, min, max);


                                vParametros.Add("@idEvento", idEvento);
                                vParametros.Add("@sParametro", sBloco);
                                vParametros.Add("@sValor", svalue.ToString());
                                vParametros.Add("@sRegistroCompleto", sRegistro);
                                EnviarItem(vParametros);
                            }

                        }
                        #endregion

                        #region | AI
                        if (idEvento != "")
                        {
                            for (int i = 1; i < 10; i++)
                            {
                                vParametros.Clear();
                                string sBloco = "AI" + i.ToString();
                                XmlNodeList xml_AI_Itens = xmlDoc.GetElementsByTagName(sBloco);
                                if (xml_AI_Itens.Count == 0)
                                {
                                    break;
                                }

                                string sdescription = xml_AI_Itens[0]["description"].InnerText;
                                string svalue = xml_AI_Itens[0]["value"].InnerText;
                                string unit = xml_AI_Itens[0]["unit"].InnerText;
                                string alarm = xml_AI_Itens[0]["alarm"].InnerText;
                                string min = xml_AI_Itens[0]["min"].InnerText;
                                string max = xml_AI_Itens[0]["max"].InnerText;
                                string multiplier = xml_AI_Itens[0]["multiplier"].InnerText;

                                decimal nValor = 0;
                                try
                                {
                                    nValor = Convert.ToDecimal(svalue.Replace('.', ',')) / Convert.ToDecimal(multiplier.Replace('.', ','));
                                }
                                catch
                                {
                                    nValor = Convert.ToDecimal(svalue);
                                }

                                string sRegistro = string.Format("BLOCO:{0},description:{1},value:{2},unit{3},multiplier{4},alarm{5},min{6},max{7} ", sBloco, sdescription, svalue, unit, multiplier, alarm, min, max);

                                vParametros.Add("@idEvento", idEvento);
                                vParametros.Add("@sParametro", sBloco);
                                vParametros.Add("@sValor", svalue + unit);
                                vParametros.Add("@sValor_Calculo", nValor.ToString("n3").Replace(",", "."));
                                vParametros.Add("@sRegistroCompleto", sRegistro);
                                EnviarItem(vParametros);
                            }

                        }
                        #endregion

                        #region | Relé
                        //Varrendo    Relê     
                        if (idEvento != "")
                        {
                            for (int i = 1; i < 10; i++)
                            {
                                vParametros.Clear();
                                string sBloco = "R" + i.ToString();
                                XmlNodeList xml_R_Itens = xmlDoc.GetElementsByTagName(sBloco);
                                if (xml_R_Itens.Count == 0)
                                {
                                    break;
                                }

                                string sdescription = xml_R_Itens[0]["description"].InnerText;
                                string svalue = xml_R_Itens[0]["value"].InnerText;
                                string sRegistro = string.Format("BLOCO:{0},description:{1},value:{2}", sBloco, sdescription, svalue);

                                vParametros.Add("@idEvento", idEvento);
                                vParametros.Add("@sParametro", sBloco);
                                vParametros.Add("@sValor", svalue);
                                vParametros.Add("@sRegistroCompleto", sRegistro);
                                EnviarItem(vParametros);
                            }

                        }
                        #endregion


                        #region | signalpercent
                        //Varrendo    signalpercent     
                        if (idEvento != "")
                        {


                            vParametros.Clear();
                            string sBloco = "signalpercent";
                            XmlNodeList xml_R_Itens = xmlDoc.GetElementsByTagName(sBloco);

                            if (xml_R_Itens.Count > 0)
                            {
                                string svalue = xml_R_Itens[0].InnerText;
                                string sRegistro = string.Format("BLOCO:{0},description:{1},value:{2}", sBloco, "signalpercent", svalue);

                                vParametros.Add("@idEvento", idEvento);
                                vParametros.Add("@sParametro", sBloco);
                                vParametros.Add("@sValor", svalue);
                                vParametros.Add("@sRegistroCompleto", sRegistro);
                                EnviarItem(vParametros);
                            }

                        }
                        #endregion



                        //Finalizar inclusão do Registro
                        vParametros.Clear();
                        DataSet dsGerarEentos;
                        Dictionary<String, String> vParametros_Gerar = new Dictionary<string, string>();
                        vParametros.Add("@idEvento", idEvento);
                        dsGerarEentos = BD.ExecutarDataSet("sp_HUB_Gerar_Eventos_Ocorrencia", vParametros);

                    }
                    

                    sGravarXML(idEvento, sNomeArquivo, sxMLEntrada);


                    //text/html; charset=UTF-8
                    //application/json

                    //Retorno para o Equipamento
                    Response.ContentType = "text/html";
                    Response.Write(GerarRetorno(idEquipamento));

                }



            }
            catch (Exception ex)
            {
                sxMLEntrada = "Erro ao recuperar parametros";
                Response.Write(ex.ToString());

            }

            finally
            {
                //Valida se tem registro para Enviar para segware
                TT_Hub.API.Segware Segware = new API.Segware();
                Segware.Enviar();
            }


            Response.End();
            Response.Close();


        }

        void sGravarXML(string idEvento, string sNomeArquivo, string sXML)
        {
            try
            {
                string sSalvarXMLDisco = IDENTITY.CarregarParametros("sSalvarXMLDisco");

                if (idEvento == "0")
                {
                    sSalvarXMLDisco = "S";
                }

                if (sSalvarXMLDisco == "S")
                {
                    //Gravando XML Arquivo em Disco
                    string sPathSalvarArquivo = HttpContext.Current.Server.MapPath("~/Arquivos/" + sNomeArquivo);
                    StreamWriter sArquivograva = new StreamWriter(sPathSalvarArquivo, true);
                    sArquivograva.Write(sXML);
                    sArquivograva.Close();
                }
            }
            catch
            {

            }


            //GravarXML
            try
            {
                string sImportarXML = IDENTITY.CarregarParametros("ImportarXML");

                if (sImportarXML == "S")
                {
                    DataSet gsGravarXML;
                    Dictionary<String, String> vParametros_XML = new Dictionary<string, string>();
                    vParametros_XML.Add("@sFuncao", "INCLUIR_XML");
                    vParametros_XML.Add("@idEvento", idEvento);
                    vParametros_XML.Add("@sXML", sXML.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", ""));
                    gsGravarXML = BD.ExecutarDataSet("sp_HUB_Manipula_tbl_Eventos", vParametros_XML);
                }
            }
            catch
            {

            }


        }

        string GerarRetorno(string idEquipamento)
        {

            string sRetorno = "";
            DataSet dsRetorno;


            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "RETORNO");
            vParametros.Add("@idEquipamento", idEquipamento);
            dsRetorno = BD.ExecutarDataSet("sp_HUB_Manipula_tbl_Eventos_Ocorrencia_Acao", vParametros);
            if (BD.ValidarDataSet(dsRetorno))
            {
                sRetorno = "set ";
                for (int nLinha = 0; nLinha < dsRetorno.Tables[0].Rows.Count; nLinha++)
                {
                    sRetorno += RETORNO.DATASET(dsRetorno, nLinha, "sValorRetorno").Trim();
                    if (nLinha < dsRetorno.Tables[0].Rows.Count - 1)
                    {
                        sRetorno += "&";
                    }
                }
            }

            sRetorno = sRetorno + "\r\n";
            return sRetorno;
        }

        void EnviarItem(Dictionary<String, String> Parametros)
        {
            DataSet dsIncluir = BD.ExecutarDataSet("sp_HUB_Manipula_tbl_Eventos_Itens", Parametros);
        }
    }
}