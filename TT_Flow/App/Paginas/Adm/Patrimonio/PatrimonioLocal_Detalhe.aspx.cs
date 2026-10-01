using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using System.Web.UI.WebControls;

namespace TT_Flow.App.Paginas.Adm.Patrimonio
{
    public partial class PatrimonioLocal_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Local de Patrimônio";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_Patrimonio_Local";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            if (!IsPostBack)
            {
                

                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Patrimonio.Local.Consultar, true);
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Patrimonio.Local.Incluir, true);
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
                    Pesquisar(hddidPatrimonioLocal.Value, true);
                }
            }

            RegistraScript("");

        }


        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar(string idLocal, bool bEdicao)
        {
            PopularCombos();
            cmdEditar.Visible = false;

            string sErro = "";

            try
            {
                LimpaCampos();

                if (idLocal != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idLocal", idLocal);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidPatrimonioLocal.Value = RETORNO.DATASET(dsPesquisa, 0, "idLocal");
                        txtidPatrimonioLocal.Text = RETORNO.DATASET(dsPesquisa, 0, "idLocal");
                        txtsDscLocal.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscLocal");

                        ddlidDepartamento.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idDepartamentoPadrao");

                        vParametros.Add("@sSituacao", ComboAtivo.Situacao_Recuperar());

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));



                        lblTituloPagina.Text = string.Format("Local {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscLocal"));
                        BreadCrumb.TitulodaPagina = string.Format("Local {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscLocal"));

                        //lblTituloPagina.Text = string.Format("{0} {1}", sTituloPagina, RETORNO.DATASET(dsPesquisa, 0, "sDscLocal"));  
                        //BreadCrumb.TitulodaPagina = string.Format("{1}", RETORNO.DATASET(dsPesquisa, 0, "sDscLocal"));

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
                    txtidPatrimonioLocal.Text = "Novo";
                    lblTituloSalvar.Text = "Confirma a Inclusão do Local de Patrimonio?";
                    cmdSalvar.Text = "Incluir";
                    txtsDscLocal.Focus();

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
                    string[] vidLocal = hddidPatrimonioLocal.Value.Split(',');
                    string idPatrimonioLocal = vidLocal[0].ToString();

                
                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idLocal", idPatrimonioLocal);

                    vParametros.Add("@idDepartamentoPadrao", ddlidDepartamento.SelectedValue);

                    vParametros.Add("@sSituacao", ComboAtivo.Situacao_Recuperar());

                    vParametros.Add("@sDscLocal", txtsDscLocal.Text);


                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idPatrimonioLocal = RETORNO.DATASET(dsSalvar, "idLocal");
                        Pesquisar(idPatrimonioLocal, false);
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
            hddidPatrimonioLocal.Value = "0";
            txtidPatrimonioLocal.Text = "Novo";

            txtsDscLocal.Text = "";

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

            if (txtsDscLocal.Text.Length < 5)
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
            FUNCOES.Popula_Combo(ddlidDepartamento, "sp_Select 'Flow_Departamentos_Local'", "idDepartamento", "sDscDepartamento", false, "Selecione o Departamento", "0");
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