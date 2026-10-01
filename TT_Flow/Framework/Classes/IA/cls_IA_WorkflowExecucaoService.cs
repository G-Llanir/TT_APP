using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IDENTITY = TT.FrameWork.Identity;

namespace TT_Flow.FrameWork.IA
{
    // Orquestra a execução REAL de um workflow (Fase 2): inicia, PAUSA num passo de escrita gravando o
    // estado, e RETOMA após a confirmação do usuário. A escrita é executada pelo caminho "confirmado" do
    // executor. Toda validação de permissão é refeita na confirmação.
    public class cls_IA_WorkflowExecucaoService
    {
        private const int MaxSnippet = 1200;
        private readonly cls_IA_Repositorio _repositorio = new cls_IA_Repositorio();

        public IAWorkflowExecResponse Iniciar(int idWorkflowIA, JObject entradas)
        {
            return IniciarOuRetomar(idWorkflowIA, entradas, 0);
        }

        public IAWorkflowExecResponse Iniciar(int idWorkflowIA, JObject entradas, int idConversaIA)
        {
            return IniciarOuRetomar(idWorkflowIA, entradas, idConversaIA);
        }

        public IAWorkflowExecResponse IniciarOuRetomar(int idWorkflowIA, JObject entradas, int idConversaIA)
        {
            if (idConversaIA > 0)
            {
                IAWorkflowExecucao ativa = _repositorio.ObterWorkflowExecucaoAtivaPorConversa(idConversaIA);
                if (ativa != null)
                {
                    return ResponderExecucaoPersistida(ativa, "Já existe uma operação ativa nesta conversa. Continue a etapa pendente antes de iniciar outro workflow.");
                }
            }

            IAWorkflow wf = _repositorio.ObterWorkflow(idWorkflowIA);
            if (wf == null)
            {
                return Erro("Workflow não localizado.");
            }
            if (!wf.Ativo)
            {
                return Erro("Workflow inativo.");
            }
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(wf.IdRecursoNecessario))
            {
                return Erro("Você não tem permissão para executar este workflow.");
            }
            if (RequerPermissaoExecutarAcoes(wf) && !cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.ExecutarAcoes))
            {
                return Erro("Você não possui permissão para executar ações pela IA.");
            }

            entradas = NormalizarEntradasListas(wf.GrafoJson, entradas ?? new JObject());
            IAWorkflowOutcome o = CriarPausaEntradasIniciais(wf, entradas);
            if (o == null)
            {
                o = new cls_IA_WorkflowEngine().Caminhar(idConversaIA, wf, null, entradas, null);
            }

            IAWorkflowExecucao ex = new IAWorkflowExecucao
            {
                IdExecucaoIA = 0,
                IdWorkflowIA = idWorkflowIA,
                IdConversaIA = idConversaIA,
                IdUsuario = IdUsuario(),
                Status = o.Status,
                ResumoAcao = o.Resumo,
                TraceJson = JsonConvert.SerializeObject(o.Trace),
                EstadoJson = MontarEstado(entradas, o, idConversaIA).ToString(Formatting.None)
            };
            if (idConversaIA > 0)
            {
                bool criada;
                IAWorkflowExecucao persistida = _repositorio.IniciarOuObterWorkflowExecucao(ex, out criada);
                if (persistida == null)
                {
                    return Erro("Não foi possível salvar a execução do workflow.");
                }
                if (!criada)
                {
                    return ResponderExecucaoPersistida(persistida, "A operação já estava em andamento e foi retomada sem criar uma nova execução.");
                }
                ex = persistida;
            }
            else
            {
                ex.IdExecucaoIA = _repositorio.SalvarWorkflowExecucao(ex);
            }

            if (ex.IdExecucaoIA <= 0)
            {
                return Erro("Não foi possível salvar a execução do workflow.");
            }

            cls_IA_Auditoria.Registrar(idConversaIA, "WORKFLOW_INICIADO", "ALERTA", new { workflow = wf.Nome, idExecucaoIA = ex.IdExecucaoIA, status = o.Status });
            return Responder(ex.IdExecucaoIA, o.Status, o.Mensagem, o.Resumo, o.Trace, o.Saidas, o.TipoPausa, DadosPausaComResumo(o.ArgsPausa, o.ResumoPausa));
        }

        public IAWorkflowExecucao ObterAtivaPorConversa(int idConversaIA)
        {
            IAWorkflowExecucao ex = _repositorio.ObterWorkflowExecucaoAtivaPorConversa(idConversaIA);
            return ex != null && ex.IdUsuario == IdUsuario() ? ex : null;
        }

        public IAWorkflowExecResponse ObterEstadoAtual(int idConversaIA)
        {
            IAWorkflowExecucao ex = ObterAtivaPorConversa(idConversaIA);
            return ex == null ? null : ResponderExecucaoPersistida(ex, string.Empty);
        }

        public IAWorkflowExecResponse Confirmar(int idExecucaoIA)
        {
            return Confirmar(idExecucaoIA, 0);
        }

        public IAWorkflowExecResponse Confirmar(int idExecucaoIA, int idConversaIA)
        {
            IAWorkflowExecucao ex = _repositorio.ObterWorkflowExecucao(idExecucaoIA);
            if (ex == null) { return Erro("Execução não localizada."); }
            if (ex.IdUsuario != IdUsuario()) { return Erro("Execução de outro usuário."); }
            if (ex.Status != "PAUSADO") { return Erro("Execução não está pausada."); }
            if (!ex.WorkflowAtivo) { return Erro("Workflow inativo."); }
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(ex.IdRecursoNecessario)) { return Erro("Sem permissão para este workflow."); }

            JObject estado;
            try { estado = JObject.Parse(ex.EstadoJson ?? "{}"); } catch { estado = new JObject(); }
            JObject entradas = estado["entradas"] as JObject ?? new JObject();
            JObject saidas = estado["saidas"] as JObject ?? new JObject();
            int idConversaOriginal = ex.IdConversaIA;
            if (idConversaOriginal <= 0) int.TryParse(Convert.ToString(estado["idConversaIA"]), out idConversaOriginal);
            if (idConversaOriginal > 0 && idConversaIA > 0 && idConversaOriginal != idConversaIA)
            {
                return Erro("Execução pertence a outra conversa.");
            }
            if (idConversaIA <= 0)
            {
                idConversaIA = idConversaOriginal;
            }

            string noAtual = Convert.ToString(estado["noAtual"]);
            string tipoPausa = Convert.ToString(estado["tipoPausa"]);
            string ferramentaNome = Convert.ToString(estado["ferramentaPausa"]);
            JObject args = estado["argsPausa"] as JObject ?? new JObject();

            List<IAWorkflowTracePasso> trace = Deserializar(ex.TraceJson);

            if (string.Equals(tipoPausa, "APROVACAO", StringComparison.OrdinalIgnoreCase))
            {
                return ProcessarAprovacao(ex, entradas, saidas, noAtual, args, trace, idConversaIA, true);
            }
            if (string.Equals(tipoPausa, "ESPERA", StringComparison.OrdinalIgnoreCase))
            {
                return ProcessarEspera(ex, entradas, saidas, noAtual, args, trace, idConversaIA);
            }
            if (string.Equals(tipoPausa, "ENTRADAS", StringComparison.OrdinalIgnoreCase))
            {
                return Responder(ex.IdExecucaoIA, "PAUSADO", "Informe as entradas solicitadas para continuar.", ex.ResumoAcao, trace, saidas, "ENTRADAS", args);
            }
            if (string.Equals(tipoPausa, "ESCOLHA", StringComparison.OrdinalIgnoreCase))
            {
                return Responder(ex.IdExecucaoIA, "PAUSADO", "Selecione uma das opções apresentadas para continuar.", ex.ResumoAcao, trace, saidas, "ESCOLHA", args);
            }
            if (string.Equals(tipoPausa, "SELECAO_MULTIPLA", StringComparison.OrdinalIgnoreCase))
            {
                return Responder(ex.IdExecucaoIA, "PAUSADO", "Selecione os itens opcionais e confirme para continuar.", ex.ResumoAcao, trace, saidas, "SELECAO_MULTIPLA", args);
            }

            IAFerramentaDefinicao def = cls_IA_ToolRegistry.Obter(ferramentaNome);
            if (def == null)
            {
                string msg = "Ferramenta do passo não localizada.";
                FinalizarErro(ex, trace, noAtual, ferramentaNome, msg);
                return Responder(ex.IdExecucaoIA, "ERRO", msg, null, trace);
            }
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(def.IdRecursoNecessario))
            {
                string msg = "Sem permissão para executar a ferramenta do passo.";
                FinalizarErro(ex, trace, noAtual, ferramentaNome, msg);
                return Responder(ex.IdExecucaoIA, "ERRO", msg, null, trace);
            }
            bool ehWrite = string.Equals(def.Escopo, "WRITE", StringComparison.OrdinalIgnoreCase);
            if (ehWrite && !cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.ExecutarAcoes))
            {
                string msg = "Você não possui permissão para executar ações pela IA.";
                FinalizarErro(ex, trace, noAtual, ferramentaNome, msg);
                return Responder(ex.IdExecucaoIA, "ERRO", msg, null, trace);
            }
            if (!_repositorio.MarcarWorkflowExecucaoExecutando(ex.IdExecucaoIA))
            {
                return Erro("Execução já foi processada ou não está mais pausada.");
            }

            string erroExec = string.Empty;
            bool ok = false;
            JToken saidaConfirmada = new JObject { { "executado", true } };
            int totalTentativas = 1 + cls_IA_WorkflowEngine.RetriesErro(ex.GrafoJson, noAtual);
            int tentativaExecutada = 0;

            for (int tentativa = 1; tentativa <= totalTentativas; tentativa++)
            {
                tentativaExecutada = tentativa;
                erroExec = string.Empty;

                if (def.EhGenerica())
                {
                    if (ehWrite)
                    {
                        ok = cls_IA_ToolExecutor.ExecutarGenericaConfirmada(def, args, out erroExec);
                    }
                    else
                    {
                        string json = new cls_IA_ToolExecutor().ExecutarLeituraGenericaConfirmada(idConversaIA, def, args, out erroExec);
                        ok = string.IsNullOrWhiteSpace(erroExec);
                        saidaConfirmada = ok ? ExtrairSaida(json) : saidaConfirmada;
                    }
                }
                else if (ehWrite)
                {
                    JToken saidaInterna;
                    ok = cls_IA_ToolExecutor.ExecutarInternaConfirmada(ferramentaNome, args, idConversaIA, out erroExec, out saidaInterna);
                    if (ok && saidaInterna != null)
                    {
                        saidaConfirmada = saidaInterna;
                    }
                }
                else
                {
                    IAFerramentaResultado res = new cls_IA_ToolExecutor().Executar(idConversaIA, new IAToolCall { Nome = ferramentaNome, Argumentos = args });
                    ok = res != null && res.Status == "SUCESSO";
                    erroExec = ok ? string.Empty : cls_IA_WorkflowEngine.DescreverErroFerramenta(ferramentaNome, res);
                    saidaConfirmada = ok ? ExtrairSaida(res.ResultadoJson) : saidaConfirmada;
                }

                if (ok)
                {
                    break;
                }

                if (tentativa < totalTentativas)
                {
                    cls_IA_WorkflowEngine.AguardarRetry(ex.GrafoJson, noAtual, tentativa, totalTentativas);
                }
            }
            if (!ok && string.IsNullOrWhiteSpace(erroExec))
            {
                erroExec = "Ferramenta \"" + ferramentaNome + "\" falhou: a execução confirmada não retornou detalhe do erro.";
            }

            string entradaTrace = args.ToString(Formatting.None);
            string saidaTrace = ok
                ? MensagemSucessoConfirmadoCompleta(ehWrite, saidaConfirmada, tentativaExecutada, totalTentativas)
                : "Falha: " + MensagemErroComTentativa(erroExec, tentativaExecutada, totalTentativas);

            trace.Add(new IAWorkflowTracePasso
            {
                No = noAtual, Tipo = "ferramenta", Titulo = ferramentaNome,
                Status = ok ? "OK" : "ERRO",
                Entrada = entradaTrace,
                Saida = saidaTrace,
                EntradaCompleta = entradaTrace,
                SaidaCompleta = saidaTrace
            });
            cls_IA_Auditoria.Registrar(idConversaIA, "WORKFLOW_PASSO_CONFIRMADO", ok ? "ALERTA" : "ERRO", new { idExecucaoIA = ex.IdExecucaoIA, no = noAtual, ferramenta = ferramentaNome, ok });

            if (!ok)
            {
                string proxErro = cls_IA_WorkflowEngine.ProximoNoErro(ex.GrafoJson, noAtual);
                if (!string.IsNullOrWhiteSpace(proxErro))
                {
                    if (trace.Count > 0)
                    {
                        trace[trace.Count - 1].ErroTratado = true;
                        if (!string.IsNullOrWhiteSpace(trace[trace.Count - 1].Saida))
                        {
                            trace[trace.Count - 1].Saida += " Encaminhado para o tratamento de erro.";
                            trace[trace.Count - 1].SaidaCompleta = trace[trace.Count - 1].Saida;
                        }
                    }
                    saidas[noAtual] = cls_IA_WorkflowEngine.SaidaErroTratado("ferramenta", ferramentaNome, erroExec, tentativaExecutada, totalTentativas);
                    IAWorkflow wfErro = new IAWorkflow { Nome = ex.WorkflowNome, GrafoJson = ex.GrafoJson, Escopo = "READ", IdRecursoNecessario = ex.IdRecursoNecessario };
                    IAWorkflowOutcome oErro = new cls_IA_WorkflowEngine().Caminhar(idConversaIA, wfErro, saidas, entradas, proxErro);
                    trace.AddRange(oErro.Trace);
                    Persistir(ex, oErro.Status, oErro.Resumo, entradas, oErro.Saidas, oErro.NoPausa, oErro.FerramentaPausa, oErro.ArgsPausa, trace, idConversaIA, oErro.TipoPausa, oErro.ResumoPausa);
                    return Responder(ex.IdExecucaoIA, oErro.Status, oErro.Mensagem, oErro.Resumo, trace, oErro.Saidas, oErro.TipoPausa, DadosPausaComResumo(oErro.ArgsPausa, oErro.ResumoPausa));
                }

                Persistir(ex, "ERRO", null, entradas, saidas, null, null, null, trace, idConversaIA, null);
                return Responder(ex.IdExecucaoIA, "ERRO", "Falha ao executar o passo confirmado: " + MensagemErroComTentativa(erroExec, tentativaExecutada, totalTentativas), null, trace);
            }

            saidas[noAtual] = saidaConfirmada ?? new JObject();

            // Retoma a partir do próximo nó.
            string prox = cls_IA_WorkflowEngine.ProximoNo(ex.GrafoJson, noAtual);
            IAWorkflow wf = new IAWorkflow { Nome = ex.WorkflowNome, GrafoJson = ex.GrafoJson, Escopo = "READ", IdRecursoNecessario = ex.IdRecursoNecessario };
            IAWorkflowOutcome o = new cls_IA_WorkflowEngine().Caminhar(idConversaIA, wf, saidas, entradas, prox);
            trace.AddRange(o.Trace);

            Persistir(ex, o.Status, o.Resumo, entradas, o.Saidas, o.NoPausa, o.FerramentaPausa, o.ArgsPausa, trace, idConversaIA, o.TipoPausa, o.ResumoPausa);

            if (o.Status == "CONCLUIDO") cls_IA_Auditoria.Registrar(idConversaIA, "WORKFLOW_CONCLUIDO", "INFO", new { idExecucaoIA = ex.IdExecucaoIA, workflow = ex.WorkflowNome });
            return Responder(ex.IdExecucaoIA, o.Status, o.Mensagem, o.Resumo, trace, o.Saidas, o.TipoPausa, DadosPausaComResumo(o.ArgsPausa, o.ResumoPausa));
        }

        public IAWorkflowExecResponse InformarEntradas(int idExecucaoIA, JObject novasEntradas)
        {
            return InformarEntradas(idExecucaoIA, novasEntradas, 0);
        }

        public IAWorkflowExecResponse InformarEntradas(int idExecucaoIA, JObject novasEntradas, int idConversaIA)
        {
            IAWorkflowExecucao ex = _repositorio.ObterWorkflowExecucao(idExecucaoIA);
            if (ex == null) { return Erro("Execução não localizada."); }
            if (ex.IdUsuario != IdUsuario()) { return Erro("Execução de outro usuário."); }
            if (ex.Status != "PAUSADO") { return Erro("Execução não está pausada."); }
            if (!ex.WorkflowAtivo) { return Erro("Workflow inativo."); }
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(ex.IdRecursoNecessario)) { return Erro("Sem permissão para este workflow."); }

            JObject estado;
            try { estado = JObject.Parse(ex.EstadoJson ?? "{}"); } catch { estado = new JObject(); }
            string tipoPausa = Convert.ToString(estado["tipoPausa"]);
            if (!string.Equals(tipoPausa, "ENTRADAS", StringComparison.OrdinalIgnoreCase))
            {
                return Erro("A execução não está aguardando entradas do usuário.");
            }

            JObject entradas = estado["entradas"] as JObject ?? new JObject();
            JObject saidas = estado["saidas"] as JObject ?? new JObject();
            JObject args = estado["argsPausa"] as JObject ?? new JObject();
            string noAtual = Convert.ToString(estado["noAtual"]);

            int idConversaOriginal = ex.IdConversaIA;
            if (idConversaOriginal <= 0) int.TryParse(Convert.ToString(estado["idConversaIA"]), out idConversaOriginal);
            if (idConversaOriginal > 0 && idConversaIA > 0 && idConversaOriginal != idConversaIA)
            {
                return Erro("Execução pertence a outra conversa.");
            }
            if (idConversaIA <= 0)
            {
                idConversaIA = idConversaOriginal;
            }

            foreach (JProperty prop in (novasEntradas ?? new JObject()).Properties())
            {
                if (!string.IsNullOrWhiteSpace(prop.Name))
                {
                    entradas[prop.Name] = prop.Value == null ? JValue.CreateString(string.Empty) : prop.Value.DeepClone();
                }
            }
            entradas = NormalizarEntradasListas(ex.GrafoJson, entradas);

            if (!_repositorio.MarcarWorkflowExecucaoExecutando(ex.IdExecucaoIA))
            {
                return Erro("Execução já foi processada ou não está mais pausada.");
            }

            List<IAWorkflowTracePasso> trace = Deserializar(ex.TraceJson);
            string entradasInformadas = (novasEntradas ?? new JObject()).ToString(Formatting.None);
            trace.Add(new IAWorkflowTracePasso
            {
                No = noAtual,
                Tipo = "entradas",
                Titulo = Convert.ToString(args["titulo"]),
                Status = "OK",
                Entrada = entradasInformadas,
                Saida = "Entradas recebidas; retomando o nó.",
                EntradaCompleta = entradasInformadas,
                SaidaCompleta = "Entradas recebidas; retomando o nó."
            });

            IAWorkflow wf = new IAWorkflow { Nome = ex.WorkflowNome, GrafoJson = ex.GrafoJson, Escopo = "READ", IdRecursoNecessario = ex.IdRecursoNecessario };
            string noRetomada = string.Equals(noAtual, "__INICIO__", StringComparison.OrdinalIgnoreCase) ? null : noAtual;
            IAWorkflowOutcome o = new cls_IA_WorkflowEngine().Caminhar(idConversaIA, wf, saidas, entradas, noRetomada);
            trace.AddRange(o.Trace);

            Persistir(ex, o.Status, o.Resumo, entradas, o.Saidas, o.NoPausa, o.FerramentaPausa, o.ArgsPausa, trace, idConversaIA, o.TipoPausa, o.ResumoPausa);

            if (o.Status == "CONCLUIDO") cls_IA_Auditoria.Registrar(idConversaIA, "WORKFLOW_CONCLUIDO", "INFO", new { idExecucaoIA = ex.IdExecucaoIA, workflow = ex.WorkflowNome });
            cls_IA_Auditoria.Registrar(idConversaIA, "WORKFLOW_ENTRADAS_INFORMADAS", "INFO", new { idExecucaoIA = ex.IdExecucaoIA, no = noAtual });
            return Responder(ex.IdExecucaoIA, o.Status, o.Mensagem, o.Resumo, trace, o.Saidas, o.TipoPausa, DadosPausaComResumo(o.ArgsPausa, o.ResumoPausa));
        }

        public IAWorkflowExecResponse SelecionarOpcao(int idExecucaoIA, string valor, int idConversaIA)
        {
            IAWorkflowExecucao ex = _repositorio.ObterWorkflowExecucao(idExecucaoIA);
            if (ex == null) return Erro("Execução não localizada.");
            if (ex.IdUsuario != IdUsuario()) return Erro("Execução de outro usuário.");
            if (ex.Status != "PAUSADO") return Erro("Execução não está pausada.");
            if (!ex.WorkflowAtivo) return Erro("Workflow inativo.");
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(ex.IdRecursoNecessario)) return Erro("Sem permissão para este workflow.");

            JObject estado;
            try { estado = JObject.Parse(ex.EstadoJson ?? "{}"); } catch { estado = new JObject(); }
            string tipoPausa = Convert.ToString(estado["tipoPausa"]);
            bool selecaoMultipla = string.Equals(tipoPausa, "SELECAO_MULTIPLA", StringComparison.OrdinalIgnoreCase);
            if (!selecaoMultipla && !string.Equals(tipoPausa, "ESCOLHA", StringComparison.OrdinalIgnoreCase))
            {
                return Erro("A execução não está aguardando uma escolha.");
            }

            int idConversaOriginal = ex.IdConversaIA;
            if (idConversaOriginal <= 0) int.TryParse(Convert.ToString(estado["idConversaIA"]), out idConversaOriginal);
            if (idConversaOriginal > 0 && idConversaIA > 0 && idConversaOriginal != idConversaIA)
            {
                return Erro("Execução pertence a outra conversa.");
            }
            if (idConversaIA <= 0) idConversaIA = idConversaOriginal;

            JObject args = estado["argsPausa"] as JObject ?? new JObject();
            JArray opcoes = (args["opcoes"] as JArray) ?? new JArray();
            List<JObject> opcoesEscolhidas = new List<JObject>();
            if (selecaoMultipla)
            {
                JArray valores;
                try
                {
                    valores = string.IsNullOrWhiteSpace(valor) ? new JArray() : JArray.Parse(valor);
                }
                catch
                {
                    return Erro("Seleção múltipla inválida.");
                }

                HashSet<string> valoresUnicos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (JToken item in valores)
                {
                    string valorItem = Convert.ToString(item);
                    if (!valoresUnicos.Add(valorItem)) continue;
                    JObject opcao = LocalizarOpcaoPausa(opcoes, valorItem);
                    if (opcao == null) return Erro("Uma das opções selecionadas não está mais disponível.");
                    opcoesEscolhidas.Add(opcao);
                }
            }
            else
            {
                JObject opcao = LocalizarOpcaoPausa(opcoes, valor);
                if (opcao == null) return Erro("Escolha inválida ou não disponível para esta execução.");
                opcoesEscolhidas.Add(opcao);
            }

            if (!_repositorio.MarcarWorkflowExecucaoExecutando(ex.IdExecucaoIA))
            {
                return Erro("Execução já foi processada ou não está mais pausada.");
            }

            JObject entradas = estado["entradas"] as JObject ?? new JObject();
            JObject saidas = estado["saidas"] as JObject ?? new JObject();
            string noAtual = Convert.ToString(estado["noAtual"]);
            JArray registrosSelecionados = new JArray();
            List<string> rotulosSelecionados = new List<string>();
            foreach (JObject opcao in opcoesEscolhidas)
            {
                JObject registroOpcao = opcao["registro"] as JObject ?? new JObject();
                registrosSelecionados.Add(registroOpcao.DeepClone());
                rotulosSelecionados.Add(Convert.ToString(opcao["rotulo"]));
            }
            JObject registro = registrosSelecionados.Count > 0 ? registrosSelecionados[0] as JObject : new JObject();
            JObject resultadoOriginal = args["resultado"] as JObject;
            JObject resultadoCanonico = resultadoOriginal != null ? (JObject)resultadoOriginal.DeepClone() : new JObject();
            resultadoCanonico["encontrado"] = registrosSelecionados.Count > 0;
            resultadoCanonico["unicoProvavel"] = registrosSelecionados.Count == 1;
            resultadoCanonico["precisaEscolha"] = false;
            resultadoCanonico["melhorResultado"] = registro.DeepClone();
            resultadoCanonico["registros"] = registrosSelecionados.DeepClone();
            resultadoCanonico["selecionados"] = registrosSelecionados.DeepClone();
            resultadoCanonico["opcoes"] = new JArray();
            saidas[noAtual] = resultadoCanonico;

            List<IAWorkflowTracePasso> trace = Deserializar(ex.TraceJson);
            trace.Add(new IAWorkflowTracePasso
            {
                No = noAtual,
                Tipo = selecaoMultipla ? "selecao_multipla" : "escolha",
                Titulo = Convert.ToString(args["titulo"]),
                Status = "OK",
                Entrada = valor ?? string.Empty,
                EntradaCompleta = valor ?? string.Empty,
                Saida = selecaoMultipla
                    ? (rotulosSelecionados.Count > 0 ? string.Join(", ", rotulosSelecionados.ToArray()) : "Nenhum item selecionado")
                    : rotulosSelecionados[0],
                SaidaCompleta = registrosSelecionados.ToString(Formatting.None)
            });

            string proximo = cls_IA_WorkflowEngine.ProximoNo(ex.GrafoJson, noAtual);
            IAWorkflow wf = new IAWorkflow { Nome = ex.WorkflowNome, GrafoJson = ex.GrafoJson, Escopo = "READ", IdRecursoNecessario = ex.IdRecursoNecessario };
            IAWorkflowOutcome o = new cls_IA_WorkflowEngine().Caminhar(idConversaIA, wf, saidas, entradas, proximo);
            trace.AddRange(o.Trace);
            Persistir(ex, o.Status, o.Resumo, entradas, o.Saidas, o.NoPausa, o.FerramentaPausa, o.ArgsPausa, trace, idConversaIA, o.TipoPausa, o.ResumoPausa);

            cls_IA_Auditoria.Registrar(idConversaIA, "WORKFLOW_OPCAO_SELECIONADA", "INFO", new { idExecucaoIA = ex.IdExecucaoIA, no = noAtual, valor });
            return Responder(ex.IdExecucaoIA, o.Status, o.Mensagem, o.Resumo, trace, o.Saidas, o.TipoPausa, DadosPausaComResumo(o.ArgsPausa, o.ResumoPausa));
        }

        private static JObject LocalizarOpcaoPausa(JArray opcoes, string valor)
        {
            const string prefixoIndice = "__indice__:";
            string valorNormalizado = valor ?? string.Empty;
            if (valorNormalizado.StartsWith(prefixoIndice, StringComparison.OrdinalIgnoreCase))
            {
                int indice;
                if (int.TryParse(valorNormalizado.Substring(prefixoIndice.Length), out indice) && indice >= 0 && indice < opcoes.Count)
                {
                    return opcoes[indice] as JObject;
                }
            }

            foreach (JToken item in opcoes)
            {
                JObject opcao = item as JObject;
                if (opcao != null && string.Equals(Convert.ToString(opcao["valor"]), valorNormalizado, StringComparison.OrdinalIgnoreCase))
                {
                    return opcao;
                }
            }
            return null;
        }

        public IAWorkflowExecResponse Rejeitar(int idExecucaoIA)
        {
            return Rejeitar(idExecucaoIA, 0);
        }

        public IAWorkflowExecResponse Rejeitar(int idExecucaoIA, int idConversaIA)
        {
            IAWorkflowExecucao ex = _repositorio.ObterWorkflowExecucao(idExecucaoIA);
            if (ex == null) { return Erro("Execução não localizada."); }
            if (ex.IdUsuario != IdUsuario()) { return Erro("Execução de outro usuário."); }
            if (ex.Status != "PAUSADO") { return Erro("Execução não está pausada."); }
            if (!ex.WorkflowAtivo) { return Erro("Workflow inativo."); }
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(ex.IdRecursoNecessario)) { return Erro("Sem permissão para este workflow."); }

            JObject estado;
            try { estado = JObject.Parse(ex.EstadoJson ?? "{}"); } catch { estado = new JObject(); }
            string tipoPausa = Convert.ToString(estado["tipoPausa"]);
            if (!string.Equals(tipoPausa, "APROVACAO", StringComparison.OrdinalIgnoreCase))
            {
                return Erro("A execução não está aguardando uma aprovação humana.");
            }

            JObject entradas = estado["entradas"] as JObject ?? new JObject();
            JObject saidas = estado["saidas"] as JObject ?? new JObject();
            int idConversaOriginal = ex.IdConversaIA;
            if (idConversaOriginal <= 0) int.TryParse(Convert.ToString(estado["idConversaIA"]), out idConversaOriginal);
            if (idConversaOriginal > 0 && idConversaIA > 0 && idConversaOriginal != idConversaIA)
            {
                return Erro("Execução pertence a outra conversa.");
            }
            if (idConversaIA <= 0)
            {
                idConversaIA = idConversaOriginal;
            }

            string noAtual = Convert.ToString(estado["noAtual"]);
            JObject args = estado["argsPausa"] as JObject ?? new JObject();
            List<IAWorkflowTracePasso> trace = Deserializar(ex.TraceJson);
            return ProcessarAprovacao(ex, entradas, saidas, noAtual, args, trace, idConversaIA, false);
        }

        public IAWorkflowExecResponse Cancelar(int idExecucaoIA)
        {
            return Cancelar(idExecucaoIA, 0);
        }

        public IAWorkflowExecResponse Cancelar(int idExecucaoIA, int idConversaIA)
        {
            IAWorkflowExecucao ex = _repositorio.ObterWorkflowExecucao(idExecucaoIA);
            if (ex == null) { return Erro("Execução não localizada."); }
            if (ex.IdUsuario != IdUsuario()) { return Erro("Execução de outro usuário."); }
            if (ex.Status != "PAUSADO") { return Erro("Execução não está pausada."); }

            int idConversaOriginal = ex.IdConversaIA;
            try
            {
                JObject estado = JObject.Parse(ex.EstadoJson ?? "{}");
                if (idConversaOriginal <= 0) int.TryParse(Convert.ToString(estado["idConversaIA"]), out idConversaOriginal);
            }
            catch
            {
            }
            if (idConversaOriginal > 0 && idConversaIA > 0 && idConversaOriginal != idConversaIA)
            {
                return Erro("Execução pertence a outra conversa.");
            }
            if (idConversaIA <= 0)
            {
                idConversaIA = idConversaOriginal;
            }

            if (!_repositorio.CancelarWorkflowExecucaoPausada(idExecucaoIA))
            {
                return Erro("Execução já foi processada ou não está mais pausada.");
            }

            cls_IA_Auditoria.Registrar(idConversaIA, "WORKFLOW_CANCELADO", "INFO", new { idExecucaoIA = ex.IdExecucaoIA, workflow = ex.WorkflowNome });
            return Responder(ex.IdExecucaoIA, "CANCELADO", "Execução cancelada.", null, Deserializar(ex.TraceJson));
        }

        public IAWorkflowLoteResponse ProcessarEsperasAgendadas(int limite)
        {
            return ProcessarEsperas(false, string.Empty, limite, 0);
        }

        public IAWorkflowLoteResponse LiberarEsperasPorEvento(string evento, int limite)
        {
            return ProcessarEsperas(true, evento, limite, 0);
        }

        public static bool RequerPermissaoExecutarAcoes(string grafoJson)
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

                    IAFerramentaDefinicao def = cls_IA_ToolRegistry.Obter(Convert.ToString(no["ferramenta"]));
                    if (def != null && string.Equals(def.Escopo, "WRITE", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        public static bool RequerPermissaoExecutarAcoes(IAWorkflow workflow)
        {
            return workflow != null
                && (string.Equals(workflow.Escopo, "WRITE", StringComparison.OrdinalIgnoreCase)
                    || RequerPermissaoExecutarAcoes(workflow.GrafoJson));
        }

        public static bool RequerPermissaoExecutarAcoes(string grafoJson, string escopo)
        {
            return string.Equals(escopo, "WRITE", StringComparison.OrdinalIgnoreCase)
                || RequerPermissaoExecutarAcoes(grafoJson);
        }

        // ---- helpers ----
        private static IAWorkflowOutcome CriarPausaEntradasIniciais(IAWorkflow workflow, JObject entradas)
        {
            JObject schema;
            try { schema = JObject.Parse(workflow != null ? workflow.SchemaParametrosJson ?? "{}" : "{}"); }
            catch { return null; }

            JObject propriedades = schema["properties"] as JObject ?? new JObject();
            JArray obrigatorios = schema["required"] as JArray ?? new JArray();
            if (obrigatorios.Count == 0) return null;

            Dictionary<string, JObject> definicoes = new Dictionary<string, JObject>(StringComparer.OrdinalIgnoreCase);
            try
            {
                JObject grafo = JObject.Parse(workflow.GrafoJson ?? "{}");
                foreach (JToken item in (grafo["entradas"] as JArray) ?? new JArray())
                {
                    JObject definicao = item as JObject;
                    string nome = definicao != null ? Convert.ToString(definicao["nome"]) : string.Empty;
                    if (!string.IsNullOrWhiteSpace(nome)) definicoes[nome] = definicao;
                }
            }
            catch { }

            JArray campos = new JArray();
            foreach (JToken token in obrigatorios)
            {
                string nome = Convert.ToString(token).Trim();
                JObject meta = propriedades[nome] as JObject ?? new JObject();
                if (string.IsNullOrWhiteSpace(nome) || EntradaInformada(entradas, nome, meta)) continue;

                JObject definicao;
                if (!definicoes.TryGetValue(nome, out definicao)) definicao = new JObject();
                campos.Add(new JObject
                {
                    { "nome", nome },
                    { "param", nome },
                    { "tipo", TipoCampo(meta["type"]) },
                    { "obrigatorio", true },
                    { "grupo", PrimeiroTexto(Convert.ToString(definicao["grupo"]), "Dados iniciais") },
                    { "descricao", PrimeiroTexto(Convert.ToString(definicao["descricao"]), Convert.ToString(meta["description"])) },
                    { "opcoes", OpcoesEntrada(definicao, meta) },
                    { "schema", meta.DeepClone() }
                });
            }

            if (campos.Count == 0) return null;
            JObject args = new JObject
            {
                { "tipo", "ENTRADAS" },
                { "no", "__INICIO__" },
                { "titulo", "Dados para iniciar o workflow" },
                { "mensagem", "Informe os dados que faltam. O workflow continuará usando tudo o que já foi fornecido nesta conversa." },
                { "grupoPrincipal", "Dados iniciais" },
                { "campos", campos }
            };
            return new IAWorkflowOutcome
            {
                Status = "PAUSADO",
                TipoPausa = "ENTRADAS",
                NoPausa = "__INICIO__",
                ArgsPausa = args,
                Resumo = "Dados para iniciar o workflow",
                Mensagem = Convert.ToString(args["mensagem"]),
                Trace = new List<IAWorkflowTracePasso>(),
                Saidas = new JObject()
            };
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
                    string valor = Convert.ToString(objeto != null ? objeto["valor"] : opcao).Trim();
                    if (valor.Length == 0) continue;
                    string rotulo = Convert.ToString(objeto != null ? objeto["rotulo"] : opcao).Trim();
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
                string valor = Convert.ToString(opcao).Trim();
                if (valor.Length > 0) resultado.Add(new JObject { { "valor", valor }, { "rotulo", valor } });
            }
            return resultado;
        }

        private static bool EntradaInformada(JObject entradas, string nome, JObject meta)
        {
            JToken valor = entradas != null ? entradas.GetValue(nome, StringComparison.OrdinalIgnoreCase) : null;
            if (!ValorInformado(valor)) return false;

            JArray lista = valor as JArray;
            JObject schemaItem = meta != null ? meta["items"] as JObject : null;
            JObject propriedadesItem = schemaItem != null ? schemaItem["properties"] as JObject : null;
            JArray obrigatoriosItem = schemaItem != null ? schemaItem["required"] as JArray : null;
            if (lista == null || propriedadesItem == null || obrigatoriosItem == null || obrigatoriosItem.Count == 0) return true;

            foreach (JToken item in lista)
            {
                JObject objeto = item as JObject;
                if (objeto == null) return false;
                foreach (JToken campoToken in obrigatoriosItem)
                {
                    string campo = Convert.ToString(campoToken);
                    if (!ValorInformado(objeto.GetValue(campo, StringComparison.OrdinalIgnoreCase))) return false;
                }
            }
            return true;
        }

        private static bool ValorInformado(JToken valor)
        {
            if (valor == null || valor.Type == JTokenType.Null || valor.Type == JTokenType.Undefined) return false;
            if (valor.Type == JTokenType.String) return !string.IsNullOrWhiteSpace(valor.ToString());
            if (valor.Type == JTokenType.Array) return ((JArray)valor).Count > 0;
            if (valor.Type == JTokenType.Object) return ((JObject)valor).Count > 0;
            return true;
        }

        private static JObject NormalizarEntradasListas(string grafoJson, JObject entradasOriginais)
        {
            JObject entradas = entradasOriginais != null ? (JObject)entradasOriginais.DeepClone() : new JObject();
            try
            {
                JObject grafo = JObject.Parse(string.IsNullOrWhiteSpace(grafoJson) ? "{}" : grafoJson);
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
                    if (string.IsNullOrWhiteSpace(nomeEntrada)) continue;
                    JToken valorLista = entradas.GetValue(nomeEntrada, StringComparison.OrdinalIgnoreCase);
                    JArray lista = valorLista as JArray;
                    if (lista == null && valorLista != null && valorLista.Type == JTokenType.String)
                    {
                        try { lista = JArray.Parse(valorLista.ToString()); } catch { }
                    }
                    if (lista == null) continue;

                    string idLoop = Convert.ToString(loop["id"]);
                    List<string> camposEsperados = CamposItemLoop(nos, idLoop);
                    if (camposEsperados.Count == 0) continue;
                    string campoPrincipal = CampoPrincipalLista(camposEsperados);
                    JArray normalizada = new JArray();
                    foreach (JToken tokenItem in lista)
                    {
                        JObject item = tokenItem as JObject;
                        item = item != null ? (JObject)item.DeepClone() : new JObject { { campoPrincipal, tokenItem != null ? tokenItem.DeepClone() : JValue.CreateNull() } };

                        if (!ValorInformado(item.GetValue(campoPrincipal, StringComparison.OrdinalIgnoreCase)))
                        {
                            JToken alias = PrimeiroValorItem(item, new[] { "produto", "nomeProduto", "nome", "descricao", "termo", "codigo", "sCodigo" });
                            if (alias != null) item[campoPrincipal] = alias.DeepClone();
                        }
                        // Quantidade não informada vale 1 (ou a entrada "quantidade" do pedido, se houver). O cartão de
                        // confirmação do item mostra a quantidade antes de gravar, então dá para corrigir ali.
                        JToken quantidadePadrao = entradas.GetValue("quantidade", StringComparison.OrdinalIgnoreCase);
                        if (!ValorInformado(quantidadePadrao)) quantidadePadrao = new JValue(1);
                        AplicarPadraoItem(item, camposEsperados, "quantidade", quantidadePadrao);
                        AplicarPadraoItem(item, camposEsperados, "tabelaPreco", entradas.GetValue("tabelaPreco", StringComparison.OrdinalIgnoreCase));
                        AplicarPadraoItem(item, camposEsperados, "tipoItem", new JValue("produto"));
                        AplicarPadraoItem(item, camposEsperados, "descontoPercentual", new JValue(0));
                        normalizada.Add(item);
                    }
                    entradas[nomeEntrada] = normalizada;
                }
            }
            catch
            {
                return entradas;
            }
            return entradas;
        }

        private static List<string> CamposItemLoop(JArray nos, string idLoop)
        {
            List<string> campos = new List<string>();
            string prefixo = (idLoop ?? string.Empty) + ".item.";
            foreach (JToken tokenNo in nos ?? new JArray())
            {
                JObject no = tokenNo as JObject;
                foreach (JToken tokenMap in (no != null ? no["entradas"] as JArray : null) ?? new JArray())
                {
                    JObject mapa = tokenMap as JObject;
                    if (mapa == null || !string.Equals(Convert.ToString(mapa["origem"]), "passo", StringComparison.OrdinalIgnoreCase)) continue;
                    string referencia = Convert.ToString(mapa["valor"]).Trim();
                    if (!referencia.StartsWith(prefixo, StringComparison.OrdinalIgnoreCase)) continue;
                    string restante = referencia.Substring(prefixo.Length);
                    int separador = restante.IndexOfAny(new[] { '.', '[' });
                    string campo = separador >= 0 ? restante.Substring(0, separador) : restante;
                    if (!string.IsNullOrWhiteSpace(campo) && !ContemIgnorandoCaso(campos, campo)) campos.Add(campo);
                }
            }
            return campos;
        }

        private static string CampoPrincipalLista(List<string> campos)
        {
            foreach (string preferido in new[] { "produto", "item", "nomeProduto", "nome", "descricao", "termo", "codigo" })
            {
                foreach (string campo in campos ?? new List<string>()) if (string.Equals(campo, preferido, StringComparison.OrdinalIgnoreCase)) return campo;
            }
            return campos != null && campos.Count > 0 ? campos[0] : "valor";
        }

        private static JToken PrimeiroValorItem(JObject item, string[] campos)
        {
            foreach (string campo in campos ?? new string[0])
            {
                JToken valor = item != null ? item.GetValue(campo, StringComparison.OrdinalIgnoreCase) : null;
                if (ValorInformado(valor)) return valor;
            }
            return null;
        }

        private static void AplicarPadraoItem(JObject item, List<string> esperados, string campo, JToken padrao)
        {
            if (!ContemIgnorandoCaso(esperados, campo) || ValorInformado(item.GetValue(campo, StringComparison.OrdinalIgnoreCase)) || !ValorInformado(padrao)) return;
            item[campo] = padrao.DeepClone();
        }

        private static bool ContemIgnorandoCaso(List<string> valores, string procurado)
        {
            foreach (string valor in valores ?? new List<string>()) if (string.Equals(valor, procurado, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        private static string RaizEntradaLista(string valor)
        {
            string caminho = (valor ?? string.Empty).Trim();
            if (caminho.StartsWith("entrada.", StringComparison.OrdinalIgnoreCase)) caminho = caminho.Substring(8);
            int separador = caminho.IndexOfAny(new[] { '.', '[' });
            return separador >= 0 ? caminho.Substring(0, separador) : caminho;
        }

        private static string TipoCampo(JToken tipo)
        {
            JArray tipos = tipo as JArray;
            if (tipos != null)
            {
                foreach (JToken item in tipos)
                {
                    if (!string.Equals(Convert.ToString(item), "null", StringComparison.OrdinalIgnoreCase)) return Convert.ToString(item);
                }
            }
            return string.IsNullOrWhiteSpace(Convert.ToString(tipo)) ? "string" : Convert.ToString(tipo);
        }

        private static string PrimeiroTexto(params string[] valores)
        {
            foreach (string valor in valores ?? new string[0]) if (!string.IsNullOrWhiteSpace(valor)) return valor.Trim();
            return string.Empty;
        }

        private static IAWorkflowExecResponse ResponderExecucaoPersistida(IAWorkflowExecucao ex, string mensagemAlternativa)
        {
            if (ex == null) return null;
            JObject estado;
            try { estado = JObject.Parse(ex.EstadoJson ?? "{}"); } catch { estado = new JObject(); }
            JObject args = estado["argsPausa"] as JObject ?? new JObject();
            string mensagemPausa = Convert.ToString(args["mensagem"]);
            string mensagem = !string.IsNullOrWhiteSpace(mensagemPausa) ? mensagemPausa : mensagemAlternativa;
            if (string.IsNullOrWhiteSpace(mensagem)) mensagem = ex.Status == "PAUSADO" ? "A operação está aguardando sua ação." : "A operação está em andamento.";
            return Responder(ex.IdExecucaoIA, ex.Status, mensagem, ex.ResumoAcao, Deserializar(ex.TraceJson),
                estado["saidas"] as JObject ?? new JObject(), Convert.ToString(estado["tipoPausa"]), DadosPausaComResumo(args, estado["resumoPausa"] as JArray));
        }

        private static JObject MontarEstado(JObject entradas, IAWorkflowOutcome o, int idConversaIA)
        {
            return new JObject
            {
                { "idConversaIA", idConversaIA },
                { "fase", o.TipoPausa ?? o.Status ?? string.Empty },
                { "resolucoes", new JObject() },
                { "entradas", entradas ?? new JObject() },
                { "saidas", o.Saidas ?? new JObject() },
                { "noAtual", o.NoPausa ?? string.Empty },
                { "ferramentaPausa", o.FerramentaPausa ?? string.Empty },
                { "argsPausa", o.ArgsPausa ?? new JObject() },
                { "resumoPausa", o.ResumoPausa ?? new JArray() },
                { "tipoPausa", o.TipoPausa ?? string.Empty }
            };
        }

        // O resumo do que a escrita pausada vai gravar viaja ao lado dos argumentos, só na resposta para a tela. Os
        // argumentos persistidos (argsPausa) continuam sendo exatamente o que a ferramenta recebe ao confirmar.
        private static JObject DadosPausaComResumo(JObject args, JArray resumo)
        {
            if (args == null || resumo == null || resumo.Count == 0) return args;

            JObject dados = (JObject)args.DeepClone();
            dados["resumoConfirmacao"] = resumo.DeepClone();
            return dados;
        }

        private void Persistir(IAWorkflowExecucao ex, string status, string resumo, JObject entradas, JObject saidas,
                               string noPausa, string ferramentaPausa, JObject argsPausa, List<IAWorkflowTracePasso> trace, int idConversaIA, string tipoPausa,
                               JArray resumoPausa = null)
        {
            ex.Status = status;
            ex.IdConversaIA = idConversaIA;
            ex.ResumoAcao = resumo;
            ex.TraceJson = JsonConvert.SerializeObject(trace);
            ex.EstadoJson = new JObject
            {
                { "idConversaIA", idConversaIA },
                { "fase", tipoPausa ?? status ?? string.Empty },
                { "resolucoes", saidas ?? new JObject() },
                { "entradas", entradas ?? new JObject() },
                { "saidas", saidas ?? new JObject() },
                { "noAtual", noPausa ?? string.Empty },
                { "ferramentaPausa", ferramentaPausa ?? string.Empty },
                { "argsPausa", argsPausa ?? new JObject() },
                { "resumoPausa", resumoPausa ?? new JArray() },
                { "tipoPausa", tipoPausa ?? string.Empty }
            }.ToString(Formatting.None);
            _repositorio.SalvarWorkflowExecucao(ex);
        }

        private IAWorkflowExecResponse ProcessarAprovacao(IAWorkflowExecucao ex, JObject entradas, JObject saidas,
                                                          string noAtual, JObject args, List<IAWorkflowTracePasso> trace,
                                                          int idConversaIA, bool aprovado)
        {
            if (!_repositorio.MarcarWorkflowExecucaoExecutando(ex.IdExecucaoIA))
            {
                return Erro("Execução já foi processada ou não está mais pausada.");
            }

            string titulo = Convert.ToString(args["titulo"]);
            if (string.IsNullOrWhiteSpace(titulo))
            {
                titulo = "Aprovação humana";
            }

            string entradaAprovacao = args.ToString(Formatting.None);
            string saidaAprovacao = aprovado ? "Aprovado pelo usuário." : "Rejeitado pelo usuário.";
            trace.Add(new IAWorkflowTracePasso
            {
                No = noAtual,
                Tipo = "aprovacao",
                Titulo = titulo,
                Status = "OK",
                Entrada = entradaAprovacao,
                Saida = saidaAprovacao,
                EntradaCompleta = entradaAprovacao,
                SaidaCompleta = saidaAprovacao
            });

            saidas[noAtual] = new JObject
            {
                { "aprovado", aprovado },
                { "status", aprovado ? "APROVADO" : "REJEITADO" },
                { "idUsuario", IdUsuario() }
            };

            cls_IA_Auditoria.Registrar(idConversaIA, aprovado ? "WORKFLOW_APROVACAO_APROVADA" : "WORKFLOW_APROVACAO_REJEITADA", "ALERTA", new
            {
                idExecucaoIA = ex.IdExecucaoIA,
                no = noAtual,
                titulo = titulo
            });

            string prox = cls_IA_WorkflowEngine.ProximoNoAprovacao(ex.GrafoJson, noAtual, aprovado);
            if (string.IsNullOrWhiteSpace(prox))
            {
                string statusFinal = aprovado ? "CONCLUIDO" : "CANCELADO";
                string mensagemFinal = aprovado ? "Aprovação registrada; workflow concluído." : "Aprovação rejeitada; workflow encerrado.";
                Persistir(ex, statusFinal, null, entradas, saidas, null, null, null, trace, idConversaIA, null);
                return Responder(ex.IdExecucaoIA, statusFinal, mensagemFinal, null, trace, saidas);
            }

            IAWorkflow wf = new IAWorkflow { Nome = ex.WorkflowNome, GrafoJson = ex.GrafoJson, Escopo = "READ", IdRecursoNecessario = ex.IdRecursoNecessario };
            IAWorkflowOutcome o = new cls_IA_WorkflowEngine().Caminhar(idConversaIA, wf, saidas, entradas, prox);
            trace.AddRange(o.Trace);

            Persistir(ex, o.Status, o.Resumo, entradas, o.Saidas, o.NoPausa, o.FerramentaPausa, o.ArgsPausa, trace, idConversaIA, o.TipoPausa, o.ResumoPausa);

            if (o.Status == "CONCLUIDO") cls_IA_Auditoria.Registrar(idConversaIA, "WORKFLOW_CONCLUIDO", "INFO", new { idExecucaoIA = ex.IdExecucaoIA, workflow = ex.WorkflowNome });
            return Responder(ex.IdExecucaoIA, o.Status, o.Mensagem, o.Resumo, trace, o.Saidas, o.TipoPausa, DadosPausaComResumo(o.ArgsPausa, o.ResumoPausa));
        }

        private IAWorkflowExecResponse ProcessarEspera(IAWorkflowExecucao ex, JObject entradas, JObject saidas,
                                                       string noAtual, JObject args, List<IAWorkflowTracePasso> trace,
                                                       int idConversaIA)
        {
            string modo = Convert.ToString(args["modo"]);
            string msgEspera;
            if (string.Equals(modo, "data_hora", StringComparison.OrdinalIgnoreCase) && !cls_IA_WorkflowEngine.EsperaPodeContinuar(args, out msgEspera))
            {
                return Responder(ex.IdExecucaoIA, "PAUSADO", msgEspera, Convert.ToString(args["titulo"]), trace, saidas, "ESPERA", args);
            }

            if (!_repositorio.MarcarWorkflowExecucaoExecutando(ex.IdExecucaoIA))
            {
                return Erro("Execução já foi processada ou não está mais pausada.");
            }

            string titulo = Convert.ToString(args["titulo"]);
            if (string.IsNullOrWhiteSpace(titulo))
            {
                titulo = "Espera";
            }

            string entradaEspera = args.ToString(Formatting.None);
            string saidaEspera = string.Equals(modo, "evento", StringComparison.OrdinalIgnoreCase) ? "Evento liberado; seguindo o fluxo." : "Espera liberada; seguindo o fluxo.";
            trace.Add(new IAWorkflowTracePasso
            {
                No = noAtual,
                Tipo = "espera",
                Titulo = titulo,
                Status = "OK",
                Entrada = entradaEspera,
                Saida = saidaEspera,
                EntradaCompleta = entradaEspera,
                SaidaCompleta = saidaEspera
            });

            saidas[noAtual] = new JObject
            {
                { "aguardou", true },
                { "status", "LIBERADO" },
                { "modo", modo ?? string.Empty },
                { "dataHora", Convert.ToString(args["dataHora"]) },
                { "evento", Convert.ToString(args["evento"]) },
                { "liberadoEm", DateTime.Now.ToString("s") },
                { "idUsuario", IdUsuario() }
            };

            cls_IA_Auditoria.Registrar(idConversaIA, "WORKFLOW_ESPERA_LIBERADA", "INFO", new
            {
                idExecucaoIA = ex.IdExecucaoIA,
                no = noAtual,
                titulo = titulo,
                modo = modo
            });

            string prox = cls_IA_WorkflowEngine.ProximoNoEspera(ex.GrafoJson, noAtual);
            if (string.IsNullOrWhiteSpace(prox))
            {
                Persistir(ex, "CONCLUIDO", null, entradas, saidas, null, null, null, trace, idConversaIA, null);
                return Responder(ex.IdExecucaoIA, "CONCLUIDO", "Espera liberada; workflow concluído.", null, trace, saidas);
            }

            IAWorkflow wf = new IAWorkflow { Nome = ex.WorkflowNome, GrafoJson = ex.GrafoJson, Escopo = "READ", IdRecursoNecessario = ex.IdRecursoNecessario };
            IAWorkflowOutcome o = new cls_IA_WorkflowEngine().Caminhar(idConversaIA, wf, saidas, entradas, prox);
            trace.AddRange(o.Trace);

            Persistir(ex, o.Status, o.Resumo, entradas, o.Saidas, o.NoPausa, o.FerramentaPausa, o.ArgsPausa, trace, idConversaIA, o.TipoPausa, o.ResumoPausa);

            if (o.Status == "CONCLUIDO") cls_IA_Auditoria.Registrar(idConversaIA, "WORKFLOW_CONCLUIDO", "INFO", new { idExecucaoIA = ex.IdExecucaoIA, workflow = ex.WorkflowNome });
            return Responder(ex.IdExecucaoIA, o.Status, o.Mensagem, o.Resumo, trace, o.Saidas, o.TipoPausa, DadosPausaComResumo(o.ArgsPausa, o.ResumoPausa));
        }

        private void FinalizarErro(IAWorkflowExecucao ex, List<IAWorkflowTracePasso> trace, string no, string ferramenta, string msg)
        {
            trace.Add(new IAWorkflowTracePasso { No = no, Tipo = "ferramenta", Titulo = ferramenta, Status = "ERRO", Saida = msg, SaidaCompleta = msg });
            ex.Status = "ERRO";
            ex.TraceJson = JsonConvert.SerializeObject(trace);
            _repositorio.SalvarWorkflowExecucao(ex);
        }

        private static JToken ExtrairSaida(string resultadoJson)
        {
            try
            {
                JObject o = JObject.Parse(resultadoJson ?? "{}");
                JArray reg = o["registros"] as JArray;
                if (reg != null && reg.Count > 0) return reg[0];
                return o;
            }
            catch
            {
                return new JObject();
            }
        }

        private static string Snippet(string s)
        {
            return string.IsNullOrEmpty(s) || s.Length <= MaxSnippet ? s : s.Substring(0, MaxSnippet) + " ...";
        }

        private static string MensagemSucessoConfirmado(bool ehWrite, JToken saidaConfirmada, int tentativa, int totalTentativas)
        {
            return Snippet(MensagemSucessoConfirmadoCompleta(ehWrite, saidaConfirmada, tentativa, totalTentativas));
        }

        private static string MensagemSucessoConfirmadoCompleta(bool ehWrite, JToken saidaConfirmada, int tentativa, int totalTentativas)
        {
            if (ehWrite)
            {
                JToken saida = saidaConfirmada ?? new JObject { { "executado", true } };
                JObject obj = saida as JObject;
                if (obj != null)
                {
                    obj = (JObject)obj.DeepClone();
                    if (obj["executado"] == null) obj["executado"] = true;
                    if (obj["mensagem"] == null) obj["mensagem"] = "Escrita executada.";
                    if (totalTentativas > 1 && tentativa > 1)
                    {
                        obj["tentativa"] = tentativa;
                        obj["totalTentativas"] = totalTentativas;
                    }
                    return obj.ToString(Formatting.None);
                }

                return saida.ToString(Formatting.None);
            }

            string texto = saidaConfirmada != null ? saidaConfirmada.ToString(Formatting.None) : "{}";
            if (totalTentativas > 1 && tentativa > 1)
            {
                texto = "Sucesso na tentativa " + tentativa + " de " + totalTentativas + ". " + texto;
            }

            return texto;
        }

        private static string MensagemErroComTentativa(string mensagem, int tentativa, int totalTentativas)
        {
            mensagem = mensagem ?? "Erro no passo.";
            if (totalTentativas <= 1)
            {
                return mensagem;
            }

            return mensagem + " Tentativas realizadas: " + tentativa + " de " + totalTentativas + ".";
        }

        private static List<IAWorkflowTracePasso> Deserializar(string traceJson)
        {
            try { return JsonConvert.DeserializeObject<List<IAWorkflowTracePasso>>(traceJson ?? "[]") ?? new List<IAWorkflowTracePasso>(); }
            catch { return new List<IAWorkflowTracePasso>(); }
        }

        private static int IdUsuario()
        {
            int id; return int.TryParse(IDENTITY.Variaveis.idUsuario(), out id) ? id : 0;
        }

        private static IAWorkflowExecResponse Erro(string msg)
        {
            return new IAWorkflowExecResponse { Status = "ERRO", Mensagem = msg, Trace = new List<IAWorkflowTracePasso>() };
        }

        private static IAWorkflowExecResponse Responder(int idExec, string status, string mensagem, string resumo, List<IAWorkflowTracePasso> trace)
        {
            return Responder(idExec, status, mensagem, resumo, trace, null);
        }

        private static IAWorkflowExecResponse Responder(int idExec, string status, string mensagem, string resumo, List<IAWorkflowTracePasso> trace, JObject saidas)
        {
            return Responder(idExec, status, mensagem, resumo, trace, saidas, null);
        }

        private static IAWorkflowExecResponse Responder(int idExec, string status, string mensagem, string resumo, List<IAWorkflowTracePasso> trace, JObject saidas, string tipoPausa)
        {
            return Responder(idExec, status, mensagem, resumo, trace, saidas, tipoPausa, null);
        }

        private static IAWorkflowExecResponse Responder(int idExec, string status, string mensagem, string resumo, List<IAWorkflowTracePasso> trace, JObject saidas, string tipoPausa, JObject dadosPausa)
        {
            return new IAWorkflowExecResponse
            {
                IdExecucao = idExec,
                Status = status,
                Mensagem = mensagem,
                Resumo = resumo,
                Trace = trace ?? new List<IAWorkflowTracePasso>(),
                SaidasJson = saidas != null ? saidas.ToString(Formatting.None) : "{}",
                TipoPausa = tipoPausa ?? string.Empty,
                DadosPausaJson = dadosPausa != null ? dadosPausa.ToString(Formatting.None) : "{}"
            };
        }

        private IAWorkflowLoteResponse ProcessarEsperas(bool porEvento, string evento, int limite, int idConversaIA)
        {
            IAWorkflowLoteResponse lote = new IAWorkflowLoteResponse
            {
                Status = "OK",
                Execucoes = new List<IAWorkflowExecResponse>()
            };

            evento = (evento ?? string.Empty).Trim();
            if (porEvento && string.IsNullOrWhiteSpace(evento))
            {
                lote.Status = "ERRO";
                lote.Mensagem = "Informe o nome do evento.";
                return lote;
            }

            List<IAWorkflowExecucao> execucoes = _repositorio.ListarWorkflowExecucoesPausadas(limite);
            lote.TotalEncontrado = execucoes.Count;

            foreach (IAWorkflowExecucao ex in execucoes)
            {
                JObject estado;
                JObject args;
                if (!TentarLerEspera(ex, out estado, out args))
                {
                    continue;
                }

                string modo = Convert.ToString(args["modo"]);
                bool deveProcessar = false;
                if (porEvento)
                {
                    deveProcessar = string.Equals(modo, "evento", StringComparison.OrdinalIgnoreCase)
                        && EventoCompativel(Convert.ToString(args["evento"]), evento);
                }
                else if (string.Equals(modo, "data_hora", StringComparison.OrdinalIgnoreCase))
                {
                    string msg;
                    deveProcessar = cls_IA_WorkflowEngine.EsperaPodeContinuar(args, out msg);
                }

                if (!deveProcessar)
                {
                    continue;
                }

                IAWorkflowExecResponse resposta = Confirmar(ex.IdExecucaoIA, idConversaIA);
                lote.Execucoes.Add(resposta);
                lote.TotalProcessado++;

                string status = resposta != null ? resposta.Status : "ERRO";
                if (string.Equals(status, "CONCLUIDO", StringComparison.OrdinalIgnoreCase)) lote.TotalConcluido++;
                else if (string.Equals(status, "PAUSADO", StringComparison.OrdinalIgnoreCase)) lote.TotalPausado++;
                else if (string.Equals(status, "ERRO", StringComparison.OrdinalIgnoreCase)) lote.TotalErro++;
            }

            if (lote.TotalProcessado == 0)
            {
                lote.Mensagem = porEvento
                    ? "Nenhuma espera pendente encontrada para o evento \"" + evento + "\"."
                    : "Nenhuma espera por data/hora está pronta para continuar.";
            }
            else
            {
                lote.Mensagem = "Esperas processadas: " + lote.TotalProcessado + ".";
            }

            return lote;
        }

        private static bool TentarLerEspera(IAWorkflowExecucao ex, out JObject estado, out JObject args)
        {
            estado = new JObject();
            args = new JObject();
            if (ex == null || !string.Equals(ex.Status, "PAUSADO", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            try { estado = JObject.Parse(ex.EstadoJson ?? "{}"); }
            catch { return false; }

            if (!string.Equals(Convert.ToString(estado["tipoPausa"]), "ESPERA", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            args = estado["argsPausa"] as JObject ?? new JObject();
            return true;
        }

        private static bool EventoCompativel(string esperado, string recebido)
        {
            esperado = (esperado ?? string.Empty).Trim();
            recebido = (recebido ?? string.Empty).Trim();
            return esperado.Length > 0 && recebido.Length > 0 && esperado.Equals(recebido, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class IAWorkflowExecResponse
    {
        public int IdExecucao { get; set; }
        public int IdExecucaoIA
        {
            get { return IdExecucao; }
            set { IdExecucao = value; }
        }
        public string Status { get; set; }       // PAUSADO | CONCLUIDO | ERRO | CANCELADO
        public string TipoPausa { get; set; }    // FERRAMENTA | APROVACAO | ESPERA | ENTRADAS | ESCOLHA | SELECAO_MULTIPLA
        public string Mensagem { get; set; }
        public string Resumo { get; set; }
        public List<IAWorkflowTracePasso> Trace { get; set; }
        public string SaidasJson { get; set; }
        public string DadosPausaJson { get; set; }
    }

    public class IAWorkflowLoteResponse
    {
        public string Status { get; set; }
        public string Mensagem { get; set; }
        public int TotalEncontrado { get; set; }
        public int TotalProcessado { get; set; }
        public int TotalConcluido { get; set; }
        public int TotalPausado { get; set; }
        public int TotalErro { get; set; }
        public List<IAWorkflowExecResponse> Execucoes { get; set; }
    }
}
