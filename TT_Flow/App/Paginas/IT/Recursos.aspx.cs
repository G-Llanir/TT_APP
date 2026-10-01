using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.App.Controles;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.IT
{
    public partial class Recursos : Page
    {
        private static string sProcedure = "sp_Manipula_tbl_Recursos";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.TI.Controle_de_Permissoes.Incluir);
                FUNCOES.ValidaPermissao(Permissao.TI.Controle_de_Permissoes.Consultar, true);
                PopulaCombos();
                Pesquisar();
            }

            RegistraScript();
        }

        protected void Pesquisar()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_RECURSOS" },
                { "@sRecursos", txtPesquisa.Text.Trim() },
                { "@idSistema", ddlidSistema.SelectedValue },
                { "@idRecursoPaiDetalhe", ddlidRecursosPai.SelectedValue },
                { "@sAtivo", ddlidStatus.SelectedValue },
                { "@idFiltro", ddlFiltro.SelectedValue },
                { "@nOrdem_Menu", ddlAndar.SelectedValue }
            };
            DataTable tb = BD.ExecutarDataTable(sProcedure, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                pnResultado.Visible = true;
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb, 0, "asc"), true);
            }
            else
            {
                pnResultado.Visible = false;
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
            }
        }

        protected void PopulaCombos()
        {
            FUNCOES.Popula_Combo(ddlidRecursosPai, "sp_Select 'Recursos', @idFiltro=1", "idRecurso", "sDscRecurso", true, "Todas as Permissões Pai", "0");
            FUNCOES.Popula_Combo(ddlAndar, "sp_Select 'Andares_Permissoes'", "nOrdem_Menu", "nOrdem_Menu", false, "Todos os Andares", "-100");
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void cmdNovo_Click(object sender, EventArgs e) => FUNCOES.DirecionaPagina("App/Paginas/IT/Recursos_Detalhe.aspx?id=0");

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("data-id", (e.Row.Cells[0].Controls[0] as HyperLink).Text);

                var row = (e.Row.DataItem as DataRowView).Row;
                string sURL = row["sURL"].ToString().Trim().ToUpper();
                string sMenu = row["sMenu"].ToString().Trim().ToUpper();
                string sIcone = row["sIcone"].ToString().Trim();

                if (sURL.Equals("N")) e.Row.Cells[2].Text = string.Empty;

                e.Row.Cells[e.Row.Cells.Count - 2].Text = sMenu.Equals("S") ? HttpUtility.HtmlDecode("✔") : string.Empty;
                e.Row.Cells[e.Row.Cells.Count - 1].Text = !sIcone.Equals("N") ? string.Format("<i class=\"{0}\"></i>", sIcone) : string.Empty;
            }
        }

        protected void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$(document).ready(function () {");

            sb.AppendLine("");

            sb.AppendLine("     function MudarBotao() {");
            sb.AppendLine("         $('#cphCorpo_cmdPesquisar').val('Pesquisando...');");
            sb.AppendLine("     }");

            sb.AppendLine("");

            sb.AppendLine("     $(document).off('click', '.linkFilhas a').on('click', '.linkFilhas a', function (e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         const $this = $(this);");
            sb.AppendLine("         if ($this.text() == 0) return;");
            sb.AppendLine("         const row = $this.closest('tr');");
            sb.AppendLine("         const idRecurso = row.attr('data-id') || '0';");
            sb.AppendLine("         const sDsc = row.find('.sDsc a').text() || '';");
            sb.AppendLine("         if (sDsc) $('#modalRecursos').find('.pai').html(`Permissão Pai - <span style='text-decoration: underline;'>${sDsc}</span>`);");
            sb.AppendLine("         else $('#modalRecursos').find('.pai').html('');");
            sb.AppendLine("         $.ajax({");
            sb.AppendLine("             url: '/App/Paginas/IT/Recursos.aspx/Get_PopulaModal_Filhas',");
            sb.AppendLine("             data: JSON.stringify({ idRecurso: idRecurso }),");
            sb.AppendLine("             type: 'POST',");
            sb.AppendLine("             dataType: 'json',");
            sb.AppendLine("             contentType: 'application/json; charset=utf-8',");
            sb.AppendLine("             success: function (response) {");
            sb.AppendLine("                 if (response && response.d) {");
            sb.AppendLine("                     let filhas = response.d;");
            sb.AppendLine("                     $('.tabela').html(filhas);");
            sb.AppendLine("                     $('#modalRecursos').modal('show');");
            sb.AppendLine("                 }");
            sb.AppendLine("             }");
            sb.AppendLine("         });");
            sb.AppendLine("     });");

            sb.AppendLine("");

            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RegistraScript", sb.ToString(), true);
        }

        [WebMethod]
        public static string Get_PopulaModal_Filhas(string idRecurso)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_RECURSOS_FILHOS" },
                { "@idRecursoDetalhe", idRecurso }
            };
            DataTable tb = BD.ExecutarDataTable(sProcedure, vParametros);

            string filhas = string.Empty;

            foreach (DataRow row in tb.Rows)
            {
                string url = row["sURL"].ToString() == "N" ? string.Empty : row["sURL"].ToString();

                filhas += $@"<tr>
                                <td style='text-align: left;'><a target='_blank' href='/App/Paginas/IT/Recursos_Detalhe.aspx?id={row["idRecurso"]}'>{row["idRecurso"]}</a></td>
                                <td style='text-align: left;'><a target='_blank' href='/App/Paginas/IT/Recursos_Detalhe.aspx?id={row["idRecurso"]}'>{row["sDscRecurso"]}</a></td>
                                <td style='text-align: left;'><a target='_blank' href='{url}'>{url}</a></td>
                                <td style='text-align: center;'>{row["nOrdem"]}</td>
                                <td style='text-align: center;'>{row["nOrdem_Menu"]}</td>
                                <td style='text-align: center;'>{(row["sMenu"].ToString().Trim().ToUpper() == "S" ? "✔" : string.Empty)}</td>
                                <td style='text-align: center;'><i class='{row["sIcone"]}'></i></td>
                            </tr>";
            }

            string tabelaBase = $@"
                <table class='table' style='margin: 0;'>
                    <thead>
                        <tr>
                            <th style='width: 5%; text-align: left;'>ID</th>
                            <th style='width: 30%; text-align: left;'>Descrição</th>
                            <th style='width: 30%; text-align: left;'>URL</th>
                            <th style='width: 5%; text-align: center;'>Ordem</th>
                            <th style='width: 15%; text-align: center;'>Andar no Menu</th>
                            <th style='width: 10%; text-align: center;'>É Menu?</th>
                            <th style='width: 5%; text-align: center;'>Ícone</th>
                        </tr>
                    </thead>
                    <tbody>
                        {filhas}
                    </tbody>
                </table>";

            return tabelaBase;
        }
    }
}