using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using System.Web.UI.WebControls;

namespace TT_Hub.App.Paginas.RRHH
{
    public partial class Avaliacao : Page
    {
        string sTituloPagina = "Avaliação";
        string sCaminho = "App/Paginas/RRHH/Avaliacao/";
        string sPagina = "Avaliacao_Detalhe.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.Incluir);

            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlsidDepartamento, "sp_Select 'Flow_Colaboradores_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Todos os Departamentos", "0");
                Pesquisar();

                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Mascaras", "$('[id*=txtsReferencia]').mask('00/0000', { reverse: false });", true);

            FUNCOES.Scripts.FocusScript(Page, txtPesquisa.ClientID);
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;
            pnMensagem.Visible = false;
            try
            {

                if (FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.VisualizarAvaliacoesCadastradas))
                {

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscAvaliacao", txtPesquisa.Text.Trim() },
                { "@sReferencia", txtsReferencia.Text.Trim() },
                { "@sidDepartamento", ddlsidDepartamento.SelectedValue}
            };
                    DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_Avaliacao", vParametros);

                    if (tb.Rows.Count > 0)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 1, "asc", "false", "''"), true);
                        pnResultado.Visible = true;
                    }
                    else
                    {
                        pnMensagem.Visible = true;
                        MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
                    }
                }

            }
            catch (Exception ex)
            {

                pnMensagem.Visible = true;
                MensagemPagina.MostraMensagem_Erro("Erro ao Carregador Dados: " + ex.Message);
            }
        }
        protected void dtgvConsulta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Resultados")
            {
                FUNCOES.DirecionaPagina(string.Format("{0}{1}?id={2}&sTipo=Resultados", sCaminho, sPagina, e.CommandArgument));
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        =>
            Pesquisar();

        protected void cmdNovo_Click(object sender, EventArgs e)
        =>
            FUNCOES.DirecionaPagina(string.Format("{0}{1}?id={2}&sTipo=Novo", sCaminho, sPagina, "0"));
    }
}