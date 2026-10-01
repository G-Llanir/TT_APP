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
using static Permissao;
using TT_Flow.FrameWork;
using System.Data.SqlClient;
using GRID = TT.FrameWork.Grid;
using TT_Hub.App.Paginas.Requisicao;
using System.Globalization;
using System.Linq;
using TT_Flow.App.Paginas.Manutencao.Feriados;

namespace TT_Flow.App.Paginas.Comercial.Manutencao
{ 
    public partial class Segmentos_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Segmentos";
        string sProcedure = "sp_Manipula_tbl_Flow_Segmentos_TipoCliente";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();
            manual.sNomeArquivo = "Manual-Segmentos.pdf";
            div_Cor.Visible = false;
            if (FUNCOES.ValidaPermissao(Permissao.Segmentos.AlterarSegmentos) == false)
            {
                ddlidTipo.Attributes.Add("disabled", "disabled");
                txtsDscSegmento_TipoCliente.ReadOnly = true;
                ddlsCor.Attributes.Add("disabled", "disabled");
                ComboAtivo.ReadOnly = true;
                cmdSalvar.Visible = false;
            }

            if (!IsPostBack)
            {
                if (Request["id"] != null)
                {
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    Pesquisar("0", true);
                    //aba_Arquivos.Visible = false;
                }
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                var requestArgs = this.Request["__EVENTARGUMENT"];
                //aba_Arquivos.Visible = false;

                if (requestTarget == "funcao_SAIR")
                {
                    FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                }
                else if (requestTarget == "funcao_SALVAR")
                {
                    Salvar_Segmentos();
                }
                else if (requestTarget == "funcao_Editar")
                {
                }
            }

            RegistraScript("");

        }
        #endregion

        #region | Metodos Banco de Dados
        protected void Pesquisar(string idSegmento_TipoCliente, bool bEdicao)
        {
            PopularCombos();

            string sErro = "";

            try
            {
                LimpaCampos();

                if (idSegmento_TipoCliente != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idSegmento_TipoCliente", idSegmento_TipoCliente);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidSegmentos.Value = RETORNO.DATASET(dsPesquisa, 0, "idSegmento_TipoCliente");
                        txtidSegmento_TipoCliente.Text = hddidSegmentos.Value;
                        txtsDscSegmento_TipoCliente.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscSegmento_TipoCliente");
                        ddlsCor.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sCor");
                        ddlidTipo.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idTipo");
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sAtivo"));

                        if (ddlidTipo.SelectedValue == "1")
                        {
                            div_Cor.Visible = true;
                        }

                        if (ddlidTipo.SelectedValue == "1")
                        {
                            lblTituloPagina.Text = "Segmento - " + txtsDscSegmento_TipoCliente.Text;
                        }
                        if (ddlidTipo.SelectedValue == "2")
                        {
                            lblTituloPagina.Text = "Tipo Cliente - " + txtsDscSegmento_TipoCliente.Text;
                        }

                        //aba_Arquivos.Visible = true;
                        //Popular_Aba_Arquivos(idSegmento_TipoCliente);
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }

                }
                else
                {
                    lblTituloSalvar.Text = "Confirma a Inclusão do Segmento?";
                    lblTituloPagina.Text = "Novo Segmento";
                    //aba_Arquivos.Visible = false;
                    txtidSegmento_TipoCliente.Text = "Novo";
                }
                RegistraScript("");

            }
            catch (Exception ex)
            {

                if (ex.Message == "Nenhum Registro Encontrado")
                    Response.Redirect("Segmentos_Detalhe.aspx");
            }

        }

        void Salvar_Segmentos()
        {
            string sErro = "";
            if (ValidarDados())
            {
                string[] vidSegmento = hddidSegmentos.Value.Split(',');
                string idSegmento = vidSegmento[0].ToString();
                try
                {
                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro


                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idSegmento_TipoCliente", idSegmento);
                    vParametros.Add("@sDscSegmento_TipoCliente", txtsDscSegmento_TipoCliente.Text);
                    vParametros.Add("@idTipo", ddlidTipo.SelectedValue);
                    vParametros.Add("@sAtivo", ComboAtivo.Situacao_Recuperar());

                    if (ddlidTipo.SelectedValue == "2")
                    {
                        vParametros.Add("@sCor", "0");
                    }
                    else
                    {
                        vParametros.Add("@sCor", ddlsCor.SelectedValue);
                    }

                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idSegmento = RETORNO.DATASET(dsSalvar, "idSegmento_TipoCliente");
                        Pesquisar(idSegmento, false);
                        MensagemPagina.MostraMensagem_Sucesso(string.Format("{1} gravado com sucesso!  </br><a href='Segmentos_Detalhe.aspx'>Clique aqui para incluir um novo {1}.</a>", Request.RawUrl.ToString(), sTituloPagina));
                    }
                    else
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }

                    //aba_Arquivos.Visible = true;

                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                    //aba_Arquivos.Visible = false;
                }

            }
            if (ddlidTipo.SelectedValue == "1")
            {
                div_Cor.Visible = true;
            }
            RegistraScript("");
        }
        #endregion

        #region | Popular Aba Arquivos
        //void Popular_Aba_Arquivos(string idRegistro)
        //{
        //    frmArquivos.Attributes.Add("src", string.Format("../Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", idRegistro, "Segmentos"));
        //}
        #endregion

        #region | Limpar Campos
        void LimpaCampos()
        {
            hddidSegmentos.Value = "0";
            txtsDscSegmento_TipoCliente.Text = "";
            ddlidTipo.SelectedValue = "0";
            ddlsCor.SelectedValue = "0";
            ComboAtivo.Text = "Ativo";
            lblTituloPagina.Text = sTituloPagina;
            PainelAtualizacao.Visible = false;
        }
        #endregion

        #region | Validação 
        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlidTipo.SelectedValue == "0")
            {
                sMensagemErro = "Selecione o Tipo!";
            }

            if (txtsDscSegmento_TipoCliente.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Descrição Válido!";
            }

            if (ddlidTipo.SelectedValue == "1")
            {
                if (ddlsCor.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione a Cor!";
                }
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina_Segmentos.MostraMensagem_Erro(sMensagemErro);
                if(txtidSegmento_TipoCliente.Text == "")
                {
                    txtidSegmento_TipoCliente.Text = "Novo";
                }
            }

            return bRetorno;
        }
        #endregion

        #region | Combos/DDL
        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlsCor, "sp_Select 'COR'", "sCor", "sDscCor", false, "Selecione a Cor", "0");
        }
        #endregion

        #region | Script 
        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

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
            sb.Append("$v192('[id*=cmdSalvar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");
            sb.Append("});");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }
        #endregion

        #region | EVENTOS
        protected void cmdAvancar_click(object sender, EventArgs e)
        {
            int id = 0;

            if (txtidSegmento_TipoCliente.Text != "Novo")
                id = Convert.ToInt32(txtidSegmento_TipoCliente.Text) + 1;

            Response.Redirect($"Segmentos_Detalhe.aspx?id={id}");
        }

        protected void cmdRetornar_click(object sender, EventArgs e)
        {
            int id = 0;

            if (txtidSegmento_TipoCliente.Text != "Novo")
                id = Convert.ToInt32(txtidSegmento_TipoCliente.Text) - 1;

            Response.Redirect($"Segmentos_Detalhe.aspx?id={id}");
        }
        #endregion

        #region | Selected Index Changed
        protected void ddlidTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(ddlidTipo.SelectedValue == "1")
            {
                div_Cor.Visible = true;
            }
        }
        #endregion
    }
}