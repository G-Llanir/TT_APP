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

namespace TT_Flow.App.Paginas.Adm.Patrimonio
{
    public partial class Categoria_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Categoria";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_Patrimonio_Categoria";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            if (!IsPostBack)
            {
                

                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Patrimonio.Categoria.Consultar, true);
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Patrimonio.Categoria.Incluir, true);
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
                    Pesquisar(hddidCategoria.Value, true);
                }
            }

            RegistraScript("");

        }


        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar(string idCategoria, bool bEdicao)
        {
            PopularCombos();

            string sErro = "";

            try
            {
                LimpaCampos();

                if (idCategoria != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idCategoria", idCategoria);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidCategoria.Value = RETORNO.DATASET(dsPesquisa, 0, "idCategoria");
                        txtidCategoria.Text = RETORNO.DATASET(dsPesquisa, 0, "idCategoria");
                        txtsCodigoCategoria.Text = RETORNO.DATASET(dsPesquisa, 0, "sCodigoCategoria");
                        txtsDscCategoria.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscCategoria");

                        ddlidCategoriaPai.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCategoriaPai");
                        ddlidCategoriaPai.Items.Remove(ddlidCategoriaPai.Items.FindByValue(RETORNO.DATASET(dsPesquisa, 0, "idCategoria")));

                        vParametros.Add("@sATivo", ComboAtivo.Situacao_Recuperar());

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));

                        lblTituloPagina.Text = string.Format("Categoria {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscCategoria"));  
                        BreadCrumb.TitulodaPagina = string.Format("Categoria {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscCategoria"));

                        lblTituloSalvar.Text = "Confirma a Alteração do " + lblTituloPagina.Text + "?";

                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Patrimonio.Categoria.Alterar);

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
                    txtidCategoria.Text = "Novo";
                    lblTituloSalvar.Text = "Confirma a Inclusão da Categoria?";
                    cmdSalvar.Text = "Incluir";
                    txtsCodigoCategoria.Focus();

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
                    string[] vidLocal = hddidCategoria.Value.Split(',');
                    string idCategoria = vidLocal[0].ToString();

                
                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idCategoria", idCategoria);
                    vParametros.Add("@sCodigoCategoria", txtsCodigoCategoria.Text);
                    vParametros.Add("@sDscCategoria", txtsDscCategoria.Text);
                    vParametros.Add("@idCategoriaPai", ddlidCategoriaPai.SelectedValue);
                    vParametros.Add("@sAtivo", ComboAtivo.Situacao_Recuperar());

                    vParametros.Add("@idusuarioatualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idCategoria = RETORNO.DATASET(dsSalvar, "idCategoria");
                        Pesquisar(idCategoria, false);
                        MensagemPagina.MostraMensagem_Sucesso(string.Format("{1} gravado com sucesso!  </br><a href='{0}?id=0'>Clique aqui para incluir uma nova {1}.</a>", Request.RawUrl.ToString(), sTituloPagina));

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
            hddidCategoria.Value = "0";
            txtidCategoria.Text = "Novo";

            txtsDscCategoria.Text = "";

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

            if (Validacoes.ValidarTexto(txtsCodigoCategoria))
            {
                sMensagemErro = "Informe um Código para a Categoria!";
            }


            if (Validacoes.ValidarTexto(txtsDscCategoria))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Descrição da Categoria inválida!";
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
            FUNCOES.Popula_Combo(ddlidCategoriaPai, "sp_Select 'tbl_Flow_Adm_Patrimonio_Categoria'", "idCategoria", "sDscCategoria", false, "Selecione a Categoria", "0");
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