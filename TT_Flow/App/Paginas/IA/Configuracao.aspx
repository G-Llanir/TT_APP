<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Configuracao.aspx.cs" Inherits="TT_Flow.App.Paginas.IA.Configuracao" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <asp:UpdatePanel ID="updConfiguracaoIA" runat="server">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Configuração IA"></asp:Label><small> Administração</small>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="Configuracao IA" />
                </div>

                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                </div>

                <div class="col-lg-12">
                    <div class="alert alert-info">
                        <i class="fa fa-info-circle"></i>
                        Passe o mouse no ícone <i class="fa fa-question-circle"></i> ao lado de cada campo para ver o que ele faz.
                        As alterações só valem depois de clicar em <strong>Salvar</strong>.
                    </div>
                </div>

                <div class="col-lg-12">
                    <ul class="nav nav-tabs" id="abasConfigIA">
                        <li class="active"><a href="#abaGerais" data-toggle="tab"><i class="fa fa-cogs"></i> Configurações gerais</a></li>
                        <li><a href="#abaFerramentas" data-toggle="tab"><i class="fa fa-plug"></i> Ferramentas</a></li>
                    </ul>
                </div>

                <div class="col-lg-12">
                    <div class="tab-content" style="padding-top:15px;">
                        <div class="tab-pane active" id="abaGerais">
                            <div class="row">

                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-cogs"></i> Provedor e comportamento
                                <i class="fa fa-question-circle" data-toggle="tooltip" title="Define qual serviço de IA o sistema usa e como ele responde."></i>
                            </h3>
                        </div>
                        <div class="panel-body">
                            <div class="row d-flex fw-w">
                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>IA habilitada
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Liga ou desliga a IA em todo o sistema. Em 'Não', o chat fica indisponível para todos os usuários."></i>
                                        </label>
                                        <asp:DropDownList ID="ddlHabilitado" CssClass="form-control" runat="server">
                                            <asp:ListItem Text="Não" Value="N" />
                                            <asp:ListItem Text="Sim" Value="S" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Provedor ativo
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Empresa de IA usada nas respostas. Precisa ter a chave dela cadastrada mais abaixo."></i>
                                        </label>
                                        <asp:DropDownList ID="ddlProvider" CssClass="form-control" runat="server">
                                            <asp:ListItem Text="OpenAI" Value="OPENAI" />
                                            <asp:ListItem Text="Gemini" Value="GEMINI" />
                                            <asp:ListItem Text="Claude" Value="CLAUDE" />
                                            <asp:ListItem Text="Groq" Value="GROQ" />
                                            <asp:ListItem Text="OpenAI compatível" Value="OPENAI_COMPATIVEL" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Provedor reserva (fallback)
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Usado automaticamente se o provedor ativo falhar. Precisa ter chave cadastrada. Vazio = desligado."></i>
                                        </label>
                                        <asp:DropDownList ID="ddlProviderFallback" CssClass="form-control" runat="server">
                                            <asp:ListItem Text="(sem reserva)" Value="" />
                                            <asp:ListItem Text="OpenAI" Value="OPENAI" />
                                            <asp:ListItem Text="Gemini" Value="GEMINI" />
                                            <asp:ListItem Text="Claude" Value="CLAUDE" />
                                            <asp:ListItem Text="Groq" Value="GROQ" />
                                            <asp:ListItem Text="OpenAI compatível" Value="OPENAI_COMPATIVEL" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Guardar conversa no provedor
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Permite que o provedor armazene as conversas nos servidores dele. Deixe 'Não' se preferir não reter dados fora do sistema."></i>
                                        </label>
                                        <asp:DropDownList ID="ddlStoreExterno" CssClass="form-control" runat="server">
                                            <asp:ListItem Text="Não" Value="N" />
                                            <asp:ListItem Text="Sim" Value="S" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Nível de raciocínio
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Quanto a IA 'pensa' antes de responder. Mais alto acerta mais em tarefas difíceis, porém é mais lento e custa mais. 'Nenhum' = resposta direta."></i>
                                        </label>
                                        <asp:DropDownList ID="ddlRaciocinioNivel" CssClass="form-control" runat="server">
                                            <asp:ListItem Text="Nenhum" Value="none" />
                                            <asp:ListItem Text="Baixo" Value="low" />
                                            <asp:ListItem Text="Médio" Value="medium" />
                                            <asp:ListItem Text="Alto" Value="high" />
                                            <asp:ListItem Text="Muito alto" Value="xhigh" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Tamanho das respostas
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Quão detalhadas são as respostas. Baixa = curtas e diretas; Alta = mais explicações."></i>
                                        </label>
                                        <asp:DropDownList ID="ddlVerbosidadeTexto" CssClass="form-control" runat="server">
                                            <asp:ListItem Text="Baixa (curtas)" Value="low" />
                                            <asp:ListItem Text="Média" Value="medium" />
                                            <asp:ListItem Text="Alta (detalhadas)" Value="high" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-cube"></i> Modelos
                                <i class="fa fa-question-circle" data-toggle="tooltip" title="O 'modelo' é a versão da IA de cada provedor. Só o modelo do provedor ativo (e do reserva) é usado; os demais ficam guardados para quando você trocar de provedor."></i>
                            </h3>
                        </div>
                        <div class="panel-body">
                            <div class="row d-flex fw-w">
                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Modelo OpenAI
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Versão usada quando o provedor OpenAI está ativo."></i>
                                        </label>
                                        <asp:DropDownList ID="ddlOpenAIModelo" CssClass="form-control" runat="server" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Modelo Gemini
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Versão usada quando o provedor Gemini está ativo."></i>
                                        </label>
                                        <asp:DropDownList ID="ddlGeminiModelo" CssClass="form-control" runat="server" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Modelo Claude
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Versão usada quando o provedor Claude está ativo."></i>
                                        </label>
                                        <asp:DropDownList ID="ddlClaudeModelo" CssClass="form-control" runat="server" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Modelo Groq
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Versão usada quando o provedor Groq está ativo."></i>
                                        </label>
                                        <asp:DropDownList ID="ddlGroqModelo" CssClass="form-control" runat="server" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Modelo compatível
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Versão usada com o provedor 'OpenAI compatível'."></i>
                                        </label>
                                        <asp:DropDownList ID="ddlCompatModelo" CssClass="form-control" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="row d-flex fw-w">
                                <div class="col-lg-6 col-md-8 col-sm-12">
                                    <div class="form-group">
                                        <label>Endereço (URL) do provedor compatível
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Só para o provedor 'OpenAI compatível': endereço do serviço que imita a API da OpenAI (ex.: um servidor próprio). Deixe em branco se não usar."></i>
                                        </label>
                                        <asp:TextBox ID="txtCompatBaseUrl" CssClass="form-control" runat="server" MaxLength="300" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Orçamento de raciocínio do Claude
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Quantos tokens o Claude pode gastar 'pensando' antes de responder. Maior = raciocínio mais profundo, porém mais lento e caro. Vale só para o Claude."></i>
                                        </label>
                                        <asp:TextBox ID="txtThinkingBudgetTokens" CssClass="form-control" runat="server" MaxLength="8" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-key"></i> Chaves de API
                                <i class="fa fa-question-circle" data-toggle="tooltip" title="A chave de API funciona como uma senha de acesso ao serviço de cada provedor. É guardada criptografada. Preencha apenas para cadastrar ou trocar a chave; deixe em branco para manter a atual."></i>
                            </h3>
                        </div>
                        <div class="panel-body">
                            <div class="row d-flex fw-w">
                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Chave OpenAI</label><br />
                                        <asp:Label ID="lblOpenAIKeyStatus" runat="server" /><br />
                                        <asp:TextBox ID="txtOpenAIKey" CssClass="form-control" runat="server" TextMode="Password" MaxLength="500" autocomplete="new-password" placeholder="Colar nova chave" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Chave Gemini</label><br />
                                        <asp:Label ID="lblGeminiKeyStatus" runat="server" /><br />
                                        <asp:TextBox ID="txtGeminiKey" CssClass="form-control" runat="server" TextMode="Password" MaxLength="500" autocomplete="new-password" placeholder="Colar nova chave" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Chave Claude</label><br />
                                        <asp:Label ID="lblClaudeKeyStatus" runat="server" /><br />
                                        <asp:TextBox ID="txtClaudeKey" CssClass="form-control" runat="server" TextMode="Password" MaxLength="500" autocomplete="new-password" placeholder="Colar nova chave" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Chave Groq</label><br />
                                        <asp:Label ID="lblGroqKeyStatus" runat="server" /><br />
                                        <asp:TextBox ID="txtGroqKey" CssClass="form-control" runat="server" TextMode="Password" MaxLength="500" autocomplete="new-password" placeholder="Colar nova chave" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Chave compatível</label><br />
                                        <asp:Label ID="lblCompatKeyStatus" runat="server" /><br />
                                        <asp:TextBox ID="txtCompatKey" CssClass="form-control" runat="server" TextMode="Password" MaxLength="500" autocomplete="new-password" placeholder="Colar nova chave" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-tachometer"></i> Limites de uso
                                <i class="fa fa-question-circle" data-toggle="tooltip" title="Controlam consumo e custo. 'Token' é a unidade que a IA usa para medir texto: 1 token equivale a cerca de 3/4 de uma palavra em português."></i>
                            </h3>
                        </div>
                        <div class="panel-body">
                            <div class="row d-flex fw-w">
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Máx. tokens de entrada
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Tamanho máximo do que é enviado à IA por mensagem (pergunta + contexto + histórico)."></i>
                                        </label>
                                        <asp:TextBox ID="txtMaxTokensEntrada" CssClass="form-control" runat="server" MaxLength="8" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Máx. tokens de resposta
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Tamanho máximo de cada resposta gerada pela IA."></i>
                                        </label>
                                        <asp:TextBox ID="txtMaxTokensSaida" CssClass="form-control" runat="server" MaxLength="8" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Mensagens anteriores
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Quantidade de mensagens anteriores enviadas como histórico para a IA. 0 = não enviar histórico."></i>
                                        </label>
                                        <asp:TextBox ID="txtHistoricoMaxMensagens" CssClass="form-control" runat="server" MaxLength="2" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Caracteres por mensagem
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Tamanho máximo de cada mensagem anterior enviada no histórico. Reduza para economizar tokens em conversas longas."></i>
                                        </label>
                                        <asp:TextBox ID="txtHistoricoMaxCharsPorMensagem" CssClass="form-control" runat="server" MaxLength="4" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Rodadas de ferramentas
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Quantidade máxima de ciclos sequenciais de chamadas de ferramentas em uma mesma mensagem. Aumentar ajuda fluxos com várias consultas; também aumenta tempo e custo."></i>
                                        </label>
                                        <asp:TextBox ID="txtFerramentasMaxRodadasPorMensagem" CssClass="form-control" runat="server" MaxLength="2" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Limite de mensagens por usuário/dia
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Quantas mensagens cada pessoa pode enviar por dia. Protege contra uso excessivo e custo."></i>
                                        </label>
                                        <asp:TextBox ID="txtRateLimit" CssClass="form-control" runat="server" MaxLength="8" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Limite de mensagens por IP/dia
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Limite diário de mensagens vindas do mesmo IP, além do limite por usuário. Protege contra abuso de várias contas do mesmo ponto. 0 = desligado (cuidado: em rede com NAT/IP compartilhado, um único IP pode ser toda a empresa)."></i>
                                        </label>
                                        <asp:TextBox ID="txtRateLimitIp" CssClass="form-control" runat="server" MaxLength="8" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Retenção da auditoria (dias)
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Por quantos dias os registros de auditoria da IA são mantidos; os mais antigos são apagados automaticamente para o banco não crescer sem limite. 0 = manter para sempre."></i>
                                        </label>
                                        <asp:TextBox ID="txtRetencaoDias" CssClass="form-control" runat="server" MaxLength="6" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-paperclip"></i> Arquivos no chat
                                <i class="fa fa-question-circle" data-toggle="tooltip" title="Regras para anexos enviados no chat (PDF, Word, Excel, etc.). O conteúdo é convertido em texto para a IA ler."></i>
                            </h3>
                        </div>
                        <div class="panel-body">
                            <div class="row d-flex fw-w">
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Permitir anexos
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Liga ou desliga o envio de arquivos no chat."></i>
                                        </label>
                                        <asp:DropDownList ID="ddlArquivosHabilitado" CssClass="form-control" runat="server">
                                            <asp:ListItem Text="Não" Value="N" />
                                            <asp:ListItem Text="Sim" Value="S" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Máx. arquivos por mensagem
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Quantos arquivos podem ser anexados de uma vez."></i>
                                        </label>
                                        <asp:TextBox ID="txtArquivosMaxArquivos" CssClass="form-control" runat="server" MaxLength="3" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Tamanho máx. por arquivo (MB)
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Arquivos maiores que isso são recusados."></i>
                                        </label>
                                        <asp:TextBox ID="txtArquivosMaxMB" CssClass="form-control" runat="server" MaxLength="4" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Converter na hora até (MB)
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Arquivos até esse tamanho são convertidos na hora. Acima disso, a conversão ocorre em segundo plano."></i>
                                        </label>
                                        <asp:TextBox ID="txtArquivosSyncMaxMB" CssClass="form-control" runat="server" MaxLength="4" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Caracteres por trecho
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Ao usar um arquivo na resposta, a IA lê pedaços ('trechos') com até esta quantidade de caracteres."></i>
                                        </label>
                                        <asp:TextBox ID="txtArquivosMaxCharsTrecho" CssClass="form-control" runat="server" MaxLength="6" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Máx. trechos por resposta
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Quantos pedaços de arquivos a IA pode usar em uma única resposta."></i>
                                        </label>
                                        <asp:TextBox ID="txtArquivosMaxTrechos" CssClass="form-control" runat="server" MaxLength="3" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Guardar original por (horas)
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Por quanto tempo o arquivo enviado fica guardado antes de ser apagado. O texto já convertido permanece."></i>
                                        </label>
                                        <asp:TextBox ID="txtArquivosRetencaoHoras" CssClass="form-control" runat="server" MaxLength="4" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <%-- ===== Base de conhecimento ===== --%>
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-book"></i> Base de conhecimento
                                <i class="fa fa-question-circle" data-toggle="tooltip" title="Documentos da empresa e FAQ aprovada que a IA consulta para responder dúvidas. Gerenciada na tela IA - Conhecimento."></i>
                            </h3>
                        </div>
                        <div class="panel-body">
                            <div class="row d-flex fw-w">
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Sempre disponível no chat
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Sim: a IA consulta a base sozinha quando a pergunta pede, sem o usuário apertar nada — e o botão do chat passa a ser o modo estrito (responder somente pela base). Não: comportamento antigo, a base só é usada com o botão ligado."></i>
                                        </label>
                                        <asp:DropDownList ID="ddlConhecimentoSempreDisponivel" CssClass="form-control" runat="server">
                                            <asp:ListItem Text="Sim" Value="S" />
                                            <asp:ListItem Text="Não" Value="N" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-6 col-md-8 col-sm-12">
                                    <div class="form-group">
                                        <label>Pasta de entrada
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Pasta do servidor onde basta largar os arquivos para entrarem na base. Subpasta vira a categoria do documento, e o arquivo é movido para _Processados ou _Erros depois de lido. Vazio = desligada. Quem lê é o serviço TT_Windows, a cada poucos minutos: a pasta precisa ser acessível (com escrita) pela conta desse serviço."></i>
                                        </label>
                                        <asp:TextBox ID="txtConhecimentoPastaEntrada" CssClass="form-control" runat="server" MaxLength="400" placeholder="Ex.: C:\TT\BaseConhecimento" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Máx. arquivos por varredura
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Quantos arquivos da pasta são processados de cada vez. O que passar do limite fica para a varredura seguinte."></i>
                                        </label>
                                        <asp:TextBox ID="txtConhecimentoPastaMax" CssClass="form-control" runat="server" MaxLength="3" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Dono dos documentos da pasta
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Usuário registrado como responsável por todo documento que entrar pela pasta. A leitura é automática, feita pelo serviço TT_Windows sem ninguém logado — por isso vale escolher um usuário técnico, como Processo Automático."></i>
                                        </label>
                                        <asp:DropDownList ID="ddlConhecimentoPastaIdUsuario" CssClass="form-control Caixa_Selecao" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-globe"></i> Assistente do site público
                                <i class="fa fa-question-circle" data-toggle="tooltip" title="Chat que atende visitantes de tecandtec.com.br e da loja. É uma aplicação separada, sem acesso a dados do ERP: estas configurações valem só para ele, e não afetam o chat interno."></i>
                            </h3>
                        </div>
                        <div class="panel-body">
                            <div class="row d-flex fw-w">
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Assistente do site ativo
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Desligar tira o chat do ar imediatamente, na requisição seguinte, sem republicar nada. É o botão de emergência."></i>
                                        </label>
                                        <asp:DropDownList ID="ddlPublicoHabilitado" CssClass="form-control" runat="server">
                                            <asp:ListItem Text="Não" Value="N" />
                                            <asp:ListItem Text="Sim" Value="S" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Provedor
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Deixe em branco para usar o mesmo provedor do chat interno. Escolher outro permite atender o site com um modelo mais barato."></i>
                                        </label>
                                        <asp:DropDownList ID="ddlPublicoProvider" CssClass="form-control" runat="server" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Modelo
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Em branco usa o modelo padrão do provedor escolhido. O site atende volume alto de perguntas simples, então um modelo rápido costuma bastar."></i>
                                        </label>
                                        <asp:TextBox ID="txtPublicoModelo" CssClass="form-control" runat="server" MaxLength="100" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Máx. tokens de resposta
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Tamanho máximo de cada resposta. Respostas curtas funcionam melhor no widget e custam menos."></i>
                                        </label>
                                        <asp:TextBox ID="txtPublicoMaxTokensSaida" CssClass="form-control" runat="server" MaxLength="6" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Máx. caracteres da pergunta
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Corta a mensagem do visitante nesse tamanho antes de enviar à IA."></i>
                                        </label>
                                        <asp:TextBox ID="txtPublicoMaxCharsEntrada" CssClass="form-control" runat="server" MaxLength="6" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Mensagens por visitante/dia
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Limite por IP em 24h. 0 desliga o limite, o que não é recomendado num site aberto à internet."></i>
                                        </label>
                                        <asp:TextBox ID="txtPublicoRateLimitIp" CssClass="form-control" runat="server" MaxLength="7" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Teto de tokens por dia
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Controle de custo. Somados todos os visitantes, ao atingir esse total no dia o assistente se desliga sozinho até a virada. 0 = sem teto."></i>
                                        </label>
                                        <asp:TextBox ID="txtPublicoTetoTokensDia" CssClass="form-control" runat="server" MaxLength="9" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Mensagens anteriores
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Quantas mensagens da conversa vão como histórico para a IA."></i>
                                        </label>
                                        <asp:TextBox ID="txtPublicoHistoricoMaxMensagens" CssClass="form-control" runat="server" MaxLength="2" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Rodadas de ferramentas
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Quantas buscas seguidas o assistente pode fazer para responder uma pergunta."></i>
                                        </label>
                                        <asp:TextBox ID="txtPublicoMaxRodadas" CssClass="form-control" runat="server" MaxLength="2" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Guardar conversas por (dias)
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="LGPD: conversas de visitante sem pedido de orçamento são apagadas depois desse prazo. 0 desliga a limpeza."></i>
                                        </label>
                                        <asp:TextBox ID="txtPublicoRetencaoDias" CssClass="form-control" runat="server" MaxLength="5" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>E-mail comercial
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Endereço que o assistente informa ao visitante, e destino dos pedidos de orçamento."></i>
                                        </label>
                                        <asp:TextBox ID="txtPublicoEmailVendas" CssClass="form-control" runat="server" MaxLength="200" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Código da loja (Nuvemshop)
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Usado só quando a integração com o catálogo da loja for ligada. O token de acesso não fica nesta tela."></i>
                                        </label>
                                        <asp:TextBox ID="txtPublicoNuvemshopStoreId" CssClass="form-control" runat="server" MaxLength="30" />
                                    </div>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Endereço do site
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Base do site institucional. É de onde o assistente coleta o conteúdo que usa para responder."></i>
                                        </label>
                                        <asp:TextBox ID="txtPublicoUrlSite" CssClass="form-control" runat="server" MaxLength="200" />
                                    </div>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Endereço da loja
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Base da loja virtual, usada nos links que o assistente oferece ao visitante."></i>
                                        </label>
                                        <asp:TextBox ID="txtPublicoUrlLoja" CssClass="form-control" runat="server" MaxLength="200" />
                                    </div>
                                </div>
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Sites autorizados a exibir o chat
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Endereços separados por vírgula. Um site fora desta lista recebe erro ao tentar usar o assistente. Serve para ninguém embutir o chat em outro lugar e gastar o orçamento de tokens."></i>
                                        </label>
                                        <asp:TextBox ID="txtPublicoCorsOrigens" CssClass="form-control" runat="server" MaxLength="1000" />
                                        <span class="text-muted">Endereço exato, com https://. Retire qualquer endereço de teste antes de usar em produção.</span>
                                    </div>
                                </div>
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Primeira mensagem do chat
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Saudação exibida quando o visitante abre o widget."></i>
                                        </label>
                                        <asp:TextBox ID="txtPublicoSaudacao" CssClass="form-control" runat="server" MaxLength="500" />
                                    </div>
                                </div>
                                <div class="col-lg-6 col-md-12">
                                    <div class="form-group">
                                        <label>Instruções extras — todas as conversas
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Texto acrescentado às instruções da IA em qualquer conversa do site. Em branco usa só as instruções padrão."></i>
                                        </label>
                                        <asp:TextBox ID="txtPublicoPromptSistema" CssClass="form-control" runat="server" TextMode="MultiLine" Rows="6" />
                                    </div>
                                </div>
                                <div class="col-lg-6 col-md-12">
                                    <div class="form-group">
                                        <label>Instruções extras — só na loja
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Aplicadas apenas quando o visitante abre o chat dentro da loja. É o que faz o assistente mandar preço e estoque para a página do produto em vez de encaminhar ao comercial."></i>
                                        </label>
                                        <asp:TextBox ID="txtPublicoPromptLoja" CssClass="form-control" runat="server" TextMode="MultiLine" Rows="6" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <fieldset class="col-lg-12 form-stacked actions">
                    <asp:Button ID="cmdSalvar" CssClass="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                    <asp:Button ID="cmdRecarregar" CssClass="btn btn-lg btn-info" runat="server" Text="Recarregar" OnClick="cmdRecarregar_Click" />
                    <span class="text-muted padd-l">As alterações só valem depois de clicar em Salvar. "Recarregar" descarta o que você mudou e recarrega os valores salvos.</span>
                </fieldset>

                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-wrench"></i> Manutenção
                                <i class="fa fa-question-circle" data-toggle="tooltip" title="A limpeza roda sozinha cerca de 1x por dia. Use o botão para forçar agora: purga a auditoria acima da retenção e apaga arquivos temporários órfãos em App_Data/IA_Temp."></i>
                            </h3>
                        </div>
                        <div class="panel-body">
                            <asp:Button ID="cmdExecutarLimpeza" CssClass="btn btn-info" runat="server" Text="Executar limpeza agora" OnClick="cmdExecutarLimpeza_Click" />
                            <span class="text-muted padd-l">Purga a auditoria acima da retenção e remove arquivos temporários órfãos. Roda automaticamente cerca de 1x por dia.</span>
                        </div>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-user"></i> Seu uso de hoje</h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-lg-4 col-md-4 col-sm-12">
                                    <strong>Mensagens hoje:</strong>
                                    <asp:Label ID="lblMensagensHoje" runat="server" />
                                </div>
                                <div class="col-lg-4 col-md-4 col-sm-12">
                                    <strong>Conversas hoje:</strong>
                                    <asp:Label ID="lblConversasHoje" runat="server" />
                                </div>
                                <div class="col-lg-4 col-md-4 col-sm-12">
                                    <strong>Tokens de resposta hoje:</strong>
                                    <asp:Label ID="lblTokensSaidaHoje" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart"></i> Uso geral — últimos 30 dias
                                <i class="fa fa-question-circle" data-toggle="tooltip" title="Visão geral do consumo de IA por todo o sistema nos últimos 30 dias."></i>
                            </h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <strong>Conversas:</strong>
                                    <asp:Label ID="lblUsoConversas30d" runat="server" />
                                </div>
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <strong>Tokens de entrada:</strong>
                                    <asp:Label ID="lblUsoTokensEntrada30d" runat="server" />
                                </div>
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <strong>Tokens de resposta:</strong>
                                    <asp:Label ID="lblUsoTokensSaida30d" runat="server" />
                                </div>
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <strong>Custo estimado:</strong>
                                    <asp:Label ID="lblUsoCusto30d" runat="server" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <h5><strong>Mensagens por dia</strong></h5>
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvUsoDia" CssClass="table table-striped table-bordered table-condensed"
                                            runat="server" Width="100%" AutoGenerateColumns="True" GridLines="None" />
                                    </div>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <h5><strong>Top usuários (tokens de resposta)</strong></h5>
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvUsoUsuarios" CssClass="table table-striped table-bordered table-condensed"
                                            runat="server" Width="100%" AutoGenerateColumns="True" GridLines="None" />
                                    </div>
                                </div>
                            </div>
                            <small>Custo estimado em US$, calculado modelo a modelo pelos preços configurados na tela IA - Uso (preço por modelo com fallback no preço padrão).</small>
                        </div>
                    </div>
                </div>

                            </div><!-- /.row #abaGerais -->
                        </div><!-- /#abaGerais -->

                        <div class="tab-pane" id="abaFerramentas">
                            <div class="row">

                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-plug"></i> Ferramentas registradas
                                <i class="fa fa-question-circle" data-toggle="tooltip" title="Ferramentas são as ações que a IA pode executar no sistema (consultar pedidos, produtos, etc.). A IA só usa as que o usuário tem permissão de acessar."></i>
                            </h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group">
                                <a href="Ferramentas_Detalhe.aspx" class="btn btn-sm btn-success"><i class="fa fa-plus"></i> Nova ferramenta</a>
                                <span class="text-muted padd-l">Interna = definida no código (edita/desativa, não exclui). Personalizada = criada aqui.</span>
                            </div>
                            <div class="table-responsive">
                                <asp:GridView ID="dtgvFerramentas" CssClass="table table-striped table-bordered table-hover"
                                    runat="server" Width="100%" AutoGenerateColumns="False" GridLines="None"
                                    Font-Names="Tahoma" Font-Size="Small" DataKeyNames="Nome"
                                    OnRowDataBound="dtgvFerramentas_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="IdFerramentaIA" HeaderText="ID" />
                                        <asp:BoundField DataField="Nome" HeaderText="Nome" />
                                        <asp:BoundField DataField="Modulo" HeaderText="Módulo" />
                                        <asp:BoundField DataField="IdRecursoNecessario" HeaderText="Permissão necessária" />
                                        <asp:BoundField DataField="MaxRegistros" HeaderText="Máx. registros" />
                                        <asp:BoundField DataField="Tipo" HeaderText="Tipo" />
                                        <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkAtiva" runat="server" AutoPostBack="true"
                                                    OnCheckedChanged="chkAtiva_CheckedChanged"
                                                    Checked='<%# Convert.ToString(Eval("Ativa")) == "Sim" %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="PermitidaUsuarioAtual" HeaderText="Permitida p/ você" />
                                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" />
                                        <asp:TemplateField HeaderText="">
                                            <ItemTemplate>
                                                <asp:HyperLink runat="server" CssClass="btn btn-default btn-xs" ToolTip="Editar"
                                                    NavigateUrl='<%# "Ferramentas_Detalhe.aspx?id=" + Eval("IdFerramentaIA") %>'><i class="fa fa-pencil"></i></asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>

                            </div><!-- /.row #abaFerramentas -->
                        </div><!-- /#abaFerramentas -->
                    </div><!-- /.tab-content -->
                </div><!-- /.col-lg-12 wrapper das abas -->
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script type="text/javascript">
        // Mantem a aba ativa entre os postbacks parciais do UpdatePanel (checkbox de ferramenta,
        // Salvar, Recarregar, Limpeza). Sem isso, cada postback voltaria para a primeira aba.
        (function () {
            var CHAVE = 'cfgIA_abaAtiva';
            function ligarAbas() {
                if (!window.jQuery) { return; }
                var $ = window.jQuery;
                var $abas = $('#abasConfigIA a[data-toggle="tab"]');
                if (!$abas.length) { return; }

                $abas.off('shown.bs.tab.cfgia').on('shown.bs.tab.cfgia', function (e) {
                    try { sessionStorage.setItem(CHAVE, $(e.target).attr('href')); } catch (x) { }
                });

                var alvo = null;
                try { alvo = sessionStorage.getItem(CHAVE); } catch (x) { }
                if (alvo && $('#abasConfigIA a[href="' + alvo + '"]').length) {
                    $('#abasConfigIA a[href="' + alvo + '"]').tab('show');
                }
            }

            if (window.jQuery) { window.jQuery(document).ready(ligarAbas); }
            if (window.Sys && window.Sys.WebForms) {
                window.Sys.WebForms.PageRequestManager.getInstance().add_endRequest(ligarAbas);
            }
        })();
    </script>
</asp:Content>
