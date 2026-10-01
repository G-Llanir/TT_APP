using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;

namespace TT_Hub.App.Paginas.Manutencao
{
    public partial class CondicaodePagamento : Page
    {
        string sTituloPagina = "Condição de Pagamento";
        string sPagina_NovoRegistro = "app/Paginas/Manutencao/CondicaodePagamento_Detalhe.aspx?id=0";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.CondicaodePagamento.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.CondicaodePagamento.Incluir);

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
                { "@sDscCondicaoPagamento", txtPesquisa.Text.Trim() },
                { "@idPedido", ddlPedido.SelectedValue }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_CondicaodePagamento", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 1, new int[1] { 3 }, "asc", "false", "''"), true);
                pnResultado.Visible = true;
            }
            else
            {
                pnMensagem.Visible = true;
                lblMensagem.Text = "Nenhum registro localizado para sua pesquisa!";
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void cmdNovo_Click(object sender, EventArgs e) => FUNCOES.DirecionaPagina(sPagina_NovoRegistro);
    }
}