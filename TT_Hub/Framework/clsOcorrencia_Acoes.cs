using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TT_Hub.FrameWork
{
    [Serializable]
    public class clsOcorrencia_Acoes
    {

        #region | Construtor 
        public clsOcorrencia_Acoes()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public clsOcorrencia_Acoes
        (
              int idRegistroAcao
            , int idOcorrencia
            , int idAcao
            , string sDscAcao
            , string sObservacao
            , int idUsuarioAcao
            , string sDscUsuarioAcao
            , string dtEnvioEquipamento
        )
        {
            _idRegistroAcao = idRegistroAcao;
            _idOcorrencia = idOcorrencia;
            _idAcao = idAcao;
            _sDscAcao = sDscAcao;
            _sObservacao = sObservacao;
            _idUsuarioAcao = idUsuarioAcao;
            _sDscUsuarioAcao = sDscUsuarioAcao;
            _dtEnvioEquipamento = dtEnvioEquipamento;
        }

        #endregion

        #region | Membros Privados 

        private int _idRegistroAcao;
        private int _idOcorrencia;
        private int _idAcao;
        private string _sDscAcao;
        private string _sObservacao;
        private int _idUsuarioAcao;
        private string _sDscUsuarioAcao;
        private string _dtEnvioEquipamento;
        #endregion

        #region | Propriedades

        public int idRegistroAcao
        {
            get { return _idRegistroAcao; }
            set { _idRegistroAcao = value; }
        }

        public int idOcorrencia
        {
            get { return _idOcorrencia; }
            set { _idOcorrencia = value; }
        }

        public int idAcao
        {
            get { return _idAcao; }
            set { _idAcao = value; }
        }

        public string sDscAcao
        {
            get { return _sDscAcao; }
            set { _sDscAcao = value; }
        }
        public string sObservacao
        {
            get { return _sObservacao; }
            set { _sObservacao = value; }
        }
        public int idUsuarioAcao
        {
            get { return _idUsuarioAcao; }
            set { _idUsuarioAcao = value; }
        }
        public string sDscUsuarioAcao
        {
            get { return _sDscUsuarioAcao; }
            set { _sDscUsuarioAcao = value; }
        }
        public string dtEnvioEquipamento
        {
            get { return _dtEnvioEquipamento; }
            set { _dtEnvioEquipamento = value; }
        }


        #endregion

    }
}