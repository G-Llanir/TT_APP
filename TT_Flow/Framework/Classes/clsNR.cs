using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TT_Flow.FrameWork
{
    [Serializable]
    public class cls_NR_Itens
    {

        #region | Construtor 
        public cls_NR_Itens()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_NR_Itens
        (
              int idColaborador
            , int idNR
            , string sTipoNR
            , string sDscNR
            , string dtEmissaoNR
            , string dtVencimentoNR

        )
        {
            _idColaborador = idColaborador;
            _idNR = idNR;
            _sTipoNR = sTipoNR;
            _sDscNR = sDscNR;
            _dtEmissaoNR = dtEmissaoNR;
            _dtVencimentoNR = dtVencimentoNR;

        }

        #endregion

        #region | Membros Privados 

        private int _idColaborador;
        private int _idNR;
        private string _sTipoNR;
        private string _sDscNR;
        private string _dtEmissaoNR;
        private string _dtVencimentoNR;


        #endregion

        #region | Propriedades


        public int idColaborador
        {
            get { return _idColaborador; }
            set { _idColaborador = value; }
        }
        public int idNR
        {
            get { return _idNR; }
            set { _idNR = value; }
        }

        public string sTipoNR
        {
            get { return _sTipoNR; }
            set { _sTipoNR = value; }
        }

        public string sDscNR
        {
            get { return _sDscNR; }
            set { _sDscNR = value; }
        }


        public string dtEmissaoNR
        {
            get { return _dtEmissaoNR; }
            set { _dtEmissaoNR = value; }
        }


        public string dtVencimentoNR
        {
            get { return _dtVencimentoNR; }
            set { _dtVencimentoNR = value; }
        }



        #endregion


    }
}