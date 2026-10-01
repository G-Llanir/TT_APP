using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using TT.FrameWork;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.FrameWork
{
    /// <summary>
    /// Uma linha da aba Instalação/Obra do Produto: um Serviço, Sub-Serviço ou Recurso
    /// com as horas que ele consome por unidade do produto.
    ///
    /// DTO enxuto de propósito. Não reaproveita cls_WMS_Produtos nem cls_Comercial_Tabelas
    /// porque a lista vive em ViewState e essas classes carregam dezenas de propriedades
    /// (impostos, zonas de preço, pesos) que não têm uso aqui.
    /// </summary>
    [Serializable]
    public class cls_ProdutoInstalacaoItem
    {
        public int IdRegistro { get; set; }

        /// <summary>Produto dono da configuração.</summary>
        public int IdProduto { get; set; }

        /// <summary>Serviço, Sub-Serviço ou Recurso.</summary>
        public int IdItemInstalacao { get; set; }

        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Tipo { get; set; }
        public string Unidade { get; set; }

        /// <summary>Horas por unidade do produto. Sempre maior que zero.</summary>
        public decimal HH { get; set; }

        public int Ordem { get; set; }
    }

    /// <summary>
    /// Acesso à configuração de instalação do Produto. Fala apenas com
    /// sp_Manipula_tbl_Flow_Produtos_Instalacao.
    /// </summary>
    public static class cls_ProdutoInstalacao
    {
        public const string sProcedure = "sp_Manipula_tbl_Flow_Produtos_Instalacao";

        /// <summary>
        /// Lê a configuração gravada de um produto.
        /// </summary>
        public static List<cls_ProdutoInstalacaoItem> Consultar(int idProduto)
        {
            List<cls_ProdutoInstalacaoItem> lista = new List<cls_ProdutoInstalacaoItem>();

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@idItem", idProduto.ToString() }
            };

            DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

            if (ds == null || ds.Tables.Count == 0)
                return lista;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                lista.Add(new cls_ProdutoInstalacaoItem
                {
                    IdRegistro       = ConverterInt(row["idRegistro"]),
                    IdProduto        = ConverterInt(row["idItem"]),
                    IdItemInstalacao = ConverterInt(row["idItemInstalacao"]),
                    Codigo           = Convert.ToString(row["sCodigo"]),
                    Descricao        = Convert.ToString(row["sDscProduto"]),
                    Tipo             = Convert.ToString(row["sDscTipoProduto"]),
                    Unidade          = Convert.ToString(row["sUnidade"]),
                    HH               = ConverterDecimal(row["nHH"]),
                    Ordem            = ConverterInt(row["nOrdem"])
                });
            }

            return lista;
        }

        /// <summary>
        /// Substitui a configuração inteira do produto, de forma atômica (a procedure
        /// faz DELETE + INSERT dentro de uma transação).
        /// </summary>
        /// <returns>Mensagem de erro, ou string vazia em caso de sucesso.</returns>
        public static string Substituir(int idProduto, List<cls_ProdutoInstalacaoItem> itens, int idUsuario)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SUBSTITUIR_CONFIGURACAO" },
                { "@idItem", idProduto.ToString() },
                { "@sItens", MontarPayload(itens) },
                { "@idUsuarioAtualizacao", idUsuario.ToString() }
            };

            DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return "Não foi possível salvar a configuração de instalação.";

            if (RETORNO.DATASET(ds, 0, "nRet") != "0")
            {
                string sMsg = RETORNO.DATASET(ds, 0, "msg");
                return string.IsNullOrWhiteSpace(sMsg) ? "Não foi possível salvar a configuração de instalação." : sMsg;
            }

            return "";
        }

        /// <summary>
        /// Monta a lista no formato esperado pela procedure:
        /// idItemInstalacao;nHH;nOrdem|idItemInstalacao;nHH;nOrdem|...
        /// O HH vai sempre com ponto decimal (InvariantCulture).
        /// </summary>
        public static string MontarPayload(List<cls_ProdutoInstalacaoItem> itens)
        {
            if (itens == null || itens.Count == 0)
                return "";

            return string.Join("|", itens.Select(i => string.Format(CultureInfo.InvariantCulture,
                                                                    "{0};{1};{2}",
                                                                    i.IdItemInstalacao,
                                                                    i.HH.ToString("0.####", CultureInfo.InvariantCulture),
                                                                    i.Ordem)));
        }

        static int ConverterInt(object valor)
        {
            int.TryParse(Convert.ToString(valor), out int nValor);
            return nValor;
        }

        static decimal ConverterDecimal(object valor)
        {
            if (valor == null || valor == DBNull.Value)
                return decimal.Zero;

            if (valor is decimal) return (decimal)valor;

            decimal.TryParse(Convert.ToString(valor), NumberStyles.Number, CultureInfo.CurrentCulture, out decimal nValor);
            return nValor;
        }
    }
}
