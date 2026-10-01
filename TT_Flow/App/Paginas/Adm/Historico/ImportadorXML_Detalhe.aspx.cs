using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using TT.FrameWork;
using TT_Flow.App.Controles;
using TT_Flow.App.Paginas.WMS.Manutencao;
using static Permissao.Administracao;
using static TT.FrameWork.BD;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using GRID = TT.FrameWork.Grid;
using IDENTITY = TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Paginas.Adm.Historico
{
    public partial class ImportadorXML_Detalhe : System.Web.UI.Page
    {
        #region | Classes
        string sTituloPagina = "Importação de XML";
        string sProcedure = "sp_Manipula_tbl_Flow_Arquivos";
        string IncluirArquivo = "S";
        public static int nItem = 0;
        public static int idItens = 0;
        public static string idEmpresa = "0";
        public static string Alterar_nNF = "";
        public static string sGerarNFe = "";

        public List<TT_Flow.FrameWork.cls_Importador_Itens> Base_Importador_Itens
        {
            get
            {
                if (ViewState["Base_Importador_Itens"] == null)
                {
                    ViewState["Base_Importador_Itens"] = new List<FrameWork.cls_Importador_Itens>();
                }
                return (List<FrameWork.cls_Importador_Itens>)ViewState["Base_Importador_Itens"];
            }

            set
            {
                ViewState["Base_Importador_Itens"] = value;
            }
        }
        #endregion

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            if (!IsPostBack)
            {
                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Financeiro.ImportadorNFe.Consultar, true);
                    Pesquisar(Request["id"].ToString(), false);
                    hddidImportador.Value = Request["id"].ToString();
                }
                if (Request["Novo"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Financeiro.ImportadorNFe.Consultar, true);
                    Pesquisar("", true);
                }
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                var requestArgs = this.Request["__EVENTARGUMENT"];

                if (requestTarget == "funcao_SAIR")
                {
                    FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                }
            }
            RegistraScript("");
        }
        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar(string idImportador, bool Novo)
        {
            string sErro = "";
            try
            {
                ddlModoFrete.Attributes.Add("disabled", "disabled");
                if (ValidarCampos(Novo))
                {
                    if (idImportador != "0")
                    {
                        Base_Importador_Itens.Clear();
                        DataSet dsPesquisa;
                        Dictionary<String, String> vParametros = new Dictionary<string, string>();
                        vParametros.Add("@sfuncao", "CONSULTAR_DETALHE");
                        vParametros.Add("@idImportador", idImportador);
                        dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ImportadorNfe", vParametros);

                        if (BD.ValidarDataSet(dsPesquisa, out sErro))
                        {
                            //emit
                            hddidImportador.Value = RETORNO.DATASET(dsPesquisa, 0, "idImportador");
                            hddidArquivo.Value = RETORNO.DATASET(dsPesquisa, 0, "idArquivo");
                            idEmpresa = RETORNO.DATASET(dsPesquisa, 0, "idEmpresa");
                            hddsNomeArquivo.Value = RETORNO.DATASET(dsPesquisa, 0, "sNomeArquivo");
                            hddsCaminho_UniNFe.Value = RETORNO.DATASET(dsPesquisa, 0, "sCaminho_UniNFe");
                            hddidStatus.Value = RETORNO.DATASET(dsPesquisa, 0, "idStatus");
                            Alterar_nNF = RETORNO.DATASET(dsPesquisa, 0, "nNF");
                            sGerarNFe = RETORNO.DATASET(dsPesquisa, 0, "sGerarNFe");
                            hddidTipoObjeto.Value = RETORNO.DATASET(dsPesquisa, 2, 0, "idTipoObjeto");

                            if (RETORNO.DATASET(dsPesquisa, 0, "sCNPJ_Emit") != "")
                                txtemitCNPJ.Text = RETORNO.DATASET(dsPesquisa, 0, "sCNPJ_Emit");
                            else
                                DIV_CNPJ.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "sNome_Emit") != "")
                                txtemitxNome.Text = RETORNO.DATASET(dsPesquisa, 0, "sNome_Emit");
                            else
                                DIV_xNome.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "xFant_Emit") != "")
                                txtemitxFant.Text = RETORNO.DATASET(dsPesquisa, 0, "xFant_Emit");
                            else
                                DIV_xFant.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "xLgr_Emit") != "")
                            {
                                txtemitxLgr.Text = RETORNO.DATASET(dsPesquisa, 0, "xLgr_Emit");
                                hddsEnderecoEmit.Value = "S";
                            }
                            else
                            {
                                DIV_xLgr.Visible = false;
                                hddsEnderecoEmit.Value = "N";
                            }

                            if (RETORNO.DATASET(dsPesquisa, 0, "nro_Emit") != "")
                                txtemitnro.Text = RETORNO.DATASET(dsPesquisa, 0, "nro_Emit");
                            else
                                DIV_nro.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "xBairro_Emit") != "")
                                txtemitxBairro.Text = RETORNO.DATASET(dsPesquisa, 0, "xBairro_Emit");
                            else
                                DIV_xBairro.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "cMun_Emit") != "")
                                txtemitcMun.Text = RETORNO.DATASET(dsPesquisa, 0, "cMun_Emit");
                            else
                                DIV_cMun.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "xMun_Emit") != "")
                                txtemitxMun.Text = RETORNO.DATASET(dsPesquisa, 0, "xMun_Emit");
                            else
                                DIV_xMun.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "UF_Emit") != "")
                                txtemitUF.Text = RETORNO.DATASET(dsPesquisa, 0, "UF_Emit");
                            else
                                DIV_UF.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "CEP_Emit") != "")
                                txtemitCEP.Text = RETORNO.DATASET(dsPesquisa, 0, "CEP_Emit");
                            else
                                DIV_CEP.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "cPais_Emit") != "")
                                txtemitcPais.Text = RETORNO.DATASET(dsPesquisa, 0, "cPais_Emit");
                            else
                                DIV_cPais.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "xPais_Emit") != "")
                                txtemitxPais.Text = RETORNO.DATASET(dsPesquisa, 0, "xPais_Emit");
                            else
                                DIV_xPais.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "fone_Emit") != "")
                                txtemitfone.Text = RETORNO.DATASET(dsPesquisa, 0, "fone_Emit");
                            else
                                DIV_fone.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "IE_Emit") != "")
                                txtemitIE.Text = RETORNO.DATASET(dsPesquisa, 0, "IE_Emit");
                            else
                                DIV_IE.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "CRT_Emit") != "")
                                txtemitCRT.Text = RETORNO.DATASET(dsPesquisa, 0, "CRT_Emit");
                            else
                                DIV_CRT.Visible = false;

                            // Dest
                            if (RETORNO.DATASET(dsPesquisa, 0, "sCNPJ_Dest") != "")
                                txtdestCNPJ.Text = RETORNO.DATASET(dsPesquisa, 0, "sCNPJ_Dest");
                            else
                                DIV_destCNPJ.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "sNome_Dest") != "")
                                txtdestxNome.Text = RETORNO.DATASET(dsPesquisa, 0, "sNome_Dest");
                            else
                                DIV_destxNome.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "xLgr_Dest") != "")
                            {
                                txtdestxLgr.Text = RETORNO.DATASET(dsPesquisa, 0, "xLgr_Dest");
                                hddsEnderecoDest.Value = "S";
                            }
                            else
                            {
                                hddsEnderecoDest.Value = "N";
                                DIV_destxLgr.Visible = false;
                            }

                            nNF.Text = RETORNO.DATASET(dsPesquisa, 0, "nNFe");
                            cUF.Text = RETORNO.DATASET(dsPesquisa, 0, "cUF");
                            cNF.Text = RETORNO.DATASET(dsPesquisa, 0, "cNF");
                            natOp.Text = RETORNO.DATASET(dsPesquisa, 0, "natOp");
                            mod.Text = RETORNO.DATASET(dsPesquisa, 0, "mod");
                            serie.Text = RETORNO.DATASET(dsPesquisa, 0, "serie");
                            dhEmi.Text = RETORNO.DATASET(dsPesquisa, 0, "dhEmi");
                            dhSaiEnt.Text = RETORNO.DATASET(dsPesquisa, 0, "dhSaiEnt");
                            tpNF.Text = RETORNO.DATASET(dsPesquisa, 0, "tpNF");
                            idDest.Text = RETORNO.DATASET(dsPesquisa, 0, "idDest");
                            cMunFG.Text = RETORNO.DATASET(dsPesquisa, 0, "cMunFG");
                            tpImp.Text = RETORNO.DATASET(dsPesquisa, 0, "tpImp");
                            tpEmis.Text = RETORNO.DATASET(dsPesquisa, 0, "tpEmis");
                            cDV.Text = RETORNO.DATASET(dsPesquisa, 0, "cDV");
                            tpAmb.Text = RETORNO.DATASET(dsPesquisa, 0, "tpAmb");
                            finNFe.Text = RETORNO.DATASET(dsPesquisa, 0, "finNFe");
                            indFinal.Text = RETORNO.DATASET(dsPesquisa, 0, "indFinal");
                            indPres.Text = RETORNO.DATASET(dsPesquisa, 0, "indPres");
                            procEmi.Text = RETORNO.DATASET(dsPesquisa, 0, "procEmi");
                            verProc.Text = RETORNO.DATASET(dsPesquisa, 0, "verProc");

                            vBCTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vBC_Total");
                            vICMSTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vICMS_Total");
                            vICMSDesonTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vICMSDeson_Total");
                            vFCPTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vFCP_Total");
                            vBCSTTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vBCST_Total");
                            vSTTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vST_Total");
                            vFCPSTTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vFCPST_Total");
                            vFCPSTRetTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vFCPSTRet_Total");
                            vProdTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vProd_Total");
                            vFreteTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vFrete_Total");
                            vSegTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vSeg_Total");
                            vDescTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vDesc_Total");
                            vIITotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vII_Total");
                            vIPITotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vIPI_Total");
                            vIPIDevolTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vIPIDevol_Total");
                            vPISTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vPIS_Total");
                            vCOFINSTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vCOFINS_Total");
                            vOutroTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vOutro_Total");
                            vNFTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vNF_Total");
                            vTotTribTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "vTotTrib_Total");
                            txtInfoFisco.Text = RETORNO.DATASET(dsPesquisa, 0, "infAdFisco");
                            txtInfoComplementares.Text = RETORNO.DATASET(dsPesquisa, 0, "infCpl");
                            ViewerPDF(RETORNO.DATASET(dsPesquisa, 0, "idArquivoPDF"));
                            if (RETORNO.DATASET(dsPesquisa, 0, "nro_Dest") != "")
                                txtdestnro.Text = RETORNO.DATASET(dsPesquisa, 0, "nro_Dest");
                            else
                                DIV_destnro.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "xCpl_Dest") != "")
                                txtdestxCpl.Text = RETORNO.DATASET(dsPesquisa, 0, "xCpl_Dest");
                            else
                                DIV_destxCpl.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "xBairro_Dest") != "")
                                txtdestxBairro.Text = RETORNO.DATASET(dsPesquisa, 0, "xBairro_Dest");
                            else
                                DIV_destxBairro.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "cMun_Dest") != "")
                                txtdestcMun.Text = RETORNO.DATASET(dsPesquisa, 0, "cMun_Dest");
                            else
                                DIV_destcMun.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "xMun_Dest") != "")
                                txtdestxMun.Text = RETORNO.DATASET(dsPesquisa, 0, "xMun_Dest");
                            else
                                DIV_destxMun.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "UF_Dest") != "")
                                txtdestUF.Text = RETORNO.DATASET(dsPesquisa, 0, "UF_Dest");
                            else
                                DIV_destUF.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "cPais_Dest") != "")
                                txtdestcPais.Text = RETORNO.DATASET(dsPesquisa, 0, "cPais_Dest");
                            else
                                DIV_destcPais.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "xPais_Dest") != "")
                                txtdestxPais.Text = RETORNO.DATASET(dsPesquisa, 0, "xPais_Dest");
                            else
                                DIV_destxPais.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "indIEDest_Dest") != "")
                                txtdestindIEDest.Text = RETORNO.DATASET(dsPesquisa, 0, "indIEDest_Dest");
                            else
                                DIV_destindIEDest.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, 0, "IE_Dest") != "")
                                txtIEdest.Text = RETORNO.DATASET(dsPesquisa, 0, "IE_Dest");
                            else
                                DIV_IEdest.Visible = false;

                            //transp
                            if (RETORNO.DATASET(dsPesquisa, 0, "modFrete") != "")
                                ddlModoFrete.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "modFrete");
                            //else
                            //    div_ModoFrete.Visible = false;
                            if (RETORNO.DATASET(dsPesquisa, 0, "CNPJtransp") != "")
                                txtCNPJTrans.Text = RETORNO.DATASET(dsPesquisa, 0, "CNPJtransp");
                            //else
                            //    DIV_CNPJTrans.Visible = false;
                            if (RETORNO.DATASET(dsPesquisa, 0, "xNometransp") != "")
                                txtNomeTrans.Text = RETORNO.DATASET(dsPesquisa, 0, "xNometransp");
                            //else
                            //DIV_NomeTrans.Visible = false;
                            if (RETORNO.DATASET(dsPesquisa, 0, "IEtransp") != "")
                                txtIETrans.Text = RETORNO.DATASET(dsPesquisa, 0, "IEtransp");
                            //else
                            //DIV_IETrans.Visible = false;
                            if (RETORNO.DATASET(dsPesquisa, 0, "xEndertransp") != "")
                                txtEnderecoTrans.Text = RETORNO.DATASET(dsPesquisa, 0, "xEndertransp");
                            //else
                            //DIV_EndereçoTrans.Visible = false;
                            if (RETORNO.DATASET(dsPesquisa, 0, "xMuntransp") != "")
                                txtMunicipiotrans.Text = RETORNO.DATASET(dsPesquisa, 0, "xMuntransp");
                            //else
                            //DIV_MunicípioTrans.Visible = false;
                            if (RETORNO.DATASET(dsPesquisa, 0, "UFtransp") != "")
                                txtUFTrans.Text = RETORNO.DATASET(dsPesquisa, 0, "UFtransp");
                            //else
                            //DIV_UFTrans.Visible = false;
                            if (RETORNO.DATASET(dsPesquisa, 0, "qVol") != "")
                                txtVolumes.Text = RETORNO.DATASET(dsPesquisa, 0, "qVol");
                            //else
                            //DIV_VolumesTrans.Visible = false;
                            if (RETORNO.DATASET(dsPesquisa, 0, "esp") != "")
                                txtEspecie.Text = RETORNO.DATASET(dsPesquisa, 0, "esp");
                            //else
                            //DIV_EspécieTrans.Visible = false;
                            if (RETORNO.DATASET(dsPesquisa, 0, "pesoL") != "")
                                txtPesoLiquido.Text = RETORNO.DATASET(dsPesquisa, 0, "pesoL");
                            //else
                            //DIV_LíquidoTrans.Visible = false;
                            if (RETORNO.DATASET(dsPesquisa, 0, "pesoB") != "")
                                txtPesoBruto.Text = RETORNO.DATASET(dsPesquisa, 0, "pesoB");
                            //else
                            //DIV_BrutoTrans.Visible = false;

                            foreach (DataRow row in dsPesquisa.Tables[1].Rows)
                            {
                                FrameWork.cls_Importador_Itens objItem = new FrameWork.cls_Importador_Itens();

                                objItem.nItem = int.Parse(row["nItem"].ToString());
                                objItem.idItens = int.Parse(row["idItens"].ToString());
                                objItem.idImportador = int.Parse(row["idImportador"].ToString());
                                objItem.cProd = row["cProd"].ToString();
                                objItem.xProd = row["xProd"].ToString();
                                objItem.cEAN = row["cEAN"].ToString();
                                objItem.NCM = row["NCM"].ToString();
                                objItem.CFOP = row["CFOP"].ToString();
                                objItem.uCom = row["uCom"].ToString();
                                objItem.qCom = row["qCom"].ToString();
                                objItem.vUnCom = row["vUnCom"].ToString();
                                objItem.vProd = row["vProd"].ToString();
                                objItem.cEANTrib = row["cEANTrib"].ToString();
                                objItem.uTrib = row["uTrib"].ToString();
                                objItem.qTrib = row["qTrib"].ToString();
                                objItem.vUnTrib = row["vUnTrib"].ToString();
                                objItem.indTot = row["indTot"].ToString();
                                objItem.xPed = row["xPed"].ToString();
                                objItem.idProduto = int.Parse(row["idProduto"].ToString());
                                objItem.idPedido = int.Parse(row["idPedido"].ToString());
                                objItem.sCodigo = row["sCodigo"].ToString();
                                //ICMS
                                objItem.orig = row["orig"].ToString();
                                objItem.CST_ICMS = row["CST_ICMS"].ToString();
                                objItem.modBC = row["modBC"].ToString();
                                objItem.vBC_ICMS = row["vBC_ICMS"].ToString();
                                objItem.pRedBC = row["pRedBC"].ToString();
                                objItem.pICMS = row["pICMS"].ToString();
                                objItem.vICMS = row["vICMS"].ToString();
                                //IPI
                                objItem.cEnq = row["cEnq"].ToString();
                                objItem.CST_IPI = row["CST_IPI"].ToString();
                                objItem.vBC_IPI = row["vBC_IPI"].ToString();
                                objItem.pIPI = row["pIPI"].ToString();
                                objItem.vIPI = row["vIPI"].ToString();
                                //II
                                objItem.vBC_II = row["vBC_II"].ToString();
                                objItem.vDespAdu = row["vDespAdu"].ToString();
                                objItem.vII = row["vII"].ToString();
                                objItem.vIOF = row["vIOF"].ToString();
                                //PIS
                                objItem.CST_PIS = row["CST_PIS"].ToString();
                                objItem.vBC_PIS = row["vBC_PIS"].ToString();
                                objItem.pPIS = row["pPIS"].ToString();
                                objItem.vPIS = row["vPIS"].ToString();
                                //COFINS
                                objItem.CST_COFINS = row["CST_COFINS"].ToString();
                                objItem.vBC_COFINS = row["vBC_COFINS"].ToString();
                                objItem.pCOFINS = row["pCOFINS"].ToString();
                                objItem.vCOFINS = row["vCOFINS"].ToString();

                                Base_Importador_Itens.Add(objItem);
                            }
                            dtgvConsulta.DataSource = Base_Importador_Itens;
                            dtgvConsulta.DataBind();
                        }
                        else
                        {
                            throw new Exception(sErro);
                        }
                    }
                    else
                    {
                        BreadCrumb.TitulodaPagina = string.Format("Nova {0}", sTituloPagina);
                        lblTituloPagina.Text = string.Format("Nova {0}", sTituloPagina);
                        aba_PDF.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        protected void cmdXML_Click(object sender, EventArgs e)
        {
            try
            {
                if (fu_Arquivo.HasFile && ddlTipo.SelectedValue != "0")
                {
                    int numeroDeArquivos = fu_Arquivo.PostedFiles.Count;
                    foreach (HttpPostedFile postedFile in fu_Arquivo.PostedFiles)
                    {
                        string fileName = postedFile.FileName;
                        if (fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                        {
                            string filePath = fileName;
                            Byte[] lObjArquivo = null;
                            TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                            lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(postedFile.FileName, postedFile.InputStream);

                            SqlDataAdapter da = new SqlDataAdapter("sp_Manipula_tbl_Flow_Arquivos", TT.FrameWork.BD.StringDeConexao);

                            DataSet tabela = new DataSet();
                            da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            SqlCommand lObjCommand = new SqlCommand();

                            da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "INCLUIR";
                            da.SelectCommand.Parameters.Add("@idTipoArquivo", SqlDbType.Int).Value = 10010;
                            da.SelectCommand.Parameters.Add("@idObjeto", SqlDbType.Int).Value = 0;
                            da.SelectCommand.Parameters.Add("@sNomeArquivo", SqlDbType.VarChar).Value = fileName;
                            da.SelectCommand.Parameters.Add("@sDscArquivo", SqlDbType.VarChar).Value = txtEnviarArquivo_sDscArquivo.Text;
                            da.SelectCommand.Parameters.Add("@sObservacao", SqlDbType.VarChar).Value = "";
                            da.SelectCommand.Parameters.Add("@vbArquivo", SqlDbType.VarBinary).Value = lObjArquivo;
                            da.SelectCommand.Parameters.Add("@idUsuario", SqlDbType.Int).Value = IDENTITY.Variaveis.idUsuario();
                            da.SelectCommand.Parameters.Add("@dtExpiracaoDoc", SqlDbType.VarChar).Value = "";
                            da.SelectCommand.Parameters.Add("@dtRegistroDoc", SqlDbType.VarChar).Value = "";

                            try
                            {
                                da.Fill(tabela);
                                hddidArquivo.Value = RETORNO.DATASET(tabela, 0, "idArquivo");

                                string sErro = "";
                                DataSet dsPesquisa;
                                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                                vParametros.Add("@sfuncao", "CONSULTAR_DETALHE");
                                vParametros.Add("@idArquivo", hddidArquivo.Value);
                                dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                                if (BD.ValidarDataSet(dsPesquisa, out sErro))
                                {
                                    string xmlData = dsPesquisa.Tables[0].Rows[0]["vbArquivo"].ToString();
                                    hddsCaminho_UniNFe.Value = dsPesquisa.Tables[0].Rows[0]["sCaminho_UniNFe"].ToString();

                                    string sNomeArquivo = "";

                                    foreach (DataRow item in dsPesquisa.Tables[0].Rows)
                                    {
                                        FileStream lObjFile;
                                        sNomeArquivo = item["sNomeArquivo"].ToString().Replace(",", "");
                                        TT.FrameWork.Arquivo objArquivoPopula = new TT.FrameWork.Arquivo();
                                        lObjFile = objArquivoPopula.TransformarArrayBytesEmArquivo((byte[])item["vbArquivo"], Server.MapPath("~/Download/" + sNomeArquivo));
                                        lObjFile.Close();
                                    }
                                    hddsNomeArquivo.Value = sNomeArquivo;
                                    lblTituloPagina.Text = "Importador de NF-e: " + sNomeArquivo;
                                    string Arquivo = Server.MapPath("~/Download/" + sNomeArquivo);
                                    Base_Importador_Itens.Clear();
                                    SalvarXML(Arquivo);
                                    string ArquivoPDF = Funcoes_NFe.Gerar_PDF_NFe.GerarNFe(true, hddsCaminho_UniNFe.Value, Arquivo, "S");

                                    Byte[] lObjArquivoPDF = null;
                                    TT.FrameWork.Arquivo objArquivoPDF = new TT.FrameWork.Arquivo();
                                    using (FileStream fileStream = new FileStream(Server.MapPath("~/Download/" + ArquivoPDF), FileMode.Open, FileAccess.Read))
                                    {
                                        lObjArquivoPDF = objArquivoPDF.TransformaArquivoEmArrayBytes(Server.MapPath("~/Download/" + ArquivoPDF), fileStream);
                                    }

                                    SqlDataAdapter daPDF = new SqlDataAdapter("sp_Manipula_tbl_Flow_Arquivos", TT.FrameWork.BD.StringDeConexao);

                                    DataSet tabelaPDF = new DataSet();
                                    daPDF.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
                                    daPDF.SelectCommand.CommandType = CommandType.StoredProcedure;
                                    SqlCommand lObjCommandPDF = new SqlCommand();

                                    daPDF.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "INCLUIR";
                                    daPDF.SelectCommand.Parameters.Add("@idTipoArquivo", SqlDbType.Int).Value = 10011;
                                    daPDF.SelectCommand.Parameters.Add("@idObjeto", SqlDbType.Int).Value = 0;
                                    daPDF.SelectCommand.Parameters.Add("@sNomeArquivo", SqlDbType.VarChar).Value = ArquivoPDF;
                                    daPDF.SelectCommand.Parameters.Add("@sDscArquivo", SqlDbType.VarChar).Value = "";
                                    daPDF.SelectCommand.Parameters.Add("@sObservacao", SqlDbType.VarChar).Value = "";
                                    daPDF.SelectCommand.Parameters.Add("@vbArquivo", SqlDbType.VarBinary).Value = lObjArquivoPDF;
                                    daPDF.SelectCommand.Parameters.Add("@idUsuario", SqlDbType.Int).Value = IDENTITY.Variaveis.idUsuario();
                                    daPDF.SelectCommand.Parameters.Add("@dtExpiracaoDoc", SqlDbType.VarChar).Value = "";
                                    daPDF.SelectCommand.Parameters.Add("@dtRegistroDoc", SqlDbType.VarChar).Value = "";
                                    string idArquivoPDF = "0";
                                    try
                                    {
                                        daPDF.Fill(tabelaPDF);
                                        idArquivoPDF = RETORNO.DATASET(tabelaPDF, 0, "idArquivo");
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception(string.Format("Erro BD-DS: {0}", ex.Message));
                                    }

                                    DataSet dsPesquisaPDF;
                                    Dictionary<String, String> vParametrosPDF = new Dictionary<string, string>();
                                    vParametrosPDF.Add("@sfuncao", "Alterar_ArquivoPDF");
                                    vParametrosPDF.Add("@idArquivoPDF", idArquivoPDF);
                                    vParametrosPDF.Add("@idImportador", hddidImportador.Value);
                                    dsPesquisaPDF = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ImportadorNfe", vParametrosPDF);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(string.Format("Erro BD-DS: {0}", ex.Message));
                            }
                        }
                        else
                        {
                            MensagemPagina.MostraMensagem_Erro("O arquivo selecionado deve estar no formato XML. Por favor, escolha um arquivo com a extensão '.xml'.");
                            txtEnviarArquivo_sDscArquivo.Text = "";
                            Pesquisar("", true);
                        }
                    }
                    if (numeroDeArquivos == 1)
                        FUNCOES.DirecionaPagina("App/Paginas/Adm/Historico/ImportadorXML_Detalhe.aspx?id=" + hddidImportador.Value);
                    else
                        FUNCOES.DirecionaPagina("App/Paginas/Adm/Historico/ImportadorXML.aspx");
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Selecione o arquivo ou selecione o tipo");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro" + ex.Message);
            }
        }

        void SalvarXML(string filePath)
        {
            XDocument xdoc = XDocument.Load(filePath);
            string chave = "";
            string nNF = "";
            string emitCNPJ = "";
            string emitxNome = "";
            string emitxFant = "";
            string emitxLgr = "";
            string emitnro = "";
            string emitxBairro = "";
            string emitcMun = "";
            string emitxMun = "";
            string emitUF = "";
            string emitCEP = "";
            string emitcPais = "";
            string emitxPais = "";
            string emitfone = "";
            string emitIE = "";
            string emitCRT = "";

            string cUF = "";
            string cNF = "";
            string natOp = "";
            string mod = "";
            string serie = "";
            string dhEmi = "";
            string dhSaiEnt = "";
            string tpNF = "";
            string idDest = "";
            string cMunFG = "";
            string tpImp = "";
            string tpEmis = "";
            string cDV = "";
            string tpAmb = "";
            string finNFe = "";
            string indFinal = "";
            string indPres = "";
            string procEmi = "";
            string verProc = "";

            string vBCTotal = "";
            string vICMSTotal = "";
            string vICMSDesonTotal = "";
            string vFCPTotal = "";
            string vFCPSTRetTotal = "";
            string vBCSTTotal = "";
            string vSTTotal = "";
            string vFCPSTTotal = "";
            string vProdTotal = "";
            string vFreteTotal = "";
            string vSegTotal = "";
            string vDescTotal = "";
            string vIITotal = "";
            string vIPITotal = "";
            string vIPIDevolTotal = "";
            string vPISTotal = "";
            string vCOFINSTotal = "";
            string vOutroTotal = "";
            string vNFTotal = "";
            string vTotTribTotal = "";

            string txtInfoFisco = "";
            string txtInfoComplementares = "";

            string destCNPJ = "";
            string destxNome = "";
            string destxLgr = "";
            string destnro = "";
            string destxCpl = "";
            string destxBairro = "";
            string destcMun = "";
            string destxMun = "";
            string destUF = "";
            string destcPais = "";
            string destxPais = "";
            string destindIEDest = "";
            string IEdest = "";
            string modFrete = "";
            string CNPJtransp = "";
            string xNometransp = "";
            string IEtransp = "";
            string xEndertransp = "";
            string xMuntransp = "";
            string UFtransp = "";
            string qVol = "0";
            string esp = "";
            string pesoL = "0";
            string pesoB = "0";

            var Corpo = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}nfeProc").FirstOrDefault();
            var infNFeElement = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}infNFe").FirstOrDefault();
            if (infNFeElement != null)
            {
                chave = infNFeElement.Attribute("Id")?.Value?.Substring(3);
            }
            //ide
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}nNF").FirstOrDefault() != null)
                nNF = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}nNF").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}cUF").FirstOrDefault() != null)
                cUF = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}cUF").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}cNF").FirstOrDefault() != null)
                cNF = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}cNF").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}natOp").FirstOrDefault() != null)
                natOp = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}natOp").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}mod").FirstOrDefault() != null)
                mod = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}mod").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}serie").FirstOrDefault() != null)
                serie = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}serie").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}dhEmi").FirstOrDefault() != null)
                dhEmi = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}dhEmi").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}dhSaiEnt").FirstOrDefault() != null)
                dhSaiEnt = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}dhSaiEnt").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpNF").FirstOrDefault() != null)
                tpNF = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpNF").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}idDest").FirstOrDefault() != null)
                idDest = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}idDest").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}cMunFG").FirstOrDefault() != null)
                cMunFG = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}cMunFG").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpImp").FirstOrDefault() != null)
                tpImp = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpImp").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpEmis").FirstOrDefault() != null)
                tpEmis = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpEmis").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}cDV").FirstOrDefault() != null)
                cDV = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}cDV").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpAmb").FirstOrDefault() != null)
                tpAmb = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpAmb").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}finNFe").FirstOrDefault() != null)
                finNFe = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}finNFe").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}indFinal").FirstOrDefault() != null)
                indFinal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}indFinal").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}indPres").FirstOrDefault() != null)
                indPres = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}indPres").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}procEmi").FirstOrDefault() != null)
                procEmi = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}procEmi").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}verProc").FirstOrDefault() != null)
                verProc = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}verProc").FirstOrDefault()?.Value;

            //--emit
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                emitCNPJ = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;
            else
                DIV_CNPJ.Visible = false;

            DataSet dsPesquisaEmit;
            Dictionary<String, String> vParametrosEmit = new Dictionary<string, string>();
            vParametrosEmit.Add("@sfuncao", "VerificarParceiro");
            vParametrosEmit.Add("@CNPJtransp", emitCNPJ);
            dsPesquisaEmit = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ImportadorNfe", vParametrosEmit);
            string idParceiroEmit = "0";
            if (dsPesquisaEmit.Tables[0].Rows.Count > 0)
            {
                idParceiroEmit = dsPesquisaEmit.Tables[0].Rows[0]["idCliente"].ToString();
            }

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                emitxNome = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;
            else
                DIV_xNome.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xFant").FirstOrDefault() != null)
                emitxFant = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xFant").FirstOrDefault()?.Value;
            else
                DIV_xFant.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault() != null)
            {
                emitxLgr = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault()?.Value;
                hddsEnderecoEmit.Value = "S";
            }
            else
            {
                DIV_xLgr.Visible = false;
                hddsEnderecoEmit.Value = "N";
            }

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault() != null)
                emitnro = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault()?.Value;
            else
                DIV_nro.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault() != null)
                emitxBairro = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault()?.Value;
            else
                DIV_xBairro.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}cMun").FirstOrDefault() != null)
                emitcMun = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}cMun").FirstOrDefault()?.Value;
            else
                DIV_cMun.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault() != null)
                emitxMun = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault()?.Value;
            else
                DIV_xMun.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault() != null)
                emitUF = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value;
            else
                DIV_UF.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault() != null)
                emitCEP = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault()?.Value;
            else
                DIV_CEP.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}cPais").FirstOrDefault() != null)
                emitcPais = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}cPais").FirstOrDefault()?.Value;
            else
                DIV_cPais.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xPais").FirstOrDefault() != null)
                emitxPais = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xPais").FirstOrDefault()?.Value;
            else
                DIV_xPais.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}fone").FirstOrDefault() != null)
                emitfone = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}fone").FirstOrDefault()?.Value;
            else
                DIV_fone.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault() != null)
                emitIE = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault()?.Value;
            else
                DIV_IE.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CRT").FirstOrDefault() != null)
                emitCRT = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CRT").FirstOrDefault()?.Value;
            else
                DIV_CRT.Visible = false;
            //--
            //--dest
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null || xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CPF").FirstOrDefault() != null)
            {
                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                    destCNPJ = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;
                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CPF").FirstOrDefault() != null)
                {
                    destCNPJ = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CPF").FirstOrDefault()?.Value;
                    lblCPF_CNPJ.InnerText = "CPF";
                }
            }
            else
            {
                DIV_destCNPJ.Visible = false;
                destCNPJ = "0";
            }

            DataSet dsPesquisaDest;
            Dictionary<String, String> vParametrosDest = new Dictionary<string, string>();
            vParametrosDest.Add("@sfuncao", "VerificarParceiro");
            vParametrosDest.Add("@CNPJtransp", destCNPJ);
            dsPesquisaDest = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ImportadorNfe", vParametrosDest);
            string idParceiroDest = "0";
            if (dsPesquisaDest.Tables.Count > 0 && dsPesquisaDest.Tables[0].Rows.Count > 0)
            {
                idParceiroDest = dsPesquisaDest.Tables[0].Rows[0]["idCliente"].ToString();
            }

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                destxNome = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;
            else
                DIV_destxNome.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault() != null)
            {
                destxLgr = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault()?.Value;
                hddsEnderecoDest.Value = "S";
            }
            else
            {
                DIV_destxLgr.Visible = false;
                hddsEnderecoDest.Value = "N";
            }

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault() != null)
                destnro = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault()?.Value;
            else
                DIV_destnro.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xCpl").FirstOrDefault() != null)
                destxCpl = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xCpl").FirstOrDefault()?.Value;
            else
                DIV_destxCpl.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault() != null)
                destxBairro = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault()?.Value;
            else
                DIV_destxBairro.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}cMun").FirstOrDefault() != null)
                destcMun = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}cMun").FirstOrDefault()?.Value;
            else
                DIV_destcMun.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault() != null)
                destxMun = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault()?.Value;
            else
                DIV_destxMun.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault() != null)
                destUF = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value;
            else
                DIV_destUF.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}cPais").FirstOrDefault() != null)
                destcPais = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}cPais").FirstOrDefault()?.Value;
            else
                DIV_destcPais.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xPais").FirstOrDefault() != null)
                destxPais = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xPais").FirstOrDefault()?.Value;
            else
                DIV_destxPais.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}indIEDest").FirstOrDefault() != null)
                destindIEDest = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}indIEDest").FirstOrDefault()?.Value;
            else
                DIV_destindIEDest.Visible = false;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault() != null)
                IEdest = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault()?.Value;
            else
                DIV_IEdest.Visible = false;
            //--

            //TRANSP
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}modFrete").FirstOrDefault() != null)
                modFrete = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}modFrete").FirstOrDefault()?.Value;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                CNPJtransp = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                xNometransp = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault() != null)
                IEtransp = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault()?.Value;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xEnder").FirstOrDefault() != null)
                xEndertransp = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xEnder").FirstOrDefault()?.Value;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault() != null)
                xMuntransp = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault()?.Value;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault() != null)
                UFtransp = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}qVol").FirstOrDefault() != null)
                qVol = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}qVol").FirstOrDefault()?.Value;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}esp").FirstOrDefault() != null)
                esp = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}esp").FirstOrDefault()?.Value;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoL").FirstOrDefault() != null)
                pesoL = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoL").FirstOrDefault()?.Value;

            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoB").FirstOrDefault() != null)
                pesoB = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoB").FirstOrDefault()?.Value;

            //Total
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                vBCTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault() != null)
                vICMSTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMSDeson").FirstOrDefault() != null)
                vICMSDesonTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMSDeson").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCP").FirstOrDefault() != null)
                vFCPTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCP").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vBCST").FirstOrDefault() != null)
                vBCSTTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vBCST").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vST").FirstOrDefault() != null)
                vSTTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vST").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCPST").FirstOrDefault() != null)
                vFCPSTTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCPST").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCPSTRet").FirstOrDefault() != null)
                vFCPSTRetTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFCPSTRet").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vProd").FirstOrDefault() != null)
                vProdTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vProd").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFrete").FirstOrDefault() != null)
                vFreteTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vFrete").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vSeg").FirstOrDefault() != null)
                vSegTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vSeg").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vDesc").FirstOrDefault() != null)
                vDescTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vDesc").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault() != null)
                vIITotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault() != null)
                vIPITotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPIDevol").FirstOrDefault() != null)
                vIPIDevolTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPIDevol").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault() != null)
                vPISTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault() != null)
                vCOFINSTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vOutro").FirstOrDefault() != null)
                vOutroTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vOutro").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vNF").FirstOrDefault() != null)
                vNFTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vNF").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vTotTrib").FirstOrDefault() != null)
                vTotTribTotal = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}vTotTrib").FirstOrDefault()?.Value;

            //infAdic
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").Descendants("{http://www.portalfiscal.inf.br/nfe}infAdFisco").FirstOrDefault() != null)
                txtInfoFisco = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").Descendants("{http://www.portalfiscal.inf.br/nfe}infAdFisco").FirstOrDefault()?.Value;
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").Descendants("{http://www.portalfiscal.inf.br/nfe}infCpl").FirstOrDefault() != null)
                txtInfoComplementares = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}infAdic").Descendants("{http://www.portalfiscal.inf.br/nfe}infCpl").FirstOrDefault()?.Value;

            XNamespace ns = "{http://www.portalfiscal.inf.br/nfe}";
            foreach (var row in xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det"))
            {
                FrameWork.cls_Importador_Itens objItem = new FrameWork.cls_Importador_Itens();

                objItem.nItem = int.Parse(row.Attribute("nItem")?.Value);
                objItem.cProd = row.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}cProd").FirstOrDefault()?.Value;
                objItem.xProd = row.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}xProd").FirstOrDefault()?.Value;
                objItem.cEAN = row.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}cEAN").FirstOrDefault()?.Value;
                objItem.NCM = row.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}NCM").FirstOrDefault()?.Value;
                objItem.CFOP = row.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}CFOP").FirstOrDefault()?.Value;
                objItem.uCom = row.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}uCom").FirstOrDefault()?.Value;
                objItem.qCom = row.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}qCom").FirstOrDefault()?.Value;
                objItem.vUnCom = row.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}vUnCom").FirstOrDefault()?.Value;
                objItem.vProd = row.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}vProd").FirstOrDefault()?.Value;
                objItem.cEANTrib = row.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}cEANTrib").FirstOrDefault()?.Value;
                objItem.uTrib = row.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}uTrib").FirstOrDefault()?.Value;
                objItem.qTrib = row.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}qTrib").FirstOrDefault()?.Value;
                objItem.vUnTrib = row.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}vUnTrib").FirstOrDefault()?.Value;
                objItem.indTot = row.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}indTot").FirstOrDefault()?.Value;
                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}xPed").FirstOrDefault() != null)
                    objItem.xPed = row.Descendants("{http://www.portalfiscal.inf.br/nfe}prod").Descendants("{http://www.portalfiscal.inf.br/nfe}xPed").FirstOrDefault()?.Value;
                else
                    objItem.xPed = "";

                //ICMS
                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}orig").FirstOrDefault() != null)
                    objItem.orig = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}orig").FirstOrDefault()?.Value;
                else
                    objItem.orig = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault() != null)
                    objItem.CST_ICMS = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault()?.Value;
                else
                    objItem.CST_ICMS = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}modBC").FirstOrDefault() != null)
                    objItem.modBC = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}modBC").FirstOrDefault()?.Value;
                else
                    objItem.modBC = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                    objItem.vBC_ICMS = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault()?.Value;
                else
                    objItem.vBC_ICMS = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}pRedBC").FirstOrDefault() != null)
                    objItem.pRedBC = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}pRedBC").FirstOrDefault()?.Value;
                else
                    objItem.pRedBC = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}pICMS").FirstOrDefault() != null)
                    objItem.pICMS = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}pICMS").FirstOrDefault()?.Value;
                else
                    objItem.pICMS = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault() != null)
                    objItem.vICMS = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault()?.Value;
                else
                    objItem.vICMS = "";

                ////IPI
                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}cEnq").FirstOrDefault() != null)
                    objItem.cEnq = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}cEnq").FirstOrDefault()?.Value;
                else
                    objItem.cEnq = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault() != null)
                    objItem.CST_IPI = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault()?.Value;
                else
                    objItem.CST_IPI = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                    objItem.vBC_IPI = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault()?.Value;
                else
                    objItem.vBC_IPI = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}pIPI").FirstOrDefault() != null)
                    objItem.pIPI = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}pIPI").FirstOrDefault()?.Value;
                else
                    objItem.pIPI = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault() != null)
                    objItem.vIPI = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault()?.Value;
                else
                    objItem.vIPI = "";

                ////II
                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                    objItem.vBC_II = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault()?.Value;
                else
                    objItem.vBC_II = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vDespAdu").FirstOrDefault() != null)
                    objItem.vDespAdu = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vDespAdu").FirstOrDefault()?.Value;
                else
                    objItem.vDespAdu = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault() != null)
                    objItem.vII = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault()?.Value;
                else
                    objItem.vII = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vIOF").FirstOrDefault() != null)
                    objItem.vIOF = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vIOF").FirstOrDefault()?.Value;
                else
                    objItem.vIOF = "";

                ////PIS
                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault() != null)
                    objItem.CST_PIS = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault()?.Value;
                else
                    objItem.CST_PIS = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                    objItem.vBC_PIS = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault()?.Value;
                else
                    objItem.vBC_PIS = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}pPIS").FirstOrDefault() != null)
                    objItem.pPIS = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}pPIS").FirstOrDefault()?.Value;
                else
                    objItem.CST_PIS = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault() != null)
                    objItem.vPIS = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault()?.Value;
                else
                    objItem.vPIS = "";

                ////COFINS
                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault() != null)
                    objItem.CST_COFINS = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault()?.Value;
                else
                    objItem.CST_COFINS = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                    objItem.vBC_COFINS = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault()?.Value;
                else
                    objItem.vBC_COFINS = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}pCOFINS").FirstOrDefault() != null)
                    objItem.pCOFINS = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}pCOFINS").FirstOrDefault()?.Value;
                else
                    objItem.pCOFINS = "";

                if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault() != null)
                    objItem.vCOFINS = row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault()?.Value;
                else
                    objItem.vCOFINS = "";

                Base_Importador_Itens.Add(objItem);
            }

            DataSet dsSalvar;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sfuncao", "SALVAR");
            vParametros.Add("@idImportador", "");
            vParametros.Add("@idArquivo", hddidArquivo.Value);
            vParametros.Add("@idParceiro_Emit", idParceiroEmit);
            vParametros.Add("@idEndereco_Emit", "");
            vParametros.Add("@sCNPJ_Emit", emitCNPJ);
            vParametros.Add("@sNome_Emit", emitxNome);
            vParametros.Add("@xFant_Emit", emitxFant);
            vParametros.Add("@xLgr_Emit", emitxLgr);
            vParametros.Add("@nro_Emit", emitnro);
            vParametros.Add("@xCpl_Emit", "");
            vParametros.Add("@xBairro_Emit", emitxBairro);
            vParametros.Add("@cMun_Emit", emitcMun);
            vParametros.Add("@xMun_Emit", emitxMun);
            vParametros.Add("@UF_Emit", emitUF);
            vParametros.Add("@CEP_Emit", emitCEP);
            vParametros.Add("@cPais_Emit", emitcPais);
            vParametros.Add("@xPais_Emit", emitxPais);
            vParametros.Add("@fone_Emit", emitfone);
            vParametros.Add("@IE_Emit", emitIE);
            vParametros.Add("@CRT_Emit", emitCRT);
            vParametros.Add("@idParceiro_Dest", idParceiroDest);
            vParametros.Add("@idEndereco_Dest", "");
            vParametros.Add("@sCNPJ_Dest", destCNPJ);
            vParametros.Add("@sNome_Dest", destxNome);
            vParametros.Add("@xFant_Dest", "");
            vParametros.Add("@xLgr_Dest", destxLgr);
            vParametros.Add("@nro_Dest", destnro);
            vParametros.Add("@xCpl_Dest", destxCpl);
            vParametros.Add("@xBairro_Dest", destxBairro);
            vParametros.Add("@cMun_Dest", destcMun);
            vParametros.Add("@xMun_Dest", destxMun);
            vParametros.Add("@UF_Dest", destUF);
            vParametros.Add("@CEP_Dest", "");
            vParametros.Add("@cPais_Dest", destcPais);
            vParametros.Add("@xPais_Dest", destxPais);
            vParametros.Add("@IE_Dest", IEdest);
            vParametros.Add("@CRT_Dest", "");
            vParametros.Add("@indIEDest_Dest", destindIEDest);
            vParametros.Add("@nNF", "N");
            vParametros.Add("@modFrete", modFrete);
            vParametros.Add("@CNPJtransp", CNPJtransp);
            vParametros.Add("@xNometransp", xNometransp);
            vParametros.Add("@IEtransp", IEtransp);
            vParametros.Add("@xEndertransp", xEndertransp);
            vParametros.Add("@xMuntransp", xMuntransp);
            vParametros.Add("@UFtransp", UFtransp);
            vParametros.Add("@qVol", qVol);
            vParametros.Add("@esp", esp);
            vParametros.Add("@pesoL", string.IsNullOrEmpty(pesoL) ? "0" : pesoL);
            vParametros.Add("@pesoB", string.IsNullOrEmpty(pesoB) ? "0" : pesoB);
            vParametros.Add("@nNFe", nNF);
            vParametros.Add("@cUF", cUF);
            vParametros.Add("@cNF", cNF);
            vParametros.Add("@natOp", natOp);
            vParametros.Add("@mod", mod);
            vParametros.Add("@serie", serie);
            vParametros.Add("@dhEmi", dhEmi);
            vParametros.Add("@dhSaiEnt", dhSaiEnt);
            vParametros.Add("@tpNF", tpNF);
            vParametros.Add("@idDest", idDest);
            vParametros.Add("@cMunFG", cMunFG);
            vParametros.Add("@tpImp", tpImp);
            vParametros.Add("@tpEmis", tpEmis);
            vParametros.Add("@cDV", cDV);
            vParametros.Add("@tpAmb", tpAmb);
            vParametros.Add("@finNFe", finNFe);
            vParametros.Add("@indFinal", indFinal);
            vParametros.Add("@indPres", indPres);
            vParametros.Add("@procEmi", procEmi);
            vParametros.Add("@verProc", verProc);
            vParametros.Add("@vBC_Total", string.IsNullOrEmpty(vBCTotal) ? "0" : vBCTotal);
            vParametros.Add("@vICMS_Total", string.IsNullOrEmpty(vICMSTotal) ? "0" : vICMSTotal); // Corrigido (era vBCTotal)
            vParametros.Add("@vICMSDeson_Total", string.IsNullOrEmpty(vICMSDesonTotal) ? "0" : vICMSDesonTotal);
            vParametros.Add("@vFCP_Total", string.IsNullOrEmpty(vFCPTotal) ? "0" : vFCPTotal);
            vParametros.Add("@vBCST_Total", string.IsNullOrEmpty(vBCSTTotal) ? "0" : vBCSTTotal);
            vParametros.Add("@vST_Total", string.IsNullOrEmpty(vSTTotal) ? "0" : vSTTotal);
            vParametros.Add("@vFCPST_Total", string.IsNullOrEmpty(vFCPSTTotal) ? "0" : vFCPSTTotal);
            vParametros.Add("@vFCPSTRet_Total", string.IsNullOrEmpty(vFCPSTRetTotal) ? "0" : vFCPSTRetTotal);
            vParametros.Add("@vProd_Total", string.IsNullOrEmpty(vProdTotal) ? "0" : vProdTotal);
            vParametros.Add("@vFrete_Total", string.IsNullOrEmpty(vFreteTotal) ? "0" : vFreteTotal);
            vParametros.Add("@vSeg_Total", string.IsNullOrEmpty(vSegTotal) ? "0" : vSegTotal);
            vParametros.Add("@vDesc_Total", string.IsNullOrEmpty(vDescTotal) ? "0" : vDescTotal);
            vParametros.Add("@vII_Total", string.IsNullOrEmpty(vIITotal) ? "0" : vIITotal);
            vParametros.Add("@vIPI_Total", string.IsNullOrEmpty(vIPITotal) ? "0" : vIPITotal);
            vParametros.Add("@vIPIDevol_Total", string.IsNullOrEmpty(vIPIDevolTotal) ? "0" : vIPIDevolTotal);
            vParametros.Add("@vPIS_Total", string.IsNullOrEmpty(vPISTotal) ? "0" : vPISTotal);
            vParametros.Add("@vCOFINS_Total", string.IsNullOrEmpty(vCOFINSTotal) ? "0" : vCOFINSTotal);
            vParametros.Add("@vOutro_Total", string.IsNullOrEmpty(vOutroTotal) ? "0" : vOutroTotal);
            vParametros.Add("@vNF_Total", string.IsNullOrEmpty(vNFTotal) ? "0" : vNFTotal);
            vParametros.Add("@vTotTrib_Total", string.IsNullOrEmpty(vTotTribTotal) ? "0" : vTotTribTotal);
            vParametros.Add("@infAdFisco", txtInfoFisco);
            vParametros.Add("@infCpl", txtInfoComplementares);
            vParametros.Add("@idStatus", ddlTipo.SelectedValue);
            dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ImportadorNfe", vParametros);

            hddidImportador.Value = RETORNO.DATASET(dsSalvar, 0, "idImportador");

            foreach (var linha in Base_Importador_Itens)
            {
                DataSet dsSalvarItens;
                Dictionary<String, String> vParametrosItens = new Dictionary<string, string>();
                vParametrosItens.Add("@sfuncao", "SALVAR_ITENS");
                vParametrosItens.Add("@idItens", "0");
                vParametrosItens.Add("@idImportador", hddidImportador.Value);
                vParametrosItens.Add("@nItem", linha.nItem.ToString());
                vParametrosItens.Add("@cProd", linha.cProd);
                vParametrosItens.Add("@cEAN", linha.cEAN);
                vParametrosItens.Add("@xProd", linha.xProd);
                vParametrosItens.Add("@NCM", linha.NCM);
                vParametrosItens.Add("@CFOP", linha.CFOP);
                vParametrosItens.Add("@uCom", linha.uCom);
                vParametrosItens.Add("@qCom", string.IsNullOrEmpty(linha.qCom) ? "0" : linha.qCom);
                vParametrosItens.Add("@vUnCom", string.IsNullOrEmpty(linha.vUnCom) ? "0" : linha.vUnCom);
                vParametrosItens.Add("@vProd", string.IsNullOrEmpty(linha.vProd) ? "0" : linha.vProd);
                vParametrosItens.Add("@cEANTrib", linha.cEANTrib);
                vParametrosItens.Add("@uTrib", linha.uTrib);
                vParametrosItens.Add("@qTrib", string.IsNullOrEmpty(linha.qTrib) ? "0" : linha.qTrib);
                vParametrosItens.Add("@vUnTrib", string.IsNullOrEmpty(linha.vUnTrib) ? "0" : linha.vUnTrib);
                vParametrosItens.Add("@indTot", linha.indTot);
                if (linha.xPed != null)
                {
                    vParametrosItens.Add("@xPed", linha.xPed);
                }
                else
                {
                    vParametrosItens.Add("@xPed", "");
                }

                //ICMS (Campos não decimais mantidos com IF)
                if (linha.orig != "")
                    vParametrosItens.Add("@orig", linha.orig);

                if (linha.CST_ICMS != "")
                    vParametrosItens.Add("@CST_ICMS", linha.CST_ICMS);

                if (linha.modBC != "")
                    vParametrosItens.Add("@modBC", linha.modBC);

                //ICMS (Campos decimais com ternário)
                vParametrosItens.Add("@vBC_ICMS", string.IsNullOrEmpty(linha.vBC_ICMS) ? "0" : linha.vBC_ICMS);
                vParametrosItens.Add("@pRedBC", string.IsNullOrEmpty(linha.pRedBC) ? "0" : linha.pRedBC);
                vParametrosItens.Add("@pICMS", string.IsNullOrEmpty(linha.pICMS) ? "0" : linha.pICMS);
                vParametrosItens.Add("@vICMS", string.IsNullOrEmpty(linha.vICMS) ? "0" : linha.vICMS);

                //IPI (Campos não decimais mantidos com IF)
                if (linha.cEnq != "")
                    vParametrosItens.Add("@cEnq", linha.cEnq);

                if (linha.CST_IPI != "")
                    vParametrosItens.Add("@CST_IPI", linha.CST_IPI);

                //IPI (Campos decimais com ternário)
                vParametrosItens.Add("@vBC_IPI", string.IsNullOrEmpty(linha.vBC_IPI) ? "0" : linha.vBC_IPI);
                vParametrosItens.Add("@pIPI", string.IsNullOrEmpty(linha.pIPI) ? "0" : linha.pIPI);
                vParametrosItens.Add("@vIPI", string.IsNullOrEmpty(linha.vIPI) ? "0" : linha.vIPI);

                //II (Campos decimais com ternário)
                vParametrosItens.Add("@vBC_II", string.IsNullOrEmpty(linha.vBC_II) ? "0" : linha.vBC_II);
                vParametrosItens.Add("@vDespAdu", string.IsNullOrEmpty(linha.vDespAdu) ? "0" : linha.vDespAdu);
                vParametrosItens.Add("@vII", string.IsNullOrEmpty(linha.vII) ? "0" : linha.vII);
                vParametrosItens.Add("@vIOF", string.IsNullOrEmpty(linha.vIOF) ? "0" : linha.vIOF);

                //PIS (Campos não decimais mantidos com IF)
                if (linha.CST_PIS != "")
                    vParametrosItens.Add("@CST_PIS", linha.CST_PIS);

                //PIS (Campos decimais com ternário)
                vParametrosItens.Add("@vBC_PIS", string.IsNullOrEmpty(linha.vBC_PIS) ? "0" : linha.vBC_PIS);
                vParametrosItens.Add("@pPIS", string.IsNullOrEmpty(linha.pPIS) ? "0" : linha.pPIS);
                vParametrosItens.Add("@vPIS", string.IsNullOrEmpty(linha.vPIS) ? "0" : linha.vPIS);

                //COFINS (Campos não decimais mantidos com IF)
                if (linha.CST_COFINS != "")
                    vParametrosItens.Add("@CST_COFINS", linha.CST_COFINS);

                //COFINS (Campos decimais com ternário)
                vParametrosItens.Add("@vBC_COFINS", string.IsNullOrEmpty(linha.vBC_COFINS) ? "0" : linha.vBC_COFINS);
                vParametrosItens.Add("@pCOFINS", string.IsNullOrEmpty(linha.pCOFINS) ? "0" : linha.pCOFINS);
                vParametrosItens.Add("@vCOFINS", string.IsNullOrEmpty(linha.vCOFINS) ? "0" : linha.vCOFINS);

                dsSalvarItens = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ImportadorNfe", vParametrosItens);
            }
        }
        #endregion

        #region | Script 
        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            //ICMS
            sb.Append("$('[id*=txtorigICMS]').mask('0', { reverse: true });");
            sb.Append("$('[id*=txtCST]').mask('00', { reverse: true });");
            sb.Append("$('[id*=txtmodBC]').mask('0', { reverse: true });");
            sb.Append("$('[id*=txtvBC]').mask('000000000000000,00', { reverse: true });");
            sb.Append("$('[id*=txtpRedBC]').mask('0000000000000,0000', { reverse: true });");
            sb.Append("$('[id*=txtpICMS]').mask('000000000000000,00', { reverse: true });");
            sb.Append("$('[id*=txtvICMS]').mask('000000000000000,00', { reverse: true });");
            //IPI
            sb.Append("$('[id*=txtcEnq]').mask('000', { reverse: true });");
            sb.Append("$('[id*=txtCSTIPI]').mask('00', { reverse: true });");
            sb.Append("$('[id*=txtvBCIPI]').mask('000000000000000,00', { reverse: true });");
            sb.Append("$('[id*=txtpIPI]').mask('000000000000000,00', { reverse: true });");
            sb.Append("$('[id*=txtvIPI]').mask('000000000000000,00', { reverse: true });");
            //II
            sb.Append("$('[id*=txtvBCII]').mask('000000000000000,00', { reverse: true });");
            sb.Append("$('[id*=txtvDespAdu]').mask('000000000000000,00', { reverse: true });");
            sb.Append("$('[id*=txtvII]').mask('000000000000000,00', { reverse: true });");
            sb.Append("$('[id*=txtvIOF]').mask('000000000000000,00', { reverse: true });");
            //PIS
            sb.Append("$('[id*=txtCSTPIS]').mask('00', { reverse: true });");
            sb.Append("$('[id*=txtvBCPIS]').mask('000000000000000,00', { reverse: true });");
            sb.Append("$('[id*=txtpPIS]').mask('000000000000000,00', { reverse: true });");
            sb.Append("$('[id*=txtvPIS]').mask('000000000000000,00', { reverse: true });");
            //COFINS
            sb.Append("$('[id*=txtCSTCOFINS]').mask('00', { reverse: true });");
            sb.Append("$('[id*=txtvBCCOFINS]').mask('000000000000000,00', { reverse: true });");
            sb.Append("$('[id*=txtpCOFINS]').mask('000000000000000,00', { reverse: true });");
            sb.Append("$('[id*=txtvCOFINS]').mask('000000000000000,00', { reverse: true });");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina" + Guid.NewGuid(), sb.ToString(), true);
        }
        #endregion

        #region | Evento
        private bool ValidarCampos(bool Novo)
        {
            if (Novo == true)
            {
                DIV_DADOS.Visible = false;
                div_Importador.Visible = true;
                btnXML.Visible = true;
                aba_PDF.Visible = false;
                return false;
            }
            else
            {
                DIV_DADOS.Visible = true;
                div_Importador.Visible = false;
                btnXML.Visible = false;
                return true;
            }
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string cProd = (e.Row.FindControl("cProd") as Label).Text;
                string xProd = (e.Row.FindControl("xProd") as Label).Text;
                string nItem = (e.Row.FindControl("nOrdem") as Label).Text;
                string[] partes = xProd.Split(new string[] { " - " }, StringSplitOptions.None);
            }

            if (hddidStatus.Value == "1")
                if (hddidTipoObjeto.Value != "13" && hddidTipoObjeto.Value != "10")
                {
                    GRID.EsconderColunas(e, 13, 14, 15);
                    //GRID.EsconderColunas(e, 18);
                }
                else
                {
                    GRID.EsconderColunas(e, 11, 12, 13, 14, 15);
                }
            else
            {
                GRID.EsconderColunas(e, 11, 12, 13, 14, 15);
                //GRID.EsconderColunas(e, 18);
            }
        }

        protected void ddlPedidos_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow row in dtgvConsulta.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    DropDownList ddlItem = (row.FindControl("ddlItemAdd") as DropDownList);
                    DropDownList ddlPedidos = (row.FindControl("ddlPedidos") as DropDownList);
                    string cProd = (row.FindControl("cProd") as Label).Text;
                    string xProd = (row.FindControl("xProd") as Label).Text;
                    string[] partes = xProd.Split(new string[] { " - " }, StringSplitOptions.None);

                    if (ddlPedidos.SelectedValue != "0")
                    {
                        if (ddlItem.SelectedValue == "" || ddlItem.SelectedValue == "0")
                        {
                            FUNCOES.Popula_Combo(ddlItem, "sp_Select 'ProdutosPedidos_Flow', @sPesquisa=" + "'" + ddlPedidos.SelectedValue + "'", "sCodigo", "sCodProd", false, "Selecione o Produto", "0");

                            try
                            {
                                ddlItem.SelectedValue = cProd;
                            }
                            catch
                            {

                            }
                            try
                            {
                                ddlItem.SelectedValue = partes[0];
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
        }

        protected void ddlItemAdd_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow row in dtgvConsulta.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    TableCell itemCell = row.Cells[3];
                    DropDownList ddlItem = (row.FindControl("ddlItemAdd") as DropDownList);
                    DropDownList ddlItemFornecedor = (row.FindControl("ddlItemFornecedor") as DropDownList);
                    DropDownList ddlPedidos = (row.FindControl("ddlPedidos") as DropDownList);
                    if (ddlPedidos.SelectedValue != "0")
                    {
                        if (ddlItem.SelectedValue == "0" && ddlItemFornecedor.SelectedValue == "0")
                        {
                            itemCell.CssClass = "Amarelo";
                        }
                        else
                        {
                            itemCell.CssClass = "";
                        }
                    }
                }
            }
        }

        void ViewerPDF(string idArquivo)
        {
            if (string.IsNullOrEmpty(idArquivo))
                return;

            // Pega o PDF do banco
            byte[] bObjArquivo = null;
            string sNomeArquivo = "";

            Dictionary<string, string> vParametrosItem = new Dictionary<string, string>
            {
                {"@sFuncao", "CONSULTAR_DETALHE"},
                {"@idArquivo", idArquivo}
            };

            DataTable dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

            if (dtArquivo.Rows.Count > 0)
            {
                DataRow item = dtArquivo.Rows[0];
                sNomeArquivo = item["sNomeArquivo"].ToString();
                bObjArquivo = (byte[])item["vbArquivo"];
            }
            if (bObjArquivo != null)
            {
                string base64 = Convert.ToBase64String(bObjArquivo);
                pdfViewer.Attributes["src"] = $"data:application/pdf;base64,{base64}";
            }
        }
        #endregion

        #region | Imposto
        protected void dtgvConsulta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                decimal vBC_ICMSTOTAL = 0;
                decimal vICMS_TOTAL = 0;
                decimal vIPI_TOTAL = 0;
                decimal vPIS_TOTAL = 0;
                decimal vCOFINS_TOTAL = 0;
                decimal vII_TOTAL = 0;
                string filePath = Server.MapPath("~/Download/" + hddsNomeArquivo.Value);
                XDocument xdoc = XDocument.Load(filePath);

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSTot").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSTot").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault().Value = vBC_ICMSTOTAL.ToString().Replace(",", ".");
                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSTot").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault() != null)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSTot").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault().Value = vICMS_TOTAL.ToString().Replace(",", ".");
                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSTot").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault() != null)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSTot").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault().Value = vII_TOTAL.ToString().Replace(",", ".");
                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSTot").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault() != null)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSTot").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault().Value = vIPI_TOTAL.ToString().Replace(",", ".");
                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSTot").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault() != null)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSTot").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault().Value = vPIS_TOTAL.ToString().Replace(",", ".");
                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSTot").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault() != null)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}total").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMSTot").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault().Value = vCOFINS_TOTAL.ToString().Replace(",", ".");

                xdoc.Save(filePath);

                foreach (var linha in Base_Importador_Itens)
                {
                    if (linha.nItem == nItem)
                    {
                        DataSet dsSalvarItens;
                        Dictionary<String, String> vParametrosItens = new Dictionary<string, string>();
                        vParametrosItens.Add("@sfuncao", "SALVAR_ITENS");
                        vParametrosItens.Add("@idItens", idItens.ToString());
                        vParametrosItens.Add("@idImportador", linha.idImportador.ToString());
                        vParametrosItens.Add("@nItem", linha.nItem.ToString());
                        vParametrosItens.Add("@cProd", linha.cProd);
                        vParametrosItens.Add("@cEAN", linha.cEAN);
                        vParametrosItens.Add("@xProd", linha.xProd);
                        vParametrosItens.Add("@NCM", linha.NCM);
                        vParametrosItens.Add("@CFOP", linha.CFOP);
                        vParametrosItens.Add("@uCom", linha.uCom);
                        vParametrosItens.Add("@qCom", linha.qCom.Replace(",", "."));
                        vParametrosItens.Add("@vUnCom", linha.vUnCom.Replace(",", "."));
                        vParametrosItens.Add("@vProd", linha.vProd.Replace(",", "."));
                        vParametrosItens.Add("@cEANTrib", linha.cEANTrib);
                        vParametrosItens.Add("@uTrib", linha.uTrib);
                        vParametrosItens.Add("@qTrib", linha.qTrib.Replace(",", "."));
                        vParametrosItens.Add("@vUnTrib", linha.vUnTrib.Replace(",", "."));
                        vParametrosItens.Add("@indTot", linha.indTot);
                        vParametrosItens.Add("@idProduto", linha.idProduto.ToString());
                        vParametrosItens.Add("@idPedido", linha.idPedido.ToString());
                        if (linha.xPed != null)
                            vParametrosItens.Add("@xPed", linha.xPed);
                        else
                            vParametrosItens.Add("@xPed", "");

                        dsSalvarItens = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ImportadorNfe", vParametrosItens);
                    }
                }

                XDocument novo = XDocument.Load(filePath);
                string Corpo = "";
                if (novo.Descendants("{http://www.portalfiscal.inf.br/nfe}nfeProc").FirstOrDefault() != null)
                    Corpo = novo.Descendants("{http://www.portalfiscal.inf.br/nfe}nfeProc").FirstOrDefault().ToString();
                if (novo.Descendants("{http://www.portalfiscal.inf.br/nfe}NFe").FirstOrDefault() != null)
                    Corpo = novo.Descendants("{http://www.portalfiscal.inf.br/nfe}NFe").FirstOrDefault().ToString();
                string chave = "";
                var infNFeElement = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}infNFe").FirstOrDefault();
                if (infNFeElement != null)
                {
                    chave = infNFeElement.Attribute("Id")?.Value?.Substring(3);
                }

                Byte[] lObjArquivoAlterar = null;
                TT.FrameWork.Arquivo objArquivoAlterar = new TT.FrameWork.Arquivo();
                try
                {
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        lObjArquivoAlterar = objArquivoAlterar.TransformaArquivoEmArrayBytes(filePath, fileStream);
                    }

                    if (lObjArquivoAlterar != null && lObjArquivoAlterar.Length > 0)
                    {
                        SqlDataAdapter da = new SqlDataAdapter("sp_Manipula_tbl_Flow_Arquivos", TT.FrameWork.BD.StringDeConexao);

                        DataSet tabela = new DataSet();
                        da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "ALTERARXML";
                        da.SelectCommand.Parameters.Add("@vbArquivo", SqlDbType.VarBinary).Value = lObjArquivoAlterar;
                        da.SelectCommand.Parameters.Add("@idArquivo", SqlDbType.Int).Value = Convert.ToInt32(hddidArquivo.Value);

                        try
                        {
                            da.Fill(tabela);
                        }
                        catch (Exception ex)
                        {
                            //MensagemPaginaUnitizados.MostraMensagem_Erro(ex.Message);
                        }
                    }
                    else
                    {
                        //MensagemPaginaUnitizados.MostraMensagem_Erro("Erro: O arquivo não foi convertido corretamente em bytes.");
                    }
                }
                catch (Exception ex)
                {
                    //MensagemPaginaUnitizados.MostraMensagem_Erro(string.Format("Erro ao ler o arquivo: {0}", ex.Message));
                }

                DataSet dsPesquisaAlterar;
                Dictionary<String, String> vParametrosAlterar = new Dictionary<string, string>();
                vParametrosAlterar.Add("@sFuncao", "ALTERARXML");
                vParametrosAlterar.Add("@sChaveNFe", chave);
                vParametrosAlterar.Add("@sXML", Corpo);
                dsPesquisaAlterar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametrosAlterar);

                MensagemPagina.MostraMensagem_Sucesso("Impostos salvo com sucesso!");
                Pesquisar(hddidImportador.Value, false);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModal", "$('#modalImposto').modal('hide');", true);
            }
            catch (Exception ex)
            {
                //MensagemPaginaUnitizados.MostraMensagem_Erro(ex.Message);
            }
            //updModal.Update();
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

            int[] pesos = { 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma = 0;

            // Multiplicar cada dígito pelos pesos correspondentes
            for (int i = 0; i < 43; i++)
            {
                soma += (chave[i] - '0') * pesos[i % pesos.Length];
            }

            // Calcular o módulo 11
            int resto = soma % 11;
            return (resto == 0 || resto == 1) ? 0 : (11 - resto);
        }
        #endregion
    }
}