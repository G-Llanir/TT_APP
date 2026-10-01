using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;


namespace TT_Flow.App.Paginas.OS.Manutencao
{
    public partial class Procedimento : Page
    {
        string sTituloPagina = "Procedimentos";
        string sPagina_NovoRegistro = "app/Paginas/OS/Manutencao/Procedimento_Detalhe.aspx?id=0";
        string sPagina_ConsultarTipo = "app/Paginas/OS/Manutencao/Procedimento_Tipo.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.OrdemServico.Procedimentos.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.OrdemServico.Procedimentos.Incluir);

            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlDepartamento, "sp_Select 'Flow_Colaboradores_Departamentos_OS'", "idDepartamento", "sDscDepartamento", false, "Selecione um Departamento", "0");
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                Pesquisar();
            }

            txtPesquisa.Focus();
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        =>
            Pesquisar();

        protected void cmdNovo_Click(object sender, EventArgs e)
        =>
            FUNCOES.DirecionaPagina(sPagina_NovoRegistro);

        protected void cmdConsultarTipo_Click(object sender, EventArgs e)
        =>
            FUNCOES.DirecionaPagina(sPagina_ConsultarTipo);

        protected void Pesquisar()
        {
            pnResultado.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscProcedimento", txtPesquisa.Text.Trim() },
                { "@idDepartamento", ddlDepartamento.SelectedValue }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Procedimentos", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 1, new int[1] { 4 }, "asc", "false", "''"), true);
                pnResultado.Visible = true;
            }
            else
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
        }
    }
}