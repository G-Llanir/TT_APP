using System;
using System.Collections.Generic;
using System.Web.UI;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using System.Data;
using TT_Flow.App.Controles;

namespace TT_Flow.App.Paginas.WMS.Manutencao
{
    public partial class Familia : Page
    {
        string sTituloPagina = "Família de Produtos";
        string sPagina_NovoRegistro = "app/Paginas/WMS/Manutencao/Familia_Detalhe.aspx?id=0";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Familia.Consultar, true);
            cmdNovoCadastro.Visible = FUNCOES.ValidaPermissao(Permissao.Familia.Incluir);

            if (!IsPostBack)
            {
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                Pesquisar();
            }
            txtPesquisa.Focus();
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        =>
            Pesquisar();

        protected void cmdNovoCadastro_Click(object sender, EventArgs e)
        =>
            FUNCOES.DirecionaPagina(sPagina_NovoRegistro);

        protected void Pesquisar()
        {
            pnResultado.Visible = false;
            pnMensagem.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscFamilia", txtPesquisa.Text.Trim() }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_WMS_Produtos_Familia", vParametros, false);

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
    }
}