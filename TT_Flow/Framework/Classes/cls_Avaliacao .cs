using System;


namespace TT_Flow.FrameWork
{
    [Serializable]
    
    public class cls_Avaliacao_Perguntas
    {

        #region | Construtor 
        public cls_Avaliacao_Perguntas()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public cls_Avaliacao_Perguntas
        (
              int idContador
            , int idPergunta
            , int idAvaliacao
            , int nOrdem
            , string sTipo
            , string sRespondidaPor
            , string sDscPergunta
            , string sGrupo
            , string sCaixadeObservacao
            , string sOpcoes
            , string sFuncao
        )
        {
            _idContador = idContador;
            _idPergunta = idPergunta;
            _idAvaliacao = idAvaliacao;
            _nOrdem = nOrdem;
            _sTipo = sTipo;
            _sRespondidaPor = sRespondidaPor;
            _sDscPergunta = sDscPergunta;
            _sGrupo = sGrupo;
            _sCaixadeObservacao = sCaixadeObservacao;
            _sOpcoes = sOpcoes;
            _sFuncao = sFuncao;
        }

        #endregion

        #region | Membros Privados 


        private int _idContador;
        private int _idPergunta;
        private int _idAvaliacao;
        private int _nOrdem;
        private string _sTipo;
        private string _sRespondidaPor;
        private string _sDscPergunta;
        private string _sGrupo;
        private string _sCaixadeObservacao;
        private string _sOpcoes;

        private string _sFuncao;

        #endregion

        #region | Propriedades
        public int idContador
        {
            get { return _idContador; }
            set { _idContador = value; }
        }
        public int idPergunta
        {
            get { return _idPergunta; }
            set { _idPergunta = value; }
        }
        public int idAvaliacao
        {
            get { return _idAvaliacao; }
            set { _idAvaliacao = value; }
        }
        public int nOrdem
        {
            get { return _nOrdem; }
            set { _nOrdem = value; }
        }
        public string sTipo
        {
            get { return _sTipo; }
            set { _sTipo = value; }
        }
        public string sRespondidaPor
        {
            get { return _sRespondidaPor; }
            set { _sRespondidaPor = value; }
        }
        public string sDscPergunta
        {
            get { return _sDscPergunta; }
            set { _sDscPergunta = value; }
        }
        public string sGrupo
        {
            get { return _sGrupo; }
            set { _sGrupo = value; }
        }
        public string sCaixadeObservacao
        {
            get { return _sCaixadeObservacao; }
            set { _sCaixadeObservacao = value; }
        }

        public string sOpcoes
        {
            get { return _sOpcoes; }
            set { _sOpcoes = value; }
        }

        public string sFuncao
        {
            get { return _sFuncao; }
            set { _sFuncao = value; }
        }

        #endregion

    }


}