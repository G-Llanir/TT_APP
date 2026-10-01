using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BD = TT.FrameWork.BD;
using System.Data;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using TT_Hub.FrameWork;
using System.Security.Claims;
using static TT.FrameWork.BD;
using RETORNO = TT.FrameWork.BD.Retorno;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;
using System.Text.RegularExpressions;
using System.Drawing;

namespace TT_Hub.T_Box
{
    public partial class Monitor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sStringRecebida = "";
            string sTamanhoPost = "";
            string sIP = "";
            try
            {
                JavaScriptSerializer sJSON_Recebido = new System.Web.Script.Serialization.JavaScriptSerializer();
                string sID = "";
                string sAcao = "";
                string sNomeArquivo = "";
                
                string sTamanhoArquivo = "";

                string idEquipamento = "";
                string idEvento = "0";
                string sErro = "";
                bool bEquipamentoTeste = false;

                sIP = Request.ServerVariables["REMOTE_ADDR"].ToString();
                sStringRecebida = System.Web.HttpUtility.UrlDecode(Request.Form.ToString());
                sStringRecebida = new StreamReader(this.Request.InputStream).ReadToEnd();
                sTamanhoArquivo = this.Request.InputStream.Length.ToString();
                sTamanhoPost = this.Request.ServerVariables["CONTENT_LENGTH"].ToString();
                //Nome do Arquivo




                dynamic sJS_Array = sJSON_Recebido.DeserializeObject(sStringRecebida);

                if (sStringRecebida.IndexOf("mac.painel") > 0)
                {
                    //T-BOX
                    foreach (KeyValuePair<string, object> _Entradas in sJS_Array)
                    {
                        string valor = _Entradas.Key;
                        string value1 = _Entradas.Value.ToString();


                    }

                    RegistraEquipamentoTeste(sStringRecebida, sIP, sTamanhoPost, true);
                    bEquipamentoTeste = true;

                }
                else if (sStringRecebida.IndexOf("ID") == -1 && sStringRecebida.IndexOf("acao") == -1) //Testes Weverton
                {
                    RegistraEquipamentoTeste(sStringRecebida, sIP, sTamanhoPost, true);
                    bEquipamentoTeste = true;
                }
                else
                {
                    //T-RMS
                    foreach (KeyValuePair<string, object> _Entradas in sJS_Array)
                    {
                        string sChavePrincipal = _Entradas.Key;
                        foreach (KeyValuePair<string, object> _Elementos in (dynamic)_Entradas.Value)
                        {

                            
                            string valor = _Elementos.Key;
                            string value1 = _Elementos.Value.ToString();

                            if (Regex.IsMatch(valor, "[AD]I[0-9]?") || Regex.IsMatch(valor, "R[0-9]?"))
                            {


                                foreach (KeyValuePair<string, object> Keys in (dynamic)_Elementos.Value)
                                {
                                    var key = Keys.Key;
                                    var value = Keys.Value;
                                    if (Keys.Key == "Val")
                                    {
                                        Gravar_Itens(idEvento, valor, Keys.Value.ToString().Replace(",", "."));
                                    }
                                }
                            }
                            else if (_Entradas.Key == "AIAC")
                            {
                                if (_Elementos.Key == "Val")
                                {
                                    Gravar_Itens(idEvento, _Entradas.Key, _Elementos.Value.ToString().Replace(",", "."));
                                }
            

                                
                            }
                            else
                            {
                                if (_Elementos.Key == "ID")
                                {
                                    sID = _Elementos.Value.ToString();
                                }
                                else if (_Elementos.Key == "acao")
                                {
                                    sAcao = _Elementos.Value.ToString();
                                }



                                if (idEvento == "0" && sAcao != "")
                                {
                                    DataSet dsPesquisa;
                                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                                    vParametros.Add("@sFuncao", "INCLUIR");
                                    vParametros.Add("@sID", sID);
                                    vParametros.Add("@sIPSolicitacao", sIP);
                                    vParametros.Add("@sAcao", sAcao);
                                    vParametros.Add("@sTamanhoArquivo", sTamanhoArquivo);
                                    vParametros.Add("@sTamanhoPost", sTamanhoPost);
                                    dsPesquisa = BD.ExecutarDataSet("sp_HUB_Manipula_tbl_Eventos", vParametros);

                                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                                    {
                                        idEvento = BD.Retorno.DATASET(dsPesquisa, 0, "idEvento");
                                        idEquipamento = BD.Retorno.DATASET(dsPesquisa, 0, "idEquipamento");

                                    }
                                    sNomeArquivo = "TRMS_" + sID.Replace(":", "") + "_" + FUNCOES.CarimboDataHora() + Guid.NewGuid() + ".json";
                                    //Gravar Json no Baanco
                                    sGravarXML(idEvento, sNomeArquivo, sStringRecebida);


                                    //Insere base de teste
                                    if (idEvento == "0")
                                    {
                                        RegistraEquipamentoTeste(sStringRecebida, sIP, sTamanhoPost, true);
                                        bEquipamentoTeste = true;
                                        break;

                                    }
                                }

                            }
                        }

                        if (bEquipamentoTeste)
                        {
                            break;
                        }
                    }

                }



                if (idEvento != "0")
                {
                    //Finalizar inclusão do Registro

                    Dictionary<String, String> vParametros_Gerar = new Dictionary<string, string>();
                    vParametros_Gerar.Add("@idEvento", idEvento);
                    BD.ExecutarDataSet("sp_HUB_Gerar_Eventos_Ocorrencia", vParametros_Gerar);

                    //Gravar Json no Baanco
                    sGravarXML(idEvento, sNomeArquivo, sStringRecebida);


                    //text/html; charset=UTF-8
                    //application/json

                    //Retorno para o Equipamento
                    Response.ContentType = "text/html";
                    Response.Write(GerarRetorno(idEquipamento, sID));
                }



            

            }
            catch (Exception ex)
            {
                RegistraEquipamentoTeste("Erro: " + ex.Message + " - " + sStringRecebida, sIP, sTamanhoPost, false);
                Response.Write(Server.HtmlDecode("Erro: " + ex.Message));
            }



            Response.ContentType = "text/html";
            Response.End();
            Response.Close();

        }

        void Gravar_Itens(string idEvento, string sParametro, string sValor)
        {
            //Insere os valores

            Dictionary<String, String> vParametros = new Dictionary<string, string>();


            vParametros.Add("@idEvento", idEvento);
            vParametros.Add("@sParametro", sParametro);
            vParametros.Add("@sValor", sValor);
            vParametros.Add("@sRegistroCompleto", "");
            BD.ExecutarDataSet("sp_HUB_Manipula_tbl_Eventos_Itens", vParametros);
        }
            
        void RegistraEquipamentoTeste(string sStringRecebida, string sIP, string sTamanhoPost, bool bPrintaMensagem)
        {
            Dictionary<String, String> vParametros1 = new Dictionary<string, string>();
            vParametros1.Add("@sFuncao", "INCLUIR");
            vParametros1.Add("@sStringRecebida", sStringRecebida);
            vParametros1.Add("@sIP", sIP);
            vParametros1.Add("@sTamanhoPost", sTamanhoPost);

            DataSet dsRegistro = BD.ExecutarDataSet("sp_Manipula_HUB_Teste_TBox", vParametros1);
            if (bPrintaMensagem)
            {
                Response.Write(Server.HtmlDecode(BD.Retorno.DATASET(dsRegistro, 0, 0, "sRetorno")));
            }
        }

        string GerarRetorno(string idEquipamento, string sID)
        {

            string sRetorno = "";   
            DataSet dsRetorno;


            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "RETORNO");
            vParametros.Add("@idEquipamento", idEquipamento);
            dsRetorno = BD.ExecutarDataSet("sp_HUB_Manipula_tbl_Eventos_Ocorrencia_Acao", vParametros);
            if (BD.ValidarDataSet(dsRetorno))
            {

                if (RETORNO.DATASET(dsRetorno, 0, "sValorRetorno").Contains("confirma"))
                {
                    sRetorno = "{\"Device\":{\"ID\":\"" + sID + "\",\"acao\":\"confirma\"}}";
                }
                else if (RETORNO.DATASET(dsRetorno, 0, "sValorRetorno").Contains("Confirma"))
                {
                    sRetorno = "{\"Device\":{\"ID\":\"" + sID + "\",\"acao\":\"Confirma\"}}";
                }
                else
                {
                    sRetorno = "{\"Device\":{\"ID\":\"" + sID + "\",\"acao\":\"executar\"},\"RL\":{";
                    //sRetorno = "{\"Device\":{\"acao\":\"executar\"},\"RL\":{";
                    for (int nLinha = 0; nLinha < dsRetorno.Tables[0].Rows.Count; nLinha++)
                    {
                        sRetorno += RETORNO.DATASET(dsRetorno, nLinha, "sValorRetorno").Trim();
                        if (nLinha < dsRetorno.Tables[0].Rows.Count - 1)
                        {
                            sRetorno += ",";
                        }

                    }
                    sRetorno += "}}";
                }
            }
            return sRetorno;
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

        }
    }
}