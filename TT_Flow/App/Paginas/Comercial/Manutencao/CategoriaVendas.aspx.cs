using System;
using System.Collections.Generic;
using System.Web.UI;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using System.Data;
using TT_Flow.App.Controles;

namespace TT_Flow.App.Paginas.Comercial.Manutencao
{
    public partial class CategoriaVendas : Page
    {
        string sTituloPagina = "Categoria de Vendas";
        string sPagina_NovoRegistro = "app/Paginas/Comercial/Manutencao/CategoriaVendas_Detalhe.aspx?id=0";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Comercial.CategoriaVendas.Consultar, true);
            cmdNovoCadastro.Visible = FUNCOES.ValidaPermissao(Permissao.Comercial.CategoriaVendas.Incluir);

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
                { "@sDscCategoriaVendas", txtPesquisa.Text.Trim() }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Comercial_CategoriaVendas", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                // Índices mudaram: as colunas Código, Padrão HH e Ativo foram inseridas antes
                // da data. Ordenação passou de 1 para 2 (Categoria de Vendas) e a coluna de
                // data, de 2 para 5.
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 2, new int[1] { 5 }, "asc", "false", "''"), true);
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