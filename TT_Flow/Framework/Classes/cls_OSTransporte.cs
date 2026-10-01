using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TT_Flow.FrameWork
{
    [Serializable]
    
    public class cls_OSTransporte_Itens
    {

        #region | Construtor 
        public cls_OSTransporte_Itens()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_OSTransporte_Itens
        (
              int idOrdemServico
            , string idTipoTransporte
            , string sDscTipoTransporte
            , string sMarca
            , string sModelo
            , string sPlaca
            , string sCor
            , string sFrota
            , string sItinerario
        )
        {
            _idOrdemServico = idOrdemServico;
            _idTipoTransporte = idTipoTransporte;
            _sDscTipoTransporte = sDscTipoTransporte;
            _sMarca = sMarca;
            _sModelo = sModelo;
            _sPlaca = sPlaca;
            _sCor = sCor;
            _sFrota = sFrota;
            _sItinerario = sItinerario;
        }

        #endregion

        #region | Membros Privados 


        private int _idOrdemServico;
        private string _idTipoTransporte;
        private string _sDscTipoTransporte;
        private string _sMarca;
        private string _sModelo;
        private string _sPlaca;
        private string _sCor;
        private string _sFrota; 
        private string _sItinerario;

        #endregion

        #region | Propriedades

        public int idOrdemServico
        {
            get { return _idOrdemServico; }
            set { _idOrdemServico = value; }
        }

        public string idTipoTransporte
        {
            get { return _idTipoTransporte; }
            set { _idTipoTransporte = value; }
        }

        public string sDscTipoTransporte
        {
            get { return _sDscTipoTransporte; }
            set { _sDscTipoTransporte = value; }
        }

        public string sMarca
        {
            get { return _sMarca; }
            set { _sMarca = value; }
        }


        public string sModelo
        {
            get { return _sModelo; }
            set { _sModelo = value; }
        }

        public string sPlaca
        {
            get { return _sPlaca; }
            set { _sPlaca = value; }
        }

        public string sCor
        {
            get { return _sCor; }
            set { _sCor = value; }
        }

        public string sFrota
        {
            get { return _sFrota; }
            set { _sFrota = value; }
        }

        public string sItinerario
        {
            get { return _sItinerario; }
            set { _sItinerario = value; }
        }

        #endregion

    }
}