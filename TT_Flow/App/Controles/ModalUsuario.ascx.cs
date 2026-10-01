using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Controles
{
    public partial class ModalUsuario : UserControl
    {
        /// <summary>
        /// Define o campo (por classe CSS ou ID) que será utilizado para abrir o ModalUsuario pelo Click.
        /// </summary>
        public string sModalUsuario_Click { get; set; } = ".modalUsuario_Click";

        private void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MensagemPagina_Script.MostraMensagem_Sucesso("sucesso", false);
                MensagemPagina_Script.MostraMensagem_Erro("erro", false);
                Pesquisar();
            }
            RegistraScript();
        }

        private void Pesquisar()
        {
            try
            {
                div_UsuarioLogado.Visible = false;
                div_Ponto.Visible = false;
                cmdSalvar.Visible = false;

                string sid = "0";
                bool bUsuario = true;
                int.TryParse(IDENTITY.Variaveis.idUsuario(), out int idUsuario);
                int.TryParse(IDENTITY.Variaveis.idColaborador(), out int idColaborador);

                if (idUsuario > 0) sid = idUsuario.ToString();
                else if (idColaborador > 0)
                {
                    sid = idColaborador.ToString();
                    bUsuario = false;
                }
                else { MensagemPagina.MostraMensagem_Erro("Houve um um erro com o Usuário Logado, por favor faça Login novamente!", false); return; }

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_EXIBICAO" },
                    { bUsuario ? "@idUsuario" : "@idColaborador", sid }
                };
                DataSet dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores", vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                {
                    lblUsuario.InnerText = RETORNO.DATASET(dsPesquisa, "sDsc");

                    txtEmpresa.Text = RETORNO.DATASET(dsPesquisa, "sDscEmpresa");
                    txtDepartamento.Text = RETORNO.DATASET(dsPesquisa, "sDscDepartamento");
                    txtsEmail.Text = RETORNO.DATASET(dsPesquisa, "sEmail");
                    txtsTelCelular.Text = RETORNO.DATASET(dsPesquisa, "sTelCelular");
                    txtsRamal.Text = RETORNO.DATASET(dsPesquisa, "sRamal");

                    DataTable dt = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_IMAGEM" }, { "@idTipoArquivo", "40" }, { "@idObjeto", RETORNO.DATASET(dsPesquisa, "idColaborador") } });
                    if (dt.Rows.Count > 0) imgColaborador.ImageUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])dt.Rows[0]["vbArquivo"]);

                    if (bUsuario && sid.Equals(IDENTITY.Variaveis.idUsuario()))
                    {
                        div_UsuarioLogado.Visible = true;
                        div_Ponto.Visible = true;
                        cmdSalvar.Visible = true;

                        FUNCOES.Popula_Combo(ddlDashboard, $"sp_Select 'Flow_DashBoards', {sid}, @idFiltro=2", "idRecurso", "sDscRecurso", false, "Sem preferência", "0");
                        ddlDashboard.SelectedValue = RETORNO.DATASET(dsPesquisa, "idPaginaInicial_Preferencia");

                        ddlDashboard.Attributes.Add("disabled", "disabled");
                        if (FUNCOES.ValidaPermissao(Permissao.Usuarios.Editar_PaginaInicial)) ddlDashboard.Attributes.Remove("disabled");

                        txtLogin.Text = RETORNO.DATASET(dsPesquisa, "sLogin");
                        txtSenha.Attributes["value"] = RETORNO.DATASET(dsPesquisa, "sSenha");
                        txtConfirmaSenha.Attributes["value"] = RETORNO.DATASET(dsPesquisa, "sSenha");

                        string idPonto = RETORNO.DATASET(dsPesquisa, "idTipoPonto_Atual");
                        rbEmExpediente.Checked = false;
                        rbEmDescanso.Checked = false;
                        rbEmAlmoco.Checked = false;
                        rbForaExpediente.Checked = false;

                        switch (idPonto)
                        {
                            case "0":
                                rbForaExpediente.Checked = true;
                                break;

                            case "1":
                                rbEmExpediente.Checked = true;
                                break;

                            case "2":
                                rbEmDescanso.Checked = true;
                                break;

                            case "3":
                                rbEmAlmoco.Checked = true;
                                break;

                            case "4":
                                rbForaExpediente.Checked = true;
                                break;
                        }
                    }
                }
                else throw new Exception(sErro);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        private void RegistraScript()
        {
            FUNCOES.Scripts.Aplica_TooltipPersonalizado(Page);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("");
            sb.AppendLine($"    $('{sModalUsuario_Click}').addClass('cursor-pointer');");
            sb.AppendLine("     $('div[id*=MensagemPagina_Script] .alert').addClass('invisivel');");
            sb.AppendLine("     var MensagemPagina_Sucesso = $('div[id*=MensagemPagina_Script] .alert-success');");
            sb.AppendLine("     var MensagemPagina_Erro = $('div[id*=MensagemPagina_Script] .alert-danger');");
            sb.AppendLine("");
            sb.AppendLine($"    $(document).off('click', '{sModalUsuario_Click}').on('click', '{sModalUsuario_Click}', function (e) {{");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         $('#modalUsuario').modal('show');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $(document).off('click', '[id*=cmdExibe]').on('click', '[id*=cmdExibe]', function (e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         const $this = $(this);");
            sb.AppendLine("         $this.addClass('invisivel');");
            sb.AppendLine("         $this.siblings('input').attr('type', 'text');");
            sb.AppendLine("         $this.siblings('a').removeClass('invisivel');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $(document).off('click', '[id*=cmdEsconde]').on('click', '[id*=cmdEsconde]', function (e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         const $this = $(this);");
            sb.AppendLine("         $this.addClass('invisivel');");
            sb.AppendLine("         $this.siblings('input').attr('type', 'password');");
            sb.AppendLine("         $this.siblings('a').removeClass('invisivel');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine($"    $(document).off('click', '#{cmdSalvar.ClientID}').on('click', '#{cmdSalvar.ClientID}', function (e) {{");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine($"        const idDashboard = $('#{ddlDashboard.ClientID }').val();");
            sb.AppendLine($"        const idPonto = $('#{rbEmExpediente.ClientID }').prop('checked') ? 1 : $('#{rbEmDescanso.ClientID}').prop('checked') ? 2 : $('#{rbEmAlmoco.ClientID}').prop('checked') ? 3 : $('#{rbForaExpediente.ClientID}').prop('checked') ? 4 : 0;");
            sb.AppendLine($"        const sSenha = $('#{txtSenha.ClientID}').val();");
            sb.AppendLine($"        const sConfirma = $('#{txtConfirmaSenha.ClientID}').val();");
            sb.AppendLine("         $.ajax({");
            sb.AppendLine("             type: 'POST',");
            sb.AppendLine("             dataType: 'json',");
            sb.AppendLine("             contentType: 'application/json; charset=utf-8',");
            sb.AppendLine("             url: '/API/Pagina_Ajax.aspx/Salvar_ModalUsuario',");
            sb.AppendLine("             data: JSON.stringify({ idDashboard: idDashboard, idPonto: idPonto.toString(), sSenha: sSenha, sConfirma_Senha: sConfirma }),");
            sb.AppendLine("             success: function (response) {");
            sb.AppendLine("                 const nRet = response.d.nRet;");
            sb.AppendLine("                 const msg = response.d.msg;");
            sb.AppendLine("                 if (nRet == 0) {");
            sb.AppendLine("                     MensagemPagina_Sucesso.html(`<button type='button' class='close' onclick=\"$(this).closest('div.alert').addClass('invisivel');\">×</button>${msg}`);");
            sb.AppendLine("                     MensagemPagina_Sucesso.removeClass('invisivel');");
            sb.AppendLine("                 } else {");
            sb.AppendLine("                     MensagemPagina_Erro.html(`<button type='button' class='close' onclick=\"$(this).closest('div.alert').addClass('invisivel');\">×</button>${msg}`);");
            sb.AppendLine("                     MensagemPagina_Erro.removeClass('invisivel');");
            sb.AppendLine("                 }");
            sb.AppendLine("             }");
            sb.AppendLine("         });");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_RegistraScript_ModalUsuario", sb.ToString(), true);
        }
    }
}