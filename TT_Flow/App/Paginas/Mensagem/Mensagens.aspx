<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Mensagens.aspx.cs" Inherits="TT_Flow.App.Paginas.Mensagem.Mensagens" %> 

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <style>
        .mensagens-filtros .form-group label {
            font-size: 12px;
            color: #666;
            margin-bottom: 4px;
        }

        .mensagens-filtros .CaixaTextoGrande {
            width: 100%;
            max-width: 100%;
        }

        .mensagens-filtros .form-group {
            margin-bottom: 12px;
        }

        .mensagens-filtros-linha {
            margin-bottom: 4px;
        }

        .mensagens-filtros-linha::after {
            content: "";
            display: table;
            clear: both;
        }

        .mensagens-filtro-slot.is-inactive {
            display: none;
        }

        .mensagens-periodo-datas {
            display: flex;
            gap: 6px;
        }

            .mensagens-periodo-datas .form-control {
                width: 100%;
                min-width: 0;
            }

        .mensagens-dept-filtro + .chosen-container {
            width: 100% !important;
        }

        .mensagens-acoes {
            margin-top: 8px;
            padding-top: 16px;
            border-top: 1px solid #e5e5e5;
            clear: both;
        }

        .mensagens-acoes .btn {
            margin-right: 6px;
        }

        .mensagens-tabela .dataTables_filter input {
            min-width: 280px;
            margin-left: 8px;
        }

        .mensagens-status {
            display: inline-block;
            min-width: 70px;
            padding: 3px 8px;
            border-radius: 12px;
            font-size: 11px;
            text-align: center;
            background: #e8eef7;
            color: #2f5d8c;
        }

        .mensagens-status.nao-lida {
            background: #f8d7da;
            color: #a94442;
            font-weight: bold;
        }

        .mensagens-aviso {
            display: inline-block;
            padding: 3px 8px;
            border-radius: 12px;
            font-size: 11px;
            background: #fcf8e3;
            color: #8a6d3b;
        }

        .mensagens-tag {
            display: inline-block;
            margin: 1px 0 1px 4px;
            padding: 2px 7px;
            border-radius: 10px;
            background: #e8eef7;
            color: #2f5d8c;
            font-size: 11px;
        }

        .mensagens-item {
            display: grid;
            grid-template-columns: minmax(0, 1fr) auto;
            grid-template-rows: auto auto;
            gap: 10px 12px;
            grid-template-areas:
                "remetente data"
                "link tags";
            padding: 4px 0;
        }

        .mensagens-item-remetente {
            grid-area: remetente;
            font-weight: 700;
            font-size: 1.05em;
            line-height: 1.3;
            color: #333;
            word-break: break-word;
        }

            .mensagens-item-remetente i {
                margin-right: 6px;
                color: #337ab7;
            }

        .mensagens-item-data {
            grid-area: data;
            text-align: right;
            font-size: 0.82em;
            line-height: 1.3;
            color: #777;
            white-space: nowrap;
        }

            .mensagens-item-data i {
                margin-right: 4px;
                color: #999;
            }

        .mensagens-item-assunto {
            grid-area: link;
            align-self: end;
            display: block;
            font-size: 0.9em;
            line-height: 1.35;
            word-break: break-word;
        }

        .mensagens-item-tags {
            grid-area: tags;
            align-self: end;
            text-align: right;
            line-height: 1.4;
        }
    </style>
    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>

            <%--<script src="/app/js/vanilla-masker.js"></script>--%>

            <script type="text/javascript">

                function MudarBotao() {
                    var botao = document.querySelector('[id$="cmdPesquisar"]');
                    if (botao) botao.value = "Pesquisando...";
                }

                function Mensagens_PrepararPostback() {
                    MudarBotao();
                    var dept = $('.mensagens-dept-filtro');
                    if (dept.length) {
                        dept.trigger('chosen:updated');
                    }
                    return true;
                }

                function Mensagens_InicializarFiltros() {
                    $('[id*=txtdtEnvio], [id*=txtdtFinal]').datepicker({
                        autoclose: true,
                        format: 'dd/mm/yyyy',
                        language: 'pt-BR'
                    });

                    $('[id*=txtdtEnvio], [id*=txtdtFinal]').mask('99/99/9999');

                    var dept = $('.mensagens-dept-filtro');
                    if (dept.length && $.fn.chosen) {
                        if (dept.next('.chosen-container').length) {
                            dept.chosen('destroy');
                        }
                        dept.chosen({
                            width: '100%',
                            placeholder_text_multiple: 'Selecione departamentos...',
                            no_results_text: 'Nenhum departamento encontrado'
                        });
                    }
                }

                function Mensagens_NormalizarTabelaGrid(tabela) {
                    var thead = tabela.find('thead');
                    if (!thead.length) {
                        var linhaCabecalho = tabela.find('tr').has('th').first();
                        if (linhaCabecalho.length) {
                            thead = $('<thead></thead>').prependTo(tabela);
                            linhaCabecalho.appendTo(thead);
                        }
                    }

                    var totalColunas = tabela.find('thead tr:first th').length;
                    if (!totalColunas) {
                        return 0;
                    }

                    tabela.find('tbody tr').each(function () {
                        var linha = $(this);
                        var celulas = linha.children('td');

                        if (celulas.length === 1 && celulas.first().attr('colspan')) {
                            linha.remove();
                            return;
                        }

                        var totalCelulas = celulas.length;
                        while (totalCelulas < totalColunas) {
                            linha.append('<td>&nbsp;</td>');
                            totalCelulas++;
                        }

                        while (totalCelulas > totalColunas) {
                            linha.children('td:last').remove();
                            totalCelulas--;
                        }
                    });

                    return totalColunas;
                }

                function Mensagens_InicializarTabela() {
                    var tabela = $('#<%= dtgvConsulta.ClientID %>');
                    if (!tabela.length || !$.fn.DataTable) {
                        return;
                    }

                    if ($.fn.DataTable.isDataTable(tabela[0])) {
                        tabela.DataTable().clear().destroy();
                        tabela.removeClass('dataTable no-footer');
                    }

                    var wrapper = tabela.parent('.dataTables_wrapper');
                    if (wrapper.length) {
                        tabela.insertBefore(wrapper);
                        wrapper.remove();
                    }

                    tabela.find('tbody tr.child').remove();

                    var totalColunas = Mensagens_NormalizarTabelaGrid(tabela);
                    if (!totalColunas) {
                        return;
                    }

                    var colunaSelecaoVisivel = $('#<%= hddExibirSelecaoMensagens.ClientID %>').val() === '1';
                    var colunaOrdenacaoInicial = totalColunas > 1 ? 1 : 0;
                    var columnDefs = [];

                    columnDefs.push({
                        targets: 0,
                        orderable: false,
                        searchable: false,
                        visible: colunaSelecaoVisivel
                    });

                    var colunasCentralizadas = [];
                    if (totalColunas >= 2) {
                        colunasCentralizadas.push(totalColunas - 2);
                    }
                    if (totalColunas >= 1) {
                        colunasCentralizadas.push(totalColunas - 1);
                    }

                    colunasCentralizadas = colunasCentralizadas.filter(function (indice) {
                        return indice >= 0 && indice < totalColunas;
                    });

                    if (colunasCentralizadas.length) {
                        columnDefs.push({ targets: colunasCentralizadas, className: 'text-center' });
                    }

                    try {
                        tabela.DataTable({
                            responsive: false,
                            deferRender: true,
                            paging: true,
                            searching: true,
                            ordering: true,
                            info: true,
                            autoWidth: false,
                            pageLength: 25,
                            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
                            order: [[colunaOrdenacaoInicial, 'desc']],
                            columnDefs: columnDefs,
                            dom: "<'row'<'col-sm-12'l>>" +
                                 "<'row'<'col-sm-12'tr>>" +
                                 "<'row'<'col-sm-5'i><'col-sm-7'p>>",
                            language: { url: 'https://cdn.datatables.net/plug-ins/1.11.5/i18n/pt-BR.json' }
                        });

                        $('#txtPesquisaTabelaMensagens').off('keyup change').on('keyup change', function () {
                            tabela.DataTable().search(this.value).draw();
                        });
                    } catch (ex) {
                        if (window.console && console.warn) {
                            console.warn('Mensagens DataTable:', ex);
                        }
                    }
                }

                function pageLoad() {
                    Mensagens_InicializarFiltros();
                    Mensagens_InicializarTabela();
                }

            </script>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Mensagens"></asp:Label><small> Internas</small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">

                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                        </div>

                        <div class="panel-body">
                            <div class="mensagens-filtros">

                                <div class="row mensagens-filtros-linha">
                                    <div class="col-lg-3 col-md-6 col-sm-12">
                                        <div class="form-group">
                                            <label>Período</label>
                                            <div class="mensagens-periodo-datas">
                                                <asp:TextBox ID="txtdtEnvio" class="form-control CaixaTextoData" placeholder="Data Inicial" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                                <asp:TextBox ID="txtdtFinal" class="form-control CaixaTextoData" placeholder="Data Final" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-3 col-md-6 col-sm-12">
                                        <div class="form-group">
                                            <label>Caixa</label>
                                            <asp:DropDownList ID="ddlsDirecaoMensagem" class="form-control" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlTipoCaixa_SelectedIndexChanged">
                                                <asp:ListItem Text="Recebidas" Value="Recebida" />
                                                <asp:ListItem Text="Enviadas" Value="Enviada" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-3 col-md-6 col-sm-12">
                                        <div class="form-group">
                                            <label>Status</label>
                                            <asp:DropDownList ID="ddlidFiltroMensagem" class="form-control" runat="server" OnSelectedIndexChanged="ddlidFiltroMensagem_SelectedIndexChanged" AutoPostBack="True">
                                                <asp:ListItem Text="Todas as Mensagens" Value="1" />
                                                <asp:ListItem Text="Não Lidas" Value="2" />
                                                <asp:ListItem Text="Lidas" Value="3" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-3 col-md-6 col-sm-12">
                                        <div class="form-group">
                                            <label>Tipo/Categoria</label>
                                            <asp:DropDownList ID="ddlCategoriaMensagem" class="form-control" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCategoriaMensagem_SelectedIndexChanged">
                                                <asp:ListItem Text="Todos os Tipos" Value="TODOS" Selected="True" />
                                                <asp:ListItem Text="Avisos" Value="AVISO" />
                                                <asp:ListItem Text="Mensagens" Value="MENSAGEM" />
                                                <asp:ListItem Text="E-mails do T-Flow" Value="EMAIL" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div class="row mensagens-filtros-linha">
                                    <asp:Panel ID="pnlTiposEmail" runat="server" CssClass="col-lg-3 col-md-6 col-sm-12 mensagens-filtro-slot is-inactive">
                                        <div class="form-group">
                                            <label>Tipos de e-mail</label>
                                            <asp:DropDownList ID="ddlTiposEmail" class="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                    </asp:Panel>

                                    <asp:Panel ID="pnlFiltroDepartamentos" runat="server" CssClass="col-lg-7 col-md-8 col-sm-12">
                                        <div class="form-group">
                                            <label>Departamentos</label>
                                            <asp:ListBox ID="ddlidDepartamento" runat="server" CssClass="form-control mensagens-dept-filtro" SelectionMode="Multiple" Rows="1" data-placeholder="Selecione departamentos..."></asp:ListBox>
                                        </div>
                                    </asp:Panel>

                                    <asp:Panel ID="pnlFiltroAssunto" runat="server" CssClass="col-lg-5 col-md-12 col-sm-12">
                                        <div class="form-group">
                                            <label>Assunto</label>
                                            <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar por assunto..." runat="server"></asp:TextBox>
                                        </div>
                                    </asp:Panel>
                                </div>

                                <div class="row mensagens-filtros-linha">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <div class="form-group">
                                            <label>Pesquisar na tabela</label>
                                            <input type="text" id="txtPesquisaTabelaMensagens" class="form-control" placeholder="Filtrar resultados exibidos..." />
                                        </div>
                                    </div>
                                </div>

                                <div class="row mensagens-filtros-linha">
                                    <div class="col-lg-12">
                                        <div class="form-group mensagens-acoes">
                                            <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="return Mensagens_PrepararPostback();" />
                                            <asp:Button ID="cmdLimpar" class="btn btn-default" runat="server" Text="Limpar" OnClick="cmdLimpar_Click" />
                                            <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                                        </div>
                                    </div>
                                </div>

                                <asp:HiddenField ID="hdddtLeitura" runat="server" />
                                <asp:HiddenField ID="hddidRegistro" runat="server" />
                                <asp:HiddenField ID="hddExibirSelecaoMensagens" runat="server" Value="0" />

                            </div>
                        </div>

                    </div>
                </div>


                <div class="col-lg-12">
                    <asp:Panel ID="pnResultado" class="" runat="server">
                        <div id="resultado">
                            <div class="panel panel-primary">
                                <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                                <div class="panel-body">
                                    <div class="table-responsive mensagens-tabela">
                                        <asp:GridView ID="dtgvConsulta" DataKeyNames="idMensagem" class="table table-condensed table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">

                                            <Columns>

                                                <asp:TemplateField HeaderText="">
                                                    <HeaderTemplate>
                                                        <asp:CheckBox
                                                            ID="chkConversas_SelecionaTodos" runat="server" AutoPostBack="true" OnCheckedChanged="chkConversas_SelecionaTodos_CheckedChanged" />
                                                    </HeaderTemplate>
                                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="2%" />

                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkConversas_Seleciona" runat="server" />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="2%" />
                                                </asp:TemplateField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idMensagem"
                                                    DataTextField="idMensagem" HeaderText="ID"
                                                    DataNavigateUrlFormatString="Mensagens_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="6%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:TemplateField HeaderText="Mensagem">
                                                    <ItemTemplate>
                                                        <div class="mensagens-item">
                                                            <div class="mensagens-item-remetente">
                                                                <i class="fa fa-user" aria-hidden="true"></i>
                                                                <%# Eval("sDscUsuarioRemetente") %>
                                                            </div>
                                                            <div class="mensagens-item-data">
                                                                <i class="fa fa-calendar" aria-hidden="true"></i>
                                                                <%# Eval("dtInclusao") %>
                                                            </div>
                                                            <asp:HyperLink ID="lnkAssunto" runat="server"
                                                                CssClass="mensagens-item-assunto"
                                                                NavigateUrl='<%# "Mensagens_Detalhe.aspx?id=" + Eval("idMensagem") %>'
                                                                Text='<%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("sAssunto"))) ? "(Sem assunto)" : Eval("sAssunto") %>' />
                                                            <asp:Panel ID="pnTagsMensagem" runat="server" CssClass="mensagens-item-tags">
                                                                <asp:Repeater ID="rptTagsMensagem" runat="server">
                                                                    <ItemTemplate>
                                                                        <span class="mensagens-tag"><%# Container.DataItem %></span>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </asp:Panel>
                                                        </div>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="39%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" />
                                                </asp:TemplateField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idMensagem"
                                                    DataTextField="sDestinatario" HeaderText="Destinatário"
                                                    DataNavigateUrlFormatString="Mensagens_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="20%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:TemplateField HeaderText="Aviso">
                                                    <ItemTemplate>
                                                        <span class='<%# Convert.ToString(Eval("sIsAviso")) == "S" ? "mensagens-aviso" : "" %>'>
                                                            <%# Convert.ToString(Eval("sIsAviso")) == "S" ? "Sim" : "Não" %>
                                                        </span>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="7%" />
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Leitura">
                                                    <ItemTemplate>
                                                        <span class='<%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("dtLeitura"))) ? "mensagens-status nao-lida" : "mensagens-status" %>'>
                                                            <%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("dtLeitura"))) ? "Não lida" : "Lida" %>
                                                        </span>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="9%" />
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>


                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                    <asp:Panel ID="pnlConversasLidos" CssClass="form-stacked actions" runat="server">
                                        <asp:Button ID="cmdConversas_Lidos" class="btn  btn-sm btn-danger" runat="server" Text="Marcar itens selecionados como lido" OnClick="cmdConversas_Lidos_Click" />
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>

                    </asp:Panel>

                    <asp:Panel ID="pnMensagem" runat="server">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />

                    </asp:Panel>
                </div>
            </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
