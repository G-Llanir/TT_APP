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

namespace TT_Flow.App.Paginas.WMS.Manutencao
{
    public partial class Unidades_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Unidades";
        string sProcedure = "sp_Manipula_tbl_Flow_Produtos_Unidade";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();
            manual.sNomeArquivo = "Manual-Unidades.pdf";
            if (FUNCOES.ValidaPermissao(Permissao.WMS.Cadastro_Unidades.Alterar) == false)
            {
                ddlsServico.Attributes.Add("disabled", "disabled");
                txtsDscUnidade.ReadOnly = true;
                txtsUnidade.ReadOnly = true;
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
                    Salvar_Unidades();
                }
            }

            RegistraScript("");
        }
        #endregion

        #region | Metodos Banco de Dados
        protected void Pesquisar(string idUnidade, bool bEdicao)
        {
            PopularCombos();
            string sErro = "";

            try
            {
                LimpaCampos();

                if (idUnidade != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idUnidade", idUnidade);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidUnidades.Value = RETORNO.DATASET(dsPesquisa, 0, "idUnidade");
                        txtidUnidade.Text = hddidUnidades.Value;
                        txtsDscUnidade.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscUnidade");
                        txtsUnidade.Text = RETORNO.DATASET(dsPesquisa, 0, "sUnidade");
                        ddlsServico.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sServico");
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sAtivo"));
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        //aba_Arquivos.Visible = true;
                        //Popular_Aba_Arquivos(idUnidades_TipoCliente);

                        lblTituloPagina.Text = "Unidade - " + txtsDscUnidade.Text;
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }
                }
                else
                {
                    lblTituloSalvar.Text = "Confirma a Inclusão da Unidade?";
                    lblTituloPagina.Text = "Nova Unidade";
                    //aba_Arquivos.Visible = false;
                    txtidUnidade.Text = "Novo";
                }
                RegistraScript("");
            }
            catch (Exception ex)
            {
                if (ex.Message == "Nenhum Registro Encontrado")
                    Response.Redirect("Unidades_Detalhe.aspx");
            }
        }

        void Salvar_Unidades()
        {
            string sErro = "";
            if (ValidarDados())
            {
                string[] vidUnidade = hddidUnidades.Value.Split(',');
                string idUnidade = vidUnidade[0].ToString();
                try
                {
                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idUnidade", idUnidade);
                    vParametros.Add("@sDscUnidade", txtsDscUnidade.Text);
                    vParametros.Add("@sUnidade", txtsUnidade.Text);
                    vParametros.Add("@sServico", ddlsServico.SelectedValue);
                    vParametros.Add("@sAtivo", ComboAtivo.Situacao_Recuperar());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());

                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {    
                        sErro = RETORNO.DATASET(dsSalvar, "sErro");

                        if (sErro == "")
                        {
                            idUnidade = RETORNO.DATASET(dsSalvar, "idUnidade");
                            Pesquisar(idUnidade, false);
                            MensagemPagina.MostraMensagem_Sucesso(string.Format("{1} gravado com sucesso!  </br><a href='Unidades_Detalhe.aspx'>Clique aqui para incluir um novo {1}.</a>", Request.RawUrl.ToString(), sTituloPagina));
                        }
                        else
                        {                            
                            txtsUnidade.Text = "";
                            MensagemPagina_Unidades.MostraMensagem_Erro(RETORNO.DATASET(dsSalvar, "sErro"), false);                            
                        }
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
            RegistraScript("");
        }
        #endregion

        #region | Popular Aba Arquivos
        
        #endregion

        #region | Limpar Campos
        void LimpaCampos()
        {
            hddidUnidades.Value = "0";
            txtsDscUnidade.Text = "";
            txtsUnidade.Text = "";
            ddlsServico.SelectedValue = "0";
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

            if (ddlsServico.SelectedValue == "0")
            {
                sMensagemErro = "Selecione o Tipo!";
            }

            if (txtsDscUnidade.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Descrição Válida!";
            }

            if (txtsUnidade.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Unidade Válida!";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina_Unidades.MostraMensagem_Erro(sMensagemErro);
                if (txtidUnidade.Text == "")
                {
                    txtidUnidade.Text = "Novo";
                }
            }
            return bRetorno;
        }
        #endregion

        #region | Combos/DDL
        void PopularCombos()
        {
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

            if (txtidUnidade.Text != "Novo")
                id = Convert.ToInt32(txtidUnidade.Text) + 1;

            Response.Redirect($"Unidades_Detalhe.aspx?id={id}");
        }

        protected void cmdRetornar_click(object sender, EventArgs e)
        {
            int id = 0;

            if (txtidUnidade.Text != "Novo")
                id = Convert.ToInt32(txtidUnidade.Text) - 1;

            Response.Redirect($"Unidades_Detalhe.aspx?id={id}");
        }
        #endregion

    }
}