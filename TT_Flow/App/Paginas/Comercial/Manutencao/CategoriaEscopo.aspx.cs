using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using System.Web.UI.WebControls;
using TT_Flow.FrameWork;
using TT.FrameWork;

namespace TT_Flow.App.Paginas.Comercial.Manutencao
{
    public partial class CategoriaEscopo : System.Web.UI.Page
    {
        string sTituloPagina = "Categoria";
        string sPagina_NovoRegistro = "App/Paginas/Comercial/Manutencao/CategoriaEscopo_Detalhe.aspx?id=0";

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
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_CATEGORIAS");
            vParametros.Add("@sDscPesquisa", txtPesquisa.Text.Trim());

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Escopo", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb, 0, "asc"), true);
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

        protected void dtgvConsulta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Duplicar")
                FUNCOES.DirecionaPagina(string.Format("App/Paginas/Comercial/Manutencao/CategoriaEscopo_Detalhe.aspx?id={0}&duplicar=true", e.CommandArgument));
        }
        #endregion
    }
}