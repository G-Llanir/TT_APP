using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using System.Web.UI.WebControls;
using TT_Flow.FrameWork;
using TT.FrameWork;
using TT_Flow.App.Controles;

namespace TT_Flow.App.Paginas.Manutencao.Empresas
{
    public partial class Empresas : System.Web.UI.Page
    {
        string sTituloPagina = "Empresas";
        string sPagina_NovoRegistro = "app/Paginas/Manutencao/Empresas_Detalhe.aspx";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Empresas.PaginaEmpresas, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Empresas.IncluirEmpresas);

            manual.sNomeArquivo = "Manual-Empresas.pdf";

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                Pesquisar();
                RegistraScriptYear();
            }
            txtPesquisa.Focus();
            RegistraScriptYear();
        }
        #endregion

        #region | Metodos Banco de Dados
        protected void Pesquisar()
        {
            string sFuncao = "CONSULTAR";
            string sDscPesquisa = "";
            sDscPesquisa = txtPesquisa.Text.Trim();
            DataTable tb;
            string sSql = "sp_Manipula_tbl_Flow_Empresas";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@sDscEmpresa", sDscPesquisa);
            vParametros.Add("@TipoPais", ddlPais.SelectedValue);

            tb = BD.ExecutarDataTable(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb, 0, "asc"), true);

                pnResultado.Visible = true;
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhum Lançamento Localizado");
            }
        }
        #endregion

        #region | CMD Click
        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina(sPagina_NovoRegistro);
        }

        protected void cmdRepetir_click(object sender, EventArgs e)
        {
            string sFuncao = "DUPLICAR-ANO";

            DataTable tb;
            string sSql = "sp_Manipula_tbl_Flow_Empresas";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            tb = BD.ExecutarDataTable(sSql, vParametros, false);
            Pesquisar();
        }
        #endregion

        #region | dtgvConsulta
        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }
        }
        #endregion

        #region | Script
        void RegistraScriptYear()
        {

            ScriptManager.RegisterStartupScript(this, this.GetType(), "DatePickerScript", @"
        <script>
            $(document).ready(function () {
                $('#txtsAno').datepicker({
                    format: ' yyyy', // Notice the Extra space at the beginning
                    viewMode: 'years',
                    minViewMode: 'years'
                });
            });
        </script>", false);

        }
        #endregion

    }
}