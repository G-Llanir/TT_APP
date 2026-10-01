using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using System.Web.UI.WebControls;
using TT_Flow.App.Controles;

namespace TT_Flow.App.Paginas.Manutencao.Certificados
{
    public partial class Certificados : Page
    {
        string sTituloPagina = "Certificados";
        string sPagina_NovoRegistro = "app/Paginas/Qualidade/Certificados_Detalhe.aspx";

        #region | Funções Incialização do Form

        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-Certificados.pdf";

            FUNCOES.ValidaPermissao(Permissao.Certificados.PaginaCertificado, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Certificados.IncluirCertificado);

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
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sNome", txtPesquisa.Text.Trim() }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Certificados", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 1, new int[2] { 5, 6 }, "asc", "false", "''"), true);
                pnResultado.Visible = true;
            }
            else
                MensagemPagina.MostraMensagem_Erro("Nenhum Lançamento Localizado");
        }

        #endregion

        #region | Eventos

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        =>
            Pesquisar();

        protected void cmdNovo_Click(object sender, EventArgs e)
        =>
            FUNCOES.DirecionaPagina(sPagina_NovoRegistro);

        protected void cmdRepetir_click(object sender, EventArgs e)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "DUPLICAR-ANO" }
            };
            BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Certificados", vParametros, false);

            Pesquisar();
        }

        #endregion

        #region | dtgvConsulta

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                (e.Row.Cells[5].Controls[0] as HyperLink).Text = DateTime.Parse((e.Row.Cells[5].Controls[0] as HyperLink).Text).ToString("dd/MM/yyyy");
                (e.Row.Cells[6].Controls[0] as HyperLink).Text = DateTime.Parse((e.Row.Cells[6].Controls[0] as HyperLink).Text).ToString("dd/MM/yyyy");
            }
        }

        #endregion

        #region | Script

        void RegistraScriptYear()
        =>
            ScriptManager.RegisterStartupScript(this, this.GetType(), "DatePickerScript", "<script>\r\n$(document).ready(function () {\r\n$('#txtsAno').datepicker({\r\nformat: ' yyyy',\r\nviewMode: 'years',\r\nminViewMode: 'years'\r\n});\r\n});\r\n</script>", false);

        #endregion

    }
}