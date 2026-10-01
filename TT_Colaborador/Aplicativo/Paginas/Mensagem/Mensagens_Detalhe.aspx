<%@ Page Title="" Language="C#" ValidateRequest="false" MasterPageFile="~/site.master" AutoEventWireup="true" CodeBehind="Mensagens_Detalhe.aspx.cs" Inherits="TT_Colaborador.Aplicativo.Paginas.Mensagem.Mensagens_Detalhe" %>

<%@ Register Src="~/Aplicativo/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .note-editor {
            border: 1px solid #ccc;
            background-color: #f5f5f5;
        }

        .note-editing-area {
            background-color: white;
        }

        .note-toolbar {
            background-color: #f5f5f5;
        }

        /*----------------------------------------------------------------*/

        /* Estilos para Dispositivos Móveis */

        @media only screen and (max-device-width: 768px) {

            .note-editable {
                font-size: 2rem !important;
            }
        }

        /*----------------------------------------------------------------*/
    </style>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <div id="hdd">
                <asp:HiddenField ID="hddidMensagens" runat="server" />
            </div>

            <div class="col-12">
                <div class="card">
                    <div class="card-header card-tt">
                        <h3 class="card-title" style="color: white"><b>
                            <asp:Label ID="lblTituloPagina" runat="server"></asp:Label></b></h1>
                        </h3>
                    </div>

                    <div class="card-body">
                        <div class="col-lg-12" style="padding: 0;">

                            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                            <div class="col-lg-6">
                                <div class="form-group">
                                    <b>
                                        <label>De:</label></b>
                                    <asp:TextBox ID="txtidUsuarioRemetente" class="form-control CaixaTextoMedio" runat="server" disabled="0"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-12" style="padding: 0; display: flex; flex-wrap: wrap;" runat="server" id="DIV_Destinatario_GRUPO">

                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <b>
                                            <label>Para:</label></b>
                                        <asp:DropDownList ID="ddlidDepartamento" runat="server" class="form-select Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidDepartamento_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-6" runat="server" id="DIV_USUARIOS">
                                    <div class="form-group">
                                        <b>
                                            <label>Usuário</label></b>
                                        <asp:DropDownList ID="ddlUsuarios" runat="server" class="form-select Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <b><label>É Aviso?</label></b>
                                        <asp:DropDownList ID="ddlIsAviso" runat="server" class="form-select">
                                            <asp:ListItem Text="Não" Value="N"></asp:ListItem>
                                            <asp:ListItem Text="Sim" Value="S"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                            </div>

                            <div class="col-lg-6" runat="server" id="DIV_Destinatario_USUARIO">
                                <div class="form-group">
                                    <b>
                                        <label>Para</label></b>
                                    <asp:TextBox ID="txtsDscUsuarioDestino" class="form-control CaixaTextoMedio" runat="server" disabled="0"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-12">
                                <div class="form-group">
                                    <b>
                                        <label>Assunto</label></b>
                                    <asp:TextBox ID="txtsAssunto" class="form-control" runat="server" MaxLength="50"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-12">
                                <div class="form-group">
                                    <b>
                                        <label>Corpo</label></b>
                                    <asp:TextBox ID="txtsCorpo" runat="server" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-12">
                                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                            </div>

                            <div class="col-lg-12">
                                <fieldset class="form-stacked actions">
                                    <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                                    <a type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" href="Mensagens?sIsAviso=T">Voltar</a>
                                </fieldset>
                            </div>

                        </div>
                    </div>

                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
