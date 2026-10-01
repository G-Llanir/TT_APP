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

namespace TT_Flow.App.Paginas.Comercial
{
    public partial class Escopo : System.Web.UI.Page
    {
        string sTituloPagina = "Escopo";
        string sPagina_NovoRegistro = "App/Paginas/Comercial/Escopo_Detalhe.aspx?id=0";

        #region | Page_Load + Pesquisar
        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-Escopo.pdf";

            FUNCOES.ValidaPermissao(Permissao.Comercial.Escopo.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Comercial.Escopo.Incluir);

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                Pesquisar();
            }
        }

        protected void Pesquisar()
        {
            string sFuncao = "CONSULTA";
            string sDscPesquisa = txtPesquisa.Text.Trim();
            string sSql = "sp_Manipula_tbl_Flow_Escopo";
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@sDscPesquisa", sDscPesquisa);

            DataTable tb = BD.ExecutarDataTable(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb, 0, "desc"), true);
                pnResultado.Visible = true;
            }
            else
            {
                pnResultado.Visible = false;
                MensagemPagina.MostraMensagem_Erro("Nenhum Registro Localizado!");
            }
        }
        #endregion

        #region | Eventos
        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina(sPagina_NovoRegistro);
        }
        #endregion

    }
}