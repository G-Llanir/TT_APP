using Microsoft.Reporting.WebForms;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using TT_Flow.App.Paginas.Adm.Financeiro;
using TT_Flow.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Paginas.WMS
{
    public partial class OPI_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Ordem de Produção Interna";
        string sProcedure = "sp_Manipula_FLow_WMS_OPI";
        string sProcedureEtiqueta = "sp_Manipula_tbl_Flow_WMS_OPI_Etiqueta";
        string sProcedureLocal = "sp_Manipula_tbl_Flow_WMS_LocalArmazenamento";
        bool isInclusao = false;
        string idImpressoraPadrao = "1";

        #region | Classes 

        //Volumes
        public EntidadeFuncoes<cls_WMS_VolumesItens> conversorVolumes = new EntidadeFuncoes<cls_WMS_VolumesItens>();
        public List<FrameWork.cls_WMS_VolumesItens> ls_VolumesItens
        {
            get
            {
                if (ViewState["ls_VolumesItens"] == null)
                {
                    ViewState["ls_VolumesItens"] = new List<cls_WMS_VolumesItens>();
                }
                return (List<cls_WMS_VolumesItens>)ViewState["ls_VolumesItens"];
            }
            set
            {
                ViewState["ls_VolumesItens"] = value;
            }
        }
        public List<FrameWork.cls_WMS_VolumesItens> ls_VolumesItensGV
        {
            get
            {
                if (ViewState["ls_VolumesItensGV"] == null)
                {
                    ViewState["ls_VolumesItensGV"] = new List<cls_WMS_VolumesItens>();
                }
                return (List<cls_WMS_VolumesItens>)ViewState["ls_VolumesItensGV"];
            }
            set
            {
                ViewState["ls_VolumesItensGV"] = value;
            }
        }
        //Produtos
        public EntidadeFuncoes<cls_WMS_Produtos> conversorProdutos = new EntidadeFuncoes<cls_WMS_Produtos>();
        public List<FrameWork.cls_WMS_Produtos> bs_Produto_Envios
        {
            get
            {
                if (ViewState["bs_Produto_Envios"] == null)
                {
                    ViewState["bs_Produto_Envios"] = new List<FrameWork.cls_WMS_Produtos>();
                }
                return (List<FrameWork.cls_WMS_Produtos>)ViewState["bs_Produto_Envios"];
            }
            set
            {
                ViewState["bs_Produto_Envios"] = value;
            }
        }
        public List<FrameWork.cls_WMS_Produtos> ls_unitizadosItens
        {
            get
            {
                if (ViewState["ls_unitizadosItens"] == null)
                {
                    ViewState["ls_unitizadosItens"] = new List<FrameWork.cls_WMS_Produtos>();
                }
                return (List<FrameWork.cls_WMS_Produtos>)ViewState["ls_unitizadosItens"];
            }
            set
            {
                ViewState["ls_unitizadosItens"] = value;
            }
        }
        public List<FrameWork.cls_WMS_Produtos> ls_unitizadosItensGV
        {
            get
            {
                if (ViewState["ls_unitizadosItensGV"] == null)
                {
                    ViewState["ls_unitizadosItensGV"] = new List<FrameWork.cls_WMS_Produtos>();
                }
                return (List<FrameWork.cls_WMS_Produtos>)ViewState["ls_unitizadosItensGV"];
            }
            set
            {
                ViewState["ls_unitizadosItensGV"] = value;
            }
        }
        public List<FrameWork.cls_WMS_Produtos> ls_VolumesProdutosGV
        {
            get
            {
                if (ViewState["ls_VolumesProdutosGV"] == null)
                {
                    ViewState["ls_VolumesProdutosGV"] = new List<FrameWork.cls_WMS_Produtos>();
                }
                return (List<FrameWork.cls_WMS_Produtos>)ViewState["ls_VolumesProdutosGV"];
            }
            set
            {
                ViewState["ls_VolumesProdutosGV"] = value;
            }
        }
        public List<cls_WMS_Produtos> ls_EtiquetaItens
        {
            get
            {
                if (ViewState["ls_EtiquetaItens"] == null)
                {
                    ViewState["ls_EtiquetaItens"] = new List<FrameWork.cls_WMS_Produtos>();
                }
                return (List<FrameWork.cls_WMS_Produtos>)ViewState["ls_EtiquetaItens"];
            }
            set
            {
                ViewState["ls_EtiquetaItens"] = value;
            }
        }

        //Etiquetas
        public EntidadeFuncoes<cls_WMS_Etiquetas> conversorEtiqueta = new EntidadeFuncoes<cls_WMS_Etiquetas>();

        public List<cls_WMS_Etiquetas> ls_Etiquetas
        {
            get
            {
                if (ViewState["ls_Etiquetas"] == null)
                {
                    ViewState["ls_Etiquetas"] = new List<cls_WMS_Etiquetas>();
                }
                return (List<cls_WMS_Etiquetas>)ViewState["ls_Etiquetas"];
            }
            set
            {
                ViewState["ls_Etiquetas"] = value;
            }
        }
        public List<cls_WMS_Etiquetas> ls_EtiquetasSeries
        {
            get
            {
                if (ViewState["ls_EtiquetasSeries"] == null)
                {
                    ViewState["ls_EtiquetasSeries"] = new List<cls_WMS_Etiquetas>();
                }
                return (List<cls_WMS_Etiquetas>)ViewState["ls_EtiquetasSeries"];
            }
            set
            {
                ViewState["ls_EtiquetasSeries"] = value;
            }
        }
        public List<string> selectedVolumesIds
        {
            get
            {
                if (ViewState["selectedVolumesIds"] == null)
                {
                    ViewState["selectedVolumesIds"] = new List<string>();
                }
                return (List<string>)ViewState["selectedVolumesIds"];
            }
            set
            {
                ViewState["selectedVolumesIds"] = value;
            }
        }
        #endregion

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            Div_FormEnvios.Visible = false;

            if (!IsPostBack)
            {
                hfScrollPosition.Value = "0";
                if (Request["id"] != null)
                {
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    Pesquisar("0", true);
                }

                if (Session["MensagemSucesso"] != null)
                {
                    MensagemPaginaAbaEnvios.MostraMensagem_Sucesso(Session["MensagemSucesso"].ToString());
                    Session.Remove("MensagemSucesso");
                }
                if (Session["MensagemExclusão"] != null)
                {
                    MensagemPaginaExclusao.MostraMensagem_Sucesso(Session["MensagemExclusão"].ToString());
                    Session.Remove("MensagemExclusão");
                }

                PopularEnviosItens();
                PopularCombos();
                PopularUnitizados();
                PopularVolumes();
                PopularItens(Request["id"]);

                if (isInclusao == false)
                {
                    PopularEtiquetas(hddidOPI.Value);
                    DivBipador.Visible = false;
                }
                RegistraScriptAvancarRetornar();
                Session["CameraAberta"] = false;
                DivBipadorVolume.Visible = false;
                Session["CameraAbertaVolume"] = false;
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                var requestArgs = this.Request["__EVENTARGUMENT"];

                if (requestTarget == "funcao_SAIR")
                {
                    FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                }
                else if (requestTarget == "funcao_Editar")
                {
                    Pesquisar(hddidOPI.Value, true);
                }

            }
            RegistraScript("");
            RegisterQuaggaLibrary();

            LeitorQuagga1.txtClient = txtsCodigoBarras.ClientID.ToString();
            LeitorQuagga1.click = cmdIncluirUnitizadoItem.ClientID.ToString();

            LeitorQuagga.txtClient = hddsCodigoBarras.ClientID.ToString();
            LeitorQuagga.click = cmdIncluirVolume.ClientID.ToString();

            RegistrarColapsoScript();
            RegistraScriptCamposPeso();
        }


        #endregion

        #region | Metodos Banco de Dados
        protected void Pesquisar(string idOPI, bool bEdicao)
        {
            string sErro = "";

            try
            {
                cmdSalvar.Visible = false;

                if (idOPI != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idOPI", idOPI);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidOPI.Value = RETORNO.DATASET(dsPesquisa, 0, "idOPI");
                        txtidOPI.Text = hddidOPI.Value;
                        txtnPedido.Text = RETORNO.DATASET(dsPesquisa, 0, "idPedido");
                        txtsCliente.Text = RETORNO.DATASET(dsPesquisa, 0, "sCliente");
                        txtsReferencia.Text = RETORNO.DATASET(dsPesquisa, 0, "sReferencia");
                        txtsDscOPI.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscOPI");
                        ddlStatus.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idStatus");

                        if (ddlStatus.SelectedValue == "6")
                        {
                            div_main.Visible = false;
                        }
                        else if (ddlStatus.SelectedValue == "4" || ddlStatus.SelectedValue == "5")
                        {
                            cmdSalvar.Visible = false;
                            ddlStatus.Attributes.Add("disabled", "disabled");
                        }
                        else
                        {
                            div_main.Visible = true;
                            cmdSalvar.Visible = false;
                            ddlStatus.Attributes.Add("disabled", "disabled");

                        }

                        if (RETORNO.DATASET(dsPesquisa, 0, "dtOPI").ToString() == "01/01/1900 00:00:00")
                        {
                            txtdtOPI.Text = null;
                        }
                        else
                        {
                            var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtOPI").ToString());
                            txtdtOPI.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');
                        }

                        PopularEnvios(hddidOPI.Value);

                        PainelAtualizacao.Visible = false;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));

                        lblTituloPagina.Text = string.Format("Ordem de Produção Interna {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscOPI"));
                        BreadCrumb.TitulodaPagina = string.Format("Ordem de Produção Interna  {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscOPI"));

                        lblTituloSalvar.Text = "Confirma a Alteração do Status da OPI ?";
                    }
                    else
                    {
                        div_main.Visible = false;
                        MostraMensagem_Personalizada(sErro);
                    }

                }
                else
                {
                    div_main.Visible = false;
                    MostraMensagem_Personalizada("Nenhum Registro Encontrado");
                }
                RegistraScript("");

            }
            catch (Exception ex)
            {
                MostraMensagem_Personalizada(ex.ToString());
            }

        }

        void MostraMensagem_Personalizada(string sErro)
        {
            MensagemPagina.MostraMensagem_Erro(sErro +
           " <a href='#' onclick='retornar();'>Retornar</a> | <a href='#' onclick='avancar();'>Avançar</a>");
        }

        void Salvar_OPI(string idStatus, string sExclusao)
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string[] vidOPI = hddidOPI.Value.Split(',');
                    string idOPI = vidOPI[0].ToString();

                    string[] vidResponsavel = hddidResponsavel.Value.Split(',');
                    string idResponsavel = vidResponsavel[0].ToString();


                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro
                    vParametros.Add("@sFuncao", "ALTERAR-STATUS");
                    txtidOPI.Text = idOPI;
                    vParametros.Add("@idOPI", txtidOPI.Text);
                    if (idStatus == "")
                    {
                        vParametros.Add("@idStatus", ddlStatus.SelectedValue);
                    }
                    else
                    {
                        vParametros.Add("@idStatus", idStatus);
                    }
                    vParametros.Add("@sExclusao", sExclusao);
                    vParametros.Add("@idUsuarioExecucao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idOPI = RETORNO.DATASET(dsSalvar, "idOPI");

                        Pesquisar(idOPI, false);
                        MensagemPagina.MostraMensagem_Sucesso("Status Alterado com sucesso!");
                        if (idStatus == "6")
                        {

                            Session["MensagemExclusão"] = "OPI Excluída com sucesso!";
                            Response.Redirect(Request.RawUrl);
                        }
                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Erro("BD: " + sErro.ToString());
                    }


                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }

            }
            RegistraScript("");
        }
        protected void cmdVerPedido_click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            var container = btn.NamingContainer;
            TextBox txtnPedido = (TextBox)container.FindControl("txtnPedido");

            if (txtnPedido != null)
            {
                string idPedido = txtnPedido.Text;
                string url = string.Format("App/Paginas/Pedidos_Detalhe.aspx?id={0}&sTp=2", idPedido);
                Funcoes.DirecionaPagina(url);
            }
        }
        protected void Excluir_Click(object sender, EventArgs e)
        {
            Salvar_OPI("6", "S");
        }
        protected void Voltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("/App/Paginas/WMS/OPI.aspx");
        }
        #endregion

        #region | Função de Imagem
        protected void CarregaImgProduto(string idProduto, System.Web.UI.WebControls.Image img)
        {
            DataTable dsPesquisa;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_IMAGEM");
            vParametros.Add("@idTipoArquivo", "201");
            vParametros.Add("@idObjeto", idProduto);
            dsPesquisa = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (dsPesquisa.Rows.Count > 0)
            {
                DataRow imgBd = dsPesquisa.Rows[0];
                string imgUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])imgBd["vbArquivo"]);

                img.ImageUrl = imgUrl;
                img.Visible = true;
            }
            else
            {
                img.ImageUrl = "/App/img/wms_dimensoes.svg";
                img.Visible = true;
            }
        }
        protected string CarregaImgProduto(string idProduto)
        {
            DataTable dsPesquisa;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_IMAGEM");
            vParametros.Add("@idTipoArquivo", "201");
            vParametros.Add("@idObjeto", idProduto);
            dsPesquisa = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (dsPesquisa.Rows.Count > 0)
            {
                DataRow imgBd = dsPesquisa.Rows[0];
                return Convert.ToBase64String((byte[])imgBd["vbArquivo"]);
            }
            else
            {
                return "Sem Imagem";
            }
        }
        #endregion

        #region | Limpar Campos
        void LimparCamposVolumes()
        {
            var dt = DateTime.Now;
            txtDtEnvio.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');
            ddlEmbalagem.SelectedValue = "0";
            txtNPesoLiquido.Text = "";
            txtNPesoBruto.Text = "";
            txtNComprimento.Text = "";
            txtNLargura.Text = "";
            txtNAltura.Text = "";
            hddidVolumeUpdt.Value = "";
            //txtNQtdVolumes.Text = "";
            MultiViewFormularios.ActiveViewIndex = 0;
        }
        void LimparCamposUnitizados()
        {
            hddidUnitizado.Value = "0";
            hddidUnitizadoUpdt.Value = "0";
            txtsDscUnitizado.Text = "";
            ddlEmbalagemUnitizado.SelectedValue = "0";
        }

        void LimpaCamposVolumes()
        {
            txtsCodigoBarrasVolume.Text = "";
            ddlEmbalagem.SelectedValue = "0";
            //txtnVolume.Text = "";
        }
        #endregion

        #region | Validação 
        private bool ValidarDadosEnvios()
        {
            bool bRetorno = true;
            string sMensagemErro = "";


            if (string.IsNullOrEmpty(txtDtEnvio.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Data";
            }
            if (string.IsNullOrEmpty(txtNPesoLiquido.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o Peso Líquido";
            }
            if (string.IsNullOrEmpty(txtNPesoBruto.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o Peso do Produto";
            }
            if (string.IsNullOrEmpty(txtNPesoBruto.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o Peso Bruto";
            }
            if (string.IsNullOrEmpty(txtNComprimento.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o Comprimento";
            }
            if (string.IsNullOrEmpty(txtNLargura.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe a Largura";
            }
            if (string.IsNullOrEmpty(txtNAltura.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe a Altura";
            }
            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPaginaEnvios.MostraMensagem_Erro(sMensagemErro, false);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalEnvio", "$('#modalEnvio').modal('show');", true);
            }

            return bRetorno;
        }
        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";
            if (ddlStatus.SelectedValue == "0")
            {
                sMensagemErro = "Informe um Status!";
            }
            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }


            return bRetorno;
        }
        private bool ValidarDadosUnitizados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";
            if (string.IsNullOrEmpty(txtsDscUnitizado.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Descrição para o Unitizado!";
            }
            if (ddlEmbalagemUnitizado.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Embalagem para o Unitizado!";
            }
            if (ls_unitizadosItens.Count == 0)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Adicione Algum Item ao Unitizado!";
            }
            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPaginaUnitizados.MostraMensagem_Erro(sMensagemErro, false);
                ManterModalAberta();
            }

            return bRetorno;
        }

        private bool ValidarDadosVolumes()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlEmbalagem.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Embalagem para o Volume!";
            }
            if (ls_VolumesItens.Count == 0)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Adicione Algum Item ao Volume!";
            }
            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPaginaEnvios.MostraMensagem_Erro(sMensagemErro, false);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalEnvio", "$('#modalEnvio').modal('show');", true);
            }

            return bRetorno;
        }
        private bool ValidarDadosUnitizados(bool isEventClick)
        {
            bool bRetorno = true;
            string sMensagemErro = "";
            if (string.IsNullOrEmpty(txtsDscUnitizado.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Descrição para o Unitizado!";
            }
            if (ddlEmbalagemUnitizado.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Embalagem para o Unitizado!";
            }
            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPaginaEnvios.MostraMensagem_Erro(sMensagemErro, false);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalEnvio", "$('#modalEnvio').modal('show');", true);
            }

            return bRetorno;
        }
        #endregion

        #region | Combos/DDL
        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlEmbalagem, "sp_Select 'Flow_WMS_Embalagem'", "idEmbalagem", "sDScEmbalagem", false, "Selecione uma Embalagem", "0");
            FUNCOES.Popula_Combo(ddlEmbalagemUnitizado, "sp_Select 'Flow_WMS_Embalagem'", "idEmbalagem", "sDScEmbalagem", false, "Selecione uma Embalagem", "0");
            //FUNCOES.Popula_Combo(ddlLocal, "sp_Manipula_tbl_Flow_Produtos_Movimentacao 'FLOW_LOCAL_ARMAZENAMENTO'", "idLocalArmazenamento", "sDscLocalArmazenamento", false, "Selecione o Local", "0");
            PopularImpressora();
        }
        void PopularImpressora()
        {
            FUNCOES.Popula_Combo(ddlImpressora, "sp_Manipula_tbl_Flow_WMS_OPI_Etiqueta 'FLOW-IMPRESSORA'", "idImpressora", "sDscImpressora", false, "Selecione a Impressora", "0");
            ddlImpressora.SelectedValue = idImpressoraPadrao;
        }
        #endregion

        #region | Campo Produto Função Global    
        [System.Web.Services.WebMethod]
        public static string[] GetProdutos(string sDscProduto)//Atualmente esse está sendo usado
        {
            List<string> lstProdutos = new List<string>();
            if (sDscProduto.Length > 3)
            {
                SqlDataReader sdr = BD.ExecutarDataReader("sp_Select 'FLOW_Produtos', 0, 'S', '" + sDscProduto + "'");
                while (sdr.Read())
                {
                    lstProdutos.Add(string.Format("{2}|{0}|{1}|{3}", sdr["idItem"], sdr["sCodigo"], sdr["sDscProduto"], sdr["sUnidade"]));
                }
            }
            return lstProdutos.ToArray();
        }
        #endregion

        #region | Script 
        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$('[id*=txtNPesoLiquido]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtNPesoBruto]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtNComprimento]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtNLargura]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtNAltura]').mask('000.000.000.000.000,00', { reverse: true });");

            // Usa a classe .mask-money para aplicar em todos os campos da grid de uma vez
            sb.Append("$('.mask-money').mask('000.000.000.000.000,00', { reverse: true });");

            sb.Append("});");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);
            RegistraScriptAvancarRetornar();
        }
        void RegistraScriptAvancarRetornar()
        {
            string script = @"
        <script type='text/javascript'>
            function retornar() {
                // Redireciona para o evento cmdRetornar_click
                __doPostBack('" + cmRetornar.UniqueID + @"', '');
            }

            function avancar() {
                // Redireciona para o evento cmdAvancar_click
                __doPostBack('" + cmAvancar.UniqueID + @"', '');
            }
         </script>
                  ";


            // Registra o script no final da página
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CustomScript", script, false);
        }
        void RegisterQuaggaLibrary()
        {
            string scriptQuagga = @"<script src='/app/js/quagga.min.js'></script>";
            Page.ClientScript.RegisterStartupScript(GetType(), "ValidarQuaggaScript", scriptQuagga, false);
        }
        void RegistrarColapsoScript()
        {
            string script = @"
    <script type='text/javascript'>
        $(document).ready(function() {
            // Define o ícone inicial
            $('.toggle-icon').addClass('fa fa-plus');
            
            // Função para alternar ícones e mostrar/ocultar div
            $('.toggle-icon').click(function() {
                var icon = $(this);
                var divId = $(this).data('div-id');
                var current = $('#' + divId).css('display');
                if (current == 'none') {
                    $('#' + divId).show('slow');
                    icon.removeClass('fa fa-plus').addClass('fa fa-minus');
                } else {
                    $('#' + divId).hide('slow');
                    icon.removeClass('fa fa-minus').addClass('fa fa-plus');
                }
                return false; // Evita o postback
            });
        });
    </script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ExibirOcultarScript", script);
        }
        void RegistraScriptCamposPeso()
        {
            string script = @"<script>
    $('[id*=txtNPesoLiquido]').mask('000.000.000.000.000,00', { reverse: true });
    $('[id*=txtNPesoBruto]').mask('000.000.000.000.000,00', { reverse: true });
    $('[id*=txtNComprimento]').mask('000.000.000.000.000,00', { reverse: true });
    $('[id*=txtNLargura]').mask('000.000.000.000.000,00', { reverse: true });
    $('[id*=txtNAltura]').mask('000.000.000.000.000,00', { reverse: true });



    function calcularPesoTotal() {
        var largura = parseFloat(document.getElementById('" + txtNLargura.ClientID + @"').value.replace(/\./g, '').replace(',', '.'));
        var altura = parseFloat(document.getElementById('" + txtNAltura.ClientID + @"').value.replace(/\./g, '').replace(',', '.'));
        var comprimento = parseFloat(document.getElementById('" + txtNComprimento.ClientID + @"').value.replace(/\./g, '').replace(',', '.'));

        console.log('Largura: ' + largura);
        console.log('Altura: ' + altura);
        console.log('Comprimento: ' + comprimento);

        if (!isNaN(largura) && !isNaN(altura) && !isNaN(comprimento)) {
            var volumeCubico = comprimento * largura * altura;
            console.log('Volume Cubico: ' + volumeCubico);
            document.getElementById('" + lblVolumeCubico.ClientID + @"').innerHTML = 'Volume m³: ' + volumeCubico.toFixed(2) + ' m³';
        } else {
            document.getElementById('" + lblVolumeCubico.ClientID + @"').innerHTML = 'Calculando...';
        }
    }

    // Re-registrar máscaras após uma atualização parcial da página
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function() {
        $('[id*=txtNPesoLiquido]').mask('000.000.000.000.000,00', { reverse: true });
        $('[id*=txtNPesoBruto]').mask('000.000.000.000.000,00', { reverse: true });
        $('[id*=txtNComprimento]').mask('000.000.000.000.000,00', { reverse: true });
        $('[id*=txtNLargura]').mask('000.000.000.000.000,00', { reverse: true });
        $('[id*=txtNAltura]').mask('000.000.000.000.000,00', { reverse: true });
    });
</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "calcularPesoTotal", script, false);
        }
        #endregion

        #region | Avançar e Retornar Botões
        protected void cmdAvancar_click(object sender, EventArgs e)
        {
            int id = 0;
            if (txtidOPI.Text != "Novo" && !string.IsNullOrEmpty(txtidOPI.Text))
                id = Convert.ToInt32(txtidOPI.Text) + 1;
            else
                id = Convert.ToInt32(Request["id"]) + 1;

            Response.Redirect($"OPI_Detalhe.aspx?id={id}");
        }

        protected void cmdRetornar_click(object sender, EventArgs e)
        {
            int id = 0;

            if (txtidOPI.Text != "Novo" && !string.IsNullOrEmpty(txtidOPI.Text))
                id = Convert.ToInt32(txtidOPI.Text) - 1;
            else
                id = Convert.ToInt32(Request["id"]) - 1;

            Response.Redirect($"OPI_Detalhe.aspx?id={id}");

        }
        #endregion

        #region | Itens
        protected void dtgItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                System.Web.UI.WebControls.Image img = (System.Web.UI.WebControls.Image)e.Row.FindControl("imgProduto");

                TextBox txtIdProduto = (TextBox)e.Row.FindControl("txtIdProduto");
                if (txtIdProduto != null)
                {
                    CarregaImgProduto(txtIdProduto.Text, img);
                }
            }
        }
        void PopularItens(string idOPI)
        {
            string sFuncao = "CONSULTAR-ITENS";

            DataTable tb;
            string sSql = "sp_Manipula_FLow_WMS_OPI";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idOPI", idOPI);

            tb = BD.ExecutarDataTable(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                if (dtgItens != null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DestroyDataTables", "if ($.fn.DataTable.isDataTable('#" + dtgItens.ClientID + "')) $('#" + dtgItens.ClientID + "').DataTable().destroy();", true);
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTablesItens", TT.FrameWork.Grid.DataBindComScript(dtgItens, tb, 0, "dsc"), true);

                Pesquisar(hddidOPI.Value, false);
            }
            else
            {
                MensagemPaginaItens.MostraMensagem_Erro("Nenhum Lançamento Localizado", false);
            }
        }
        #endregion

        #region | Envios
        void PopularEnvios(string idOPI)
        {
            string sFuncao = "CONSULTAR-ENVIOS";

            DataTable tb;
            string sSql = "sp_Manipula_FLow_WMS_OPI";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idOPI", idOPI);

            var dt = DateTime.Now;
            txtDtEnvio.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');

            tb = BD.ExecutarDataTable(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                cmdExcluir.Visible = false;
                divTableEnvios.Visible = true;
                //AtualizarBarraProgresso();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTablesEnvios", TT.FrameWork.Grid.DataBindComScript(dtgvEnvios, tb), true);
            }
            else
            {
                cmdExcluir.Visible = FUNCOES.ValidaPermissao(Permissao.WMS.OPI.Excluir, true);

                divTableEnvios.Visible = true;
                dtgvEnvios.DataSource = null;
                dtgvEnvios.DataBind();

                cmdAbrir.Visible = true;

                //MensagemPaginaEnvios.MostraMensagem_Erro("Nenhum Lançamento Localizado");
                //divTableEnvios.Visible = false;
                //cmdAbrir.Visible = true;
            }
        }
        protected void chkOpcaoItemVolume_CheckedChanged(object sender, EventArgs e)
        {
            RegistraScriptCamposPeso();
            CheckBox chk = (CheckBox)sender;

            if (chk.Checked)
            {
                Div_FormEnvios.Visible = true;
                cmdEfetuarEnvio.Visible = true;
            }
            if (!chk.Checked)
            {
                foreach (GridViewRow row in dtgVolumes.Rows)
                {
                    CheckBox chkItens = row.FindControl("chkOpcaoItemVolume") as CheckBox;
                    if (chkItens != null && chkItens.Checked)
                    {
                        if (chkItens.Checked)
                        {
                            Div_FormEnvios.Visible = true;
                            cmdEfetuarEnvio.Visible = true;
                            break;
                        }
                    }
                    else
                    {
                        Div_FormEnvios.Visible = false;
                        cmdEfetuarEnvio.Visible = false;
                    }
                }
            }
            AbrirModal_Click(sender, e);
        }
        #region | Views Form Envios
        #region | Grid Base Envios
        protected void dtgItensEnvios_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                System.Web.UI.WebControls.Image img = (System.Web.UI.WebControls.Image)e.Row.FindControl("imgProduto");

                TextBox txtIdProduto = (TextBox)e.Row.FindControl("txtIdItem");
                if (txtIdProduto != null)
                {
                    CarregaImgProduto(txtIdProduto.Text, img);
                }
            }
        }
        void PopularEnviosItens()
        {
            string sFuncao = "CONSULTAR-ITENS";
            string sErro = "";
            DataSet dsEnviosItens;
            string sSql = "sp_Manipula_FLow_WMS_OPI";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idOPI", hddidOPI.Value);

            dsEnviosItens = BD.ExecutarDataSet(sSql, vParametros, false);

            if (BD.ValidarDataSet(dsEnviosItens, out sErro))
            {
                bs_Produto_Envios.Clear();
                bool algumItemEnviado = false;

                foreach (DataRow row in dsEnviosItens.Tables[0].Rows)
                {
                    if (Convert.ToDecimal(row["nEnviar"].ToString()) > 0)
                    {
                        algumItemEnviado = true;
                        FrameWork.cls_WMS_Produtos objItem = new FrameWork.cls_WMS_Produtos();
                        objItem.IdItem = Convert.ToInt32(row["idProduto"].ToString());
                        objItem.IdItemOPI = Convert.ToInt32(row["idItemOPI"].ToString());
                        objItem.SCodigo = row["sCodigo"].ToString();
                        objItem.SDscProduto = row["sDscProduto"].ToString();
                        objItem.nEnviar = Convert.ToDecimal(row["nEnviar"].ToString());
                        objItem.SUnidade = row["sUnidade"].ToString();
                        objItem.nPesoLiquido = Convert.ToDecimal(row["nPesoLiquido"]);
                        //objItem.IdVolume = Convert.ToInt32(row["idVolume"]);

                        objItem.sTipoEtiqueta = row["sTipoEtiqueta"].ToString();

                        bs_Produto_Envios.Add(objItem);
                    }
                }
                if (algumItemEnviado && ddlStatus.SelectedValue != "5")
                {
                    cmdAbrir.Visible = true;
                }
                else
                {
                    if (ddlStatus.SelectedValue == "5")
                    {
                        cmdAbrir.Visible = true;
                        MensagemPagina.MostraMensagem_Aviso("OPI Cancelada, não é mais possível fazer Envios.", false);
                        Div_Envios.Visible = false;
                    }
                    else
                    {
                        cmdAbrir.Visible = false;
                        cmdEfetuarEnvio.Visible = false;
                        MensagemPaginaAbaEnvios.MostraMensagem_Aviso("Todos os Itens já foram enviados, Informações de enviados abaixo:", false);
                        Alterar_Status(hddidOPI.Value, "4");
                    }
                    //cmdAbrir.Visible = true;
                }
                //dtgEnviosItens_DataBind();
            }
            else
            {
                MensagemPaginaAbaEnvios.MostraMensagem_Erro("Nenhum Item Encontrado para essa OPI!", false);
            }
        }
        #endregion

        #region| Funções de BD
        protected void btnEfetuarEnvio_Click(object sender, EventArgs e)
        {
            Gerar_Envio();
            AbrirModal_Click(sender, e);
        }
        bool EnvioVolumes_Salvar(string idEnvio)
        {
            bool validado = false;
            string sErro;
            try
            {
                foreach (GridViewRow row in dtgVolumes.Rows)
                {
                    CheckBox chkOpcaoItemVolume = (CheckBox)row.FindControl("chkOpcaoItemVolume");

                    if (chkOpcaoItemVolume.Checked)
                    {
                        Dictionary<String, String> vParametros = new Dictionary<string, string>();
                        string idVolume = dtgVolumes.DataKeys[row.RowIndex].Value.ToString();
                        vParametros.Add("@sFuncao", "INCLUIR-ITENS-ENVIO");
                        vParametros.Add("@idEnvioOPI", idEnvio);
                        vParametros.Add("@idVolume", idVolume);
                        vParametros.Add("@sDscOPI", txtsDscOPI.Text);
                        vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                        vParametros.Add("@idPedido", txtnPedido.Text);

                        vParametros.Add("@sSerieCadastrada", "N");


                        if (txtDtEnvio.Text == "")
                        {
                            vParametros.Add("@dtEnvio", txtDtEnvio.Text);
                        }
                        else
                        {
                            DateTime dt = DateTime.Parse(txtDtEnvio.Text.ToString(), CultureInfo.InvariantCulture);
                            vParametros.Add("@dtEnvio", dt.ToString());
                        }

                        DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);
                        if (BD.ValidarDataSet(dsSalvar, out sErro))
                        {
                            //hddidMovimentacao.Value = RETORNO.DATASET(dsSalvar, "idMovimentacao");
                            validado = true;
                        }
                        else
                        {
                            validado = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaVolume.MostraMensagem_Erro(ex.Message, false);
                return false;
            }

            return validado;
        }
        private int contadorVolumes()
        {
            int quantidadeMarcada = 0;

            foreach (GridViewRow row in dtgVolumes.Rows)
            {
                CheckBox chkOpcaoItemVolume = (CheckBox)row.FindControl("chkOpcaoItemVolume");

                if (chkOpcaoItemVolume.Checked)
                {
                    quantidadeMarcada++;
                }
            }
            return quantidadeMarcada;
        }
        void Gerar_Envio()
        {
            string sErro = "";
            int qtdVolumes = contadorVolumes();
            if (ValidarDadosEnvios() && qtdVolumes != 0)
            {
                try
                {
                    string[] vidOPI = hddidOPI.Value.Split(',');
                    string idOPI = vidOPI[0].ToString();

                    // ==============================================================
                    // PEDÁGIO DE VALIDAÇÃO: Verifica saldo antes de gerar o envio
                    // ==============================================================
                    string idsVolumesSelecionados = ObterIdsVolumesSelecionados();

                    Dictionary<String, String> vParamValidacao = new Dictionary<string, string>();
                    vParamValidacao.Add("@sFuncao", "VALIDAR-LIMITE-ENVIO");
                    vParamValidacao.Add("@idOPI", idOPI);
                    vParamValidacao.Add("@sVolumes", idsVolumesSelecionados); // Manda os IDs pro banco somar

                    DataSet dsValidacao = BD.ExecutarDataSet(sProcedure, vParamValidacao);

                    // Se ValidarDataSet retornar false, significa que estourou o limite!
                    if (!BD.ValidarDataSet(dsValidacao, out sErro))
                    {
                        MensagemPaginaVolume.MostraMensagem_Erro(sErro, false);
                        return; // PARA A EXECUÇÃO AQUI! Não gera o envio.
                    }

                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "GERAR-ENVIO");

                    txtidOPI.Text = idOPI;
                    vParametros.Add("@idOPI", txtidOPI.Text);
                    vParametros.Add("@nQtdVolumes", qtdVolumes.ToString());
                    if (txtDtEnvio.Text == "")
                    {
                        vParametros.Add("@dtEnvio", txtDtEnvio.Text);
                    }
                    else
                    {
                        DateTime dt = DateTime.Parse(txtDtEnvio.Text.ToString(), CultureInfo.InvariantCulture);
                        vParametros.Add("@dtEnvio", dt.ToString());
                    }
                    vParametros.Add("@idUsuarioEnvio", IDENTITY.Variaveis.idUsuario());

                    vParametros.Add("@nPesoLiquido", BD.Conversoes.Numerico(txtNPesoLiquido));
                    vParametros.Add("@nPesoBruto", BD.Conversoes.Numerico(txtNPesoBruto));
                    vParametros.Add("@nComprimento", BD.Conversoes.Numerico(txtNComprimento));
                    vParametros.Add("@nLargura", BD.Conversoes.Numerico(txtNLargura));
                    vParametros.Add("@nAltura", BD.Conversoes.Numerico(txtNAltura));
                    //vParametros.Add("@nQtdVolumes", BD.Conversoes.Numerico(txtNQtdVolumes));

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {

                        hddidEnvioOPI.Value = RETORNO.DATASET(dsSalvar, 0, "idEnvioOPI");

                        if (EnvioVolumes_Salvar(hddidEnvioOPI.Value))
                        {
                            Pesquisar(idOPI, false);
                            LimparCamposVolumes();
                            MensagemPaginaVolume.MostraMensagem_Sucesso("Registro gravado com sucesso", false);
                            Session["MensagemSucesso"] = "Registro gravado com sucesso";
                            //Session["MensagemInfo"] = "";

                            if (ddlStatus.SelectedValue != "4")
                                Alterar_Status(hddidOPI.Value, "3");

                            Response.Redirect(Request.RawUrl);
                        }
                    }
                    else
                    {
                        MensagemPaginaVolume.MostraMensagem_Erro("BD: " + sErro.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MensagemPaginaVolume.MostraMensagem_Erro(ex.Message, false);
                }
            }
            else if (qtdVolumes == 0)
            {
                MensagemPaginaVolume.MostraMensagem_Erro("Selecione um Volume!");
            }
        }
        private string ObterIdsVolumesSelecionados()
        {
            List<string> ids = new List<string>();
            foreach (GridViewRow row in dtgVolumes.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkOpcaoItemVolume");
                if (chk != null && chk.Checked)
                {
                    ids.Add(dtgVolumes.DataKeys[row.RowIndex].Value.ToString());
                }
            }
            return string.Join(",", ids);
        }
        void Alterar_Status(string idOPI, string idStatus)
        {
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "INCLUIR-STATUS-ENVIO");
            vParametros.Add("@idOPI", idOPI);
            vParametros.Add("@idStatus", idStatus);
            DataSet dsStatus = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsStatus))
            {
                ddlStatus.SelectedValue = RETORNO.DATASET(dsStatus, 0, "idStatus");
            }
        }

        #endregion

        #endregion
        protected void AbrirEnvio_Click(object sender, EventArgs e)
        {
            MultiViewFormularios.ActiveViewIndex = 0;
            lbltituloModal.Text = "Envio";
            AbrirModal_Click(sender, e);
        }

        protected void dtgVolumesItens_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Excluir")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                List<FrameWork.cls_WMS_VolumesItens> listaAtual = ls_VolumesItens;

                if (index >= 0 && index < listaAtual.Count)
                {
                    listaAtual.RemoveAt(index);
                    ls_VolumesItens = listaAtual;
                    dtgVolumesItensDataBind();
                }
            }
        }
        #endregion

        #region | Etiquetas

        #region | DataBind/Popular
        void PopularEtiquetas(string idOPI)
        {
            string sFuncao = "CONSULTAR";
            string sErro = "";
            DataSet dsEtiquetas;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idOPI", idOPI);
            vParametros.Add("@sUnitizado", "N");
            vParametros.Add("@sVolume", "N");
            dsEtiquetas = BD.ExecutarDataSet(sProcedureEtiqueta, vParametros, false);

            dtgItensEtiquetas_DataBind();

            if (BD.ValidarDataSet(dsEtiquetas, out sErro))
            {
                ls_Etiquetas.Clear();

                ls_Etiquetas = dsEtiquetas.Tables[0].AsEnumerable().Select(row =>
                {
                    return new cls_WMS_Etiquetas
                    {
                        IdOPI = Convert.ToInt32(row["idOPI"]),
                        IdEtiqueta = Convert.ToInt32(row["idEtiqueta"]),
                        SdscEtiqueta = row["sDscEtiqueta"].ToString(),
                        NQuantidade = Convert.ToDecimal(row["nQuantidade"]),
                        DtImpressao = row["dtImpressao"].ToString() == "" ? "Não Impresso" : row["dtImpressao"].ToString(),
                        IdProduto = Convert.ToInt32(row["idProduto"]),
                        SdscProduto = row["sDscProduto"].ToString(),
                        SCodigoBarras = row["sCodigoBarras"].ToString(),
                        Sobservacao = row["sObservacao"].ToString(),
                        SGarantia = row["sControlaGarantia"].ToString(),
                        IdImpressora = Convert.ToInt32(row["idImpressora"].ToString()),
                        sTipoEtiqueta = row["sTipoEtiqueta"].ToString(),
                        idLocalOPI = Convert.ToInt32(row["idLocalOPI"].ToString())
                    }
            ;
                }).ToList();
            }

            dtgEtiquetas_DataBind();
        }
        protected void dtgItensEtiquetas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string tipoEtiqueta = "U";
                string unidade = "UN";
                decimal saldo = 0;

                if (e.Row.DataItem is FrameWork.cls_WMS_Produtos)
                {
                    var item = (FrameWork.cls_WMS_Produtos)e.Row.DataItem;
                    unidade = item.SUnidade;
                    saldo = item.nEnviar;

                    if (!string.IsNullOrEmpty(item.sTipoEtiqueta))
                    {
                        tipoEtiqueta = item.sTipoEtiqueta;
                    }
                }
                else if (e.Row.DataItem is DataRowView)
                {
                    var row = (DataRowView)e.Row.DataItem;
                    try { tipoEtiqueta = row["sTipoEtiqueta"].ToString(); } catch { }
                    try { unidade = row["sUnidade"].ToString(); } catch { }
                    try { saldo = Convert.ToDecimal(row["nEnviar"]); } catch { }
                }

                e.Row.Attributes.Add("data-tipo", tipoEtiqueta);
                e.Row.Attributes.Add("data-unidade", unidade);
                e.Row.Attributes.Add("data-saldo", saldo.ToString("N4", new System.Globalization.CultureInfo("pt-BR")));

                System.Web.UI.WebControls.Image img = (System.Web.UI.WebControls.Image)e.Row.FindControl("imgProduto");
                TextBox txtIdProduto = (TextBox)e.Row.FindControl("txtIdItem");

                if (txtIdProduto != null && !string.IsNullOrEmpty(txtIdProduto.Text))
                {
                    CarregaImgProduto(txtIdProduto.Text, img);
                }
            }
        }
        void dtgItensEtiquetas_DataBind()
        {
            ls_EtiquetaItens = bs_Produto_Envios;

            dtgItensEtiquetas.DataSource = ls_EtiquetaItens;
            dtgItensEtiquetas.DataBind();

            RegistraScript("");
        }

        void dtgEtiquetas_DataBind()
        {
            if (ls_Etiquetas == null) ls_Etiquetas = new List<cls_WMS_Etiquetas>();

            dtgEtiquetas.DataSource = ls_Etiquetas;
            try
            {
                dtgEtiquetas.DataBind();
            }
            catch (Exception ex)
            {

                MensagemPaginaEtiquetaPN.MostraMensagem_Erro($"Erro: {ex}");
            }

            RegistraScript("");
        }
        #endregion
        protected void SelecionaTipo_Change(object sender, EventArgs e)
        {

            dtgItensEtiquetas_DataBind();

            AbrirModal_Click(sender, e);
        }

        protected void IncluirEtiquetas_Click(object sender, EventArgs e)
        {
            bool algumaGerada = false;
            int totalEtiquetasGeradas = 0;

            try
            {
                foreach (GridViewRow row in dtgItensEtiquetas.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        TextBox txtQtd = (TextBox)row.FindControl("txtQtdEtiquetas");
                        TextBox txtUnid = (TextBox)row.FindControl("txtnQuantidadeEnvio");
                        TextBox txtObs = (TextBox)row.FindControl("txtObservacao");
                        HiddenField hddDesc = (HiddenField)row.FindControl("hddSDscEtiqueta");

                        string idItemStr = dtgItensEtiquetas.DataKeys[row.RowIndex]["idItem"].ToString();
                        int idProduto = Convert.ToInt32(idItemStr);
                        string sDscProduto = hddDesc != null ? hddDesc.Value : "Produto";

                        int qtdLoop = 0;
                        decimal qtdPorEtiqueta = 0;

                        int.TryParse(txtQtd.Text, out qtdLoop);

                        if (!string.IsNullOrEmpty(txtUnid.Text))
                        {
                            qtdPorEtiqueta = Convert.ToDecimal(BD.Conversoes.Numerico(txtUnid.Text));
                        }

                        if (qtdLoop > 0 && qtdPorEtiqueta > 0)
                        {
                            // ====================================================================
                            // 1. TRAVA DE LIMITE (SALDO) NA IMPRESSÃO
                            // ====================================================================
                            decimal totalSolicitado = qtdLoop * qtdPorEtiqueta;
                            var prodRef = bs_Produto_Envios.FirstOrDefault(p => p.IdItem == idProduto);
                            decimal saldoPermitido = prodRef != null ? prodRef.nEnviar : 0;

                            if (totalSolicitado > saldoPermitido)
                            {
                                MensagemPaginaEtiquetaPN.MostraMensagem_Aviso($"Bloqueado: Você tentou gerar {totalSolicitado} etiquetas para o item {sDscProduto}, mas o limite disponível na OPI é {saldoPermitido}.");
                                continue; // Pula este item abusivo e continua lendo o resto da Grid
                            }

                            for (int i = 0; i < qtdLoop; i++)
                            {
                                GravarEtiquetaUnitariaNoBanco(idProduto, sDscProduto, qtdPorEtiqueta, txtObs.Text);
                                totalEtiquetasGeradas++;
                            }

                            algumaGerada = true;

                            txtQtd.Text = "";
                            txtUnid.Text = "";
                            txtObs.Text = "";
                        }
                    }
                }

                if (algumaGerada)
                {
                    MensagemPaginaEtiquetaPN.MostraMensagem_Sucesso($"{totalEtiquetasGeradas} etiquetas geradas e salvas com sucesso!");

                    PopularEtiquetas(hddidOPI.Value);
                    PopularItens(hddidOPI.Value);
                }
                else
                {
                    MensagemPaginaEtiquetaPN.MostraMensagem_Aviso("Preencha a 'Qtd Etiquetas' e 'Qtd p/ Etiqueta' corretamente dentro do saldo permitido.");
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaEtiquetaPN.MostraMensagem_Erro("Erro ao gerar etiquetas: " + ex.Message);
            }

            string scriptNavegacao = @"
                $('#modalGerenciarEtiquetas').modal('hide'); 
                $('.nav-tabs a[href=""#Etiqueta""]').tab('show');
            ";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "IrParaAbaEtiquetas", scriptNavegacao, true);
        }

        protected void ExcluirEtiquetas_Click(object sender, EventArgs e)
        {
            LinkButton btnExcluir = (LinkButton)sender;
            string idEtiqueta = btnExcluir.CommandArgument;

            if (idEtiqueta != "0")
            {
                var etiqueta = ls_Etiquetas.FirstOrDefault(et => et.IdEtiqueta == Convert.ToInt32(idEtiqueta));
                if (etiqueta != null)
                {
                    etiqueta.SExclusao = "S";
                }

                GridViewRow rowToDelete = (GridViewRow)btnExcluir.NamingContainer;
                rowToDelete.Visible = false;
            }
            else
            {
                GridViewRow row = (GridViewRow)btnExcluir.NamingContainer;
                int rowIndex = row.RowIndex;
                ls_Etiquetas.RemoveAt(rowIndex);
            }

            dtgEtiquetas.DataSource = ls_Etiquetas;
            dtgEtiquetas.DataBind();

            AbrirModal_Click(sender, e);

        }

        protected void dtgEtiquetas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sControla = "";

                cls_WMS_Etiquetas etiqueta = (cls_WMS_Etiquetas)e.Row.DataItem;
                if (etiqueta.SExclusao == "S")
                {
                    e.Row.Visible = false;
                }

                if (e.Row.DataItem != null)
                {
                    object valorGarantia = DataBinder.Eval(e.Row.DataItem, "SGarantia");
                    sControla = valorGarantia != null ? valorGarantia.ToString() : "";
                }
            }
        }

        private string GravarEtiquetaUnitariaNoBanco(int idProduto, string dscProduto, decimal quantidade, string observacao)
        {
            Dictionary<String, String> vParametros = new Dictionary<string, string>();

            vParametros.Add("@sFuncao", "SALVAR");
            vParametros.Add("@idEtiqueta", "0");
            vParametros.Add("@idOPI", hddidOPI.Value);
            vParametros.Add("@idProduto", idProduto.ToString());
            vParametros.Add("@sDscEtiqueta", dscProduto);
            vParametros.Add("@nQuantidade", quantidade.ToString(System.Globalization.CultureInfo.InvariantCulture));
            vParametros.Add("@sObservacao", observacao ?? "");
            vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
            vParametros.Add("@sExclusao", "N");
            vParametros.Add("@sUnitizado", "N");
            vParametros.Add("@sVolume", "N");

            string codigoBarrasBase = "PR" + idProduto.ToString();
            vParametros.Add("@sCodigoBarras", codigoBarrasBase);

            if (!string.IsNullOrEmpty(ddlImpressora.SelectedValue))
            {
                vParametros.Add("@idImpressora", ddlImpressora.SelectedValue);
            }


            // Roda a procedure
            DataSet ds = BD.ExecutarDataSet(sProcedureEtiqueta, vParametros);
            string idEtiquetaStr = RETORNO.DATASET(ds, 0, "idEtiqueta");

            // ======= A MÁGICA CONTRA O BUG DO F5 =======
            // 2. Faz o Select com DataReader igual ao WebMethod para pegar a string exata
            string codigoExatoDoBanco = "";
            string query = "SELECT sCodigoBarras FROM tbl_Flow_WMS_OPI_Etiqueta (NOLOCK) WHERE idEtiqueta = " + idEtiquetaStr;

            SqlDataReader sdr = BD.ExecutarDataReader(query);
            if (sdr.Read())
            {
                codigoExatoDoBanco = sdr["sCodigoBarras"].ToString();
            }

            if (!sdr.IsClosed) { sdr.Close(); }

            return codigoExatoDoBanco;
        }

        protected void ddlLocal_Salvos_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlLocal = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddlLocal.NamingContainer;
            DropDownList ddlPosicaoPai = (DropDownList)row.FindControl("ddlPosicaoPai");

            if (ddlPosicaoPai != null)
            {
                FUNCOES.Popula_Combo(ddlPosicaoPai, $"{sProcedureLocal} 'FLOW-POSICOES', @idLocalArmazenamento={ddlLocal.SelectedValue}", "idPosicao", "sCodigoLocal", false, "Selecione a Posição", "0");
            }
            AbrirModalUnitizados();

        }

        #region | Modal Etiquetas
        protected void cmdAbrirEtiqueta_Click(object sender, EventArgs e)
        {
            Pesquisar(hddidOPI.Value, false);
            try
            {
                PopularImpressora();
                ddlImpressora.SelectedValue = idImpressoraPadrao;
            }
            catch (Exception ex)
            {
                MensagemPaginaAbaEnvios.MostraMensagem_Erro(ex.ToString());
            }

            string mensagemInfo = " Para não imprimir etiquetas de algum produto passe 0 no campo de quantidade de etiqueta ou clique em <b>Limpar</b> e adicione a quantidade nos itens desejados.";

            string scriptJS = $"ExibirAvisoValor('{mensagemInfo}', null, 'info'); $('#modalGerenciarEtiquetas').modal('show');";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "AbrirModalComMsgInfo", scriptJS, true);

        }
        #endregion
        #endregion

        #region | Unitizados
        #region | DataGrid Unitizado
        void PopularUnitizados()
        {
            DataSet dsUnitizados = new DataSet();
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR-UNITIZADOS");
            vParametros.Add("@idOPI", hddidOPI.Value);
            dsUnitizados = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsUnitizados))
            {
                div_UnitizadoGV.Visible = true;
                ls_unitizadosItensGV.Clear();
                dtgUnitizadosDataBind(dsUnitizados);
            }
            else
            {
                dtgUnitizadosDataBind(null);
            }
        }

        protected void ImportarPendentesUnitizado_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR-ETIQUETAS-PNDNTS-UN");
                vParametros.Add("@idOPI", hddidOPI.Value);

                // Usando a procedure correta conforme seu padrão
                DataSet dsEtiquetas = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsEtiquetas))
                {
                    int contagemAdicionados = 0;

                    List<cls_WMS_Produtos> novosItens = dsEtiquetas.Tables[0].AsEnumerable()
                        .Select(row => new cls_WMS_Produtos()
                        {
                            IdItem = Convert.ToInt32(row["idProduto"]),
                            SCodigo = row["sCodigo"].ToString(),
                            SDscProduto = row["sDscProduto"].ToString(),
                            NQuantidade = Convert.ToDecimal(row["nQuantidade"]),
                            SCodigoBarras = row["sCodigoBarras"].ToString(),
                        }).ToList();

                    foreach (var item in novosItens)
                    {
                        // Evita duplicar o que já foi bipado/adicionado na grid atual
                        if (!ls_unitizadosItens.Any(x => x.SCodigoBarras == item.SCodigoBarras))
                        {
                            ls_unitizadosItens.Add(item);
                            contagemAdicionados++;
                        }
                    }

                    if (contagemAdicionados > 0)
                    {
                        dtgUNItensDataBind();
                        MensagemPaginaUnitizados.MostraMensagem_Sucesso($"{contagemAdicionados} itens importados!", false);
                    }
                    else
                    {
                        MensagemPaginaUnitizados.MostraMensagem_Aviso("Não há novos itens pendentes para importar.", false);
                    }
                }
                else
                {
                    MensagemPaginaUnitizados.MostraMensagem_Aviso("Nenhuma etiqueta pendente encontrada.");
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaUnitizados.MostraMensagem_Erro("Erro na importação: " + ex.Message);
            }

            FecharLeitor();
            ManterModalAberta();
        }

        void PopularUnitizados(string idUnitizado)
        {
            DataSet dsUnitizados = new DataSet();
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR-UNITIZADOS");
            vParametros.Add("@idOPI", hddidOPI.Value);
            vParametros.Add("@idUnitizado", idUnitizado);

            dsUnitizados = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsUnitizados))
            {
                ddlEmbalagemUnitizado.SelectedValue = RETORNO.DATASET(dsUnitizados, "idProdutoEmbalagem");
                txtsDscUnitizado.Text = RETORNO.DATASET(dsUnitizados, "sDscUnitizado");
                div_UnitizadoGV.Visible = true;
                dtgUnitizadosDataBind(dsUnitizados);
                ls_unitizadosItensGV.Clear();
            }


            string sFuncao = "CONSULTAR-UNITIZADOS-ITENS-OPI";
            Dictionary<String, String> vParametrosItens = new Dictionary<string, string>();
            vParametrosItens.Add("@sFuncao", sFuncao);
            vParametrosItens.Add("@idUnitizado", idUnitizado);

            DataSet dsItems = BD.ExecutarDataSet(sProcedure, vParametrosItens);

            ls_unitizadosItens = conversorProdutos.ConverterDataSet(dsItems, "Table");


            dtgUNItensDataBind();
        }
        void dtgUnitizadosDataBind(DataSet dsUnitizados)
        {
            if (dsUnitizados != null && dsUnitizados.Tables.Count > 0 && dsUnitizados.Tables[0].Rows.Count > 0)
            {
                dtgUnitizado.DataSource = dsUnitizados;
            }
            else
            {
                // Cria um DataTable vazio mas com a estrutura de colunas correta ou null
                dtgUnitizado.DataSource = null;
            }

            dtgUnitizado.DataBind();
            RegistrarColapsoScript();
        }
        protected void dtgUnitizado_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string idUnitizado = dtgUnitizado.DataKeys[e.Row.RowIndex].Value.ToString();

                string sEnviado = DataBinder.Eval(e.Row.DataItem, "sEnviado").ToString();
                LinkButton cmdEditar = (LinkButton)e.Row.FindControl("cmdEditarUnitizado");
                LinkButton cmdExcluir = (LinkButton)e.Row.FindControl("cmdExcluirUnitizadoGeral");

                var dtgUnitizadosItens = (GridView)e.Row.FindControl("dtgUnitizadosItens");

                if (sEnviado == "Sim")
                {
                    if (cmdEditar != null)
                    {
                        cmdEditar.Visible = false;
                    }
                    if (cmdExcluir != null)
                    {
                        cmdExcluir.Visible = false;
                    }
                }
                else
                {
                    if (cmdEditar != null)
                    {
                        cmdEditar.Visible = true;
                    }
                    if (cmdExcluir != null)
                    {
                        cmdExcluir.Visible = true;
                    }
                }

                ConsultaUnitizadosItens(idUnitizado);

                //tenho que puxar os dados da tabela itens envios, para cá
                dtgUnitizadosItens.DataSource = ls_unitizadosItensGV;
                dtgUnitizadosItens.DataBind();
            }
        }
        void ConsultaUnitizadosItens(string idUnitizado)
        {
            string sFuncao = "CONSULTAR-UNITIZADOS-ITENS-OPI";
            DataSet dsUnitizadosItens;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idUnitizado", idUnitizado);

            dsUnitizadosItens = BD.ExecutarDataSet(sProcedure, vParametros, false);

            ls_unitizadosItensGV = conversorProdutos.ConverterDataSet(dsUnitizadosItens, "Table");
        }
        //trazer grid unitizado e add link para view de arquivos, passando o idUnitizado
        #endregion

        #region | DataGrid UnitizadosItens
        protected void IncluirUnitizadosItens_Click(object sender, EventArgs e)
        {
            if (chkPesquisaManualUnitizado.Checked)
            {
                string idSelecionado = hddIdProdutoManualUnitizado.Value;

                if (string.IsNullOrEmpty(idSelecionado))
                {
                    MensagemPaginaUnitizados.MostraMensagem_Aviso("Por favor, pesquise e selecione um produto na lista suspensa.");
                    ManterModalAberta();
                    return;
                }

                int idProduto = Convert.ToInt32(idSelecionado);
                string dscProduto = txtBuscaManualUnitizado.Text; // Pega o nome que ficou na caixa de texto

                // Chama a função e recebe a string perfeita gerada pelo Banco
                txtsCodigoBarras.Text = GravarEtiquetaUnitariaNoBanco(idProduto, dscProduto, 1, "Etiqueta Manual de Produto.");

                // Limpa os campos manuais para a próxima busca
                txtBuscaManualUnitizado.Text = "";
                hddIdProdutoManualUnitizado.Value = "";
            }
            else // 2. Lógica Original do Leitor
            {
                if (!string.IsNullOrEmpty(LeitorQuagga1.GetCodigoBarras(txtsCodigoBarras)))
                    txtsCodigoBarras.Text = LeitorQuagga1.GetCodigoBarras(txtsCodigoBarras);
            }

            if (!string.IsNullOrEmpty(txtsCodigoBarras.Text))
            {
                string codigoLido = txtsCodigoBarras.Text.Trim();

                // 1. TRAVA DE MEMÓRIA: Evitar bipar a mesma etiqueta duas vezes na grid
                if (ls_unitizadosItens.Any(x => x.SCodigoBarras.Equals(codigoLido, StringComparison.OrdinalIgnoreCase)))
                {
                    MensagemPaginaUnitizados.MostraMensagem_Aviso($"A etiqueta {codigoLido} já foi escaneada e está na lista!");
                    txtsCodigoBarras.Text = "";
                    cmdSalvarUnitizado.Visible = true;
                    FecharLeitor();
                    ManterModalAberta();
                    return;
                }

                string sErro = "";
                DataSet dsProduto = new DataSet();
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR-CODIGO-BARRAS");
                vParametros.Add("@sCodigoBarras", codigoLido);
                vParametros.Add("@idOPI", hddidOPI.Value);
                dsProduto = BD.ExecutarDataSet(sProcedure, vParametros);

                // 2. TRAVA DO BANCO: Lê a validação do SQL usando o out sErro
                if (BD.ValidarDataSet(dsProduto, out sErro))
                {
                    List<cls_WMS_Produtos> novosItens = dsProduto.Tables[0].AsEnumerable().Select(row =>
                    {
                        return new cls_WMS_Produtos()
                        {
                            IdItem = Convert.ToInt32(row["idProduto"]),
                            SDscProduto = row["sDscProduto"].ToString(),
                            SCodigo = row["sCodigo"].ToString(),
                            NQuantidade = Convert.ToDecimal(row["nQuantidade"]),
                            SCodigoBarras = row["sCodigoBarras"].ToString(),
                        };
                    }).ToList();

                    // ====================================================================
                    // 3. TRAVA DE LIMITE (SALDO): Verifica se excedeu a quantidade da OPI
                    // ====================================================================
                    bool excedeuLimite = false;
                    string msgErroLimite = "";

                    foreach (var grp in novosItens.GroupBy(x => x.IdItem))
                    {
                        decimal qtdNova = grp.Sum(x => x.NQuantidade);
                        decimal qtdJaNaGrid = ls_unitizadosItens.Where(x => x.IdItem == grp.Key).Sum(x => x.NQuantidade);

                        var prodRef = bs_Produto_Envios.FirstOrDefault(p => p.IdItem == grp.Key);
                        decimal saldoPermitido = prodRef != null ? prodRef.nEnviar : 0;

                        if ((qtdNova + qtdJaNaGrid) > saldoPermitido)
                        {
                            excedeuLimite = true;
                            msgErroLimite = $"Limite excedido para o produto {grp.First().SCodigo}. Saldo na OPI: {saldoPermitido}. Quantidade na caixa: {(qtdNova + qtdJaNaGrid)}.";
                            break;
                        }
                    }

                    if (excedeuLimite)
                    {
                        MensagemPaginaUnitizados.MostraMensagem_Aviso(msgErroLimite);
                        txtsCodigoBarras.Text = "";
                    }
                    else
                    {
                        ls_unitizadosItens.AddRange(novosItens);
                        dtgUNItensDataBind();
                        txtsCodigoBarras.Text = "";
                    }
                }
                else
                {
                    // Exibe a mensagem exata do banco ou a genérica
                    MensagemPaginaUnitizados.MostraMensagem_Erro(!string.IsNullOrEmpty(sErro) ? sErro : "Escaneie um Código de Barras Válido para o Produto Desejado!");
                    txtsCodigoBarras.Text = "";
                }
            }
            else
            {
                MensagemPaginaUnitizados.MostraMensagem_Aviso("O Campo do Código de Barras é Obrigatório!");
            }
            cmdSalvarUnitizado.Visible = true;

            FecharLeitor();

            ManterModalAberta();
        }

        void FecharLeitor()
        {
            LeitorQuagga1.DesligarCamVariante();
            DivBipador.Visible = false;
            cmdAbrirCâmera.Text = "Abrir Leitor";
        }

        void dtgUNItensDataBind()
        {
            if (ls_unitizadosItens == null) ls_unitizadosItens = new List<FrameWork.cls_WMS_Produtos>();

            dtgUNItens.DataSource = ls_unitizadosItens;
            dtgUNItens.DataBind();
            RegistraScript("");
        }

        protected void UnitizadosItens_Salvar(string idUnitizado)
        {
            // ETAPA 1: Remover do banco itens que o usuário excluiu da grid visualmente
            // -------------------------------------------------------------------------
            Dictionary<string, string> vParametrosBusca = new Dictionary<string, string>();
            vParametrosBusca.Add("@sFuncao", "CONSULTAR-UNITIZADOS-ITENS-OPI");
            vParametrosBusca.Add("@idUnitizado", idUnitizado);

            DataSet dsBanco = BD.ExecutarDataSet(sProcedure, vParametrosBusca);
            List<FrameWork.cls_WMS_Produtos> itensNoBanco = new List<FrameWork.cls_WMS_Produtos>();

            if (BD.ValidarDataSet(dsBanco))
            {
                itensNoBanco = conversorProdutos.ConverterDataSet(dsBanco, "Table");
            }

            foreach (var itemBanco in itensNoBanco)
            {
                bool aindaExiste = ls_unitizadosItens.Any(x => x.SCodigoBarras == itemBanco.SCodigoBarras);

                if (!aindaExiste)
                {
                    Dictionary<string, string> vParametrosDel = new Dictionary<string, string>();
                    vParametrosDel.Add("@sFuncao", "EXCLUIR-ITEM-UNITIZADO");
                    vParametrosDel.Add("@idUnitizado", idUnitizado);
                    vParametrosDel.Add("@idProdutoUnitizado", itemBanco.IdItem.ToString()); // Mantém por compatibilidade

                    // --- OBRIGATÓRIO AGORA ---
                    vParametrosDel.Add("@sCodigoBarras", itemBanco.SCodigoBarras);

                    BD.ExecutarDataSet(sProcedure, vParametrosDel);
                }
            }

            // ETAPA 2: Salvar (Upsert) os itens da lista
            // -------------------------------------------------------------------------
            if (ls_unitizadosItens.Count > 0)
            {
                Dictionary<string, string> vParametrosItens = new Dictionary<string, string>();

                // Loop direto na lista, SEM AGRUPAMENTO, conforme solicitado
                ls_unitizadosItens.ForEach(a =>
                {
                    vParametrosItens.Clear();
                    vParametrosItens.Add("@sFuncao", "SALVAR-ITEM-UNITIZADO");
                    vParametrosItens["@idUnitizado"] = idUnitizado;
                    vParametrosItens["@nQuantidadeUnitizado"] = BD.Conversoes.Numerico(a.NQuantidade);
                    vParametrosItens["@idProdutoUnitizado"] = a.IdItem.ToString();
                    vParametrosItens["@sCodigoBarras"] = a.SCodigoBarras.ToString();

                    BD.ExecutarDataSet(sProcedure, vParametrosItens);
                });

                // Limpa a lista temporária e o campo de input
                ls_unitizadosItens.Clear();
                dtgUNItensDataBind();
                txtsCodigoBarras.Text = "";
            }
            else
            {
                // Lista vazia (usuário pode ter excluído tudo)
                ls_unitizadosItens.Clear();
                dtgUNItensDataBind();
            }
        }
        #endregion

        #region | Controles FrontEnd
        protected void AbrirUnitizados_Click(object sender, EventArgs e)
        {
            LimparCamposUnitizados();
            ls_unitizadosItens.Clear();

            MultiViewFormularios.ActiveViewIndex = 1;
            lbltituloModal.Text = "Unitizados";

            if (MultiViewUnitizados.ActiveViewIndex == 1 || MultiViewUnitizados.ActiveViewIndex == 2) ;

            AbrirModalUnitizados();
        }
        protected void ProximoUnitizado_Click(object sender, EventArgs e)
        {
            if (ValidarDadosUnitizados(true))
            {
                cmdSalvarUnitizado.Visible = true;
                MultiViewUnitizados.ActiveViewIndex = 1;
            }

            ManterModalAberta();
        }
        protected void AbrirArquivos_Click(object sender, EventArgs e)
        {
            LinkButton cmdArquivo = (LinkButton)sender;
            string idUnitizado = cmdArquivo.CommandArgument;

            cmdSalvarUnitizado.Visible = true;
            MultiViewUnitizados.ActiveViewIndex = 2;
            lbltituloModal.Text = "Fotos do Unitizado";

            Popular_aba_ArquivoUnitizado(idUnitizado);
            ManterModalAberta();
        }
        protected void AnteriorUnitizado_Click(object sender, EventArgs e)
        {
            LimparCamposUnitizados();
            ls_unitizadosItens.Clear();

            MultiViewUnitizados.ActiveViewIndex = 0;
            PopularUnitizados();

            btnAbaListaUnitizado.CssClass = "nav-link activeMn";
            btnAbaNovoUnitizado.CssClass = "nav-link";

            ManterModalAberta();
        }

        protected void cmdAbrirUnitizados_Click(object sender, EventArgs e)
        {
            LimparCamposUnitizados();
            ls_unitizadosItens.Clear();

            MultiViewUnitizados.ActiveViewIndex = 1;
            PopularUnitizados();

            dtgUNItensDataBind();

            btnAbaListaUnitizado.CssClass = "nav-link";
            btnAbaNovoUnitizado.CssClass = "nav-link activeMn";

            AbrirModalUnitizados();
            UpdatePanel1.Update();
        }
        public void ManterModalAberta()
        {
            // Apenas registra o script para garantir que a modal fique visível
            string script = "$('#modalUnitizado').modal('show');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ManterModalUnitizado", script, true);
        }
        protected void AbrirCamera_Click(object sender, EventArgs e)
        {
            bool abrir = (bool)Session["CameraAberta"];

            abrir = !abrir;

            Session["CameraAberta"] = abrir;

            DivBipador.Visible = abrir;

            cmdAbrirCâmera.Text = abrir ? "Fechar Leitor" : "Abrir Leitor";

            if (cmdAbrirCâmera.Text == "Fechar Leitor")
            {

                LeitorQuagga1.AbrirCameraVariante();
                //divCodigoB.Style["Display"] = "none";
            }
            else
            {
                LeitorQuagga1.DesligarCamVariante();
                //divCodigoB.Style["Display"] = "block";
            }

            ManterModalAberta();
        }
        protected void dtgUNItens_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Excluir")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                if (index >= 0 && index < ls_unitizadosItens.Count)
                {
                    ls_unitizadosItens.RemoveAt(index);

                    dtgUNItensDataBind();
                }
            }
            ManterModalAberta();
        }
        protected void AbrirNovoUnitizado_Click(object sender, EventArgs e)
        {
            LimparCamposUnitizados();
            ls_unitizadosItens.Clear();

            MultiViewUnitizados.ActiveViewIndex = 1;
            lbltituloModal.Text = "Novo Unitizado";

            dtgUNItensDataBind();

            btnAbaListaUnitizado.CssClass = "nav-link";
            btnAbaNovoUnitizado.CssClass = "nav-link activeMn";

            AbrirModalUnitizados();
            UpdatePanel1.Update();
        }

        public void AbrirModalUnitizados()
        {
            string script = "$('#modalUnitizado').modal('show');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "AbrirModalUnitizados", script, true);
        }

        protected void EditarUnitizado_Click(object sender, EventArgs e)
        {
            LinkButton cmdEditar = (LinkButton)sender;
            string idUnitizado = cmdEditar.CommandArgument;

            hddidUnitizadoUpdt.Value = idUnitizado;

            MultiViewUnitizados.ActiveViewIndex = 1;
            lbltituloModal.Text = "Editar Unitizado";

            btnAbaListaUnitizado.CssClass = "nav-link";
            btnAbaNovoUnitizado.CssClass = "nav-link activeMn";

            PopularUnitizados(idUnitizado);

            ManterModalAberta();
        }
        #endregion

        protected void SalvarUnitizado_Click(object sender, EventArgs e)
        {
            SalvarUnitizados(hddidUnitizadoUpdt.Value);
            ManterModalAberta();
        }
        void LimparAbaUnitizado()
        {
            LimparCamposUnitizados();
            ls_unitizadosItens.Clear();

            MultiViewUnitizados.ActiveViewIndex = 1;
            lbltituloModal.Text = "Novo Unitizado";

            dtgUNItensDataBind();

            btnAbaListaUnitizado.CssClass = "nav-link";
            btnAbaNovoUnitizado.CssClass = "nav-link activeMn";

            //UpdatePanel1.Update();
        }
        void SalvarUnitizados(string idUnitizado)
        {
            string sErro = "";

            if (idUnitizado == "" || idUnitizado == "0")
            {
                if (ValidarDadosUnitizados())
                {
                    try
                    {
                        string[] vidOPI = hddidOPI.Value.Split(',');
                        string idOPI = vidOPI[0].ToString();
                        Dictionary<String, String> vParametros = new Dictionary<string, string>();
                        vParametros.Add("@sFuncao", "SALVAR-UNITIZADO");

                        txtidOPI.Text = idOPI;
                        vParametros.Add("@idUnitizado", hddidUnitizadoUpdt.Value);
                        vParametros.Add("@idOPI", txtidOPI.Text);
                        vParametros.Add("@idProdutoEmbalagem", ddlEmbalagemUnitizado.SelectedValue);
                        vParametros.Add("@sDscUnitizado", txtsDscUnitizado.Text);
                        vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());

                        DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                        if (BD.ValidarDataSet(dsSalvar, out sErro))
                        {
                            string novoId = RETORNO.DATASET(dsSalvar, 0, "idUnitizado");

                            hddidUnitizado.Value = novoId;
                            hddidUnitizadoUpdt.Value = novoId;

                            UnitizadosItens_Salvar(novoId);
                            PopularUnitizados(novoId);
                            MensagemPaginaUnitizados.MostraMensagem_Sucesso("Unitizado criado com sucesso!", false);
                            MultiViewUnitizados.ActiveViewIndex = 1;
                            PopularEtiquetas(hddidOPI.Value);
                            LimparAbaUnitizado();
                        }
                        else
                        {
                            MensagemPaginaUnitizados.MostraMensagem_Erro("BD: " + sErro.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        MensagemPaginaUnitizados.MostraMensagem_Erro(ex.Message, false);
                    }
                }
            }
            // CASO 2: EDIÇÃO (Já tem ID)
            else if (idUnitizado != "" && idUnitizado != "0")
            {
                // Apenas salva os itens e recarrega
                UnitizadosItens_Salvar(idUnitizado);

                PopularUnitizados(idUnitizado);

                MensagemPaginaUnitizados.MostraMensagem_Sucesso("Unitizado atualizado com sucesso", false);
                LimparAbaUnitizado();
                // Mantém na tela de edição
                MultiViewUnitizados.ActiveViewIndex = 1;
            }
        }
        #endregion

        #region |  Volumes
        #region | datagrid volumesItens
        void dtgVolumesItensDataBind()
        {
            dtgVolumesItens.DataSource = ls_VolumesItens;
            dtgVolumesItens.DataBind();
            RegistraScript("");
        }
        #endregion
        protected void IncluirVolumesItens_Click(object sender, EventArgs e)
        {
            bool abrir = (bool)Session["CameraAbertaVolume"];
            // 1. Lógica se o usuário usar o Dropdown Manual
            if (chkPesquisaManualVolume.Checked)
            {
                string idSelecionado = hddIdProdutoManualVolume.Value;

                if (string.IsNullOrEmpty(idSelecionado))
                {
                    MensagemPaginaVolume.MostraMensagem_Aviso("Por favor, pesquise e selecione um produto na lista suspensa.");
                    AbrirModal_Click(sender, e);
                    return;
                }

                int idProduto = Convert.ToInt32(idSelecionado);
                string dscProduto = txtBuscaManualVolume.Text;

                // Chama a função e recebe a string perfeita gerada pelo Banco
                txtsCodigoBarrasVolume.Text = GravarEtiquetaUnitariaNoBanco(idProduto, dscProduto, 1, "Etiqueta Manual de Produto.");

                // Limpa os campos manuais para a próxima busca
                txtBuscaManualVolume.Text = "";
                hddIdProdutoManualVolume.Value = "";
            }
            else // 2. Lógica Original do Leitor
            {
                if (!string.IsNullOrEmpty(LeitorQuagga.GetCodigoBarras(txtsCodigoBarrasVolume)))
                    txtsCodigoBarrasVolume.Text = LeitorQuagga.GetCodigoBarras(txtsCodigoBarrasVolume);
                else if (!string.IsNullOrEmpty(hddsCodigoBarras.Value))
                    txtsCodigoBarrasVolume.Text = hddsCodigoBarras.Value;
            }

            if (!string.IsNullOrEmpty(txtsCodigoBarrasVolume.Text))
            {
                string codigoLido = txtsCodigoBarrasVolume.Text.Trim();

                // 1. TRAVA DE MEMÓRIA: Evitar bipar a mesma etiqueta duas vezes na grid
                if (ls_VolumesItens.Any(x => x.SCodigoBarras.Equals(codigoLido, StringComparison.OrdinalIgnoreCase)))
                {
                    MensagemPaginaVolume.MostraMensagem_Aviso($"A etiqueta {codigoLido} já foi escaneada e está neste volume!", true);
                    txtsCodigoBarrasVolume.Text = "";

                    if (abrir)
                        AbrirCameraVolume_Click(sender, e);
                    AbrirModal_Click(sender, e);
                    return;
                }

                string sErro = "";
                DataSet dsVolumeItem = new DataSet();
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR-CODIGO-BARRAS-VOL");
                vParametros.Add("@sCodigoBarras", codigoLido);
                vParametros.Add("@idOPI", hddidOPI.Value);
                dsVolumeItem = BD.ExecutarDataSet(sProcedure, vParametros);

                if (codigoLido.StartsWith("PR"))
                {
                    // 2. TRAVA DO BANCO (Lendo o sErro)
                    if (BD.ValidarDataSet(dsVolumeItem, out sErro))
                    {
                        List<cls_WMS_VolumesItens> novosItens = dsVolumeItem.Tables[0].AsEnumerable().Select(row =>
                        {
                            return new cls_WMS_VolumesItens()
                            {
                                IdObjeto = Convert.ToInt32(row["idProduto"]),
                                SDscObjeto = row["sDscProduto"].ToString(),
                                SCodigoBarras = codigoLido,
                                NQuantidade = Convert.ToInt32(row["nQuantidade"]),
                                SCodigoProduto = row["sCodigo"].ToString()
                                //NVolume = Convert.ToInt32(txtnVolume.Text),
                            };
                        }).ToList();

                        // ====================================================================
                        // 3. TRAVA DE LIMITE (SALDO): Verifica se excedeu a quantidade da OPI
                        // ====================================================================
                        bool excedeuLimite = false;
                        string msgErroLimite = "";

                        foreach (var grp in novosItens.GroupBy(x => x.IdObjeto)) // IdObjeto = idProduto para PR
                        {
                            decimal qtdNova = grp.Sum(x => x.NQuantidade);
                            decimal qtdJaNaGrid = ls_VolumesItens.Where(x => x.IdObjeto == grp.Key && x.SCodigoBarras.StartsWith("PR", StringComparison.OrdinalIgnoreCase)).Sum(x => x.NQuantidade);

                            var prodRef = bs_Produto_Envios.FirstOrDefault(p => p.IdItem == grp.Key);
                            decimal saldoPermitido = prodRef != null ? prodRef.nEnviar : 0;

                            if ((qtdNova + qtdJaNaGrid) > saldoPermitido)
                            {
                                excedeuLimite = true;
                                msgErroLimite = $"Limite excedido para o produto {grp.First().SCodigoProduto}. Saldo na OPI: {saldoPermitido}. Quantidade neste volume: {(qtdNova + qtdJaNaGrid)}.";
                                break;
                            }
                        }

                        if (excedeuLimite)
                        {
                            MensagemPaginaVolume.MostraMensagem_Aviso(msgErroLimite);
                            txtsCodigoBarrasVolume.Text = "";
                        }
                        else
                        {
                            ls_VolumesItens.AddRange(novosItens);
                            dtgVolumesItensDataBind();
                            txtsCodigoBarrasVolume.Text = "";
                            txtsCodigoBarrasVolume.Focus();
                        }
                    }
                    else
                    {
                        MensagemPaginaVolume.MostraMensagem_Erro(!string.IsNullOrEmpty(sErro) || sErro != "Nenhum Registro Encontrado" ? sErro : "Código de Barras Inválido para o Produto Desejado!", true);
                    }
                }
                else if (codigoLido.StartsWith("UN"))
                {
                    if (BD.ValidarDataSet(dsVolumeItem, out sErro))
                    {
                        List<cls_WMS_VolumesItens> novosItens = dsVolumeItem.Tables[0].AsEnumerable().Select(row =>
                        {
                            return new cls_WMS_VolumesItens()
                            {
                                IdObjeto = Convert.ToInt32(row["idUnitizado"]),
                                SDscObjeto = row["sDscUnitizado"].ToString(),
                                SCodigoBarras = codigoLido,
                                //NVolume = Convert.ToInt32(txtnVolume.Text),
                            };
                        }).ToList();

                        ls_VolumesItens.AddRange(novosItens);

                        dtgVolumesItensDataBind();
                        txtsCodigoBarrasVolume.Text = "";
                    }
                    else
                    {
                        MensagemPaginaVolume.MostraMensagem_Aviso(!string.IsNullOrEmpty(sErro) || sErro != "Nenhum Registro Encontrado" ? sErro : "Código de Barras InVálido para o Unitizado Desejado!", true);
                    }
                }
                else if (codigoLido.StartsWith("VOL"))
                {
                    if (BD.ValidarDataSet(dsVolumeItem, out sErro))
                    {
                        List<cls_WMS_VolumesItens> novosItens = dsVolumeItem.Tables[0].AsEnumerable().Select(row =>
                        {
                            return new cls_WMS_VolumesItens()
                            {
                                IdObjeto = Convert.ToInt32(row["idVolume"]),
                                SDscObjeto = row["sDscVolume"].ToString(),
                                SCodigoBarras = codigoLido,
                                //NVolume = Convert.ToInt32(txtnVolume.Text),
                            };
                        }).ToList();

                        ls_VolumesItens.AddRange(novosItens);

                        dtgVolumesItensDataBind();
                        txtsCodigoBarrasVolume.Text = "";
                    }
                    else
                    {
                        MensagemPaginaVolume.MostraMensagem_Aviso(!string.IsNullOrEmpty(sErro) ? sErro : "Código de Barras InVálido para o Volume Desejado!", true);
                    }
                }
                else
                {
                    MensagemPaginaVolume.MostraMensagem_Erro("Código de Barras Inválido", true);
                }
                txtsCodigoBarrasVolume.Text = "";
            }

            if (abrir)
                AbrirCameraVolume_Click(sender, e);

            AbrirModal_Click(sender, e);
        }

        protected void GerarVolume_Click(object sender, EventArgs e)
        {
            if (Atualizar_VolumesItensClasse() && ValidarDadosVolumes())
            {
                string sErro = "";
                try
                {
                    string[] vidOPI = hddidOPI.Value.Split(',');
                    string idOPI = vidOPI[0].ToString();
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR-VOLUME");
                    txtidOPI.Text = idOPI;
                    vParametros.Add("@idVolume", string.IsNullOrEmpty(hddidVolumeUpdt.Value) ? "0" : hddidVolumeUpdt.Value); // <-- MÁGICA AQUI
                    vParametros.Add("@idOPI", txtidOPI.Text);
                    vParametros.Add("@idProdutoEmbalagem", ddlEmbalagem.SelectedValue);
                    //vParametros.Add("@sDscVolume", "Volume Teste");
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    //vParametros.Add("@sUnitizado", ddlsUnitizado.SelectedValue);

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {

                        hddidVolume.Value = RETORNO.DATASET(dsSalvar, 0, "idVolume");

                        VolumesItens_Salvar(hddidVolume.Value);
                        PopularVolumes();
                        MensagemPaginaVolume.MostraMensagem_Sucesso("Registro gravado com sucesso", false);
                        LimpaCamposVolumes();
                        PopularEtiquetas(hddidOPI.Value);
                    }
                    else
                    {
                        MensagemPaginaVolume.MostraMensagem_Erro("BD: " + sErro.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MensagemPaginaVolume.MostraMensagem_Erro(ex.Message, false);
                }
            }
            AbrirModal_Click(sender, e);
        }

        protected void VolumesItens_Salvar(string idVolume)
        {
            // ETAPA 1: Buscar itens que já estão no banco para esse volume
            Dictionary<string, string> vParametrosBusca = new Dictionary<string, string>();
            vParametrosBusca.Add("@sFuncao", "CONSULTAR-VOLUMES-ITENS");
            vParametrosBusca.Add("@idVolume", idVolume);

            DataSet dsBanco = BD.ExecutarDataSet(sProcedure, vParametrosBusca);
            List<FrameWork.cls_WMS_VolumesItens> itensNoBanco = new List<FrameWork.cls_WMS_VolumesItens>();

            if (BD.ValidarDataSet(dsBanco))
            {
                itensNoBanco = conversorVolumes.ConverterDataSet(dsBanco, "Table");
            }

            // AQUI ESTÁ A CORREÇÃO:
            // Deletamos TUDO que estava no banco para esse volume. 
            // Assim, limpamos o terreno e é impossível duplicar o que ficou para trás.
            foreach (var itemBanco in itensNoBanco)
            {
                Dictionary<string, string> vParametrosDel = new Dictionary<string, string>();
                vParametrosDel.Add("@sFuncao", "EXCLUIR-ITEM-VOLUME");
                vParametrosDel.Add("@idVolume", idVolume);
                vParametrosDel.Add("@sCodigoBarras", itemBanco.SCodigoBarras);

                BD.ExecutarDataSet(sProcedure, vParametrosDel);
            }

            // ETAPA 2: Salvar os itens da Grid (Insert limpo com os dados atualizados)
            if (ls_VolumesItens.Count > 0)
            {
                Dictionary<string, string> vParametrosItens = new Dictionary<string, string>();
                ls_VolumesItens.ForEach(a =>
                {
                    vParametrosItens.Clear();
                    vParametrosItens.Add("@sFuncao", "INCLUIR-VOLUMES-ITENS");
                    vParametrosItens.Add("@idVolume", idVolume);
                    vParametrosItens.Add("@idObjeto", a.IdObjeto.ToString());
                    vParametrosItens.Add("@nVolume", a.NVolume.ToString());
                    vParametrosItens.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().ToString());
                    vParametrosItens.Add("@sCodigoBarras", a.SCodigoBarras);
                    vParametrosItens.Add("@sDscObjeto", a.SDscObjeto);

                    BD.ExecutarDataSet(sProcedure, vParametrosItens);
                });

                ls_VolumesItens.Clear();
                dtgVolumesItensDataBind();
                txtsCodigoBarrasVolume.Text = "";
                hddidVolumeUpdt.Value = ""; // Reseta a edição após salvar
            }
            else
            {
                MensagemPaginaVolume.MostraMensagem_Aviso("O Volume foi Salvo sem Itens!", false);
            }
        }

        #region | Controles Front-End Volumes
        protected void AbrirVolumes_Click(object sender, EventArgs e)
        {
            MultiViewFormularios.ActiveViewIndex = 0;
            AtualizarBarraProgresso();
            lbltituloModal.Text = "Volumes";

            AbrirModal_Click(sender, e);
        }
        protected void AbrirCameraVolume_Click(object sender, EventArgs e)
        {
            bool abrir = (bool)Session["CameraAbertaVolume"];

            abrir = !abrir;

            Session["CameraAbertaVolume"] = abrir;

            DivBipadorVolume.Visible = abrir;

            cmdAbrirCamVolume.Text = abrir ? "Fechar Leitor" : "Abrir Leitor";
            if (cmdAbrirCamVolume.Text == "Fechar Leitor")
            {
                LeitorQuagga.AbrirCameraVariante();
                divCodigoB.Style["Display"] = "none";
            }
            else
            {
                LeitorQuagga.DesligarCamVariante();
                divCodigoB.Style["Display"] = "block";
            }
            AbrirModal_Click(sender, e);
        }
        protected void AbrirArquivosVolumes_Click(object sender, EventArgs e)
        {
            LinkButton cmdArquivo = (LinkButton)sender;
            string idVolume = cmdArquivo.CommandArgument;

            MultiViewFormularios.ActiveViewIndex = 1;
            lbltituloModal.Text = "Fotos do Volume";
            Popular_aba_ArquivoVolume(idVolume);
            AbrirModal_Click(sender, e);
        }

        protected void ImportarEtiquetas_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR-ETIQUETAS-PENDENTES");
                vParametros.Add("@idOPI", hddidOPI.Value);

                DataSet dsEtiquetas = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsEtiquetas))
                {
                    int contagemAdicionados = 0;

                    List<FrameWork.cls_WMS_VolumesItens> novosItens = dsEtiquetas.Tables[0].AsEnumerable()
                        .Select(row => new FrameWork.cls_WMS_VolumesItens()
                        {
                            IdObjeto = Convert.ToInt32(row["idProduto"]),
                            SCodigoProduto = row["sCodigo"].ToString(),
                            SDscObjeto = row["sDscProduto"].ToString(),
                            NQuantidade = Convert.ToDecimal(row["nQuantidade"]),
                            SCodigoBarras = row["sCodigoBarras"].ToString(),
                        }).ToList();

                    foreach (var item in novosItens)
                    {
                        if (!ls_VolumesItens.Any(x => x.SCodigoBarras == item.SCodigoBarras))
                        {
                            ls_VolumesItens.Add(item);
                            contagemAdicionados++;
                        }
                    }

                    if (contagemAdicionados > 0)
                    {
                        dtgVolumesItensDataBind();
                        MensagemPaginaVolume.MostraMensagem_Sucesso($"{contagemAdicionados} etiquetas importadas com sucesso!", false);
                    }
                    else
                    {
                        MensagemPaginaVolume.MostraMensagem_Aviso("Todas as etiquetas pendentes já estão na lista.", false);
                    }
                }
                else
                {
                    MensagemPaginaVolume.MostraMensagem_Aviso("Não há etiquetas pendentes de envio para importar.", false);
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaVolume.MostraMensagem_Erro("Erro ao importar etiquetas: " + ex.Message, false);
            }

            bool abrir = (bool)Session["CameraAbertaVolume"];

            if (abrir)
                AbrirCameraVolume_Click(sender, e);

            AbrirModal_Click(sender, e);
        }
        #endregion

        #region | DataGrid VolumeGeral
        void PopularVolumes()
        {
            DataSet dsVolumes = new DataSet();
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR-VOLUMES-OPCOES");
            vParametros.Add("@idOPI", hddidOPI.Value);
            dsVolumes = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsVolumes))
            {
                div_VolumesSalvos.Visible = true;
                dtgVolumesDataBind(dsVolumes);
                //ls_unitizadosItensGV.Clear();
            }
            else
            {
                dtgVolumes.DataSource = new List<object>();
                dtgVolumes.DataBind();
            }

        }
        void dtgVolumesDataBind(DataSet dsVolumes)
        {
            dtgVolumes.DataSource = dsVolumes;
            dtgVolumes.DataBind();
            RegistrarColapsoScript();
        }
        protected void dtgVolume_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string id = dtgVolumes.DataKeys[e.Row.RowIndex].Value.ToString();
                var dtgVolumesItensGV = (GridView)e.Row.FindControl("dtgVolumesItensGV");
                ConsultaVolumesItens(id);
                dtgVolumesItensGV.DataSource = ls_VolumesItensGV;
                dtgVolumesItensGV.DataBind();
            }
        }

        protected void dtgVolumeItem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridView dtgVolumesItensGV = (GridView)sender;
                LinkButton btnToggle = (LinkButton)e.Row.FindControl("btnToggle");

                if (dtgVolumesItensGV != null)
                {
                    string idObjeto = dtgVolumesItensGV.DataKeys[e.Row.RowIndex].Value.ToString();

                    var dtgItensVL = (GridView)e.Row.FindControl("dtgItensVL");
                    ConsultaItensVL(idObjeto);
                    dtgItensVL.DataSource = ls_VolumesProdutosGV;
                    dtgItensVL.DataBind();
                }
            }
        }
        void ConsultaVolumesItens(string id)
        {
            string sFuncao = "CONSULTAR-VOLUMES-ITENS";
            DataSet dsVolumesItens;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idVolume", id);

            dsVolumesItens = BD.ExecutarDataSet(sProcedure, vParametros, false);

            ls_VolumesItensGV = conversorVolumes.ConverterDataSet(dsVolumesItens, "Table");
        }
        void ConsultaItensVL(string id)
        {
            string sFuncao = "CONSULTAR-UNITIZADOS-ITENS-OPI";
            DataSet dsProdutos;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idUnitizado", id);

            dsProdutos = BD.ExecutarDataSet(sProcedure, vParametros, false);

            ls_VolumesProdutosGV = conversorProdutos.ConverterDataSet(dsProdutos, "Table");
        }
        #endregion

        #region | Atualizar Classe

        bool Atualizar_VolumesItensClasse()
        {
            if (ls_VolumesItens.Count < dtgVolumesItens.Rows.Count)
            {
                MensagemPaginaVolume.MostraMensagem_Erro("Erro de sincronia na lista. Tente recarregar a página.", false);
                return false;
            }

            for (int i = 0; i < dtgVolumesItens.Rows.Count; i++)
            {
                GridViewRow item = dtgVolumesItens.Rows[i];

                if (item.RowType == DataControlRowType.DataRow)
                {
                    TextBox txtnVolumeGV = (TextBox)item.FindControl("txtnVolumeGV");

                    if (txtnVolumeGV == null) continue;

                    int valorDigitado = 0;

                    bool ehNumero = int.TryParse(txtnVolumeGV.Text, out valorDigitado);

                    if (ehNumero && valorDigitado > 0)
                    {
                        ls_VolumesItens[i].NVolume = valorDigitado;
                    }
                    else
                    {
                        ls_VolumesItens[i].NVolume = 0;
                        //MensagemPaginaVolume.MostraMensagem_Aviso("Lembrete: A Ordem de alguns itens ficou como Zero.", false);
                    }
                }
            }
            return true;
        }
        #endregion

        protected void EditarVolume_Click(object sender, EventArgs e)
        {
            LinkButton cmdEditar = (LinkButton)sender;
            string idVolume = cmdEditar.CommandArgument;

            hddidVolumeUpdt.Value = idVolume;

            // 1. Tentar puxar a Embalagem salva (se a consulta trouxer a coluna)
            DataSet dsVolumes = BD.ExecutarDataSet(sProcedure, new Dictionary<string, string> { { "@sFuncao", "CONSULTAR-VOLUMES-OPCOES" }, { "@idOPI", hddidOPI.Value } });
            if (BD.ValidarDataSet(dsVolumes) && dsVolumes.Tables[0].Columns.Contains("idProdutoEmbalagem"))
            {
                DataRow[] rows = dsVolumes.Tables[0].Select("idVolume = " + idVolume);
                if (rows.Length > 0 && rows[0]["idProdutoEmbalagem"] != DBNull.Value)
                {
                    ddlEmbalagem.SelectedValue = rows[0]["idProdutoEmbalagem"].ToString();
                }
            }

            // 2. Puxar os Itens para a Grid de Bipagem
            ConsultaVolumesItens(idVolume); // Aproveitamos a função que já existe e alimenta o ls_VolumesItensGV

            ls_VolumesItens.Clear();
            foreach (var itemGV in ls_VolumesItensGV)
            {
                ls_VolumesItens.Add(new cls_WMS_VolumesItens()
                {
                    IdObjeto = itemGV.IdObjeto,
                    SDscObjeto = itemGV.SDscObjeto,
                    SCodigoBarras = itemGV.SCodigoBarras,
                    NQuantidade = itemGV.NQuantidade,
                    SCodigoProduto = itemGV.SCodigoProduto,
                    NVolume = itemGV.NVolume
                });
            }
            dtgVolumesItensDataBind();

            // 3. Trocar a View e Abrir Modal
            MultiViewFormularios.ActiveViewIndex = 0;
            lbltituloModal.Text = "Editar Volume";
            btnAbaConfig.CssClass = "nav-link activeMn"; // Ativa a aba de configuração visualmente

            AbrirModal_Click(sender, e);
        }
        #endregion

        #region | Controle de Modal
        protected void AbrirModal_Click(object sender, EventArgs e)
        {
            Pesquisar(hddidOPI.Value, false);
            try
            {
                PopularImpressora();
                ddlImpressora.SelectedValue = idImpressoraPadrao;
                dtgVolumesItensDataBind();
                updModal.Update();
            }
            catch (Exception ex)
            {
                MensagemPaginaAbaEnvios.MostraMensagem_Erro(ex.ToString());
            }


            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalEnvio", "$('#modalEnvio').modal('show');", true);
        }

        protected void btnAnterior_Click(object sender, EventArgs e)
        {
            if (MultiViewFormularios.ActiveViewIndex > 0)
            {
                MultiViewFormularios.ActiveViewIndex--;
            }
            AbrirModal_Click(sender, e);
        }

        protected void btnProximo_Click(object sender, EventArgs e)
        {
            if (MultiViewFormularios.ActiveViewIndex < MultiViewFormularios.Views.Count - 1)
            {
                MultiViewFormularios.ActiveViewIndex++;
            }

            AbrirModal_Click(sender, e);
        }
        protected void VoltarModal_Click(object sender, EventArgs e)
        {
            MultiViewFormularios.ActiveViewIndex = 0;
            AbrirModal_Click(sender, e);
        }
        protected void AtualizarBarraProgresso()
        {
            int totalEtapas = MultiViewFormularios.Views.Count - 1;
            int etapaAtual = MultiViewFormularios.ActiveViewIndex + 1;
            int porcentagemConcluida = (etapaAtual * 100) / totalEtapas;
            RegistraScriptCamposPeso();
        }
        #endregion

        #region | validação Codigo de Barras
        [WebMethod]
        public static void CodigoBarras(string codigoBarras)
        {
            Page currentPage = HttpContext.Current.Handler as Page;
            if (currentPage != null)
            {
                OPI_Detalhe pageInstance = currentPage as OPI_Detalhe;
                if (pageInstance != null)
                {
                    pageInstance.ValidaCodigoBarras(codigoBarras);
                }
            }
        }

        public void ValidaCodigoBarras(string codigoBarras)
        {
            string sErro = "";
            DataSet dsProduto = new DataSet();
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR-CODIGO-BARRAS");
            vParametros.Add("@sCodigoBarras", codigoBarras);

            dsProduto = BD.ExecutarDataSet(sProcedure, vParametros);

            // AQUI: Lê o sErro do banco
            if (BD.ValidarDataSet(dsProduto, out sErro))
            {
                DivBipador.Visible = false;
                MensagemPaginaUnItens.MostraMensagem_Erro("Código de Barras Validado Com Sucesso!", true);
            }
            else
            {
                // AQUI: Exibe o erro específico ou o genérico
                MensagemPaginaUnItens.MostraMensagem_Erro(!string.IsNullOrEmpty(sErro) ? sErro : "Código de Barras Inválido!", true);
            }
        }
        #endregion

        #region | Arquivos
        //Unitizados
        void Popular_aba_ArquivoUnitizado(string idUnitizado)
        {
            //Popular_aba_ArquivoUnitizado(hddidOPI.Value);           
            frmArquivos.Attributes.Add("src", $"~/app/Paginas/Arquivos.aspx?idObjeto={idUnitizado}&sTipoObjeto={"Unitizado"}");
        }
        //Volumes
        void Popular_aba_ArquivoVolume(string idVolume)
        {
            frmArquivos2.Attributes.Add("src", $"~/app/Paginas/Arquivos.aspx?idObjeto={idVolume}&sTipoObjeto={"Volume"}");
        }
        #endregion

        #region | Método de Gerar Linha na Grid
        public String NovaLinha(object id, string gridNome)
        {
            /* 
            * 1. Fecha a célula atual
            * 2. Fecha a linha Atual
            * 3. Cria uma nova linha com o ID e a classe <TR id='...' style='...'>
            * 4. Cria uma célula em branco: <TD></TD>
            * 5. Cria uma nova célula para conter o gridview dtgUnitizadosItens
            ************************************************************/
            if (id != null && !string.IsNullOrEmpty(id.ToString()))
            {
                // Se houver um ID, retorna a nova linha com o ID e a classe
                return string.Format(@"</td></tr><tr id='tr{0}{1}' class='collapsed-row'>
                               <td></td><td colspan='100' style='padding:0px; margin:0px;'>", gridNome, id);
            }
            else
            {
                // Se não houver ID, retorna uma string vazia para que nada seja renderizado
                // e o botão de colapso desapareça
                return string.Empty;
            }
        }
        #endregion

        #region | Exportar Excel
        protected void ExportarExcel_Click(object sender, EventArgs e)
        {
            EXC_OPI();
        }
        void EXC_OPI()
        {
            try
            {
                Microsoft.Reporting.WebForms.ReportViewer rv4 = new Microsoft.Reporting.WebForms.ReportViewer();

                rv4.ProcessingMode = ProcessingMode.Local;
                rv4.LocalReport.EnableExternalImages = true;

                rv4.LocalReport.ReportPath = Server.MapPath("~/App/Reports/EnvioOPI.rdlc");

                string sFuncao = "CONSULTAR-ITENS";

                DataSet dsItensExcel;
                string sSql = "sp_Manipula_FLow_WMS_OPI";
                Dictionary<String, String> vParametrosItens = new Dictionary<string, string>();
                vParametrosItens.Add("@sFuncao", sFuncao);
                vParametrosItens.Add("@idOPI", hddidOPI.Value);

                dsItensExcel = BD.ExecutarDataSet(sSql, vParametrosItens);
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@idOPI", hddidOPI.Value);
                vParametros.Add("@sFuncao", "CONSULTAR");

                //if(selectedEnvioIds != null)
                //{
                //    EXC_ENVIO(selectedEnvioIds);
                //}                      

                DataSet dsOPI;
                dsOPI = BD.ExecutarDataSet("sp_Manipula_FLow_WMS_OPI", vParametros);

                if (dsOPI.Tables.Count == 0 || dsOPI.Tables[0].Rows.Count == 0)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao Gerar Excel: Nenhum dado encontrado.");
                    return;
                }
                if (dsItensExcel.Tables.Count == 0 || dsItensExcel.Tables[0].Rows.Count == 0)
                {
                    MensagemPagina.MostraMensagem_Aviso("Sem Itens Nessa OPI");
                }


                rv4.LocalReport.DataSources.Clear();
                rv4.LocalReport.DataSources.Add(new ReportDataSource("dsOPI", dsOPI.Tables[0]));
                rv4.LocalReport.DataSources.Add(new ReportDataSource("dsItens", dsItensExcel.Tables[0]));
                DataRow row = dsOPI.Tables[0].AsEnumerable().FirstOrDefault();
                DataRow row2 = dsItensExcel.Tables[0].AsEnumerable().FirstOrDefault();

                if (row != null)
                {
                    ReportParameter[] rp = new ReportParameter[5];

                    // Substituindo com os dados das colunas especificadas
                    rp[0] = new ReportParameter("idOPI", row["idOPI"].ToString());
                    rp[1] = new ReportParameter("sReferencia", row["sReferencia"].ToString());
                    rp[2] = new ReportParameter("sCliente", row["sCliente"].ToString());
                    rp[3] = new ReportParameter("sStatus", row["sStatus"].ToString());
                    rp[4] = new ReportParameter("dtOPI", row["dtOPI"].ToString());

                    rv4.LocalReport.SetParameters(rp);
                    rv4.LocalReport.Refresh();

                    Microsoft.Reporting.WebForms.Warning[] warnings;
                    string[] streamIds;
                    string mimeType, encoding, extension;

                    byte[] bytes = rv4.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                    string sNomeArquivoOriginal = "PL_" + dsOPI.Tables[0].Rows[0]["sReferencia"].ToString() + "_" + FUNCOES.CarimboDataHora() + ".xls";
                    string sNomeArquivo = LimparNomeArquivo(sNomeArquivoOriginal);

                    File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);
                    Pesquisar(hddidOPI.Value, false);
                    FUNCOES.DownloadArquivo(Page, sNomeArquivo);
                    // Mostrar mensagem de sucesso
                    MensagemPagina.MostraMensagem_Sucesso("Excel da OPI gerada com sucesso!");
                }
                else
                {
                    // Lidar com o caso onde não há dados
                    MensagemPagina.MostraMensagem_Erro("Erro ao Gerar Excel");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao gerar o Excel da OPI! </br>" + ex.Message);
            }
        }
        protected void GerarExcelEnvio_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string idEnvioOPI = btn.CommandArgument;
            EXC_ENVIO(idEnvioOPI);
        }
        void EXC_ENVIO(string idEnvio)
        {
            try
            {
                Microsoft.Reporting.WebForms.ReportViewer rv4 = new Microsoft.Reporting.WebForms.ReportViewer();

                rv4.ProcessingMode = ProcessingMode.Local;
                rv4.LocalReport.EnableExternalImages = true;

                rv4.LocalReport.ReportPath = Server.MapPath("~/App/Reports/EnviosVolumes.rdlc");
                DataSet dsEnvios = new DataSet();

                Dictionary<string, string> vParametrosEnvios = new Dictionary<string, string>();
                vParametrosEnvios.Add("@sFuncao", "CONSULTAR-ENVIOS");
                vParametrosEnvios.Add("@idEnvioOPI", idEnvio);

                // Executar a consulta
                dsEnvios = BD.ExecutarDataSet("sp_Manipula_FLow_WMS_OPI", vParametrosEnvios);

                if (dsEnvios.Tables.Count == 0 || dsEnvios.Tables[0].Rows.Count == 0)
                {
                    MensagemPaginaAbaEnvios.MostraMensagem_Aviso("Não foi encontrado dados nesse envio");
                    return;
                }

                DataSet dsVolumes = new DataSet();

                Dictionary<string, string> vParametrosVolumes = new Dictionary<string, string>();
                vParametrosVolumes.Add("@sFuncao", "CONSULTAR-VOLUMES-EXCEL");
                vParametrosVolumes.Add("@idEnvioOPI", idEnvio);

                // Executar a consulta
                dsVolumes = BD.ExecutarDataSet("sp_Manipula_FLow_WMS_OPI", vParametrosVolumes);

                if (dsVolumes.Tables.Count == 0 || dsVolumes.Tables[0].Rows.Count == 0)
                {
                    MensagemPaginaAbaEnvios.MostraMensagem_Aviso("Não foi encontrado dados nesse envio");
                    return;
                }

                DataSet dsFotosVolumes = new DataSet();

                Dictionary<string, string> vParametrosFotos = new Dictionary<string, string>();
                vParametrosFotos.Add("@sFuncao", "CONSULTAR-IMAGEM-VOLUME");
                vParametrosFotos.Add("@idEnvioOPI", idEnvio);

                // Executar a consulta
                dsFotosVolumes = BD.ExecutarDataSet("sp_Manipula_FLow_WMS_OPI", vParametrosFotos);

                rv4.LocalReport.DataSources.Clear();
                rv4.LocalReport.DataSources.Add(new ReportDataSource("dsEnvio", dsEnvios.Tables[0]));
                rv4.LocalReport.DataSources.Add(new ReportDataSource("dsVolumesUnitizados", dsVolumes.Tables[0]));
                rv4.LocalReport.DataSources.Add(new ReportDataSource("dsFotosVolumes", dsFotosVolumes.Tables[0]));
                Microsoft.Reporting.WebForms.Warning[] warnings;
                string[] streamIds;
                string mimeType, encoding, extension;

                byte[] bytes = rv4.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                string dataSemBarras = dsEnvios.Tables[0].Rows[0]["dtEnvio"].ToString().Replace("/", "");

                string sNomeArquivoOriginal = "Envio_E" + dsEnvios.Tables[0].Rows[0]["idEnvioOPI"].ToString() + "DTENV" + dataSemBarras + "_" + FUNCOES.CarimboDataHora() + ".xls";
                string sNomeArquivo = LimparNomeArquivo(sNomeArquivoOriginal);
                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);

                Pesquisar(hddidOPI.Value, false);
                FUNCOES.DownloadArquivo(Page, sNomeArquivo);
                MensagemPaginaAbaEnvios.MostraMensagem_Sucesso("Excel do Envio gerado com sucesso!");
            }
            catch (Exception ex)
            {
                MensagemPaginaAbaEnvios.MostraMensagem_Erro("Erro ao gerar o Excel do Envio! </br>" + ex.Message);
            }
        }
        public string LimparNomeArquivo(string nomeArquivo)
        {
            char[] caracteresInvalidos = System.IO.Path.GetInvalidFileNameChars();
            char[] caracteresInvalidosDiretorio = System.IO.Path.GetInvalidPathChars();

            List<char> caracteresValidos = new List<char>();

            foreach (char c in nomeArquivo)
            {
                if (!caracteresInvalidos.Contains(c) && !caracteresInvalidosDiretorio.Contains(c))
                {
                    caracteresValidos.Add(c);
                }
                else
                {
                    caracteresValidos.Add('_');
                }
            }

            return new string(caracteresValidos.ToArray());
        }
        #endregion

        #region | Pesquisa Manual de Produtos (TextBox)
        protected void chkPesquisaManualVolume_CheckedChanged(object sender, EventArgs e)
        {
            bool isManual = chkPesquisaManualVolume.Checked;
            divCodigoBarrasVolumeWrapper.Visible = !isManual;
            divPesquisaManualVolume.Visible = isManual;
            AbrirModal_Click(sender, e);
        }

        protected void chkPesquisaManualUnitizado_CheckedChanged(object sender, EventArgs e)
        {
            bool isManual = chkPesquisaManualUnitizado.Checked;
            divCodigoBarrasUnitizadoWrapper.Visible = !isManual;
            divPesquisaManualUnitizado.Visible = isManual;
            ManterModalAberta();
        }

        [WebMethod]
        public static object[] BuscarProdutosOpiAutocomplete(string termo, string idOpi)
        {
            List<object> lstProdutos = new List<object>();

            if (termo.Length >= 2 && !string.IsNullOrEmpty(idOpi))
            {
                try
                {
                    string termoLimpo = termo.Replace("'", "");

                    string query = $@"
                        WITH cte_Enviado AS (
                            SELECT 
                                ISNULL(ui.idProduto, vi.idObjeto) AS idProduto, 
                                SUM(CASE 
                                    WHEN LEFT(vi.sCodigoBarras, 2) = 'UN' THEN ui.nQuantidade
                                    WHEN LEFT(vi.sCodigoBarras, 3) = 'VOL' THEN ui.nQuantidade
                                    WHEN vi.sCodigoBarras = p.sCodigo THEN 1
                                    ELSE ep.nQuantidade
                                END) AS nTotalEnviado
                            FROM tbl_Flow_WMS_OPI_Envios e (NOLOCK) 
                            INNER JOIN tbl_Flow_WMS_OPI_Envios_Volumes ve (NOLOCK) ON ve.idEnvioOPI = e.idEnvioOPI 
                            INNER JOIN tbl_Flow_WMS_OPI_VolumeItens vi (NOLOCK) ON vi.idVolume = ve.idVolume
                            LEFT JOIN tbl_Flow_WMS_OPI_UnitizadosItens ui (NOLOCK) ON vi.idObjeto = ui.idUnitizado AND (LEFT(vi.sCodigoBarras, 2) = 'UN' OR LEFT(vi.sCodigoBarras, 3) = 'VOL')
                            LEFT JOIN tbl_Flow_WMS_OPI_Etiqueta ep (NOLOCK) ON vi.sCodigoBarras = ep.sCodigoBarras
                            LEFT JOIN tbl_Flow_Produtos p (NOLOCK) ON p.idItem = vi.idObjeto
                            WHERE e.idOPI = {Convert.ToInt32(idOpi)}
                            GROUP BY ISNULL(ui.idProduto, vi.idObjeto)
                        ),
                        cte_Pedido AS (
                            SELECT idProduto, SUM(nQuantidade) as nSolicitado
                            FROM tbl_Flow_WMS_OPI_Itens (NOLOCK)
                            WHERE idOPI = {Convert.ToInt32(idOpi)}
                            GROUP BY idProduto
                        )
                        SELECT 
                            p.idItem, 
                            p.sCodigo, 
                            p.sDscProduto, 
                            p.sCodigoEAN, 
                            (ISNULL(cp.nSolicitado, 0) - ISNULL(ce.nTotalEnviado, 0)) AS nEnviar
                        FROM cte_Pedido cp
                        INNER JOIN tbl_Flow_Produtos p (NOLOCK) ON cp.idProduto = p.idItem
                        LEFT JOIN cte_Enviado ce ON cp.idProduto = ce.idProduto
                        WHERE (p.sCodigo LIKE '%{termoLimpo}%' 
                               OR p.sCodigoEAN LIKE '%{termoLimpo}%' 
                               OR p.sDscProduto LIKE '%{termoLimpo}%')
                    ";

                    SqlDataReader sdr = BD.ExecutarDataReader(query);

                    if (sdr != null)
                    {
                        while (sdr.Read())
                        {
                            decimal saldo = Convert.ToDecimal(sdr["nEnviar"]);
                            string label = $"{sdr["sCodigo"]} - {sdr["sDscProduto"]}";

                            if (sdr["sCodigoEAN"] != DBNull.Value && !string.IsNullOrEmpty(sdr["sCodigoEAN"].ToString()))
                            {
                                label += $" (EAN: {sdr["sCodigoEAN"]})";
                            }

                            if (saldo <= 0)
                            {
                                lstProdutos.Add(new
                                {
                                    label = label, // Texto LIMPO que vai para o TextBox e Banco de Dados
                                    exibicao = "🚫 " + label + " (Limite atingido)", // Texto COM EMOJI que vai aparecer na lista suspensa
                                    value = "ESGOTADO"
                                });
                            }
                            else
                            {
                                // PRODUTO DISPONÍVEL (Verde)
                                lstProdutos.Add(new
                                {
                                    label = label, // Texto LIMPO que vai para o TextBox e Banco de Dados
                                    exibicao = "✅ " + label, // Texto COM EMOJI para a lista suspensa
                                    value = sdr["idItem"].ToString()
                                });
                            }
                        }

                        if (!sdr.IsClosed) { sdr.Close(); }
                    }
                }
                catch
                {
                    return lstProdutos.ToArray();
                }
            }

            return lstProdutos.ToArray();
        }
        #endregion


        #region | Exclusão Geral (Volume e Unitizado)
        protected void ExcluirUnitizadoGeral_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string idUnitizado = btn.CommandArgument;

            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "EXCLUIR-UNITIZADO");
                vParametros.Add("@idUnitizado", idUnitizado);

                BD.ExecutarDataSet(sProcedure, vParametros);

                MensagemPaginaUnitizados.MostraMensagem_Sucesso("Unitizado excluído e itens desvinculados com sucesso!", false);

                // Recarrega a Grid de Unitizados
                PopularUnitizados();
            }
            catch (Exception ex)
            {
                MensagemPaginaUnitizados.MostraMensagem_Erro("Erro ao excluir Unitizado: " + ex.Message, false);
            }

            ManterModalAberta(); // Mantém a modal aberta para o usuário continuar trabalhando
        }

        protected void ExcluirVolumeGeral_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string idVolume = btn.CommandArgument;

            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "EXCLUIR-VOLUME");
                vParametros.Add("@idVolume", idVolume);

                BD.ExecutarDataSet(sProcedure, vParametros);

                MensagemPaginaVolume.MostraMensagem_Sucesso("Volume desfeito e itens desvinculados com sucesso!", false);

                // Recarrega a Grid de Volumes
                PopularVolumes();
            }
            catch (Exception ex)
            {
                MensagemPaginaVolume.MostraMensagem_Erro("Erro ao excluir Volume: " + ex.Message, false);
            }

            AbrirModal_Click(sender, e); // Como Volumes usa outra função para manter aberto, chamamos ela
        }
        #endregion
    }
}