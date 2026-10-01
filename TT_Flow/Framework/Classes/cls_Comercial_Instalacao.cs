using System;
using System.Collections.Generic;
using System.Linq;

namespace TT_Flow.FrameWork
{
    #region | Entradas

    /// <summary>
    /// Um produto como ele aparece no orçamento. O motor não conhece a tela: quem chama
    /// monta esta lista a partir de listProdutos / listProdutos_Composicao.
    /// </summary>
    [Serializable]
    public class cls_InstalacaoProdutoOrcamento
    {
        /// <summary>Zero quando o produto não está cadastrado (acontece em orçamento importado).</summary>
        public int IdProduto { get; set; }

        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Unidade { get; set; }
        public decimal Quantidade { get; set; }

        /// <summary>O tipo do produto é Sistema.</summary>
        public bool EhSistema { get; set; }

        /// <summary>É componente de um Sistema (o item tem idProdutoPai no orçamento).</summary>
        public bool EhComponenteDeSistema { get; set; }
    }

    /// <summary>
    /// O que o cadastro diz sobre um produto: sua categoria, o HH padrão dessa categoria
    /// e a configuração própria de instalação, se houver.
    /// </summary>
    [Serializable]
    public class cls_InstalacaoCadastroProduto
    {
        public int IdProduto { get; set; }

        /// <summary>Zero quando o produto está sem Categoria de Vendas.</summary>
        public int IdCategoriaVendas { get; set; }

        public string CodigoCategoria { get; set; }
        public string DescricaoCategoria { get; set; }

        /// <summary>Nulo = categoria sem padrão configurado. Diferente de zero.</summary>
        public decimal? PadraoHHInstalacao { get; set; }

        public List<cls_ProdutoInstalacaoItem> Configuracao { get; set; }

        public cls_InstalacaoCadastroProduto()
        {
            Configuracao = new List<cls_ProdutoInstalacaoItem>();
        }
    }

    #endregion

    #region | Saídas

    /// <summary>Uma linha do detalhamento (o "+" da grid do orçamento).</summary>
    [Serializable]
    public class cls_OrcamentoInstalacaoItem
    {
        public int IdCategoriaVendas { get; set; }
        public int IdProduto { get; set; }
        public string CodigoProduto { get; set; }
        public string DescricaoProduto { get; set; }
        public string Unidade { get; set; }
        public decimal QuantidadeTotal { get; set; }

        /// <summary>Zero quando a origem é o padrão da categoria.</summary>
        public int IdServico { get; set; }

        public string DescricaoServico { get; set; }
        public decimal HH { get; set; }
        public decimal TotalHH { get; set; }

        /// <summary>"C" = configuração do produto, "P" = padrão da categoria (linha amarela).</summary>
        public string OrigemHH { get; set; }

        public bool UsouPadrao
        {
            get { return OrigemHH == cls_Comercial_Instalacao.OrigemPadrao; }
        }
    }

    /// <summary>Uma linha do resumo por Categoria de Vendas.</summary>
    [Serializable]
    public class cls_OrcamentoInstalacaoCategoria
    {
        public int IdCategoriaVendas { get; set; }
        public string CodigoCategoria { get; set; }
        public string DescricaoCategoria { get; set; }

        /// <summary>Calculado.</summary>
        public decimal QtdHH { get; set; }

        /// <summary>Editável. Multiplicador, não percentual: 1,10 significa ×1,10.</summary>
        public decimal FatorCorrecao { get; set; }

        /// <summary>Calculado.</summary>
        public decimal QtdHoras { get; set; }

        /// <summary>Editável.</summary>
        public decimal ValorUnitario { get; set; }

        /// <summary>Editável. Percentual.</summary>
        public decimal Desconto { get; set; }

        /// <summary>Calculado.</summary>
        public decimal ValorTotal { get; set; }

        public int Ordem { get; set; }

        public cls_OrcamentoInstalacaoCategoria()
        {
            FatorCorrecao = 1m;
        }
    }

    /// <summary>Resultado completo de um cálculo.</summary>
    [Serializable]
    public class cls_InstalacaoResultado
    {
        public List<cls_OrcamentoInstalacaoCategoria> Categorias { get; set; }
        public List<cls_OrcamentoInstalacaoItem> Itens { get; set; }

        /// <summary>
        /// Pendências que impedem o cálculo. Vazia = resultado confiável.
        /// Nunca se devolve zero em silêncio no lugar de um dado que falta.
        /// </summary>
        public List<string> Inconsistencias { get; set; }

        public cls_InstalacaoResultado()
        {
            Categorias = new List<cls_OrcamentoInstalacaoCategoria>();
            Itens = new List<cls_OrcamentoInstalacaoItem>();
            Inconsistencias = new List<string>();
        }

        public bool Calculou
        {
            get { return Inconsistencias.Count == 0; }
        }

        public decimal TotalQtdHH { get { return Categorias.Sum(c => c.QtdHH); } }
        public decimal TotalQtdHoras { get { return Categorias.Sum(c => c.QtdHoras); } }
        public decimal TotalValor { get { return Categorias.Sum(c => c.ValorTotal); } }
    }

    #endregion

    /// <summary>
    /// Como tratar produto do tipo Sistema, que no orçamento existe como item pai MAIS os
    /// seus componentes. Contar os dois duplicaria o HH.
    ///
    /// PENDENTE DE DEFINIÇÃO DO NEGÓCIO. Não há default de propósito: quem chama precisa
    /// escolher explicitamente, para a decisão não passar despercebida.
    /// </summary>
    public enum eTratamentoSistema
    {
        /// <summary>Usa os componentes e ignora o item pai.</summary>
        SomenteComponentes = 1,

        /// <summary>Usa o item pai e ignora os componentes.</summary>
        SomenteSistema = 2
    }

    /// <summary>
    /// Motor de cálculo da instalação. Sem dependência de tela, banco ou ViewState:
    /// entra lista, sai resultado. É o que permite testá-lo isoladamente.
    /// </summary>
    public static class cls_Comercial_Instalacao
    {
        public const string OrigemConfiguracao = "C";
        public const string OrigemPadrao = "P";

        /// <summary>
        /// Calcula a instalação lendo os cadastros. É o caminho do primeiro cálculo e do
        /// botão Recalcular.
        /// </summary>
        /// <param name="produtos">Produtos do orçamento, como estão na tela.</param>
        /// <param name="cadastro">O que o cadastro diz sobre cada produto.</param>
        /// <param name="tratamentoSistema">Ver <see cref="eTratamentoSistema"/>.</param>
        /// <param name="valoresAnteriores">
        /// Resumo já existente. Fator, Valor Unitário e Desconto são digitados pelo usuário
        /// e por isso são preservados por categoria. Categoria nova entra com 1 / 0 / 0.
        /// </param>
        public static cls_InstalacaoResultado CalcularDosCadastros(
            List<cls_InstalacaoProdutoOrcamento> produtos,
            List<cls_InstalacaoCadastroProduto> cadastro,
            eTratamentoSistema tratamentoSistema,
            List<cls_OrcamentoInstalacaoCategoria> valoresAnteriores = null)
        {
            cls_InstalacaoResultado resultado = new cls_InstalacaoResultado();

            List<cls_InstalacaoProdutoOrcamento> considerados =
                FiltrarPorTratamentoSistema(produtos ?? new List<cls_InstalacaoProdutoOrcamento>(), tratamentoSistema);

            if (!considerados.Any())
                return resultado;

            // 1. Produto repetido no orçamento vira uma linha só, com as quantidades somadas.
            List<cls_InstalacaoProdutoOrcamento> consolidados = ConsolidarProdutos(considerados, resultado);

            if (!resultado.Calculou)
                return resultado;

            Dictionary<int, cls_InstalacaoCadastroProduto> porId =
                (cadastro ?? new List<cls_InstalacaoCadastroProduto>())
                    .GroupBy(c => c.IdProduto)
                    .ToDictionary(g => g.Key, g => g.First());

            // 2. Resolve o HH de cada produto e gera as linhas de detalhe.
            foreach (cls_InstalacaoProdutoOrcamento produto in consolidados)
            {
                cls_InstalacaoCadastroProduto dados;

                if (!porId.TryGetValue(produto.IdProduto, out dados))
                {
                    resultado.Inconsistencias.Add(string.Format(
                        "Não foi possível ler o cadastro do produto {0} - {1}.",
                        produto.Codigo, produto.Descricao));
                    continue;
                }

                if (dados.IdCategoriaVendas <= 0)
                {
                    resultado.Inconsistencias.Add(string.Format(
                        "O produto {0} - {1} não tem Categoria de Vendas.",
                        produto.Codigo, produto.Descricao));
                    continue;
                }

                resultado.Itens.AddRange(ResolverHH(produto, dados, resultado));
            }

            if (!resultado.Calculou)
                return resultado;

            // 3. Agrupa por idCategoriaVendas - nunca por código, que pode repetir.
            resultado.Categorias = AgruparPorCategoria(resultado.Itens, porId, valoresAnteriores);

            AtualizarTotais(resultado);

            return resultado;
        }

        /// <summary>
        /// Recalcula apenas as fórmulas, usando o HH que já está no resultado. É o caminho
        /// de quando o orçamento muda (quantidade, produto incluído ou removido) ou quando
        /// o usuário edita Fator, Valor Unitário ou Desconto: NÃO volta ao cadastro.
        /// </summary>
        public static void AtualizarTotais(cls_InstalacaoResultado resultado)
        {
            if (resultado == null)
                return;

            foreach (cls_OrcamentoInstalacaoCategoria categoria in resultado.Categorias)
            {
                categoria.QtdHH = Arredondar(resultado.Itens
                                                .Where(i => i.IdCategoriaVendas == categoria.IdCategoriaVendas)
                                                .Sum(i => i.TotalHH));

                categoria.QtdHoras = Arredondar(categoria.QtdHH * categoria.FatorCorrecao);

                decimal nBruto = Arredondar(categoria.QtdHoras * categoria.ValorUnitario);

                categoria.ValorTotal = Arredondar(nBruto - Arredondar(nBruto * (categoria.Desconto / 100m)));
            }
        }

        /// <summary>
        /// Reaplica sobre o snapshot as quantidades atuais do orçamento, usando o HH que
        /// já está nele. NÃO volta ao cadastro - é o caminho de "mudou a quantidade".
        ///
        /// Devolve true quando o CONJUNTO de produtos mudou: entrou produto novo ou saiu um
        /// que estava no snapshot. Produto novo não tem HH aqui e não há como inventar um,
        /// então só o Recalcular resolve - quem chama avisa o usuário.
        /// </summary>
        public static bool SincronizarQuantidades(
            cls_InstalacaoResultado resultado,
            List<cls_InstalacaoProdutoOrcamento> produtos,
            eTratamentoSistema tratamentoSistema)
        {
            if (resultado == null || !resultado.Itens.Any())
                return false;

            // Mesmo recorte do cálculo. Sem passar pelo ponto único de decisão do Sistema,
            // a quantidade do item pai entraria somada à dos componentes.
            Dictionary<int, decimal> quantidades =
                FiltrarPorTratamentoSistema(produtos ?? new List<cls_InstalacaoProdutoOrcamento>(), tratamentoSistema)
                    .Where(p => p.IdProduto > 0 && p.Quantidade > decimal.Zero)
                    .GroupBy(p => p.IdProduto)
                    .ToDictionary(g => g.Key, g => g.Sum(p => p.Quantidade));

            // Guarda contra apagar o snapshot inteiro: orçamento sem nenhum produto e com
            // snapshot preenchido é sinal de lista não carregada, não de instalação zerada.
            if (!quantidades.Any())
                return false;

            List<cls_OrcamentoInstalacaoItem> saidos = new List<cls_OrcamentoInstalacaoItem>();

            foreach (cls_OrcamentoInstalacaoItem item in resultado.Itens)
            {
                decimal nQuantidade;

                if (!quantidades.TryGetValue(item.IdProduto, out nQuantidade))
                {
                    // Produto saiu do orçamento: sai também da instalação.
                    saidos.Add(item);
                    continue;
                }

                item.QuantidadeTotal = nQuantidade;
                item.TotalHH         = Arredondar(nQuantidade * item.HH);
            }

            foreach (cls_OrcamentoInstalacaoItem item in saidos)
                resultado.Itens.Remove(item);

            // Todo produto de um snapshot válido tem ao menos uma linha - configuração
            // própria ou padrão da categoria. Estar no orçamento e não ter linha nenhuma
            // significa produto novo, e para esse não existe HH a reaproveitar.
            bool bNovos = quantidades.Keys.Any(id => !resultado.Itens.Any(i => i.IdProduto == id));

            // Categoria que perdeu todos os itens fica com QtdHH zero em vez de sumir: é o
            // que preserva o Fator, o Valor Unitário e o Desconto já digitados nela.
            AtualizarTotais(resultado);

            return bNovos || saidos.Any();
        }

        /// <summary>
        /// Resolve quais itens de instalação um produto gera.
        ///
        /// Regra tudo-ou-nada: se o produto tem configuração própria, usa exclusivamente
        /// ela, uma linha por serviço. Se não tem, usa o HH padrão da categoria, numa linha
        /// só, marcada como origem "P". Não existe completar parcialmente.
        /// </summary>
        public static List<cls_OrcamentoInstalacaoItem> ResolverHH(
            cls_InstalacaoProdutoOrcamento produto,
            cls_InstalacaoCadastroProduto dados,
            cls_InstalacaoResultado resultado)
        {
            List<cls_OrcamentoInstalacaoItem> itens = new List<cls_OrcamentoInstalacaoItem>();

            List<cls_ProdutoInstalacaoItem> configuracao =
                (dados.Configuracao ?? new List<cls_ProdutoInstalacaoItem>())
                    .OrderBy(c => c.Ordem)
                    .ToList();

            if (configuracao.Any())
            {
                foreach (cls_ProdutoInstalacaoItem config in configuracao)
                {
                    if (config.HH <= decimal.Zero)
                    {
                        resultado.Inconsistencias.Add(string.Format(
                            "O item {0} da instalação do produto {1} - {2} está com HH inválido.",
                            config.Descricao, produto.Codigo, produto.Descricao));
                        continue;
                    }

                    itens.Add(MontarItem(produto, dados, config.IdItemInstalacao, config.Descricao,
                                         config.HH, OrigemConfiguracao));
                }

                return itens;
            }

            // Sem configuração própria: cai no padrão da categoria.
            if (!dados.PadraoHHInstalacao.HasValue)
            {
                resultado.Inconsistencias.Add(string.Format(
                    "A categoria {0} não tem Padrão de HH de Instalação, exigido pelo produto {1} - {2}.",
                    string.IsNullOrWhiteSpace(dados.DescricaoCategoria) ? dados.IdCategoriaVendas.ToString() : dados.DescricaoCategoria,
                    produto.Codigo, produto.Descricao));

                return itens;
            }

            itens.Add(MontarItem(produto, dados, 0, "Padrão da Categoria",
                                 dados.PadraoHHInstalacao.Value, OrigemPadrao));

            return itens;
        }

        /// <summary>
        /// Junta as linhas do mesmo produto e soma as quantidades. Consolida por idProduto,
        /// nunca por código: o código pode mudar e não é chave.
        /// </summary>
        public static List<cls_InstalacaoProdutoOrcamento> ConsolidarProdutos(
            List<cls_InstalacaoProdutoOrcamento> produtos,
            cls_InstalacaoResultado resultado)
        {
            List<cls_InstalacaoProdutoOrcamento> naoCadastrados =
                produtos.Where(p => p.IdProduto <= 0).ToList();

            foreach (cls_InstalacaoProdutoOrcamento produto in naoCadastrados)
            {
                resultado.Inconsistencias.Add(string.Format(
                    "O produto {0} - {1} não está cadastrado e por isso não tem Categoria de Vendas.",
                    produto.Codigo, produto.Descricao));
            }

            foreach (cls_InstalacaoProdutoOrcamento produto in produtos.Where(p => p.IdProduto > 0 && p.Quantidade <= decimal.Zero))
            {
                resultado.Inconsistencias.Add(string.Format(
                    "O produto {0} - {1} está com quantidade inválida.",
                    produto.Codigo, produto.Descricao));
            }

            return produtos
                    .Where(p => p.IdProduto > 0 && p.Quantidade > decimal.Zero)
                    .GroupBy(p => p.IdProduto)
                    .Select(g => new cls_InstalacaoProdutoOrcamento
                    {
                        IdProduto   = g.Key,
                        Codigo      = g.First().Codigo,
                        Descricao   = g.First().Descricao,
                        Unidade     = g.First().Unidade,
                        Quantidade  = g.Sum(p => p.Quantidade),
                        EhSistema   = g.First().EhSistema,
                        EhComponenteDeSistema = g.First().EhComponenteDeSistema
                    })
                    .ToList();
        }

        /// <summary>
        /// Ponto ÚNICO de decisão sobre Produto Sistema. Enquanto a regra não estiver
        /// definida pelo negócio, é aqui - e só aqui - que ela muda.
        /// </summary>
        public static List<cls_InstalacaoProdutoOrcamento> FiltrarPorTratamentoSistema(
            List<cls_InstalacaoProdutoOrcamento> produtos,
            eTratamentoSistema tratamento)
        {
            if (tratamento == eTratamentoSistema.SomenteComponentes)
            {
                // Fora o item pai; os componentes entram como produtos comuns.
                return produtos.Where(p => !p.EhSistema).ToList();
            }

            // SomenteSistema: fora os componentes; o pai entra e responde pelo conjunto.
            return produtos.Where(p => !p.EhComponenteDeSistema).ToList();
        }

        #region | Internos

        static cls_OrcamentoInstalacaoItem MontarItem(
            cls_InstalacaoProdutoOrcamento produto,
            cls_InstalacaoCadastroProduto dados,
            int idServico,
            string descricaoServico,
            decimal nHH,
            string sOrigem)
        {
            return new cls_OrcamentoInstalacaoItem
            {
                IdCategoriaVendas = dados.IdCategoriaVendas,
                IdProduto         = produto.IdProduto,
                CodigoProduto     = produto.Codigo,
                DescricaoProduto  = produto.Descricao,
                Unidade           = produto.Unidade,
                QuantidadeTotal   = produto.Quantidade,
                IdServico         = idServico,
                DescricaoServico  = descricaoServico,
                HH                = nHH,
                TotalHH           = Arredondar(produto.Quantidade * nHH),
                OrigemHH          = sOrigem
            };
        }

        static List<cls_OrcamentoInstalacaoCategoria> AgruparPorCategoria(
            List<cls_OrcamentoInstalacaoItem> itens,
            Dictionary<int, cls_InstalacaoCadastroProduto> cadastro,
            List<cls_OrcamentoInstalacaoCategoria> valoresAnteriores)
        {
            List<cls_OrcamentoInstalacaoCategoria> categorias = new List<cls_OrcamentoInstalacaoCategoria>();
            int nOrdem = 1;

            foreach (var grupo in itens.GroupBy(i => i.IdCategoriaVendas))
            {
                cls_InstalacaoCadastroProduto referencia = cadastro.Values
                                                            .FirstOrDefault(c => c.IdCategoriaVendas == grupo.Key);

                cls_OrcamentoInstalacaoCategoria categoria = new cls_OrcamentoInstalacaoCategoria
                {
                    IdCategoriaVendas  = grupo.Key,
                    CodigoCategoria    = referencia != null ? referencia.CodigoCategoria : "",
                    DescricaoCategoria = referencia != null ? referencia.DescricaoCategoria : "",
                    Ordem              = nOrdem++
                };

                // Fator, Valor Unitário e Desconto são digitados: preserva o que já existia.
                cls_OrcamentoInstalacaoCategoria anterior = valoresAnteriores == null
                                                          ? null
                                                          : valoresAnteriores.FirstOrDefault(v => v.IdCategoriaVendas == grupo.Key);

                if (anterior != null)
                {
                    categoria.FatorCorrecao = anterior.FatorCorrecao;
                    categoria.ValorUnitario = anterior.ValorUnitario;
                    categoria.Desconto      = anterior.Desconto;
                }

                categorias.Add(categoria);
            }

            return categorias.OrderBy(c => c.DescricaoCategoria).ToList();
        }

        /// <summary>
        /// Duas casas, mesma convenção do resto do orçamento (AtualizaClasses usa
        /// Math.Round(x, 2) simples). Mantido igual de propósito: divergir aqui produziria
        /// diferença de centavos entre a aba Instalação e as demais.
        /// </summary>
        static decimal Arredondar(decimal nValor)
        {
            return Math.Round(nValor, 2);
        }

        #endregion
    }
}
