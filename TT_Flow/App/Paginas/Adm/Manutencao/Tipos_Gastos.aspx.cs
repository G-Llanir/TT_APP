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
using static TT.FrameWork.BD;
using RETORNO = TT.FrameWork.BD.Retorno;
using static Permissao;

namespace TT_Flow.App.Paginas.Adm.Manutencao
{
    public partial class Tipos_Gastos : System.Web.UI.Page
    {
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Relatorio_Despesas";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
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
            FUNCOES.ValidaPermissao(Permissao.RelatorioDespesas.Consultar, true);
            lblTituloPagina.Text = "Tipos de Despesa";

            hddidRecursoFiltro.Value = ddlidRecursoFiltro.SelectedValue;
            hddidCategoriaPagarfiltro.Value = ddlidCategoriaFiltro.SelectedValue;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_TIPOS" },
                { "@sPesquisa", txtStatus.Text},
                { "@sAtivo", ddlsAtivo.SelectedValue},
                { "@idCategoriaPagar", ddlidCategoriaFiltro.SelectedValue},
                { "@idRecurso", ddlidRecursoFiltro.SelectedValue}
            };
            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa))
            {
                div_gvRelatorio.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", GRID.DataBindComScriptData(gvRelatorio, dsPesquisa, 0, new int[1] { 5 }, "desc", "false", "''"), true);
                PopularCombos();
            }
            else
            {
                div_gvRelatorio.Visible = false;
            }
        }

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidCategoriaFiltro, "sp_Select 'tbl_Flow_Adm_Contas_Pagar_Categoria'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Todas Categorias do Contas a Pagar", "0");
            FUNCOES.Popula_Combo(ddlidRecursoFiltro, $"{sProcedure} 'FLOW-RECURSOS'", "idRecurso", "sDscRecurso", false, "Todos Recursos de Vendas", "0");


            if (string.IsNullOrEmpty(hddidRecursoFiltro.Value) || hddidRecursoFiltro.Value != "0")
            {
                ddlidRecursoFiltro.SelectedValue = hddidRecursoFiltro.Value;
            }

            if (string.IsNullOrEmpty(hddidCategoriaPagarfiltro.Value) || hddidCategoriaPagarfiltro.Value != "0")
            {
                ddlidCategoriaFiltro.SelectedValue = hddidCategoriaPagarfiltro.Value;
            }
        }
        #endregion

        #region | Script
        void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("$('[id*=txtvlrMaximo]').mask('000.000.000.000.000,00', { reverse: true });");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }
        #endregion

        #region | Eventos
        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            PainelAtualizacao.Visible = false;

            FUNCOES.Popula_Combo(ddlidCategoriaPagar, "sp_Select 'tbl_Flow_Adm_Contas_Pagar_Categoria'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Selecione a Categoria do Contas a Pagar", "0");
            FUNCOES.Popula_Combo(ddlidRecurso, $"{sProcedure} 'FLOW-RECURSOS'", "idRecurso", "sDscRecurso", false, "Selecione o Recurso de Vendas", "0");

            txtidTipoGastos.Text = "NOVO";
            txtsDscGasto.Text = "";
            ddlidCategoriaPagar.SelectedValue = "0";
            ddlidRecurso.SelectedValue = "0";
            txtvlrMaximo.Text = "";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModal", "$('#modal_Tipos').modal('show');", true);
        }

        protected void gvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[3].Text == "S")
                    e.Row.Cells[3].Text = "Sim";
                else if (e.Row.Cells[3].Text == "N")
                    e.Row.Cells[3].Text = "Não";
            }
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidarTipo())
                {
                    string sErro = "";
                    string idTipoGastos = "";
                    if (txtidTipoGastos.Text != "NOVO")
                        idTipoGastos = txtidTipoGastos.Text;

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "INCLUIR_TIPOS" },
                        { "@idTipoGastos", idTipoGastos},
                        { "@sDscObservacao", txtsDscGasto.Text},
                        { "@idCategoriaPagar", ddlidCategoriaPagar.SelectedValue},
                        { "@idRecurso", ddlidRecurso.SelectedValue},
                        { "@nValor", txtvlrMaximo.Text.Replace(".", "").Replace(",", ".")},
                        { "@sAtivo", ddlComboAtivo.Situacao_Recuperar()},
                        { "@idUsuario", IDENTITY.Variaveis.idUsuario()}
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidTipoGastos.Value = RETORNO.DATASET(dsPesquisa, 0, "idTipoGastos");
                        //PesquisarTipos(hddidTipoGastos.Value);
                        MensagemPagina.MostraMensagem_Sucesso("Tipo de gasto salvo com sucesso!");
                        Pesquisar();
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModal", "$('#modal_Tipos').modal('show');", true);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        void PesquisarTipos(string idTipoGastos)
        {
            string sErro = "";
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_TIPOS" },
                { "@idTipoGastos", idTipoGastos},
                { "@sAtivo", ddlsAtivo.SelectedValue}
            };
            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                FUNCOES.Popula_Combo(ddlidCategoriaPagar, "sp_Select 'tbl_Flow_Adm_Contas_Pagar_Categoria'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Selecione a Categoria", "0");

                txtidTipoGastos.Text = RETORNO.DATASET(dsPesquisa, 0, "idTipoGastos");
                txtsDscGasto.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscGasto");
                ddlidCategoriaPagar.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCategoria");
                ddlidRecurso.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idRecurso");
                txtvlrMaximo.Text = RETORNO.DATASET(dsPesquisa, 0, "vlrMaximo");
                ddlComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sAtivo"));
                PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario"));

                PainelAtualizacao.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModal", "$('#modal_Tipos').modal('show');", true);
            }
        }

        private bool ValidarTipo()
        {
            string sMensagem = "";
            if (txtsDscGasto.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva a Descrição!";
            }
            if (ddlidCategoriaPagar.SelectedValue == "0")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione a Categoria a Pagar!";
            }
            if (ddlidRecurso.SelectedValue == "0")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Recurso!";
            }
            if (txtvlrMaximo.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva o Valor Maximo!";
            }

            if (sMensagem != "")
            {
                MensagemPagina1.MostraMensagem_Erro(sMensagem);
                return false;
            }
            else
            {
                return true;

            }
        }

        protected void gvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Pesquisar")
            {
                string idTipoGastos = e.CommandArgument.ToString();
                PesquisarTipos(idTipoGastos);
            }
        }
        #endregion
    }
}