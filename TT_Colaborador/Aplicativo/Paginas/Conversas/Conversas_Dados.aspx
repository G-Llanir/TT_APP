<%@ Page Title="" Language="C#" MasterPageFile="~/site.master" AutoEventWireup="true" CodeBehind="Conversas_Dados.aspx.cs" Inherits="TT_Colaborador.Aplicativo.Paginas.Conversas.Conversas_Dados" %>

<%@ Register Src="~/Aplicativo/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="content_frmConversas_Dados" ContentPlaceHolderID="MainContent" runat="server">
    <div id="DIV_ESPACO" class="h-100" runat="server" style="display: none;"></div>

    <style>
        .chat-messages {
            display: flex;
            flex-direction: column;
            max-height: 400px;
            overflow-y: scroll
        }

        .chat-message-left,
        .chat-message-right {
            display: flex;
            flex-shrink: 0
        }

        .chat-message-left {
            margin-right: auto
        }

        .chat-message-right {
            flex-direction: row-reverse;
            margin-left: auto
        }

        .py-3 {
            padding-top: 1rem !important;
            padding-bottom: 1rem !important;
        }

        .px-4 {
            padding-right: 1.5rem !important;
            padding-left: 1.5rem !important;
        }

        .altura-scroll {
            width: 100%;
            max-height: 600px;
            overflow-y: scroll;
            padding-right: 5px;
        }

        .altura-scroll-chat {
            min-height: 350px;
            overflow-y: auto;
        }

        .texto-resumido {
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
            max-width: 95%;
        }

        p {
            margin: 0;
        }

        .mensagem {
            display: flex;
            flex-wrap: wrap;
            width: 100%;
            height: 100%;
            border: 1px solid lightgrey;
            border-right: 0;
            border-left: 0;
            margin-bottom: 15px;
            padding: 5px;
        }

            .mensagem .dados-mensagem {
                width: 85%;
                height: 100%;
            }

                .mensagem .dados-mensagem .assunto-mensagem {
                    color: black;
                    font: 1rem bold;
                    width: 100%;
                    height: 50%;
                }

                .mensagem .dados-mensagem .texto-mensagem {
                    color: darkgrey !important;
                    font: .75rem !important;
                    width: 100%;
                    height: 50%;
                }

            .mensagem .btn-mensagem {
                width: 15%;
                height: 100%;
                align-self: center;
            }

        /*----------------------------------------------------------------*/

        /* Estilos para Dispositivos Móveis */

        @media only screen and (max-device-width: 768px) {

            .mensagem .dados-mensagem .assunto-mensagem {
                color: black;
                font: 1.75rem bold;
                width: 100%;
                height: 50%;
            }

            .mensagem .dados-mensagem .texto-mensagem {
                color: grey !important;
                font: 1.5rem !important;
                width: 100%;
                height: 50%;
            }
        }

        /*----------------------------------------------------------------*/
    </style>

    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <div id="hdd">
                <asp:HiddenField ID="hddConversa_idRegistroConvesa" runat="server" />
                <asp:HiddenField ID="hddidStatus" runat="server" />
                <asp:HiddenField ID="hddidUsuarioAtualizacao" runat="server" />
                <asp:HiddenField ID="hddidRegistroConvesa" runat="server" />
                <asp:HiddenField ID="hddidColaborador" runat="server" />
            </div>

            <div class="col-lg-12">
                <div class="card">
                    <div class="card-header color-card-header card-tt">
                        <h3 class="card-title"><b><asp:Label ID="lblTituloPagina" runat="server"></asp:Label></b></h3>
                    </div>
                    <div class="row">

                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaConversas" />
                        </div>

                        <div class="col-lg-3">
                            <div id="divMostrar" runat="server" class="card card-box ribbon-box">
                                <div>
                                    <div class="ribbon ribbon-primary">Conversas em Aberto</div>
                                    <asp:Button ID="cmdNovaConversa" class="btn btn-sm btn-success" Style="position: relative; float: right;" runat="server" Text="Nova Conversa" OnClick="cmdNovaConversa_Click" />
                                </div>
                                <div class="altura-scroll">

                                    <asp:Repeater ID="rptConversas" runat="server">
                                        <ItemTemplate>
                                            <div class="mensagem">

                                                <div class="dados-mensagem">

                                                    <p class="assunto-mensagem">
                                                        <asp:Literal runat="server" Text='<%# Eval("sDscStatusConversa") %>'></asp:Literal>
                                                    </p>
                                                    <p class="texto-mensagem texto-resumido">
                                                        <asp:Literal runat="server" Text='<%# Eval("sDscConversa") %>'></asp:Literal>
                                                    </p>

                                                </div>

                                                <div class="btn-mensagem">
                                                    <p>
                                                        <asp:LinkButton ID="lnkConversasEditar" runat="server" ToolTip="Ver Detalhes" class="btn btn-icon btn-sm waves-effect waves-light btn-danger" TabIndex="100" CommandName="VerDetalhes" OnClick="lnkConversasEditar_Click" CommandArgument='<%# Eval("idRegistroConvesa") %>'>
                                                           <i class="fa fa-eye"></i><span class="badge bg-danger text-white"><%# Eval("nQtdConversasPendentes") %></span>
                                                        </asp:LinkButton>
                                                    </p>
                                                </div>

                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                </div>
                            </div>
                        </div>

                        <div class="col-lg-9">

                            <div runat="server" id="Div_Conversas">
                                <div class="card card-body" runat="server" id="DIV_CONVERSA">
                                    <div class="form-group row">
                                        <div runat="server" id="Div_Inclusao">
                                            <div class="form-group row">
                                                <div class="col-lg-4">
                                                    <div class="form-group">
                                                        <b>
                                                            <label for="ddlConversas_idTipoEvento">Tipos de Conversas</label></b>

                                                        <asp:DropDownList ID="ddlConversas_idTipoEvento" class="form-select" runat="server"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-lg-8">
                                                    <div class="form-group">
                                                        <b>
                                                            <label for="txtConversas_sDscConversa">Assunto</label></b>
                                                        <asp:TextBox ID="txtConversas_sDscConversa" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                    <br />
                                    <div id="DIV_Historico" class="historico">

                                        <div class="position-relative">
                                            <div class="chat-messages p-4 altura-scroll-chat">
                                                <asp:Repeater ID="rptHistorico" runat="server">
                                                    <ItemTemplate>

                                                        <div class="chat-message-<%# GetLiClass(Eval("idUsuarioAtualizacao")) %> pb-4">
                                                            <div>
                                                                <img src="<%# GetLiImagemColaborador(Eval("imgColaborador")) %>" class="rounded-circle mr-1" width="40" height="40">
                                                            </div>
                                                            <div class="flex-shrink-1 bg-light rounded py-2 px-3 mr-3">
                                                                <div class="font-weight-bold mb-1">
                                                                    <%# Eval("sDscUsuario") %>
                                                                </div>
                                                                <div>
                                                                    <pre style="white-space: pre-line">
                                                                                          <%# Eval("sObservacaoConversa") %>
                                                                                        </pre>
                                                                </div>
                                                                <div class="text-muted small text-nowrap mt-2"><%# Eval("dtAtualizacao", "{0:dd/MM/yyyy HH:mm:ss}") %></div>
                                                            </div>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </div>
                                        </div>
                                        <div runat="server" id="div_EditarConversa" class="card-footer">
                                            <div class="input-group">
                                                <asp:TextBox ID="txtConversas_sObservacaoConversa" class="form-control" runat="server" MaxLength="2000" TextMode="MultiLine" Height="80px"></asp:TextBox>

                                                <asp:Button ID="cmdConversas_Salvar" class="btn btn-success btn-sm" Style="margin-left: 2px" runat="server" ValidationGroup="Item" Text="Enviar" OnClick="cmdConversas_Salvar_Click" />
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>

                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
