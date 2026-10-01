using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class RelatorioBeneficios_Detalhe : Page
    {
        #region | Construtores

        private static string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Beneficios";
        private static string _sConfirma = "N";

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.RRHH.RelatorioBeneficios.Consultar, true);
            PainelAtualizacao.Visible = false;

            decimal.TryParse(lblVlr_VR.InnerText.Replace("R$", ""), out decimal nVR);
            if (nVR <= 0)
            {
                DataTable tb = BD.ExecutarDataTable(sProcedure, new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_VALOR_VR" } });
                decimal.TryParse(tb.Rows[0][0].ToString(), out nVR);
            }
            lblVlr_VR.InnerText = $"R${nVR.ToString("N2")}";

            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
                FUNCOES.Popula_Combo(lstTipoContrato, "sp_Select 'tbl_Flow_Colaboradores_TipoContrato'", "idTipoContrato", "sDscTipoContrato", false);

                if (Request.QueryString["action"] == "export")
                {
                    if (Session["RelatorioExcel"] != null)
                    {
                        string sNomeArquivo = "Relatorio_VT_VR_";

                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "Consultar_Relatorio_Excel" },
                            { "@idRegistro", Session["RelatorioExcel"].ToString() }
                        };
                        DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

                        string sTituloRelatorio = Session["TituloRelatorioExcel"] != null ? Session["TituloRelatorioExcel"].ToString() : "Relatório VT e VR" ;

                        ExportarConsultaSQLparaXLS(ds, sNomeArquivo, sTituloRelatorio);
                    }

                    return;
                }

                if (Request["id"] != null)
                {
                    hddidRelatorio.Value = Request["id"];
                    PesquisarRelatorio(hddidRelatorio.Value);
                }
                else
                    Pesquisar();

                FUNCOES.Scripts.FocusScript(Page, "cphCorpo_Referencia_MesAno_ddlMes");
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];

                if (requestTarget == "funcao_SAIR")
                    FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                else if (requestTarget == "funcao_SALVAR")
                {
                    _sConfirma = "S";
                    GerarRelatorio();
                }
            }           

            RegistraScript();
        }

        protected void Pesquisar()
        {
            AlternarVisualizacao(false, false, false, false);
            lnkExportar.Visible = false;
            lblTituloPagina.Text = "Apuração VT e VR";
            hddidRelatorio.Value = "0";

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR-CONTROLE" },
                { "@dtReferencia", Referencia_MesAno.RetornaData().ToString() },
                { "@idEmpresa", ddlidEmpresa.SelectedValue },
                { "@sTipoContrato", string.Join("|", lstTipoContrato.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value)) }
            };
            DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(ds))
            {                
                AlternarVisualizacao(true, true, false, false);

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables_ConsultaResumo", Grid.DataBindComScriptData(dtgConsultaResumo, ds.Tables[0], 0, new int[1] { 5 }, "desc", "false", "''"), true);
            }
            else
                MensagemPagina.MostraMensagem_Erro("Nenhum Relatório encontrado!");
        }

        protected void PesquisarRelatorio(string idRegistro)
        {
            hddidColaborador.Value = "0";
            AlternarVisualizacao(false, false, false, false);
            lnkExportar.Visible = true;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR-RELATORIO" },
                { "@idRegistro", idRegistro }
            };
            DataSet dsResumo = BD.ExecutarDataSet(sProcedure, vParametros, false);

            if (BD.ValidarDataSet(dsResumo))
            {                
                string sMes_Ano = "";

                if (DateTime.TryParse(RETORNO.DATASET(dsResumo, "dtReferencia"), out DateTime dt))
                {
                    sMes_Ano = dt.ToString("MMMM/yyyy", new System.Globalization.CultureInfo("pt-BR"));
                    lblTituloPagina.Text = $"Relatório de {sMes_Ano}";
                }

                PainelAtualizacao.Visible = true;
                PainelAtualizacao.Atualizar(RETORNO.DATASET(dsResumo, 0, "dtAtualizacao"), RETORNO.DATASET(dsResumo, 0, "sDscUsuarioAtualizacao"));              

                AlternarVisualizacao(true, false, true, false);

                Session["RelatorioExcel"] = idRegistro;
                Session["TituloRelatorioExcel"] = $"Relatório VT e VR de {sMes_Ano}";

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables_ConsultaRelatorio", Grid.DataBindComScriptData(dtgRelatorioSalvo, dsResumo.Tables[0], 1, new int[1] { 2 }, "asc", "false", "''"), true);
                Grid.SomarColunas(dtgRelatorioSalvo, true, Grid.Formatação.Moeda, 9, 10);

                if (_sConfirma == "S")
                    MensagemPagina.MostraMensagem_Sucesso("Relatório salvo com sucesso!");
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhum Relatório encontrado!");                
            }
        }

        private void AlternarVisualizacao(bool bPanelGrid, bool bResumo, bool bConsulta, bool bNovo)
        {
            pnResultado.Visible = bPanelGrid;
            resultadoResumo.Visible = bResumo;
            resultadoSalvo.Visible = bConsulta;
            resultado.Visible = bNovo;
        }

        #endregion

        #region | Gerar Relatório

        protected void GerarRelatorio()
        {
            AlternarVisualizacao(false, false, false, false);
            lnkExportar.Visible = false;

            if (ValidarDados())
            {
                lblTituloPagina.Text = "Novo Relatório - ";

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "GERAR-CONTROLE" },
                    { "@dtReferencia", Referencia_MesAno.RetornaData().ToString() },
                    { "@idFiltro", IDENTITY.Variaveis.idUsuario() },
                    { "@sConfirma", _sConfirma },
                    { "@sTipoContrato",  hddsTipoContrato.Value},
                    { "@idEmpresa", hddidEmpresa.Value }

                };
                DataTable tb = BD.ExecutarDataTable(sProcedure, vParametros, false);

                if (tb.Rows.Count > 0)
                {
                    AlternarVisualizacao(true, false, false, true);

                    string mes_ano = Referencia_MesAno.RetornaData().Value.ToString("MMMM/yyyy");
                    lblTituloPagina.Text += mes_ano;
                    lblCabecalho_Novo.Text = $"Dados para Novo Relatório - {mes_ano}";

                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables_GeraRelatorio", Grid.DataBindComScript(dtgvConsulta, tb), true);
                    Grid.SomarColunas(dtgvConsulta, true, Grid.Formatação.Moeda, 9, 10);

                    if (_sConfirma == "S")
                    {
                        MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!");
                        Referencia_MesAno.LimparCampos(true, false);
                        ddlidEmpresa.SelectedValue = "0";
                        lstTipoContrato.ClearSelection();
                        Pesquisar();
                    }
                }
                else // VALIDAR
                {
                    if (hddidRelatorio.Value != null && string.IsNullOrEmpty(hddidRelatorio.Value))
                    {                        
                        MensagemPagina.MostraMensagem_Erro("Nenhum Lançamento encontrado!");
                    }
                    else if (hddidRelatorio.Value == null || hddidRelatorio.Value != null)
                        MensagemPagina.MostraMensagem_Erro("Um Relatório para este Tipo Contrato neste Mês/Ano de Referência já existe. Só é possível excluí-lo para então criar um Novo para substituí-lo!");
                }
            }
            
        }

        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (string.IsNullOrEmpty(Referencia_MesAno.RetornaData().ToString()))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "É necessário selecionar o Mês/Ano de referência!";
            }

            if (ddlidEmpresa.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa";
            }

            if (!lstTipoContrato.Items.Cast<ListItem>().Any(c => c.Selected))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "É obrigatório selecionar ao menos um Tipo de Contrato";
            }           

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        public static void ExportarConsultaSQLparaXLS(DataSet ds, string sNomeArquivoSemExten, string sTituloRelatorio)
        {
            string sNomeArquivoComExtensao = sNomeArquivoSemExten + FUNCOES.CarimboDataHora() + ".xls";
            var server = HttpContext.Current.Server;
            var response = HttpContext.Current.Response;

            string logoBase64 = "";
            string caminhoLogo = server.MapPath("~/img/LogoTT.png");
            if (File.Exists(caminhoLogo))
            {
                byte[] imageBytes = File.ReadAllBytes(caminhoLogo);
                string base64String = Convert.ToBase64String(imageBytes);
                logoBase64 = "data:image/png;base64," + base64String;
            }

            response.BufferOutput = true;
            response.Clear();
            response.ClearHeaders();
            response.AddHeader("content-disposition", "attachment; filename=" + sNomeArquivoComExtensao);
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.ContentType = "application/vnd.ms-excel";
            response.ContentEncoding = System.Text.Encoding.UTF8;

            StringBuilder lSbExcel = new StringBuilder();

            lSbExcel.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\r\n");
            lSbExcel.Append("<style type=\"text/css\">\r\n");
            lSbExcel.Append("body { font-family: Arial, Helvetica, sans-serif; color: #333; }\r\n");
            lSbExcel.Append(".report-table { border-collapse: collapse; width: 100%; font-size: 12px; }\r\n");

            // --- ESTILO AQUI ---
            lSbExcel.Append(".report-table th, .report-table td { font-family: Arial, Helvetica, sans-serif; border: 1px solid #999999; padding: 12px 15px; text-align: left; vertical-align: middle; }\r\n");

            lSbExcel.Append(".report-table th { background-color: #009a22; color: #ffffff; font-size: 13px; font-weight: bold; text-transform: uppercase; }\r\n");
            lSbExcel.Append(".report-table tr.alt-row td { background-color: #f2f2f2; }\r\n");
            lSbExcel.Append(".header-container { text-align: center; margin-bottom: 25px; }\r\n");
            lSbExcel.Append(".logo { max-height: 60px; margin-bottom: 15px; }\r\n");
            lSbExcel.Append(".report-title { font-family: Arial, Helvetica, sans-serif; color: #024e0a; font-size: 24px; font-weight: bold; margin: 0; }\r\n");
            lSbExcel.Append(".report-subtitle { font-family: Arial, Helvetica, sans-serif; font-size: 14px; text-align: center; color: #666; margin-top: 5px; }\r\n");
            lSbExcel.Append("hr.separator { border: 0; height: 2px; background-color: #009a22; margin-top: 25px; }\r\n");
            lSbExcel.Append("</style>\r\n\r\n");

            // --- Montagem do Cabeçalho ---
            lSbExcel.Append("<div class='header-container'>");
            if (!string.IsNullOrEmpty(logoBase64))
            {
                lSbExcel.AppendFormat("<img src='{0}' class='logo' />", logoBase64);
            }
            lSbExcel.Append($"<div class='report-title'>{sTituloRelatorio}</div>");
            lSbExcel.AppendFormat("<div class='report-subtitle'>Gerado em: {0}</div>", DateTime.Now.ToString("dd/MM/yyyy 'às' HH:mm:ss"));
            lSbExcel.Append("</div>");
            lSbExcel.Append("<hr class='separator' />");

            // --- Montagem da Tabela de Dados ---
            lSbExcel.Append("<table class=\"report-table\">\r\n");
            lSbExcel.Append("<thead>\r\n");
            lSbExcel.Append("<tr>\r\n");
            foreach (DataColumn Coluna in ds.Tables[0].Columns)
            {
                lSbExcel.AppendFormat("\t<th>{0}</th>\r\n", Coluna.ColumnName);
            }
            lSbExcel.Append("</tr>\r\n");
            lSbExcel.Append("</thead>\r\n");
            lSbExcel.Append("<tbody>\r\n");

            int rowIndex = 0;
            foreach (DataRow Linha in ds.Tables[0].Rows)
            {
                string rowClass = (rowIndex % 2 != 0) ? "class='alt-row'" : "";
                lSbExcel.AppendFormat("<tr {0}>\r\n", rowClass);
                foreach (DataColumn Coluna in ds.Tables[0].Columns)
                {
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(Linha[Coluna].ToString().Trim()));
                }
                lSbExcel.Append("</tr>\r\n");
                rowIndex++;
            }

            lSbExcel.Append("</tbody>\r\n");
            lSbExcel.Append("</table>\r\n");

            // --- Finalização da Resposta ---
            response.Write(lSbExcel.ToString());
            response.Flush();
            response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        #endregion

        #region | Eventos

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void cmdVoltar_Click(object sender, EventArgs e) { Referencia_MesAno.DefinirData(DateTime.Today); Pesquisar(); }

        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            if (ValidarDados())
            {
                _sConfirma = "N";
                hddidEmpresa.Value = ddlidEmpresa.SelectedValue;
                hddsTipoContrato.Value = string.Join("|", lstTipoContrato.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value));
                GerarRelatorio();
            }
           
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e) { if (e.Row.RowType == DataControlRowType.DataRow) e.Row.Attributes.Add("data-id", "0"); }

        protected void dtgRelatorioSalvo_RowDataBound(object sender, GridViewRowEventArgs e) { if (e.Row.RowType == DataControlRowType.DataRow) e.Row.Attributes.Add("data-id", dtgRelatorioSalvo.DataKeys[e.Row.RowIndex]["idRelatorio"].ToString()); }

        protected void dtgConsultaResumo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "EXCLUIR-CONTROLE" },
                { "@idRegistroResumo", e.CommandName }
            };
            DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(ds))
            {
                MensagemPagina.MostraMensagem_Sucesso("Relatório excluído com sucesso!");
                Pesquisar();
            }
            else
                MensagemPagina.MostraMensagem_Sucesso("Não foi possível excluir o Relatório!");
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            AlternarVisualizacao(true, true, false, false);
            Pesquisar();
        }

        #endregion

        #region | Script 

        void RegistraScript()
        {
            FUNCOES.Scripts.Aplica_TooltipPersonalizado(Page);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("");
            sb.AppendLine("     function MudarBotao() {");
            sb.AppendLine("         $('#cphCorpo_cmdPesquisar').val('Pesquisando...');");
            sb.AppendLine("     }");
            sb.AppendLine("");
            sb.AppendLine("     $(document).off('click', '.linkModal a').on('click', '.linkModal a', function (e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         const $this = $(this);");
            sb.AppendLine("         if ($this.text() == 0) return;");
            sb.AppendLine("         const $link = $this.closest('.linkModal');");
            sb.AppendLine("         const row = $this.closest('tr');");
            sb.AppendLine("         const id = row.attr('data-id') || '0';");
            sb.AppendLine("         const idColaborador = row.find('.id a').text() || '0';");
            sb.AppendLine("         const sTipo = row.closest('table').attr('id') == 'dtgvConsulta' ? '1' : '2';");
            sb.AppendLine("         const sidTipo = $link.hasClass('adicionais') ? '2' : $link.hasClass('ausencias') ? '1' : '0';");
            sb.AppendLine("         const sDsc = row.find('.sDsc a').text() || '';");
            sb.AppendLine("         if (sDsc) $('#mdoalDias').find('.pai').html(`Colaborador - <u>${sDsc}</u>`);");
            sb.AppendLine("         else $('#mdoalDias').find('.pai').html('');");
            sb.AppendLine("         if (sidTipo == '1') $('#mdoalDias').find('.modal-title').html(`Ausências`);");
            sb.AppendLine("         else if (sidTipo == '2') $('#mdoalDias').find('.modal-title').html(`Adicionais`);");
            sb.AppendLine("         $.ajax({");
            sb.AppendLine("             url: '/App/Paginas/RRHH/RelatorioBeneficios_Detalhe.aspx/Get_PopulaModal_Dias',");
            sb.AppendLine("             data: JSON.stringify({ id: id, idColaborador: idColaborador, sidTipo: sidTipo }),");
            sb.AppendLine("             type: 'POST',");
            sb.AppendLine("             dataType: 'json',");
            sb.AppendLine("             contentType: 'application/json; charset=utf-8',");
            sb.AppendLine("             success: function (response) {");
            sb.AppendLine("                 if (response && response.d) {");
            sb.AppendLine("                     let resposta = response.d;");
            sb.AppendLine("                     if (resposta.relatorio) { $('.relatorio').removeClass('invisivel'); $('#relatorio').text(resposta.relatorio); }");
            sb.AppendLine("                     else $('.relatorio').addClass('invisivel');");
            sb.AppendLine("                     $('.tabela').html(resposta.tabela);");
            sb.AppendLine("                     $('#mdoalDias').modal('show');");
            sb.AppendLine("                 }");
            sb.AppendLine("             }");
            sb.AppendLine("         });");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("});");
            sb.AppendLine("");
            sb.AppendLine("$v192(function() {");
            sb.AppendLine("");
            sb.AppendLine("     $v192('#dialog-Salvar').dialog({");
            sb.AppendLine("         resizable: false,");
            sb.AppendLine("         height: 'auto',");
            sb.AppendLine("         width: 400,");
            sb.AppendLine("         modal: true,");
            sb.AppendLine("         autoOpen: false,");
            sb.AppendLine("         buttons: {");
            sb.AppendLine("             'Sim': function() {");
            sb.AppendLine("                 __doPostBack('funcao_SALVAR', '');");
            sb.AppendLine("                 $v192(this).dialog('close');");
            sb.AppendLine("             },");
            sb.AppendLine("             'Não': function() {");
            sb.AppendLine("                 $v192(this).dialog('close');");
            sb.AppendLine("             },");
            sb.AppendLine("         }");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $v192('[id*=cmdSalvar]').click(function(e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         $v192('#dialog-Salvar').dialog('open');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }

        #endregion

        #region | WebMethods

        [WebMethod]
        public static object Get_PopulaModal_Dias(string id, string idColaborador, string sidTipo)
        {
            int.TryParse(sidTipo, out int idTipo);
            string sFuncao = string.Empty;

            switch (idTipo)
            {
                case 1: // Ausências
                    sFuncao = "CONSULTAR_AUSENCIAS";
                    break;

                case 2: // Adicionais
                    sFuncao = "CONSULTAR_ADICIONAIS";
                    break;
            }

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", sFuncao },
                { "@idRelatorio", id },
                { "@idColaborador", idColaborador }
            };
            DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);
            DataTable tb = ds.Tables[0];

            string relatorio = string.Empty;
            try { relatorio = DateTime.Parse(ds.Tables[1].Rows[0]["dtReferencia"].ToString()).ToString("Y"); } catch { }

            string filhas = string.Empty;

            foreach (DataRow row in tb.Rows)
            {
                if (idTipo == 1)
                {
                    DateTime.TryParse(row["dtInicioAtestado"].ToString(), out DateTime dtInicio);
                    DateTime.TryParse(row["dtRetornoAtestado"].ToString(), out DateTime dtRetorno);

                    filhas += $@"<tr>
                                    <td style='text-align: left;'>{row["sDscAtestado"]}</td>
                                    <td style='text-align: left;'>{row["sObservacaoAtestado"]}</td>
                                    <td style='text-align: center;'>{dtInicio.ToString("dd/MM/yyyy HH:mm:ss")}</td>
                                    <td style='text-align: center;'>{dtRetorno.ToString("dd/MM/yyyy HH:mm:ss")}</td>
                                    <td style='text-align: center;'>{(row["sDescontoVT"].ToString().Trim().ToUpper() == "S" ? "✔" : string.Empty)}</td>
                                    <td style='text-align: center;'>{(row["sDescontoVR"].ToString().Trim().ToUpper() == "S" ? "✔" : string.Empty)}</td>
                                </tr>";
                }
                else if (idTipo == 2)
                {
                    DateTime.TryParse(row["dtInicioCredito"].ToString(), out DateTime dtInicio);
                    DateTime.TryParse(row["dtRetornoCredito"].ToString(), out DateTime dtRetorno);

                    filhas += $@"<tr>
                                    <td style='text-align: left;'>{row["sDscJustificativa"]}</td>
                                    <td style='text-align: left;'>{row["sObservacaoCredito"]}</td>
                                    <td style='text-align: center;'>{dtInicio.ToString("dd/MM/yyyy HH:mm:ss")}</td>
                                    <td style='text-align: center;'>{dtRetorno.ToString("dd/MM/yyyy HH:mm:ss")}</td>
                                    <td style='text-align: center;'>{(row["sCreditaVT"].ToString().Trim().ToUpper() == "S" ? "✔" : string.Empty)}</td>
                                    <td style='text-align: center;'>{(row["sCreditaVR"].ToString().Trim().ToUpper() == "S" ? "✔" : string.Empty)}</td>
                                </tr>";
                }
            }

            string tabela = $@"
                <table class='table' style='margin: 0;'>
                    <thead>
                        <tr>
                            <th style='width: 20%; text-align: left;'>Título</th>
                            <th style='width: 30%; text-align: left;'>Observação</th>
                            <th style='width: 15%; text-align: center;'>Data de Início</th>
                            <th style='width: 15%; text-align: center;'>Data de Retorno</th>
                            <th style='width: 10%; text-align: center;'>Desconta VT?</th>
                            <th style='width: 10%; text-align: center;'>Desconta VR?</th>
                        </tr>
                    </thead>
                    <tbody>
                        {filhas}
                    </tbody>
                </table>";

            return new { tabela, relatorio };
        }

        #endregion

        
    }
}