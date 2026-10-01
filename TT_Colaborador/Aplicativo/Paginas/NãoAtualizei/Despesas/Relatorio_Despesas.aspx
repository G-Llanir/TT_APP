<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Relatorio_Despesas.aspx.cs" Inherits="TT_Colaborador.Aplicativo.Paginas.Despesas.Relatorio_Despesas" %>

<%@ Register Src="~/Aplicativo/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmRelatorio_Despesas" ContentPlaceHolderID="MainContent" runat="server">

    <div id="DIV_ESPACO" class="h-100" runat="server" style="display: none;"></div>

    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>

            <style>
                .altura-scroll {
                    max-height: 600px;
                    overflow-y: scroll;
                }

                .efeito {
                    cursor: pointer;
                    text-decoration: none;
                    border-color: black;
                }

                    .efeito:hover {
                        background-color: #f0f0f0;
                        text-decoration: none;
                    }

                    .efeito.selecionado {
                        background-color: #2a8b01;
                        color: #fff;
                    }

                div .efeito .card:hover {
                    cursor: pointer;
                }

                .texto-resumido {
                    display: block;
                    white-space: nowrap;
                    overflow: hidden;
                    text-overflow: ellipsis;
                    max-width: 95%;
                }

                .corpo-mensagem {
                    color: darkgrey !important;
                    font-size: .85rem !important;
                    overflow: hidden;
                }

                .badge {
                    font: .85rem Rubik,sans-serif !important;
                    font-weight: bold !important;
                }

                /* Estilos para Dispositivos Móveis */
                @media only screen and (max-device-width: 768px) {

                    .altura-scroll {
                        max-height: 1200px !important;
                    }

                    .efeito {
                        border: 3px solid lawngreen;
                    }

                    .badge {
                        font: 1.5rem Rubik,sans-serif !important;
                        font-weight: bold !important;
                    }

                    .message-content b {
                        font-size: 2rem !important;
                    }

                    .message-content p {
                        font-size: 2rem !important;
                    }

                        .message-content p b {
                            font-size: 2rem !important;
                        }

                    .corpo-mensagem {
                        font-size: 1.25rem !important;
                    }

                    .corpo-heading-mensagem {
                        font-size: 1.75rem !important;
                    }
                }

                .table > tbody > tr > td.info, .table > tbody > tr.info > td,
                .table > tbody > tr > th.info, .table > tbody > tr.info > th {
                    background-color: #d9edf7 !important;
                }

                .table > tbody > tr > td.warning, .table > tbody > tr.warning > td,
                .table > tbody > tr > th.warning, .table > tbody > tr.warning > th {
                    background-color: #fcf8e3 !important;
                }

                .table > tbody > tr > td.success, .table > tbody > tr.success > td,
                .table > tbody > tr > th.success, .table > tbody > tr.success > th {
                    background-color: #dff0d8 !important;
                }

                .table > tbody > tr > td.danger, .table > tbody > tr.danger > td,
                .table > tbody > tr > th.danger, .table > tbody > tr.danger > th {
                    background-color: #f2dede !important;
                }
            </style>

            <div id="hdd">
                <asp:HiddenField ID="hddidColaborador" runat="server" />
                <asp:HiddenField ID="hddidEntregaEPI" runat="server" />
            </div>

            <div class="card">

                <div class="card-header card-tt">
                    <h3 class="card-title" style="color: white"><b>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Relatório de Gastos"></asp:Label></b>
                        <small class="text-muted"></small></h1>
                    </h3>
                </div>

                <div class="col-lg-12">
                    <div class="card">
                        <div class="card-header bg-primary text-white">
                            <h3 class="card-title"><i class="fa fa-bar-chart me-2"></i>Filtro para pesquisa </h3>
                        </div>

                        <div class="card-body row">
                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:TextBox ID="txtStatus" class="form-control" runat="server" placeholder="Pesquisar"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlsStatus" runat="server" class="form-select">
                                        <asp:ListItem Value="0">Selecione o Status</asp:ListItem>
                                        <asp:ListItem Value="1">Aberto</asp:ListItem>
                                        <asp:ListItem Value="2">Finalizado</asp:ListItem>
                                        <asp:ListItem Value="3">Aprovado</asp:ListItem>
                                        <asp:ListItem Value="4">Rejeitado</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-lg-2">
                                <fieldset class="form-stacked actions">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-md btn-primary" runat="server" Text="Pesquisar" />
                                    <asp:Button ID="cmdNovo" class="btn btn-md btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                                </fieldset>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-12" runat="server" id="div_Relatorios">
                    <div class="card">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Entregas" />
                        <div id="div_dados" runat="server" class="col-lg-12  altura-scroll">
                            <div class="row">
                                <asp:Repeater ID="rptRelatorio" runat="server" OnItemDataBound="rptRelatorio_ItemDataBound">
                                    <ItemTemplate>
                                        <div class="card-body">
                                            <div class="message-box">
                                                <div class="message-content">
                                                    <a id="resultado" class="efeito card" style="padding: 20px;" href="Relatorio_Despesas_Detalhe.aspx?id=<%# Eval("idDespesas") %>">
                                                        <label style="width: 100%;" class='<%# 
                                                                Eval("sCor") != null && Eval("sCor").ToString() == "warning" ? "badge bg-warning" :
                                                                Eval("sCor").ToString() == "primary" ? "badge bg-primary" :
                                                                Eval("sCor").ToString() == "success" ? "badge bg-success" :
                                                                Eval("sCor").ToString() == "danger" ? "badge bg-danger" : "badge bg-primary" %>'>
                                                            <asp:Literal runat="server" ID="litLido" Text='<%# 
                                                                    Eval("idStatus") != null && Eval("idStatus").ToString() == "1" ? "Aberto" :
                                                                    Eval("idStatus").ToString() == "2" ? "Finalizado" :
                                                                    Eval("idStatus").ToString() == "3" ? "Aprovado" :
                                                                    Eval("idStatus").ToString() == "5" ? "Excluido" :
                                                                    Eval("idStatus").ToString() == "4" ? "Rejeitado" : "Aberto" %>' />
                                                        </label>

                                                        <p>
                                                            <asp:Literal runat="server" Text='<%# string.Format("<b>Data do Relatório:</b> {0}", Eval("dtInclusao")) %>' /></p>

                                                        <p>
                                                            <asp:Literal runat="server" Text='<%# string.Format("<b>Colaborador:</b> {0}", Eval("sDscUsuario")) %>' /></p>

                                                        <p>
                                                            <asp:Literal runat="server" Text='<%# string.Format("<b>Tipo:</b> {0}", Eval("sTipo")) %>' /></p>

                                                        <p>
                                                            <asp:Literal runat="server" Text='<%# string.Format("<b>Centro de Custo:</b> {0}", Eval("sDscPedido")) %>' /></p>

                                                        <p>
                                                            <asp:Literal runat="server" Text='<%# string.Format("<b>Motivo:</b> {0}", Eval("sDscMotivo")) %>' /></p>

                                                        <p>
                                                            <asp:Literal runat="server" Text='<%# string.Format("<b>Valor Total (R$):</b> {0}", Eval("nValor")) %>' /></p>

                                                        <div class="corpo-heading-mensagem" runat="server" id="div_Gasto"><b>Gastos:</b></div>

                                                        <div runat="server" id="divTabelaGastos">
                                                            <table class="table-bordered corpo-mensagem" style="border-collapse: collapse; width: 100%; box-shadow: 0 2px 15px rgba(0, 0, 0, 0.15); border: 1px;">
                                                                <thead>
                                                                    <tr style="background-color: #009A22; color: #fff;">
                                                                        <th style="padding: 8px; border: 1px solid #000;">Recurso</th>
                                                                        <th style="padding: 8px; border: 1px solid #000;">Local</th>
                                                                        <th style="padding: 8px; border: 1px solid #000;">Forma de Pagamento</th>
                                                                        <th style="padding: 8px; border: 1px solid #000;">Participantes</th>
                                                                        <th style="padding: 8px; border: 1px solid #000;">Data</th>
                                                                        <th style="padding: 8px; border: 1px solid #000;">Valor (R$)</th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody>
                                                                    <asp:Repeater ID="rptGastos" runat="server">
                                                                        <ItemTemplate>
                                                                            <tr>
                                                                                <td style="padding: 8px; border: 1px solid #000; color: black;"><%# Eval("sDscGasto") %></td>
                                                                                <td style="padding: 8px; border: 1px solid #000; color: black;"><%# Eval("sLocal") %></td>
                                                                                <td style="padding: 8px; border: 1px solid #000; color: black;"><%# Eval("sDscFormaPagamento") %></td>
                                                                                <td style="padding: 8px; border: 1px solid #000; color: black;"><%# Eval("sParticipantes") %></td>
                                                                                <td style="padding: 8px; border: 1px solid #000; color: black;"><%# Eval("dtDespesa") %></td>
                                                                                <td style="padding: 8px; border: 1px solid #000; color: black;"><%# Eval("nValor") %></td>
                                                                            </tr>
                                                                        </ItemTemplate>
                                                                    </asp:Repeater>
                                                                </tbody>
                                                            </table>
                                                        </div>
                                                    </a>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
