using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.App.Controles;
using static TT.FrameWork.BD;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Grid;

namespace TT_Flow.App.Paginas.Adm.Faturamento
{
    public partial class EmissaoNFE : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-ImportadordeNF-e.pdf";
            ValidaPermissao(Permissao.Administracao.EmissaoNFe.Consultar, true);

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = "Emissor NF-e";
                pnResultado.Visible = false;

                Pesquisar("");
            }

            txtPesquisa.Focus();
        }

        protected void Pesquisar(string sPesquisa)
        {
            try
            {
                pnResultado.Visible = false;
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR" },
                    { "@sPesquisa", sPesquisa }
                };
                DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_EmissaoNFE", vParametros);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "DataTables", DataBindComScriptData(dtgvConsulta, ds, 0, new int[1] { 3 }, "desc", "false", "false"), true);
                    pnResultado.Visible = true;
                }
                else MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado!");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.ToString());
            }
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
           
            EsconderColunas(e, 0, 7, 8, 9, 10);
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar(txtPesquisa.Text);

        protected void cmdNovo_Click(object sender, EventArgs e) => DirecionaPagina("App/Paginas/Adm/Faturamento/EmissaoNFE_Detalhe.aspx");
    }
}