using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.BD;

namespace TT_Flow.App.Paginas.WMS.Manutencao
{
    public partial class NCM : Page
    {
        string sTituloPagina = "NCM";
        readonly string sPagina_NovoRegistro = "app/Paginas/WMS/Manutencao/NCM_Detalhe.aspx?id=0";

        protected void Page_Load(object sender, EventArgs e)
        {
            ValidaPermissao(Permissao.NCM.Consultar, true);
            cmdNovoCadastro.Visible = ValidaPermissao(Permissao.NCM.Incluir);

            if (!IsPostBack)
            {
                PopularCombos();
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                Pesquisar();
            }

            txtPesquisa.Focus();
        }

        protected DataTable Pesquisar()
        {
            pnResultado.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sPesquisa", txtPesquisa.Text.Trim() },
                { "@idCEST", ddlCEST.SelectedValue }
            };
            DataTable tb = ExecutarDataTable("sp_Manipula_tbl_Flow_WMS_NCM", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, tb, 2, new int[1] { 5 }, "asc", "false", "''"), true);
                pnResultado.Visible = true;
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado!");
                return null;
            }

            return tb;
        }

        void PopularCombos() => Popula_Combo(ddlCEST, "sp_Select 'Flow_WMS_Produtos_CEST'", "idCEST", "sCodigoCEST", false, "Todos os CEST", "0");

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void cmdNovoCadastro_Click(object sender, EventArgs e) => DirecionaPagina(sPagina_NovoRegistro);

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
                e.Row.Cells[4].Text = e.Row.Cells[4].Text == "S" ? "<i class='fa fa-check'></i>" : "";
        }

        protected void cmdExportar_Click(object sender, EventArgs e)
        {
            var dt = Pesquisar();

            if (dt != null)
            {
                dt.Columns["sCodigoNCM"].ColumnName = "sCodigo";
                dt.Columns["sDscNCM"].ColumnName = "sDscItem";
                dt.Columns["sDscCEST"].ColumnName = "sDsc_1";
                dt.Columns["dtAtualizacao"].ColumnName = "data_1";
                dt.Columns["sDscUsuarioAtualizacao"].ColumnName = "sDsc_2";

                // --------------------------------------------
                // Gabriel Llanir - 02/09/2024
                try
                {
                    ReportViewer rv = new ReportViewer();

                    rv.LocalReport.ReportPath = "App\\Reports\\Excel_Geral.rdlc";

                    rv.LocalReport.DataSources.Add(new ReportDataSource("ds_Consulta", dt));

                    ReportParameter[] rp = new ReportParameter[2];

                    rp[0] = new ReportParameter("Aparece_Tabela", "NCM_");
                    rp[1] = new ReportParameter("Titulo_Personalizado", " ");

                    rv.LocalReport.SetParameters(rp);
                    rv.LocalReport.Refresh();

                    byte[] bytes = rv.LocalReport.Render("Excel", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);
                    string sNomeArquivo = "Consulta_NCM_" + CarimboDataHora() + ".xlsx";

                    File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);

                    DownloadArquivo(Page, sNomeArquivo);

                    MensagemPagina.MostraMensagem_Sucesso("Relatório em Excel gerado com sucesso!", true);
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Houve um erro ao Gerar o Relatório em Excel da Consulta!<br />" + ex.InnerException + "<br />" + ex.Message, true);
                }
            }
        }
    }
}