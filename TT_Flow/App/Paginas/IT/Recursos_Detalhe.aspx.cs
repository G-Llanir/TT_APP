using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using TT.FrameWork;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using System.Web.Services;

namespace TT_Flow.App.Paginas.IT
{
    public partial class Recursos_Detalhe : Page
    {
        string sProcedure = "sp_Manipula_tbl_Recursos";
        string sProcedure_Valida_Recursos = "sp_Flow_Valida_Recursos_x_Usuarios";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["id"] == "0")
                {
                    div_PreviaIcone.Visible = false;
                    div_idRecursoPai.Visible = false;

                    FUNCOES.ValidaPermissao(Permissao.TI.Controle_de_Permissoes.Incluir, true);

                    txtidRecurso.Text = "Novo";
                    PainelAtualizacao.Visible = false;
                }

                Pesquisar(Request["id"]);

                if (Request["id"] != "0")
                {
                    if (!FUNCOES.ValidaPermissao(Permissao.TI.Controle_de_Permissoes.Alterar))
                    {
                        ddlidSistema.Attributes.Add("disabled", "disabled");
                        txtsDscRecurso.ReadOnly = true;
                        ddlidRecursoPai.Attributes.Add("disabled", "disabled");
                        SwitchMenu.BloquearEdicao(true);
                        txtsURL.ReadOnly = true;
                        txtsIcone.ReadOnly = true;
                        txtnOrdem.ReadOnly = true;
                        txtnOrdem_Menu.ReadOnly = true;
                        SwitchAtivo.BloquearEdicao(true);
                        cmdSalvar.Visible = false;
                        field_cancel.Value = "Voltar";
                    }
                }

                if (Request["msg"] == "1")
                    MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!");
                if (Request["msg"] == "2")
                    MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!<br /><a href='/App/Paginas/IT/Recursos_Detalhe.aspx?id=0' target='_blank'>Criar Nova Permissão...</a>");
            }

            RegistraScript();
        }

        protected void Pesquisar(string idRecurso)
        {
            try
            {
                hddIcones.Value += BD.ExecutarDataTable(sProcedure, new Dictionary<string, string> { { "@sFuncao", "CONSULTA_ICONES" } }).Rows[0][0].ToString();

                div_PosicaoMenu.Visible = false;
                icon_nOrdem.Visible = false;
                icon_nOrdem_Menu.Visible = false;

                LimpaCampos();

                icon_nOrdem.Attributes["title"] = "Este campo se refere à Ordem em que esta Permissão aparece dentro de seu Permissão Pai!";

                lblTituloPagina.Text = "Nova Permissão";

                if (idRecurso != "0")
                {
                    div_sIcone.Visible = true;
                    div_sURL.Visible = true;
                    div_PreviaIcone.Visible = false;
                    div_idRecursoPai.Visible = true;

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTA_RECURSO_DETALHE" },
                        { "@idRecursoDetalhe", idRecurso }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        txtidRecurso.Text = RETORNO.DATASET(dsPesquisa, "idRecurso");
                        ddlidSistema.SelectedValue = RETORNO.DATASET(dsPesquisa, "idSistema");
                        PopulaCombos(ddlidSistema.SelectedValue);
                        ddlidRecursoPai.SelectedValue = RETORNO.DATASET(dsPesquisa, "idRecursoPai");
                        txtsDscRecurso.Text = RETORNO.DATASET(dsPesquisa, "sDscRecurso");
                        txtsURL.Text = RETORNO.DATASET(dsPesquisa, "sURL");
                        txtnOrdem.Text = RETORNO.DATASET(dsPesquisa, "nOrdem");
                        SwitchMenu.Definir(RETORNO.DATASET(dsPesquisa, "sMenu"), "É Menu?", "N");
                        SwitchPaginaInicial.Definir(RETORNO.DATASET(dsPesquisa, "sPaginaInicial"), "Página Inicial?", "N");
                        SwitchAtivo.Definir(RETORNO.DATASET(dsPesquisa, "sAtivo"), "Ativo?", "N");
                        txtsIcone.Text = RETORNO.DATASET(dsPesquisa, "sIcone");
                        txtnOrdem_Menu.Text = RETORNO.DATASET(dsPesquisa, "nOrdem_Menu");

                        lblTituloPagina.Text = txtsDscRecurso.Text;

                        if (int.TryParse(RETORNO.DATASET(dsPesquisa, "idRecursoPai"), out int idRecursoPai) && idRecursoPai > 5 && idRecursoPai < 9000)
                        {
                            icon_nOrdem.Visible = true;
                            icon_nOrdem_Menu.Visible = true;

                            icon_nOrdem_Menu.Attributes["title"] = RETORNO.DATASET(dsPesquisa, "sIcone_nOrdem_Menu");
                        }
                        else if (idRecursoPai == 2)
                        {
                            icon_nOrdem.Visible = true;
                            icon_nOrdem_Menu.Visible = true;

                            icon_nOrdem.Attributes["title"] = "Este campo se refere à Ordem em que esta Permissão aparece dentro de sua Permissão Pai!<br>Neste caso a Ordem em que ele aparece no Menu!";
                            icon_nOrdem_Menu.Attributes["title"] = "Este campo se refere ao Andar de Permissões no Menu, ao qual esta Permissão pertence!<br>Neste caso ele define qual o Andar desta Permissão e também de toda sua cadeia de Permissões Filhos!";
                        }

                        if (txtsIcone.Text != "N")
                        {
                            div_PreviaIcone.Visible = true;
                            ltrPreviaIcone.Text = string.Format("<i class=\"{0}\"></i>", txtsIcone.Text);
                        }

                        if (txtsURL.Text != "N" || SwitchMenu.Recuperar() == "S")
                            div_PosicaoMenu.Visible = true;

                        if (ddlidSistema.SelectedValue == "8")
                        {
                            div_PosicaoMenu.Visible = false;
                            div_txtnOrdem_Menu.Visible = false;
                            div_sMenu.Visible = false;

                            txtnOrdem_Menu.Text = "0";
                            SwitchMenu.Definir("N", "É Menu?", "N");
                        }
                        else
                        {
                            if (txtsURL.Text != "N" || SwitchMenu.Recuperar() == "S")
                                div_PosicaoMenu.Visible = true;

                            div_txtnOrdem_Menu.Visible = true;
                            div_sMenu.Visible = true;
                        }

                        if (div_PosicaoMenu.Visible) PopulaPosicaoMenu();

                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, "sDscUsuario"));
                    }
                }
                else
                    pnMenu.Visible = false;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        protected void PopulaCombos(string idSistema) => FUNCOES.Popula_Combo(ddlidRecursoPai, "sp_Select 'Recursos', @idPesquisa=" + idSistema, "idRecurso", "sDscRecurso", true, "Selecione uma Permissão Pai", "0");

        protected void PopulaPosicaoMenu()
        {
            ltrPosicaoMenu.Text = $@"<div class='linksMenu'>
                                        <table class='table table-bordered'>
                                            <tbody id='menuTableBody'>

                                            </tbody>
                                        </table>
                                    </div>
                                    <div class='editarMenu navbar-default sidebar-collapse' role='navigation'>
                                        <ul class='nav'>
                                            {BD.ExecutarDataTable(sProcedure_Valida_Recursos, new Dictionary<string, string> { { "@idRecurso", Request["id"] }, { "@idUsuario", Identity.Variaveis.idUsuario() } }).Rows[0][0]}
                                        </ul>
                                    </div>";

            RegistraScript();
        }

        protected void LimpaCampos()
        {
            ddlidSistema.SelectedValue = "0";
            txtsDscRecurso.Text = "";
            ddlidRecursoPai.SelectedValue = "0";
            SwitchMenu.Definir("N", "É Menu?", "N");
            SwitchPaginaInicial.Definir("N", "Página Inicial?", "N");
            SwitchAtivo.Definir("S", "Ativo?", "S");
            txtsURL.Text = "";
            txtsIcone.Text = "";
            txtnOrdem.Text = "";
            txtnOrdem_Menu.Text = "";
        }

        protected bool ValidarDados()
        {
            if (ddlidSistema.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione um Sistema!");
                return false;
            }
            if (ddlidRecursoPai.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione uma Permissão Pai!");
                return false;
            }
            if (txtsDscRecurso.Text.Length < 3)
            {
                MensagemPagina.MostraMensagem_Erro("A descrição deve possuir ao menos 3 caracteres!");
                return false;
            }
            if (txtnOrdem.Text.Length < 1)
            {
                MensagemPagina.MostraMensagem_Erro("Ordem da Permissão inválida!");
                return false;
            }
            if (txtnOrdem_Menu.Text.Length < 1)
            {
                MensagemPagina.MostraMensagem_Erro("Posição no Menu inválida!");
                return false;
            }

            return true;
        }

        protected void SalvarDados()
        {
            if (ValidarDados())
            {
                try
                {
                    string idRecurso = Request["id"].Trim();
                    string msg = "1";

                    if (idRecurso == "0")
                        msg = "2";

                    if (string.IsNullOrEmpty(txtsURL.Text))
                        txtsURL.Text = "N";

                    if (string.IsNullOrEmpty(txtsIcone.Text))
                        txtsIcone.Text = "N";

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR_RECURSO" },
                        { "@idRecursoDetalhe", idRecurso },
                        { "@idSistema", ddlidSistema.SelectedValue },
                        { "@idRecursoPaiDetalhe", ddlidRecursoPai.SelectedValue },
                        { "@sDscRecurso", txtsDscRecurso.Text },
                        { "@sURL", txtsURL.Text },
                        { "@nOrdem", txtnOrdem.Text },
                        { "@sMenu", SwitchMenu.Recuperar() },
                        { "@sPaginaInicial", SwitchPaginaInicial.Recuperar() },
                        { "@sAtivo", SwitchAtivo.Recuperar() },
                        { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() },
                        { "@sIcone", txtsIcone.Text },
                        { "@nOrdem_Menu", txtnOrdem_Menu.Text }
                    };
                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out string sErro))
                    {
                        idRecurso = RETORNO.DATASET(dsSalvar, "idRecurso");

                        if (!string.IsNullOrEmpty(idRecurso))
                            FUNCOES.DirecionaPagina(string.Format("App/Paginas/IT/Recursos_Detalhe.aspx?id={0}&msg={1}", idRecurso, msg));
                    }
                    else
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
                }
            }
        }

        protected void cmdSalvar_Click(object sender, EventArgs e) => SalvarDados();

        protected void txtsIcone_TextChanged(object sender, EventArgs e)
        {
            var txtIcone = sender as TextBox;

            if (txtIcone.Text.StartsWith("fa fa-"))
            {
                div_PreviaIcone.Visible = true;
                ltrPreviaIcone.Text = string.Format("<i class=\"{0}\"></i>", txtIcone.Text);
            }
            else
                div_PreviaIcone.Visible = false;

            MensagemPagina.MostraMensagem_Aviso("<b>Lembrete: </b>os Ícones que são sugeridos no campo de Ícone não são os únicos existentes ou permitidos, lá aparecerão todos que já foram cadastrados em outras Permissões do Menu, apenas com o intuito de auxiliar sua busca.", false);

            FUNCOES.Scripts.FocusScript(Page, txtnOrdem.ClientID);
        }

        protected void ddlidRecursoPai_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                pnMenu.Visible = true;

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTA_RECURSOS_FILHOS" },
                    { "@idRecursoPaiDetalhe", ddlidRecursoPai.SelectedValue }
                };
                DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                if (dsPesquisa.Tables[0].Rows.Count > 0)
                {
                    txtnOrdem.Text = (int.Parse(dsPesquisa.Tables[0].Rows[dsPesquisa.Tables[0].Rows.Count - 1]["nOrdem"].ToString()) + 1).ToString();

                    if (ddlidRecursoPai.SelectedValue != "2")
                        txtnOrdem_Menu.Text = int.Parse(RETORNO.DATASET(dsPesquisa, "nOrdem_MenuPai")).ToString();
                    else
                        txtnOrdem_Menu.Text = (int.Parse(dsPesquisa.Tables[0].Rows[dsPesquisa.Tables[0].Rows.Count - 1]["nOrdem_Menu"].ToString()) + 1).ToString();
                }
                else
                {
                    txtnOrdem.Text = "0";
                    txtnOrdem_Menu.Text = RETORNO.DATASET(dsPesquisa, 1, 0, "nOrdem_Menu");
                }

                if (Request["id"] == "0")
                {
                    icon_nOrdem.Visible = true;
                    icon_nOrdem_Menu.Visible = true;

                    icon_nOrdem.Attributes["title"] = "Este campo será responsável por definir a Ordem em que esta Permissão aparecerá dentro de seu Permissão Pai!";
                    icon_nOrdem_Menu.Attributes["title"] = $"Este campo será responsável por definir o Andar de Permissões no Menu, ao qual esta Permissão pertence!<br>O ideal é que se mantenha igual ao Andar de sua Permissão Pai, sendo:<br><b>{txtnOrdem_Menu.Text}</b>";
                }

                    FUNCOES.Scripts.FocusScript(Page, txtsURL.ClientID);

            }
            catch { }
        }

        protected void ddlidSistema_SelectedIndexChanged(object sender, EventArgs e)
        {
            div_idRecursoPai.Visible = true;

            if (ddlidSistema.SelectedValue != "0") PopulaCombos(ddlidSistema.SelectedValue);

            if (ddlidSistema.SelectedValue == "8")
            {
                div_PosicaoMenu.Visible = false;
                div_txtnOrdem_Menu.Visible = false;
                div_sMenu.Visible = false;

                txtnOrdem_Menu.Text = "0";
            }
            else
            {
                if ((!string.IsNullOrEmpty(txtsURL.Text) && txtsURL.Text != "N") || SwitchMenu.Recuperar() == "S")
                {
                    div_PosicaoMenu.Visible = true;
                    PopulaPosicaoMenu();
                }

                div_txtnOrdem_Menu.Visible = true;
                div_sMenu.Visible = true;

                ddlidRecursoPai_SelectedIndexChanged(null, null);
            }

            SwitchMenu.Definir("N", "É Menu?", "N");

            FUNCOES.Scripts.FocusScript(Page, txtsDscRecurso.ClientID);
        }

        protected void RegistraScript()
        {
            FUNCOES.Scripts.Aplica_TooltipPersonalizado(Page, "tooltip");

            StringBuilder sb = new StringBuilder();

            string icones = JsonConvert.SerializeObject(hddIcones.Value.Split('|').ToList().OrderBy(i => i).Distinct());

            sb.Append("$v192(function() {\r\n");
            sb.Append("$v192(\"[id*=" + txtsIcone.ClientID + "]\").autocomplete({\r\n");
            sb.Append("source: function(request, response) {\r\n");
            sb.Append("$v192.ajax({\r\n");
            sb.Append("url: '/app/Paginas/IT/Recursos_Detalhe.aspx/GetIcones',\r\n");
            sb.Append("data: JSON.stringify({\r\n");

            sb.Append("'hddIcones': " + icones + ", \r\n");
            sb.Append("'sDscIcone': JSON.stringify(request.term), \r\n");

            sb.Append("}),\r\n"); // Adicione uma vírgula após a chave 'data'

            sb.Append("dataType: \"json\",\r\n");
            sb.Append("type: \"POST\",\r\n");
            sb.Append("contentType: \"application/json; charset=utf-8\",\r\n");
            sb.Append("success: function(data) {\r\n");
            sb.Append("response($v192.map(data.d, function(item) {\r\n");
            sb.Append("return {\r\n");

            sb.Append("label: item.split('|')[0],\r\n");

            sb.Append(txtsIcone.ClientID + ": item.split('|')[0],\r\n");

            sb.Append("};\r\n");
            sb.Append("}));\r\n"); // Adicione parênteses de fechamento para a função 'map'
            sb.Append("},\r\n");
            sb.Append("error: function(response) {\r\n");
            sb.Append("console.log(response.responseText);\r\n");
            sb.Append("},\r\n");
            sb.Append("failure: function(response) {\r\n");
            sb.Append("console.log(response.responseText);\r\n");
            sb.Append("}\r\n");
            sb.Append("});\r\n");
            sb.Append("},\r\n");
            sb.Append("select: function(e, i) {\r\n");

            sb.Append("$(\"[id$=" + txtsIcone.ClientID + "]\").val(i.item." + txtsIcone.ClientID + ");\r\n");

            sb.Append("},\r\n");
            sb.Append("minLength: 2\r\n");
            sb.Append("});\r\n");
            sb.Append("});\r\n\r\n");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Recursos_Icones", sb.ToString(), true);

            sb.Clear();

            sb.AppendLine("$(document).ready(function() {");
            sb.AppendLine("     $('.cmdMenu').off('click').on('click', function() {");
            sb.AppendLine("         var $this = $(this).find('.fa');");
            sb.AppendLine("         $this.toggleClass('fa-chevron-down fa-chevron-up');");
            sb.AppendLine("         $this.closest('.panel-heading').siblings('.panel-body').toggle('normal');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('.editarMenu').find('li').each(function() {");
            sb.AppendLine("         $(this).children('a').children('span').removeClass('fa arrow').addClass('fa fa-angle-left');");
            sb.AppendLine($"        $(this).find('a.{Request["id"]}').addClass('recursoAtual');");
            sb.AppendLine($"        $(this).find('a.{Request["id"]}').closest('ul').siblings('a').addClass('recursoPai');");
            sb.AppendLine("         var $submenu = $(this).children('ul');");
            sb.AppendLine("         $(this).children('a').off('click').on('click', function(e) {");
            sb.AppendLine("             e.preventDefault();");
            sb.AppendLine("             $(this).find('span').toggleClass('fa-angle-left fa-angle-down');");
            sb.AppendLine("             if ($submenu.length) {");
            sb.AppendLine("                 $submenu.toggleClass('collapse in');");
            sb.AppendLine("                 if (!$submenu.hasClass('in')) {");
            sb.AppendLine("                     $submenu.find('li ul').removeClass('in').addClass('collapse');");
            sb.AppendLine("                     $submenu.find('li ul').siblings('a').find('span').removeClass('fa-angle-down').addClass('fa-angle-left');");
            sb.AppendLine("                 }");
            sb.AppendLine("                 atualizarTabela();");
            sb.AppendLine("             }");
            sb.AppendLine("         });");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     function atualizarTabela() {");
            sb.AppendLine("         $('#menuTableBody').empty();");
            sb.AppendLine("         $('.editarMenu a').each(function () {");
            sb.AppendLine("             var $text = $(this).text();");
            sb.AppendLine("             var classeID = $(this).attr('class').split(' ')[0];");
            sb.AppendLine("             var href = `/App/Paginas/IT/Recursos_Detalhe.aspx?id=${classeID}`;");
            sb.AppendLine("             if ($(this).closest('ul').hasClass('in') || $(this).siblings('ul').hasClass('nav nav-second-level') || (!$(this).parents('ul').hasClass('nav-second-level') && !$(this).parents('ul').hasClass('nav-third-level') && !$(this).siblings('ul').length)) {");
            sb.AppendLine("                 var linha = `<tr class='${classeID}'><td><a href='${href}' class='btn btn-xs btn-primary' target='_blank' data-toggle='tooltip' title='${$text}'><i class='fa fa-external-link'></i></a></td></tr>`;");
            sb.AppendLine("                 $('#menuTableBody').append(linha);");
            sb.AppendLine("             }");
            sb.AppendLine("         });");
            sb.AppendLine("         $('[data-toggle=\"tooltip\"]').tooltip();");
            sb.AppendLine("     }");
            sb.AppendLine("");
            sb.AppendLine("     atualizarTabela();");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Edicao_Menu", sb.ToString(), true);
        }

        [WebMethod]
        public static string[] GetIcones(List<string> hddIcones, string sDscIcone) => hddIcones.Where(s => s.Split('|')[0].Trim().ToLower().Contains(sDscIcone.Replace("\"", "").Trim().ToLower())).ToArray();
    }
}