using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;

using Funcoes = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.Adm.Financeiro
{
    public partial class RelatorioFinanceiro_Parceiros : System.Web.UI.Page
    {
        string sTituloPagina = "Relatório Financeiro";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_Contas_Pagar";

        public List<cls_Relatorio_Financeiro> bs_Relatorio_Financeiro
        {
            get
            {
                if (ViewState["bs_Relatorio_Financeiro"] == null)
                {
                    ViewState["bs_Relatorio_Financeiro"] = new List<cls_Relatorio_Financeiro>();
                }
                return (List<cls_Relatorio_Financeiro>)ViewState["bs_Relatorio_Financeiro"];
            }
            set
            {
                ViewState["bs_Relatorio_Financeiro"] = value;
            }
        }
        
        public List<cls_Relatorio_Financeiro_Detalhe> bs_Relatorio_Financeiro_Detalhe
        {
            get
            {
                if (ViewState["bs_Relatorio_Financeiro_Detalhe"] == null)
                {
                    ViewState["bs_Relatorio_Financeiro_Detalhe"] = new List<cls_Relatorio_Financeiro_Detalhe>();
                }
                return (List<cls_Relatorio_Financeiro_Detalhe>)ViewState["bs_Relatorio_Financeiro_Detalhe"];
            }
            set
            {
                ViewState["bs_Relatorio_Financeiro_Detalhe"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Funcoes.ValidaPermissao(Permissao.Financeiro.RelatorioFinanceiro.Consultar, true);

                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                div_gvConsulta.Visible = false;

                object objSender = new object();
                EventArgs objEventArgs = new EventArgs();
                ddlsLocal_SelectedIndexChanged(objSender, objEventArgs);

                DateTime dtInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                DateTime dtFinal = DateTime.Today;

                txtdtInicio.Text = dtInicio.ToString("yyyy-MM-dd");
                txtdtFinal.Text = dtFinal.ToString("yyyy-MM-dd");
                Funcoes.Popula_Combo(lstEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + Identity.Variaveis.idUsuario(), "idEmpresa", "sDscCodigoEmpresa", false);

            }

            if (bs_Relatorio_Financeiro.Count > 0)
            {
                hlExcelRelatorio.Visible = true;
            }

            if (Request.QueryString["action"] == "export")
            {
                if (Session["Relatorio_Financeiro"] != null)
                {
                    cls_Relatorio_Financeiro relatorio = (cls_Relatorio_Financeiro)Session["Relatorio_Financeiro"];
                    ExportarRelatorioSeguroParaXLS(relatorio, "Relatorio_Financeiro_");

                    MensagemPagina.MostraMensagem_Sucesso("Relatório Gerado com sucesso!");
                }

                return;
            }
        }

        private void Pesquisar()
        {
            if ((txtdtInicio.Text == "" && txtdtFinal.Text != "") || (txtdtInicio.Text != "" && txtdtFinal.Text == ""))
            {
                MensagemPagina.MostraMensagem_Erro("Necessário informar uma Data Início e uma Data Final");
            }
            else
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "Consultar_Relatorio_Financeiro" },
                    { "@dtInicio", txtdtInicio.Text },
                    { "@dtFinal", txtdtFinal.Text },
                    { "@sLocal", ddlsLocal.SelectedValue },
                    { "@sTipo", ddlsTipo.SelectedValue },
                    { "@sStatus", ddlsStatus.SelectedValue },
                    { "@idParceiro", ddlidParceiro.SelectedValue },
                    { "@sAgrupar", cbAgruparCNPJ.Checked ? "S" : "N" },
                    { "@sFiltro", cbItemFinanceiro.Checked ? "S" : "N" },
                    { "@sEmpresa", string.Join("|", lstEmpresa.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value)) }
                };
                DataTable tb = BD.ExecutarDataTable(sProcedure, vParametros, false);

                if (tb.Rows.Count > 0)
                {
                    PopularClasse(tb);
                    div_gvConsulta.Visible = true;
                    hlExcelRelatorio.Visible = true;
                    string sStatus = ddlsStatus.SelectedValue == "" ? "Todos os Status" : ddlsStatus.SelectedItem.ToString();
                    string dtInicio = txtdtInicio.Text == "" ? "" : DateTime.Parse(txtdtInicio.Text).ToString("dd/MM/yyyy");
                    string dtFinal = txtdtFinal.Text == "" ? "" : DateTime.Parse(txtdtFinal.Text).ToString("dd/MM/yyyy");
                    string sPeriodo = dtInicio == "" || dtFinal == "" ? "" : " - Período " + dtInicio + " a " + dtFinal;
                    lblTituloTabela.Text = "Títulos do " + ddlsLocal.SelectedItem + " - " + sStatus + sPeriodo; 

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, bs_Relatorio_Financeiro_Detalhe, 0, "asc"), true);
                    Grid.SomarColunas(dtgvConsulta, true, Grid.Formatação.Moeda, 3);
                    Grid.SomarColunas(dtgvConsulta, true, Grid.Formatação.Inteiro, 4);
               
                }
                else
                {
                    div_gvConsulta.Visible = false;
                    MensagemPagina.MostraMensagem_Erro("Nenhum registro encontrado");
                }
            }
        }

        private void PopularClasse(DataTable dt)
        {
            bs_Relatorio_Financeiro.Clear();
            bs_Relatorio_Financeiro_Detalhe.Clear();
            //int idLinha = 0;

            cls_Relatorio_Financeiro objFiltro = new cls_Relatorio_Financeiro();
            objFiltro.dtInicio = txtdtInicio.Text;
            objFiltro.dtFinal = txtdtFinal.Text;
            objFiltro.sLocal = ddlsLocal.SelectedValue;
            objFiltro.sTipo = ddlsTipo.SelectedValue;
            objFiltro.sStatus = ddlsStatus.SelectedValue;
            objFiltro.idParceiro = ddlidParceiro.SelectedValue;
            objFiltro.sAgruparCNPJ = cbAgruparCNPJ.Checked ? "S" : "N";
            objFiltro.sFiltroItemFinanceiro = cbItemFinanceiro.Checked ? "S" : "N";
            objFiltro.sEmpresa = string.Join("|", lstEmpresa.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value));

            bs_Relatorio_Financeiro.Add(objFiltro);

            foreach (DataRow row in dt.Rows)
            {
                cls_Relatorio_Financeiro_Detalhe objItem = new cls_Relatorio_Financeiro_Detalhe();
                objItem.idLinha = Convert.ToInt32(row["nOrdem"]);
                objItem.sCNPJ = row["sCNPJ"].ToString();
                objItem.sRazaoSocial = row["sRazaoSocial"].ToString();
                objItem.nValorTotal = Convert.ToDecimal(row["nValorTotal"]);
                objItem.nQtdTitulos = Convert.ToInt32(row["nQtdTitulos"]);
                objItem.idParceiro = row["idParceiro"].ToString();

                bs_Relatorio_Financeiro_Detalhe.Add(objItem);
            }

            objFiltro.lsFinanceiroDetalhe = bs_Relatorio_Financeiro_Detalhe;

            Session["Relatorio_Financeiro"] = bs_Relatorio_Financeiro.FirstOrDefault();
        }       

        protected void ddlsLocal_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sAgrupar = cbAgruparCNPJ.Checked ? "S" : "N";
            Funcoes.Popula_Combo(ddlidParceiro, $"sp_Manipula_tbl_Flow_Adm_Contas_Pagar 'Consulta_Parceiro_Relatorio', @sLocal={ddlsLocal.SelectedValue}, @sAgrupar={sAgrupar}", "idParceiro", "sRazaoSocial", false, "Todos os Parceiros", "0");

            if (bs_Relatorio_Financeiro.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, bs_Relatorio_Financeiro_Detalhe, 0, "asc"), true);
                Grid.SomarColunas(dtgvConsulta, true, Grid.Formatação.Moeda, 3);
                Grid.SomarColunas(dtgvConsulta, true, Grid.Formatação.Inteiro, 4);
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
            Pesquisar();
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow) 
            {
                int idLinha = Convert.ToInt32(dtgvConsulta.DataKeys[e.Row.RowIndex].Value);

                HyperLink hlRazaoSocial = (HyperLink)e.Row.FindControl("hlRazaoSocial");
                if (hlRazaoSocial != null)
                {
                    var relatorio_detalhe = bs_Relatorio_Financeiro_Detalhe.FirstOrDefault(x => x.idLinha == idLinha);
                    var relatorio_filtro = bs_Relatorio_Financeiro.FirstOrDefault();

                    string url;
                    if (relatorio_filtro.sLocal == "Receber")
                    {
                        url = string.Format("/App/Paginas/Adm/Financeiro/ContasReceber.aspx?dashboard={0}&sTipo={1}&dtInicio={2}&dtFinal={3}&idParceiro={4}&sAgrupar={5}&sStatus={6}&sFiltroFinanceiro={7}&sEmpresaRelatorio={8}", "relatorioFinanceiro", relatorio_filtro.sTipo, relatorio_filtro.dtInicio, relatorio_filtro.dtFinal, relatorio_detalhe.idParceiro, relatorio_filtro.sAgruparCNPJ, relatorio_filtro.sStatus, relatorio_filtro.sFiltroItemFinanceiro, relatorio_filtro.sEmpresa);
                    }
                    else
                    {
                        url = string.Format("/App/Paginas/Adm/Financeiro/ContasPagar.aspx?dashboard={0}&sTipo={1}&dtInicio={2}&dtFinal={3}&idParceiro={4}&sAgrupar={5}&sStatus={6}&sFiltroFinanceiro={7}&sEmpresaRelatorio={8}", "relatorioFinanceiro", relatorio_filtro.sTipo, relatorio_filtro.dtInicio, relatorio_filtro.dtFinal, relatorio_detalhe.idParceiro, relatorio_filtro.sAgruparCNPJ, relatorio_filtro.sStatus, relatorio_filtro.sFiltroItemFinanceiro, relatorio_filtro.sEmpresa);
                    }

                    hlRazaoSocial.NavigateUrl = ResolveUrl(url);
                }               
            }
        }

        public static void ExportarRelatorioSeguroParaXLS(cls_Relatorio_Financeiro relatorio, string sNomeArquivoSemExten)
        {
            string sNomeArquivoComExtensao = sNomeArquivoSemExten + Funcoes.CarimboDataHora() + ".xls";
            var server = HttpContext.Current.Server;
            var response = HttpContext.Current.Response;


            // Carrega logo
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
            response.ClearContent();
            response.AddHeader("content-disposition", "attachment; filename=" + sNomeArquivoComExtensao);
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.ContentType = "application/vnd.ms-excel";
            response.ContentEncoding = System.Text.Encoding.UTF8;

            StringBuilder lSbExcel = new StringBuilder();

            // --- Estilos ---
            lSbExcel.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\r\n");
            lSbExcel.Append("<style type=\"text/css\">\r\n");
            lSbExcel.Append("body { font-family: Arial, Helvetica, sans-serif; color: #333; }\r\n");
            lSbExcel.Append(".report-table { border-collapse: collapse; width: 100%; font-size: 12px; }\r\n");
            lSbExcel.Append(".report-table th, .report-table td { font-family: Arial, Helvetica, sans-serif; border: 1px solid #999999; padding: 12px 15px; text-align: left; vertical-align: middle; }\r\n");
            lSbExcel.Append(".report-table th { background-color: #009a22; color: #ffffff; font-size: 13px; font-weight: bold; text-transform: uppercase; }\r\n");
            lSbExcel.Append(".report-table tr.alt-row td { background-color: #f2f2f2; }\r\n");
            lSbExcel.Append(".header-container { text-align: center; margin-bottom: 25px; }\r\n");
            lSbExcel.Append(".logo { max-height: 60px; margin-bottom: 15px; }\r\n");
            lSbExcel.Append(".report-title { font-family: Arial, Helvetica, sans-serif; color: #024e0a; font-size: 24px; font-weight: bold; margin: 0; }\r\n");
            lSbExcel.Append(".report-subtitle { font-family: Arial, Helvetica, sans-serif; font-size: 14px; text-align: center; color: #666; margin-top: 5px; }\r\n");
            lSbExcel.Append(".info-section { margin-bottom: 20px; }\r\n");
            lSbExcel.Append(".info-table { border-collapse: collapse; width: 100%; margin-bottom: 15px; }\r\n");
            lSbExcel.Append(".info-table td { padding: 6px 10px; border: 1px solid #ddd; font-size: 13px; }\r\n");
            lSbExcel.Append(".info-label { font-weight: bold; width: 25%; background-color: #f9f9f9; }\r\n");
            lSbExcel.Append("hr.separator { border: 0; height: 2px; background-color: #009a22; margin-top: 25px; }\r\n");
            lSbExcel.Append("</style>\r\n\r\n");

            // --- Cabeçalho ---
            lSbExcel.Append("<div class='header-container'>");
            if (!string.IsNullOrEmpty(logoBase64))
            {
                lSbExcel.AppendFormat("<img src='{0}' class='logo' />", logoBase64);
            }
            lSbExcel.Append("<div class='report-title'>Relatório Financeiro</div>");
            lSbExcel.AppendFormat("<div class='report-subtitle'>Gerado em: {0}</div>", DateTime.Now.ToString("dd/MM/yyyy 'às' HH:mm:ss"));
            lSbExcel.Append("</div>");
            lSbExcel.Append("<hr class='separator' />");

            // --- DADOS PRINCIPAIS ---
            lSbExcel.Append("<div class='report-subtitle'>Período: " + HttpUtility.HtmlEncode(DateTime.Parse(relatorio.dtInicio).ToString("dd/MM/yyyy")) + " a " + HttpUtility.HtmlEncode(DateTime.Parse(relatorio.dtFinal).ToString("dd/MM/yyyy")) + "</div>");
            lSbExcel.Append("<div class='report-subtitle'>Títulos do Contas a " + HttpUtility.HtmlEncode(relatorio.sLocal) + "</div>");

            lSbExcel.Append("<table style='width:100%; height:20px;'><tr><td></td></tr></table>");

            // --- TABELA DETALHADA ---
            if (relatorio.lsFinanceiroDetalhe != null && relatorio.lsFinanceiroDetalhe.Any())
            {
                lSbExcel.Append("<table class=\"report-table\">\r\n");
                lSbExcel.Append("<thead>\r\n");
                lSbExcel.Append("<tr>\r\n");

                lSbExcel.Append("\t<th>Ordem</th>\r\n");
                lSbExcel.Append("\t<th>CNPJ</th>\r\n");
                lSbExcel.Append("\t<th>Razão Social</th>\r\n");
                lSbExcel.Append("\t<th>Valor Total</th>\r\n");
                lSbExcel.Append("\t<th>Quantidade de Títulos</th>\r\n");

                lSbExcel.Append("</tr>\r\n");
                lSbExcel.Append("</thead>\r\n");
                lSbExcel.Append("<tbody>\r\n");

                var itensOrdenados = relatorio.lsFinanceiroDetalhe.OrderBy(x => x.idLinha);

                int rowIndex = 0;
                foreach (var item in itensOrdenados)
                {
                    string rowClass = (rowIndex % 2 != 0) ? "class='alt-row'" : "";
                    lSbExcel.AppendFormat("<tr {0}>\r\n", rowClass);

                    lSbExcel.AppendFormat("\t<td style='mso-number-format:\\@'>{0}</td>\r\n", HttpUtility.HtmlEncode(item.idLinha));
                    lSbExcel.AppendFormat("\t<td style='mso-number-format:\\@'>{0}</td>\r\n", HttpUtility.HtmlEncode(item.sCNPJ));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.sRazaoSocial));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.nValorTotal.ToString("N2")));
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(item.nQtdTitulos.ToString()));

                    lSbExcel.Append("</tr>\r\n");
                    rowIndex++;
                }

                lSbExcel.Append("</tbody>\r\n");
                lSbExcel.Append("</table>\r\n");
            }
            else
            {
                lSbExcel.Append("<p><em>Nenhum dado detalhado encontrado.</em></p>");
            }

            lSbExcel.Append("<table style='width:100%; height:20px;'><tr><td></td></tr></table>");           

            // --- Finalização (mantida) ---
            response.Write(lSbExcel.ToString());
            response.Flush();
            response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        protected void cbAgruparCNPJ_CheckedChanged(object sender, EventArgs e)
        {
            string sAgrupar = cbAgruparCNPJ.Checked ? "S" : "N";
            Funcoes.Popula_Combo(ddlidParceiro, $"sp_Manipula_tbl_Flow_Adm_Contas_Pagar 'Consulta_Parceiro_Relatorio', @sLocal={ddlsLocal.SelectedValue}, @sAgrupar={sAgrupar}", "idParceiro", "sRazaoSocial", false, "Todos os Parceiros", "0");

        }
    }

    [Serializable]
    public class cls_Relatorio_Financeiro
    {        
        public string dtInicio { get; set; }
        public string dtFinal { get; set; }
        public string idParceiro { get; set; }
        public string sStatus { get; set; }
        public string sTipo { get; set; }
        public string sLocal { get; set; }
        public string sAgruparCNPJ { get; set; }
        public string sFiltroItemFinanceiro { get; set; }
        public string sEmpresa { get; set; }
        public List<cls_Relatorio_Financeiro_Detalhe> lsFinanceiroDetalhe { get; set; }

    }

    [Serializable]
    public class cls_Relatorio_Financeiro_Detalhe
    {
        public int idLinha { get; set; }
        public string sCNPJ { get; set; }
        public string sRazaoSocial { get; set; }
        public decimal nValorTotal { get; set; }
        public int nQtdTitulos { get; set; }
        public string idParceiro { get; set; }
    }
}