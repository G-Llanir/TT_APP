using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using System.Text;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using static TT.FrameWork.Identity;
using TT_Colaborador.FrameWork;
using System.Linq;
using GRID = TT.FrameWork.Grid;
using System.Web.UI.HtmlControls;
using System.Web; // <-- ADICIONADO

namespace TT_Colaborador.Aplicativo.Paginas.Despesas
{
    public partial class Relatorio_Despesas : Page
    {
        #region | Classes
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Relatorio_Despesas";
        #endregion

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Relatorio_de_Gastos.Consultar, true, true);
            //cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Relatorio_de_Gastos.Incluir, false, true);

            if (!IsPostBack) // Executa a pesquisa apenas no primeiro load
            {
                Pesquisar();
            }

            if (IsPostBack)
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                if (requestTarget == "funcao_SAIR")
                    FUNCOES.DirecionaPagina("/Aplicativo/MenuColaborador.aspx");
                else if (requestTarget == "funcao_CONFIRMAR")
                {

                }
            }

            RegistraScript();
        }
        #endregion

        #region | Metodos Banco de Dados
        protected void Pesquisar()
        {
            lblTituloPagina.Text = "Relatório de Despesas";

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                // ALTERAÇÃO AQUI: Chamando a nova função que filtra por participante
                { "@sFuncao", "CONSULTAR-DESPESAS-COLABORADOR" },
                { "@idUsuario", IDENTITY.Variaveis.idUsuario() },
                { "@sPesquisa", txtStatus.Text.Trim()},
                { "@idStatus", ddlsStatus.SelectedValue}
            };

            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa))
            {
                rptRelatorio.DataSource = dsPesquisa.Tables[0];
                rptRelatorio.DataBind();

                if (dsPesquisa.Tables[0].Rows.Count == 0)
                {
                    MensagemPagina_Entregas.MostraMensagem("Nenhum relatório encontrado para os filtros informados.", "info", false);
                }
            }
            else
            {
                rptRelatorio.DataSource = null;
                rptRelatorio.DataBind();
                MensagemPagina_Entregas.MostraMensagem("Nenhum relatório encontrado onde você seja participante.", "info", false);
            }
        }
        #endregion

        #region | Script
        void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();
            // Script de modal (se houver) pode ser registrado aqui.
            // O script do toggleHistory está direto no ASPX, o que é ideal para UpdatePanels.
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }
        #endregion

        #region | Eventos
        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina("Aplicativo/Paginas/Despesas/Relatorio_Despesas_Detalhe.aspx?id=0");
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar(); // Adicionado evento de clique para o botão de pesquisa
        }


        // Evento gvRelatorio_RowDataBound não é mais necessário, pois gvRelatorio foi removido.
        // protected void gvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) { ... }


        protected void rptRelatorio_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    DataRowView row = (DataRowView)e.Item.DataItem;
                    int idDespesa = Convert.ToInt32(row["idDespesas"]);

                    // --- Bloco 1: Carregar Gastos (Sua lógica original RESTAURADA) ---
                    HtmlButton btnVerGastos = (HtmlButton)e.Item.FindControl("btnVerGastos");

                    // Re-executa a query principal "CONSULTAR" para pegar a Tabela[1]
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@idUsuario", IDENTITY.Variaveis.idUsuario() },
                { "@sPesquisa", txtStatus.Text.Trim() },
                { "@idStatus", ddlsStatus.SelectedValue }
            };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    // Valida se a Tabela[1] (Gastos) existe
                    if (!BD.ValidarDataSet(dsPesquisa) || dsPesquisa.Tables.Count < 2)
                    {
                        if (btnVerGastos != null) btnVerGastos.Visible = false;
                    }
                    else
                    {
                        // Filtra os gastos (Tabela[1]) para este relatório específico
                        DataTable detalhes = dsPesquisa.Tables[1];
                        DataView gastosFiltrados = new DataView(detalhes);
                        gastosFiltrados.RowFilter = $"idDespesas = {idDespesa}";

                        if (gastosFiltrados.Count > 0)
                        {
                            // Associa o Repeater aninhado de Gastos
                            Repeater rptGastos = (Repeater)e.Item.FindControl("rptGastos");
                            rptGastos.DataSource = gastosFiltrados;
                            rptGastos.DataBind();
                        }
                        else
                        {
                            // Se não houver gastos, esconde o botão
                            if (btnVerGastos != null) btnVerGastos.Visible = false;
                        }
                    }

                    // --- Bloco 2: Carregar Histórico (Lógica NOVA) ---
                    HtmlButton btnVerHistorico = (HtmlButton)e.Item.FindControl("btnVerHistorico");
                    Repeater rptHistorico = (Repeater)e.Item.FindControl("rptHistorico");

                    Dictionary<String, String> vParamsHistorico = new Dictionary<string, string>();
                    vParamsHistorico.Add("@sFuncao", "CONSULTAR-HISTORICO");
                    vParamsHistorico.Add("@idDespesas", idDespesa.ToString());

                    DataSet dsHistorico = BD.ExecutarDataSet(sProcedure, vParamsHistorico);

                    if (BD.ValidarDataSet(dsHistorico) && dsHistorico.Tables[0].Rows.Count > 0)
                    {
                        rptHistorico.DataSource = dsHistorico.Tables[0];
                        rptHistorico.DataBind();
                    }
                    else
                    {
                        // Esconde o botão de histórico se não tiver nada
                        if (btnVerHistorico != null) btnVerHistorico.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Entregas.MostraMensagem_Erro($"Erro no ItemDataBound: {ex.Message}");
            }
        }
        #endregion

        #region | Métodos do Histórico (Timeline)

        /// <summary>
        /// Método auxiliar para o Repeater definir a cor da bolinha da timeline
        /// </summary>
        protected string GetStatusClass(object sCor)
        {
            if (sCor == null || sCor == DBNull.Value) return "status-primary";
            return "status-" + sCor.ToString();
        }

        /// <summary>
        /// Método auxiliar para formatar o motivo (para o Literal)
        /// </summary>
        protected string GetMotivo(object sDscMotivo)
        {
            if (sDscMotivo == null || sDscMotivo == DBNull.Value) return "";

            string motivo = sDscMotivo.ToString();

            if (!motivo.StartsWith("<b>"))
            {
                // Formata como um parágrafo simples se for texto puro (ex: rejeição)
                return "<br/><p>" + HttpUtility.HtmlEncode(motivo) + "</p>";
            }

            // Se for HTML (dos itens), retorna como está
            return motivo;
        }

        #endregion
    }
}