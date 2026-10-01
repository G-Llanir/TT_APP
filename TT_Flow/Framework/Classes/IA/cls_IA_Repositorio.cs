using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json.Linq;
using TT.FrameWork;

namespace TT_Flow.FrameWork.IA
{
    public class cls_IA_Repositorio
    {
        private const string Procedure = "sp_Manipula_tbl_Flow_IA_Chat";
        private readonly string _idUsuarioOverride;

        public cls_IA_Repositorio()
            : this(string.Empty)
        {
        }

        public cls_IA_Repositorio(string idUsuarioOverride)
        {
            _idUsuarioOverride = idUsuarioOverride ?? string.Empty;
        }

        public int CriarConversa(string titulo, string modelo)
        {
            return CriarConversa(titulo, modelo, null);
        }

        public int CriarConversa(string titulo, string modelo, IAContextoTela contextoTela)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CRIAR_CONVERSA" },
                { "@idUsuario", IdUsuarioAtual() },
                { "@idColaborador", TT.FrameWork.Identity.Variaveis.idColaborador() },
                { "@sTitulo", titulo },
                { "@sOrigem", "TT_FLOW" },
                { "@sModelo", modelo },
                { "@sContextoTela", contextoTela == null ? string.Empty : (contextoTela.Tela ?? string.Empty) },
                { "@nIdContexto", contextoTela == null ? "0" : contextoTela.IdRegistro.ToString() }
            };

            DataSet ds = BD.ExecutarDataSet(Procedure, parametros);
            return ParaInt(BD.Retorno.DATASET(ds, "idConversaIA"));
        }

        public int SalvarMensagem(int idConversaIA, string papel, string conteudo, string jsonOriginal, int tokens)
        {
            return SalvarMensagem(idConversaIA, papel, conteudo, jsonOriginal, tokens, 0);
        }

        public int SalvarMensagem(int idConversaIA, string papel, string conteudo, string jsonOriginal, int tokens, int tokensEntrada)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_MENSAGEM" },
                { "@idConversaIA", idConversaIA.ToString() },
                { "@idUsuario", IdUsuarioAtual() },
                { "@sPapel", papel },
                { "@sConteudo", conteudo },
                { "@sConteudoResumo", cls_IA_Sanitizacao.Resumir(conteudo, 500) },
                { "@sJsonOriginal", jsonOriginal ?? string.Empty },
                { "@nTokens", tokens.ToString() },
                { "@nTokensEntrada", tokensEntrada.ToString() }
            };

            DataSet ds = BD.ExecutarDataSet(Procedure, parametros);
            return ParaInt(BD.Retorno.DATASET(ds, "idMensagemIA"));
        }

        public int SalvarTemplate(string titulo, string conteudo, string contextoTela)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_TEMPLATE" },
                { "@idUsuario", IdUsuarioAtual() },
                { "@sTitulo", titulo },
                { "@sConteudo", conteudo },
                { "@sContextoTela", contextoTela ?? string.Empty }
            };

            DataSet ds = BD.ExecutarDataSet(Procedure, parametros);
            return ParaInt(BD.Retorno.DATASET(ds, "idTemplatePromptIA"));
        }

        public List<IATemplatePrompt> ListarTemplates()
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "LISTAR_TEMPLATES" },
                { "@idUsuario", IdUsuarioAtual() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            List<IATemplatePrompt> templates = new List<IATemplatePrompt>();

            foreach (DataRow row in tb.Rows)
            {
                templates.Add(new IATemplatePrompt
                {
                    IdTemplatePromptIA = ParaInt(Valor(row, "idTemplatePromptIA")),
                    Titulo = Valor(row, "sTitulo"),
                    Conteudo = Valor(row, "sConteudo"),
                    ContextoTela = Valor(row, "sContextoTela")
                });
            }

            return templates;
        }

        public bool DeletarTemplate(int idTemplatePromptIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "DELETAR_TEMPLATE" },
                { "@idUsuario", IdUsuarioAtual() },
                { "@idTemplatePromptIA", idTemplatePromptIA.ToString() }
            };

            DataSet ds = BD.ExecutarDataSet(Procedure, parametros);
            return ParaInt(BD.Retorno.DATASET(ds, "nAfetados")) > 0;
        }

        public DataSet ConsultarUsoPeriodo()
        {
            return ConsultarUsoPeriodo(30);
        }

        public DataSet ConsultarUsoPeriodo(int dias)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_USO_PERIODO" },
                { "@nLimite", dias.ToString() }
            };

            return BD.ExecutarDataSet(Procedure, parametros);
        }

        public int SalvarAprovacao(int idChamadaFerramentaIA, string resumoAcao, string argumentosJson)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_APROVACAO" },
                { "@idUsuario", IdUsuarioAtual() },
                { "@idChamadaFerramentaIA", idChamadaFerramentaIA.ToString() },
                { "@sResumoAcao", resumoAcao ?? string.Empty },
                { "@sArgumentosJson", argumentosJson ?? string.Empty }
            };

            DataSet ds = BD.ExecutarDataSet(Procedure, parametros);
            return ParaInt(BD.Retorno.DATASET(ds, "idAprovacaoIA"));
        }

        public IAAprovacao ConsultarAprovacao(int idAprovacaoIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_APROVACAO" },
                { "@idUsuario", IdUsuarioAtual() },
                { "@idAprovacaoIA", idAprovacaoIA.ToString() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            if (tb == null || tb.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = tb.Rows[0];
            return new IAAprovacao
            {
                IdAprovacaoIA = ParaInt(Valor(row, "idAprovacaoIA")),
                IdConversaIA = ParaInt(Valor(row, "idConversaIA")),
                NomeFerramenta = Valor(row, "sNomeFerramenta"),
                Status = Valor(row, "sStatus"),
                ResumoAcao = Valor(row, "sResumoAcao"),
                ArgumentosJson = Valor(row, "sArgumentosJson"),
                ResultadoJson = Valor(row, "sResultadoJson")
            };
        }

        public bool AtualizarAprovacao(int idAprovacaoIA, string status)
        {
            return AtualizarAprovacao(idAprovacaoIA, status, string.Empty);
        }

        public bool AtualizarAprovacao(int idAprovacaoIA, string status, string resultadoJson)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "ATUALIZAR_APROVACAO" },
                { "@idUsuario", IdUsuarioAtual() },
                { "@idAprovacaoIA", idAprovacaoIA.ToString() },
                { "@sStatus", status ?? string.Empty },
                { "@sResultadoJson", resultadoJson ?? string.Empty }
            };

            DataSet ds = BD.ExecutarDataSet(Procedure, parametros);
            return ParaInt(BD.Retorno.DATASET(ds, "nAfetados")) > 0;
        }

        public bool AdicionarObservacaoPedido(int idPedido, string observacao)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "ADICIONAR_OBSERVACAO_PEDIDO" },
                { "@idUsuario", IdUsuarioAtual() },
                { "@idPedido", idPedido.ToString() },
                { "@sConteudo", observacao ?? string.Empty }
            };

            DataSet ds = BD.ExecutarDataSet(Procedure, parametros);
            return ParaInt(BD.Retorno.DATASET(ds, "nAfetados")) > 0;
        }

        public int SalvarChamadaFerramenta(int idConversaIA, IAFerramentaResultado resultado)
        {
            if (resultado == null)
            {
                return 0;
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_CHAMADA_FERRAMENTA" },
                { "@idConversaIA", idConversaIA.ToString() },
                { "@idUsuario", IdUsuarioAtual() },
                { "@idFerramentaIA", resultado.IdFerramentaIA.ToString() },
                { "@sNomeFerramenta", resultado.NomeFerramenta ?? string.Empty },
                { "@sArgumentosJson", resultado.ArgumentosJson ?? string.Empty },
                { "@sResultadoJson", resultado.ResultadoJson ?? string.Empty },
                { "@sStatus", resultado.Status ?? string.Empty },
                { "@sErro", resultado.Erro ?? string.Empty },
                { "@idRecursoValidado", resultado.IdRecursoValidado.ToString() },
                { "@nDuracaoMs", resultado.DuracaoMs.ToString() }
            };

            DataSet ds = BD.ExecutarDataSet(Procedure, parametros);
            return ParaInt(BD.Retorno.DATASET(ds, "idChamadaFerramentaIA"));
        }

        public List<IAConversaResumo> ListarConversas()
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_CONVERSAS" },
                { "@idUsuario", IdUsuarioAtual() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            List<IAConversaResumo> conversas = new List<IAConversaResumo>();

            foreach (DataRow row in tb.Rows)
            {
                conversas.Add(new IAConversaResumo
                {
                    IdConversaIA = ParaInt(Valor(row, "idConversaIA")),
                    Titulo = Valor(row, "sTitulo"),
                    Status = Valor(row, "sStatus"),
                    DtUltimaMensagem = Valor(row, "dtUltimaMensagem")
                });
            }

            return conversas;
        }

        public List<IAConversaResumo> ListarConversasPaginado(int pagina, int tamanhoPagina, string termo)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_CONVERSAS_PAGINADO" },
                { "@idUsuario", IdUsuarioAtual() },
                { "@nPagina", pagina.ToString() },
                { "@nLimite", tamanhoPagina.ToString() },
                { "@sTermoConversa", termo ?? string.Empty }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            List<IAConversaResumo> conversas = new List<IAConversaResumo>();

            foreach (DataRow row in tb.Rows)
            {
                conversas.Add(new IAConversaResumo
                {
                    IdConversaIA = ParaInt(Valor(row, "idConversaIA")),
                    Titulo = Valor(row, "sTitulo"),
                    Status = Valor(row, "sStatus"),
                    Favorita = Valor(row, "sFavorita") == "S",
                    DtUltimaMensagem = Valor(row, "dtUltimaMensagem")
                });
            }

            return conversas;
        }

        public bool FavoritarConversa(int idConversaIA, bool favorita)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "FAVORITAR_CONVERSA" },
                { "@idUsuario", IdUsuarioAtual() },
                { "@idConversaIA", idConversaIA.ToString() },
                { "@sFavorita", favorita ? "S" : "N" }
            };

            DataSet ds = BD.ExecutarDataSet(Procedure, parametros);
            return ParaInt(BD.Retorno.DATASET(ds, "nAfetados")) > 0;
        }

        public bool DeletarConversa(int idConversaIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "DELETAR_CONVERSA" },
                { "@idUsuario", IdUsuarioAtual() },
                { "@idConversaIA", idConversaIA.ToString() }
            };

            DataSet ds = BD.ExecutarDataSet(Procedure, parametros);
            return ParaInt(BD.Retorno.DATASET(ds, "nAfetados")) > 0;
        }

        // Versao leve para montar a memoria enviada ao provider: so papel + conteudo resumido,
        // sem carregar arquivos e fontes por mensagem como o CarregarMensagens faz.
        public List<IAMensagemHistorico> CarregarHistoricoProvider(int idConversaIA, int maxMensagens, int maxCharsPorMensagem)
        {
            List<IAMensagemHistorico> historico = new List<IAMensagemHistorico>();

            if (idConversaIA <= 0 || maxMensagens <= 0)
            {
                return historico;
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_MENSAGENS" },
                { "@idUsuario", IdUsuarioAtual() },
                { "@idConversaIA", idConversaIA.ToString() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);

            int inicio = Math.Max(0, tb.Rows.Count - maxMensagens);
            for (int i = inicio; i < tb.Rows.Count; i++)
            {
                DataRow row = tb.Rows[i];
                string papel = Valor(row, "sPapel").Trim().ToUpperInvariant();

                if (papel != "USER" && papel != "ASSISTANT")
                {
                    continue;
                }

                string conteudo = cls_IA_Sanitizacao.Resumir(Valor(row, "sConteudo"), maxCharsPorMensagem);
                if (string.IsNullOrWhiteSpace(conteudo))
                {
                    continue;
                }

                string contextoWorkflow = ResumoWorkflowHistorico(Valor(row, "sJsonOriginal"));
                if (!string.IsNullOrWhiteSpace(contextoWorkflow))
                {
                    conteudo = cls_IA_Sanitizacao.Resumir(
                        conteudo + "\n\n[Contexto técnico persistido do workflow concluído]\n" + contextoWorkflow,
                        Math.Max(maxCharsPorMensagem, 3500));
                }

                historico.Add(new IAMensagemHistorico
                {
                    Papel = papel,
                    Conteudo = conteudo
                });
            }

            return historico;
        }

        public List<IAMensagem> CarregarMensagens(int idConversaIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_MENSAGENS" },
                { "@idUsuario", IdUsuarioAtual() },
                { "@idConversaIA", idConversaIA.ToString() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            List<IAMensagem> mensagens = new List<IAMensagem>();

            foreach (DataRow row in tb.Rows)
            {
                string papel = Valor(row, "sPapel");
                string jsonOriginal = Valor(row, "sJsonOriginal");
                List<IAFonteResposta> fontes = ExtrairFontesMensagem(jsonOriginal);

                IAMensagem mensagem = new IAMensagem
                {
                    IdMensagemIA = ParaInt(Valor(row, "idMensagemIA")),
                    IdConversaIA = ParaInt(Valor(row, "idConversaIA")),
                    Ordem = ParaInt(Valor(row, "nOrdem")),
                    Papel = papel,
                    Conteudo = NormalizarConteudoMensagem(papel, Valor(row, "sConteudo"), fontes),
                    DtInclusao = Valor(row, "dtInclusao"),
                    Avaliacao = Valor(row, "sAvaliacao"),
                    AvaliacaoComentario = Valor(row, "sComentarioAvaliacao"),
                    Fontes = fontes
                };

                mensagem.Arquivos = ConsultarArquivosMensagem(mensagem.IdMensagemIA);
                mensagens.Add(mensagem);
            }

            return mensagens;
        }

        public DataTable ConsultarAuditoria(int idConversaIA, int idUsuarioFiltro, string dtInicial, string dtFinal, string evento, string severidade, string busca)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_AUDITORIA" },
                { "@idConversaIA", idConversaIA.ToString() },
                { "@idUsuarioFiltro", idUsuarioFiltro.ToString() },
                { "@dtInicial", dtInicial ?? string.Empty },
                { "@dtFinal", dtFinal ?? string.Empty },
                { "@sEvento", evento ?? string.Empty },
                { "@sSeveridadeFiltro", severidade ?? string.Empty },
                { "@sBusca", busca ?? string.Empty }
            };

            return BD.ExecutarDataTable(Procedure, parametros, false);
        }

        public DataTable ConsultarChamadasFerramenta(int idConversaIA, int idUsuarioFiltro, string dtInicial, string dtFinal, string nomeFerramenta, string status, string busca)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_CHAMADAS_FERRAMENTA" },
                { "@idConversaIA", idConversaIA.ToString() },
                { "@idUsuarioFiltro", idUsuarioFiltro.ToString() },
                { "@dtInicial", dtInicial ?? string.Empty },
                { "@dtFinal", dtFinal ?? string.Empty },
                { "@sNomeFerramenta", nomeFerramenta ?? string.Empty },
                { "@sStatus", status ?? string.Empty },
                { "@sBusca", busca ?? string.Empty }
            };

            return BD.ExecutarDataTable(Procedure, parametros, false);
        }

        public DataTable ConsultarAvaliacoes(int idConversaIA, int idUsuarioFiltro, string dtInicial, string dtFinal, string avaliacao, string busca)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_AVALIACOES" },
                { "@idConversaIA", idConversaIA.ToString() },
                { "@idUsuarioFiltro", idUsuarioFiltro.ToString() },
                { "@dtInicial", dtInicial ?? string.Empty },
                { "@dtFinal", dtFinal ?? string.Empty },
                { "@sAvaliacao", avaliacao ?? string.Empty },
                { "@sBusca", busca ?? string.Empty }
            };

            return BD.ExecutarDataTable(Procedure, parametros, false);
        }

        // Valores distintos de sEvento para popular o dropdown de filtro (Chosen) da tela de Auditoria.
        public DataTable ListarEventosDistintos()
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "LISTAR_EVENTOS_DISTINTOS" }
            };

            return BD.ExecutarDataTable(Procedure, parametros, false);
        }

        public DataTable ConsultarConfig()
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_CONFIG" }
            };

            return BD.ExecutarDataTable(Procedure, parametros, false);
        }

        public void SalvarConfig(string chave, string valor)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_CONFIG" },
                { "@idUsuario", IdUsuarioAtual() },
                { "@sChave", chave ?? string.Empty },
                { "@sValor", valor ?? string.Empty }
            };

            BD.ExecutarDataTable(Procedure, parametros, false);
        }

        public DataTable SalvarAvaliacao(int idMensagemIA, string avaliacao, string comentario)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_AVALIACAO" },
                { "@idUsuario", IdUsuarioAtual() },
                { "@idMensagemIA", idMensagemIA.ToString() },
                { "@sAvaliacao", avaliacao ?? string.Empty },
                { "@sComentario", comentario ?? string.Empty }
            };

            return BD.ExecutarDataTable(Procedure, parametros, false);
        }

        public IAUsoUsuario ConsultarUsoUsuarioHoje()
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_USO_USUARIO_HOJE" },
                { "@idUsuario", IdUsuarioAtual() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            IAUsoUsuario uso = new IAUsoUsuario
            {
                IdUsuario = ParaInt(IdUsuarioAtual())
            };

            if (tb.Rows.Count > 0)
            {
                DataRow row = tb.Rows[0];
                uso.IdUsuario = ParaInt(Valor(row, "idUsuario"));
                uso.TotalMensagensHoje = ParaInt(Valor(row, "nMensagensUsuarioHoje"));
                uso.TotalConversasHoje = ParaInt(Valor(row, "nConversasHoje"));
                uso.TokensSaidaHoje = ParaInt(Valor(row, "nTokensSaidaHoje"));
                uso.DataReferencia = Valor(row, "dtReferencia");
            }

            return uso;
        }

        // Conta mensagens do usuario (evento de auditoria) originadas do mesmo IP hoje.
        // Usado no rate limit por IP, complementar ao rate limit por usuario.
        public int ConsultarMensagensIpHoje(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                return 0;
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_USO_IP_HOJE" },
                { "@sIP", ip }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            return tb.Rows.Count > 0 ? ParaInt(Valor(tb.Rows[0], "nMensagensIpHoje")) : 0;
        }

        // Executa a limpeza/retencao de dados da IA. A propria SP faz o throttle (1x/~20h),
        // salvo quando forcar = true (execucao manual pelo admin).
        public IAManutencaoResultado ExecutarManutencao(int retencaoDias, bool forcar)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "EXECUTAR_MANUTENCAO" },
                { "@nRetencaoDias", retencaoDias.ToString() },
                { "@sForcar", forcar ? "S" : "N" }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            IAManutencaoResultado resultado = new IAManutencaoResultado();
            if (tb.Rows.Count > 0)
            {
                resultado.Executou = ParaInt(Valor(tb.Rows[0], "nExecutou")) == 1;
                resultado.AuditoriaRemovidas = ParaInt(Valor(tb.Rows[0], "nAuditoriaRemovidas"));
            }

            return resultado;
        }

        // Salva um rascunho de mensagem proposto pela IA (nao envia). Retorna o id gerado.
        public int SalvarRascunho(int idConversaIA, string destino, string assunto, string corpo)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_RASCUNHO" },
                { "@idUsuario", IdUsuarioAtual() },
                { "@idConversaIA", idConversaIA.ToString() },
                { "@sRascunhoDestino", destino ?? string.Empty },
                { "@sRascunhoAssunto", assunto ?? string.Empty },
                { "@sRascunhoCorpo", corpo ?? string.Empty }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            return tb.Rows.Count > 0 ? ParaInt(Valor(tb.Rows[0], "idRascunhoIA")) : 0;
        }

        // Catalogo de ferramentas persistido: le tbl_Flow_IA_Ferramentas (metadados).
        public List<IAFerramentaDefinicao> ListarFerramentas(bool incluirInativas)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "LISTAR_FERRAMENTAS" },
                { "@sIncluirInativas", incluirInativas ? "S" : "N" }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            List<IAFerramentaDefinicao> lista = new List<IAFerramentaDefinicao>();

            foreach (DataRow row in tb.Rows)
            {
                IAFerramentaDefinicao ferramenta = new IAFerramentaDefinicao
                {
                    IdFerramentaIA = ParaInt(Valor(row, "idFerramentaIA")),
                    Nome = Valor(row, "sNomeInterno"),
                    Descricao = Valor(row, "sDescricao"),
                    Modulo = Valor(row, "sModulo"),
                    IdRecursoNecessario = ParaInt(Valor(row, "idRecursoNecessario")),
                    Escopo = Valor(row, "sEscopo"),
                    Ativo = Valor(row, "sAtivo") == "S",
                    RequerConfirmacao = Valor(row, "sRequerConfirmacao") == "S",
                    MaxRegistros = ParaInt(Valor(row, "nMaxRegistros")),
                    ProcedureGenerica = Valor(row, "sProcedureGenerica"),
                    MapaParametros = Valor(row, "sMapaParametros"),
                    ColunasRetorno = Valor(row, "sColunasRetorno"),
                    IndiceTabelaRetorno = ParaInt(Valor(row, "nIndiceTabelaRetorno"))
                };

                string schema = Valor(row, "sSchemaParametros");
                if (!string.IsNullOrWhiteSpace(schema))
                {
                    try { ferramenta.SchemaParametros = JObject.Parse(schema); }
                    catch { ferramenta.SchemaParametros = null; }
                }

                lista.Add(ferramenta);
            }

            return lista;
        }

        // Insere uma ferramenta apenas se ainda nao existe (usado no seed a partir do codigo).
        public int InserirFerramentaSeAusente(IAFerramentaDefinicao ferramenta)
        {
            if (ferramenta == null)
            {
                return 0;
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SEED_FERRAMENTA" },
                { "@sNomeFerramenta", ferramenta.Nome ?? string.Empty },
                { "@sDescricaoFerr", ferramenta.Descricao ?? string.Empty },
                { "@sModuloFerr", ferramenta.Modulo ?? string.Empty },
                { "@idRecursoFerr", ferramenta.IdRecursoNecessario.ToString() },
                { "@sEscopoFerr", string.IsNullOrWhiteSpace(ferramenta.Escopo) ? "READ" : ferramenta.Escopo },
                { "@sRequerConfirmacaoFerr", ferramenta.RequerConfirmacao ? "S" : "N" },
                { "@nMaxRegistrosFerr", ferramenta.MaxRegistros.ToString() },
                { "@sSchemaFerr", ferramenta.SchemaParametros != null ? ferramenta.SchemaParametros.ToString(Newtonsoft.Json.Formatting.None) : string.Empty }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            return tb.Rows.Count > 0 ? ParaInt(Valor(tb.Rows[0], "idFerramentaIA")) : 0;
        }

        public bool SetFerramentaAtiva(string nomeInterno, bool ativo)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SET_FERRAMENTA_ATIVA" },
                { "@sNomeFerramenta", nomeInterno ?? string.Empty },
                { "@sAtivoFerr", ativo ? "S" : "N" }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            return tb.Rows.Count > 0 && ParaInt(Valor(tb.Rows[0], "nRet")) > 0;
        }

        public IAFerramentaDefinicao ObterFerramenta(int idFerramentaIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "OBTER_FERRAMENTA" },
                { "@idFerramentaIA", idFerramentaIA.ToString() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            if (tb.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = tb.Rows[0];
            IAFerramentaDefinicao ferramenta = new IAFerramentaDefinicao
            {
                IdFerramentaIA = ParaInt(Valor(row, "idFerramentaIA")),
                Nome = Valor(row, "sNomeInterno"),
                Descricao = Valor(row, "sDescricao"),
                Modulo = Valor(row, "sModulo"),
                IdRecursoNecessario = ParaInt(Valor(row, "idRecursoNecessario")),
                Escopo = Valor(row, "sEscopo"),
                Ativo = Valor(row, "sAtivo") == "S",
                RequerConfirmacao = Valor(row, "sRequerConfirmacao") == "S",
                MaxRegistros = ParaInt(Valor(row, "nMaxRegistros")),
                ProcedureGenerica = Valor(row, "sProcedureGenerica"),
                MapaParametros = Valor(row, "sMapaParametros"),
                ColunasRetorno = Valor(row, "sColunasRetorno"),
                IndiceTabelaRetorno = ParaInt(Valor(row, "nIndiceTabelaRetorno"))
            };

            string schema = Valor(row, "sSchemaParametros");
            if (!string.IsNullOrWhiteSpace(schema))
            {
                try { ferramenta.SchemaParametros = JObject.Parse(schema); }
                catch { ferramenta.SchemaParametros = null; }
            }

            return ferramenta;
        }

        // Cria (idFerramentaIA = 0) ou atualiza uma ferramenta. Retorna o id salvo (0 em erro) e a mensagem da SP.
        public int SalvarFerramenta(IAFerramentaDefinicao ferramenta, string schemaJson, out string mensagem)
        {
            mensagem = string.Empty;
            if (ferramenta == null)
            {
                return 0;
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_FERRAMENTA" },
                { "@idFerramentaIA", ferramenta.IdFerramentaIA.ToString() },
                { "@sNomeFerramenta", ferramenta.Nome ?? string.Empty },
                { "@sDescricaoFerr", ferramenta.Descricao ?? string.Empty },
                { "@sModuloFerr", ferramenta.Modulo ?? string.Empty },
                { "@idRecursoFerr", ferramenta.IdRecursoNecessario.ToString() },
                { "@sEscopoFerr", string.IsNullOrWhiteSpace(ferramenta.Escopo) ? "READ" : ferramenta.Escopo },
                { "@sRequerConfirmacaoFerr", ferramenta.RequerConfirmacao ? "S" : "N" },
                { "@nMaxRegistrosFerr", ferramenta.MaxRegistros.ToString() },
                { "@sAtivoFerr", ferramenta.Ativo ? "S" : "N" },
                { "@sSchemaFerr", schemaJson ?? string.Empty },
                { "@sProcedureGenerica", ferramenta.ProcedureGenerica ?? string.Empty },
                { "@sMapaParametros", ferramenta.MapaParametros ?? string.Empty },
                { "@sColunasRetorno", ferramenta.ColunasRetorno ?? string.Empty },
                { "@nIndiceTabelaRetorno", ferramenta.IndiceTabelaRetorno.ToString() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            if (tb.Rows.Count == 0)
            {
                return 0;
            }

            DataRow row = tb.Rows[0];
            mensagem = Valor(row, "sMsg");
            if (ParaInt(Valor(row, "nRet")) != 0)
            {
                return 0;
            }

            return ParaInt(Valor(row, "idFerramentaIA"));
        }

        // ===================== WORKFLOWS =====================
        public List<IAWorkflow> ListarWorkflows(bool incluirInativos)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "LISTAR_WORKFLOWS" },
                { "@sIncluirInativas", incluirInativos ? "S" : "N" }
            };

            List<IAWorkflow> lista = new List<IAWorkflow>();
            foreach (DataRow row in BD.ExecutarDataTable(Procedure, parametros, false).Rows)
            {
                lista.Add(MapearWorkflow(row));
            }

            PreencherDescricoesRecursos(lista);
            return lista;
        }

        private static void PreencherDescricoesRecursos(List<IAWorkflow> workflows)
        {
            if (workflows == null || workflows.Count == 0)
            {
                return;
            }

            bool precisaBuscar = false;
            foreach (IAWorkflow workflow in workflows)
            {
                if (workflow != null && workflow.IdRecursoNecessario > 0 && string.IsNullOrWhiteSpace(workflow.RecursoDescricao))
                {
                    precisaBuscar = true;
                    break;
                }
            }

            if (!precisaBuscar)
            {
                return;
            }

            try
            {
                DataTable recursos = BD.ExecutarDataTable("sp_Select 'Recursos'");
                Dictionary<int, string> descricoes = new Dictionary<int, string>();
                foreach (DataRow row in recursos.Rows)
                {
                    int id = ParaInt(Valor(row, "idRecurso"));
                    string descricao = Valor(row, "sDscRecurso");
                    if (id > 0 && !descricoes.ContainsKey(id))
                    {
                        descricoes.Add(id, descricao);
                    }
                }

                foreach (IAWorkflow workflow in workflows)
                {
                    string descricao;
                    if (workflow != null
                        && workflow.IdRecursoNecessario > 0
                        && string.IsNullOrWhiteSpace(workflow.RecursoDescricao)
                        && descricoes.TryGetValue(workflow.IdRecursoNecessario, out descricao))
                    {
                        workflow.RecursoDescricao = descricao;
                    }
                }
            }
            catch
            {
                // A tela continua exibindo o id da permissão se a consulta auxiliar falhar.
            }
        }

        public IAWorkflow ObterWorkflow(int idWorkflowIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "OBTER_WORKFLOW" },
                { "@idWorkflowIA", idWorkflowIA.ToString() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            return tb.Rows.Count == 0 ? null : MapearWorkflow(tb.Rows[0]);
        }

        // Cria (idWorkflowIA = 0) ou atualiza. Reusa os parametros @s*Ferr/@idRecursoFerr da SP (carriers genericos).
        public int SalvarWorkflow(IAWorkflow workflow, out string mensagem)
        {
            mensagem = string.Empty;
            if (workflow == null)
            {
                return 0;
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_WORKFLOW" },
                { "@idWorkflowIA", workflow.IdWorkflowIA.ToString() },
                { "@sNomeFerramenta", workflow.Nome ?? string.Empty },
                { "@sDescricaoFerr", workflow.Descricao ?? string.Empty },
                { "@idRecursoFerr", workflow.IdRecursoNecessario.ToString() },
                { "@sEscopoFerr", string.IsNullOrWhiteSpace(workflow.Escopo) ? "READ" : workflow.Escopo },
                { "@sAtivoFerr", workflow.Ativo ? "S" : "N" },
                { "@sSchemaFerr", workflow.SchemaParametrosJson ?? string.Empty },
                { "@sGrafoJson", workflow.GrafoJson ?? string.Empty }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            if (tb.Rows.Count == 0)
            {
                return 0;
            }

            DataRow row = tb.Rows[0];
            mensagem = Valor(row, "sMsg");
            if (ParaInt(Valor(row, "nRet")) != 0)
            {
                return 0;
            }

            return ParaInt(Valor(row, "idWorkflowIA"));
        }

        public bool SetWorkflowAtivo(int idWorkflowIA, bool ativo)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SET_WORKFLOW_ATIVO" },
                { "@idWorkflowIA", idWorkflowIA.ToString() },
                { "@sAtivoFerr", ativo ? "S" : "N" }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            return tb.Rows.Count > 0 && ParaInt(Valor(tb.Rows[0], "nRet")) > 0;
        }

        // Arquivar tira o workflow de toda lista/tela (exceto ListarWorkflowsArquivados), sem apagar nada -
        // reversivel por SetWorkflowArquivado(id, false). O banco só arquiva quem já está inativo (nRet=-1,
        // mensagem explicando, senão); desarquivar não tem pré-condição.
        public bool SetWorkflowArquivado(int idWorkflowIA, bool arquivar, out string mensagem)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SET_WORKFLOW_ARQUIVADO" },
                { "@idWorkflowIA", idWorkflowIA.ToString() },
                { "@sAtivoFerr", arquivar ? "S" : "N" }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            if (tb.Rows.Count == 0)
            {
                mensagem = "Não foi possível atualizar o workflow.";
                return false;
            }

            mensagem = Valor(tb.Rows[0], "msg");
            return ParaInt(Valor(tb.Rows[0], "nRet")) > 0;
        }

        // Só os arquivados - usado pela aba "Arquivados" de Workflows.aspx.
        public List<IAWorkflow> ListarWorkflowsArquivados()
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "LISTAR_WORKFLOWS_ARQUIVADOS" }
            };

            List<IAWorkflow> lista = new List<IAWorkflow>();
            foreach (DataRow row in BD.ExecutarDataTable(Procedure, parametros, false).Rows)
            {
                lista.Add(MapearWorkflow(row));
            }

            PreencherDescricoesRecursos(lista);
            return lista;
        }

        // ---- Exportar/Importar workflow (grafo + metadados) ----
        // Procedures dedicadas (nao passam pelo dispatch de @sFuncao) - ver SQL/IA/20_exportar_importar_workflow.sql.
        // Ambas fazem THROW no proprio banco para nome inexistente ou pacote invalido; a excecao sobe com a
        // mensagem pronta para o usuario (o chamador so precisa expo-la, sem reclassificar).

        public string ExportarWorkflow(string sNomeInterno)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sNomeInterno", sNomeInterno ?? string.Empty }
            };

            DataTable tb = BD.ExecutarDataTable("dbo.sp_IA_Workflow_Exportar", parametros, false);
            return tb.Rows.Count > 0 ? Valor(tb.Rows[0], "workflowJson") : string.Empty;
        }

        // UPDATE se sNomeInterno ja existe (preserva idWorkflowIA e execucoes ligadas a ele), INSERT senao.
        public bool ImportarWorkflow(string sWorkflowJson, out int idWorkflowIA, out string sNomeInterno)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sWorkflowJson", sWorkflowJson ?? string.Empty }
            };

            DataTable tb = BD.ExecutarDataTable("dbo.sp_IA_Workflow_Importar", parametros, false);
            if (tb.Rows.Count == 0)
            {
                idWorkflowIA = 0;
                sNomeInterno = string.Empty;
                return false;
            }

            idWorkflowIA = ParaInt(Valor(tb.Rows[0], "idWorkflowIA"));
            sNomeInterno = Valor(tb.Rows[0], "sNomeInterno");
            return true;
        }

        // ---- Execucoes de workflow (Fase 2) ----
        // Cria (id=0) ou atualiza a execucao. Retorna o idExecucaoIA.
        public int SalvarWorkflowExecucao(IAWorkflowExecucao ex)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_WORKFLOW_EXECUCAO" },
                { "@idExecucaoIA", ex.IdExecucaoIA.ToString() },
                { "@idWorkflowIA", ex.IdWorkflowIA.ToString() },
                { "@idConversaIA", ex.IdConversaIA.ToString() },
                { "@idUsuario", ex.IdUsuario > 0 ? ex.IdUsuario.ToString() : IdUsuarioAtual() },
                { "@sStatus", ex.Status ?? string.Empty },
                { "@sEstadoJson", ex.EstadoJson ?? string.Empty },
                { "@sResumoAcao", ex.ResumoAcao ?? string.Empty },
                { "@sTraceJson", ex.TraceJson ?? string.Empty }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            return tb.Rows.Count > 0 ? ParaInt(Valor(tb.Rows[0], "idExecucaoIA")) : 0;
        }

        public IAWorkflowExecucao IniciarOuObterWorkflowExecucao(IAWorkflowExecucao ex, out bool criada)
        {
            criada = false;
            if (ex == null || ex.IdConversaIA <= 0)
            {
                return null;
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "INICIAR_OU_OBTER_WORKFLOW_EXECUCAO" },
                { "@idExecucaoIA", "0" },
                { "@idWorkflowIA", ex.IdWorkflowIA.ToString() },
                { "@idConversaIA", ex.IdConversaIA.ToString() },
                { "@idUsuario", ex.IdUsuario > 0 ? ex.IdUsuario.ToString() : IdUsuarioAtual() },
                { "@sStatus", ex.Status ?? string.Empty },
                { "@sEstadoJson", ex.EstadoJson ?? string.Empty },
                { "@sResumoAcao", ex.ResumoAcao ?? string.Empty },
                { "@sTraceJson", ex.TraceJson ?? string.Empty }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            if (tb.Rows.Count == 0)
            {
                return null;
            }

            criada = Valor(tb.Rows[0], "bNovaExecucao") == "True" || Valor(tb.Rows[0], "bNovaExecucao") == "1";
            return MapearWorkflowExecucao(tb.Rows[0]);
        }

        public IAWorkflowExecucao ObterWorkflowExecucao(int idExecucaoIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "OBTER_WORKFLOW_EXECUCAO" },
                { "@idExecucaoIA", idExecucaoIA.ToString() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            if (tb.Rows.Count == 0) return null;

            return MapearWorkflowExecucao(tb.Rows[0]);
        }

        public IAWorkflowExecucao ObterWorkflowExecucaoAtivaPorConversa(int idConversaIA)
        {
            if (idConversaIA <= 0)
            {
                return null;
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "OBTER_WORKFLOW_EXECUCAO_ATIVA_CONVERSA" },
                { "@idConversaIA", idConversaIA.ToString() },
                { "@idUsuario", IdUsuarioAtual() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            return tb.Rows.Count == 0 ? null : MapearWorkflowExecucao(tb.Rows[0]);
        }

        public List<IAWorkflowExecucao> ListarWorkflowExecucoesPausadas(int limite)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "LISTAR_WORKFLOW_EXECUCOES_PAUSADAS" },
                { "@idUsuario", IdUsuarioAtual() },
                { "@nLimite", limite <= 0 ? "50" : Math.Min(limite, 200).ToString() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            List<IAWorkflowExecucao> lista = new List<IAWorkflowExecucao>();
            foreach (DataRow row in tb.Rows)
            {
                lista.Add(MapearWorkflowExecucao(row));
            }
            return lista;
        }

        public bool MarcarWorkflowExecucaoExecutando(int idExecucaoIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "MARCAR_WORKFLOW_EXECUTANDO" },
                { "@idExecucaoIA", idExecucaoIA.ToString() },
                { "@idUsuario", IdUsuarioAtual() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            return tb.Rows.Count > 0 && ParaInt(Valor(tb.Rows[0], "nRet")) > 0;
        }

        public bool CancelarWorkflowExecucaoPausada(int idExecucaoIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CANCELAR_WORKFLOW_EXECUCAO" },
                { "@idExecucaoIA", idExecucaoIA.ToString() },
                { "@idUsuario", IdUsuarioAtual() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            return tb.Rows.Count > 0 && ParaInt(Valor(tb.Rows[0], "nRet")) > 0;
        }

        private static IAWorkflow MapearWorkflow(DataRow row)
        {
            return new IAWorkflow
            {
                IdWorkflowIA = ParaInt(Valor(row, "idWorkflowIA")),
                Nome = Valor(row, "sNomeInterno"),
                Descricao = Valor(row, "sDescricao"),
                IdRecursoNecessario = ParaInt(Valor(row, "idRecursoNecessario")),
                RecursoDescricao = Valor(row, "sDscRecurso"),
                Escopo = Valor(row, "sEscopo"),
                Ativo = Valor(row, "sAtivo") == "S",
                Arquivado = Valor(row, "sArquivado") == "S",
                SchemaParametrosJson = Valor(row, "sSchemaParametros"),
                GrafoJson = Valor(row, "sGrafoJson")
            };
        }

        private static IAWorkflowExecucao MapearWorkflowExecucao(DataRow row)
        {
            return new IAWorkflowExecucao
            {
                IdExecucaoIA = ParaInt(Valor(row, "idExecucaoIA")),
                IdWorkflowIA = ParaInt(Valor(row, "idWorkflowIA")),
                IdConversaIA = ParaInt(Valor(row, "idConversaIA")),
                IdUsuario = ParaInt(Valor(row, "idUsuario")),
                Status = Valor(row, "sStatus"),
                EstadoJson = Valor(row, "sEstadoJson"),
                ResumoAcao = Valor(row, "sResumoAcao"),
                TraceJson = Valor(row, "sTraceJson"),
                WorkflowNome = Valor(row, "sNomeInterno"),
                IdRecursoNecessario = ParaInt(Valor(row, "idRecursoNecessario")),
                GrafoJson = Valor(row, "sGrafoJson"),
                WorkflowAtivo = Valor(row, "sAtivo") == "S"
            };
        }

        private static string ResumoWorkflowHistorico(string jsonOriginal)
        {
            if (string.IsNullOrWhiteSpace(jsonOriginal))
            {
                return string.Empty;
            }

            try
            {
                JObject raiz = JObject.Parse(jsonOriginal);
                string status = TextoJson(raiz["status"]);
                JArray workflowsConcluidos = raiz["workflowsConcluidos"] as JArray ?? new JArray();
                bool execucaoDiretaConcluida = string.Equals(status, "CONCLUIDO", StringComparison.OrdinalIgnoreCase);
                if (!execucaoDiretaConcluida && workflowsConcluidos.Count == 0)
                {
                    return string.Empty;
                }

                JArray links = new JArray();
                JArray saidasRelevantes = new JArray();
                HashSet<string> linksVistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (execucaoDiretaConcluida)
                {
                    ColetarSaidasWorkflow(raiz["resumoConfirmado"], "resumoConfirmado", links, linksVistos, saidasRelevantes);
                    ColetarSaidasWorkflow(raiz["saidas"], "saidas", links, linksVistos, saidasRelevantes);
                }
                ColetarSaidasWorkflow(workflowsConcluidos, "workflowsConcluidos", links, linksVistos, saidasRelevantes);

                JObject resumo = new JObject
                {
                    { "status", "CONCLUIDO" },
                    { "mensagem", TextoJson(raiz["mensagem"]) },
                    { "observacao", "Workflow concluído. Use estes dados como fonte de verdade para perguntas de acompanhamento; não trate esta execução como pendente." },
                    { "links", links },
                    { "saidasRelevantes", saidasRelevantes }
                };

                return cls_IA_Sanitizacao.Resumir(resumo.ToString(Newtonsoft.Json.Formatting.None), 2500);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void ColetarSaidasWorkflow(JToken token, string caminho, JArray links, HashSet<string> linksVistos, JArray saidasRelevantes)
        {
            if (token == null)
            {
                return;
            }

            JObject obj = token as JObject;
            if (obj != null)
            {
                JObject relevante = SaidaWorkflowRelevante(obj, caminho);
                if (relevante.Count > 1)
                {
                    saidasRelevantes.Add(relevante);
                }

                string link = TextoJson(obj["link"]);
                if (!string.IsNullOrWhiteSpace(link) && linksVistos.Add(link))
                {
                    links.Add(link);
                }

                foreach (JProperty prop in obj.Properties())
                {
                    ColetarSaidasWorkflow(prop.Value, caminho + "." + prop.Name, links, linksVistos, saidasRelevantes);
                }
                return;
            }

            JArray arr = token as JArray;
            if (arr != null)
            {
                for (int i = 0; i < arr.Count; i++)
                {
                    ColetarSaidasWorkflow(arr[i], caminho + "[" + i + "]", links, linksVistos, saidasRelevantes);
                }
            }
        }

        private static JObject SaidaWorkflowRelevante(JObject obj, string caminho)
        {
            JObject relevante = new JObject { { "origem", caminho ?? string.Empty } };
            string[] campos = new[]
            {
                "idOrcamento",
                "idPedido",
                "nNumeroOrcamento",
                "idRegistroCRM",
                "idCRM",
                "idCliente",
                "idContatoCliente",
                "idEnderecoFiscal",
                "idEnderecoEntrega",
                "referencia",
                "clienteNome",
                "codigoProduto",
                "descricao",
                "tipoItem",
                "quantidade",
                "valorTotal",
                "link",
                "mensagem",
                "observacao"
            };

            foreach (string campo in campos)
            {
                JToken valor = obj[campo];
                if (valor != null && valor.Type != JTokenType.Null && valor.Type != JTokenType.Undefined)
                {
                    string texto = TextoJson(valor);
                    if (!string.IsNullOrWhiteSpace(texto) || valor.Type == JTokenType.Integer || valor.Type == JTokenType.Float || valor.Type == JTokenType.Boolean)
                    {
                        relevante[campo] = valor.DeepClone();
                    }
                }
            }

            return relevante;
        }

        private static string TextoJson(JToken token)
        {
            return token == null || token.Type == JTokenType.Null || token.Type == JTokenType.Undefined
                ? string.Empty
                : token.ToString().Trim();
        }

        public bool ExcluirFerramenta(int idFerramentaIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "EXCLUIR_FERRAMENTA" },
                { "@idFerramentaIA", idFerramentaIA.ToString() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            return tb.Rows.Count > 0 && ParaInt(Valor(tb.Rows[0], "nRet")) == 0;
        }

        public int SalvarArquivo(IAArquivoChat arquivo, int retencaoHoras)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_ARQUIVO" },
                { "@idConversaIA", arquivo != null ? arquivo.IdConversaIA.ToString() : "0" },
                { "@idUsuario", IdUsuarioAtual() },
                { "@sNomeOriginal", arquivo != null ? arquivo.NomeOriginal ?? string.Empty : string.Empty },
                { "@sExtensao", arquivo != null ? arquivo.Extensao ?? string.Empty : string.Empty },
                { "@sMimeType", arquivo != null ? arquivo.MimeType ?? string.Empty : string.Empty },
                { "@nTamanhoBytes", arquivo != null ? arquivo.TamanhoBytes.ToString() : "0" },
                { "@sHashSHA256", arquivo != null ? arquivo.HashSHA256 ?? string.Empty : string.Empty },
                { "@sStatusArquivo", arquivo != null ? arquivo.Status ?? string.Empty : string.Empty },
                { "@sArquivoTemporario", arquivo != null ? arquivo.ArquivoTemporario ?? string.Empty : string.Empty },
                { "@nRetencaoHoras", retencaoHoras.ToString() },
                { "@sTipoArquivo", arquivo != null && !string.IsNullOrWhiteSpace(arquivo.Tipo) ? arquivo.Tipo : "C" },
                { "@sCategoriaConhecimento", arquivo != null ? arquivo.Categoria ?? string.Empty : string.Empty },
                { "@sTituloConhecimento", arquivo != null ? arquivo.TituloConhecimento ?? string.Empty : string.Empty }
            };

            DataSet ds = BD.ExecutarDataSet(Procedure, parametros);
            return ParaInt(BD.Retorno.DATASET(ds, "idArquivoIA"));
        }

        // ===== Base de conhecimento (documentos sTipo='B' + FAQ) =====

        public DataTable ListarArquivosConhecimento(string categoria)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "LISTAR_ARQUIVOS_CONHECIMENTO" },
                { "@sCategoriaConhecimento", categoria ?? string.Empty }
            };
            return BD.ExecutarDataTable(Procedure, parametros, false);
        }

        // Estado do job tmrConhecimentoPasta no TT_Windows (tbl_Parametros_Timers, via sp_Select 'Parametros').
        // So para a tela IA - Conhecimento dizer de quanto em quanto tempo a pasta e lida — ou que o job esta
        // desligado, que e a causa mais provavel de "copiei o arquivo e nao entrou".
        // Devolve null quando o job nao esta cadastrado ou a consulta falha.
        public DataRow ConsultarJobConhecimentoPasta()
        {
            try
            {
                DataSet ds = BD.ExecutarDataSet("sp_Select", new Dictionary<string, string> { { "@sTabela", "Parametros" } });
                if (ds == null || ds.Tables.Count < 2 || !ds.Tables[1].Columns.Contains("sNomeTimer"))
                {
                    return null;
                }

                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    // sNomeTimer aceita "nome|parametros"; so o nome importa aqui
                    string nome = Convert.ToString(row["sNomeTimer"]).Split('|')[0].Trim();
                    if (string.Equals(nome, "tmrConhecimentoPasta", StringComparison.OrdinalIgnoreCase))
                    {
                        return row;
                    }
                }
            }
            catch
            {
                // Tela nao pode quebrar por causa de um aviso informativo
            }

            return null;
        }

        // FAQ da casa (tbl_Flow_Faq): puxa TODAS as aprovadas (idStatus=1) via sp_Manipula_tbl_Flow_FAQ
        // CONSULTAR com @sPesquisa vazio. O casamento com o termo e feito no C# (titulo+tags+corpo, sem
        // acento/case) porque o CONSULTAR do SP so busca em titulo+tag, nao no corpo da resposta. Reusa
        // o SP testado -> nao acessa a tabela direto, herda a semantica de "aprovado" da tela de FAQ.
        // Colunas retornadas: idFaq, sTitulo, sCorpo, idCategoria, idDepartamento, idStatus, sTag, ...
        public DataTable ListarFaqAprovada()
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sPesquisa", string.Empty },
                { "@dtCriado", string.Empty },
                { "@idCategoria", "0" },
                { "@idDepartamento", "0" },
                { "@idStatus", "1" },
                { "@sUsuarioAprova", string.Empty }
            };

            return BD.ExecutarDataTable("sp_Manipula_tbl_Flow_FAQ", parametros, false);
        }

        public void AtualizarArquivoProcessamento(IAArquivoChat arquivo)
        {
            if (arquivo == null || arquivo.IdArquivoIA <= 0)
            {
                return;
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "ATUALIZAR_ARQUIVO_PROCESSAMENTO" },
                { "@idArquivoIA", arquivo.IdArquivoIA.ToString() },
                { "@idUsuario", IdUsuarioAtual() },
                { "@sStatusArquivo", arquivo.Status ?? string.Empty },
                { "@sMarkdown", arquivo.Markdown ?? string.Empty },
                { "@sResumo", arquivo.Resumo ?? string.Empty },
                { "@sErro", arquivo.Erro ?? string.Empty },
                { "@sArquivoTemporario", arquivo.ArquivoTemporario ?? string.Empty },
                { "@nTotalCaracteres", arquivo.TotalCaracteres.ToString() },
                { "@nTotalTrechos", arquivo.TotalTrechos.ToString() }
            };

            BD.ExecutarDataTable(Procedure, parametros, false);
        }

        public void LimparTrechosArquivo(int idArquivoIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "LIMPAR_TRECHOS_ARQUIVO" },
                { "@idArquivoIA", idArquivoIA.ToString() },
                { "@idUsuario", IdUsuarioAtual() }
            };

            BD.ExecutarDataTable(Procedure, parametros, false);
        }

        public int SalvarTrechoArquivo(IAArquivoTrecho trecho)
        {
            if (trecho == null || trecho.IdArquivoIA <= 0)
            {
                return 0;
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_TRECHO_ARQUIVO" },
                { "@idArquivoIA", trecho.IdArquivoIA.ToString() },
                { "@idUsuario", IdUsuarioAtual() },
                { "@nOrdem", trecho.Ordem.ToString() },
                { "@sTituloTrecho", trecho.Titulo ?? string.Empty },
                { "@sConteudoTrecho", trecho.ConteudoMarkdown ?? string.Empty },
                { "@nInicioChar", trecho.InicioChar.ToString() },
                { "@nFimChar", trecho.FimChar.ToString() },
                { "@nTotalCaracteres", trecho.TotalCaracteres.ToString() }
            };

            DataSet ds = BD.ExecutarDataSet(Procedure, parametros);
            return ParaInt(BD.Retorno.DATASET(ds, "idArquivoTrechoIA"));
        }

        public IAArquivoChat ConsultarArquivoUsuario(int idArquivoIA, int idConversaIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_ARQUIVO_USUARIO" },
                { "@idArquivoIA", idArquivoIA.ToString() },
                { "@idConversaIA", idConversaIA.ToString() },
                { "@idUsuario", IdUsuarioAtual() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            return tb != null && tb.Rows.Count > 0 ? MapearArquivo(tb.Rows[0], true) : null;
        }

        public IAArquivoChat ConsultarArquivoAdmin(int idArquivoIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_ARQUIVO_ADMIN" },
                { "@idArquivoIA", idArquivoIA.ToString() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            return tb != null && tb.Rows.Count > 0 ? MapearArquivo(tb.Rows[0], true) : null;
        }

        public List<IAArquivoChat> ConsultarArquivosMensagem(int idMensagemIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_ARQUIVOS_MENSAGEM" },
                { "@idMensagemIA", idMensagemIA.ToString() },
                { "@idUsuario", IdUsuarioAtual() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            List<IAArquivoChat> arquivos = new List<IAArquivoChat>();

            foreach (DataRow row in tb.Rows)
            {
                arquivos.Add(MapearArquivo(row, false));
            }

            return arquivos;
        }

        public List<IAArquivoChat> ConsultarArquivosConversa(int idConversaIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_ARQUIVOS_CONVERSA" },
                { "@idConversaIA", idConversaIA.ToString() },
                { "@idUsuario", IdUsuarioAtual() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            List<IAArquivoChat> arquivos = new List<IAArquivoChat>();

            foreach (DataRow row in tb.Rows)
            {
                arquivos.Add(MapearArquivo(row, false));
            }

            return arquivos;
        }

        public DataTable ConsultarArquivosAdmin(int idArquivoIA, int idConversaIA, int idUsuarioFiltro, string status, string termo, string dtInicial, string dtFinal, string extensao)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_ARQUIVOS_ADMIN" },
                { "@idArquivoIA", idArquivoIA.ToString() },
                { "@idConversaIA", idConversaIA.ToString() },
                { "@idUsuarioFiltro", idUsuarioFiltro.ToString() },
                { "@sStatusArquivo", status ?? string.Empty },
                { "@sExtensaoFiltro", extensao ?? string.Empty },
                { "@sTermoArquivo", cls_IA_Sanitizacao.LimparEntradaUsuario(termo ?? string.Empty, 200) },
                { "@dtInicial", dtInicial ?? string.Empty },
                { "@dtFinal", dtFinal ?? string.Empty }
            };

            return BD.ExecutarDataTable(Procedure, parametros, false);
        }

        // Extensoes distintas dos arquivos (dropdown de filtro da tela de Arquivos).
        public DataTable ListarExtensoesArquivos()
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "LISTAR_EXTENSOES_ARQUIVOS" }
            };

            return BD.ExecutarDataTable(Procedure, parametros, false);
        }

        public DataTable ReenfileirarArquivoAdmin(int idArquivoIA, int retencaoHoras)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "REENFILEIRAR_ARQUIVO_ADMIN" },
                { "@idArquivoIA", idArquivoIA.ToString() },
                { "@nRetencaoHoras", retencaoHoras.ToString() }
            };

            return BD.ExecutarDataTable(Procedure, parametros, false);
        }

        public DataTable VincularArquivoMensagem(int idConversaIA, int idMensagemIA, int idArquivoIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "VINCULAR_ARQUIVO_MENSAGEM" },
                { "@idConversaIA", idConversaIA.ToString() },
                { "@idMensagemIA", idMensagemIA.ToString() },
                { "@idArquivoIA", idArquivoIA.ToString() },
                { "@idUsuario", IdUsuarioAtual() }
            };

            return BD.ExecutarDataTable(Procedure, parametros, false);
        }

        public DataTable RemoverArquivo(int idArquivoIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "REMOVER_ARQUIVO" },
                { "@idArquivoIA", idArquivoIA.ToString() },
                { "@idUsuario", IdUsuarioAtual() }
            };

            return BD.ExecutarDataTable(Procedure, parametros, false);
        }

        // Base de conhecimento (sTipo='B'): remove sem filtro por dono (a permissao 694 ja governa).
        public DataTable RemoverArquivoConhecimento(int idArquivoIA)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "REMOVER_ARQUIVO_CONHECIMENTO" },
                { "@idArquivoIA", idArquivoIA.ToString() }
            };

            return BD.ExecutarDataTable(Procedure, parametros, false);
        }

        public List<IAArquivoTrecho> ConsultarTrechosArquivos(int idConversaIA, List<int> idsArquivos, string termo, int limite)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_TRECHOS_ARQUIVOS" },
                { "@idConversaIA", idConversaIA.ToString() },
                { "@idUsuario", IdUsuarioAtual() },
                { "@sIdsArquivos", string.Join(",", (idsArquivos ?? new List<int>()).ToArray()) },
                { "@sTermoArquivo", cls_IA_Sanitizacao.LimparEntradaUsuario(termo ?? string.Empty, 200) },
                { "@nLimite", limite.ToString() }
            };

            DataTable tb = BD.ExecutarDataTable(Procedure, parametros, false);
            List<IAArquivoTrecho> trechos = new List<IAArquivoTrecho>();

            foreach (DataRow row in tb.Rows)
            {
                trechos.Add(new IAArquivoTrecho
                {
                    IdArquivoIA = ParaInt(Valor(row, "idArquivoIA")),
                    IdArquivoTrechoIA = ParaInt(Valor(row, "idArquivoTrechoIA")),
                    Ordem = ParaInt(Valor(row, "nOrdem")),
                    Titulo = Valor(row, "sTitulo"),
                    ConteudoMarkdown = Valor(row, "sConteudoMarkdown"),
                    InicioChar = ParaInt(Valor(row, "nInicioChar")),
                    FimChar = ParaInt(Valor(row, "nFimChar")),
                    TotalCaracteres = ParaInt(Valor(row, "nTotalCaracteres")),
                    NomeOriginal = Valor(row, "sNomeOriginal"),
                    HashSHA256 = Valor(row, "sHashSHA256")
                });
            }

            return trechos;
        }

        // Busca na base de conhecimento (global, compartilhada): arquivos sTipo='B' + FAQ, via Full-Text/LIKE.
        public DataTable ConsultarConhecimento(string termo, string categoria, int limite)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_CONHECIMENTO" },
                { "@sTermoArquivo", cls_IA_Sanitizacao.LimparEntradaUsuario(termo ?? string.Empty, 200) },
                { "@sCategoriaConhecimento", cls_IA_Sanitizacao.LimparEntradaUsuario(categoria ?? string.Empty, 100) },
                { "@nLimite", limite.ToString() }
            };

            return BD.ExecutarDataTable(Procedure, parametros, false);
        }

        private static string NormalizarConteudoMensagem(string papel, string conteudo, List<IAFonteResposta> fontes)
        {
            if (!string.IsNullOrWhiteSpace(conteudo))
            {
                return conteudo;
            }

            if ((papel ?? string.Empty).Trim().Equals("ASSISTANT", StringComparison.OrdinalIgnoreCase))
            {
                if (fontes != null && fontes.Count > 0)
                {
                    return "Esta resposta foi registrada sem conteudo textual. As fontes consultadas continuam disponiveis abaixo; reenvie a pergunta para gerar a resposta novamente.";
                }

                return "Esta resposta foi registrada sem conteudo textual. Reenvie a pergunta para gerar a resposta novamente.";
            }

            return conteudo ?? string.Empty;
        }

        private static int ParaInt(string valor)
        {
            int retorno;
            return int.TryParse((valor ?? string.Empty).Trim(), out retorno) ? retorno : 0;
        }

        private string IdUsuarioAtual()
        {
            return string.IsNullOrWhiteSpace(_idUsuarioOverride)
                ? TT.FrameWork.Identity.Variaveis.idUsuario()
                : _idUsuarioOverride;
        }

        private static long ParaLong(string valor)
        {
            long retorno;
            return long.TryParse((valor ?? string.Empty).Trim(), out retorno) ? retorno : 0;
        }

        private static IAArquivoChat MapearArquivo(DataRow row, bool incluirMarkdown)
        {
            IAArquivoChat arquivo = new IAArquivoChat
            {
                IdArquivoIA = ParaInt(Valor(row, "idArquivoIA")),
                IdConversaIA = ParaInt(Valor(row, "idConversaIA")),
                IdMensagemIA = ParaInt(Valor(row, "idMensagemIA")),
                IdUsuario = ParaInt(Valor(row, "idUsuario")),
                NomeOriginal = Valor(row, "sNomeOriginal"),
                Extensao = Valor(row, "sExtensao"),
                MimeType = Valor(row, "sMimeType"),
                TamanhoBytes = ParaLong(Valor(row, "nTamanhoBytes")),
                HashSHA256 = Valor(row, "sHashSHA256"),
                Status = Valor(row, "sStatus"),
                Resumo = Valor(row, "sResumo"),
                Erro = Valor(row, "sErro"),
                ArquivoTemporario = Valor(row, "sArquivoTemporario"),
                TotalCaracteres = ParaInt(Valor(row, "nTotalCaracteres")),
                TotalTrechos = ParaInt(Valor(row, "nTotalTrechos")),
                DtUpload = Valor(row, "dtUpload"),
                DtProcessamento = Valor(row, "dtProcessamento"),
                DtExpiracao = Valor(row, "dtExpiracao")
            };

            arquivo.TamanhoFormatado = FormatarBytes(arquivo.TamanhoBytes);

            // sTipo/categoria/titulo so vem em SPs da base de conhecimento; nos demais o Valor devolve
            // vazio e mantemos o default do construtor (Tipo 'C' = anexo de conversa).
            string tipoArquivo = Valor(row, "sTipo");
            if (!string.IsNullOrEmpty(tipoArquivo))
            {
                arquivo.Tipo = tipoArquivo;
            }
            arquivo.Categoria = Valor(row, "sCategoria");
            arquivo.TituloConhecimento = Valor(row, "sTituloConhecimento");

            if (incluirMarkdown)
            {
                arquivo.Markdown = Valor(row, "sMarkdown");
            }

            return arquivo;
        }

        private static string FormatarBytes(long bytes)
        {
            if (bytes <= 0)
            {
                return "0 KB";
            }

            if (bytes < 1024 * 1024)
            {
                return Math.Max(1, bytes / 1024) + " KB";
            }

            decimal mb = (decimal)bytes / (1024 * 1024);
            return mb.ToString("0.##") + " MB";
        }

        private static List<IAFonteResposta> ExtrairFontesMensagem(string jsonOriginal)
        {
            List<IAFonteResposta> fontes = new List<IAFonteResposta>();

            if (string.IsNullOrWhiteSpace(jsonOriginal))
            {
                return fontes;
            }

            try
            {
                JObject obj = JObject.Parse(jsonOriginal);
                JArray array = obj["fontes"] as JArray;
                if (array == null)
                {
                    return fontes;
                }

                foreach (JToken item in array)
                {
                    fontes.Add(new IAFonteResposta
                    {
                        Ferramenta = ValorFonte(item, "ferramenta"),
                        Tipo = ValorFonte(item, "tipo"),
                        Titulo = ValorFonte(item, "titulo"),
                        Url = ValorFonte(item, "url"),
                        Resumo = ValorFonte(item, "resumo"),
                        IdArquivoIA = ParaInt(ValorFonte(item, "idArquivoIA")),
                        IdArquivoTrechoIA = ParaInt(ValorFonte(item, "idArquivoTrechoIA")),
                        HashSHA256 = ValorFonte(item, "hashSHA256"),
                        Trecho = ValorFonte(item, "trecho"),
                        InicioChar = ParaInt(ValorFonte(item, "inicioChar")),
                        FimChar = ParaInt(ValorFonte(item, "fimChar"))
                    });
                }
            }
            catch
            {
                return new List<IAFonteResposta>();
            }

            return fontes;
        }

        private static string ValorFonte(JToken item, string campo)
        {
            if (item == null || item[campo] == null)
            {
                return string.Empty;
            }

            return item[campo].ToString().Trim();
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
