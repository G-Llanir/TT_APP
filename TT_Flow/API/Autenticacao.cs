using System.Collections.Generic;
using System.Data;
using System.Web;
using BD = TT.FrameWork.BD;

namespace Api
{
    public static class Autenticacao
    {
        public static DataSet ValidarSessao(HttpContext context)
        {
            string sChaveSessao = context.Request.Headers["sChaveSessao"];

            if (string.IsNullOrWhiteSpace(sChaveSessao))
                return null;

            DataSet ds = BD.ExecutarDataSet(
                "sp_Manipula_tbl_Usuarios_Sessao",
                new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR SESSAO" },
                    { "@sChaveSessao", sChaveSessao }
                });

            if (!BD.ValidarDataSet(ds))
                return null;

            return ds;
        }

        public static string ObterIdUsuario(DataSet dsSessao)
        {
            return BD.Retorno.DATASET(dsSessao, "idUsuario");
        }
        public static string obterPermissaoUsuario(DataSet dsSessao)
        {
            return BD.Retorno.DATASET(dsSessao, "sPermissao");
        }
    }
}
