using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using static TT.FrameWork.BD;
using TT_Flow.App.Controles;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Web.UI.WebControls;
using TT.FrameWork;
using System.Text.RegularExpressions;
using static Permissao.Administracao;
using System.Net.Http;
using System.Text;
using TT_Flow.App.Paginas.Requisicao;
//using static iTextSharp.text.pdf.AcroFields;
//using NPOI.SS.Formula.Functions;
using static TT.FrameWork.Funcoes_NFe;
using System.Diagnostics;

namespace TT_Flow.App.Paginas.Adm.Faturamento
{
    public partial class ImportadorNFE_Detalhe : System.Web.UI.Page
    {
        #region | Classes
        string sTituloPagina = "NFe Importação";
        string sProcedure = "sp_Manipula_tbl_Flow_Arquivos";
        string IncluirArquivo = "S";
        public static int nItem = 0;
        public static int idItens = 0;
        public static string idEmpresa = "0";
        public static string Alterar_nNF = "";
        public static string sGerarNFe = "";

        int idOrigem_Emissor = 3;

        int nColuna_idXML = 0;
        int nColuna_idObjeto = 1;
        int nColuna_sTipo = 3;
        int nColuna_sChave = 6;
        int nColuna_Status = 7;
        int nColuna_nNumeroCartaCorrecao = 10;
        int nColuna_Cancelamento = 11;
        int nColuna_sChaveNFe = 12;

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
        public List<TT_Flow.FrameWork.cls_WMS_Produtos> bs_Itens_Pedido_Compra
        {
            get
            {
                if (ViewState["bs_Itens_Pedido_Compra"] == null)
                {
                    ViewState["bs_Itens_Pedido_Compra"] = new List<FrameWork.cls_WMS_Produtos>();
                }
                return (List<FrameWork.cls_WMS_Produtos>)ViewState["bs_Itens_Pedido_Compra"];
            }

            set
            {
                ViewState["bs_Itens_Pedido_Compra"] = value;
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
                              

                if (requestTarget == "funcao_Excluir")
                    ExcluirImportador();
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
                DIV15.Visible = false;
                //btnGerar.Visible = false;
                aba_NFe.Visible = false;
                btnExcluir.Visible = false;

                if (ValidarControles(Novo))
                {
                    if (idImportador != "0")
                    {
                        DataSet dsPesquisa;
                        Dictionary<String, String> vParametros = new Dictionary<string, string>();
                        vParametros.Add("@sfuncao", "CONSULTAR_DETALHE");
                        vParametros.Add("@idImportador", idImportador);
                        dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ImportadorNfe", vParametros);

                        if (BD.ValidarDataSet(dsPesquisa, out sErro))
                        {
                            FUNCOES.Popula_Combo(ddlidParceiroDest, "sp_Select 'Flow_Clientes'", "idCliente", "sCPNJ_RazaoSocial", false, "Selecione o Destinatario", "0");
                            FUNCOES.Popula_Combo(ddlidParceiroEmit, "sp_Select 'Flow_Clientes'", "idCliente", "sCPNJ_RazaoSocial", false, "Selecione o Destinatario", "0");
                            FUNCOES.Popula_Combo(ddlidArmazenamento, "sp_Select 'Flow_WMS_Produtos_Local_Armazenamento'", "idLocalArmazenamento", "sDscLocalArmazenamento", false, "Selecione o local de armazenamento", "0");
                            FUNCOES.Popula_Combo(ddlidTranportadora, "sp_Select 'Flow_Parceiro_Transportadora'", "idParceiro", "sRazaoSocial", false, "Selecione a Transportadora", "20");
                            //emit
                            hddidImportador.Value = RETORNO.DATASET(dsPesquisa, 0, "idImportador");
                            hddidArquivo.Value = RETORNO.DATASET(dsPesquisa, 0, "idArquivo");
                            ddlidParceiroEmit.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idParceiro_Emit");
                            idEmpresa = RETORNO.DATASET(dsPesquisa, 0, "idEmpresa");
                            ddlidEnderecoEmit.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idEndereco_Emit");
                            hddsNomeArquivo.Value = RETORNO.DATASET(dsPesquisa, 0, "sNomeArquivo");

                            string filePath  = Server.MapPath("~/Download/" + hddsNomeArquivo.Value);
                            if (!File.Exists(filePath))
                            {

                                byte[] bObjArquivo = (byte[])dsPesquisa.Tables[0].Rows[0]["vbArquivo"];
                                FileStream lObjFile = new Arquivo().TransformarArrayBytesEmArquivo(bObjArquivo, filePath);
                                lObjFile.Close();
                                lObjFile.Dispose();

                            }


                            hddsCaminho_UniNFe.Value = RETORNO.DATASET(dsPesquisa, 0, "sCaminho_UniNFe");

                            hddidStatus.Value = "0";
                            string sDscStatus = RETORNO.DATASET(dsPesquisa, "sDscStatus");
                            if ((sDscStatus  != "Pendente") && (sDscStatus != "Cancelada"))
                            {
                                hddidStatus.Value = "1";
                            }

                            if (sDscStatus == "Pendente")
                            {
                                btnExcluir.Visible = true;
                            }

                            //hddidStatus.Value = RETORNO.DATASET(dsPesquisa, 0, "idStatus");
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
                            ddlidParceiroDest.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idParceiro_Dest");
                            ddlidEnderecoDest.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idEndereco_Dest");

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
                            if (RETORNO.DATASET(dsPesquisa, 0, "idTransportadora") != "")
                                ddlidTranportadora.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idTransportadora");
                            //else
                            //div_Transportadora.Visible = false;

                            CarregarItens(idImportador);

                            
                            if (RETORNO.DATASET(dsPesquisa, 0, "idParceiro_Emit") != "0")
                                ddlidParceiroEmit.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idParceiro_Emit");

                            if (RETORNO.DATASET(dsPesquisa, 0, "idEndereco_Emit") != "0")
                            {
                                FUNCOES.Popula_Combo(ddlidEnderecoEmit, string.Format("sp_Select 'Flow_Clientes_Endereco', {0}", ddlidParceiroEmit.SelectedValue), "idEndereco", "sEnderecoClt", false, "Selecione um Endereço", "0");
                                ddlidEnderecoEmit.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idEndereco_Emit");
                            }
                            else
                            {
                                // Emit
                                if (txtemitCNPJ.Text != "")
                                {
                                    DataSet dsPesquisaParceiroEmit;
                                    Dictionary<String, String> vParametrosParceiroEmit = new Dictionary<string, string>();
                                    vParametrosParceiroEmit.Add("@sfuncao", "ConsultarParceiro");
                                    vParametrosParceiroEmit.Add("@sCPF_CNPJ", txtemitCNPJ.Text);
                                    dsPesquisaParceiroEmit = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParametrosParceiroEmit);
                                    ddlidParceiroEmit.SelectedValue = RETORNO.DATASET(dsPesquisaParceiroEmit, 0, "idCliente");
                                    idEmpresa = RETORNO.DATASET(dsPesquisaParceiroEmit, 0, "idEmpresa");
                                }
                                if (ddlidParceiroEmit.SelectedValue != "0")
                                {
                                    if (hddsEnderecoEmit.Value == "S")
                                        Div10.Visible = true;
                                    else
                                        Div5.Visible = false;
                                    FUNCOES.Popula_Combo(ddlidEnderecoEmit, string.Format("sp_Select 'Flow_Clientes_Endereco', {0}", ddlidParceiroEmit.SelectedValue), "idEndereco", "sEnderecoClt", false, "Selecione um Endereço", "0");
                                    DataSet dsPesquisaEnderecoEmit;
                                    Dictionary<String, String> vParametrosEnderecoEmit = new Dictionary<string, string>();
                                    vParametrosEnderecoEmit.Add("@sfuncao", "ConsultarEndereco");
                                    vParametrosEnderecoEmit.Add("@sLogradouro", txtemitxLgr.Text);
                                    vParametrosEnderecoEmit.Add("@sNumero", txtemitnro.Text);
                                    vParametrosEnderecoEmit.Add("@idCliente", ddlidParceiroEmit.SelectedValue);
                                    dsPesquisaEnderecoEmit = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParametrosEnderecoEmit);
                                    ddlidEnderecoEmit.SelectedValue = RETORNO.DATASET(dsPesquisaEnderecoEmit, 0, "idEndereco");
                                }
                            }

                            if (RETORNO.DATASET(dsPesquisa, 0, "idParceiro_Dest") != "0")
                                ddlidParceiroDest.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idParceiro_Dest");

                            if (RETORNO.DATASET(dsPesquisa, 0, "idEndereco_Dest") != "0")
                            {
                                FUNCOES.Popula_Combo(ddlidEnderecoDest, string.Format("sp_Select 'Flow_Clientes_Endereco', {0}", ddlidParceiroDest.SelectedValue), "idEndereco", "sEnderecoClt", false, "Selecione um Endereço", "0");
                                ddlidEnderecoDest.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idEndereco_Dest");
                            }
                            else
                            {
                                // Dest;
                                if (ddlidParceiroDest.SelectedValue != "0")
                                {
                                    if (hddsEnderecoDest.Value == "S")
                                        Div5.Visible = true;
                                    else
                                        Div5.Visible = false;
                                    FUNCOES.Popula_Combo(ddlidEnderecoDest, string.Format("sp_Select 'Flow_Clientes_Endereco', {0}", ddlidParceiroDest.SelectedValue), "idEndereco", "sEnderecoClt", false, "Selecione um Endereço", "0");
                                    DataSet dsPesquisaEndereco;
                                    Dictionary<String, String> vParametrosEndereco = new Dictionary<string, string>();
                                    vParametrosEndereco.Add("@sfuncao", "ConsultarEndereco");
                                    vParametrosEndereco.Add("@sLogradouro", txtdestxLgr.Text);
                                    vParametrosEndereco.Add("@sNumero", txtdestnro.Text);
                                    vParametrosEndereco.Add("@idCliente", ddlidParceiroDest.SelectedValue);
                                    dsPesquisaEndereco = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParametrosEndereco);
                                    ddlidEnderecoDest.SelectedValue = RETORNO.DATASET(dsPesquisaEndereco, 0, "idEndereco");
                                }
                                else
                                {
                                    Div5.Visible = false;
                                }
                            }

                            if (hddidStatus.Value == "1")
                            {
                                div_Ordem.Visible = false;
                                btnSalvar.Visible = false;
                                cmdGerarNFPiloto.Visible = false;
                                ddlidParceiroEmit.Attributes.Add("disabled", "disabled");
                                ddlidEnderecoEmit.Attributes.Add("disabled", "disabled");
                                ddlidParceiroDest.Attributes.Add("disabled", "disabled");
                                ddlidEnderecoDest.Attributes.Add("disabled", "disabled");
                                ddlModoFrete.Attributes.Add("disabled", "disabled");
                                ddlidTranportadora.Attributes.Add("disabled", "disabled");
                                foreach (GridViewRow Row in dtgvConsulta.Rows)
                                {
                                    DropDownList ddlPedidos = (Row.FindControl("ddlPedidos") as DropDownList);

                                    ddlPedidos.Attributes.Add("disabled", "disabled");
                                }
                            }
                            else
                            {
                                div_Ordem.Visible = true;
                                ddlidParceiroEmit.Attributes.Remove("disabled"); ;
                                ddlidEnderecoEmit.Attributes.Remove("disabled"); ;
                                ddlidParceiroDest.Attributes.Remove("disabled"); ;
                                ddlidEnderecoDest.Attributes.Remove("disabled"); ;
                                ddlModoFrete.Attributes.Remove("disabled"); ;
                                ddlidTranportadora.Attributes.Remove("disabled"); ;
                                cmdGerarNFPiloto.Visible = true;
                                btnSalvar.Visible = true;


                            }
                            if (dsPesquisa.Tables[2].Rows.Count> 0)
                            {
                                aba_NFe.Visible = true;
                                gvDocumentosFiscais.DataSource = dsPesquisa.Tables[2];
                                gvDocumentosFiscais.DataBind();

                            }
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
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        void CarregarItens(string idImportador)
        {
            cmdGerarNFPiloto.Visible = false;
            string sErro = "";
            string idParceiro_Destino = ddlidParceiroDest.SelectedValue;

            if (idParceiro_Destino != "0")
            {
                FUNCOES.Popula_Combo(ddlPedidos_Geral, "sp_Select 'FLOW-PEDIDO-COMEX_IMPORTACAO', @idPesquisa=" + ddlidParceiroDest.SelectedValue, "idPedido", "sReferencia", false, "Selecione o Pedido", "0");

                Base_Importador_Itens.Clear();
                DataSet dsPesquisa;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sfuncao", "CONSULTAR_DETALHE_ITENS");
                vParametros.Add("@idImportador", idImportador);
                vParametros.Add("@idParceiro_Dest", idParceiro_Destino);
                dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ImportadorNfe", vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out sErro))
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

                        objItem.sCodigo_Pesquisa = row["sCodigo_Pesquisa"].ToString();
                        objItem.idProduto_Pesquisa = row["idProduto_Pesquisa"].ToString();
                        objItem.sDscProduto_Pesquisa = row["sDscProduto_Pesquisa"].ToString();
                        objItem.sPesquisa = row["sPesquisa"].ToString();
                        objItem.sReferencia = row["sReferenciaPedido"].ToString();


                        Base_Importador_Itens.Add(objItem);
                    }
                dtgvConsulta.DataSource = Base_Importador_Itens;
                dtgvConsulta.DataBind();
                //aba_Itens.Visible = true;
                cmdGerarNFPiloto.Visible = true;
            

            }
            else
            {
                MensagemPagina1.MostraMensagem_Aviso("Para exibir os produtos, primeiro selecione o Destinatário!");
            }


        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            Salvar("Consultar");

        }

        void Salvar(string sFuncao)
        {

            try
            {
                string filePath = Server.MapPath("~/Download/" + hddsNomeArquivo.Value);



                XDocument xdoc = XDocument.Load(filePath);

                string cnpj = "";
                //--emit
                if (txtemitCNPJ.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value)
                {
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault().Value = txtemitCNPJ.Text;
                    cnpj = txtemitCNPJ.Text;
                }
                else
                {
                    cnpj = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault().Value;
                }

                if (txtemitxNome.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault().Value = txtemitxNome.Text;

                if (txtemitxFant.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xFant").FirstOrDefault()?.Value)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xFant").FirstOrDefault().Value = txtemitxFant.Text;

                if (txtemitxLgr.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault()?.Value)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault().Value = txtemitxLgr.Text;

                if (txtemitnro.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault()?.Value)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault().Value = txtemitnro.Text;

                if (txtemitxBairro.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault()?.Value)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault().Value = txtemitxBairro.Text;

                if (txtemitcMun.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}cMun").FirstOrDefault()?.Value)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}cMun").FirstOrDefault().Value = txtemitcMun.Text;

                if (txtemitxMun.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault()?.Value)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault().Value = txtemitxMun.Text;

                if (txtemitUF.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault().Value = txtemitUF.Text;

                if (txtemitCEP.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault()?.Value)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault().Value = txtemitCEP.Text;

                if (txtemitcPais.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}cPais").FirstOrDefault()?.Value)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}cPais").FirstOrDefault().Value = txtemitcPais.Text;

                if (txtemitxPais.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xPais").FirstOrDefault()?.Value)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xPais").FirstOrDefault().Value = txtemitxPais.Text;

                if (txtemitfone.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}fone").FirstOrDefault()?.Value)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}fone").FirstOrDefault().Value = txtemitfone.Text;

                if (txtemitIE.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault()?.Value)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault().Value = txtemitIE.Text;

                if (txtemitCRT.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CRT").FirstOrDefault()?.Value)
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CRT").FirstOrDefault().Value = txtemitCRT.Text;
                //--
                //--dest
                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null || xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CPF").FirstOrDefault() != null)
                {
                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault().Value = txtdestCNPJ.Text;
                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CPF").FirstOrDefault() != null)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CPF").FirstOrDefault().Value = txtdestCNPJ.Text;
                }

                DataSet dsPesquisaParceiro;
                Dictionary<String, String> vParametrosParceiro = new Dictionary<string, string>();
                vParametrosParceiro.Add("@sfuncao", "CONSULTAR_tpAmb");
                dsPesquisaParceiro = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametrosParceiro);
                string tpAmb = RETORNO.DATASET(dsPesquisaParceiro, 0, "tpAmb");

                if (tpAmb == "1")
                {
                    if (txtdestxNome.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault().Value = txtdestxNome.Text;

                    if ("1" != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpAmb").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpAmb").FirstOrDefault().Value = "1";
                }
                else
                {
                    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault().Value = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL";

                    if ("2" != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpAmb").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}tpAmb").FirstOrDefault().Value = "2";
                }

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault() != null)
                    if (txtdestxLgr.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault().Value = txtdestxLgr.Text;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault() != null)
                    if (txtdestnro.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault().Value = txtdestnro.Text;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xCpl").FirstOrDefault() != null)
                    if (txtdestxCpl.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xCpl").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xCpl").FirstOrDefault().Value = txtdestxCpl.Text;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault() != null)
                    if (txtdestxBairro.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xBairro").FirstOrDefault().Value = txtdestxBairro.Text;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}cMun").FirstOrDefault() != null)
                    if (txtdestcMun.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}cMun").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}cMun").FirstOrDefault().Value = txtdestcMun.Text;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault() != null)
                    if (txtdestxMun.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault().Value = txtdestxMun.Text;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault() != null)
                    if (txtdestUF.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault().Value = txtdestUF.Text;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}cPais").FirstOrDefault() != null)
                    if (txtdestcPais.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}cPais").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}cPais").FirstOrDefault().Value = txtdestcPais.Text;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNxPaisPJ").FirstOrDefault() != null)
                    if (txtdestxPais.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNxPaisPJ").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNxPaisPJ").FirstOrDefault().Value = txtdestxPais.Text;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}indIEDest").FirstOrDefault() != null)
                    if (txtdestindIEDest.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}indIEDest").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}indIEDest").FirstOrDefault().Value = txtdestindIEDest.Text;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault() != null)
                    if (txtIEdest.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault().Value = txtIEdest.Text;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}modFrete").FirstOrDefault() != null)
                    if (ddlModoFrete.SelectedValue != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}modFrete").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}modFrete").FirstOrDefault().Value = ddlModoFrete.SelectedValue;

                XNamespace ns = "http://www.portalfiscal.inf.br/nfe";
                var transp = xdoc.Descendants(ns + "transp").FirstOrDefault();
                var transporta = transp.Element(ns + "transporta");
                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                {
                    if (txtCNPJTrans.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault().Value = txtCNPJTrans.Text;
                }
                else
                {
                    if (txtCNPJTrans.Text != "")
                    {
                        if (transporta == null)
                        {
                            transporta = new XElement(ns + "transporta",
                                            new XElement(ns + "CNPJ", txtCNPJTrans.Text));

                            // Tenta inserir antes do <vol>
                            var vol = transp.Element(ns + "vol");
                            if (vol != null)
                                vol.AddBeforeSelf(transporta);
                            else
                                transp.Add(transporta); // Se não tiver <vol>, adiciona no fim
                        }
                        else
                        {
                            var cnpjElement = transporta.Element(ns + "CNPJ");
                            if (cnpjElement != null)
                            {
                                if (txtCNPJTrans.Text != cnpjElement.Value)
                                    cnpjElement.Value = txtCNPJTrans.Text;
                            }
                            else if (!string.IsNullOrWhiteSpace(txtCNPJTrans.Text))
                            {
                                transporta.Add(new XElement(ns + "CNPJ", txtCNPJTrans.Text));
                            }
                        }
                    }
                }

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                {
                    if (txtNomeTrans.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault().Value = txtNomeTrans.Text;
                }
                else
                {
                    if (txtNomeTrans.Text != "")
                    {
                        if (transporta == null)
                        {
                            transporta = new XElement(ns + "transporta",
                                            new XElement(ns + "xNome", txtNomeTrans.Text));

                            // Tenta inserir antes do <vol>
                            var vol = transp.Element(ns + "vol");
                            if (vol != null)
                                vol.AddBeforeSelf(transporta);
                            else
                                transp.Add(transporta); // Se não tiver <vol>, adiciona no fim
                        }
                        else
                        {
                            var xNomeElement = transporta.Element(ns + "xNome");
                            if (xNomeElement != null)
                            {
                                if (txtNomeTrans.Text != xNomeElement.Value)
                                    xNomeElement.Value = txtNomeTrans.Text;
                            }
                            else if (!string.IsNullOrWhiteSpace(txtNomeTrans.Text))
                            {
                                transporta.Add(new XElement(ns + "xNome", txtNomeTrans.Text));
                            }
                        }
                    }
                }

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault() != null)
                {
                    if (txtIETrans.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}IE").FirstOrDefault().Value = txtIETrans.Text;
                }
                else
                {
                    if (txtIETrans.Text != "")
                    {
                        if (transporta == null)
                        {
                            transporta = new XElement(ns + "transporta",
                                            new XElement(ns + "IE", txtIETrans.Text));

                            // Tenta inserir antes do <vol>
                            var vol = transp.Element(ns + "vol");
                            if (vol != null)
                                vol.AddBeforeSelf(transporta);
                            else
                                transp.Add(transporta); // Se não tiver <vol>, adiciona no fim
                        }
                        else
                        {
                            var xNomeElement = transporta.Element(ns + "IE");
                            if (xNomeElement != null)
                            {
                                if (txtIETrans.Text != xNomeElement.Value)
                                    xNomeElement.Value = txtIETrans.Text;
                            }
                            else if (!string.IsNullOrWhiteSpace(txtIETrans.Text))
                            {
                                transporta.Add(new XElement(ns + "IE", txtIETrans.Text));
                            }
                        }
                    }
                }

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xEnder").FirstOrDefault() != null)
                {
                    if (txtEnderecoTrans.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xEnder").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xEnder").FirstOrDefault().Value = txtEnderecoTrans.Text;
                }
                else
                {
                    if (txtEnderecoTrans.Text != "")
                    {
                        if (transporta == null)
                        {
                            transporta = new XElement(ns + "transporta",
                                            new XElement(ns + "xEnder", txtEnderecoTrans.Text));

                            // Tenta inserir antes do <vol>
                            var vol = transp.Element(ns + "vol");
                            if (vol != null)
                                vol.AddBeforeSelf(transporta);
                            else
                                transp.Add(transporta); // Se não tiver <vol>, adiciona no fim
                        }
                        else
                        {
                            var xNomeElement = transporta.Element(ns + "xEnder");
                            if (xNomeElement != null)
                            {
                                if (txtEnderecoTrans.Text != xNomeElement.Value)
                                    xNomeElement.Value = txtEnderecoTrans.Text;
                            }
                            else if (!string.IsNullOrWhiteSpace(txtEnderecoTrans.Text))
                            {
                                transporta.Add(new XElement(ns + "xEnder", txtEnderecoTrans.Text));
                            }
                        }
                    }
                }

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault() != null)
                {
                    if (txtMunicipiotrans.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}xMun").FirstOrDefault().Value = txtMunicipiotrans.Text;
                }
                else
                {
                    if (txtMunicipiotrans.Text != "")
                    {
                        if (transporta == null)
                        {
                            transporta = new XElement(ns + "transporta",
                                            new XElement(ns + "xMun", txtMunicipiotrans.Text));

                            // Tenta inserir antes do <vol>
                            var vol = transp.Element(ns + "vol");
                            if (vol != null)
                                vol.AddBeforeSelf(transporta);
                            else
                                transp.Add(transporta); // Se não tiver <vol>, adiciona no fim
                        }
                        else
                        {
                            var xNomeElement = transporta.Element(ns + "xMun");
                            if (xNomeElement != null)
                            {
                                if (txtMunicipiotrans.Text != xNomeElement.Value)
                                    xNomeElement.Value = txtMunicipiotrans.Text;
                            }
                            else if (!string.IsNullOrWhiteSpace(txtMunicipiotrans.Text))
                            {
                                transporta.Add(new XElement(ns + "xMun", txtMunicipiotrans.Text));
                            }
                        }
                    }
                }

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault() != null)
                {
                    if (txtUFTrans.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault().Value = txtUFTrans.Text;
                }
                else
                {
                    if (txtUFTrans.Text != "")
                    {
                        if (transporta == null)
                        {
                            transporta = new XElement(ns + "transporta",
                                            new XElement(ns + "UF", txtUFTrans.Text));

                            // Tenta inserir antes do <vol>
                            var vol = transp.Element(ns + "vol");
                            if (vol != null)
                                vol.AddBeforeSelf(transporta);
                            else
                                transp.Add(transporta); // Se não tiver <vol>, adiciona no fim
                        }
                        else
                        {
                            var xNomeElement = transporta.Element(ns + "UF");
                            if (xNomeElement != null)
                            {
                                if (txtUFTrans.Text != xNomeElement.Value)
                                    xNomeElement.Value = txtUFTrans.Text;
                            }
                            else if (!string.IsNullOrWhiteSpace(txtUFTrans.Text))
                            {
                                transporta.Add(new XElement(ns + "UF", txtUFTrans.Text));
                            }
                        }
                    }
                }

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}qVol").FirstOrDefault() != null)
                {
                    if (txtVolumes.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}qVol").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}qVol").FirstOrDefault().Value = txtVolumes.Text;
                }
                else
                {
                    if (txtVolumes.Text != "0")
                    {
                        // Procura um <vol> já existente dentro de <transp>
                        var volElement = transp.Element(ns + "vol");

                        if (volElement == null)
                        {
                            // Não existe ainda: cria e adiciona no <transp>
                            volElement = new XElement(ns + "vol",
                                new XElement(ns + "qVol", txtVolumes.Text)
                            );
                            transp.Add(volElement);
                        }
                        else
                        {
                            // Já existe: atualiza ou cria <qVol>
                            var qVolElement = volElement.Element(ns + "qVol");
                            if (qVolElement == null)
                            {
                                volElement.Add(new XElement(ns + "qVol", txtVolumes.Text));
                            }
                            else
                            {
                                qVolElement.Value = txtVolumes.Text;
                            }
                        }
                    }
                }

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}esp").FirstOrDefault() != null)
                {
                    if (txtEspecie.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}esp").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}esp").FirstOrDefault().Value = txtEspecie.Text;
                }
                else
                {
                    if (txtEspecie.Text != "")
                    {
                        var volElement = transp.Element(ns + "vol");

                        if (volElement == null)
                        {
                            // Não existe ainda: cria e adiciona no <transp>
                            volElement = new XElement(ns + "vol",
                                new XElement(ns + "esp", txtEspecie.Text)
                            );
                            transp.Add(volElement);
                        }
                        else
                        {
                            // Já existe: atualiza ou cria <qVol>
                            var qVolElement = volElement.Element(ns + "esp");
                            if (qVolElement == null)
                            {
                                volElement.Add(new XElement(ns + "esp", txtEspecie.Text));
                            }
                            else
                            {
                                qVolElement.Value = txtEspecie.Text;
                            }
                        }
                    }
                }

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoL").FirstOrDefault() != null)
                {
                    if (txtPesoLiquido.Text.Replace(",", ".") != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoL").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoL").FirstOrDefault().Value = txtPesoLiquido.Text.Replace(",", ".");
                }
                else
                {
                    if (txtPesoLiquido.Text != "0,000")
                    {
                        var volElement = transp.Element(ns + "vol");

                        if (volElement == null)
                        {
                            // Não existe ainda: cria e adiciona no <transp>
                            volElement = new XElement(ns + "vol",
                                new XElement(ns + "pesoL", txtPesoLiquido.Text.Replace(",", "."))
                            );
                            transp.Add(volElement);
                        }
                        else
                        {
                            // Já existe: atualiza ou cria <qVol>
                            var qVolElement = volElement.Element(ns + "pesoL");
                            if (qVolElement == null)
                            {
                                volElement.Add(new XElement(ns + "pesoL", txtPesoLiquido.Text.Replace(",", ".")));
                            }
                            else
                            {
                                qVolElement.Value = txtPesoLiquido.Text.Replace(",", ".");
                            }
                        }
                    }
                }

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoB").FirstOrDefault() != null)
                {
                    if (txtPesoBruto.Text.Replace(",", ".") != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoB").FirstOrDefault()?.Value)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}transp").Descendants("{http://www.portalfiscal.inf.br/nfe}pesoB").FirstOrDefault().Value = txtPesoBruto.Text.Replace(",", ".");
                }
                else
                {
                    if (txtPesoBruto.Text != "0,000")
                    {
                        var volElement = transp.Element(ns + "vol");

                        if (volElement == null)
                        {
                            // Não existe ainda: cria e adiciona no <transp>
                            volElement = new XElement(ns + "vol",
                                new XElement(ns + "pesoB", txtPesoBruto.Text.Replace(",", "."))
                            );
                            transp.Add(volElement);
                        }
                        else
                        {
                            // Já existe: atualiza ou cria <qVol>
                            var qVolElement = volElement.Element(ns + "pesoB");
                            if (qVolElement == null)
                            {
                                volElement.Add(new XElement(ns + "pesoB", txtPesoBruto.Text.Replace(",", ".")));
                            }
                            else
                            {
                                qVolElement.Value = txtPesoBruto.Text.Replace(",", ".");
                            }
                        }
                    }
                }
                    string chave = "";
                    var infNFeElement = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}infNFe").FirstOrDefault();
                    if (infNFeElement != null)
                    {
                        chave = infNFeElement.Attribute("Id")?.Value?.Substring(3);
                    }
                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}cDV").FirstOrDefault() != null)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}cDV").FirstOrDefault().Value = chave[chave.Length - 1].ToString();
                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}dhEmi").FirstOrDefault() != null)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}dhEmi").FirstOrDefault().Value = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                    if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}dhSaiEnt").FirstOrDefault() != null)
                        xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}dhSaiEnt").FirstOrDefault().Value = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");

                DataSet dsSalvar;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sfuncao", "SALVAR");
                vParametros.Add("@idImportador", hddidImportador.Value);
                vParametros.Add("@idArquivo", hddidArquivo.Value);
                vParametros.Add("@idParceiro_Emit", ddlidParceiroEmit.SelectedValue);
                vParametros.Add("@idEndereco_Emit", ddlidEnderecoEmit.SelectedValue);
                vParametros.Add("@sCNPJ_Emit", txtemitCNPJ.Text);
                vParametros.Add("@sNome_Emit", txtemitxNome.Text);
                vParametros.Add("@xFant_Emit", txtemitxFant.Text);
                vParametros.Add("@xLgr_Emit", txtemitxLgr.Text);
                vParametros.Add("@nro_Emit", txtemitnro.Text);
                vParametros.Add("@xCpl_Emit", "");
                vParametros.Add("@xBairro_Emit", txtemitxBairro.Text);
                vParametros.Add("@cMun_Emit", txtemitcMun.Text);
                vParametros.Add("@xMun_Emit", txtemitxMun.Text);
                vParametros.Add("@UF_Emit", txtemitUF.Text);
                vParametros.Add("@CEP_Emit", txtemitCEP.Text);
                vParametros.Add("@cPais_Emit", txtemitcPais.Text);
                vParametros.Add("@xPais_Emit", txtemitxPais.Text);
                vParametros.Add("@fone_Emit", txtemitfone.Text);
                vParametros.Add("@IE_Emit", txtemitIE.Text);
                vParametros.Add("@CRT_Emit", txtemitCRT.Text);
                vParametros.Add("@idParceiro_Dest", ddlidParceiroDest.SelectedValue);
                vParametros.Add("@idEndereco_Dest", ddlidEnderecoDest.SelectedValue);
                vParametros.Add("@sCNPJ_Dest", txtdestCNPJ.Text);
                vParametros.Add("@sNome_Dest", txtdestxNome.Text);
                vParametros.Add("@xFant_Dest", "");
                vParametros.Add("@xLgr_Dest", txtdestxLgr.Text);
                vParametros.Add("@nro_Dest", txtdestnro.Text);
                vParametros.Add("@xCpl_Dest", txtdestxCpl.Text);
                vParametros.Add("@xBairro_Dest", txtdestxBairro.Text);
                vParametros.Add("@cMun_Dest", txtdestcMun.Text);
                vParametros.Add("@xMun_Dest", txtdestxMun.Text);
                vParametros.Add("@UF_Dest", txtdestUF.Text);
                vParametros.Add("@CEP_Dest", "");
                vParametros.Add("@cPais_Dest", txtdestcPais.Text);
                vParametros.Add("@xPais_Dest", txtdestxPais.Text);
                vParametros.Add("@IE_Dest", txtIEdest.Text);
                vParametros.Add("@CRT_Dest", "");
                vParametros.Add("@indIEDest_Dest", txtdestindIEDest.Text);
                vParametros.Add("@nNF", "S");
                vParametros.Add("@modFrete", ddlModoFrete.SelectedValue);
                vParametros.Add("@CNPJtransp", txtCNPJTrans.Text);
                vParametros.Add("@xNometransp", txtNomeTrans.Text);
                vParametros.Add("@IEtransp", txtIETrans.Text);
                vParametros.Add("@xEndertransp", txtEnderecoTrans.Text);
                vParametros.Add("@xMuntransp", txtMunicipiotrans.Text);
                vParametros.Add("@UFtransp", txtUFTrans.Text);
                vParametros.Add("@qVol", txtVolumes.Text);
                vParametros.Add("@esp", txtEspecie.Text);
                vParametros.Add("@pesoL", txtPesoLiquido.Text.Replace(",", "."));
                vParametros.Add("@pesoB", txtPesoBruto.Text.Replace(",", "."));
                vParametros.Add("@idTransportadora", ddlidTranportadora.SelectedValue);
                vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                if (sGerarNFe == "S")
                {
                    vParametros.Add("@sGerarNFe", "S");
                    vParametros.Add("@idStatus", "1");
                }
                else
                {
                    vParametros.Add("@idStatus", "0");
                    vParametros.Add("@sGerarNFe", "N");
                }
                dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ImportadorNfe", vParametros);

                foreach (GridViewRow item in dtgvConsulta.Rows)
                {
                    string nItemValue = (item.FindControl("nOrdem") as Label).Text;
                    //DropDownList ddlItem = (item.FindControl("ddlItemAdd") as DropDownList);
                    //DropDownList ddlItemFornecedor = (item.FindControl("ddlItemFornecedor") as DropDownList);
                    DropDownList ddlPedidos = (item.FindControl("ddlPedidos") as DropDownList);
                    DropDownList Produtos_ddlidItem = (item.FindControl("Produtos_ddlidItem") as DropDownList);

                    int idItens = Convert.ToInt32(dtgvConsulta.DataKeys[item.RowIndex].Value);

                    var itens = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Select(d => new
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

                    }).ToList();

                    var itemXML = itens.FirstOrDefault(i => i.nItem == nItemValue);

                    if (itemXML != null)
                    {
                        string Cod = "";
                        string Desc = "";

                        var txtCProd = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}cProd").FirstOrDefault().Value;
                        var txtxProd = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}xProd").FirstOrDefault().Value;

                        string sDscFornecedorProduto = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}xProd").FirstOrDefault().Value;
                        string sCodigoFornecedor = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}cProd").FirstOrDefault().Value;
                        string nValorUnitario = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}vUnCom").FirstOrDefault().Value;
                        string vUnTrib = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}vUnTrib").FirstOrDefault().Value;

                        nValorUnitario = nValorUnitario.Split('.')[0] + "." + nValorUnitario.Split('.')[1].PadRight(10, '0');
                        vUnTrib = vUnTrib.Split('.')[0] + "." + vUnTrib.Split('.')[1].PadRight(10, '0');

                        string id = "0";


                        var txtcEAN = item.FindControl("EAN") as Label;
                        var txtNCM = item.FindControl("NCM") as Label;
                        var txtCFOP = item.FindControl("CFOP") as Label;
                        var txtuCom = item.FindControl("uCom") as Label;
                        var txtqCom = item.FindControl("qCom") as Label;
                        var txtvUnCom = item.FindControl("vUnCom") as Label;
                        var txtvProd = item.FindControl("vProd") as Label;
                        var txtcEANTrib = item.FindControl("cEANTrib") as Label;
                        var txtuTrib = item.FindControl("uTrib") as Label;
                        var txtqTrib = item.FindControl("qTrib") as Label;
                        var txtvUnTrib = item.FindControl("vUnTrib") as Label;
                        var txtindTot = item.FindControl("indTot") as Label;
                        var txtxPed = item.FindControl("xPed") as Label;

                        if (txtCProd != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                           .Descendants("{http://www.portalfiscal.inf.br/nfe}cProd").FirstOrDefault()?.Value)
                        {
                            xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}cProd").FirstOrDefault().Value = txtCProd;
                        }
                        //if (txtcEAN.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                        //   .Descendants("{http://www.portalfiscal.inf.br/nfe}cEAN").FirstOrDefault()?.Value)
                        //{
                        //    xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                        //    .Descendants("{http://www.portalfiscal.inf.br/nfe}cEAN").FirstOrDefault().Value = txtcEAN.Text;
                        //}
                        if (txtxProd != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                           .Descendants("{http://www.portalfiscal.inf.br/nfe}xProd").FirstOrDefault()?.Value)
                        {
                            xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}xProd").FirstOrDefault().Value = txtxProd;
                        }
                        if (txtNCM.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                           .Descendants("{http://www.portalfiscal.inf.br/nfe}NCM").FirstOrDefault()?.Value)
                        {
                            xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}NCM").FirstOrDefault().Value = txtNCM.Text;
                        }
                        if (txtCFOP.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                           .Descendants("{http://www.portalfiscal.inf.br/nfe}CFOP").FirstOrDefault()?.Value)
                        {
                            xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}CFOP").FirstOrDefault().Value = txtCFOP.Text;
                        }
                        if (txtuCom.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                           .Descendants("{http://www.portalfiscal.inf.br/nfe}uCom").FirstOrDefault()?.Value)
                        {
                            xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}uCom").FirstOrDefault().Value = txtuCom.Text;
                        }
                        if (txtqCom.Text.Replace(",", ".") != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                           .Descendants("{http://www.portalfiscal.inf.br/nfe}qCom").FirstOrDefault()?.Value)
                        {
                            xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}qCom").FirstOrDefault().Value = txtqCom.Text.Replace(",", ".");
                        }
                        if (nValorUnitario.Replace(",", ".") != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                           .Descendants("{http://www.portalfiscal.inf.br/nfe}vUnCom").FirstOrDefault()?.Value)
                        {
                            xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}vUnCom").FirstOrDefault().Value = nValorUnitario.Replace(",", ".");
                        }
                        if (txtvProd.Text.Replace(",", ".") != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                           .Descendants("{http://www.portalfiscal.inf.br/nfe}vProd").FirstOrDefault()?.Value)
                        {
                            xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}vProd").FirstOrDefault().Value = txtvProd.Text.Replace(",", ".");
                        }
                        if (txtcEANTrib.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                           .Descendants("{http://www.portalfiscal.inf.br/nfe}cEANTrib").FirstOrDefault()?.Value)
                        {
                            xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}cEANTrib").FirstOrDefault().Value = txtcEANTrib.Text;
                        }
                        if (txtuTrib.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                           .Descendants("{http://www.portalfiscal.inf.br/nfe}uTrib").FirstOrDefault()?.Value)
                        {
                            xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}uTrib").FirstOrDefault().Value = txtuTrib.Text;
                        }
                        if (txtqTrib.Text.Replace(",", ".") != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                           .Descendants("{http://www.portalfiscal.inf.br/nfe}qTrib").FirstOrDefault()?.Value)
                        {
                            xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}qTrib").FirstOrDefault().Value = txtqTrib.Text.Replace(",", ".");
                        }
                        if (vUnTrib.Replace(",", ".") != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                           .Descendants("{http://www.portalfiscal.inf.br/nfe}vUnTrib").FirstOrDefault()?.Value)
                        {
                            xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}vUnTrib").FirstOrDefault().Value = vUnTrib.Replace(",", ".");
                        }
                        if (txtindTot.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                           .Descendants("{http://www.portalfiscal.inf.br/nfe}indTot").FirstOrDefault()?.Value)
                        {
                            xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                            .Descendants("{http://www.portalfiscal.inf.br/nfe}indTot").FirstOrDefault().Value = txtindTot.Text;
                        }
                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                           .Descendants("{http://www.portalfiscal.inf.br/nfe}xPed").FirstOrDefault() != null)
                        {
                            if (txtxPed.Text != xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                                 .Descendants("{http://www.portalfiscal.inf.br/nfe}xPed").FirstOrDefault()?.Value)
                            {
                                xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}prod")
                                .Descendants("{http://www.portalfiscal.inf.br/nfe}xPed").FirstOrDefault().Value = txtxPed.Text;
                            }
                        }
                        //ICMS
                        string orig = "";
                        string CST_ICMS = "";
                        string modBC = "";
                        string vBC_ICMS = "";
                        string pRedBC = "";
                        string pICMS = "";
                        string vICMS = "";
                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}orig").FirstOrDefault() != null)
                            orig = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}orig").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault() != null)
                            CST_ICMS = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}modBC").FirstOrDefault() != null)
                            modBC = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}modBC").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                            vBC_ICMS = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}pRedBC").FirstOrDefault() != null)
                            pRedBC = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}pRedBC").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}pICMS").FirstOrDefault() != null)
                            pICMS = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}pICMS").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault() != null)
                            vICMS = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault()?.Value;

                        ////IPI
                        string cEnq = "";
                        string CST_IPI = "";
                        string vBC_IPI = "";
                        string pIPI = "";
                        string vIPI = "";
                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}cEnq").FirstOrDefault() != null)
                            cEnq = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}cEnq").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault() != null)
                            CST_IPI = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                            vBC_IPI = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}pIPI").FirstOrDefault() != null)
                            pIPI = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}pIPI").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault() != null)
                            vIPI = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault()?.Value;

                        ////II
                        string vBC_II = "";
                        string vDespAdu = "";
                        string vII = "";
                        string vIOF = "";
                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                            vBC_II = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vDespAdu").FirstOrDefault() != null)
                            vDespAdu = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vDespAdu").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault() != null)
                            vII = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vIOF").FirstOrDefault() != null)
                            vIOF = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vIOF").FirstOrDefault()?.Value;

                        ////PIS
                        string CST_PIS = "";
                        string vBC_PIS = "";
                        string pPIS = "";
                        string vPIS = "";
                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault() != null)
                            CST_PIS = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                            vBC_PIS = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}pPIS").FirstOrDefault() != null)
                            pPIS = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}pPIS").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault() != null)
                            vPIS = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault()?.Value;

                        ////COFINS
                        string CST_COFINS = "";
                        string vBC_COFINS = "";
                        string pCOFINS = "";
                        string vCOFINS = "";
                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault() != null)
                            CST_COFINS = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                            vBC_COFINS = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}pCOFINS").FirstOrDefault() != null)
                            pCOFINS = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}pCOFINS").FirstOrDefault()?.Value;

                        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault() != null)
                            vCOFINS = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det").Where(d => d.Attribute("nItem")?.Value == nItemValue).Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault()?.Value;

                        DataSet dsSalvarItens;
                        Dictionary<String, String> vParametrosItens = new Dictionary<string, string>();
                        vParametrosItens.Add("@sfuncao", "SALVAR_ITENS");
                        vParametrosItens.Add("@idItens", idItens.ToString());
                        vParametrosItens.Add("@idImportador", hddidImportador.Value);
                        vParametrosItens.Add("@nItem", nItemValue);
                        vParametrosItens.Add("@cProd", txtCProd);
                        vParametrosItens.Add("@cEAN", txtcEAN.Text);
                        vParametrosItens.Add("@xProd", txtxProd);
                        vParametrosItens.Add("@NCM", txtNCM.Text);
                        vParametrosItens.Add("@CFOP", txtCFOP.Text);
                        vParametrosItens.Add("@uCom", txtuCom.Text);
                        vParametrosItens.Add("@qCom", txtqCom.Text.Replace(",", "."));
                        vParametrosItens.Add("@vUnCom", txtvUnCom.Text.Replace(",", "."));
                        vParametrosItens.Add("@vProd", txtvProd.Text.Replace(",", "."));
                        vParametrosItens.Add("@cEANTrib", txtcEANTrib.Text);
                        vParametrosItens.Add("@uTrib", txtuTrib.Text);
                        vParametrosItens.Add("@qTrib", txtqTrib.Text.Replace(",", "."));
                        vParametrosItens.Add("@vUnTrib", txtvUnTrib.Text.Replace(",", "."));
                        vParametrosItens.Add("@indTot", txtindTot.Text);
                        vParametrosItens.Add("@xPed", txtxPed.Text);
                        vParametrosItens.Add("@idProduto", id);
                        vParametrosItens.Add("@idPedido", ddlPedidos.SelectedValue);
                        //ICMS
                        if (orig != "")
                            vParametrosItens.Add("@orig", orig);
                        if (CST_ICMS != "")
                            vParametrosItens.Add("@CST_ICMS", CST_ICMS);
                        if (modBC != "")
                            vParametrosItens.Add("@modBC", modBC);
                        if (vBC_ICMS != "")
                            vParametrosItens.Add("@vBC_ICMS", vBC_ICMS);
                        if (pRedBC != "")
                            vParametrosItens.Add("@pRedBC", pRedBC);
                        if (pICMS != "")
                            vParametrosItens.Add("@pICMS", pICMS);
                        if (vICMS != "")
                            vParametrosItens.Add("@vICMS", vICMS);
                        //IPI
                        if (cEnq != "")
                            vParametrosItens.Add("@cEnq", cEnq);
                        if (CST_IPI != "")
                            vParametrosItens.Add("@CST_IPI", CST_IPI);
                        if (vBC_IPI != "")
                            vParametrosItens.Add("@vBC_IPI", vBC_IPI);
                        if (pIPI != "")
                            vParametrosItens.Add("@pIPI", pIPI);
                        if (vIPI != "")
                            vParametrosItens.Add("@vIPI", vIPI);
                        //II
                        if (vBC_II != "")
                            vParametrosItens.Add("@vBC_II", vBC_II);
                        if (vDespAdu != "" && vDespAdu != null)
                            vParametrosItens.Add("@vDespAdu", vDespAdu);
                        if (vII != "")
                            vParametrosItens.Add("@vII", vII);
                        if (vIOF != "")
                            vParametrosItens.Add("@vIOF", vIOF);
                        //PIS
                        if (CST_PIS != "")
                            vParametrosItens.Add("@CST_PIS", CST_PIS);
                        if (vBC_PIS != "")
                            vParametrosItens.Add("@vBC_PIS", vBC_PIS);
                        if (pPIS != "")
                            vParametrosItens.Add("@pPIS", pPIS);
                        if (vPIS != "")
                            vParametrosItens.Add("@vPIS", vPIS);
                        //COFINS
                        if (CST_COFINS != "")
                            vParametrosItens.Add("@CST_COFINS", CST_COFINS);
                        if (vBC_COFINS != "")
                            vParametrosItens.Add("@vBC_COFINS", vBC_COFINS);
                        if (pCOFINS != "")
                            vParametrosItens.Add("@pCOFINS", pCOFINS);
                        if (vCOFINS != "")
                            vParametrosItens.Add("@vCOFINS", vCOFINS);

                        vParametrosItens.Add("@sCodigo", Produtos_ddlidItem.SelectedValue);
                        vParametrosItens.Add("@idParceiro_Dest", ddlidParceiroDest.SelectedValue);
                        dsSalvarItens = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ImportadorNfe", vParametrosItens);
                    }
                }

                xdoc.Save(filePath);

                Byte[] lObjArquivo = null;
                TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                try
                {
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(filePath, fileStream);
                    }

                    if (lObjArquivo != null && lObjArquivo.Length > 0)
                    {
                        SqlDataAdapter da = new SqlDataAdapter("sp_Manipula_tbl_Flow_Arquivos", TT.FrameWork.BD.StringDeConexao);

                        DataSet tabela = new DataSet();
                        da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "ALTERARXML";
                        da.SelectCommand.Parameters.Add("@vbArquivo", SqlDbType.VarBinary).Value = lObjArquivo;
                        da.SelectCommand.Parameters.Add("@idArquivo", SqlDbType.Int).Value = Convert.ToInt32(hddidArquivo.Value);

                        try
                        {
                            da.Fill(tabela);
                            if (sFuncao.ToUpper() == "CONSULTAR")
                            {
                                Pesquisar(hddidImportador.Value, false);
                                MensagemPagina.MostraMensagem_Sucesso("XML salvo com sucesso!");
                            }
                        }
                        catch (Exception ex)
                        {
                            MensagemPagina.MostraMensagem_Erro(string.Format("Erro BD-DS: {0}", ex.Message));
                        }
                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Erro("Erro: O arquivo não foi convertido corretamente em bytes.");
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(string.Format("Erro ao ler o arquivo: {0}", ex.Message));
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        protected void cmdImportarNovoXML_Click(object sender, EventArgs e)
        {
            try
            {
                if (fu_Arquivo.HasFile)
                {
                    string fileName = fu_Arquivo.PostedFile.FileName;
                    if (fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    {
                        string filePath = fileName;
                        Byte[] lObjArquivo = null;
                        TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                        lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(fu_Arquivo.FileName, fu_Arquivo.PostedFile.InputStream);

                        SqlDataAdapter da = new SqlDataAdapter("sp_Manipula_tbl_Flow_Arquivos", TT.FrameWork.BD.StringDeConexao);

                        DataSet tabela = new DataSet();
                        da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        SqlCommand lObjCommand = new SqlCommand();

                        da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "INCLUIR";
                        da.SelectCommand.Parameters.Add("@idTipoArquivo", SqlDbType.Int).Value = 10003;
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
                                SalvarXML(Arquivo);
                            }

                            FUNCOES.DirecionaPagina("App/Paginas/Adm/Faturamento/ImportadorNFE_Detalhe.aspx?id=" + hddidImportador.Value);
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

            //--emit
            if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                emitCNPJ = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;
            else
                DIV_CNPJ.Visible = false;

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
                DIV_destCNPJ.Visible = false;

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
            vParametros.Add("@idParceiro_Emit", "");
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
            vParametros.Add("@idParceiro_Dest", "");
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
            vParametros.Add("@pesoL", pesoL);
            vParametros.Add("@pesoB", pesoB);
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
                vParametrosItens.Add("@qCom", linha.qCom);
                vParametrosItens.Add("@vUnCom", linha.vUnCom);
                vParametrosItens.Add("@vProd", linha.vProd);
                vParametrosItens.Add("@cEANTrib", linha.cEANTrib);
                vParametrosItens.Add("@uTrib", linha.uTrib);
                vParametrosItens.Add("@qTrib", linha.qTrib);
                vParametrosItens.Add("@vUnTrib", linha.vUnTrib);
                vParametrosItens.Add("@indTot", linha.indTot);
                if (linha.xPed != null)
                {
                    vParametrosItens.Add("@xPed", linha.xPed);
                }
                else
                {
                    vParametrosItens.Add("@xPed", "");
                }
                //ICMS
                if (linha.orig != "")
                    vParametrosItens.Add("@orig", linha.orig);

                if (linha.CST_ICMS != "")
                    vParametrosItens.Add("@CST_ICMS", linha.CST_ICMS);

                if (linha.modBC != "")
                    vParametrosItens.Add("@modBC", linha.modBC);

                if (linha.vBC_ICMS != "")
                    vParametrosItens.Add("@vBC_ICMS", linha.vBC_ICMS);

                if (linha.pRedBC != "")
                    vParametrosItens.Add("@pRedBC", linha.pRedBC);

                if (linha.pICMS != "")
                    vParametrosItens.Add("@pICMS", linha.pICMS);

                if (linha.vICMS != "")
                    vParametrosItens.Add("@vICMS", linha.vICMS);
                //IPI
                if (linha.cEnq != "")
                    vParametrosItens.Add("@cEnq", linha.cEnq);

                if (linha.CST_IPI != "")
                    vParametrosItens.Add("@CST_IPI", linha.CST_IPI);

                if (linha.vBC_IPI != "")
                    vParametrosItens.Add("@vBC_IPI", linha.vBC_IPI);

                if (linha.pIPI != "")
                    vParametrosItens.Add("@pIPI", linha.pIPI);

                if (linha.vIPI != "")
                    vParametrosItens.Add("@vIPI", linha.vIPI);
                //II
                if (linha.vBC_II != "" && linha.vDespAdu != "" && linha.vII != "" && linha.vIOF != "")
                {
                    if (linha.vBC_II != "")
                        vParametrosItens.Add("@vBC_II", linha.vBC_II);

                    if (linha.vDespAdu != "")
                        vParametrosItens.Add("@vDespAdu", linha.vDespAdu);

                    if (linha.vII != "")
                        vParametrosItens.Add("@vII", linha.vII);

                    if (linha.vIOF != "")
                        vParametrosItens.Add("@vIOF", linha.vIOF);
                }
                //PIS
                if (linha.CST_PIS != "")
                    vParametrosItens.Add("@CST_PIS", linha.CST_PIS);

                if (linha.vBC_PIS != "")
                    vParametrosItens.Add("@vBC_PIS", linha.vBC_PIS);

                if (linha.pPIS != "")
                    vParametrosItens.Add("@pPIS", linha.pPIS);

                if (linha.vPIS != "")
                    vParametrosItens.Add("@vPIS", linha.vPIS);
                //COFINS
                if (linha.CST_COFINS != "")
                    vParametrosItens.Add("@CST_COFINS", linha.CST_COFINS);

                if (linha.vBC_COFINS != "")
                    vParametrosItens.Add("@vBC_COFINS", linha.vBC_COFINS);

                if (linha.pCOFINS != "")
                    vParametrosItens.Add("@pCOFINS", linha.pCOFINS);

                if (linha.vCOFINS != "")
                    vParametrosItens.Add("@vCOFINS", linha.vCOFINS);

                dsSalvarItens = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ImportadorNfe", vParametrosItens);
            }
        }

        protected void cmdGerarNFPiloto_Click(object sender, EventArgs e)
        {
            try
            {
                //if (DIV15.Visible == false)
                //{
                //    DIV15.Visible = true;
                //    txtsObservacao.Focus();
                //}
                //else if (DIV15.Visible == true && txtsObservacao.Text != "" && ddlidArmazenamento.SelectedValue != "0")
                if (Validar())
                {
                    Salvar("INCLUIR");
                    string filePath = Server.MapPath("~/Download/" + hddsNomeArquivo.Value);

                    XmlDocument XML_NFe = new XmlDocument();
                    XML_NFe.Load(filePath);

                    XmlNamespaceManager nsManager = new XmlNamespaceManager(XML_NFe.NameTable);
                    nsManager.AddNamespace("nfe", "http://www.portalfiscal.inf.br/nfe");

                    XmlNode infNFe = XML_NFe.SelectSingleNode("//nfe:infNFe", nsManager);
                    XmlNode emit = XML_NFe.SelectSingleNode("//nfe:infNFe/nfe:emit", nsManager);
                    XmlNode ide = XML_NFe.SelectSingleNode("//nfe:infNFe/nfe:ide", nsManager);


                    string cUF = ide["cUF"].InnerText;
                    string cNF = new Random().Next(10000000, 99999999).ToString();
                    string mod = ide["mod"].InnerText;
                    string serie = "99";
                    string nNF = "999999";
                    string tpEmis = ide["tpEmis"].InnerText;
                    string cnpj = emit["CNPJ"].InnerText;
                    string anoMesEmissao = DateTime.Now.ToString("yyMM");
                    string chaveAcesso = XML_GerarNFE.GerarChaveAcesso(cUF, anoMesEmissao, cnpj, mod, serie, nNF, tpEmis, cNF);

                    string sNome_Emitente = emit["xNome"].InnerText;
                    string sNome_Destinatario = txtdestxNome.Text;


                    //Atualiza os Valores
                    ide["cNF"].InnerText = cNF;
                    ide["serie"].InnerText = serie;
                    ide["nNF"].InnerText = nNF;
                    ide["dhEmi"].InnerText = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                    ide["dhSaiEnt"].InnerText = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                    ide["cDV"].InnerText = chaveAcesso.Substring(46, 1);
                    infNFe.Attributes[0].Value = chaveAcesso;



                    //if (Incluir == true)
                    {
                        DataSet dsPesquisaGerenciador;
                        Dictionary<String, String> vParametrosGerenciador = new Dictionary<string, string>();
                        vParametrosGerenciador.Add("@sfuncao", "INCLUIR_XML");
                        vParametrosGerenciador.Add("@idTipoObjeto", "12");
                        vParametrosGerenciador.Add("@idObjeto", hddidImportador.Value);
                        vParametrosGerenciador.Add("@sChaveNFe", chaveAcesso.Replace("NFe", ""));
                        vParametrosGerenciador.Add("@idOrigem", "3");
                        vParametrosGerenciador.Add("@sCNPJ_Emitente", cnpj);
                        vParametrosGerenciador.Add("@nSerie", serie);
                        vParametrosGerenciador.Add("@nNumeroNF", nNF);
                        vParametrosGerenciador.Add("@sXML", XML_NFe.InnerXml);
                        vParametrosGerenciador.Add("@sNome_Emitente", sNome_Emitente);
                        //vParametrosGerenciador.Add("@sNomeFantasia_Emitente", xFant);
                        //vParametrosGerenciador.Add("@sLogradouro_Emitente", xLgr);
                        //vParametrosGerenciador.Add("@sNumero_Emitente", nro);
                        //vParametrosGerenciador.Add("@sUF_Emitente", UF);
                        //vParametrosGerenciador.Add("@sCEP_Emitente", CEP);
                        //vParametrosGerenciador.Add("@sPais_Emitente", xPais);
                        //vParametrosGerenciador.Add("@sTelefone_Emitente", fone);
                        vParametrosGerenciador.Add("@sNome_Destinatario", sNome_Destinatario);
                        vParametrosGerenciador.Add("@sTipoXML", "2");

                        dsPesquisaGerenciador = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametrosGerenciador);

                        if (dsPesquisaGerenciador != null)
                        {
                            Pesquisar(hddidImportador.Value, false);
                            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        //protected void btnGerar_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DataSet dsPesquisa;
        //        Dictionary<String, String> vParametros = new Dictionary<string, string>();
        //        vParametros.Add("@sfuncao", "Alterar_sGerarNFe");
        //        vParametros.Add("@idImportador", hddidImportador.Value);
        //        vParametros.Add("@sGerarNFe", "S");
        //        dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ImportadorNfe", vParametros);

        //        string filePath = Server.MapPath("~/Download/" + hddsNomeArquivo.Value);
        //        XDocument xdoc = XDocument.Load(filePath);

        //        string chave = "";
        //        string CNPJ = "";
        //        string Corpo = "";
        //        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}nfeProc").FirstOrDefault() != null)
        //            Corpo = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}nfeProc").FirstOrDefault().ToString();
        //        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}NFe").FirstOrDefault() != null)
        //            Corpo = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}NFe").FirstOrDefault().ToString();
        //        var infNFeElement = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}infNFe").FirstOrDefault();
        //        if (infNFeElement != null)
        //        {
        //            chave = infNFeElement.Attribute("Id")?.Value?.Substring(3);
        //        }
        //        if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
        //            CNPJ = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

        //        if (sGerarNFe == "S")
        //        {
        //            DataSet dsPesquisaidTipoObjeto;
        //            Dictionary<String, String> vParametrosidTipoObjeto = new Dictionary<string, string>();
        //            vParametrosidTipoObjeto.Add("@sfuncao", "ALTERAR_idTipoObjeto");
        //            vParametrosidTipoObjeto.Add("@sChaveNFe", chave);
        //            vParametrosidTipoObjeto.Add("@idTipoObjeto", "12");
        //            vParametrosidTipoObjeto.Add("@sObservacao", "");
        //            vParametrosidTipoObjeto.Add("@sXML", Corpo.ToString());
        //            dsPesquisaidTipoObjeto = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametrosidTipoObjeto);
        //        }

        //        string baseDir = hddsCaminho_UniNFe.Value;
        //        string cnpjPattern = @"\d{14}";
        //        var cnpjDirectories = Directory.GetDirectories(baseDir);

        //        foreach (var cnpjDir in cnpjDirectories)
        //        {
        //            string dirName = Path.GetFileName(cnpjDir);
        //            if (Regex.IsMatch(dirName, cnpjPattern))
        //            {
        //                if (dirName == CNPJ)
        //                {
        //                    string[] pastas = new string[]
        //                    {
        //                            Path.Combine(cnpjDir, "Envio")
        //                    };

        //                    foreach (var pasta in pastas)
        //                    {
        //                        if (Directory.Exists(pasta))
        //                        {
        //                            if (IncluirArquivo != "N")
        //                            {
        //                                string Arquivo = filePath;

        //                                if (File.Exists(Arquivo))
        //                                {
        //                                    try
        //                                    {
        //                                        string nomeArquivoDestino = Path.Combine(pasta, Path.GetFileName(chave + "-nfe" + ".xml"));

        //                                        File.Copy(Arquivo, nomeArquivoDestino);
        //                                    }
        //                                    catch (Exception ex)
        //                                    {
        //                                        MensagemPagina.MostraMensagem_Erro(string.Format("Erro: {0}", ex.Message));
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        Pesquisar(hddidImportador.Value, false);
        //        MensagemPagina.MostraMensagem_Sucesso("NF-e está em processo de emissão!");
        //    }
        //    catch (Exception ex)
        //    {
        //        MensagemPagina.MostraMensagem_Erro(ex.Message);

        //    }
        //}

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

            sb.Append("$v192(function() {");
            sb.Append("$v192(\"#dialog-Excluir\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_Excluir\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=btnExcluir]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Excluir').dialog('open');");
            sb.Append("});");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina" + Guid.NewGuid(), sb.ToString(), true);
        }
        #endregion

        #region | Evento

        private bool ValidarControles(bool bNovaImportacao)
        {
            DIV_DADOS.Visible = !bNovaImportacao;
            div_Importador.Visible = bNovaImportacao;
            cmdImportarNovoXML.Visible = bNovaImportacao;
            btnSalvar.Visible = !bNovaImportacao;
            cmdGerarNFPiloto.Visible = !bNovaImportacao;
            return !bNovaImportacao;
        }

        private bool Validar()
        {
            string sMensagem = "";
            if (DIV8.Visible == true)
            {
                if (ddlidParceiroEmit.SelectedValue == "0")
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Parceiro do Emitente!";
                }
            }

            if (Div10.Visible == true)
            {
                if (ddlidEnderecoEmit.SelectedValue == "0")
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Endereço do Emitente!";
                }
            }

            if (Div27.Visible == true)
            {
                if (ddlidParceiroDest.SelectedValue == "0")
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Parceiro do destinatário!";
                }
            }

            if (Div5.Visible == true)
            {
                if (ddlidEnderecoDest.SelectedValue == "0")
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Endereço do destinatário!";
                }
            }

            foreach (GridViewRow row in dtgvConsulta.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    DropDownList Produtos_ddlidItem = (row.FindControl("Produtos_ddlidItem") as DropDownList);

                    if (Produtos_ddlidItem.SelectedValue == "")
                    {
                        string cProd = row.Cells[2].Text.Replace("-", "");

                        sMensagem += (sMensagem != "" ? "</br>" : "") + string.Format("Falta vincular o item: {0} - {1}!", row.Cells[1].Text, row.Cells[2].Text);
                    }

                }
            }

            if (sMensagem != "")
            {
                MensagemPagina.MostraMensagem_Erro(sMensagem);
                return false;
            }
            return true;
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (hddidStatus.Value == "0")
                {

                    DropDownList Produtos_ddlidItem = (e.Row.FindControl("Produtos_ddlidItem") as DropDownList);
                    DropDownList ddlPedidos = (e.Row.FindControl("ddlPedidos") as DropDownList);
                    int nIndex = Base_Importador_Itens.FindIndex(x => x.nItem.Equals(Convert.ToInt32((e.Row.FindControl("nOrdem") as Label).Text)));

                    string cProd = e.Row.Cells[2].Text.Replace("-", "");
                    string[] vPesquisa = cProd.Split(new string[] { " " }, StringSplitOptions.None);


                    //ddlPedidos = ddlPedidos_Geral;

                    FUNCOES.Popula_Combo(ddlPedidos, "sp_Select 'FLOW-PEDIDO-COMEX_IMPORTACAO', @idPesquisa=" + ddlidParceiroDest.SelectedValue, "idPedido", "sReferencia", false, "Selecione o Pedido", "0");
                    if (Base_Importador_Itens[nIndex].idPedido.ToString() != "0")
                        ddlPedidos.SelectedValue = Base_Importador_Itens[nIndex].idPedido.ToString();


                    if (ddlPedidos.SelectedValue != "0")
                    {
                        Carregar_ItensPedido(ddlPedidos.SelectedValue, e.Row);
                        //Produtos_ddlidItem, vPesquisa[0].Replace("-", ""));
                    }

                    if (Base_Importador_Itens[nIndex].idProduto != 0)
                    {
                        Produtos_ddlidItem.SelectedValue = Base_Importador_Itens[nIndex].sCodigo;
                    }
                }
                //

            }

            GRID.EsconderColunas(e, 5, 10, 11, 12, 13);

            if (hddidStatus.Value == "0")
            {
                GRID.EsconderColunas(e, 14, 15, 19);
            }
            else
            {
                GRID.EsconderColunas(e, 14, 15, 16, 17, 18);
            }


        }

        protected void ddlidParceiroDest_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hddsEnderecoDest.Value == "S")
                Div5.Visible = true;
            else
                Div5.Visible = false;
            FUNCOES.Popula_Combo(ddlidEnderecoDest, string.Format("sp_Select 'Flow_Clientes_Endereco', {0}", ddlidParceiroDest.SelectedValue), "idEndereco", "sEnderecoClt", false, "Selecione um Endereço", "0");
            if (ddlidParceiroDest.SelectedValue != "0")
            {
                string Parceiro = ddlidParceiroDest.SelectedItem.ToString();
                string[] partes = Parceiro.Split(new string[] { " - " }, StringSplitOptions.None);

                if (partes.Count() > 2)
                {
                    txtdestCNPJ.Text = partes[0];
                    for (int i = 1; i < partes.Count(); i++)
                    {
                        if (i == 1)
                            txtdestxNome.Text += partes[i] + " - ";
                        else
                            txtdestxNome.Text += partes[i];
                    }
                }
                else if (partes.Count() == 1)
                {
                    txtdestxNome.Text = partes[0].Replace("- ", "");
                }
                else
                {
                    txtdestCNPJ.Text = partes[0];
                    txtdestxNome.Text = partes[1];
                }

                CarregarItens(hddidImportador.Value);
            }
        }

        protected void ddlidEnderecoDest_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidEnderecoDest.SelectedValue != "0")
            {
                string Endereco = ddlidEnderecoDest.SelectedItem.ToString();
                string[] partes = Endereco.Split(new string[] { "-" }, StringSplitOptions.None);
                if (partes[0] != "")
                    txtdestxLgr.Text = partes[0];
                if (partes[1] != "")
                    txtdestnro.Text = partes[1];
                if (partes[3] != "")
                    txtdestxCpl.Text = partes[3];
                if (partes[2] != "")
                    txtdestxBairro.Text = partes[2];
                if (partes[8] != "")
                    txtdestcMun.Text = partes[8];
                if (partes[4] != "")
                    txtdestxMun.Text = partes[4];
                if (partes[5] != "" && partes[5] != "  ")
                    txtdestUF.Text = partes[5];
                if (partes[9] != "")
                    txtdestcPais.Text = partes[9];
                if (partes[7] != "")
                    txtdestxPais.Text = partes[7];
                if (partes[10] != "")
                {
                    DIV_IEdest.Visible = true;
                    txtIEdest.Text = partes[10];
                }
            }
        }

        protected void ddlidParceiroEmit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hddsEnderecoEmit.Value == "S")
                Div10.Visible = true;
            else
                Div5.Visible = false;
            FUNCOES.Popula_Combo(ddlidEnderecoEmit, string.Format("sp_Select 'Flow_Clientes_Endereco', {0}", ddlidParceiroEmit.SelectedValue), "idEndereco", "sEnderecoClt", false, "Selecione um Endereço", "0");
            if (ddlidParceiroEmit.SelectedValue != "0")
            {
                string Parceiro = ddlidParceiroEmit.SelectedItem.ToString();
                string[] partes = Parceiro.Split(new string[] { " - " }, StringSplitOptions.None);

                if (partes.Count() > 2)
                {
                    txtemitCNPJ.Text = partes[0];
                    for (int i = 1; i < partes.Count(); i++)
                    {
                        if (i == 1)
                            txtemitxNome.Text += partes[i] + " - ";
                        else
                            txtemitxNome.Text += partes[i];
                    }
                }
                else
                {
                    txtemitCNPJ.Text = partes[0];
                    txtemitxNome.Text = partes[1];
                }

                DataSet dsPesquisaParceiroEmit;
                Dictionary<String, String> vParametrosParceiroEmit = new Dictionary<string, string>();
                vParametrosParceiroEmit.Add("@sfuncao", "ConsultarParceiro");
                vParametrosParceiroEmit.Add("@sCPF_CNPJ", txtemitCNPJ.Text);
                dsPesquisaParceiroEmit = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParametrosParceiroEmit);
                idEmpresa = RETORNO.DATASET(dsPesquisaParceiroEmit, 0, "idEmpresa");
            }
        }

        protected void ddlidEnderecoEmit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidEnderecoEmit.SelectedValue != "0")
            {
                string Endereco = ddlidEnderecoEmit.SelectedItem.ToString();
                string[] partes = Endereco.Split(new string[] { "-" }, StringSplitOptions.None);

                txtemitxLgr.Text = partes[0];
                txtemitnro.Text = partes[1];
                txtemitIE.Text = partes[10].Replace(".", "");
                txtemitxBairro.Text = partes[2];
                txtemitcMun.Text = partes[8];
                txtemitxMun.Text = partes[4];
                txtemitUF.Text = partes[5];
                txtemitcPais.Text = partes[9];
                txtemitxPais.Text = partes[7];
                txtemitCEP.Text = partes[6];
            }
        }

        //protected void SelecionarPedido_CheckedChanged(object sender, EventArgs e)
        //{
        //    string sPedido = "";
        //    string Item = "";
        //    try
        //    {
        //        string Pedido = "0";
        //        bool algumSelecionado = false;

        //        if (SelecionarPedido.Checked == true)
        //        {
        //            foreach (GridViewRow Row in dtgvConsulta.Rows)
        //            {
        //                DropDownList ddlPedidos = (Row.FindControl("ddlPedidos") as DropDownList);

        //                if (ddlPedidos.SelectedValue != "0")
        //                {
        //                    Pedido = ddlPedidos.SelectedValue;
        //                    sPedido = ddlPedidos.SelectedItem.Text;
        //                    algumSelecionado = true;
        //                }
        //            }
        //            if (algumSelecionado)
        //            {
        //                try
        //                {
        //                    foreach (GridViewRow Row in dtgvConsulta.Rows)
        //                    {
        //                        DropDownList ddlPedidos = (Row.FindControl("ddlPedidos") as DropDownList);
        //                        DropDownList ddlItem = (Row.FindControl("ddlItemAdd") as DropDownList);
        //                        Item = (Row.FindControl("cProd") as Label).Text;
        //                        string xProd = (Row.FindControl("xProd") as Label).Text;
        //                        string[] partes = xProd.Split(new string[] { " - " }, StringSplitOptions.None);
        //                        ddlPedidos.SelectedValue = Pedido;

        //                        if (ddlItem.SelectedValue == "")
        //                        {
        //                            FUNCOES.Popula_Combo(ddlItem, "sp_Select 'ProdutosPedidos_Flow', @sPesquisa=" + "'" + ddlPedidos.SelectedValue + "'", "sCodigo", "sCodProd", false, "Selecione o Produto", "0");
        //                            try
        //                            {
        //                                ddlItem.SelectedValue = Item;
        //                            }
        //                            catch
        //                            {

        //                            }
        //                            try
        //                            {
        //                                ddlItem.SelectedValue = partes[0];
        //                            }
        //                            catch
        //                            {

        //                            }
        //                        }
        //                    }
        //                }
        //                catch
        //                {
        //                    SelecionarPedido.Checked = false;
        //                    MensagemPagina1.MostraMensagem_Erro("O item '" + Item + "' não esta cadastrado no pedido '" + sPedido + "'");
        //                }
        //            }
        //            else
        //            {
        //                SelecionarPedido.Checked = false;
        //                MensagemPagina1.MostraMensagem_Erro("Selecione um Pedido!");
        //            }
        //        }
        //        else
        //        {
        //            foreach (GridViewRow Row in dtgvConsulta.Rows)
        //            {
        //                DropDownList ddlPedidos = (Row.FindControl("ddlPedidos") as DropDownList);
        //                ddlPedidos.SelectedValue = "0";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MensagemPagina1.MostraMensagem_Erro(ex.Message);
        //    }
        //}
        void Carregar_ItensPedido(string idPedido, GridViewRow row)
        {
            bool bLocalizado = false;
            DropDownList Produtos_ddlidItem = (row.FindControl("Produtos_ddlidItem") as DropDownList);
            DropDownList ddlPedidos = (row.FindControl("ddlPedidos") as DropDownList);

            string cProd = row.Cells[2].Text.Replace("-", "");
            string xProd = row.Cells[2].Text.Trim();
            string[] vPesquisa = cProd.Split(new string[] { " " }, StringSplitOptions.None);


            Produtos_ddlidItem.Items.Clear();
            if (bs_Itens_Pedido_Compra.Count == 0 || bs_Itens_Pedido_Compra[0].idPedido.ToString() != idPedido)
            {
                SqlDataReader dr;
                dr = BD.ExecutarDataReader("sp_Select @sTabela='ProdutosPedidos_Flow',   @idPesquisa = " + idPedido);

                if (dr != null)
                {
                    bs_Itens_Pedido_Compra.Clear();
                    while (dr.Read())
                    {
                        FrameWork.cls_WMS_Produtos objItem = new FrameWork.cls_WMS_Produtos();
                        objItem.IdItem = Convert.ToInt32(dr["idItem"].ToString());
                        objItem.SCodigo = dr["sCodigo"].ToString();
                        objItem.SDscProduto = dr["sDscProduto"].ToString();
                        objItem.idPedido = Convert.ToInt32(dr["idPedido"].ToString());
                        bs_Itens_Pedido_Compra.Add(objItem);
                    }
                }
                dr.Close();
            }

            //adicionar Combo

            Produtos_ddlidItem.Items.Add(new ListItem("Selecione o Item do Cadastro", ""));

            foreach (var linha in bs_Itens_Pedido_Compra)
            {
                Produtos_ddlidItem.Items.Add(new ListItem(linha.SDscProduto, linha.SCodigo));
            }


            foreach (ListItem lst in Produtos_ddlidItem.Items)
            {

                if (lst.Value.Replace("-", "") == vPesquisa[0].Replace("-", ""))
                {
                    lst.Selected = true;
                    bLocalizado = true;
                }
            }


            if (!bLocalizado)
            {
                //Tenta achar o produto que tenha descrição igual ao produto do pedido

                foreach (ListItem lst in Produtos_ddlidItem.Items)
                {


                    string sDscProduto_Comex = Funcoes.RemoverAcentos(lst.Text.Replace("  ", " ").Replace("\"", "").ToUpper().Trim());
                    if (sDscProduto_Comex.Length > 120)
                        sDscProduto_Comex = sDscProduto_Comex.Substring(0, 120);


                    Debug.Print("P:" + sDscProduto_Comex + "-");
                    Debug.Print("X:" + xProd.ToUpper() + "-");



                    if (sDscProduto_Comex == xProd.ToUpper())
                    {
                        lst.Selected = true;
                        bLocalizado = true;
                    }
                }



            }

            if (!bLocalizado)
            {
                //Tenta achar o produto com o cadatro do fornecedor

                row.CssClass = "Amarelo";
            }


        }

        protected void ddlPedidos_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            GridViewRow row_atual = (GridViewRow)ddl.NamingContainer;
            DropDownList Produtos_ddlidItem_ = (row_atual.FindControl("Produtos_ddlidItem") as DropDownList);
            DropDownList ddlPedidos_ = (row_atual.FindControl("ddlPedidos") as DropDownList);
            string idPedido = ddlPedidos_.SelectedValue;


            if (idPedido != "0" )
            {
                Carregar_ItensPedido(idPedido, row_atual); 
            }
    }

        //protected void ddlItemAdd_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    foreach (GridViewRow row in dtgvConsulta.Rows)
        //    {
        //        if (row.RowType == DataControlRowType.DataRow)
        //        {
        //            TableCell itemCell = row.Cells[3];
        //            DropDownList ddlItem = (row.FindControl("ddlItemAdd") as DropDownList);
        //            DropDownList ddlItemFornecedor = (row.FindControl("ddlItemFornecedor") as DropDownList);
        //            DropDownList ddlPedidos = (row.FindControl("ddlPedidos") as DropDownList);
        //            if (ddlPedidos.SelectedValue != "0")
        //            {
        //                if (ddlItem.SelectedValue == "0" && ddlItemFornecedor.SelectedValue == "0")
        //                {
        //                    itemCell.CssClass = "Amarelo";
        //                }
        //                else
        //                {
        //                    itemCell.CssClass = "";
        //                }
        //            }
        //        }
        //    }
        //}

        private void ExcluirImportador()
        {
            DataSet dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "Excluir_Importador");
            vParametros.Add("@idImportador", hddidImportador.Value);
            vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
            dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ImportadorNfe", vParametros);

            FUNCOES.DirecionaPagina("app/Paginas/Adm/Faturamento/ImportadorNFE.aspx?msg=1");
        }

        #endregion


        #region | Imposto
        protected void dtgvConsulta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Editar")
            {
                int index = Convert.ToInt32(e.CommandArgument) - 1;
                nItem = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = dtgvConsulta.Rows[index];
                idItens = int.Parse(dtgvConsulta.DataKeys[row.RowIndex].Value.ToString());
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModal", "$('#modalImposto').modal('show');", true);

                foreach (var itens in Base_Importador_Itens)
                {
                    if (itens.idItens == idItens)
                    {
                        lblModalImposto.Text = itens.cProd + " - " + itens.xProd;
                        //ICMS
                        if (itens.orig != "")
                            txtorigICMS.Text = itens.orig;
                        else
                            DIV18.Visible = false;

                        if (itens.CST_ICMS != "")
                            txtCST.Text = itens.CST_ICMS;
                        else
                            DIV22.Visible = false;

                        if (itens.modBC != "")
                            txtmodBC.Text = itens.modBC;
                        else
                            DIV24.Visible = false;

                        if (itens.vBC_ICMS != "")
                            txtvBC.Text = itens.vBC_ICMS;
                        else
                            DIV25.Visible = false;

                        if (itens.pRedBC != "")
                            txtpRedBC.Text = itens.pRedBC;
                        else
                            DIV26.Visible = false;

                        if (itens.pICMS != "")
                            txtpICMS.Text = itens.pICMS;
                        else
                            DIV28.Visible = false;

                        if (itens.vICMS != "")
                            txtvICMS.Text = itens.vICMS;
                        else
                            DIV29.Visible = false;

                        //IPI
                        if (itens.cEnq != "")
                            txtcEnq.Text = itens.cEnq;
                        else
                            DIV33.Visible = false;

                        if (itens.CST_IPI != "")
                            txtCSTIPI.Text = itens.CST_IPI;
                        else
                            DIV34.Visible = false;

                        if (itens.vBC_IPI != "")
                            txtvBCIPI.Text = itens.vBC_IPI;
                        else
                            DIV35.Visible = false;

                        if (itens.pIPI != "")
                            txtpIPI.Text = itens.pIPI;
                        else
                            DIV36.Visible = false;

                        if (itens.vIPI != "")
                            txtvIPI.Text = itens.vIPI;
                        else
                            DIV37.Visible = false;
                        //II
                        if (itens.vBC_II != "")
                            txtvBCII.Text = itens.vBC_II;
                        else
                            DIV43.Visible = false;

                        if (itens.vDespAdu != "")
                            txtvDespAdu.Text = itens.vDespAdu;
                        else
                            DIV44.Visible = false;

                        if (itens.vII != "")
                            txtvII.Text = itens.vII;
                        else
                            DIV45.Visible = false;

                        if (itens.vIOF != "")
                            txtvIOF.Text = itens.vIOF;
                        else
                            DIV46.Visible = false;

                        if (DIV43.Visible == false && DIV44.Visible == false && DIV45.Visible == false && DIV46.Visible == false)
                            div40.Visible = false;
                        //PIS
                        if (itens.CST_PIS != "")
                            txtCSTPIS.Text = itens.CST_PIS;
                        else
                            DIV48.Visible = false;

                        if (itens.vBC_PIS != "")
                            txtvBCPIS.Text = itens.vBC_PIS;
                        else
                            DIV49.Visible = false;

                        if (itens.pPIS != "")
                            txtpPIS.Text = itens.pPIS;
                        else
                            DIV50.Visible = false;

                        if (itens.vPIS != "")
                            txtvPIS.Text = itens.vPIS;
                        else
                            DIV51.Visible = false;
                        //COFINS
                        if (itens.CST_COFINS != "")
                            txtCSTCOFINS.Text = itens.CST_COFINS;
                        else
                            DIV56.Visible = false;

                        if (itens.vBC_COFINS != "")
                            txtvBCCOFINS.Text = itens.vBC_COFINS;
                        else
                            DIV57.Visible = false;

                        if (itens.pCOFINS != "")
                            txtpCOFINS.Text = itens.pCOFINS;
                        else
                            DIV58.Visible = false;

                        if (itens.vCOFINS != "")
                            txtvCOFINS.Text = itens.vCOFINS;
                        else
                            DIV59.Visible = false;
                    }
                }
                updModal.Update();
            }
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

                foreach (var row in xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}det"))
                {
                    if (int.Parse(row.Attribute("nItem")?.Value) == nItem)
                    {
                        //ICMS
                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}orig").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}orig").FirstOrDefault().Value = txtorigICMS.Text.Replace(",", ".");


                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault().Value = txtCST.Text.Replace(",", ".");


                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}modBC").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}modBC").FirstOrDefault().Value = txtmodBC.Text.Replace(",", ".");


                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                        {
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault().Value = txtvBC.Text.Replace(",", ".");
                        }

                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}pRedBC").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}pRedBC").FirstOrDefault().Value = txtpRedBC.Text.Replace(",", ".");


                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}pICMS").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}pICMS").FirstOrDefault().Value = txtpICMS.Text.Replace(",", ".");


                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault() != null)
                        {
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault().Value = txtvICMS.Text.Replace(",", ".");
                        }

                        ////IPI
                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}cEnq").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}cEnq").FirstOrDefault().Value = txtcEnq.Text.Replace(",", ".");

                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault().Value = txtCSTIPI.Text.Replace(",", ".");

                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault().Value = txtvBCIPI.Text.Replace(",", ".");

                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}pIPI").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}pIPI").FirstOrDefault().Value = txtpIPI.Text.Replace(",", ".");

                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault().Value = txtvIPI.Text.Replace(",", ".");

                        ////II
                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault().Value = txtvBCII.Text.Replace(",", ".");

                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vDespAdu").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vDespAdu").FirstOrDefault().Value = txtvDespAdu.Text.Replace(",", ".");

                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault().Value = txtvII.Text.Replace(",", ".");

                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vIOF").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vIOF").FirstOrDefault().Value = txtvIOF.Text.Replace(",", ".");

                        ////PIS
                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault().Value = txtCSTPIS.Text.Replace(",", ".");

                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault().Value = txtvBCPIS.Text.Replace(",", ".");

                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}pPIS").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}pPIS").FirstOrDefault().Value = txtpPIS.Text.Replace(",", ".");

                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault().Value = txtvPIS.Text.Replace(",", ".");

                        ////COFINS
                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}CST").FirstOrDefault().Value = txtCSTCOFINS.Text.Replace(",", ".");

                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault().Value = txtvBCCOFINS.Text.Replace(",", ".");

                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}pCOFINS").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}pCOFINS").FirstOrDefault().Value = txtpCOFINS.Text.Replace(",", ".");

                        if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault() != null)
                            row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault().Value = txtvCOFINS.Text.Replace(",", ".");
                    }

                    if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault() != null)
                    {
                        vBC_ICMSTOTAL += decimal.Parse(row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}vBC").FirstOrDefault().Value.Replace(".", ","));
                    }
                    if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault() != null)
                    {
                        vICMS_TOTAL += decimal.Parse(row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}ICMS").Descendants("{http://www.portalfiscal.inf.br/nfe}vICMS").FirstOrDefault().Value.Replace(".", ","));
                    }
                    if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault() != null)
                    {
                        vII_TOTAL += decimal.Parse(row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}II").Descendants("{http://www.portalfiscal.inf.br/nfe}vII").FirstOrDefault().Value.Replace(".", ","));
                    }
                    if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault() != null)
                    {
                        vIPI_TOTAL += decimal.Parse(row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}IPI").Descendants("{http://www.portalfiscal.inf.br/nfe}vIPI").FirstOrDefault().Value.Replace(".", ","));
                    }
                    if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault() != null)
                    {
                        vPIS_TOTAL += decimal.Parse(row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}PIS").Descendants("{http://www.portalfiscal.inf.br/nfe}vPIS").FirstOrDefault().Value.Replace(".", ","));
                    }
                    if (row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault() != null)
                    {
                        vCOFINS_TOTAL += decimal.Parse(row.Descendants("{http://www.portalfiscal.inf.br/nfe}imposto").Descendants("{http://www.portalfiscal.inf.br/nfe}COFINS").Descendants("{http://www.portalfiscal.inf.br/nfe}vCOFINS").FirstOrDefault().Value.Replace(".", ","));
                    }
                }

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
                        //ICMS
                        if (DIV18.Visible != false)
                            vParametrosItens.Add("@orig", txtorigICMS.Text.Replace(",", "."));
                        if (DIV22.Visible != false)
                            vParametrosItens.Add("@CST_ICMS", txtCST.Text.Replace(",", "."));
                        if (DIV24.Visible != false)
                            vParametrosItens.Add("@modBC", txtmodBC.Text.Replace(",", "."));
                        if (DIV25.Visible != false)
                            vParametrosItens.Add("@vBC_ICMS", txtvBC.Text.Replace(",", "."));
                        if (DIV26.Visible != false)
                            vParametrosItens.Add("@pRedBC", txtpRedBC.Text.Replace(",", "."));
                        if (DIV28.Visible != false)
                            vParametrosItens.Add("@pICMS", txtpICMS.Text.Replace(",", "."));
                        if (DIV29.Visible != false)
                            vParametrosItens.Add("@vICMS", txtvICMS.Text.Replace(",", "."));
                        //IPI
                        if (DIV33.Visible != false)
                            vParametrosItens.Add("@cEnq", txtcEnq.Text.Replace(",", "."));
                        if (DIV34.Visible != false)
                            vParametrosItens.Add("@CST_IPI", txtCSTIPI.Text.Replace(",", "."));
                        if (DIV35.Visible != false)
                            vParametrosItens.Add("@vBC_IPI", txtvBCIPI.Text.Replace(",", "."));
                        if (DIV36.Visible != false)
                            vParametrosItens.Add("@pIPI", txtpIPI.Text.Replace(",", "."));
                        if (DIV37.Visible != false)
                            vParametrosItens.Add("@vIPI", txtvIPI.Text.Replace(",", "."));
                        //II
                        if (DIV42.Visible != false)
                            vParametrosItens.Add("@vBC_II", txtvBCII.Text.Replace(",", "."));
                        if (DIV44.Visible != false)
                            vParametrosItens.Add("@vDespAdu", txtvDespAdu.Text.Replace(",", "."));
                        if (DIV45.Visible != false)
                            vParametrosItens.Add("@vII", txtvII.Text.Replace(",", "."));
                        if (DIV46.Visible != false)
                            vParametrosItens.Add("@vIOF", txtvIOF.Text.Replace(",", "."));
                        //PIS
                        if (DIV48.Visible != false)
                            vParametrosItens.Add("@CST_PIS", txtCSTPIS.Text.Replace(",", "."));
                        if (DIV49.Visible != false)
                            vParametrosItens.Add("@vBC_PIS", txtvBCPIS.Text.Replace(",", "."));
                        if (DIV50.Visible != false)
                            vParametrosItens.Add("@pPIS", txtpPIS.Text.Replace(",", "."));
                        if (DIV51.Visible != false)
                            vParametrosItens.Add("@vPIS", txtvPIS.Text.Replace(",", "."));
                        //COFINS
                        if (DIV56.Visible != false)
                            vParametrosItens.Add("@CST_COFINS", txtCSTCOFINS.Text.Replace(",", "."));
                        if (DIV57.Visible != false)
                            vParametrosItens.Add("@vBC_COFINS", txtvBCCOFINS.Text.Replace(",", "."));
                        if (DIV58.Visible != false)
                            vParametrosItens.Add("@pCOFINS", txtpCOFINS.Text.Replace(",", "."));
                        if (DIV59.Visible != false)
                            vParametrosItens.Add("@vCOFINS", txtvCOFINS.Text.Replace(",", "."));

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
                            MensagemPaginaUnitizados.MostraMensagem_Erro(ex.Message);
                        }
                    }
                    else
                    {
                        MensagemPaginaUnitizados.MostraMensagem_Erro("Erro: O arquivo não foi convertido corretamente em bytes.");
                    }
                }
                catch (Exception ex)
                {
                    MensagemPaginaUnitizados.MostraMensagem_Erro(string.Format("Erro ao ler o arquivo: {0}", ex.Message));
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
                MensagemPaginaUnitizados.MostraMensagem_Erro(ex.Message);
            }
            updModal.Update();
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

        protected void ddlidTranportadora_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidTranportadora.SelectedValue != "0")
            {
                try
                {
                    string sErro = "";
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_Transportadora");
                    vParametros.Add("@idCliente", ddlidTranportadora.SelectedValue);
                    dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametros);
                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        div_InfoTransportadora.Visible = true;
                        txtCNPJTrans.Text = RETORNO.DATASET(dsPesquisa, 0, "sCPF_CNPJ");
                        txtNomeTrans.Text = RETORNO.DATASET(dsPesquisa, 0, "sRazaoSocial");
                        txtIETrans.Text = RETORNO.DATASET(dsPesquisa, 0, "sRG_IE");

                        txtEnderecoTrans.Text = RETORNO.DATASET(dsPesquisa, 0, "sLogradouro");
                        txtMunicipiotrans.Text = RETORNO.DATASET(dsPesquisa, 0, "sCidade");
                        txtUFTrans.Text = RETORNO.DATASET(dsPesquisa, 0, "sEstado");
                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Erro("A transportadora selecionada não possui um endereço principal cadastrado.");
                        ddlidTranportadora.SelectedValue = "0";
                        txtCNPJTrans.Text = "";
                        txtNomeTrans.Text = "";
                        txtIETrans.Text = "";
                        txtEnderecoTrans.Text = "";
                        txtMunicipiotrans.Text = "";
                        txtUFTrans.Text = "";
                        div_InfoTransportadora.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
                }
            }
            else
            {
                div_InfoTransportadora.Visible = false;
                txtCNPJTrans.Text = "";
                txtNomeTrans.Text = "";
                txtIETrans.Text = "";
                txtEnderecoTrans.Text = "";
                txtMunicipiotrans.Text = "";
                txtUFTrans.Text = "";
            }
        }

        protected void ddlPedidos_Geral_SelectedIndexChanged(object sender, EventArgs e)
        {

            //if (ddlPedidos_Geral.SelectedValue != "0")
            {
                foreach (GridViewRow Row in dtgvConsulta.Rows)
                {
                    DropDownList ddlPedidos = (Row.FindControl("ddlPedidos") as DropDownList);
                    ddlPedidos.SelectedValue = ddlPedidos_Geral.SelectedValue;
                    Carregar_ItensPedido(ddlPedidos.SelectedValue, Row); // Produtos_ddlidItem, vPesquisa[0].Replace("-", ""));


                }
            }
        }

        #region | Aba Documentos Fiscais
        protected void gvDocumentosFiscais_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                LinkButton DocumentosFiscais_cmdGerarNFe = e.Row.FindControl("DocumentosFiscais_cmdGerarNFe") as LinkButton;
                LinkButton DocumentosFiscais_cmdDownloadDANFE = e.Row.FindControl("DocumentosFiscais_cmdDownloadDANFE") as LinkButton;
                LinkButton DocumentosFiscais_cmdDownloadXML = e.Row.FindControl("DocumentosFiscais_cmdDownloadXML") as LinkButton;
                LinkButton DocumentosFiscais_cmdCartaCorrecao = e.Row.FindControl("DocumentosFiscais_cmdCartaCorrecao") as LinkButton;
                LinkButton DocumentosFiscais_cmdDownloadCCePDF = e.Row.FindControl("DocumentosFiscais_cmdDownloadCCePDF") as LinkButton;
                LinkButton DocumentosFiscais_cmdDownloadCCeXML = e.Row.FindControl("DocumentosFiscais_cmdDownloadCCeXML") as LinkButton;
                LinkButton DocumentosFiscais_cmdCancelarNFE = e.Row.FindControl("DocumentosFiscais_cmdCancelarNFE") as LinkButton;

                DocumentosFiscais_cmdGerarNFe.Visible = false;
                DocumentosFiscais_cmdDownloadDANFE.Visible = false;
                DocumentosFiscais_cmdDownloadXML.Visible = false;

                DocumentosFiscais_cmdCartaCorrecao.Visible = false;
                DocumentosFiscais_cmdDownloadCCePDF.Visible = false;
                DocumentosFiscais_cmdDownloadCCeXML.Visible = false;
                DocumentosFiscais_cmdCancelarNFE.Visible = false;


                switch (e.Row.Cells[nColuna_Status].Text)
                {
                    case "Espelho":
                        DocumentosFiscais_cmdGerarNFe.Visible = true;
                        DocumentosFiscais_cmdCancelarNFE.Visible = true;
                        DocumentosFiscais_cmdDownloadXML.Visible = true;
                        DocumentosFiscais_cmdDownloadDANFE.Visible = true;
                        DocumentosFiscais_cmdDownloadDANFE.Text = "Espelho";
                        DocumentosFiscais_cmdCancelarNFE.Text = "Excluir";
                        break;

                    case "Autorizada":

                        if (e.Row.Cells[nColuna_sTipo].Text == "NF-e")
                        {
                            DocumentosFiscais_cmdDownloadXML.Visible = true;
                            DocumentosFiscais_cmdDownloadDANFE.Visible = true;
                            if (e.Row.Cells[nColuna_Cancelamento].Text == "N")
                            {
                                //DocumentosFiscais_cmdEnviarEmail.Visible = true;
                                DocumentosFiscais_cmdCartaCorrecao.Visible = true;
                                //DocumentosFiscais_cmdDevolucao.Visible = true;
                                DocumentosFiscais_cmdCancelarNFE.Visible = true;
                            }
                        }

                        else if (e.Row.Cells[nColuna_sTipo].Text == "CC-e")
                        {
                            DocumentosFiscais_cmdDownloadCCePDF.Visible = true;
                            DocumentosFiscais_cmdDownloadCCeXML.Visible = true;
                        }
                        else if (e.Row.Cells[nColuna_sTipo].Text == "NFD-e")
                        {
                            DocumentosFiscais_cmdDownloadXML.Visible = true;
                            DocumentosFiscais_cmdDownloadDANFE.Visible = true;
                            if (e.Row.Cells[nColuna_Cancelamento].Text == "N")
                            {
                                DocumentosFiscais_cmdCancelarNFE.Visible = true;
                                DocumentosFiscais_cmdCartaCorrecao.Visible = true;

                            }

                        }
                        break;

                    case "Em processamento":
                        if (e.Row.Cells[nColuna_sTipo].Text == "NF-e")
                        {
                            DocumentosFiscais_cmdDownloadXML.Visible = true;
                        }
                        break;

                    case "Rejeitada":
                        DocumentosFiscais_cmdDownloadXML.Visible = true;
                        DocumentosFiscais_cmdCancelarNFE.Text = "Excluir";
                        DocumentosFiscais_cmdCancelarNFE.Visible = true;
                        break;

                    case "Aguardando DANFE":
                        DocumentosFiscais_cmdDownloadXML.Visible = true;
                        DocumentosFiscais_cmdCancelarNFE.Visible = true;
                        break;

                    case "Cancelada":
                        DocumentosFiscais_cmdDownloadXML.Visible = true;
                        DocumentosFiscais_cmdDownloadDANFE.Visible = true;
                        break;
                }

                //Fazer Validação de DATA para Cancelamento, carta de correção e Devolução
            }

            GRID.EsconderColunas(e, nColuna_idObjeto, nColuna_nNumeroCartaCorrecao, nColuna_Cancelamento, nColuna_sChaveNFe);


        }


        protected void DocumentosFiscais_cmdGerarNFe_Click(object sender, EventArgs e)
        {
            var row = (sender as LinkButton).NamingContainer as GridViewRow;
            hddidXML.Value = row.Cells[nColuna_idXML].Text;
            div_Modal.Style.Add("width", "35%");
            hddsAcaoNFe.Value = "ENVIAR_SEFAZ";
            txtsJustificativa_AcoesNFe.Visible = false;
            lblTitulos_AcoesNFe.Text = "Envio para SEFAZ";
            lblTituloJustificativa_AcoesNFe.Text = "Confirma o envio da NF-e para o SEFAZ?";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Confirma_SEFAZ", "$('#Modal_AcoesNFe').modal('show');", true);
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
        }
        protected void DocumentosFiscais_cmdCancelarNFE_Click(object sender, EventArgs e)
        {
            var row = (sender as LinkButton).NamingContainer as GridViewRow;
            hddidXML.Value = row.Cells[nColuna_idXML].Text;
            txtsJustificativa_AcoesNFe.Visible = true;

            if (row.Cells[nColuna_Status].Text == "Espelho" || row.Cells[nColuna_Status].Text == "Rejeitada")
            {
                lblTitulos_AcoesNFe.Text = "Excluir NF Espelho";
                txtsJustificativa_AcoesNFe.Visible = false;
                div_Modal.Style.Add("width", "35%");
                lblTituloJustificativa_AcoesNFe.Text = "Confirma a exclusão do Espelho?";
                hddsAcaoNFe.Value = "EXCLUIR_ESPELHO";
            }
            else if (row.Cells[nColuna_Status].Text == "Autorizada")
            {
                div_Modal.Style.Add("width", "60%");
                lblTitulos_AcoesNFe.Text = "Cancelar NF-e";
                lblTituloJustificativa_AcoesNFe.Text = "Informe a justificativa para cancelamento";
                hddsAcaoNFe.Value = "CANCELAR_NFe";
                txtsJustificativa_AcoesNFe.Focus();
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModalAcoes", "$('#Modal_AcoesNFe').modal('show');", true);
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");

        }
        protected void DocumentosFiscais_cmdCartaCorrecao_Click(object sender, EventArgs e)
        {
            var row = (sender as LinkButton).NamingContainer as GridViewRow;
            hddidXML.Value = row.Cells[nColuna_idXML].Text;

            txtsJustificativa_AcoesNFe.Visible = true;
            div_Modal.Style.Add("width", "60%");
            lblTitulos_AcoesNFe.Text = "Carta de Correção NFe";
            lblTituloJustificativa_AcoesNFe.Text = "Informe o texto de correção";
            hddsAcaoNFe.Value = "CCe";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenCCe", "$('#Modal_AcoesNFe').modal('show');", true);
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
            txtsJustificativa_AcoesNFe.Focus();
        }

        protected void cmdConfirmar_AcoesNFe_Click(object sender, EventArgs e)
        {
            string sRetorno = "";

            if (txtsJustificativa_AcoesNFe.Visible)
            {
                if (txtsJustificativa_AcoesNFe.Text == "" && txtsJustificativa_AcoesNFe.Text.Length < 15)
                    sRetorno = "Informe uma justificativa válida!</br> Texto maior que 15 caracteres.";
            }

            if (sRetorno == "")
            {
                switch (hddsAcaoNFe.Value)
                {
                    case "SALVAR":
//                        Salvar_Detalhe();
                        break;

                    case "EXCLUIR_ESPELHO":
                        sRetorno = Funcoes_NFe.XML.Excluir(idOrigem_Emissor.ToString(), hddidXML.Value);
                        if (sRetorno == "")
                        {
                            Pesquisar(hddidImportador.Value, false);
                            MensagemPagina.MostraMensagem_Sucesso("Espelho excluido com sucesso!");
                        }
                        break;

					case "ENVIAR_SEFAZ":
                        string nNF = "";
                        sRetorno = Funcoes_NFe.XML.EnviarXML_SEFAZ(hddidXML.Value, out nNF);
                        Pesquisar(hddidImportador.Value, false);
                        MensagemPagina.MostraMensagem_Sucesso(string.Format("Nota Fiscal n.º {0}, Gerada com sucesso!", nNF.PadLeft(6, '0')) );
                        FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
                        break;

                    case "CCe":
                        sRetorno = Funcoes_CCe.EnviarXML_SEFAZ(hddidXML.Value, txtsJustificativa_AcoesNFe.Text);
                        Pesquisar(hddidImportador.Value, false);
                        MensagemPagina.MostraMensagem_Sucesso("Nota Fiscal Gerada com sucesso!");
                        FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
                        break;

                    case "CANCELAR_NFe":
                        sRetorno = Funcoes_NFe.XML.SEFAZ_Cancelar(hddidXML.Value, txtsJustificativa_AcoesNFe.Text);
                        if (sRetorno == "")
                        {
                            Pesquisar(hddidImportador.Value, false);
                            MensagemPagina.MostraMensagem_Sucesso("Cancelamento efetuado!");
                            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
                        }
                        break;
                }

            }


            if (sRetorno != "")
            {
                MensagemPaginaAcoesNFe.MostraMensagem_Erro(sRetorno);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenCCe", "$('#Modal_AcoesNFe').modal('show');", true);
                FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
            }


        }


        protected void DocumentosFiscais_cmdDownloadXML_Click(object sender, EventArgs e)
        {
            var row = (sender as LinkButton).NamingContainer as GridViewRow;
            Funcoes_NFe.Download.XML(Page, row.Cells[nColuna_idXML].Text);
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
        }

        protected void DocumentosFiscais_cmdDownloadDANFE_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (sender as LinkButton).NamingContainer as GridViewRow;
                Funcoes_NFe.Download.PDF(Page, row.Cells[nColuna_idXML].Text);
                FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
            }
            catch (Exception ex)
            {

                MensagemPagina.MostraMensagem_Erro("Erro ao Gerar PDF: " + ex.Message);
            }

        }


        protected void DocumentosFiscais_cmdDownloadCCeXML_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (sender as LinkButton).NamingContainer as GridViewRow;
                Funcoes_CCe.Download_XML(Page, row.Cells[nColuna_sChaveNFe].Text, row.Cells[nColuna_nNumeroCartaCorrecao].Text);
                FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        protected void DocumentosFiscais_cmdDownloadCCePDF_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (sender as LinkButton).NamingContainer as GridViewRow;
                Funcoes_CCe.Download_PDF(Page, row.Cells[nColuna_sChaveNFe].Text, row.Cells[nColuna_nNumeroCartaCorrecao].Text);
                FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }


        #endregion
        
    }
}