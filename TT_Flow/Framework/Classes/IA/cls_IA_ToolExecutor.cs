using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TT.FrameWork;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;

namespace TT_Flow.FrameWork.IA
{
    public class cls_IA_ToolExecutor
    {
        private readonly cls_IA_Repositorio _repositorio;
        private readonly List<int> _idsArquivosPermitidos;
        private readonly int _maxTrechosArquivos;

        public cls_IA_ToolExecutor()
            : this(new List<int>(), 8)
        {
        }

        public cls_IA_ToolExecutor(List<int> idsArquivosPermitidos, int maxTrechosArquivos)
        {
            _repositorio = new cls_IA_Repositorio();
            _idsArquivosPermitidos = idsArquivosPermitidos ?? new List<int>();
            _maxTrechosArquivos = maxTrechosArquivos > 0 ? maxTrechosArquivos : 8;
        }

        public IAFerramentaResultado Executar(int idConversaIA, IAToolCall chamada)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            IAFerramentaResultado resultado = new IAFerramentaResultado
            {
                NomeFerramenta = chamada != null ? chamada.Nome : string.Empty,
                ArgumentosJson = chamada != null ? chamada.ArgumentosJson : string.Empty,
                Status = "ERRO",
                ResultadoJson = "{}"
            };

            try
            {
                cls_IA_Auditoria.Registrar(idConversaIA, "TOOL_REQUESTED", "INFO", new
                {
                    ferramenta = resultado.NomeFerramenta,
                    argumentos = resultado.ArgumentosJson
                });

                IAFerramentaDefinicao ferramenta = cls_IA_ToolRegistry.Obter(resultado.NomeFerramenta);
                if (ferramenta == null)
                {
                    resultado.Status = "NEGADO";
                    resultado.Erro = cls_IA_ToolRegistry.DiagnosticarDisponibilidade(resultado.NomeFerramenta, true);
                    if (string.IsNullOrWhiteSpace(resultado.Erro)) resultado.Erro = "Ferramenta inexistente ou indisponível.";
                    resultado.ResultadoJson = JsonErro(resultado.Erro);
                    return resultado;
                }

                resultado.IdFerramentaIA = ferramenta.IdFerramentaIA;
                resultado.IdRecursoValidado = ferramenta.IdRecursoNecessario;

                if (!FUNCOES.ValidaPermissao(ferramenta.IdRecursoNecessario, false))
                {
                    resultado.Status = "NEGADO";
                    resultado.Erro = "Usuario sem permissao para executar esta ferramenta.";
                    resultado.ResultadoJson = JsonErro(resultado.Erro);
                    cls_IA_Auditoria.Registrar(idConversaIA, "TOOL_DENIED", "ALERTA", new
                    {
                        ferramenta = ferramenta.Nome,
                        recurso = ferramenta.IdRecursoNecessario
                    });
                    return resultado;
                }

                if (string.Equals(ferramenta.Escopo, "WRITE", StringComparison.OrdinalIgnoreCase) &&
                    !FUNCOES.ValidaPermissao(Permissao.IA.ExecutarAcoes, false))
                {
                    resultado.Status = "NEGADO";
                    resultado.Erro = "Usuario sem permissao para executar acoes pela IA.";
                    resultado.ResultadoJson = JsonErro(resultado.Erro);
                    cls_IA_Auditoria.Registrar(idConversaIA, "TOOL_DENIED", "ALERTA", new
                    {
                        ferramenta = ferramenta.Nome,
                        recurso = Permissao.IA.ExecutarAcoes
                    });
                    return resultado;
                }

                JObject argumentos = chamada != null && chamada.Argumentos != null
                    ? chamada.Argumentos
                    : ParseArgumentos(resultado.ArgumentosJson);

                cls_IA_Auditoria.Registrar(idConversaIA, "TOOL_ALLOWED", "INFO", new
                {
                    ferramenta = ferramenta.Nome,
                    recurso = ferramenta.IdRecursoNecessario
                });

                if (chamada != null && chamada.Argumentos != null)
                {
                    resultado.ArgumentosJson = argumentos.ToString(Formatting.None);
                }

                // Workflow composto: executa o grafo pelo service e pode retornar concluido ou pausado.
                if (ferramenta.EhWorkflow())
                {
                    resultado.ResultadoJson = ExecutarWorkflowComposto(idConversaIA, ferramenta, argumentos, resultado);
                }
                // Ferramenta generica (configurada 100% pela tela): motor unico via SP + mapa + colunas.
                else if (ferramenta.EhGenerica())
                {
                    bool ehWrite = string.Equals(ferramenta.Escopo, "WRITE", StringComparison.OrdinalIgnoreCase);
                    // Confirmacao: sempre em WRITE; e tambem quando a ferramenta esta marcada como "pedir
                    // autorizacao" (RequerConfirmacao), inclusive READ. O READ so roda apos o OK do usuario
                    // (cls_IA_Chat.ConfirmarAcao reexecuta a consulta e responde com os dados).
                    bool precisaConfirmar = ehWrite || ferramenta.RequerConfirmacao;
                    resultado.ResultadoJson = precisaConfirmar
                        ? PrepararFerramentaGenerica(idConversaIA, ferramenta, argumentos, resultado)
                        : ExecutarFerramentaGenerica(idConversaIA, ferramenta, argumentos);
                }
                else
                {
                    // Dispatch por nome: mapa "ferramenta -> handler" (ferramentas internas, execucao no codigo).
                    Dictionary<string, Func<string>> handlers = new Dictionary<string, Func<string>>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "sistema_buscar_recurso_menu",         () => ExecutarSistemaBuscarRecursoMenu(ferramenta, argumentos) },
                        { "sistema_buscar_informacao",           () => ExecutarSistemaBuscarInformacao(idConversaIA, ferramenta, argumentos) },
                        { "pedidos_consultar_resumo",            () => ExecutarPedidosConsultarResumo(ferramenta, argumentos, resultado) },
                        { "pedidos_consultar_historico",         () => ExecutarPedidosConsultarHistorico(ferramenta, argumentos, resultado) },
                        { "pedidos_listar_arquivos",             () => ExecutarPedidosListarArquivos(ferramenta, argumentos, resultado) },
                        { "pedidos_pesquisar",                   () => ExecutarPedidosPesquisar(ferramenta, argumentos, resultado) },
                        { "pedidos_adicionar_observacao",        () => PrepararPedidosAdicionarObservacao(ferramenta, argumentos, resultado) },
                        { "mensagens_criar_rascunho",            () => PrepararMensagensCriarRascunho(ferramenta, argumentos, resultado) },
                        { "produtos_consultar",                  () => ExecutarProdutosConsultar(ferramenta, argumentos) },
                        { "parceiros_consultar",                 () => ExecutarParceirosConsultar(ferramenta, argumentos) },
                        { "mensagens_consultar",                 () => ExecutarMensagensConsultar(ferramenta, argumentos) },
                        { "arquivos_buscar_trechos",             () => ExecutarArquivosBuscarTrechos(idConversaIA, ferramenta, argumentos) },
                        { "conhecimento_buscar",                 () => ExecutarConhecimentoBuscar(idConversaIA, ferramenta, argumentos) },
                        { "workflows_listar_disponiveis",        () => ExecutarWorkflowsListarDisponiveis(ferramenta) },
                        { "orcamento_tipos_listar",              () => ExecutarOrcamentoTiposListar(ferramenta, argumentos) },
                        { "orcamento_tipo_detalhar",             () => ExecutarOrcamentoTipoDetalhar(ferramenta, argumentos) },
                        { "orcamento_escopos_listar",            () => ExecutarOrcamentoEscoposListar(ferramenta, argumentos) },
                        { "orcamento_servicos_recursos_listar",  () => ExecutarOrcamentoServicosRecursosListar(ferramenta, argumentos) },
                        { "orcamento_clientes_buscar",           () => ExecutarOrcamentoClientesBuscar(ferramenta, argumentos) },
                        { "clientes_resolver_para_orcamento",    () => ExecutarClientesResolverParaOrcamento(ferramenta, argumentos) },
                        { "orcamento_cliente_detalhar",          () => ExecutarOrcamentoClienteDetalhar(ferramenta, argumentos) },
                        { "clientes_criar_para_orcamento",       () => PrepararClientesCriarParaOrcamento(ferramenta, argumentos, resultado) },
                        { "clientes_completar_para_orcamento",   () => PrepararClientesCompletarParaOrcamento(ferramenta, argumentos, resultado) },
                        { "cep_consultar",                       () => ExecutarCepConsultar(ferramenta, argumentos) },
                        { "orcamento_empresas_buscar",           () => ExecutarOrcamentoEmpresasBuscar(ferramenta, argumentos) },
                        { "orcamento_vendedores_buscar",         () => ExecutarOrcamentoVendedoresBuscar(ferramenta, argumentos) },
                        { "orcamento_usuarios_resolver",          () => ExecutarOrcamentoUsuariosResolver(ferramenta, argumentos) },
                        { "orcamento_empresa_detalhar",          () => ExecutarOrcamentoEmpresaDetalhar(ferramenta, argumentos) },
                        { "orcamento_tabelas_preco_buscar",      () => ExecutarOrcamentoTabelasPrecoBuscar(ferramenta, argumentos) },
                        { "orcamento_tabelas_preco_listar",      () => ExecutarOrcamentoTabelasPrecoListar(ferramenta, argumentos) },
                        { "orcamento_tipos_envio_buscar",        () => ExecutarOrcamentoTiposEnvioBuscar(ferramenta, argumentos) },
                        { "orcamento_tabela_preco_detalhar",     () => ExecutarOrcamentoTabelaPrecoDetalhar(ferramenta, argumentos) },
                        { "orcamento_itens_disponiveis_listar",  () => ExecutarOrcamentoItensDisponiveisListar(ferramenta, argumentos) },
                        { "orcamento_produtos_buscar",           () => ExecutarOrcamentoProdutosBuscar(ferramenta, argumentos) },
                        { "orcamento_produto_preco_resolver",    () => ExecutarOrcamentoProdutoPrecoResolver(ferramenta, argumentos) },
                        { "orcamento_produtos_sugestoes_listar", () => ExecutarOrcamentoProdutosSugestoesListar(ferramenta, argumentos) },
                        { "orcamento_produto_codigo_consultar",  () => ExecutarOrcamentoProdutoCodigoConsultar(ferramenta, argumentos) },
                        { "orcamento_produto_composicao_consultar", () => ExecutarOrcamentoProdutoComposicaoConsultar(ferramenta, argumentos) },
                        { "orcamento_condicao_pagamento_detalhar", () => ExecutarOrcamentoCondicaoPagamentoDetalhar(ferramenta, argumentos) },
                        { "orcamento_criar_cabecalho",           () => PrepararOrcamentoCriarCabecalho(ferramenta, argumentos, resultado) },
                        { "orcamento_duplicar",                  () => PrepararOrcamentoDuplicar(ferramenta, argumentos, resultado) },
                        { "orcamento_adicionar_item",            () => PrepararOrcamentoAdicionarItem(ferramenta, argumentos, resultado) },
                        { "orcamento_salvar_resposta_escopo",    () => PrepararOrcamentoSalvarRespostaEscopo(ferramenta, argumentos, resultado) },
                        { "crm_negocios_buscar",                 () => ExecutarCrmNegociosBuscar(ferramenta, argumentos) },
                        { "crm_negocio_detalhar",                () => ExecutarCrmNegocioDetalhar(ferramenta, argumentos) },
                        { "crm_negocio_criar_para_orcamento",    () => PrepararCrmNegocioCriarParaOrcamento(ferramenta, argumentos, resultado) },
                        { "crm_followup_criar",                  () => PrepararCrmFollowupCriar(ferramenta, argumentos, resultado) },
                        { "rrhh_consultar_colaborador_basico",   () => ExecutarRRHHConsultarColaboradorBasico(ferramenta, argumentos) },
                        { "financeiro_consultar_contas_receber", () => ExecutarFinanceiroContasReceber(ferramenta, argumentos) },
                        { "financeiro_consultar_contas_pagar",   () => ExecutarFinanceiroContasPagar(ferramenta, argumentos) }
                    };

                    Func<string> handler;
                    if (!handlers.TryGetValue(ferramenta.Nome ?? string.Empty, out handler))
                    {
                        resultado.Status = "NEGADO";
                        resultado.Erro = "Ferramenta nao mapeada no executor.";
                        resultado.ResultadoJson = JsonErro(resultado.Erro);
                        return resultado;
                    }

                    resultado.ResultadoJson = handler();
                }

                if (resultado.Status != "NEGADO" && resultado.Status != "PENDENTE_CONFIRMACAO" && resultado.Status != "PAUSADO")
                {
                    resultado.Status = ResultadoIndicaSucesso(resultado.ResultadoJson) ? "SUCESSO" : "ERRO";
                    if (resultado.Status == "ERRO" && string.IsNullOrWhiteSpace(resultado.Erro))
                    {
                        resultado.Erro = MensagemErroJson(resultado.ResultadoJson, "Falha ao executar a ferramenta.");
                    }
                }
            }
            catch (Exception ex)
            {
                resultado.Status = "ERRO";
                resultado.Erro = ex.Message;
                resultado.ResultadoJson = JsonErro("Erro ao executar ferramenta: " + ex.Message);
                cls_IA_Auditoria.Registrar(idConversaIA, "TOOL_ERROR", "ERRO", new
                {
                    ferramenta = resultado.NomeFerramenta,
                    erro = ex.Message
                });
            }
            finally
            {
                stopwatch.Stop();
                resultado.DuracaoMs = stopwatch.ElapsedMilliseconds;

                int idChamadaFerramenta = 0;
                try
                {
                    idChamadaFerramenta = _repositorio.SalvarChamadaFerramenta(idConversaIA, resultado);
                }
                catch (Exception ex)
                {
                    cls_IA_Auditoria.Registrar(idConversaIA, "TOOL_LOG_ERROR", "ERRO", new
                    {
                        ferramenta = resultado.NomeFerramenta,
                        erro = ex.Message
                    });
                }

                if (resultado.Status == "PENDENTE_CONFIRMACAO")
                {
                    try
                    {
                        resultado.IdAprovacaoIA = _repositorio.SalvarAprovacao(idChamadaFerramenta, resultado.ResumoAcao, resultado.ArgumentosJson);

                        JObject jsonPendente = ParseArgumentos(resultado.ResultadoJson);
                        jsonPendente["idAprovacaoIA"] = resultado.IdAprovacaoIA;
                        resultado.ResultadoJson = jsonPendente.ToString(Formatting.None);

                        cls_IA_Auditoria.Registrar(idConversaIA, "ACTION_CONFIRMATION_REQUIRED", "INFO", new
                        {
                            idAprovacaoIA = resultado.IdAprovacaoIA,
                            ferramenta = resultado.NomeFerramenta,
                            resumo = resultado.ResumoAcao
                        });
                    }
                    catch (Exception ex)
                    {
                        resultado.Status = "ERRO";
                        resultado.Erro = "Falha ao registrar a acao pendente: " + ex.Message;
                        resultado.ResultadoJson = JsonErro(resultado.Erro);
                        resultado.IdAprovacaoIA = 0;
                    }
                }

                resultado.ResultadoModeloJson = ProjetarResultadoModelo(resultado.NomeFerramenta, resultado.ResultadoJson);

                cls_IA_Auditoria.Registrar(idConversaIA, "TOOL_COMPLETED", resultado.Status == "SUCESSO" ? "INFO" : "ALERTA", new
                {
                    ferramenta = resultado.NomeFerramenta,
                    status = resultado.Status,
                    duracaoMs = resultado.DuracaoMs,
                    erro = resultado.Erro
                });
            }

            return resultado;
        }

        private static string ProjetarResultadoModelo(string nomeFerramenta, string resultadoJson)
        {
            if (string.IsNullOrWhiteSpace(resultadoJson)) return "{}";
            try
            {
                JObject origem = JObject.Parse(resultadoJson);
                if (origem["statusWorkflow"] != null)
                {
                    return ProjetarWorkflowParaModelo(origem).ToString(Formatting.None);
                }
                if (!string.Equals(nomeFerramenta, "orcamento_produtos_buscar", StringComparison.OrdinalIgnoreCase))
                {
                    return resultadoJson;
                }

                JObject compacto = new JObject();
                string[] metadados = { "sucesso", "ferramenta", "termo", "totalEncontrado", "totalRetornado", "encontrado", "unicoProvavel", "precisaEscolha", "mensagem" };
                foreach (string campo in metadados) if (origem[campo] != null) compacto[campo] = origem[campo].DeepClone();

                JArray registros = new JArray();
                foreach (JToken token in (origem["registros"] as JArray) ?? new JArray())
                {
                    JObject item = token as JObject;
                    if (item == null) continue;
                    registros.Add(SelecionarCampos(item, new[]
                    {
                        "idItem", "idProduto", "idTabela", "sCodigo", "sDscProduto", "sCodigoComDescricao",
                        "sUnidade", "idTipoProduto", "sDscTipoProduto", "sDscGrupo", "sDscFamilia",
                        "scoreResolucao", "link"
                    }));
                }
                compacto["registros"] = registros;

                JObject melhor = origem["melhorResultado"] as JObject;
                compacto["melhorResultado"] = melhor != null
                    ? SelecionarCampos(melhor, new[] { "idItem", "idProduto", "idTabela", "sCodigo", "sDscProduto", "sCodigoComDescricao", "sUnidade", "idTipoProduto", "sDscTipoProduto", "scoreResolucao", "link" })
                    : new JObject();

                JArray opcoes = new JArray();
                foreach (JToken token in (origem["opcoes"] as JArray) ?? new JArray())
                {
                    JObject opcao = token as JObject;
                    if (opcao == null) continue;
                    JObject registro = opcao["registro"] as JObject;
                    opcoes.Add(new JObject
                    {
                        { "valor", opcao["valor"] != null ? opcao["valor"].DeepClone() : JValue.CreateString(string.Empty) },
                        { "rotulo", opcao["rotulo"] != null ? opcao["rotulo"].DeepClone() : JValue.CreateString(string.Empty) },
                        { "descricao", opcao["descricao"] != null ? opcao["descricao"].DeepClone() : JValue.CreateString(string.Empty) },
                        { "registro", registro != null ? SelecionarCampos(registro, new[] { "idItem", "idProduto", "idTabela", "sCodigo", "sDscProduto", "sCodigoComDescricao", "sUnidade", "idTipoProduto", "sDscTipoProduto", "scoreResolucao", "link" }) : new JObject() }
                    });
                }
                compacto["opcoes"] = opcoes;
                return compacto.ToString(Formatting.None);
            }
            catch
            {
                return resultadoJson;
            }
        }

        private static JObject ProjetarWorkflowParaModelo(JObject origem)
        {
            JObject compacto = new JObject();
            string[] metadados =
            {
                "sucesso", "workflow", "idWorkflowIA", "idExecucaoIA", "statusWorkflow",
                "tipoPausa", "mensagem", "acaoPendente", "resumo", "instrucao"
            };
            foreach (string campo in metadados) if (origem[campo] != null) compacto[campo] = origem[campo].DeepClone();

            JObject pausa = origem["dadosPausa"] as JObject;
            if (pausa != null)
            {
                JObject pausaCompacta = (JObject)pausa.DeepClone();
                pausaCompacta.Remove("resultado");
                JArray opcoes = pausaCompacta["opcoes"] as JArray;
                if (opcoes != null)
                {
                    foreach (JToken token in opcoes)
                    {
                        JObject opcao = token as JObject;
                        if (opcao != null) opcao.Remove("registro");
                    }
                }
                compacto["dadosPausa"] = pausaCompacta;
            }

            JObject diagnosticoErro = origem["diagnosticoErro"] as JObject;
            if (diagnosticoErro != null && diagnosticoErro.Count > 0)
            {
                compacto["diagnosticoErro"] = diagnosticoErro.DeepClone();
            }

            JArray saidas = new JArray();
            JArray links = new JArray();
            ColetarSaidasWorkflowModelo(origem["saidas"], "saidas", saidas, links, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
            compacto["saidasConfirmadas"] = saidas;
            compacto["links"] = links;
            return compacto;
        }

        private static void ColetarSaidasWorkflowModelo(JToken token, string caminho, JArray saidas, JArray links, HashSet<string> linksVistos)
        {
            if (token == null || saidas == null || saidas.Count >= 60) return;
            JObject obj = token as JObject;
            if (obj != null)
            {
                JObject item = new JObject { { "origem", caminho ?? string.Empty } };
                string[] campos =
                {
                    "idOrcamento", "idPedido", "nNumeroOrcamento", "idRegistroCRM", "idCRM", "idCliente",
                    "referencia", "clienteNome", "idProduto", "codigoProduto", "descricao", "tipoItem",
                    "quantidade", "valorUnitario", "valorTotal", "origemPreco", "totalProdutos",
                    "totalServicos", "valorOriginal", "link", "pdfLink", "mensagem"
                };
                foreach (string campo in campos) if (obj[campo] != null) item[campo] = obj[campo].DeepClone();
                if (item.Count > 1) saidas.Add(item);

                string link = obj["link"] != null ? obj["link"].ToString().Trim() : string.Empty;
                if (!string.IsNullOrWhiteSpace(link) && linksVistos.Add(link)) links.Add(link);
                foreach (JProperty prop in obj.Properties())
                {
                    ColetarSaidasWorkflowModelo(prop.Value, caminho + "." + prop.Name, saidas, links, linksVistos);
                }
                return;
            }

            JArray arr = token as JArray;
            if (arr == null) return;
            for (int i = 0; i < arr.Count; i++) ColetarSaidasWorkflowModelo(arr[i], caminho + "[" + i + "]", saidas, links, linksVistos);
        }

        private static JObject SelecionarCampos(JObject origem, string[] campos)
        {
            JObject destino = new JObject();
            foreach (string campo in campos ?? new string[0])
            {
                if (origem[campo] != null) destino[campo] = origem[campo].DeepClone();
            }
            return destino;
        }

        private static string ExecutarSistemaBuscarRecursoMenu(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 120);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (termo.Length < 2)
            {
                return JsonErro("Informe ao menos 2 caracteres para buscar recursos.");
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@idUsuario", IDENTITY.Variaveis.idUsuario() },
                { "@sDscRecurso", termo },
                { "@sSalvaLog", "N" }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Flow_Valida_Recursos_x_Usuarios", parametros, false);
            return TabelaParaJson(ferramenta, tb, limite, new[] { "sDscRecurso", "sURL" }, null);
        }

        private static string ExecutarSistemaBuscarInformacao(int idConversaIA, IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 120);
            string tipo = Texto(argumentos, "tipo", 20).ToUpperInvariant();
            int limiteTotal = Limite(argumentos, ferramenta.MaxRegistros);

            if (termo.Length < 2)
            {
                return JsonErro("Informe ao menos 2 caracteres para a busca universal.");
            }

            if (tipo != "MENU" && tipo != "PRODUTO" && tipo != "PARCEIRO" && tipo != "MENSAGEM" && tipo != "PEDIDO")
            {
                tipo = "TODOS";
            }

            JArray registros = new JArray();
            JArray modulosConsultados = new JArray();
            JArray modulosSemPermissao = new JArray();
            int limitePorModulo = tipo == "TODOS" ? Math.Max(3, Math.Min(6, limiteTotal)) : limiteTotal;

            if (DeveBuscar(tipo, "MENU"))
            {
                modulosConsultados.Add("MENU");
                IAFerramentaDefinicao ferramentaMenu = cls_IA_ToolRegistry.Obter("sistema_buscar_recurso_menu") ?? ferramenta;
                AdicionarRegistrosBuscaUniversal(
                    registros,
                    "Menu",
                    "sistema_buscar_recurso_menu",
                    ExecutarSistemaBuscarRecursoMenu(ferramentaMenu, ArgsPesquisa(termo, limitePorModulo)),
                    limiteTotal);
            }

            if (DeveBuscar(tipo, "PRODUTO"))
            {
                if (FUNCOES.ValidaPermissao(Permissao.Produtos.Consultar, false))
                {
                    modulosConsultados.Add("PRODUTO");
                    IAFerramentaDefinicao ferramentaProduto = cls_IA_ToolRegistry.Obter("produtos_consultar") ?? ferramenta;
                    AdicionarRegistrosBuscaUniversal(
                        registros,
                        "Produto",
                        "produtos_consultar",
                        ExecutarProdutosConsultar(ferramentaProduto, ArgsPesquisa(termo, limitePorModulo)),
                        limiteTotal);
                }
                else
                {
                    modulosSemPermissao.Add("PRODUTO");
                }
            }

            if (DeveBuscar(tipo, "PARCEIRO"))
            {
                if (FUNCOES.ValidaPermissao(Permissao.Parceiros.Consultar, false))
                {
                    modulosConsultados.Add("PARCEIRO");
                    IAFerramentaDefinicao ferramentaParceiro = cls_IA_ToolRegistry.Obter("parceiros_consultar") ?? ferramenta;
                    JObject args = ArgsPesquisa(termo, limitePorModulo);
                    args["idTipo"] = 0;
                    AdicionarRegistrosBuscaUniversal(
                        registros,
                        "Parceiro",
                        "parceiros_consultar",
                        ExecutarParceirosConsultar(ferramentaParceiro, args),
                        limiteTotal);
                }
                else
                {
                    modulosSemPermissao.Add("PARCEIRO");
                }
            }

            if (DeveBuscar(tipo, "MENSAGEM"))
            {
                if (FUNCOES.ValidaPermissao(Permissao.Mensagens.Consultar, false))
                {
                    modulosConsultados.Add("MENSAGEM");
                    IAFerramentaDefinicao ferramentaMensagem = cls_IA_ToolRegistry.Obter("mensagens_consultar") ?? ferramenta;
                    JObject args = ArgsPesquisa(termo, limitePorModulo);
                    args["categoria"] = "TODOS";
                    AdicionarRegistrosBuscaUniversal(
                        registros,
                        "Mensagem",
                        "mensagens_consultar",
                        ExecutarMensagensConsultar(ferramentaMensagem, args),
                        limiteTotal);
                }
                else
                {
                    modulosSemPermissao.Add("MENSAGEM");
                }
            }

            if (DeveBuscar(tipo, "PEDIDO") && Inteiro(termo, 0) > 0)
            {
                if (FUNCOES.ValidaPermissao(Permissao.Pedidos.Consultar, false))
                {
                    modulosConsultados.Add("PEDIDO");
                    IAFerramentaDefinicao ferramentaPedido = cls_IA_ToolRegistry.Obter("pedidos_consultar_resumo") ?? ferramenta;
                    JObject args = new JObject { { "idPedido", Inteiro(termo, 0) } };
                    AdicionarRegistrosBuscaUniversal(
                        registros,
                        "Pedido",
                        "pedidos_consultar_resumo",
                        ExecutarPedidosConsultarResumo(ferramentaPedido, args, new IAFerramentaResultado()),
                        limiteTotal);
                }
                else
                {
                    modulosSemPermissao.Add("PEDIDO");
                }
            }

            cls_IA_Auditoria.Registrar(idConversaIA, "BUSCA_UNIVERSAL_EXECUTADA", "INFO", new
            {
                termo = termo,
                tipo = tipo,
                totalRetornado = registros.Count,
                modulosConsultados = modulosConsultados,
                modulosSemPermissao = modulosSemPermissao
            });

            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "tipoSolicitado", tipo },
                { "termo", termo },
                { "modulosConsultados", modulosConsultados },
                { "modulosSemPermissao", modulosSemPermissao },
                { "totalEncontrado", registros.Count },
                { "totalRetornado", registros.Count },
                { "registros", registros }
            }.ToString(Formatting.None);
        }

        // Aceita id interno, numero visivel do pedido ou controle TT; devolve o idPedido real.
        // Retorna 0 quando nada foi encontrado ou quando ha ambiguidade (candidatos preenchidos).
        private static int ResolverIdPedido(int valorInformado, out JArray candidatos)
        {
            candidatos = new JArray();

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "RESOLVER_PEDIDO" },
                { "@idPedido", valorInformado.ToString() }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_IA_Chat", parametros, false);

            // Prioridade de resolucao: NUMERO visivel do pedido > controle TT > id interno.
            // O usuario quase sempre digita o numero exibido na tela (ex.: "001854"), nao o id interno.
            // Sem essa ordem, um pedido com idPedido = 1854 vencia o pedido cujo numero e 1854.
            List<DataRow> porNumero = new List<DataRow>();
            List<DataRow> porId = new List<DataRow>();
            List<DataRow> porControle = new List<DataRow>();

            foreach (DataRow row in tb.Rows)
            {
                int nNum = Inteiro(Valor(row, "nNumeroPedido"), 0);
                int idPed = Inteiro(Valor(row, "idPedido"), 0);

                candidatos.Add(new JObject
                {
                    { "idPedido", idPed },
                    { "nNumeroPedido", nNum },
                    { "idTipo", Inteiro(Valor(row, "idTipo"), 0) }
                });

                if (nNum == valorInformado)
                {
                    porNumero.Add(row);
                }
                else if (idPed == valorInformado)
                {
                    porId.Add(row);
                }
                else
                {
                    // Presente no resultado sem casar numero nem id: casou pelo controle TT
                    porControle.Add(row);
                }
            }

            if (porNumero.Count == 1)
            {
                return Inteiro(Valor(porNumero[0], "idPedido"), 0);
            }

            if (porNumero.Count == 0)
            {
                if (porControle.Count == 1)
                {
                    return Inteiro(Valor(porControle[0], "idPedido"), 0);
                }

                if (porControle.Count == 0 && porId.Count == 1)
                {
                    return Inteiro(Valor(porId[0], "idPedido"), 0);
                }
            }

            // Ambiguo (varios numeros iguais entre tipos, ou controle/id multiplos): pede desambiguacao
            return 0;
        }

        private static string ResolverPedidoOuErro(int valorInformado, out int idPedido)
        {
            JArray candidatos;
            idPedido = ResolverIdPedido(valorInformado, out candidatos);

            if (idPedido > 0)
            {
                return null;
            }

            if (candidatos.Count > 1)
            {
                return new JObject
                {
                    { "sucesso", false },
                    { "erro", "Mais de um pedido corresponde ao numero informado. Pergunte ao usuario qual e o correto antes de continuar." },
                    { "candidatos", candidatos }
                }.ToString(Formatting.None);
            }

            return JsonErro("Nenhum pedido encontrado com id, numero ou controle TT igual a " + valorInformado + ".");
        }

        private static string ExecutarPedidosConsultarResumo(IAFerramentaDefinicao ferramenta, JObject argumentos, IAFerramentaResultado resultado)
        {
            int idPedido = Inteiro(argumentos, "idPedido", 0);
            if (idPedido <= 0)
            {
                return JsonErro("idPedido invalido.");
            }

            string erroResolucao = ResolverPedidoOuErro(idPedido, out idPedido);
            if (erroResolucao != null)
            {
                return erroResolucao;
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA PEDIDO" },
                { "@idPedido", idPedido.ToString() },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
            };

            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", parametros);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return JsonErro("Pedido nao encontrado ou sem acesso para consulta.");
            }

            DataRow row = ds.Tables[0].Rows[0];
            int idTipo = Inteiro(Valor(row, "idTipo"), 0);
            if (idTipo == 7 && !FUNCOES.ValidaPermissao(Permissao.Compras.Pedido_Compras.Consultar, false))
            {
                resultado.Status = "NEGADO";
                resultado.Erro = "Usuario sem permissao para consultar pedido de compras.";
                return JsonErro(resultado.Erro);
            }

            JObject registro = new JObject();
            // Allowlist ampliado: TabelaParaJson/AdicionarColunas ignoram colunas ausentes,
            // entao so aparece o que o CONSULTA PEDIDO realmente retorna para o tipo do pedido.
            AdicionarColunas(registro, row, new[]
            {
                "idPedido",
                "nNumeroPedido",
                "sPedidoCliente",
                "sPedidoCompras",
                "idTipo",
                "sDscTipo",
                "idCliente",
                "sRazaoSocial",
                "sNomeFantasia",
                "nControleTT",
                "sReferencia",
                "sStatus",
                "sDscStatus",
                "sDscFluxo",
                "sDscDepartamento",
                "sDscTipoEnvio",
                "sDscModal",
                "sDscVendedor",
                "sDscUsuarioVendedor",
                "dtPedido",
                "dtPrevisaoEntrega",
                "dtPrevisaoEntregaFormatada",
                "dtEstimativaEntrega",
                "sPossuiProduto",
                "sPossuiServico",
                "vlrTotal",
                "dtCadastro",
                "dtAtualizacao",
                "sDscUsuarioAtualizacao"
            });
            registro["link"] = "/App/Paginas/Pedidos_Detalhe.aspx?id=" + idPedido;

            return new JObject
            {
                { "sucesso", true },
                { "totalEncontrado", 1 },
                { "totalRetornado", 1 },
                { "registros", new JArray(registro) }
            }.ToString(Formatting.None);
        }

        private static string ExecutarPedidosPesquisar(IAFerramentaDefinicao ferramenta, JObject argumentos, IAFerramentaResultado resultado)
        {
            string termo = Texto(argumentos, "termo", 120);
            // Tipo 2 = pedido de venda: e o default do proprio proc e onde estao os pedidos com cliente.
            int tipo = Inteiro(argumentos, "tipo", 2);
            if (tipo != 1 && tipo != 2 && tipo != 3 && tipo != 6 && tipo != 7)
            {
                tipo = 2;
            }

            if (tipo == 7 && !FUNCOES.ValidaPermissao(Permissao.Compras.Pedido_Compras.Consultar, false))
            {
                resultado.Status = "NEGADO";
                resultado.Erro = "Usuario sem permissao para consultar pedidos de compras.";
                return JsonErro(resultado.Erro);
            }

            // O @sPesquisa da procedure so casa referencia, controle TT e numero do pedido de compras
            // (NAO o nome do cliente). Para filtrar por cliente resolvemos o nome -> idCliente e passamos
            // @idCliente. ATENCAO: a procedure IGNORA @idCliente quando @idTipo = 2 (condicao
            // "@idTipo = 2 OR a.idCliente = @idCliente"); por isso, quando ha filtro de cliente, tambem
            // filtramos o resultado por cliente em C# (FiltrarPorCliente) para nao devolver outros clientes.
            string cliente = Texto(argumentos, "cliente", 120);
            int idCliente = 0;
            if (cliente.Length >= 2)
            {
                idCliente = ResolverIdCliente(cliente);
                if (idCliente <= 0)
                {
                    return JsonErro("Nenhum cliente encontrado com o nome \"" + cliente + "\".");
                }
            }

            // A tela de pesquisa sempre envia um periodo; datas vazias podem zerar o resultado na procedure.
            // Por isso, quando o usuario nao informa, aplicamos um periodo amplo (equivale a "sem filtro de data").
            string dataInicio = DataValidaOuVazia(Texto(argumentos, "dataInicio", 10));
            string dataFim = DataValidaOuVazia(Texto(argumentos, "dataFim", 10));
            if (dataInicio.Length == 0)
            {
                dataInicio = "01/01/2000";
            }
            if (dataFim.Length == 0)
            {
                dataFim = DateTime.Now.ToString("dd/MM/yyyy", new System.Globalization.CultureInfo("pt-BR"));
            }

            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            // Mesma procedure e parametros da tela de pesquisa de pedidos (sp_Consulta_tbl_Flow_Pedidos / PESQUISA).
            // Datas em dd/MM/yyyy ou vazio; @sPesquisa e o texto livre do campo de busca da tela.
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "PESQUISA" },
                { "@idTipo", tipo.ToString() },
                { "@dtInicio", dataInicio },
                { "@dtFinal", dataFim },
                { "@idDepartamento", "0" },
                { "@idStatus", "0" },
                { "@sidFluxos", string.Empty },
                { "@idEmpresa", "0" },
                { "@idCliente", idCliente.ToString() },
                { "@sPesquisa", termo },
                { "@idUsuario", IDENTITY.Variaveis.idUsuario() }
            };

            if (tipo == 7)
            {
                parametros.Add("@sTipoCompra", "0");
            }
            else if (tipo == 2)
            {
                parametros.Add("@sPedidoCliente", string.Empty);
            }
            
            DataSet ds = BD.ExecutarDataSet("sp_Consulta_tbl_Flow_Pedidos", parametros);

            // Pedido de compras (tipo 7) retorna a lista na tabela 3; os demais na tabela 0
            int indiceTabela = tipo == 7 ? 3 : 0;
            if (ds == null || ds.Tables.Count <= indiceTabela)
            {
                return JsonErro("Nao foi possivel pesquisar pedidos.");
            }

            DataTable tabelaPedidos = ds.Tables[indiceTabela];
            if (idCliente > 0 && cliente.Length >= 2)
            {
                tabelaPedidos = FiltrarPorCliente(tabelaPedidos, cliente);
            }

            return TabelaParaJson(ferramenta, tabelaPedidos, limite, new[]
            {
                "idPedido",
                "idTipo",
                "nNumeroPedido",
                "sPedidoCliente",
                "nControleTT",
                "sReferencia",
                "sDscCliente",
                "sRazaoSocial",
                "sDscFluxo",
                "sDscStatus",
                "sStatus",
                "sDscDepartamento",
                "sDscTipoEnvio",
                "sDscVendedor",
                "dtPedido",
                "dtPrevisaoEntregaFormatada",
                "dtPrevisaoEntrega",
                "dtEstimativaEntrega"
            }, "/App/Paginas/Pedidos_Detalhe.aspx?id={idPedido}&sTp={idTipo}");
        }

        // A procedure de pesquisa ignora @idCliente quando @idTipo = 2, entao garantimos o filtro por
        // cliente aqui: mantem apenas as linhas cujo nome de cliente contem o termo pesquisado.
        private static DataTable FiltrarPorCliente(DataTable tabela, string cliente)
        {
            if (tabela == null)
            {
                return null;
            }

            string alvo = (cliente ?? string.Empty).Trim().ToUpperInvariant();
            if (alvo.Length == 0)
            {
                return tabela;
            }

            bool temRazao = tabela.Columns.Contains("sRazaoSocial");
            bool temDsc = tabela.Columns.Contains("sDscCliente");
            if (!temRazao && !temDsc)
            {
                return tabela; // sem coluna de cliente para filtrar; nao arrisca esconder tudo
            }

            DataTable filtrada = tabela.Clone();
            foreach (DataRow row in tabela.Rows)
            {
                string razao = temRazao ? (Valor(row, "sRazaoSocial") ?? string.Empty).ToUpperInvariant() : string.Empty;
                string dsc = temDsc ? (Valor(row, "sDscCliente") ?? string.Empty).ToUpperInvariant() : string.Empty;
                if (razao.Contains(alvo) || dsc.Contains(alvo))
                {
                    filtrada.ImportRow(row);
                }
            }

            return filtrada;
        }

        // Aceita dd/MM/yyyy (formato da tela); qualquer outro valor vira vazio (sem filtro de data)
        private static string DataValidaOuVazia(string valor)
        {
            valor = (valor ?? string.Empty).Trim();
            DateTime data;
            if (DateTime.TryParseExact(valor, "dd/MM/yyyy", new System.Globalization.CultureInfo("pt-BR"), System.Globalization.DateTimeStyles.None, out data))
            {
                return valor;
            }

            return string.Empty;
        }

        private static string ExecutarPedidosConsultarHistorico(IAFerramentaDefinicao ferramenta, JObject argumentos, IAFerramentaResultado resultado)
        {
            int idPedido = Inteiro(argumentos, "idPedido", 0);
            if (idPedido <= 0)
            {
                return JsonErro("idPedido invalido.");
            }

            string erroResolucao = ResolverPedidoOuErro(idPedido, out idPedido);
            if (erroResolucao != null)
            {
                return erroResolucao;
            }

            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA PEDIDO" },
                { "@idPedido", idPedido.ToString() },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
            };

            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", parametros);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return JsonErro("Pedido nao encontrado ou sem acesso para consulta.");
            }

            int idTipo = Inteiro(Valor(ds.Tables[0].Rows[0], "idTipo"), 0);
            if (idTipo == 7 && !FUNCOES.ValidaPermissao(Permissao.Compras.Pedido_Compras.Consultar, false))
            {
                resultado.Status = "NEGADO";
                resultado.Erro = "Usuario sem permissao para consultar pedido de compras.";
                return JsonErro(resultado.Erro);
            }

            // O historico volta como a tabela de indice 2 do CONSULTA PEDIDO, a mesma da aba Historico do Pedidos_Detalhe
            if (ds.Tables.Count <= 2)
            {
                return JsonErro("Historico indisponivel para este pedido.");
            }

            return TabelaParaJson(ferramenta, ds.Tables[2], limite, new[]
            {
                "dtLog",
                "sDscUsuario",
                "sTipoAcao",
                "sAcao"
            }, "/App/Paginas/Pedidos_Detalhe.aspx?id=" + idPedido);
        }

        private static string ExecutarPedidosListarArquivos(IAFerramentaDefinicao ferramenta, JObject argumentos, IAFerramentaResultado resultado)
        {
            int idPedido = Inteiro(argumentos, "idPedido", 0);
            if (idPedido <= 0)
            {
                return JsonErro("idPedido invalido.");
            }

            string erroResolucao = ResolverPedidoOuErro(idPedido, out idPedido);
            if (erroResolucao != null)
            {
                return erroResolucao;
            }

            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            // Valida existencia/acesso ao pedido e obtem o idTipo para escolher o sTipoObjeto correto do anexo
            Dictionary<string, string> parametrosPedido = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA PEDIDO" },
                { "@idPedido", idPedido.ToString() },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
            };

            DataSet dsPedido = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", parametrosPedido);
            if (dsPedido == null || dsPedido.Tables.Count == 0 || dsPedido.Tables[0].Rows.Count == 0)
            {
                return JsonErro("Pedido nao encontrado ou sem acesso para consulta.");
            }

            int idTipo = Inteiro(Valor(dsPedido.Tables[0].Rows[0], "idTipo"), 0);
            if (idTipo == 7 && !FUNCOES.ValidaPermissao(Permissao.Compras.Pedido_Compras.Consultar, false))
            {
                resultado.Status = "NEGADO";
                resultado.Erro = "Usuario sem permissao para consultar pedido de compras.";
                return JsonErro(resultado.Erro);
            }

            // Mesmo mapeamento de cls_Arquivos.ObterTipoObjetoArquivoPedido usado pelo iframe de anexos
            string tipoObjeto = idTipo == 3 || idTipo == 6 ? "Pedido-Comex" : (idTipo == 7 ? "PedidoCompras" : "Pedido");

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "Consultar" },
                { "@sTipoObjeto", tipoObjeto },
                { "@idObjeto", idPedido.ToString() },
                { "@idUsuario", IDENTITY.Variaveis.idUsuario() }
            };

            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", parametros);
            if (ds == null || ds.Tables.Count == 0)
            {
                return JsonErro("Nao foi possivel listar os anexos do pedido.");
            }

            // Apenas metadados dos anexos ativos; o conteudo binario (vbArquivo) nunca entra no allowlist
            return TabelaParaJson(ferramenta, ds.Tables[0], limite, new[]
            {
                "idArquivo",
                "sDscArquivo",
                "sDscTipoArquivo",
                "sDscCategoria",
                "dtAtualizacao",
                "sDscUsuarioAtualizacao"
            }, "/App/Paginas/Pedidos_Detalhe.aspx?id=" + idPedido);
        }

        private static string PrepararPedidosAdicionarObservacao(IAFerramentaDefinicao ferramenta, JObject argumentos, IAFerramentaResultado resultado)
        {
            int idPedido = Inteiro(argumentos, "idPedido", 0);
            string observacao = Texto(argumentos, "sObservacao", 500);

            if (idPedido <= 0)
            {
                return JsonErro("idPedido invalido.");
            }

            if (observacao.Length < 5)
            {
                return JsonErro("Informe uma observacao com ao menos 5 caracteres.");
            }

            string erroResolucao = ResolverPedidoOuErro(idPedido, out idPedido);
            if (erroResolucao != null)
            {
                return erroResolucao;
            }

            // Normaliza o id resolvido nos argumentos persistidos: a execucao confirmada
            // (ExecutarAcaoAprovada) le o idPedido da aprovacao e precisa do id interno real
            argumentos["idPedido"] = idPedido;
            resultado.ArgumentosJson = argumentos.ToString(Formatting.None);

            // Valida existencia e acesso ao pedido ANTES de propor a acao
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA PEDIDO" },
                { "@idPedido", idPedido.ToString() },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
            };

            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", parametros);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return JsonErro("Pedido nao encontrado ou sem acesso para consulta.");
            }

            int idTipo = Inteiro(Valor(ds.Tables[0].Rows[0], "idTipo"), 0);
            if (idTipo == 7 && !FUNCOES.ValidaPermissao(Permissao.Compras.Pedido_Compras.Consultar, false))
            {
                resultado.Status = "NEGADO";
                resultado.Erro = "Usuario sem permissao para pedido de compras.";
                return JsonErro(resultado.Erro);
            }

            resultado.Status = "PENDENTE_CONFIRMACAO";
            resultado.ResumoAcao = "Adicionar observacao ao pedido " + idPedido + ": \"" + observacao + "\"";

            return new JObject
            {
                { "sucesso", true },
                { "acaoPendente", true },
                { "resumo", resultado.ResumoAcao },
                { "instrucao", "A acao ficou pendente de confirmacao. Informe ao usuario que ele precisa confirmar pelos botoes exibidos no chat; nao afirme que a observacao ja foi gravada." }
            }.ToString(Formatting.None);
        }

        private static string PrepararMensagensCriarRascunho(IAFerramentaDefinicao ferramenta, JObject argumentos, IAFerramentaResultado resultado)
        {
            string assunto = Texto(argumentos, "assunto", 200);
            string corpo = Texto(argumentos, "corpo", 4000);
            string destino = Texto(argumentos, "destino", 200);

            if (assunto.Length < 3)
            {
                return JsonErro("Informe um assunto com ao menos 3 caracteres.");
            }

            if (corpo.Length < 10)
            {
                return JsonErro("Informe o corpo do rascunho com ao menos 10 caracteres.");
            }

            // Normaliza os argumentos persistidos: a execucao confirmada (ExecutarAcaoAprovada)
            // le assunto/corpo/destino da aprovacao e grava o rascunho
            argumentos["assunto"] = assunto;
            argumentos["corpo"] = corpo;
            argumentos["destino"] = destino;
            resultado.ArgumentosJson = argumentos.ToString(Formatting.None);

            resultado.Status = "PENDENTE_CONFIRMACAO";
            resultado.ResumoAcao = "Salvar rascunho de mensagem" +
                (destino.Length > 0 ? " para " + destino : string.Empty) + ": \"" + assunto + "\"";

            return new JObject
            {
                { "sucesso", true },
                { "acaoPendente", true },
                { "resumo", resultado.ResumoAcao },
                { "instrucao", "O rascunho ficou pendente de confirmacao. Mostre ao usuario o texto completo do rascunho e avise que ele precisa confirmar pelos botoes do chat; o rascunho ainda NAO foi salvo e a mensagem NAO sera enviada." }
            }.ToString(Formatting.None);
        }

        private static string ExecutarProdutosConsultar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 120);
            string familia = Texto(argumentos, "familia", 120);
            string tipo = Texto(argumentos, "tipo", 120);
            string grupo = Texto(argumentos, "grupo", 120);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            // Resolve cada filtro pelo nome -> id usando o mesmo lookup dos combos da tela de produtos.
            // Os nomes nunca entram na SQL (o sp_Select lista tudo; o match e feito em C#).
            int idFamilia = 0;
            if (familia.Length >= 2)
            {
                idFamilia = ResolverIdPorLookup("sp_Select 'Flow_WMS_Produtos_Familia'", "idFamilia", "sDscFamilia", familia);
                if (idFamilia <= 0)
                {
                    return JsonErro("Nenhuma familia de produto encontrada com o nome \"" + familia + "\".");
                }
            }

            int idTipoProduto = 0;
            if (tipo.Length >= 2)
            {
                idTipoProduto = ResolverIdPorLookup("sp_Select 'Flow_Produtos_Tipo'", "idTipoProduto", "sDscTipoProduto", tipo);
                if (idTipoProduto <= 0)
                {
                    return JsonErro("Nenhum tipo de produto encontrado com o nome \"" + tipo + "\".");
                }
            }

            int idGrupo = 0;
            if (grupo.Length >= 2)
            {
                idGrupo = ResolverIdPorLookup("sp_Select 'Flow_WMS_Produtos_Grupos_PAI'", "idGrupo", "sDscGrupo", grupo);
                if (idGrupo <= 0)
                {
                    return JsonErro("Nenhum grupo de produto encontrado com o nome \"" + grupo + "\".");
                }
            }

            // Precisa de ao menos um criterio: termo (>=2 chars) ou um filtro resolvido
            if (termo.Length < 2 && idFamilia <= 0 && idTipoProduto <= 0 && idGrupo <= 0)
            {
                return JsonErro("Informe ao menos 2 caracteres no termo, ou uma familia, tipo ou grupo para buscar produtos.");
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscProduto", termo },
                { "@sCodigo", termo },
                { "@idTipoProduto", idTipoProduto.ToString() },
                { "@idFamilia", idFamilia.ToString() },
                { "@idGrupo", idGrupo.ToString() },
                { "@idPais", "0" },
                { "@sTipoPesquisa", termo },
                { "@sCodigoNCM", string.Empty },
                { "@sExibeComercial", "T" },
                { "@sExibeLM", "T" },
                { "@sExibeComposicao", "T" },
                { "@sSituacao", "S" },
                { "@sSituacaoCadastral", string.Empty },
                { "@sSubTipo", "0" },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Produtos", parametros, false);
            // Allowlist ampliado (familia, grupo, tipo, subtipo, NCM, unidade): colunas ausentes no
            // retorno da SP sao ignoradas por TabelaParaJson, entao so aparece o que existir de fato.
            return TabelaParaJson(ferramenta, tb, limite, new[]
            {
                "idItem",
                "sCodigo",
                "sDscProduto",
                "sDscFamilia",
                "sDscGrupo",
                "sDscTipoProduto",
                "sDscSubTipo",
                "sDscCategoria",
                "sCodigoNCM",
                "sNCM",
                "sDscUnidade",
                "sDscUnidadeMedida",
                "sSituacao_Completa",
                "sSituacaoCadastral_Completa",
                "dtAtualizacao",
                "sDscUsuarioAtualizacao"
            }, "/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={idItem}");
        }

        // Resolve nome -> id usando um lookup sp_Select (familia, tipo ou grupo de produto).
        // O nome informado NUNCA entra na SQL (o sp_Select lista tudo); o match e feito em C#:
        // prefere igualdade exata na descricao; senao, o primeiro que contem o texto.
        private static int ResolverIdPorLookup(string comandoSelect, string colunaId, string colunaDesc, string nome)
        {
            nome = (nome ?? string.Empty).Trim();
            if (nome.Length == 0)
            {
                return 0;
            }

            DataTable tb;
            try
            {
                tb = BD.ExecutarDataTable(comandoSelect);
            }
            catch
            {
                return 0;
            }

            if (tb == null || !tb.Columns.Contains(colunaId) || !tb.Columns.Contains(colunaDesc))
            {
                return 0;
            }

            int idContem = 0;
            foreach (DataRow row in tb.Rows)
            {
                string desc = Valor(row, colunaDesc);
                if (string.Equals(desc, nome, StringComparison.OrdinalIgnoreCase))
                {
                    return Inteiro(Valor(row, colunaId), 0);
                }

                if (idContem == 0 && desc.IndexOf(nome, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    idContem = Inteiro(Valor(row, colunaId), 0);
                }
            }

            return idContem;
        }

        // Resolve o nome do cliente para idCliente reaproveitando a busca de clientes (CONSULTAR por razao social).
        // Prefere match exato em razao social ou fantasia; senao usa o primeiro resultado (a SP ja filtrou por LIKE).
        private static int ResolverIdCliente(string nomeCliente)
        {
            nomeCliente = (nomeCliente ?? string.Empty).Trim();
            if (nomeCliente.Length < 2)
            {
                return 0;
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sRazaoSocial", nomeCliente },
                { "@idTipo", "0" }
            };

            DataTable tb;
            try
            {
                tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Clientes", parametros, false);
            }
            catch
            {
                return 0;
            }

            if (tb == null || tb.Rows.Count == 0 || !tb.Columns.Contains("idCliente"))
            {
                return 0;
            }

            foreach (DataRow row in tb.Rows)
            {
                string razao = Valor(row, "sRazaoSocial");
                string fantasia = Valor(row, "sNomeFantasia");
                if (string.Equals(razao, nomeCliente, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(fantasia, nomeCliente, StringComparison.OrdinalIgnoreCase))
                {
                    return Inteiro(Valor(row, "idCliente"), 0);
                }
            }

            return Inteiro(Valor(tb.Rows[0], "idCliente"), 0);
        }

        private static string ExecutarParceirosConsultar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 120);
            int idTipo = Inteiro(argumentos, "idTipo", 0);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (termo.Length < 2)
            {
                return JsonErro("Informe ao menos 2 caracteres para buscar parceiros.");
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sRazaoSocial", termo },
                { "@idTipo", idTipo.ToString() }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Clientes", parametros, false);
            return TabelaParaJson(ferramenta, tb, limite, new[]
            {
                "idCliente",
                "sRazaoSocial",
                "sNomeFantasia",
                "sCPF_CNPJ",
                "sDscTipoSituacaoCliente",
                "sTipoPesquisa",
                "sDscTipoParceiro"
            }, "/App/Paginas/Manutencao/Parceiros_Detalhe.aspx?id={idCliente}");
        }

        private static string ExecutarMensagensConsultar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 120);
            string categoria = Texto(argumentos, "categoria", 20).ToUpperInvariant();
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (categoria != "AVISO" && categoria != "MENSAGEM" && categoria != "EMAIL")
            {
                categoria = "TODOS";
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_COM_FILTRO" },
                { "@idUsuarioPesquisa", IDENTITY.Variaveis.idUsuario() },
                { "@idDepartamentoDestino", "0" },
                { "@sDepartamentosDestino", string.Empty },
                { "@dtInicial", string.Empty },
                { "@dtFinal", string.Empty },
                { "@sAssunto", termo },
                { "@idUsuarioDestino", IDENTITY.Variaveis.idUsuario() },
                { "@idFiltroMensagem", "1" },
                { "@sDirecaoMensagem", "Recebida" },
                { "@sCategoriaMensagem", categoria },
                { "@idTipoObjeto", "-99" }
            };

            if (categoria == "AVISO")
            {
                parametros.Add("@sIsAviso", "S");
            }
            else if (categoria == "MENSAGEM")
            {
                parametros.Add("@sIsAviso", "N");
            }

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Mensagens", parametros, false);
            return TabelaParaJson(ferramenta, tb, limite, new[]
            {
                "idMensagem",
                "sDscUsuarioRemetente",
                "sAssunto",
                "sDestinatario",
                "sIsAviso",
                "dtLeitura",
                "dtInclusao",
                "sDscTipoObjeto"
            }, "/App/Paginas/Mensagem/Mensagens_Detalhe.aspx?id={idMensagem}");
        }

        private static string ExecutarRRHHConsultarColaboradorBasico(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 120);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (termo.Length < 2)
            {
                return JsonErro("Informe ao menos 2 caracteres (nome do colaborador) para pesquisar.");
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sPesquisa", termo },
                { "@sSituacao", "T" },
                { "@sCBO", "0" }, // default '' dispara o filtro (IF @sCBO <> '0'); '0' = nao filtrar
                // A SP filtra por linha usando o usuario logado (departamentos/vendedores/diretores permitidos)
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores", parametros, false);
            return TabelaParaJson(ferramenta, tb, limite, new[]
            {
                "idColaborador",
                "sDscColaborador",
                "sDscCargo",
                "sDscDepartamento",
                "sDscEmpresa",
                "sDscFuncao",
                "sDscGHE_Completo",
                "sDscSupervisorDireto",
                "sSituacao_Completa"
            }, "/App/Paginas/RRHH/Colaboradores_Detalhe.aspx?id={idColaborador}");
        }

        private static string ExecutarFinanceiroContasReceber(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 120);
            string status = NormalizarStatusFinanceiro(Texto(argumentos, "status", 30));
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (status == null)
            {
                return JsonErro("Status invalido. Use 'Em Atraso', 'Em Aberto', 'Liquidado' ou vazio para todos.");
            }

            if (termo.Length < 2 && status == string.Empty)
            {
                return JsonErro("Informe um cliente/codigo/documento (2+ caracteres) ou um status (ex.: 'Em Atraso') para pesquisar.");
            }

            // Replica a visibilidade por linha da tela: sem 'Visualizar Tudo', so ve os proprios titulos
            string permissaoTudo = FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasReceber.VisualizarTudo, false) ? "S" : "N";

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDashBoard", string.Empty },
                { "@sUsuarioLogado", IDENTITY.Variaveis.idUsuario() },
                { "@sPermissao", permissaoTudo },
                { "@sPesquisa", termo },
                { "@sStatus", status },       // '' = todos; 'Em Atraso'/'Em Aberto'/'Liquidado'
                { "@sConciliado", "T" },      // 'T' = nao filtrar por conciliacao
                { "@sTituloAdiantado", "T" }  // default 'N' esconderia adiantados; 'T' = todos
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Adm_Contas_Receber", parametros, false);
            return TabelaParaJson(ferramenta, tb, limite, new[]
            {
                "idContasReceber",
                "sCodigo",
                "sDocumento",
                "sRazaoSocial",
                "dtEmissao_ordem",
                "dtVencimento_ordem",
                "nValorOriginal",
                "nSaldo",
                "nTotal",
                "sStatus",
                "sDiasParaVencer",
                "sDscCategoriaReceber",
                "sDscFormaRecebimento",
                "sDscEmpresa",
                "dtLiquidado"
            }, "/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={idContasReceber}");
        }

        private static string ExecutarFinanceiroContasPagar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 120);
            string status = NormalizarStatusFinanceiro(Texto(argumentos, "status", 30));
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (status == null)
            {
                return JsonErro("Status invalido. Use 'Em Atraso', 'Em Aberto', 'Liquidado' ou vazio para todos.");
            }

            if (termo.Length < 2 && status == string.Empty)
            {
                return JsonErro("Informe um fornecedor/codigo/documento (2+ caracteres) ou um status (ex.: 'Em Atraso') para pesquisar.");
            }

            string permissaoTudo = FUNCOES.ValidaPermissao(Permissao.Financeiro.ContasPagar.VisualizarTudo, false) ? "S" : "N";

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDashBoard", string.Empty },
                { "@sUsuarioLogado", IDENTITY.Variaveis.idUsuario() },
                { "@sPermissao", permissaoTudo },
                { "@sPesquisa", termo },
                { "@sStatus", status },
                { "@sConciliado", "T" }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", parametros, false);
            return TabelaParaJson(ferramenta, tb, limite, new[]
            {
                "idContasPagar",
                "sCodigo",
                "sDocumento",
                "sRazaoSocial",
                "sDscCredor",
                "sDscColaborador",
                "dtEmissao_ordem",
                "dtVencimento_ordem",
                "nValorOriginal",
                "nSaldo",
                "nTotal",
                "sStatus",
                "sDiasParaVencer",
                "sDscCategoriaPagar",
                "sDscEmpresa",
                "sDscAprovacao",
                "dtLiquidado"
            }, "/App/Paginas/Adm/Financeiro/ContasPagar_Detalhe.aspx?id={idContasPagar}");
        }

        private static string ExecutarOrcamentoTiposListar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 100);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscPesquisa", string.Empty }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Comercial_Orcamento_Tipo", parametros, false);
            string termoResolucao = cls_IA_ResolucaoTexto.AplicarAliasCategoria(termo);
            if (!string.IsNullOrWhiteSpace(termoResolucao))
            {
                tb = FiltrarTabelaPorTermo(tb, termoResolucao, "sDscTipoOrcamento");
                return TabelaResolucaoParaJson(ferramenta, tb, limite, termoResolucao, new[]
                {
                    "idTipoOrcamento", "sDscTipoOrcamento", "sAtivo", "sAtivoCompleto", "dtAtualizacao", "sDscUsuarioAtualizacao"
                }, new[] { "idTipoOrcamento", "sDscTipoOrcamento" }, "/App/Paginas/Comercial/Manutencao/TipoOrcamento_Detalhe.aspx?id={idTipoOrcamento}");
            }

            return TabelaParaJson(ferramenta, tb, limite, new[]
            {
                "idTipoOrcamento",
                "sDscTipoOrcamento",
                "sAtivo",
                "sAtivoCompleto",
                "dtAtualizacao",
                "sDscUsuarioAtualizacao"
            }, "/App/Paginas/Comercial/Manutencao/TipoOrcamento_Detalhe.aspx?id={idTipoOrcamento}");
        }

        private static string ExecutarOrcamentoTipoDetalhar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            int idTipoOrcamento = Inteiro(argumentos, "idTipoOrcamento", 0);
            if (idTipoOrcamento <= 0)
            {
                return JsonErro("idTipoOrcamento inválido.");
            }

            DataSet ds = ConsultarTipoOrcamentoDetalhe(idTipoOrcamento);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return JsonErro("Tipo de orçamento não encontrado.");
            }

            int limite = ferramenta.MaxRegistros > 0 ? ferramenta.MaxRegistros : 30;
            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "idTipoOrcamento", idTipoOrcamento },
                { "detalhe", TabelaParaArray(Tabela(ds, 0), 1, new[]
                    {
                        "idTipoOrcamento",
                        "sDscTipoOrcamento",
                        "sAtivo",
                        "dtAtualizacao",
                        "sDscUsuarioAtualizacao"
                    }, "/App/Paginas/Comercial/Manutencao/TipoOrcamento_Detalhe.aspx?id={idTipoOrcamento}") },
                { "fluxos", TabelaParaArray(Tabela(ds, 1), limite, new[] { "idFluxo", "sDscFluxo" }, null) },
                { "tiposServico", TabelaParaArray(Tabela(ds, 2), limite, new[] { "idTipoProduto", "sDscTipoProduto" }, null) },
                { "escopos", TabelaParaArray(Tabela(ds, 3), limite, new[]
                    {
                        "idEscopo",
                        "sDscEscopo",
                        "sAtivo",
                        "dtAtualizacao",
                        "sDscUsuarioAtualizacao"
                    }, null) }
            }.ToString(Formatting.None);
        }

        private static string ExecutarOrcamentoEscoposListar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            int idTipoOrcamento = Inteiro(argumentos, "idTipoOrcamento", 0);
            int idOrcamento = Inteiro(argumentos, "idOrcamento", 0);
            string sidEscopos = Texto(argumentos, "sidEscopos", 400);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (idTipoOrcamento <= 0 && idOrcamento <= 0)
            {
                return JsonErro("Informe idTipoOrcamento para novo orçamento ou idOrcamento para consultar respostas salvas.");
            }

            JArray respostasExistentes = new JArray();
            if (idOrcamento > 0)
            {
                DataSet dsOrcamento = ConsultarEscoposOrcamento(0, idOrcamento, string.Empty);
                respostasExistentes = TabelaParaArray(Tabela(dsOrcamento, 1), limite, new[]
                {
                    "idTipoOrcamento",
                    "idRegistro",
                    "idEscopo",
                    "idCategoriaEscopo",
                    "sPergunta",
                    "sOpcao",
                    "idOpcao",
                    "idPergunta"
                }, null);

                if (idTipoOrcamento <= 0 && dsOrcamento != null && dsOrcamento.Tables.Count > 1 && dsOrcamento.Tables[1].Rows.Count > 0)
                {
                    idTipoOrcamento = Inteiro(Valor(dsOrcamento.Tables[1].Rows[0], "idTipoOrcamento"), 0);
                }
            }

            if (idTipoOrcamento <= 0)
            {
                return JsonErro("Não foi possível identificar o tipo do orçamento.");
            }

            if (string.IsNullOrWhiteSpace(sidEscopos))
            {
                sidEscopos = ResolverSidEscoposTipoOrcamento(idTipoOrcamento);
            }

            DataSet ds = ConsultarEscoposOrcamento(idTipoOrcamento, idOrcamento, sidEscopos);
            JArray respostas = respostasExistentes.Count > 0
                ? respostasExistentes
                : TabelaParaArray(Tabela(ds, 1), limite, new[]
                    {
                        "idTipoOrcamento",
                        "idRegistro",
                        "idEscopo",
                        "idCategoriaEscopo",
                        "sPergunta",
                        "sOpcao",
                        "idOpcao",
                        "idPergunta"
                    }, null);

            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "idTipoOrcamento", idTipoOrcamento },
                { "idOrcamento", idOrcamento },
                { "sidEscopos", sidEscopos },
                { "perguntas", TabelaParaArray(Tabela(ds, 0), limite, new[]
                    {
                        "idCategoria",
                        "idEscopo",
                        "sDscEscopo",
                        "sDscCategoria",
                        "sPerguntas",
                        "sOpcoes",
                        "sAtivo",
                        "dtAtualizacao",
                        "sDscUsuarioAtualizacao"
                    }, null) },
                { "respostas", respostas }
            }.ToString(Formatting.None);
        }

        private static string ExecutarOrcamentoServicosRecursosListar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            int idTipoOrcamento = Inteiro(argumentos, "idTipoOrcamento", 0);
            string sidTipoServicos = Texto(argumentos, "sidTipoServicos", 400);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (idTipoOrcamento <= 0)
            {
                return JsonErro("idTipoOrcamento inválido.");
            }

            if (string.IsNullOrWhiteSpace(sidTipoServicos))
            {
                sidTipoServicos = ResolverSidTiposServicoTipoOrcamento(idTipoOrcamento);
            }

            if (string.IsNullOrWhiteSpace(sidTipoServicos))
            {
                return JsonErro("O tipo de orçamento não possui tipos de serviço/recurso vinculados.");
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA_SERVICOS_RECURSOS_x_ORCAMENTO" },
                { "@idTipoOrcamento", idTipoOrcamento.ToString() },
                { "@sidTipoServicos", sidTipoServicos }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Comercial_Orcamento_Tipo", parametros, false);
            return TabelaParaJson(ferramenta, tb, limite, new[]
            {
                "idItem",
                "nPreco",
                "sCodigo",
                "sDscProduto",
                "idGrupo",
                "sDscGrupo",
                "idFamilia",
                "sDscFamilia",
                "sTipo",
                "sUnidade",
                "idTipo",
                "idTipoRegra",
                "sProjeto"
            }, "/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={idItem}");
        }

        private static string ExecutarOrcamentoClientesBuscar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 120);
            string somenteAtivos = Texto(argumentos, "somenteAtivos", 1).ToUpperInvariant() == "N" ? "N" : "S";
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (termo.Length < 2)
            {
                return JsonErro("Informe ao menos 2 caracteres para buscar clientes.");
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sRazaoSocial", termo },
                { "@sSituacao", somenteAtivos == "S" ? "S" : "T" },
                { "@idTipo", "0" },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().Trim() }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Clientes", parametros, false);
            return TabelaResolucaoParaJson(ferramenta, tb, limite, termo, new[]
            {
                "idCliente",
                "idParceiro",
                "sNomeFantasia",
                "sRazaoSocial",
                "sCPF_CNPJ",
                "sEmail",
                "sTelefone",
                "sTipoPesquisa",
                "sSituacao",
                "sSituacao_Completa",
                "idTipoSituacaoCliente",
                "sDscTipoSituacaoCliente",
                "sTipoCliente",
                "sVendaIndividual",
                "idPais"
            }, new[] { "idCliente", "sRazaoSocial", "sNomeFantasia", "sCPF_CNPJ", "sTipoPesquisa" }, "/App/Paginas/Manutencao/Parceiros_Detalhe.aspx?id={idCliente}");
        }

        private static string ExecutarClientesResolverParaOrcamento(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 120);
            string somenteAtivos = Texto(argumentos, "somenteAtivos", 1).ToUpperInvariant() == "N" ? "N" : "S";
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (termo.Length < 2)
            {
                return JsonErro("Informe ao menos 2 caracteres para resolver o cliente.");
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sRazaoSocial", termo },
                { "@sSituacao", somenteAtivos == "S" ? "S" : "T" },
                { "@idTipo", "0" },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().Trim() }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Clientes", parametros, false);
            if ((tb == null || tb.Rows.Count == 0) && SomenteDigitos(termo).Length < 5)
            {
                foreach (string palavra in ExtrairPalavras(termo))
                {
                    if (palavra.Length < 3) continue;
                    parametros["@sRazaoSocial"] = palavra;
                    DataTable alternativa = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Clientes", parametros, false);
                    if (alternativa != null && alternativa.Rows.Count > 0)
                    {
                        tb = alternativa;
                        break;
                    }
                }
            }
            string jsonResolucao = TabelaResolucaoParaJson(ferramenta, tb, limite, termo, new[]
            {
                "idCliente",
                "idParceiro",
                "sNomeFantasia",
                "sRazaoSocial",
                "sCPF_CNPJ",
                "sEmail",
                "sTelefone",
                "sTipoPesquisa",
                "sSituacao",
                "sSituacao_Completa",
                "idTipoSituacaoCliente",
                "sDscTipoSituacaoCliente",
                "sTipoCliente",
                "sVendaIndividual",
                "idPais"
            }, new[] { "idCliente", "sRazaoSocial", "sNomeFantasia", "sCPF_CNPJ", "sTipoPesquisa" }, "/App/Paginas/Manutencao/Parceiros_Detalhe.aspx?id={idCliente}");

            JObject retorno = JObject.Parse(jsonResolucao);
            int totalEncontrado = Inteiro(retorno["totalEncontrado"] != null ? retorno["totalEncontrado"].ToString() : string.Empty, 0);
            bool encontrado = totalEncontrado > 0 && retorno.Value<bool?>("precisaEscolha") != true;
            retorno["encontrado"] = encontrado;
            retorno["precisaCadastro"] = totalEncontrado == 0;

            JObject melhor = retorno["melhorResultado"] as JObject;
            if (melhor != null && encontrado)
            {
                int idCliente = Inteiro(melhor["idCliente"] != null ? melhor["idCliente"].ToString() : string.Empty, 0);
                if (idCliente > 0)
                {
                    retorno["idCliente"] = idCliente;
                }
            }

            if (totalEncontrado == 0)
            {
                retorno["unicoProvavel"] = false;
                retorno["precisaEscolha"] = false;
                retorno["mensagem"] = "Cliente não encontrado. Use um nó de aprovação para perguntar se o usuário deseja cadastrar este cliente antes de continuar.";
            }
            else if (!encontrado)
            {
                retorno["mensagem"] = "Foram encontrados vários clientes possíveis. Aguarde a escolha explícita do usuário antes de continuar.";
            }

            return retorno.ToString(Formatting.None);
        }

        private static string PrepararClientesCriarParaOrcamento(IAFerramentaDefinicao ferramenta, JObject argumentos, IAFerramentaResultado resultado)
        {
            JObject normalizados;
            string resumo;
            string erro;
            if (!ValidarClientesCriarParaOrcamento(argumentos, out normalizados, out resumo, out erro))
            {
                resultado.Erro = erro;
                return JsonErro(erro);
            }

            resultado.ArgumentosJson = normalizados.ToString(Formatting.None);
            resultado.Status = "PENDENTE_CONFIRMACAO";
            resultado.ResumoAcao = resumo;

            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "acaoPendente", true },
                { "resumo", resumo },
                { "dadosNormalizados", normalizados },
                { "instrucao", "A ação ficou pendente de confirmação; não afirme que o cliente já foi cadastrado." }
            }.ToString(Formatting.None);
        }

        private static bool ExecutarClientesCriarParaOrcamentoConfirmado(JObject argumentos, out string erro, out JToken saida)
        {
            erro = string.Empty;
            saida = new JObject { { "executado", true } };

            if (!FUNCOES.ValidaPermissao(Permissao.Parceiros.Incluir, false))
            {
                erro = "Usuário sem permissão para incluir parceiros/clientes.";
                return false;
            }

            JObject normalizados;
            string resumo;
            if (!ValidarClientesCriarParaOrcamento(argumentos, out normalizados, out resumo, out erro))
            {
                return false;
            }

            try
            {
                DataSet dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Clientes", MontarParametrosClientesCriar(normalizados));
                string erroBanco = ErroRetornoDataSet(dsSalvar);
                if (!string.IsNullOrWhiteSpace(erroBanco))
                {
                    erro = "Não foi possível criar o cliente: " + erroBanco;
                    return false;
                }

                DataTable tbRetorno = Tabela(dsSalvar, 0);
                int idCliente = tbRetorno != null && tbRetorno.Rows.Count > 0 ? Inteiro(Valor(tbRetorno.Rows[0], "idCliente"), 0) : 0;
                int idHistorico = tbRetorno != null && tbRetorno.Rows.Count > 0 ? Inteiro(Valor(tbRetorno.Rows[0], "idHistorico"), 0) : 0;

                if (idCliente <= 0)
                {
                    erro = "A procedure de clientes não retornou o idCliente criado.";
                    return false;
                }

                DataSet dsContato = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Clientes", MontarParametrosClienteContato(normalizados, idCliente, idHistorico));
                erroBanco = ErroRetornoDataSetSeHouver(dsContato);
                if (!string.IsNullOrWhiteSpace(erroBanco))
                {
                    erro = "Cliente criado, mas não foi possível criar o contato principal: " + erroBanco;
                    return false;
                }

                DataSet dsEnderecoFiscal = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Clientes", MontarParametrosClienteEndereco(normalizados, idCliente, idHistorico, 1, string.Empty));
                erroBanco = ErroRetornoDataSetSeHouver(dsEnderecoFiscal);
                if (!string.IsNullOrWhiteSpace(erroBanco))
                {
                    erro = "Cliente criado, mas não foi possível criar o endereço fiscal: " + erroBanco;
                    return false;
                }

                DataSet dsEnderecoEntrega = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Clientes", MontarParametrosClienteEndereco(normalizados, idCliente, idHistorico, 3, "entrega"));
                erroBanco = ErroRetornoDataSetSeHouver(dsEnderecoEntrega);
                if (!string.IsNullOrWhiteSpace(erroBanco))
                {
                    erro = "Cliente criado, mas não foi possível criar o endereço de entrega: " + erroBanco;
                    return false;
                }

                DataSet dsDetalhe = ConsultarClienteOrcamento(idCliente);
                DataTable tbCliente = Tabela(dsDetalhe, 0);
                DataTable tbContatos = Tabela(dsDetalhe, 1);
                DataTable tbEnderecos = Tabela(dsDetalhe, 2);

                DataRow contato = LocalizarContatoCliente(tbContatos, Texto(normalizados, "contatoNome", 50), Texto(normalizados, "contatoEmail", 100));
                DataRow enderecoFiscal = LocalizarEnderecoCliente(tbEnderecos, 1, Texto(normalizados, "cep", 10), Texto(normalizados, "logradouro", 200), Texto(normalizados, "cidade", 200));
                DataRow enderecoEntrega = LocalizarEnderecoCliente(tbEnderecos, 3, Texto(normalizados, "entregaCep", 10), Texto(normalizados, "entregaLogradouro", 200), Texto(normalizados, "entregaCidade", 200));

                int idContatoCliente = contato != null ? Inteiro(Valor(contato, "idContato"), 0) : 0;
                int idEnderecoFiscal = enderecoFiscal != null ? Inteiro(Valor(enderecoFiscal, "idEndereco"), 0) : 0;
                int idEnderecoEntrega = enderecoEntrega != null ? Inteiro(Valor(enderecoEntrega, "idEndereco"), 0) : 0;

                saida = new JObject
                {
                    { "executado", true },
                    { "idCliente", idCliente },
                    { "idParceiro", idCliente },
                    { "idContatoCliente", idContatoCliente },
                    { "idEnderecoFiscal", idEnderecoFiscal },
                    { "idEnderecoEntrega", idEnderecoEntrega },
                    { "idsOrcamento", new JObject
                        {
                            { "idCliente", idCliente },
                            { "idContatoCliente", idContatoCliente },
                            { "idEnderecoFiscal", idEnderecoFiscal },
                            { "idEnderecoEntrega", idEnderecoEntrega }
                        }
                    },
                    { "cliente", TabelaParaArray(tbCliente, 1, new[]
                        {
                            "idCliente",
                            "sNomeFantasia",
                            "sRazaoSocial",
                            "sEmail",
                            "sTelefone",
                            "sTipo",
                            "sCPF_CNPJ",
                            "sRG_IE",
                            "sSituacao",
                            "sTipoCliente",
                            "sidTipoParceiro",
                            "idPais"
                        }, "/App/Paginas/Manutencao/Parceiros_Detalhe.aspx?id={idCliente}") },
                    { "contatoPrincipal", contato != null ? ObjetoLinha(contato, new[] { "idContato", "sNome", "sEmail", "sTelefone", "sTipoContato", "sRecebeEmail" }, null) : new JObject() },
                    { "enderecoFiscal", enderecoFiscal != null ? ObjetoLinha(enderecoFiscal, new[] { "idEndereco", "idTipoEndereco", "sDscTipoEndereco", "sCEP", "sLogradouro", "sNumero", "sComplemento", "sBairro", "sCidade", "sEstado", "sPais", "sEnderecoCompleto" }, null) : new JObject() },
                    { "enderecoEntrega", enderecoEntrega != null ? ObjetoLinha(enderecoEntrega, new[] { "idEndereco", "idTipoEndereco", "sDscTipoEndereco", "sCEP", "sLogradouro", "sNumero", "sComplemento", "sBairro", "sCidade", "sEstado", "sPais", "sEnderecoCompleto" }, null) : new JObject() },
                    { "link", "/App/Paginas/Manutencao/Parceiros_Detalhe.aspx?id=" + idCliente }
                };

                return true;
            }
            catch (Exception ex)
            {
                erro = "Erro ao criar cliente para orçamento: " + ex.Message;
                return false;
            }
        }

        private static string PrepararClientesCompletarParaOrcamento(IAFerramentaDefinicao ferramenta, JObject argumentos, IAFerramentaResultado resultado)
        {
            int idCliente = Inteiro(argumentos, "idCliente", 0);
            if (idCliente <= 0)
            {
                resultado.Erro = "idCliente inválido.";
                return JsonErro(resultado.Erro);
            }

            resultado.ArgumentosJson = (argumentos ?? new JObject()).ToString(Formatting.None);
            resultado.Status = "PENDENTE_CONFIRMACAO";
            resultado.ResumoAcao = "Completar contato ou endereço ausente do cliente " + idCliente + " para continuar o orçamento.";
            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "acaoPendente", true },
                { "resumo", resultado.ResumoAcao },
                { "instrucao", "A alteração do cliente depende da confirmação pelo botão; não afirme que já foi executada." }
            }.ToString(Formatting.None);
        }

        private static bool ExecutarClientesCompletarParaOrcamentoConfirmado(JObject argumentos, out string erro, out JToken saida)
        {
            erro = string.Empty;
            saida = new JObject { { "executado", true } };
            if (!FUNCOES.ValidaPermissao(Permissao.Parceiros.Alterar, false))
            {
                erro = "Usuário sem permissão para alterar parceiros/clientes.";
                return false;
            }

            int idCliente = Inteiro(argumentos, "idCliente", 0);
            if (idCliente <= 0)
            {
                erro = "idCliente inválido.";
                return false;
            }

            try
            {
                JObject dados = argumentos != null ? (JObject)argumentos.DeepClone() : new JObject();
                DataSet detalhe = ConsultarClienteOrcamento(idCliente);
                DataTable contatos = Tabela(detalhe, 1);
                DataTable enderecos = Tabela(detalhe, 2);
                if (Tabela(detalhe, 0) == null || Tabela(detalhe, 0).Rows.Count == 0)
                {
                    erro = "Cliente não encontrado.";
                    return false;
                }

                bool adicionouContato = false;
                bool adicionouFiscal = false;
                bool adicionouEntrega = false;
                string erroBanco;

                if (contatos == null || contatos.Rows.Count == 0)
                {
                    if (Texto(dados, "contatoNome", 50).Length < 3)
                    {
                        erro = "O cliente não possui contato. Informe contatoNome para completar o cadastro.";
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(Texto(dados, "contatoTipo", 50))) dados["contatoTipo"] = "Comercial";
                    DataSet dsContato = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Clientes", MontarParametrosClienteContato(dados, idCliente, 0));
                    erroBanco = ErroRetornoDataSetSeHouver(dsContato);
                    if (!string.IsNullOrWhiteSpace(erroBanco))
                    {
                        erro = "Não foi possível adicionar o contato ao cliente: " + erroBanco;
                        return false;
                    }
                    adicionouContato = true;
                }

                DataRow fiscalExistente = LocalizarEnderecoCliente(enderecos, 1, string.Empty, string.Empty, string.Empty);
                if (fiscalExistente == null)
                {
                    string cep = SomenteDigitos(Texto(dados, "cep", 10));
                    string logradouro = Texto(dados, "logradouro", 200);
                    string bairro = Texto(dados, "bairro", 200);
                    string cidade = Texto(dados, "cidade", 200);
                    string uf = Texto(dados, "uf", 2).ToUpperInvariant();
                    string pais = Texto(dados, "pais", 100);
                    CompletarEnderecoPorCep(cep, ref logradouro, ref bairro, ref cidade, ref uf, ref pais);
                    if (logradouro.Length < 2 || cidade.Length < 2)
                    {
                        erro = "O cliente não possui endereço fiscal. Informe CEP e número ou os dados completos do endereço.";
                        return false;
                    }
                    dados["cep"] = cep; dados["logradouro"] = logradouro; dados["bairro"] = bairro; dados["cidade"] = cidade; dados["uf"] = uf; dados["pais"] = PrimeiroTextoNaoVazio(pais, "Brasil");
                    DataSet dsFiscal = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Clientes", MontarParametrosClienteEndereco(dados, idCliente, 0, 1, string.Empty));
                    erroBanco = ErroRetornoDataSetSeHouver(dsFiscal);
                    if (!string.IsNullOrWhiteSpace(erroBanco))
                    {
                        erro = "Não foi possível adicionar o endereço fiscal ao cliente: " + erroBanco;
                        return false;
                    }
                    adicionouFiscal = true;
                }

                detalhe = ConsultarClienteOrcamento(idCliente);
                enderecos = Tabela(detalhe, 2);
                DataRow entregaExistente = LocalizarEnderecoCliente(enderecos, 3, string.Empty, string.Empty, string.Empty);
                if (entregaExistente == null)
                {
                    if (NormalizarSimNao(Texto(dados, "usarMesmoEnderecoEntrega", 10)) == "S")
                    {
                        DataRow fiscalAtual = LocalizarEnderecoCliente(enderecos, 1, string.Empty, string.Empty, string.Empty);
                        if (fiscalAtual != null)
                        {
                            dados["cep"] = PrimeiroTextoNaoVazio(Texto(dados, "cep", 10), Valor(fiscalAtual, "sCEP"));
                            dados["logradouro"] = PrimeiroTextoNaoVazio(Texto(dados, "logradouro", 200), Valor(fiscalAtual, "sLogradouro"));
                            dados["numero"] = PrimeiroTextoNaoVazio(Texto(dados, "numero", 50), Valor(fiscalAtual, "sNumero"));
                            dados["complemento"] = PrimeiroTextoNaoVazio(Texto(dados, "complemento", 50), Valor(fiscalAtual, "sComplemento"));
                            dados["bairro"] = PrimeiroTextoNaoVazio(Texto(dados, "bairro", 200), Valor(fiscalAtual, "sBairro"));
                            dados["cidade"] = PrimeiroTextoNaoVazio(Texto(dados, "cidade", 200), Valor(fiscalAtual, "sCidade"));
                            dados["uf"] = PrimeiroTextoNaoVazio(Texto(dados, "uf", 2), Valor(fiscalAtual, "sEstado"));
                            dados["pais"] = PrimeiroTextoNaoVazio(Texto(dados, "pais", 100), Valor(fiscalAtual, "sPais"), "Brasil");
                        }
                        dados["entregaCep"] = dados["cep"];
                        dados["entregaLogradouro"] = dados["logradouro"];
                        dados["entregaNumero"] = dados["numero"];
                        dados["entregaComplemento"] = dados["complemento"];
                        dados["entregaBairro"] = dados["bairro"];
                        dados["entregaCidade"] = dados["cidade"];
                        dados["entregaUf"] = dados["uf"];
                        dados["entregaPais"] = dados["pais"];
                    }
                    if (Texto(dados, "entregaLogradouro", 200).Length < 2 || Texto(dados, "entregaCidade", 200).Length < 2)
                    {
                        erro = "O cliente não possui endereço de entrega. Informe os dados de entrega ou use usarMesmoEnderecoEntrega = S.";
                        return false;
                    }
                    DataSet dsEntrega = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Clientes", MontarParametrosClienteEndereco(dados, idCliente, 0, 3, "entrega"));
                    erroBanco = ErroRetornoDataSetSeHouver(dsEntrega);
                    if (!string.IsNullOrWhiteSpace(erroBanco))
                    {
                        erro = "Não foi possível adicionar o endereço de entrega ao cliente: " + erroBanco;
                        return false;
                    }
                    adicionouEntrega = true;
                }

                detalhe = ConsultarClienteOrcamento(idCliente);
                contatos = Tabela(detalhe, 1);
                enderecos = Tabela(detalhe, 2);
                DataRow contato = LocalizarContatoCliente(contatos, Texto(dados, "contatoNome", 50), Texto(dados, "contatoEmail", 100));
                DataRow fiscal = LocalizarEnderecoCliente(enderecos, 1, Texto(dados, "cep", 10), Texto(dados, "logradouro", 200), Texto(dados, "cidade", 200));
                DataRow entrega = LocalizarEnderecoCliente(enderecos, 3, Texto(dados, "entregaCep", 10), Texto(dados, "entregaLogradouro", 200), Texto(dados, "entregaCidade", 200));
                saida = new JObject
                {
                    { "executado", true },
                    { "alterado", adicionouContato || adicionouFiscal || adicionouEntrega },
                    { "idCliente", idCliente },
                    { "idContatoCliente", contato != null ? Inteiro(Valor(contato, "idContato"), 0) : 0 },
                    { "idEnderecoFiscal", fiscal != null ? Inteiro(Valor(fiscal, "idEndereco"), 0) : 0 },
                    { "idEnderecoEntrega", entrega != null ? Inteiro(Valor(entrega, "idEndereco"), 0) : 0 },
                    { "link", "/App/Paginas/Manutencao/Parceiros_Detalhe.aspx?id=" + idCliente }
                };
                return true;
            }
            catch (Exception ex)
            {
                erro = "Erro ao completar cliente para orçamento: " + ex.Message;
                return false;
            }
        }

        private static bool ValidarClientesCriarParaOrcamento(JObject argumentos, out JObject normalizados, out string resumo, out string erro)
        {
            normalizados = new JObject();
            resumo = string.Empty;
            erro = string.Empty;

            if (argumentos == null)
            {
                argumentos = new JObject();
            }

            string razaoSocial = Texto(argumentos, "razaoSocial", 200);
            string nomeFantasia = Texto(argumentos, "nomeFantasia", 200);
            string tipoPessoa = Texto(argumentos, "tipoPessoa", 1).ToUpperInvariant();
            string origemParceiro = Texto(argumentos, "origemParceiro", 1).ToUpperInvariant();
            string documento = Texto(argumentos, "documento", 50);
            string permitirSemDocumento = NormalizarSimNao(Texto(argumentos, "permitirSemDocumento", 10));
            string rgIe = Texto(argumentos, "rgIe", 15);
            string inscricaoMunicipal = Texto(argumentos, "inscricaoMunicipal", 15);
            int idVendedor = Inteiro(argumentos, "idVendedor", 0);
            string sidTabela = NormalizarIdsComSeparador(Texto(argumentos, "sidTabela", 10), '|', false);
            string sidTipoParceiro = ResolverSidTipoParceiroCliente();
            int idPais = Inteiro(argumentos, "idPais", 0);
            string observacao = Texto(argumentos, "observacao", 1000);
            string contatoNome = Texto(argumentos, "contatoNome", 50);
            string contatoTipo = Texto(argumentos, "contatoTipo", 50);
            string contatoTelefone = Texto(argumentos, "contatoTelefone", 20);
            string contatoEmail = Texto(argumentos, "contatoEmail", 100);
            string cep = SomenteDigitos(Texto(argumentos, "cep", 10));
            string logradouro = Texto(argumentos, "logradouro", 200);
            string numero = Texto(argumentos, "numero", 50);
            string complemento = Texto(argumentos, "complemento", 50);
            string bairro = Texto(argumentos, "bairro", 200);
            string cidade = Texto(argumentos, "cidade", 200);
            string uf = Texto(argumentos, "uf", 2).ToUpperInvariant();
            string pais = Texto(argumentos, "pais", 100);
            string usarMesmoEnderecoEntrega = NormalizarSimNao(Texto(argumentos, "usarMesmoEnderecoEntrega", 10));
            string entregaCep = SomenteDigitos(Texto(argumentos, "entregaCep", 10));
            string entregaLogradouro = Texto(argumentos, "entregaLogradouro", 200);
            string entregaNumero = Texto(argumentos, "entregaNumero", 50);
            string entregaComplemento = Texto(argumentos, "entregaComplemento", 50);
            string entregaBairro = Texto(argumentos, "entregaBairro", 200);
            string entregaCidade = Texto(argumentos, "entregaCidade", 200);
            string entregaUf = Texto(argumentos, "entregaUf", 2).ToUpperInvariant();
            string entregaPais = Texto(argumentos, "entregaPais", 100);

            if (string.IsNullOrWhiteSpace(nomeFantasia))
            {
                nomeFantasia = razaoSocial;
            }
            if (tipoPessoa != "J" && tipoPessoa != "F" && tipoPessoa != "O")
            {
                tipoPessoa = "J";
            }
            if (origemParceiro != "N" && origemParceiro != "E" && origemParceiro != "U")
            {
                origemParceiro = "N";
            }
            if (origemParceiro == "N")
            {
                documento = SomenteDigitos(documento);
            }
            if (string.IsNullOrWhiteSpace(contatoTipo))
            {
                contatoTipo = "Comercial";
            }
            if (string.IsNullOrWhiteSpace(pais))
            {
                pais = "Brasil";
            }

            CompletarEnderecoPorCep(cep, ref logradouro, ref bairro, ref cidade, ref uf, ref pais);
            CompletarEnderecoPorCep(entregaCep, ref entregaLogradouro, ref entregaBairro, ref entregaCidade, ref entregaUf, ref entregaPais);

            if (usarMesmoEnderecoEntrega == "S")
            {
                entregaCep = cep;
                entregaLogradouro = logradouro;
                entregaNumero = numero;
                entregaComplemento = complemento;
                entregaBairro = bairro;
                entregaCidade = cidade;
                entregaUf = uf;
                entregaPais = pais;
            }
            else if (string.IsNullOrWhiteSpace(entregaPais))
            {
                entregaPais = pais;
            }

            List<string> erros = new List<string>();
            if (razaoSocial.Length < 3) erros.Add("Informe a razão social/nome do cliente.");
            if (string.IsNullOrWhiteSpace(documento) && permitirSemDocumento != "S") erros.Add("Informe o documento do cliente ou marque permitirSemDocumento = S.");
            if (contatoNome.Length < 3) erros.Add("Informe o contato principal.");
            if (logradouro.Length < 2) erros.Add("Informe o logradouro fiscal.");
            if (cidade.Length < 2) erros.Add("Informe a cidade fiscal.");
            if (!UfValidaOuVazia(uf)) erros.Add("UF fiscal inválida.");
            if (usarMesmoEnderecoEntrega != "S")
            {
                if (entregaLogradouro.Length < 2) erros.Add("Informe o logradouro de entrega ou use usarMesmoEnderecoEntrega = S.");
                if (entregaCidade.Length < 2) erros.Add("Informe a cidade de entrega ou use usarMesmoEnderecoEntrega = S.");
                if (!UfValidaOuVazia(entregaUf)) erros.Add("UF de entrega inválida.");
            }

            if (erros.Count > 0)
            {
                erro = string.Join(" ", erros.ToArray());
                return false;
            }

            normalizados = new JObject
            {
                { "razaoSocial", razaoSocial },
                { "nomeFantasia", nomeFantasia },
                { "tipoPessoa", tipoPessoa },
                { "origemParceiro", origemParceiro },
                { "documento", documento },
                { "permitirSemDocumento", permitirSemDocumento },
                { "rgIe", rgIe },
                { "inscricaoMunicipal", inscricaoMunicipal },
                { "idVendedor", idVendedor },
                { "sidTabela", sidTabela },
                { "sidTipoParceiro", sidTipoParceiro },
                { "idPais", idPais },
                { "observacao", observacao },
                { "contatoNome", contatoNome },
                { "contatoTipo", contatoTipo },
                { "contatoTelefone", contatoTelefone },
                { "contatoEmail", contatoEmail },
                { "cep", cep },
                { "logradouro", logradouro },
                { "numero", numero },
                { "complemento", complemento },
                { "bairro", bairro },
                { "cidade", cidade },
                { "uf", uf },
                { "pais", pais },
                { "usarMesmoEnderecoEntrega", usarMesmoEnderecoEntrega },
                { "entregaCep", entregaCep },
                { "entregaLogradouro", entregaLogradouro },
                { "entregaNumero", entregaNumero },
                { "entregaComplemento", entregaComplemento },
                { "entregaBairro", entregaBairro },
                { "entregaCidade", entregaCidade },
                { "entregaUf", entregaUf },
                { "entregaPais", entregaPais }
            };

            resumo = "Criar cliente para orçamento: " + razaoSocial + " (" + (string.IsNullOrWhiteSpace(documento) ? "sem documento informado" : documento) + ")";
            return true;
        }

        private static string ExecutarCepConsultar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string cep = SomenteDigitos(Texto(argumentos, "cep", 10));
            if (cep.Length != 8)
            {
                return JsonErro("CEP inválido. Informe 8 dígitos.");
            }

            JObject endereco = EnderecoPorCep(cep);
            bool encontrado = endereco != null && endereco.Value<bool>("encontrado");
            if (!encontrado)
            {
                return new JObject
                {
                    { "sucesso", true },
                    { "ferramenta", ferramenta.Nome },
                    { "cep", cep },
                    { "encontrado", false },
                    { "mensagem", "CEP não encontrado." }
                }.ToString(Formatting.None);
            }

            endereco["sucesso"] = true;
            endereco["ferramenta"] = ferramenta.Nome;
            return endereco.ToString(Formatting.None);
        }

        private static Dictionary<string, string> MontarParametrosClientesCriar(JObject args)
        {
            return new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR" },
                { "@idParceiro", "0" },
                { "@idCliente", "0" },
                { "@sNomeFantasia", Texto(args, "nomeFantasia", 200) },
                { "@sRazaoSocial", Texto(args, "razaoSocial", 200) },
                { "@sTipo", Texto(args, "tipoPessoa", 1) },
                { "@sCPF_CNPJ", Texto(args, "documento", 50) },
                { "@sRG_IE", Texto(args, "rgIe", 15) },
                { "@idVendedor", Inteiro(args, "idVendedor", 0).ToString() },
                { "@sidTabela", Texto(args, "sidTabela", 10) },
                { "@sObservacao", Texto(args, "observacao", 1000) },
                { "@sObservacaoFinanceira", string.Empty },
                { "@sidTipoParceiro", Texto(args, "sidTipoParceiro", 50) },
                { "@sSituacao", "S" },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().Trim() },
                { "@sTipoCliente", Texto(args, "origemParceiro", 1) },
                { "@sVendaIndividual", "N" },
                { "@sDrawback", "N" },
                { "@sContribuinte", "N" },
                { "@idPais", Inteiro(args, "idPais", 0).ToString() },
                { "@idComprador", "0" },
                { "@sInscricaoMunicipal", Texto(args, "inscricaoMunicipal", 15) },
                { "@sCadeadoFinanceiro", "N" },
                { "@sSuframa", string.Empty }
            };
        }

        private static Dictionary<string, string> MontarParametrosClienteContato(JObject args, int idCliente, int idHistorico)
        {
            return new Dictionary<string, string>
            {
                { "@sFuncao", "CONTATO_INCLUIR" },
                { "@idContato", "0" },
                { "@idParceiro", idCliente.ToString() },
                { "@idCliente", idCliente.ToString() },
                { "@sTipoContato", Texto(args, "contatoTipo", 50) },
                { "@sNome", Texto(args, "contatoNome", 50) },
                { "@sTelefone", Texto(args, "contatoTelefone", 20) },
                { "@sEmail", Texto(args, "contatoEmail", 100) },
                { "@sRecebeEmail", string.IsNullOrWhiteSpace(Texto(args, "contatoEmail", 100)) ? "N" : "S" },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().Trim() },
                { "@idHistorico", idHistorico.ToString() }
            };
        }

        private static Dictionary<string, string> MontarParametrosClienteEndereco(JObject args, int idCliente, int idHistorico, int idTipoEndereco, string prefixo)
        {
            string p = prefixo == "entrega" ? "entrega" : string.Empty;
            string cep = p == "entrega" ? Texto(args, "entregaCep", 10) : Texto(args, "cep", 10);
            string logradouro = p == "entrega" ? Texto(args, "entregaLogradouro", 200) : Texto(args, "logradouro", 200);
            string numero = p == "entrega" ? Texto(args, "entregaNumero", 50) : Texto(args, "numero", 50);
            string complemento = p == "entrega" ? Texto(args, "entregaComplemento", 50) : Texto(args, "complemento", 50);
            string bairro = p == "entrega" ? Texto(args, "entregaBairro", 200) : Texto(args, "bairro", 200);
            string cidade = p == "entrega" ? Texto(args, "entregaCidade", 200) : Texto(args, "cidade", 200);
            string uf = p == "entrega" ? Texto(args, "entregaUf", 2) : Texto(args, "uf", 2);
            string pais = p == "entrega" ? Texto(args, "entregaPais", 100) : Texto(args, "pais", 100);

            return new Dictionary<string, string>
            {
                { "@sFuncao", "ENDERECO_INCLUIR" },
                { "@idEndereco", "0" },
                { "@idParceiro", idCliente.ToString() },
                { "@idCliente", idCliente.ToString() },
                { "@idTipoEndereco", idTipoEndereco.ToString() },
                { "@sCEP", cep },
                { "@sLogradouro", logradouro },
                { "@sNumero", numero },
                { "@sComplemento", complemento },
                { "@sBairro", bairro },
                { "@sCidade", cidade },
                { "@sEstado", uf.ToUpperInvariant() },
                { "@sPais", pais },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().Trim() },
                { "@idHistorico", idHistorico.ToString() }
            };
        }

        private static void CompletarEnderecoPorCep(string cep, ref string logradouro, ref string bairro, ref string cidade, ref string uf, ref string pais)
        {
            JObject dados = EnderecoPorCep(cep);
            if (dados == null || !dados.Value<bool>("encontrado"))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(logradouro)) logradouro = StrJ(dados["logradouro"]);
            if (string.IsNullOrWhiteSpace(bairro)) bairro = StrJ(dados["bairro"]);
            if (string.IsNullOrWhiteSpace(cidade)) cidade = StrJ(dados["cidade"]);
            if (string.IsNullOrWhiteSpace(uf)) uf = StrJ(dados["uf"]).ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(pais)) pais = StrJ(dados["pais"]);
        }

        private static JObject EnderecoPorCep(string cep)
        {
            cep = SomenteDigitos(cep);
            if (cep.Length != 8)
            {
                return new JObject { { "cep", cep }, { "encontrado", false } };
            }

            try
            {
                Dictionary<string, string> parametros = new Dictionary<string, string>
                {
                    { "@sCEP", cep }
                };

                DataSet ds = BD.ExecutarDataSet("sp_Consulta_CEP", parametros);
                DataTable tb = Tabela(ds, 0);
                if (tb == null || tb.Rows.Count == 0)
                {
                    return new JObject { { "cep", cep }, { "encontrado", false } };
                }

                DataRow row = tb.Rows[0];
                string cidade = PrimeiroTextoNaoVazio(Valor(row, "sCidade"), Valor(row, "sDscCidade"), Valor(row, "Cidade"), Valor(row, "cidade"));
                string uf = PrimeiroTextoNaoVazio(Valor(row, "sUF"), Valor(row, "UF"), Valor(row, "sEstado"));

                return new JObject
                {
                    { "cep", cep },
                    { "encontrado", true },
                    { "logradouro", PrimeiroTextoNaoVazio(Valor(row, "sLogradouro"), Valor(row, "Logradouro"), Valor(row, "logradouro")) },
                    { "bairro", PrimeiroTextoNaoVazio(Valor(row, "sBairro"), Valor(row, "Bairro"), Valor(row, "bairro")) },
                    { "cidade", cidade },
                    { "uf", uf },
                    { "idCidade", Inteiro(Valor(row, "idCidade"), Inteiro(Valor(row, "idMunicipio"), 0)) },
                    { "pais", PrimeiroTextoNaoVazio(Valor(row, "sPais"), Valor(row, "Pais"), "Brasil") }
                };
            }
            catch
            {
                return new JObject { { "cep", cep }, { "encontrado", false } };
            }
        }

        private static string ResolverSidTipoParceiroCliente()
        {
            try
            {
                Dictionary<string, string> parametros = new Dictionary<string, string>
                {
                    { "@sTabela", "Flow_Tipo_Parceiro" }
                };

                DataTable tb = BD.ExecutarDataTable("sp_Select", parametros);
                int idContem = 0;
                if (tb != null)
                {
                    foreach (DataRow row in tb.Rows)
                    {
                        int id = Inteiro(PrimeiroTextoNaoVazio(Valor(row, "idTipoParceiro"), Valor(row, "idRegistro"), Valor(row, "idTipo")), 0);
                        string nome = Normalizar(PrimeiroTextoNaoVazio(Valor(row, "sDscTipoParceiro"), Valor(row, "sDescricao"), Valor(row, "sDscTipo"), Valor(row, "sTipoParceiro")));
                        if (id <= 0 || string.IsNullOrWhiteSpace(nome))
                        {
                            continue;
                        }

                        if (nome == "cliente")
                        {
                            return id.ToString() + ";";
                        }
                        if (idContem <= 0 && nome.IndexOf("cliente", StringComparison.Ordinal) >= 0)
                        {
                            idContem = id;
                        }
                    }
                }

                if (idContem > 0)
                {
                    return idContem.ToString() + ";";
                }
            }
            catch
            {
            }

            return "1;";
        }

        private static string ExecutarOrcamentoEmpresasBuscar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 120);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (termo.Length < 2)
            {
                return JsonErro("Informe ao menos 2 caracteres para buscar empresas.");
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscEmpresa", string.Empty },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().Trim() }
            };

            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Empresas", parametros);
            DataTable tb = FiltrarTabelaPorTermo(Tabela(ds, 0), termo, "sDscEmpresa", "sDscEmpresaReduzida", "sDscParceiro", "CNPJ", "CNPJParceiro", "sEstado");
            return TabelaResolucaoParaJson(ferramenta, tb, limite, termo, new[]
            {
                "idEmpresa",
                "sDscEmpresa",
                "sDscEmpresaReduzida",
                "idParceiro",
                "sDscParceiro",
                "CNPJ",
                "CNPJParceiro",
                "idTipoPais",
                "sdscTipoPais",
                "sEstado",
                "idMunicipio",
                "sDscMunicipio",
                "idTributacao",
                "sDscTributacao",
                "sTipoOrcamento",
                "sidTipoFaturamento"
            }, new[] { "idEmpresa", "sDscEmpresa", "sDscEmpresaReduzida", "sDscParceiro", "CNPJ", "CNPJParceiro", "sEstado" }, null);
        }

        private static string ExecutarOrcamentoVendedoresBuscar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 120);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (termo.Length < 2)
            {
                return JsonErro("Informe ao menos 2 caracteres para buscar usuários do sistema.");
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sTabela", "FLOW_Vendedores" }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Select", parametros);
            tb = FiltrarTabelaPorTermo(tb, termo, "sDscUsuario", "sEmail", "sLogin", "sNome");
            ConverterVendedorParaUsuario(tb);

            return TabelaResolucaoParaJson(ferramenta, tb, limite, termo, new[]
            {
                "idVendedor",
                "idUsuario",
                "idVendedorComercial",
                "sDscUsuario",
                "sEmail",
                "sLogin"
            }, new[] { "idVendedor", "idUsuario", "idVendedorComercial", "sDscUsuario", "sEmail", "sLogin" }, null);
        }

        /// <summary>
        /// O orçamento guarda no campo de vendedor o USUÁRIO responsável: a tela grava o que está
        /// selecionado em ddlVendedor, que é lista de usuários, e é assim que ela lê de volta.
        ///
        /// A consulta FLOW_Vendedores traz as duas coisas — o código comercial (idVendedor) e o
        /// usuário (idUsuario) da mesma pessoa. Devolver o código comercial aqui fazia o orçamento
        /// nascer apontando para outra pessoa: o vendedor 28 (Thiago) virava o usuário 28 (Maria),
        /// e a tela de orçamento quebrava ao editar, porque esse número não existe na lista de
        /// usuários. Por isso idVendedor sai com o id do usuário; o código comercial continua
        /// disponível em idVendedorComercial.
        /// </summary>
        private static void ConverterVendedorParaUsuario(DataTable tabela)
        {
            if (tabela == null || !tabela.Columns.Contains("idUsuario"))
            {
                return;
            }

            if (!tabela.Columns.Contains("idVendedor"))
            {
                tabela.Columns.Add("idVendedor", typeof(string));
            }

            if (!tabela.Columns.Contains("idVendedorComercial"))
            {
                tabela.Columns.Add("idVendedorComercial", typeof(string));
            }

            foreach (DataRow linha in tabela.Rows)
            {
                string idUsuario = Valor(linha, "idUsuario");
                if (string.IsNullOrWhiteSpace(idUsuario))
                {
                    continue;
                }

                linha["idVendedorComercial"] = Valor(linha, "idVendedor");
                linha["idVendedor"] = idUsuario;
            }
        }

        private static string ExecutarOrcamentoUsuariosResolver(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string modoUsuario = Texto(argumentos, "modoUsuario", 30).ToUpperInvariant();
            if (modoUsuario != "USUARIO_LOGADO" && modoUsuario != "OUTRO_USUARIO")
            {
                return JsonErro("Escolha usar o usuário logado ou selecionar outro usuário do sistema.");
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sTabela", "Usuarios" }
            };

            DataTable usuarios = BD.ExecutarDataTable("sp_Select", parametros);
            if (usuarios == null)
            {
                return JsonErro("Não foi possível consultar os usuários do sistema.");
            }

            GarantirColunaAlias(usuarios, "idVendedor", "idUsuario");
            int idUsuarioLogado = Inteiro(IDENTITY.Variaveis.idUsuario().Trim(), 0);
            DataTable candidatos = usuarios.Clone();
            foreach (DataRow usuario in usuarios.Rows)
            {
                int idUsuario = Inteiro(Valor(usuario, "idUsuario"), 0);
                bool incluir = modoUsuario == "USUARIO_LOGADO"
                    ? idUsuario > 0 && idUsuario == idUsuarioLogado
                    : idUsuario > 0 && idUsuario != idUsuarioLogado;
                if (incluir)
                {
                    candidatos.ImportRow(usuario);
                }
            }

            string termoRanking = string.Empty;
            if (modoUsuario == "USUARIO_LOGADO" && candidatos.Rows.Count > 0)
            {
                termoRanking = PrimeiroTextoNaoVazio(
                    Valor(candidatos.Rows[0], "sDscUsuario"),
                    Valor(candidatos.Rows[0], "sLogin"),
                    idUsuarioLogado.ToString());
            }

            JObject resolucao = JObject.Parse(TabelaResolucaoParaJson(ferramenta, candidatos, ferramenta.MaxRegistros, termoRanking, new[]
            {
                "idUsuario",
                "idVendedor",
                "sDscUsuario",
                "sEmail",
                "sLogin"
            }, new[] { "sDscUsuario", "sLogin", "sEmail", "idUsuario" }, null));

            resolucao["modoUsuario"] = modoUsuario;
            if (modoUsuario == "USUARIO_LOGADO")
            {
                if (candidatos.Rows.Count == 0)
                {
                    return JsonErro("O usuário autenticado não foi localizado entre os usuários disponíveis do sistema.");
                }

                resolucao["encontrado"] = true;
                resolucao["unicoProvavel"] = true;
                resolucao["precisaEscolha"] = false;
                resolucao["mensagem"] = "O usuário autenticado será o responsável pelo orçamento.";
            }
            else
            {
                resolucao["encontrado"] = false;
                resolucao["unicoProvavel"] = false;
                resolucao["precisaEscolha"] = candidatos.Rows.Count > 0;
                resolucao["melhorResultado"] = new JObject();
                resolucao["mensagem"] = candidatos.Rows.Count > 0
                    ? "Escolha o usuário do sistema que será responsável pelo orçamento."
                    : "Não há outro usuário do sistema disponível para seleção.";
            }

            return resolucao.ToString(Formatting.None);
        }

        private static string ExecutarOrcamentoTabelasPrecoBuscar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 120);
            int idTipoTabela = Inteiro(argumentos, "idTipoTabela", 0);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (termo.Length < 2)
            {
                return JsonErro("Informe ao menos 2 caracteres para buscar tabelas de preço.");
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscTabela", termo },
                { "@idTipoTabela", idTipoTabela.ToString() },
                { "@sSituacao", "S" },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().Trim() }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", parametros, false);
            if (tb == null || tb.Rows.Count == 0)
            {
                parametros["@sDscTabela"] = string.Empty;
                tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", parametros, false);
                tb = FiltrarTabelaPorTermo(tb, termo, "sDscTabela", "sObservacao", "sDscTipoTabela", "sDscParceiro");
            }

            return TabelaResolucaoParaJson(ferramenta, tb, limite, termo, new[]
            {
                "idTabela",
                "idTipoTabela",
                "sDscTabela",
                "sObservacao",
                "sDscTipoTabela",
                "sSimboloMoedaOrigem",
                "sSimboloMoedaDestino",
                "sSituacao",
                "sValidada",
                "sCalculaImpostos",
                "sidParceiro",
                "sDscParceiro",
                "dtAtualizacao"
            }, new[] { "idTabela", "sDscTabela", "sObservacao", "sDscTipoTabela", "sDscParceiro" }, null);
        }

        private static string ExecutarOrcamentoTabelasPrecoListar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            int idTipoTabela = Inteiro(argumentos, "idTipoTabela", 0);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscTabela", string.Empty },
                { "@idTipoTabela", idTipoTabela.ToString() },
                { "@sSituacao", "S" },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().Trim() }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", parametros, false);
            return TabelaParaJson(ferramenta, tb, limite, new[]
            {
                "idTabela",
                "idTipoTabela",
                "sDscTabela",
                "sObservacao",
                "sDscTipoTabela",
                "sSimboloMoedaOrigem",
                "sSimboloMoedaDestino",
                "sSituacao",
                "sValidada",
                "sCalculaImpostos",
                "sidParceiro",
                "sDscParceiro",
                "dtAtualizacao"
            }, null);
        }

        private static string ExecutarOrcamentoTiposEnvioBuscar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 120);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (termo.Length < 2)
            {
                return JsonErro("Informe ao menos 2 caracteres para buscar formas de envio.");
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sTabela", "Flow_Pedidos_TipoEnvio" },
                { "@idPesquisa", "2" }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Select", parametros);
            tb = FiltrarTabelaPorTermo(tb, termo, "sDscTipoEnvio");

            return TabelaResolucaoParaJson(ferramenta, tb, limite, termo, new[]
            {
                "idTipoEnvio",
                "sDscTipoEnvio",
                "sSituacao",
                "sAtivo"
            }, new[] { "idTipoEnvio", "sDscTipoEnvio" }, null);
        }

        private static string ExecutarOrcamentoProdutosBuscar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 120);
            int idTabela = Inteiro(argumentos, "idTabela", 0);
            string tabelaPreco = Texto(argumentos, "tabelaPreco", 120);
            string tipoItem = Texto(argumentos, "tipoItem", 30).ToLowerInvariant();
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (termo.Length < 2)
            {
                return JsonErro("Informe ao menos 2 caracteres para buscar produtos, serviços ou recursos.");
            }

            if (tipoItem != "todos" && tipoItem != "produto" && tipoItem != "servico_recurso")
            {
                return JsonErro("tipoItem inválido. Use todos, produto ou servico_recurso.");
            }
            if (!string.IsNullOrWhiteSpace(tabelaPreco))
            {
                string erroTabela;
                int idTabelaItem = ResolverTabelaPrecoPorTermo(tabelaPreco, out erroTabela);
                if (idTabelaItem <= 0)
                {
                    return JsonErro(erroTabela);
                }
                idTabela = idTabelaItem;
            }

            DataTable tb;
            if (idTabela > 0)
            {
                Dictionary<string, string> parametrosTabela = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_ITENS_DISPONIVEIS" },
                    { "@idTabela", idTabela.ToString() }
                };

                tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", parametrosTabela, false);
                tb = FiltrarTabelaPorTermo(tb, termo, "sCodigo", "sDscProduto", "sCodigoComDescricao", "sDscGrupo", "sDscFamilia", "sDscTipoProduto");
            }
            else
            {
                tb = BuscarProdutosCadastro(termo, tipoItem);
            }

            return TabelaResolucaoParaJson(ferramenta, tb, limite, termo, new[]
            {
                "idItem",
                "idProduto",
                "idTabela",
                "nPreco",
                "nTotal",
                "nMargem",
                "sLiberado",
                "sCodigo",
                "sDscProduto",
                "sCodigoComDescricao",
                "sUnidade",
                "idTipoProduto",
                "sDscTipoProduto",
                "idGrupo",
                "sDscGrupo",
                "idFamilia",
                "sDscFamilia",
                "sIndustrializado",
                "sOrigem",
                "sExibeComercial",
                "sSituacao",
                "sSituacao_Completa"
            }, new[] { "idItem", "idProduto", "sCodigo", "sDscProduto", "sCodigoComDescricao", "sDscGrupo", "sDscFamilia", "sDscTipoProduto" }, "/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={idItem}");
        }

        private static string ExecutarOrcamentoProdutoPrecoResolver(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            int idProduto = Inteiro(argumentos, "idProduto", 0);
            int idTabelaPreco = Inteiro(argumentos, "idTabelaPreco", 0);
            int idOrcamento = Inteiro(argumentos, "idOrcamento", 0);
            string tabelaPreco = Texto(argumentos, "tabelaPreco", 120);
            decimal quantidade = DecimalValor(argumentos, "quantidade", 0m);
            decimal descontoPercentual = DecimalValor(argumentos, "descontoPercentual", 0m);
            string tipoItem = Texto(argumentos, "tipoItem", 30).ToLowerInvariant();

            if (idProduto <= 0)
            {
                return JsonErro("idProduto inválido.");
            }
            if (quantidade <= 0)
            {
                return JsonErro("Informe uma quantidade maior que zero.");
            }
            if (descontoPercentual < 0 || descontoPercentual > 100)
            {
                return JsonErro("O desconto deve ficar entre 0 e 100%.");
            }
            if (tipoItem != "produto" && tipoItem != "servico_recurso")
            {
                return JsonErro("tipoItem inválido. Use produto ou servico_recurso.");
            }
            if (idTabelaPreco <= 0 && !string.IsNullOrWhiteSpace(tabelaPreco))
            {
                string erroTabela;
                idTabelaPreco = ResolverTabelaPrecoPorTermo(tabelaPreco, out erroTabela);
                if (idTabelaPreco <= 0)
                {
                    return JsonErro(erroTabela);
                }
            }

            JObject preco;
            string erro;
            if (!ResolverPrecoProdutoOrcamento(idProduto, idTabelaPreco, idOrcamento, quantidade, descontoPercentual, tipoItem, out preco, out erro))
            {
                return JsonErro(erro);
            }

            preco["sucesso"] = true;
            preco["ferramenta"] = ferramenta.Nome;
            return preco.ToString(Formatting.None);
        }

        private static string ExecutarOrcamentoProdutosSugestoesListar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            int idProduto = Inteiro(argumentos, "idProduto", 0);
            int idCliente = Inteiro(argumentos, "idCliente", 0);
            decimal quantidade = DecimalValor(argumentos, "quantidade", 0m);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (idProduto <= 0) return JsonErro("idProduto inválido para consultar sugestões.");
            if (quantidade <= 0) return JsonErro("Informe a quantidade do produto principal.");

            DataTable tb = BD.ExecutarDataTable("sp_IA_Orcamento_ProdutosSugestoes", new Dictionary<string, string>
            {
                { "@idProduto", idProduto.ToString() },
                { "@idCliente", idCliente.ToString() },
                { "@nQuantidadePrincipal", DecimalSql(quantidade, 4) },
                { "@nLimite", limite.ToString() }
            }, false);

            JObject retorno = JObject.Parse(TabelaParaJson(ferramenta, tb, limite, new[]
            {
                "idProdutoPrincipal", "idProduto", "sCodigo", "sDscProduto", "sUnidade",
                "quantidadeBase", "quantidadeSugerida", "sSituacao", "sExibeComercial"
            }, "/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={idProduto}"));

            JArray registros = retorno["registros"] as JArray ?? new JArray();
            JArray opcoes = new JArray();
            foreach (JToken token in registros)
            {
                JObject registro = token as JObject;
                if (registro == null) continue;
                string codigo = StrJ(registro["sCodigo"]);
                string descricao = StrJ(registro["sDscProduto"]);
                string quantidadeTexto = StrJ(registro["quantidadeSugerida"]);
                string unidade = StrJ(registro["sUnidade"]);
                opcoes.Add(new JObject
                {
                    { "valor", "idProduto:" + StrJ(registro["idProduto"]) },
                    { "rotulo", PrimeiroTextoNaoVazio((codigo + " - " + descricao).Trim(' ', '-'), descricao, codigo) },
                    { "descricao", "Quantidade sugerida: " + quantidadeTexto + (string.IsNullOrWhiteSpace(unidade) ? string.Empty : " " + unidade) },
                    { "registro", registro.DeepClone() }
                });
            }

            bool temSugestoes = registros.Count > 0;
            retorno["encontrado"] = temSugestoes;
            retorno["unicoProvavel"] = false;
            retorno["precisaEscolha"] = temSugestoes;
            retorno["selecaoMultipla"] = true;
            retorno["melhorResultado"] = new JObject();
            retorno["opcoes"] = opcoes;
            retorno["mensagem"] = temSugestoes
                ? "Há produtos complementares opcionais. Selecione os que deseja incluir."
                : "Nenhum produto complementar ativo foi encontrado.";
            return retorno.ToString(Formatting.None);
        }

        private static string ExecutarOrcamentoClienteDetalhar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            int idCliente = Inteiro(argumentos, "idCliente", 0);
            if (idCliente <= 0)
            {
                return JsonErro("idCliente inválido.");
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idParceiro", idCliente.ToString() }
            };

            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Clientes", parametros);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return JsonErro("Cliente não encontrado.");
            }

            int limite = Limite(argumentos, ferramenta.MaxRegistros);
            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "cliente", TabelaParaArray(Tabela(ds, 0), 1, new[]
                    {
                        "idCliente",
                        "sNomeFantasia",
                        "sRazaoSocial",
                        "sEmail",
                        "sTelefone",
                        "sTipo",
                        "sCPF_CNPJ",
                        "sRG_IE",
                        "sObservacao",
                        "sidTabela",
                        "idVendedor",
                        "sSituacao",
                        "sSituacao_Completa",
                        "idTipoSituacaoCliente",
                        "sTipoCliente",
                        "sTabelaVinculada",
                        "sVendaIndividual",
                        "sDrawback",
                        "sContribuinte",
                        "idPais",
                        "idComprador",
                        "sidTipoParceiro",
                        "sInscricaoMunicipal",
                        "sCadeadoFinanceiro",
                        "sSuframa"
                    }, "/App/Paginas/Manutencao/Parceiros_Detalhe.aspx?id={idCliente}") },
                { "contatos", TabelaParaArray(Tabela(ds, 1), limite, new[]
                    {
                        "idContato",
                        "sNome",
                        "sEmail",
                        "sTelefone",
                        "sCelular",
                        "sCargo",
                        "sDepartamento",
                        "sSituacao"
                    }, null) },
                { "enderecos", TabelaParaArray(Tabela(ds, 2), limite, new[]
                    {
                        "idEndereco",
                        "idTipoEndereco",
                        "sDscTipoEndereco",
                        "sCEP",
                        "sLogradouro",
                        "sNumero",
                        "sComplemento",
                        "sBairro",
                        "sCidade",
                        "sEstado",
                        "sPais",
                        "sEnderecoCompleto",
                        "sEnderecoEstrangeiro"
                    }, null) }
            }.ToString(Formatting.None);
        }

        private static string ExecutarOrcamentoEmpresaDetalhar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            int idEmpresa = Inteiro(argumentos, "idEmpresa", 0);
            int idParceiro = Inteiro(argumentos, "idParceiro", 0);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (idEmpresa <= 0 && idParceiro <= 0)
            {
                return JsonErro("Informe idEmpresa ou idParceiro.");
            }

            DataSet ds = ConsultarEmpresaOrcamento(idEmpresa, idParceiro);
            DataTable empresas = Tabela(ds, 0);
            if ((empresas == null || empresas.Rows.Count == 0) && ds != null && ds.Tables.Count > 2)
            {
                empresas = ds.Tables[2];
            }

            if (empresas == null || empresas.Rows.Count == 0)
            {
                return JsonErro("Empresa não encontrada.");
            }

            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "empresas", TabelaParaArray(empresas, limite, new[]
                    {
                        "idEmpresa",
                        "sDscEmpresa",
                        "sDscEmpresaReduzida",
                        "idParceiro",
                        "sDscParceiro",
                        "idTipoPais",
                        "sdscTipoPais",
                        "CNPJParceiro",
                        "CNPJ",
                        "sEstado",
                        "idMunicipio",
                        "sDscMunicipio",
                        "idTributacao",
                        "sDscTributacao",
                        "sTipoOrcamento",
                        "sTipoCliente",
                        "sEfetuaCompras",
                        "sIE_Empresa",
                        "sEnderecoEmpresaCompleto",
                        "sidTipoFaturamento"
                    }, null) },
                { "impostos", TabelaParaArray(Tabela(ds, 1), limite, new[]
                    {
                        "idRegistro",
                        "idEmpresa",
                        "idTipo",
                        "sDscTipo",
                        "nPis",
                        "nCofins",
                        "nICMS",
                        "nCSSL",
                        "nIRPJ",
                        "nAno"
                    }, null) }
            }.ToString(Formatting.None);
        }

        private static string ExecutarOrcamentoTabelaPrecoDetalhar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            int idTabela = Inteiro(argumentos, "idTabela", 0);
            if (idTabela <= 0)
            {
                return JsonErro("idTabela inválido.");
            }

            int limite = Limite(argumentos, ferramenta.MaxRegistros);
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idTabela", idTabela.ToString() }
            };

            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", parametros);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return JsonErro("Tabela de preço não encontrada.");
            }

            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "detalhe", TabelaParaArray(Tabela(ds, 0), 1, new[]
                    {
                        "idTabela",
                        "sDscTabela",
                        "sObservacao",
                        "idTipoTabela",
                        "sDscTipoTabela",
                        "idMoedaOrigem",
                        "idMoedaDestino",
                        "sSimboloMoedaOrigem",
                        "sSimboloMoedaDestino",
                        "nTaxaCambio",
                        "nFator",
                        "nDesconto",
                        "nTaxaEnvio",
                        "nTaxaLocal",
                        "nMargem",
                        "sSituacao",
                        "sValidada",
                        "sCalculaImpostos",
                        "dtVigencia_Inicial",
                        "dtVigencia_Final",
                        "nItens"
                    }, null) },
                { "itensAmostra", TabelaParaArray(Tabela(ds, 1), limite, ColunasItensTabelaPreco(), "/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={idItem}") },
                { "tabelasVinculadas", TabelaParaArray(Tabela(ds, 2), limite, new[] { "idTabela", "sDscTabela" }, null) },
                { "controlesFator", TabelaParaArray(Tabela(ds, 3), limite, new[] { "idTabela", "idTipoObjeto", "sDscObjeto", "idObjeto", "nFator", "sBloquearEdicao" }, null) }
            }.ToString(Formatting.None);
        }

        private static string ExecutarOrcamentoItensDisponiveisListar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            int idTabela = Inteiro(argumentos, "idTabela", 0);
            string termo = Texto(argumentos, "termo", 120);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (idTabela <= 0)
            {
                return JsonErro("idTabela inválido.");
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_ITENS_DISPONIVEIS" },
                { "@idTabela", idTabela.ToString() }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", parametros, false);
            tb = FiltrarTabelaPorTermo(tb, termo, "sCodigo", "sDscProduto", "sCodigoComDescricao", "sDscGrupo", "sDscFamilia", "sDscTipoProduto");
            return TabelaParaJson(ferramenta, tb, limite, ColunasItensTabelaPreco(), "/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={idItem}");
        }

        private static string ExecutarOrcamentoProdutoCodigoConsultar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string codigoProduto = Texto(argumentos, "codigoProduto", 50);
            int idTabela = Inteiro(argumentos, "idTabela", 0);
            int idCliente = Inteiro(argumentos, "idCliente", 0);

            if (string.IsNullOrWhiteSpace(codigoProduto))
            {
                return JsonErro("Informe o código do produto.");
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA_PRODUTO_x_CODIGO" },
                { "@sCodigo", codigoProduto },
                { "@idTabela", idTabela.ToString() },
                { "@idParceiro_Cliente", idCliente.ToString() }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Produtos", parametros, false);
            string erroTabela = ErroRetornoTabela(tb);
            if (!string.IsNullOrWhiteSpace(erroTabela))
            {
                return JsonErro(erroTabela);
            }

            return TabelaParaJson(ferramenta, tb, ferramenta.MaxRegistros, new[]
            {
                "idItem",
                "idTipoProduto",
                "sTipoProduto",
                "idGrupo",
                "idFamilia",
                "sDscGrupo",
                "sDscFamilia",
                "sDscProduto",
                "sCodigoNCM",
                "sCodigoCEST",
                "sCodigo",
                "sUnidade",
                "idTipoTabela",
                "idTabela",
                "nTotal",
                "sLiberado",
                "sIndustrializado",
                "sOrigem",
                "nIPI",
                "nPIS",
                "nCOFINS",
                "nICMS",
                "nPesoBruto",
                "nPesoNeto",
                "nVolume",
                "sExibeComercial"
            }, "/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={idItem}");
        }

        private static string ExecutarOrcamentoProdutoComposicaoConsultar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            int idProduto = Inteiro(argumentos, "idProduto", 0);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (idProduto <= 0)
            {
                return JsonErro("idProduto inválido.");
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_COMPOSICAO" },
                { "@idProduto", idProduto.ToString() }
            };

            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", parametros);
            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "idProduto", idProduto },
                { "tipoItem", TabelaParaArray(Tabela(ds, 0), 1, new[] { "idTipo" }, null) },
                { "composicao", TabelaParaArray(Tabela(ds, 1), limite, new[]
                    {
                        "idRegistro",
                        "iditem",
                        "idProduto",
                        "nOrdem",
                        "idItemComposicao",
                        "sCodigo",
                        "sDscProduto",
                        "sDscTipoProduto",
                        "nQuantidade",
                        "sUnidade",
                        "sExibePedido",
                        "idTabela",
                        "nPreco",
                        "nFator",
                        "idTipo"
                    }, "/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={idItemComposicao}") }
            }.ToString(Formatting.None);
        }

        private static string ExecutarOrcamentoCondicaoPagamentoDetalhar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            int idCondicaoPagamento = Inteiro(argumentos, "idCondicaoPagamento", 0);
            string termo = Texto(argumentos, "termo", 50);
            int tipoVinculo = Inteiro(argumentos, "tipoVinculo", 0);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (tipoVinculo < 0 || tipoVinculo > 2)
            {
                tipoVinculo = 0;
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", idCondicaoPagamento > 0 ? "CONSULTAR_DETALHE" : "CONSULTAR" },
                { "@idCondicaoPagamento", idCondicaoPagamento.ToString() },
                { "@sDscCondicaoPagamento", termo },
                { "@idPedido", tipoVinculo.ToString() }
            };

            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_CondicaodePagamento", parametros);
            if (ds == null || ds.Tables.Count == 0)
            {
                return JsonErro("Não foi possível consultar condições de pagamento.");
            }

            DataTable condicoes = Tabela(ds, 0);
            if (idCondicaoPagamento <= 0 && !string.IsNullOrWhiteSpace(termo) && (condicoes == null || condicoes.Rows.Count == 0))
            {
                parametros["@sDscCondicaoPagamento"] = string.Empty;
                ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_CondicaodePagamento", parametros);
                condicoes = FiltrarTabelaPorTermo(Tabela(ds, 0), termo, "sDscCondicaoPagamento");
            }

            if (idCondicaoPagamento <= 0 && !string.IsNullOrWhiteSpace(termo))
            {
                JObject resolucao = JObject.Parse(TabelaResolucaoParaJson(ferramenta, condicoes, limite, termo, new[]
                {
                    "idCondicaoPagamento",
                    "sDscCondicaoPagamento",
                    "nQtdParcelas",
                    "sSituacao",
                    "sSituacao_Completa",
                    "idPedido",
                    "sLinkPedido",
                    "dtAtualizacao",
                    "sDscUsuarioAtualizacao"
                }, new[] { "idCondicaoPagamento", "sDscCondicaoPagamento" }, null));

                JArray registros = resolucao["registros"] as JArray ?? new JArray();
                HashSet<string> ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (JObject registro in registros.OfType<JObject>())
                {
                    string id = StrJ(registro["idCondicaoPagamento"]).Trim();
                    if (id.Length > 0) ids.Add(id);
                }

                resolucao["condicoes"] = registros.DeepClone();
                resolucao["parcelas"] = TabelaParaArray(FiltrarTabelaPorValores(Tabela(ds, 1), "idCondicaoPagamento", ids), limite, new[]
                {
                    "idRegistroCondicaoPagamento",
                    "idCondicaoPagamento",
                    "idTipoCondicaoPagamento",
                    "sDscCondicaoPagamento",
                    "nPorcentagemValor",
                    "nDDL"
                }, null);
                return resolucao.ToString(Formatting.None);
            }

            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "condicoes", TabelaParaArray(Tabela(ds, 0), limite, new[]
                    {
                        "idCondicaoPagamento",
                        "sDscCondicaoPagamento",
                        "nQtdParcelas",
                        "sSituacao",
                        "sSituacao_Completa",
                        "idPedido",
                        "sLinkPedido",
                        "dtAtualizacao",
                        "sDscUsuarioAtualizacao"
                    }, null) },
                { "parcelas", TabelaParaArray(Tabela(ds, 1), limite, new[]
                    {
                        "idRegistroCondicaoPagamento",
                        "idCondicaoPagamento",
                        "idTipoCondicaoPagamento",
                        "sDscCondicaoPagamento",
                        "nPorcentagemValor",
                        "nDDL"
                    }, null) }
            }.ToString(Formatting.None);
        }

        private static string PrepararOrcamentoCriarCabecalho(IAFerramentaDefinicao ferramenta, JObject argumentos, IAFerramentaResultado resultado)
        {
            JObject normalizados;
            string resumo;
            string erro;
            if (!ValidarOrcamentoCriarCabecalho(argumentos, out normalizados, out resumo, out erro))
            {
                resultado.Erro = erro;
                return JsonErro(erro);
            }

            resultado.ArgumentosJson = normalizados.ToString(Formatting.None);
            resultado.Status = "PENDENTE_CONFIRMACAO";
            resultado.ResumoAcao = resumo;

            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "acaoPendente", true },
                { "resumo", resumo },
                { "dadosNormalizados", normalizados },
                { "instrucao", "A ação ficou pendente de confirmação; não afirme que o orçamento já foi criado." }
            }.ToString(Formatting.None);
        }

        private static bool ExecutarOrcamentoCriarCabecalhoConfirmado(JObject argumentos, out string erro, out JToken saida)
        {
            erro = string.Empty;
            saida = new JObject { { "executado", true } };

            if (!FUNCOES.ValidaPermissao(Permissao.Comercial.Orcamento.Incluir, false))
            {
                erro = "Usuário sem permissão para incluir orçamento.";
                return false;
            }

            JObject normalizados;
            string resumo;
            if (!ValidarOrcamentoCriarCabecalho(argumentos, out normalizados, out resumo, out erro))
            {
                return false;
            }

            try
            {
                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", MontarParametrosOrcamentoCriarCabecalho(normalizados));
                string erroBanco = ErroRetornoDataSet(ds);
                if (!string.IsNullOrWhiteSpace(erroBanco))
                {
                    erro = "Não foi possível criar o cabeçalho do orçamento: " + erroBanco;
                    return false;
                }

                DataTable tb = Tabela(ds, 0);
                int idOrcamento = tb != null && tb.Rows.Count > 0 ? Inteiro(Valor(tb.Rows[0], "idPedido"), 0) : 0;
                if (idOrcamento <= 0)
                {
                    erro = "A procedure não retornou o idPedido do orçamento criado.";
                    return false;
                }

                int nNumeroOrcamento = 0;
                DataSet dsOrcamento = ConsultarPedidoOrcamento(idOrcamento);
                DataTable tbOrcamento = Tabela(dsOrcamento, 0);
                if (tbOrcamento != null && tbOrcamento.Rows.Count > 0)
                {
                    nNumeroOrcamento = Inteiro(Valor(tbOrcamento.Rows[0], "nNumeroPedido"), 0);
                }

                if (nNumeroOrcamento <= 0 && tb != null && tb.Rows.Count > 0)
                {
                    nNumeroOrcamento = Inteiro(Valor(tb.Rows[0], "nNumeroPedido"), 0);
                }

                string observacaoHistorico = Texto(normalizados, "observacaoHistorico", 500);
                bool historicoGravado = false;
                string avisoHistorico = string.Empty;
                if (!string.IsNullOrWhiteSpace(observacaoHistorico))
                {
                    try
                    {
                        historicoGravado = new cls_IA_Repositorio().AdicionarObservacaoPedido(idOrcamento, observacaoHistorico);
                        if (!historicoGravado)
                        {
                            avisoHistorico = "Cabeçalho criado, mas não foi possível gravar a observação técnica no histórico.";
                        }
                    }
                    catch (Exception exHistorico)
                    {
                        avisoHistorico = "Cabeçalho criado, mas não foi possível gravar a observação técnica no histórico: " + exHistorico.Message;
                    }
                }

                saida = new JObject
                {
                    { "executado", true },
                    { "idOrcamento", idOrcamento },
                    { "idPedido", idOrcamento },
                    { "nNumeroOrcamento", nNumeroOrcamento },
                    { "referencia", Texto(normalizados, "referencia", 60) },
                    { "link", LinkOrcamento(idOrcamento) },
                    { "pdfLink", LinkPdfOrcamento(idOrcamento) },
                    { "observacaoUsuario", Texto(normalizados, "observacao", 1000) },
                    { "observacaoHistorico", observacaoHistorico },
                    { "observacaoHistoricoGravada", historicoGravado },
                    { "observacao", "Cabeçalho criado. Itens, serviços e escopos ainda devem ser adicionados por etapas próprias." }
                };

                if (!string.IsNullOrWhiteSpace(avisoHistorico))
                {
                    ((JObject)saida)["avisoHistorico"] = avisoHistorico;
                }

                return true;
            }
            catch (Exception ex)
            {
                erro = "Erro ao criar cabeçalho do orçamento: " + ex.Message;
                return false;
            }
        }

        private static string PrepararOrcamentoDuplicar(IAFerramentaDefinicao ferramenta, JObject argumentos, IAFerramentaResultado resultado)
        {
            JObject normalizados;
            string resumo;
            string erro;
            if (!ValidarOrcamentoDuplicar(argumentos, out normalizados, out resumo, out erro))
            {
                resultado.Erro = erro;
                return JsonErro(erro);
            }

            resultado.ArgumentosJson = normalizados.ToString(Formatting.None);
            resultado.Status = "PENDENTE_CONFIRMACAO";
            resultado.ResumoAcao = resumo;
            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "acaoPendente", true },
                { "resumo", resumo },
                { "dadosNormalizados", normalizados },
                { "instrucao", "A duplicação ficou pendente de confirmação; não afirme que o novo orçamento já foi criado." }
            }.ToString(Formatting.None);
        }

        private static bool ValidarOrcamentoDuplicar(JObject argumentos, out JObject normalizados, out string resumo, out string erro)
        {
            normalizados = new JObject();
            resumo = string.Empty;
            erro = string.Empty;

            int idOrcamento = Inteiro(argumentos, "idOrcamento", 0);
            int numero = Inteiro(argumentos, "nNumeroOrcamento", 0);
            if (idOrcamento <= 0 && numero <= 0)
            {
                erro = "Informe idOrcamento ou nNumeroOrcamento para duplicar.";
                return false;
            }

            string dataTexto = Texto(argumentos, "dataEstimativaEntrega", 10);
            DateTime data;
            if (!string.IsNullOrWhiteSpace(dataTexto) && !DateTime.TryParse(dataTexto, out data))
            {
                erro = "dataEstimativaEntrega inválida. Use dd/MM/yyyy ou string vazia.";
                return false;
            }

            string chave = Texto(argumentos, "chaveIdempotencia", 80);
            if (string.IsNullOrWhiteSpace(chave)) chave = Guid.NewGuid().ToString("N");

            normalizados["idOrcamento"] = idOrcamento;
            normalizados["nNumeroOrcamento"] = numero;
            normalizados["referencia"] = Texto(argumentos, "referencia", 60);
            normalizados["observacao"] = Texto(argumentos, "observacao", 1000);
            normalizados["dataEstimativaEntrega"] = dataTexto;
            normalizados["chaveIdempotencia"] = chave;

            string origem = idOrcamento > 0 ? "ID " + idOrcamento : "número " + numero;
            resumo = "Duplicar orçamento de origem " + origem + ". A operação criará um novo número e copiará os itens sem vínculos de CRM, pedido ou cotação.";
            return true;
        }

        private static bool ExecutarOrcamentoDuplicarConfirmado(JObject argumentos, out string erro, out JToken saida)
        {
            erro = string.Empty;
            saida = new JObject { { "executado", true } };
            if (!FUNCOES.ValidaPermissao(Permissao.Comercial.Orcamento.Incluir, false))
            {
                erro = "Usuário sem permissão para incluir orçamento.";
                return false;
            }

            JObject normalizados;
            string resumo;
            if (!ValidarOrcamentoDuplicar(argumentos, out normalizados, out resumo, out erro)) return false;

            try
            {
                int idUsuario = Inteiro(IDENTITY.Variaveis.idUsuario(), 0);
                cls_Comercial_OrcamentoDuplicacaoResultado duplicacao = cls_Comercial_OrcamentoOperacoes.Duplicar(
                    Inteiro(normalizados, "idOrcamento", 0),
                    Inteiro(normalizados, "nNumeroOrcamento", 0),
                    Texto(normalizados, "referencia", 60),
                    Texto(normalizados, "observacao", 1000),
                    Texto(normalizados, "dataEstimativaEntrega", 10),
                    Texto(normalizados, "chaveIdempotencia", 80),
                    idUsuario);

                if (!duplicacao.Sucesso)
                {
                    erro = duplicacao.Erro;
                    return false;
                }

                saida = new JObject
                {
                    { "executado", true },
                    { "idOrcamento", duplicacao.IdOrcamento },
                    { "idPedido", duplicacao.IdOrcamento },
                    { "nNumeroOrcamento", duplicacao.NumeroOrcamento },
                    { "referencia", duplicacao.Referencia },
                    { "chaveIdempotencia", Texto(normalizados, "chaveIdempotencia", 80) },
                    { "idempotente", duplicacao.Idempotente },
                    { "link", LinkOrcamento(duplicacao.IdOrcamento) },
                    { "pdfLink", LinkPdfOrcamento(duplicacao.IdOrcamento) },
                    { "mensagem", duplicacao.Idempotente ? "A duplicação já havia sido concluída; o orçamento existente foi retornado." : "Orçamento duplicado com sucesso." }
                };
                return true;
            }
            catch (Exception ex)
            {
                erro = "Erro ao duplicar orçamento: " + ex.Message;
                return false;
            }
        }

        private static bool ValidarOrcamentoCriarCabecalho(JObject argumentos, out JObject normalizados, out string resumo, out string erro)
        {
            normalizados = new JObject();
            resumo = string.Empty;
            erro = string.Empty;

            if (argumentos == null)
            {
                argumentos = new JObject();
            }

            int idCliente = Inteiro(argumentos, "idCliente", 0);
            int idTipoOrcamento = Inteiro(argumentos, "idTipoOrcamento", 0);
            int idFluxo = Inteiro(argumentos, "idFluxo", 0);
            int idVendedor = Inteiro(argumentos, "idVendedor", 0);
            int idCondicaoPagamento = Inteiro(argumentos, "idCondicaoPagamento", 0);
            int idEmpresa = Inteiro(argumentos, "idEmpresa", 0);
            int idTabelaPreco = Inteiro(argumentos, "idTabelaPreco", 0);
            int idEnderecoFiscal = Inteiro(argumentos, "idEnderecoFiscal", 0);
            int idEnderecoEntrega = Inteiro(argumentos, "idEnderecoEntrega", 0);
            int idContatoCliente = Inteiro(argumentos, "idContatoCliente", 0);
            int idTipoEnvio = Inteiro(argumentos, "idTipoEnvio", 0);
            int idTipoCliente = Inteiro(argumentos, "idTipoCliente", 0);
            int idCidadeEntrega = Inteiro(argumentos, "idCidadeEntrega", 0);
            int idInstalador = Inteiro(argumentos, "idInstalador", 0);
            int validadeDias = Inteiro(argumentos, "validadeDias", 0);
            int diasPrevisao = Inteiro(argumentos, "diasPrevisao", 0);

            string referencia = Texto(argumentos, "referencia", 60);
            string dataEstimativaEntrega = DataObrigatoriaOrcamento(Texto(argumentos, "dataEstimativaEntrega", 30));
            string destinoVenda = Texto(argumentos, "destinoVenda", 1).ToUpperInvariant();
            string observacao = Texto(argumentos, "observacao", 1000);
            string observacaoHistorico = Texto(argumentos, "observacaoHistorico", 500);
            string confidencial = NormalizarSimNao(Texto(argumentos, "confidencial", 10));
            string empresaTransporte = Texto(argumentos, "empresaTransporte", 200);
            decimal fretePrevisto = DecimalValor(argumentos, "fretePrevisto", 0m);
            string ufFiscal = Texto(argumentos, "ufFiscal", 2).ToUpperInvariant();
            string ufEntrega = Texto(argumentos, "ufEntrega", 2).ToUpperInvariant();
            string sidSegmentosCliente = NormalizarListaIdsPipe(Texto(argumentos, "sidSegmentosCliente", 200));
            string sidTiposServicos = NormalizarListaIdsPipe(Texto(argumentos, "sidTiposServicos", 200));
            string sidEscopos = NormalizarListaIdsPipe(Texto(argumentos, "sidEscopos", 200));

            List<string> erros = new List<string>();
            if (idCliente <= 0) erros.Add("Informe um cliente válido.");
            if (idTipoOrcamento <= 0) erros.Add("Informe o tipo de orçamento.");
            if (idFluxo <= 0) erros.Add("Informe o fluxo do orçamento.");
            if (idVendedor <= 0) erros.Add("Informe o vendedor.");
            if (idCondicaoPagamento <= 0) erros.Add("Informe a condição de pagamento.");
            if (idEmpresa <= 0) erros.Add("Informe a empresa do orçamento.");
            if (idTabelaPreco <= 0) erros.Add("Informe a tabela de preço.");
            if (idEnderecoFiscal <= 0) erros.Add("Informe o endereço fiscal do cliente.");
            if (idEnderecoEntrega == 0 || idEnderecoEntrega < -1) erros.Add("Informe o endereço de entrega ou use -1 para Coleta.");
            if (idContatoCliente <= 0) erros.Add("Informe o contato do cliente.");
            if (idTipoEnvio <= 0) erros.Add("Informe a forma de envio.");
            if (referencia.Length < 3) erros.Add("Informe uma referência com ao menos 3 caracteres.");
            if (string.IsNullOrWhiteSpace(dataEstimativaEntrega)) erros.Add("Informe a data estimada de entrega em dd/MM/yyyy.");
            if (!Regex.IsMatch(destinoVenda, "^[CRI]$")) erros.Add("Destino de venda inválido. Use C, R ou I.");
            if (validadeDias <= 0) erros.Add("Informe a validade do orçamento em dias.");
            if (!UfValidaOuVazia(ufFiscal)) erros.Add("UF fiscal inválida.");
            if (!UfValidaOuVazia(ufEntrega)) erros.Add("UF de entrega inválida.");

            if (erros.Count > 0)
            {
                erro = string.Join(" ", erros.ToArray());
                return false;
            }

            try
            {
                DataSet dsCliente = ConsultarClienteOrcamento(idCliente);
                DataTable tbCliente = Tabela(dsCliente, 0);
                if (tbCliente == null || tbCliente.Rows.Count == 0)
                {
                    erro = "Cliente não encontrado para o orçamento.";
                    return false;
                }

                if (!ExisteId(Tabela(dsCliente, 1), "idContato", idContatoCliente))
                {
                    erro = "Contato do cliente não encontrado para o cliente informado.";
                    return false;
                }

                if (!ExisteId(Tabela(dsCliente, 2), "idEndereco", idEnderecoFiscal))
                {
                    erro = "Endereço fiscal não encontrado para o cliente informado.";
                    return false;
                }

                if (idEnderecoEntrega != -1 && !ExisteId(Tabela(dsCliente, 2), "idEndereco", idEnderecoEntrega))
                {
                    erro = "Endereço de entrega não encontrado para o cliente informado.";
                    return false;
                }

                DataSet dsTipo = ConsultarTipoOrcamentoDetalhe(idTipoOrcamento);
                DataTable tbTipo = Tabela(dsTipo, 0);
                if (tbTipo == null || tbTipo.Rows.Count == 0)
                {
                    erro = "Tipo de orçamento não encontrado.";
                    return false;
                }

                DataTable tbFluxos = Tabela(dsTipo, 1);
                if (tbFluxos != null && tbFluxos.Rows.Count > 0 && !ExisteId(tbFluxos, "idFluxo", idFluxo))
                {
                    erro = "Fluxo não vinculado ao tipo de orçamento informado.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(sidTiposServicos))
                {
                    sidTiposServicos = JuntarIds(Tabela(dsTipo, 2), "idTipoProduto");
                }

                if (string.IsNullOrWhiteSpace(sidEscopos))
                {
                    sidEscopos = JuntarIds(Tabela(dsTipo, 3), "idEscopo");
                }

                DataSet dsEmpresa = ConsultarEmpresaOrcamento(0, idEmpresa);
                DataTable tbEmpresa = Tabela(dsEmpresa, 0);
                if (tbEmpresa == null || tbEmpresa.Rows.Count == 0)
                {
                    dsEmpresa = ConsultarEmpresaOrcamento(idEmpresa, 0);
                    tbEmpresa = Tabela(dsEmpresa, 0);
                }
                if (tbEmpresa == null || tbEmpresa.Rows.Count == 0)
                {
                    erro = "Empresa não encontrada para o orçamento.";
                    return false;
                }

                DataSet dsTabela = ConsultarTabelaPrecoOrcamento(idTabelaPreco);
                if (Tabela(dsTabela, 0) == null || Tabela(dsTabela, 0).Rows.Count == 0)
                {
                    erro = "Tabela de preço não encontrada.";
                    return false;
                }

                DataSet dsCondicao = ConsultarCondicaoPagamentoOrcamento(idCondicaoPagamento);
                if (Tabela(dsCondicao, 0) == null || Tabela(dsCondicao, 0).Rows.Count == 0)
                {
                    erro = "Condição de pagamento não encontrada.";
                    return false;
                }

                string clienteNome = Valor(tbCliente.Rows[0], "sRazaoSocial");
                if (string.IsNullOrWhiteSpace(clienteNome))
                {
                    clienteNome = Valor(tbCliente.Rows[0], "sNomeFantasia");
                }
                string tipoNome = Valor(tbTipo.Rows[0], "sDscTipoOrcamento");

                normalizados = new JObject
                {
                    { "idCliente", idCliente },
                    { "idTipoOrcamento", idTipoOrcamento },
                    { "idFluxo", idFluxo },
                    { "idVendedor", idVendedor },
                    { "idCondicaoPagamento", idCondicaoPagamento },
                    { "idEmpresa", idEmpresa },
                    { "idTabelaPreco", idTabelaPreco },
                    { "idEnderecoFiscal", idEnderecoFiscal },
                    { "idEnderecoEntrega", idEnderecoEntrega },
                    { "idContatoCliente", idContatoCliente },
                    { "idTipoEnvio", idTipoEnvio },
                    { "idTipoCliente", idTipoCliente },
                    { "idCidadeEntrega", idCidadeEntrega },
                    { "idInstalador", idInstalador },
                    { "referencia", referencia },
                    { "dataEstimativaEntrega", dataEstimativaEntrega },
                    { "destinoVenda", destinoVenda },
                    { "validadeDias", validadeDias },
                    { "observacao", observacao },
                    { "observacaoHistorico", observacaoHistorico },
                    { "confidencial", confidencial },
                    { "empresaTransporte", empresaTransporte },
                    { "fretePrevisto", fretePrevisto },
                    { "diasPrevisao", diasPrevisao },
                    { "ufFiscal", ufFiscal },
                    { "ufEntrega", ufEntrega },
                    { "sidSegmentosCliente", sidSegmentosCliente },
                    { "sidTiposServicos", sidTiposServicos },
                    { "sidEscopos", sidEscopos }
                };
                resumo = "Criar cabeçalho de orçamento para " + clienteNome + " (tipo: " + tipoNome + ", referência: " + referencia + ")";
                return true;
            }
            catch (Exception ex)
            {
                erro = "Erro ao validar dados do orçamento: " + ex.Message;
                return false;
            }
        }

        private static Dictionary<string, string> MontarParametrosOrcamentoCriarCabecalho(JObject args)
        {
            return new Dictionary<string, string>
            {
                { "@sFuncao", "INCLUIR PEDIDO" },
                { "@idPedido", "0" },
                { "@idTipo", "1" },
                { "@idTipoOrcamento", Inteiro(args, "idTipoOrcamento", 0).ToString() },
                { "@idCliente", Inteiro(args, "idCliente", 0).ToString() },
                { "@idFluxo", Inteiro(args, "idFluxo", 0).ToString() },
                { "@idVendedor", Inteiro(args, "idVendedor", 0).ToString() },
                { "@idCondicaoDePagamento", Inteiro(args, "idCondicaoPagamento", 0).ToString() },
                { "@idEmpresa", Inteiro(args, "idEmpresa", 0).ToString() },
                { "@idEnderecoEntrega", Inteiro(args, "idEnderecoFiscal", 0).ToString() },
                { "@nControleTT", string.Empty },
                { "@sReferencia", Texto(args, "referencia", 60) },
                { "@dtPedido", DateTime.Today.ToString("dd/MM/yyyy") },
                { "@dtEstimativaEntrega", Texto(args, "dataEstimativaEntrega", 10) },
                { "@sObservacao", Texto(args, "observacao", 1000) },
                { "@idUsuarioInclusao", IDENTITY.Variaveis.idUsuario().Trim() },
                { "@idTipoEnvio", Inteiro(args, "idTipoEnvio", 0).ToString() },
                { "@nVlrProdutos", "0" },
                { "@nVlrServicos", "0" },
                { "@sConfidencial", Texto(args, "confidencial", 1) },
                { "@sEmpresaTransporte", Texto(args, "empresaTransporte", 200) },
                { "@nFretePrevisto", DecimalSql(DecimalValor(args, "fretePrevisto", 0m), 2) },
                { "@idTabelaPreco", Inteiro(args, "idTabelaPreco", 0).ToString() },
                { "@sAlteracaoTabelaPreco_Obs", string.Empty },
                { "@idVersao", "0" },
                { "@sDestinoVenda", Texto(args, "destinoVenda", 1) },
                { "@idEnderecoDestino", Inteiro(args, "idEnderecoEntrega", 0).ToString() },
                { "@sUF_Entrega", Texto(args, "ufEntrega", 2).ToUpperInvariant() },
                { "@nDiasPrevisao", Inteiro(args, "diasPrevisao", 0).ToString() },
                { "@sUF_Fiscal", Texto(args, "ufFiscal", 2).ToUpperInvariant() },
                { "@idCidade_Entrega", Inteiro(args, "idCidadeEntrega", 0).ToString() },
                { "@nValidadeOrcamento", Inteiro(args, "validadeDias", 0).ToString() },
                { "@idContato_Cliente", Inteiro(args, "idContatoCliente", 0).ToString() },
                { "@idTipoCliente", Inteiro(args, "idTipoCliente", 0).ToString() },
                { "@sidSegmentosCliente", Texto(args, "sidSegmentosCliente", 200) },
                { "@sidTiposServicos", Texto(args, "sidTiposServicos", 200) },
                { "@sidEscopos", Texto(args, "sidEscopos", 200) },
                { "@sAlteracaoItens", "N" },
                { "@sComparativos", "N" },
                { "@sEmpreitada", "N" },
                { "@sDuplicado", "N" },
                { "@idInstalador", Inteiro(args, "idInstalador", 0).ToString() }
            };
        }

        private static string PrepararOrcamentoAdicionarItem(IAFerramentaDefinicao ferramenta, JObject argumentos, IAFerramentaResultado resultado)
        {
            JObject normalizados;
            string resumo;
            string erro;
            if (!ValidarOrcamentoAdicionarItem(argumentos, out normalizados, out resumo, out erro))
            {
                resultado.Erro = erro;
                return JsonErro(erro);
            }

            resultado.ArgumentosJson = normalizados.ToString(Formatting.None);
            resultado.Status = "PENDENTE_CONFIRMACAO";
            resultado.ResumoAcao = resumo;

            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "acaoPendente", true },
                { "resumo", resumo },
                { "dadosNormalizados", normalizados },
                { "instrucao", "A ação ficou pendente de confirmação; não afirme que o item já foi adicionado ao orçamento." }
            }.ToString(Formatting.None);
        }

        private static bool ExecutarOrcamentoAdicionarItemConfirmado(JObject argumentos, out string erro, out JToken saida)
        {
            erro = string.Empty;
            saida = new JObject { { "executado", true } };

            if (!FUNCOES.ValidaPermissao(Permissao.Comercial.Orcamento.Alterar, false))
            {
                erro = "Usuário sem permissão para alterar orçamento.";
                return false;
            }

            JObject normalizados;
            string resumo;
            if (!ValidarOrcamentoAdicionarItem(argumentos, out normalizados, out resumo, out erro))
            {
                return false;
            }

            try
            {
                int idOrcamento = Inteiro(normalizados, "idOrcamento", 0);
                int idProduto = Inteiro(normalizados, "idProduto", 0);

                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", MontarParametrosOrcamentoAdicionarItem(normalizados));
                string erroBanco = ErroRetornoDataSetSeHouver(ds);
                if (!string.IsNullOrWhiteSpace(erroBanco))
                {
                    erro = "Não foi possível adicionar o item ao orçamento: " + erroBanco;
                    return false;
                }

                int idItemOrcamento = 0;
                DataTable tbRetorno = Tabela(ds, 0);
                if (tbRetorno != null && tbRetorno.Rows.Count > 0)
                {
                    idItemOrcamento = Inteiro(Valor(tbRetorno.Rows[0], "idRegistro"), 0);
                }

                JObject totais;
                string avisoTotais = string.Empty;
                DataSet dsAtualizado;
                if (!RecalcularTotaisOrcamento(idOrcamento, out totais, out avisoTotais, out dsAtualizado))
                {
                    totais = new JObject();
                }

                DataRow rowItem = LocalizarItemOrcamento(Tabela(dsAtualizado, 1), idProduto, Inteiro(normalizados, "idProdutoPai", 0), Inteiro(normalizados, "idProdutoAvo", 0), Inteiro(normalizados, "idProdutoBisavo", 0));
                if (idItemOrcamento <= 0 && rowItem != null)
                {
                    idItemOrcamento = Inteiro(Valor(rowItem, "idItem"), 0);
                }

                JObject retorno = new JObject
                {
                    { "executado", true },
                    { "idOrcamento", idOrcamento },
                    { "idPedido", idOrcamento },
                    { "idItemOrcamento", idItemOrcamento },
                    { "idProduto", idProduto },
                    { "codigoProduto", Texto(normalizados, "codigoProduto", 50) },
                    { "descricao", Texto(normalizados, "descricao", 200) },
                    { "tipoItem", Texto(normalizados, "tipoItem", 30) },
                    { "quantidade", DecimalValor(normalizados, "quantidade", 0m) },
                    { "valorUnitario", DecimalValor(normalizados, "valorUnitario", 0m) },
                    { "valorTotal", DecimalValor(normalizados, "valorTotal", 0m) },
                    { "origemPreco", Texto(normalizados, "origemPreco", 40) },
                    { "totais", totais },
                    { "link", LinkOrcamento(idOrcamento) },
                    { "pdfLink", LinkPdfOrcamento(idOrcamento) }
                };

                if (!string.IsNullOrWhiteSpace(avisoTotais))
                {
                    retorno["aviso"] = avisoTotais;
                }

                saida = retorno;
                return true;
            }
            catch (Exception ex)
            {
                erro = "Erro ao adicionar item ao orçamento: " + ex.Message;
                return false;
            }
        }

        private static bool ValidarOrcamentoAdicionarItem(JObject argumentos, out JObject normalizados, out string resumo, out string erro)
        {
            normalizados = new JObject();
            resumo = string.Empty;
            erro = string.Empty;

            if (argumentos == null)
            {
                argumentos = new JObject();
            }

            int idOrcamento = Inteiro(argumentos, "idOrcamento", 0);
            int idProduto = Inteiro(argumentos, "idProduto", 0);
            string codigoProduto = Texto(argumentos, "codigoProduto", 50);
            string descricao = Texto(argumentos, "descricao", 200);
            string unidade = Texto(argumentos, "unidade", 3).ToUpperInvariant();
            decimal quantidade = DecimalValor(argumentos, "quantidade", 0m);
            decimal valorUnitario = DecimalValor(argumentos, "valorUnitario", 0m);
            decimal descontoPercentual = DecimalValor(argumentos, "descontoPercentual", 0m);
            decimal margemPercentual = DecimalValor(argumentos, "margemPercentual", 0m);
            decimal ajustePercentual = DecimalValor(argumentos, "ajustePercentual", 0m);
            decimal valorTotal = DecimalValor(argumentos, "valorTotal", 0m);
            int ordem = Inteiro(argumentos, "ordem", 0);
            int diasPrevisaoEntrega = Inteiro(argumentos, "diasPrevisaoEntrega", 0);
            int idProdutoPai = Inteiro(argumentos, "idProdutoPai", 0);
            int idProdutoAvo = Inteiro(argumentos, "idProdutoAvo", 0);
            int idProdutoBisavo = Inteiro(argumentos, "idProdutoBisavo", 0);
            string tipoItem = Texto(argumentos, "tipoItem", 30).ToLowerInvariant();

            List<string> erros = new List<string>();
            if (idOrcamento <= 0) erros.Add("Informe um orçamento válido.");
            if (idProduto <= 0) erros.Add("Informe um produto, serviço ou recurso válido.");
            if (quantidade <= 0) erros.Add("Informe uma quantidade maior que zero.");
            if (valorUnitario < 0) erros.Add("O valor unitário não pode ser negativo.");
            if (descontoPercentual < 0 || descontoPercentual > 100) erros.Add("O desconto deve ficar entre 0 e 100%.");
            if (margemPercentual < 0) erros.Add("A margem não pode ser negativa.");
            if (ajustePercentual < 0) erros.Add("O ajuste não pode ser negativo.");
            if (idProdutoPai > 0 || idProdutoAvo > 0 || idProdutoBisavo > 0) erros.Add("Composição de itens ainda não é gravada por esta ferramenta; use 0 nos campos de item pai/avô/bisavô.");
            if (tipoItem != "produto" && tipoItem != "servico_recurso") erros.Add("tipoItem inválido. Use produto ou servico_recurso.");

            if (erros.Count > 0)
            {
                erro = string.Join(" ", erros.ToArray());
                return false;
            }

            try
            {
                DataSet dsOrcamento;
                DataRow cabecalho;
                if (!ObterOrcamentoEditavel(idOrcamento, out dsOrcamento, out cabecalho, out erro))
                {
                    return false;
                }

                DataSet dsProduto = ConsultarProdutoOrcamento(idProduto);
                DataTable tbProduto = Tabela(dsProduto, 0);
                if (tbProduto == null || tbProduto.Rows.Count == 0)
                {
                    erro = "Produto, serviço ou recurso não encontrado.";
                    return false;
                }

                DataRow produto = tbProduto.Rows[0];
                if (!string.Equals(Valor(produto, "sSituacao"), "S", StringComparison.OrdinalIgnoreCase))
                {
                    erro = "O item informado não está ativo no cadastro de produtos.";
                    return false;
                }

                if (string.Equals(Valor(produto, "sExibeComercial"), "N", StringComparison.OrdinalIgnoreCase))
                {
                    erro = "O item informado está bloqueado para exibição comercial.";
                    return false;
                }

                string codigoCadastro = Valor(produto, "sCodigo");
                if (!string.IsNullOrWhiteSpace(codigoProduto) && !string.Equals(codigoProduto, codigoCadastro, StringComparison.OrdinalIgnoreCase))
                {
                    erro = "O código informado não pertence ao produto selecionado. Código esperado: " + codigoCadastro + ".";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(codigoProduto))
                {
                    codigoProduto = codigoCadastro;
                }
                if (string.IsNullOrWhiteSpace(descricao))
                {
                    descricao = Valor(produto, "sDscProduto");
                }
                if (string.IsNullOrWhiteSpace(unidade))
                {
                    unidade = Valor(produto, "sUnidade").ToUpperInvariant();
                }

                int idTipoProdutoOrcamento = Inteiro(Valor(produto, "idTipo"), 0);
                if (!ProdutoCompativelTipoItem(tipoItem, idTipoProdutoOrcamento))
                {
                    erro = tipoItem == "produto"
                        ? "O item selecionado é serviço/recurso; use tipoItem servico_recurso."
                        : "O item selecionado é produto; use tipoItem produto.";
                    return false;
                }

                string origemPreco = Texto(argumentos, "origemPreco", 40);
                int idTabelaPreco = Inteiro(Valor(cabecalho, "idTabelaPreco"), Inteiro(argumentos, "idTabelaPreco", 0));
                if (valorUnitario <= 0m || valorTotal <= 0m)
                {
                    if (valorUnitario > 0m && valorTotal <= 0m)
                    {
                        decimal fatorDesconto = descontoPercentual > 0 ? (1m - (descontoPercentual / 100m)) : 1m;
                        valorTotal = quantidade * valorUnitario * fatorDesconto;
                        origemPreco = string.IsNullOrWhiteSpace(origemPreco) ? "informado" : origemPreco;
                    }
                    else if (valorTotal > 0m && valorUnitario <= 0m)
                    {
                        valorUnitario = valorTotal / quantidade;
                        origemPreco = string.IsNullOrWhiteSpace(origemPreco) ? "informado" : origemPreco;
                    }
                    else
                    {
                        JObject precoResolvido;
                        string erroPreco;
                        if (!ResolverPrecoProdutoOrcamento(idProduto, idTabelaPreco, idOrcamento, quantidade, descontoPercentual, tipoItem, out precoResolvido, out erroPreco))
                        {
                            erro = "Não foi possível resolver preço do item: " + erroPreco;
                            return false;
                        }

                        valorUnitario = DecimalValor(precoResolvido, "valorUnitario", 0m);
                        valorTotal = DecimalValor(precoResolvido, "valorTotal", 0m);
                        origemPreco = Texto(precoResolvido, "origemPreco", 40);
                        codigoProduto = PrimeiroTextoNaoVazio(Texto(precoResolvido, "codigoProduto", 50), codigoProduto);
                        descricao = PrimeiroTextoNaoVazio(Texto(precoResolvido, "descricao", 200), descricao);
                        unidade = PrimeiroTextoNaoVazio(Texto(precoResolvido, "unidade", 3), unidade).ToUpperInvariant();
                    }
                }

                if (valorTotal <= 0m)
                {
                    erro = "Não foi possível calcular o valor total do item. Verifique quantidade, tabela de preço e cadastro do produto.";
                    return false;
                }

                if (ordem <= 0)
                {
                    ordem = ProximaOrdemItem(Tabela(dsOrcamento, 1));
                }

                string dataPrevisaoEntrega = DataPrevisaoItem(cabecalho, diasPrevisaoEntrega);
                string referencia = Valor(cabecalho, "sReferenciaCompleta");
                if (string.IsNullOrWhiteSpace(referencia))
                {
                    referencia = Valor(cabecalho, "sReferencia");
                }

                normalizados = new JObject
                {
                    { "idOrcamento", idOrcamento },
                    { "idProduto", idProduto },
                    { "codigoProduto", codigoProduto },
                    { "descricao", descricao },
                    { "unidade", unidade },
                    { "quantidade", quantidade },
                    { "valorUnitario", valorUnitario },
                    { "descontoPercentual", descontoPercentual },
                    { "margemPercentual", margemPercentual },
                    { "ajustePercentual", ajustePercentual },
                    { "valorTotal", valorTotal },
                    { "origemPreco", origemPreco },
                    { "idTabelaPreco", idTabelaPreco },
                    { "ordem", ordem },
                    { "diasPrevisaoEntrega", diasPrevisaoEntrega },
                    { "dataPrevisaoEntrega", dataPrevisaoEntrega },
                    { "idProdutoPai", 0 },
                    { "idProdutoAvo", 0 },
                    { "idProdutoBisavo", 0 },
                    { "tipoItem", tipoItem }
                };

                resumo = "Adicionar " + (tipoItem == "produto" ? "produto" : "serviço/recurso") + " ao orçamento " + idOrcamento + " (" + referencia + "): " + codigoProduto + " - " + descricao + ", quantidade " + DecimalSql(quantidade, 4) + ", total " + DecimalSql(valorTotal, 2);
                return true;
            }
            catch (Exception ex)
            {
                erro = "Erro ao validar item do orçamento: " + ex.Message;
                return false;
            }
        }

        private static Dictionary<string, string> MontarParametrosOrcamentoAdicionarItem(JObject args)
        {
            decimal valorUnitario = DecimalValor(args, "valorUnitario", 0m);
            decimal valorTotal = DecimalValor(args, "valorTotal", 0m);

            return new Dictionary<string, string>
            {
                { "@sFuncao", "INCLUIR ITEM" },
                { "@idPedido", Inteiro(args, "idOrcamento", 0).ToString() },
                { "@idItem", "0" },
                { "@idProduto", Inteiro(args, "idProduto", 0).ToString() },
                { "@nOrdem", Inteiro(args, "ordem", 0).ToString() },
                { "@sCodigo", Texto(args, "codigoProduto", 50) },
                { "@sDscProduto", Texto(args, "descricao", 200) },
                { "@sUnidade", Texto(args, "unidade", 3) },
                { "@nQuantidade", DecimalSql(DecimalValor(args, "quantidade", 0m), 4) },
                { "@nValorUnitario", DecimalSql(valorUnitario, 4) },
                { "@nDesconto", DecimalSql(DecimalValor(args, "descontoPercentual", 0m), 4) },
                { "@nMargem", DecimalSql(DecimalValor(args, "margemPercentual", 0m), 4) },
                { "@nAjuste", DecimalSql(DecimalValor(args, "ajustePercentual", 0m), 4) },
                { "@nValorTotal", DecimalSql(valorTotal, 4) },
                { "@idUsuarioInclusao", IDENTITY.Variaveis.idUsuario().Trim() },
                { "@dtPrevisaoEntrega", Texto(args, "dataPrevisaoEntrega", 10) },
                { "@idProdutoPai", Inteiro(args, "idProdutoPai", 0).ToString() },
                { "@idProdutoAvo", Inteiro(args, "idProdutoAvo", 0).ToString() },
                { "@idProdutoBisavo", Inteiro(args, "idProdutoBisavo", 0).ToString() },
                { "@nValorReal", DecimalSql(valorUnitario, 4) },
                { "@nValorRealTotal", DecimalSql(valorTotal, 4) },
                { "@nResultado", "0" },
                { "@nTotalComposicao", "0" },
                { "@nValorTblPreco", DecimalSql(valorUnitario, 2) },
                { "@sUnidadeEntrega", Texto(args, "unidade", 3) }
            };
        }

        private static string PrepararOrcamentoSalvarRespostaEscopo(IAFerramentaDefinicao ferramenta, JObject argumentos, IAFerramentaResultado resultado)
        {
            JObject normalizados;
            string resumo;
            string erro;
            if (!ValidarOrcamentoSalvarRespostaEscopo(argumentos, out normalizados, out resumo, out erro))
            {
                resultado.Erro = erro;
                return JsonErro(erro);
            }

            resultado.ArgumentosJson = normalizados.ToString(Formatting.None);
            resultado.Status = "PENDENTE_CONFIRMACAO";
            resultado.ResumoAcao = resumo;

            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "acaoPendente", true },
                { "resumo", resumo },
                { "dadosNormalizados", normalizados },
                { "instrucao", "A ação ficou pendente de confirmação; não afirme que a resposta de escopo já foi salva." }
            }.ToString(Formatting.None);
        }

        private static bool ExecutarOrcamentoSalvarRespostaEscopoConfirmado(JObject argumentos, out string erro, out JToken saida)
        {
            erro = string.Empty;
            saida = new JObject { { "executado", true } };

            if (!FUNCOES.ValidaPermissao(Permissao.Comercial.Orcamento.Alterar, false))
            {
                erro = "Usuário sem permissão para alterar orçamento.";
                return false;
            }

            JObject normalizados;
            string resumo;
            if (!ValidarOrcamentoSalvarRespostaEscopo(argumentos, out normalizados, out resumo, out erro))
            {
                return false;
            }

            try
            {
                BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_Orcamento_Tipo", MontarParametrosOrcamentoRespostaEscopo(normalizados));

                int idOrcamento = Inteiro(normalizados, "idOrcamento", 0);
                DataRow resposta = LocalizarRespostaEscopo(idOrcamento, normalizados);
                int idRegistro = resposta != null ? Inteiro(Valor(resposta, "idRegistro"), 0) : 0;

                saida = new JObject
                {
                    { "executado", true },
                    { "idOrcamento", idOrcamento },
                    { "idPedido", idOrcamento },
                    { "idRegistro", idRegistro },
                    { "idEscopo", Inteiro(normalizados, "idEscopo", 0) },
                    { "idCategoria", Inteiro(normalizados, "idCategoria", 0) },
                    { "idPergunta", Inteiro(normalizados, "idPergunta", 0) },
                    { "idOpcao", Inteiro(normalizados, "idOpcao", 0) },
                    { "pergunta", Texto(normalizados, "pergunta", 500) },
                    { "opcao", Texto(normalizados, "opcao", 500) },
                    { "link", LinkOrcamento(idOrcamento) },
                    { "pdfLink", LinkPdfOrcamento(idOrcamento) }
                };

                return true;
            }
            catch (Exception ex)
            {
                erro = "Erro ao salvar resposta de escopo do orçamento: " + ex.Message;
                return false;
            }
        }

        private static bool ValidarOrcamentoSalvarRespostaEscopo(JObject argumentos, out JObject normalizados, out string resumo, out string erro)
        {
            normalizados = new JObject();
            resumo = string.Empty;
            erro = string.Empty;

            if (argumentos == null)
            {
                argumentos = new JObject();
            }

            int idOrcamento = Inteiro(argumentos, "idOrcamento", 0);
            int idEscopo = Inteiro(argumentos, "idEscopo", 0);
            int idCategoria = Inteiro(argumentos, "idCategoria", 0);
            int idPergunta = Inteiro(argumentos, "idPergunta", 0);
            int idOpcao = Inteiro(argumentos, "idOpcao", 0);
            string pergunta = Texto(argumentos, "pergunta", 500);
            string opcao = Texto(argumentos, "opcao", 500);

            List<string> erros = new List<string>();
            if (idOrcamento <= 0) erros.Add("Informe um orçamento válido.");
            if (idEscopo <= 0) erros.Add("Informe o escopo.");
            if (idCategoria <= 0) erros.Add("Informe a categoria do escopo.");
            if (string.IsNullOrWhiteSpace(pergunta)) erros.Add("Informe a pergunta do escopo.");
            if (idOpcao <= 0) erros.Add("Informe a opção escolhida.");
            if (string.IsNullOrWhiteSpace(opcao)) erros.Add("Informe o texto da opção escolhida.");

            if (erros.Count > 0)
            {
                erro = string.Join(" ", erros.ToArray());
                return false;
            }

            try
            {
                DataSet dsOrcamento;
                DataRow cabecalho;
                if (!ObterOrcamentoEditavel(idOrcamento, out dsOrcamento, out cabecalho, out erro))
                {
                    return false;
                }

                int idTipoOrcamento = Inteiro(Valor(cabecalho, "idTipoOrcamento"), 0);
                string sidEscopos = Valor(cabecalho, "sidEscopos");
                DataSet dsEscopos = ConsultarEscoposOrcamento(idTipoOrcamento, idOrcamento, sidEscopos);
                DataTable tbPerguntas = Tabela(dsEscopos, 0);

                if (!ExisteCategoriaEscopo(tbPerguntas, idEscopo, idCategoria))
                {
                    erro = "Escopo ou categoria não encontrado no tipo de orçamento informado.";
                    return false;
                }

                normalizados = new JObject
                {
                    { "idOrcamento", idOrcamento },
                    { "idEscopo", idEscopo },
                    { "idCategoria", idCategoria },
                    { "pergunta", pergunta },
                    { "idPergunta", idPergunta },
                    { "idOpcao", idOpcao },
                    { "opcao", opcao }
                };

                resumo = "Salvar resposta de escopo no orçamento " + idOrcamento + ": " + pergunta + " = " + opcao;
                return true;
            }
            catch (Exception ex)
            {
                erro = "Erro ao validar resposta de escopo do orçamento: " + ex.Message;
                return false;
            }
        }

        private static Dictionary<string, string> MontarParametrosOrcamentoRespostaEscopo(JObject args)
        {
            return new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_MATRIZ_ESCOPO" },
                { "@idOrcamento", Inteiro(args, "idOrcamento", 0).ToString() },
                { "@idEscopo", Inteiro(args, "idEscopo", 0).ToString() },
                { "@idCategoria", Inteiro(args, "idCategoria", 0).ToString() },
                { "@idOpcao", Inteiro(args, "idOpcao", 0).ToString() },
                { "@sPergunta", Texto(args, "pergunta", 500) },
                { "@sOpcao", Texto(args, "opcao", 500) },
                { "@idPergunta", Inteiro(args, "idPergunta", 0).ToString() }
            };
        }

        private static string ExecutarCrmNegociosBuscar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 120);
            int idCliente = Inteiro(argumentos, "idCliente", 0);
            int idVendedor = Inteiro(argumentos, "idVendedor", 0);
            int idTipoCotacao = Inteiro(argumentos, "idTipoCotacao", 0);
            int idStatus = Inteiro(argumentos, "idStatus", 0);
            string confidencial = Texto(argumentos, "confidencial", 1).ToUpperInvariant();
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (confidencial != "S" && confidencial != "N")
            {
                confidencial = "T";
            }

            if (!FUNCOES.ValidaPermissao(Permissao.Comercial.CRM.Confidencial, false))
            {
                confidencial = "N";
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@idCliente", idCliente.ToString() },
                { "@idVendedor", idVendedor.ToString() },
                { "@idTipoCotacao", idTipoCotacao.ToString() },
                { "@idStatus", idStatus.ToString() },
                { "@sConfidencial", confidencial == "T" ? string.Empty : confidencial },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().Trim() }
            };

            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_CRM", parametros);
            DataTable tb = Tabela(ds, 0);
            if (!string.IsNullOrWhiteSpace(termo))
            {
                tb = FiltrarTabelaPorTermo(tb, termo, "idRegistroCRM", "sCliente", "sReferencia", "sVendedor", "sStatus", "nControle");
            }

            return TabelaParaJson(ferramenta, tb, limite, new[]
            {
                "idRegistroCRM",
                "dtInclusao",
                "sCliente",
                "nValor",
                "idStatus",
                "sStatus",
                "sStatusCancelamento",
                "sStatusExcluido",
                "sStatusFinalizado",
                "sCor",
                "sReferencia",
                "idVendedor",
                "sVendedor",
                "dtUltimoContato",
                "dtPrevisao",
                "sChance",
                "sConfidencial",
                "nMaterial",
                "nServico",
                "dtProximoContato",
                "idTipoCliente",
                "sidSegmentosCliente",
                "nControle"
            }, "/App/Paginas/Comercial/CRM.aspx?id={idRegistroCRM}");
        }

        private static string ExecutarCrmNegocioDetalhar(IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            int idRegistroCRM = Inteiro(argumentos, "idRegistroCRM", 0);
            int limite = Limite(argumentos, ferramenta.MaxRegistros);

            if (idRegistroCRM <= 0)
            {
                return JsonErro("idRegistroCRM inválido.");
            }

            DataSet ds = ConsultarCrmNegocio(idRegistroCRM);
            DataTable tbNegocio = Tabela(ds, 0);
            if (tbNegocio == null || tbNegocio.Rows.Count == 0)
            {
                return JsonErro("Negócio CRM não encontrado.");
            }

            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "negocio", TabelaParaArray(tbNegocio, 1, new[]
                    {
                        "idRegistroCRM",
                        "dtInclusao",
                        "idVendedor",
                        "idVendedor_Usuario",
                        "idTipoCotacao",
                        "idStatus",
                        "sTipo",
                        "idParceiro",
                        "sDscParceiro",
                        "sContato",
                        "sTelefoneContato",
                        "sEmail",
                        "nNumeroOrcamento",
                        "sReferencia",
                        "sObservacao",
                        "sDscStatus",
                        "sCor",
                        "dtFinalizacao",
                        "dtPrevisao",
                        "sChance",
                        "sConfidencial",
                        "nServico",
                        "nMaterial",
                        "idOrcamento",
                        "idTipoCliente",
                        "sidSegmentosCliente",
                        "dtProximoContato",
                        "nControle"
                    }, "/App/Paginas/Comercial/CRM.aspx?id={idRegistroCRM}") },
                { "historico", TabelaParaArray(Tabela(ds, 1), limite, new[] { "idRegistroCRM", "dtAcao", "sDscUsuario", "sAcao", "sObservacao" }, null) },
                { "followups", TabelaParaArray(Tabela(ds, 2), limite, new[] { "idRegistroCRM", "sDscMeioContato", "dtContato", "sContato", "dtProximoContato", "sObservacao" }, null) }
            }.ToString(Formatting.None);
        }

        private static string PrepararCrmNegocioCriarParaOrcamento(IAFerramentaDefinicao ferramenta, JObject argumentos, IAFerramentaResultado resultado)
        {
            JObject normalizados;
            string resumo;
            string erro;
            if (!ValidarCrmNegocioCriarParaOrcamento(argumentos, out normalizados, out resumo, out erro))
            {
                resultado.Erro = erro;
                return JsonErro(erro);
            }

            resultado.ArgumentosJson = normalizados.ToString(Formatting.None);
            resultado.Status = "PENDENTE_CONFIRMACAO";
            resultado.ResumoAcao = resumo;

            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "acaoPendente", true },
                { "resumo", resumo },
                { "dadosNormalizados", normalizados },
                { "instrucao", "A ação ficou pendente de confirmação; não afirme que o negócio CRM já foi criado." }
            }.ToString(Formatting.None);
        }

        private static bool ExecutarCrmNegocioCriarParaOrcamentoConfirmado(JObject argumentos, out string erro, out JToken saida)
        {
            erro = string.Empty;
            saida = new JObject { { "executado", true } };

            if (!FUNCOES.ValidaPermissao(Permissao.Comercial.CRM.Consultar, false))
            {
                erro = "Usuário sem permissão para consultar/criar CRM.";
                return false;
            }

            JObject normalizados;
            string resumo;
            if (!ValidarCrmNegocioCriarParaOrcamento(argumentos, out normalizados, out resumo, out erro))
            {
                return false;
            }

            try
            {
                DataSet dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_CRM", MontarParametrosCrmNegocioCriar(normalizados));
                string erroBanco = ErroRetornoDataSet(dsSalvar);
                if (!string.IsNullOrWhiteSpace(erroBanco))
                {
                    erro = "Não foi possível criar o negócio CRM: " + erroBanco;
                    return false;
                }

                DataTable tbRetorno = Tabela(dsSalvar, 0);
                int idRegistroCRM = tbRetorno != null && tbRetorno.Rows.Count > 0 ? Inteiro(Valor(tbRetorno.Rows[0], "idRegistroCRM"), 0) : 0;
                if (idRegistroCRM <= 0)
                {
                    erro = "A procedure de CRM não retornou o idRegistroCRM criado.";
                    return false;
                }

                int nNumeroOrcamento = Inteiro(normalizados, "nNumeroOrcamento", 0);
                Dictionary<string, string> parametrosVinculo = new Dictionary<string, string>
                {
                    { "@sFuncao", "VINCULAR_CRM" },
                    { "@idCRM", idRegistroCRM.ToString() },
                    { "@idPedido", nNumeroOrcamento.ToString() },
                    { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().Trim() }
                };

                DataSet dsVinculo = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", parametrosVinculo);
                erroBanco = ErroRetornoDataSetSeHouver(dsVinculo);
                if (!string.IsNullOrWhiteSpace(erroBanco))
                {
                    erro = "CRM criado, mas não foi possível vincular ao orçamento: " + erroBanco;
                    return false;
                }

                saida = new JObject
                {
                    { "executado", true },
                    { "idRegistroCRM", idRegistroCRM },
                    { "idCRM", idRegistroCRM },
                    { "idOrcamento", Inteiro(normalizados, "idOrcamento", 0) },
                    { "idPedido", Inteiro(normalizados, "idOrcamento", 0) },
                    { "nNumeroOrcamento", nNumeroOrcamento },
                    { "idCliente", Inteiro(normalizados, "idCliente", 0) },
                    { "referencia", Texto(normalizados, "referencia", 200) },
                    { "link", "/App/Paginas/Comercial/CRM.aspx?id=" + idRegistroCRM + "&orcamento=" + nNumeroOrcamento }
                };

                return true;
            }
            catch (Exception ex)
            {
                erro = "Erro ao criar negócio CRM para orçamento: " + ex.Message;
                return false;
            }
        }

        private static bool ValidarCrmNegocioCriarParaOrcamento(JObject argumentos, out JObject normalizados, out string resumo, out string erro)
        {
            normalizados = new JObject();
            resumo = string.Empty;
            erro = string.Empty;

            if (argumentos == null)
            {
                argumentos = new JObject();
            }

            int idOrcamento = Inteiro(argumentos, "idOrcamento", 0);
            int nNumeroOrcamento = Inteiro(argumentos, "nNumeroOrcamento", 0);
            int idTipoCotacao = Inteiro(argumentos, "idTipoCotacao", 0);
            int idVendedor = Inteiro(argumentos, "idVendedor", 0);
            int idStatus = Inteiro(argumentos, "idStatus", 0);
            int idCliente = Inteiro(argumentos, "idCliente", 0);
            int idContatoCliente = Inteiro(argumentos, "idContatoCliente", 0);
            string clienteNome = Texto(argumentos, "clienteNome", 200);
            string referencia = Texto(argumentos, "referencia", 200);
            string observacao = Texto(argumentos, "observacao", 1000);
            string dataPrevisao = Texto(argumentos, "dataPrevisao", 30);
            string confidencial = NormalizarSimNao(Texto(argumentos, "confidencial", 10));
            string chance = Texto(argumentos, "chance", 5);
            decimal valorMaterial = DecimalValor(argumentos, "valorMaterial", 0m);
            decimal valorServico = DecimalValor(argumentos, "valorServico", 0m);
            string controle = Texto(argumentos, "controle", 100);

            List<string> erros = new List<string>();
            if (idOrcamento <= 0) erros.Add("Informe o ID interno do orçamento.");
            if (referencia.Length < 3) erros.Add("Informe uma referência com ao menos 3 caracteres.");
            if (valorMaterial < 0) erros.Add("Valor de material não pode ser negativo.");
            if (valorServico < 0) erros.Add("Valor de serviço não pode ser negativo.");

            int chanceNumero = Inteiro(chance, 0);
            if (string.IsNullOrWhiteSpace(chance))
            {
                chance = "0";
            }
            else if (chanceNumero < 0 || chanceNumero > 100)
            {
                erros.Add("Chance deve ficar entre 0 e 100.");
            }

            DataRow cabecalho = null;
            if (idOrcamento > 0)
            {
                DataSet dsOrcamento = ConsultarPedidoOrcamento(idOrcamento);
                DataTable tbOrcamento = Tabela(dsOrcamento, 0);
                if (tbOrcamento == null || tbOrcamento.Rows.Count == 0)
                {
                    erros.Add("Orçamento não encontrado para vincular o CRM.");
                }
                else
                {
                    cabecalho = tbOrcamento.Rows[0];
                    int numeroCabecalho = Inteiro(Valor(cabecalho, "nNumeroPedido"), 0);
                    if (numeroCabecalho > 0)
                    {
                        nNumeroOrcamento = numeroCabecalho;
                    }
                    if (string.IsNullOrWhiteSpace(dataPrevisao))
                    {
                        dataPrevisao = PrimeiroTextoNaoVazio(Valor(cabecalho, "dtEstimativaEntrega"), Valor(cabecalho, "dtPrevisaoEntrega"));
                    }

                    int idTipoCotacaoCabecalho = Inteiro(Valor(cabecalho, "idTipoOrcamento"), 0);
                    if (idTipoCotacaoCabecalho > 0)
                    {
                        idTipoCotacao = idTipoCotacaoCabecalho;
                    }

                    int idVendedorCabecalho = Inteiro(Valor(cabecalho, "idVendedor"), 0);
                    if (idVendedorCabecalho > 0)
                    {
                        idVendedor = idVendedorCabecalho;
                    }

                    int idClienteCabecalho = Inteiro(Valor(cabecalho, "idCliente"), 0);
                    if (idCliente <= 0 && idClienteCabecalho > 0)
                    {
                        idCliente = idClienteCabecalho;
                    }
                }
            }

            if (nNumeroOrcamento <= 0)
            {
                erros.Add("Não foi possível resolver o número visível do orçamento para vincular ao CRM.");
            }
            if (idTipoCotacao <= 0)
            {
                erros.Add("Não foi possível resolver o tipo de cotação/orçamento do CRM.");
            }
            else if (!TipoOrcamentoExiste(idTipoCotacao))
            {
                erros.Add("Tipo de cotação/orçamento não encontrado. Use o idTipoOrcamento retornado pelo tipo de orçamento ou deixe 0 para resolver pelo orçamento.");
            }
            if (idVendedor <= 0)
            {
                erros.Add("Informe o vendedor do CRM.");
            }
            else
            {
                idVendedor = ResolverIdVendedorCrm(idVendedor);
                if (idVendedor <= 0)
                {
                    erros.Add("Vendedor do CRM não encontrado. Use o idVendedor retornado pelo orçamento ou deixe 0 para resolver pelo orçamento.");
                }
            }
            if (idCliente <= 0)
            {
                erros.Add("Informe o cliente do CRM.");
            }

            if (idStatus <= 0)
            {
                idStatus = ResolverStatusCrmInicial();
            }
            if (idStatus <= 0)
            {
                erros.Add("Não foi possível resolver o status inicial do CRM.");
            }

            DataSet dsCliente = idCliente > 0 ? ConsultarClienteOrcamento(idCliente) : null;
            DataTable tbCliente = Tabela(dsCliente, 0);
            if (idCliente > 0 && (tbCliente == null || tbCliente.Rows.Count == 0))
            {
                erros.Add("Cliente não encontrado para criar o CRM.");
            }
            else if (tbCliente != null && tbCliente.Rows.Count > 0 && string.IsNullOrWhiteSpace(clienteNome))
            {
                clienteNome = PrimeiroTextoNaoVazio(Valor(tbCliente.Rows[0], "sRazaoSocial"), Valor(tbCliente.Rows[0], "sNomeFantasia"));
            }

            if (idContatoCliente > 0 && !ExisteId(Tabela(dsCliente, 1), "idContato", idContatoCliente))
            {
                erros.Add("Contato informado não pertence ao cliente do CRM.");
            }

            if (clienteNome.Length < 3)
            {
                erros.Add("Informe o nome/razão social do cliente para o CRM.");
            }

            DateTime dataPrevisaoCrm = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(dataPrevisao))
            {
                dataPrevisaoCrm = DateTime.Today;
            }
            else if (!TentarParseDataHora(dataPrevisao, out dataPrevisaoCrm))
            {
                erros.Add("Data de previsão inválida. Use dd/MM/yyyy ou yyyy-MM-dd.");
            }
            dataPrevisao = dataPrevisaoCrm == DateTime.MinValue
                ? string.Empty
                : dataPrevisaoCrm.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

            if (erros.Count > 0)
            {
                erro = string.Join(" ", erros.ToArray());
                return false;
            }

            normalizados = new JObject
            {
                { "idOrcamento", idOrcamento },
                { "nNumeroOrcamento", nNumeroOrcamento },
                { "idTipoCotacao", idTipoCotacao },
                { "idVendedor", idVendedor },
                { "idStatus", idStatus },
                { "idCliente", idCliente },
                { "clienteNome", clienteNome },
                { "idContatoCliente", idContatoCliente },
                { "referencia", referencia },
                { "observacao", observacao },
                { "dataPrevisao", dataPrevisao },
                { "confidencial", confidencial },
                { "chance", chance },
                { "valorMaterial", valorMaterial },
                { "valorServico", valorServico },
                { "controle", controle }
            };

            resumo = "Criar CRM vinculado ao orçamento " + nNumeroOrcamento + " para " + clienteNome + " (" + referencia + ")";
            return true;
        }

        private static Dictionary<string, string> MontarParametrosCrmNegocioCriar(JObject args)
        {
            decimal valorMaterial = DecimalValor(args, "valorMaterial", 0m);
            decimal valorServico = DecimalValor(args, "valorServico", 0m);
            DateTime dataPrevisao;
            if (!TentarParseDataHora(Texto(args, "dataPrevisao", 30), out dataPrevisao))
            {
                dataPrevisao = DateTime.Today;
            }

            return new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR" },
                { "@idRegistroCRM", "0" },
                { "@dtInclusao", DateTime.Now.ToString("yyyyMMdd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) },
                { "@idTipoCotacao", Inteiro(args, "idTipoCotacao", 0).ToString() },
                { "@idVendedor", Inteiro(args, "idVendedor", 0).ToString() },
                { "@idStatus", Inteiro(args, "idStatus", 0).ToString() },
                { "@idCliente", Inteiro(args, "idCliente", 0).ToString() },
                { "@idParceiro", Inteiro(args, "idCliente", 0).ToString() },
                { "@sDscParceiro", Texto(args, "clienteNome", 200) },
                { "@idContato_Cliente", Inteiro(args, "idContatoCliente", 0).ToString() },
                { "@sReferencia", Texto(args, "referencia", 200) },
                { "@sObservacao", Texto(args, "observacao", 1000) },
                { "@dtPrevisao", dataPrevisao.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) },
                { "@sConfidencial", Texto(args, "confidencial", 1) },
                { "@sChance", Texto(args, "chance", 5) },
                { "@nMaterial", DecimalSql(valorMaterial, 2) },
                { "@nServico", DecimalSql(valorServico, 2) },
                { "@nValor", DecimalSql(valorMaterial + valorServico, 2) },
                { "@nControle", Texto(args, "controle", 100) },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().Trim() },
                { "@idOrcamento", Inteiro(args, "nNumeroOrcamento", 0).ToString() }
            };
        }

        private static string PrepararCrmFollowupCriar(IAFerramentaDefinicao ferramenta, JObject argumentos, IAFerramentaResultado resultado)
        {
            JObject normalizados;
            string resumo;
            string erro;
            if (!ValidarCrmFollowupCriar(argumentos, out normalizados, out resumo, out erro))
            {
                resultado.Erro = erro;
                return JsonErro(erro);
            }

            resultado.ArgumentosJson = normalizados.ToString(Formatting.None);
            resultado.Status = "PENDENTE_CONFIRMACAO";
            resultado.ResumoAcao = resumo;

            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "acaoPendente", true },
                { "resumo", resumo },
                { "dadosNormalizados", normalizados },
                { "instrucao", "A ação ficou pendente de confirmação; não afirme que o follow-up já foi criado." }
            }.ToString(Formatting.None);
        }

        private static bool ExecutarCrmFollowupCriarConfirmado(JObject argumentos, out string erro, out JToken saida)
        {
            erro = string.Empty;
            saida = new JObject { { "executado", true } };

            if (!FUNCOES.ValidaPermissao(Permissao.Comercial.CRM.Consultar, false))
            {
                erro = "Usuário sem permissão para consultar/criar follow-up de CRM.";
                return false;
            }

            JObject normalizados;
            string resumo;
            if (!ValidarCrmFollowupCriar(argumentos, out normalizados, out resumo, out erro))
            {
                return false;
            }

            try
            {
                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_CRM", MontarParametrosCrmFollowupCriar(normalizados));
                string erroBanco = ErroRetornoDataSet(ds);
                if (!string.IsNullOrWhiteSpace(erroBanco))
                {
                    erro = "Não foi possível criar o follow-up do CRM: " + erroBanco;
                    return false;
                }

                int idRegistroCRM = Inteiro(normalizados, "idRegistroCRM", 0);
                saida = new JObject
                {
                    { "executado", true },
                    { "idRegistroCRM", idRegistroCRM },
                    { "idCRM", idRegistroCRM },
                    { "followups", TabelaParaArray(Tabela(ds, 1), 20, new[] { "idRegistroCRM", "sDscMeioContato", "dtContato", "sContato", "dtProximoContato", "sObservacao" }, null) },
                    { "link", "/App/Paginas/Comercial/CRM.aspx?id=" + idRegistroCRM }
                };

                return true;
            }
            catch (Exception ex)
            {
                erro = "Erro ao criar follow-up do CRM: " + ex.Message;
                return false;
            }
        }

        private static bool ValidarCrmFollowupCriar(JObject argumentos, out JObject normalizados, out string resumo, out string erro)
        {
            normalizados = new JObject();
            resumo = string.Empty;
            erro = string.Empty;

            if (argumentos == null)
            {
                argumentos = new JObject();
            }

            int idRegistroCRM = Inteiro(argumentos, "idRegistroCRM", 0);
            string dataContatoTexto = Texto(argumentos, "dataContato", 30);
            int idMeioContato = Inteiro(argumentos, "idMeioContato", 0);
            string contatoCom = Texto(argumentos, "contatoCom", 100);
            string observacao = Texto(argumentos, "observacao", 300);
            string dataProximoTexto = Texto(argumentos, "dataProximoContato", 30);
            DateTime dataContato;
            DateTime dataProximo;

            List<string> erros = new List<string>();
            if (idRegistroCRM <= 0) erros.Add("Informe um negócio CRM válido.");
            if (idMeioContato <= 0) erros.Add("Informe o meio de contato.");
            if (contatoCom.Length < 3) erros.Add("Informe com quem foi feito o contato.");

            if (idRegistroCRM > 0)
            {
                DataSet dsCrm = ConsultarCrmNegocio(idRegistroCRM);
                if (Tabela(dsCrm, 0) == null || Tabela(dsCrm, 0).Rows.Count == 0)
                {
                    erros.Add("Negócio CRM não encontrado.");
                }
            }

            if (string.IsNullOrWhiteSpace(dataContatoTexto))
            {
                dataContato = DateTime.Now;
            }
            else if (!TentarParseDataHora(dataContatoTexto, out dataContato))
            {
                erros.Add("Data do contato inválida.");
            }

            if (!TentarParseDataHora(dataProximoTexto, out dataProximo))
            {
                erros.Add("Data do próximo contato inválida.");
            }

            if (erros.Count > 0)
            {
                erro = string.Join(" ", erros.ToArray());
                return false;
            }

            normalizados = new JObject
            {
                { "idRegistroCRM", idRegistroCRM },
                { "dataContato", dataContato.ToString("yyyy-MM-dd HH:mm:ss") },
                { "idMeioContato", idMeioContato },
                { "contatoCom", contatoCom },
                { "observacao", observacao },
                { "dataProximoContato", dataProximo.ToString("yyyy-MM-dd HH:mm:ss") }
            };

            resumo = "Criar follow-up no CRM " + idRegistroCRM + " com " + contatoCom + " em " + dataProximo.ToString("dd/MM/yyyy HH:mm");
            return true;
        }

        private static Dictionary<string, string> MontarParametrosCrmFollowupCriar(JObject args)
        {
            return new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_FOLLOWUP" },
                { "@idRegistroCRM", Inteiro(args, "idRegistroCRM", 0).ToString() },
                { "@dtContato", Texto(args, "dataContato", 30) },
                { "@idMeioContato", Inteiro(args, "idMeioContato", 0).ToString() },
                { "@sContatoCom", Texto(args, "contatoCom", 100) },
                { "@sObservacaoFollowUP", Texto(args, "observacao", 300) },
                { "@dtProximoContato", Texto(args, "dataProximoContato", 30) },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().Trim() }
            };
        }

        private static DataSet ConsultarTipoOrcamentoDetalhe(int idTipoOrcamento)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idTipoOrcamento", idTipoOrcamento.ToString() }
            };

            return BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_Orcamento_Tipo", parametros);
        }

        private static bool TipoOrcamentoExiste(int idTipoOrcamento)
        {
            DataTable tb = Tabela(ConsultarTipoOrcamentoDetalhe(idTipoOrcamento), 0);
            return tb != null && tb.Rows.Count > 0;
        }

        private static DataSet ConsultarEscoposOrcamento(int idTipoOrcamento, int idOrcamento, string sidEscopos)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA_ESCOPOS" },
                { "@idTipoOrcamento", idTipoOrcamento.ToString() },
                { "@idOrcamento", idOrcamento.ToString() },
                { "@sidEscopos", sidEscopos ?? string.Empty }
            };

            return BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_Orcamento_Tipo", parametros);
        }

        private static string ResolverSidEscoposTipoOrcamento(int idTipoOrcamento)
        {
            DataSet ds = ConsultarTipoOrcamentoDetalhe(idTipoOrcamento);
            return JuntarIds(Tabela(ds, 3), "idEscopo");
        }

        private static string ResolverSidTiposServicoTipoOrcamento(int idTipoOrcamento)
        {
            DataSet ds = ConsultarTipoOrcamentoDetalhe(idTipoOrcamento);
            return JuntarIds(Tabela(ds, 2), "idTipoProduto");
        }

        private static DataSet ConsultarEmpresaOrcamento(int idEmpresa, int idParceiro)
        {
            DataSet ds = ExecutarConsultaEmpresaOrcamento(idEmpresa, idParceiro);
            if (idEmpresa <= 0 && idParceiro > 0 && ds != null && ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
            {
                int idEmpresaResolvida = Inteiro(Valor(ds.Tables[2].Rows[0], "idEmpresa"), 0);
                if (idEmpresaResolvida > 0)
                {
                    return ExecutarConsultaEmpresaOrcamento(idEmpresaResolvida, idParceiro);
                }
            }

            return ds;
        }

        private static DataSet ExecutarConsultaEmpresaOrcamento(int idEmpresa, int idParceiro)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idEmpresa", idEmpresa.ToString() },
                { "@idParceiro", idParceiro.ToString() }
            };

            return BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Empresas", parametros);
        }

        private static DataSet ConsultarClienteOrcamento(int idCliente)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idParceiro", idCliente.ToString() }
            };

            return BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Clientes", parametros);
        }

        private static DataSet ConsultarTabelaPrecoOrcamento(int idTabela)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idTabela", idTabela.ToString() }
            };

            return BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", parametros);
        }

        private static DataSet ConsultarCondicaoPagamentoOrcamento(int idCondicaoPagamento)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idCondicaoPagamento", idCondicaoPagamento.ToString() }
            };

            return BD.ExecutarDataSet("sp_Manipula_tbl_Flow_CondicaodePagamento", parametros);
        }

        private static DataSet ConsultarPedidoOrcamento(int idOrcamento)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA PEDIDO" },
                { "@idPedido", idOrcamento.ToString() },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().Trim() }
            };

            return BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", parametros);
        }

        private static DataSet ConsultarCrmNegocio(int idRegistroCRM)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idRegistroCRM", idRegistroCRM.ToString() },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().Trim() }
            };

            return BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_CRM", parametros);
        }

        private static int ResolverStatusCrmInicial()
        {
            try
            {
                Dictionary<string, string> parametros = new Dictionary<string, string>
                {
                    { "@sTabela", "Status CRM" },
                    { "@idPesquisa", "5" },
                    { "@idFiltro", "1" }
                };

                DataTable tb = BD.ExecutarDataTable("sp_Select", parametros);
                if (tb != null)
                {
                    foreach (DataRow row in tb.Rows)
                    {
                        int idStatus = Inteiro(Valor(row, "idStatus"), 0);
                        if (idStatus > 0)
                        {
                            return idStatus;
                        }
                    }
                }
            }
            catch
            {
            }

            return 0;
        }

        private static int ResolverIdVendedorCrm(int idVendedor)
        {
            if (idVendedor <= 0)
            {
                return 0;
            }

            try
            {
                Dictionary<string, string> parametros = new Dictionary<string, string>
                {
                    { "@sTabela", "FLOW_Vendedores" }
                };

                DataTable tb = BD.ExecutarDataTable("sp_Select", parametros);
                if (tb == null || tb.Rows.Count == 0)
                {
                    return idVendedor;
                }

                bool temIdVendedor = tb.Columns.Contains("idVendedor");
                foreach (DataRow row in tb.Rows)
                {
                    int idVendedorLinha = Inteiro(Valor(row, "idVendedor"), 0);
                    int idUsuarioLinha = Inteiro(Valor(row, "idUsuario"), 0);

                    if (idVendedorLinha > 0 && idVendedorLinha == idVendedor)
                    {
                        return idVendedorLinha;
                    }
                    if (idVendedorLinha > 0 && idUsuarioLinha == idVendedor)
                    {
                        return idVendedorLinha;
                    }
                }

                return temIdVendedor ? 0 : idVendedor;
            }
            catch
            {
                return idVendedor;
            }
        }

        private static DataSet ConsultarProdutoOrcamento(int idProduto)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idProduto", idProduto.ToString() },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().Trim() }
            };

            return BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", parametros);
        }

        private static DataTable BuscarProdutosCadastro(string termo, string tipoItem)
        {
            DataTable retorno = null;
            HashSet<int> idsIncluidos = new HashSet<int>();
            List<int> tipos = new List<int>();

            if (tipoItem == "servico_recurso")
            {
                tipos.Add(1);
                tipos.Add(2);
                tipos.Add(3);
            }
            else if (tipoItem == "produto")
            {
                tipos.Add(0);
            }
            else
            {
                tipos.Add(0);
                tipos.Add(1);
                tipos.Add(2);
                tipos.Add(3);
            }

            foreach (int tipo in tipos)
            {
                DataTable tb = ConsultarProdutosCadastroPorTipo(termo, tipo);
                if (tb == null)
                {
                    continue;
                }

                if (retorno == null)
                {
                    retorno = tb.Clone();
                }

                foreach (DataRow row in tb.Rows)
                {
                    int idProduto = Inteiro(Valor(row, "idProduto"), Inteiro(Valor(row, "idItem"), 0));
                    if (idProduto <= 0 || idsIncluidos.Contains(idProduto))
                    {
                        continue;
                    }

                    idsIncluidos.Add(idProduto);
                    retorno.ImportRow(row);
                }
            }

            return retorno;
        }

        private static DataTable ConsultarProdutosCadastroPorTipo(string termo, int tipo)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sCodigo", termo },
                { "@sDscProduto", termo },
                { "@sSituacao", "S" },
                { "@sExibeComercial", "S" },
                { "@sExibeLM", "T" },
                { "@sTipo", tipo.ToString() },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().Trim() }
            };

            return BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Produtos", parametros, false);
        }

        private static bool ResolverPrecoProdutoOrcamento(int idProduto, int idTabelaPreco, int idOrcamento, decimal quantidade, decimal descontoPercentual, string tipoItem, out JObject preco, out string erro)
        {
            preco = new JObject();
            erro = string.Empty;

            if (idTabelaPreco <= 0 && idOrcamento > 0)
            {
                idTabelaPreco = ResolverTabelaPrecoOrcamento(idOrcamento);
            }

            DataSet dsProduto = ConsultarProdutoOrcamento(idProduto);
            DataTable tbProduto = Tabela(dsProduto, 0);
            if (tbProduto == null || tbProduto.Rows.Count == 0)
            {
                erro = "Produto, serviço ou recurso não encontrado para resolver preço.";
                return false;
            }

            DataRow produto = tbProduto.Rows[0];
            if (!string.Equals(Valor(produto, "sSituacao"), "S", StringComparison.OrdinalIgnoreCase))
            {
                erro = "O item informado não está ativo no cadastro de produtos.";
                return false;
            }
            if (string.Equals(Valor(produto, "sExibeComercial"), "N", StringComparison.OrdinalIgnoreCase))
            {
                erro = "O item informado está bloqueado para exibição comercial.";
                return false;
            }

            int idTipoProdutoOrcamento = Inteiro(PrimeiroTextoNaoVazio(Valor(produto, "idTipo"), Valor(produto, "idTipoProduto")), 0);
            if (!ProdutoCompativelTipoItem(tipoItem, idTipoProdutoOrcamento))
            {
                erro = tipoItem == "produto"
                    ? "O item selecionado é serviço/recurso; use tipoItem servico_recurso."
                    : "O item selecionado é produto; use tipoItem produto.";
                return false;
            }

            string codigo = PrimeiroTextoNaoVazio(Valor(produto, "sCodigo"), Valor(produto, "sCodigoProduto"));
            string descricao = PrimeiroTextoNaoVazio(Valor(produto, "sDscProduto"), Valor(produto, "sCodigoComDescricao"));
            string unidade = PrimeiroTextoNaoVazio(Valor(produto, "sUnidade"), "UN").ToUpperInvariant();
            decimal valorUnitario = 0m;
            string origemPreco = string.Empty;
            DataRow itemTabela = null;

            if (idTabelaPreco > 0)
            {
                DataTable tbItens = ConsultarItensDisponiveisTabelaPreco(idTabelaPreco);
                itemTabela = LocalizarItemPrecoTabela(tbItens, idProduto, codigo);
                if (itemTabela != null)
                {
                    valorUnitario = PrimeiroDecimalMaiorQueZero(itemTabela, "nTotal", "nPreco", "nValorUnitario", "nValor");
                    if (valorUnitario > 0m)
                    {
                        origemPreco = "tabela_preco";
                        codigo = PrimeiroTextoNaoVazio(Valor(itemTabela, "sCodigo"), codigo);
                        descricao = PrimeiroTextoNaoVazio(Valor(itemTabela, "sDscProduto"), descricao);
                        unidade = PrimeiroTextoNaoVazio(Valor(itemTabela, "sUnidade"), unidade).ToUpperInvariant();
                    }
                }

                if (valorUnitario <= 0m && !string.IsNullOrWhiteSpace(codigo))
                {
                    int idCliente = ResolverClienteOrcamento(idOrcamento);
                    DataTable tbCodigo = ConsultarProdutoPorCodigoPreco(codigo, idTabelaPreco, idCliente);
                    string erroTabela = ErroRetornoTabela(tbCodigo);
                    if (string.IsNullOrWhiteSpace(erroTabela) && tbCodigo != null && tbCodigo.Rows.Count > 0)
                    {
                        DataRow rowCodigo = tbCodigo.Rows[0];
                        valorUnitario = PrimeiroDecimalMaiorQueZero(rowCodigo, "nTotal", "nPreco", "nValorUnitario", "nValor");
                        if (valorUnitario > 0m)
                        {
                            origemPreco = "consulta_codigo";
                            itemTabela = rowCodigo;
                            codigo = PrimeiroTextoNaoVazio(Valor(rowCodigo, "sCodigo"), codigo);
                            descricao = PrimeiroTextoNaoVazio(Valor(rowCodigo, "sDscProduto"), descricao);
                            unidade = PrimeiroTextoNaoVazio(Valor(rowCodigo, "sUnidade"), unidade).ToUpperInvariant();
                        }
                    }
                }
            }

            if (valorUnitario <= 0m)
            {
                valorUnitario = PrimeiroDecimalMaiorQueZero(produto, "nTotal", "nPreco", "nValorUnitario", "nValorVenda", "nValor", "nPrecoVenda");
                if (valorUnitario > 0m)
                {
                    origemPreco = "cadastro";
                }
            }

            if (valorUnitario <= 0m)
            {
                erro = idTabelaPreco > 0
                    ? "Não foi encontrado preço para o produto na tabela informada e o cadastro não possui preço padrão disponível."
                    : "Não foi encontrado preço padrão disponível para o produto.";
                return false;
            }

            decimal fatorDesconto = descontoPercentual > 0m ? (1m - (descontoPercentual / 100m)) : 1m;
            decimal valorTotal = quantidade * valorUnitario * fatorDesconto;

            preco = new JObject
            {
                { "idProduto", idProduto },
                { "idTabelaPreco", idTabelaPreco },
                { "idOrcamento", idOrcamento },
                { "tipoItem", tipoItem },
                { "quantidade", quantidade },
                { "descontoPercentual", descontoPercentual },
                { "valorUnitario", Math.Round(valorUnitario, 4) },
                { "valorTotal", Math.Round(valorTotal, 4) },
                { "origemPreco", origemPreco },
                { "codigoProduto", codigo },
                { "descricao", descricao },
                { "unidade", unidade },
                { "tabela", idTabelaPreco > 0 ? ObjetoTabelaPrecoBasico(idTabelaPreco) : new JObject() },
                { "itemTabela", itemTabela != null ? ObjetoLinha(itemTabela, ColunasItensTabelaPreco(), null, true) : new JObject() }
            };

            return true;
        }

        private static int ResolverTabelaPrecoOrcamento(int idOrcamento)
        {
            if (idOrcamento <= 0)
            {
                return 0;
            }

            DataTable tb = Tabela(ConsultarPedidoOrcamento(idOrcamento), 0);
            return tb != null && tb.Rows.Count > 0 ? Inteiro(Valor(tb.Rows[0], "idTabelaPreco"), 0) : 0;
        }

        private static int ResolverTabelaPrecoPorTermo(string termo, out string erro)
        {
            erro = string.Empty;
            termo = (termo ?? string.Empty).Trim();
            if (termo.Length < 2)
            {
                erro = "Informe ao menos 2 caracteres para resolver a tabela de preço do item.";
                return 0;
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscTabela", termo },
                { "@idTipoTabela", "0" },
                { "@sSituacao", "S" },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().Trim() }
            };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", parametros, false);
            List<KeyValuePair<int, DataRow>> linhas = RankearTabelaResolucao(tb, termo, new[] { "idTabela", "sDscTabela", "sObservacao", "sDscTipoTabela", "sDscParceiro" });
            if (linhas.Count == 0)
            {
                erro = "Tabela de preço não encontrada para \"" + termo + "\".";
                return 0;
            }

            int melhorScore = linhas[0].Key;
            int segundoScore = linhas.Count > 1 ? linhas[1].Key : 0;
            if (linhas.Count > 1 && melhorScore < 100 && segundoScore == melhorScore)
            {
                erro = "Tabela de preço ambígua para \"" + termo + "\". Informe um nome mais específico.";
                return 0;
            }

            int idTabela = Inteiro(Valor(linhas[0].Value, "idTabela"), 0);
            if (idTabela <= 0)
            {
                erro = "Tabela de preço encontrada, mas sem idTabela válido.";
                return 0;
            }

            return idTabela;
        }

        private static int ResolverClienteOrcamento(int idOrcamento)
        {
            if (idOrcamento <= 0)
            {
                return 0;
            }

            DataTable tb = Tabela(ConsultarPedidoOrcamento(idOrcamento), 0);
            return tb != null && tb.Rows.Count > 0 ? Inteiro(Valor(tb.Rows[0], "idCliente"), 0) : 0;
        }

        private static JObject ObjetoTabelaPrecoBasico(int idTabelaPreco)
        {
            DataTable tbTabela = Tabela(ConsultarTabelaPrecoOrcamento(idTabelaPreco), 0);
            if (tbTabela == null || tbTabela.Rows.Count == 0)
            {
                return new JObject { { "idTabela", idTabelaPreco } };
            }

            return ObjetoLinha(tbTabela.Rows[0], new[] { "idTabela", "idTipoTabela", "sDscTabela", "sDscTipoTabela", "sSituacao", "sValidada" }, null);
        }

        private static DataTable ConsultarItensDisponiveisTabelaPreco(int idTabelaPreco)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_ITENS_DISPONIVEIS" },
                { "@idTabela", idTabelaPreco.ToString() }
            };

            return BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", parametros, false);
        }

        private static DataTable ConsultarProdutoPorCodigoPreco(string codigo, int idTabelaPreco, int idCliente)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA_PRODUTO_x_CODIGO" },
                { "@sCodigo", codigo },
                { "@idTabela", idTabelaPreco.ToString() },
                { "@idParceiro_Cliente", idCliente.ToString() }
            };

            return BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Produtos", parametros, false);
        }

        private static DataRow LocalizarItemPrecoTabela(DataTable tbItens, int idProduto, string codigo)
        {
            if (tbItens == null)
            {
                return null;
            }

            foreach (DataRow row in tbItens.Rows)
            {
                int idItem = Inteiro(PrimeiroTextoNaoVazio(Valor(row, "idProduto"), Valor(row, "idItem")), 0);
                if (idItem > 0 && idItem == idProduto)
                {
                    return row;
                }

                if (!string.IsNullOrWhiteSpace(codigo) && string.Equals(Valor(row, "sCodigo"), codigo, StringComparison.OrdinalIgnoreCase))
                {
                    return row;
                }
            }

            return null;
        }

        private static decimal PrimeiroDecimalMaiorQueZero(DataRow row, params string[] colunas)
        {
            foreach (string coluna in colunas ?? new string[0])
            {
                decimal valor = DecimalValor(Valor(row, coluna), 0m);
                if (valor > 0m)
                {
                    return valor;
                }
            }

            return 0m;
        }

        private static bool ObterOrcamentoEditavel(int idOrcamento, out DataSet dsOrcamento, out DataRow cabecalho, out string erro)
        {
            dsOrcamento = null;
            cabecalho = null;
            erro = string.Empty;

            if (idOrcamento <= 0)
            {
                erro = "Informe um orçamento válido.";
                return false;
            }

            dsOrcamento = ConsultarPedidoOrcamento(idOrcamento);
            DataTable tbCabecalho = Tabela(dsOrcamento, 0);
            if (tbCabecalho == null || tbCabecalho.Rows.Count == 0)
            {
                erro = "Orçamento não encontrado ou sem acesso para consulta.";
                return false;
            }

            cabecalho = tbCabecalho.Rows[0];
            if (Inteiro(Valor(cabecalho, "idTipo"), 0) != 1)
            {
                erro = "O registro informado não é um orçamento.";
                return false;
            }

            if (string.Equals(Valor(cabecalho, "sPermiteEdicaoPedido"), "N", StringComparison.OrdinalIgnoreCase)
                || string.Equals(Valor(cabecalho, "sPedidoFinalizado"), "S", StringComparison.OrdinalIgnoreCase)
                || Inteiro(Valor(cabecalho, "idMotivoCancelamento"), 0) > 0)
            {
                erro = "O orçamento não permite edição no status atual.";
                return false;
            }

            return true;
        }

        private static bool ProdutoCompativelTipoItem(string tipoItem, int idTipoProdutoOrcamento)
        {
            if (string.Equals(tipoItem, "produto", StringComparison.OrdinalIgnoreCase))
            {
                return idTipoProdutoOrcamento == 0;
            }

            return idTipoProdutoOrcamento == 1 || idTipoProdutoOrcamento == 2 || idTipoProdutoOrcamento == 3;
        }

        private static int ProximaOrdemItem(DataTable itens)
        {
            int maior = 0;
            if (itens != null)
            {
                foreach (DataRow row in itens.Rows)
                {
                    if (!ItemRaiz(row))
                    {
                        continue;
                    }

                    int ordem = Inteiro(Valor(row, "nOrdem"), 0);
                    if (ordem > maior)
                    {
                        maior = ordem;
                    }
                }
            }

            return maior + 10;
        }

        private static string DataPrevisaoItem(DataRow cabecalho, int diasPrevisaoEntrega)
        {
            DateTime dataBase;
            string dtPedido = Valor(cabecalho, "dtPedido");
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");
            if (!DateTime.TryParse(dtPedido, cultura, System.Globalization.DateTimeStyles.None, out dataBase))
            {
                dataBase = DateTime.Today;
            }

            return dataBase.AddDays(Math.Max(0, diasPrevisaoEntrega)).ToString("dd/MM/yyyy");
        }

        private static bool RecalcularTotaisOrcamento(int idOrcamento, out JObject totais, out string erro, out DataSet dsAtualizado)
        {
            totais = new JObject();
            erro = string.Empty;
            dsAtualizado = null;

            DataRow cabecalho;
            if (!ObterOrcamentoEditavel(idOrcamento, out dsAtualizado, out cabecalho, out erro))
            {
                return false;
            }

            decimal totalProdutos;
            decimal totalServicos;
            int idTipoFaturamento;
            TotalizarItensOrcamento(Tabela(dsAtualizado, 1), out totalProdutos, out totalServicos, out idTipoFaturamento);

            if (!AtualizarTotaisOrcamento(cabecalho, totalProdutos, totalServicos, idTipoFaturamento, out erro))
            {
                return false;
            }

            totais = new JObject
            {
                { "totalProdutos", totalProdutos },
                { "totalServicos", totalServicos },
                { "valorOriginal", totalProdutos + totalServicos },
                { "idTipoFaturamento", idTipoFaturamento }
            };

            return true;
        }

        private static void TotalizarItensOrcamento(DataTable itens, out decimal totalProdutos, out decimal totalServicos, out int idTipoFaturamento)
        {
            totalProdutos = 0m;
            totalServicos = 0m;
            idTipoFaturamento = 0;

            if (itens != null)
            {
                foreach (DataRow row in itens.Rows)
                {
                    if (!ItemRaiz(row))
                    {
                        continue;
                    }

                    decimal valorTotal = DecimalValor(Valor(row, "nValorTotal"), 0m);
                    bool servico = string.Equals(Valor(row, "sTipoProduto_Servico"), "S", StringComparison.OrdinalIgnoreCase)
                        || Valor(row, "sTipoItem").IndexOf("Servico", StringComparison.OrdinalIgnoreCase) >= 0
                        || Valor(row, "sTipoItem").IndexOf("Serviço", StringComparison.OrdinalIgnoreCase) >= 0;

                    if (servico)
                    {
                        totalServicos += valorTotal;
                    }
                    else
                    {
                        totalProdutos += valorTotal;
                    }
                }
            }

            if (totalProdutos > 0m && totalServicos > 0m)
            {
                idTipoFaturamento = 3;
            }
            else if (totalServicos > 0m)
            {
                idTipoFaturamento = 2;
            }
            else if (totalProdutos > 0m)
            {
                idTipoFaturamento = 1;
            }
        }

        private static bool AtualizarTotaisOrcamento(DataRow cabecalho, decimal totalProdutos, decimal totalServicos, int idTipoFaturamento, out string erro)
        {
            erro = string.Empty;
            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", MontarParametrosOrcamentoAlterarTotais(cabecalho, totalProdutos, totalServicos, idTipoFaturamento));
            string erroBanco = ErroRetornoDataSetSeHouver(ds);
            if (!string.IsNullOrWhiteSpace(erroBanco))
            {
                erro = "Item gravado, mas não foi possível recalcular os totais do orçamento: " + erroBanco;
                return false;
            }

            return true;
        }

        private static Dictionary<string, string> MontarParametrosOrcamentoAlterarTotais(DataRow row, decimal totalProdutos, decimal totalServicos, int idTipoFaturamento)
        {
            decimal valorEnvio = DecimalValor(Valor(row, "nValorEnvio"), 0m);
            decimal valorOriginal = totalProdutos + totalServicos + valorEnvio;

            return new Dictionary<string, string>
            {
                { "@sFuncao", "ALTERAR PEDIDO" },
                { "@idPedido", Inteiro(Valor(row, "idPedido"), 0).ToString() },
                { "@idTipo", "1" },
                { "@idCliente", Inteiro(Valor(row, "idCliente"), 0).ToString() },
                { "@sReferencia", ValorLimitado(row, "sReferencia", 60) },
                { "@sPedidoCliente", ValorLimitado(row, "sPedidoCliente", 50) },
                { "@dtPedido", ValorLimitado(row, "dtPedido", 10) },
                { "@idCondicaoDePagamento", Inteiro(Valor(row, "idCondicaoPagamento"), 0).ToString() },
                { "@idFluxo", Inteiro(Valor(row, "idFluxo"), 0).ToString() },
                { "@idVendedor", Inteiro(Valor(row, "idVendedor_Usuario"), Inteiro(Valor(row, "idVendedor"), 0)).ToString() },
                { "@sObservacao", ValorLimitado(row, "sObservacao", 4000) },
                { "@idUsuarioInclusao", IDENTITY.Variaveis.idUsuario().Trim() },
                { "@nVlrProdutos", DecimalSql(totalProdutos, 2) },
                { "@nVlrServicos", DecimalSql(totalServicos, 2) },
                { "@idTipoFaturamento", idTipoFaturamento.ToString() },
                { "@idTipoEnvio", Inteiro(Valor(row, "idTipoEnvio"), 0).ToString() },
                { "@idEmpresa", Inteiro(Valor(row, "idEmpresa"), 0).ToString() },
                { "@idVersao", Inteiro(Valor(row, "idVersao"), 0).ToString() },
                { "@nVlrComissao", DecimalSql(DecimalValor(Valor(row, "nVlrComissao"), 0m), 2) },
                { "@idParceiro_Comissionador", Inteiro(Valor(row, "idParceiro_Comissionador"), 0).ToString() },
                { "@sComissaoPaga", ValorLimitado(row, "sComissaoPaga", 1) },
                { "@idModal", Inteiro(Valor(row, "idModal"), 0).ToString() },
                { "@Importacao_idTipoMercadoria", Inteiro(Valor(row, "Importacao_idTipoMercadoria"), 0).ToString() },
                { "@Importacao_idDespacho", Inteiro(Valor(row, "Importacao_idDespacho"), 0).ToString() },
                { "@Importacao_idFornecedor", Inteiro(Valor(row, "Importacao_idFornecedor"), 0).ToString() },
                { "@Importacao_idDespachante", Inteiro(Valor(row, "Importacao_idDespachante"), 0).ToString() },
                { "@Importacao_idPais", Inteiro(Valor(row, "Importacao_idPais"), 0).ToString() },
                { "@nExWorks", DecimalSql(DecimalValor(Valor(row, "nExWorks"), 0m), 2) },
                { "@nInlandF", DecimalSql(DecimalValor(Valor(row, "nInlandF"), 0m), 2) },
                { "@nHandling", DecimalSql(DecimalValor(Valor(row, "nHandling"), 0m), 2) },
                { "@nConsular", DecimalSql(DecimalValor(Valor(row, "nConsular"), 0m), 2) },
                { "@nOcean_Air", DecimalSql(DecimalValor(Valor(row, "nOcean_Air"), 0m), 2) },
                { "@nInsurance", DecimalSql(DecimalValor(Valor(row, "nInsurance"), 0m), 2) },
                { "@nOther_Charges", DecimalSql(DecimalValor(Valor(row, "nOther_Charges"), 0m), 2) },
                { "@Importacao_idCondicaoDePagamento", Inteiro(Valor(row, "Importacao_idCondicaoDePagamento"), 0).ToString() },
                { "@sInvoiceNumber", ValorLimitado(row, "sInvoiceNumber", 20) },
                { "@nPesoBruto", DecimalSql(DecimalValor(Valor(row, "nPesoBruto"), 0m), 4) },
                { "@idContato_Empresa", Inteiro(Valor(row, "idContato_Empresa"), 0).ToString() },
                { "@idContato_Exportador", Inteiro(Valor(row, "idContato_Exportador"), 0).ToString() },
                { "@idContato_Despachante", Inteiro(Valor(row, "idContato_Despachante"), 0).ToString() },
                { "@nOrderNumber", ValorLimitado(row, "nOrderNumber", 20) },
                { "@sZonaEnvio", ValorLimitado(row, "sZonaEnvio", 50) },
                { "@Importacao_idImportador", Inteiro(Valor(row, "Importacao_idImportador"), 0).ToString() },
                { "@idContato_Importador", Inteiro(Valor(row, "idContato_Importador"), 0).ToString() },
                { "@nSaldo", DecimalSql(DecimalValor(Valor(row, "nSaldo"), 0m), 2) },
                { "@nValorPago", DecimalSql(DecimalValor(Valor(row, "nValorPago"), 0m), 2) },
                { "@nValorOriginal", DecimalSql(valorOriginal, 2) },
                { "@nValorEnvio", DecimalSql(valorEnvio, 4) },
                { "@nValorEnvioReal", DecimalSql(DecimalValor(Valor(row, "nValorEnvioReal"), 0m), 4) },
                { "@nResultadoEnvio", DecimalSql(DecimalValor(Valor(row, "nResultadoEnvio"), 0m), 4) },
                { "@nTotalEnvio", DecimalSql(DecimalValor(Valor(row, "nTotalEnvio"), 0m), 2) },
                { "@idEnvioPago", Inteiro(Valor(row, "idEnvioPago"), 0).ToString() },
                { "@idTipoOrcamento", Inteiro(Valor(row, "idTipoOrcamento"), 0).ToString() },
                { "@sConfidencial", ValorLimitado(row, "sConfidencial", 1) },
                { "@sEmpresaTransporte", ValorLimitado(row, "sEmpresaTransporte", 200) },
                { "@nFretePrevisto", DecimalSql(DecimalValor(Valor(row, "nFretePrevisto"), 0m), 4) },
                { "@idTabelaPreco", Inteiro(Valor(row, "idTabelaPreco"), 0).ToString() },
                { "@sAlteracaoTabelaPreco_Obs", ValorLimitado(row, "sAlteracaoTabelaPreco_Obs", 100) },
                { "@idEnderecoEntrega", Inteiro(Valor(row, "idEnderecoEntrega"), 0).ToString() },
                { "@sDestinoVenda", ValorLimitado(row, "sDestinoVenda", 1) },
                { "@idEnderecoDestino", Inteiro(Valor(row, "idEnderecoDestino"), 0).ToString() },
                { "@sUF_Entrega", ValorLimitado(row, "sUF_Entrega", 2) },
                { "@nDiasPrevisao", Inteiro(Valor(row, "nDiasPrevisao"), 0).ToString() },
                { "@sUF_Fiscal", ValorLimitado(row, "sUF_Fiscal", 2) },
                { "@idCidade_Entrega", Inteiro(Valor(row, "idCidade_Entrega"), 0).ToString() },
                { "@nValidadeOrcamento", Inteiro(Valor(row, "nValidadeOrcamento"), 0).ToString() },
                { "@idContato_Cliente", Inteiro(Valor(row, "idContato_Cliente"), 0).ToString() },
                { "@idTipoCliente", Inteiro(Valor(row, "idTipoCliente"), 0).ToString() },
                { "@sidSegmentosCliente", ValorLimitado(row, "sidSegmentosCliente", 200) },
                { "@sidTiposServicos", ValorLimitado(row, "sidTiposServicos", 200) },
                { "@sidEscopos", ValorLimitado(row, "sidEscopos", 200) },
                { "@sTipoCompra", ValorLimitado(row, "sTipoCompra", 1) },
                { "@idTipoMoeda", Inteiro(Valor(row, "idTipoMoeda"), 0).ToString() },
                { "@dtEstimativaEntrega", ValorLimitado(row, "dtEstimativaEntrega", 10) },
                { "@sLogradouro", LogradouroPedidoParaPersistir(row) },
                { "@nControleTT", ValorLimitado(row, "nControleTT", 100) },
                { "@sClienteFinal", ValorLimitado(row, "sClienteFinal", 1) },
                { "@idClienteFinal", Inteiro(Valor(row, "idClienteFinal"), 0).ToString() },
                { "@sTipoDrawback", ValorLimitado(row, "sTipoDrawback", 1) },
                { "@sAlteracaoCondPag_Motivo", ValorLimitado(row, "sAlteracaoCondPag_Motivo", 100) },
                { "@nVlrDIFAL", DecimalSql(DecimalValor(Valor(row, "nVlrDIFAL"), 0m), 2) },
                { "@nVlrST", DecimalSql(DecimalValor(Valor(row, "nVlrST"), 0m), 2) },
                { "@idConceito", Inteiro(Valor(row, "idConceito"), 0).ToString() },
                { "@idCentroCusto", Inteiro(Valor(row, "idCentroCusto"), 0).ToString() },
                { "@idGrupoPatrimonio", Inteiro(Valor(row, "idGrupoPatrimonio"), 0).ToString() },
                { "@idFrete", Inteiro(Valor(row, "idFrete"), 0).ToString() },
                { "@nDesc", DecimalSql(DecimalValor(Valor(row, "nDesconto"), 0m), 4) },
                { "@nVlrDesconto", DecimalSql(DecimalValor(Valor(row, "nVlrDesconto"), 0m), 4) },
                { "@nDiasContrato", Inteiro(Valor(row, "nDiasContrato"), 0).ToString() },
                { "@sTipoPeriodoContrato", ValorLimitado(row, "sTipoPeriodoContrato", 1) },
                { "@sAlteracaoItens", "S" }
            };
        }

        private static bool ItemRaiz(DataRow row)
        {
            return Inteiro(Valor(row, "idProdutoPai"), 0) == 0
                && Inteiro(Valor(row, "idProdutoAvo"), 0) == 0
                && Inteiro(Valor(row, "idProdutoBisavo"), 0) == 0;
        }

        private static DataRow LocalizarItemOrcamento(DataTable itens, int idProduto, int idProdutoPai, int idProdutoAvo, int idProdutoBisavo)
        {
            if (itens == null)
            {
                return null;
            }

            DataRow encontrado = null;
            int maiorId = 0;
            foreach (DataRow row in itens.Rows)
            {
                if (Inteiro(Valor(row, "idProduto"), 0) != idProduto
                    || Inteiro(Valor(row, "idProdutoPai"), 0) != idProdutoPai
                    || Inteiro(Valor(row, "idProdutoAvo"), 0) != idProdutoAvo
                    || Inteiro(Valor(row, "idProdutoBisavo"), 0) != idProdutoBisavo)
                {
                    continue;
                }

                int idItem = Inteiro(Valor(row, "idItem"), 0);
                if (idItem >= maiorId)
                {
                    maiorId = idItem;
                    encontrado = row;
                }
            }

            return encontrado;
        }

        private static bool ExisteCategoriaEscopo(DataTable tb, int idEscopo, int idCategoria)
        {
            if (tb == null)
            {
                return false;
            }

            foreach (DataRow row in tb.Rows)
            {
                if (Inteiro(Valor(row, "idEscopo"), 0) == idEscopo && Inteiro(Valor(row, "idCategoria"), 0) == idCategoria)
                {
                    return true;
                }
            }

            return false;
        }

        private static DataRow LocalizarRespostaEscopo(int idOrcamento, JObject args)
        {
            DataSet dsOrcamento = ConsultarPedidoOrcamento(idOrcamento);
            DataRow cabecalho = Tabela(dsOrcamento, 0) != null && Tabela(dsOrcamento, 0).Rows.Count > 0 ? Tabela(dsOrcamento, 0).Rows[0] : null;
            int idTipoOrcamento = Inteiro(Valor(cabecalho, "idTipoOrcamento"), 0);
            string sidEscopos = Valor(cabecalho, "sidEscopos");

            DataSet dsEscopos = ConsultarEscoposOrcamento(idTipoOrcamento, idOrcamento, sidEscopos);
            DataTable tbRespostas = Tabela(dsEscopos, 1);
            if (tbRespostas == null)
            {
                return null;
            }

            int idEscopo = Inteiro(args, "idEscopo", 0);
            int idCategoria = Inteiro(args, "idCategoria", 0);
            int idPergunta = Inteiro(args, "idPergunta", 0);
            int idOpcao = Inteiro(args, "idOpcao", 0);
            string pergunta = Texto(args, "pergunta", 500);

            foreach (DataRow row in tbRespostas.Rows)
            {
                bool mesmaCategoria = Inteiro(Valor(row, "idEscopo"), 0) == idEscopo
                    && Inteiro(Valor(row, "idCategoriaEscopo"), 0) == idCategoria;
                bool mesmaPergunta = (idPergunta > 0 && Inteiro(Valor(row, "idPergunta"), 0) == idPergunta)
                    || string.Equals(Valor(row, "sPergunta"), pergunta, StringComparison.OrdinalIgnoreCase);
                bool mesmaOpcao = idOpcao <= 0 || Inteiro(Valor(row, "idOpcao"), 0) == idOpcao;

                if (mesmaCategoria && mesmaPergunta && mesmaOpcao)
                {
                    return row;
                }
            }

            return null;
        }

        private static string ValorLimitado(DataRow row, string coluna, int limite)
        {
            return cls_IA_Sanitizacao.LimparEntradaUsuario(Valor(row, coluna), limite);
        }

        private static string LogradouroPedidoParaPersistir(DataRow row)
        {
            string logradouro = (Valor(row, "sLogradouro") ?? string.Empty).Trim();
            string cep = (Valor(row, "sCEP") ?? string.Empty).Trim();
            string numero = (Valor(row, "sNumero") ?? string.Empty).Trim();

            string prefixoCep = string.IsNullOrWhiteSpace(cep) ? string.Empty : cep + " - ";
            while (prefixoCep.Length > 0 && logradouro.StartsWith(prefixoCep, StringComparison.OrdinalIgnoreCase))
            {
                logradouro = logradouro.Substring(prefixoCep.Length).Trim();
            }

            string sufixoNumero = string.IsNullOrWhiteSpace(numero) ? string.Empty : ", " + numero;
            while (sufixoNumero.Length > 0 && logradouro.EndsWith(sufixoNumero, StringComparison.OrdinalIgnoreCase))
            {
                logradouro = logradouro.Substring(0, logradouro.Length - sufixoNumero.Length).Trim();
            }

            return cls_IA_Sanitizacao.LimparEntradaUsuario(logradouro, 200);
        }

        private static bool ExisteId(DataTable tb, string coluna, int id)
        {
            if (tb == null || !tb.Columns.Contains(coluna))
            {
                return false;
            }

            foreach (DataRow row in tb.Rows)
            {
                if (Inteiro(Valor(row, coluna), 0) == id)
                {
                    return true;
                }
            }

            return false;
        }

        private static DataRow LocalizarContatoCliente(DataTable contatos, string nome, string email)
        {
            if (contatos == null || contatos.Rows.Count == 0)
            {
                return null;
            }

            DataRow primeiro = contatos.Rows[0];
            foreach (DataRow row in contatos.Rows)
            {
                bool mesmoNome = string.Equals(Valor(row, "sNome"), nome, StringComparison.OrdinalIgnoreCase);
                bool mesmoEmail = !string.IsNullOrWhiteSpace(email) && string.Equals(Valor(row, "sEmail"), email, StringComparison.OrdinalIgnoreCase);
                if (mesmoNome || mesmoEmail)
                {
                    return row;
                }
            }

            return primeiro;
        }

        private static DataRow LocalizarEnderecoCliente(DataTable enderecos, int idTipoEndereco, string cep, string logradouro, string cidade)
        {
            if (enderecos == null || enderecos.Rows.Count == 0)
            {
                return null;
            }

            DataRow primeiroTipo = null;
            foreach (DataRow row in enderecos.Rows)
            {
                if (Inteiro(Valor(row, "idTipoEndereco"), 0) != idTipoEndereco)
                {
                    continue;
                }

                if (primeiroTipo == null)
                {
                    primeiroTipo = row;
                }

                bool mesmoCep = !string.IsNullOrWhiteSpace(cep) && string.Equals(SomenteDigitos(Valor(row, "sCEP")), SomenteDigitos(cep), StringComparison.OrdinalIgnoreCase);
                bool mesmoLogradouro = !string.IsNullOrWhiteSpace(logradouro) && Normalizar(Valor(row, "sLogradouro")) == Normalizar(logradouro);
                bool mesmaCidade = !string.IsNullOrWhiteSpace(cidade) && Normalizar(Valor(row, "sCidade")) == Normalizar(cidade);

                if (mesmoCep || (mesmoLogradouro && mesmaCidade))
                {
                    return row;
                }
            }

            return primeiroTipo;
        }

        private static string ErroRetornoDataSet(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0)
            {
                return "A procedure não retornou dados.";
            }

            foreach (DataTable tb in ds.Tables)
            {
                if (tb == null || tb.Rows.Count == 0)
                {
                    continue;
                }

                DataRow row = tb.Rows[0];
                string msg = Valor(row, "msg");
                if (string.IsNullOrWhiteSpace(msg))
                {
                    msg = Valor(row, "sMsg");
                }

                string retTexto = Valor(row, "nRet");
                if (string.IsNullOrWhiteSpace(retTexto))
                {
                    retTexto = Valor(row, "RET");
                }
                if (string.IsNullOrWhiteSpace(retTexto))
                {
                    retTexto = Valor(row, "ret");
                }

                if (!string.IsNullOrWhiteSpace(retTexto) && Inteiro(retTexto, 0) != 0)
                {
                    return string.IsNullOrWhiteSpace(msg) ? "A procedure retornou erro." : msg;
                }

                if (string.Equals(Valor(row, "sErro"), "1", StringComparison.OrdinalIgnoreCase))
                {
                    return string.IsNullOrWhiteSpace(msg) ? "A procedure retornou erro." : msg;
                }
            }

            return string.Empty;
        }

        private static string ErroRetornoDataSetSeHouver(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0)
            {
                return string.Empty;
            }

            foreach (DataTable tb in ds.Tables)
            {
                if (tb == null || tb.Rows.Count == 0)
                {
                    continue;
                }

                DataRow row = tb.Rows[0];
                string msg = Valor(row, "msg");
                if (string.IsNullOrWhiteSpace(msg))
                {
                    msg = Valor(row, "sMsg");
                }

                string retTexto = Valor(row, "nRet");
                if (string.IsNullOrWhiteSpace(retTexto))
                {
                    retTexto = Valor(row, "RET");
                }
                if (string.IsNullOrWhiteSpace(retTexto))
                {
                    retTexto = Valor(row, "ret");
                }

                if (!string.IsNullOrWhiteSpace(retTexto) && Inteiro(retTexto, 0) != 0)
                {
                    if (msg.IndexOf("Nenhuma Alter", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return string.Empty;
                    }

                    return string.IsNullOrWhiteSpace(msg) ? "A procedure retornou erro." : msg;
                }

                if (string.Equals(Valor(row, "sErro"), "1", StringComparison.OrdinalIgnoreCase))
                {
                    return string.IsNullOrWhiteSpace(msg) ? "A procedure retornou erro." : msg;
                }
            }

            return string.Empty;
        }

        private static bool UfValidaOuVazia(string uf)
        {
            return string.IsNullOrWhiteSpace(uf) || Regex.IsMatch(uf.Trim().ToUpperInvariant(), "^[A-Z]{2}$");
        }

        private static string DataObrigatoriaOrcamento(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return string.Empty;
            }

            DateTime data;
            string texto = valor.Trim();
            string[] formatos = new[] { "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ss.fffK" };
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");
            if (DateTime.TryParseExact(texto, formatos, cultura, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out data)
                || DateTime.TryParse(texto, cultura, System.Globalization.DateTimeStyles.None, out data))
            {
                return data.ToString("dd/MM/yyyy");
            }

            return string.Empty;
        }

        private static bool TentarParseDataHora(string valor, out DateTime data)
        {
            data = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(valor))
            {
                return false;
            }

            string texto = valor.Trim();
            string[] formatos = new[]
            {
                "dd/MM/yyyy HH:mm:ss",
                "dd/MM/yyyy HH:mm",
                "dd/MM/yyyy",
                "d/M/yyyy H:mm:ss",
                "d/M/yyyy H:mm",
                "d/M/yyyy",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd HH:mm",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm",
                "yyyy-MM-dd",
                "yyyyMMdd"
            };

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");
            return DateTime.TryParseExact(texto, formatos, cultura, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out data)
                || DateTime.TryParse(texto, cultura, System.Globalization.DateTimeStyles.None, out data);
        }

        private static decimal DecimalValor(JObject argumentos, string chave, decimal padrao)
        {
            if (argumentos == null || argumentos[chave] == null)
            {
                return padrao;
            }

            JToken token = argumentos[chave];
            if (token.Type == JTokenType.Integer || token.Type == JTokenType.Float)
            {
                try
                {
                    return token.Value<decimal>();
                }
                catch
                {
                    return padrao;
                }
            }

            string texto = token.ToString().Trim();
            if (string.IsNullOrWhiteSpace(texto))
            {
                return padrao;
            }

            decimal valor;
            if (decimal.TryParse(texto, System.Globalization.NumberStyles.Number, new System.Globalization.CultureInfo("pt-BR"), out valor))
            {
                return valor;
            }
            if (decimal.TryParse(texto, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out valor))
            {
                return valor;
            }

            return padrao;
        }

        private static decimal DecimalValor(string texto, decimal padrao)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return padrao;
            }

            decimal valor;
            if (decimal.TryParse(texto.Trim(), System.Globalization.NumberStyles.Number, new System.Globalization.CultureInfo("pt-BR"), out valor))
            {
                return valor;
            }
            if (decimal.TryParse(texto.Trim(), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out valor))
            {
                return valor;
            }

            return padrao;
        }

        private static string DecimalSql(decimal valor, int casas)
        {
            if (casas <= 0)
            {
                return Math.Round(valor, 0).ToString("0", System.Globalization.CultureInfo.InvariantCulture);
            }

            return Math.Round(valor, casas).ToString("0." + new string('0', casas), System.Globalization.CultureInfo.InvariantCulture);
        }

        private static string NormalizarSimNao(string valor)
        {
            string texto = (valor ?? string.Empty).Trim();
            if (string.Equals(texto, "S", StringComparison.OrdinalIgnoreCase)
                || string.Equals(texto, "SIM", StringComparison.OrdinalIgnoreCase)
                || string.Equals(texto, "TRUE", StringComparison.OrdinalIgnoreCase)
                || texto == "1")
            {
                return "S";
            }

            return "N";
        }

        private static string NormalizarListaIdsPipe(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return string.Empty;
            }

            List<string> ids = new List<string>();
            foreach (string parte in valor.Split(new[] { '|', ',', ';', ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int id = Inteiro(parte, 0);
                string texto = id.ToString();
                if (id > 0 && !ids.Contains(texto))
                {
                    ids.Add(texto);
                }
            }

            return ids.Count == 0 ? string.Empty : "|" + string.Join("|", ids.ToArray()) + "|";
        }

        private static string JuntarIds(DataTable tb, string coluna)
        {
            if (tb == null || !tb.Columns.Contains(coluna))
            {
                return string.Empty;
            }

            List<string> ids = new List<string>();
            foreach (DataRow row in tb.Rows)
            {
                int id = Inteiro(Valor(row, coluna), 0);
                if (id > 0)
                {
                    string texto = id.ToString();
                    if (!ids.Contains(texto))
                    {
                        ids.Add(texto);
                    }
                }
            }

            return ids.Count == 0 ? string.Empty : "|" + string.Join("|", ids.ToArray()) + "|";
        }

        private static string NormalizarIdsComSeparador(string valor, char separador, bool terminarComSeparador)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return string.Empty;
            }

            List<string> ids = new List<string>();
            foreach (string parte in valor.Split(new[] { '|', ',', ';', ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int id = Inteiro(parte, 0);
                string texto = id.ToString();
                if (id > 0 && !ids.Contains(texto))
                {
                    ids.Add(texto);
                }
            }

            if (ids.Count == 0)
            {
                return string.Empty;
            }

            string retorno = string.Join(separador.ToString(), ids.ToArray());
            if (terminarComSeparador || separador == '|')
            {
                retorno += separador;
            }

            return retorno;
        }

        private static string[] ColunasItensTabelaPreco()
        {
            return new[]
            {
                "idItem",
                "idTabela",
                "nPreco",
                "nPreco_Zona_SD",
                "nPreco_Zona_ND",
                "nPreco_Zona_N",
                "nPreco_Zona_CO",
                "nPreco_Zona_S",
                "nFator",
                "idTabelaOrigem",
                "nTaxaEnvio",
                "nTaxaLocal",
                "nTotal",
                "nMargem",
                "nII",
                "nIPI",
                "nPIS",
                "nCOFINS",
                "nICMS",
                "sLiberado",
                "sCodigo",
                "sDscProduto",
                "sUnidade",
                "idGrupo",
                "idFamilia",
                "sDscGrupo",
                "sDscFamilia",
                "sCodigoComDescricao",
                "sDscTipoProduto",
                "nTaxaImpostos",
                "sSimboloMoedaOrigem",
                "sSimboloMoedaDestino",
                "sIndustrializado",
                "sOrigem"
            };
        }

        private static string ErroRetornoTabela(DataTable tb)
        {
            if (tb == null || tb.Rows.Count == 0)
            {
                return string.Empty;
            }

            DataRow row = tb.Rows[0];
            string msg = Valor(row, "sMsg");
            if (!string.IsNullOrWhiteSpace(msg))
            {
                return msg;
            }

            string naoExiste = Valor(row, "sNaoExiste");
            if (string.Equals(naoExiste, "S", StringComparison.OrdinalIgnoreCase))
            {
                return "Produto não encontrado pelo código informado.";
            }

            return string.Empty;
        }

        // ===== Motor de execucao generica (ferramenta configurada 100% pela tela) =====
        // Sem SQL dinamico: so chamada de SP parametrizada. A allowlist de colunas e a mascara de
        // CPF/CNPJ continuam no TabelaParaJson.

        // READ: chama a SP e projeta as colunas configuradas.
        private string ExecutarFerramentaGenerica(int idConversaIA, IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string procedure = (ferramenta.ProcedureGenerica ?? string.Empty).Trim();
            if (procedure.Length == 0)
            {
                return JsonErro("Ferramenta generica sem procedure configurada.");
            }

            List<string> colunas = new List<string>();
            foreach (JToken col in ParseArrayGenerico(ferramenta.ColunasRetorno))
            {
                string nome = col != null ? col.ToString().Trim() : string.Empty;
                if (nome.Length > 0)
                {
                    colunas.Add(nome);
                }
            }

            if (colunas.Count == 0)
            {
                return JsonErro("Ferramenta generica sem colunas de retorno configuradas.");
            }

            Dictionary<string, string> parametros = MontarParametrosGenerico(ferramenta.MapaParametros, argumentos);

            cls_IA_Auditoria.Registrar(idConversaIA, "TOOL_GENERICA", "ALERTA", new
            {
                ferramenta = ferramenta.Nome,
                procedure = procedure,
                colunas = string.Join(",", colunas)
            });

            DataSet ds = BD.ExecutarDataSet(procedure, parametros);
            if (ds == null || ds.Tables.Count == 0)
            {
                return JsonErro("A procedure generica nao retornou dados.");
            }

            int indice = ferramenta.IndiceTabelaRetorno;
            if (indice < 0 || indice >= ds.Tables.Count)
            {
                indice = 0;
            }

            int limite = Limite(argumentos, ferramenta.MaxRegistros);
            return TabelaParaJson(ferramenta, ds.Tables[indice], limite, colunas.ToArray(), null);
        }

        private static string ExecutarWorkflowsListarDisponiveis(IAFerramentaDefinicao ferramenta)
        {
            JArray registros = new JArray();
            int totalEncontrado = 0;
            int limite = ferramenta != null && ferramenta.MaxRegistros > 0 ? ferramenta.MaxRegistros : 50;

            foreach (IAFerramentaDefinicao item in cls_IA_ToolRegistry.ListarPermitidas())
            {
                if (item == null || !item.EhWorkflow())
                {
                    continue;
                }

                totalEncontrado++;
                if (registros.Count >= limite)
                {
                    continue;
                }

                registros.Add(new JObject
                {
                    { "nome", item.Nome ?? string.Empty },
                    { "descricao", item.Descricao ?? string.Empty },
                    { "tipo", string.Equals(item.Escopo, "WRITE", StringComparison.OrdinalIgnoreCase) ? "WRITE" : "READ" },
                    { "requerConfirmacao", string.Equals(item.Escopo, "WRITE", StringComparison.OrdinalIgnoreCase) },
                    { "parametros", ParametrosSchema(item.SchemaParametros) },
                    { "instrucaoUso", "Para executar, chame a ferramenta \"" + (item.Nome ?? string.Empty) + "\" com os parametros declarados." }
                });
            }

            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta != null ? ferramenta.Nome : "workflows_listar_disponiveis" },
                { "totalEncontrado", totalEncontrado },
                { "totalRetornado", registros.Count },
                { "mensagem", registros.Count == 0 ? "Nenhum workflow disponivel para o usuario atual." : "Workflows disponiveis para o usuario atual." },
                { "registros", registros }
            }.ToString(Formatting.None);
        }

        private static JArray ParametrosSchema(JObject schema)
        {
            JArray parametros = new JArray();
            JObject props = schema != null ? schema["properties"] as JObject : null;
            JArray required = schema != null ? schema["required"] as JArray : null;
            HashSet<string> obrigatorios = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (JToken req in required ?? new JArray())
            {
                obrigatorios.Add(Convert.ToString(req));
            }

            if (props == null)
            {
                return parametros;
            }

            foreach (JProperty p in props.Properties())
            {
                JObject meta = p.Value as JObject;
                JToken tipo = meta != null ? meta["type"] : null;
                parametros.Add(new JObject
                {
                    { "nome", p.Name },
                    { "tipo", TipoSchema(tipo) },
                    { "obrigatorio", obrigatorios.Contains(p.Name) },
                    { "descricao", meta != null ? Convert.ToString(meta["description"]) : string.Empty }
                });
            }

            return parametros;
        }

        private static string TipoSchema(JToken tipo)
        {
            if (tipo == null)
            {
                return "string";
            }

            JArray tipos = tipo as JArray;
            if (tipos == null)
            {
                return tipo.ToString();
            }

            List<string> partes = new List<string>();
            foreach (JToken t in tipos)
            {
                partes.Add(t != null ? t.ToString() : string.Empty);
            }
            return string.Join("|", partes.ToArray());
        }

        private static string ExecutarWorkflowComposto(int idConversaIA, IAFerramentaDefinicao ferramenta, JObject argumentos, IAFerramentaResultado resultado)
        {
            if (ferramenta == null || ferramenta.IdWorkflowIA <= 0)
            {
                resultado.Status = "ERRO";
                resultado.Erro = "Workflow inválido no catálogo.";
                return JsonErro(resultado.Erro);
            }

            IAWorkflowExecResponse resposta = new cls_IA_WorkflowExecucaoService().Iniciar(ferramenta.IdWorkflowIA, argumentos ?? new JObject(), idConversaIA);
            if (resposta == null)
            {
                resultado.Status = "ERRO";
                resultado.Erro = "Workflow não retornou resultado.";
                return JsonErro(resultado.Erro);
            }

            resultado.IdWorkflowExecucaoIA = resposta.IdExecucao;
            resultado.TipoPausaWorkflow = resposta.TipoPausa ?? string.Empty;
            resultado.ResumoAcao = string.IsNullOrWhiteSpace(resposta.Resumo) ? ferramenta.Nome : resposta.Resumo;

            bool pausado = string.Equals(resposta.Status, "PAUSADO", StringComparison.OrdinalIgnoreCase);
            bool concluido = string.Equals(resposta.Status, "CONCLUIDO", StringComparison.OrdinalIgnoreCase);
            resultado.Status = concluido ? "SUCESSO" : (pausado ? "PAUSADO" : "ERRO");
            if (!concluido && !pausado)
            {
                resultado.Erro = string.IsNullOrWhiteSpace(resposta.Mensagem) ? "Workflow interrompido." : resposta.Mensagem;
            }

            JObject diagnosticoErro = !concluido && !pausado
                ? cls_IA_WorkflowEngine.DiagnosticarErroWorkflow(resposta.Trace, resposta.Mensagem)
                : new JObject();
            if (diagnosticoErro.Count > 0 && !string.IsNullOrWhiteSpace(Convert.ToString(diagnosticoErro["detalheTecnico"])))
            {
                resultado.Erro = Convert.ToString(diagnosticoErro["detalheTecnico"]);
            }

            return new JObject
            {
                { "sucesso", concluido || pausado },
                { "workflow", ferramenta.Nome },
                { "idWorkflowIA", ferramenta.IdWorkflowIA },
                { "idExecucaoIA", resposta.IdExecucao },
                { "statusWorkflow", resposta.Status ?? string.Empty },
                { "tipoPausa", resposta.TipoPausa ?? string.Empty },
                { "dadosPausa", ParseArgumentos(resposta.DadosPausaJson) },
                { "mensagem", resposta.Mensagem ?? string.Empty },
                { "acaoPendente", pausado },
                { "resumo", resultado.ResumoAcao ?? string.Empty },
                { "trace", JArray.FromObject(resposta.Trace ?? new List<IAWorkflowTracePasso>()) },
                { "saidas", ParseArgumentos(resposta.SaidasJson) },
                { "diagnosticoErro", diagnosticoErro },
                { "instrucao", pausado ? InstrucaoWorkflowPausado(resposta.TipoPausa) : (!concluido ? InstrucaoWorkflowErro() : string.Empty) }
            }.ToString(Formatting.None);
        }

        private static string InstrucaoWorkflowErro()
        {
            return "O workflow falhou e não foi concluído. Explique ao usuário qual etapa falhou, a possível causa e como corrigir usando diagnosticoErro. Não afirme sucesso, não invente resultados posteriores e não reinicie o workflow automaticamente.";
        }

        private static string InstrucaoWorkflowPausado(string tipoPausa)
        {
            if (string.Equals(tipoPausa, "APROVACAO", StringComparison.OrdinalIgnoreCase))
            {
                return "O workflow pausou aguardando aprovação humana; não afirme que ele já foi concluído.";
            }
            if (string.Equals(tipoPausa, "ESPERA", StringComparison.OrdinalIgnoreCase))
            {
                return "O workflow pausou aguardando continuação ou data/evento; não afirme que ele já foi concluído.";
            }
            if (string.Equals(tipoPausa, "ENTRADAS", StringComparison.OrdinalIgnoreCase))
            {
                return "O workflow pausou aguardando dados do usuário; não invente valores ausentes.";
            }
            if (string.Equals(tipoPausa, "ESCOLHA", StringComparison.OrdinalIgnoreCase))
            {
                return "O workflow pausou aguardando uma escolha explícita do usuário; não selecione um candidato sozinho.";
            }
            return "O workflow pausou aguardando confirmação; não afirme que o passo já foi executado.";
        }

        // WRITE: nao executa; deixa a acao pendente de confirmacao (mesmo padrao dos Preparar*).
        private static string PrepararFerramentaGenerica(int idConversaIA, IAFerramentaDefinicao ferramenta, JObject argumentos, IAFerramentaResultado resultado)
        {
            if (string.IsNullOrWhiteSpace(ferramenta.ProcedureGenerica))
            {
                return JsonErro("Ferramenta generica sem procedure configurada.");
            }

            resultado.ArgumentosJson = argumentos.ToString(Formatting.None);
            resultado.Status = "PENDENTE_CONFIRMACAO";
            string resumoArgs = ResumirArgumentos(argumentos);
            resultado.ResumoAcao = "Executar a acao \"" + ferramenta.Nome + "\"" + (resumoArgs.Length > 0 ? " (" + resumoArgs + ")" : string.Empty);

            return new JObject
            {
                { "sucesso", true },
                { "acaoPendente", true },
                { "resumo", resultado.ResumoAcao },
                { "instrucao", "A acao ficou pendente de confirmacao; nao afirme que ja foi executada." }
            }.ToString(Formatting.None);
        }

        // WRITE interno confirmado: usado por workflows e pode ser reaproveitado pelo chat.
        public static bool ExecutarInternaConfirmada(string nomeFerramenta, JObject argumentos, int idConversaIA, out string erro)
        {
            JToken saida;
            return ExecutarInternaConfirmada(nomeFerramenta, argumentos, idConversaIA, out erro, out saida);
        }

        // WRITE interno confirmado: variante que devolve os dados gravados para o trace/workflow.
        public static bool ExecutarInternaConfirmada(string nomeFerramenta, JObject argumentos, int idConversaIA, out string erro, out JToken saida)
        {
            erro = string.Empty;
            saida = new JObject { { "executado", true } };

            if (!FUNCOES.ValidaPermissao(Permissao.IA.ExecutarAcoes, false))
            {
                erro = "Usuário sem permissão para executar ações pela IA.";
                return false;
            }

            if (string.Equals(nomeFerramenta, "orcamento_criar_cabecalho", StringComparison.OrdinalIgnoreCase))
            {
                return ExecutarOrcamentoCriarCabecalhoConfirmado(argumentos, out erro, out saida);
            }

            if (string.Equals(nomeFerramenta, "orcamento_duplicar", StringComparison.OrdinalIgnoreCase))
            {
                return ExecutarOrcamentoDuplicarConfirmado(argumentos, out erro, out saida);
            }

            if (string.Equals(nomeFerramenta, "orcamento_adicionar_item", StringComparison.OrdinalIgnoreCase))
            {
                return ExecutarOrcamentoAdicionarItemConfirmado(argumentos, out erro, out saida);
            }

            if (string.Equals(nomeFerramenta, "orcamento_salvar_resposta_escopo", StringComparison.OrdinalIgnoreCase))
            {
                return ExecutarOrcamentoSalvarRespostaEscopoConfirmado(argumentos, out erro, out saida);
            }

            if (string.Equals(nomeFerramenta, "clientes_criar_para_orcamento", StringComparison.OrdinalIgnoreCase))
            {
                return ExecutarClientesCriarParaOrcamentoConfirmado(argumentos, out erro, out saida);
            }

            if (string.Equals(nomeFerramenta, "clientes_completar_para_orcamento", StringComparison.OrdinalIgnoreCase))
            {
                return ExecutarClientesCompletarParaOrcamentoConfirmado(argumentos, out erro, out saida);
            }

            if (string.Equals(nomeFerramenta, "crm_negocio_criar_para_orcamento", StringComparison.OrdinalIgnoreCase))
            {
                return ExecutarCrmNegocioCriarParaOrcamentoConfirmado(argumentos, out erro, out saida);
            }

            if (string.Equals(nomeFerramenta, "crm_followup_criar", StringComparison.OrdinalIgnoreCase))
            {
                return ExecutarCrmFollowupCriarConfirmado(argumentos, out erro, out saida);
            }

            if (string.Equals(nomeFerramenta, "pedidos_adicionar_observacao", StringComparison.OrdinalIgnoreCase))
            {
                int idPedido = Inteiro(argumentos, "idPedido", 0);
                string observacao = Texto(argumentos, "sObservacao", 500);

                if (idPedido <= 0)
                {
                    erro = "idPedido invalido.";
                    return false;
                }

                if (observacao.Length < 5)
                {
                    erro = "Informe uma observacao com ao menos 5 caracteres.";
                    return false;
                }

                string erroResolucao = ResolverPedidoOuErro(idPedido, out idPedido);
                if (erroResolucao != null)
                {
                    erro = MensagemErroJson(erroResolucao, "Pedido nao localizado.");
                    return false;
                }

                Dictionary<string, string> parametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTA PEDIDO" },
                    { "@idPedido", idPedido.ToString() },
                    { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                };

                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", parametros);
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    erro = "Pedido nao encontrado ou sem acesso para consulta.";
                    return false;
                }

                int idTipo = Inteiro(Valor(ds.Tables[0].Rows[0], "idTipo"), 0);
                if (idTipo == 7 && !FUNCOES.ValidaPermissao(Permissao.Compras.Pedido_Compras.Consultar, false))
                {
                    erro = "Usuario sem permissao para pedido de compras.";
                    return false;
                }

                if (!new cls_IA_Repositorio().AdicionarObservacaoPedido(idPedido, observacao))
                {
                    erro = "Pedido nao localizado.";
                    return false;
                }

                return true;
            }

            if (string.Equals(nomeFerramenta, "mensagens_criar_rascunho", StringComparison.OrdinalIgnoreCase))
            {
                string assunto = Texto(argumentos, "assunto", 200);
                string corpo = Texto(argumentos, "corpo", 4000);
                string destino = Texto(argumentos, "destino", 200);

                if (assunto.Length < 3)
                {
                    erro = "Informe um assunto com ao menos 3 caracteres.";
                    return false;
                }

                if (corpo.Length < 10)
                {
                    erro = "Informe o corpo do rascunho com ao menos 10 caracteres.";
                    return false;
                }

                if (!FUNCOES.ValidaPermissao(Permissao.Mensagens.Consultar, false))
                {
                    erro = "Usuario sem permissao no modulo de mensagens.";
                    return false;
                }

                if (new cls_IA_Repositorio().SalvarRascunho(idConversaIA, destino, assunto, corpo) <= 0)
                {
                    erro = "Nao foi possivel salvar o rascunho.";
                    return false;
                }

                return true;
            }

            erro = "Ferramenta de acao nao suportada.";
            return false;
        }

        // WRITE confirmado: chamado por cls_IA_Chat.ExecutarAcaoAprovada apos o usuario confirmar.
        public static bool ExecutarGenericaConfirmada(IAFerramentaDefinicao ferramenta, JObject argumentos, out string erro)
        {
            JToken saida;
            return ExecutarGenericaConfirmada(ferramenta, argumentos, out erro, out saida);
        }

        // Mantem o retorno seguro da escrita: metadados basicos sempre saem e, quando a ferramenta
        // possui colunas configuradas, somente essa allowlist e projetada para a conversa.
        public static bool ExecutarGenericaConfirmada(IAFerramentaDefinicao ferramenta, JObject argumentos, out string erro, out JToken saida)
        {
            erro = string.Empty;
            saida = new JObject { { "executado", false } };
            if (ferramenta == null)
            {
                erro = "Ferramenta generica invalida.";
                return false;
            }

            string procedure = (ferramenta.ProcedureGenerica ?? string.Empty).Trim();
            if (procedure.Length == 0)
            {
                erro = "Ferramenta generica sem procedure.";
                return false;
            }

            Dictionary<string, string> parametros = MontarParametrosGenerico(ferramenta.MapaParametros, argumentos);
            try
            {
                DataSet ds = BD.ExecutarDataSet(procedure, parametros);
                // Se a SP devolver nRet/sMsg (padrao das sp_Manipula), respeita o erro
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    if (row.Table.Columns.Contains("nRet"))
                    {
                        int nRet;
                        int.TryParse(Convert.ToString(row["nRet"]), out nRet);
                        if (nRet != 0)
                        {
                            erro = row.Table.Columns.Contains("sMsg") ? Convert.ToString(row["sMsg"]) : "A procedure retornou erro.";
                            return false;
                        }
                    }
                }

                JObject retorno = new JObject
                {
                    { "executado", true },
                    { "ferramenta", ferramenta.Nome ?? string.Empty }
                };

                List<string> colunas = new List<string>();
                foreach (JToken col in ParseArrayGenerico(ferramenta.ColunasRetorno))
                {
                    string nome = col != null ? col.ToString().Trim() : string.Empty;
                    if (nome.Length > 0)
                    {
                        colunas.Add(nome);
                    }
                }

                if (ds != null && ds.Tables.Count > 0 && colunas.Count > 0)
                {
                    int indice = ferramenta.IndiceTabelaRetorno;
                    if (indice < 0 || indice >= ds.Tables.Count)
                    {
                        indice = 0;
                    }

                    int limite = ferramenta.MaxRegistros > 0 ? ferramenta.MaxRegistros : 20;
                    retorno["retorno"] = JObject.Parse(TabelaParaJson(
                        ferramenta,
                        ds.Tables[indice],
                        limite,
                        colunas.ToArray(),
                        null));
                }

                saida = retorno;
                return true;
            }
            catch (Exception ex)
            {
                erro = ex.Message;
                return false;
            }
        }

        // READ generico aprovado pelo usuario: roda a consulta agora e devolve o JSON de dados para a
        // IA responder. Reusa o mesmo motor da execucao inline (whitelist de colunas + mascara). Erro
        // (excecao) vira null + mensagem; a SP tambem pode devolver um JSON de erro no proprio retorno.
        public string ExecutarLeituraGenericaConfirmada(int idConversaIA, IAFerramentaDefinicao ferramenta, JObject argumentos, out string erro)
        {
            erro = string.Empty;
            if (ferramenta == null || !ferramenta.EhGenerica())
            {
                erro = "Ferramenta invalida para leitura confirmada.";
                return null;
            }

            try
            {
                return ExecutarFerramentaGenerica(idConversaIA, ferramenta, argumentos ?? new JObject());
            }
            catch (Exception ex)
            {
                erro = ex.Message;
                return null;
            }
        }

        // Monta os parametros da SP a partir do mapa configurado. Cada entrada:
        // { "param": "@x", "origem": "arg"|"fixo"|"token", "valor": "<nome do arg | valor fixo | nome do token>" }
        private static Dictionary<string, string> MontarParametrosGenerico(string mapaJson, JObject argumentos)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            foreach (JToken item in ParseArrayGenerico(mapaJson))
            {
                JObject entrada = item as JObject;
                if (entrada == null)
                {
                    continue;
                }

                string param = entrada["param"] != null ? entrada["param"].ToString().Trim() : string.Empty;
                if (param.Length == 0)
                {
                    continue;
                }
                if (!param.StartsWith("@"))
                {
                    param = "@" + param;
                }

                string origem = (entrada["origem"] != null ? entrada["origem"].ToString() : string.Empty).Trim().ToLowerInvariant();
                string valorCfg = entrada["valor"] != null ? entrada["valor"].ToString() : string.Empty;

                string valor;
                if (origem == "fixo")
                {
                    valor = Cortar(valorCfg, 4000);
                }
                else if (origem == "token")
                {
                    valor = ResolverTokenGenerico(valorCfg);
                }
                else // "arg": pega o valor do argumento informado pela IA cujo nome esta em valorCfg
                {
                    // Case-insensitive de proposito: o nome do argumento e digitado na tela em dois lugares
                    // (schema e mapa) e divergir na caixa devolveria vazio sem ninguem perceber.
                    JToken tokenArg = null;
                    string nomeArg = valorCfg.Trim();
                    if (nomeArg.Length > 0 && argumentos != null)
                    {
                        argumentos.TryGetValue(nomeArg, StringComparison.OrdinalIgnoreCase, out tokenArg);
                    }

                    valor = (tokenArg != null && tokenArg.Type != JTokenType.Null)
                        ? Cortar(tokenArg.ToString(), 4000)
                        : string.Empty;
                }

                parametros[param] = valor;
            }

            return parametros;
        }

        private static string ResolverTokenGenerico(string token)
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

        private static string ResumirArgumentos(JObject argumentos)
        {
            if (argumentos == null)
            {
                return string.Empty;
            }

            List<string> partes = new List<string>();
            foreach (JProperty p in argumentos.Properties())
            {
                string v = p.Value != null ? p.Value.ToString() : string.Empty;
                if (v.Length > 60)
                {
                    v = v.Substring(0, 60) + "...";
                }
                partes.Add(p.Name + "=" + v);
            }

            return string.Join(", ", partes);
        }

        private static JArray ParseArrayGenerico(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new JArray();
            }

            try { return JArray.Parse(json); }
            catch { return new JArray(); }
        }

        private static string Cortar(string valor, int max)
        {
            valor = valor ?? string.Empty;
            return valor.Length > max ? valor.Substring(0, max) : valor;
        }

        // Botao "Testar" da tela: READ executa e devolve amostra; WRITE (simular=true) so resolve os params.
        public static IAResultadoTeste TestarGenerica(IAFerramentaDefinicao ferramenta, JObject argumentos, bool simular)
        {
            IAResultadoTeste r = new IAResultadoTeste();
            string procedure = (ferramenta.ProcedureGenerica ?? string.Empty).Trim();
            if (procedure.Length == 0)
            {
                r.Mensagem = "Informe a procedure.";
                return r;
            }

            Dictionary<string, string> parametros = MontarParametrosGenerico(ferramenta.MapaParametros, argumentos);

            List<string> partesParam = new List<string>();
            foreach (KeyValuePair<string, string> kv in parametros)
            {
                partesParam.Add(kv.Key + " = " + kv.Value);
            }
            r.ParametrosResolvidos = string.Join(" | ", partesParam);

            if (simular)
            {
                r.Simulacao = true;
                r.Sucesso = true;
                r.Mensagem = "Simulacao: a procedure " + procedure + " seria chamada com os parametros abaixo (nada foi executado).";
                return r;
            }

            try
            {
                DataSet ds = BD.ExecutarDataSet(procedure, parametros);
                if (ds == null || ds.Tables.Count == 0)
                {
                    r.Mensagem = "A procedure nao retornou nenhuma tabela.";
                    return r;
                }

                int indice = ferramenta.IndiceTabelaRetorno;
                if (indice < 0 || indice >= ds.Tables.Count)
                {
                    indice = 0;
                }

                DataTable tb = ds.Tables[indice];
                r.TotalEncontrado = tb.Rows.Count;

                foreach (JToken col in ParseArrayGenerico(ferramenta.ColunasRetorno))
                {
                    string nome = col != null ? col.ToString().Trim() : string.Empty;
                    if (nome.Length > 0 && tb.Columns.Contains(nome))
                    {
                        r.Colunas.Add(nome);
                    }
                }

                if (r.Colunas.Count == 0)
                {
                    r.Mensagem = "Nenhuma das colunas configuradas existe no retorno da procedure (tabela " + indice + ").";
                    return r;
                }

                int amostra = Math.Min(tb.Rows.Count, 10);
                for (int i = 0; i < amostra; i++)
                {
                    Dictionary<string, string> linha = new Dictionary<string, string>();
                    foreach (string c in r.Colunas)
                    {
                        string val = Valor(tb.Rows[i], c);
                        linha[c] = EhColunaDocumento(c) ? MascararDocumento(val) : val;
                    }
                    r.Linhas.Add(linha);
                }

                r.Sucesso = true;
                r.Mensagem = "OK - " + r.TotalEncontrado + " linha(s) encontrada(s), mostrando " + r.Linhas.Count + ".";
            }
            catch (Exception ex)
            {
                r.Mensagem = "Erro ao executar: " + ex.Message;
            }

            return r;
        }

        // Busca na base de conhecimento (arquivos sTipo='B' + FAQ, global). Sem escopo de conversa.
        // Com re-rank habilitado, recupera MAIS candidatos que o limite (lexical) e pede ao LLM para
        // escolher os mais relevantes — melhora a precisao sem infra nova. Falha do re-rank nunca
        // derruba a busca (fallback para a ordem lexical).
        private string ExecutarConhecimentoBuscar(int idConversaIA, IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            string termo = Texto(argumentos, "termo", 200);
            string categoria = Texto(argumentos, "categoria", 100);
            int limite = Limite(argumentos, Math.Min(ferramenta.MaxRegistros, _maxTrechosArquivos));

            cls_IA_Config config = cls_IA_Config.Carregar();
            bool rerank = config != null && config.ConhecimentoRerankHabilitado && !string.IsNullOrWhiteSpace(termo);
            int alvoBusca = rerank
                ? Math.Min(50, Math.Max(limite * 3, config.ConhecimentoRerankCandidatos))
                : limite;

            List<JObject> candidatos = new List<JObject>();

            // Permissionamento da FAQ (mesma logica das ferramentas): cada FAQ so entra se o usuario
            // tiver o recurso mapeado a categoria dela. Categoria sem mapa -> exige a permissao base de
            // FAQ (219). Cache por recurso para nao repetir ValidaPermissao.
            Dictionary<int, int> mapaFaqCatRecurso = ParsearMapaCategoriaRecurso(config != null ? config.FaqCategoriaRecursoJson : null);
            Dictionary<int, bool> cacheFaqPermissao = new Dictionary<int, bool>();
            int faqBloqueadas = 0;

            // 1) FAQ aprovada da casa (curada) primeiro: e a resposta revisada por humano. O CONSULTAR do
            // SP so busca em titulo+tag, entao puxamos todas as aprovadas e casamos aqui por palavra-chave
            // (titulo+tags+corpo, sem acento/case) para achar tambem pelo texto da resposta.
            int totalFaq = 0;
            foreach (DataRow row in RankearFaq(_repositorio.ListarFaqAprovada(), termo, alvoBusca))
            {
                if (candidatos.Count >= alvoBusca)
                {
                    break;
                }

                if (!UsuarioPodeVerFaq(row, mapaFaqCatRecurso, cacheFaqPermissao))
                {
                    faqBloqueadas++;
                    continue;
                }

                // sCorpo vem do editor rico (HTML) -> converte para texto limpo antes de mandar a IA.
                candidatos.Add(new JObject
                {
                    { "titulo", "FAQ" },
                    { "sTituloTrecho", Valor(row, "sTitulo") },
                    { "sConteudoMarkdown", cls_IA_Sanitizacao.Resumir(RemoverHtml(Valor(row, "sCorpo")), 900) },
                    { "tipo", "FAQ" }
                });
                totalFaq++;
            }

            // 2) Documentos da base (full-text/LIKE) preenchem o restante dos candidatos.
            DataTable tbDocs = _repositorio.ConsultarConhecimento(termo, categoria, alvoBusca);
            if (tbDocs != null)
            {
                foreach (DataRow row in tbDocs.Rows)
                {
                    if (candidatos.Count >= alvoBusca)
                    {
                        break;
                    }

                    candidatos.Add(new JObject
                    {
                        { "titulo", Valor(row, "sFonte") },
                        { "sTituloTrecho", Valor(row, "sTitulo") },
                        { "sConteudoMarkdown", cls_IA_Sanitizacao.Resumir(Valor(row, "sTexto"), 900) },
                        { "tipo", "Conhecimento" }
                    });
                }
            }

            // 3) Re-rank pelo LLM (best-effort): reordena os candidatos por relevancia real ao termo.
            bool rerankAplicado = false;
            if (rerank && candidatos.Count > limite)
            {
                Stopwatch cronometroRerank = Stopwatch.StartNew();
                List<int> ordem = RerankearPorLLM(config, termo, candidatos, limite);
                cronometroRerank.Stop();

                if (ordem != null && ordem.Count > 0)
                {
                    List<JObject> reordenados = new List<JObject>();
                    foreach (int idx in ordem)
                    {
                        if (idx >= 0 && idx < candidatos.Count && !reordenados.Contains(candidatos[idx]))
                        {
                            reordenados.Add(candidatos[idx]);
                        }
                    }
                    // Completa com os demais na ordem lexical (caso o LLM devolva menos que o limite).
                    foreach (JObject c in candidatos)
                    {
                        if (!reordenados.Contains(c))
                        {
                            reordenados.Add(c);
                        }
                    }
                    candidatos = reordenados;
                    rerankAplicado = true;
                }

                cls_IA_Auditoria.Registrar(idConversaIA, "CONHECIMENTO_RERANK", rerankAplicado ? "INFO" : "WARN", new
                {
                    termo = termo,
                    candidatos = candidatos.Count,
                    aplicado = rerankAplicado,
                    duracaoMs = cronometroRerank.ElapsedMilliseconds
                });
            }

            // 4) Corte final + fontes distintas retornadas (auditoria: depurar "por que nao achou X").
            JArray registros = new JArray();
            List<string> fontesRecuperadas = new List<string>();
            foreach (JObject c in candidatos)
            {
                if (registros.Count >= limite)
                {
                    break;
                }

                registros.Add(c);

                string fonte = string.Equals((string)c["tipo"], "FAQ", StringComparison.OrdinalIgnoreCase)
                    ? "FAQ: " + cls_IA_Sanitizacao.Resumir((string)c["sTituloTrecho"], 120)
                    : (string)c["titulo"];
                if (!string.IsNullOrEmpty(fonte) && !fontesRecuperadas.Contains(fonte))
                {
                    fontesRecuperadas.Add(fonte);
                }
            }

            cls_IA_Auditoria.Registrar(idConversaIA, "CONHECIMENTO_USADO", "INFO", new
            {
                termo = termo,
                categoria = categoria,
                totalFaq = totalFaq,
                faqBloqueadasPermissao = faqBloqueadas,
                totalCandidatos = candidatos.Count,
                rerank = rerankAplicado,
                fontes = fontesRecuperadas,
                total = registros.Count
            });

            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "totalEncontrado", registros.Count },
                { "totalRetornado", registros.Count },
                { "registros", registros }
            }.ToString(Formatting.None);
        }

        // Mapa JSON "idCategoria":idRecurso -> Dictionary. Entradas invalidas sao ignoradas (fail-safe).
        private static Dictionary<int, int> ParsearMapaCategoriaRecurso(string json)
        {
            Dictionary<int, int> mapa = new Dictionary<int, int>();
            if (string.IsNullOrWhiteSpace(json))
            {
                return mapa;
            }

            try
            {
                JObject obj = JObject.Parse(json);
                foreach (JProperty p in obj.Properties())
                {
                    int categoria, recurso;
                    if (int.TryParse(p.Name, out categoria) && int.TryParse(p.Value != null ? p.Value.ToString() : string.Empty, out recurso) && recurso > 0)
                    {
                        mapa[categoria] = recurso;
                    }
                }
            }
            catch
            {
                // JSON invalido -> mapa vazio: toda FAQ cai no recurso base (219), nao vaza por engano.
            }

            return mapa;
        }

        // Uma FAQ so aparece se o usuario tem o recurso da categoria dela; categoria sem mapa exige a
        // permissao base de FAQ (219). Cache por recurso para nao repetir ValidaPermissao na mesma busca.
        private static bool UsuarioPodeVerFaq(DataRow row, Dictionary<int, int> mapaCatRecurso, Dictionary<int, bool> cachePermissao)
        {
            int idCategoria;
            int.TryParse(Valor(row, "idCategoria"), out idCategoria);

            int recurso;
            if (!mapaCatRecurso.TryGetValue(idCategoria, out recurso) || recurso <= 0)
            {
                recurso = Permissao.FAQ.Consultar; // 219 = piso para categorias sem mapa
            }

            bool permitido;
            if (!cachePermissao.TryGetValue(recurso, out permitido))
            {
                permitido = FUNCOES.ValidaPermissao(recurso, false);
                cachePermissao[recurso] = permitido;
            }

            return permitido;
        }

        // Pede ao provider ativo os indices (0-based no retorno) dos candidatos mais relevantes.
        // Retorna null em QUALQUER falha — o chamador segue com a ordem lexical. A chamada e leve
        // (sem ferramentas, sem historico, ~100 tokens de saida) e usa apenas trechos que ja seriam
        // enviados ao provider de qualquer forma no tool_result (mesma exposicao do chat).
        private static List<int> RerankearPorLLM(cls_IA_Config config, string termo, List<JObject> candidatos, int limite)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine("Termo de busca: " + termo);
                sb.AppendLine("Trechos candidatos:");
                for (int i = 0; i < candidatos.Count; i++)
                {
                    string fonte = (string)candidatos[i]["titulo"];
                    string tituloTrecho = (string)candidatos[i]["sTituloTrecho"];
                    string conteudo = cls_IA_Sanitizacao.Resumir((string)candidatos[i]["sConteudoMarkdown"], 300);
                    sb.AppendLine("[" + (i + 1) + "] (" + fonte + " - " + tituloTrecho + ") " + conteudo);
                }
                sb.AppendLine();
                sb.Append("Responda SOMENTE com os numeros dos ate " + limite +
                          " trechos mais relevantes para o termo de busca, em ordem de relevancia, separados por virgula. Sem texto adicional.");

                IIAProvider provider = cls_IA_ProviderFactory.Criar(config);
                IAProviderResponse resposta = provider.GerarResposta(new IAProviderRequest
                {
                    Modelo = config.ModeloPadrao,
                    Instrucoes = "Voce e um re-rankeador de resultados de busca. Responda apenas com numeros separados por virgula.",
                    MensagemUsuario = sb.ToString(),
                    MaxTokensSaida = 100,
                    ForcarSemFerramentas = true,
                    StoreExterno = config.StoreExterno
                });

                if (resposta == null || !resposta.Sucesso || string.IsNullOrWhiteSpace(resposta.Resposta))
                {
                    return null;
                }

                List<int> indices = new List<int>();
                foreach (System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(resposta.Resposta, @"\d+"))
                {
                    int n;
                    if (int.TryParse(m.Value, out n) && n >= 1 && n <= candidatos.Count && !indices.Contains(n - 1))
                    {
                        indices.Add(n - 1);
                        if (indices.Count >= limite)
                        {
                            break;
                        }
                    }
                }

                return indices.Count > 0 ? indices : null;
            }
            catch
            {
                return null;
            }
        }

        private string ExecutarArquivosBuscarTrechos(int idConversaIA, IAFerramentaDefinicao ferramenta, JObject argumentos)
        {
            List<int> idsSolicitados = IdsInteiros(argumentos, "idsArquivos");
            List<int> idsPermitidos = IntersectarIdsPermitidos(idsSolicitados);
            string termo = Texto(argumentos, "termo", 120);
            int limite = Limite(argumentos, Math.Min(ferramenta.MaxRegistros, _maxTrechosArquivos));

            if (idsPermitidos.Count == 0)
            {
                return JsonErro("Nenhum arquivo solicitado esta vinculado a esta mensagem ou permitido para o usuario.");
            }

            List<IAArquivoTrecho> trechos = _repositorio.ConsultarTrechosArquivos(idConversaIA, idsPermitidos, termo, limite);
            JArray registros = new JArray();

            foreach (IAArquivoTrecho trecho in trechos)
            {
                registros.Add(new JObject
                {
                    { "idArquivoIA", trecho.IdArquivoIA },
                    { "idArquivoTrechoIA", trecho.IdArquivoTrechoIA },
                    { "sNomeArquivo", trecho.NomeOriginal ?? string.Empty },
                    { "sHashSHA256", trecho.HashSHA256 ?? string.Empty },
                    { "nOrdem", trecho.Ordem },
                    { "sTituloTrecho", trecho.Titulo ?? string.Empty },
                    { "sConteudoMarkdown", trecho.ConteudoMarkdown ?? string.Empty },
                    { "nInicioChar", trecho.InicioChar },
                    { "nFimChar", trecho.FimChar },
                    { "tipo", "Arquivo" }
                });
            }

            cls_IA_Auditoria.Registrar(idConversaIA, "ARQUIVO_TRECHOS_USADOS", "INFO", new
            {
                idsArquivos = idsPermitidos,
                termo = termo,
                totalTrechos = registros.Count
            });

            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "totalEncontrado", registros.Count },
                { "totalRetornado", registros.Count },
                { "registros", registros }
            }.ToString(Formatting.None);
        }

        private static JObject ArgsPesquisa(string termo, int limite)
        {
            return new JObject
            {
                { "termo", termo ?? string.Empty },
                { "limite", limite }
            };
        }

        private static bool DeveBuscar(string tipoSolicitado, string tipoAlvo)
        {
            return string.Equals(tipoSolicitado, "TODOS", StringComparison.OrdinalIgnoreCase)
                || string.Equals(tipoSolicitado, tipoAlvo, StringComparison.OrdinalIgnoreCase);
        }

        private static void AdicionarRegistrosBuscaUniversal(JArray destino, string tipo, string origem, string jsonResultado, int limiteTotal)
        {
            if (destino == null || destino.Count >= limiteTotal || string.IsNullOrWhiteSpace(jsonResultado))
            {
                return;
            }

            JObject obj;
            try
            {
                obj = JObject.Parse(jsonResultado);
            }
            catch
            {
                return;
            }

            if (obj["sucesso"] != null && obj["sucesso"].Type == JTokenType.Boolean && !obj["sucesso"].Value<bool>())
            {
                return;
            }

            JArray registros = obj["registros"] as JArray;
            if (registros == null)
            {
                return;
            }

            foreach (JToken registro in registros)
            {
                if (destino.Count >= limiteTotal)
                {
                    break;
                }

                JObject item = registro as JObject;
                if (item == null)
                {
                    continue;
                }

                JObject normalizado = (JObject)item.DeepClone();
                normalizado["tipo"] = tipo;
                normalizado["origem"] = origem;
                AplicarCamposBuscaUniversal(normalizado, tipo);
                destino.Add(normalizado);
            }
        }

        private static void AplicarCamposBuscaUniversal(JObject item, string tipo)
        {
            string titulo = PrimeiroValor(item,
                "titulo",
                "sDscRecurso",
                "sDscProduto",
                "sRazaoSocial",
                "sNomeFantasia",
                "sAssunto",
                "nNumeroPedido",
                "sPedidoCompras",
                "nControleTT",
                "idPedido",
                "idItem",
                "idCliente",
                "idMensagem");

            if (string.Equals(tipo, "Pedido", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(titulo)
                && !titulo.StartsWith("Pedido ", StringComparison.OrdinalIgnoreCase))
            {
                titulo = "Pedido " + titulo;
            }

            string descricao = DescricaoBuscaUniversal(item, tipo);
            string link = PrimeiroValor(item, "link", "sURL", "url");

            item["titulo"] = cls_IA_Sanitizacao.Resumir(titulo, 140);
            item["descricao"] = cls_IA_Sanitizacao.Resumir(descricao, 240);

            if (!string.IsNullOrWhiteSpace(link))
            {
                item["link"] = link;
            }
        }

        private static string DescricaoBuscaUniversal(JObject item, string tipo)
        {
            List<string> partes = new List<string>();

            if (string.Equals(tipo, "Menu", StringComparison.OrdinalIgnoreCase))
            {
                AdicionarParte(partes, "URL", PrimeiroValor(item, "sURL", "link"));
            }
            else if (string.Equals(tipo, "Produto", StringComparison.OrdinalIgnoreCase))
            {
                AdicionarParte(partes, "Codigo", PrimeiroValor(item, "sCodigo"));
                AdicionarParte(partes, "Status", PrimeiroValor(item, "sSituacao_Completa", "sSituacaoCadastral_Completa"));
            }
            else if (string.Equals(tipo, "Parceiro", StringComparison.OrdinalIgnoreCase))
            {
                AdicionarParte(partes, "Fantasia", PrimeiroValor(item, "sNomeFantasia"));
                AdicionarParte(partes, "Documento", PrimeiroValor(item, "sCPF_CNPJ"));
                AdicionarParte(partes, "Status", PrimeiroValor(item, "sDscTipoSituacaoCliente"));
            }
            else if (string.Equals(tipo, "Mensagem", StringComparison.OrdinalIgnoreCase))
            {
                AdicionarParte(partes, "Remetente", PrimeiroValor(item, "sDscUsuarioRemetente"));
                AdicionarParte(partes, "Data", PrimeiroValor(item, "dtInclusao"));
            }
            else if (string.Equals(tipo, "Pedido", StringComparison.OrdinalIgnoreCase))
            {
                AdicionarParte(partes, "Cliente", PrimeiroValor(item, "sRazaoSocial", "sNomeFantasia"));
                AdicionarParte(partes, "Status", PrimeiroValor(item, "sStatus", "sDscStatus"));
                AdicionarParte(partes, "Atualizado", PrimeiroValor(item, "dtAtualizacao", "dtCadastro"));
            }

            return string.Join(" | ", partes.ToArray());
        }

        private static void AdicionarParte(List<string> partes, string label, string valor)
        {
            if (!string.IsNullOrWhiteSpace(valor))
            {
                partes.Add(label + ": " + valor);
            }
        }

        private static string PrimeiroValor(JObject item, params string[] campos)
        {
            foreach (string campo in campos)
            {
                if (item != null && item[campo] != null)
                {
                    string valor = item[campo].ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        return valor;
                    }
                }
            }

            return string.Empty;
        }

        private static DataTable Tabela(DataSet ds, int indice)
        {
            if (ds == null || indice < 0 || ds.Tables.Count <= indice)
            {
                return null;
            }

            return ds.Tables[indice];
        }

        private static JArray TabelaParaArray(DataTable tb, int limite, string[] colunasPermitidas, string linkTemplate)
        {
            JArray registros = new JArray();
            int totalRetornado = 0;

            if (tb == null)
            {
                return registros;
            }

            foreach (DataRow row in tb.Rows)
            {
                if (limite > 0 && totalRetornado >= limite)
                {
                    break;
                }

                JObject item = new JObject();
                AdicionarColunas(item, row, colunasPermitidas);

                string link = MontarLink(row, linkTemplate);
                if (!string.IsNullOrWhiteSpace(link))
                {
                    item["link"] = link;
                }

                registros.Add(item);
                totalRetornado++;
            }

            return registros;
        }

        private static JObject ObjetoLinha(DataRow row, string[] colunasPermitidas, string linkTemplate)
        {
            return ObjetoLinha(row, colunasPermitidas, linkTemplate, false);
        }

        private static JObject ObjetoLinha(DataRow row, string[] colunasPermitidas, string linkTemplate, bool preservarTextoCompleto)
        {
            JObject item = new JObject();
            if (row == null)
            {
                return item;
            }

            AdicionarColunas(item, row, colunasPermitidas, preservarTextoCompleto);

            string link = MontarLink(row, linkTemplate);
            if (!string.IsNullOrWhiteSpace(link))
            {
                item["link"] = link;
            }

            return item;
        }

        private static string TabelaParaJson(IAFerramentaDefinicao ferramenta, DataTable tb, int limite, string[] colunasPermitidas, string linkTemplate)
        {
            JArray registros = new JArray();
            int totalEncontrado = tb != null ? tb.Rows.Count : 0;
            int totalRetornado = 0;

            if (tb != null)
            {
                foreach (DataRow row in tb.Rows)
                {
                    if (totalRetornado >= limite)
                    {
                        break;
                    }

                    JObject item = new JObject();
                    AdicionarColunas(item, row, colunasPermitidas);

                    string link = MontarLink(row, linkTemplate);
                    if (!string.IsNullOrWhiteSpace(link))
                    {
                        item["link"] = link;
                    }

                    registros.Add(item);
                    totalRetornado++;
                }
            }

            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "totalEncontrado", totalEncontrado },
                { "totalRetornado", totalRetornado },
                { "registros", registros }
            }.ToString(Formatting.None);
        }

        private static string TabelaResolucaoParaJson(IAFerramentaDefinicao ferramenta, DataTable tb, int limite, string termo, string[] colunasPermitidas, string[] colunasRanking, string linkTemplate)
        {
            List<KeyValuePair<int, DataRow>> linhas = RankearTabelaResolucao(tb, termo, colunasRanking);
            JArray registros = new JArray();
            int totalRetornado = 0;

            foreach (KeyValuePair<int, DataRow> linha in linhas)
            {
                if (totalRetornado >= limite)
                {
                    break;
                }

                JObject item = new JObject();
                AdicionarColunas(item, linha.Value, colunasPermitidas);
                if (item["idProduto"] == null && linha.Value.Table.Columns.Contains("idItem"))
                {
                    item["idProduto"] = Inteiro(Valor(linha.Value, "idItem"), 0);
                }
                item["scoreResolucao"] = linha.Key;

                string link = MontarLink(linha.Value, linkTemplate);
                if (!string.IsNullOrWhiteSpace(link))
                {
                    item["link"] = link;
                }

                registros.Add(item);
                totalRetornado++;
            }

            int primeiroScore = linhas.Count > 0 ? linhas[0].Key : 0;
            int segundoScore = linhas.Count > 1 ? linhas[1].Key : 0;
            bool unicoProvavel = (linhas.Count == 1 && primeiroScore >= 70) || (primeiroScore >= 100 && segundoScore < 100);
            bool precisaEscolha = linhas.Count > 0 && !unicoProvavel;
            // Abaixo de 70 nem o melhor candidato contem o termo por inteiro (so palavras parecidas ou incompletas):
            // vira "voce quis dizer...?", nunca escolha silenciosa.
            bool aproximado = linhas.Count > 0 && primeiroScore < 70;
            JArray opcoes = new JArray();
            foreach (JToken token in registros)
            {
                JObject registro = token as JObject;
                if (registro == null) continue;
                opcoes.Add(new JObject
                {
                    { "valor", ValorOpcaoResolucao(registro) },
                    { "rotulo", RotuloOpcaoResolucao(registro) },
                    { "descricao", DescricaoOpcaoResolucao(registro) },
                    { "registro", registro.DeepClone() }
                });
            }

            return new JObject
            {
                { "sucesso", true },
                { "ferramenta", ferramenta.Nome },
                { "termo", termo ?? string.Empty },
                { "totalEncontrado", linhas.Count },
                { "totalRetornado", totalRetornado },
                { "encontrado", linhas.Count > 0 && !precisaEscolha },
                { "unicoProvavel", unicoProvavel },
                { "precisaEscolha", precisaEscolha },
                { "aproximado", aproximado },
                { "melhorResultado", registros.Count > 0 ? registros[0].DeepClone() : new JObject() },
                { "registros", registros },
                { "opcoes", opcoes },
                { "mensagem", MensagemResolucao(linhas.Count, unicoProvavel, aproximado) }
            }.ToString(Formatting.None);
        }

        private static string ValorOpcaoResolucao(JObject registro)
        {
            string[] campos = { "idCliente", "idEmpresa", "idVendedor", "idUsuario", "idCondicaoPagamento", "idProduto", "idItem", "idTabelaPreco", "idTabela", "idTipoEnvio", "idTipoOrcamento", "id" };
            foreach (string campo in campos)
            {
                string valor = StrJ(registro[campo]).Trim();
                if (valor.Length > 0 && valor != "0") return campo + ":" + valor;
            }
            return registro.ToString(Formatting.None);
        }

        private static string RotuloOpcaoResolucao(JObject registro)
        {
            string[] campos = { "sRazaoSocial", "sNomeFantasia", "sDscUsuario", "sNome", "nome", "sDscTabela", "sDscTipoOrcamento", "sDscTipoEnvio", "sDscCondicaoPagamento", "sDscEmpresa", "sDscEmpresaReduzida", "sDscParceiro", "sDscProduto", "sDescricao", "descricao", "sCodigoComDescricao", "sCodigo" };
            foreach (string campo in campos)
            {
                string valor = StrJ(registro[campo]).Trim();
                if (valor.Length > 0) return valor;
            }
            return ValorOpcaoResolucao(registro);
        }

        private static string DescricaoOpcaoResolucao(JObject registro)
        {
            List<string> partes = new List<string>();
            string codigo = PrimeiroTextoNaoVazio(StrJ(registro["sCodigo"]), StrJ(registro["codigo"]));
            string documento = PrimeiroTextoNaoVazio(StrJ(registro["sCPF_CNPJ"]), StrJ(registro["documento"]));
            string cidade = PrimeiroTextoNaoVazio(StrJ(registro["sCidade"]), StrJ(registro["cidade"]));
            string login = StrJ(registro["sLogin"]);
            string email = StrJ(registro["sEmail"]);
            if (!string.IsNullOrWhiteSpace(codigo)) partes.Add("Código: " + codigo);
            if (!string.IsNullOrWhiteSpace(documento)) partes.Add("Documento: " + documento);
            if (!string.IsNullOrWhiteSpace(cidade)) partes.Add("Cidade: " + cidade);
            if (!string.IsNullOrWhiteSpace(login)) partes.Add("Login: " + login);
            if (!string.IsNullOrWhiteSpace(email)) partes.Add("E-mail: " + email);
            return string.Join(" | ", partes.ToArray());
        }

        private static List<KeyValuePair<int, DataRow>> RankearTabelaResolucao(DataTable tb, string termo, string[] colunasRanking)
        {
            List<KeyValuePair<int, DataRow>> linhas = new List<KeyValuePair<int, DataRow>>();
            if (tb == null)
            {
                return linhas;
            }

            foreach (DataRow row in tb.Rows)
            {
                linhas.Add(new KeyValuePair<int, DataRow>(PontuarLinhaResolucao(row, termo, colunasRanking), row));
            }

            linhas.Sort(delegate (KeyValuePair<int, DataRow> a, KeyValuePair<int, DataRow> b)
            {
                int score = b.Key.CompareTo(a.Key);
                if (score != 0)
                {
                    return score;
                }

                return string.Compare(Valor(a.Value, colunasRanking != null && colunasRanking.Length > 0 ? colunasRanking[0] : string.Empty), Valor(b.Value, colunasRanking != null && colunasRanking.Length > 0 ? colunasRanking[0] : string.Empty), StringComparison.OrdinalIgnoreCase);
            });

            return linhas;
        }

        private static int PontuarLinhaResolucao(DataRow row, string termo, string[] colunasRanking)
        {
            string alvo = Normalizar(termo);
            string alvoDigitos = SomenteDigitos(termo);
            int melhor = 1;

            foreach (string coluna in colunasRanking ?? new string[0])
            {
                if (row == null || row.Table == null || !row.Table.Columns.Contains(coluna))
                {
                    continue;
                }

                string valor = Valor(row, coluna);
                string normalizado = Normalizar(valor);
                if (normalizado.Length == 0 || alvo.Length == 0)
                {
                    continue;
                }

                melhor = Math.Max(melhor, cls_IA_ResolucaoTexto.Pontuar(alvo, normalizado));

                string valorDigitos = SomenteDigitos(valor);
                if (alvoDigitos.Length >= 3 && valorDigitos.Contains(alvoDigitos))
                {
                    melhor = Math.Max(melhor, valorDigitos == alvoDigitos ? 120 : 85);
                }
            }

            return melhor;
        }

        private static string MensagemResolucao(int totalEncontrado, bool unicoProvavel, bool aproximado)
        {
            if (totalEncontrado == 0)
            {
                return "Nenhum registro encontrado para o termo informado.";
            }

            if (unicoProvavel)
            {
                return "Foi encontrado um resultado provável; use melhorResultado para preencher o próximo passo.";
            }

            if (aproximado)
            {
                return "Nenhum registro corresponde exatamente ao termo informado; estes são os mais parecidos. Peça ao usuário para escolher o correto antes de continuar.";
            }

            return "Foram encontrados vários resultados; peça ao usuário para escolher um registro antes de continuar.";
        }

        private static void AdicionarColunas(JObject item, DataRow row, string[] colunasPermitidas)
        {
            AdicionarColunas(item, row, colunasPermitidas, false);
        }

        private static void AdicionarColunas(JObject item, DataRow row, string[] colunasPermitidas, bool preservarTextoCompleto)
        {
            foreach (string coluna in colunasPermitidas)
            {
                if (row.Table.Columns.Contains(coluna))
                {
                    string valor = Valor(row, coluna);
                    item[coluna] = EhColunaDocumento(coluna)
                        ? MascararDocumento(valor)
                        : (preservarTextoCompleto ? cls_IA_Sanitizacao.RemoverHtml(valor).Trim() : cls_IA_Sanitizacao.Resumir(valor, 500));
                }
            }
        }

        private static string MontarLink(DataRow row, string template)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                return string.Empty;
            }

            string retorno = template;
            foreach (DataColumn coluna in row.Table.Columns)
            {
                retorno = retorno.Replace("{" + coluna.ColumnName + "}", Valor(row, coluna.ColumnName));
            }

            return retorno.Contains("{") ? string.Empty : retorno;
        }

        private static string LinkOrcamento(int idOrcamento)
        {
            return "/App/Paginas/Comercial/Orcamento_Detalhe.aspx?id=" + idOrcamento;
        }

        private static string LinkPdfOrcamento(int idOrcamento)
        {
            return LinkOrcamento(idOrcamento) + "&PDF=true";
        }

        private static DataTable FiltrarTabelaPorTermo(DataTable tabela, string termo, params string[] colunas)
        {
            if (tabela == null || string.IsNullOrWhiteSpace(termo) || colunas == null || colunas.Length == 0)
            {
                return tabela;
            }

            string alvo = Normalizar(termo);
            if (alvo.Length == 0)
            {
                return tabela;
            }

            DataTable filtrada = tabela.Clone();
            string alvoDigitos = SomenteDigitos(termo);
            foreach (DataRow row in tabela.Rows)
            {
                foreach (string coluna in colunas)
                {
                    if (!tabela.Columns.Contains(coluna))
                    {
                        continue;
                    }

                    string valor = Valor(row, coluna);
                    bool textoEncontrado = Normalizar(valor).Contains(alvo);
                    bool digitosEncontrados = alvoDigitos.Length >= 3 && SomenteDigitos(valor).Contains(alvoDigitos);
                    if (textoEncontrado || digitosEncontrados)
                    {
                        filtrada.ImportRow(row);
                        break;
                    }
                }
            }

            if (filtrada.Rows.Count > 0)
            {
                return filtrada;
            }

            foreach (DataRow row in tabela.Rows)
            {
                foreach (string coluna in colunas)
                {
                    if (!tabela.Columns.Contains(coluna)) continue;
                    if (!cls_IA_ResolucaoTexto.EhCandidatoAproximado(alvo, Valor(row, coluna))) continue;
                    filtrada.ImportRow(row);
                    break;
                }
            }

            return filtrada;
        }

        private static DataTable FiltrarTabelaPorValores(DataTable tabela, string coluna, ISet<string> valores)
        {
            if (tabela == null || string.IsNullOrWhiteSpace(coluna) || !tabela.Columns.Contains(coluna) || valores == null || valores.Count == 0)
            {
                return tabela != null ? tabela.Clone() : null;
            }

            DataTable filtrada = tabela.Clone();
            foreach (DataRow row in tabela.Rows)
            {
                if (valores.Contains(Valor(row, coluna)))
                {
                    filtrada.ImportRow(row);
                }
            }

            return filtrada;
        }

        private static void GarantirColunaAlias(DataTable tabela, string colunaDestino, string colunaOrigem)
        {
            if (tabela == null || !tabela.Columns.Contains(colunaOrigem))
            {
                return;
            }

            if (!tabela.Columns.Contains(colunaDestino))
            {
                tabela.Columns.Add(colunaDestino, typeof(string));
            }

            foreach (DataRow row in tabela.Rows)
            {
                if (string.IsNullOrWhiteSpace(Valor(row, colunaDestino)))
                {
                    row[colunaDestino] = Valor(row, colunaOrigem);
                }
            }
        }

        private static JObject ParseArgumentos(string json)
        {
            try
            {
                return string.IsNullOrWhiteSpace(json) ? new JObject() : JObject.Parse(json);
            }
            catch
            {
                return new JObject();
            }
        }

        private static string Texto(JObject argumentos, string chave, int limite)
        {
            string valor = argumentos != null && argumentos[chave] != null ? argumentos[chave].ToString() : string.Empty;
            return cls_IA_Sanitizacao.LimparEntradaUsuario(valor, limite);
        }

        private static string StrJ(JToken token)
        {
            return token == null ? string.Empty : token.ToString().Trim();
        }

        // Status financeiro aceito pela tela/SP de contas a pagar e receber. Devolve o valor canonico
        // (com a caixa correta que o @sStatus espera), string vazia para "todos", ou null se invalido.
        private static string NormalizarStatusFinanceiro(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return string.Empty;
            }

            string alvo = status.Trim();
            foreach (string valido in new[] { "Em Aberto", "Em Atraso", "Liquidado" })
            {
                if (string.Equals(valido, alvo, StringComparison.OrdinalIgnoreCase))
                {
                    return valido;
                }
            }

            return null;
        }

        private static int Limite(JObject argumentos, int limitePadrao)
        {
            int limite = Inteiro(argumentos, "limite", limitePadrao);
            if (limite <= 0)
            {
                limite = limitePadrao;
            }

            if (limite > limitePadrao)
            {
                limite = limitePadrao;
            }

            return limite;
        }

        private static int Inteiro(JObject argumentos, string chave, int padrao)
        {
            return argumentos != null ? Inteiro(argumentos[chave] != null ? argumentos[chave].ToString() : string.Empty, padrao) : padrao;
        }

        private static List<int> IdsInteiros(JObject argumentos, string chave)
        {
            List<int> ids = new List<int>();
            HashSet<int> vistos = new HashSet<int>();
            JArray array = argumentos != null ? argumentos[chave] as JArray : null;

            if (array != null)
            {
                foreach (JToken item in array)
                {
                    int id = Inteiro(item != null ? item.ToString() : string.Empty, 0);
                    if (id > 0 && !vistos.Contains(id))
                    {
                        vistos.Add(id);
                        ids.Add(id);
                    }
                }
            }
            else
            {
                string texto = Texto(argumentos, chave, 1000);
                foreach (string parte in texto.Split(','))
                {
                    int id = Inteiro(parte, 0);
                    if (id > 0 && !vistos.Contains(id))
                    {
                        vistos.Add(id);
                        ids.Add(id);
                    }
                }
            }

            return ids;
        }

        private List<int> IntersectarIdsPermitidos(List<int> idsSolicitados)
        {
            List<int> ids = new List<int>();
            foreach (int idSolicitado in idsSolicitados ?? new List<int>())
            {
                foreach (int idPermitido in _idsArquivosPermitidos)
                {
                    if (idSolicitado == idPermitido && !ids.Contains(idSolicitado))
                    {
                        ids.Add(idSolicitado);
                    }
                }
            }

            return ids;
        }

        private static int Inteiro(string valor, int padrao)
        {
            int retorno;
            return int.TryParse((valor ?? string.Empty).Trim(), out retorno) ? retorno : padrao;
        }

        private static string Valor(DataRow row, string coluna)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(coluna) || row[coluna] == DBNull.Value)
            {
                return string.Empty;
            }

            return row[coluna].ToString().Trim();
        }

        // O corpo da FAQ da casa vem do editor rico (HTML). Para a IA usar como texto de contexto,
        // remove as tags, decodifica entidades e colapsa espacos. Nao e sanitizacao de seguranca
        // (a saida vai para o modelo, nao para o DOM) -> so limpa o ruido de marcacao.
        private static string RemoverHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }

            string semTags = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", " ");
            semTags = System.Net.WebUtility.HtmlDecode(semTags);
            return System.Text.RegularExpressions.Regex.Replace(semTags, "\\s+", " ").Trim();
        }

        // Casa o termo com as FAQs aprovadas por palavra-chave (sem acento/case) em titulo+tags+corpo,
        // ja que o CONSULTAR do SP so procura em titulo+tag. Pontua por posicao (titulo > tag > corpo) e
        // devolve as melhores ate o limite. Sem termo util, nao retorna nada (evita despejar toda a FAQ).
        private static List<DataRow> RankearFaq(DataTable tbFaq, string termo, int limite)
        {
            List<DataRow> resultado = new List<DataRow>();
            if (tbFaq == null || tbFaq.Rows.Count == 0)
            {
                return resultado;
            }

            List<string> palavras = ExtrairPalavras(termo);
            if (palavras.Count == 0)
            {
                return resultado;
            }

            List<KeyValuePair<int, DataRow>> pontuados = new List<KeyValuePair<int, DataRow>>();
            foreach (DataRow row in tbFaq.Rows)
            {
                string titulo = Normalizar(Valor(row, "sTitulo"));
                string tags = Normalizar(Valor(row, "sTag"));
                string corpo = Normalizar(RemoverHtml(Valor(row, "sCorpo")));

                int score = 0;
                foreach (string p in palavras)
                {
                    if (titulo.IndexOf(p, StringComparison.Ordinal) >= 0)
                    {
                        score += 3;
                    }
                    else if (tags.IndexOf(p, StringComparison.Ordinal) >= 0)
                    {
                        score += 2;
                    }
                    else if (corpo.IndexOf(p, StringComparison.Ordinal) >= 0)
                    {
                        score += 1;
                    }
                }

                if (score > 0)
                {
                    pontuados.Add(new KeyValuePair<int, DataRow>(score, row));
                }
            }

            pontuados.Sort((a, b) => b.Key.CompareTo(a.Key));
            for (int i = 0; i < pontuados.Count && resultado.Count < limite; i++)
            {
                resultado.Add(pontuados[i].Value);
            }

            return resultado;
        }

        // Quebra o termo em palavras uteis (>= 3 chars, ja normalizadas). Descarta conectivos curtos.
        private static List<string> ExtrairPalavras(string termo)
        {
            List<string> palavras = new List<string>();
            if (string.IsNullOrWhiteSpace(termo))
            {
                return palavras;
            }

            char[] separadores = { ' ', '\t', '\r', '\n', ',', ';', '.', '?', '!', ':', '/', '(', ')', '"', '\'', '-' };
            foreach (string parte in Normalizar(termo).Split(separadores, StringSplitOptions.RemoveEmptyEntries))
            {
                if (parte.Length >= 3 && !palavras.Contains(parte))
                {
                    palavras.Add(parte);
                }
            }

            // Se sobrou so token curto (ex.: uma sigla de 2 letras), usa o termo inteiro normalizado.
            if (palavras.Count == 0)
            {
                string inteiro = Normalizar(termo).Trim();
                if (inteiro.Length > 0)
                {
                    palavras.Add(inteiro);
                }
            }

            return palavras;
        }

        // Normalização compartilhada: acentos, caixa, pontuação, hífens e espaços repetidos.
        private static string Normalizar(string texto)
        {
            return cls_IA_ResolucaoTexto.Normalizar(texto);
        }

        private static string SomenteDigitos(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            char[] buffer = new char[texto.Length];
            int total = 0;
            foreach (char c in texto)
            {
                if (char.IsDigit(c))
                {
                    buffer[total] = c;
                    total++;
                }
            }

            return new string(buffer, 0, total);
        }

        private static string JsonErro(string erro)
        {
            return new JObject
            {
                { "sucesso", false },
                { "erro", erro }
            }.ToString(Formatting.None);
        }

        private static string MensagemErroJson(string json, string fallback)
        {
            try
            {
                JObject obj = JObject.Parse(json ?? "{}");
                string erro = PrimeiroTextoNaoVazio(
                    obj["erro"] != null ? obj["erro"].ToString() : string.Empty,
                    obj["mensagem"] != null ? obj["mensagem"].ToString() : string.Empty,
                    obj["message"] != null ? obj["message"].ToString() : string.Empty,
                    obj["sMsg"] != null ? obj["sMsg"].ToString() : string.Empty,
                    obj["msg"] != null ? obj["msg"].ToString() : string.Empty,
                    obj["detalhe"] != null ? obj["detalhe"].ToString() : string.Empty
                );
                if (!string.IsNullOrWhiteSpace(erro))
                {
                    return erro;
                }

                JToken sucesso = obj["sucesso"];
                if (sucesso != null && sucesso.Type == JTokenType.Boolean && !sucesso.Value<bool>())
                {
                    return "A ferramenta retornou sucesso=false, mas não informou uma mensagem de erro.";
                }

                return fallback;
            }
            catch
            {
                return string.IsNullOrWhiteSpace(json) ? fallback : json;
            }
        }

        private static string PrimeiroTextoNaoVazio(params string[] valores)
        {
            foreach (string valor in valores ?? new string[0])
            {
                if (!string.IsNullOrWhiteSpace(valor))
                {
                    return valor.Trim();
                }
            }

            return string.Empty;
        }

        private static bool ResultadoIndicaSucesso(string json)
        {
            try
            {
                JObject obj = JObject.Parse(json ?? "{}");
                return obj["sucesso"] == null || obj["sucesso"].Type != JTokenType.Boolean || obj["sucesso"].Value<bool>();
            }
            catch
            {
                return false;
            }
        }

        // Mascara colunas cujo nome sugere documento (CPF/CNPJ), nao so a coluna exata sCPF_CNPJ.
        // Importante para ferramentas genericas, onde o admin escolhe as colunas de retorno.
        private static bool EhColunaDocumento(string coluna)
        {
            string c = (coluna ?? string.Empty).ToUpperInvariant();
            return c.Contains("CPF") || c.Contains("CNPJ");
        }

        private static string MascararDocumento(string documento)
        {
            string digitos = Regex.Replace(documento ?? string.Empty, "\\D", string.Empty);
            if (digitos.Length <= 4)
            {
                return string.IsNullOrWhiteSpace(digitos) ? string.Empty : "****";
            }

            return new string('*', digitos.Length - 4) + digitos.Substring(digitos.Length - 4);
        }
    }
}
