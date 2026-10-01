using NPOI.SS.Formula.Functions;
using OfficeOpenXml.FormulaParsing;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;

namespace TT_Hub.App.Paginas.RRHH
{
    public partial class GHE : Page
    {
        string sTituloPagina = "GHE";
        string sCaminho = "App/Paginas/RRHH/";
        string sPagina = "GHE_Detalhe.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.RRHH.GHE.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.GHE.Incluir);

            if (Request.QueryString["action"] == "export")
            {
                if (Session["RelatorioGHE_DS"] != null)
                {
                    DataSet dsParaExportar = (DataSet)Session["RelatorioGHE_DS"];

                    ExportarConsultaSQLparaXLS(dsParaExportar, "Relatorio_GHE_Colaboradores");
                }
                // Impede o resto da página de carregar
                return;
            }

            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlidPlano, "sp_Manipula_tbl_Flow_Colaboradores_GHE 'SELECT-PLANOS'", "idPlano", "sDscPlano", false, "Todos os Planos", "0");
                FUNCOES.Popula_Combo(ddlsidSetor, "sp_Manipula_tbl_Flow_Colaboradores_Setor 'SELECT_SETOR'", "idSetor", "sDscSetor", false, "Todos os Setores", "0");
                FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Todas as Empresas", "0");

                Pesquisar();

                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
            }
            FUNCOES.Scripts.FocusScript(Page, txtsPesquisa.ClientID);
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;
            pnMensagem.Visible = false;
            try
            {

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR" },
                    { "@sDscGHE", txtsPesquisa.Text.Trim() },
                    { "@sidEmpresa", ddlidEmpresa.SelectedValue},
                    { "@sidSetor", ddlsidSetor.SelectedValue},
                    { "@idPlano", ddlidPlano.SelectedValue} // NOVO PARÂMETRO ADICIONADO
                };
                DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_GHE", vParametros);

                if (tb.Rows.Count > 0)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 1, "asc", "false", "''"), true);
                    pnResultado.Visible = true;
                }
                else
                {
                    pnMensagem.Visible = true;
                    MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
                }

            }
            catch (Exception ex)
            {

                pnMensagem.Visible = true;
                MensagemPagina.MostraMensagem_Erro("Erro ao Carregador Dados: " + ex.Message);
            }
        }
        protected void dtgvConsulta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //if (e.CommandName == "Resultados")
            //{
            //    FUNCOES.DirecionaPagina(string.Format("{0}{1}?id={2}&sTipo=Resultados", sCaminho, sPagina, e.CommandArgument));
            //}
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        =>
            Pesquisar();

        protected void cmdNovo_Click(object sender, EventArgs e)
        =>
            FUNCOES.DirecionaPagina(string.Format("{0}{1}?id={2}&sTipo=Novo", sCaminho, sPagina, "0"));

        protected void cmdRelatorioColaborador_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
        {
            { "@sFuncao", "CONSULTAR_GHE_COLABORADOR" },
            { "@sDscGHE", txtsPesquisa.Text.Trim() },
            { "@sidEmpresa", ddlidEmpresa.SelectedValue},
            { "@sidSetor", ddlsidSetor.SelectedValue},
            { "@idPlano", ddlidPlano.SelectedValue} // NOVO PARÂMETRO ADICIONADO
        };
                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_GHE", vParametros);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Session["RelatorioGHE_DS"] = ds;

                    gvRelatorio.DataSource = ds.Tables[0];
                    gvRelatorio.DataBind();
                    pbRelatorio.Visible = true; 

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "AbrirModalRelatorio", "$('#modalRelatorio').modal('show');", true);
                }
                else
                {
                    pnMensagem.Visible = true;
                    MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para o relatório, para gerar é necessário pelo menos 1 colaborador com uma função ligada ao GHE!");
                }
            }
            catch (Exception ex)
            {
                pnMensagem.Visible = true;
                MensagemPagina.MostraMensagem_Erro("Erro ao gerar relatório: " + ex.Message);
            }
        }

        protected void cmdExportarModal_Click(object sender, EventArgs e)
        {
            if (Session["RelatorioGHE_DS"] != null)
            {
                DataSet dsParaExportar = (DataSet)Session["RelatorioGHE_DS"];

                ExportarConsultaSQLparaXLS(dsParaExportar, "Relatorio_GHE_Colaboradores");
            }
        }



        public static void ExportarConsultaSQLparaXLS(DataSet dsExportar, string pStrNomeArquivoSemExten)
        {
            string lStrNomeArquivoComExtensao = pStrNomeArquivoSemExten + FUNCOES.CarimboDataHora() + ".xls";
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
            response.AddHeader("content-disposition", "attachment; filename=" + lStrNomeArquivoComExtensao);
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
            lSbExcel.Append("<div class='report-title'>Relatório de Colaboradores por GHE</div>");
            lSbExcel.AppendFormat("<div class='report-subtitle'>Gerado em: {0}</div>", DateTime.Now.ToString("dd/MM/yyyy 'às' HH:mm:ss"));
            lSbExcel.Append("</div>");
            lSbExcel.Append("<hr class='separator' />");

            // --- Montagem da Tabela de Dados ---
            lSbExcel.Append("<table class=\"report-table\">\r\n");
            lSbExcel.Append("<thead>\r\n");
            lSbExcel.Append("<tr>\r\n");
            foreach (DataColumn Coluna in dsExportar.Tables[0].Columns)
            {
                lSbExcel.AppendFormat("\t<th>{0}</th>\r\n", Coluna.ColumnName);
            }
            lSbExcel.Append("</tr>\r\n");
            lSbExcel.Append("</thead>\r\n");
            lSbExcel.Append("<tbody>\r\n");

            int rowIndex = 0;
            foreach (DataRow Linha in dsExportar.Tables[0].Rows)
            {
                string rowClass = (rowIndex % 2 != 0) ? "class='alt-row'" : "";
                lSbExcel.AppendFormat("<tr {0}>\r\n", rowClass);
                foreach (DataColumn Coluna in dsExportar.Tables[0].Columns)
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

        public override void VerifyRenderingInServerForm(Control control)
        { }

        protected void cmdExportarExcel_Click(object sender, EventArgs e)
        {
            string snomearquivo = "GHE_Colaboradores_" + FUNCOES.CarimboDataHora() + ".xls";

            Exportacao.ExportarGRID_Excel(gvRelatorio, "teste01");
            ExcelApp.ExportarArquivoExcel(gvRelatorio, snomearquivo, Response, Server.MapPath("~/Download/" + snomearquivo), Page);

        }
    }
}
