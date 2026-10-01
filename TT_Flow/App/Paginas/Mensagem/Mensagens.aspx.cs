using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using TT.FrameWork;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using System.Data.SqlClient;
using static TT.FrameWork.Identity;
using System.Reflection.Emit;
using System.Security.Policy;
using TT_Flow.FrameWork;
using TT_Hub.App.Paginas.Manutencao;

//using IDENTITY = TT.FrameWork.Identity;
//using FUNCOES = TT.FrameWork.Funcoes;
//using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;

namespace TT_Flow.App.Paginas.Mensagem
{
    public partial class Mensagens : System.Web.UI.Page
    {
        private const int nLimiteRegistrosTabela = 500;
        string sTituloPagina = "Mensagens";
        string sPagina_NovoRegistro = "App/Paginas/Mensagem/Mensagens_Detalhe.aspx?id=0";


        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Mensagens.Consultar, true);

            GarantirTiposEmailPopulados();

            if (!IsPostBack)
            {
                PopularDepartamentos();
                AplicarVisibilidadeFiltros();
                cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Mensagens.Enviar, false);

                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                pnMensagem.Visible = false;
                pnlConversasLidos.Visible = false;

                if (Request["sFiltro"] != null)
                {
                    ddlidFiltroMensagem.SelectedValue = Request["sFiltro"];
                }
                else
                {
                    ddlidFiltroMensagem.SelectedValue = "1";
                }

                if (Request["sCategoria"] != null && ddlCategoriaMensagem != null)
                {
                    string sCategoria = Request["sCategoria"].Trim().ToUpperInvariant();
                    if (ddlCategoriaMensagem.Items.FindByValue(sCategoria) != null)
                    {
                        ddlCategoriaMensagem.SelectedValue = sCategoria;
                    }
                }

                Pesquisar();
            }
            else
            {
                AplicarVisibilidadeFiltros();
                if (ddlidDepartamento.Items.Count == 0)
                {
                    PopularDepartamentos();
                }
            }

            txtPesquisa.Focus();
        }

        private void PopularDepartamentos()
        {
            List<string> selecionados = RecuperarDepartamentosSelecionadosLista();
            ddlidDepartamento.Items.Clear();

            DataTable tbDepartamentos = ConsultarDepartamentosMensagens();

            if (tbDepartamentos == null || tbDepartamentos.Rows.Count == 0)
            {
                try
                {
                    tbDepartamentos = BD.ExecutarDataTable("sp_Select 'Departamentos_x_Usuarios', " + IDENTITY.Variaveis.idUsuario());
                }
                catch
                {
                    tbDepartamentos = null;
                }
            }

            if (tbDepartamentos == null || tbDepartamentos.Rows.Count == 0)
            {
                tbDepartamentos = BD.ExecutarDataTable("sp_Select 'Flow_Departamentos'");
            }

            PopularDepartamentos_DataBind(tbDepartamentos, selecionados);
        }

        private DataTable ConsultarDepartamentosMensagens()
        {
            try
            {
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR_DEPARTAMENTOS_MENSAGENS");
                vParametros.Add("@idUsuarioPesquisa", IDENTITY.Variaveis.idUsuario());

                DataSet dsDepartamentos = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Mensagens", vParametros);

                if (BD.ValidarDataSet(dsDepartamentos))
                {
                    return dsDepartamentos.Tables[0];
                }
            }
            catch
            {
            }

            return null;
        }

        private void PopularDepartamentos_DataBind(DataTable tbDepartamentos, List<string> selecionados)
        {
            if (tbDepartamentos == null || tbDepartamentos.Rows.Count == 0)
            {
                return;
            }

            ddlidDepartamento.DataSource = tbDepartamentos;
            ddlidDepartamento.DataValueField = "idDepartamento";
            ddlidDepartamento.DataTextField = tbDepartamentos.Columns.Contains("sDscDepartamento")
                ? "sDscDepartamento"
                : tbDepartamentos.Columns[1].ColumnName;
            ddlidDepartamento.DataBind();

            foreach (ListItem item in ddlidDepartamento.Items)
            {
                item.Selected = selecionados.Contains(item.Value);
            }
        }

        private void PopularTiposEmail()
        {
            if (ddlTiposEmail == null)
            {
                return;
            }

            FUNCOES.Popula_Combo(ddlTiposEmail, "sp_Select 'Flow_Mensagens_x_TipoObjeto'", "idTipoObjeto", "sDscTipoObjeto", false, "Todos os tipos de e-mail", "-99");
        }

        private void GarantirTiposEmailPopulados()
        {
            if (ddlTiposEmail != null && ddlTiposEmail.Items.Count == 0)
            {
                PopularTiposEmail();
            }
        }

        private bool ExibirSelecaoMensagens()
        {
            return ddlidFiltroMensagem != null
                && ddlidFiltroMensagem.SelectedValue == "2";
        }

        private void AplicarVisibilidadeSelecao()
        {
            bool bExibir = ExibirSelecaoMensagens()
                && pnResultado.Visible
                && dtgvConsulta.Rows.Count > 0;

            pnlConversasLidos.Visible = bExibir;
            cmdConversas_Lidos.Visible = bExibir;

            if (hddExibirSelecaoMensagens != null)
            {
                hddExibirSelecaoMensagens.Value = bExibir ? "1" : "0";
            }
        }

        private void AplicarVisibilidadeFiltros()
        {
            if (pnlTiposEmail == null || ddlCategoriaMensagem == null)
            {
                return;
            }

            bool bEmail = ddlCategoriaMensagem.SelectedValue == "EMAIL";
            string sClasseSlot = "col-lg-3 col-md-6 col-sm-12 mensagens-filtro-slot";
            pnlTiposEmail.CssClass = bEmail ? sClasseSlot : sClasseSlot + " is-inactive";
            pnlFiltroDepartamentos.CssClass = bEmail
                ? "col-lg-5 col-md-6 col-sm-12"
                : "col-lg-7 col-md-8 col-sm-12";
            pnlFiltroAssunto.CssClass = bEmail
                ? "col-lg-4 col-md-12 col-sm-12"
                : "col-lg-5 col-md-12 col-sm-12";
        }

        private void LimparTiposEmail()
        {
            if (ddlTiposEmail == null || ddlTiposEmail.Items.Count == 0)
            {
                return;
            }

            ListItem itemTodos = ddlTiposEmail.Items.FindByValue("-99");
            ddlTiposEmail.ClearSelection();

            if (itemTodos != null)
            {
                itemTodos.Selected = true;
            }
            else
            {
                ddlTiposEmail.SelectedIndex = 0;
            }
        }

        private string RecuperarDepartamentosSelecionados()
        {
            return string.Join(",", RecuperarDepartamentosSelecionadosLista());
        }

        private List<string> RecuperarDepartamentosSelecionadosLista()
        {
            return ddlidDepartamento.Items.Cast<ListItem>()
                .Where(item => item.Selected)
                .Select(item => item.Value)
                .ToList();
        }

        private void LimparSelecaoDepartamentos()
        {
            foreach (ListItem item in ddlidDepartamento.Items)
            {
                item.Selected = false;
            }
        }

        private string LimparTextoDataFiltro(string sData)
        {
            if (string.IsNullOrWhiteSpace(sData))
            {
                return "";
            }

            return sData.Trim().Replace("_", "");
        }

        private string NormalizarDataFiltroSql(string sData)
        {
            string sDataLimpa = LimparTextoDataFiltro(sData);
            if (string.IsNullOrWhiteSpace(sDataLimpa))
            {
                return "";
            }

            string[] formatos = { "dd/MM/yyyy", "d/M/yyyy", "dd/MM/yy", "yyyy-MM-dd" };
            CultureInfo culturaPtBr = CultureInfo.GetCultureInfo("pt-BR");
            DateTime dt;

            if (DateTime.TryParseExact(sDataLimpa, formatos, culturaPtBr, DateTimeStyles.None, out dt)
                || DateTime.TryParse(sDataLimpa, culturaPtBr, DateTimeStyles.None, out dt))
            {
                // ISO (yyyy-MM-dd) evita erro de conversão implícita no SQL Server (ex.: 17/06/2026).
                return dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            return "";
        }

        private bool ValidarPeriodoFiltro(out string sMensagemErro)
        {
            sMensagemErro = "";
            string sDataInicial = LimparTextoDataFiltro(txtdtEnvio.Text);
            string sDataFinal = LimparTextoDataFiltro(txtdtFinal.Text);
            bool bTemInicial = !string.IsNullOrWhiteSpace(sDataInicial);
            bool bTemFinal = !string.IsNullOrWhiteSpace(sDataFinal);

            if (!bTemInicial && !bTemFinal)
            {
                return true;
            }

            CultureInfo culturaPtBr = CultureInfo.GetCultureInfo("pt-BR");
            string[] formatos = { "dd/MM/yyyy", "d/M/yyyy", "dd/MM/yy", "yyyy-MM-dd" };

            if (bTemInicial)
            {
                if (!DateTime.TryParseExact(sDataInicial, formatos, culturaPtBr, DateTimeStyles.None, out DateTime dtInicial)
                    && !DateTime.TryParse(sDataInicial, culturaPtBr, DateTimeStyles.None, out dtInicial))
                {
                    sMensagemErro = "Data inicial inválida.";
                    return false;
                }
            }

            if (bTemFinal)
            {
                if (!DateTime.TryParseExact(sDataFinal, formatos, culturaPtBr, DateTimeStyles.None, out DateTime dtFinal)
                    && !DateTime.TryParse(sDataFinal, culturaPtBr, DateTimeStyles.None, out dtFinal))
                {
                    sMensagemErro = "Data final inválida.";
                    return false;
                }
            }

            if (bTemInicial && bTemFinal)
            {
                sMensagemErro = Validacoes.ValidaDatas(sDataInicial, sDataFinal);
                if (!string.IsNullOrEmpty(sMensagemErro))
                {
                    return false;
                }
            }

            return true;
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;
            pnMensagem.Visible = false;

            string sErroPeriodo;
            if (!ValidarPeriodoFiltro(out sErroPeriodo))
            {
                pnMensagem.Visible = true;
                MensagemPagina1.MostraMensagem_Erro(sErroPeriodo);
                cmdPesquisar.Text = "Buscar";
                return;
            }

            string sFuncao = "CONSULTAR_COM_FILTRO";
            DataTable tb;
            string sSql = "sp_Manipula_tbl_Flow_Mensagens";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idUsuarioPesquisa", IDENTITY.Variaveis.idUsuario());
            vParametros.Add("@idDepartamentoDestino", "0");
            vParametros.Add("@sDepartamentosDestino", RecuperarDepartamentosSelecionados());
            vParametros.Add("@dtInicial", NormalizarDataFiltroSql(txtdtEnvio.Text));
            vParametros.Add("@dtFinal", NormalizarDataFiltroSql(txtdtFinal.Text));
            vParametros.Add("@sAssunto", txtPesquisa.Text);
            vParametros.Add("@idUsuarioDestino", IDENTITY.Variaveis.idUsuario());
            vParametros.Add("@idFiltroMensagem", ddlidFiltroMensagem.SelectedValue.ToString());
            vParametros.Add("@sDirecaoMensagem", ddlsDirecaoMensagem.SelectedValue);
            string sCategoriaMensagem = ddlCategoriaMensagem != null ? ddlCategoriaMensagem.SelectedValue : "TODOS";
            string sTipoObjeto = "-99";
            if (sCategoriaMensagem == "EMAIL" && ddlTiposEmail != null && ddlTiposEmail.Items.Count > 0)
            {
                sTipoObjeto = ddlTiposEmail.SelectedValue;
            }

            vParametros.Add("@sCategoriaMensagem", sCategoriaMensagem);
            vParametros.Add("@idTipoObjeto", sTipoObjeto);

            if (sCategoriaMensagem == "AVISO")
            {
                vParametros.Add("@sIsAviso", "S");
            }
            else if (sCategoriaMensagem == "MENSAGEM")
            {
                vParametros.Add("@sIsAviso", "N");
            }

            try
            {
                tb = BD.ExecutarDataTable(sSql, vParametros, false);
            }
            catch (Exception ex)
            {
                pnMensagem.Visible = true;
                MensagemPagina1.MostraMensagem_Erro("Erro ao consultar mensagens: " + ex.Message);
                cmdPesquisar.Text = "Buscar";
                return;
            }

            tb = PrepararTabelaListagem(tb);

            if (tb.Rows.Count > 0)
            {
                dtgvConsulta.DataSource = tb;
                dtgvConsulta.DataBind();
                if (dtgvConsulta.HeaderRow != null)
                {
                    dtgvConsulta.UseAccessibleHeader = true;
                    dtgvConsulta.HeaderRow.TableSection = TableRowSection.TableHeader;
                }

                pnResultado.Visible = true;
            }
            else
            {
                pnMensagem.Visible = true;
                MensagemPagina1.MostraMensagem_Erro("Nenhuma mensagem localizada para sua pesquisa!");
            }

            cmdConversas_Lidos.Enabled = true;
            cmdPesquisar.Text = "Buscar";
            AplicarVisibilidadeSelecao();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (pnResultado.Visible)
            {
                AplicarVisibilidadeSelecao();
            }
        }

        private DataTable PrepararTabelaListagem(DataTable tb)
        {
            if (tb == null)
            {
                return new DataTable();
            }

            GarantirColunaTags(tb);

            bool bResultadoLimitado = false;
            if (tb.Rows.Count > nLimiteRegistrosTabela)
            {
                DataTable tbLimitada = tb.Clone();
                for (int i = 0; i < nLimiteRegistrosTabela; i++)
                {
                    tbLimitada.ImportRow(tb.Rows[i]);
                }

                tb = tbLimitada;
                bResultadoLimitado = true;
            }

            RemoverColunasNaoUtilizadas(tb);

            if (bResultadoLimitado)
            {
                MensagemPagina.MostraMensagem_Aviso("Foram exibidas as " + nLimiteRegistrosTabela + " mensagens mais recentes encontradas. Refine os filtros para localizar mensagens mais antigas.", false);
            }

            return tb;
        }

        private void GarantirColunaTags(DataTable tb)
        {
            if (tb.Columns.Cast<DataColumn>().Any(c => c.ColumnName.Equals("sTags", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            tb.Columns.Add("sTags", typeof(string));
            foreach (DataRow row in tb.Rows)
            {
                row["sTags"] = "";
            }
        }

        private void RemoverColunasNaoUtilizadas(DataTable tb)
        {
            HashSet<string> colunasUtilizadas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "idMensagem",
                "dtInclusao",
                "sDscUsuarioRemetente",
                "sAssunto",
                "sDestinatario",
                "sTags",
                "sIsAviso",
                "dtLeitura"
            };

            for (int i = tb.Columns.Count - 1; i >= 0; i--)
            {
                if (!colunasUtilizadas.Contains(tb.Columns[i].ColumnName))
                {
                    tb.Columns.RemoveAt(i);
                }
            }
        }


        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        protected void cmdLimpar_Click(object sender, EventArgs e)
        {
            txtdtEnvio.Text = "";
            txtdtFinal.Text = "";
            txtPesquisa.Text = "";
            ddlsDirecaoMensagem.SelectedValue = "Recebida";
            ddlidFiltroMensagem.SelectedValue = "1";
            ddlCategoriaMensagem.SelectedValue = "TODOS";
            GarantirTiposEmailPopulados();
            LimparTiposEmail();
            LimparSelecaoDepartamentos();
            AplicarVisibilidadeFiltros();

            Pesquisar();
        }

        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Mensagens.Enviar, true);
            FUNCOES.DirecionaPagina(sPagina_NovoRegistro);
        }

        public class nQtdMensagens
        {
            nQtdMensagens NumeroMensagem = new nQtdMensagens();
        }

        protected void ddlidFiltroMensagem_SelectedIndexChanged(object sender, EventArgs e)
        {
            Pesquisar();
        }

        protected void ddlCategoriaMensagem_SelectedIndexChanged(object sender, EventArgs e)
        {
            GarantirTiposEmailPopulados();
            LimparTiposEmail();
            AplicarVisibilidadeFiltros();
            Pesquisar();
        }

        protected void ddlTipoCaixa_SelectedIndexChanged(object sender, EventArgs e)
        {
            Pesquisar();
        }

        protected void ddlTisAviso_SelectedIndexChanged(object sender, EventArgs e)
        {
            Pesquisar();
        }


        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            bool bExibirSelecao = ExibirSelecaoMensagens();

            CheckBox chkSelecionar = e.Row.FindControl("chkConversas_Seleciona") as CheckBox;
            if (chkSelecionar != null)
            {
                chkSelecionar.Visible = bExibirSelecao;
                chkSelecionar.Enabled = bExibirSelecao;
            }

            CheckBox chkSelecionarTodos = e.Row.FindControl("chkConversas_SelecionaTodos") as CheckBox;
            if (chkSelecionarTodos != null)
            {
                chkSelecionarTodos.Visible = bExibirSelecao;
                chkSelecionarTodos.Enabled = bExibirSelecao;
            }

            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.Cells.Count > 0)
            {
                var dataItem = e.Row.DataItem as DataRowView;

                Repeater rptTagsMensagem = e.Row.FindControl("rptTagsMensagem") as Repeater;
                Panel pnTagsMensagem = e.Row.FindControl("pnTagsMensagem") as Panel;
                if (rptTagsMensagem != null && dataItem != null)
                {
                    string sTags = dataItem.Row.Table.Columns.Contains("sTags")
                        ? Convert.ToString(dataItem.Row["sTags"])
                        : "";
                    string[] tags = (sTags ?? "").Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    rptTagsMensagem.DataSource = tags;
                    rptTagsMensagem.DataBind();

                    if (pnTagsMensagem != null)
                    {
                        pnTagsMensagem.Visible = tags.Length > 0;
                    }
                }

                string nomeColuna = "dtLeitura";
                var dtLeitura = dataItem != null && dataItem.Row.Table.Columns.Contains(nomeColuna)
                    ? dataItem.Row[nomeColuna].ToString()
                    : "";

                if (dtLeitura == "" && dataItem != null && dataItem.Row.Table.Columns.Contains(nomeColuna))
                {
                    foreach (TableCell cell in e.Row.Cells)
                    {
                        cell.Font.Bold = true;
                    }
                }
            }
        }

        protected void cmdConversas_Lidos_Click(object sender, EventArgs e)
        {
            var idsSelecionados = new List<string>();

            foreach (GridViewRow item in dtgvConsulta.Rows)
            {
                CheckBox chkConversas_Seleciona_Linha = (CheckBox)item.FindControl("chkConversas_Seleciona");
                if (chkConversas_Seleciona_Linha != null && chkConversas_Seleciona_Linha.Checked)
                {
                    idsSelecionados.Add(dtgvConsulta.DataKeys[item.RowIndex]["idMensagem"].ToString());
                }
            }

            if (idsSelecionados.Count == 0)
            {
                MensagemPagina.MostraMensagem_Erro("Selecione ao menos uma mensagem para marcar como lida.");
                Pesquisar();
                return;
            }

            Mensagens_Acao_DesabilitarBotao("Ler Mensagens");

            foreach (string idMensagem in idsSelecionados)
            {
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "LER_MENSAGEM");
                vParametros.Add("@idMensagem", idMensagem);
                vParametros.Add("@idUsuarioLeitura", IDENTITY.Variaveis.idUsuario());
                vParametros.Add("@sDirecaoMensagem", ddlsDirecaoMensagem.SelectedValue);
                BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Mensagens", vParametros);
            }

            string sMensagemSucesso = idsSelecionados.Count == 1
                ? "Mensagem marcada como lida com sucesso."
                : string.Format("{0} mensagens marcadas como lidas com sucesso.", idsSelecionados.Count);

            MensagemPagina.MostraMensagem_Sucesso(sMensagemSucesso);
            Pesquisar();
        }
    
        void Mensagens_Acao_DesabilitarBotao(string sTitulo)
        {
            cmdConversas_Lidos.Enabled = false;
        }

        protected void btnSelecionarTodos_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow item in dtgvConsulta.Rows)
            {
                CheckBox chkConversas_Seleciona_Linha = (CheckBox)item.FindControl("chkConversas_Seleciona");
                chkConversas_Seleciona_Linha.Checked = true;
            }
        }

        protected void chkConversas_SelecionaTodos_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkConversas_Seleciona_Todos = (CheckBox)dtgvConsulta.HeaderRow.Cells[0].FindControl("chkConversas_SelecionaTodos");
            foreach (GridViewRow item in dtgvConsulta.Rows)
            {

                CheckBox chkConversas_Seleciona_Linha = (CheckBox)item.FindControl("chkConversas_Seleciona");
                chkConversas_Seleciona_Linha.Checked = chkConversas_Seleciona_Todos.Checked;
                
            }

        }
    }
}
