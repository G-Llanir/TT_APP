using System;

namespace TT_Flow.FrameWork
{
    #region | Classes

    [Serializable]
    public class cls_Atividades
    {
        #region | Membros Privados

        private int _idAtividade;
        private int _idAtividadePai;
        private int _idProjeto;
        private int _idStatus;
        private string _sDscTitulo;
        private string _sDscDescricao;
        private decimal? _nHoras_Previsao;
        private DateTime? _dtInicial;
        private DateTime? _dtInicial_Previsao;
        private DateTime? _dtFinal_Previsao;
        private DateTime? _dtFinal;
        private bool _bAtividadePai;

        #endregion

        #region | Propriedades

        public int idAtividade { get => _idAtividade; set => _idAtividade = value; }
        public int idAtividadePai { get => _idAtividadePai; set => _idAtividadePai = value; }
        public int idProjeto { get => _idProjeto; set => _idProjeto = value; }
        public int idStatus { get => _idStatus; set => _idStatus = value; }
        public string sDscTitulo { get => _sDscTitulo; set => _sDscTitulo = value; }
        public string sDscDescricao { get => _sDscDescricao; set => _sDscDescricao = value; }
        public decimal? nHoras_Previsao { get => _nHoras_Previsao; set => _nHoras_Previsao = value; }
        public DateTime? dtInicial { get => _dtInicial; set => _dtInicial = value; }
        public DateTime? dtInicial_Previsao { get => _dtInicial_Previsao; set => _dtInicial_Previsao = value; }
        public DateTime? dtFinal_Previsao { get => _dtFinal_Previsao; set => _dtFinal_Previsao = value; }
        public DateTime? dtFinal { get => _dtFinal; set => _dtFinal = value; }
        public bool bAtividadePai { get => _bAtividadePai; set => _bAtividadePai = value; }

        #endregion
    }

    [Serializable]
    public class cls_Apontamentos
    {
        #region | Membros Privados

        private int _idApontamento;
        private int _idAtividade;
        private int _idTipo;
        private int _idUsuario;
        private string _sObservacao;
        private decimal _nHoras;
        private DateTime _dtApontamento;
        private string _sDscUsuario;
        private bool _bSalva;

        #endregion

        #region | Propriedades

        public int idApontamento { get => _idApontamento; set => _idApontamento = value; }
        public int idAtividade { get => _idAtividade; set => _idAtividade = value; }
        public int idTipo { get => _idTipo; set => _idTipo = value; }
        public int idUsuario { get => _idUsuario; set => _idUsuario = value; }
        public string sObservacao { get => _sObservacao; set => _sObservacao = value; }
        public decimal nHoras { get => _nHoras; set => _nHoras = value; }
        public DateTime dtApontamento { get => _dtApontamento; set => _dtApontamento = value; }
        public string sDscUsuario { get => _sDscUsuario; set => _sDscUsuario = value; }
        public bool bSalva { get => _bSalva; set => _bSalva = value; }

        #endregion
    }

    [Serializable]
    public class cls_Apontamentos_Historico
    {
        #region | Membros Privados

        private int _idHistorico;
        private int _idApontamento;
        private string _sDscMotivo;
        private string _sDscAlteracao;
        private int _idUsuario;
        private string _sDscUsuario;
        private DateTime _dtAlteracao;

        #endregion

        #region | Propriedades

        public int idHistorico { get => _idHistorico; set => _idHistorico = value; }
        public int idApontamento { get => _idApontamento; set => _idApontamento = value; }
        public string sDscMotivo { get => _sDscMotivo; set => _sDscMotivo = value; }
        public string sDscAlteracao { get => _sDscAlteracao; set => _sDscAlteracao = value; }
        public int idUsuario { get => _idUsuario; set => _idUsuario = value; }
        public string sDscUsuario { get => _sDscUsuario; set => _sDscUsuario = value; }
        public DateTime dtAlteracao { get => _dtAlteracao; set => _dtAlteracao = value; }

        #endregion
    }

    #endregion
}