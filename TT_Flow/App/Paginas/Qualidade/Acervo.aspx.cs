using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using static TT.FrameWork.BD;
using Funcoes = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.Qualidade
{
    public partial class Acervo : Page
    {
        string sTituloPagina = "Acervo";

        protected void Page_Init(object sender, EventArgs e)
        {
            Funcoes.Popula_Combo(ddlidParceiro, "sp_Select 'tbl_Flow_Clientes'", "idCliente", "sRazaoSocial", false, "Todos os Parceiros", "0");
            Funcoes.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa'", "idEmpresa", "sDscEmpresa", false, "Todas as Empresas", "0");
            Funcoes.Popula_Combo(ddlidFluxo, "sp_Select 'Flow_Fluxo'", "idFluxo", "sDscFluxo", false, "Todos os Fluxos", "0");
            Pesquisar();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Funcoes.ValidaPermissao(Permissao.Qualidade.Acervo.Consultar, true);

            manual.sNomeArquivo = "Manual_Acervo.pdf";

            if (!IsPostBack)
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
        }

        private void Pesquisar()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA_ACERVO" },
                { "@dtInicial", txtdtInicial.Text },
                { "@dtFinal", txtdtFinal.Text },
                { "@idEmpresa", ddlidEmpresa.SelectedValue },
                { "@idCliente", ddlidParceiro.SelectedValue },
                { "@idFluxo", ddlidFluxo.SelectedValue },
                { "@sReferencia", txtsDescricao.Text }
            };
            DataTable tb = ExecutarDataTable("sp_Manipula_tbl_Flow_Pedidos", vParametros, false);

            AdicionaColuna(tb);

            if (tb.Rows.Count > 0)
            {
                pnResultado.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvAcervo, tb, 1, new int[1] { 2 }, "desc", "false", "''"), true);
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhum registro encontrado!");
                pnResultado.Visible = false;
            }
        }

        private void AdicionaColuna(DataTable tb)
        {
            foreach (DataColumn coluna in tb.Columns)
            {
                if (coluna.ColumnName != "idPedido" && coluna.ColumnName != "sReferencia" && coluna.ColumnName != "sRazaoSocial" && coluna.ColumnName != "nValor" && coluna.ColumnName != "idArquivo" && coluna.ColumnName != "sNomeArquivo" && coluna.ColumnName != "sDscEmpresa" && coluna.ColumnName != "dtConclusao" && coluna.ColumnName != "nNumeroPedido")
                {
                    BoundField bf = new BoundField();
                    bf.DataField = coluna.ColumnName;
                    bf.HeaderText = coluna.ColumnName;
                    bf.HeaderStyle.Width = 15;
                    dtgvAcervo.Columns.Add(bf);
                }

                if (coluna.ColumnName == "idArquivo")
                {
                    TemplateField tf = new TemplateField();
                    tf.HeaderText = "Arquivo";
                    tf.HeaderStyle.Width = Unit.Percentage(5);
                    tf.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                    tf.ItemStyle.VerticalAlign = VerticalAlign.Middle;
                    tf.ItemStyle.CssClass = "download";

                    tf.ItemTemplate = new DownloadTemplate();
                    dtgvAcervo.Columns.Add(tf);
                }
            }
        }

        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            RemoverColuna();
            Pesquisar();
        }

        private void RemoverColuna()
        {
            List<DataControlField> colunasParaRemover = new List<DataControlField>();

            foreach (DataControlField coluna in dtgvAcervo.Columns)
            {
                if (coluna is BoundField bf && bf.DataField != "sRazaoSocial" && bf.DataField != "nValor" && bf.DataField != "sNomeArquivo" && bf.DataField != "sDscEmpresa" && bf.DataField != "dtConclusao" && bf.DataField != "nNumeroPedido")
                    colunasParaRemover.Add(coluna);

                if (coluna is TemplateField tf && tf.HeaderText == "Arquivo")
                    colunasParaRemover.Add(coluna);
            }

            foreach (var coluna in colunasParaRemover)
            {
                dtgvAcervo.Columns.Remove(coluna);
            }
        }

        protected void dtgvAcervo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 4; i < e.Row.Cells.Count - 1; i++)
                {
                    e.Row.Cells[i].Text = HttpUtility.HtmlDecode(e.Row.Cells[i].Text);
                }
            }
        }

        protected void dtgvAcervo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Download")
            {
                string idArquivo = e.CommandArgument.ToString();
                byte[] bObjArquivo;
                string urlAtualPagina = Request.UrlReferrer.ToString().Replace(Request.RawUrl, "/Download/");

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idArquivo", idArquivo }
                };
                DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParametros);

                if (ValidarDataSet(ds))
                {
                    try
                    {
                        string sNomeArquivo = Retorno.DATASET(ds, 0, "sNomeArquivo");
                        DataRow acervo = ds.Tables[0].Rows[0];
                        bObjArquivo = (byte[])acervo["vbArquivo"];

                        Arquivo objArquivo = new Arquivo();
                        FileStream lObjFile = objArquivo.TransformarArrayBytesEmArquivo(bObjArquivo, Server.MapPath("~/Download/" + sNomeArquivo));
                        lObjFile.Close();
                        lObjFile.Dispose();

                        StringBuilder strDownload = new StringBuilder();
                        strDownload.AppendLine("var link = document.createElement('a');");
                        strDownload.AppendLine("link.download = '" + sNomeArquivo + "';");
                        strDownload.AppendLine("link.href = '" + string.Concat(urlAtualPagina, sNomeArquivo) + "';");
                        strDownload.AppendLine("link.click();");
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Download_dArquivos", strDownload.ToString(), true);
                    }
                    catch
                    {
                        MensagemPagina.MostraMensagem_Erro("Houve um erro no documento!");
                    }
                }
                else
                    MensagemPagina.MostraMensagem_Erro("Não foi encontrado documento!");
            }

            btnPesquisar_Click(btnPesquisar, new EventArgs());
        }

        public class DownloadTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                LinkButton lb = new LinkButton();
                lb.ID = "lbAcervoDownload";
                lb.CssClass = "fa fa-download";
                lb.CommandName = "Download";
                lb.DataBinding += new EventHandler(lb_DataBinding);
                container.Controls.Add(lb);
            }

            private void lb_DataBinding(object sender, EventArgs e)
            =>
                (sender as LinkButton).CommandArgument = DataBinder.Eval(((sender as LinkButton).NamingContainer as GridViewRow).DataItem, "idArquivo").ToString();
        }
    }
}