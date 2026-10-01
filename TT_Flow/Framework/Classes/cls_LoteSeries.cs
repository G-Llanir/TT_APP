using NPOI.SS.Formula.Functions;
using System;

namespace TT_Flow.FrameWork
{
    [Serializable]
    public class cls_LoteSeries : cls_WMS_MovimentacaoRel
    {
        #region | Construtor
        public cls_LoteSeries() { }

        #endregion

        #region | Propriedades
        //public int IdRegistro { get; set; }
        public string SdscProduto { get; set; }
        public string SExclusao { get; set; }
        public int IdEtiquetaSerie { get; set; }
        public int IdEtiqueta { get; set; }
        public int IdLocal { get; set; }
        public int IdObjeto { get; set; }
        public string STipoObjeto { get; set; }
        public int IdOPI { get; set; }

        #endregion
    }
}
