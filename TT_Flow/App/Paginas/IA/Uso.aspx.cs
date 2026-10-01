using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TT_Flow.FrameWork.IA;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.IA
{
    public partial class Uso : Page
    {
        private readonly cls_IA_Repositorio _repositorio = new cls_IA_Repositorio();

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.IA.VisualizarUso, true);

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = "IA - Uso";
                CarregarPainel();
                CarregarPrecos();
            }

            FUNCOES.Scripts.Aplica_TooltipPersonalizado(Page, "tooltip");
        }

        protected void cmdAtualizar_Click(object sender, EventArgs e)
        {
            CarregarPainel();
        }

        protected void cmdSalvarPrecos_Click(object sender, EventArgs e)
        {
            if (!ValidarPermissaoPrecos())
            {
                return;
            }

            decimal precoEntrada;
            decimal precoSaida;

            if (!TentarLerPreco(txtPrecoEntrada.Text, out precoEntrada) || !TentarLerPreco(txtPrecoSaida.Text, out precoSaida))
            {
                MensagemPagina.MostraMensagem_Erro("Informe preços válidos em US$: número maior ou igual a zero, ex.: 2.50 ou 2,50.");
                return;
            }

            _repositorio.SalvarConfig("IA.Custo.PrecoEntradaPor1M", PrecoParaConfig(precoEntrada));
            _repositorio.SalvarConfig("IA.Custo.PrecoSaidaPor1M", PrecoParaConfig(precoSaida));

            cls_IA_Auditoria.Registrar(0, "CONFIG_UPDATED", "INFO", new
            {
                origem = "Uso.aspx",
                precoPadraoEntradaPor1M = precoEntrada,
                precoPadraoSaidaPor1M = precoSaida
            });

            MensagemPagina.MostraMensagem_Sucesso("Preço padrão salvo. As estimativas já usam os novos valores.");
            CarregarPainel();
            CarregarPrecos();
        }

        protected void cmdSalvarPrecoModelo_Click(object sender, EventArgs e)
        {
            if (!ValidarPermissaoPrecos())
            {
                return;
            }

            string modelo = (ddlModeloPreco.SelectedValue ?? string.Empty).Trim();
            decimal precoEntrada;
            decimal precoSaida;

            if (modelo.Length == 0)
            {
                MensagemPagina.MostraMensagem_Erro("Selecione o modelo na lista.");
                return;
            }

            if (!TentarLerPreco(txtPrecoModeloEntrada.Text, out precoEntrada) || !TentarLerPreco(txtPrecoModeloSaida.Text, out precoSaida))
            {
                MensagemPagina.MostraMensagem_Erro("Informe preços válidos em US$: número maior ou igual a zero, ex.: 2.50 ou 2,50.");
                return;
            }

            cls_IA_Config config = cls_IA_Config.Carregar();
            Dictionary<string, IAPrecoModelo> precos = cls_IA_Custo.ObterPrecosModelos(config);
            precos[modelo] = new IAPrecoModelo { EntradaPor1M = precoEntrada, SaidaPor1M = precoSaida, Definido = true };

            _repositorio.SalvarConfig("IA.Custo.PrecosModelosJson", cls_IA_Custo.SerializarPrecos(precos));

            cls_IA_Auditoria.Registrar(0, "CONFIG_UPDATED", "INFO", new
            {
                origem = "Uso.aspx",
                modelo = modelo,
                precoEntradaPor1M = precoEntrada,
                precoSaidaPor1M = precoSaida
            });

            if (ddlModeloPreco.Items.Count > 0)
            {
                ddlModeloPreco.SelectedIndex = 0;
            }

            txtPrecoModeloEntrada.Text = string.Empty;
            txtPrecoModeloSaida.Text = string.Empty;

            MensagemPagina.MostraMensagem_Sucesso("Preço do modelo " + modelo + " salvo.");
            CarregarPainel();
            CarregarPrecos();
        }

        protected void dtgvPrecosModelos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "RemoverPreco" || !ValidarPermissaoPrecos())
            {
                return;
            }

            // O botao manda o proprio modelo (antes era ButtonField, que so passa o indice da linha).
            string modelo = (e.CommandArgument ?? string.Empty).ToString().Trim();
            if (modelo.Length == 0)
            {
                return;
            }

            cls_IA_Config config = cls_IA_Config.Carregar();
            Dictionary<string, IAPrecoModelo> precos = cls_IA_Custo.ObterPrecosModelos(config);

            if (precos.Remove(modelo))
            {
                _repositorio.SalvarConfig("IA.Custo.PrecosModelosJson", cls_IA_Custo.SerializarPrecos(precos));
                cls_IA_Auditoria.Registrar(0, "CONFIG_UPDATED", "INFO", new { origem = "Uso.aspx", modeloRemovido = modelo });
                MensagemPagina.MostraMensagem_Sucesso("Preço do modelo " + modelo + " removido.");
            }

            CarregarPainel();
            CarregarPrecos();
        }

        private bool ValidarPermissaoPrecos()
        {
            if (FUNCOES.ValidaPermissao(Permissao.IA.AdministrarFerramentas, false))
            {
                return true;
            }

            MensagemPagina.MostraMensagem_Erro("Você não possui permissão para alterar os preços.");
            return false;
        }

        private void CarregarPrecos()
        {
            cls_IA_Config config = cls_IA_Config.Carregar();

            CultureInfo culturaPreco = CultureInfo.GetCultureInfo("pt-BR");
            txtPrecoEntrada.Text = config.CustoPrecoEntradaPor1M.ToString("0.00", culturaPreco);
            txtPrecoSaida.Text = config.CustoPrecoSaidaPor1M.ToString("0.00", culturaPreco);
            pnPrecos.Visible = FUNCOES.ValidaPermissao(Permissao.IA.AdministrarFerramentas, false);

            Dictionary<string, IAPrecoModelo> precos = cls_IA_Custo.ObterPrecosModelos(config);
            DataTable tb = new DataTable();
            tb.Columns.Add("Modelo", typeof(string));
            tb.Columns.Add("Entrada", typeof(string));
            tb.Columns.Add("Saida", typeof(string));

            foreach (KeyValuePair<string, IAPrecoModelo> item in precos)
            {
                tb.Rows.Add(
                    item.Key,
                    item.Value.EntradaPor1M.ToString("N2"),
                    item.Value.SaidaPor1M.ToString("N2"));
            }

            tb.DefaultView.Sort = "Modelo ASC";
            dtgvPrecosModelos.DataSource = tb.DefaultView;
            dtgvPrecosModelos.DataBind();

            lblPrecosVazio.Visible = tb.Rows.Count == 0;
        }

        private void CarregarPainel()
        {
            try
            {
                cls_IA_Config config = cls_IA_Config.Carregar();
                Dictionary<string, IAPrecoModelo> precos = cls_IA_Custo.ObterPrecosModelos(config);
                DataSet ds = _repositorio.ConsultarUsoPeriodo(LerDias());

                if (ds == null || ds.Tables.Count < 3)
                {
                    LimparPainel();
                    return;
                }

                long perguntas = 0;
                long respostas = 0;

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    perguntas += ValorLong(row, "nPerguntas");
                    respostas += ValorLong(row, "nRespostas");
                }

                long conversas = 0;
                long tokensEntrada = 0;
                long tokensSaida = 0;

                if (ds.Tables[2].Rows.Count > 0)
                {
                    DataRow totais = ds.Tables[2].Rows[0];
                    conversas = ValorLong(totais, "nTotalConversas");
                    tokensEntrada = ValorLong(totais, "nTotalTokensEntrada");
                    tokensSaida = ValorLong(totais, "nTotalTokensSaida");
                }

                lblCardConversas.Text = conversas.ToString("N0");
                lblCardPerguntas.Text = perguntas.ToString("N0");
                lblCardRespostas.Text = respostas.ToString("N0");
                lblCardTokensEntrada.Text = tokensEntrada.ToString("N0");
                lblCardTokensSaida.Text = tokensSaida.ToString("N0");

                // Custo total exato: soma modelo a modelo pelo preco de cada um
                if (cls_IA_Custo.AlgumPrecoConfigurado(config) && ds.Tables.Count > 3)
                {
                    decimal custoTotal = 0;
                    foreach (DataRow row in ds.Tables[3].Rows)
                    {
                        IAPrecoModelo preco = cls_IA_Custo.ObterPrecoModelo(config, precos, Valor(row, "sModelo"));
                        custoTotal += cls_IA_Custo.Calcular(ValorLong(row, "nTokensEntrada"), ValorLong(row, "nTokensSaida"), preco);
                    }

                    lblCardCusto.Text = cls_IA_Custo.FormatarUSD(custoTotal);
                }
                else
                {
                    // Card do Dashboard usa fonte grande: frase longa nao cabe. A explicacao fica no painel de precos.
                    lblCardCusto.Text = "-";
                }

                ltrResumoPerguntas.Text = MontarResumo(ds.Tables[0], "nPerguntas");
                ltrResumoTokens.Text = MontarResumo(ds.Tables[0], "nTokensSaida");

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "GraficosUso",
                        MontarGraficoMorris(ds.Tables[0], "nPerguntas", "GraficoPerguntas", "Perguntas") +
                        MontarGraficoMorris(ds.Tables[0], "nTokensSaida", "GraficoTokens", "Tokens de saída"), true);
                }

                dtgvUsoUsuarios.DataSource = MontarTabelaUsuarios(ds.Tables[1], config, precos);
                dtgvUsoUsuarios.DataBind();

                dtgvUsoModelos.DataSource = ds.Tables.Count > 3 ? MontarTabelaModelos(ds.Tables[3], config, precos) : null;
                dtgvUsoModelos.DataBind();

                PopularModelosPreco(ds, precos);
            }
            catch (Exception ex)
            {
                LimparPainel();
                MensagemPagina.MostraMensagem_Erro("Erro ao carregar o painel de uso: " + ex.Message);
            }
        }

        // Select com todos os modelos cadastrados: catalogo curado do sistema (cls_IA_Config)
        // + modelos realmente usados no periodo + modelos que ja possuem preco
        private void PopularModelosPreco(DataSet ds, Dictionary<string, IAPrecoModelo> precos)
        {
            string selecionado = ddlModeloPreco.SelectedValue;
            List<string> modelos = cls_IA_Config.ModelosCatalogo();

            if (ds.Tables.Count > 3)
            {
                foreach (DataRow row in ds.Tables[3].Rows)
                {
                    string modelo = Valor(row, "sModelo");
                    if (modelo.Length > 0 && modelo != "(sem modelo)" && !ContemModelo(modelos, modelo))
                    {
                        modelos.Add(modelo);
                    }
                }
            }

            foreach (string modelo in precos.Keys)
            {
                if (!ContemModelo(modelos, modelo))
                {
                    modelos.Add(modelo);
                }
            }

            modelos.Sort(StringComparer.OrdinalIgnoreCase);

            ddlModeloPreco.Items.Clear();
            ddlModeloPreco.Items.Add(new ListItem("(selecione o modelo)", string.Empty));

            foreach (string modelo in modelos)
            {
                ddlModeloPreco.Items.Add(new ListItem(modelo, modelo));
            }

            if (!string.IsNullOrEmpty(selecionado) && ddlModeloPreco.Items.FindByValue(selecionado) != null)
            {
                ddlModeloPreco.SelectedValue = selecionado;
            }
        }

        private static bool ContemModelo(List<string> modelos, string modelo)
        {
            foreach (string existente in modelos)
            {
                if (string.Equals(existente, modelo, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private void LimparPainel()
        {
            lblCardConversas.Text = "-";
            lblCardPerguntas.Text = "-";
            lblCardRespostas.Text = "-";
            lblCardTokensEntrada.Text = "-";
            lblCardTokensSaida.Text = "-";
            lblCardCusto.Text = "-";
            ltrResumoPerguntas.Text = "<span class=\"help-block\">Sem dados no período.</span>";
            ltrResumoTokens.Text = "<span class=\"help-block\">Sem dados no período.</span>";
        }

        private int LerDias()
        {
            int dias;
            if (!int.TryParse(ddlPeriodo.SelectedValue, out dias))
            {
                return 30;
            }

            return dias == 7 || dias == 30 || dias == 90 ? dias : 30;
        }

        // Morris.Bar, o mesmo grafico dos outros dashboards (Dashboard, Dashboard_Adm, Dashboard_RRHH),
        // ja carregado no master. Os dados vao serializados por Newtonsoft de proposito: os dashboards
        // concatenam string na mao, o que quebra o JS se o rotulo tiver aspas.
        private static string MontarGraficoMorris(DataTable tb, string coluna, string elemento, string rotulo)
        {
            JArray dados = new JArray();

            // As linhas vem em ordem decrescente de data; o grafico vai da mais antiga para a mais recente.
            for (int i = tb.Rows.Count - 1; i >= 0; i--)
            {
                dados.Add(new JObject
                {
                    { "dia", EncurtarData(Valor(tb.Rows[i], "dtDia")) },
                    { "valor", ValorLong(tb.Rows[i], coluna) }
                });
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("$(function() {");
            sb.Append("$('#").Append(elemento).Append("').empty();"); // postback redesenha: sem isso empilha outro SVG
            sb.Append("Morris.Bar({");
            sb.Append("element: '").Append(elemento).Append("',");
            sb.Append("data: ").Append(dados.ToString(Formatting.None)).Append(",");
            sb.Append("xkey: 'dia',");
            sb.Append("ykeys: ['valor'],");
            sb.Append("labels: ['").Append(rotulo).Append("'],");
            sb.Append("resize: true,");
            sb.Append("barRatio: 0.5,");
            sb.Append("xLabelAngle: 45,");
            sb.Append("hideHover: 'auto'");
            sb.Append("});");
            sb.Append("});");
            return sb.ToString();
        }

        // O Morris mostra valor no hover, mas nao da agregado: total, media e pico ficam ao lado do titulo.
        private static string MontarResumo(DataTable tb, string coluna)
        {
            if (tb == null || tb.Rows.Count == 0)
            {
                return "<span class=\"help-block\">Sem dados no período.</span>";
            }

            long total = 0;
            long maximo = 0;
            string diaPico = string.Empty;

            foreach (DataRow row in tb.Rows)
            {
                long valor = ValorLong(row, coluna);
                total += valor;

                if (valor > maximo)
                {
                    maximo = valor;
                    diaPico = Valor(row, "dtDia");
                }
            }

            double media = total / (double)tb.Rows.Count;

            StringBuilder sb = new StringBuilder();
            sb.Append("<span class=\"help-block\">Total: <strong>").Append(total.ToString("N0")).Append("</strong>");
            sb.Append(" &middot; Média/dia: <strong>").Append(media.ToString("N1")).Append("</strong>");

            if (maximo > 0)
            {
                sb.Append(" &middot; Pico: <strong>").Append(maximo.ToString("N0"))
                  .Append("</strong> em ").Append(HttpUtility.HtmlEncode(EncurtarData(diaPico)));
            }

            sb.Append("</span>");
            return sb.ToString();
        }

        private static string EncurtarData(string dtDia)
        {
            dtDia = (dtDia ?? string.Empty).Trim();
            return dtDia.Length >= 5 ? dtDia.Substring(0, 5) : dtDia;
        }

        private sealed class AcumuladoUsuario
        {
            public string Nome;
            public long Conversas;
            public long TokensEntrada;
            public long TokensSaida;
            public decimal Custo;
        }

        // A origem vem por usuario x modelo; consolida por usuario aplicando o preco de cada modelo
        private static DataTable MontarTabelaUsuarios(DataTable origem, cls_IA_Config config, Dictionary<string, IAPrecoModelo> precos)
        {
            Dictionary<int, AcumuladoUsuario> porUsuario = new Dictionary<int, AcumuladoUsuario>();
            bool temPreco = cls_IA_Custo.AlgumPrecoConfigurado(config);

            if (origem != null)
            {
                foreach (DataRow row in origem.Rows)
                {
                    int idUsuario = (int)ValorLong(row, "idUsuario");
                    AcumuladoUsuario acumulado;

                    if (!porUsuario.TryGetValue(idUsuario, out acumulado))
                    {
                        acumulado = new AcumuladoUsuario { Nome = Valor(row, "sDscUsuario") };
                        porUsuario[idUsuario] = acumulado;
                    }

                    long entrada = ValorLong(row, "nTokensEntrada");
                    long saida = ValorLong(row, "nTokensSaida");

                    acumulado.Conversas += ValorLong(row, "nConversas");
                    acumulado.TokensEntrada += entrada;
                    acumulado.TokensSaida += saida;

                    if (temPreco)
                    {
                        IAPrecoModelo preco = cls_IA_Custo.ObterPrecoModelo(config, precos, Valor(row, "sModelo"));
                        acumulado.Custo += cls_IA_Custo.Calcular(entrada, saida, preco);
                    }
                }
            }

            List<AcumuladoUsuario> lista = new List<AcumuladoUsuario>(porUsuario.Values);
            lista.Sort(delegate (AcumuladoUsuario a, AcumuladoUsuario b)
            {
                int comparacao = b.Custo.CompareTo(a.Custo);
                return comparacao != 0 ? comparacao : b.TokensSaida.CompareTo(a.TokensSaida);
            });

            DataTable tb = new DataTable();
            tb.Columns.Add("Usuário", typeof(string));
            tb.Columns.Add("Conversas", typeof(long));
            tb.Columns.Add("Tokens entrada", typeof(string));
            tb.Columns.Add("Tokens saída", typeof(string));
            tb.Columns.Add("Custo (US$)", typeof(string));

            int limite = Math.Min(10, lista.Count);
            for (int i = 0; i < limite; i++)
            {
                AcumuladoUsuario item = lista[i];
                tb.Rows.Add(
                    item.Nome,
                    item.Conversas,
                    item.TokensEntrada.ToString("N0"),
                    item.TokensSaida.ToString("N0"),
                    temPreco ? item.Custo.ToString("N4") : "-");
            }

            return tb;
        }

        private static DataTable MontarTabelaModelos(DataTable origem, cls_IA_Config config, Dictionary<string, IAPrecoModelo> precos)
        {
            bool temPreco = cls_IA_Custo.AlgumPrecoConfigurado(config);

            DataTable tb = new DataTable();
            tb.Columns.Add("Modelo", typeof(string));
            tb.Columns.Add("Conversas", typeof(long));
            tb.Columns.Add("Tokens entrada", typeof(string));
            tb.Columns.Add("Tokens saída", typeof(string));
            tb.Columns.Add("Custo (US$)", typeof(string));

            if (origem != null)
            {
                foreach (DataRow row in origem.Rows)
                {
                    string modelo = Valor(row, "sModelo");
                    long entrada = ValorLong(row, "nTokensEntrada");
                    long saida = ValorLong(row, "nTokensSaida");

                    string custo = "-";
                    if (temPreco)
                    {
                        IAPrecoModelo preco = cls_IA_Custo.ObterPrecoModelo(config, precos, modelo);
                        custo = preco.Definido ? cls_IA_Custo.Calcular(entrada, saida, preco).ToString("N4") : "sem preço";
                    }

                    tb.Rows.Add(
                        modelo,
                        ValorLong(row, "nConversas"),
                        entrada.ToString("N0"),
                        saida.ToString("N0"),
                        custo);
                }
            }

            return tb;
        }

        private static bool TentarLerPreco(string texto, out decimal preco)
        {
            texto = (texto ?? string.Empty).Trim().Replace(",", ".");
            return decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out preco) && preco >= 0 && preco <= 100000;
        }

        private static string PrecoParaConfig(decimal preco)
        {
            // Precos padronizados em duas casas decimais, persistidos em invariant culture
            return preco.ToString("0.00", CultureInfo.InvariantCulture);
        }

        private static string Valor(DataRow row, string coluna)
        {
            if (row == null || !row.Table.Columns.Contains(coluna) || row[coluna] == DBNull.Value)
            {
                return string.Empty;
            }

            return row[coluna].ToString().Trim();
        }

        private static long ValorLong(DataRow row, string coluna)
        {
            long retorno;
            return long.TryParse(Valor(row, coluna), out retorno) ? retorno : 0;
        }
    }
}
