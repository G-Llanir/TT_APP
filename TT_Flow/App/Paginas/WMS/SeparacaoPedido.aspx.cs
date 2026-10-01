using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using TT_Flow.FrameWork;
using TT_Flow.App.Controles;
using TT.FrameWork;

namespace TT_Flow.App.Paginas.WMS
{
    public partial class SeparacaoPedido : Page
    {
        string sTituloPagina = "Separação de Pedidos";
        string sPagina_NovoRegistro = "app/Paginas/WMS/SeparacaoPedido.aspx?id=0";
        string sTipoVisualizacao = "N";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.WMS.SeparacaoPedidos.Separacao, true);

            if (!IsPostBack)
            {
                if(FUNCOES.ValidaPermissao(Permissao.Produtos.Alterar))

                pnResultado.Visible = false;
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                Pesquisar();
            }
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_SEPARACAOPEDIDOS" },
                { "@sDscPesquisa", txtPesquisa.Text.Trim() },
                { "@sSeparacao", ddlsSeparacao.SelectedValue }
            };

            DataSet ds = BD.ExecutarDataSet("sp_Manipula_FLow_WMS_OPI", vParametros, false);


            if (ds.Tables[0].Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", Grid.DataBindComScript(dtgvConsulta, ds, 0, "desc"), true);
                pnResultado.Visible = true;
            }
            else
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        =>
            Pesquisar();

        protected void ddlSituacaoCadastral_SelectedIndexChanged(object sender, EventArgs e)
        {
           Pesquisar();
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCorSeparacao").ToString();
            }
        }
    }
}