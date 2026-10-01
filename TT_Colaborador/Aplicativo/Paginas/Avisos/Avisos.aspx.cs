using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;

namespace TT_Colaborador.Aplicativo.Paginas.Avisos
{
    public partial class Avisos : System.Web.UI.Page
    {
        string sTituloPagina = "Avisos";
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Area";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            if (!IsPostBack)
            {
                string idUsuarioLogado = IDENTITY.Variaveis.idUsuario(); //
                Pesquisar(idUsuarioLogado, false); // Tem que receber id do Usuario Logado
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                var requestArgs = this.Request["__EVENTARGUMENT"];

                if (requestTarget == "funcao_SAIR")
                {
                    FUNCOES.DirecionaPagina("/Aplicativo/MenuColaborador.aspx");
                }
                else if (requestTarget == "funcao_SALVAR")
                {
                    Salvar_Colaborador();
                }
                else if (requestTarget == "funcao_Editar")
                {
                    Pesquisar(hddidColaborador.Value, true);
                }
            }

            RegistraScript("");

        }
        #endregion


        #region |Metodos Banco de Dados
        protected void Pesquisar(string idUsuario, bool bEdicao)
        {
            PopularCombos();

            string sErro = "";

            try
            {
                LimpaCampos();

                if (idUsuario != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR-USUARIO");
                    vParametros.Add("@idUsuarioIntegrado", idUsuario);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidColaborador.Value = RETORNO.DATASET(dsPesquisa, 0, "idColaborador");

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));


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

        void Salvar_Colaborador()
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
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idColaborador", idColaborador);


                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idColaborador = RETORNO.DATASET(dsSalvar, "idPatrimonioGrupo");
                        Pesquisar(idColaborador, false);
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
            //RegistraScript("");
        }

        #endregion

        #region | Limpar Campos
        void LimpaCampos()
        {
            hddidColaborador.Value = "0";

            PainelAtualizacao.Visible = false;
            //lblTituloPagina.Text = sTituloPagina;
        }

        #endregion

        #region | Validação 
        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";


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