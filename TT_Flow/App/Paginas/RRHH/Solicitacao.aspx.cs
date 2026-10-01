using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using System.Web.UI.WebControls;
using IDENTITY = TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.RRHH.Solicitacoes
{
    public partial class Solicitacao : System.Web.UI.Page
    {
        string sTituloPagina = "Solicitações do Colaborador";
        string sPagina_NovoRegistro = "app/Paginas/RRHH/Solicitacoes/Solicitacao_Detalhe.aspx";
        string sistema = "TFLOW";
        string sProcedure = "sp_Manipula_tbl_Flow_Solicitacoes";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FUNCOES.ValidaPermissao(Permissao.RRHH.Consultar, true);
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                Pesquisar();
                PopulaCombos();
            }
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;
            
            string sFuncao = "CONSULTAR-TODOS";

            DataTable tb;
            string sSql = "sp_Manipula_tbl_Flow_Solicitacoes";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idUsuarioLogado", IDENTITY.Variaveis.idUsuario());
            vParametros.Add("@sSistema", sistema);
            vParametros.Add("@idFiltro", ddlidDepartamento.SelectedValue);
            //vParametros.Add("@dtSolicitacao", txtdtSolicitacao.Text);
            vParametros.Add("@idUsuario", ddlidUsuario.SelectedValue);
            vParametros.Add("@idTipoSolicitacao", ddlidTipo.SelectedValue);
            vParametros.Add("@idStatus", ddlidStatus.SelectedValue);
            tb = BD.ExecutarDataTable(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 1, "desc", "false", "''"), true);

                pnResultado.Visible = true;
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhum Lançamento Localizado");
            }
        }

        void PopulaCombos()
        {
            FUNCOES.Popula_Combo(ddlidDepartamento, "sp_Select 'Flow_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Todos Departamento", "0");
            FUNCOES.Popula_Combo(ddlidUsuario, "sp_Select 'Usuarios'", "idUsuario", "sDscUsuario", false, "Todos Usuários", "0");
            FUNCOES.Popula_Combo(ddlidTipo, "sp_Select 'Flow_Solicitacao_Tipo'", "idTipo", "sDscTipo", false, "Todos os Tipos", "0");
            FUNCOES.Popula_Combo(ddlidStatus, $"{sProcedure} 'Flow_Status'", "idStatus", "sDscStatus", false, "Todos Status", "0");
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina(sPagina_NovoRegistro);
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ////string idSolicitacao = DataBinder.Eval(e.Row.DataItem, "idSolicitacao").ToString();
                ////string descricao = DataBinder.Eval(e.Row.DataItem, "sDscSolicitacao").ToString();
                ////string textoFormatado;

                ////if (System.Text.RegularExpressions.Regex.IsMatch(descricao.Trim(), @"^#?\s*\d+\s*-"))
                ////{
                ////    textoFormatado = descricao.Trim().TrimStart('#').Trim();
                ////}
                ////else
                ////{
                ////    textoFormatado = $"{idSolicitacao} - {descricao.Trim()}";
                ////}
                ////int indexColunaDesc = 3;
                ////if (e.Row.Cells[indexColunaDesc].Controls.Count > 0 && e.Row.Cells[indexColunaDesc].Controls[0] is HyperLink)
                ////{
                ////    HyperLink link = (HyperLink)e.Row.Cells[indexColunaDesc].Controls[0];
                ////    link.Text = textoFormatado;
                ////}

                // ---------------------------------------------------------
                // 2. LÓGICA ORIGINAL: Cores (Mantida)
                // ---------------------------------------------------------

                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
                // Percorre todas as células da linha atual
                foreach (TableCell cell in e.Row.Cells)
                {
                    // Aplica a classe de cor Bootstrap se existir na célula
                    string corClass = cell.Text.Trim();
                    if (!string.IsNullOrEmpty(corClass) && corClass.StartsWith("bg-"))
                    {
                        cell.CssClass = corClass; // Aplica a classe como estilo CSS
                    }
                }
            }
        }

    }
}