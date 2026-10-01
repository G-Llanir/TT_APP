using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using TT.FrameWork;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace TT_Flow.App.Paginas.Adm.Patrimonio
{
    public partial class PatrimonioLocal : System.Web.UI.Page
    {
        string sTituloPagina = "Local de Patrimônio";
        string sPagina_NovoRegistro = "app/Paginas/Adm/Patrimonio/PatrimonioLocal_Detalhe.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Patrimonio.Local.Consultar, true); ;
            if (!FUNCOES.ValidaPermissao(Permissao.Patrimonio.Local.Incluir))
            {
                cmdNovo.Visible = false;
            }
            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
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
                { "@sDscLocal", txtPesquisa.Text.Trim() }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Adm_Patrimonio_Local", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, tb, 1, new int[1] { 4 }, "asc", "false", "''"), true);

                pnResultado.Visible = true;
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhum registro encontrado!");
                return null;
            }
            return tb;
        }

        protected void cmdExportar_Click(object sender, EventArgs e)
        {
            var dt = Pesquisar();

            if (dt != null)
            {
                dt.Columns["sDscLocal"].ColumnName = "sDscItem";
                dt.Columns["sDscDepartamento"].ColumnName = "sDsc_1";
                dt.Columns["sSituacao_Completa"].ColumnName = "sDsc_2";
                dt.Columns["dtAtualizacao"].ColumnName = "data_1";
                dt.Columns["sDscUsuarioAtualizacao"].ColumnName = "sDsc_3";

                // --------------------------------------------
                // Gabriel Llanir - 02/09/2024
                try
                {
                    ReportViewer rv = new ReportViewer();

                    rv.LocalReport.ReportPath = "App\\Reports\\Excel_Geral.rdlc";

                    rv.LocalReport.DataSources.Add(new ReportDataSource("ds_Consulta", dt));

                    ReportParameter[] rp = new ReportParameter[2];

                    rp[0] = new ReportParameter("Aparece_Tabela", "LocalPatrimonio_");
                    rp[1] = new ReportParameter("Titulo_Personalizado", " ");

                    rv.LocalReport.SetParameters(rp);
                    rv.LocalReport.Refresh();

                    byte[] bytes = rv.LocalReport.Render("Excel", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);
                    string sNomeArquivo = "Consulta_Locais_Patrimonio_" + FUNCOES.CarimboDataHora() + ".xlsx";

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
        {
            Pesquisar();
        }

        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina(sPagina_NovoRegistro);
        }

    }
}