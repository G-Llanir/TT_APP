using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using TT.FrameWork;

namespace TT_Flow.App.Paginas.WMS.Manutencao
{
    public partial class TipoProduto : Page
    {
        string sTituloPagina = "Tipos";
        string sPagina_NovoRegistro = "app/Paginas/WMS/Manutencao/TipoProduto_Detalhe.aspx?id=0";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.TipoProdutos.Consultar, true);
            cmdNovoCadastro.Visible = FUNCOES.ValidaPermissao(Permissao.TipoProdutos.Incluir);

            if (!IsPostBack)
            {
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                Pesquisar();
            }
            txtPesquisa.Focus();
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;
            pnMensagem.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscTipoProduto", txtPesquisa.Text.Trim() },
                { "@sFiltroPersonalizado", ddlFiltroPersonalizado.SelectedValue }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_WMS_Produtos_Tipo", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, tb, 1, new int[1] { 13 }, "asc", "false", "''"), true);
                pnResultado.Visible = true;
            }
            else
            {
                pnMensagem.Visible = true;
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        =>
            Pesquisar();

        protected void cmdNovoCadastro_Click(object sender, EventArgs e)
        =>
            FUNCOES.DirecionaPagina(sPagina_NovoRegistro);

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 2; i <= dtgvConsulta.Columns.Count - 3; i++)
                {
                    e.Row.Cells[i].Text = HttpUtility.HtmlDecode(e.Row.Cells[i].Text.Replace("?", "✔"));
                }
            }
        }
    }
}