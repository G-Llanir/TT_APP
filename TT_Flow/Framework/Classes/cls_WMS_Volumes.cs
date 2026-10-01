using System.Web.UI;
using System;

namespace TT_Flow.FrameWork
{
    [Serializable]
    public class cls_WMS_Volumes
    {
        #region | Construtor
        public cls_WMS_Volumes() { }


        #endregion

        #region | Membros Privados
        private int _idVolume;
        private string _sDscVolume;

        #endregion

        #region | Propriedades
        public int IdVolume { get => _idVolume; set => _idVolume = value; }
        public string SDscVolume { get => _sDscVolume; set => _sDscVolume = value; }

        #endregion
    }

    [Serializable]
    public class cls_WMS_VolumesItens
    {
        #region | Construtor
        public cls_WMS_VolumesItens() { }


        #endregion

        #region | Membros Privados
        private int _idVolumeItem;
        private int _idObjeto;
        private string _sDscObjeto;
        private int _nVolume;
        private int _idOPI;
        private int _idUsuarioAtualizacao;
        private string _sCodigoBarras;
        //private string _sCodigoBarrasUnitizado;
       // private string _sCodigoBarrasProduto;
        #endregion

        #region | Propriedades
        public int IdVolumeItem { get => _idVolumeItem; set => _idVolumeItem = value; }
        public int IdObjeto { get => _idObjeto; set => _idObjeto = value; }
        public string SDscObjeto { get => _sDscObjeto; set => _sDscObjeto = value; }
        public int NVolume { get => _nVolume; set => _nVolume = value; }
        public int IdOPI { get => _idOPI; set => _idOPI = value; }
        public int IdUsuarioAtualizacao { get => _idUsuarioAtualizacao; set => _idUsuarioAtualizacao = value; }
        public string SCodigoBarras { get => _sCodigoBarras; set => _sCodigoBarras = value; }
        public string SCodigoProduto { get; set; }
        public string SControlaGarantia { get; set; }
        public decimal NQuantidade { get; set; }
        //public string SCodigoBarrasUnitizado { get => _sCodigoBarrasUnitizado; set => _sCodigoBarrasUnitizado = value; }
        // public string SCodigoBarrasProduto { get => _sCodigoBarrasProduto; set => _sCodigoBarrasProduto = value; }
        #endregion
    }
}