using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TT_Flow.FrameWork
{
    [Serializable]
    public class cls_EPI_Itens
    {

        #region | Construtor 
        public cls_EPI_Itens()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_EPI_Itens
        (
                int idColaborador
            , int idEPI
            , string sEPI
            , string sTamanhoEPI
            , string dtRecebimentoEPI
            , string dtVencimentoEPI
            , string sObservacaoEPI
            , string sQuantidadeEPI
            , string sCA

        )
        {

            _idColaborador = idColaborador;
            _idEPI = idEPI;
            _sEPI = sEPI;
            _sTamanhoEPI = sTamanhoEPI;
            _dtRecebimentoEPI = dtRecebimentoEPI;
            _dtVencimentoEPI = dtVencimentoEPI;
            _sObservacaoEPI = sObservacaoEPI;
            _sQuantidadeEPI = sQuantidadeEPI;
            _sCA = sCA;
        }

        #endregion

        #region | Membros Privados 

        private int _idColaborador;
        private int _idEPI;
        private string _sEPI;
        private string _sTamanhoEPI;
        private string _dtRecebimentoEPI;
        private string _dtVencimentoEPI;
        private string _sObservacaoEPI;
        private string _sQuantidadeEPI;
        private string _sCA;
        

        #endregion

        #region | Propriedades


        public int idColaborador
        {
            get { return _idColaborador; }
            set { _idColaborador = value; }
        }
        public int idEPI
        {
            get { return _idEPI; }
            set { _idEPI = value; }
        }

        public string sEPI
        {
            get { return _sEPI; }
            set { _sEPI = value; }
        }

        public string sTamanhoEPI
        {
            get { return _sTamanhoEPI; }
            set { _sTamanhoEPI = value; }
        }

        public string dtRecebimentoEPI
        {
            get { return _dtRecebimentoEPI; }
            set { _dtRecebimentoEPI = value; }
        }


        public string dtVencimentoEPI
        {
            get { return _dtVencimentoEPI; }
            set { _dtVencimentoEPI = value; }
        }


        public string sObservacaoEPI
        {
            get { return _sObservacaoEPI; }
            set { _sObservacaoEPI = value; }
        }

        public string sQuantidadeEPI
        {
            get { return _sQuantidadeEPI; }
            set { _sQuantidadeEPI = value; }
        }


        public string sCA
        {
            get { return _sCA; }
            set { _sCA = value; }
        }


        #endregion


    }
}