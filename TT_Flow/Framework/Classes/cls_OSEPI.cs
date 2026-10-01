using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TT_Flow.FrameWork
{
    [Serializable]
    
    public class cls_OSEPI_Itens
    {

        #region | Construtor 
        public cls_OSEPI_Itens()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_OSEPI_Itens
        (
              int idOrdemServico
            , string idEPI
            , string sDscEPI
            , string nQuantidadeEPI
        )
        {
            _idOrdemServico = idOrdemServico;
            _idEPI = idEPI;
            _sDscEPI = sDscEPI;
            _nQuantidadeEPI = nQuantidadeEPI;
        }

        #endregion

        #region | Membros Privados 


        private int _idOrdemServico;
        private string _idEPI;
        private string _sDscEPI;
        private string _nQuantidadeEPI;

        #endregion

        #region | Propriedades

        public int idOrdemServico
        {
            get { return _idOrdemServico; }
            set { _idOrdemServico = value; }
        }

        public string idEPI
        {
            get { return _idEPI; }
            set { _idEPI = value; }
        }

        public string sDscEPI
        {
            get { return _sDscEPI; }
            set { _sDscEPI = value; }
        }


        public string nQuantidadeEPI
        {
            get { return _nQuantidadeEPI; }
            set { _nQuantidadeEPI = value; }
        }

        #endregion

    }
}