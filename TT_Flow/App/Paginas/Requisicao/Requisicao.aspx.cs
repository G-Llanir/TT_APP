using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;

namespace TT_Hub.App.Paginas.Requisicao
{
    public partial class Requisicao : Page
    {
        string sTituloPagina = "Requisições";
        string sPagina_NovoRegistro = "app/Paginas/Requisicao/Requisicao_Detalhe.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidarPermissaoAcesso();
            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlidStatus, "sp_Select 'Flow_Requisicao_Status'", "idStatus", "sDscStatus", false);
                FUNCOES.Popula_Combo(ddlidTipoRequisicao, "sp_Select 'Flow_Requisicao_Tipo'", "idTipoRequisicao", "sDscTipoRequisicao", false, "Todos os Tipos ", "0");
                FUNCOES.Popula_Combo(ddlidDepartamento, "sp_Select 'Flow_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Todos os Departamentos ", "0");

                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                Pesquisar();
            }
            txtPesquisa.Focus();
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@dtInicio", txtdtInicial.Text },
                { "@dtFinal", txtdtFinal.Text },
                { "@idTipoRequisicao", ddlidTipoRequisicao.SelectedValue },
                { "@idDepartamento", ddlidDepartamento.SelectedValue },
                { "@idStatus", ddlidStatus.SelectedValue },
                { "@idUsuarioRequisicao", IDENTITY.Variaveis.idUsuario() },
                { "@sPesquisa", txtPesquisa.Text.Trim() }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Requisicao", vParametros, false);

            if (tb.Rows.Count > 0)
            {                
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables_Consulta", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 1, new int[1] { 1 }, "desc", "false", "''"), true);

                pnResultado.Visible = true;
            }
            else
                MensagemPagina.MostraMensagem_Erro("Nenhum registro Localizado");
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        =>
            Pesquisar();

        protected void cmdNovo_Click(object sender, EventArgs e)
        =>
            FUNCOES.DirecionaPagina(sPagina_NovoRegistro);

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
        }
    }
}
