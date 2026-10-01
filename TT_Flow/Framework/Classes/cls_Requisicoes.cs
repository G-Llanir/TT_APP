using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TT_Flow.FrameWork
{
    [Serializable]
    public class cls_Requisicao_Itens
    {

        #region | Construtor 
        public cls_Requisicao_Itens()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_Requisicao_Itens
        (
              int idRequisicao
            , string sCodigo
            , string sDscProduto
            , string sUnidade
            , string nQuantidade
            , string dtPrevisaoUso
            , int idProduto
        )
        {
            _idRequisicao = idRequisicao;
            _sCodigo = sCodigo;
            _sDscProduto = sDscProduto;
            _sUnidade = sUnidade;
            _nQuantidade = nQuantidade;
            _dtPrevisaoUso = dtPrevisaoUso;
            _idProduto = idProduto;
        }

        #endregion

        #region | Membros Privados 


        private int _idRequisicao;
        private string _sCodigo;
        private string _sDscProduto;
        private string _sUnidade;
        private string _nQuantidade;
        private string _dtPrevisaoUso;
        private int _idProduto;

        #endregion

        #region | Propriedades

        public int idRequisicao
        {
            get { return _idRequisicao; }
            set { _idRequisicao = value; }
        }

        public string sCodigo
        {
            get { return _sCodigo; }
            set { _sCodigo = value; }
        }


        public string sDscProduto
        {
            get { return _sDscProduto; }
            set { _sDscProduto = value; }
        }


        public string sUnidade
        {
            get { return _sUnidade; }
            set { _sUnidade = value; }
        }

        public string nQuantidade
        {
            get { return _nQuantidade; }
            set { _nQuantidade = value; }
        }
        public string dtPrevisaoUso
        {
            get { return _dtPrevisaoUso; }
            set { _dtPrevisaoUso = value; }
        }

        public int idProduto
        {
            get { return _idProduto; }
            set { _idProduto = value; }
        }

        #endregion

    }
}
