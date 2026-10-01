using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static TT.FrameWork.BD;
using TT.FrameWork;

namespace TT_Flow.FrameWork
{

    #region | Importador
    [Serializable]
    public class cls_Importador_Itens
    {
        #region | Construtor 
        public cls_Importador_Itens()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_Importador_Itens
        (
              int idItens
            , int idImportador
            , int nItem
            , string cProd
            , string xProd
            , string cEAN
            , string NCM
            , string CFOP
            , string uCom
            , string qCom
            , string vUnCom
            , string vProd
            , string cEANTrib
            , string uTrib
            , string qTrib
            , string vUnTrib
            , string indTot
            , string xPed
            , int idProduto
            , int idPedido
            , string sCodigo
            //ICMS
            , string orig
            , string CST_ICMS
            , string modBC
            , string vBC_ICMS
            , string pRedBC
            , string pICMS
            , string vICMS
            //IPI
            , string cEnq
            , string CST_IPI
            , string vBC_IPI
            , string pIPI
            , string vIPI
            //II
            , string vBC_II
            , string vDespAdu
            , string vII
            , string vIOF
            //PIS
            , string CST_PIS
            , string vBC_PIS
            , string pPIS
            , string vPIS
            //COFINS
            , string CST_COFINS
            , string vBC_COFINS
            , string pCOFINS
            , string vCOFINS

            , string idProduto_Pesquisa
            , string sCodigo_Pesquisa
            , string sDscProduto_Pesquisa
            , string sPesquisa
            , string sReferencia

        )
        {
            _idItens = idItens;
            _idImportador = idImportador;
            _nItem = nItem;
            _cProd = cProd;
            _xProd = xProd;
            _cEAN = cEAN;
            _NCM = NCM;
            _CFOP = CFOP;
            _uCom = uCom;
            _qCom = qCom;
            _vUnCom = vUnCom;
            _vProd = vProd;
            _cEANTrib = cEANTrib;
            _uTrib = uTrib;
            _qTrib = qTrib;
            _vUnTrib = vUnTrib;
            _indTot = indTot;
            _xPed = xPed;
            _idProduto = idProduto;
            _idPedido = idPedido;
            _sCodigo = sCodigo;
            //ICMS
            _orig = orig;
            _CST_ICMS = CST_ICMS;
            _modBC = modBC;
            _vBC_ICMS = vBC_ICMS;
            _pRedBC = pRedBC;
            _pICMS = pICMS;
            _vICMS = vICMS;
            //IPI
            _cEnq = cEnq;
            _CST_IPI = CST_IPI;
            _vBC_IPI = vBC_IPI;
            _pIPI = pIPI;
            _vIPI = vIPI;
            //II
            _vBC_II = vBC_II;
            _vDespAdu = vDespAdu;
            _vII = vII;
            _vIOF = vIOF;
            //PIS
            _CST_PIS = CST_PIS;
            _vBC_PIS = vBC_PIS;
            _pPIS = pPIS;
            _vPIS = vPIS;
            //COFINS
            _CST_COFINS = CST_COFINS;
            _vBC_COFINS = vBC_COFINS;
            _pCOFINS = pCOFINS;
            _vCOFINS = vCOFINS;

            _idProduto_Pesquisa = idProduto_Pesquisa;
            _sCodigo_Pesquisa =  sCodigo_Pesquisa;
            _sDscProduto_Pesquisa = sDscProduto_Pesquisa;
            _sPesquisa = sPesquisa;
            _sReferencia = sReferencia;
        }
        #endregion

        #region | Membros Privados 
        private int _idItens;
        private int _idImportador;
        private int _nItem;
        private string _cProd;
        private string _xProd;
        private string _cEAN;
        private string _NCM;
        private string _CFOP;
        private string _uCom;
        private string _qCom;
        private string _vUnCom;
        private string _vProd;
        private string _cEANTrib;
        private string _uTrib;
        private string _qTrib;
        private string _vUnTrib;
        private string _indTot;
        private string _xPed;
        private int _idProduto;
        private int _idPedido;
        private string _sCodigo;
        //ICMS
        private string _orig;
        private string _CST_ICMS;
        private string _modBC;
        private string _vBC_ICMS;
        private string _pRedBC;
        private string _pICMS;
        private string _vICMS;
        //IPI
        private string _cEnq;
        private string _CST_IPI;
        private string _vBC_IPI;
        private string _pIPI;
        private string _vIPI;
        //II
        private string _vBC_II;
        private string _vDespAdu;
        private string _vII;
        private string _vIOF;
        //PIS
        private string _CST_PIS;
        private string _vBC_PIS;
        private string _pPIS;
        private string _vPIS;
        //COFINS
        private string _CST_COFINS;
        private string _vBC_COFINS;
        private string _pCOFINS;
        private string _vCOFINS;

        private string _idProduto_Pesquisa;
        private string _sCodigo_Pesquisa;
        private string _sDscProduto_Pesquisa;
        private string _sPesquisa;
        private string _sReferencia;

        #endregion

        #region | Propriedades
        public int idItens
        {
            get { return _idItens; }
            set { _idItens = value; }
        }
        public int idImportador
        {
            get { return _idImportador; }
            set { _idImportador = value; }
        }

        public int nItem
        {
            get { return _nItem; }
            set { _nItem = value; }
        }

        public string cProd
        {
            get { return _cProd; }
            set { _cProd = value; }
        }

        public string xProd
        {
            get { return _xProd; }
            set { _xProd = value; }
        }

        public string cEAN
        {
            get { return _cEAN; }
            set { _cEAN = value; }
        }

        public string NCM
        {
            get { return _NCM; }
            set { _NCM = value; }
        }

        public string CFOP
        {
            get { return _CFOP; }
            set { _CFOP = value; }
        }

        public string uCom
        {
            get { return _uCom; }
            set { _uCom = value; }
        }

        public string qCom
        {
            get { return _qCom; }
            set { _qCom = value; }
        }

        public string vUnCom
        {
            get { return _vUnCom; }
            set { _vUnCom = value; }
        }

        public string vProd
        {
            get { return _vProd; }
            set { _vProd = value; }
        }

        public string cEANTrib
        {
            get { return _cEANTrib; }
            set { _cEANTrib = value; }
        }

        public string uTrib
        {
            get { return _uTrib; }
            set { _uTrib = value; }
        }

        public string qTrib
        {
            get { return _qTrib; }
            set { _qTrib = value; }
        }

        public string vUnTrib
        {
            get { return _vUnTrib; }
            set { _vUnTrib = value; }
        }

        public string indTot
        {
            get { return _indTot; }
            set { _indTot = value; }
        }

        public string xPed
        {
            get { return _xPed; }
            set { _xPed = value; }
        }

        public int idProduto
        {
            get { return _idProduto; }
            set { _idProduto = value; }
        }

        public int idPedido
        {
            get { return _idPedido; }
            set { _idPedido = value; }
        }

        public string sCodigo
        {
            get { return _sCodigo; }
            set { _sCodigo = value; }
        }

        public string orig
        {
            get { return _orig; }
            set { _orig = value; }
        }

        public string CST_ICMS
        {
            get { return _CST_ICMS; }
            set { _CST_ICMS = value; }
        }

        public string modBC
        {
            get { return _modBC; }
            set { _modBC = value; }
        }

        public string vBC_ICMS
        {
            get { return _vBC_ICMS; }
            set { _vBC_ICMS = value; }
        }

        public string pRedBC
        {
            get { return _pRedBC; }
            set { _pRedBC = value; }
        }

        public string pICMS
        {
            get { return _pICMS; }
            set { _pICMS = value; }
        }

        public string vICMS
        {
            get { return _vICMS; }
            set { _vICMS = value; }
        }

        public string cEnq
        {
            get { return _cEnq; }
            set { _cEnq = value; }
        }

        public string CST_IPI
        {
            get { return _CST_IPI; }
            set { _CST_IPI = value; }
        }

        public string vBC_IPI
        {
            get { return _vBC_IPI; }
            set { _vBC_IPI = value; }
        }

        public string pIPI
        {
            get { return _pIPI; }
            set { _pIPI = value; }
        }

        public string vIPI
        {
            get { return _vIPI; }
            set { _vIPI = value; }
        }

        public string vBC_II
        {
            get { return _vBC_II; }
            set { _vBC_II = value; }
        }

        public string vDespAdu
        {
            get { return _vDespAdu; }
            set { _vDespAdu = value; }
        }

        public string vII
        {
            get { return _vII; }
            set { _vII = value; }
        }

        public string vIOF
        {
            get { return _vIOF; }
            set { _vIOF = value; }
        }

        public string CST_PIS
        {
            get { return _CST_PIS; }
            set { _CST_PIS = value; }
        }

        public string vBC_PIS
        {
            get { return _vBC_PIS; }
            set { _vBC_PIS = value; }
        }

        public string pPIS
        {
            get { return _pPIS; }
            set { _pPIS = value; }
        }

        public string vPIS
        {
            get { return _vPIS; }
            set { _vPIS = value; }
        }

        public string CST_COFINS
        {
            get { return _CST_COFINS; }
            set { _CST_COFINS = value; }
        }

        public string vBC_COFINS
        {
            get { return _vBC_COFINS; }
            set { _vBC_COFINS = value; }
        }

        public string pCOFINS
        {
            get { return _pCOFINS; }
            set { _pCOFINS = value; }
        }

        public string vCOFINS
        {
            get { return _vCOFINS; }
            set { _vCOFINS = value; }
        }

        public string idProduto_Pesquisa
        {
            get { return _idProduto_Pesquisa; }
            set { _idProduto_Pesquisa = value; }
        }

        public string sCodigo_Pesquisa
        {
            get { return _sCodigo_Pesquisa; }
            set { _sCodigo_Pesquisa = value; }
        }

        public string sDscProduto_Pesquisa
        {
            get { return _sDscProduto_Pesquisa; }
            set { _sDscProduto_Pesquisa = value; }
        }

        public string sPesquisa
        {
            get { return _sPesquisa; }
            set { _sPesquisa = value; }
        }

        public string sReferencia
        {
            get { return _sReferencia; }
            set { _sReferencia = value; }
        }

        #endregion
    }
    #endregion
}
