using Framework.API;

namespace TT.FrameWork
{
    public class API
    {
        public class LegisWeb
        {
            public static string idUsuario = "63382";
            public static string sToken = "dec7f1cd9704f3f813b18e5e8c844a7a";
            public static string sLinkAPI = "https://www.legisweb.com.br/api";

            public static string Consultar_II(string sNCM) => Funcoes_API.ConsumirAPI($"{sLinkAPI}/ii/?ncm={sNCM}&c={idUsuario}&t={sToken}");

            public static string Consultar_IPI(string sNCM) => Funcoes_API.ConsumirAPI($"{sLinkAPI}/ipi/?ncm={sNCM}&c={idUsuario}&t={sToken}");

            public static string Consultar_PIS_COFINS(string sNCM, string sRegime_Origem, string sAtividade_Origem) => Funcoes_API.ConsumirAPI($"{sLinkAPI}/piscofins/?ncm={sNCM}&regime_tributario_origem={sRegime_Origem}&atividade_origem={sAtividade_Origem}&c={idUsuario}&t={sToken}");

            public static string Consultar_ICMS(string sEstado) => Funcoes_API.ConsumirAPI($"{sLinkAPI}/aliquota-padrao/?estado={sEstado}&c={idUsuario}&t={sToken}");

            public static string Consultar_ST_Interestadual(string sUFOrigem, string sUFDestino, string sNCM, string sTipoVenda) => Funcoes_API.ConsumirAPI($"{sLinkAPI}/st-interestadual/?ncm={sNCM}&estado_origem={sUFOrigem}&estado_destino={sUFDestino}&destinacao_mercadoria={sTipoVenda}&c={idUsuario}&t={sToken}");

            public static string Consultar_ST_Interestadual_x_CEST(string sUFOrigem, string sUFDestino, string sCEST, string sTipoVenda) => Funcoes_API.ConsumirAPI($"{sLinkAPI}/st-interestadual/?cest={sCEST}&estado_origem={sUFOrigem}&estado_destino={sUFDestino}&destinacao_mercadoria={sTipoVenda}&c={idUsuario}&t={sToken}");
        }
    }
}