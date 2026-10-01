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

namespace TT_Login
{
    public partial class AlterarDados : System.Web.UI.Page
    {
        string sTituloPagina = "Alterar Dados";
        string sProcedure = "sp_Manipula_tbl_Usuarios";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidarPermissaoAcesso();

            RegistraScript("");
            if (!IsPostBack)
            {

                PesquisarUsuario(IDENTITY.Variaveis.idUsuario());

            }
            else
            {

                if (hddsSenha.Value != "")
                {
                    txtsSenha.Text = hddsSenha.Value;
                }

                if (hddsSenhaConfirmacao.Value != "")
                {
                    txtsSenha_Confirmacao.Text = hddsSenhaConfirmacao.Value;
                }
                var requestTarget = this.Request["__EVENTTARGET"];
                if (requestTarget == "funcao_SALVAR")
                {
                    GravarUsuario();
                }
            }


        }

        protected string RetornarScripts()
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();


            //Mensagens de Confirmação
            sb.Append("$(function() {");


            sb.Append("$(\"#dialog-Salvar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_SALVAR\", \"\");");
            sb.Append("$(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$('#ContentPlaceHolder1_cmdSalvar').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("});");

            //sb.Append("alert('A');");


            return sb.ToString();


        }

        void RegistraScript(string sScript)
        {
            sScript = RetornarScripts() + sScript;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "AddShowModalScript", sScript, true);

        }
        void PesquisarUsuario(string idUsuario)
        {
            string sErro = "";

            try
            {
                LimparCampos();

                hddIdUsuario.Value = "0";
                cmdSalvar.Text = "Salvar";

                if (idUsuario != "0")
                {

                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@idUsuario", idUsuario);
                    vParametros.Add("@sFuncao", "CONSULTAR");
                    dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios", vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        txtsLogin.Text = RETORNO.DATASET(dsPesquisa, 0, "sLogin");
                        txtsDsUsuario.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario");
                        txtsSenha.Text = RETORNO.DATASET(dsPesquisa, 0, "sSenha");
                        txtsSenha_Confirmacao.Text = RETORNO.DATASET(dsPesquisa, 0, "sSenha");
                        txtsEmail.Text = RETORNO.DATASET(dsPesquisa, 0, "sEmail");

                        txtsSenha_Confirmacao.Attributes["value"] = RETORNO.DATASET(dsPesquisa, 0, "sSenha");
                        txtsSenha.Attributes["value"] = RETORNO.DATASET(dsPesquisa, 0, "sSenha");

                        hddIdUsuario.Value = idUsuario;

                        txtsLogin.ReadOnly = true;
                        cmdSalvar.Text = "Salvar";
                        lblTituloSalvar.Text = "Confirma a alteração nos dados de " + RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario") + "?";



                    }
                    else
                    {
                        throw new Exception("Erro ao consultar BD: " + sErro);
                    }
                }

                else
                {
                    cmdSalvar.Text = "Incluir";
                    txtsLogin.Focus();
                    hddIdUsuario.Value = "0";
                }

            }
            catch
            {
                //MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }



        void GravarUsuario()
        {


            string sErro = "";
            if (AplicarValidacoes())
            {

                try
                {
                    string[] vidUsuario = hddIdUsuario.Value.Split(',');
                    string idUsuarioUtilizar = vidUsuario[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR_ALTERAR_DADOS");
                    vParametros.Add("@idUsuario", idUsuarioUtilizar);
                    vParametros.Add("@sLogin", txtsLogin.Text);
                    vParametros.Add("@sDsUsuario", txtsDsUsuario.Text);
                    vParametros.Add("@sSenha", txtsSenha.Text);
                    vParametros.Add("@sEmail", txtsEmail.Text);
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios", vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idUsuarioUtilizar = RETORNO.DATASET(dsSalvar, 0, "idUsuario");
                        PesquisarUsuario(idUsuarioUtilizar);
                        MensagemPagina_AlterarDados.MostraMensagem_Sucesso("Dados alterados com sucesso!");
                        HttpContext.Current.Session["sUsuarioLogado"] = txtsDsUsuario.Text;

                    }
                    else
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina_AlterarDados.MostraMensagem_Erro(ex.Message);
                }


            }
        }

        void LimparCampos()
        {
            txtsLogin.Text = "";
            txtsDsUsuario.Text = "";
            txtsSenha.Text = "";
            txtsSenha_Confirmacao.Text = "";
            txtsEmail.Text = "";

        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {

        }

        bool AplicarValidacoes()
        {
            bool retorno = true;

            if (txtsLogin.Text.Length < 3)
            {
                //MensagemPagina.MostraMensagem_Erro("Informe um nome válido para Login!");
                txtsLogin.Focus();
                return false;
            }

            if (txtsDsUsuario.Text.Length < 3)
            {
                MensagemPagina_AlterarDados.MostraMensagem_Erro("Informe o nome do Usuário!");
                txtsDsUsuario.Focus();
                return false;
            }

            if (txtsSenha.Text.Length == 0)
            {
                MensagemPagina_AlterarDados.MostraMensagem_Erro("Informe uma senha valida!");
                txtsSenha.Focus();
                return false;
            }
            if (txtsSenha.Text != txtsSenha_Confirmacao.Text)
            {
                MensagemPagina_AlterarDados.MostraMensagem_Erro("Senha e confirmação de senha são diferentes!");
                txtsSenha.Focus();
                return false;
            }


            return retorno;
        }


        protected void txtsSenha_TextChanged(object sender, EventArgs e)
        {
            hddsSenha.Value = txtsSenha.Text;
        }

        protected void txtsSenha_Confirmacao_TextChanged(object sender, EventArgs e)
        {
            hddsSenhaConfirmacao.Value = txtsSenha_Confirmacao.Text;
        }

        protected void cmdCancelar_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina("app/Sistemas.aspx");
        }
    }



}