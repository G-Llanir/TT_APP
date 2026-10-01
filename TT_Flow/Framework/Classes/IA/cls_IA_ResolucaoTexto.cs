using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TT_Flow.FrameWork.IA
{
    internal static class cls_IA_ResolucaoTexto
    {
        private static readonly Dictionary<string, string> AliasesCategorias = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "ecomerce", "ecommerce" },
            { "ecommerce", "ecommerce" },
            { "ecommerceb2b", "ecommerce b2b" },
            { "ecommerceb2c", "ecommerce b2c" }
        };

        public static string Normalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

            string formD = texto.ToLowerInvariant().Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder(formD.Length);
            bool ultimoEspaco = true;
            foreach (char c in formD)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark) continue;

                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                    ultimoEspaco = false;
                }
                else if (c == '+' && !ultimoEspaco)
                {
                    // "+" colado no fim da palavra faz parte do nome: BR2, BR2+ e BR2++ sao tabelas diferentes.
                    // "+" solto ("Material + Servicos") continua sendo separador.
                    sb.Append(c);
                }
                else if (!ultimoEspaco)
                {
                    sb.Append(' ');
                    ultimoEspaco = true;
                }
            }

            return sb.ToString().Trim().Normalize(NormalizationForm.FormC);
        }

        public static string Compactar(string texto)
        {
            return Normalizar(texto).Replace(" ", string.Empty);
        }

        public static string AplicarAliasCategoria(string texto)
        {
            string compacto = Compactar(texto);
            string alias;
            return AliasesCategorias.TryGetValue(compacto, out alias) ? alias : Normalizar(texto);
        }

        public static int Pontuar(string termo, string valor)
        {
            string alvo = Normalizar(termo);
            string candidato = Normalizar(valor);
            if (alvo.Length == 0 || candidato.Length == 0) return 1;

            if (candidato == alvo) return 120;

            string alvoCompacto = Compactar(alvo);
            string candidatoCompacto = Compactar(candidato);
            if (candidatoCompacto == alvoCompacto) return 115;
            if (candidato.StartsWith(alvo, StringComparison.Ordinal)) return 90;
            // 71 quando o trecho aparece como palavra inteira ("br2" em "vendas br2") e 70 quando e pedaco de palavra
            // ("br2" em "vendas br2+"): so desempata a ordem das opcoes, sem mudar o limiar de 70 de "provavel".
            if (candidato.Contains(alvo)) return (" " + candidato + " ").Contains(" " + alvo + " ") ? 71 : 70;

            int porPalavras = PontuarPorPalavras(alvo, candidato);
            if (porPalavras > 0) return porPalavras;

            if (alvoCompacto.Length < 4 || candidatoCompacto.Length < 4) return 1;
            double similaridade = Similaridade(alvoCompacto, candidatoCompacto);
            if (similaridade < 0.70d) return 1;

            return Math.Min(69, 40 + (int)Math.Round(similaridade * 29d));
        }

        public static bool EhCandidatoAproximado(string termo, string valor)
        {
            return Pontuar(termo, valor) >= 60;
        }

        // Palavras de ligacao que o usuario costuma omitir ou acrescentar em nomes ("Maria de Souza" x "Maria Souza").
        private static readonly HashSet<string> PalavrasDeLigacao = new HashSet<string>(StringComparer.Ordinal)
        {
            "de", "da", "do", "das", "dos", "e", "a", "o"
        };

        // Casa o termo com o candidato palavra por palavra, em qualquer ordem e com palavras no meio:
        // "Thiago Reis" x "Thiago Garcia B. dos Reis". Cada palavra do termo precisa achar uma palavra
        // diferente do candidato, igual (70), comecando com ela (66) ou parecida por erro de digitacao (62).
        // So as igualdades chegam a 70, a pontuacao em que um unico candidato ja e tratado como provavel;
        // prefixo e semelhanca ficam abaixo, entao o usuario confirma em vez de a escolha ser silenciosa.
        // Devolve 0 quando alguma palavra do termo nao acha par.
        private static int PontuarPorPalavras(string alvo, string candidato)
        {
            List<string> palavrasAlvo = SepararPalavras(alvo, true);
            if (palavrasAlvo.Count == 0) return 0;

            List<string> livres = SepararPalavras(candidato, false);
            if (livres.Count == 0) return 0;

            bool houvePrefixo = false;
            bool houveSemelhanca = false;
            foreach (string palavra in palavrasAlvo)
            {
                int melhorIdx = -1;
                int melhorTipo = 0; // 3 igual, 2 prefixo, 1 semelhante
                for (int i = 0; i < livres.Count; i++)
                {
                    int tipo = TipoDeCasamento(palavra, livres[i]);
                    if (tipo > melhorTipo)
                    {
                        melhorTipo = tipo;
                        melhorIdx = i;
                        if (tipo == 3) break;
                    }
                }

                if (melhorIdx < 0) return 0;

                if (melhorTipo == 2) houvePrefixo = true;
                else if (melhorTipo == 1) houveSemelhanca = true;
                livres.RemoveAt(melhorIdx);
            }

            if (houveSemelhanca) return 62;
            return houvePrefixo ? 66 : 70;
        }

        private static int TipoDeCasamento(string palavra, string candidata)
        {
            if (palavra == candidata) return 3;
            if (palavra.Length >= 3 && candidata.StartsWith(palavra, StringComparison.Ordinal)) return 2;
            if (palavra.Length >= 4 && candidata.Length >= 4 && Similaridade(palavra, candidata) >= 0.75d) return 1;
            return 0;
        }

        private static List<string> SepararPalavras(string normalizado, bool ignorarLigacao)
        {
            List<string> palavras = new List<string>();
            foreach (string p in normalizado.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                palavras.Add(p);
            }

            if (!ignorarLigacao) return palavras;

            List<string> semLigacao = palavras.FindAll(delegate (string p) { return !PalavrasDeLigacao.Contains(p); });
            return semLigacao.Count > 0 ? semLigacao : palavras;
        }

        private static double Similaridade(string a, string b)
        {
            int maior = Math.Max(a.Length, b.Length);
            if (maior == 0) return 1d;
            return 1d - ((double)DistanciaLevenshtein(a, b) / maior);
        }

        private static int DistanciaLevenshtein(string a, string b)
        {
            int[] anterior = new int[b.Length + 1];
            int[] atual = new int[b.Length + 1];
            for (int j = 0; j <= b.Length; j++) anterior[j] = j;

            for (int i = 1; i <= a.Length; i++)
            {
                atual[0] = i;
                for (int j = 1; j <= b.Length; j++)
                {
                    int custo = a[i - 1] == b[j - 1] ? 0 : 1;
                    atual[j] = Math.Min(Math.Min(atual[j - 1] + 1, anterior[j] + 1), anterior[j - 1] + custo);
                }

                int[] troca = anterior;
                anterior = atual;
                atual = troca;
            }

            return anterior[b.Length];
        }
    }
}
