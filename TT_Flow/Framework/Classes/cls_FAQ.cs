using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using TT.FrameWork;

namespace TT_Flow.FrameWork.Classes
{
    [Serializable]
    public class cls_FAQ
    {
        #region Atributos
        public cls_FAQ()
        {

        }

        public int IdFAQ { get; set; }
        public int IdComentarion { get; set; }
        public string STema { get; set; }
        public string SCorpo { get; set; }
        public DateTime dtAtualizacao { get; set; }
        public string SDscUsuarioAtualizacao { get; set; }
        #endregion

        #region Consultar_FAQ
        public static DataSet Consultar_FAQ(string sDscPesquisa, string dtCriado, string idCategoria, string idDepartamento, string idStatus, string sAprova)
        {
            DataSet dsPesquisa;
            string sProcedure = "sp_Manipula_tbl_Flow_FAQ";

            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR");
            vParametros.Add("@sPesquisa", sDscPesquisa);
            vParametros.Add("@dtCriado", dtCriado);
            vParametros.Add("@idCategoria", idCategoria);
            vParametros.Add("@idDepartamento", idDepartamento);
            vParametros.Add("@idStatus", idStatus);
            vParametros.Add("@sUsuarioAprova", sAprova);
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            return dsPesquisa;

        }
        #endregion

        #region Consultar_FAQ_Tags_Detalhe
        public static DataSet Consultar_FAQ_Tags(string idFaq)
        {
            DataSet dsPesquisa;
            string sProcedure = "sp_Manipula_tbl_Flow_FAQ";

            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_TAGS");
            vParametros.Add("@idFaq", idFaq);
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsPesquisa))
            {
                return dsPesquisa;
            }
            else
            {
                throw new Exception("Erro de busca no banco de dados");
            }

        }
        #endregion

        #region Consultar_FAQ_nComentarios
        public static DataSet Consultar_FAQ_nComentarios(string idFaq)
        {
            DataSet dsPesquisa;
            string sProcedure = "sp_Manipula_tbl_Flow_FAQ";

            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_NCOMENTARIOS");
            vParametros.Add("@idFaq", idFaq);
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
            
            return dsPesquisa;

        }
        #endregion

    }
}