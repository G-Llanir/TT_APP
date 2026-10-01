using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.FrameWork;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Services;
using System.Data.Common.CommandTrees.ExpressionBuilder;
using System.Web;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace TT_Flow.App.Paginas.WMS
{
    public partial class OPI_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Ordem de Produção Interna";
        string sProcedure = "sp_Manipula_FLow_WMS_OPI";
        string sProcedureEtiqueta = "sp_Manipula_tbl_Flow_WMS_OPI_Etiqueta";
        string sProcedureLocal = "sp_Manipula_tbl_Flow_WMS_LocalArmazenamento";
        bool isInclusao = false;
        string idImpressoraPadrao = "0";

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
                ViewState["ls_Produtos"] = value;
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
            div_botoesVolumes.Visible = false;
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
                //PopularVolumes();
                //PopularItens(Request["id"]);
                PopularEnviosItens();
                PopularCombos();
                PopularUnitizados();
                PopularVolumes();
                PopularItens(Request["id"]);

                if (isInclusao == false)
                {
                    PopularEtiquetas(hddidOPI.Value);
                    cmdSalvarEtiquetas.Visible = false;
                    cmdIncluirEtiqueta.Visible = false;
                    div_botoesUnitizado.Visible = false;
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

                if (MultiViewFormularios.ActiveViewIndex == 2)
                    div_botoesVolumes.Visible = true;
                else
                    div_botoesVolumes.Visible = false;
            }
            RegistraScript("");
            RegisterQuaggaLibrary();
            //RegistraQuaggaScript();
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
                //cmdEfetuarEnvio.Visible = false;

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

                        //Popular_aba_Arquivo(idOPI);
                        //PopularItens(hddidOPI.Value);

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
        void LimpaCampos()
        {
            hddidOPI.Value = "0";
            txtidOPI.Text = "Novo";

            txtsDscOPI.Text = "";

            PainelAtualizacao.Visible = false;
            lblTituloPagina.Text = sTituloPagina;
            //aba_Arquivo.Visible = false;

        }
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
            //if (ddlEmbalagem.SelectedValue == "0")
            //{
            //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um Tipo de Embalagem";
            //}
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
            //if (string.IsNullOrEmpty(txtNQtdVolumes.Text))
            //{
            //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe a Quantidade de Volume";
            //}
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

                MensagemPaginaEnvios.MostraMensagem_Erro(sMensagemErro, false);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalEnvio", "$('#modalEnvio').modal('show');", true);
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
            FUNCOES.Popula_Combo(ddlLocal, "sp_Manipula_tbl_Flow_Produtos_Movimentacao 'FLOW_LOCAL_ARMAZENAMENTO'", "idLocalArmazenamento", "sDscLocalArmazenamento", false, "Selecione o Local", "0");
            PopularImpressora();
        }
        void PopularImpressora()
        {
            FUNCOES.Popula_Combo(ddlImpressora, "sp_Manipula_tbl_Flow_WMS_OPI_Etiqueta 'FLOW-IMPRESSORA'", "idImpressora", "sDscImpressora", false, "Selecione a Impressora", "0");
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
        void RegistraQuaggaScript()
        {
            string script = @"
    <script>
        function iniciarQuagga() {
            Quagga.init({
                inputStream: {
                    name: 'Live',
                    type: 'LiveStream',
                    target: document.querySelector('#camera')    // Ou '#seuElemento' (opcional)
                },
                decoder: {
                    readers: ['code_128_reader','ean_reader']
                }
            }, function (err) {
                if (err) {
                    console.log(err);
                    return;
                }
                console.log('Inicialização concluída. Pronto para começar');
                Quagga.start();

            // Definir as dimensões do vídeo após a inicialização do Quagga
            var videoElement = document.querySelector('#camera video');
            if (videoElement) {
                videoElement.style.width = '98%';
            }
                // Aplicar estilos ao elemento <canvas> após a inicialização do Quagga
                var canvasElement = document.querySelector('#camera canvas');
                if (canvasElement) {
                canvasElement.removeAttribute('width');
                canvasElement.style.height = '1px'
                    // Outros estilos que você deseja aplicar ;
                }
            });

            Quagga.onDetected(function (data) {
                console.log('escaneado!');
                console.log(data.codeResult.code);
                document.querySelector('#resultado').innerText = data.codeResult.code;

              // Enviar o valor para o TextBox
            var codigoBarras = data.codeResult.code;
            var textBox = document.getElementById('cphCorpo_txtsCodigoBarras');
            if (textBox) {
                textBox.value = codigoBarras;
            } else {
                console.log('TextBox não encontrado.');
            }

 // Delay
    setTimeout(function() {
        // Chamar o método de validação
        $.ajax({
            type: ""POST"",
            url: ""/app/Paginas/WMS/OPI_Detalhe.aspx/CodigoBarras"",
            data: JSON.stringify({ codigoBarras: codigoBarras }),
            contentType: ""application/json; charset=utf-8"",
            dataType: ""json"",
            success: function(response) {
                console.log('Resposta do servidor: ' + response.d);
                // Manipular a resposta do servidor conforme necessário
            },
            error: function(xhr, status, error) {
                console.error('Erro na chamada AJAX: ' + error);
            }
        });
    }, 1000); // 4000 milissegundos = 1 segundo
            });
        }

        // Chamar a função de inicialização no carregamento da página
        window.onload = function () {
            iniciarQuagga();
        };
    </script>";

            // Registrar o script no cliente
            Page.ClientScript.RegisterStartupScript(GetType(), "IniciarQuaggaScript", script, false);
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
        void RegistraQuaggaScriptVolume()
        {
            string script = @"
    <script>
         //Volume
           function iniciarQuagga() {
            Quagga.init({
                inputStream: {
                    name: 'Live',
                    type: 'LiveStream',
                    target: document.querySelector('#cameraVolume')    // Ou '#seuElemento' (opcional)
                },
                decoder: {
                    readers: ['code_128_reader','ean_reader']
                }
            }, function (err) {
                if (err) {
                    console.log(err);
                    return;
                }
                console.log('Inicialização concluída. Pronto para começar');
                Quagga.start();

            // Definir as dimensões do vídeo após a inicialização do Quagga
            var videoElement = document.querySelector('#cameraVolume video');
            if (videoElement) {
                videoElement.style.width = '98%';
            }
                // Aplicar estilos ao elemento <canvas> após a inicialização do Quagga
                var canvasElement = document.querySelector('#cameraVolume canvas');
                if (canvasElement) {
                canvasElement.removeAttribute('width');
                canvasElement.style.height = '1px'
                    // Outros estilos que você deseja aplicar ;
                }
            });
                Quagga.onDetected(function (data) {
                console.log('escaneado!');
                console.log(data.codeResult.code);
                document.querySelector('#resultadoVolume').innerText = data.codeResult.code;

              // Enviar o valor para o TextBox
            var codigoBarras = data.codeResult.code;
            var textBox = document.getElementById('cphCorpo_txtsCodigoBarrasVolume');
            if (textBox) {
                textBox.value = codigoBarras;
            } else {
                console.log('TextBox não encontrado.');
            }

 // Delay
    setTimeout(function() {
        // Chamar o método de validação
        $.ajax({
            type: ""POST"",
            url: ""/app/Paginas/WMS/OPI_Detalhe.aspx/CodigoBarras"",
            data: JSON.stringify({ codigoBarras: codigoBarras }),
            contentType: ""application/json; charset=utf-8"",
            dataType: ""json"",
            success: function(response) {
                console.log('Resposta do servidor: ' + response.d);
                // Manipular a resposta do servidor conforme necessário
            },
            error: function(xhr, status, error) {
                console.error('Erro na chamada AJAX: ' + error);
            }
        });
    }, 1000); // 4000 milissegundos = 1 segundo
            });
        }

        // Chamar a função de inicialização no carregamento da página
        window.onload = function () {
            iniciarQuagga();
        };
    </script>";

            // Registrar o script no cliente
            Page.ClientScript.RegisterStartupScript(GetType(), "IniciarVolQuaggaScript", script, false);
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
        void Popular_aba_Arquivo(string idOPI)
        {
            frmArquivos.Attributes.Add("src", string.Format("~/app/Paginas/Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", idOPI, "OPI"));
            //aba_Arquivo.Visible = true;
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
                    //destrói a instancia existente
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DestroyDataTables", "if ($.fn.DataTable.isDataTable('#" + dtgItens.ClientID + "')) $('#" + dtgItens.ClientID + "').DataTable().destroy();", true);
                }
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptMinPag(dtgItens, tb, 10), true);
                // Registra o novo script para inicializar o DataTable com os novos dados
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTablesItens", TT.FrameWork.Grid.DataBindComScript(dtgItens, tb, 0, "dsc"), true);

                Pesquisar(hddidOPI.Value, false);
            }
            else
            {
                MensagemPaginaItens.MostraMensagem_Erro("Nenhum Lançamento Localizado", false);
            }
        }

        //protected void Postback_Click(object sender, EventArgs e)
        //{
        //    Pesquisar(hddidOPI.Value, false);
        //}
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
                AtualizarBarraProgresso();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTablesEnvios", TT.FrameWork.Grid.DataBindComScript(dtgvEnvios, tb), true);
            }
            else
            {
                cmdExcluir.Visible = FUNCOES.ValidaPermissao(Permissao.WMS.OPI.Excluir, true);
                //MensagemPaginaEnvios.MostraMensagem_Erro("Nenhum Lançamento Localizado");
                divTableEnvios.Visible = false;
                cmdAbrir.Visible = true;
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
                        cmdAbrir.Visible = false;
                        MensagemPagina.MostraMensagem_Aviso("OPI Cancelada, não é mais possível fazer Envios.", false);
                        Div_Envios.Visible = false;
                    }
                    else
                    {
                        cmdAbrir.Visible = false;
                        MensagemPaginaAbaEnvios.MostraMensagem_Aviso("Todos os Itens já foram enviados, Informações de enviados abaixo:", false);
                        Alterar_Status(hddidOPI.Value, "4");
                    }
                    cmdAbrir.Visible = false;
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
            div_botoesVolumes.Visible = true;
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
            MultiViewFormularios.ActiveViewIndex = 3;
            AtualizarBarraProgresso();
            lbltituloModal.Text = "Envio";
            LimparClassesAtivas();
            //cmdEnvio.CssClass += " activeMn";
            div_botoesUnitizado.Visible = false;
            AbrirModal_Click(sender, e);
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

            //div_formEtiqueta.Visible = false;
            div_ItensEtiquetas.Visible = true;
            dtgItensEtiquetas_DataBind();

            if (BD.ValidarDataSet(dsEtiquetas, out sErro))
            {
                ls_Etiquetas.Clear();

                //Usei uma atribuição usando LINQ
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
                        //SLote = row["sLote"].ToString()
                    };
                }).ToList();

                dtgEtiquetas_DataBind();
            }
            else
            {
                MensagemPaginaEtiqueta.MostraMensagem_Erro("Nenhuma Etiqueta para essa OPI!", false);
            }
        }
        protected void dtgItensEtiquetas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                System.Web.UI.WebControls.Image img = (System.Web.UI.WebControls.Image)e.Row.FindControl("imgProduto");

                TextBox txtIdProduto = (TextBox)e.Row.FindControl("txtIdItem");
                CheckBox chkItemEtiqueta = (CheckBox)e.Row.FindControl("chkOpcaoItemEtiqueta");

                if (txtIdProduto != null)
                {
                    CarregaImgProduto(txtIdProduto.Text, img);
                }
                if (chkItemEtiqueta.Checked)
                    cmdIncluirEtiqueta.Visible = true;

                // 2. CORREÇÃO: Limpar o campo de quantidade se for 0 para exibir o placeholder
                TextBox txtQtd = (TextBox)e.Row.FindControl("txtnQuantidadeEnvio");

                if (txtQtd != null)
                {
                    decimal valor;
                    // Tenta converter. Se for numérico e igual a 0, limpa o campo.
                    if (decimal.TryParse(txtQtd.Text, out valor) && valor == 0)
                    {
                        txtQtd.Text = string.Empty;
                    }
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
        //protected void dtgItensEtiquetas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    dtgItensEtiquetas.PageIndex = e.NewPageIndex;
        //    dtgEtiquetas_DataBind(); // Método para vincular dados ao GridView
        //    AbrirModal_Click(sender, e);
        //}
        void dtgEtiquetas_DataBind()
        {
            dtgEtiquetas.DataSource = ls_Etiquetas;
            try
            {
                dtgEtiquetas.DataBind();
            }
            catch (Exception ex)
            {

                MensagemPaginaEtiqueta.MostraMensagem_Erro($"Erro: {ex}");
            }

            RegistraScript("");
        }
        #endregion
        protected void SelecionaTipo_Change(object sender, EventArgs e)
        {

            //div_formEtiqueta.Visible = false;
            div_ItensEtiquetas.Visible = true;
            dtgItensEtiquetas_DataBind();

            //if (ddlsTipoEtiqueta.SelectedValue == "Volume")
            //{
            //    div_ItensEtiquetas.Visible = false;
            //}
            //if (ddlsTipoEtiqueta.SelectedValue == "Unitizado")
            //{
            //    div_ItensEtiquetas.Visible = false;
            //}
            AbrirModal_Click(sender, e);
        }

        protected void IncluirEtiquetas_Click(object sender, EventArgs e)
        {
            cls_WMS_Etiquetas obj_Etiqueta = new cls_WMS_Etiquetas();
            //bool itemCheckado = false;

            foreach (GridViewRow row in dtgItensEtiquetas.Rows)
            {
                CheckBox chkOpcaoItemEtiqueta = (CheckBox)row.FindControl("chkOpcaoItemEtiqueta");
                TextBox txtnQuantidadeEnvio = (TextBox)row.FindControl("txtnQuantidadeEnvio");
                TextBox txtidItem = (TextBox)row.FindControl("txtidItem");
                TextBox txtsDscEtiqueta = (TextBox)row.FindControl("txtsDscEtiqueta");
                TextBox txtObservacao = (TextBox)row.FindControl("txtObservacao");
                //TextBox txtsLote = (TextBox)row.FindControl("txtsLote");
                TextBox txtSaldo = (TextBox)row.FindControl("txtnQtdPendente");
                if (chkOpcaoItemEtiqueta.Checked)
                {
                    if (txtnQuantidadeEnvio.Text != "0,00" && txtnQuantidadeEnvio.Text != "" && txtnQuantidadeEnvio.Text != "0")
                    {
                        if (Convert.ToInt32(BD.Conversoes.Numerico(txtnQuantidadeEnvio.Text)) <= Convert.ToInt32(BD.Conversoes.Numerico(txtSaldo.Text)))
                        {
                            //int id = ObterProximoIdUnitizado(obj_Etiqueta.IdEtiqueta);
                            obj_Etiqueta = new cls_WMS_Etiquetas();
                            obj_Etiqueta.StipoEtiqueta = "Produto";
                            obj_Etiqueta.IdOPI = Convert.ToInt32(hddidOPI.Value);
                            obj_Etiqueta.DtImpressao = "Não Impresso";
                            obj_Etiqueta.SdscEtiqueta = txtsDscEtiqueta.Text;
                            obj_Etiqueta.NQuantidade = Convert.ToDecimal(txtnQuantidadeEnvio.Text.ToString());
                            obj_Etiqueta.Sobservacao = txtObservacao.Text == "" ? "Sem Observação" : txtObservacao.Text;
                            //obj_Etiqueta.SLote = txtsLote.Text;
                            obj_Etiqueta.IdProduto = Convert.ToInt32(txtidItem.Text);
                            var idItem = txtidItem.Text.PadLeft(4, '0');
                            var codigo = $"PR{idItem}";
                            obj_Etiqueta.SCodigoBarras = codigo;
                            obj_Etiqueta.IdImpressora = Convert.ToInt32(ddlImpressora.SelectedValue);
                            isInclusao = true;
                            ls_Etiquetas.Add(obj_Etiqueta);
                        }
                        else
                        {
                            MensagemPaginaEtiqueta.MostraMensagem_Erro("A Quantidade não pode ser maior que o Saldo à Enviar");
                            break;
                        }

                    }
                    else
                    {
                        MensagemPaginaEtiqueta.MostraMensagem_Erro("Campo de Quantidade dos Produto Selecionado, precisa ser diferente de 0");
                        break;
                    }
                }
                //else if (!itemCheckado)
                //{
                //    MensagemPaginaEtiqueta.MostraMensagem_Erro("Nenhum Produto foi Selecionado, é obrigatório.");
                //    break;
                //}

                cmdSalvarEtiquetas.Visible = true;
                cmdIncluirEtiqueta.Focus();
            }

            dtgEtiquetas_DataBind();
            AbrirModal_Click(sender, e);
        }

        protected void SalvarEtiquetas_Click(object sender, EventArgs e)
        {
            Salvar_Etiquetas();
            AbrirModal_Click(sender, e);
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

            cmdSalvarEtiquetas.Visible = true;
        }

        protected void dtgEtiquetas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sControla = "";
                LinkButton cmdAdicionarSerie = e.Row.FindControl("cmdAdicionarSerie") as LinkButton;

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

                if (sControla == "S")
                {
                    cmdAdicionarSerie.Visible = true;
                }
                else
                {
                    cmdAdicionarSerie.Visible = false;
                }
            }
        }

        void Salvar_Etiquetas()
        {
            string sErro = "";

            try
            {
                string[] vidOPI = hddidOPI.Value.Split(',');
                string idOPI = vidOPI[0].ToString();
                DataSet dsSalvar = new DataSet();
                Dictionary<String, String> vParametros = new Dictionary<string, string>();

                vParametros.Clear();

                vParametros.Add("@sFuncao", "SALVAR");

                ls_Etiquetas.ForEach(e =>
                {
                    vParametros["@idEtiqueta"] = e.IdEtiqueta.ToString();
                    vParametros["@idOPI"] = idOPI;
                    vParametros["@sDscEtiqueta"] = e.SdscEtiqueta;
                    vParametros["@idUsuarioAtualizacao"] = IDENTITY.Variaveis.idUsuario();
                    vParametros["@sObservacao"] = e.Sobservacao ?? string.Empty;
                    vParametros["@sCodigoBarras"] = e.SCodigoBarras.ToString();

                    if (e.IdImpressora != 0)
                        vParametros["@idImpressora"] = e.IdImpressora.ToString();

                    //vParametros["@sLote"] = e.SLote.ToString();
                    vParametros["@sExclusao"] = string.IsNullOrEmpty(e.SExclusao) ? "N" : e.SExclusao;

                    if (e.StipoEtiqueta == "Volume")
                    {
                        vParametros["@sVolume"] = "S";
                    }
                    else if (e.StipoEtiqueta == "Unitizado")
                    {
                        vParametros["@sUnitizado"] = "S";
                    }
                    else if (e.StipoEtiqueta == "Produto")
                    {
                        vParametros["@idProduto"] = e.IdProduto.ToString();
                        //vParametros["@nQuantidade"] = e.NQuantidade.ToString().Replace(",", ".");
                        vParametros["@nQuantidade"] = e.NQuantidade.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    }

                    dsSalvar = BD.ExecutarDataSet(sProcedureEtiqueta, vParametros);
                });


                if (BD.ValidarDataSet(dsSalvar, out sErro))
                {
                    //Pesquisar(idOPI, false);                       
                    MensagemPaginaEtiqueta.MostraMensagem_Sucesso("Etiquetas Salvas com Sucesso!");
                    PopularEtiquetas(hddidOPI.Value);
                }
                else
                {
                    MensagemPaginaEtiqueta.MostraMensagem_Erro("BD: " + sErro.ToString());
                }

            }
            catch (Exception ex)
            {
                MensagemPaginaEtiqueta.MostraMensagem_Erro(ex.Message);
            }
            cmdSalvarEtiquetas.Visible = false;
            cmdIncluirEtiqueta.Visible = false;
            dtgItensEtiquetas_DataBind();
            RegistraScript("");
        }

        protected void chkOpcaoItemEtiqueta_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            if (chk.Checked)
            {
                cmdIncluirEtiqueta.Visible = true;
            }
            if (!chk.Checked)
            {
                foreach (GridViewRow row in dtgItensEtiquetas.Rows)
                {
                    CheckBox chkItens = row.FindControl("chkOpcaoItemEtiqueta") as CheckBox;
                    if (chkItens != null && chkItens.Checked)
                    {
                        if (chkItens.Checked)
                        {
                            cmdIncluirEtiqueta.Visible = true;
                            break;
                        }
                    }
                    else
                    {
                        cmdIncluirEtiqueta.Visible = false;
                    }
                }
            }
            AbrirModal_Click(sender, e);
        }

        protected void AbrirEtiquetas_Click(object sender, EventArgs e)
        {
            div_allView.Visible = true;
            div_seriesEtiquetas.Visible = false;
            MultiViewFormularios.ActiveViewIndex = 0;
            AtualizarBarraProgresso();
            lbltituloModal.Text = "Etiqueta";
            LimparClassesAtivas();
            cmdEtiquetas.CssClass += " activeMn";
            div_botoesUnitizado.Visible = false;
            div_botoesVolumes.Visible = false;
            AbrirModal_Click(sender, e);
        }
        protected void ddlLocal_SelectedIndexChanged(object sender, EventArgs e)
        {
            FUNCOES.Popula_Combo(ddlPosicaoPai, $"{sProcedureLocal} 'FLOW-POSICOES', @idLocalArmazenamento={ddlLocal.SelectedValue}", "idPosicao", "sCodigoLocal", false, "Selecione a Posicao", "0");
            AbrirModal_Click(sender, e);
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
            AbrirModal_Click(sender, e);

        }
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
                dtgUnitizadosDataBind(dsUnitizados);
                ls_unitizadosItensGV.Clear();
            }
            else
            {
                div_UnitizadoGV.Visible = false;
            }

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
            else
            {
                div_UnitizadoGV.Visible = false;
            }
        }
        void dtgUnitizadosDataBind(DataSet dsUnitizados)
        {
            dtgUnitizado.DataSource = dsUnitizados;
            dtgUnitizado.DataBind();
            RegistrarColapsoScript();
        }
        protected void dtgUnitizado_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string idUnitizado = dtgUnitizado.DataKeys[e.Row.RowIndex].Value.ToString();
                var dtgUnitizadosItens = (GridView)e.Row.FindControl("dtgUnitizadosItens");
                ConsultaUnitizadosItens(idUnitizado);
                //tenho que puxar os dados da tabela itens envios, para cá
                dtgUnitizadosItens.DataSource = ls_unitizadosItensGV;
                dtgUnitizadosItens.DataBind();
            }
        }
        void ConsultaUnitizadosItens(string idUnitizado)
        {
            string sFuncao = "CONSULTAR-UNITIZADOS-ITENS";
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
            if (!string.IsNullOrEmpty(LeitorQuagga1.GetCodigoBarras(txtsCodigoBarras)))
                txtsCodigoBarras.Text = LeitorQuagga1.GetCodigoBarras(txtsCodigoBarras);

            if (!string.IsNullOrEmpty(txtsCodigoBarras.Text))
            {
                DataSet dsProduto = new DataSet();
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR-CODIGO-BARRAS");
                vParametros.Add("@sCodigoBarras", txtsCodigoBarras.Text);
                vParametros.Add("@idOPI", hddidOPI.Value);
                dsProduto = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(dsProduto))
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

                    ls_unitizadosItens.AddRange(novosItens);

                    dtgUNItensDataBind();
                }
                else
                {
                    MensagemPaginaUnitizados.MostraMensagem_Aviso("Escaneie um Código de Barras Válido para o Produto Desejado!");
                }
            }
            else
            {
                MensagemPaginaUnitizados.MostraMensagem_Aviso("O Campo do Código de Barras é Obrigatório!");
            }
            cmdSalvarUnitizado.Visible = true;
            AbrirModal_Click(sender, e);
        }
        void dtgUNItensDataBind()
        {
            dtgUNItens.DataSource = ls_unitizadosItens;
            dtgUNItens.DataBind();
            RegistraScript("");
        }
        protected void UnitizadosItens_Salvar(string idUnitizado)
        {
            if (ls_unitizadosItens.Count > 0)
            {
                DataSet dsSalvarEnvioItens = new DataSet();
                Dictionary<string, string> vParametrosItens = new Dictionary<string, string>();
                vParametrosItens.Add("@sFuncao", "INCLUIR-ITENS-UNITIZADOS");
                ls_unitizadosItens.ForEach(a =>
                {
                    vParametrosItens["@idUnitizado"] = idUnitizado;
                    vParametrosItens["@nQuantidadeUnitizado"] = BD.Conversoes.Numerico(a.NQuantidade);
                    vParametrosItens["@idProdutoUnitizado"] = a.IdItem.ToString();
                    vParametrosItens["@sCodigoBarras"] = a.SCodigoBarras.ToString();
                    dsSalvarEnvioItens = BD.ExecutarDataSet(sProcedure, vParametrosItens);
                });

                if (BD.ValidarDataSet(dsSalvarEnvioItens))
                {
                    ls_unitizadosItens.Clear();
                    dtgUNItensDataBind();
                    txtsCodigoBarras.Text = "";
                }
                else
                {
                    MensagemPaginaUnitizados.MostraMensagem_Erro("Erro Ao Salvar");
                }
            }
            else
            {
                MensagemPaginaUnitizados.MostraMensagem_Aviso("O Unitizado foi Salvo sem Itens!", false);
            }
        }
        #endregion

        #region | Controles FrontEnd
        protected void AbrirUnitizados_Click(object sender, EventArgs e)
        {
            //RegistraQuaggaScript();
            MultiViewFormularios.ActiveViewIndex = 1;
            AtualizarBarraProgresso();
            lbltituloModal.Text = "Unitizados";
            div_botoesVolumes.Visible = false;
            LimparClassesAtivas();
            cmdUnitizados.CssClass += " activeMn";
            if (MultiViewUnitizados.ActiveViewIndex == 1 || MultiViewUnitizados.ActiveViewIndex == 2)
                div_botoesUnitizado.Visible = true;
            AbrirModal_Click(sender, e);
        }
        protected void ProximoUnitizado_Click(object sender, EventArgs e)
        {
            if (ValidarDadosUnitizados(true))
            {
                div_botoesUnitizado.Visible = true;
                cmdSalvarUnitizado.Visible = true;
                MultiViewUnitizados.ActiveViewIndex = 1;
            }

            AbrirModal_Click(sender, e);
        }
        protected void AbrirArquivos_Click(object sender, EventArgs e)
        {
            LinkButton cmdArquivo = (LinkButton)sender;
            string idUnitizado = cmdArquivo.CommandArgument;

            div_botoesUnitizado.Visible = true;
            cmdSalvarUnitizado.Visible = false;
            cmdAnteriorUnitizado.Visible = true;
            MultiViewUnitizados.ActiveViewIndex = 2;
            lbltituloModal.Text = "Fotos do Unitizado";

            Popular_aba_ArquivoUnitizado(idUnitizado);
            AbrirModal_Click(sender, e);
        }
        protected void AnteriorUnitizado_Click(object sender, EventArgs e)
        {
            div_botoesUnitizado.Visible = false;
            LimparCamposUnitizados();
            MultiViewUnitizados.ActiveViewIndex = 0;
            PopularUnitizados();
            AbrirModal_Click(sender, e);
        }
        protected void AbrirCamera_Click(object sender, EventArgs e)
        {
            bool abrir = (bool)Session["CameraAberta"];

            abrir = !abrir;

            Session["CameraAberta"] = abrir;

            DivBipador.Visible = abrir;

            cmdAbrirCâmera.Text = abrir ? "Fechar Leitor" : "Abrir Leitor";

            // if (cmdAbrirCâmera.Text == "Abrir Leitor")
            //    RegistraQuaggaScript();

            AbrirModal_Click(sender, e);
        }

        protected void EditarUnitizado_Click(object sender, EventArgs e)
        {
            LinkButton cmdEditar = (LinkButton)sender;
            string idUnitizado = cmdEditar.CommandArgument;

            hddidUnitizadoUpdt.Value = idUnitizado;
            div_botoesUnitizado.Visible = true;
            cmdSalvarUnitizado.Visible = true;
            cmdAnteriorUnitizado.Visible = true;
            MultiViewUnitizados.ActiveViewIndex = 1;
            lbltituloModal.Text = "Editar Unitizado";

            PopularUnitizados(idUnitizado);

            AbrirModal_Click(sender, e);
        }
        #endregion

        protected void SalvarUnitizado_Click(object sender, EventArgs e)
        {
            SalvarUnitizados("");
            AbrirModal_Click(sender, e);
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
                        //vParametros.Add("@sUnitizado", ddlsUnitizado.SelectedValue);

                        DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                        if (BD.ValidarDataSet(dsSalvar, out sErro))
                        {

                            hddidUnitizado.Value = RETORNO.DATASET(dsSalvar, 0, "idUnitizado");

                            UnitizadosItens_Salvar(hddidUnitizado.Value);
                            PopularUnitizados();
                            //Popular_aba_ArquivoUnitizado(hddidUnitizado.Value);
                            MensagemPaginaUnitizados.MostraMensagem_Sucesso("Registro gravado com sucesso", false);
                            LimparCamposUnitizados();
                            MultiViewUnitizados.ActiveViewIndex = 0;
                            div_botoesUnitizado.Visible = false;
                            PopularEtiquetas(hddidOPI.Value);
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
            else if (idUnitizado != "" || idUnitizado != "0")
            {
                UnitizadosItens_Salvar(idUnitizado);
                PopularUnitizados();
                //Popular_aba_ArquivoUnitizado(hddidUnitizado.Value);
                MensagemPaginaUnitizados.MostraMensagem_Sucesso("Registro salvo com sucesso", false);
                MultiViewUnitizados.ActiveViewIndex = 0;
                div_botoesUnitizado.Visible = false;
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
            if (!string.IsNullOrEmpty(LeitorQuagga.GetCodigoBarras(txtsCodigoBarrasVolume)))
                txtsCodigoBarrasVolume.Text = LeitorQuagga.GetCodigoBarras(txtsCodigoBarrasVolume);

            if (!string.IsNullOrEmpty(txtsCodigoBarrasVolume.Text))
            {
                DataSet dsVolumeItem = new DataSet();
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR-CODIGO-BARRAS-VOL");
                vParametros.Add("@sCodigoBarras", txtsCodigoBarrasVolume.Text);
                vParametros.Add("@idOPI", hddidOPI.Value);
                dsVolumeItem = BD.ExecutarDataSet(sProcedure, vParametros);

                if (txtsCodigoBarrasVolume.Text.Substring(0, 2) == "PR")
                {
                    if (BD.ValidarDataSet(dsVolumeItem))
                    {
                        List<cls_WMS_VolumesItens> novosItens = dsVolumeItem.Tables[0].AsEnumerable().Select(row =>
                        {
                            return new cls_WMS_VolumesItens()
                            {
                                IdObjeto = Convert.ToInt32(row["idProduto"]),
                                SDscObjeto = row["sDscProduto"].ToString(),
                                SCodigoBarras = txtsCodigoBarrasVolume.Text,
                                //NVolume = Convert.ToInt32(txtnVolume.Text),
                            };
                        }).ToList();

                        ls_VolumesItens.AddRange(novosItens);

                        dtgVolumesItensDataBind();
                    }
                    else
                    {
                        MensagemPaginaVolume.MostraMensagem_Aviso("Código de Barras InVálido para o Produto Desejado!");
                    }
                }
                else if (txtsCodigoBarrasVolume.Text.Substring(0, 2) == "UN")
                {
                    if (BD.ValidarDataSet(dsVolumeItem))
                    {
                        List<cls_WMS_VolumesItens> novosItens = dsVolumeItem.Tables[0].AsEnumerable().Select(row =>
                        {
                            return new cls_WMS_VolumesItens()
                            {
                                IdObjeto = Convert.ToInt32(row["idUnitizado"]),
                                SDscObjeto = row["sDscUnitizado"].ToString(),
                                SCodigoBarras = txtsCodigoBarrasVolume.Text,
                                //NVolume = Convert.ToInt32(txtnVolume.Text),
                            };
                        }).ToList();

                        ls_VolumesItens.AddRange(novosItens);

                        dtgVolumesItensDataBind();
                    }
                    else
                    {
                        MensagemPaginaVolume.MostraMensagem_Aviso("Código de Barras InVálido para o Unitizado Desejado!");
                    }
                }
                else if (txtsCodigoBarrasVolume.Text.Substring(0, 2) == "VOL")
                {
                    if (BD.ValidarDataSet(dsVolumeItem))
                    {
                        List<cls_WMS_VolumesItens> novosItens = dsVolumeItem.Tables[0].AsEnumerable().Select(row =>
                        {
                            return new cls_WMS_VolumesItens()
                            {
                                IdObjeto = Convert.ToInt32(row["idVolume"]),
                                SDscObjeto = row["sDscVolume"].ToString(),
                                SCodigoBarras = txtsCodigoBarrasVolume.Text,
                                //NVolume = Convert.ToInt32(txtnVolume.Text),
                            };
                        }).ToList();

                        ls_VolumesItens.AddRange(novosItens);

                        dtgVolumesItensDataBind();
                    }
                    else
                    {
                        MensagemPaginaVolume.MostraMensagem_Aviso("Código de Barras InVálido para o Unitizado Desejado!");
                    }
                }
                else
                {
                    MensagemPaginaVolume.MostraMensagem_Erro("Código de Barras Inválido");
                }
                div_botoesVolumes.Visible = true;
                txtsCodigoBarrasVolume.Text = "";
            }

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
                    vParametros.Add("@idOPI", txtidOPI.Text);
                    vParametros.Add("@idProdutoEmbalagem", ddlEmbalagem.SelectedValue);
                    vParametros.Add("@sDscVolume", "Volume Teste");
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
            div_botoesVolumes.Visible = true;
            AbrirModal_Click(sender, e);
        }
        protected void VolumesItens_Salvar(string id)
        {
            if (ls_VolumesItens.Count > 0)
            {
                DataSet dsVolumesItens = new DataSet();
                Dictionary<string, string> vParametrosItens = new Dictionary<string, string>();
                vParametrosItens.Add("@sFuncao", "INCLUIR-VOLUMES-ITENS");
                ls_VolumesItens.ForEach(a =>
                {
                    vParametrosItens["@idVolume"] = id;
                    vParametrosItens["@idObjeto"] = a.IdObjeto.ToString();
                    vParametrosItens["@nVolume"] = a.NVolume.ToString();
                    vParametrosItens["@idUsuarioAtualizacao"] = IDENTITY.Variaveis.idUsuario().ToString();
                    vParametrosItens["@sCodigoBarras"] = a.SCodigoBarras;
                    vParametrosItens["@sDscObjeto"] = a.SDscObjeto;

                    dsVolumesItens = BD.ExecutarDataSet(sProcedure, vParametrosItens);
                });

                if (BD.ValidarDataSet(dsVolumesItens))
                {
                    ls_VolumesItens.Clear();
                    dtgVolumesItensDataBind();
                    txtsCodigoBarrasVolume.Text = "";
                }
                else
                {
                    MensagemPaginaVolume.MostraMensagem_Erro("Erro Ao Salvar");
                }
            }
            else
            {
                MensagemPaginaVolume.MostraMensagem_Aviso("O Unitizado foi Salvo sem Itens!", false);
            }
        }
        #region | Controles Front-End Volumes
        protected void AbrirVolumes_Click(object sender, EventArgs e)
        {
            //RegistraQuaggaScriptVolume();
            MultiViewFormularios.ActiveViewIndex = 2;
            AtualizarBarraProgresso();
            lbltituloModal.Text = "Volumes";
            LimparClassesAtivas();
            cmdVolumes.CssClass += " activeMn";
            div_botoesUnitizado.Visible = false;
            div_botoesVolumes.Visible = true;
            AbrirModal_Click(sender, e);
        }
        protected void AbrirCameraVolume_Click(object sender, EventArgs e)
        {
            //RegistraQuaggaScriptVolume();
            bool abrir = (bool)Session["CameraAbertaVolume"];

            abrir = !abrir;

            Session["CameraAbertaVolume"] = abrir;

            DivBipadorVolume.Visible = abrir;

            cmdAbrirCamVolume.Text = abrir ? "Fechar Leitor" : "Abrir Leitor";
            if (cmdAbrirCamVolume.Text == "Fechar Leitor")
            {
                divCodigoB.Visible = false;
            }
            else
            {
                LeitorQuagga.DesligarCam();
                //LeitorQuagga.FecharCamera();
                divCodigoB.Visible = true;
            }
            AbrirModal_Click(sender, e);
        }
        protected void AbrirArquivosVolumes_Click(object sender, EventArgs e)
        {
            LinkButton cmdArquivo = (LinkButton)sender;
            string idVolume = cmdArquivo.CommandArgument;

            MultiViewFormularios.ActiveViewIndex = 3;
            lbltituloModal.Text = "Fotos do Volume";
            div_botoesVolumes.Visible = true;
            Popular_aba_ArquivoVolume(idVolume);
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
                div_VolumesSalvos.Visible = false;
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

                //var dataItem = e.Row.DataItem as DataRowView;

                //if (dataItem != null)
                //{
                //    if (dataItem["SCodigoProduto"] == DBNull.Value || string.IsNullOrEmpty(dataItem["SCodigoProduto"].ToString()))
                //    {
                //        btnToggle.Attributes.Add("disabled", "disabled");
                //        e.Row.Cells[2].Visible = false;
                //        ((GridView)sender).HeaderRow.Cells[1].Visible = false;
                //    }
                //    else
                //    {
                //        btnToggle.Attributes.Remove("disabled");
                //    }

                //    if (dataItem["NQuantidade"] == DBNull.Value || string.IsNullOrEmpty(dataItem["NQuantidade"].ToString()))
                //    {
                //        e.Row.Cells[3].Visible = false;
                //        ((GridView)sender).HeaderRow.Cells[3].Visible = false;
                //    }

                //    if (dataItem["SControlaGarantia"] == DBNull.Value || string.IsNullOrEmpty(dataItem["SControlaGarantia"].ToString()))
                //    {
                //        e.Row.Cells[4].Visible = false;
                //        ((GridView)sender).HeaderRow.Cells[4].Visible = false;
                //    }
                //}
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
            string sFuncao = "CONSULTAR-UNITIZADOS-ITENS";
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
            for (int i = 0; i < dtgVolumesItens.Rows.Count; i++)
            {
                GridViewRow item = dtgVolumesItens.Rows[i];

                if (item.RowType == DataControlRowType.DataRow)
                {
                    try
                    {
                        TextBox txtnVolumeGV = (TextBox)item.FindControl("txtnVolumeGV");
                        if (Convert.ToInt32(txtnVolumeGV.Text) > 0)
                        {
                            ls_VolumesItens[i].NVolume = Convert.ToInt32(txtnVolumeGV.Text);
                        }
                        else
                        {
                            MensagemPaginaVolume.MostraMensagem_Aviso("Lembrete: A Ordem Foi Adicionada Como Zero.", false);
                            return true;
                        }
                    }
                    catch
                    {
                        MensagemPaginaVolume.MostraMensagem_Erro("Dados Incorretos na Grid", false);
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion
        #endregion

        #region | Controle de Modal
        protected void AbrirModal_Click(object sender, EventArgs e)
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

            if (MultiViewFormularios.ActiveViewIndex == 2)
            {
                //cmdEfetuarEnvio.Visible = true;
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalEnvio", "$('#modalEnvio').modal('show');", true);
        }

        protected void btnAnterior_Click(object sender, EventArgs e)
        {
            if (MultiViewFormularios.ActiveViewIndex > 0)
            {
                MultiViewFormularios.ActiveViewIndex--;
                AtualizarBarraProgresso();
            }
            AbrirModal_Click(sender, e);
        }

        protected void btnProximo_Click(object sender, EventArgs e)
        {
            if (MultiViewFormularios.ActiveViewIndex < MultiViewFormularios.Views.Count - 1)
            {
                MultiViewFormularios.ActiveViewIndex++;

                AtualizarBarraProgresso();
            }

            AbrirModal_Click(sender, e);
        }
        protected void VoltarModal_Click(object sender, EventArgs e)
        {
            MultiViewFormularios.ActiveViewIndex = 2;
            div_botoesVolumes.Visible = true;
            AbrirModal_Click(sender, e);
        }
        protected void AtualizarBarraProgresso()
        {
            int totalEtapas = MultiViewFormularios.Views.Count - 1;
            int etapaAtual = MultiViewFormularios.ActiveViewIndex + 1;
            int porcentagemConcluida = (etapaAtual * 100) / totalEtapas;
            RegistraScriptCamposPeso();
            divProgresso.InnerText = etapaAtual + " de " + totalEtapas;

            divProgresso.Attributes["style"] = "width: " + porcentagemConcluida + "%;";
        }
        private void LimparClassesAtivas()
        {
            cmdEtiquetas.CssClass = cmdEtiquetas.CssClass.Replace("active", "").Trim();
            cmdUnitizados.CssClass = cmdUnitizados.CssClass.Replace("active", "").Trim();
            cmdVolumes.CssClass = cmdVolumes.CssClass.Replace("active", "").Trim();
            //cmdEnvio.CssClass = cmdEnvio.CssClass.Replace("active", "").Trim();
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
            DataSet dsProduto = new DataSet();
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR-CODIGO-BARRAS");
            vParametros.Add("@sCodigoBarras", codigoBarras);

            dsProduto = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsProduto))
            {
                DivBipador.Visible = false;
                MensagemPaginaUnItens.MostraMensagem_Aviso("Código de Barras Validado Com Sucesso!");
            }
            else
            {
                MensagemPaginaUnItens.MostraMensagem_Aviso("Código de Barras Inválido!");
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
            div_botoesVolumes.Visible = false;
            //Popular_aba_ArquivoUnitizado(hddidOPI.Value);           
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

        #region | Serielização
        protected void cmdAdicionarSerie_Click(object sender, EventArgs e)
        {
            LinkButton lknSerie = (LinkButton)sender;
            string idEtiqueta = lknSerie.CommandArgument;

            GridViewRow row = (GridViewRow)lknSerie.NamingContainer;

            string sCodigoBarras = row.Cells[0].Text;
            string sDscProduto = HttpUtility.HtmlDecode(row.Cells[1].Text);
            string nQuantidade = row.Cells[2].Text;
            string idProduto = dtgEtiquetas.DataKeys[row.RowIndex].Values["idProduto"]?.ToString();

            lblTituloProduto.Text = sDscProduto;

            hddsCodigoBarras.Value = sCodigoBarras;
            hddidProdutoSerie.Value = idProduto;
            hddnQtdSerie.Value = nQuantidade;
            hddsProdutoSerie.Value = sDscProduto;
            hddidEtiqueta.Value = idEtiqueta;

            string script = $@"
              document.getElementById('{hddsCodigoBarras.ClientID}').value = '{sCodigoBarras}';
              document.getElementById('{hddidProdutoSerie.ClientID}').value = '{idProduto}';
              document.getElementById('{hddnQtdSerie.ClientID}').value = '{nQuantidade}';
              document.getElementById('{hddsProdutoSerie.ClientID}').value = '{sDscProduto}';
              document.getElementById('{hddidEtiqueta.ClientID}').value = '{idEtiqueta}';
             ";

            ScriptManager.RegisterStartupScript(this, GetType(), "AtualizarHiddenFields", script, true);

            MultiViewFormularios.ActiveViewIndex = 0;
            AtualizarBarraProgresso();
            lbltituloModal.Text = "Etiqueta";
            LimparClassesAtivas();
            cmdEtiquetas.CssClass += " activeMn";
            //div_botoesUnitizado.Visible = false;
            cmdIncluirSerie.Visible = true;
            div_botoesVolumes.Visible = false;
            AbrirModal_Click(sender, e);
            div_seriesEtiquetas.Visible = true;
            div_allView.Visible = false;
            PopularEtiquetasSerie(idEtiqueta);
            //Adicionar hiddnFields e Associar com linha da grid

        }
        void PopularEtiquetasSerie(string idEtiqueta)
        {
            string sFuncao = "CONSULTAR-ETIQUETAS-SERIES";
            string sErro = "";
            DataSet dsEtiquetas;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idEtiqueta", idEtiqueta);

            dsEtiquetas = BD.ExecutarDataSet(sProcedureEtiqueta, vParametros, false);

            if (BD.ValidarDataSet(dsEtiquetas, out sErro))
            {
                ls_EtiquetasSeries.Clear();

                //Usei uma atribuição usando LINQ
                ls_EtiquetasSeries = dsEtiquetas.Tables[0].AsEnumerable().Select(row =>
                {
                    return new cls_WMS_Etiquetas
                    {
                        IdEtiquetaSerie = Convert.ToInt32(row["idEtiquetaLoteSerie"]),
                        IdEtiqueta = Convert.ToInt32(row["idEtiqueta"]),
                        IdProduto = Convert.ToInt32(row["idProduto"]),
                        SdscProduto = row["sDscProduto"].ToString(),
                        SCodigoBarras = row["sCodigoBarras"].ToString(),
                        IdLocal = Convert.ToInt32(row["idLocal"]),
                        SLote = row["sLote"].ToString(),
                        NSerie = row["sSerie"].ToString(),
                        sCodigo = row["sCodigo"].ToString(),
                        IdPosicao = Convert.ToInt32(row["idPosicao"]),

                    };
                }).ToList();

                dtgEtiquetasSerie_DataBind();

                if (ls_EtiquetasSeries.Count > 0)
                {
                    cmdSalvarSeries.Visible = true;
                }
                else
                {
                    cmdSalvarSeries.Visible = false;
                }

                if (!string.IsNullOrEmpty(hddnQtdSerie.Value) && hddnQtdSerie.Value == ls_EtiquetasSeries.Count.ToString())
                {
                    cmdIncluirSerie.Visible = false;
                    if (ls_EtiquetasSeries.Count == Convert.ToInt32(hddnQtdSerie.Value))
                    {
                        MensagemPaginaSeries.MostraMensagem("<b>Lembrete:</b> Limite de inclusão atingido. Salve os Itens Adicionados Para Atualizar.", "info", false);
                    }
                }
                else
                {
                    cmdIncluirSerie.Visible = true;
                }
                div_gvSerie.Visible = true;
            }
            else
            {
                div_gvSerie.Visible = false;
                MensagemPaginaSeries.MostraMensagem_Erro("Nenhuma Série Adicionada", false);
            }
        }
        void dtgEtiquetasSerie_DataBind()
        {
            dtgEtiquetasSerie.DataSource = ls_EtiquetasSeries;
            try
            {
                dtgEtiquetasSerie.DataBind();
            }
            catch (Exception ex)
            {

                MensagemPaginaEtiqueta.MostraMensagem_Erro($"ERRO: {ex}");
            }

            RegistraScript("");
        }
        protected void dtgEtiquetasSeries_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlArmazenamento = e.Row.FindControl("ddlArmazenamento") as DropDownList;
                DropDownList ddlPosicaoPai = e.Row.FindControl("ddlPosicaoPai") as DropDownList;

                FUNCOES.Popula_Combo(ddlArmazenamento, "sp_Manipula_tbl_Flow_Produtos_Movimentacao 'FLOW_LOCAL_ARMAZENAMENTO'", "idLocalArmazenamento", "sDscLocalArmazenamento", false, "Selecione o Local", "0");


                var item = (cls_WMS_Etiquetas)e.Row.DataItem;

                if (item != null && ddlArmazenamento.Items.FindByValue(item.IdLocal.ToString()) != null)
                {
                    ddlArmazenamento.SelectedValue = item.IdLocal.ToString();
                    FUNCOES.Popula_Combo(ddlPosicaoPai, $"{sProcedureLocal} 'FLOW-POSICOES', @idLocalArmazenamento={ddlArmazenamento.SelectedValue}", "idPosicao", "sCodigoLocal", false, "Selecione a Posição", "0");
                    ddlPosicaoPai.SelectedValue = item.IdPosicao.ToString();
                }

                if (item.SExclusao == "S")
                {
                    e.Row.Visible = false;
                }

            }
        }
        protected void VoltarEtiquetas_Click(object sender, EventArgs e)
        {
            div_allView.Visible = true;
            div_seriesEtiquetas.Visible = false;
            MultiViewFormularios.ActiveViewIndex = 0;
            AtualizarBarraProgresso();
            lbltituloModal.Text = "Etiqueta";
            LimparClassesAtivas();
            cmdEtiquetas.CssClass += " activeMn";
            div_botoesUnitizado.Visible = false;
            div_botoesVolumes.Visible = false;
            cmdSalvarSeries.Visible = false;
            LimparCamposSeries();
            ls_EtiquetasSeries.Clear();
            AbrirModal_Click(sender, e);
        }
        bool AtualizarClasseSerie()
        {
            List<FrameWork.cls_WMS_Etiquetas> listaAtual = new List<FrameWork.cls_WMS_Etiquetas>();
            string sMensagemErro = "";

            foreach (GridViewRow row in dtgEtiquetasSerie.Rows)
            {
                TextBox txtsLote = (TextBox)row.FindControl("txtsLote");
                TextBox txtNSerie = (TextBox)row.FindControl("txtNSerie");
                DropDownList ddlArmazenamento = (DropDownList)row.FindControl("ddlArmazenamento");
                DropDownList ddlPosicaoPai = (DropDownList)row.FindControl("ddlPosicaoPai");

                HiddenField hddidEtiquetaSerie = (HiddenField)row.FindControl("hddidEtiquetaSerie");
                HiddenField hddidEtiqueta = (HiddenField)row.FindControl("hddidEtiqueta");
                HiddenField hddidProdutoSerie = (HiddenField)row.FindControl("hddidProdutoSerie");
                HiddenField hddsProdutoSerie = (HiddenField)row.FindControl("hddsProdutoSerie");
                HiddenField hddsCodigoBarras = (HiddenField)row.FindControl("hddsCodigoBarras");
                HiddenField hddidOPI = (HiddenField)row.FindControl("hddidOPI");
                HiddenField hddsExclusao = (HiddenField)row.FindControl("hddsExclusao");

                string lote = txtsLote.Text.Trim();
                string serie = txtNSerie.Text.Trim();
                int idArmazenamento = string.IsNullOrEmpty(ddlArmazenamento.SelectedValue) ? 0 : int.Parse(ddlArmazenamento.SelectedValue);

                int idEtiquetaSerie = string.IsNullOrEmpty(hddidEtiquetaSerie.Value) ? 0 : int.Parse(hddidEtiquetaSerie.Value);
                int idEtiqueta = string.IsNullOrEmpty(hddidEtiqueta.Value) ? 0 : int.Parse(hddidEtiqueta.Value);
                int idProduto = string.IsNullOrEmpty(hddidProdutoSerie.Value) ? 0 : int.Parse(hddidProdutoSerie.Value);
                string sDscProduto = hddsProdutoSerie.Value;
                string sCodigoBarras = hddsCodigoBarras.Value;
                int idOPI = string.IsNullOrEmpty(hddidOPI.Value) ? 0 : int.Parse(hddidOPI.Value);
                string sExclusao = hddsExclusao.Value;
                int idPosicao = string.IsNullOrEmpty(ddlPosicaoPai.SelectedValue) ? 0 : int.Parse(ddlPosicaoPai.SelectedValue);

                var itemExistente = listaAtual.FirstOrDefault(item => item.NSerie == serie || item.SLote == lote);
                if (itemExistente != null)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Não pode haver séries ou lotes repetidos.";
                }
                if (idArmazenamento == 0) //idPosicao == 0 Retirado
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O Local de Armazenamento não pode ser vazio.";
                }

                // Criar objeto para validar no banco de dados
                var novoItem = new FrameWork.cls_WMS_Etiquetas
                {
                    IdEtiquetaSerie = idEtiquetaSerie,
                    IdEtiqueta = idEtiqueta,
                    IdProduto = idProduto,
                    SdscProduto = sDscProduto,
                    SCodigoBarras = sCodigoBarras,
                    IdLocal = idArmazenamento,
                    SLote = lote,
                    NSerie = serie,
                    IdOPI = idOPI,
                    SExclusao = sExclusao,
                    IdPosicao = idPosicao
                };

                // Validação 2: Verificar se a série já existe no banco de dados
                DataSet dsPesquisa;
                Dictionary<string, string> vParametros = new Dictionary<string, string>
        {
            { "@sFuncao", "VERIFICAR-SERIES" },
            { "@nSerie", novoItem.NSerie }
        };

                dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);
                if (!string.IsNullOrEmpty(RETORNO.DATASET(dsPesquisa, "nSerie")))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O Número de Série já existe no Banco de Dados e não pode ser repetido.";
                    //return false;
                }

                // Se passou pelas validações, adicionar à lista
                listaAtual.Add(novoItem);
            }

            if (!string.IsNullOrEmpty(sMensagemErro))
            {
                MensagemPaginaSeries.MostraMensagem_Erro(sMensagemErro, false);
                dtgEtiquetasSerie_DataBind();
                return false;
            }
            else
            {
                ls_EtiquetasSeries = listaAtual;
                dtgEtiquetasSerie_DataBind();
                return true;
            }

        }

        void LimparCamposSeries()
        {
            txtsLoteEtiquetas.Text = "";
            txtnSerie.Text = "";
            ddlLocal.SelectedValue = "0";

            hddsCodigoBarras.Value = "";
            hddidProdutoSerie.Value = "";
            hddnQtdSerie.Value = "";
            hddsProdutoSerie.Value = "";
            hddidEtiqueta.Value = "";
        }
        bool ValidarDadosSeries()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (string.IsNullOrEmpty(txtsLoteEtiquetas.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um Lote.";
            }
            if (string.IsNullOrEmpty(txtnSerie.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um Número de Série.";
            }
            if (ddlLocal.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um Local.";
            }
            //if (ddlPosicaoPai.SelectedValue == "0")
            //{
            //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Posição.";
            //}

            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPaginaSeries.MostraMensagem_Erro(sMensagemErro, false);
            }

            return bRetorno;
        }
        protected void IncluirSerie_Click(object sender, EventArgs e)
        {
            string sMensagemErro = "";
            if (ValidarDadosSeries())
            {
                int idLocal = Convert.ToInt32(ddlLocal.SelectedValue);
                string sLote = txtsLoteEtiquetas.Text;
                string nSerie = txtnSerie.Text;
                int idPosicao = Convert.ToInt32(ddlPosicaoPai.SelectedValue);

                var itemExistente = ls_EtiquetasSeries.FirstOrDefault(item => item.NSerie == nSerie || item.SLote == sLote);

                if (itemExistente != null)
                {
                    MensagemPaginaSeries.MostraMensagem_Erro("Não pode haver séries repetidas.");
                }
                else
                {
                    var novoItem = new cls_WMS_Etiquetas()
                    {
                        SCodigoBarras = hddsCodigoBarras.Value,
                        IdProduto = Convert.ToInt32(hddidProdutoSerie.Value),
                        NQuantidade = Convert.ToInt32(hddnQtdSerie.Value),
                        SdscProduto = hddsProdutoSerie.Value,
                        SLote = sLote,
                        NSerie = nSerie,
                        IdLocal = idLocal,
                        IdPosicao = idPosicao
                    };

                    var ultimoItemAdicionado = novoItem;

                    foreach (var item in ls_EtiquetasSeries.Where(item => item.SExclusao == "N"))
                    {
                        DataSet dsPesquisa;
                        Dictionary<string, string> vParametros = new Dictionary<string, string>();
                        vParametros.Add("@sFuncao", "VERIFICAR-SERIES");
                        vParametros.Add("@nSerie", item.NSerie);

                        dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);
                        if (string.IsNullOrEmpty(RETORNO.DATASET(dsPesquisa, "nSerie")))
                        {
                            if (item.NSerie.Length >= 4 && item.SLote.Length >= 4)
                            {
                                dtgEtiquetasSerie_DataBind();
                            }
                            else
                            {
                                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "A Série e Lote Precisam ter 4 ou mais Caracateres.";
                            }
                        }
                        else
                        {
                            sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O Número de Série já existe no Banco de Dados e não pode ser repetido.";
                        }
                        //if (sMensagemErro != "")
                        //{                           
                        //    MensagemPaginaSeries.MostraMensagem_Erro(sMensagemErro, false);
                        //    //ls_EtiquetasSeries.Remove(ultimoItemAdicionado);
                        //}

                    }
                    if (!string.IsNullOrEmpty(sMensagemErro))
                    {
                        MensagemPaginaSeries.MostraMensagem_Erro(sMensagemErro, false);
                    }
                    else
                    {
                        ls_EtiquetasSeries.Add(novoItem);
                        dtgEtiquetasSerie_DataBind();
                        div_gvSerie.Visible = true;
                    }
                }
                int qtdSeriesValidas = ls_EtiquetasSeries.Count(i => i.SExclusao != "S");

                if (qtdSeriesValidas > 0)
                {
                    cmdSalvarSeries.Visible = true;
                }
                else
                {
                    cmdSalvarSeries.Visible = false;
                }
            }
            int qtdSeriesValidas2 = ls_EtiquetasSeries.Count(i => i.SExclusao != "S");
            if (!string.IsNullOrEmpty(hddnQtdSerie.Value))
            {
                if (qtdSeriesValidas2 == Convert.ToInt32(hddnQtdSerie.Value))
                {
                    cmdIncluirSerie.Visible = false;
                    MensagemPaginaSeries.MostraMensagem("<b>Lembrete:</b> Limite de inclusão atingido. Salve os Itens Adicionados Para Atualizar.", "info", false);
                }
                else
                {
                    cmdIncluirSerie.Visible = true;
                }
            }
            else
            {
                cmdIncluirSerie.Visible = true;
            }



            LimparCamposSeries();
            AbrirModal_Click(sender, e);
        }
        protected void SalvarSeries_Click(object sender, EventArgs e)
        {
            Salvar_Series();
            AbrirModal_Click(sender, e);
        }
        protected void ExcluirSerie_Click(object sender, EventArgs e)
        {
            LinkButton btnExcluir = (LinkButton)sender;
            string idEtiquetaSerie = btnExcluir.CommandArgument;

            if (idEtiquetaSerie != "0")
            {
                var serie = ls_EtiquetasSeries.FirstOrDefault(et => et.IdEtiquetaSerie == Convert.ToInt32(idEtiquetaSerie));
                if (serie != null)
                {
                    serie.SExclusao = "S";
                }

                GridViewRow rowToDelete = (GridViewRow)btnExcluir.NamingContainer;
                rowToDelete.Visible = false;
            }
            else
            {
                GridViewRow row = (GridViewRow)btnExcluir.NamingContainer;
                int rowIndex = row.RowIndex;
                ls_EtiquetasSeries.RemoveAt(rowIndex);
            }

            dtgEtiquetasSerie.DataSource = ls_EtiquetasSeries;
            dtgEtiquetasSerie.DataBind();

            AbrirModal_Click(sender, e);

            int qtdSeriesValidas = ls_EtiquetasSeries.Count(i => i.SExclusao != "S");

            if (qtdSeriesValidas > 0)
            {
                cmdSalvarSeries.Visible = true;
            }
            else
            {
                cmdSalvarSeries.Visible = false;
            }

            if (!string.IsNullOrEmpty(hddnQtdSerie.Value))
            {
                if (qtdSeriesValidas == Convert.ToInt32(hddnQtdSerie.Value))
                {
                    cmdIncluirSerie.Visible = false;
                    MensagemPaginaSeries.MostraMensagem("<b>Lembrete:</b> Limite de inclusão atingido. Salve os Itens Adicionados Para Atualizar.", "info", false);
                }
                else
                {
                    cmdIncluirSerie.Visible = true;
                }
            }


            //cmdSalvarEtiquetas.Visible = true;
        }
        void Salvar_Series()
        {
            if (AtualizarClasseSerie())
            {
                string sErro = "";

                try
                {
                    DataSet dsSalvar = new DataSet();
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    vParametros.Clear();

                    vParametros.Add("@sFuncao", "SALVAR-SERIES");

                    ls_EtiquetasSeries.ForEach(e =>
                    {
                        vParametros["@idEtiquetaLoteSerie"] = e.IdEtiquetaSerie.ToString();
                        vParametros["@idEtiqueta"] = hddidEtiqueta.Value;
                        vParametros["@idProduto"] = e.IdProduto.ToString();
                        vParametros["@idUsuarioAtualizacao"] = IDENTITY.Variaveis.idUsuario();
                        vParametros["@idLocal"] = e.IdLocal.ToString();
                        vParametros["@sCodigoBarras"] = e.SCodigoBarras.ToString();
                        vParametros["@sLote"] = e.SLote.ToString();
                        vParametros["@nSerie"] = e.NSerie.ToString();
                        vParametros["@idPosicao"] = e.IdPosicao.ToString();
                        vParametros["@sExclusao"] = string.IsNullOrEmpty(e.SExclusao) ? "N" : e.SExclusao;


                        dsSalvar = BD.ExecutarDataSet(sProcedureEtiqueta, vParametros);
                    });


                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        //Pesquisar(idOPI, false);                       
                        MensagemPaginaSeries.MostraMensagem_Sucesso("Series Salvas com Sucesso!");
                        PopularEtiquetasSerie(hddidEtiqueta.Value);
                    }
                    else
                    {
                        MensagemPaginaEtiqueta.MostraMensagem_Erro("BD: " + sErro.ToString());
                    }

                }
                catch (Exception ex)
                {
                    MensagemPaginaEtiqueta.MostraMensagem_Erro(ex.Message);
                }
                //cmdSalvarEtiquetas.Visible = false;
                //cmdIncluirEtiqueta.Visible = false;
                dtgEtiquetasSerie_DataBind();
                RegistraScript("");
            }

        }


        #endregion

    }
}