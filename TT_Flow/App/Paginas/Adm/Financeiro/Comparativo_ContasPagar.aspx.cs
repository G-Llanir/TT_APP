using Org.BouncyCastle.Pqc.Crypto.Lms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;

namespace TT_Hub.App.Paginas.Adm.Financeiro
{
    public partial class Comparativo_ContasPagar : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasPagar.Comparativo, true);
            cmdExportarExcel.Visible = false;


            if (Request.QueryString["export"] == "1")
            {

                if (Session["RelatorioData"] != null && Session["RelatorioFiltros"] != null)
                {
                    var filtros = (Dictionary<string, string>)Session["RelatorioFiltros"];
                    DataTable tb = (DataTable)Session["RelatorioData"];


                    txtdtInicio.Text = filtros["dtInicio"];
                    txtdtFinal.Text = filtros["dtFinal"];


                    if (ddlidEmpresa.Items.FindByValue(filtros["idEmpresa"]) == null)
                        ddlidEmpresa.Items.Add(new ListItem(filtros["dscEmpresa"], filtros["idEmpresa"]));
                    ddlidEmpresa.SelectedValue = filtros["idEmpresa"];

                    if (ddlsTipoPesquisa.Items.FindByText(filtros["tipoPesquisa"]) == null)
                        ddlsTipoPesquisa.Items.Add(new ListItem(filtros["tipoPesquisa"], ""));
                    ddlsTipoPesquisa.ClearSelection();
                    ddlsTipoPesquisa.Items.FindByText(filtros["tipoPesquisa"]).Selected = true;

                    if (ddlTipoVisualizacao.Items.FindByValue(filtros["tipoVisualizacaoVal"]) == null)
                        ddlTipoVisualizacao.Items.Add(new ListItem(filtros["tipoVisualizacaoTxt"], filtros["tipoVisualizacaoVal"]));
                    ddlTipoVisualizacao.SelectedValue = filtros["tipoVisualizacaoVal"];


                    GeraRelatorio(tb);
                    pnResultado.Visible = true;

                    string css = @"
                    <style>
                        body, table, td, th { font-family: 'Arial', sans-serif; font-size: 10pt; }
                        table { border-collapse: collapse; width: 100%; }
                        table, th, td { border: 1px solid #999; }
                        th, td { padding: 4px; text-align: left; }
                        td[style*='text-align:right'] { text-align: right; mso-number-format:'\#\,\#\#0\.00'; }
                        h4 { font-size: 14pt; font-weight: bold; text-align:center; }
                        b { font-weight: bold; }
                        i { font-style: italic; }
                        tr[style*='#fce4e4'] { background-color: #fce4e4 !important; }
                        tr[style*='#e4fce4'] { background-color: #e4fce4 !important; }
                        tr[style*='#dbe9fc'] { background-color: #dbe9fc !important; }
                        tr[style*='#f2f2f2'] { background-color: #f2f2f2 !important; }
                        a { text-decoration: none; color: black; } 
                    </style>
                    ";

                    if (DIV_RELATORIO.Controls.Count > 0 && DIV_RELATORIO.Controls[0] is Literal && ((Literal)DIV_RELATORIO.Controls[0]).Text.StartsWith("<style>"))
                    {
                        DIV_RELATORIO.Controls.RemoveAt(0);
                    }
                    DIV_RELATORIO.Controls.AddAt(0, new Literal { Text = css });

                    ExportarPainel_Excel(this, DIV_RELATORIO, "ComparativoFinanceiro");
                }
                else
                {
                    Response.Write("<script>alert('Sessão expirada. Gere o relatório novamente.'); window.close();</script>");
                    Response.End();
                }
                return;
            }

            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Todas as Empresas", "0");

                BreadCrumb_Pagina.TitulodaPagina = "Comparativo";
                BreadCrumb_Pagina.NivelPagina = 2;
                txtdtFinal.Text = DateTime.Today.ToString("u").Substring(0, 10);
                txtdtInicio.Text = DateTime.Today.Year.ToString() + "-01-01";
                pnResultado.Visible = false;

                PopulaFiltroCategoria();
            }

            if (IsPostBack)
            {
                if (Session["RelatorioData"] != null)
                {
                    DataTable tb = (DataTable)Session["RelatorioData"];
                    GeraRelatorio(tb);
                    pnResultado.Visible = true;
                    cmdExportarExcel.Visible = true;
                }
            }
        }

        protected void PopulaFiltroCategoria()
        {
            string sTipoVisualizacao = ddlTipoVisualizacao.SelectedValue;
            lstidCategoriaPagar.Items.Clear();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "ReaplicarMultiselect", "ReaplicarMultiselect();", true);

            if (sTipoVisualizacao == "R" || sTipoVisualizacao == "C")
            {
                try
                {
                    DataTable tbReceber = BD.ExecutarDataTable("sp_Select 'Flow_Adm_Contas_Receber_Categoria'");
                    foreach (DataRow row in tbReceber.Rows)
                    {
                        ListItem item = new ListItem();
                        item.Value = "R_" + row["idCategoriaReceber"].ToString();
                        item.Text = "RECEITA: " + row["sDscCategoriaReceber"].ToString();
                        lstidCategoriaPagar.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao carregar categorias de receita: " + ex.Message);
                }
            }

            if (sTipoVisualizacao == "D" || sTipoVisualizacao == "C")
            {
                try
                {
                    DataTable tbPagar = BD.ExecutarDataTable("sp_Select 'Flow_Adm_Contas_Pagar_Categoria'");
                    foreach (DataRow row in tbPagar.Rows)
                    {
                        ListItem item = new ListItem();
                        item.Value = "P_" + row["idCategoriaPagar"].ToString();
                        item.Text = "DESPESA: " + row["sDscCategoriaPagar"].ToString();
                        lstidCategoriaPagar.Items.Add(item);
                    }

                    if (sTipoVisualizacao == "D")
                    {
                        ListItem custoFixo = new ListItem("Custo Fixo", "P_-9999");
                        lstidCategoriaPagar.Items.Insert(0, custoFixo);
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao carregar categorias de despesa: " + ex.Message);
                }
            }
        }

        protected void ddlTipoVisualizacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulaFiltroCategoria();
            cmdPesquisar_Click(sender, e);
        }

        protected void Pesquisar(string DashBoard)
        {
            pnResultado.Visible = false;
            cmdExportarExcel.Visible = false;
            Session["RelatorioData"] = null;
            Session["RelatorioFiltros"] = null;

            try
            {
                string sSql = "sp_Manipula_tbl_Flow_Adm_Contas_Pagar_Comparativo";
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@dtInicio", txtdtInicio.Text);
                vParametros.Add("@dtFim", txtdtFinal.Text);
                vParametros.Add("@sTipoPesquisa", ddlsTipoPesquisa.SelectedValue);
                vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                vParametros.Add("@sTipoVisualizacao", ddlTipoVisualizacao.SelectedValue);

                string slstidCategoriaPagar = "";
                string slstidCategoriaReceber = "";

                foreach (ListItem item in lstidCategoriaPagar.Items)
                {
                    if (item.Selected)
                    {
                        if (item.Value.StartsWith("R_"))
                        {
                            slstidCategoriaReceber += item.Value.Substring(2) + "|";
                        }
                        else if (item.Value.StartsWith("P_"))
                        {
                            slstidCategoriaPagar += item.Value.Substring(2) + "|";
                        }
                    }
                }

                vParametros.Add("@sidCategoria", slstidCategoriaPagar);
                vParametros.Add("@sidCategoriaReceber", slstidCategoriaReceber);

                DataTable tb = BD.ExecutarDataTable(sSql, vParametros, false);

                if (tb.Rows.Count > 0)
                {
                    GeraRelatorio(tb);
                    pnResultado.Visible = true;
                    cmdExportarExcel.Visible = true;
                    Session["RelatorioData"] = tb;

                    var filtros = new Dictionary<string, string>
                    {
                        { "dtInicio", txtdtInicio.Text },
                        { "dtFinal", txtdtFinal.Text },
                        { "idEmpresa", ddlidEmpresa.SelectedValue },
                        { "dscEmpresa", ddlidEmpresa.SelectedItem.Text },
                        { "tipoPesquisa", ddlsTipoPesquisa.SelectedItem.Text },
                        { "tipoVisualizacaoVal", ddlTipoVisualizacao.SelectedValue },
                        { "tipoVisualizacaoTxt", ddlTipoVisualizacao.SelectedItem.Text }
                    };
                    Session["RelatorioFiltros"] = filtros;
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Nenhum Registro Localizado");
                    Session["RelatorioData"] = null;
                    Session["RelatorioFiltros"] = null;
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao Consultar: " + ex.Message);
            }
        }

        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (txtdtInicio.Text != "" || txtdtFinal.Text != "")
            {
                sMensagemErro += Validacoes.ValidaDatas(txtdtInicio.Text, txtdtFinal.Text);
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            if (ValidarDados())
            {
                Pesquisar("");
            }
        }

        #region | Geração Relatório |

        void GeraRelatorio(DataTable tb)
        {
            StringBuilder sHTML = new StringBuilder();
            Literal lObjLiteral = new Literal();
            cResultados vResultados = new cResultados();
            lObjLiteral.Text = "";

            string sTipoLancamento = "";
            string sTipoLancamento_Anterior = "";
            string sDscCategoriaTipo = "";
            string sDscCategoriaTipo_Anterior = "";
            string sDscCategoriaPai = "";
            string sDscCategoriaPai_Anterior = "";
            string sDscCategoriaTipoPai = "";
            string sDscCategoriaTipoPai_Anterior = "";
            string sDscCategoriaPagar_Completa = "";

            string sEmpresa = "";
            string sMesColuna = "";
            string sTituloRelatorio = "Comparativo Financeiro (" + ddlTipoVisualizacao.SelectedItem.Text + ")";
            DateTime dtInicio = DateTime.ParseExact(txtdtInicio.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dtFinal = DateTime.ParseExact(txtdtFinal.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string sPeriodo = dtInicio.ToString("dd/MM/yyyy") + " a " + dtFinal.ToString("dd/MM/yyyy") + "</br>" + ddlsTipoPesquisa.SelectedItem.Text;
            int nqtdColunas = 0;
            int nContadorLinha = 0;
            int nContadorMeses = 0;

            if (ddlidEmpresa.SelectedValue != "0")
                sEmpresa = ddlidEmpresa.SelectedItem.Text + "</br>";

            foreach (DataRow row in tb.Rows)
            {
                if (nContadorLinha == 0)
                {
                    nContadorMeses = Convert.ToInt32(row["nQtdMeses"].ToString());
                    nqtdColunas = 1 + nContadorMeses + 1;
                    sHTML.Append("<table border='1' cellspacing='0'  class='table table-sm table-striped table-bordered table-hover table-condensed'>");
                    sHTML.Append("  <thead>");
                    sHTML.Append("  <tr>");
                    sHTML.Append("      <td colspan='" + nqtdColunas.ToString() + "' style='text-align:center;' ><b><h4>" + sTituloRelatorio + "</h4>" + sEmpresa + sPeriodo + "</td>");
                    sHTML.Append("  </tr>");
                    sHTML.Append("  </thead>");

                    sHTML.Append("  <thead>");
                    sHTML.Append("  <tr>");
                    sHTML.Append("      <td style='text-align:center;width:300px;'><b>Categoria</b></td>");
                    for (int i = 0; i < nContadorMeses; i++)
                    {
                        sMesColuna = Convert.ToDateTime(txtdtInicio.Text).AddMonths(i).ToString("MM/yyyy").ToString();
                        sHTML.Append("      <td style='text-align:center;width:55px;'><b>" + sMesColuna + " </b></td>");
                    }
                    sHTML.Append("      <td style='text-align:center;width:80px;'><b>Total</b></td>");
                    sHTML.Append("  </tr>");
                    sHTML.Append("  </thead>");
                }

                sTipoLancamento = row["sTipoLancamento"].ToString();

                if (sTipoLancamento != sTipoLancamento_Anterior)
                {
                    if (sTipoLancamento_Anterior != "")
                    {
                        sHTML.Append(Totaliza_sDscCategoriaPai(sTipoLancamento_Anterior, sDscCategoriaTipo_Anterior, sDscCategoriaPai_Anterior, row, nContadorMeses, vResultados));
                        sHTML.Append(Totaliza_sDscCategoriaTipo(sTipoLancamento_Anterior, sDscCategoriaTipo_Anterior, row, nContadorMeses, vResultados));
                        sHTML.Append(Totaliza_Geral(sTipoLancamento_Anterior, row, nContadorMeses, vResultados));
                    }
                    //else if (sTipoLancamento_Anterior == "R")
                    //{

                    //    sHTML.Append(Totaliza_Receita_sDscCategoriaPai(sDscCategoriaPai_Receita_Anterior, row, nContadorMeses, vResultados));
                    //    sHTML.Append(Totaliza_Receita_sDscCategoriaTipo(sDscCategoriaTipo_Anterior, row, nContadorMeses, vResultados));
                    //    sHTML.Append(Totaliza_Receitas_Geral(row, nContadorMeses, vResultados));
                    //}

                    string sTituloSecao = (sTipoLancamento == "R") ? "RECEITAS" : "DESPESAS";
                    sHTML.Append("      <tr><td colspan='" + nqtdColunas.ToString() + "' style='text-align:left; background-color: #f2f2f2;' ><b>" + sTituloSecao + "</b></td></tr>");

                    sDscCategoriaTipo_Anterior = "";
                    sDscCategoriaPai_Anterior = "";
                    sDscCategoriaTipoPai_Anterior = "";
                }

                {
                    sDscCategoriaTipo = row["sDscCategoriaTipo"].ToString();
                    sDscCategoriaPai = row["sDscCategoriaPai"].ToString();
                    sDscCategoriaTipoPai = sDscCategoriaTipo + sDscCategoriaPai;
                    sDscCategoriaPagar_Completa = row["sDscCategoriaPagar_Completa"].ToString();

                    if (sDscCategoriaTipoPai != sDscCategoriaTipoPai_Anterior)
                    {
                        if (sDscCategoriaTipo_Anterior != "")
                        {
                            sHTML.Append(Totaliza_sDscCategoriaPai(sTipoLancamento, sDscCategoriaTipo_Anterior, sDscCategoriaPai_Anterior, row, nContadorMeses, vResultados));
                            if (sDscCategoriaTipo != sDscCategoriaTipo_Anterior)
                            {
                                sHTML.Append(Totaliza_sDscCategoriaTipo(sTipoLancamento, sDscCategoriaTipo_Anterior, row, nContadorMeses, vResultados));
                            }
                        }
                        if (sDscCategoriaTipo != sDscCategoriaTipo_Anterior)
                        {
                            sHTML.Append("      <tr><td colspan='" + nqtdColunas.ToString() + "' style='text-align:left;padding-left: 5px;' ><b>" + row["sDscCategoriaTipo"].ToString() + "</b></td></tr>");
                        }
                        sHTML.Append("      <tr><td colspan='" + nqtdColunas.ToString() + "' style='text-align:left;padding-left: 15px;' ><i>" + row["sDscCategoriaPai"].ToString() + "</i></td></tr>");
                    }

                    sHTML.Append("  <tr>");
                    sHTML.Append("      <td style='text-align:left;padding-left: 25px;wid' >" + row["sDscCategoriaPagar"].ToString() + "</td>");

                    int ncontadorcoluna = 0;
                    double nTotalLinha = 0;
                    foreach (DataColumn col in row.Table.Columns)
                    {
                        if (col.Ordinal == (row.Table.Columns.Count - nContadorMeses) + ncontadorcoluna)
                        {
                            sMesColuna = Convert.ToDateTime(txtdtInicio.Text).AddMonths(ncontadorcoluna).ToString("MM/yyyy").ToString();
                            double nValor = Convert.ToDouble(row[col.ToString()].ToString());
                            nTotalLinha += nValor;

                            if (nValor == 0)
                            {
                                sHTML.Append("      <td style='text-align:right;wid' >" + nValor.ToString("N2") + "</td>");
                            }
                            else
                            {
                                string sDataInicial = string.Format("01/{0}", sMesColuna);
                                if (ncontadorcoluna == 0)
                                    sDataInicial = dtInicio.ToString("dd/MM/yyyy");

                                string sDataFinal = dtFinal.ToString("dd/MM/yyyy");
                                if (ncontadorcoluna < nContadorMeses - 1)
                                    sDataFinal = string.Format("{0}/{1}", System.DateTime.DaysInMonth(Convert.ToInt32(sMesColuna.Substring(3, 4)), Convert.ToInt32(sMesColuna.Substring(0, 2))).ToString().PadLeft(2, '0'), sMesColuna);

                                string sPagina = "ContasReceber.aspx";
                                if (sTipoLancamento == "P")
                                    sPagina = "ContasPagar.aspx";

                                sHTML.Append("      <td style='text-align:right;wid'><a href='" + sPagina + "?" + string.Format("DashBoard=Comparativo&dtInicio={0}&dtFinal={1}&idCategoria={2}&idEmpresa={3}&idDataPesquisa={4}", sDataInicial, sDataFinal, row["idCategoria"].ToString(), ddlidEmpresa.SelectedValue, ddlsTipoPesquisa.SelectedValue) + "' target='_blank'>" + nValor.ToString("N2") + "</a></td>");
                            }
                            vResultados.Adicionar(col.ToString(), sTipoLancamento, sDscCategoriaPai, sDscCategoriaPagar_Completa, nValor, sDscCategoriaTipo);
                            ncontadorcoluna++;
                        }
                    }
                    vResultados.Adicionar("Total", sTipoLancamento, sDscCategoriaPai, sDscCategoriaPagar_Completa, nTotalLinha, sDscCategoriaTipo);
                    sHTML.Append("      <td style='text-align:right;wid' >" + nTotalLinha.ToString("N2") + "</td>");
                    sHTML.Append("  </tr>");

                    sDscCategoriaTipo_Anterior = sDscCategoriaTipo;
                    sDscCategoriaPai_Anterior = sDscCategoriaPai;
                    sDscCategoriaTipoPai_Anterior = sDscCategoriaTipoPai;
                }

                sTipoLancamento_Anterior = sTipoLancamento;
                nContadorLinha += 1;

                if (nContadorLinha == tb.Rows.Count)
                {
                    //if (sTipoLancamento_Anterior == "P")
                    {
                        sHTML.Append(Totaliza_sDscCategoriaPai(sTipoLancamento_Anterior, sDscCategoriaTipo_Anterior, sDscCategoriaPai_Anterior, row, nContadorMeses, vResultados));
                        sHTML.Append(Totaliza_sDscCategoriaTipo(sTipoLancamento_Anterior, sDscCategoriaTipo_Anterior, row, nContadorMeses, vResultados));
                        sHTML.Append(Totaliza_Geral(sTipoLancamento_Anterior, row, nContadorMeses, vResultados));
                    }


                    if (ddlTipoVisualizacao.SelectedValue == "C")
                    {
                        sHTML.Append(Totaliza_LIQUIDO(row, nContadorMeses, vResultados));
                    }
                }
            }

            sHTML.Append("</table>");
            vResultados.Clear();

            lObjLiteral.Text = sHTML.ToString();
            DIV_RELATORIO.Controls.Clear();
            DIV_RELATORIO.Controls.Add(lObjLiteral);
        }

        #region Totalizadores (Corrigido para 'P')

        StringBuilder Totaliza_sDscCategoriaPai(string sTipoLancamento,string sDscCategoriaTipo_Anterior,  string sDscCategoriaPai_Anterior, DataRow row, int nContadorMeses, cResultados vResultados)
        {
            StringBuilder sHTML = new StringBuilder();
            if (sDscCategoriaPai_Anterior != "")
            {
                sHTML.Append("      <tr><td colspan='" + "1" + "' style='text-align:left;padding-left: 15px;wid' ><i> Total " + sDscCategoriaPai_Anterior + "</i></td>");
                int ncontadorcoluna = 0;
                foreach (DataColumn col in row.Table.Columns)
                {
                    if (col.Ordinal == (row.Table.Columns.Count - nContadorMeses) + ncontadorcoluna)
                    {
                        sHTML.Append("      <td style='text-align:right;wid' ><i>" + vResultados.TotalizarColuna_sDscCategoriaPai(sTipoLancamento, sDscCategoriaTipo_Anterior, sDscCategoriaPai_Anterior, col.ToString()).ToString("N2") + "</i></td>");
                        ncontadorcoluna++;
                    }

                }
                sHTML.Append("      <td style='text-align:right;wid' ><i>" + vResultados.TotalizarColuna_sDscCategoriaPai(sTipoLancamento, sDscCategoriaTipo_Anterior, sDscCategoriaPai_Anterior, "Total").ToString("N2") + "</i></td>");
                sHTML.Append("</tr>");
            }
            return sHTML;
        }

        StringBuilder Totaliza_sDscCategoriaTipo(string sTipoLancamento, string sDscCategoriaTipo_Anterior, DataRow row, int nContadorMeses, cResultados vResultados)
        {
            StringBuilder sHTML = new StringBuilder();
            
            string sDscTipoLancamento = "RECEITAS";
            if (sTipoLancamento != "R")
            {
                sDscTipoLancamento = "DESPESAS";
            }


            if (sDscCategoriaTipo_Anterior != "")
            {
                double nValor = 0;
                sHTML.Append("      <tr><td colspan='" + "1" + "' style='text-align:left;wid' ><b> Total " + sDscCategoriaTipo_Anterior + "</b></td>");
                int ncontadorcoluna = 0;
                foreach (DataColumn col in row.Table.Columns)
                {
                    if (col.Ordinal == (row.Table.Columns.Count - nContadorMeses) + ncontadorcoluna)
                    {
                        nValor = vResultados.TotalizarColuna_sDscCategoriaTipo(sTipoLancamento, sDscCategoriaTipo_Anterior, col.ToString());
                        sHTML.Append("      <td style='text-align:right;wid' ><b>" + nValor.ToString("N2") + "</b></td>");

                        vResultados.Adicionar(col.ToString(), sTipoLancamento, "SOMA_GERAL_"+ sDscTipoLancamento, "", nValor, "SOMA_GERAL_" + sDscTipoLancamento);

                        ncontadorcoluna++;
                    }

                }
                nValor = vResultados.TotalizarColuna_sDscCategoriaTipo(sTipoLancamento, sDscCategoriaTipo_Anterior, "Total");
                sHTML.Append("      <td style='text-align:right;wid' ><b>" + nValor.ToString("N2") + "</b></td>");

                vResultados.Adicionar("Total", sTipoLancamento, "SOMA_GERAL_" + sDscTipoLancamento, "", nValor, "SOMA_GERAL_" + sDscTipoLancamento);

                sHTML.Append("</tr>");
            }
            return sHTML;
        }

        StringBuilder Totaliza_Geral(string sTipoLancamento,DataRow row, int nContadorMeses, cResultados vResultados)
        {
            string sDscTipoLancamento = "RECEITAS";
            string sCor = "#dbe9fc;";
            if (sTipoLancamento != "R")
            {
                sDscTipoLancamento = "DESPESAS";
                sCor = "#fce4e4;";
            }


            StringBuilder sHTML = new StringBuilder();
      
            sHTML.Append("      <tr style='background-color:" + sCor +"'><td colspan='" + "1" + "' style='text-align:left;wid' ><b> Total Geral de " + sDscTipoLancamento + "</b></td>");
            int ncontadorcoluna = 0;
            double nValor = 0;
            foreach (DataColumn col in row.Table.Columns)
            {
                if (col.Ordinal == (row.Table.Columns.Count - nContadorMeses) + ncontadorcoluna)
                {
                    nValor = vResultados.TotalizarColuna_sDscCategoriaTipo(sTipoLancamento, "SOMA_GERAL_" + sDscTipoLancamento, col.ToString());
                    sHTML.Append("      <td style='text-align:right;wid' ><b>" + nValor.ToString("N2") + "</b></td>");
                    ncontadorcoluna++;
                }
            }
            nValor = vResultados.TotalizarColuna_sDscCategoriaTipo(sTipoLancamento, "SOMA_GERAL_"+ sDscTipoLancamento, "Total");
            sHTML.Append("      <td style='text-align:right;wid' ><b>" + nValor.ToString("N2") + "</b></td>");
            sHTML.Append("</tr>");
            return sHTML;
        }



      
        StringBuilder Totaliza_LIQUIDO(DataRow row, int nContadorMeses, cResultados vResultados)
        {
            StringBuilder sHTML = new StringBuilder();
            sHTML.Append("      <tr style='background-color: #dbe9fc;'><td colspan='" + "1" + "' style='text-align:left;wid' ><b> LÍQUIDO </b></td>");
            int ncontadorcoluna = 0;
            double nValorReceita = 0;
            double nValorDespesa = 0;
            double nValorLiquido = 0;

            foreach (DataColumn col in row.Table.Columns)
            {
                if (col.Ordinal == (row.Table.Columns.Count - nContadorMeses) + ncontadorcoluna)
                {
                    nValorReceita = vResultados.TotalizarColuna_sDscCategoriaTipo("R", "SOMA_GERAL_RECEITAS", col.ToString());
                    nValorDespesa = vResultados.TotalizarColuna_sDscCategoriaTipo("P", "SOMA_GERAL_DESPESAS", col.ToString());
                    nValorLiquido = nValorReceita - nValorDespesa;
                    string sCorCelula = "#dbe9fc;";
                    if (nValorLiquido < 0)
                    {
                        sCorCelula = "#fce4e4;";
                    }
                    sHTML.Append("      <td style='text-align:right;wid;background-color:" + sCorCelula + "' ><b>" + nValorLiquido.ToString("N2") + "</b></td>");
                    ncontadorcoluna++;
                }
            }

            nValorReceita = vResultados.TotalizarColuna_sDscCategoriaTipo("R", "SOMA_GERAL_RECEITAS", "Total");
            nValorDespesa = vResultados.TotalizarColuna_sDscCategoriaTipo("P", "SOMA_GERAL_DESPESAS", "Total");
            nValorLiquido = nValorReceita - nValorDespesa;
            sHTML.Append("      <td style='text-align:right;wid' ><b>" + nValorLiquido.ToString("N2") + "</b></td>");
            sHTML.Append("</tr>");
            return sHTML;
        }

        #endregion

        #region | Classe de Totalização (Corrigida) |
        public class cResultados : CollectionBase
        {
            public cResultados() { List.Clear(); }
            public int Adicionar(Valores sValores) { return List.Add(sValores); }
            public int Adicionar(string sColuna, string sTipoLancamento, string sDscCategoriaPai, string sDscCategoriaPagar_Completa, double nValor, string sDscCategoriaTipo)
            {
                Valores sValores = new Valores(sColuna, sTipoLancamento, sDscCategoriaTipo, sDscCategoriaPai, sDscCategoriaPagar_Completa, nValor);
                return List.Add(sValores);
            }

            public double Totalizar_sDscCategoriaPagar_Completa(string sDscCategoriaPagar_Completa)
            {
                double retorno = 0;
                for (int v = 0; v < List.Count; v++)
                {
                    Valores ListaSomar = (Valores)List[v];
                    if (string.Compare(ListaSomar.sDscCategoriaPagar_Completa, sDscCategoriaPagar_Completa) == 0) { retorno += ListaSomar.nValor; }
                }
                return retorno;
            }

            public double TotalizarColuna_sDscCategoriaPai(string sTipoLancamento, string sDscCategoriaTipo,  string sDscCategoriaPai, string sColuna)
            {
                double retorno = 0;
                for (int v = 0; v < List.Count; v++)
                {
                    Valores ListaSomar = (Valores)List[v];

                    if (ListaSomar.sDscCategoriaTipo == "SOMA_GERAL_RECEITAS" || ListaSomar.sDscCategoriaTipo == "SOMA_GERAL_DESPESAS")
                    {
                        continue; 
                    }

                    if (string.Compare(ListaSomar.sTipoLancamento, sTipoLancamento) == 0 &&
                        string.Compare(ListaSomar.sDscCategoriaPai, sDscCategoriaPai) == 0 &&
                        string.Compare(ListaSomar.sDscCategoriaTipo, sDscCategoriaTipo) == 0 &&

                        string.Compare(ListaSomar.sColuna, sColuna) == 0)
                    {
                        retorno += ListaSomar.nValor;
                    }
                }
                return retorno;
            }

            public double TotalizarColuna_sDscCategoriaTipo(string sTipoLancamento, string sDscCategoriaTipo, string sColuna)
            {
                double retorno = 0;
                for (int v = 0; v < List.Count; v++)
                {
                    Valores ListaSomar = (Valores)List[v];

                    if (sDscCategoriaTipo == "SOMA_GERAL_RECEITAS" || sDscCategoriaTipo == "SOMA_GERAL_DESPESAS")
                    {
                        if (string.Compare(ListaSomar.sTipoLancamento, sTipoLancamento) == 0 &&
                            string.Compare(ListaSomar.sDscCategoriaTipo, sDscCategoriaTipo) == 0 &&
                            string.Compare(ListaSomar.sColuna, sColuna) == 0)
                        {
                            retorno += ListaSomar.nValor;
                        }
                    }
                    else 
                    {
                        if (ListaSomar.sDscCategoriaTipo == "SOMA_GERAL_RECEITAS" || ListaSomar.sDscCategoriaTipo == "SOMA_GERAL_DESPESAS")
                        {
                            continue;
                        }

                        if (string.Compare(ListaSomar.sTipoLancamento, sTipoLancamento) == 0 &&
                            string.Compare(ListaSomar.sDscCategoriaTipo, sDscCategoriaTipo) == 0 &&
                            string.Compare(ListaSomar.sColuna, sColuna) == 0)
                        {
                            retorno += ListaSomar.nValor;
                        }
                    }
                }
                return retorno;
            }

            public Valores this[int index] { get { return (Valores)List[index]; } set { List[index] = value; } }
        }
        public class Valores
        {
            protected string _sColuna;
            protected string _sTipoLancamento;
            protected string _sDscCategoriaPagar_Completa;
            protected string _sDscCategoriaTipo;
            protected string _sDscCategoriaPai;
            protected double _nValor;

            public Valores() { }
            public Valores(string sColuna, string sTipoLancamento, string sDscCategoriaTipo, string sDscCategoriaPai, string sDscCategoriaPagar_Completa, double nValor)
            {
                this._sColuna = sColuna;
                this._sTipoLancamento = sTipoLancamento;
                this._sDscCategoriaTipo = sDscCategoriaTipo;
                this._sDscCategoriaPai = sDscCategoriaPai;
                this._sDscCategoriaPagar_Completa = sDscCategoriaPagar_Completa;
                this._nValor = nValor;
            }
            public string sColuna { get { return this._sColuna; } set { _sColuna = value; } }
            public string sTipoLancamento { get { return this._sTipoLancamento; } set { _sTipoLancamento = value; } }
            public string sDscCategoriaTipo { get { return this._sDscCategoriaTipo; } set { _sDscCategoriaTipo = value; } }
            public string sDscCategoriaPai { get { return this._sDscCategoriaPai; } set { _sDscCategoriaPai = value; } }

            public string sDscCategoriaPagar_Completa { get { return this._sDscCategoriaPagar_Completa; } set { _sDscCategoriaPagar_Completa = value; } }
            public double nValor { get { return this._nValor; } set { _nValor = value; } }
        }
        #endregion

        #endregion


        protected void cmdExportarExcel_Click(object sender, EventArgs e)
        {
            if (Session["RelatorioData"] != null)
            {

                DataTable tb = (DataTable)Session["RelatorioData"]; // Usar Session
                GeraRelatorio(tb);
                pnResultado.Visible = true;

                string css = @"
                <style>
                    body, table, td, th { font-family: 'Arial', sans-serif; font-size: 10pt; }
                    table { border-collapse: collapse; width: 100%; }
                    table, th, td { border: 1px solid #999; }
                    th, td { padding: 4px; text-align: left; }
                    td[style*='text-align:right'] { text-align: right; } 
                    h4 { font-size: 14pt; font-weight: bold; text-align:center; }
                    b { font-weight: bold; }
                    i { font-style: italic; }
                    tr[style*='#fce4e4'] { background-color: #fce4e4 !important; } /* Despesas */
                    tr[style*='#e4fce4'] { background-color: #e4fce4 !important; } /* Receitas */
                    tr[style*='#dbe9fc'] { background-color: #dbe9fc !important; } /* Líquido */
                    tr[style*='#f2f2f2'] { background-color: #f2f2f2 !important; } /* Cabeçalho Seção */
                    a { text-decoration: none; color: black; } /* Remove sublinhado e cor de link */
                </style>
                ";

                if (DIV_RELATORIO.Controls.Count > 0 && DIV_RELATORIO.Controls[0] is Literal && ((Literal)DIV_RELATORIO.Controls[0]).Text.StartsWith("<style>"))
                {
                    DIV_RELATORIO.Controls.RemoveAt(0);
                }
                DIV_RELATORIO.Controls.AddAt(0, new Literal { Text = css });

                ExportarPainel_Excel(this, pnResultado, "ComparativoFinanceiro");
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Não há dados para exportar. Por favor, gere o relatório primeiro.");
            }
        }

        public static string CarimboDataHora()
        {
            return DateTime.Now.ToString("yyyyMMdd_HHmmss");
        }
        public static void ExportarPainel_Excel(Page page, Control ctrl, string sNomeArquivo)
        {
            string attachment = "attachment; filename=" + sNomeArquivo + "_" + CarimboDataHora() + ".xls";
            HttpResponse response = HttpContext.Current.Response;

            response.Clear();
            response.Buffer = true;
            response.AddHeader("content-disposition", attachment);
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.ContentType = "application/vnd.ms-excel";
            response.ContentEncoding = Encoding.UTF8;
            response.Charset = "UTF-8";

            using (StringWriter sw = new StringWriter())
            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
            {
                ctrl.RenderControl(htw);

                response.Write(Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble()));
                response.Write(sw.ToString());
            }

            try
            {
                response.End();
                response.Write("<script>window.close();</script>");
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
        }
    }
}
