using System.Text.RegularExpressions;
using System.Web;

namespace TT_Flow.FrameWork.IA
{
    public static class cls_IA_Sanitizacao
    {
        public static string LimparEntradaUsuario(string texto, int limiteCaracteres)
        {
            texto = (texto ?? string.Empty).Trim();
            texto = Regex.Replace(texto, @"[\x00-\x08\x0B\x0C\x0E-\x1F]", string.Empty); // Remove caracteres de controle invisíveis da entrada do usuário.

            if (limiteCaracteres > 0 && texto.Length > limiteCaracteres)
            {
                texto = texto.Substring(0, limiteCaracteres);
            }

            return texto;
        }

        public static string Resumir(string texto, int limiteCaracteres)
        {
            texto = RemoverHtml(texto ?? string.Empty).Trim();

            if (limiteCaracteres > 0 && texto.Length > limiteCaracteres)
            {
                texto = texto.Substring(0, limiteCaracteres) + "...";
            }

            return texto;
        }

        public static string RemoverHtml(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            string decodificado = HttpUtility.HtmlDecode(texto);
            return Regex.Replace(decodificado, "<.*?>", string.Empty);
        }
    }
}
