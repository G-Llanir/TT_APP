using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using static Permissao.Comercial;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;

namespace TT_Hub.App.Paginas.Manutencao
{
    public partial class Produtos : Page
    {
        string sTituloPagina = "Produtos";
        string sTituloPaginaServico = "Serviços";
        string sTituloPaginaSubServico = "Sub-Serviços";
        string sTituloPaginaRecurso = "Recursos";
        string sPagina_NovoRegistro = "app/Paginas/Manutencao/produtos_Detalhe.aspx?id=0";
        string sPagina_NovoRegistroServico = "app/Paginas/Manutencao/produtos_Detalhe.aspx?id=0&stp=1";
        string sPagina_NovoRegistroRecurso = "app/Paginas/Manutencao/produtos_Detalhe.aspx?id=0&stp=2";
        string sPagina_NovoRegistroSubServico = "app/Paginas/Manutencao/produtos_Detalhe.aspx?id=0&stp=3";

        protected void Page_Load(object sender, EventArgs e)
        {
            pnResultado.Visible = false;
            pnMensagem.Visible = false;

            if (!IsPostBack)
            {
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                RegistraScript("");

                div_sSubTipo.Visible = false;
                ddlsSubTipo.SelectedValue = "0";

                if (Request["stp"] == "1") // Serviços
                {
                    FUNCOES.ValidaPermissao(Permissao.Servicos.Consultar, true);
                    cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Servicos.Incluir);

                    PopulaCombo(1);
                }
                else if (Request["stp"] == "2") // Recursos
                {
                    FUNCOES.ValidaPermissao(Permissao.Produtos_Recursos.Consultar, true);
                    cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Produtos_Recursos.Incluir);

                    PopulaCombo(2);
                }
                else if (Request["stp"] == "3") // Sub-Serviços
                {
                    //FUNCOES.ValidaPermissao(Permissao.Sub_Servicos.Consultar, true);
                    //cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Sub_Servicos.Incluir);

                    PopulaCombo(3);
                }
                else // Produtos
                {
                    FUNCOES.ValidaPermissao(Permissao.Produtos.Consultar, true);
                    cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Produtos.Incluir);
                    cmdImportar.Visible = true;

                    PopulaCombo(0);
                }
            }

            txtPesquisa.Focus();

            string sTipo = Request["stp"] ?? "";

            if (!string.IsNullOrEmpty(sTipo) && sTipo != "0")
            {
                div_CodigoNCM.Visible = false;
                div_ExibeLM.Visible = false;
                div_Familia.Visible = false;
                //div_Grupo.Visible = false;
                div_Pais.Visible = false;
                div_ExibeComposicao.Visible = false;
                div_LocalArmazenamento.Visible = false;

                if (sTipo == "2")
                {
                    div_CategoriaVenda.Visible = false;
                }
            }

            if (sTipo.Equals("1")) hddsManual.Value = "Manual-Servicos.pdf";
            else if (sTipo.Equals("2")) hddsManual.Value = "Manual-Recursos.pdf";
            else if (sTipo.Equals("3")) hddsManual.Value = "Manual-SubServicos.pdf";

            if (sTipo.Equals("1"))
            {
                lblTituloPagina.Text = sTituloPaginaServico;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPaginaServico;
            }
            else if (sTipo.Equals("2"))
            {
                lblTituloPagina.Text = sTituloPaginaRecurso;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPaginaRecurso;
            }
            else if (sTipo.Equals("3"))
            {
                lblTituloPagina.Text = sTituloPaginaSubServico;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPaginaSubServico;
            }

            manual.sNomeArquivo = hddsManual.Value;
        }

        protected void PopulaCombo(int tipo)
        {
            string sTipo = Request["stp"] ?? "";

            if (tipo  == 0)
            {
                FUNCOES.Popula_Combo(ddlFamilia, "sp_Select 'Flow_WMS_Produtos_Familia'", "idFamilia", "sDscFamilia", false, "Todas as Famílias", "0");
                FUNCOES.Popula_Combo(ddlPaisOrigem, "sp_Select 'tbl_Flow_WMS_Produtos_Origem'", "idPais", "sDscPais", false, "Todos Países de Origem", "0");
                FUNCOES.Popula_Combo(ddlLocalArmazenamento, "sp_Select 'Flow_WMS_Produtos_Local_Armazenamento'", "idLocalArmazenamento", "sDscLocalArmazenamento", false, "Todos os Locais de Armazenamento", "0");
            }

            FUNCOES.Popula_Combo(ddlTipoProduto, "sp_Select 'Flow_Produtos_Tipo', @idPesquisa=" + tipo, "idTipoProduto", "sDscTipoProduto", false, "Todos os Tipos", "0");
            FUNCOES.Popula_Combo(ddlGrupo, "sp_Select 'Flow_WMS_Produtos_Grupos_PAI', @idPesquisa=" + tipo, "idGrupo", "sDscGrupo", false, "Todos os Grupos", "0");
            FUNCOES.Popula_Combo(ddlCategoriaVenda, "sp_Manipula_tbl_Flow_Produtos @sFuncao='Consulta_CategoriaVenda'", "idCategoriaVendas", "sDscCategoriaVendas", false, "Todas as Categorias de Vendas", "");
  
        }

        protected DataTable Pesquisar()
        {
            manual.Visible = false;

            int.TryParse(Request["tabelaPreco"], out int tabelaPreco);

            if (tabelaPreco == 1) ddlGrupo.SelectedValue = Request["gp"];
            else if (tabelaPreco == 2) ddlFamilia.SelectedValue = Request["fml"];

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscProduto", txtPesquisa.Text.Trim() },
                { "@sCodigo", txtPesquisa.Text.Trim() },
                { "@idTipoProduto", ddlTipoProduto.SelectedValue },
                { "@idFamilia", ddlFamilia.SelectedValue },
                { "@idGrupo", ddlGrupo.SelectedValue },
                { "@idPais", ddlPaisOrigem.SelectedValue },
                { "@sidCategoriaVendas", ddlCategoriaVenda.SelectedValue },
                { "@sTipoPesquisa", txtPesquisa.Text },
                { "@sCodigoNCM", txtsCodigoNCM.Text },
                { "@sExibeComercial", ddlsExibeComercial.SelectedValue },
                { "@sExibeLM", ddlsExibeLM.SelectedValue },
                { "@sExibeComposicao", ddlsExibeComposicao.SelectedValue },
                { "@sSituacao", ddlsSituacao.SelectedValue },
                { "@sSituacaoCadastral", ddlSituacaoCadastral.SelectedValue },
                { "@sSubTipo", ddlsSubTipo.SelectedValue },
                { "@idLocalArmazenamento", ddlLocalArmazenamento.SelectedValue }, 
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Produtos", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                pnResultado.Visible = true;
                pnResultado.Style["display"] = "block";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", Grid.DataBindComScript(dtgvConsulta, tb, 2, "asc"), true);
            }
            else
            {
                pnMensagem.Visible = true;
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");

                return null;
            }

            RegistraScript("");

            return tb;
        }

        protected DataTable PesquisarProdutos_x_Tipo()
        {
            manual.Visible = true;

            string sTipo = Request["stp"];

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscProduto", txtPesquisa.Text.Trim() },
                { "@sCodigo", txtPesquisa.Text.Trim() },
                { "@idTipoProduto", ddlTipoProduto.SelectedValue },
                { "@sTipoPesquisa", txtPesquisa.Text.Trim() },
                { "@sExibeComercial", ddlsExibeComercial.SelectedValue },
                { "@sExibeComposicao", ddlsExibeComposicao.SelectedValue },
                { "@sSituacao", ddlsSituacao.SelectedValue },
                { "@sTipo", sTipo },
                { "@sSituacaoCadastral", ddlSituacaoCadastral.SelectedValue },
                { "@sSubTipo", ddlsSubTipo.SelectedValue },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Produtos", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", Grid.DataBindComScript(dtgvConsulta, tb, 0, "desc"), true);

                HyperLinkField hpl0 = dtgvConsulta.Columns[0] as HyperLinkField;
                HyperLinkField hpl1 = dtgvConsulta.Columns[1] as HyperLinkField;
                HyperLinkField hpl2 = dtgvConsulta.Columns[2] as HyperLinkField;

                if (hpl0 != null && hpl1 != null && hpl2 != null)
                {
                    hpl0.DataNavigateUrlFormatString += string.Format("&stp={0}", sTipo);
                    hpl1.DataNavigateUrlFormatString += string.Format("&stp={0}", sTipo);
                    hpl2.DataNavigateUrlFormatString += string.Format("&stp={0}", sTipo);
                }

                pnResultado.Visible = true;
                pnResultado.Style["display"] = "block";
            }
            else
            {
                pnMensagem.Visible = true;
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");

                return null;
            }

            RegistraScript("");

            return tb;
        }

        protected void cmdExportar_Click(object sender, EventArgs e)
        {
            string tipo = string.IsNullOrEmpty(Request["stp"]) ? string.Empty : Request["stp"].Trim();
            var dt = string.IsNullOrEmpty(tipo) ? Pesquisar() : PesquisarProdutos_x_Tipo();

            if (dt != null)
            {
                dt.Columns["sCodigo"].ColumnName = "sCodigo";
                dt.Columns["sDscProduto"].ColumnName = "sDscItem";
                dt.Columns["sSituacao_Completa"].ColumnName = "sDsc_1";
                dt.Columns["sSituacaoCadastral_Completa"].ColumnName = "sDsc_2";
                dt.Columns["dtAtualizacao"].ColumnName = "data_1";
                dt.Columns["sDscUsuarioAtualizacao"].ColumnName = "sDsc_3";

                // --------------------------------------------
                // Gabriel Llanir - 02/09/2024
                try
                {
                    ReportViewer rv = new ReportViewer();

                    rv.LocalReport.ReportPath = "App\\Reports\\Excel_Geral.rdlc";

                    rv.LocalReport.DataSources.Add(new ReportDataSource("ds_Consulta", dt));

                    ReportParameter[] rp = new ReportParameter[2];

                    rp[0] = new ReportParameter("Aparece_Tabela", "Produtos_");
                    rp[1] = new ReportParameter("Titulo_Personalizado", tipo.Equals("1") ? "Serviços" : tipo.Equals("2") ? "Recursos" : tipo.Equals("3") ? "Sub-Serviços" : "Produtos");

                    rv.LocalReport.SetParameters(rp);
                    rv.LocalReport.Refresh();

                    byte[] bytes = rv.LocalReport.Render("Excel", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);
                    string sNomeArquivo = "Consulta_Produtos_" + FUNCOES.CarimboDataHora() + ".xlsx";

                    File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);

                    FUNCOES.DownloadArquivo(Page, sNomeArquivo);

                    MensagemPagina.MostraMensagem_Sucesso("Relatório em Excel gerado com sucesso!", true);
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Houve um erro ao Gerar o Relatório em Excel da Consulta!<br />" + ex.InnerException + "<br />" + ex.Message, true);
                }
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            if (Request["stp"] != null) PesquisarProdutos_x_Tipo();
            else Pesquisar();

            pnResultado.Style["display"] = "block";
        }

        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            if (Request["stp"] == "1") FUNCOES.DirecionaPagina(sPagina_NovoRegistroServico);
            else if (Request["stp"] == "2") FUNCOES.DirecionaPagina(sPagina_NovoRegistroRecurso);
            else if (Request["stp"] == "3") FUNCOES.DirecionaPagina(sPagina_NovoRegistroSubServico);
            else FUNCOES.DirecionaPagina(sPagina_NovoRegistro);
        }

        protected void cmdFecharModal_Click(object sender, EventArgs e)
        {
            ProdutosImportador.FecharModal("modalImportarEAN");
            cmdPesquisar_Click(sender, e);
        }

        protected void dtgvConsulta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Duplicar")
            {
                if (Request["stp"] != null)
                    FUNCOES.DirecionaPagina(string.Format("App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}&stp={1}&duplicar=true", e.CommandArgument, Request["stp"]));
                else
                    FUNCOES.DirecionaPagina(string.Format("App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}&duplicar=true", e.CommandArgument));
            }
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sStatus = DataBinder.Eval(e.Row.DataItem, "sSituacaoCadastral").ToString();

                if (sStatus == "Sem classificaçaõ")
                {
                    e.Row.CssClass = "success";
                }
                else if (sStatus == "Classificados")
                {
                    e.Row.CssClass = "warning";
                }
            }
        }

        protected void RegistraScript(string str)
        {
            string codigoJavaScript = Server.MapPath("~/App/JS/Mascaras.js");
            string script = File.ReadAllText(codigoJavaScript);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", script, true);
        }
    }
}
