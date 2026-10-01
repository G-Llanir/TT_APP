using System;
using System.Collections.Generic;
using System.Linq;

namespace TT_Flow.FrameWork
{
    [Serializable]
    public class cls_Escopo
    {
        #region | Construtor

        public cls_Escopo() { }

        public cls_Escopo(
            int idEscopo
            , string sDscEscopo
            , string sidCategorias
            )
        { }

        #endregion

        #region | Membros Privados

        private int _idEscopo;
        private string _sDscEscopo;
        private string _sidCategorias;

        #endregion

        #region | Propriedades

        public int idEscopo { get => _idEscopo; set => _idEscopo = value; }
        public string sDscEscopo { get => _sDscEscopo; set => _sDscEscopo = value; }
        public string sidCategorias { get => _sidCategorias; set => _sidCategorias = value; }

        #endregion
    }

    [Serializable]
    public class cls_Categoria 
    {
        #region | Construtor

        public cls_Categoria() { }

        public cls_Categoria(
            int idCategoria
            , int idEscopo
            , string sDscCategoria
            , string sPerguntas
            , string sOpcoes
            )
        { }

        #endregion

        #region | Membros Privados

        private int _idCategoria;
        private int _idEscopo;
        private string _sDscCategoria;
        private string _sPerguntas;
        private string _sOpcoes;

        #endregion

        #region | Propriedades

        public int idCategoria { get => _idCategoria; set => _idCategoria = value; }
        public int idEscopo { get => _idEscopo; set => _idEscopo = value; }
        public string sDscCategoria { get => _sDscCategoria; set => _sDscCategoria = value; }
        public string sPerguntas { get => _sPerguntas; set => _sPerguntas = value; }
        public string sOpcoes { get => _sOpcoes; set => _sOpcoes = value; }

        #endregion
    }
}