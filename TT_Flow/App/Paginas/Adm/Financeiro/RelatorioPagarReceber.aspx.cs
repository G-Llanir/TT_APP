using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using Identity = TT.FrameWork.Identity;
using System.IO;
using Microsoft.Reporting.WebForms;

namespace TT_Flow.App.Paginas.Adm.Financeiro
{
    public partial class RelatorioPagarReceber : System.Web.UI.Page
    {
        string sTituloPagina = "Relatório";
        string sPagina_NovoRegistro = "app/Paginas/Adm/Financeiro/ContasPagar_Detalhe.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Financeiro.RelatorioPagarReceber.Consultar, true);

            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + Identity.Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                //Pesquisar("");
            }
            else
            {

            }
        }

        protected void Pesquisar(string DashBoard)
        {
            pnResultado.Visible = false;

            try
            {
                string sFuncao = "CONSULTAR";
                string sDscPesquisa = "";
                string sSql = "";

                DataTable tb;
                if (cblsPagarReceber.SelectedValue == "P")
                {
                    lblTituloPagina.Text = "Relatório Contas a Pagar";
                    sSql = "sp_Manipula_tbl_Flow_Adm_Contas_Pagar";
                }
                else if (cblsPagarReceber.SelectedValue == "R")
                {
                    lblTituloPagina.Text = "Relatório Contas a Receber";
                    sSql = "sp_Manipula_tbl_Flow_Adm_Contas_Receber";
                }
                else
                {
                    lblTituloPagina.Text = "Relatório";
                }

                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", sFuncao);

                if (DashBoard == "")
                {
                    string sPermissao = "N";
                    if (FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasPagar.VisualizarTudo))
                    {
                        sPermissao = "S";
                    }
                    else
                    {
                        sPermissao = "N";
                    }
                    string sUsuarioLogado = "";
                    sUsuarioLogado = HttpContext.Current.Session["idUsuario"].ToString();

                    vParametros.Add("@sUsuarioLogado", sUsuarioLogado);
                    vParametros.Add("@sPermissao", sPermissao);
                    vParametros.Add("@sStatus", "Liquidado");
                    vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                    vParametros.Add("@dtVencimento_Inicial", txtdtInicio.Text);
                    vParametros.Add("@dtVencimento_Final", txtdtFinal.Text);

                }
                tb = BD.ExecutarDataTable(sSql, vParametros, false);

                if (tb.Rows.Count > 0)
                {
                    if (cblsPagarReceber.SelectedValue == "P")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", GRID.DataBindComScriptData(dtgvConsulta, tb, 8, new int[2] { 7, 8 }, "desc", "false", "''"), true);
                        gvPagar.Visible = true;
                        gvReceber.Visible = false;
                        pnResultado.Visible = true;
                        GRID.SomarColunas(dtgvConsulta, true, GRID.Formatação.Moeda, 9);
                    }
                    else if (cblsPagarReceber.SelectedValue == "R")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", GRID.DataBindComScriptData(dtgvReceber, tb, 8, new int[2] { 7, 8 }, "desc", "false", "''"), true);
                        gvReceber.Visible = true;
                        gvPagar.Visible = false;
                        pnResultado.Visible = true;
                        GRID.SomarColunas(dtgvReceber, true, GRID.Formatação.Moeda, 9);
                    }
                }
                else
                {

                    if (cblsPagarReceber.SelectedValue == "P")
                    {
                        MensagemPagina.MostraMensagem_Erro("Nenhuma Pagamento Localizado");
                    }
                    else if (cblsPagarReceber.SelectedValue == "R")
                    {
                        MensagemPagina.MostraMensagem_Erro("Nenhum Recebimento Localizado");
                    }
                }
            }
            catch (Exception ex)
            {

                MensagemPagina.MostraMensagem_Erro("Erro ao Consultar: " + ex.Message);
            }

        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            if (ValidarPDF())
            {
                    Pesquisar("");
            }
        }

        private bool ValidarPDF()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (txtdtInicio.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Por favor, informe a data de início.";
            }
            if (txtdtFinal.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Por favor, informe a data final.";
            }
            if (ddlidEmpresa.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Por favor, selecione a empresa.";
            }
            if(cblsPagarReceber.SelectedValue == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Por favor, selecione se é contas a pagar ou receber.";
            }
            if (sMensagemErro != "")
            {
                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
                bRetorno = false;
            }

            return bRetorno;
        }

        protected void cmdPDF_Click(object sender, EventArgs e)
        {
            if (ValidarPDF())
            {
                GerarRelatorio(true);
            }
        }

        protected void cmdExcel_Click(object sender, EventArgs e)
        {
            if (ValidarPDF())
            {
                GerarRelatorio(false);
            }
        }

        void GerarRelatorio(bool bPDF)
        {
            try
            {
                ReportViewer rv = new ReportViewer();

                rv.ProcessingMode = ProcessingMode.Local;
                rv.LocalReport.EnableExternalImages = true;
                rv.LocalReport.EnableHyperlinks = true;

                rv.LocalReport.ReportPath = "App\\Reports\\" + "Relatório.rdlc";

                Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>();
                vParametrosProduct.Add("@dtInicio", txtdtInicio.Text);
                vParametrosProduct.Add("@dtFinal", txtdtFinal.Text);
                vParametrosProduct.Add("@idEmpresa", ddlidEmpresa.SelectedValue);

                DataSet dtProduct1;
                DataSet dtProduct;
                dtProduct1 = BD.ExecutarDataSet("sp_Manipula_tbl_Relatorio_Contas_Pagar_PDF", vParametrosProduct);
                dtProduct = BD.ExecutarDataSet("sp_Manipula_tbl_Relatorio_Contas_Receber_PDF", vParametrosProduct);

                bool Pagar = false;
                bool Receber = false;
                string Nome = "";
                if (cblsPagarReceber.SelectedValue == "P")
                {
                    Pagar = true;
                    Nome = "RelatórioPagar_";
                }
                else if (cblsPagarReceber.SelectedValue == "R")
                {
                    Receber = true;
                    Nome = "RelatórioReceber_";
                }

                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtProduct1.Tables[0]));
                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", dtProduct.Tables[0]));

                ReportParameter[] rp = new ReportParameter[2];

                rp[0] = new ReportParameter("Pagar", Pagar.ToString());
                rp[1] = new ReportParameter("Receber", Receber.ToString());

                rv.LocalReport.SetParameters(rp);

                rv.LocalReport.Refresh();
                string sNomeArquivo = "";
                byte[] bytes = bPDF ? rv.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings) : rv.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                DateTime Inicio = DateTime.Parse(txtdtInicio.Text);
                DateTime Final = DateTime.Parse(txtdtFinal.Text);
                if (bPDF == false)
                    sNomeArquivo = Nome + Inicio.ToString("dd-MM-yyyy") + "_X_" + Final.ToString("dd-MM-yyyy") + ".xlsx";
                else
                    sNomeArquivo = Nome + Inicio.ToString("dd-MM-yyyy") + "_X_" + Final.ToString("dd-MM-yyyy") + ".pdf";

                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);
                FUNCOES.DownloadArquivo(Page, sNomeArquivo);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao gerar o Relatório! </br>" + ex.Message + (ex.InnerException == null ? "" : "<br />" + ex.InnerException.Message));
            }
        }

        protected void cblsPagarReceber_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (ListItem item in cblsPagarReceber.Items)
            {
                if (item.Selected)
                {
                    foreach (ListItem otherItem in cblsPagarReceber.Items)
                    {
                        if (otherItem != item)
                        {
                            otherItem.Selected = false;
                        }
                    }
                    break;
                }
            }
        }
    }
}