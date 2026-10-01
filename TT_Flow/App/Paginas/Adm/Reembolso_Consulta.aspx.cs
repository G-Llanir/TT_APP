using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using System.Text;
using GRID = TT.FrameWork.Grid;

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class Reembolso_Consulta : System.Web.UI.Page
    {
        // 1. ALTERADO: Usando a SP correta de Reembolso
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Reembolso";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            // 2. Opcional: Alterar o manual se houver um específico para reembolso
            manual.sNomeArquivo = "Manual-RelatorioReembolso.pdf";
            Pesquisar();

            if (IsPostBack)
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                if (requestTarget == "funcao_SAIR")
                    FUNCOES.DirecionaPagina("/Aplicativo/MenuColaborador.aspx");
                else if (requestTarget == "funcao_CONFIRMAR")
                {

                }
            }
            else
            {

            }

            RegistraScript();
        }
        #endregion

        #region | Metodos Banco de Dados
        protected void Pesquisar()
        {
            // Nota: Se houver uma Permissao específica para reembolso, substitua aqui
            FUNCOES.ValidaPermissao(Permissao.RelatorioDespesas.Reembolso, true);

            lblTituloPagina.Text = "Relatório de Reembolsos"; // 3. Título ajustado

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@idUsuario", IDENTITY.Variaveis.idUsuario() },
                { "@sPesquisa", txtStatus.Text},
                { "@idStatus", ddlsStatus.SelectedValue},
                { "@Sistema", "T-Flow"}
            };
            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa))
            {
                div_gvRelatorio.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", GRID.DataBindComScriptData(gvRelatorio, dsPesquisa.Tables[0], 0, new int[1] { 0 }, "desc", "false", "''"), true);
            }
            else
            {
                div_gvRelatorio.Visible = false;
            }
        }
        #endregion

        #region | Script
        void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }
        #endregion

        #region | Eventos
        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            // 4. ALTERADO: Direciona para a tela de Reembolso_Detalhe
            FUNCOES.DirecionaPagina("App/Paginas/Adm/Reembolso_Detalhe.aspx?id=0");
        }

        protected void gvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string cor = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
                if (cor == "primary")
                    cor = "info";
                e.Row.CssClass = cor;
            }
        }
        #endregion
    }
}