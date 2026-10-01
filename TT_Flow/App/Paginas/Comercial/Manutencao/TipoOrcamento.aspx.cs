using System;
using System.Data;
using System.Collections.Generic;
using System.Web.UI;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;

namespace TT_Flow.App.Paginas.Comercial.Manutencao
{
    public partial class TipoOrcamento : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-TipoOrcamento.pdf";

            if (!IsPostBack)
            {
                div_resultado.Visible = false;

                FUNCOES.ValidaPermissao(Permissao.Comercial.Manutencao.TipoOrcamento.Consultar, true);

                Pesquisar();
            }
        }

        void Pesquisar()
        {
            lblTituloPagina.Text = "Tipos de Orçamento";
            BreadCrumb_Pagina.TitulodaPagina = "Tipo de Orçamento";

            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Comercial.Manutencao.TipoOrcamento.Incluir);

            Dictionary<string, string> vParametros = new Dictionary<string, string>()
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscPesquisa", txtPesquisa.Text }
            };
            DataSet dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_Orcamento_Tipo", vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out string sErro))
            {
                div_resultado.Visible = true;

                // ColunasDatas passou de 3 para 4: a coluna "Calcular Instalação" foi inserida
                // antes de "Data de Atualização", deslocando o índice.
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Consulta_TipoOrcamento", GRID.DataBindComScriptData(gvConsulta, dsPesquisa.Tables[0], 1, new int[1] { 4 }, "asc", "false", "''"), true);
            }
            else
                MensagemPagina.MostraMensagem_Erro("Não foi encontrado nenhum registro!");
        }

        protected void cmdNovo_Click(object sender, EventArgs e)
        =>
            FUNCOES.DirecionaPagina("App/Paginas/Comercial/Manutencao/TipoOrcamento_Detalhe.aspx?id=0");

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        =>
            Pesquisar();
    }
}