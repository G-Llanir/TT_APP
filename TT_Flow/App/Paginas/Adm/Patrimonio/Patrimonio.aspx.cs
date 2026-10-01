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

namespace TT_Flow.App.Paginas.Adm.Patrimonio
{
    public partial class Patrimonio : System.Web.UI.Page
    {
        string sTituloPagina = "Patrimônio";
        string sPagina_NovoRegistro = "app/Paginas/Adm/Patrimonio/Patrimonio_Detalhe.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Patrimonio.ControlePatrimonio.Consultar, true);

            if (!FUNCOES.ValidaPermissao(Permissao.Patrimonio.ControlePatrimonio.Incluir))
                cmdNovo.Visible = false;

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                PopularCombos();
                Pesquisar();
            }

            txtPesquisa.Focus();
        }

        protected void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlCategoria, "sp_Select 'Flow_Patrimonio_Categorias'", "idCategoria", "sDscCategoria", false, "Todas as Categorias", "0");
            FUNCOES.Popula_Combo(ddlGrupo, "sp_Select 'Flow_Patrimonio_Grupo'", "idPatrimonioGrupo", "sDscPatrimonio", false, "Todos os Grupos", "0");
            FUNCOES.Popula_Combo(ddlLocal, "sp_Select 'Flow_Patrimonio_Local'", "idLocal", "sDscLocal", false, "Todos os Locais", "0");
            FUNCOES.Popula_Combo(ddlEstadoConservacao, "sp_Select 'Flow_Patrimonio_EstadoConservacao'", "idEstadoConservacao", "sDscEstadoConservacao", false, "Todos os Estados de Conservação", "0");
            FUNCOES.Popula_Combo(ddlParceiro, "sp_Select 'Flow_Parceiros_Fornecedores'", "idCliente", "sRazaoSocial", false, "Todos os Fornecedores", "0");
            FUNCOES.Popula_Combo(ddlDepartamento, "sp_Select 'Flow_Departamentos_Local'", "idDepartamento", "sDscDepartamento", false, "Selecione o Departamento", "0");
        }

        protected DataTable Pesquisar()
        {
            pnResultado.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@idCategoria", ddlCategoria.SelectedValue },
                { "@idGrupo", ddlGrupo.SelectedValue },
                { "@idLocal", ddlLocal.SelectedValue },
                { "@idEstadoConservacao", ddlEstadoConservacao.SelectedValue },
                { "@idParceiro", ddlParceiro.SelectedValue },
                { "@idDepartamento", ddlDepartamento.SelectedValue },
                { "@sDscPatrimonio", txtPesquisa.Text.Trim() },
                { "@sEtiquetaPatrimonio", txtPesquisa.Text.Trim() }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Adm_Patrimonio", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, tb, 3, new int[1] { 2 }, "asc", "false", "''"), true);
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
                dt.Columns["sEtiquetaPatrimonio"].ColumnName = "sCodigo";
                dt.Columns["dtInclusao"].ColumnName = "data_1";
                dt.Columns["sDscPatrimonio"].ColumnName = "sDscItem";
                dt.Columns["sDscCategoria"].ColumnName = "sDsc_1";
                dt.Columns["sDscParceiro"].ColumnName = "sDsc_2";
                dt.Columns["sDscLocal"].ColumnName = "sDsc_3";
                dt.Columns["sDscResponsavel"].ColumnName = "sDsc_4";
                dt.Columns["sDscEstadoConservacao"].ColumnName = "sDsc_5";

                // --------------------------------------------
                // Gabriel Llanir - 02/09/2024
                try
                {
                    ReportViewer rv = new ReportViewer();

                    rv.LocalReport.ReportPath = "App\\Reports\\Excel_Geral.rdlc";

                    rv.LocalReport.DataSources.Add(new ReportDataSource("ds_Consulta", dt));

                    ReportParameter[] rp = new ReportParameter[2];

                    rp[0] = new ReportParameter("Aparece_Tabela", "Patrimonios_");
                    rp[1] = new ReportParameter("Titulo_Personalizado", " ");

                    rv.LocalReport.SetParameters(rp);
                    rv.LocalReport.Refresh();

                    byte[] bytes = rv.LocalReport.Render("Excel", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);
                    string sNomeArquivo = "Consulta_Patrimonios_" + FUNCOES.CarimboDataHora() + ".xlsx";

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

        protected void dtgvConsulta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Duplicar")
            {
                int idPatrimonio = Convert.ToInt32(e.CommandArgument);

                FUNCOES.DirecionaPagina($"App/Paginas/Adm/Patrimonio/Patrimonio_Detalhe.aspx?id={idPatrimonio}&duplicar=true");
            }
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
        }
    }
}