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

namespace TT_Flow.App.Paginas.WMS.Manutencao
{
    public partial class Unidades : System.Web.UI.Page
    {
        string sTituloPagina = "Unidades";
        string sPagina_NovaUnidade = "App/Paginas/WMS/Manutencao/Unidades_Detalhe.aspx";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-Unidades.pdf";

            FUNCOES.ValidaPermissao(Permissao.WMS.Cadastro_Unidades.Consultar, true);

            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.WMS.Cadastro_Unidades.Incluir);

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
            string sDscUnidade = "";
            sDscUnidade = txtPesquisa.Text.Trim();
            DataTable tb;
            string sSql = "sp_Manipula_tbl_Flow_Produtos_Unidade";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@sDscUnidade", sDscUnidade);

            tb = BD.ExecutarDataTable(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb, 0, "desc"), true);

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
            FUNCOES.DirecionaPagina(sPagina_NovaUnidade);
        }

        protected void cmdRepetir_click(object sender, EventArgs e)
        {
            string sFuncao = "DUPLICAR-ANO";

            DataTable tb;
            string sSql = "sp_Manipula_tbl_Flow_Produtos_Unidade";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            tb = BD.ExecutarDataTable(sSql, vParametros, false);
            Pesquisar();
        }
        #endregion

        #region | dtgvConsulta
        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            
        }
        #endregion

        #region | Script
        void RegistraScriptYear()
        {

        }
        #endregion

    }
}