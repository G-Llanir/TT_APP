using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using TT_Flow.App.Controles;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using GRID = TT.FrameWork.Grid;
using IDENTITY = TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.Adm.Faturamento
{
    public partial class ImportadorNFE : System.Web.UI.Page
    {
        string sTituloPagina = "NFe Importação";
        string sPagina_NovoRegistro = "app/Paginas/Adm/Faturamento/ImportadorNFE_Detalhe.aspx";

        List<string> itensSelecionados = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-ImportadordeNF-e.pdf";
            FUNCOES.ValidaPermissao(Permissao.Financeiro.ImportadorNFe.Consultar, true);

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                PopularCombos();
                itensSelecionados.Clear();
                Pesquisar("");

                if (Request["msg"] != null && Request["msg"] == "1")
                {
                    MensagemPagina.MostraMensagem_Sucesso("NFe excluído com sucesso!");
                }
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
                    { "@idParceiro_Emit", ddlsEmitente.SelectedValue },
                    { "@idParceiro_Dest", ddlidParceiro.SelectedValue },
                    { "@sDscStatus", ddlsStatus.SelectedValue },
                    { "@sPesquisa", sPesquisa }
                };

                DataSet dsArquivos = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ImportadorNfe", vParametros);

                if (dsArquivos.Tables[0].Rows.Count > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, dsArquivos.Tables[0], 1, "DESC", "false", "false"), true);
                    pnResultado.Visible = true;
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Nenhuma OPI Localizada");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.ToString());
            }
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();

                Literal litLinkAdiantados = e.Row.FindControl("litLinkAdiantados") as Literal;
                if (litLinkAdiantados != null)
                {
                    DataRowView rowView = (DataRowView)e.Row.DataItem;
                    string sidsContasReceber = rowView["sidsContasReceber"].ToString();
                    if ((!string.IsNullOrEmpty(sidsContasReceber) && sidsContasReceber != DBNull.Value.ToString()))
                    {
                        litLinkAdiantados.Text = sidsContasReceber;
                    }
                    else
                    {
                        litLinkAdiantados.Text = string.Empty;
                    }
                }
            }

            GRID.EsconderColunas(e, 0);            
        }

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlsEmitente, "sp_Select 'Flow_Clientes'", "idCliente", "sCPNJ_RazaoSocial", false, "Todos os Emitentes", "0");
            FUNCOES.Popula_Combo(ddlidParceiro, "sp_Select 'Flow_Clientes'", "idCliente", "sCPNJ_RazaoSocial", false, "Todos os Parceiros", "0");
        }

        protected void cmdXMLDownload_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string arquivoId = btn.CommandArgument;
            string sNomeArquivo = "";
            byte[] bObjArquivo = null;

            Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
            DataTable dtArquivo;
            vParametrosItem = new Dictionary<string, string>
            {
                {"@sFuncao",                    "CONSULTAR_DETALHE" },
                {"@idArquivo",                  arquivoId}
            };

            dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

            foreach (DataRow item in dtArquivo.Rows)
            {
                try
                {
                    sNomeArquivo = item["sNomeArquivo"].ToString();
                    bObjArquivo = (byte[])item["vbArquivo"];
                }
                catch
                {

                }

                TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                FileStream lObjFile = objArquivo.TransformarArrayBytesEmArquivo(bObjArquivo, Server.MapPath("~/Download/" + sNomeArquivo));
                lObjFile.Close();
                lObjFile.Dispose();

                FUNCOES.DirecionaPagina_NovaAba(Page, "/Download/" + sNomeArquivo);
            }
            Pesquisar("");
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar(txtPesquisa.Text);
        }

        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina("App/Paginas/Adm/Faturamento/ImportadorNFE_Detalhe.aspx?Novo=S");
        }
    }
}