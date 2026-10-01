using System;
using System.Collections.Generic;
using System.Web.UI;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using System.Data;

namespace TT_Flow.App.Paginas.WMS.Manutencao
{
    public partial class LocalArmazenamento : System.Web.UI.Page
    {
        string sTituloPagina = "Local de Armazenamento";
        string sPagina_NovoRegistro = "app/Paginas/WMS/Manutencao/LocalArmazenamento_Detalhe.aspx?id=0";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(90, true);

            if (!FUNCOES.ValidaPermissao(91))
                cmdNovoCadastro.Visible = false;

            if (!IsPostBack)
            {

                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                pnMensagem.Visible = false;
                Pesquisar();
            }

            txtPesquisa.Focus();
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;
            pnMensagem.Visible = false;

            string sSql = "sp_Manipula_tbl_Flow_WMS_LocalArmazenamento";
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscLocalArmazenamento", txtPesquisa.Text.Trim() }
            };
            DataTable tb = BD.ExecutarDataTable(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 1, new int[1] { 2 }, "asc", "false", "''"), true);
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
    }
}