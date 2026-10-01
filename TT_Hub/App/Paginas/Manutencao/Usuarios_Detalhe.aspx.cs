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

namespace TT_Hub.App.Paginas.Manutencao
{
    public partial class Usuarios_Detalhe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            string idCliente;
            FUNCOES.ValidarPermissaoAcesso();

            RegistraScript("");
            if (!IsPostBack)
            {

                idCliente = IDENTITY.Variaveis.idCliente();
                if (Request["idu"] != null)
                {
                    PesquisarUsuario(Request["idu"].ToString());
                }
                else
                {
                    PesquisarUsuario("0");
                }

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

                if (hddidCliente.Value != "0")
                {
                    ddlCliente.SelectedValue = hddidCliente.Value;
                }

                var requestTarget = this.Request["__EVENTTARGET"];
                var requestArgs = this.Request["__EVENTARGUMENT"];

                if (requestTarget == "funcao_SAIR")
                {
                    FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                }
                else if (requestTarget == "funcao_SALVAR")
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
            sb.Append("$v192('#ContentPlaceHolder1_cmdSalvar').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
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

        
        void LimparCampos()
        {

            DIV_Cliente.Visible = false;
            PainelAtualizacao.Visible = false;
            //txtsDescricao.Text = "";
            //txtsDscCliente.Text = "";
            //ddlCliente.SelectedValue = "0";
            //ddlTipoEquipamento.SelectedValue = "0";
            //txtsID.Text = "";
            //txtsw_Account.Text = "";
            //ddlOperadora.SelectedValue = "0";
            //txtsIMEI.Text = "";
            //txtsCEP.Text = "";
            //txtsLogradouro.Text = "";
            //txtsNumero.Text = "";
            //txtsComplemento.Text = "";
            //txtsCidade.Text = "";
            //ddlsUF.SelectedValue = "";


            //txtsDscCliente.Visible = false;
            //ddlCliente.Visible = false;
        }

        void PesquisarUsuario(string idUsuario)
        {
            string sErro = "";

            try
            {
                LimparCampos();

                hddIdUsuario.Value = "0";
                hddidCliente.Value = "0";
                cmdSalvar.Text = "Salvar";

                if (idUsuario != "0")
                {

                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@idUsuario", idUsuario);
                    vParametros.Add("@sFuncao", "CONSULTAR");
                    vParametros.Add("@idCliente", IDENTITY.Variaveis.idCliente());
                    dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios", vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        txtsLogin.Text                  = RETORNO.DATASET(dsPesquisa, 0, "sLogin");
                        txtsDsUsuario.Text              = RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario");
                        txtsSenha.Text                  = RETORNO.DATASET(dsPesquisa, 0, "sSenha");
                        txtsSenha_Confirmacao.Text      = RETORNO.DATASET(dsPesquisa, 0, "sSenha");

                        txtsSenha_Confirmacao.Attributes["value"] = RETORNO.DATASET(dsPesquisa, 0, "sSenha");
                        txtsSenha.Attributes["value"] = RETORNO.DATASET(dsPesquisa, 0, "sSenha");

                        ddlTpUsuario.SelectedValue      = RETORNO.DATASET(dsPesquisa, 0, "sTipo");
                        hddidCliente.Value              = RETORNO.DATASET(dsPesquisa, 0, "idCliente");
                        AjustarCliente(RETORNO.DATASET(dsPesquisa, 0, "sTipo"), RETORNO.DATASET(dsPesquisa, 0, "idCliente"));
                        txtsEmail.Text                  = RETORNO.DATASET(dsPesquisa, 0, "sEmail");
                        ddlAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        PainelAtualizacao.Visible = true;

                        lblTituloPagina.Text = string.Format("Usuário {0} - {1}", RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario"), RETORNO.DATASET(dsPesquisa, 0, "sLogin"));
                        BreadCrumb.TitulodaPagina = txtsLogin.Text;
                        hddIdUsuario.Value = idUsuario;

                        cmdSalvar.Text = "Salvar";
                        lblTituloSalvar.Text = "Confirma a alteração nos dados do Usuário " + RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario") + "?";


                    }
                    else
                    {
                        throw new Exception("Erro ao consultar BD: " + sErro);
                    }
                }
            
                else
                {
                    BreadCrumb.TitulodaPagina = "Novo Usuário";
                    lblTituloPagina.Text = "Novo Usuário";
                    cmdSalvar.Text = "Incluir";
                    lblTituloSalvar.Text = "Confirma a INCLUSÃO do Usuário?";
                    txtsLogin.Focus();
                    hddIdUsuario.Value = "0";

                    if (IDENTITY.Variaveis.idCliente() != "0")
                    {
                        hddidCliente.Value = IDENTITY.Variaveis.idCliente();
                        ddlTpUsuario.SelectedValue = "C";
                        AjustarCliente("C", IDENTITY.Variaveis.idCliente());
                    }
                }

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao consultar BD: " + ex.Message);
            }
        }

        void AjustarCliente(string sTipo, string idCliente)
        {
            DIV_Cliente.Visible = false;

            if (sTipo == "C")
            {
                if (idCliente != "0")
                {
                    ddlTpUsuario.SelectedValue = "C";
                    if (IDENTITY.Variaveis.idCliente() != "0")
                    {
                        ddlTpUsuario.Attributes.Add("disabled", "disabled");
                        ddlCliente.Attributes.Add("disabled", "disabled");

                    }
                }

                FUNCOES.Popula_Combo(ddlCliente, "sp_Select 'CLIENTE', " + IDENTITY.Variaveis.idCliente(), "idCliente", "sDscCliente", false, "Selecione o Ciente", "0");
                ddlCliente.SelectedValue = idCliente;
                DIV_Cliente.Visible = true;
            }
        }


        void GravarUsuario()
        {

            String sFuncao = "SALVAR";
            string sErro = "";
            string idCliente = "0";
            if (AplicarValidacoes())
            {

                try
                {
                    string[] vidUsuario = hddIdUsuario.Value.Split(',');
                    string idUsuarioUtilizar = vidUsuario[0].ToString();


                    if (ddlTpUsuario.SelectedValue == "C")
                    {
                        idCliente = ddlCliente.SelectedValue;
                    }
 
                    DataSet dsGravar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", sFuncao);
                    vParametros.Add("@idUsuario", idUsuarioUtilizar);
                    vParametros.Add("@idCliente", idCliente);
                    vParametros.Add("@sLogin", txtsLogin.Text);
                    vParametros.Add("@sDsUsuario", txtsDsUsuario.Text);
                    vParametros.Add("@sSenha", txtsSenha.Text);
                    vParametros.Add("@sEmail", txtsEmail.Text);
                    vParametros.Add("@sTipo", ddlTpUsuario.SelectedValue);
                    vParametros.Add("@sSituacao", ddlAtivo.Situacao_Recuperar());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsGravar = BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios", vParametros);

                    if (BD.ValidarDataSet(dsGravar, out sErro))
                    {
                        idUsuarioUtilizar = RETORNO.DATASET(dsGravar, 0, "idUsuario");
                        PesquisarUsuario(idUsuarioUtilizar);
                        MensagemPagina.MostraMensagem_Sucesso("Usuário gravado com sucesso!");
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

                RegistraScript("");
            }
        }


        protected void cmdSalvar_Click(object sender, EventArgs e)
        {

        }

        bool AplicarValidacoes()
        {
            bool retorno = true;

            if (txtsLogin.Text.Length < 3)
            {
                MensagemPagina.MostraMensagem_Erro("Informe um nome válido para Login!");
                txtsLogin.Focus();
                return false;
            }

            if (txtsDsUsuario.Text.Length < 3)
            {
                MensagemPagina.MostraMensagem_Erro("Informe o nome do Usuário!");
                txtsDsUsuario.Focus();
                return false;
            }

            if (txtsSenha.Text.Length == 0)
            {
                MensagemPagina.MostraMensagem_Erro("Informe uma senha!");
                txtsSenha.Focus();
                return false;
            }
            if (txtsSenha.Text != txtsSenha_Confirmacao.Text)
            {
                MensagemPagina.MostraMensagem_Erro("Senha e confirmação de senha são diferentes!");
                txtsSenha.Focus();
                return false;
            }

            if (ddlTpUsuario.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione o Tipo de Usuário!");
                return false;
            }

            if (ddlCliente.Visible == true)
            {
                if (ddlCliente.SelectedValue == "0")
                {
                    MensagemPagina.MostraMensagem_Erro("Selecione um Cliente!");
                    return false;
                }
            }
            return retorno;
        }

        protected void ddlTpUsuario_SelectedIndexChanged(object sender, EventArgs e)
        {
            AjustarCliente(ddlTpUsuario.SelectedValue, hddidCliente.Value);
        }

        protected void txtsSenha_TextChanged(object sender, EventArgs e)
        {
            hddsSenha.Value = txtsSenha.Text;
        }

        protected void txtsSenha_Confirmacao_TextChanged(object sender, EventArgs e)
        {
            hddsSenhaConfirmacao.Value = txtsSenha_Confirmacao.Text;
        }

        protected void ddlCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            hddidCliente.Value = ddlCliente.SelectedValue;
        }
    }



}