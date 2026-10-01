using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using TT.FrameWork;

namespace TT_Flow.FrameWork.IA
{
    public class cls_IA_Config
    {
        public const string ProviderOpenAI = "OPENAI";
        public const string ProviderGemini = "GEMINI";
        public const string ProviderClaude = "CLAUDE";
        public const string ProviderGroq = "GROQ";
        public const string ProviderOpenAICompativel = "OPENAI_COMPATIVEL";

        // Catalogo curado de modelos por provider (rotulo, id) — fonte unica usada pelos combos
        // da tela de Configuracao e pelo select de precos por modelo da tela IA - Uso
        public static readonly string[,] ModelosCuradosOpenAI =
        {
            { "GPT-5.5", "gpt-5.5" },
            { "GPT-5.4", "gpt-5.4" },
            { "GPT-5.4 mini", "gpt-5.4-mini" },
            { "GPT-5.4 nano", "gpt-5.4-nano" },
            { "GPT-5.3 Codex", "gpt-5.3-codex" }
        };

        public static readonly string[,] ModelosCuradosGemini =
        {
            { "Gemini 3.5 Flash", "gemini-3.5-flash" },
            { "Gemini 3.1 Pro Preview", "gemini-3.1-pro-preview" },
            { "Gemini 3 Flash Preview", "gemini-3-flash-preview" },
            { "Gemini 3.1 Flash-Lite", "gemini-3.1-flash-lite" },
            { "Gemini 2.5 Pro", "gemini-2.5-pro" },
            { "Gemini 2.5 Flash", "gemini-2.5-flash" },
            { "Gemini 2.5 Flash-Lite", "gemini-2.5-flash-lite" }
        };

        public static readonly string[,] ModelosCuradosClaude =
        {
            { "Claude Fable 5", "claude-fable-5" },
            { "Claude Opus 4.8", "claude-opus-4-8" },
            { "Claude Sonnet 4.6", "claude-sonnet-4-6" },
            { "Claude Haiku 4.5", "claude-haiku-4-5" },
            { "Claude Haiku 4.5 Snapshot", "claude-haiku-4-5-20251001" },
            { "Claude Mythos 5", "claude-mythos-5" }
        };

        public static readonly string[,] ModelosCuradosGroq =
        {
            { "GPT OSS 120B", "openai/gpt-oss-120b" },
            { "GPT OSS 20B", "openai/gpt-oss-20b" },
            { "Llama 3.3 70B Versatile", "llama-3.3-70b-versatile" },
            { "Llama 3.1 8B Instant", "llama-3.1-8b-instant" }
        };

        public static readonly string[,] ModelosCuradosCompat =
        {
            { "Selecione", "" },
            { "GPT OSS 120B", "gpt-oss-120b" },
            { "GPT OSS 20B", "gpt-oss-20b" },
            { "DeepSeek Chat", "deepseek-chat" },
            { "DeepSeek Reasoner", "deepseek-reasoner" },
            { "Qwen3 Coder", "qwen/qwen3-coder" },
            { "Mistral Large", "mistral-large-latest" }
        };

        public static List<string> ModelosCatalogo()
        {
            List<string> modelos = new List<string>();
            AdicionarModelosCatalogo(modelos, ModelosCuradosOpenAI);
            AdicionarModelosCatalogo(modelos, ModelosCuradosGemini);
            AdicionarModelosCatalogo(modelos, ModelosCuradosClaude);
            AdicionarModelosCatalogo(modelos, ModelosCuradosGroq);
            AdicionarModelosCatalogo(modelos, ModelosCuradosCompat);
            return modelos;
        }

        private static void AdicionarModelosCatalogo(List<string> destino, string[,] origem)
        {
            for (int i = 0; i < origem.GetLength(0); i++)
            {
                string valor = (origem[i, 1] ?? string.Empty).Trim();
                if (valor.Length > 0 && !destino.Contains(valor))
                {
                    destino.Add(valor);
                }
            }
        }

        public bool SqlDisponivel { get; set; }
        public string ErroConfiguracao { get; set; }
        public bool Habilitado { get; set; }
        public string Provider { get; set; }
        public string ProviderFallback { get; set; }
        public string ModeloPadrao { get; set; }
        public string OpenAIModelo { get; set; }
        public string GeminiModelo { get; set; }
        public string ClaudeModelo { get; set; }
        public string GroqModelo { get; set; }
        public string CompatModelo { get; set; }
        public string CompatBaseUrl { get; set; }
        public string RaciocinioNivel { get; set; }
        public string VerbosidadeTexto { get; set; }
        public int ThinkingBudgetTokens { get; set; }
        public bool StoreExterno { get; set; }
        public int MaxTokensEntrada { get; set; }
        public int MaxTokensSaida { get; set; }
        public int RateLimitPorUsuarioDia { get; set; }
        public int RateLimitPorIpDia { get; set; }
        public int RetencaoDias { get; set; }
        public int HistoricoMaxMensagens { get; set; }
        public int HistoricoMaxCharsPorMensagem { get; set; }
        public int FerramentasMaxRodadasPorMensagem { get; set; }
        // Tempo máximo (segundos) de uma passada do motor de workflow antes de abortar com "Tempo limite". 10 a 300.
        public int WorkflowTimeoutSegundos { get; set; }
        public decimal CustoPrecoEntradaPor1M { get; set; }
        public decimal CustoPrecoSaidaPor1M { get; set; }
        public string CustoPrecosModelosJson { get; set; }
        public bool ArquivosHabilitado { get; set; }
        public int ArquivosMaxArquivosPorMensagem { get; set; }
        public int ArquivosMaxMBArquivo { get; set; }
        public int ArquivosSyncMaxMBArquivo { get; set; }
        public int ArquivosMaxCharsTrecho { get; set; }
        public int ArquivosMaxTrechosPorResposta { get; set; }
        public int ArquivosRetencaoOriginalHoras { get; set; }
        // Base de conhecimento: re-ranking dos candidatos da busca pelo LLM (melhora a precisao).
        public bool ConhecimentoRerankHabilitado { get; set; }
        public int ConhecimentoRerankCandidatos { get; set; }
        // Base de conhecimento: verificacao de citacao pos-resposta (grounding, modo aviso).
        public bool ConhecimentoVerificacaoCitacao { get; set; }
        // Base de conhecimento SEMPRE no catalogo (default S): conhecimento_buscar e oferecida ao modelo
        // sem depender do botao do chat, e a IA decide quando consultar. 'N' volta ao comportamento antigo
        // (so com o botao ligado) — kill switch sem deploy. A permissao 695 continua valendo nos dois casos.
        public bool ConhecimentoSempreDisponivel { get; set; }
        // Pasta de entrada da base de conhecimento (hot folder). Vazio = varredura desligada.
        // Subpasta de 1o nivel vira a categoria do documento; arquivos vao para _Processados/_Erros.
        public string ConhecimentoPastaEntrada { get; set; }
        // Teto de arquivos ingeridos por ciclo do job tmrConhecimentoPasta do TT_Windows (o resto fica para o
        // ciclo seguinte). Lido la em IAArquivoMarkdownWorker.CarregarConfig; aqui so para a tela de Configuracao.
        public int ConhecimentoPastaMaxPorVarredura { get; set; }
        // Dono (idUsuario) de todo documento que entra pela pasta. Desde 16/09/2026 so o job do TT_Windows le a
        // pasta, sem sessao nem usuario logado, entao e sempre este. Vale um usuario tecnico (ex.: Processo Automatico).
        public string ConhecimentoPastaIdUsuario { get; set; }
        // FAQ permissionada: mapa JSON "idCategoria":idRecurso. A IA so mostra uma FAQ se o usuario
        // tiver o recurso da categoria dela; categoria sem mapa exige a permissao base de FAQ (219).
        public string FaqCategoriaRecursoJson { get; set; }
        // OCR de PDFs escaneados (Tesseract on-prem). Default DESLIGADO: so ligar depois que os
        // binarios nativos (bin\x64, pdfium) e a pasta App_Data\tessdata estiverem no servidor.
        public bool ArquivosOcrHabilitado { get; set; }
        public int ArquivosOcrMaxPaginas { get; set; }
        public string OpenAIApiKey { get; set; }
        public string GeminiApiKey { get; set; }
        public string ClaudeApiKey { get; set; }
        public string GroqApiKey { get; set; }
        public string CompatApiKey { get; set; }

        private const string Procedure = "sp_Manipula_tbl_Flow_IA_Chat";

        public static cls_IA_Config Carregar()
        {
            cls_IA_Config config = Padrao();

            try
            {
                Dictionary<string, string> parametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_CONFIG" }
                };

                DataTable tbConfig = BD.ExecutarDataTable(Procedure, parametros, false);
                config.SqlDisponivel = true;

                foreach (DataRow row in tbConfig.Rows)
                {
                    string chave = Valor(row, "sChave");
                    string valor = Valor(row, "sValor");
                    AplicarValor(config, chave, valor);
                }
            }
            catch (Exception ex)
            {
                config.SqlDisponivel = false;
                config.ErroConfiguracao = "Estrutura SQL da IA nao aplicada: " + ex.Message;
            }

            AplicarSobrescritasAmbiente(config);
            AtualizarModeloAtivo(config);
            return config;
        }

        public string ObterApiKeyProviderAtual()
        {
            switch ((Provider ?? string.Empty).ToUpperInvariant())
            {
                case ProviderGemini:
                    return GeminiApiKey;
                case ProviderClaude:
                    return ClaudeApiKey;
                case ProviderGroq:
                    return GroqApiKey;
                case ProviderOpenAICompativel:
                    return CompatApiKey;
                case ProviderOpenAI:
                default:
                    return OpenAIApiKey;
            }
        }

        private static cls_IA_Config Padrao()
        {
            return new cls_IA_Config
            {
                SqlDisponivel = false,
                ErroConfiguracao = string.Empty,
                Habilitado = false,
                Provider = ProviderOpenAI,
                ProviderFallback = string.Empty,
                ModeloPadrao = "gpt-5.5",
                OpenAIModelo = "gpt-5.5",
                GeminiModelo = "gemini-3.5-flash",
                ClaudeModelo = "claude-opus-4-8",
                GroqModelo = "openai/gpt-oss-120b",
                CompatModelo = string.Empty,
                CompatBaseUrl = string.Empty,
                RaciocinioNivel = "medium",
                VerbosidadeTexto = "medium",
                ThinkingBudgetTokens = 0,
                StoreExterno = false,
                MaxTokensEntrada = 8000,
                MaxTokensSaida = 1200,
                RateLimitPorUsuarioDia = 100,
                RateLimitPorIpDia = 0,
                RetencaoDias = 365,
                HistoricoMaxMensagens = 24,
                HistoricoMaxCharsPorMensagem = 3000,
                FerramentasMaxRodadasPorMensagem = 6,
                WorkflowTimeoutSegundos = 30,
                CustoPrecoEntradaPor1M = 0,
                CustoPrecoSaidaPor1M = 0,
                CustoPrecosModelosJson = "{}",
                ArquivosHabilitado = true,
                ArquivosMaxArquivosPorMensagem = 3,
                ArquivosMaxMBArquivo = 10,
                ArquivosSyncMaxMBArquivo = 10,   // = MaxMB: converte tudo na requisicao, sem depender do worker TT_Windows

                ArquivosMaxCharsTrecho = 4000,
                ArquivosMaxTrechosPorResposta = 8,
                ArquivosRetencaoOriginalHoras = 24,
                ConhecimentoRerankHabilitado = true,
                ConhecimentoRerankCandidatos = 24,
                ConhecimentoVerificacaoCitacao = true,
                ConhecimentoSempreDisponivel = true,
                ConhecimentoPastaEntrada = string.Empty,
                ConhecimentoPastaMaxPorVarredura = 20,
                ConhecimentoPastaIdUsuario = "0",
                FaqCategoriaRecursoJson = "{}",
                ArquivosOcrHabilitado = false,
                ArquivosOcrMaxPaginas = 50,
                OpenAIApiKey = string.Empty,
                GeminiApiKey = string.Empty,
                ClaudeApiKey = string.Empty,
                GroqApiKey = string.Empty,
                CompatApiKey = string.Empty
            };
        }

        private static void AplicarSobrescritasAmbiente(cls_IA_Config config)
        {
            AplicarValor(config, "IA.Habilitado", LerSettingOuAmbiente("IA.Habilitado", "TT_FLOW_IA_HABILITADO"));
            AplicarValor(config, "IA.Provider", LerSettingOuAmbiente("IA.Provider", "TT_FLOW_IA_PROVIDER"));
            AplicarValor(config, "IA.Provider.Fallback", LerSettingOuAmbiente("IA.Provider.Fallback", "TT_FLOW_IA_PROVIDER_FALLBACK"));
            AplicarValor(config, "IA.ModeloPadrao", LerSettingOuAmbiente("IA.ModeloPadrao", "TT_FLOW_IA_MODELO"));
            AplicarValor(config, "IA.OpenAI.Modelo", LerSettingOuAmbiente("IA.OpenAI.Modelo", "TT_FLOW_IA_OPENAI_MODELO"));
            AplicarValor(config, "IA.Gemini.Modelo", LerSettingOuAmbiente("IA.Gemini.Modelo", "TT_FLOW_IA_GEMINI_MODELO"));
            AplicarValor(config, "IA.Claude.Modelo", LerSettingOuAmbiente("IA.Claude.Modelo", "TT_FLOW_IA_CLAUDE_MODELO"));
            AplicarValor(config, "IA.Groq.Modelo", LerSettingOuAmbiente("IA.Groq.Modelo", "TT_FLOW_IA_GROQ_MODELO"));
            AplicarValor(config, "IA.Compat.Modelo", LerSettingOuAmbiente("IA.Compat.Modelo", "TT_FLOW_IA_COMPAT_MODELO"));
            AplicarValor(config, "IA.Compat.BaseUrl", LerSettingOuAmbiente("IA.Compat.BaseUrl", "TT_FLOW_IA_COMPAT_BASE_URL"));
            AplicarValor(config, "IA.RaciocinioNivel", LerSettingOuAmbiente("IA.RaciocinioNivel", "TT_FLOW_IA_RACIOCINIO_NIVEL"));
            AplicarValor(config, "IA.VerbosidadeTexto", LerSettingOuAmbiente("IA.VerbosidadeTexto", "TT_FLOW_IA_VERBOSIDADE_TEXTO"));
            AplicarValor(config, "IA.ThinkingBudgetTokens", LerSettingOuAmbiente("IA.ThinkingBudgetTokens", "TT_FLOW_IA_THINKING_BUDGET_TOKENS"));
            AplicarValor(config, "IA.StoreExterno", LerSettingOuAmbiente("IA.StoreExterno", "TT_FLOW_IA_STORE_EXTERNO"));
            AplicarValor(config, "IA.MaxTokensEntrada", LerSettingOuAmbiente("IA.MaxTokensEntrada", "TT_FLOW_IA_MAX_TOKENS_ENTRADA"));
            AplicarValor(config, "IA.MaxTokensSaida", LerSettingOuAmbiente("IA.MaxTokensSaida", "TT_FLOW_IA_MAX_TOKENS_SAIDA"));
            AplicarValor(config, "IA.RateLimitPorUsuarioDia", LerSettingOuAmbiente("IA.RateLimitPorUsuarioDia", "TT_FLOW_IA_RATE_LIMIT_USUARIO_DIA"));
            AplicarValor(config, "IA.RateLimitPorIpDia", LerSettingOuAmbiente("IA.RateLimitPorIpDia", "TT_FLOW_IA_RATE_LIMIT_IP_DIA"));
            AplicarValor(config, "IA.RetencaoDias", LerSettingOuAmbiente("IA.RetencaoDias", "TT_FLOW_IA_RETENCAO_DIAS"));
            AplicarValor(config, "IA.Historico.MaxMensagens", LerSettingOuAmbiente("IA.Historico.MaxMensagens", "TT_FLOW_IA_HISTORICO_MAX_MENSAGENS"));
            AplicarValor(config, "IA.Historico.MaxCharsPorMensagem", LerSettingOuAmbiente("IA.Historico.MaxCharsPorMensagem", "TT_FLOW_IA_HISTORICO_MAX_CHARS_POR_MENSAGEM"));
            AplicarValor(config, "IA.Ferramentas.MaxRodadasPorMensagem", LerSettingOuAmbiente("IA.Ferramentas.MaxRodadasPorMensagem", "TT_FLOW_IA_FERRAMENTAS_MAX_RODADAS_POR_MENSAGEM"));
            AplicarValor(config, "IA.Workflow.TimeoutSegundos", LerSettingOuAmbiente("IA.Workflow.TimeoutSegundos", "TT_FLOW_IA_WORKFLOW_TIMEOUT_SEGUNDOS"));
            AplicarValor(config, "IA.Custo.PrecoEntradaPor1M", LerSettingOuAmbiente("IA.Custo.PrecoEntradaPor1M", "TT_FLOW_IA_CUSTO_PRECO_ENTRADA_POR_1M"));
            AplicarValor(config, "IA.Custo.PrecoSaidaPor1M", LerSettingOuAmbiente("IA.Custo.PrecoSaidaPor1M", "TT_FLOW_IA_CUSTO_PRECO_SAIDA_POR_1M"));
            AplicarValor(config, "IA.Custo.PrecosModelosJson", LerSettingOuAmbiente("IA.Custo.PrecosModelosJson", "TT_FLOW_IA_CUSTO_PRECOS_MODELOS_JSON"));
            AplicarValor(config, "IA.Arquivos.Habilitado", LerSettingOuAmbiente("IA.Arquivos.Habilitado", "TT_FLOW_IA_ARQUIVOS_HABILITADO"));
            AplicarValor(config, "IA.Arquivos.MaxArquivosPorMensagem", LerSettingOuAmbiente("IA.Arquivos.MaxArquivosPorMensagem", "TT_FLOW_IA_ARQUIVOS_MAX_ARQUIVOS_POR_MENSAGEM"));
            AplicarValor(config, "IA.Arquivos.MaxMBArquivo", LerSettingOuAmbiente("IA.Arquivos.MaxMBArquivo", "TT_FLOW_IA_ARQUIVOS_MAX_MB_ARQUIVO"));
            AplicarValor(config, "IA.Arquivos.SyncMaxMBArquivo", LerSettingOuAmbiente("IA.Arquivos.SyncMaxMBArquivo", "TT_FLOW_IA_ARQUIVOS_SYNC_MAX_MB_ARQUIVO"));
            AplicarValor(config, "IA.Arquivos.MaxCharsTrecho", LerSettingOuAmbiente("IA.Arquivos.MaxCharsTrecho", "TT_FLOW_IA_ARQUIVOS_MAX_CHARS_TRECHO"));
            AplicarValor(config, "IA.Arquivos.MaxTrechosPorResposta", LerSettingOuAmbiente("IA.Arquivos.MaxTrechosPorResposta", "TT_FLOW_IA_ARQUIVOS_MAX_TRECHOS_POR_RESPOSTA"));
            AplicarValor(config, "IA.Arquivos.RetencaoOriginalHoras", LerSettingOuAmbiente("IA.Arquivos.RetencaoOriginalHoras", "TT_FLOW_IA_ARQUIVOS_RETENCAO_ORIGINAL_HORAS"));

            config.OpenAIApiKey = PrimeiroValor(config.OpenAIApiKey, LerSettingOuAmbiente("IA.OpenAI.ApiKey", "TT_FLOW_IA_OPENAI_API_KEY"), Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
            config.GeminiApiKey = PrimeiroValor(config.GeminiApiKey, LerSettingOuAmbiente("IA.Gemini.ApiKey", "TT_FLOW_IA_GEMINI_API_KEY"), Environment.GetEnvironmentVariable("GEMINI_API_KEY"));
            config.ClaudeApiKey = PrimeiroValor(config.ClaudeApiKey, LerSettingOuAmbiente("IA.Claude.ApiKey", "TT_FLOW_IA_CLAUDE_API_KEY"), Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY"));
            config.GroqApiKey = PrimeiroValor(config.GroqApiKey, LerSettingOuAmbiente("IA.Groq.ApiKey", "TT_FLOW_IA_GROQ_API_KEY"), Environment.GetEnvironmentVariable("GROQ_API_KEY"));
            config.CompatApiKey = PrimeiroValor(config.CompatApiKey, LerSettingOuAmbiente("IA.Compat.ApiKey", "TT_FLOW_IA_COMPAT_API_KEY"));
        }

        private static string LerSettingOuAmbiente(string chaveAppSettings, string chaveAmbiente)
        {
            string valor = ConfigurationManager.AppSettings[chaveAppSettings];
            if (!string.IsNullOrWhiteSpace(valor))
            {
                return valor;
            }

            return Environment.GetEnvironmentVariable(chaveAmbiente);
        }

        private static void AplicarValor(cls_IA_Config config, string chave, string valor)
        {
            if (string.IsNullOrWhiteSpace(chave) || valor == null)
            {
                return;
            }

            switch (chave.Trim().ToUpperInvariant())
            {
                case "IA.HABILITADO":
                    config.Habilitado = ParaBool(valor);
                    break;
                case "IA.PROVIDER":
                    config.Provider = NormalizarProvider(valor);
                    break;
                case "IA.PROVIDER.FALLBACK":
                    // Vazio = desligado; qualquer outro valor e normalizado para um provider valido
                    config.ProviderFallback = string.IsNullOrWhiteSpace(valor) ? string.Empty : NormalizarProvider(valor);
                    break;
                case "IA.MODELOPADRAO":
                    config.ModeloPadrao = valor.Trim();
                    config.OpenAIModelo = valor.Trim();
                    break;
                case "IA.OPENAI.MODELO":
                    config.OpenAIModelo = valor.Trim();
                    break;
                case "IA.GEMINI.MODELO":
                    config.GeminiModelo = valor.Trim();
                    break;
                case "IA.CLAUDE.MODELO":
                    config.ClaudeModelo = valor.Trim();
                    break;
                case "IA.GROQ.MODELO":
                    config.GroqModelo = valor.Trim();
                    break;
                case "IA.COMPAT.MODELO":
                    config.CompatModelo = valor.Trim();
                    break;
                case "IA.COMPAT.BASEURL":
                    config.CompatBaseUrl = valor.Trim();
                    break;
                case "IA.RACIOCINIONIVEL":
                    config.RaciocinioNivel = NormalizarRaciocinio(valor);
                    break;
                case "IA.VERBOSIDADETEXTO":
                    config.VerbosidadeTexto = NormalizarVerbosidade(valor);
                    break;
                case "IA.THINKINGBUDGETTOKENS":
                    config.ThinkingBudgetTokens = ParaInt(valor, config.ThinkingBudgetTokens);
                    break;
                case "IA.STOREEXTERNO":
                    config.StoreExterno = ParaBool(valor);
                    break;
                case "IA.MAXTOKENSENTRADA":
                    config.MaxTokensEntrada = ParaInt(valor, config.MaxTokensEntrada);
                    break;
                case "IA.MAXTOKENSSAIDA":
                    config.MaxTokensSaida = ParaInt(valor, config.MaxTokensSaida);
                    break;
                case "IA.RATELIMITPORUSUARIODIA":
                    config.RateLimitPorUsuarioDia = ParaInt(valor, config.RateLimitPorUsuarioDia);
                    break;
                case "IA.RATELIMITPORIPDIA":
                    // 0 desliga o limite por IP (util em rede com NAT/IP compartilhado)
                    config.RateLimitPorIpDia = ParaInt(valor, config.RateLimitPorIpDia);
                    break;
                case "IA.RETENCAODIAS":
                    // 0 = manter para sempre (nao purga a auditoria)
                    config.RetencaoDias = ParaInt(valor, config.RetencaoDias);
                    break;
                case "IA.HISTORICO.MAXMENSAGENS":
                    // 0 desliga o envio de historico ao provider
                    config.HistoricoMaxMensagens = LimitarInt(valor, config.HistoricoMaxMensagens, 0, 60);
                    break;
                case "IA.HISTORICO.MAXCHARSPORMENSAGEM":
                    config.HistoricoMaxCharsPorMensagem = LimitarInt(valor, config.HistoricoMaxCharsPorMensagem, 200, 8000);
                    break;
                case "IA.FERRAMENTAS.MAXRODADASPORMENSAGEM":
                    config.FerramentasMaxRodadasPorMensagem = LimitarInt(valor, config.FerramentasMaxRodadasPorMensagem, 1, 10);
                    break;
                case "IA.WORKFLOW.TIMEOUTSEGUNDOS":
                    // Teto abaixo do ScriptTimeout do streaming (IA_Stream.aspx, 330 s)
                    config.WorkflowTimeoutSegundos = LimitarInt(valor, config.WorkflowTimeoutSegundos, 10, 300);
                    break;
                case "IA.CUSTO.PRECOENTRADAPOR1M":
                    config.CustoPrecoEntradaPor1M = ParaDecimal(valor, config.CustoPrecoEntradaPor1M);
                    break;
                case "IA.CUSTO.PRECOSAIDAPOR1M":
                    config.CustoPrecoSaidaPor1M = ParaDecimal(valor, config.CustoPrecoSaidaPor1M);
                    break;
                case "IA.CUSTO.PRECOSMODELOSJSON":
                    config.CustoPrecosModelosJson = valor.Trim();
                    break;
                case "IA.ARQUIVOS.HABILITADO":
                    config.ArquivosHabilitado = ParaBool(valor);
                    break;
                case "IA.ARQUIVOS.MAXARQUIVOSPORMENSAGEM":
                    config.ArquivosMaxArquivosPorMensagem = LimitarInt(valor, config.ArquivosMaxArquivosPorMensagem, 1, 10);
                    break;
                case "IA.ARQUIVOS.MAXMBARQUIVO":
                    config.ArquivosMaxMBArquivo = LimitarInt(valor, config.ArquivosMaxMBArquivo, 1, 100);
                    break;
                case "IA.ARQUIVOS.SYNCMAXMBARQUIVO":
                    config.ArquivosSyncMaxMBArquivo = LimitarInt(valor, config.ArquivosSyncMaxMBArquivo, 1, Math.Max(1, config.ArquivosMaxMBArquivo));
                    break;
                case "IA.ARQUIVOS.MAXCHARSTRECHO":
                    config.ArquivosMaxCharsTrecho = LimitarInt(valor, config.ArquivosMaxCharsTrecho, 1000, 20000);
                    break;
                case "IA.ARQUIVOS.MAXTRECHOSPORRESPOSTA":
                    config.ArquivosMaxTrechosPorResposta = LimitarInt(valor, config.ArquivosMaxTrechosPorResposta, 1, 20);
                    break;
                case "IA.ARQUIVOS.RETENCAOORIGINALHORAS":
                    config.ArquivosRetencaoOriginalHoras = LimitarInt(valor, config.ArquivosRetencaoOriginalHoras, 1, 168);
                    break;
                case "IA.CONHECIMENTO.RERANKHABILITADO":
                    config.ConhecimentoRerankHabilitado = ParaBool(valor);
                    break;
                case "IA.CONHECIMENTO.RERANKCANDIDATOS":
                    config.ConhecimentoRerankCandidatos = LimitarInt(valor, config.ConhecimentoRerankCandidatos, 8, 50);
                    break;
                case "IA.CONHECIMENTO.VERIFICACAOCITACAO":
                    config.ConhecimentoVerificacaoCitacao = ParaBool(valor);
                    break;
                case "IA.CONHECIMENTO.SEMPREDISPONIVEL":
                    config.ConhecimentoSempreDisponivel = ParaBool(valor);
                    break;
                case "IA.CONHECIMENTO.PASTAENTRADA":
                    config.ConhecimentoPastaEntrada = (valor ?? string.Empty).Trim();
                    break;
                case "IA.CONHECIMENTO.PASTAMAXPORVARREDURA":
                    config.ConhecimentoPastaMaxPorVarredura = LimitarInt(valor, config.ConhecimentoPastaMaxPorVarredura, 1, 200);
                    break;
                case "IA.CONHECIMENTO.PASTAIDUSUARIO":
                    config.ConhecimentoPastaIdUsuario = LimitarInt(valor, 0, 0, int.MaxValue).ToString();
                    break;
                case "IA.CONHECIMENTO.FAQCATEGORIARECURSO":
                    config.FaqCategoriaRecursoJson = string.IsNullOrWhiteSpace(valor) ? "{}" : valor.Trim();
                    break;
                case "IA.ARQUIVOS.OCRHABILITADO":
                    config.ArquivosOcrHabilitado = ParaBool(valor);
                    break;
                case "IA.ARQUIVOS.OCRMAXPAGINAS":
                    config.ArquivosOcrMaxPaginas = LimitarInt(valor, config.ArquivosOcrMaxPaginas, 1, 500);
                    break;
                case "IA.OPENAI.APIKEYCRIPTOGRAFADA":
                    config.OpenAIApiKey = DescriptografarSeguro(valor);
                    break;
                case "IA.GEMINI.APIKEYCRIPTOGRAFADA":
                    config.GeminiApiKey = DescriptografarSeguro(valor);
                    break;
                case "IA.CLAUDE.APIKEYCRIPTOGRAFADA":
                    config.ClaudeApiKey = DescriptografarSeguro(valor);
                    break;
                case "IA.GROQ.APIKEYCRIPTOGRAFADA":
                    config.GroqApiKey = DescriptografarSeguro(valor);
                    break;
                case "IA.COMPAT.APIKEYCRIPTOGRAFADA":
                    config.CompatApiKey = DescriptografarSeguro(valor);
                    break;
            }
        }

        private static void AtualizarModeloAtivo(cls_IA_Config config)
        {
            if (string.IsNullOrWhiteSpace(config.OpenAIModelo))
            {
                config.OpenAIModelo = "gpt-5.5";
            }

            if (string.IsNullOrWhiteSpace(config.GeminiModelo))
            {
                config.GeminiModelo = "gemini-3.5-flash";
            }

            if (string.IsNullOrWhiteSpace(config.ClaudeModelo))
            {
                config.ClaudeModelo = "claude-opus-4-8";
            }

            if (string.IsNullOrWhiteSpace(config.GroqModelo))
            {
                config.GroqModelo = "openai/gpt-oss-120b";
            }

            switch ((config.Provider ?? string.Empty).ToUpperInvariant())
            {
                case ProviderGemini:
                    config.ModeloPadrao = config.GeminiModelo;
                    break;
                case ProviderClaude:
                    config.ModeloPadrao = config.ClaudeModelo;
                    break;
                case ProviderGroq:
                    config.ModeloPadrao = config.GroqModelo;
                    break;
                case ProviderOpenAICompativel:
                    config.ModeloPadrao = config.CompatModelo;
                    break;
                case ProviderOpenAI:
                default:
                    config.Provider = ProviderOpenAI;
                    config.ModeloPadrao = config.OpenAIModelo;
                    break;
            }
        }

        private static string NormalizarProvider(string valor)
        {
            valor = (valor ?? string.Empty).Trim().ToUpperInvariant();

            if (valor == ProviderGemini || valor == ProviderClaude || valor == ProviderGroq || valor == ProviderOpenAICompativel || valor == "OPENAI_COMPATIBLE")
            {
                return valor == "OPENAI_COMPATIBLE" ? ProviderOpenAICompativel : valor;
            }

            return ProviderOpenAI;
        }

        private static string NormalizarRaciocinio(string valor)
        {
            valor = (valor ?? string.Empty).Trim().ToLowerInvariant();
            if (valor == "none" || valor == "low" || valor == "medium" || valor == "high" || valor == "xhigh")
            {
                return valor;
            }

            return "medium";
        }

        private static string NormalizarVerbosidade(string valor)
        {
            valor = (valor ?? string.Empty).Trim().ToLowerInvariant();
            if (valor == "low" || valor == "medium" || valor == "high")
            {
                return valor;
            }

            return "medium";
        }

        private static string PrimeiroValor(params string[] valores)
        {
            foreach (string valor in valores)
            {
                if (!string.IsNullOrWhiteSpace(valor))
                {
                    return valor.Trim();
                }
            }

            return string.Empty;
        }

        private static string DescriptografarSeguro(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return string.Empty;
            }

            try
            {
                return cls_IA_Criptografia.Descriptografar(valor);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static bool ParaBool(string valor)
        {
            valor = (valor ?? string.Empty).Trim().ToUpperInvariant();
            return valor == "S" || valor == "SIM" || valor == "TRUE" || valor == "1";
        }

        private static int ParaInt(string valor, int padrao)
        {
            int retorno;
            return int.TryParse((valor ?? string.Empty).Trim(), out retorno) ? retorno : padrao;
        }

        private static decimal ParaDecimal(string valor, decimal padrao)
        {
            valor = (valor ?? string.Empty).Trim().Replace(",", ".");
            decimal retorno;
            return decimal.TryParse(valor, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out retorno) && retorno >= 0
                ? retorno
                : padrao;
        }

        private static int LimitarInt(string valor, int padrao, int minimo, int maximo)
        {
            int retorno = ParaInt(valor, padrao);
            if (retorno < minimo)
            {
                return minimo;
            }

            if (retorno > maximo)
            {
                return maximo;
            }

            return retorno;
        }

        private static string Valor(DataRow row, string coluna)
        {
            if (row == null || !row.Table.Columns.Contains(coluna) || row[coluna] == DBNull.Value)
            {
                return string.Empty;
            }

            return row[coluna].ToString().Trim();
        }
    }
}
