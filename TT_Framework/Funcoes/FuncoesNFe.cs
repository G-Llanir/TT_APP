using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json.Linq;
using NFe.Danfe.PdfClown;
using NFe.Danfe.PdfClown.Modelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using TT.Framework.Schemas.NFS;
using static TT.Framework.Schemas.NFS.CancelamentoNFS;
using static TT.Framework.Schemas.NFS.EnvioNFS;
using static TT.FrameWork.BD;
using static TT.FrameWork.BD.Retorno;
using static TT.FrameWork.Funcoes;
using Document = iTextSharp.text.Document;

namespace TT.FrameWork
{
    public class Funcoes_NFe
    {
        public class XML
        {
            public static string EnviarXML_SEFAZ(string idXML, out string nNF)
            {
                nNF = "";
                string sRetorno = "";
                string sCaminho = "";

                Dictionary<string, string> vParametrosXML = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE_ENVIO_SEFAZ" },
                    { "@idXML", idXML }
                };
                DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametrosXML);

                if (ValidarDataSet(ds))
                {
                    try
                    {
                        sCaminho = HttpContext.Current.Server.MapPath("~/Download/" + string.Format("{0}_TEMP.xml", DATASET(ds, "chNFe")));

                        XmlDocument XML_NFe = new XmlDocument();
                        XML_NFe.LoadXml(DATASET(ds, "sXML"));

                        XmlNamespaceManager nsManager = new XmlNamespaceManager(XML_NFe.NameTable);
                        nsManager.AddNamespace("nfe", "http://www.portalfiscal.inf.br/nfe");

                        XmlNode infNFe = XML_NFe.SelectSingleNode("//nfe:infNFe", nsManager);
                        XmlNode ide = XML_NFe.SelectSingleNode("//nfe:infNFe/nfe:ide", nsManager);
                        XmlNode emit = XML_NFe.SelectSingleNode("//nfe:infNFe/nfe:emit", nsManager);

                        string cUF = ide["cUF"].InnerText;
                        string cNF = new Random().Next(10000000, 99999999).ToString();
                        string mod = ide["mod"].InnerText;
                        string serie = DATASET(ds, 1, 0, "sSerieNFe").ToString();
                        nNF = DATASET(ds, 1, 0, "sNumeroNFE");
                        string tpEmis = ide["tpEmis"].InnerText;
                        string cnpj = emit["CNPJ"].InnerText;
                        string anoMesEmissao = DateTime.Now.ToString("yyMM");
                        string chaveAcesso = XML_GerarNFE.GerarChaveAcesso(cUF, anoMesEmissao, cnpj, mod, serie, nNF, tpEmis, cNF);

                        // Atualiza os Valores
                        ide["cNF"].InnerText = cNF;
                        ide["serie"].InnerText = serie;
                        ide["nNF"].InnerText = nNF;
                        ide["dhEmi"].InnerText = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                        //ide["dhSaiEnt"].InnerText = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                        ide["cDV"].InnerText = chaveAcesso.Substring(46, 1);
                        infNFe.Attributes[0].Value = chaveAcesso;

                        // Salvar no Diretório de Envio
                        string sPath_Salvar = Path.Combine(DATASET(ds, 2, 0, "sCaminho_UniNFE").ToString(), cnpj, "envio");
                        if (!Directory.Exists(sPath_Salvar))
                            Directory.CreateDirectory(sPath_Salvar);
                        XML_NFe.Save(Path.Combine(sPath_Salvar, chaveAcesso.Replace("NFe", "") + "-nfe" + ".xml"));


                        // Atualiza Arquivo XML
                        Dictionary<string, string> vParametrosXMLAtualizar = new Dictionary<string, string>
                        {
                            { "@sFuncao", "GERAR_XML_SEFAZ" },
                            { "@idXML", idXML },
                            { "@sXML", XML_NFe.InnerXml },
                            { "@nNumeroNF", nNF },
                            { "@sChaveNFe",chaveAcesso.Replace("NFe", "") }
                        };
                        ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametrosXMLAtualizar);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Erro ao gerar Arquivo: " + ex.Message);
                    }
                }

                return sRetorno;
            }

            public static string GerarArquivo(string idXML, string sChaveNFe)
            {
                string sCaminho = "";
                Dictionary<string, string> vParametrosXML = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idXML", idXML },
                    { "@sChaveNFe", sChaveNFe }


                };
                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametrosXML);

                if (BD.ValidarDataSet(ds))
                {
                    try
                    {
                        sCaminho = HttpContext.Current.Server.MapPath("~/Download/" + string.Format("{0}_TEMP.xml", Retorno.DATASET(ds, 0, "chNFe")));

                        File.WriteAllText(sCaminho, Retorno.DATASET(ds, 0, "sXML"));
                    }
                    catch (Exception ex)
                    {

                    }
                }

                return sCaminho;
            }

            public static string Excluir(string idOrigem, string idXML)
            {
                string sRetorno = "";
                try
                {
                    Dictionary<string, string> vParametrosXML = new Dictionary<string, string>
                {
                    { "@sFuncao", "EXCLUIR_XML" },
                    { "@idXML", idXML },
                    { "@idOrigem", idOrigem }
                };
                    DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametrosXML);

                    if (BD.ValidarDataSet(ds))
                    {
                        sRetorno = Retorno.DATASET(ds, "msg");
                    }
                }
                catch (Exception ex)
                {
                    sRetorno = "Erro ao Excluir: " + ex.Message;
                }
                return sRetorno;
            }

            public static string SEFAZ_Cancelar(string idXML, string sJustificativa)
            {
                string sRetorno = "";

                string CNPJ = "";
                string cOrgao = "";
                string tpAmb = "";
                string nProt = "";
                string sChaveNFe = "";

                try
                {

                    Dictionary<string, string> vParametrosXML = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idXML", idXML }
                    };
                    DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametrosXML);
                    if (Retorno.DATASET(ds, "sEspelho") == "N")
                    {
                        sChaveNFe = Retorno.DATASET(ds, 0, "chNFe");
                        XmlDocument XML_NFe = new XmlDocument();
                        XML_NFe.LoadXml(Retorno.DATASET(ds, 0, "sXML"));


                        XmlNamespaceManager nsManager = new XmlNamespaceManager(XML_NFe.NameTable);
                        nsManager.AddNamespace("nfe", "http://www.portalfiscal.inf.br/nfe");

                        XmlNode infNFe = XML_NFe.SelectSingleNode("//nfe:infNFe", nsManager);
                        XmlNode ide = XML_NFe.SelectSingleNode("//nfe:infNFe/nfe:ide", nsManager);
                        XmlNode emit = XML_NFe.SelectSingleNode("//nfe:infNFe/nfe:emit", nsManager);

                        CNPJ = emit["CNPJ"].InnerText;
                        cOrgao = ide["cUF"].InnerText;
                        tpAmb = ide["tpAmb"].InnerText;
                        nProt = Retorno.DATASET(ds, "snProt");

                        string sPath_Salvar = Path.Combine(Retorno.DATASET(ds, "sCaminho_UniNFE").ToString(), CNPJ, "envio");
                        if (!Directory.Exists(sPath_Salvar))
                        {
                            Directory.CreateDirectory(sPath_Salvar);
                        }
                        sPath_Salvar = Path.Combine(sPath_Salvar, sChaveNFe + -110111 + "-" + "01" + "-ped-eve.xml");

                        XmlWriterSettings settings = new XmlWriterSettings
                        {
                            Indent = true,
                            Encoding = System.Text.Encoding.UTF8
                        };
                        using (XmlWriter writer = XmlWriter.Create(sPath_Salvar, settings))
                        {
                            writer.WriteStartDocument();
                            writer.WriteStartElement("envEvento", "http://www.portalfiscal.inf.br/nfe");
                            writer.WriteAttributeString("versao", "1.00");
                            writer.WriteElementString("idLote", "000000000000001");

                            writer.WriteStartElement("evento");
                            writer.WriteAttributeString("versao", "1.00");
                            writer.WriteStartElement("infEvento");
                            writer.WriteAttributeString("Id", "ID110111" + sChaveNFe + "01");

                            writer.WriteElementString("cOrgao", cOrgao);
                            writer.WriteElementString("tpAmb", tpAmb);
                            writer.WriteElementString("CNPJ", CNPJ);
                            writer.WriteElementString("chNFe", sChaveNFe);
                            writer.WriteElementString("dhEvento", DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                            writer.WriteElementString("tpEvento", "110111");
                            writer.WriteElementString("nSeqEvento", "1");
                            writer.WriteElementString("verEvento", "1.00");

                            writer.WriteStartElement("detEvento");
                            writer.WriteAttributeString("versao", "1.00");

                            writer.WriteElementString("descEvento", "Cancelamento");
                            writer.WriteElementString("nProt", nProt);
                            writer.WriteElementString("xJust", HttpUtility.HtmlDecode(sJustificativa.TrimEnd()));
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }

                        //DataSet dsPesquisa;
                        //Dictionary<String, String> vParametros = new Dictionary<string, string>();
                        //vParametros.Add("@sFuncao", "Inativar_ContasReceber");
                        //vParametros.Add("@idEnvioOPI", hddidEnvioOPI.Value);
                        //dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);


                        Dictionary<String, String> vParametros_CCe = new Dictionary<string, string>();
                        vParametros_CCe.Add("@sFuncao", "REGISTRA_CANCELAMENTO");
                        vParametros_CCe.Add("@sObservacao", HttpUtility.HtmlDecode(sJustificativa.TrimEnd()));
                        vParametros_CCe.Add("@sChaveNFe", sChaveNFe);
                        BD.ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametros_CCe);
                    }
                    else
                    {
                        throw new Exception("NFe não foi autorizada, impossivel cancelar!");
                    }

                }
                catch (Exception ex)
                {

                    sRetorno = "Erro ao gerar Cancelamento: " + ex.Message;
                }

                return sRetorno;

            }
        }

        public class XML_GerarNFE
        {
            static int nTabela_Dados = 0;
            static int nTabela_CNPJ_Emitente = 1;
            static int nTabela_CNPJ_Destinatario = 2;
            static int nTabela_Mod_Frete = 3;
            static int nTabela_Dados_NFe = 4;
            static int nTabela_Dados_Emitente = 5;
            static int nTabela_IE_Emitente = 6;
            static int nTabela_Dados_Destinatario = 7;
            static int nTablea_IE_Destinatario = 8;
            static int nTabela_Dados_Desembarque = 9;
            static int nTabela_Fabricante = 10;
            static int nTabela_xPedido = 11;
            static int nTabela_Total_Tributo = 12;
            static int nTabela_ICMS = 13;
            static int nTabela_cEnq = 14;
            static int nTabela_IPI = 15;
            static int nTabela_II = 16;
            static int nTabela_PIS = 17;
            static int nTabela_COFINS = 18;
            static int nTabela_infAdicionais = 19;
            static int nTabela_Totais = 20;
            static int nTabela_Volumes = 21;
            static int nTabela_Pagamento = 22;
            static int nTabela_infAdicionais_Fisco = 23;
            static int nTabela_Contato = 24;
            static int nTabela_UF = 25;
            static int nTabela_Produtos = 26;
            static int nTabela_nNumeroNFE = 27;
            static int nTabela_Volumes_2 = 28;
            static int nTabela_Transportadora = 29;
            static int nTabela_xPed = 30;
            static int nTabela_ICMS_2 = 31;

            static int nTabela_IBS = 32;
            static int nTabela_gIBSUF = 33;
            static int nTabela_gIBSMun = 34;
            static int nTabela_gCBS = 35;

            static int nTabela_Total_gIBS = 36;
            static int nTabela_Total_gIBSUF = 37;
            static int nTabela_Total_gIBSMun = 38;
            static int nTabela_Total_gCBS = 39;
            static int nTabela_Exporta = 40;

            public static string GerarXML(string idPedido, string NumeroPedido, Page Page, string idEmpresa, bool ProdutoCliente, string idEnvioOPI, string sFabricante, string sUsarProdutosCliente, bool Devolucao, string ChaveDevolucao, string Itens, string Remessa, bool bGerarEspelho, DateTime dtEntrada_Saida = default)
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTA_XML" },
                    { "@idPedido", idPedido },
                    { "@idEmpresa", idEmpresa },
                    { "@idEnvioOPI", idEnvioOPI },
                    { "@sFabricante", sFabricante },
                    { "@sGerarPiloto", bGerarEspelho ? "S" : "N" },
                    { "@sUsarProdutosCliente", sUsarProdutosCliente },
                    { "@Itens", Itens }
                };
                DataSet dsPesquisa = ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametros);

                string filePath;
                if (idEnvioOPI == "0")
                    filePath = "pedido" + NumeroPedido + ".xml";
                else
                    filePath = "pedido" + NumeroPedido + "-" + "Envio" + idEnvioOPI + ".xml";

                string arquivoFinal = AppContext.BaseDirectory + "Download\\" + filePath;
                string nPedido = int.Parse(NumeroPedido).ToString();
                EstruturaXML(dsPesquisa, arquivoFinal, nPedido, ProdutoCliente, idEnvioOPI, Devolucao, ChaveDevolucao, Remessa, bGerarEspelho, dtEntrada_Saida);

                if ((!bGerarEspelho) && (idEnvioOPI != "0" || idPedido != "0"))
                {
                    int nNFe = int.Parse(dsPesquisa.Tables[nTabela_nNumeroNFE].Rows[0]["nNFe"].ToString()) + 1;
                    Dictionary<string, string> vParametrosAtualiza = new Dictionary<string, string>
                    {
                        { "@sFuncao", "AtualizarNFe" },
                        { "@idEmpresa", idEmpresa },
                        { "@nNFe", nNFe.ToString() }
                    };
                    ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametrosAtualiza);
                }

                return filePath;
            }

            public static void EstruturaXML(DataSet dataSet, string filePath, string NumeroPedido, bool ProdutoCliente, string idEnvioOPI, bool Devolucao, string ChaveDevolucao, string Remessa, bool bGerarPiloto, DateTime dtEntrada_Saida = default)
            {
                string cUF = "";
                string cnpj = "";

                if (dataSet.Tables[nTabela_UF].Rows.Count == 0)
                    throw new Exception("A empresa pode não ter um parceiro cadastrado ou, caso tenha, o parceiro pode não ter um endereço principal associado.");

                if (dataSet.Tables[nTabela_UF].Rows[0]["UF"].ToString() == "")
                    throw new Exception("O estado do endereço principal do parceiro da empresa cadastrada não foi encontrado.");
                else
                    cUF = dataSet.Tables[nTabela_UF].Rows[0]["UF"].ToString(); // Código da UF (São Paulo)

                if (dataSet.Tables[nTabela_CNPJ_Emitente].Rows[0]["CNPJ"].ToString().Length == 14)
                    cnpj = dataSet.Tables[nTabela_CNPJ_Emitente].Rows[0]["CNPJ"].ToString();
                else
                    throw new Exception("O CNPJ do parceiro da empresa cadastrada não foi encontrado ou está inválido. Certifique-se de que o CNPJ está no formato correto (14 dígitos).");

                string mod = dataSet.Tables[nTabela_nNumeroNFE].Rows[0]["mod"].ToString();
                string serie = dataSet.Tables[nTabela_nNumeroNFE].Rows[0]["serie"].ToString();
                string nNF = dataSet.Tables[nTabela_nNumeroNFE].Rows[0]["nNFe"].ToString();
                string tpAmb = dataSet.Tables[nTabela_nNumeroNFE].Rows[0]["tpAmb"].ToString();
                string tpEmis = "1";
                string cNF = new Random().Next(10000000, 99999999).ToString();
                string indFinal = dataSet.Tables[nTablea_IE_Destinatario].Rows[0]["indIEDest"].ToString() == "9" ? "1" : "0";
                string anoMesEmissao = DateTime.Now.ToString("yyMM");
                string chaveAcesso = GerarChaveAcesso(cUF, anoMesEmissao, cnpj, mod, serie, nNF, tpEmis, cNF);

                string idDest;
                if (dataSet.Tables[nTabela_Dados_Destinatario].Rows.Count > 0)
                {
                    if (dataSet.Tables[nTabela_Dados_Emitente].Rows[0]["xPais"].ToString() == dataSet.Tables[nTabela_Dados_Destinatario].Rows[0]["xPais"].ToString())
                    {
                        if (dataSet.Tables[nTabela_Dados_Emitente].Rows[0]["UF"].ToString() == dataSet.Tables[nTabela_Dados_Destinatario].Rows[0]["UF"].ToString())
                            idDest = "1";
                        else
                            idDest = "2";
                    }
                    else
                        idDest = "3";
                }
                else
                    idDest = "1";

                XmlWriterSettings settings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 };
                using (XmlWriter writer = XmlWriter.Create(filePath, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("NFe", "http://www.portalfiscal.inf.br/nfe");
                    writer.WriteStartElement("infNFe");
                    writer.WriteAttributeString("Id", chaveAcesso);
                    writer.WriteAttributeString("versao", "4.00");
                    writer.WriteStartElement("ide");
                    writer.WriteElementString("cUF", dataSet.Tables[nTabela_UF].Rows[0]["UF"].ToString());
                    writer.WriteElementString("cNF", cNF);

                    string natOp = "";
                    if (Remessa == "S")
                        natOp = "Remessa";
                    else if (Remessa == "EMISSAO")
                        natOp = DATASET(dataSet, 30, 0, "sNaturezaOperacao");
                    else
                    {
                        if (Devolucao == false)
                            natOp = "Venda";
                        else
                            natOp = "Devolução";
                    }

                    writer.WriteElementString("natOp", natOp);

                    writer.WriteElementString("mod", mod);
                    writer.WriteElementString("serie", serie);
                    writer.WriteElementString("nNF", nNF);
                    writer.WriteElementString("dhEmi", DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));

                    if (dtEntrada_Saida != default)
                        writer.WriteElementString("dhSaiEnt", dtEntrada_Saida.ToString("yyyy-MM-ddTHH:mm:sszzz"));

                    writer.WriteElementString("tpNF", dataSet.Tables[nTabela_nNumeroNFE].Rows[0]["tpNF"].ToString());
                    writer.WriteElementString("idDest", idDest);
                    writer.WriteElementString("cMunFG", dataSet.Tables[nTabela_Dados_Emitente].Rows[0]["cMun"].ToString());
                    writer.WriteElementString("tpImp", "1");
                    writer.WriteElementString("tpEmis", tpEmis);
                    writer.WriteElementString("cDV", chaveAcesso[chaveAcesso.Length - 1].ToString());
                    writer.WriteElementString("tpAmb", tpAmb);
                    writer.WriteElementString("finNFe", DATASET(dataSet, nTabela_nNumeroNFE, 0, "finNFe"));
                    writer.WriteElementString("indFinal", indFinal);
                    writer.WriteElementString("indPres", "0");
                    writer.WriteElementString("procEmi", "0");
                    writer.WriteElementString("verProc", "4.00");

                    if (Devolucao == true)
                    {
                        writer.WriteStartElement("NFref");
                        writer.WriteElementString("refNFe", ChaveDevolucao);
                        writer.WriteEndElement();
                    }

                    if (Remessa == "EMISSAO")
                    {
                        if (dataSet.Tables[nTabela_xPed].Rows[0]["refNFe"].ToString() != "")
                        {
                            writer.WriteStartElement("NFref");
                            writer.WriteElementString("refNFe", dataSet.Tables[nTabela_xPed].Rows[0]["refNFe"].ToString());
                            writer.WriteEndElement();
                        }
                    }

                    writer.WriteEndElement();

                    if (dataSet.Tables.Count > 1)
                    {
                        writer.WriteStartElement("emit");
                        PopularEstrutura(writer, dataSet.Tables[nTabela_CNPJ_Emitente]);

                        writer.WriteStartElement("enderEmit");
                        PopularEstrutura(writer, dataSet.Tables[nTabela_Dados_Emitente]);
                        writer.WriteEndElement();

                        PopularEstrutura(writer, dataSet.Tables[nTabela_IE_Emitente]);
                        writer.WriteEndElement();
                    }

                    if (dataSet.Tables.Count > 2)
                    {
                        writer.WriteStartElement("dest");
                        if (dataSet.Tables[nTabela_CNPJ_Destinatario].Rows[0]["CNPJ"].ToString().Length == 11)
                            dataSet.Tables[nTabela_CNPJ_Destinatario].Columns["CNPJ"].ColumnName = "CPF";
                        else if (dataSet.Tables[nTabela_Dados_Destinatario].Rows[0]["UF"].ToString() == "EX")
                        {
                            dataSet.Tables[nTabela_CNPJ_Destinatario].Columns["CNPJ"].ColumnName = "idEstrangeiro";
                            dataSet.Tables[nTabela_Dados_Destinatario].Columns.Remove("CEP");
                        }

                        PopularEstrutura(writer, dataSet.Tables[nTabela_CNPJ_Destinatario]);

                        if (dataSet.Tables[nTabela_Dados_Destinatario].Rows.Count > 0)
                        {
                            writer.WriteStartElement("enderDest");
                            if (dataSet.Tables[nTabela_Dados_Destinatario].Rows[0]["xCpl"].ToString() == "" || string.IsNullOrEmpty(dataSet.Tables[nTabela_Dados_Destinatario].Rows[0]["xCpl"].ToString()))
                                dataSet.Tables[nTabela_Dados_Destinatario].Columns.Remove("xCpl");

                            PopularEstrutura(writer, dataSet.Tables[nTabela_Dados_Destinatario]);

                            writer.WriteEndElement();
                        }

                        if (dataSet.Tables[nTablea_IE_Destinatario].Rows[0]["IE"].ToString() == "" || string.IsNullOrEmpty(dataSet.Tables[nTablea_IE_Destinatario].Rows[0]["IE"].ToString()) || dataSet.Tables[nTablea_IE_Destinatario].Rows[0]["IE"].ToString().ToUpper() == "ISENTO")
                            dataSet.Tables[nTablea_IE_Destinatario].Columns.Remove("IE");

                        if (dataSet.Tables[nTablea_IE_Destinatario].Rows[0]["ISUF"].ToString() == "")
                            dataSet.Tables[nTablea_IE_Destinatario].Columns.Remove("ISUF");

                        PopularEstrutura(writer, dataSet.Tables[nTablea_IE_Destinatario]);

                        writer.WriteEndElement();
                    }

                    int coluna = 0;
                    for (int i = 0; i < dataSet.Tables[nTabela_Dados].Rows.Count; i++)
                    {
                        string id = "";

                        writer.WriteStartElement("det");
                        writer.WriteAttributeString("nItem", (i + 1).ToString());
                        writer.WriteStartElement("prod");

                        if (ProdutoCliente)
                            PopularEstruturaIndex(writer, dataSet.Tables[nTabela_Produtos], i);
                        else
                        {
                            if (coluna == 0)
                            {
                                if (dataSet.Tables[nTabela_Dados].Rows[0]["vFrete"].ToString() == "0.00" || string.IsNullOrEmpty(dataSet.Tables[nTabela_Dados].Rows[0]["vFrete"].ToString()))
                                    dataSet.Tables[nTabela_Dados].Columns.Remove("vFrete");

                                if (coluna == 0)
                                {
                                    dataSet.Tables[nTabela_Dados].Columns.RemoveAt(dataSet.Tables[nTabela_Dados].Columns.Count - 2);
                                    dataSet.Tables[nTabela_Dados].Columns.RemoveAt(dataSet.Tables[nTabela_Dados].Columns.Count - 2);
                                    dataSet.Tables[nTabela_Dados].Columns.RemoveAt(dataSet.Tables[nTabela_Dados].Columns.Count - 2);
                                    dataSet.Tables[nTabela_Dados].Columns.RemoveAt(dataSet.Tables[nTabela_Dados].Columns.Count - 2);
                                }

                                coluna = 1;
                            }

                            PopularEstruturaIndex(writer, dataSet.Tables[nTabela_Dados], i);

                            id = dataSet.Tables[nTabela_Dados].Rows[i]["ID"].ToString();
                        }

                        if (dataSet.Tables[nTabela_Dados_Destinatario].Rows.Count > 0)
                        {
                            if (dataSet.Tables[nTabela_Dados_Emitente].Rows[0]["xPais"].ToString() != dataSet.Tables[nTabela_Dados_Destinatario].Rows[0]["xPais"].ToString())
                            {
                                if (dataSet.Tables[nTabela_Dados_Desembarque].Rows[i]["nDI"].ToString() != "")
                                {
                                    writer.WriteStartElement("DI");
                                    PopularEstruturaIndex(writer, dataSet.Tables[nTabela_Dados_Desembarque], i);
                                    writer.WriteStartElement("adi");
                                    PopularEstruturaIndex(writer, dataSet.Tables[nTabela_Fabricante], i);
                                    writer.WriteEndElement();
                                    writer.WriteEndElement();
                                }
                            }
                        }

                        if (idEnvioOPI != "")
                        {
                            DataRow[] linhasxped = dataSet.Tables[nTabela_xPedido].Select($"ID = '{id}'");
                            DataTable tabelaxped = dataSet.Tables[nTabela_xPedido].Clone();

                            foreach (DataRow linha in linhasxped)
                                tabelaxped.ImportRow(linha);

                            PopularEstrutura(writer, tabelaxped);
                        }

                        writer.WriteEndElement();
                        writer.WriteStartElement("imposto");

                        bool bEmitente_igual_Destinatario = false;
                        if (dataSet.Tables[nTabela_CNPJ_Destinatario].Columns.Contains("CNPJ"))
                        {
                            if (dataSet.Tables[nTabela_CNPJ_Emitente].Rows[0]["CNPJ"].ToString() == dataSet.Tables[nTabela_CNPJ_Destinatario].Rows[0]["CNPJ"].ToString())
                                bEmitente_igual_Destinatario = true;
                        }

                        DataRow[] linhasFiltradas = dataSet.Tables[nTabela_Total_Tributo].Select($"ID = '{id}'");
                        DataTable tabelaFiltrada = dataSet.Tables[nTabela_Total_Tributo].Clone();
                        foreach (DataRow linha in linhasFiltradas)
                            tabelaFiltrada.ImportRow(linha);

                        PopularEstrutura(writer, tabelaFiltrada);

                        writer.WriteStartElement("ICMS");
                        string sICMS = "";
                        foreach (DataRow linha in dataSet.Tables[nTabela_ICMS].Rows)
                        {
                            DataTable dt = dataSet.Tables[nTabela_ICMS].Copy();
                            int contador = 0;
                            while (true)
                            {
                                if (dt.Rows[contador].Field<int>("ID") != int.Parse(id))
                                    dt.Rows.RemoveAt(contador);
                                else
                                    contador++;
                                if (contador >= dt.Rows.Count)
                                    break;
                            }

                            sICMS = dt.Rows[0]["CST"].ToString();
                            if (sICMS == "41")
                                sICMS = "40";

                            writer.WriteStartElement("ICMS" + sICMS);
                            break;
                        }

                        foreach (DataRow linha in dataSet.Tables[nTabela_ICMS].Rows)
                        {
                            DataTable dt = dataSet.Tables[nTabela_ICMS].Copy();
                            int contador = 0;
                            while (true)
                            {
                                if (dt.Rows[contador].Field<int>("ID") != int.Parse(id))
                                    dt.Rows.RemoveAt(contador);
                                else
                                    contador++;
                                if (contador >= dt.Rows.Count)
                                    break;
                            }

                            if (dt.Columns.Contains("pRedBC"))
                            {
                                if (dt.Rows[0]["pRedBC"].ToString() == "0.0000" || string.IsNullOrEmpty(dt.Rows[0]["pRedBC"].ToString()))
                                    dt.Columns.Remove("pRedBC");
                            }

                            if (dt.Columns.Contains("modBCST"))
                            {
                                if (dt.Rows[0]["modBCST"].ToString() == "0" || string.IsNullOrEmpty(dt.Rows[0]["modBCST"].ToString()))
                                    dt.Columns.Remove("modBCST");
                            }

                            if (dt.Columns.Contains("pMVAST"))
                            {
                                if (dt.Rows[0]["pMVAST"].ToString() == "0.00" || string.IsNullOrEmpty(dt.Rows[0]["pMVAST"].ToString()))
                                    dt.Columns.Remove("pMVAST");
                            }

                            if (dt.Columns.Contains("vBCST"))
                            {
                                if (dt.Rows[0]["vBCST"].ToString() == "0.00" || string.IsNullOrEmpty(dt.Rows[0]["vBCST"].ToString()))
                                    dt.Columns.Remove("vBCST");
                            }

                            if (dt.Columns.Contains("pICMSST"))
                            {
                                if (dt.Rows[0]["pICMSST"].ToString() == "0.00" || string.IsNullOrEmpty(dt.Rows[0]["pICMSST"].ToString()))
                                    dt.Columns.Remove("pICMSST");
                            }

                            if (dt.Columns.Contains("vICMSST"))
                            {
                                if (dt.Rows[0]["vICMSST"].ToString() == "0.00" || string.IsNullOrEmpty(dt.Rows[0]["vICMSST"].ToString()))
                                    dt.Columns.Remove("vICMSST");
                            }

                            // Caso o Emitente = Destinatário
                            if (bEmitente_igual_Destinatario || sICMS == "40")
                            {
                                if (dt.Columns.Contains("vBC"))
                                {
                                    if (dt.Rows[0]["vBC"].ToString() == "0.00" || string.IsNullOrEmpty(dt.Rows[0]["vBC"].ToString()))
                                        dt.Columns.Remove("vBC");
                                }

                                if (dt.Columns.Contains("pICMS"))
                                {
                                    if (dt.Rows[0]["pICMS"].ToString() == "0.00" || string.IsNullOrEmpty(dt.Rows[0]["pICMS"].ToString()))
                                        dt.Columns.Remove("pICMS");
                                }

                                if (dt.Columns.Contains("vICMS"))
                                {
                                    if (dt.Rows[0]["vICMS"].ToString() == "0.00" || string.IsNullOrEmpty(dt.Rows[0]["vICMS"].ToString()))
                                        dt.Columns.Remove("vICMS");
                                }

                                if (dt.Columns.Contains("modBC"))
                                {
                                    if (dt.Rows[0]["modBC"].ToString() == "0" || string.IsNullOrEmpty(dt.Rows[0]["modBC"].ToString()))
                                        dt.Columns.Remove("modBC");
                                }
                            }

                            PopularEstrutura(writer, dt);
                            break;
                        }

                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteStartElement("IPI");

                        PopularEstrutura(writer, dataSet.Tables[nTabela_cEnq]);

                        //----------------------IPI--------------------------
                        string sTAG_IPI = DATASET(dataSet, nTabela_IPI, i, "sTAG");
                        if (bEmitente_igual_Destinatario)
                            sTAG_IPI = "IPINT";

                        writer.WriteStartElement(sTAG_IPI);
                        foreach (DataRow linha in dataSet.Tables[nTabela_IPI].Rows)
                        {
                            DataTable dt = dataSet.Tables[nTabela_IPI].Copy();
                            int contador = 0;
                            while (true)
                            {
                                if (dt.Rows[contador].Field<int>("ID") != int.Parse(id))
                                    dt.Rows.RemoveAt(contador);
                                else
                                    contador++;
                                if (contador >= dt.Rows.Count)
                                    break;
                            }

                            dt.Columns.Remove("sTAG");

                            if (sTAG_IPI == "IPINT")
                            {
                                if (dt.Columns.Contains("vBC"))
                                {
                                    if (dt.Rows[0]["vBC"].ToString() == "0.00" || string.IsNullOrEmpty(dt.Rows[0]["vBC"].ToString()))
                                        dt.Columns.Remove("vBC");
                                }

                                if (dt.Columns.Contains("pIPI"))
                                {
                                    if (dt.Rows[0]["pIPI"].ToString() == "0.00" || string.IsNullOrEmpty(dt.Rows[0]["pIPI"].ToString()))
                                        dt.Columns.Remove("pIPI");
                                }

                                if (dt.Columns.Contains("vIPI"))
                                {
                                    if (dt.Rows[0]["vIPI"].ToString() == "0.00" || string.IsNullOrEmpty(dt.Rows[0]["vIPI"].ToString()))
                                        dt.Columns.Remove("vIPI");
                                }
                            }

                            PopularEstrutura(writer, dt);
                            break;
                        }

                        writer.WriteEndElement();
                        writer.WriteEndElement();

                        if (dataSet.Tables[nTabela_Dados_Destinatario].Rows.Count > 0)
                        {
                            if (dataSet.Tables[nTabela_Dados_Emitente].Rows[0]["xPais"].ToString() != dataSet.Tables[nTabela_Dados_Destinatario].Rows[0]["xPais"].ToString())
                            {
                                writer.WriteStartElement("II");
                                PopularEstruturaIndex(writer, dataSet.Tables[nTabela_II], i);
                                writer.WriteEndElement();
                            }
                        }

                        //----------------------PIS------------------
                        writer.WriteStartElement("PIS");

                        string sTAG_PIS = DATASET(dataSet, nTabela_PIS, i, "sTAG");
                        writer.WriteStartElement(sTAG_PIS);

                        foreach (DataRow linha in dataSet.Tables[nTabela_PIS].Rows)
                        {
                            DataTable dt = dataSet.Tables[nTabela_PIS].Copy();
                            int contador = 0;
                            while (true)
                            {
                                if (dt.Rows[contador].Field<int>("ID") != int.Parse(id))
                                    dt.Rows.RemoveAt(contador);
                                else
                                    contador++;
                                if (contador >= dt.Rows.Count)
                                    break;
                            }

                            dt.Columns.Remove("sTAG");

                            if (sTAG_PIS == "PISNT")
                            {
                                if (dt.Columns.Contains("vBC"))
                                {
                                    if (dt.Rows[0]["vBC"].ToString() == "0.00" || string.IsNullOrEmpty(dt.Rows[0]["vBC"].ToString()))
                                        dt.Columns.Remove("vBC");
                                }

                                if (dt.Columns.Contains("pPIS"))
                                {
                                    if (dt.Rows[0]["pPIS"].ToString() == "0.00" || string.IsNullOrEmpty(dt.Rows[0]["pPIS"].ToString()))
                                        dt.Columns.Remove("pPIS");
                                }

                                if (dt.Columns.Contains("vPIS"))
                                {
                                    if (dt.Rows[0]["vPIS"].ToString() == "0.00" || string.IsNullOrEmpty(dt.Rows[0]["vPIS"].ToString()))
                                        dt.Columns.Remove("vPIS");
                                }
                            }

                            PopularEstrutura(writer, dt);
                            break;
                        }

                        writer.WriteEndElement();
                        writer.WriteEndElement();

                        //----------------------COFINS------------------
                        writer.WriteStartElement("COFINS");

                        string sTAG_COFINS = DATASET(dataSet, nTabela_COFINS, i, "sTAG");
                        writer.WriteStartElement(sTAG_COFINS);

                        foreach (DataRow linha in dataSet.Tables[nTabela_COFINS].Rows)
                        {
                            DataTable dt = dataSet.Tables[nTabela_COFINS].Copy();
                            int contador = 0;
                            while (true)
                            {
                                if (dt.Rows[contador].Field<int>("ID") != int.Parse(id))
                                    dt.Rows.RemoveAt(contador);
                                else
                                    contador++;
                                if (contador >= dt.Rows.Count)
                                    break;
                            }
                            dt.Columns.Remove("sTAG");

                            if (sTAG_COFINS == "COFINSNT")
                            {
                                if (dt.Columns.Contains("vBC"))
                                {
                                    if (dt.Rows[0]["vBC"].ToString() == "0.00" || string.IsNullOrEmpty(dt.Rows[0]["vBC"].ToString()))
                                        dt.Columns.Remove("vBC");
                                }

                                if (dt.Columns.Contains("pCOFINS"))
                                {
                                    if (dt.Rows[0]["pCOFINS"].ToString() == "0.00" || string.IsNullOrEmpty(dt.Rows[0]["pCOFINS"].ToString()))
                                        dt.Columns.Remove("pCOFINS");
                                }

                                if (dt.Columns.Contains("vCOFINS"))
                                {
                                    if (dt.Rows[0]["vCOFINS"].ToString() == "0.00" || string.IsNullOrEmpty(dt.Rows[0]["vCOFINS"].ToString()))
                                        dt.Columns.Remove("vCOFINS");
                                }
                            }

                            PopularEstrutura(writer, dt);
                            break;
                        }

                        writer.WriteEndElement();
                        writer.WriteEndElement();

                        if (idEnvioOPI != "0")
                        {
                            if (dataSet.Tables[nTabela_ICMS_2].Rows.Count > 0)
                            {
                                if (dataSet.Tables[nTabela_ICMS_2].Rows[0]["sCalculoDIFAL"].ToString() == "S")
                                {
                                    writer.WriteStartElement("ICMSUFDest");

                                    foreach (DataRow linha in dataSet.Tables[nTabela_ICMS_2].Rows)
                                    {
                                        DataTable dt = dataSet.Tables[nTabela_ICMS_2].Copy();
                                        int contador = 0;
                                        while (true)
                                        {
                                            if (dt.Rows[contador].Field<int>("ID") != int.Parse(id))
                                                dt.Rows.RemoveAt(contador);
                                            else
                                                contador++;
                                            if (contador >= dt.Rows.Count)
                                                break;
                                        }

                                        PopularEstrutura(writer, dt);
                                        break;
                                    }

                                    writer.WriteEndElement();
                                }
                            }
                        }

                        if (dataSet.Tables[nTabela_IBS].Rows.Count > 0)
                        {
                            //----------------------IBS/CBS-------------------
                            writer.WriteStartElement("IBSCBS");
                            writer.WriteElementString("CST", DATASET(dataSet, nTabela_IBS, i, "CST"));
                            writer.WriteElementString("cClassTrib", DATASET(dataSet, nTabela_IBS, i, "cClassTrib"));

                            writer.WriteStartElement("gIBSCBS");
                            writer.WriteElementString("vBC", DATASET(dataSet, nTabela_IBS, i, "vBC"));

                            writer.WriteStartElement("gIBSUF");
                            PopularEstruturaIndex(writer, dataSet.Tables[nTabela_gIBSUF], i);
                            writer.WriteEndElement();

                            writer.WriteStartElement("gIBSMun");
                            PopularEstruturaIndex(writer, dataSet.Tables[nTabela_gIBSMun], i);
                            writer.WriteEndElement();

                            writer.WriteElementString("vIBS", DATASET(dataSet, nTabela_IBS, i, "vIBS"));

                            writer.WriteStartElement("gCBS");
                            PopularEstruturaIndex(writer, dataSet.Tables[nTabela_gCBS], i);
                            writer.WriteEndElement();

                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }

                        // Finalização nDet
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }

                    if (dataSet.Tables.Count > 5)
                    {
                        writer.WriteStartElement("total");
                        writer.WriteStartElement("ICMSTot");
                        PopularEstrutura(writer, dataSet.Tables[nTabela_Totais]);
                        writer.WriteEndElement();

                        if (dataSet.Tables[nTabela_Total_gIBS].Rows.Count > 0)
                        {
                            //----------------------Totalização IBS/CBS---------------------------
                            writer.WriteStartElement("IBSCBSTot");
                            writer.WriteElementString("vBCIBSCBS", DATASET(dataSet, nTabela_Total_gIBS, 0, "vBCIBSCBS"));

                            writer.WriteStartElement("gIBS");
                            writer.WriteStartElement("gIBSUF");
                            PopularEstrutura(writer, dataSet.Tables[nTabela_Total_gIBSUF]);
                            writer.WriteEndElement();

                            writer.WriteStartElement("gIBSMun");
                            PopularEstrutura(writer, dataSet.Tables[nTabela_Total_gIBSMun]);
                            writer.WriteEndElement();

                            writer.WriteElementString("vIBS", DATASET(dataSet, nTabela_Total_gIBS, 0, "vIBS"));
                            writer.WriteElementString("vCredPres", DATASET(dataSet, nTabela_Total_gIBS, 0, "vCredPres"));
                            writer.WriteElementString("vCredPresCondSus", DATASET(dataSet, nTabela_Total_gIBS, 0, "vCredPresCondSus"));
                            writer.WriteEndElement();

                            writer.WriteStartElement("gCBS");
                            PopularEstrutura(writer, dataSet.Tables[nTabela_Total_gCBS]);
                            writer.WriteEndElement();

                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    }

                    if (dataSet.Tables.Count > 3)
                    {
                        writer.WriteStartElement("transp");
                        PopularEstrutura(writer, dataSet.Tables[nTabela_Mod_Frete]);

                        if (dataSet.Tables.Count > 28 && dataSet.Tables[nTabela_Transportadora].Rows.Count > 0)
                        {
                            writer.WriteStartElement("transporta");
                            if (dataSet.Tables[nTabela_Transportadora].Rows[0]["IE"].ToString() == "" || string.IsNullOrEmpty(dataSet.Tables[nTabela_Transportadora].Rows[0]["IE"].ToString()))
                                dataSet.Tables[nTabela_Transportadora].Columns.Remove("IE");

                            if (dataSet.Tables[nTabela_Transportadora].Rows[0]["CNPJ"].ToString() == "" || string.IsNullOrEmpty(dataSet.Tables[nTabela_Transportadora].Rows[0]["CNPJ"].ToString()))
                                dataSet.Tables[nTabela_Transportadora].Columns.Remove("CNPJ");
                            else
                            {
                                if (dataSet.Tables[nTabela_Transportadora].Rows[0]["CNPJ"].ToString().Length == 11)
                                    dataSet.Tables[nTabela_Transportadora].Columns["CNPJ"].ColumnName = "CPF";
                            }

                            if (dataSet.Tables[nTabela_Transportadora].Rows[0]["xNome"].ToString() == "" || string.IsNullOrEmpty(dataSet.Tables[nTabela_Transportadora].Rows[0]["xNome"].ToString()))
                                dataSet.Tables[nTabela_Transportadora].Columns.Remove("xNome");

                            if (dataSet.Tables[nTabela_Transportadora].Rows[0]["xEnder"].ToString() == "" || string.IsNullOrEmpty(dataSet.Tables[nTabela_Transportadora].Rows[0]["xEnder"].ToString()))
                                dataSet.Tables[nTabela_Transportadora].Columns.Remove("xEnder");

                            if (dataSet.Tables[nTabela_Transportadora].Rows[0]["xMun"].ToString() == "" || string.IsNullOrEmpty(dataSet.Tables[nTabela_Transportadora].Rows[0]["xMun"].ToString()))
                                dataSet.Tables[nTabela_Transportadora].Columns.Remove("xMun");

                            if (dataSet.Tables[nTabela_Transportadora].Rows[0]["UF"].ToString() == "" || string.IsNullOrEmpty(dataSet.Tables[nTabela_Transportadora].Rows[0]["UF"].ToString()))
                                dataSet.Tables[nTabela_Transportadora].Columns.Remove("UF");

                            PopularEstrutura(writer, dataSet.Tables[nTabela_Transportadora]);
                            writer.WriteEndElement();
                        }

                        if (dataSet.Tables.Count > nTabela_Volumes_2 && dataSet.Tables[nTabela_Volumes_2].Rows.Count > 0)
                        {
                            if (dataSet.Tables[nTabela_Volumes_2].Rows[0]["qVol"].ToString() != "0")
                            {
                                writer.WriteStartElement("vol");

                                if (dataSet.Tables[nTabela_Volumes_2].Rows[0]["qVol"].ToString() == "" || string.IsNullOrEmpty(dataSet.Tables[nTabela_Volumes_2].Rows[0]["qVol"].ToString()) || dataSet.Tables[nTabela_Volumes_2].Rows[0]["qVol"].ToString() == "0")
                                    dataSet.Tables[nTabela_Volumes_2].Columns.Remove("qVol");

                                if (dataSet.Tables[nTabela_Volumes_2].Rows[0]["esp"].ToString() == "" || string.IsNullOrEmpty(dataSet.Tables[nTabela_Volumes_2].Rows[0]["esp"].ToString()))
                                    dataSet.Tables[nTabela_Volumes_2].Columns.Remove("esp");

                                if (dataSet.Tables[nTabela_Volumes_2].Rows[0]["pesoL"].ToString() == "" || string.IsNullOrEmpty(dataSet.Tables[nTabela_Volumes_2].Rows[0]["pesoL"].ToString()) || dataSet.Tables[28].Rows[0]["pesoL"].ToString() == "0.000")
                                    dataSet.Tables[nTabela_Volumes_2].Columns.Remove("pesoL");

                                if (dataSet.Tables[nTabela_Volumes_2].Rows[0]["pesoB"].ToString() == "" || string.IsNullOrEmpty(dataSet.Tables[nTabela_Volumes_2].Rows[0]["pesoB"].ToString()) || dataSet.Tables[28].Rows[0]["pesoB"].ToString() == "0.000")
                                    dataSet.Tables[nTabela_Volumes_2].Columns.Remove("pesoB");

                                PopularEstrutura(writer, dataSet.Tables[nTabela_Volumes_2]);
                                writer.WriteEndElement();
                            }
                        }

                        writer.WriteEndElement();
                    }

                    if (dataSet.Tables.Count > 6)
                    {
                        writer.WriteStartElement("pag");
                        writer.WriteStartElement("detPag");
                        PopularEstrutura(writer, dataSet.Tables[nTabela_Pagamento]);
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }

                    if (dataSet.Tables[nTabela_infAdicionais_Fisco].Rows.Count > 0)
                    {
                        if (dataSet.Tables[nTabela_infAdicionais_Fisco].Rows[0]["infAdFisco"].ToString() != "" || dataSet.Tables[nTabela_infAdicionais_Fisco].Rows[0]["infCpl"].ToString() != "")
                        {
                            writer.WriteStartElement("infAdic");

                            if (dataSet.Tables[nTabela_infAdicionais_Fisco].Rows[0]["infAdFisco"].ToString() == "" || string.IsNullOrEmpty(dataSet.Tables[nTabela_infAdicionais_Fisco].Rows[0]["infAdFisco"].ToString()))
                                dataSet.Tables[nTabela_infAdicionais_Fisco].Columns.Remove("infAdFisco");

                            if (dataSet.Tables[nTabela_infAdicionais_Fisco].Rows[0]["infCpl"].ToString() == "" || string.IsNullOrEmpty(dataSet.Tables[nTabela_infAdicionais_Fisco].Rows[0]["infCpl"].ToString()))
                                dataSet.Tables[nTabela_infAdicionais_Fisco].Columns.Remove("infCpl");

                            PopularEstrutura(writer, dataSet.Tables[nTabela_infAdicionais_Fisco]);
                            writer.WriteEndElement();
                        }
                    }

                    // 28/05/2026 - Antonio Lemos
                    if (dataSet.Tables[nTabela_Exporta].Rows.Count > 0)
                    {
                        if (dataSet.Tables[nTabela_Exporta].Rows[0]["UFSaidaPais"].ToString() != "" || dataSet.Tables[nTabela_Exporta].Rows[0]["UFSaidaPais"].ToString() != "")
                        {
                            writer.WriteStartElement("exporta");
                            PopularEstrutura(writer, dataSet.Tables[nTabela_Exporta]);
                            writer.WriteEndElement();
                        }

                    }

                    if (idEnvioOPI != "0")
                    {
                        if (dataSet.Tables.Count > nTabela_xPed)
                        {
                            if (dataSet.Tables[nTabela_xPed].Rows[0]["xPed"].ToString() != "")
                            {
                                writer.WriteStartElement("compra");
                                PopularEstrutura(writer, dataSet.Tables[nTabela_xPed]);
                                writer.WriteEndElement();
                            }
                        }
                    }

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }


			public static void PopularEstruturaIndex(XmlWriter writer, DataTable table, int rowIndex)
            {
                DataRow row = table.Rows[rowIndex];

                foreach (DataColumn column in table.Columns)
                {
                    string value = row[column] != DBNull.Value ? row[column].ToString() : string.Empty;
                    if (column.ColumnName == "vDesc" || column.ColumnName == "ID")
                    {
                        if (column.ColumnName == "vDesc")
                            if (value != "0.00")
                            {
                                writer.WriteElementString(column.ColumnName, value.ToString().Trim());
                            }
                        
                    }
                    else if (column.ColumnName == "cBenef")
                    {
                        if (value != "")
                        {
                            writer.WriteElementString(column.ColumnName, value);
                        }
                    }
                    else
                    {
                        writer.WriteElementString(column.ColumnName, value);
                    }
                }
            }
            public static void PopularEstrutura(XmlWriter writer, DataTable table)
            {
                foreach (DataRow row in table.Rows)
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        string value = row[column] != DBNull.Value ? row[column].ToString() : string.Empty;
                        if (column.ColumnName != "ID" && column.ColumnName != "sCalculoDIFAL")
                        {
                            writer.WriteElementString(column.ColumnName, value);
                        }
                    }
                }
            }

            public static string GerarChaveAcesso(string cUF, string anoMesEmissao, string cnpj, string mod, string serie, string nNF, string tpEmis, string cNF)
            {
                // Montar a parte inicial da chave de acesso
                string chave = $"{cUF}{anoMesEmissao}{cnpj}{mod}{serie.PadLeft(3, '0')}{nNF.PadLeft(9, '0')}{tpEmis}{cNF.PadLeft(8, '0')}";

                // Calcular o dígito verificador (DV)
                int dv = CalcularDigitoVerificador(chave);

                // Adicionar o dígito verificador à chave
                chave += dv.ToString();

                // Retornar a chave completa precedida de "NFe"
                return "NFe" + chave;
            }

            private static int CalcularDigitoVerificador(string chave)
            {
                if (chave.Length != 43)
                {
                    throw new ArgumentException("A chave deve ter exatamente 43 caracteres.");
                }

                int soma = 0;
                int peso = 2;

                // percorre da direita para a esquerda
                for (int i = chave.Length - 1; i >= 0; i--)
                {
                    soma += (chave[i] - '0') * peso;
                    peso++;
                    if (peso > 9) peso = 2; // ciclo 2..9
                }

                int resto = soma % 11;
                return (resto == 0 || resto == 1) ? 0 : 11 - resto;
            }
        }

        public class NFS
        {
            private static string sProcedure_XML = "sp_Manipula_tbl_Flow_XML_NFe";

            public sealed class Utf8StringWriter : StringWriter { public override Encoding Encoding => new UTF8Encoding(false); }

            public static void CancelamentoNFS_XML(string sCPF_CNPJ_Prestador, string sInscricao_Prestador, string sNumeroNFS, string sCodigoVerificacao, string sLoteNF, byte[] certificado, string sSenha_Certificado)
            {
                long.TryParse(sInscricao_Prestador, out long nInscricao_Prestador);
                long.TryParse(sNumeroNFS, out long nNumeroNFS);

                PedidoCancelamentoNFe cancelamentoNFS = new PedidoCancelamentoNFe
                {
                    Cabecalho = new PedidoCancelamentoNFeCabecalho
                    {
                        CPFCNPJRemetente = new tpCPFCNPJ
                        {
                            Item = sCPF_CNPJ_Prestador,
                            ItemElementName = sCPF_CNPJ_Prestador.Length == 14 ? ItemChoiceType.CNPJ : ItemChoiceType.CPF
                        },
                        Versao = 2,
                        transacao = true
                    },
                    Detalhe = new PedidoCancelamentoNFeDetalhe[]
                    {
                        new PedidoCancelamentoNFeDetalhe
                        {
                            ChaveNFe = new tpChaveNFe
                            {
                                InscricaoPrestador = nInscricao_Prestador,
                                NumeroNFe = nNumeroNFS,
                                CodigoVerificacao = sCodigoVerificacao
                            },
                            AssinaturaCancelamento = AssinarComCertificado(sInscricao_Prestador.PadLeft(8, '0') + sNumeroNFS.PadLeft(12, '0'), certificado, sSenha_Certificado)
                        }
                    }
                };

                string xml = "";
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(PedidoCancelamentoNFe), "http://www.prefeitura.sp.gov.br/nfe");
                using (Utf8StringWriter sw = new Utf8StringWriter())
                using (XmlWriter writer = XmlWriter.Create(sw, new XmlWriterSettings { Indent = true, Encoding = new UTF8Encoding(false), OmitXmlDeclaration = false }))
                {
                    xmlSerializer.Serialize(writer, cancelamentoNFS);
                    xml = sw.ToString();
                }

                if (!ValidarXML($"{AppContext.BaseDirectory.Replace("TT_Flow\\", "")}/TT_Framework/Schemas/NFS", xml, out string sErro, "PedidoEnvioRPS_v02.xsd"))
                    throw new Exception(sErro);

                File.WriteAllText($@"{Consulta_sCaminho_UniNFe()}\{sCPF_CNPJ_Prestador}\nfse\Envio\{sCPF_CNPJ_Prestador}_{sLoteNF}-ped-cannfse.xml", xml, Encoding.UTF8);
            }

            public static (string nome, string xml, string rps, string serie) EnvioNFS_XML(string idFaturamento, string sDscServico, string sIR, string sISS, string sINSS, string sCSLL, string sPIS, string sCOFINS, string sRPS, string idClassCST)
            {
                Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "RPS_XML" },
                    { "@idFaturamento", idFaturamento },
                    { "@sDscServico", sDscServico },
                    { "@sIR", sIR },
                    { "@sISS", sISS },
                    { "@sINSS", sINSS },
                    { "@sCSLL", sCSLL },
                    { "@sPIS", sPIS },
                    { "@sCOFINS", sCOFINS },
                    { "@sRPS", sRPS },
                    { "@idClassCST", idClassCST }
                };
                DataSet ds = ExecutarDataSet(sProcedure_XML, vParam);
                DataRow row = ds.Tables[0].Rows[0];
                DataRow rowItem = ds.Tables[1].Rows[0];

                object certificado = null;
                string senha = null;

                if (ds.Tables[2].Rows.Count > 0)
                {
                    certificado = ds.Tables[2].Rows[0]["vbArquivo"];
                    senha = ds.Tables[2].Rows[0]["sSenha_Certificado"].ToString();
                }

                if (certificado == null || string.IsNullOrWhiteSpace(senha))
                    throw new Exception($"<b>Erro: </b> Certificado Digital não encontrado para o envio da NFS-e.<br />Por favor registre o Certificado Digital e Senha no cadastro da empresa <a target='_blank' href='/App/Paginas/Manutencao/Empresas_Detalhe.aspx?id={DATASET(ds, "idEmpresa")}'>{DATASET(ds, "sDscEmpresa")}</a>");

                string nNumeroRPS = row.GetValue<string>("nNumeroRPS"),
                        sSerieRPS = row.GetValue<string>("sSerieRPS"),
                        sCPF_CNPJ_Prestador = row.GetValue<string>("sCPF_CNPJ_Prestador"),
                        sCPF_CNPJ_Tomador = row.GetValue<string>("sCPF_CNPJ_Tomador");

                decimal nVlrInicial = Math.Round(rowItem.GetValue<decimal>("nVlrInicial"), 2),
                    nVlrDeducoes = Math.Round(rowItem.GetValue<decimal>("nVlrDeducoes"), 2);

                DateTime dtEmissao = DateTime.Now;
                var statusRPS = tpStatusNFe.N;

                string sAssinatura = GerarAssinatura_RPS(row.GetValue<string>("sIM_Prestador"), sSerieRPS, nNumeroRPS, dtEmissao, sRPS, statusRPS.ToString(), sISS == "S" ? "S" : "N", nVlrInicial, nVlrDeducoes, rowItem.GetValue<string>("sCodigoMunicipal"), sCPF_CNPJ_Tomador);

                PedidoEnvioRPS envioNFS = new PedidoEnvioRPS
                {
                    Cabecalho = new PedidoEnvioRPSCabecalho
                    {
                        CPFCNPJRemetente = new tpCPFCNPJ { Item = sCPF_CNPJ_Prestador, ItemElementName = sCPF_CNPJ_Prestador.Length == 14 ? ItemChoiceType.CNPJ : ItemChoiceType.CPF },
                        Versao = 2
                    },
                    RPS = new tpRPS
                    {
                        Assinatura = AssinarComCertificado(sAssinatura, (byte[])certificado, senha),
                        ChaveRPS = new tpChaveRPS { InscricaoPrestador = row.GetValue<long>("sIM_Prestador"), SerieRPS = sSerieRPS, NumeroRPS = Convert.ToInt64(nNumeroRPS) },
                        TipoRPS = tpTipoRPS.RPS,
                        DataEmissao = dtEmissao,
                        StatusRPS = statusRPS, // N - Normal, C - Cancelada, E - Extraviada
                        TributacaoRPS = sRPS,
                        Item = nVlrInicial,
                        ItemElementName = ItemChoiceType2.ValorFinalCobrado,
                        ValorDeducoes = nVlrDeducoes,
                        ValorPIS = Math.Round(rowItem.GetValue<decimal>("nVlrPIS"), 2),
                        ValorCOFINS = Math.Round(rowItem.GetValue<decimal>("nVlrCOFINS"), 2),
                        ValorINSS = Math.Round(rowItem.GetValue<decimal>("nVlrINSS"), 2),
                        ValorIR = Math.Round(rowItem.GetValue<decimal>("nVlrIR"), 2),
                        ValorCSLL = Math.Round(rowItem.GetValue<decimal>("nVlrCSLL"), 2),
                        CodigoServico = rowItem.GetValue<int>("sCodigoMunicipal"),
                        AliquotaServicos = Math.Round(rowItem.GetValue<decimal>("nAliquota") / 100, 2),
                        ISSRetido = sISS == "S",
                        CPFCNPJTomador = new tpCPFCNPJNIF { Item = sCPF_CNPJ_Tomador, ItemElementName = sCPF_CNPJ_Tomador.Length == 14 ? ItemChoiceType1.CNPJ : ItemChoiceType1.CPF },
                        InscricaoMunicipalTomador = row.GetValue<long>("sIM_Tomador"),
                        InscricaoMunicipalTomadorSpecified = !string.IsNullOrWhiteSpace(row.GetValue<string>("sIM_Tomador")),
                        InscricaoEstadualTomador = row.GetValue<long>("sIE_Tomador"),
                        InscricaoEstadualTomadorSpecified = !string.IsNullOrWhiteSpace(row.GetValue<string>("sIE_Tomador")),
                        RazaoSocialTomador = row.GetValue<string>("sRazaoSocial_Tomador"),
                        EnderecoTomador = new tpEndereco
                        {
                            Logradouro = row.GetValue<string>("sLogradouro_Tomador"),
                            NumeroEndereco = row.GetValue<string>("sNumero_Tomador"),
                            ComplementoEndereco = row.GetValue<string>("sComplemento_Tomador"),
                            Bairro = row.GetValue<string>("sBairro_Tomador"),
                            Cidade = row.GetValue<int>("nCidade_Tomador"),
                            CidadeSpecified = true,
                            UF = row.GetValue<string>("sUF_Tomador"),
                            CEP = row.GetValue<int>("sCEP_Tomador"),
                            CEPSpecified = true
                        },
                        Discriminacao = sDscServico,
                        ValorIPI = Math.Round(rowItem.GetValue<decimal>("nVlrIPI"), 2),
                        ExigibilidadeSuspensa = rowItem.GetValue<int>("ExigibilidadeSuspensa"),
                        NBS = rowItem.GetValue<string>("sNBS").PadLeft(9, '0'),
                        cLocPrestacao = row.GetValue<int>("nCidade_Tomador"),
                        IBSCBS = new tpIBSCBS
                        {
                            finNFSe = rowItem.GetValue<int>("finNFSe"),
                            indFinal = rowItem.GetValue<int>("indFinal"),
                            cIndOp = rowItem.GetValue<string>("cIndOp").PadLeft(6, '0'),
                            valores = new tpValores { trib = new tpTrib { gIBSCBS = new tpGIBSCBS { cClassTrib = rowItem.GetValue<string>("cClassTrib").PadLeft(6, '0') } } }
                        }
                    }
                };

                string xml = "";
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(PedidoEnvioRPS), "http://www.prefeitura.sp.gov.br/nfe");
                using (Utf8StringWriter sw = new Utf8StringWriter())
                using (XmlWriter writer = XmlWriter.Create(sw, new XmlWriterSettings { Indent = true, Encoding = new UTF8Encoding(false), OmitXmlDeclaration = false }))
                {
                    xmlSerializer.Serialize(writer, envioNFS);
                    xml = sw.ToString();
                }

                if (!ValidarXML($"{AppContext.BaseDirectory.Replace("TT_Flow\\", "")}/TT_Framework/Schemas/NFS", xml, out string sErro, "PedidoCancelamentoNFe_v02.xsd"))
                    throw new Exception(sErro);

                Dictionary<string, string> vParametrosRPS = new Dictionary<string, string>
                {
                    { "@sFuncao", "Numero_RPS" },
                    { "@sNumeroRPS", nNumeroRPS },
                    { "@idFaturamento", idFaturamento }
                };
                ExecutarDataSet(sProcedure_XML, vParametrosRPS);

                string sNomeArquivo = $"RPS{nNumeroRPS}-{CarimboDataHora()}.xml";
                File.WriteAllText($"{AppContext.BaseDirectory}Download\\{sNomeArquivo}", xml, Encoding.UTF8);

                return (sNomeArquivo, xml, nNumeroRPS, sSerieRPS);
            }

            public static (string nome, string txt, string rps, string serie) EnvioNFS_TXT(string idFaturamento, string idEmpresa, string sDscServico, string sIR, string sISS, string sINSS, string sCSLL, string sPIS, string sCOFINS, string sRPS)
            {
                StringBuilder sbArquivo = new StringBuilder();
                string serie = "", numeroRPS = "";

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "NFS_TXT" },
                    { "@idFaturamento", idFaturamento },
                    { "@sDscServico", sDscServico },
                    { "@sIR", sIR },
                    { "@sISS", sISS },
                    { "@sINSS", sINSS },
                    { "@sCSLL", sCSLL },
                    { "@sPIS", sPIS },
                    { "@sCOFINS", sCOFINS },
                    { "@sRPS", sRPS }
                };
                DataSet dsPesquisa = ExecutarDataSet(sProcedure_XML, vParametros);

                int TipoRegistro = 1;
                foreach (DataRow linha in dsPesquisa.Tables[0].Rows)
                {
                    string valor = linha["Valor"]?.ToString() ?? "",
                           campo = linha["Campo"]?.ToString() ?? "";

                    if (campo == "Número do RPS")
                        numeroRPS = valor;

                    if (campo == "Série do RPS")
                        serie = valor;

                    if (campo == "Tipo de registro")
                    {
                        if (TipoRegistro == 2) valor = "6";
                        if (TipoRegistro == 3) valor = "9";

                        TipoRegistro++;
                    }

                    if (campo == "Caractere de Fim de Linha") sbArquivo.AppendLine();
                    else sbArquivo.Append(valor);
                }

                Dictionary<string, string> vParametrosRPS = new Dictionary<string, string>
                {
                    { "@sFuncao", "Numero_RPS" },
                    { "@sNumeroRPS", (int.Parse(numeroRPS) + 1).ToString() },
                    { "@idEmpresa", idEmpresa }
                };
                ExecutarDataSet(sProcedure_XML, vParametrosRPS);

                string txt = RemoverAcentos(sbArquivo.ToString());
                return ($"RPS{numeroRPS}-{CarimboDataHora()}.txt", txt, numeroRPS, serie);
            }

            public static byte[] PDF_Servicos(string NumeroRPS, string idFaturamento, string discriminacao, string sContrib, string serie, string sIR, string sINSS, string sCSLL, string sPIS, string sCOFINS)
            {
                ReportViewer rv = new ReportViewer { ProcessingMode = ProcessingMode.Local };
                rv.LocalReport.ReportPath = "App\\Reports\\NF-s.rdlc";
                rv.LocalReport.EnableExternalImages = true;

                if (string.IsNullOrEmpty(sContrib)) sContrib = "---";

                Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>
                {
                    { "@idFaturamento", idFaturamento },
                    { "@sDscServico", discriminacao },
                    { "@sDscContrib", sContrib },
                    { "@sIR", sIR },
                    { "@sINSS", sINSS },
                    { "@sCSLL", sCSLL },
                    { "@sPIS", sPIS },
                    { "@sCOFINS", sCOFINS }
                };
                DataTable tb = ExecutarDataTable("sp_Manipula_tbl_Flow_NotaServicos_PDF", vParametrosProduct);

                foreach (DataRow row in tb.Rows) { row["Discriminacao"] = row["Discriminacao"].ToString().Replace("|", Environment.NewLine); }

                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", tb));

                ReportParameter[] rp = new ReportParameter[4];
                rp[0] = new ReportParameter("Espaco", " ");
                rp[1] = new ReportParameter("Link", " ");
                rp[2] = new ReportParameter("NumeroRPS", NumeroRPS.PadLeft(12, '0'));
                rp[3] = new ReportParameter("serie", serie);

                rv.LocalReport.SetParameters(rp);
                rv.LocalReport.Refresh();

                return rv.LocalReport.Render("PDF", null, out _, out _, out _, out _, out _);
            }

            public static (string, List<(string idXML, string idFaturamento, string nNFS, string sCodigoVerificacao)>) Valida_ArquivoRetorno_NFS(Stream txt, string idPedido)
            {
                string sErro = "";
                var lista = new List<(string idXML, string idFaturamento, string nNFS, string sCodigoVerificacao)>();

                using (var reader = new StreamReader(txt))
                {
                    string linha;
                    while ((linha = reader.ReadLine()) != null)
                    {
                        if (linha.StartsWith("2"))
                        {
                            try
                            {
                                string nNFS = linha.Substring(9, 8).Trim(),
                                        numeroRPS = linha.Substring(49, 12).Trim(),
                                        sCodigoVerificacao = linha.Substring(31, 8).Trim();

                                Dictionary<string, string> vParam = new Dictionary<string, string>
                                {
                                    { "@sFuncao", "VALIDAR_RETORNO_NFS" },
                                    { "@idPedido", idPedido },
                                    { "@nNumeroNF", numeroRPS },
                                    { "@sCodigoVerificacao", sCodigoVerificacao }
                                };
                                DataSet ds = ExecutarDataSet(sProcedure_XML, vParam);

                                foreach (DataRow row in ds.Tables[0].Rows)
                                    lista.Add((row["idXML"].ToString(), row["idFaturamento"].ToString(), nNFS, sCodigoVerificacao));
                            }
                            catch (Exception ex)
                            {
                                sErro = ex.Message;
                            }
                        }
                    }
                }

                if (!lista.Any())
                    sErro = "Nenhuma RPS vinculada à este Pedido foi encontrada no arquivo de retorno.";

                return (sErro, lista);
            }

            public static bool ValidarXML(string pastaSchemas, string xml, out string sErro, params string[] schemasNaoValidos)
            {
                sErro = "";
                var erros = new List<(bool bErro, string sMsg)>();

                if (string.IsNullOrWhiteSpace(pastaSchemas) || !Directory.Exists(pastaSchemas))
                {
                   sErro = $"Pasta de schemas não encontrada! ({pastaSchemas})";
                     return false;
                }

                XmlSchemaSet schemaSet = new XmlSchemaSet
                {
                    XmlResolver = new XmlUrlResolver()
                };

                string[] schemas = Directory.GetFiles(pastaSchemas, "*.xsd").Where(s => !schemasNaoValidos.Contains(Path.GetFileName(s))).ToArray();
                foreach (string schema in schemas)
                    schemaSet.Add(null, schema);

                schemaSet.Compile();

                XmlReaderSettings settings = new XmlReaderSettings { Schemas = schemaSet, ValidationType = ValidationType.Schema };
                settings.ValidationEventHandler += (sender, e) =>
                {
                    if (e.Severity == XmlSeverityType.Error)
                        erros.Add((true, $"Erro: {e.Message}\r\n"));
                    else
                        erros.Add((false, $"Aviso: {e.Message}\r\n"));
                };

                using (StringReader sr = new StringReader(xml))
                using (XmlReader reader = XmlReader.Create(sr, settings))
                {
                    try
                    {
                        while (reader.Read()) { }
                    }
                    catch (Exception ex)
                    {
                        sErro = ex.Message;
                        return false;
                    }
                }

                sErro = string.Join("\r\n", erros.OrderByDescending(e => e.bErro).Select(e => e.sMsg));
                return !erros.Any(e => e.bErro);
            }

            private static byte[] AssinarComCertificado(string sAssinatura, byte[] bCertificado, string sSenha_Certificado)
            {
                X509Certificate2 certificado = new X509Certificate2(bCertificado, sSenha_Certificado, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
                RSA rsa = certificado.GetRSAPrivateKey() ?? throw new InvalidOperationException("O certificado não possui chave privada RSA.");
                return rsa.SignHash(SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(sAssinatura)), HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            }

            private static string GerarAssinatura_RPS(string sIM_Prestador, string sSerie, string sNumeroRPS, DateTime dtEmissao, string sTipoTributacao, string sStatus, string sISS_Retido, decimal nVlrServico, decimal nVlrDeducoes, string sCodigoServico, string sCPF_CNPJ_Tomador)
            {
                return sIM_Prestador.PadLeft(12, '0') +
                       sSerie.PadRight(5, ' ') +
                       sNumeroRPS.PadLeft(12, '0') +
                       dtEmissao.ToString("yyyyMMdd") +
                       sTipoTributacao +
                       sStatus +
                       sISS_Retido +
                       nVlrServico.ToString("F2", CultureInfo.InvariantCulture).Replace(".", "").PadLeft(15, '0') +
                       nVlrDeducoes.ToString("F2", CultureInfo.InvariantCulture).Replace(".", "").PadLeft(15, '0') +
                       sCodigoServico.PadLeft(5, '0') +
                       (sCPF_CNPJ_Tomador.Length == 14 ? "2" : sCPF_CNPJ_Tomador.Length == 11 ? "1" : "3") +
                       sCPF_CNPJ_Tomador.PadLeft(14, '0');
            }
        }

        public class DANFE
        {
            /// <summary>
            /// PdfClown (DanfeViewModelCreator) exige nfeProc com NFe + protNFe.
            /// Espelhos gravam só NFe (ou NFe + protNFe colados sem envelope válido).
            /// </summary>
            private static string LimparElementoNFe(XElement nfe)
            {
                const string ns = "http://www.portalfiscal.inf.br/nfe";
                nfe.Descendants(XName.Get("Signature", "http://www.w3.org/2000/09/xmldsig#")).Remove();
                if (nfe.Attribute("xmlns") == null)
                    nfe.SetAttributeValue("xmlns", ns);
                return nfe.ToString(SaveOptions.DisableFormatting);
            }

            private static string ExtrairBlocoXml(string sXML, string tagName)
            {
                string openTag = "<" + tagName;
                int idx = sXML.IndexOf(openTag, StringComparison.OrdinalIgnoreCase);
                if (idx < 0)
                    return null;

                string closeTag = "</" + tagName + ">";
                int idxFim = sXML.IndexOf(closeTag, idx, StringComparison.OrdinalIgnoreCase);
                if (idxFim < 0)
                    return null;

                return sXML.Substring(idx, idxFim - idx + closeTag.Length);
            }

            private static string ExtrairElementoNFe(string sXML)
            {
                if (string.IsNullOrWhiteSpace(sXML))
                    return sXML;

                sXML = sXML.Trim().TrimStart('\uFEFF');
                const string ns = "http://www.portalfiscal.inf.br/nfe";
                XName nfeName = XName.Get("NFe", ns);

                try
                {
                    int idxNFe = sXML.IndexOf("<NFe", StringComparison.OrdinalIgnoreCase);
                    if (idxNFe >= 0)
                    {
                        int idxFim = sXML.IndexOf("</NFe>", idxNFe, StringComparison.OrdinalIgnoreCase);
                        if (idxFim >= 0)
                            return LimparElementoNFe(XElement.Parse(sXML.Substring(idxNFe, idxFim - idxNFe + "</NFe>".Length)));
                    }

                    int idxInf = sXML.IndexOf("<infNFe", StringComparison.OrdinalIgnoreCase);
                    if (idxInf >= 0)
                    {
                        int idxFimInf = sXML.IndexOf("</infNFe>", idxInf, StringComparison.OrdinalIgnoreCase);
                        if (idxFimInf >= 0)
                        {
                            XElement inf = XElement.Parse(sXML.Substring(idxInf, idxFimInf - idxInf + "</infNFe>".Length));
                            return LimparElementoNFe(new XElement(nfeName, inf));
                        }
                    }

                    if (sXML.StartsWith("<nfeProc", StringComparison.OrdinalIgnoreCase))
                    {
                        XElement nfe = XDocument.Parse(sXML).Descendants(nfeName).FirstOrDefault();
                        if (nfe != null)
                            return LimparElementoNFe(new XElement(nfe));
                    }
                }
                catch
                {
                }

                return sXML;
            }

            private static string MontarProtNFeEspelho(string versao, string chNFe, string tpAmb)
            {
                const string ns = "http://www.portalfiscal.inf.br/nfe";
                if (!string.IsNullOrEmpty(chNFe) && chNFe.StartsWith("NFe", StringComparison.OrdinalIgnoreCase))
                    chNFe = chNFe.Substring(3);

                string dhRecbto = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                return $"<protNFe versao=\"{versao}\" xmlns=\"{ns}\"><infProt><tpAmb>{tpAmb}</tpAmb><verAplic>TT_FLOW</verAplic><chNFe>{chNFe}</chNFe><dhRecbto>{dhRecbto}</dhRecbto><nProt>000000000000000</nProt><digVal>0000000000000000000000000000000000000000</digVal><cStat>100</cStat><xMotivo>ESPELHO - SEM VALOR FISCAL</xMotivo></infProt></protNFe>";
            }

            private static string PrepararXmlParaPdfClown(string sXML, string sChaveNFe, bool bEspelho)
            {
                if (string.IsNullOrWhiteSpace(sXML))
                    return sXML;

                sXML = sXML.Trim().TrimStart('\uFEFF');
                const string ns = "http://www.portalfiscal.inf.br/nfe";

                string nfeProcExistente = ExtrairBlocoXml(sXML, "nfeProc");
                if (!bEspelho && !string.IsNullOrEmpty(nfeProcExistente))
                {
                    try
                    {
                        XDocument.Parse(nfeProcExistente);
                        return nfeProcExistente;
                    }
                    catch
                    {
                    }
                }

                string nfeXml = ExtrairElementoNFe(sXML);
                string protXml = ExtrairBlocoXml(sXML, "protNFe");
                string versao = "4.00";
                string tpAmb = "2";
                string chNFe = sChaveNFe;

                try
                {
                    XDocument nfeDoc = XDocument.Parse(nfeXml);
                    XElement inf = nfeDoc.Descendants(XName.Get("infNFe", ns)).FirstOrDefault();
                    if (inf?.Attribute("versao") != null)
                        versao = inf.Attribute("versao").Value;

                    XElement tpAmbEl = nfeDoc.Descendants(XName.Get("tpAmb", ns)).FirstOrDefault();
                    if (tpAmbEl != null)
                        tpAmb = tpAmbEl.Value;

                    if (string.IsNullOrEmpty(chNFe))
                    {
                        string id = inf?.Attribute("Id")?.Value ?? "";
                        if (id.StartsWith("NFe", StringComparison.OrdinalIgnoreCase))
                            chNFe = id.Substring(3);
                    }
                }
                catch
                {
                }

                if (bEspelho || string.IsNullOrEmpty(protXml))
                    protXml = MontarProtNFeEspelho(versao, chNFe, tpAmb);

                return $"<nfeProc versao=\"{versao}\" xmlns=\"{ns}\">{nfeXml}{protXml}</nfeProc>";
            }

            //public static string GerarDANFE_PdfClown(string sPath_Salvar, string sXML, string sChaveNFe, string idEmpresa)
            //{
            //    string sNomeGerado = "";

            //    try
            //    {
            //        sXML = sXML.Replace("\u00a0", " ");
            //        var model = DanfeViewModelCreator.CriarDeStringXml(sXML);

            //        using (var pdfStream = new MemoryStream())
            //        {
            //            using (var danfe = new DanfeDoc(model))
            //            {
            //                byte[] logoMarca = ConsultaImagemDANFE(idEmpresa);

            //                if (logoMarca != null)
            //                {
            //                    using (var logo = new MemoryStream(logoMarca))
            //                    {
            //                        danfe.AdicionarLogoImagem(logo);
            //                    }
            //                }

            //                danfe.Gerar();
            //                byte[] pdfBytes = danfe.ObterPdfBytes(pdfStream);

            //                sNomeGerado = sChaveNFe + "_DANFE.pdf";
            //                string sCaminhoCompleto = Path.Combine(sPath_Salvar, sNomeGerado);
            //                File.WriteAllBytes(sCaminhoCompleto, pdfBytes);
            //            }
            //        }
            //    }
            //    catch (Exception exc)
            //    {
            //        sNomeGerado = "";
            //        //throw new Exception(exc.Message);
            //    }


            //    return sNomeGerado;
            //}

            private static void AplicarFonteSansSerifDanfe(DanfeDoc danfe)
            {
                FieldInfo field = typeof(DanfeDoc).GetField("_FonteFamilia", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                    return;

                object helvetica = Enum.Parse(field.FieldType, "Helvetica");
                field.SetValue(danfe, helvetica);
            }

            public static string GerarDANFE_PdfClown(string sPath_Salvar, string sXML, string sChaveNFe, string idEmpresa, string sStatus)
            {
                bool bEspelho = string.Equals(sStatus, "Espelho", StringComparison.OrdinalIgnoreCase);

                try
                {
                    const string ns = "http://www.portalfiscal.inf.br/nfe";
                    string nfeXml = ExtrairElementoNFe(sXML);

                    XDocument xdoc = XDocument.Parse(nfeXml);
                    string cnpjEmitente = xdoc.Descendants(XName.Get("emit", ns)).Descendants(XName.Get("CNPJ", ns)).FirstOrDefault()?.Value;
                    XElement infAdic = xdoc.Descendants(XName.Get("infAdic", ns)).FirstOrDefault();
                    if (infAdic != null)
                    {
                        XName infAdFiscoName = XName.Get("infAdFisco", ns);
                        XName infCplName = XName.Get("infCpl", ns);
                        string infAdFisco = infAdic.Element(infAdFiscoName)?.Value ?? "";
                        string infCpl = infAdic.Element(infCplName)?.Value ?? "";
                        string novoInfCpl = string.IsNullOrWhiteSpace(infAdFisco)
                            ? infCpl
                            : $"{infAdFisco}\n{infCpl}".Trim();

                        if (infAdic.Element(infCplName) != null)
                            infAdic.Element(infCplName).Value = novoInfCpl;
                        else if (!string.IsNullOrEmpty(novoInfCpl))
                            infAdic.Add(new XElement(infCplName, novoInfCpl));
                    }

                    nfeXml = xdoc.Root.ToString(SaveOptions.DisableFormatting).Replace("\u00a0", " ");
                    string xmlNfeProc = PrepararXmlParaPdfClown(nfeXml, sChaveNFe, bEspelho);

                    DanfeViewModel model = DanfeViewModelCreator.CriarDeStringXml(xmlNfeProc);

                    using (var pdfStream = new MemoryStream())
                    {
                        using (var danfe = new DanfeDoc(model))
                        {
                            AplicarFonteSansSerifDanfe(danfe);

                            byte[] logoMarca = ConsultaImagemDANFE(idEmpresa, cnpjEmitente);
                            if (logoMarca != null)
                            {
                                using (var logo = new MemoryStream(logoMarca))
                                    danfe.AdicionarLogoImagem(logo);
                            }

                            danfe.Gerar();
                            byte[] pdfBytes = danfe.ObterPdfBytes(pdfStream);

                            string sNomeGerado = sChaveNFe + "_" + sStatus + ".pdf";
                            File.WriteAllBytes(Path.Combine(sPath_Salvar, sNomeGerado), pdfBytes);
                            return sNomeGerado;
                        }
                    }
                }
                catch (Exception exc)
                {
                    if (bEspelho)
                        throw new Exception("Erro ao gerar espelho (PdfClown): " + exc.GetBaseException().Message, exc);
                    return "";
                }
            }


            public static string GerarDANFE(string sPath_Salvar, string sChaveNFe, string sXML, bool bForcarGerarPDF, string sStatus, string idEmpresa)
            {
                string sNomeArquivoGErado = "";
                string sCaminho_UniNFE = @"C:\Unimake\UniNFe";
                bool bArquivoExiste = false;
                try
                {
                    if (sStatus != "")
                        bForcarGerarPDF = true;

                    if (!bForcarGerarPDF && Directory.Exists(sCaminho_UniNFE))
                    {
                        string cnpjPattern = @"\d{14}";
                        foreach (var cnpjDir in Directory.GetDirectories(sCaminho_UniNFE))
                        {
                            string dirName = Path.GetFileName(cnpjDir);
                            if (Regex.IsMatch(dirName, cnpjPattern))
                            {
                                string sDiretorio = cnpjDir + @"\T-FLOW\PROCESSADO";

                                if (File.Exists(sDiretorio + @"\" + sChaveNFe + "_DANFE.pdf"))
                                {
                                    Arquivo.CopiarArquivo_Windows(sDiretorio + @"\" + sChaveNFe + "_DANFE.pdf", sPath_Salvar, sChaveNFe + "_DANFE.pdf");
                                    bArquivoExiste = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!bArquivoExiste)
                    {
                        if (!Directory.Exists(sPath_Salvar))
                        {
                            Directory.CreateDirectory(sPath_Salvar);
                        }

                        string sNomeArquivoXML = string.Format("{0}.xml", sChaveNFe);
                        string sDiretorioXML = sPath_Salvar + "/" + sNomeArquivoXML;

                        sXML = ExtrairElementoNFe(sXML);
                        sNomeArquivoGErado = GerarDANFE_PdfClown(sPath_Salvar, sXML, sChaveNFe, idEmpresa, sStatus);

                        if (sNomeArquivoGErado == "" && string.Equals(sStatus, "Espelho", StringComparison.OrdinalIgnoreCase))
                            throw new ArgumentException("Não foi possível gerar o PDF do espelho.");

                        if (sNomeArquivoGErado == "")
                        {

                            if (!File.Exists(sDiretorioXML))
                                File.WriteAllText(sDiretorioXML, sXML);



                            ReportViewer rv = new ReportViewer();



                            rv.ProcessingMode = ProcessingMode.Local;
                            rv.LocalReport.EnableExternalImages = true;
                            rv.LocalReport.ReportPath = "App\\Reports\\" + "NF-e.rdlc";

                            #region | DADOS NF-e
                            XDocument xml = XDocument.Load(sDiretorioXML);
                            string nNF = "";
                            string tpAmb = "";
                            string serie = "";
                            string dhEmi = "";
                            string CNPJ = "";
                            string xNome = "";
                            string xLgr = "";
                            string nro = "";
                            string xBairro = "";
                            string CEP = "";
                            string xMun = "";
                            string UF = "";
                            string IE = "";
                            string mod = "";
                            string DESTxNome = "";
                            string DESTCNPJ = "";
                            string DESTxLgr = "";
                            string DESTnro = "";
                            string DESTxCpl = "";
                            string DESTxBairro = "";
                            string DESTCEP = "";
                            string DESTxMun = "";
                            string DESTUF = "";
                            string DESIE = "";
                            string fone = "";
                            string vNF = "";
                            string vBC = "";
                            string vICMS = "";
                            string vBCST = "";
                            string vST = "";
                            string vProd = "";
                            string chave = "";
                            string natOp = "";
                            string nProt = "";
                            string dhRecbto = "";
                            string dhSaiEnt = "";
                            string horaSaiEnt = "";
                            string vFrete = "";
                            string vSeg = "";
                            string vDesc = "";
                            string vOutro = "";
                            string vIPI = "";
                            string vTotTrib = "0";
                            string vICMSUFDest = "";
                            string vICMSDeson = "";
                            string vFCP = "";
                            string vFCPST = "";
                            string vFCPSTRet = "";
                            string vIPIDevol = "";
                            string vPIS = "";
                            string vCOFINS = "";
                            string modFrete = "";
                            string transportaCNPJ = "";
                            string transportaxNome = "";
                            string transportaIE = "";
                            string transportaxEnder = "";
                            string transportaxMun = "";
                            string transportaUF = "";
                            string qVol = "";
                            string esp = "";
                            string pesoL = "";
                            string pesoB = "";
                            string infAdFisco = "";
                            string infCpl = "";
                            string vII = "";

                            var infNFeElement = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}infNFe").FirstOrDefault();

                            XNamespace ns = "http://www.portalfiscal.inf.br/nfe";
                            var infProt = xml.Descendants(ns + "infProt").FirstOrDefault();

                            if (infProt != null)
                            {
                                nProt = infProt.Element(ns + "nProt")?.Value;

                                if (DateTimeOffset.TryParse(infProt.Element(ns + "dhRecbto")?.Value, out var dhRecebto))
                                {
                                    dhRecbto = dhRecebto.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
                                }
                            }

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}natOp").FirstOrDefault() != null)
                                natOp = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}natOp").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}nNF").FirstOrDefault() != null)
                                nNF = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}nNF").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpAmb").FirstOrDefault() != null)
                                tpAmb = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpAmb").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}serie").FirstOrDefault() != null)
                                serie = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}serie").FirstOrDefault()?.Value;

                            var dhEmiElement = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide")
                                  .Descendants("{http://www.portalfiscal.inf.br/nfe}dhEmi")
                                  .FirstOrDefault();

                            if (dhEmiElement != null)
                            {
                                DateTime dataEmissao;
                                if (DateTime.TryParse(dhEmiElement.Value, out dataEmissao))
                                {
                                    dhEmi = dataEmissao.ToString("dd/MM/yyyy");
                                }
                            }

                            var dhSaiEntElement = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}dhSaiEnt").FirstOrDefault();

                            if (dhSaiEntElement != null)
                            {
                                DateTime dataSaida;
                                if (DateTime.TryParse(dhSaiEntElement.Value, out dataSaida))
                                {
                                    dhSaiEnt = dataSaida.ToString("dd/MM/yyyy");
                                    horaSaiEnt = dataSaida.ToString("HH:mm:ss");
                                }
                            }

                            if (infNFeElement != null)
                            {
                                chave = infNFeElement.Attribute("Id")?.Value?.Substring(3);
                            }
                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vProd").FirstOrDefault() != null)
                                vProd = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vProd").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMSDeson").FirstOrDefault() != null)
                                vICMSDeson = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMSDeson").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCP").FirstOrDefault() != null)
                                vFCP = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCP").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault() != null)
                                vPIS = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault() != null)
                                vCOFINS = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCPST").FirstOrDefault() != null)
                                vFCPST = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCPST").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMSUFDest").FirstOrDefault() != null)
                                vICMSUFDest = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMSUFDest").FirstOrDefault()?.Value;
                            else
                                vICMSUFDest = "0,00";

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault() != null)
                                vII = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault() != null)
                                vIPI = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vTotTrib").FirstOrDefault() != null)
                                vTotTrib = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vTotTrib").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vDesc").FirstOrDefault() != null)
                                vDesc = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vDesc").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPIDevol").FirstOrDefault() != null)
                                vIPIDevol = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPIDevol").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCPSTRet").FirstOrDefault() != null)
                                vFCPSTRet = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCPSTRet").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vOutro").FirstOrDefault() != null)
                                vOutro = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vOutro").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFrete").FirstOrDefault() != null)
                                vFrete = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFrete").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vSeg").FirstOrDefault() != null)
                                vSeg = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vSeg").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                                vBC = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault() != null)
                                vICMS = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vNF").FirstOrDefault() != null)
                                vNF = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vNF").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vBCST").FirstOrDefault() != null)
                                vBCST = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vBCST").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vST").FirstOrDefault() != null)
                                vST = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vST").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                                CNPJ = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                                xNome = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault() != null)
                                xLgr = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}fone").FirstOrDefault() != null)
                                fone = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}fone").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault() != null)
                                nro = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault() != null)
                                xBairro = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault() != null)
                                CEP = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault() != null)
                                xMun = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault() != null)
                                UF = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault() != null)
                                IE = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}mod").FirstOrDefault() != null)
                                mod = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}mod").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                                DESTCNPJ = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CPF").FirstOrDefault() != null)
                                DESTCNPJ = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CPF").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                                DESTxNome = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault() != null)
                                DESTxLgr = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault() != null)
                                DESTnro = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xCpl").FirstOrDefault() != null)
                                DESTxCpl = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xCpl").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault() != null)
                                DESTxBairro = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault() != null)
                                DESTCEP = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault() != null)
                                DESTxMun = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault() != null)
                                DESTUF = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault() != null)
                                DESIE = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}modFrete").FirstOrDefault() != null)
                                modFrete = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}modFrete").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                                transportaCNPJ = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                                transportaxNome = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault() != null)
                                transportaIE = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xEnder").FirstOrDefault() != null)
                                transportaxEnder = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xEnder").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault() != null)
                                transportaxMun = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault() != null)
                                transportaUF = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}qVol").FirstOrDefault() != null)
                                qVol = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}qVol").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}esp").FirstOrDefault() != null)
                                esp = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}esp").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoL").FirstOrDefault() != null)
                                pesoL = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoL").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoB").FirstOrDefault() != null)
                                pesoB = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoB").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").Descendants("{http://www.portalfiscal.inf.br/nfe}infCpl").FirstOrDefault() != null)
                                infCpl = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").Descendants("{http://www.portalfiscal.inf.br/nfe}infCpl").FirstOrDefault()?.Value;

                            if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").Descendants("{http://www.portalfiscal.inf.br/nfe}infAdFisco").FirstOrDefault() != null)
                                infAdFisco = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").Descendants("{http://www.portalfiscal.inf.br/nfe}infAdFisco").FirstOrDefault()?.Value;

                            var itens = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Select(d => new
                            {
                                nItem = d.Attribute("nItem")?.Value,
                                cProd = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}cProd").FirstOrDefault()?.Value,
                                cEAN = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}cEAN").FirstOrDefault()?.Value,
                                xProd = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}xProd").FirstOrDefault()?.Value,
                                NCM = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}NCM").FirstOrDefault()?.Value,
                                CFOP = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}CFOP").FirstOrDefault()?.Value,
                                uCom = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}uCom").FirstOrDefault()?.Value,
                                qCom = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}qCom").FirstOrDefault()?.Value,
                                vUnCom = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}vUnCom").FirstOrDefault()?.Value,
                                vProd = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}vProd").FirstOrDefault()?.Value,
                                cEANTrib = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}cEANTrib").FirstOrDefault()?.Value,
                                uTrib = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}uTrib").FirstOrDefault()?.Value,
                                qTrib = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}qTrib").FirstOrDefault()?.Value,
                                vUnTrib = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}vUnTrib").FirstOrDefault()?.Value,
                                indTot = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}indTot").FirstOrDefault()?.Value,
                                xPed = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}xPed").FirstOrDefault()?.Value,
                                CEST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}CEST").FirstOrDefault()?.Value,
                                vDesc = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}vDesc").FirstOrDefault()?.Value,

                                orig = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "orig").FirstOrDefault()?.Value,
                                CST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "CST").FirstOrDefault()?.Value,
                                vBC = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "vBC").FirstOrDefault()?.Value,
                                pICMS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "pICMS").FirstOrDefault()?.Value,
                                vICMS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "vICMS").FirstOrDefault()?.Value,
                                modBCST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "modBCST").FirstOrDefault()?.Value,
                                pMVAST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "pMVAST").FirstOrDefault()?.Value,
                                vBCST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "vBCST").FirstOrDefault()?.Value,
                                pICMSST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "pICMSST").FirstOrDefault()?.Value,
                                vICMSST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "vICMSST").FirstOrDefault()?.Value,

                                vBCIPI = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants().Where(x => x.Name.LocalName == "vBC").FirstOrDefault()?.Value,
                                pIPI = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants().Where(x => x.Name.LocalName == "pIPI").FirstOrDefault()?.Value,
                                vIPI = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants().Where(x => x.Name.LocalName == "vIPI").FirstOrDefault()?.Value,

                                vBCPIS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants().Where(x => x.Name.LocalName == "vBC").FirstOrDefault()?.Value,
                                pPIS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants().Where(x => x.Name.LocalName == "pPIS").FirstOrDefault()?.Value,
                                vPIS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants().Where(x => x.Name.LocalName == "vPIS").FirstOrDefault()?.Value,

                                vBCCOFINS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants().Where(x => x.Name.LocalName == "vBC").FirstOrDefault()?.Value,
                                pCOFINS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants().Where(x => x.Name.LocalName == "pCOFINS").FirstOrDefault()?.Value,
                                vCOFINS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants().Where(x => x.Name.LocalName == "vCOFINS").FirstOrDefault()?.Value,

                                vBCUFDest = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "vBCUFDest").FirstOrDefault()?.Value,
                                pFCPUFDest = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "pFCPUFDest").FirstOrDefault()?.Value,
                                pICMSUFDest = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "pICMSUFDest").FirstOrDefault()?.Value,
                                pICMSInter = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "pICMSInter").FirstOrDefault()?.Value,
                                pICMSInterPart = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "pICMSInterPart").FirstOrDefault()?.Value,
                                vICMSUFDestuni = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "vICMSUFDest").FirstOrDefault()?.Value,
                                vICMSUFRemet = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "vICMSUFRemet").FirstOrDefault()?.Value,
                                vFCPUFDest = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "vFCPUFDest").FirstOrDefault()?.Value,
                            }).ToList();
                            #endregion

                            Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>
                            {

                            };
                            DataSet dtProduct1 = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_NFe", vParametrosProduct);

                            if (modFrete != "")
                            {
                                if (modFrete == "0")
                                    modFrete = "0 - " + "Remetente";
                                if (modFrete == "1")
                                    modFrete = "1 - " + "Destinatário";
                                if (modFrete == "2")
                                    modFrete = "2 - " + "Terceiros";
                                if (modFrete == "3")
                                    modFrete = "3 - " + "próp Remetente";
                                if (modFrete == "4")
                                    modFrete = "4 - " + "próp Destinatário";
                                if (modFrete == "9")
                                    modFrete = "9 - " + "Sem transporte";
                            }

                            foreach (DataRow row in dtProduct1.Tables[0].Rows)
                            {
                                row["nNF"] = int.Parse(nNF).ToString("D9").Insert(3, ".").Insert(7, ".");
                                row["serie"] = Convert.ToInt32(serie).ToString("D3");
                                row["chave"] = chave;
                                row["dhEmi"] = dhEmi;
                                row["dhSaiEnt"] = dhSaiEnt;
                                row["nProt"] = nProt;
                                row["natOp"] = natOp;
                                row["dhRecbto"] = dhRecbto;
                                row["horaSaiEnt"] = horaSaiEnt;

                                if (!string.IsNullOrWhiteSpace(CNPJ) && CNPJ.Length == 14)
                                {
                                    CNPJ = Convert.ToUInt64(CNPJ).ToString(@"00\.000\.000\/0000\-00");
                                }
                                row["CNPJ"] = CNPJ;
                                row["xNome"] = xNome;
                                row["xLgr"] = xLgr;
                                row["nro"] = nro;
                                row["xBairro"] = xBairro;
                                row["xMun"] = xMun;
                                row["UF"] = UF;
                                if (!string.IsNullOrWhiteSpace(CEP) && CEP.Length == 8)
                                {
                                    CEP = Convert.ToUInt64(CEP).ToString(@"00000\-000");
                                }
                                row["CEP"] = CEP;
                                row["fone"] = fone;
                                row["IE"] = IE;

                                if (!string.IsNullOrWhiteSpace(DESTCNPJ) && DESTCNPJ.Length == 14)
                                {
                                    DESTCNPJ = Convert.ToUInt64(DESTCNPJ).ToString(@"00\.000\.000\/0000\-00");
                                }
                                row["destCNPJ"] = DESTCNPJ;
                                row["destxNome"] = DESTxNome;
                                row["destxLgr"] = DESTxLgr;
                                row["destnro"] = DESTnro;
                                row["destxCpl"] = DESTxCpl;
                                row["destxBairro"] = DESTxBairro;
                                //row["destcMun"] = IE;
                                row["destxMun"] = DESTxMun;
                                row["destUF"] = DESTUF;
                                if (!string.IsNullOrWhiteSpace(DESTCEP) && DESTCEP.Length == 8)
                                {
                                    DESTCEP = Convert.ToUInt64(DESTCEP).ToString(@"00000\-000");
                                }
                                row["destCEP"] = DESTCEP;
                                //row["destxPais"] = IE;
                                row["destIE"] = DESIE;

                                decimal vBCtotal = decimal.Parse(vBC, CultureInfo.InvariantCulture);
                                row["vBC"] = vBCtotal.ToString("N2");
                                decimal vICMSTotal = decimal.Parse(vICMS, CultureInfo.InvariantCulture);
                                row["vICMS"] = vICMSTotal.ToString("N2");
                                decimal vICMSDesontotal = decimal.Parse(vICMSDeson, CultureInfo.InvariantCulture);
                                row["vICMSDeson"] = vICMSDesontotal.ToString("N2");
                                decimal vICMSUFDestTotal = decimal.Parse(vICMSUFDest, CultureInfo.InvariantCulture);
                                row["vICMSUFDest"] = vICMSUFDestTotal.ToString("N2");
                                decimal vFCPTotal = decimal.Parse(vFCP, CultureInfo.InvariantCulture);
                                row["vFCP"] = vFCPTotal.ToString("N2");
                                decimal vBCSTTotal = decimal.Parse(vBCST, CultureInfo.InvariantCulture);
                                row["vBCST"] = vBCSTTotal.ToString("N2");
                                decimal vSTTotal = decimal.Parse(vST, CultureInfo.InvariantCulture);
                                row["vST"] = vSTTotal.ToString("N2");
                                decimal vFCPSTTotal = decimal.Parse(vFCPST, CultureInfo.InvariantCulture);
                                row["vFCPST"] = vFCPSTTotal.ToString("N2");
                                decimal vFCPSTRetTotal = decimal.Parse(vFCPSTRet, CultureInfo.InvariantCulture);
                                row["vFCPSTRet"] = vFCPSTRetTotal.ToString("N2");
                                decimal vProdTotal = decimal.Parse(vProd, CultureInfo.InvariantCulture);
                                row["vProd"] = vProdTotal.ToString("N2");
                                decimal vFreteTotal = decimal.Parse(vFrete, CultureInfo.InvariantCulture);
                                row["vFrete"] = vFreteTotal.ToString("N2");
                                decimal vSegTotal = decimal.Parse(vSeg, CultureInfo.InvariantCulture);
                                row["vSeg"] = vSegTotal.ToString("N2");
                                decimal vDescTotal = decimal.Parse(vDesc, CultureInfo.InvariantCulture);
                                row["vDesc"] = vDescTotal.ToString("N2");
                                decimal vIITotal = decimal.Parse(vII, CultureInfo.InvariantCulture);
                                row["vII"] = vIITotal.ToString("N2");
                                decimal vIPITotal = decimal.Parse(vIPI, CultureInfo.InvariantCulture);
                                row["vIPI"] = vIPITotal.ToString("N2");
                                decimal vIPIDevolTotal = decimal.Parse(vIPIDevol, CultureInfo.InvariantCulture);
                                row["vIPIDevol"] = vIPIDevolTotal.ToString("N2");
                                decimal vPISTotal = decimal.Parse(vPIS, CultureInfo.InvariantCulture);
                                row["vPIS"] = vPISTotal.ToString("N2");
                                decimal vCOFINSTotal = decimal.Parse(vCOFINS, CultureInfo.InvariantCulture);
                                row["vCOFINS"] = vCOFINSTotal.ToString("N2");
                                decimal vOutroTotal = decimal.Parse(vOutro, CultureInfo.InvariantCulture);
                                row["vOutro"] = vOutroTotal.ToString("N2");
                                decimal vNFSTotal = decimal.Parse(vNF, CultureInfo.InvariantCulture);
                                row["vNF"] = vNFSTotal.ToString("N2");
                                decimal vTotTribTotal = decimal.Parse(vTotTrib, CultureInfo.InvariantCulture);
                                row["vTotTrib"] = vTotTribTotal.ToString("N2");

                                row["modFrete"] = modFrete;
                                if (!string.IsNullOrWhiteSpace(transportaCNPJ) && transportaCNPJ.Length == 14)
                                {
                                    transportaCNPJ = Convert.ToUInt64(transportaCNPJ).ToString(@"00\.000\.000\/0000\-00");
                                }
                                row["transportaCNPJ"] = transportaCNPJ;
                                row["transportaxNome"] = transportaxNome;
                                row["transportaIE"] = transportaIE;
                                row["transportaxEnder"] = transportaxEnder;
                                row["transportaxMun"] = transportaxMun;
                                row["transportaUF"] = transportaUF;
                                row["qVol"] = qVol;
                                row["esp"] = esp;
                                if (pesoL != "")
                                {
                                    decimal pesoLTotal = decimal.Parse(pesoL, CultureInfo.InvariantCulture);
                                    row["pesoL"] = pesoLTotal;
                                }
                                else
                                    row["pesoL"] = "";

                                if (pesoB != "")
                                {
                                    decimal pesoBTotal = decimal.Parse(pesoB, CultureInfo.InvariantCulture);
                                    row["pesoB"] = pesoBTotal;
                                }
                                else
                                    row["pesoB"] = "";

                                row["infAdFisco"] = infAdFisco;
                                row["infCpl"] = infCpl;
                            }

                            DataTable tabela = dtProduct1.Tables[1];
                            tabela.Clear();
                            foreach (var item in itens)
                            {
                                DataRow row = tabela.NewRow();

                                row["cProd"] = item.cProd;
                                row["cEAN"] = item.cEAN;
                                row["xProd"] = item.xProd;
                                row["NCM"] = item.NCM;
                                row["CFOP"] = item.CFOP;
                                row["uCom"] = item.uCom;
                                decimal qCom = decimal.Parse(item.qCom, CultureInfo.InvariantCulture);
                                row["qCom"] = qCom.ToString("N2");
                                decimal vUnCom = decimal.Parse(item.vUnCom, CultureInfo.InvariantCulture);
                                row["vUnCom"] = vUnCom.ToString("N2");
                                decimal vProduni = decimal.Parse(item.vProd, CultureInfo.InvariantCulture);
                                row["vProduni"] = vProduni.ToString("N2");
                                row["CEST"] = item.CEST;
                                if (item.vDesc == null)
                                {
                                    row["vDescuni"] = "0,00";
                                }
                                else
                                {
                                    decimal vDescuni = decimal.Parse(item.vDesc, CultureInfo.InvariantCulture);
                                    row["vDescuni"] = vDescuni;
                                }

                                row["orig"] = item.orig;
                                row["CST"] = item.CST;

                                decimal vBCICMS = 0;
                                if (item.vBC != null)
                                    vBCICMS = decimal.Parse(item.vBC, CultureInfo.InvariantCulture);

                                row["vBCICMS"] = vBCICMS.ToString("N2");



                                decimal pICMS = 0;
                                if (item.pICMS != null)
                                    pICMS = decimal.Parse(item.pICMS, CultureInfo.InvariantCulture);
                                row["pICMS"] = pICMS.ToString("N2");


                                decimal vICMSuni = 0;
                                if (item.vICMS != null)
                                    vICMSuni = decimal.Parse(item.vICMS, CultureInfo.InvariantCulture);
                                row["vICMSuni"] = vICMSuni;

                                row["modBCST"] = item.modBCST;


                                if (item.pMVAST != null)
                                {
                                    decimal pMVAST = decimal.Parse(item.pMVAST, CultureInfo.InvariantCulture);
                                    row["pMVAST"] = pMVAST.ToString("N2");
                                }
                                else
                                {
                                    row["pMVAST"] = "0,00";
                                }

                                if (item.vBCST != null)
                                {
                                    decimal vBCSTun = decimal.Parse(item.vBCST, CultureInfo.InvariantCulture);
                                    row["vBCST"] = vBCSTun.ToString("N2");
                                }
                                else
                                    row["vBCST"] = "0,00";

                                if (item.pICMSST != null)
                                {
                                    decimal pICMSST = decimal.Parse(item.pICMSST, CultureInfo.InvariantCulture);
                                    row["pICMSST"] = pICMSST.ToString("N2");
                                }
                                else
                                    row["pICMSST"] = "0,00";

                                if (item.vICMSST != null)
                                {
                                    decimal vICMSST = decimal.Parse(item.vICMSST, CultureInfo.InvariantCulture);
                                    row["vICMSST"] = vICMSST.ToString("N2");
                                }
                                else
                                    row["vICMSST"] = "0,00";

                                if (item.vBCIPI != null)
                                {
                                    decimal vBCIPI = decimal.Parse(item.vBCIPI, CultureInfo.InvariantCulture);
                                    row["vBCIPI"] = vBCIPI.ToString("N2");
                                }
                                else
                                    row["vBCIPI"] = "0,00";
                                if (item.pIPI != null)
                                {
                                    decimal pIPI = decimal.Parse(item.pIPI, CultureInfo.InvariantCulture);
                                    row["pIPI"] = pIPI.ToString("N2");
                                }
                                else
                                    row["pIPI"] = "0,00";
                                if (item.vIPI != null)
                                {
                                    decimal vIPIuni = decimal.Parse(item.vIPI, CultureInfo.InvariantCulture);
                                    row["vIPIuni"] = vIPIuni.ToString("N2");
                                }
                                else
                                    row["vIPIuni"] = "0,00";

                                if (item.vBCPIS != null)
                                {
                                    decimal vBCPIS = decimal.Parse(item.vBCPIS, CultureInfo.InvariantCulture);
                                    row["vBCPIS"] = vBCPIS.ToString("N2");
                                }
                                else
                                    row["vBCPIS"] = "0,00";
                                if (item.pPIS != null)
                                {
                                    decimal pPIS = decimal.Parse(item.pPIS, CultureInfo.InvariantCulture);
                                    row["pPIS"] = pPIS.ToString("N2");
                                }
                                else
                                    row["pPIS"] = "0,00";
                                if (item.vPIS != null)
                                {
                                    decimal vPISuni = decimal.Parse(item.vPIS, CultureInfo.InvariantCulture);
                                    row["vPISuni"] = vPISuni.ToString("N2");
                                }
                                else
                                    row["vPISuni"] = "0,00";

                                if (item.vBCCOFINS != null)
                                {
                                    decimal vBCCOFINS = decimal.Parse(item.vBCCOFINS, CultureInfo.InvariantCulture);
                                    row["vBCCOFINS"] = vBCCOFINS;
                                }
                                else
                                    row["vBCCOFINS"] = "0,00";
                                if (item.pCOFINS != null)
                                {
                                    decimal pCOFINS = decimal.Parse(item.pCOFINS, CultureInfo.InvariantCulture);
                                    row["pCOFINS"] = pCOFINS;
                                }
                                else
                                    row["pCOFINS"] = "0,00";
                                if (item.vCOFINS != null)
                                {
                                    decimal vCOFINSuni = decimal.Parse(item.vCOFINS, CultureInfo.InvariantCulture);
                                    row["vCOFINSuni"] = vCOFINSuni;
                                }
                                else
                                    row["vCOFINSuni"] = "0,00";

                                if (item.vBCUFDest != null)
                                {
                                    decimal vBCUFDest = decimal.Parse(item.vBCUFDest, CultureInfo.InvariantCulture);
                                    row["vBCUFDest"] = vBCUFDest;
                                }
                                else
                                    row["vBCUFDest"] = "0,00";
                                if (item.pFCPUFDest != null)
                                {
                                    decimal pFCPUFDest = decimal.Parse(item.pFCPUFDest, CultureInfo.InvariantCulture);
                                    row["pFCPUFDest"] = pFCPUFDest;
                                }
                                else
                                    row["pFCPUFDest"] = "0,00";
                                if (item.pICMSUFDest != null)
                                {
                                    decimal pICMSUFDest = decimal.Parse(item.pICMSUFDest, CultureInfo.InvariantCulture);
                                    row["pICMSUFDest"] = pICMSUFDest;
                                }
                                else
                                    row["pICMSUFDest"] = "0,00";
                                if (item.pICMSInter != null)
                                {
                                    decimal pICMSInter = decimal.Parse(item.pICMSInter, CultureInfo.InvariantCulture);
                                    row["pICMSInter"] = pICMSInter;
                                }
                                else
                                    row["pICMSInter"] = "0,00";
                                if (item.pICMSInterPart != null)
                                {
                                    decimal pICMSInterPart = decimal.Parse(item.pICMSInterPart, CultureInfo.InvariantCulture);
                                    row["pICMSInterPart"] = pICMSInterPart;
                                }
                                else
                                    row["pICMSInterPart"] = "0,00";
                                if (item.vFCPUFDest != null)
                                {
                                    decimal vFCPUFDest = decimal.Parse(item.vFCPUFDest, CultureInfo.InvariantCulture);
                                    row["vFCPUFDest"] = vFCPUFDest;
                                }
                                else
                                    row["vFCPUFDest"] = "0,00";
                                if (item.vICMSUFDestuni != null)
                                {
                                    decimal vICMSUFDestuni = decimal.Parse(item.vICMSUFDestuni, CultureInfo.InvariantCulture);
                                    row["vICMSUFDestuni"] = vICMSUFDestuni;
                                }
                                else
                                    row["vICMSUFDestuni"] = "0,00";
                                if (item.vICMSUFRemet != null)
                                {
                                    decimal vICMSUFRemet = decimal.Parse(item.vICMSUFRemet, CultureInfo.InvariantCulture);
                                    row["vICMSUFRemet"] = vICMSUFRemet;
                                }
                                else
                                    row["vICMSUFRemet"] = "0,00";
                                tabela.Rows.Add(row);
                            }

                            var todosItens = dtProduct1.Tables[1];

                            int itensPorPagina = 1;

                            DataTable itensPagina1 = todosItens.Clone();
                            DataTable itensRestantes = todosItens.Clone();

                            for (int i = 0; i < todosItens.Rows.Count; i++)
                            {
                                if (i < itensPorPagina)
                                    itensPagina1.ImportRow(todosItens.Rows[i]);
                                else
                                    itensRestantes.ImportRow(todosItens.Rows[i]);
                            }

                            rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtProduct1.Tables[0]));
                            rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", itensPagina1));
                            rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet3", itensRestantes));

                            byte[] imagemBytes;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                Barcode128 codigoBarras = new Barcode128
                                {
                                    Code = chave,
                                    CodeType = Barcode.CODE128,
                                    StartStopText = true,
                                    GenerateChecksum = true,
                                    ChecksumText = true,
                                    BarHeight = 40f,
                                    X = 0.8f,
                                    Font = null
                                };

                                var imagem = codigoBarras.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White);
                                imagem.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                imagemBytes = ms.ToArray();
                            }

                            string imagemBase64 = Convert.ToBase64String(imagemBytes);
                            int Quantidade = itensRestantes.Rows.Count;

                            DateTime data = DateTime.Now;
                            ReportParameter[] rp = new ReportParameter[6];
                            rp[0] = new ReportParameter("Espaco", " ");
                            rp[1] = new ReportParameter("imagem", imagemBase64);
                            rp[2] = new ReportParameter("Data", data.ToString("dd/MM/yyyy"));
                            rp[3] = new ReportParameter("Hora", data.ToString("HH:mm:ss"));
                            rp[4] = new ReportParameter("tpAmb", tpAmb);
                            rp[5] = new ReportParameter("QuantidadeItens", Quantidade.ToString());
                            rv.LocalReport.SetParameters(rp);

                            rv.LocalReport.Refresh();

                            bool bPDF = true;
                            byte[] bytesNFe = bPDF ? rv.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings) : rv.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                            string pdfPath = "";

                            sNomeArquivoGErado = chave + "_" + sStatus + ".pdf";
                            pdfPath = sPath_Salvar + "/" + sNomeArquivoGErado;

                            if (bPDF && Quantidade == 0)
                            {
                                using (var input = new MemoryStream(bytesNFe))
                                using (var reader = new iTextSharp.text.pdf.PdfReader(input))
                                using (var tempOutput = new MemoryStream())
                                {
                                    using (var doc = new iTextSharp.text.Document(reader.GetPageSizeWithRotation(1)))
                                    using (var copy = new iTextSharp.text.pdf.PdfCopy(doc, tempOutput))
                                    {
                                        doc.Open();
                                        if (reader.NumberOfPages >= 1)
                                        {
                                            copy.AddPage(copy.GetImportedPage(reader, 1));
                                        }
                                    }

                                    byte[] onePagePdf = tempOutput.ToArray();
                                    using (var reader2 = new iTextSharp.text.pdf.PdfReader(onePagePdf))
                                    using (var output = new MemoryStream())
                                    {
                                        using (var stamper = new iTextSharp.text.pdf.PdfStamper(reader2, output))
                                        {
                                            int totalPages = reader2.NumberOfPages;

                                            for (int i = 1; i <= totalPages; i++)
                                            {
                                                PdfContentByte cb = stamper.GetOverContent(i);
                                                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                                                cb.BeginText();
                                                cb.SetFontAndSize(bf, 7);

                                                string texto = $"{i}/{totalPages}";
                                                float pageWidth = reader2.GetPageSize(i).Width;

                                                float x = (pageWidth / 2) + 7;
                                                float y = 692;

                                                cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, texto, x, y, 0);
                                                cb.EndText();
                                            }
                                        }

                                        File.WriteAllBytes(pdfPath, output.ToArray());

                                        if (sStatus != "DANFE")
                                        {
                                            AdicionarMarcaDagua(sStatus, pdfPath);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                using (var input = new MemoryStream(bytesNFe))
                                using (var reader = new iTextSharp.text.pdf.PdfReader(input))
                                using (var output = new MemoryStream())
                                {
                                    using (var stamper = new iTextSharp.text.pdf.PdfStamper(reader, output))
                                    {
                                        int totalPages = reader.NumberOfPages;

                                        for (int i = 1; i <= totalPages; i++)
                                        {
                                            PdfContentByte cb = stamper.GetOverContent(i);
                                            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                                            cb.BeginText();
                                            cb.SetFontAndSize(bf, 8);

                                            string texto = $"{i}/{totalPages}";
                                            float pageWidth = reader.GetPageSize(i).Width;

                                            float x;
                                            float y;
                                            int alignment;

                                            if (i == 1)
                                            {
                                                x = (pageWidth / 2) + 7;
                                                y = 692;
                                                alignment = PdfContentByte.ALIGN_CENTER;
                                            }
                                            else
                                            {
                                                x = (pageWidth / 2) + 13;
                                                y = 746;
                                                alignment = PdfContentByte.ALIGN_RIGHT;
                                            }

                                            cb.ShowTextAligned(alignment, texto, x, y, 0);
                                            cb.EndText();
                                        }
                                    }

                                    File.WriteAllBytes(pdfPath, output.ToArray());
                                    if (sStatus != "DANFE")
                                    {
                                        AdicionarMarcaDagua(sStatus, pdfPath);
                                    }
                                }
                                string NomeArquivo = chave + ".pdf";
                                return NomeArquivo;
                            }

                        }
                        else
                        {
                            string sDiretorioPdf = sPath_Salvar + "/" + sNomeArquivoGErado;
                            if (sStatus != "DANFE")
                            {
                                try
                                {
                                    AdicionarMarcaDagua(sStatus, sDiretorioPdf);
                                }
                                catch
                                {
                                }
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    throw new ArgumentException(ex.Message);
                }


                return sNomeArquivoGErado;
            }
        }

        public class Gerar_PDF_NFe
        {
            public static string GerarNFe(bool bPDF, string sCaminho_UniNFe, string ArquivoXML, string sImportador)
            {

                try
                {
                    decimal ParseOrZero(string valor)
                    {
                        return decimal.TryParse(valor ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out decimal resultado)
                            ? resultado
                            : 0;
                    }
                    if (ArquivoXML == "")
                    {
                        #region | ReportViewer
                        string baseDir = sCaminho_UniNFe;
                        string cnpjPattern = @"\d{14}";
                        var cnpjDirectories = Directory.GetDirectories(baseDir);

                        foreach (var cnpjDir in cnpjDirectories)
                        {
                            string dirName = Path.GetFileName(cnpjDir);
                            if (Regex.IsMatch(dirName, cnpjPattern))
                            {
                                string[] pastas = new string[]
                                {
                                Path.Combine(cnpjDir, "Enviado\\Autorizados")
                                };
                                string pastaDownloadNFe = Path.Combine(cnpjDir, "DownloadNFe");
                                foreach (var pasta in pastas)
                                {
                                    var Teste = Directory.GetDirectories(pasta);

                                    string[] PastasArquivos = new string[]
                                    {
                                 Path.Combine(Teste)
                                    };
                                    foreach (var arquivoPastas in Teste)
                                    {
                                        if (Directory.Exists(arquivoPastas))
                                        {
                                            string[] arquivos = Directory.GetFiles(arquivoPastas);
                                            string[] arquivosDownloadNFe = Directory.GetFiles(pastaDownloadNFe);

                                            var arquivosFiltrados = arquivos.Where(arquivo =>
                                            {
                                                if (arquivo.EndsWith("-procNFe.xml"))
                                                {
                                                    string pdfCorrelato = Path.ChangeExtension(arquivo.Replace("-procNFe.xml", ""), ".pdf");
                                                    return !arquivosDownloadNFe.Any(pdf => Path.GetFileName(pdf) == Path.GetFileName(pdfCorrelato));
                                                }
                                                return true;
                                            }).ToArray();

                                            foreach (var arquivo in arquivosFiltrados)
                                            {
                                                string NomeArquivo = Path.GetFileName(arquivo);

                                                if (NomeArquivo.Contains("-procNFe"))
                                                {
                                                    #region | NF-e
                                                    ReportViewer rv = new ReportViewer();
                                                    rv.ProcessingMode = ProcessingMode.Local;
                                                    rv.LocalReport.EnableExternalImages = true;
                                                    rv.LocalReport.ReportPath = "App\\Reports\\" + "NF-e.rdlc";

                                                    #region | DADOS NF-e
                                                    XDocument xml = XDocument.Load(arquivo);

                                                    string nNF = "";
                                                    string tpAmb = "";
                                                    string serie = "";
                                                    string dhEmi = "";
                                                    string CNPJ = "";
                                                    string xNome = "";
                                                    string xLgr = "";
                                                    string nro = "";
                                                    string xBairro = "";
                                                    string CEP = "";
                                                    string xMun = "";
                                                    string UF = "";
                                                    string IE = "";
                                                    string mod = "";
                                                    string DESTxNome = "";
                                                    string DESTCNPJ = "";
                                                    string DESTxLgr = "";
                                                    string DESTnro = "";
                                                    string DESTxCpl = "";
                                                    string DESTxBairro = "";
                                                    string DESTCEP = "";
                                                    string DESTxMun = "";
                                                    string DESTUF = "";
                                                    string DESIE = "";
                                                    string fone = "";
                                                    string vNF = "";
                                                    string vBC = "";
                                                    string vICMS = "";
                                                    string vBCST = "";
                                                    string vST = "";
                                                    string vProd = "";
                                                    string chave = "";
                                                    string natOp = "";
                                                    string nProt = "";
                                                    string dhRecbto = "";
                                                    string dhSaiEnt = "";
                                                    string horaSaiEnt = "";
                                                    string vFrete = "";
                                                    string vSeg = "";
                                                    string vDesc = "";
                                                    string vOutro = "";
                                                    string vIPI = "";
                                                    string vTotTrib = "";
                                                    string vICMSUFDest = "";
                                                    string vICMSDeson = "";
                                                    string vFCP = "";
                                                    string vFCPST = "";
                                                    string vFCPSTRet = "";
                                                    string vIPIDevol = "";
                                                    string vPIS = "";
                                                    string vCOFINS = "";
                                                    string modFrete = "";
                                                    string transportaCNPJ = "";
                                                    string transportaxNome = "";
                                                    string transportaIE = "";
                                                    string transportaxEnder = "";
                                                    string transportaxMun = "";
                                                    string transportaUF = "";
                                                    string qVol = "";
                                                    string esp = "";
                                                    string pesoL = "";
                                                    string pesoB = "";
                                                    string infAdFisco = "";
                                                    string infCpl = "";
                                                    string vII = "";

                                                    var infNFeElement = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}infNFe").FirstOrDefault();

                                                    XNamespace ns = "http://www.portalfiscal.inf.br/nfe";
                                                    var infProt = xml.Descendants(ns + "infProt").FirstOrDefault();

                                                    if (infProt != null)
                                                    {
                                                        nProt = infProt.Element(ns + "nProt")?.Value;

                                                        if (DateTimeOffset.TryParse(infProt.Element(ns + "dhRecbto")?.Value, out var dhRecebto))
                                                        {
                                                            dhRecbto = dhRecebto.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
                                                        }
                                                    }

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}natOp").FirstOrDefault() != null)
                                                        natOp = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}natOp").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}nNF").FirstOrDefault() != null)
                                                        nNF = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}nNF").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpAmb").FirstOrDefault() != null)
                                                        tpAmb = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpAmb").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}serie").FirstOrDefault() != null)
                                                        serie = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}serie").FirstOrDefault()?.Value;

                                                    var dhEmiElement = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide")
                                                          .Descendants("{http://www.portalfiscal.inf.br/nfe}dhEmi")
                                                          .FirstOrDefault();

                                                    if (dhEmiElement != null)
                                                    {
                                                        DateTime dataEmissao;
                                                        if (DateTime.TryParse(dhEmiElement.Value, out dataEmissao))
                                                        {
                                                            dhEmi = dataEmissao.ToString("dd/MM/yyyy");
                                                        }
                                                    }

                                                    var dhSaiEntElement = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}dhSaiEnt").FirstOrDefault();

                                                    if (dhSaiEntElement != null)
                                                    {
                                                        DateTime dataSaida;
                                                        if (DateTime.TryParse(dhSaiEntElement.Value, out dataSaida))
                                                        {
                                                            dhSaiEnt = dataSaida.ToString("dd/MM/yyyy");
                                                            horaSaiEnt = dataSaida.ToString("HH:mm:ss");
                                                        }
                                                    }

                                                    if (infNFeElement != null)
                                                    {
                                                        chave = infNFeElement.Attribute("Id")?.Value?.Substring(3);
                                                    }
                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vProd").FirstOrDefault() != null)
                                                        vProd = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vProd").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMSDeson").FirstOrDefault() != null)
                                                        vICMSDeson = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMSDeson").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCP").FirstOrDefault() != null)
                                                        vFCP = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCP").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault() != null)
                                                        vPIS = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault() != null)
                                                        vCOFINS = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCPST").FirstOrDefault() != null)
                                                        vFCPST = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCPST").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMSUFDest").FirstOrDefault() != null)
                                                        vICMSUFDest = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMSUFDest").FirstOrDefault()?.Value;
                                                    else
                                                        vICMSUFDest = "0,00";

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault() != null)
                                                        vII = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault() != null)
                                                        vIPI = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vTotTrib").FirstOrDefault() != null)
                                                        vTotTrib = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vTotTrib").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vDesc").FirstOrDefault() != null)
                                                        vDesc = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vDesc").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPIDevol").FirstOrDefault() != null)
                                                        vIPIDevol = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPIDevol").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCPSTRet").FirstOrDefault() != null)
                                                        vFCPSTRet = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCPSTRet").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vOutro").FirstOrDefault() != null)
                                                        vOutro = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vOutro").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFrete").FirstOrDefault() != null)
                                                        vFrete = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFrete").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vSeg").FirstOrDefault() != null)
                                                        vSeg = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vSeg").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                                                        vBC = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault() != null)
                                                        vICMS = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vNF").FirstOrDefault() != null)
                                                        vNF = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vNF").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vBCST").FirstOrDefault() != null)
                                                        vBCST = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vBCST").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vST").FirstOrDefault() != null)
                                                        vST = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vST").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                                                        CNPJ = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                                                        xNome = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault() != null)
                                                        xLgr = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}fone").FirstOrDefault() != null)
                                                        fone = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}fone").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault() != null)
                                                        nro = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault() != null)
                                                        xBairro = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault() != null)
                                                        CEP = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault() != null)
                                                        xMun = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault() != null)
                                                        UF = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault() != null)
                                                        IE = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}mod").FirstOrDefault() != null)
                                                        mod = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}mod").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                                                        DESTCNPJ = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CPF").FirstOrDefault() != null)
                                                        DESTCNPJ = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CPF").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                                                        DESTxNome = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault() != null)
                                                        DESTxLgr = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault() != null)
                                                        DESTnro = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xCpl").FirstOrDefault() != null)
                                                        DESTxCpl = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xCpl").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault() != null)
                                                        DESTxBairro = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault() != null)
                                                        DESTCEP = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault() != null)
                                                        DESTxMun = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault() != null)
                                                        DESTUF = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault() != null)
                                                        DESIE = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}modFrete").FirstOrDefault() != null)
                                                        modFrete = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}modFrete").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                                                        transportaCNPJ = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                                                        transportaxNome = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault() != null)
                                                        transportaIE = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xEnder").FirstOrDefault() != null)
                                                        transportaxEnder = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xEnder").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault() != null)
                                                        transportaxMun = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault() != null)
                                                        transportaUF = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}qVol").FirstOrDefault() != null)
                                                        qVol = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}qVol").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}esp").FirstOrDefault() != null)
                                                        esp = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}esp").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoL").FirstOrDefault() != null)
                                                        pesoL = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoL").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoB").FirstOrDefault() != null)
                                                        pesoB = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoB").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").Descendants("{http://www.portalfiscal.inf.br/nfe}infCpl").FirstOrDefault() != null)
                                                        infCpl = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").Descendants("{http://www.portalfiscal.inf.br/nfe}infCpl").FirstOrDefault()?.Value;

                                                    if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").Descendants("{http://www.portalfiscal.inf.br/nfe}infAdFisco").FirstOrDefault() != null)
                                                        infAdFisco = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").Descendants("{http://www.portalfiscal.inf.br/nfe}infAdFisco").FirstOrDefault()?.Value;

                                                    var itens = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Select(d => new
                                                    {
                                                        nItem = d.Attribute("nItem")?.Value,
                                                        cProd = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}cProd").FirstOrDefault()?.Value,
                                                        cEAN = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}cEAN").FirstOrDefault()?.Value,
                                                        xProd = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}xProd").FirstOrDefault()?.Value,
                                                        NCM = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}NCM").FirstOrDefault()?.Value,
                                                        CFOP = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}CFOP").FirstOrDefault()?.Value,
                                                        uCom = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}uCom").FirstOrDefault()?.Value,
                                                        qCom = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}qCom").FirstOrDefault()?.Value,
                                                        vUnCom = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}vUnCom").FirstOrDefault()?.Value,
                                                        vProd = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}vProd").FirstOrDefault()?.Value,
                                                        cEANTrib = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}cEANTrib").FirstOrDefault()?.Value,
                                                        uTrib = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}uTrib").FirstOrDefault()?.Value,
                                                        qTrib = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}qTrib").FirstOrDefault()?.Value,
                                                        vUnTrib = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}vUnTrib").FirstOrDefault()?.Value,
                                                        indTot = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}indTot").FirstOrDefault()?.Value,
                                                        xPed = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}xPed").FirstOrDefault()?.Value,
                                                        CEST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}CEST").FirstOrDefault()?.Value,
                                                        vDesc = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}vDesc").FirstOrDefault()?.Value,

                                                        orig = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "orig").FirstOrDefault()?.Value,
                                                        CST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "CST").FirstOrDefault()?.Value,
                                                        vBC = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "vBC").FirstOrDefault()?.Value,
                                                        pICMS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "pICMS").FirstOrDefault()?.Value,
                                                        vICMS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "vICMS").FirstOrDefault()?.Value,
                                                        modBCST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "modBCST").FirstOrDefault()?.Value,
                                                        pMVAST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "pMVAST").FirstOrDefault()?.Value,
                                                        vBCST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "vBCST").FirstOrDefault()?.Value,
                                                        pICMSST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "pICMSST").FirstOrDefault()?.Value,
                                                        vICMSST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "vICMSST").FirstOrDefault()?.Value,

                                                        vBCIPI = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants().Where(x => x.Name.LocalName == "vBC").FirstOrDefault()?.Value,
                                                        pIPI = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants().Where(x => x.Name.LocalName == "pIPI").FirstOrDefault()?.Value,
                                                        vIPI = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants().Where(x => x.Name.LocalName == "vIPI").FirstOrDefault()?.Value,

                                                        vBCPIS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants().Where(x => x.Name.LocalName == "vBC").FirstOrDefault()?.Value,
                                                        pPIS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants().Where(x => x.Name.LocalName == "pPIS").FirstOrDefault()?.Value,
                                                        vPIS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants().Where(x => x.Name.LocalName == "vPIS").FirstOrDefault()?.Value,

                                                        vBCCOFINS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants().Where(x => x.Name.LocalName == "vBC").FirstOrDefault()?.Value,
                                                        pCOFINS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants().Where(x => x.Name.LocalName == "pCOFINS").FirstOrDefault()?.Value,
                                                        vCOFINS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants().Where(x => x.Name.LocalName == "vCOFINS").FirstOrDefault()?.Value,

                                                        vBCUFDest = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "vBCUFDest").FirstOrDefault()?.Value,
                                                        pFCPUFDest = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "pFCPUFDest").FirstOrDefault()?.Value,
                                                        pICMSUFDest = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "pICMSUFDest").FirstOrDefault()?.Value,
                                                        pICMSInter = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "pICMSInter").FirstOrDefault()?.Value,
                                                        pICMSInterPart = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "pICMSInterPart").FirstOrDefault()?.Value,
                                                        vICMSUFDestuni = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "vICMSUFDest").FirstOrDefault()?.Value,
                                                        vICMSUFRemet = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "vICMSUFRemet").FirstOrDefault()?.Value,
                                                        vFCPUFDest = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "vFCPUFDest").FirstOrDefault()?.Value,
                                                    }).ToList();
                                                    #endregion

                                                    #region | POPULA TABELA
                                                    Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>
                                                    {

                                                    };
                                                    DataSet dtProduct1 = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_NFe", vParametrosProduct);

                                                    if (modFrete != "")
                                                    {
                                                        if (modFrete == "0")
                                                            modFrete = "0 - " + "Remetente";
                                                        if (modFrete == "1")
                                                            modFrete = "1 - " + "Destinatário";
                                                        if (modFrete == "2")
                                                            modFrete = "2 - " + "Terceiros";
                                                        if (modFrete == "3")
                                                            modFrete = "3 - " + "próp Remetente";
                                                        if (modFrete == "4")
                                                            modFrete = "4 - " + "próp Destinatário";
                                                        if (modFrete == "9")
                                                            modFrete = "9 - " + "Sem transporte";
                                                    }

                                                    foreach (DataRow row in dtProduct1.Tables[0].Rows)
                                                    {
                                                        row["nNF"] = int.Parse(nNF).ToString("D9").Insert(3, ".").Insert(7, ".");
                                                        row["serie"] = Convert.ToInt32(serie).ToString("D3");
                                                        row["chave"] = chave;
                                                        row["dhEmi"] = dhEmi;
                                                        row["dhSaiEnt"] = dhSaiEnt;
                                                        row["nProt"] = nProt;
                                                        row["natOp"] = natOp;
                                                        row["dhRecbto"] = dhRecbto;
                                                        row["horaSaiEnt"] = horaSaiEnt;

                                                        if (!string.IsNullOrWhiteSpace(CNPJ))
                                                        {
                                                            CNPJ = CNPJ.Trim();

                                                            if (CNPJ.Length == 14)
                                                            {
                                                                CNPJ = Convert.ToUInt64(CNPJ).ToString(@"00\.000\.000\/0000\-00");
                                                            }
                                                            else if (CNPJ.Length == 11)
                                                            {
                                                                CNPJ = Convert.ToUInt64(CNPJ).ToString(@"000\.000\.000\-00");
                                                            }
                                                        }
                                                        row["CNPJ"] = CNPJ;
                                                        row["xNome"] = xNome;
                                                        row["xLgr"] = xLgr;
                                                        row["nro"] = nro;
                                                        row["xBairro"] = xBairro;
                                                        row["xMun"] = xMun;
                                                        row["UF"] = UF;
                                                        if (!string.IsNullOrWhiteSpace(CEP) && CEP.Length == 8)
                                                        {
                                                            CEP = Convert.ToUInt64(CEP).ToString(@"00000\-000");
                                                        }
                                                        row["CEP"] = CEP;
                                                        row["fone"] = fone;
                                                        row["IE"] = IE;

                                                        if (!string.IsNullOrWhiteSpace(DESTCNPJ))
                                                        {
                                                            DESTCNPJ = DESTCNPJ.Trim();

                                                            if (CNPJ.Length == 14)
                                                            {
                                                                DESTCNPJ = Convert.ToUInt64(DESTCNPJ).ToString(@"00\.000\.000\/0000\-00");
                                                            }
                                                            else if (DESTCNPJ.Length == 11)
                                                            {
                                                                DESTCNPJ = Convert.ToUInt64(DESTCNPJ).ToString(@"000\.000\.000\-00");
                                                            }
                                                        }
                                                        row["destCNPJ"] = DESTCNPJ;
                                                        row["destxNome"] = DESTxNome;
                                                        row["destxLgr"] = DESTxLgr;
                                                        row["destnro"] = DESTnro;
                                                        row["destxCpl"] = DESTxCpl;
                                                        row["destxBairro"] = DESTxBairro;
                                                        //row["destcMun"] = IE;
                                                        row["destxMun"] = DESTxMun;
                                                        row["destUF"] = DESTUF;
                                                        if (!string.IsNullOrWhiteSpace(DESTCEP) && DESTCEP.Length == 8)
                                                        {
                                                            DESTCEP = Convert.ToUInt64(DESTCEP).ToString(@"00000\-000");
                                                        }
                                                        row["destCEP"] = DESTCEP;
                                                        //row["destxPais"] = IE;
                                                        row["destIE"] = DESIE;



                                                        row["vBC"] = ParseOrZero(vBC).ToString("N2");
                                                        row["vICMS"] = ParseOrZero(vICMS).ToString("N2");
                                                        row["vICMSDeson"] = ParseOrZero(vICMSDeson).ToString("N2");
                                                        row["vICMSUFDest"] = ParseOrZero(vICMSUFDest).ToString("N2");
                                                        row["vFCP"] = ParseOrZero(vFCP).ToString("N2");
                                                        row["vBCST"] = ParseOrZero(vBCST).ToString("N2");
                                                        row["vST"] = ParseOrZero(vST).ToString("N2");
                                                        row["vFCPST"] = ParseOrZero(vFCPST).ToString("N2");
                                                        row["vFCPSTRet"] = ParseOrZero(vFCPSTRet).ToString("N2");
                                                        row["vProd"] = ParseOrZero(vProd).ToString("N2");
                                                        row["vFrete"] = ParseOrZero(vFrete).ToString("N2");
                                                        row["vSeg"] = ParseOrZero(vSeg).ToString("N2");
                                                        row["vDesc"] = ParseOrZero(vDesc).ToString("N2");
                                                        row["vII"] = ParseOrZero(vII).ToString("N2");
                                                        row["vIPI"] = ParseOrZero(vIPI).ToString("N2");
                                                        row["vIPIDevol"] = ParseOrZero(vIPIDevol).ToString("N2");
                                                        row["vPIS"] = ParseOrZero(vPIS).ToString("N2");
                                                        row["vCOFINS"] = ParseOrZero(vCOFINS).ToString("N2");
                                                        row["vOutro"] = ParseOrZero(vOutro).ToString("N2");
                                                        row["vNF"] = ParseOrZero(vNF).ToString("N2");
                                                        row["vTotTrib"] = ParseOrZero(vTotTrib).ToString("N2");

                                                        row["modFrete"] = modFrete;

                                                        if (!string.IsNullOrWhiteSpace(transportaCNPJ) && transportaCNPJ.Length == 14)
                                                        {
                                                            transportaCNPJ = Convert.ToUInt64(transportaCNPJ).ToString(@"00\.000\.000\/0000\-00");
                                                        }
                                                        row["transportaCNPJ"] = transportaCNPJ;
                                                        row["transportaxNome"] = transportaxNome;
                                                        row["transportaIE"] = transportaIE;
                                                        row["transportaxEnder"] = transportaxEnder;
                                                        row["transportaxMun"] = transportaxMun;
                                                        row["transportaUF"] = transportaUF;
                                                        row["qVol"] = qVol;
                                                        row["esp"] = esp;

                                                        if (!string.IsNullOrWhiteSpace(pesoL))
                                                        {
                                                            decimal pesoLTotal = ParseOrZero(pesoL);
                                                            row["pesoL"] = pesoLTotal;
                                                        }
                                                        else
                                                        {
                                                            row["pesoL"] = "";
                                                        }

                                                        // peso bruto
                                                        if (!string.IsNullOrWhiteSpace(pesoB))
                                                        {
                                                            decimal pesoBTotal = ParseOrZero(pesoB);
                                                            row["pesoB"] = pesoBTotal;
                                                        }
                                                        else
                                                        {
                                                            row["pesoB"] = "";
                                                        }

                                                        row["infAdFisco"] = infAdFisco;
                                                        row["infCpl"] = infCpl;
                                                    }

                                                    DataTable tabela = dtProduct1.Tables[1];
                                                    tabela.Clear();

                                                    foreach (var item in itens)
                                                    {
                                                        DataRow row = tabela.NewRow();

                                                        row["cProd"] = item.cProd;
                                                        row["cEAN"] = item.cEAN;
                                                        row["xProd"] = item.xProd;
                                                        row["NCM"] = item.NCM;
                                                        row["CFOP"] = item.CFOP;
                                                        row["uCom"] = item.uCom;

                                                        // Quantidade e valores unitários/total
                                                        row["qCom"] = ParseOrZero(item.qCom).ToString("N2");
                                                        row["vUnCom"] = ParseOrZero(item.vUnCom).ToString("N2");
                                                        row["vProduni"] = ParseOrZero(item.vProd).ToString("N2");

                                                        row["CEST"] = item.CEST;

                                                        // Desconto unitário
                                                        row["vDescuni"] = string.IsNullOrWhiteSpace(item.vDesc)
                                                            ? "0,00"
                                                            : ParseOrZero(item.vDesc).ToString("N2");

                                                        row["orig"] = item.orig;
                                                        row["CST"] = item.CST;

                                                        // ICMS
                                                        row["vBCICMS"] = ParseOrZero(item.vBC).ToString("N2");
                                                        row["pICMS"] = ParseOrZero(item.pICMS).ToString("N2");
                                                        row["vICMSuni"] = ParseOrZero(item.vICMS);

                                                        row["modBCST"] = item.modBCST;

                                                        // ST e outros impostos
                                                        row["pMVAST"] = ParseOrZero(item.pMVAST).ToString("N2");
                                                        row["vBCST"] = ParseOrZero(item.vBCST).ToString("N2");
                                                        row["pICMSST"] = ParseOrZero(item.pICMSST).ToString("N2");
                                                        row["vICMSST"] = ParseOrZero(item.vICMSST).ToString("N2");

                                                        row["vBCIPI"] = ParseOrZero(item.vBCIPI).ToString("N2");
                                                        row["pIPI"] = ParseOrZero(item.pIPI).ToString("N2");
                                                        row["vIPIuni"] = ParseOrZero(item.vIPI).ToString("N2");

                                                        row["vBCPIS"] = ParseOrZero(item.vBCPIS).ToString("N2");
                                                        row["pPIS"] = ParseOrZero(item.pPIS).ToString("N2");
                                                        row["vPISuni"] = ParseOrZero(item.vPIS).ToString("N2");

                                                        row["vBCCOFINS"] = ParseOrZero(item.vBCCOFINS);
                                                        row["pCOFINS"] = ParseOrZero(item.pCOFINS);
                                                        row["vCOFINSuni"] = ParseOrZero(item.vCOFINS);

                                                        // ICMS UF Destino / FCP
                                                        row["vBCUFDest"] = ParseOrZero(item.vBCUFDest).ToString("N2");
                                                        row["pFCPUFDest"] = ParseOrZero(item.pFCPUFDest).ToString("N2");
                                                        row["pICMSUFDest"] = ParseOrZero(item.pICMSUFDest).ToString("N2");
                                                        row["pICMSInter"] = ParseOrZero(item.pICMSInter).ToString("N2");
                                                        row["pICMSInterPart"] = ParseOrZero(item.pICMSInterPart).ToString("N2");
                                                        row["vFCPUFDest"] = ParseOrZero(item.vFCPUFDest).ToString("N2");
                                                        row["vICMSUFDestuni"] = ParseOrZero(item.vICMSUFDestuni).ToString("N2");
                                                        row["vICMSUFRemet"] = ParseOrZero(item.vICMSUFRemet).ToString("N2");

                                                        tabela.Rows.Add(row);
                                                    }

                                                    var todosItens = dtProduct1.Tables[1];

                                                    int itensPorPagina = 15;

                                                    DataTable itensPagina1 = todosItens.Clone();
                                                    DataTable itensRestantes = todosItens.Clone();

                                                    for (int i = 0; i < todosItens.Rows.Count; i++)
                                                    {
                                                        if (i < itensPorPagina)
                                                            itensPagina1.ImportRow(todosItens.Rows[i]);
                                                        else
                                                            itensRestantes.ImportRow(todosItens.Rows[i]);
                                                    }

                                                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtProduct1.Tables[0]));
                                                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", itensPagina1));
                                                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet3", itensRestantes));

                                                    byte[] imagemBytes;
                                                    using (MemoryStream ms = new MemoryStream())
                                                    {
                                                        Barcode128 codigoBarras = new Barcode128
                                                        {
                                                            Code = chave,
                                                            CodeType = Barcode.CODE128,
                                                            StartStopText = true,
                                                            GenerateChecksum = true,
                                                            ChecksumText = true,
                                                            BarHeight = 40f,
                                                            X = 0.8f,
                                                            Font = null
                                                        };

                                                        var imagem = codigoBarras.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White);
                                                        imagem.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                                        imagemBytes = ms.ToArray();
                                                    }

                                                    string imagemBase64 = Convert.ToBase64String(imagemBytes);
                                                    int Quantidade = itensRestantes.Rows.Count;

                                                    DateTime data = DateTime.Now;
                                                    ReportParameter[] rp = new ReportParameter[6];
                                                    rp[0] = new ReportParameter("Espaco", " ");
                                                    rp[1] = new ReportParameter("imagem", imagemBase64);
                                                    rp[2] = new ReportParameter("Data", data.ToString("dd/MM/yyyy"));
                                                    rp[3] = new ReportParameter("Hora", data.ToString("HH:mm:ss"));
                                                    rp[4] = new ReportParameter("tpAmb", tpAmb);
                                                    rp[5] = new ReportParameter("QuantidadeItens", Quantidade.ToString());
                                                    rv.LocalReport.SetParameters(rp);

                                                    rv.LocalReport.Refresh();
                                                    #endregion

                                                    byte[] bytesNFe = bPDF ? rv.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings) : rv.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                                                    string pdfPath = "C:\\Unimake\\UniNFe\\" + dirName + "\\DownloadNFe\\" + NomeArquivo.Replace("-procNFe.xml", "") + ".pdf";

                                                    if (bPDF && Quantidade == 0)
                                                    {
                                                        using (var input = new MemoryStream(bytesNFe))
                                                        using (var reader = new iTextSharp.text.pdf.PdfReader(input))
                                                        using (var tempOutput = new MemoryStream())
                                                        {
                                                            using (var doc = new iTextSharp.text.Document(reader.GetPageSizeWithRotation(1)))
                                                            using (var copy = new iTextSharp.text.pdf.PdfCopy(doc, tempOutput))
                                                            {
                                                                doc.Open();
                                                                if (reader.NumberOfPages >= 1)
                                                                {
                                                                    copy.AddPage(copy.GetImportedPage(reader, 1));
                                                                }
                                                            }

                                                            byte[] onePagePdf = tempOutput.ToArray();
                                                            using (var reader2 = new iTextSharp.text.pdf.PdfReader(onePagePdf))
                                                            using (var output = new MemoryStream())
                                                            {
                                                                using (var stamper = new iTextSharp.text.pdf.PdfStamper(reader2, output))
                                                                {
                                                                    int totalPages = reader2.NumberOfPages;

                                                                    for (int i = 1; i <= totalPages; i++)
                                                                    {
                                                                        PdfContentByte cb = stamper.GetOverContent(i);
                                                                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                                                                        cb.BeginText();
                                                                        cb.SetFontAndSize(bf, 7);

                                                                        string texto = $"{i}/{totalPages}";
                                                                        float pageWidth = reader2.GetPageSize(i).Width;

                                                                        float x = (pageWidth / 2) + 7;
                                                                        float y = 692;

                                                                        cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, texto, x, y, 0);
                                                                        cb.EndText();
                                                                    }
                                                                }

                                                                File.WriteAllBytes(pdfPath, output.ToArray());
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        using (var input = new MemoryStream(bytesNFe))
                                                        using (var reader = new iTextSharp.text.pdf.PdfReader(input))
                                                        using (var output = new MemoryStream())
                                                        {
                                                            using (var stamper = new iTextSharp.text.pdf.PdfStamper(reader, output))
                                                            {
                                                                int totalPages = reader.NumberOfPages;

                                                                for (int i = 1; i <= totalPages; i++)
                                                                {
                                                                    PdfContentByte cb = stamper.GetOverContent(i);
                                                                    BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                                                                    cb.BeginText();
                                                                    cb.SetFontAndSize(bf, 8);

                                                                    string texto = $"{i}/{totalPages}";
                                                                    float pageWidth = reader.GetPageSize(i).Width;

                                                                    float x;
                                                                    float y;
                                                                    int alignment;

                                                                    if (i == 1)
                                                                    {
                                                                        x = (pageWidth / 2) + 7;
                                                                        y = 692;
                                                                        alignment = PdfContentByte.ALIGN_CENTER;
                                                                    }
                                                                    else
                                                                    {
                                                                        x = (pageWidth / 2) + 13;
                                                                        y = 746;
                                                                        alignment = PdfContentByte.ALIGN_RIGHT;
                                                                    }

                                                                    cb.ShowTextAligned(alignment, texto, x, y, 0);
                                                                    cb.EndText();
                                                                }
                                                            }

                                                            File.WriteAllBytes(pdfPath, output.ToArray());
                                                        }

                                                    }
                                                    #endregion
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        return "";
                    }
                    else
                    {
                        ReportViewer rv = new ReportViewer();

                        rv.ProcessingMode = ProcessingMode.Local;
                        rv.LocalReport.EnableExternalImages = true;
                        rv.LocalReport.ReportPath = "App\\Reports\\" + "NF-e.rdlc";

                        #region | DADOS NF-e
                        XDocument xml = XDocument.Load(ArquivoXML);

                        string nNF = "";
                        string tpAmb = "";
                        string serie = "";
                        string dhEmi = "";
                        string CNPJ = "";
                        string xNome = "";
                        string xLgr = "";
                        string nro = "";
                        string xBairro = "";
                        string CEP = "";
                        string xMun = "";
                        string UF = "";
                        string IE = "";
                        string mod = "";
                        string DESTxNome = "";
                        string DESTCNPJ = "";
                        string DESTxLgr = "";
                        string DESTnro = "";
                        string DESTxCpl = "";
                        string DESTxBairro = "";
                        string DESTCEP = "";
                        string DESTxMun = "";
                        string DESTUF = "";
                        string DESIE = "";
                        string fone = "";
                        string vNF = "";
                        string vBC = "";
                        string vICMS = "";
                        string vBCST = "";
                        string vST = "";
                        string vProd = "";
                        string chave = "";
                        string natOp = "";
                        string nProt = "";
                        string dhRecbto = "";
                        string dhSaiEnt = "";
                        string horaSaiEnt = "";
                        string vFrete = "";
                        string vSeg = "";
                        string vDesc = "";
                        string vOutro = "";
                        string vIPI = "";
                        string vTotTrib = "";
                        string vICMSUFDest = "";
                        string vICMSDeson = "";
                        string vFCP = "";
                        string vFCPST = "";
                        string vFCPSTRet = "";
                        string vIPIDevol = "";
                        string vPIS = "";
                        string vCOFINS = "";
                        string modFrete = "";
                        string transportaCNPJ = "";
                        string transportaxNome = "";
                        string transportaIE = "";
                        string transportaxEnder = "";
                        string transportaxMun = "";
                        string transportaUF = "";
                        string qVol = "";
                        string esp = "";
                        string pesoL = "";
                        string pesoB = "";
                        string infAdFisco = "";
                        string infCpl = "";
                        string vII = "";

                        var infNFeElement = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}infNFe").FirstOrDefault();

                        XNamespace ns = "http://www.portalfiscal.inf.br/nfe";
                        var infProt = xml.Descendants(ns + "infProt").FirstOrDefault();

                        if (infProt != null)
                        {
                            nProt = infProt.Element(ns + "nProt")?.Value;

                            if (DateTimeOffset.TryParse(infProt.Element(ns + "dhRecbto")?.Value, out var dhRecebto))
                            {
                                dhRecbto = dhRecebto.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
                            }
                        }

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}natOp").FirstOrDefault() != null)
                            natOp = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}natOp").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}nNF").FirstOrDefault() != null)
                            nNF = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}nNF").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpAmb").FirstOrDefault() != null)
                            tpAmb = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpAmb").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}serie").FirstOrDefault() != null)
                            serie = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}serie").FirstOrDefault()?.Value;

                        var dhEmiElement = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide")
                              .Descendants("{http://www.portalfiscal.inf.br/nfe}dhEmi")
                              .FirstOrDefault();

                        if (dhEmiElement != null)
                        {
                            DateTime dataEmissao;
                            if (DateTime.TryParse(dhEmiElement.Value, out dataEmissao))
                            {
                                dhEmi = dataEmissao.ToString("dd/MM/yyyy");
                            }
                        }

                        var dhSaiEntElement = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}dhSaiEnt").FirstOrDefault();

                        if (dhSaiEntElement != null)
                        {
                            DateTime dataSaida;
                            if (DateTime.TryParse(dhSaiEntElement.Value, out dataSaida))
                            {
                                dhSaiEnt = dataSaida.ToString("dd/MM/yyyy");
                                horaSaiEnt = dataSaida.ToString("HH:mm:ss");
                            }
                        }

                        if (infNFeElement != null)
                        {
                            chave = infNFeElement.Attribute("Id")?.Value?.Substring(3);
                        }
                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vProd").FirstOrDefault() != null)
                            vProd = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vProd").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMSDeson").FirstOrDefault() != null)
                            vICMSDeson = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMSDeson").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCP").FirstOrDefault() != null)
                            vFCP = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCP").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault() != null)
                            vPIS = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault() != null)
                            vCOFINS = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCPST").FirstOrDefault() != null)
                            vFCPST = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCPST").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMSUFDest").FirstOrDefault() != null)
                            vICMSUFDest = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMSUFDest").FirstOrDefault()?.Value;
                        else
                            vICMSUFDest = "0,00";

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault() != null)
                            vII = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault() != null)
                            vIPI = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vTotTrib").FirstOrDefault() != null)
                            vTotTrib = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vTotTrib").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vDesc").FirstOrDefault() != null)
                            vDesc = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vDesc").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPIDevol").FirstOrDefault() != null)
                            vIPIDevol = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPIDevol").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCPSTRet").FirstOrDefault() != null)
                            vFCPSTRet = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCPSTRet").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vOutro").FirstOrDefault() != null)
                            vOutro = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vOutro").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFrete").FirstOrDefault() != null)
                            vFrete = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFrete").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vSeg").FirstOrDefault() != null)
                            vSeg = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vSeg").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                            vBC = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault() != null)
                            vICMS = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vNF").FirstOrDefault() != null)
                            vNF = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vNF").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vBCST").FirstOrDefault() != null)
                            vBCST = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vBCST").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vST").FirstOrDefault() != null)
                            vST = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vST").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                            CNPJ = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                            xNome = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault() != null)
                            xLgr = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}fone").FirstOrDefault() != null)
                            fone = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}fone").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault() != null)
                            nro = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault() != null)
                            xBairro = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault() != null)
                            CEP = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault() != null)
                            xMun = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault() != null)
                            UF = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault() != null)
                            IE = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}mod").FirstOrDefault() != null)
                            mod = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}mod").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                            DESTCNPJ = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CPF").FirstOrDefault() != null)
                            DESTCNPJ = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CPF").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                            DESTxNome = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault() != null)
                            DESTxLgr = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault() != null)
                            DESTnro = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xCpl").FirstOrDefault() != null)
                            DESTxCpl = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xCpl").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault() != null)
                            DESTxBairro = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault() != null)
                            DESTCEP = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault() != null)
                            DESTxMun = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault() != null)
                            DESTUF = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault() != null)
                            DESIE = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}modFrete").FirstOrDefault() != null)
                            modFrete = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}modFrete").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                            transportaCNPJ = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                            transportaxNome = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault() != null)
                            transportaIE = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xEnder").FirstOrDefault() != null)
                            transportaxEnder = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xEnder").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault() != null)
                            transportaxMun = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault() != null)
                            transportaUF = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}qVol").FirstOrDefault() != null)
                            qVol = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}qVol").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}esp").FirstOrDefault() != null)
                            esp = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}esp").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoL").FirstOrDefault() != null)
                            pesoL = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoL").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoB").FirstOrDefault() != null)
                            pesoB = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoB").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").Descendants("{http://www.portalfiscal.inf.br/nfe}infCpl").FirstOrDefault() != null)
                            infCpl = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").Descendants("{http://www.portalfiscal.inf.br/nfe}infCpl").FirstOrDefault()?.Value;

                        if (xml.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").Descendants("{http://www.portalfiscal.inf.br/nfe}infAdFisco").FirstOrDefault() != null)
                            infAdFisco = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").Descendants("{http://www.portalfiscal.inf.br/nfe}infAdFisco").FirstOrDefault()?.Value;

                        var itens = xml.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Select(d => new
                        {
                            nItem = d.Attribute("nItem")?.Value,
                            cProd = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}cProd").FirstOrDefault()?.Value,
                            cEAN = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}cEAN").FirstOrDefault()?.Value,
                            xProd = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}xProd").FirstOrDefault()?.Value,
                            NCM = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}NCM").FirstOrDefault()?.Value,
                            CFOP = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}CFOP").FirstOrDefault()?.Value,
                            uCom = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}uCom").FirstOrDefault()?.Value,
                            qCom = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}qCom").FirstOrDefault()?.Value,
                            vUnCom = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}vUnCom").FirstOrDefault()?.Value,
                            vProd = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}vProd").FirstOrDefault()?.Value,
                            cEANTrib = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}cEANTrib").FirstOrDefault()?.Value,
                            uTrib = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}uTrib").FirstOrDefault()?.Value,
                            qTrib = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}qTrib").FirstOrDefault()?.Value,
                            vUnTrib = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}vUnTrib").FirstOrDefault()?.Value,
                            indTot = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}indTot").FirstOrDefault()?.Value,
                            xPed = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}xPed").FirstOrDefault()?.Value,
                            CEST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}CEST").FirstOrDefault()?.Value,
                            vDesc = d.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}vDesc").FirstOrDefault()?.Value,

                            orig = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "orig").FirstOrDefault()?.Value,
                            CST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "CST").FirstOrDefault()?.Value,
                            vBC = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "vBC").FirstOrDefault()?.Value,
                            pICMS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "pICMS").FirstOrDefault()?.Value,
                            vICMS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "vICMS").FirstOrDefault()?.Value,
                            modBCST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "modBCST").FirstOrDefault()?.Value,
                            pMVAST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "pMVAST").FirstOrDefault()?.Value,
                            vBCST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "vBCST").FirstOrDefault()?.Value,
                            pICMSST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "pICMSST").FirstOrDefault()?.Value,
                            vICMSST = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants().Where(x => x.Name.LocalName == "vICMSST").FirstOrDefault()?.Value,

                            vBCIPI = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants().Where(x => x.Name.LocalName == "vBC").FirstOrDefault()?.Value,
                            pIPI = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants().Where(x => x.Name.LocalName == "pIPI").FirstOrDefault()?.Value,
                            vIPI = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants().Where(x => x.Name.LocalName == "vIPI").FirstOrDefault()?.Value,

                            vBCPIS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants().Where(x => x.Name.LocalName == "vBC").FirstOrDefault()?.Value,
                            pPIS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants().Where(x => x.Name.LocalName == "pPIS").FirstOrDefault()?.Value,
                            vPIS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants().Where(x => x.Name.LocalName == "vPIS").FirstOrDefault()?.Value,

                            vBCCOFINS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants().Where(x => x.Name.LocalName == "vBC").FirstOrDefault()?.Value,
                            pCOFINS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants().Where(x => x.Name.LocalName == "pCOFINS").FirstOrDefault()?.Value,
                            vCOFINS = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants().Where(x => x.Name.LocalName == "vCOFINS").FirstOrDefault()?.Value,

                            vBCUFDest = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "vBCUFDest").FirstOrDefault()?.Value,
                            pFCPUFDest = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "pFCPUFDest").FirstOrDefault()?.Value,
                            pICMSUFDest = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "pICMSUFDest").FirstOrDefault()?.Value,
                            pICMSInter = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "pICMSInter").FirstOrDefault()?.Value,
                            pICMSInterPart = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "pICMSInterPart").FirstOrDefault()?.Value,
                            vICMSUFDestuni = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "vICMSUFDest").FirstOrDefault()?.Value,
                            vICMSUFRemet = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "vICMSUFRemet").FirstOrDefault()?.Value,
                            vFCPUFDest = d.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSUFDest").Descendants().Where(x => x.Name.LocalName == "vFCPUFDest").FirstOrDefault()?.Value,
                        }).ToList();
                        #endregion

                        Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>
                        {

                        };
                        DataSet dtProduct1 = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_NFe", vParametrosProduct);

                        if (modFrete != "")
                        {
                            if (modFrete == "0")
                                modFrete = "0 - " + "Remetente";
                            if (modFrete == "1")
                                modFrete = "1 - " + "Destinatário";
                            if (modFrete == "2")
                                modFrete = "2 - " + "Terceiros";
                            if (modFrete == "3")
                                modFrete = "3 - " + "próp Remetente";
                            if (modFrete == "4")
                                modFrete = "4 - " + "próp Destinatário";
                            if (modFrete == "9")
                                modFrete = "9 - " + "Sem transporte";
                        }

                        foreach (DataRow row in dtProduct1.Tables[0].Rows)
                        {
                            row["nNF"] = int.Parse(nNF).ToString("D9").Insert(3, ".").Insert(7, ".");
                            row["serie"] = Convert.ToInt32(serie).ToString("D3");
                            row["chave"] = chave;
                            row["dhEmi"] = dhEmi;
                            row["dhSaiEnt"] = dhSaiEnt;
                            row["nProt"] = nProt;
                            row["natOp"] = natOp;
                            row["dhRecbto"] = dhRecbto;
                            row["horaSaiEnt"] = horaSaiEnt;

                            if (!string.IsNullOrWhiteSpace(CNPJ) && CNPJ.Length == 14)
                            {
                                CNPJ = Convert.ToUInt64(CNPJ).ToString(@"00\.000\.000\/0000\-00");
                            }
                            row["CNPJ"] = CNPJ;
                            row["xNome"] = xNome;
                            row["xLgr"] = xLgr;
                            row["nro"] = nro;
                            row["xBairro"] = xBairro;
                            row["xMun"] = xMun;
                            row["UF"] = UF;
                            if (!string.IsNullOrWhiteSpace(CEP) && CEP.Length == 8)
                            {
                                CEP = Convert.ToUInt64(CEP).ToString(@"00000\-000");
                            }
                            row["CEP"] = CEP;
                            row["fone"] = fone;
                            row["IE"] = IE;

                            if (!string.IsNullOrWhiteSpace(DESTCNPJ) && DESTCNPJ.Length == 14)
                            {
                                DESTCNPJ = Convert.ToUInt64(DESTCNPJ).ToString(@"00\.000\.000\/0000\-00");
                            }
                            row["destCNPJ"] = DESTCNPJ;
                            row["destxNome"] = DESTxNome;
                            row["destxLgr"] = DESTxLgr;
                            row["destnro"] = DESTnro;
                            row["destxCpl"] = DESTxCpl;
                            row["destxBairro"] = DESTxBairro;
                            //row["destcMun"] = IE;
                            row["destxMun"] = DESTxMun;
                            row["destUF"] = DESTUF;
                            if (!string.IsNullOrWhiteSpace(DESTCEP) && DESTCEP.Length == 8)
                            {
                                DESTCEP = Convert.ToUInt64(DESTCEP).ToString(@"00000\-000");
                            }
                            row["destCEP"] = DESTCEP;
                            //row["destxPais"] = IE;
                            row["destIE"] = DESIE;

                            row["vBC"] = ParseOrZero(vBC).ToString("N2");
                            row["vICMS"] = ParseOrZero(vICMS).ToString("N2");
                            row["vICMSDeson"] = ParseOrZero(vICMSDeson).ToString("N2");
                            row["vICMSUFDest"] = ParseOrZero(vICMSUFDest).ToString("N2");
                            row["vFCP"] = ParseOrZero(vFCP).ToString("N2");
                            row["vBCST"] = ParseOrZero(vBCST).ToString("N2");
                            row["vST"] = ParseOrZero(vST).ToString("N2");
                            row["vFCPST"] = ParseOrZero(vFCPST).ToString("N2");
                            row["vFCPSTRet"] = ParseOrZero(vFCPSTRet).ToString("N2");
                            row["vProd"] = ParseOrZero(vProd).ToString("N2");
                            row["vFrete"] = ParseOrZero(vFrete).ToString("N2");
                            row["vSeg"] = ParseOrZero(vSeg).ToString("N2");
                            row["vDesc"] = ParseOrZero(vDesc).ToString("N2");
                            row["vII"] = ParseOrZero(vII).ToString("N2");
                            row["vIPI"] = ParseOrZero(vIPI).ToString("N2");
                            row["vIPIDevol"] = ParseOrZero(vIPIDevol).ToString("N2");
                            row["vPIS"] = ParseOrZero(vPIS).ToString("N2");
                            row["vCOFINS"] = ParseOrZero(vCOFINS).ToString("N2");
                            row["vOutro"] = ParseOrZero(vOutro).ToString("N2");
                            row["vNF"] = ParseOrZero(vNF).ToString("N2");
                            row["vTotTrib"] = ParseOrZero(vTotTrib).ToString("N2");

                            row["modFrete"] = modFrete;
                            if (!string.IsNullOrWhiteSpace(transportaCNPJ) && transportaCNPJ.Length == 14)
                            {
                                transportaCNPJ = Convert.ToUInt64(transportaCNPJ).ToString(@"00\.000\.000\/0000\-00");
                            }
                            row["transportaCNPJ"] = transportaCNPJ;
                            row["transportaxNome"] = transportaxNome;
                            row["transportaIE"] = transportaIE;
                            row["transportaxEnder"] = transportaxEnder;
                            row["transportaxMun"] = transportaxMun;
                            row["transportaUF"] = transportaUF;
                            row["qVol"] = qVol;
                            row["esp"] = esp;
                            if (!string.IsNullOrWhiteSpace(pesoL))
                            {
                                decimal pesoLTotal = ParseOrZero(pesoL);
                                row["pesoL"] = pesoLTotal;
                            }
                            else
                            {
                                row["pesoL"] = "";
                            }

                            // peso bruto
                            if (!string.IsNullOrWhiteSpace(pesoB))
                            {
                                decimal pesoBTotal = ParseOrZero(pesoB);
                                row["pesoB"] = pesoBTotal;
                            }
                            else
                            {
                                row["pesoB"] = "";
                            }

                            row["infAdFisco"] = infAdFisco;
                            row["infCpl"] = infCpl;
                        }

                        DataTable tabela = dtProduct1.Tables[1];
                        tabela.Clear();

                        foreach (var item in itens)
                        {
                            DataRow row = tabela.NewRow();

                            row["cProd"] = item.cProd;
                            row["cEAN"] = item.cEAN;
                            row["xProd"] = item.xProd;
                            row["NCM"] = item.NCM;
                            row["CFOP"] = item.CFOP;
                            row["uCom"] = item.uCom;

                            // Quantidades e valores
                            row["qCom"] = ParseOrZero(item.qCom).ToString("N2");
                            row["vUnCom"] = ParseOrZero(item.vUnCom).ToString("N2");
                            row["vProduni"] = ParseOrZero(item.vProd).ToString("N2");

                            row["CEST"] = item.CEST;
                            row["vDescuni"] = ParseOrZero(item.vDesc).ToString("N2");

                            row["orig"] = item.orig;
                            row["CST"] = item.CST;

                            // ICMS
                            row["vBCICMS"] = ParseOrZero(item.vBC).ToString("N2");
                            row["pICMS"] = ParseOrZero(item.pICMS).ToString("N2");
                            row["vICMSuni"] = ParseOrZero(item.vICMS);

                            row["modBCST"] = item.modBCST;
                            row["pMVAST"] = ParseOrZero(item.pMVAST).ToString("N2");
                            row["vBCST"] = ParseOrZero(item.vBCST).ToString("N2");
                            row["pICMSST"] = ParseOrZero(item.pICMSST).ToString("N2");
                            row["vICMSST"] = ParseOrZero(item.vICMSST).ToString("N2");

                            // IPI
                            row["vBCIPI"] = ParseOrZero(item.vBCIPI).ToString("N2");
                            row["pIPI"] = ParseOrZero(item.pIPI).ToString("N2");
                            row["vIPIuni"] = ParseOrZero(item.vIPI).ToString("N2");

                            // PIS
                            row["vBCPIS"] = ParseOrZero(item.vBCPIS).ToString("N2");
                            row["pPIS"] = ParseOrZero(item.pPIS).ToString("N2");
                            row["vPISuni"] = ParseOrZero(item.vPIS).ToString("N2");

                            // COFINS
                            row["vBCCOFINS"] = ParseOrZero(item.vBCCOFINS);
                            row["pCOFINS"] = ParseOrZero(item.pCOFINS);
                            row["vCOFINSuni"] = ParseOrZero(item.vCOFINS);

                            // ICMS UF Destino / FCP
                            row["vBCUFDest"] = ParseOrZero(item.vBCUFDest).ToString("N2");
                            row["pFCPUFDest"] = ParseOrZero(item.pFCPUFDest).ToString("N2");
                            row["pICMSUFDest"] = ParseOrZero(item.pICMSUFDest).ToString("N2");
                            row["pICMSInter"] = ParseOrZero(item.pICMSInter).ToString("N2");
                            row["pICMSInterPart"] = ParseOrZero(item.pICMSInterPart).ToString("N2");
                            row["vFCPUFDest"] = ParseOrZero(item.vFCPUFDest).ToString("N2");
                            row["vICMSUFDestuni"] = ParseOrZero(item.vICMSUFDestuni).ToString("N2");
                            row["vICMSUFRemet"] = ParseOrZero(item.vICMSUFRemet).ToString("N2");

                            tabela.Rows.Add(row);
                        }

                        var todosItens = dtProduct1.Tables[1];

                        int itensPorPagina = 15;

                        DataTable itensPagina1 = todosItens.Clone();
                        DataTable itensRestantes = todosItens.Clone();

                        for (int i = 0; i < todosItens.Rows.Count; i++)
                        {
                            if (i < itensPorPagina)
                                itensPagina1.ImportRow(todosItens.Rows[i]);
                            else
                                itensRestantes.ImportRow(todosItens.Rows[i]);
                        }

                        rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtProduct1.Tables[0]));
                        rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", itensPagina1));
                        rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet3", itensRestantes));

                        byte[] imagemBytes;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            Barcode128 codigoBarras = new Barcode128
                            {
                                Code = chave,
                                CodeType = Barcode.CODE128,
                                StartStopText = true,
                                GenerateChecksum = true,
                                ChecksumText = true,
                                BarHeight = 40f,
                                X = 0.8f,
                                Font = null
                            };

                            var imagem = codigoBarras.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White);
                            imagem.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            imagemBytes = ms.ToArray();
                        }

                        string imagemBase64 = Convert.ToBase64String(imagemBytes);
                        int Quantidade = itensRestantes.Rows.Count;

                        DateTime data = DateTime.Now;
                        ReportParameter[] rp = new ReportParameter[6];
                        rp[0] = new ReportParameter("Espaco", " ");
                        rp[1] = new ReportParameter("imagem", imagemBase64);
                        rp[2] = new ReportParameter("Data", data.ToString("dd/MM/yyyy"));
                        rp[3] = new ReportParameter("Hora", data.ToString("HH:mm:ss"));
                        rp[4] = new ReportParameter("tpAmb", tpAmb);
                        rp[5] = new ReportParameter("QuantidadeItens", Quantidade.ToString());
                        rv.LocalReport.SetParameters(rp);

                        rv.LocalReport.Refresh();

                        byte[] bytesNFe = bPDF ? rv.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings) : rv.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                        string pdfPath = "";
                        if (sImportador == "N")
                        {
                            pdfPath = "C:\\Unimake\\UniNFe\\" + CNPJ.Replace(".", "").Replace("/", "").Replace("-", "") + "\\Piloto\\" + chave + ".pdf";
                        }
                        else
                        {
                            pdfPath = HttpContext.Current.Server.MapPath("~/Download/") + chave + ".pdf";
                        }

                        if (bPDF && Quantidade == 0)
                        {
                            using (var input = new MemoryStream(bytesNFe))
                            using (var reader = new iTextSharp.text.pdf.PdfReader(input))
                            using (var tempOutput = new MemoryStream())
                            {
                                using (var doc = new iTextSharp.text.Document(reader.GetPageSizeWithRotation(1)))
                                using (var copy = new iTextSharp.text.pdf.PdfCopy(doc, tempOutput))
                                {
                                    doc.Open();
                                    if (reader.NumberOfPages >= 1)
                                    {
                                        copy.AddPage(copy.GetImportedPage(reader, 1));
                                    }
                                }

                                byte[] onePagePdf = tempOutput.ToArray();
                                using (var reader2 = new iTextSharp.text.pdf.PdfReader(onePagePdf))
                                using (var output = new MemoryStream())
                                {
                                    using (var stamper = new iTextSharp.text.pdf.PdfStamper(reader2, output))
                                    {
                                        int totalPages = reader2.NumberOfPages;

                                        for (int i = 1; i <= totalPages; i++)
                                        {
                                            PdfContentByte cb = stamper.GetOverContent(i);
                                            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                                            cb.BeginText();
                                            cb.SetFontAndSize(bf, 7);

                                            string texto = $"{i}/{totalPages}";
                                            float pageWidth = reader2.GetPageSize(i).Width;

                                            float x = (pageWidth / 2) + 7;
                                            float y = 692;

                                            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, texto, x, y, 0);
                                            cb.EndText();
                                        }
                                    }

                                    File.WriteAllBytes(pdfPath, output.ToArray());
                                }
                            }
                            string NomeArquivo = chave + ".pdf";
                            return NomeArquivo;
                        }
                        else
                        {
                            using (var input = new MemoryStream(bytesNFe))
                            using (var reader = new iTextSharp.text.pdf.PdfReader(input))
                            using (var output = new MemoryStream())
                            {
                                using (var stamper = new iTextSharp.text.pdf.PdfStamper(reader, output))
                                {
                                    int totalPages = reader.NumberOfPages;

                                    for (int i = 1; i <= totalPages; i++)
                                    {
                                        PdfContentByte cb = stamper.GetOverContent(i);
                                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                                        cb.BeginText();
                                        cb.SetFontAndSize(bf, 8);

                                        string texto = $"{i}/{totalPages}";
                                        float pageWidth = reader.GetPageSize(i).Width;

                                        float x;
                                        float y;
                                        int alignment;

                                        if (i == 1)
                                        {
                                            x = (pageWidth / 2) + 7;
                                            y = 692;
                                            alignment = PdfContentByte.ALIGN_CENTER;
                                        }
                                        else
                                        {
                                            x = (pageWidth / 2) + 13;
                                            y = 746;
                                            alignment = PdfContentByte.ALIGN_RIGHT;
                                        }

                                        cb.ShowTextAligned(alignment, texto, x, y, 0);
                                        cb.EndText();
                                    }
                                }

                                File.WriteAllBytes(pdfPath, output.ToArray());
                            }
                            string NomeArquivo = chave + ".pdf";
                            return NomeArquivo;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(ex.Message);
                }
            }
        }

        public class Download
        {
            public static void XML(Page page, string idXML) => Download_Arquivos(page, "XML", idXML);
            public static void PDF(Page page, string idXML) => Download_Arquivos(page, "DANFE", idXML);

            private static void Download_Arquivos(Page page, string sTipo, string idXML)
            {
                Dictionary<string, string> vParametrosXML = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idXML", idXML }
                };
                DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametrosXML);

                if (ValidarDataSet(ds))
                {
                    try
                    {
                        if (DATASET(ds, "idOrigem") == "2") // NFS-e
                        {
                            bool bAutorizada = DATASET(ds, "sStatus") == "NFS A";

                            if (sTipo == "XML")
                                DownloadArquivo(page, XML_NFS(page, DATASET(ds, bAutorizada ? "idArquivo" : "idArquivo_Retorno"), DATASET(ds, "nNumeroNF")));
                            else if (sTipo == "DANFE")
                                PDF_NFS(page, idXML, DATASET(ds, "idArquivoPDF"), bAutorizada);
                        }
                        else // NF-e
                        {
                            string sNomeArquivo = "";
                            if (sTipo == "XML")
                            {
                                sNomeArquivo = $"{DATASET(ds, "chNFe")}.xml";
                                File.WriteAllText(HttpContext.Current.Server.MapPath("~/Download/" + sNomeArquivo), DATASET(ds, "sXML"));
                            }
                            else if (sTipo == "DANFE")
                            {
                                sNomeArquivo = string.Format("{0}_{1}.pdf", DATASET(ds, "chNFe"), DATASET(ds, "sStatus"));
                                DANFE.GerarDANFE(HttpContext.Current.Server.MapPath("~/Download/"), DATASET(ds, "chNFe"), DATASET(ds, "sXML"), false, DATASET(ds, 0, "sStatus"), DATASET(ds, "idEmpresa"));
                            }

                            DownloadArquivo(page, sNomeArquivo);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Houve um erro ao gerar o documento " + sTipo + "</br>" + ex.Message);
                    }
                }
                else throw new Exception("Não foi encontrado documento " + sTipo);
            }

            public static string XML_NFS(Page page, string idArquivo, string nNumeroNF)
            {
                string pasta = HttpContext.Current.Server.MapPath("~/Download/");
                string arquivoMaisRecente = Directory.GetFiles(pasta, $"RPS{nNumeroNF}-*.xml").OrderByDescending(f => File.GetLastWriteTime(f)).FirstOrDefault();

                string sNomeArquivo;
                if (arquivoMaisRecente != null)
                    sNomeArquivo = Path.GetFileName(arquivoMaisRecente);
                else
                {
                    Dictionary<string, string> vParam = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idArquivo", idArquivo }
                    };
                    DataTable tb_NFS = ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParam);

                    sNomeArquivo = tb_NFS.Rows[0]["sNomeArquivo"].ToString();
                    File.WriteAllBytes(Path.Combine(pasta, sNomeArquivo), (byte[])tb_NFS.Rows[0]["vbArquivo"]);
                }

                return sNomeArquivo;
            }

            public static void PDF_NFS(Page page, string idXML, string idArquivo, bool bAutorizada)
            {
                DataTable tb = ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_DETALHE" }, { "@idArquivo", idArquivo } });
                DataTable tb_XML = ExecutarDataTable("sp_Manipula_tbl_Flow_XML_NFe", new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_DETALHE" }, { "@idXML", idXML } });

                var imagem = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App", "img", bAutorizada ? "LogoTT_Horizontal.png" : "Espelho.png");
                byte[] PDF = Adiciona_MarcaDagua((byte[])tb.Rows[0]["vbArquivo"], imagem, "", null, null, 0, 10, 200, -100, 35, 0.35f);

                string sXML = tb_XML.Rows[0]["sXML"].ToString();
                if (bAutorizada && !string.IsNullOrWhiteSpace(sXML))
                {
                    string idArquivo_Retorno = tb_XML.Rows[0]["idArquivo_Retorno"].ToString();
                    idArquivo_Retorno = string.IsNullOrWhiteSpace(idArquivo_Retorno) || idArquivo_Retorno == "0" ? "1" : idArquivo_Retorno;
                    DataTable tb_TXT = ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_DETALHE" }, { "@idArquivo", idArquivo_Retorno } });

                    string sCodigoNF = "", nNumeroNF = tb_XML.Rows[0]["nNumeroNF"].ToString().PadLeft(8, '0');
                    if (sXML.StartsWith("<"))
                    {
                        byte[] vbArquivo = (byte[])tb_TXT.Rows[0]["vbArquivo"];
                        using (MemoryStream stream = new MemoryStream(vbArquivo))
                        {
                            XDocument xml = XDocument.Load(stream);
                            nNumeroNF = (xml.Descendants("NumeroNFe").FirstOrDefault()?.Value ?? "").PadLeft(8, '0');
                            sCodigoNF = xml.Descendants("CodigoVerificacao").FirstOrDefault()?.Value ?? "";
                        }
                    }
                    else
                    {
                        sXML = Encoding.UTF8.GetString((byte[])tb_TXT.Rows[0]["vbArquivo"]);
                        foreach (string linha in sXML.Split('\n'))
                        {
                            if (linha.StartsWith("2") && linha.Substring(9, 8) == nNumeroNF)
                                sCodigoNF = linha.Substring(31, 8);
                        }
                    }

                    PDF = Adiciona_MarcaDagua(PDF, null, nNumeroNF, null, BaseColor.BLACK, 10, 10, 200, -100, 0, 0.85f, 487, 812);
                    PDF = Adiciona_MarcaDagua(PDF, null, sCodigoNF, null, BaseColor.BLACK, 10, 10, 200, -100, 0, 0.85f, 491, 760);
                }

                HttpContext.Current.Session["ExibeArquivo"] = PDF;
                HttpContext.Current.Session["ExibeArquivo_Nome"] = tb.Rows[0]["sNomeArquivo"];

                DirecionaPagina_NovaAba(page, "/App/ExibeArquivo.aspx");
            }
        }

        public static void AdicionarMarcaDagua(string sTexto, string sNomeArquivo)
        {
            PdfReader pdfReader = new PdfReader(sNomeArquivo);
            string sNomeArquivo_TEMP = sNomeArquivo.Replace(".pdf", "TEMP.pdf");
            using (FileStream outputStream = new FileStream(sNomeArquivo_TEMP, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                PdfStamper pdfStamper = new PdfStamper(pdfReader, outputStream);

                int pageCount = pdfReader.NumberOfPages;

                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                PdfGState gState = new PdfGState();
                gState.FillOpacity = 0.4f;

                for (int i = 1; i <= pageCount; i++)
                {
                    var pageSize = pdfReader.GetPageSizeWithRotation(i);
                    float width = pageSize.Width;
                    float height = pageSize.Height;

                    PdfContentByte content = pdfStamper.GetOverContent(i);
                    content.SaveState();
                    content.SetGState(gState);
                    content.BeginText();
                    content.SetColorFill(BaseColor.RED);
                    content.SetFontAndSize(bf, 70);

                    float x = width / 2;
                    float y = height / 2;

                    float deslocamento = 25; // ajusta distância entre as palavras na diagonal

                    //// Primeira palavra "DOCUMENTO"
                    //content.ShowTextAligned(Element.ALIGN_CENTER, "DOCUMENTO", x - deslocamento, y + deslocamento, 40);

                    //// Segunda palavra "CANCELADO"
                    //content.ShowTextAligned(Element.ALIGN_CENTER, "CANCELADO", x + deslocamento, y - deslocamento, 40);

                    content.ShowTextAligned(Element.ALIGN_CENTER, sTexto, x + deslocamento, y - deslocamento, 40);
                    content.EndText();
                    content.RestoreState();
                }

                pdfStamper.Close();
                pdfReader.Close();

                if (File.Exists(sNomeArquivo))
                {
                    File.Delete(sNomeArquivo);
                    File.Move(sNomeArquivo_TEMP, sNomeArquivo);
                }
            }
        }

        public static byte[] ConsultaImagemDANFE(string idEmpresa, string cnpjEmitente = null)
        {
            byte[] logo = BuscarLogoEmpresaDANFE(idEmpresa);

            if (logo == null && !string.IsNullOrWhiteSpace(cnpjEmitente))
            {
                string idEmpresaEmitente = ResolverIdEmpresaPorCnpj(cnpjEmitente);
                if (!string.IsNullOrEmpty(idEmpresaEmitente))
                    logo = BuscarLogoEmpresaDANFE(idEmpresaEmitente);
            }

            return NormalizarImagemLogo(logo);
        }

        private static byte[] BuscarLogoEmpresaDANFE(string idEmpresa)
        {
            if (string.IsNullOrWhiteSpace(idEmpresa) || idEmpresa == "0")
                return null;

            Dictionary<string, string> vParametrosXML = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_IMAGEM" },
                { "@idObjeto", idEmpresa },
                { "@idTipoArquivo", "322" }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosXML);

            if (tb.Rows.Count > 0)
                return (byte[])tb.Rows[0]["vbArquivo"];

            return null;
        }

        private static string ResolverIdEmpresaPorCnpj(string cnpj)
        {
            cnpj = Regex.Replace(cnpj ?? "", @"\D", "");
            if (cnpj.Length != 14)
                return null;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sfuncao", "ConsultarParceiro" },
                { "@sCPF_CNPJ", cnpj }
            };
            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParametros);

            if (BD.ValidarDataSet(ds))
                return Retorno.DATASET(ds, 0, "idEmpresa");

            return null;
        }

        /// <summary>
        /// PdfClown (AdicionarLogoImagem) exige JPEG baseline (não progressivo). PNG e outros formatos são convertidos.
        /// </summary>
        private static byte[] NormalizarImagemLogo(byte[] imagem)
        {
            if (imagem == null || imagem.Length == 0)
                return null;

            try
            {
                using (var entrada = new MemoryStream(imagem))
                using (System.Drawing.Image imgOriginal = System.Drawing.Image.FromStream(entrada))
                using (var bitmap = new System.Drawing.Bitmap(imgOriginal.Width, imgOriginal.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                using (var g = System.Drawing.Graphics.FromImage(bitmap))
                using (var saida = new MemoryStream())
                {
                    g.Clear(System.Drawing.Color.White);
                    g.DrawImage(imgOriginal, 0, 0, imgOriginal.Width, imgOriginal.Height);

                    ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageEncoders()
                        .FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);

                    if (jpegCodec != null)
                    {
                        using (var encoderParams = new EncoderParameters(1))
                        {
                            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                            bitmap.Save(saida, jpegCodec, encoderParams);
                        }
                    }
                    else
                    {
                        bitmap.Save(saida, ImageFormat.Jpeg);
                    }

                    return saida.ToArray();
                }
            }
            catch
            {
                return null;
            }
        }
    }

    public class Funcoes_CCe
    {
        public static string EnviarXML_SEFAZ(string idXML, string sTextoCorrecao)
        {
            string sRetorno = "";

            string CNPJ = "";
            string cOrgao = "";
            string tpAmb = "";
            string sChaveNFe = "";
            int nNumeroCartaCorrecao = 0;


            try
            {
                Dictionary<string, string> vParametrosXML = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idXML", idXML }
                };
                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametrosXML);

                if (Retorno.DATASET(ds, "sEspelho") == "N")
                {
                    sChaveNFe = Retorno.DATASET(ds, 0, "chNFe");
                    nNumeroCartaCorrecao = Convert.ToInt32(Retorno.DATASET(ds, 0, "nQtdCCe")) + 1;
                    XmlDocument XML_NFe = new XmlDocument();
                    XML_NFe.LoadXml(Retorno.DATASET(ds, 0, "sXML"));

                    XmlNamespaceManager nsManager = new XmlNamespaceManager(XML_NFe.NameTable);
                    nsManager.AddNamespace("nfe", "http://www.portalfiscal.inf.br/nfe");

                    XmlNode infNFe = XML_NFe.SelectSingleNode("//nfe:infNFe", nsManager);
                    XmlNode ide = XML_NFe.SelectSingleNode("//nfe:infNFe/nfe:ide", nsManager);
                    XmlNode emit = XML_NFe.SelectSingleNode("//nfe:infNFe/nfe:emit", nsManager);

                    CNPJ = emit["CNPJ"].InnerText;
                    cOrgao = ide["cUF"].InnerText;
                    tpAmb = ide["tpAmb"].InnerText;

                    string sPath_Salvar = Path.Combine(Retorno.DATASET(ds, "sCaminho_UniNFE").ToString(), CNPJ, "envio");
                    if (!Directory.Exists(sPath_Salvar))
                    {
                        Directory.CreateDirectory(sPath_Salvar);
                    }

                    sPath_Salvar = Path.Combine(sPath_Salvar, sChaveNFe + -110110 + "-" + nNumeroCartaCorrecao.ToString("D2") + "-ped-eve.xml");

                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        Encoding = System.Text.Encoding.UTF8
                    };
                    using (XmlWriter writer = XmlWriter.Create(sPath_Salvar, settings))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("envEvento", "http://www.portalfiscal.inf.br/nfe");
                        writer.WriteAttributeString("versao", "1.00");
                        writer.WriteElementString("idLote", "000000000000001");

                        writer.WriteStartElement("evento", "http://www.portalfiscal.inf.br/nfe");
                        writer.WriteAttributeString("versao", "1.00");
                        writer.WriteStartElement("infEvento");
                        writer.WriteAttributeString("Id", "ID110110" + sChaveNFe + nNumeroCartaCorrecao.ToString("D2"));

                        writer.WriteElementString("cOrgao", cOrgao);
                        writer.WriteElementString("tpAmb", tpAmb);
                        writer.WriteElementString("CNPJ", CNPJ);
                        writer.WriteElementString("chNFe", sChaveNFe);
                        writer.WriteElementString("dhEvento", DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        writer.WriteElementString("tpEvento", "110110");
                        writer.WriteElementString("nSeqEvento", nNumeroCartaCorrecao.ToString());
                        writer.WriteElementString("verEvento", "1.00");

                        writer.WriteStartElement("detEvento");
                        writer.WriteAttributeString("versao", "1.00");

                        writer.WriteElementString("descEvento", "Carta de Correção");
                        writer.WriteElementString("xCorrecao", sTextoCorrecao);
                        writer.WriteElementString("xCondUso", "A Carta de Correção é disciplinada pelo § 1º-A do art. 7º do Convênio S/N, de 15 de dezembro de 1970 e pode ser utilizada " +
                            "para regularização de erro ocorrido na emissão de documento fiscal, desde que o erro não esteja relacionado com: I - as variáveis que determinam o valor do imposto " +
                            "tais como: base de cálculo, alíquota, diferença de preço, quantidade, valor da operação ou da prestação; II - a correção de dados cadastrais que implique mudança do " +
                            "remetente ou do destinatário; III - a data de emissão ou de saída.");
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }

                    Dictionary<String, String> vParametros_CCe = new Dictionary<string, string>();
                    vParametros_CCe.Add("@sFuncao", "REGISTRA_CCE");
                    vParametros_CCe.Add("@sObservacao", sTextoCorrecao);
                    vParametros_CCe.Add("@sChaveNFe", sChaveNFe);
                    vParametros_CCe.Add("@nNumeroCartaCorrecao", nNumeroCartaCorrecao.ToString());
                    BD.ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametros_CCe);
                }
                else
                {
                    throw new Exception("NFe não foi autorizada, impossivel gerar CCe!");
                }
            }
            catch (Exception)
            {

                throw;
            }

            return sRetorno;
        }

        public static void Download_XML(Page page, string sChaveNFe, string nNumeroCartaCorrecao)
        {
            try
            {
                string sCaminho_TFlow = BD.CarregarParametro(BD.Parametro.Caminho_TFlow) + "NFe\\";
                string sNomeArquivo_XML_CCe = string.Format("{0}-CCe-{01}.xml", sChaveNFe, nNumeroCartaCorrecao.ToString().PadLeft(2, '0'));
                string sPathCompletoSalvar = sCaminho_TFlow + sNomeArquivo_XML_CCe;

                if (!File.Exists(sPathCompletoSalvar))
                {
                    if (!Directory.Exists(sCaminho_TFlow))
                        Directory.CreateDirectory(sCaminho_TFlow);

                    Dictionary<string, string> vParametrosXML = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE_CCE" },
                        { "@sChaveNFe", sChaveNFe },
                        {"@nNumeroCartaCorrecao", nNumeroCartaCorrecao }

                    };
                    DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametrosXML);
                    if (BD.ValidarDataSet(ds))
                    {
                        File.WriteAllText(sPathCompletoSalvar, Retorno.DATASET(ds, 1, 0, "sXML"));
                    }
                    else
                    {
                        throw new Exception("Nenhuma CCe Localizada");
                    }

                }
                Funcoes.DownloadArquivo_NFe(page, sNomeArquivo_XML_CCe);

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar arquivo XML CCe: " + ex.Message);
            }
        }

        public static void Download_PDF(Page page, string sChaveNFe, string nNumeroCartaCorrecao)
        {
            string sNomeArquivo = Gerar_PDF(sChaveNFe, nNumeroCartaCorrecao, BD.CarregarParametro(BD.Parametro.Caminho_TFlow), false);
            Funcoes.DownloadArquivo_NFe(page, sNomeArquivo);
        }

        public static string Gerar_PDF(string sChaveNFe, string nNumeroCartaCorrecao, string sPathSalvar, bool bForcarGeracao)
        {
            string sRetorno = "";
            try
            {
                string sNomeArquivo_PDF_CCe = string.Format("{0}-CCe-{01}.pdf", sChaveNFe, nNumeroCartaCorrecao.ToString().PadLeft(2, '0'));
                string sPathCompletoSalvar = sPathSalvar + "NFe\\" + sNomeArquivo_PDF_CCe;
                if (!File.Exists(sPathCompletoSalvar) && !bForcarGeracao)
                    bForcarGeracao = true;

                if (bForcarGeracao)
                {
                    if (!Directory.Exists(sPathSalvar))
                        Directory.CreateDirectory(sPathSalvar);


                    Dictionary<string, string> vParametrosXML = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE_CCE" },
                        { "@sChaveNFe", sChaveNFe },
                        {"@nNumeroCartaCorrecao", nNumeroCartaCorrecao.ToString() }

                    };
                    DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametrosXML);
                    if (BD.ValidarDataSet(ds))
                    {
                        XmlDocument XML_CCe = new XmlDocument();
                        XmlDocument XML_NFe = new XmlDocument();

                        XML_NFe.LoadXml(Retorno.DATASET(ds, 0, "sXML"));
                        XML_CCe.LoadXml(Retorno.DATASET(ds, 1, 0, "sXML"));

                        XmlNamespaceManager nsManager = new XmlNamespaceManager(XML_CCe.NameTable);
                        nsManager.AddNamespace("nfe", "http://www.portalfiscal.inf.br/nfe");

                        XmlNode XML_NFE_IDE = XML_NFe.SelectSingleNode("//nfe:nfeProc/nfe:NFe/nfe:infNFe/nfe:ide", nsManager);
                        XmlNode XML_NFE_IDE_EMIT = XML_NFE_IDE.SelectSingleNode("//nfe:emit", nsManager);
                        XmlNode XML_CCE_INFEVENTO = XML_CCe.SelectSingleNode("//nfe:procEventoNFe/nfe:evento/nfe:infEvento", nsManager);
                        XmlNode XML_CCE_RET_EVENTO = XML_CCe.SelectSingleNode("//nfe:procEventoNFe/nfe:retEvento/nfe:infEvento", nsManager);

                        #region | DADOS CARTA CORREÇÃO
                        string mod = XML_NFE_IDE["mod"].InnerText;
                        string numero = XML_NFE_IDE["nNF"].InnerText.PadLeft(6, '0');
                        string serie = XML_NFE_IDE["serie"].InnerText.PadLeft(3, '0');
                        string cOrgao = XML_NFE_IDE["cUF"].InnerText;

                        string xCorrecao = XML_CCE_INFEVENTO.SelectSingleNode("//nfe:detEvento", nsManager)["xCorrecao"].InnerText;
                        string xCondUso = XML_CCE_INFEVENTO.SelectSingleNode("//nfe:detEvento", nsManager)["xCondUso"].InnerText;

                        string cStat = XML_CCE_RET_EVENTO["cStat"].InnerText;
                        string xMotivo = XML_CCE_RET_EVENTO["xMotivo"].InnerText;
                        string nProt = XML_CCE_RET_EVENTO["nProt"].InnerText;



                        string xNome = XML_NFE_IDE.SelectSingleNode("//nfe:emit", nsManager)["xNome"].InnerText;
                        string CNPJ = XML_NFE_IDE.SelectSingleNode("//nfe:emit", nsManager)["CNPJ"].InnerText;

                        string xLgr = XML_NFE_IDE.SelectSingleNode("//nfe:emit/nfe:enderEmit", nsManager)["xLgr"].InnerText;
                        string nro = XML_NFE_IDE.SelectSingleNode("//nfe:emit/nfe:enderEmit", nsManager)["nro"].InnerText;
                        string xBairro = XML_NFE_IDE.SelectSingleNode("//nfe:emit/nfe:enderEmit", nsManager)["xBairro"].InnerText;
                        string CEP = XML_NFE_IDE.SelectSingleNode("//nfe:emit/nfe:enderEmit", nsManager)["CEP"].InnerText;
                        string xMun = XML_NFE_IDE.SelectSingleNode("//nfe:emit/nfe:enderEmit", nsManager)["xMun"].InnerText;
                        string UF = XML_NFE_IDE.SelectSingleNode("//nfe:emit/nfe:enderEmit", nsManager)["UF"].InnerText;
                        string IE = XML_NFE_IDE.SelectSingleNode("//nfe:emit", nsManager)["IE"].InnerText;

                        string DESTxNome = XML_NFE_IDE.SelectSingleNode("//nfe:dest", nsManager)["xNome"].InnerText;
                        string DESTCNPJ = "";


                        if (XML_NFE_IDE.SelectSingleNode("//nfe:dest", nsManager).InnerXml.Contains("CPF"))
                        {
                            DESTCNPJ = XML_NFE_IDE.SelectSingleNode("//nfe:dest", nsManager)["CPF"].InnerText;
                        }
                        else
                        {
                            if (XML_NFE_IDE.SelectSingleNode("//nfe:dest", nsManager).InnerXml.Contains("CNPJ"))
                            {
                                DESTCNPJ = XML_NFE_IDE.SelectSingleNode("//nfe:dest", nsManager)["CNPJ"].InnerText;
                            }
                        }


                        string DESTxLgr = XML_NFE_IDE.SelectSingleNode("//nfe:dest/nfe:enderDest", nsManager)["xLgr"].InnerText;
                        string DESTnro = XML_NFE_IDE.SelectSingleNode("//nfe:dest/nfe:enderDest", nsManager)["nro"].InnerText;
                        string DESTxCpl = "";
                        if (XML_NFE_IDE.SelectSingleNode("//nfe:dest", nsManager).InnerXml.Contains("xCpl"))
                            DESTxCpl = XML_NFE_IDE.SelectSingleNode("//nfe:dest/nfe:enderDest", nsManager)["xCpl"].InnerText;
                        string DESTxBairro = XML_NFE_IDE.SelectSingleNode("//nfe:dest/nfe:enderDest", nsManager)["xBairro"].InnerText;
                        string DESTCEP = "";
                        if (XML_NFE_IDE.SelectSingleNode("//nfe:dest", nsManager).InnerXml.Contains("CEP"))
                            DESTCEP = XML_NFE_IDE.SelectSingleNode("//nfe:dest/nfe:enderDest", nsManager)["CEP"].InnerText;

                        string DESTxMun = XML_NFE_IDE.SelectSingleNode("//nfe:dest/nfe:enderDest", nsManager)["xMun"].InnerText;
                        string DESTUF = XML_NFE_IDE.SelectSingleNode("//nfe:dest/nfe:enderDest", nsManager)["UF"].InnerText;
                        string DESIE = "";



                        string dataEvento = XML_CCE_RET_EVENTO["dhRegEvento"].InnerText;
                        string chaveFormatada = Regex.Replace(sChaveNFe, ".{4}", "$0 ").Trim();
                        string dataFormatada = "";
                        if (DateTime.TryParse(dataEvento, out DateTime data))
                        {
                            dataFormatada = data.ToString("dd/MM/yyyy");
                        }

                        #endregion

                        #region | Font
                        iTextSharp.text.Font fontMaior = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                        iTextSharp.text.Font font10 = FontFactory.GetFont(FontFactory.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                        iTextSharp.text.Font font8 = FontFactory.GetFont(FontFactory.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                        iTextSharp.text.Font font7 = FontFactory.GetFont(FontFactory.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                        iTextSharp.text.Font font6 = FontFactory.GetFont(FontFactory.HELVETICA, 6, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                        iTextSharp.text.Font font6BOLD = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 6, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                        iTextSharp.text.Font fontMenor = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                        iTextSharp.text.Font fon8BOLD = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                        #endregion

                        if (File.Exists(sPathCompletoSalvar))
                        {
                            File.Delete(sPathCompletoSalvar);
                        }

                        #region | REMETENTE
                        Document doc = new Document(PageSize.A4, 30, 30, 30, 30);
                        PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(sPathCompletoSalvar, FileMode.Create));
                        doc.Open();

                        //Criação tabela Historico
                        PdfPTable tabidentific = new PdfPTable(2)
                        {
                            WidthPercentage = 100
                        };
                        tabidentific.SetWidths(new float[] { 50f, 50f });

                        // Criar tabela interna
                        PdfPTable tabela = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // Adiciona as células sem borda
                        PdfPCell cell1 = new PdfPCell(new Phrase("IDENTIFICAÇÃO DO EMITENTE", FontFactory.GetFont(FontFactory.HELVETICA, 7)))
                        {
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                        tabela.AddCell(cell1);

                        PdfPCell cell = new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.HELVETICA, 7)))
                        {
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                        tabela.AddCell(cell);

                        PdfPCell cell2 = new PdfPCell(new Phrase(xNome, FontFactory.GetFont(FontFactory.HELVETICA, 12)))
                        {
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                        tabela.AddCell(cell2);

                        PdfPCell cell3 = new PdfPCell(new Phrase(xLgr + ", " + nro + " - " + xBairro, FontFactory.GetFont(FontFactory.HELVETICA, 7)))
                        {
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                        tabela.AddCell(cell3);

                        PdfPCell cell4 = new PdfPCell(new Phrase(CEP + " - " + xMun + " - " + UF, FontFactory.GetFont(FontFactory.HELVETICA, 7)))
                        {
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                        tabela.AddCell(cell4);

                        // Adiciona a tabela como célula da tabidentific
                        PdfPCell celulaTituloHistorico = new PdfPCell(tabela)
                        {
                            Rowspan = 3,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_TOP
                        };

                        tabidentific.AddCell(celulaTituloHistorico);

                        //Coluna 1 tabela Historico 
                        Chunk chunkCCe = new Chunk("CC-e\n", fontMaior);
                        Chunk chunkCarta = new Chunk("CARTA DE CORREÇÃO ELETRÔNICA DE NF-e", fontMenor);
                        Phrase fraseComFontesDiferentes = new Phrase
                                                {
                                                    chunkCCe,
                                                    chunkCarta
                                                };
                        PdfPCell celulaHist1 = new PdfPCell(fraseComFontesDiferentes)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            PaddingBottom = 2f
                        };
                        tabidentific.AddCell(celulaHist1);

                        //Coluna 2 tabela Historico
                        PdfPTable tabelaInterna = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        PdfPCell celulaTitulo = new PdfPCell(new Phrase("CHAVE DE ACESSO", font7))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 0f,
                            PaddingTop = 0f
                        };
                        tabelaInterna.AddCell(celulaTitulo);

                        PdfPCell celulaChave = new PdfPCell(new Phrase(chaveFormatada, fontMenor))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelaInterna.AddCell(celulaChave);

                        PdfPCell celulaHist2 = new PdfPCell(tabelaInterna)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        tabidentific.AddCell(celulaHist2);


                        //Coluna 2 tabela Historico
                        Barcode128 codigoBarras = new Barcode128
                        {
                            Code = sChaveNFe,
                            CodeType = Barcode.CODE128,
                            TextAlignment = Element.ALIGN_CENTER,
                            StartStopText = true,
                            GenerateChecksum = true,
                            ChecksumText = true,
                            BarHeight = 40f,
                            X = 0.8f,
                            Font = null // <<<<< Isto remove os números abaixo do código de barras
                        };
                        iTextSharp.text.Image imagemCodigoBarras = codigoBarras.CreateImageWithBarcode(writer.DirectContent, null, null);
                        imagemCodigoBarras.ScalePercent(100f);
                        imagemCodigoBarras.Alignment = Element.ALIGN_CENTER;
                        PdfPCell celulaCodigoBarras = new PdfPCell(imagemCodigoBarras)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Padding = 3f
                        };
                        tabidentific.AddCell(celulaCodigoBarras);
                        doc.Add(tabidentific);

                        // Tabela principal com 7 colunas
                        PdfPTable tabiden = new PdfPTable(7)
                        {
                            WidthPercentage = 100
                        };
                        tabiden.SetWidths(new float[] { 25f, 25f, 8f, 8f, 14f, 11f, 9f });

                        //--------------------------------------------------
                        PdfPTable tabelaIE = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // INSCRIÇÃO ESTADUAL (título)
                        PdfPCell cellLabel = new PdfPCell(new Phrase("INSCRIÇÃO ESTADUAL", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelaIE.AddCell(cellLabel);

                        // INSCRIÇÃO ESTADUAL (valor)
                        PdfPCell cellKey = new PdfPCell(new Phrase(IE, font8))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelaIE.AddCell(cellKey);

                        // Adiciona tabelaIE como a célula da primeira coluna
                        PdfPCell cellIE = new PdfPCell(tabelaIE)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        tabiden.AddCell(cellIE);

                        //--------------------------------------------------
                        PdfPTable tabelaCNPJ = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // CNPJ (título)
                        PdfPCell CNPJteble = new PdfPCell(new Phrase("CNPJ", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelaCNPJ.AddCell(CNPJteble);

                        // CNPJ (valor)
                        PdfPCell CNPJvalor = new PdfPCell(new Phrase(CNPJ, font8))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelaCNPJ.AddCell(CNPJvalor);

                        // Adiciona tabelaCNPJ como a célula da segunda coluna
                        PdfPCell cellCNPJ = new PdfPCell(tabelaCNPJ)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        tabiden.AddCell(cellCNPJ);

                        //--------------------------------------------------
                        PdfPTable tabelamodelo = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // MODELO (título)
                        PdfPCell MODELOteble = new PdfPCell(new Phrase("MODELO", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelamodelo.AddCell(MODELOteble);

                        // MODELO (valor)
                        PdfPCell MODELOvalor = new PdfPCell(new Phrase(mod, font8))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelamodelo.AddCell(MODELOvalor);

                        // Adiciona cellMODELO como a célula da segunda coluna
                        PdfPCell cellMODELO = new PdfPCell(tabelamodelo)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        tabiden.AddCell(cellMODELO);

                        //--------------------------------------------------
                        PdfPTable tabelaSERIE = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };
                        // SERIE (título)
                        PdfPCell SERIEteble = new PdfPCell(new Phrase("SÉRIE", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelaSERIE.AddCell(SERIEteble);

                        // SERIE (valor)
                        PdfPCell SERIEvalor = new PdfPCell(new Phrase(serie, font8))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelaSERIE.AddCell(SERIEvalor);

                        // Adiciona cellSERIE como a célula da segunda coluna
                        PdfPCell cellSERIE = new PdfPCell(tabelaSERIE)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        tabiden.AddCell(cellSERIE);

                        //--------------------------------------------------
                        PdfPTable tabelaNÚMERO = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };
                        // SERIE (título)
                        PdfPCell NÚMEROteble = new PdfPCell(new Phrase("NÚMERO NF-e", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelaNÚMERO.AddCell(NÚMEROteble);

                        // SERIE (valor)
                        PdfPCell NÚMEROvalor = new PdfPCell(new Phrase(numero, font8))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelaNÚMERO.AddCell(NÚMEROvalor);

                        // Adiciona cellSERIE como a célula da segunda coluna
                        PdfPCell cellNÚMERO = new PdfPCell(tabelaNÚMERO)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        tabiden.AddCell(cellNÚMERO);

                        //--------------------------------------------------
                        PdfPTable tabelaEMISSAO = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };
                        // SERIE (título)
                        PdfPCell EMISSAOteble = new PdfPCell(new Phrase("EMISSÃO", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelaEMISSAO.AddCell(EMISSAOteble);

                        // SERIE (valor)
                        PdfPCell EMISSAOvalor = new PdfPCell(new Phrase(dataFormatada, font8))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelaEMISSAO.AddCell(EMISSAOvalor);

                        // Adiciona cellSERIE como a célula da segunda coluna
                        PdfPCell cellEMISSAO = new PdfPCell(tabelaEMISSAO)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        tabiden.AddCell(cellEMISSAO);

                        //--------------------------------------------------
                        PdfPTable tabelaFOLHA = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };
                        // SERIE (título)
                        PdfPCell FOLHAteble = new PdfPCell(new Phrase("FOLHA", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelaFOLHA.AddCell(FOLHAteble);

                        // SERIE (valor)
                        PdfPCell FOLHAvalor = new PdfPCell(new Phrase("1/1", font8))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelaFOLHA.AddCell(FOLHAvalor);

                        // Adiciona cellSERIE como a célula da segunda coluna
                        PdfPCell cellFOLHA = new PdfPCell(tabelaFOLHA)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        tabiden.AddCell(cellFOLHA);

                        doc.Add(tabiden);
                        #endregion

                        #region | DESTINATARIO
                        PdfPTable DESTINATARIO = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // Adiciona o texto "DESTINATÁRIO" diretamente
                        PdfPCell cellDESTINATARIO = new PdfPCell(new Phrase("DESTINATÁRIO", font6BOLD))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };

                        DESTINATARIO.AddCell(cellDESTINATARIO);
                        doc.Add(DESTINATARIO);

                        PdfPTable tbldestinario = new PdfPTable(4)
                        {
                            WidthPercentage = 100
                        };
                        tbldestinario.SetWidths(new float[] { 50f, 10f, 20f, 20f });

                        //--------------------------------------------------
                        PdfPTable tabelaNOME = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // INSCRIÇÃO ESTADUAL (título)
                        PdfPCell NOMELabel = new PdfPCell(new Phrase("NOME / RAZÃO SOCIAL", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelaNOME.AddCell(NOMELabel);

                        // INSCRIÇÃO ESTADUAL (valor)
                        PdfPCell NOMEVALOR = new PdfPCell(new Phrase(DESTxNome, font8))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelaNOME.AddCell(NOMEVALOR);

                        // Adiciona tabelaIE como a célula da primeira coluna
                        PdfPCell cellNOME = new PdfPCell(tabelaNOME)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Colspan = 3
                        };
                        tbldestinario.AddCell(cellNOME);

                        //--------------------------------------------------
                        PdfPTable tabelaCNPJCPF = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // CNPJ (título)
                        PdfPCell CNPJCPFtable = new PdfPCell(new Phrase("CNPJ / CPF", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelaCNPJCPF.AddCell(CNPJCPFtable);

                        // CNPJ (valor)
                        PdfPCell CNPJCPFvalor = new PdfPCell(new Phrase(DESTCNPJ, font8))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelaCNPJCPF.AddCell(CNPJCPFvalor);

                        // Adiciona tabelaIE como a célula da primeira coluna
                        PdfPCell cellCNPJCPF = new PdfPCell(tabelaCNPJCPF)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        tbldestinario.AddCell(cellCNPJCPF);

                        //--------------------------------------------------
                        PdfPTable tabelaEndereco = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // ENDEREÇO (título)
                        PdfPCell Enderecotable = new PdfPCell(new Phrase("ENDEREÇO", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelaEndereco.AddCell(Enderecotable);

                        // ENDEREÇO (valor)
                        PdfPCell Enderecovalor = new PdfPCell(new Phrase(DESTxLgr + ", " + DESTnro + " - " + DESTxCpl, font8))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelaEndereco.AddCell(Enderecovalor);

                        // Adiciona ENDEREÇO como a célula da primeira coluna
                        PdfPCell cellEndereco = new PdfPCell(tabelaEndereco)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Colspan = 2
                        };
                        tbldestinario.AddCell(cellEndereco);

                        //--------------------------------------------------
                        PdfPTable tabelaBairro = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // ENDEREÇO (título)
                        PdfPCell Bairrotable = new PdfPCell(new Phrase("BAIRRO / DISTRITO", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelaBairro.AddCell(Bairrotable);

                        // ENDEREÇO (valor)
                        PdfPCell Bairrovalor = new PdfPCell(new Phrase(DESTxBairro, font8))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelaBairro.AddCell(Bairrovalor);

                        // Adiciona ENDEREÇO como a célula da primeira coluna
                        PdfPCell cellBairro = new PdfPCell(tabelaBairro)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        tbldestinario.AddCell(cellBairro);

                        //--------------------------------------------------
                        PdfPTable tabelaCep = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // ENDEREÇO (título)
                        PdfPCell Ceptable = new PdfPCell(new Phrase("CEP", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelaCep.AddCell(Ceptable);

                        // ENDEREÇO (valor)
                        PdfPCell Cepvalor = new PdfPCell(new Phrase(DESTCEP, font8))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelaCep.AddCell(Cepvalor);

                        // Adiciona ENDEREÇO como a célula da primeira coluna
                        PdfPCell cellCep = new PdfPCell(tabelaCep)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        tbldestinario.AddCell(cellCep);

                        //--------------------------------------------------
                        PdfPTable tabelaMunicipio = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // MUNICÍPIO (título)
                        PdfPCell Municipiotable = new PdfPCell(new Phrase("MUNICÍPIO", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelaMunicipio.AddCell(Municipiotable);

                        // MUNICÍPIO (valor)
                        PdfPCell Municipiovalor = new PdfPCell(new Phrase(DESTxMun, font8))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelaMunicipio.AddCell(Municipiovalor);

                        // Adiciona MUNICÍPIO como a célula da primeira coluna
                        PdfPCell cellMunicipio = new PdfPCell(tabelaMunicipio)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        tbldestinario.AddCell(cellMunicipio);

                        //--------------------------------------------------
                        PdfPTable tabelaUF = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // UF (título)
                        PdfPCell UFtable = new PdfPCell(new Phrase("UF", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelaUF.AddCell(UFtable);

                        // UF (valor)
                        PdfPCell UFvalor = new PdfPCell(new Phrase(DESTUF, font8))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelaUF.AddCell(UFvalor);

                        // Adiciona UF como a célula da primeira coluna
                        PdfPCell cellUF = new PdfPCell(tabelaUF)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        tbldestinario.AddCell(cellUF);

                        //--------------------------------------------------
                        PdfPTable tabelaFONE = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // FONE (título)
                        PdfPCell FONEtable = new PdfPCell(new Phrase("FONE / FAX", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelaFONE.AddCell(FONEtable);

                        // FONE (valor)
                        PdfPCell FONEvalor = new PdfPCell(new Phrase("", font8))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelaFONE.AddCell(FONEvalor);

                        // Adiciona FONE como a célula da primeira coluna
                        PdfPCell cellFONE = new PdfPCell(tabelaFONE)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        tbldestinario.AddCell(cellFONE);

                        //--------------------------------------------------
                        PdfPTable tabelaESTADUAL = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // ESTADUAL (título)
                        PdfPCell ESTADUALtable = new PdfPCell(new Phrase("INSCRIÇÃO ESTADUAL", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelaESTADUAL.AddCell(ESTADUALtable);

                        // ESTADUAL (valor)
                        PdfPCell ESTADUALvalor = new PdfPCell(new Phrase(DESIE, font8))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelaESTADUAL.AddCell(ESTADUALvalor);

                        // Adiciona ESTADUAL como a célula da primeira coluna
                        PdfPCell cellESTADUAL = new PdfPCell(tabelaESTADUAL)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        tbldestinario.AddCell(cellESTADUAL);

                        doc.Add(tbldestinario);
                        #endregion

                        #region | CONDIÇÃO DE USO
                        PdfPTable Condicao = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // Adiciona o texto "DESTINATÁRIO" diretamente
                        PdfPCell cellCondicao = new PdfPCell(new Phrase("CONDIÇÃO DE USO", font6BOLD))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };

                        Condicao.AddCell(cellCondicao);
                        doc.Add(Condicao);

                        PdfPTable tabcondicao = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };
                        tabcondicao.SetWidths(new float[] { 100f });

                        //--------------------------------------------------
                        PdfPTable tabelacondicao = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // INSCRIÇÃO ESTADUAL (valor)
                        PdfPCell condicaoVALOR = new PdfPCell(new Phrase(xCondUso, fon8BOLD))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelacondicao.AddCell(condicaoVALOR);

                        // Adiciona tabelaIE como a célula da primeira coluna
                        PdfPCell cellcondicao = new PdfPCell(tabelacondicao)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Colspan = 3
                        };
                        tabcondicao.AddCell(cellcondicao);

                        doc.Add(tabcondicao);
                        #endregion

                        #region | Eventos
                        PdfPTable EVENTOS = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // Adiciona o texto "DESTINATÁRIO" diretamente
                        PdfPCell cellEVENTOS = new PdfPCell(new Phrase("EVENTOS / CORREÇÕES", font6BOLD))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };

                        EVENTOS.AddCell(cellEVENTOS);
                        doc.Add(EVENTOS);

                        PdfPTable tabEventos = new PdfPTable(4)
                        {
                            WidthPercentage = 100
                        };
                        tabEventos.SetWidths(new float[] { 10f, 50f, 20f, 20f });

                        //--------------------------------------------------
                        PdfPTable tabelSEG = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        PdfPCell SEGLabel = new PdfPCell(new Phrase("SEQ", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelSEG.AddCell(SEGLabel);

                        // INSCRIÇÃO ESTADUAL (valor)
                        PdfPCell SEGVALOR = new PdfPCell(new Phrase("1", fon8BOLD))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelSEG.AddCell(SEGVALOR);

                        // Adiciona tabelaIE como a célula da primeira coluna
                        PdfPCell cellSEG = new PdfPCell(tabelSEG)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER
                        };
                        tabEventos.AddCell(cellSEG);

                        //--------------------------------------------------
                        PdfPTable tabelSTATUS = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        PdfPCell STATUSLabel = new PdfPCell(new Phrase("STATUS / MOTIVO", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelSTATUS.AddCell(STATUSLabel);

                        // INSCRIÇÃO ESTADUAL (valor)
                        PdfPCell STATUSVALOR = new PdfPCell(new Phrase(cStat + " " + xMotivo, fon8BOLD))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelSTATUS.AddCell(STATUSVALOR);

                        // Adiciona tabelaIE como a célula da primeira coluna
                        PdfPCell cellSTATUS = new PdfPCell(tabelSTATUS)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER
                        };
                        tabEventos.AddCell(cellSTATUS);

                        //--------------------------------------------------
                        PdfPTable tabelDATA = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        PdfPCell DATALabel = new PdfPCell(new Phrase("DATA DO REGISTRO", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelDATA.AddCell(DATALabel);

                        // INSCRIÇÃO ESTADUAL (valor)
                        PdfPCell DATAVALOR = new PdfPCell(new Phrase(dataFormatada, fon8BOLD))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelDATA.AddCell(DATAVALOR);

                        // Adiciona tabelaIE como a célula da primeira coluna
                        PdfPCell cellDATA = new PdfPCell(tabelDATA)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER
                        };
                        tabEventos.AddCell(cellDATA);

                        //--------------------------------------------------
                        PdfPTable tabelPROTOCOLO = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        PdfPCell PROTOCOLOLabel = new PdfPCell(new Phrase("NÚMERO DO PROTOCOLO", font6))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 1f,
                            PaddingTop = 1f
                        };
                        tabelPROTOCOLO.AddCell(PROTOCOLOLabel);

                        // INSCRIÇÃO ESTADUAL (valor)
                        PdfPCell PROTOCOLOVALOR = new PdfPCell(new Phrase(nProt, fon8BOLD))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 1f
                        };
                        tabelPROTOCOLO.AddCell(PROTOCOLOVALOR);

                        // Adiciona tabelaIE como a célula da primeira coluna
                        PdfPCell cellPROTOCOLO = new PdfPCell(tabelPROTOCOLO)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER
                        };
                        tabEventos.AddCell(cellPROTOCOLO);

                        //--------------------------------------------------
                        // Tabela para correção
                        PdfPTable tabelcorrecao = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        // Linha separadora com borda no topo
                        PdfPCell correcaoLabel = new PdfPCell(new Phrase("", font6))
                        {
                            Border = iTextSharp.text.Rectangle.TOP_BORDER, // Apenas linha no topo
                            BorderWidthTop = 1f,    // Espessura da linha
                            Colspan = 1,
                            PaddingBottom = 2f,
                            PaddingTop = 2f
                        };
                        tabelcorrecao.AddCell(correcaoLabel);

                        // Conteúdo da correção
                        PdfPCell correcaoVALOR = new PdfPCell(new Phrase("CORREÇÃO: '" + xCorrecao + "'", font10))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_TOP,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            PaddingBottom = 2f,
                            PaddingTop = 2f,
                            MinimumHeight = 500f
                        };
                        tabelcorrecao.AddCell(correcaoVALOR);

                        // Célula principal que vai ocupar as 4 colunas e conter essa sub-tabela
                        PdfPCell cellcorrecao = new PdfPCell(tabelcorrecao)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = iTextSharp.text.Rectangle.NO_BORDER,
                            Colspan = 4 // Ocupa toda a largura
                        };
                        tabEventos.AddCell(cellcorrecao);


                        PdfPCell cellBorda = new PdfPCell(tabEventos)
                        {
                            Border = iTextSharp.text.Rectangle.BOX, // Borda em volta da tabela toda
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };

                        // Tabela externa com 1 coluna
                        PdfPTable tblComBorda = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };
                        tblComBorda.AddCell(cellBorda);

                        // Adiciona ao documento
                        doc.Add(tblComBorda);

                        #endregion

                        doc.Close();
                    }



                }

                sRetorno = sNomeArquivo_PDF_CCe;

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar CCe: " + ex.Message);
            }




            return sRetorno;
        }
    }
}
