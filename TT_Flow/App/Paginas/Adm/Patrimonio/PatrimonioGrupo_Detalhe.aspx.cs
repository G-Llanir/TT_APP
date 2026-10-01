using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;

namespace TT_Flow.App.Paginas.Adm.Patrimonio
{
    public partial class PatrimonioGrupo_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Grupo de Patrimônio";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_Patrimonio_Grupo";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            if (!IsPostBack)
            {
                

                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Patrimonio.Grupos.Consultar, true);
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Patrimonio.Grupos.Incluir, true);
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
                    Salvar_Patrimonio();
                }
                else if (requestTarget == "funcao_Editar")
                {
                    Pesquisar(hddidPatrimonioGrupo.Value, true);
                }
            }

            RegistraScript("");

        }


        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar(string idPatrimonioGrupo, bool bEdicao)
        {
            PopularCombos();
            cmdEditar.Visible = false;

            string sErro = "";

            try
            {
                LimpaCampos();

                if (idPatrimonioGrupo != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idPatrimonioGrupo", idPatrimonioGrupo);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidPatrimonioGrupo.Value = RETORNO.DATASET(dsPesquisa, 0, "idPatrimonioGrupo");
                        txtidPatrimonioGrupo.Text = RETORNO.DATASET(dsPesquisa, 0, "idPatrimonioGrupo");
                        txtsDscPatrimonio.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscPatrimonio");
                        txtnVidaUtil.Text = RETORNO.DATASET(dsPesquisa, 0, "nVidaUtil");
                        txtnTxAnualDepreciacao.Text = RETORNO.DATASET(dsPesquisa, 0, "nTxAnualDepreciacao");

                        vParametros.Add("@sSituacao", ComboAtivo.Situacao_Recuperar());

                        ddlidContabil.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idContabil");//ddlidContabil

                        ddlidContabil_Depreciacao.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idContabil_Depreciacao");//ddlidContabil_Depreciacao
   
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));

                        lblTituloPagina.Text = string.Format("Grupo de Patrimônio {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscPatrimonio"));  
                        BreadCrumb.TitulodaPagina = string.Format("Grupo de Patrimônio {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscPatrimonio"));

                        lblTituloSalvar.Text = "Confirma a Alteração do " + lblTituloPagina.Text + "?";
                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Patrimonio.Grupos.Alterar);

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
                    txtidPatrimonioGrupo.Text = "Novo";
                    lblTituloSalvar.Text = "Confirma a Inclusão do Patrimonio?";
                    cmdSalvar.Text = "Incluir";
                    txtsDscPatrimonio.Focus();

                }
                RegistraScript("");

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }

        void Salvar_Patrimonio()
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string[] vidPatrimonioGrupo = hddidPatrimonioGrupo.Value.Split(',');
                    string idPatrimonioGrupo = vidPatrimonioGrupo[0].ToString();

                
                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idPatrimonioGrupo", idPatrimonioGrupo);

                    vParametros.Add("@idContabil", ddlidContabil.SelectedValue);
                    vParametros.Add("@idContabil_Depreciacao", ddlidContabil_Depreciacao.SelectedValue);

                    vParametros.Add("@sSituacao", ComboAtivo.Situacao_Recuperar());

                    vParametros.Add("@sDscPatrimonio", txtsDscPatrimonio.Text);
                    vParametros.Add("@nVidaUtil", BD.Conversoes.Numerico(txtnVidaUtil));
                    vParametros.Add("@nTxAnualDepreciacao", BD.Conversoes.Numerico(txtnTxAnualDepreciacao));

                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idPatrimonioGrupo = RETORNO.DATASET(dsSalvar, "idPatrimonioGrupo");
                        Pesquisar(idPatrimonioGrupo, false);
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

        #region | Limpar Campos
        void LimpaCampos()
        {
            hddidPatrimonioGrupo.Value = "0";
            txtidPatrimonioGrupo.Text = "Novo";

            txtsDscPatrimonio.Text = "";
            txtnVidaUtil.Text = "";
            txtnTxAnualDepreciacao.Text = "";

            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
        }

        #endregion

        #region | Validação 
        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (txtsDscPatrimonio.Text.Length < 5)
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

        #region | Combos/DDL
        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidContabil, "sp_Select 'Flow_Contabil_Patrimonio'", "idContabil", "sCodContabil", false, "Selecione o código Contábil", "0");
            FUNCOES.Popula_Combo(ddlidContabil_Depreciacao, "sp_Select 'Flow_CodigoContabil'", "idContabil", "sCodContabil", false, "Selecione o código Contábil de depreciação", "0");//ddlidContabil_Depreciacao
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

    }
}