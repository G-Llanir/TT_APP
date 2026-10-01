using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using System.Web.UI.WebControls;
using TT_Flow.FrameWork;
using TT.FrameWork;

namespace TT_Flow.App.Paginas.WMS
{
    public partial class OPI : Page
    {
        string sTituloPagina = "Ordem de Produção Interna";
        string sPagina_NovoRegistro = "app/Paginas/WMS/OPI_Detalhe.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
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
        =>
            FUNCOES.Popula_Combo(ddlCliente, "sp_Select 'Flow_Clientes'", "idCliente", "sRazaoSocial", false, "Todos os Clientes", "0");

        protected void Pesquisar()
        {
            pnResultado.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA-COM-FILTRO" },
                { "@sDscOPI", txtPesquisa.Text.Trim() },
                { "@idCliente", ddlCliente.SelectedValue },
                { "@idStatus", ddlStatus.SelectedValue }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_FLow_WMS_OPI", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, tb, 0, new int[1] { 1 }, "desc", "false", "''"), true);
                pnResultado.Visible = true;
            }
            else
                MensagemPagina.MostraMensagem_Erro("Nenhum Lançamento Localizado");
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
            {
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }
        }

    }
}