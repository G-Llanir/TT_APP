using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;

namespace TT_Flow.App.Paginas.COMEX.Manutencao
{
    public partial class Ato_Concessorio : System.Web.UI.Page
    {
        string sTituloPagina = "Ato Concessório";

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FUNCOES.ValidaPermissao(Permissao.Comex.Manutenção_de_Dados.AtoConcessorio.Consultar, true);
                Pesquisar();
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Comex.Manutenção_de_Dados.AtoConcessorio.Incluir);
            }
        }
        #endregion

        #region Pesquisar
        void Pesquisar()
        {
            string sFuncao = "CONSULTAR";
            pnResultado.Visible = false;
           
            DataTable tb;
            string sSql = "sp_Manipula_tbl_Flow_Comex_AtoConcessorio";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@sDscPesquisa", txtPesquisa.Text);

            tb = BD.ExecutarDataTable(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                GRID.DataBind(gvAtoConcessorio, tb);
                pnResultado.Visible = true;
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
            }
        }
        #endregion

        #region Pesquisar_Click
        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }
        #endregion

        #region Novo_Click
        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina("app/Paginas/COMEX/Manutencao/Ato_Concessorio_Detalhe.aspx?id=0");
        }
        #endregion

    }
}