namespace TT_Flow.FrameWork.IA
{
    public interface IIAProvider
    {
        IAProviderResponse GerarResposta(IAProviderRequest request);
    }

    // Implementada apenas pelos providers com suporte a stream de texto (OpenAI e Claude).
    // O retorno final deve ser equivalente ao de GerarResposta; os deltas sao apenas notificacao.
    public interface IIAProviderStreaming
    {
        IAProviderResponse GerarRespostaStream(IAProviderRequest request, System.Action<string> onDeltaTexto);
    }

    public static class cls_IA_ProviderFactory
    {
        public static IIAProvider Criar(cls_IA_Config config)
        {
            if (config == null)
            {
                return new cls_IA_ProviderNaoConfigurado("Provider de IA nao configurado.");
            }

            switch ((config.Provider ?? string.Empty).ToUpperInvariant())
            {
                case cls_IA_Config.ProviderOpenAI:
                    return new cls_IA_OpenAIProvider(config);
                case cls_IA_Config.ProviderGemini:
                    return new cls_IA_GeminiProvider(config);
                case cls_IA_Config.ProviderClaude:
                    return new cls_IA_ClaudeProvider(config);
                case cls_IA_Config.ProviderGroq:
                    return new cls_IA_GroqProvider(config);
                case cls_IA_Config.ProviderOpenAICompativel:
                    return new cls_IA_OpenAICompatProvider(config);
            }

            return new cls_IA_ProviderNaoConfigurado("Provider de IA nao suportado ou nao configurado.");
        }

        // Cria um provider especifico pelo nome (usado no failover), devolvendo tambem o modelo dele.
        // Retorna false quando o provider nao e suportado ou nao tem chave configurada — nesse caso
        // nao vale a pena tentar o fallback.
        public static bool CriarPorNome(cls_IA_Config config, string nomeProvider, out IIAProvider provider, out string modelo)
        {
            provider = null;
            modelo = string.Empty;

            if (config == null)
            {
                return false;
            }

            switch ((nomeProvider ?? string.Empty).Trim().ToUpperInvariant())
            {
                case cls_IA_Config.ProviderOpenAI:
                    provider = new cls_IA_OpenAIProvider(config);
                    modelo = config.OpenAIModelo;
                    return !string.IsNullOrWhiteSpace(config.OpenAIApiKey);
                case cls_IA_Config.ProviderGemini:
                    provider = new cls_IA_GeminiProvider(config);
                    modelo = config.GeminiModelo;
                    return !string.IsNullOrWhiteSpace(config.GeminiApiKey);
                case cls_IA_Config.ProviderClaude:
                    provider = new cls_IA_ClaudeProvider(config);
                    modelo = config.ClaudeModelo;
                    return !string.IsNullOrWhiteSpace(config.ClaudeApiKey);
                case cls_IA_Config.ProviderGroq:
                    provider = new cls_IA_GroqProvider(config);
                    modelo = config.GroqModelo;
                    return !string.IsNullOrWhiteSpace(config.GroqApiKey);
                case cls_IA_Config.ProviderOpenAICompativel:
                    provider = new cls_IA_OpenAICompatProvider(config);
                    modelo = config.CompatModelo;
                    return !string.IsNullOrWhiteSpace(config.CompatApiKey);
            }

            return false;
        }
    }

    public class cls_IA_ProviderNaoConfigurado : IIAProvider
    {
        private readonly string _mensagem;

        public cls_IA_ProviderNaoConfigurado(string mensagem)
        {
            _mensagem = mensagem;
        }

        public IAProviderResponse GerarResposta(IAProviderRequest request)
        {
            return new IAProviderResponse
            {
                Sucesso = false,
                Erro = _mensagem
            };
        }
    }
}
