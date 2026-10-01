using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using static TT.FrameWork.Identity;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.BD;
using static TT.FrameWork.Grid;

namespace TT_Flow.App.Paginas.Comercial
{
    public partial class Orcamento : Page
    {
        string sProcedure = "sp_Consulta_tbl_Flow_Pedidos";
        string sDashboard = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            ValidaPermissao(Permissao.Comercial.Orcamento.Consultar, true);
            cmdNovo.Visible = ValidaPermissao(Permissao.Comercial.Orcamento.Incluir, false);

            manual.sNomeArquivo = "Manual-Orcamento.pdf";

            lblTituloPagina.Text = "Orçamentos";
            BreadCrumb_Pagina.TitulodaPagina = "Orçamentos";

            if (!IsPostBack)
            {
                resultado.Visible = false;
                PopulaCombos();
            }

            if (!string.IsNullOrEmpty(Request.QueryString["dashboard"]))
            {
                sDashboard = Request.QueryString["dashboard"];
                Pesquisar();
            }
        }

        protected void Pesquisar()
        {
            string permissao = "N";
            bool bNivel_1 = true;

            if (ValidaPermissao(Permissao.Comercial.Orcamento.Nivel_1, false)) permissao = "N";
            if (ValidaPermissao(Permissao.Comercial.Orcamento.Nivel_2, false)) bNivel_1 = false;
            if (ValidaPermissao(Permissao.Comercial.Orcamento.Nivel_3, false))
            {
                permissao = "";
                bNivel_1 = false;
            }

            if (bNivel_1)
                div_Vendedores.Visible = false;

            if (permissao != "")
                EsconderColunas(gvConsulta, "Confidencial");

            Dictionary<string, string> vParametros = new Dictionary<string, string> { { "@sFuncao", "PESQUISA" }, { "@idTipo", "1" } };
            if (sDashboard == "")
            {
                vParametros.Add("@sPesquisa", txtPesquisa.Text);
                vParametros.Add("@idCliente", ddlidCliente.SelectedValue);
                vParametros.Add("@idTipoOrcamento", ddlTipoOrcamento.SelectedValue);
                vParametros.Add("@idFluxo", ddlFluxoOrcamento.SelectedValue);
                vParametros.Add("@idUsuario", Variaveis.idUsuario());
                vParametros.Add("@dtFiltroAno", ddlFiltro_Ano.SelectedValue == "0" ? "0" : ddlFiltro_Ano.SelectedItem.Text);
                vParametros.Add("@idVendedor", bNivel_1 ? Variaveis.idUsuario() : ddlVendedores.SelectedValue);
                vParametros.Add("@sConfidencial", permissao);
                vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                vParametros.Add("@idStatus", ddlStatus.SelectedValue);
                vParametros.Add("@idFiltro_Revisao", ddlRevisoes.SelectedValue);
                vParametros.Add("@sFiltro_Nacionalidade", ddlNacionalidade.SelectedValue);
            }
            else
            {
                vParametros.Add("@sDashboard", sDashboard);
                vParametros.Add("@sidFluxos", Request["idFluxo"] ?? "0");
                vParametros.Add("@idUsuario", Variaveis.idUsuario());
                vParametros.Add("@sidVendedores", Request["idVendedor"] ?? "0");
                vParametros.Add("@sidEmpresas", Request["idEmpresa"] ?? "0");
                vParametros.Add("@idStatus", Request["idStatus"] ?? "0");
                vParametros.Add("@dtInicio", Request["dtInicio"]);
                vParametros.Add("@dtFinal", Request["dtFinal"]);

                div_FiltroPesquisa.Visible = false;
            }
            DataSet ds = ExecutarDataSet(sProcedure, vParametros);

            if (ValidarDataSet(ds, out string sErro))
            {
                resultado.Visible = true;
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ConsultaOrcamento", DataBindComScriptData(gvConsulta, ds.Tables[0], 0, new int[1] { 4 }, "desc", "false", "''"), true);
            }
            else
            {
                resultado.Visible = false;
                MensagemPagina.MostraMensagem_Erro("Nenhum registro encontrado!");
            }
        }

        protected void PopulaCombos()
        {
            Popula_Combo(ddlidCliente, "sp_Select 'Flow_Clientes'", "idCliente", "sRazaoSocial", false, "Todos os Clientes", "0");
            Popula_Combo(ddlTipoOrcamento, "sp_Select 'tbl_Flow_Comercial_Orcamento_Tipo'", "idTipoOrcamento", "sDscTipoOrcamento", false, "Todos os Tipos", "0");
            Popula_Combo(ddlFluxoOrcamento, "sp_Select 'FLOW_Fluxo'", "idFluxo", "sDscFluxo", false, "Todos os Fluxos", "0");
            Popula_Combo(ddlVendedores, "sp_Select 'FLOW_Vendedores'", "idUsuario", "sDscUsuario", false, "Todos os Vendedores", "0");
            Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @idUSuario=" + Variaveis.idUsuario(), "idParceiro", "sDscEmpresa", false, "Todas as Empresas", "0");
            Popula_Combo(ddlFiltro_Ano, "sp_Select 'ANOS_PEDIDOS', @idFiltro=1", "Ano", "Ano", false, "Todos os Anos", "0");
            Popula_Combo(ddlStatus, "sp_Select 'Flow_Status', @idPesquisa=1, @idFiltro=1", "idStatus", "sDscStatus", false, "Todos os Status", "0");

            ddlStatus.SelectedIndex = 2;
            ddlRevisoes.SelectedIndex = 1;
        }

        protected void gvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[e.Row.Cells.Count - 1].Text.ToUpper().Contains(" - EMPREITADA"))
                {
                    e.Row.Cells[e.Row.Cells.Count - 2].Controls[1].Visible = false;
                    e.Row.Cells[e.Row.Cells.Count - 3].Controls[1].Visible = false;
                }
                else if (!(e.Row.FindControl("hddidPedido_Vinculado") as HiddenField).Value.Equals("0"))
                    e.Row.Cells[e.Row.Cells.Count - 3].Controls[1].Visible = false;
            }
        }

        protected void gvConsulta_RowCommand(object sender, GridViewCommandEventArgs e) => DirecionaPagina($"App/Paginas/Comercial/Orcamento_Detalhe.aspx?id={e.CommandArgument}&{e.CommandName.Trim().ToLower()}=true");

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();
    }
}