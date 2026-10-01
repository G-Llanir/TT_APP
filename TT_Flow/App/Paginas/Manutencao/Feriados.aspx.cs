using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using System.Web.UI.WebControls;
using TT.FrameWork;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace TT_Flow.App.Paginas.Manutencao.Feriados
{
    public partial class Feriado : Page
    {
        string sTituloPagina = "Feriados";
        string sPagina_NovoRegistro = "app/Paginas/Manutencao/Feriados_Detalhe.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Manutencao.Feriados.Consultar, true);

            if (!FUNCOES.ValidaPermissao(Permissao.Manutencao.Feriados.Incluir))
            {
                cmdNovo.Visible = false;
                cmdRepetirAno.Visible = false;
            }

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                Pesquisar();
                RegistraScriptYear();
            }

            txtPesquisa.Focus();
            RegistraScriptYear();
        }

        protected DataTable Pesquisar()
        {
            pnResultado.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscFeriado", txtPesquisa.Text.Trim() },
                { "@dtFeriado", txtPesquisa.Text.Trim() }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Feriados", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, tb, 1, new int[1] { 3 }, "asc", "false", "''"), true);
                pnResultado.Visible = true;
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhum Lançamento Localizado");
                return null;
            }

            return tb;
        }

        protected void cmdExportar_Click(object sender, EventArgs e)
        {
            var dt = Pesquisar();

            if (dt != null)
            {
                dt.Columns["sDscFeriado"].ColumnName = "sDscItem";
                dt.Columns["sTipo"].ColumnName = "sDsc_1";
                dt.Columns["dtFeriado"].ColumnName = "data_1";

                // --------------------------------------------
                // Gabriel Llanir - 02/09/2024
                try
                {
                    ReportViewer rv = new ReportViewer();

                    rv.LocalReport.ReportPath = "App\\Reports\\Excel_Geral.rdlc";

                    rv.LocalReport.DataSources.Add(new ReportDataSource("ds_Consulta", dt));

                    ReportParameter[] rp = new ReportParameter[2];

                    rp[0] = new ReportParameter("Aparece_Tabela", "Feriados_");
                    rp[1] = new ReportParameter("Titulo_Personalizado", " ");

                    rv.LocalReport.SetParameters(rp);
                    rv.LocalReport.Refresh();

                    byte[] bytes = rv.LocalReport.Render("Excel", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);
                    string sNomeArquivo = "Consulta_Feriados_" + FUNCOES.CarimboDataHora() + ".xlsx";

                    File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);

                    FUNCOES.DownloadArquivo(Page, sNomeArquivo);

                    MensagemPagina.MostraMensagem_Sucesso("Relatório em Excel gerado com sucesso!", true);
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Houve um erro ao Gerar o Relatório em Excel da Consulta!<br />" + ex.InnerException + "<br />" + ex.Message, true);
                }
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        =>
            Pesquisar();

        protected void cmdNovo_Click(object sender, EventArgs e)
        =>
            FUNCOES.DirecionaPagina(sPagina_NovoRegistro);

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
        }

        protected void cmdRepetir_click(object sender, EventArgs e)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "DUPLICAR-ANO" },
                { "@sAnoDuplicar", txtsAno.Text }
            };
            BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Feriados", vParametros, false);
            Pesquisar();
        }

        void RegistraScriptYear()
        =>
            ScriptManager.RegisterStartupScript(this, this.GetType(), "DatePickerScript", "<script>\r\n$(document).ready(function () {\r\n$('#txtsAno').datepicker({\r\nformat: ' yyyy',\r\nviewMode: 'years',\r\nminViewMode: 'years'\r\n});\r\n});\r\n</script>", false);
    }
}