using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;


namespace TT_Flow.App.Paginas.Adm.Manutencao
{
    public partial class LancamentoContabil_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Código Contábil";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_CodigoContabil";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();


            if (!IsPostBack)
            {
                PopularCombos();
                

                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Administracao.CodigoContabil.Consultar, true);
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Administracao.CodigoContabil.Incluir, true);
                    Pesquisar("0", true);

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
                    Salvar_LancamentoContabil();
                }
                else if (requestTarget == "funcao_Editar")
                {
                    Pesquisar(hddidContabil.Value, true);
                }
            }

            RegistraScript("");

        }


        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar(string idContabil, bool bEdicao)
        {
            PopularCombos();

            cmdEditar.Visible = false;
            aba_Historico.Visible = false;

            string sErro = "";

            try
            {
                LimpaCampos();

                if (idContabil != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idContabil", idContabil);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidContabil.Value = RETORNO.DATASET(dsPesquisa, 0, "idContabil");
                        txtidContabil.Text = RETORNO.DATASET(dsPesquisa, 0, "idContabil");
                        txtsCodContabil.Text = RETORNO.DATASET(dsPesquisa, 0, "sCodContabil");
                        txtsDscCodContabil.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscCodContabil");
                        ddlidContabil.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idContabilPai");
                        ddlidContabil.Items.Remove(ddlidContabil.Items.FindByValue(RETORNO.DATASET(dsPesquisa, 0, "idContabil")));

                        ddlsExibirPatrimonio.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sExibirPatrimonio");
                        ddlsExibirFinanceiro.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sExibirFinanceiro");

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));

                        lblTituloPagina.Text = string.Format("Código Contábil {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscCodContabil"));  
                        BreadCrumb.TitulodaPagina = string.Format("Código Contábil {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscCodContabil"));

                        lblTituloSalvar.Text = "Confirma a Alteração da " + lblTituloPagina.Text + "?";
                        lblTituloEdiar.Text = "Deseja editar o Lançamento " + lblTituloPagina.Text + "?";

                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Administracao.CodigoContabil.Alterar);
                        Popular_Aba_Historico(dsPesquisa);

                        if (!bEdicao)
                        {
                            //Popular_Aba_Documentos(idColaborador);

                            //if (!FUNCOES.ValidaPermissao(Permissao.Adm.Alterar))
                            //{
                            //    cmdEditar.Visible = false;
                            //    cmdSalvar.Visible = false;
                            //}
                        }

                    }
                    else
                    {
                        throw new Exception(sErro);
                    }

                }
                else
                {
                    BreadCrumb.TitulodaPagina = string.Format("Novo {0}", sTituloPagina);
                    lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
                    txtidContabil.Text = "Novo";
                    lblTituloSalvar.Text = "Confirma a Inclusão do Lançamento?";
                    cmdSalvar.Text = "Incluir";
                    txtsCodContabil.Focus();

                }
                RegistraScript("");

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }


        void Salvar_LancamentoContabil()
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string[] vidContabil = hddidContabil.Value.Split(',');
                    string idContabil = vidContabil[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idContabil",          idContabil);
                    vParametros.Add("@sCodContabil",        txtsCodContabil.Text);
                    vParametros.Add("@sDscCodContabil",     txtsDscCodContabil.Text);
                    vParametros.Add("@idContabilPai",       ddlidContabil.SelectedValue);

                    vParametros.Add("@sExibirPatrimonio", ddlsExibirPatrimonio.SelectedValue);
                    vParametros.Add("@sExibirFinanceiro", ddlsExibirFinanceiro.SelectedValue);

                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idContabil = RETORNO.DATASET(dsSalvar, "idContabil");
                        Pesquisar(idContabil, false);
                        //MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                        MensagemPagina.MostraMensagem_Sucesso(string.Format("{1} gravado com sucesso!  </br><a href='{0}?id=0'>Clique aqui para incluir um novo {1}.</a>", Request.RawUrl.ToString(), sTituloPagina));
                    }
                    else
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }


                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }

            }
            RegistraScript("");
        }

        #endregion

        #region | Aba Documentos 
        //void Popular_Aba_Documentos(string idColaborador)
        //{
        //    frmDocumentos.Attributes.Add("src", string.Format("../RRHH/Documentos_RRHH.aspx?idObjeto={0}&sTipoObjeto={1}", idContabil, "Adm"));
        //    aba_Documentos.Visible = true;

        //}
        #endregion

        #region | Limpar Campos
        void LimpaCampos()
        {
            hddidContabil.Value = "0";
            txtsCodContabil.Text = "";
            txtsDscCodContabil.Text = "";
            ddlidContabil.Text = "0";
            txtidContabil.Text = "Novo";
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;

            ddlsExibirPatrimonio.SelectedValue = "N";
            ddlsExibirFinanceiro.SelectedValue = "N";
        }

        #endregion

        #region | Validação 
        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (txtsDscCodContabil.Text.Length < 5)
            {
                sMensagemErro = "Descrição inválida!";
            }


            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }


            return bRetorno;
        }
        #endregion

        #region | Script 
        void RegistraScript(string sFuncao)
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
            sb.Append("$v192('[id*=cmdSalvar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("$v192(\"#dialog-Editar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_Editar\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=cmdEditar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Editar').dialog('open');");
            sb.Append("});");


            sb.Append("});");
            //Caixa de seleção de datas
            //sb.Append("$(function() {$('[id*=txtdtRequisicao]').datepicker({");
            //sb.Append("autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }
        #endregion

        #region | Combos/DDL
        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidContabil, "sp_Select 'Flow_Adm_Contabil_Pai'", "idContabil", "sDscCodContabil", false, "Selecione o código Contábil Pai", "0");
        }
        #endregion


        void Popular_Aba_Historico(DataSet ds)
        {
            gv_Historico.DataSource = ds.Tables[1];
            gv_Historico.DataBind();
            aba_Historico.Visible = true;
        }

    }
}