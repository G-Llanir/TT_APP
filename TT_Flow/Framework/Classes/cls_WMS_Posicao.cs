using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace TT_Flow.FrameWork.Classes
{
    [Serializable]
    public class cls_WMS_Posicao
    {
        public cls_WMS_Posicao()
        {

        }

        public int IdPosicao { get; set; }
        public string SDscPosicao { get; set; }
        public string SCodigoLocal { get; set; }
        public string SCodigoLocalPai { get; set; }
        public int? IdLocal { get; set; }
        public string SDscLocal { get; set; }
        public int? IdPosicaoPai { get; set; }
        public string SDscPai { get; set; }
        public DateTime? DtAtualizacao { get; set; }
        public int? IdUsuario { get; set; }
        public int? NQuantidadeLimite { get; set; }
        public string SSituacao { get; set; }
        public Guid IdTemporario { get; set; } = Guid.NewGuid();
        public Guid? IdTemporarioPai { get; set; } = null;
    }
}
