using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IDENTITY = TT.FrameWork.Identity;

namespace TT_Flow.FrameWork.IA
{
    // Motor de execução dos workflows agênticos. O núcleo Caminhar percorre o grafo executando os nós
    // de leitura (ferramenta READ / condição / definir) pelo cls_IA_ToolExecutor (herdando permissão,
    // whitelist, máscara e auditoria) e PAUSA ao chegar num passo de ESCRITA (resolve os argumentos mas
    // não executa). Quem executa a escrita e retoma é o cls_IA_WorkflowExecucaoService (Fase 2), após a
    // confirmação do usuário. Executar(...) é um atalho dry-run (READ-only) usado pelo botão Testar.
    public class cls_IA_WorkflowEngine
    {
        private const int MaxNosExecutados = 300;
        private const int TimeoutPadraoMs = 30000;
        // Menor valor aceito em IA.Workflow.TimeoutSegundos: antes disso a passada nem consulta a configuração.
        private const int TimeoutMinimoMs = 10000;
        private const int MaxSnippet = 1200;
        private const int MaxLoopIteracoes = 50;
        private const int MaxRetriesErro = 3;
        private const int MaxRetryIntervaloSegundos = 10;

        // Prefixos das mensagens da checagem prévia de referências (ReferenciaObrigatoriaVazia). ClassificarErro e
        // OrientarErroWorkflow reconhecem a mensagem por eles, antes de qualquer heurística por palavra.
        private const string PrefixoRegistroNaoEncontrado = "Registro não encontrado:";
        private const string PrefixoReferenciaVazia = "Referência vazia:";
        private const string PrefixoNoIASemDados = "Nó de IA sem dados:";
        private const string PrefixoTempoLimite = "Tempo limite do workflow excedido";

        // Dry-run (Testar): roda os READs e para no primeiro WRITE. Não executa escrita nem persiste.
        public IAWorkflowResultado Executar(int idConversaIA, IAWorkflow workflow, JObject entradas)
        {
            IAWorkflowOutcome o = Caminhar(idConversaIA, workflow, null, entradas, null);
            cls_IA_Auditoria.Registrar(idConversaIA, "WORKFLOW_TESTADO", "INFO", new { workflow = workflow != null ? workflow.Nome : "", status = o.Status });
            return new IAWorkflowResultado
            {
                Sucesso = o.Status != "ERRO",
                Status = o.Status,
                TipoPausa = o.TipoPausa,
                Mensagem = o.Mensagem,
                Trace = o.Trace,
                SaidaFinal = o.Saidas,
                DadosPausaJson = o.ArgsPausa != null ? o.ArgsPausa.ToString(Formatting.None) : "{}"
            };
        }

        // Núcleo: caminha do noInicial (ou do início) com o contexto dado. Para em CONCLUIDO (fim),
        // PAUSADO (chegou num passo de escrita - args resolvidos em ArgsPausa) ou ERRO.
        public IAWorkflowOutcome Caminhar(int idConversaIA, IAWorkflow workflow, JObject saidasIniciais, JObject entradas, string noInicial)
        {
            IAWorkflowOutcome outcome = new IAWorkflowOutcome { Trace = new List<IAWorkflowTracePasso>(), Status = "ERRO" };

            if (workflow == null || string.IsNullOrWhiteSpace(workflow.GrafoJson))
            {
                outcome.Mensagem = "Workflow sem grafo configurado.";
                return outcome;
            }

            JObject grafo;
            try { grafo = JObject.Parse(workflow.GrafoJson); }
            catch (Exception ex) { outcome.Mensagem = "Grafo inválido: " + ex.Message; return outcome; }

            Dictionary<string, JObject> nos = new Dictionary<string, JObject>(StringComparer.OrdinalIgnoreCase);
            string idInicio = null;
            foreach (JToken t in (grafo["nos"] as JArray) ?? new JArray())
            {
                JObject no = t as JObject;
                if (no == null) continue;
                string id = Str(no["id"]);
                if (id.Length == 0) continue;
                nos[id] = no;
                if (string.Equals(Str(no["tipo"]), "inicio", StringComparison.OrdinalIgnoreCase)) idInicio = id;
            }

            entradas = entradas ?? new JObject();
            Dictionary<string, JToken> saidas = new Dictionary<string, JToken>(StringComparer.OrdinalIgnoreCase);
            if (saidasIniciais != null)
                foreach (JProperty p in saidasIniciais.Properties()) saidas[p.Name] = p.Value;

            bool iniciarDoInicio = noInicial == null;
            string atual = iniciarDoInicio ? idInicio : noInicial;
            if (string.IsNullOrWhiteSpace(atual))
            {
                if (iniciarDoInicio)
                {
                    outcome.Mensagem = "O workflow não tem nó de início.";
                    return outcome;
                }

                outcome.Status = "CONCLUIDO";
                outcome.Mensagem = "Workflow concluído.";
                outcome.Saidas = SaidasParaJObject(saidas);
                return outcome;
            }

            cls_IA_ToolExecutor executor = new cls_IA_ToolExecutor();
            Stopwatch cronometro = Stopwatch.StartNew();
            int passos = 0;
            bool erro = false;
            int timeoutMs = 0;                       // só é lido da configuração quando a passada passa de TimeoutMinimoMs
            IAWorkflowTracePasso passoAberto = null; // passo em andamento: se o loop sair sem fechá-lo, recebe a duração no fim
            long inicioPasso = 0;
            long duracaoAntecipada = 0;              // tempo de uma busca já feita na validação prévia, que é do passo dela

            // Validação prévia das buscas (só na primeira passada): roda as buscas que o workflow vai precisar assim que
            // os dados estão normalizados, antes de qualquer pergunta ao usuário. Ver BuscasAntecipadasFalharam.
            List<JObject> ordemNos = ListarNosEmOrdem(grafo);
            Dictionary<string, List<string>> buscasObrigatorias = iniciarDoInicio
                ? BuscasObrigatorias(ordemNos, cls_IA_ToolRegistry.Obter)
                : new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, BuscaAntecipada> buscasFeitas = new Dictionary<string, BuscaAntecipada>(StringComparer.OrdinalIgnoreCase);
            bool validacaoPreviaPendente = buscasObrigatorias.Count > 0;
            if (validacaoPreviaPendente && !GrafoTemNoIA(ordemNos))
            {
                // Sem nó de IA para normalizar dados, as buscas já podem rodar com as entradas do workflow.
                validacaoPreviaPendente = false;
                if (BuscasAntecipadasFalharam(idConversaIA, executor, grafo, ordemNos, buscasObrigatorias, entradas, saidas, buscasFeitas, outcome, workflow))
                {
                    erro = true;
                    atual = null;
                }
            }

            while (atual != null && nos.ContainsKey(atual))
            {
                if (++passos > MaxNosExecutados) { outcome.Mensagem = "Limite de nós executados atingido (possível loop)."; erro = true; break; }
                if (TempoEsgotado(cronometro.ElapsedMilliseconds, ref timeoutMs))
                {
                    RegistrarTempoEsgotado(outcome, nos[atual], atual, timeoutMs);
                    erro = true; break;
                }

                JObject no = nos[atual];
                string tipo = Str(no["tipo"]).ToLowerInvariant();
                IAWorkflowTracePasso passo = new IAWorkflowTracePasso { No = atual, Tipo = tipo, Titulo = TituloNo(no) };
                passoAberto = passo;
                inicioPasso = cronometro.ElapsedMilliseconds;
                duracaoAntecipada = 0;

                if (tipo == "inicio") { passo.Status = "OK"; atual = Str(no["proximo"]); }
                else if (tipo == "fim") { passo.Status = "OK"; outcome.Trace.Add(passo); outcome.Status = "CONCLUIDO"; break; }
                else if (tipo == "ferramenta")
                {
                    string nome = Str(no["ferramenta"]);
                    IAFerramentaDefinicao def = cls_IA_ToolRegistry.Obter(nome);
                    string diagnostico = def != null
                        ? cls_IA_ToolRegistry.DiagnosticarDisponibilidade(def, true)
                        : cls_IA_ToolRegistry.DiagnosticarDisponibilidade(nome, true);
                    if (def == null || !string.IsNullOrWhiteSpace(diagnostico))
                    {
                        if (string.IsNullOrWhiteSpace(diagnostico)) diagnostico = "Ferramenta inexistente ou indisponível: " + nome;
                        if (!EncaminharErroTratado(no, atual, diagnostico, nos, saidas, passo, out atual, 1, 1))
                        {
                            passo.Status = "ERRO"; passo.Saida = diagnostico;
                            outcome.Trace.Add(passo); erro = true; break;
                        }
                    }
                    else if (def.EhWorkflow())
                    {
                        if (!EncaminharErroTratado(no, atual, "Workflow não pode chamar outro workflow nesta fase.", nos, saidas, passo, out atual, 1, 1))
                        {
                            passo.Status = "ERRO"; passo.Saida = "Workflow não pode chamar outro workflow nesta fase.";
                            outcome.Trace.Add(passo); erro = true; break;
                        }
                    }
                    else
                    {
                        JArray camposPendentes = EntradasPendentesParaNo(no, def, grafo, entradas);
                        if (camposPendentes.Count > 0)
                        {
                            JObject argsPendentes = ArgsEntradasPendentes(atual, no, nome, camposPendentes);
                            passo.Status = "PAUSADO";
                            DefinirEntrada(passo, argsPendentes.ToString(Formatting.None));
                            passo.Saida = "Aguardando entradas do usuário.";
                            outcome.Trace.Add(passo);
                            outcome.Status = "PAUSADO";
                            outcome.TipoPausa = "ENTRADAS";
                            outcome.NoPausa = atual;
                            outcome.FerramentaPausa = nome;
                            outcome.ArgsPausa = argsPendentes;
                            outcome.Resumo = TituloNo(no);
                            outcome.Mensagem = "Informe os dados necessários para continuar o nó \"" + TituloNo(no) + "\".";
                            break;
                        }

                        JObject args = ResolverEntradas(no["entradas"] as JArray, entradas, saidas);
                        string referenciaVazia = ReferenciaObrigatoriaVazia(no, atual, def, args, nos, saidas);

                        if (referenciaVazia.Length > 0)
                        {
                            // Identificador obrigatório vazio vindo de outro passo: não executa nem pede confirmação
                            // (o usuário confirmaria uma escrita que já nasce inválida).
                            DefinirEntrada(passo, args.ToString(Formatting.None));
                            if (!EncaminharErroTratado(no, atual, referenciaVazia, nos, saidas, passo, out atual, 1, 1))
                            {
                                passo.Status = "ERRO"; DefinirSaida(passo, referenciaVazia);
                                outcome.Trace.Add(passo); erro = true; break;
                            }
                        }
                        else if (string.Equals(def.Escopo, "WRITE", StringComparison.OrdinalIgnoreCase) || def.RequerConfirmacao)
                        {
                            // Passo de ESCRITA: resolve os argumentos, marca como pausado e PARA (não executa).
                            string descricaoConfirmacao = string.IsNullOrWhiteSpace(def.Descricao)
                                ? "Revise os dados e confirme para executar a ferramenta \"" + nome + "\"."
                                : def.Descricao;
                            passo.Status = "PAUSADO";
                            DefinirEntrada(passo, args.ToString(Formatting.None));
                            passo.Saida = "Passo de escrita - aguardando confirmação.";
                            outcome.Trace.Add(passo);
                            outcome.Status = "PAUSADO";
                            outcome.TipoPausa = "FERRAMENTA";
                            outcome.NoPausa = atual;
                            outcome.FerramentaPausa = nome;
                            outcome.ArgsPausa = args;
                            outcome.ResumoPausa = MontarResumoEscritaSeguro(no, def, args, saidas);
                            outcome.Resumo = nome;
                            outcome.Mensagem = descricaoConfirmacao;
                            break;
                        }
                        else
                        {
                            DefinirEntrada(passo, args.ToString(Formatting.None));
                            int totalTentativas = 1 + RetriesErro(no);
                            int tentativaExecutada = 0;
                            string msgErro = string.Empty;
                            IAFerramentaResultado res = null;
                            if (!buscasFeitas.ContainsKey(atual)) AvisarProgresso("Executando ", no);
                            for (int tentativa = 1; tentativa <= totalTentativas; tentativa++)
                            {
                                tentativaExecutada = tentativa;
                                long duracaoBusca;
                                res = TomarBuscaAntecipada(buscasFeitas, atual, args, out duracaoBusca);
                                if (res != null) duracaoAntecipada += duracaoBusca;
                                else res = executor.Executar(idConversaIA, new IAToolCall { Nome = nome, Argumentos = args });
                                if (res != null && res.Status == "SUCESSO")
                                {
                                    break;
                                }

                                msgErro = DescreverErroFerramenta(nome, res);
                                if (tentativa < totalTentativas)
                                {
                                    AguardarRetry(no, tentativa, totalTentativas);
                                }
                            }

                            if (res == null || res.Status != "SUCESSO")
                            {
                                if (!EncaminharErroTratado(no, atual, msgErro, nos, saidas, passo, out atual, tentativaExecutada, totalTentativas))
                                {
                                    passo.Status = "ERRO";
                                    passo.Saida = MensagemErroComTentativa(msgErro, tentativaExecutada, totalTentativas);
                                    outcome.Trace.Add(passo); erro = true; break;
                                }
                            }
                            else
                            {
                                JToken saida = ExtrairSaida(res.ResultadoJson);
                                JObject resolucao = saida as JObject;
                                if (resolucao != null && resolucao.Value<bool?>("precisaEscolha") == true)
                                {
                                    bool selecaoMultipla = resolucao.Value<bool?>("selecaoMultipla") == true;
                                    JObject argsEscolha = ArgsEscolha(atual, no, nome, resolucao);
                                    passo.Status = "PAUSADO";
                                    DefinirSaida(passo, selecaoMultipla
                                        ? "Há sugestões opcionais. Aguardando seleção do usuário."
                                        : "Foram encontrados vários resultados. Aguardando escolha do usuário.");
                                    outcome.Trace.Add(passo);
                                    outcome.Status = "PAUSADO";
                                    outcome.TipoPausa = selecaoMultipla ? "SELECAO_MULTIPLA" : "ESCOLHA";
                                    outcome.NoPausa = atual;
                                    outcome.FerramentaPausa = nome;
                                    outcome.ArgsPausa = argsEscolha;
                                    outcome.Resumo = TituloNo(no);
                                    outcome.Mensagem = Convert.ToString(argsEscolha["mensagem"]);
                                    break;
                                }

                                saidas[atual] = saida;
                                passo.Status = "OK";
                                string saidaTexto = saida != null ? saida.ToString(Formatting.None) : "{}";
                                if (tentativaExecutada > 1)
                                {
                                    saidaTexto = "Sucesso na tentativa " + tentativaExecutada + " de " + totalTentativas + ". " + saidaTexto;
                                }
                                DefinirSaida(passo, saidaTexto);
                                atual = Str(no["proximo"]);
                            }
                        }

                    }
                }
                else if (tipo == "condicao")
                {
                    bool cond = AvaliarCondicao(no, entradas, saidas);
                    passo.Status = "OK";
                    passo.Entrada = Str(no["campo"]) + " " + Str(no["operador"]) + " \"" + Str(no["valor"]) + "\"";
                    passo.Saida = cond ? "verdadeiro -> ramo se sim" : "falso -> ramo se não";
                    atual = Str(cond ? no["seVerdadeiro"] : no["seFalso"]);
                }
                else if (tipo == "definir")
                {
                    JToken v = ResolverValor(Str(no["origem"]), Str(no["valor"]), entradas, saidas);
                    saidas[atual] = new JObject { { "valor", v } };
                    passo.Status = "OK";
                    DefinirSaida(passo, v != null ? v.ToString() : "");
                    atual = Str(no["proximo"]);
                }
                else if (tipo == "ia")
                {
                    JObject args = ResolverEntradas(no["entradas"] as JArray, entradas, saidas);
                    DefinirEntrada(passo, args.ToString(Formatting.None));

                    string erroIA = string.Empty;
                    bool exigeDadosIA = DadosDoNoLidosAdiante(ordemNos, atual);
                    int totalTentativas = 1 + RetriesErro(no);
                    int tentativaExecutada = 0;
                    JToken saidaIA = null;
                    AvisarProgresso("Executando ", no);
                    for (int tentativa = 1; tentativa <= totalTentativas; tentativa++)
                    {
                        tentativaExecutada = tentativa;
                        saidaIA = ExecutarNoIA(no, args, out erroIA, exigeDadosIA);
                        if (saidaIA != null)
                        {
                            break;
                        }

                        if (tentativa < totalTentativas)
                        {
                            AguardarRetry(no, tentativa, totalTentativas);
                        }
                    }

                    if (saidaIA == null)
                    {
                        if (!EncaminharErroTratado(no, atual, erroIA, nos, saidas, passo, out atual, tentativaExecutada, totalTentativas))
                        {
                            passo.Status = "ERRO";
                            passo.Saida = MensagemErroComTentativa(erroIA, tentativaExecutada, totalTentativas);
                            outcome.Trace.Add(passo); erro = true; break;
                        }
                    }
                    else
                    {
                        saidas[atual] = saidaIA;
                        passo.Status = "OK";
                        string saidaTexto = saidaIA.ToString(Formatting.None);
                        if (tentativaExecutada > 1)
                        {
                            saidaTexto = "Sucesso na tentativa " + tentativaExecutada + " de " + totalTentativas + ". " + saidaTexto;
                        }
                        DefinirSaida(passo, saidaTexto);
                        atual = Str(no["proximo"]);
                    }
                }
                else if (tipo == "merge")
                {
                    passo.Status = "OK";
                    passo.Saida = "Ramos reunidos; seguindo para o próximo nó.";
                    atual = Str(no["proximo"]);
                }
                else if (tipo == "aprovacao")
                {
                    string mensagemAprovacao = Str(no["mensagem"]);
                    JObject args = new JObject
                    {
                        { "titulo", string.IsNullOrWhiteSpace(Str(no["titulo"])) ? "Aprovação humana" : Str(no["titulo"]) },
                        { "mensagem", mensagemAprovacao }
                    };

                    passo.Status = "PAUSADO";
                    DefinirEntrada(passo, args.ToString(Formatting.None));
                    passo.Saida = "Aguardando aprovação humana.";
                    outcome.Trace.Add(passo);
                    outcome.Status = "PAUSADO";
                    outcome.TipoPausa = "APROVACAO";
                    outcome.NoPausa = atual;
                    outcome.ArgsPausa = args;
                    outcome.Resumo = TituloNo(no);
                    outcome.Mensagem = string.IsNullOrWhiteSpace(mensagemAprovacao)
                        ? "Revise a aprovação solicitada e escolha como o workflow deve continuar."
                        : mensagemAprovacao;
                    break;
                }
                else if (tipo == "loop")
                {
                    if (LoopModoLista(no))
                    {
                        JArray lista = ResolverListaLoop(no, entradas, saidas);
                        int total = lista.Count;
                        int indiceAtual = IndiceLoopAtual(saidas, atual);
                        if (indiceAtual < total)
                        {
                            int iteracao = indiceAtual + 1;
                            saidas[atual] = new JObject
                            {
                                { "indice", iteracao },
                                { "indice0", indiceAtual },
                                { "iteracao", iteracao },
                                { "total", total },
                                { "limite", total },
                                { "restante", Math.Max(0, total - iteracao) },
                                { "continuar", true },
                                { "item", lista[indiceAtual] != null ? lista[indiceAtual].DeepClone() : JValue.CreateNull() }
                            };
                            passo.Status = "OK";
                            passo.Saida = "Item " + iteracao + " de " + total + ".";
                            atual = Str(no["proximo"]);
                        }
                        else
                        {
                            saidas[atual] = new JObject
                            {
                                { "indice", indiceAtual },
                                { "indice0", total > 0 ? total - 1 : -1 },
                                { "iteracao", indiceAtual },
                                { "total", total },
                                { "limite", total },
                                { "restante", 0 },
                                { "continuar", false },
                                { "item", JValue.CreateNull() }
                            };
                            passo.Status = "OK";
                            passo.Saida = total == 0 ? "Lista vazia; seguindo para conclusão." : "Loop concluído após " + indiceAtual + " item(ns).";
                            atual = Str(no["aoConcluir"]);
                        }
                    }
                    else
                    {
                        int limite = LimiteLoop(no);
                        if (limite <= 0)
                        {
                            passo.Status = "ERRO";
                            passo.Saida = "Loop sem limite válido.";
                            outcome.Trace.Add(passo); erro = true; break;
                        }

                        int indiceAtual = IndiceLoopAtual(saidas, atual);
                        if (indiceAtual < limite)
                        {
                            int iteracao = indiceAtual + 1;
                            saidas[atual] = new JObject
                            {
                                { "indice", iteracao },
                                { "iteracao", iteracao },
                                { "limite", limite },
                                { "restante", Math.Max(0, limite - iteracao) },
                                { "continuar", true }
                            };
                            passo.Status = "OK";
                            passo.Saida = "Iteração " + iteracao + " de " + limite + ".";
                            atual = Str(no["proximo"]);
                        }
                        else
                        {
                            saidas[atual] = new JObject
                            {
                                { "indice", indiceAtual },
                                { "iteracao", indiceAtual },
                                { "limite", limite },
                                { "restante", 0 },
                                { "continuar", false }
                            };
                            passo.Status = "OK";
                            passo.Saida = "Loop concluído após " + indiceAtual + " iteração(ões).";
                            atual = Str(no["aoConcluir"]);
                        }
                    }
                }
                else if (tipo == "espera")
                {
                    JObject args = ArgsEspera(no);
                    string msgEspera;
                    bool continuar = EsperaPodeContinuar(args, out msgEspera);
                    DefinirEntrada(passo, args.ToString(Formatting.None));

                    if (continuar)
                    {
                        saidas[atual] = new JObject
                        {
                            { "aguardou", false },
                            { "status", "LIBERADO" },
                            { "modo", Str(args["modo"]) },
                            { "dataHora", Str(args["dataHora"]) },
                            { "evento", Str(args["evento"]) }
                        };
                        passo.Status = "OK";
                        passo.Saida = string.IsNullOrWhiteSpace(msgEspera) ? "Espera liberada; seguindo o fluxo." : msgEspera;
                        atual = Str(no["proximo"]);
                    }
                    else
                    {
                        saidas[atual] = new JObject
                        {
                            { "aguardou", true },
                            { "status", "AGUARDANDO" },
                            { "modo", Str(args["modo"]) },
                            { "dataHora", Str(args["dataHora"]) },
                            { "evento", Str(args["evento"]) }
                        };
                        passo.Status = "PAUSADO";
                        passo.Saida = string.IsNullOrWhiteSpace(msgEspera) ? "Aguardando liberação." : msgEspera;
                        outcome.Trace.Add(passo);
                        outcome.Status = "PAUSADO";
                        outcome.TipoPausa = "ESPERA";
                        outcome.NoPausa = atual;
                        outcome.ArgsPausa = args;
                        outcome.Resumo = TituloNo(no);
                        outcome.Mensagem = passo.Saida;
                        break;
                    }
                }
                else
                {
                    passo.Status = "ERRO"; passo.Saida = "Tipo de nó desconhecido: " + tipo;
                    outcome.Trace.Add(passo); erro = true; break;
                }

                passo.DuracaoMs = DuracaoDoPasso(cronometro, inicioPasso, duracaoAntecipada);
                passoAberto = null;
                if (tipo != "fim") outcome.Trace.Add(passo);

                cls_IA_Auditoria.Registrar(idConversaIA, "WORKFLOW_NO_EXECUTADO", passo.Status == "OK" ? "INFO" : "ALERTA", new
                {
                    workflow = workflow.Nome, no = passo.No, tipo = passo.Tipo, status = passo.Status, duracaoMs = passo.DuracaoMs
                });

                if (string.IsNullOrEmpty(atual)) break;

                if (validacaoPreviaPendente && tipo == "ia" && passo.Status == "OK")
                {
                    validacaoPreviaPendente = false;
                    if (BuscasAntecipadasFalharam(idConversaIA, executor, grafo, ordemNos, buscasObrigatorias, entradas, saidas, buscasFeitas, outcome, workflow))
                    {
                        erro = true;
                        break;
                    }
                }
            }

            // Saídas por break (erro, pausa, fim) já puseram o passo no trace sem fechar a duração.
            if (passoAberto != null) passoAberto.DuracaoMs = DuracaoDoPasso(cronometro, inicioPasso, duracaoAntecipada);

            if (erro) outcome.Status = "ERRO";
            else if (outcome.Status == "ERRO") outcome.Status = "CONCLUIDO"; // saiu do loop sem erro nem pausa/fim explicito

            // Contexto (saídas) atualizado, para o service persistir/retomar.
            outcome.Saidas = SaidasParaJObject(saidas);

            if (string.IsNullOrEmpty(outcome.Mensagem))
                outcome.Mensagem = outcome.Status == "CONCLUIDO" ? "Workflow concluído." : (outcome.Status == "PAUSADO" ? "Aguardando confirmação de um passo de escrita." : "Workflow interrompido.");

            return outcome;
        }

        // ---- Tempo e progresso ----------------------------------------------------------------------------------------
        // Duração de cada passo no trace, limite de tempo configurável e aviso do passo em andamento para quem espera.

        private static long DuracaoDoPasso(Stopwatch cronometro, long inicioMs, long extraMs)
        {
            return Math.Max(0, cronometro.ElapsedMilliseconds - inicioMs) + Math.Max(0, extraMs);
        }

        // O limite vem de IA.Workflow.TimeoutSegundos (padrão 30). A configuração custa uma ida ao banco, então só é lida
        // quando a passada já passou do menor valor possível; passadas rápidas nunca pagam por isso.
        private static bool TempoEsgotado(long decorridoMs, ref int timeoutMs)
        {
            if (decorridoMs <= TimeoutMinimoMs) return false;
            if (timeoutMs <= 0) timeoutMs = TimeoutConfiguradoMs();
            return decorridoMs > timeoutMs;
        }

        private static int TimeoutConfiguradoMs()
        {
            try
            {
                cls_IA_Config config = cls_IA_Config.Carregar();
                if (config != null && config.WorkflowTimeoutSegundos >= TimeoutMinimoMs / 1000) return config.WorkflowTimeoutSegundos * 1000;
            }
            catch
            {
                // sem configuração legível, vale o padrão
            }
            return TimeoutPadraoMs;
        }

        // Estourou o tempo antes de começar o passo idNo. O passo entra no trace como ERRO (o diagnóstico aponta o nó) e a
        // mensagem diz quanto foi o limite e onde o tempo foi gasto.
        private static void RegistrarTempoEsgotado(IAWorkflowOutcome outcome, JObject noPendente, string idNo, int timeoutMs)
        {
            string titulo = TituloNo(noPendente);
            string mensagem = MensagemTempoEsgotado(outcome.Trace, idNo, titulo, timeoutMs);
            IAWorkflowTracePasso passo = new IAWorkflowTracePasso { No = idNo, Tipo = Str(noPendente["tipo"]).ToLowerInvariant(), Titulo = titulo, Status = "ERRO" };
            DefinirSaida(passo, mensagem);
            outcome.Trace.Add(passo);
            outcome.Mensagem = mensagem;
        }

        internal static string MensagemTempoEsgotado(List<IAWorkflowTracePasso> trace, string idNo, string tituloNo, int timeoutMs)
        {
            StringBuilder texto = new StringBuilder(PrefixoTempoLimite + " (" + FormatarSegundos(timeoutMs) + ") antes de executar o passo " + idNo);
            if (!string.IsNullOrWhiteSpace(tituloNo)) texto.Append(" (").Append(tituloNo).Append(")");
            texto.Append(".");

            List<IAWorkflowTracePasso> lentos = new List<IAWorkflowTracePasso>();
            foreach (IAWorkflowTracePasso p in trace ?? new List<IAWorkflowTracePasso>())
            {
                if (p != null && p.DuracaoMs >= 1000) lentos.Add(p);
            }
            lentos.Sort(delegate (IAWorkflowTracePasso a, IAWorkflowTracePasso b) { return b.DuracaoMs.CompareTo(a.DuracaoMs); });

            if (lentos.Count > 0)
            {
                texto.Append(" Passos mais lentos: ");
                for (int i = 0; i < lentos.Count && i < 3; i++)
                {
                    if (i > 0) texto.Append(", ");
                    texto.Append(lentos[i].No).Append(" ").Append(lentos[i].Titulo).Append(" (").Append(FormatarSegundos(lentos[i].DuracaoMs)).Append(")");
                }
                texto.Append(".");
            }

            texto.Append(" O limite (IA.Workflow.TimeoutSegundos) pode ser aumentado se a demora for da própria consulta.");
            return texto.ToString();
        }

        private static string FormatarSegundos(long ms)
        {
            return (ms / 1000.0).ToString("0.#", CultureInfo.GetCultureInfo("pt-BR")) + " s";
        }

        // Diz a quem espera (o chat em streaming) qual passo está rodando. Só os que fazem trabalho lento (ferramenta e IA).
        private static void AvisarProgresso(string acao, JObject no)
        {
            cls_IA_Progresso.Avisar("status", acao + TituloNo(no) + "...");
        }

        private static JObject SaidasParaJObject(Dictionary<string, JToken> saidas)
        {
            JObject saidasOut = new JObject();
            foreach (var kv in saidas ?? new Dictionary<string, JToken>()) saidasOut[kv.Key] = kv.Value;
            return saidasOut;
        }

        // Próximo nó após um dado nó (para o service retomar depois de executar a escrita).
        public static string ProximoNo(string grafoJson, string idNo)
        {
            try
            {
                JObject grafo = JObject.Parse(grafoJson ?? "{}");
                foreach (JToken t in (grafo["nos"] as JArray) ?? new JArray())
                {
                    JObject no = t as JObject;
                    if (no != null && string.Equals(Str(no["id"]), idNo, StringComparison.OrdinalIgnoreCase))
                        return Str(no["proximo"]);
                }
            }
            catch { }
            return string.Empty;
        }

        public static string ProximoNoAprovacao(string grafoJson, string idNo, bool aprovado)
        {
            try
            {
                JObject grafo = JObject.Parse(grafoJson ?? "{}");
                foreach (JToken t in (grafo["nos"] as JArray) ?? new JArray())
                {
                    JObject no = t as JObject;
                    if (no != null && string.Equals(Str(no["id"]), idNo, StringComparison.OrdinalIgnoreCase))
                        return Str(aprovado ? no["proximo"] : no["seRejeitado"]);
                }
            }
            catch { }
            return string.Empty;
        }

        public static string ProximoNoEspera(string grafoJson, string idNo)
        {
            return ProximoNo(grafoJson, idNo);
        }

        public static string ProximoNoErro(string grafoJson, string idNo)
        {
            try
            {
                JObject grafo = JObject.Parse(grafoJson ?? "{}");
                foreach (JToken t in (grafo["nos"] as JArray) ?? new JArray())
                {
                    JObject no = t as JObject;
                    if (no != null && string.Equals(Str(no["id"]), idNo, StringComparison.OrdinalIgnoreCase))
                        return Str(no["emErro"]);
                }
            }
            catch { }
            return string.Empty;
        }

        public static int RetriesErro(string grafoJson, string idNo)
        {
            try
            {
                JObject grafo = JObject.Parse(grafoJson ?? "{}");
                foreach (JToken t in (grafo["nos"] as JArray) ?? new JArray())
                {
                    JObject no = t as JObject;
                    if (no != null && string.Equals(Str(no["id"]), idNo, StringComparison.OrdinalIgnoreCase))
                        return RetriesErro(no);
                }
            }
            catch { }
            return 0;
        }

        public static void AguardarRetry(string grafoJson, string idNo, int tentativaAtual, int totalTentativas)
        {
            try
            {
                JObject grafo = JObject.Parse(grafoJson ?? "{}");
                foreach (JToken t in (grafo["nos"] as JArray) ?? new JArray())
                {
                    JObject no = t as JObject;
                    if (no != null && string.Equals(Str(no["id"]), idNo, StringComparison.OrdinalIgnoreCase))
                    {
                        AguardarRetry(no, tentativaAtual, totalTentativas);
                        return;
                    }
                }
            }
            catch { }
        }

        public static JObject SaidaErroTratado(string tipoNo, string titulo, string mensagem, int tentativa, int totalTentativas)
        {
            IAWorkflowErroInfo info = ClassificarErro(mensagem);
            return new JObject
            {
                { "erro", true },
                { "mensagem", mensagem ?? string.Empty },
                { "categoria", info.Categoria },
                { "codigo", info.Codigo },
                { "tipo", tipoNo ?? string.Empty },
                { "titulo", titulo ?? string.Empty },
                { "tentativa", tentativa },
                { "totalTentativas", totalTentativas }
            };
        }

        public static JObject DiagnosticarErroWorkflow(List<IAWorkflowTracePasso> trace, string mensagem)
        {
            IAWorkflowTracePasso falha = null;
            if (trace != null)
            {
                for (int i = trace.Count - 1; i >= 0; i--)
                {
                    IAWorkflowTracePasso passo = trace[i];
                    if (passo != null
                        && string.Equals(passo.Status, "ERRO", StringComparison.OrdinalIgnoreCase)
                        && !passo.ErroTratado)
                    {
                        falha = passo;
                        break;
                    }
                }

                if (falha == null)
                {
                    for (int i = trace.Count - 1; i >= 0; i--)
                    {
                        IAWorkflowTracePasso passo = trace[i];
                        if (passo != null && string.Equals(passo.Status, "ERRO", StringComparison.OrdinalIgnoreCase))
                        {
                            falha = passo;
                            break;
                        }
                    }
                }
            }

            string detalhe = PrimeiroTextoNaoVazio(
                falha != null ? falha.SaidaCompleta : null,
                falha != null ? falha.Saida : null,
                mensagem,
                "O workflow foi interrompido sem detalhar a falha."
            );
            IAWorkflowErroInfo info = ClassificarErro(detalhe);
            string causa;
            JArray comoResolver;
            OrientarErroWorkflow(detalhe, info, out causa, out comoResolver);

            JObject diagnostico = new JObject
            {
                { "no", falha != null ? falha.No ?? string.Empty : string.Empty },
                { "tipoNo", falha != null ? falha.Tipo ?? string.Empty : string.Empty },
                { "ferramenta", falha != null ? falha.Titulo ?? string.Empty : string.Empty },
                { "categoria", info.Categoria ?? "execucao" },
                { "codigo", info.Codigo ?? "ERRO_EXECUCAO" },
                { "detalheTecnico", detalhe },
                { "causaProvavel", causa },
                { "comoResolver", comoResolver }
            };

            string entrada = falha != null ? PrimeiroTextoNaoVazio(falha.EntradaCompleta, falha.Entrada) : string.Empty;
            if (!string.IsNullOrWhiteSpace(entrada))
            {
                diagnostico["entradaNo"] = ParseJsonOuTexto(entrada);
            }
            return diagnostico;
        }

        public static string DescreverErroFerramenta(string nomeFerramenta, IAFerramentaResultado resultado)
        {
            string nome = string.IsNullOrWhiteSpace(nomeFerramenta) ? "ferramenta" : nomeFerramenta;
            if (resultado == null)
            {
                return "Ferramenta \"" + nome + "\" falhou: o executor não retornou resultado.";
            }

            string detalhe = PrimeiroTextoNaoVazio(
                resultado.Erro,
                MensagemErroResultadoJson(resultado.ResultadoJson),
                "Falha sem detalhe retornado pelo executor."
            );

            string status = string.IsNullOrWhiteSpace(resultado.Status) ? "ERRO" : resultado.Status;
            return "Ferramenta \"" + nome + "\" falhou. Status: " + status + ". Detalhe: " + detalhe;
        }

        private static string MensagemErroResultadoJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return string.Empty;
            }

            try
            {
                JObject obj = JObject.Parse(json);
                string texto = PrimeiroTextoNaoVazio(
                    Str(obj["erro"]),
                    Str(obj["mensagem"]),
                    Str(obj["message"]),
                    Str(obj["sMsg"]),
                    Str(obj["msg"]),
                    Str(obj["detalhe"]),
                    Str(obj["instrucao"])
                );
                if (!string.IsNullOrWhiteSpace(texto))
                {
                    return texto;
                }

                JToken sucesso = obj["sucesso"];
                if (sucesso != null && sucesso.Type == JTokenType.Boolean && !sucesso.Value<bool>())
                {
                    return "A ferramenta retornou sucesso=false, mas não informou uma mensagem de erro.";
                }
            }
            catch
            {
                return Snippet(json);
            }

            return string.Empty;
        }

        private static string PrimeiroTextoNaoVazio(params string[] valores)
        {
            foreach (string v in valores ?? new string[0])
            {
                if (!string.IsNullOrWhiteSpace(v))
                {
                    return v.Trim();
                }
            }

            return string.Empty;
        }

        public static bool EsperaPodeContinuar(JObject args, out string mensagem)
        {
            args = args ?? new JObject();
            string modo = Str(args["modo"]).Trim().ToLowerInvariant();
            if (modo == "data_hora")
            {
                DateTime data;
                if (!TentarParseDataHora(Str(args["dataHora"]), out data))
                {
                    mensagem = "Data/hora de espera inválida.";
                    return false;
                }

                if (DateTime.Now >= data)
                {
                    mensagem = "Data/hora alcançada; seguindo o fluxo.";
                    return true;
                }

                mensagem = "Aguardando até " + data.ToString("dd/MM/yyyy HH:mm") + ".";
                return false;
            }

            if (modo == "evento")
            {
                mensagem = "Aguardando evento: " + (string.IsNullOrWhiteSpace(Str(args["evento"])) ? "evento externo" : Str(args["evento"])) + ".";
                return false;
            }

            mensagem = "Aguardando continuação manual.";
            return false;
        }

        private static JObject ResolverEntradas(JArray entradas, JObject entradasWf, Dictionary<string, JToken> saidas)
        {
            JObject args = new JObject();
            foreach (JToken t in entradas ?? new JArray())
            {
                JObject e = t as JObject;
                if (e == null) continue;
                string param = Str(e["param"]).Trim();
                if (param.Length == 0) continue;
                string origem = Str(e["origem"]);
                string valor = Str(e["valor"]).Trim();
                if (string.Equals(origem, "entrada", StringComparison.OrdinalIgnoreCase) && valor.Length == 0)
                {
                    valor = param;
                }

                args[param] = ResolverValor(origem, valor, entradasWf, saidas);
            }
            return args;
        }

        private static JObject ArgsEntradasPendentes(string idNo, JObject no, string ferramenta, JArray campos)
        {
            return new JObject
            {
                { "tipo", "ENTRADAS" },
                { "no", idNo ?? string.Empty },
                { "titulo", TituloNo(no) },
                { "ferramenta", ferramenta ?? string.Empty },
                { "mensagem", MensagemEntradasPendentes(ferramenta, campos) },
                { "grupoPrincipal", GrupoPrincipalEntradas(campos) },
                { "campos", campos ?? new JArray() }
            };
        }

        private static JObject ArgsEscolha(string idNo, JObject no, string ferramenta, JObject resultado)
        {
            bool selecaoMultipla = resultado.Value<bool?>("selecaoMultipla") == true;
            JArray opcoes = resultado["opcoes"] as JArray ?? new JArray();
            if (opcoes.Count == 0)
            {
                foreach (JToken item in (resultado["registros"] as JArray) ?? new JArray())
                {
                    JObject registro = item as JObject;
                    if (registro == null) continue;
                    opcoes.Add(new JObject
                    {
                        { "valor", ValorOpcao(registro) },
                        { "rotulo", RotuloOpcao(registro) },
                        { "descricao", DescricaoOpcao(registro) },
                        { "registro", registro.DeepClone() }
                    });
                }
            }

            string termoBuscado = Str(resultado["termo"]).Trim();
            string citacao = termoBuscado.Length > 0 ? " “" + termoBuscado + "”" : string.Empty;
            bool aproximado = resultado.Value<bool?>("aproximado") == true;
            // "mensagem" e o texto para o modelo; "mensagemUsuario" e o que o cartao mostra para a pessoa.
            string mensagemUsuario = selecaoMultipla
                ? string.Empty
                : (aproximado
                    ? "Não encontrei exatamente" + citacao + ". Estes são os registros mais parecidos; escolha o correto para continuar."
                    : "Encontrei mais de uma opção" + (citacao.Length > 0 ? " para" + citacao : string.Empty) + ". Escolha a correta para continuar.");

            return new JObject
            {
                { "tipo", selecaoMultipla ? "SELECAO_MULTIPLA" : "ESCOLHA" },
                { "no", idNo ?? string.Empty },
                { "titulo", TituloNo(no) },
                { "ferramenta", ferramenta ?? string.Empty },
                { "mensagem", selecaoMultipla
                    ? PrimeiroTextoNaoVazio(Str(resultado["mensagem"]), "Selecione os itens opcionais que deseja incluir. Você também pode continuar sem selecionar nenhum.")
                    : PrimeiroTextoNaoVazio(Str(resultado["mensagem"]), "Encontrei mais de uma opção possível. Escolha o registro correto para o workflow continuar.") },
                { "mensagemUsuario", mensagemUsuario },
                { "selecaoMultipla", selecaoMultipla },
                { "opcoes", opcoes },
                { "resultado", resultado.DeepClone() }
            };
        }

        private static string ValorOpcao(JObject registro)
        {
            string[] preferidos = { "idCliente", "idEmpresa", "idVendedor", "idUsuario", "idCondicaoPagamento", "idProduto", "idItem", "idTabelaPreco", "idTabela", "idTipoEnvio", "idTipoOrcamento", "id" };
            foreach (string campo in preferidos)
            {
                string valor = Str(registro[campo]).Trim();
                if (valor.Length > 0 && valor != "0") return campo + ":" + valor;
            }
            return registro.ToString(Formatting.None);
        }

        private static string RotuloOpcao(JObject registro)
        {
            string rotulo = RotuloDescritivo(registro);
            return rotulo.Length > 0 ? rotulo : ValorOpcao(registro);
        }

        private static string DescricaoOpcao(JObject registro)
        {
            List<string> partes = new List<string>();
            string codigo = PrimeiroTextoNaoVazio(Str(registro["sCodigo"]), Str(registro["codigo"]));
            string documento = PrimeiroTextoNaoVazio(Str(registro["sCPF_CNPJ"]), Str(registro["documento"]));
            string cidade = PrimeiroTextoNaoVazio(Str(registro["sCidade"]), Str(registro["cidade"]));
            string login = Str(registro["sLogin"]);
            string email = Str(registro["sEmail"]);
            if (!string.IsNullOrWhiteSpace(codigo)) partes.Add("Código: " + codigo);
            if (!string.IsNullOrWhiteSpace(documento)) partes.Add("Documento: " + documento);
            if (!string.IsNullOrWhiteSpace(cidade)) partes.Add("Cidade: " + cidade);
            if (!string.IsNullOrWhiteSpace(login)) partes.Add("Login: " + login);
            if (!string.IsNullOrWhiteSpace(email)) partes.Add("E-mail: " + email);
            return string.Join(" | ", partes.ToArray());
        }

        private static string MensagemEntradasPendentes(string ferramenta, JArray campos)
        {
            if (string.Equals(ferramenta, "clientes_criar_para_orcamento", StringComparison.OrdinalIgnoreCase)
                || string.Equals(GrupoPrincipalEntradas(campos), "Cadastro do cliente", StringComparison.OrdinalIgnoreCase))
            {
                return "O cliente não foi localizado e precisamos desses dados para criar o cadastro antes de continuar o orçamento.";
            }

            return "Informe os dados necessários para continuar este nó.";
        }

        private static string GrupoPrincipalEntradas(JArray campos)
        {
            Dictionary<string, int> contagem = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (JToken t in campos ?? new JArray())
            {
                JObject campo = t as JObject;
                string grupo = campo != null ? Str(campo["grupo"]).Trim() : string.Empty;
                if (grupo.Length == 0) grupo = "Geral";
                contagem[grupo] = contagem.ContainsKey(grupo) ? contagem[grupo] + 1 : 1;
            }

            string melhor = string.Empty;
            int total = 0;
            foreach (KeyValuePair<string, int> item in contagem)
            {
                if (item.Value > total)
                {
                    melhor = item.Key;
                    total = item.Value;
                }
            }

            return melhor;
        }

        private static JArray EntradasPendentesParaNo(JObject no, IAFerramentaDefinicao def, JObject grafo, JObject entradasWf)
        {
            JArray pendentes = new JArray();
            if (no == null || def == null) return pendentes;

            JObject props = def.SchemaParametros != null ? def.SchemaParametros["properties"] as JObject : null;
            JArray required = def.SchemaParametros != null ? def.SchemaParametros["required"] as JArray : null;
            if (props == null || required == null || required.Count == 0) return pendentes;

            HashSet<string> obrigatorios = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (JToken req in required)
            {
                string nomeReq = Str(req).Trim();
                if (nomeReq.Length > 0) obrigatorios.Add(nomeReq);
            }

            bool possuiPendente = false;
            HashSet<string> vistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (JToken t in (no["entradas"] as JArray) ?? new JArray())
            {
                JObject map = t as JObject;
                if (map == null) continue;

                string param = Str(map["param"]).Trim();
                string origem = Str(map["origem"]).Trim().ToLowerInvariant();
                string nomeEntrada = Str(map["valor"]).Trim();
                if (param.Length == 0 || origem != "entrada")
                {
                    continue;
                }
                if (nomeEntrada.Length == 0)
                {
                    nomeEntrada = param;
                }

                JObject schemaParam = props[param] as JObject ?? new JObject();
                JObject definicaoEntrada = DefinicaoEntrada(grafo, nomeEntrada);
                string obrigatoriedadeEntrada = PrimeiroTextoNaoVazio(Str(definicaoEntrada["obrigatoriedade"]), "obrigatorio");
                bool parametroObrigatorio = obrigatorios.Contains(param);
                bool parametroPermiteVazio = ParametroPermiteVazio(schemaParam);
                bool entradaCondicional = string.Equals(obrigatoriedadeEntrada, "condicional", StringComparison.OrdinalIgnoreCase);
                bool campoBloqueante = parametroObrigatorio && !parametroPermiteVazio && !Bool(map["vazioExplicito"]);

                if (!parametroObrigatorio && !entradaCondicional)
                {
                    continue;
                }
                if (Bool(map["vazioExplicito"]) && ParametroPermiteVazio(schemaParam))
                {
                    continue;
                }

                bool jaInformado = campoBloqueante
                    ? EntradaPossuiValor(entradasWf, nomeEntrada)
                    : EntradaFoiInformada(entradasWf, nomeEntrada);
                if (vistos.Contains(nomeEntrada) || jaInformado)
                {
                    continue;
                }

                vistos.Add(nomeEntrada);
                possuiPendente = true;
                pendentes.Add(new JObject
                {
                    { "nome", nomeEntrada },
                    { "param", param },
                    { "tipo", PrimeiroTextoNaoVazio(Str(definicaoEntrada["tipo"]), TipoSchema(schemaParam), "string") },
                    { "obrigatorio", campoBloqueante },
                    { "grupo", PrimeiroTextoNaoVazio(Str(definicaoEntrada["grupo"]), "Geral") },
                    { "obrigatoriedade", obrigatoriedadeEntrada },
                    { "descricao", PrimeiroTextoNaoVazio(Str(definicaoEntrada["descricao"]), Str(schemaParam["description"])) },
                    { "opcoes", OpcoesEntrada(definicaoEntrada, schemaParam) }
                });
            }

            return possuiPendente ? pendentes : new JArray();
        }

        private static JArray OpcoesEntrada(JObject definicao, JObject schema)
        {
            JArray resultado = new JArray();
            JArray definidas = definicao != null ? definicao["opcoes"] as JArray : null;
            if (definidas != null)
            {
                foreach (JToken opcao in definidas)
                {
                    JObject objeto = opcao as JObject;
                    string valor = Str(objeto != null ? objeto["valor"] : opcao).Trim();
                    if (valor.Length == 0) continue;
                    string rotulo = Str(objeto != null ? objeto["rotulo"] : opcao).Trim();
                    resultado.Add(new JObject
                    {
                        { "valor", valor },
                        { "rotulo", string.IsNullOrWhiteSpace(rotulo) ? valor : rotulo }
                    });
                }
                if (resultado.Count > 0) return resultado;
            }

            foreach (JToken opcao in (schema != null ? schema["enum"] as JArray : null) ?? new JArray())
            {
                if (opcao == null || opcao.Type == JTokenType.Null) continue;
                string valor = Str(opcao).Trim();
                if (valor.Length > 0) resultado.Add(new JObject { { "valor", valor }, { "rotulo", valor } });
            }
            return resultado;
        }

        private static JObject DefinicaoEntrada(JObject grafo, string nomeEntrada)
        {
            string nomeBase = NomeEntradaBase(nomeEntrada);
            foreach (JToken t in (grafo != null ? grafo["entradas"] as JArray : null) ?? new JArray())
            {
                JObject entrada = t as JObject;
                string nome = entrada != null ? Str(entrada["nome"]).Trim() : string.Empty;
                if (entrada != null
                    && (string.Equals(nome, nomeEntrada, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(nome, nomeBase, StringComparison.OrdinalIgnoreCase)))
                {
                    return entrada;
                }
            }

            return new JObject();
        }

        private static string NomeEntradaBase(string nomeEntrada)
        {
            nomeEntrada = (nomeEntrada ?? string.Empty).Trim();
            if (nomeEntrada.StartsWith("entrada.", StringComparison.OrdinalIgnoreCase))
            {
                nomeEntrada = nomeEntrada.Substring("entrada.".Length);
            }

            int ponto = nomeEntrada.IndexOf('.');
            int colchete = nomeEntrada.IndexOf('[');
            int corte = -1;
            if (ponto >= 0 && colchete >= 0) corte = Math.Min(ponto, colchete);
            else if (ponto >= 0) corte = ponto;
            else if (colchete >= 0) corte = colchete;

            return corte >= 0 ? nomeEntrada.Substring(0, corte) : nomeEntrada;
        }

        private static bool EntradaPossuiValor(JObject entradasWf, string nomeEntrada)
        {
            if (entradasWf == null || string.IsNullOrWhiteSpace(nomeEntrada))
            {
                return false;
            }

            JToken valor = ResolverEntradaWorkflow(nomeEntrada, entradasWf);
            if (valor.Type == JTokenType.Null || valor.Type == JTokenType.Undefined)
            {
                return false;
            }

            if (valor.Type == JTokenType.String) return !string.IsNullOrWhiteSpace(valor.ToString());
            if (valor.Type == JTokenType.Array) return ((JArray)valor).Count > 0;
            if (valor.Type == JTokenType.Object) return ((JObject)valor).Count > 0;
            return true;
        }

        private static bool EntradaFoiInformada(JObject entradasWf, string nomeEntrada)
        {
            if (entradasWf == null || string.IsNullOrWhiteSpace(nomeEntrada))
            {
                return false;
            }

            string referencia = (nomeEntrada ?? string.Empty).Trim();
            if (ObjetoTemPropriedade(entradasWf, referencia))
            {
                return true;
            }

            if (referencia.StartsWith("entrada.", StringComparison.OrdinalIgnoreCase))
            {
                referencia = referencia.Substring("entrada.".Length);
            }

            if (ObjetoTemPropriedade(entradasWf, referencia))
            {
                return true;
            }

            try
            {
                return entradasWf.SelectToken(referencia, false) != null;
            }
            catch
            {
                return false;
            }
        }

        private static bool ObjetoTemPropriedade(JObject obj, string nome)
        {
            if (obj == null || string.IsNullOrWhiteSpace(nome))
            {
                return false;
            }

            foreach (JProperty prop in obj.Properties())
            {
                if (string.Equals(prop.Name, nome, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ParametroPermiteVazio(JObject schemaParam)
        {
            if (schemaParam == null) return false;

            string tipo = TipoSchema(schemaParam).ToLowerInvariant();
            if (tipo.IndexOf("string") < 0 && tipo.IndexOf("texto") < 0)
            {
                return false;
            }

            int minLength;
            if (int.TryParse(Str(schemaParam["minLength"]), out minLength) && minLength == 0)
            {
                return true;
            }

            string descricao = Str(schemaParam["description"]);
            return descricao.IndexOf("string vazia", StringComparison.OrdinalIgnoreCase) >= 0
                || descricao.IndexOf("texto vazio", StringComparison.OrdinalIgnoreCase) >= 0
                || descricao.IndexOf("valor vazio", StringComparison.OrdinalIgnoreCase) >= 0
                || descricao.IndexOf("vazia", StringComparison.OrdinalIgnoreCase) >= 0
                || descricao.IndexOf("vazio", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string TipoSchema(JObject schemaParam)
        {
            JToken tipo = schemaParam != null ? schemaParam["type"] : null;
            if (tipo == null) return string.Empty;
            if (tipo.Type == JTokenType.Array)
            {
                List<string> tipos = new List<string>();
                foreach (JToken item in (JArray)tipo) tipos.Add(Str(item));
                return string.Join("|", tipos.ToArray());
            }

            return Str(tipo);
        }

        private static JToken ResolverValor(string origem, string valorCfg, JObject entradasWf, Dictionary<string, JToken> saidas)
        {
            switch ((origem ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "entrada":
                    return ResolverEntradaWorkflow(valorCfg, entradasWf);
                case "passo":
                    return ResolverReferencia(valorCfg, saidas);
                case "token":
                    return JValue.CreateString(ResolverToken(valorCfg));
                default:
                    return JValue.CreateString(valorCfg ?? string.Empty);
            }
        }

        private static JToken ResolverEntradaWorkflow(string referencia, JObject entradasWf)
        {
            referencia = (referencia ?? string.Empty).Trim();
            if (entradasWf == null || referencia.Length == 0)
            {
                return JValue.CreateString(string.Empty);
            }

            if (entradasWf[referencia] != null)
            {
                return entradasWf[referencia];
            }

            if (referencia.StartsWith("entrada.", StringComparison.OrdinalIgnoreCase))
            {
                referencia = referencia.Substring("entrada.".Length);
            }

            if (entradasWf[referencia] != null)
            {
                return entradasWf[referencia];
            }

            return ResolverCaminhoJson(entradasWf, referencia);
        }

        // Alternativas de uma referência: "nA.campo || nB.campo" devolve o primeiro valor não vazio. Serve para o que antes
        // exigia um nó de IA só para escolher entre dois caminhos (ex.: id do cliente criado ou do cliente que já existia;
        // quem não rodou não tem saída e é pulado).
        internal static string[] DividirAlternativas(string referencia)
        {
            List<string> alternativas = new List<string>();
            foreach (string parte in (referencia ?? string.Empty).Split(new[] { "||" }, StringSplitOptions.None))
            {
                string limpa = parte.Trim();
                if (limpa.Length > 0) alternativas.Add(limpa);
            }
            return alternativas.Count > 0 ? alternativas.ToArray() : new[] { string.Empty };
        }

        private static JToken ResolverReferencia(string referencia, Dictionary<string, JToken> saidas)
        {
            string[] alternativas = DividirAlternativas(referencia);
            if (alternativas.Length == 1) return ResolverReferenciaSimples(alternativas[0], saidas);

            JToken resolvido = JValue.CreateString(string.Empty);
            foreach (string alternativa in alternativas)
            {
                resolvido = ResolverReferenciaSimples(alternativa, saidas);
                if (!ValorVazio(resolvido)) return resolvido;
            }
            return resolvido;
        }

        private static JToken ResolverReferenciaSimples(string referencia, Dictionary<string, JToken> saidas)
        {
            referencia = (referencia ?? string.Empty).Trim();
            int idxPonto = referencia.IndexOf('.');
            string idNo = idxPonto >= 0 ? referencia.Substring(0, idxPonto) : referencia;
            string caminho = idxPonto >= 0 ? referencia.Substring(idxPonto + 1) : string.Empty;

            JToken saida;
            if (!saidas.TryGetValue(idNo, out saida) || saida == null) return JValue.CreateString(string.Empty);
            if (string.IsNullOrWhiteSpace(caminho)) return saida;

            JToken valor = ResolverCaminhoJson(saida, caminho);
            if (!ValorVazio(valor)) return valor;

            // Compatibilidade: as ferramentas de busca (tipo de orcamento, cliente, empresa, tabela...) devolvem o
            // registro escolhido em "melhorResultado", e o motor guarda o objeto inteiro como saida do no (ver
            // ExtrairSaida). Grafos antigos leem "nX.idCampo" no topo, que deixou de existir. Tenta o mesmo caminho
            // dentro de melhorResultado. So entra quando o caminho nao resolve e o melhorResultado tem conteudo:
            // busca sem resultado continua vazia, nunca inventa valor.
            JObject objetoSaida = saida as JObject;
            JObject melhor = objetoSaida != null ? objetoSaida["melhorResultado"] as JObject : null;
            if (melhor != null && melhor.HasValues && !caminho.StartsWith("melhorResultado", StringComparison.OrdinalIgnoreCase))
            {
                JToken viaMelhor = ResolverCaminhoJson(melhor, caminho);
                if (!ValorVazio(viaMelhor)) return viaMelhor;
            }

            return valor;
        }

        // Vazio = nulo, indefinido ou texto em branco. ResolverCaminhoJson devolve string vazia quando o caminho nao existe.
        private static bool ValorVazio(JToken valor)
        {
            if (valor == null || valor.Type == JTokenType.Null || valor.Type == JTokenType.Undefined) return true;
            return valor.Type == JTokenType.String && string.IsNullOrWhiteSpace(valor.ToString());
        }

        private static JToken ResolverCaminhoJson(JToken atual, string caminho)
        {
            string[] partes = (caminho ?? string.Empty).Split('.');
            foreach (string bruto in partes)
            {
                string parte = (bruto ?? string.Empty).Trim();
                if (parte.Length == 0) return JValue.CreateString(string.Empty);

                atual = ResolverSegmentoJson(atual, parte);
                if (atual == null) return JValue.CreateString(string.Empty);
            }

            return atual ?? JValue.CreateString(string.Empty);
        }

        private static JToken ResolverSegmentoJson(JToken atual, string segmento)
        {
            int pos = 0;
            while (pos < segmento.Length)
            {
                if (atual == null) return null;

                if (segmento[pos] == '[')
                {
                    int fimIndice = segmento.IndexOf(']', pos + 1);
                    if (fimIndice < 0) return null;

                    int indice;
                    if (!int.TryParse(segmento.Substring(pos + 1, fimIndice - pos - 1), out indice)) return null;

                    JArray arrDireto = atual as JArray;
                    if (arrDireto == null || indice < 0 || indice >= arrDireto.Count) return null;

                    atual = arrDireto[indice];
                    pos = fimIndice + 1;
                    continue;
                }

                int proxColchete = segmento.IndexOf('[', pos);
                string nome = proxColchete >= 0 ? segmento.Substring(pos, proxColchete - pos) : segmento.Substring(pos);
                if (nome.Length == 0) return null;

                JObject obj = atual as JObject;
                if (obj != null)
                {
                    atual = obj[nome];
                    if (atual == null) return null;
                }
                else
                {
                    JArray arr = atual as JArray;
                    int indice;
                    if (arr != null && int.TryParse(nome, out indice) && indice >= 0 && indice < arr.Count)
                    {
                        atual = arr[indice];
                    }
                    else
                    {
                        return null;
                    }
                }

                pos = proxColchete >= 0 ? proxColchete : segmento.Length;
            }

            return atual;
        }

        private static string ResolverToken(string token)
        {
            switch ((token ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "usuariologado":
                case "idusuario":
                    return IDENTITY.Variaveis.idUsuario();
                default:
                    return string.Empty;
            }
        }

        private static bool AvaliarCondicao(JObject no, JObject entradas, Dictionary<string, JToken> saidas)
        {
            string esq = TokenParaTexto(ResolverReferencia(Str(no["campo"]), saidas));
            string origemDir = Str(no["valorOrigem"]);
            string dir = TokenParaTexto(ResolverValor(origemDir.Length > 0 ? origemDir : "fixo", Str(no["valor"]), entradas, saidas));
            string op = Str(no["operador"]).Trim();

            decimal ne = 0, nd = 0;
            bool numerico = decimal.TryParse(esq, NumberStyles.Any, CultureInfo.InvariantCulture, out ne)
                          && decimal.TryParse(dir, NumberStyles.Any, CultureInfo.InvariantCulture, out nd);

            switch (op)
            {
                case "!=": return !esq.Equals(dir, StringComparison.OrdinalIgnoreCase);
                case ">": return numerico && ne > nd;
                case "<": return numerico && ne < nd;
                case ">=": return numerico && ne >= nd;
                case "<=": return numerico && ne <= nd;
                case "contem": return esq.IndexOf(dir, StringComparison.OrdinalIgnoreCase) >= 0;
                default: return esq.Equals(dir, StringComparison.OrdinalIgnoreCase);
            }
        }

        private static JToken ExtrairSaida(string resultadoJson)
        {
            try
            {
                JObject o = JObject.Parse(resultadoJson ?? "{}");
                if (o["melhorResultado"] != null)
                {
                    return o;
                }

                JArray reg = o["registros"] as JArray;
                if (reg != null && reg.Count > 0) return reg[0];
                return o;
            }
            catch { return new JObject(); }
        }

        // ---- Validação prévia das buscas ----------------------------------------------------------------------------
        // Antes de fazer qualquer pergunta ao usuário, confere as buscas (tipo, vendedor, empresa, tabela, envio...) que o
        // workflow vai precisar. Se alguma não achar nada, o workflow para ali com todas as pendências juntas, em vez de
        // pedir uma escolha e só depois descobrir que outro dado já informado não existe.

        private sealed class BuscaAntecipada
        {
            public string IdNo;
            public JObject No;
            public string ArgsJson;
            public IAFerramentaResultado Res;
            public string Termo;
            public string Situacao; // ENCONTRADO | NAO_ENCONTRADO | ESCOLHA
            public long DuracaoMs;  // quanto a consulta levou; o passo que reaproveita o resultado herda esse tempo
            public List<string> Opcoes = new List<string>();
            public int TotalOpcoes;
            public List<string> Consumidores = new List<string>();
        }

        private static List<JObject> ListarNosEmOrdem(JObject grafo)
        {
            List<JObject> lista = new List<JObject>();
            foreach (JToken t in (grafo["nos"] as JArray) ?? new JArray())
            {
                JObject no = t as JObject;
                if (no != null && Str(no["id"]).Length > 0) lista.Add(no);
            }
            return lista;
        }

        private static bool GrafoTemNoIA(List<JObject> ordem)
        {
            foreach (JObject no in ordem)
            {
                if (string.Equals(Str(no["tipo"]), "ia", StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        // Nós de busca cuja falta de resultado já garante que o workflow vai falhar mais adiante: ferramenta de leitura com
        // "termo", sem rota emErro, cuja saída alimenta ao menos um identificador obrigatório de outro nó de ferramenta
        // (também sem emErro). Se uma condição, definição, nó de IA ou loop lê a saída, o autor previu o "não achou" (é o
        // caso do cliente, que leva ao cadastro) e o nó fica de fora. Devolve id do nó -> quem exige o identificador.
        private static Dictionary<string, List<string>> BuscasObrigatorias(List<JObject> ordem, Func<string, IAFerramentaDefinicao> obterDef)
        {
            Dictionary<string, List<string>> resultado = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            try
            {
                foreach (JObject candidato in ordem)
                {
                    if (!string.Equals(Str(candidato["tipo"]), "ferramenta", StringComparison.OrdinalIgnoreCase)) continue;
                    if (Str(candidato["emErro"]).Trim().Length > 0) continue;

                    IAFerramentaDefinicao def = obterDef(Str(candidato["ferramenta"]));
                    if (def == null || def.EhWorkflow() || def.RequerConfirmacao
                        || string.Equals(def.Escopo, "WRITE", StringComparison.OrdinalIgnoreCase)) continue;

                    JObject props = def.SchemaParametros != null ? def.SchemaParametros["properties"] as JObject : null;
                    if (props == null || props["termo"] == null || !MapeiaParametro(candidato, "termo")) continue;

                    string idNo = Str(candidato["id"]);
                    List<string> consumidores = ConsumidoresObrigatorios(idNo, ordem, obterDef);
                    if (consumidores != null && consumidores.Count > 0) resultado[idNo] = consumidores;
                }
            }
            catch
            {
                resultado.Clear(); // qualquer surpresa na análise do grafo: segue sem validação prévia
            }
            return resultado;
        }

        private static bool MapeiaParametro(JObject no, string param)
        {
            foreach (JToken t in (no["entradas"] as JArray) ?? new JArray())
            {
                JObject mapeamento = t as JObject;
                if (mapeamento != null && string.Equals(Str(mapeamento["param"]).Trim(), param, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        private static bool ReferenciaComecaNoNo(string referencia, string idNo)
        {
            foreach (string alternativa in DividirAlternativas(referencia))
            {
                if (alternativa.Equals(idNo, StringComparison.OrdinalIgnoreCase)
                    || alternativa.StartsWith(idNo + ".", StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        // Quem lê a saída do nó, ou null quando algum leitor não é um identificador obrigatório de outro nó de ferramenta.
        private static List<string> ConsumidoresObrigatorios(string idNo, List<JObject> ordem, Func<string, IAFerramentaDefinicao> obterDef)
        {
            List<string> lista = new List<string>();
            Regex leituraNoTexto = new Regex(@"(?<![A-Za-z0-9_])" + Regex.Escape(idNo) + @"\.", RegexOptions.IgnoreCase);

            foreach (JObject outro in ordem)
            {
                string idOutro = Str(outro["id"]);
                if (string.Equals(idOutro, idNo, StringComparison.OrdinalIgnoreCase)) continue;

                if (!string.Equals(Str(outro["tipo"]), "ferramenta", StringComparison.OrdinalIgnoreCase))
                {
                    if (leituraNoTexto.IsMatch(outro.ToString(Formatting.None))) return null;
                    continue;
                }

                IAFerramentaDefinicao defOutro = obterDef(Str(outro["ferramenta"]));
                JObject propsOutro = defOutro != null && defOutro.SchemaParametros != null ? defOutro.SchemaParametros["properties"] as JObject : null;
                foreach (JToken t in (outro["entradas"] as JArray) ?? new JArray())
                {
                    JObject mapeamento = t as JObject;
                    if (mapeamento == null || !string.Equals(Str(mapeamento["origem"]).Trim(), "passo", StringComparison.OrdinalIgnoreCase)) continue;
                    if (!ReferenciaComecaNoNo(Str(mapeamento["valor"]), idNo)) continue;

                    // Quem só repassa o valor a um parâmetro opcional não trata a ausência: não conta, nem atrapalha.
                    string param = Str(mapeamento["param"]).Trim();
                    if (propsOutro == null || !ParametroEhIdObrigatorio(param, propsOutro[param] as JObject)) continue;

                    // O autor previu a falha justamente nesse consumidor (rota emErro): a busca fica com ele.
                    if (Str(outro["emErro"]).Trim().Length > 0) return null;

                    lista.Add("\"" + param + "\" do nó " + idOutro + " (" + TituloNo(outro) + ")");
                }
            }
            return lista;
        }

        // A busca só pode rodar agora se tudo que ela lê de outros nós já existe.
        private static bool DependenciasDisponiveis(JObject no, Dictionary<string, JToken> saidas)
        {
            foreach (JToken t in (no["entradas"] as JArray) ?? new JArray())
            {
                JObject mapeamento = t as JObject;
                if (mapeamento == null || !string.Equals(Str(mapeamento["origem"]).Trim(), "passo", StringComparison.OrdinalIgnoreCase)) continue;

                // Com alternativas ("a || b"), basta que uma delas já tenha saída.
                bool algumaDisponivel = false;
                foreach (string alternativa in DividirAlternativas(Str(mapeamento["valor"])))
                {
                    int ponto = alternativa.IndexOf('.');
                    string idOrigem = ponto >= 0 ? alternativa.Substring(0, ponto) : alternativa;
                    JToken saida;
                    if (saidas.TryGetValue(idOrigem, out saida) && saida != null) { algumaDisponivel = true; break; }
                }
                if (!algumaDisponivel) return false;
            }
            return true;
        }

        private static List<BuscaAntecipada> ExecutarBuscasAntecipadas(int idConversaIA, cls_IA_ToolExecutor executor, JObject grafo, List<JObject> ordem,
            Dictionary<string, List<string>> buscas, JObject entradas, Dictionary<string, JToken> saidas)
        {
            List<BuscaAntecipada> lista = new List<BuscaAntecipada>();
            foreach (JObject no in ordem)
            {
                string idNo = Str(no["id"]);
                List<string> consumidores;
                if (!buscas.TryGetValue(idNo, out consumidores)) continue;
                if (saidas.ContainsKey(idNo) || !DependenciasDisponiveis(no, saidas)) continue;

                string nome = Str(no["ferramenta"]);
                IAFerramentaDefinicao def = cls_IA_ToolRegistry.Obter(nome);
                if (def == null || !string.IsNullOrWhiteSpace(cls_IA_ToolRegistry.DiagnosticarDisponibilidade(def, true))) continue;
                if (EntradasPendentesParaNo(no, def, grafo, entradas).Count > 0) continue;

                JObject args = ResolverEntradas(no["entradas"] as JArray, entradas, saidas);
                IAFerramentaResultado res;
                AvisarProgresso("Conferindo ", no);
                Stopwatch cronometroBusca = Stopwatch.StartNew();
                try { res = executor.Executar(idConversaIA, new IAToolCall { Nome = nome, Argumentos = args }); }
                catch { continue; }
                long duracaoBusca = cronometroBusca.ElapsedMilliseconds;

                // Erro da ferramenta (termo curto demais, permissão...) não é "não encontrado": o passo normal do
                // workflow trata e reporta como sempre.
                if (res == null || res.Status != "SUCESSO") continue;

                JObject saida = ExtrairSaida(res.ResultadoJson) as JObject;
                if (saida == null) continue;

                BuscaAntecipada busca = new BuscaAntecipada
                {
                    IdNo = idNo,
                    No = no,
                    ArgsJson = args.ToString(Formatting.None),
                    Res = res,
                    Termo = PrimeiroTextoNaoVazio(Str(saida["termo"]).Trim(), Str(args["termo"]).Trim()),
                    Consumidores = consumidores,
                    Situacao = "ENCONTRADO",
                    DuracaoMs = duracaoBusca
                };

                try
                {
                    if (saida.Value<bool?>("precisaEscolha") == true)
                    {
                        busca.Situacao = "ESCOLHA";
                        busca.TotalOpcoes = saida.Value<int?>("totalEncontrado") ?? 0;
                        foreach (JToken r in (saida["registros"] as JArray) ?? new JArray())
                        {
                            JObject registro = r as JObject;
                            if (registro != null && busca.Opcoes.Count < 4) busca.Opcoes.Add(RotuloOpcao(registro));
                        }
                    }
                    else if (saida.Value<bool?>("encontrado") == false)
                    {
                        busca.Situacao = "NAO_ENCONTRADO";
                    }
                }
                catch
                {
                    continue; // formato de saída inesperado: não decide nada por conta própria
                }

                lista.Add(busca);
            }
            return lista;
        }

        private static string MensagemBuscasAntecipadas(List<BuscaAntecipada> buscas)
        {
            List<string> frases = new List<string>();
            List<string> escolhas = new List<string>();
            foreach (BuscaAntecipada busca in buscas)
            {
                string termo = busca.Termo.Length > 0 ? " para \"" + busca.Termo + "\"" : string.Empty;
                if (busca.Situacao == "NAO_ENCONTRADO")
                {
                    frases.Add("O passo " + busca.IdNo + " (" + TituloNo(busca.No) + ") não encontrou nenhum registro" + termo
                        + "; ele alimenta " + string.Join(", ", busca.Consumidores.ToArray()) + ".");
                }
                else if (busca.Situacao == "ESCOLHA")
                {
                    string opcoes = string.Join(", ", busca.Opcoes.ToArray()) + (busca.TotalOpcoes > busca.Opcoes.Count ? ", ..." : string.Empty);
                    escolhas.Add(TituloNo(busca.No) + termo + " (" + busca.TotalOpcoes + " opções: " + opcoes + ")");
                }
            }

            if (frases.Count == 0) return string.Empty;

            string mensagem = PrefixoRegistroNaoEncontrado + " " + string.Join(" ", frases.ToArray());
            if (escolhas.Count > 0)
            {
                mensagem += " Depois de corrigir isso, ainda será preciso escolher: " + string.Join("; ", escolhas.ToArray()) + ".";
            }
            return mensagem;
        }

        // Roda as buscas antecipadas. Devolve true (e registra o passo com erro no trace) quando alguma não achou
        // nenhum registro; nesse caso o workflow para. Quando está tudo certo, guarda os resultados para o passo normal
        // reaproveitar em vez de consultar de novo. Qualquer falha inesperada aqui só desliga a validação prévia.
        private bool BuscasAntecipadasFalharam(int idConversaIA, cls_IA_ToolExecutor executor, JObject grafo, List<JObject> ordem,
            Dictionary<string, List<string>> buscas, JObject entradas, Dictionary<string, JToken> saidas,
            Dictionary<string, BuscaAntecipada> cache, IAWorkflowOutcome outcome, IAWorkflow workflow)
        {
            try
            {
                List<BuscaAntecipada> lista = ExecutarBuscasAntecipadas(idConversaIA, executor, grafo, ordem, buscas, entradas, saidas);
                string mensagem = MensagemBuscasAntecipadas(lista);
                if (mensagem.Length == 0)
                {
                    foreach (BuscaAntecipada busca in lista) cache[busca.IdNo] = busca;
                    return false;
                }

                BuscaAntecipada primeira = lista.Find(delegate (BuscaAntecipada b) { return b.Situacao == "NAO_ENCONTRADO"; });
                IAWorkflowTracePasso passo = new IAWorkflowTracePasso { No = primeira.IdNo, Tipo = "ferramenta", Titulo = TituloNo(primeira.No), Status = "ERRO", DuracaoMs = primeira.DuracaoMs };
                DefinirEntrada(passo, primeira.ArgsJson);
                DefinirSaida(passo, mensagem);
                outcome.Trace.Add(passo);
                cls_IA_Auditoria.Registrar(idConversaIA, "WORKFLOW_VALIDACAO_PREVIA", "ALERTA", new { workflow = workflow != null ? workflow.Nome : string.Empty, mensagem = mensagem });
                return true;
            }
            catch
            {
                cache.Clear();
                return false;
            }
        }

        // Resultado da busca antecipada para o nó, se os argumentos ainda são os mesmos com que ela rodou. duracaoMs é
        // quanto a consulta levou (0 quando não há resultado a reaproveitar).
        private static IAFerramentaResultado TomarBuscaAntecipada(Dictionary<string, BuscaAntecipada> cache, string idNo, JObject args, out long duracaoMs)
        {
            duracaoMs = 0;
            BuscaAntecipada busca;
            if (cache == null || !cache.TryGetValue(idNo, out busca)) return null;

            cache.Remove(idNo);
            if (!string.Equals(busca.ArgsJson, args.ToString(Formatting.None), StringComparison.Ordinal)) return null;

            duracaoMs = busca.DuracaoMs;
            return busca.Res;
        }

        // ---- Resumo do que uma escrita pausada vai gravar -------------------------------------------------------------
        // Linhas { rotulo, valor } para o cartão de confirmação. Segue a ordem dos parâmetros do schema da ferramenta;
        // identificadores só aparecem com o nome do registro (achado na saída dos passos anteriores) e o que é técnico,
        // vazio ou zero fica de fora.

        private static readonly string[] CamposRotulo =
        {
            "sRazaoSocial", "sNomeFantasia", "sDscUsuario", "sNome", "nome", "sDscTabela", "sDscTipoOrcamento", "sDscTipoEnvio",
            "sDscCondicaoPagamento", "sDscEmpresa", "sDscEmpresaReduzida", "sDscParceiro", "sDscProduto", "sDescricao",
            "descricao", "sCodigoComDescricao", "sCodigo"
        };

        private static readonly Dictionary<string, string> RotulosParametros = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "idCliente", "Cliente" }, { "idTipoOrcamento", "Tipo de orçamento" }, { "idFluxo", "Fluxo" }, { "idVendedor", "Responsável" },
            { "idCondicaoPagamento", "Condição de pagamento" }, { "idEmpresa", "Empresa" }, { "idTabelaPreco", "Tabela de preço" },
            { "idTipoEnvio", "Tipo de envio" }, { "idEnderecoFiscal", "Endereço fiscal" }, { "idEnderecoEntrega", "Endereço de entrega" },
            { "idContatoCliente", "Contato" }, { "idProduto", "Produto" }, { "idOrcamento", "Orçamento" }, { "referencia", "Referência" },
            { "dataEstimativaEntrega", "Entrega estimada" }, { "validadeDias", "Validade (dias)" }, { "observacao", "Observação" },
            { "quantidade", "Quantidade" }, { "valorUnitario", "Valor unitário" }, { "valorTotal", "Valor total" },
            { "descontoPercentual", "Desconto (%)" }, { "tipoItem", "Tipo do item" }, { "unidade", "Unidade" }, { "descricao", "Descrição" },
            { "razaoSocial", "Razão social" }, { "nomeFantasia", "Nome fantasia" }, { "documento", "Documento" }, { "contatoNome", "Contato" },
            { "contatoEmail", "E-mail" }, { "contatoTelefone", "Telefone" }, { "cep", "CEP" }, { "logradouro", "Logradouro" },
            { "numero", "Número" }, { "complemento", "Complemento" }, { "bairro", "Bairro" }, { "cidade", "Cidade" }, { "uf", "UF" },
            { "clienteNome", "Cliente" }, { "nNumeroOrcamento", "Nº do orçamento" }, { "chance", "Chance" }, { "dataPrevisao", "Previsão" }
        };

        private static readonly HashSet<string> ParametrosForaDoResumo = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "observacaoHistorico", "confidencial", "destinoVenda", "empresaTransporte", "ufFiscal", "ufEntrega", "ordem",
            "idProdutoPai", "idProdutoAvo", "idProdutoBisavo", "margemPercentual", "ajustePercentual", "diasPrevisaoEntrega",
            "diasPrevisao", "codigoProduto", "fretePrevisto"
        };

        private static JArray MontarResumoEscritaSeguro(JObject no, IAFerramentaDefinicao def, JObject args, Dictionary<string, JToken> saidas)
        {
            try { return MontarResumoEscrita(no, def, args, saidas); }
            catch { return new JArray(); } // o cartão de confirmação continua funcionando sem o resumo
        }

        private static JArray MontarResumoEscrita(JObject no, IAFerramentaDefinicao def, JObject args, Dictionary<string, JToken> saidas)
        {
            JArray linhas = new JArray();
            JObject props = def != null && def.SchemaParametros != null ? def.SchemaParametros["properties"] as JObject : null;
            if (props == null || args == null) return linhas;

            HashSet<string> rotulosUsados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> textosLongosUsados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (JProperty propriedade in props.Properties())
            {
                if (linhas.Count >= 12) break;

                string param = propriedade.Name;
                if (ParametrosForaDoResumo.Contains(param) || param.StartsWith("sid", StringComparison.OrdinalIgnoreCase)) continue;

                string texto = ValorParaResumo(args[param]);
                if (ValorSemInteresse(texto)) continue;

                string exibicao = texto;
                if (param.Length > 2 && param.StartsWith("id", StringComparison.Ordinal) && char.IsUpper(param[2]))
                {
                    exibicao = RotuloDoId(no, param, texto, saidas);
                    if (exibicao.Length == 0) continue; // id sem nome conhecido é técnico demais para o cartão
                }

                // Sem repetição: o número do orçamento já aparece em "Orçamento: nº ..."; o mesmo rótulo só entra uma vez
                // (idCliente e clienteNome são ambos "Cliente"); e o mesmo texto longo não se repete (produto e descrição).
                string rotulo = RotuloParametro(param);
                if (string.Equals(param, "nNumeroOrcamento", StringComparison.OrdinalIgnoreCase) && rotulosUsados.Contains("Orçamento")) continue;
                if (rotulosUsados.Contains(rotulo)) continue;
                if (exibicao.Length >= 12 && textosLongosUsados.Contains(exibicao)) continue;

                rotulosUsados.Add(rotulo);
                if (exibicao.Length >= 12) textosLongosUsados.Add(exibicao);
                linhas.Add(new JObject { { "rotulo", rotulo }, { "valor", exibicao } });
            }
            return linhas;
        }

        private static string RotuloParametro(string param)
        {
            string rotulo;
            if (RotulosParametros.TryGetValue(param, out rotulo)) return rotulo;

            string nome = param.Length > 2 && param.StartsWith("id", StringComparison.Ordinal) && char.IsUpper(param[2]) ? param.Substring(2) : param;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < nome.Length; i++)
            {
                char c = nome[i];
                if (i == 0) sb.Append(char.ToUpperInvariant(c));
                else if (char.IsUpper(c) && !char.IsUpper(nome[i - 1])) sb.Append(' ').Append(char.ToLowerInvariant(c));
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private static string ValorParaResumo(JToken valor)
        {
            if (valor == null || valor.Type == JTokenType.Null) return string.Empty;

            string texto = Str(valor).Trim();
            DateTime data;
            if (Regex.IsMatch(texto, @"^\d{4}-\d{2}-\d{2}$")
                && DateTime.TryParseExact(texto, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out data))
            {
                return data.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            return texto.Length > 160 ? texto.Substring(0, 157) + "..." : texto;
        }

        private static bool ValorSemInteresse(string texto)
        {
            return texto.Length == 0 || texto == "0" || texto == "0.0" || texto == "0.00" || texto == "0,00";
        }

        // Só o rótulo que descreve o registro (nome, descrição...), sem o "campo:valor" de reserva das opções.
        private static string RotuloDescritivo(JObject registro)
        {
            if (registro == null) return string.Empty;
            foreach (string campo in CamposRotulo)
            {
                string valor = Str(registro[campo]).Trim();
                if (valor.Length > 0) return valor;
            }
            return string.Empty;
        }

        // Nome do registro que um identificador aponta: primeiro pelo próprio mapeamento do parâmetro (a saída do passo de
        // origem), depois por qualquer saída que traga o mesmo identificador junto de um nome.
        private static string RotuloDoId(JObject no, string param, string valorId, Dictionary<string, JToken> saidas)
        {
            JObject mapeamento = null;
            foreach (JToken t in (no["entradas"] as JArray) ?? new JArray())
            {
                JObject m = t as JObject;
                if (m != null && string.Equals(Str(m["param"]).Trim(), param, StringComparison.OrdinalIgnoreCase)) { mapeamento = m; break; }
            }

            if (mapeamento != null && string.Equals(Str(mapeamento["origem"]).Trim(), "passo", StringComparison.OrdinalIgnoreCase))
            {
                foreach (string referencia in DividirAlternativas(Str(mapeamento["valor"])))
                {
                    int ponto = referencia.IndexOf('.');
                    string idOrigem = ponto >= 0 ? referencia.Substring(0, ponto) : referencia;
                    string caminho = ponto >= 0 ? referencia.Substring(ponto + 1) : string.Empty;

                    JToken saidaOrigem;
                    if (!saidas.TryGetValue(idOrigem, out saidaOrigem) || saidaOrigem == null) continue;

                    int ultimo = caminho.LastIndexOf('.');
                    string pai = ultimo >= 0 ? caminho.Substring(0, ultimo) : string.Empty;
                    JObject container = (pai.Length == 0 ? saidaOrigem : ResolverCaminhoJson(saidaOrigem, pai)) as JObject;

                    if (container != null && string.Equals(param, "idOrcamento", StringComparison.OrdinalIgnoreCase))
                    {
                        string numero = Str(container["nNumeroOrcamento"]).Trim();
                        if (numero.Length > 0 && numero != "0") return "nº " + numero;
                    }

                    string rotulo = RotuloDescritivo(container);
                    if (rotulo.Length > 0) return rotulo;
                }
            }

            foreach (KeyValuePair<string, JToken> saida in saidas)
            {
                JObject raiz = saida.Value as JObject;
                if (raiz == null) continue;

                JObject[] candidatos =
                {
                    raiz,
                    raiz["melhorResultado"] as JObject,
                    raiz["dados"] as JObject,
                    (raiz["registros"] as JArray) != null && ((JArray)raiz["registros"]).Count > 0 ? ((JArray)raiz["registros"])[0] as JObject : null
                };
                foreach (JObject candidato in candidatos)
                {
                    if (candidato == null || !string.Equals(Str(candidato[param]).Trim(), valorId, StringComparison.Ordinal)) continue;

                    string rotulo = RotuloDescritivo(candidato);
                    if (rotulo.Length > 0) return rotulo;
                }
            }
            return string.Empty;
        }

        // Identificador obrigatório do schema: inteiro (sem null) com mínimo >= 1. É o caso em que o executor sempre
        // recusa o valor vazio ("idX inválido" / "Informe ..."). Texto fica de fora: alguns executores completam o
        // valor sozinhos (ex.: logradouro e cidade a partir do CEP), então vazio ali nem sempre é erro.
        // Identificador de negócio obrigatório: nome no formato idXxx e schema de inteiro obrigatório com mínimo >= 1.
        // Números que não são ids (limite, validadeDias...) ficam de fora: o executor pode ter valor padrão para eles.
        internal static bool ParametroEhIdObrigatorio(string param, JObject schemaParam)
        {
            return param != null && param.Length > 2 && param.StartsWith("id", StringComparison.Ordinal) && char.IsUpper(param[2])
                && ParametroIdObrigatorio(schemaParam);
        }

        internal static bool ParametroIdObrigatorio(JObject schemaParam)
        {
            if (schemaParam == null) return false;

            string tipos = "|" + TipoSchema(schemaParam).ToLowerInvariant() + "|";
            if (tipos.Contains("|null|") || (!tipos.Contains("|integer|") && !tipos.Contains("|number|"))) return false;

            JToken minimo = schemaParam["minimum"];
            return minimo != null
                && (minimo.Type == JTokenType.Integer || minimo.Type == JTokenType.Float)
                && minimo.Value<double>() >= 1;
        }

        // Antes de executar (ou de pedir confirmação de) uma ferramenta, confere se algum identificador obrigatório
        // mapeado de OUTRO passo chegou vazio. Sem isso o executor só responde "idX inválido", e o erro parece ser dos
        // dados digitados pelo usuário quando na verdade é o mapeamento do grafo ou uma busca sem resultado.
        // Devolve a mensagem de erro, ou vazio quando está tudo certo.
        private static string ReferenciaObrigatoriaVazia(JObject no, string idNo, IAFerramentaDefinicao def, JObject args, Dictionary<string, JObject> nos, Dictionary<string, JToken> saidas)
        {
            JObject props = def != null && def.SchemaParametros != null ? def.SchemaParametros["properties"] as JObject : null;
            JArray mapeamentos = no != null ? no["entradas"] as JArray : null;
            if (props == null || mapeamentos == null || args == null) return string.Empty;

            List<string> problemas = new List<string>();
            bool soNaoEncontrado = true;
            foreach (JToken t in mapeamentos)
            {
                JObject mapeamento = t as JObject;
                if (mapeamento == null || !string.Equals(Str(mapeamento["origem"]).Trim(), "passo", StringComparison.OrdinalIgnoreCase)) continue;

                string param = Str(mapeamento["param"]).Trim();
                if (param.Length == 0 || !ParametroEhIdObrigatorio(param, props[param] as JObject) || !ValorVazio(args[param])) continue;

                bool naoEncontrado;
                problemas.Add(DescreverReferenciaVazia(idNo, no, param, Str(mapeamento["valor"]).Trim(), nos, saidas, out naoEncontrado));
                if (!naoEncontrado) soNaoEncontrado = false;
            }

            if (problemas.Count == 0) return string.Empty;
            return (soNaoEncontrado ? PrefixoRegistroNaoEncontrado : PrefixoReferenciaVazia) + " " + string.Join(" ", problemas.ToArray());
        }

        private static string DescreverReferenciaVazia(string idNo, JObject no, string param, string referencia, Dictionary<string, JObject> nos, Dictionary<string, JToken> saidas, out bool naoEncontrado)
        {
            naoEncontrado = false;

            if (DividirAlternativas(referencia).Length > 1)
            {
                return "O parâmetro \"" + param + "\" do nó " + idNo + " (" + TituloNo(no) + ") veio vazio de \"" + referencia
                    + "\": nenhuma das alternativas tem valor.";
            }

            int idxPonto = referencia.IndexOf('.');
            string idOrigem = idxPonto >= 0 ? referencia.Substring(0, idxPonto) : referencia;
            JObject noOrigem = null;
            if (nos != null) nos.TryGetValue(idOrigem, out noOrigem);
            string passoOrigem = "passo " + idOrigem + (noOrigem != null ? " (" + TituloNo(noOrigem) + ")" : string.Empty);
            string destino = "parâmetro \"" + param + "\" do nó " + idNo + " (" + TituloNo(no) + ")";

            JToken saidaOrigem = null;
            if (saidas != null) saidas.TryGetValue(idOrigem, out saidaOrigem);
            if (saidaOrigem == null)
            {
                return "O " + destino + " depende de \"" + referencia + "\", mas o " + passoOrigem + " não tem saída (não foi executado ou o id do nó está errado no mapeamento).";
            }

            // Busca sem resultado (as ferramentas de resolução devolvem encontrado=false e melhorResultado vazio).
            JObject objeto = saidaOrigem as JObject;
            JToken encontrado = objeto != null ? objeto["encontrado"] : null;
            if (encontrado != null && encontrado.Type == JTokenType.Boolean && !encontrado.Value<bool>() && !Bool(objeto["precisaEscolha"]))
            {
                naoEncontrado = true;
                string termo = Str(objeto["termo"]).Trim();
                return "O " + passoOrigem + " não encontrou nenhum registro" + (termo.Length > 0 ? " para \"" + termo + "\"" : string.Empty) + "; por isso o " + destino + " ficou vazio.";
            }

            string campos = CamposDisponiveis(saidaOrigem);
            return "O " + destino + " veio vazio de \"" + referencia + "\""
                + (campos.Length > 0 ? ": o " + passoOrigem + " devolveu os campos " + campos + ". Confira o caminho no mapeamento." : ".");
        }

        // Nomes dos campos que a saída de um passo realmente tem (abrindo um nível de objeto, como melhorResultado),
        // para quem mantém o workflow ver o que dá para referenciar.
        private static string CamposDisponiveis(JToken saida)
        {
            JObject objeto = saida as JObject;
            if (objeto == null) return string.Empty;

            List<string> nomes = new List<string>();
            foreach (JProperty propriedade in objeto.Properties())
            {
                if (nomes.Count >= 20) { nomes.Add("..."); break; }

                JObject filho = propriedade.Value as JObject;
                if (filho == null || !filho.HasValues)
                {
                    nomes.Add(propriedade.Name);
                    continue;
                }

                List<string> internos = new List<string>();
                foreach (JProperty interno in filho.Properties())
                {
                    if (internos.Count >= 12) { internos.Add("..."); break; }
                    internos.Add(interno.Name);
                }
                nomes.Add(propriedade.Name + " (" + string.Join(", ", internos.ToArray()) + ")");
            }
            return string.Join(", ", nomes.ToArray());
        }

        private static bool ComecaCom(string mensagem, string prefixo)
        {
            return (mensagem ?? string.Empty).TrimStart().StartsWith(prefixo, StringComparison.OrdinalIgnoreCase);
        }

        private static bool EncaminharErroTratado(JObject no, string idNo, string mensagem,Dictionary<string, JObject> nos, Dictionary<string, JToken> saidas, IAWorkflowTracePasso passo, out string proximo, int tentativa, int totalTentativas)
        {
            proximo = Str(no["emErro"]);
            if (string.IsNullOrWhiteSpace(proximo) || nos == null || !nos.ContainsKey(proximo))
            {
                return false;
            }

            saidas[idNo] = SaidaErroTratado(Str(no["tipo"]), TituloNo(no), mensagem, tentativa, totalTentativas);
            passo.Status = "ERRO";
            passo.ErroTratado = true;
            DefinirSaida(passo, MensagemErroComTentativa(mensagem, tentativa, totalTentativas) + " Encaminhado para o tratamento de erro.");
            return true;
        }

        private static int RetriesErro(JObject no)
        {
            int retries;
            if (!int.TryParse(Str(no["retries"]), out retries))
            {
                int.TryParse(Str(no["tentativasErro"]), out retries);
            }

            if (retries < 0) return 0;
            return Math.Min(retries, MaxRetriesErro);
        }

        private static int IntervaloRetrySegundos(JObject no)
        {
            int segundos;
            if (!int.TryParse(Str(no["intervaloRetrySegundos"]), out segundos))
            {
                int.TryParse(Str(no["intervaloRetry"]), out segundos);
            }

            if (segundos < 0) return 0;
            return Math.Min(segundos, MaxRetryIntervaloSegundos);
        }

        private static bool RetryBackoff(JObject no)
        {
            string valor = Str(no["backoffRetry"]).Trim();
            return valor.Equals("S", StringComparison.OrdinalIgnoreCase)
                || valor.Equals("true", StringComparison.OrdinalIgnoreCase)
                || valor.Equals("1", StringComparison.OrdinalIgnoreCase);
        }

        private static void AguardarRetry(JObject no, int tentativaAtual, int totalTentativas)
        {
            if (tentativaAtual <= 0 || tentativaAtual >= totalTentativas)
            {
                return;
            }

            int segundos = IntervaloRetrySegundos(no);
            if (segundos <= 0)
            {
                return;
            }

            int multiplicador = RetryBackoff(no) ? tentativaAtual : 1;
            int esperaSegundos = Math.Min(segundos * multiplicador, MaxRetryIntervaloSegundos);
            if (esperaSegundos > 0)
            {
                Thread.Sleep(esperaSegundos * 1000);
            }
        }

        private static string MensagemErroComTentativa(string mensagem, int tentativa, int totalTentativas)
        {
            mensagem = mensagem ?? "Erro no nó.";
            if (totalTentativas <= 1)
            {
                return mensagem;
            }

            return mensagem + " Tentativas realizadas: " + tentativa + " de " + totalTentativas + ".";
        }

        private static IAWorkflowErroInfo ClassificarErro(string mensagem)
        {
            string m = (mensagem ?? string.Empty).ToLowerInvariant();
            if (ComecaCom(mensagem, PrefixoRegistroNaoEncontrado))
            {
                return new IAWorkflowErroInfo { Categoria = "nao_encontrado", Codigo = "REGISTRO_NAO_ENCONTRADO" };
            }
            if (ComecaCom(mensagem, PrefixoReferenciaVazia))
            {
                return new IAWorkflowErroInfo { Categoria = "referencia", Codigo = "REFERENCIA_VAZIA" };
            }
            if (ComecaCom(mensagem, PrefixoNoIASemDados))
            {
                return new IAWorkflowErroInfo { Categoria = "ia", Codigo = "NO_IA_SEM_DADOS" };
            }
            if (ComecaCom(mensagem, PrefixoTempoLimite))
            {
                // Antes das heurísticas por palavra: a mensagem cita nomes de ferramentas, que podem casar com elas.
                return new IAWorkflowErroInfo { Categoria = "timeout", Codigo = "TIMEOUT" };
            }
            if (m.IndexOf("permiss", StringComparison.OrdinalIgnoreCase) >= 0 || m.IndexOf("negado", StringComparison.OrdinalIgnoreCase) >= 0 || m.IndexOf("acesso", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return new IAWorkflowErroInfo { Categoria = "permissao", Codigo = "PERMISSAO_NEGADA" };
            }
            if (m.IndexOf("valid", StringComparison.OrdinalIgnoreCase) >= 0
                || m.IndexOf("obrigat", StringComparison.OrdinalIgnoreCase) >= 0
                || m.IndexOf("inválid", StringComparison.OrdinalIgnoreCase) >= 0
                || m.IndexOf("inval", StringComparison.OrdinalIgnoreCase) >= 0
                || m.IndexOf("informe", StringComparison.OrdinalIgnoreCase) >= 0
                || m.IndexOf("ao menos", StringComparison.OrdinalIgnoreCase) >= 0
                || m.IndexOf("vazio", StringComparison.OrdinalIgnoreCase) >= 0
                || m.IndexOf("mínim", StringComparison.OrdinalIgnoreCase) >= 0
                || m.IndexOf("minim", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return new IAWorkflowErroInfo { Categoria = "validacao", Codigo = "VALIDACAO" };
            }
            if (m.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0 || m.IndexOf("tempo limite", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return new IAWorkflowErroInfo { Categoria = "timeout", Codigo = "TIMEOUT" };
            }
            if (m.IndexOf("indispon", StringComparison.OrdinalIgnoreCase) >= 0 || m.IndexOf("não localizada", StringComparison.OrdinalIgnoreCase) >= 0 || m.IndexOf("nao localizada", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return new IAWorkflowErroInfo { Categoria = "indisponivel", Codigo = "INDISPONIVEL" };
            }
            return new IAWorkflowErroInfo { Categoria = "execucao", Codigo = "ERRO_EXECUCAO" };
        }

        private static void OrientarErroWorkflow(string mensagem, IAWorkflowErroInfo info, out string causa, out JArray comoResolver)
        {
            string m = (mensagem ?? string.Empty).ToLowerInvariant();
            comoResolver = new JArray();

            if (ComecaCom(mensagem, PrefixoRegistroNaoEncontrado))
            {
                causa = "Uma ou mais buscas do workflow não encontraram nenhum registro para o texto informado, e o passo que depende delas ficaria sem um identificador obrigatório. Os demais dados informados pelo usuário não são o problema.";
                comoResolver.Add("Peça ao usuário para conferir apenas os dados citados no detalhe técnico (nome ou código) ou informar outra parte deles, mais curta ou mais específica.");
                comoResolver.Add("Se o detalhe citar escolhas que ainda serão necessárias, avise que elas serão pedidas depois desses ajustes.");
                comoResolver.Add("Não peça de novo os dados que ele já informou; reaproveite-os ao executar o workflow outra vez.");
                return;
            }
            if (ComecaCom(mensagem, PrefixoNoIASemDados))
            {
                causa = "O passo de IA do workflow, que interpreta os dados do pedido, não devolveu o formato esperado (JSON), mesmo depois de uma nova tentativa. Não é erro dos dados informados pelo usuário.";
                comoResolver.Add("Peça ao usuário para tentar de novo em instantes, sem redigitar os dados que já informou.");
                comoResolver.Add("Se repetir, encaminhe ao suporte o nó indicado e o modelo de IA configurado para que revisem o prompt e o provedor.");
                return;
            }
            if (ComecaCom(mensagem, PrefixoTempoLimite))
            {
                causa = "O workflow passou do tempo limite antes de terminar: algum passo (consulta ou nó de IA) demorou mais que o esperado. Não é erro dos dados informados pelo usuário.";
                comoResolver.Add("Peça ao usuário para tentar de novo em instantes, sem redigitar os dados que já informou.");
                comoResolver.Add("Se repetir, encaminhe ao suporte os passos mais lentos citados no detalhe técnico (consulta ou procedure lenta) ou aumente o limite IA.Workflow.TimeoutSegundos.");
                return;
            }
            if (ComecaCom(mensagem, PrefixoReferenciaVazia))
            {
                causa = "Um identificador obrigatório chegou vazio de outro passo do workflow. É falha no mapeamento entre os passos, não erro de digitação do usuário.";
                comoResolver.Add("Não peça ao usuário para redigitar dados que ele já informou.");
                comoResolver.Add("Se o detalhe técnico indicar que uma busca não encontrou registro, peça ao usuário para conferir apenas esse dado.");
                comoResolver.Add("Nos demais casos, encaminhe ao suporte o nó, o passo de origem e os campos listados no detalhe técnico para corrigir a referência no workflow.");
                return;
            }
            if (m.Contains("permiss") || m.Contains("acesso negado") || m.Contains("sem acesso"))
            {
                causa = "O usuário atual provavelmente não possui a permissão exigida por esta etapa.";
                comoResolver.Add("Solicite a liberação da permissão indicada no erro para o usuário que executa o workflow.");
                comoResolver.Add("Depois da liberação, execute o workflow novamente.");
                return;
            }
            if (m.Contains("inativ") || m.Contains("indispon") || m.Contains("não localizada no catálogo") || m.Contains("nao localizada no catalogo"))
            {
                causa = "A ferramenta usada pelo nó está inativa, indisponível ou não foi encontrada no catálogo da IA.";
                comoResolver.Add("Confira o cadastro da ferramenta e deixe-a ativa para o ambiente atual.");
                comoResolver.Add("Valide o workflow novamente antes de executar.");
                return;
            }
            if (m.Contains("duplic") || m.Contains("já existe") || m.Contains("ja existe"))
            {
                causa = "O sistema provavelmente rejeitou a gravação porque já existe um registro com os mesmos dados únicos.";
                comoResolver.Add("Pesquise o registro existente e use-o no workflow.");
                comoResolver.Add("Se for realmente um novo cadastro, corrija o documento, código ou outro campo duplicado.");
                return;
            }
            if (m.Contains("timeout") || m.Contains("tempo limite"))
            {
                causa = "A etapa não respondeu dentro do tempo permitido.";
                comoResolver.Add("Tente executar novamente após alguns instantes.");
                comoResolver.Add("Se persistir, verifique o serviço ou a procedure usada pelo nó.");
                return;
            }
            if (m.Contains("bd-ds") || m.Contains("sql") || m.Contains("procedure") || m.Contains("banco de dados") || m.Contains("identificador de várias partes") || m.Contains("identificador de varias partes"))
            {
                causa = "A ferramenta chegou ao banco de dados, mas a procedure ou consulta rejeitou a operação.";
                comoResolver.Add("Revise o detalhe técnico e os parâmetros enviados ao nó.");
                comoResolver.Add("Se os parâmetros estiverem corretos, encaminhe o nome do nó e o detalhe técnico para correção da procedure.");
                return;
            }
            if (m.Contains("não encontr") || m.Contains("nao encontr") || m.Contains("nenhum registro") || m.Contains("nenhum resultado"))
            {
                causa = "Nenhum registro compatível foi localizado com os dados recebidos por esta etapa.";
                comoResolver.Add("Confira o nome, código ou filtro informado e tente uma descrição mais específica.");
                comoResolver.Add("Quando houver tabela ou tipo associado, confirme também se esse contexto está correto.");
                return;
            }
            if (m.Contains("informe") || m.Contains("obrigat") || m.Contains("vazio") || m.Contains("ao menos") || m.Contains("mínim") || m.Contains("minim"))
            {
                causa = "Um parâmetro obrigatório chegou vazio, incompleto ou abaixo do formato mínimo aceito pela ferramenta.";
                comoResolver.Add("Confira a entrada exibida para o nó e informe o valor solicitado pelo detalhe técnico.");
                comoResolver.Add("Se o valor vem de outro nó ou de um loop, revise a referência usada no mapeamento.");
                return;
            }
            if (info != null && string.Equals(info.Categoria, "validacao", StringComparison.OrdinalIgnoreCase))
            {
                causa = "A ferramenta recusou um ou mais parâmetros por formato ou valor inválido.";
                comoResolver.Add("Compare a entrada do nó com os valores aceitos descritos no erro.");
                comoResolver.Add("Corrija o valor fixo ou a referência que alimenta o parâmetro e execute novamente.");
                return;
            }

            causa = "A etapa indicada devolveu um erro de execução que impediu a continuação do workflow.";
            comoResolver.Add("Revise o detalhe técnico e a entrada enviada ao nó que falhou.");
            comoResolver.Add("Corrija os dados ou a configuração dessa etapa e execute o workflow novamente.");
        }

        private static JToken ParseJsonOuTexto(string valor)
        {
            try
            {
                return JToken.Parse(valor ?? string.Empty);
            }
            catch
            {
                return new JValue(valor ?? string.Empty);
            }
        }

        private static JObject ArgsEspera(JObject no)
        {
            string modo = Str(no["modo"]).Trim().ToLowerInvariant();
            if (modo != "data_hora" && modo != "evento")
            {
                modo = "manual";
            }

            return new JObject
            {
                { "titulo", string.IsNullOrWhiteSpace(Str(no["titulo"])) ? "Espera" : Str(no["titulo"]) },
                { "modo", modo },
                { "dataHora", Str(no["dataHora"]) },
                { "evento", Str(no["evento"]) },
                { "mensagem", Str(no["mensagem"]) }
            };
        }

        private static bool TentarParseDataHora(string valor, out DateTime data)
        {
            valor = (valor ?? string.Empty).Trim();
            string[] formatos = new[]
            {
                "yyyy-MM-ddTHH:mm",
                "yyyy-MM-dd HH:mm",
                "dd/MM/yyyy HH:mm",
                "dd/MM/yyyy"
            };

            if (DateTime.TryParseExact(valor, formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out data))
            {
                return true;
            }

            CultureInfo pt = new CultureInfo("pt-BR");
            return DateTime.TryParse(valor, pt, DateTimeStyles.None, out data) || DateTime.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.None, out data);
        }

        private static bool LoopModoLista(JObject no)
        {
            string modo = Str(no["modoLoop"]);
            if (string.IsNullOrWhiteSpace(modo))
            {
                modo = Str(no["modo"]);
            }

            return string.Equals(modo, "lista", StringComparison.OrdinalIgnoreCase);
        }

        private static JArray ResolverListaLoop(JObject no, JObject entradas, Dictionary<string, JToken> saidas)
        {
            string origem = Str(no["listaOrigem"]);
            if (string.IsNullOrWhiteSpace(origem))
            {
                origem = "entrada";
            }

            string valor = Str(no["listaValor"]);
            JToken token = ResolverValor(origem, valor, entradas, saidas);
            if (token == null || token.Type == JTokenType.Null || token.Type == JTokenType.Undefined)
            {
                return new JArray();
            }

            JArray arr = token as JArray;
            if (arr != null)
            {
                return arr;
            }

            JObject obj = token as JObject;
            if (obj != null)
            {
                foreach (string campo in new[] { "registros", "produtos", "itens", "dados" })
                {
                    JArray interno = obj[campo] as JArray;
                    if (interno != null)
                    {
                        return interno;
                    }
                }
            }

            if (token.Type == JTokenType.String)
            {
                string texto = token.ToString().Trim();
                if (texto.Length == 0)
                {
                    return new JArray();
                }

                try
                {
                    JToken parsed = JToken.Parse(texto);
                    JArray parsedArray = parsed as JArray;
                    if (parsedArray != null)
                    {
                        return parsedArray;
                    }
                }
                catch
                {
                }
            }

            return new JArray();
        }

        private static int LimiteLoop(JObject no)
        {
            int limite;
            if (!int.TryParse(Str(no["limite"]), out limite))
            {
                return 0;
            }

            if (limite < 1) return 0;
            return Math.Min(limite, MaxLoopIteracoes);
        }

        private static int IndiceLoopAtual(Dictionary<string, JToken> saidas, string idNo)
        {
            JToken saida;
            if (!saidas.TryGetValue(idNo, out saida) || saida == null)
            {
                return 0;
            }

            int indice;
            JObject obj = saida as JObject;
            if (obj != null && int.TryParse(Str(obj["indice"]), out indice) && indice > 0)
            {
                return indice;
            }

            return 0;
        }

        // Algum outro nó lê nX.dados...? Então o nó de IA precisa mesmo devolver dados estruturados.
        private static bool DadosDoNoLidosAdiante(List<JObject> ordem, string idNo)
        {
            Regex leitura = new Regex(@"(?<![A-Za-z0-9_])" + Regex.Escape(idNo) + @"\.dados\b", RegexOptions.IgnoreCase);
            foreach (JObject outro in ordem)
            {
                if (string.Equals(Str(outro["id"]), idNo, StringComparison.OrdinalIgnoreCase)) continue;
                if (leitura.IsMatch(outro.ToString(Formatting.None))) return true;
            }
            return false;
        }

        private static bool TemDadosEstruturados(JToken saida)
        {
            JObject objeto = saida as JObject;
            JObject dados = objeto != null ? objeto["dados"] as JObject : null;
            return dados != null && dados.HasValues;
        }

        private static IAProviderResponse ChamarProviderNoIA(cls_IA_Config config, string mensagemUsuario)
        {
            return cls_IA_ProviderFactory.Criar(config).GerarResposta(new IAProviderRequest
            {
                Modelo = config.ModeloPadrao,
                Instrucoes = "Você é um nó interno de um workflow do TT_Flow. Não chame ferramentas. Responda somente em JSON válido no formato {\"texto\":\"resumo curto\",\"dados\":{}}. Coloque em dados os campos que os próximos nós podem usar.",
                MensagemUsuario = mensagemUsuario,
                MaxTokensSaida = config.MaxTokensSaida,
                ForcarSemFerramentas = true
            });
        }

        private static JToken ExecutarNoIA(JObject no, JObject dadosEntrada, out string erro, bool exigeDados)
        {
            erro = string.Empty;
            string prompt = Str(no["prompt"]).Trim();
            if (prompt.Length == 0)
            {
                erro = "Nó de IA sem prompt configurado.";
                return null;
            }

            cls_IA_Config config = cls_IA_Config.Carregar();
            if (config == null || !config.Habilitado)
            {
                erro = "IA não configurada ou desabilitada.";
                return null;
            }

            StringBuilder mensagem = new StringBuilder();
            mensagem.AppendLine(prompt);
            mensagem.AppendLine();
            mensagem.AppendLine("Dados de entrada do nó (JSON):");
            mensagem.AppendLine((dadosEntrada ?? new JObject()).ToString(Formatting.None));

            string schema = Str(no["schemaSaida"]).Trim();
            if (schema.Length > 0)
            {
                mensagem.AppendLine();
                mensagem.AppendLine("Quando preencher dados estruturados, respeite este schema de saída:");
                mensagem.AppendLine(schema);
            }

            IAProviderResponse resposta = ChamarProviderNoIA(config, mensagem.ToString());
            if (resposta == null || !resposta.Sucesso)
            {
                erro = resposta != null && !string.IsNullOrWhiteSpace(resposta.Erro) ? resposta.Erro : "Falha ao executar o nó de IA.";
                return null;
            }

            JObject saida = NormalizarSaidaIA(resposta.Resposta);
            if (!exigeDados || TemDadosEstruturados(saida)) return saida;

            // Os próximos passos leem nX.dados, mas o modelo respondeu sem o JSON pedido. Antes era só um dados = {}
            // silencioso, e os passos seguintes reclamavam de campos "não informados" pelo usuário. Tenta mais uma vez,
            // lembrando o formato; se falhar de novo, é erro do nó, dito como tal.
            IAProviderResponse segunda = ChamarProviderNoIA(config, mensagem
                + "\n\nATENÇÃO: sua resposta anterior não era um JSON válido no formato pedido. Responda SOMENTE com o JSON {\"texto\":\"...\",\"dados\":{...}}, sem nenhum texto antes ou depois.");
            if (segunda != null && segunda.Sucesso) saida = NormalizarSaidaIA(segunda.Resposta);
            if (TemDadosEstruturados(saida)) return saida;

            string recebido = Str(saida["texto"]).Trim();
            if (recebido.Length > 160) recebido = recebido.Substring(0, 160) + "...";
            erro = PrefixoNoIASemDados + " o modelo não devolveu o JSON com os dados que os próximos passos usam"
                + (recebido.Length > 0 ? " (resposta recebida: \"" + recebido + "\")" : string.Empty) + ".";
            return null;
        }

        private static JObject NormalizarSaidaIA(string texto)
        {
            texto = LimparJsonMarkdown(texto);
            JObject bruto = null;
            try { bruto = JObject.Parse(texto ?? "{}"); } catch { }

            if (bruto == null)
            {
                return new JObject
                {
                    { "texto", texto ?? string.Empty },
                    { "dados", new JObject() }
                };
            }

            JToken dados = bruto["dados"];
            JObject dadosObj = dados as JObject;
            if (dadosObj == null)
            {
                dadosObj = new JObject();
                foreach (JProperty p in bruto.Properties())
                {
                    if (!string.Equals(p.Name, "texto", StringComparison.OrdinalIgnoreCase))
                    {
                        dadosObj[p.Name] = p.Value;
                    }
                }
            }

            return new JObject
            {
                { "texto", Str(bruto["texto"]).Length > 0 ? Str(bruto["texto"]) : texto },
                { "dados", dadosObj }
            };
        }

        private static string LimparJsonMarkdown(string texto)
        {
            texto = (texto ?? string.Empty).Trim();
            if (texto.StartsWith("```", StringComparison.Ordinal))
            {
                int quebra = texto.IndexOf('\n');
                int fim = texto.LastIndexOf("```", StringComparison.Ordinal);
                if (quebra >= 0 && fim > quebra)
                {
                    texto = texto.Substring(quebra + 1, fim - quebra - 1).Trim();
                }
            }
            return texto;
        }

        private static string ResumirArgs(JObject args)
        {
            if (args == null) return string.Empty;
            List<string> partes = new List<string>();
            foreach (JProperty p in args.Properties())
            {
                string v = p.Value != null ? p.Value.ToString() : string.Empty;
                if (v.Length > 60) v = v.Substring(0, 60) + "...";
                partes.Add(p.Name + "=" + v);
                if (partes.Count >= 5) break;
            }
            return string.Join(", ", partes);
        }

        private static string TituloNo(JObject no)
        {
            string tipo = Str(no["tipo"]).ToLowerInvariant();
            if (tipo == "ferramenta") return Str(no["ferramenta"]);
            if (tipo == "condicao") return "se " + Str(no["campo"]);
            if (tipo == "ia") return string.IsNullOrWhiteSpace(Str(no["titulo"])) ? "IA" : Str(no["titulo"]);
            if (tipo == "merge") return string.IsNullOrWhiteSpace(Str(no["titulo"])) ? "Merge" : Str(no["titulo"]);
            if (tipo == "aprovacao") return string.IsNullOrWhiteSpace(Str(no["titulo"])) ? "Aprovação humana" : Str(no["titulo"]);
            if (tipo == "loop") return string.IsNullOrWhiteSpace(Str(no["titulo"])) ? "Loop" : Str(no["titulo"]);
            if (tipo == "espera") return string.IsNullOrWhiteSpace(Str(no["titulo"])) ? "Espera" : Str(no["titulo"]);
            return tipo;
        }

        private static string TokenParaTexto(JToken t) { return t == null || t.Type == JTokenType.Null ? string.Empty : t.ToString(); }
        private static string Str(JToken t) { return t == null ? string.Empty : t.ToString(); }
        private static bool Bool(JToken t)
        {
            if (t == null) return false;
            if (t.Type == JTokenType.Boolean) return t.Value<bool>();
            string valor = Str(t).Trim();
            return valor == "1"
                || valor.Equals("true", StringComparison.OrdinalIgnoreCase)
                || valor.Equals("S", StringComparison.OrdinalIgnoreCase)
                || valor.Equals("sim", StringComparison.OrdinalIgnoreCase);
        }
        private static void DefinirEntrada(IAWorkflowTracePasso passo, string valor)
        {
            if (passo == null) return;
            valor = valor ?? string.Empty;
            passo.EntradaCompleta = valor;
            passo.Entrada = valor;
        }

        private static void DefinirSaida(IAWorkflowTracePasso passo, string valor)
        {
            if (passo == null) return;
            valor = valor ?? string.Empty;
            passo.SaidaCompleta = valor;
            passo.Saida = valor;
        }

        private static string Snippet(string s) { return string.IsNullOrEmpty(s) || s.Length <= MaxSnippet ? s : s.Substring(0, MaxSnippet) + " ..."; }
    }

    public class IAWorkflowTracePasso
    {
        public string No { get; set; }
        public string Tipo { get; set; }
        public string Titulo { get; set; }
        public string Status { get; set; }   // OK | ERRO | PAUSADO
        public string Entrada { get; set; }
        public string Saida { get; set; }
        public string EntradaCompleta { get; set; }
        public string SaidaCompleta { get; set; }
        public bool ErroTratado { get; set; }
        public long DuracaoMs { get; set; }  // quanto o passo levou, com tentativas e esperas do retry; 0 em traces antigos
    }

    public class IAWorkflowResultado
    {
        public bool Sucesso { get; set; }
        public string Status { get; set; }
        public string TipoPausa { get; set; }
        public string DadosPausaJson { get; set; }
        public string Mensagem { get; set; }
        public List<IAWorkflowTracePasso> Trace { get; set; }
        public JObject SaidaFinal { get; set; }
    }

    public class IAWorkflowOutcome
    {
        public string Status { get; set; }        // CONCLUIDO | PAUSADO | ERRO
        public string TipoPausa { get; set; }     // FERRAMENTA | APROVACAO | ESPERA | ENTRADAS | ESCOLHA | SELECAO_MULTIPLA
        public string NoPausa { get; set; }
        public string FerramentaPausa { get; set; }
        public JObject ArgsPausa { get; set; }
        public JArray ResumoPausa { get; set; }   // linhas { rotulo, valor } do que a escrita pausada vai gravar
        public string Resumo { get; set; }
        public List<IAWorkflowTracePasso> Trace { get; set; }
        public JObject Saidas { get; set; }
        public string Mensagem { get; set; }
    }

    public class IAWorkflowErroInfo
    {
        public string Categoria { get; set; }
        public string Codigo { get; set; }
    }
}
