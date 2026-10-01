using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;

namespace TT_Colaborador.Aplicativo.Paginas.Mensagem
{
    public partial class Mensagens_Detalhe : Page
    {
        string sProcedure = "sp_Manipula_tbl_Flow_Mensagens";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidarPermissaoAcesso();

            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlidDepartamento, "sp_Select 'Flow_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Todos os Departamentos", "0");

                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Mensagens.Consultar, true, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Mensagens.Incluir, true, true);
                    Pesquisar("0");
                }
            }

            RegistraScript();
        }

        protected void Pesquisar(string idMensagem)
        {
            try
            {
                LimpaCampos();

                hddidMensagens.Value = "0";
                cmdSalvar.Text = "Enviar";
                cmdSalvar.Visible = false;
                txtsAssunto.ReadOnly = true;
                txtsCorpo.ReadOnly = true;

                if (idMensagem != "0")
                {
                    lblTituloPagina.Text = "Mensagem / Aviso";

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idMensagem", idMensagem },
                        { "@idUsuarioPesquisa", IDENTITY.Variaveis.idUsuario() }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        hddidMensagens.Value = RETORNO.DATASET(dsPesquisa, 0, "idMensagem");
                        ddlidDepartamento.Text = RETORNO.DATASET(dsPesquisa, 0, "idDepartamentoDestino");

                        txtidUsuarioRemetente.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioRemetente");
                        txtsAssunto.Text = RETORNO.DATASET(dsPesquisa, 0, "sAssunto");

                        if (!string.IsNullOrEmpty(txtsAssunto.Text))
                            lblTituloPagina.Text += " - " + txtsAssunto.Text;

                        txtsCorpo.Text = HttpUtility.HtmlDecode(RETORNO.DATASET(dsPesquisa, 0, "sCorpo"));
                        ddlIsAviso.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sIsAviso").ToUpper().Trim().Equals("T") ? "N" : RETORNO.DATASET(dsPesquisa, 0, "sIsAviso");

                        string sDscUsuarioDestino = RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioDestino");

                        if (sDscUsuarioDestino != "")
                        {
                            DIV_Destinatario_GRUPO.Visible = false;
                            DIV_Destinatario_USUARIO.Visible = true;
                            txtsDscUsuarioDestino.Text = sDscUsuarioDestino;
                        }

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtInclusao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioRemetente"));

                        hddidMensagens.Value = idMensagem;
                        txtidUsuarioRemetente.ReadOnly = true;

                        ddlIsAviso.Attributes.Remove("disabled");
                        ddlIsAviso.Attributes.Add("disabled", "disabled");

                        ddlidDepartamento.Attributes.Remove("disabled");
                        ddlidDepartamento.Attributes.Add("disabled", "disabled");

                        ddlUsuarios.Enabled = false;
                    }
                    else
                        throw new Exception(sErro);
                }
                else
                {
                    lblTituloPagina.Text = "Nova Mensagem";

                    cmdSalvar.Visible = true;
                    txtidUsuarioRemetente.Text = IDENTITY.Variaveis.sUsuarioLogado();
                    txtsAssunto.ReadOnly = false;
                    txtsCorpo.ReadOnly = false;
                }

                ddlidDepartamento.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        void LimpaCampos()
        {
            ddlidDepartamento.SelectedValue = "0";
            ddlIsAviso.SelectedValue = "N";
            txtidUsuarioRemetente.Text = "";
            txtsAssunto.Text = "";
            txtsCorpo.Text = "";
            cmdSalvar.Text = "Salvar";
            DIV_Destinatario_GRUPO.Visible = true;
            DIV_Destinatario_USUARIO.Visible = false;
            DIV_USUARIOS.Visible = false;
            PainelAtualizacao.Visible = false;
        }

        private bool ValidarDados()
        {
            if (txtsAssunto.Text.Length < 5)
            {
                MensagemPagina.MostraMensagem_Erro("Assunto deve possuir ao menos 5 caracteres!");
                return false;
            }
            if (txtsCorpo.Text.Length < 10)
            {
                MensagemPagina.MostraMensagem_Erro("Mensagem deve posuir ao menos 10 caracteres!");
                return false;
            }

            return true;
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            if (ValidarDados())
            {
                try
                {
                    string[] vidMensagem = hddidMensagens.Value.Split(',');
                    string idMensagem = vidMensagem[0].ToString();

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idUsuarioRemetente", IDENTITY.Variaveis.idUsuario() },
                        { "@idTipoObjeto", "0" },
                        { "@sCorpo", txtsCorpo.Text.Replace(Environment.NewLine, "<br/>") },
                        { "@sAssunto", txtsAssunto.Text },
                        { "@sIsAviso", ddlIsAviso.SelectedValue }
                    };

                    if (ddlUsuarios.SelectedValue != "")
                        vParametros.Add("@idUsuarioDestino", ddlUsuarios.SelectedValue);
                    else
                        vParametros.Add("@idDepartamentoDestino", ddlidDepartamento.SelectedValue);

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out string sErro))
                    {
                        MensagemPagina.MostraMensagem_Sucesso("Mensagem gravada com sucesso!");
                        Pesquisar(dsSalvar.Tables[0].Rows[0]["idMensagem"].ToString());
                        RegistraScript();
                    }
                    else
                        throw new Exception("BD: " + sErro.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Houve um erro na tentativa de Salvar a Mensagem!<br />Erro ao Salvar: " + ex.Message);
                }
            }
        }

        protected void ddlidDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            DIV_USUARIOS.Visible = false;

            if (ddlidDepartamento.SelectedValue != "0")
            {
                FUNCOES.Popula_Combo(ddlUsuarios, "sp_Select 'Usuarios_x_Departamentos'," + ddlidDepartamento.SelectedValue, "idUsuario", "sDscUsuario", false, "Todos do Departamento", "0");

                ddlUsuarios.Visible = ddlidDepartamento.SelectedValue != "0";
                DIV_USUARIOS.Visible = true;

                FUNCOES.Scripts.FocusScript(Page, ddlUsuarios.ClientID);
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("É necessário selecionar um Departamento!", false);

                FUNCOES.Scripts.FocusScript(Page, ddlidDepartamento.ClientID);
            }
        }

        #region | Script 

        void RegistraScript()
        {
            string disabled = txtsCorpo.ReadOnly ? "txtCorpo.summernote('disable');" : "";

            string script = @"
            <script>
                $(document).ready(function () {
                    var summernoteOptions = {
                        placeholder: 'Digite sua Mensagem',
                        tabsize: 2,
                        height: 350
                    };

                    var txtCorpo = $('#" + txtsCorpo.ClientID + @"');
                    txtCorpo.summernote(summernoteOptions);" + 
                    disabled + @"
                });
            </script>";

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptSummernote", script, false);
        }

        #endregion
    }
}