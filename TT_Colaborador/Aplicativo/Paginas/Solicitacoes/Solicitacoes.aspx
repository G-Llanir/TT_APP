<%@ Page Title="Minhas Solicitações" Language="C#" MasterPageFile="~/Site.master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="Solicitacoes.aspx.cs" Inherits="TT_Colaborador.Aplicativo.Paginas.Solicitacoes.Solicitacoes" %>

<%@ Register Src="~/Aplicativo/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* ESTILOS PARA A NOVA TELA */
        .solicitacao-card {
            border-left: 5px solid #007bff;
            margin-bottom: 1.5rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            transition: all 0.2s ease-in-out;
        }

            .solicitacao-card:hover {
                transform: translateY(-3px);
                box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            }

        .status-badge {
            font-size: 0.9rem;
            font-weight: 600;
            padding: 0.4em 0.8em;
            border-radius: 20px;
        }

        .btn-historico {
            font-size: 0.8rem;
            font-weight: 600;
        }

        .timeline {
            position: relative;
            padding: 1rem 0;
            margin-top: 1rem;
            list-style: none;
        }

            .timeline:before {
                content: '';
                position: absolute;
                left: 20px;
                top: 0;
                bottom: 0;
                width: 2px;
                background: #e9ecef;
            }

        .timeline-item {
            position: relative;
            margin-bottom: 1.5rem;
        }

        .timeline-icon {
            position: absolute;
            left: 14px;
            top: 5px;
            width: 15px;
            height: 15px;
            border-radius: 50%;
            border: 2px solid #fff;
            z-index: 10;
        }

        .timeline-content {
            margin-left: 45px;
            padding: 1rem;
            background-color: #f8f9fa;
            border-radius: 6px;
        }

            .timeline-content p {
                margin-bottom: 0.25rem;
            }

            .timeline-content .motivo {
                font-style: italic;
                color: #555;
                border-left: 3px solid #ccc;
                padding-left: 10px;
                margin-top: 5px;
            }

        .observacao-content {
            margin-top: 1rem;
            padding: 1rem;
            background-color: #f8f9fa;
            border-radius: 6px;
            border-left: 3px solid #0d6efd;
                   
        }

        .loader-mask {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(255, 255, 255, 0.9); /* Fundo branco meio transparente */
            z-index: 50;
            display: flex;
            justify-content: center;
            align-items: center;
            border-radius: 4px;
        }

        .loader-spinner {
            border: 5px solid #f3f3f3;
            border-top: 5px solid #337ab7; /* Azul padrão */
            border-radius: 50%;
            width: 50px;
            height: 50px;
            animation: spin 1s linear infinite;
        }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }

            100% {
                transform: rotate(360deg);
            }
        }
    </style>
</asp:Content>

<asp:Content ID="content_frmSolicitacoes" ContentPlaceHolderID="MainContent" runat="server">
    <script src='<%= ResolveUrl("~/Aplicativo/JS/vendor/ckeditor/ckeditor.js") %>'></script>

    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>
            <div id="hdd">
                <asp:HiddenField ID="hddidColaborador" runat="server" />
                <asp:HiddenField ID="hddidSolicitacao" runat="server" />
            </div>

            <div class="card">
                <div class="card-header card-tt">
                    <h3 class="card-title" style="color: white"><b>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Solicitações"></asp:Label></b>
                    </h3>
                </div>
                <div class="card-body">
                    <div id="div_solicitacoes" runat="server">
                        <div class="card mb-4">
                            <div class="card-header bg-light">
                                <h5 class="card-title mb-0"><i class="fa fa-filter me-2"></i>Filtros e Ações</h5>
                            </div>
                            <div class="card-body">
                                <div class="row align-items-end g-3">
                                    <div class="col-md-3">
                                        <label>Data Inicial</label>
                                        <asp:TextBox ID="txtdtInicial" type="date" CssClass="form-control" runat="server"></asp:TextBox>
                                    </div>
                                    <div class="col-md-3">
                                        <label>Data Final</label>
                                        <asp:TextBox ID="txtdtFinal" type="date" CssClass="form-control" runat="server"></asp:TextBox>
                                    </div>
                                    <div class="col-md-3">
                                        <label>Tipo de Solicitação</label>
                                        <asp:DropDownList ID="ddlidTipoSolicitacao" CssClass="form-select" runat="server"></asp:DropDownList>
                                    </div>
                                    <div class="col-md-3">
                                        <label>Pesquisar por Descrição</label>
                                        <asp:TextBox ID="txtPesquisa" CssClass="form-control" placeholder="Digite para pesquisar..." runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <hr />
                                <div class="row">
                                    <div class="col-12 text-end">
                                        <asp:Button ID="cmdPesquisar" CssClass="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                        <asp:Button ID="cmdNovaSolicitacao" CssClass="btn btn-success" runat="server" Text="Nova Solicitação" OnClick="cmdNovaSolicitacao_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Solicitacoes" />
                        <asp:Repeater ID="rptSolicitacoes" runat="server">
                            <ItemTemplate>
                                <div class='card solicitacao-card border-start border-4 border-<%# Eval("CorStatusAtual") %>'>
                                    <div class="card-body">
                                        <div class="d-flex justify-content-between align-items-start">
                                            <div>
                                                <h5 class="card-title mb-1">
                                                    <%# Eval("Tipo") %>
                                                    <small class="text-muted">- ID #<%# Eval("IdSolicitacao") %></small>
                                                </h5>
                                                <p class="card-text text-muted mb-2">
                                                    <i class="fa fa-calendar me-1"></i>
                                                    Solicitado em: <%# Eval("DataSolicitacao") %>
                                                </p>
                                            </div>

                                            <%-- Usa o nome da classe Bootstrap para colorir o badge --%>
                                            <span class='status-badge badge bg-<%# Eval("CorStatusAtual") %>'>
                                                <%# Eval("StatusAtual") %>
                                            </span>
                                        </div>

                                        <hr />

                                        <p class="card-text"><%# Eval("Descricao") %></p>

                                        <div class="observacao-container" style="display: none;">
                                            <div class="observacao-content">
                                                <asp:Literal ID="litObservacao" runat="server" Text='<%# Eval("Observacao") %>'></asp:Literal>
                                            </div>
                                        </div>

                                        <div class="text-end mt-3">
                                            <asp:LinkButton
                                                runat="server"
                                                OnClick="AbrirSolicitacao_Click"
                                                CommandArgument='<%# Eval("IdSolicitacao") %>'
                                                CssClass="btn btn-sm btn-outline-secondary me-2">
                        <i class="fa fa-eye me-1"></i>Ver Detalhes
                                            </asp:LinkButton>

                                            <span runat="server" visible='<%# !string.IsNullOrEmpty(Eval("Observacao") as string) %>'>
                                                <button type="button"
                                                    class="btn btn-sm btn-outline-info btn-observacao"
                                                    onclick="toggleObservacao(this)">
                                                    <i class="fa fa-eye"></i>Ver Descrição
   
                                                </button>
                                            </span>

                                            <button type="button"
                                                class="btn btn-sm btn-outline-primary btn-historico"
                                                onclick="toggleHistory(this)">
                                                <i class="fa fa-history me-1"></i>Ver Histórico
                                            </button>
                                        </div>

                                        <div class="historico-container" style="display: none;">
                                            <ul class="timeline">
                                                <asp:Repeater ID="rptHistorico" runat="server" DataSource='<%# Eval("Historico") %>'>
                                                    <ItemTemplate>
                                                        <li class="timeline-item">
                                                            <%-- Aqui também usamos a classe ao invés de cor fixa --%>
                                                            <div class='timeline-icon bg-<%# Eval("Cor") %>'></div>
                                                            <div class="timeline-content">
                                                                <p><strong><%# Eval("Status") %></strong></p>
                                                                <p class="text-muted small">
                                                                    <i class="fa fa-user me-1"></i>
                                                                    <%# Eval("Usuario") %> em <%# Eval("DataAtualizacao") %>
                                                                </p>
                                                                <p class="motivo"><%# Eval("Motivo") %></p>
                                                            </div>
                                                        </li>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                    </div>

                    <div id="div_dados" runat="server" visible="false">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Principal" />
                        <div class="card">
                            <div class="card-body">

                                <%--   <div class="row">
                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <h3>
                                                <span class="status" title="Status">
                                                    <asp:Label ID="lblsStatus" runat="server" TabIndex="100"></asp:Label>
                                                </span>
                                            </h3>
                                        </div>
                                    </div>
                                </div>--%>

                                <div class="row mb-3">
                                    <div class="col-lg-12">
                                        <div class="card p-2 bg-light border-0">
                                            <div class="d-flex justify-content-between align-items-center">

                                                <div>
                                                    <h3 class="m-0">
                                                        <span class="status" title="Status">
                                                            <asp:Label ID="lblsStatus" runat="server" TabIndex="100"></asp:Label>
                                                        </span>
                                                    </h3>
                                                </div>

                                                <div id="divCalcDias" style="display: none;">
                                                    <span class="badge bg-primary shadow-sm" style="font-size: 1.1rem; padding: 10px 15px;">
                                                        <i class="fa fa-calendar-check me-2"></i>
                                                        <span id="lblTotalDiasValor">0</span> Dias
                    </span>
                                                </div>

                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-lg-3">
                                        <div class="form-group">
                                            <label><b>Tipo de Solicitação</b></label>
                                            <asp:DropDownList ID="ddlidTipo" class="form-select" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidTipo_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div runat="server" id="divFormulario_Solicitacao" visible="false" class="row">
                                        <div runat="server" id="div_Solicitacao" class="col-12 row">
                                            <div class="col-lg-3" runat="server" id="div_dtInicio">
                                                <div class="form-group">
                                                    <label><b runat="server" id="lblDtInicial">Data Inicial</b></label>
                                                    <asp:TextBox ID="txtsdtInicio" class="form-control" type="date" runat="server" onchange="calcularDiasFerias()" onkeyup="calcularDiasFerias()"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-3" runat="server" id="div_dtFinal_Form">
                                                <div class="form-group">
                                                    <label><b runat="server" id="lblDtFim">Data Final Férias</b></label>

                                                    <div class="input-group">
                                                        <asp:TextBox ID="txtsdtFinal" class="form-control" type="date" runat="server" onchange="calcularDiasFerias()" onkeyup="calcularDiasFerias()"></asp:TextBox>
                                                        <asp:LinkButton ID="btnOpenModalDatas" runat="server" CssClass="btn btn-primary" OnClick="btnOpenModalDatas_Click" ToolTip="Adicionar novas opções de datas">
                                                            <i class="fa fa-plus"></i>
                                                        </asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>
                                            <%--   <div class="col-lg-3" runat="server" id="div_dtFinal_Form">
                                                <div class="form-group">
                                                    <label><b runat="server" id="lblDtFim">Data Final Férias</b></label>
                                                    <asp:TextBox ID="txtsdtFinal" class="form-control" type="date" runat="server"></asp:TextBox>
                                                </div>
                                            </div>--%>

                                            <div class="col-lg-12" runat="server" id="div_OpcoesDatas" visible="false">
                                                <label><b>Opções de Datas Adicionais</b></label>
                                                <div class="card p-2 bg-light">
                                                    <asp:Repeater ID="rptOpcoesDatas" runat="server" OnItemCommand="rptOpcoesDatas_ItemCommand" OnItemDataBound="rptOpcoesDatas_ItemDataBound">
                                                        <ItemTemplate>
                                                            <div class="d-flex justify-content-between align-items-center mb-1 border-bottom pb-1">
                                                                <span class="small">
                                                                    <i class="fa fa-calendar-alt text-primary"></i>
                                                                    De <b><%# Eval("DtInicio", "{0:dd/MM/yyyy}") %></b> até <b><%# Eval("DtFinal", "{0:dd/MM/yyyy}") %></b>
                                                                </span>
                                                                <asp:LinkButton ID="btnRemoverData" runat="server" CommandName="Remover" CommandArgument='<%# Container.ItemIndex %>' CssClass="btn btn-sm btn-outline-danger" ToolTip="Remover opção">
                        <i class="fa fa-times"></i>
                    </asp:LinkButton>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                    <div runat="server" id="divSemDatas" visible="false" class="text-muted small font-italic">Nenhuma data extra adicionada.</div>
                                                </div>
                                            </div>

                                            <div id="modal-datas-opcoes" class="modal fade" tabindex="-1" role="dialog">
                                                <div class="modal-dialog modal-sm" role="document">
                                                    <div class="modal-content">
                                                        <div class="modal-header">
                                                            <h5 class="modal-title">Adicionar Opção</h5>
                                                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                                        </div>
                                                        <div class="modal-body">
                                                            <div class="form-group mb-2">
                                                                <label>Início</label>
                                                                <asp:TextBox ID="txtModalDtInicio" type="date" CssClass="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                            <div class="form-group">
                                                                <label>Fim</label>
                                                                <asp:TextBox ID="txtModalDtFim" type="date" CssClass="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                            <div class="text-danger small mt-2">
                                                                <asp:Literal ID="litErroModalData" runat="server"></asp:Literal>
                                                            </div>
                                                        </div>
                                                        <div class="modal-footer">
                                                            <asp:Button ID="btnAdicionarDataLista" runat="server" Text="Adicionar" CssClass="btn btn-success btn-sm" OnClick="btnAdicionarDataLista_Click" />
                                                            <button type="button" class="btn btn-secondary btn-sm" data-bs-dismiss="modal">Fechar</button>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                        <div class="col-lg-12" runat="server" id="div_sDscSolicitacao">
                                            <div class="form-group">
                                                <label><b>Titulo da Solicitação</b></label>
                                                <asp:TextBox ID="txtsDscSolicitacao" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-12" runat="server" id="div1">
                                            <div class="form-group">
                                                <label><b>Descrição da Solicitação</b></label>
                                                <asp:TextBox ID="txtsObservacao" TextMode="multiline" Height="150px" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-lg-12">
                                        <fieldset class="form-stacked actions">
                                            <asp:Button ID="cmdSalvar" class="btn btn-md btn-success" runat="server" Text="Salvar" ValidationGroup="DETALHE" />
                                            <asp:Button ID="btnFechar" class="btn btn-md btn-warning" runat="server" Text="Voltar para a Lista" OnClick="fechar_click" />
                                        </fieldset>
                                        <div id="dialog-Salvar" class="modal" title="Salvar">
                                            <p>
                                                <asp:Label ID="lblTituloSalvar" runat="server" Text="Deseja salvar a Nova Solicitação?"></asp:Label>
                                            </p>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <br />
                        <div style="display: none;">
                            <asp:Button ID="btnAtualizarViaUpload" runat="server" OnClick="btnAtualizarViaUpload_Click" />
                        </div>
                        <div runat="server" id="divFormArquivo" class="card" visible="false">
                            <div class="card-body">
                                <h4>Anexos</h4>
                                <div id="divArquivos" runat="server">
                                    <div runat="server" id="div_arquivos_form" style="position: relative;">

                                        <div id="loadingMaskArquivo" class="loader-mask">
                                            <div class="loader-spinner"></div>
                                            <h4 style="margin-left: 15px; color: #555;">Carregando visualização...</h4>
                                        </div>

                                        <div class="embed-responsive embed-responsive-16by9" style="min-height: 200px">
                                            <embed type="text/html" runat="server" id="frmArquivos" width="100%" height="550" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <asp:HiddenField ID="hddidStatus" runat="server" />
            <asp:HiddenField ID="hddsStatus" runat="server" />

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

            <script type="text/javascript">

                // Esta função será chamada de dentro do Iframe (Arquivos.aspx)
                function AtualizarPaginaPai() {
                    // Simula o clique no botão do servidor
                    var btn = document.getElementById('<%= btnAtualizarViaUpload.ClientID %>');
                    if (btn) {
                        btn.click();
                    }
                }

                function toggleHistory(btn) {
                    var container = $(btn).closest('.card-body').find('.historico-container');
                    container.slideToggle();
                    if ($(btn).text().includes("Ver")) {
                        $(btn).html('<i class="fa fa-history me-1"></i>Ocultar Histórico');
                    } else {
                        $(btn).html('<i class="fa fa-history me-1"></i>Ver Histórico');
                    }
                }

                function toggleObservacao(btn) {
                    var container = $(btn).closest('.card-body').find('.observacao-container');
                    container.slideToggle();
                    if ($(btn).text().includes("Ver")) {
                        $(btn).html('<i class="fa fa-file-alt me-1"></i>Ocultar Descrição');
                    } else {
                        $(btn).html('<i class="fa fa-file-alt me-1"></i>Ver Descrição');
                    }
                }

                // --- FUNÇÃO DE CÁLCULO DE DIAS ---
                function calcularDiasFerias() {
                    var idIni = '<%= txtsdtInicio.ClientID %>';
                    var idFim = '<%= txtsdtFinal.ClientID %>';

                    var elIni = document.getElementById(idIni);
                    var elFim = document.getElementById(idFim);
                    var divMostra = document.getElementById('divCalcDias');
                    var spanValor = document.getElementById('lblTotalDiasValor');

                    // Verifica se os elementos existem (para não dar erro em outros tipos de solicitação)
                    if (elIni && elFim && divMostra) {

                        // Só mostra se ambas as datas estiverem preenchidas
                        if (elIni.value && elFim.value) {

                            // Força o horário T00:00:00 para evitar problemas de fuso horário no cálculo
                            var dt1 = new Date(elIni.value + 'T00:00:00');
                            var dt2 = new Date(elFim.value + 'T00:00:00');

                            // Diferença em milissegundos
                            var diffTime = dt2 - dt1;

                            // Converte para dias
                            var diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

                            // Soma +1 (Inclusivo)
                            var totalDias = diffDays + 1;

                            if (totalDias > 0) {
                                spanValor.innerText = totalDias;
                                // Usa display flex para manter o alinhamento do bootstrap
                                divMostra.style.display = 'block';
                            } else {
                                divMostra.style.display = 'none';
                            }
                        } else {
                            divMostra.style.display = 'none';
                        }
                    }
                }
            </script>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script type="text/javascript">

        // Função de inicialização robusta
        function initCKEditor() {
            // 1. Verifica se o script já foi carregado
            if (typeof CKEDITOR === 'undefined') {
                // Se não, tenta de novo em 100ms
                setTimeout(initCKEditor, 100);
                return;
            }

            // 2. Se o script existe, vamos inicializar
            var editorId = '<%= txtsObservacao.ClientID %>';
            var elemento = document.getElementById(editorId);

            // Se o elemento não estiver na página (ex: div oculta), não faz nada
            if (!elemento) {
                return;
            }

            try {
                // Remove a instância antiga se ela existir (após um postback)
                if (CKEDITOR.instances[editorId]) {
                    CKEDITOR.instances[editorId].destroy(true);
                }

                // 1. Verifica se o controle ASP.NET está como ReadOnly ou Disabled
                var estaBloqueado = elemento.hasAttribute('readonly') || elemento.hasAttribute('disabled');

                // Cria a nova instância
                // Como é Bootstrap 5, o tema 'bootstrapck' pode ficar bom
                CKEDITOR.replace(editorId, {
                    height: '250px',
                    versionCheck: false,
                    readOnly: estaBloqueado,

                    // 1. Impede que o CKEditor remova tags HTML (tabelas, cores, etc)
                    allowedContent: true,

                    // 2. Remove a barra de ferramentas se estiver bloqueado (fica mais limpo)
                    toolbar: estaBloqueado ? [] : null,

                    // 3. IMPORTANTE: Injeta o Bootstrap DENTRO do CKEditor para a tabela ficar bonita
                    // Ajuste o caminho abaixo para onde está o seu bootstrap.css ou use o link CDN para testar
                    contentsCss: [
                        'https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css',
            // Se tiver CSS customizado, adicione aqui também:
                      // '<%= ResolveUrl("~/Content/seuestilo.css") %>'
                    ],

                    // 4. Garante que o fundo fique com cor de "leitura" se bloqueado
                    on: {
                        instanceReady: function (evt) {
                            if (estaBloqueado) {
                                // Define o fundo do corpo do editor como cinza claro
                                evt.editor.document.getBody().setStyle('background-color', '#e9ecef');
                            }
                        }
                    }
                });
            } catch (e) {
                console.error('Falha ao inicializar o CKEditor:', e);
            }
        }

        // --- FUNÇÃO DO LOAD FAKE ---
        function iniciarLoaderArquivos() {
            var loader = document.getElementById('loadingMaskArquivo');

            // Só roda se o loader existir na tela (ou seja, se a div de arquivos estiver visível)
            if (loader) {
                // Garante que está visível ao iniciar (reset)
                loader.style.display = 'flex';

                // Espera 3 segundos (3000ms) e esconde suavemente
                setTimeout(function () {
                    $(loader).fadeOut('slow');
                }, 9000);
            }
        }


        function openModalDatas() {
            var myModalEl = document.getElementById('modal-datas-opcoes');

            // Se já houver uma instância, usa ela, senão cria nova
            var modal = bootstrap.Modal.getInstance(myModalEl);
            if (!modal) {
                modal = new bootstrap.Modal(myModalEl);
            }
            modal.show();
        }

        function closeModalDatas() {
            var myModalEl = document.getElementById('modal-datas-opcoes');

            // 1. Tenta fechar o modal suavemente via Bootstrap
            var modal = bootstrap.Modal.getInstance(myModalEl);
            if (modal) {
                modal.hide();
            } else {
                // Se a instância foi perdida no Postback, força um hide
                var newModal = new bootstrap.Modal(myModalEl);
                newModal.hide();
            }

            // 2. FORÇA BRUTA: Remove o Backdrop (sombra preta) manualmente
            // O UpdatePanel costuma deixar o backdrop órfão, então removemos na força
            var backdrops = document.getElementsByClassName('modal-backdrop');
            while (backdrops.length > 0) {
                backdrops[0].parentNode.removeChild(backdrops[0]);
            }

            // 3. Limpa a classe e estilo do body que trava a rolagem
            document.body.classList.remove('modal-open');
            document.body.style.overflow = '';
            document.body.style.paddingRight = '';
        }

        // GATILHO 1: Para o carregamento inicial da página
        $(document).ready(function () {
            initCKEditor();
            iniciarLoaderArquivos();

            setTimeout(calcularDiasFerias, 200);
        });

        // GATILHO 2: Para qualquer postback do UpdatePanel
        if (typeof Sys !== 'undefined') {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                initCKEditor();
                iniciarLoaderArquivos();

                setTimeout(calcularDiasFerias, 200);
            });
        }




</script>
</asp:Content>
