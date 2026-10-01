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
    public partial class Relatorio_Despesas : System.Web.UI.Page
    {
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Relatorio_Despesas";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-RelatorioDespesas.pdf";
            Pesquisar();

            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.RelatorioDespesas.NovoRelatorio);

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
            FUNCOES.ValidaPermissao(Permissao.RelatorioDespesas.Consultar, true);
            lblTituloPagina.Text = "Relatório de Despesas";

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR-TUDO" },
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
                //gvRelatorio.DataSource = dsPesquisa.Tables[0];
                //gvRelatorio.DataBind();
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
            FUNCOES.DirecionaPagina("App/Paginas/Adm/Relatorio_Despesas_Detalhe.aspx?id=0");
        }

        //protected void gvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    GRID.EsconderColunas(e, 0);

        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        string cor = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
        //        if (cor == "primary")
        //            cor = "info";
        //        e.Row.CssClass = cor;
        //    }
        //}

        protected void gvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // GRID.EsconderColunas(e, 0); 

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // 1. Status Color (Já existente)
                string cor = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
                if (cor == "primary") cor = "info";
                e.Row.CssClass = cor;

                // 2. Saldo Color (Já existente)
                string sSaldo = DataBinder.Eval(e.Row.DataItem, "nSaldo").ToString();
                // O índice da célula mudou porque trocamos um campo. 
                // Saldo agora é a penúltima coluna (Index 6 se começar do 0)
                // Confirme o índice correto contando as colunas no ASPX (0=ID, 1=Data, 2=Solic, 3=Part, 4=Motivo, 5=Gasto, 6=Saldo, 7=Status)
                int indexSaldo = 6;

                if (sSaldo.Contains("-"))
                {
                    e.Row.Cells[indexSaldo].ForeColor = System.Drawing.Color.Red;
                    e.Row.Cells[indexSaldo].Font.Bold = true;
                }
                else
                {
                    e.Row.Cells[indexSaldo].ForeColor = System.Drawing.Color.Green;
                }

                // -----------------------------------------------------------
                // 3. NOVO VISUAL DOS PARTICIPANTES (Badges)
                // -----------------------------------------------------------
                Literal litPart = (Literal)e.Row.FindControl("litParticipantes");
                string nomesRaw = DataBinder.Eval(e.Row.DataItem, "sParticipantesNomes").ToString();

                if (litPart != null && !string.IsNullOrEmpty(nomesRaw))
                {
                    // Se for "Apenas Solicitante", deixa simples
                    if (nomesRaw == "Apenas Solicitante")
                    {
                        litPart.Text = "<span class='text-muted' style='font-style:italic; font-size:11px;'>Apenas Solicitante</span>";
                    }
                    else
                    {
                        // Separa por vírgula
                        string[] nomes = nomesRaw.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        StringBuilder sbHtml = new StringBuilder();

                        sbHtml.Append("<div style='line-height: 1.6;'>"); // Espaçamento vertical

                        foreach (string nome in nomes)
                        {
                            // Gera um badge cinza claro para cada nome
                            // style='display:inline-block' garante que não quebre o badge no meio
                            sbHtml.Append($"<span class='label label-default' style='font-size: 11px; margin-right: 3px; display:inline-block; padding:4px 6px;'>{nome.Trim()}</span> ");
                        }
                        sbHtml.Append("</div>");

                        litPart.Text = sbHtml.ToString();
                    }
                }
            }
        }
        #endregion
    }
}