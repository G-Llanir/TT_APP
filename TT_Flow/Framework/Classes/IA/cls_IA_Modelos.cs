using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TT_Flow.FrameWork.IA
{
    [Serializable]
    public class IAChatResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public string Resposta { get; set; }
        public int IdConversaIA { get; set; }
        public bool IAConfigurada { get; set; }
        public bool IAHabilitada { get; set; }
        public List<IAConversaResumo> Conversas { get; set; }
        public List<IAMensagem> Mensagens { get; set; }
        public List<IAFonteResposta> Fontes { get; set; }
        public List<IAArquivoChat> Arquivos { get; set; }
        public List<IATemplatePrompt> Templates { get; set; }
        public IAArquivoChat Arquivo { get; set; }
        public IAAcaoPendente AcaoPendente { get; set; }

        public IAChatResponse()
        {
            Conversas = new List<IAConversaResumo>();
            Mensagens = new List<IAMensagem>();
            Fontes = new List<IAFonteResposta>();
            Arquivos = new List<IAArquivoChat>();
            Templates = new List<IATemplatePrompt>();
        }
    }

    [Serializable]
    public class IATemplatePrompt
    {
        public int IdTemplatePromptIA { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public string ContextoTela { get; set; }
    }

    [Serializable]
    public class IAConversaResumo
    {
        public int IdConversaIA { get; set; }
        public string Titulo { get; set; }
        public string Status { get; set; }
        public bool Favorita { get; set; }
        public string DtUltimaMensagem { get; set; }
    }

    [Serializable]
    public class IAMensagem
    {
        public int IdMensagemIA { get; set; }
        public int IdConversaIA { get; set; }
        public int Ordem { get; set; }
        public string Papel { get; set; }
        public string Conteudo { get; set; }
        public string DtInclusao { get; set; }
        public string Avaliacao { get; set; }
        public string AvaliacaoComentario { get; set; }
        public List<IAFonteResposta> Fontes { get; set; }
        public List<IAArquivoChat> Arquivos { get; set; }

        public IAMensagem()
        {
            Avaliacao = string.Empty;
            AvaliacaoComentario = string.Empty;
            Fontes = new List<IAFonteResposta>();
            Arquivos = new List<IAArquivoChat>();
        }
    }

    [Serializable]
    public class IAFonteResposta
    {
        public string Ferramenta { get; set; }
        public string Tipo { get; set; }
        public string Titulo { get; set; }
        public string Url { get; set; }
        public string Resumo { get; set; }
        public int IdArquivoIA { get; set; }
        public int IdArquivoTrechoIA { get; set; }
        public string HashSHA256 { get; set; }
        public string Trecho { get; set; }
        public int InicioChar { get; set; }
        public int FimChar { get; set; }

        public IAFonteResposta()
        {
            Ferramenta = string.Empty;
            Tipo = string.Empty;
            Titulo = string.Empty;
            Url = string.Empty;
            Resumo = string.Empty;
            HashSHA256 = string.Empty;
            Trecho = string.Empty;
        }
    }

    public class IAProviderRequest
    {
        public string Modelo { get; set; }
        public string Instrucoes { get; set; }
        public string MensagemUsuario { get; set; }
        public JArray InputItens { get; set; }
        public JArray Ferramentas { get; set; }
        public List<IAToolResult> ToolResults { get; set; }
        public List<IAMensagemHistorico> Historico { get; set; }
        public bool StoreExterno { get; set; }
        public int MaxTokensSaida { get; set; }
        public bool ForcarSemFerramentas { get; set; }

        public IAProviderRequest()
        {
            ToolResults = new List<IAToolResult>();
            Historico = new List<IAMensagemHistorico>();
        }
    }

    [Serializable]
    public class IAMensagemHistorico
    {
        public string Papel { get; set; }
        public string Conteudo { get; set; }
    }

    public class IAProviderResponse
    {
        public bool Sucesso { get; set; }
        public string Resposta { get; set; }
        public string Erro { get; set; }
        public string JsonOriginal { get; set; }
        public int TokensEntrada { get; set; }
        public int TokensSaida { get; set; }
        public JArray OutputItens { get; set; }
        public List<IAToolCall> ToolCalls { get; set; }
        public List<IAFonteResposta> Fontes { get; set; }
        public IAAcaoPendente AcaoPendente { get; set; }

        public IAProviderResponse()
        {
            OutputItens = new JArray();
            ToolCalls = new List<IAToolCall>();
            Fontes = new List<IAFonteResposta>();
        }
    }

    public class IAToolCall
    {
        public string CallId { get; set; }
        public string Nome { get; set; }
        public string ArgumentosJson { get; set; }
        public JObject Argumentos { get; set; }
    }

    public class IAToolResult
    {
        public string CallId { get; set; }
        public string Nome { get; set; }
        public string ResultadoJson { get; set; }
        public bool Erro { get; set; }
    }

    public class IAFerramentaDefinicao
    {
        public int IdFerramentaIA { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Modulo { get; set; }
        public int IdRecursoNecessario { get; set; }
        public int MaxRegistros { get; set; }
        public string Escopo { get; set; }
        public bool RequerConfirmacao { get; set; }
        public bool Ativo { get; set; }
        public JObject SchemaParametros { get; set; }

        // Execução genérica (configurada pela tela). Vazio = ferramenta interna (execução no código).
        public string ProcedureGenerica { get; set; }
        public string MapaParametros { get; set; }   // JSON: [{ "param": "@x", "origem": "arg|fixo|token", "valor": "..." }]
        public string ColunasRetorno { get; set; }   // JSON: ["colA","colB",...]
        public int IndiceTabelaRetorno { get; set; }

        // Workflow composto exposto como ferramenta para a IA.
        public int IdWorkflowIA { get; set; }
        public string GrafoJson { get; set; }

        public bool EhGenerica()
        {
            return !string.IsNullOrWhiteSpace(ProcedureGenerica);
        }

        public bool EhWorkflow()
        {
            return IdWorkflowIA > 0 || !string.IsNullOrWhiteSpace(GrafoJson);
        }

        public JObject ParaOpenAI()
        {
            return new JObject
            {
                { "type", "function" },
                { "name", Nome },
                { "description", Descricao },
                { "strict", true },
                { "parameters", SchemaParametros }
            };
        }
    }

    // Workflow agêntico: encadeia ferramentas do catálogo. sGrafoJson guarda os nós+conexões;
    // SchemaParametrosJson descreve os inputs declarados (para a IA, na Fase 3).
    public class IAWorkflow
    {
        public int IdWorkflowIA { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int IdRecursoNecessario { get; set; }
        public string RecursoDescricao { get; set; }
        public string Escopo { get; set; }
        public bool Ativo { get; set; }
        // Arquivado tira o workflow de toda lista/tela (exceto a aba Arquivados), sem apagar nada; exige
        // Ativo = false para arquivar (ver cls_IA_Repositorio.SetWorkflowArquivado).
        public bool Arquivado { get; set; }
        public string SchemaParametrosJson { get; set; }
        public string GrafoJson { get; set; }
    }

    // Execução de workflow (Fase 2): estado persistido para pausar num passo de escrita e retomar.
    public class IAWorkflowExecucao
    {
        public int IdExecucaoIA { get; set; }
        public int IdWorkflowIA { get; set; }
        public int IdConversaIA { get; set; }
        public int IdUsuario { get; set; }
        public string Status { get; set; }        // EXECUTANDO/PAUSADO/CONCLUIDO/ERRO/CANCELADO
        public string EstadoJson { get; set; }     // { entradas, saidas, noAtual, ferramentaPausa, argsPausa }
        public string ResumoAcao { get; set; }
        public string TraceJson { get; set; }
        // Do workflow (join):
        public string WorkflowNome { get; set; }
        public int IdRecursoNecessario { get; set; }
        public string GrafoJson { get; set; }
        public bool WorkflowAtivo { get; set; }
    }

    public class IAFerramentaResultado
    {
        public int IdFerramentaIA { get; set; }
        public int IdRecursoValidado { get; set; }
        public string NomeFerramenta { get; set; }
        public string ArgumentosJson { get; set; }
        public string ResultadoJson { get; set; }
        // Resultado completo permanece na auditoria/trace. Esta projeção menor é enviada ao modelo.
        public string ResultadoModeloJson { get; set; }
        public string Status { get; set; }
        public string Erro { get; set; }
        public long DuracaoMs { get; set; }
        public int IdAprovacaoIA { get; set; }
        public int IdWorkflowExecucaoIA { get; set; }
        public string TipoPausaWorkflow { get; set; }
        public string ResumoAcao { get; set; }
    }

    [Serializable]
    public class IAAcaoPendente
    {
        public int IdAprovacaoIA { get; set; }
        public int IdExecucaoIA { get; set; }
        public string Tipo { get; set; }
        public string Ferramenta { get; set; }
        public string Resumo { get; set; }
        public string Descricao { get; set; }
        public string DadosJson { get; set; }
    }

    [Serializable]
    public class IAAprovacao
    {
        public int IdAprovacaoIA { get; set; }
        public int IdConversaIA { get; set; }
        public string NomeFerramenta { get; set; }
        public string Status { get; set; }
        public string ResumoAcao { get; set; }
        public string ArgumentosJson { get; set; }
        public string ResultadoJson { get; set; }
    }

    public class IAUsoUsuario
    {
        public int IdUsuario { get; set; }
        public int TotalMensagensHoje { get; set; }
        public int TotalConversasHoje { get; set; }
        public int TokensSaidaHoje { get; set; }
        public string DataReferencia { get; set; }
    }

    public class IAManutencaoResultado
    {
        public bool Executou { get; set; }
        public int AuditoriaRemovidas { get; set; }
        public int ArquivosTemporariosRemovidos { get; set; }
    }

    // Resultado do botao "Testar" de uma ferramenta generica (tela de administracao)
    public class IAResultadoTeste
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public bool Simulacao { get; set; }
        public string ParametrosResolvidos { get; set; }
        public List<string> Colunas { get; set; }
        public List<Dictionary<string, string>> Linhas { get; set; }
        public int TotalEncontrado { get; set; }

        public IAResultadoTeste()
        {
            Colunas = new List<string>();
            Linhas = new List<Dictionary<string, string>>();
        }
    }

    [Serializable]
    public class IAArquivoChat
    {
        public int IdArquivoIA { get; set; }
        public int IdConversaIA { get; set; }
        public int IdMensagemIA { get; set; }
        public int IdUsuario { get; set; }
        public string Tipo { get; set; }                  // C = anexo de conversa; B = base de conhecimento
        public string Categoria { get; set; }
        public string TituloConhecimento { get; set; }
        public string NomeOriginal { get; set; }
        public string Extensao { get; set; }
        public string MimeType { get; set; }
        public long TamanhoBytes { get; set; }
        public string TamanhoFormatado { get; set; }
        public string HashSHA256 { get; set; }
        public string Status { get; set; }
        public string Markdown { get; set; }
        public string Resumo { get; set; }
        public string Erro { get; set; }
        public string ArquivoTemporario { get; set; }
        public int TotalCaracteres { get; set; }
        public int TotalTrechos { get; set; }
        public string DtUpload { get; set; }
        public string DtProcessamento { get; set; }
        public string DtExpiracao { get; set; }
        public List<IAArquivoTrecho> Trechos { get; set; }

        public IAArquivoChat()
        {
            Tipo = "C";
            Categoria = string.Empty;
            TituloConhecimento = string.Empty;
            NomeOriginal = string.Empty;
            Extensao = string.Empty;
            MimeType = string.Empty;
            HashSHA256 = string.Empty;
            Status = string.Empty;
            Markdown = string.Empty;
            Resumo = string.Empty;
            Erro = string.Empty;
            ArquivoTemporario = string.Empty;
            DtUpload = string.Empty;
            DtProcessamento = string.Empty;
            DtExpiracao = string.Empty;
            TamanhoFormatado = string.Empty;
            Trechos = new List<IAArquivoTrecho>();
        }
    }

    [Serializable]
    public class IAArquivoTrecho
    {
        public int IdArquivoTrechoIA { get; set; }
        public int IdArquivoIA { get; set; }
        public int Ordem { get; set; }
        public int InicioChar { get; set; }
        public int FimChar { get; set; }
        public int TotalCaracteres { get; set; }
        public string Titulo { get; set; }
        public string ConteudoMarkdown { get; set; }
        public string NomeOriginal { get; set; }
        public string HashSHA256 { get; set; }

        public IAArquivoTrecho()
        {
            Titulo = string.Empty;
            ConteudoMarkdown = string.Empty;
            NomeOriginal = string.Empty;
            HashSHA256 = string.Empty;
        }
    }
}
