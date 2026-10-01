using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Services;
using System.Web.UI;
using TT.FrameWork;
using static TT.FrameWork.Identity;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class Consulta_Colaboradores : Page
    {
        string sTituloPagina = "Consulta Colaboradores";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.RRHH.ConsultarDadosColaborador, true);

            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlidDepartamento, "sp_Select 'Flow_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Selecione o Departamento ", "0");

                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                Pesquisar();
            }
            RegistraScript();
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;
            cmdOrganograma.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA_DADOS_COLABORADORES" },
                { "@sPesquisa", txtPesquisa.Text.Trim() },
                { "@idDepartamento", ddlidDepartamento.SelectedValue },
                { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
            };
            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores", vParametros, false);

            if (BD.ValidarDataSet(ds))
            {
                pnResultado.Visible = true;
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, ds.Tables[0], 1, new int[1] { 6 }, "asc", "false", "''"), true);

                try { divOrganograma_Completo.Text = ds.Tables[1].Rows[0][0].ToString(); cmdOrganograma.Visible = true; } catch { }
            }
            else MensagemPagina.MostraMensagem_Erro("Nenhum registro Localizado");
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void RegistraScript()
        {
            FUNCOES.Scripts.FocusScript(Page, txtPesquisa.ClientID);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$(document).ready(function() {");
            sb.AppendLine("");
            sb.AppendLine($"    $('#{txtColaborador_Organograma.ClientID}').keydown(function(e) {{");
            sb.AppendLine("         if (e.keyCode == 13) {");
            sb.AppendLine("             e.preventDefault();");
            sb.AppendLine("             return false;");
            sb.AppendLine("         }");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("    $('.cmdSelecionados').off('click').on('click', function(e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         $('.colaborador .checkColaborador:has(.fa-check)').siblings('a').each(function() { this.click(); });");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("    $('.cmdLimpa_Selecionados').off('click').on('click', function(e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         $('.colaborador .checkColaborador .fa-check').removeClass('fa fa-check');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("    $('.checkColaborador').off('click').on('click', function(e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         $(this).find('i').toggleClass('fa fa-check');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine($"   $('#{cmdOrganograma.ClientID}').off('click').on('click', function(e) {{");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         $('#imagemModal').modal('show');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('#imagemModal').on('shown.bs.modal', function () {");
            sb.AppendLine("         const colaborador = $('li[data-idcolaborador=\"1\"] a');");
            sb.AppendLine("         if (colaborador.length) {");
            sb.AppendLine("             colaborador[0].scrollIntoView({ behavior: 'smooth', block: 'nearest', inline: 'center' });");
            sb.AppendLine("             setTimeout(function() {");
            sb.AppendLine("                 $('#" + txtColaborador_Organograma.ClientID + "').focus();");
            sb.AppendLine("             }, 10);");
            sb.AppendLine("         }");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("});");

            if (cmdOrganograma.Visible)
            {
                sb.AppendLine("$v192(function() {");
                sb.AppendLine("     $v192('#" + txtColaborador_Organograma.ClientID + "').autocomplete({");
                sb.AppendLine("         source: function(request, response) {");
                sb.AppendLine("             $v192.ajax({");
                sb.AppendLine("                 url:'/App/Paginas/RRHH/Consulta_Colaboradores.aspx/GetColaborador',");
                sb.AppendLine("                 data: JSON.stringify({ 'sDscColaborador': request.term }),");
                sb.AppendLine("                 dataType: \"json\",");
                sb.AppendLine("                 type: \"POST\",");
                sb.AppendLine("                 contentType: \"application/json; charset=utf-8\",");
                sb.AppendLine("                 success: function(data) {");
                sb.AppendLine("                     response($v192.map(data.d, function(item) {");
                sb.AppendLine("                         return {");
                sb.AppendLine("                             label: item.sDscColaborador,");
                sb.AppendLine("                             id: item.idColaborador");
                sb.AppendLine("                         };");
                sb.AppendLine("                     }));");
                sb.AppendLine("                 },");
                sb.AppendLine("                 error: function(response) {");
                sb.AppendLine("                     console.error(response.responseText);");
                sb.AppendLine("                 },");
                sb.AppendLine("                 failure: function(response) {");
                sb.AppendLine("                     console.error(response.responseText);");
                sb.AppendLine("                 }");
                sb.AppendLine("             });");
                sb.AppendLine("         },");
                sb.AppendLine("         select: function(e, i) {");
                sb.AppendLine("             const colaborador = $(`li[data-idcolaborador='${i.item.id}']`);");
                sb.AppendLine("             if (colaborador && colaborador.length) {");
                sb.AppendLine("                 colaborador[0].scrollIntoView({ behavior: 'smooth', block: 'nearest', inline: 'center' });");
                sb.AppendLine("                 setTimeout(function() {");
                sb.AppendLine("                     $('#" + txtColaborador_Organograma.ClientID + "').val('');");
                sb.AppendLine("                 }, 10);");
                sb.AppendLine("             }");
                sb.AppendLine("             colaborador.find('.checkColaborador i').addClass('fa fa-check');");
                sb.AppendLine("         }, minLength: 3");
                sb.AppendLine("     });");
                sb.AppendLine("});");
            }

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_RegistraScript", sb.ToString(), true);
        }

        [WebMethod]
        public static List<object> GetColaborador(string sDscColaborador)
        {
            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores", new Dictionary<string, string> { { "@sFuncao", "CONSULTA_COLABORADOR" }, { "@sDscColaborador", sDscColaborador } });

            List<object> list = new List<object>();
            foreach (DataRow dr in ds.Tables[0].Rows) { list.Add(new { sDscColaborador = dr["sDscColaborador"], idColaborador = dr["idColaborador"] }); }

            return list;
        }
    }
}