using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.FrameWork.IA;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.IA
{
    public partial class Configuracao : Page
    {
        private readonly cls_IA_Repositorio _repositorio = new cls_IA_Repositorio();

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            CarregarOpcoesModelos();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.IA.AdministrarFerramentas, true);

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = "Configuracao IA";
                CarregarConfig();
                CarregarFerramentas();
            }

            // Registra via ScriptManager, entao reaplica sozinho apos os postbacks do UpdatePanel.
            FUNCOES.Scripts.Aplica_TooltipPersonalizado(Page, "tooltip");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            string mensagemErro;
            if (!Validar(out mensagemErro))
            {
                MensagemPagina.MostraMensagem_Erro(mensagemErro);
                return;
            }

            cls_IA_Config configAtual = cls_IA_Config.Carregar();
            string openAIKey = ValorNovoOuAtual(txtOpenAIKey.Text, configAtual.OpenAIApiKey);
            string geminiKey = ValorNovoOuAtual(txtGeminiKey.Text, configAtual.GeminiApiKey);
            string claudeKey = ValorNovoOuAtual(txtClaudeKey.Text, configAtual.ClaudeApiKey);
            string groqKey = ValorNovoOuAtual(txtGroqKey.Text, configAtual.GroqApiKey);
            string compatKey = ValorNovoOuAtual(txtCompatKey.Text, configAtual.CompatApiKey);
            string openAIModelo = ModeloSelecionado(ddlOpenAIModelo);
            string geminiModelo = ModeloSelecionado(ddlGeminiModelo);
            string claudeModelo = ModeloSelecionado(ddlClaudeModelo);
            string groqModelo = ModeloSelecionado(ddlGroqModelo);
            string compatModelo = ModeloSelecionado(ddlCompatModelo);

            if (ddlHabilitado.SelectedValue == "S" && !ValidarProviderAtivo(openAIKey, geminiKey, claudeKey, groqKey, compatKey, out mensagemErro))
            {
                MensagemPagina.MostraMensagem_Erro(mensagemErro);
                return;
            }

            _repositorio.SalvarConfig("IA.Habilitado", ddlHabilitado.SelectedValue);
            _repositorio.SalvarConfig("IA.Provider", ddlProvider.SelectedValue);
            _repositorio.SalvarConfig("IA.Provider.Fallback", ddlProviderFallback.SelectedValue);
            _repositorio.SalvarConfig("IA.ModeloPadrao", ModeloAtivoSelecionado());
            _repositorio.SalvarConfig("IA.OpenAI.Modelo", openAIModelo);
            _repositorio.SalvarConfig("IA.Gemini.Modelo", geminiModelo);
            _repositorio.SalvarConfig("IA.Claude.Modelo", claudeModelo);
            _repositorio.SalvarConfig("IA.Groq.Modelo", groqModelo);
            _repositorio.SalvarConfig("IA.Compat.Modelo", compatModelo);
            _repositorio.SalvarConfig("IA.Compat.BaseUrl", txtCompatBaseUrl.Text.Trim());
            _repositorio.SalvarConfig("IA.RaciocinioNivel", ddlRaciocinioNivel.SelectedValue);
            _repositorio.SalvarConfig("IA.VerbosidadeTexto", ddlVerbosidadeTexto.SelectedValue);
            _repositorio.SalvarConfig("IA.ThinkingBudgetTokens", txtThinkingBudgetTokens.Text.Trim());
            _repositorio.SalvarConfig("IA.StoreExterno", ddlStoreExterno.SelectedValue);
            _repositorio.SalvarConfig("IA.MaxTokensEntrada", txtMaxTokensEntrada.Text.Trim());
            _repositorio.SalvarConfig("IA.MaxTokensSaida", txtMaxTokensSaida.Text.Trim());
            _repositorio.SalvarConfig("IA.Historico.MaxMensagens", txtHistoricoMaxMensagens.Text.Trim());
            _repositorio.SalvarConfig("IA.Historico.MaxCharsPorMensagem", txtHistoricoMaxCharsPorMensagem.Text.Trim());
            _repositorio.SalvarConfig("IA.Ferramentas.MaxRodadasPorMensagem", txtFerramentasMaxRodadasPorMensagem.Text.Trim());
            _repositorio.SalvarConfig("IA.RateLimitPorUsuarioDia", txtRateLimit.Text.Trim());
            _repositorio.SalvarConfig("IA.RateLimitPorIpDia", txtRateLimitIp.Text.Trim());
            _repositorio.SalvarConfig("IA.RetencaoDias", txtRetencaoDias.Text.Trim());
            _repositorio.SalvarConfig("IA.Arquivos.Habilitado", ddlArquivosHabilitado.SelectedValue);
            _repositorio.SalvarConfig("IA.Arquivos.MaxArquivosPorMensagem", txtArquivosMaxArquivos.Text.Trim());
            _repositorio.SalvarConfig("IA.Arquivos.MaxMBArquivo", txtArquivosMaxMB.Text.Trim());
            _repositorio.SalvarConfig("IA.Arquivos.SyncMaxMBArquivo", txtArquivosSyncMaxMB.Text.Trim());
            _repositorio.SalvarConfig("IA.Arquivos.MaxCharsTrecho", txtArquivosMaxCharsTrecho.Text.Trim());
            _repositorio.SalvarConfig("IA.Arquivos.MaxTrechosPorResposta", txtArquivosMaxTrechos.Text.Trim());
            _repositorio.SalvarConfig("IA.Arquivos.RetencaoOriginalHoras", txtArquivosRetencaoHoras.Text.Trim());
            _repositorio.SalvarConfig("IA.Conhecimento.SempreDisponivel", ddlConhecimentoSempreDisponivel.SelectedValue);
            _repositorio.SalvarConfig("IA.Conhecimento.PastaEntrada", txtConhecimentoPastaEntrada.Text.Trim());
            _repositorio.SalvarConfig("IA.Conhecimento.PastaMaxPorVarredura", txtConhecimentoPastaMax.Text.Trim());
            _repositorio.SalvarConfig("IA.Conhecimento.PastaIdUsuario", ddlConhecimentoPastaIdUsuario.SelectedValue);

            SalvarConfigPublico();

            bool openAIKeyAlterada = SalvarChaveSeInformada("IA.OpenAI.ApiKeyCriptografada", txtOpenAIKey);
            bool geminiKeyAlterada = SalvarChaveSeInformada("IA.Gemini.ApiKeyCriptografada", txtGeminiKey);
            bool claudeKeyAlterada = SalvarChaveSeInformada("IA.Claude.ApiKeyCriptografada", txtClaudeKey);
            bool groqKeyAlterada = SalvarChaveSeInformada("IA.Groq.ApiKeyCriptografada", txtGroqKey);
            bool compatKeyAlterada = SalvarChaveSeInformada("IA.Compat.ApiKeyCriptografada", txtCompatKey);

            cls_IA_Auditoria.Registrar(0, "CONFIG_UPDATED", "ALERTA", new
            {
                habilitado = ddlHabilitado.SelectedValue,
                provider = ddlProvider.SelectedValue,
                providerFallback = ddlProviderFallback.SelectedValue,
                modeloAtivo = ModeloAtivoSelecionado(),
                openAIModelo = openAIModelo,
                geminiModelo = geminiModelo,
                claudeModelo = claudeModelo,
                groqModelo = groqModelo,
                compatModelo = compatModelo,
                compatBaseUrl = txtCompatBaseUrl.Text.Trim(),
                raciocinioNivel = ddlRaciocinioNivel.SelectedValue,
                verbosidadeTexto = ddlVerbosidadeTexto.SelectedValue,
                thinkingBudgetTokens = txtThinkingBudgetTokens.Text.Trim(),
                storeExterno = ddlStoreExterno.SelectedValue,
                maxTokensEntrada = txtMaxTokensEntrada.Text.Trim(),
                maxTokensSaida = txtMaxTokensSaida.Text.Trim(),
                historicoMaxMensagens = txtHistoricoMaxMensagens.Text.Trim(),
                historicoMaxCharsPorMensagem = txtHistoricoMaxCharsPorMensagem.Text.Trim(),
                ferramentasMaxRodadasPorMensagem = txtFerramentasMaxRodadasPorMensagem.Text.Trim(),
                rateLimitPorUsuarioDia = txtRateLimit.Text.Trim(),
                rateLimitPorIpDia = txtRateLimitIp.Text.Trim(),
                retencaoDias = txtRetencaoDias.Text.Trim(),
                arquivosHabilitado = ddlArquivosHabilitado.SelectedValue,
                arquivosMaxArquivos = txtArquivosMaxArquivos.Text.Trim(),
                arquivosMaxMB = txtArquivosMaxMB.Text.Trim(),
                arquivosSyncMaxMB = txtArquivosSyncMaxMB.Text.Trim(),
                arquivosMaxCharsTrecho = txtArquivosMaxCharsTrecho.Text.Trim(),
                arquivosMaxTrechos = txtArquivosMaxTrechos.Text.Trim(),
                arquivosRetencaoHoras = txtArquivosRetencaoHoras.Text.Trim(),
                conhecimentoSempreDisponivel = ddlConhecimentoSempreDisponivel.SelectedValue,
                conhecimentoPastaEntrada = txtConhecimentoPastaEntrada.Text.Trim(),
                conhecimentoPastaMax = txtConhecimentoPastaMax.Text.Trim(),
                conhecimentoPastaIdUsuario = ddlConhecimentoPastaIdUsuario.SelectedValue,
                openAIKeyAlterada = openAIKeyAlterada,
                geminiKeyAlterada = geminiKeyAlterada,
                claudeKeyAlterada = claudeKeyAlterada,
                groqKeyAlterada = groqKeyAlterada,
                compatKeyAlterada = compatKeyAlterada
            });

            MensagemPagina.MostraMensagem_Sucesso("Configuracao salva com sucesso.");
            CarregarConfig();
            CarregarFerramentas();
        }

        protected void cmdRecarregar_Click(object sender, EventArgs e)
        {
            CarregarConfig();
            CarregarFerramentas();
        }

        protected void cmdExecutarLimpeza_Click(object sender, EventArgs e)
        {
            cls_IA_Config config = cls_IA_Config.Carregar();
            if (!config.SqlDisponivel)
            {
                MensagemPagina.MostraMensagem_Erro(config.ErroConfiguracao);
                return;
            }

            IAManutencaoResultado resultado = cls_IA_Manutencao.ExecutarAgora(config, _repositorio);

            cls_IA_Auditoria.Registrar(0, "MANUTENCAO_MANUAL", "ALERTA", new
            {
                retencaoDias = config.RetencaoDias,
                auditoriaRemovidas = resultado.AuditoriaRemovidas,
                arquivosTemporariosRemovidos = resultado.ArquivosTemporariosRemovidos
            });

            MensagemPagina.MostraMensagem_Sucesso(
                "Limpeza concluida. Registros de auditoria removidos: " + resultado.AuditoriaRemovidas +
                ". Arquivos temporarios removidos: " + resultado.ArquivosTemporariosRemovidos + ".");

            CarregarConfig();
            CarregarFerramentas();
        }

        private void CarregarConfig()
        {
            cls_IA_Config config = cls_IA_Config.Carregar();

            // Popula antes de selecionar, senao o SelectedValue nao acha o item. Popula_Combo ja limpa a
            // lista, entao rodar de novo depois de salvar nao duplica. Mesmo padrao das telas IA - Arquivos
            // e IA - Auditoria.
            FUNCOES.Popula_Combo(ddlConhecimentoPastaIdUsuario, "sp_Select 'Usuarios'", "idUsuario", "sDscUsuario", false, "Não definido", "0");

            Selecionar(ddlHabilitado, config.Habilitado ? "S" : "N");
            Selecionar(ddlProvider, string.IsNullOrWhiteSpace(config.Provider) ? cls_IA_Config.ProviderOpenAI : config.Provider);
            Selecionar(ddlProviderFallback, config.ProviderFallback ?? string.Empty);
            SelecionarModelo(ddlOpenAIModelo, config.OpenAIModelo);
            SelecionarModelo(ddlGeminiModelo, config.GeminiModelo);
            SelecionarModelo(ddlClaudeModelo, config.ClaudeModelo);
            SelecionarModelo(ddlGroqModelo, config.GroqModelo);
            SelecionarModelo(ddlCompatModelo, config.CompatModelo);
            txtCompatBaseUrl.Text = config.CompatBaseUrl;
            Selecionar(ddlRaciocinioNivel, config.RaciocinioNivel);
            Selecionar(ddlVerbosidadeTexto, config.VerbosidadeTexto);
            txtThinkingBudgetTokens.Text = config.ThinkingBudgetTokens.ToString();
            Selecionar(ddlStoreExterno, config.StoreExterno ? "S" : "N");
            txtMaxTokensEntrada.Text = config.MaxTokensEntrada.ToString();
            txtMaxTokensSaida.Text = config.MaxTokensSaida.ToString();
            txtHistoricoMaxMensagens.Text = config.HistoricoMaxMensagens.ToString();
            txtHistoricoMaxCharsPorMensagem.Text = config.HistoricoMaxCharsPorMensagem.ToString();
            txtFerramentasMaxRodadasPorMensagem.Text = config.FerramentasMaxRodadasPorMensagem.ToString();
            txtRateLimit.Text = config.RateLimitPorUsuarioDia.ToString();
            txtRateLimitIp.Text = config.RateLimitPorIpDia.ToString();
            txtRetencaoDias.Text = config.RetencaoDias.ToString();
            Selecionar(ddlArquivosHabilitado, config.ArquivosHabilitado ? "S" : "N");
            txtArquivosMaxArquivos.Text = config.ArquivosMaxArquivosPorMensagem.ToString();
            txtArquivosMaxMB.Text = config.ArquivosMaxMBArquivo.ToString();
            txtArquivosSyncMaxMB.Text = config.ArquivosSyncMaxMBArquivo.ToString();
            txtArquivosMaxCharsTrecho.Text = config.ArquivosMaxCharsTrecho.ToString();
            txtArquivosMaxTrechos.Text = config.ArquivosMaxTrechosPorResposta.ToString();
            txtArquivosRetencaoHoras.Text = config.ArquivosRetencaoOriginalHoras.ToString();
            Selecionar(ddlConhecimentoSempreDisponivel, config.ConhecimentoSempreDisponivel ? "S" : "N");
            txtConhecimentoPastaEntrada.Text = config.ConhecimentoPastaEntrada ?? string.Empty;
            txtConhecimentoPastaMax.Text = config.ConhecimentoPastaMaxPorVarredura.ToString();
            Selecionar(ddlConhecimentoPastaIdUsuario, string.IsNullOrWhiteSpace(config.ConhecimentoPastaIdUsuario) ? "0" : config.ConhecimentoPastaIdUsuario);

            CarregarConfigPublico();

            LimparCamposChave();
            AtualizarStatusChave(lblOpenAIKeyStatus, !string.IsNullOrWhiteSpace(config.OpenAIApiKey));
            AtualizarStatusChave(lblGeminiKeyStatus, !string.IsNullOrWhiteSpace(config.GeminiApiKey));
            AtualizarStatusChave(lblClaudeKeyStatus, !string.IsNullOrWhiteSpace(config.ClaudeApiKey));
            AtualizarStatusChave(lblGroqKeyStatus, !string.IsNullOrWhiteSpace(config.GroqApiKey));
            AtualizarStatusChave(lblCompatKeyStatus, !string.IsNullOrWhiteSpace(config.CompatApiKey));

            if (config.SqlDisponivel)
            {
                IAUsoUsuario uso = _repositorio.ConsultarUsoUsuarioHoje();
                lblMensagensHoje.Text = uso.TotalMensagensHoje + " / " + config.RateLimitPorUsuarioDia;
                lblConversasHoje.Text = uso.TotalConversasHoje.ToString();
                lblTokensSaidaHoje.Text = uso.TokensSaidaHoje.ToString();
                CarregarUsoPeriodo(config);
            }
            else
            {
                lblMensagensHoje.Text = "-";
                lblConversasHoje.Text = "-";
                lblTokensSaidaHoje.Text = "-";
                LimparUsoPeriodo();
                MensagemPagina.MostraMensagem_Erro(config.ErroConfiguracao);
            }
        }

        private void CarregarUsoPeriodo(cls_IA_Config config)
        {
            try
            {
                DataSet ds = _repositorio.ConsultarUsoPeriodo();

                if (ds == null || ds.Tables.Count < 3)
                {
                    LimparUsoPeriodo();
                    return;
                }

                dtgvUsoDia.DataSource = ds.Tables[0];
                dtgvUsoDia.DataBind();
                dtgvUsoUsuarios.DataSource = ds.Tables[1];
                dtgvUsoUsuarios.DataBind();

                long conversas = 0;
                long tokensEntrada = 0;
                long tokensSaida = 0;

                if (ds.Tables[2].Rows.Count > 0)
                {
                    DataRow totais = ds.Tables[2].Rows[0];
                    conversas = ValorLong(totais, "nTotalConversas");
                    tokensEntrada = ValorLong(totais, "nTotalTokensEntrada");
                    tokensSaida = ValorLong(totais, "nTotalTokensSaida");
                }

                lblUsoConversas30d.Text = conversas.ToString("N0");
                lblUsoTokensEntrada30d.Text = tokensEntrada.ToString("N0");
                lblUsoTokensSaida30d.Text = tokensSaida.ToString("N0");

                if (cls_IA_Custo.AlgumPrecoConfigurado(config))
                {
                    decimal custo = 0;
                    System.Collections.Generic.Dictionary<string, IAPrecoModelo> precos = cls_IA_Custo.ObterPrecosModelos(config);

                    if (ds.Tables.Count > 3)
                    {
                        // Soma modelo a modelo pelo preco de cada um (mesma conta da tela IA - Uso)
                        foreach (DataRow rowModelo in ds.Tables[3].Rows)
                        {
                            string modelo = rowModelo.Table.Columns.Contains("sModelo") && rowModelo["sModelo"] != DBNull.Value ? rowModelo["sModelo"].ToString() : string.Empty;
                            IAPrecoModelo preco = cls_IA_Custo.ObterPrecoModelo(config, precos, modelo);
                            custo += cls_IA_Custo.Calcular(ValorLong(rowModelo, "nTokensEntrada"), ValorLong(rowModelo, "nTokensSaida"), preco);
                        }
                    }
                    else
                    {
                        custo = cls_IA_Custo.Calcular(tokensEntrada, tokensSaida, cls_IA_Custo.ObterPrecoModelo(config, precos, string.Empty));
                    }

                    lblUsoCusto30d.Text = cls_IA_Custo.FormatarUSD(custo);
                }
                else
                {
                    lblUsoCusto30d.Text = "defina os preços na tela IA - Uso";
                }
            }
            catch
            {
                LimparUsoPeriodo();
            }
        }

        private void LimparUsoPeriodo()
        {
            lblUsoConversas30d.Text = "-";
            lblUsoTokensEntrada30d.Text = "-";
            lblUsoTokensSaida30d.Text = "-";
            lblUsoCusto30d.Text = "-";
        }

        private static long ValorLong(DataRow row, string coluna)
        {
            if (row == null || !row.Table.Columns.Contains(coluna) || row[coluna] == DBNull.Value)
            {
                return 0;
            }

            long retorno;
            return long.TryParse(row[coluna].ToString(), out retorno) ? retorno : 0;
        }

        private void CarregarFerramentas()
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("IdFerramentaIA");
            tb.Columns.Add("Nome");
            tb.Columns.Add("Modulo");
            tb.Columns.Add("IdRecursoNecessario");
            tb.Columns.Add("MaxRegistros");
            tb.Columns.Add("Tipo");
            tb.Columns.Add("Ativa");
            tb.Columns.Add("PermitidaUsuarioAtual");
            tb.Columns.Add("Descricao");

            foreach (IAFerramentaDefinicao ferramenta in cls_IA_ToolRegistry.ListarTodasIncluindoInativas())
            {
                DataRow row = tb.NewRow();
                row["IdFerramentaIA"] = ferramenta.IdFerramentaIA.ToString();
                row["Nome"] = ferramenta.Nome;
                row["Modulo"] = ferramenta.Modulo;
                row["IdRecursoNecessario"] = ferramenta.IdRecursoNecessario.ToString();
                row["MaxRegistros"] = ferramenta.MaxRegistros.ToString();
                row["Tipo"] = cls_IA_ToolRegistry.EhInterna(ferramenta.Nome) ? "Interna" : "Personalizada";
                row["Ativa"] = ferramenta.Ativo ? "Sim" : "Nao";
                row["PermitidaUsuarioAtual"] = FUNCOES.ValidaPermissao(ferramenta.IdRecursoNecessario, false) ? "Sim" : "Nao";
                row["Descricao"] = ferramenta.Descricao;
                tb.Rows.Add(row);
            }

            dtgvFerramentas.DataSource = tb;
            dtgvFerramentas.DataBind();
        }

        // O par input+label vira o switch verde do master.css (.grid-toggle-check:checked + .grid-toggle-label).
        // Sem CssClass no proprio CheckBox: senao o WebForms embrulha tudo num <span> e quebra o seletor de irmao.
        protected void dtgvFerramentas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            CheckBox chk = e.Row.FindControl("chkAtiva") as CheckBox;
            if (chk == null)
            {
                return;
            }

            chk.InputAttributes["class"] = "grid-toggle-check";
            chk.LabelAttributes["class"] = "grid-toggle-label";
            chk.Text = chk.Checked ? "Ativa" : "Inativa";
        }

        protected void chkAtiva_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (chk == null)
            {
                return;
            }

            GridViewRow linha = chk.NamingContainer as GridViewRow;
            if (linha == null)
            {
                return;
            }

            // O proprio switch ja diz o estado desejado - nao precisa ler o atual e inverter.
            string nome = Convert.ToString(dtgvFerramentas.DataKeys[linha.RowIndex].Value);
            AlternarFerramenta(nome, chk.Checked);
        }

        private void AlternarFerramenta(string nome, bool novoEstado)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return;
            }

            if (!_repositorio.SetFerramentaAtiva(nome, novoEstado))
            {
                MensagemPagina.MostraMensagem_Erro("Nao foi possivel alterar a ferramenta.");
                CarregarFerramentas(); // devolve o switch ao estado real do banco
                return;
            }

            cls_IA_ToolRegistry.InvalidarCache();
            cls_IA_Auditoria.Registrar(0, "FERRAMENTA_ATIVACAO", "ALERTA", new { ferramenta = nome, ativa = novoEstado });
            MensagemPagina.MostraMensagem_Sucesso("Ferramenta '" + nome + "' " + (novoEstado ? "ativada" : "desativada") + ".");
            CarregarFerramentas();
        }

        private bool Validar(out string mensagemErro)
        {
            mensagemErro = string.Empty;

            /*
                Segredo mestre: exigido apenas quando ALGUMA chave de API foi digitada nesta
                gravacao. Quem so muda modelo ou limite nao precisa dele.

                A checagem vive aqui, e nao no meio do salvamento, porque cmdSalvar_Click grava
                dezenas de configs ANTES de chegar nas chaves — se a criptografia estourasse la,
                o resultado seria meia configuracao salva mais uma tela de erro do ASP.NET.
                Ver Framework/IA/cls_IA_Criptografia.cs.
            */
            if (!ValidarConfigPublico(out mensagemErro))
            {
                return false;
            }

            if (AlgumaChaveInformada() && !cls_IA_Criptografia.SegredoConfigurado())
            {
                mensagemErro = "Chave de API nao pode ser gravada: o segredo mestre da IA nao esta " +
                               "configurado neste servidor. Defina appSettings[\"IA.ChaveMestra\"] no " +
                               "Web.config ou a variavel de ambiente TT_IA_CHAVE_MESTRA (mesmo valor " +
                               "usado no servidor do assistente publico) e tente de novo. As demais " +
                               "configuracoes desta tela podem ser salvas normalmente.";
                return false;
            }

            /*
                Formato da chave. O campo e cego (nao mostra o que ja esta gravado), entao um
                erro de area de transferencia passava direto e so aparecia depois, como
                "Unauthorized - invalid x-api-key" na primeira conversa — longe da causa.
                Ja aconteceu de gravarem um CAMINHO DE PASTA aqui.
            */
            string motivoChave;
            if (!ChaveApiPlausivel(txtOpenAIKey, "OpenAI", out motivoChave)
                || !ChaveApiPlausivel(txtGeminiKey, "Gemini", out motivoChave)
                || !ChaveApiPlausivel(txtClaudeKey, "Claude", out motivoChave)
                || !ChaveApiPlausivel(txtGroqKey, "Groq", out motivoChave)
                || !ChaveApiPlausivel(txtCompatKey, "compativel", out motivoChave))
            {
                mensagemErro = motivoChave;
                return false;
            }

            string openAIModelo = ModeloSelecionado(ddlOpenAIModelo);
            string geminiModelo = ModeloSelecionado(ddlGeminiModelo);
            string claudeModelo = ModeloSelecionado(ddlClaudeModelo);
            string groqModelo = ModeloSelecionado(ddlGroqModelo);
            string compatModelo = ModeloSelecionado(ddlCompatModelo);

            if (!TextoValido(openAIModelo, 1, 100))
            {
                mensagemErro = "Informe um modelo OpenAI valido.";
                return false;
            }

            if (!TextoValido(geminiModelo, 1, 100))
            {
                mensagemErro = "Informe um modelo Gemini valido.";
                return false;
            }

            if (!TextoValido(claudeModelo, 1, 100))
            {
                mensagemErro = "Informe um modelo Claude valido.";
                return false;
            }

            if (!TextoValido(groqModelo, 1, 100))
            {
                mensagemErro = "Informe um modelo Groq valido.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(compatModelo) && !TextoValido(compatModelo, 1, 100))
            {
                mensagemErro = "Informe um modelo compativel valido.";
                return false;
            }

            if (ddlProvider.SelectedValue == cls_IA_Config.ProviderOpenAICompativel)
            {
                if (!TextoValido(compatModelo, 1, 100))
                {
                    mensagemErro = "Informe o modelo do provider OpenAI compativel.";
                    return false;
                }

                string erroBaseUrl;
                if (!cls_IA_OpenAICompatProvider.BaseUrlPermitida(txtCompatBaseUrl.Text, out erroBaseUrl))
                {
                    mensagemErro = erroBaseUrl;
                    return false;
                }
            }
            else if (!string.IsNullOrWhiteSpace(txtCompatBaseUrl.Text))
            {
                string erroBaseUrlInativa;
                if (!cls_IA_OpenAICompatProvider.BaseUrlPermitida(txtCompatBaseUrl.Text, out erroBaseUrlInativa))
                {
                    mensagemErro = erroBaseUrlInativa;
                    return false;
                }
            }

            if (!ValidarInteiro(txtThinkingBudgetTokens.Text, 0, 100000))
            {
                mensagemErro = "Thinking budget deve ficar entre 0 e 100000.";
                return false;
            }

            if (!ValidarInteiro(txtMaxTokensEntrada.Text, 1000, 50000))
            {
                mensagemErro = "Max entrada deve ficar entre 1000 e 50000.";
                return false;
            }

            if (!ValidarInteiro(txtMaxTokensSaida.Text, 100, 100000))
            {
                mensagemErro = "Max saida deve ficar entre 100 e 100000.";
                return false;
            }

            if (!ValidarInteiro(txtHistoricoMaxMensagens.Text, 0, 60))
            {
                mensagemErro = "Mensagens anteriores devem ficar entre 0 e 60.";
                return false;
            }

            if (!ValidarInteiro(txtHistoricoMaxCharsPorMensagem.Text, 200, 8000))
            {
                mensagemErro = "Caracteres por mensagem devem ficar entre 200 e 8000.";
                return false;
            }

            if (!ValidarInteiro(txtFerramentasMaxRodadasPorMensagem.Text, 1, 10))
            {
                mensagemErro = "Rodadas de ferramentas devem ficar entre 1 e 10.";
                return false;
            }

            if (!ValidarInteiro(txtRateLimit.Text, 1, 10000))
            {
                mensagemErro = "Limite usuario/dia deve ficar entre 1 e 10000.";
                return false;
            }

            if (!ValidarInteiro(txtArquivosMaxArquivos.Text, 1, 10))
            {
                mensagemErro = "Arquivos/msg deve ficar entre 1 e 10.";
                return false;
            }

            if (!ValidarInteiro(txtArquivosMaxMB.Text, 1, 100))
            {
                mensagemErro = "Max MB/arquivo deve ficar entre 1 e 100.";
                return false;
            }

            if (!ValidarInteiro(txtArquivosSyncMaxMB.Text, 1, 100))
            {
                mensagemErro = "Sync ate MB deve ficar entre 1 e 100.";
                return false;
            }

            if (ParaInt(txtArquivosSyncMaxMB.Text) > ParaInt(txtArquivosMaxMB.Text))
            {
                mensagemErro = "Sync ate MB nao pode ser maior que Max MB/arquivo.";
                return false;
            }

            if (!ValidarInteiro(txtArquivosMaxCharsTrecho.Text, 1000, 20000))
            {
                mensagemErro = "Chars/trecho deve ficar entre 1000 e 20000.";
                return false;
            }

            if (!ValidarInteiro(txtArquivosMaxTrechos.Text, 1, 20))
            {
                mensagemErro = "Trechos/resposta deve ficar entre 1 e 20.";
                return false;
            }

            if (!ValidarInteiro(txtArquivosRetencaoHoras.Text, 1, 168))
            {
                mensagemErro = "Retencao original h deve ficar entre 1 e 168.";
                return false;
            }

            if (!ValidarInteiro(txtConhecimentoPastaMax.Text, 1, 200))
            {
                mensagemErro = "Max. arquivos por varredura deve ficar entre 1 e 200.";
                return false;
            }

            // Dono dos documentos da pasta nao precisa mais de validacao: virou dropdown de usuarios
            // (sp_Select 'Usuarios'), entao nao ha como digitar um id que nao exista.

            // Avisa na hora de salvar, nao so quando a varredura falhar: caminho errado ou sem permissao
            // para a conta do servico TT_Windows (que roda no mesmo servidor e e quem le a pasta) e o erro mais
            // comum de configuracao.
            string pastaConhecimento = txtConhecimentoPastaEntrada.Text.Trim();
            if (pastaConhecimento.Length > 0 && !System.IO.Directory.Exists(pastaConhecimento))
            {
                mensagemErro = "Pasta de entrada da base de conhecimento nao encontrada ou sem acesso: " + pastaConhecimento;
                return false;
            }

            return true;
        }

        private bool ValidarProviderAtivo(string openAIKey, string geminiKey, string claudeKey, string groqKey, string compatKey, out string mensagemErro)
        {
            mensagemErro = string.Empty;

            switch (ddlProvider.SelectedValue)
            {
                case cls_IA_Config.ProviderGemini:
                    if (string.IsNullOrWhiteSpace(geminiKey))
                    {
                        mensagemErro = "A chave Gemini nao esta configurada.";
                        return false;
                    }
                    return true;
                case cls_IA_Config.ProviderClaude:
                    if (string.IsNullOrWhiteSpace(claudeKey))
                    {
                        mensagemErro = "A chave Claude nao esta configurada.";
                        return false;
                    }
                    return true;
                case cls_IA_Config.ProviderGroq:
                    if (string.IsNullOrWhiteSpace(groqKey))
                    {
                        mensagemErro = "A chave Groq nao esta configurada.";
                        return false;
                    }
                    return true;
                case cls_IA_Config.ProviderOpenAICompativel:
                    if (string.IsNullOrWhiteSpace(compatKey))
                    {
                        mensagemErro = "A chave do provider OpenAI compativel nao esta configurada.";
                        return false;
                    }
                    return true;
                case cls_IA_Config.ProviderOpenAI:
                default:
                    if (string.IsNullOrWhiteSpace(openAIKey))
                    {
                        mensagemErro = "A chave OpenAI nao esta configurada.";
                        return false;
                    }
                    return true;
            }
        }

        /*
            Recusa o que comprovadamente NAO e chave de API, sem tentar adivinhar o formato de
            cada provider — prefixos mudam, e bloquear por prefixo quebraria a tela no dia em
            que o provider mudar o padrao. Aqui so pegamos o que nunca pode ser chave:
            caminho de arquivo, texto com espaco e valor curto demais.
        */
        private bool ChaveApiPlausivel(TextBox campo, string rotuloProvider, out string mensagemErro)
        {
            mensagemErro = string.Empty;

            if (campo == null || string.IsNullOrWhiteSpace(campo.Text))
            {
                return true;   // campo em branco = manter a chave atual
            }

            string valor = campo.Text.Trim();
            string prefixo = "Chave " + rotuloProvider + ": ";

            if (valor.IndexOf('\\') >= 0 || valor.IndexOf('/') >= 0 || System.Text.RegularExpressions.Regex.IsMatch(valor, @"^[A-Za-z]:"))
            {
                mensagemErro = prefixo + "o valor informado parece um caminho de arquivo ou pasta, " +
                               "nao uma chave de API. Cole o CONTEUDO da chave, nao o caminho de onde ela esta salva.";
                return false;
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(valor, @"\s"))
            {
                mensagemErro = prefixo + "ha espaco ou quebra de linha no meio do valor. " +
                               "Confira se a copia pegou a chave inteira e nada alem dela.";
                return false;
            }

            if (valor.Length < 20)
            {
                mensagemErro = prefixo + "valor curto demais (" + valor.Length + " caracteres) para ser uma chave de API. " +
                               "Confira se a copia pegou a chave inteira.";
                return false;
            }

            return true;
        }

        // Os campos de chave vem sempre vazios ao carregar a tela: valor preenchido = troca pedida agora.
        private bool AlgumaChaveInformada()
        {
            return !string.IsNullOrWhiteSpace(txtOpenAIKey.Text)
                || !string.IsNullOrWhiteSpace(txtGeminiKey.Text)
                || !string.IsNullOrWhiteSpace(txtClaudeKey.Text)
                || !string.IsNullOrWhiteSpace(txtGroqKey.Text)
                || !string.IsNullOrWhiteSpace(txtCompatKey.Text);
        }

        /*
            Assistente publico (projeto TT_Assistente).

            As chaves IA.Publico.* nao existem no cls_IA_Config — ele so mapeia as do chat
            interno — e o TT_Flow nao referencia o projeto do assistente. Entao a leitura aqui
            e feita direto do dicionario de configuracao, e a gravacao pelo mesmo SalvarConfig
            das demais. Ao acrescentar uma chave nova, e so incluir nos dois metodos abaixo.
        */
        private static readonly string[] ProvidersPublico =
        {
            "", cls_IA_Config.ProviderOpenAI, cls_IA_Config.ProviderGemini,
            cls_IA_Config.ProviderClaude, cls_IA_Config.ProviderGroq, cls_IA_Config.ProviderOpenAICompativel
        };

        private void CarregarConfigPublico()
        {
            Dictionary<string, string> valores = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                DataTable tb = _repositorio.ConsultarConfig();
                foreach (DataRow row in tb.Rows)
                {
                    string chave = (row["sChave"] ?? string.Empty).ToString();
                    if (chave.StartsWith("IA.Publico.", StringComparison.OrdinalIgnoreCase) && !valores.ContainsKey(chave))
                    {
                        valores.Add(chave, (row["sValor"] ?? string.Empty).ToString());
                    }
                }
            }
            catch
            {
                // Estrutura do assistente ainda nao aplicada neste banco: campos ficam em branco
            }

            MontarComboProviderPublico();

            Selecionar(ddlPublicoHabilitado, ValorPublico(valores, "IA.Publico.Habilitado", "N").ToUpperInvariant() == "S" ? "S" : "N");
            Selecionar(ddlPublicoProvider, ValorPublico(valores, "IA.Publico.Provider", "").ToUpperInvariant());

            txtPublicoModelo.Text = ValorPublico(valores, "IA.Publico.Modelo", "");
            txtPublicoMaxTokensSaida.Text = ValorPublico(valores, "IA.Publico.MaxTokensSaida", "900");
            txtPublicoMaxCharsEntrada.Text = ValorPublico(valores, "IA.Publico.MaxCharsEntrada", "2000");
            txtPublicoRateLimitIp.Text = ValorPublico(valores, "IA.Publico.RateLimitPorIpDia", "40");
            txtPublicoTetoTokensDia.Text = ValorPublico(valores, "IA.Publico.TetoTokensDia", "400000");
            txtPublicoHistoricoMaxMensagens.Text = ValorPublico(valores, "IA.Publico.HistoricoMaxMensagens", "12");
            txtPublicoMaxRodadas.Text = ValorPublico(valores, "IA.Publico.MaxRodadasFerramenta", "4");
            txtPublicoRetencaoDias.Text = ValorPublico(valores, "IA.Publico.RetencaoDias", "180");
            txtPublicoEmailVendas.Text = ValorPublico(valores, "IA.Publico.EmailVendas", "");
            txtPublicoNuvemshopStoreId.Text = ValorPublico(valores, "IA.Publico.Nuvemshop.StoreId", "");
            txtPublicoUrlSite.Text = ValorPublico(valores, "IA.Publico.UrlSite", "");
            txtPublicoUrlLoja.Text = ValorPublico(valores, "IA.Publico.UrlLoja", "");
            txtPublicoCorsOrigens.Text = ValorPublico(valores, "IA.Publico.CorsOrigens", "");
            txtPublicoSaudacao.Text = ValorPublico(valores, "IA.Publico.SaudacaoInicial", "");
            txtPublicoPromptSistema.Text = ValorPublico(valores, "IA.Publico.PromptSistema", "");
            txtPublicoPromptLoja.Text = ValorPublico(valores, "IA.Publico.PromptSistema.Loja", "");
        }

        private void SalvarConfigPublico()
        {
            _repositorio.SalvarConfig("IA.Publico.Habilitado", ddlPublicoHabilitado.SelectedValue);
            _repositorio.SalvarConfig("IA.Publico.Provider", ddlPublicoProvider.SelectedValue);
            _repositorio.SalvarConfig("IA.Publico.Modelo", txtPublicoModelo.Text.Trim());
            _repositorio.SalvarConfig("IA.Publico.MaxTokensSaida", txtPublicoMaxTokensSaida.Text.Trim());
            _repositorio.SalvarConfig("IA.Publico.MaxCharsEntrada", txtPublicoMaxCharsEntrada.Text.Trim());
            _repositorio.SalvarConfig("IA.Publico.RateLimitPorIpDia", txtPublicoRateLimitIp.Text.Trim());
            _repositorio.SalvarConfig("IA.Publico.TetoTokensDia", txtPublicoTetoTokensDia.Text.Trim());
            _repositorio.SalvarConfig("IA.Publico.HistoricoMaxMensagens", txtPublicoHistoricoMaxMensagens.Text.Trim());
            _repositorio.SalvarConfig("IA.Publico.MaxRodadasFerramenta", txtPublicoMaxRodadas.Text.Trim());
            _repositorio.SalvarConfig("IA.Publico.RetencaoDias", txtPublicoRetencaoDias.Text.Trim());
            _repositorio.SalvarConfig("IA.Publico.EmailVendas", txtPublicoEmailVendas.Text.Trim());
            _repositorio.SalvarConfig("IA.Publico.Nuvemshop.StoreId", txtPublicoNuvemshopStoreId.Text.Trim());
            _repositorio.SalvarConfig("IA.Publico.UrlSite", txtPublicoUrlSite.Text.Trim());
            _repositorio.SalvarConfig("IA.Publico.UrlLoja", txtPublicoUrlLoja.Text.Trim());
            _repositorio.SalvarConfig("IA.Publico.CorsOrigens", txtPublicoCorsOrigens.Text.Trim());
            _repositorio.SalvarConfig("IA.Publico.SaudacaoInicial", txtPublicoSaudacao.Text.Trim());
            _repositorio.SalvarConfig("IA.Publico.PromptSistema", txtPublicoPromptSistema.Text.Trim());
            _repositorio.SalvarConfig("IA.Publico.PromptSistema.Loja", txtPublicoPromptLoja.Text.Trim());
        }

        private void MontarComboProviderPublico()
        {
            if (ddlPublicoProvider.Items.Count > 0)
            {
                return;
            }

            foreach (string provider in ProvidersPublico)
            {
                string rotulo = provider.Length == 0 ? "(mesmo do chat interno)" : provider;
                ddlPublicoProvider.Items.Add(new ListItem(rotulo, provider));
            }
        }

        private static string ValorPublico(Dictionary<string, string> valores, string chave, string padrao)
        {
            string valor;
            if (valores.TryGetValue(chave, out valor) && !string.IsNullOrWhiteSpace(valor))
            {
                return valor;
            }

            return padrao;
        }

        private bool ValidarConfigPublico(out string mensagemErro)
        {
            mensagemErro = string.Empty;

            if (!InteiroValido(txtPublicoMaxTokensSaida.Text, 200, 4000))
            {
                mensagemErro = "Assistente do site: máx. tokens de resposta deve ficar entre 200 e 4000.";
                return false;
            }

            if (!InteiroValido(txtPublicoMaxCharsEntrada.Text, 200, 8000))
            {
                mensagemErro = "Assistente do site: máx. caracteres da pergunta deve ficar entre 200 e 8000.";
                return false;
            }

            if (!InteiroValido(txtPublicoRateLimitIp.Text, 0, 100000))
            {
                mensagemErro = "Assistente do site: mensagens por visitante/dia deve ficar entre 0 e 100000.";
                return false;
            }

            if (!InteiroValido(txtPublicoTetoTokensDia.Text, 0, int.MaxValue))
            {
                mensagemErro = "Assistente do site: teto de tokens por dia inválido.";
                return false;
            }

            if (!InteiroValido(txtPublicoHistoricoMaxMensagens.Text, 0, 40))
            {
                mensagemErro = "Assistente do site: mensagens anteriores deve ficar entre 0 e 40.";
                return false;
            }

            if (!InteiroValido(txtPublicoMaxRodadas.Text, 1, 8))
            {
                mensagemErro = "Assistente do site: rodadas de ferramentas deve ficar entre 1 e 8.";
                return false;
            }

            if (!InteiroValido(txtPublicoRetencaoDias.Text, 0, 3650))
            {
                mensagemErro = "Assistente do site: guardar conversas por deve ficar entre 0 e 3650 dias.";
                return false;
            }

            /*
                Sem origem autorizada o widget nao funciona em lugar nenhum — o navegador
                bloqueia toda chamada. Ligar o assistente sem preencher isso produz um chat
                que parece no ar e nao responde, entao vale barrar aqui.
            */
            if (ddlPublicoHabilitado.SelectedValue == "S" && string.IsNullOrWhiteSpace(txtPublicoCorsOrigens.Text))
            {
                mensagemErro = "Assistente do site: informe ao menos um site autorizado antes de ativar, " +
                               "senão o chat é bloqueado pelo navegador em qualquer página.";
                return false;
            }

            foreach (string origem in (txtPublicoCorsOrigens.Text ?? string.Empty).Split(','))
            {
                string item = origem.Trim();
                if (item.Length > 0 && !item.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                                    && !item.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    mensagemErro = "Assistente do site: cada site autorizado precisa começar com https:// (ex.: " +
                                   "https://tecandtec.com.br). Valor inválido: " + item;
                    return false;
                }
            }

            return true;
        }

        private static bool InteiroValido(string texto, int minimo, int maximo)
        {
            int valor;
            if (!int.TryParse((texto ?? string.Empty).Trim(), out valor))
            {
                return false;
            }

            return valor >= minimo && valor <= maximo;
        }

        private bool SalvarChaveSeInformada(string chave, TextBox campo)
        {
            if (campo == null || string.IsNullOrWhiteSpace(campo.Text))
            {
                return false;
            }

            _repositorio.SalvarConfig(chave, cls_IA_Criptografia.Criptografar(campo.Text));
            campo.Text = string.Empty;
            return true;
        }

        private string ModeloAtivoSelecionado()
        {
            switch (ddlProvider.SelectedValue)
            {
                case cls_IA_Config.ProviderGemini:
                    return ModeloSelecionado(ddlGeminiModelo);
                case cls_IA_Config.ProviderClaude:
                    return ModeloSelecionado(ddlClaudeModelo);
                case cls_IA_Config.ProviderGroq:
                    return ModeloSelecionado(ddlGroqModelo);
                case cls_IA_Config.ProviderOpenAICompativel:
                    return ModeloSelecionado(ddlCompatModelo);
                case cls_IA_Config.ProviderOpenAI:
                default:
                    return ModeloSelecionado(ddlOpenAIModelo);
            }
        }

        private void LimparCamposChave()
        {
            txtOpenAIKey.Text = string.Empty;
            txtGeminiKey.Text = string.Empty;
            txtClaudeKey.Text = string.Empty;
            txtGroqKey.Text = string.Empty;
            txtCompatKey.Text = string.Empty;
        }

        private static void AtualizarStatusChave(Label label, bool configurada)
        {
            label.Text = configurada ? "Configurada" : "Nao configurada";
            label.CssClass = configurada ? "label label-success" : "label label-warning";
        }

        private static string ValorNovoOuAtual(string novoValor, string valorAtual)
        {
            return string.IsNullOrWhiteSpace(novoValor) ? (valorAtual ?? string.Empty) : novoValor.Trim();
        }

        private static bool TextoValido(string valor, int minimo, int maximo)
        {
            valor = (valor ?? string.Empty).Trim();
            return valor.Length >= minimo && valor.Length <= maximo;
        }

        private static bool ValidarInteiro(string valor, int minimo, int maximo)
        {
            int numero;
            if (!int.TryParse((valor ?? string.Empty).Trim(), out numero))
            {
                return false;
            }

            return numero >= minimo && numero <= maximo;
        }

        private static int ParaInt(string valor)
        {
            int numero;
            return int.TryParse((valor ?? string.Empty).Trim(), out numero) ? numero : 0;
        }

        private static void Selecionar(DropDownList ddl, string valor)
        {
            if (ddl.Items.FindByValue(valor) != null)
            {
                ddl.SelectedValue = valor;
            }
        }

        private void CarregarOpcoesModelos()
        {
            // Catalogo central em cls_IA_Config: mesma fonte do select de precos da tela IA - Uso
            CarregarOpcoes(ddlOpenAIModelo, cls_IA_Config.ModelosCuradosOpenAI);
            CarregarOpcoes(ddlGeminiModelo, cls_IA_Config.ModelosCuradosGemini);
            CarregarOpcoes(ddlClaudeModelo, cls_IA_Config.ModelosCuradosClaude);
            CarregarOpcoes(ddlGroqModelo, cls_IA_Config.ModelosCuradosGroq);
            CarregarOpcoes(ddlCompatModelo, cls_IA_Config.ModelosCuradosCompat);
        }

        private static void CarregarOpcoes(DropDownList ddl, string[,] opcoes)
        {
            if (ddl == null)
            {
                return;
            }

            ddl.Items.Clear();

            for (int i = 0; i < opcoes.GetLength(0); i++)
            {
                ddl.Items.Add(new ListItem(opcoes[i, 0], opcoes[i, 1]));
            }
        }

        private static void SelecionarModelo(DropDownList ddl, string valor)
        {
            valor = (valor ?? string.Empty).Trim();
            if (ddl == null)
            {
                return;
            }

            if (ddl.Items.FindByValue(valor) == null)
            {
                ddl.Items.Add(new ListItem(string.IsNullOrWhiteSpace(valor) ? "Selecione" : valor + " (atual)", valor));
            }

            ddl.SelectedValue = valor;
        }

        private static string ModeloSelecionado(DropDownList ddl)
        {
            return ddl == null ? string.Empty : (ddl.SelectedValue ?? string.Empty).Trim();
        }
    }
}
