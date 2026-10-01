using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class Colaboradores_Exibicao : Page
    {
        private static string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores";
        private static string sProcedure_Usuarios = "sp_Manipula_tbl_Usuarios";

        #region | Page_Load + Pesquisar

        private void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request["id"]) && Request["id"] != "0")
                    Pesquisar(Request["id"], false);
                else if (!string.IsNullOrEmpty(Request["idu"]) && Request["idu"] != "0")
                    Pesquisar(Request["idu"], true);
                else
                    Pesquisar(IDENTITY.Variaveis.idUsuario(), true);
            }

            RegistraScript();
        }

        private void Pesquisar(string id, bool bUsuario)
        {
            try
            {
                div_UsuarioLogado.Visible = false;
                cmdSalvar.Visible = false;

                if (id != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_EXIBICAO" },
                        { bUsuario ? "@idUsuario" : "@idColaborador", id }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        lblTituloPagina.Text = RETORNO.DATASET(dsPesquisa, "sDsc");

                        txtEmpresa.Text = RETORNO.DATASET(dsPesquisa, "sDscEmpresa");
                        txtDepartamento.Text = RETORNO.DATASET(dsPesquisa, "sDscDepartamento");
                        txtsEmail.Text = RETORNO.DATASET(dsPesquisa, "sEmail");
                        txtsTelCelular.Text = RETORNO.DATASET(dsPesquisa, "sTelCelular");
                        txtsRamal.Text = RETORNO.DATASET(dsPesquisa, "sRamal");

                        string idColaborador = RETORNO.DATASET(dsPesquisa, "idColaborador");
                        carregaimgColaborador(idColaborador);

                        if (bUsuario && id.Equals(IDENTITY.Variaveis.idUsuario()))
                        {
                            div_UsuarioLogado.Visible = true;
                            cmdSalvar.Visible = true;
                            ddlDashboard.Attributes.Add("disabled", "disabled");

                            FUNCOES.Popula_Combo(ddlDashboard, $"sp_Select 'Flow_DashBoards', {id}, @idFiltro=2", "idRecurso", "sDscRecurso", false, "Sem preferência", "0");
                            ddlDashboard.SelectedValue = RETORNO.DATASET(dsPesquisa, "idPaginaInicial_Preferencia");

                            if (FUNCOES.ValidaPermissao(Permissao.Usuarios.Editar_PaginaInicial)) 
                                ddlDashboard.Attributes.Remove("disabled");

                            txtLogin.Text = RETORNO.DATASET(dsPesquisa, "sLogin");
                            txtSenha.Attributes["value"] = RETORNO.DATASET(dsPesquisa, "sSenha");
                            txtConfirmaSenha.Attributes["value"] = RETORNO.DATASET(dsPesquisa, "sSenha");
                        }
                    }
                    else
                        throw new Exception(sErro);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        #endregion

        #region | Utils

        private void carregaimgColaborador(string idObjeto)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_IMAGEM" },
                { "@idTipoArquivo", "40" },
                { "@idObjeto", idObjeto }
            };
            DataTable dt = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (dt.Rows.Count > 0) imgColaborador.ImageUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])dt.Rows[0]["vbArquivo"]);
        }

        #endregion

        #region | Script

        private void RegistraScript()
        {
            FUNCOES.Scripts.Aplica_TooltipPersonalizado(Page);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$(document).ready(function () {");

            sb.AppendLine("     $(document).off('click', '[id*=cmdExibe]').on('click', '[id*=cmdExibe]', function (e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         const $this = $(this);");
            sb.AppendLine("         $this.addClass('invisivel');");
            sb.AppendLine("         $this.siblings('input').attr('type', 'text');");
            sb.AppendLine("         $this.siblings('a').removeClass('invisivel');");
            sb.AppendLine("     });");

            sb.AppendLine("     $(document).off('click', '[id*=cmdEsconde]').on('click', '[id*=cmdEsconde]', function (e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         const $this = $(this);");
            sb.AppendLine("         $this.addClass('invisivel');");
            sb.AppendLine("         $this.siblings('input').attr('type', 'password');");
            sb.AppendLine("         $this.siblings('a').removeClass('invisivel');");
            sb.AppendLine("     });");

            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RegistraScript", sb.ToString(), true);
        }

        #endregion

        #region | Eventos

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                txtSenha.Attributes["value"] = txtSenha.Text;
                txtConfirmaSenha.Attributes["value"] = txtSenha.Text;

                if (txtSenha.Text.Length < 3)
                {
                    MensagemPagina.MostraMensagem_Erro("A Senha deve possuir ao menos 3 caracteres!", false);
                    return;
                }

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "SALVAR_EXIBICAO" },
                    { "@idDashboard", ddlDashboard.SelectedValue },
                    { "@sSenha", txtSenha.Text },
                    { "@idUsuario", IDENTITY.Variaveis.idUsuario() }
                };
                BD.ExecutarDataSet(sProcedure_Usuarios, vParametros);

                MensagemPagina.MostraMensagem_Sucesso($"Informações salvas com sucesso!", true);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Houve um Erro ao Salvar as informações do Usuário!<br />Erro: " + ex.Message, false);
            }
        }

        #endregion
    }
}