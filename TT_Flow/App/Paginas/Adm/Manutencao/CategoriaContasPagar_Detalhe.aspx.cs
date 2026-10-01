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
    public partial class CategoriaContasPagar_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Categoria Contas Pagar";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_Contas_Pagar_Categoria";

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
                    FUNCOES.ValidaPermissao(Permissao.Administracao.CategoriaContasPagar.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Administracao.CategoriaContasPagar.Incluir, true);
                    Pesquisar("0");

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
                    Salvar_CategoriaPagar();
                }
                else if (requestTarget == "funcao_Editar")
                {
                    Pesquisar(hddidCategoriaPagar.Value);
                }
            }

            RegistraScript("");            
        }


        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar(string idCategoriaPagar)
        {
            PopularCombos();

            cmdEditar.Visible = false;
            //aba_Historico.Visible = false;
            //Div_Parceiro.Visible = false;
            string sErro = "";

            try
            {
                LimpaCampos();

                if (idCategoriaPagar != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idCategoriaPagar", idCategoriaPagar);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidCategoriaPagar.Value               = RETORNO.DATASET(dsPesquisa, 1, 0, "idCategoriaPagar");
                        txtidCategoriaPagar.Text                = RETORNO.DATASET(dsPesquisa, 1, 0, "idCategoriaPagar");
                        txtsDscCategoriaPagar.Text              = RETORNO.DATASET(dsPesquisa, 1, 0, "sDscCategoriaPagar");
                        
                        ddlidCategoriaTipo.SelectedValue        = RETORNO.DATASET(dsPesquisa, 1, 0, "idCategoriaTipo");
                        ddlidContabil.SelectedValue             = RETORNO.DATASET(dsPesquisa, 1, 0, "idContabil");                       
                        
                        ddlidCategoriaPagarPai.SelectedValue = RETORNO.DATASET(dsPesquisa, 1, 0, "idCategoriaPagarPai");

                        ddlsDataExibirCaixa.SelectedValue = RETORNO.DATASET(dsPesquisa, 1, 0, "sDataExibirCaixa");
                        ddlsDataExibirContabil.SelectedValue = RETORNO.DATASET(dsPesquisa, 1, 0, "sDataExibirContabil");

                        SwitchEmprestimo.Definir(RETORNO.DATASET(dsPesquisa, 1, 0, "sEmprestimoPag"),"É Empréstimo","");
                        SwitchExibirDash.Definir(RETORNO.DATASET(dsPesquisa, 1, 0, "sExibirDash"), "Exibir no DashBoard", "");
                        SwitchDespesaFixa.Definir(RETORNO.DATASET(dsPesquisa, 1, 0, "sDespesaFixaPag"),"É Custo Fixo", "");
                        SwitchDespesa.Definir(RETORNO.DATASET(dsPesquisa, 1, 0, "sDespesa"), "É Despesa", "");
                        //SwitchExibirDespesa.Definir(RETORNO.DATASET(dsPesquisa, 1, 0, "sExibirDespesas"), "Exibir em Relatório de Despesas", "");

                        SwitchProdutoFinanceiro.Definir(RETORNO.DATASET(dsPesquisa, 1, 0, "sProdutoFinanceiroPag"), "Produtos Financeiros", "");

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 1, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 1, 0, "sDscUsuarioAtualizacao"));

                        lblTituloPagina.Text = string.Format("Categoria Contas Pagar {0}", RETORNO.DATASET(dsPesquisa, 1, 0, "sDscCategoriaPagar"));  
                        BreadCrumb.TitulodaPagina = string.Format("Categoria Contas Pagar {0}", RETORNO.DATASET(dsPesquisa, 1, 0, "sDscCategoriaPagar"));

                        lblTituloSalvar.Text = "Confirma a Alteração da " + lblTituloPagina.Text + "?";
                        lblTituloEdiar.Text = "Deseja editar a Categoria " + lblTituloPagina.Text + "?";

                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Administracao.CategoriaContasPagar.Incluir);
                        //Popular_Aba_Historico(dsPesquisa);

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
                    txtidCategoriaPagar.Text = "Nova";
                    lblTituloSalvar.Text = "Confirma a Inclusão da Categoria?";
                    cmdSalvar.Text = "Incluir";                    

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtsDscCategoriaPagar]').focus();", true);
                }
                RegistraScript("");

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }


        void Salvar_CategoriaPagar()
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string[] vidCategoriaPagar = hddidCategoriaPagar.Value.Split(',');
                    string idCategoriaPagar = vidCategoriaPagar[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idCategoriaPagar",        idCategoriaPagar);

                    vParametros.Add("@sDscCategoriaPagar",      txtsDscCategoriaPagar.Text);
                    vParametros.Add("@idCategoriaPagarPai",     ddlidCategoriaPagarPai.SelectedValue);
                    vParametros.Add("@idCategoriaTipo",         ddlidCategoriaTipo.SelectedValue);
                    vParametros.Add("@idContabil",              ddlidContabil.SelectedValue);
                    vParametros.Add("@sEmprestimoPag", SwitchEmprestimo.Recuperar());
                    vParametros.Add("@sDespesaFixaPag", SwitchDespesaFixa.Recuperar());
                    vParametros.Add("@sExibirDash", SwitchExibirDash.Recuperar());
                    vParametros.Add("@sDespesa", SwitchDespesa.Recuperar());
                    //vParametros.Add("@sExibirDespesas", SwitchExibirDespesa.Recuperar());
                    vParametros.Add("@sDataExibirCaixa", ddlsDataExibirCaixa.SelectedValue);
                    vParametros.Add("@sDataExibirContabil", ddlsDataExibirContabil.SelectedValue);

                    vParametros.Add("@sProdutoFinanceiroPag", SwitchProdutoFinanceiro.Recuperar());

                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idCategoriaPagar = RETORNO.DATASET(dsSalvar, "idCategoriaPagar");
                        Pesquisar(idCategoriaPagar);
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
            hddidCategoriaPagar.Value = "0";

            txtidCategoriaPagar.Text = "Novo";
            txtsDscCategoriaPagar.Text = "";
            ddlidCategoriaPagarPai.SelectedValue = "0";
            ddlidCategoriaTipo.SelectedValue = "0";
            ddlidContabil.SelectedValue = "0";            
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
            SwitchEmprestimo.Definir("N", "É Empréstimo", "");
            SwitchExibirDash.Definir("S", "Exibir no DashBoard", "");
            SwitchDespesaFixa.Definir("N", "É Custo Fixo", "");
            SwitchDespesa.Definir("N", "É Receita", "");
            //SwitchExibirDespesa.Definir("N", "Exibir em Relatório de Despesas", "");
            SwitchProdutoFinanceiro.Definir("N", "Produtos Financeiros", "");

            // ddlidParceiro.SelectedValue = "0";
            
        }

        #endregion

        #region | Validação 
        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (txtsDscCategoriaPagar.Text.Length < 4)
            {
                sMensagemErro = "Descrição inválida minimo de 4 caracteres!";
            }
            if (ddlidCategoriaTipo.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Categoria!";
            }
            if (ddlidContabil.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Código Contábil!";
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

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }
        #endregion

        #region | Combos/DDL
        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidCategoriaTipo, "sp_Select 'Flow_Adm_Contas_Pagar_Categoria_Tipo'", "idCategoriaTipo", "sDscCategoriaTipo", false, "Selecione a Categoria", "0");
            FUNCOES.Popula_Combo(ddlidCategoriaPagarPai, "sp_Select 'Flow_Adm_Contas_Pagar_Categoria_Pai'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Selecione a Categoria Pai", "0");
            FUNCOES.Popula_Combo(ddlidContabil, "sp_Select 'Flow_Adm_Contabil_Pai'", "idContabil", "sDscCodContabil", false, "Selecione o código Contábil Pai", "0");
            
        }
        
        #endregion


        //void Popular_Aba_Historico(DataSet ds)
        //{
        //    gv_Historico.DataSource = ds.Tables[1];
        //    gv_Historico.DataBind();
        //    aba_Historico.Visible = true;
        //}

    }
}