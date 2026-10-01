using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.FrameWork;
using TT_Hub.App.Paginas.RRHH;
using static Permissao.WMS;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.WMS
{
    public partial class RelatorioMovimentacao : System.Web.UI.Page
    {
        string sTituloPagina = "Relatório de Movimentação";
        string sPagina_NovoRegistro = "app/Paginas/WMS/RelatorioMovimentacao_Detalhe.aspx";

        public List<FrameWork.cls_WMS_ProdutoMovimentacaoRel> ls_MovimentacaoRel
        {
            get
            {
                if (ViewState["ls_MovimentacaoRel"] == null)
                {
                    ViewState["ls_MovimentacaoRel"] = new List<cls_WMS_ProdutoMovimentacaoRel>();
                }
                return (List<cls_WMS_ProdutoMovimentacaoRel>)ViewState["ls_MovimentacaoRel"];
            }
            set
            {
                ViewState["ls_MovimentacaoRel"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.WMS.RelatorioMovimentacao.Consultar, true);
            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                //PopularCombos();
                Pesquisar();
                PopulaCombo();
            }

            txtPesquisa.Focus();
        }
        protected void PopulaCombo()
        {
            FUNCOES.Popula_Combo(ddlTipo, "sp_Select 'Flow_Produtos_Tipo', @idPesquisa=" + 0, "idTipoProduto", "sDscTipoProduto", false, "Todos os Tipos", "0");
            FUNCOES.Popula_Combo(ddlFamilia, "sp_Select 'Flow_WMS_Produtos_Familia'", "idFamilia", "sDscFamilia", false, "Todas as Famílias", "0");
            FUNCOES.Popula_Combo(ddlGrupo, "sp_Select 'Flow_WMS_Produtos_Grupos_PAI'", "idGrupo", "sDscGrupo", false, "Todos os Grupos", "0");
            FUNCOES.Popula_Combo(ddlLocal, "sp_Manipula_tbl_Flow_WMS_Relatorio_Movimentacao 'FLOW-LOCAL'", "idLocalArmazenamento", "sDscLocalArmazenamento", false, "Todos Locais", "0");
        }
        protected void Pesquisar()
        {
            pnResultado.Visible = false;

            string dtFiltro = "";
            string dtFinal = "";
            if (!string.IsNullOrEmpty(txtdtFiltro.Text))
            {
                DateTime dt = DateTime.Parse(txtdtFiltro.Text.ToString(), CultureInfo.InvariantCulture);
                dtFiltro = dt.ToString();
            }
            if (!string.IsNullOrEmpty(txtdtFinal.Text))
            {
                DateTime dt = DateTime.Parse(txtdtFinal.Text.ToString(), CultureInfo.InvariantCulture);
                dtFinal = dt.ToString();
            }
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscProduto", txtPesquisa.Text.Trim() },
                { "@dtFiltro", dtFiltro },
                { "@dtFinal", dtFinal },
                { "@dtInicio", dtFiltro },
                { "@idFamilia", ddlFamilia.SelectedValue },
                { "@idGrupo", ddlGrupo.SelectedValue },
                { "@idTipo", ddlTipo.SelectedValue },
                { "@idLocal", ddlLocal.SelectedValue }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_WMS_Relatorio_Movimentacao", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, tb, 0, new int[1] { 0 }, "asc", "false", "''"), true);
                pnResultado.Visible = true;

                ls_MovimentacaoRel = new List<cls_WMS_ProdutoMovimentacaoRel>();

                foreach (DataRow row in tb.Rows)
                {
                    cls_WMS_ProdutoMovimentacaoRel item = new cls_WMS_ProdutoMovimentacaoRel
                    {
                        IdItem = row["idItem"] != DBNull.Value ? Convert.ToInt32(row["idItem"]) : 0,
                        NEstoqueAtual = row["nEstoqueAtual"] != DBNull.Value ? Convert.ToInt32(row["nEstoqueAtual"]) : 0,
                        SCodigo = row["sCodigo"] != DBNull.Value ? row["sCodigo"].ToString() : string.Empty,
                        SDscProduto = row["sDscProduto"] != DBNull.Value ? row["sDscProduto"].ToString() : string.Empty,
                        SUnidade = row["sUnidade"] != DBNull.Value ? row["sUnidade"].ToString() : string.Empty,
                        SDscFamilia = row["sDscFamilia"] != DBNull.Value ? row["sDscFamilia"].ToString() : string.Empty,
                        SLocal = row["sLocal"] != DBNull.Value ? row["sLocal"].ToString() : string.Empty,
                        NCompras = row["nCompras"] != DBNull.Value ? Convert.ToInt32(row["nCompras"]) : 0,
                        NVendas = row["nVendas"] != DBNull.Value ? Convert.ToInt32(row["nVendas"]) : 0,
                        NEstoqueAnterior = row["nEstoqueAnterior"] != DBNull.Value ? Convert.ToInt32(row["nEstoqueAnterior"]) : 0,
                        DtMovimentacao = row["dtMovimentacao"] != DBNull.Value ? Convert.ToDateTime(row["dtMovimentacao"]) : DateTime.MinValue,
                        SImagem = row["SImagem"] != DBNull.Value ? (byte[])row["sImagem"] : new byte[0] // Inicializa com um array vazio se for nulo
                    };

                    ls_MovimentacaoRel.Add(item);
                }
            }
            else
                MensagemPagina.MostraMensagem_Erro("Nenhum Lançamento Localizado");
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        =>
            Pesquisar();


        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }
        }

        protected void ExportarExcel_Click(object sender, EventArgs e)
        {
            EXC_RelatorioMovimentacao();
        }
        void EXC_RelatorioMovimentacao()
        {
            try
            {
                if (ls_MovimentacaoRel == null || ls_MovimentacaoRel.Count == 0)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao Gerar Excel: Nenhum dado encontrado.");
                    return;
                }

                DataTable dtRel = ConverterListaParaDataTable(ls_MovimentacaoRel);
                if (dtRel.Rows.Count == 0)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao Gerar Excel: Nenhum dado encontrado.");
                    return;
                }

                ReportViewer rv4 = new ReportViewer();
                rv4.ProcessingMode = ProcessingMode.Local;
                rv4.LocalReport.EnableExternalImages = true;
                rv4.LocalReport.ReportPath = Server.MapPath("~/App/Reports/RelatorioMovimentacao.rdlc");
                rv4.LocalReport.DataSources.Clear();
                rv4.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtRel));

                DataRow row = dtRel.Rows[0];
                ReportParameter[] rp = new ReportParameter[10];
                rp[0] = new ReportParameter("sCodigo", row["SCodigo"].ToString());
                rp[1] = new ReportParameter("sDscProduto", row["SDscProduto"].ToString());
                rp[2] = new ReportParameter("sUnidade", row["SUnidade"].ToString());
                rp[3] = new ReportParameter("sDscFamilia", row["SDscFamilia"].ToString());
                rp[4] = new ReportParameter(
    "sLocal",
    row["SLocal"] == DBNull.Value ? "nenhum local" : row["SLocal"].ToString()
);
                rp[5] = new ReportParameter("nVendas", row["NVendas"].ToString());
                rp[6] = new ReportParameter("nCompras", row["NCompras"].ToString());
                rp[7] = new ReportParameter("nEstoqueAnterior", row["NEstoqueAnterior"].ToString());
                rp[8] = new ReportParameter("nEstoqueAtual", row["NEstoqueAtual"].ToString());

                string dtMov = string.Empty;
                if (DateTime.TryParse(row["DtMovimentacao"].ToString(), out DateTime dtMovimentacao))
                {
                    dtMov = dtMovimentacao.ToString("dd/MM/yyyy");
                }
                rp[9] = new ReportParameter("dtMovimentacao", dtMov);

                rv4.LocalReport.SetParameters(rp);
                rv4.LocalReport.Refresh();

                Microsoft.Reporting.WebForms.Warning[] warnings;
                string[] streamIds;
                string mimeType, encoding, extension;
                byte[] bytes = rv4.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                string path = Server.MapPath("~/Download/");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                string sNomeArquivoOriginal = "RelatorioMovimentacao_" + FUNCOES.CarimboDataHora() + ".xls";
                string sNomeArquivo = LimparNomeArquivo(sNomeArquivoOriginal);

                File.WriteAllBytes(Path.Combine(path, sNomeArquivo), bytes);
                FUNCOES.DownloadArquivo(Page, sNomeArquivo);

                MensagemPagina.MostraMensagem_Sucesso("Relatório de Movimentação gerado com sucesso!");
            }
            catch (Exception ex)
            {
                string mensagemCompleta = ex.Message;
                if (ex.InnerException != null)
                {
                    mensagemCompleta += " - " + ex.InnerException.Message;
                }
                MensagemPagina.MostraMensagem_Erro("Erro ao Gerar Excel: " + mensagemCompleta);
            }
        }

        public string LimparNomeArquivo(string nomeArquivo)
        {
            char[] caracteresInvalidos = System.IO.Path.GetInvalidFileNameChars();
            char[] caracteresInvalidosDiretorio = System.IO.Path.GetInvalidPathChars();

            List<char> caracteresValidos = new List<char>();

            foreach (char c in nomeArquivo)
            {
                if (!caracteresInvalidos.Contains(c) && !caracteresInvalidosDiretorio.Contains(c))
                {
                    caracteresValidos.Add(c);
                }
                else
                {
                    caracteresValidos.Add('_');
                }
            }

            return new string(caracteresValidos.ToArray());
        }
        private DataTable ConverterListaParaDataTable(List<cls_WMS_ProdutoMovimentacaoRel> lista)
        {
            DataTable dt = new DataTable();
            // Criação das colunas conforme a classe
            dt.Columns.Add("IdItem", typeof(int));
            dt.Columns.Add("NEstoqueAtual", typeof(int));
            dt.Columns.Add("SCodigo", typeof(string));
            dt.Columns.Add("SDscProduto", typeof(string));
            dt.Columns.Add("SUnidade", typeof(string));
            dt.Columns.Add("SDscFamilia", typeof(string));
            dt.Columns.Add("SLocal", typeof(string));
            dt.Columns.Add("NCompras", typeof(int));
            dt.Columns.Add("NVendas", typeof(int));
            dt.Columns.Add("NEstoqueAnterior", typeof(int));
            dt.Columns.Add("DtMovimentacao", typeof(DateTime));
            dt.Columns.Add("SImagem", typeof(byte[]));
            // Preenche as linhas do DataTable com os dados da lista
            foreach (var item in lista)
            {
                dt.Rows.Add(
                    item.IdItem,
                    item.NEstoqueAtual,
                    item.SCodigo,
                    item.SDscProduto,
                    item.SUnidade,
                    item.SDscFamilia,
                    item.SLocal,
                    item.NCompras,
                    item.NVendas,
                    item.NEstoqueAnterior,
                    item.DtMovimentacao,
                    item.SImagem
                );
            }

            return dt;
        }

        //protected void dtgItens_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        System.Web.UI.WebControls.Image img = (System.Web.UI.WebControls.Image)e.Row.FindControl("imgProduto");

        //        TextBox txtIdProduto = (TextBox)e.Row.FindControl("txtIdItem");
        //        //CheckBox chkItemEtiqueta = (CheckBox)e.Row.FindControl("chkOpcaoItemEtiqueta");

        //        if (txtIdProduto != null)
        //        {
        //            CarregaImgProduto(txtIdProduto.Text, img);
        //        }
        //    }
        //}
        //protected void CarregaImgProduto(string idProduto, System.Web.UI.WebControls.Image img)
        //{
        //    DataTable dsPesquisa;
        //    Dictionary<string, string> vParametros = new Dictionary<string, string>();
        //    vParametros.Add("@sFuncao", "CONSULTAR_IMAGEM");
        //    vParametros.Add("@idTipoArquivo", "201");
        //    vParametros.Add("@idObjeto", idProduto);
        //    dsPesquisa = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);

        //    if (dsPesquisa.Rows.Count > 0)
        //    {
        //        DataRow imgBd = dsPesquisa.Rows[0];
        //        string imgUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])imgBd["vbArquivo"]);

        //        img.ImageUrl = imgUrl;
        //        img.Visible = true;
        //    }
        //    else
        //    {
        //        img.ImageUrl = "/App/img/wms_dimensoes.svg";
        //        img.Visible = true;
        //    }
        //}
    }
}