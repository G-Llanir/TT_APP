using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;

namespace TT_Hub.App.Paginas.Manutencao
{
    public partial class RegistroVeiculo : Page
    {
        string sTituloPagina = "Veículos";
        string sPagina_NovoRegistro = "app/Paginas/Manutencao/RegistroVeiculo_Detalhe.aspx?id=0";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Veiculos.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Veiculos.Incluir);

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

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sPesquisa", txtPesquisa.Text.Trim() }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Veiculos", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 2, new int[1] { 4 }, "asc", "false", "''"), true);
                pnResultado.Visible = true;
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
                pnResultado.Visible = true;
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        => Pesquisar();

        protected void cmdNovo_Click(object sender, EventArgs e)
        => FUNCOES.DirecionaPagina(sPagina_NovoRegistro);
    }
}