using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using TT.FrameWork;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.FrameWork
{
    /// <summary>
    /// Acesso a dados da instalação do orçamento. Faz a ponte entre
    /// <see cref="cls_Comercial_Instalacao"/> (que não conhece banco) e
    /// sp_Manipula_tbl_Flow_Pedidos_Instalacao.
    ///
    /// A tela não monta payload nem lê DataSet: chama estes métodos.
    /// </summary>
    public static class cls_OrcamentoInstalacao
    {
        public const string sProcedure = "sp_Manipula_tbl_Flow_Pedidos_Instalacao";

        static readonly CultureInfo culturaInvariante = CultureInfo.InvariantCulture;

        #region | Leitura

        /// <summary>
        /// Lê o snapshot gravado. NÃO consulta cadastro: é o que garante que reabrir um
        /// orçamento antigo mostre exatamente o que foi calculado na época.
        /// </summary>
        public static cls_InstalacaoResultado ConsultarSnapshot(int idPedido)
        {
            cls_InstalacaoResultado resultado = new cls_InstalacaoResultado();

            if (idPedido <= 0)
                return resultado;

            DataSet ds = BD.ExecutarDataSet(sProcedure, new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@idPedido", idPedido.ToString() }
            });

            if (ds == null || ds.Tables.Count == 0)
                return resultado;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                resultado.Categorias.Add(new cls_OrcamentoInstalacaoCategoria
                {
                    IdCategoriaVendas  = Inteiro(row["idCategoriaVendas"]),
                    CodigoCategoria    = Texto(row["sCodigoCategoria"]),
                    DescricaoCategoria = Texto(row["sDscCategoria"]),
                    QtdHH              = Decimal_(row["nQtdHH"]),
                    FatorCorrecao      = Decimal_(row["nFatorCorrecao"]),
                    QtdHoras           = Decimal_(row["nQtdHoras"]),
                    ValorUnitario      = Decimal_(row["nValorUnitario"]),
                    Desconto           = Decimal_(row["nDesconto"]),
                    ValorTotal         = Decimal_(row["nValorTotal"]),
                    Ordem              = Inteiro(row["nOrdem"])
                });
            }

            if (ds.Tables.Count > 1)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    resultado.Itens.Add(new cls_OrcamentoInstalacaoItem
                    {
                        IdCategoriaVendas = Inteiro(row["idCategoriaVendas"]),
                        IdProduto         = Inteiro(row["idProduto"]),
                        CodigoProduto     = Texto(row["sCodigoProduto"]),
                        DescricaoProduto  = Texto(row["sDscProduto"]),
                        Unidade           = Texto(row["sUnidade"]),
                        QuantidadeTotal   = Decimal_(row["nQuantidadeTotal"]),
                        IdServico         = Inteiro(row["idServico"]),
                        DescricaoServico  = Texto(row["sDscServico"]),
                        HH                = Decimal_(row["nHH"]),
                        TotalHH           = Decimal_(row["nTotalHH"]),
                        OrigemHH          = Texto(row["sOrigemHH"])
                    });
                }
            }

            return resultado;
        }

        /// <summary>
        /// Busca no cadastro o que o motor precisa: categoria, padrão de HH e a
        /// configuração de instalação de cada produto. É o caminho do primeiro cálculo
        /// e do botão Recalcular - o único momento em que o cadastro é lido.
        /// </summary>
        public static List<cls_InstalacaoCadastroProduto> ConsultarBaseCalculo(List<int> idProdutos)
        {
            List<cls_InstalacaoCadastroProduto> lista = new List<cls_InstalacaoCadastroProduto>();

            if (idProdutos == null || !idProdutos.Any(i => i > 0))
                return lista;

            string sIds = "|" + string.Join("|", idProdutos.Where(i => i > 0).Distinct()) + "|";

            DataSet ds = BD.ExecutarDataSet(sProcedure, new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_BASE_CALCULO" },
                { "@sidProdutos", sIds }
            });

            if (ds == null || ds.Tables.Count == 0)
                return lista;

            Dictionary<int, cls_InstalacaoCadastroProduto> porId = new Dictionary<int, cls_InstalacaoCadastroProduto>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                cls_InstalacaoCadastroProduto dados = new cls_InstalacaoCadastroProduto
                {
                    IdProduto          = Inteiro(row["idProduto"]),
                    IdCategoriaVendas  = Inteiro(row["idCategoriaVendas"]),
                    CodigoCategoria    = Texto(row["sCodigoCategoria"]),
                    DescricaoCategoria = Texto(row["sDscCategoria"]),
                    // NULL aqui significa "categoria sem padrão configurado", e o motor
                    // trata como pendência. Converter para zero apagaria essa distinção.
                    PadraoHHInstalacao = DecimalOuNulo(row["nPadraoHHInstalacao"])
                };

                if (!porId.ContainsKey(dados.IdProduto))
                {
                    porId.Add(dados.IdProduto, dados);
                    lista.Add(dados);
                }
            }

            if (ds.Tables.Count > 1)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    int idItem = Inteiro(row["idItem"]);

                    cls_InstalacaoCadastroProduto dados;

                    if (!porId.TryGetValue(idItem, out dados))
                        continue;

                    dados.Configuracao.Add(new cls_ProdutoInstalacaoItem
                    {
                        IdProduto        = idItem,
                        IdItemInstalacao = Inteiro(row["idItemInstalacao"]),
                        Codigo           = Texto(row["sCodigo"]),
                        Descricao        = Texto(row["sDscProduto"]),
                        Unidade          = Texto(row["sUnidade"]),
                        HH               = Decimal_(row["nHH"]),
                        Ordem            = Inteiro(row["nOrdem"])
                    });
                }
            }

            return lista;
        }

        #endregion

        #region | Gravação

        /// <summary>
        /// Substitui o snapshot inteiro do orçamento. A procedure faz DELETE + INSERT
        /// numa transação; deve ser chamada dentro do TransactionScope do orçamento.
        /// </summary>
        /// <returns>Mensagem de erro, ou string vazia em caso de sucesso.</returns>
        public static string Substituir(int idPedido, cls_InstalacaoResultado resultado, int idUsuario)
        {
            if (idPedido <= 0)
                return "Orçamento inválido para gravar a instalação.";

            DataSet ds = BD.ExecutarDataSet(sProcedure, new Dictionary<string, string>
            {
                { "@sFuncao", "SUBSTITUIR" },
                { "@idPedido", idPedido.ToString() },
                { "@sResumo", MontarPayloadResumo(resultado) },
                { "@sItens", MontarPayloadItens(resultado) },
                { "@idUsuarioAtualizacao", idUsuario.ToString() }
            });

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return "Não foi possível gravar a instalação do orçamento.";

            if (RETORNO.DATASET(ds, 0, "nRet") != "0")
            {
                string sMsg = RETORNO.DATASET(ds, 0, "msg");
                return string.IsNullOrWhiteSpace(sMsg) ? "Não foi possível gravar a instalação do orçamento." : sMsg;
            }

            return "";
        }

        /// <summary>
        /// idCategoria;nQtdHH;nFator;nQtdHoras;nValorUnitario;nDesconto;nValorTotal;nOrdem|...
        /// Só números: nenhum texto livre, para o delimitador nunca colidir com conteúdo.
        /// </summary>
        public static string MontarPayloadResumo(cls_InstalacaoResultado resultado)
        {
            if (resultado == null || !resultado.Categorias.Any())
                return "";

            StringBuilder sb = new StringBuilder();

            foreach (cls_OrcamentoInstalacaoCategoria c in resultado.Categorias)
            {
                if (sb.Length > 0) sb.Append("|");

                sb.Append(string.Join(";", new string[]
                {
                    c.IdCategoriaVendas.ToString(culturaInvariante),
                    Numero(c.QtdHH),
                    Numero(c.FatorCorrecao),
                    Numero(c.QtdHoras),
                    Numero(c.ValorUnitario),
                    Numero(c.Desconto),
                    Numero(c.ValorTotal),
                    c.Ordem.ToString(culturaInvariante)
                }));
            }

            return sb.ToString();
        }

        /// <summary>
        /// idCategoria;idProduto;nQuantidadeTotal;idServico;nHH;nTotalHH;sOrigemHH|...
        /// </summary>
        public static string MontarPayloadItens(cls_InstalacaoResultado resultado)
        {
            if (resultado == null || !resultado.Itens.Any())
                return "";

            StringBuilder sb = new StringBuilder();

            foreach (cls_OrcamentoInstalacaoItem i in resultado.Itens)
            {
                if (sb.Length > 0) sb.Append("|");

                sb.Append(string.Join(";", new string[]
                {
                    i.IdCategoriaVendas.ToString(culturaInvariante),
                    i.IdProduto.ToString(culturaInvariante),
                    Numero(i.QuantidadeTotal),
                    i.IdServico.ToString(culturaInvariante),
                    Numero(i.HH),
                    Numero(i.TotalHH),
                    i.OrigemHH == cls_Comercial_Instalacao.OrigemPadrao
                        ? cls_Comercial_Instalacao.OrigemPadrao
                        : cls_Comercial_Instalacao.OrigemConfiguracao
                }));
            }

            return sb.ToString();
        }

        #endregion

        #region | Conversões

        /// <summary>Sempre com ponto decimal: é o que a procedure espera.</summary>
        static string Numero(decimal nValor)
        {
            return nValor.ToString("0.####", culturaInvariante);
        }

        static int Inteiro(object valor)
        {
            if (valor == null || valor == DBNull.Value) return 0;

            int nValor;
            int.TryParse(Convert.ToString(valor, CultureInfo.CurrentCulture), out nValor);
            return nValor;
        }

        static string Texto(object valor)
        {
            return valor == null || valor == DBNull.Value ? "" : Convert.ToString(valor).Trim();
        }

        static decimal Decimal_(object valor)
        {
            decimal? nValor = DecimalOuNulo(valor);
            return nValor.HasValue ? nValor.Value : decimal.Zero;
        }

        static decimal? DecimalOuNulo(object valor)
        {
            if (valor == null || valor == DBNull.Value) return null;

            if (valor is decimal) return (decimal)valor;

            decimal nValor;

            if (decimal.TryParse(Convert.ToString(valor, CultureInfo.CurrentCulture),
                                 NumberStyles.Number, CultureInfo.CurrentCulture, out nValor))
                return nValor;

            if (decimal.TryParse(Convert.ToString(valor, CultureInfo.InvariantCulture),
                                 NumberStyles.Number, CultureInfo.InvariantCulture, out nValor))
                return nValor;

            return null;
        }

        #endregion
    }
}
