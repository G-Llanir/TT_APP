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

namespace TT_Flow.App.Paginas.Comercial.Manutencao
{
    public partial class Segmentos : System.Web.UI.Page
    {
        string sTituloPagina = "Segmentos";
        string sPagina_NovoRegistro = "app/Paginas/Comercial/Manutencao/Segmentos_Detalhe.aspx";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-Segmentos.pdf";

            FUNCOES.ValidaPermissao(Permissao.Segmentos.PaginaSegmentos, true);

            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Segmentos.IncluirSegmentos);

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
            string sSql = "sp_Manipula_tbl_Flow_Segmentos_TipoCliente";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@sDscSegmento_TipoCliente", sDscPesquisa);

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
            FUNCOES.DirecionaPagina(sPagina_NovoRegistro);
        }

        protected void cmdRepetir_click(object sender, EventArgs e)
        {
            string sFuncao = "DUPLICAR-ANO";

            DataTable tb;
            string sSql = "sp_Manipula_tbl_Flow_Segmentos";
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