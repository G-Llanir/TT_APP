using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using static TT.FrameWork.BD;
using static TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.Atividades
{
    public partial class Projetos : Page
    {
        protected static string sTituloPagina = "Projetos";
        protected static string sPagina_NovoRegistro = "App/Paginas/Atividades/Projetos_Detalhe.aspx?id=0";
        protected static int nColunasPadrao_gvApontamentos = 9;
        protected static int nColunas_AntesDasDatas_Apontamentos = 18;

        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-Projetos.pdf";
            manual.sConfig_Introducao = manual.ConfigurarIntroducao(GerarConfig_Manual());

            ValidaPermissao(Permissao.Atividades.Projetos.Consultar, true);
            cmdNovo.Visible = ValidaPermissao(Permissao.Atividades.Projetos.Incluir);

            if (!IsPostBack)
            {
                string filtroDepto = !ValidaPermissao(Permissao.Atividades.Projetos.Nivel_Diretor, false) ? $"'Departamentos_x_Usuarios', {Identity.Variaveis.idUsuario()}" : "'Flow_Departamentos'";

                Popula_Combo(ddlidEmpresa, $"sp_Select 'Flow_Empresa', @idUsuario={Identity.Variaveis.idUsuario()}", "idEmpresa", "sDscCodigoEmpresa", false, "Todas as Empresas", "0");
                Popula_Combo(ddlidDepartamento, $"sp_Select {filtroDepto}", "idDepartamento", "sDscDepartamento", false, "Todos os Departamentos", "0");
                ddlidDepartamento.SelectedIndex = 0;
                Popula_Combo(ddlidUsuario, $"sp_Select 'Usuarios'", "idUsuario", "sDscUsuario", false, "Todos os Usuários", "0");
                ddlidUsuario.SelectedIndex = 0;
                Popula_Combo(ddlTipoPonto, $"sp_Select 'tbl_Flow_Usuarios_Ponto_Tipo'", "idTipoPonto", "sDscTipoPonto", false, "Todos os Tipos", "0");

                txtData_Inicio.Text = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
                txtData_Fim.Text = DateTime.Today.ToString("yyyy-MM-dd");

                if (!cbApontamentos.Checked) ddlidStatus.SelectedIndex = 0; 
                else ddlidStatus.SelectedIndex = 1;

                try
                {
                    DataTable tb = ExecutarDataTable("sp_Manipula_tbl_Flow_Atividades", new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_TIPO_VISUALIZACAO_x_USUARIO" }, { "@idUsuario_Logado", Identity.Variaveis.idUsuario() } });

                    if (!ValidaPermissao(Permissao.Atividades.Projetos.Nivel_Diretor, false) && !ValidaPermissao(Permissao.Atividades.Projetos.Nivel_Gestor, false))
                    {
                        ddlidDepartamento.Attributes.Add("disabled", "disabled");
                        ddlidUsuario.Attributes.Add("disabled", "disabled");

                        ddlidUsuario.SelectedValue = Identity.Variaveis.idUsuario();
                    }

                    Pesquisar(tb.Rows[0].Field<int>("idTipoVisualizacao_Atividades"), false);
                }
                catch
                {
                    Pesquisar(1);
                }
            }

            Scripts.FocusScript(Page, txtPesquisa.ClientID);
            RegistraScript();
        }

        protected void Pesquisar(int idTipoVisualizacao, bool bSalva_TipoVisualizacao = false)
        {
            Calendario.TipoEvento = 2;

            pnProjetos.Visible = false;
            pnAtividades.Visible = false;
            pnCalendario.Attributes.Add("class", "invisivel");
            pnApontamentos.Visible = false;
            pnKanban.Visible = false;
            pnPonto.Visible = false;
            pnMensagem.Visible = false;

            gvProjetos.DataSource = null;
            gvProjetos.DataBind();
            gvAtividades.DataSource = null;
            gvAtividades.DataBind();
            gvApontamentos.DataSource = null;
            gvApontamentos.DataBind();
            //gvKanban.DataSource = null;
            //gvKanban.DataBind();
            gvPonto.DataSource = null;
            gvPonto.DataBind();

            cbProjetos.Checked = false;
            cbAtividades.Checked = false;
            cbCalendario.Checked = false;
            cbApontamentos.Checked = false;
            cbKanban.Checked = false;
            cbPonto.Checked = false;

            div_ddlidStatus.Visible = true;
            div_ddlidEmpresa.Visible = false;
            div_ddlTipoPonto.Visible = false;
            div_datas.Visible = false;

            divLegenda.Visible = false;

            ddlidStatus.Items.FindByValue("4").Enabled = true;
            ddlidStatus.Items.FindByValue("7").Enabled = false;

            bool projetos = idTipoVisualizacao == 1;
            bool atividades = idTipoVisualizacao == 2;
            bool calendario = idTipoVisualizacao == 3;
            bool apontamentos = idTipoVisualizacao == 4;
            //bool kanban = idTipoVisualizacao == 5;
            bool ponto = idTipoVisualizacao == 6;

            string sFuncao = "CONSULTAR_PROJETOS";
            sTituloPagina = "Projetos";

            if (projetos)
            {
                cbProjetos.Checked = true;
                div_ddlidEmpresa.Visible = true;
                ddlidStatus.Items.FindByValue("4").Enabled = false;
                ddlidStatus.Items.FindByValue("7").Enabled = true;

                divLegenda.Visible = true;
            }
            else if (atividades)
            {
                sFuncao = "CONSULTAR_ATIVIDADES";
                sTituloPagina = "Atividades";
                cbAtividades.Checked = true;
                ddlidEmpresa.SelectedIndex = 0;
            }
            else if (calendario)
            {
                sFuncao = "CONSULTAR_ATIVIDADES";
                sTituloPagina = "Calendário de Atividades";
                cbCalendario.Checked = true;
                ddlidEmpresa.SelectedIndex = 0;
            }
            else if (apontamentos)
            {
                sFuncao = "CONSULTAR_APONTAMENTOS";
                sTituloPagina = "Apontamentos";
                cbApontamentos.Checked = true;
                ddlidEmpresa.SelectedIndex = 0;
                div_ddlidStatus.Visible = false;
                div_datas.Visible = true;
            }
            //else if (kanban)
            //{
            //    sFuncao = "CONSULTAR_APONTAMENTOS";
            //    sTituloPagina = "Apontamentos";
            //    cbApontamentos.Checked = true;
            //    ddlidEmpresa.SelectedIndex = 0;
            //    div_datas.Visible = true;
            //}
            else if (ponto)
            {
                sFuncao = "CONSULTAR_PONTOS";
                sTituloPagina = "Pontos";
                cbPonto.Checked = true;
                ddlidEmpresa.SelectedIndex = 0;
                div_ddlidStatus.Visible = false;
                div_ddlTipoPonto.Visible = true;
                div_datas.Visible = true;
                cmdNovo.Visible = false;
            }

            lblTituloPagina.InnerText = sTituloPagina;
            BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
            BreadCrumb_Pagina.AtualizarTitulo_BreadCrumb(titulo: sTituloPagina);

            string sUsuarios = string.Empty;
            string sDepartametos = string.Empty;
            string sStatus = string.Empty;
            foreach (ListItem item in ddlidUsuario.Items) if (item.Selected) sUsuarios += item.Value + ",";
            foreach (ListItem item in ddlidDepartamento.Items) if (item.Selected) sDepartametos += item.Value + ",";
            if (div_ddlidStatus.Visible) foreach (ListItem item in ddlidStatus.Items) if (item.Selected) sStatus += item.Value + ",";

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", sFuncao },
                { "@sDscPesquisa", txtPesquisa.Text.Trim() },
                { "@idEmpresa", ddlidEmpresa.SelectedValue },
                { "@sUsuarios", sUsuarios.TrimEnd(',').Trim() },
                { "@sDepartamentos", sDepartametos.TrimEnd(',').Trim() },
                { "@sStatus", sStatus.TrimEnd(',').Trim() },
                { "@idTipoPonto", ddlTipoPonto.SelectedValue },
                { "@dtInicio_Filtro", txtData_Inicio.Text },
                { "@dtFim_Filtro", txtData_Fim.Text },
                { "@idTipoVisualizacao", bSalva_TipoVisualizacao ? idTipoVisualizacao.ToString() : "0" },
                { "@sSalva_Preferencias", IsPostBack ? "S" : "N" },
                { "@nNivel_Permissao", ValidaPermissao(Permissao.Atividades.Projetos.Nivel_Diretor, false) ? "3" : ValidaPermissao(Permissao.Atividades.Projetos.Nivel_Gestor, false) ? "2" : "1" },
                { "@idUsuario_Logado", Identity.Variaveis.idUsuario() }
            };
            DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Atividades", vParametros);
            DataTable tb = ds.Tables[0];

            if (tb.Rows.Count > 0)
            {
                if (!calendario)
                {
                    Calendario.Desativar_Calendario();

                    if (projetos)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables_Projetos", Grid.DataBindComScriptData(gvProjetos, tb, 0, new int[3] { 1, 9, 10 }, "desc", "false", "''"), true);
                        pnProjetos.Visible = true;
                    }
                    else if (atividades)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables_Atividades", Grid.DataBindComScriptData(gvAtividades, tb, 0, new int[2] { 9, 10 }, 1, "desc", "false", "''"), true);
                        pnAtividades.Visible = true;
                    }
                    else if (apontamentos)
                    {
                        int[] colunasTotais = null;

                        while (gvApontamentos.Columns.Count > nColunasPadrao_gvApontamentos) { try { gvApontamentos.Columns.RemoveAt(nColunasPadrao_gvApontamentos); } catch { } }

                        if (tb.Columns.Count > nColunas_AntesDasDatas_Apontamentos)
                        {
                            int coluna = nColunasPadrao_gvApontamentos;
                            colunasTotais = new int[tb.Columns.Count - nColunas_AntesDasDatas_Apontamentos];

                            foreach (DataColumn col in tb.Columns)
                            {
                                if (tb.Columns.IndexOf(col) < nColunas_AntesDasDatas_Apontamentos) continue;

                                DateTime dtApontamento = DateTime.Parse(col.ColumnName);
                                string classeDia = dtApontamento.DayOfWeek == DayOfWeek.Sunday ? "domingo" : dtApontamento.DayOfWeek == DayOfWeek.Monday ? "segunda" : dtApontamento.DayOfWeek == DayOfWeek.Tuesday ? "terca" :
                                                    dtApontamento.DayOfWeek == DayOfWeek.Wednesday ? "quarta" : dtApontamento.DayOfWeek == DayOfWeek.Thursday ? "quinta" : dtApontamento.DayOfWeek == DayOfWeek.Friday ? "sexta" :
                                                    dtApontamento.DayOfWeek == DayOfWeek.Saturday ? "sabado" : "";

                                HyperLinkField link = new HyperLinkField
                                {
                                    HeaderText = dtApontamento.ToString("dd/MM/yyyy"),
                                    DataTextField = col.ColumnName,
                                    ItemStyle = { Width = Unit.Percentage(5), CssClass = $"linkApontamentos {classeDia}", HorizontalAlign = HorizontalAlign.Center, VerticalAlign = VerticalAlign.Middle },
                                    HeaderStyle = { CssClass = classeDia },
                                    Target = col.ColumnName
                                };
                                gvApontamentos.Columns.Add(link);

                                colunasTotais[coluna - nColunasPadrao_gvApontamentos] = coluna;
                                coluna++;
                            }
                        }

                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables_Apontamentos", Grid.DataBindComScript_Geral(gvApontamentos, tb, true, false, true, "false", "''", "50", true, true, false, -1, "", new int[2] { 7, 8 }, 1, colunasTotais, false, false, true), true);
                        pnApontamentos.Visible = true;
                    }
                    //else if (kanban)
                    //{
                    //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables_Kanban", Grid.DataBindComScriptData(gvKanban, tb, 0, new int[3] { 1, 9, 10 }, "desc", "false", "''"), true);
                    //    pnKanban.Visible = true;
                    //}
                    else if (ponto)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables_Ponto", Grid.DataBindComScriptData(gvPonto, tb, 0, new int[1] { 3 }, "desc", "false", "''"), true);
                        pnPonto.Visible = true;
                    }
                }
                else
                {
                    Calendario.RegistraScript_Atividades(tb);
                    pnCalendario.Attributes.Remove("class");
                }
            }
            else
            {
                pnMensagem.Visible = true;
                MensagemPagina.MostraMensagem_Erro("Nenhum registro foi localizado para sua pesquisa!");
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar(cbProjetos.Checked ? 1 : cbAtividades.Checked ? 2 : cbCalendario.Checked ? 3 : cbApontamentos.Checked ? 4 : cbKanban.Checked ? 5 : cbPonto.Checked ? 6 : 0);

        protected void cmdNovo_Click(object sender, EventArgs e) => DirecionaPagina(sPagina_NovoRegistro);

        protected void cbTipoVisualizacao_CheckedChanged(object sender, EventArgs e) => Pesquisar(int.Parse((sender as CheckBox).Attributes["data-tipo"].ToString()), true);

        protected void gvProjetos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string idStatus = e.Row.Cells[3].Text;
                e.Row.Cells[3].Text = idStatus == "1" ? "Não Iniciado" : idStatus == "2" ? "Em Andamento" : idStatus == "3" ? "Pausado" : idStatus == "5" ? "Finalizado" : idStatus == "6" ? "Impeditivo" : idStatus == "7" ? "Pendente" : "Não Classificado";
            }
        }

        protected void gvApontamentos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("data-idAtividade", DataBinder.Eval(e.Row.DataItem, "idAtividade").ToString());
                e.Row.Attributes.Add("data-idUsuario", DataBinder.Eval(e.Row.DataItem, "idUsuario").ToString());
            }
        }

        protected void gvPonto_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
                e.Row.Cells[2].Text = HttpUtility.HtmlDecode(e.Row.Cells[2].Text);
        }

        public string GerarConfig_Manual()
        {
            string sConfig = $@"
                {manual.ConstruirPasso("Visão Geral", "Esta é a Página de <b>Projetos</b>, onde é possível consultar e cadastrar Projetos, Atividades e Apontamentos.")}
                , {manual.ConstruirPasso("Filtros", "Utilizando estes campos é possível filtrar a consulta dos dados.", "#painelFiltros")}
                , {manual.ConstruirPasso("Pesquisar", "Clicando neste botão será possível consultar os dados a partir dos <b>Filtros</b> selecionados.", $"#{cmdPesquisar.ClientID}")}
                , {manual.ConstruirPasso("Novo Projeto", "Clicando neste botão você será direcionado para a Página de <b>Novo Projeto</b>.", $"#{cmdNovo.ClientID}")}
                , {manual.ConstruirPasso("Tipos de Visualização", "Por aqui será possível selecionar o <b>Tipo de Visualização</b> dos dados filtrados.", "#div_TiposView", "left")}
            ";

            if (!pnMensagem.Visible)
            {
                if (cbProjetos.Checked) sConfig += $", {manual.ConstruirPasso("Projetos", "Nesta seção serão exibidos os <b>Projetos</b> consultados.", $"#{pnProjetos.ClientID}", "top")}";
                else if (cbAtividades.Checked) sConfig += $", {manual.ConstruirPasso("Atividades", "Nesta seção serão exibidas as <b>Atividades</b> consultadas.", $"#{pnAtividades.ClientID}", "top")}";
                else if (cbApontamentos.Checked) sConfig += $", {manual.ConstruirPasso("Apontamentos", "Nesta seção serão exibidos os <b>Apontamentos</b> consultados.<br />Os Apontamentos podem ser visualizados nas colunas de Datas para cada Atividade.<br />Estas Atividades estarão agrupadas por Usuário Associado ao Apontamento.", $"#{pnApontamentos.ClientID}", "top")}";
                else if (cbCalendario.Checked) sConfig += $", {manual.ConstruirPasso("Calendário de Atividades", "Nesta seção serão exibidas as <b>Atividades</b> consultadas, em modelo de Calendário.", $"#{pnCalendario.ClientID}", "top")}";
            }

            return sConfig;
        }

        protected void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("");
            sb.AppendLine("     $(document).off('click', '.linkApontamentos a').on('click', '.linkApontamentos a', function (e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         const $this = $(this);");
            sb.AppendLine("         const row = $this.closest('tr');");
            sb.AppendLine("         const idAtividade = row.attr('data-idAtividade') || '0';");
            sb.AppendLine("         const idUsuario = row.attr('data-idUsuario') || '0';");
            sb.AppendLine("         const sDscAtividade = row.find('.tituloAtividade a').text() || '';");
            sb.AppendLine("         const dtApontamento = $this.attr('target') || '';");
            sb.AppendLine("         const total = parseFloat($this.text().replace('.', '').replace(',', '.')) || parseFloat($this.val().replace('.', '').replace(',', '.')) || 0;");
            sb.AppendLine("         $('#modalApontamentos').find('.modal-title').text(`Apontamentos${dtApontamento ? ' do dia ' + new Date(dtApontamento.replace('-', '/')).toLocaleDateString('pt-BR') : ''}`);");
            sb.AppendLine("         if (sDscAtividade) $('#modalApontamentos').find('.atividade_apontamentos').text(sDscAtividade);");
            sb.AppendLine("         $.ajax({");
            sb.AppendLine("             url: '/App/Paginas/Atividades/Projetos.aspx/Get_PopulaModal_Apontamentos',");
            sb.AppendLine("             data: JSON.stringify({ idAtividade: idAtividade, dtApontamento: dtApontamento, total: total, idUsuario: idUsuario }),");
            sb.AppendLine("             type: 'POST',");
            sb.AppendLine("             dataType: 'json',");
            sb.AppendLine("             contentType: 'application/json; charset=utf-8',");
            sb.AppendLine("             success: function (response) {");
            sb.AppendLine("                 if (response && response.d) {");
            sb.AppendLine("                     let apontamentos = response.d;");
            sb.AppendLine("                     $('.tabelaApontamentos').html(apontamentos);");
            sb.AppendLine("                     $('#modalApontamentos').modal('show');");
            sb.AppendLine("                 }");
            sb.AppendLine("             }");
            sb.AppendLine("         });");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $(document).off('change', '[id*=ddlidDepartamento]').on('change', '[id*=ddlidDepartamento]', function () {");
            sb.AppendLine("         const sDepartamentos = ($('select[id*=ddlidDepartamento]').val() || ['']).join('|');");
            sb.AppendLine("         const ddlUsuarios = $('select[id*=ddlidUsuario]');");
            sb.AppendLine("         const idUsuario = ddlUsuarios.val();");
            sb.AppendLine("         $.ajax({");
            sb.AppendLine("             url: '/App/Paginas/Atividades/Projetos.aspx/Get_PopulaUsuarios',");
            sb.AppendLine("             data: JSON.stringify({ sDepartamentos: sDepartamentos }),");
            sb.AppendLine("             type: 'POST',");
            sb.AppendLine("             dataType: 'json',");
            sb.AppendLine("             contentType: 'application/json; charset=utf-8',");
            sb.AppendLine("             success: function (response) {");
            sb.AppendLine("                 let usuarios = response && response.d ? response.d : response;");
            sb.AppendLine("                 ddlUsuarios.empty();");
            sb.AppendLine("                 ddlUsuarios.append(usuarios);");
            sb.AppendLine("                 ddlUsuarios.val(usuarios && idUsuario.length ? idUsuario : '0');");
            sb.AppendLine("                 ddlUsuarios.trigger('chosen:updated');");
            sb.AppendLine("             }");
            sb.AppendLine("         });");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     setTimeout(function() {");
            sb.AppendLine("         $('[id*=ddlidDepartamento]').trigger('change');");
            sb.AppendLine("     }, 50);");
            sb.AppendLine("");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(Page, GetType(), "RegistraScript", sb.ToString(), true);
        }

        [WebMethod]
        public static string Get_PopulaUsuarios(string sDepartamentos)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_USUARIOS_x_DEPTO" },
                { "@idUsuario_Logado", Identity.Variaveis.idUsuario() },
                { "@nNivel_Permissao", ValidaPermissao(Permissao.Atividades.Projetos.Nivel_Diretor, false) ? "3" : ValidaPermissao(Permissao.Atividades.Projetos.Nivel_Gestor, false) ? "2" : "1" },
                { "@sDepartamentos", sDepartamentos }
            };
            DataTable tb = ExecutarDataTable("sp_Manipula_tbl_Flow_Atividades", vParametros);

            string usuarios = string.Empty;

            if (tb.Rows.Count > 0)
            {
                usuarios = $"<option value='0'>Todos os Usuários</option>";
                foreach (DataRow row in tb.Rows)
                    usuarios += $"<option value='{row[0]}'>{row[1]}</option>";
            }

            return usuarios;
        }

        [WebMethod]
        public static string Get_PopulaModal_Apontamentos(string idAtividade, string dtApontamento, decimal total, string idUsuario)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_APONTAMENTOS_MODAL" },
                { "@idAtividade", idAtividade },
                { "@dtFiltro", dtApontamento },
                { "@idUsuario", idUsuario }
            };
            DataTable tb = ExecutarDataTable("sp_Manipula_tbl_Flow_Atividades", vParametros);

            string apontamentos = string.Empty;

            foreach (DataRow row in tb.Rows)
            {
                apontamentos += $@"
						            <tr>
							            <td style='text-align: left;'>{row["sObservacao"]}</td>
							            <td style='text-align: center;'>{row["nHoras"]}</td>
							            <td style='text-align: left;'>{row["sDscUsuario"]}</td>
						            </tr>";
            }

            string tabelaBase = $@"
                <table class='table' style='margin: 0;'>
                    <thead>
                        <tr>
                            <th style='width: 70%; text-align: left;'>Observação</th>
                            <th style='width: 10%; text-align: center;'>Horas</th>
                            <th style='width: 20%; text-align: left;'>Usuário</th>
                        </tr>
                    </thead>
                    <tbody>
                        {apontamentos}
                        <tr>
                            <td><b>Total: </b></td>
                            <td style='text-align: center;'><b>{total:N2}</b></td>
                            <td></td>
                        </tr>
                    </tbody>
                </table>";

            return tabelaBase;
        }
    }
}