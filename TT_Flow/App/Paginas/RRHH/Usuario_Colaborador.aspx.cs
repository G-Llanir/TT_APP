using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class Usuario_Colaborador : System.Web.UI.Page
    {

        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();
            lblTituloSalvar.Text = "Confirma a Associação do Colaborador" + "?";

            if (!IsPostBack)
            {
                
                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Recursos.Consultar, true);
                    Pesquisar(Request["id"].ToString(), false);
                    PainelAtualizacao.Visible = true;
                    PopularDtg();
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Recursos.Incluir, true);
                    Pesquisar("0", true);
                    PainelAtualizacao.Visible = false;
                    PopularDtg();
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
                    Salvar_UsuarioColaborador();
                }
                else if (requestTarget == "funcao_Editar")
                {
                    Pesquisar(hddidColaborador.Value, true);
                }
            }
            PopularDtg();
            RegistraScript("");
        }


        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar(string idColaborador, bool bEdicao)
        {
            PopularCombos();
            cmdEditar.Visible = false;
            
            string sErro = "";

            try
            {

                if (idColaborador != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idColaborador", idColaborador);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
                    PopularDtg();

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidColaborador.Value = RETORNO.DATASET(dsPesquisa, 0, "idColaborador");
                        ddlidColaboradores.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idColaborador");
                        ddlidUsuarioAssociado.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idUsuarioIntegrado");                     

                        lblTituloPagina.Text = string.Format("Usuario Associado {0}", "Colaborador");  
                        BreadCrumb.TitulodaPagina = string.Format("Usuario Associado{0}", "Colaborador");

                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));

                        lblTituloSalvar.Text = "Confirma a Integração do Colaborador" + "?";
                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Recursos.Alterar);

                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Recursos.Alterar);
                        PainelAtualizacao.Visible = true;
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }
                }
                RegistraScript("");

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }

        void Salvar_UsuarioColaborador()
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string[] vidColaborador = hddidColaborador.Value.Split(',');
                    string idColaborador = vidColaborador[0].ToString();

                
                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro
                    vParametros.Add("@sFuncao", "INTEGRAR");
                    vParametros.Add("@idColaborador", ddlidColaboradores.SelectedValue);

                    vParametros.Add("@idUsuarioIntegrado", ddlidUsuarioAssociado.SelectedValue);
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());

                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idColaborador = RETORNO.DATASET(dsSalvar, "idColaborador");
                        Pesquisar(ddlidColaboradores.SelectedValue, false);
                        PopularDtg();
                        MensagemPagina.MostraMensagem_Sucesso(string.Format("Usuário Associado com sucesso!"));
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

        #region | Eventos
        protected void ddlidColaboradores_SelectedIndexChanged(object sender, EventArgs e)
        {
            Pesquisar(ddlidColaboradores.SelectedValue, false);
            PainelAtualizacao.Visible = true;
        }

        protected void PopularDtg()
        {

            string sFuncao = "CONSULTAR-ASSOCIADOS";

            DataTable tb;
            string sSql = "sp_Manipula_tbl_Flow_Colaboradores_Area";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);

            tb = BD.ExecutarDataTable(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb), true);
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhum Lançamento Localizado");
            }
        }
        #endregion

        #region | Validação 
        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            //if (ddlidColaboradores.SelectedValue != "0")
            //{
            //    if (ddlidUsuarioAssociado.SelectedValue == "0")
            //    {
            //        sMensagemErro = "Precisa de um Usuario";
            //    }
            //}

            if (ddlidColaboradores.SelectedValue == "0")
            {
                sMensagemErro = "Selecione um Colaborador";
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
            FUNCOES.Popula_Combo(ddlidUsuarioAssociado, "sp_Select 'Flow_Usuarios_Colaboradores'", "idUsuario", "sDscUsuario", false, "Selecione o Usuário", "0");//'Usuarios'
            FUNCOES.Popula_Combo(ddlidColaboradores, "sp_Select 'Flow_Colaboradores'", "idColaborador", "sDscColaborador", false, "Selecione o Colaborador", "0");
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