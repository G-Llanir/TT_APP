using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;

namespace TT_Hub.App.Paginas.Manutencao
{
    public partial class Recursos : Page
    {
        string sTituloPagina = "Recursos";
        string sPagina_NovoRegistro = "app/Paginas/Manutencao/Recursos_Detalhe.aspx?id=0";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Recursos.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Recursos.Incluir);

            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlidTipoRecurso, "sp_Select 'Flow_Recursos_Tipo'", "idTipoRecurso", "sDscTipoRecurso", false, "Todos os Tipos", "0");
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

        protected void Pesquisar()
        {
            pnResultado.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR");
            vParametros.Add("@sDscRecurso", txtPesquisa.Text.Trim());
            vParametros.Add("@idTipoRecurso", ddlidTipoRecurso.SelectedValue);

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Recursos", vParametros, false);

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