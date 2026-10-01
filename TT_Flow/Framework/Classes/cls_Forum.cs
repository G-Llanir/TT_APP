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
    public class cls_Forum
    {
        #region Atributos
        public cls_Forum()
        {

        }

        public int IdForum { get; set; }
        public int IdComentarion { get; set; }
        public string STema { get; set; }
        public string SCorpo { get; set; }
        public DateTime dtAtualizacao { get; set; }
        public string SDscUsuarioAtualizacao { get; set; }
        #endregion

        #region Consultar_Forum
        public static DataSet Consultar_Forum(string sDscPesquisa, string dtCriado, string idCategoria, string idDepartamento, string idStatus)
        {
            DataSet dsPesquisa;
            string sProcedure = "sp_Manipula_tbl_Flow_Forum";

            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR");
            vParametros.Add("@sPesquisa", sDscPesquisa);
            vParametros.Add("@dtCriado", dtCriado);
            vParametros.Add("@idCategoria", idCategoria);
            vParametros.Add("@idDepartamento", idDepartamento);
            vParametros.Add("@idStatus", idStatus);
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            return dsPesquisa;

        }
        #endregion

        #region Consultar_Forum_Tags_Detalhe
        public static DataSet Consultar_Forum_Tags(string idForum)
        {
            DataSet dsPesquisa;
            string sProcedure = "sp_Manipula_tbl_Flow_Forum";

            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_TAGS");
            vParametros.Add("@idForum", idForum);
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

        #region Consultar_Forum_nComentarios
        public static DataSet Consultar_Forum_nRespostas(string idForum)
        {
            DataSet dsPesquisa;
            string sProcedure = "sp_Manipula_tbl_Flow_Forum";

            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_NRESPOSTAS");
            vParametros.Add("@idForum", idForum);
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
            
            return dsPesquisa;

        }
        #endregion

        #region ConverterDatasetForum
        void ConverterDatasetClasseForum(DataSet ds, List<cls_Forum> BS_Forum)
        {
            
            foreach(DataRow row in ds.Tables)
            {
                cls_Forum cls = new cls_Forum();
                cls.IdForum = Convert.ToInt32(row["idForum"]);
                cls.STema = row["sTema"].ToString();
                cls.dtAtualizacao = Convert.ToDateTime(row["dtAtualizacao"].ToString());
                cls.SDscUsuarioAtualizacao = row["sDscUsuarioAtualizacao"].ToString();
            }
        }
        #endregion

    }
}