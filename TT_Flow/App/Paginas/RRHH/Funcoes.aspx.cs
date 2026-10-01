using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;

namespace TT_Hub.App.Paginas.RRHH
{
    public partial class Funcoes : Page
    {
        string sTituloPagina = "Funções";
        string sPagina_NovoRegistro = "app/Paginas/RRHH/Funcoes_Detalhe.aspx?id=0";
        int idColuna_Salario = 6;

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.RRHH.FuncoesCarteira.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.FuncoesCarteira.Incluir);

            if (!IsPostBack)
            {


                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                PopularCombo();
                if (Request["idGHE"] != null)
                {
                    ddlidGHE.SelectedValue = Convert.ToInt32(Request["idGHE"].ToString()).ToString();
                    lblSubTituloPagina.Text = " GHE " + ddlidGHE.SelectedItem.Text;
                    DIV_Filtro.Visible = false;
                }
                Pesquisar();
            }

            txtPesquisa.Focus();
        }

        protected void PopularCombo()
        {
            FUNCOES.Popula_Combo(ddlsCBO, "sp_Select 'Flow_Colaboradores_FuncaoCarteira_sCBO'", "sCBO", "sCBO", false, "Todos os CBO", "0");
            FUNCOES.Popula_Combo(ddlidDepartamento, "sp_Select 'Flow_Colaboradores_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Todos os Departamentos ", "0");
            FUNCOES.Popula_Combo(ddlidTipoContrato, "sp_Select 'tbl_Flow_Colaboradores_TipoContrato'", "idTipoContrato", "sDscTipoContrato", false, "Todos os Tipos de Contratos", "0");
            FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Todas as Empresas", "0");
            PopularCombo_GHE("0");


        }
        protected void PopularCombo_GHE(string idEmpresa)
        {
            FUNCOES.Popula_Combo(ddlidGHE, "sp_Manipula_tbl_Flow_Colaboradores_GHE 'SELECT_GHE', @sidEmpresa=" + idEmpresa, "idGHE", "sDscGHE", false, "Todos os GHEs", "0");
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;
            pnMensagem.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscFuncao", txtPesquisa.Text.Trim() },
                { "@idDepartamento", ddlidDepartamento.SelectedValue },
                { "@idEmpresa", ddlidEmpresa.SelectedValue },
                { "@idTipoContrato", ddlidTipoContrato.SelectedValue },
                { "@idGHE", ddlidGHE.SelectedValue },
                { "@sCBO", ddlsCBO.SelectedValue },

                { "@idTipoFuncao", ddlTipoFuncao.SelectedValue },
                { "@sDuplaFuncaoAtiva", ddlDuplaFuncao.SelectedValue },
                { "@sPericulosidadeAtiva", ddlPericulosidade.SelectedValue },
                { "@sNoturnidadeAtiva", ddlNoturnidade.SelectedValue }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_FuncaoCarteira", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                pnResultado.Visible = true;

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb), true);

                if (FUNCOES.ValidaPermissao(Permissao.RRHH.FuncoesCarteira.ConsultarDadosNívelII))
                    dtgvConsulta.Columns[idColuna_Salario].Visible = true;
                else
                    dtgvConsulta.Columns[idColuna_Salario].Visible = false;
            }
            else
            {
                pnMensagem.Visible = true;
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void cmdNovo_Click(object sender, EventArgs e) => FUNCOES.DirecionaPagina(sPagina_NovoRegistro);

        protected void ddlidEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularCombo_GHE(ddlidEmpresa.SelectedValue);
        }
    }
}