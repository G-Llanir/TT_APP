using System;


namespace TT_Colaborador.FrameWork
{
    [Serializable]
    public class cls_Despesas
    {
        #region | Construtor 
        public cls_Despesas()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_Despesas
        (
            int idItens
            , int idDespesas
            , int idContador
            , string dtDespesa
            , decimal nValor
            , int idFormaPagamento
            , int idCategoriaPagar
            , string sDscObservacao
            , string sFuncao
            , string sDscCategoriaPagar
            , string sDscFormaPagamento
            , string sLocal
            , string sNomeArquivo
            , string sObservacaoArquivo
            , int idArquivo
            , string sNomeArquivo2
            , string sObservacaoArquivo2
            , int idArquivo2
            , string sidArquivo
            , string sParticipantes
            , string sidParticipantes
        )
        {
            _idItens = idItens;
            _idDespesas = idDespesas;
            _dtDespesa = dtDespesa;
            _nValor = nValor;
            _idFormaPagamento = idFormaPagamento;
            _idCategoriaPagar = idCategoriaPagar;
            _sDscObservacao = sDscObservacao;
            _sFuncao = sFuncao;
            _sDscCategoriaPagar = sDscCategoriaPagar;
            _sDscFormaPagamento = sDscFormaPagamento;
            _sLocal = sLocal;
            _idContador = idContador;
            _sNomeArquivo = sNomeArquivo;
            _sObservacaoArquivo = sObservacaoArquivo;
            _idArquivo = idArquivo;
            _sNomeArquivo2 = sNomeArquivo2;
            _sObservacaoArquivo2 = sObservacaoArquivo2;
            _idArquivo2 = idArquivo2;
            _sidArquivo = sidArquivo;
            _sParticipantes = sParticipantes;
            _sidParticipantes = sidParticipantes;
        }

        #endregion

        #region | Membros Privados 

        private int _idItens;
        private int _idDespesas;
        private int _idContador;
        private string _dtDespesa;
        private decimal _nValor;
        private int _idFormaPagamento;
        private int _idCategoriaPagar;
        private string _sDscObservacao;
        private string _sFuncao;
        private string _sDscCategoriaPagar;
        private string _sDscFormaPagamento;
        private string _sLocal;
        private string _sNomeArquivo;
        private string _sObservacaoArquivo;
        private Byte[] _objArquivo;
        private int _idArquivo;
        private string _sNomeArquivo2;
        private string _sObservacaoArquivo2;
        private string _sidArquivo;
        private Byte[] _objArquivo2;
        private int _idArquivo2;
        private string _sParticipantes;
        private string _sidParticipantes;
        #endregion

        #region | Propriedades 

        public int idItens
        {
            get { return _idItens; }
            set { _idItens = value; }
        }
        public int idDespesas
        {
            get { return _idDespesas; }
            set { _idDespesas = value; }
        }
        public string dtDespesa
        {
            get { return _dtDespesa; }
            set { _dtDespesa = value; }
        }
        public decimal nValor
        {
            get { return _nValor; }
            set { _nValor = value; }
        }

        public int idFormaPagamento
        {
            get { return _idFormaPagamento; }
            set { _idFormaPagamento = value; }
        }

        public int idCategoriaPagar
        {
            get { return _idCategoriaPagar; }
            set { _idCategoriaPagar = value; }
        }

        public string sDscObservacao
        {
            get { return _sDscObservacao; }
            set { _sDscObservacao = value; }
        }

        public string sFuncao
        {
            get { return _sFuncao; }
            set { _sFuncao = value; }
        }

        public string sDscCategoriaPagar
        {
            get { return _sDscCategoriaPagar; }
            set { _sDscCategoriaPagar = value; }
        }

        public string sDscFormaPagamento
        {
            get { return _sDscFormaPagamento; }
            set { _sDscFormaPagamento = value; }
        }

        public string sLocal
        {
            get { return _sLocal; }
            set { _sLocal = value; }
        }

        public int idContador
        {
            get { return _idContador; }
            set { _idContador = value; }
        }

        public string sNomeArquivo
        {
            get { return _sNomeArquivo; }
            set { _sNomeArquivo = value; }
        }

        public string sObservacaoArquivo
        {
            get { return _sObservacaoArquivo; }
            set { _sObservacaoArquivo = value; }
        }

        public int idArquivo
        {
            get { return _idArquivo; }
            set { _idArquivo = value; }
        }

        public byte[] objArquivo 
        { 
            get => _objArquivo; 
            set => _objArquivo = value; 
        }

        public string sNomeArquivo2
        {
            get { return _sNomeArquivo2; }
            set { _sNomeArquivo2 = value; }
        }

        public string sObservacaoArquivo2
        {
            get { return _sObservacaoArquivo2; }
            set { _sObservacaoArquivo2 = value; }
        }

        public int idArquivo2
        {
            get { return _idArquivo2; }
            set { _idArquivo2 = value; }
        }

        public byte[] objArquivo2
        {
            get => _objArquivo2;
            set => _objArquivo2 = value;
        }

        public string sidArquivo
        {
            get { return _sidArquivo; }
            set { _sidArquivo = value; }
        }

        public string sParticipantes
        {
            get { return _sParticipantes; }
            set { _sParticipantes = value; }
        }

        public string sidParticipantes
        {
            get { return _sidParticipantes; }
            set { _sidParticipantes = value; }
        }

        public int idTipoDespesa { get; set; }

        public string sDscTipoDespesa { get; set; }
        #endregion
    }
}