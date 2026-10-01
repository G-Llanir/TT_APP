using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using TT.FrameWork;
using TT_Flow.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using Identity = TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.Adm.Financeiro
{
    public partial class FluxoCaixa : Page
    {
        private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("pt-BR");
        private readonly List<CampoFluxo> _categorias = new List<CampoFluxo>();
        private bool _consultaCarregada;
        private string sPeriodoAtual;
        private List<Dictionary<string, string>> _categoriasTipoCols = new List<Dictionary<string, string>>();

        private sealed class CampoFluxo
        {
            public string Nome;
            public string Titulo;
            public string Classe;
            public string Destino;
            public string Parametro;
            public bool Saldo;
        }

        private sealed class GradeFluxo
        {
            public string Titulo;
            public string Contas;
            public string Empresas;
            public bool Banco;
            public DataTable Dados;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Financeiro.FluxoCaixa.Consultar, true);
            // Todos os valores dos filtros já foram carregados no postback.
            // O Page_PreRender executa uma única consulta com o conjunto atualizado.
            chkPorEmpresa.Attributes["onchange"] = ClientScript.GetPostBackEventReference(chkPorEmpresa, string.Empty);
            chkPorBanco.Attributes["onchange"] = ClientScript.GetPostBackEventReference(chkPorBanco, string.Empty);
            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = "Pool Bancário";
                Funcoes.Popula_Combo(lstContaBancaria, "sp_Select 'tbl_Flow_Adm_ContasBancarias'", "idConta", "sDscConta", false);
                Funcoes.Popula_Combo(lstEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + Identity.Variaveis.idUsuario(), "idEmpresa", "sDscCodigoEmpresa", false);
                txtdtInicio.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtdtFinal.Text = DateTime.Today.AddDays(30).ToString("yyyy-MM-dd");
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!_consultaCarregada)
                CarregarFluxoCaixa();
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            CarregarFluxoCaixa();
        }

        private static string Selecionados(ListControl lista)
        {
            return string.Join(";", lista.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value));
        }

        private void CarregarFluxoCaixa()
        {
            _consultaCarregada = true;
            phGrades.Controls.Clear();
            try
            {
                DateTime inicio, fim;
                if (!DateTime.TryParseExact(txtdtInicio.Text, "yyyy-MM-dd", Cultura, DateTimeStyles.None, out inicio) ||
                    !DateTime.TryParseExact(txtdtFinal.Text, "yyyy-MM-dd", Cultura, DateTimeStyles.None, out fim) || inicio > fim)
                {
                    MensagemPagina.MostraMensagem_Erro("Informe um período válido, com a data inicial anterior ou igual à final.");
                    return;
                }
                sPeriodoAtual = ddlPeriodicidade.SelectedValue;
                var parametros = new Dictionary<string, string>
                {
                    { "@sidConta", Selecionados(lstContaBancaria) },
                    { "@sidEmpresa", Selecionados(lstEmpresa) },
                    { "@sPeriodo", sPeriodoAtual },
                    { "@dtInicio", inicio.ToString("yyyy-MM-dd") },
                    { "@dtFinal", fim.ToString("yyyy-MM-dd") }
                };
                DataTable dados = BD.ExecutarDataTable("sp_Manipula_FluxoCaixa", parametros, false);
                if (dados == null || dados.Rows.Count == 0)
                {
                    MensagemPagina.MostraMensagem_Erro("Nenhum registro encontrado no período selecionado.");
                    return;
                }
                // A versão nova devolve uma empresa por linha, nunca uma lista de empresas misturadas.
                if (!dados.Columns.Contains("sDscEmpresa") || dados.AsEnumerable().Any(r => Convert.ToString(r["idEmpresa"]).Contains(";")))
                    throw new InvalidOperationException("Atualize a procedure do Pool Bancário para a versão com detalhamento por empresa e conta.");

                CarregarCategorias(dados);
                var grades = MontarGrades(dados, chkPorEmpresa.Checked, chkPorBanco.Checked,
                    parametros["@sidConta"], parametros["@sidEmpresa"]);
                for (int i = 0; i < grades.Count; i++)
                    RenderizarGrade(grades[i], i, ddlVisualizacao.SelectedValue == "C");
                PrepararDadosGrafico(grades[0].Dados, sPeriodoAtual, inicio, fim);
            }
            catch (Exception ex)
            {
                phGrades.Controls.Clear();
                System.Diagnostics.Trace.TraceError("Erro no Pool Bancário: {0}", ex);
                MensagemPagina.MostraMensagem_Erro("Erro ao carregar fluxo de caixa: " + HttpUtility.HtmlEncode(ex.Message));
            }
        }

        private void CarregarCategorias(DataTable dados)
        {
            _categorias.Clear();
            _categoriasTipoCols.Clear();
            foreach (DataColumn coluna in dados.Columns)
            {
                if (!coluna.ColumnName.StartsWith("Tipo_", StringComparison.Ordinal))
                    continue;
                string[] partes = coluna.ColumnName.Split(new[] { '_' }, 3);
                int id;
                if (partes.Length != 3 || !int.TryParse(partes[1], out id))
                    throw new InvalidOperationException("Categoria inválida no retorno do fluxo de caixa.");
                _categorias.Add(new CampoFluxo { Nome = coluna.ColumnName, Titulo = partes[2].Replace('.', ' '),
                    Classe = "saida-vermelha fluxo-celula-saidas", Destino = "ContasPagar.aspx",
                    Parametro = "adiantamento=N&idCategoriaTipo=" + (id == 0 ? -1 : id) });
                _categoriasTipoCols.Add(new Dictionary<string, string> { { "idCategoria", partes[1] }, { "sDscCategoria", partes[2] } });
            }
        }

        private List<GradeFluxo> MontarGrades(DataTable dados, bool empresas, bool bancos, string contasFiltro, string empresasFiltro)
        {
            var linhas = dados.AsEnumerable().ToList();
            var grades = new List<GradeFluxo>();
            grades.Add(new GradeFluxo { Titulo = "Todas as empresas", Contas = contasFiltro, Empresas = empresasFiltro, Dados = Consolidar(linhas) });
            if (empresas)
            {
                foreach (var empresa in linhas.GroupBy(r => Convert.ToInt32(r["idEmpresa"]))
                    .OrderBy(g => Convert.ToString(g.First()["sDscEmpresa"])))
                {
                    string nome = Convert.ToString(empresa.First()["sDscEmpresa"]);
                    grades.Add(new GradeFluxo { Titulo = "Empresa: " + nome, Contas = contasFiltro,
                        Empresas = empresa.Key.ToString(), Dados = Consolidar(empresa) });
                    if (bancos)
                        AdicionarBancos(grades, empresa, empresa.Key.ToString());
                }
            }
            else if (bancos)
                AdicionarBancos(grades, linhas, empresasFiltro);
            return grades;
        }

        private void AdicionarBancos(List<GradeFluxo> grades, IEnumerable<DataRow> linhas, string empresas)
        {
            foreach (var banco in linhas.GroupBy(r => Convert.ToInt32(r["idConta"]))
                .OrderBy(g => Convert.ToString(g.First()["sDscConta"])))
                grades.Add(new GradeFluxo { Titulo = "Banco / conta: " + Convert.ToString(banco.First()["sDscConta"]),
                    Banco = true, Contas = banco.Key.ToString(), Empresas = empresas, Dados = Consolidar(banco) });
        }

        private static decimal Valor(DataRow linha, string campo)
        {
            return linha.IsNull(campo) ? 0m : Convert.ToDecimal(linha[campo]);
        }

        private DataTable Consolidar(IEnumerable<DataRow> origem)
        {
            var linhas = origem.OrderBy(r => Convert.ToDateTime(r["dtReferencia"])).ToList();
            DataTable resultado = new DataTable();
            resultado.Columns.Add("sPeriodo", typeof(string));
            resultado.Columns.Add("dtReferencia", typeof(DateTime));
            resultado.Columns.Add("dtFimPeriodo", typeof(DateTime));
            var campos = CamposLinhas().Where(c => c.Nome != "sPeriodo").Select(c => c.Nome).ToList();
            foreach (string campo in campos) resultado.Columns.Add(campo, typeof(decimal));
            // Cada par empresa/conta contribui uma única vez para o saldo de abertura.
            decimal saldo = linhas.GroupBy(r => new { Empresa = Convert.ToInt32(r["idEmpresa"]), Conta = Convert.ToInt32(r["idConta"]) })
                .Sum(g => Valor(g.First(), "nSaldoAnterior"));
            foreach (var periodo in linhas.GroupBy(r => Convert.ToDateTime(r["dtReferencia"])))
            {
                DataRow linha = resultado.NewRow();
                linha["dtReferencia"] = periodo.Key;
                linha["dtFimPeriodo"] = periodo.Max(r => Convert.ToDateTime(r["dtFimPeriodo"]));
                linha["sPeriodo"] = periodo.First()["sPeriodo"];
                foreach (string campo in campos.Where(c => c != "nSaldoAnterior" && c != "nSaldoAtual"))
                    linha[campo] = periodo.Sum(r => Valor(r, campo));
                linha["nSaldoAnterior"] = saldo;
                saldo += Valor(linha, "nTotalEntradas") - Valor(linha, "nTotalSaidas");
                linha["nSaldoAtual"] = saldo;
                resultado.Rows.Add(linha);
            }
            return resultado;
        }

        private List<CampoFluxo> CamposLinhas()
        {
            var campos = new List<CampoFluxo>
            {
                new CampoFluxo { Nome = "sPeriodo", Titulo = "Período", Classe = "fluxo-periodo" },
                new CampoFluxo { Nome = "nSaldoAnterior", Titulo = "Saldo Inicial", Classe = "fluxo-saldo-valor fluxo-fim-bloco", Saldo = true },
                new CampoFluxo { Nome = "Cliente A+", Titulo = "Cliente A+", Classe = "entrada-azul fluxo-celula-entradas", Destino = "ContasReceber.aspx", Parametro = "sClientePontual=S&adiantamento=N" },
                new CampoFluxo { Nome = "Cliente A-", Titulo = "Cliente A-", Classe = "entrada-azul fluxo-celula-entradas", Destino = "ContasReceber.aspx", Parametro = "sClientePontual=N&adiantamento=N" },
                new CampoFluxo { Nome = "nTotalEntradas", Titulo = "Total de Entradas", Classe = "entrada-azul fluxo-celula-entradas fluxo-total fluxo-fim-bloco" }
            };
            campos.AddRange(_categorias);
            campos.AddRange(new[] {
                new CampoFluxo { Nome = "nTotalSaidas", Titulo = "Total de Saídas", Classe = "saida-vermelha fluxo-celula-saidas fluxo-total fluxo-fim-bloco" },
                new CampoFluxo { Nome = "Adiantamento Receber", Titulo = "Adiantamento Receber", Classe = "entrada-azul fluxo-celula-adiantamento", Destino = "ContasReceber.aspx", Parametro = "adiantamento=S" },
                new CampoFluxo { Nome = "Adiantamento Pagar", Titulo = "Adiantamento Pagar", Classe = "saida-vermelha fluxo-celula-adiantamento fluxo-fim-bloco", Destino = "ContasPagar.aspx", Parametro = "adiantamento=S" },
                new CampoFluxo { Nome = "nSaldoAtual", Titulo = "Saldo Final", Classe = "fluxo-coluna-saldo fluxo-saldo-valor", Saldo = true }
            });
            return campos;
        }

        private void RenderizarGrade(GradeFluxo grade, int indice, bool transposta)
        {
            var painel = new Panel { CssClass = "panel panel-primary fluxo-painel" + (grade.Banco ? " fluxo-painel-banco" : "") };
            var titulo = new HtmlGenericControl("h3");
            titulo.Attributes["class"] = "panel-title";
            titulo.InnerText = grade.Titulo;
            var cabecalho = new Panel { CssClass = "panel-heading" };
            cabecalho.Controls.Add(titulo);
            painel.Controls.Add(cabecalho);
            var corpo = new Panel { CssClass = "panel-body" };
            var scroll = new Panel { CssClass = "table-responsive" };
            var grid = new GridView { ID = "fluxo_" + indice, AutoGenerateColumns = false,
                EnableViewState = false, GridLines = GridLines.None, CssClass = "table table-bordered fluxo-tabela" + (transposta ? " fluxo-transposta" : ""),
                Width = Unit.Percentage(100), ShowFooter = !transposta, UseAccessibleHeader = true };
            scroll.Controls.Add(grid);
            corpo.Controls.Add(scroll);
            painel.Controls.Add(corpo);
            phGrades.Controls.Add(painel);
            if (transposta) PreencherTransposta(grid, grade);
            else PreencherLinhas(grid, grade);
        }

        private void PreencherLinhas(GridView grid, GradeFluxo grade)
        {
            var campos = CamposLinhas();
            foreach (var campo in campos)
            {
                var coluna = new BoundField { DataField = campo.Nome, HeaderText = campo.Titulo, HtmlEncode = true };
                coluna.HeaderStyle.CssClass = campo.Classe;
                coluna.ItemStyle.CssClass = campo.Classe;
                coluna.FooterStyle.CssClass = campo.Classe;
                coluna.ItemStyle.HorizontalAlign = campo.Nome == "sPeriodo" ? HorizontalAlign.Center : HorizontalAlign.Right;
                coluna.FooterStyle.HorizontalAlign = HorizontalAlign.Right;
                grid.Columns.Add(coluna);
            }
            grid.RowDataBound += (s, e) => {
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                DataRow dados = ((DataRowView)e.Row.DataItem).Row;
                for (int i = 1; i < campos.Count; i++)
                    PreencherValor(e.Row.Cells[i], campos[i], dados, grade);
            };
            grid.DataSource = grade.Dados;
            grid.DataBind();
            if (grid.HeaderRow == null) return;
            grid.HeaderRow.TableSection = TableRowSection.TableHeader;
            var grupos = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal) { TableSection = TableRowSection.TableHeader };
            Grupo(grupos, "", 2, "fluxo-fim-bloco");
            Grupo(grupos, "Entradas", 3, "fluxo-grupo-entradas fluxo-fim-bloco");
            Grupo(grupos, "Saídas", _categorias.Count + 1, "fluxo-grupo-saidas fluxo-fim-bloco");
            Grupo(grupos, "Adiantamento", 2, "fluxo-grupo-adiantamento fluxo-fim-bloco");
            Grupo(grupos, "", 1, "");
            ((Table)grid.HeaderRow.Parent).Rows.AddAt(0, grupos);
            grid.FooterRow.CssClass = "fluxo-rodape";
            grid.FooterRow.TableSection = TableRowSection.TableFooter;
            grid.FooterRow.Cells[0].Text = "Total do período";
            for (int i = 1; i < campos.Count; i++)
            {
                grid.FooterRow.Cells[i].Text = TotalCampo(grade.Dados, campos[i]).ToString("C2", Cultura);
                if (campos[i].Saldo)
                    grid.FooterRow.Cells[i].ToolTip = campos[i].Nome == "nSaldoAnterior" ? "Saldo inicial do primeiro período" : "Saldo final do último período";
            }
        }

        private static void Grupo(GridViewRow linha, string texto, int quantidade, string classe)
        {
            var celula = new TableHeaderCell { Text = texto, ColumnSpan = quantidade, CssClass = "fluxo-grupo " + classe };
            if (texto.Length > 0) celula.Attributes["scope"] = "colgroup";
            linha.Cells.Add(celula);
        }

        private decimal ValorCampo(DataRow linha, CampoFluxo campo)
        {
            if (campo.Nome == "SaidasSemAdiantamento")
                return Valor(linha, "nTotalSaidas") - Valor(linha, "Adiantamento Pagar");
            if (campo.Nome == "SaldoAposSaidas")
                return Valor(linha, "nSaldoAnterior") - Valor(linha, "nTotalSaidas") + Valor(linha, "Adiantamento Pagar");
            return Valor(linha, campo.Nome);
        }

        private decimal TotalCampo(DataTable dados, CampoFluxo campo)
        {
            if (campo.Nome == "nSaldoAnterior") return ValorCampo(dados.Rows[0], campo);
            if (campo.Saldo) return ValorCampo(dados.Rows[dados.Rows.Count - 1], campo);
            return dados.AsEnumerable().Sum(r => ValorCampo(r, campo));
        }

        private void PreencherValor(TableCell celula, CampoFluxo campo, DataRow dados, GradeFluxo grade)
        {
            decimal valor = ValorCampo(dados, campo);
            celula.Controls.Clear();
            celula.Text = "";
            if (valor != 0 && campo.Destino != null)
            {
                var link = new HyperLink { Text = valor.ToString("C2", Cultura), Target = "_blank",
                    NavigateUrl = CriarLink(grade, dados, campo), CssClass = "fluxo-link" };
                link.Attributes["rel"] = "noopener";
                celula.Controls.Add(link);
            }
            else celula.Text = valor.ToString("C2", Cultura);
        }

        private static string CriarLink(GradeFluxo grade, DataRow dados, CampoFluxo campo)
        {
            return campo.Destino + "?Dashboard=fluxoCaixa&sidConta=" + HttpUtility.UrlEncode(grade.Contas) +
                "&sidEmpresa=" + HttpUtility.UrlEncode(grade.Empresas) +
                "&dtInicio=" + Convert.ToDateTime(dados["dtReferencia"]).ToString("yyyy-MM-dd") +
                "&dtFinal=" + Convert.ToDateTime(dados["dtFimPeriodo"]).ToString("yyyy-MM-dd") +
                "&" + campo.Parametro;
        }

        private void PreencherTransposta(GridView grid, GradeFluxo grade)
        {
            var padrao = CamposLinhas();
            var campos = new List<CampoFluxo> { padrao.First(c => c.Nome == "nSaldoAnterior") };
            campos.AddRange(_categorias);
            campos.Add(new CampoFluxo { Nome = "SaidasSemAdiantamento", Titulo = "Total de Saídas (sem adiantamento)", Classe = "saida-vermelha fluxo-celula-saidas fluxo-total" });
            campos.Add(new CampoFluxo { Nome = "SaldoAposSaidas", Titulo = "Saldo Após Saídas", Classe = "fluxo-saldo-valor fluxo-linha-separador", Saldo = true });
            campos.Add(padrao.First(c => c.Nome == "Cliente A+"));
            campos.Add(padrao.First(c => c.Nome == "Cliente A-"));
            campos.Add(padrao.First(c => c.Nome == "Adiantamento Pagar"));
            campos.Add(padrao.First(c => c.Nome == "Adiantamento Receber"));
            campos.Add(padrao.First(c => c.Nome == "nSaldoAtual"));
            var tabela = new DataTable();
            tabela.Columns.Add("Descricao", typeof(string));
            for (int i = 0; i < grade.Dados.Rows.Count; i++) tabela.Columns.Add("P" + i, typeof(decimal));
            tabela.Columns.Add("Total", typeof(decimal));
            foreach (CampoFluxo campo in campos)
            {
                var row = tabela.NewRow();
                row["Descricao"] = campo.Titulo;
                for (int i = 0; i < grade.Dados.Rows.Count; i++) row["P" + i] = ValorCampo(grade.Dados.Rows[i], campo);
                row["Total"] = TotalCampo(grade.Dados, campo);
                tabela.Rows.Add(row);
            }
            var descricao = new BoundField { DataField = "Descricao", HeaderText = "Contas / Período" };
            descricao.ItemStyle.CssClass = "fluxo-descricao fluxo-fim-bloco";
            descricao.HeaderStyle.CssClass = "fluxo-descricao fluxo-fim-bloco";
            grid.Columns.Add(descricao);
            for (int i = 0; i < grade.Dados.Rows.Count; i++)
            {
                var periodo = new BoundField { DataField = "P" + i, HeaderText = Convert.ToString(grade.Dados.Rows[i]["sPeriodo"]), DataFormatString = "{0:C2}" };
                periodo.HeaderStyle.CssClass = "fluxo-periodo";
                periodo.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                grid.Columns.Add(periodo);
            }
            var total = new BoundField { DataField = "Total", HeaderText = "Total do período", DataFormatString = "{0:C2}" };
            total.HeaderStyle.CssClass = "fluxo-total";
            total.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
            total.ItemStyle.CssClass = "fluxo-total";
            grid.Columns.Add(total);
            grid.RowDataBound += (s, e) => {
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                CampoFluxo campo = campos[e.Row.RowIndex];
                string classe = campo.Classe.Replace("fluxo-fim-bloco", "");
                foreach (TableCell celula in e.Row.Cells) celula.CssClass = (celula.CssClass + " " + classe).Trim();
                for (int i = 0; i < grade.Dados.Rows.Count; i++) PreencherValor(e.Row.Cells[i + 1], campo, grade.Dados.Rows[i], grade);
                if (campo.Nome == "nSaldoAtual") e.Row.CssClass = "fluxo-rodape";
                if (campo.Saldo) e.Row.Cells[e.Row.Cells.Count - 1].ToolTip = campo.Nome == "nSaldoAnterior" ? "Saldo inicial do primeiro período" : "Saldo do último período";
            };
            grid.DataSource = tabela;
            grid.DataBind();
            if (grid.HeaderRow != null) grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        private void PrepararDadosGrafico(DataTable tbGeral, string sPeriodo, DateTime dtInicio, DateTime dtFinal)
        {
            try
            {
                var labels = new System.Collections.Generic.List<string>();
                var entradas = new System.Collections.Generic.List<decimal>();
                var saidas = new System.Collections.Generic.List<decimal>();
                var saldo = new System.Collections.Generic.List<decimal>();

                foreach (DataRow row in tbGeral.Rows)
                {
                    string sPeriodoLabel = row["sPeriodo"] != DBNull.Value ? row["sPeriodo"].ToString() : "";
                    labels.Add(sPeriodoLabel);

                    decimal clienteAPositivo = row["Cliente A+"] != DBNull.Value ? Convert.ToDecimal(row["Cliente A+"]) : 0;
                    decimal clienteANegativo = row["Cliente A-"] != DBNull.Value ? Convert.ToDecimal(row["Cliente A-"]) : 0;
                    decimal adiantamentoReceber = row["Adiantamento Receber"] != DBNull.Value ? Convert.ToDecimal(row["Adiantamento Receber"]) : 0;
                    decimal totalEntradas = clienteAPositivo + clienteANegativo + adiantamentoReceber;
                    entradas.Add(totalEntradas);

                    decimal adiantamentoPagar = row["Adiantamento Pagar"] != DBNull.Value ? Convert.ToDecimal(row["Adiantamento Pagar"]) : 0;
                    decimal totalCategorias = 0;

                    foreach (var categoria in _categoriasTipoCols)
                    {
                        string nomeColuna = "Tipo_" + categoria["idCategoria"] + "_" + categoria["sDscCategoria"];
                        if (tbGeral.Columns.Contains(nomeColuna) && row[nomeColuna] != DBNull.Value)
                        {
                            totalCategorias += Convert.ToDecimal(row[nomeColuna]);
                        }
                    }

                    decimal totalSaidas = adiantamentoPagar + totalCategorias;
                    saidas.Add(totalSaidas);

                    decimal saldoAtual = row["nSaldoAtual"] != DBNull.Value ? Convert.ToDecimal(row["nSaldoAtual"]) : 0;
                    saldo.Add(saldoAtual);
                }

                string sContasSelecionadas = string.Join(", ", lstContaBancaria.Items.Cast<ListItem>()
                    .Where(item => item.Selected)
                    .Select(item => item.Text));
                string sTituloGrafico = $"Fluxo de Caixa - {dtInicio:dd/MM/yyyy} a {dtFinal:dd/MM/yyyy}";
                if (!string.IsNullOrEmpty(sContasSelecionadas))
                {
                    sTituloGrafico += $" - {sContasSelecionadas}";
                }
                else
                {
                    sTituloGrafico += " - Todas as Contas";
                }

                string jsonLabels = JsonConvert.SerializeObject(labels);
                string jsonEntradas = JsonConvert.SerializeObject(entradas);
                string jsonSaidas = JsonConvert.SerializeObject(saidas);
                string jsonSaldo = JsonConvert.SerializeObject(saldo);
                string jsonTitulo = JsonConvert.SerializeObject(sTituloGrafico);

                string script = $@"
                    var dadosGrafico = {{
                        labels: {jsonLabels},
                        entradas: {jsonEntradas},
                        saidas: {jsonSaidas},
                        saldo: {jsonSaldo},
                        titulo: {jsonTitulo}
                    }};
                    atualizarGraficoFluxoCaixa(dadosGrafico);
                ";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "AtualizarGraficoFluxoCaixa", script, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao preparar dados do gráfico: " + ex.Message);
            }
        }
    }
}
