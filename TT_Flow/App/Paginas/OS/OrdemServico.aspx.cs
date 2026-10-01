using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using TT.FrameWork;
using VALIDACOES = TT.FrameWork.Validacoes;

namespace TT_Flow.App.Paginas.OS
{
    public partial class OrdemServico : System.Web.UI.Page
    {
        // string sTituloPagina = "Ordem de Serviço";
        // string sPagina_NovoRegistro = "app/Paginas/OrdemServico/OrdemServico_Detalhe.aspx";

        string sTituloPagina = "Ordem Serviço";
        string sPagina_NovoRegistro = "app/Paginas/OS/OrdemServico_Detalhe.aspx?id=0";


        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidarPermissaoAcesso();
            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlidTipoOrdemServico, "sp_Select 'Flow_OrdemServico_Tipo'", "idTipoOrdemServico", "sDscTipoOrdemServico", false, "Todos os Tipos ", "0");
                FUNCOES.Popula_Combo(ddlidDepartamento, "sp_Select 'Flow_Colaboradores_Departamentos_OS'", "idDepartamento", "sDscDepartamento", false, "Todos os Departamentos ", "0");
                FUNCOES.Popula_Combo(ddlidStatus, "sp_Select 'Flow_OrdemServico_Status'", "idStatus", "sDscStatus", false);
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                
            }
            Pesquisar();
            txtPesquisa.Focus();
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;


            string sFuncao = "CONSULTAR";
            string sDscPesquisa = "";

            sDscPesquisa = txtPesquisa.Text.Trim();
            DataTable tb;
            string sSql = "sp_Manipula_tbl_Flow_OrdemServico";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@dtInicio", txtdtDataInicio.Text);
            vParametros.Add("@dtFinal", txtdtDataTermino.Text);
            vParametros.Add("@idTipoOrdemServico", ddlidTipoOrdemServico.SelectedValue);
            vParametros.Add("@idDepartamento", ddlidDepartamento.SelectedValue);
            vParametros.Add("@idStatus", ddlidStatus.SelectedValue);
            vParametros.Add("@idSolicitante", IDENTITY.Variaveis.idUsuario());

            tb = BD.ExecutarDataTable(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb, 0, "desc"), true);

                pnResultado.Visible = true;
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhum registro Localizado");
            }
            RegistraScript("");
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
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }
        }

        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$(function() {$('[id*=txtdtDataInicio]').datepicker({");
            sb.Append("autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");

            sb.Append("$(function() {$('[id*=txtdtDataTermino]').datepicker({");
            sb.Append("autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }
    }
}