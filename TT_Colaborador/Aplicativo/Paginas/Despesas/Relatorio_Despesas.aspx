<%@ Page Title="Relatório de Gastos" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Relatorio_Despesas.aspx.cs" Inherits="TT_Colaborador.Aplicativo.Paginas.Despesas.Relatorio_Despesas" %>

<%@ Register Src="~/Aplicativo/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* ESTILOS ADAPTADOS DA TELA DE SOLICITAÇÕES */
        .solicitacao-card {
            border-left: 5px solid #007bff; /* Cor padrão, será sobrescrita pelo Eval */
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

        /* Estilos da Tabela de Gastos (Timeline adaptada) */
        .historico-container {
            padding-top: 1rem;
            margin-top: 1rem;
            border-top: 1px solid #e9ecef;
        }

            .historico-container table {
                margin-bottom: 0; /* Remover margem da tabela dentro do container */
            }

        /* Estilos da tabela interna de gastos */
        .gastos-tabela {
            border-collapse: collapse;
            width: 100%;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
            border-radius: 6px;
            overflow: hidden; /* Garante que o border-radius funcione no header */
        }

            .gastos-tabela th {
                background-color: #009A22;
                color: #fff;
                padding: 10px 12px;
                border: 1px solid #007a1b;
                font-size: 0.85rem;
            }

            .gastos-tabela td {
                padding: 10px 12px;
                border: 1px solid #ddd;
                color: #333;
                font-size: 0.9rem;
                background-color: #fdfdfd;
            }

            .gastos-tabela tr:nth-child(even) td {
                background-color: #f8f9fa;
            }

        ================================================
        CSS DA TIMELINE DE HISTÓRICO
        ================================================
        */
        .timeline-container-wrapper {
            display: none;
            margin-top: 1rem;
            border-top: 1px solid #e9ecef;
            padding-top: 1rem;
        }

        .timeline-container {
            position: relative;
            padding-left: 40px;
            list-style: none;
            margin-top: 10px;
        }

            .timeline-container::before { /* A linha vertical */
                content: '';
                position: absolute;
                left: 20px;
                top: 0;
                bottom: 0;
                width: 2px;
                background-color: #ddd;
            }

        .timeline-item {
            position: relative;
            margin-bottom: 20px;
        }

        .timeline-dot {
            position: absolute;
            left: -29px; /* Posição da "bolinha" */
            top: 5px;
            width: 18px;
            height: 18px;
            border-radius: 50%;
            background-color: #fff;
            border: 4px solid #007bff; /* Azul padrão */
        }

        /* Cores dinâmicas para a bolinha */
        .timeline-item.status-success .timeline-dot {
            border-color: #28a745;
        }

        .timeline-item.status-danger .timeline-dot {
            border-color: #dc3545;
        }

        .timeline-item.status-warning .timeline-dot {
            border-color: #ffc107;
        }

        .timeline-item.status-primary .timeline-dot {
            border-color: #007bff;
        }

        .timeline-content {
            background-color: #f9f9f9;
            padding: 15px;
            border-radius: 5px;
            border: 1px solid #eee;
        }

        .timeline-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 10px;
        }

        .timeline-user {
            font-weight: bold;
            font-size: 1.0em;
        }

        .timeline-date {
            font-size: 0.9em;
            color: #777;
        }

        .timeline-body {
            font-size: 1em;
        }

            /* Para renderizar o HTML do motivo */
            .timeline-body b {
                font-weight: bold;
            }

            .timeline-body span[style*="color:red"] {
                color: #dc3545 !important;
                text-decoration: line-through;
            }

            .timeline-body span[style*="color:green"] {
                color: #28a745 !important;
            }

        /* Ajuste no badge de status */
        .label {
            display: inline;
            padding: .2em .6em .3em;
            font-size: 75%;
            font-weight: 700;
            line-height: 1;
            color: #fff;
            text-align: center;
            white-space: nowrap;
            vertical-align: baseline;
            border-radius: .25em;
        }

        .label-success {
            background-color: #28a745;
        }

        .label-danger {
            background-color: #dc3545;
        }

        .label-warning {
            background-color: #ffc107;
        }

        .label-primary {
            background-color: #007bff;
        }
    </style>
</asp:Content>

<asp:Content ID="content_frmRelatorio_Despesas" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>
            <div id="hdd">
                <asp:HiddenField ID="hddidColaborador" runat="server" />
                <asp:HiddenField ID="hddidEntregaEPI" runat="server" />
            </div>

            <div class="card">
                <div class="card-header card-tt">
                    <h3 class="card-title" style="color: white"><b>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Relatório de Despesas"></asp:Label></b>
                    </h3>
                </div>
                <div class="card-body">
                    <div id="div_lista_relatorios" runat="server">

                        <div class="card mb-4">
                            <div class="card-header bg-light">
                                <h5 class="card-title mb-0"><i class="fa fa-filter me-2"></i>Filtros e Ações</h5>
                            </div>
                            <div class="card-body">
                                <div class="row align-items-end g-3">
                                    <div class="col-md-5">
                                        <label>Pesquisar por Motivo/Tipo/C. Custo</label>
                                        <asp:TextBox ID="txtStatus" CssClass="form-control" placeholder="Digite para pesquisar..." runat="server"></asp:TextBox>
                                    </div>
                                    <div class="col-md-4">
                                        <label>Status do Relatório</label>
                                        <asp:DropDownList ID="ddlsStatus" runat="server" class="form-select">
                                            <asp:ListItem Value="0">Selecione o Status</asp:ListItem>
                                            <asp:ListItem Value="1">Aberto</asp:ListItem>
                                            <asp:ListItem Value="2">Finalizado</asp:ListItem>
                                            <asp:ListItem Value="3">Aprovado</asp:ListItem>
                                            <asp:ListItem Value="4">Rejeitado</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-md-3 text-end">
                                        <asp:Button ID="cmdPesquisar" CssClass="btn btn-primary me-2" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                        <asp:Button ID="cmdNovo" CssClass="btn btn-success" runat="server" Text="Novo Relatório" OnClick="cmdNovo_Click" Visible="false" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Entregas" />

                        <asp:Repeater ID="rptRelatorio" runat="server" OnItemDataBound="rptRelatorio_ItemDataBound">
                            <ItemTemplate>
                                <div class='card solicitacao-card border-start border-4 border-<%# Eval("sCor") %>'>
                                    <div class="card-body">

                                        <div class="d-flex justify-content-between align-items-start">
                                            <div>
                                                <h5 class="card-title mb-1">Relatório #<%# Eval("idDespesas") %>
                                                    <small class="text-muted">- <%# Eval("sTipo") %></small>
                                                </h5>
                                                <p class="card-text text-muted mb-2">
                                                    <i class="fa fa-calendar me-1"></i>
                                                    Criado em: <%# Eval("dtInclusao") %> por <b><%# Eval("sDscUsuario") %></b>
                                                </p>
                                            </div>

                                            <span class='status-badge badge bg-<%# Eval("sCor") %>'>
                                                <asp:Literal runat="server" ID="litLido" Text='<%#  
                        Eval("idStatus") != null && Eval("idStatus").ToString() == "1" ? "Aberto" :
                        Eval("idStatus").ToString() == "2" ? "Finalizado" :
                        Eval("idStatus").ToString() == "3" ? "Aprovado" :
                        Eval("idStatus").ToString() == "5" ? "Excluido" :
                        Eval("idStatus").ToString() == "4" ? "Rejeitado" : "Aberto" %>' />
                                            </span>
                                        </div>

                                        <hr />

                                        <p class="card-text mb-1"><strong>Motivo:</strong> <%# Eval("sDscMotivo") %></p>
                                        <p class="card-text text-muted"><small><strong>Centro de Custo:</strong> <%# Eval("sDscPedido") %></small></p>

                                        <div class="d-flex justify-content-between align-items-center mt-3">
                                            <h4 class="mb-0 text-success">
                                                <strong>R$ <%# Eval("nValor") %></strong>
                                            </h4>

                                            <div class="text-end">
                                                <a href='Relatorio_Despesas_Detalhe.aspx?id=<%# Eval("idDespesas") %>'
                                                    class="btn btn-sm btn-outline-secondary me-2">
                                                    <i class="fa fa-eye me-1"></i>Ver Detalhes
                    </a>

                                                <button type="button"
                                                    id="btnVerGastos" runat="server"
                                                    class="btn btn-sm btn-outline-primary btn-historico me-2"
                                                    onclick="toggleGastos(this)">
                                                    <%-- ATENÇÃO: Renomeei a função JS --%>
                                                    <i class="fa fa-list me-1"></i>Ver Gastos
                   
                                                </button>

                                                <button type="button"
                                                    id="btnVerHistorico" runat="server"
                                                    class="btn btn-sm btn-outline-info btn-historico"
                                                    onclick="toggleTimeline(this)">
                                                    <%-- ATENÇÃO: Nova função JS --%>
                                                    <i class="fa fa-history me-1"></i>Ver Histórico
                   
                                                </button>
                                            </div>
                                        </div>

                                        <div class="historico-container table-responsive" style="display: none;">
                                            <div runat="server" id="divTabelaGastos">
                                                <table class="gastos-tabela">
                                                    <thead>
                                                        <tr>
                                                            <th>Recurso</th>
                                                            <th>Local</th>
                                                            <th>Forma de Pagamento</th>
                                                            <th>Participantes</th>
                                                            <th>Data</th>
                                                            <th>Valor (R$)</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater ID="rptGastos" runat="server">
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <td><%# Eval("sDscGasto") %></td>
                                                                    <td><%# Eval("sLocal") %></td>
                                                                    <td><%# Eval("sDscFormaPagamento") %></td>
                                                                    <td><%# Eval("sParticipantes") %></td>
                                                                    <td><%# Eval("dtDespesa") %></td>
                                                                    <td><%# Eval("nValor") %></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>

                                        <br />
                                        <hr />

                                        <div class="timeline-container-wrapper" style="display: none;">
                                            <h6 class="ms-3">Histórico de Status</h6>
                                            <ul class="timeline-container">
                                                <asp:Repeater ID="rptHistorico" runat="server">
                                                    <ItemTemplate>
                                                        <li class="timeline-item <%# GetStatusClass(Eval("sCor")) %>">
                                                            <div class="timeline-dot"></div>
                                                            <div class="timeline-content" style="background-color: #fff;">
                                                                <div class="timeline-header">
                                                                    <span class="timeline-user"><i class="fa fa-user"></i><%# Eval("sDscUsuario") %></span>
                                                                    <span class="timeline-date"><i class="fa fa-calendar"></i><%# Eval("dtStatusFormatada") %></span>
                                                                </div>
                                                                <div class="timeline-body">
                                                                    <strong>Status: <span class="label label-<%# Eval("sCor") %>"><%# Eval("sStatus") %></span></strong>
                                                                    <asp:Literal ID="litMotivo" runat="server" Text='<%# GetMotivo(Eval("sDscMotivo")) %>' />
                                                                </div>
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
                </div>
            </div>

            <script type="text/javascript">
                // Função existente (renomeada para clareza)
                function toggleGastos(btn) {
                    var container = $(btn).closest('.card-body').find('.historico-container'); // Este é o container da TABELA de gastos
                    container.slideToggle();
                    if ($(btn).text().includes("Ver")) {
                        $(btn).html('<i class="fa fa-list me-1"></i>Ocultar Gastos');
                    } else {
                        $(btn).html('<i class="fa fa-list me-1"></i>Ver Gastos');
                    }
                }

                // NOVA FUNÇÃO DA TIMELINE
                function toggleTimeline(btn) {
                    var container = $(btn).closest('.card-body').find('.timeline-container-wrapper'); // Este é o container da TIMELINE
                    container.slideToggle();
                    if ($(btn).text().includes("Ver")) {
                        $(btn).html('<i class="fa fa-history me-1"></i>Ocultar Histórico');
                    } else {
                        $(btn).html('<i class="fa fa-history me-1"></i>Ver Histórico');
                    }
                }
</script>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
