using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;

namespace TT_Hub.App.Paginas
{
    public partial class Ocorrencia_Detalhe : System.Web.UI.Page
    {

        string sTituloPagina = "Ocorrência";
        string sProcedure = "sp_HUB_Consulta_Ocorrencia";

        public List<TT_Hub.FrameWork.clsOcorrencia_Acoes> Base_gvAcoes
        {
            get
            {
                if (ViewState["Base_gvAcoes"] == null)
                {
                    ViewState["Base_gvAcoes"] = new List<FrameWork.clsOcorrencia_Acoes>();
                }
                return (List<FrameWork.clsOcorrencia_Acoes>)ViewState["Base_gvAcoes"];
            }

            set
            {
                ViewState["Base_gvAcoes"] = value;
            }

        }


        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidarPermissaoAcesso();
            RegistraScript("");
            if (!IsPostBack)
            {

  
                if (Request["id"] != null)
                {
                    
                    Pesquisar(Request["id"].ToString(), "0");
                }
                else
                {
                    Pesquisar("0", "0");
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
                else if (requestTarget == "funcao_SALVAR")
                {
                    GravarAcoes();
                }
                
            }

        }

        protected void Pesquisar(string idOcorrencia, string idCliente_Sistema)
        {
            string sErro = "";
            try
            {
                LimpaCampos();
                Base_gvAcoes.Clear();

                if (idOcorrencia != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTA DETALHE");
                    vParametros.Add("@idOcorrencia", idOcorrencia);
                    vParametros.Add("@idCliente", IDENTITY.Variaveis.idCliente());

                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidOcorrencia.Value           = RETORNO.DATASET(dsPesquisa, 0, "idOcorrencia");
                        string sIdOcorrencia            = RETORNO.DATASET(dsPesquisa, 0, "idOcorrencia");
                        string sDscTipoStatus           = RETORNO.DATASET(dsPesquisa, 0, "sDscTipoStatus");
                        string sOK                      = RETORNO.DATASET(dsPesquisa, 0, "sOK");
                        string sPermiteAcao             = RETORNO.DATASET(dsPesquisa, 0, "sPermiteAcao");
                        string idTipoEquipamento        = RETORNO.DATASET(dsPesquisa, 0, "idTipoEquipamento");


                        FUNCOES.Popula_Combo(ddlAccao, "sp_Select 'ACOES', " + idTipoEquipamento, "idAcao", "sDscAcao", false, "Escolha uma ação", "0");

                        BreadCrumb.TitulodaPagina       = string.Format("{0} {1}", sTituloPagina, sIdOcorrencia);
                        lblTituloPagina.Text            = string.Format("ID: #{0} - {1}", sIdOcorrencia, RETORNO.DATASET(dsPesquisa, 0, "sLinkEquipamento"));
                        lblsDscTipoStatus.Text          = sDscTipoStatus;
                        lblsDscTipoStatus.CssClass      = string.Format("label label-{0}", RETORNO.DATASET(dsPesquisa, 0, "sCor"));
                        
                        caixaTitulo.Attributes.CssStyle.Add("class", string.Format("bg-{0}", RETORNO.DATASET(dsPesquisa, 0, "sCor")));

                        txtdtOcorrencia.Text            = RETORNO.DATASET(dsPesquisa, 0, "dtEvento");
                        txtsDscCliente.Text             = RETORNO.DATASET(dsPesquisa, 0, "sDscCliente");
                        txtsContatoTecnico.Text         = RETORNO.DATASET(dsPesquisa, 0, "sContatoTecnico");
                        txtsTelefoneTecnico.Text        = RETORNO.DATASET(dsPesquisa, 0, "sTelefoneTecnico");

                        txtsDscTipoStatus.Text          = RETORNO.DATASET(dsPesquisa, 0, "sDscTipoStatus");
                        txtsDscTipoEquipamento.Text     = RETORNO.DATASET(dsPesquisa, 0, "sDscTipoEquipamento");
                        txtsID.Text                     = RETORNO.DATASET(dsPesquisa, 0, "sID");
                        txtsNomeArquivo.Text            = RETORNO.DATASET(dsPesquisa, 0, "sNomeArquivo");
                        txtsRegistroCompleto.Text       = string.Format("Parametro: {0} - Valor: {1}", RETORNO.DATASET(dsPesquisa, 0, "sParametro"), RETORNO.DATASET(dsPesquisa, 0, "sValor"));

                        txtsEnviarSWCompleto.Text       = RETORNO.DATASET(dsPesquisa, 0, "sEnviarSWCompleto");
                        txtsCodigoSW.Text               = RETORNO.DATASET(dsPesquisa, 0, "sCodigoSW");
                        txtdtEnvioSW.Text               = RETORNO.DATASET(dsPesquisa, 0, "dtEnvioSW");

                        txtdtResolucao.Text             = RETORNO.DATASET(dsPesquisa, 0, "dtResolucao");
                        txtsDscUsuarioResolucao.Text    = RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioResolucao");
                        txtdtEnvioSW_Resolucao.Text     = RETORNO.DATASET(dsPesquisa, 0, "dtEnvioSW_Resolucao");
                        txtsCodigoSW_Resolucao.Text     = RETORNO.DATASET(dsPesquisa, 0, "sCodigoSW_Resolucao");

                        txtsDscEventoCompleto.Text      = RETORNO.DATASET(dsPesquisa, 0, "sDscEventoCompleto")
;



                        if (txtsEnviarSWCompleto.Text == "Não")
                        {
                            dv_CodigoSW.Visible = false;
                            dv_dtEnvioSW.Visible = false;
                            div_SW_Resolucao.Visible = false;
                        }




                        txtsDscTipoStatus.CssClass      = "form-control uppercase CaixaTextoMedio " + RETORNO.DATASET(dsPesquisa, 0, "sCor");

                        if (sPermiteAcao == "N")
                        {
                            Div_Acoes.Visible = false;
                            cmdEnviar.Visible = false;
                        }
                        else
                        {

                            Popular_GVAcoes(dsPesquisa);
                        }


                        ValidarCampos();
                        lblTituloSalvar.Text = "Confirma a envio das ações para o equipamento  " + txtsDscTipoEquipamento.Text + "?";
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }

                }
                else
                {
                    BreadCrumb.TitulodaPagina = "Incluir";
                    lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
                    cmdEnviar.Text = "Incluir";
                }

                //txt.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }

        void Popular_GVAcoes(DataSet ds)
        {

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                FrameWork.clsOcorrencia_Acoes objItem = new FrameWork.clsOcorrencia_Acoes();
                objItem.idRegistroAcao = Convert.ToInt32(row["idRegistroAcao"].ToString());
                objItem.idOcorrencia = Convert.ToInt32(row["idOcorrencia"].ToString());
                objItem.idAcao = Convert.ToInt32(row["idAcao"].ToString());
                objItem.sDscAcao = row["sDscAcao"].ToString();
                objItem.sObservacao = row["sObservacao"].ToString();
                objItem.idUsuarioAcao = Convert.ToInt32(row["idUsuarioAcao"].ToString());
                objItem.sDscUsuarioAcao = row["sDscUsuarioAcao"].ToString();
                objItem.dtEnvioEquipamento = row["dtEnvioEquipamento"].ToString();
                Base_gvAcoes.Add(objItem);
            }
            DataBind_dtgAcoes();
        }

        void DataBind_dtgAcoes()
        {
            dtgAcoes.DataSource = Base_gvAcoes;
            dtgAcoes.DataBind();
        }

        void ValidarCampos()
        {
            txtdtEnvioSW.ReadOnly = true;
            txtdtOcorrencia.ReadOnly = true;
            txtsCodigoSW.ReadOnly = true;
            txtsDscCliente.ReadOnly = true;
            txtsContatoTecnico.ReadOnly = true;
            txtsTelefoneTecnico.ReadOnly = true;
            txtsDscTipoEquipamento.ReadOnly = true;
            txtsEnviarSWCompleto.ReadOnly = true;
            txtsID.ReadOnly = true;
            txtsNomeArquivo.ReadOnly = true;
            txtsRegistroCompleto.ReadOnly = true;
            txtsDscTipoStatus.ReadOnly = true;
            txtdtResolucao.ReadOnly = true;
            txtsDscUsuarioResolucao.ReadOnly = true;
            txtdtEnvioSW_Resolucao.ReadOnly = true;
            txtsCodigoSW_Resolucao.ReadOnly = true;
            txtsDscEventoCompleto.ReadOnly = true;


            if (txtdtResolucao.Text != "" )
            {
                div_Resolucao.Visible = true;
                div_SelecaoAcao.Visible = false;
                cmdEnviar.Visible = false;
            }


        }
        void LimpaCampos()
        {
            div_Resolucao.Visible = false;
            txtdtResolucao.Text = "";

            //txtssDscCliente.Text = "";
            //txtsw_Account.Text = "";
            //hddidCliente.Value = "0";
            //PainelAtualizacao.Visible = false;
            //cmdSalvar.Text = "Salvar";
            //lblTituloPagina.Text = sTituloPagina;
        }

        void GravarAcoes()
        {
            string idOcorrencia = hddidOcorrencia.Value;

            try
            {
                foreach (GridViewRow item in dtgAcoes.Rows)
                {
                    string idRegistroAcao = item.Cells[0].Text;
                    string idAcao = item.Cells[1].Text;
                    string idUsuarioAcao = item.Cells[2].Text;
                    string sObservacao = item.Cells[4].Text;

                    if (idRegistroAcao == "0")
                    {
                        DataSet dsAcao;
                        Dictionary<String, String> vParametrosAcao = new Dictionary<string, string>();

                        vParametrosAcao.Add("@sFuncao", "REGISTRAR ACAO");
                        vParametrosAcao.Add("@sObservacao", sObservacao);
                        vParametrosAcao.Add("@idUsuario", idUsuarioAcao);
                        vParametrosAcao.Add("@idOcorrencia", idOcorrencia);
                        vParametrosAcao.Add("@idAcao", idAcao);
                        dsAcao = BD.ExecutarDataSet("sp_HUB_Manipula_tbl_Eventos_Ocorrencia_Acao", vParametrosAcao);
                    }
                }
    
                Pesquisar(idOcorrencia, "0");
                MensagemPagina.MostraMensagem_Sucesso("Enviado com sucesso!");
                
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }
        private bool AplicarValidacoes()
        {

            return true;
        }


        protected string RetornarScripts()
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();


            //Mensagens de Confirmação
            sb.Append("$v192(function() {");
            sb.Append("$v192(\"#dialog-Salvar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_SALVAR\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('#ContentPlaceHolder1_cmdEnviar').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");
            sb.Append("});");
            return sb.ToString();


        }

        void RegistraScript(string sScript)
        {
            sScript = RetornarScripts() + sScript;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "AddShowModalScript", sScript, true);
        }


        
        protected void cmdIncluirAcao_Click(object sender, EventArgs e)
        {
            string sMensagem = "";

            if (ddlAccao.SelectedValue == "0")
            {
                sMensagem = "Selecione uma ação!";
            }

            //else if (string.IsNullOrEmpty(txtsDDD.Text))
            //{
            //    sMensagem = "Informe o DDD!";
            //    //txtsDDD.Focus();
            //}

            else if (string.IsNullOrEmpty(txtsObservacaoAcao.Text))
            {
                sMensagem = "Informe uma observação!";
                // txtsTelefone.Focus();
            }
          

            if (sMensagem == "")
            {

                FrameWork.clsOcorrencia_Acoes  objItem = new FrameWork.clsOcorrencia_Acoes();

                objItem.idAcao = Convert.ToInt32(ddlAccao.SelectedValue.ToString());
                objItem.sObservacao = txtsObservacaoAcao.Text;
                objItem.idUsuarioAcao = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                objItem.sDscAcao = ddlAccao.SelectedItem.Text;
                objItem.sDscUsuarioAcao = IDENTITY.Variaveis.sUsuarioLogado();
                objItem.dtEnvioEquipamento = "Não Enviado";
                Base_gvAcoes.Add(objItem);
                DataBind_dtgAcoes();

                lblMensagem_Acao.Text = "";
                lblMensagem_Acao.Visible = false;

                //txtsDDD.Text = "";
                txtsObservacaoAcao.Text = "";
                ddlAccao.SelectedValue = "0";
            }
            else
            {
                lblMensagem_Acao.Text = sMensagem;
                lblMensagem_Acao.Visible = true;
            }

        }

        protected void dtgAcoes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            FrameWork.Grid.EsconderColunas(e, 0, 1, 2);
        }
    }
}