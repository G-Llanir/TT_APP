<%@ Page Title="" Language="C#" MasterPageFile="~/site.master" AutoEventWireup="true" CodeBehind="Mensagens.aspx.cs" Inherits="TT_Colaborador.Aplicativo.Paginas.Mensagem.Mensagens" %>

<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .altura-scroll {
            max-height: 600px;
            overflow-y: scroll;
        }

        .efeito {
            cursor: pointer;
            text-decoration: none;
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

        /*----------------------------------------------------------------*/

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

        /*----------------------------------------------------------------*/
    </style>

    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>

            <div class="card">
                <div class="card-header card-tt">
                    <h3 class="card-title"><b><asp:Label ID="lblTituloPagina" runat="server"></asp:Label></b></h3>
                </div>
                <div class="card-body">
                    <div class="row">

                        <div class="col-lg-4">

                            <div class="card">
                                <div class="card-header bg-primary text-white">
                                    <h3 class="card-title"><i class="fa fa-bar-chart me-2"></i>Filtro para pesquisa </h3>
                                </div>

                                <div class="card-body">

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <asp:DropDownList ID="ddlsDirecaoMensagem" CssClass="form-select" runat="server">
                                                <asp:ListItem Text="Apenas Recebidas" Value="Recebida" />
                                                <asp:ListItem Text="Apenas Enviadas" Value="Enviada" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <hr />

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <asp:DropDownList ID="ddlidFiltroMensagem" CssClass="form-select" runat="server">
                                                <asp:ListItem Text="Todos" Value="1" />
                                                <asp:ListItem Text="Apenas Não Lidas" Value="2" />
                                                <asp:ListItem Text="Apenas Lidas" Value="3" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <hr />

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <asp:DropDownList ID="ddlAvisos" CssClass="form-select" runat="server">
                                                <asp:ListItem Text="Todos" Value="T" />
                                                <asp:ListItem Text="Apenas Mensagens" Value="N" />
                                                <asp:ListItem Text="Apenas Avisos" Value="S" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <hr />

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <asp:DropDownList ID="ddlidTipoObjeto" CssClass="form-select Caixa_Selecao" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <hr />

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <asp:DropDownList ID="ddlidDepartamento" CssClass="form-select CaixaTextoGrande Caixa_Selecao" runat="server" attrname="idDepartamento"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <hr />

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <asp:TextBox ID="txtPesquisa" CssClass="form-control CaixaTextoGrande" placeholder="Pesquisar Avisos" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <hr />

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <asp:Button ID="cmdPesquisar" CssClass="btn btn-md btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                            <asp:Button ID="cmdNovo" CssClass="btn btn-md btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                                        </div>
                                    </div>

                                    <asp:HiddenField ID="hdddtLeitura" runat="server" />
                                    <asp:HiddenField ID="hddidRegistro" runat="server" />

                                </div>
                            </div>
                        </div>

                        <div class="col-lg-8">

                            <uc1:MensagemPagina runat="server" ID="MensagemPagina2" />

                            <div id="divAviso" runat="server" class="card">
                                <div class="card-header card-tt">
                                    <h3 class="card-title">
                                        <b>
                                            <asp:Label ID="lblTituloCard" runat="server" Text="Visualização"></asp:Label>
                                            <span id="spanQtdLidos" runat="server" class="badge bg-primary"></span>

                                            <span id="spanQtdNaoLidos" runat="server" class="badge bg-danger"></span>
                                        </b>
                                    </h3>
                                </div>

                                <div class="altura-scroll">

                                    <asp:Repeater ID="rptMessages" runat="server" OnItemDataBound="rptMessages_ItemDataBound">
                                        <ItemTemplate>

                                            <div class="card-body">
                                                <div class="message-box">

                                                        <div class="message-content">
                                                    <a id="resultado" class="efeito card" style="padding: 20px;" href="Mensagens_Detalhe.aspx?id=<%# Eval("idMensagem") %>&sIsAviso=S">

                                                            <label class='<%# string.IsNullOrEmpty(Eval("dtLeitura").ToString()) ? "badge bg-danger" : "badge bg-primary" %>' style="width: 100%;">
                                                                <asp:Literal runat="server" ID="litLido" Text='<%# string.IsNullOrEmpty(Eval("dtLeitura").ToString()) ? "Não Lido" : "Lido" %>' />
                                                            </label>

                                                            <p>
                                                                <b class="texto-resumido">Assunto: <asp:Literal runat="server" ID="litAssunto" Text='<%# Eval("sAssunto") %>' /></b>
                                                            </p>

                                                            <p>
                                                                <asp:Literal runat="server" ID="litDate" Text='<%# Eval("dtInclusao") %>' />
                                                            </p>

                                                            <b>
                                                                <asp:Literal runat="server" ID="litRemetente" Text='<%# string.Format("Remetente: {0}", Eval("sDscUsuarioRemetente")) %>' /></b>

                                                            <p>
                                                                <asp:Literal runat="server" ID="litDestinatario" Text='<%# string.Format("Destinatário: {0}", string.IsNullOrEmpty(Eval("sDscUsuarioDestino").ToString().Trim()) ? Eval("sDscDepartamento") : Eval("sDscUsuarioDestino")) %>' />
                                                            </p>

                                                            <asp:Label class="corpo-heading-mensagem" runat="server" Text='<%# string.IsNullOrEmpty(Eval("sCorpo").ToString()) ? "" : "Corpo:" %>'></asp:Label>
                                                            <label class="card corpo-mensagem" style="padding: 10px;"><asp:Literal runat="server" ID="litCorpo" ClientIDMode="Static" /></label>
                                                            
                                                    </a>
                                                        </div>

                                                </div>
                                            </div>

                                        </ItemTemplate>
                                    </asp:Repeater>

                                </div>
                            </div>

                            <asp:Panel ID="Panel2" class="mt-3" runat="server">
                                <uc1:MensagemPagina runat="server" ID="MensagemPagina3" />
                            </asp:Panel>

                        </div>

                    </div>
                </div>

            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
