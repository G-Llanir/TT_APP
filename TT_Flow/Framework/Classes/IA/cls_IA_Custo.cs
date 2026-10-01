using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace TT_Flow.FrameWork.IA
{
    [Serializable]
    public class IAPrecoModelo
    {
        public decimal EntradaPor1M { get; set; }
        public decimal SaidaPor1M { get; set; }
        public bool Definido { get; set; }
    }

    // Calculo central de custo estimado em US$ por 1 milhao de tokens.
    // Preco especifico por modelo (IA.Custo.PrecosModelosJson) com fallback no preco padrao
    // (IA.Custo.PrecoEntradaPor1M / PrecoSaidaPor1M). Sem nenhum preco configurado, custo fica indefinido.
    public static class cls_IA_Custo
    {
        public static Dictionary<string, IAPrecoModelo> ObterPrecosModelos(cls_IA_Config config)
        {
            Dictionary<string, IAPrecoModelo> precos = new Dictionary<string, IAPrecoModelo>(StringComparer.OrdinalIgnoreCase);

            if (config == null || string.IsNullOrWhiteSpace(config.CustoPrecosModelosJson))
            {
                return precos;
            }

            try
            {
                JObject obj = JObject.Parse(config.CustoPrecosModelosJson);

                foreach (JProperty propriedade in obj.Properties())
                {
                    JObject preco = propriedade.Value as JObject;
                    string modelo = (propriedade.Name ?? string.Empty).Trim();

                    if (preco == null || modelo.Length == 0)
                    {
                        continue;
                    }

                    precos[modelo] = new IAPrecoModelo
                    {
                        EntradaPor1M = LerDecimal(preco["entrada"]),
                        SaidaPor1M = LerDecimal(preco["saida"]),
                        Definido = true
                    };
                }
            }
            catch
            {
                // JSON invalido na config: ignora e cai no preco padrao
            }

            return precos;
        }

        public static IAPrecoModelo ObterPrecoModelo(cls_IA_Config config, Dictionary<string, IAPrecoModelo> precosModelos, string modelo)
        {
            IAPrecoModelo preco;
            if (!string.IsNullOrWhiteSpace(modelo) && precosModelos != null && precosModelos.TryGetValue(modelo.Trim(), out preco))
            {
                return preco;
            }

            if (config != null && (config.CustoPrecoEntradaPor1M > 0 || config.CustoPrecoSaidaPor1M > 0))
            {
                return new IAPrecoModelo
                {
                    EntradaPor1M = config.CustoPrecoEntradaPor1M,
                    SaidaPor1M = config.CustoPrecoSaidaPor1M,
                    Definido = true
                };
            }

            return new IAPrecoModelo();
        }

        public static decimal Calcular(long tokensEntrada, long tokensSaida, IAPrecoModelo preco)
        {
            if (preco == null || !preco.Definido)
            {
                return 0;
            }

            return (tokensEntrada / 1000000m) * preco.EntradaPor1M + (tokensSaida / 1000000m) * preco.SaidaPor1M;
        }

        public static bool AlgumPrecoConfigurado(cls_IA_Config config)
        {
            if (config == null)
            {
                return false;
            }

            if (config.CustoPrecoEntradaPor1M > 0 || config.CustoPrecoSaidaPor1M > 0)
            {
                return true;
            }

            return ObterPrecosModelos(config).Count > 0;
        }

        public static string SerializarPrecos(Dictionary<string, IAPrecoModelo> precos)
        {
            JObject obj = new JObject();

            foreach (KeyValuePair<string, IAPrecoModelo> item in precos ?? new Dictionary<string, IAPrecoModelo>())
            {
                if (item.Value == null)
                {
                    continue;
                }

                obj[item.Key] = new JObject
                {
                    { "entrada", item.Value.EntradaPor1M },
                    { "saida", item.Value.SaidaPor1M }
                };
            }

            return obj.ToString(Newtonsoft.Json.Formatting.None);
        }

        public static string FormatarUSD(decimal valor)
        {
            return "US$ " + valor.ToString("N4");
        }

        private static decimal LerDecimal(JToken token)
        {
            if (token == null)
            {
                return 0;
            }

            decimal retorno;
            string texto = token.ToString().Trim().Replace(",", ".");

            return decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out retorno) && retorno >= 0
                ? retorno
                : 0;
        }
    }
}
