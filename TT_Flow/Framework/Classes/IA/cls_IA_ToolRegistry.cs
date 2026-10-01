using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json.Linq;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Flow.FrameWork.IA
{
    public static class cls_IA_ToolRegistry
    {
        public static List<IAFerramentaDefinicao> ListarPermitidas()
        {
            List<IAFerramentaDefinicao> ferramentas = new List<IAFerramentaDefinicao>();

            foreach (IAFerramentaDefinicao ferramenta in ListarTodas())
            {
                // Ferramentas de escrita exigem tambem a permissao IA.ExecutarAcoes (685)
                if (string.Equals(ferramenta.Escopo, "WRITE", System.StringComparison.OrdinalIgnoreCase) &&
                    !FUNCOES.ValidaPermissao(Permissao.IA.ExecutarAcoes, false))
                {
                    continue;
                }

                if (FUNCOES.ValidaPermissao(ferramenta.IdRecursoNecessario, false))
                {
                    ferramentas.Add(ferramenta);
                }
            }

            return ferramentas;
        }

        public static JArray ListarOpenAI()
        {
            JArray tools = new JArray();

            foreach (IAFerramentaDefinicao ferramenta in ListarPermitidas())
            {
                // As ferramentas viajam todas na MESMA requisicao: uma so com schema invalido faz o provider
                // recusar a chamada inteira e derruba o chat. Melhor a ferramenta sumir do catalogo e virar
                // alerta na auditoria do que um admin conseguir tirar a IA do ar sem querer.
                string motivo;
                if (!SchemaAceitavel(ferramenta, out motivo))
                {
                    cls_IA_Auditoria.Registrar(0, "FERRAMENTA_IGNORADA", "ALERTA", new
                    {
                        idFerramentaIA = ferramenta.IdFerramentaIA,
                        ferramenta = ferramenta.Nome,
                        motivo = motivo
                    });
                    continue;
                }

                tools.Add(ferramenta.ParaOpenAI());
            }

            return tools;
        }

        // Os providers exigem o nome em ^[a-zA-Z0-9_-]{1,64}$ e as chaves do schema em ^[a-zA-Z0-9.-]{1,64}$
        // (sem espaco, acento ou underline nas chaves).
        private static readonly System.Text.RegularExpressions.Regex _rgxNomeFerramenta =
            new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9_-]{1,64}$", System.Text.RegularExpressions.RegexOptions.Compiled);

        private static readonly System.Text.RegularExpressions.Regex _rgxChaveSchema =
            new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9.-]{1,64}$", System.Text.RegularExpressions.RegexOptions.Compiled);

        private static bool SchemaAceitavel(IAFerramentaDefinicao ferramenta, out string motivo)
        {
            motivo = null;

            if (string.IsNullOrWhiteSpace(ferramenta.Nome) || !_rgxNomeFerramenta.IsMatch(ferramenta.Nome))
            {
                motivo = "nome interno fora do padrao aceito pelos providers";
                return false;
            }

            if (ferramenta.SchemaParametros == null)
            {
                motivo = "schema de parametros vazio ou invalido";
                return false;
            }

            JObject propriedades = ferramenta.SchemaParametros["properties"] as JObject;
            if (propriedades != null)
            {
                foreach (JProperty propriedade in propriedades.Properties())
                {
                    if (!_rgxChaveSchema.IsMatch(propriedade.Name))
                    {
                        motivo = "argumento com nome invalido: " + propriedade.Name;
                        return false;
                    }
                }
            }

            return true;
        }

        public static IAFerramentaDefinicao Obter(string nome)
        {
            foreach (IAFerramentaDefinicao ferramenta in ListarTodas())
            {
                if ((ferramenta.Nome ?? string.Empty).Equals(nome ?? string.Empty, System.StringComparison.OrdinalIgnoreCase))
                {
                    return ferramenta;
                }
            }

            return null;
        }

        public static IAFerramentaDefinicao ObterPermitida(string nome)
        {
            IAFerramentaDefinicao ferramenta = Obter(nome);

            if (ferramenta == null || !FUNCOES.ValidaPermissao(ferramenta.IdRecursoNecessario, false))
            {
                return null;
            }

            return ferramenta;
        }

        public static string DiagnosticarDisponibilidade(string nome, bool validarPermissao)
        {
            if (string.IsNullOrWhiteSpace(nome)) return "Ferramenta sem nome.";

            IAFerramentaDefinicao encontrada = null;
            foreach (IAFerramentaDefinicao ferramenta in ListarTodasIncluindoInativas())
            {
                if (string.Equals(ferramenta.Nome, nome, StringComparison.OrdinalIgnoreCase))
                {
                    encontrada = ferramenta;
                    break;
                }
            }

            if (encontrada == null)
            {
                foreach (IAWorkflow workflow in new cls_IA_Repositorio().ListarWorkflows(true))
                {
                    if (!string.Equals(workflow.Nome, nome, StringComparison.OrdinalIgnoreCase)) continue;
                    if (!workflow.Ativo) return "Workflow/ferramenta \"" + nome + "\" está inativo.";
                    return string.Empty;
                }
                return "Ferramenta \"" + nome + "\" não está cadastrada.";
            }
            return DiagnosticarDisponibilidade(encontrada, validarPermissao);
        }

        public static string DiagnosticarDisponibilidade(IAFerramentaDefinicao encontrada, bool validarPermissao)
        {
            if (encontrada == null) return "Ferramenta não cadastrada.";
            string nome = encontrada.Nome ?? string.Empty;
            if (!encontrada.Ativo) return "Ferramenta \"" + nome + "\" está inativa.";

            string motivoSchema;
            if (!SchemaAceitavel(encontrada, out motivoSchema))
            {
                return "Ferramenta \"" + nome + "\" possui schema inválido: " + motivoSchema + ".";
            }
            if (validarPermissao && !FUNCOES.ValidaPermissao(encontrada.IdRecursoNecessario, false))
            {
                return "Usuário sem permissão para a ferramenta \"" + nome + "\".";
            }
            if (validarPermissao && string.Equals(encontrada.Escopo, "WRITE", StringComparison.OrdinalIgnoreCase)
                && !FUNCOES.ValidaPermissao(Permissao.IA.ExecutarAcoes, false))
            {
                return "Usuário sem permissão para executar ações WRITE pela IA.";
            }
            return string.Empty;
        }

        // Catálogo em runtime (apenas ativas), carregado do banco. Fonte da verdade das ferramentas =
        // tbl_Flow_IA_Ferramentas; workflows ativos entram aqui como ferramentas compostas.
        public static List<IAFerramentaDefinicao> ListarTodas()
        {
            return CarregarCatalogo(false, true);
        }

        // Inclui inativas, mas apenas ferramentas reais — usado pela tela de administração de ferramentas.
        public static List<IAFerramentaDefinicao> ListarTodasIncluindoInativas()
        {
            return CarregarCatalogo(true, false);
        }

        // Ferramenta "interna" = definida no codigo (DefaultsCodigo). O seed a recria se apagada,
        // por isso ela pode ser editada/desativada mas nao excluida. As demais sao "personalizadas".
        public static bool EhInterna(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return false;
            }

            foreach (IAFerramentaDefinicao def in DefaultsCodigo())
            {
                if (string.Equals(def.Nome, nome, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        // Limpa o cache por-requisicao (chamar apos ativar/desativar uma ferramenta na mesma requisicao).
        public static void InvalidarCache()
        {
            HttpContext ctx = HttpContext.Current;
            if (ctx == null)
            {
                return;
            }

            ctx.Items.Remove("IA_CATALOGO_ATIVAS");
            ctx.Items.Remove("IA_CATALOGO_TODAS");
            ctx.Items.Remove("IA_CATALOGO_ATIVAS_WORKFLOWS");
        }

        private static List<IAFerramentaDefinicao> CarregarCatalogo(bool incluirInativas, bool incluirWorkflows)
        {
            string chaveCache = incluirWorkflows ? "IA_CATALOGO_ATIVAS_WORKFLOWS" : (incluirInativas ? "IA_CATALOGO_TODAS" : "IA_CATALOGO_ATIVAS");
            HttpContext ctx = HttpContext.Current;
            if (ctx != null && ctx.Items[chaveCache] is List<IAFerramentaDefinicao> emCache)
            {
                return emCache;
            }

            List<IAFerramentaDefinicao> catalogo = MontarCatalogo(incluirInativas, incluirWorkflows);
            if (ctx != null)
            {
                ctx.Items[chaveCache] = catalogo;
            }

            return catalogo;
        }

        private static List<IAFerramentaDefinicao> MontarCatalogo(bool incluirInativas, bool incluirWorkflows)
        {
            List<IAFerramentaDefinicao> defaults = DefaultsCodigo();
            cls_IA_Repositorio repositorio = new cls_IA_Repositorio();

            // Semeia no banco as ferramentas do codigo que ainda nao existem (idempotente, por nome).
            List<IAFerramentaDefinicao> doBanco = repositorio.ListarFerramentas(true);
            HashSet<string> nomesBanco = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (IAFerramentaDefinicao f in doBanco)
            {
                nomesBanco.Add(f.Nome ?? string.Empty);
            }

            bool inseriu = false;
            foreach (IAFerramentaDefinicao def in defaults)
            {
                if (!nomesBanco.Contains(def.Nome ?? string.Empty))
                {
                    try { repositorio.InserirFerramentaSeAusente(def); }
                    catch { /* corrida de seed entre requisicoes: a linha ja existe, segue */ }
                    inseriu = true;
                }
            }

            if (inseriu)
            {
                doBanco = repositorio.ListarFerramentas(true);
            }

            // Mapa nome -> default para reconciliar o schema (se o schema do banco estiver invalido/vazio).
            Dictionary<string, IAFerramentaDefinicao> mapaDefault = new Dictionary<string, IAFerramentaDefinicao>(StringComparer.OrdinalIgnoreCase);
            foreach (IAFerramentaDefinicao def in defaults)
            {
                mapaDefault[def.Nome ?? string.Empty] = def;
            }

            List<IAFerramentaDefinicao> resultado = new List<IAFerramentaDefinicao>();
            foreach (IAFerramentaDefinicao f in doBanco)
            {
                if (!incluirInativas && !f.Ativo)
                {
                    continue;
                }

                // Ferramenta interna (definida no codigo): o schema e o CONTRATO do executor, entao o
                // codigo e autoritativo. Sem isso, uma mudanca de schema no codigo nao chega ao modelo
                // num ambiente ja semeado (o schema do banco, gravado no seed, fica velho). Ferramenta
                // custom/generica mantem o schema configurado no banco.
                if (mapaDefault.ContainsKey(f.Nome ?? string.Empty))
                {
                    f.SchemaParametros = mapaDefault[f.Nome ?? string.Empty].SchemaParametros;
                }

                // Sem schema valido a ferramenta nao pode ser oferecida ao modelo; some do runtime, mas
                // continua visivel para o admin (para diagnostico) quando incluirInativas.
                if (f.SchemaParametros == null && !incluirInativas)
                {
                    continue;
                }

                resultado.Add(f);
            }

            if (incluirWorkflows)
            {
                AdicionarWorkflowsAoCatalogo(resultado, repositorio);
            }

            return resultado;
        }

        private static void AdicionarWorkflowsAoCatalogo(List<IAFerramentaDefinicao> catalogo, cls_IA_Repositorio repositorio)
        {
            HashSet<string> nomes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (IAFerramentaDefinicao f in catalogo)
            {
                nomes.Add(f.Nome ?? string.Empty);
            }

            foreach (IAWorkflow workflow in repositorio.ListarWorkflows(false))
            {
                if (workflow == null || !workflow.Ativo || string.IsNullOrWhiteSpace(workflow.Nome))
                {
                    continue;
                }

                if (nomes.Contains(workflow.Nome))
                {
                    cls_IA_Auditoria.Registrar(0, "WORKFLOW_IGNORADO_CATALOGO", "ALERTA", new
                    {
                        idWorkflowIA = workflow.IdWorkflowIA,
                        workflow = workflow.Nome,
                        motivo = "nome ja usado por uma ferramenta"
                    });
                    continue;
                }

                JObject schema = SchemaWorkflowParaChat(workflow.SchemaParametrosJson, workflow.GrafoJson, catalogo);
                string escopo = WorkflowRequerPermissaoExecutarAcoes(workflow.GrafoJson, catalogo)
                    ? "WRITE"
                    : (string.IsNullOrWhiteSpace(workflow.Escopo) ? "READ" : workflow.Escopo);
                catalogo.Add(new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 0,
                    IdWorkflowIA = workflow.IdWorkflowIA,
                    Nome = workflow.Nome,
                    Descricao = DescricaoWorkflowParaChat(workflow, schema),
                    Modulo = "Workflows",
                    IdRecursoNecessario = workflow.IdRecursoNecessario,
                    MaxRegistros = 1,
                    Escopo = escopo,
                    RequerConfirmacao = false,
                    Ativo = workflow.Ativo,
                    SchemaParametros = schema,
                    GrafoJson = workflow.GrafoJson
                });
                nomes.Add(workflow.Nome);
            }
        }

        private static bool WorkflowRequerPermissaoExecutarAcoes(string grafoJson, List<IAFerramentaDefinicao> ferramentasBase)
        {
            try
            {
                JObject grafo = JObject.Parse(grafoJson ?? "{}");
                foreach (JToken t in (grafo["nos"] as JArray) ?? new JArray())
                {
                    JObject no = t as JObject;
                    if (no == null || !string.Equals(Convert.ToString(no["tipo"]), "ferramenta", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string nome = Convert.ToString(no["ferramenta"]);
                    foreach (IAFerramentaDefinicao def in ferramentasBase)
                    {
                        if (def.EhWorkflow())
                        {
                            continue;
                        }

                        if (string.Equals(def.Nome, nome, StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(def.Escopo, "WRITE", StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        private static JObject SchemaWorkflow(string schemaJson)
        {
            try
            {
                JObject schema = string.IsNullOrWhiteSpace(schemaJson) ? null : JObject.Parse(schemaJson);
                if (schema != null)
                {
                    if (schema["type"] == null) schema["type"] = "object";
                    if (schema["properties"] == null) schema["properties"] = new JObject();
                    if (schema["required"] == null) schema["required"] = new JArray();
                    if (schema["additionalProperties"] == null) schema["additionalProperties"] = false;
                    return schema;
                }
            }
            catch
            {
            }

            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject() },
                { "required", new JArray() },
                { "additionalProperties", false }
            };
        }

        private static List<IAFerramentaDefinicao> DefaultsCodigo()
        {
            return new List<IAFerramentaDefinicao>
            {
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 1,
                    Nome = "sistema_buscar_recurso_menu",
                    Descricao = "Busca telas e recursos do menu que o usuario autenticado tem permissao para acessar.",
                    Modulo = "Sistema",
                    IdRecursoNecessario = Permissao.IA.Consultar,
                    MaxRegistros = 10,
                    SchemaParametros = SchemaPesquisa("Texto para localizar no nome do recurso ou tela.")
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 2,
                    Nome = "pedidos_consultar_resumo",
                    Descricao = "Consulta os dados de um pedido: cliente, referencia, fluxo, status, departamento, vendedor, forma de envio, datas (pedido, previsao de entrega), controle TT e numero do pedido do cliente. Aceita o numero do pedido exibido na tela, o controle TT ou o id interno. Nao retorna anexos.",
                    Modulo = "Pedidos",
                    IdRecursoNecessario = Permissao.Pedidos.Consultar,
                    MaxRegistros = 1,
                    SchemaParametros = new JObject
                    {
                        { "type", "object" },
                        { "properties", new JObject
                            {
                                { "idPedido", new JObject
                                    {
                                        { "type", "integer" },
                                        { "description", "Numero do pedido exibido na tela, controle TT ou id interno." },
                                        { "minimum", 1 }
                                    }
                                }
                            }
                        },
                        { "required", new JArray("idPedido") },
                        { "additionalProperties", false }
                    }
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 3,
                    Nome = "produtos_consultar",
                    Descricao = "Pesquisa produtos ativos pelo codigo, descricao ou NCM e/ou filtrando por familia, tipo e grupo (por nome). Retorna dados cadastrais incluindo familia, grupo, tipo, subtipo, categoria, NCM e unidade quando disponiveis.",
                    Modulo = "Produtos",
                    IdRecursoNecessario = Permissao.Produtos.Consultar,
                    MaxRegistros = 10,
                    SchemaParametros = new JObject
                    {
                        { "type", "object" },
                        { "properties", new JObject
                            {
                                { "termo", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Codigo, parte da descricao ou NCM do produto. Use string vazia para listar apenas pelos filtros (familia/tipo/grupo)." },
                                        { "maxLength", 120 }
                                    }
                                },
                                { "familia", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Nome da familia do produto para filtrar. Use string vazia para nao filtrar por familia." },
                                        { "maxLength", 120 }
                                    }
                                },
                                { "tipo", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Nome do tipo de produto para filtrar. Use string vazia para nao filtrar por tipo." },
                                        { "maxLength", 120 }
                                    }
                                },
                                { "grupo", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Nome do grupo do produto para filtrar. Use string vazia para nao filtrar por grupo." },
                                        { "maxLength", 120 }
                                    }
                                },
                                { "limite", SchemaLimite() }
                            }
                        },
                        { "required", new JArray("termo", "familia", "tipo", "grupo", "limite") },
                        { "additionalProperties", false }
                    }
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 4,
                    Nome = "parceiros_consultar",
                    Descricao = "Pesquisa clientes, fornecedores e parceiros por nome ou razao social, com documento mascarado.",
                    Modulo = "Parceiros",
                    IdRecursoNecessario = Permissao.Parceiros.Consultar,
                    MaxRegistros = 10,
                    SchemaParametros = new JObject
                    {
                        { "type", "object" },
                        { "properties", new JObject
                            {
                                { "termo", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Nome, fantasia ou razao social para pesquisa." },
                                        { "minLength", 2 },
                                        { "maxLength", 120 }
                                    }
                                },
                                { "idTipo", new JObject
                                    {
                                        { "type", "integer" },
                                        { "description", "Tipo de parceiro. Use 0 para todos." },
                                        { "minimum", 0 }
                                    }
                                },
                                { "limite", SchemaLimite() }
                            }
                        },
                        { "required", new JArray("termo", "idTipo", "limite") },
                        { "additionalProperties", false }
                    }
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 5,
                    Nome = "mensagens_consultar",
                    Descricao = "Consulta mensagens, avisos ou e-mails do usuario autenticado por assunto.",
                    Modulo = "Mensagens",
                    IdRecursoNecessario = Permissao.Mensagens.Consultar,
                    MaxRegistros = 10,
                    SchemaParametros = new JObject
                    {
                        { "type", "object" },
                        { "properties", new JObject
                            {
                                { "termo", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Texto para buscar no assunto. Use string vazia para listar recentes." },
                                        { "maxLength", 120 }
                                    }
                                },
                                { "categoria", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Categoria da consulta." },
                                        { "enum", new JArray("TODOS", "AVISO", "MENSAGEM", "EMAIL") }
                                    }
                                },
                                { "limite", SchemaLimite() }
                            }
                        },
                        { "required", new JArray("termo", "categoria", "limite") },
                        { "additionalProperties", false }
                    }
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 6,
                    Nome = "arquivos_buscar_trechos",
                    Descricao = "Busca trechos do markdown sanitizado dos arquivos anexados selecionados para a mensagem atual. Use apenas para responder perguntas sobre esses anexos.",
                    Modulo = "Arquivos IA",
                    IdRecursoNecessario = Permissao.IA.Consultar,
                    MaxRegistros = 8,
                    SchemaParametros = new JObject
                    {
                        { "type", "object" },
                        { "properties", new JObject
                            {
                                { "idsArquivos", new JObject
                                    {
                                        { "type", "array" },
                                        { "description", "IDs dos arquivos anexados nesta mensagem." },
                                        { "items", new JObject
                                            {
                                                { "type", "integer" },
                                                { "minimum", 1 }
                                            }
                                        },
                                        { "minItems", 1 },
                                        { "maxItems", 3 }
                                    }
                                },
                                { "termo", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Termo para localizar no markdown. Use string vazia para ler os primeiros trechos." },
                                        { "maxLength", 120 }
                                    }
                                },
                                { "limite", new JObject
                                    {
                                        { "type", "integer" },
                                        { "description", "Quantidade maxima de trechos. Use entre 1 e 8." },
                                        { "minimum", 1 },
                                        { "maximum", 8 }
                                    }
                                }
                            }
                        },
                        { "required", new JArray("idsArquivos", "termo", "limite") },
                        { "additionalProperties", false }
                    }
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 16,
                    Nome = "conhecimento_buscar",
                    Descricao = "Busca na base de conhecimento interna (documentos de vendas/processos/politicas e FAQ). Use para responder perguntas gerais mesmo sem arquivo anexado. Sempre cite a fonte retornada e nao invente.",
                    Modulo = "Base de Conhecimento",
                    IdRecursoNecessario = Permissao.IA.Consultar,
                    MaxRegistros = 8,
                    SchemaParametros = new JObject
                    {
                        { "type", "object" },
                        { "properties", new JObject
                            {
                                { "termo", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Palavras-chave da pergunta para buscar na base." },
                                        { "maxLength", 200 }
                                    }
                                },
                                { "categoria", new JObject
                                    {
                                        { "type", new JArray("string", "null") },
                                        { "description", "Categoria opcional para restringir (ex.: vendas, produtos). Use null para buscar em tudo." },
                                        { "maxLength", 100 }
                                    }
                                },
                                { "limite", new JObject
                                    {
                                        { "type", "integer" },
                                        { "description", "Quantidade maxima de trechos. Use entre 1 e 8." },
                                        { "minimum", 1 },
                                        { "maximum", 8 }
                                    }
                                }
                            }
                        },
                        { "required", new JArray("termo", "categoria", "limite") },
                        { "additionalProperties", false }
                    }
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 7,
                    Nome = "sistema_buscar_informacao",
                    Descricao = "Busca universal controlada por termo em entidades permitidas ao usuario: menu, produtos, parceiros, mensagens e pedido por ID. Use para perguntas amplas quando o usuario nao indicar claramente uma ferramenta especifica.",
                    Modulo = "Sistema",
                    IdRecursoNecessario = Permissao.IA.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = new JObject
                    {
                        { "type", "object" },
                        { "properties", new JObject
                            {
                                { "termo", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Texto, codigo, nome, assunto ou ID para buscar no sistema." },
                                        { "minLength", 2 },
                                        { "maxLength", 120 }
                                    }
                                },
                                { "tipo", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Escopo preferido da busca. Use TODOS quando estiver em duvida." },
                                        { "enum", new JArray("TODOS", "MENU", "PRODUTO", "PARCEIRO", "MENSAGEM", "PEDIDO") }
                                    }
                                },
                                { "limite", new JObject
                                    {
                                        { "type", "integer" },
                                        { "description", "Quantidade maxima total de registros. Use entre 1 e 20." },
                                        { "minimum", 1 },
                                        { "maximum", 20 }
                                    }
                                }
                            }
                        },
                        { "required", new JArray("termo", "tipo", "limite") },
                        { "additionalProperties", false }
                    }
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 8,
                    Nome = "pedidos_consultar_historico",
                    Descricao = "Consulta os eventos do historico (log) de um pedido: data, usuario, tipo e acao registrada. Aceita numero do pedido, controle TT ou id interno. Combine com pedidos_consultar_resumo para montar um resumo completo do pedido.",
                    Modulo = "Pedidos",
                    IdRecursoNecessario = Permissao.Pedidos.Consultar,
                    MaxRegistros = 15,
                    SchemaParametros = new JObject
                    {
                        { "type", "object" },
                        { "properties", new JObject
                            {
                                { "idPedido", new JObject
                                    {
                                        { "type", "integer" },
                                        { "description", "Numero do pedido exibido na tela, controle TT ou id interno." },
                                        { "minimum", 1 }
                                    }
                                },
                                { "limite", new JObject
                                    {
                                        { "type", "integer" },
                                        { "description", "Quantidade maxima de eventos. Use entre 1 e 15." },
                                        { "minimum", 1 },
                                        { "maximum", 15 }
                                    }
                                }
                            }
                        },
                        { "required", new JArray("idPedido", "limite") },
                        { "additionalProperties", false }
                    }
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 9,
                    Nome = "pedidos_adicionar_observacao",
                    Descricao = "Propoe adicionar uma observacao ao historico de um pedido. Aceita numero do pedido, controle TT ou id interno. A acao NAO e executada imediatamente: cria uma pendencia que o usuario precisa confirmar pelos botoes exibidos no chat antes de a observacao ser gravada.",
                    Modulo = "Pedidos",
                    IdRecursoNecessario = Permissao.Pedidos.Consultar,
                    MaxRegistros = 1,
                    Escopo = "WRITE",
                    RequerConfirmacao = true,
                    SchemaParametros = new JObject
                    {
                        { "type", "object" },
                        { "properties", new JObject
                            {
                                { "idPedido", new JObject
                                    {
                                        { "type", "integer" },
                                        { "description", "Numero do pedido exibido na tela, controle TT ou id interno." },
                                        { "minimum", 1 }
                                    }
                                },
                                { "sObservacao", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Texto da observacao a registrar no historico do pedido." },
                                        { "minLength", 5 },
                                        { "maxLength", 500 }
                                    }
                                }
                            }
                        },
                        { "required", new JArray("idPedido", "sObservacao") },
                        { "additionalProperties", false }
                    }
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 10,
                    Nome = "pedidos_listar_arquivos",
                    Descricao = "Lista os anexos ativos de um pedido (nome, tipo, categoria, data e usuario), sem retornar o conteudo binario. Aceita numero do pedido, controle TT ou id interno.",
                    Modulo = "Pedidos",
                    IdRecursoNecessario = Permissao.Pedidos.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = new JObject
                    {
                        { "type", "object" },
                        { "properties", new JObject
                            {
                                { "idPedido", new JObject
                                    {
                                        { "type", "integer" },
                                        { "description", "Numero do pedido exibido na tela, controle TT ou id interno." },
                                        { "minimum", 1 }
                                    }
                                },
                                { "limite", new JObject
                                    {
                                        { "type", "integer" },
                                        { "description", "Quantidade maxima de anexos. Use entre 1 e 20." },
                                        { "minimum", 1 },
                                        { "maximum", 20 }
                                    }
                                }
                            }
                        },
                        { "required", new JArray("idPedido", "limite") },
                        { "additionalProperties", false }
                    }
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 11,
                    Nome = "pedidos_pesquisar",
                    Descricao = "Pesquisa uma LISTA de pedidos por criterios: nome do cliente, texto (referencia, controle TT ou numero do pedido de compras), periodo de datas e tipo de pedido. Retorna varios pedidos com cliente, fluxo, status, vendedor, forma de envio, datas e previsao de entrega. Use quando o usuario quer encontrar/listar pedidos por um criterio, nao um pedido especifico ja identificado.",
                    Modulo = "Pedidos",
                    IdRecursoNecessario = Permissao.Pedidos.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = new JObject
                    {
                        { "type", "object" },
                        { "properties", new JObject
                            {
                                { "cliente", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Nome, razao social ou fantasia do cliente para filtrar os pedidos. Use string vazia para nao filtrar por cliente." },
                                        { "maxLength", 120 }
                                    }
                                },
                                { "termo", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Texto para pesquisar em referencia, controle TT ou numero do pedido de compras (NAO busca por nome de cliente - use o parametro cliente para isso). Use string vazia para nao filtrar por texto." },
                                        { "maxLength", 120 }
                                    }
                                },
                                { "tipo", new JObject
                                    {
                                        { "type", "integer" },
                                        { "description", "Tipo do pedido: 2 = pedido de venda (padrao), 3 = importacao, 6 = exportacao, 7 = compras. Use 2 quando nao especificado." },
                                        { "enum", new JArray(1, 2, 3, 6, 7) }
                                    }
                                },
                                { "dataInicio", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Data inicial do periodo no formato dd/MM/yyyy. Use string vazia para nao filtrar por data." },
                                        { "maxLength", 10 }
                                    }
                                },
                                { "dataFim", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Data final do periodo no formato dd/MM/yyyy. Use string vazia para nao filtrar por data." },
                                        { "maxLength", 10 }
                                    }
                                },
                                { "limite", new JObject
                                    {
                                        { "type", "integer" },
                                        { "description", "Quantidade maxima de pedidos. Use entre 1 e 20." },
                                        { "minimum", 1 },
                                        { "maximum", 20 }
                                    }
                                }
                            }
                        },
                        { "required", new JArray("cliente", "termo", "tipo", "dataInicio", "dataFim", "limite") },
                        { "additionalProperties", false }
                    }
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 12,
                    Nome = "mensagens_criar_rascunho",
                    Descricao = "Propoe SALVAR um rascunho de mensagem (assunto e corpo, com destinatario pretendido opcional). NAO envia a mensagem e NAO cria mensagem oficial: apenas guarda um rascunho para o usuario revisar e usar depois. A acao NAO e executada imediatamente: cria uma pendencia que o usuario precisa confirmar pelos botoes exibidos no chat. Mostre o texto completo do rascunho antes de propor.",
                    Modulo = "Mensagens",
                    IdRecursoNecessario = Permissao.Mensagens.Consultar,
                    MaxRegistros = 1,
                    Escopo = "WRITE",
                    RequerConfirmacao = true,
                    SchemaParametros = new JObject
                    {
                        { "type", "object" },
                        { "properties", new JObject
                            {
                                { "assunto", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Assunto/titulo curto do rascunho." },
                                        { "minLength", 3 },
                                        { "maxLength", 200 }
                                    }
                                },
                                { "corpo", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Texto completo do rascunho da mensagem." },
                                        { "minLength", 10 },
                                        { "maxLength", 4000 }
                                    }
                                },
                                { "destino", new JObject
                                    {
                                        { "type", "string" },
                                        { "description", "Destinatario pretendido em texto livre (ex.: nome do departamento ou parceiro). Use string vazia se nao souber." },
                                        { "maxLength", 200 }
                                    }
                                }
                            }
                        },
                        { "required", new JArray("assunto", "corpo", "destino") },
                        { "additionalProperties", false }
                    }
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 17,
                    Nome = "workflows_listar_disponiveis",
                    Descricao = "Lista os workflows ativos que o usuario autenticado pode executar pelo chat, com nome interno, descricao, tipo e parametros esperados. Use quando o usuario perguntar quais workflows, automacoes ou processos automatizados estao disponiveis.",
                    Modulo = "Workflows",
                    IdRecursoNecessario = Permissao.IA.Consultar,
                    MaxRegistros = 50,
                    Escopo = "READ",
                    SchemaParametros = new JObject
                    {
                        { "type", "object" },
                        { "properties", new JObject() },
                        { "required", new JArray() },
                        { "additionalProperties", false }
                    }
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 18,
                    Nome = "orcamento_tipos_listar",
                    Descricao = "Lista tipos de orçamento comercial por nome. Use antes de criar ou analisar um orçamento para identificar idTipoOrcamento, descrição e situação.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaOrcamentoTiposListar()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 19,
                    Nome = "orcamento_tipo_detalhar",
                    Descricao = "Detalha um tipo de orçamento, incluindo fluxos, tipos de serviço/produto e escopos vinculados. Use para montar os próximos passos do orçamento.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 30,
                    SchemaParametros = SchemaIdInteiro("idTipoOrcamento", "ID do tipo de orçamento.", 1)
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 20,
                    Nome = "orcamento_escopos_listar",
                    Descricao = "Lista perguntas/opções de escopo para um tipo de orçamento e, opcionalmente, as respostas já salvas em um orçamento existente.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 50,
                    SchemaParametros = SchemaOrcamentoEscopos()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 21,
                    Nome = "orcamento_servicos_recursos_listar",
                    Descricao = "Lista serviços e recursos disponíveis para um tipo de orçamento, considerando uma tabela de preço. Use para sugerir itens de serviço/recurso.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 30,
                    SchemaParametros = SchemaOrcamentoServicosRecursos()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 32,
                    Nome = "orcamento_clientes_buscar",
                    Descricao = "Busca clientes/parceiros por razão social, nome fantasia, CPF/CNPJ ou IE/RG para resolver o idCliente antes de criar um orçamento.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaOrcamentoClientesBuscar()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 38,
                    Nome = "clientes_resolver_para_orcamento",
                    Descricao = "Resolve um cliente por nome, fantasia, CPF/CNPJ ou IE/RG para o workflow de orçamento. Retorna se encontrou, melhorResultado, registros candidatos e se precisa de escolha do usuário.",
                    Modulo = "Parceiros / Orçamento",
                    IdRecursoNecessario = Permissao.Parceiros.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaClientesResolverParaOrcamento()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 22,
                    Nome = "orcamento_cliente_detalhar",
                    Descricao = "Consulta dados resumidos de um cliente/parceiro para uso em orçamento, incluindo contatos e endereços. Documento é mascarado.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaIdInteiro("idCliente", "ID do cliente/parceiro.", 1)
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 39,
                    Nome = "clientes_criar_para_orcamento",
                    Descricao = "Propõe criar um cliente/parceiro mínimo para orçamento, com contato principal e endereços fiscal e entrega. A ação exige confirmação do usuário antes de salvar.",
                    Modulo = "Parceiros / Orçamento",
                    IdRecursoNecessario = Permissao.Parceiros.Incluir,
                    MaxRegistros = 1,
                    Escopo = "WRITE",
                    RequerConfirmacao = true,
                    SchemaParametros = SchemaClientesCriarParaOrcamento()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 47,
                    Nome = "clientes_completar_para_orcamento",
                    Descricao = "Completa contato e endereços fiscal/entrega ausentes de um cliente existente para permitir a criação do orçamento. Não duplica dados já cadastrados e exige confirmação do usuário.",
                    Modulo = "Parceiros / Orçamento",
                    IdRecursoNecessario = Permissao.Parceiros.Alterar,
                    MaxRegistros = 1,
                    Escopo = "WRITE",
                    RequerConfirmacao = true,
                    SchemaParametros = SchemaClientesCompletarParaOrcamento()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 44,
                    Nome = "cep_consultar",
                    Descricao = "Consulta endereço por CEP para preencher automaticamente logradouro, bairro, cidade, UF e idCidade em cadastros de cliente do workflow.",
                    Modulo = "Parceiros / Endereço",
                    IdRecursoNecessario = Permissao.Parceiros.Consultar,
                    MaxRegistros = 1,
                    SchemaParametros = SchemaCepConsultar()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 33,
                    Nome = "orcamento_empresas_buscar",
                    Descricao = "Busca empresas por nome, nome reduzido, parceiro vinculado ou CNPJ para resolver o idEmpresa usado no orçamento.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaOrcamentoTermoLimite("Nome da empresa, nome reduzido, parceiro ou CNPJ.", 2, 20)
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 36,
                    Nome = "orcamento_vendedores_buscar",
                    Descricao = "Ferramenta legada de compatibilidade: busca usuários por nome para resolver o idVendedor técnico usado no orçamento. Devolve em idVendedor o id do USUÁRIO responsável, que é o que o orçamento guarda; o código comercial do vendedor vem à parte, em idVendedorComercial.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaOrcamentoTermoLimite("Nome do usuário do sistema.", 2, 20)
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 50,
                    Nome = "orcamento_usuarios_resolver",
                    Descricao = "Resolve o usuário responsável pelo orçamento. Permite usar o usuário autenticado ou apresenta os demais usuários do sistema para escolha.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 100,
                    SchemaParametros = SchemaOrcamentoUsuariosResolver()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 23,
                    Nome = "orcamento_empresa_detalhar",
                    Descricao = "Consulta empresa usada no orçamento por idEmpresa ou idParceiro, com dados fiscais resumidos e impostos cadastrados.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaOrcamentoEmpresaDetalhar()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 34,
                    Nome = "orcamento_tabelas_preco_buscar",
                    Descricao = "Busca tabelas de preço ativas por nome ou observação para resolver o idTabelaPreco antes de criar orçamento.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaOrcamentoTabelasPrecoBuscar()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 45,
                    Nome = "orcamento_tabelas_preco_listar",
                    Descricao = "Lista tabelas de preço ativas, sem exigir termo de busca, para o workflow sugerir ou selecionar uma tabela padrão.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaOrcamentoTabelasPrecoListar()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 37,
                    Nome = "orcamento_tipos_envio_buscar",
                    Descricao = "Busca formas/tipos de envio por nome para resolver o idTipoEnvio usado no orçamento.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaOrcamentoTermoLimite("Nome da forma ou tipo de envio.", 2, 20)
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 24,
                    Nome = "orcamento_tabela_preco_detalhar",
                    Descricao = "Consulta dados de uma tabela de preço para validar vigência, moeda, tipo e configurações antes de montar itens do orçamento.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaIdInteiro("idTabela", "ID da tabela de preço.", 1)
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 25,
                    Nome = "orcamento_itens_disponiveis_listar",
                    Descricao = "Lista produtos/itens liberados em uma tabela de preço, com preço, unidade, grupo, família, tipo, moeda e impostos resumidos.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 30,
                    SchemaParametros = SchemaOrcamentoItensDisponiveis()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 35,
                    Nome = "orcamento_produtos_buscar",
                    Descricao = "Busca produtos, serviços ou recursos por código/descrição, opcionalmente dentro de uma tabela de preço, para resolver idProduto e preço antes de adicionar item ao orçamento.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 30,
                    SchemaParametros = SchemaOrcamentoProdutosBuscar()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 46,
                    Nome = "orcamento_produto_preco_resolver",
                    Descricao = "Resolve o preço de um produto para orçamento usando idTabelaPreco, nome de tabela do item ou a tabela do orçamento; se não encontrar, tenta o preço padrão disponível no cadastro.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 5,
                    SchemaParametros = SchemaOrcamentoProdutoPrecoResolver()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 48,
                    Nome = "orcamento_produtos_sugestoes_listar",
                    Descricao = "Lista produtos complementares cadastrados para um produto principal. As sugestões são opcionais e devem ser apresentadas ao usuário para seleção múltipla; cada item aceito ainda precisa ter preço resolvido e ser adicionado separadamente.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaOrcamentoProdutosSugestoes()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 26,
                    Nome = "orcamento_produto_codigo_consultar",
                    Descricao = "Consulta um produto por código no contexto de orçamento, validando cliente e tabela de preço quando informados.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 5,
                    SchemaParametros = SchemaOrcamentoProdutoCodigo()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 27,
                    Nome = "orcamento_produto_composicao_consultar",
                    Descricao = "Consulta a composição de um produto/serviço para orçamento. Use quando o item principal tiver componentes ou sistemas associados.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 30,
                    SchemaParametros = SchemaIdComLimite("idProduto", "ID do produto/serviço principal.", 1, 30)
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 28,
                    Nome = "orcamento_condicao_pagamento_detalhar",
                    Descricao = "Consulta condições de pagamento e parcelas. Use para validar idCondicaoPagamento antes de criar um orçamento.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaOrcamentoCondicaoPagamento()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 29,
                    Nome = "orcamento_criar_cabecalho",
                    Descricao = "Propõe criar o cabeçalho/rascunho inicial de um orçamento comercial. Não grava itens, produtos, serviços ou escopos respondidos. A ação exige confirmação do usuário antes de salvar.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Incluir,
                    MaxRegistros = 1,
                    Escopo = "WRITE",
                    RequerConfirmacao = true,
                    SchemaParametros = SchemaOrcamentoCriarCabecalho()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 49,
                    Nome = "orcamento_duplicar",
                    Descricao = "Propõe duplicar um orçamento existente, gerando novo número, referência e datas sem copiar vínculos de CRM, pedido ou cotação. A cópia do cabeçalho e dos itens é transacional e exige confirmação.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Incluir,
                    MaxRegistros = 1,
                    Escopo = "WRITE",
                    RequerConfirmacao = true,
                    SchemaParametros = SchemaOrcamentoDuplicar()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 30,
                    Nome = "orcamento_adicionar_item",
                    Descricao = "Propõe adicionar um produto, serviço ou recurso raiz em um orçamento existente e recalcular os totais. Use após criar o cabeçalho e validar o item na tabela de preço. A ação exige confirmação do usuário antes de salvar.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Alterar,
                    MaxRegistros = 1,
                    Escopo = "WRITE",
                    RequerConfirmacao = true,
                    SchemaParametros = SchemaOrcamentoAdicionarItem()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 31,
                    Nome = "orcamento_salvar_resposta_escopo",
                    Descricao = "Propõe salvar ou atualizar uma resposta de escopo/matriz de perguntas de um orçamento existente. A ação exige confirmação do usuário antes de salvar.",
                    Modulo = "Comercial / Orçamento",
                    IdRecursoNecessario = Permissao.Comercial.Orcamento.Alterar,
                    MaxRegistros = 1,
                    Escopo = "WRITE",
                    RequerConfirmacao = true,
                    SchemaParametros = SchemaOrcamentoSalvarRespostaEscopo()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 40,
                    Nome = "crm_negocios_buscar",
                    Descricao = "Consulta negócios CRM por cliente, vendedor, tipo de cotação, status, referência ou controle, retornando dados úteis para vincular ao orçamento.",
                    Modulo = "Comercial / CRM",
                    IdRecursoNecessario = Permissao.Comercial.CRM.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaCrmNegociosBuscar()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 41,
                    Nome = "crm_negocio_detalhar",
                    Descricao = "Detalha um negócio CRM, incluindo dados principais, histórico e follow-ups.",
                    Modulo = "Comercial / CRM",
                    IdRecursoNecessario = Permissao.Comercial.CRM.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaIdComLimite("idRegistroCRM", "ID do negócio CRM.", 1, 20)
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 42,
                    Nome = "crm_negocio_criar_para_orcamento",
                    Descricao = "Propõe criar um negócio CRM vinculado ao orçamento já gerado. A ação exige confirmação do usuário antes de salvar.",
                    Modulo = "Comercial / CRM",
                    IdRecursoNecessario = Permissao.Comercial.CRM.Consultar,
                    MaxRegistros = 1,
                    Escopo = "WRITE",
                    RequerConfirmacao = true,
                    SchemaParametros = SchemaCrmNegocioCriarParaOrcamento()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 43,
                    Nome = "crm_followup_criar",
                    Descricao = "Propõe criar um follow-up simples em um negócio CRM. A ação exige confirmação do usuário antes de salvar.",
                    Modulo = "Comercial / CRM",
                    IdRecursoNecessario = Permissao.Comercial.CRM.Consultar,
                    MaxRegistros = 1,
                    Escopo = "WRITE",
                    RequerConfirmacao = true,
                    SchemaParametros = SchemaCrmFollowupCriar()
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 13,
                    Nome = "rrhh_consultar_colaborador_basico",
                    Descricao = "Consulta dados BASICOS de colaboradores (nome, cargo, departamento, setor/GHE, funcao, supervisor e situacao) buscando por nome. NAO retorna salario, CPF, RG, endereco, telefone, data de nascimento nem dados bancarios. Respeita as permissoes do usuario: so mostra colaboradores dos departamentos/cargos que ele pode visualizar.",
                    Modulo = "RRHH",
                    IdRecursoNecessario = Permissao.RRHH.ConsultarDadosColaborador,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaPesquisa("Nome do colaborador (ou parte dele). Minimo 2 caracteres.")
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 14,
                    Nome = "financeiro_consultar_contas_receber",
                    Descricao = "Consulta titulos de CONTAS A RECEBER por cliente, codigo do titulo ou numero do documento, e/ou pelo status. Use status='Em Atraso' para listar os vencidos e nao pagos (pode ser sem termo). Retorna cliente, emissao, vencimento, valor, saldo, valor recebido, status e empresa. NAO retorna dados bancarios nem CPF/CNPJ. Respeita a visibilidade do usuario (quem nao tem 'Visualizar Tudo' so ve os proprios titulos).",
                    Modulo = "Financeiro",
                    IdRecursoNecessario = Permissao.Financeiro.ContasReceber.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaPesquisaFinanceiro("Nome/razao social do cliente, codigo do titulo ou numero do documento.")
                },
                new IAFerramentaDefinicao
                {
                    IdFerramentaIA = 15,
                    Nome = "financeiro_consultar_contas_pagar",
                    Descricao = "Consulta titulos de CONTAS A PAGAR por fornecedor/credor, codigo do titulo ou numero do documento, e/ou pelo status. Use status='Em Atraso' para listar os vencidos e nao pagos (pode ser sem termo). Retorna credor, emissao, vencimento, valor, saldo, valor pago, status, categoria, situacao de aprovacao e empresa. NAO retorna dados bancarios. Respeita a visibilidade do usuario (quem nao tem 'Visualizar Tudo' so ve os proprios titulos).",
                    Modulo = "Financeiro",
                    IdRecursoNecessario = Permissao.Financeiro.ContasPagar.Consultar,
                    MaxRegistros = 20,
                    SchemaParametros = SchemaPesquisaFinanceiro("Nome/razao social do fornecedor ou credor, codigo do titulo ou numero do documento.")
                }
            };
        }

        private static JObject SchemaPesquisa(string descricaoTermo)
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "termo", new JObject
                            {
                                { "type", "string" },
                                { "description", descricaoTermo },
                                { "minLength", 2 },
                                { "maxLength", 120 }
                            }
                        },
                        { "limite", SchemaLimite() }
                    }
                },
                { "required", new JArray("termo", "limite") },
                { "additionalProperties", false }
            };
        }

        // Pesquisa financeira (contas a pagar/receber): termo OPCIONAL + filtro de status. Assim da para
        // listar por situacao (ex.: "Em Atraso") sem citar cliente/fornecedor, como faz a tela.
        private static JObject SchemaPesquisaFinanceiro(string descricaoTermo)
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "termo", new JObject
                            {
                                { "type", "string" },
                                { "description", descricaoTermo + " Use string vazia para nao filtrar por termo (ex.: listar apenas por status)." },
                                { "maxLength", 120 }
                            }
                        },
                        { "status", new JObject
                            {
                                { "type", "string" },
                                { "enum", new JArray("", "Em Aberto", "Em Atraso", "Liquidado") },
                                { "description", "Situacao do titulo. 'Em Atraso' = vencido e nao pago; 'Em Aberto' = ainda nao liquidado; 'Liquidado' = pago. Use string vazia para todos." }
                            }
                        },
                        { "limite", new JObject
                            {
                                { "type", "integer" },
                                { "description", "Quantidade maxima de registros. Use entre 1 e 20." },
                                { "minimum", 1 },
                                { "maximum", 20 }
                            }
                        }
                    }
                },
                { "required", new JArray("termo", "status", "limite") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaWorkflowParaChat(string schemaJson, string grafoJson, List<IAFerramentaDefinicao> catalogo)
        {
            JObject schema = SchemaWorkflow(schemaJson);
            EnriquecerListasWorkflow(schema, grafoJson, catalogo);
            JObject propriedades = schema["properties"] as JObject ?? new JObject();
            JArray required = new JArray();

            foreach (JProperty propriedade in propriedades.Properties())
            {
                JObject meta = propriedade.Value as JObject;
                if (meta == null) continue;
                JToken tipo = meta["type"];
                JArray tipos = tipo as JArray;
                if (tipos == null)
                {
                    tipos = new JArray(string.IsNullOrWhiteSpace(Convert.ToString(tipo)) ? "string" : Convert.ToString(tipo));
                }
                bool possuiNull = false;
                foreach (JToken item in tipos)
                {
                    if (string.Equals(Convert.ToString(item), "null", StringComparison.OrdinalIgnoreCase)) possuiNull = true;
                }
                if (!possuiNull) tipos.Add("null");
                meta["type"] = tipos;
                JArray valoresEnum = meta["enum"] as JArray;
                if (valoresEnum != null)
                {
                    bool enumPossuiNull = false;
                    foreach (JToken item in valoresEnum)
                    {
                        if (item == null || item.Type == JTokenType.Null) enumPossuiNull = true;
                    }
                    if (!enumPossuiNull) valoresEnum.Add(JValue.CreateNull());
                }
                required.Add(propriedade.Name);
            }

            // Em strict mode todos os campos precisam constar em required. O valor null representa
            // dado ainda não informado e será coletado pela execução persistente.
            schema["required"] = required;
            return schema;
        }

        private static string DescricaoWorkflowParaChat(IAWorkflow workflow, JObject schema)
        {
            string descricao = workflow != null && !string.IsNullOrWhiteSpace(workflow.Descricao)
                ? workflow.Descricao.Trim()
                : "Executa o workflow \"" + (workflow != null ? workflow.Nome : string.Empty) + "\".";

            JObject propriedades = schema != null ? schema["properties"] as JObject : null;
            JObject produtos = propriedades != null ? propriedades["produtos"] as JObject : null;
            JObject itens = produtos != null ? produtos["items"] as JObject : null;
            if (itens != null && string.Equals(Convert.ToString(itens["type"]), "object", StringComparison.OrdinalIgnoreCase))
            {
                descricao += " A entrada produtos deve ser uma lista de objetos; cada item deve seguir exatamente o schema informado, usando nome ou código do produto, nunca ID interno.";
            }

            return descricao + " Inicie o workflow diretamente com os dados de negócio informados pelo usuário. O próprio workflow executa suas buscas, resolve nomes e códigos e solicita escolhas quando houver ambiguidade; não antecipe nem repita essas etapas com outras ferramentas.";
        }

        private static void EnriquecerListasWorkflow(JObject schema, string grafoJson, List<IAFerramentaDefinicao> catalogo)
        {
            try
            {
                JObject grafo = JObject.Parse(string.IsNullOrWhiteSpace(grafoJson) ? "{}" : grafoJson);
                JObject propriedades = schema["properties"] as JObject ?? new JObject();
                JArray nos = grafo["nos"] as JArray ?? new JArray();

                foreach (JToken tokenLoop in nos)
                {
                    JObject loop = tokenLoop as JObject;
                    if (loop == null || !string.Equals(Convert.ToString(loop["tipo"]), "loop", StringComparison.OrdinalIgnoreCase)) continue;
                    string modo = Convert.ToString(loop["modoLoop"]);
                    if (string.IsNullOrWhiteSpace(modo)) modo = Convert.ToString(loop["modo"]);
                    if (!string.Equals(modo, "lista", StringComparison.OrdinalIgnoreCase)) continue;
                    if (!string.Equals(Convert.ToString(loop["listaOrigem"]), "entrada", StringComparison.OrdinalIgnoreCase)) continue;

                    string nomeEntrada = RaizEntradaLista(Convert.ToString(loop["listaValor"]));
                    JObject metaEntrada = propriedades[nomeEntrada] as JObject;
                    if (metaEntrada == null || !TipoSchemaContem(metaEntrada["type"], "array")) continue;

                    string idLoop = Convert.ToString(loop["id"]);
                    JObject propriedadesItem = new JObject();
                    foreach (JToken tokenNo in nos)
                    {
                        JObject no = tokenNo as JObject;
                        if (no == null) continue;
                        IAFerramentaDefinicao ferramentaNo = LocalizarFerramentaCatalogo(catalogo, Convert.ToString(no["ferramenta"]));
                        JObject parametrosFerramenta = ferramentaNo != null && ferramentaNo.SchemaParametros != null
                            ? ferramentaNo.SchemaParametros["properties"] as JObject
                            : null;

                        foreach (JToken tokenMap in (no["entradas"] as JArray) ?? new JArray())
                        {
                            JObject mapa = tokenMap as JObject;
                            if (mapa == null || !string.Equals(Convert.ToString(mapa["origem"]), "passo", StringComparison.OrdinalIgnoreCase)) continue;
                            string campoItem = CampoItemLoop(Convert.ToString(mapa["valor"]), idLoop);
                            if (string.IsNullOrWhiteSpace(campoItem) || propriedadesItem[campoItem] != null) continue;

                            string parametro = Convert.ToString(mapa["param"]);
                            JObject origemMeta = parametrosFerramenta != null ? parametrosFerramenta[parametro] as JObject : null;
                            propriedadesItem[campoItem] = SchemaCampoItemWorkflow(campoItem, origemMeta);
                        }
                    }

                    if (propriedadesItem.Count == 0) continue;
                    JArray obrigatoriosItem = new JArray();
                    foreach (JProperty propriedade in propriedadesItem.Properties()) obrigatoriosItem.Add(propriedade.Name);
                    metaEntrada["items"] = new JObject
                    {
                        { "type", "object" },
                        { "properties", propriedadesItem },
                        { "required", obrigatoriosItem },
                        { "additionalProperties", false }
                    };
                    string campos = string.Join(", ", obrigatoriosItem.ToObject<string[]>());
                    string descricaoAtual = Convert.ToString(metaEntrada["description"]).Trim();
                    metaEntrada["description"] = descricaoAtual + (descricaoAtual.Length > 0 ? " " : string.Empty) +
                        "Formato obrigatório de cada item: { " + campos + " }. Não envie uma lista simples de textos.";
                }
            }
            catch
            {
                // Mantém o schema salvo caso um workflow antigo não permita inferir a estrutura da lista.
            }
        }

        private static JObject SchemaCampoItemWorkflow(string campo, JObject origem)
        {
            JObject schema = origem != null ? (JObject)origem.DeepClone() : new JObject { { "type", "string" } };
            schema.Remove("default");
            if (string.Equals(campo, "produto", StringComparison.OrdinalIgnoreCase))
            {
                schema["type"] = "string";
                schema["description"] = "Nome ou código exato do produto informado pelo usuário. Não use ID interno e não deixe vazio.";
            }
            else if (string.Equals(campo, "quantidade", StringComparison.OrdinalIgnoreCase))
            {
                schema["type"] = new JArray("number", "null");
                schema["description"] = "Quantidade solicitada para este produto. Se o usuário não informar, envie null: o workflow usa 1 e o cartão de confirmação do item mostra a quantidade antes de gravar. Não invente outro valor.";
            }
            else if (string.Equals(campo, "tipoItem", StringComparison.OrdinalIgnoreCase))
            {
                schema["type"] = "string";
                schema["enum"] = new JArray("produto", "servico_recurso");
                schema["description"] = "Use produto para mercadorias; use servico_recurso somente quando o usuário pedir serviço ou recurso.";
            }
            else if (string.Equals(campo, "descontoPercentual", StringComparison.OrdinalIgnoreCase))
            {
                schema["type"] = "number";
                schema["minimum"] = 0;
                schema["maximum"] = 100;
                schema["description"] = "Desconto percentual deste produto. Use 0 quando o usuário não pedir desconto.";
            }
            else if (string.Equals(campo, "tabelaPreco", StringComparison.OrdinalIgnoreCase))
            {
                schema["type"] = "string";
                schema["description"] = "Nome da tabela de preço deste produto. Use a tabela geral informada no pedido quando não houver uma tabela específica para o item.";
            }
            return schema;
        }

        private static IAFerramentaDefinicao LocalizarFerramentaCatalogo(List<IAFerramentaDefinicao> catalogo, string nome)
        {
            foreach (IAFerramentaDefinicao ferramenta in catalogo ?? new List<IAFerramentaDefinicao>())
            {
                if (string.Equals(ferramenta.Nome, nome, StringComparison.OrdinalIgnoreCase)) return ferramenta;
            }
            return null;
        }

        private static bool TipoSchemaContem(JToken tipo, string esperado)
        {
            JArray tipos = tipo as JArray;
            if (tipos == null) return string.Equals(Convert.ToString(tipo), esperado, StringComparison.OrdinalIgnoreCase);
            foreach (JToken item in tipos) if (string.Equals(Convert.ToString(item), esperado, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        private static string RaizEntradaLista(string valor)
        {
            string caminho = (valor ?? string.Empty).Trim();
            if (caminho.StartsWith("entrada.", StringComparison.OrdinalIgnoreCase)) caminho = caminho.Substring(8);
            int separador = caminho.IndexOfAny(new[] { '.', '[' });
            return separador >= 0 ? caminho.Substring(0, separador) : caminho;
        }

        private static string CampoItemLoop(string referencia, string idLoop)
        {
            string prefixo = (idLoop ?? string.Empty) + ".item.";
            string valor = (referencia ?? string.Empty).Trim();
            if (!valor.StartsWith(prefixo, StringComparison.OrdinalIgnoreCase)) return string.Empty;
            string restante = valor.Substring(prefixo.Length);
            int separador = restante.IndexOfAny(new[] { '.', '[' });
            return separador >= 0 ? restante.Substring(0, separador) : restante;
        }

        private static JObject SchemaIdInteiro(string nome, string descricao, int minimo)
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { nome, new JObject
                            {
                                { "type", "integer" },
                                { "description", descricao },
                                { "minimum", minimo }
                            }
                        }
                    }
                },
                { "required", new JArray(nome) },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaIdComLimite(string nome, string descricao, int minimo, int limiteMaximo)
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { nome, new JObject
                            {
                                { "type", "integer" },
                                { "description", descricao },
                                { "minimum", minimo }
                            }
                        },
                        { "limite", SchemaLimiteAte(limiteMaximo) }
                    }
                },
                { "required", new JArray(nome, "limite") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoTiposListar()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "termo", new JObject
                            {
                                { "type", "string" },
                                { "description", "Parte do nome do tipo de orçamento. Use string vazia para listar todos." },
                                { "maxLength", 100 }
                            }
                        },
                        { "limite", SchemaLimiteAte(20) }
                    }
                },
                { "required", new JArray("termo", "limite") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoEscopos()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "idTipoOrcamento", new JObject
                            {
                                { "type", "integer" },
                                { "description", "ID do tipo de orçamento. Use 0 quando estiver consultando um orçamento existente por idOrcamento." },
                                { "minimum", 0 }
                            }
                        },
                        { "idOrcamento", new JObject
                            {
                                { "type", "integer" },
                                { "description", "ID do orçamento existente para trazer respostas já salvas. Use 0 para novo orçamento." },
                                { "minimum", 0 }
                            }
                        },
                        { "sidEscopos", new JObject
                            {
                                { "type", "string" },
                                { "description", "IDs de escopos separados por pipe, ex.: |1|2|. Use string vazia para o sistema recuperar pelo tipo de orçamento." },
                                { "maxLength", 400 }
                            }
                        },
                        { "limite", SchemaLimiteAte(50) }
                    }
                },
                { "required", new JArray("idTipoOrcamento", "idOrcamento", "sidEscopos", "limite") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoServicosRecursos()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "idTipoOrcamento", new JObject
                            {
                                { "type", "integer" },
                                { "description", "ID do tipo de orçamento." },
                                { "minimum", 1 }
                            }
                        },
                        { "sidTipoServicos", new JObject
                            {
                                { "type", "string" },
                                { "description", "IDs dos tipos de serviço/produto separados por pipe, ex.: |11|12|. Use string vazia para recuperar pelo tipo de orçamento." },
                                { "maxLength", 400 }
                            }
                        },
                        { "limite", SchemaLimiteAte(30) }
                    }
                },
                { "required", new JArray("idTipoOrcamento", "sidTipoServicos", "limite") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoClientesBuscar()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "termo", new JObject
                            {
                                { "type", "string" },
                                { "description", "Nome, razão social, fantasia, CPF/CNPJ, IE/RG ou parte do texto do cliente." },
                                { "minLength", 2 },
                                { "maxLength", 120 }
                            }
                        },
                        { "somenteAtivos", new JObject
                            {
                                { "type", "string" },
                                { "description", "S para buscar apenas clientes ativos; N para incluir inativos." },
                                { "enum", new JArray("S", "N") }
                            }
                        },
                        { "limite", SchemaLimiteAte(20) }
                    }
                },
                { "required", new JArray("termo", "somenteAtivos", "limite") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaClientesResolverParaOrcamento()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "termo", new JObject
                            {
                                { "type", "string" },
                                { "description", "Nome, razão social, fantasia, CPF/CNPJ, IE/RG ou parte do texto do cliente informado pelo usuário." },
                                { "minLength", 2 },
                                { "maxLength", 120 }
                            }
                        },
                        { "somenteAtivos", new JObject
                            {
                                { "type", "string" },
                                { "description", "S para buscar apenas clientes ativos; N para incluir inativos." },
                                { "enum", new JArray("S", "N") }
                            }
                        },
                        { "limite", SchemaLimiteAte(20) }
                    }
                },
                { "required", new JArray("termo", "somenteAtivos", "limite") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaClientesCriarParaOrcamento()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "razaoSocial", SchemaTexto("Razão social/nome principal do cliente.", 3, 200) },
                        { "nomeFantasia", SchemaTexto("Nome fantasia. Use string vazia para copiar a razão social.", 0, 200) },
                        { "tipoPessoa", new JObject
                            {
                                { "type", "string" },
                                { "description", "J = Jurídica, F = Física, O = Outros. Use J quando não der para inferir." },
                                { "enum", new JArray("J", "F", "O") }
                            }
                        },
                        { "origemParceiro", new JObject
                            {
                                { "type", "string" },
                                { "description", "N = Nacional, E = Estrangeiro, U = USA. Use N por padrão." },
                                { "enum", new JArray("N", "E", "U") }
                            }
                        },
                        { "documento", SchemaTexto("CPF, CNPJ, VAT, FEIN ou documento equivalente. Recomendado; use string vazia apenas com permitirSemDocumento = S.", 0, 50) },
                        { "permitirSemDocumento", new JObject
                            {
                                { "type", "string" },
                                { "description", "S permite criar sem documento; N exige documento." },
                                { "enum", new JArray("S", "N") }
                            }
                        },
                        { "rgIe", SchemaTexto("RG/IE. Use string vazia se não houver.", 0, 15) },
                        { "inscricaoMunicipal", SchemaTexto("Inscrição municipal. Use string vazia se não houver.", 0, 15) },
                        { "idVendedor", SchemaInteiro("ID do vendedor responsável. Use 0 se não souber.", 0, null) },
                        { "sidTabela", SchemaTexto("IDs de tabelas vinculadas separados por pipe ou ponto e vírgula. Use string vazia se não houver.", 0, 10) },
                        { "idPais", SchemaInteiro("ID do país. Use 0 se não souber.", 0, null) },
                        { "observacao", SchemaTexto("Observação do cadastro. Use string vazia se não houver.", 0, 1000) },
                        { "contatoNome", SchemaTexto("Nome do contato principal.", 3, 50) },
                        { "contatoTipo", SchemaTexto("Tipo do contato. Use Comercial por padrão.", 0, 50) },
                        { "contatoTelefone", SchemaTexto("Telefone do contato. Use string vazia se não houver.", 0, 20) },
                        { "contatoEmail", SchemaTexto("E-mail do contato. Use string vazia se não houver.", 0, 100) },
                        { "cep", SchemaTexto("CEP do endereço fiscal. Use string vazia quando for estrangeiro ou desconhecido.", 0, 10) },
                        { "logradouro", SchemaTexto("Logradouro do endereço fiscal.", 2, 200) },
                        { "numero", SchemaTexto("Número do endereço fiscal. Use string vazia se não houver.", 0, 50) },
                        { "complemento", SchemaTexto("Complemento do endereço fiscal. Use string vazia se não houver.", 0, 50) },
                        { "bairro", SchemaTexto("Bairro do endereço fiscal. Use string vazia se não houver.", 0, 200) },
                        { "cidade", SchemaTexto("Cidade do endereço fiscal.", 2, 200) },
                        { "uf", SchemaTexto("UF do endereço fiscal em duas letras. Use string vazia quando for estrangeiro.", 0, 2) },
                        { "pais", SchemaTexto("País do endereço fiscal. Use Brasil por padrão.", 0, 100) },
                        { "usarMesmoEnderecoEntrega", new JObject
                            {
                                { "type", "string" },
                                { "description", "S para criar entrega igual ao fiscal; N para usar campos de entrega separados." },
                                { "enum", new JArray("S", "N") }
                            }
                        },
                        { "entregaCep", SchemaTexto("CEP do endereço de entrega. Use string vazia se usar o mesmo fiscal ou quando desconhecido.", 0, 10) },
                        { "entregaLogradouro", SchemaTexto("Logradouro do endereço de entrega. Use string vazia se usar o mesmo fiscal.", 0, 200) },
                        { "entregaNumero", SchemaTexto("Número do endereço de entrega. Use string vazia se usar o mesmo fiscal ou se não houver.", 0, 50) },
                        { "entregaComplemento", SchemaTexto("Complemento do endereço de entrega. Use string vazia se não houver.", 0, 50) },
                        { "entregaBairro", SchemaTexto("Bairro do endereço de entrega. Use string vazia se não houver.", 0, 200) },
                        { "entregaCidade", SchemaTexto("Cidade do endereço de entrega. Use string vazia se usar o mesmo fiscal.", 0, 200) },
                        { "entregaUf", SchemaTexto("UF do endereço de entrega. Use string vazia quando for estrangeiro.", 0, 2) },
                        { "entregaPais", SchemaTexto("País do endereço de entrega. Use string vazia se usar o mesmo fiscal.", 0, 100) }
                    }
                },
                { "required", new JArray(
                    "razaoSocial",
                    "nomeFantasia",
                    "tipoPessoa",
                    "origemParceiro",
                    "documento",
                    "permitirSemDocumento",
                    "rgIe",
                    "inscricaoMunicipal",
                    "idVendedor",
                    "sidTabela",
                    "idPais",
                    "observacao",
                    "contatoNome",
                    "contatoTipo",
                    "contatoTelefone",
                    "contatoEmail",
                    "cep",
                    "logradouro",
                    "numero",
                    "complemento",
                    "bairro",
                    "cidade",
                    "uf",
                    "pais",
                    "usarMesmoEnderecoEntrega",
                    "entregaCep",
                    "entregaLogradouro",
                    "entregaNumero",
                    "entregaComplemento",
                    "entregaBairro",
                    "entregaCidade",
                    "entregaUf",
                    "entregaPais") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaCepConsultar()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "cep", SchemaTexto("CEP com ou sem máscara.", 8, 10) }
                    }
                },
                { "required", new JArray("cep") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaClientesCompletarParaOrcamento()
        {
            JObject propriedades = new JObject
            {
                { "idCliente", SchemaInteiro("ID do cliente existente que precisa ser completado.", 1, null) },
                { "contatoNome", SchemaTexto("Nome do contato principal. Use string vazia se o cliente já possuir contato.", 0, 50) },
                { "contatoTipo", SchemaTexto("Tipo do contato. Use Comercial por padrão.", 0, 50) },
                { "contatoTelefone", SchemaTexto("Telefone do contato. Use string vazia se não houver.", 0, 20) },
                { "contatoEmail", SchemaTexto("E-mail do contato. Use string vazia se não houver.", 0, 100) },
                { "cep", SchemaTexto("CEP do endereço fiscal. Pode preencher logradouro, bairro, cidade e UF.", 0, 10) },
                { "logradouro", SchemaTexto("Logradouro fiscal. Use string vazia quando o CEP puder preenchê-lo.", 0, 200) },
                { "numero", SchemaTexto("Número do endereço fiscal.", 0, 50) },
                { "complemento", SchemaTexto("Complemento fiscal. Use string vazia se não houver.", 0, 50) },
                { "bairro", SchemaTexto("Bairro fiscal. Use string vazia quando o CEP puder preenchê-lo.", 0, 200) },
                { "cidade", SchemaTexto("Cidade fiscal. Use string vazia quando o CEP puder preenchê-la.", 0, 200) },
                { "uf", SchemaTexto("UF fiscal. Use string vazia quando o CEP puder preenchê-la.", 0, 2) },
                { "pais", SchemaTexto("País fiscal. Use Brasil por padrão.", 0, 100) },
                { "usarMesmoEnderecoEntrega", new JObject { { "type", "string" }, { "description", "S para copiar o fiscal na entrega; N para informar outro endereço." }, { "enum", new JArray("S", "N") } } },
                { "entregaCep", SchemaTexto("CEP de entrega. Use string vazia se for igual ao fiscal.", 0, 10) },
                { "entregaLogradouro", SchemaTexto("Logradouro de entrega. Use string vazia se for igual ao fiscal.", 0, 200) },
                { "entregaNumero", SchemaTexto("Número de entrega. Use string vazia se for igual ao fiscal.", 0, 50) },
                { "entregaComplemento", SchemaTexto("Complemento de entrega. Use string vazia se não houver.", 0, 50) },
                { "entregaBairro", SchemaTexto("Bairro de entrega. Use string vazia se for igual ao fiscal.", 0, 200) },
                { "entregaCidade", SchemaTexto("Cidade de entrega. Use string vazia se for igual ao fiscal.", 0, 200) },
                { "entregaUf", SchemaTexto("UF de entrega. Use string vazia se for igual ao fiscal.", 0, 2) },
                { "entregaPais", SchemaTexto("País de entrega. Use string vazia se for igual ao fiscal.", 0, 100) }
            };
            JArray required = new JArray();
            foreach (JProperty propriedade in propriedades.Properties()) required.Add(propriedade.Name);
            return new JObject
            {
                { "type", "object" },
                { "properties", propriedades },
                { "required", required },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoTermoLimite(string descricaoTermo, int tamanhoMinimo, int limiteMaximo)
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "termo", new JObject
                            {
                                { "type", "string" },
                                { "description", descricaoTermo },
                                { "minLength", tamanhoMinimo },
                                { "maxLength", 120 }
                            }
                        },
                        { "limite", SchemaLimiteAte(limiteMaximo) }
                    }
                },
                { "required", new JArray("termo", "limite") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoUsuariosResolver()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "modoUsuario", new JObject
                            {
                                { "type", "string" },
                                { "enum", new JArray("USUARIO_LOGADO", "OUTRO_USUARIO") },
                                { "description", "USUARIO_LOGADO para usar o usuário autenticado; OUTRO_USUARIO para escolher outro usuário do sistema." }
                            }
                        }
                    }
                },
                { "required", new JArray("modoUsuario") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoEmpresaDetalhar()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "idEmpresa", new JObject
                            {
                                { "type", "integer" },
                                { "description", "ID da empresa. Use 0 se tiver apenas o idParceiro." },
                                { "minimum", 0 }
                            }
                        },
                        { "idParceiro", new JObject
                            {
                                { "type", "integer" },
                                { "description", "ID do parceiro vinculado à empresa. Use 0 se tiver idEmpresa." },
                                { "minimum", 0 }
                            }
                        },
                        { "limite", SchemaLimiteAte(20) }
                    }
                },
                { "required", new JArray("idEmpresa", "idParceiro", "limite") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoTabelasPrecoBuscar()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "termo", new JObject
                            {
                                { "type", "string" },
                                { "description", "Nome ou observação da tabela de preço." },
                                { "minLength", 2 },
                                { "maxLength", 120 }
                            }
                        },
                        { "idTipoTabela", new JObject
                            {
                                { "type", "integer" },
                                { "description", "ID do tipo da tabela. Use 0 para todos." },
                                { "minimum", 0 }
                            }
                        },
                        { "limite", SchemaLimiteAte(20) }
                    }
                },
                { "required", new JArray("termo", "idTipoTabela", "limite") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoTabelasPrecoListar()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "idTipoTabela", new JObject
                            {
                                { "type", "integer" },
                                { "description", "ID do tipo da tabela. Use 0 para todos." },
                                { "minimum", 0 }
                            }
                        },
                        { "limite", SchemaLimiteAte(20) }
                    }
                },
                { "required", new JArray("idTipoTabela", "limite") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoItensDisponiveis()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "idTabela", new JObject
                            {
                                { "type", "integer" },
                                { "description", "ID da tabela de preço." },
                                { "minimum", 1 }
                            }
                        },
                        { "termo", new JObject
                            {
                                { "type", "string" },
                                { "description", "Código, descrição, grupo, família ou tipo do item. Use string vazia para listar os primeiros itens liberados." },
                                { "maxLength", 120 }
                            }
                        },
                        { "limite", SchemaLimiteAte(30) }
                    }
                },
                { "required", new JArray("idTabela", "termo", "limite") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoProdutosBuscar()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "termo", new JObject
                            {
                                { "type", "string" },
                                { "description", "Código, descrição, grupo, família ou tipo do produto/serviço/recurso." },
                                { "minLength", 2 },
                                { "maxLength", 120 }
                            }
                        },
                        { "idTabela", new JObject
                            {
                                { "type", "integer" },
                                { "description", "ID da tabela de preço. Use 0 para buscar no cadastro geral de produtos." },
                                { "minimum", 0 }
                            }
                        },
                        { "tabelaPreco", SchemaTexto("Nome da tabela de preço do item. Quando informado, sobrescreve idTabela só nesta busca.", 0, 120) },
                        { "tipoItem", new JObject
                            {
                                { "type", "string" },
                                { "description", "Filtro do tipo esperado." },
                                { "enum", new JArray("todos", "produto", "servico_recurso") }
                            }
                        },
                        { "limite", SchemaLimiteAte(30) }
                    }
                },
                { "required", new JArray("termo", "idTabela", "tipoItem", "limite") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoProdutoPrecoResolver()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "idProduto", SchemaInteiro("ID do produto/serviço/recurso validado.", 1, null) },
                        { "idTabelaPreco", SchemaInteiro("ID da tabela de preço. Use 0 para resolver pela tabela do orçamento.", 0, null) },
                        { "tabelaPreco", SchemaTexto("Nome da tabela de preço do item. Use vazio para usar idTabelaPreco ou a tabela do orçamento.", 0, 120) },
                        { "idOrcamento", SchemaInteiro("ID do orçamento já criado. Use 0 se informou idTabelaPreco.", 0, null) },
                        { "quantidade", SchemaNumero("Quantidade solicitada.", 0, null) },
                        { "tipoItem", new JObject
                            {
                                { "type", "string" },
                                { "description", "Classificação esperada do item." },
                                { "enum", new JArray("produto", "servico_recurso") }
                            }
                        },
                        { "descontoPercentual", SchemaNumero("Desconto percentual. Use 0 se não houver.", 0, 100) }
                    }
                },
                { "required", new JArray("idProduto", "idTabelaPreco", "idOrcamento", "quantidade", "tipoItem", "descontoPercentual") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoProdutoCodigo()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "codigoProduto", new JObject
                            {
                                { "type", "string" },
                                { "description", "Código exato do produto." },
                                { "minLength", 1 },
                                { "maxLength", 50 }
                            }
                        },
                        { "idTabela", new JObject
                            {
                                { "type", "integer" },
                                { "description", "ID da tabela de preço. Use 0 se ainda não souber." },
                                { "minimum", 0 }
                            }
                        },
                        { "idCliente", new JObject
                            {
                                { "type", "integer" },
                                { "description", "ID do cliente/parceiro. Use 0 se ainda não souber." },
                                { "minimum", 0 }
                            }
                        }
                    }
                },
                { "required", new JArray("codigoProduto", "idTabela", "idCliente") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoProdutosSugestoes()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "idProduto", SchemaInteiro("ID do produto principal já resolvido.", 1, null) },
                        { "idCliente", SchemaInteiro("ID do cliente do orçamento. Use 0 quando ainda não estiver resolvido.", 0, null) },
                        { "quantidade", SchemaNumero("Quantidade do produto principal usada para calcular a quantidade proporcional sugerida.", 0, null) },
                        { "limite", SchemaLimiteAte(20) }
                    }
                },
                { "required", new JArray("idProduto", "idCliente", "quantidade", "limite") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoDuplicar()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "idOrcamento", SchemaInteiro("ID interno do orçamento de origem. Use 0 para localizar pelo número visível.", 0, null) },
                        { "nNumeroOrcamento", SchemaInteiro("Número visível do orçamento de origem. Use 0 quando informar idOrcamento.", 0, null) },
                        { "referencia", SchemaTexto("Nova referência. Use string vazia para acrescentar ' - Duplicado' à referência atual.", 0, 60) },
                        { "observacao", SchemaTexto("Nova observação. Use string vazia para manter a observação do orçamento de origem.", 0, 1000) },
                        { "dataEstimativaEntrega", SchemaTexto("Nova estimativa no formato dd/MM/yyyy. Use string vazia para manter data futura ou recalcular uma data vencida.", 0, 10) },
                        { "chaveIdempotencia", SchemaTexto("Chave técnica da operação. Use string vazia; o sistema gera e preserva uma chave antes da confirmação.", 0, 80) }
                    }
                },
                { "required", new JArray("idOrcamento", "nNumeroOrcamento", "referencia", "observacao", "dataEstimativaEntrega", "chaveIdempotencia") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoCondicaoPagamento()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "idCondicaoPagamento", new JObject
                            {
                                { "type", "integer" },
                                { "description", "ID da condição de pagamento. Use 0 para pesquisar por termo." },
                                { "minimum", 0 }
                            }
                        },
                        { "termo", new JObject
                            {
                                { "type", "string" },
                                { "description", "Início do nome da condição de pagamento. Use string vazia para consultar por ID ou listar." },
                                { "maxLength", 50 }
                            }
                        },
                        { "tipoVinculo", new JObject
                            {
                                { "type", "integer" },
                                { "description", "0 = todas, 1 = apenas vinculadas a pedido/orçamento, 2 = apenas globais." },
                                { "enum", new JArray(0, 1, 2) }
                            }
                        },
                        { "limite", SchemaLimiteAte(20) }
                    }
                },
                { "required", new JArray("idCondicaoPagamento", "termo", "tipoVinculo", "limite") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoCriarCabecalho()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "idCliente", SchemaInteiro("ID do cliente/parceiro do orçamento.", 1, null) },
                        { "idTipoOrcamento", SchemaInteiro("ID do tipo de orçamento.", 1, null) },
                        { "idFluxo", SchemaInteiro("ID do fluxo vinculado ao tipo de orçamento.", 1, null) },
                        { "idVendedor", SchemaInteiro("ID de usuário do vendedor, como usado no combo da tela.", 1, null) },
                        { "idCondicaoPagamento", SchemaInteiro("ID da condição de pagamento.", 1, null) },
                        { "idEmpresa", SchemaInteiro("ID da empresa no orçamento; use o valor do combo da tela, normalmente o idParceiro da empresa.", 1, null) },
                        { "idTabelaPreco", SchemaInteiro("ID da tabela de preço.", 1, null) },
                        { "idEnderecoFiscal", SchemaInteiro("ID do endereço fiscal do cliente.", 1, null) },
                        { "idEnderecoEntrega", SchemaInteiro("ID do endereço de entrega do cliente; use -1 para Coleta.", -1, null) },
                        { "idContatoCliente", SchemaInteiro("ID do contato do cliente.", 1, null) },
                        { "idTipoEnvio", SchemaInteiro("ID da forma de envio.", 1, null) },
                        { "idTipoCliente", SchemaInteiro("ID do tipo de cliente. Use 0 se não souber.", 0, null) },
                        { "idCidadeEntrega", SchemaInteiro("ID da cidade de entrega. Use 0 se não souber.", 0, null) },
                        { "idInstalador", SchemaInteiro("ID do instalador. Use 0 se não houver.", 0, null) },
                        { "referencia", SchemaTexto("Referência do orçamento.", 3, 60) },
                        { "dataEstimativaEntrega", SchemaTexto("Data estimada de entrega no formato dd/MM/yyyy.", 10, 10) },
                        { "destinoVenda", new JObject
                            {
                                { "type", "string" },
                                { "description", "Tipo de venda: C = Consumo, R = Revenda, I = Industrialização." },
                                { "enum", new JArray("C", "R", "I") }
                            }
                        },
                        { "validadeDias", SchemaInteiro("Validade do orçamento em dias.", 1, 365) },
                        { "observacao", SchemaTexto("Observação do usuário que ficará gravada no orçamento. Use string vazia se não houver.", 0, 1000) },
                        { "observacaoHistorico", SchemaTexto("Observação técnica para gravar no histórico após criar o orçamento. Use string vazia se não houver.", 0, 500) },
                        { "confidencial", new JObject
                            {
                                { "type", "string" },
                                { "description", "S para confidencial, N para normal." },
                                { "enum", new JArray("S", "N") }
                            }
                        },
                        { "empresaTransporte", SchemaTexto("Nome da transportadora/empresa de transporte. Use string vazia se não houver.", 0, 200) },
                        { "fretePrevisto", SchemaNumero("Valor de frete previsto. Use 0 se não houver.", 0, null) },
                        { "diasPrevisao", SchemaInteiro("Dias de previsão. Use 0 se não houver.", 0, null) },
                        { "ufFiscal", SchemaTexto("UF fiscal em duas letras. Use string vazia se não souber.", 0, 2) },
                        { "ufEntrega", SchemaTexto("UF de entrega em duas letras. Use string vazia se não souber.", 0, 2) },
                        { "sidSegmentosCliente", SchemaTexto("IDs de segmentos do cliente separados por pipe, ex.: |1|2|. Use string vazia se não souber.", 0, 200) },
                        { "sidTiposServicos", SchemaTexto("IDs de tipos de serviço/produto separados por pipe, ex.: |11|12|. Use string vazia para recuperar pelo tipo de orçamento.", 0, 200) },
                        { "sidEscopos", SchemaTexto("IDs de escopos separados por pipe, ex.: |1|2|. Use string vazia para recuperar pelo tipo de orçamento.", 0, 200) }
                    }
                },
                { "required", new JArray(
                    "idCliente",
                    "idTipoOrcamento",
                    "idFluxo",
                    "idVendedor",
                    "idCondicaoPagamento",
                    "idEmpresa",
                    "idTabelaPreco",
                    "idEnderecoFiscal",
                    "idEnderecoEntrega",
                    "idContatoCliente",
                    "idTipoEnvio",
                    "idTipoCliente",
                    "idCidadeEntrega",
                    "idInstalador",
                    "referencia",
                    "dataEstimativaEntrega",
                    "destinoVenda",
                    "validadeDias",
                    "observacao",
                    "confidencial",
                    "empresaTransporte",
                    "fretePrevisto",
                    "diasPrevisao",
                    "ufFiscal",
                    "ufEntrega",
                    "sidSegmentosCliente",
                    "sidTiposServicos",
                    "sidEscopos") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoAdicionarItem()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "idOrcamento", SchemaInteiro("ID do orçamento existente.", 1, null) },
                        { "idProduto", SchemaInteiro("ID do produto/serviço/recurso validado.", 1, null) },
                        { "codigoProduto", SchemaTexto("Código do produto/serviço/recurso. Use string vazia se tiver apenas o ID.", 0, 50) },
                        { "descricao", SchemaTexto("Descrição que será gravada no item. Use string vazia para usar a descrição do cadastro.", 0, 200) },
                        { "unidade", SchemaTexto("Unidade do item. Use string vazia para usar a unidade do cadastro.", 0, 3) },
                        { "quantidade", SchemaNumero("Quantidade do item.", 0, null) },
                        { "valorUnitario", SchemaNumero("Valor unitário resolvido automaticamente pela tabela de preço. Use 0 para o backend resolver.", 0, null) },
                        { "descontoPercentual", SchemaNumero("Desconto percentual. Use 0 se não houver.", 0, 100) },
                        { "margemPercentual", SchemaNumero("Margem percentual para serviço/recurso. Use 0 se não houver.", 0, 1000) },
                        { "ajustePercentual", SchemaNumero("Ajuste percentual para serviço/recurso. Use 0 se não houver.", 0, 1000) },
                        { "valorTotal", SchemaNumero("Valor total do item. Use 0 para o sistema calcular ou resolver junto com o preço.", 0, null) },
                        { "ordem", SchemaInteiro("Ordem visual do item. Use 0 para o sistema escolher a próxima ordem.", 0, null) },
                        { "diasPrevisaoEntrega", SchemaInteiro("Dias a partir da data do orçamento para previsão de entrega do item.", 0, null) },
                        { "idProdutoPai", SchemaInteiro("ID do item pai quando for composição. Nesta fase use 0 para item raiz.", 0, null) },
                        { "idProdutoAvo", SchemaInteiro("ID do item avô quando for composição. Nesta fase use 0 para item raiz.", 0, null) },
                        { "idProdutoBisavo", SchemaInteiro("ID do item bisavô quando for composição. Nesta fase use 0 para item raiz.", 0, null) },
                        { "tipoItem", new JObject
                            {
                                { "type", "string" },
                                { "description", "Classificação esperada do item no orçamento." },
                                { "enum", new JArray("produto", "servico_recurso") }
                            }
                        }
                    }
                },
                { "required", new JArray(
                    "idOrcamento",
                    "idProduto",
                    "codigoProduto",
                    "descricao",
                    "unidade",
                    "quantidade",
                    "descontoPercentual",
                    "margemPercentual",
                    "ajustePercentual",
                    "ordem",
                    "diasPrevisaoEntrega",
                    "idProdutoPai",
                    "idProdutoAvo",
                    "idProdutoBisavo",
                    "tipoItem") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaOrcamentoSalvarRespostaEscopo()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "idOrcamento", SchemaInteiro("ID do orçamento existente.", 1, null) },
                        { "idEscopo", SchemaInteiro("ID do escopo.", 1, null) },
                        { "idCategoria", SchemaInteiro("ID da categoria do escopo.", 1, null) },
                        { "pergunta", SchemaTexto("Texto da pergunta da matriz de escopo.", 1, 500) },
                        { "idPergunta", SchemaInteiro("ID da pergunta quando existir. Use 0 quando a pergunta não tiver ID separado.", 0, null) },
                        { "idOpcao", SchemaInteiro("ID da opção escolhida.", 1, null) },
                        { "opcao", SchemaTexto("Texto da opção escolhida.", 1, 500) }
                    }
                },
                { "required", new JArray(
                    "idOrcamento",
                    "idEscopo",
                    "idCategoria",
                    "pergunta",
                    "idPergunta",
                    "idOpcao",
                    "opcao") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaCrmNegociosBuscar()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "termo", SchemaTexto("Texto para filtrar cliente, referência, vendedor, status, controle ou ID. Use string vazia para filtrar apenas por IDs.", 0, 120) },
                        { "idCliente", SchemaInteiro("ID do cliente/parceiro. Use 0 para todos.", 0, null) },
                        { "idVendedor", SchemaInteiro("ID do vendedor. Use 0 para todos.", 0, null) },
                        { "idTipoCotacao", SchemaInteiro("ID do tipo de cotação/orçamento. Use 0 para todos.", 0, null) },
                        { "idStatus", SchemaInteiro("ID do status CRM. Use 0 para ativos/não excluídos.", 0, null) },
                        { "confidencial", new JObject
                            {
                                { "type", "string" },
                                { "description", "T = todos permitidos, S = confidenciais, N = normais. Usuários sem permissão de confidencial serão forçados para N." },
                                { "enum", new JArray("T", "S", "N") }
                            }
                        },
                        { "limite", SchemaLimiteAte(20) }
                    }
                },
                { "required", new JArray("termo", "idCliente", "idVendedor", "idTipoCotacao", "idStatus", "confidencial", "limite") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaCrmNegocioCriarParaOrcamento()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "idOrcamento", SchemaInteiro("ID interno do orçamento criado.", 1, null) },
                        { "nNumeroOrcamento", SchemaInteiro("Número visível do orçamento. Use 0 para resolver pelo idOrcamento.", 0, null) },
                        { "idTipoCotacao", SchemaInteiro("ID do tipo de cotação/orçamento. Use o mesmo idTipoOrcamento do cabeçalho ou 0 para resolver pelo idOrcamento.", 0, null) },
                        { "idVendedor", SchemaInteiro("ID do vendedor. Use 0 para resolver pelo orçamento.", 0, null) },
                        { "idStatus", SchemaInteiro("ID do status inicial CRM. Use 0 para usar o primeiro status ativo do CRM.", 0, null) },
                        { "idCliente", SchemaInteiro("ID do cliente/parceiro. Use 0 para resolver pelo orçamento.", 0, null) },
                        { "clienteNome", SchemaTexto("Razão social/nome do cliente para exibir no CRM. Use string vazia para resolver pelo cliente.", 0, 200) },
                        { "idContatoCliente", SchemaInteiro("ID do contato do cliente. Use 0 se não houver.", 0, null) },
                        { "referencia", SchemaTexto("Referência do negócio CRM.", 3, 200) },
                        { "observacao", SchemaTexto("Observação do negócio CRM. Use string vazia se não houver.", 0, 1000) },
                        { "dataPrevisao", SchemaTexto("Data de previsão no formato dd/MM/yyyy ou yyyy-MM-dd. Use string vazia para usar a previsão do orçamento ou hoje.", 0, 30) },
                        { "confidencial", new JObject
                            {
                                { "type", "string" },
                                { "description", "S para confidencial, N para normal." },
                                { "enum", new JArray("S", "N") }
                            }
                        },
                        { "chance", SchemaTexto("Chance do negócio, de 0 a 100. Use 0 se não souber.", 0, 5) },
                        { "valorMaterial", SchemaNumero("Valor de material. Use 0 se não houver.", 0, null) },
                        { "valorServico", SchemaNumero("Valor de serviço. Use 0 se não houver.", 0, null) },
                        { "controle", SchemaTexto("Número/controle comercial. Use string vazia se não houver.", 0, 100) }
                    }
                },
                { "required", new JArray(
                    "idOrcamento",
                    "nNumeroOrcamento",
                    "idTipoCotacao",
                    "idVendedor",
                    "idStatus",
                    "idCliente",
                    "clienteNome",
                    "idContatoCliente",
                    "referencia",
                    "observacao",
                    "dataPrevisao",
                    "confidencial",
                    "chance",
                    "valorMaterial",
                    "valorServico",
                    "controle") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaCrmFollowupCriar()
        {
            return new JObject
            {
                { "type", "object" },
                { "properties", new JObject
                    {
                        { "idRegistroCRM", SchemaInteiro("ID do negócio CRM.", 1, null) },
                        { "dataContato", SchemaTexto("Data/hora do contato. Use string vazia para agora.", 0, 30) },
                        { "idMeioContato", SchemaInteiro("ID do meio de contato do CRM.", 1, null) },
                        { "contatoCom", SchemaTexto("Nome da pessoa contatada.", 3, 100) },
                        { "observacao", SchemaTexto("Observação do follow-up. Use string vazia se não houver.", 0, 300) },
                        { "dataProximoContato", SchemaTexto("Data/hora do próximo contato em dd/MM/yyyy ou yyyy-MM-dd.", 10, 30) }
                    }
                },
                { "required", new JArray("idRegistroCRM", "dataContato", "idMeioContato", "contatoCom", "observacao", "dataProximoContato") },
                { "additionalProperties", false }
            };
        }

        private static JObject SchemaInteiro(string descricao, int minimo, int? maximo)
        {
            JObject schema = new JObject
            {
                { "type", "integer" },
                { "description", descricao },
                { "minimum", minimo }
            };

            if (maximo.HasValue)
            {
                schema["maximum"] = maximo.Value;
            }

            return schema;
        }

        private static JObject SchemaNumero(string descricao, decimal minimo, decimal? maximo)
        {
            JObject schema = new JObject
            {
                { "type", "number" },
                { "description", descricao },
                { "minimum", minimo }
            };

            if (maximo.HasValue)
            {
                schema["maximum"] = maximo.Value;
            }

            return schema;
        }

        private static JObject SchemaTexto(string descricao, int minimo, int maximo)
        {
            JObject schema = new JObject
            {
                { "type", "string" },
                { "description", descricao },
                { "maxLength", maximo }
            };

            if (minimo > 0)
            {
                schema["minLength"] = minimo;
            }

            return schema;
        }

        private static JObject SchemaLimiteAte(int maximo)
        {
            return new JObject
            {
                { "type", "integer" },
                { "description", "Quantidade máxima de registros para retornar. Use entre 1 e " + maximo + "." },
                { "minimum", 1 },
                { "maximum", maximo }
            };
        }

        private static JObject SchemaLimite()
        {
            return new JObject
            {
                { "type", "integer" },
                { "description", "Quantidade maxima de registros para retornar. Use entre 1 e 10." },
                { "minimum", 1 },
                { "maximum", 10 }
            };
        }
    }
}
