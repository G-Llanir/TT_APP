using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace TT_Flow.FrameWork.IA
{
    public class IAWorkflowAvisoGrafo
    {
        public string Nivel { get; set; } // ERRO | AVISO
        public string No { get; set; }
        public string Mensagem { get; set; }
    }

    // Confere o grafo de um workflow antes de salvar. O editor já valida a estrutura (conectores, parâmetros obrigatórios);
    // aqui entram as referências entre nós e os vícios que só aparecem na hora de rodar, sem nenhum erro visível:
    // - ERRO (bloqueia o salvamento): referência a nó que não existe ou a própria saída do nó. O motor resolve isso
    //   para vazio em silêncio e o passo seguinte falha com uma mensagem que parece culpa do usuário.
    // - AVISO: nó que lê a saída de outro que não roda antes dele; caminho de saída que só funciona por compatibilidade;
    //   entrada não declarada (inclusive em prompt de nó de IA); valor fixo em parâmetro que costuma variar; nó
    //   inalcançável a partir do início.
    public static class cls_IA_WorkflowValidador
    {
        // Ferramentas que devolvem o formato de resolução (TabelaResolucaoParaJson): o registro escolhido fica em
        // melhorResultado, não no topo da saída.
        private static readonly HashSet<string> FerramentasDeResolucao = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "orcamento_tipos_listar", "clientes_resolver_para_orcamento", "orcamento_empresas_buscar", "orcamento_vendedores_buscar",
            "orcamento_usuarios_resolver", "orcamento_tabelas_preco_buscar", "orcamento_tipos_envio_buscar", "orcamento_produtos_buscar",
            "orcamento_condicao_pagamento_detalhar"
        };

        private static readonly HashSet<string> CamposDeTopoDaResolucao = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "sucesso", "ferramenta", "termo", "totalEncontrado", "totalRetornado", "encontrado", "unicoProvavel", "precisaEscolha",
            "aproximado", "melhorResultado", "registros", "opcoes", "mensagem", "selecionados", "selecaoMultipla", "condicoes", "parcelas"
        };

        private static readonly string[] CamposDeSaida = { "proximo", "seVerdadeiro", "seFalso", "seRejeitado", "aoConcluir", "emErro" };

        // Parâmetros que escolhem o registro de negócio: com valor fixo, o workflow deixa de seguir o que o usuário pediu.
        private static readonly HashSet<string> ParametrosSeletores = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "tabelaPreco", "termo"
        };

        private static readonly Regex EntradaNoTexto = new Regex(@"(?<![A-Za-z0-9_])entrada\.([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Compiled);

        private sealed class Contexto
        {
            public Dictionary<string, JObject> Nos = new Dictionary<string, JObject>(StringComparer.OrdinalIgnoreCase);
            public List<JObject> Ordem = new List<JObject>();
            public HashSet<string> EntradasDeclaradas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public Dictionary<string, List<string>> Origens = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            public Dictionary<string, HashSet<string>> CacheAncestrais = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            public Func<string, IAFerramentaDefinicao> ObterDef;
            public List<IAWorkflowAvisoGrafo> Saida = new List<IAWorkflowAvisoGrafo>();
        }

        public static List<IAWorkflowAvisoGrafo> Validar(string grafoJson, Func<string, IAFerramentaDefinicao> obterDef)
        {
            Contexto c = new Contexto { ObterDef = obterDef ?? (nome => null) };

            JObject grafo;
            try { grafo = JObject.Parse(string.IsNullOrWhiteSpace(grafoJson) ? "{}" : grafoJson); }
            catch (Exception ex)
            {
                Adicionar(c, "ERRO", string.Empty, "Grafo inválido: " + ex.Message);
                return c.Saida;
            }

            foreach (JToken t in (grafo["entradas"] as JArray) ?? new JArray())
            {
                string nome = Str(t["nome"]).Trim();
                if (nome.Length > 0) c.EntradasDeclaradas.Add(nome);
            }

            string idInicio = null;
            Dictionary<string, List<string>> destinos = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (JToken t in (grafo["nos"] as JArray) ?? new JArray())
            {
                JObject no = t as JObject;
                string id = no != null ? Str(no["id"]) : string.Empty;
                if (id.Length == 0) continue;

                c.Nos[id] = no;
                c.Ordem.Add(no);
                if (string.Equals(Str(no["tipo"]), "inicio", StringComparison.OrdinalIgnoreCase)) idInicio = id;
            }

            foreach (JObject no in c.Ordem)
            {
                string id = Str(no["id"]);
                foreach (string campo in CamposDeSaida)
                {
                    string destino = Str(no[campo]);
                    if (destino.Length == 0 || !c.Nos.ContainsKey(destino)) continue;

                    List<string> lista;
                    if (!destinos.TryGetValue(id, out lista)) destinos[id] = lista = new List<string>();
                    lista.Add(destino);
                    if (!c.Origens.TryGetValue(destino, out lista)) c.Origens[destino] = lista = new List<string>();
                    lista.Add(id);
                }
            }

            HashSet<string> alcancaveis = Percorrer(idInicio, destinos);

            foreach (JObject no in c.Ordem)
            {
                string id = Str(no["id"]);
                string tipo = Str(no["tipo"]).ToLowerInvariant();

                if (idInicio != null && !alcancaveis.Contains(id))
                {
                    Adicionar(c, "AVISO", id, "nó não é alcançável a partir do início; ele nunca será executado.");
                }

                if (tipo == "ferramenta" || tipo == "ia") ValidarMapeamentos(c, no, tipo);
                else if (tipo == "condicao") ValidarCondicao(c, no);
                else if (tipo == "definir") ValidarOrigemUnica(c, no, Str(no["origem"]), Str(no["valor"]), "valor");
                else if (tipo == "loop" && string.Equals(Str(no["modoLoop"]), "lista", StringComparison.OrdinalIgnoreCase))
                {
                    ValidarOrigemUnica(c, no, Str(no["listaOrigem"]), Str(no["listaValor"]), "lista");
                }

                if (tipo == "ia") ValidarPromptDoNoIA(c, no);
            }

            return c.Saida;
        }

        private static void ValidarMapeamentos(Contexto c, JObject no, string tipo)
        {
            IAFerramentaDefinicao def = tipo == "ferramenta" ? c.ObterDef(Str(no["ferramenta"])) : null;
            foreach (JToken t in (no["entradas"] as JArray) ?? new JArray())
            {
                JObject mapeamento = t as JObject;
                if (mapeamento == null) continue;

                string param = Str(mapeamento["param"]).Trim();
                string origem = Str(mapeamento["origem"]).Trim().ToLowerInvariant();
                string valor = Str(mapeamento["valor"]).Trim();

                if (origem == "passo") ValidarReferenciaPasso(c, no, param, valor);
                else if (origem == "entrada") ValidarEntrada(c, no, param, valor.Length > 0 ? valor : param);
                else if (origem == "fixo" && def != null) AvaliarValorFixo(c, no, def, param, valor);
            }
        }

        private static void ValidarCondicao(Contexto c, JObject no)
        {
            ValidarReferenciaPasso(c, no, "campo", Str(no["campo"]));
            ValidarOrigemUnica(c, no, Str(no["valorOrigem"]), Str(no["valor"]), "valor");
        }

        private static void ValidarOrigemUnica(Contexto c, JObject no, string origem, string valor, string rotulo)
        {
            origem = (origem ?? string.Empty).Trim().ToLowerInvariant();
            if (origem == "passo") ValidarReferenciaPasso(c, no, rotulo, valor);
            else if (origem == "entrada") ValidarEntrada(c, no, rotulo, valor);
        }

        private static void ValidarEntrada(Contexto c, JObject no, string param, string nome)
        {
            nome = (nome ?? string.Empty).Trim();
            if (nome.StartsWith("entrada.", StringComparison.OrdinalIgnoreCase)) nome = nome.Substring("entrada.".Length);
            int corte = nome.IndexOfAny(new[] { '.', '[' });
            string basico = corte >= 0 ? nome.Substring(0, corte) : nome;
            if (basico.Length == 0 || c.EntradasDeclaradas.Contains(basico)) return;

            Adicionar(c, "AVISO", Str(no["id"]), Descrever(param) + " lê a entrada \"" + basico + "\", que não está declarada nas entradas do workflow.");
        }

        private static void ValidarPromptDoNoIA(Contexto c, JObject no)
        {
            HashSet<string> vistas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (Match m in EntradaNoTexto.Matches(Str(no["prompt"])))
            {
                string nome = m.Groups[1].Value;
                if (c.EntradasDeclaradas.Contains(nome) || !vistas.Add(nome)) continue;

                Adicionar(c, "AVISO", Str(no["id"]), "o prompt cita \"entrada." + nome + "\", que não está declarada nas entradas do workflow (o prompt pode estar desatualizado).");
            }
        }

        // "nA.campo || nB.campo": cada alternativa é conferida sozinha (um erro de digitação numa delas também é bug).
        private static void ValidarReferenciaPasso(Contexto c, JObject consumidor, string param, string referencia)
        {
            foreach (string alternativa in cls_IA_WorkflowEngine.DividirAlternativas(referencia))
            {
                ValidarReferenciaPassoSimples(c, consumidor, param, alternativa);
            }
        }

        private static void ValidarReferenciaPassoSimples(Contexto c, JObject consumidor, string param, string referencia)
        {
            referencia = (referencia ?? string.Empty).Trim();
            if (referencia.Length == 0) return; // sem valor: os obrigatórios já são cobrados pelo editor

            string idConsumidor = Str(consumidor["id"]);
            int ponto = referencia.IndexOf('.');
            string idOrigem = ponto >= 0 ? referencia.Substring(0, ponto) : referencia;
            string caminho = ponto >= 0 ? referencia.Substring(ponto + 1) : string.Empty;
            string quem = Descrever(param);

            JObject origem;
            if (!c.Nos.TryGetValue(idOrigem, out origem))
            {
                // "entrada.x" onde só cabe saída de passo é engano comum, mas grafos antigos convivem com ele: só avisa.
                bool confusaoComEntrada = string.Equals(idOrigem, "entrada", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(idOrigem, "token", StringComparison.OrdinalIgnoreCase);
                Adicionar(c, confusaoComEntrada ? "AVISO" : "ERRO", idConsumidor, confusaoComEntrada
                    ? quem + " lê \"" + referencia + "\" como se fosse a saída de um passo; para ler uma entrada do workflow use a origem \"entrada\"."
                    : quem + " lê \"" + referencia + "\", mas o nó " + idOrigem + " não existe no grafo.");
                return;
            }
            if (string.Equals(idOrigem, idConsumidor, StringComparison.OrdinalIgnoreCase))
            {
                Adicionar(c, "ERRO", idConsumidor, quem + " lê a própria saída (\"" + referencia + "\").");
                return;
            }

            string tipoOrigem = Str(origem["tipo"]).ToLowerInvariant();
            if (tipoOrigem == "inicio" || tipoOrigem == "fim" || tipoOrigem == "merge")
            {
                Adicionar(c, "AVISO", idConsumidor, quem + " lê \"" + referencia + "\", mas o nó " + idOrigem + " (" + tipoOrigem + ") não produz saída.");
                return;
            }

            if (!Ancestrais(c, idConsumidor).Contains(idOrigem))
            {
                Adicionar(c, "AVISO", idConsumidor, quem + " lê \"" + referencia + "\", mas o nó " + idOrigem + " não é executado antes deste em nenhum caminho do grafo.");
                return;
            }

            if (tipoOrigem == "ferramenta" && caminho.Length > 0 && FerramentasDeResolucao.Contains(Str(origem["ferramenta"])))
            {
                int corte = caminho.IndexOfAny(new[] { '.', '[' });
                string primeiro = corte >= 0 ? caminho.Substring(0, corte) : caminho;
                if (!CamposDeTopoDaResolucao.Contains(primeiro))
                {
                    Adicionar(c, "AVISO", idConsumidor, quem + " lê \"" + referencia + "\", mas " + Str(origem["ferramenta"])
                        + " guarda o registro escolhido em melhorResultado. Use \"" + idOrigem + ".melhorResultado." + caminho
                        + "\" (o caminho antigo só funciona por compatibilidade).");
                }
            }
        }

        private static void AvaliarValorFixo(Contexto c, JObject no, IAFerramentaDefinicao def, string param, string valor)
        {
            if (valor.Length == 0 || valor == "0") return;

            JObject props = def.SchemaParametros != null ? def.SchemaParametros["properties"] as JObject : null;
            bool idObrigatorio = props != null && cls_IA_WorkflowEngine.ParametroEhIdObrigatorio(param, props[param] as JObject);
            if (!ParametrosSeletores.Contains(param) && !idObrigatorio) return;

            Adicionar(c, "AVISO", Str(no["id"]), "o parâmetro \"" + param + "\" tem valor fixo \"" + valor
                + "\"; se ele deve acompanhar o que o usuário pediu, mapeie da entrada ou de um passo anterior.");
        }

        private static string Descrever(string param)
        {
            return string.IsNullOrEmpty(param) ? "a referência" : "o parâmetro \"" + param + "\"";
        }

        private static HashSet<string> Percorrer(string inicio, Dictionary<string, List<string>> arestas)
        {
            HashSet<string> vistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (inicio == null) return vistos;

            Queue<string> fila = new Queue<string>();
            fila.Enqueue(inicio);
            vistos.Add(inicio);
            while (fila.Count > 0)
            {
                string atual = fila.Dequeue();
                List<string> proximos;
                if (!arestas.TryGetValue(atual, out proximos)) continue;
                foreach (string p in proximos) if (vistos.Add(p)) fila.Enqueue(p);
            }
            return vistos;
        }

        // Nós que podem ter sido executados antes do dado (qualquer caminho até ele, inclusive voltas de loop).
        private static HashSet<string> Ancestrais(Contexto c, string id)
        {
            HashSet<string> cache;
            if (c.CacheAncestrais.TryGetValue(id, out cache)) return cache;

            cache = Percorrer(id, c.Origens);
            cache.Remove(id);
            // Um nó dentro de um loop pode ler a própria saída da volta anterior; isso é tratado pelo ERRO de auto-leitura.
            c.CacheAncestrais[id] = cache;
            return cache;
        }

        private static void Adicionar(Contexto c, string nivel, string no, string mensagem)
        {
            c.Saida.Add(new IAWorkflowAvisoGrafo { Nivel = nivel, No = no, Mensagem = (no.Length > 0 ? no + ": " : string.Empty) + mensagem });
        }

        private static string Str(JToken t) { return t == null ? string.Empty : t.ToString(); }
    }
}
