using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.WMS
{
    public partial class Movimentacao_Consulta : System.Web.UI.Page
    {
        string sTituloPagina = "WMS - Movimentação";
        readonly string sPagina_NovoRegistro = "app/Paginas/WMS/Movimentacao_Detalhe.aspx?id=0";
        readonly string sPagina_ImportarRegistro = "app/Paginas/WMS/Movimentacao_Detalhe.aspx?id=0&ACAO=ImportarXML";

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            scriptManager.RegisterPostBackControl(this.cmdExportar);
            cmdImportarXml.Visible = FUNCOES.ValidaPermissao(Permissao.WMS.Movimentacao.ImportarXML, false);

            if (!IsPostBack)
            {
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                PopularCombos();
                Pesquisa();
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisa();
        }

        protected void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidTipoMovimentacao, "sp_Select 'Flow_WMS_Produtos_Movimentacao_Tipo'", "idTipoMovimentacao", "sDscTipoMovimentacao", false,
            "Todos os Tipos de Movimentação", "0");

            ddlidMotivo.Visible = false;

            FUNCOES.Popula_Combo(ddlidStatus, "sp_Manipula_tbl_Flow_Produtos_Movimentacao @sFuncao = 'SELECT_STATUS'", "idStatus", "sDscStatus", false, "Todos os Status", "0");
            ddlidStatus.SelectedValue = "-1";
        }

        protected DataTable Pesquisa()
        {
            pnResultado.Visible = false;
            pnMensagem.Visible = true;


            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@idTipoMovimentacao", ddlidTipoMovimentacao.SelectedValue },
                { "@idMotivo", ddlidMotivo.SelectedValue }, 
                { "@idStatus", ddlidStatus.SelectedValue},
                { "@dtInicial", txtDataInicial.Text },
                { "@dtFinal", txtDataFinal.Text }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", Grid.DataBindComScript(dtgvConsulta, tb), true);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 0, new int[1] {1}, "desc", "false", "''"), true);
                pnResultado.Visible = true;
            }
            else
            {
                pnMensagem.Visible = true;
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
                return null;
            }

            return tb;
        }

        protected void cmdExportar_Click(object sender, EventArgs e)
        {
            var dt = Pesquisa();

            if (dt != null)
            {
                dt.Columns["dtMovimentacao"].ColumnName = "data_1";
                dt.Columns["sDscTipoMovimentacao"].ColumnName = "sDscItem";
                dt.Columns["sDscTipo"].ColumnName = "sDsc_1";
                dt.Columns["sDscMotivo"].ColumnName = "sDsc_2";
                dt.Columns["sDscUsuarioAtualizacao"].ColumnName = "sDsc_3";

                // --------------------------------------------
                // Gabriel Llanir - 02/09/2024
                try
                {
                    ReportViewer rv = new ReportViewer();

                    rv.LocalReport.ReportPath = "App\\Reports\\Excel_Geral.rdlc";

                    rv.LocalReport.DataSources.Add(new ReportDataSource("ds_Consulta", dt));

                    ReportParameter[] rp = new ReportParameter[2];

                    rp[0] = new ReportParameter("Aparece_Tabela", "Movimentacao_");
                    rp[1] = new ReportParameter("Titulo_Personalizado", " ");

                    rv.LocalReport.SetParameters(rp);
                    rv.LocalReport.Refresh();

                    byte[] bytes = rv.LocalReport.Render("Excel", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);
                    string sNomeArquivo = "Consulta_Movimentacao_" + FUNCOES.CarimboDataHora() + ".xlsx";

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

        protected void cmdNovaMovimentacao_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina(sPagina_NovoRegistro);
        }

        protected void cmdImportarXml_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina(sPagina_ImportarRegistro);
        }

        protected void ddlidTipoMovimentacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidTipoMovimentacao.SelectedValue == "0")
            {
                ddlidMotivo.Items.Clear();
                ddlidMotivo.Visible = false;
            }
            else
            {
                FUNCOES.Popula_Combo(ddlidMotivo, "sp_Select 'Flow_WMS_Produtos_Movimentacao_Motivo'", Convert.ToInt32(ddlidTipoMovimentacao.SelectedValue), "idMotivo", "sDscMotivo", false, "Todos os Motivos", "0");
                ddlidMotivo.Visible = true;
                Pesquisa();
            }
        }
        protected void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {
            Pesquisa();
        }
        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCorStatus").ToString();
            }
        }
    }
}