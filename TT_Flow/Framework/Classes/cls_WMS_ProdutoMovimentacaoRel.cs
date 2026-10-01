using System.Web.UI;
using System;

namespace TT_Flow.FrameWork
{
    [Serializable]
    public class cls_WMS_ProdutoMovimentacaoRel
    {
        #region | Construtor
        public cls_WMS_ProdutoMovimentacaoRel() { }
        #endregion

        #region | Membros Privados
        private int _idItem;
        private int _nEstoqueAtual;
        private string _sCodigo;
        private string _sDscProduto;
        private string _sUnidade;
        private string _sDscFamilia;
        private string _sLocal;
        private int _nCompras;
        private int _nVendas;
        private int _nEstoqueAnterior;
        private string _sImagem;
        private DateTime _dtMovimentacao;
        #endregion

        #region | Propriedades
        public int IdItem { get => _idItem; set => _idItem = value; }
        public int NEstoqueAtual { get => _nEstoqueAtual; set => _nEstoqueAtual = value; }
        public string SCodigo { get => _sCodigo; set => _sCodigo = value; }
        public string SDscProduto { get => _sDscProduto; set => _sDscProduto = value; }
        public string SUnidade { get => _sUnidade; set => _sUnidade = value; }
        public string SDscFamilia { get => _sDscFamilia; set => _sDscFamilia = value; }
        public string SLocal { get => _sLocal; set => _sLocal = value; }
        public int NCompras { get => _nCompras; set => _nCompras = value; }
        public int NVendas { get => _nVendas; set => _nVendas = value; }
        public int NEstoqueAnterior { get => _nEstoqueAnterior; set => _nEstoqueAnterior = value; }
        public DateTime DtMovimentacao { get => _dtMovimentacao; set => _dtMovimentacao = value; }
        public byte[] SImagem { get; set; }
        #endregion
    }

}