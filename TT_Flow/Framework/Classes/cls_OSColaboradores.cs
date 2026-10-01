using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TT_Flow.FrameWork
{
    [Serializable]
    public class cls_OSColaboradores_Itens
    {

        #region | Construtor 
        public cls_OSColaboradores_Itens()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_OSColaboradores_Itens
        (
              int idOrdemServico
            , string sDscColaborador
            , int idColaborador
            , string dtPrevisaoInicio
            , string dtPrevisaoTermino
        )
        {
            _idOrdemServico = idOrdemServico;
            _idColaborador = idColaborador;
            _sDscColaborador = sDscColaborador;
            _dtPrevisaoInicio = dtPrevisaoInicio;
            _dtPrevisaoTermino = dtPrevisaoTermino;
        }

        #endregion

        #region | Membros Privados 


        private int _idOrdemServico;
        private int _idColaborador;
        private string _sDscColaborador;
        private string _dtPrevisaoInicio;
        private string _dtPrevisaoTermino;

        #endregion

        #region | Propriedades

        public int idOrdemServico
        {
            get { return _idOrdemServico; }
            set { _idOrdemServico = value; }
        }

        public int idColaborador
        {
            get { return _idColaborador; }
            set { _idColaborador = value; }
        }

        public string sDscColaborador
        {
            get { return _sDscColaborador; }
            set { _sDscColaborador = value; }
        }


        public string dtPrevisaoInicio
        {
            get { return _dtPrevisaoInicio; }
            set { _dtPrevisaoInicio = value; }
        }


        public string dtPrevisaoTermino
        {
            get { return _dtPrevisaoTermino; }
            set { _dtPrevisaoTermino = value; }
        }

        #endregion

    }

    
}