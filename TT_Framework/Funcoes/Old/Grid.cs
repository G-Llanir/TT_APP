using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TT.FrameWork
{
    public class Grid
    {

        public static void DataBind(GridView gv, object tb)
        {
            gv.DataSource = tb;
            gv.DataBind();

            if (gv.Rows.Count > 0)
            {
                gv.HeaderRow.TableSection = TableRowSection.TableHeader;
                gv.UseAccessibleHeader = true;
                gv.FooterRow.TableSection = TableRowSection.TableFooter;
            }
        }

        public static void DataBind(GridView gv, DataSet ds) => DataBind(gv, ds, 0);
        public static void DataBind(GridView gv, DataSet ds, int idTabela)
        {
            gv.DataSource = ds.Tables[idTabela];
            gv.DataBind();

            if (ds.Tables[idTabela].Rows.Count > 0)
            {
                gv.HeaderRow.TableSection = TableRowSection.TableHeader;
                gv.UseAccessibleHeader = true;
                gv.FooterRow.TableSection = TableRowSection.TableFooter;
            }
        }

        public static void BotoesOcultarColuna(PlaceHolder placeholder, GridView gridView, Page page, List<int> ignoreIndexes = null)
        {
            if (ignoreIndexes == null)
                ignoreIndexes = new List<int>();

            foreach (DataControlField column in gridView.Columns)
            {
                if (column.Visible && !string.IsNullOrEmpty(column.HeaderText) && !ignoreIndexes.Contains(gridView.Columns.IndexOf(column)))
                {
                    string checkboxId = "toggle-col-" + gridView.Columns.IndexOf(column);
                    string gridViewClientId = gridView.ClientID;
                    string headerText = column.HeaderText.Replace(" ", "-");

                    Literal inputCheckbox = new Literal();
                    inputCheckbox.Text = $"<input type='checkbox' class='grid-toggle-check' id='{checkboxId}' data-gridview-id='{gridViewClientId}' data-column-header='{headerText}' onclick='toggleColumnVisibility(this);' autocomplete='off' {(column.Visible ? "checked" : "")}>";

                    Literal label = new Literal();
                    label.Text = $"<label class='grid-toggle-label' for='{checkboxId}'>{column.HeaderText}</label>";

                    placeholder.Controls.Add(inputCheckbox);
                    placeholder.Controls.Add(label);
                }
            }
        }

        public static void BotoesOcultarColunaComFiltro(PlaceHolder placeHolder, GridView gridView, Page page, List<int> ignoreIndexes = null, List<int> indicesOcultadosInicial = null)
        {
            if (ignoreIndexes == null)
                ignoreIndexes = new List<int>();

            if (indicesOcultadosInicial == null)
                indicesOcultadosInicial = new List<int>();

            foreach (DataControlField column in gridView.Columns)
            {
                int columnIndex = gridView.Columns.IndexOf(column);

                if (ignoreIndexes.Contains(columnIndex))
                    continue;

                if (string.IsNullOrEmpty(column.HeaderText))
                    continue;

                string checkboxId = "toggle-col-" + columnIndex;
                string gridViewClientId = gridView.ClientID;
                string heardText = column.HeaderText.Replace(" ", "-");

                bool isChecked = !indicesOcultadosInicial.Contains(columnIndex);

                Literal inputCheckbox = new Literal();
                inputCheckbox.Text = $"<input type='checkbox' class='grid-toggle-check' id='{checkboxId}' data-gridview-id='{gridViewClientId}' data-column-header='{heardText}' onclick='toggleColumnVisibility(this);' autocomplete= 'off' {(isChecked ? "checked" : "")}>";

                Literal label = new Literal();
                label.Text = $"<label class='grid-toggle-label' for='{checkboxId}'>{column.HeaderText}</label>";

                placeHolder.Controls.Add(inputCheckbox);
                placeHolder.Controls.Add(label);
            }

            if (indicesOcultadosInicial != null && indicesOcultadosInicial.Any())
            {
                var script = new StringBuilder();
                script.AppendLine("<script>");
                script.AppendLine("document.addEventListener('DOMContentLoaded', function () {");
                script.AppendLine("     function ocultarColunasIniciais() {");
                script.AppendLine("         var hiddenColumns = [");

                var hiddenHeaders = new List<string>();
                foreach (int index in indicesOcultadosInicial)
                {
                    if (index >= 0 && index < gridView.Columns.Count)
                    {
                        var col = gridView.Columns[index];
                        if (!ignoreIndexes.Contains(index) && !string.IsNullOrEmpty(col.HeaderText))
                        {
                            hiddenHeaders.Add(col.HeaderText.Replace(" ", "-"));
                        }
                    }
                }

                script.AppendLine(string.Join(",", hiddenHeaders.Select(h => $"\"{h}\"")));
                script.AppendLine("        ];");

                script.AppendLine($"        var gridViewId = '{gridView.ClientID}';");
                script.AppendLine("        hiddenColumns.forEach(function(headerText) {");
                script.AppendLine("            var checkbox = Array.from(document.querySelectorAll('.grid-toggle-check')).find(cb => cb.getAttribute('data-column-header') === headerText);");
                script.AppendLine("                ");
                script.AppendLine("            if (checkbox && typeof toggleColumnVisibility === 'function') {");
                script.AppendLine("                checkbox.checked = false;");
                script.AppendLine("                toggleColumnVisibility(checkbox);");
                script.AppendLine("            }");
                script.AppendLine("        });");
                script.AppendLine("    }");

                script.AppendLine("    if (typeof toggleColumnVisibility === 'function') {");
                script.AppendLine("        ocultarColunasIniciais();");
                script.AppendLine("    } else {");
                script.AppendLine("        setTimeout(ocultarColunasIniciais, 100);");
                script.AppendLine("    }");
                script.AppendLine("});");
                script.AppendLine("</script>");

                placeHolder.Controls.Add(new Literal { Text = script.ToString() });

            }
        }

        public static string DataBindComScript(GridView gv, DataTable tb) => DataBindComScript(gv, tb, 1, "asc");
        public static string DataBindComScript(GridView gv, object tb, int OrdenarColuna) => DataBindComScript(gv, tb, OrdenarColuna, "asc");
        public static string DataBindComScript(GridView gv, object tb, int OrdenarColuna, int[] naoOrdena) => DataBindComScript_Geral(gv, tb, true, true, true, "false", "''", "50", true, true, true, OrdenarColuna, "asc", null, colunasNaoOrdena: naoOrdena);
        public static string DataBindComScript(GridView gv, object tb, int OrdenarColuna, string TipoOrdenacao) => DataBindComScript_Geral(gv, tb, true, true, true, "false", "''", "50", true, true, true, OrdenarColuna, TipoOrdenacao, null);
        public static string DataBindComScriptPaging(GridView gv, object tb, bool bPaginacao, bool bPesquisa, bool bInfo, int OrdenarColuna, string TipoOrdenacao) => DataBindComScript_Geral(gv, tb, true, bPaginacao, true, "false", "''", "50", bPesquisa,
                                                                                                                                                                        bInfo, true, OrdenarColuna, TipoOrdenacao, null);
        public static string DataBindComScript(GridView gv, object tb, int OrdenarColuna, string TipoOrdenacao, string NumeroItens) => DataBindComScript_Geral(gv, tb, true, true, true, "false", "''", NumeroItens, true, true, true, OrdenarColuna,
                                                                                                                                        TipoOrdenacao, null);
        public static string DataBindComScript(GridView gv, object tb, int OrdenarColuna, string TipoOrdenacao, bool ExibirNumeroPaginas, bool ExibirPesquisa, bool ExibirItensPorPagina, bool ExibirInfoItens) => DataBindComScript_Geral(gv, tb, true,
                                                                                                                                                                                                                    ExibirNumeroPaginas, true, "false", "''",
                                                                                                                                                                                                                    "50", ExibirPesquisa, ExibirInfoItens,
                                                                                                                                                                                                                    ExibirItensPorPagina, OrdenarColuna,
                                                                                                                                                                                                                    TipoOrdenacao, null);
        public static string DataBindComScript(GridView gv, object ds) => DataBindComScript_Geral(gv, (ds as DataSet).Tables[0], true, true, true, "false", "''", "50", true, true, true, 1, "asc", null);
        public static string DataBindComScript(GridView gv, object tb, bool bPaging, int OrdenarColuna, string TipoOrdenacao, string scrollX, string scrollY) => DataBindComScript_Geral(gv, tb, true, bPaging, true, scrollX, scrollY, "50", true, true, true,
                                                                                                                                                                    OrdenarColuna, TipoOrdenacao, null);
        public static string DataBindComScriptData(GridView gv, object tb, int OrdenarColuna, string TipoOrdenacao, string scrollX, string scrollY)
            => DataBindComScript_Geral(gv, tb, true, true, true, scrollX, scrollY, "50", true, true, true, OrdenarColuna, TipoOrdenacao, new int[1] { OrdenarColuna });
        public static string DataBindComScriptData(GridView gv, object tb, int[] OrdenarColuna, string TipoOrdenacao, string scrollX, string scrollY)
            => DataBindComScript_Geral(gv, tb, true, true, true, scrollX, scrollY, "50", true, true, true, OrdenarColuna[0], TipoOrdenacao, OrdenarColuna);
        public static string DataBindComScriptData(GridView gv, object tb, int OrdenarColuna, int[] ColunasDatas, string TipoOrdenacao, string scrollX, string scrollY)
            => DataBindComScript_Geral(gv, tb, true, true, true, scrollX, scrollY, "50", true, true, true, OrdenarColuna, TipoOrdenacao, ColunasDatas);
        public static string DataBindComScriptData(GridView gv, object tb, int OrdenarColuna, int[] ColunasDatas, int colunaGrupo, string TipoOrdenacao, string scrollX, string scrollY)
            => DataBindComScript_Geral(gv, tb, true, true, true, scrollX, scrollY, "50", true, true, true, OrdenarColuna, TipoOrdenacao, ColunasDatas, colunaGrupo);
        public static string DataBindComScriptData(GridView gv, object tb, int OrdenarColuna, int[] ColunasDatas, int colunaGrupo, int[] colunasTotais, string TipoOrdenacao, string scrollX, string scrollY)
            => DataBindComScript_Geral(gv, tb, true, true, true, scrollX, scrollY, "50", true, true, true, OrdenarColuna, TipoOrdenacao, ColunasDatas, colunaGrupo, colunasTotais);
        public static string DataBindComScriptData(GridView gv, object tb, int OrdenarColuna, int[] ColunasDatas, int[] colunasTotais, int[] colunasInvisiveis, string TipoOrdenacao, string scrollX, string scrollY)
            => DataBindComScript_Geral(gv, tb, true, true, true, scrollX, scrollY, "50", true, true, true, OrdenarColuna, TipoOrdenacao, ColunasDatas, 0, colunasTotais, colunasInvisiveis);
        public static string DataBindComScriptData(GridView gv, object tb, int OrdenarColuna, int[] ColunasDatas, int colunaGrupo, int[] colunasTotais, int[] colunasInvisiveis, string TipoOrdenacao, string scrollX, string scrollY)
            => DataBindComScript_Geral(gv, tb, true, true, true, scrollX, scrollY, "50", true, true, true, OrdenarColuna, TipoOrdenacao, ColunasDatas, colunaGrupo, colunasTotais, colunasInvisiveis);
        public static string DataBindComScriptDataPaging(GridView gv, object tb, bool bPaging, int[] OrdenarColuna, string TipoOrdenacao, string scrollX, string scrollY)
            => DataBindComScript_Geral(gv, tb, true, bPaging, true, scrollX, scrollY, "50", true, true, true, OrdenarColuna[0], TipoOrdenacao, OrdenarColuna);
        public static string DataBindComScript_Geral(GridView gv, object tb, bool bResponsivo, bool bPaging, bool bScrollCollapse, string scrollX, string scrollY, string sPageLength, bool bPesquisa, bool bInfo, bool bAlterarNumeroItens, int OrdenarColuna,
            string sTipoOrdenacao, int[] colunasData, int colunaGrupo = -1, int[] colunasTotais = null, int[] colunasInvisiveis = null, bool bOrdenarColunas = true, bool bTotalGeral = false, bool bTotais_x_Grupos = false,
            int[] colunasNaoOrdena = null)
        {
            string grupos = string.Empty,
                    datas = string.Empty,
                    invisiveis = string.Empty,
                    naoOrdena = string.Empty,
                    columnDefs;

            if (colunaGrupo >= 0) grupos = "{ visible: false, targets: " + colunaGrupo + " }";
            if (colunasData != null) datas = "{ type: \"date-uk\", targets: [" + string.Join(",", colunasData) + "] }";
            if (colunasInvisiveis != null) invisiveis = "{ visible: false, targets: [" + string.Join(",", colunasInvisiveis) + "] }";
            if (colunasNaoOrdena != null) naoOrdena = "{ orderable: false, targets: [" + string.Join(",", colunasNaoOrdena) + "] }";

            columnDefs = grupos + datas + invisiveis + naoOrdena;
            columnDefs = columnDefs.Trim().Replace("}{", "}, {");

            StringBuilder sbRetorno = new StringBuilder();
            sbRetorno.AppendLine("$(document).ready(function() {");

            if (colunasData != null)
            {
                sbRetorno.AppendLine("      $.extend($.fn.dataTableExt.oSort, {");
                sbRetorno.AppendLine("        \"date-uk-pre\": function (a) {");
                sbRetorno.AppendLine("            var content;");
                sbRetorno.AppendLine("            try {");
                sbRetorno.AppendLine("                var $a = $(a);");
                sbRetorno.AppendLine("                if ($a.is('a')) {");
                sbRetorno.AppendLine("                    content = $a.text().trim();");
                sbRetorno.AppendLine("                } else {");
                sbRetorno.AppendLine("                    content = a.trim();");
                sbRetorno.AppendLine("                }");
                sbRetorno.AppendLine("            } catch (e) {");
                sbRetorno.AppendLine("                content = a.trim();");
                sbRetorno.AppendLine("            }");
                sbRetorno.AppendLine("            var parts = content.split(' ');");
                sbRetorno.AppendLine("            var ukDatea = parts[0].split('/');");
                sbRetorno.AppendLine("            var time = parts.length > 1 ? parts[1].split(':') : ['00', '00', '00'];");
                sbRetorno.AppendLine("            return (ukDatea[2] + ukDatea[1] + ukDatea[0] + time[0] + time[1] + time[2]) * 1;");
                sbRetorno.AppendLine("        },");
                sbRetorno.AppendLine("        \"date-uk-asc\": function (a, b) {");
                sbRetorno.AppendLine("            return ((a < b) ? -1 : ((a > b) ? 1 : 0));");
                sbRetorno.AppendLine("        },");
                sbRetorno.AppendLine("        \"date-uk-desc\": function (a, b) {");
                sbRetorno.AppendLine("            return ((a < b) ? 1 : ((a > b) ? -1 : 0));");
                sbRetorno.AppendLine("        }");
                sbRetorno.AppendLine("    });");
            }

            sbRetorno.AppendLine("      if ($.fn.DataTable.isDataTable('#" + gv.ClientID + "')) {");
            sbRetorno.AppendLine("          $('#" + gv.ClientID + "').DataTable().clear().destroy();");
            sbRetorno.AppendLine("      }");
            sbRetorno.AppendLine("      $('#" + gv.ClientID + "').DataTable({");
            sbRetorno.AppendLine($"         responsive: {bResponsivo.ToString().ToLower()},");
            sbRetorno.AppendLine($"         paging: {bPaging.ToString().ToLower()},");
            sbRetorno.AppendLine($"         scrollCollapse: {bScrollCollapse.ToString().ToLower()},");
            sbRetorno.AppendLine($"         scrollX: {scrollX},");
            sbRetorno.AppendLine($"         scrollY: {scrollY},");
            sbRetorno.AppendLine($"         pageLength: {sPageLength},");
            sbRetorno.AppendLine($"         searching: {bPesquisa.ToString().ToLower()},");
            sbRetorno.AppendLine($"         info: {bInfo.ToString().ToLower()},");
            sbRetorno.AppendLine($"         bLengthChange: {bAlterarNumeroItens.ToString().ToLower()},");
            sbRetorno.AppendLine($"         ordering: {bOrdenarColunas.ToString().ToLower()},");

            if (OrdenarColuna >= 0) sbRetorno.AppendLine($"         order: [[{OrdenarColuna}, '{sTipoOrdenacao}']],");
            if (colunasData != null || colunasNaoOrdena != null || colunaGrupo >= 0) sbRetorno.AppendLine($"         columnDefs: [ {columnDefs} ],");

            if (colunaGrupo >= 0 && colunasTotais != null && colunasTotais.Length > 0)
            {
                sbRetorno.AppendLine("          drawCallback: function (settings) {");
                sbRetorno.AppendLine("              var api = this.api();");
                sbRetorno.AppendLine("              var rows = api.rows({ page: 'current' }).nodes();");
                sbRetorno.AppendLine("              var lastGrupo = null;");
                sbRetorno.AppendLine("              var colunaGrupo = " + colunaGrupo + ";");
                sbRetorno.AppendLine("              var colunasTotais = [" + string.Join(",", colunasTotais) + "];");
                sbRetorno.AppendLine("              var grupoData = api.column(colunaGrupo, { page: 'current' }).data();");
                sbRetorno.AppendLine("              var totais = {};");
                sbRetorno.AppendLine("              var totalGeral = {};");
                sbRetorno.AppendLine("              grupoData.each(function (grupo, i) {");
                sbRetorno.AppendLine("                  if (lastGrupo !== grupo) {");
                sbRetorno.AppendLine("                      if (lastGrupo !== null && " + bTotais_x_Grupos.ToString().ToLower() + ") {");
                sbRetorno.AppendLine("                          var linhaTotalGrupo = $('<tr class=\"total-row\"/>');");
                sbRetorno.AppendLine("                          var primeiraVisivel = true;");
                sbRetorno.AppendLine("                          var colunasVisiveis = settings.aoColumns.map(function (col) { return col.bVisible; });");
                sbRetorno.AppendLine("                          api.columns().every(function (colIndex) {");
                sbRetorno.AppendLine("                              var visivel = colunasVisiveis[colIndex];");
                sbRetorno.AppendLine("                              var $td = $('<td/>');");
                sbRetorno.AppendLine("                              if (!visivel) { $td.css('display', 'none'); }");
                sbRetorno.AppendLine("                              if (visivel && primeiraVisivel) {");
                sbRetorno.AppendLine("                                  $td.html('<strong>Total</strong>').css({ 'padding-left': '6px' });");
                sbRetorno.AppendLine("                                  primeiraVisivel = false;");
                sbRetorno.AppendLine("                              } else if (colunasTotais.includes(colIndex)) {");
                sbRetorno.AppendLine("                                  var total = totais[colIndex] || 0;");
                sbRetorno.AppendLine("                                  $td.html('<strong>' + total.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '</strong>').css({ 'text-align': 'right' });");
                sbRetorno.AppendLine("                              }");
                sbRetorno.AppendLine("                              linhaTotalGrupo.append($td);");
                sbRetorno.AppendLine("                          });");
                sbRetorno.AppendLine("                          $(rows).eq(i - 1).after(linhaTotalGrupo);");
                sbRetorno.AppendLine("                          totais = {}; // reset para o próximo grupo");
                sbRetorno.AppendLine("                      }");
                sbRetorno.AppendLine("                      $(rows).eq(i).before(");
                sbRetorno.AppendLine("                          $('<tr class=\"group-header-row\"/>').append(");
                sbRetorno.AppendLine("                              $('<td/>').attr('colspan', api.columns().indexes().length).html('<strong>' + grupo + '</strong>').css({ 'background': '#d9edf7', 'font-weight': 'bold', 'padding': '8px' })");
                sbRetorno.AppendLine("                          )");
                sbRetorno.AppendLine("                      );");
                sbRetorno.AppendLine("                      lastGrupo = grupo;");
                sbRetorno.AppendLine("                  }");
                sbRetorno.AppendLine("                  colunasTotais.forEach(function (colIndex) {");
                sbRetorno.AppendLine("                      var valor = api.cell(i, colIndex).data();");
                sbRetorno.AppendLine("                      if (valor.toString().includes('<')) valor = parseFloat($(valor).val().toString().replace('.', '').replace(',', '.')) || parseFloat($(valor).text().toString().replace('.', '').replace(',', '.')) || 0;");
                sbRetorno.AppendLine("                      valor = valor || 0;");
                sbRetorno.AppendLine("                      totalGeral[colIndex] = (totalGeral[colIndex] || 0) + valor;");
                sbRetorno.AppendLine("                      totais[colIndex] = (totais[colIndex] || 0) + valor;");
                sbRetorno.AppendLine("                  });");
                sbRetorno.AppendLine("              });");
                sbRetorno.AppendLine("              if (" + bTotais_x_Grupos.ToString().ToLower() + ") {");
                sbRetorno.AppendLine("                  var linhaTotalGrupo = $('<tr class=\"total-row\"/>');");
                sbRetorno.AppendLine("                  var primeiraVisivel = true;");
                sbRetorno.AppendLine("                  var colunasVisiveis = settings.aoColumns.map(function (col) { return col.bVisible; });");
                sbRetorno.AppendLine("                  api.columns().every(function (colIndex) {");
                sbRetorno.AppendLine("                      var visivel = colunasVisiveis[colIndex];");
                sbRetorno.AppendLine("                      var $td = $('<td/>');");
                sbRetorno.AppendLine("                      if (!visivel) { $td.css('display', 'none'); }");
                sbRetorno.AppendLine("                      if (visivel && primeiraVisivel) {");
                sbRetorno.AppendLine("                          $td.html('<strong>Total</strong>').css({ 'padding-left': '6px' });");
                sbRetorno.AppendLine("                          primeiraVisivel = false;");
                sbRetorno.AppendLine("                      } else if (colunasTotais.includes(colIndex)) {");
                sbRetorno.AppendLine("                          var total = totais[colIndex] || 0;");
                sbRetorno.AppendLine("                          $td.html('<strong>' + total.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '</strong>')");
                sbRetorno.AppendLine("                              .css({ 'text-align': 'right' });");
                sbRetorno.AppendLine("                      }");
                sbRetorno.AppendLine("                      linhaTotalGrupo.append($td);");
                sbRetorno.AppendLine("                  });");
                sbRetorno.AppendLine("                  $(rows).last().after(linhaTotalGrupo);");
                sbRetorno.AppendLine("              }");
                sbRetorno.AppendLine("              if (" + bTotalGeral.ToString().ToLower() + ") {");
                sbRetorno.AppendLine("                  var linhaTotalGeral = $('<tr class=\"total-row\"/>');");
                sbRetorno.AppendLine("                  var primeiraVisivel = true;");
                sbRetorno.AppendLine("                  var colunasVisiveis = settings.aoColumns.map(function (col) { return col.bVisible; });");
                sbRetorno.AppendLine("                  api.columns().every(function (colIndex) {");
                sbRetorno.AppendLine("                      var visivel = colunasVisiveis[colIndex];");
                sbRetorno.AppendLine("                      var $td = $('<td/>');");
                sbRetorno.AppendLine("                      if (!visivel) { $td.css('display', 'none'); }");
                sbRetorno.AppendLine("                      if (visivel && primeiraVisivel) {");
                sbRetorno.AppendLine("                          $td.html('<strong>Total Geral</strong>').css({ 'padding-left': '6px' });");
                sbRetorno.AppendLine("                          primeiraVisivel = false;");
                sbRetorno.AppendLine("                      } else if (colunasTotais.includes(colIndex)) {");
                sbRetorno.AppendLine("                          var total = totalGeral[colIndex] || 0;");
                sbRetorno.AppendLine("                          $td.html('<strong>' + total.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '</strong>')");
                sbRetorno.AppendLine("                              .css({ 'text-align': 'right' });");
                sbRetorno.AppendLine("                      }");
                sbRetorno.AppendLine("                      linhaTotalGeral.append($td);");
                sbRetorno.AppendLine("                  });");
                sbRetorno.AppendLine("                  $(rows).last().after(linhaTotalGeral);");
                sbRetorno.AppendLine("              }");
                sbRetorno.AppendLine("          },");
            }
            else if (colunaGrupo >= 0)
            {
                sbRetorno.AppendLine("          drawCallback: function (settings) {");
                sbRetorno.AppendLine("              var api = this.api();");
                sbRetorno.AppendLine("              var rows = api.rows({ page: 'current' }).nodes();");
                sbRetorno.AppendLine("              var last = null;");
                sbRetorno.AppendLine("              api.column(" + colunaGrupo + ", { page: 'current' }).data().each(function (grupo, i) {");
                sbRetorno.AppendLine("                  if (last !== grupo) {");
                sbRetorno.AppendLine("                      $(rows).eq(i).before(");
                sbRetorno.AppendLine("                          $('<tr class=\"group-header-row\"/>').append(");
                sbRetorno.AppendLine("                              $('<td/>').attr('colspan', api.columns().indexes().length).html(grupo).css({ 'background': '#d9edf7', 'font-weight': 'bold', 'padding': '8px' })");
                sbRetorno.AppendLine("                          )");
                sbRetorno.AppendLine("                      );");
                sbRetorno.AppendLine("                      last = grupo;");
                sbRetorno.AppendLine("                  }");
                sbRetorno.AppendLine("              });");
                sbRetorno.AppendLine("          },");
            }
            else if (colunasTotais != null && colunasTotais.Length > 0)
            {
                sbRetorno.AppendLine("          drawCallback: function (settings) {");
                sbRetorno.AppendLine("              var api = this.api();");
                sbRetorno.AppendLine("              var rows = api.rows({ page: 'current' }).nodes();");
                sbRetorno.AppendLine("              var colunasTotais = [" + string.Join(",", colunasTotais) + "];");
                sbRetorno.AppendLine("              var totalGeral = {};");
                sbRetorno.AppendLine("              api.rows({ page: 'current' }).indexes().each(function (i) {");
                sbRetorno.AppendLine("                  colunasTotais.forEach(function (colIndex) {");
                sbRetorno.AppendLine("                      var valor = api.cell(i, colIndex).data();");
                sbRetorno.AppendLine("                      if (valor.toString().includes('<')) valor = parseFloat($(valor).val().toString().replace('.', '').replace(',', '.')) || parseFloat($(valor).text().toString().replace('.', '').replace(',', '.')) || 0;");
                sbRetorno.AppendLine("                      valor = valor || 0;");
                sbRetorno.AppendLine("                      totalGeral[colIndex] = (totalGeral[colIndex] || 0) + valor;");
                sbRetorno.AppendLine("                  });");
                sbRetorno.AppendLine("              });");
                sbRetorno.AppendLine("              var linhaTotalGeral = $('<tr class=\"total-row\"/>');");
                sbRetorno.AppendLine("              var primeiraVisivel = true;");
                sbRetorno.AppendLine("              var colunasVisiveis = settings.aoColumns.map(function (col) { return col.bVisible; });");
                sbRetorno.AppendLine("              api.columns().every(function (colIndex) {");
                sbRetorno.AppendLine("                  var visivel = colunasVisiveis.includes(colIndex);");
                sbRetorno.AppendLine("                  var $td = $('<td/>');");
                sbRetorno.AppendLine("                  if (!visivel) {");
                sbRetorno.AppendLine("                      $td.css('display', 'none');");
                sbRetorno.AppendLine("                  }");
                sbRetorno.AppendLine("                  if (visivel && primeiraVisivel) {");
                sbRetorno.AppendLine("                      $td.html('<strong>Total:</strong>').css({ 'padding-left': '6px' });");
                sbRetorno.AppendLine("                      primeiraVisivel = false;");
                sbRetorno.AppendLine("                  } else if (colunasTotais.includes(colIndex)) {");
                sbRetorno.AppendLine("                      var total = totalGeral[colIndex] || 0;");
                sbRetorno.AppendLine("                      $td.html('<strong>' + total.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '</strong>').css({ 'text-align': 'right' });");
                sbRetorno.AppendLine("                  }");
                sbRetorno.AppendLine("                  linhaTotalGeral.append($td);");
                sbRetorno.AppendLine("              });");
                sbRetorno.AppendLine("              $(rows).last().after(linhaTotalGeral);");
                sbRetorno.AppendLine("          },");
            }

            sbRetorno.AppendLine("          language: { url: 'https://cdn.datatables.net/plug-ins/1.11.5/i18n/pt-BR.json' }");
            sbRetorno.AppendLine("      });");
            sbRetorno.AppendLine("});");

            DataBind(gv, tb);
            return sbRetorno.ToString();
        }

        public static void EsconderColunas(GridViewRowEventArgs e, params int[] colunas)
        {
            if (e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Footer)
            {
                foreach (var col in colunas)
                {
                    if (col != -1) e.Row.Cells[col].Visible = false;
                }
            }
        }

        public static void MostrarColunas(GridViewRowEventArgs e, params int[] colunas)
        {
            if (e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Footer)
            {
                foreach (var col in colunas)
                {
                    if (col != -1) e.Row.Cells[col].Visible = true;
                }
            }
        }

        public enum Formatação
        {
            Inteiro = 1,
            Moeda = 2,
            Peso = 3,
            Numero = 4,
            ValorDecimal = 5
        }

        /// <summary>
        /// Função para Somar e Formatar Colunas de Valores.
        /// </summary>
        /// <param name="gv">GridView</param>
        /// <param name="MostraRotulo">Mostra totalização de Linhas</param>
        /// <param name="TipoFormatacao">Tipo da Formatação da Coluna <br />1 - Inteiro | N0<br />2 - Moeda | C2<br />3 - Número | N2<br />4 - Peso | N3<br />5 - Valor Decimal | N4</param>
        public static void SomarColunas(GridView gv, bool MostraRotulo, Formatação TipoFormatacao, params int[] colunas) => SomarColunas(gv, MostraRotulo, TipoFormatacao, null, "", "", colunas);
        public static void SomarColunas(GridView gv, bool MostraRotulo, Formatação TipoFormatacao, Type tipoCampo, params int[] colunas) => SomarColunas(gv, MostraRotulo, TipoFormatacao, tipoCampo, "", "", colunas);
        public static void SomarColunas(GridView gv, bool MostraRotulo, Formatação TipoFormatacao, string preFixo, string suFixo, params int[] colunas) => SomarColunas(gv, MostraRotulo, TipoFormatacao, null, preFixo, suFixo, colunas);
        public static void SomarColunas(GridView gv, bool MostraRotulo, Formatação TipoFormatacao, Type tipoCampo, string preFixo, string suFixo, params int[] colunas)
        {
            gv.ShowFooter = true;

            double[] totais = new double[colunas.Length];

            string sTpFormatacao = "";
            switch (TipoFormatacao)
            {
                case Formatação.Inteiro: sTpFormatacao = "{0:n0}"; break;
                case Formatação.Moeda: sTpFormatacao = "{0:C2}"; break;
                case Formatação.Numero: sTpFormatacao = "{0:n2}"; break;
                case Formatação.Peso: sTpFormatacao = "{0:n3}"; break;
                case Formatação.ValorDecimal: sTpFormatacao = "{0:n4}"; break;
            }

            int linhas = 0;
            for (int i = 0; i < gv.Rows.Count; i++)
            {
                if (gv.Rows[i].Visible)
                {
                    linhas++;

                    for (int j = 0; j < colunas.Length; j++)
                    {
                        int col = colunas[j];
                        if (col != -1)
                        {
                            double valor = RecuperaValorColuna(gv.Rows[i].Cells[col]);
                            string texto = string.Format(sTpFormatacao, valor);

                            if (tipoCampo == null)
                            {
                                gv.Rows[i].Cells[col].Text = texto;
                                gv.Rows[i].Cells[col].HorizontalAlign = HorizontalAlign.Left;
                            }
                            else if (tipoCampo == typeof(HyperLink))
                            {
                                var link = gv.Rows[i].Cells[col].Controls.OfType<HyperLink>().FirstOrDefault();
                                if (link != null) link.Text = texto;
                            }
                            else if (tipoCampo == typeof(TextBox))
                            {
                                var txt = gv.Rows[i].Cells[col].Controls.OfType<TextBox>().FirstOrDefault();
                                if (txt != null) txt.Text = texto;
                            }

                            totais[j] += valor;
                        }
                    }
                }
            }

            if (gv.Rows.Count > 0)
            {
                gv.FooterRow.Font.Bold = true;

                if (MostraRotulo)
                {
                    int nColunaTotalizacao = gv.FooterRow.Cells[0].Visible ? 0 : 1;
                    gv.FooterRow.Cells[nColunaTotalizacao].Text = "Total: " + gv.Rows.Count.ToString();
                    gv.FooterRow.Cells[nColunaTotalizacao].HorizontalAlign = HorizontalAlign.Left;
                }

                for (int j = 0; j < colunas.Length; j++)
                {
                    int col = colunas[j];
                    if (col != -1)
                    {
                        gv.FooterRow.Cells[col].Text = preFixo + string.Format(sTpFormatacao, totais[j]) + suFixo;
                        gv.FooterRow.Cells[col].HorizontalAlign = HorizontalAlign.Right;
                    }
                }
            }
        }

        static double RecuperaValorColuna(TableCell celula)
        {
            string sValor;

            try
            {
                if (celula.Controls.Count > 0)
                {
                    var link = celula.Controls.OfType<HyperLink>().FirstOrDefault();
                    var txt = celula.Controls.OfType<TextBox>().FirstOrDefault();

                    if (link != null) sValor = link.Text;
                    else if (txt != null) sValor = txt.Text;
                    else sValor = celula.Text;
                }
                else sValor = celula.Text;
            }
            catch
            {
                sValor = celula.Text;
            }

            double.TryParse(sValor.Replace("R$", "").Trim(), out double nValor);
            return nValor;
        }

        /// <summary>
        /// Função para obter o Index da Coluna a partir de seu Título.
        /// </summary>
        /// <param name="gv">recebe a GridView</param>
        /// <param name="titulo">recebe o Título da Coluna para procurar</param>
        /// <returns>Retorna um valor tipo INT com o Index da Coluna.</returns>
        public static int ObterIndexColunaPorTitulo(GridView gv, string titulo)
        {
            try
            {
                for (int i = 0; i <= gv.Columns.Count - 1; i++)
                {
                    if (gv.HeaderRow.Cells[i].Text.Contains(titulo))
                        return i;
                }

                return -1;
            }
            catch
            {
                return -1;
            }
        }
    }
}