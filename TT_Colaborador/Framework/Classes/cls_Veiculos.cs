using System;

namespace TT_Colaborador.FrameWork
{
    [Serializable]
    public class cls_Veiculos
    {
        public cls_Veiculos() { }

        #region | Membros Privados 

        private int _idRegistro;
        private int _idMovimentacao;
        private int _idCentroCusto;
        private int _idTipoViagem;
        private int _nKilometros;
        private int _idSituacao_Tanque;
        private int _idUsuario;

        private object _vbArquivo_Colaborador;
        private object _vbArquivo_Painel;
        private object _vbArquivo_Frente;
        private object _vbArquivo_Traseira;
        private object _vbArquivo_Lat_Direita;
        private object _vbArquivo_Lat_Esquerda;

        private decimal _nLatitude;
        private decimal _nLongitude;

        private string _sObservacao;
        private string _sDscTipoMovimentacao;
        private string _sDscUsuario;

        private DateTime _dtMovimentacao;

        private CheckList_Movimentacao _sPneusDianteiros;
        private CheckList_Movimentacao _sPneusTraseiros;
        private CheckList_Movimentacao _sRodasDiateiras;
        private CheckList_Movimentacao _sRodasTraseiras;
        private CheckList_Movimentacao _sBancos;
        private CheckList_Movimentacao _sPainel;
        private CheckList_Movimentacao _sConsoles;
        private CheckList_Movimentacao _sForro;
        private CheckList_Movimentacao _sTapetes;
        private CheckList_Movimentacao _sCalotas;
        private CheckList_Movimentacao _sRetrovisores;
        private CheckList_Movimentacao _sPalhetas;
        private CheckList_Movimentacao _sTriangulo;
        private CheckList_Movimentacao _sMacaco;
        private CheckList_Movimentacao _sEstepe;
        private CheckList_Movimentacao _sBateria;
        private CheckList_Movimentacao _sChaves;
        private CheckList_Movimentacao _sDocumentos;
        private CheckList_Movimentacao _sSom;
        private CheckList_Movimentacao _sCaixaSelada;

        #endregion

        #region | Propriedades 

        public int idRegistro { get { return _idRegistro; } set { _idRegistro = value; } }
        public int idMovimentacao { get { return _idMovimentacao; } set { _idMovimentacao = value; } }
        public int idCentroCusto { get { return _idCentroCusto; } set { _idCentroCusto = value; } }
        public int idTipoViagem { get { return _idTipoViagem; } set { _idTipoViagem = value; } }
        public int nKilometros { get { return _nKilometros; } set { _nKilometros = value; } }
        public int idSituacao_Tanque { get { return _idSituacao_Tanque; } set { _idSituacao_Tanque = value; } }
        public int idUsuario { get { return _idUsuario; } set { _idUsuario = value; } }

        public object vbArquivo_Colaborador { get { return _vbArquivo_Colaborador; } set { _vbArquivo_Colaborador = value; } }
        public object vbArquivo_Painel { get { return _vbArquivo_Painel; } set { _vbArquivo_Painel = value; } }
        public object vbArquivo_Frente { get { return _vbArquivo_Frente; } set { _vbArquivo_Frente = value; } }
        public object vbArquivo_Traseira { get { return _vbArquivo_Traseira; } set { _vbArquivo_Traseira = value; } }
        public object vbArquivo_Lat_Direita { get { return _vbArquivo_Lat_Direita; } set { _vbArquivo_Lat_Direita = value; } }
        public object vbArquivo_Lat_Esquerda { get { return _vbArquivo_Lat_Esquerda; } set { _vbArquivo_Lat_Esquerda = value; } }

        public decimal nLatitude { get { return _nLatitude; } set { _nLatitude = value; } }
        public decimal nLongitude { get { return _nLongitude; } set { _nLongitude = value; } }

        public string sObservacao { get { return _sObservacao; } set { _sObservacao = value; } }
        public string sDscTipoMovimentacao { get { return _sDscTipoMovimentacao; } set { _sDscTipoMovimentacao = value; } }
        public string sDscUsuario { get { return _sDscUsuario; } set { _sDscUsuario = value; } }

        public DateTime dtMovimentacao { get { return _dtMovimentacao; } set { _dtMovimentacao = value; } }

        public CheckList_Movimentacao sPneusDianteiros { get { return _sPneusDianteiros; } set { _sPneusDianteiros = value; } }
        public CheckList_Movimentacao sPneusTraseiros { get { return _sPneusTraseiros; } set { _sPneusTraseiros = value; } }
        public CheckList_Movimentacao sRodasDiateiras { get { return _sRodasDiateiras; } set { _sRodasDiateiras = value; } }
        public CheckList_Movimentacao sRodasTraseiras { get { return _sRodasTraseiras; } set { _sRodasTraseiras = value; } }
        public CheckList_Movimentacao sBancos { get { return _sBancos; } set { _sBancos = value; } }
        public CheckList_Movimentacao sPainel { get { return _sPainel; } set { _sPainel = value; } }
        public CheckList_Movimentacao sConsoles { get { return _sConsoles; } set { _sConsoles = value; } }
        public CheckList_Movimentacao sForro { get { return _sForro; } set { _sForro = value; } }
        public CheckList_Movimentacao sTapetes { get { return _sTapetes; } set { _sTapetes = value; } }
        public CheckList_Movimentacao sCalotas { get { return _sCalotas; } set { _sCalotas = value; } }
        public CheckList_Movimentacao sRetrovisores { get { return _sRetrovisores; } set { _sRetrovisores = value; } }
        public CheckList_Movimentacao sPalhetas { get { return _sPalhetas; } set { _sPalhetas = value; } }
        public CheckList_Movimentacao sTriangulo { get { return _sTriangulo; } set { _sTriangulo = value; } }
        public CheckList_Movimentacao sMacaco { get { return _sMacaco; } set { _sMacaco = value; } }
        public CheckList_Movimentacao sEstepe { get { return _sEstepe; } set { _sEstepe = value; } }
        public CheckList_Movimentacao sBateria { get { return _sBateria; } set { _sBateria = value; } }
        public CheckList_Movimentacao sChaves { get { return _sChaves; } set { _sChaves = value; } }
        public CheckList_Movimentacao sDocumentos { get { return _sDocumentos; } set { _sDocumentos = value; } }
        public CheckList_Movimentacao sSom { get { return _sSom; } set { _sSom = value; } }
        public CheckList_Movimentacao sCaixaSelada { get { return _sCaixaSelada; } set { _sCaixaSelada = value; } }

        #endregion

        public enum CheckList_Movimentacao
        {
            Novo = 'N',
            Bom = 'B',
            Ruim = 'R',
            NaoPossui = '0'
        }
    }

    [Serializable]
    public class cls_Tipos_Movimentacao
    {
        public int idTipo { get; set; }
        public string sDscTipo { get; set; }
        public bool bRepete { get; set; }
        public bool bChecklist_Completo { get; set; }
    }
}